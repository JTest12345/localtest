using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Specialized;
using SLCommonLib.DataBase;
using System.Data.Common;
using GEICS.Database;

namespace GEICS
{
    public class ConnectQCIL : IDisposable
    {
        public enum ServerKB
        {
            Main,
            Sub,
        }

		private enum TypeGroup
		{
			SIDEVIEW,
			MAP,
			Lamp,
			GA,
			SMD3in1,
			X19,
			MPL,
			X83_385,
            NTSV_DMC,
			VOYAGER_CSP_093_KIRAMEKI,
            NLCV,
            KIRAMEKI,
        }
		
		public const string CONNECT_USER_ID = "sla05";
		public const string CONNECT_USER_PASS = "ZP3wnIC";

		public const string CONNECT_INLINE_USER_ID = "inline";
		public const string CONNECT_INLINE_USER_PASS = "R28uHta";

		public const string CONNECT_ID_MAP_AUTO = "inline";
		public const string CONNECT_PASS_MAP_AUTO = "R28uHta";

		public const string CONNECT_ID_MAP_OUT = "inline";
		public const string CONNECT_PASS_MAP_OUT = "R28uHta";

		public const string CONNECT_ID_MAP_HIGH = "inline";
		public const string CONNECT_PASS_MAP_HIGH = "R28uHta";

		public const string CONNECT_ID_SV_HIGH_KYO = "inline";
		public const string CONNECT_PASS_SV_HIGH_KYO = "R28uHta";

		public const string CONNECT_ID_SV_HIGH_MIK = "inline";
		public const string CONNECT_PASS_SV_HIGH_MIK = "R28uHta";

		public const string CONNECT_ID_SV_AUTO_AOI = "inline";
		public const string CONNECT_PASS_SV_AUTO_AOI = "R28uHta";

		public const string CONNECT_ID_SV_HIGH_AOI = "inline";
		public const string CONNECT_PASS_SV_HIGH_AOI = "R28uHta";

		public const string CONNECT_ID_MAP_HIGH_AOI = "inline";
		public const string CONNECT_PASS_MAP_HIGH_AOI = "R28uHta";

		public const string CONNECT_ID_MAP_AUTO_AOI = "inline";
		public const string CONNECT_PASS_MAP_AUTO_AOI = "R28uHta";

		public const string CONNECT_ID_LAMP_OUT_NMC = "inline";
		public const string CONNECT_PASS_LAMP_OUT_NMC = "R28uHta";

		public const string CONNECT_ID_CE = "inline";
		public const string CONNECT_PASS_CE = "R28uHta";

        public const string CONNECT_ID_NTSV_AOI = "inline";
        public const string CONNECT_PASS_NTSV_AOI = "R28uHta";

        public const string CONNECT_ID_NLCV = "inline";
        public const string CONNECT_PASS_NLCV = "R28uHta";

        public const string CONNECT_ID_KMC = "inline";
        public const string CONNECT_PASS_KMC = "R28uHta";


#if TEST || DEBUG
        public const string CONNECT_SERVER_DEBUG = @"SLA-0040-2\SQLEXPRESS";
		public const string CONNECT_ID_DEBUG = "inline";
		public const string CONNECT_PASS_DEBUG = "R28uHta";
#endif

		private static Dictionary<string, string> userIDList = new Dictionary<string, string>()
		{
			{"試用サーバ", "inline"},
			{"SIDEVIEW合理化/3in1自動搬送", "sla05"},
            //{"MAP自動搬送", "inline"},
            //{"MAP高生産性", "inline"},
            {"MAP合理化", "inline"},
            {"3in1高生産性", "inline"},
			{"19/SHP高生産性", "inline"},
			{"MPL合理化", "inline"},
			{"83/385/48/COB高生産性", "inline"},
			//{"MAP高生産性(アオイ)", "inline"},
			//{"MAP自動搬送(アオイ)", "inline"},
            {"MAP合理化(アオイ)", "inline"},
            {"SIDEVIEW合理化(京都)", "inline"},
			{"3in1合理化(京都)", "inline"},
			//{"SIDEVIEW高生産性(三方)", "inline"},
			//{"SIDEVIEW自動搬送(三方)", "inline"},
            {"SIDEVIEW合理化(三方)", "inline"},
            {"LampOut(NMC)", "inline"},
			//{"GA自動搬送", "inline"},
			//{"GA高生産性", "inline"},
            {"GA合理化", "inline"},
            //{"MAP自動搬送（シチズン境川）", "inline"},
            //{"MAP高生産性(シチズン境川)", "inline"},
            {"MAP合理化(シチズン境川)", "inline"},
            {"COB高生産性(シチズン)", "inline"},
            {"NTSV/DMC合理化", "inline"},
            {"NTSV合理化(アオイ)", "inline"},
            //{"NLCV-DMC", "inline"},
            {"NLCV-導光板", "inline"},
            {"NLCV-セグメント", "inline"},
            {"NLCV-二次実装", "inline"},
        };

		private static Dictionary<string, string> userPassList = new Dictionary<string, string>()
		{
			{"試用サーバ", "R28uHta"},
			{"SIDEVIEW合理化/3in1自動搬送", "ZP3wnIC"},
			//{"MAP自動搬送", "R28uHta"},
			//{"MAP高生産性", "R28uHta"},
            {"MAP合理化", "R28uHta"},
            {"3in1高生産性", "R28uHta"},
			{"19/SHP高生産性", "R28uHta"},
			{"MPL合理化", "R28uHta"},
			{"83/385/48/COB高生産性", "R28uHta"},
			{"SIDEVIEW自動搬送(アオイ)", "R28uHta"},
			{"SIDEVIEW高生産性(アオイ)", "R28uHta"},
			//{"MAP高生産性(アオイ)", "R28uHta"},
			//{"MAP自動搬送(アオイ)", "R28uHta"},
            {"MAP合理化(アオイ)", "R28uHta"},
            {"SIDEVIEW合理化(京都)", "R28uHta"},
			{"3in1合理化(京都)", "R28uHta"},
			//{"SIDEVIEW高生産性(三方)", "R28uHta"},
			//{"SIDEVIEW自動搬送(三方)", "R28uHta"},
            {"SIDEVIEW合理化(三方)", "R28uHta"},
            {"LampOut(NMC)", "R28uHta"},
			//{"GA自動搬送", "R28uHta"},
			//{"GA高生産性", "R28uHta"},
            {"GA合理化", "R28uHta"},
            //{"MAP自動搬送（シチズン境川）", "R28uHta"},
            //{"MAP高生産性(シチズン境川)", "R28uHta"},
            {"MAP合理化(シチズン境川)", "R28uHta"},
            {"COB高生産性(シチズン)", "R28uHta"},
            {"NTSV/DMC合理化", "R28uHta"},
            {"NTSV合理化(アオイ)", "R28uHta"},
            //{"NLCV-DMC", "R28uHta"},
            {"NLCV-導光板", "R28uHta"},
            {"NLCV-セグメント", "R28uHta"},
            {"NLCV-二次実装", "R28uHta"},
        };

		private static Dictionary<string, string> server2Type = new Dictionary<string, string>()
		{
			{"試用サーバ", TypeGroup.SIDEVIEW.ToString()},
			{"SIDEVIEW合理化/3in1自動搬送", TypeGroup.SIDEVIEW.ToString()},
			//{"MAP自動搬送", TypeGroup.MAP.ToString()},
			//{"MAP高生産性", TypeGroup.MAP.ToString()},
            {"MAP合理化", TypeGroup.MAP.ToString()},
            {"3in1高生産性", TypeGroup.SMD3in1.ToString()},
			{"19/SHP高生産性", TypeGroup.X19.ToString()},
			{"MPL合理化", TypeGroup.MPL.ToString()},
			{"83/385/48/COB高生産性", TypeGroup.MPL.ToString()},
			//{"MAP高生産性(アオイ)", TypeGroup.MAP.ToString()},
			//{"MAP自動搬送(アオイ)", TypeGroup.MAP.ToString()},
            {"MAP合理化(アオイ)", TypeGroup.MAP.ToString()},
            {"SIDEVIEW合理化(京都)", TypeGroup.SIDEVIEW.ToString()},
			{"3in1合理化(京都)", TypeGroup.SMD3in1.ToString()},
			//{"SIDEVIEW高生産性(三方)", TypeGroup.SIDEVIEW.ToString()},
			//{"SIDEVIEW自動搬送(三方)", TypeGroup.SIDEVIEW.ToString()},
            {"SIDEVIEW合理化(三方)", TypeGroup.SIDEVIEW.ToString()},
            {"LampOut(NMC)", TypeGroup.Lamp.ToString()},
			//{"GA自動搬送", TypeGroup.GA.ToString()},
			//{"GA高生産性", TypeGroup.GA.ToString()},
            {"GA合理化", TypeGroup.GA.ToString()},
            //{"MAP自動搬送（シチズン境川）", TypeGroup.MAP.ToString()},
            //{"MAP高生産性(シチズン境川)", TypeGroup.MAP.ToString()},
            {"MAP合理化(シチズン境川)", TypeGroup.MAP.ToString()},
            {"COB高生産性(シチズン)", TypeGroup.MPL.ToString()},
            {"NTSV/DMC合理化", TypeGroup.NTSV_DMC.ToString()},
            {"VOYAGER/CSP/R-PKG/N-KIRAMEKI/A-MAP高生産性", TypeGroup.VOYAGER_CSP_093_KIRAMEKI.ToString()},
            {"NTSV合理化(アオイ)", TypeGroup.NTSV_DMC.ToString()},
            //{"NLCV-DMC", TypeGroup.NLCV.ToString()},
            {"NLCV-導光板", TypeGroup.NLCV.ToString()},
            {"NLCV-セグメント", TypeGroup.NLCV.ToString()},
            {"NLCV-二次実装", TypeGroup.NLCV.ToString()},
            {"KIRAMEKI高生産性(小糸製作所)", TypeGroup.KIRAMEKI.ToString()},
        };

        /// <summary>コネクション</summary>
        private SLCommonLib.DataBase.DBConnect connection;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="beginTrans"></param>
        /// <param name="server"></param>
        public ConnectQCIL(bool beginTrans, string conStr)
        {
            try
            {
                this.connection = SLCommonLib.DataBase.DBConnect.CreateInstance(conStr, "System.Data.SqlClient", beginTrans);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        public static string GetConnectionString(ServerKB servKB, string serverNM, string databaseNM)
        {
            string connStr = string.Empty;
            switch (servKB)
            {
                case ServerKB.Main:
                    connStr = string.Format(GEICS.Properties.Settings.Default.ConnectionString_QCIL_MAIN, serverNM, databaseNM, CONNECT_USER_ID, CONNECT_USER_PASS);
                    break;
                case ServerKB.Sub:
                    connStr = string.Format(GEICS.Properties.Settings.Default.ConnectionString_QCIL_SUB, CONNECT_USER_ID, CONNECT_USER_PASS);
                    break;
            }            
            return connStr;
        }
        public string GetConnectionString(ServerKB servKB)
        {
            return GetConnectionString(servKB, string.Empty, string.Empty);
        }
        public string GetConnectionString(ServerKB servKB, string serverNM) 
        {
            return GetConnectionString(servKB, serverNM, string.Empty);
        }

		public static string GetQCILConnString(string serverNM, string connStrFmt)
		{
			string userID, userPass;
			if (userIDList.ContainsKey(serverNM))
				userID = userIDList[serverNM];
			else
				userID = CONNECT_INLINE_USER_ID;

			if (userPassList.ContainsKey(serverNM))
				userPass = userPassList[serverNM];
			else
				userPass = CONNECT_INLINE_USER_PASS;

			return string.Format(connStrFmt, userID, userPass);
		}

		public static string GetQCILConnStrFromServerNM(string serverNM)
		{
			string connStr = string.Empty;
			//app.configからサーバ名、接続文字列フォーマット情報を取得
			NameValueCollection serverList = SLCommonLib.Commons.Configuration.GetAppConfigNVC("ServerList");

			//選択されたサーバの接続文字列を取得
			for (int index = 0; index < serverList.Count; index++)
			{
				if (serverList[index] == serverNM)
				{
					connStr = ConnectQCIL.GetQCILConnString(serverNM, serverList.GetKey(index));
					break;
				}
			}

			return connStr;
		}

		public static string GetTypeGroup(string serverNM)
		{
			return server2Type[serverNM];
		}

        #region IDisposable メンバ

        public void Dispose()
        {
            if (this.connection != null)
            {
                this.connection.Dispose();
                this.connection = null;
            }
        }

        #endregion

        public static string GetARMSConnString(int lineCD)
        {
            using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                string sql = @" SELECT Server_NO
                                FROM dbo.TmServ AS TmServ 
                                WHERE (Del_FG = 0) AND (InLine_CD = @LineCD) ";

                SqlParameter param = new SqlParameter("@LineCD", DbType.Int32);
                param.Value = lineCD;
                conn.Command.Parameters.Add(param);

                conn.Command.CommandText = sql;
                object serverNO = conn.Command.ExecuteScalar();

                if (serverNO != null)
                {

                    string rServerNO = Convert.ToString(serverNO).Trim();
                    return string.Format(Constant.SQLite_ARMS, rServerNO);
                }
                else
                {
                    return "";
                }
            }
        }

        public static SortedList<int, QCLogData> GetQCItem(List<int> Process, int backNum, int lineCD, DateTime measureDT, string typeCD, int defectNO)
        {
            int nCnt = 0;

            SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();
            string sWhereSQL = ConnectQCIL.GETWhereSQLProcess(Process);

            string BaseSql = "SELECT TOP {0} NascaLot_NO,DParameter_VAL,Equipment_NO,Measure_DT,QcParam_NO  FROM TnLOG WITH(NOLOCK) " +
                             "WHERE Inline_CD ={1} AND Measure_DT<'{2}'  AND NascaLot_No <> '' AND Material_CD='{3}' ";
            string sqlCmdTxt = string.Format(BaseSql, backNum, lineCD, measureDT, typeCD);

            sqlCmdTxt = sqlCmdTxt + sWhereSQL + " ORDER BY Measure_DT";

            /*
            string BaseSql = "SELECT NascaLot_NO,DParameter_VAL,Equipment_NO,Measure_DT,QcParam_NO  FROM TnLOG WITH(NOLOCK) " +
                             "WHERE Inline_CD ={0} AND (Measure_DT>'{1}' AND Measure_DT<='{2}') AND NascaLot_No <> '' AND Material_CD='{3}' AND Equipment_NO='{4}'";
            string sqlCmdTxt = string.Format(BaseSql, _nLineCD, _dtMeasure.AddDays(-1), _dtMeasure, _sType, _sEquiNO);
            sqlCmdTxt = sqlCmdTxt + sWhereSQL + " ORDER BY Measure_DT ASC";
            */
            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    while (reader.Read())
                    {
                        QCLogData wQCLogData = new QCLogData();                                //こっちが正解
                        wQCLogData.EquiNO = Convert.ToString(reader["Equipment_NO"]).Trim();   //設備番号
                        wQCLogData.LotNO = Convert.ToString(reader["NascaLot_NO"]).Trim();     //Lot
                        wQCLogData.TypeCD = typeCD.Trim();                                     //Type
                        wQCLogData.MeasureDT = Convert.ToDateTime(reader["Measure_DT"]);       //計測日時
                        wQCLogData.Data = Convert.ToDouble(reader["DParameter_VAL"]);          //data
                        wQCLogData.Defect = defectNO;                                          //監視項目No
                        wQCLogData.QcprmNO = Convert.ToInt32(reader["QcParam_NO"]);            //監視項目No
                        cndDataItem.Add(nCnt, wQCLogData);
                        nCnt = nCnt + 1;
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return cndDataItem;
        }

        private static string GETWhereSQLProcess(List<int> wProcess)
        {
            string sWhereSql = "@";

            foreach (int nProcess in wProcess)
            {
                sWhereSql = sWhereSql + " OR QcParam_NO = " + nProcess;
            }

            sWhereSql = sWhereSql.Replace("@ OR", "AND (");
            sWhereSql = sWhereSql + ")";

            if (sWhereSql == "@)")
            {
                sWhereSql = "";
            }

            return sWhereSql;
        }

        /// <summary>
        /// ダイス数量を取得
        /// </summary>
        /// <param name="typeCD"></param>
        /// <returns></returns>
        public static int GetDiceCount(string typeCD) 
        {
            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" SELECT Die_CT
                                    FROM TmDIECT with(nolock)
                                    WHERE (Material_CD = @Material_CD) AND (Del_FG = 0) ";

                    SqlParameter param = new SqlParameter("@Material_CD", SqlDbType.Char);
                    param.Value = typeCD;
                    conn.Command.Parameters.Add(param);

                    conn.Command.CommandText = sql;

                    object diceCount = conn.Command.ExecuteScalar();
                    if (diceCount == null)
                    {
                        return int.MinValue;
                    }
                    else 
                    {
                        return Convert.ToInt32(diceCount);
                    }
                }
            }
            catch (Exception err) 
            {
                throw err;
            }
        }

        /// <summary>
        /// 装置分類を取得
        /// </summary>
        /// <returns></returns>
        public static NameValueCollection GetAssets()
        {
            SqlDataReader rd = null;
            NameValueCollection nvc = new NameValueCollection();

            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" SELECT Assets_NM
                                    FROM dbo.TmEQUI AS TmEQUI WITH(nolock)
                                    WHERE (Del_FG = 0) 
                                    GROUP BY Assets_NM ";

                    conn.Command.CommandText = sql;

                    using (rd = conn.Command.ExecuteReader())
                    {
                        if (!rd.HasRows)
                        {
                            throw new Exception(Constant.MessageInfo.Message_71);
                        }
                        while (rd.Read())
                        {
                            nvc.Add(rd.GetString(rd.GetOrdinal("Assets_NM")), rd.GetString(rd.GetOrdinal("Assets_NM")));
                        }
                    }
                }
                return nvc;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmDIECTに追加
        /// </summary>
        /// <param name="typeCD">型番</param>
        /// <param name="diceCount">ダイス数量</param>
        public static void InsertTmDIECT(string typeCD, int diceCount)
        {
            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" INSERT INTO TmDIECT(Material_CD, Die_CT, UpdUser_CD, Bump_FG, Del_FG, LastUpd_DT)
                                VALUES(@MaterialCD, @DiceCount, '5175', 0, 0, @LastUpdDT) ";

                    SqlParameter param = new SqlParameter("@MaterialCD", SqlDbType.Char);
                    param.Value = typeCD;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@DiceCount", SqlDbType.Int);
                    param.Value = diceCount;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@LastUpdDT", SqlDbType.DateTime);
                    param.Value = System.DateTime.Now;
                    conn.Command.Parameters.Add(param);

                    conn.Command.CommandText = sql;
#if TEST
#else
                    conn.Command.ExecuteNonQuery();
#endif
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmDIECTを更新
        /// </summary>
        /// <param name="typeCD">型番</param>
        /// <param name="diceCount">ダイス数量</param>
        public static void UpdateTmDIECT(string typeCD, int diceCount)
        {
            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" UPDATE TmDIECT
                                    SET Die_CT = @DiceCount, LastUpd_DT = @LastUpdDT
                                    WHERE (Material_CD = @MaterialCD) ";

                    SqlParameter param = new SqlParameter("@MaterialCD", SqlDbType.Char);
                    param.Value = typeCD;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@DiceCount", SqlDbType.Int);
                    param.Value = diceCount;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@LastUpdDT", SqlDbType.DateTime);
                    param.Value = System.DateTime.Now;
                    conn.Command.Parameters.Add(param);

                    conn.Command.CommandText = sql;
#if TEST
#else
                    conn.Command.ExecuteNonQuery();
#endif
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmUSERFUNCTION(使用可能機能)を取得
        /// </summary>
        /// <param name="typeCD"></param>
        /// <returns></returns>
        public static List<UserFunctionInfo> GetUserFunction(string employeeCD)
        {
            SqlDataReader rd = null;
            List<UserFunctionInfo> userFunctionList = new List<UserFunctionInfo>();
            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" SELECT TmUSERFUNCTION.Employee_CD, TmUSERFUNCTION.Function_CD, TmFUNCTION.Function_NM
                                    FROM dbo.TmUSERFUNCTION AS TmUSERFUNCTION WITH(nolock)
                                    INNER JOIN dbo.TmFUNCTION AS TmFUNCTION WITH(nolock) ON TmUSERFUNCTION.Function_CD = TmFUNCTION.Function_CD
                                    WHERE (TmFUNCTION.Del_FG = 0) AND (TmUSERFUNCTION.Employee_CD = @EmployeeCD) ";

                    SqlParameter param = new SqlParameter("@EmployeeCD", SqlDbType.Char);
                    param.Value = employeeCD;
                    conn.Command.Parameters.Add(param);
                    conn.Command.CommandText = sql;

                    using (rd = conn.Command.ExecuteReader()) 
                    {
                        while(rd.Read())
                        {
                            UserFunctionInfo userFunctionInfo = new UserFunctionInfo();
                            userFunctionInfo.EmployeeCD = rd.GetString(rd.GetOrdinal("Employee_CD")).Trim();
                            userFunctionInfo.FunctionCD = rd.GetString(rd.GetOrdinal("Function_CD")).Trim();
                            userFunctionInfo.FunctionNM = rd.GetString(rd.GetOrdinal("Function_NM")).Trim();
                            userFunctionList.Add(userFunctionInfo);
                        }
                    }
                }
                return userFunctionList;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmUSERFUNCTION(使用可能機能)を追加
        /// </summary>
        public static void InsertUserFunction(string empCD, string functionCD, string updUserCD) 
        {
            if (empCD == "" || functionCD == "" || updUserCD == "") 
            {
                throw new Exception(Constant.MessageInfo.Message_73);
            }

            try 
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" INSERT INTO TmUSERFUNCTION(Employee_CD, Function_CD, UpdUser_CD)
                                    VALUES (@EmpCD, @FunctionCD, @UpdUserCD) ";

                    SqlParameter param = new SqlParameter("@EmpCD", SqlDbType.Char);
                    param.Value = empCD;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@FunctionCD", SqlDbType.Char);
                    param.Value = functionCD;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@UpdUserCD", SqlDbType.Char);
                    param.Value = updUserCD;
                    conn.Command.Parameters.Add(param);
                    
                    conn.Command.CommandText = sql;
#if TEST
#else
                    conn.Command.ExecuteNonQuery();
#endif
                }
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmUSERFUNCTION(使用可能機能)を削除
        /// </summary>
        public static void DeleteUserFunction(string empCD, string functionCD)
        {
            if (empCD == "" || functionCD == "")
            {
                throw new Exception(Constant.MessageInfo.Message_73);
            }

            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" DELETE FROM dbo.TmUSERFUNCTION
                                    WHERE employee_cd = @EmpCD and function_cd = @FunctionCD ";

                    SqlParameter param = new SqlParameter("@EmpCD", SqlDbType.Char);
                    param.Value = empCD;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@FunctionCD", SqlDbType.Char);
                    param.Value = functionCD;
                    conn.Command.Parameters.Add(param);

                    conn.Command.CommandText = sql;
#if TEST
#else
                    conn.Command.ExecuteNonQuery();
#endif
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmFUNCTION(機能)を取得
        /// </summary>
        /// <param name="typeCD"></param>
        /// <returns></returns>
        public static NameValueCollection GetFunction()
        {
            SqlDataReader rd = null;
            NameValueCollection nvc = new NameValueCollection();

            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" SELECT TmFUNCTION.Function_CD, TmFUNCTION.Function_NM
                                    FROM dbo.TmFUNCTION AS TmFUNCTION WITH(nolock)
                                    WHERE (TmFUNCTION.Del_FG = 0) ";

                    conn.Command.CommandText = sql;

                    using (rd = conn.Command.ExecuteReader())
                    {
                        if (!rd.HasRows) 
                        {
                            throw new Exception(Constant.MessageInfo.Message_74);
                        }
                        while (rd.Read())
                        {
                            nvc.Add(rd.GetString(rd.GetOrdinal("Function_CD")).Trim(), rd.GetString(rd.GetOrdinal("Function_NM")).Trim());
                        }
                    }
                }
                return nvc;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmFNM(定型文)取得
        /// </summary>
        /// <param name="assetsNM"></param>
        /// <returns></returns>
        public static List<FNMInfo> GetFNM(string assetsNM) 
        {
            SqlDataReader rd = null;
            List<FNMInfo> fnmList = new List<FNMInfo>();

            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" SELECT Assets_NM, Fixed_NO, Fixed_NM, UpdUser_CD, Del_FG, LastUpd_DT
                                    FROM dbo.TmFNM AS TmFNM WITH(nolock) ";

                    if (assetsNM != "") 
                    {
                        sql += " WHERE Assets_NM = @AssetsNM ";
                        SqlParameter param = new SqlParameter("@AssetsNM", SqlDbType.VarChar);
                        param.Value = assetsNM;
                        conn.Command.Parameters.Add(param);
                    }

                    conn.Command.CommandText = sql;

                    using (rd = conn.Command.ExecuteReader())
                    {
                        if (!rd.HasRows)
                        {
                            throw new Exception(Constant.MessageInfo.Message_72);
                        }
                        while (rd.Read())
                        {
                            FNMInfo fnmInfo = new FNMInfo();
                            fnmInfo.AssetsNM = rd.GetString(rd.GetOrdinal("Assets_NM"));
                            fnmInfo.FixedNO = rd.GetInt32(rd.GetOrdinal("Fixed_NO"));
                            fnmInfo.FixedNM = rd.GetString(rd.GetOrdinal("Fixed_NM"));
                            fnmInfo.DelFG = rd.GetBoolean(rd.GetOrdinal("Del_FG"));
                            fnmInfo.UpdUserCD = rd.GetString(rd.GetOrdinal("UpdUser_CD"));
                            fnmInfo.LastUpdDT = rd.GetDateTime(rd.GetOrdinal("LastUpd_DT"));
                            fnmList.Add(fnmInfo);
                        }
                    }
                }
                return fnmList;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmFNM(定型文)を追加
        /// </summary>
        /// <param name="assetsNM"></param>
        /// <param name="fixedNO"></param>
        /// <param name="fixedNM"></param>
        /// <param name="updUserCD"></param>
        public static void InsertFNM(string assetsNM, int fixedNO, string fixedNM, string updUserCD)
        {
            if (assetsNM == "" || fixedNO == 0 || fixedNM == "" || updUserCD == "")
            {
                throw new Exception(Constant.MessageInfo.Message_73);
            }

            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" INSERT INTO TmFNM(Assets_NM, Fixed_NO, Fixed_NM, UpdUser_CD)
                                    VALUES (@AssetsNM, @Fixed_NO, @Fixed_NM, @UpdUserCD) ";

                    SqlParameter param = new SqlParameter("@AssetsNM", SqlDbType.VarChar);
                    param.Value = assetsNM;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@Fixed_NO", SqlDbType.Int);
                    param.Value = fixedNO;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@Fixed_NM", SqlDbType.VarChar);
                    param.Value = fixedNM;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@UpdUserCD", SqlDbType.Char);
                    param.Value = updUserCD;
                    conn.Command.Parameters.Add(param);

                    conn.Command.CommandText = sql;
#if TEST
#else
                    conn.Command.ExecuteNonQuery();
#endif
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmFNM(定型文)を更新
        /// </summary>
        /// <param name="assetsNM"></param>
        /// <param name="fixedNO"></param>
        /// <param name="fixedNM"></param>
        /// <param name="updUserCD"></param>
        public static void UpdateFNM(string assetsNM, int fixedNO, string fixedNM, string updUserCD, bool delFG)
        {
            if (assetsNM == "" || fixedNO == 0 || fixedNM == "" || updUserCD == "")
            {
                throw new Exception(Constant.MessageInfo.Message_73);
            }

            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" UPDATE TmFNM SET Fixed_NM = @Fixed_NM, UpdUser_CD = @UpdUserCD, Del_FG = @DelFG, LastUpd_DT = getdate()
                                    WHERE (Assets_NM = @AssetsNM) AND (Fixed_NO = @Fixed_NO) ";

                    SqlParameter param = new SqlParameter("@AssetsNM", SqlDbType.VarChar);
                    param.Value = assetsNM;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@Fixed_NO", SqlDbType.Int);
                    param.Value = fixedNO;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@Fixed_NM", SqlDbType.VarChar);
                    param.Value = fixedNM;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@UpdUserCD", SqlDbType.Char);
                    param.Value = updUserCD;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@DelFG", SqlDbType.Bit);
                    param.Value = delFG;
                    conn.Command.Parameters.Add(param);

                    conn.Command.CommandText = sql;
#if TEST
#else
                    conn.Command.ExecuteNonQuery();
#endif
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmFNM(定型文)FixedNOのMAXNOを取得
        /// </summary>
        /// <param name="assetsNM"></param>
        public static int GetFNMFixedNOMAX(string assetsNM) 
        {
            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" SELECT MAX(Fixed_NO) as maxNO
                                    FROM dbo.TmFNM AS TmFNM WITH(nolock)
                                    WHERE (Assets_NM = @AssetsNM) AND (Del_FG = 0) ";

                    SqlParameter param = new SqlParameter("@AssetsNM", SqlDbType.VarChar);
                    param.Value = assetsNM;
                    conn.Command.Parameters.Add(param);

                    conn.Command.CommandText = sql;

                    object maxNO = conn.Command.ExecuteScalar();
                    if (maxNO == null)
                    {
                        //新規レコード
                        return 1;
                    }
                    else
                    {
                        return Convert.ToInt32(maxNO)+1;
                    }
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// 管理番号から監視番号を取得
        /// </summary>
        /// <param name="qcParamNO"></param>
        /// <returns></returns>
        public static int GetInspectionNO(int qcParamNO)
        {
            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" SELECT Inspection_NO
                                    FROM dbo.TmQsub AS TmQsub WITH(nolock)
                                    WHERE (Process_NO Like @QcParamNO) AND (Del_FG = 0) ";

                    SqlParameter param = new SqlParameter("@QcParamNO", SqlDbType.NVarChar);
                    param.Value = "%," + qcParamNO + ",%";
                    conn.Command.Parameters.Add(param);

                    conn.Command.CommandText = sql;

                    object inspNO = conn.Command.ExecuteScalar();
                    if (inspNO == null)
                    {
                        return int.MinValue;
                    }
                    else 
                    {
                        return Convert.ToInt32(inspNO);
                    }
                }
            }
            catch (Exception err) 
            {
                throw err;
            }
        }

        /// <summary>
        /// 打点数を取得
        /// </summary>
        /// <param name="typeCD"></param>
        /// <param name="qcParamNO"></param>
        /// <returns></returns>
        public static int GetQCnumVAL(string typeCD, int inspectionNO, int qcnumNO)
        {
            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT QCnum_VAL
                                    FROM dbo.TmQCST AS TmQCST WITH(nolock)
                                    WHERE (Del_FG = 0) AND (USE_FG = 1)
                                    AND (Inspection_NO = @InspectionNO) AND (Material_CD = @MaterialCD) AND (QCnum_NO = @QCnumNO)";

                    conn.SetParameter("@InspectionNO", SqlDbType.Int, inspectionNO);
                    conn.SetParameter("@MaterialCD", SqlDbType.Char, typeCD);
                    conn.SetParameter("@QCnumNO", SqlDbType.Int, qcnumNO);

                    object qcnumVAL = conn.ExecuteScalar(sql);
                    if (qcnumVAL == null)
                    {
                        return int.MinValue;
                    }
                    else
                    {
                        return Convert.ToInt32(qcnumVAL);
                    }
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TnQCNR(監視項目異常履歴)を取得
        /// </summary>
        /// <param name="measureDT"></param>
        /// <param name="lotNO"></param>
        /// <param name="inspectionNO"></param>
        /// <returns></returns>
        public static QCNRInfo GetQCNRInfo(DateTime measureDT, string lotNO, int inspectionNO)
        {
            SqlDataReader rd = null;
            QCNRInfo qcnrInfo = null;

            try
            {
                using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    string sql = @" SELECT Confirm_NM
                                  FROM dbo.TnQCNR AS TnQCNR WITH(nolock)
                                  INNER JOIN dbo.TnQCNRCnfm AS TnQCNRCnfm WITH(nolock) ON TnQCNR.Inline_CD = TnQCNRCnfm.Inline_CD AND TnQCNR.QCNR_NO = TnQCNRCnfm.QCNR_NO 
                                    WHERE (NascaLot_NO = @NascaLotNO) AND (Measure_DT = @MeasureDT) AND (Inspection_NO = @InspectionNO) ";

                    SqlParameter param = new SqlParameter("@NascaLotNO", SqlDbType.VarChar);
                    param.Value = lotNO;
                    conn.Command.Parameters.Add(param);
                    
                    param = new SqlParameter("@MeasureDT", SqlDbType.DateTime);
                    param.Value = measureDT;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@InspectionNO", SqlDbType.Int);
                    param.Value = inspectionNO;
                    conn.Command.Parameters.Add(param);

                    conn.Command.CommandText = sql;

                    using (rd = conn.Command.ExecuteReader())
                    {
                        if (!rd.HasRows)
                        {
                            //対策入力が未完了の場合
                            return null;
                        }

                        if (rd.Read())
                        {
                            qcnrInfo = new QCNRInfo();
                            qcnrInfo.ConfirmNM = rd.GetString(rd.GetOrdinal("Confirm_NM")).Replace("\r\n","。");
                        }      
                    }
                }
                return qcnrInfo;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TnDEFECT(不良履歴)を取得
        /// </summary>
        /// <param name="searchInfo"></param>
        /// <returns></returns>
        public static List<DefectInfo> GetDefectData(DefectSearchInfo searchInfo)
        {
            List<DefectInfo> defectList = new List<DefectInfo>();

            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {

                    string sql = @" SELECT TnDEFECT.Line_CD, TnDEFECT.Plant_CD, TnDEFECT.Lot_NO, TnDEFECT.DefAddress_NO, TnDEFECT.DefUnit_NO, TnDEFECT.Target_DT, 
                                    TnDEFECT.Work_CD, TnDEFECT.DefItem_CD, TnDEFECT.DefItem_NM, TnDEFECT.DefCause_CD, TnDEFECT.DefCause_NM, 
                                    TnDEFECT.DefClass_CD, TnDEFECT.DefClass_NM, TnDEFECT.Tran_CD, TnDEFECT.UpdateTran_CD, TnDEFECT.UpdUser_CD, 
                                    TnDEFECT.Del_FG, TnDEFECT.LastUpd_DT, TnDEFECTRESIN.Address_NO
                                    FROM TnDEFECT AS TnDEFECT WITH (nolock) 
                                    LEFT OUTER JOIN TnDEFECTRESIN ON TnDEFECT.Lot_NO = TnDEFECTRESIN.Lot_NO AND TnDEFECT.DefAddress_NO = TnDEFECTRESIN.Address_NO
                                    WHERE 1=1 ";

					if (searchInfo.IsTargetDelRecord == false)
					{
						sql += " AND TnDEFECT.Del_FG = 0 ";
					}

                    if (searchInfo.LineCD != 0) 
                    {
                        sql += " AND Line_CD Like @Line_CD ";
                        conn.SetParameter("@Line_CD", SqlDbType.Char, "%" + searchInfo.LineCD + "%");
                    }
                    if (searchInfo.PlantCD != "")
                    {
                        sql += " AND Plant_CD Like @Plant_CD ";
                        conn.SetParameter("@Plant_CD", SqlDbType.Char, "%" + searchInfo.PlantCD + "%");
                    }
                    if (searchInfo.LotNO != "")
                    {
                        sql += " AND TnDEFECT.Lot_NO Like @Lot_NO ";
                        conn.SetParameter("@Lot_NO", SqlDbType.VarChar, "%" + searchInfo.LotNO + "%");
                    }
                    if (searchInfo.TargetFromDT != null)
                    {
                        sql += " AND Target_DT >= @TargetFrom_DT AND Target_DT < @TargetTo_DT";
                        conn.SetParameter("@TargetFrom_DT", SqlDbType.DateTime, searchInfo.TargetFromDT);
                        conn.SetParameter("@TargetTo_DT", SqlDbType.DateTime, searchInfo.TargetToDT);
                    }
                    if (searchInfo.DefItemNM != "")
                    {
                        sql += " AND DefItem_NM Like @DefItem_NM ";
                        conn.SetParameter("@DefItem_NM", SqlDbType.NVarChar, "%" +searchInfo.DefItemNM + "%");
                    }

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            DefectInfo defectInfo = new DefectInfo();
                            defectInfo.LineCD = rd.GetInt32(rd.GetOrdinal("Line_CD"));
                            defectInfo.PlantCD = rd.GetString(rd.GetOrdinal("Plant_CD")).Trim();
                            defectInfo.LotNO = rd.GetString(rd.GetOrdinal("Lot_NO")).Trim();
                            defectInfo.DefAddressNO = rd.GetString(rd.GetOrdinal("DefAddress_NO")).Trim();
                            defectInfo.DefUnitNO = rd.GetString(rd.GetOrdinal("DefUnit_NO")).Trim();
                            defectInfo.TargetDT = rd.GetDateTime(rd.GetOrdinal("Target_DT"));
                            defectInfo.WorkCD = rd.GetString(rd.GetOrdinal("Work_CD")).Trim();
                            defectInfo.DefItemCD = rd.GetString(rd.GetOrdinal("DefItem_CD")).Trim();
                            defectInfo.DefItemNM = rd.GetString(rd.GetOrdinal("DefItem_NM")).Trim();
                            defectInfo.DefCauseCD = rd.GetString(rd.GetOrdinal("DefCause_CD")).Trim();
                            defectInfo.DefCauseNM = rd.GetString(rd.GetOrdinal("DefCause_NM")).Trim();
                            defectInfo.DefClassCD = rd.GetString(rd.GetOrdinal("DefClass_CD")).Trim();
                            defectInfo.DefClassNM = rd.GetString(rd.GetOrdinal("DefClass_NM")).Trim();
                            
                            int tranCDOrdinal = rd.GetOrdinal("Tran_CD");
                            if (!rd.IsDBNull(tranCDOrdinal))
                            {
                                defectInfo.TranCD = rd.GetString(tranCDOrdinal).Trim();
                            }

                            int updateTranCDOrdinal = rd.GetOrdinal("UpdateTran_CD");
                            if (!rd.IsDBNull(updateTranCDOrdinal))
                            {
                                defectInfo.UpdateTranCD = rd.GetString(updateTranCDOrdinal).Trim();
                            }
                            
                            defectInfo.UpdUserCD = rd.GetString(rd.GetOrdinal("UpdUser_CD")).Trim();
                            defectInfo.DelFG = rd.GetBoolean(rd.GetOrdinal("Del_FG"));
                            defectInfo.LastUpdDT = rd.GetDateTime(rd.GetOrdinal("LastUpd_DT"));

                            if (!rd.IsDBNull(rd.GetOrdinal("Address_NO"))) 
                            {
                                defectInfo.AddressCompareFG = true;
                            }

                            defectList.Add(defectInfo);
                        }
                    }
                }

                return defectList;
            }
            catch (Exception err) 
            {
                throw err;
            }
        }

        /// <summary>
        /// TmGENERAL(汎用)を取得
        /// </summary>
        /// <param name="generalGrpCD"></param>
        /// <returns></returns>
        public static List<GeneralInfo> GetGeneralData(string generalGrpCD) 
        {
            List<GeneralInfo> generalList = new List<GeneralInfo>();

            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT General_CD, General_NM
                                    FROM TmGENERAL WITH(nolock) 
                                    WHERE Del_FG = 0 AND (GeneralGrp_CD = @GeneralGrp_CD)";

                    conn.SetParameter("GeneralGrp_CD", SqlDbType.Char, generalGrpCD);

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            GeneralInfo generalInfo = new GeneralInfo();
                            generalInfo.GeneralCD = rd.GetString(rd.GetOrdinal("General_CD")).Trim();
                            generalInfo.GeneralNM = rd.GetString(rd.GetOrdinal("General_NM")).Trim();
                            generalList.Add(generalInfo);
                        }
                    }
                }

                return generalList;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmServ(中間PC)を取得
        /// </summary>
        /// <returns></returns>
        public static List<ServInfo> GetServData() 
        {
            List<ServInfo> servList = new List<ServInfo>();

            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT InLine_CD, Server_NO, Database_NM, MainServer_FG, SubServer_FG, TmGENERAL.General_NM
									FROM TmServ with(nolock) inner join TmGENERAL with(nolock) on TmServ.InLine_CD = TmGENERAL.General_CD
									WHERE (TmGENERAL.GeneralGrp_CD = @GeneralGrpCD) AND (TmServ.Del_FG = 0) And (SubServer_FG = 1) ";

					conn.SetParameter("@GeneralGrpCD", SqlDbType.NVarChar, General.GROUPCD_LINE);

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            ServInfo servInfo = new ServInfo();
                            servInfo.ServerCD = rd.GetString(rd.GetOrdinal("Server_NO")).Trim();
                            servInfo.ServerNM = rd.GetString(rd.GetOrdinal("General_NM")).Trim();
                            servInfo.DatabaseNM = rd.GetString(rd.GetOrdinal("Database_NM"));
                            servInfo.MainServerFG = rd.GetBoolean(rd.GetOrdinal("MainServer_FG"));
                            servInfo.SubServerFG = rd.GetBoolean(rd.GetOrdinal("SubServer_FG"));
                            servList.Add(servInfo);
                        }
                    }
                }

                return servList;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// フレームマスタ情報(TmFRAME)を取得
        /// </summary>
        /// <returns></returns>
        public static FRAMEInfo GetFRAMEData(string typeCD)
        {
            FRAMEInfo frameInfo = new FRAMEInfo();

            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT TmFRAME.Frame_NO, TmFILEFMTTYPE.Type_CD, TmFRAME.XPackage_CT, TmFRAME.YPackage_CT, TmFRAME.MagazineStep_CT,TmFRAME.MagazineStepMAX_CT
                            FROM TmFRAME WITH(nolock)
                            INNER JOIN TmFILEFMTTYPE WITH(nolock) ON TmFRAME.Frame_NO = TmFILEFMTTYPE.Frame_NO
                            WHERE (TmFILEFMTTYPE.Type_CD = @TypeCD) AND (TmFILEFMTTYPE.del_fg = 0) AND (TmFRAME.Del_FG = 0) ";

                    conn.SetParameter("@TypeCD", SqlDbType.Char, typeCD);

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        if (!rd.HasRows)
                        {
                            throw new Exception(string.Format(Constant.MessageInfo.Message_41, typeCD));
                        }

                        if (rd.Read())
                        {
                            frameInfo.FrameNO = rd.GetInt64(rd.GetOrdinal("Frame_NO"));
                            frameInfo.TypeCD = rd.GetString(rd.GetOrdinal("Type_CD")).Trim();
                            frameInfo.XPackageCT = rd.GetInt32(rd.GetOrdinal("XPackage_CT"));
                            frameInfo.YPackageCT = rd.GetInt32(rd.GetOrdinal("YPackage_CT"));
                            frameInfo.MagazineStepCT = rd.GetInt32(rd.GetOrdinal("MagazineStep_CT"));
                            frameInfo.MagazineStepMAXCT = rd.GetInt32(rd.GetOrdinal("MagazineStepMAX_CT"));
                            frameInfo.FramePackageCT = frameInfo.XPackageCT * frameInfo.YPackageCT;
                            frameInfo.MagazinPackageCT = frameInfo.FramePackageCT * frameInfo.MagazineStepCT;
                            frameInfo.MagazinPackageMAXCT = frameInfo.FramePackageCT * frameInfo.MagazineStepMAXCT;
                        }
                    }
                }

                return frameInfo;
            }
            catch (Exception err)
            {
                throw err;
            }

        }

        /// <summary>
        /// TnDEFECT(不良履歴)を更新
        /// </summary>
        /// <param name="defectInfo"></param>
        public void UpdateDefectInfo(DefectInfo defectInfo) 
        {
            try
            {
				string sql = @" UPDATE TnDEFECT SET Del_FG = @Del_FG, UpdUser_CD = @UpdUser_CD, LastUpd_DT = @LastUpd_DT ";

				this.connection.SetParameter("@Del_FG", SqlDbType.Bit, defectInfo.DelFG);
				this.connection.SetParameter("@UpdUser_CD", SqlDbType.Char, defectInfo.UpdUserCD);
				this.connection.SetParameter("@LastUpd_DT", SqlDbType.DateTime, DateTime.Now);

				if (string.IsNullOrEmpty(defectInfo.TranCD) == false)
				{
					sql += @" , Tran_CD = @Tran_CD ";
					this.connection.SetParameter("@Tran_CD", SqlDbType.Char, defectInfo.TranCD);
				}

				if (string.IsNullOrEmpty(defectInfo.UpdateTranCD) == false)
				{
					sql = @" , UpdateTran_CD = @UpdateTran_CD ";
					this.connection.SetParameter("@UpdateTran_CD", SqlDbType.Char, defectInfo.UpdateTranCD);
				}

				if (string.IsNullOrEmpty(defectInfo.UpdateDefAddressNO) == false)
				{
					sql += @" , DefAddress_NO = @UpdateDefAddress_NO ";
					this.connection.SetParameter("@UpdateDefAddress_NO", SqlDbType.VarChar, defectInfo.UpdateDefAddressNO);
				}

				string conditionSql = @" WHERE (Line_CD = @Line_CD) AND (Plant_CD = @Plant_CD) 
										   AND (Lot_NO = @Lot_NO) AND (DefAddress_NO = @DefAddress_NO) AND (DefUnit_NO = @DefUnit_NO) ";



                this.connection.SetParameter("@Line_CD", SqlDbType.Int, defectInfo.LineCD);
                this.connection.SetParameter("@Plant_CD", SqlDbType.Char, defectInfo.PlantCD);
                this.connection.SetParameter("@Lot_NO", SqlDbType.VarChar, defectInfo.LotNO);
                this.connection.SetParameter("@DefAddress_NO", SqlDbType.VarChar, defectInfo.DefAddressNO);

                this.connection.SetParameter("@DefUnit_NO", SqlDbType.VarChar, defectInfo.DefUnitNO);

				sql += conditionSql;
#if TEST
				this.connection.ExecuteNonQuery(sql);
#else
                this.connection.ExecuteNonQuery(sql);
#endif
            }
            catch (Exception err)
            {
                this.Connection.Rollback();
                throw err;
            }
        }



        public static List<ParamInfo> GetPRMData() 
        {
            return GetPRMData(string.Empty, int.MinValue); 
        }
        public static ParamInfo GetPRMData(int qcParamNO) 
        {
            return GetPRMData(string.Empty, qcParamNO)[0];
        }
        public static List<ParamInfo> GetPRMData(string parameterNM, int qcParamNO) 
        {
            List<ParamInfo> paramList = new List<ParamInfo>();

            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT TmPRM.QcParam_NO, TmPRM.Model_NM, TmPRM.Chip_NM, TmPRM.Die_KB, TmPRM.Class_NM, TmPRM.Parameter_NM, 
                                    TmPRM.Manage_NM, TmPRM.Timing_NO, TmPRM.ChangeUnit_VAL, TmPRM.Total_KB, TmPRM.EquipManage_FG,
                                    TmTIM.Timing_NM,
                                    TmPRMInfo.Info1_NM, TmPRMInfo.Info2_NM, TmPRMInfo.Info3_NM, TmPRM.UnManageTrend_FG, TmPRM.WithoutFileFmt_FG, 
                                    TmPRM.ResinGroupManage_FG
                                    FROM TmPRM with(nolock) 
                                    INNER JOIN TmPRMInfo with(nolock) ON TmPRM.QcParam_NO = TmPRMInfo.QcParam_NO
                                    INNER JOIN TmTIM ON TmPRM.Timing_NO = TmTIM.Timing_NO
                                    WHERE (TmPRM.Del_FG = 0) AND (TmPRMInfo.Del_FG = 0) ";

                    if (!string.IsNullOrEmpty(parameterNM))
                    {
                        sql += " AND (TmPRM.Parameter_NM like @ParameterNM) ";
                        conn.SetParameter("@ParameterNM", SqlDbType.VarChar, parameterNM + "%");
                    }
                    if (qcParamNO != int.MinValue)
                    {
                        sql += " AND (TmPRM.QcParam_NO = @QcParamNO) ";
                        conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNO);
                    }

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
						int ordEquipManageFG = rd.GetOrdinal("EquipManage_FG");
						int ordChipNM = rd.GetOrdinal("Chip_NM");
						int ordDieKB = rd.GetOrdinal("Die_KB");
						int ordInfo1 = rd.GetOrdinal("Info1_NM");
						int ordInfo2 = rd.GetOrdinal("Info2_NM");
						int ordInfo3 = rd.GetOrdinal("Info3_NM");
						int ordUnManageFg = rd.GetOrdinal("UnManageTrend_FG");
						int ordWithoutFg = rd.GetOrdinal("WithoutFileFmt_FG");
                        int ordResinGroupManageFG = rd.GetOrdinal("ResinGroupManage_FG");

                        while (rd.Read())
                        {
                            ParamInfo paramInfo = new ParamInfo();
                            paramInfo.QcParamNO = rd.GetInt32(rd.GetOrdinal("QcParam_NO"));
                            paramInfo.ModelNM = rd.GetString(rd.GetOrdinal("Model_NM"));
                            
                            if (!rd.IsDBNull(ordChipNM))
                            {
                                paramInfo.ChipNM = rd.GetString(ordChipNM);
                            }
                            
                            if (!rd.IsDBNull(ordDieKB))
                            {
                                paramInfo.DieKB = rd.GetString(ordDieKB);
                            }

                            paramInfo.ClassNM = rd.GetString(rd.GetOrdinal("Class_NM"));
                            paramInfo.ParameterNM = rd.GetString(rd.GetOrdinal("Parameter_NM"));
                            paramInfo.ManageNM = rd.GetString(rd.GetOrdinal("Manage_NM"));
                            paramInfo.TimingNM = rd.GetString(rd.GetOrdinal("Timing_NM"));
                            paramInfo.ChangeUnitVAL = rd.GetString(rd.GetOrdinal("ChangeUnit_VAL"));
							paramInfo.EquipManageFG = rd.GetInt32(ordEquipManageFG);
							paramInfo.UnManageTrendFG = rd.GetBoolean(ordUnManageFg);
							paramInfo.WithoutFileFmtFG = rd.GetBoolean(ordWithoutFg);

                            if (!rd.IsDBNull(ordInfo1))
                            {
                                paramInfo.Info1NM = rd.GetString(ordInfo1);
                            }
                            
                            if (!rd.IsDBNull(ordInfo2))
                            {
                                paramInfo.Info2NM = rd.GetString(ordInfo2);
                            }
                            
                            if (!rd.IsDBNull(ordInfo3))
                            {
                                paramInfo.Info3NM = rd.GetString(ordInfo3);
                            }
                            paramInfo.ResinGroupManageFG = rd.GetInt32(ordResinGroupManageFG);

                            paramList.Add(paramInfo);
                        }
                    }
                }

				if (paramList.Count == 0)
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_101, parameterNM, qcParamNO));
				}

                return paramList;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static List<FileFmtTypeInfo> GetFILEFMTTYPEData(string typeCD)
        {
            List<FileFmtTypeInfo> fileFmtTypeList = new List<FileFmtTypeInfo>();

            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT Type_CD, FileFmt_NO, Frame_NO, Del_FG, UpdUser_CD, LastUpd_DT
                                    FROM TmFILEFMTTYPE WITH (nolock)
                                    WHERE (Del_FG = 0) ";

                    if (typeCD != string.Empty)
                    {
                        sql += " AND (TmFILEFMTTYPE.Type_CD = @TypeCD) ";
                        conn.SetParameter("@TypeCD", SqlDbType.Char, typeCD);
                    }

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            FileFmtTypeInfo fileFmtTypeInfo = new FileFmtTypeInfo();
                            fileFmtTypeInfo.TypeCD = rd.GetString(rd.GetOrdinal("Type_CD"));
                            fileFmtTypeInfo.OldTypeCD = fileFmtTypeInfo.TypeCD;

                            fileFmtTypeInfo.FileFmtNO = rd.GetInt32(rd.GetOrdinal("FileFmt_NO"));
                            fileFmtTypeInfo.FrameNO = rd.GetInt64(rd.GetOrdinal("Frame_NO"));
                            fileFmtTypeInfo.DelFG = rd.GetBoolean(rd.GetOrdinal("Del_FG"));
                            fileFmtTypeInfo.UpdUserCD = rd.GetString(rd.GetOrdinal("UpdUser_CD"));
                            fileFmtTypeInfo.LastUpdDT = rd.GetDateTime(rd.GetOrdinal("LastUpd_DT"));
                            fileFmtTypeList.Add(fileFmtTypeInfo);
                        }
                    }
                }

                return fileFmtTypeList;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        #region TmPLM

        /// <summary>
        /// TmPLM(閾値マスタ)に登録されている型番を取得
        /// </summary>
        public static List<string> GetPLMTypeCD(bool includeDelRecord)
        {
            return GetPLMTypeCD(includeDelRecord, null, null);
        }

        /// <summary>
        /// TmPLM(閾値マスタ)に登録されている型番を取得
        /// </summary>
        public static List<string> GetPLMTypeCD(bool includeDelRecord, bool? isResinGroupManageCondition, bool? isProgMat)
        {
            List<string> typeList = new List<string>();

            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT Material_CD 
                            FROM TmPLM WITH(NOLOCK)
                            WHERE 1 = 1 ";
					
					if(includeDelRecord == false)
					{
						sql += " AND Del_FG = 0 ";
					}

                    if (isResinGroupManageCondition.HasValue)
                    {
                        sql += " AND QcParam_NO IN ( SELECT QcParam_NO FROM TmPRM WITH(NOLOCK) WHERE ResinGroupManage_FG = @ResinGroupManageFG ) ";
                        conn.SetParameter("@ResinGroupManageFG", SqlDbType.Int, Common.ParseBoolToInt(isResinGroupManageCondition.Value));
                    }

                    if (isProgMat.HasValue)
                    {
                        sql += " AND QcParam_NO IN ( SELECT QcParam_NO FROM TmPRM WITH(NOLOCK) WHERE ProgramMaterialCd_FG = @ProgramMaterialCd_FG ) ";
                        conn.SetParameter("@ProgramMaterialCd_FG", SqlDbType.Int, isProgMat.Value);
                    }

                    sql += " GROUP BY Material_CD ";
                            
                    //ORDER BY Material_CD ";

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            typeList.Add(rd.GetString(rd.GetOrdinal("Material_CD")).Trim());
                        }
                    }
                }

				return typeList.OrderBy(t => t).ToList();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmPLM(閾値マスタ)に登録されている樹脂Grを取得
        /// </summary>
        public static List<string> GetPLMResinGroupCD()
        {
            List<string> resinGroupList = new List<string>();

            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT ResinGroup_CD 
                            FROM TmPLM WITH(NOLOCK) 
                            GROUP BY ResinGroup_CD ";
                    
                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            resinGroupList.Add(rd.GetString(rd.GetOrdinal("ResinGroup_CD")).Trim());
                        }
                    }
                }

                return resinGroupList.OrderBy(t => t).ToList();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// 特定のテーブルに登録されている特定の列情報を取得
        /// </summary>
        public static List<string> GetElementbyString(string ElementNM, string TableNM)
        {
            List<string> ElementList = new List<string>();

            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql_org = @" SELECT {0} 
                            FROM {1} WITH(NOLOCK) 
                            GROUP BY {0}
                            ORDER BY {0} ";

                    string sql = String.Format(sql_org, ElementNM, TableNM);

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            ElementList.Add(rd.GetString(rd.GetOrdinal(ElementNM)).Trim());
                        }
                    }
                }
                return ElementList;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// 特定のテーブルに登録されている特定の列情報を取得
        /// </summary>
        public static List<string> GetElementbyInt(string ElementNM, string TableNM)
        {
            List<string> ElementList = new List<string>();

            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql_org = @" SELECT {0} 
                            FROM {1} WITH(NOLOCK) 
                            GROUP BY {0}
                            ORDER BY {0} ";

                    string sql = String.Format(sql_org, ElementNM, TableNM);

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            ElementList.Add(rd.GetInt32(rd.GetOrdinal(ElementNM)).ToString());
                        }
                    }
                }
                return ElementList;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// TmFILEFMTTYPE_WB(ファイル紐付マスタ)に登録されているPRM情報(管理番号 + 管理名)を取得
        /// </summary>
        public static NameValueCollection GetPrmFromFilefmtWB()
        {
            NameValueCollection nvc = new NameValueCollection();
            
            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT QcParam_NO, Parameter_NM 
                            FROM TmPRM WITH(NOLOCK) 
                            WHERE Exists(
                                    SELECT *
                                    FROM TmFILEFMT_WB WITH(NOLOCK)
                                    WHERE QcParam_NO = TmPRM.QcParam_NO
                                    ) ";

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            int QcParam_NO;
                            string Parameter_NM;
                            QcParam_NO = rd.GetInt32(rd.GetOrdinal("QcParam_NO"));
                            Parameter_NM = rd.GetString(rd.GetOrdinal("Parameter_NM"));
                            nvc.Add(QcParam_NO.ToString(), Parameter_NM);
                        }
                    }
                }
                return nvc;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

		public static Dictionary<int, PlmInfo> GetPLMData(List<string> typeList, List<int> qcParamList, string modelNM, bool includeDelRecordFG)
		{
			Dictionary<int, PlmInfo> PlmDict = new Dictionary<int, PlmInfo>();
			List<string> strQcParamNoList = new List<string>();

			foreach (int qcParamNo in qcParamList)
			{
				if (!strQcParamNoList.Contains(qcParamNo.ToString()))
				{
					strQcParamNoList.Add(qcParamNo.ToString());
				}
			}
			List<PlmInfo> PlmList = GetPLMData(typeList, strQcParamNoList, modelNM, includeDelRecordFG, null, null, null);

			foreach (PlmInfo plmInfo in PlmList)
			{
                if (PlmDict.Where(p => p.Key == plmInfo.QcParamNO).Any())
                    continue;

				PlmDict.Add(plmInfo.QcParamNO, plmInfo);
			}

			return PlmDict;

		}

        /// <summary>
        /// TmPLM(閾値マスタ)を取得
        /// </summary>
        /// <param name="typeCD">製品型番</param>
        /// <param name="qcParamNO">管理NO</param>
        /// <returns></returns>
        public static List<PlmInfo> GetPLMData(List<string> typeList, List<string> qcParamList, string modelNM, bool includeDelRecordFG, bool? resinGroupManage_FG, List<string> resinGroupList, bool? progMatFg)
        {
            List<PlmInfo> plmList = new List<PlmInfo>();
            
            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT TmPRM.Chip_NM, TmPRM.Class_NM, TmPRM.Parameter_NM, TmPRM.Manage_NM, 
                                    TmTIM.Timing_NM,
                                    TmPLM.Model_NM, TmPLM.QcParam_NO, TmPLM.Material_CD, TmPLM.Parameter_MAX, TmPLM.Parameter_MIN, TmPLM.Parameter_VAL, 
                                    TmPLM.QcLine_MAX, TmPLM.QcLine_MIN, TmPLM.AimLine_VAL, TmPLM.AimRate_VAL, TmPLM.Del_FG, TmPLM.UpdUser_CD, TmPLM.LastUpd_DT,
                                    TmPRMInfo.Info1_NM, TmPRMInfo.Info2_NM, TmPRMInfo.Info3_NM, TmPLM.InnerUpperLimit, TmPLM.InnerLowerLimit, TmPLM.Equipment_NO,
									TmPRM.EquipManage_FG, TmPRM.UnManageTrend_FG, TmPRM.WithoutFileFmt_FG, TmPRM.ResinGroupManage_FG, TmPLM.ResinGroup_CD,
                                    TmPLM.ParamGetUpperCond, TmPLM.ParamGetLowerCond, TmPRM.ProgramMaterialCd_FG
                                    FROM  dbo.TmPRM WITH (nolock) 
                                    INNER JOIN dbo.TmPRMInfo WITH (nolock) ON dbo.TmPRM.QcParam_NO = dbo.TmPRMInfo.QcParam_NO 
                                    INNER JOIN dbo.TmPLM WITH (nolock) ON dbo.TmPRM.QcParam_NO = dbo.TmPLM.QcParam_NO 
                                    INNER JOIN dbo.TmTIM WITH (nolock) ON dbo.TmPRM.Timing_NO = dbo.TmTIM.Timing_NO 
                                    WHERE (TmPRM.Del_FG = 0) AND (TmPRMInfo.Del_FG = 0) ";

                    if (typeList.Count != 0) 
                    {
                        sql += " AND TmPLM.Material_CD IN " + Common.GetMultiSql(typeList);
                    }
                    if (qcParamList.Count != 0)
                    {
                        sql += " AND TmPLM.QcParam_NO IN " + Common.GetMultiSql(qcParamList);
                    }
					//検索に削除済みレコードを含まない場合
					if (includeDelRecordFG == false)
					{
						sql += " AND (TmPLM.Del_FG = 0) ";
					}

					if (!string.IsNullOrEmpty(modelNM))
					{
						sql += " AND (TmPLM.Model_NM = @ModelNM) ";
						conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM);
                    }
                    if (resinGroupList != null && resinGroupList.Count != 0)
                    {
                        sql += " AND TmPLM.ResinGroup_CD IN " + Common.GetMultiSql(resinGroupList);
                    }
                    if (resinGroupManage_FG.HasValue)
                    {
                        sql += " AND (TmPRM.ResinGroupManage_FG = @ResinGroupManageFG) ";
                        conn.SetParameter("@ResinGroupManageFG", SqlDbType.Int, Common.ParseBoolToInt(resinGroupManage_FG.Value));
                    }
                    if (progMatFg.HasValue)
                    {
                        sql += " AND (TmPRM.ProgramMaterialCd_FG = @ProgramMaterialCdFG) ";
                        conn.SetParameter("@ProgramMaterialCdFG", SqlDbType.Bit, progMatFg.Value);
                    }
                    //if (typeCD != string.Empty)
                    //{
                    //    sql += " AND (TmPLM.Material_CD = @MaterialCD) ";
                    //    conn.SetParameter("@MaterialCD", SqlDbType.Char, typeCD);
                    //}
                    //if (qcParamNO != int.MinValue)
                    //{
                    //    sql += " AND (TmPLM.QcParam_NO = @QcParamNO) ";
                    //    conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNO);
                    //}

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
						int ordChipNM = rd.GetOrdinal("Chip_NM");
						int ordParameterInfo2NM = rd.GetOrdinal("Info2_NM");
						int ordParameterInfo3NM = rd.GetOrdinal("Info3_NM");
						int ordEquipManageFG = rd.GetOrdinal("EquipManage_FG");
						int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
						int ordParameterVAL = rd.GetOrdinal("Parameter_VAL");
						int ordParameterMAX = rd.GetOrdinal("Parameter_MAX");
						int ordParameterMIN = rd.GetOrdinal("Parameter_MIN");
						int ordQcLineMAX = rd.GetOrdinal("QcLine_MAX");
						int ordQcLineMIN = rd.GetOrdinal("QcLine_MIN");
						int ordInnerUpperLimit = rd.GetOrdinal("InnerUpperLimit");
						int ordInnerLowerLimit = rd.GetOrdinal("InnerLowerLimit");
						int ordUnManageFG = rd.GetOrdinal("UnManageTrend_FG");
						int ordWithoutFG = rd.GetOrdinal("WithoutFileFmt_FG");
                        int ordResinGroupManageFG = rd.GetOrdinal("ResinGroupManage_FG");
                        int ordResinGroupCD = rd.GetOrdinal("ResinGroup_CD");
                        int ordParamGetUpperCond = rd.GetOrdinal("ParamGetUpperCond");
                        int ordParamGetLowerCond = rd.GetOrdinal("ParamGetLowerCond");
                        int ordProgramMaterialCdFG = rd.GetOrdinal("ProgramMaterialCd_FG");

                        while (rd.Read())
                        {
                            PlmInfo plmInfo = new PlmInfo();
                            plmInfo.ModelNM = rd.GetString(rd.GetOrdinal("Model_NM")).Trim();
                            plmInfo.QcParamNO = rd.GetInt32(rd.GetOrdinal("QcParam_NO"));
                            plmInfo.MaterialCD = rd.GetString(rd.GetOrdinal("Material_CD")).Trim();
                                                        
                            if (!rd.IsDBNull(ordChipNM))
                            {
                                plmInfo.ChipNM = rd.GetString(ordChipNM).Trim();
                            }                        
                            
                            plmInfo.ClassNM = rd.GetString(rd.GetOrdinal("Class_NM")).Trim();
                            plmInfo.ParameterNM = rd.GetString(rd.GetOrdinal("Parameter_NM")).Trim();

                            if (!rd.IsDBNull(ordParameterVAL))
                            {
                                plmInfo.ParameterVAL = rd.GetString(ordParameterVAL).Trim();
                            }

                            plmInfo.ManageNM = rd.GetString(rd.GetOrdinal("Manage_NM")).Trim();
                            plmInfo.Info1NM = rd.GetString(rd.GetOrdinal("Info1_NM")).Trim();

							if (!rd.IsDBNull(ordParameterInfo2NM))
							{
								plmInfo.Info2NM = rd.GetString(ordParameterInfo2NM).Trim();
							}

                            if (!rd.IsDBNull(ordParameterInfo3NM))
                            {
                                plmInfo.Info3NM = rd.GetString(ordParameterInfo3NM).Trim();
                            }
                            plmInfo.TimingNM = rd.GetString(rd.GetOrdinal("Timing_NM")).Trim();

							plmInfo.EquipmentNO = rd.GetString(ordEquipmentNO).Trim();

                            if (!rd.IsDBNull(ordParameterMAX))
                            {
                                plmInfo.ParameterMAX = rd.GetDecimal(ordParameterMAX);
                            }

                            if (!rd.IsDBNull(ordParameterMIN))
                            {
                                plmInfo.ParameterMIN = rd.GetDecimal(ordParameterMIN);
                            }

							if (!rd.IsDBNull(ordQcLineMAX))
                            {
                                plmInfo.QcLineMAX = rd.GetDecimal(ordQcLineMAX);
                            }                  
                          
							if (!rd.IsDBNull(ordQcLineMIN))
                            {
                                plmInfo.QcLineMIN = rd.GetDecimal(ordQcLineMIN);
                            }

							plmInfo.DelFG = rd.GetBoolean(rd.GetOrdinal("Del_FG"));
                            plmInfo.UpdUserCD = rd.GetString(rd.GetOrdinal("UpdUser_CD"));
                            plmInfo.LastUpdDT = rd.GetDateTime(rd.GetOrdinal("LastUpd_DT"));

                            if (!rd.IsDBNull(ordInnerUpperLimit))
                            {
                                plmInfo.InnerUpperLimit = rd.GetDecimal(ordInnerUpperLimit);
                            }

                            if (!rd.IsDBNull(ordInnerLowerLimit))
                            {
                                plmInfo.InnerLowerLimit = rd.GetDecimal(ordInnerLowerLimit);
                            }

                            if (!rd.IsDBNull(ordParamGetUpperCond))
                            {
                                plmInfo.ParamGetUpperCond = rd.GetFloat(ordParamGetUpperCond);
                            }

                            if (!rd.IsDBNull(ordParamGetLowerCond))
                            {
                                plmInfo.ParamGetLowerCond = rd.GetFloat(ordParamGetLowerCond);
                            }

                            plmInfo.EquipManageFG = rd.GetInt32(ordEquipManageFG);

                            plmInfo.ResinGroupManageFG = rd.GetInt32(ordResinGroupManageFG);
                            plmInfo.ResinGroupCD = rd.GetString(ordResinGroupCD);
                            plmInfo.ProgramMaterialCdFG =  rd.GetBoolean(ordProgramMaterialCdFG);
                            
                            plmList.Add(plmInfo);
                        }
                    }
                }
                return plmList;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static List<PlmInfo> GetPLMData(List<string> typeList, bool includeDelRecordFG) 
        {
            List<string> qcParamList = new List<string>();

			return GetPLMData(typeList, qcParamList, string.Empty, includeDelRecordFG, null, null, null);
        }
        public static List<PlmInfo> GetPLMData(string typeCD, bool includeDelRecordFG) 
        {
            List<string> typeList = new List<string>();
            typeList.Add(typeCD);

            return GetPLMData(typeList, includeDelRecordFG);
        }
     
        /// <summary>
        /// TmPLM(閾値マスタ)追加更新
        /// </summary>
        /// <param name="plmInfo"></param>
        public void InsertUpdatePLM(PlmInfo plmInfo)
        {

            if (string.IsNullOrEmpty(plmInfo.ModelNM) || plmInfo.QcParamNO == 0 || string.IsNullOrEmpty(plmInfo.MaterialCD))
            {
                throw new ApplicationException(string.Format(
                    Constant.MessageInfo.Message_88, plmInfo.ModelNM, plmInfo.QcParamNO, plmInfo.MaterialCD));
            }

			if (plmInfo.EquipManageFG != 0 && string.IsNullOrEmpty(plmInfo.EquipmentNO))
			{
				throw new ApplicationException(string.Format("設備CD別管理が【必要】なパラメータで【設備CD指定の無い】規格が存在します。タイプ:{0} / パラメータ番号:{1} / パラメータ名:{2}\r\n",
					plmInfo.MaterialCD, plmInfo.QcParamNO, plmInfo.ParameterNM));
			}
			else if (plmInfo.EquipManageFG == 0 && string.IsNullOrEmpty(plmInfo.EquipmentNO) == false)
			{
				throw new ApplicationException(string.Format("設備CD別管理が【不要】なパラメータで【設備CD指定のある】規格が存在します。　タイプ:{0} / パラメータ番号:{1} / パラメータ名:{2}\r\n",
					plmInfo.MaterialCD, plmInfo.QcParamNO, plmInfo.ParameterNM));
			}

            string sql = @" UPDATE TmPLM SET 
                                Parameter_MAX = @ParameterMAX, Parameter_MIN = @ParameterMIN, Parameter_VAL = @ParameterVAL, 
                                QcLine_MAX = @QcLineMAX, QcLine_MIN = @QcLineMIN, AimLine_VAL = @AimLineVAL, AimRate_VAL = @AimRateVAL, 
                                Del_FG = @DelFG, UpdUser_CD = @UpdUserCD, LastUpd_DT = @LastUpdDT,
                                InnerUpperLimit = @InnerUpperLimit, InnerLowerLimit = @InnerLowerLimit, 
                                ParamGetUpperCond = @ParamGetUpperCond, ParamGetLowerCond = @ParamGetLowerCond
                                WHERE Model_NM = @ModelNM AND QcParam_NO = @QcParamNO AND Material_CD = @MaterialCD AND Equipment_NO = @EquipmentNO
                                AND ResinGroup_CD = @ResinGroupCD
                            INSERT INTO TmPLM (Model_NM, QcParam_NO, Material_CD, Equipment_NO, Parameter_MAX, Parameter_MIN, Parameter_VAL,
                                               QcLine_MAX, QcLine_MIN, AimLine_VAL, AimRate_VAL, UpdUser_CD,
                                               InnerUpperLimit, InnerLowerLimit, ParamGetUpperCond, ParamGetLowerCond, ResinGroup_CD)
                                SELECT @ModelNM, @QcParamNO, @MaterialCD, @EquipmentNO,
                                    @ParameterMAX, @ParameterMIN, @ParameterVAL, 
                                    @QcLineMAX, @QcLineMIN, @AimLineVAL, @AimRateVAL, @UpdUserCD,
                                    @InnerUpperLimit, @InnerLowerLimit, @ParamGetUpperCond, @ParamGetLowerCond, @ResinGroupCD
                                WHERE NOT EXISTS 
                                    (SELECT * FROM TmPLM 
                                    WHERE Model_NM = @ModelNM AND QcParam_NO = @QcParamNO AND 
										  Material_CD = @MaterialCD AND Equipment_NO = @EquipmentNO AND ResinGroup_CD = @ResinGroupCD) ";

            this.connection.SetParameter("@ModelNM", SqlDbType.VarChar, Common.GetParameterValue(plmInfo.ModelNM));
            this.connection.SetParameter("@QcParamNO", SqlDbType.Int, Common.GetParameterValue(plmInfo.QcParamNO));
            this.connection.SetParameter("@MaterialCD", SqlDbType.Char, Common.GetParameterValue(plmInfo.MaterialCD));
			this.connection.SetParameter("@EquipmentNO", SqlDbType.VarChar, plmInfo.EquipmentNO.ToUpper());
            this.connection.SetParameter("@ParameterMAX", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.ParameterMAX));
            this.connection.SetParameter("@ParameterMIN", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.ParameterMIN));
            this.connection.SetParameter("@ParameterVAL", SqlDbType.Char, Common.GetParameterValue(plmInfo.ParameterVAL));
            this.connection.SetParameter("@QcLineMAX", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.QcLineMAX));
            this.connection.SetParameter("@QcLineMIN", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.QcLineMIN));
            this.connection.SetParameter("@AimLineVAL", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.AimLineVAL));
            this.connection.SetParameter("@AimRateVAL", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.AimRateVAL));
            this.connection.SetParameter("@DelFG", SqlDbType.Bit, plmInfo.DelFG);
            this.connection.SetParameter("@UpdUserCD", SqlDbType.Char, Constant.EmployeeInfo.EmployeeCD);
            this.connection.SetParameter("@LastUpdDT", SqlDbType.DateTime, System.DateTime.Now);
            this.connection.SetParameter("@InnerUpperLimit", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.InnerUpperLimit));
            this.connection.SetParameter("@InnerLowerLimit", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.InnerLowerLimit));
            this.connection.SetParameter("@ParamGetUpperCond", SqlDbType.Real, Common.GetParameterValue(plmInfo.ParamGetUpperCond));
            this.connection.SetParameter("@ParamGetLowerCond", SqlDbType.Real, Common.GetParameterValue(plmInfo.ParamGetLowerCond));
            this.connection.SetParameter("@ResinGroupCD", SqlDbType.VarChar, plmInfo.ResinGroupCD);
#if TEST
            this.connection.ExecuteNonQuery(sql);
#else
            this.connection.ExecuteNonQuery(sql);
#endif

        }

        #endregion

        #region TmPLMHist

        public static List<PlmInfo> GetPLMHistData(string typeCD, bool isLatestRevData)
        {
            return GetPLMHistData(typeCD, string.Empty, int.MinValue, string.Empty, null, isLatestRevData);
        }

        public static List<PlmInfo> GetPLMHistData(string typeCD, string modelNM, int qcParamNO)
        {
            return GetPLMHistData(typeCD, modelNM, qcParamNO, string.Empty, null, false);
        }

        public static List<PlmInfo> GetPLMHistData(string typeCD, string modelNM, int qcParamNO, string equipNO, string resinGroupCd)
        {
            return GetPLMHistData(typeCD, modelNM, qcParamNO, equipNO, resinGroupCd, false);
        }

        public static List<PlmInfo> GetPLMHistData(string typeCD, string modelNM, int qcParamNO, string equipNO, string resinGroupCd, bool IsLastestRevData)
        {
            List<PlmInfo> plmHistList = new List<PlmInfo>();

            try
            {
                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {

                    string sql = @" SELECT TmPLMHist.Rev_NO, TmPLMHist.Reason_NM, TmPRM.Class_NM, TmPRM.Parameter_NM, TmPRM.Manage_NM, 
                                    TmTIM.Timing_NM,
                                    TmPLMHist.Model_NM, TmPLMHist.QcParam_NO, TmPLMHist.Material_CD, TmPLMHist.Parameter_MAX, TmPLMHist.Parameter_MIN, TmPLMHist.Parameter_VAL, 
                                    TmPLMHist.QcLine_MAX, TmPLMHist.QcLine_MIN, TmPLMHist.QcLine_PNT, TmPLMHist.AimLine_VAL, TmPLMHist.AimRate_VAL, TmPLMHist.UpdUser_CD, TmPLMHist.LastUpd_DT,
                                    TmPRMInfo.Info1_NM, TmPRMInfo.Info2_NM, TmPRMInfo.Info3_NM, TmPLMHist.InnerUpperLimit, TmPLMHist.InnerLowerLimit,
									TmPLMHist.Equipment_NO, TmPRM.ResinGroupManage_FG, TmPLMHist.ResinGroup_CD
                                    FROM  dbo.TmPRM WITH (nolock) 
                                    INNER JOIN dbo.TmPRMInfo WITH (nolock) ON dbo.TmPRM.QcParam_NO = dbo.TmPRMInfo.QcParam_NO 
                                    INNER JOIN dbo.TmPLMHist WITH (nolock) ON dbo.TmPRM.QcParam_NO = dbo.TmPLMHist.QcParam_NO 
                                    INNER JOIN dbo.TmTIM WITH (nolock) ON dbo.TmPRM.Timing_NO = dbo.TmTIM.Timing_NO 
                                    WHERE (TmPLMHist.Del_FG = 0) AND (TmPRM.Del_FG = 0) AND (TmPRMInfo.Del_FG = 0) ";
                                       //AND dbo.TmPRM.Model_NM = dbo.TmPLMHist.Model_NM 
                    if (typeCD != string.Empty)
                    {
                        sql += " AND (TmPLMHist.Material_CD = @MaterialCD) ";
                        conn.SetParameter("@MaterialCD", SqlDbType.Char, typeCD);
                    }
                    if (modelNM != string.Empty)
                    {
                        sql += " AND (TmPLMHist.Model_NM = @ModelNM) ";
                        conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM);
                    }
                    if (qcParamNO != int.MinValue)
                    {
                        sql += " AND (TmPLMHist.QcParam_NO = @QcParamNO) ";
                        conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNO);
                    }
					if (equipNO != string.Empty)
					{
						sql += " AND (TmPLMHist.Equipment_NO = @EquipmentNO) ";
						conn.SetParameter("@EquipmentNO", SqlDbType.VarChar, equipNO);
                    }
                    if (string.IsNullOrEmpty(resinGroupCd) == false)
                    {
                        sql += " AND (TmPLMHist.ResinGroup_CD = @ResinGroupCD) ";
                        conn.SetParameter("@ResinGroupCD", SqlDbType.VarChar, resinGroupCd);
                    }

                    if (IsLastestRevData)
					{
						sql += @" AND EXISTS(SELECT MAX(TmPLMHist2.Rev_NO) FROM TmPLMHist AS TmPLMHist2 WITH(NOLOCK)
								  WHERE
									(TmPLMHist2.Del_FG = 0) AND (TmPLMHist2.Material_CD = @Material_CD) AND 
									(TmPLMHist.Model_NM = TmPLMHist2.Model_NM) AND
									(TmPLMHist.QcParam_NO = TmPLMHist2.QcParam_NO) AND
									(TmPLMHist.Material_CD = TmPLMHist2.Material_CD) AND
									(TmPLMHist.Equipment_NO = TmPLMHist2.Equipment_NO)AND
									(TmPLMHist.ResinGroup_CD = TmPLMHist2.ResinGroup_CD) ";
						conn.SetParameter("@Material_CD", SqlDbType.VarChar, typeCD);
									
					}

                    sql += " ORDER BY TmPLMHist.Rev_NO ";

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
						int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
                        int ordResinGroupManageFG = rd.GetOrdinal("ResinGroupManage_FG");
                        int ordResinGroupCD = rd.GetOrdinal("ResinGroup_CD");
                        while (rd.Read())
                        {
                            PlmInfo plmInfo = new PlmInfo();
                            
                            plmInfo.ModelNM = rd.GetString(rd.GetOrdinal("Model_NM")).Trim();
                            plmInfo.QcParamNO = rd.GetInt32(rd.GetOrdinal("QcParam_NO"));
                            plmInfo.MaterialCD = rd.GetString(rd.GetOrdinal("Material_CD")).Trim();
                            plmInfo.ClassNM = rd.GetString(rd.GetOrdinal("Class_NM")).Trim();
                            plmInfo.ParameterNM = rd.GetString(rd.GetOrdinal("Parameter_NM")).Trim();

                            int ordParameterVAL = rd.GetOrdinal("Parameter_VAL");
                            if (!rd.IsDBNull(ordParameterVAL))
                            {
                                plmInfo.ParameterVAL = rd.GetString(ordParameterVAL).Trim();
                            }

                            plmInfo.ManageNM = rd.GetString(rd.GetOrdinal("Manage_NM")).Trim();
                            plmInfo.Info1NM = rd.GetString(rd.GetOrdinal("Info1_NM")).Trim();
                            plmInfo.Info2NM = rd.GetString(rd.GetOrdinal("Info2_NM")).Trim();

                            int ordParameterInfo3NM = rd.GetOrdinal("Info3_NM");
                            if (!rd.IsDBNull(ordParameterInfo3NM))
                            {
                                plmInfo.Info3NM = rd.GetString(ordParameterInfo3NM).Trim();
                            }
                            plmInfo.TimingNM = rd.GetString(rd.GetOrdinal("Timing_NM")).Trim();

                            int ordParameterMAX = rd.GetOrdinal("Parameter_MAX");
                            if (!rd.IsDBNull(ordParameterMAX))
                            {
                                plmInfo.ParameterMAX = rd.GetDecimal(ordParameterMAX);
                            }
                            int ordParameterMIN = rd.GetOrdinal("Parameter_MIN");
                            if (!rd.IsDBNull(ordParameterMIN))
                            {
                                plmInfo.ParameterMIN = rd.GetDecimal(ordParameterMIN);
                            }
                            int ordQcLineMAX = rd.GetOrdinal("QcLine_MAX");
                            if (!rd.IsDBNull(ordQcLineMAX))
                            {
                                plmInfo.QcLineMAX = rd.GetDecimal(ordQcLineMAX);
                            }
                            int ordQcLineMIN = rd.GetOrdinal("QcLine_MIN");
                            if (!rd.IsDBNull(ordQcLineMIN))
                            {
                                plmInfo.QcLineMIN = rd.GetDecimal(ordQcLineMIN);
                            }
                            int ordQcLinePNT = rd.GetOrdinal("QcLine_PNT");
                            if (!rd.IsDBNull(ordQcLinePNT))
                            {
                                plmInfo.QcLinePNT = rd.GetInt32(ordQcLinePNT);
                            }

                            plmInfo.RevNO = rd.GetInt32(rd.GetOrdinal("Rev_NO"));

                            int ordReasonVAL = rd.GetOrdinal("Reason_NM");
                            if (!rd.IsDBNull(ordReasonVAL))
                            {
                                plmInfo.ReasonVAL = rd.GetString(ordReasonVAL).Trim();
                            }
                            plmInfo.UpdUserCD = rd.GetString(rd.GetOrdinal("UpdUser_CD"));
                            plmInfo.LastUpdDT = rd.GetDateTime(rd.GetOrdinal("LastUpd_DT"));

                            int ordInnerUpperLimit = rd.GetOrdinal("InnerUpperLimit");
                            if (!rd.IsDBNull(ordInnerUpperLimit))
                            {
                                plmInfo.InnerUpperLimit = rd.GetDecimal(ordInnerUpperLimit);
                            }
                            int ordInnerLowerLimit = rd.GetOrdinal("InnerLowerLimit");
                            if (!rd.IsDBNull(ordInnerLowerLimit))
                            {
                                plmInfo.InnerLowerLimit = rd.GetDecimal(ordInnerLowerLimit);
                            }

							plmInfo.EquipmentNO = rd.GetString(ordEquipmentNO).Trim();
                            plmInfo.ResinGroupManageFG = rd.GetInt32(ordResinGroupManageFG);
                            plmInfo.ResinGroupCD = rd.GetString(ordResinGroupCD).Trim();

                            plmHistList.Add(plmInfo);
                        }
                    }
                }
                return plmHistList;
            }
            catch (Exception err)
            {
                throw err;
            }
        }


        /// <summary>
        /// TmPLMHist(閾値履歴マスタ)追加
        /// </summary>
        /// <param name="plmInfo"></param>
        public void InsertPLMHist(PlmInfo plmInfo)
        {
			try
			{
				string indexSql = @" SELECT MAX(Rev_NO) FROM TmPLMHist WHERE (Model_NM = @ModelNM AND QcParam_NO = @QcParamNO AND Material_CD = @MaterialCD
									AND Equipment_NO = @EquipmentNO　AND ResinGroup_CD = @ResinGroupCD ) ";

				string sql = @" INSERT INTO TmPLMHist (Model_NM, QcParam_NO, Material_CD, Rev_NO,
                                               Parameter_MAX, Parameter_MIN, Parameter_VAL,
                                               QcLine_PNT, QcLine_MAX, QcLine_MIN, AimLine_VAL, AimRate_VAL, Reason_NM, UpdUser_CD,
                                               InnerUpperLimit, InnerLowerLimit, Equipment_NO, ResinGroup_CD)
                            VALUES (@ModelNM, @QcParamNO, @MaterialCD, @RevNO,
                                    @ParameterMAX, @ParameterMIN, @ParameterVAL, 
                                    @QcLinePNT, @QcLineMAX, @QcLineMIN, @AimLineVAL, @AimRateVAL, @ReasonVAL, @UpdUserCD,
                                    @InnerUpperLimit, @InnerLowerLimit, @EquipmentNO, @ResinGroupCD) ";

				this.connection.SetParameter("@ModelNM", SqlDbType.VarChar, Common.GetParameterValue(plmInfo.ModelNM));
				this.connection.SetParameter("@QcParamNO", SqlDbType.Int, Common.GetParameterValue(plmInfo.QcParamNO));
				this.connection.SetParameter("@MaterialCD", SqlDbType.Char, Common.GetParameterValue(plmInfo.MaterialCD));
				this.connection.SetParameter("@ParameterMAX", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.ParameterMAX));
				this.connection.SetParameter("@ParameterMIN", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.ParameterMIN));
				this.connection.SetParameter("@ParameterVAL", SqlDbType.Char, Common.GetParameterValue(plmInfo.ParameterVAL));
				this.connection.SetParameter("@QcLinePNT", SqlDbType.Int, Common.GetParameterValue(plmInfo.QcLinePNT));
				this.connection.SetParameter("@QcLineMAX", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.QcLineMAX));
				this.connection.SetParameter("@QcLineMIN", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.QcLineMIN));
				this.connection.SetParameter("@AimLineVAL", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.AimLineVAL));
				this.connection.SetParameter("@AimRateVAL", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.AimRateVAL));
				this.connection.SetParameter("@ReasonVAL", SqlDbType.NVarChar, Common.GetParameterValue(plmInfo.ReasonVAL));
				this.connection.SetParameter("@UpdUserCD", SqlDbType.Char, Constant.EmployeeInfo.EmployeeCD);
				this.connection.SetParameter("@InnerUpperLimit", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.InnerUpperLimit));
				this.connection.SetParameter("@InnerLowerLimit", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.InnerLowerLimit));

				if (plmInfo.EquipmentNO == null)
				{
					plmInfo.EquipmentNO = string.Empty;
				}

				this.connection.SetParameter("@EquipmentNO", SqlDbType.VarChar, plmInfo.EquipmentNO.ToUpper());
                if (plmInfo.ResinGroupCD == null)
                {
                    plmInfo.ResinGroupCD = string.Empty;
                }

                this.connection.SetParameter("@ResinGroupCD", SqlDbType.VarChar, plmInfo.ResinGroupCD);
#if TEST
                object revNO = this.connection.ExecuteScalar(indexSql);
                if (revNO == System.DBNull.Value)
                {
                    revNO = 0;
                }
                this.connection.SetParameter("@RevNO", SqlDbType.Int, Convert.ToInt32(revNO) + 1);

                this.connection.ExecuteNonQuery(sql);
#else
				object revNO = this.connection.ExecuteScalar(indexSql);
				if (revNO == System.DBNull.Value)
				{
					revNO = 0;
				}
				this.connection.SetParameter("@RevNO", SqlDbType.Int, Convert.ToInt32(revNO) + 1);

				this.connection.ExecuteNonQuery(sql);
#endif
            }
            catch (SqlException err)
			{
			}
			catch (Exception err)
			{

			}
		}

#endregion

        /// <summary>
        /// TmQCST(連続点閾値マスタ)追加更新
        /// </summary>
        /// <param name="plmInfo"></param>
        public void InsertUpdateQCST(PlmInfo plmInfo) 
        {
            if (string.IsNullOrEmpty(plmInfo.MaterialCD) || plmInfo.QcParamNO == 0 || plmInfo.QCnumNO == 0)
            {
                throw new ApplicationException(string.Format(
                    Constant.MessageInfo.Message_89, plmInfo.MaterialCD, plmInfo.InspectionNO, plmInfo.QCnumNO));
            }

            int inspectionNO = GetInspectionNO(plmInfo.QcParamNO);
            if (inspectionNO == int.MinValue) 
            {
                throw new Exception(string.Format(Constant.MessageInfo.Message_90, plmInfo.QcParamNO));
            }

            string sql = @" UPDATE TmQCST SET 
                                QCnum_VAL = @QCnumVAL, USE_FG = @UseFG, Del_FG = @DelFG, UpdUser_CD = @UpdUserCD, LastUpd_DT = @LastUpdDT 
                            WHERE Material_CD = @MaterialCD AND Inspection_NO = @InspectionNO AND QCnum_NO = @QCnumNO 
                            INSERT INTO TmQCST (Material_CD, Inspection_NO, QCnum_NO, QCnum_VAL, USE_FG, UpdUser_CD)
                            SELECT @MaterialCD, @InspectionNO, @QCnumNO, @QCnumVAL, @UseFG, @UpdUserCD
                            WHERE NOT EXISTS 
                                (SELECT * FROM TmQCST
                                 WHERE Material_CD = @MaterialCD AND Inspection_NO = @InspectionNO AND QCnum_NO = @QCnumNO) ";

            this.connection.SetParameter("@MaterialCD", SqlDbType.Char, Common.GetParameterValue(plmInfo.MaterialCD));
            this.connection.SetParameter("@InspectionNO", SqlDbType.Decimal, Common.GetParameterValue(inspectionNO));
            this.connection.SetParameter("@QCnumNO", SqlDbType.Decimal, Common.GetParameterValue(plmInfo.QCnumNO));
            this.connection.SetParameter("@QCnumVAL", SqlDbType.Char, Common.GetParameterValue(plmInfo.QcLinePNT));
            this.connection.SetParameter("@UseFG", SqlDbType.Bit, true);
            this.connection.SetParameter("@DelFG", SqlDbType.Bit, plmInfo.DelFG);
            this.connection.SetParameter("@UpdUserCD", SqlDbType.Char, Constant.EmployeeInfo.EmployeeCD);
            this.connection.SetParameter("@LastUpdDT", SqlDbType.DateTime, System.DateTime.Now);
#if TEST
			this.connection.ExecuteNonQuery(sql);
#else
            this.connection.ExecuteNonQuery(sql);
#endif
        }

        public static string GetDefectName(string defItemCD)
        {
            using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
            {
                string sql = @" SELECT DefItem_JA FROM TmFCNV WITH(NOLOCK) 
                                WHERE DefItem_CD = @DefItemCD ";

                conn.SetParameter("@DefItemCD", SqlDbType.NVarChar, defItemCD);

                object defItemNM = conn.ExecuteScalar(sql);
                if (defItemNM == null)
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_93, defItemCD));
                }
                else
                {
                    return Convert.ToString(defItemNM);
                }
            }
        }


        /// <summary>
        /// TmDIECT(ダイス個数マスタ)追加更新
        /// </summary>
        /// <param name="typeCD"></param>
        /// <param name="diceCount"></param>
        public void InsertUpdateDIECT(string typeCD, int diceCount) 
        {
			if (string.IsNullOrEmpty(typeCD) || diceCount == 0)
			{
				//List<Plm> plmList = Plm.GetDatas(this.connection, typeCD, null, true);

				//if (plmList.Count > 0)
				//{
					throw new ApplicationException(string.Format(
						Constant.MessageInfo.Message_92, typeCD));
				//}
				//else
				//{
				//    return;
				//}
			}

			string sql = @" UPDATE TmDIECT SET 
                            Die_CT = @DieCT, UpdUser_CD = @UpdUserCD, LastUpd_DT = @LastUpdDT 
                        WHERE Material_CD = @MaterialCD 
                        INSERT INTO TmDIECT (Material_CD, Die_CT, UpdUser_CD)
                        SELECT @MaterialCD, @DieCT, @UpdUserCD
                        WHERE NOT EXISTS 
                            (SELECT * FROM TmDIECT
                             WHERE Material_CD = @MaterialCD) ";

			this.connection.SetParameter("@MaterialCD", SqlDbType.Char, Common.GetParameterValue(typeCD));
			this.connection.SetParameter("@DieCT", SqlDbType.Int, Common.GetParameterValue(diceCount));
			this.connection.SetParameter("@UpdUserCD", SqlDbType.Char, Constant.EmployeeInfo.EmployeeCD);
			this.connection.SetParameter("@LastUpdDT", SqlDbType.DateTime, System.DateTime.Now);
#if TEST
			this.connection.ExecuteNonQuery(sql);
#else
			this.connection.ExecuteNonQuery(sql);
#endif

        }

        //public static int GetInspectionNO(int QcParamNO)
        //{
        //    int inspectionNO = int.MinValue;

        //    try
        //    {
        //        using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
        //        {
        //            string sql = " SELECT Inspection_NO, Process_NO TmQsub WHERE Del_FG = 0 ";

        //            using (DbDataReader rd = conn.GetReader(sql))
        //            {
        //                while (rd.Read())
        //                {
        //                    string processNO = rd.GetString(rd.GetOrdinal("Process_NO"));
        //                    List<string> inspectionNOList = processNO.Split(',').ToList();
        //                    if (inspectionNOList.Exists(i => i == processNO)) 
        //                    {
        //                        inspectionNO = rd.GetInt32(rd.GetOrdinal("Inspection_NO"));
        //                    }
        //                }
        //            }
        //        }

        //        if (inspectionNO == int.MinValue) 
        //        {
        //            throw new Exception(string.Format(Constant.MessageInfo.Message_90, QcParamNO));
        //        }

        //        return inspectionNO;
        //    }
        //    catch (Exception err)
        //    {
        //        throw err;
        //    }
        //}

		/// <summary>TmLSETから全lineCDを重複無く取得</summary>
		/// <param name="connStr"></param>
		/// <returns></returns>
		public static List<int> GetLineCDList(string connStr)
		{
			string sql = @" Select Inline_CD From TmLSET with(nolock) Group by Inline_CD ";
			List<int> lineCDList = new List<int>();

			using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(connStr, "System.Data.SqlClient", false))
			{
				using (DbDataReader rd = conn.GetReader(sql))
				{
					int ordLineCD = rd.GetOrdinal("Inline_CD");

					while (rd.Read())
					{
						int lineCD = rd.GetInt32(ordLineCD);

						lineCDList.Add(lineCD);
					}
				}
			}

			return lineCDList;
		}

#region プロパティ

        /// <summary>
        /// コネクション
        /// </summary>
        public SLCommonLib.DataBase.DBConnect Connection
        {
            get { return this.connection; }
        }

#endregion
    }
}
