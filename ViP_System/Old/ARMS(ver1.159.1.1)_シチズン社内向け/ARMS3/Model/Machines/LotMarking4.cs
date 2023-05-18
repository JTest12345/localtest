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
    /// DMC/ロットマーキング装置(DMC/PE製自動機)・NTSVバッドマーキング装置
    /// </summary>
    public class LotMarking4 : MachineBase
    {
        public const string DM_2D_READ_REQUEST_ADDRESS="EM50051";
        public const string DM_2D_READ_JUDGE_ADDRESS="EM50021";
        public const string DISCHARGE_REQUEST_ADDRESS = "EM50052";
        public const string DISCHARGE_RESPONSE_ADDRESS = "EM50022";
        public const string WORKSTARTDT_ADDRESS_YY = "EM51500";
        public const string WORKENDDT_ADDRESS_YY = "EM51508";
        public const string LD_DM_ADDRESS = "EM51000";
        public const string UNLD_DM_ADDRESS = "EM51010";
        public const string JUDGE_DOWORK_ADDRESS = "EM50023";
        public const int MAG_WORD_LEN = 10;
        public const string JUDGEOK = "0";
        public const string JUDGENG = "1";
        public const int JUDGE_DOWORK_NECESSITY = 1;
        public const int JUDGE_DOWORK_UNNECESSITY = 2;


        //ARMS4(松島さんの旧マッピングシステム)と併用する場合mpdファイルのリネームをしないため
        //それ用のタグ。なければfalse扱い。
        public bool NotRenameTrgFile { get; set; }
        public bool inOutMagAddrIsSameFg { get; set; }
        public bool isJudgeDoWork { get; set; }

        protected override void concreteThreadWork()
        {
            try
            {
                if (Convert.ToInt32(Plc.GetBit(DM_2D_READ_REQUEST_ADDRESS)) == Convert.ToInt32(PLC.Common.BIT_ON))
                {
                    workStart();
                }

                // 作業完了登録
                if (Convert.ToInt32(Plc.GetBit(DISCHARGE_REQUEST_ADDRESS)) == Convert.ToInt32(PLC.Common.BIT_ON))
                {
                    workComplete();
                }

                if(NotRenameTrgFile == false)
                {
                    this.substrateStart();
                }

            }
            catch (Exception ex)
            {
                FrmErrHandle frmErr = new FrmErrHandle(ex.ToString(), this.MacNo);
                frmErr.ShowDialog();

                if (frmErr.Method == ErrorHandleMethod.None)
                {
                    throw;
                }
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
            int judgeDoWorkMode = 0;
            //LENSがマーキングデータ有無を確認するモードの場合はLENS書込み完了まで待機。
            if (this.isJudgeDoWork == true)
            {
                judgeDoWorkMode = this.Plc.GetWordAsDecimalData(JUDGE_DOWORK_ADDRESS);
                if (judgeDoWorkMode == 0) return;
            }

            try
            {
                VirtualMag mag = new VirtualMag();

                string dm = this.Plc.GetMagazineNo(LD_DM_ADDRESS, MAG_WORD_LEN);


                List<string> listlotno = new List<string>();

                if (string.IsNullOrWhiteSpace(dm) == true)
                {
                    throw new ApplicationException("[開始登録異常] 装置からマガジン情報が取得出来ません。");
                }

                List<bool> listng = new List<bool>();

                Magazine svrmag = Magazine.GetCurrent(dm);

                if (svrmag == null)
                {
                    throw new ApplicationException("[開始登録異常] 稼働中マガジン情報が1件に絞れないか、取得出来ません。マガジン:" + dm);
                }

                AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                if (svrlot == null)
                {
                    throw new ApplicationException("[開始登録異常] ロット情報が、取得出来ません。ロット:" + svrmag.NascaLotNO);
                }

                Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);
                if (nextproc == null)
                {
                    throw new ApplicationException("[開始登録異常] 現在の完了工程の次作業がありません。ロット:" + svrmag.NascaLotNO);
                }

                mag.MagazineNo = svrmag.MagazineNo;
                mag.ProcNo = nextproc.ProcNo;

                Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);
                order.WorkStartDt = DateTime.Now;

                OutputSysLog(string.Format("[開始処理] 開始 MagazineNo:{0}/DM:{1}", svrmag.MagazineNo, dm));
                ArmsApiResponse workResponse = CommonApi.WorkStart(order);

                if (workResponse.IsError)
                {
                    OutputSysLog(string.Format("[開始登録異常] 装置:{0}/DM:{1}/理由:{2}", this.MacNo, dm, workResponse.Message));

                    Plc.SetBit(DM_2D_READ_JUDGE_ADDRESS, 1, JUDGENG);//1=NG
                }
                else
                {
                    OutputSysLog(string.Format("[開始処理] 完了 LoaderMagazineNo:{0}/DM:{1}", mag.MagazineNo, dm));

                    Plc.SetBit(DM_2D_READ_JUDGE_ADDRESS, 1, JUDGEOK);//const 0
                }

                //作業不要の場合は完了処理も行う。
                if(this.isJudgeDoWork == true && judgeDoWorkMode == JUDGE_DOWORK_UNNECESSITY && workResponse.IsError == false)
                {
                    OutputSysLog(string.Format("[開始処理] 作業スキップによる完了登録開始 MagazineNo:{0}/DM:{1}", svrmag.MagazineNo, dm));

                    mag.LastMagazineNo = svrmag.MagazineNo;
                    mag.WorkStart = order.WorkStartDt;        
                    mag.WorkComplete = mag.WorkStart;       //開始時間と同じ時間を記録

                    VirtualMag[] mags = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Unloader).ToString(), svrlot.NascaLotNo, nextproc.ProcNo);
                    mags = mags.OrderBy(m => m.orderid).ToArray();

                    if (mags.Length == 0)
                    {
                        this.Enqueue(mag, Station.Unloader);
                    }
                    else
                    {
                        mags[0].WorkComplete = mag.WorkComplete;
                        mags[0].Updatequeue();
                    }
                    OutputSysLog(string.Format("[開始処理] 作業スキップによる完了登録完了 MagazineNo:{0}/DM:{1}", svrmag.MagazineNo, dm));
                }

            }
            catch (Exception ex)
            {
                OutputSysLog(ex.Message);
                Plc.SetBit(DM_2D_READ_JUDGE_ADDRESS, 1, JUDGENG);//const 1
                throw;
            }
            finally
            {
                //処理終わったので、要求をOFF
                Plc.SetBit(DM_2D_READ_REQUEST_ADDRESS, 1, Keyence.BIT_OFF);
            }
        }

        public void workComplete()
        {
            VirtualMag mag = new VirtualMag();

            string dm = string.Empty;
            if(inOutMagAddrIsSameFg == true)
            {
                dm = this.Plc.GetMagazineNo(LD_DM_ADDRESS, MAG_WORD_LEN);
            }
            else
            {
                dm = this.Plc.GetMagazineNo(UNLD_DM_ADDRESS, MAG_WORD_LEN);
            }


            if (string.IsNullOrWhiteSpace(dm) == true)
            {
                throw new ApplicationException("[完了登録異常] 装置からマガジン情報が取得出来ません。");
            }

            Magazine svrmag = Magazine.GetCurrent(dm);
            if (svrmag == null)
            {
                throw new ApplicationException("[完了登録異常] 稼働中マガジン情報が1件に絞れないか、取得出来ません。マガジン:" + dm);
            }
            OutputSysLog(string.Format("[完了処理] 開始 MagazineNo:{0}/DM:{1}", svrmag.MagazineNo, dm));

            AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            if (svrlot == null)
            {
                throw new ApplicationException("[完了登録異常] ロット情報が、取得出来ません。ロット:" + svrmag.NascaLotNO);
            }

            Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

            mag.ProcNo = nextproc.ProcNo;
            mag.MagazineNo = svrmag.MagazineNo;
            mag.LastMagazineNo = svrmag.MagazineNo;
            mag.WorkStart = getPlcDt(WORKSTARTDT_ADDRESS_YY);        //装置メモリから取得
            mag.WorkComplete = getPlcDt(WORKENDDT_ADDRESS_YY);       //装置メモリから取得

            //既に[同一ロット/プロセス]でTnVertualMagにあるか?
            //  (作業開始時にはVertualMagは作らず、作業完了時にTnVertualMagの作成又は更新)
            //　(1)ある場合：TnVertualMagへDelete&Insert
            //  (2)ない場合：TnVertualMagへInsert
            VirtualMag[] mags = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Unloader).ToString(), svrlot.NascaLotNo, nextproc.ProcNo);
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
            OutputSysLog(string.Format("[完了処理] 完了 MagazineNo:{0}/DM:{1}", svrmag.MagazineNo, dm));

            Plc.SetBit(DISCHARGE_RESPONSE_ADDRESS, 1, Keyence.BIT_OFF);
            Plc.SetBit(DISCHARGE_REQUEST_ADDRESS, 1, Keyence.BIT_OFF);

        }

        private void substrateStart()
        {
            List<MachineLog.TriggerFile> trgList = MachineLog.TriggerFile.GetAllFiles(this.LogInputDirectoryPath);
            if (trgList.Count == 0)
            {
                return;
            }
            //更新時間順に並べ替え
            trgList = trgList.OrderBy(t => File.GetLastWriteTime(t.FullName)).ToList();

            CarrireWorkData stepData = new CarrireWorkData();
            stepData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;

            foreach (MachineLog.TriggerFile trg in trgList)
            {
                OutputSysLog(string.Format("[開始処理] 開始 Trgファイル取得成功 FileName:{0}", trg.FullName));

                AsmLot lot = AsmLot.GetAsmLot(trg.NascaLotNo);
                OutputSysLog(string.Format("[開始処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = Process.GetNowProcess(lot).ProcNo;
                OutputSysLog(string.Format("[開始処理] 工程取得成功 ProcNo:{0}", procno));

                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(trg.FullName, lot.NascaLotNo, lot.TypeCd, procno, trg.MagNo, trg.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", trg.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, trg.MagNo);
                OutputSysLog(string.Format("[開始処理] ファイル名変更 from:{0} to:{1}", trg.FullName, fileName));

                //段数情報を記録
                stepData.LotNo = lot.NascaLotNo;
                stepData.ProcNo = procno;
                stepData.CarrierNo = trg.CarrierNo;
                stepData.Delfg = 0;
                stepData.RegisterMagazineStepWithAutotCalc();

                OutputSysLog(string.Format("[開始処理] 完了"));
                
            }
        }

        public DateTime getPlcDt(string plcaddress)
        {
            DateTime WorkDt = DateTime.MinValue;

            WorkDt = Plc.GetWordsAsDateTime(plcaddress);

            if (WorkDt == null)
            {
                throw new ApplicationException(string.Format("装置メモリから日付データが取得出来ませんでした。ADDRESS:{0}", plcaddress));
            }
            //2017年の下2桁(17)しか入っていない装置がある為、その対応
            if (WorkDt.Year < 1000)
            {
                WorkDt = WorkDt.AddYears(2000);
            }

            return WorkDt;
        }
    }
}
