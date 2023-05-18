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
    /// 研削 NTSV用
    /// </summary>
    class Grinder : MachineBase
    {
		/// <summary>
		/// ローダー側マガジンNo読取完了アドレス(プログラム内では狙い厚み書込み完了信号(ARMSが開始登録する際のトリガ)、
		/// 兼装置への動作再開信号通知アドレス(ARMSの開始登録完了後の動作再開トリガ)として使用するが、
		/// ARMSの開始登録のトリガで採用されることの多いアドレス名なので、この変数を採用した）
		/// </summary>
		public string LoaderQRReadCompleteBitAddress { get; set; }
        public string LoaderRingLabelAddress { get; set; }
        public string NotifyRingLabelNGAddress { get; set; }
        public string UnLoaderQRReadCompleteBitAddress { get; set; }

        private const int THICKNESS_RANK_WRITE_COMPLETE_VAL = 2;
        private const string ADDRESS_HEADER = "ZR";
        private const int DATAMATRIX_WORD_LENGTH = 20;
        private const int MAGAZINE_WORD_LENGTH = 20;
        private const string PLC_STAGENO_ADDR = "ZR01AEF8";

        protected override void concreteThreadWork()
        {
            try
            {
                workStart();

                workComplete();
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
        /// NTSVは基板開始時
        /// DMCはリング基板開始時
        /// </summary>
        private void workStart()
        {
			try
			{
				if (this.Plc == null) return;

				//狙い厚み書込要求ビットの確認
				if (IsRegistrableWorkStart() == false)
				{
					return;
				}

				Log.ApiLog.Info("Grinder装置 START BEGIN:" + this.MacNo);

				//基板DM
				string ringLabelStr = this.Plc.GetWord(LoaderRingLabelAddress, DATAMATRIX_WORD_LENGTH).Trim('\r','\0');

                //データがリングか基板かを要素数で判断
                bool isSubstrate = false;
                string[] ringLabelElement = ringLabelStr.Split(' ');
                if(ringLabelElement.Count() == 1)
                {
                    isSubstrate = true;
                }

                string lotno = string.Empty;

				//研削用リングと基板/角リング紐付けテーブルTnLotCarrier、もしくはTnCassetteからアッセンロットを取得
                if(isSubstrate == true)
                {
                    lotno = LotCarrier.GetLotNo(ringLabelStr, true)[0];
                }
                else
                {
                    lotno = LotCarrier.GetLotNoFromRingNo(ringLabelStr);
                }

				Magazine.CheckIdentifyData(lotno);
				Magazine mag = Magazine.GetMagazine(lotno, true)[0];

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
				if (!isError)
				{
					//TODO 既に過去データがある場合は上書きしないと仕様書にあるので、そのチェックを入れる
					Order[] orderArray = Order.SearchOrder(lot.NascaLotNo, p.ProcNo, null, true, false);

					if (orderArray == null || orderArray.Length == 0)
					{
                        order.DeleteInsert(order.LotNo);
                    }

					NotifyStartableSignalToMachine();
				}
				else
				{
                    // ダミー実績を登録した際は、削除 + 巻き戻し
                    if (insertDummyTranFg == true)
                    {
                        Order.ReturnDummyWork(mag, dummyprocno, nowprocno);
                    }
                    //装置停止要求をON
                    this.Plc.SetWordAsDecimalData(NotifyRingLabelNGAddress, 1);

					Log.ApiLog.Info(string.Format("[{0}] Grinder装置 START ERROR {1}", this.MacNo, errMsg));
				}

				Log.ApiLog.Info("Grinder装置 START COMPLETE:" + this.MacNo);
			}
			catch (Exception ex)
			{
				//装置停止要求をON
				this.Plc.SetWordAsDecimalData(NotifyRingLabelNGAddress, 1);

				Log.ApiLog.Error("Grinder装置 START ERROR" + this.MacNo + "ERROR:", ex);
				throw ex;
			}
		}

        public bool IsRegistrableWorkStart()
        {
            decimal retv = this.Plc.GetWordAsDecimalData(LoaderQRReadCompleteBitAddress);
            if (retv == THICKNESS_RANK_WRITE_COMPLETE_VAL)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		public void NotifyStartableSignalToMachine()
		{
			//狙い厚み書込完了ビットを1にする。開始登録完了後の装置動作再開の為のトリガ
			this.Plc.SetWordAsDecimalData(LoaderQRReadCompleteBitAddress, 1);
		}

        /// <summary>
        /// 基板完了時
        /// </summary>
        private void workComplete()
        {
            if (this.Plc == null) return;

			// 製品情報書込通知ビットの確認
			if (IsStartableWorkComplete() == false)
			{
				return;
			}

			Log.ApiLog.Info("Grinder装置 END BEGIN:" + this.MacNo);

			// 基板DM
			string dm = this.Plc.GetWord(LoaderRingLabelAddress, DATAMATRIX_WORD_LENGTH).Trim('\r', '\0');
            if (string.IsNullOrEmpty(dm) == true)
            {
                throw new ApplicationException("読み込んだデータマトリックスが空の為、作業完了登録ができません。");
            }
            
            string lotNo = string.Empty;

            // 研削用リングと基板/角リング紐付けテーブルTnLotCarrier、もしくはTnCassetteからアッセンロットを取得
            bool isSubstrate = HasSubstrate(dm);
            if (isSubstrate == true)
            {
                lotNo = LotCarrier.GetLotNo(dm, true)[0];
            }
            else
            {
                lotNo = LotCarrier.GetLotNoFromRingNo(dm);
            }
  
            // 完了処理時はデータが対象データが無ければ処理スル―。（装置信号が落ちなくて2回目の処理に入るときの考慮）
            if (string.IsNullOrWhiteSpace(lotNo) == true) return;

			Magazine.CheckIdentifyData(lotNo);

			Magazine mag = Magazine.GetMagazine(lotNo, true)[0];

            bool isRenameSuccess = false;

            RenameLotFiles(mag, out isRenameSuccess);

            if (isRenameSuccess == false) return;

            Log.SysLog.Info("Grinder装置 ファイルリネーム完了:" + this.MacNo);

            Order[] orders = Order.SearchOrder(mag.NascaLotNO, null, null, this.MacNo, false, false, null, null, null, null);
			if (orders.Length == 0)
			{
				throw new ApplicationException("作業開始レコードが見つかりません lotno:" + mag.NascaLotNO + " 装置:" + this.MacNo.ToString());
			}
			Order order = orders.FirstOrDefault();


			VirtualMag existingVirtualMag = VirtualMag.GetLastTailMagazine(this.MacNo, Station.Unloader);

			if (existingVirtualMag != null && existingVirtualMag.WorkComplete < DateTime.Now)
			{
				existingVirtualMag.WorkComplete = DateTime.Now;
				existingVirtualMag.Updatequeue();
			}
			else
			{
				VirtualMag newMagazine = new VirtualMag();

				if (string.IsNullOrEmpty(mag.NascaLotNO) == false)
				{
					newMagazine.MagazineNo = mag.NascaLotNO;
				}
				else
				{
					Log.RBLog.Info("Grinder装置ロットNOの取得に失敗");
					return;
				}

				newMagazine.LastMagazineNo = order.InMagazineNo;

				// 作業IDを取得
				newMagazine.ProcNo = Order.GetLastProcNo(this.MacNo, mag.NascaLotNO);
				newMagazine.WorkComplete = DateTime.Now;
				newMagazine.WorkStart = order.WorkStartDt;

				this.Enqueue(newMagazine, Station.Unloader);
			}

            // ステージ位置情報をデータベースに記録
            RegisterStageNo(order, dm);

            if (isSubstrate == false)
            {
                LotCarrier.DeleteRingNo(dm, lotNo);
            }

            
			Log.ApiLog.Info("Grinder装置 END COMPLETE:" + this.MacNo);
		}

		public bool IsStartableWorkComplete()
		{
            decimal retv = this.Plc.GetWordAsDecimalData(UnLoaderQRReadCompleteBitAddress);
            if (retv.ToString() == PLC.Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// EICSが作成したファイル名のリネーム
        /// </summary>
        public void RenameLotFiles(Magazine svmag, out bool successed)
        {
            AsmLot lot = AsmLot.GetAsmLot(svmag.NascaLotNO);
            successed = false;

            Order[] orders = Order.SearchOrder(svmag.NascaLotNO, null, null, this.MacNo, false, false, null, null, null, null);
            if (orders.Length == 0)
            {
                return;
            }
            Order order = orders.FirstOrDefault();

            List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, order.WorkStartDt, DateTime.Now);

            if (lotFiles.Count == 0)
            {
                return;
            }

            if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath, true, false) == false)
            {
                return;
            }

            foreach (string lotFile in lotFiles)
            {
                string dataMatrix = System.IO.Path.GetFileNameWithoutExtension(lotFile);

                //リネイム済みは除外
                if (dataMatrix.Split('_').Length >= MachineLog.FINISHED_RENAME_ELEMENT_NUM)
                {
                    continue;
                }

                MachineLog.ChangeFileName(lotFile, svmag.NascaLotNO, lot.TypeCd, order.ProcNo, svmag.MagazineNo);
                OutputSysLog(string.Format("[完了処理] ファイル名称変更 FileName:{0}", lotFile));

            }

            successed = true;
        }

        private void RegisterStageNo(Order order, string dataMatrix)
        {
            CarrireWorkData regData = new CarrireWorkData();
            regData.LotNo = order.LotNo;
            regData.ProcNo = order.ProcNo;
            regData.Infoid = CarrireWorkData.STAGENO_INFOCD;
            regData.CarrierNo = dataMatrix;

            decimal retv = this.Plc.GetWordAsDecimalData(PLC_STAGENO_ADDR);
            regData.Value = Convert.ToString(retv);

            regData.InsertUpdate();
        }

        public bool HasSubstrate(string dm)
        {
            //読込データの要素数で基板かリングかを判断
            string[] dmElement = dm.Split(' ');
            if (dmElement.Count() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
