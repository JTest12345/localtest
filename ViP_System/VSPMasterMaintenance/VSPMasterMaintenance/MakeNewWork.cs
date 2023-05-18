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
    public partial class MakeNewWork : Form
    {
        public MakeNewWork()
        {
            InitializeComponent();

            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = "SELECT procno, procnm, workcd FROM TmProcess order by procno asc";
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        workcombo.Items.Add(SQLite.ParseString(reader["procno"]) + "_" + SQLite.ParseString(reader["procnm"]) + "_" + SQLite.ParseString(reader["workcd"]));
                    }
                }
                cmd.Parameters.Clear();
            }
        }

        private void CheckWork_Click(object sender, EventArgs e)
        {
            //作業追加時
            //TmWorkFlow 機種(Type)追加時にも実施するため、どちらで追加するのか
            //TmProcess ★ここに登録されている前提
            //TmProcCondCheck
            //TmWorkCond

            string procno;
            procno = workcombo.Text.Substring(0, workcombo.Text.IndexOf("_")); ;

            workcheckresult.ResetText();
            int count = 0;

            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //TmWorkFlow
                cmd.CommandText = "SELECT typecd, workorder FROM TmWorkFlow where procno = @pr";
                cmd.Parameters.Add(new SqlParameter("@pr", procno));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        count++;
                        workcheckresult.AppendText("【TmWorkFlow】procno:" + procno + ", workorder:" + SQLite.ParseString(reader["workorder"]) +
                            ", typecd:" + SQLite.ParseString(reader["typecd"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        workcheckresult.AppendText("【TmWorkFlow】登録なし:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();
            }


        }
    }
}
