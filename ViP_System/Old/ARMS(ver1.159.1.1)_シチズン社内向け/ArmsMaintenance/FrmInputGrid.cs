using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArmsMaintenance
{
    public partial class FrmInputGrid : Form
    {
        public List<string> ValList
        {
            get
            {
                List<string> retv = new List<string>();
                if (this.grdItems.Rows.Count > 0)
                {
                    for (int i = 0; i < this.grdItems.Rows.Count; i++)
                    {
                        if (Convert.ToString(this.grdItems.Rows[i].Cells[0].Value) != "")
                        {
                            retv.Add(Convert.ToString(this.grdItems.Rows[i].Cells[0].Value));
                        }
                    }
                }

                return retv;
            }
        }

        public FrmInputGrid(List<string> valList)
        {
            InitializeComponent();

            this.setGrid(valList);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.pasteClipboard();
        }


        /// <summary>
        /// コピー内容をグリッドに出力
        /// </summary>
        private void pasteClipboard()
        {
            try
            {
                List<string> valList = new List<string>();

                Dictionary<string, string> pasteDictionary = new Dictionary<string, string>();

                string val = "";

                IDataObject iData = Clipboard.GetDataObject();
                if (iData.GetDataPresent(DataFormats.Text))
                {
                    string[] datalist = iData.GetData(DataFormats.Text).ToString().Split(new char[] { '\r' });

                    foreach (string data in datalist)
                    {
                        object[] splitdata = data.Split(new char[] { '\t' });

                        val = splitdata[0].ToString().Replace("\n", "");

                        //空白は追加しない
                        if (val == "" || val == "\n") continue;

                        //重複を省いて格納
                        if (valList.Contains(val) == false)
                        {
                            valList.Add(val);
                        }
                    }
                }

                this.setGrid(valList);
            }
            catch { }
        }


        private void setGrid(List<string> valList)
        {
            this.grdItems.Rows.Clear();
            foreach (string v in valList)
            {
                this.grdItems.Rows.Add();
                this.grdItems.Rows[this.grdItems.NewRowIndex - 1].Cells[0].Value = v;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.grdItems.Rows.Clear();
        }
    }
}
