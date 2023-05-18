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
    public partial class FrmCutBlend : Form
    {
        CutBlend[] blendlist;
        private MachineInfo machine;

        private Button[] macButtons = new Button[11];

        public FrmCutBlend(int macno)
        {
            InitializeComponent();
            macButtons = new Button[11];
            macButtons[0] = btnMac1;
            macButtons[1] = btnMac2;
            macButtons[2] = btnMac3;
            macButtons[3] = btnMac4;
            macButtons[4] = btnMac5;
            macButtons[5] = btnMac6;
			macButtons[6] = btnMac7;
			macButtons[7] = btnMac8;
			macButtons[8] = btnMac9;
			macButtons[9] = btnMac10;
			macButtons[10] = btnMac11;
        }

        private void updateGrid()
        {
            this.blendlist = CutBlend.GetCurrentBlendItems(machine.MacNo);

            if (this.blendlist != null)
            {
                this.blendlist = this.blendlist.OrderBy(o => o.StartDt).ToArray();
            }

            this.grdCutBlend.DataSource = this.blendlist;
            int i = 0;
            foreach (DataGridViewRow row in this.grdCutBlend.Rows)
            {
                if (i < Config.Settings.CutBlendMagazineCt)
                {
                    row.Cells[0].Value = true;
                }
                i++;
            }
            foreach (DataGridViewColumn col in this.grdCutBlend.Columns)
            {
                switch (col.Name)
                {
                    case "colBlend":
                        break;

                    case "LotNo":
                        col.HeaderText = "ロット番号";
                        col.ReadOnly = true;
                        break;

                    case "StartDt":
                        col.HeaderText = "投入日";
                        col.ReadOnly = true;
                        break;

                    case "MagNo":
                        col.HeaderText = "マガジンNo";
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
        }

        private void FrmCutBlend_Load(object sender, EventArgs e)
        {
            int?[] MacList = Config.Settings.FrmDefInputMacNoList;
            if (MacList == null) 
            {
                MacList = new List<int?>().ToArray();
            }
            for (int i = 0; i <= 10; i++)
            {
                if (MacList.Length <= i)
                {
                    macButtons[i].Tag = null;
                    macButtons[i].Text = "";
                    continue;
                }
                if (MacList[i] == null)
                {
                    macButtons[i].Tag = null;
                    macButtons[i].Text = "";
                    continue;
                }
                MachineInfo m = MachineInfo.GetMachine(MacList[i].Value);
                macButtons[i].Tag = m;
                macButtons[i].Text = m.MachineName;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in this.grdCutBlend.Rows)
                {
                    if (row.Cells[0].Value != null && (bool)row.Cells[0].Value == true)
                    {
                        CutBlend cb = (CutBlend)row.DataBoundItem;

                        FrmPasswordDialog frm = new FrmPasswordDialog(
                            cb.MagNo,
                            cb.MagNo + " の作業実績を削除します。\r\n本当に削除する場合は入力欄に" + cb.MagNo + "と入力してください。");
                        DialogResult res = frm.ShowDialog();

                        if (res == DialogResult.OK)
                        {
                            MessageBox.Show("削除完了");
                            cb.DelFg = true;
                            cb.DeleteInsert();
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            updateGrid();
        }

        private void btnBlend_Click(object sender, EventArgs e)
        {
            try
            {
                List<CutBlend> complist = new List<CutBlend>();
                foreach (DataGridViewRow row in this.grdCutBlend.Rows)
                {
                    if (row.Cells[0].Value != null && (bool)row.Cells[0].Value == true)
                    {
                        CutBlend cb = (CutBlend)row.DataBoundItem;
                        complist.Add(cb);
                    }
                }

                if (complist.Count >  Config.Settings.CutBlendMagazineCt)
                {
                    MessageBox.Show(this, "カットブレンド内のアッセンロットが規定数を超えています。\n履歴を修正してください。",
                        "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (complist.Count == 0)
                {
                    return;
                }

                DialogResult res = MessageBox.Show(
                    this,
                    "ブレンドロットを完成します\nよろしいですか？"
                    , "確認"
                    , MessageBoxButtons.OKCancel
                    , MessageBoxIcon.Asterisk
                    , MessageBoxDefaultButton.Button2);

                if (res != DialogResult.OK) return;

                CarrierInfo carrier = Route.GetReachable(new Location(int.Parse(txtMacNo.Text), Station.Loader));
                MachineInfo mac = MachineInfo.GetMachine(int.Parse(txtMacNo.Text));
                CutBlend.CompleteBlend(DateTime.Now, 
                    complist.ToArray(), 
                    carrier.CarNo.ToString().Substring(carrier.CarNo.ToString().Length -2, 2), "", mac.IsAutoLine);

                MessageBox.Show("登録完了");
            }
            catch (Exception ex)
            {
                MessageBox.Show("登録失敗：" + ex.ToString());
            }
            
            updateGrid();
        }

        private void btnSelectMachine_Click(object sender, EventArgs e)
        {
            try
            {
                FrmSelectMachine frmmac = new FrmSelectMachine();
                DialogResult res = frmmac.ShowDialog();

                if (res == DialogResult.OK)
                {
                    MachineInfo m = frmmac.SelectedMachine;
                    this.machine = m;
                    this.txtMachine.Text = m.LongName;
                    this.txtMacNo.Text = m.MacNo.ToString();
                    updateGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnMac_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Tag != null)
            {
                MachineInfo m = ((Button)sender).Tag as MachineInfo;
                if (m == null) return;

                try
                {
                    this.machine = m;
                    this.txtMachine.Text = m.LongName;
                    this.txtMacNo.Text = m.MacNo.ToString();
                    updateGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}
