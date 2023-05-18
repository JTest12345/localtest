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

namespace LMResultView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void markF_read_Click(object sender, EventArgs e)
        {
            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";

            List<MarkF> retv = new List<MarkF>();

            dataGridViewF.Rows.Clear();

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //SELECTコマンド
                cmd.CommandText = "SELECT aoino ,pcbno ,pcblotno ,markstr FROM TnPcbMarkF where pcblotno like @pl order by aoino asc";
                cmd.Parameters.Add(new SqlParameter("@pl", kibanlot.Text + "%"));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MarkF m = new MarkF();
                        m.aoino = SQLite.ParseInt(reader["aoino"]);
                        m.pcbno = SQLite.ParseInt(reader["pcbno"]);
                        m.pcblotno = SQLite.ParseString(reader["pcblotno"]);
                        m.markstr = SQLite.ParseString(reader["markstr"]);

                        retv.Add(m);
                    }
                }

                foreach (MarkF List in retv)
                {
                    dataGridViewF.Rows.Add(List.aoino, List.pcbno, List.pcblotno, List.markstr);

                }

                con.Close();
            }

            }

        private void magresult_Click(object sender, EventArgs e)
        {
            MarkM MM = new MarkM();
            MM.Show();
        }
    }
}
