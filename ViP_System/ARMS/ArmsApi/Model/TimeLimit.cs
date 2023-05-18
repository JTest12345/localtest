using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    /// <summary>
    /// 時間監視確認結果
    /// </summary>
    public class LimitCheckResult
    {
        public enum ResultKb
        {
            Attension,
            Warning,
            Expired,
            Normal,
            Stop,
            None,
        }

        public string LotNo { get; set; }

        public string MagNo { get; set; }

        /// <summary>
        /// 時間監視マスタ
        /// </summary>
        public TimeLimit Limit { get; set; }


        public string FromProcNm
        {
            get
            {
                return Limit.TgtProc.InlineProNM;
            }
        }

        public string FromKb
        {
            get
            {
                return Limit.TgtKb.ToString();
            }
        }

        public string ChkProcNm
        {
            get
            {
                return Limit.ChkProc.InlineProNM;
            }
        }

        public string ChkKb
        {
            get
            {
                return Limit.ChkKb.ToString();
            }
        }


        /// <summary>
        /// 有効期限
        /// </summary>
        public DateTime LimitDt { get; set; }


        public int Remains
        {
            get
            {
                return (int)(LimitDt - DateTime.Now).TotalMinutes;
            }
        }

        /// <summary>
        /// 判定結果
        /// </summary>
        public ResultKb Result { get; set; }

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="limit"></param>
        /// <param name="fromdt"></param>
        /// <param name="limitdt"></param>
        /// <param name="kb"></param>
        public LimitCheckResult(string lotno, TimeLimit limit, DateTime limitdt, ResultKb kb)
        {
            this.Limit = limit;
            this.LotNo = lotno;

            Magazine[] mags = Magazine.GetMagazine(lotno, true);
            this.MagNo = string.Join(",", mags.Select(m => m.MagazineNo));

            this.LimitDt = limitdt;
            this.Result = kb;
        }
        #endregion
    }

    /// <summary>
    /// 有効期限監視マスタクラス
    /// </summary>
    public class TimeLimit
    {
        public enum JudgeKb
        {
            Start,
            End,
        }
		public static string GetNascaJudgeKb(JudgeKb jk)
		{
			if (jk == JudgeKb.Start){ return JUDGEKB_WORKSTART; }
			else{ return JUDGEKB_WORKEND;}
		}

        public const string JUDGEKB_WORKSTART = "S";
        public const string JUDGEKB_WORKEND = "E";

        public string TypeCd { get; set; }

        public Process ChkProc { get; set; }
        public string ChkWorkCd { get; set; }
        public JudgeKb ChkKb { get; set; }

        public Process TgtProc { get; set; }
        public string TgtWorkCd { get; set; }
        public JudgeKb TgtKb { get; set; }

        /// <summary>
        /// 調合樹脂有効期限監視では未使用
        /// </summary>
        public int EffectLimit { get; set; }
        public int AttensionBefore { get; set; }
        public int WarningBefore { get; set; }
		public int ChkLinNo { get; set; }

        public string ResinGroupCd { get; set; }

        #region 時間監視マスタ取得 GetLimit

        /// <summary>
        /// 時間監視マスタ取得
        /// </summary>
        /// <param name="typecd"></param>
        /// <returns></returns>
        public static TimeLimit[] GetLimits(string typecd, int? checkProcNo, bool includeDelete)
        {
            List<TimeLimit> retv = new List<TimeLimit>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    string sql = @"
                        SELECT 
                          typecd , 
                          chkworkcd , 
                          chkworkkb ,
                          tgtworkcd ,
                          tgtworkkb , 
                          attension , 
                          warning , 
                          effect ,
						  chklinno
                        FROM 
                          TmTimeLimit with(nolock)
                        WHERE 
                          1=1";

                    if (string.IsNullOrEmpty(typecd) == false)
                    {
                        cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
                        sql += " AND typecd = @TYPECD ";
                    }

                    if (!includeDelete)
                    {
                        sql += " AND delfg = 0 ";
                    }


                    if (checkProcNo.HasValue == true)
                    {
                        Process chkproc = Process.GetProcess(checkProcNo.Value);
                        sql += " AND chkworkcd = @CHKWORKCD ";
                        cmd.Parameters.Add("@CHKWORKCD", SqlDbType.NVarChar).Value = chkproc.WorkCd;
                    }

                    cmd.CommandText = sql;
                    cmd.CommandText = cmd.CommandText.Replace("\r\n", "");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TimeLimit l = new TimeLimit();
                            l.TypeCd = SQLite.ParseString(reader["typecd"]);

                            //ワークフローから
                            Process[] flow = Process.GetWorkFlow(l.TypeCd);

                            l.ChkWorkCd = SQLite.ParseString(reader["chkworkcd"]);

                            //CHK側工程
                            Process cp = flow.Where(p => p.WorkCd == l.ChkWorkCd).FirstOrDefault();
                            if (cp != null) l.ChkProc = cp;
                            else throw new ArmsException("作業実績監視マスタの監視作業が作業順マスタに見つかりません:" + l.TypeCd + ":" +l.ChkWorkCd);

                            //CHK側区分
                            string ckb = SQLite.ParseString(reader["chkworkkb"]);
                            if (ckb == JUDGEKB_WORKSTART) l.ChkKb = JudgeKb.Start;
                            else if (ckb == JUDGEKB_WORKEND) l.ChkKb = JudgeKb.End;
                            else throw new ArmsException("作業実績監視マスタの監視作業の監視区分が不正です:" + l.TypeCd + ":" + ckb);

                            l.TgtWorkCd = SQLite.ParseString(reader["tgtworkcd"]);

                            //TGT側工程
                            Process tp = flow.Where(p => p.WorkCd == l.TgtWorkCd).FirstOrDefault();
                            if (tp != null) l.TgtProc = tp;
                            else throw new ArmsException("作業実績監視マスタの対象作業が作業順マスタに見つかりません: " + l.TypeCd + ":" + l.TgtWorkCd);

                            //TGT側区分
                            string tkb = SQLite.ParseString(reader["tgtworkkb"]);
                            if (tkb == JUDGEKB_WORKSTART) l.TgtKb = JudgeKb.Start;
                            else if (tkb == JUDGEKB_WORKEND) l.TgtKb = JudgeKb.End;
                            else throw new ArmsException("作業実績監視マスタの対象作業の対象作業区分が不正です:" + l.TypeCd + ":" + tkb);

                            l.AttensionBefore = SQLite.ParseInt(reader["attension"]);
                            l.WarningBefore = SQLite.ParseInt(reader["warning"]);
                            l.EffectLimit = SQLite.ParseInt(reader["effect"]);

                            l.ChkLinNo = SQLite.ParseInt(reader["chklinno"]);

                            retv.Add(l);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }

            return retv.ToArray();
        }

        public static TimeLimit[] GetLimits(string typecd, string targetWorkCd, JudgeKb targetWorkKb, string checkWorkCd, JudgeKb checkWorkKb)
        {
            Process proc = Process.GetProcess(checkWorkCd);
            TimeLimit[] limits = GetLimits(typecd, proc.ProcNo);

            limits = limits.Where(l => l.ChkKb == checkWorkKb && l.TgtWorkCd == targetWorkCd && l.TgtKb == targetWorkKb).ToArray();
            if (limits.Length == 0) 
            {
                throw new ApplicationException(
                    string.Format("時間監視マスタに登録されていません。型番:{0} 開始作業:{1},{2} 完了作業:{3},{4}",
                    typecd, targetWorkCd, targetWorkKb, checkWorkCd, checkWorkKb));
            }
                        
            return limits;
        }

		public static TimeLimit[] GetLimits(string typecd, bool includeDelFg) 
		{
			return GetLimits(typecd, null, includeDelFg);
		}

		public static TimeLimit[] GetLimits(string typecd, int? checkProcNo)
		{
			return GetLimits(typecd, checkProcNo, false);
		}

        #endregion

        public static TimeLimit GetResinLimit(string resingroupCd)
        {
            TimeLimit retv = null;

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                string sql = @" SELECT resingroupcd, attension, warning FROM TmResinTimeLimit WITH(nolock) 
                                WHERE delfg = 0 AND resingroupcd = @RESINGROUPCD ";

                cmd.Parameters.Add("RESINGROUPCD", SqlDbType.NVarChar).Value = resingroupCd;

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv = new TimeLimit();

                        retv.ResinGroupCd = SQLite.ParseString(rd["resingroupcd"]);
                        retv.AttensionBefore = SQLite.ParseInt(rd["attension"]);
                        retv.WarningBefore = SQLite.ParseInt(rd["warning"]);
                    }
                }
            }

            return retv;
        }

        public static void Delete(string lotno)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    string sql = " DELETE FROM TnTimeLimitChecked WHERE lotno = @LotNo";
                    cmd.CommandText = sql;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@LotNo", System.Data.SqlDbType.NVarChar).Value = lotno;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        #region 警告無効フラグ

        /// <summary>
        /// 警告無効フラグON
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="chkWorkCd"></param>
        /// <param name="chkWorkKb"></param>
        /// <param name="tgtWorkCd"></param>
        /// <param name="tgtWorkKb"></param>
        public static void Check(string lotno, string chkWorkCd, JudgeKb chkWorkKb, string tgtWorkCd, JudgeKb tgtWorkKb)
        {
            if (IsChecked(lotno, chkWorkCd, chkWorkKb, tgtWorkCd, tgtWorkKb) == true)
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                cmd.Parameters.Add("@CHKWORKCD", SqlDbType.NVarChar).Value = chkWorkCd ?? "";
                cmd.Parameters.Add("@CHKWORKKB", SqlDbType.NVarChar).Value = (chkWorkKb == JudgeKb.Start) ? JUDGEKB_WORKSTART : JUDGEKB_WORKEND;
                cmd.Parameters.Add("@TGTWORKCD", SqlDbType.NVarChar).Value = tgtWorkCd ?? "";
                cmd.Parameters.Add("@TGTWORKKB", SqlDbType.NVarChar).Value = (tgtWorkKb == JudgeKb.Start) ? JUDGEKB_WORKSTART : JUDGEKB_WORKEND;
                cmd.Parameters.Add("@CHKDT", SqlDbType.DateTime).Value = DateTime.Now;

                try
                {
                    con.Open();
                    cmd.CommandText = @"
                        INSERT INTO TnTimeLimitChecked(lotno, chkworkcd, chkworkkb, tgtworkcd, tgtworkkb, checkdt)
                        VALUES (@LOTNO, @CHKWORKCD, @CHKWORKKB, @TGTWORKCD, @TGTWORKKB, @CHKDT)";

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }
        }


        /// <summary>
        /// 警告無効フラグ確認
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="chkWorkCd"></param>
        /// <param name="chkWorkKb"></param>
        /// <param name="tgtWorkCd"></param>
        /// <param name="tgtWorkKb"></param>
        /// <returns></returns>
        public static bool IsChecked(string lotno, string chkWorkCd, JudgeKb chkWorkKb, string tgtWorkCd, JudgeKb tgtWorkKb)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                cmd.Parameters.Add("@CHKWORKCD", SqlDbType.NVarChar).Value = chkWorkCd ?? "";
                cmd.Parameters.Add("@CHKWORKKB", SqlDbType.NVarChar).Value = (chkWorkKb == JudgeKb.Start) ? JUDGEKB_WORKSTART : JUDGEKB_WORKEND;
                cmd.Parameters.Add("@TGTWORKCD", SqlDbType.NVarChar).Value = tgtWorkCd ?? "";
                cmd.Parameters.Add("@TGTWORKKB", SqlDbType.NVarChar).Value = (tgtWorkKb == JudgeKb.Start) ? JUDGEKB_WORKSTART : JUDGEKB_WORKEND;

                try
                {
                    con.Open();
                    cmd.CommandText = @"
                        SELECT 
                          checkdt
                        FROM 
                          TnTimeLimitChecked
                        WHERE 
                          lotno = @LOTNO AND chkworkcd=@CHKWORKCD AND chkworkkb=@chkworkkb
                           AND tgtworkcd=@TGTWORKCD AND tgtworkkb=@TGTWORKKB";

                    object dt = cmd.ExecuteScalar();
                    if (dt != null) return true;

                    return false;
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }

            }
        }
        #endregion

        /// <summary>
        /// 対象工程がcheckになっている期限切れのみ取得
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="targetProc"></param>
        /// <returns></returns>
        public static LimitCheckResult[] CheckProcExpired(string typeCd, AsmLot lot, int checkProc, Order newOrder)
        {
            TimeLimit[] limitList = GetLimits(typeCd, checkProc);


            LimitCheckResult[] result = CheckTimeLimit(lot, limitList, newOrder, false, null);
            List<LimitCheckResult> retv = new List<LimitCheckResult>();

            foreach (LimitCheckResult res in result)
            {
                //Expireのみ返す
                if (res.Result == LimitCheckResult.ResultKb.Expired)
                {
                    retv.Add(res);
                }
            }

            return retv.ToArray();
        }

        #region 有効期限切れ判定 CheckTimeLimit



        /// <summary>
        /// 期限切れ判定
        /// macnoListはnull許可。設定した場合は起点側の作業でそのコードが含まれていない場合は無視
        /// </summary>
        /// <param name="lot"></param>
        /// <returns></returns>
        public static LimitCheckResult[] CheckTimeLimit(AsmLot lot, TimeLimit[] limitList, Order newOrder, bool removeFinished, int[] macnoList)
        {
            List<LimitCheckResult> retv = new List<LimitCheckResult>();

            Order[] orders = Order.GetOrder(lot.NascaLotNo);
            if (newOrder != null)
            {
                Array.Resize(ref orders, orders.Length + 1);
                orders[orders.Length - 1] = newOrder;
            }

            foreach (TimeLimit limit in limitList)
            {
                //マイナス作業時間はここでは判定しない
                if (limit.EffectLimit < 0) continue;

                //監視開始時間（tgt側の開始or完了で上書き）
                DateTime from = DateTime.MaxValue.AddYears(-2);

                Order[] selectedTgt = orders.Where(o => o.ProcNo == limit.TgtProc.ProcNo).ToArray();
                //TGT側作業未開始の場合はその条件は無視
                if (selectedTgt.Length == 0)
                {
                    continue;
                }

                foreach (Order tgt in selectedTgt)
                {
                    //macnoListに指定がある場合、起点側作業に含まれていなければ無視
                    if (macnoList != null)
                    {
                        if (macnoList.Contains(tgt.MacNo) == false) continue;
                    }

                    //TGT側が完了からで、実作業が未完了の場合も無視
                    if (limit.TgtKb == JudgeKb.End)
                    {
                        if (tgt.WorkEndDt.HasValue == false)
                        {
                            continue;
                        }
                        else
                        {
                            from = tgt.WorkEndDt.Value;
                        }
                    }
                    else
                    {
                        from = tgt.WorkStartDt;
                    }


                    Order[] selectedChk = orders.Where(o => o.ProcNo == limit.ChkProc.ProcNo).ToArray();

                    Process.MagazineDevideStatus tgtdst = Process.GetMagazineDevideStatus(lot, limit.TgtProc.ProcNo);
                    Process.MagazineDevideStatus chkdst = Process.GetMagazineDevideStatus(lot, limit.ChkProc.ProcNo);

                    if (tgtdst == Process.MagazineDevideStatus.Single || tgtdst == Process.MagazineDevideStatus.DoubleToSingle)
                    {
                        if (chkdst == Process.MagazineDevideStatus.Single || chkdst == Process.MagazineDevideStatus.DoubleToSingle)
                        {
                            #region 単マガジン→単マガジンの判定

                            if (limit.ChkKb == JudgeKb.Start)
                            {
                                if (removeFinished && selectedChk.Length >= 1)
                                {
                                    //開始までチェックならOrderがある時点で対象外
                                    continue;
                                }
                                else
                                {
                                    if (selectedChk.Length >= 1)
                                    {
                                        retv.Add(calcResult(tgt, selectedChk[0], from, limit));
                                    }
                                    else
                                    {
                                        retv.Add(calcResult(tgt, null, from, limit));
                                    }
                                }
                            }
                            else
                            {
                                if (selectedChk.Length == 0)
                                {
                                    //CHK側にレコード無ければ現在時刻と比較
                                    retv.Add(calcResult(tgt, null, from, limit));
                                }
                                else if (removeFinished)
                                {
                                    if (!selectedChk[0].IsComplete)
                                    {
                                        //未完了の場合
                                        retv.Add(calcResult(tgt, selectedChk[0], from, limit));
                                    }
                                }
                                else
                                {
                                    //完了・未完了に関わらず
                                    retv.Add(calcResult(tgt, selectedChk[0], from, limit));
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            #region 単マガジン→分割マガジンの判定
                            if (limit.ChkKb == JudgeKb.Start)
                            {
                                if (removeFinished)
                                {
                                    if (selectedChk.Length == 2)
                                    {
                                        //開始レコードが2つ揃っている場合は無視
                                        continue;
                                    }
                                    else
                                    {
                                        //1つ以下なら現在時刻と比較
                                        retv.Add(calcResult(tgt, null, from, limit));
                                    }
                                }
                                else
                                {
                                    foreach (Order o in selectedChk)
                                    {
                                        retv.Add(calcResult(tgt, o, from, limit));
                                    }
                                    if (selectedChk.Length < 2)
                                    {
                                        //1つ以下なら現在時刻と比較
                                        retv.Add(calcResult(tgt, null, from, limit));
                                    }
                                }
                            }
                            else
                            {

                                if (selectedChk.Length == 0)
                                {
                                    //CHK側にレコード無ければ現在時刻と比較
                                    retv.Add(calcResult(tgt, null, from, limit));
                                }
                                else if (removeFinished)
                                {

                                    foreach (Order o in selectedChk)
                                    {
                                        if (o.IsComplete)
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            retv.Add(calcResult(tgt, o, from, limit));
                                        }
                                    }

                                    //完了指図数が2に満たなければ現在時刻と比較
                                    if (selectedChk.Length < 2)
                                    {
                                        retv.Add(calcResult(tgt, null, from, limit));
                                    }
                                }
                                else
                                {
                                    foreach (Order o in selectedChk)
                                    {
                                        retv.Add(calcResult(tgt, o, from, limit));
                                    }

                                    //完了指図数が2に満たなければ現在時刻と比較
                                    if (selectedChk.Length < 2)
                                    {
                                        retv.Add(calcResult(tgt, null, from, limit));
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                    else
                    {
                        if (chkdst == Process.MagazineDevideStatus.Single || chkdst == Process.MagazineDevideStatus.DoubleToSingle)
                        {
                            #region 複マガジン→単マガジンの判定

                            if (limit.ChkKb == JudgeKb.Start)
                            {
                                //開始側
                                if (removeFinished && selectedChk.Length >= 1)
                                {
                                    //開始までチェックならOrderがある時点で対象外
                                    continue;
                                }
                                else
                                {
                                    if (selectedChk.Length >= 1)
                                    {
                                        retv.Add(calcResult(tgt, selectedChk[0], from, limit));
                                    }
                                    else
                                    {
                                        retv.Add(calcResult(tgt, null, from, limit));
                                    }
                                }
                            }
                            else
                            {
                                //完了側
                                Order chk = selectedChk.Where(c => c.DevidedMagazineSeqNo == 0).FirstOrDefault();
                                if (chk == null)
                                {
                                    //CHK側にレコード無ければ現在時刻と比較
                                    retv.Add(calcResult(tgt, null, from, limit));
                                }
                                else if (removeFinished)
                                {
                                    if (!chk.IsComplete)
                                    {
                                        //未完了の場合
                                        retv.Add(calcResult(tgt, chk, from, limit));
                                    }
                                }
                                else
                                {
                                    //完了・未完了に関わらず
                                    retv.Add(calcResult(tgt, chk, from, limit));
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            Order chk = selectedChk.Where(c => c.LotNo == tgt.LotNo).FirstOrDefault();
                            #region 複マガジン→複マガジンの判定
                            if (limit.ChkKb == JudgeKb.Start)
                            {
                                if (removeFinished)
                                {
                                    if (chk != null)
                                    {
                                        //開始レコードがある場合は無視
                                        continue;
                                    }
                                    else
                                    {
                                        //無ければ現在時刻と比較
                                        retv.Add(calcResult(tgt, null, from, limit));
                                    }
                                }
                                else
                                {
                                    if (chk != null)
                                    {
                                        retv.Add(calcResult(tgt, chk, from, limit));
                                    }
                                    else
                                    {
                                        //無ければ現在時刻と比較
                                        retv.Add(calcResult(tgt, null, from, limit));
                                    }
                                }
                            }
                            else
                            {
                                if (chk == null)
                                {
                                    //CHK側にレコード無ければ現在時刻と比較
                                    retv.Add(calcResult(tgt, null, from, limit));
                                }
                                else if (removeFinished)
                                {
                                    if (!chk.IsComplete)
                                    {
                                        //未完了の場合
                                        retv.Add(calcResult(tgt, chk, from, limit));
                                    }
                                }
                                else
                                {
                                    retv.Add(calcResult(tgt, chk, from, limit));
                                }
                            }
                            #endregion
                        }
                    }
                }
            }


            return retv.ToArray();
        }

        private static LimitCheckResult calcResult(Order tgt, Order chk, DateTime from, TimeLimit limit)
        {
            //チェック側の開始・完了時間（実績なしの場合は現在時刻）
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            if (chk != null)
            {
                start = chk.WorkStartDt;
                if (chk.WorkEndDt.HasValue) end = chk.WorkEndDt.Value;
            }


            //有効期限確定
            DateTime ExpiredDt = from.AddMinutes(limit.EffectLimit);


            //期限切れ判定
            bool isExpired = isLimitOver(ExpiredDt, limit.ChkKb, start, end);
            if (isExpired)
            {
                return new LimitCheckResult(tgt.LotNo, limit, ExpiredDt, LimitCheckResult.ResultKb.Expired);
            }

            //Warning判定
            bool isWarning = isLimitOver(ExpiredDt.AddMinutes(-1 * limit.WarningBefore), limit.ChkKb, start, end);
            if (isWarning)
            {
                return new LimitCheckResult(tgt.LotNo, limit, ExpiredDt, LimitCheckResult.ResultKb.Warning);
            }

            //Attension判定
            bool isAttention = isLimitOver(ExpiredDt.AddMinutes(-1 * limit.AttensionBefore), limit.ChkKb, start, end);
            if (isAttention)
            {
                return new LimitCheckResult(tgt.LotNo, limit, ExpiredDt, LimitCheckResult.ResultKb.Attension);
            }

            return new LimitCheckResult(tgt.LotNo, limit, ExpiredDt, LimitCheckResult.ResultKb.Normal);

        }



        /// <summary>
        /// 時間超過判定
        /// </summary>
        /// <param name="expired"></param>
        /// <param name="kb"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static bool isLimitOver(DateTime expired, JudgeKb kb, DateTime start, DateTime end)
        {
            if (kb == JudgeKb.Start)
            {
                if (expired <= start)
                {
                    return true;
                }
            }
            else
            {
                if (expired <= end)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="checkProc"></param>
        /// <param name="newOrder"></param>
        /// <returns></returns>
        public static bool IsErrorStartWork(string lotNo, string typeCd, int checkProc, Order newOrder)
        {
            TimeLimit[] limitList = GetLimits(typeCd, checkProc);

            bool needcheck = false;
            foreach (TimeLimit limit in limitList)
            {
                //マイナス作業時間監視のみチェックを行う
                if (limit.EffectLimit < 0) needcheck = true;
            }

            //マイナスの作業時間監視条件が一つも無い場合は無条件でNG無し
            if (needcheck == false) return false;


            Order[] orders = Order.GetOrder(lotNo);
            if (newOrder != null)
            {
                Array.Resize(ref orders, orders.Length + 1);
                orders[orders.Length - 1] = newOrder;
            }

            foreach (TimeLimit limit in limitList)
            {
                //マイナス作業時間監視のみチェックを行う
                if (limit.EffectLimit > 0) continue;
                //CHK側がStartになっていない条件も無視
                if (limit.ChkKb != JudgeKb.Start) continue;

                //監視開始時間（tgt側の開始or完了で上書き）
                DateTime from = DateTime.MaxValue.AddYears(-2);

                #region tgt側確定　作業未開始の場合は条件スキップ

                Order[] selectedTgt = orders.Where(o => o.ProcNo == limit.TgtProc.ProcNo).ToArray();

                //TGT側作業未開始の場合はその条件は無視
                if (selectedTgt.Length == 0)
                {
                    continue;
                }
                else
                {
                    Order tgt = selectedTgt[0];

                    //TGT側が完了からで、実作業が未完了の場合も無視
                    if (limit.TgtKb == JudgeKb.End)
                    {
                        if (tgt.WorkEndDt.HasValue == false)
                        {
                            continue;
                        }
                        else
                        {
                            from = tgt.WorkEndDt.Value;
                        }
                    }
                    else
                    {
                        from = tgt.WorkStartDt;
                    }
                }
                #endregion

                Order chk = null;
                Order[] selectedChk = orders.Where(o => o.ProcNo == limit.ChkProc.ProcNo).ToArray();
                if (selectedChk.Length >= 1)
                {
                    chk = selectedChk[0];
                }

                //チェック側の開始・完了時間（実績なしの場合は現在時刻）
                DateTime start = DateTime.Now;
                if (chk != null)
                {
                    start = chk.WorkStartDt;
                }

                if (start.AddMinutes(limit.EffectLimit) < from)
                {
                    return true;
                }
            }

            return false;
        }





    }
}
