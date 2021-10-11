using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// IPS装置(DMC高生産/外製機)
    /// </summary>
    public class IPS : MachineBase
	{

        protected virtual string Tray2DReadRequestAddress() { return "DM2902"; }
        protected virtual string Tray2DReadJudgeAddress() { return "DM2903"; }
        protected virtual string DischargeRequestAddress() { return "DM2904"; }
        protected virtual string DischargeResponseAddress() { return "DM2905"; }
        protected virtual string WorkStartDtAddress_yy() { return "DM2710"; }
        protected virtual string WorkEndDtAddress_yy() { return "DM2717"; }
        protected virtual string workCompleteDtAddress() { return "DM2807"; }
        protected virtual string Tray2DAddress() { return "DM2700"; }
        protected virtual int Tray_WORD_LEN() { return 10; }

        /*
        //装置スタートタイミング(EICS側に必要)

        //トレイ読み込みタイミング
        private const string    Tray2DReadRequestAddress    = "DM2902"; //トレイ2D読み込み時ON
        private const string    Tray2DReadJudgeAddress      = "DM2903"; //トレイ2D読み込み時の判定結果

        //作業完了タイミング
        private const string    DischargeRequestAddress     = "DM2904"; //作業完了時ON
        private const string    DischargeResponseAddress       = "DM2905"; //作業完了時の判定

        //開始時刻
        private const string WorkStartDtAddress_yy = "DM2710"; //開始時間:年の下2桁 0～99

        //完了時刻
        private const string WorkEndDtAddress_yy = "DM2717"; //開始時間:年の下2桁 0～99

        private const string    Tray2DAddress               = "DM2700";
        private const int       Tray_WORD_LEN               = 7;      //DM2700
        */

        protected override void concreteThreadWork()
        {

            // 作業開始登録
            if (Convert.ToInt32(Plc.GetBit(Tray2DReadRequestAddress())) == 1)
            {
                workStart();
            }

            // 作業完了登録
            if (Convert.ToInt32(Plc.GetBit(DischargeRequestAddress())) == 1)
            {
                workComplete(null);
            }
        }

        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //実マガジンのアンローダー以外は何もしない
            if (station != Station.Unloader)
            {
                return true;
            }

            return base.Enqueue(mag, station);
        }


        public void workStart()
        {
            try
            {
                VirtualMag mag = new VirtualMag();

                //トレイDM
                string dm = Plc.GetString(Tray2DAddress(), Tray_WORD_LEN());
                //dm = "tray1";

                List<string> listlotno = new List<string>();

                if (string.IsNullOrWhiteSpace(dm) == false)
                {
                    listlotno = getLotList(dm);
                }
                else
                {
                    throw new ApplicationException("[開始登録異常] IPS装置搬入マガジンNOの取得に失敗。\nIPS装置搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
                }

                //①TnLotCarrierに該当トレイがあるか確認してロット集計。
                //②TnCassetteにtraynoフィールド追加。上記同様に該当トレイがあるか確認してロット集計。
                List<bool> listng = new List<bool>();

                foreach (string lotno in listlotno)
                {
                    Magazine[] magArray = Magazine.GetMagazine(lotno, true);

                    if (magArray.Length != 1)
                    {
                        throw new ApplicationException("[開始登録異常] マガジン情報が1件に絞れません。ロット:" + lotno);
                    }

                    string magno = magArray[0].MagazineNo;
                    Magazine svrmag = magArray[0];

                    AsmLot svrlot = AsmLot.GetAsmLot(lotno);
                    Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

                    mag.MagazineNo = magno;
                    mag.ProcNo = nextproc.ProcNo;

                    List<Order> orderList = ArmsApi.Model.Order.SearchOrder(lotno, mag.ProcNo, null, true, false).ToList();

                    if (orderList.Exists(o => o.MacNo != this.MacNo) && orderList.Count > 0)
                    {
                        throw new ApplicationException(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo,
                            string.Format("他の装置での開始実績が既に存在します。macno(複数の場合カンマ区切り):{0}", string.Join(",", orderList.Select(o => o.MacNo)))));
                    }

                    //時間取得したら0しか入ってないので、落ちる
                    mag.WorkStart = getPlcDt(WorkStartDtAddress_yy());

                    Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);
                    order.LotNo = svrlot.NascaLotNo;
                    order.TranStartEmpCd = "660";

                    if (orderList.Exists(o => o.MacNo == this.MacNo))
                    {
                        string errMsg;
                        MachineInfo machine = MachineInfo.GetMachine(this.MacNo);
                        bool isError = WorkChecker.IsErrorBeforeStartWork(svrlot, machine, order, nextproc, out errMsg);
                        if (isError)
                        {
                            throw new ApplicationException(string.Format("[開始登録異常] 装置:{0}/TrayNo:{1}/理由:{2}", this.MacNo, dm, errMsg));
                        }
                        else
                        {
                            OutputSysLog(string.Format("[開始処理] 完了 LoaderMagazineNo:{0}/TrayNo:{1}", mag.MagazineNo,dm));
                        }
                    }
                    else
                    {
                        OutputSysLog(string.Format("[開始処理] 開始 MagazineNo:{0}/TrayNo:{1}", magno, dm));
                        ArmsApiResponse workResponse = CommonApi.WorkStart(order);

                        if (workResponse.IsError)
                        {
                            throw new ApplicationException(string.Format("[開始登録異常] 装置:{0}/TrayNo:{1}/理由:{2}", this.MacNo, dm, workResponse.Message));
                        }
                        else
                        {
                            OutputSysLog(string.Format("[開始処理] 完了 LoaderMagazineNo:{0}/TrayNo:{1}", mag.MagazineNo, dm));
                        }
                    }
                }

                //処理終わったので、要求をOFF
                Plc.SetBit(Tray2DReadJudgeAddress(), 1, getResponseCode("OK"));
                Plc.SetBit(Tray2DReadRequestAddress(), 1, Keyence.BIT_OFF);
            }
            catch(Exception ex)
            {
                Plc.SetBit(Tray2DReadJudgeAddress(), 1, getResponseCode("NG"));
                Plc.SetBit(Tray2DReadRequestAddress(), 1, Keyence.BIT_OFF);

                OutputSysLog(ex.Message);
            }

        }

        public virtual string getResponseCode(string judgecode)
        {
            string retv;
            if (judgecode == "OK")
            {
                retv = "2";
            }
            else if (judgecode == "NG")
            {
                retv = "1";
            } else if (judgecode == "FinOK") {
                retv = "1";
            }
            else {
                throw new ApplicationException("[getResponseCode]に異常な引数が指定されました。judgecode:" + judgecode);
            }

            return retv;
        }

        public void workComplete(string Tray2DFinAddress)
        {
            VirtualMag mag = new VirtualMag();
            string dm;
            if (Tray2DFinAddress == null)
            {   //外製IPS
                dm = Plc.GetString(Tray2DAddress(), Tray_WORD_LEN());
            }
            else {
                //内製IPS
                dm = Plc.GetString(Tray2DFinAddress, Tray_WORD_LEN());
            }

        //dm = "tray1";//トレイ情報が装置から取れないので一時的に。

        List<string> listlotno = new List<string>();

            if (string.IsNullOrEmpty(dm) == false)
            {
                //dmはトレイの2D
                listlotno = getLotList(dm);

                if (listlotno.Count == 0)
                {
                    throw new ApplicationException("[完了登録異常] IPS装置搬入ロットNOの取得に失敗。(読み込んだトレイDMに紐つくロットが存在しません。)\nIPS装置搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。 DM:" + dm);
                }
            }
            else
            {
                throw new ApplicationException("[完了登録異常] IPS装置搬入マガジンNOの取得に失敗。\nIPS装置搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
            }

            string magno = "";

            foreach (string lotno in listlotno)
            {
                Magazine[] magArray = Magazine.GetMagazine(lotno, true);

                if (magArray.Length != 1)
                {
                    throw new ApplicationException("[完了登録異常] マガジン情報が1件に絞れません。ロット:" + lotno + "/取得マガジン(カンマ区切り):" + string.Join(",", magArray.Select(m => m.MagazineNo)));
                }

                magno = magArray[0].MagazineNo;
                Magazine svrmag = magArray[0];

                OutputSysLog(string.Format("[完了処理] 開始 MagazineNo:{0}/TrayNo:{1}", magno, dm));

                AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);
                mag.ProcNo = nextproc.ProcNo;
                mag.MagazineNo = magno;
                mag.LastMagazineNo = magno;
                mag.WorkStart = getPlcDt(WorkStartDtAddress_yy());        //装置メモリから取得
                mag.WorkComplete = getPlcDt(WorkEndDtAddress_yy());       //装置メモリから取得

                //既に[同一ロット/プロセス]でTnVertualMagにあるか?
                //  (作業開始時にはVertualMagは作らず、作業完了時にTnVertualMagの作成又は更新)
                //　(1)ある場合：TnVertualMagへDelete&Insert
                //  (2)ない場合：TnVertualMagへInsert
                //Order startOrder = Order.GetMachineOrder(this.MacNo, svrmag.NascaLotNO, nextproc.ProcNo);

                VirtualMag[] mags = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Unloader).ToString(), lotno, nextproc.ProcNo);
                mags = mags.OrderBy(m => m.orderid).ToArray();

                //  (2)ない場合：TnVertualMagへInsert
                if (mags.Length == 0)
                {
                    this.Enqueue(mag, Station.Unloader);
                }
                else
                //　(1)ある場合：TnVirtualMagへDelete&Insert
                {
                    mags[0].WorkComplete = mag.WorkComplete;
                    mags[0].Updatequeue();
                }
                OutputSysLog(string.Format("[完了処理] 完了 MagazineNo:{0}/TrayNo:{1}", magno, dm));
            }
            Plc.SetBit(DischargeResponseAddress(), 1, getResponseCode("FinOK"));    //判定OK=0 内製OK=0 外製は1
            Plc.SetBit(DischargeRequestAddress(), 1, Keyence.BIT_OFF); //排出要求OFF

            //トレイIDを解除
            //ARMS4がLENSに組み込まれるまではTnCussetteもチェックする。最終的にはTnLotCarrierのみになる。
            List<Cassette> cassetteList = Cassette.GetCassetteList(null, null, dm, 1);
            if (cassetteList.Count() > 0)
            {
                foreach (Cassette ct in cassetteList)
                {
                    ct.RingNo = null;
                    ct.Lastupddt = DateTime.Now;

                    ct.Update();
                }
            }
            else
            {
                string lotno = ArmsApi.Model.LotCarrier.GetLotNoFromRingNo(dm);
                string[] carrierNoArray = ArmsApi.Model.LotCarrier.GetCarrierNo(lotno, true, dm);
                foreach(string carrier in carrierNoArray)
                {
                    LotCarrier lc = LotCarrier.GetData(carrier, true, true);
                    lc.RingNo = null;
                    lc.EmpCd = "660";

                    lc.UpdateRingNo(true);
                }
            }
        }

        public DateTime getPlcDt(string plcaddress)
        {
            DateTime WorkDt = DateTime.MinValue;
            
            WorkDt = Plc.GetWordsAsDateTime(plcaddress);

            if(WorkDt == null){
                throw new ApplicationException(string.Format("装置メモリから日付データが取得出来ませんでした。ADDRESS:{0}", plcaddress));
            }
            //2017年の下2桁(17)しか入っていない装置がある為、その対応
            if (WorkDt.Year < 1000) {
                WorkDt=WorkDt.AddYears(2000);
            }
           
            return WorkDt;
        }

        private List<string> getLotList(string datamatrixid)
        {
            List<string> retlist = new List<string>();

            //dmはトレイの2D
            string[] lotNoArray = ArmsApi.Model.LotCarrier.GetLotNo(null, datamatrixid, true, false);   //TnLotCarrierからロット一覧取得
            string[] lotNoArray2 = ArmsApi.Model.Cassette.Getlotno(datamatrixid);                       //TnCassetteからロット一覧取得

            retlist = new List<string>();
            retlist.AddRange(lotNoArray);
            retlist.AddRange(lotNoArray2);
            retlist = retlist.Distinct().ToList();//両テーブルのロット一覧(ロット重複省いたリスト)
            
            return retlist;//ロットの重複を省いたリスト
        }
    }
}
