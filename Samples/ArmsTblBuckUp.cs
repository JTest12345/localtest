using System;
using System.Data;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Collections.Generic;

namespace SqlServerSample
{

    class Program
    {
        static void _Main(string[] args)
        {

            var typecd = new string[]
            {
                "AS0309H1210E1W302M2X2",
                "AU0309-1210E1W11-00E-6012"
            };

            try
            {
                var sqlbkup = string.Empty;

                // yamlを読込
                var yamlPath = @"C:\Oskas\procmaster\shomei\ver9\model\root\arms" + @"\tmworkflow.yaml";
                var workflow_yaml = new StreamReader(yamlPath, Encoding.UTF8);
                var deserializer = new DeserializerBuilder()
                                   .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                   .Build();
                var workflowCols = deserializer.Deserialize<List<string>>(workflow_yaml);


                // 接続文字列を構築
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "vautom4\\SQLExpress";   // 接続先の SQL Server インスタンス
                builder.UserID = "inline";              // 接続ユーザー名
                builder.Password = "R28uHta"; // 接続パスワード
                builder.InitialCatalog = "ARMS";  // 接続するデータベース(ここは変えないでください)
                // builder.ConnectTimeout = 60000;  // 接続タイムアウトの秒数(ms) デフォルトは 15 秒

                // SQL Server に接続
                Console.Write("SQL Server に接続しています... ");
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    Console.WriteLine("接続成功。");
                    var sql = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS where table_name = 'TmWorkFlow'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        var sqlbkupheader = string.Empty;
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            var i=0;
                            while (reader.Read())
                            {
                                if (!workflowCols.Contains(reader[0].ToString()))
                                {
                                    throw new Exception("Columnが不足しています");
                                }
                                //Console.WriteLine(reader[0]);
                            }
                        }
                    }

                    sql = "SELECT ";
                    foreach (var col in workflowCols)
                    {
                        sql += col + ",";
                    }
                    sql = sql.Substring(0, sql.Length - 1);
                    sql += " FROM TmWorkFlow ";
                    if (typecd.Length > 0)
                    {
                        sql += "WHERE";
                        foreach (var tcd in typecd)
                        {
                            sql += $" typecd = '{tcd}' or";
                        }
                    }
                    sql = sql.RemoveAtLast("or");


                    Console.WriteLine(sql);

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        var sqlbkupheader = string.Empty;
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
                                        colmnames += row[column].ToString() + ",";
                                    }
                                }
                            }

                            sqlbkupheader = "INSERT INTO TmWorkFlow (" + colmnames.Substring(0, colmnames.Length - 1) + ") VALUES ";

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
                                            if (!workflowCols.Contains(colmname))
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
                                    if(reader[colmname].ToString() == "")
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
                                rslt = sqlbkupheader + rslt;
                                sqlbkup += rslt + ";\r\n";
                                //Console.WriteLine(sqlbkup);
                            }
                        }
                    }
                }

                sqlbkup = "SET XACT_ABORT ON;\r\n" + "BEGIN TRANSACTION;\r\n" + sqlbkup + "COMMIT TRANSACTION;";

                var FilePath = @"C:\Oskas\procmaster\shomei\ver9\model\root\arms\bu" + @"\tmworkflow.sql";
                using (FileStream fs = File.Create(FilePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(sqlbkup);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("全て完了しました。任意のキーを押して終了します...");
            Console.ReadKey(true);
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