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
    public partial class FrmSelectDBProfile : Form
    {
        DataTable dbt;

        public FrmSelectDBProfile()
        {
            InitializeComponent();
            dbt = new DataTable();
            dbt.Columns.Add("設備No");
            dbt.Columns.Add("設備名称");
            dbt.Columns.Add("プロファイルID");
            dbt.Columns.Add("プロファイル名");
        }


        private void FrmSelectDBProfile_Load(object sender, EventArgs e)
        {
            //loadDBProfiles();
        }

        private void searchDBProfiles(string plantnm)
        {
            this.dbt.Rows.Clear();
            MachineInfo[] maclist = MachineInfo.SearchMachine(null, null, true, false, true, null, null, null);

            // <!--
            if (string.IsNullOrWhiteSpace(plantnm) == false)
            {
                maclist = maclist.Where(m => m.LongName.ToUpper().Contains(plantnm.ToUpper())).ToArray();
            }
            //  -->

            foreach (MachineInfo mac in maclist.OrderBy(m => m.MacNo))
            {
                Profile p = Profile.GetCurrentDBProfile(mac.MacNo);
                if (p == null) continue;

                dbt.Rows.Add(new object[] { mac.MacNo, mac.LongName, p.ProfileId, p.ProfileNm });
            }

            this.grdDBProfile.DataSource = dbt;
        }

        private void grdDBProfile_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                DataRowView row = (DataRowView)grdDBProfile.Rows[e.RowIndex].DataBoundItem;
                if (row == null) return;

                FrmSelectProfile frm = new FrmSelectProfile(true);
                DialogResult res = frm.ShowDialog(this);
                if (res != DialogResult.OK) return;
                if (frm.Selected == null) return;

                int macno = Convert.ToInt32(row["設備No"]);
                Profile.SetCurrentDBProfile(macno, frm.Selected.ProfileId);

                MessageBox.Show("更新完了");
                //loadDBProfiles();

                string plantnm = this.txtPlantName.Text;
                searchDBProfiles(plantnm);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // <!--
            // 検索条件のチェック
            string plantnm = this.txtPlantName.Text;
            if (string.IsNullOrWhiteSpace(plantnm))
            {
                DialogResult dr = MessageBox.Show("検索条件が入力されていない為、検索に時間が掛かる可能性があります。よろしいですか？",
                    "Infomation", MessageBoxButtons.OKCancel);
                if (dr != DialogResult.OK)
                {
                    return;
                }
            }
            // -->

            searchDBProfiles(plantnm);
        }
    }
}
