using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace VSPMasterMaintenance
{
    public partial class MakeNewType : Form
    {
        public MakeNewType()
        {
            InitializeComponent();

            //最初にコンボボックスにコピー元品種をセットしておく
            
            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();              
                cmd.CommandText = "SELECT DISTINCT typecd FROM TmWorkFlow order by typecd asc";
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CopyType_CB.Items.Add(SQLite.ParseString(reader["typecd"]));
                    }
                }
                cmd.Parameters.Clear();
            }
        }

        private void CheckType_Click(object sender, EventArgs e)
        {
            
            string typecd;
            typecd = CopyType_CB.Text;

            typecheckresult.ResetText();
            int count = 0;

            //機種追加時]

            //ARMS
            //TmWorkFlow ★ここに登録されている前提
            //TmDBResinLife
            //TmOvenProf
            //TmWorkMacCond
            //TmDefect
            //TmDefectJudge
            //TmGeneral
            //TmTimeLimit
            //TmTypeCond

            //LENS
            //TmType

            //20221208 QCIL必要
            //TmBonding
            //TmFILEFMTTYPE
            //TmLSET
            //TmPLM
            //TmQCST
            //TnQCNR ※Tnなので修正不要かも

            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //TmDBResinLife
                cmd.CommandText = "SELECT resinmaterialcd, lifefromunpacktoworkstart, lifefromunpacktoworkend, lifefrominput, forbiddenfromunpacktoinput FROM TmDBResinLife where typecd = @ty";
                cmd.Parameters.Add(new SqlParameter("@ty", typecd));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        typecheckresult.AppendText("【TmDBResinLife】resinmaterialcd:" + SQLite.ParseString(reader["resinmaterialcd"]) + "\r\n" +
                            "解凍から開始:" + SQLite.ParseString(reader["lifefromunpacktoworkstart"]) + "\r\n" +
                            "解凍から完了:" + SQLite.ParseString(reader["lifefromunpacktoworkend"]) + "\r\n" +
                            "割付からの期限:" + SQLite.ParseString(reader["lifefrominput"]) + "\r\n" +
                            "解凍待機時間:" + SQLite.ParseString(reader["forbiddenfromunpacktoinput"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        typecheckresult.AppendText("【TmDBResinLife】登録なし:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();

                //TmOvenProf
                cmd.CommandText = "SELECT procno, profileid FROM TmOvenProf where typecd = @ty order by procno";
                cmd.Parameters.Add(new SqlParameter("@ty", typecd));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        typecheckresult.AppendText("【TmOvenProf】procno:" + SQLite.ParseString(reader["procno"]) +
                            " profileid:" + SQLite.ParseString(reader["profileid"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        typecheckresult.AppendText("【TmOvenProf】登録なし:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();

                //TmWorkMacCond
                cmd.CommandText = "SELECT procno, plantcd FROM TmWorkMacCond where typecd = @ty order by procno";
                cmd.Parameters.Add(new SqlParameter("@ty", typecd));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        typecheckresult.AppendText("【TmWorkMacCond】procno:" + SQLite.ParseString(reader["procno"]) +
                            " plantcd:" + SQLite.ParseString(reader["plantcd"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        typecheckresult.AppendText("【TmWorkMacCond】登録なし:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();

                //TmDefect
                cmd.CommandText = "SELECT itemcd FROM TmDefect where typecd = @ty";
                cmd.Parameters.Add(new SqlParameter("@ty", typecd));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        break;
                        //項目が多いため、不良コードは一覧は表示しない
                        //typecheckresult.AppendText("【TmDefect】procno:" + SQLite.ParseString(reader["procno"]) +
                            //" plantcd:" + SQLite.ParseString(reader["plantcd"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        typecheckresult.AppendText("【TmDefect】登録なし:" + "\r\n");
                    }
                    else if(count > 0)
                    {
                        typecheckresult.AppendText("【TmDefect】登録あり:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();

                //TmDefectJudge
                cmd.CommandText = "SELECT defitemcd FROM TmDefectJudge where typecd = @ty";
                cmd.Parameters.Add(new SqlParameter("@ty", typecd));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        break;
                        //項目が多いため、不良コードは一覧は表示しない
                        //typecheckresult.AppendText("【TmDefect】procno:" + SQLite.ParseString(reader["procno"]) +
                        //" plantcd:" + SQLite.ParseString(reader["plantcd"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        typecheckresult.AppendText("【TmDefectJudge】登録なし:" + "\r\n");
                    }
                    else if (count > 0)
                    {
                        typecheckresult.AppendText("【TmDefectJudge】登録あり:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();

                //TmGeneral
                cmd.CommandText = "SELECT tid FROM TmGeneral where code = @ty";
                cmd.Parameters.Add(new SqlParameter("@ty", typecd));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        break;
                        //項目が多いため、一覧は表示しない
                        //typecheckresult.AppendText("【TmDefect】procno:" + SQLite.ParseString(reader["procno"]) +
                        //" plantcd:" + SQLite.ParseString(reader["plantcd"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        typecheckresult.AppendText("【TmGeneral】登録なし:" + "\r\n");
                    }
                    else if (count > 0)
                    {
                        typecheckresult.AppendText("【TmGeneral】登録あり:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();

                //TmTimeLimit
                cmd.CommandText = "SELECT chkworkcd FROM TmTimeLimit where typecd = @ty";
                cmd.Parameters.Add(new SqlParameter("@ty", typecd));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        break;
                        //項目が多いため、一覧は表示しない
                        //typecheckresult.AppendText("【TmDefect】procno:" + SQLite.ParseString(reader["procno"]) +
                        //" plantcd:" + SQLite.ParseString(reader["plantcd"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        typecheckresult.AppendText("【TmTimeLimit】登録なし:" + "\r\n");
                    }
                    else if (count > 0)
                    {
                        typecheckresult.AppendText("【TmTimeLimit】登録あり:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();

                //TmTypeCond
                cmd.CommandText = "SELECT condcd FROM TmTypeCond where typecd = @ty";
                cmd.Parameters.Add(new SqlParameter("@ty", typecd));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        break;
                        //項目が多いため、一覧は表示しない
                        //typecheckresult.AppendText("【TmDefect】procno:" + SQLite.ParseString(reader["procno"]) +
                        //" plantcd:" + SQLite.ParseString(reader["plantcd"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        typecheckresult.AppendText("【TmTypeCond】登録なし:" + "\r\n");
                    }
                    else if (count > 0)
                    {
                        typecheckresult.AppendText("【TmTypeCond】登録あり:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();

                con.Close();
            }

            //LENSのDBへ
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=LENS;UID=inline;PWD=R28uHta;database=LENS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                //TmType
                cmd.CommandText = "SELECT FileFmtNO, MagazineID FROM TmType where TypeCD = @ty";
                cmd.Parameters.Add(new SqlParameter("@ty", typecd));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //arr.Add("【TmProMac】procno:" + SQLite.ParseString(reader["procno"]));
                        count++;
                        typecheckresult.AppendText("【LENS->TmType】FileFmtNO:" + SQLite.ParseString(reader["FileFmtNO"]) + ", MagazineID:" + SQLite.ParseString(reader["MagazineID"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        typecheckresult.AppendText("【LENS->TmType】登録なし:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();
                con.Close();

            }
        }
    }
}
