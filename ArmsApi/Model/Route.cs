using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class Route
    {
        public int MacNo { get; set; }

        public bool IsHandover { get; set; }

        /// <summary>
        /// Routeのレコードに含まれない装置一覧を取得
        /// </summary>
        /// <returns></returns>
        public static bool HasRouteInfo(int macno)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @" SELECT carrierno FROM TmRoute
                                            WHERE (delfg = 0) AND (macno = @MACNO)";

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno;

                    if (cmd.ExecuteScalar() == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

		public static bool IsReachable(Location from, Location to, bool includeHandover, bool isHandover)
		{
			try
			{
				using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
				using (SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					string sql = @" SELECT carrierno FROM TmRoute as rFrom WITH(nolock)
                                         WHERE (rFrom.delfg = 0) AND (rFrom.macno = @FROMMACNO) AND EXISTS (
                                            SELECT * FROM dbo.TmRoute as rTo  
                                            WHERE (rTo.delfg = 0) AND (rTo.macno = @TOMACNO) AND (rFrom.carrierno = rTo.carrierno) ";
                    if (includeHandover)
                    {
                        sql += "         AND (rTo.handoverfg = @HANDOVERFG) ";
                        sql += "      ) ";
                        sql += "      AND (rFrom.handoverfg = @HANDOVERFG) ";

                        cmd.Parameters.Add("@HANDOVERFG", SqlDbType.Int).Value = SQLite.ParseInt(isHandover);
                    }
                    else
                    {
                        sql += ") ";
                    }
					cmd.CommandText = sql;

					cmd.Parameters.Add("@FROMMACNO", SqlDbType.BigInt).Value = from.MacNo;
					cmd.Parameters.Add("@TOMACNO", SqlDbType.BigInt).Value = to.MacNo;

					if (cmd.ExecuteScalar() == null)
					{
						return false;
					}
					else
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Log.SysLog.Error(ex);
				throw ex;
			}
		}

		/// <summary>
		/// ハンドオーバー込みで到達可能か判定
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static bool IsReachable(Location from, Location to)
		{
			return IsReachable(from, to, false, false);
		}

		/// <summary>
		/// ハンドオーバー無しで到達可能か判定
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static bool IsMyReachable(Location from, Location to)
		{
			return IsReachable(from, to, true, false);
		}

//		/// <summary>
//		/// ハンドオーバー込みで到達可能か判定
//		/// </summary>
//		/// <param name="from"></param>
//		/// <param name="to"></param>
//		/// <returns></returns>
//		public static bool IsReachable(Location from, Location to)
//		{
//			try
//			{
//				using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
//				using (SqlCommand cmd = con.CreateCommand())
//				{
//					con.Open();

//					cmd.CommandText = @" SELECT carrierno FROM TmRoute as rFrom WITH(nolock)
//                                         WHERE (delfg = 0) AND (macno = @FROMMACNO) AND EXISTS (
//                                            SELECT * FROM dbo.TmRoute as rTo  
//                                            WHERE (delfg = 0) AND (rTo.macno = @TOMACNO) AND (rFrom.carrierno = rTo.carrierno)) ";

//					cmd.Parameters.Add("@FROMMACNO", SqlDbType.BigInt).Value = from.MacNo;
//					cmd.Parameters.Add("@TOMACNO", SqlDbType.BigInt).Value = to.MacNo;

//					if (cmd.ExecuteScalar() == null)
//					{
//						return false;
//					}
//					else 
//					{
//						return true;
//					}
//				}
//			}
//			catch (Exception ex)
//			{
//				Log.SysLog.Error(ex);
//				throw ex;
//			}
//		}

        /// <summary>
        /// 指定したキャリアでの到達可否判定
        /// </summary>
        /// <param name="loc"></param>
        /// <param name="cr"></param>
        /// <returns></returns>
        public static bool IsReachable(Location loc, CarrierInfo carrier)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @" SELECT carrierno FROM TmRoute WITH(nolock)
                                         WHERE (delfg = 0) AND (carrierno = @CARNO) AND (macno = @MACNO) ";

                    cmd.Parameters.Add("@CARNO", SqlDbType.BigInt).Value = carrier.CarNo;
                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = loc.MacNo;

                    if (cmd.ExecuteScalar() == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 到達可能なキャリアを取得
        /// </summary>
        /// <param name="loc">ロケーション</param>
        /// <returns>キャリアNO</returns>
        public static CarrierInfo GetReachable(Location loc)
        {
            try
            {
                CarrierInfo carrier = new CarrierInfo();

                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @" SELECT carrierno FROM TmRoute WITH(nolock)
                                         WHERE (delfg = 0) AND (macno = @MACNO) AND (handoverfg = 0) ";

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = loc.MacNo;

                    object carrierno = cmd.ExecuteScalar();
                    if (carrierno == null)
                    {
                        return null;
                    }
                    else
                    {
                        carrier.CarNo = Convert.ToInt32(carrierno);
                    }
                }
                return carrier;
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        public static List<Location> GetHandoverLocationList(CarrierInfo from, CarrierInfo to)
        {
            return GetHandoverLocationList(from.CarNo, to.CarNo);
        }

        public static List<Location> GetHandoverLocationList(int fromCarrierNo, int toCarrierNo)
        {
            List<Location> retV = new List<Location>();

            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" SELECT locationmacno, locationid
                                    FROM TmHandover WITH(nolock)
                                    WHERE (delfg = 0) AND (fromcarrierno = @FROMCARNO) AND (tocarrierno = @TOCARNO) ";

                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@FROMCARNO", SqlDbType.BigInt).Value = fromCarrierNo;
                    cmd.Parameters.Add("@TOCARNO", SqlDbType.BigInt).Value = toCarrierNo;

                    SqlDataReader rd = cmd.ExecuteReader();
                    if (!rd.HasRows)
                    {
                        throw new ArmsException(string.Format("キャリア間の受け渡し場所がマスタ設定されていません。FROM:{0} TO:{1}", fromCarrierNo, toCarrierNo));
                    }
                    while (rd.Read())
                    {
                        Location loc = new Location(SQLite.ParseInt(rd["locationmacno"]),
                                                    (Station)SQLite.ParseInt(rd["locationid"]));
                        retV.Add(loc);
                    }
                }
                return retV;
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// キャリア間の受け渡しに使われるLocation取得
        /// 存在しない場合はNull
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Location GetHandoverLocation(CarrierInfo from, CarrierInfo to)
        {
            List<Location> locList = GetHandoverLocationList(from.CarNo, to.CarNo);
            if (locList.Count == 1)
            {
                return locList.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public static bool IsHandoverMachine(int macNo)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" SELECT locationmacno
                                    FROM TmHandover WITH(nolock)
                                    WHERE (delfg = 0) AND (locationmacno = @MACNO)  ";

                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macNo;

                    object hasdata = cmd.ExecuteScalar();
                    if (hasdata == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 指定したキャリアの到達可能装置を取得
        /// </summary>
        /// <param name="carNo"></param>
        /// <returns></returns>
        public static List<int> GetMachines(int carno)
        {
            List<int> macList = new List<int>();
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" SELECT TmRoute.macno
                                    FROM TmRoute WITH(nolock)
                                    INNER JOIN TmMachine WITH(nolock) ON TmRoute.macno = TmMachine.macno 
                                    WHERE carrierno = @CARNO AND handoverfg = 0 AND (TmRoute.delfg = 0) AND (TmMachine.delfg = 0) ";
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@CARNO", SqlDbType.Int).Value = carno;

                    using (SqlDataReader rd = cmd.ExecuteReader()) 
                    {
                        if (!rd.HasRows) 
                        {
                            throw new ApplicationException(string.Format("到達できる装置が存在しません。キャリアNO:{0}", carno));
                        }

                        int ordMacno = rd.GetOrdinal("macno");
                        while(rd.Read())
                        {
                            macList.Add(Convert.ToInt32(rd[ordMacno]));
                        }               
                    }
                }

                return macList;
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 到達可能な装置を優先した並び変え後装置リストを取得
        /// </summary>
        /// <param name="carno"></param>
        /// <param name="machines"></param>
        /// <returns></returns>
        public static List<int> GetSortReachableMachine(int carno, List<int> machines) 
        {
            List<int> rMachines = new List<int>();
            List<int> hMachines = new List<int>();

            //到達可能な装置リストを取得
            //*J TmRouteとTmMachineから抽出
            List<int> reachMachines = GetMachines(carno);

            foreach(int machine in machines)
            {
                //到達可能
                if (reachMachines.Exists(m => m == machine))
                {
                    rMachines.Add(machine);
                }
                //ハンドオーバー、高効率
                else 
                {
                    hMachines.Add(machine);
                }
            }
            rMachines.AddRange(hMachines);

            return rMachines;
        }

        /// <summary>
        /// 排出CV
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static int GetDischargeConveyor(int macno)
        {
            return GetConveyor(macno, "排出CV");
        }

        public static List<int> GetDischargeConveyors(int macno)
        {
            return GetMultipleConveyor(macno, "排出CV");
        }

        /// <summary>
        /// 排出CV(アオイ)
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static int GetAoiDischargeConveyor(int macno)
        {
            return GetConveyor(macno, "排出CV(アオイ)");
        }

        /// <summary>
        /// 完成品排出CV
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static int GetCompltDischargeConveyor(int macno)
        {
            return GetConveyor(macno, "完成品排出CV");
        }

        public static List<int> GetCompltDischargeConveyors(int macno)
        {
            return GetMultipleConveyor(macno, "完成品排出CV");
        }

        /// <summary>
        /// 空マガジンCV
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static int GetEmptyMagazineLoadConveyor(int macno)
        {
            return GetConveyor(macno, "空マガジン投入CV");
        }

        /// <summary>
        /// 空マガジンCVリスト (共用投入CV含む)
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static List<int> GetEmptyMagazineLoadConveyors(int macno)
        {
            List<string> addClasnmList = new List<string>() { "共用投入CV" };
            return GetMultipleConveyor(macno, "空マガジン投入CV", addClasnmList, true);
        }

        /// <summary>
        /// 途中投入CV
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static int GetLoadConveyor(int macno) 
        {
            return GetConveyor(macno, "途中投入CV");
        }

        /// <summary>
        /// 途中投入CV(アオイ)
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static int GetAoiLoadConveyor(int macno)
        {
            return GetConveyor(macno, "途中投入CV(アオイ)");
        }

        /// <summary>
        /// ラインアウト排出CV
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static int GetAutoLineOutConveyor(int macno)
        {
            return GetConveyor(macno, "ラインアウト排出CV");
        }

        public static List<int> GetAutoLineOutConveyors(int macno)
        {
            List<string> clasNmList = new List<string>() { "" };
            return GetMultipleConveyor(macno, "ラインアウト排出CV");
        }

        /// <summary>
        /// AGV投入CV
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static int GetAgvLoadConveyor(int macno)
        {
            return GetConveyor(macno, "AGV投入CV");
        }

        public static List<int> GetAgvLoadConveyors(int macno)
        {
            return GetMultipleConveyor(macno, "AGV投入CV", null, true);
        }

        private static List<Route> GetConveyorList(int macno, string clasnm)
        {
            return GetConveyorList(macno, clasnm, true);
        }

        private static List<Route> GetConveyorList(int macno, string clasnm, bool isCheckRecord)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @" SELECT TmMachine.macno, rTo.handoverfg FROM TmRoute AS rTo WITH(nolock)
                                            INNER JOIN TmMachine WITH(nolock) ON TmMachine.macno = rTo.macno
                                         WHERE (TmMachine.delfg = 0) AND (TmMachine.clasnm = @CLASNM)
                                            AND EXISTS (
                                                SELECT * FROM TmRoute AS rFrom WHERE (macno = @MACNO) AND (handoverfg = 0) AND (TmMachine.delfg = 0)
                                                AND (rFrom.carrierno = rTo.carrierno)) ";

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno;
                    cmd.Parameters.Add("@CLASNM", SqlDbType.NVarChar).Value = clasnm;

                    List<Route> routs = new List<Route>();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Route route = new Route();
                            route.MacNo = SQLite.ParseInt(rd["macno"]);
                            route.IsHandover = SQLite.ParseBool(rd["handoverfg"]);

                            routs.Add(route);
                        }
                    }

                    if (isCheckRecord == true && routs.Count == 0)
                    {
                        throw new ArmsException(string.Format("該当するCVがマスタに存在しません。コンベア:{0}", clasnm));
                    }

                    return routs;
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        private static int GetConveyor(int macno, string clasnm) 
        {
            List<Route> routs = GetConveyorList(macno, clasnm);
            int index = routs.FindIndex(r => r.IsHandover == false);
            if (index == -1)
            {
                index = routs.FindIndex(r => r.IsHandover == true);
            }

            return routs[index].MacNo;
        }

        private static List<int> GetMultipleConveyor(int macno, string clasnm)
        {
            return GetMultipleConveyor(macno, clasnm, null, false);
        }
        
        private static List<int> GetMultipleConveyor(int macno, string clasnm, List<string> clasnmlist, bool includeHandoverFg)
        {
            List<Route> routs = GetConveyorList(macno, clasnm, false);
            if (clasnmlist != null)
            {
                foreach (string c in clasnmlist)
                {
                    routs.AddRange(GetConveyorList(macno, c, false));
                }
            }

            if (includeHandoverFg == true)
            {
                // handoverfg = 0 のレコードがリストに含まれていれば、0のレコードのみに絞る
                if (routs.Exists(r => r.IsHandover == false) == true)
                {
                    routs = routs.Where(r => r.IsHandover == false).ToList();
                }
            }

            IOrderedEnumerable<Route> ordRouts = routs.OrderBy(r => r.IsHandover);

            return ordRouts.Select(r => r.MacNo).ToList();
        }
    }
}
