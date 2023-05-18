using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi.Model;
using ArmsApi.Model.NASCA;

namespace ArmsMaintenance
{
    public partial class FrmNascaSync : Form
    {
		public Process TargetEndProcess = null;

        Exporter exp;

        DataTable nascaSyncTable;
        DataTable asmlotSyncTable;

        private static FrmNascaSync instance;
        private List<string> selectedLotList = new List<string>();

        public static FrmNascaSync GetInstance()
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new FrmNascaSync();
            }

            return instance;
        }


        private FrmNascaSync()
        {
            InitializeComponent();
            
            this.nascaSyncTable = new DataTable();
            DataColumn col =  this.nascaSyncTable.Columns.Add("ロット");
            this.nascaSyncTable.Columns.Add("状態");
            this.nascaSyncTable.PrimaryKey = new DataColumn[] { col };

            this.asmlotSyncTable = new DataTable();
            col = this.asmlotSyncTable.Columns.Add("ロット");
            this.asmlotSyncTable.Columns.Add("状態");
            this.asmlotSyncTable.PrimaryKey = new DataColumn[] { col };

            this.exp = Exporter.GetInstance();
            exp.OnStatusUpdate += new ExportLogEventHandler(exp_OnStatusUpdate);
            exp.OnComplete += new EventHandler(exp_OnComplete);
            exp.OnLotComplete += new ExportLotCompleteEventHandler(exp_OnLotComplete);
            exp.OnError += new ExportLotErrorEventHandler(exp_OnError);
            if (exp.IsRunning) this.btnSync.Enabled = false;
        }

        void exp_OnError(string lotno, string procnm, string sendcmd, string receivemsg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ロット：");
            sb.Append(lotno);
            sb.Append("  工程：");
            sb.Append(procnm);
            sb.Append(Environment.NewLine);
            sb.Append("送信コマンド：");
            sb.Append(sendcmd);
            sb.Append(Environment.NewLine);
            sb.Append("NASCA応答：");
            sb.Append(receivemsg);
            sb.Append(Environment.NewLine);

            AppendErrLog(sb.ToString());
        }

        void exp_OnLotComplete(string lotno, string msg)
        {
            Action act = new Action(() =>
                {
                    foreach (DataGridViewRow row in this.grdCutLot.Rows)
                    {
                        if (row.Cells["ロット"].Value.ToString().Trim().ToUpper() == lotno.Trim().ToUpper())
                        {
                            row.Cells["状態"].Value = msg;
                        }
                    }
                    foreach (DataGridViewRow row in this.grdAsmLot.Rows)
                    {
                        if (row.Cells["ロット"].Value.ToString().Trim().ToUpper() == lotno.Trim().ToUpper())
                        {
                            row.Cells["状態"].Value = msg;
                        }
                    }
                });
            this.Invoke(act);
        }

        void exp_OnComplete(object sender, EventArgs e)
        {
            try
            {
                if (instance != null && !instance.IsDisposed)
                {
                    this.Invoke(new Action(() => this.btnSync.Enabled = true));
                }
                MessageBox.Show(this, "連携完了");
            }
            catch{}
        }

        void exp_OnStatusUpdate(string msg)
        {
            AppendLog(msg);
        }


        /// <summary>
        /// 流動中の全マガジンをアッセンロット連携に追加
        /// </summary>
        private void ImportAllAsmLot()
        {
            Magazine[] maglist = Magazine.GetMagazine(null, null, true, null);

            try
            {
                foreach (Magazine mag in maglist)
                {
                    if (string.IsNullOrEmpty(mag.NascaLotNO)) continue;

                    // ロットNoがアッセンロットNoでないレコードは無視する。
                    // アッセンロットの文字数 = 11 (「ロット番号付与基準書」参照)
                    if (Order.MagLotToNascaLot(mag.NascaLotNO).Length <= 10) continue;

                    if (this.asmlotSyncTable.Rows.Contains(mag.NascaLotNO) == true)
                    {
                        continue;
                    }

                    this.asmlotSyncTable.Rows.Add(new object[] { mag.NascaLotNO, "" });
                }

                foreach (DataGridViewRow row in this.grdAsmLot.Rows)
                {
                    row.Cells[0].Value = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        /// <summary>
        /// NASCASyncボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSync_Click(object sender, EventArgs e)
        {
            if (rbAllAsmLot.Checked)
            {
                ImportAllAsmLot();
            }

            try
            {
                this.btnSync.Enabled = false;

                foreach (DataGridViewRow row in this.grdAsmLot.Rows)
                {
                    if (row.Cells[0].Value != null && (bool)row.Cells[0].Value == true)
                    {
                        AsmLot lot = AsmLot.GetAsmLot(row.Cells[1].Value.ToString());
                        if (lot != null)
                        {
							if (ArmsApi.Config.Settings.IsDBProfileRequireMode)
							{
								Order[] orders = Order.GetOrder(lot.NascaLotNo);
								if (orders.Count() < 2)
								{
									continue;
								}
							}

							exp.Enqueue(lot);
                        }
                    }
                }

                foreach (DataGridViewRow row in this.grdCutLot.Rows)
                {
                    if (row.Cells[0].Value != null && (bool)row.Cells[0].Value == true)
                    {
                        exp.Enqueue(row.Cells[1].Value.ToString());
                    }
                }

				if (chkTargetEndProcess.Checked && TargetEndProcess != null)
				{
					exp.TargetEndProcess = TargetEndProcess;
				}

                exp.SyncAllQueue();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        #region AppendLog
        
        public static void AppendLog(string log)
        {
            try
            {
                if (instance == null || instance.IsDisposed)
                {
                    return;
                }

                if (instance.InvokeRequired == true)
                {
                    instance.Invoke(new Action<string>(s => FrmNascaSync.AppendLog(s)), log);
                }
                else
                {
                    if (instance.txtLog.Lines.Length >= 5000)
                    {
                        instance.txtLog.Text = instance.txtLog.Text.Remove(0, instance.txtLog.Lines[0].Length + 2);
                    }

                    instance.txtLog.AppendText(log + "\r\n");
                }
            }
            catch { }
        }


        public static void AppendErrLog(string log)
        {
            try
            {
                if (instance == null || instance.IsDisposed)
                {
                    return;
                }

                if (instance.InvokeRequired == true)
                {
                    instance.Invoke(new Action<string>(s => FrmNascaSync.AppendErrLog(s)), log);
                }
                else
                {
                    if (instance.txtErrLog.Lines.Length >= 5000)
                    {
                        instance.txtErrLog.Text = instance.txtErrLog.Text.Remove(0, instance.txtErrLog.Lines[0].Length + 2);
                    }

                    instance.txtErrLog.AppendText(log + "\r\n");
                }
            }
            catch { }
        }

        #endregion

        private void FrmNascaSync_Load(object sender, EventArgs e)
        {
			txtMacGroup.Text = Properties.Settings.Default.SelectMacGroup;

			searchLastOrder();
        }

		private void searchLastOrder()
		{
			this.nascaSyncTable.Clear();

			Order[] orders = Order.SearchOrder(null, null, null, false, true);
            Dictionary<string, int> finalProcList = Process.GetFinalProcNoDictionary();
            // <!-- 画面起動時間高速化対応 (最終作業リストの内の作業以外の実績を対象外にする)
            orders = orders.Where(o => finalProcList.Values.Contains(o.ProcNo)).ToArray();
            //  -->
            foreach (Order ord in orders)
			{
				Process p = Process.GetProcess(ord.ProcNo);
				if (p == null) 
				{
					throw new Exception(
						string.Format("作業マスタに存在しない実績が存在します。LotNo:{0} 作業No:{1}", ord.NascaLotNo, p.ProcNo));
				}
                AsmLot lot = AsmLot.GetAsmLot(ord.LotNo);

                CutBlend[] cbs = CutBlend.SearchBlendRecord(null, ord.LotNo, null, false, false);
                if (lot == null)
                {
                    //ブレンドされているロット
                    if (cbs.Length > 0)
                    {
                        lot = AsmLot.GetAsmLot(cbs.First().LotNo);
                        lot.NascaLotNo = ord.LotNo;
                    }
                }

                if (p.FinalSt == true)
                {
                    //ブレンドされているロット
                    if (cbs.Length > 0)
                    {
                        if (lot == null)
                        {
                            continue;
                        }

                        // 最終工程 = 作業順の最後
                        int finalprocno;
                        if (finalProcList.TryGetValue(lot.TypeCd, out finalprocno) == false)
                        {
                            continue;
                        }
                        if (p.ProcNo != finalprocno)
                        {
                            // 途中のロットは表示対象外
                            continue;
                        }
                    }

                    //CTまでの品種ライン想定
                    if (string.IsNullOrEmpty(txtMacGroup.Text) == false)
                    {
                        List<CutBlend> cutLotList = CutBlend.SearchBlendRecord(null, ord.LotNo, null, false, true).ToList();
                        if (cutLotList.Exists(c => AsmLot.GetAsmLot(c.LotNo) != null
                            && AsmLot.GetAsmLot(c.LotNo).MacGroup.Contains(txtMacGroup.Text)))
                        {
                            nascaSyncTable.Rows.Add(new object[] { ord.LotNo, string.Empty });
                        }
                    }
                    else
                    {
                        nascaSyncTable.Rows.Add(new object[] { ord.LotNo, string.Empty });
                    }
                }
                else
                {
                    if (lot == null)
                    {
                        continue;
                    }

                    // 最終工程 = 作業順の最後
                    int finalprocno;
                    if (finalProcList.TryGetValue(lot.TypeCd, out finalprocno) == false)
                    {
                        continue;
                    }
                    if (p.ProcNo != finalprocno)
                    {
                        // 途中のロットは表示対象外
                        continue;
                    }

                    //MDオーブンまでの品種ライン想定
                    if (string.IsNullOrEmpty(txtMacGroup.Text) == false)
                    {
                        if (lot.MacGroup.Contains(txtMacGroup.Text) == false)
                        {
                            continue;
                        }
                    }

                    Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, p.ProcNo);
                    if (dst == Process.MagazineDevideStatus.Double || dst == Process.MagazineDevideStatus.SingleToDouble)
                    {
                        if (orders.Where(o => o.NascaLotNo == ord.NascaLotNo).Count() >= 2)
                        {
                            nascaSyncTable.Rows.Add(new object[] { ord.LotNo, string.Empty });
                        }
                    }
                    else
                    {
                        nascaSyncTable.Rows.Add(new object[] { ord.LotNo, string.Empty });
                    }
                }
			}
			this.grdCutLot.DataSource = nascaSyncTable;

			for (int i = 0; i < this.asmlotSyncTable.Rows.Count; i++)
			{
				if (this.asmlotSyncTable.Rows[i]["状態"].ToString() == Exporter.SYNC_COMPLETE_STRING)
				{
					this.asmlotSyncTable.Rows[i].Delete();
					i--;
				}
			}
			this.grdAsmLot.DataSource = this.asmlotSyncTable;
		}

        private void FrmNascaSync_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (exp.IsRunning)
            {
                MessageBox.Show("連携処理が進行中です");
                e.Cancel = true;
                return;
            }

            exp.OnStatusUpdate -= new ExportLogEventHandler(exp_OnStatusUpdate);
            exp.OnComplete -= new EventHandler(exp_OnComplete);
            exp.OnLotComplete -= new ExportLotCompleteEventHandler(exp_OnLotComplete);
            exp.OnError -= new ExportLotErrorEventHandler(exp_OnError);
        }

        private void 最新の情報に更新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
			searchLastOrder();

			Properties.Settings.Default.SelectMacGroup = txtMacGroup.Text;

			//try
			//{
			//	Order[] orders = Order.SearchOrder(null, null, null, false, true);

			//	this.nascaSyncTable.Clear();
			//	foreach (Order ord in orders)
			//	{
			//		Process p = Process.GetProcess(ord.ProcNo);
			//		if (p.FinalSt == true)
			//		{
			//			AsmLot lot = AsmLot.GetAsmLot(ord.LotNo);
			//			if (lot != null)
			//			{
			//				if (string.IsNullOrEmpty(txtMacGroup.Text) == false)
			//				{
			//					if (lot.MacGroup.Contains(txtMacGroup.Text) == false)
			//					{
			//						continue;
			//					}
			//					else
			//					{
			//						Properties.Settings.Default.SelectMacGroup = txtMacGroup.Text;
			//					}
			//				}

			//				Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, p.ProcNo);
			//				if (dst == Process.MagazineDevideStatus.Double || dst == Process.MagazineDevideStatus.SingleToDouble)
			//				{
			//					if (orders.Where(o => o.NascaLotNo == ord.NascaLotNo).Count() >= 2)
			//					{
			//						nascaSyncTable.Rows.Add(new object[] { ord.LotNo, string.Empty });
			//					}
			//				}
			//				else
			//				{
			//					nascaSyncTable.Rows.Add(new object[] { ord.LotNo, string.Empty });
			//				}
			//			}
			//		}
			//	}

			//	this.grdCutLot.DataSource = nascaSyncTable;

			//	for (int i = 0; i < this.asmlotSyncTable.Rows.Count; i++)
			//	{
			//		if (this.asmlotSyncTable.Rows[i]["状態"].ToString() == Exporter.SYNC_COMPLETE_STRING)
			//		{
			//			this.asmlotSyncTable.Rows[i].Delete();
			//			i--;
			//		}
			//	}
			//}
			//catch (Exception ex)
			//{
			//	MessageBox.Show(ex.ToString());
			//}
        }


        /// <summary>
        /// アッセンロット追加ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInput_Click(object sender, EventArgs e)
        {
            lotListAdd();
            //try
            //{
            //    string searchLotNo = getLotNoSearchChar();
            //    if (string.IsNullOrEmpty(searchLotNo) && selectedLotList.Count() == 0) return;

            //    List<string> lotlist = new List<string>(selectedLotList);

            //    if (string.IsNullOrEmpty(searchLotNo) == false)
            //    {
            //        lotlist.Add(searchLotNo);
            //    }
            //    foreach (string lotno in lotlist)
            //    {
            //        AsmLot lot = AsmLot.GetAsmLot(lotno);
            //        if (lot == null)
            //        {
            //            MessageBox.Show($"ロット情報が存在しません。ロットNo{lotno}");
            //            return;
            //        }

            //        if (this.asmlotSyncTable.Rows.Contains(lotno) == true)
            //        {
            //            //意味が無さそうなので重複時にエラーは出さずにスルーする。
            //            //MessageBox.Show($"既に連携リストに入っています。ロットNo{lotno}");
            //            continue;
            //        }
            //        this.asmlotSyncTable.Rows.Add(new object[] { lot.NascaLotNo, "" });
            //        this.grdAsmLot[0, grdAsmLot.RowCount - 1].Value = true;
            //    }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
        }

        private void lotListAdd()
        {
            try
            {
                string searchLotNo = getLotNoSearchChar();
                if (string.IsNullOrEmpty(searchLotNo) && selectedLotList.Count() == 0) return;

                List<string> lotlist = new List<string>(selectedLotList);

                if (string.IsNullOrEmpty(searchLotNo) == false)
                {
                    lotlist.Add(searchLotNo);
                }
                foreach (string lotno in lotlist)
                {
                    AsmLot lot = AsmLot.GetAsmLot(lotno);
                    if (lot == null)
                    {
                        MessageBox.Show($"ロット情報が存在しません。ロットNo{lotno}");
                        return;
                    }

                    if (this.asmlotSyncTable.Rows.Contains(lotno) == true)
                    {
                        //意味が無さそうなので重複時にエラーは出さずにスルーする。
                        //MessageBox.Show($"既に連携リストに入っています。ロットNo{lotno}");
                        continue;
                    }
                    this.asmlotSyncTable.Rows.Add(new object[] { lot.NascaLotNo, "" });
                    this.grdAsmLot[0, grdAsmLot.RowCount - 1].Value = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnStopReq_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(this, "連携処理を中断します", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);

            if (res == DialogResult.OK)
            {
                exp.StopRequest = true;
            }
        }

        private void chkAllAsmLot_CheckedChanged(object sender, EventArgs e)
        {
			this.txtLotNo.Enabled = !rbAllAsmLot.Checked;
			this.btnInput.Enabled = !rbAllAsmLot.Checked;
        }

		private void btnSelectProcess_Click(object sender, EventArgs e)
		{
			FrmSelectProcess frmSelectProcess = new FrmSelectProcess(this.TargetEndProcess, false);
			frmSelectProcess.ShowDialog();

			if (frmSelectProcess.SelectProcess == null)
			{
				btnSelectProcess.ImageIndex = 1;
				chkTargetEndProcess.Checked = false;
			}
			else 
			{
				btnSelectProcess.ImageIndex = 0;
				chkTargetEndProcess.Checked = true;
			}

			this.TargetEndProcess = frmSelectProcess.SelectProcess;
		}

		private void rbAllAsmLot_CheckedChanged(object sender, EventArgs e)
		{
			if (rbAllAsmLot.Checked)
			{
				txtLotNo.Enabled = false;
				txtLotNo.Text = string.Empty;
			}
			else 
			{
				txtLotNo.Enabled = true;
			}
		}

        private void btnInputMultiLot_Click(object sender, EventArgs e)
        {
            try
            {
                FrmInputGrid frm = new FrmInputGrid(this.selectedLotList);
                DialogResult res = frm.ShowDialog();

                if (res == DialogResult.OK)
                {
                    this.selectedLotList = frm.ValList;

                    if (frm.ValList.Count > 0)
                        this.btnInputMultiLot.ImageIndex = 0;
                    else
                        this.btnInputMultiLot.ImageIndex = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 検索条件のロットNO取得
        /// </summary>
        /// <returns></returns>
        private string getLotNoSearchChar()
        {
            string lotNo = string.Empty;
            if (!string.IsNullOrEmpty(txtLotNo.Text.Trim()))
            {
                string[] lotChar = txtLotNo.Text.Trim().Split(' ');
                if (lotChar.Count() >= 2)
                {
                    lotNo = lotChar[1];
                }
                else
                {
                    lotNo = lotChar[0];
                }
            }
            return lotNo;
        }

        private void txtLotNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lotListAdd();
            }
        }
    }
}
