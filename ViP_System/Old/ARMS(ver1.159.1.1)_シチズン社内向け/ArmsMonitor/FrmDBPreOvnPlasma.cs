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
    public partial class FrmDBPreOvnPlasma : Form
    {
        class TimeLimitSetting
        {
            public string TypeCd { get; set; }
            public string ChkWorkCd { get; set; }
        }

        private static string PL_WORK_CD = "DB0027";

        private System.Media.SoundPlayer sp;

        public static bool isRunning = false;

        private const string MOLD_WAIT_TIMELIMIT_SETTING_TXT = "MoldWaitTimeLimitSetting.txt";
        private static List<TimeLimitSetting> MoldWaitTimeLimitSettingList;

        private static FrmDBPreOvnPlasma instance;

        public static FrmDBPreOvnPlasma GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new FrmDBPreOvnPlasma();
            }

            if (string.IsNullOrEmpty(Config.Settings.DBPreOvnPLWorkCD) == false)
            {
                PL_WORK_CD = Config.Settings.DBPreOvnPLWorkCD;
                Process p = Process.GetProcess(PL_WORK_CD);
                if (p != null)
                {
                    instance.Text = $"{p.InlineProNM}({PL_WORK_CD})時間監視";
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


        private FrmDBPreOvnPlasma()
        {
            InitializeComponent();
        }

        private void FrmDBPreOvnPlasma_Load(object sender, EventArgs e)
        {
            this.timer1.Enabled = true;
            this.sp = new System.Media.SoundPlayer("ringin.wav");
        }

        private DBPreOvnPLInst[] insts;

        private void UpdateData()
        {
            if (isRunning) return;

            try
            {
                isRunning = true;

                insts = getInsts();

                this.Invoke(new Action(setGrid));

                if (this.insts.Length >= 1)
                {
                    this.sp.PlayLooping();
                    //MessageBox.Show("プラズマ投入待ちロットが存在します");
                    MessageBox.Show("投入可能ロットが存在します");
                    this.sp.Stop();
                }
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
                    case "LotNo":
                        col.HeaderText = "ロット";
                        col.ReadOnly = true;
                        break;

                    case "MagNo":
                        col.HeaderText = "マガジン";
                        col.ReadOnly = true;
                        break;

                    case "TypeCd":
                        col.HeaderText = "タイプ";
                        col.ReadOnly = true;
                        break;

                    case "MacGroup":
                        col.HeaderText = "グループ";
                        col.ReadOnly = true;
                        break;

                    case "DBEndDt":
                        col.HeaderText = "DB終了日";
                        if (string.IsNullOrEmpty(Config.Settings.DBPreOvnPLWorkCD) == false)
                        {
                            col.HeaderText = "起点作業終了日";
                        }
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
        }
        #endregion

        #region getInsts


        private DBPreOvnPLInst[] getInsts()
        {
            List<DBPreOvnPLInst> retv = new List<DBPreOvnPLInst>();

            Process plProc = Process.GetProcess(PL_WORK_CD);

            TimeLimit[] limits = TimeLimit.GetLimits(null, plProc.ProcNo, false);
            SortedList<string, TimeLimit> includeTypes = new SortedList<string, TimeLimit>();
            List<int> includeProcs = new List<int>();

            foreach (var lim in limits)
            {
                //マイナス時間を持っている設定しか考慮しない
                if (lim.EffectLimit < 0)
                {
                    if (MoldWaitTimeLimitSettingList.Count() > 0)
                    {
                        if (MoldWaitTimeLimitSettingList.Any(m => m.TypeCd == lim.TypeCd && m.ChkWorkCd == lim.ChkWorkCd) == false)
                        {
                            continue;
                        }
                    }
                    includeTypes.Add(lim.TypeCd, lim);
                }
            }

            Magazine[] mags = Magazine.GetMagazine(true);

            foreach (Magazine m in mags)
            {
                AsmLot lot = AsmLot.GetAsmLot(m.NascaLotNO);

                //監視タイプに含まれていれば処理無し
                if (includeTypes.Keys.Contains(lot.TypeCd) == false) continue;

                //硬化前プラズマ開始ロットは通知なし
                Order ord = Order.GetMagazineOrder(lot.NascaLotNo, plProc.ProcNo);
                if (ord != null)
                {
                    continue;
                }

                TimeLimit limit = includeTypes[lot.TypeCd];
                Process dbproc = limit.TgtProc;

                ord = Order.GetMagazineOrder(lot.NascaLotNo, dbproc.ProcNo);

                //DB完了前ロットは通知なし
                if (ord == null || ord.WorkEndDt.HasValue == false)
                {
                    continue;
                }

                //ダイボンド終了時刻から現時刻が時間監視のマイナス値以下なら通知なし
                //if ((DateTime.Now - ord.WorkEndDt.Value).TotalMinutes <= Math.Abs(limit.EffectLimit))
                //{
                //    continue;
                //}

                DateTime tgtDt;
                if (limit.TgtKb == TimeLimit.JudgeKb.End)
                {
                    tgtDt = ord.WorkEndDt.Value;
                }
                else
                {
                    tgtDt = ord.WorkStartDt;
                }
                if ((DateTime.Now - tgtDt).TotalMinutes <= Math.Abs(limit.EffectLimit))
                {
                    continue;
                }

                DBPreOvnPLInst ins = new DBPreOvnPLInst();
                ins.TypeCd = lot.TypeCd;
                ins.LotNo = lot.NascaLotNo;
                ins.MacGroup = string.Join(",", lot.MacGroup);
                ins.MagNo = m.MagazineNo;
                ins.ProcNo = m.NowCompProcess;
                ins.DBEndDt = ord.WorkEndDt.Value;

                retv.Add(ins);
            }

            return retv.OrderBy(o => o.DBEndDt).ToArray();
        }
        #endregion

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
                timer1.Interval = Config.Settings.MonitorTimerMilliSecond;
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
