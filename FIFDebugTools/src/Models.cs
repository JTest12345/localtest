using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ArmsApi;
using System.Data;

namespace FIFDebugTools
{
    //
    // ARMS DB
    //
    public class TnLot
    {
        public string lotno { get; set; }
        public string typecd { get; set; }
        public string warningfg { get; set; } = "0";
        public string restrictfg { get; set; } = "0";
        public string profileid { get; set; } = "60000";
        public string mnfctplanno { get; set; } = null;
        public string blendcd { get; set; } = "BC";
        public string resingpcd { get; set; } = "RC";
        public string cutblendcd { get; set; } = "CC";
        public string lifetestfg { get; set; } = "0";
        public string khltestfg { get; set; } = "0";
        public string colortestfg { get; set; } = "0";
        public string templotno { get; set; }
        public string istemp { get; set; } = "0";
        public string isnascalotcharend { get; set; } = "0";
        public string lastupddt { get; set; }
        public string lotsize { get; set; } = null;
        public string isfullsizeinspection { get; set; } = null;
        public string ismappinginspection { get; set; } = null;
        public string ischangepointlot { get; set; } = null;
        public string macgroup { get; set; } = "2";
        public string isbadmarkframe { get; set; } = "0";
        public string dbthrowdt { get; set; }
        public string reflowtestfg { get; set; } = "0";
        public string reflowtestwirebondfg { get; set; } = "0";
        public string elasticitytestfg { get; set; } = "0";
        public string diesheartestfg { get; set; } = "0";
        public string movestockct { get; set; } = "0";
        public string beforelifetestcondcd { get; set; } = "";
        public string tempcutblendid { get; set; } = "0";
        public string tempcutblendno { get; set; } = "";
        public string mnggrid { get; set; } = null;
        public string mnggrnm { get; set; } = null;
        public string limitsheartestfg { get; set; } = "0";
        public string resingpcd2 { get; set; } = "";

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnlot.csv";

        public bool insertTnlot(ref string msg)
        {
            DateTime dt = DateTime.Now;
            lastupddt = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO[dbo].[TnLot]" +
            "([lotno]" +
            ",[typecd]" +
            ",[warningfg]" +
            ",[restrictfg]" +
            ",[profileid]" +
            ",[mnfctplanno]" +
            ",[blendcd]" +
            ",[resingpcd]" +
            ",[cutblendcd]" +
            ",[lifetestfg]" +
            ",[khltestfg]" +
            ",[colortestfg]" +
            ",[templotno]" +
            ",[istemp]" +
            ",[isnascalotcharend]" +
            ",[lastupddt]" +
            ",[lotsize]" +
            ",[isfullsizeinspection]" +
            ",[ismappinginspection]" +
            ",[ischangepointlot]" +
            ",[macgroup]" +
            ",[isbadmarkframe]" +
            ",[dbthrowdt]" +
            ",[reflowtestfg]" +
            ",[reflowtestwirebondfg]" +
            ",[elasticitytestfg]" +
            ",[diesheartestfg]" +
            ",[movestockct]" +
            ",[beforelifetestcondcd]" +
            ",[tempcutblendid]" +
            ",[tempcutblendno]" +
            ",[mnggrid]" +
            ",[mnggrnm]" +
            ",[limitsheartestfg]" +
            ",[resingpcd2])" +
         " VALUES " +
            $"('{lotno}'" +
            $",'{typecd}'" +
            $",'{warningfg}'" +
            $",'{restrictfg}'" +
            $",'{profileid}'" +
            $",'{mnfctplanno}'" +
            $",'{blendcd}'" +
            $",'{resingpcd}'" +
            $",'{cutblendcd}'" +
            $",'{lifetestfg}'" +
            $",'{khltestfg}'" +
            $",'{colortestfg}'" +
            $",'{templotno}'" +
            $",'{istemp}'" +
            $",'{isnascalotcharend}'" +
            $",'{lastupddt}'" +
            $",'{lotsize}'" +
            $",'{isfullsizeinspection}'" +
            $",'{ismappinginspection}'" +
            $",'{ischangepointlot}'" +
            $",'{macgroup}'" +
            $",'{isbadmarkframe}'" +
            $",'{dbthrowdt}'" +
            $",'{reflowtestfg}'" +
            $",'{reflowtestwirebondfg}'" +
            $",'{elasticitytestfg}'" +
            $",'{diesheartestfg}'" +
            $",'{movestockct}'" +
            $",'{beforelifetestcondcd}'" +
            $",'{tempcutblendid}'" +
            $",'{tempcutblendno}'" +
            $",'{mnggrid}'" +
            $",'{mnggrnm}'" +
            $",'{limitsheartestfg}'" +
            $",'{resingpcd2}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);

        }

        public bool DeleteTnlot(string key, string value, ref string msg)
        {
            var SqlStrings = $"DELETE FROM [dbo].[TnLot] " +
                            $"WHERE {key} like '{value}'";
            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [lotno]" +
                            ",[typecd]" +
                            ",[warningfg]" +
                            ",[restrictfg]" +
                            ",[profileid]" +
                            ",[mnfctplanno]" +
                            ",[blendcd]" +
                            ",[resingpcd]" +
                            ",[cutblendcd]" +
                            ",[lifetestfg]" +
                            ",[khltestfg]" +
                            ",[colortestfg]" +
                            ",[templotno]" +
                            ",[istemp]" +
                            ",[isnascalotcharend]" +
                            ",[lastupddt]" +
                            ",[lotsize]" +
                            ",[isfullsizeinspection]" +
                            ",[ismappinginspection]" +
                            ",[ischangepointlot]" +
                            ",[macgroup]" +
                            ",[isbadmarkframe]" +
                            ",[dbthrowdt]" +
                            ",[reflowtestfg]" +
                            ",[reflowtestwirebondfg]" +
                            ",[elasticitytestfg]" +
                            ",[diesheartestfg]" +
                            ",[movestockct]" +
                            ",[beforelifetestcondcd]" +
                            ",[tempcutblendid]" +
                            ",[tempcutblendno]" +
                            ",[mnggrid]" +
                            ",[mnggrnm]" +
                            ",[limitsheartestfg]" +
                            ",[resingpcd2] " +
                            "FROM[ARMS].[dbo].[TnLot]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["lotno"]+
                            "," + reader["typecd"]+
                            "," + reader["warningfg"]+
                            "," + reader["restrictfg"]+
                            "," + reader["profileid"]+
                            "," + reader["mnfctplanno"]+
                            "," + reader["blendcd"]+
                            "," + reader["resingpcd"]+
                            "," + reader["cutblendcd"]+
                            "," + reader["lifetestfg"]+
                            "," + reader["khltestfg"]+
                            "," + reader["colortestfg"]+
                            "," + reader["templotno"]+
                            "," + reader["istemp"]+
                            "," + reader["isnascalotcharend"]+
                            "," + reader["lastupddt"]+
                            "," + reader["lotsize"]+
                            "," + reader["isfullsizeinspection"]+
                            "," + reader["ismappinginspection"]+
                            "," + reader["ischangepointlot"]+
                            "," + reader["macgroup"]+
                            "," + reader["isbadmarkframe"]+
                            "," + reader["dbthrowdt"]+
                            "," + reader["reflowtestfg"]+
                            "," + reader["reflowtestwirebondfg"]+
                            "," + reader["elasticitytestfg"]+
                            "," + reader["diesheartestfg"]+
                            "," + reader["movestockct"]+
                            "," + reader["beforelifetestcondcd"]+
                            "," + reader["tempcutblendid"]+
                            "," + reader["tempcutblendno"]+
                            "," + reader["mnggrid"]+
                            "," + reader["mnggrnm"]+
                            "," + reader["limitsheartestfg"]+
                            "," + reader["resingpcd2"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }
    public class TnMag
    {
        public string lotno { get; set; }
        public string magno { get; set; }
        public string inlineprocno { get; set; }
        public string newfg { get; set; }
        public string lastupddt { get; set; }

        public bool InsertTnMag(ref string msg)
        {
            DateTime dt = DateTime.Now;
            lastupddt = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO[dbo].[TnMag]" +
                    "([lotno]" +
                    ",[magno]" +
                    ",[inlineprocno]" +
                    ",[newfg]" +
                    ",[lastupddt])" +
                    " VALUES " +
                    $"('{lotno}'" +
                    $",'{magno}'" +
                    $",'{inlineprocno}'" +
                    $",'{newfg}'" +
                    $",'{lastupddt}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }

        public bool UpdateTnMag(string lotno, string magno, string procno, ref string msg)
        {
            var SqlStrings = $"UPDATE [dbo].[TnMag] " +
                            $"SET [inlineprocno] = '{procno}', [newfg] = 1 " +
                            $"WHERE lotno = '{lotno}' and magno = '{magno}'";
            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }

        public bool DeleteTnMag(string key, string value, ref string msg)
        {
            var SqlStrings = $"DELETE FROM [dbo].[TnMag] " +
                            $"WHERE {key} like '{value}'";
            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }
    }

    public class TnTran
    {
        public string lotno { get; set; }
        public string procno { get; set; }
        public string macno { get; set; }
        public string inmag { get; set; }
        public string outmag { get; set; }
        public string startdt { get; set; }
        public string enddt { get; set; }
        public string iscomplt { get; set; }
        public string stocker1 { get; set; }
        public string stocker2 { get; set; }
        public string comment { get; set; }
        public string transtartempcd { get; set; }
        public string trancompempcd { get; set; }
        public string inspectempcd { get; set; }
        public string inspectct { get; set; }
        public string isnascastart { get; set; }
        public string isnascaend { get; set; }
        public string lastupddt { get; set; }
        public string isnascadefectend { get; set; }
        public string isnascacommentend { get; set; }
        public string delfg { get; set; }
        public string isdefectend { get; set; }
        public string isdefectautoimportend { get; set; }
        public string isnascarunning { get; set; }
        public string isautoimport { get; set; }
        public string isresinmixordered { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tntran.csv";

        public bool InsertTnTran(ref string msg)
        {
            DateTime dt = DateTime.Now;
            lastupddt = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO [dbo].[TnTran]" +
                            "([lotno]" +
                            ",[procno]" +
                            ",[macno]" +
                            ",[inmag]" +
                            ",[outmag]" +
                            ",[startdt]" +
                            ",[enddt]" +
                            ",[iscomplt]" +
                            ",[stocker1]" +
                            ",[stocker2]" +
                            ",[comment]" +
                            ",[transtartempcd]" +
                            ",[trancompempcd]" +
                            ",[inspectempcd]" +
                            ",[inspectct]" +
                            ",[isnascastart]" +
                            ",[isnascaend]" +
                            ",[lastupddt]" +
                            ",[isnascadefectend]" +
                            ",[isnascacommentend]" +
                            ",[delfg]" +
                            ",[isdefectend]" +
                            ",[isdefectautoimportend]" +
                            ",[isnascarunning]" +
                            ",[isautoimport]" +
                            ",[isresinmixordered])" +
                        " VALUES " +
                            $"('{lotno}'" +
                            $",'{procno}'" +
                            $",'{macno}'" +
                            $",'{inmag}'" +
                            $",'{outmag}'" +
                            $",'{startdt}'" +
                            $",'{enddt}'" +
                            $",'{iscomplt}'" +
                            $",'{stocker1}'" +
                            $",'{stocker2}'" +
                            $",'{comment}'" +
                            $",'{transtartempcd}'" +
                            $",'{trancompempcd}'" +
                            $",'{inspectempcd}'" +
                            $",'{inspectct}'" +
                            $",'{isnascastart}'" +
                            $",'{isnascaend}'" +
                            $",'{lastupddt}'" +
                            $",'{isnascadefectend}'" +
                            $",'{isnascacommentend}'" +
                            $",'{delfg}'" +
                            $",'{isdefectend}'" +
                            $",'{isdefectautoimportend}'" +
                            $",'{isnascarunning}'" +
                            $",'{isautoimport}'" +
                            $",'{isresinmixordered}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }

        public bool DeleteTnTran(string key, string value, ref string msg)
        {
            var SqlStrings = $"DELETE FROM [dbo].[TnTran] " +
                            $"WHERE {key} like '{value}'";
            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [lotno] " +
                              ",[procno] " +
                              ",[macno] " +
                              ",[inmag] " +
                              ",[outmag] " +
                              ",[startdt] " +
                              ",[enddt] " +
                              ",[iscomplt] " +
                              ",[stocker1] " +
                              ",[stocker2] " +
                              ",[comment] " +
                              ",[transtartempcd] " +
                              ",[trancompempcd] " +
                              ",[inspectempcd] " +
                              ",[inspectct] " +
                              ",[isnascastart] " +
                              ",[isnascaend] " +
                              ",[lastupddt] " +
                              ",[isnascadefectend] " +
                              ",[isnascacommentend] " +
                              ",[delfg] " +
                              ",[isdefectend] " +
                              ",[isdefectautoimportend] " +
                              ",[isnascarunning] " +
                              ",[isautoimport] " +
                              ",[isresinmixordered] " +
                              "FROM[ARMS].[dbo].[TnTran]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["lotno"] +
                              "," + reader["procno"] +
                              "," + reader["macno"] +
                              "," + reader["inmag"] +
                              "," + reader["outmag"] +
                              "," + reader["startdt"] +
                              "," + reader["enddt"] +
                              "," + reader["iscomplt"] +
                              "," + reader["stocker1"] +
                              "," + reader["stocker2"] +
                              "," + reader["comment"] +
                              "," + reader["transtartempcd"] +
                              "," + reader["trancompempcd"] +
                              "," + reader["inspectempcd"] +
                              "," + reader["inspectct"] +
                              "," + reader["isnascastart"] +
                              "," + reader["isnascaend"] +
                              "," + reader["lastupddt"] +
                              "," + reader["isnascadefectend"] +
                              "," + reader["isnascacommentend"] +
                              "," + reader["delfg"] +
                              "," + reader["isdefectend"] +
                              "," + reader["isdefectautoimportend"] +
                              "," + reader["isnascarunning"] +
                              "," + reader["isautoimport"] +
                              "," + reader["isresinmixordered"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }
    public class TnVirtualMag
    {
        public string macno { get; set; }
        public string locationid { get; set; }
        public string orderid { get; set; }
        public string magno { get; set; }
        public string lastmagno { get; set; }
        public string procno { get; set; }
        public string workstartdt { get; set; }
        public string workenddt { get; set; }
        public string nextmachines { get; set; }
        public string relatedmaterials { get; set; }
        public string framematerialcd { get; set; }
        public string framelotno { get; set; }
        public string framecurrentct { get; set; }
        public string framemaxct { get; set; }
        public string stockerstartno { get; set; }
        public string stockerendno { get; set; }
        public string stockerchangect { get; set; }
        public string stockerlastupddt { get; set; }
        public string stocker1no { get; set; }
        public string stocker2no { get; set; }
        public string waferstartno { get; set; }
        public string waferendno { get; set; }
        public string waferchangect { get; set; }
        public string waferlastupddt { get; set; }
        public string mapaoimaglotno { get; set; }
        public string purgereason { get; set; }
        public string originno { get; set; }
        public string programtotalminutes { get; set; }
        public string lastupddt { get; set; }
        public string priorityfg { get; set; }
        public string originlocationid { get; set; }

        public bool InsertVirtualMag(ref string msg)
        {
            DateTime dt = DateTime.Now;
            lastupddt = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO [dbo].[TnVirtualMag] " +
                           "([macno] " +
                           ",[locationid] " +
                           ",[orderid] " +
                           ",[magno] " +
                           ",[lastmagno] " +
                           ",[procno] " +
                           ",[workstartdt] " +
                           ",[workenddt] " +
                           ",[nextmachines] " +
                           ",[relatedmaterials] " +
                           ",[framematerialcd] " +
                           ",[framelotno] " +
                           ",[framecurrentct] " +
                           ",[framemaxct] " +
                           ",[stockerstartno] " +
                           ",[stockerendno] " +
                           ",[stockerchangect] " +
                           ",[stockerlastupddt] " +
                           ",[stocker1no] " +
                           ",[stocker2no] " +
                           ",[waferstartno] " +
                           ",[waferendno] " +
                           ",[waferchangect] " +
                           ",[waferlastupddt] " +
                           ",[mapaoimaglotno] " +
                           ",[purgereason] " +
                           ",[originno] " +
                           ",[programtotalminutes] " +
                           ",[lastupddt] " +
                           ",[priorityfg] " +
                           ",[originlocationid]) " +
                     " VALUES  " +
                           $"('{macno}' " +
                           $",'{locationid}' " +
                           $",'{orderid}' " +
                           $",'{magno}'" +
                           $",'{lastmagno}' " +
                           $",'{procno}'" +
                           $",'{workstartdt}' " +
                           $",'{workenddt}' " +
                           $",'{nextmachines}' " +
                           $",'{relatedmaterials}' " +
                           $",'{framematerialcd}'" +
                           $",'{framelotno}'" +
                           $",'{framecurrentct}'" +
                           $",'{framemaxct}'" +
                           $",'{stockerstartno}'" +
                           $",'{stockerendno}'" +
                           $",'{stockerchangect}' " +
                           $",'{stockerlastupddt}' " +
                           $",'{stocker1no}'" +
                           $",'{stocker2no}' " +
                           $",'{waferstartno}' " +
                           $",'{waferendno}'" +
                           $",'{waferchangect}' " +
                           $",'{waferlastupddt}' " +
                           $",'{mapaoimaglotno}'" +
                           $",'{purgereason}'" +
                           $",'{originno}'" +
                           $",'{programtotalminutes}' " +
                           $",'{lastupddt}'" +
                           $",'{priorityfg}' " +
                           $",'{originlocationid}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }

        public bool DeleteTnVirtualMag(string key, string value, ref string msg)
        {
            var SqlStrings = $"DELETE FROM [dbo].[TnVirtualMag] " +
                            $"WHERE {key} like '{value}'";
            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }
    }

    public class TnLotLog
    {
        public string lotno { get; set; }
        public string indt { get; set; }
        public string msg { get; set; }
        public string magno { get; set; }
        public string errorfg { get; set; }
        public string line { get; set; }
        public string updusercd { get; set; }
        public string plantcd { get; set; }
        public string moniterviewfg { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnlotlog.csv";

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [lotno] " +
                              ",[indt] " +
                              ",[msg] " +
                              ",[magno] " +
                              ",[errorfg] " +
                              ",[line] " +
                              ",[updusercd] " +
                              ",[plantcd] " +
                              ",[moniterviewfg] " +
                              "FROM[ARMS].[dbo].[TnLotLog]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["lotno"] +
                              "," + reader["indt"] +
                              "," + reader["msg"] +
                              "," + reader["magno"] +
                              "," + reader["errorfg"] +
                              "," + reader["line"] +
                              "," + reader["updusercd"] +
                              "," + reader["plantcd"] +
                              "," + reader["moniterviewfg"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }

    public class TnRestrict
    {
        public string lotno { get; set; }
        public string procno { get; set; }
        public string reason { get; set; }
        public string delfg { get; set; }
        public string lastupddt { get; set; }
        public string reasonkb { get; set; }
        public string updusercd { get; set; }
        public string restrictreleasefg { get; set; }
        public string representativelotno { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnrestrict.csv";

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [lotno] " +
                              ",[procno] " +
                              ",[reason] " +
                              ",[delfg] " +
                              ",[lastupddt] " +
                              ",[reasonkb] " +
                              ",[updusercd] " +
                              ",[restrictreleasefg] " +
                              ",[representativelotno] " +
                              "FROM[ARMS].[dbo].[TnRestrict]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["lotno"] +
                              "," + reader["procno"] +
                              "," + reader["reason"] +
                              "," + reader["delfg"] +
                              "," + reader["lastupddt"] +
                              "," + reader["reasonkb"] +
                              "," + reader["updusercd"] +
                              "," + reader["restrictreleasefg"] +
                              "," + reader["representativelotno"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }
    public class TnInspection
    {
        public string lotno { get; set; }
        public string procno { get; set; } = "10";
        public string sgyfg { get; set; } = "0";
        public string lastupddt { get; set; }

        public bool InsertTnMag(ref string msg)
        {
            DateTime dt = DateTime.Now;
            lastupddt = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO[dbo].[TnInspection]" +
                    "([lotno]" +
                    ",[procno]" +
                    ",[sgyfg]" +
                    ",[lastupddt])" +
                    " VALUES " +
                    $"('{lotno}'" +
                    $",'{procno}'" +
                    $",'{sgyfg}'" +
                    $",'{lastupddt}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }
    }

    //
    // LENS DB
    //
    public class TnLot_Lens
    {
        public string LotNO { get; set; }
        public string TypeCD { get; set; }
        public string IsFullsizeInspection { get; set; } = "0";
        public string IsMappingInspection { get; set; } = "1";
        public string IsChangePoint { get; set; } = "0";
        public string LastupdDT { get; set; }
        public string InspectionResultCD { get; set; } = "0";

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnlot_lens.csv";

        public bool InsertTnlot(ref string msg)
        {
            DateTime dt = DateTime.Now;
            LastupdDT = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO [dbo].[TnLot] " +
                           "([LotNO] " +
                           ",[TypeCD] " +
                           ",[IsFullsizeInspection] " +
                           ",[IsMappingInspection] " +
                           ",[IsChangePoint] " +
                           ",[LastupdDT] " +
                           ",[InspectionResultCD]) " +
                        " VALUES " +
                           $"('{LotNO}' " +
                           $",'{TypeCD}' " +
                           $",'{IsFullsizeInspection}' " +
                           $",'{IsMappingInspection}' " +
                           $",'{IsChangePoint}' " +
                           $",'{LastupdDT}'" +
                           $",'{InspectionResultCD}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand_Lens(SqlStrings, ref msg);
        }

        public bool DeletTnLot_Lens(string key, string value, ref string msg)
        {
            var SqlStrings = $"DELETE FROM [LENS].[dbo].[TnLot] " +
                            $"WHERE {key} like '{value}'";
            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [lotno] " +
                              ",[TypeCD] " +
                              ",[IsFullsizeInspection] " +
                              ",[IsMappingInspection] " +
                              ",[IsChangePoint] " +
                              ",[LastupdDT] " +
                              ",[InspectionResultCD] " +
                              "FROM[LENS].[dbo].[TnLot]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LENSConSTR))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["lotno"] +
                              "," + reader["TypeCD"] +
                              "," + reader["IsFullsizeInspection"] +
                              "," + reader["IsMappingInspection"] +
                              "," + reader["IsChangePoint"] +
                              "," + reader["LastupdDT"] +
                              "," + reader["InspectionResultCD"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }
    public class TnTran_Lens
    {
        public string LotNO { get; set; }
        public string ProcNO { get; set; }
        public string PlantCD { get; set; }
        public string StartDT { get; set; }
        public string EndDT { get; set; }
        public string IsCompleted { get; set; }
        public string DelFG { get; set; } = "0";
        public string LastUpdDT { get; set; }
        public string CarrierNO { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tntran_lens.csv";

        public bool InsertTnTran(ref string msg)
        {
            DateTime dt = DateTime.Now;
            LastUpdDT = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO [dbo].[TnTran]" +
                          " ([LotNO]" +
                          " ,[ProcNO]" +
                          " ,[PlantCD]" +
                          " ,[StartDT]" +
                          " ,[EndDT]" +
                          " ,[IsCompleted]" +
                          " ,[DelFG]" +
                          " ,[LastUpdDT]" +
                          " ,[CarrierNO])" +
                          " VALUES" +
                          $" ('{LotNO}'" +
                          $" ,'{ProcNO}'" +
                          $" ,'{PlantCD}'" +
                          $" ,'{StartDT}'" +
                          $" ,'{EndDT}'" +
                          $" ,'{IsCompleted}'" +
                          $" ,'{DelFG}'" +
                          $" ,'{LastUpdDT}'" +
                          $" ,'{CarrierNO}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand_Lens(SqlStrings, ref msg);
        }
        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [LotNO] " +
                              ",[ProcNO] " +
                              ",[PlantCD] " +
                              ",[StartDT] " +
                              ",[EndDT] " +
                              ",[IsCompleted] " +
                              ",[DelFG] " +
                              ",[LastUpdDT] " +
                              ",[CarrierNO] " +
                              "FROM[LENS].[dbo].[TnTran]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LENSConSTR))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["LotNO"] +
                              "," + reader["ProcNO"] +
                              "," + reader["PlantCD"] +
                              "," + reader["StartDT"] +
                              "," + reader["EndDT"] +
                              "," + reader["IsCompleted"] +
                              "," + reader["DelFG"] +
                              "," + reader["LastUpdDT"] +
                              "," + reader["CarrierNO"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }

    //
    // QCIL DB
    //
    public class TnLog_Qcil
    {
        public string Inline_CD { get; set; }
        public string Equipment_NO { get; set; }
        public string Measure_DT { get; set; }
        public string Seq_NO { get; set; }
        public string QcParam_NO { get; set; }
        public string Material_CD { get; set; }
        public string Magazine_NO { get; set; }
        public string NascaLot_NO { get; set; }
        public string DParameter_VAL { get; set; }
        public string SParameter_VAL { get; set; }
        public string Message_NM { get; set; }
        public string Error_FG { get; set; }
        public string Check_FG { get; set; }
        public string Del_FG { get; set; }
        public string UpdUser_CD { get; set; }
        public string LastUpd_DT { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnlog_qcil.csv";

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [Inline_CD] " +
                              ",[Equipment_NO] " +
                              ",[Measure_DT] " +
                              ",[Seq_NO] " +
                              ",[QcParam_NO] " +
                              ",[Material_CD] " +
                              ",[Magazine_NO] " +
                              ",[NascaLot_NO] " +
                              ",[DParameter_VAL] " +
                              ",[SParameter_VAL] " +
                              ",[Message_NM] " +
                              ",[Error_FG] " +
                              ",[Check_FG] " +
                              ",[Del_FG] " +
                              ",[UpdUser_CD] " +
                              ",[LastUpd_DT] " +
                              "FROM[QCIL].[dbo].[TnLOG]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.QCILConSTR))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["Inline_CD"] +
                              "," + reader["Equipment_NO"] +
                              "," + reader["Measure_DT"] +
                              "," + reader["Seq_NO"] +
                              "," + reader["QcParam_NO"] +
                              "," + reader["Material_CD"] +
                              "," + reader["Magazine_NO"] +
                              "," + reader["NascaLot_NO"] +
                              "," + reader["DParameter_VAL"] +
                              "," + reader["SParameter_VAL"] +
                              "," + reader["Message_NM"] +
                              "," + reader["Error_FG"] +
                              "," + reader["Check_FG"] +
                              "," + reader["Del_FG"] +
                              "," + reader["UpdUser_CD"] +
                              "," + reader["LastUpd_DT"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }

    public class TnLott_Qcil
    {
        public string Inline_CD { get; set; }
        public string NascaLot_NO { get; set; }
        public string Equipment_NO { get; set; }
        public string Measure_DT { get; set; }
        public string Assets_NM { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnlott_qcil.csv";

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [Inline_CD] " +
                                ",[NascaLot_NO] " +
                                ",[Equipment_NO] " +
                                ",[Measure_DT] " +
                                ",[Assets_NM] " +
                                "FROM[QCIL].[dbo].[TnLOTT]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.QCILConSTR))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["Inline_CD"] +
                                "," + reader["NascaLot_NO"] +
                                "," + reader["Equipment_NO"] +
                                "," + reader["Measure_DT"] +
                                "," + reader["Assets_NM"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }

    }

    public class TnQcnr_Qcil
    {
        public string QCNR_NO { get; set; }
        public string Inline_CD { get; set; }
        public string Equipment_NO { get; set; }
        public string Defect_NO { get; set; }
        public string Inspection_NO { get; set; }
        public string Multi_NO { get; set; }
        public string NascaLot_NO { get; set; }
        public string Measure_DT { get; set; }
        public string Message { get; set; }
        public string Type_CD { get; set; }
        public string BackNum_NO { get; set; }
        public string Check_NO { get; set; }
        public string UpdUser_CD { get; set; }
        public string LastUpd_DT { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnqcnr_qcil.csv";

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [QCNR_NO] " +
                                ",[Inline_CD] " +
                                ",[Equipment_NO] " +
                                ",[Defect_NO] " +
                                ",[Inspection_NO] " +
                                ",[Multi_NO] " +
                                ",[NascaLot_NO] " +
                                ",[Measure_DT] " +
                                ",[Message] " +
                                ",[Type_CD] " +
                                ",[BackNum_NO] " +
                                ",[Check_NO] " +
                                ",[UpdUser_CD] " +
                                ",[LastUpd_DT] " +
                                "FROM[QCIL].[dbo].[TnLOG]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.QCILConSTR))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["QCNR_NO"] +
                                "," + reader["Inline_CD"] +
                                "," + reader["Equipment_NO"] +
                                "," + reader["Defect_NO"] +
                                "," + reader["Inspection_NO"] +
                                "," + reader["Multi_NO"] +
                                "," + reader["NascaLot_NO"] +
                                "," + reader["Measure_DT"] +
                                "," + reader["Message"] +
                                "," + reader["Type_CD"] +
                                "," + reader["BackNum_NO"] +
                                "," + reader["Check_NO"] +
                                "," + reader["UpdUser_CD"] +
                                "," + reader["LastUpd_DT"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }

    }


    //
    // 共通DBクラス
    //
    public class DbControls
    {
        public bool execSqlCommand(string SqlStrings, ref string msg)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = SqlStrings;
                    con.Open();
                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();
                    con.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        public bool execSqlCommand_Lens(string SqlStrings, ref string msg)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = SqlStrings;
                    con.Open();
                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();
                    con.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        //
        /* テキストファイル一行づつ読み込み関数
         * OK: true
         * NG: false
         * 読込内容(配列)：contents 
         */
        //
        public bool ReadTextFileLine(string filepath, ref List<string> contents, string enccode = "UTF-8")
        {
            try
            {
                StreamReader sr = new StreamReader(filepath, Encoding.GetEncoding(enccode));
                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();
                    line = new string(line.Where(c => !char.IsControl(c)).ToArray());
                    contents.Add(line);
                }
                sr.Close();

                contents.RemoveAt(0);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    
}
