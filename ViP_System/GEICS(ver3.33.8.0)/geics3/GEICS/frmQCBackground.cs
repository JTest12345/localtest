using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace GEICS
{
    public partial class frmQCBackground : Form
    {
        Common Com = new Common();

        //key
        static int nInlineCD;
        static int nQcnrNO;
        static int nCnfmNO;
        static string sAssetsNM;
        //static string sEquipmentNO;
        string _sEquiNO = "";
        string _sEquiNOAdd = "";

        static int nInspectionNO;
        static DateTime dtMeasureDT;
        //sub
        static bool fCheckFG;//確認フラグ

		public bool InputFg { get; set; }

        public frmQCBackground(int nw1, int nw2, int nw3, string sw4,int nw5,string sw6,int nw7,DateTime dtw8)
        {
			InputFg = false;

            InitializeComponent();

            //key入力
            nInlineCD = nw1;
            nQcnrNO = nw2;
            nCnfmNO = nw3;
            sAssetsNM = sw4;
            //sEquipmentNO = sw6;
            _sEquiNOAdd = sw6;//「***号機(S*****)」表記
            int npos = _sEquiNOAdd.IndexOf("(");
            _sEquiNO = _sEquiNOAdd.Substring(npos + 1, _sEquiNOAdd.Length - npos - 2);//「S*****」表記


            nInspectionNO =nw7;
            dtMeasureDT = dtw8;

            if (nw5 > 0)
            {
                fCheckFG = true;
            }
            else
            {
                fCheckFG = false;
            }
        }

        private void frmQCBackground_Load(object sender, EventArgs e)
        {
            //定型句選択リストボックス初期化
            SetFixedList();

            if (nCnfmNO != 0)
            {
                if (fCheckFG == true)
                {
                    //表示項目
                    string sConfirmNM = ""; //対応内容
                    bool flg = true;        //有効
                    string sOpeNM = "";     //対応者

                    string BaseSql = "Select Confirm_NM,Product_FG,Operator_NM From TnQCNRCnfm WITH(NOLOCK) Where Inline_CD={0} AND QCNR_NO={1}";
                    string sqlCmdTxt = string.Format(BaseSql, nInlineCD, nQcnrNO);

                    using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                    {
                        SqlDataReader reader = null;
                        try
                        {
                            connect.Command.CommandText = sqlCmdTxt;
                            reader = connect.Command.ExecuteReader();
                            while (reader.Read())
                            {
                                sConfirmNM = Convert.ToString(reader["Confirm_NM"]);
                                flg = Convert.ToBoolean(reader["Product_FG"]);
                                sOpeNM = Convert.ToString(reader["Operator_NM"]);
                            }
                        }
                        finally
                        {
                            if (reader != null) reader.Close();
                            connect.Close();
                        }
                    }

                    //コントロールへ
                    lblCnfmNo.Text = Convert.ToString(nCnfmNO);
                    txtbBackground.Text = sConfirmNM;
                    if (flg == true)
                    {
                        rbOn.Checked = true;
                        rbOff.Checked = false;
                    }
                    else
                    {
                        rbOn.Checked = false;
                        rbOff.Checked = true;
                    }
                    txtbUpdUser.Text = sOpeNM;
                    btnFix.Enabled = false;
                    btnDelete.Enabled = true;
                }
                else//対応入力されていたが、削除されていた場合
                {
                    lblCnfmNo.Text = Convert.ToString(nCnfmNO + 1);
                    txtbBackground.Text = "";
                    rbOn.Checked = true;
                    txtbUpdUser.Text = "";
                    btnDelete.Enabled = false;
                }
            }
            else
            {
                lblCnfmNo.Text = Convert.ToString(nCnfmNO + 1);
                txtbBackground.Text = "";
                rbOn.Checked = true;
                txtbUpdUser.Text = "";
                btnDelete.Enabled = false;
            }
        }

        private void SetFixedList()
        {
            string BaseSql = "Select Fixed_NM From TmFNM WITH(NOLOCK) Where Assets_NM='{0}' AND Del_FG = 0 ORDER BY Fixed_NO DESC";
            string sqlCmdTxt = string.Format(BaseSql, sAssetsNM);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        int i = 0;
                        lstbFixedNM.Items.Insert(i, Convert.ToString(reader["Fixed_NM"]));
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }

        //OKボタン
        private void btnFix_Click(object sender, EventArgs e)
        {
            //[対応内容][対応者]が未記入でなければ、OKとする
            if (fInputCheck())
            {
                int nflg = 0;
                if (rbOn.Checked == true)
                {
                    nflg = 1;
                }
                else
                {
                    nflg = 0;
                }

                //List<int> ListQCNRNo = Com.GetTnQCNRSameNo(nInlineCD,sEquipmentNO,nInspectionNO,dtMeasureDT);
                List<int> ListQCNRNo = Com.GetTnQCNRSameNo(nInlineCD, _sEquiNO, nInspectionNO, dtMeasureDT,0);
                //Com.InsertTnQCNRCnfm(nInlineCD, nQcnrNO, nCnfmNO + 1, txtbBackground.Text, nflg, txtbUpdUser.Text);
                //Com.SetTnQCNRCheckFG(1, nInlineCD, nQcnrNO);

                //同一エラーは1入力でOK対応
                for (int i = 0; i < ListQCNRNo.Count; i++)
                {
                    //データベース登録
                    Com.InsertTnQCNRCnfm(nInlineCD, ListQCNRNo[i], nCnfmNO + 1, txtbBackground.Text, nflg, txtbUpdUser.Text);
                    Com.SetTnQCNRCheckFG(1, nInlineCD, ListQCNRNo[i]);
                }

				InputFg = true;

                DialogResult result=MessageBox.Show(Constant.MessageInfo.Message_8);

                 //OK押下
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.Close();
                }
            }
        }

        private bool fInputCheck()
        {
            if (txtbBackground.Text == "" || txtbUpdUser.Text == "")
            {
                MessageBox.Show(Constant.MessageInfo.Message_51);
                return false;
            }

            if (txtbBackground.Text.Length > 400)
            {
                MessageBox.Show(Constant.MessageInfo.Message_52);
                return false;
            }

            if (txtbUpdUser.Text.Length > 32)
            {
                MessageBox.Show(Constant.MessageInfo.Message_53);
                return false;
            }
            return true;
        }

        //キャンセルボタン
        private void btnCancel_Click(object sender, EventArgs e)
        {
			InputFg = false;
            this.Close();
        }

        //[対応内容]変更
        private void txtbBackground_TextChanged(object sender, EventArgs e)
        {
            if (btnFix.Enabled == false)
            {
                btnFix.Enabled = true;
                btnDelete.Enabled = false;
                lblCnfmNo.Text = Convert.ToString(nCnfmNO + 1);
                lblCnfmNo.ForeColor = Color.Red;
            }
        }
        //[対応者]変更
        private void txtbUpdUser_TextChanged(object sender, EventArgs e)
        {
            if (btnFix.Enabled == false)
            {
                btnFix.Enabled = true;
                btnDelete.Enabled = false;
                lblCnfmNo.Text = Convert.ToString(nCnfmNO + 1);
                lblCnfmNo.ForeColor = Color.Red;
            }
        }
        //[有効]ボタン変更
        private void rbOn_CheckedChanged(object sender, EventArgs e)
        {
            if (btnFix.Enabled == false)
            {
                btnFix.Enabled = true;
                btnDelete.Enabled = false;
                lblCnfmNo.Text = Convert.ToString(nCnfmNO + 1);
                lblCnfmNo.ForeColor = Color.Red;
            }
        }
        //[無効]ボタン変更
        private void rbOff_CheckedChanged(object sender, EventArgs e)
        {
            if (btnFix.Enabled == false)
            {
                btnFix.Enabled = true;
                btnDelete.Enabled = false;
                lblCnfmNo.Text = Convert.ToString(nCnfmNO + 1);
                lblCnfmNo.ForeColor = Color.Red;
            }
        }

        //[削除]ボタン押下
        private void btnDelete_Click(object sender, EventArgs e)
        {
            List<int> ListQCNRNo = Com.GetTnQCNRSameNo(nInlineCD, _sEquiNO, nInspectionNO, dtMeasureDT,1);
            
            //同一エラーは1入力でOK対
            for (int i = 0; i < ListQCNRNo.Count; i++)
            {
                //削除
                //Com.DeleteTnLOGCnfm(nInlineCD,sEquipmentNO,dtMesureDT,nSeqNO,nQcParamNO);
                //Com.SetTnLOGCheckFG(0, nInlineCD, sEquipmentNO, dtMesureDT, nSeqNO, nQcParamNO, dtLastUpdDT);
                //Com.DeleteTnQCNRCnfm(nInlineCD, nQcnrNO);
                //Com.SetTnQCNRCheckFG(0, nInlineCD, nQcnrNO);

                Com.DeleteTnQCNRCnfm(nInlineCD, ListQCNRNo[i], nCnfmNO);
                Com.SetTnQCNRCheckFG(0, nInlineCD, ListQCNRNo[i]);
            }
            DialogResult result = MessageBox.Show(Constant.MessageInfo.Message_13);
        }

        //挿入ボタン押下
        private void btnInsert_Click(object sender, EventArgs e)
        {
            txtbBackground.Text = txtbBackground.Text + lstbFixedNM.Text;
        }


    }
}