using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Xml;
using System.IO;
using System.Linq;
using EICS.Machine;
using EICS.Database;
using System.Net;

namespace EICS
{
    public partial class F01_MachineWatch : Form
    {
        public enum FormStyle 
        {
            Neutral,
            ContactError
        }

        /// <summary>サイレン(閾値越えｴﾗｰ)</summary>
        public static System.Media.SoundPlayer sp = new System.Media.SoundPlayer(Application.StartupPath + @"\Component\se_mod02.wav");

        /// <summary>サイレン(装置通信ｴﾗｰ)</summary>
        public static System.Media.SoundPlayer spMachine = new System.Media.SoundPlayer(Application.StartupPath + @"\Component\ringin.wav");

        /// <summary>実行中スレッドリスト</summary>
        private Dictionary<string, BackgroundWorker> tranList = new Dictionary<string, BackgroundWorker>();

		private Dictionary<string, Constant.PatLamp> stopReservationMachineList = new Dictionary<string, Constant.PatLamp>();

		/// <summary>ログメッセージをキューから取り出して表示する為のスレッド</summary>
		private BackgroundWorker logIndicator = new BackgroundWorker();
		private BackgroundWorker alertLogIndicator = new BackgroundWorker();

        /// <summary>パトランプ変更デリゲート</summary>
        /// <param name="plantCD">設備CD</param>
        /// <param name="patLamp">変更するパトランプ</param>
        public delegate void ChangePatLampDelegate(string plantCD, Constant.PatLamp patLamp);

        /// <summary>異常ログメッセージ追加デリゲート</summary>
        /// <param name="message">追加するメッセージ</param>
        /// <param name="color">フォント色</param>
        public delegate void AddNGMessageDelegate(string message, Color color);

		/// <summary>動作ログメッセージ追加デリゲート</summary>
		/// <param name="message">追加するメッセージ</param>
		/// <param name="color">フォント色</param>
		public delegate void AddOKMessageDelegate(string message, Color color);

        /// <summary>ツリーテキスト変更デリゲート</summary>
        /// <param name="machineNM"></param>
        /// <param name="equipmentNO"></param>
        /// <param name="typeCD"></param>
        /// <param name="chipNM"></param>
        public delegate void ChangeTreeTextDelegate(string machineNM, string equipmentNO, string typeCD, string chipNM);

        /// <summary>スレッドロック</summary>
        static public System.Object lockThis = new System.Object();

		List<IMachine> machineList = new List<IMachine>();

        /// <summary>コンストラクタ</summary>
        public F01_MachineWatch()
        {
            InitializeComponent();
        }

        /// <summary>フォームロード時</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void G001_MachineWatch_Load(object sender, EventArgs e)
        {
#if DEBUG
			this.Text = this.Text += "#if DEBUG";
#elif DEBUG_PLC
			this.Text = this.Text += "#if DEBUG_PLC";
#endif

			try
            {
				toolCheckDBConnectInfo.Visible = SettingInfo.HasDebugSetting();

				logIndicator.DoWork += new DoWorkEventHandler(OutputRunningLog);
				logIndicator.WorkerSupportsCancellation = true;
				logIndicator.RunWorkerAsync();

				alertLogIndicator.DoWork += new DoWorkEventHandler(OutputAlertLog);
				alertLogIndicator.WorkerSupportsCancellation = true;
				alertLogIndicator.RunWorkerAsync();

				tmRemoteErrReceiver.Interval = SettingInfo.GetErrChkIntervalSec() * 1000;
				tmRemoteErrReceiver.Start();

                ChangeFormStyle(FormStyle.Neutral);

                //装置リスト表示
                showMachineTree();

				TmTypeChanger.Enabled = true;
            }
            catch (ApplicationException err) 
            {
                AddNGMessageInvoke(err.Message, Color.Red);
                ChangeFormStyle(FormStyle.ContactError);
            }
            catch (Exception err)
            {
                AddNGMessageInvoke(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), Color.Red);
                ChangeFormStyle(FormStyle.ContactError);
            }
        }

        /// <summary>装置一覧を表示</summary>
		private void showMachineTree()
		{
			SettingInfo settingInfo = SettingInfo.GetSingleton();

			foreach (SettingInfo settingInfoPerLine in settingInfo.SettingInfoList)
			{
#if Debug
				//equipList.RemoveAll(e => e.EquipmentNO != "");
#else
				//起動必須ソフトの確認
				if (!checkSoftware(settingInfoPerLine))
				{
					ChangeFormStyle(FormStyle.ContactError);
				}
#endif

				//装置のリスト取得
				List<EquipmentInfo> equipList = ConnectDB.GetEquipmentList(settingInfoPerLine.LineCD);
				if (equipList.Count == 0)
				{
					throw new ApplicationException(Constant.MessageInfo.Message_35);
				}

				//設定ファイルから装置型番、チップを取得
				//equipList = getEquipSettingData(settingInfoPerLine.LineCD, ref equipList);

				foreach (EquipmentInfo equipmentInfo in equipList)
				{
					equipmentInfo.TypeCD = settingInfoPerLine.GetMaterialCD(equipmentInfo.EquipmentNO);
					if (equipmentInfo.TypeCD == "" && settingInfoPerLine.GetUnSelectableTypeFG(equipmentInfo.EquipmentNO) == false)
					{
						AddNGMessageInvoke(Constant.MessageInfo.Message_16 + equipmentInfo.EquipmentNO + " " + Constant.MessageInfo.Message_20, Color.Red);
					}
					equipmentInfo.ChipNM = settingInfoPerLine.GetChipNM(equipmentInfo.EquipmentNO);
					//ﾀﾞｲﾎﾞﾝﾀﾞｰの場合チップの設定がされていない場合はError
					if (equipmentInfo.ChipNM == "" && equipmentInfo.AssetsNM == DBMachineInfo.ASSETS_NM)
					{
						AddNGMessageInvoke(Constant.MessageInfo.Message_16 + equipmentInfo.EquipmentNO + " " + Constant.MessageInfo.Message_21, Color.Red);
					}

                    equipmentInfo.TypeGroupCD = settingInfoPerLine.GetTypeGroupCD(equipmentInfo.EquipmentNO);
				}

				List<string> processList = new List<string>();

				foreach (EquipmentInfo equipmentInfo in equipList)
				{
#if DEBUG
					//if (equipmentInfo.EquipmentNO != "S07578")
					//{
					//    continue;
					//}
#endif
					if (!processList.Exists(p => p == equipmentInfo.AssetsNM))
					{
						processList.Add(equipmentInfo.AssetsNM);
					}
				}

				List<TreeNode> tnProcessList = new List<TreeNode>();

				//項目の追加(設備)
				foreach (string process in processList)
				{
					List<TreeNode> tnMachineList = new List<TreeNode>();
					foreach(EquipmentInfo equipmentInfo in equipList.FindAll(e => e.AssetsNM == process))
					{
						TreeNode tnItem;

						tnItem = new TreeNode(equipmentInfo.MachineNM + Constant.MessageInfo.Message_49 + "(" + equipmentInfo.EquipmentNO + ") " + equipmentInfo.DisplayTypeCD + " " + equipmentInfo.ChipNM);

						tnItem.Name = equipmentInfo.EquipmentNO;
						
						tnMachineList.Add(tnItem);//lvMachine.Items.Add(equipmentInfo.EquipmentNO, ItemNM, (int)Constant.PatLamp.Yellow);						
						
						//各Machineのインスタンスのリストを作成
						machineList.Add(MachineBase.GetMachineInfo(equipmentInfo));
					}
					tnProcessList.Add(new TreeNode(process, tnMachineList.ToArray()));
				}
				TreeNode tnLine = new TreeNode(settingInfoPerLine.LineCD.ToString(), tnProcessList.ToArray());
				tvMachine.Nodes.Add(tnLine);
			}
			tvMachine.Sort();
		}


        /// <summary>必須起動ソフトの確認</summary>
        private bool checkSoftware(SettingInfo settingInfoPerLine)
        {
            bool status = true;

			settingInfoPerLine.KissFG = "ON";

			//WBはKISSの起動確認
			if (settingInfoPerLine.KissFG == "ON")
            {
                if (!KLinkInfo.CheckKISS())
                {
                    status = false;
                    sp.PlayLooping();
                    AddNGMessageInvoke(string.Format(Constant.MessageInfo.Message_6, settingInfoPerLine.LineCD), Color.Red);
                }
            }

            //AI,MD,PLA,ECKはBlackJumbDogの起動確認
			if (settingInfoPerLine.BlackJumboDogFG == "ON")
            {
                if (!KLinkInfo.CheckBlackJumboDog())
                {
                    status = false;
                    sp.PlayLooping();
                    AddNGMessageInvoke(string.Format(Constant.MessageInfo.Message_14, settingInfoPerLine.LineCD), Color.Red);
                }
            }

            return status;
        }


		//EICS3で削除

		///// <summary>
		///// 設定ファイルから装置型番を取得
		///// </summary>
		///// <param name="equipList"></param>
		//private void getEquipSettingData(int lineCD, ref List<EquipmentInfo> equipList)
		//{
		//    SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(lineCD);
		//}


        /// <summary>項目(設備)押下時</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void tvMachine_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
			if ((e.Button != MouseButtons.Left) || (((TreeView)sender).SelectedNode == null))
			{
				return;
			}

			TreeNode selectedNode = ((TreeView)sender).SelectedNode;

			int lineCD = GetLineNoFromNode(selectedNode);

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);

			if(settingInfoPerLine.EquipmentList.Exists(equipInfo => equipInfo.EquipmentNO == selectedNode.Name))
			{
				//装置単位の設定画面を開いての処理
				OpenMachineSettingPerMachine(selectedNode);
			}

			AllTreeNodeExpandAndCollapse(selectedNode);

        }

		private void OpenMachineSettingPerMachine(TreeNode tnMachine)
		{
			string itemText = tnMachine.Text;
			string equipmentNO = tnMachine.Name;

			int lineCD = GetLineNoFromNode(tnMachine);

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);

			//設備情報を取得
			EquipmentInfo equipInfoFromDB = ConnectDB.GetEquipmentData(lineCD, equipmentNO);
			//equipmentInfo.TypeCD = settingInfoPerLine.GetMaterialCD(equipmentNO);
			//equipmentInfo.ChipNM = settingInfoPerLine.GetChipNM(equipmentNO);
			//equipmentInfo.BMCountFG = settingInfoPerLine.GetBMCount(equipmentNO);
			//equipmentInfo.UnSelectableTypeFG = settingInfoPerLine.GetUnSelectableTypeFG(equipmentNO);
			//equipmentInfo.IsOutputNasFile = settingInfoPerLine.IsOutputNasFile(equipmentNO);

			EquipmentInfo equipmentInfo = settingInfoPerLine.GetEquipInfo(equipmentNO);

			equipmentInfo.AssetsNM = equipInfoFromDB.AssetsNM;
			equipmentInfo.InputFolderNM = equipInfoFromDB.InputFolderNM;
			equipmentInfo.MachineNM = equipInfoFromDB.MachineNM;
			equipmentInfo.ModelNM = equipInfoFromDB.ModelNM;


			//設備状態を取得
			Constant.MachineStatus machineStatus = Constant.MachineStatus.Wait;
			if (tranList.ContainsKey(equipmentInfo.EquipmentNO))
			{
				//実行中
				machineStatus = Constant.MachineStatus.Runtime;
			}
			else
			{
				//停止
				machineStatus = Constant.MachineStatus.Wait;
			}

			//装置設定画面を開く
			F02_MachineSetting g002 = new F02_MachineSetting(lineCD, equipmentInfo, machineStatus);
			g002.ShowDialog();

			//停止が押された場合
			if (g002.StopFG)
			{
				BackgroundWorker bgWorkerInfo = null;
				bool isRunning = tranList.TryGetValue(equipmentInfo.EquipmentNO, out bgWorkerInfo);

				if (isRunning)
				{
					bgWorkerInfo.CancelAsync();
				}
			}
			//開始が押された場合
			else if (g002.StartFG)
			{
				BackgroundWorker tran = new BackgroundWorker();
				tran.DoWork += new DoWorkEventHandler(tran_DoWork);
				tran.WorkerSupportsCancellation = true;

				//string paramVAL = equipmentInfo.EquipmentNO;
				if (machineList.FindAll(m => m.Code == equipmentInfo.EquipmentNO).Count != 1)
				{
					throw new Exception();
				}

				IMachine machineInstance = machineList.Find(m => m.Code == equipmentInfo.EquipmentNO);

				

				tran.RunWorkerAsync(machineInstance);
				lock (lockThis)
				{
					tranList.Add(equipmentInfo.EquipmentNO, tran);
				}
			}

			//型番、チップが変更された場合、ListViewも更新する
			if (g002.ChangeFG)
			{
				string ItemNM = equipmentInfo.MachineNM + Constant.MessageInfo.Message_49 + "(" + equipmentInfo.EquipmentNO + ") " + g002.TypeCD + " " + g002.ChipNM;
				tnMachine.Text = ItemNM;
				settingInfoPerLine.GetSettingData(Constant.SETTING_FILE_PATH);
			}
		}

        /// <summary>設備毎ファイル処理、通信処理スレッド</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tran_DoWork(object sender, DoWorkEventArgs e)
        {
			string plantCD = ((IMachine)e.Argument).Code;

			ChangePatLampInvoke(plantCD, Constant.PatLamp.Green);

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(((IMachine)e.Argument).LineCD);

			//設備情報を取得
			LSETInfo lsetInfo = ConnectDB.GetLSETInfo(((IMachine)e.Argument).LineCD, plantCD);
            //lsetInfo.ChipNM = settingInfoPerLine.GetChipNM(plantCD);
			lsetInfo.DirWBMagazine = settingInfoPerLine.GetWBMagazineDir(plantCD);
			lsetInfo.ReportType = settingInfoPerLine.GetReportType(plantCD);
			lsetInfo.EquipInfo = settingInfoPerLine.GetEquipInfo(lsetInfo.EquipmentNO);

			try
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("監視開始 {0} {1} {2}号機", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO));
				BackgroundWorker tran = ((BackgroundWorker)sender);
				switch (lsetInfo.ReportType)
				{
					//ログファイル処理
					case Constant.ReportType.LogFile:
						CheckMachineFile(e, tran, lsetInfo);
						break;

					//通信メッセージ処理
					case Constant.ReportType.HSMS:
						CheckReportMessage(e, tran, lsetInfo);
						break;
				}
			}
			catch (IOException err)
			{
				string message = string.Format(Constant.MessageInfo.Message_42, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO) + err.Message;
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, message);
				AddNGMessageInvoke(message, Color.Red);

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("監視停止 {0} {1} {2}号機", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO));
				ChangePatLampInvoke(lsetInfo.EquipmentNO, Constant.PatLamp.Red);
				sp.PlayLooping();
				((BackgroundWorker)sender).CancelAsync();
			}
			catch (ApplicationException err)
			{
				string message = string.Format(Constant.MessageInfo.Message_42, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO) + err.Message;
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, message);
				AddNGMessageInvoke(message, Color.Red);

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("監視停止 {0} {1} {2}号機", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO));
				ChangePatLampInvoke(lsetInfo.EquipmentNO, Constant.PatLamp.Red);
				sp.PlayLooping();
				((BackgroundWorker)sender).CancelAsync();
			}
			catch (Exception err)
			{
				string message = string.Format(Constant.MessageInfo.Message_42, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO) + err.Message;
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, message);
				string exceptionMsg = string.Format("EICS3 PC名:{0} ライン：{1} 装置：{2}/{3}[{4}]\r\nエラー内容{5}: {6} / {7}", Dns.GetHostName(), ((IMachine)e.Argument).LineCD, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO, plantCD, err.Message, err.Source, err.StackTrace);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, exceptionMsg);
				AddNGMessageInvoke(message, Color.Red);

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("監視停止 {0} {1} {2}号機", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO));
				ChangePatLampInvoke(lsetInfo.EquipmentNO, Constant.PatLamp.Red);
				sp.PlayLooping();
				((BackgroundWorker)sender).CancelAsync();
			}
			finally
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("監視停止 {0} {1} {2}号機", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO));

				Constant.PatLamp patLamp;
				if(stopReservationMachineList.TryGetValue(lsetInfo.EquipmentNO, out patLamp) == false)
				{
					patLamp = Constant.PatLamp.Yellow;
				}

				ChangePatLampInvoke(lsetInfo.EquipmentNO, patLamp);

				lock (lockThis)
				{
					removeFromRunnningMachineList(plantCD);
				}

			}
        }

		private void removeFromRunnningMachineList(string plantCD)
		{
			tranList.Remove(plantCD);
			stopReservationMachineList.Remove(plantCD);
		}

		private void OutputRunningLog(object sender, DoWorkEventArgs e)
		{
			RunningLog runningLog = RunningLog.GetInstance();

			while (!logIndicator.CancellationPending)
			{
				while (runningLog.logMessageQue.Count != 0)
				{
					lock (RunningLog.lockThis)
					{
						AddOKMessageInvoke(string.Format("{0} : {1}", DateTime.Now, runningLog.logMessageQue.Dequeue()), Color.Black);
					}
					Thread.Sleep(50);
				}
				Thread.Sleep(500);
			}
		}

		//警告ログ表示
		private void OutputAlertLog(object sender, DoWorkEventArgs e)
		{
			
			AlertLog alertLog = AlertLog.GetInstance();

			while (!alertLogIndicator.CancellationPending)
			{
				bool alermFG = false;
				while (alertLog.logMessageQue.Count != 0)
				{
					lock (AlertLog.lockThis)
					{
						string outputMsg = alertLog.logMessageQue.Dequeue();
						AddNGMessageInvoke(outputMsg, Color.Red);
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, outputMsg);
						alermFG = true;
					}
					Thread.Sleep(50);
				}

				if (alermFG)
				{
					sp.PlayLooping();
				}

				Thread.Sleep(500);
			}
		}

        /// <summary>ログファイル処理</summary>
        /// <param name="lsetInfo"></param>
        /// <param name="machineStatus"></param>
        /// <param name="errMessageList"></param>
        /// <param name="frameSupplyStatus"></param>
        public void CheckMachineFile(DoWorkEventArgs e, BackgroundWorker tran, LSETInfo lsetInfo) 
        {
           // Constant.FrameSupplyStatus frameSupplyStatus = Constant.FrameSupplyStatus.Wait;   //フレーム供給状態　0=NG　1=OK

			((IMachine)e.Argument).InitFirstLoop(lsetInfo);

            //停止命令を行うまでスレッドをループ
            while (!tran.CancellationPending)
            {
				bool alermOnFg = false;

				SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
				lsetInfo.TypeCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);

                string displayType = lsetInfo.TypeCD;
                string typeGroupCD = settingInfo.GetTypeGroupCD(lsetInfo.EquipmentNO);
                if (string.IsNullOrEmpty(typeGroupCD) == false)
                {
                    displayType = typeGroupCD;
                }

                //外部(PDA)から設定ファイルの変更があった場合、ツリーをリフレッシュ
                ChangeTreeTextInvoke(lsetInfo.MachineNM, lsetInfo.EquipmentNO, displayType, lsetInfo.ChipNM);

                ((IMachine)e.Argument).InitErrorMessageList();

//#if Debug
//                ((IMachine)e.Argument).CheckFile(lsetInfo);
//#else
				CheckNotYetDisplayedLog(lsetInfo);

				//((IMachine)e.Argument).CheckFile(lsetInfo);
                try
                {
                    ((IMachine)e.Argument).CheckFile(lsetInfo);

                    //<--NASCA不良のエラー判定実施:TnLogWaitingQueue(未判定)⇒判定⇒TnLogへ
                    ((IMachine)e.Argument).CheckNascaError(lsetInfo);
                    //-->NASCA不良のエラー判定実施
                }
                finally
                {
                    //エラーメッセージの表示
                    foreach (ErrMessageInfo errMessageInfo in ((IMachine)e.Argument).GetErrorMessageList())
                    {
                        alermOnFg = true;
                        string equipmentMessage = string.Format(Constant.MessageInfo.Message_42, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO);
                        if (!errMessageInfo.MessageVAL.Contains(equipmentMessage))
                        {
                            //設備の情報が無い場合付け足す
                            errMessageInfo.MessageVAL = equipmentMessage + errMessageInfo.MessageVAL;
                        }

                        AddNGMessageInvoke(errMessageInfo.MessageVAL, errMessageInfo.ShowColor);
                    }

                    if (alermOnFg)
                    {
                        sp.PlayLooping();
                    }
                    ((IMachine)e.Argument).InitErrorMessageList();
                }
//#endif

                //通信異常時
                if ( ((IMachine)e.Argument).GetMachineStatus() == Constant.MachineStatus.Stop)
                {
                    ChangePatLampInvoke(lsetInfo.EquipmentNO, Constant.PatLamp.Red);
                    spMachine.Play();
                    break;
                }

                Thread.Sleep(1000);
            }
        }


		//EICS3で削除

		/// <summary>通信メッセージ処理</summary>
		/// <param name="lsetInfo"></param>
		/// <param name="tran"></param>
		/// <param name="errMessageList"></param>
		private void CheckReportMessage(DoWorkEventArgs e, BackgroundWorker tran, LSETInfo lsetInfo)
		{
			//try
			//{
				bool firstContactFG = true;
				SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

				lsetInfo.TypeCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);

				((IMachine)e.Argument).InitHSMS(lsetInfo);
				while (!tran.CancellationPending)
				{
#if Debug
#else
					((IMachine)e.Argument).CommunicationHSMS(ref firstContactFG);
#endif
					//エラーメッセージの表示
					foreach (ErrMessageInfo errMessageInfo in ((IMachine)e.Argument).GetErrorMessageList())
					{
						string equipmentMessage = string.Format(Constant.MessageInfo.Message_42, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO);
						if (!errMessageInfo.MessageVAL.Contains(equipmentMessage))
						{
							//設備の情報が無い場合付け足す
							errMessageInfo.MessageVAL = equipmentMessage + errMessageInfo.MessageVAL;
						}

						AddNGMessageInvoke(errMessageInfo.MessageVAL, errMessageInfo.ShowColor);
					}
					((IMachine)e.Argument).InitErrorMessageList();
				}
			//}
			//catch (Exception err)
			//{

			//}

		}

#region ツールボックス

        /// <summary>全装置開始ボタン</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAllMachineStart_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show(Constant.MessageInfo.Message_1, "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
            {
                return;
            }

			//すでに開始している場合次へ
			foreach (TreeNode tnLine in tvMachine.Nodes)
			{
				ThreadStart(GetMachineFromNode(tnLine));
			}
			
        }

		private void ThreadStart(List<string> machineNoList)
		{
			BackgroundWorker bw = null;
			foreach (string machineNo in machineNoList)
			{
				if (tranList.TryGetValue(machineNo, out bw))
				{
					continue;
				}

				BackgroundWorker tran = new BackgroundWorker();

				tran.DoWork += new DoWorkEventHandler(tran_DoWork); // tran_DoWorkではループ処理だけしておいて、その中で各インスタンスの中の処理を走らせる（各インスタンスの中の処理 = これまでのtran_DoWork内の処理）

				tran.WorkerSupportsCancellation = true;

				IMachine machineInstance = machineList.Find(m => m.Code == machineNo);

				tran.RunWorkerAsync(machineInstance);

				lock (lockThis)
				{
					tranList.Add(machineNo, tran);
				}
			}
		}

        /// <summary>全装置停止ボタン</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAllMachineStop_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show(Constant.MessageInfo.Message_2, "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)) 
            {
                return;
            }

            foreach (BackgroundWorker bw in tranList.Values)
            {
                bw.CancelAsync();
            }
        }

		private void ThreadStop(List<string> machineNoList)
		{
			foreach (string machineNo in machineNoList)
			{
				BackgroundWorker bw = new BackgroundWorker();

				if (tranList.TryGetValue(machineNo, out bw))
				{
					bw.CancelAsync();
				}				
			}
		}

        /// <summary>装置管理</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolMachineList_Click(object sender, EventArgs e)
        {
			if (tvMachine.SelectedNode.Text != GetLineNoFromNode(tvMachine.SelectedNode).ToString())
			{
				MessageBox.Show("ラインを選択して下さい。");
				return;
			}

			F03_MachineList g003 = new F03_MachineList(GetLineNoFromNode(tvMachine.SelectedNode));
			g003.ShowDialog();
        }

        /// <summary>DB/WB紐付けツール</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolLotChain_Click(object sender, EventArgs e)
        {
			if (tvMachine.SelectedNode == null || tvMachine.SelectedNode.Text != GetLineNoFromNode(tvMachine.SelectedNode).ToString())
			{
				MessageBox.Show("ラインを選択して下さい。");
				return;
			}

			F05_ChainTool frmChainTool = new F05_ChainTool(GetLineNoFromNode(tvMachine.SelectedNode));
			frmChainTool.ShowDialog();
        }

#endregion

#region スレッド処理中 パトランプ変更
        /// <summary>パトランプの色を変更</summary>
        /// <param name="plantCD">変更対象の設備番号</param>
        /// <param name="patLamp">変更する色</param>
        public void ChangePatLampInvoke(string plantCD, Constant.PatLamp patLamp)
        {
            this.Invoke(new ChangePatLampDelegate(ChangePatLamp), new object[] { plantCD, patLamp });
        }
        public void ChangePatLamp(string plantCD, Constant.PatLamp patLamp)
        {
			TreeNode targetNode = GetNodeFromMachineNo(plantCD);
			if (targetNode.Name == plantCD)
			{
				//パトランプが赤で黄色に変更しようとしている場合は処理を抜ける
				if (targetNode.ImageIndex == (int)Constant.PatLamp.Red && patLamp == Constant.PatLamp.Yellow)
				{
					return;
				}
				lock (lockThis)
				{
					targetNode.ImageIndex = (int)patLamp;
					targetNode.SelectedImageIndex = (int)patLamp;

					ChangePatLampAtParentNode(targetNode.Parent);
					ChangePatLampAtParentNode(targetNode.Parent.Parent);

					RefreshTreeView();
				}
			}
        }

		private void ChangePatLampAtParentNode(TreeNode tnParent)
		{
			bool hasError = false;
			bool isObserving = false;

			foreach (TreeNode tnChild in tnParent.Nodes)
			{
				//1装置でも異常停止がある場合、その装置の属している工程のパトランプも異常停止状態にする。
				if (tnChild.ImageIndex == (int)Constant.PatLamp.Red)
				{
					tnParent.ImageIndex = (int)Constant.PatLamp.Red;
					tnParent.SelectedImageIndex = (int)Constant.PatLamp.Red;

					hasError = true;
				}
				else if (tnChild.ImageIndex == (int)Constant.PatLamp.Green)
				{
					isObserving = true;
				}
			}

			if ((isObserving == true) && (hasError == false))
			{
				tnParent.ImageIndex = (int)Constant.PatLamp.Green;
				tnParent.SelectedImageIndex = (int)Constant.PatLamp.Green;
			}
			else if ((isObserving == false) && (hasError == false))
			{
				tnParent.ImageIndex = (int)Constant.PatLamp.Yellow;
				tnParent.SelectedImageIndex = (int)Constant.PatLamp.Yellow;
			}
			else
			{
				tnParent.ImageIndex = (int)Constant.PatLamp.Red;
				tnParent.SelectedImageIndex = (int)Constant.PatLamp.Red;
			}
		}
#endregion

#region スレッド処理中 メッセージ追加
        /// <summary>異常ログリストにメッセージを表示する</summary>
        /// <param name="message">表示するメッセージ</param>
        /// <param name="color">表示するフォント色</param>
        public void AddNGMessageInvoke(string message, Color color) 
        {
            this.Invoke(new AddNGMessageDelegate(AddNGMessage), new object[] { message, color });
        }
        public void AddNGMessage(string message, Color color)
        {
            TreeNode node = new TreeNode(message);
            node.ForeColor = color;
            lock (lockThis) 
            {
                tvNGMessage.Nodes.Add(node);
            }
        }

		/// <summary>動作ログリストにメッセージを表示する</summary>
		/// <param name="message">表示するメッセージ</param>
		/// <param name="color">表示するフォント色</param>
		public void AddOKMessageInvoke(string message, Color color)
		{
			this.Invoke(new AddOKMessageDelegate(AddOKMessage), new object[] { message, color });
		}
		public void AddOKMessage(string message, Color color)
		{
			TreeNode node = new TreeNode(message);
			node.ForeColor = color;
			lock (lockThis)
			{
				tvOKMessage.Nodes.Add(node);
			}
		}
#endregion

#region スレッド処理中 ツリーテキスト変更
        public void ChangeTreeTextInvoke(string machineNM, string equipmentNO, string typeCD, string chipNM)
        {
            this.Invoke(new ChangeTreeTextDelegate(ChangeTreeText), new object[] { machineNM, equipmentNO, typeCD, chipNM });
        }
        public void ChangeTreeText(string machineNM, string equipmentNO, string typeCD, string chipNM)
        {
            string ItemNM = machineNM + Constant.MessageInfo.Message_49 + "(" + equipmentNO + ") " + typeCD + " " + chipNM;

			if (GetNodeFromMachineNo(equipmentNO).Text == ItemNM)
			{
				return;
			}

			lock (lockThis)
			{
				GetNodeFromMachineNo(equipmentNO).Text = ItemNM;
			}
        }
#endregion

        /// <summary>アプリケーション終了ボタン</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolApplicationExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>アプリケーション終了処理</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void F01_MachineWatch_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tranList.Count != 0)
            {
                if (DialogResult.Cancel == MessageBox.Show(Constant.MessageInfo.Message_18, "Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    e.Cancel = true;
                    return;
                }

                foreach (BackgroundWorker bw in tranList.Values)
                {
                    bw.CancelAsync();
                }

                if (tranList.Count >= 1)
                {
                    MessageBox.Show(Constant.MessageInfo.Message_19, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    e.Cancel = true;
                    return;
                }
            }
            else 
            {
#if Debug
#else
                F04_Password form04 = new F04_Password();
                form04.ShowDialog();

                if (!form04.PassCompleteFG) 
                {
                    e.Cancel = true;
                }
#endif
            }
        }

        /// <summary>サイレンを消す</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSilent_Click(object sender, EventArgs e)
        {
            sp.Stop();
            spMachine.Stop();
        }

        /// <summary>異常内容を消去</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		/// 
        private void tvMessage_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            F06_ErrCheck form06 = new F06_ErrCheck(e.Node.Text);
            form06.ShowDialog();

            if (!form06.FOK)
            {
                return;
            }
            /*
            if (DialogResult.OK != MessageBox.Show(
                string.Format(Constant.MessageInfo.Message_30, e.Node.Text), "Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
            {
                return;
            }
            */

            e.Node.Remove();
            if (tvNGMessage.Nodes.Count == 0)
            {
                sp.Stop();
            }
        }

        /// <summary>設定ファイル(QCIL.xml)の更新を監視</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
			SettingInfo settingInfo = SettingInfo.GetSingleton();
            lock (lockThis)
            {
                try
                {
                    //設定が変更された場合、設定情報を読み直す
                    settingInfo.GetSettingData(Constant.SETTING_FILE_PATH);
					//2015.11.9 改修タイプのみデータベースから読込
					SettingInfo.SettingFileTypeChanger();
                }
                catch (IOException) 
                {
                    //読み直しはできているがFileSystemWatcherの仕様でロック状態になる為。
                }
            }
        }

        /// <summary>フォーム形態変更</summary>
        private void ChangeFormStyle(FormStyle style)
        {
            switch (style)
            {
                case FormStyle.Neutral:
                    //NMCモードの表示
                    if (Constant.fNmc)
                    {
                        this.Text += "【Mode NMC】";
                    }

                    //画面にバージョン表示
					loggingSoftVer(true);

                    break;

                case FormStyle.ContactError:
					//lvMachine.Enabled = false;
                    toolAllMachineStart.Enabled = false;
                    toolAllMachineStop.Enabled = false;

                    break;
            }
		}
		
		/// <summary>装置リストの指定ノードが属しているラインのラインCDを取得する。</summary>
		/// <param name="tvNode"></param>
		/// <returns></returns>
		private int GetLineNoFromNode(TreeNode tnNode)
		{
			if (tnNode.Level == 0)
			{
				int lineCD;

				if (int.TryParse(tnNode.Text, out lineCD) == false)
				{
					throw new Exception(string.Format(Constant.MessageInfo.Message_96, tnNode.Text));
				}

				return lineCD;
			}

			return GetLineNoFromNode(tnNode.Parent);
		}

		private void cmsObserve_Opening(object sender, CancelEventArgs e)
		{
			TreeNode selectedNode = tvMachine.SelectedNode;

			if (selectedNode == null)
			{
				e.Cancel = true;
				return;
			}

			int lineCD = GetLineNoFromNode(selectedNode);

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);

			if (settingInfoPerLine.EquipmentList.Exists(equipInfo => equipInfo.EquipmentNO == selectedNode.Name))
			{
				e.Cancel = true;
			}
		}

		private void cmsObserve_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (e.ClickedItem == tsmStart)
			{
				ThreadStart(GetMachineFromNode(tvMachine.SelectedNode));
			}
			else if (e.ClickedItem == tsmStop)
			{
				ThreadStop(GetMachineFromNode(tvMachine.SelectedNode));
			}
		}

#region 指定ノード以下の最下層ノードのNameのリストを取得する。
		/// <summary>指定ノード以下の最下層ノードのNameのリストを取得する。</summary>
		/// <param name="tnNode"></param>
		/// <returns></returns>
		private List<string> GetMachineFromNode(TreeNode tnNode)
		{
			List<string> machineNoList = new List<string>();

			GetMachineFromNode(tnNode, ref machineNoList);

			return machineNoList;
		}

		/// <summary>指定ノード以下の最下層ノードのNameのリストを取得する。（再帰関数）</summary>
		/// <param name="tnNode"></param>
		/// <param name="machineNoList"></param>
		/// <returns></returns>
		private List<string> GetMachineFromNode(TreeNode tnNode, ref List<string> machineNoList)
		{
			if (tnNode.FirstNode == null)
			{
				machineNoList.Add(tnNode.Name);
				return machineNoList;
			}

			foreach (TreeNode childNode in tnNode.Nodes)
			{
				GetMachineFromNode(childNode, ref machineNoList);
			}

			return machineNoList;
		}
#endregion

#region 装置リストから指定した装置Noからノードを取得
		/// <summary>装置リストから指定した装置NoとNameが一致した最下層ノードを取得する。ノードが存在しない場合、重複する場合はnullを返す。</summary>
		/// <param name="targetMachineNo"></param>
		/// <returns></returns>
		private TreeNode GetNodeFromMachineNo(string targetMachineNo)
		{
			int nodeCount = 0;

			foreach (TreeNode tnNode in tvMachine.Nodes)
			{
				foreach (string machineNo in GetMachineFromNode(tnNode))
				{
					if (targetMachineNo == machineNo)
					{
						nodeCount++;
					}
				}
			}

			if (nodeCount != 1)
			{
				return null;
			}

			return GetNodeFromMachineNo(tvMachine, targetMachineNo);

		}

		/// <summary>指定ノード以下でmachineNoとNameが最初に一致した再下層ノードを取得する。対象ノードが無い場合はnullを返す。（再帰関数）</summary>
		/// <param name="tnNode"></param>
		/// <param name="machineNo"></param>
		/// <returns></returns>
		private TreeNode GetNodeFromMachineNo(TreeNode tnNode, string machineNo)
		{
			if (tnNode.FirstNode == null)
			{
				if (tnNode.Name == machineNo)
				{
					return tnNode;
				}
				return null;
			}

			TreeNode gettedNode = new TreeNode();
			foreach (TreeNode childNode in tnNode.Nodes)
			{
				gettedNode = GetNodeFromMachineNo(childNode, machineNo);

				if (gettedNode != null)
				{
					break;
				}
			}

			return gettedNode;
		}

		/// <summary>指定したTreeViewからmachineNoとNameが最初に一致した最下層ノードを取得する。対象ノードが無い場合はnullを返す。</summary>
		/// <param name="tvTarget"></param>
		/// <param name="machineNo"></param>
		/// <returns></returns>
		private TreeNode GetNodeFromMachineNo(TreeView tvTarget, string machineNo)
		{
			TreeNode gettedNode = new TreeNode();
			foreach (TreeNode tnNode in tvTarget.Nodes)
			{
				gettedNode = GetNodeFromMachineNo(tnNode, machineNo);

				if (gettedNode != null)
				{
					break;
				}
			}
			return gettedNode;
		}
#endregion

		private void RefreshTreeView()
		{
			Rectangle rect = tvMachine.Bounds;

			if (rect.Width > 0 && rect.Height > 0)
			{
				Bitmap bitmap = new Bitmap(rect.Width, rect.Height);

				Graphics g = Graphics.FromImage(bitmap);
				PaintEventArgs pea = new PaintEventArgs(g, rect);

				this.InvokePaintBackground(tvMachine, pea);
			}
		}

		private void tmRemoteErrReceiver_Tick(object sender, EventArgs e)
		{
#if Debug
#else
			foreach (int targetLineCD in SettingInfo.GetLineCDList())
			{
				List<ErrCommunicate> errComList = ErrorCommunicator.GetReceiveError(targetLineCD, targetLineCD);

				foreach (ErrCommunicate errCom in errComList)
				{
					EquipmentInfo equiInfo = ConnectDB.GetEquipmentData(targetLineCD, errCom.EquipmentNO);

					if (equiInfo == null)
					{
						continue;
					}

					string message = string.Format("《【遠隔エラー通知:{0}⇒{1}】{2}  {3}/設備:{4}》通知内容：{5}", errCom.SendLineCD, errCom.RecvLineCD, errCom.AlertDT, equiInfo.ModelNM, errCom.EquipmentNO, errCom.MessageVAL);

					AddNGMessageInvoke(message, Color.Red);
				}

				if (errComList.Count != 0)
				{
					ErrCommunicate.DeleteData(targetLineCD, errComList);
				}
			}

            deleteSystemLog();
#endif
		}

		private void AllTreeNodeExpandAndCollapse(TreeNode node)
		{
			bool isAllExpand = true;

			if (node.Parent == null)
			{
				foreach (TreeNode childNode in node.Nodes)
				{
					if (childNode.IsExpanded == false)
					{
						isAllExpand = false;
						break;
					}
				}

				if (isAllExpand)
				{
					node.Collapse();
				}
				else
				{
					node.ExpandAll();
				}
			}
		}

        /// <summary>
        /// システムログの削除
        /// </summary>
        private void deleteSystemLog()
        {
            if(SettingInfo.GetDelSysLogIntervalDay() == 0)
            {
                return;
            }

			string[] logFiles = Directory.GetFiles(Constant.LOG_DIR_PATH);

            List<FileInfo> logFileInfos = new List<FileInfo>();

            foreach (string logFile in logFiles)
            {
                FileInfo fileInfo = new FileInfo(logFile);
                logFileInfos.Add(fileInfo);
            }

            logFileInfos
                = logFileInfos.Where(l => DateTime.Now.AddDays(-SettingInfo.GetDelSysLogIntervalDay()) > l.CreationTime).ToList();

            foreach (FileInfo logFileInfo in logFileInfos)
            {
                try
                {
                    File.Delete(logFileInfo.FullName);
                }
                catch (IOException) { }
            }
        }

		private void loggingSoftVer(bool isStartUp)
		{
			//画面にバージョン表示
			string systemVer = "";
			if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
			{
				//クリックワンス
				systemVer = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
			}
			else
			{
				//クリックワンス以外
				systemVer = Application.ProductVersion;

			}
			this.Text += " (Ver." + systemVer + ")";

			string logMsg = this.Text + " 起動";

			if (isStartUp == false)
			{
				logMsg += "中";
			}

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

		}

		private void toolCheckDBConnectInfo_Click(object sender, EventArgs e)
		{
			SettingInfo.CheckDebugSetting();
		}

		/// <summary>
		/// 未だエラー表示していないデータを閾値判定し、表示する
		/// </summary>
		private void CheckNotYetDisplayedLog(LSETInfo lset)
		{
			List<Log> logs = Database.Log.GetNotYetDisplayedData(lset.InlineCD, lset.EquipmentNO);
			foreach (Log l in logs)
			{
				Plm plm = Plm.GetData(lset.InlineCD, l.MaterialCD, lset.ModelNM, l.QcParamNO, true);
				if (plm == null)
				{
					throw new Exception(string.Format(Constant.MessageInfo.Message_28, l.MaterialCD, plm.QcParamNO, plm.ParameterNM));
				}

				string message = ParameterInfo.CheckParameter(plm, Log.GetParameterValue(l.SParameterVAL, l.DParameterVAL), lset, l.NascaLotNO);
				if (string.IsNullOrEmpty(message) == false)
				{
					Log.ErrorUpdate(lset.InlineCD, l.EquipmentNO, l.QcParamNO, l.NascaLotNO, message);

					AlertLog aLog = AlertLog.GetInstance();
					aLog.logMessageQue.Enqueue(message);
				}

				Log.CompleteDisplayedData(lset.InlineCD, l.EquipmentNO, l.QcParamNO, l.NascaLotNO);
			}
		}

		private void TmTypeChanger_Tick(object sender, EventArgs e)
		{
#if DEBUG
			return;
#endif

			//設定データ
			SettingInfo settingInfo = SettingInfo.GetSingleton();
			List<int> lineList = SettingInfo.GetLineCDList();

			try
			{
				foreach (int lineCd in lineList)
				{
					List<Lset> lsetDataList = Lset.GetLsetData(lineCd);

					using (var eicsDB = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCd)))
					{
						foreach (Lset lsetData in lsetDataList)
						{
							//LSETにまとめ型番が設定されている場合は、workingTypeCDが最新の纏め型番マスタの先頭型番と一致しているかチェック
							//不一致ならworkingTypeCDを纏め型番マスタの先頭型番に更新する

							for (int i = 0; settingInfo.SettingInfoList.Count() > i; i++)
							{
								EquipmentInfo equipmentInfo = settingInfo.SettingInfoList[i].EquipmentList.Find(l => l.EquipmentNO == lsetData.EquipmentNO);

								if (equipmentInfo == null)
								{
									continue;
								}

								DataContext.TmTypeGroup typeGrMaster = eicsDB.TmTypeGroup
									.Where(t => t.Model_NM == equipmentInfo.ModelNM && t.TypeGroup_CD == lsetData.WorkingTypeGroup_CD 
										&& equipmentInfo.ChipNM == lsetData.ChipNM && t.Del_FG == false).OrderBy(t => t.LastUpd_DT).FirstOrDefault();

								//まとめ型番の先頭の社内型番が変わった場合（内部的に選択されていた社内型番がまとめ型番から除外された場合）
								if (typeGrMaster != null && typeGrMaster.Type_CD.Trim() != lsetData.WorkingType_CD)
								{
									lsetData.WorkingType_CD = typeGrMaster.Type_CD.Trim();

									if (stopReservationMachineList.ContainsKey(equipmentInfo.EquipmentNO) == false)
									{
										reserveStopMachineThread(equipmentInfo, "まとめ型番マスタが変更された為、一時停止します。再監視して下さい。"
											, Constant.PatLamp.Red);
									}
								}
								//画面上で選択されたまとめ型番で取得できるマスタが存在しなくなった場合
								else if(typeGrMaster == null && string.IsNullOrWhiteSpace(lsetData.WorkingTypeGroup_CD) == false)
								{
									if (stopReservationMachineList.ContainsKey(equipmentInfo.EquipmentNO) == false)
									{
										reserveStopMachineThread(equipmentInfo, "まとめ型番マスタが変更された為、一時停止します。再監視して下さい。"
											, Constant.PatLamp.Red);
									}
								}


								string oldTypeGrpCD = equipmentInfo.TypeGroupCD;
								if (equipmentInfo.TypeGroupCD != lsetData.WorkingTypeGroup_CD)
								{
									equipmentInfo.TypeGroupCD = lsetData.WorkingTypeGroup_CD;
								}

								if (equipmentInfo.TypeCD != lsetData.WorkingType_CD)
								{
									string oldTypeCD = equipmentInfo.TypeCD;
									if (string.IsNullOrWhiteSpace(oldTypeGrpCD) == false)
									{
										oldTypeCD = oldTypeGrpCD;
									}
									equipmentInfo.TypeCD = lsetData.WorkingType_CD;

									string newTypeCD = equipmentInfo.TypeCD;
									if (string.IsNullOrWhiteSpace(equipmentInfo.TypeGroupCD) == false)
									{
										newTypeCD = equipmentInfo.TypeGroupCD;
									}
									updateTvMachineNode(equipmentInfo.EquipmentNO, oldTypeCD, newTypeCD);
								}
                            }
                        }
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private void reserveStopMachineThread(EquipmentInfo equipmentInfo, string reason, Constant.PatLamp patLamp)
		{
			BackgroundWorker bgWorkerInfo = null;
			bool isRunning = tranList.TryGetValue(equipmentInfo.EquipmentNO, out bgWorkerInfo);

			if (isRunning)
			{
				AlertLog alertLog = AlertLog.GetInstance();
				alertLog.logMessageQue.Enqueue($"{equipmentInfo.AssetsNM}/{equipmentInfo.ModelNM}/{equipmentInfo.MachineNM}/{equipmentInfo.EquipmentNO} " + reason);
				bgWorkerInfo.CancelAsync();
				stopReservationMachineList.Add(equipmentInfo.EquipmentNO, patLamp);
			}
		}

		private void updateTvMachineNode(string equipCD, string oldTypeCD, string newTypeCD)
		{
			TreeNode tn = tvMachine.Nodes.Find(equipCD, true).FirstOrDefault();

			if(tn != null)
			{
				tn.Text = tn.Text.Replace(oldTypeCD, newTypeCD);
			}
		}

        //トレイ紐付編集画面
        private void toolTrayEdit_Click(object sender, EventArgs e)
        {

            F07_TrayLotEdit frmtraylotedit = new F07_TrayLotEdit(machineList[0].LineCD);
            frmtraylotedit.ShowDialog();
        }
    }
}
