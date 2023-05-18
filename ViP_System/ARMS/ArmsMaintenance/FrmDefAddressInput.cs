using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi.Model;

namespace ArmsMaintenance
{
    public partial class FrmDefAddressInput : Form
    {
        string magno;
        DefItem def;
        AsmLot lot;
        string empcd;

        public FrmDefAddressInput(string magno, DefItem def, AsmLot lot, string empcd)
        {
            InitializeComponent();
            this.magno = magno;
            this.def = def;
            this.lot = lot;
            this.empcd = empcd;

            this.txtLotno.Text = lot.NascaLotNo;
            this.txtDefName.Text = def.DefectName;
        }

        #region テンキークリック動作
        private void btnNum0_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("0");
            SendKeys.Flush();
        }

        private void btnNum1_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("1");
            SendKeys.Flush();
        }

        private void btnNum2_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("2");
            SendKeys.Flush();
        }

        private void btnNum3_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("3");
            SendKeys.Flush();
        }

        private void btnNum4_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("4");
            SendKeys.Flush();
        }

        private void btnNum5_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("5");
            SendKeys.Flush();
        }

        private void btnNum6_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("6");
            SendKeys.Flush();
        }

        private void btnNum7_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("7");
            SendKeys.Flush();
        }

        private void btnNum8_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("8");
            SendKeys.Flush();
        }

        private void btnNum9_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("9");
            SendKeys.Flush();
        }

        private void btnNumDel_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{BACKSPACE}");
            SendKeys.Flush();
        }
        #endregion

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                updateEicsWBAddress(this.txtAddress.Text, this.txtUnit.Text);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show("登録を中止しました：" + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("登録を中止しました：" + ex.ToString());
            }

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// EICSテーブル更新
        /// </summary>
        private void updateEicsWBAddress(string address, string unit)
        {
			if (string.IsNullOrEmpty(address) && string.IsNullOrEmpty(unit))
			{
				// アドレス, ユニットが両方空白なら何もしない
				return;
			}
			else if (string.IsNullOrEmpty(address) && string.IsNullOrEmpty(unit) == false)
			{
				throw new ApplicationException("ユニットが入力されているのにアドレスが空白です");
			}

			if (string.IsNullOrWhiteSpace(unit))
			{
				unit = "0";
			}

			int unitno;
			if (!int.TryParse(unit, out unitno))
			{
				throw new ApplicationException("ユニットに数値変換できない文字が入力されています。");
			}

			//2015.2.13 LENS2改修
            //Defect.UpdateEICSWireBondAddress(magno, lot, def, address, unit, empcd);
			ArmsApi.Model.LENS.MacDefect.InsertUpdate(magno, lot, def, address, unit, empcd);
        }

    }
}
