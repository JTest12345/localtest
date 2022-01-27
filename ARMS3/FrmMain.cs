using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArmsApi;
using ARMS3.Model;
using ARMS3.Model.Machines;
using ARMS3.Model.Carriers;
using System.Threading;
using System.IO;

//juni
using System.Data.SqlClient;
using ARMS3.Model.PLC;

namespace ARMS3
{
    public partial class FrmMain : Form
    {
        /// <summary>
        /// ログの最大表示行数
        /// </summary>
        private const int MAX_LOG_LINES = 5000;

        /// <summary>
        /// Singleton
        /// </summary>
        static FrmMain instance;

        /// <summary>
        /// LineKeeper参照
        /// </summary>
        private static LineKeeper keeper;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();
            instance = this;

            //log4netのMemoryLogger設定
            Log.SetupMemoryLogToAction(Log.RBLog, "%date: %message", logAction);
            Log.SetupMemoryLogToAction(Log.SysLog, "%date: %message", logAction);
            Log.SetupMemoryLogToAction(Log.ApiLog, "%date: %message", apilogAction);
        }

        /// <summary>
        /// FormLoad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_Load(object sender, EventArgs e)
        {
            //バージョン表示
            lblVersion.Text += Application.ProductVersion;

            //////////////////////////////
            ////sql connection test
            ////Debug Code by juni
            /////////////////////////////
            /////
            //try
            //{
            //    using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            //    using (SqlCommand cmd = con.CreateCommand())
            //    {
            //        con.Open();
            //        cmd.Parameters.Add("@MATERIALCD", SqlDbType.NVarChar).Value = "testcd";
            //        cmd.CommandText = "SELECT TOP 1 empname FROM TmEmployee WHERE empcode = 1";

            //        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

            //        //MessageBox.Show(Result);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("SQL FAIL!");
            //}
            ////
            //////////////////////////////
            ////plc connection test
            ////Debug Code by juni
            /////////////////////////////
            /////
            //try
            //{
            //    var Plc = new Mitsubishi("192.168.1.99", 1026);
            //    //MessageBox.Show("PLC Instance build!");

            //    Plc.SetBit("B0000E8", 1, Mitsubishi.BIT_ON);

            //    string retv = Plc.GetBit("B0000E8");
            //    MessageBox.Show(retv);

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("PLC COM FAIL!");
            //}
            //////////////////////////////

            keeper = LineKeeper.GetInstance();

            lstCarActiveNode = this.treeCarList.Nodes.Add("active", ACTIVE_NODE_STRING);
            lstCarIdleNode = this.treeCarList.Nodes.Add("idle", IDLE_NODE_STRING);
            lstCarWaitForStopNode = this.treeCarList.Nodes.Add("wait", WATI_NODE_STRING);

            lstMacActiveNode = this.treeMacList.Nodes.Add("active", ACTIVE_NODE_STRING);
            lstMacIdleNode = this.treeMacList.Nodes.Add("idle", IDLE_NODE_STRING);
            lstMacWaitForStopNode = this.treeMacList.Nodes.Add("wait", WATI_NODE_STRING);

            //搬送機が存在しない=高効率の場合、画面スタイル変更
            if (LineKeeper.Carriers.Count == 0)
            {
                grbMachine.Location = new Point(13, 29);
                grbMachine.Size = new Size(273, 553);
                statusCarrierLabel.Visible = false; statusCarrierThread.Visible = false;
            }

            // ArmsConfig.xmlに項目がない、もしくはリストの中身が全てnull または 空文字の、ボタンを消す
            if (Config.Settings.SignalDisplayLineNoList == null ||
                Config.Settings.SignalDisplayLineNoList.Any(l => !string.IsNullOrWhiteSpace(l)) == false)
            {
                this.btnSignalDisp.Visible = false;
            }

            updateTreeNode();
        }

        ///////////////////////
        // Debug Code by juni
        ///////////////////////
        private void FrmMain_Shown(Object sender, EventArgs e)
        {
            //FakeVIPline.FrmFakeNline frmFakeNline = new FakeVIPline.FrmFakeNline();
            //frmFakeNline.Show();

            FakeVIPline.FrmLmSim FrmLmSim = new FakeVIPline.FrmLmSim();
            FrmLmSim.Show();
        }

        /// <summary>
        /// FormClosing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (lstMacActiveNode.Nodes.Count != 0 || lstMacWaitForStopNode.Nodes.Count != 0
                || lstCarActiveNode.Nodes.Count != 0 || lstCarWaitForStopNode.Nodes.Count != 0)
            {
                foreach (IMachine m in LineKeeper.Machines)
                {
                    m.StopRequested = true;
                    if (m.IsHighLine)
                    {
                        if (m.Plc == null) { continue; }
                        if (m.Plc is RelayMachinePLC)
                        {
                            ((RelayMachinePLC)m.Plc).Plc.Dispose();
                        }
                        //else
                        //{
                        //	((PLC)m.Plc).Dispose();
                        //}                  
                    }
                }

                foreach (ICarrier c in LineKeeper.Carriers)
                {
                    c.StopRequested = true;
                }

                MessageBox.Show("処理を終了しています。暫く経ってから再度ボタンを押して下さい。");

                e.Cancel = true;
                return;
            }

            FrmConfirm frmConfirm = new FrmConfirm();
            frmConfirm.ShowDialog();

            if (!frmConfirm.ConfirmCompleteFG)
            {
                e.Cancel = true;
            }
        }

        #region ログ(Robot, Api)
        static Action<string> logAction = new Action<string>(WriteLog);
        static Action<string> apilogAction = new Action<string>(WriteApiLog);

        /// <summary>
        /// ログ表示用メソッド
        /// </summary>
        /// <param name="log"></param>
        public static void WriteLog(string log)
        {
            if (instance == null) return;

            if (instance.InvokeRequired)
            {
                instance.Invoke(logAction, log);
            }
            else
            {
                if (instance.txtLog.Lines.Length >= MAX_LOG_LINES)
                {
                    instance.txtLog.Text = instance.txtLog.Text.Remove(0, instance.txtLog.Lines[0].Length + 2);
                }
                instance.txtLog.AppendText(log + "\r\n");
            }
        }

        /// <summary>
        /// APIログ表示用メソッド
        /// </summary>
        /// <param name="log"></param>
        public static void WriteApiLog(string log)
        {
            if (instance == null)
            {
                return;
            }

            if (instance.InvokeRequired)
            {
                instance.Invoke(apilogAction, log);
            }
            else
            {
                if (instance.txtApiLog.Lines.Length >= MAX_LOG_LINES)
                {
                    instance.txtApiLog.Text = instance.txtApiLog.Text.Remove(0, instance.txtApiLog.Lines[0].Length + 2);
                }
                instance.txtApiLog.AppendText(log + "\r\n");
            }
        }
        #endregion

        #region ツリー(搬送機リスト、装置リスト)

        private const string ACTIVE_NODE_STRING = "稼働中";
        private const string IDLE_NODE_STRING = "停止中";
        private const string WATI_NODE_STRING = "停止待ち";

        TreeNode lstCarActiveNode;
        TreeNode lstCarIdleNode;
        TreeNode lstCarWaitForStopNode;

        TreeNode lstMacActiveNode;
        TreeNode lstMacIdleNode;
        TreeNode lstMacWaitForStopNode;

        /// <summary>
        /// 装置状態に応じてTreeView更新
        /// </summary>
        private void updateTreeNode()
        {
            //搬送機リスト
            foreach (ICarrier c in LineKeeper.Carriers)
            {
                switch (c.GetWorkStatus())
                {
                    case WorkStatus.Idle:
                        swichTreeNode(c.CarNo.ToString(), c.Name, treeCarList, lstCarIdleNode);
                        break;

                    case WorkStatus.Active:
                        swichTreeNode(c.CarNo.ToString(), c.Name, treeCarList, lstCarActiveNode);
                        break;

                    case WorkStatus.WaitForStop:
                        swichTreeNode(c.CarNo.ToString(), c.Name, treeCarList, lstCarWaitForStopNode);
                        break;
                }
            }
            
            lstCarActiveNode.Text = ACTIVE_NODE_STRING + " (" + lstCarActiveNode.Nodes.Count.ToString() + ")";
            lstCarActiveNode.Expand();

            lstCarIdleNode.Text = IDLE_NODE_STRING + " (" + lstCarIdleNode.Nodes.Count.ToString() + ")";
            lstCarIdleNode.Expand();

            lstCarWaitForStopNode.Text = WATI_NODE_STRING + " (" + lstCarWaitForStopNode.Nodes.Count.ToString() + ")";
            lstCarWaitForStopNode.Expand();

            //装置リスト
            foreach (IMachine m in LineKeeper.Machines)
            {
                switch (m.GetWorkStatus())
                {
                    case WorkStatus.Idle:
                        swichTreeNode(m.LineNo, m.MacNo.ToString(), m.Name, treeMacList, lstMacIdleNode);
                        break;

                    case WorkStatus.Active:
                        swichTreeNode(m.LineNo, m.MacNo.ToString(), m.Name, treeMacList, lstMacActiveNode);
                        break;

                    case WorkStatus.WaitForStop:
                        swichTreeNode(m.LineNo, m.MacNo.ToString(), m.Name, treeMacList, lstMacWaitForStopNode);
                        break;
                }
            }

            int activeNodeCount = 0;
            foreach (TreeNode node in lstMacActiveNode.Nodes)
            {
                activeNodeCount += node.Nodes.Count;
            }
            lstMacActiveNode.Text = ACTIVE_NODE_STRING + " (" + activeNodeCount.ToString() + ")";
            lstMacActiveNode.Expand();

            int idleNodeCount = 0;
            foreach (TreeNode node in lstMacIdleNode.Nodes)
            {
                idleNodeCount += node.Nodes.Count;
            }
            lstMacIdleNode.Text = IDLE_NODE_STRING + " (" + idleNodeCount.ToString() + ")";
            lstMacIdleNode.Expand();

            int waitNodeCount = 0;
            foreach (TreeNode node in lstMacWaitForStopNode.Nodes)
            {
                waitNodeCount += node.Nodes.Count;
            }
            lstMacWaitForStopNode.Text = WATI_NODE_STRING + " (" + waitNodeCount.ToString() + ")";
            lstMacWaitForStopNode.Expand();
        }

        /// <summary>
        /// TreeViewNodeを他Nodeに付替え
        /// </summary>
        /// <param name="m"></param>
        /// <param name="node"></param>
        private void swichTreeNode(string nodekeyLineNo, string nodeKeyMacNo, string nodeName, TreeView tv, TreeNode node)
        {
            foreach (TreeNode n in tv.Nodes)
            {
                if (n != node)
                {
                    //状態の違うNodeは削除
                    TreeNodeCollection targetNodes;
                    if (nodekeyLineNo == string.Empty)
                    {
                        targetNodes = n.Nodes;
                    }
                    else
                    {
                        if (n.Nodes.ContainsKey(nodekeyLineNo))
                        {
                            targetNodes = n.Nodes[nodekeyLineNo].Nodes;
                        }
                        else { targetNodes = n.Nodes; }
                    }

                    if (targetNodes.ContainsKey(nodeKeyMacNo))
                    {
                        targetNodes.RemoveByKey(nodeKeyMacNo);
                    }
                }
                else
                {
                    //状態一致

                    TreeNode targetNode;
                    if (nodekeyLineNo == string.Empty)
                    {
                        targetNode = n;
                    }
                    else
                    {
                        //ラインNo追加
                        if (n.Nodes.ContainsKey(nodekeyLineNo))
                        {
                            targetNode = n.Nodes[nodekeyLineNo];
                        }
                        else
                        {
                            TreeNode newnode = n.Nodes.Add(nodekeyLineNo, string.Format("#{0}", nodekeyLineNo), 1);
                            targetNode = newnode;
                        }
                    }

                    if (targetNode.Nodes.ContainsKey(nodeKeyMacNo))
                    {
                        continue;
                    }
                    else
                    {
                        //Nodeが存在しない場合は追加
                        TreeNode newnode = targetNode.Nodes.Add(nodeKeyMacNo, nodeName, 1);
                        newnode.Tag = node;
                    }
                }

                //ライン毎のすべての装置が無い場合、ラインも削除
                if (nodekeyLineNo != string.Empty && n.Nodes.Count != 0)
                {
                    foreach (TreeNode no in n.Nodes)
                    {
                        if (no == null)
                        {
                            continue;
                        }
                        if (no.Nodes.Count == 0)
                        {
                            no.Remove();
                        }
                    }
                }
            }

        }
        private void swichTreeNode(string nodeKeyMacNo, string nodeName, TreeView tv, TreeNode node) 
        {
            swichTreeNode(string.Empty, nodeKeyMacNo, nodeName, tv, node);
        }

        #region 装置リストメニュー

        /// <summary>
        /// 開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemMacStart_Click(object sender, EventArgs e)
        {
            if (treeMacList.SelectedNode.Level == 1)
            {
                //ライン選択時
                foreach(TreeNode node in treeMacList.SelectedNode.Nodes)
                {
                    IMachine machine = LineKeeper.Machines.Find(m => m.MacNo.ToString() == node.Name);
                    machine.RunWork();
                }
            }
            else 
            {
                //装置選択時
                IMachine machine = LineKeeper.Machines.Find(m => m.MacNo.ToString() == treeMacList.SelectedNode.Name);
                machine.RunWork();
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemMacStop_Click(object sender, EventArgs e)
        {
            if (treeMacList.SelectedNode.Level == 1)
            {
                //ライン選択時
                foreach (TreeNode node in treeMacList.SelectedNode.Nodes)
                {
                    IMachine machine = LineKeeper.Machines.Find(m => m.MacNo.ToString() == node.Name);
                    machine.StopRequested = true;

                    if (machine.IsHighLine)
                    {
                        if (machine.Plc != null)
                        {
							if (machine.Plc is RelayMachinePLC)
							{
								((RelayMachinePLC)machine.Plc).Plc.Dispose();
							}
                        }
                    }
                }
            }
            else
            {
                //装置選択時
                IMachine machine = LineKeeper.Machines.Find(m => m.MacNo.ToString() == treeMacList.SelectedNode.Name);
                machine.StopRequested = true;

                if (machine.IsHighLine)
                {
                    if (machine.Plc != null)
                    {
						if (machine.Plc is RelayMachinePLC)
						{
							((RelayMachinePLC)machine.Plc).Plc.Dispose();
						}
                    }
                }
            }
        }

        /// <summary>
        /// 仮想マガジン削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemMacMagDelete_Click(object sender, EventArgs e)
        {
            if (treeMacList.SelectedNode.Level != 1) 
            {
                if (DialogResult.OK != MessageBox.Show(string.Format("{0}内の仮想マガジンを全て削除します。よろしいですか？", treeMacList.SelectedNode.Name), "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    return;
                }

                Log.SysLog.Info(string.Format("[仮想マガジン削除]装置名:{0}",treeMacList.SelectedNode.Name));

                //装置選択時
                IMachine machine = LineKeeper.Machines.Find(m => m.MacNo.ToString() == treeMacList.SelectedNode.Name);
                machine.ClearVirtualMagazines();
            }
        }

        private void menuMachine_Opening(object sender, CancelEventArgs e)
        {
            if (treeMacList.SelectedNode == null || treeMacList.SelectedNode.Level < 1)
            {
                e.Cancel = true;
                return;
            }

            TreeNode statusNode;
            if (treeMacList.SelectedNode.Level == 1)
            {
                //ライン選択時
                statusNode = treeMacList.SelectedNode.Parent;
            }
            else 
            {
                //装置選択時
                statusNode = treeMacList.SelectedNode.Parent.Parent;
            }
            if (statusNode.Name == "idle")
            {
                itemMacStart.Enabled = true;
                itemMacStop.Enabled = false;
            }
            else
            {
                itemMacStart.Enabled = false;
                itemMacStop.Enabled = true;
            }
        }

        #endregion

        #region 搬送機リストメニュー

        /// <summary>
        /// 開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemCarStart_Click(object sender, EventArgs e)
        {
            ICarrier carrier = LineKeeper.Carriers.Find(c => c.CarNo.ToString() == treeCarList.SelectedNode.Name);
            carrier.RunWork();
        }

        /// <summary>
        /// 停止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void itemCarStop_Click(object sender, EventArgs e)
        {
            ICarrier carrier = LineKeeper.Carriers.Find(c => c.CarNo.ToString() == treeCarList.SelectedNode.Name);
            carrier.StopRequested = true;
        }

        private void menuCarrier_Opening(object sender, CancelEventArgs e)
        {
            if (treeCarList.SelectedNode == null || treeCarList.SelectedNode.Level < 1)
            {
                e.Cancel = true;
                return;
            }

            if (treeCarList.SelectedNode.Parent.Name == "idle")
            {
                itemCarStart.Enabled = true;
                itemCarStop.Enabled = false;
            }
            else
            {
                itemCarStart.Enabled = false;
                itemCarStop.Enabled = true;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// タイマー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            updateTreeNode();

            updateThreadStatus();

            //changeOvenProfile();

            deleteSystemLog();
        }

        /// <summary>
        /// 装置、搬送機のスレッド状態を更新
        /// </summary>
        private void updateThreadStatus()
        {
            //搬送機、装置が全実行中の場合、コントロールをロックする
            //停止しているスレッドを発見した場合、ロックを解除する。

            if (lstMacIdleNode.Nodes.Count == 0 && lstMacWaitForStopNode.Nodes.Count == 0)
            {
                statusMachineThread.Text = "実行中";
                statusMachineThread.ForeColor = Color.Green;

                //treeMacList.Enabled = false;
            }
            else 
            {
                statusMachineThread.Text = "停止有り";
                statusMachineThread.ForeColor = Color.Red;

                //treeMacList.Enabled = true;
            }

            if (lstCarIdleNode.Nodes.Count == 0 && lstCarWaitForStopNode.Nodes.Count == 0)
            {
                statusCarrierThread.Text = "実行中";
                statusCarrierThread.ForeColor = Color.Green;

                //treeCarList.Enabled = false;
            }
            else 
            {
                statusCarrierThread.Text = "停止有り";
                statusCarrierThread.ForeColor = Color.Red;

                //treeCarList.Enabled = true;
            }
        }
    
		///// <summary>
		///// オーブンプロファイル自動登録
		///// </summary>
		//private void changeOvenProfile()
		//{
		//	if (Config.Settings.UseOvenProfiler)
		//	{
		//		if ((DateTime.Now - LineKeeper.LastProfileReserve).Minutes >= Properties.Settings.Default.OvenProfilerTimerMinutes)
		//		{
		//			Log.ApiLog.Info("Profile Auto Reserve Start");
		//			foreach (ICarrier carrier in LineKeeper.Carriers)
		//			{
		//				//PLC電源断対策　毎回ReadyをON
		//				LineKeeper.OvenProfileChanger.SetReady(((CarrierBase)carrier).Plc);
		//				LineKeeper.OvenProfileChanger.ChangeDBProfile(((CarrierBase)carrier).LastDBOvenProfileReserve, ((CarrierBase)carrier).Plc);
		//				LineKeeper.OvenProfileChanger.ChangeMDProfile(((CarrierBase)carrier).LastMDOvenProfileReserve, ((CarrierBase)carrier).Plc);

		//				LineKeeper.LastProfileReserve = DateTime.Now;              
		//			}
		//			Log.ApiLog.Info("Profile Auto Reserve Complete");
		//		}
		//	}
		//}

        /// <summary>
        /// システムログの削除
        /// </summary>
        private void deleteSystemLog()
        {
            //string logDirPath = Path.Combine(Application.StartupPath, "LOG");
            string logDirPath = @"C:\ARMS\LOG";
            string[] logFiles = Directory.GetFiles(logDirPath);

            List<FileInfo> logFileInfos = new List<FileInfo>();

            foreach (string logFile in logFiles)
            {
                FileInfo fileInfo = new FileInfo(logFile);
                logFileInfos.Add(fileInfo);
            }

            logFileInfos
                = logFileInfos.Where(l => DateTime.Now.AddDays(-Config.Settings.SystemLogKeepDay) > l.CreationTime).ToList();

            foreach (FileInfo logFileInfo in logFileInfos)
            {
                try
                {
                    File.Delete(logFileInfo.FullName);
                }
                catch (IOException) { }
            }
        }

        #region ツールバー Top

        /// <summary>
        /// 停止(全搬送機、全装置)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopReqAll_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(this, "全搬送機、全装置の監視を現在のジョブ完了後に終了します", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);
            if (res != DialogResult.OK) return;

            this.Close();
            //foreach (IMachine m in LineKeeper.Machines)
            //{
            //    m.StopRequested = true;

            //    if (m.Plc != null)
            //    {
            //        ((RelayMachinePLC)m.Plc).Plc.Dispose();
            //    }
            //}

            //foreach (ICarrier c in LineKeeper.Carriers)
            //{
            //    c.StopRequested = true;
            //}
        }

        /// <summary>
        /// 開始(全搬送機、全装置)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartAll_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(this, "全搬送機、全装置の監視スレッドを開始します", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);
            if (res != DialogResult.OK) return;

            foreach (IMachine m in LineKeeper.Machines)
            {
                m.RunWork();
            }

            foreach (ICarrier c in LineKeeper.Carriers)
            {
                c.RunWork();
            }
        }

        #endregion

        #region ツールバー Bottom

		/// <summary>
		/// 常温CV待機時間変更
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnChangeCVWait_Click(object sender, EventArgs e)
		{
			//DialogResult res = MessageBox.Show(this, "常温CV待機時間の設定を変更します。\n\n既に投入済みのマガジンにも適用されるので注意してください",
			//	"確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);

			//if (res == DialogResult.OK)
			//{
			//	FrmCVWaitInput frm = new FrmCVWaitInput();
			//	frm.ShowDialog();
			//	this.txtCVWaitMinute.Text = Properties.Settings.Default.MoldConvayorWaitMinutes.ToString();
			//}
		}

        /// <summary>
        /// TestMode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestMode_Click(object sender, EventArgs e)
        {
            FrmTestMode form = new FrmTestMode(LineKeeper.Carriers);
            form.Show();
        }

        #endregion

        /// <summary>
        /// 装置信号表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSignalDisp_Click(object sender, EventArgs e)
        {
            var form = new FrmSignalDisplay();
            form.Show();
        }

    }
}
