using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using System.Diagnostics;
using System.Xml;
using log4net;
using System.Collections.Specialized;
using SLCommonLib.DataBase;
 
namespace GEICS
{
    public class ConnectARMS
    {
        /// <summary>
        /// ロット番号から製品型番を取得
        /// </summary>
        /// <param name="lotNO">ロット番号</param>
        /// <returns>製品型番</returns>
        public static string GetTypeCD(string lotNO)
        {
            string sql = @" SELECT t.typecd 
                        FROM tnlot t With(NOLOCK) 
                        WHERE t.lotno = @LOTNO ";

            using (IConnection conn = NascaConnection.CreateInstance(F04_ErrorRecord.StrARMS, false))
            {
                if (lotNO.Contains("_#"))
                {
                    lotNO = lotNO.Substring(0, lotNO.IndexOf("_#"));
                }
                SqlParameter param = new SqlParameter("@LotNO", DbType.String);
                param.Value = lotNO;
                conn.Command.Parameters.Add(param);

                conn.Command.CommandText = sql;
                object typeCD = conn.Command.ExecuteScalar();
                if (typeCD == System.DBNull.Value || typeCD == null)
                {
                    throw new Exception(string.Format(Constant.MessageInfo.Message_84, lotNO));
                }
                else
                {
                    return typeCD.ToString().Trim();
                }
            }
        }

		/// <summary>
        /// ロット・設備グリッド用データ取得
        /// </summary>
        /// <param name="sLot"></param>
        /// <returns></returns>
		public static List<LotStbInfo> SetTblLotStb_ARMS(string lotNO)
		{
			return SetTblLotStb_ARMS(lotNO, string.Empty);
		}

        /// <summary>
        /// ロット・設備グリッド用データ取得
        /// </summary>
        /// <param name="sLot"></param>
        /// <returns></returns>
        public static List<LotStbInfo> SetTblLotStb_ARMS(string lotNO, string assetsNM)
        {
            List<LotStbInfo> dicStbList = new List<LotStbInfo>();
            SqlDataReader rd = null;

            using (IConnection conn = NascaConnection.CreateInstance(F04_ErrorRecord.StrARMS, false))
            {
				string sql = @" SELECT startdt, tntran.lotno, tmmachine.plantcd, tmmachine.plantnm, tmmachine.clasnm
                    FROM tntran WITH(nolock) inner join tmmachine WITH(nolock) on tntran.macno = tmmachine.macno
                    WHERE lotno Like @LotNO
                    AND (tmmachine.macno not in (30001, 99999)) ";

                SqlParameter param = new SqlParameter("@LotNO", DbType.String);
                param.Value = lotNO + "%";
                conn.Command.Parameters.Add(param);

				if (!string.IsNullOrEmpty(assetsNM))
				{
					sql += " AND tmmachine.clasnm = @AssetsNM ";
					SqlParameter assetsNMParam = new SqlParameter("@AssetsNM", DbType.String);
					assetsNMParam.Value = assetsNM;
					conn.Command.Parameters.Add(assetsNMParam);
				}

				sql += @" GROUP BY startdt, tntran.lotno, tmmachine.plantcd, tmmachine.plantnm, tmmachine.clasnm
                    ORDER BY startdt OPTION (MAXDOP 1) ";

                try
                {
                    conn.Command.CommandText = sql;
                    rd = conn.Command.ExecuteReader();
                    while (rd.Read()) 
                    {
                        LotStbInfo lotStdInfo = new LotStbInfo();

                        lotStdInfo.LotNO = Convert.ToString(rd["lotno"]);
                        lotStdInfo.PlantCD = Convert.ToString(rd["plantcd"]);
                        lotStdInfo.PlantNM = Convert.ToString(rd["plantnm"]);
                        lotStdInfo.PlantClasNM = Convert.ToString(rd["clasnm"]);

                        dicStbList.Add(lotStdInfo);
                    }
                }
                finally
                {
                    if (!rd.IsClosed){ rd.Close(); }
                    conn.Close();
                }

            }

            return dicStbList;

            //using (SQLiteCommand cmd = con.CreateCommand())
            //{
            //    con.Open();

            //    try
            //    {
            //        /*
            //        cmd.CommandText = @"
            //            SELECT 
            //              t.lotno
            //            FROM 
            //              TnInspection t 
            //            WHERE 
            //              t.lotno = @LOTNO AND
            //              t.procno = 32";//32:外観検査

            //        cmd.Parameters.Add("@LOTNO", DbType.String).Value = sLot;

            //        cmd.CommandText = cmd.CommandText.Replace("\r\n", "");

            //        using (SQLiteDataReader reader = cmd.ExecuteReader())
            //        {
            //            while (reader.Read())
            //            {
            //                sCheck = SQLite.ParseString(reader["lotno"]);
            //            }
            //        }
            //        */
            //    }
            //    catch (Exception ex)
            //    {
            //        //nrtn = -1;
            //        Log.Logger.Info(ex.ToString());
            //        throw ex;
            //    }
            //    finally
            //    {
            //        con.Close();
            //    }
            //}
        }

        /// <summary>
        /// 資材データ
        /// </summary>
        /// <param name="lotNO"></param>
        /// <param name="stockerNO"></param>
        /// <returns></returns>
        public static List<MaterialStbInfo> GetTblMaterialStb_ARMS(string lotNO)
        {
            List<MaterialStbInfo> dicStbList = new List<MaterialStbInfo>();
            SqlDataReader rd = null;

            using (IConnection conn = NascaConnection.CreateInstance(F04_ErrorRecord.StrARMS, false))
            {
                string sql = @"select distinct tntran.lotno as clotno, 
                                tnmacmat.materialcd as mmaterialcd, tnmaterials.materialnm as mmaterialnm, tnmacmat.lotno as mlotno, tmmachine.plantcd,
                                tntran.enddt
                                from tnmacmat WITH(nolock)
                                inner join tntran WITH(nolock) on tntran.macno = tnmacmat.macno
                                inner join tnmaterials WITH(nolock) on tnmaterials.materialcd = tnmacmat.materialcd and tnmaterials.lotno = tnmacmat.lotno
                                inner join tmmachine WITH(nolock) on tmmachine.macno = tnmacmat.macno
                                where (tnmacmat.stockerno = 0) and (tntran.lotno LIKE @LotNO) 
                                and (tntran.enddt >= tnmacmat.startdt)
                                and ((tntran.startdt <= tnmacmat.enddt) or (tnmacmat.enddt is null))
                                and (tntran.enddt is not null) ";
                                
                sql = string.Format(sql);

                SqlParameter param = new SqlParameter("@LotNO", DbType.String);
                param.Value = lotNO + "%";
                conn.Command.Parameters.Add(param);

                try
                {
                    conn.Command.CommandText = sql;
                    rd = conn.Command.ExecuteReader();

                    while (rd.Read())
                    {
                        MaterialStbInfo matStdInfo = new MaterialStbInfo();

                        matStdInfo.CLotNO = Convert.ToString(rd["clotno"]).Trim();
                        matStdInfo.CMaterialCD = ConnectARMS.GetTypeCD(matStdInfo.CLotNO);
                        matStdInfo.MaterialCD = Convert.ToString(rd["mmaterialcd"]);
                        matStdInfo.MaterialNM = Convert.ToString(rd["mmaterialnm"]);
                        matStdInfo.LotNO = Convert.ToString(rd["mlotno"]);
                        matStdInfo.CompleteDT = rd.GetDateTime(rd.GetOrdinal("enddt"));
                        matStdInfo.PlantCD = Convert.ToString(rd["plantcd"]);

                        if (!dicStbList.Exists(m => m.LotNO == matStdInfo.LotNO && m.CompleteDT < matStdInfo.CompleteDT))
                        {
                            //マガジン管理になった為、後のマガジンの終了日時を取得する
                            dicStbList.Add(matStdInfo);
                        }
                    }

                    return dicStbList;
                }
                finally
                {
                    if (!rd.IsClosed) { rd.Close(); }
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 資材データ(搭載機)
        /// </summary>
        /// <param name="lotNO"></param>
        /// <param name="stockerNO"></param>
        /// <returns></returns>
        public static List<MaterialStbInfo> GetTblMaterialStb_ARMS(string lotNO, string stockerNO)
        {
            List<MaterialStbInfo> dicStbList = new List<MaterialStbInfo>();
            SqlDataReader rd = null;

            using (IConnection conn = NascaConnection.CreateInstance(F04_ErrorRecord.StrARMS, false))
            {
                string sql = @"select distinct tntran.lotno as clotno, 
                                tnmacmat.materialcd as mmaterialcd, tnmaterials.materialnm as mmaterialnm, tnmacmat.lotno as mlotno, tmmachine.plantcd,
                                tntran.enddt
                                from tnmacmat WITH(nolock)
                                inner join tntran WITH(nolock) on tntran.macno = tnmacmat.macno and tntran.{0} = Convert(varchar, 1)
                                inner join tnmaterials WITH(nolock) on tnmaterials.materialcd = tnmacmat.materialcd and tnmaterials.lotno = tnmacmat.lotno
                                inner join tmmachine WITH(nolock) on tmmachine.macno = tnmacmat.macno
                                where (tntran.lotno Like @LotNO) 
                                and (tntran.enddt >= tnmacmat.startdt)
                                and ((tntran.startdt <= tnmacmat.enddt) or (tnmacmat.enddt is null))
                                and (tntran.enddt is not null)
                                OPTION (MAXDOP 1) ";
                sql = string.Format(sql, stockerNO);
 
                SqlParameter param = new SqlParameter("@LotNO", DbType.String);
                param.Value = lotNO + "%";
                conn.Command.Parameters.Add(param);

                try
                {
                    conn.Command.CommandText = sql;
                    rd = conn.Command.ExecuteReader();

                    while (rd.Read())
                    {
                        MaterialStbInfo matStdInfo = new MaterialStbInfo();

                        matStdInfo.CLotNO = Convert.ToString(rd["clotno"]).Trim();
                        matStdInfo.CMaterialCD = ConnectARMS.GetTypeCD(matStdInfo.CLotNO);
                        matStdInfo.MaterialCD = Convert.ToString(rd["mmaterialcd"]);
                        matStdInfo.MaterialNM = Convert.ToString(rd["mmaterialnm"]);
                        matStdInfo.LotNO = Convert.ToString(rd["mlotno"]);
                        matStdInfo.CompleteDT = rd.GetDateTime(rd.GetOrdinal("enddt"));
                        matStdInfo.PlantCD = Convert.ToString(rd["plantcd"]);

                        if (!dicStbList.Exists(m => m.LotNO == matStdInfo.LotNO && m.CompleteDT < matStdInfo.CompleteDT))
                        {
                            //マガジン管理になった為、後のマガジンの終了日時を取得する
                            dicStbList.Add(matStdInfo);
                        }
                    }
                }
                finally
                {
                    if (!rd.IsClosed){ rd.Close(); }
                    conn.Close();
                }
            }


            return dicStbList;
        }
 
        /// <summary>
        /// 資材データ(DBウェハー)
        /// </summary>
        /// <param name="lotNO"></param>
        /// <param name="stockerID"></param>
        /// <returns></returns>
        public static MaterialStbInfo GetTblMaterialStb_ARMS(string lotNO, int stockerID) 
        {
            MaterialStbInfo matStbInfo = new MaterialStbInfo();
            SqlDataReader rd = null;

            using (IConnection conn = NascaConnection.CreateInstance(F04_ErrorRecord.StrARMS, false))
            {
                string sql = @"select distinct tntran.lotno as clotno, 
                                tnmacmat.materialcd as mmaterialcd, tnmaterials.materialnm as mmaterialnm, tnmacmat.lotno as mlotno, tmmachine.plantcd,
                                tntran.enddt
                                from tnmacmat WITH(nolock)
                                inner join tntran WITH(nolock) on tntran.macno = tnmacmat.macno
                                inner join tnmaterials WITH(nolock) on tnmaterials.materialcd = tnmacmat.materialcd and tnmaterials.lotno = tnmacmat.lotno
                                inner join tmmachine WITH(nolock) on tmmachine.macno = tnmacmat.macno
                                where (tntran.procno = 1) and (tnmacmat.stockerno = @StockerID) and (tntran.lotno = @LotNO) 
                                and (tntran.enddt >= tnmacmat.startdt)
                                and ((tntran.startdt <= tnmacmat.enddt) or (tnmacmat.enddt is null))
                                and (tntran.enddt is not null)
                                and (tntran.stocker1 is not null and tntran.stocker2 is not null)
                                OPTION (MAXDOP 1) ";
             

                SqlParameter param = new SqlParameter("@LotNO", DbType.String);
                param.Value = lotNO;
                conn.Command.Parameters.Add(param);

                param = new SqlParameter("@StockerID", DbType.Int32);
                param.Value = stockerID;
                conn.Command.Parameters.Add(param);

                try
                {
                    conn.Command.CommandText = sql;
                    rd = conn.Command.ExecuteReader();
                    while (rd.Read())
                    {
                        matStbInfo = new MaterialStbInfo();

                        matStbInfo.CLotNO = Convert.ToString(rd["clotno"]).Trim();
                        matStbInfo.CMaterialCD = ConnectARMS.GetTypeCD(matStbInfo.CLotNO);
                        matStbInfo.MaterialCD = Convert.ToString(rd["mmaterialcd"]);
                        matStbInfo.MaterialNM = Convert.ToString(rd["mmaterialnm"]);
                        matStbInfo.LotNO = Convert.ToString(rd["mlotno"]);
                        matStbInfo.CompleteDT = rd.GetDateTime(rd.GetOrdinal("enddt"));
                        matStbInfo.PlantCD = Convert.ToString(rd["plantcd"]);
                    }
                }
                finally
                {
                    if (!rd.IsClosed) { rd.Close(); }
                    conn.Close();
                }
            }

            return matStbInfo;
        }

        /// <summary>
        /// DBウェハー用 Stockerの始点,終点を取得
        /// </summary>
        /// <param name="lotNO"></param>
        /// <returns></returns>
        public static void GetMaterialStockerStartEnd(string lotNO, ref int startID, ref int endID)
        {
            SqlDataReader rd = null;

            using (IConnection conn = NascaConnection.CreateInstance(F04_ErrorRecord.StrARMS, false))
            {
                string sql = @" select stocker1, stocker2 
                        from tntran WITH(nolock)
                        where (tntran.lotno = @LotNO)
                        and (tntran.enddt is not null) and (tntran.procno = 1)
                        and (tntran.stocker1 is not null and tntran.stocker2 is not null)
                        OPTION (MAXDOP 1) ";

                SqlParameter param = new SqlParameter("@LotNO", DbType.String);
                param.Value = lotNO;
                conn.Command.Parameters.Add(param);

                try
                {
                    conn.Command.CommandText = sql;
                    rd = conn.Command.ExecuteReader();
                    while (rd.Read())
                    {
                        if (rd["stocker1"] != System.DBNull.Value && rd["stocker1"].ToString() !="")
                        {
                            int startLength = rd["stocker1"].ToString().IndexOf('-') + 1;
                            int endLength = rd["stocker1"].ToString().Length - startLength;
                            startID = Convert.ToInt32(rd["stocker1"].ToString().Substring(startLength, endLength));

                        }
                        if (rd["stocker2"] != System.DBNull.Value && rd["stocker2"].ToString() != "")
                        {
                            int startLength = rd["stocker2"].ToString().IndexOf('-') + 1;
                            int endLength = rd["stocker2"].ToString().Length - startLength;
                            endID = Convert.ToInt32(rd["stocker2"].ToString().Substring(startLength, endLength));
                        }
                    }
                }
                finally
                {
                    if (!rd.IsClosed) { rd.Close(); }
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// DBウェハー用 StockerのMax値を取得
        /// </summary>
        /// <param name="lotNO"></param>
        /// <returns></returns>
        public static int GetMaterialStockerMax(string lotNO) 
        {
            using (IConnection conn = NascaConnection.CreateInstance(F04_ErrorRecord.StrARMS, false))
            {
                string sql = @" select max(stockerno) 
                        from tntran WITH(nolock)
                        inner join tnmacmat WITH(nolock) on tnmacmat.macno = tntran.macno
                        where (tntran.lotno Like @LotNO)
                        and (tntran.procno = 1) and (tntran.enddt >= tnmacmat.startdt)
                        and ((tntran.startdt <= tnmacmat.enddt) or (tnmacmat.enddt is null))
                        and (tntran.enddt is not null) 
                        and (tntran.stocker1 is not null and tntran.stocker2 is not null)
                        OPTION (MAXDOP 1) ";

                SqlParameter param = new SqlParameter("@LotNO", DbType.String);
                param.Value = lotNO + "%";
                conn.Command.Parameters.Add(param);

                try
                {
                    conn.Command.CommandText = sql;
                    object maxCT = conn.Command.ExecuteScalar();
                    if (maxCT == System.DBNull.Value)
                    {
                        return -1;
                    }
                    else
                    {
                        return Convert.ToInt32(maxCT);
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// DBウェハー用 Stockerを複数使用しているか確認
        /// </summary>
        /// <returns></returns>
        public static bool GetMaterialStockerMultiFG(string lotNO) 
        {
            SqlDataReader rd = null;
            bool status = false;

            using (IConnection conn = NascaConnection.CreateInstance(F04_ErrorRecord.StrARMS, false))
            {
                string sql = @" select stocker1, stocker2 
                        from tntran WITH(nolock)
                        where (tntran.lotno = @LotNO)
                        and (tntran.enddt is not null) and (tntran.procno = 1)
                        and (tntran.stocker1 is not null and tntran.stocker2 is not null)
                        OPTION (MAXDOP 1) ";

                SqlParameter param = new SqlParameter("@LotNO", DbType.String);
                param.Value = lotNO;
                conn.Command.Parameters.Add(param);

                try
                {
                    conn.Command.CommandText = sql;
                    rd = conn.Command.ExecuteReader();
                    while (rd.Read())
                    {
                        if ((rd["stocker1"] != System.DBNull.Value && rd["stocker1"].ToString() != "") &&
                            (rd["stocker2"] != System.DBNull.Value && rd["stocker2"].ToString() != ""))
                        {
                        	if (rd["stocker1"].ToString().Substring(0, 1) == rd["stocker2"].ToString().Substring(0, 1))
    	                    {
	                            status = false;
                        	}
                        	else 
                        	{
                            	status = true;
                            }
                        }
                    }
                }
                finally
                {
                    if (!rd.IsClosed) { rd.Close(); }
                    conn.Close();
                }
            }

            return status;
        }

        /// <summary>
        /// 資材データ(樹脂)
        /// </summary>
        /// <param name="lotNO"></param>
        /// <returns></returns>
        public static List<MaterialStbInfo> GetTblMaterialResinStb_ARMS(string lotNO)
        {
            //List<int> mixResultIDList = new List<int>();

            List<MaterialStbInfo> dicStbList = new List<MaterialStbInfo>();
            SqlDataReader rd = null;

            using (IConnection conn = NascaConnection.CreateInstance(F04_ErrorRecord.StrARMS, false))
            {
                string sql = @" select distinct mixresultid, tntran.lotno as clotno, tmmachine.plantcd, tntran.enddt
                            from tntran WITH(nolock) 
                            inner join tnmacresin WITH(nolock) on tnmacresin.macno = tntran.macno
                            inner join tmmachine WITH(nolock) on tmmachine.macno = tntran.macno
                            where (tntran.lotno Like @LotNO) 
                            and (tntran.enddt >= tnmacresin.startdt)
                            and ((tntran.startdt <= tnmacresin.enddt) or (tnmacresin.enddt is null))
                            and (tntran.enddt is not null) 
                            OPTION (MAXDOP 1) ";

                SqlParameter param = new SqlParameter("@LotNO", DbType.String);
                param.Value = lotNO + "%";
                conn.Command.Parameters.Add(param);

                try
                {
                    conn.Command.CommandText = sql;
                    rd = conn.Command.ExecuteReader();
                    while (rd.Read())
                    {
                        MaterialStbInfo matStbInfo = new MaterialStbInfo();
                        matStbInfo.CLotNO = lotNO;
                        matStbInfo.CMaterialCD = ConnectARMS.GetTypeCD(matStbInfo.CLotNO);
                        matStbInfo.PlantCD = Convert.ToString(rd["plantcd"]);
                        matStbInfo.MaterialNM = "樹脂";
                        matStbInfo.MixResultID = Convert.ToString(rd["mixresultid"]);
                        matStbInfo.CompleteDT = rd.GetDateTime(rd.GetOrdinal("enddt"));

                        if (!dicStbList.Exists(m => m.LotNO == matStbInfo.LotNO && m.CompleteDT < matStbInfo.CompleteDT))
                        {
                            //マガジン管理になった為、後のマガジンの終了日時を取得する
                            dicStbList.Add(matStbInfo);
                        }
                    }
                }
                finally
                {
                    if (!rd.IsClosed) { rd.Close(); }
                    conn.Close();
                }
            }
            return dicStbList;
        }

        /// <summary>
        /// 資材の交換日時を取得
        /// </summary>
        /// <param name="matStbInfo"></param>
        /// <returns></returns>
        public static DateTime? GetChangeTimingDT(MaterialStbInfo matStbInfo) 
        {
			//bool isMDResinFG = false;
			//string[] MDResinNames = Constant.ENUM_HMGP_MDRESIN.GetNames(typeof(Constant.ENUM_HMGP_MDRESIN));
#if MEASURE_TIME
			DateTime baseTime = DateTime.Now;
#endif
			//foreach (string MDResinNM in MDResinNames)
			//{
			//    if (matStbInfo.MateGroupCD == MDResinNM)
			//    {
			//        isMDResinFG = true;
			//        break;
			//    }
			//}

#if MEASURE_TIME
			Console.WriteLine("GetChangeTimingDT() foreach(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
			baseTime = DateTime.Now;
#endif

            using (IConnection conn = NascaConnection.CreateInstance(F04_ErrorRecord.StrARMS, false))
            {
                string sql = "";
                //if (matStbInfo.MaterialNM == "樹脂" || isMDResinFG)
				if(matStbInfo.MixResultID != null)
                {
                    sql = @" select max(tnmacresin.startdt) 
                             from tnmacresin WITH(nolock) 
                             inner join tmmachine WITH(nolock) on tmmachine.macno = tnmacresin.macno
                             where (mixresultid = @MixResultID)
                             and (tmmachine.plantcd = @PlantCD) 
                             and (tnmacresin.startdt <= @StartDT)
                             OPTION (MAXDOP 1) ";

                    //tntran WITH(nolock) inner join  on tnmacresin.macno = tntran.macno
                    //(tntran.lotno Like @LotNO) and

                    SqlParameter param = new SqlParameter("@MixResultID", DbType.Int64);
                    param.Value = matStbInfo.MixResultID;
                    conn.Command.Parameters.Add(param);

                    //param = new SqlParameter("@LotNO", DbType.String);
                    //param.Value = matStbInfo.CLotNO + "%";
                    //conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@PlantCD", DbType.String);
                    param.Value = matStbInfo.PlantCD;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@StartDT", DbType.String);
                    param.Value = matStbInfo.CompleteDT;
                    conn.Command.Parameters.Add(param);
                }
                else
                {
                    sql = @" select max(startdt) 
                            FROM dbo.TnMacMat AS TnMacMat WITH(nolock) INNER JOIN
                                  dbo.TmMachine AS TmMachine WITH(nolock) ON TnMacMat.macno = TmMachine.macno
                            WHERE (materialcd = @MaterialCD) AND (lotno = @LotNO) AND (plantcd = @PlantCD)
                            AND (startdt <= @StartDT)
                            OPTION (MAXDOP 1) ";

                    SqlParameter param = new SqlParameter("@MaterialCD", DbType.String);
                    param.Value = matStbInfo.MaterialCD;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@LotNO", DbType.String);
                    param.Value = matStbInfo.LotNO;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@PlantCD", DbType.String);
                    param.Value = matStbInfo.PlantCD;
                    conn.Command.Parameters.Add(param);

                    param = new SqlParameter("@StartDT", DbType.String);
                    param.Value = matStbInfo.CompleteDT;
                    conn.Command.Parameters.Add(param);
                }

                conn.Command.CommandText = sql;

                try
                {
                    object changeDT = conn.Command.ExecuteScalar();
                    if (changeDT == System.DBNull.Value)
                    {
                        return null;
                    }
                    else 
                    {
                        return Convert.ToDateTime(changeDT);
                    }
                }
                finally
                {
                    conn.Close();
#if MEASURE_TIME
					Console.WriteLine("GetChangeTimingDT() connect(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
#endif
                }
            }
        }

    }
}
