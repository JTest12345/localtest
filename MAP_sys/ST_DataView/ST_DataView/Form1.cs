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

namespace ST_DataView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Upper_checkBox.Checked = true;

        }

        private void GetSTdata_Click(object sender, EventArgs e)
        {
            ST_Result.Text = "";

            List<STitem> retv = new List<STitem>();

            //二つチェックされていたら、以下のチェックボックスを外す
            if(Upper_checkBox.Checked == true && Lower_checkBox.Checked == true)
            {
                Lower_checkBox.Checked = false;
            }
            if (numkisei.Text == "")
                numkisei.Text = "1";

            //テーブルの名前を抽出してコンボボックスへ追加
            string constr;
            //サーバ接続時 ※SQLite.csの接続文字列も修正する
            constr = "server=svopt01;Connect Timeout=30;Application Name=OEMTest;UID=sa;PWD=admin_4121opt;database=OEMTest;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                if (podnum.Text != "")
                {
                    if (Upper_checkBox.Checked == true)
                        cmd.CommandText = "SELECT ロット番号, 型番, 選別規格, ポット番号, 数量 FROM ADEP004 where ロット番号 like @lotname and 型番 like @typecd and 選別規格 like @STkikaku and ポット番号 = @podnum and 数量 >= @num order by ロット番号 asc,ポット番号 asc";
                    else
                        cmd.CommandText = "SELECT ロット番号, 型番, 選別規格, ポット番号, 数量 FROM ADEP004 where ロット番号 like @lotname and 型番 like @typecd and 選別規格 like @STkikaku and ポット番号 = @podnum and 数量 <= @num order by ロット番号 asc,ポット番号 asc";

                    cmd.Parameters.Add(new SqlParameter("@lotname", "%" + lotnum.Text + "%"));
                    cmd.Parameters.Add(new SqlParameter("@typecd", "%" + typecd.Text + "%"));
                    cmd.Parameters.Add(new SqlParameter("@STkikaku", "%" + STkikaku.Text + "%"));
                    cmd.Parameters.Add(new SqlParameter("@podnum", podnum.Text));
                    cmd.Parameters.Add(new SqlParameter("@num", numkisei.Text));
                }
                else if(podnum.Text == "")
                {
                    if (Upper_checkBox.Checked == true)
                        cmd.CommandText = "SELECT ロット番号, 型番, 選別規格, ポット番号, 数量 FROM ADEP004 where ロット番号 like @lotname and 型番 like @typecd and 選別規格 like @STkikaku and ポット番号 like @podnum and 数量 >= @num order by ロット番号 asc,ポット番号 asc";
                    else
                        cmd.CommandText = "SELECT ロット番号, 型番, 選別規格, ポット番号, 数量 FROM ADEP004 where ロット番号 like @lotname and 型番 like @typecd and 選別規格 like @STkikaku and ポット番号 like @podnum and 数量 <= @num order by ロット番号 asc,ポット番号 asc";

                    cmd.Parameters.Add(new SqlParameter("@lotname", "%" + lotnum.Text + "%"));
                    cmd.Parameters.Add(new SqlParameter("@typecd", "%" + typecd.Text + "%"));
                    cmd.Parameters.Add(new SqlParameter("@STkikaku", "%" + STkikaku.Text + "%"));
                    cmd.Parameters.Add(new SqlParameter("@podnum", podnum.Text + "%"));
                    cmd.Parameters.Add(new SqlParameter("@num", numkisei.Text));
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        retv.Add(new STitem { lotno = SQLite.ParseString(reader["ロット番号"]).Trim(), typecd = SQLite.ParseString(reader["型番"]).Trim(), STkikaku = SQLite.ParseString(reader["選別規格"]).Trim(), podnum = SQLite.ParseString(reader["ポット番号"]).Trim(), num = SQLite.ParseString(reader["数量"]).Trim() });
                    }

                    //エラーの場合
                    if (retv.Count == 0)
                    {
                        //結果なし
                        MessageBox.Show("STデータがありませんでした.", "データなしエラー");
                    }               

                }
                cmd.Parameters.Clear();
                con.Close();
            }

            ST_Result.Text = "①ロット番号, ②型番, ③選別規格, ④ポット番号, ⑤数量";
            foreach (STitem datalist in retv)
            {
                ST_Result.Text += datalist.lotno + "," + datalist.typecd + "," + datalist.STkikaku + "," + datalist.podnum + "," + datalist.num + "\r\n";
            }

        }
    }
}
