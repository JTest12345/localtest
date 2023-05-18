using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi;
using ArmsApi.Model;

namespace ArmsMaintenance
{
    public partial class FrmEditTran : Form
    {
        private DefItem[] orgDefectList;
        private DefItem[] DefectList;
        private Inspection[] InspectionList;
        private Material[] MatList;
        private Resin[] ResinList;

        private Process[] WorkFlow;
        public AsmLot lot;
        private Process proc;
        private int magSeqNo;
        private MachineInfo machine;
        
        private Order order;

        public FrmEditTran(Magazine mag)
        {
            this.lot = AsmLot.GetAsmLot(mag.NascaLotNO);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setLotInfo();
            setWorkFlow();
            setInspection();
            setMaterialGrid();
            setResinGrid();
        }


        private void setInspection()
        {
            Inspection[] specs = Inspection.GetInspections(this.lot.NascaLotNo);
            this.InspectionList = specs;

            if (specs == null || specs.Length == 0)
            {
                this.grdInspections.DataSource = null;
                return;
            }

            this.grdInspections.DataSource = this.InspectionList;

            this.grdInspections.Columns["LotNo"].Visible = false;
            this.grdInspections.Columns["ProcNm"].HeaderText = "抜取り検査工程";
            this.grdInspections.Columns["ProcNo"].Visible = false;;
            this.grdInspections.Columns["IsInspected"].HeaderText = "検査済み";
        }


        #region Lot情報更新
        /// <summary>
        /// ロット情報をフォームに反映
        /// </summary>
        private void setLotInfo()
        {
            this.txtLotNo.Text = this.lot.NascaLotNo;
            this.txtTypeCd.Text = this.lot.TypeCd;
            this.txtBlendCd.Text = this.lot.BlendCd;

            this.txtResinGr.Text = string.Join(",", this.lot.ResinGpCd);
            this.txtCutBlendCd.Text = this.lot.CutBlendCd;
            // Ver1.99.0 予定選別規格 追加
            this.txtScheduleSelectionStandard.Text = this.lot.ScheduleSelectionStandard;

            this.chkColorTest.Checked = this.lot.IsColorTest;
            this.chkWarning.Checked = this.lot.IsWarning;
            this.chkKHLTest.Checked = this.lot.IsKHLTest;
            this.chkLifeTest.Checked = this.lot.IsLifeTest;
            this.chkFullInspection.Checked = this.lot.IsFullSizeInspection;
            this.chkWBMapping.Checked = this.lot.IsMappingInspection;
            this.chkChangePointLot.Checked = this.lot.IsChangePointLot;
            this.chkLoopCondChange.Checked = this.lot.IsLoopCondChange;
        }
        #endregion


        /// <summary>
        /// 画面左の工程情報取得処理
        /// </summary>
        private void setWorkFlow()
        {
            this.WorkFlow = Process.GetWorkFlow(lot.TypeCd);

            foreach (Process p in WorkFlow)
            {
                Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(this.lot, p.ProcNo);

                switch (dst)
                {
                    case Process.MagazineDevideStatus.Single:
                    case Process.MagazineDevideStatus.DoubleToSingle:
                        //MEL_MAPだけは最終がモールド硬化なので見せる
						if (p.FinalSt == true) continue;
                        this.treeWork.Nodes.Add(p.ProcNo.ToString(), p.InlineProNM).Tag = 0;
                        break;

                    case Process.MagazineDevideStatus.SingleToDouble:
                    case Process.MagazineDevideStatus.Double:
                        //分割工程の場合はツリー2つ表示
                        this.treeWork.Nodes.Add(p.ProcNo.ToString(), p.InlineProNM + "_#1").Tag = 1;
                        this.treeWork.Nodes.Add(p.ProcNo.ToString(), p.InlineProNM + "_#2").Tag = 2;
                        break;

                    default:
                        throw new ApplicationException("マガジン分割ステータス異常 " + lot.TypeCd + ":" + p.ProcNo.ToString());
                }
            }
        }


        #region 作業実績情報更新
        

        /// <summary>
        /// 画面右の作業情報取得処理
        /// </summary>
        private void setWorkRecord()
        {
            this.order = Order.GetMagazineOrder(this.lot.NascaLotNo, this.magSeqNo, this.proc.ProcNo);
            if (order == null)
            {
				Process.MagazineDevideStatus magdevstatus = Process.GetMagazineDevideStatus(lot, this.proc.ProcNo);
				if (magdevstatus == Process.MagazineDevideStatus.DoubleToSingle)
				{
					//統合作業は前作業が完了しているか判断できない為、規制をしない。
				}
				else
				{
					bool skipcheck = WorkChecker.IsWorkSkiped(Order.NascaLotToMagLot(this.lot.NascaLotNo, this.magSeqNo), this.proc.ProcNo);
					if (skipcheck == true)
					{
						MessageBox.Show("前作業の実績が入力されていません");
						return;
					}
				}

				this.order = Order.CreateNewOrder(this.proc.ProcNo);
			}

            this.machine = MachineInfo.GetMachine(order.MacNo, true);

            if (this.machine != null)
            {
                this.txtMachine.Text = this.machine.MachineName;
                this.txtMacNo.Text = this.machine.MacNo.ToString();
            }
            else
            {
                this.txtMacNo.Text = string.Empty;
                this.txtMachine.Text = string.Empty;
            }

            Process proc = Process.GetProcess(order.ProcNo);
            this.txtProcess.Text = proc.InlineProNM;
            this.txtProcNo.Text = order.ProcNo.ToString();
            this.dtpWorkStart.Value = order.WorkStartDt;

            if (order.WorkEndDt.HasValue)
            {
                this.dtpWorkEnd.Value = order.WorkEndDt.Value;
                this.chkWorkEnd.Checked = true;
            }
            else
            {
                this.dtpWorkEnd.Value = DateTime.Now;
                this.chkWorkEnd.Checked = false;
                this.dtpWorkEnd.Enabled = false;
            }


            this.txtWorkStartEmpCd.Text = order.TranStartEmpCd;
            this.txtWorkEndEmpCd.Text = order.TranCompEmpCd;

            this.txtStocker1.Text = order.ScNo1;
            this.txtStocker2.Text = order.ScNo2;

            this.txtComment.Text = order.Comment;

            this.txtInspectEmpCd.Text = order.InspectEmpCd;

            if (this.order.InspectCt < 0)
            {
                this.rdoInspectAll.Checked = true;
                this.rdoInspectSample.Checked = false;
                this.txtInspectCt.Text = "";
            }
            else
            {
                this.rdoInspectSample.Checked = true;
                this.rdoInspectAll.Checked = false;
                this.txtInspectCt.Text = this.order.InspectCt.ToString();
            }

            this.chkNascaStart.Checked = this.order.IsNascaStart;
            this.chkNascaEnd.Checked = this.order.IsNascaEnd;
			this.chkDefectEnd.Checked = this.order.IsDefectEnd;
            this.chkPreparationOrder.Checked = this.order.IsResinMixOrdered;

            setDefectGrid();
            setMaterialGrid();
            setResinGrid();
        }


        /// <summary>
        /// 画面入力情報から指図データを作成してDBへ保存
        /// </summary>
        private bool updateWorkRecord()
        {
            if (this.order == null)
            {
                MessageBox.Show("作業実績データがありません");
                return false;
            }

            if (string.IsNullOrEmpty(this.txtMacNo.Text))
            {
                MessageBox.Show("作業号機を選択してください");
                return false;
            }
            this.order.MacNo = int.Parse(this.txtMacNo.Text);
            this.order.ProcNo = int.Parse(this.txtProcNo.Text);
            this.order.WorkStartDt = this.dtpWorkStart.Value;

            this.order.ScNo1 = this.txtStocker1.Text;
            this.order.ScNo2 = this.txtStocker2.Text;
            this.order.Comment = this.txtComment.Text;
            this.order.InspectEmpCd = this.txtInspectEmpCd.Text;

            if (this.rdoInspectAll.Checked == true)
            {
                this.order.InspectCt = -1;
            }
            else
            {
                int inspectct;
                if (int.TryParse(this.txtInspectCt.Text, out inspectct))
                {
                    this.order.InspectCt = inspectct;
                }
                else
                {
                    MessageBox.Show(this, "検査数は数字を入力してください。");
                    return false;
                }
            }

            if (this.order.InspectCt != 0 && string.IsNullOrEmpty(this.txtInspectEmpCd.Text))
            {
                MessageBox.Show(this, "検査数がある場合は検査者の社員番号を入力してください。");
                return false;
            }

            if (!string.IsNullOrEmpty(this.txtInspectEmpCd.Text))
            {
                int num;
                if (!int.TryParse(this.txtInspectEmpCd.Text, out num))
                {
                    MessageBox.Show(this, "検査者の社員番号は数字を入力してください。");
                    return false;
                }
                this.order.InspectEmpCd = num.ToString();
            }

            this.order.TranStartEmpCd = this.txtWorkStartEmpCd.Text;
            this.order.TranCompEmpCd = this.txtWorkEndEmpCd.Text;
            this.order.IsNascaStart = this.chkNascaStart.Checked;
            this.order.IsNascaEnd = this.chkNascaEnd.Checked;
			this.order.IsDefectEnd = true;
            this.order.IsResinMixOrdered = this.chkPreparationOrder.Checked;

            this.order.LotNo = this.lot.NascaLotNo;
            this.order.DevidedMagazineSeqNo = this.magSeqNo;

            string errMsg;
            bool isError;

            try
            {
                if (this.chkWorkEnd.Checked)
                {
                    this.order.WorkEndDt = this.dtpWorkEnd.Value;
                    isError = WorkChecker.IsErrorWorkComplete(this.order, this.machine, this.lot, true, out errMsg);

                    //作業履歴保存問合せ
                    FrmMainteHistory frmHst = new FrmMainteHistory(this.lot.NascaLotNo, this.order);
                    DialogResult res = frmHst.ShowDialog();
                    if (res != DialogResult.OK)
                    {
                        return false;
                    }

                    if (isError)
                    {
                        //完了登録時はエラーでも登録可能
                        MessageBox.Show(errMsg);
                        this.lot.IsWarning = true;
                        this.lot.Update();
                        this.chkWarning.Checked = true;
                    }

                    this.order.DeleteInsert(this.order.LotNo, true);
                }
                else
                {
                    this.order.WorkEndDt = null;
                    isError = WorkChecker.IsErrorBeforeStartWork(this.lot, this.machine, this.order, null, this.proc, true, out errMsg);
                    if (isError)
                    {
                        //開始のエラーは登録不可
                        MessageBox.Show(errMsg);
                        return false;
                    }

                    //作業履歴保存問合せ
                    FrmMainteHistory frmHst = new FrmMainteHistory(this.lot.NascaLotNo, this.order);
                    DialogResult res = frmHst.ShowDialog();
                    if (res != DialogResult.OK)
                    {
                        return false;
                    }

                    this.order.DeleteInsert(this.order.LotNo, true);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show("登録エラー：\n" + ex.Message);
                return false;
            }

            return true;

        }
        #endregion

        #region 不良登録関連


        /// <summary>
        /// 不良登録グリッドの初期化
        /// </summary>
        private void setDefectGrid()
        {
            DefItem[] org = Defect.GetAllDefect(this.lot, this.proc.ProcNo);

            if (org == null || org.Length == 0)
            {
                this.DefectList = org;
                //this.grdDefect.DataSource = null;
                this.grdDefect.Rows.Clear();
                return;
            }


            //検査数が0の場合はマスタ参照
            if (this.order.InspectCt == 0)
            {
                int? defaultct = Defect.GetDefaultInspectCtMaster(this.lot.TypeCd, this.order.ProcNo);
                if (defaultct.HasValue == false)
                {
                    this.txtInspectCt.Text = "";
                }
                else
                {
                    this.txtInspectCt.Text = defaultct.ToString();
                }
            }
            

            this.DefectList = org.OrderBy(i => i.CauseCd).ThenBy(i => i.ClassCd).ThenBy(i => i.OrderNo).ToArray();
            //this.orgDefectList = this.DefectList;

            #region 変更前 (並び替え機能を使用するため、DataSource使用をやめる)

            //this.grdDefect.DataSource = this.DefectList;

            //this.grdDefect.Columns["OrderNo"].Visible = false;
            //this.grdDefect.Columns["CauseCd"].Visible = false;
            //this.grdDefect.Columns["CauseName"].ReadOnly = true;
            //this.grdDefect.Columns["CauseName"].HeaderText = "起因";
            //this.grdDefect.Columns["ClassCd"].Visible = false;
            //this.grdDefect.Columns["ClassName"].ReadOnly = true;
            //this.grdDefect.Columns["ClassName"].HeaderText = "分類";
            //this.grdDefect.Columns["DefectCd"].ReadOnly = true;
            //this.grdDefect.Columns["DefectCd"].HeaderText = "コード";
            //this.grdDefect.Columns["DefectName"].ReadOnly = true;
            //this.grdDefect.Columns["DefectName"].HeaderText = "名称";
            //this.grdDefect.Columns["DefectCt"].DefaultCellStyle.BackColor = Color.Yellow;
            //this.grdDefect.Columns["DefectCt"].HeaderText = "数量";
            //this.grdDefect.Columns["IsDisplayedEICS"].Visible = false;
            //this.grdDefect.Columns["ProcNo"].Visible = false;

            #endregion

            #region 変更後 (並び替え機能を使用するため、DataSource使用をやめる)
            this.grdDefect.Rows.Clear();
            foreach (var def in this.DefectList)
            {
                this.grdDefect.Rows.Add(new object[] {
                    def.OrderNo, def.CauseCd, def.CauseName, def.ClassCd, def.ClassName, def.DefectCd, def.DefectName, def.DefectCt, def.DefectCt
                });
            }
            #endregion

            countAndSetDefectTotal();
        }


        private void updateDefect()
        {
            this.grdDefect.EndEdit();
            Defect def = new Defect();
            def.LotNo = this.lot.NascaLotNo;
            def.ProcNo = int.Parse(this.txtProcNo.Text);
            def.DefItems = new List<DefItem>();

            foreach (DataGridViewRow row in this.grdDefect.Rows)
            {
                
                DefItem d = new DefItem();
                d.CauseCd = row.Cells["CauseCd"].Value.ToString();
                d.ClassCd = row.Cells["ClassCd"].Value.ToString();
                d.DefectCd = row.Cells["DefectCd"].Value.ToString();
                d.DefectCt = Convert.ToInt32(row.Cells["DefectCt"].Value);

                if (d.DefectCt >= 1)
                {
                    def.DefItems.Add(d);
                }
            }

            def.DeleteInsert();
        }

        /// <summary>
        /// 不良合計数のカウント
        /// </summary>
        private void countAndSetDefectTotal()
        {
            int total = 0;

            #region 変更前 (並び替え機能を使用するため、DataSource使用をやめる)
            //foreach (DefItem item in this.DefectList)
            //{
            //    total += item.DefectCt;
            //}
            #endregion

            #region 変更後
            int colDefCt = this.grdDefect.Columns["DefectCt"].Index;
            foreach(DataGridViewRow row in this.grdDefect.Rows)
            {
                total += Convert.ToInt32(row.Cells[colDefCt].Value);
            }
            #endregion

            this.txtDefTotal.Text = total.ToString();
        }


        /// <summary>
        /// 不良グリッドのフォーマットエラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdDefect_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("不良数の値が不正です");
        }

        private void grdDefect_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {            
            if (e.RowIndex <= -1) return;

            // 数量列の変更時のみ、処理する
            int colDefCt = this.grdDefect.Columns["DefectCt"].Index;
            if (colDefCt != e.ColumnIndex) return;

            int colOrgDefCt = this.grdDefect.Columns["OrgDefectCt"].Index;

            var obj = this.grdDefect.Rows[e.RowIndex].Cells[colDefCt].Value;

            // 空白の場合、入力値を0にする
            if (obj == null) obj = 0;

            int defectCt;
            if (int.TryParse(obj.ToString(), out defectCt) == false)
            {
                MessageBox.Show($"不良数の値が不正です 値=「{obj.ToString()}」");
                this.grdDefect.Rows[e.RowIndex].Cells[colDefCt].Value = this.grdDefect.Rows[e.RowIndex].Cells[colOrgDefCt].Value;
                return;
            }

            // 入力直後は数量が文字列になっている。このままだと数量列を並び替えしようとした時に例外エラーが発生するので、数値型で上書きする
            this.grdDefect.Rows[e.RowIndex].Cells[colDefCt].Value = defectCt;

            countAndSetDefectTotal();
        }
        #endregion

        #region 原材料グリッド表示
        
        /// <summary>
        /// 原材料グリッド
        /// </summary>
        private void setMaterialGrid()
        {
            if (this.machine == null)
            {
                this.grdMaterials.DataSource = null;
                return;
            }

            this.MatList = order.GetMaterials();

            if (this.MatList == null || this.MatList.Length == 0)
            {
                this.grdMaterials.DataSource = null;
                return;
            }

            this.grdMaterials.DataSource = this.MatList;

            foreach (DataGridViewColumn col in this.grdMaterials.Columns)
            {
                switch (col.Name)
                {
                    case "MaterialCd":
                        col.HeaderText = "原材料CD";
                        break;

                    case "MaterialNm":
                        col.HeaderText = "品名";
                        break;

                    case "LotNo":
                        col.HeaderText = "ロット";
                        break;

                    case "StockerNo":
                        col.HeaderText = "段数";
                        break;

                    case "InputDt":
                        col.HeaderText = "投入日";
                        break;

                    case "RemoveDt":
                        col.HeaderText = "終了日";
                        break;

                    case "LimitDt":
                        col.HeaderText = "使用期限";
                        break;

                    default:
                        col.Visible = false;
                        break;
                }

            }
        }
        #endregion

        #region 樹脂グリッド表示


        /// <summary>
        /// 樹脂グリッド
        /// </summary>
        private void setResinGrid()
        {
            if (this.machine == null)
            {
                this.grdResin.DataSource = null;
                return;
            }


            this.ResinList = order.GetResins();

            if (this.ResinList == null || this.ResinList.Length == 0)
            {
                this.grdResin.DataSource = null;
                return;
            }

            this.grdResin.DataSource = this.ResinList;

            foreach (DataGridViewColumn col in this.grdResin.Columns)
            {
                switch (col.Name)
                {
                    case "MixResultId":
                        col.HeaderText = "調合結果ID";
                        break;

                    case "ResinGroupCd":
                        col.HeaderText = "樹脂Gr";
                        break;

                    case "InputDt":
                        col.HeaderText = "投入日";
                        break;

                    case "RemoveDt":
                        col.HeaderText = "終了日";
                        break;

                    case "LimitDt":
                        col.HeaderText = "使用期限";
                        break;

                    default:
                        col.Visible = false;
                        break;
                }

            }
        }
        #endregion

        #region 作業完了フラグ

        private void chkWorkEnd_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkWorkEnd.Checked)
            {
                this.dtpWorkEnd.Enabled = true;
            }
            else
            {
                this.dtpWorkEnd.Enabled = false;
            }
        }
        #endregion

        #region 装置選択ボタン
        
        /// <summary>
        /// 装置選択ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectMachine_Click(object sender, EventArgs e)
        {
            try
            {
                FrmSelectMachine frmmac = new FrmSelectMachine();
                DialogResult res = frmmac.ShowDialog();

                if (res == DialogResult.OK)
                {
                    MachineInfo m = frmmac.SelectedMachine;
                    this.machine = m;
                    this.txtMachine.Text = m.LongName;
                    this.txtMacNo.Text = m.MacNo.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        private void treeWork_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.proc != null)
            {
                if (this.proc.ProcNo.ToString() == e.Node.Name && this.magSeqNo == Convert.ToInt32(e.Node.Tag))
                {
                    return;
                }
            }

            this.proc = Process.GetProcess(int.Parse(e.Node.Name));
            this.magSeqNo = Convert.ToInt32(e.Node.Tag);
            this.chkChangeStock.Checked = false;
            this.txtDefTotal.Text = "0";
            this.txtInspectCt.Text = "";
            setWorkRecord();
            
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int total = 0;
                foreach (DefItem item in this.DefectList)
                {
                    total += item.DefectCt;
                }

                if (total > 0)
                {
                    if (string.IsNullOrEmpty(this.txtInspectEmpCd.Text))
                    {
                        MessageBox.Show(this, "検査結果がある場合は検査者の社員番号を入力してください。");
                        return;
                    }

                    int num;
                    if (!int.TryParse(this.txtInspectEmpCd.Text, out num))
                    {
                        MessageBox.Show(this, "社員番号は数字を入力してください。");
                        return;
                    }

                }

                if (string.IsNullOrEmpty(txtWorkStartEmpCd.Text) == false)
                {
                    int num;
                    if (!int.TryParse(this.txtWorkStartEmpCd.Text, out num))
                    {
                        MessageBox.Show(this, "開始完了作業者は空白または社員番号を入力してください。");
                        return;
                    }

                    if (!int.TryParse(this.txtWorkEndEmpCd.Text, out num))
                    {
                        MessageBox.Show(this, "開始完了作業者は空白または社員番号を入力してください。");
                        return;
                    }
                }

                if (CommonApi.IsHalfSizeChar(txtStocker1.Text) == false || CommonApi.IsHalfSizeChar(txtStocker2.Text) == false)
                {
                    MessageBox.Show(this, "ストッカー欄は半角文字で入力して下さい。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                DialogResult res = MessageBox.Show(this, "保存しますか?", "確認",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (res != DialogResult.OK)
                {
                    return;
                }

                bool success = updateWorkRecord();

                if (success)
                {
                    //不良情報更新
                    updateDefect();

                    if (order.IsNascaEnd)
                    {
                        MessageBox.Show(this, "このロットはNASCA連携済みです。\nNASCA側の履歴も修正してください");
                    }

                    this.chkChangeStock.Checked = false;
                    this.txtDefTotal.Text = "0";
                    this.txtInspectCt.Text = "";
                    setWorkRecord();

                    this.lot = AsmLot.GetAsmLot(this.lot.NascaLotNo);
                    MessageBox.Show(this, "更新完了");
                }
                else
                {
                    MessageBox.Show(this, "更新を中断しました");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        #region 作業実績削除
        
        private void 作業実績削除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process nextProcess = Process.GetNextProcess(this.proc.ProcNo, this.lot);
                if (nextProcess != null)
                {
                    Order nextOrder = Order.GetNextMagazineOrder(this.lot.NascaLotNo, this.magSeqNo, this.proc.ProcNo);

                    if (nextOrder != null)
                    {
                        MessageBox.Show(this, "次作業の実績があるため作業実績を削除できません");
                        return;
                    }
                }


                //削除理由問合せ
                FrmMainteHistory frm = new FrmMainteHistory(this.lot.NascaLotNo, this.proc.ProcNo);
                DialogResult res = frm.ShowDialog();

                if (res == DialogResult.OK)
                {
                    if (order.IsNascaEnd)
                    {
                        MessageBox.Show(this, "このロットはNASCA連携済みです。\nNASCA側の履歴も修正してください");
                    }
                    this.order.DelFg = true;
                    this.order.DeleteInsert(this.order.LotNo);
                    MessageBox.Show(this, "削除完了");
                }
                else
                {
                    MessageBox.Show(this, "削除処理を中断しました");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        private void chkChangeStock_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkChangeStock.Checked)
            {
                this.txtStocker1.ReadOnly = false;
                this.txtStocker2.ReadOnly = false;
            }
            else
            {
                this.txtStocker1.ReadOnly = true;
                this.txtStocker2.ReadOnly = true;
            }
        }


        private void 設備資材情報編集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.machine != null)
                {
                    FrmMacValues frm = new FrmMacValues(FrmMacValueDataType.Material, this.machine.MacNo, this.dtpWorkStart.Value, this.dtpWorkEnd.Value);
                    frm.ShowDialog();
                }
                else
                {
                    FrmMacValues frm = new FrmMacValues(FrmMacValueDataType.Material);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void 設備樹脂情報編集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.machine != null)
                {
                    FrmMacValues frm = new FrmMacValues(FrmMacValueDataType.Resin, this.machine.MacNo, this.dtpWorkStart.Value, this.dtpWorkEnd.Value);
                    frm.ShowDialog();
                }
                else
                {
                    FrmMacValues frm = new FrmMacValues(FrmMacValueDataType.Resin);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        private void 設備条件情報編集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.machine != null)
                {
                    FrmMacValues frm = new FrmMacValues(FrmMacValueDataType.Condition, this.machine.MacNo, this.dtpWorkStart.Value, this.dtpWorkEnd.Value);
                    frm.ShowDialog();
                }
                else
                {
                    FrmMacValues frm = new FrmMacValues(FrmMacValueDataType.Condition);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ロット情報編集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmEditLot frm = new FrmEditLot(this.lot);
                frm.ShowDialog();
                this.lot = frm.Lot;
                this.setLotInfo();

                this.InspectionList = frm.InspectionList;
                this.setInspection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ロット資材追加ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.order == null) return;

            try
            {
                FrmMatInput frm = new FrmMatInput(this.lot, this.order, this.proc);
                frm.ShowDialog();
                setMaterialGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void FrmEditTran_FormClosing(object sender, FormClosingEventArgs e)
        {
            FrmMagazineMainte.RemoveTranFrmInstance(this.lot.NascaLotNo);
        }



        #region 不良情報更新ボタン
        
        /// <summary>
        /// 不良情報更新ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateDefect_Click(object sender, EventArgs e)
        {
            try
            {
                int total = 0;
                foreach (DefItem item in this.DefectList)
                {
                    total += item.DefectCt;
                }

                int empcd = 0;
                if (total > 0)
                {
                    if (string.IsNullOrEmpty(this.txtInspectEmpCd.Text))
                    {
                        MessageBox.Show(this, "検査結果がある場合は検査者の社員番号を入力してください。");
                        return;
                    }

                    if (!int.TryParse(this.txtInspectEmpCd.Text, out empcd))
                    {
                        MessageBox.Show(this, "社員番号は数字を入力してください。");
                        return;
                    }
                }

                //<--Y.Matsushima SGA-IM0000008218 2018/08/22
                //↓が既存処理です。参考までにおいています。
                // 四宮さん削除お願いします。
                //DialogResult res = MessageBox.Show(this, "不良情報を保存しますか?", "確認",
                //    MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                //if (res != DialogResult.OK)
                //{
                //    return;
                //}

                //不良数に変更があった不良のリスト作成
                List<DefItem> DefectChangeList = GetDefectChnageList();

                //不良一覧(変更部)を確認画面に渡す。
                // 戻り値 [true] : OKボタンが押された
                //        [false]: Cancel/画面を閉じた
                bool ret = FrmConfirmation.ShowForm(DefectChangeList); ;

                //確認画面で[OK]を押さなかった場合
                if (ret == false)
                {
                    return;
                }

                //-->Y.Matsushima SGA-IM0000008218 2018/08/22

                //検査者、検査数の更新
                Order ord = Order.GetMagazineOrder(this.lot.NascaLotNo, this.magSeqNo, this.proc.ProcNo);
                if (rdoInspectAll.Checked == true)
                {
                    ord.InspectCt = -1;
                }
                else
                {
                    int inspectct;
                    if (int.TryParse(this.txtInspectCt.Text, out inspectct))
                    {
                        ord.InspectCt = inspectct;
                    }
                    else
                    {
                        MessageBox.Show(this, "検査数は数字を入力してください。");
                        return;
                    }
                }

                if (ord.InspectCt != 0 && string.IsNullOrEmpty(this.txtInspectEmpCd.Text))
                {
                    MessageBox.Show(this, "検査数がある場合は検査者の社員番号を入力してください。");
                    return;
                }

                if (!string.IsNullOrEmpty(this.txtInspectEmpCd.Text))
                {
                    int num;
                    if (!int.TryParse(this.txtInspectEmpCd.Text, out num))
                    {
                        MessageBox.Show(this, "検査者の社員番号は数字を入力してください。");
                        return;
                    }
                    empcd = num;
                }

                if (empcd == 0)
                {
                    ord.InspectEmpCd = string.Empty;
                }
                else
                {
                    ord.InspectEmpCd = empcd.ToString();
                }
                ord.DeleteInsert(ord.LotNo, true);


                //不良情報更新
                updateDefect();

                if (order.IsNascaEnd)
                {
                    MessageBox.Show("このロットはNASCA連携済みです。\nNASCA側の履歴も修正してください");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //<--Y.Matsushima SGA-IM0000008218 2018/08/22
        /// <summary>
        /// 不良数を変更した不良のリスト作成
        /// </summary>
        /// <returns></returns>
        private List<DefItem> GetDefectChnageList()
        {
            List<DefItem> DefectChangeList = new List<DefItem>();

            ////変更前の不良数を取得
            //DefItem[] org = Defect.GetAllDefect(this.lot, this.proc.ProcNo);
            //this.orgDefectList = org.OrderBy(i => i.CauseCd).ThenBy(i => i.ClassCd).ThenBy(i => i.OrderNo).ToArray();

            this.grdDefect.EndEdit();
            Defect def = new Defect();
            def.LotNo = this.lot.NascaLotNo;
            def.ProcNo = int.Parse(this.txtProcNo.Text);
            def.DefItems = new List<DefItem>();

            //変更前後の不良数を比較して、違いがあればDefectChangeList[不良リスト(変更部)]に追加
            //  [変更前]this.orgDefectList
            //  [変更後]this.grdDefect.Rows
            int cnt = 0;            //変更前後を比較する為の行数合わせ用
            int beforeDefectCt = 0; //変更前の不良数
            foreach (DataGridViewRow row in this.grdDefect.Rows)
            {

                DefItem d = new DefItem();
                d.CauseCd = row.Cells["CauseCd"].Value.ToString();
                d.CauseName = row.Cells["CauseName"].Value.ToString();
                d.ClassCd = row.Cells["ClassCd"].Value.ToString();
                d.ClassName = row.Cells["ClassName"].Value.ToString();
                d.DefectCd = row.Cells["DefectCd"].Value.ToString();
                d.DefectName = row.Cells["DefectName"].Value.ToString();
                d.DefectCt = Convert.ToInt32(row.Cells["DefectCt"].Value);

                beforeDefectCt = Convert.ToInt32(row.Cells["OrgDefectCt"].Value);

                //変更前後の不良数を比較し、変更があった場合、不良変更リストに追加
                if (d.DefectCt != beforeDefectCt)
                {
                    DefectChangeList.Add(d);
                }
                cnt = cnt + 1;
            }

            return DefectChangeList;
        }
        //-->Y.Matsushima SGA-IM0000008218 2018/08/22
        #endregion

        private void メンテ履歴参照ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FrmHistoryView frm = new FrmHistoryView(this.lot.NascaLotNo);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void rdoInspectSample_CheckedChanged(object sender, EventArgs e)
        {
            this.rdoInspectAll.Checked = !rdoInspectSample.Checked;
            this.txtInspectCt.Enabled = rdoInspectSample.Checked;
        }

        private void chkNascaStart_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkNascaEnd_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void grdMaterials_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
