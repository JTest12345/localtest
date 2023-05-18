using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace MacMnt
{
    public partial class MacFailInput : Form
    {
        public MacFailInput()
        {
            InitializeComponent();
        }

        private void InputInfo_Click(object sender, EventArgs e)
        {
            //DateTime d;
            //d = dateTimePicker1.Value.ToShortDateString;
            //textBox1.Text = dateTimePicker1.Value.ToShortDateString();
            //textBox1.Text = dateTimePicker1.Value.ToLongDateString();

            //テキストボックスに選択された日付を入れるサンプルコード
            //textBox1.Text = dateTimePicker1.Value.ToShortDateString();

            if (comboBox1.Text == "") 
            {
                MessageBox.Show("設備を選択してください",
                "設備未選択",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return;
            }

            if (comboBox2.Text == "")
            {
                MessageBox.Show("原因を選択してください",
                "原因未選択",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return;
            }

            if (textBox1.Text == "")
            {
                MessageBox.Show("作業者名を入力してください",
                "作業者名空白",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return;
            }

            /*if (textBox1.Text is string == true)
            {
                MessageBox.Show("作業者名が文字列です。数字で入力してください",
                "文字列入力異常",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return;
            }*/

            //macnoのみDBへ登録するため分割
            string[] macdata = comboBox1.Text.Split('_');
            string split_macno = macdata[2];

            

            string constr = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=ARMS;Integrated Security=true";
            
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Clear();

                //データが空白、または型違いの場合、リターン

                
                //Insertコマンド
                cmd.CommandText = cmd.CommandText = @"
                            INSERT
                             INTO TnMacFail(macno
	                            , cause
	                            , worker, faildate, inputdate)
                            values(@macno
	                            , @cause
	                            , @worker, @faildate, @inputdate)";

                cmd.Parameters.Add(new SqlParameter("@macno", split_macno));
                cmd.Parameters.Add(new SqlParameter("@cause", comboBox2.Text));
                cmd.Parameters.Add(new SqlParameter("@worker", textBox1.Text));
                cmd.Parameters.Add(new SqlParameter("@faildate", dateTimePicker1.Value));
                cmd.Parameters.Add(new SqlParameter("@inputdate", DateTime.Now));

                cmd.ExecuteNonQuery();

                MessageBox.Show("故障履歴データ登録されました。",
                "登録完了",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
        }

        private void MacFailInput_Load(object sender, EventArgs e)
        {
            // コンボボックスにアイテムを追加
            //comboBox1.Items.Add("123456");

            //yyyy/MM/dd HH:mm:ss
            //dateTimePicker1.Format = DateTimePickerFormat.Custom;
            //dateTimePicker1.CustomFormat = "yyyy/MM/dd";
            //dateTimePicker1.CustomFormat = "yyyy/MM/dd HH:mm:ss";

            int MN;
            string clasname;
            string plantnm;

            string constr = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=ARMS;Integrated Security=true";          

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //SELECTコマンド
                cmd.CommandText = "SELECT macno, clasnm, plantnm FROM TmMachine where [macno] not like '2%' and [macno] not like '3%' order by macno asc";               

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                        MN = SQLite.ParseInt(reader["macno"]);
                        clasname = SQLite.ParseString(reader["clasnm"]);
                        plantnm = SQLite.ParseString(reader["plantnm"]);
                        comboBox1.Items.Add(clasname + "_" + plantnm + "_" + MN);


                    }
                }

                
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            //dateTimePicker1.Value = DateTime.Now;
        }
    }
}
