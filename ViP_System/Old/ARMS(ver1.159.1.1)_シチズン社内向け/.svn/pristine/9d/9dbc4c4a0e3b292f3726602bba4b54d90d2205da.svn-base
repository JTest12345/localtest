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

namespace ArmsNascaBridge3
{
	public partial class FrmBridgeMain : Form
	{
		BackgroundWorker bgw = new BackgroundWorker();

		NotifyIcon nicon;
		private bool close = false;

		#region Singleton

		private static FrmBridgeMain instance;
		public static FrmBridgeMain GetInstance()
		{
			if (instance == null || instance.IsDisposed)
			{
				instance = new FrmBridgeMain();
			}
			return instance;

		}

		private FrmBridgeMain()
		{
			InitializeComponent();
		}
		#endregion

		#region FormLoad

		private void FrmBridgeMain_Load(object sender, EventArgs e)
		{
			//バージョン表示
			lblVersion.Text += Application.ProductVersion;

			nicon = new NotifyIcon();
			nicon.Icon = this.Icon;
			nicon.Text = "NASCA Bridge3";
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
				ActionEverytime();
			}
			catch (Exception ex)
			{
				Log.SysLog.Error(string.Format("[ArmsNascaBridge3] Error {0}\r\n{1}", ex.Message, ex.StackTrace));
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
		private void ActionEverytime()
		{
			Log.SysLog.Info("[ArmsNascaBridge3] Start");

			AppendLog("Wafer start:" + DateTime.Now.ToString());
			Material.Import();
			AppendLog("Wafer end:" + DateTime.Now.ToString());

			Log.SysLog.Info("[ArmsNascaBridge3] End");
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

				instance.txtLog.AppendText(DateTime.Now.ToString() + ": " + txt + "\r\n");

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
