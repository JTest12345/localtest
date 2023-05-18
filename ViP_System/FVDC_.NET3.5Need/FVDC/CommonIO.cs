/*************************************************************************************
 * システム名     : システム
 *  
 * 処理名         : CommonIO
 * 
 * 概略           : テーブルの入出力で共通的な処理を集めたクラス
 * 
 * 作成           : 2006/10/25 内田(SLA)
 * 
 * 修正履歴       : 
 ************************************************************************************/
 
using System;
using System. Collections. Generic;
using System. Text;
using System. Data;
using System. Data. SqlClient;
using System. Windows.Forms;

namespace FVDC
{	
	/// <summary>
	/// テーブルの入出力で共通的な処理を集めたクラス
	/// </summary>
	public class CommonIO
	{
		/// <summary>
		/// 読込
		/// </summary>
		/// <param name="ConnectServer">接続先</param>
		/// <param name="TableName">テーブル名</param>
		/// <param name="WhereSql">検索条件</param>
		/// <param name="dsTable">DataTable	dsTable	= データセット.データテーブル.Copy();</param>
		/// <returns></returns>
		public bool Read(string ConnectServer, string TableName, string WhereSql, ref DataTable dsTable)
		{
//			/// データをクリアする
			if (dsTable != null) dsTable.Clear();

			string SelectSql		= "SELECT ";
			string FromSql			= " FROM " + TableName + " WITH (NOLOCK) ";

			/// 読込対象カラム設定
			for(int j = 0; j < dsTable.Columns.Count; j++)
			{
				SelectSql			= SelectSql + "," + dsTable.Columns[j].ColumnName;
			}
			SelectSql				= SelectSql.Replace(" ,", " ");

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, false))
			{
				SqlDataReader reader = null;
				try
				{
					/// 読む
					connect.Command.CommandText = SelectSql + FromSql + WhereSql;
					reader = connect.Command.ExecuteReader();

					for ( int i = 0; reader.Read(); i++)
					{
						object[] DetailData	= new object[dsTable.Columns.Count];
						for(int j = 0; j < dsTable.Columns.Count; j++)
						{
							DetailData[j]	= reader[dsTable.Columns[j].ColumnName];
						}
						/// データセットテーブルに追加する
						dsTable.Rows.Add(DetailData);
					}
				}
				catch (SqlException ex)
				{
					///TODO: エラー処理
					MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer,"CommonIO Read");
					return false;
				}
				finally
				{
					if (reader != null)	reader.Close();
				}
			}
			return true;
		}

		/// <summary>
		/// 読込
		/// </summary>
		/// <param name="ConnectServer">接続先</param>
		/// <param name="TableName">テーブル名</param>
		/// <param name="WhereSql">検索条件</param>
		/// <param name="dsTable">DataTable	dsTable	= データセット.データテーブル.Copy();</param>
		/// <param name="RetryFG">デッドロック・タイムオーバーのときON</param>
		/// <returns></returns>
		public bool Read(string ConnectServer, string TableName, string WhereSql, ref DataTable dsTable, ref bool RetryFG)
		{
			/// データをクリアする
			if (dsTable != null) dsTable.Clear();
			RetryFG					= false;

			string SelectSql		= "SELECT ";
			string FromSql			= " FROM " + TableName + " WITH (NOLOCK) ";

			/// 読込対象カラム設定
			for(int j = 0; j < dsTable.Columns.Count; j++)
			{
				SelectSql			= SelectSql + "," + dsTable.Columns[j].ColumnName;
			}
			SelectSql				= SelectSql.Replace(" ,", " ");

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, false))
			{
				SqlDataReader reader = null;
				try
				{
					/// 読む
					connect.Command.CommandText = SelectSql + FromSql + WhereSql;
					reader = connect.Command.ExecuteReader();

					for ( int i = 0; reader.Read(); i++)
					{
						object[] DetailData	= new object[dsTable.Columns.Count];
						for(int j = 0; j < dsTable.Columns.Count; j++)
						{
							DetailData[j]	= reader[dsTable.Columns[j].ColumnName];
						}
						/// データセットテーブルに追加する
						dsTable.Rows.Add(DetailData);
					}
				}
				catch (SqlException ex)
				{
					///デッドロック・タイムオーバーのときは処理を繰り返す
					if (ex.Number == 1205 || ex.Number == -20 || ex.Number == -2)	
					{
						RetryFG				= true;
						return true;
					}
					///TODO: エラー処理
					MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer,"CommonIO Read");
					return false;
				}
				finally
				{
					if (reader != null)	reader.Close();
				}
			}
			return true;
		}

		/// <summary>
		/// 最大値読込
		/// </summary>
		/// <param name="ConnectServer">接続先</param>
		/// <param name="TableName">テーブル名</param>
		/// <param name="WhereSql">検索条件</param>
		/// <param name="dsTable">DataTable	dsTable	= データセット.データテーブル.Copy();</param>
		/// <returns></returns>
		public bool MaxRead(string ConnectServer, string TableName, string ItemName, string WhereSql, ref object MaxData)
		{
			string SelectSql		= "SELECT MAX(" + ItemName + ")";
			string FromSql			= " FROM " + TableName + " WITH (NOLOCK) ";

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, false))
			{
				SqlDataReader reader = null;
				try
				{
					/// 読む
					connect.Command.CommandText = SelectSql + FromSql + WhereSql;
					MaxData = connect.Command.ExecuteScalar();
				}
				catch (SqlException ex)
				{
					///TODO: エラー処理
					MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer,"CommonIO MaxRead");
					return false;
				}
				finally
				{
					connect.Close();
				}
			}
			return true;
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="ConnectServer">接続先</param>
		/// <param name="TableName">テーブル名</param>　
		/// <param name="dsTable">追加データ</param>
		/// <returns></returns>
		public bool Update(string ConnectServer, string TableName, string UpdateChar, string WhereChar)
		{
			string sql = "UPDATE " + TableName  + " SET " + UpdateChar + WhereChar;

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
			{
				SqlDataReader reader = null;
				try
				{
					/// 削除
					connect.Command.CommandText =  sql;
					int ExecCount = connect.Command.ExecuteNonQuery();

					if (ExecCount > 0)
					{
						connect.Commit();
					}
				}
				catch (SqlException ex)
				{
					///TODO: エラー処理
					MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer,"CommonIO Update");
					return false;
				}
				finally
				{
					connect.Close();
				}
			}
			return true;
		}

        /// <summary>
        /// 更新(新旧)
        /// </summary>
        /// <param name="ConnectServer"></param>
        /// <param name="TableName"></param>
        /// <param name="IX"></param>
        /// <param name="dsTable_OLD"></param>
        /// <param name="dsTable_NEW"></param>
        /// <returns></returns>
        public bool Update(string ConnectServer, string TableName, int IX, DataTable dsTable_OLD, DataTable dsTable_NEW)
        {
            string UpdateChar           = "@";
            string WhereChar            = "@";

            /// データを更新し易い文字タイプに成型する
            DataCast objDataCast        = new DataCast();
            string[] stgNewItem         = new string[dsTable_NEW.Columns.Count];
            DataRow dsRow               = dsTable_NEW.Rows[IX];
            objDataCast.dsRowCast(dsRow, ref stgNewItem);

            /// 更新対象設定
            for (int i = 1; i < dsTable_NEW.Columns.Count; i++)
            {
                UpdateChar              += "," + dsTable_NEW.Columns[i].ColumnName + " = " + stgNewItem[i];
            }
            UpdateChar                  = UpdateChar.Replace("@,", "");

            /// データを検索し易い文字タイプに成型する
            string[] stgOldItem         = new string[dsTable_OLD.Columns.Count];
            DataRow dsRow_OLD           = dsTable_OLD.Rows[IX];
            objDataCast.dsRowCast(dsRow_OLD, ref stgOldItem);

            /// 検索対象設定（前４項目を検索対象とする）
            for (int i = 1; (i < 5 && i < dsTable_OLD.Columns.Count); i++)
            {
                WhereChar               += " AND " + dsTable_OLD.Columns[i].ColumnName + " = " + stgOldItem[i];
            }
            WhereChar                   = WhereChar.Replace("@ AND ", " WHERE ");


            string sql                  = "UPDATE " + TableName + " SET " + UpdateChar + WhereChar;

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
            {
                SqlDataReader reader    = null;
                try
                {
                    /// 更新
                    connect.Command.CommandText = sql;
                    int ExecCount       = connect.Command.ExecuteNonQuery();

                    if (ExecCount > 0)
                    {
                        connect.Commit();
                    }
                }
                catch (SqlException ex)
                {
                    ///TODO: エラー処理
                    MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer, "CommonIO Update");
                    return false;
                }
                finally
                {
                    connect.Close();
                }
            }
            return true;
        }

		/// <summary>
		/// 追加
		/// </summary>
		/// <param name="ConnectServer">接続先</param>
		/// <param name="TableName">テーブル名</param>　
		/// <param name="dsTable">追加データ</param>
		/// <returns></returns>
        public bool Insert(string ConnectServer, string TableName, int IX, DataTable dsTable)
		{
			DataCast	objDataCast		= new DataCast();
			string IntoSql				= "INSERT INTO " + TableName + "("; 
			string ValuesSql			= " VALUES";

			/// 追加対象カラム設定
			for(int i = 1; i < dsTable.Columns.Count; i++)
			{
				IntoSql					= IntoSql + "," + dsTable.Columns[i].ColumnName;
			}
			IntoSql						= IntoSql.Replace("(,", "(") + ")";
						
            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
			{
				SqlDataReader reader = null;
				try
				{
					/// データを更新し易い文字タイプに成型する
					string[] stgItem			= new string[dsTable.Columns.Count];
                    DataRow dsRow               = dsTable.Rows[IX];
					objDataCast.dsRowCast(dsRow, ref stgItem);
					/// 追加対象カラム設定
					string InsData				= "(";
					for(int j = 1; j < dsTable.Columns.Count; j++)
					{
						InsData					= InsData + "," + stgItem[j];
					}
					InsData						= InsData.Replace("(,", "(") + ")";

					/// 追加
					connect.Command.CommandText =  IntoSql + ValuesSql + InsData;
					int ExecCount = connect.Command.ExecuteNonQuery();

					if (ExecCount > 0)
					{
						connect.Commit();
					}
				}
				catch (SqlException ex)
				{
					switch(ex.Number)
					{
						case 11:		/// ネットワークの一般エラー
							break;
						case 2627:		/// 重複エラー
						case 8152:		/// 桁あふれエラー
							break;		/// そのままＯＫとする
						default:
							///TODO: エラー処理
							MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer,"CommonIO Insert");
							return false;
					}
				}
				finally
				{
					connect.Close();
				}
			}
			return true;
		}
                
        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="ConnectServer">接続先</param>
        /// <param name="TableName">テーブル名</param>
        /// <param name="WhereSql">検索条件</param>
        /// <returns></returns>
        public bool Delete(string ConnectServer, string TableName, string WhereSql)
        {
            string sql = "DELETE FROM " + TableName + " " + WhereSql;

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
            {
                SqlDataReader reader = null;
                try
                {
                    /// 削除
                    connect.Command.CommandText = sql;
                    int ExecCount = connect.Command.ExecuteNonQuery();

                    if (ExecCount > 0)
                    {
                        connect.Commit();
                    }
                }
                catch (SqlException ex)
                {
                    ///TODO: エラー処理
                    MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer, "CommonIO Delete");
                    return false;
                }
                finally
                {
                    connect.Close();
                }
            }
            return true;
        }

        /// <summary>
        /// 削除
        /// </summary>
        /// <param name="ConnectServer">接続先</param>
        /// <param name="TableName">テーブル名</param>
        /// <param name="WhereSql">検索条件</param>
        /// <returns></returns>
        public bool Delete(string ConnectServer, string TableName, int IX, DataTable dsTable)
        {            
            /// データを検索し易い文字タイプに成型する
            DataCast objDataCast        = new DataCast();
            string[] stgOldItem         = new string[dsTable.Columns.Count];
            DataRow dsRow_OLD           = dsTable.Rows[IX];
            objDataCast.dsRowCast(dsRow_OLD, ref stgOldItem);

            /// 検索対象設定（全項目を検索対象とする）
            string WhereChar            = "@";
            for (int i = 1; (i < dsTable.Columns.Count); i++)
            {
                WhereChar               += " AND " + dsTable.Columns[i].ColumnName + " = " + stgOldItem[i];
            }
            WhereChar                   = WhereChar.Replace("@ AND ", " WHERE ");
            
            string sql = "DELETE FROM " + TableName + " " + WhereChar;

            using (IConnection connect = ServerConnection.CreateInstance(ConnectServer, true))
            {
                try
                {
                    /// 削除
                    connect.Command.CommandText = sql;
                    int ExecCount               = connect.Command.ExecuteNonQuery();

                    if (ExecCount > 0)
                    {
                        connect.Commit();
                    }
                }
                catch (SqlException ex)
                {
                    ///TODO: エラー処理
                    MessageBox.Show(ex.Number + ":" + ex.Message + "\n" + connect.Command.CommandText + "\n" + ConnectServer, "CommonIO Delete");
                    return false;
                }
                finally
                {
                    connect.Close();
                }
            }
            return true;
        }
	}
}
