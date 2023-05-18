using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace EICS
{
    public partial class F03_MachineList : Form
    {
        private int lineCD = 0;

        public F03_MachineList(int _lineCD)
        {
            InitializeComponent();
            lineCD = _lineCD;
        }

        private void G003_MachineList_Load(object sender, EventArgs e)
        {
            txtLineCD.Text = Convert.ToString(lineCD);

            dsG003.TmLSET.Clear();

            List<LSETInfo> lsetList = ConnectDB.GetLSETData(lineCD, "");
            foreach (LSETInfo lsetInfo in lsetList) 
            {
                DataRow dr = dsG003.TmLSET.NewRow();
                
                dr["Inline_CD"] = lineCD;
                dr["Disp_NO"] = lsetInfo.DispCD;
                dr["Equipment_NO"] = lsetInfo.EquipmentNO;
                dr["Equipment_CD"] = lsetInfo.EquipmentCD;
                dr["Process_CD"] = lsetInfo.ProcessCD;
                dr["Seq_NO"] = lsetInfo.SeqCD;
                dr["IPAddress_NO"] = lsetInfo.IPAddressNO;
                dr["Port_NO"] = lsetInfo.PortNO;
                dr["InputFolder_NM"] = lsetInfo.InputFolderNM;
                dr["Del_FG"] = lsetInfo.DelFG;
                dr["UpdUser_CD"] = lsetInfo.UpdUserCD;
                dr["LastUpd_DT"] = lsetInfo.LastUpdDT;
                dr["Assets_NM"] = lsetInfo.AssetsNM;
                dr["MachinSeq_NO"] = lsetInfo.MachineSeqNO;
                dr["Model_NM"] = lsetInfo.ModelNM;
                dsG003.TmLSET.Rows.Add(dr);
            }
        }

        private void toolReturn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
