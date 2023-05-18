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

namespace Make_QR
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //サーバ用
            //string constr = "server=10.129.97.150\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";
            string constr = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=ARMS;Integrated Security=true";
            string contents = textBox1.Text;

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //SELECTコマンド
                cmd.CommandText = "SELECT plantcd FROM TnMacMnt WHERE macno = @MntMacNo";

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@MntMacNo", System.Data.SqlDbType.NVarChar).Value = contents;

                //object R2 = cmd.ExecuteScalar();
                string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                //Updateコマンド
                cmd.CommandText = " UPDATE TnMacMnt SET delfg = 1  WHERE macno = @MntMacNo ";
                cmd.ExecuteNonQuery();

                //Insertコマンド
                cmd.CommandText = cmd.CommandText = @"
                            INSERT
                             INTO TnMacMnt(macno
	                            , plantcd
	                            , maintedate
	                            , delfg)
                            values(@MntMacNo
	                            , @Plandcd
	                            , @DateT
	                            , @delfg_val)";

                cmd.Parameters.Add(new SqlParameter("@Plandcd", "TSTEST2"));
                cmd.Parameters.Add(new SqlParameter("@DateT", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("@delfg_val", 1));

                cmd.ExecuteNonQuery();

            }
            

        }

        private void Alert_Click(object sender, EventArgs e)
        {
            //以下、グリッドにデータを追加するときの処理サンプル
            //dataGridView1.ReadOnly = false;
            /*dataGridView1.Rows.Clear();

            MntData M = new MntData();
            //MntData M;
            M.MacNo = 123456;
            M.Plantcd = "TSDB001";
            M.MntDt = DateTime.Now;
            M.DelFg = false;

            dataGridView1.Rows.Add(M.MacNo, M.Plantcd, M.MntDt, M.DelFg);

            M.MacNo = 654321;
            M.Plantcd = "TSDB002";
            M.MntDt = DateTime.Now;
            M.DelFg = false;

            this.dataGridView1.Rows.Add(new object[] { M.MacNo, M.Plantcd, M.MntDt, M.DelFg });
            */
            //一度リスト更新をしておく。表の時刻を修正した後に値を取得すると、DateTime型からString型になり、エラーとなるため
            GetListBtn_Click(sender, e);

            int count_row;
            DateTime six_month_after;
            string str;
            count_row = dataGridView1.Rows.Count;
            //count_col = dataGridView1.Columns.Count;
            if (count_row > 0)
            {

                for (int i = 0; i < count_row; i++)
                {
                    //if ((dataGridView1.Rows[i].Cells[2].Value is DateTime) == true)
                            //{
                        six_month_after = (DateTime)dataGridView1.Rows[i].Cells[2].Value;
                    //}

                    //1か月以内の設備を赤文字にする
                    if (DateTime.Now.AddMonths(1) > six_month_after)
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                    }
                    //3か月以内の設備を黄色文字にする
                    else if (DateTime.Now.AddMonths(3) > six_month_after)
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                    }

                }
            }
        }

        private void GetListBtn_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            List<MntData> retv = new List<MntData>();

            string constr = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=ARMS;Integrated Security=true";
            string contents = textBox1.Text + "%";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //SELECTコマンド
                cmd.CommandText = "SELECT macno ,plantcd ,maintedate ,delfg FROM TnMacMnt where macno like @MN order by macno asc";
                if (contents == "")
                {
                    contents = "%";
                }
                cmd.Parameters.Add(new SqlParameter("@MN", contents));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MntData m = new MntData();
                        m.MacNo = SQLite.ParseInt(reader["macno"]);
                        m.Plantcd = SQLite.ParseString(reader["plantcd"]);
                        m.MntDt = SQLite.ParseDate(reader["maintedate"]) ?? DateTime.MinValue;
                        m.DelFg = SQLite.ParseBool(reader["delfg"]);


                        retv.Add(m);
                    }
                }

                foreach (MntData List in retv)
                {
                    dataGridView1.Rows.Add(List.MacNo, List.Plantcd, List.MntDt, List.DelFg);

                }
            }
        }

         private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void DBUpdate_Click(object sender, EventArgs e)
        {


            // List<MntData> retv = new List<MntData>();
            int count_row;
            //int count_col;
            string constr = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=ARMS;Integrated Security=true";
            //string contents = textBox1.Text;

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                //cmd.CommandText = "TRUNCATE table TnMacMnt;";
                //cmd.ExecuteNonQuery();

                //SELECTコマンド
                //cmd.CommandText = "SELECT macno ,plantcd ,maintedate ,delfg FROM TnMacMnt order by macno asc";
                count_row = dataGridView1.Rows.Count;
                //count_col = dataGridView1.Columns.Count;
                if (count_row > 0)
                {
                    
                    for (int i = 0; i < count_row; i++)
                    {
                        cmd.CommandText = " UPDATE TnMacMnt SET maintedate = @date  WHERE macno = @MntMacNo ";
                        cmd.Parameters.Add(new SqlParameter("@date", dataGridView1.Rows[i].Cells[2].Value));
                        cmd.Parameters.Add(new SqlParameter("@MntMacNo", dataGridView1.Rows[i].Cells[0].Value));
                        //Insertコマンド
                        /*cmd.CommandText = cmd.CommandText = @"
                        INSERT
                         INTO TnMacMnt(macno
                            , plantcd
                            , maintedate
                            , delfg)
                        values(@macno
                            , @Plandcd
                            , @DateT
                            , @delfg_val)";

                    //if (i == 0)
                    //{
                        cmd.Parameters.Add(new SqlParameter("@macno", dataGridView1.Rows[i].Cells[0].Value));
                        cmd.Parameters.Add(new SqlParameter("@Plandcd", dataGridView1.Rows[i].Cells[1].Value));
                        cmd.Parameters.Add(new SqlParameter("@DateT", dataGridView1.Rows[i].Cells[2].Value));
                        cmd.Parameters.Add(new SqlParameter("@delfg_val", dataGridView1.Rows[i].Cells[3].Value));*/
                        //}
                        /*else
                        {
                            //cmd.Parameters.Add("@MntMacNo", System.Data.SqlDbType.NVarChar).Value = dataGridView1.Rows[i].Cells[0].Value;
                            cmd.Parameters.Add("@macno", System.Data.SqlDbType.NVarChar).Value = dataGridView1.Rows[i].Cells[0].Value;
                            cmd.Parameters.Add("@Plandcd", System.Data.SqlDbType.NVarChar).Value = dataGridView1.Rows[i].Cells[1].Value;
                            cmd.Parameters.Add("@DateT", System.Data.SqlDbType.NVarChar).Value = dataGridView1.Rows[i].Cells[2].Value;
                            cmd.Parameters.Add("@delfg_val", System.Data.SqlDbType.NVarChar).Value = dataGridView1.Rows[i].Cells[3].Value;

                        }*/
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                    }
                }


            }
        }
    }
}
