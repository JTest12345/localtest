using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GEICS
{
    public partial class M11_General : Form
    {
        public List<General> SelectGenerals { get; set; }

        private string GeneralGrpCD;
        public M11_General(string generalGrpCD)
        {
            InitializeComponent();
            this.GeneralGrpCD = generalGrpCD;

            SelectGenerals = new List<General>();
        }

        private void M11_General_Load(object sender, EventArgs e)
        {
            bsGeneral.DataSource = General.GetGeneralData(this.GeneralGrpCD);
        }

        private void dgvItems_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                DataGridViewCheckBoxCell targetCell = ((DataGridViewCheckBoxCell)dgvItems.Rows[e.RowIndex].Cells["CheckFG"]);
                if (Convert.ToBoolean(targetCell.Value))
                {
                    targetCell.Value = false;
                }
                else
                {
                    targetCell.Value = true;
                }
                dgvItems.EndEdit();
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolDataReturn_Click(object sender, EventArgs e)
        {
            try
            {
				//if (dgvItems.Rows.Count == 0)
				//{
				//    throw new ApplicationException(Constant.MessageInfo.Message_9);
				//}

                List<General> genes = (List<General>)bsGeneral.DataSource;
                genes = genes.Where(p => p.IsCheck).ToList();
				//if (genes.Count == 0)
				//{
				//    throw new ApplicationException(Constant.MessageInfo.Message_9);
				//}

                SelectGenerals = genes;

                this.Close();
                this.Dispose();
            }
            catch (ApplicationException err)
            {
                MessageBox.Show(err.Message, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
