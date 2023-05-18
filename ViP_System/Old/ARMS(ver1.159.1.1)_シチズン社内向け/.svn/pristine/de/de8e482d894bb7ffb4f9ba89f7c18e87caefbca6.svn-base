using ARMS3.Model.PLC;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    public class Sputter2 : Sputter
    {
        private class CompleteLot
        {
            /// <summary>
            /// 装置から完了実績をリストで抜く用のプロパティ
            /// </summary>
            public string completeMagazineNo { get; set; }
            public DateTime completeWorkTime { get; set; }
        }

        /// <summary>
        /// 開始登録判定アドレス (OK=1、NG=2)
        /// </summary>
        public string workStartJudgeAddress { get; set; }

        
        /// <summary>
        /// 開始登録用定数
        private const int JUDGE_OK = 1;
        private const int JUDGE_NG = 2;
        private const int MAGAZINE_LENGTH = 9;

        /// <summary>
        /// 出来栄えデータの1レコード毎のシフト量
        /// </summary>
        private const int WORK_DATA_ADDRS_HIST_QTY = 20;

        protected override void concreteThreadWork()
        {
            try
            { 

                if (IsRequireOutput())
                {
                    workComplete();
                }
            
                if (IsLoaderQRReadComplete())
                {
                    workStart();
                }
            }
            catch (Exception ex)
            {
                FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
                frmErr.ShowDialog();

                if (frmErr.Method == ErrorHandleMethod.None)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        private void workComplete()
        {


            //排出データの取得
            CompleteLot completeLot = getLatestCompleteLot();
            
            if(completeLot == null)
            {
                throw new ApplicationException("有効な完了実績が取得できませんでした。");
            }

            VirtualMag newMagazine = new VirtualMag();
                        
            Magazine svrmag = Magazine.GetCurrent(completeLot.completeMagazineNo);
            if (svrmag == null)
            {
                throw new ApplicationException($"稼動中ﾏｶﾞｼﾞﾝが存在しません。MagazineNo:{completeLot.completeMagazineNo}");
            }

            Order[] orders = Order.SearchOrder(svrmag.NascaLotNO, null, null, this.MacNo, false, false, null, null, null, null);
            if (orders.Length == 0)
            {
                throw new ApplicationException("作業開始レコードが見つかりません lotno:" + svrmag.NascaLotNO + " 装置:" + this.MacNo.ToString());
            }
            Order order = orders.FirstOrDefault();


            newMagazine.MagazineNo = completeLot.completeMagazineNo;
            newMagazine.LastMagazineNo = completeLot.completeMagazineNo;

            AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            Process p = Process.GetNextProcess(svrmag.NowCompProcess, lot);

            newMagazine.ProcNo = svrmag.NowCompProcess;
            newMagazine.WorkComplete = completeLot.completeWorkTime;
            newMagazine.WorkStart = order.WorkStartDt;
            newMagazine.MacNo = this.MacNo;
            
            VirtualMag vMag = VirtualMag.GetVirtualMag(this.MacNo, (int)Station.Unloader, lot.NascaLotNo);
            if (vMag == null)
            {
                this.Enqueue(newMagazine, Station.Unloader);
            }
            else
            {
                vMag.WorkComplete = newMagazine.WorkComplete;
                vMag.Updatequeue();
                OutputApiLog($"Unloaderに既に同一マガジンが存在するため、完了時間を更新:{vMag.MagazineNo}");
            }

            Plc.SetBit(UnLoaderReqBitAddress, 1, Mitsubishi.BIT_OFF);
        }

        private void workStart()
        {
            try
            {
                string magno = Plc.GetMagazineNo(LoaderMagazineAddress, MAGAZINE_LENGTH);

                if (String.IsNullOrWhiteSpace(magno) == true)
                {
                    throw new Exception($"開始時に装置からマガジン番号が取得できませんでした。設備:{this.MacNo}");
                }

                Magazine mag = Magazine.GetCurrent(magno);
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);

                Magazine svrMag = Magazine.GetMagazine(lot.NascaLotNo);
                if (svrMag == null)
                {
                    throw new Exception(string.Format("稼働中マガジンが存在しない為、開始登録ができません。LotNo:{0}",
                        lot.NascaLotNo));
                }

                Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);
                if (p == null)
                {
                    throw new Exception(string.Format("稼動中マガジンの次作業Noが存在しない為、開始登録ができません。LotNo:{0} 稼動中マガジン完了工程No:{1}",
                        lot.NascaLotNo, svrMag.NowCompProcess));
                }

                //同ロットで他装置での開始実績が有ると開始処理エラーにする。
                List<Order> orderList = ArmsApi.Model.Order.SearchOrder(lot.NascaLotNo, p.ProcNo, null, true, false).ToList();

                if (orderList.Exists(o => o.MacNo != this.MacNo) && orderList.Count > 0)
                {
                    throw new ApplicationException(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo,
                        string.Format("他の装置での開始実績が既に存在します。macno(複数の場合カンマ区切り):{0}", string.Join(",", orderList.Select(o => o.MacNo)))));
                }

                VirtualMag vMag = new VirtualMag();
                vMag.MagazineNo = svrMag.MagazineNo;
                vMag.ProcNo = p.ProcNo;

                Order order = ArmsApi.CommonApi.GetWorkStartOrder(vMag, this.MacNo);

                // 1基板目の開始時間をマガジンの開始時間とする為、既に開始実績が存在する場合(2基板目以降)は登録しない
                Order[] startedOrder = Order.GetOrder(lot.NascaLotNo, p.ProcNo);

                if (startedOrder.Count() == 0)
                {
                    ArmsApiResponse workResponse = ArmsApi.CommonApi.WorkStart(order);
                    if (workResponse.IsError)
                    {
                        ArmsApi.Log.ApiLog.Info(string.Format("[{0}] スパッタ START ERROR {1}", this.MacNo, workResponse.Message));
                        Plc.SetWordAsDecimalData(workStartJudgeAddress, JUDGE_NG);
                        return;
                    }
                }
                else
                {
                    //既に履歴がある場合でも開始登録チェックは毎基板実施する
                    MachineInfo machine = MachineInfo.GetMachine(this.MacNo);
                    order.LotNo = lot.NascaLotNo;
                    order.ProcNo = p.ProcNo;
                    order.InMagazineNo = svrMag.MagazineNo;
                    order.MacNo = this.MacNo;
                    order.WorkStartDt = DateTime.Now;
                    order.WorkEndDt = null;

                    string errMsg;
                    bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, p, out errMsg);
                    if (isError)
                    {
                        ArmsApi.Log.ApiLog.Info(string.Format("[{0}] スパッタ START ERROR {1}", this.MacNo, errMsg));
                        Plc.SetWordAsDecimalData(workStartJudgeAddress, JUDGE_NG);
                        return;
                    }
                }

                Plc.SetWordAsDecimalData(workStartJudgeAddress, JUDGE_OK);
            }
            catch(Exception ex)
            {
                Plc.SetWordAsDecimalData(workStartJudgeAddress, JUDGE_NG);
                throw new Exception(ex.ToString(), ex);
            }
            finally
            {
                Plc.SetBit(LoaderQRReadCompleteBitAddress, 1, Mitsubishi.BIT_OFF);
            }
        }

        /// <summary>
        /// 最新10レコードの完了履歴を追って最新の「完了時刻が入っているレコード」を取得。
        /// （途中で投入中断、ないし投入中のレコードは完了時刻が無い）
        /// </summary>
        /// <returns></returns>
        private CompleteLot getLatestCompleteLot()
        {
           CompleteLot retv = new CompleteLot();

            string correntWorkCompleteTimeAddress = WorkCompleteTimeAddress;
            string correntUnLoaderMagazineAddress = UnLoaderMagazineAddress;
            DateTime tempCompleteDt;
            
            for(int i = 0; i < 10; i++)
            {
                if(i != 0)
                {
                    correntWorkCompleteTimeAddress = PLC.Mitsubishi.GetMemAddrAfterAdding(correntWorkCompleteTimeAddress, WORK_DATA_ADDRS_HIST_QTY);
                    correntUnLoaderMagazineAddress = PLC.Mitsubishi.GetMemAddrAfterAdding(correntUnLoaderMagazineAddress, WORK_DATA_ADDRS_HIST_QTY);
                }

                try
                {
                    tempCompleteDt = this.Plc.GetWordsAsDateTime(correntWorkCompleteTimeAddress);
                }
                catch
                {
                    continue; //完了時間が取得できないレコードは無視
                }

                retv = new CompleteLot();

                retv.completeWorkTime = tempCompleteDt;
                retv.completeMagazineNo = Plc.GetMagazineNo(correntUnLoaderMagazineAddress, MAGAZINE_LENGTH);
                break; //データ取得できた時点で抜ける
            }           
            
            return retv;
        }
    }
}
