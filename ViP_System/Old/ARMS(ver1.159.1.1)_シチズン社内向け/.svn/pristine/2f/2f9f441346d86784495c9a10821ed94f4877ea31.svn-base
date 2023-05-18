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
    public partial class FrmSelectMaterial : Form
    {

        public Material SelectedMat;

        public List<Material> SelectedMatList = new List<Material>();

        private Material[] matlist;

        private List<string> selectedLotList = new List<string>();

        /// <summary>
        /// 複数選択モード
        /// </summary>
        private bool multiMode;

        public FrmSelectMaterial()
        {
            InitializeComponent();
            this.setMultiSelect(false);
        }

        public FrmSelectMaterial(bool multifg)
        {
            InitializeComponent();
            this.setMultiSelect(multifg);
        }

        public FrmSelectMaterial(bool multifg, List<Material> selected)
        {
            InitializeComponent();
            this.setMultiSelect(multifg);
            this.matlist = selected.ToArray();
            applyGrid(true);
        }


        private void setMultiSelect(bool multifg)
        {
            this.multiMode = multifg;
            this.btnSelectLot.Visible = multifg;
            this.grdMaterials.MultiSelect = multifg;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {

                this.matlist = new Material[0];

                if (rdoLotSearch.Checked)
                {
                    this.matlist = searchByMatLot();
                }
                else if (rdoAsmLot.Checked)
                {
                    this.matlist = searchByAsmLot();
                }

                applyGrid(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void applyGrid(bool selected)
        {
			if (matlist == null) return;

            this.grdMaterials.DataSource = matlist;
            this.grdMaterials.Columns["StockerNo"].Visible = false;
            this.grdMaterials.Columns["InputDt"].Visible = false;
            this.grdMaterials.Columns["RemoveDt"].Visible = false;
            this.grdMaterials.Columns["IsRemoved"].Visible = false;
            this.grdMaterials.Columns["MaterialCd"].HeaderText = "原材料CD";
            this.grdMaterials.Columns["MaterialNm"].HeaderText = "原材料名";
            this.grdMaterials.Columns["LotNo"].HeaderText = "ロット";
            this.grdMaterials.Columns["LimitDt"].HeaderText = "使用期限";
           
            if (selected)
            {
                this.grdMaterials.SelectAll();
            }
        }


        private Material[] searchByAsmLot()
        {
            List<Material> retv = new List<Material>();
            if (string.IsNullOrWhiteSpace(txtAsmLot.Text)) return retv.ToArray();

            AsmLot lot = AsmLot.GetAsmLot(this.txtAsmLot.Text);
            if (lot == null)
            {
                MessageBox.Show("アッセンロットが見つかりません");
                return new Material[0];
            }

            int fromProcNo = ((Process)cmbDBProc.SelectedItem).ProcNo;
            int currentProcNo = ((Process)cmbCurrentProc.SelectedValue).ProcNo;

            int rowno = (int)this.numMagRow.Value;

            //指定工程との間に反転がある場合は段数を反転
            bool isReverse = Process.IsReverseFramePlacement(lot.NascaLotNo, fromProcNo, currentProcNo);
            if (isReverse == true)
            {
                int? step = ArmsApi.Model.LENS.Mag.GetMagStep(lot.TypeCd);
                int? stepcd = ArmsApi.Model.LENS.Mag.GetMagStepCd(lot.TypeCd);
                if (step == null || stepcd == null)
                {
                    MessageBox.Show("LENS TmMagマスタにStepCdの設定がありません:" + lot.TypeCd);
                    return new Material[0];
                }
                rowno = Magazine.ReverseRow(rowno, step.Value, stepcd.Value);
            }


            Order[] orders = Order.GetOrder(lot.NascaLotNo, fromProcNo);
            if (orders.Length == 0)
            {
                MessageBox.Show("作業実績が見つかりません:" + lot.NascaLotNo + ":" + cmbDBProc.Text);
                return new Material[0];
            }
            MachineInfo mac = MachineInfo.GetMachine(orders[0].MacNo);
            DateTime workEndDt = orders[0].WorkEndDt.Value;

            DBWaferLog[] logs = DBWaferLog.GetAllLogs(mac.NascaPlantCd, workEndDt.AddDays(-1), workEndDt);
            logs = logs.OrderByDescending(l => l.LogDT).ToArray();

            bool isLotChange = false;
            foreach (var log in logs)
            {
                if (log.KB == DBWaferLog.LogKB.ロット完成 && log.AsmLotNo != lot.NascaLotNo)
                {
                    //別ロットの完成レコード以降のウェハー交換については段数判定しない
                    isLotChange = true;
                }

                if (log.KB == DBWaferLog.LogKB.ウェハー段数変更_自動 || log.KB == DBWaferLog.LogKB.ウェハー段数変更_手動)
                {
                    if (isLotChange)
                    {
                        //ロット交換を挟んでいた場合は最初に見つかったウェハーだけで終了
                        retv.Add(log.Wafer);
                        break;
                    }

                    if (log.MagRowNo > rowno)
                    {
                        //検索段数以降の交換ログは無視する
                        continue;
                    }

                    if (log.MagRowNo < rowno)
                    {
                        //検索段数より前の交換ログが見つかった場合も終了
                        retv.Add(log.Wafer);
                        break;
                    }

                    if (log.MagRowNo == rowno)
                    {
                        //検索段数と同一段数の交換ログなら前半に別ウェハーが使われているので続行
                        retv.Add(log.Wafer);
                    }
                }
            }

            if (retv.Count == 0)
            {
                MessageBox.Show("過去24時間以内にウェハー交換ログが有りません。異常なロットなので別の方法でトレースしてください。");
            }
            else
            {
            }

            return retv.ToArray();
        }

        private Material[] searchByMatLot()
        {
            try
            {
                List<string> lotlist = new List<string>();

                if (selectedLotList.Count >= 1) lotlist.AddRange(selectedLotList);
                if (string.IsNullOrEmpty(this.txtLotNo.Text) == false) lotlist.Add(this.txtLotNo.Text);

                //検索条件ロットが全く指定されていない場合は全検索
                if (lotlist.Count == 0) lotlist.Add("");

                List<Material> list = new List<Material>();
                foreach (string lot in lotlist)
                {
                    Material[] m = Material.GetMaterials(null, this.txtMatNm.Text, lot, true, false);
                    for (int i = 0; i < m.Length; i++)
                    {
                        list.Add(m[i]);
                    }
                }
                return list.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return new Material[0];
            }
        }


        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.grdMaterials.SelectedRows.Count == 0)
                {
                    return;
                }

                if (this.rdoAsmLot.Checked)
                {
                    MessageBox.Show("対象ロットがフレーム(基板)廃棄していないか確認してください。フレーム(基板)廃棄している場合は正常にトレースできません");
                }

                if (this.multiMode == false)
                {
                    SelectedMat = (Material)this.grdMaterials.SelectedRows[0].DataBoundItem;
                }
                else
                {
                    foreach (DataGridViewRow row in this.grdMaterials.SelectedRows)
                    {
                        SelectedMatList.Add((Material)row.DataBoundItem);
                    }
                }

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSelectLot_Click(object sender, EventArgs e)
        {
            try
            {
                FrmInputGrid frm = new FrmInputGrid(this.selectedLotList);
                DialogResult res = frm.ShowDialog();

                if (res == DialogResult.OK)
                {
                    this.selectedLotList = frm.ValList;

                    if (frm.ValList.Count > 0)
                        this.btnSelectLot.ImageIndex = 0;
                    else
                        this.btnSelectLot.ImageIndex = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void FrmSelectMaterial_Load(object sender, EventArgs e)
        {
            Process[] procs = Process.GetWorkflowProcess();
            this.cmbCurrentProc.DataSource = procs;
            this.cmbCurrentProc.DisplayMember = "InlineProNM";

            Process[] procs2 = Process.GetWorkflowProcess();
            this.cmbDBProc.DataSource = procs2;
            this.cmbDBProc.DisplayMember = "InlineProNM";

            applyGrid(true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.grdMaterials.SelectAll();
        }
    }
}
