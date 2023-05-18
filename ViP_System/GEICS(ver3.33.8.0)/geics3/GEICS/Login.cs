using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;

namespace GEICS
{
    public partial class Login : Form
    {
        static List<string> ListAdmin=new List<string>();

        public Login()
        {
            InitializeComponent();
            MyInitialize();
        }

        private void MyInitialize()
        {
            GetMasterAdmin();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //ROOTSのCommonAPI メソッド名:Login2の引数群
            //string domain = "Nichia";           //ﾄﾞﾒｲﾝ名
            string username = txtbUserID.Text.Trim();//ﾕｰｻﾞ名
            string password = txtbPassword.Text.Trim();//ﾊﾟｽﾜｰﾄﾞ
            //int langkb = 1;                     //1=日本語
            //string majorver = "1";              //MajorVer=1を指定する事
            //string minorver = "2";              //MinorVer=2を指定する事
            //string revisionver = "0";           //RevisionVer=適当に指定する
            //int maxct = 0;                      //最大取得数
            //string appname = "GEICS";           //ｱﾌﾟﾘｹｰｼｮﾝ名
            //bool simple = false;                //ﾓｰﾄﾞ  true    =ADからの情報のみ取得。ｾｯｼｮﾝ保持しない

            //string sLoginLevel = "";
            //string sWork, sUserID = "", sName = "", sSyozoku = "";

            //2011.10.3 HIshiguchi　外注展開できないので認証を廃止
            //GEICS.rweb.CommonAPI api = new GEICS.rweb.CommonAPI();
            //GEICS.rweb.LoginUserReturn UserReturn = new GEICS.rweb.LoginUserReturn();
            //UserReturn = api.Login2(domain, username, password, langkb, majorver, minorver, revisionver, maxct, appname, simple);

            //ログイン出来た場合
            //if (UserReturn.MsgCd == 0)
            //{
            //    //書き込み権限(TmADMINに登録あり)の場合
            //    if (ListAdmin.Contains(username) == true)
            //    {
            //        this.Close();

            //        //次画面へ
            //        frmMasterSetting frmMasterSetting;
            //        frmMasterSetting = new frmMasterSetting(username, true);//書き込み権限
            //        frmMasterSetting.Show();
            //    }
            //    else
            //    {//読み込み権限(TmADMINに登録なし)の場合
            //        //次画面へ
            //        frmMasterSetting frmMasterSetting;
            //        frmMasterSetting = new frmMasterSetting(username, false);//読み取り権限
            //        frmMasterSetting.Show();
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("ユーザ名/パスワードに誤りがあります。");
            //}
            //2011.10.3----------------------------------------------------

            if (txtbUserID.Text == "")
            {
                MessageBox.Show(Constant.MessageInfo.Message_1);
                return;
            }

            //書き込み権限(TmADMINに登録あり)の場合
            if (ListAdmin.Contains(username) == true)
            {
                this.Close();

                //次画面へ
                frmMasterSetting frmMasterSetting;
                frmMasterSetting = new frmMasterSetting(true);//書き込み権限
                frmMasterSetting.Show();
            }else
            {//読み込み権限(TmADMINに登録なし)の場合
                //次画面へ
                frmMasterSetting frmMasterSetting;
                frmMasterSetting = new frmMasterSetting(false);//読み取り権限
                frmMasterSetting.Show();
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void GetMasterAdmin()
        {
            string BaseSql = "", sqlCmdTxt = "";

            BaseSql = "SELECT Login_UID From  TmADMIN WITH(NOLOCK)";
            sqlCmdTxt = string.Format(BaseSql);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        ListAdmin.Add(Convert.ToString(reader["Login_UID"]).Trim());
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }
    }
}
