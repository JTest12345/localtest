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
    public partial class frmBackground : Form
    {
        Common Com = new Common();

        //key
        static int nInlineCD;
        static string sEquipmentNO;
        static DateTime dtMesureDT;
        static int nSeqNO;
        static int nQcParamNO;
        static int nCnfmNO;
        static string sAssetsNM;
        static DateTime dtLastUpdDT;
        //sub
        static bool fCheckFG;//確認フラグ

        public frmBackground(int nw1, string sw2, DateTime dtw3, int nw4, int nw5, int nw6, bool fw7, string sw8, DateTime dtw9)
        {
            InitializeComponent();

            //key入力
            nInlineCD = nw1;
            sEquipmentNO = sw2;
            dtMesureDT = dtw3;
            nSeqNO = nw4;
            nQcParamNO = nw5;
            nCnfmNO=nw6;
            fCheckFG = fw7;
            sAssetsNM = sw8;    //設備名
            dtLastUpdDT = dtw9;
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

                    string BaseSql = "Select Confirm_NM,Product_FG,Operator_NM From TnLOGCnfm WITH(NOLOCK) "
                                    + "Where Inline_CD={0} AND Equipment_NO='{1}' AND Measure_DT='{2}' AND Seq_NO={3} AND QcParam_NO={4} AND Confirm_NO={5}";
                    string sqlCmdTxt = string.Format(BaseSql, nInlineCD, sEquipmentNO, dtMesureDT, nSeqNO, nQcParamNO, nCnfmNO);

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
                lblCnfmNo.Text = Convert.ToString(nCnfmNO+1);
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
                        lstbFixedNM.Items.Insert(i,Convert.ToString(reader["Fixed_NM"]));
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
                
                //データベース登録
                Com.InsertTnLOGCnfm(nInlineCD, sEquipmentNO, dtMesureDT, nSeqNO, nQcParamNO, nCnfmNO + 1, txtbBackground.Text, nflg, txtbUpdUser.Text);
                Com.SetTnLOGCheckFG(1, nInlineCD, sEquipmentNO, dtMesureDT, nSeqNO, nQcParamNO, dtLastUpdDT);

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

            if(txtbBackground.Text.Length>400){
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
            this.Close();
        }

        //[対応内容]変更
        private void txtbBackground_TextChanged(object sender, EventArgs e)
        {
            if (btnFix.Enabled == false)
            {
                btnFix.Enabled = true;
                btnDelete.Enabled = false;
                lblCnfmNo.Text = Convert.ToString(nCnfmNO+1);
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
            //削除
            Com.DeleteTnLOGCnfm(nInlineCD,sEquipmentNO,dtMesureDT,nSeqNO,nQcParamNO);
            Com.SetTnLOGCheckFG(0, nInlineCD, sEquipmentNO, dtMesureDT, nSeqNO, nQcParamNO, dtLastUpdDT);
            DialogResult result = MessageBox.Show(Constant.MessageInfo.Message_13);
        }

        //挿入ボタン押下
        private void btnInsert_Click(object sender, EventArgs e)
        {
            txtbBackground.Text = txtbBackground.Text+lstbFixedNM.Text;
        }
    }
}