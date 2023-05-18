/*************************************************************************************
 * システム名     : 設備マスタシステム
 *  
 * 処理名         : CommonRead
 * 
 * 概略           : 共有データセットを作成する処理を集めたクラス
 * 
 * 作成           : 2016/02/29 SLA2.Uchida
 * 
 * 修正履歴       : 
 ************************************************************************************/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FVDC
{
	/// <summary>
	/// 共有データセットの読み込み
	/// </summary>
	public class CommonRead
	{
		/// <summary>
		/// 名前情報の読み込み
		/// </summary>
		/// <param name="ConnectServer"></param>
		/// <param name="TableName"></param>
		/// <param name="KeyName"></param>
		/// <param name="DataName"></param>
		/// <param name="WhereSql"></param>
		/// <param name="dsName"></param>
		/// <returns></returns>
        public bool NameRead(string ServerNM, string LoginNM, string DatabaseNM, string TableName, string KeyName, string DataName, string WhereSql, bool DistinctON, ref DsName dsName)
        {
            
		    string SelectSql		= "SELECT DISTINCT " + KeyName + ", " + DataName;
			if ( KeyName == DataName )
			{
				SelectSql			= "SELECT DISTINCT " + KeyName;
            }
            if (!DistinctON)
            {
                SelectSql = SelectSql.Replace(" DISTINCT", "");
            }
		    string FromSql			= " FROM " + TableName + " WITH (NOLOCK) ";
            string[] splKeyName     = KeyName.Split('.');
            if (splKeyName.Length > 1)
			{
                KeyName             = splKeyName[1];
			}
            splKeyName              = DataName.Split('.');
            if (splKeyName.Length > 1)
			{
                DataName            = splKeyName[1];
			}

			/// 項目名でＡＳ修飾を使用しているとき、実際の名称のみにする
			int asPiont				= KeyName.LastIndexOf(" AS ");
			if ( asPiont > -1 )
			{
				KeyName				= KeyName.Substring(asPiont + 4).Trim();
			}
			asPiont					= DataName.LastIndexOf(" AS ");
			if ( asPiont > -1 )
			{
				DataName			= DataName.Substring(asPiont + 4).Trim();
			}
			try
			{
                using (IConnection sqlInfo = ServerConnection.CreateInstance(ConnectServer(ServerNM, LoginNM, DatabaseNM), true))
				{
					SqlDataReader reader = null;
					try
					{
						/// 部門マスタを読む
						sqlInfo.Command.CommandText	= SelectSql + FromSql + WhereSql;
						reader = sqlInfo.Command.ExecuteReader();

						for ( int i = 0; reader.Read(); i++)
						{
							if (TableName == "TtFATD"
								&& dsName.Name.Rows.Count == 0 
								&& reader[DataName].ToString().Trim() != "")
							{
								dsName.Name.Rows.Add(new object[] { "", "" });
                            }
                            try
                            {
                                dsName.Name.Rows.Add(
                                    new object[] {
                                             reader[KeyName],
                                             reader[DataName].ToString().Trim()
                                                 });
                            }
                            catch
                            {
                                dsName.Name.Rows.Add(
                                    new object[] {
                                             reader[0],
                                             reader[1]
                                                 });
                            }
                        }
						return true;
					}
					catch (SqlException ex)
					{
						///TODO: エラー処理
						System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText,"CommonRead NameRead");
						return false;
					}
					finally
					{
						if (reader != null)	reader.Close();
					}
				}
			}
			catch (Exception ex)
			{
				///TODO: エラー処理
				//System.Windows.Forms.MessageBox.Show(ex.Message,"CommonRead NameRead");
		        return false;
			}
		}

		/// <summary>
		/// 名前情報の読み込み
		/// </summary>
		/// <param name="sqlInfo"></param>
		/// <param name="TableName"></param>
		/// <param name="KeyName"></param>
		/// <param name="DataName"></param>
		/// <param name="WhereSql"></param>
		/// <param name="DistinctON"></param>
		/// <param name="dsName"></param>
		/// <returns></returns>
		public bool NameRead(IConnection sqlInfo, string TableName, string KeyName, string DataName, string WhereSql, bool DistinctON, ref DsName dsName)
		{
		    string SelectSql		= "SELECT DISTINCT " + KeyName + ", " + DataName;
			if ( KeyName == DataName )
			{
				SelectSql			= "SELECT DISTINCT " + KeyName;
			}
			if ( !DistinctON )
			{
				SelectSql			= SelectSql.Replace(" DISTINCT","");
			}
		    string FromSql			= " FROM " + TableName + " WITH (NOLOCK) ";
			string [] splName		= KeyName.Split('.');
			if ( splName.Length > 1 )
			{
				KeyName				= splName [1];
			}
			splName					= DataName.Split('.');
			if ( splName.Length > 1 )
			{
				DataName			= splName [1];
			}

			/// 項目名でＡＳ修飾を使用しているとき、実際の名称のみにする
			int asPiont				= KeyName.LastIndexOf(" AS ");
			if ( asPiont > -1 )
			{
				KeyName				= KeyName.Substring(asPiont + 4).Trim();
			}
			asPiont					= DataName.LastIndexOf(" AS ");
			if ( asPiont > -1 )
			{
				DataName			= DataName.Substring(asPiont + 4).Trim();
			}
			try
			{
				SqlDataReader reader = null;
				try
				{
					/// 読む
					sqlInfo.Command.CommandText	= SelectSql + FromSql + WhereSql;
					reader = sqlInfo.Command.ExecuteReader();

					for ( int i = 0; reader.Read(); i++)
					{
						if (TableName == "TtReqt"
							&& dsName.Name.Rows.Count == 0 
							&& reader[DataName].ToString().Trim() != "")
						{
							dsName.Name.Rows.Add(new object[] { "", "" });
						}
                        try
                        {
                            dsName.Name.Rows.Add(
                                new object[] {
                                             reader[KeyName],
                                             reader[DataName].ToString().Trim()
                                             });
                        }
                        catch
                        {
                            try
                            {

                                dsName.Name.Rows.Add(
                                    new object[] {
                                             reader[0],
                                             reader[1]
                                                 });
                            }
                            catch
                            {
                                dsName.Name.Rows.Add(
                                    new object[] {
                                             reader[0],
                                             reader[0]
                                                 });
                            }
                        }
					}
					return true;
				}
				catch (SqlException ex)
				{
					///TODO: エラー処理
					System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText,"CommonRead NameRead");
					return false;
				}
				finally
				{
					if (reader != null)	reader.Close();
				}
			}
			catch (Exception ex)
			{
				///TODO: エラー処理
				//System.Windows.Forms.MessageBox.Show(ex.Message,"CommonRead NameRead");
		        return false;
			}
		}

        /// <summary>
        /// 名前情報の読み込み
        /// </summary>
        /// <param name="sqlInfo"></param>
        /// <param name="TableName"></param>
        /// <param name="KeyName"></param>
        /// <param name="DataName"></param>
        /// <param name="WhereSql"></param>
        /// <param name="DistinctON"></param>
        /// <param name="dsName"></param>
        /// <returns></returns>
        public bool TopNameRead(IConnection sqlInfo, string TopCount, string TableName, string KeyName, string DataName, string WhereSql, bool DistinctON, ref DsName dsName)
        {
            string SelectSql = "SELECT DISTINCT " + TopCount + KeyName + ", " + DataName;
            if (KeyName == DataName)
            {
                SelectSql = "SELECT DISTINCT " + TopCount + KeyName;
            }
            if (!DistinctON)
            {
                SelectSql = SelectSql.Replace(" DISTINCT", "");
            }
            string FromSql = " FROM " + TableName + " WITH (NOLOCK) ";
            string[] splName = KeyName.Split('.');
            if (splName.Length > 1)
            {
                KeyName = splName[1];
            }
            splName = DataName.Split('.');
            if (splName.Length > 1)
            {
                DataName = splName[1];
            }

            /// 項目名でＡＳ修飾を使用しているとき、実際の名称のみにする
            int asPiont = KeyName.LastIndexOf(" AS ");
            if (asPiont > -1)
            {
                KeyName = KeyName.Substring(asPiont + 4).Trim();
            }
            asPiont = DataName.LastIndexOf(" AS ");
            if (asPiont > -1)
            {
                DataName = DataName.Substring(asPiont + 4).Trim();
            }
            try
            {
                SqlDataReader reader = null;
                try
                {
                    /// 読む
                    sqlInfo.Command.CommandText = SelectSql + FromSql + WhereSql;
                    reader = sqlInfo.Command.ExecuteReader();

                    for (int i = 0; reader.Read(); i++)
                    {
                        if (TableName == "TtReqt"
                            && dsName.Name.Rows.Count == 0
                            && reader[DataName].ToString().Trim() != "")
                        {
                            dsName.Name.Rows.Add(new object[] { "", "" });
                        }
                        try
                        {
                            dsName.Name.Rows.Add(
                                new object[] {
                                             reader[KeyName],
                                             reader[DataName].ToString().Trim()
                                             });
                        }
                        catch
                        {
                            dsName.Name.Rows.Add(
                                new object[] {
                                             reader[0],
                                             reader[1]
                                             });
                        }
                    }
                    return true;
                }
                catch (SqlException ex)
                {
                    ///TODO: エラー処理
                    System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText, "CommonRead TopNameRead");
                    return false;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
            catch (Exception ex)
            {
                ///TODO: エラー処理
                //System.Windows.Forms.MessageBox.Show(ex.Message,"CommonRead TopNameRead");
                return false;
            }
        }

        /// <summary>
        /// 最大値情報の読み込み
        /// </summary>
        /// <param name="sqlInfo"></param>
        /// <param name="TableName"></param>
        /// <param name="KeyName"></param>
        /// <param name="WhereSql"></param>
        /// <returns>MaxData</returns>
        public bool MaxRead(IConnection sqlInfo, string TableName, string KeyName, string WhereSql, ref string MaxData)
        {
            string FromSql              = " FROM " + TableName + " WITH (NOLOCK) ";

            /// 項目名でＡＳ修飾を使用しているとき、実際の名称のみにする
            int asPiont = KeyName.LastIndexOf(" AS ");
            if (asPiont > -1)
            {
                KeyName = KeyName.Substring(asPiont + 4).Trim();
            }
            string SelectSql            = "SELECT MAX(" + KeyName + ") ";
            try
            {
                SqlDataReader reader = null;
                try
                {
                    /// 読む
                    sqlInfo.Command.CommandText = SelectSql + FromSql + WhereSql;
                    MaxData = sqlInfo.Command.ExecuteScalar().ToString();
                    return true;
                }
                catch (SqlException ex)
                {
                    ///TODO: エラー処理
                    System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText, "CommonRead MaxRead");
                    return false;
                }
                finally
                {
                    if (reader != null) reader.Close();
                }
            }
            catch (Exception ex)
            {
                ///TODO: エラー処理
                //System.Windows.Forms.MessageBox.Show(ex.Message,"CommonRead MaxRead");
                return false;
            }
        }
        
        /// <summary>
        /// 名前情報の読み込み
        /// </summary>
        /// <param name="sqlInfo"></param>
        /// <param name="TableName"></param>
        /// <param name="DataName"></param>
        /// <param name="WhereSql"></param>
        /// <param name="refString"></param>
        /// <returns></returns>
        public bool OnceRead(IConnection sqlInfo, string TableName, string DataName, string WhereSql, ref string refString)
        {
            refString                           = "";
            string SelectSql                    = "SELECT " + DataName;
            string FromSql                      = " FROM " + TableName + " WITH (NOLOCK) ";

            /// 項目名でＡＳ修飾を使用しているとき、実際の名称のみにする
            int asPiont = DataName.LastIndexOf(" AS ");
            if (asPiont > -1)
            {
                DataName = DataName.Substring(asPiont + 4).Trim();
            }
            try
            {
                try
                {
                    /// 読む
                    sqlInfo.Command.CommandText = SelectSql + FromSql + WhereSql;
                    object objString            = sqlInfo.Command.ExecuteScalar();
                    if (objString == null)
                    {
                        return false;
                    }
                    else 
                    {
                        switch (objString.GetType().ToString())
                        {
                            case "System.String":
                                refString       = (string)objString;
                                refString       = refString.Trim();
                                break;
                            case "System.Int32":
                                refString       = Convert.ToInt32(objString).ToString();
                                break;
                            case "System.Double":
                                refString       = Convert.ToDouble(objString).ToString();
                                break;
                            case "System.DateTime":
                                refString       = Convert.ToDateTime(objString).ToString();
                                break;
                        }
                    }

                    return true;
                }
                catch (SqlException ex)
                {
                    ///TODO: エラー処理
                    System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText,"CommonRead OnceRead");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ///TODO: エラー処理
                //System.Windows.Forms.MessageBox.Show(ex.Message,"CommonRead OnceRead");
                return false;
            }
        }

        /// <summary>
        /// 名前情報の読み込み
        /// </summary>
        /// <param name="sqlInfo"></param>
        /// <param name="TableName"></param>
        /// <param name="DataName"></param>
        /// <param name="WhereSql"></param>
        /// <param name="refString"></param>
        /// <returns></returns>
        public bool OnceRead(string ServerNM, string LoginNM, string DatabaseNM, string TableName, string DataName, string WhereSql, ref string refString)
        {

            using (IConnection sqlInfo = ServerConnection.CreateInstance(ConnectServer(ServerNM, LoginNM, DatabaseNM), true))
            {
                /// データベースに接続出来なかったとき
                if (sqlInfo.Transaction == null)
                {
                    return false;
                }
                refString = "";
                string SelectSql    = "SELECT " + DataName;
                string FromSql      = " FROM " + TableName + " WITH (NOLOCK) ";

                /// 項目名でＡＳ修飾を使用しているとき、実際の名称のみにする
                int asPiont         = DataName.LastIndexOf(" AS ");
                if (asPiont > -1)
                {
                    DataName        = DataName.Substring(asPiont + 4).Trim();
                }
                try
                {
                    try
                    {
                        /// 読む
                        sqlInfo.Command.CommandText     = SelectSql + FromSql + WhereSql;
                        object objString                = sqlInfo.Command.ExecuteScalar();
                        if (objString == null)
                        {
                            return false;
                        }
                        else
                        {
                            switch (objString.GetType().ToString())
                            {
                                case "System.String":
                                    refString   = (string)objString;
                                    refString   = refString.Trim();
                                    break;
                                case "System.Int32":
                                    refString   = Convert.ToInt32(objString).ToString();
                                    break;
                                case "System.Int64":
                                    refString   = Convert.ToInt64(objString).ToString();
                                    break;
                                case "System.Double":
                                    refString   = Convert.ToDouble(objString).ToString();
                                    break;
                                case "System.DateTime":
                                    refString   = Convert.ToDateTime(objString).ToString();
                                    break;
                            }
                        }

                        return true;
                    }
                    catch (SqlException ex)
                    {
                        ///TODO: エラー処理
                        System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText, "CommonRead OnceRead");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    ///TODO: エラー処理
                    //System.Windows.Forms.MessageBox.Show(ex.Message,"CommonRead OnceRead");
                    return false;
                }
            }
        }
        
        /// <summary>
        /// SQLテーブル情報を読込み対応したデータセットを生成します。
        /// </summary>
        /// <param name="sqlInfo"></param>
        /// <param name="TableNM"></param>
        /// <param name="dsGridView"></param>
        /// <returns></returns>
        public bool SqlDataSetCreate(IConnection sqlInfo, string TableNM, ref DsSqlData dsGridView)
        {
            string sql      = "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WITH (NOLOCK) "
                                + " WHERE (TABLE_SCHEMA = 'dbo') AND (TABLE_NAME = '{0}')";

            SqlDataReader reader = null;
            try
            {
                //	基本情報の読み込み
                sqlInfo.Command.CommandText = string.Format(sql, TableNM);

                reader = sqlInfo.Command.ExecuteReader();
				for ( int i = 0; reader.Read(); i++)
				{
                    string ColumnNM         = reader["COLUMN_NAME"].ToString().Trim();

                    /// NULLを許可しているカラムは無条件に文字型とする
                    if (reader["IS_NULLABLE"].ToString().Trim() == "YES" && ColumnNM != "LastUpd_DT")
                    {
                        dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.String"));
                    }
                    else
                    {
                        /// データ型に応じてカラムを追加する
                        switch (reader["DATA_TYPE"].ToString().Trim())
                        {
                            case "char":
                            case "nchar":
                            case "varchar":
                            case "nvarchar":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.String"));
                                break;
                            case "datetime":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.DateTime"));
                                break;
                            case "int":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.Int32"));
                                break;
                            case "bigint":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.Int64"));
                                break;
                            case "numeric":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.Double"));
                                break;
                            case "bit":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.Boolean"));
                                break;
                            default:
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.String"));
                                break;
                        }
                    }
                    dsGridView.LIST.Columns[ColumnNM].Caption = ColumnNM;
                }
                return true;
            }
            catch (SqlException ex)
            {
                ///TODO: エラー処理
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText, "CommonRead SqlDataSetCreate");
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// 必要資材データセットを生成します。
        /// </summary>
        /// <param name="TableNM"></param>
        /// <param name="DsSqlData"></param>
        /// <returns></returns>
        public bool SqlDataSetCreate(ref CheckBox chkTableNM, ref DsSqlData dsGridView)
        {

            chkTableNM.Text     = "ARMS【必要資材】";
            chkTableNM.Checked  = false;

            dsGridView.LIST.Columns.Add("profileid", Type.GetType("System.Int32"));
            dsGridView.LIST.Columns["profileid"].Caption        = "profileid";
            dsGridView.LIST.Columns.Add("typecd", Type.GetType("System.String"));
            dsGridView.LIST.Columns["typecd"].Caption           = "typecd";
            dsGridView.LIST.Columns.Add("profilenm", Type.GetType("System.String"));
            dsGridView.LIST.Columns["profilenm"].Caption        = "profilenm";
            dsGridView.LIST.Columns.Add("workcd", Type.GetType("System.String"));
            dsGridView.LIST.Columns["workcd"].Caption           = "workcd";
            dsGridView.LIST.Columns.Add("materialcd", Type.GetType("System.String"));
            dsGridView.LIST.Columns["materialcd"].Caption       = "materialcd";
            dsGridView.LIST.Columns.Add("materialnm", Type.GetType("System.String"));
            dsGridView.LIST.Columns["materialnm"].Caption       = "materialnm";
            dsGridView.LIST.Columns.Add("lotno", Type.GetType("System.String"));
            dsGridView.LIST.Columns["lotno"].Caption            = "materialnm";

            return true;
        }
        
        /// <summary>
        /// プロファイルIDに関連する必要資材データを読込みデータセットを生成します。
        /// </summary>
        /// <param name="ServerNM"></param>
        /// <param name="DatabaseNM"></param>
        /// <param name="TableNM"></param>
        /// <param name="DsSqlData"></param>
        /// <returns></returns>
        public bool SqlDataFind(IConnection sqlInfo, string ProfileID, ref DsSqlData dsGridView)
        {
            /// ダイボンダー必要資材読込
            string sql = "SELECT DISTINCT TmProfile.profileid AS profileid, TmProfile.typecd AS typecd, TmProfile.profilenm AS profilenm, TmTypeCond.workcd AS workcd, TmTypeCond.materialcd AS materialcd, TnMaterials.materialnm AS materialnm "
                                + " FROM TmProfile WITH (NOLOCK) INNER JOIN TmTypeCond WITH (NOLOCK) ON TmProfile.typecd = TmTypeCond.typecd"
                                + " INNER JOIN TmBom WITH (NOLOCK) ON TmProfile.profileid = TmBom.profileid AND TmTypeCond.materialcd = TmBom.materialcd"
                                + " INNER JOIN TmMatRequire WITH (NOLOCK) ON TmBom.lotcharcd = TmMatRequire.lotcharcd AND TmTypeCond.workcd = TmMatRequire.workcd"
                                + " INNER JOIN TnMaterials WITH (NOLOCK) ON TmTypeCond.materialcd = TnMaterials.materialcd"
                                + " WHERE (TmProfile.profileid = {0}) AND (TmTypeCond.condcd = N'26') AND (TmTypeCond.delfg = 0) AND (TmMatRequire.delfg = 0) AND (TmBom.delfg = 0) AND (TnMaterials.delfg = 0)"
                                + " ORDER BY TmTypeCond.workcd, TmTypeCond.materialcd ";
            
            SqlDataReader reader            = null;
            try
            {
                //	基本情報の読み込み
                sqlInfo.Command.CommandText = string.Format(sql, ProfileID);

                reader                      = sqlInfo.Command.ExecuteReader();
                for (int i = 0; reader.Read(); i++)
                {
                    dsGridView.LIST.Rows.Add(
                        new object[] {
                                             "照会",
                                             reader["profileid"],
                                             reader["typecd"].ToString().Trim(),
                                             reader["profilenm"].ToString().Trim(),
                                             reader["workcd"].ToString().Trim(),
                                             reader["materialcd"].ToString().Trim(),
                                             reader["materialnm"].ToString().Trim()
                                         });
                }
            }
            catch (SqlException ex)
            {
                ///TODO: エラー処理
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText, "CommonRead SqlDataSetCreate1");
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }


            /// ワイヤーボンダー必要資材読込
            sql = "SELECT DISTINCT TmProfile.profileid AS profileid, TmProfile.typecd AS typecd, TmProfile.profilenm AS profilenm, TmMatRequire.workcd AS workcd, TmBom.materialcd AS materialcd, TnMaterials.materialnm AS materialnm "
                                 + " FROM TmProfile WITH (NOLOCK) INNER JOIN TmBom WITH (NOLOCK) ON TmProfile.profileid = TmBom.profileid"
                                 + " INNER JOIN TmMatRequire WITH (NOLOCK) ON TmBom.lotcharcd = TmMatRequire.lotcharcd"
                                 + " INNER JOIN TnMaterials WITH (NOLOCK) ON TmBom.materialcd = TnMaterials.materialcd"
                                 + " WHERE (TmProfile.profileid = {0}) AND (TmMatRequire.delfg = 0) AND (TmBom.delfg = 0) AND (TnMaterials.delfg = 0) AND (TmMatRequire.workcd LIKE N'WB%')";

            reader          = null;
            try
            {
                //	基本情報の読み込み
                sqlInfo.Command.CommandText = string.Format(sql, ProfileID);

                reader = sqlInfo.Command.ExecuteReader();
                for (int i = 0; reader.Read(); i++)
                {
                    dsGridView.LIST.Rows.Add(
                        new object[] {
                                             "照会",
											 reader["profileid"],
											 reader["typecd"].ToString().Trim(),
											 reader["profilenm"].ToString().Trim(),
											 reader["workcd"].ToString().Trim(),
											 reader["materialcd"].ToString().Trim(),
											 reader["materialnm"].ToString().Trim()
										 });
                }
            }
            catch (SqlException ex)
            {
                ///TODO: エラー処理
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText, "CommonRead SqlDataSetCreate2");
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            /// 1stダイボンダー必要資材読込
            sql = "SELECT TOP (10) TmProfile.profileid AS profileid, TmProfile.typecd AS typecd, TmProfile.profilenm AS profilenm, TnMaterials.materialcd AS materialcd, TnMaterials.materialnm AS materialnm, TnMaterials.lotno AS lotno "
                                 + " FROM TmProfile WITH (NOLOCK) INNER JOIN TmBom WITH (NOLOCK) ON TmProfile.profileid = TmBom.profileid "
                                 + " INNER JOIN TnMaterials WITH (NOLOCK) ON TmBom.materialcd = TnMaterials.materialcd "
                                 + " WHERE (TmProfile.profileid = {0}) AND (TmBom.lotcharcd = N'M0000002') AND (TnMaterials.delfg = 0) "
                                 + " ORDER BY TnMaterials.limitdt DESC ";

            reader = null;
            try
            {
                //	基本情報の読み込み
                sqlInfo.Command.CommandText = string.Format(sql, ProfileID);

                reader = sqlInfo.Command.ExecuteReader();
                for (int i = 0; reader.Read(); i++)
                {
                    dsGridView.LIST.Rows.Add(
                        new object[] {
                                             "照会",
											 reader["profileid"],
											 reader["typecd"].ToString().Trim(),
											 reader["profilenm"].ToString().Trim(),
											 "DB0003",
											 reader["materialcd"].ToString().Trim(),
											 reader["materialnm"].ToString().Trim(),
                                             reader["lotno"].ToString().Trim()
										 });
                }
            }
            catch (SqlException ex)
            {
                ///TODO: エラー処理
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText, "CommonRead SqlDataSetCreate2");
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            /// ダイボンダー必要資材読込
            sql = "SELECT DISTINCT TmProfile.profileid AS profileid, TmProfile.typecd AS typecd, TmProfile.profilenm AS profilenm, TnMaterials.workcd AS workcd, TnMaterials.materialcd AS materialcd, TnMaterials.materialnm AS materialnm, TnMaterials.lotno AS lotno "
                                 + " FROM TmProfile WITH (NOLOCK) INNER JOIN TnMaterials WITH (NOLOCK) ON TmProfile.blendcd = TnMaterials.blendcd"
                                 + " WHERE (TmProfile.profileid = {0}) AND (TnMaterials.delfg = 0) "
                                 + " ORDER BY TnMaterials.lotno "; 

            reader = null;
            try
            {
                //	基本情報の読み込み
                sqlInfo.Command.CommandText = string.Format(sql, ProfileID);

                reader = sqlInfo.Command.ExecuteReader();
                for (int i = 0; reader.Read(); i++)
                {
                    dsGridView.LIST.Rows.Add(
                        new object[] {
                                             "照会",
											 reader["profileid"],
											 reader["typecd"].ToString().Trim(),
											 reader["profilenm"].ToString().Trim(),
											 "DB",
											 reader["materialcd"].ToString().Trim(),
											 reader["materialnm"].ToString().Trim(),
                                             reader["lotno"].ToString().Trim()
										 });
                }
                return true;
            }
            catch (SqlException ex)
            {
                ///TODO: エラー処理
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText, "CommonRead SqlDataSetCreate2");
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// SQLテーブル情報を読込み対応したデータセットを生成します。
        /// </summary>
        /// <param name="ServerNM"></param>
        /// <param name="DatabaseNM"></param>
        /// <param name="TableNM"></param>
        /// <param name="DsSqlData"></param>
        /// <returns></returns>
        public bool SqlDataSetCreate(IConnection sqlInfo, string DatabaseNM, string TableNM, ref CheckBox chkTableNM, ref DsSqlData dsGridView)
        {
            string Title            = "{0}【{1}】";
            string sql              = "SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_OCTET_LENGTH, NUMERIC_PRECISION FROM INFORMATION_SCHEMA.COLUMNS WITH (NOLOCK) "
                                        + " WHERE (TABLE_SCHEMA = 'dbo') AND (TABLE_NAME = '{0}')";

            chkTableNM.Text         = string.Format(Title, DatabaseNM, TableNM);
            chkTableNM.Checked      = false;

            SqlDataReader reader    = null;
            try
            {
                //	基本情報の読み込み
                sqlInfo.Command.CommandText     = string.Format(sql, TableNM);

                reader              = sqlInfo.Command.ExecuteReader();
                for (int i = 0; reader.Read(); i++)
                {
                    string ColumnNM = reader["COLUMN_NAME"].ToString().Trim();


                    try
                    {
                        /// ↓↓↓↓↓↓　2017/11/27 SLA2.Uchida 予約語の場合には前後にカッコを付与する　↓↓↓↓↓↓
                        if (ColumnNM == "lineno")
                        {
                            ColumnNM = "[lineno]";
                        }
                        /// ↑↑↑↑↑↑  2017/11/27 SLA2.Uchida 予約語の場合には前後にカッコを付与する  ↑↑↑↑↑↑

                        /// データ型に応じてカラムを追加する
                        switch (reader["DATA_TYPE"].ToString().Trim())
                        {
                            case "char":
                            case "nchar":
                            case "varchar":
                            case "nvarchar":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.String"));
                                break;
                            case "datetime":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.DateTime"));
                                break;
                            case "int":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.Int32"));
                                break;
                            case "bigint":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.Int64"));
                                break;
                            case "numeric":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.Double"));
                                break;
                            case "bit":
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.Boolean"));
                                break;
                            default:
                                dsGridView.LIST.Columns.Add(ColumnNM, Type.GetType("System.String"));
                                break;
                        }
                        dsGridView.LIST.Columns[ColumnNM].Caption   = ColumnNM;
                    }
                    catch { }
                }
                if (dsGridView.LIST.Columns.Count <= 1) return false;
                return true;
            }
            catch (SqlException ex)
            {
                ///TODO: エラー処理
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText, "CommonRead SqlDataSetCreate");
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// SQLテーブル情報を読込み対応したデータセットを生成します。（最大200件まで）
        /// </summary>
        /// <param name="ServerNM"></param>
        /// <param name="DatabaseNM"></param>
        /// <param name="TableNM"></param>
        /// <param name="DsSqlData"></param>
        /// <returns></returns>
        public bool SqlDataFind(IConnection sqlInfo, string RowHedNM, string TableNM, string ColumnNM, string FindData, ref DsSqlData dsGridView)
        {
            /// 読込む項目をテーブルより生成する（最大200件まで）
            string FindColumn           = "SELECT TOP 200 @";
            for (int i = 1; i < dsGridView.LIST.Columns.Count; i++)
            {
                /// 文字型のときNULLの可能性がある
                if (dsGridView.LIST.Columns[i].DataType.Equals(Type.GetType("System.String")))
                {
                    /// データがNULLのとき文字型でNULLを返すようにする
                    FindColumn              += ",ISNULL(CONVERT(CHAR(200)," + TableNM + "." + dsGridView.LIST.Columns[i].Caption + "),'NULL') AS " + dsGridView.LIST.Columns[i].Caption;
                }
                else
                {
                    FindColumn              += "," + TableNM + "." + dsGridView.LIST.Columns[i].Caption;
                }
            }
            FindColumn                      = FindColumn.Replace("@,", "");

            /// テーブル名設定
            string sql                      = "";

            /// 入力が無いときは全件検索とする
            if (ColumnNM == "" && FindData == "")
            {
                sql                         = FindColumn + " FROM " + TableNM + " WITH (NOLOCK) ";
            }
            /// 条件式が入力されているとき
            else if ((FindData.Contains("=") || (FindData.Contains("LIKE"))
                    || (FindData.Contains("IN"))
                    || (FindData.Contains(">")) || (FindData.Contains("<"))))
            {
                if (ColumnNM == "")
                {
                    sql                     = FindColumn + " FROM " + TableNM + " WITH (NOLOCK) " + FindData;
                }
                else
                {
                    sql                     = FindColumn + " FROM " + TableNM + " WITH (NOLOCK) WHERE " + ColumnNM + " " + FindData;
                }
            }
            else
            {
                if ((ColumnNM == "macno")
                    || (ColumnNM == "Equipment_CD"))
                {
                    sql                     = FindColumn + " FROM " + TableNM + " WITH (NOLOCK) "
                                                + " WHERE ({0} IN ({1}))";
                    sql                     = string.Format(sql, ColumnNM, FindData);
                }
                else if (FindData.Contains(" IN("))
                {
                    sql                     = FindColumn + " FROM " + TableNM + " WITH (NOLOCK) " + FindData;
                }
                else
                {
                    string [] sptFindData   = FindData.Split(',');
                    sql                     = FindColumn + " FROM " + TableNM + " WITH (NOLOCK) WHERE @";
                    for (int j = 0; j < sptFindData.Length; j++)
                    {
                        sql                 += "OR ({0} LIKE " + sptFindData[j] + "%') ";
                    }
                    sql                     = string.Format(sql, ColumnNM);
                    sql                     = sql.Replace("'%", "%").Replace("@OR ", "");
                }
            }
            
            SqlDataReader reader        = null;
            try
            {
                ///	基本情報の読み込み
                sqlInfo.Command.CommandText                     = sql;

                reader              = sqlInfo.Command.ExecuteReader();
                /// 全てのレコードを取得する
                for (int i = dsGridView.LIST.Rows.Count; reader.Read(); i++)
                {
                    dsGridView.LIST.Rows.Add(new object[] { RowHedNM });
                    /// カラムデータを設定
                    for (int j = 1; j < dsGridView.LIST.Columns.Count; j++)
                    {
                        /// Booleanのとき
                        if (dsGridView.LIST.Columns[j].DataType.Equals(Type.GetType("System.Boolean")))
                        {
                            /// NULLのとき"NULL"をセット
                            try
                            {
                                if (reader[j - 1].ToString().Trim() == "")
                                {
                                    dsGridView.LIST.Rows[i][j]  = false;
                                }
                                else
                                {
                                    dsGridView.LIST.Rows[i][j]  = Boolean.Parse(reader[j - 1].ToString());
                                }
                            }
                            catch { }
                        }
                        else if (dsGridView.LIST.Columns[j].DataType.Equals(Type.GetType("System.String")))
                        {
                            try
                            {
                                if (dsGridView.LIST.Columns[j].ColumnName.Contains("dt"))
                                {
                                    dsGridView.LIST.Rows[i][j]  = Convert.ToDateTime(reader[j - 1]);
                                }
                                else
                                {
                                    dsGridView.LIST.Rows[i][j]  = reader[j - 1].ToString().Trim();
                                }
                            }
                            catch
                            {
                                try
                                {
                                    dsGridView.LIST.Rows[i][j]  = "NULL";
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            /// NULLのとき"NULL"をセット
                            try
                            {
                                dsGridView.LIST.Rows[i][j]      = reader[j - 1];
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    dsGridView.LIST.Rows[i][j]  = "NULL";
                                }
                                catch { }
                            }
                        }
                    }
                }
                return true;
            }
            catch (SqlException ex)
            {
                ///TODO: エラー処理
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + sqlInfo.Command.CommandText, "CommonRead SqlDataFind");
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
            
        }
        /// <summary>
        /// 接続情報を編集する
        /// </summary>
        /// <param name="ServerNM"></param>
        /// <param name="LoginNM"></param>
        /// <param name="DatabaseNM"></param>
        /// <returns></returns>
        private string ConnectServer(string ServerNM, string LoginNM, string DatabaseNM)
        {
            string ServerDB         = string.Format(Constant.StrSQLDB, ServerNM, DatabaseNM);
            /// 本番サーバーでログイン情報が有るとき
            if (LoginNM != "")
            {
                /// ユーザーIDとパスワードを分離してログイン情報とする
                string UserName     = "";
                string PassName     = "";
                string[] splName    = LoginNM.Split('/');
                if (splName.Length > 1)
                {
                    UserName        = splName[0];
                    PassName        = splName[1];
                    ServerDB        = string.Format(Constant.StrLoginDB, ServerNM, DatabaseNM, UserName, PassName);
                }
            }
            return ServerDB;
        }
	}
}
