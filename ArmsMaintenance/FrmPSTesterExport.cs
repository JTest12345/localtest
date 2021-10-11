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
    public partial class FrmPSTesterExport : Form
    {
        private Magazine mag;

        public FrmPSTesterExport(Magazine mag)
        {
            InitializeComponent();
            this.mag = mag;
        }

        private void FrmPSTesterExport_Load(object sender, EventArgs e)
        {
            AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);

            if (lot == null)
            {
                MessageBox.Show(this, "ロット情報が見つかりません:" + mag.NascaLotNO, "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            List<KeyValuePair<int, string>?> cfglist = Config.Settings.PSTesterLinkInfo;
            if (cfglist == null || cfglist.Count == 0)
            {
                MessageBox.Show(this, "PSTesterへの出力設定が存在しません", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

             //WB工程ID
           int wbproc = cfglist.Where(c => !string.IsNullOrEmpty(c.Value.Value)).First().Value.Key;

            Order wborder = Order.GetMagazineOrder(mag.NascaLotNO, wbproc);
            if (wborder == null)
            {
                this.txtWBMacNm.Text = "未投入";
                this.btnExport.Enabled = false;
            }
            else
            {

                MachineInfo wbmac = MachineInfo.GetMachine(wborder.MacNo);
                if (wbmac == null)
                {
                    this.txtWBMacNm.Text = "マスタ不整合";
                    this.btnExport.Enabled = false;
                }
                else
                {
                    this.txtWBMacNm.Text = wbmac.MachineName;
                }
            }


            this.txtLotNo.Text = mag.NascaLotNO;
            this.txtTypeCd.Text = lot.TypeCd;
            this.txtMagNo.Text = mag.MagazineNo;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                this.mag.RecordAGVPSTester();

                this.mag.WritePSTesterFile();
                MessageBox.Show(this, "正常に終了しました");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "エラー:" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBallExport_Click(object sender, EventArgs e)
        {
            try
            {
                this.mag.WriteBDTesterFile();
                MessageBox.Show(this, "正常に終了しました");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "エラー:" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFilmExport_Click(object sender, EventArgs e)
        {
            try
            {
                this.mag.WriteFilmTesterFile();
                MessageBox.Show(this, "正常に終了しました");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "エラー:" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
