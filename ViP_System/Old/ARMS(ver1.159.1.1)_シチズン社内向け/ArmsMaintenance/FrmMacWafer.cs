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
    public partial class FrmMacWafer : Form
    {
        MachineInfo machine;
        List<Material> waflist;

        public FrmMacWafer(MachineInfo m)
        {
            InitializeComponent();

            this.machine = m;
            this.txtMacNo.Text = machine.MacNo.ToString();
            this.txtMachine.Text = machine.MachineName;
            this.dtpInputDt.Value = DateTime.Now;
        }

        private void btnQR_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtQR.Text))
            {
                return;
            }

            string[] inputval = this.txtQR.Text.Split(',');
            waflist = new List<Material>();

            //QRコード内容分解
            for (int i = 0; i < inputval.Length; i++)
            {
                try
                {
                    string[] subval = inputval[i].Split(':');
                    if (subval.Length != 3)
                    {
                        MessageBox.Show("QRコード内容が不正です");
                        return;
                    }

                    string stocker = subval[0];
                    string matcd = subval[1];
                    string lotno = subval[2];

                    Material waf = Material.GetMaterial(matcd, lotno);
                    if (waf == null)
                    {
                        MessageBox.Show("ロット情報が存在しません：" + lotno);
                        return;
                    }

                    //YAGガラス・蛍光体ブロック対応。ウェハ縛り解除。2016.12.3 湯浅
                    //if (!waf.IsWafer)
                    //{
                    //    MessageBox.Show("ウェハーではありません：" + lotno);
                    //    return;
                    //}

                    int stockerNo;
                    if (int.TryParse(stocker, out stockerNo))
                    {
                        waf.StockerNo = stockerNo;
                    }
                    else
                    {
                        MessageBox.Show("ストッカー段数が不正です：" + stocker);
                        return;
                    }

                    waf.InputDt = DateTime.Now;
                    waflist.Add(waf);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }

            this.grdWafers.DataSource = this.waflist;

            foreach (DataGridViewColumn col in this.grdWafers.Columns)
            {
                switch (col.Name)
                {
                    case "MaterialCd":
                        col.HeaderText = "品目CD";
                        col.DisplayIndex= 3;
                        col.ReadOnly = true;
                        break;

                    case "StockerNo":
                        col.HeaderText = "No";
                        col.DisplayIndex= 1;
                        col.ReadOnly = true;
                        break;

                    case "LotNo":
                        col.HeaderText = "LotNo";
                        col.DisplayIndex = 2;
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
        }

        private void btnApplyEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.waflist == null || this.waflist.Count == 0) return;

                DialogResult res = MessageBox.Show(this, "カセットの割り付けを行います", "確認",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);

                if (res != DialogResult.OK) return;


                Material[] mats = machine.GetMaterials(DateTime.Now, DateTime.Now);
                
                //判定条件をiswaferからstockerno=0以外に変更。2016.12.3 湯浅
                if (mats.Where(m => m.IsWafer == true).Count() > 0)
                {
                    MessageBox.Show("先に既存のカセットを取り外してください");
                    return;
                }

                //投入前条件照合
                foreach (Material waf in waflist)
                {
                    string errmsg;
                    bool iserror = WorkChecker.IsErrorBeforeInputMat(waf, machine, out errmsg);

                    if (iserror)
                    {
                        MessageBox.Show("照合エラー:" + errmsg);
                        return;
                    }
                }

                foreach (Material waf in waflist)
                {
                    machine.DeleteInsertMacMat(waf);
                }

                MessageBox.Show("完了");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FrmMacWafer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                btnQR_Click(null, null);
            }
        }

        private void FrmMacWafer_Load(object sender, EventArgs e)
        {
            
        }

        private void FrmMacWafer_Shown(object sender, EventArgs e)
        {
            this.txtQR.Focus();
        }
    }
}
