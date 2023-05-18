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
    public partial class MarkM : Form
    {
        public MarkM()
        {
            InitializeComponent();

            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = "SELECT DISTINCT materialcd FROM TnPcbMarkM";
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        materialcombo.Items.Add(SQLite.ParseString(reader["materialcd"]));
                    }
                }
                cmd.Parameters.Clear();
            }
        }

        private void SearchM_Click(object sender, EventArgs e)
        {
            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";

            List<MarkM_Col> retv = new List<MarkM_Col>();

            dataGridViewM.Rows.Clear();

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //SELECTコマンド
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

                con.Close();
            }
        }
    }
}
