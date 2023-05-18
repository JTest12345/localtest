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
    public partial class FrmManualNewOrder : Form
    {
        public FrmManualNewOrder()
        {
            InitializeComponent();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = MessageBox.Show(this, "現在のプロファイルで新規指図を発行します。\n処理を続行しますか？", "確認", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);

                if (res != DialogResult.OK)
                {
                    return;
                }

                if (string.IsNullOrEmpty(this.getMagazineNoInputChar()))
                {
                    MessageBox.Show("マガジン番号を入力してください");
                    return;
                }
                string magno = this.getMagazineNoInputChar();

                if (string.IsNullOrEmpty(this.txtMacNo.Text))
                {
                    MessageBox.Show("搭載装置を選択してください");
                    return;
                }

                Profile prof = null;
                //if (Config.Settings.UseDBProfile == false)
                //{
                //    prof = Profile.GetCurrentProfile();
                //}
                //else
                //{
                    prof = Profile.GetCurrentDBProfile(int.Parse(this.txtMacNo.Text));
                //}

                if (prof == null)
                {
                    MessageBox.Show("投入可能なプロファイルが存在しません");
                    return;
                }

                Process[] flow = Process.GetWorkFlow(prof.TypeCd);
                Process first = flow.Where(p => p.FirstSt == true).FirstOrDefault();

                if (first == null)
                {
                    MessageBox.Show("ワークフローに先頭工程が存在しません:" + prof.TypeCd);
                    return;
                }

                MachineInfo mac = MachineInfo.GetMachine(int.Parse(this.txtMacNo.Text));

                string cmd =
                    Config.Settings.InlineNo + ",NEL,011," + first.ProcNo.ToString() + "," + this.txtMacNo.Text + ",30 " + magno + ",30 " + magno
                    + "," + this.dtpWorkStartTime.Value.ToString() + "," + this.dtpWorkEndTime.Value.ToString();

                if (mac.HasWaferChanger)
                {
                    cmd += ",0-0,0-0";
                }
                else
                {
                    if (chkST1.Checked)
                    {
                        cmd += ",1";
                    }
                    else
                    {
                        cmd += ",0";
                    }

                    if (chkST2.Checked)
                    {
                        cmd += ",1";
                    }
                    else
                    {
                        cmd += ",0";
                    }
                }
                cmd += ",660,660";


                InlineAPI api = new InlineAPI();
                ArmsApiResponse ret = api.NelInput(cmd);

                //ロットに装置グループ登録
                Magazine svrmag = Magazine.GetCurrent(magno);
                AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                svrlot.MacGroup = mac.MacGroup;
                svrlot.Update();

                if (!ret.IsError)
                {
                    MessageBox.Show("発行完了");
                }
                else
                {
                    MessageBox.Show(ret.Message);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


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
                    this.txtMachine.Text = m.LongName;
                    this.txtMacNo.Text = m.MacNo.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 入力中のマガジンNO取得
        /// </summary>
        /// <returns></returns>
        private string getMagazineNoInputChar()
        {
            string magNo = string.Empty;
            if (!string.IsNullOrEmpty(txtMagNo.Text.Trim()))
            {
                string[] magChar = txtMagNo.Text.Trim().Split(' ');
                if (magChar.Count() >= 2)
                {
                    magNo = magChar[1];
                }
                else
                {
                    magNo = magChar[0];
                }
            }
            return magNo;
        }
    }
}
