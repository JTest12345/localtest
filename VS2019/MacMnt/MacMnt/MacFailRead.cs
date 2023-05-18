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
    public partial class MacFailRead : Form
    {
        public MacFailRead()
        {
            InitializeComponent();
        }

        private void dataread_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            List<MacFailData> retv = new List<MacFailData>();

            string constr = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=ARMS;Integrated Security=true";
            string contents = textBox1.Text + "%";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //SELECTコマンド
                cmd.CommandText = "SELECT macno ,cause ,worker ,faildate, inputdate FROM TnMacFail where macno like @MN order by macno asc";

                if (contents == "")
                {
                    contents = "%";
                }
                cmd.Parameters.Add(new SqlParameter("@MN", contents));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MacFailData m = new MacFailData();
                        m.macno = SQLite.ParseInt(reader["macno"]);
                        m.cause = SQLite.ParseString(reader["cause"]);
                        m.worker = SQLite.ParseString(reader["worker"]);
                        m.faildate = SQLite.ParseDate(reader["faildate"]) ?? DateTime.MinValue;
                        m.inputdate = SQLite.ParseDate(reader["inputdate"]) ?? DateTime.MinValue;


                        retv.Add(m);
                    }
                }

                foreach (MacFailData List in retv)
                {
                    dataGridView1.Rows.Add(List.macno, List.cause, List.worker, List.faildate, List.inputdate);

                }
            }
        }
    }
}
