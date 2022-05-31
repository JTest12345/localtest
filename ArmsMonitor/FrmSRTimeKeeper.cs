using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArmsApi.Model;
using ArmsApi;

namespace ArmsMonitor
{
    public partial class FrmSRTimeKeeper : Form
    {
        class TimeLimitSetting
        {
            public string TypeCd { get; set; }
            public string ChkWorkCd { get; set; }
        }

        // 検索対象時間(日, 時間, 分, 秒)
        private TimeSpan searchtsMax = new TimeSpan(0, 24, 0, 0);
        private TimeSpan searchtsMin = new TimeSpan(0, -24, 0, 0);

        private static string SR_WORK_CD = "SR";

        private System.Media.SoundPlayer sp;

        public static bool isRunning = false;

        private const string MOLD_WAIT_TIMELIMIT_SETTING_TXT = "MoldWaitTimeLimitSetting.txt";
        private static List<TimeLimitSetting> MoldWaitTimeLimitSettingList;

        private static FrmSRTimeKeeper instance;

        public static FrmSRTimeKeeper GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new FrmSRTimeKeeper();
            }

            if (string.IsNullOrEmpty(Config.Settings.DBPreOvnPLWorkCD) == false)
            {
                SR_WORK_CD = Config.Settings.DBPreOvnPLWorkCD;
                Process p = Process.GetProcess(SR_WORK_CD);
                if (p != null)
                {
                    instance.Text = $"{p.InlineProNM}({SR_WORK_CD})時間監視";
                }
            }

            MoldWaitTimeLimitSettingList = new List<TimeLimitSetting>();
            if (System.IO.File.Exists(MOLD_WAIT_TIMELIMIT_SETTING_TXT))
            {
                string[] myDatas = System.IO.File.ReadAllLines(MOLD_WAIT_TIMELIMIT_SETTING_TXT);
                foreach (string myData in myDatas)
                {
                    string[] splitData = myData.Split(',');
                    if (splitData.Length < 2) continue;

                    MoldWaitTimeLimitSettingList.Add(new TimeLimitSetting() { TypeCd = splitData[0], ChkWorkCd = splitData[1] });
                }
            }

            return instance;
        }


        private FrmSRTimeKeeper()
        {
            InitializeComponent();
        }

        private void FrmSRTimeKeeper_Load(object sender, EventArgs e)
        {
            this.timer1.Enabled = true;
            this.sp = new System.Media.SoundPlayer("ringin.wav");
        }

        //private DBPreOvnPLInst[] insts;
        private SRReginInst[] insts;

        private void UpdateData()
        {
            if (isRunning) return;

            try
            {
                isRunning = true;

                insts = getSRInsts();

                this.Invoke(new Action(setGrid));

                //if (this.insts.Length >= 1)
                //{
                //    this.sp.PlayLooping();
                //    MessageBox.Show("プラズマ投入待ちロットが存在します");
                //    MessageBox.Show("投入可能ロットが存在します");
                //    this.sp.Stop();
                //}
            }
            finally
            {
                isRunning = false;
            }
        }

        #region setLimitGrid

        private void setGrid()
        {
            BindingSource bs = new BindingSource(this.insts, string.Empty);
            this.grdLimit.DataSource = bs;

            foreach (DataGridViewColumn col in this.grdLimit.Columns)
            {
                switch (col.Name)
                {
                    case "mixresultid":
                        col.HeaderText = "カップNo.";
                        col.ReadOnly = true;
                        col.Width = 150;
                        break;

                    case "workstartdt":
                        col.HeaderText = "使用許可日時";
                        col.ReadOnly = true;
                        col.Width = 150;
                        break;

                    case "timetostart":
                        col.HeaderText = "許可残り時間";
                        col.ReadOnly = true;
                        col.Width = 150;
                        break;

                    case "uselimit":
                        col.HeaderText = "使用期限";
                        col.ReadOnly = true;
                        col.Width = 150;

                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }

            foreach (DataGridViewRow row in this.grdLimit.Rows)
            {
                switch ((ResultKb)row.Cells["Result"].Value)
                {
                    case ResultKb.Expired:
                        row.DefaultCellStyle.BackColor = Color.Red;
                        break;

                    case ResultKb.Warning:
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                        break;

                    case ResultKb.CanBeStart:
                        row.DefaultCellStyle.BackColor = Color.White;
                        break;

                    case ResultKb.BeforeStart:
                        row.DefaultCellStyle.BackColor = Color.Cyan;
                        break;
                }
            }
        }
        #endregion

        #region getInsts

        private SRReginInst[] getSRInsts()
        {
            List<SRReginInst> retv = new List<SRReginInst>();
            List<Resin> rlist = Resin.GetResinList(searchtsMax);
            List<LimitCheckResult> lcr = new List<LimitCheckResult>();

            foreach (Resin rsin in rlist)
            {
                SRReginInst ins = new SRReginInst();
                ins.mixresultid = rsin.MixResultId;
                ins.bincd = rsin.BinCD;
                ins.stirringlimitdt = rsin.StirringLimitDt.ToString();
                ins.mixtypecd = rsin.MixTypeCd;
                
                //開始可能時間処理
                ins.workstartdt = rsin.WorkStartDt.ToString();
                var t2s = new TimeSpan(0, 0, 0, 0);

                if (rsin.WorkStartDt != null)
                {
                    t2s = (DateTime)rsin.WorkStartDt - DateTime.Now;
                    if (t2s > new TimeSpan(0,0,0,0))
                    {
                        var t2sarr = t2s.ToString("G").Split(':');
                        if (t2sarr[0] != "0")
                        {
                            ins.timetostart += t2sarr[0] + "日 ";
                        }
                        if (t2sarr[1] != "0" && ins.timetostart != null)
                        {
                            ins.timetostart += t2sarr[1] + "時間 ";
                        }
                        if (t2sarr[2] != "0" || ins.timetostart != null)
                        {
                            ins.timetostart += t2sarr[2] + "分 ";
                        }

                        ins.Result = ResultKb.BeforeStart;
                    }
                    else
                    {
                        ins.timetostart = "開始可能";
                        ins.Result = ResultKb.CanBeStart;
                    }
                }

                //使用期限時間処理
                ins.uselimit = rsin.LimitDt.ToString();
                var warnts = new TimeSpan(0, 0, 60, 0);
                var odts = new TimeSpan(0, 0, 90, 0);
                var currts = new TimeSpan(0, 0, 0, 0);

                if (rsin.LimitDt != null)
                {
                    currts = (DateTime)rsin.LimitDt - DateTime.Now;
                    if (currts > new TimeSpan(0, 0, 0, 0))
                    {
                        if (currts < warnts)
                        {
                            ins.Result = ResultKb.Warning;
                        }
                    }
                    else
                    {
                        ins.Result = ResultKb.Expired;
                    }
                }
                if (currts < searchtsMax && currts > searchtsMin)
                {
                    retv.Add(ins);
                }
            }

            return retv.OrderBy(o => o.uselimit).ToArray();
        }


        //private DBPreOvnPLInst[] getInsts()
        //{
        //    List<DBPreOvnPLInst> retv = new List<DBPreOvnPLInst>();

        //    Process plProc = Process.GetProcess(SR_WORK_CD);

        //    TimeLimit[] limits = TimeLimit.GetLimits(null, plProc.ProcNo, false);
        //    SortedList<string, TimeLimit> includeTypes = new SortedList<string, TimeLimit>();
        //    List<int> includeProcs = new List<int>();

        //    foreach (var lim in limits)
        //    {
        //        //マイナス時間を持っている設定しか考慮しない
        //        if (lim.EffectLimit < 0)
        //        {
        //            if (MoldWaitTimeLimitSettingList.Count() > 0)
        //            {
        //                if (MoldWaitTimeLimitSettingList.Any(m => m.TypeCd == lim.TypeCd && m.ChkWorkCd == lim.ChkWorkCd) == false)
        //                {
        //                    continue;
        //                }
        //            }
        //            includeTypes.Add(lim.TypeCd, lim);
        //        }
        //    }

        //    Magazine[] mags = Magazine.GetMagazine(true);

        //    foreach (Magazine m in mags)
        //    {
        //        AsmLot lot = AsmLot.GetAsmLot(m.NascaLotNO);

        //        //監視タイプに含まれていれば処理無し
        //        if (includeTypes.Keys.Contains(lot.TypeCd) == false) continue;

        //        //硬化前プラズマ開始ロットは通知なし
        //        Order ord = Order.GetMagazineOrder(lot.NascaLotNo, plProc.ProcNo);
        //        if (ord != null)
        //        {
        //            continue;
        //        }

        //        TimeLimit limit = includeTypes[lot.TypeCd];
        //        Process dbproc = limit.TgtProc;

        //        ord = Order.GetMagazineOrder(lot.NascaLotNo, dbproc.ProcNo);

        //        //DB完了前ロットは通知なし
        //        if (ord == null || ord.WorkEndDt.HasValue == false)
        //        {
        //            continue;
        //        }

        //        //ダイボンド終了時刻から現時刻が時間監視のマイナス値以下なら通知なし
        //        //if ((DateTime.Now - ord.WorkEndDt.Value).TotalMinutes <= Math.Abs(limit.EffectLimit))
        //        //{
        //        //    continue;
        //        //}

        //        DateTime tgtDt;
        //        if (limit.TgtKb == TimeLimit.JudgeKb.End)
        //        {
        //            tgtDt = ord.WorkEndDt.Value;
        //        }
        //        else
        //        {
        //            tgtDt = ord.WorkStartDt;
        //        }
        //        if ((DateTime.Now - tgtDt).TotalMinutes <= Math.Abs(limit.EffectLimit))
        //        {
        //            continue;
        //        }

        //        DBPreOvnPLInst ins = new DBPreOvnPLInst();
        //        ins.TypeCd = lot.TypeCd;
        //        ins.LotNo = lot.NascaLotNo;
        //        ins.MacGroup = string.Join(",", lot.MacGroup);
        //        ins.MagNo = m.MagazineNo;
        //        ins.ProcNo = m.NowCompProcess;
        //        ins.DBEndDt = ord.WorkEndDt.Value;

        //        retv.Add(ins);
        //    }

        //    return retv.OrderBy(o => o.DBEndDt).ToArray();
        //}

        #endregion


        public enum ResultKb
        {
            Attension,
            Warning,
            Expired,
            Normal,
            Stop,
            None,
            BeforeStart,
            CanBeStart,
        }


        public class SRReginInst
        {
            public string mixresultid { get; set; }
            public string workstartdt { get; set; }
            public string timetostart { get; set; }
            public string uselimit { get; set; }
            public string bincd { get; set; }
            public string lastupddt { get; set; }
            public string mixtypecd { get; set; }
            public string stirringlimitdt { get; set; }
            public ResultKb Result { get; set; }
        }

        public class DBPreOvnPLInst
        {
            public int ProcNo { get; set; }
            public string MagNo { get; set; }
            public string TypeCd { get; set; }
            public string LotNo { get; set; }
            public string MacGroup { get; set; }
            public DateTime DBEndDt { get; set; }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                //timer1.Interval = Config.Settings.MonitorTimerMilliSecond;
                timer1.Interval = 10000;
                Action act = new Action(UpdateData);
                act.BeginInvoke(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FrmDBPreOvnPlasma_FormClosing(object sender, FormClosingEventArgs e)
        {
            FrmPasswordDialog pwd = new FrmPasswordDialog("ARMS", "時間監視を本当に終了しますか?\r\n終了する場合パスワードを入力してください");
            DialogResult res = pwd.ShowDialog();

            if (res != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        private void grdLimit_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

    }
}
