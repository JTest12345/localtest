using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SpreadsheetLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Oskas;
using ExcelDataReader;


namespace ProcMasterIF
{

    class ArmsSqlIF
    {
        string workingdir = @"C:\Oskas\procmaster\shomei\ver9";
        string armsworkingdir = @"C:\Oskas\procmaster\shomei\ver9\model\root\arms";
        string armssqldir = @"C:\Oskas\procmaster\shomei\ver9\sql\arms";
        ArmsDbConfig armsconf;
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
        string msg;
        string crlf = "\r\n";

        public ArmsSqlIF()
        {
            // armsconfigfile
            var armsyamlPath = armsworkingdir + @"\config\armsconfig.yaml";
            var armsconfig_yaml = new StreamReader(armsyamlPath, Encoding.UTF8);
            var armsdeserializer = new DeserializerBuilder()
                               .WithNamingConvention(CamelCaseNamingConvention.Instance)
                               .Build();
            armsconf = armsdeserializer.Deserialize<ArmsDbConfig>(armsconfig_yaml);

            // 接続文字列を構築
            builder.DataSource = armsconf.datasource;   // 接続先の SQL Server インスタンス
            builder.UserID = armsconf.userid;              // 接続ユーザー名
            builder.Password = armsconf.password; // 接続パスワード
            builder.InitialCatalog = armsconf.initialcatalog;  // 接続するデータベース(ここは変えないでください)
        }

        public bool Excute()
        {
            var pm = new MakeProcMst();
            if (!pm.Excute())
            {
                OskNLog.Log("新機種展開表／素子波長一覧の読込が失敗しました", Cnslcnf.msg_info);
                return false;
            }

            //*************************************************************************************************************
            // ◆形状シリーズ毎の処理
            // 新機種展開表／素子波長指定一覧をExcelDataReaderで読込
            //*************************************************************************************************************
            foreach (var sr in pm.srList)
            {
                var orthankanfld = workingdir + @"\model\sources\" + sr.model.folder + @"\" + sr.model.hankan;
                var ortkanseifld = workingdir + @"\model\sources\" + sr.model.folder + @"\" + sr.model.kansei;

                ///////////////////////////////////////////
                /// シリーズフォルダ処理
                /// 形状シリーズ毎に半完／完成の
                /// ①工程定義
                /// ②ルートの情報に対するオーバーライド
                /// 
                /// 形状シリーズ毎にオブジェクト辞書化
                /// key: 製品名
                /// 半完：typecPrObjDict_hankan
                /// 完成：typecPrObjDict_kansei
                ///////////////////////////////////////////

                var typecPrObjDict_hankan = new Dictionary<string, PROC_OBJECTS>();
                if (!pm.makeProcObjectsAndJson(orthankanfld, sr, false, ref typecPrObjDict_hankan))
                {
                    OskNLog.Log("半完成品を処理中に問題が発生しました", Cnslcnf.msg_info);
                    return false;
                }

                var typecPrObjDict_kansei = new Dictionary<string, PROC_OBJECTS>();
                if (!pm.makeProcObjectsAndJson(ortkanseifld, sr, true, ref typecPrObjDict_kansei))
                {
                    OskNLog.Log("完成品を処理中に問題が発生しました", Cnslcnf.msg_info);
                    return false;
                }

                // Memo 2021.11.19
                // 純規が指摘した部材ライフマスタのFコードは機種ごとに変えられるのか問題について
                //
                // 現状はシリーズ毎にJsonを上記のように作っているので下記のSQLに部品表の情報(ペーストのFコードなど)入れるには
                // 上記に組み込むか、上記で出力したprocessjsonを読みこむ（こっちのがいいかな、、、）
                // 部材の場合は/process[]⇒工程名で検索⇒material/m4の品目名で検索するのがよいと思う
                // なので検索のキーは[工程名][品目名]がよいかと。
                // 品目名でユニークにならない場合はどうしようもないのでFコードをベタ打ちするしかない
                // [結論]、、、あまり頑張っても仕方なさそうなのでこのままExcelにベタ打ちがよいように思う
                //

                if (!MakeSQLFiles(typecPrObjDict_hankan, false))
                {
                    OskNLog.Log("半完成品用SQLファイル出力中に問題が発生しました", Cnslcnf.msg_info);
                    return false;
                }

                if (!MakeSQLFiles(typecPrObjDict_kansei, true))
                {
                    OskNLog.Log("完成品用SQLファイル出力中に問題が発生しました", Cnslcnf.msg_info);
                    return false;
                }

            }
            return true;
        }

        public bool MakeSQLFiles(Dictionary<string, PROC_OBJECTS> typecPrObjDict, bool isThisKansei)
        {
            foreach (var PrObj in typecPrObjDict)
            {
                //*************************************************************************************************************
                // ◆ バックアップSQL作成
                // 各テーブルに対象機種が存在した場合、各テーブルを復元する為のSQLを作成する
                //*************************************************************************************************************
                var typecd = PrObj.Key;
                var sql = "SET XACT_ABORT ON;" + crlf + "BEGIN TRANSACTION;" + crlf;
                var sqlret_all = string.Empty;

                foreach (var tblnm in armsconf.table)
                {
                    if (!ArmsTableBuckUp(new List<string> { typecd }, tblnm, out string sqlret))
                    {
                        OskNLog.Log("ARMSテーブルのSQLバックアップ作成中に問題が発生しました", Cnslcnf.msg_error);
                        return false;
                    }
                    sqlret_all += sqlret;
                }

                if (!string.IsNullOrEmpty(sqlret_all))
                {
                    sql += sqlret_all + "COMMIT TRANSACTION;";

                    var FilePath = armssqldir + $@"\bu\hankan\{typecd}_bu.sql";
                    if (isThisKansei)
                    {
                        FilePath = armssqldir + $@"\bu\kansei\{typecd}_bu.sql";
                    }
                    using (FileStream fs = File.Create(FilePath))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(sql);
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }

                //*************************************************************************************************************
                // ◆ インサート用SQL作成
                //*************************************************************************************************************
                sql = "SET XACT_ABORT ON;" + crlf + "BEGIN TRANSACTION;" + crlf;
                sqlret_all = string.Empty;
                foreach (var tblnm in armsconf.table)
                {
                    if (!InsertTypeCdKeyRecords(typecd, PrObj.Value.ProcObj.procmastermodel.amstbls[tblnm], tblnm, out string sqlret))
                    {
                        OskNLog.Log("ARMSテーブルのインサート用SQL作成中に問題が発生しました", Cnslcnf.msg_error);
                        return false;
                    }
                    sqlret_all += sqlret;
                }

                if (!string.IsNullOrEmpty(sqlret_all))
                {
                    sql += sqlret_all + "COMMIT TRANSACTION;";

                    var FilePath = armssqldir + $@"\ins\hankan\{typecd}_ins.sql";
                    if (isThisKansei)
                    {
                        FilePath = armssqldir + $@"\ins\kansei\{typecd}_ins.sql";
                    }
                    using (FileStream fs = File.Create(FilePath))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(sql);
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }
            }
            return true;
        }

        public bool InsertTypeCdKeyRecords(string typecd, System.Data.DataTable amstbl, string tblname, out string sqlret)
        {
            sqlret = string.Empty;
            var insertsql = string.Empty;
            var inssqlheader = string.Empty;
            var colmnames = string.Empty;

            try
            {
                // yamlを読込
                var yamlPath = armsworkingdir + $@"\config\{tblname}.yaml";
                var tbl_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                var deserializer = new DeserializerBuilder()
                                   .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                   .Build();
                var tblCols = deserializer.Deserialize<List<string>>(tbl_yaml);
                var dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                foreach (var item in amstbl.Rows[0].ItemArray)
                {
                    if (!tblCols.Contains(item))
                    {
                        //Console.WriteLine("Columnが不足しています：テーブル " + tblname + ":カラム " + item);
                        throw new Exception("Columnが不足しています：テーブル " + tblname + ":カラム " + item);
                    }
                    colmnames += "[" + item + "],";
                }

                inssqlheader = $"INSERT INTO {tblname}" + crlf + "(" + colmnames.Substring(0, colmnames.Length - 1) + ") " + crlf + "VALUES " + crlf;

                for (var i=1; i< amstbl.Rows.Count; i++)
                {
                    var values = "'" + typecd + "',";
                    //foreach (var item in amstbl.Rows[i].ItemArray)
                    for (var l=1; l<amstbl.Rows[i].ItemArray.Length; l++)
                    {
                        if (amstbl.Rows[i].ItemArray[l].ToString().ToLower() != "null")
                        {
                            var data = amstbl.Rows[i].ItemArray[l].ToString();

                            if (data == "typecd")
                            {
                                data = typecd;
                            }
                            else if (data == "lastupddt")
                            {
                                data = dt;
                            }

                                values += "'" + data + "',";
                        }
                        else
                        {
                            values += amstbl.Rows[i].ItemArray[l].ToString() + ",";
                        }
                    }

                    if (values != "")
                    {
                        insertsql += "(" + values.Substring(0, values.Length - 1) + ")," + crlf;
                    }
                        
                }

                if (insertsql != "")
                {
                    insertsql = insertsql.RemoveAtLast(",") + ";" + crlf;
                    sqlret = inssqlheader + insertsql;
                }

                return true;
            }
            catch (Exception e)
            {
                OskNLog.Log(e.ToString(), Cnslcnf.msg_error);
                return false;
            }
        }

        public bool ArmsTableBuckUp(List<string> typecd, string tblname, out string sqlret)
        {
            //typecd = new List<string>()
            //{
            //    "AS0309H1210E1W302M2X2",
            //    "AU0309-1210E1W11-00E-6012"
            //};
            sqlret = string.Empty;

            if (typecd.Count == 0)
            {
                OskNLog.Log("製品の指定が空です", Cnslcnf.msg_error);
                return false;
            }

            if (typecd.Count > 999)
            {
                OskNLog.Log("製品の指定が1000を超えている為、処理できませんでした", Cnslcnf.msg_error);
                return false;
            }

            try
            {
                var sqlbkup = string.Empty;
                var sqlbkupheader = string.Empty;

                // yamlを読込
                var yamlPath = armsworkingdir + $@"\config\{tblname}.yaml";
                var tbl_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                var deserializer = new DeserializerBuilder()
                                   .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                   .Build();
                var tblCols = deserializer.Deserialize<List<string>>(tbl_yaml);


                // SQL Server に接続
                Console.Write("SQL Server に接続しています... ");
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("接続成功。");
                    var sql = $"SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS where table_name = '{tblname}'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            var i = 0;
                            while (reader.Read())
                            {
                                if (!tblCols.Contains(reader[0].ToString()))
                                {
                                    throw new Exception("Columnが不足しています:テーブル " + tblname + ":カラム " + reader[0].ToString());
                                }
                                //Console.WriteLine(reader[0]);
                            }
                        }
                    }

                    sql = "SELECT ";
                    foreach (var col in tblCols)
                    {
                        sql += "[" + col + "],";
                    }
                    sql = sql.Substring(0, sql.Length - 1);
                    sql += $" FROM {tblname} ";
                    sql += "WHERE typecd in (" + crlf;
                    foreach (var tcd in typecd)
                    {
                        sql += $"'{tcd}' ," + crlf;
                    }
                    sql = sql.RemoveAtLast(",") + ")";
                    

                    Console.WriteLine(sql);

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable schemaTable = reader.GetSchemaTable();
                            var colmnames = string.Empty;
                            foreach (DataRow row in schemaTable.Rows)
                            {
                                foreach (DataColumn column in schemaTable.Columns)
                                {
                                    if (column.ColumnName == "ColumnName")
                                    {
                                        colmnames += "[" + row[column].ToString() + "],";
                                    }
                                }
                            }

                            sqlbkupheader = $"INSERT INTO {tblname}" + crlf +  "(" + colmnames.Substring(0, colmnames.Length - 1) + ") " + crlf + "VALUES " + crlf;

                            while (reader.Read())
                            {
                                string rslt = "(";

                                foreach (DataRow row in schemaTable.Rows)
                                {
                                    var colmname = string.Empty;
                                    foreach (DataColumn column in schemaTable.Columns)
                                    {
                                        if (column.ColumnName == "ColumnName")
                                        {
                                            colmname = row[column].ToString();
                                            if (!tblCols.Contains(colmname))
                                            {
                                                Console.WriteLine("Columnが不足しています");
                                                throw new Exception("Columnが不足");
                                            }
                                        }
                                    }
                                    bool allownull = false;
                                    foreach (DataColumn column in schemaTable.Columns)
                                    {
                                        if (column.ColumnName == "AllowDBNull")
                                        {
                                            if (row[column].ToString() == "True")
                                            {
                                                allownull = true;
                                            }
                                            else
                                            {
                                                allownull = false;
                                            }
                                        }
                                    }
                                    if (reader[colmname].ToString() == "")
                                    {
                                        if (allownull)
                                        {
                                            rslt += " NULL,";
                                        }
                                        else
                                        {
                                            rslt += " ,";
                                        }
                                    }
                                    else
                                    {
                                        rslt += "'" + reader[colmname] + "',";
                                    }
                                }

                                rslt = rslt.Substring(0, rslt.Length - 1);
                                rslt += ")";
                                sqlbkup += rslt + "," + crlf;
                                //Console.WriteLine(sqlbkup);
                            }
                        }
                    }
                }

                if (sqlbkup != "")
                {
                    sqlbkup = sqlbkup.RemoveAtLast(",") + ";" + crlf;
                    sqlret = sqlbkupheader + sqlbkup;
                }
                return true;
            }
            catch (Exception e)
            {
                OskNLog.Log(e.ToString(), Cnslcnf.msg_error);
                return false;
            }
        }

    }

    public static class StringExtensions
    {
        /// <summary>
        /// <para>指定された文字列がこのインスタンス内で最後に見つかった場合、</para>
        /// <para>その文字列を削除した新しい文字列を返します</para>
        /// </summary>
        public static string RemoveAtLast(this string self, string value)
        {
            return self.Remove(self.LastIndexOf(value), value.Length);
        }
    }
}
