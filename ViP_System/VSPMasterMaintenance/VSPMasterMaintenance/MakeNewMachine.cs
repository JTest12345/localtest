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
    public partial class MakeNewMachine : Form
    {
        public MakeNewMachine()
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
                cmd.CommandText = "SELECT macno, plantcd, clasnm FROM TmMachine order by macno asc";
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if(SQLite.ParseString(reader["plantcd"]) != "-") 
                        {
                            maccombo.Items.Add(SQLite.ParseString(reader["macno"]) + "_" + SQLite.ParseString(reader["plantcd"]) + "_" + SQLite.ParseString(reader["clasnm"]));
                        }
                        
                    }

                }
                cmd.Parameters.Clear();
            }
        }

        private void CheckMachine_Click(object sender, EventArgs e)
        {
            //設備追加時
            //TmHandover ロボット
            //TmMachine ★ここに登録されている前提
            //TmProMac
            //TmRoute
            //LENS->TmMachine

            //PlantCDチェックするなら以下必要
            //QCIL->EQUI
            //QCIL->LSET
            

            //18101 #1A 18201 #1B
            //18102 #2A 18202 #2B

            string Search_Type_Name;
            string macno;
            string plantcd;
            Search_Type_Name = maccombo.Text;
            maccheckresult.ResetText();
            int count=0;
            

            if (Search_Type_Name == "")
            {
                MessageBox.Show("設備を選択してください。", "設備入力エラー");
                return;
            }
            

            macno = Search_Type_Name.Substring(0, 6);
            //plantcdの文字数変更あれば、修正必要あり Substring(7, 8);
            plantcd = Search_Type_Name.Substring(7, 8);

            //設備が登録されているか確認
            List<string> arr = new List<string>();
            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                //TmProMac
                cmd.CommandText = "SELECT procno FROM TmProMac where macno like @mn";
                cmd.Parameters.Add(new SqlParameter("@mn", macno));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {                                                                                
                    while (reader.Read())
                    {
                        //arr.Add("【TmProMac】procno:" + SQLite.ParseString(reader["procno"]));
                        count++;
                        maccheckresult.AppendText("【TmProMac】procno:" + SQLite.ParseString(reader["procno"]) +"\r\n");
                    }
                    if(count == 0)
                    {
                        maccheckresult.AppendText("【TmProMac】登録なし:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();

                //TmRoute
                cmd.CommandText = "SELECT carrierno FROM TmRoute where macno like @mn";
                cmd.Parameters.Add(new SqlParameter("@mn", macno));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {                            
                    while (reader.Read())
                    {
                        count++;
                        maccheckresult.AppendText("【TmRoute】carrierno:" + SQLite.ParseString(reader["carrierno"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        maccheckresult.AppendText("【TmRoute】登録なし:" + "\r\n");
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
                //TmMachine
                cmd.CommandText = "SELECT ClassCD, MachineNM, PlcAddress FROM TmMachine where PlantCD like @pl";
                cmd.Parameters.Add(new SqlParameter("@pl", plantcd));
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //arr.Add("【TmProMac】procno:" + SQLite.ParseString(reader["procno"]));
                        count++;
                        maccheckresult.AppendText("【LENS->TmMachine】ClassCD:" + SQLite.ParseString(reader["ClassCD"]) + ", MachineNM:" + SQLite.ParseString(reader["MachineNM"]) + ", PlcAddress:" + SQLite.ParseString(reader["PlcAddress"]) + "\r\n");
                    }
                    if (count == 0)
                    {
                        maccheckresult.AppendText("【LENS->TmMachine】登録なし:" + "\r\n");
                    }
                    count = 0;
                }
                cmd.Parameters.Clear();
                con.Close();

            }
        }
    }
}
