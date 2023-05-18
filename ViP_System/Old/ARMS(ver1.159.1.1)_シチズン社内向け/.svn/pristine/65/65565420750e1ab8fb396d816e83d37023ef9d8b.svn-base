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


    public partial class FrmEditMagazine : Form
    {
        private Mode editMode;

        Magazine originalMag;
        string originalLot;

        enum Mode
        {
            New,
            Edit,
        }

        public FrmEditMagazine(Magazine mag)
        {
            this.editMode = Mode.Edit;
            this.originalMag = mag;
            InitializeComponent();
        }

        public FrmEditMagazine(string lotno)
        {
            this.editMode = Mode.New;
            this.originalLot = lotno;
            InitializeComponent();
        }


        private void FrmEditMagazine_Load(object sender, EventArgs e)
        {
            if (editMode == Mode.Edit)
            {
                AsmLot lot = AsmLot.GetAsmLot(originalMag.NascaLotNO);

                if (lot == null)
                {
                    MessageBox.Show(this, "ロット情報が見つかりません:" + originalMag.NascaLotNO, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                Process[] plist = Process.GetWorkFlow(lot.TypeCd);

                this.txtLotNo.Text = originalMag.NascaLotNO;
                this.txtTypeCd.Text = lot.TypeCd;
                this.txtMagNo.Text = originalMag.MagazineNo;
                this.txtMagNo.ReadOnly = true;
                this.txtResinGpCd.Text = string.Join(",", lot.ResinGpCd);
                this.chkActive.Checked = originalMag.NewFg;


                this.cmbNowCompProcess.DataSource = plist;
                this.cmbNowCompProcess.ValueMember = "ProcNo";
                this.cmbNowCompProcess.DisplayMember = "InlineProNM";
                this.cmbNowCompProcess.SelectedValue = originalMag.NowCompProcess;
            }



            if (editMode == Mode.New)
            {
                AsmLot lot = AsmLot.GetAsmLot(this.originalLot);
                if (lot == null)
                {
                    MessageBox.Show(this, "ロット情報が見つかりません:" + this.originalLot, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                Process[] plist = Process.GetWorkFlow(lot.TypeCd);

                this.txtLotNo.Text = lot.NascaLotNo;
                this.txtTypeCd.Text = lot.TypeCd;
                this.txtResinGpCd.Text = string.Join(",", lot.ResinGpCd);
                this.chkActive.Checked = true;
                this.chkActive.Enabled = false;

                this.cmbNowCompProcess.DataSource = plist;
                this.cmbNowCompProcess.ValueMember = "ProcNo";
                this.cmbNowCompProcess.DisplayMember = "InlineProNM";
            }
        }


        #region 更新ボタン
        
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                Magazine mag = new Magazine();
                mag.MagazineNo = this.getMagazineNoInputChar();
                mag.NascaLotNO = this.txtLotNo.Text;
                mag.NowCompProcess = (int)this.cmbNowCompProcess.SelectedValue;
                mag.NewFg = this.chkActive.Checked;

                //ロット情報の存在チェック
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                if (lot == null)
                {
                    MessageBox.Show(this, "ロット情報が見つかりません\n更新を中止します",
                        "error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //同一マガジンの別レコード存在チェック
                if (mag.NewFg == true)
                {
                    Magazine[] exists = Magazine.GetMagazine(mag.MagazineNo, null, true, null);

                    foreach (Magazine otherMag in exists)
                    {
                        if (otherMag.NascaLotNO != mag.NascaLotNO)
                        {
                            MessageBox.Show(this, "このマガジンは他のロット番号で現在稼働中の状態になっています\n更新を中止します",
                                "error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                }

                //新規追加時は同一のマガジン：ロットの存在チェック
                if (editMode == Mode.New)
                {
                    Magazine[] exists = Magazine.GetMagazine(mag.MagazineNo, mag.NascaLotNO, false, null);

                    if (exists.Length > 0)
                    {
                        MessageBox.Show(this, "同一のマガジン：ロットが既に存在しています\n更新を中止します",
                            "error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    exists = Magazine.GetMagazine(null, mag.NascaLotNO, true, null);
                    if (exists.Length > 0)
                    {
                        MessageBox.Show(this, "このロットは別のマガジンで現在稼働中です\n更新を中止します",
                            "error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    exists = Magazine.GetMagazine(mag.MagazineNo, null, true, null);
                    if (exists.Length > 0)
                    {
                        MessageBox.Show(this, "このマガジンは別ロットで現在稼働中です\n更新を中止します",
                            "error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    //メンテ履歴記録
                    FrmMainteHistory frm = new FrmMainteHistory(mag.NascaLotNO, null, mag);
                    DialogResult res = frm.ShowDialog(this);
                    if (res != DialogResult.OK)
                    {
                        MessageBox.Show(this, "更新を中止します");
                        return;
                    }

                }
                else
                {
                    //更新処理の履歴保存
                    Magazine old = Magazine.GetCurrent(mag.MagazineNo);

                    //メンテ履歴記録
                    FrmMainteHistory frm = new FrmMainteHistory(mag.NascaLotNO, old, mag);
                    DialogResult res = frm.ShowDialog(this);
                    if (res != DialogResult.OK)
                    {
                        MessageBox.Show(this, "更新を中止します");
                        return;
                    }
                }

                mag.Update();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            this.Close();
        }
        #endregion


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
