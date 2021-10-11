using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Oskas
{
    public class MySQL
    {
        public bool PingUpdatesState(string[] ConnectionStrings, ref string globalmsg)
        {
            try
            {
                int i = 0;
                foreach (var ConnectionString in ConnectionStrings)
                {
                    i++;
                    MySqlConnection conn = new MySqlConnection(ConnectionString);
                    conn.Open();
                    if (conn.Ping())
                    {
                        globalmsg += $"[DB-{i}] Ping:Pass ";
                    }
                    else
                    {
                        globalmsg += $"[DB-{i}] Ping:Fail, ";
                        return false;
                    }
                    conn.Close();
                    conn.Dispose();
                }

                return true;
            }
            catch(Exception ex)
            {
                globalmsg += ex.Message;
            }
            return false;
        }

        public bool _SqlTask_Read(string taskid, string ConnectionString, string query, int dataLen, ref Dictionary<string, string> dict, ref string globalmsg)
        {
            try
            {
                using (var connection = new MySqlConnection(ConnectionString))
                using (var command = new MySqlCommand())
                {
                    var nameList = new List<string>();
                    var retList = new List<string>();

                    // Check Count(*)
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = query.Replace("*", "count(*)");
                    var cnt = command.ExecuteReader();

                    int cout = 0;
                    while (cnt.Read())
                    {
                        cout = int.Parse(cnt.GetString(0));
                    }

                    if (cout != dataLen)
                    {
                        globalmsg += "SQL取得レコード数が不正です: taskid=" + taskid;
                        return false;
                    }
                    connection.Close();


                    // Select Data (*)
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = query;
                    var reader = command.ExecuteReader();

                    string[] names = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                        names[i] = reader.GetName(i);
                    nameList.AddRange(names);

                    while (reader.Read())
                    {
                        string[] ret = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                            ret[i] = reader.GetString(i);
                        retList.AddRange(ret);
                    }

                    for (int i = 0; i < nameList.Count(); i++)
                    {
                        dict.Add(nameList[i], retList[i]);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                globalmsg += ex.Message + ":taskid = " + taskid;
                return false;
            }

        }

        public bool SqlTask_Read(string taskid, string ConnectionString, string query, int dataLen, ref Dictionary<string, string> dict, ref string globalmsg)
        {
            try
            {
                // コネクションオブジェクトとコマンドオブジェクトを生成します。
                using (var connection = new MySqlConnection(ConnectionString))
                using (var command = new MySqlCommand())
                {
                    var nameList = new List<string>();
                    var retList = new List<string>();

                    // コネクションをオープンします。
                    connection.Open();

                    command.Connection = connection;
                    command.CommandText = query;

                    var reader = command.ExecuteReader();


                    string[] names = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                        names[i] = reader.GetName(i);
                    nameList.AddRange(names);

                    while (reader.Read())
                    {
                        string[] ret = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                            ret[i] = reader.GetString(i);
                        retList.AddRange(ret);
                    }

                    if (retList.Count() != dataLen)
                    {
                        globalmsg += "SQL取得データ数が不正です: taskid=" + taskid;
                        return false;
                    }

                    for (int i = 0; i < nameList.Count(); i++)
                    {
                        dict.Add(nameList[i], retList[i]);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                globalmsg += ex.Message + ":taskid = " + taskid; 
                return false;
            }

        }


        public bool SqlTask_Write(string taskid, string ConnectionString, string query, ref string globalmsg)
        {
            MySqlConnection conn = new MySqlConnection(ConnectionString);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();

                conn.Close();
                globalmsg += "データベースの書き込み処理が成功しました：taskid=" + taskid;
                return true;
            }
            catch (Exception ex)
            {
                globalmsg += ex.Message + ":taskid = " + taskid;
                return false;
            }

        }
    }
}
