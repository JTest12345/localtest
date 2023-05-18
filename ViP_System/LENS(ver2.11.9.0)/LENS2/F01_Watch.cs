﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LENS2.Machine;
using LENS2_Api;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using PLC;
using System.Media;
using System.Net;
namespace LENS2
{
	public partial class F01_Watch : Form
	{
		private bool isClose = false;

		/// <summary>監視中のスレッドリスト</summary>
		private Dictionary<string, BackgroundWorker> workers = new Dictionary<string, BackgroundWorker>();

		/// <summary>スレッドロック</summary>
		static public System.Object lockThis = new System.Object();

		/// <summary>異常音</summary>
		public static SoundPlayer errorSound = new System.Media.SoundPlayer(Application.StartupPath + @"\ringin.wav");

		static F01_Watch instance;

		public F01_Watch()
		{
			InitializeComponent();
			instance = this;
		}

		private void F01_Watch_Load(object sender, EventArgs e)
		{
			//バージョン表示
			toolTxtVersion.Text = Application.ProductVersion;
			toolTxtMachineGrp.Text = Config.Settings.MachineGroupCd;
			
			Machine.MachineBase.CommonHideLog(string.Format("Ver:{0} 起動", Application.ProductVersion));

			string hostNm = string.Empty;
			try{
				hostNm = Dns.GetHostName();
			}
			catch(Exception err)
			{
				Machine.MachineBase.CommonHideLog("DNSからホスト名取得に失敗\r\n" + err.Message);
			}

			this.Text = string.Format("{0} on {1}", this.Text, hostNm);

			SetDebugFg();

			//監視装置取得
			List<MachineInfo> machines = MachineInfo.GetDatas(string.Empty, Config.Settings.MachineGroupCd);
			foreach(MachineInfo machine in machines)
			{
				addMachineTree(machine.NascaPlantCd, machine.NascaPlantCd, machine.ClassNm, machine.MachineNm);
			}

#if 工程内検証
            tslDebugMsg1.Visible = true;
            tslDebugMsg2.Visible = true;
#endif
		}

		private void F01_Watch_FormClosing(object sender, FormClosingEventArgs e)
		{
            isClose = true;
			if (isClose == true)
			{
				if (workers.Count != 0)
				{
					foreach (BackgroundWorker w in workers.Values) 
					{
						w.CancelAsync();
					}
					MessageBox.Show("停止処理中の為、暫く経ってから再度終了して下さい。");

					return;
				}

				F02_Confirm confirm = new F02_Confirm();
				confirm.ShowDialog();

				if (confirm.IsCompletedConfirm)
				{
					notifyIcon.Dispose();
				}
				else 
				{
					e.Cancel = true;
					isClose = false;
					return;
				}
			}
			else
			{
				ChangeVisible(false);
				e.Cancel = true;
				return;
			}
		}



		private void btnSoundStop_Click(object sender, EventArgs e)
		{
			errorSound.Stop();
		}

		#region ツールバー

		private void toolApplicationExit_Click(object sender, EventArgs e)
		{
			isClose = true;
			this.Close();
		}

		/// <summary>
		/// 監視開始
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStartMonitoring_Click(object sender, EventArgs e)
		{
			if (DialogResult.OK != MessageBox.Show("装置監視を開始します。よろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
			{
				return;
			}

			foreach(TreeNode node in tvMachine.Nodes)
			{
				threadStart(node.Name);
			}

			toolMachineWatchStatus.Text = "監視中";
			toolMachineWatchStatus.ForeColor = Color.Green;

			this.Hide();
		}

		/// <summary>
		/// 監視停止
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolStopMonitoring_Click(object sender, EventArgs e)
		{
			if (DialogResult.OK != MessageBox.Show("装置監視を停止します。よろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
			{
				return;
			}

			foreach (BackgroundWorker worker in workers.Values)
			{
				worker.CancelAsync();
			}

			toolMachineWatchStatus.Text = "監視を停止している装置が無いか確認して下さい。";
			toolMachineWatchStatus.ForeColor = Color.Red;
		}

		/// <summary>
		/// データメンテナンス - 不良修正
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolMachineDefectMainte_Click(object sender, EventArgs e)
		{
			F03_MachineDefectMainte form = new F03_MachineDefectMainte();
			form.ShowDialog();
		}

		#endregion

		#region 監視装置ショートカットメニュー

		private void menuWorkerStartMonitoring_Click(object sender, EventArgs e)
		{
			threadStart(tvMachine.SelectedNode.Name);
		}

		private void menuWorkerStopMonitoring_Click(object sender, EventArgs e)
		{
			workers[tvMachine.SelectedNode.Name].CancelAsync();
		}

		#endregion

		#region タスクバーアイコンショートカットメニュー

		private void notifyIcon_DoubleClick(object sender, EventArgs e)
		{
			ChangeVisible(true);
		}

		private void menuTaskApplicationExit_Click(object sender, EventArgs e)
		{
			isClose = true;
			this.Close();
		}

		private void menuTaskStartMonitoring_Click(object sender, EventArgs e)
		{
			if (DialogResult.OK != MessageBox.Show("装置監視を開始します。よろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
			{
				return;
			}

			foreach (string workmac in workers.Keys)
			{
				threadStop(workmac);
			}
		}

		private void menuTaskStopMonitoring_Click(object sender, EventArgs e)
		{
			if (DialogResult.OK != MessageBox.Show("装置監視を停止します。よろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
			{
				return;
			}

			foreach (TreeNode node in tvMachine.Nodes)
			{
				threadStart(node.Name);
			}
		}

		#endregion

		private void threadStart(string threadname)
		{
			if (workers.ContainsKey(threadname)) 
			{
				//既に開始している場合は処理抜け
				return;
			}

			BackgroundWorker workmac = new BackgroundWorker();
			workmac.WorkerSupportsCancellation = true;
			workmac.DoWork += new DoWorkEventHandler(workerMachine_DoWork);
			workmac.RunWorkerAsync(threadname);

			lock (lockThis)
			{
				workers.Add(threadname, workmac);
			}
		}

		private void threadStop(string threadname) 
		{
			workers[threadname].CancelAsync();
		}

		private void workerMachine_DoWork(object sender, DoWorkEventArgs e) 
		{
			string plantcd = e.Argument.ToString().Trim();
			MachineInfo machine = MachineInfo.GetData(plantcd);

			ChangeWorkerMode(plantcd, true);

			Log.Info(machine.ClassNm, machine.MachineNm, "監視開始", true);

			try
			{
				IMachine mac;
				switch (machine.ClassCd)
				{
					case "Wirebonder":
						string raw = File.ReadAllText(Path.Combine(Properties.Settings.Default.ConfigFilePath, "Wirebonder.xml"), Encoding.UTF8);
						mac = JsonConvert.DeserializeObject<Wirebonder>(raw);
						break;

					case "Inspector":
						raw = File.ReadAllText(Path.Combine(Properties.Settings.Default.ConfigFilePath, "Inspector.xml"), Encoding.UTF8);
						mac = JsonConvert.DeserializeObject<Inspector>(raw);
						break;

					case "Mold":
						raw = File.ReadAllText(Path.Combine(Properties.Settings.Default.ConfigFilePath, "Mold.xml"), Encoding.UTF8);
						mac = JsonConvert.DeserializeObject<Mold>(raw);

						break;

					default:
						throw new ApplicationException(string.Format("想定外の装置種類が存在します。装置種類:{0}", machine.ClassNm));
				}
				mac.NascaPlantCD = machine.NascaPlantCd;
				mac.ClassNM = machine.ClassNm;
				mac.MachineNM = machine.MachineNm;
				mac.WatchingDirectoryPath = machine.WatchingDirectoryPath;

				mac.Plc = new Keyence(machine.PlcIpAddress, machine.PlcPort);

				while (!((BackgroundWorker)sender).CancellationPending)
				{
					mac.MainWork();

					//ChangeMachineWorkStatus(mac.MachineNO, mac.WorkStatus);

					Thread.Sleep(1000);
				}
			}
			catch(MachineException)	
			{
				errorSound.PlayLooping();
			}
			catch(Exception err)
			{
				errorSound.PlayLooping();
				Log.Error(machine.ClassNm, machine.MachineNm, 
					string.Format(@"{0} {1}", err.Message, err.StackTrace), true);
			}
			finally 
			{
				Log.Info(machine.ClassNm, machine.MachineNm, "監視停止", true);
				lock (lockThis)
				{
					workers.Remove(plantcd);
				}
				ChangeWorkerMode(plantcd, false);
				ChangeVisible(true);
			}
		}

		private void ctmWorker_Opening(object sender, CancelEventArgs e)
		{
			if (tvMachine.SelectedNode == null)
			{
				e.Cancel = true;
				return;
			}

			if (workers.ContainsKey(tvMachine.SelectedNode.Name))
			{
				menuWorkerStartMonitoring.Enabled = false;
				menuWorkerStopMonitoring.Enabled = true;
			}
			else 
			{
				menuWorkerStartMonitoring.Enabled = true;
				menuWorkerStopMonitoring.Enabled = false;
			}
		}

		private void addMachineTree(string plantcd, string machinecd, string classnm, string machinenm) 
		{
			tvMachine.Nodes.Add(plantcd, string.Format("{0} {1}({2})", classnm, machinenm, machinecd), "wave_stop.ico");
		}

		static Action<int, string> machineWorkStatusAction = new Action<int, string>(ChangeMachineWorkStatus);
		public static void ChangeMachineWorkStatus(int macno, string status)
		{
			if (instance == null) return;
			if (instance.InvokeRequired)
			{
				instance.Invoke(machineWorkStatusAction, macno, status);
			}
			else 
			{
				string machineText = instance.tvMachine.Nodes[macno.ToString()].Text;
				if (machineText.Contains("_"))
				{
					instance.tvMachine.Nodes[macno.ToString()].Text 
						= string.Format("{0}_{1}", string.Join("", machineText.Take(machineText.IndexOf("_"))), status) ;
				}
				else
				{
					instance.tvMachine.Nodes[macno.ToString()].Text 
						= string.Format("{0}_{1}", machineText, status);
				}
			}
		}

		static Action<Image, string, string, string> logAction = new Action<Image, string, string, string>(WriteLog);
		public static void WriteLog(Image infotype, string classnm, string machinenm, string message)
		{
			if (instance == null) return;

			if (instance.InvokeRequired)
			{
				instance.Invoke(logAction, infotype, classnm, machinenm, message);
			}
			else
			{
				if (instance.bsLog.Count > Properties.Settings.Default.ScreenLogLimit)
				{
					instance.bsLog.RemoveAt(0);
				}

				ScreenLog log = new ScreenLog();
				log.InfoType = infotype;
				log.LastUpdDT = System.DateTime.Now;
				log.ClassNM = classnm;
				log.MachineNM = machinenm;
				log.Message = message;
				instance.bsLog.Add(log);

                instance.dgvLog.FirstDisplayedScrollingRowIndex = instance.dgvLog.Rows.Count - 1;
			}
		}

		static Action<bool> visibleAction = new Action<bool>(ChangeVisible);
		public static void ChangeVisible(bool isVisible)
		{
			if (instance == null) return;

			if (instance.InvokeRequired)
			{
				instance.Invoke(visibleAction, isVisible);
			}
			else
			{
				if (isVisible)
				{
					instance.Show();
					instance.WindowState = FormWindowState.Normal;
					instance.Activate();
				}
				else
				{
					instance.Hide();
					instance.WindowState = FormWindowState.Minimized;
				}
			}
		}

		static Action<string, bool> workerModeAction = new Action<string, bool>(ChangeWorkerMode);
		public static void ChangeWorkerMode(string nodeKey, bool isRunning) 
		{
			if (instance == null) return;

			if (instance.InvokeRequired)
			{
				instance.Invoke(workerModeAction, nodeKey, isRunning);
			}
			else
			{
				if (isRunning)
				{
					instance.tvMachine.Nodes[nodeKey].ImageKey = "wave_run.ico";
					instance.tvMachine.Nodes[nodeKey].SelectedImageKey = "wave_run.ico";
				}
				else
				{
					instance.tvMachine.Nodes[nodeKey].ImageKey = "wave_stop.ico";
					instance.tvMachine.Nodes[nodeKey].SelectedImageKey = "wave_stop.ico";
				}
			}
		}

		public class ScreenLog
		{
			public Image InfoType { get; set; }
			public DateTime LastUpdDT { get; set; }
			public string ClassNM { get; set; }
			public string MachineNM { get; set; }
			public string Message { get; set; }
			public int MessageNO { get; set; }
		}

		private void tmChkDebugLogFg_Tick(object sender, EventArgs e)
		{
			SetDebugFg();
		}

		private void SetDebugFg()
		{
			bool debugLogFg, debugModeFg;

			if (File.Exists(@"C:\LENS\Config\DEBUGLOGOUT"))
			{
				debugLogFg = true;
			}
			else
			{
				debugLogFg = false;
			}

			if (File.Exists(@"C:\LENS\Config\DEBUGMODE"))
			{
				debugModeFg = true;
			}
			else
			{
				debugModeFg = false;
			}

			Config.Settings.DebugLogOutFg = debugLogFg;
			//toolCompOutMachineLogMainte.Visible = debugModeFg;
		}

		private void toolCompOutMachineLogMainte_Click(object sender, EventArgs e)
		{
			F91_MachineLogOutputCompletionMainte form = new F91_MachineLogOutputCompletionMainte();
			form.ShowDialog();
		}

		private void toolTxtVersion_Click(object sender, EventArgs e)
		{
			SetDebugFg();
		}

		private void F01_Watch_FormClosed(object sender, FormClosedEventArgs e)
		{
			notifyIcon.Dispose();
		}
	}
}