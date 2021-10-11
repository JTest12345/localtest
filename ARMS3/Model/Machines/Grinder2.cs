using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// Grinder(1基板づつ研削)とは違う4基板づつ研削する型式
    /// </summary>
    class Grinder2 : Grinder
    {
        /// <summary>
        /// 研削する4基板分のDM読取アドレス
        /// </summary>
        private List<string> workingSubstrateDmPlcAddressList
        {
            get { return new List<string> { "ZR0186C8", "ZR0186D2", "ZR0186E8", "ZR0186F2" }; }
        }
        
        public const string StageNo_PlcAddress = "ZR01AEF8";
        public const int DatamatrixWordLength = 10;

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
        /// 4基板開始時
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
                string dm = this.Plc.GetWord(LoaderRingLabelAddress, DatamatrixWordLength).Trim('\r', '\0');
                if (string.IsNullOrEmpty(dm) == true)
                {
                    throw new ApplicationException("読み込んだデータマトリックスが空の為、作業開始登録ができません。");
                }

                string lotno = string.Empty;

                //研削用リングと基板/角リング紐付けテーブルTnLotCarrier、もしくはTnCassetteからアッセンロットを取得
                bool isSubstrate = HasSubstrate(dm);
                if (isSubstrate == true)
                {
                    lotno = LotCarrier.GetLotNo(dm, true)[0];
                }
                else
                {
                    lotno = LotCarrier.GetLotNoFromRingNo(dm);
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

        /// <summary>
        /// 4基板完了時
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
            string dm = this.Plc.GetWord(LoaderRingLabelAddress, DatamatrixWordLength).Trim('\r', '\0');
            if (string.IsNullOrEmpty(dm) == true)
            {
                throw new ApplicationException("読み込んだデータマトリックスが空の為、作業完了登録ができません。");
            }

            // データがリングか基板かを要素数で判断
            bool isSubstrate = HasSubstrate(dm);

            string lotNo = string.Empty;

            // 研削用リングと基板/角リング紐付けテーブルTnLotCarrier、もしくはTnCassetteからアッセンロットを取得
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

            List<string> substrateList = getWorkingSubstrateDm();
            int stage = getStage();

            // トレース用に作業情報をデータベースに記録
            saveCarrierWork(order, substrateList, stage);

            if (isSubstrate == false)
            {
                LotCarrier.DeleteRingNo(dm, lotNo);
            }

            Log.ApiLog.Info("Grinder装置 END COMPLETE:" + this.MacNo);
        }

        /// <summary>
        /// 作業中の基板DM取得
        /// </summary>
        /// <returns></returns>
        private List<string> getWorkingSubstrateDm()
        {
            List<string> retv = new List<string>();

            foreach (string address in workingSubstrateDmPlcAddressList)
            {
                string dm = this.Plc.GetWord(address, DatamatrixWordLength).Trim('\r', '\0');
                if (string.IsNullOrEmpty(dm) == false)
                {
                    retv.Add(dm);
                }
            }

            return retv;
        }

        /// <summary>
        /// 装置内ステージ位置取得
        /// </summary>
        /// <returns></returns>
        private int getStage()
        {
            int retv = this.Plc.GetWordAsDecimalData(StageNo_PlcAddress);
            return retv;
        }

        /// <summary>
        /// トレース用作業記録(ロット、基板と使用マガジン段数、ステージの記録)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="substrateList"></param>
        /// <param name="stage"></param>
        public void saveCarrierWork(Order order, List<string> substrateList, int stage)
        {
            // 同ロットで先に登録した記録がある場合、マガジン段数をその続きにする必要がある為、登録段数の最大を取得する
            int magStepMax = 0;
            List<CarrireWorkData> carrireWorkList = CarrireWorkData.GetDataFromLot(order.LotNo);
            carrireWorkList = carrireWorkList.Where(c => c.Infoid == CarrireWorkData.MAGAZINE_STEP_INFOCD).ToList();
            if (carrireWorkList.Any())
            {
                magStepMax = carrireWorkList.Max(c => int.Parse(c.Value));
            }
                 
            // マガジン段数と使用したロット、基板の記録       
            int magStep = magStepMax + 1;
            foreach (string substrate in substrateList)
            {
                // 既に登録済は処理無し
                if (CarrireWorkData.Exists(substrate, order.ProcNo, CarrireWorkData.MAGAZINE_STEP_INFOCD) == true)
                    continue;
              
                CarrireWorkData d = new CarrireWorkData();
                d.LotNo = order.LotNo;
                d.ProcNo = order.ProcNo;
                d.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;
                d.CarrierNo = substrate;
                d.Value = Convert.ToString(magStep);
                d.InsertUpdate();

                magStep = magStep + 1;
            }

            // ステージと使用したロット、基板の記録       
            foreach (string substrate in substrateList)
            {
                // 既に登録済は処理無し
                if (CarrireWorkData.Exists(substrate, order.ProcNo, CarrireWorkData.STAGENO_INFOCD) == true)
                    continue;
                
                CarrireWorkData d = new CarrireWorkData();
                d.LotNo = order.LotNo;
                d.ProcNo = order.ProcNo;
                d.Infoid = CarrireWorkData.STAGENO_INFOCD;
                d.CarrierNo = substrate;
                d.Value = Convert.ToString(stage);
                d.InsertUpdate();
            }
        }
    }
}
