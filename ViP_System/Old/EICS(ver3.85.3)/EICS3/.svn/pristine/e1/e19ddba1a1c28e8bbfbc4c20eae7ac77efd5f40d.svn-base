using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EICS
{
    public partial class F07_TrayLotEdit : Form
    {
        int linecd;
        public F07_TrayLotEdit(int _linecd)
        {
            linecd = _linecd;
            InitializeComponent();
        }

        //検索ボタン
        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<DataContext.TnSortingTrayLot> tnsortingtraylotlist = new List<DataContext.TnSortingTrayLot>();
            using (var eicsDB = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, linecd)))
            {
                if (chkbEffectFG.Checked == true)
                {
                    if (string.IsNullOrWhiteSpace(txtbTrayID.Text) == false && string.IsNullOrWhiteSpace(txtbNascaLot.Text) == false)
                    {
                        tnsortingtraylotlist = eicsDB.TnSortingTrayLot.Where(t => t.Tray_ID == txtbTrayID.Text.Trim() && t.NascaLot_NO == txtbNascaLot.Text.Trim()).ToList();
                    }
                    else if (string.IsNullOrWhiteSpace(txtbTrayID.Text) == false && string.IsNullOrWhiteSpace(txtbNascaLot.Text) == true)
                    {
                        tnsortingtraylotlist = eicsDB.TnSortingTrayLot.Where(t => t.Tray_ID == txtbTrayID.Text.Trim()).ToList();
                    }
                    else if (string.IsNullOrWhiteSpace(txtbTrayID.Text) == true && string.IsNullOrWhiteSpace(txtbNascaLot.Text) == false)
                    {
                        tnsortingtraylotlist = eicsDB.TnSortingTrayLot.Where(t => t.NascaLot_NO == txtbNascaLot.Text.Trim()).ToList();
                    }
                    else
                    {
                        MessageBox.Show("どちらか一方は検索項目を入力してください。");
                    }

                }
                else
                {
                    if (string.IsNullOrWhiteSpace(txtbTrayID.Text) == false && string.IsNullOrWhiteSpace(txtbNascaLot.Text) == false)
                    {
                        tnsortingtraylotlist = eicsDB.TnSortingTrayLot.Where(t => t.Tray_ID == txtbTrayID.Text.Trim() && t.NascaLot_NO == txtbNascaLot.Text.Trim() && t.Effect_FG == true).ToList();
                    }
                    else if (string.IsNullOrWhiteSpace(txtbTrayID.Text) == false && string.IsNullOrWhiteSpace(txtbNascaLot.Text) == true)
                    {
                        tnsortingtraylotlist = eicsDB.TnSortingTrayLot.Where(t => t.Tray_ID == txtbTrayID.Text.Trim() && t.Effect_FG == true).ToList();
                    }
                    else if (string.IsNullOrWhiteSpace(txtbTrayID.Text) == true && string.IsNullOrWhiteSpace(txtbNascaLot.Text) == false)
                    {
                        tnsortingtraylotlist = eicsDB.TnSortingTrayLot.Where(t => t.NascaLot_NO == txtbNascaLot.Text.Trim() && t.Effect_FG == true).ToList();
                    }
                    else
                    {
                        MessageBox.Show("どちらか一方は検索項目を入力してください。");
                    }
                }
            }



            BindingSource src = new BindingSource();
            src.DataSource = tnsortingtraylotlist;
            dgvTnSortingTrayLot.DataSource = src;

            if (tnsortingtraylotlist.Count > 0)
            {
                //DataGridViewのヘッダ名
                dgvTnSortingTrayLot.Columns[0].HeaderText = "トレイID";
                dgvTnSortingTrayLot.Columns[1].HeaderText = "ロットNo";
                dgvTnSortingTrayLot.Columns[2].HeaderText = "ポットNo";
                dgvTnSortingTrayLot.Columns[3].HeaderText = "有効";
                dgvTnSortingTrayLot.Columns[4].HeaderText = "登録日";
                dgvTnSortingTrayLot.Columns[5].HeaderText = "更新日";
            }

        }

        //登録ボタン
        private void btnRegister_Click(object sender, EventArgs e)
        {

        }
    }
}
