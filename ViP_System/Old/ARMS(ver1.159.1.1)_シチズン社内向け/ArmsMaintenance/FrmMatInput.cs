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

namespace ArmsMaintenance
{
    public partial class FrmMatInput : Form
    {
        private AsmLot asmlot;
        private Order order;
        private Process proc;

        private Material[] matlist;

        public FrmMatInput(AsmLot lot, Order ord, Process prc)
        {
            InitializeComponent();

            this.asmlot = lot;
            this.order = ord;
            this.proc = prc;
            this.txtTypeCd.Text = lot.TypeCd;
            this.txtAsmLotNo.Text = lot.NascaLotNo;
        }

        private void btnSelectMat_Click(object sender, EventArgs e)
        {
            try
            {
                FrmSelectMaterial frm = new FrmSelectMaterial();
                DialogResult res = frm.ShowDialog();

                if (res == DialogResult.OK)
                {
                    this.txtLotNO.Text = frm.SelectedMat.LotNo;
                    this.txtMaterialCd.Text = frm.SelectedMat.MaterialCd;
                    this.txtMaterialNm.Text = frm.SelectedMat.MaterialNm;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnInsertMat_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtLotNO.Text)) return;

            Material newmat = Material.GetMaterial(this.txtMaterialCd.Text, this.txtLotNO.Text);
            if (newmat == null)
            {
                MessageBox.Show("原材料レコードが見つかりません");
                return;
            }

            Material[] matlist = order.GetRelatedMaterials();

            List<Material> newlist = new List<Material>();
            foreach (Material mat in matlist)
            {
                if (mat.MaterialCd == newmat.MaterialCd && mat.LotNo == newmat.LotNo)
                {
                    MessageBox.Show("既に割付済みです");
                    return;
                }
                else
                {
                    newlist.Add(mat);
                }
            }
            newlist.Add(newmat);
            
            string errMsg;
            newmat.InputDt = DateTime.Now;
            bool isError = WorkChecker.IsErrorBeforeInputMat(newmat, asmlot, proc.WorkCd, out errMsg);
            if (isError)
            {
                MessageBox.Show("照合エラー:" + errMsg);
                return;
            }
            
            this.order.UpdateMaterialRelation(newlist.ToArray());
            updateGrid();
        }

        private void FrmMatInput_Load(object sender, EventArgs e)
        {
            updateGrid();
        }


        private void updateGrid()
        {
            this.matlist = order.GetRelatedMaterials();
            this.grdMatList.DataSource = this.matlist;

            #region グリッド表示列変更
            
            foreach (DataGridViewColumn col in this.grdMatList.Columns)
            {
                switch (col.Name)
                {
                    case "ColChk":
                        break;


                    case "MaterialCd":
                        col.HeaderText = "品目CD";
                        col.ReadOnly = true;
                        break;


                    case "LotNo":
                        col.HeaderText = "ロット番号";
                        col.ReadOnly = true;
                        break;

                    case "MaterialNm":
                        col.HeaderText = "原材料名称";
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
            #endregion

        }

        private void button1_Click(object sender, EventArgs e)
        {

            List<Material> newlist = new List<Material>();
            foreach (DataGridViewRow row in this.grdMatList.Rows)
            {
                object cell = row.Cells["ColChk"].Value;

                if ((bool?)cell != true)
                {
                    Material mat = row.DataBoundItem as Material;
                    if (mat == null) continue;

                    newlist.Add(mat);
                }
            }

            this.order.UpdateMaterialRelation(newlist.ToArray());
            updateGrid();
        }
    }
}
