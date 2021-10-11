using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ArmsApi.Model;


namespace ArmsMaintenance
{
    public partial class FrmWorkViewReport : Form
    {
        public FrmWorkViewReport()
        {
            InitializeComponent();
        }


        private void btnOutput_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (chkExcel.Checked == false)
            {
                dlg.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            }
            else
            {
                dlg.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            }
            DialogResult res = dlg.ShowDialog();

            if (res != DialogResult.OK) return;

            DateTime? endfrom = (DateTime?)dtpWorkEndFrom.Value;
            DateTime? endto = (DateTime?)dtpWorkEndTo.Value;


            bool hasline = false;
            foreach (Process p in this.lstProcess.CheckedItems)
            {
                Order[] orders = Order.SearchOrder(null, null, p.ProcNo, null, false, false, null, null, endfrom, endto);

                foreach (Order o in orders)
                {
                    hasline = true;
                    AsmLot lot = AsmLot.GetAsmLot(o.LotNo);
                    Profile prof = null;
                    string shipid = string.Empty;
                    if (this.chkAimShipData.Checked || this.chkProfile.Checked)
                    {
                        prof = Profile.GetProfile(lot.ProfileId);
                        if (prof != null)
                        {
                            shipid = AsmLot.GetNascaWaferShipId(prof.BlendCd);
                        }
                    }
                    appendLine(dlg.FileName, lot, o, p, prof, shipid);
                }
            }

            if (!hasline)
            {
                MessageBox.Show("作業実績が存在しません");
            }
            else
            {
                MessageBox.Show("出力完了");
            }

        }
               
        private void appendLine(string path, AsmLot lot, Order o, Process p, Profile prof, string shipid)
        {
            MachineInfo mac = MachineInfo.GetMachine(o.MacNo);

            if (lot == null) return;

            bool exists = File.Exists(path);

            using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
            {
                if (!exists)
                {
                    string headerTxt = "作業,型番,ロット,設備,開始時間,完了時間,開始Mg,完了Mg,ストッカー1,ストッカー2,開始作業者,完了作業者,検査作業者";
                    if(chkAimShipData.Checked)
                    {
                        headerTxt += ",狙いランク,供給ID";
                    }
                    if (chkResinGrp.Checked)
                    {
                        headerTxt += ",樹脂Grコード";
                    }
                    if (chkDBThrowDt.Checked)
                    {
                        headerTxt += ",DB投入日";
                    }
                    if (chkProfile.Checked)
                    {
                        headerTxt += ",プロファイル";
                    }
                    sw.WriteLine(headerTxt);
                }

                string lineTxt
                    = p.InlineProNM + ","
                    + lot.TypeCd + ","
                    + o.LotNo + ","
                    + ((mac == null) ? o.MacNo.ToString() : mac.LongName) + ","
                    + o.WorkStartDt + ","
                    + o.WorkEndDt + ","
                    + o.InMagazineNo + ","
                    + o.OutMagazineNo + ","
                    + o.ScNo1 + ","
                    + o.ScNo2 + ","
                    + o.TranStartEmpCd + ","
                    + o.TranCompEmpCd + ","
                    + o.InspectEmpCd;

                if (chkAimShipData.Checked)
                {
                    string aimrank = prof != null ? prof.AimRank : "";
                    lineTxt += ",\"" + aimrank + "\"," + shipid;
                }
                if (chkResinGrp.Checked)
                {
                    lineTxt += ",\"" + string.Join(",",lot.ResinGpCd) + "\"";
                }
                if (chkDBThrowDt.Checked)
                {
                    lineTxt += ",\"" + lot.DBThrowDT + "\"";
                }
                if (chkProfile.Checked)
                {
                    string profilenm = prof != null ? "\"" +prof.ProfileNm + "\"" : "";
                    lineTxt += "," + profilenm;
                }

                sw.WriteLine(lineTxt);
            }
        }


        private void FrmWorkViewReport_Load(object sender, EventArgs e)
        {
            Process[] proclist = Process.GetWorkflowProcess();
            this.lstProcess.Items.AddRange(proclist);

            this.dtpWorkEndFrom.Value = DateTime.Today;
            this.dtpWorkEndTo.Value = DateTime.Today.AddDays(1).AddSeconds(-1);
        }
    }
}
