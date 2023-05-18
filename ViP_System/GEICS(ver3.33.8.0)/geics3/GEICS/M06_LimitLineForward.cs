using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using SLCommonLib.Commons;
namespace GEICS
{
    public partial class M06_LimitLineForward : Form
    {
        public List<string> TargetTypeList = new List<string>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="targetTypeList"></param>
        public M06_LimitLineForward(List<string> targetTypeList)
        {
            InitializeComponent();

            TargetTypeList = targetTypeList;
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M06_LimitLineForward_Load(object sender, EventArgs e)
        {
            try
            {
                //転送対象
                List<MultiSelect> multiTypeList = new List<MultiSelect>();
                foreach (string targetType in TargetTypeList)
                {
                    MultiSelect multi = new MultiSelect(true, targetType);
                    multiTypeList.Add(multi);
                }
                bsTargetType.DataSource = multiTypeList.ToSortableBindingList();

                //転送先ライン
                List<ServInfo> servList = ConnectQCIL.GetServData();
                bsLine.DataSource = servList.Where(s => s.MainServerFG).OrderByDescending(s => s.ServerNM).ToSortableBindingList();
            }
            catch (ApplicationException err)
            {
                MessageBox.Show(err.Message, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 製品型番選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdTargetType_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }

            DataGridViewCheckBoxCell selectCell = (DataGridViewCheckBoxCell)grdTargetType.Rows[e.RowIndex].Cells["SelectFG"];
            if (Convert.ToBoolean(selectCell.Value))
            {
                selectCell.Value = false;
            }
            else
            {
                selectCell.Value = true;
            }
        }

        /// <summary>
        /// ライン選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdLine_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }

            DataGridViewCheckBoxCell selectCell = (DataGridViewCheckBoxCell)grdLine.Rows[e.RowIndex].Cells["LineSelectFG"];
            if (Convert.ToBoolean(selectCell.Value))
            {
                selectCell.Value = false;
            }
            else
            {
                selectCell.Value = true;
            }
        }

        /// <summary>
        /// ライン全選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAllSelect_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in grdLine.Rows)
            {
                dr.Cells["LineSelectFG"].Value = true;
            }
        }

        /// <summary>
        /// ライン全解除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAllCancel_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in grdLine.Rows)
            {
                dr.Cells["LineSelectFG"].Value = false;
            }
        }

        /// <summary>
        /// 転送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolForward_Click(object sender, EventArgs e)
        {
            grdTargetType.EndEdit();
            grdLine.EndEdit();

            try
            {
                if (txtReason.Text == string.Empty) 
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_91, "ALL"));                   
                }
            
                //対象ライン
                List<ServInfo> servList = ((BindingList<ServInfo>)bsLine.List)
                    .Where(s => s.SelectFG).ToList();
                if (servList.Count == 0) 
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_94);
                }

                //対象製品型番
                List<string> typeList = ((BindingList<MultiSelect>)bsTargetType.List)
                    .Where(t => t.SelectFG).Select(t => t.ItemNM).ToList();
                if (typeList.Count == 0) 
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_95);
                }

                foreach (ServInfo serv in servList) 
                {
                    foreach (string type in typeList)
                    {
                        //対象製品型番の閾値を転送
                        ForwardLimit(serv, type, txtReason.Text);
                    }
                }

                MessageBox.Show(Constant.MessageInfo.Message_63);
            }
            catch (ApplicationException err)
            {
                MessageBox.Show(err.Message, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 閾値を転送
        /// </summary>
        /// <param name="servCD">転送先サーバ</param>
        /// <param name="typeCD">製品型番</param>
        /// <param name="reasonVAL">変更理由</param>
        private void ForwardLimit(ServInfo serv, string typeCD, string reasonVAL) 
        {
            List<PlmInfo> plmList = ConnectQCIL.GetPLMData(typeCD, true);
			plmList = GetQCnumVal(plmList);
			
            int diceCT = ConnectQCIL.GetDiceCount(typeCD);

            string forwardConnStr 
                = ConnectQCIL.GetConnectionString(ConnectQCIL.ServerKB.Main, serv.ServerCD, serv.DatabaseNM);
           
            using (ConnectQCIL conn = new ConnectQCIL(true, forwardConnStr))
            {
                try
                {
                    foreach (PlmInfo plmInfo in plmList)
                    {
                        plmInfo.ReasonVAL = reasonVAL;

                        conn.InsertUpdatePLM(plmInfo);
                        conn.InsertPLMHist(plmInfo);

                        if (plmInfo.QcLinePNT != null)
                        {
                            conn.InsertUpdateQCST(plmInfo);
                        }
                    }

                    conn.InsertUpdateDIECT(typeCD, Convert.ToInt32(diceCT));
                    
                    conn.Connection.Commit();
                }
                catch (SqlException err)
                {
                    conn.Connection.Rollback();
                    throw new ApplicationException(err.Message, err);
                }
            }
        }

		private List<PlmInfo> GetQCnumVal(List<PlmInfo> plmList)
		{
			foreach (PlmInfo plmInfo in plmList)
			{
				//打点数を取得
				int inspectionNO = ConnectQCIL.GetInspectionNO(plmInfo.QcParamNO);
				if (inspectionNO == int.MinValue)
				{
					continue;
				}
				plmInfo.InspectionNO = inspectionNO;

				int qcnumVAL = ConnectQCIL.GetQCnumVAL(plmInfo.MaterialCD, plmInfo.InspectionNO, 9);
				if (qcnumVAL == int.MinValue)
				{
					continue;
				}
				plmInfo.QcLinePNT = qcnumVAL;
				plmInfo.QCnumNO = 9;
			}

			return plmList;
		}
    }
}
