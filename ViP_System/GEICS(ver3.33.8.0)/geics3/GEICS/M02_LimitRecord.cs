using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GEICS
{
    public partial class M02_LimitRecord : Form
    {
        private string _MaterialCD = string.Empty;
        private string _ModelNM = string.Empty;
        private int _QcParamNO = int.MinValue;
		private string _EquipmentNO = string.Empty;
        private string _ResinGroupCD = string.Empty;

        public M02_LimitRecord(string materialCD, string modelNM, int qcParamNO, string equipNO, string resinGroupCD)
        {
            InitializeComponent();

            this._MaterialCD = materialCD;
            this._ModelNM = modelNM;
            this._QcParamNO = qcParamNO;
			this._EquipmentNO = equipNO;
            this._ResinGroupCD = resinGroupCD;
        }

        private void frmMasterLimitRecord_Load(object sender, EventArgs e)
        {
            try
            {
                List<PlmInfo> plmHistList = ConnectQCIL.GetPLMHistData(this._MaterialCD, this._ModelNM, this._QcParamNO, this._EquipmentNO, this._ResinGroupCD);
                foreach (PlmInfo plmHistInfo in plmHistList)
                {
                    //不良名を取得
					//if (plmHistInfo.QcParamNO >= 10000 && !((plmHistInfo.QcParamNO >= 200000) && (plmHistInfo.QcParamNO < 300000)))
					if(plmHistInfo.UnManageTrendFG == true)
					{
                        plmHistInfo.ParameterNM += "(" + ConnectQCIL.GetDefectName(plmHistInfo.ParameterNM) + ")";
                    }

                    ////打点数を取得
                    //int inspectionNO = ConnectQCIL.GetInspectionNO(plmHistInfo.QcParamNO);
                    //if (inspectionNO == int.MinValue)
                    //{
                    //    continue;
                    //}
                    //plmHistInfo.InspectionNO = inspectionNO;

                    //int qcnumVAL = ConnectQCIL.GetQCcnumVAL(plmHistInfo.MaterialCD, plmHistInfo.InspectionNO, 9);
                    //if (qcnumVAL == int.MinValue)
                    //{
                    //    continue;
                    //}
                    //plmHistInfo.QcLinePNT = qcnumVAL;
                }

                bsLimitRecord.DataSource = plmHistList.ToSortableBindingList();
            }
            catch (Exception err) 
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
