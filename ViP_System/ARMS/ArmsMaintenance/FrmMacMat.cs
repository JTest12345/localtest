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
    public partial class FrmMacMat : Form
    {
        private MachineInfo machine;

        private Material original;

        /// <summary>
        /// 新規追加時コンストラクタ
        /// </summary>
        /// <param name="machine"></param>
        public FrmMacMat(MachineInfo machine)
        {
            InitializeComponent();
            this.machine = machine;

            this.txtMacNo.Text = machine.MacNo.ToString();
            this.txtMachine.Text = machine.LongName;

            this.dtpInputDt.Value = DateTime.Now;
            this.dtpInputDt.Enabled = true;
            this.dtpRemoveDt.Value = DateTime.Now;
            this.dtpRemoveDt.Enabled = false;
            this.chkRemove.Checked = false;
        }

        /// <summary>
        /// 更新時コンストラクタ
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="mat"></param>
        public FrmMacMat(MachineInfo machine, Material mat)
        {
            InitializeComponent();

            this.machine = machine;
            this.txtMacNo.Text = machine.MacNo.ToString();
            this.txtMachine.Text = machine.MachineName;

            this.original = mat;

            this.txtLotNO.Text = mat.LotNo;
            this.txtMaterialCd.Text = mat.MaterialCd;
            this.txtMaterialNm.Text = mat.MaterialNm;

            this.dtpInputDt.Value = mat.InputDt.Value;
            this.numStocker.Value = mat.StockerNo;

            if (mat.isRemoved)
            {
                this.chkRemove.Checked = true;
                this.dtpRemoveDt.Enabled = true;
                this.dtpRemoveDt.Value = mat.RemoveDt.Value;
            }
            else
            {
                this.dtpRemoveDt.Enabled = false;
                this.chkRemove.Checked = false;
                this.dtpRemoveDt.Value = DateTime.Now;
            }
        }


        /// <summary>
        /// 更新ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyEdit_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = MessageBox.Show(this, "更新を保存します", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (res != DialogResult.OK) return;

                Material m = Material.GetMaterial(this.txtMaterialCd.Text, txtLotNO.Text);
                if (m == null)
                {
                    MessageBox.Show("原材料が見つかりません:" + this.txtLotNO.Text);
                    return;
                }

                m.StockerNo = (int)this.numStocker.Value;
                m.InputDt = this.dtpInputDt.Value;

                if (chkRemove.Checked) m.RemoveDt = this.dtpRemoveDt.Value;
                else m.RemoveDt = null;

                string errmsg;
                bool iserror = WorkChecker.IsErrorBeforeInputMat(m, machine, out errmsg);
                if (iserror)
                {
                    MessageBox.Show("照合エラー:" + errmsg);
                    return;
                }

                try
                {
                    this.machine.DeleteInsertMacMat(m);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("更新エラー:" + ex.Message);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }




        /// <summary>
        /// 原材料選択処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        private void chkRemove_CheckedChanged(object sender, EventArgs e)
        {
            this.dtpRemoveDt.Enabled = chkRemove.Checked;
        }

        private void FrmMacMat_Load(object sender, EventArgs e)
        {
            //this.txtQR.Focus();
        }

        private void btnQR_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtQR.Text)) return;

            try
            {
                Material mat = new Material();
                string lotno;
                string[] inputval = this.txtQR.Text.Split(' ');

                if (chkRindId.Checked)//リングID
                {
                    mat = Material.GetMaterial(this.txtQR.Text.Trim());
                }
                else if (inputval.Length == 1)//ウェハラベル
                {
                    mat.LotNo = inputval[0];

                    //mat = Material.GetMaterial(null, lotno);
                }
                else if(inputval.Length == 3)//資材ラベル
                {
                    string labelkb = inputval[0];
                    string labelno = inputval[1];

                    string ggcode = Material.GetMaterialCdFromLabel(labelkb, labelno);
                    lotno = inputval[2];

                    if (ggcode == null)
                    {
                        this.txtQR.Clear();
                        MessageBox.Show("原材料ラベルのマスタに存在しません");
                        return;
                    }

                    mat = Material.GetMaterial(ggcode, lotno);
                    if (mat == null)
                    {
                        this.txtQR.Clear();
                        MessageBox.Show("原材料ロットが存在しません");
                        return;
                    }

                }
                else if (inputval.Length != 3)//ウェハラベル、資材ラベルのいずれでもない
                {
                    this.txtQR.Clear();
                    MessageBox.Show("バーコード内容が不正です");
                    return;
                }

                this.txtLotNO.Text = mat.LotNo;
                this.txtMaterialCd.Text = mat.MaterialCd;
                this.txtMaterialNm.Text = mat.MaterialNm;
                this.txtQR.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        /// <summary>
        /// EnterキーをQR読込ボタンクリックとして扱う
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMacMat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                btnQR_Click(null, null);
            }
        }

        private void FrmMacMat_Shown(object sender, EventArgs e)
        {
            this.txtQR.Focus();
        }
    }
}
