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

namespace DBErrorList
{
    public partial class DBErrorList : Form
    {
        public DBErrorList()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Now.AddMonths(-1);
        }

        private void ErrorListRead_Click(object sender, EventArgs e)
        {
            ErrorListView.Rows.Clear();
            List<DBcol> retv = new List<DBcol>();
            DateTime d;

            d = dateTimePicker1.Value;

            string constr = "server=svopt01;Connect Timeout=30;Application Name=OEMTest;UID=sa;PWD=admin_4121opt;database=OEMTest;Max Pool Size=100";
            string contents = macfilter.Text + "%";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //SELECTコマンド
                //CheckBoxがチェックされている場合
                if (OldDataCheckBox.Checked)
                {
                    cmd.CommandText = "SELECT ymd ,hms ,assenlot ,macno, x, y, area, kibanno, LorR, chipad, filetype, delfg FROM DBMonitor where macno like @MN and ymd >= @YMD order by ymd desc,hms desc";
                }
                else
                {
                    cmd.CommandText = "SELECT ymd ,hms ,assenlot ,macno, x, y, area, kibanno, LorR, chipad, filetype, delfg FROM DBMonitor where delfg = 0 and macno like @MN and ymd >= @YMD order by ymd desc, hms desc";
                }

                if (contents == "")
                {
                    contents = "%";
                }
                cmd.Parameters.Add(new SqlParameter("@YMD", d));
                cmd.Parameters.Add(new SqlParameter("@MN", contents));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DBcol m = new DBcol();
                        m.ymd = SQLite.ParseString(reader["ymd"]);
                        m.hms = SQLite.ParseString(reader["hms"]);
                        m.assenlot = SQLite.ParseString(reader["assenlot"]);
                        m.macno = SQLite.ParseString(reader["macno"]);                       
                        m.x = SQLite.ParseSingle(reader["x"]);
                        m.y = SQLite.ParseSingle(reader["y"]);
                        m.area = SQLite.ParseSingle(reader["area"]);
                        m.kibanno = SQLite.ParseString(reader["kibanno"]);
                        m.LorR = SQLite.ParseString(reader["LorR"]);
                        m.chipad = SQLite.ParseString(reader["chipad"]);
                        m.filetype = SQLite.ParseString(reader["filetype"]);
                        m.delfg = SQLite.ParseInt(reader["delfg"]);

                        retv.Add(m);
                    }
                }

                foreach (DBcol List in retv)
                {
                    DataGridViewCheckBoxColumn colChk = new DataGridViewCheckBoxColumn();
                    ErrorListView.Rows.Add(colChk.TrueValue, List.ymd, List.hms, List.assenlot, List.macno, List.x, List.y, List.area, List.kibanno, List.LorR, List.chipad, List.filetype, List.delfg);
                }
            }
        }

        private void GetCheckBoxTF_Click(object sender, EventArgs e)
        {
            int count_row;
            int checkbox_count = 0;
            count_row = ErrorListView.Rows.Count;
            if (count_row > 0)
            {
                string constr = "server=svopt01;Connect Timeout=30;Application Name=OEMTest;UID=sa;PWD=admin_4121opt;database=OEMTest;Max Pool Size=100";
                string contents = macfilter.Text + "%";

                using (SqlConnection con = new SqlConnection(constr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    for (int i = 0; i < count_row; i++)
                    {
                        //チェックボックスがチェックされているか確認
                        if (true.Equals(ErrorListView.Rows[i].Cells[0].Value))
                        {
                            
                            //ErrorListView.Rows[i].DefaultCellStyle.BackColor = Color.Red;
                            cmd.CommandText = " UPDATE DBMonitor SET delfg = 1 WHERE ymd = @YMD and hms = @HMS and assenlot = @ASSENLOT and macno = @MACNO";
                            cmd.Parameters.Add(new SqlParameter("@YMD", ErrorListView.Rows[i].Cells[1].Value));
                            cmd.Parameters.Add(new SqlParameter("@HMS", ErrorListView.Rows[i].Cells[2].Value));
                            cmd.Parameters.Add(new SqlParameter("@ASSENLOT", ErrorListView.Rows[i].Cells[3].Value));
                            cmd.Parameters.Add(new SqlParameter("@MACNO", ErrorListView.Rows[i].Cells[4].Value));

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            checkbox_count++;
                        }
                    }

                    if (checkbox_count == 0)
                    {
                        MessageBox.Show("チェックされていないため、更新するデータはありません.");
                    }
                    else
                    {
                        MessageBox.Show(checkbox_count.ToString() + "件のデータを更新しました.");
                    }
                    //エラーリスト読込ボタンをクリックした動作を呼び出す
                    ErrorListRead.PerformClick();
                }
            }
        }
    }
}
