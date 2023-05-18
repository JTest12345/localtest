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
    public partial class FrmMacMaster : Form
    {
        // <!-- 改善：次装置の情報の表示
        bool canManteFg { get; set; }

        List<int> setMacList { get; set; }

        public FrmMacMaster() : this(true, null)
        {

        }
                
        public FrmMacMaster(bool canmantefg, List<int> setmaclist)
        {
            this.canManteFg = canmantefg;
            this.setMacList = setmaclist;

            InitializeComponent();
        }

        //public FrmMacMaster()
        //{
        //    InitializeComponent();
        //}

        // --> 改善：次装置の情報の表示


        private void FrmMacMaster_Load(object sender, EventArgs e)
        {
            setMacRecord();
        }

        private MachineInfo[] maclist;

        private void setMacRecord()
        {
            // <!-- 改善：次装置の情報の表示
            MachineInfo[] machines = null;
            if (this.setMacList == null || this.setMacList.Any() == false)
            {
                machines = MachineInfo.GetDupulicateMachines();
            }
            else
            {
                machines = MachineInfo.SearchMachine(null, null, false, false, false, null, null, null);
                machines = machines.Where(m => this.setMacList.Contains(m.MacNo)).ToArray();
            }
            this.maclist = machines;
            if (this.canManteFg == false)
            {
                this.Text = "装置リスト表示";
                this.btnUpdate.Visible = false;
            }
            // --> 改善：次装置の情報の表示

            if (machines == null || machines.Length == 0)
            {
                this.grdMachines.DataSource = null;
                return;
            }

            this.grdMachines.DataSource = this.maclist;


            foreach (DataGridViewColumn col in this.grdMachines.Columns)
            {
                switch (col.Name)
                {
                    #region 列設定

                    case "ColAvailable":
                        if (this.canManteFg == false)
                        {
                            col.Visible = false;
                        }
                        break;

                    case "MacNo":
                        col.HeaderText = "装置NO";
                        col.ReadOnly = true;
                        break;


                    case "NascaPlantCd":
                        col.HeaderText = "設備CD";
                        col.ReadOnly = true;
                        break;

                    case "ClassName":
                        col.HeaderText = "分類";
                        col.ReadOnly = true;
                        break;

                    case "MachineName":
                        col.HeaderText = "設備名";
                        col.ReadOnly = true;
                        break;

                    case "LineNo":
                        col.HeaderText = "ラインNo";
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                    #endregion
                }
            }

            // <!-- 画面起動時間高速化対応 (MachineInfoクラスにColAvailable列を追加 + 画面デザインの第1列のDataPropertyNameを(なし)→ColAvailableに変更)
            //foreach (DataGridViewRow row in this.grdMachines.Rows)
            //{
            //    row.Cells["ColAvailable"].Value = !(bool)row.Cells["DelFg"].Value;
            //}
            // 【改修1.133.0】 -->
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            FrmPasswordDialog frm = new FrmPasswordDialog(
                "OK",
              "設備稼働フラグを更新します。\r\n本当に更新する場合は入力欄にOKと入力してください。" );
            DialogResult res = frm.ShowDialog();

            List<string> availableMachines = new List<string>();

            if (res == DialogResult.OK)
            {
                foreach (DataGridViewRow row in this.grdMachines.Rows)
                {
                    bool isAvailable = (bool)row.Cells["ColAvailable"].Value;
                    row.Cells["DelFg"].Value = !isAvailable;

                    if (isAvailable)
                    {
                        string plantcd = (string)row.Cells["NascaPlantCd"].Value;
                        if (!string.IsNullOrEmpty(plantcd) && plantcd != "-" && availableMachines.Contains(plantcd))
                        {
                            MessageBox.Show(this, "設備が重複しています:" + plantcd);
                            return;
                        }
                        else
                        {
                            availableMachines.Add(plantcd);
                        }
                    }
                }

                foreach (MachineInfo mac in this.maclist)
                {
                    MachineInfo.UpdateDelFg(mac);
                }
            }

            MessageBox.Show("更新完了");
        }
    }
}
