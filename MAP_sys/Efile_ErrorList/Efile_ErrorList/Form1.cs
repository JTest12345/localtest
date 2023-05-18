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
using System.IO;

namespace Efile_ErrorList
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Efile_date.Value = DateTime.Now.AddDays(-1);
        }

        private void get_efile_Click(object sender, EventArgs e)
        {
            //string enddt = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            //選択した日付を代入
            string enddt = Efile_date.Value.ToString("yyyyMMdd");
            
            List<ADEP001_ARMS> retv = new List<ADEP001_ARMS>();

            string constr = "server=svopt01;Connect Timeout=30;Application Name=OEMTest;UID=sa;PWD=admin_4121opt;database=OEMTest;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT ロット番号, 型番, 作業名称, 号機名, 完了日, 完了時間 FROM ADEP001_ARMS where " +
                        "完了日 like @enddt and 作業名称 like @workname order by 作業名称 asc, 完了時間 asc";
                cmd.Parameters.Add(new SqlParameter("@enddt", enddt));
                cmd.Parameters.Add(new SqlParameter("@workname", "%" + "ダイボン作業"));


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ADEP001_ARMS AData = new ADEP001_ARMS();
                        AData.lotno = SQLite.ParseString(reader["ロット番号"]);
                        AData.type = SQLite.ParseString(reader["型番"]);
                        AData.workname = SQLite.ParseString(reader["作業名称"]);
                        AData.machinename = SQLite.ParseString(reader["号機名"]);
                        AData.enddt = SQLite.ParseString(reader["完了日"]);
                        AData.endtime = SQLite.ParseString(reader["完了時間"]);
                        //AData.lastupddt = SQLite.ParseDate(reader["lastupddt"]) ?? DateTime.MinValue;

                        retv.Add(AData);
                    }
                }
                cmd.Parameters.Clear();


                con.Close();

                //Eファイルのフォルダのファイル名を取得
                string path = @"\\ce05006\E_File\" + enddt;

                //フォルダ存在チェック
                if (!Directory.Exists(path))
                {
                    MessageBox.Show(path + "のフォルダがありません");
                    return;
                }

                    string[] names = Directory.GetFiles(path, "*");
                List<string> filenames = new List<string>();
                
                foreach (string name in names)
                {
                    string lotselect = Path.GetFileName(name).Substring(9,11);
                    filenames.Add(lotselect);
                }

                //Eファイルの有無
                bool efile_exist;
                int existcount = 0;
                int nonexistcount = 0;
                Efile_View.Rows.Clear();

                foreach (ADEP001_ARMS list in retv)
                {
                    efile_exist = false;
                    foreach(string fname in filenames)
                    {
                        if(list.lotno == fname)
                        {
                            efile_exist = true;
                            break;
                        }
                    }
                    //Eファイルがあった場合
                    if (efile_exist == true)
                    {
                        existcount += 1;
                        Efile_View.Rows.Add(list.lotno, list.type, list.workname, list.machinename, list.enddt, list.endtime, "〇");
                    }
                    //Eファイルが無かった場合
                    else if (efile_exist == false)
                    {
                        nonexistcount += 1;
                        Efile_View.Rows.Add(list.lotno, list.type, list.workname, list.machinename, list.enddt, list.endtime, "×");
                    }
                    
                }
                existtext.Text = existcount.ToString();
                nonexisttext.Text = nonexistcount.ToString();
            }
        }
    }
}
