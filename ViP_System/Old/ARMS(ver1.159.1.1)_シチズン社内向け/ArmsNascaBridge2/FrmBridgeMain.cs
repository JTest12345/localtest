using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi.Model;
using System.Xml;
using System.Xml.Linq;
using ArmsApi;

namespace ArmsNascaBridge2
{
    public partial class FrmBridgeMain : Form
    {
        BackgroundWorker bgw = new BackgroundWorker();

        NotifyIcon nicon;
        private bool close = false;

        private const string LAST_ACTION_RECORD_XML = "lastactionrecord.xml";

        private const string LAST_15MIN_NODE = "last15min";
        private const string LAST_1HOUR_NODE = "last1hour";
        private const string LAST_4HOUR_NODE = "last4hour";
        private const string LAST_24HOUR_NODE = "last24hour";

        public const string MANUAL_MODE = "/M";
        private string commandLine = string.Empty;

        public const string ASM_MAT_CODE_SURFIX_A = "A";
        public const int NASCA_LINE_GROUP_CODE_A = 700002;

        /// <summary>
        /// 不良明細取り込みで過去何時間のデータを対象とするか
        /// </summary>
        public const int DEFECT_TARGETPASTHOURS = 4;

        #region Singleton

        private static FrmBridgeMain instance;
        public static FrmBridgeMain GetInstance(string commandLine)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new FrmBridgeMain(commandLine);
            }
            return instance;

        }

        private FrmBridgeMain(string commandLine)
        {
            InitializeComponent();
            this.commandLine = commandLine;
        }
        #endregion

        #region FormLoad
        
        private void FrmBridgeMain_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.commandLine) ||
                this.commandLine != MANUAL_MODE)
            {
                this.splitContainer1.Panel2Collapsed = true;
            }

            //バージョン表示
            lblVersion.Text += Application.ProductVersion;

            if (string.IsNullOrWhiteSpace(this.commandLine) ||
                this.commandLine != MANUAL_MODE)
            {
                nicon = new NotifyIcon();
                nicon.Icon = this.Icon;
                nicon.Text = "ArmsNascaBridge2";
                nicon.Visible = true;
                nicon.ContextMenuStrip = this.contextMenuStrip1;
                nicon.DoubleClick += new EventHandler(nicon_DoubleClick);
                this.Hide();
            }
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }
        #endregion

        public void MainLoop()
        {
            DateTime last15MinAction = loadTime(LAST_15MIN_NODE);
            DateTime last1HourAction = loadTime(LAST_1HOUR_NODE);
            DateTime last4HourAction = loadTime(LAST_4HOUR_NODE);
            DateTime last24HourAction = loadTime(LAST_24HOUR_NODE);
            
            try
            {
                if ((DateTime.Now - last15MinAction).TotalMinutes >= 15)
                {
                    ActionEvery15Min();
                    saveTime(DateTime.Now, LAST_15MIN_NODE);
                }

                if ((DateTime.Now - last1HourAction).TotalMinutes >= 60)
                {
                    ActionEvery1Hour();
                    saveTime(DateTime.Now, LAST_1HOUR_NODE);
                }

                if ((DateTime.Now - last4HourAction).TotalMinutes >= 240)
                {
                    ActionEvery4Hour();
                    saveTime(DateTime.Now, LAST_4HOUR_NODE);
                }

                if ((DateTime.Now - last24HourAction).TotalMinutes >= 1440)
                {
                    ActionEvery24Hour();
                    saveTime(DateTime.Now, LAST_24HOUR_NODE);
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(string.Format("[ArmsNascaBridge2] Error {0}\r\n{1}", ex.Message, ex.StackTrace));
            }
            finally
            {
                System.Threading.Thread.Sleep(10000);
            }
        }

        #region XML関係
        
        private void saveTime(DateTime dt, string node)
        {
            XDocument doc;
            if (System.IO.File.Exists(LAST_ACTION_RECORD_XML))
            {
                doc = XDocument.Load(LAST_ACTION_RECORD_XML);
            }
            else
            {
                doc = new XDocument();
            }

            XElement root = doc.Element("settings");
            if (root == null)
            {
                root = new XElement("settings");
                doc.Add(root);
            }

            XElement elm = root.Element(node);
            if (elm == null)
            {
                elm = new XElement(node);
                root.Add(elm);
            }

            elm.Value = dt.ToString();
            doc.Save(LAST_ACTION_RECORD_XML);
        }


        private DateTime loadTime(string node)
        {
            XDocument doc;
            if (System.IO.File.Exists(LAST_ACTION_RECORD_XML))
            {
                doc = XDocument.Load(LAST_ACTION_RECORD_XML);
            }
            else
            {
                return DateTime.MinValue;
            }

            XElement setting = doc.Element("settings").Element(node);

            if (setting == null)
            {
                return DateTime.MinValue;
            }

            DateTime dt;
            if (DateTime.TryParse(setting.Value, out dt))
            {
                return dt;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        #endregion

		/// <summary>
		/// 15min間隔
		/// 各機能でエラーが発生した場合はエラーメールを送信して処理継続
		/// </summary>
        private void ActionEvery15Min()
        {
        
        }

		/// <summary>
		/// 1hr間隔
		/// 各機能でエラーが発生した場合はエラーメールを送信して処理継続
		/// </summary>
        private void ActionEvery1Hour()
        {
            Log.SysLog.Info("[ArmsNascaBridge2] Start(1hr)");

            AppendLog("TimeLimit Import Start:" + DateTime.Now.ToString());
            TimeLimit.Import();
            AppendLog("TimeLimit Import End:" + DateTime.Now.ToString());

            AppendLog("CutPressDie Import Start:" + DateTime.Now.ToString());
            CutPressDie.Import();
            AppendLog("CutPressDie Import End:" + DateTime.Now.ToString());

            AppendLog("DefectCt Import Start:" + DateTime.Now.ToString());
            DefectCt.Import();
            AppendLog("DefectCt Import End:" + DateTime.Now.ToString());

            AppendLog("DBResinLife Import Start:" + DateTime.Now.ToString());
            DBResinLife.Import();
            AppendLog("DBResinLife Import End:" + DateTime.Now.ToString());

            AppendLog("DBWaferWashLife Import Start:" + DateTime.Now.ToString());
            DBWaferWashedLife.Import();
            AppendLog("DBWaferWashLife Import End:" + DateTime.Now.ToString());

            AppendLog("TypeCondtion Import start:" + DateTime.Now.ToString());
            TypeCondition.Import();
            AppendLog("TypeCondtion Import End:" + DateTime.Now.ToString());

            AppendLog("General Import Start:" + DateTime.Now.ToString());
            General.Import();
            AppendLog("General Import End:" + DateTime.Now.ToString());

            AppendLog("LifeTestResult Import Start:" + DateTime.Now.ToString());
            LifeTestResult.Import();
            AppendLog("LifeTestResult Import End:" + DateTime.Now.ToString());

            AppendLog("LotChar Import Start:" + DateTime.Now.ToString());
            LotChar.Import();
            AppendLog("LotChar Import End:" + DateTime.Now.ToString());

            AppendLog("AsmLot Export Start:" + DateTime.Now.ToString());
            AsmLotExport.Export();
            AppendLog("AsmLot Export End:" + DateTime.Now.ToString());

            //NTSV用(基板メーカーからもらった基板の厚みランクをDBに登録する)
            if (Config.Settings.ImportSubstrateThicknessRankData)
            {
                AppendLog("SubstrateThicknessRankData start:" + DateTime.Now.ToString());
                SubstrateThicknessRankData.Import();
                AppendLog("SubstrateThicknessRankData end:" + DateTime.Now.ToString());
            }

            AppendLog("BomD Import Start:" + DateTime.Now.ToString());
            BomD.Import();
            AppendLog("BomD Import End:" + DateTime.Now.ToString());

            AppendLog("SubstrateMarkingNoData Start:" + DateTime.Now.ToString());
            SubstrateMarkingNoData.Import();
            AppendLog("SubstrateMarkingNoData End:" + DateTime.Now.ToString());

            AppendLog("test sampling start:" + DateTime.Now.ToString());
            TestSampler.Import();
            AppendLog("test sampling end:" + DateTime.Now.ToString());

            Log.SysLog.Info("[ArmsNascaBridge2] End(1hr)");
        }

		/// <summary>
		/// 4hr間隔
		/// 各機能でエラーが発生した場合はエラーメールを送信して処理継続
		/// </summary>
        private void ActionEvery4Hour()
        {
			Log.SysLog.Info("[ArmsNascaBridge2] Start(4hr)");

            if (Config.Settings.ImportWorkFlow)
            {
                AppendLog("workflow master import start:" + DateTime.Now.ToString());
                WorkFlows.Import();
                AppendLog("workflow master import end:" + DateTime.Now.ToString());
            }

            AppendLog("matlabel master import start:" + DateTime.Now.ToString());
            MatLabel.Import();
            AppendLog("matlabel master import end:" + DateTime.Now.ToString());

            AppendLog("first article start:" + DateTime.Now.ToString());
            FirstArticle.Import();
            AppendLog("first article end:" + DateTime.Now.ToString());

            AppendLog("defect judge start:" + DateTime.Now.ToString());
            DefectJudge.Import();
            AppendLog("defect judge end:" + DateTime.Now.ToString());

            AppendLog("WorkMacCond start:" + DateTime.Now.ToString());
            WorkMacCondition.Import();
            AppendLog("WorkMacCond end:" + DateTime.Now.ToString());

            AppendLog("DieShearSampler start:" + DateTime.Now.ToString());
            DieShearSampler.Import();
            AppendLog("DieShearSampler end:" + DateTime.Now.ToString());

            AppendLog("SpecBox Import Start:" + DateTime.Now.ToString());
            SpecBox.Import();
            AppendLog("SpecBox Import End:" + DateTime.Now.ToString());

            //NTSV用(基板メーカーからもらった基板の厚みランクをDBに登録する)
            if (Config.Settings.ImportSubstrateThicknessRankData)
            {
                AppendLog("SubstrateThicknessRankData start:" + DateTime.Now.ToString());
                SubstrateThicknessRankData.Import();
                AppendLog("SubstrateThicknessRankData end:" + DateTime.Now.ToString());
            }
            // defectは処理が遅いので最後
            AppendLog("defect start:" + DateTime.Now.ToString());
            Defect.Import();
            AppendLog("defect end:" + DateTime.Now.ToString());

            Log.SysLog.Info("[ArmsNascaBridge2] End(4hr)");
        }

		/// <summary>
		/// 24hr間隔
		/// 各機能でエラーが発生した場合はエラーメールを送信して処理継続
		/// </summary>
        private void ActionEvery24Hour()
        {
            Log.SysLog.Info("[ArmsNascaBridge2] Start(24hr)");

            AppendLog("employee start:" + DateTime.Now.ToString());
            Employee.Import();
            AppendLog("employee end:" + DateTime.Now.ToString());

            Log.SysLog.Info("[ArmsNascaBridge2] End(24hr)");
        }

        #region NotifyIcon関連


        void nicon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void FrmBridgeMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((string.IsNullOrWhiteSpace(this.commandLine) ||
                this.commandLine != MANUAL_MODE))
            {
                if (close == false)
                {
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;
                    e.Cancel = true;
                    return;
                }
                nicon.Dispose();
            }
        }

        private void 終了ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            close = true;
            this.Close();
        }

        protected override void WndProc(ref Message m)
        {
            if ((m.Msg == 0x112) && (m.WParam == (IntPtr)0xF020))
            {
                this.Hide();
            }
            else
            {
                base.WndProc(ref m);
            }
        }


        #endregion

        #region AppendLog
        
        /// <summary>
        /// ログ記録
        /// </summary>
        /// <param name="txt"></param>
        public static void AppendLog(string txt)
        {
            if (instance == null || instance.IsDisposed)
            {
                return;
            }

            if (instance.InvokeRequired)
            {
                instance.Invoke(new Action<string>(s => AppendLog(s)), txt);
            }
            else
            {
                if (instance.txtLog.Lines.Length >= 300)
                {
                    instance.txtLog.Text = instance.txtLog.Text.Remove(0, instance.txtLog.Lines[0].Length + 2);
                }

                instance.txtLog.AppendText(DateTime.Now.ToString() + ": " +  txt + "\r\n");

				Log.SysLog.Info(txt);
            }
        }
        #endregion

        private void FrmBridgeMain_Shown(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.commandLine) ||
                this.commandLine != MANUAL_MODE)
            {
                Action act = new Action(MainLoop);
                act.BeginInvoke(new AsyncCallback(Complete), act);
            }
        }

        private void Complete(IAsyncResult res)
        {
            try
            {
                Action act = (Action)res.AsyncState;
                act.EndInvoke(res);
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("NascaBridge2連携エラー:" + ex.ToString());
            }
            finally
            {
                this.close = true;
                this.Invoke(new Action(this.Close));
            }
        }

        /// <summary>
        /// 手動連携実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_RelationStart_Click(object sender, EventArgs e)
        {
            try
            {
                ActionEveryTime();
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(string.Format("[ArmsNascaBridgeManual] Error {0}\r\n{1}", ex.Message, ex.StackTrace));
            }
        }

        /// <summary>
        /// フォームでチェックを入れた項目を連携。
        /// 各機能でエラーが発生した場合はエラーメールを送信して処理継続
        /// </summary>
        private void ActionEveryTime()
        {
            Log.SysLog.Info("[ArmsNascaBridgeManual] Start");

            //作業実績監視マスタ
            bool complete_TimeLimit = true;
            if (checkBox_TimeLimit.Checked)
            {
                AppendLog("TimeLimit Import Start:" + DateTime.Now.ToString());
                complete_TimeLimit = TimeLimit.Import();
                AppendLog("TimeLimit Import End:" + DateTime.Now.ToString());
            }

            //カット金型マスタ （※作業設備マスタ）
            bool complete_CutPressDie = true;
            if (checkBox_CutPressDie.Checked)
            {
                AppendLog("CutPressDie Import Start:" + DateTime.Now.ToString());
                complete_CutPressDie = CutPressDie.Import();
                AppendLog("CutPressDie Import End:" + DateTime.Now.ToString());
            }

            //検査数マスタ
            bool complete_DefectCt = true;
            if (checkBox_DefectCt.Checked)
            {
                AppendLog("DefectCt Import Start:" + DateTime.Now.ToString());
                complete_DefectCt = DefectCt.Import();
                AppendLog("DefectCt Import End:" + DateTime.Now.ToString());
            }

            //開封後/投入後期限マスタ
            bool complete_DBResinLife = true;
            if (checkBox_DBResinLife.Checked)
            {
                AppendLog("DBResinLife Import Start:" + DateTime.Now.ToString());
                complete_DBResinLife = DBResinLife.Import();
                AppendLog("DBResinLife Import End:" + DateTime.Now.ToString());
            }

            //水洗浄対象品目マスタ
            bool complete_DBWaferWashLife = true;
            if (checkBox_DBWaferWashLife.Checked)
            {
                AppendLog("DBWaferWashLife Import Start:" + DateTime.Now.ToString());
                complete_DBWaferWashLife = DBWaferWashedLife.Import();
                AppendLog("DBWaferWashLife Import End:" + DateTime.Now.ToString());
            }

            //製造条件マスタ
            bool complete_TypeCondtion = true;
            if (checkBox_TypeCondtion.Checked)
            {
                AppendLog("TypeCondtion Import start:" + DateTime.Now.ToString());
                complete_TypeCondtion = TypeCondition.Import();
                AppendLog("TypeCondtion Import End:" + DateTime.Now.ToString());
            }

            //DCブレード取付位置
            bool complete_General = true;
            if (checkBox_General.Checked)
            {
                AppendLog("General Import Start:" + DateTime.Now.ToString());
                complete_General = General.Import();
                AppendLog("General Import End:" + DateTime.Now.ToString());
            }

            //作業順マスタ （※取込設定が無効のラインは機能しません）
            bool complete_WorkFlow = true;
            if (checkBox_WorkFlow.Checked && Config.Settings.ImportWorkFlow)
            {
                AppendLog("workflow master import start:" + DateTime.Now.ToString());
                complete_WorkFlow = WorkFlows.Import();
                AppendLog("workflow master import end:" + DateTime.Now.ToString());
            }

            //資材ラベルマスタ
            bool complete_MatLabel = true;
            if (checkBox_MatLabel.Checked)
            {
                AppendLog("matlabel master import start:" + DateTime.Now.ToString());
                complete_MatLabel = MatLabel.Import();
                AppendLog("matlabel master import end:" + DateTime.Now.ToString());
            }

            //仕様に不足があり機能が未展開なので対象外。（コメントアウト）
            //AppendLog("defect judge start:" + DateTime.Now.ToString());
            //DefectJudge.Import();
            //AppendLog("defect judge end:" + DateTime.Now.ToString());

            //作業設備マスタ
            bool complete_WorkMacCond = true;
            if (checkBox_WorkMacCond.Checked)
            {
                AppendLog("WorkMacCond start:" + DateTime.Now.ToString());
                complete_WorkMacCond = WorkMacCondition.Import();
                AppendLog("WorkMacCond end:" + DateTime.Now.ToString());
            }

            //REDダイシェア抜取対象リスト
            bool complete_DieShearSampler = true;
            if (checkBox_DieShearSampler.Checked)
            {
                AppendLog("DieShearSampler start:" + DateTime.Now.ToString());
                complete_DieShearSampler = DieShearSampler.Import();
                AppendLog("DieShearSampler end:" + DateTime.Now.ToString());
            }

            //不良明細マスタ
            bool complete_defect = true;
            if (checkBox_defect.Checked)
            {
                AppendLog("defect start:" + DateTime.Now.ToString());
                complete_defect = Defect.Import(DEFECT_TARGETPASTHOURS);
                AppendLog("defect end:" + DateTime.Now.ToString());
            }

            Log.SysLog.Info("[ArmsNascaBridgeManual] End");

            if (!complete_TimeLimit || !complete_CutPressDie || !complete_DefectCt || !complete_DBResinLife
                || !complete_DBWaferWashLife || !complete_TypeCondtion || !complete_General || !complete_WorkFlow
                || !complete_MatLabel || !complete_WorkMacCond || !complete_DieShearSampler || !complete_defect)
            {
                MessageBox.Show("連携に失敗しました。システム管理者に連絡して下さい。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("連携が完了しました。", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
