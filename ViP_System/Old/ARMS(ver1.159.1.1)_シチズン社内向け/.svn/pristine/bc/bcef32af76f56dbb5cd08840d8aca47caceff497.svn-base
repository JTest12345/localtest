using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using System.Data.SqlClient;
using ArmsApi;
using System.Data;
using System.Text.RegularExpressions;

namespace WorkInstruction
{
    /// <summary>
    /// 調合
    /// </summary>
    public class Preparation
    {
        /// <summary>
        /// 依頼マスタ
        /// </summary>
        public class ResinMixOrderMaster
        {
            /// <summary>
            /// まとめ型番
            /// </summary>
            public string TypeGroupCd { get; set; }

            /// <summary>
            /// 調合作業CD
            /// </summary>
            public string PreparationWorkCd { get; set; }
            public int PreparationProcNo { get; set; }

            /// <summary>
            /// 依頼作業CD
            /// 依頼条件完成作業が完成しているロットの中で1ロットでもこの作業にかかれば依頼
            /// </summary>
            public string OrderWorkCd { get; set; }
            public int OrderProcNo { get; set; }

            /// <summary>
            /// 依頼条件完成作業時期(作業開始時、完了時)
            /// </summary>
            public Timing OrderCondCompltTiming { get; set; }

            /// <summary>
            /// 依頼条件ロット数
            /// </summary>
            public int OrderCondLotCt { get; set; }

            /// <summary>
            /// 依頼条件完成作業CD
            /// </summary>
            public string OrderCondCompleteWorkCd { get; set; }
            public int OrderCondCompleteProcNo { get; set; }

            /// <summary>
            /// 依頼後限界時間
            /// </summary>
            public int AfterOrderLimitTime { get; set; }

            /// <summary>
            /// 依頼後完成希望日時
            /// </summary>
            private int AfterOrderCompletePlanTime { get; set; }

            public DateTime AfterOrderCompletePlanDt
            {
                get
                {
                    return System.DateTime.Now.AddMinutes(this.AfterOrderCompletePlanTime);
                }
            }

            public enum Timing
            {
                Start = 1,
                Complete = 2,
            }

            public List<string> MixTypeNm { get; set; } = new List<string>();

            public static List<ResinMixOrderMaster> GetOrderList()
            {
                List<ResinMixOrderMaster> retv = new List<ResinMixOrderMaster>();

                using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

//#if DEBUG
                    string sql = @" SELECT OrderCondCompltTiming_ID, OrderCondLot_CT, OrderLimitWait_TM, OrderCompletePlan_TM,
                                    PreparationArmsProc_No, OrderArmsProc_No, OrderCondCompltArmsProc_No, TypeGroup_CD
                                    FROM JtmMIXORDER_TEST WHERE del_fg = 0 ";
//#else
//                    string sql = @" SELECT OrderCondCompltTiming_ID, OrderCondLot_CT, OrderLimitWait_TM, OrderCompletePlan_TM,
//                                    PreparationArmsProc_No, OrderArmsProc_No, OrderCondCompltArmsProc_No, TypeGroup_CD
//                                    FROM JtmMIXORDER WHERE del_fg = 0 ";
//#endif

                    cmd.CommandText = sql;
                    SqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        ResinMixOrderMaster o = new ResinMixOrderMaster();

                        o.PreparationProcNo = Convert.ToInt32(rd["PreparationArmsProc_No"]);
                        Process p = Process.GetProcess(o.PreparationProcNo);

                        foreach (string mixTypeCd in p.MixTypeCd)
                        {
                            o.MixTypeNm.Add(GetMixTypeName(mixTypeCd));
                        }
                        o.OrderProcNo = Convert.ToInt32(rd["OrderArmsProc_No"]);

                        int timingNo = Convert.ToInt32(rd["OrderCondCompltTiming_ID"]);
                        if (timingNo == (int)Timing.Start)
                        {
                            o.OrderCondCompltTiming = Timing.Start;
                        }
                        else
                        {
                            o.OrderCondCompltTiming = Timing.Complete;
                        }
                        o.OrderCondLotCt = Convert.ToInt32(rd["OrderCondLot_CT"]);

                        o.OrderCondCompleteProcNo = Convert.ToInt32(rd["OrderCondCompltArmsProc_No"]);

                        o.AfterOrderLimitTime = Convert.ToInt32(rd["OrderLimitWait_TM"]);
                        o.AfterOrderCompletePlanTime = Convert.ToInt32(rd["OrderCompletePlan_TM"]);

                        o.TypeGroupCd = rd["TypeGroup_CD"].ToString().Trim();

                        retv.Add(o);
                    }
                }

                return retv;
            }
        }

        /// <summary>
        /// 依頼
        /// </summary>
        public class ResinMixOrder
        {
            public string ResinGroup { get; set; }

            public bool IsPassingOrderWork { get; set; }

            public string TypeCd { get; set; }

            public string LotNo { get; set; }

            public int ProcNo { get; set; }

            /// <summary>
            /// 依頼条件の完了作業が開始している日時
            /// </summary>
            public DateTime CompleteWorkStartDt { get; set; }

            /// <summary>
            /// 依頼条件の完了作業が完了している日時
            /// </summary>
            public DateTime CompleteWorkEndDt { get; set; }

            /// <summary>
            /// 依頼をかける作業が開始している日時
            /// </summary>
            public DateTime OrderWorkStartDt { get; set; }


            /// <summary>
            /// 依頼
            /// </summary>
            /// <param name="orderDt">依頼日時</param>
            /// <param name="resinGroup">樹脂グループ</param>
            /// <param name="lotCt">ロット数</param>
            /// <param name="completePlanDt">調合完了希望日時</param>
            /// <param name="mixTypeNm">調合タイプ名</param>
            public static void Insert(string resinGroupCd, DateTime orderDt, int lotCt, DateTime completePlanDt, List<string> mixTypeNm, List<ResinMixOrder> lotList, int preparationProcNo)
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    string sql = @" INSERT INTO JttReqest(Sec_CD,Req_DT, Priority_FG, ResinGroup_CD, Lot_CT, UpdUser_CD, 
                        Line_CD, ExpectComp_DT, ExpectPlace_CD, ActualPlace_CD, Status_CD, Start_DT, End_DT, LastUpd_DT,
                        MixType_NM, Machine_CT, Pre_FG, Comment_NM) 
                        VALUES(@Sec_CD, @Req_DT, @Priority_FG, @ResinGroup_CD, @Lot_CT, 660,
                        @Line_CD, @ExpectComp_DT, @ExpectPlace_CD, @ActualPlace_CD, @Status_CD, @Start_DT, @End_DT, getdate(),
                        @MixType_NM, 1, @Pre_FG, NULL) 
                        SELECT CAST(SCOPE_IDENTITY() AS int) ";

                    try
                    {
                        cmd.Parameters.Add("@Sec_CD", SqlDbType.NVarChar).Value = ArmsApi.Config.Settings.NascaApiServerCd.Replace("_", "-");
                        cmd.Parameters.Add("@Req_DT", SqlDbType.NVarChar).Value = orderDt;
                        cmd.Parameters.Add("@Priority_FG", SqlDbType.Bit).Value = false;
                        cmd.Parameters.Add("@ResinGroup_CD", SqlDbType.NChar).Value = resinGroupCd;
                        cmd.Parameters.Add("@Lot_CT", SqlDbType.Int).Value = lotCt;
                        cmd.Parameters.Add("@Line_CD", SqlDbType.NVarChar).Value = "-";
                        cmd.Parameters.Add("@ExpectComp_DT", SqlDbType.DateTime).Value = completePlanDt;
                        cmd.Parameters.Add("@ExpectPlace_CD", SqlDbType.NVarChar).Value = "-";
                        cmd.Parameters.Add("@ActualPlace_CD", SqlDbType.NVarChar).Value = "-";
                        cmd.Parameters.Add("@Status_CD", SqlDbType.Int).Value = 0;
                        cmd.Parameters.Add("@Start_DT", SqlDbType.DateTime).Value = DBNull.Value;
                        cmd.Parameters.Add("@End_DT", SqlDbType.DateTime).Value = DBNull.Value;
                        cmd.Parameters.Add("@MixType_NM", SqlDbType.NVarChar).Value = string.Join(",", mixTypeNm);
                        cmd.Parameters.Add("@Pre_FG", SqlDbType.Bit).Value = false;

                        cmd.CommandText = sql;
                        int id = (int)cmd.ExecuteScalar();

                        SqlParameter paramReqId = cmd.Parameters.Add("@Req_ID", SqlDbType.Int);
                        SqlParameter paramTypeCd = cmd.Parameters.Add("@Type_CD", SqlDbType.NVarChar);
                        SqlParameter paramLotNo = cmd.Parameters.Add("@Lot_NO", SqlDbType.NVarChar);
                        SqlParameter paramProcNo = cmd.Parameters.Add("@ArmsProc_NO", SqlDbType.Int);

                        foreach (ResinMixOrder lot in lotList)
                        {
                            sql = @" INSERT JttReqestLot(Req_ID, Type_CD, Lot_NO, ArmsProc_NO) VALUES(@Req_ID, @Type_CD, @Lot_NO, @ArmsProc_NO) ";

                            cmd.CommandText = sql;

                            paramReqId.Value = id;
                            paramTypeCd.Value = lot.TypeCd;
                            paramLotNo.Value = lot.LotNo;
                            paramProcNo.Value = preparationProcNo;
                                
                            cmd.ExecuteNonQuery();
                        }
                        
                        cmd.Transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        cmd.Transaction.Rollback();
                        throw new Exception(ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 指示
        /// </summary>
        public static void Direction()
        {
            Magazine[] mags = Magazine.GetMagazine(true);

            // 依頼マスタ取得
            List<ResinMixOrderMaster> orderList = ResinMixOrderMaster.GetOrderList();

            orderList = orderList.Where(o => string.IsNullOrEmpty(o.TypeGroupCd) == false).ToList();

            foreach (ResinMixOrderMaster o in orderList)
            {
                Log.SysLog.Info(string.Format("□[依頼確認開始] まとめ型番:{0} 樹脂使用作業:{1}", o.TypeGroupCd, o.PreparationProcNo));

                try
                {
                    // 依頼条件の作業が開始/完了しているロット一覧取得 (依頼済み除く)
                    List<ResinMixOrder> lotList = GetCompleteLotList(mags, o.TypeGroupCd, o.OrderCondCompleteProcNo, o.OrderCondCompltTiming, o.PreparationProcNo);
                    if (lotList.Count == 0)
                    {
                        continue;
                    }

                    ILookup<string, ResinMixOrder> resinGroupList = lotList.ToLookup(l => l.ResinGroup);
                    foreach (IGrouping<string, ResinMixOrder> resinGroupLotList in resinGroupList)
                    {
                        // 依頼をかける作業を1ロット以上通過しているか確認
                        foreach (ResinMixOrder lot in resinGroupLotList)
                        {
                            DateTime? passDt;
                            if (IsPassingWork(lot.LotNo, o.OrderProcNo, ResinMixOrderMaster.Timing.Start, out passDt))
                            {
                                lot.IsPassingOrderWork = true;
                                lot.OrderWorkStartDt = passDt.Value;
                            }
                        }
                        if (lotList.Where(p => p.IsPassingOrderWork == true).Count() == 0)
                            continue;

                        // 依頼をかける作業を開始しているロットを優先して依頼ロットに入れ、バッチマガジン数に絞る
                        IEnumerable<ResinMixOrder> orderLotList = resinGroupLotList
                            .OrderByDescending(l => l.IsPassingOrderWork == true)
                            .ThenBy(l => l.OrderWorkStartDt)
                            .Take(o.OrderCondLotCt);

                        Log.SysLog.Info(
                            string.Format("[依頼作業通過] 樹脂グループ:{0} 依頼作業:{1} 通過ロット:{2} 通過日時:{3}", resinGroupLotList.Key, o.OrderWorkCd, orderLotList.First().LotNo, orderLotList.First().OrderWorkStartDt));

                        // 依頼条件のロット数溜まっている、溜まっていなくても依頼をかける作業を開始しているロットが限界時間を超えているは依頼
                        if (o.OrderCondLotCt > orderLotList.Count() && IsWaitedLimitOver(orderLotList.First(), o.AfterOrderLimitTime) == false)
                        {
                            Log.SysLog.Info(
                                string.Format("[依頼見送り] バッチ数に満たない又は通過後待機限界時間(分)を超過していない バッチ数:{0} LotNo:{1} 通過後待機限界時間(分):{2}", o.OrderCondLotCt, string.Join(", ", orderLotList.Select(l => l.LotNo)), o.AfterOrderLimitTime));
                            continue;
                        }

                        Log.SysLog.Info(
                            string.Format("[依頼バッチ数確認] 樹脂グループ:{0} LotNo:{1} 通過後待機限界時間(分):{2}", resinGroupLotList.Key, string.Join(",", orderLotList.Select(l => l.LotNo)), o.AfterOrderLimitTime));

#if DEBUG

#else
                        // 依頼
                        ResinMixOrder.Insert(resinGroupLotList.Key, System.DateTime.Now, orderLotList.Count(), o.AfterOrderCompletePlanDt, o.MixTypeNm, orderLotList.ToList(), o.PreparationProcNo);
#endif

                        Log.SysLog.Info(
                            string.Format("[依頼送信] 樹脂グループ:{0} LotNo:{1} ", resinGroupLotList.Key, string.Join(",", orderLotList.Select(l => l.LotNo))));
                    }
                }
                catch (Exception err)
                {
                    Log.SysLog.Error(err.Message);
                }
                finally
                {
                    Log.SysLog.Info(string.Format("■[依頼確認完了] まとめ型番:{0} 樹脂使用作業:{1}", o.TypeGroupCd, o.PreparationProcNo));
                }
            }
        }

        /// <summary>
        /// 依頼条件の作業が開始/完了しているロット一覧取得
        /// ※依頼済みロット除く
        /// </summary>
        /// <param name="workCd"></param>
        /// <returns></returns>
        public static List<ResinMixOrder> GetCompleteLotList(Magazine[] mags, string typeGroupCd, int procNo, ResinMixOrderMaster.Timing timing, int preparationProcNo)
        {
            List<ResinMixOrder> retv = new List<ResinMixOrder>();

            // MD/反射材の作業を開始してるロットは除外
            mags = mags.Where(m => IsPassingWork(m.NascaLotNO, preparationProcNo, ResinMixOrderMaster.Timing.Start) == false).ToArray();
            if (mags.Count() == 0)
                return retv;

            // 依頼済みロットは除く
            mags = mags.Where(m => isResinMixOrdered(m.NascaLotNO, preparationProcNo) == false).ToArray();
            if (mags.Count() == 0)
                return retv;

            foreach (Magazine mag in mags)
            {
                try
                {
                    AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);

                    if (Process.HasWorkflowExists(preparationProcNo, lot.TypeCd) == false)
                    {
                        // 作業順にMD/反射材の作業が無いロットは除外
                        continue;
                    }

                    if (Regex.IsMatch(lot.TypeCd, "^" + typeGroupCd + "$") == false)
                    {
                        // まとめ型番に一致しないロットは除外
                        continue;
                    }

                    ArmsApi.Model.Order[] o = ArmsApi.Model.Order.GetOrder(mag.NascaLotNO, procNo);
                    if (o.Count() == 0)
                        continue;

                    if (timing == ResinMixOrderMaster.Timing.Complete)
                    {
                        if (o.Single().IsComplete == false)
                        {
                            continue;
                        }
                    }

                    ResinMixOrder rOrder = new ResinMixOrder();
                    rOrder.LotNo = o.Single().LotNo;
                    rOrder.TypeCd = lot.TypeCd;
                    rOrder.CompleteWorkStartDt = o.Single().WorkStartDt;

                    if (o.Single().WorkEndDt.HasValue)
                    {
                        rOrder.CompleteWorkEndDt = o.Single().WorkEndDt.Value;
                    }

                    rOrder.ProcNo = o.Single().ProcNo;
                    rOrder.ResinGroup = GetResinGroupFromLot(rOrder.LotNo, preparationProcNo);

                    retv.Add(rOrder);
                }
                catch (Exception ex)
                {
                    // 1ロットの実績データ不備、樹脂グループの特定ができない場合はログを残して
                    // そのロットは対象外にする
                    Log.SysLog.Error(ex.ToString());
                }
            }

            return retv;
        }

        /// <summary>
        /// 指定作業(開始又は完了)をロットが通過しているか確認
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="workCd"></param>
        /// <param name="timing"></param>
        /// <returns></returns>
        public static bool IsPassingWork(string lotNo, int procNo, ResinMixOrderMaster.Timing timing, out DateTime? passDt)
        {
            passDt = null;

            ArmsApi.Model.Order[] order = ArmsApi.Model.Order.GetOrder(lotNo, procNo);
            if (order.Count() == 0)
            {
                return false;
            }

            if (timing == ResinMixOrderMaster.Timing.Complete)
            {
                if (order.Single().IsComplete)
                {
                    passDt = order.Single().WorkEndDt.Value;
                    return true;
                }
                else
                    return false;
            }
            else
            {
                passDt = order.Single().WorkStartDt;
                return true;
            }
        }
        public static bool IsPassingWork(string lotNo, int procNo, ResinMixOrderMaster.Timing timing)
        {
            DateTime? date;
            return IsPassingWork(lotNo, procNo, timing, out date);
        }

        /// <summary>
        /// 指定作業(開始又は完了)のロットが依頼する限界時間(分)を超えているか確認
        /// </summary>
        /// <returns></returns>
        //public static bool IsLimitOver(string lotNo, int procNo, OrderMaster.Timing timing, int limitSec)
        //{
        //    bool retv = false;

        //    ArmsApi.Model.Order[] order = ArmsApi.Model.Order.GetOrder(lotNo, procNo);
        //    if (order.Count() == 0)
        //    {
        //        return false;
        //    }

        //    if (timing == OrderMaster.Timing.Complete)
        //    {
        //        if (order.Single().WorkEndDt.HasValue && order.Single().WorkEndDt)

        //            return true;

        //        else
        //            return false;
        //    }
        //    else
        //    {


        //    }
        //}

        public static bool IsWaitedLimitOver(ResinMixOrder lot, int limitMinutes)
        {
            //if (timing == ResinMixOrderMaster.Timing.Start)
            //{
            //ResinMixOrder lastLot = lotList.OrderByDescending(l => l.CompleteWorkStartDt).First();
            if (lot.OrderWorkStartDt.AddMinutes(limitMinutes) <= System.DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }
            //}
            //else
            //{
            //    //ResinMixOrder lastLot = lotList.OrderByDescending(l => l.CompleteWorkEndDt).First();
            //    if (lot.CompleteWorkEndDt.AddMinutes(limitMinutes) <= System.DateTime.Now)
            //    {
            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
        }

        /// <summary>
        /// ロットの樹脂グループ一覧から指定作業で使用する樹脂グループを特定する
        /// （工程マスタの調合タイプと樹脂グループを基に樹脂調合システムに問い合わせる）
        /// ※先行色調結果待ちなどの現在使用不可の樹脂グループはここで判断されロット一覧から除外される
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="procNo"></param>
        /// <returns></returns>
        public static string GetResinGroupFromLot(string lotNo, int procNo)
        {
            string rResinGroup = string.Empty;

            Process p = Process.GetProcess(procNo);
            
            //if (string.IsNullOrEmpty(p.MixTypeCd))
            if (p.MixTypeCd.Any() == false)
            {
                throw new Exception(string.Format("工程マスタに調合タイプが設定されていない為、依頼する樹脂グループが取得できませんでした。LotNo:{0} 工程No:{1}", lotNo, procNo));
            }

            AsmLot lot = AsmLot.GetAsmLot(lotNo);

            using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                SqlParameter paramMixTypeCd = cmd.Parameters.Add("@MixType_CD", SqlDbType.Char);
                SqlParameter paramResinGroupCd = cmd.Parameters.Add("@ResinGroup_CD", SqlDbType.Char);

                foreach (string cd in lot.ResinGpCd)
                {
                    string sql = @" SELECT ResinGroup_CD FROM JtmMIXRATE WITH(nolock) 
                                WHERE Del_FG = 0  AND ResinGroup_CD = @ResinGroup_CD 
                                AND UseMode_CD = '3'";  //UseMode_CD= 3 使用中
                    sql += $" AND MixType_CD IN ('{string.Join("','", p.MixTypeCd)}') ";
                    //AND MixType_CD = @MixType_CD
                    //paramMixTypeCd.Value = p.MixTypeCd;
                    paramResinGroupCd.Value = cd;

                    cmd.CommandText = sql;

                    object resinGroupCd = cmd.ExecuteScalar();
                    if (resinGroupCd != null)
                    {
                        if (string.IsNullOrEmpty(rResinGroup) == false)
                        {
                            //ロットに紐づく樹脂グループリストで、同一の調合タイプに紐づく樹脂グループがあった場合
                            //間違いなのでエラー
                            throw new Exception(string.Format(
                                @"ロットの樹脂グループから依頼する樹脂グループの特定ができませんでした。
                            同一調合タイプに紐づく樹脂グループがあります。LotNo:{0} 樹脂グループ:{1} 調合タイプCD:{2}", lotNo, string.Join(",", lot.ResinGpCd), string.Join(",", p.MixTypeCd)));
                        }

                        rResinGroup = resinGroupCd.ToString().Trim();
                    }
                }

                if (string.IsNullOrEmpty(rResinGroup))
                {
                    throw new Exception(string.Format(
                        @"ロットの樹脂グループから依頼する樹脂グループの特定ができませんでした。
                    樹脂調合システムの調合比マスタに存在するか確認して下さい。LotNo:{0} 樹脂グループ:{1} 調合タイプCD:{2}", lotNo, string.Join(",", lot.ResinGpCd), string.Join(",", p.MixTypeCd)));
                }
            }

            return rResinGroup;
        }

        /// <summary>
        /// 調合タイプ名
        /// </summary>
        /// <returns></returns>
        public static string GetMixTypeName(string mixTypeCd)
        {
            string retv = string.Empty;

            using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT General_NM FROM JtmGENPARTD WITH(nolock) 
                                WHERE General_CD = @General_CD AND GENERAL_KB = 1";　//1 = 調合タイプ

                cmd.Parameters.Add("@General_CD", SqlDbType.Char).Value = mixTypeCd;

                cmd.CommandText = sql;

                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    retv = rd["General_NM"].ToString().Trim();
                }
            }

            return retv;
        }

        private static bool isResinMixOrdered(string lotNo, int procNo)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT JttReqestLot.Lot_NO FROM dbo.JttReqestLot AS JttReqestLot WITH (nolock) 
                                INNER JOIN dbo.JttReqest AS JttReqest WITH (nolock) ON JttReqestLot.Req_ID = JttReqest.Req_ID
                                WHERE JttReqestLot.Lot_NO = @Lot_NO AND JttReqestLot.ArmsProc_NO = @ArmsProc_NO 
                                AND (JttReqest.Status_CD <> 9) "; // ステータス「削除」以外

                cmd.Parameters.Add("@Lot_NO", SqlDbType.NVarChar).Value = lotNo;
                cmd.Parameters.Add("@ArmsProc_NO", SqlDbType.BigInt).Value = procNo;

                cmd.CommandText = sql;

                object lot = cmd.ExecuteScalar();
                if (lot == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
