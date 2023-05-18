using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi.Model;
using ArmsApi;

namespace ArmsMaintenance
{
    public partial class FrmPreDBMonitor : Form
    {
        private System.Media.SoundPlayer sp;

        MapFrameBaking[] fbresult;
        Map1stDBLoader[] dbresult;

        List<string> fbfirstWarning = new List<string>();

        private static FrmPreDBMonitor instance;
        public static FrmPreDBMonitor GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new FrmPreDBMonitor();
            }

            return instance;
        }

        private FrmPreDBMonitor()
        {
            InitializeComponent();
        }


        private void UpdateData()
        {
            fbresult = MapFrameBaking.GetNotIgnoredList();
            this.Invoke(new Action(setBakingGrid));


            dbresult = Map1stDBLoader.GetAllRecord();
            this.Invoke(new Action(setDBGrid));

            bool hasFbWarning = false;
            foreach (DataGridViewRow row in this.grdLimit.Rows)
            {
                if (row.DefaultCellStyle.BackColor == Color.Yellow)
                {
                    string fblotno = row.Cells["FrameLotNo"].Value.ToString();

                    if (!this.fbfirstWarning.Contains(fblotno))
                    {
                        hasFbWarning = true;
                        fbfirstWarning.Add(fblotno);
                    }
                }
                if (row.DefaultCellStyle.BackColor == Color.Pink)
                {
                    hasFbWarning = true;
                }
            }

            bool hasDbWarning = false;
            foreach (DataGridViewRow row in this.grdDBLoader.Rows)
            {
                if (row.DefaultCellStyle.BackColor == Color.Pink)
                {
                    bool dbIgnoreFg = (bool)row.Cells["IgnoreFg"].Value;
                    if (dbIgnoreFg == false)
                    {
                        hasDbWarning = true;
                    }
                }
            }


            if (hasFbWarning)
            {
                this.sp.PlayLooping();
                MessageBox.Show("ベーキング作業監視警告が存在します");
                this.sp.Stop();
            }

            if (hasDbWarning)
            {
                this.sp.PlayLooping();
                MessageBox.Show("基板マガジンを投入してベーキングを開始して下さい");
                this.sp.Stop();
            }
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

        private void FrmPreDBMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            FrmPasswordDialog pwd = new FrmPasswordDialog("CLOSE", "時間監視を本当に終了しますか?\r\n終了する場合CLOSEと入力してください");
            DialogResult res = pwd.ShowDialog();

            if (res != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }


        #region 無効ボタン関係
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIgnoreDBLoader_Click(object sender, EventArgs e)
        {
            List<Map1stDBLoader> checklist = new List<Map1stDBLoader>();
            foreach (DataGridViewRow row in this.grdDBLoader.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colChk2"].Value) == true)
                {
                    checklist.Add((Map1stDBLoader)row.DataBoundItem);
                }
            }

            if (checklist.Count == 0) return;

            FrmPasswordDialog pwd = new FrmPasswordDialog("OK", "チェックを付けた行を削除します\r\n続行する場合OKと入力してください");
            DialogResult res = pwd.ShowDialog();

            if (res != DialogResult.OK) return;

            foreach (Map1stDBLoader dbl in checklist)
            {
                dbl.Delete();
            }

            Action act = new Action(UpdateData);
            act.BeginInvoke(null, null);

            MessageBox.Show("設定完了");

            timer1.Interval = Config.Settings.MonitorTimerMilliSecond;
        }

        private void btnStopAlert_Click(object sender, EventArgs e)
        {

            List<Map1stDBLoader> checklist = new List<Map1stDBLoader>();
            foreach (DataGridViewRow row in this.grdDBLoader.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colChk2"].Value) == true)
                {
                    checklist.Add((Map1stDBLoader)row.DataBoundItem);
                }
            }

            if (checklist.Count == 0) return;

            FrmPasswordDialog pwd = new FrmPasswordDialog("OK", "チェックを付けた行を警報無効に設定します\r\n続行する場合OKと入力してください");
            DialogResult res = pwd.ShowDialog();

            if (res != DialogResult.OK) return;

            foreach (Map1stDBLoader dbl in checklist)
            {
                dbl.IgnoreFg = true;
                dbl.Update();
            }

            Action act = new Action(UpdateData);
            act.BeginInvoke(null, null);

            MessageBox.Show("設定完了");

            timer1.Interval = Config.Settings.MonitorTimerMilliSecond;
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            List<MapFrameBaking> checklist = new List<MapFrameBaking>();
            foreach (DataGridViewRow row in this.grdLimit.Rows)
            {
                if (Convert.ToBoolean(row.Cells["colChk"].Value) == true)
                {
                    checklist.Add((MapFrameBaking)row.DataBoundItem);
                }
            }

            if (checklist.Count == 0) return;

            FrmPasswordDialog pwd = new FrmPasswordDialog("OK", "チェックを付けた行の前回ベーキング記録を無効に設定します\r\n続行する場合OKと入力してください");
            DialogResult res = pwd.ShowDialog();

            if (res != DialogResult.OK) return;

            foreach (MapFrameBaking fb in checklist)
            {
                fb.IgnoreLastBakingFg = true;
                fb.Update();
            }

            Action act = new Action(UpdateData);
            act.BeginInvoke(null, null);

            MessageBox.Show("設定完了");

            timer1.Interval = Config.Settings.MonitorTimerMilliSecond;
        }
        #endregion


        private void FrmPreDBMonitor_Load(object sender, EventArgs e)
        {
            this.timer1.Enabled = true;
            this.sp = new System.Media.SoundPlayer("ringin.wav");
        }

        private void grdLimit_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void setBakingGrid()
        {
            this.grdLimit.DataSource = fbresult;

            #region 列設定

            foreach (DataGridViewColumn col in this.grdLimit.Columns)
            {
                switch (col.Name)
                {
                    case "colChk":
                        break;

                    case "FrameLotNo":
                        col.HeaderText = "ロット";
                        col.ReadOnly = true;
                        break;

                    case "MagIndex":
                        col.HeaderText = "連番";
                        col.ReadOnly = true;
                        break;

                    case "CurrentWorkingMacName":
                        col.HeaderText = "装置";
                        col.ReadOnly = true;
                        break;

                    case "LimitDt":
                        col.HeaderText = "期限";
                        col.ReadOnly = true;
                        break;

                    case "BakingCt":
                        col.HeaderText = "ベーキング回数";
                        col.ReadOnly = true;
                        break;

                    case "RemainsMinutes":
                        col.HeaderText = "残り(分)";
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
            #endregion

            foreach (DataGridViewRow row in this.grdLimit.Rows)
            {
                double remains = ( ((DateTime)row.Cells["LimitDt"].Value) - DateTime.Now).TotalMinutes;

                if (remains <= 300)
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                }
                if (remains < 60)
                {
                    row.DefaultCellStyle.BackColor = Color.Pink;
                }
            }
        }


        private void setDBGrid()
        {
            this.grdDBLoader.DataSource = dbresult;

            #region 列設定

            foreach (DataGridViewColumn col in this.grdDBLoader.Columns)
            {
                switch (col.Name)
                {
                    case "colChk2":
                        break;

                    case "MacName":
                        col.HeaderText = "装置";
                        col.ReadOnly = true;
                        break;

                    case "MagCt":
                        col.HeaderText = "マガジン数";
                        col.ReadOnly = true;
                        break;

                    case "FrameCt":
                        col.HeaderText = "基板枚数";
                        col.ReadOnly = true;
                        break;

                    case "WarningDt":
                        col.HeaderText = "期限";
                        col.ReadOnly = true;
                        break;

                    case "RemainsMinutes":
                        col.HeaderText = "残り(分)";
                        col.ReadOnly = true;
                        break;


                    case "IgnoreFg":
                        col.HeaderText = "警報無効";
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
            #endregion

            foreach (DataGridViewRow row in this.grdDBLoader.Rows)
            {
                double remains = (((DateTime)row.Cells["WarningDt"].Value) - DateTime.Now).TotalMinutes;

                if (remains < 0)
                {
                    row.DefaultCellStyle.BackColor = Color.Pink;
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            Action act = new Action(UpdateData);
            act.BeginInvoke(null, null);
            timer1.Interval = Config.Settings.MonitorTimerMilliSecond;
        }
    }
}
