using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace E_fileViewer
{
    public class ErrorData
    {
        public string lotno { get; set; }
        public string machineno { get; set; }
        public string zdled { get; set; }
        public string typecd { get; set; }
        public string errorcode { get; set; }
        public int errorcount { get; set; }
        public DateTime lastupddt { get; set; }

        public static List<ErrorData> _GetErrorDataInfo(string lotno, string machineno, string zdled, string typecd, string errorcode, DateTime datestart, DateTime dateend)
        {
            List<ErrorData> retv = new List<ErrorData>();

            //PMMS接続
            string constr = "server=svopt01;Connect Timeout=30;Application Name=OEMTest;UID=sa;PWD=admin_4121opt;database=OEMTest;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                /*
                cmd.CommandText = "SELECT TOP 1000 lotno, machineno, zdled, typecd, errorcode, errorcount, lastupddt FROM TnDBErrorHistory" +
                    " where lastupddt >= @dtstart and lastupddt <= @dtend order by lotno asc";
                cmd.Parameters.Add(new SqlParameter("@dtstart", datestart));
                cmd.Parameters.Add(new SqlParameter("@dtend", dateend));
                */

                if (errorcode != "")
                {
                    cmd.CommandText = "SELECT TOP 10000 lotno, machineno, zdled, typecd, errorcode, errorcount, lastupddt FROM TnDBErrorHistory where " +
                        "lotno like @lot and machineno like @mn and zdled like @zdled and typecd like @tc and errorcode = @ecode and lastupddt >= @dtstart and lastupddt <= @dtend order by lotno asc, errorcode asc";
                    cmd.Parameters.Add(new SqlParameter("@lot", lotno + "%"));
                    cmd.Parameters.Add(new SqlParameter("@mn", machineno + "%"));
                    cmd.Parameters.Add(new SqlParameter("@zdled", zdled + "%"));
                    cmd.Parameters.Add(new SqlParameter("@tc", "%" + typecd + "%"));
                    cmd.Parameters.Add(new SqlParameter("@ecode", errorcode));
                    cmd.Parameters.Add(new SqlParameter("@dtstart", datestart));
                    cmd.Parameters.Add(new SqlParameter("@dtend", dateend));
                }
                //エラーコードが空白なら
                else
                {
                    cmd.CommandText = "SELECT TOP 10000 lotno, machineno, zdled, typecd, errorcode, errorcount, lastupddt FROM TnDBErrorHistory where " +
                        "lotno like @lot and machineno like @mn and zdled like @zdled and typecd like @tc and errorcode like @ecode and lastupddt >= @dtstart and lastupddt <= @dtend order by lotno asc, errorcode asc";
                    cmd.Parameters.Add(new SqlParameter("@lot", lotno + "%"));
                    cmd.Parameters.Add(new SqlParameter("@mn", machineno + "%"));
                    cmd.Parameters.Add(new SqlParameter("@zdled", zdled + "%"));
                    cmd.Parameters.Add(new SqlParameter("@tc", "%" + typecd + "%"));
                    cmd.Parameters.Add(new SqlParameter("@ecode", errorcode + "%"));
                    cmd.Parameters.Add(new SqlParameter("@dtstart", datestart));
                    cmd.Parameters.Add(new SqlParameter("@dtend", dateend));
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ErrorData EData = new ErrorData();
                        EData.lotno = SQLite.ParseString(reader["lotno"]);
                        EData.machineno = SQLite.ParseString(reader["machineno"]);
                        EData.zdled = SQLite.ParseString(reader["zdled"]);
                        EData.typecd = SQLite.ParseString(reader["typecd"]);
                        EData.errorcode = SQLite.ParseString(reader["errorcode"]);
                        EData.errorcount = SQLite.ParseInt(reader["errorcount"]);
                        EData.lastupddt = SQLite.ParseDate(reader["lastupddt"]) ?? DateTime.MinValue;

                        retv.Add(EData);
                    }
                }
                cmd.Parameters.Clear();
                con.Close();
            }


            return retv;

        }

        public static List<ErrorData> GetErrorDataInfo(List<string> lotno, string machineno, string zdled, string typecd, string errorcode, DateTime datestart, DateTime dateend)
        {
            List<ErrorData> retv = new List<ErrorData>();

            //PMMS接続
            string constr = "server=svopt01;Connect Timeout=30;Application Name=OEMTest;UID=sa;PWD=admin_4121opt;database=OEMTest;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string strlot = "";
                if (lotno.Count >= 2)
                {
                    //ロットが複数ある場合
                    strlot = "";
                    foreach (string lot in lotno)
                    {
                        strlot += lot + " or lotno = ";
                    }
                    strlot = strlot.Remove(strlot.Length - 12);
                    strlot += ")";
                }
                else
                {
                    foreach (string lot in lotno)
                    {
                        strlot = lot;
                    }
                }

                int count = 1;

                //lotnoのリストが2個以上なら複数一致検索
                //個数が1ロット以下ならあいまい検索
                if (errorcode != "")
                {
                    //複数ロットの場合
                    if (lotno.Count >= 2)
                    {
                        cmd.CommandText = "SELECT TOP 10000 lotno, machineno, zdled, typecd, errorcode, errorcount, lastupddt FROM TnDBErrorHistory where " +
                        "machineno like @mn and zdled like @zdled and typecd like @tc and errorcode = @ecode and lastupddt >= @dtstart and lastupddt <= @dtend and (";
                        cmd.Parameters.Add(new SqlParameter("@lot", strlot));
                        cmd.Parameters.Add(new SqlParameter("@mn", machineno + "%"));
                        cmd.Parameters.Add(new SqlParameter("@zdled", zdled + "%"));
                        cmd.Parameters.Add(new SqlParameter("@tc", "%" + typecd + "%"));
                        cmd.Parameters.Add(new SqlParameter("@ecode", errorcode));
                        cmd.Parameters.Add(new SqlParameter("@dtstart", datestart));
                        cmd.Parameters.Add(new SqlParameter("@dtend", dateend));

                        foreach (string lot in lotno)
                        {
                            if (lot != "")
                            {
                                cmd.CommandText += "lotno like @lot" + count.ToString() + " or ";
                                cmd.Parameters.Add(new SqlParameter("@lot" + count.ToString(), lot));///+ "%"));
                                count += 1;
                            }
                        }
                        cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 4);
                        cmd.CommandText += ") order by lotno asc, errorcode asc";
                    }
                    //単ロットの場合
                    else
                    {
                        cmd.CommandText = "SELECT TOP 10000 lotno, machineno, zdled, typecd, errorcode, errorcount, lastupddt FROM TnDBErrorHistory where " +
                        "lotno like @lot and machineno like @mn and zdled like @zdled and typecd like @tc and errorcode = @ecode and lastupddt >= @dtstart and lastupddt <= @dtend order by lotno asc, errorcode asc";
                        cmd.Parameters.Add(new SqlParameter("@lot", strlot + "%"));
                        cmd.Parameters.Add(new SqlParameter("@mn", machineno + "%"));
                        cmd.Parameters.Add(new SqlParameter("@zdled", zdled + "%"));
                        cmd.Parameters.Add(new SqlParameter("@tc", "%" + typecd + "%"));
                        cmd.Parameters.Add(new SqlParameter("@ecode", errorcode));
                        cmd.Parameters.Add(new SqlParameter("@dtstart", datestart));
                        cmd.Parameters.Add(new SqlParameter("@dtend", dateend));
                    }
                }
                //エラーコードが空白なら
                else
                {
                    //複数ロットの場合
                    if (lotno.Count >= 2)
                    {
                        cmd.CommandText = "SELECT TOP 10000 lotno, machineno, zdled, typecd, errorcode, errorcount, lastupddt FROM TnDBErrorHistory where " +
                        "machineno like @mn and zdled like @zdled and typecd like @tc and errorcode like @ecode and lastupddt >= @dtstart and lastupddt <= @dtend and (";
                        //cmd.Parameters.Add(new SqlParameter("@lot", strlot));
                        cmd.Parameters.Add(new SqlParameter("@mn", machineno + "%"));
                        cmd.Parameters.Add(new SqlParameter("@zdled", zdled + "%"));
                        cmd.Parameters.Add(new SqlParameter("@tc", "%" + typecd + "%"));
                        cmd.Parameters.Add(new SqlParameter("@ecode", errorcode + "%"));
                        cmd.Parameters.Add(new SqlParameter("@dtstart", datestart));
                        cmd.Parameters.Add(new SqlParameter("@dtend", dateend));

                        foreach (string lot in lotno)
                        {
                            if (lot != "")
                            {
                                cmd.CommandText += "lotno like @lot" + count.ToString() + " or ";
                                cmd.Parameters.Add(new SqlParameter("@lot" + count.ToString(), lot));// + "%"));
                                count += 1;
                            }
                        }
                        cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 4);
                        cmd.CommandText += ") order by lotno asc, errorcode asc";

                    }
                    //単ロットの場合
                    else
                    {
                        cmd.CommandText = "SELECT TOP 10000 lotno, machineno, zdled, typecd, errorcode, errorcount, lastupddt FROM TnDBErrorHistory where " +
                        "lotno like @lot and machineno like @mn and zdled like @zdled and typecd like @tc and errorcode like @ecode and lastupddt >= @dtstart and lastupddt <= @dtend order by lotno asc, errorcode asc";
                        cmd.Parameters.Add(new SqlParameter("@lot", strlot + "%"));
                        cmd.Parameters.Add(new SqlParameter("@mn", machineno + "%"));
                        cmd.Parameters.Add(new SqlParameter("@zdled", zdled + "%"));
                        cmd.Parameters.Add(new SqlParameter("@tc", "%" + typecd + "%"));
                        cmd.Parameters.Add(new SqlParameter("@ecode", errorcode + "%"));
                        cmd.Parameters.Add(new SqlParameter("@dtstart", datestart));
                        cmd.Parameters.Add(new SqlParameter("@dtend", dateend));
                    }
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ErrorData EData = new ErrorData();
                        EData.lotno = SQLite.ParseString(reader["lotno"]);
                        EData.machineno = SQLite.ParseString(reader["machineno"]);
                        EData.zdled = SQLite.ParseString(reader["zdled"]);
                        EData.typecd = SQLite.ParseString(reader["typecd"]);
                        EData.errorcode = SQLite.ParseString(reader["errorcode"]);
                        EData.errorcount = SQLite.ParseInt(reader["errorcount"]);
                        EData.lastupddt = SQLite.ParseDate(reader["lastupddt"]) ?? DateTime.MinValue;

                        retv.Add(EData);
                    }
                }
                cmd.Parameters.Clear();
                con.Close();
            }

            return retv;

        }
    }
}
