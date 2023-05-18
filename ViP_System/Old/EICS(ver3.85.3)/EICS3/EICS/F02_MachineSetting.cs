using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EICS.Database;

namespace EICS
{
    public partial class F02_MachineSetting : Form
    {
		//private string selectModelNM = "";
		//private string selectPlantCD = "";
		//private string selectTypeCD = "";
		//private string selectChipNM = "";
		//private string selectAssetsNM = "";
		//private bool selectBMCountFG;
		//private int selectLineCD = 0;
		//private bool unSelectableTypeFG;

		EquipmentInfo selectedEquipInfo = new EquipmentInfo();

        private Constant.MachineStatus machineStatus = Constant.MachineStatus.Wait;
            
        private bool startFG = false;
        public bool StartFG 
        {
            get { return this.startFG; }
        }

        private bool stopFG = false;
        public bool StopFG 
        {
            get { return this.stopFG; }
        }

        public string TypeCD { get { return this.cmbType.Text; } }

        public string ChipNM { get { return this.cmbChip.Text; } }

		public bool BMCountFG { get { return this.cbBMCount.Checked; } }
		public bool IsOutputNasFile { get { return this.cbNasOutput.Checked; } }

		public List<string> AllTypeCDList { get; set; }
		public List<string> TypeList { get; set; }


		private bool changeFG = false;
        public bool ChangeFG 
        {
            get { return this.changeFG; }
        }

        /// <summary>
        /// 初期の装置型式
        /// </summary>
        private string orgModelNm { get; set; }

        public F02_MachineSetting(int lineCD, EquipmentInfo equipmentInfo, Constant.MachineStatus status)
        {
            InitializeComponent();

			selectedEquipInfo = equipmentInfo;
			selectedEquipInfo.LineNO = lineCD;

            this.AllTypeCDList = new List<string>();

            this.orgModelNm = equipmentInfo.ModelNM;

            machineStatus = status;
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void G002_MachineSetting_Load(object sender, EventArgs e)
        {
			txtLineCD.Text = selectedEquipInfo.LineNO.ToString();
			txtPlantCD.Text = selectedEquipInfo.EquipmentNO;

            cmbType.Text = selectedEquipInfo.DisplayTypeCD;
            cmbChip.Text = selectedEquipInfo.ChipNM;
			cbBMCount.Checked = selectedEquipInfo.BMCountFG;
			cbNasOutput.Checked = selectedEquipInfo.IsOutputNasFile;

            this.updateModelNmList();

            if (selectedEquipInfo.UnSelectableTypeFG)
			{
				cmbType.Enabled = false;
			}
			else
			{
                this.updateTypeCdList();
            }

			if (selectedEquipInfo.UnSelectableWorkFG)
            {
                cmbChip.Enabled = false;
                cbBMCount.Enabled = false;
            }
            else
            {
                this.updateChipNmList();
            }

            if (machineStatus == Constant.MachineStatus.Runtime)
            {
                btnStart.Enabled = false;
                cmbChip.Enabled = false;
                cmbType.Enabled = false;
            }
            else 
            {
                btnStop.Enabled = false;
            }
        }

        /// <summary>
        /// 開始ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            startFG = true;
            this.Close();
        }

        /// <summary>
        /// 停止ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            stopFG = true;
            this.Close();
        }

        /// <summary>
        /// 型番変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbType.Text == "")
            {
                MessageBox.Show(cmbType.Text + Constant.MessageInfo.Message_9, "Exclamation", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                return;
            }

            if (DialogResult.Cancel == MessageBox.Show(string.Format(Constant.MessageInfo.Message_7, cmbType.Text), "Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
            {
                return;
            }
            
            changeFG = true;
            MessageBox.Show(Constant.MessageInfo.Message_8, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// チップ変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbChip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbChip.Text == "")
            {
                MessageBox.Show(cmbChip.Text + Constant.MessageInfo.Message_9, "Exclamation", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                return;
            }

            if (DialogResult.Cancel == MessageBox.Show(string.Format(Constant.MessageInfo.Message_7, cmbChip.Text), "Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
            {
                return;
            }

			this.selectedEquipInfo.ChipNM = cmbChip.Text.Trim();

            changeFG = true;
            MessageBox.Show(Constant.MessageInfo.Message_8, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void cbBMCount_CheckStateChanged(object sender, EventArgs e)
		{
			if (cbBMCount.Checked)
			{
				//設定ファイルの対象設備のBMCountFGを変更する
				if (!SettingInfo.SetMachineSetting(txtPlantCD.Text, "", "", "true", ""))
				{
					return;
				}
			}
			else
			{
				if (!SettingInfo.SetMachineSetting(txtPlantCD.Text, "", "", "false", ""))
				{
					return;
				}
			}
		}

		private void cbNasOutput_CheckedChanged(object sender, EventArgs e)
		{
			if (cbNasOutput.Checked)
			{
				//設定ファイルの対象設備のBMCountFGを変更する
				if (!SettingInfo.SetMachineSetting(txtPlantCD.Text, "", "", "", "true"))
				{
					return;
				}
			}
			else
			{
				if (!SettingInfo.SetMachineSetting(txtPlantCD.Text, "", "", "", "false"))
				{
					return;
				}
			}
		}

		private void txtTypeSearchCond_Leave(object sender, EventArgs e)
		{
			cmbType.Items.Clear();

			List<string> typeCDList = new List<string>();

			if (string.IsNullOrEmpty(txtTypeSearchCond.Text))
			{
				typeCDList = AllTypeCDList;
			}
			else
			{
				typeCDList = AllTypeCDList.FindAll(t => t.Contains(txtTypeSearchCond.Text));
			}
			
			if (typeCDList.Count != 0)
			{
				foreach (string typeCD in typeCDList)
				{
					cmbType.Items.Add(typeCD);
				}
			}
		}

		private void F02_MachineSetting_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(selectedEquipInfo.UnSelectableTypeFG == true)
			{
				return;
			}

			DataContext.TmTypeGroup typeGrMaster;

			//コンボボックスで選択されている文字列で型番で昇順に並べたまとめ型番マスタの先頭レコード取得
			using (var eicsDB = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, selectedEquipInfo.LineNO)))
			{
				typeGrMaster = eicsDB.TmTypeGroup.Where(t => t.TypeGroup_CD == cmbType.Text && t.Model_NM == this.selectedEquipInfo.ModelNM && t.Chip_NM == this.selectedEquipInfo.ChipNM && t.Del_FG == false)
					.OrderBy(t => t.LastUpd_DT).FirstOrDefault();
			}

			string newType, newTypeGr;

			if (typeGrMaster != null)
			{
				newType = typeGrMaster.Type_CD;
				newTypeGr = typeGrMaster.TypeGroup_CD;
			}
			else if (TypeList.Contains(cmbType.Text) == true)
			{
				newType = cmbType.Text;
				newTypeGr = null;
			}
			else
			{
				MessageBox.Show(
					$"まとめ型番マスタに設定されていない組み合わせが選択されています。\r\n"
					+ $"選択型番『{cmbType.Text}』 装置型式『{this.selectedEquipInfo.ModelNM}』 "
					+ $"チップ/作業『{this.selectedEquipInfo.ChipNM}』");
				e.Cancel = true;
				//startFG = false;

				//btnStart.Enabled = false;

				EditFormWhenStopping();

				return;
			}

            // 装置型式が変わっている場合、「チップ/作業」の選択内容が正しいかチェック
            if (selectedEquipInfo.ModelNM != orgModelNm && selectedEquipInfo.UnSelectableWorkFG == false)
            {
                List<string> chipNMList = ConnectDB.GetPRMChipData(selectedEquipInfo.ModelNM, selectedEquipInfo.LineNO);
                if (chipNMList.Contains(cmbChip.Text) == false)
                {
                    MessageBox.Show(
                        $"閾値マスタに設定されていない組み合わせが選択されています。\r\n"
                        + $"装置型式『{this.selectedEquipInfo.ModelNM}』 "
                        + $"チップ/作業『{this.selectedEquipInfo.ChipNM}』");
                    e.Cancel = true;
                    if (startFG) startFG = false;
                    EditFormWhenStopping();

                    return;
                }
            }

            this.changeEquiModelNm();

			Lset.UpdateWorkingType(selectedEquipInfo.LineNO, txtPlantCD.Text, newType, newTypeGr);

			Lset.UpdateChipName(selectedEquipInfo.LineNO, txtPlantCD.Text, cmbChip.Text.Trim());
			
		}

		private void EditFormWhenStopping()
		{
			if (stopFG == false)
			{
				return;
			}

			if (selectedEquipInfo.UnSelectableTypeFG)
			{
				cmbType.Enabled = false;
			}
			else
			{
				cmbType.Enabled = true;
			}

            if (selectedEquipInfo.UnSelectableWorkFG)
            {
                cmbChip.Enabled = false;
                cbBMCount.Enabled = false;
			}
			else
			{
                cmbChip.Enabled = true;
                cbBMCount.Enabled = true;
            }
		}

        /// <summary>
        /// 「装置型式」選択欄の項目更新
        /// </summary>
        private void updateModelNmList()
        {
            // delfg問わず、設備CDでレコードを抽出
            List<Equi> equiList = Equi.GetEquipmentInfo(selectedEquipInfo.LineNO, selectedEquipInfo.EquipmentNO);
            if (equiList.Count() <= 1)
            {
                // 設備CDのレコードが単体
                lblModel.Visible = false;
                cmbModel.Visible = false;
            }
            else
            {
                // 設備CDのレコードが複数
                cmbModel.Text = selectedEquipInfo.ModelNM;
                cmbModel.Items.Clear();
                equiList.ForEach(l => cmbModel.Items.Add(l.ModelNM));
            }
        }

        /// <summary>
        /// 「型番」選択欄の項目更新
        /// </summary>
        private void updateTypeCdList()
        {
            List<string> displayTypeList = new List<string>();

            List<string> groupTypeList = new List<string>();

            this.AllTypeCDList.Clear();
            using (var eicsDB = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, selectedEquipInfo.LineNO)))
            {
                List<DataContext.TmTypeGroup> typeGroupList = eicsDB.TmTypeGroup
                    .Where(t => t.Model_NM == this.selectedEquipInfo.ModelNM && t.Del_FG == false).ToList();
                groupTypeList = typeGroupList.Select(t => t.Type_CD).ToList();

                this.AllTypeCDList.AddRange(typeGroupList
                    .Select(t => t.TypeGroup_CD).Distinct());
            }

            this.TypeList = ConnectDB.GetPLMTypeData(this.selectedEquipInfo.ModelNM, selectedEquipInfo.LineNO);
            this.TypeList = TypeList.Where(t => groupTypeList.Contains(t) == false).ToList();

            this.AllTypeCDList.AddRange(TypeList);

            displayTypeList = AllTypeCDList;

            List<string> typeCDList = this.AllTypeCDList;
            if (string.IsNullOrEmpty(this.txtTypeSearchCond.Text) == false)
            {
                typeCDList = typeCDList.FindAll(t => t.Contains(this.txtTypeSearchCond.Text));
            }

            this.cmbType.Items.Clear();
            if (typeCDList.Any() == true)
            {
                typeCDList.ForEach(l => this.cmbType.Items.Add(l));
            }
            
        }

        /// <summary>
        /// 「チップ/作業」選択欄の項目更新
        /// </summary>
        private void updateChipNmList()
        {
            List<string> chipNMList = ConnectDB.GetPRMChipData(this.selectedEquipInfo.ModelNM, this.selectedEquipInfo.LineNO);

            if (chipNMList.Count != 0)
            {
                this.cmbChip.Items.Clear();
                foreach (string chipNM in chipNMList)
                {
                    this.cmbChip.Items.Add(chipNM);
                }
            }
        }

        /// <summary>
        /// TmEquiの内容を更新 選択したModelNmのレコードのみdelfg = 0 にして、それ他レコードをdelfg = 1にする
        /// </summary>
        private void changeEquiModelNm()
        {
            if (this.cmbModel.Visible == false) return;

            List<Equi> equiList = Equi.GetEquipmentInfo(this.selectedEquipInfo.LineNO, this.selectedEquipInfo.EquipmentNO);

            // 装置型式の変更がない場合は、何もしない
            if (this.selectedEquipInfo.ModelNM == this.orgModelNm) return;

            // 新しい装置型式のレコードを delfg = 0にして、その他レコードの内delfg = 0のレコードをdelfg - 1にする
            Equi.UpdateDelFgOn(this.selectedEquipInfo.LineNO, this.selectedEquipInfo.EquipmentNO, null);
            Equi.UpdateDelFgOff(this.selectedEquipInfo.LineNO, this.selectedEquipInfo.EquipmentNO, this.selectedEquipInfo.ModelNM);
        }

        private void cmbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.selectedEquipInfo.ModelNM == this.cmbModel.Text) return;
            
            this.selectedEquipInfo.ModelNM = this.cmbModel.Text.Trim();
            
            // 装置型式変更に伴い、「型番」「チップ/作業」の選択欄のリストを更新する
            if (this.selectedEquipInfo.UnSelectableTypeFG == false)
            {
                this.updateTypeCdList();
            }
            if (this.selectedEquipInfo.UnSelectableWorkFG == false)
            {
                this.updateChipNmList();
            }
        }
    }
}
