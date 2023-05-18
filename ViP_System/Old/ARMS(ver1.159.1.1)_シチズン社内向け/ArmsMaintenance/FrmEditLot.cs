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
    public partial class FrmEditLot : Form
    {
        public AsmLot Lot { get; set; }

        public Inspection[] InspectionList { get; set; }

        private Process[] workflow;

        public FrmEditLot(AsmLot lot)
        {
            InitializeComponent();
            this.Lot = lot;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                updateLotInfo();
                updateInspection();

                //LENSのタイプ変更
                Lot.updateLensLot(this.txtLotNo.Text, this.txtTypeCd.Text);

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        /// <summary>
        /// ロット情報をフォームに反映
        /// </summary>
        private void setLotInfo()
        {
            this.txtLotNo.Text = this.Lot.NascaLotNo;
            this.txtTypeCd.Text = this.Lot.TypeCd;
            this.txtBlendCd.Text = this.Lot.BlendCd;

            this.txtResinGr.Text = string.Join(",", this.Lot.ResinGpCd);
            this.txtResinGr2.Text = string.Join(",", this.Lot.ResinGpCd2);
            this.txtCutBlendCd.Text = this.Lot.CutBlendCd;
            // Ver1.99.0 予定選別規格 追加
            this.txtScheduleSelectionStandard.Text = this.Lot.ScheduleSelectionStandard;

            this.chkColorTest.Checked = this.Lot.IsColorTest;
            this.chkWarning.Checked = this.Lot.IsWarning;
            this.chkKHLTest.Checked = this.Lot.IsKHLTest;
            this.chkLifeTest.Checked = this.Lot.IsLifeTest;
            this.numLotsize.Value = this.Lot.LotSize;
            this.chkFullInspection.Checked = this.Lot.IsFullSizeInspection;
            this.chkWBMapping.Checked = this.Lot.IsMappingInspection;
            this.chkChangePointLot.Checked = this.Lot.IsChangePointLot;
            this.txtMacGroup.Text = string.Join(",", this.Lot.MacGroup);
            this.chkLoopCondChange.Checked = this.Lot.IsLoopCondChange;

			this.numMoveStockCt.Value = this.Lot.MoveStockCt;

            this.txtProfileId.Text = this.Lot.ProfileId.ToString().Trim();
            this.txtProfileNm.Text = Profile.GetProfile(this.Lot.ProfileId).ProfileNm;
            this.txtDBThrowDt.Text = this.Lot.DBThrowDT;
        }


        /// <summary>
        /// フォーム情報からロット情報を更新
        /// </summary>
        private void updateLotInfo()
        {
            this.Lot.TypeCd = this.txtTypeCd.Text;
            this.Lot.BlendCd = this.txtBlendCd.Text;
            this.Lot.ResinGpCd = this.txtResinGr.Text.Split(',').ToList();
            this.Lot.ResinGpCd2 = this.txtResinGr2.Text.Split(',').ToList();
            this.Lot.CutBlendCd = this.txtCutBlendCd.Text;
            // Ver1.99.0 予定選別規格 追加
            this.Lot.ScheduleSelectionStandard = this.txtScheduleSelectionStandard.Text.Trim();

            this.Lot.IsColorTest = this.chkColorTest.Checked;
            this.Lot.IsWarning = this.chkWarning.Checked;
            this.Lot.IsKHLTest = this.chkKHLTest.Checked;
            this.Lot.IsLifeTest = this.chkLifeTest.Checked;
            this.Lot.LotSize = (int)this.numLotsize.Value;
            this.Lot.IsMappingInspection = this.chkWBMapping.Checked;
            this.Lot.IsFullSizeInspection = this.chkFullInspection.Checked;
            this.Lot.IsChangePointLot = this.chkChangePointLot.Checked;
            this.Lot.MacGroup = this.txtMacGroup.Text.Split(',').ToList();
            this.Lot.IsLoopCondChange = this.chkLoopCondChange.Checked;

            this.Lot.MoveStockCt = (int)this.numMoveStockCt.Value;

            this.Lot.ProfileId = int.Parse(this.txtProfileId.Text);
            this.Lot.DBThrowDT = this.txtDBThrowDt.Text;

            this.Lot.Update();
            
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateInspection()
        {
            foreach (DataGridViewRow row in this.grdInspections.Rows)
            {
                Inspection isp = (Inspection)row.DataBoundItem;
                isp.DeleteInsert();
            }
        }


        /// <summary>
        /// 抜き取り検査情報の更新
        /// </summary>
        private void setInspection()
        {
            Inspection[] specs = Inspection.GetInspections(this.Lot.NascaLotNo);
            this.InspectionList = specs;

            if (specs == null || specs.Length == 0)
            {
                this.grdInspections.DataSource = null;
                return;
            }

            this.grdInspections.DataSource = this.InspectionList;

            this.grdInspections.Columns["LotNo"].Visible = false;
            this.grdInspections.Columns["ProcNm"].ReadOnly = true;
            this.grdInspections.Columns["ProcNm"].HeaderText = "抜取り検査工程";
            this.grdInspections.Columns["ProcNo"].Visible = false; ;
            this.grdInspections.Columns["IsInspected"].DefaultCellStyle.BackColor = Color.Yellow;
            this.grdInspections.Columns["IsInspected"].HeaderText = "検査済み";
        }

        private void setProc()
        {
            workflow = Process.GetWorkFlow(this.Lot.TypeCd);
            this.cmbProc.DataSource = workflow;
            this.cmbProc.DisplayMember = "InlineProNM";
            this.cmbProc.ValueMember = "ProcNo";
        }

        private void FrmEditLot_Load(object sender, EventArgs e)
        {
            setLotInfo();
            setInspection();
            setProc();
        }

        private void btnAddInspection_Click(object sender, EventArgs e)
        {
            try
            {
                FrmPasswordDialog dlg = new FrmPasswordDialog(this.Lot.NascaLotNo,
                    "状態検査フラグを追加します\n　処理を続行する場合は" + this.Lot.NascaLotNo + "と入力してください");

                DialogResult res = dlg.ShowDialog();
                if (res == DialogResult.OK)
                {
                    int procno = (int)cmbProc.SelectedValue;
                    Inspection isp = new Inspection();
                    isp.LotNo = this.Lot.NascaLotNo;
                    isp.ProcNo = procno;
                    isp.DeleteInsert();

                    setInspection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }   
        }

        private void btnSelectProfile_Click(object sender, EventArgs e)
        {
            try
            {
                Order[] orders = Order.GetOrder(this.txtLotNo.Text.Trim());
                /*
                if (orders.Count() >= 2)
                {
                    throw new Exception("実績が2件以上ある場合は変更できません。");
                }
                */

                FrmSelectProfile frm = new FrmSelectProfile(true);
                DialogResult res = frm.ShowDialog(this);
                if (res != DialogResult.OK) return;
                if (frm.Selected == null) return;

                txtProfileId.Text = frm.Selected.ProfileId.ToString();
                txtProfileNm.Text = frm.Selected.ProfileNm;

                Profile p = Profile.GetProfile(frm.Selected.ProfileId);
                txtTypeCd.Text = p.TypeCd;
                txtBlendCd.Text = p.BlendCd;
                txtResinGr.Text = string.Join(",", p.ResinGpCd);
                txtResinGr2.Text = string.Join(",", p.ResinGpCd2);
                txtCutBlendCd.Text = p.CutBlendCd;
                numLotsize.Value = p.LotSize;
                txtDBThrowDt.Text = p.DBThrowDt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
