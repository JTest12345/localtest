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
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.IO;


namespace ArmsMaintenance
{
    public partial class FrmMagazineMainte : Form
    {
        private System.Media.SoundPlayer sp = new System.Media.SoundPlayer("alert2.wav");

        public FrmMagazineMainte()
        {
            InitializeComponent();
        }

        private static SortedList<string, FrmEditTran> frmtrans = new SortedList<string, FrmEditTran>();

        public static void RemoveTranFrmInstance(string nascalotno)
        {
            lock (frmtrans)
            {
                if (frmtrans != null && frmtrans.Keys.Contains(nascalotno))
                {
                    frmtrans.Remove(nascalotno);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                searchMagazines(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                timer2.Stop();

                if (Config.Settings.OperationMachineGroupShowKey == null)
                {
                    throw new ApplicationException("自動登録の装置作業者設定を使用する場合は、設定ファイル(ArmsConfig.xml)のOperationMachineGroupShowKeyに表示させたいキー文字を設定して下さい。");
                }

                if (MachineInfo.HasOverWorkOperator(Config.Settings.OperationMachineGroupShowKey) == true)
                {
                    throw new ApplicationException("時間設定を超えている作業者がいます。作業者の更新をして下さい。");
                }

                MachineInfo[] allMachines = MachineInfo.GetMachineList(true);
                if (allMachines.Where(m => string.IsNullOrEmpty(m.OperationMacGroupCd)).Count() >= 1)
                {
                    IEnumerable<string> plantCdList = allMachines
                        .Where(m => string.IsNullOrEmpty(m.OperationMacGroupCd))
                        .Select(m => m.ClassName + ":" + m.NascaPlantCd).ToArray();

                    string showPlantCdList = string.Join(Environment.NewLine, plantCdList);
                    throw new ApplicationException($"装置作業者が未設定の装置があります。下記設備番号一覧参照{Environment.NewLine + showPlantCdList}");
                }           
            }
            catch (Exception ex)
            {
                sp.PlayLooping();
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                timer2.Start();
                sp.Stop();
            }
        }
        
        /// <summary>
        /// FormLoad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMagazineMainte_Load(object sender, EventArgs e)
        {		
			//バージョン表示
            lblVersion.Text += Application.ProductVersion;
            this.timer1.Interval = Convert.ToInt32(this.numInterval.Value);

            if (Config.Settings.ActiveMachineOperator == true)
            {
                this.timer2.Interval = 60000;
                this.timer2.Start();
            }

            ArmsApi.Model.Process.GetWorkFlowProcNo();

            //コンボボックス(完了工程)
            ArmsApi.Model.Process[] procList = ArmsApi.Model.Process.GetWorkflowProcess();
            cmbProcess.ValueMember = "ProcNo";
            cmbProcess.DisplayMember = "InlineProNM";
            cmbProcess.DataSource = procList;
            cmbProcess.SelectedIndex = -1;
        }

         /// <summary>
        /// FormClosing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMagazineMainte_FormClosing(object sender, FormClosingEventArgs e)
        {
            FrmPasswordDialog pwd = new FrmPasswordDialog("CLOSE", "本当に終了しますか?\r\n終了する場合CLOSEと入力してください");
            DialogResult res = pwd.ShowDialog();

            if (res != DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        #region 検索条件

        #region 自動更新

        /// <summary>
        /// 自動更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoUpdate.Checked)
            {
                this.timer1.Interval = (int)this.numInterval.Value * 1000;
                this.timer1.Enabled = true;

                timer1_Tick(null, null);

                this.btnSearch.Enabled = false;
                this.btnAddRow.Enabled = false;
                this.btnInput.Enabled = false;
                this.btnEdit.Enabled = false;
            }
            else
            {
                this.timer1.Enabled = false;
                this.btnSearch.Enabled = true;
                this.btnInput.Enabled = true;
                this.btnAddRow.Enabled = true;
                this.btnEdit.Enabled = true;
            }
        }

        /// <summary>
        /// 自動更新秒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numInterval_ValueChanged(object sender, EventArgs e)
        {
            if (numInterval.Value <= 30)
            {
                MessageBox.Show("更新間隔は30秒以上に設定してください");
                this.numInterval.Value = 30;
            }

            this.timer1.Interval = Convert.ToInt32(numInterval.Value);
        }

        #endregion

        /// <summary>
        /// プロファイル選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProfileSelect_Click(object sender, EventArgs e)
        {
            try
            {
                FrmSelectProfile frm = new FrmSelectProfile(true);
                DialogResult res = frm.ShowDialog(this);
                if (res != DialogResult.OK) return;
                if (frm.Selected == null) return;

                txtProfileId.Text = frm.Selected.ProfileId.ToString();
                txtProfileNm.Text = frm.Selected.ProfileNm;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchMagazines(true);
        }

        private void searchMagazines(bool showErrorMsg)
        {
            this.grdMagazine.Rows.Clear();

            //マガジン情報検索
            
            int? procNo = null;
            if (cmbProcess.Text != "") 
            {
                procNo = Convert.ToInt32(cmbProcess.SelectedValue);
            }

			List<GnlMaster> lifeTestResultGoodList = null;

            Magazine[] maglist = Magazine.GetMagazine(
                this.getMagazineNoSearchChar(), this.getLotNoSearchChar(),
                this.chkOnlyActive.Checked, txtResinGrp.Text, procNo);

            //上位1000行のみ表示
            foreach (Magazine m in maglist)
            {
                if (this.grdMagazine.Rows.Count > 1000)
                {
                    if (showErrorMsg)
                    {
                        MessageBox.Show("検索結果が1000件を超えたため\n上位1000件のみ表示しています。");
                    }
                    break;
                }

                AsmLot lot = AsmLot.GetAsmLot(m.NascaLotNO);
                if (lot == null)
                {
                    MessageBox.Show(string.Format("ロット情報が存在しません。マガジン番号:{0}", m.NascaLotNO));
                    continue;
                }
                
                if (string.IsNullOrEmpty(txtTypeNo.Text) == false)
                {
                    //検索条件の型番でない場合は次へ
                    if (lot.TypeCd.Contains(txtTypeNo.Text.Trim()) == false) continue;
                }

                string ctblendcd = (lot != null ? lot.CutBlendCd : "");
				
                if (txtProfileId.Text != string.Empty)
                {
                    //検索条件のプロファイルで無い場合は次へ
                    if (int.Parse(txtProfileId.Text.Trim()) != lot.ProfileId) continue;
                }

                if (txtLineCD.Text != string.Empty) 
                {
                    //検索条件のラインで無い場合は次へ　※ロット番号定義が変更した場合は変更が必要
                    if (!Regex.IsMatch(lot.NascaLotNo, string.Format("^.......{0}.*$", txtLineCD.Text))) continue; 
                }

                Profile profile = Profile.GetProfile(lot.ProfileId);
                string profilenm = (profile != null ? profile.ProfileNm : "");

                string dbthrowdt = (profile != null && profile.DBThrowDt != null ? profile.DBThrowDt : "");
                if (txtDbThrowDT.Text != string.Empty)
                {
                    //検索条件のD/B投入日で無い場合は次へ
                    if (!dbthrowdt.Contains(txtDbThrowDT.Text)) continue; 
                }
				string macgroup = string.Join(",", lot.MacGroup);

                string lifeTestResult = string.Empty;
                string tempCutBlendNo = string.Empty;

				if (chkLifeTestResultInfo.Checked) 
				{
					if (lot.IsBeforeLifeTest == false) continue;

					if (lifeTestResultGoodList == null) 
						lifeTestResultGoodList = GnlMaster.GetLifeTestGoodResult(lot.BeforeLifeTestCondCd).ToList();
					
					LotChar result = LotChar.GetLifeTestResult(lot.TempCutBlendNo);
					if (result != null)
					{
						if (lifeTestResultGoodList.Exists(l => l.Val == result.ListVal))
						{
							lifeTestResult = string.Format("{0}({1})", "良", result.ListVal);
						}
						else 
						{
							lifeTestResult = string.Format("{0}({1})", "否", result.ListVal);
						}
					}

					tempCutBlendNo = lot.TempCutBlendNo;
				}

                int r = this.grdMagazine.Rows.Add(new object[] { 
                    false, lot.IsDieShearLot, m.NascaLotNO, m.MagazineNo, lot.TypeCd, m.NowCompProcessNm, string.Join(",", lot.ResinGpCd), m.NewFg,
                    m.NowCompProcess, ctblendcd, dbthrowdt, profilenm, macgroup, tempCutBlendNo, lifeTestResult, lot.BlendCd,
                    lot.ScheduleSelectionStandard, lot.MnggrId.HasValue ? lot.MnggrId.ToString() : ""});

                if (profilenm.Contains("特別"))
                {
                    // 特別、先行色調の両文字が含まれる場合、特別が優先
                    this.grdMagazine.Rows[r].DefaultCellStyle.BackColor = Color.Yellow;
                }
                else if (profilenm.Contains("先行色調"))
                {
                    this.grdMagazine.Rows[r].DefaultCellStyle.BackColor = Color.Pink;
                }
                else
                {
                    this.grdMagazine.Rows[r].DefaultCellStyle.BackColor = Color.White;
                }
            }
            toolRowMaxCount.Text = this.grdMagazine.RowCount.ToString();
        }

        #endregion

        #region Topツールバー

        #region 設備資材/条件割り付け

        private void toolMacMat_Click(object sender, EventArgs e)
        {
            try
            {
                FrmMacValues frm = new FrmMacValues(FrmMacValueDataType.Material);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void toolMacResin_Click(object sender, EventArgs e)
        {
            try
            {
                FrmMacValues frm = new FrmMacValues(FrmMacValueDataType.Resin);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void toolMacCond_Click(object sender, EventArgs e)
        {
            try
            {
                FrmMacValues frm = new FrmMacValues(FrmMacValueDataType.Condition);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        private void プロファイル選択ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Config.Settings.UseDBProfile)
            {
                FrmSelectDBProfile frm = new FrmSelectDBProfile();
                frm.ShowDialog();
            }
            else
            {
                FrmSelectProfile frm = new FrmSelectProfile();
                frm.ShowDialog();
            }
        }

        #region NASCA連携

        private void カット完了品NASCA連携ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                FrmNascaSync frm = FrmNascaSync.GetInstance();
                frm.Show();
                frm.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void nASCAロット取り込みToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmImportLot frm = FrmImportLot.GetInstance();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void 設備マスタ編集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmMacMaster frm = new FrmMacMaster();
            frm.ShowDialog();
        }

        #endregion

        #region レポート

        private void 作業履歴照会ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmWorkView frm = new FrmWorkView();
            frm.Show();
        }

        private void toolCutWorkView_Click(object sender, EventArgs e)
        {
            try
            {
                FrmCutBlendWorkView frm = new FrmCutBlendWorkView();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        private void dBウェハー確認ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmWaferCheck frm = new FrmWaferCheck();
            frm.ShowDialog();
        }

        private void cSV作業実績出力ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmWorkViewReport frm = new FrmWorkViewReport();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        #endregion

        #region サブ画面

        /// <summary>
        /// タッチパネル用不良入力画面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolTouchPanelDefectMenu_Click(object sender, EventArgs e)
        {
            try
            {
                FrmDefInput frm = new FrmDefInput();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        /// <summary>
        /// 指図手動発行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolManualInstruction_Click(object sender, EventArgs e)
        {
            try
            {
                FrmManualNewOrder frm = new FrmManualNewOrder();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void mAPダイボンド前ベーキング管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmPreDBMonitor frm = FrmPreDBMonitor.GetInstance();
                frm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void メンテナンス履歴照会ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmHistoryView frm = new FrmHistoryView();
                frm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void toolRestrict_Click(object sender, EventArgs e)
        {
            try
            {
                FrmRestrict frm = new FrmRestrict();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ロットトレース一括規制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmLotTrace frm = new FrmLotTrace();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDebug frm = new FrmDebug();
            frm.Show();
        }

        private void 仮想マガジンToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmVirtualMagMainte frm = new FrmVirtualMagMainte();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

		private void データメンテナンス定型文設定ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FrmMainteFixedPhrase frm = new FrmMainteFixedPhrase();
			frm.ShowDialog();
		}

		private void カットラベル印刷設定ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FrmCutLabelSetting frm = new FrmCutLabelSetting();
			frm.ShowDialog();
		}

        private void 資材在庫数編集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmMaterialStock frm = new FrmMaterialStock();
            frm.ShowDialog();
        }

        #endregion

        #endregion

        /// <summary>
        /// グリッドセルダブルクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdMagazine_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1) { return; }

            DataGridViewCell targetCell = grdMagazine.Rows[e.RowIndex].Cells["SelectFg"];
            if (Convert.ToBoolean(targetCell.Value) == true)
            {
                targetCell.Value = false;
            }
            else
            {
                targetCell.Value = true;
            }
        }

        #region BindingNavigator

        /// <summary>
        /// 全選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAllSelect_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in grdMagazine.Rows)
            {
                row.Cells["SelectFg"].Value = true;
            }
        }

        /// <summary>
        /// 全解除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAllCancel_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in grdMagazine.Rows)
            {
                row.Cells["SelectFg"].Value = false;
            }
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolDelete_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> selectmags = selectMagazineData();
            if (selectmags.Count == 0) 
            {
                MessageBox.Show("選択されているレコードが存在しません。", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            FrmPasswordDialog frm = new FrmPasswordDialog("OK", "選択中の関連データを全て削除します。\r\n本当に削除する場合は入力欄にOKと入力してください。");
            DialogResult res = frm.ShowDialog();
            if (res != DialogResult.OK)
            {
                return;
            }

            Log.DelLog.Info("[削除開始]");

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    foreach (DataGridViewRow dgv in selectmags)
                    {
                        string lotno = dgv.Cells["NascaLotNo"].Value.ToString();

                        //ARMS------------------------------------------------------------------------
                        //TnMagから情報取得(newfg=1)
                        Magazine mag = ArmsApi.Model.Magazine.GetMagazine(lotno, true).Single();

                        //TnMag
                        Magazine.Delete(cmd, lotno);

                        //TnVirtualMag
                        ArmsApi.Model.VirtualMag vmag = new ArmsApi.Model.VirtualMag();
                        vmag.Delete(lotno);         //lotnoで削除
                        vmag.Delete(mag.MagazineNo);//magnoで削除

                        //TnLot
                        AsmLot lot = AsmLot.GetAsmLot(lotno);
                        lot.Delete(cmd);

                        //TnTran
                        Order.Delete(cmd, lotno);

                        //TnLotCond
                        LotChar.Delete(cmd, lotno);

                        //TnInspection
                        Inspection.Delete(cmd, lotno);
  
                        //TnDefect
                        Defect.Delete(cmd, lotno);

                        //TnCutBlend
                        CutBlend.Delete(cmd, lotno);

                        //TnRestrict
                        Restrict.Delete(cmd, lotno);

                        //TnMatRelation
                        Order.DeleteMaterialRelation(cmd, lotno);

                        //TnLotLog
                        AsmLot.DeleteLotLog(cmd, lotno);

                        //TnHistory
                        History.Delete(cmd, lotno);

                        //TnReservedTran
                        ReservedOrder.Delete(lotno);

                        //TnLotCarrier
                        LotCarrier.Delete(lotno);

                        //TnLotTray
                        AsmLot.DeleteLotTray(lotno);

                        //TnLotTrayDataMatrix
                        LotTrayDataMatrix.Delete(lotno);

                        //TnLotMark
                        LotMarkData.Delete(lotno);

                        //TnTimeLimitChecked
                        TimeLimit.Delete(lotno);

                        //LENS------------------------------------------------------------------------
                        //TnLot
                        ArmsApi.Model.LENS.Lot.Delete(lotno);

                        //TnMapResult
                        ArmsApi.Model.LENS.MapResult.Delete(lotno);

                        //TnTran
                        ArmsApi.Model.LENS.WorkResult.Delete(lotno);

                        //LAMS------------------------------------------------------------------------
                        //TnWork
                        ArmsApi.Model.LAMS.Work.Delete(lotno);
                    }

                    cmd.Transaction.Commit();

                    Log.DelLog.Info("[削除完了]");
                    MessageBox.Show("削除完了しました。", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception err)
                {
                    cmd.Transaction.Rollback();

                    Log.DelLog.Info("[削除失敗]");
                    MessageBox.Show(string.Format("削除に失敗しました。\r\n{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #endregion

        /// <summary>
        /// 選択中のマガジンデータを取得
        /// </summary>
        /// <returns></returns>
        private List<DataGridViewRow> selectMagazineData() 
        {
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in grdMagazine.Rows)
            {
                if (Convert.ToBoolean(row.Cells["SelectFg"].Value) == true) 
                {
                    rows.Add(row);
                }
            }
            return rows;
        }

        /// <summary>
        /// 編集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.grdMagazine.SelectedRows.Count >= 1)
                {
                    Magazine mag = ConvertToMagazine(this.grdMagazine.SelectedRows[0]);

                    FrmEditMagazine frm = new FrmEditMagazine(mag);
                    frm.ShowDialog();
                    searchMagazines(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// ロット複製
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddRow_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.grdMagazine.SelectedRows.Count >= 1)
                {
                    Magazine mag = ConvertToMagazine(this.grdMagazine.SelectedRows[0]);

                    if (mag.NewFg == true)
                    {
                        MessageBox.Show(
                            "複製を作成する場合、先に稼働中フラグを外してください");
                        return;
                    }

                    FrmEditMagazine frm = new FrmEditMagazine(mag.NascaLotNO);
                    frm.ShowDialog();
                    searchMagazines(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 強度データ出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPSTester_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.grdMagazine.SelectedRows.Count >= 1)
                {
                    Magazine mag = ConvertToMagazine(this.grdMagazine.SelectedRows[0]);

                    FrmPSTesterExport frm = new FrmPSTesterExport(mag);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private const string SVR_FORM_EXE_NAME = "ArmsMonitor.exe";

        [DllImport("user32.dll")]
        extern static bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 時間監視
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnInput_Click(object sender, EventArgs e)
        {
            //クリックワンスを起動する
            System.Diagnostics.Process.Start(ArmsApi.Config.Settings.ArmsMonitorClickOnceFullPath);

            //System.Diagnostics.Process[] exists = System.Diagnostics.Process.GetProcessesByName("ArmsMonitor");

            //if (exists.Length >= 1)
            //{
            //    SetForegroundWindow(exists[0].MainWindowHandle);
            //}
            //else
            //{
            //    ProcessStartInfo psi = new ProcessStartInfo(SVR_FORM_EXE_NAME);
            //    System.Diagnostics.Process.Start(psi);
            //}
        }

        /// <summary>
        /// カットブレンド完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCutBlend_Click(object sender, EventArgs e)
        {
            try
            {
                FrmCutBlend frm = new FrmCutBlend(11001);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// カットブレンド編集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditCutBlend_Click(object sender, EventArgs e)
        {
            try
            {
                FrmCutBlendMainte frm = new FrmCutBlendMainte();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 実績入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInput_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.grdMagazine.SelectedRows.Count >= 1)
                {
                    Magazine mag = ConvertToMagazine(this.grdMagazine.SelectedRows[0]);
                    FrmEditTran frm;

                    lock (frmtrans)
                    {
                        if (frmtrans.Keys.Contains(Order.MagLotToNascaLot(mag.NascaLotNO)))
                        {
                            frm = frmtrans[Order.MagLotToNascaLot(mag.NascaLotNO)];
                            if (frm == null || frm.IsDisposed)
                            {
                                frm = new FrmEditTran(mag);
                            }
                        }
                        else
                        {
                            frm = new FrmEditTran(mag);
                            frmtrans.Add(Order.MagLotToNascaLot(mag.NascaLotNO), frm);
                        }
                    }
                    frm.Show();
                    frm.Activate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private Magazine ConvertToMagazine(DataGridViewRow row)
        {
            Magazine retv = new Magazine();

            try
            {
                retv.MagazineNo = (string)row.Cells["MagazineNo"].Value;
                retv.NascaLotNO = (string)row.Cells["NascaLotNo"].Value;
                retv.NowCompProcess = (int)row.Cells["NowCompProcess"].Value;
                retv.NewFg = (bool)row.Cells["NewFg"].Value;

                return retv;
            }
            catch
            {
                return null;
            }

        }

        private void ロットエラー履歴照会ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmLotLog frm = new FrmLotLog();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ロット進捗確認ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmLotProgress frm = new FrmLotProgress();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
        /// <summary>
        /// 検索条件のマガジンNO取得
        /// </summary>
        /// <returns></returns>
        private string getMagazineNoSearchChar() 
        {
            string magNo = string.Empty;
            if (!string.IsNullOrEmpty(txtMagNo.Text.Trim()))
            {
                string[] magChar = txtMagNo.Text.Trim().Split(' ');
                if (magChar.Count() >= 2) 
                { 
                    magNo = magChar[1]; 
                }
                else 
                { 
                    magNo = magChar[0]; 
                }
            }
            return magNo;
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

		private void マーキング情報編集ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				FrmLotmark frm = new FrmLotmark();
				frm.ShowDialog();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

        private void スパッタ用トレイ割当て情報ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmLotTray frm = new FrmLotTray();
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void toolMachineOperator_Click(object sender, EventArgs e)
        {
            FrmMachineOperator frm = new FrmMachineOperator();
            frm.Show();
        }

        private void ロット情報全削除_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> selectmags = selectMagazineData();
            if (selectmags.Count == 0)
            {
                MessageBox.Show("選択されているレコードが存在しません。", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            FrmPasswordDialog frm = new FrmPasswordDialog("OK", "選択中の関連データを全て削除します。\r\n本当に削除する場合は入力欄にOKと入力してください。");
            DialogResult res = frm.ShowDialog();
            if (res != DialogResult.OK)
            {
                return;
            }

            Log.DelLog.Info("[削除開始]");

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    foreach (DataGridViewRow dgv in selectmags)
                    {
                        string lotno = dgv.Cells["NascaLotNo"].Value.ToString();

                        //ARMS------------------------------------------------------------------------
                        //TnMagから情報取得(newfg=1)
                        Magazine mag = ArmsApi.Model.Magazine.GetMagazine(lotno, true).Single();

                        //TnMag
                        Magazine.Delete(cmd, lotno);

                        //TnVirtualMag
                        ArmsApi.Model.VirtualMag vmag = new ArmsApi.Model.VirtualMag();
                        vmag.Delete(lotno);         //lotnoで削除
                        vmag.Delete(mag.MagazineNo);//magnoで削除

                        //TnLot
                        AsmLot lot = AsmLot.GetAsmLot(lotno);
                        lot.Delete(cmd);

                        //TnTran
                        Order.Delete(lotno);//物理削除

                        //TnLotCond
                        LotChar.Delete(cmd, lotno);

                        //TnInspection
                        Inspection.Delete(cmd, lotno);

                        //TnDefect
                        Defect.Delete(cmd, lotno);

                        //TnCutBlend
                        CutBlend.Delete(cmd, lotno);

                        //TnRestrict
                        Restrict.Delete(cmd, lotno);

                        //TnMatRelation
                        Order.DeleteMaterialRelation(cmd, lotno);

                        //TnLotLog
                        AsmLot.DeleteLotLog(cmd, lotno);

                        //TnHistory
                        History.Delete(cmd, lotno);

                        //TnReservedTran
                        ReservedOrder.Delete(lotno);

                        //TnLotCarrier
                        LotCarrier.Delete(lotno);

                        //TnLotTray
                        AsmLot.DeleteLotTray(lotno);

                        //TnLotTrayDataMatrix
                        LotTrayDataMatrix.Delete(lotno);

                        //TnLotMark
                        LotMarkData.Delete(lotno);

                        //TnTimeLimitChecked
                        TimeLimit.Delete(lotno);

                        //LENS------------------------------------------------------------------------
                        //TnLot
                        ArmsApi.Model.LENS.Lot.Delete(lotno);

                        //TnMapResult
                        ArmsApi.Model.LENS.MapResult.Delete(lotno);

                        //TnTran
                        ArmsApi.Model.LENS.WorkResult.Delete(lotno);

                        //LAMS------------------------------------------------------------------------
                        //TnWork
                        ArmsApi.Model.LAMS.Work.Delete(lotno);
                    }

                    cmd.Transaction.Commit();

                    Log.DelLog.Info("[削除完了]");
                    MessageBox.Show("削除完了しました。", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception err)
                {
                    cmd.Transaction.Rollback();

                    Log.DelLog.Info("[削除失敗]");
                    MessageBox.Show(string.Format("削除に失敗しました。\r\n{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
