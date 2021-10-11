using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// SLS1 色調補正機 (内製モールド機と同じ仕様)
	/// </summary>
	public class ColorRevision : MachineBase
	{
		/// <summary>
		/// 開始登録OKBit
		/// </summary>
		public string WorkStartOKBitAddress { get; set; }

		/// <summary>
		/// 開始登録NGBit
		/// </summary>
		public string WorkStartNGBitAddress { get; set; }

		/// <summary>
		/// 完了登録OKBit
		/// </summary>
		public string WorkCompleteOKBitAddress { get; set; }

		/// <summary>
		/// 完了登録NGBit
		/// </summary>
		public string WorkCompleteNGBitAddress { get; set; }

		/// <summary>
		/// 完了登録NG理由
		/// </summary>
		public string WorkCompleteNGReasonAddress { get; set; }

		public string LoaderQRReadCompleteBitAddress { get; set; }

		public string LoaderMagazineAddress { get; set; }

		public string UnLoaderMagazineAddress { get; set; }

        #region 装置ログ関連

        private const int DATA_NO_COL = 0;
        private const int MAG_NO_COL = 3;

        #endregion

        protected override void concreteThreadWork()
		{
			try
			{
				if (this.IsRequireOutput() == true)
				{
					workCompletehigh();
				}

				if (this.IsLoaderQRReadComplete() == true) 
				{
					workStart();
				}

                CheckMachineLogFile();
            }
			catch (Exception ex)
			{
				FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
				frmErr.ShowDialog();

				if (frmErr.Method == ErrorHandleMethod.None)
				{
					throw;
				}
			}
		}

		private void workStart() 
		{
			try
			{
				OutputSysLog("[開始処理] 開始");

				string magno = Plc.GetMagazineNo(this.LoaderMagazineAddress, true);
				Magazine svrMag = Magazine.GetCurrent(magno);
				if (svrMag == null)
				{
					throw new ApplicationException(string.Format("稼働中ではないマガジンです。MagazineNo:{0}", magno));
				}
				OutputSysLog(string.Format("[開始処理] マガジン番号取得成功 LoaderMagazineNo:{0}", svrMag.MagazineNo));

				AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
				Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);
				MachineInfo machine = MachineInfo.GetMachine(this.MacNo);

                // この時点での現在完了工程と次作業( = ダミー実績候補の作業)を記録 → 作業規制NG時のダミー実績削除処理に使用
                int nowprocno = svrMag.NowCompProcess;
                int dummyprocno = p.ProcNo;
                // 次作業のダミー実績登録判定 + 判定OK時にダミー実績登録
                bool insertDummyTranFg = dummyTranCheckAndInsert(lot, svrMag, ref p);

                Order order = new Order();
				order.LotNo = svrMag.NascaLotNO;
				order.ProcNo = p.ProcNo;
				order.InMagazineNo = svrMag.MagazineNo;
				order.MacNo = this.MacNo;
				order.WorkStartDt = DateTime.Now;
				order.WorkEndDt = null;
				order.TranStartEmpCd = "660";
				order.TranCompEmpCd = "660";

				if (p.IsNascaDefect)
				{
					order.IsDefectEnd = false;
				}
				else
				{
					order.IsDefectEnd = true;
				}

				string errMsg;

				bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, p, out errMsg);
				if (isError)
				{
                    // ダミー実績を登録した際は、削除 + 巻き戻し
                    if (insertDummyTranFg == true)
                    {
                        Order.ReturnDummyWork(svrMag, dummyprocno, nowprocno);
                    }
                    throw new ApplicationException(errMsg);
				}
				else
				{
					Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, p.ProcNo);
					if (dst == Process.MagazineDevideStatus.SingleToDouble)
					{
						svrMag.NewFg = false;
						svrMag.Update();

						order.DevidedMagazineSeqNo = 1;
						order.DeleteInsert(order.LotNo);

						svrMag.NascaLotNO = order.LotNo;
						svrMag.MagazineNo = order.LotNo;
						svrMag.NewFg = true;
						svrMag.Update();

						order.DevidedMagazineSeqNo = 2;
						order.DeleteInsert(order.LotNo);

						svrMag.NascaLotNO = order.LotNo;
						svrMag.MagazineNo = order.LotNo;
						svrMag.Update();
					}
					else
					{
						order.DeleteInsert(order.LotNo);
					}

					Plc.SetBit(WorkStartOKBitAddress, 1, PLC.Common.BIT_ON);
					OutputSysLog("[開始処理] 完了");
				}
			}
			catch (Exception)
			{
				//NASCA開始NG理由をPLCに書き込み
				//現在は固定で「1」を書き込み
				Plc.SetBit(WorkCompleteNGReasonAddress, 1, "1");

				//NASCA開始登録NGをPLCに書き込み
				Plc.SetBit(this.WorkStartNGBitAddress, 1, Common.BIT_ON);
				throw;
			}
		}

		private void workCompletehigh()
		{
			try
			{
				//VirtualMag ulMag = this.Peek(Station.Unloader);
				//if (ulMag != null)
				//{
				//	return;
				//}

				VirtualMag lMag = new VirtualMag();

				OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", lMag.MagazineNo));

				lMag.MagazineNo = Plc.GetMagazineNo(this.UnLoaderMagazineAddress, true);
				Magazine svrMag = Magazine.GetCurrent(lMag.MagazineNo);
				if (svrMag == null)
				{
					throw new ApplicationException(string.Format("稼働中ではないマガジンです。MagazineNo:{0}", lMag.MagazineNo));
				}
				OutputSysLog(string.Format("[完了処理] マガジン番号取得成功 UnLoaderMagazineNo:{0}", svrMag.MagazineNo));

				lMag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
				OutputSysLog(string.Format("[完了処理] 開始日時取得成功 StartDt:{0}", lMag.WorkStart.Value));

				lMag.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
				OutputSysLog(string.Format("[完了処理] 完了日時取得成功 EndDt:{0}", lMag.WorkComplete.Value));

				int procno = Order.GetLastProcNo(this.MacNo, svrMag.MagazineNo);
				lMag.ProcNo = procno;

				AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
				OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

				//List<string> lotFiles = MachineLog.GetLotFiles(this.LogOutputDirectoryPath, lMag.WorkStart.Value, lMag.WorkComplete.Value);
				//List<string> lotFiles = MachineLog.GetLotFiles(this.LogOutputDirectoryPath, lMag.WorkStart.Value, System.DateTime.Now);
				List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, lMag.WorkStart.Value, System.DateTime.Now);

                //関数の外側にリネーム関数を用意したので削除。2017.12.11湯浅
				//foreach (string lotFile in lotFiles)
				//{
				//  MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, procno);
				//	OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
				//}

				lMag.LastMagazineNo = lMag.MagazineNo;

				if (this.Enqueue(lMag, Station.Unloader))
				{
					Plc.SetBit(this.WorkCompleteOKBitAddress, 1, Common.BIT_ON);
					OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", lMag.MagazineNo));
				}
			}
			catch (Exception)
			{
				Plc.SetBit(this.WorkCompleteNGBitAddress, 1, Common.BIT_ON);
				throw;
			}
		}

		private bool IsLoaderQRReadComplete() 
		{
			if (string.IsNullOrEmpty(this.LoaderQRReadCompleteBitAddress)) 
			{
				return false;
			}

			if (Plc.GetBit(this.LoaderQRReadCompleteBitAddress) == PLC.Common.BIT_ON)
			{
				return true;
			}
			else 
			{
				return false;
			}
		}

        public void CheckMachineLogFile()
        {
            List<string> logFiles = MachineLog.GetFiles(this.LogOutputDirectoryPath);
            foreach (string logFile in logFiles)
            {
                if (IsRenamedFile(logFile)) continue;

                if (Mold.IsStartLogFile(logFile) == false)
                {
                    MachineLog mLog = Mold.parseMachineLog(logFile);
                    if (mLog.IsUnknownData)
                    {
                        MachineLog.ChangeFileName(logFile, MachineLog.FILE_UNKNOWN, MachineLog.FILE_UNKNOWN, 0, MachineLog.FILE_UNKNOWN);
                        return;
                    }

                    Magazine svrMag = Magazine.GetCurrent(mLog.MagazineNo);
                    if (svrMag == null)
                    {
                        svrMag = Magazine.GetCurrent(mLog.MagazineNo + "_#2");
                    }
                    if (svrMag == null)
                        throw new ApplicationException(
                            string.Format("装置ログ内のマガジンNo:{0}の稼働中マガジンが存在しません。", mLog.MagazineNo));

                    AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                    Order[] o = Order.SearchOrder(svrMag.NascaLotNO, null, this.MacNo, true, false);
                    if (o.Length == 0)
                        throw new ApplicationException(
                            string.Format("装置ログ内のマガジンNo:{0}の稼働中ロット：{1}の作業中実績が存在しません。", mLog.MagazineNo, svrMag.NascaLotNO));
                    int procNo = o[0].ProcNo;

                    MachineLog.ChangeFileName(logFile, lot.NascaLotNo, lot.TypeCd, procNo, svrMag.MagazineNo);
                }

            }
        }
    }
}
