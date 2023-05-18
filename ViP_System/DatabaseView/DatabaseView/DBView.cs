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

namespace DatabaseView
{
    public partial class DBView : Form
    {
        public DBView()
        {
            InitializeComponent();

            //データベースの名前を抽出してコンボボックスへ追加
            DBname_combo.Items.Add("ARMS");
            //DBname_combo.Items.Add("ARMS_MST");
            DBname_combo.Items.Add("QCIL");
            DBname_combo.Items.Add("LENS");
            DBname_combo.Items.Add("OVNP");
            DBname_combo.Items.Add("PMMS");

            List<string> retv = new List<string>();

            //テーブルの名前を抽出してコンボボックスへ追加
            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
 
                var dt = con.GetSchema("Tables");
                foreach (DataRow row in dt.Rows)
                {
                    var tableName = row["TABLE_NAME"].ToString();
                    retv.Add(tableName);                   
                }
                //一度昇順に並べ替え
                retv.Sort();

                foreach (string row in retv)
                {
                    tablename_combo.Items.Add(row);
                }                             
            }
        }

        private void chooseDB_Click(object sender, EventArgs e)
        {
            string db_name;
            db_name = DBname_combo.Text;

            //テーブル名を選択していない場合、エラーになるので、回避するように修正 20220627
            if (tablename_combo.Text == "")
            {
                return;
            }

            List<string> retv = new List<string>();
            dataGridView1.Rows.Clear();
            // 全ての列を全削除
            dataGridView1.Columns.Clear();

            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            //constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=" + db_name + ";UID=inline;PWD=R28uHta;database=" + db_name + ";Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                var dt = con.GetSchema("Columns");
                foreach (DataRow row in dt.Rows)
                {
                    var tableName = row["TABLE_NAME"].ToString();

                    if(tablename_combo.Text == tableName) 
                    {
                        var fieldName = row["COLUMN_NAME"].ToString();
                        if (fieldName != "lineno")
                        {
                            retv.Add(fieldName + ", ");
                            dataGridView1.Columns.Add(fieldName, fieldName);
                        }
                    }                   
                }
                
                string colname_all = "";
               
                string temp = "";
                
                int count=0;
                //データをTOP100ほど抽出
                //カラムの順序に気を付けて追加していく
                foreach (string colname in retv)
                {
                    colname_all += colname;
                }
                
                colname_all = colname_all.Substring(0, colname_all.Length - 2);

                cmd.CommandText = "SELECT TOP 100 ";
              
                //cmd.Parameters.Add(new SqlParameter("@col", colname_all));
                //cmd.Parameters.Add(new SqlParameter("@dname", tablename_combo.Text));
                cmd.CommandText += colname_all;
                cmd.CommandText += " FROM " + tablename_combo.Text;

                if (colname_combo.Text != "")
                {
                    cmd.CommandText += " where " + colname_combo.Text + " like '" + search_val.Text + "%'";

                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //gridrow_all = "";
                        count = 0;
                        foreach (string colname in retv)
                        {
                            temp = SQLite.ParseString(reader[colname.Substring(0, colname.Length - 2)]);
                            

                            if(count == 0) 
                            {
                                dataGridView1.Rows.Add(temp);
                            }
                            else
                            {
                                dataGridView1.Rows[dataGridView1.Rows.Count-1].Cells[count].Value = temp;
                            }
                            count = count + 1;

                            /*if (temp == "") 
                            {
                                temp = "NULL";                           
                            }
                            gridrow_all += "\"" + temp + "\"" + ",";
                            */
                            
                        }
                        //gridrow_all = gridrow_all.Substring(0, gridrow_all.Length - 1);
                        //gridrow_all = gridrow_all.Replace("\"","");
                        
                        //dataGridView1.Rows.Add(gridrow_all);
                        //dataGridView1.Rows.Add("BC", "1", "test", "2022/01/09", "NA", "1", "1");
                    }
                }
                cmd.Parameters.Clear();

                /*
                //データ追加までのサンプルコード
                cmd.CommandText = "SELECT aoino ,magno ,typecd ,materialcd, workstdt, workenddt FROM TnPcbMarkM where magno like @mag and materialcd like @mat order by workstdt desc";
                cmd.Parameters.Add(new SqlParameter("@mag", magnotext.Text + "%"));
                cmd.Parameters.Add(new SqlParameter("@mat", materialcombo.Text + "%"));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MarkM_Col m = new MarkM_Col();
                        m.aoino = SQLite.ParseInt(reader["aoino"]);
                        m.magno = SQLite.ParseString(reader["magno"]);
                        m.typecd = SQLite.ParseString(reader["typecd"]);
                        m.materialcd = SQLite.ParseString(reader["materialcd"]);
                        m.workstdt = SQLite.ParseDate(reader["workstdt"]) ?? DateTime.MinValue;
                        m.workenddt = SQLite.ParseDate(reader["workenddt"]) ?? DateTime.MinValue;

                        retv.Add(m);
                    }
                }

                foreach (MarkM_Col List in retv)
                {
                    dataGridViewM.Rows.Add(List.aoino, List.magno, List.typecd, List.materialcd, List.workstdt, List.workenddt);
                }
                */
            }
        }

        private void DBname_combo_TextChanged(object sender, EventArgs e)
        {
            string db_name;
            db_name = DBname_combo.Text;
            List<string> retv = new List<string>();

            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name="+ db_name + ";UID=inline;PWD=R28uHta;database=" + db_name + ";Max Pool Size=100";

            //テーブル名クリア
            tablename_combo.Items.Clear();

            //新しく選択したデータベース名で再度テーブル名を取得してコンボボックスへ追加
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                var dt = con.GetSchema("Tables");
                foreach (DataRow row in dt.Rows)
                {
                    var tableName = row["TABLE_NAME"].ToString();
                    retv.Add(tableName);
                }
                //一度昇順に並べ替え
                retv.Sort();

                foreach (string row in retv)
                {
                    tablename_combo.Items.Add(row);
                }
            }
        }

        private void tablename_combo_TextChanged(object sender, EventArgs e)
        {
            string db_name;
            db_name = DBname_combo.Text;
            List<string> retv = new List<string>();

            //テーブル名クリア
            colname_combo.Items.Clear();
            search_val.Clear();

            string constr;     
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            //constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=" + db_name + ";UID=inline;PWD=R28uHta;database=" + db_name + ";Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                var dt = con.GetSchema("Columns");
                foreach (DataRow row in dt.Rows)
                {
                    var tableName = row["TABLE_NAME"].ToString();

                    if (tablename_combo.Text == tableName)
                    {
                        var fieldName = row["COLUMN_NAME"].ToString();
                        if (fieldName != "lineno")
                        {
                            retv.Add(fieldName);
                        }
                    }
                }
                //一度昇順に並べ替え
                retv.Sort();

                foreach (string row2 in retv)
                {
                    colname_combo.Items.Add(row2);
                }

            }
        }
    }
}
