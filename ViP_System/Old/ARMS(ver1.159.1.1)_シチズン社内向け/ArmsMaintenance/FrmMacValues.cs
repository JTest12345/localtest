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
    public enum FrmMacValueDataType
    { 
        Material,
        Resin,
        Condition,
    }

    public partial class FrmMacValues : Form
    {
        private FrmMacValueDataType dataType;
        private MachineInfo machine;
        private Button[] macButtons;

        /// <summary>
        /// 装置選択画面に渡す装置リスト
        /// </summary>
        private List<MachineInfoGroup> macInfoGroupList { get; set; }

        private const int MAX_MACBUTTON = 11;

        #region コンストラクタ


        public FrmMacValues(FrmMacValueDataType dataType)
        {
            this.dataType = dataType;
            InitializeComponent();

            switch (this.dataType)
            {
                case FrmMacValueDataType.Material:
                    this.btnDeleteAllWafers.Enabled = true;
                    this.btnAddWafer.Enabled = true;
                    break;

                case FrmMacValueDataType.Resin:
                    this.btnDeleteAllWafers.Enabled = false;
                    this.btnAddWafer.Enabled = false;
                    break;

                case FrmMacValueDataType.Condition:
                    this.btnDeleteAllWafers.Enabled = false;
                    this.btnAddWafer.Enabled = false;
                    break;
            }
        }

        public FrmMacValues(FrmMacValueDataType dataType, int macno, DateTime workstart, DateTime workend)
        {
            this.dataType = dataType;
            InitializeComponent();

            this.dtpWorkStart.Value = workstart;
            this.dtpWorkEnd.Value = workend;
            MachineInfo m = MachineInfo.GetMachine(macno);
            if (m != null)
            {
                this.machine = m;
                this.txtMachine.Text = m.LongName;
                this.txtMacNo.Text = m.MacNo.ToString();
                setGrid();
            }
        }

        private void FrmMacValues_Load(object sender, EventArgs e)
        {
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

            int?[] MacList = Config.Settings.FrmDefInputMacNoList;
            if (MacList == null)
            {
                MacList = new List<int?>().ToArray();
            }
            for (int i = 0; i < MAX_MACBUTTON; i++)
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

            if (Config.Settings.FrmDefInputLineAndMachineClassList != null)
            {
                this.macInfoGroupList = MachineInfoGroup.GetMachineInfoGroupList(Config.Settings.FrmDefInputLineAndMachineClassList);
                // 装置ボタンを非表示にする
                for (int i = 0; i < MAX_MACBUTTON; i++)
                {
                    macButtons[i].Visible = false;
                }
            }
            else
            {
                // 装置選択ボタンを非表示にする
                this.btnMachineSelect.Visible = false;
            }
        }

        #endregion

        /// <summary>
        /// グリッド更新処理
        /// </summary>
        private void setGrid()
        {
            if (this.machine == null) return;

            switch (this.dataType)
            {
                case FrmMacValueDataType.Material:
                    this.grdValues.DataSource = machine.GetMaterials(dtpWorkStart.Value, dtpWorkEnd.Value);
                    break;

                case FrmMacValueDataType.Resin:
                    this.grdValues.DataSource = machine.GetResins(dtpWorkStart.Value, dtpWorkEnd.Value);
                    break;

                case FrmMacValueDataType.Condition:
                    this.grdValues.DataSource = machine.GetWorkConditions(dtpWorkStart.Value, dtpWorkEnd.Value);
                    break;
            }

            foreach (DataGridViewColumn col in grdValues.Columns)
            {
                switch (col.Name)
                {
                    case "MaterialCd":
                        col.HeaderText = "原材料CD";
                        break;

                    case "MaterialNm":
                        col.HeaderText = "原材料名";
                        break;

                    case "LotNo":
                        col.HeaderText = "ロット";
                        break;

                    case "StockerNo":
                        col.HeaderText = "ストッカー";
                        break;

                    case "StartDt":
                    case "InputDt":
                        col.HeaderText = "投入日";
                        break;

                    case "EndDt":
                    case "RemoveDt":
                        col.HeaderText = "取外し日";
                        break;

                    case "LimitDt":
                        col.HeaderText = "期限";
                        break;

                    case "RingId":
                        col.HeaderText = "リングID";
                        break;

                    case "MixResultId":
                        col.HeaderText = "調合ID";
                        break;

                    case "ResinGroupCd":
                        col.HeaderText = "樹脂Gr";
                        break;

                    case "CondCd":
                        col.HeaderText = "特性CD";
                        break;

                    case "CondVal":
                        col.HeaderText = "特性値";
                        break;

                    case "CondName":
                        col.HeaderText = "特性名称";
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }

            if (this.grdValues.SelectedRows.Count == 0)
            {
                this.btnEdit.Enabled = false;
                if (this.dataType == FrmMacValueDataType.Material) this.btnDeleteMat.Enabled = false;
            }
            else
            {
                this.btnEdit.Enabled = true;
                if (this.dataType == FrmMacValueDataType.Material) this.btnDeleteMat.Enabled = true;
            }
        }

        /// <summary>
        /// 新規追加ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.machine == null) return;

                DialogResult res = DialogResult.Cancel;
                switch (this.dataType)
                {
                    case FrmMacValueDataType.Material:
                        res = new FrmMacMat(this.machine).ShowDialog();
                        break;

                    case FrmMacValueDataType.Resin:
                        res = new FrmMacResin(this.machine).ShowDialog();
                        break;

                    case FrmMacValueDataType.Condition:
                        res = new FrmMacCnd(this.machine).ShowDialog();
                        break;
                }

                if (res == DialogResult.OK)
                {
                    setGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        /// <summary>
        /// 変更ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.machine == null) return;

                DialogResult res = DialogResult.Cancel;
                switch (this.dataType)
                {
                    case FrmMacValueDataType.Material:
                        Material mat = (Material)this.grdValues.SelectedRows[0].DataBoundItem;
                        res = new FrmMacMat(this.machine, mat).ShowDialog();
                        break;

                    case FrmMacValueDataType.Resin:
                        Resin resin = (Resin)this.grdValues.SelectedRows[0].DataBoundItem;
                        res = new FrmMacResin(this.machine, resin).ShowDialog();
                        break;

                    case FrmMacValueDataType.Condition:
                        WorkCondition cnd = (WorkCondition)this.grdValues.SelectedRows[0].DataBoundItem;
                        res = new FrmMacCnd(this.machine, cnd).ShowDialog();
                        break;
                }

                if (res == DialogResult.OK)
                {
                    setGrid();
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
                    this.machine = m;
                    this.txtMachine.Text = m.LongName;
                    this.txtMacNo.Text = m.MacNo.ToString();
                    this.grdValues.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                setGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        /// <summary>
        /// グリッド選択行変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdMaterials_SelectionChanged(object sender, EventArgs e)
        {
            if (this.grdValues.SelectedRows.Count == 0)
            {
                this.btnEdit.Enabled = false;
                if (this.dataType == FrmMacValueDataType.Material) this.btnDeleteMat.Enabled = false;
            }
            else
            {
                this.btnEdit.Enabled = true;
                if (this.dataType == FrmMacValueDataType.Material) this.btnDeleteMat.Enabled = true;
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
                    this.grdValues.DataSource = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        #region btnDeleteAllWafer
        
        /// <summary>
        /// ウェハー全解除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteAllWafers_Click(object sender, EventArgs e)
        {
            if (this.machine == null) return;

            try
            {
                DialogResult res = MessageBox.Show(this, this.machine.MachineName + "の\n全カセットの割り付けを解除します", "確認",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);

                if (res != DialogResult.OK) return;

                Material[] matlist = machine.GetMaterials(DateTime.Now, DateTime.Now);
                foreach (Material mat in matlist)
                {
                    if (mat.StockerNo == 0)
                    {
                        continue;
                    }

                    mat.RemoveDt = DateTime.Now.AddSeconds(-1);
                    machine.DeleteInsertMacMat(mat);
                }

                MessageBox.Show("解除完了");
                setGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion


        /// <summary>
        /// 選択原材料削除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteMat_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.machine == null) return;


                DialogResult res = MessageBox.Show(this, this.machine.MachineName + "の\n選択原材料の割り付けを解除します", "確認",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);

                if (res != DialogResult.OK) return;

                switch(this.dataType)
                {
                    case FrmMacValueDataType.Material:
                        Material mat = (Material)this.grdValues.SelectedRows[0].DataBoundItem;
                        if (mat == null)
                        {
                            MessageBox.Show("原材料情報が不正です");
                            return;
                        }

                        mat.RemoveDt = DateTime.Now.AddSeconds(-1);
                        machine.DeleteInsertMacMat(mat);

                        break;

                    case FrmMacValueDataType.Resin:
                        Resin resin = (Resin)this.grdValues.SelectedRows[0].DataBoundItem;
                        if (resin == null)
                        {
                            MessageBox.Show("原材料情報が不正です");
                            return;
                        }

                        resin.RemoveDt = DateTime.Now.AddSeconds(-1);
                        machine.DeleteInsertMacResin(resin);

                        break;
                }               

                MessageBox.Show("解除完了");
                setGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnAddWafer_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.machine == null) return;

                DialogResult res = DialogResult.Cancel;
                res = new FrmMacWafer(this.machine).ShowDialog();

                if (res == DialogResult.OK)
                {
                    setGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnMachineSelect_Click(object sender, EventArgs e)
        {
            int macno;
            if (int.TryParse(this.txtMacNo.Text, out macno) == false)
            {
                macno = 0;
            }

            FrmSelectMachineButtons frm = new FrmSelectMachineButtons(macno, this.macInfoGroupList);
            frm.ShowDialog();

            MachineInfo m = frm.SelectedMacInfo;
            
            if (m == null) return;

            try
            {
                this.machine = m;
                this.txtMachine.Text = m.LongName;
                this.txtMacNo.Text = m.MacNo.ToString();
                this.grdValues.DataSource = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
