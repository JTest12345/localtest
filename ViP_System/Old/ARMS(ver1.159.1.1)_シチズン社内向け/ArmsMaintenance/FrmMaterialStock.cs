using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArmsApi.Model;

namespace ArmsMaintenance
{
    public partial class FrmMaterialStock : Form
    {
        public FrmMaterialStock()
        {
            InitializeComponent();
        }

        public class Material : ArmsApi.Model.Material
        {
            public bool ChangeFg { get; set; }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            search(txtLotNo.Text.Trim());
        }

        private void search(string lotNo)
        {
            List<Material> showList = new List<Material>();

            List<ArmsApi.Model.Material> materials = Material.GetMaterials(lotNo, true).ToList();
            foreach (ArmsApi.Model.Material mat in materials)
            {
                Material m = new Material();
                m.MaterialCd = mat.MaterialCd;
                m.LotNo = mat.LotNo;
                m.StockCt = mat.StockCt;
                m.StockLastUpdDt = mat.StockLastUpdDt;
                showList.Add(m);
            }
          
            bsItems.DataSource = showList;
        }

        private void grdItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }

            if (grdItems.Columns["ChangeFG"].Index == e.ColumnIndex)
            {
                return;
            }

            ((DataGridViewCheckBoxCell)grdItems.Rows[e.RowIndex].Cells["ChangeFG"]).Value = true;

            grdItems.EndEdit();

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bsItems.EndEdit();
                grdItems.EndEdit();

                if (DialogResult.Cancel == MessageBox.Show("変更した内容を保存してよろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    return;
                }

                List<Material> changeList
                    = ((List<Material>)bsItems.DataSource).Where(p => p.ChangeFg).ToList();
                if (changeList.Count == 0)
                {
                    throw new ApplicationException("変更対象がありません。");
                }

                if (changeList.Where(c => string.IsNullOrEmpty(c.UpdUserCd)).Count() != 0)
                {
                    throw new ApplicationException("更新者CDを入力して下さい。");
                }
            
                foreach (Material c in changeList)
                {
                    Material.UpdateStockCount(c.MaterialCd, c.LotNo, c.StockCt, c.UpdUserCd);
                }

                MessageBox.Show("登録完了しました。", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmMaterialStock_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
