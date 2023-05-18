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

namespace ArmsNascaBridge
{
    public partial class FrmBridgeMain : Form
    {
        BackgroundWorker bgw = new BackgroundWorker();

        NotifyIcon nicon;
        private bool close = false;

        public const string RESIN_MODE = "/resin";
        public const string WAFER_MODE = "/wafer";
        /// lot指定の場合は、プロファイル・資材・ロットの処理を行うモード
        public const string LOTIMPORT_MODE = "/lot";
        public const string RESTRICT_MODE = "/restrict";
        public const string BLEND_MODE = "/blend";
        public const string CANCELRESTRICTION_MODE = "/cancelrestriction";
        private List<string> commandLine;

        #region Singleton

        private static FrmBridgeMain instance;
        public static FrmBridgeMain GetInstance(string[] args)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new FrmBridgeMain(args);
            }
            return instance;

        }

        private FrmBridgeMain(string[] args)
        {
            InitializeComponent();
            this.commandLine = args.ToList();
        }
        #endregion

        #region FormLoad

        private void FrmBridgeMain_Load(object sender, EventArgs e)
        {
			//バージョン表示
			lblVersion.Text += Application.ProductVersion;
			
			nicon = new NotifyIcon();
            nicon.Icon = this.Icon;
            nicon.Text = "NASCA Bridge";
            nicon.Visible = true;
            nicon.ContextMenuStrip = this.contextMenuStrip1;
            nicon.DoubleClick += new EventHandler(nicon_DoubleClick);
            this.Hide();

        }
        #endregion

        public void MainLoop()
        {
            try
            {
                ActionEverytime(this.commandLine);
            }
            catch (Exception ex)
            {
				Log.SysLog.Error(string.Format("[ArmsNascaBridge] Error {0}\r\n{1}", ex.Message, ex.StackTrace));
			}
            finally
            {
                System.Threading.Thread.Sleep(10000);
            }
        }

		/// <summary>
		/// 5min間隔(タスク設定の時間)
		/// 各機能でエラーが発生した場合はエラーメールを送信して処理継続
		/// </summary>
        private void ActionEverytime(List<string> commandLine)
        {
            if (commandLine.Any() == true)
            {
                Log.SysLog.Info("[ArmsNascaBridge] Start(5min)(対象指定)");
                foreach (string cmd in commandLine)
                {
                    if (cmd == RESIN_MODE)
                    {
                        AppendLog("resin start:" + DateTime.Now.ToString());
                        Resin.Import();
                        AppendLog("resin end:" + DateTime.Now.ToString());
                    }
                    else if (cmd == WAFER_MODE)
                    {
                        AppendLog("wafer start:" + DateTime.Now.ToString());
                        Wafer.Import();
                        AppendLog("wafer end:" + DateTime.Now.ToString());
                    }
                    else if (cmd == LOTIMPORT_MODE)
                    {
                        // Lot は Profile, matより後に実行する必要がある為セット実行とする
                        AppendLog("profile start:" + DateTime.Now.ToString());
                        Profiles.Import();
                        AppendLog("profile end:" + DateTime.Now.ToString());
                        AppendLog("mat start:" + DateTime.Now.ToString());
                        Material.Import();
                        AppendLog("mat end:" + DateTime.Now.ToString());
                        AppendLog("mel lot import start:" + DateTime.Now.ToString());
                        LotImport.Import();
                        AppendLog("mel lot import end:" + DateTime.Now.ToString());
                    }
                    else if (cmd == RESTRICT_MODE)
                    {
                        AppendLog("restrict start:" + DateTime.Now.ToString());
                        ArmsApi.Model.Restrict.CancelDieShaerRestrict();
                        AppendLog("restrict end:" + DateTime.Now.ToString());
                    }
                    else if (cmd == BLEND_MODE)
                    {
                        AppendLog("blend start:" + DateTime.Now.ToString());
                        Blend.Import();
                        AppendLog("blend end:" + DateTime.Now.ToString());
                    }
                    else if (cmd == CANCELRESTRICTION_MODE)
                    {
                        AppendLog("cancelrestriction start:" + DateTime.Now.ToString());
                        Restrict.CancelRestriction();
                        AppendLog("cancelrestriction end:" + DateTime.Now.ToString());
                    }
                }
                Log.SysLog.Info("[ArmsNascaBridge] End(5min)(対象指定)");
            }
            else
            {
                Log.SysLog.Info("[ArmsNascaBridge] Start(5min)");

                AppendLog("profile start:" + DateTime.Now.ToString());
                Profiles.Import();
                AppendLog("profile end:" + DateTime.Now.ToString());

                AppendLog("resin start:" + DateTime.Now.ToString());
                Resin.Import();
                AppendLog("resin end:" + DateTime.Now.ToString());

                AppendLog("wafer start:" + DateTime.Now.ToString());
                Wafer.Import();
                AppendLog("wafer end:" + DateTime.Now.ToString());

                AppendLog("mat start:" + DateTime.Now.ToString());
                Material.Import();
                AppendLog("mat end:" + DateTime.Now.ToString());

                // Lot は Profile, matより後に実行する
                AppendLog("mel lot import start:" + DateTime.Now.ToString());
                LotImport.Import();
                AppendLog("mel lot import end:" + DateTime.Now.ToString());

                AppendLog("restrict start:" + DateTime.Now.ToString());
                ArmsApi.Model.Restrict.CancelDieShaerRestrict();
                AppendLog("restrict end:" + DateTime.Now.ToString());

                AppendLog("blend start:" + DateTime.Now.ToString());
                Blend.Import();
                AppendLog("blend end:" + DateTime.Now.ToString());

                AppendLog("cancelrestriction start:" + DateTime.Now.ToString());
                Restrict.CancelRestriction();
                AppendLog("cancelrestriction end:" + DateTime.Now.ToString());

                Log.SysLog.Info("[ArmsNascaBridge] End(5min)");
            }
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
            if (close == false)
            {
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
                e.Cancel = true;
                return;
            }
            nicon.Dispose();
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
            Action act = new Action(MainLoop);
            act.BeginInvoke(new AsyncCallback(Complete), act);
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
                Log.SysLog.Error("NASCA連携エラー：" + ex.ToString());
            }
            finally
            {
                this.close = true;
                this.Invoke(new Action(this.Close));
            }
        }
    }
}
