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
    /// ホットプレス装置 NTSV用
    /// </summary>
    public class HotPress : CifsMachineBase
    {
        /// <summary>
        /// データ受付準備OK(LDマガジンID)
        /// </summary>
        private const string STR_ADDRESS_DATA_ACCEPT_OK_MAGAZINE = "EM50001";

		/// <summary>
		/// データ受付準備OK(製品出来栄え)
		/// </summary>
		private const string STR_ADDRESS_DATA_ACCEPT_OK_PRODUCT_QUALITY = "EM50002";

		/// <summary>
		/// LDマガジンID装置停止要求(0：継続 1：停止)
		/// </summary>
		private const string STR_ADDRESS_REQUEST_STOP_MAGAZINE = "EM50021";

		/// <summary>
		/// 製品出来栄え装置停止要求(0：継続 1：停止)
		/// </summary>
		private const string STR_ADDRESS_REQUEST_STOP_PRODUCT_QUALITY = "EM50022";

		/// <summary>
		/// データ確認要求(LDマガジンID)
		/// </summary>
		private const string STR_ADDRESS_DATA_CHECK_REQUEST_MAGAZINE = "EM50051";

        /// <summary>
        /// LDマガジン
        /// </summary>
        private const string STR_ADDRESS_LD_MAGAZINE = "EM51000";

        /// <summary>
        /// ULDマガジン
        /// </summary>
        private const string STR_ADDRESS_ULD_MAGAZINE = "EM51500";

        /// <summary>
        /// データ確認要求(SM_製品出来栄え)
        /// </summary>
        private const string STR_ADDRESS_DATA_CHECK_PRODUCT_QUALITY = "EM50052";

        /// <summary>
        /// マガジンの文字数
        /// </summary>
        private const int MAGAZINE_LENGTH = 10;

        protected override void concreteThreadWork()
        {
            try
            {
                //データ受付準備OK(LDマガジンID)ON (ハートビート)
                this.Plc.SetBit(STR_ADDRESS_DATA_ACCEPT_OK_MAGAZINE, 1, Keyence.BIT_ON);
				this.Plc.SetBit(STR_ADDRESS_DATA_ACCEPT_OK_PRODUCT_QUALITY, 1, Keyence.BIT_ON);

                if (this.IsAutoLine || this.IsAgvLine)
                {
                    workComplete();
                }
                else
                {
                    magazineComplete();
                }

                workStart();
                
                if (IsConveyor)
                {
                    base.StartMagazineMoveStatusUpdate();
                }

                //workComlpeteForEICS();
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

        /// <summary>
        /// 作業開始登録
        /// </summary>
        private void workStart()
        {
            //開始処理（NASCA自動登録）
            try
            {
                if (!isRequestMagazine())
                {
                    return;
                }

                Log.ApiLog.Info("HotPress装置 START BEGIN:" + this.MacNo);

                string mgno = this.Plc.GetMagazineNo(STR_ADDRESS_LD_MAGAZINE, MAGAZINE_LENGTH);

                Magazine mag = Magazine.GetCurrent(mgno);
                if (mag == null)
                {
                    throw new Exception(string.Format("[{0}] 稼働中ではないマガジンです。magazine:{1}", this.MacNo, mgno));
                }

                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
                MachineInfo machine = MachineInfo.GetMachine(this.MacNo);

                // この時点での現在完了工程と次作業( = ダミー実績候補の作業)を記録 → 作業規制NG時のダミー実績削除処理に使用
                int nowprocno = mag.NowCompProcess;
                int dummyprocno = p.ProcNo;
                // 次作業のダミー実績登録判定 + 判定OK時にダミー実績登録
                bool insertDummyTranFg = dummyTranCheckAndInsert(lot, mag, ref p);

                Order order = new Order();
                order.LotNo = mag.NascaLotNO;
                order.ProcNo = p.ProcNo;
                order.InMagazineNo = mag.MagazineNo;
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
                //if (!isError)
                //{
                //    if (this.IsAutoLine || this.IsAgvLine)
                //    {
                //        // Loader仮想マガジンの開始時刻を更新
                //        VirtualMag lMagazine = this.Peek(Station.Loader);
                //        if (lMagazine != null)
                //        {
                //            if (lMagazine.MagazineNo == mgno)
                //            {
                //                lMagazine.WorkStart = order.WorkStartDt;
                //                lMagazine.Updatequeue();
                //            }
                //            else
                //            {
                //                errMsg = $"Loader仮想マガジンの先頭マガジンのマガジンNoと一致しません。投入マガジン：『{mgno}』,仮想マガジンの先頭マガジン：『{lMagazine.MagazineNo}』";
                //                isError = true;
                //            }
                //        }
                //        else
                //        {
                //            errMsg = "Loader仮想マガジンがありません。";
                //            isError = true;
                //        }
                //    }
                //}
                if (!isError)
                {
                    order.DeleteInsert(order.LotNo);

                    if (this.IsAutoLine || this.IsAgvLine)
                    {
                        // Loader仮想マガジンを生成
                        VirtualMag lMagazine = new VirtualMag();
                        lMagazine.MagazineNo = order.InMagazineNo;
                        lMagazine.WorkStart = order.WorkStartDt;
                        lMagazine.ProcNo = order.ProcNo;
                        this.Enqueue(lMagazine, Station.Loader);
                    }

                    //装置停止要求をOFF
                    this.Plc.SetBit(STR_ADDRESS_REQUEST_STOP_MAGAZINE, 1, Keyence.BIT_OFF);

                    //マガジンOFF
                    this.Plc.SetBit(STR_ADDRESS_DATA_CHECK_REQUEST_MAGAZINE, 1, Keyence.BIT_OFF);
                }
                else
                {
                    // ダミー実績を登録した際は、削除 + 巻き戻し
                    if (insertDummyTranFg == true)
                    {
                        Order.ReturnDummyWork(mag, dummyprocno, nowprocno);
                    }
                    //装置停止要求をON
                    this.Plc.SetBit(STR_ADDRESS_REQUEST_STOP_MAGAZINE, 1, Keyence.BIT_ON);

                    Log.ApiLog.Info(string.Format("[{0}] HotPress装置 START ERROR {1}", this.MacNo, errMsg));
                }

                Log.ApiLog.Info("HotPress装置 START COMPLETE:" + this.MacNo);
            }
            catch (Exception ex)
            {
                //装置停止要求をON
                this.Plc.SetBit(STR_ADDRESS_REQUEST_STOP_MAGAZINE, 1, Keyence.BIT_ON);

                Log.ApiLog.Error("HotPress装置 WS" + this.MacNo + "ERROR:", ex);
                throw ex;
            }
        }

        /// <summary>
        /// 作業完了(自動搬送)
        /// </summary>
        private void workComplete()
        {
            if (!isCheckProductQuality())
            {
                return;
            }

            try
            {
                //VirtualMag ulMagazine = this.Peek(Station.Unloader);
                //if (ulMagazine != null)
                //{
                //    return;
                //}

                VirtualMag lMagazine = this.Peek(Station.Loader);
                if (lMagazine == null)
                {
                    return;
                }

                this.OutputSysLog("[完了処理] 開始");

                //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
                string newmagno = this.Plc.GetMagazineNo(STR_ADDRESS_ULD_MAGAZINE, MAGAZINE_LENGTH);
                if (string.IsNullOrEmpty(newmagno) == false)
                {
                    lMagazine.LastMagazineNo = lMagazine.MagazineNo;
                    lMagazine.MagazineNo = newmagno;
                }
                else
                {
                    this.OutputSysLog("QR読取のマガジンNO取得に失敗");
                    return;
                }

                Magazine svmag = Magazine.GetCurrent(newmagno);

                this.OutputSysLog($"[完了処理] ロットNoの取得成功 ロットNo:『{svmag.NascaLotNO}』");

                Order[] orders = Order.SearchOrder(svmag.NascaLotNO, null, null, this.MacNo, false, false, null, null, null, null);
                if (orders.Length == 0)
                {
                    throw new ApplicationException("作業開始レコードが見つかりません mag:" + newmagno + " 装置:" + this.MacNo.ToString());
                }
                Order order = orders.FirstOrDefault();

                //作業IDを取得
                lMagazine.ProcNo = Order.GetLastProcNo(this.MacNo, newmagno);
                
                lMagazine.WorkComplete = DateTime.Now;

                if (this.Enqueue(lMagazine, Station.Unloader))
                {
                    this.Dequeue(Station.Loader);
                    this.WorkComplete(lMagazine, this, true);
                }
                this.Plc.SetBit(STR_ADDRESS_DATA_CHECK_PRODUCT_QUALITY, 1, Keyence.BIT_OFF);
                this.OutputSysLog("[完了処理] 完了");

            }
            catch (Exception err)
            {
                this.Plc.SetBit(STR_ADDRESS_REQUEST_STOP_PRODUCT_QUALITY, 1, Keyence.BIT_ON);
                throw new ApplicationException(string.Format("マガジン完了時に例外発生：{0}", err.ToString()));
            }
        }

        private bool isRequestMagazine()
        {
            if (this.Plc == null) return false;

            string retv = this.Plc.GetBit(STR_ADDRESS_DATA_CHECK_REQUEST_MAGAZINE);

            if (Convert.ToBoolean(Convert.ToInt32(retv)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// マガジン完了
        /// </summary>
        private void magazineComplete()
        {
            if (!isCheckProductQuality())
            {
                return;
            }

			try
			{
				Log.RBLog.Info("HotPress装置");

				//キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
				string newmagno = this.Plc.GetMagazineNo(STR_ADDRESS_ULD_MAGAZINE, MAGAZINE_LENGTH);

				Magazine svmag = Magazine.GetCurrent(newmagno);

				Order[] orders = Order.SearchOrder(svmag.NascaLotNO, null, null, this.MacNo, false, false, null, null, null, null);
				if (orders.Length == 0)
				{
					throw new ApplicationException("作業開始レコードが見つかりません mag:" + newmagno + " 装置:" + this.MacNo.ToString());
				}
				Order order = orders.FirstOrDefault();

				VirtualMag newMagazine = new VirtualMag();

				if (string.IsNullOrEmpty(newmagno) == false)
				{
					newMagazine.MagazineNo = newmagno;
				}
				else
				{
					Log.RBLog.Info("HotPress装置マガジンNOの取得に失敗");
					return;
				}
				newMagazine.LastMagazineNo = order.InMagazineNo;

				//作業IDを取得
				newMagazine.ProcNo = Order.GetLastProcNo(this.MacNo, newmagno);

				//開始・完了時間をPLCから取得
				newMagazine.WorkComplete = DateTime.Now;
				newMagazine.WorkStart = order.WorkStartDt;

				this.Enqueue(newMagazine, Station.Unloader);
				this.Plc.SetBit(STR_ADDRESS_DATA_CHECK_PRODUCT_QUALITY, 1, Keyence.BIT_OFF);

			}
			catch (Exception err)
			{
				this.Plc.SetBit(STR_ADDRESS_REQUEST_STOP_PRODUCT_QUALITY, 1, Keyence.BIT_ON);
				throw new ApplicationException(string.Format("マガジン完了時に例外発生：{0}", err.ToString()));
			}

		}

        private bool isCheckProductQuality()
        {
            if (this.Plc == null) return false;

            string retv = this.Plc.GetBit(STR_ADDRESS_DATA_CHECK_PRODUCT_QUALITY);
			
            if (Convert.ToBoolean(Convert.ToInt32(retv)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// EICSが作成したファイル名のリネーム
        /// </summary>
        private void workComlpeteForEICS()
        {
            string newmagno = this.Plc.GetMagazineNo(STR_ADDRESS_ULD_MAGAZINE, MAGAZINE_LENGTH);

			if (string.IsNullOrWhiteSpace(newmagno))
			{
				return;
			}

			Magazine svmag = Magazine.GetCurrent(newmagno);

            AsmLot lot = AsmLot.GetAsmLot(svmag.NascaLotNO);

            Order[] orders = Order.SearchOrder(svmag.NascaLotNO, null, null, this.MacNo, false, false, null, null, null, null);
            if (orders.Length == 0)
            {
                return;
            }
            Order order = orders.FirstOrDefault();

            List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, order.WorkStartDt, DateTime.Now);
            foreach (string lotFile in lotFiles)
            {
                string fileNm = System.IO.Path.GetFileNameWithoutExtension(lotFile);

                //リネイム済みは除外
                if (fileNm.Split('_').Length >= MachineLog.FINISHED_RENAME_ELEMENT_NUM)
                {
                    continue;
                }

                MachineLog.ChangeFileName(lotFile, svmag.NascaLotNO, lot.TypeCd, order.ProcNo, newmagno);
                OutputSysLog(string.Format("[完了処理] ファイル名称変更 FileName:{0}", lotFile));
            }
        }
        
        /// <summary>
        /// 投入要求確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireInput()
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            string retv = Mitsubishi.BIT_OFF;
            if (this.IsConveyor)
            {
                return this.IsRequireConveyorInput(this.CarrierPlc);
            }
            else
            {
                if (string.IsNullOrEmpty(this.LoaderReqBitAddress) == true)
                {
                    return false;
                }

                try
                {
                    retv = Plc.GetBit(this.LoaderReqBitAddress);
                    if (retv == Mitsubishi.BIT_ON)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{this.LoaderReqBitAddress}』, エラー内容：{ex.Message}");
                    return false;
                }
            }
        }
    }
}
