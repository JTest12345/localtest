using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ArmsApi.Model
{
    public class MachineInfo
    {
        public const int STOCKER_TYPE_DOUBLE_FRAMELOADER = 1;
        public const int STOCKER_TYPE_WAFER_CHANGER = 2;

        #region プロパティ
        
        /// <summary>
        /// 現在有効フラグ (Armsメンテナンスの設備マスタ編集用)
        /// </summary>
        public bool ColAvailable
        {
            get { return !DelFg; }
            set { DelFg = !value; }
        }

        /// <summary>
        /// インライン設備CD
        /// </summary>
        public int MacNo { get; set; }

        /// <summary>
        /// 設備CD
        /// </summary>
        public string NascaPlantCd { get; set; }

        /// <summary>
        /// 設備名
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// 分類 日本名（設備コード）
        /// </summary>
        public string LongName
        {
            get { return ClassName + " " + MachineName + "(" + NascaPlantCd + ")"; }
        }

        /// <summary>
        /// 分類名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 樹脂チェックフラグ
        /// </summary>
        public bool ResinCheckFg { get; set; }

        /// <summary>
        /// ウェハー判定フラグ
        /// </summary>
        public bool WaferCheckFg { get; set; }

        /// <summary>
        /// ウェハーチェンジャー有り
        /// </summary>
        public bool HasWaferChanger { get; set; }

        /// <summary>
        /// 2ストッカーあり
        /// </summary>
        public bool HasDoubleStocker { get; set; }

        /// <summary>
        /// アウトライン装置フラグ
        /// </summary>
        public bool IsOutLine { get; set; }

        /// <summary>
        /// 自動化ライン装置フラグ
        /// </summary>
        public bool IsAutoLine { get; set; }

        /// <summary>
        /// 高効率ライン装置フラグ
        /// </summary>
        public bool IsHighLine { get; set; }

        /// <summary>
        /// 新型自動搬送ライン(AGV搬送ライン)装置フラグ
        /// </summary>
        public bool IsAgvLine { get; set; }

        /// <summary>
        /// マガジン反転フラグ
        /// </summary>
        public bool RevDeployFg { get; set; }

        public bool DelFg { get; set; }

        public List<string> MacGroup { get; set; }

        /// <summary>
        /// 外部マガジン(アオイ基板マガジン)使用フラグ
        /// </summary>
        public bool OuterMagFg { get; set; }

        /// <summary>
        /// マガジン変更
        /// </summary>
        public bool MagazineChgFg { get; set; }

        /// <summary>
        /// 開始、完了要求(**.in, **.outファイルの要求)をする
        /// </summary>
        public bool RequestStartEndFg { get; set; }

        //2016.09.29 フォルダの持ち方変更のため廃止
        ///// <summary>
        ///// 開始フォルダ
        ///// </summary>
        //public string WatchingDirectoryPath { get; set; }

        /// <summary>
        /// 傾向管理・マッピングインプットフォルダ
        /// </summary>
        public string LogInputDirectoryPath { get; set; }
        /// <summary>
        /// 傾向管理・マッピングアウトプットフォルダ
        /// </summary>
        public string LogOutputDirectoryPath { get; set; }
        /// <summary>
        /// マッピングインプットフォルダ【※傾向管理とマッピングの出力先が異なる場合のみ使用】
        /// </summary>
        public string AltMapInputDirectoryPath { get; set; }
        /// <summary>
        /// マッピングアウトプットフォルダ【※傾向管理とマッピングの出力先が異なる場合のみ使用】
        /// </summary>
        public string AltMapOutputDirectoryPath { get; set; }

        /// <summary>
        /// 基板作業処理ありか否かフラグ
        /// </summary>
        public bool IsSubstrateComplete { get; set; }

        /// <summary>
        /// CV⇒装置へのマガジン移動要求(開始通知)
        /// </summary>
        public bool RequestMoveStartMagFg { get; set; }

        /// <summary>
        /// ローダー部最大積載マガジン数
        /// </summary>
        public int LoaderMagazineMaxCt { get; set; }

        /// <summary>
        /// ラインNo (元LineConfig.xmlの＜lineno＞タグ
        /// </summary>
        public string LineNo { get; set; }

        public string OperationMacGroupCd { get; set; }

        public bool Canredoprocessfg { get; set; }

        /// <summary>
        /// ARMS未接続装置かどうかのフラグ。(Webの開始登録時の処理に影響)
        /// </summary>
        public bool IsNoConnected { get; set; }

        /// <summary>
        /// ARMS3の信号確認画面のタブ名
        /// </summary>
        public string SignalDisplayFlowName { get; set; }

        /// <summary>
        /// 照合ログ出力フォルダ【※現状はMD3D測定機から戻ってきたトレイの照合結果のファイル出力に使用】
        /// </summary>
        public string VerificationLogOutputDirectoryPath { get; set; }

        #endregion

        #region インライン設備マスタ情報取得

        /// <summary>
        /// 全装置を返す
        /// </summary>
        /// <returns></returns>
        public static MachineInfo[] GetMachineList(bool onlyInline)
        {
            return SearchMachine(null, null, onlyInline, false);
        }

        /// <summary>
        /// インライン設備マスタ情報取得
        /// </summary>
        /// <param name="schParam">検索条件</param>
        /// <returns>インライン設備マスタ構造体</returns>
        public static MachineInfo GetMachine(int inlineMachineNo)
        {
            return GetMachine(inlineMachineNo, false);
        }

        public static MachineInfo GetMachine(int inlineMachineNo, bool includeDelFg)
        {
            MachineInfo[] maclist = SearchMachine(inlineMachineNo, null, false, false, includeDelFg, null, null, null);
            return maclist.Where(m => m.MacNo == inlineMachineNo).FirstOrDefault();
        }

        public static MachineInfo GetMachine(string nascaPlantCd)
        {
            MachineInfo[] maclist = SearchMachine(null, nascaPlantCd, false, false, false, null, null, null);
            return maclist.Where(m => m.NascaPlantCd == nascaPlantCd).FirstOrDefault();
        }

        public static MachineInfo[] SearchMachine(int? macno, string plantcd, bool onlyInline, bool onlyOutline)
        {
            return SearchMachine(macno, plantcd, onlyInline, onlyOutline, false, null, null, null);
        }

        public static MachineInfo[] SearchMachine(int? macno, string plantcd, bool onlyInline, bool onlyOutline, bool includeDelFg, string operationMacgroupCd,
            string clasnm, string lineno)
        {
            List<MachineInfo> retv = new List<MachineInfo>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT macno, plantcd, clasnm, plantnm, resinchkfg, waferchkfg, partschkfg, outline, delfg
                                        , stockertype, macgroup, outermagfg, magazinechgfg, autoline, highline, revdeployfg
                                        , requeststartendfg, requestmovestartmagfg, issubstratecomplete, requestmovestartmagfg
                                        , agvline, loadermagazinemaxct, loginputdirectorypath, logoutputdirectorypath
                                        , altmapinputdirectorypath, altmapoutputdirectorypath, [lineno], operationmacgroupcd
                                        , canredoprocessfg, isnoconnected, signaldisplayflownm, verificationlogoutputdirectorypath
										FROM TmMachine WITH(nolock) WHERE 1=1 ";

                if (macno != null)
                {
                    sql += " AND (macno = @macno) ";
                    cmd.Parameters.Add("@macno", SqlDbType.BigInt).Value = macno;
                }
                if (!string.IsNullOrEmpty(plantcd))
                {
                    sql += " AND (plantcd = @plantcd) ";
                    cmd.Parameters.Add("@plantcd", SqlDbType.NVarChar).Value = plantcd;
                }
                if (onlyInline)
                {
                    sql += " AND (outline = 0) ";
                }
                if (!includeDelFg)
                {
                    sql += " AND (delfg = 0) ";
                }

                if (string.IsNullOrEmpty(operationMacgroupCd) == false)
                {
                    sql += " AND (operationmacgroupcd = @operationmacgroupcd) ";
                    cmd.Parameters.Add("@operationmacgroupcd", SqlDbType.NVarChar).Value = operationMacgroupCd;
                }

                if (string.IsNullOrEmpty(clasnm) == false)
                {
                    sql += " AND (clasnm like @clasnm) ";
                    cmd.Parameters.Add("@clasnm", SqlDbType.NVarChar).Value = "%" + clasnm + "%";
                }

                if (string.IsNullOrEmpty(lineno) == false)
                {
                    sql += " AND ([lineno] = @lineno) ";
                    cmd.Parameters.Add("@lineno", SqlDbType.NVarChar).Value = lineno;
                }

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    int ordMacGroup = rd.GetOrdinal("macgroup");
                    while (rd.Read())
                    {
                        MachineInfo mi = new MachineInfo();
                        mi.MacNo = SQLite.ParseInt(rd["macno"]);
                        mi.NascaPlantCd = SQLite.ParseString(rd["plantcd"]);
                        mi.ClassName = SQLite.ParseString(rd["clasnm"]);
                        mi.MachineName = SQLite.ParseString(rd["plantnm"]);
                        mi.ResinCheckFg = SQLite.ParseBool(rd["resinchkfg"]);
                        mi.WaferCheckFg = SQLite.ParseBool(rd["waferchkfg"]);
                        mi.HasWaferChanger = SQLite.ParseBool(rd["partschkfg"]);
                        mi.IsOutLine = SQLite.ParseBool(rd["outline"]);
                        mi.IsAutoLine = SQLite.ParseBool(rd["autoline"]);
                        mi.IsHighLine = SQLite.ParseBool(rd["highline"]);
                        mi.DelFg = SQLite.ParseBool(rd["delfg"]);
                        mi.RevDeployFg = SQLite.ParseBool(rd["revdeployfg"]);
                        //mi.LongName = string.Format("{0}({1})", mi.MachineName, mi.NascaPlantCd);
                        if (SQLite.ParseInt(rd["stockertype"]) == STOCKER_TYPE_DOUBLE_FRAMELOADER)
                        {
                            mi.HasDoubleStocker = true;
                        }
                        else if (SQLite.ParseInt(rd["stockertype"]) == STOCKER_TYPE_WAFER_CHANGER)
                        {
                            mi.HasWaferChanger = true;
                        }
                        mi.MacGroup = new List<string>();
                        if (!rd.IsDBNull(ordMacGroup))
                        {
                            mi.MacGroup.AddRange(rd.GetString(ordMacGroup).Split(','));
                        }
                        mi.OuterMagFg = SQLite.ParseBool(rd["outermagfg"]);
                        mi.MagazineChgFg = SQLite.ParseBool(rd["magazinechgfg"]);
                        mi.RequestStartEndFg = SQLite.ParseBool(rd["requeststartendfg"]);
                        mi.LogInputDirectoryPath = SQLite.ParseString(rd["loginputdirectorypath"]);
                        mi.LogOutputDirectoryPath = SQLite.ParseString(rd["logoutputdirectorypath"]);
                        mi.AltMapInputDirectoryPath = SQLite.ParseString(rd["altmapinputdirectorypath"]);
                        mi.AltMapOutputDirectoryPath = SQLite.ParseString(rd["altmapoutputdirectorypath"]);
                        mi.IsSubstrateComplete = SQLite.ParseBool(rd["issubstratecomplete"]);
                        mi.RequestMoveStartMagFg = SQLite.ParseBool(rd["requestmovestartmagfg"]);
                        mi.IsAgvLine = SQLite.ParseBool(rd["agvline"]);
                        mi.LoaderMagazineMaxCt = SQLite.ParseInt(rd["loaderMagazineMaxCt"]);
                        mi.LineNo = SQLite.ParseString(rd["lineno"]);
                        mi.OperationMacGroupCd = SQLite.ParseString(rd["OperationMacGroupCd"]);
                        mi.Canredoprocessfg = SQLite.ParseBool(rd["canredoprocessfg"]);
                        mi.IsNoConnected = SQLite.ParseBool(rd["isnoconnected"]);
                        mi.SignalDisplayFlowName = SQLite.ParseString(rd["signaldisplayflownm"]);
                        mi.VerificationLogOutputDirectoryPath = SQLite.ParseString(rd["verificationlogoutputdirectorypath"]);
                        retv.Add(mi);
                    }
                }
            }

            return retv.ToArray();
        }

        public static MachineInfo[] GetOperationMachines(string operationMacgroupCd)
        {
            return SearchMachine(null, null, false, false, false, operationMacgroupCd, null, null);
        }

        #endregion

        #region 設備投入資材の更新関連 DeleteInsertMacMat
        public void DeleteInsertMacMat(Material mat)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {

                try
                {
                    con.Open();

                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.NVarChar).Value = this.MacNo;
                    cmd.Parameters.Add("@MATCD", System.Data.SqlDbType.NVarChar).Value = mat.MaterialCd ?? "";
                    cmd.Parameters.Add("@STOCKERNO", SqlDbType.BigInt).Value = mat.StockerNo;
                    cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.NVarChar).Value = mat.LotNo ?? "";
                    cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = (object)mat.InputDt ?? DateTime.Now;
                    cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = (object)mat.RemoveDt ?? DBNull.Value;
                    cmd.Parameters.Add("@ISSAMPLED", SqlDbType.Int).Value = SQLite.SerializeBool(mat.IsTimeSampled);
                    cmd.Parameters.Add("@RINGID", System.Data.SqlDbType.NVarChar).Value = mat.RingId ?? "";

                    cmd.CommandText = @"
                        DELETE FROM TnMacMat 
                        WHERE macno=@MACNO AND materialcd=@MATCD AND lotno=@LOTNO
                        AND stockerno=@STOCKERNO AND startdt=@STARTDT";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                        INSERT INTO TnMacMat(macno, stockerno, materialcd, lotno, startdt, enddt, issampled, ringid)
                        VALUES (@MACNO, @STOCKERNO, @MATCD, @LOTNO, @STARTDT, @ENDDT, @ISSAMPLED, @RINGID)";

                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("投入資材更新エラー", ex);
                }
            }
        }
        #endregion

        #region 設備投入樹脂の更新関連 DeleteInsertMacResin
        public void DeleteInsertMacResin(Resin resin)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {

                try
                {
                    con.Open();

                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.MacNo;
                    cmd.Parameters.Add("@MIXRESID", SqlDbType.NVarChar).Value = resin.MixResultId;
                    cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = (object)resin.InputDt ?? DBNull.Value;
                    cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = (object)resin.RemoveDt ?? DBNull.Value;
                    cmd.Parameters.Add("@ISNASCASTART", SqlDbType.Int).Value = SQLite.SerializeBool(resin.IsNascaStart);
                    cmd.Parameters.Add("@ISNASCAEND", SqlDbType.Int).Value = SQLite.SerializeBool(resin.IsNascaEnd);

                    cmd.CommandText = @"
                        DELETE FROM TnMacResin WHERE macno=@MACNO AND mixresultid=@MIXRESID AND startdt=@STARTDT";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                    INSERT INTO TnMacResin(macno, mixresultid, startdt, enddt, isnascastart, isnascaend)
                        VALUES(@MACNO, @MIXRESID, @STARTDT, @ENDDT, @ISNASCASTART, @ISNASCAEND)";

                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("投入資材更新エラー", ex);
                }
            }
        }
        #endregion

        #region 製造条件の更新関連 DeleteInsertWorkCond

        /// <summary>
        /// 新規製造条件割り付け
        /// </summary>
        /// <param name="cnd"></param>
        public void DeleteInsertWorkCond(WorkCondition cnd)
        {
            Log.SysLog.Info("製造条件割り付け" + this.MacNo + ":" + cnd.CondCd + ":" + cnd.CondVal);
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {

                try
                {
                    con.Open();

                    cmd.Transaction = con.BeginTransaction();
                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.MacNo;
                    cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar).Value = cnd.CondCd ?? "";
                    cmd.Parameters.Add("@CONDVAL", SqlDbType.NVarChar).Value = cnd.CondVal ?? "";
                    cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = cnd.StartDt;
                    cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = (object)cnd.EndDt ?? DBNull.Value;

                    cmd.CommandText = @"
                        DELETE FROM TnMacCond WHERE macno=@MACNO AND condcd=@CONDCD AND startdt=@STARTDT";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                        INSERT INTO TnMacCond(macno, condcd, condval, startdt, enddt)
                        VALUES (@MACNO, @CONDCD, @CONDVAL, @STARTDT, @ENDDT)";

                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("製造条件割り付け更新エラー", ex);
                }
            }
        }
        #endregion

        #region 期間中に割り付いていた製造条件特性リスト取得 GetWorkConditions
        public WorkCondition[] GetWorkConditions(DateTime startdt, DateTime? enddt)
        {
            return GetWorkConditions(null, startdt, enddt);
        }

        /// <summary>
        /// 指定期間に割り付いていた特性リスト取得
        /// </summary>
        /// <param name="macno"></param>
        /// <param name="condcd"></param>
        /// <param name="startdt"></param>
        /// <param name="enddt"></param>
        /// <returns></returns>
        public WorkCondition[] GetWorkConditions(string condcd, DateTime startdt, DateTime? enddt)
        {
            //指定期間に割り付いていた製造条件リスト取得
            //enddtが指定されている場合は取り外し日付とも照合
            //ログを残すこと

            DateTime compDate = DateTime.Now;
            if (enddt.HasValue)
            {
                compDate = enddt.Value;
            }

            List<WorkCondition> retv = new List<WorkCondition>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT
                        macno
                        , condcd
                        , condval
                        , startdt
                        , enddt
                        FROM TnMacCond
                        WHERE macno=@MACNO
                        AND startdt<=@ENDDT AND (enddt IS NULL OR enddt >= @STARTDT)";

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.MacNo;
                    cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = startdt;
                    cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = compDate;
                    if (string.IsNullOrEmpty(condcd) == false)
                    {
                        cmd.CommandText += " AND condcd=@CONDCD";
                        cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar).Value = condcd;
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string code = SQLite.ParseString(reader["condcd"]);
                            WorkCondition c = WorkCondition.GetCondition(code);
                            c.CondVal = SQLite.ParseString(reader["condval"]);
                            c.StartDt = SQLite.ParseDate(reader["startdt"]) ?? DateTime.MinValue;
                            c.EndDt = SQLite.ParseDate(reader["enddt"]);

                            retv.Add(c);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("装置製造条件取得エラー", ex);
            }

            return retv.ToArray();

        }
        #endregion

        #region 期間中に投入されていた樹脂一覧を取得 GetResins

        public Resin[] GetResins()
        {
            return GetResins(null, null, false, true);
        }

        /// <summary>
        /// 期間中に装置に投入されていた樹脂一覧を取得
        /// ！！ロットに直接紐づいているものは取得できないので注意！！
        /// </summary>
        /// <param name="fromdt"></param>
        /// <param name="todt"></param>
        /// <returns></returns>
        public Resin[] GetResins(DateTime fromdt, DateTime? todt)
        {
            return GetResins(fromdt, todt, false, false);
        }

        /// <summary>
        /// 期間中に装置に投入されていた樹脂一覧を取得
        /// ！！ロットに直接紐づいているものは取得できないので注意！！
        /// </summary>
        /// <param name="fromdt"></param>
        /// <param name="todt"></param>
        /// <returns></returns>
        public Resin[] GetResins(DateTime? fromdt, DateTime? todt, bool notNascaEndOnly, bool newfg)
        {
            DateTime compDate = DateTime.Now;
            if (todt.HasValue)
            {
                compDate = todt.Value;
            }

            List<Resin> retv = new List<Resin>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                       SELECT macno, mixresultid, startdt, enddt, isnascastart, isnascaend
                        FROM TnMacResin 
                        WHERE 
                          macno     = @MACNO";

                    cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.NVarChar).Value = this.MacNo;

                    if (fromdt.HasValue)
                    {
                        cmd.CommandText += " AND (enddt is null OR enddt >= @STARTDT)";
                        cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = fromdt;
                    }

                    if (todt.HasValue)
                    {
                        cmd.CommandText += " AND startdt <= @ENDDT";
                        cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = compDate;
                    }

                    if (notNascaEndOnly)
                    {
                        cmd.CommandText += " AND isnascaend=0";
                    }

                    if (newfg)
                    {
                        cmd.CommandText += " AND enddt is null ";
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                             string mixresultid = SQLite.ParseString(reader["mixresultid"]);

                            Resin r = Resin.GetResin(mixresultid);
                            r.InputDt = SQLite.ParseDate(reader["startdt"]) ?? DateTime.MinValue;
                            r.RemoveDt = SQLite.ParseDate(reader["enddt"]);
                            r.IsNascaStart = SQLite.ParseBool(reader["isnascastart"]);
                            r.IsNascaEnd = SQLite.ParseBool(reader["isnascaend"]);

                            retv.Add(r);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }

            return retv.ToArray();
        }

        #endregion

        #region 期間中に投入されていたウェハー一覧を取得

        /// <summary>
        /// 装置投入中のWaferを返す
        /// Wafer以外の原材料しかない場合はLength=0の配列を返す
        /// 
        /// ※このメソッドはストッカーの段数無関係で装置セット中の全ウェハーを返す
        /// 
        /// </summary>
        /// <param name="lotStart"></param>
        /// <param name="lotEnd"></param>
        /// <returns></returns>
        public Material[] GetWafers(DateTime lotStart)
        {

            //※このメソッドはストッカーの段数無関係で装置セット中の全ウェハーを返す

            List<Material> retv = new List<Material>();

            Material[] matlist = GetMaterials(lotStart, lotStart);

            foreach (Material mat in matlist)
            {
                if (!mat.IsWafer) continue;
                retv.Add(mat);
            }


            return retv.ToArray();
        }

        public Material[] GetWafers(DateTime lotStart, DateTime lotEnd)
        {
            List<Material> retv = new List<Material>();

            Material[] matlist = GetMaterials(lotStart, lotEnd);

            foreach (Material mat in matlist)
            {
                if (!mat.IsWafer) continue;
                retv.Add(mat);
            }

            return retv.ToArray();
        }

        #endregion

        #region 期間中に投入されていた原材料の一覧を取得 GetMaterials

        /// <summary>
        /// 現在割り付け中の原材料を返す
        /// </summary>
        /// <returns></returns>
        public Material[] GetMaterials()
        {
            return GetMaterials(null, null, true);
        }

        /// <summary>
        /// このメソッドはストッカー無関係に全ての原材料を返す
        /// </summary>
        /// <param name="fromdt"></param>
        /// <param name="todt"></param>
        /// <returns></returns>
        public Material[] GetMaterials(DateTime fromdt, DateTime? todt)
        {
            return GetMaterials(fromdt, todt, false);
        }

        /// <summary>
        /// 期間中に投入されていた原材料の一覧を取得
        /// ストッカーの状況は無視
        /// </summary>
        /// <param name="fromdt"></param>
        /// <param name="todt"></param>
        /// <returns></returns>
        public Material[] GetMaterials(DateTime? fromdt, DateTime? todt, bool newfg)
        {
            DateTime compDate = DateTime.Now;
            if (todt.HasValue)
            {
                compDate = todt.Value;
            }

            List<Material> retv = new List<Material>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT macno, stockerno, materialcd, lotno, startdt, enddt, issampled, ringid
                        FROM TnMacMat
                        WHERE 
                         macno = @MACNO";
                    //, isnascastart, isnascaend
                    cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.NVarChar).Value = this.MacNo;

                    if (fromdt.HasValue)
                    {
                        cmd.CommandText += " AND (enddt is null OR enddt >= @STARTDT)";
                        cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = fromdt;
                    }

                    if (todt.HasValue)
                    {
                        cmd.CommandText += " AND startdt <= @ENDDT";
                        cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = compDate;
                    }

                    if (newfg)
                    {
                        cmd.CommandText += " AND enddt is null ";
                    }
                    //if (notNascaEndOnly)
                    //{
                    //    cmd.CommandText += " AND isnascaend=0";
                    //}


                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            string materialcd = SQLite.ParseString(rd["materialcd"]);
                            string lotno = SQLite.ParseString(rd["lotno"]);

                            Material m = Material.GetMaterial(materialcd, lotno);
                            if (m == null)
                            {
                                throw new ArmsException("原材料ロット情報が存在しません:" + lotno);
                            }
                            m.InputDt = SQLite.ParseDate(rd["startdt"]) ?? DateTime.MinValue;
                            m.RemoveDt = SQLite.ParseDate(rd["enddt"]);
                            m.StockerNo = SQLite.ParseInt(rd["stockerno"]);
                            //m.IsNascaStart = SQLite.ParseBool(rd["isnascastart"]);
                            //m.IsNascaEnd = SQLite.ParseBool(rd["isnascaend"]);
                            m.IsTimeSampled = SQLite.ParseBool(rd["issampled"]);
                            m.RingId = SQLite.ParseString(rd["ringid"]);

                            retv.Add(m);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("投入済み原材料取得エラー", ex);
            }
            return retv.ToArray();
        }

        /// <summary>
        /// ストッカー段数を考慮して実投入された原材料を取得
        /// ロットに直接関連づけられた原材料は取得しない
        /// </summary>
        /// <param name="fromdt"></param>
        /// <param name="todt"></param>
        /// <param name="machine"></param>
        /// <param name="stocker1"></param>
        /// <param name="stocker2"></param>
        /// <returns></returns>
        public Material[] GetMaterials(DateTime fromdt, DateTime? todt, MachineInfo machine, string stocker1, string stocker2)
        {
            //注意！
            //MAP基板など、ロットに直接紐づく原材料は取得しない
            //Order.GetMaterials()を使うこと

            Material[] matlist;

            if (machine.HasDoubleStocker)
            {
                matlist = machine.GetMaterialsFrameLoader(fromdt, todt, stocker1, stocker2);
            }
            else if (machine.HasWaferChanger)
            {
                matlist = machine.GetMaterialsDieBond(fromdt, todt, stocker1, stocker2);
            }
            else
            {
                matlist = machine.GetMaterials(fromdt, todt);
            }

            return matlist;

        }

        #region 投入されたウェハーをカセット情報から取得

        /// <summary>
        /// 期間中に投入されていたウェハー、ダイボンド樹脂の一覧を取得
        /// </summary>
        /// <param name="fromdt"></param>
        /// <param name="todt"></param>
        /// <returns></returns>
        public Material[] GetMaterialsDieBond(DateTime fromdt, DateTime? todt, string stocker1, string stocker2)
        {
            try
            {

                int from = 0;
                if (string.IsNullOrEmpty(stocker1) == false)
                {
                    from = int.Parse(stocker1.Split('-')[1]);
                }

                int to = 0;
                int change = 0;

                if (string.IsNullOrEmpty(stocker2) == false)
                {
                    to = int.Parse(stocker2.Split('-')[1]);
                    change = int.Parse(stocker2.Split('-')[0]);

                }
                List<Material> retv = new List<Material>();

                Material[] matlist = GetMaterials(fromdt, todt);
                if (matlist.Length == 0)
                {
                    return matlist;
                }

                matlist = matlist.OrderBy(m => m.StockerNo).OrderBy(m => m.InputDt).ToArray();

                int prevStocker = 0;
                int currentBox = 0;


                //チェンジャー交換回数より実際の交換回数が少ない場合は、
                //チェンジャー交換回数を回数に合わせる　（メンテ対策）
                foreach (Material m in matlist)
                {
                    if (m.StockerNo == 0) continue;

                    if (m.StockerNo < prevStocker)
                    {
                        //ストッカー番号が若返った場合は箱が変わったと判定
                        currentBox += 1;
                    }
                    prevStocker = m.StockerNo;
                }
                if (currentBox < change) change = currentBox;



                currentBox = 0;
                prevStocker = 1;
                foreach (Material m in matlist)
                {
                    //StockerNo=0は通常原材料
                    if (m.StockerNo == 0)
                    {
                        retv.Add(m);
                        continue;
                    }


                    if (m.StockerNo < prevStocker)
                    {
                        //ストッカー番号が若返った場合は箱が変わったと判定
                        currentBox += 1;
                    }
                    prevStocker = m.StockerNo;

                    //最初のカセットはFROM以上の段数を原材料に採用
                    if (currentBox == 0)
                    {
                        if (m.StockerNo >= from)
                        {
                            if (change == 0)
                            {
                                //カセット交換回数が0の場合はTOより若い段数のみ
                                if (m.StockerNo <= to)
                                {
                                    retv.Add(m);
                                }
                            }
                            else
                            {
                                retv.Add(m);
                            }
                        }
                    }
                    //2個目以降のカセットで交換回数に満たないカセットは全投入
                    else if (currentBox < change)
                    {
                        retv.Add(m);
                    }
                    //交換回数と一致しているカセット（最終カセット）はTO以前の段数を採用
                    else if (currentBox == change)
                    {
                        if (m.StockerNo <= to)
                        {
                            retv.Add(m);
                        }
                    }
                }

                return retv.ToArray();
            }
            catch (Exception ex)
            {
                throw new ArmsException("ウェハー段数取得エラー", ex);
            }

        }
        #endregion

        #region 投入されたフレームをストッカー情報から取得

        /// <summary>
        /// 期間中に投入されていたフレームの一覧を取得
        /// </summary>
        /// <param name="fromdt"></param>
        /// <param name="todt"></param>
        /// <returns></returns>
        public Material[] GetMaterialsFrameLoader(DateTime fromdt, DateTime? todt, string stocker1, string stocker2)
        {
            List<Material> retv = new List<Material>();

            Material[] matlist = GetMaterials(fromdt, todt);

            if (!string.IsNullOrEmpty(stocker1) && stocker1 != "0")
            {
                Material[] org = matlist.Where(m => m.StockerNo == 1).ToArray();

                foreach (Material m in org)
                {
                    retv.Add(m);
                }
            }


            if (!string.IsNullOrEmpty(stocker2) && stocker2 != "0")
            {
                Material[] org = matlist.Where(m => m.StockerNo == 2).ToArray();
                foreach (Material m in org)
                {
                    retv.Add(m);
                }
            }

            //Stocker0番はそのまま返す
            Material[] org0 = matlist.Where(m => m.StockerNo == 0).ToArray();

            foreach (Material m in org0)
            {
                retv.Add(m);
            }

            return retv.ToArray();
        }
        #endregion

        #endregion

        #region 未完了のロット取得 GetLotList
        /// <summary>
        /// 未完了のロット取得
        /// </summary>
        /// <param name="macNo"></param>
        /// <returns></returns>
        public static List<Material> GetLotList(int macNo)
        {
            List<Material> retv = new List<Material>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT stockerno, materialcd, lotno, startdt, issampled, ringid
                        FROM TnMacMat
                        WHERE 
                         macno = @MACNO 
                         AND enddt is null";
                    cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.NVarChar).Value = macNo;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Material m = new Material();
                            m.StockerNo = SQLite.ParseInt(rd["stockerno"]);
                            m.LotNo = SQLite.ParseString(rd["lotno"]);
                            m.MaterialCd = SQLite.ParseString(rd["materialcd"]);
                            m.InputDt = SQLite.ParseDate(rd["startdt"]) ?? DateTime.MinValue;
                            m.IsTimeSampled = SQLite.ParseBool(rd["issampled"]);
                            m.RingId = SQLite.ParseString(rd["ringid"]);

                            retv.Add(m);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("未完了のロット取得エラー", ex);
            }
            return retv;
        }
        #endregion

        #region GetAvailableMachines その工程で投入可能な設備一覧を取得

        /// <summary>
        /// その工程で投入可能な設備一覧を取得する
        /// </summary>
        /// <param name="inlineNo"></param>
        /// <param name="procNo"></param>
        /// <returns></returns>
        public static MachineInfo[] GetAvailableMachines(int procNo, List<string> macGroup)
        {
            List<MachineInfo> retv = new List<MachineInfo>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT 
                          procno , 
                          tmpromac.macno 
                        FROM 
                          tmpromac INNER JOIN
                                  TmMachine ON TmProMac.macno = TmMachine.macno
                        WHERE 
                          procno = @PROCNO AND (TmMachine.delfg = 0)";

                    cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procNo;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int macno = SQLite.ParseInt(reader["macno"]);
                            MachineInfo mac = MachineInfo.GetMachine(macno);
                            if (mac == null)
                            {
                                continue;
                            }
                            if (macGroup.Count == 0)
                            {
                                retv.Add(mac);
                            }
                            else
                            {
                                //同グループの装置のみ追加
                                foreach (string mg in macGroup)
                                {
                                    if (mac.MacGroup.Exists(m => m == mg) && retv.Any(m => m.MacNo == mac.MacNo) == false)
                                    {
                                        retv.Add(mac);
                                    }
                                }
                            }
                        }
                    }
                }
                return retv.ToArray();

            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// その設備、その工程で投入可能な型番リストを取得
        /// </summary>
        /// <param name="plantcd"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        public static List<string> GetAvailableTypes(string plantcd, int? procno)
        {
            List<string> retv = new List<string>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT typecd
									FROM TmWorkMacCond WITH(nolock) 
									WHERE delfg = 0 AND plantcd = @PLANTCD ";

                cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantcd;

                if (procno.HasValue)
                {
                    sql += " AND procno = @PROCNO ";
                    cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno;
                }

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(rd["typecd"].ToString().Trim());
                    }
                }
            }

            return retv;
        }

        /// <summary>
        /// その設備でタイプが投入禁止か否かを判定
        /// </summary>
        /// <param name="plantcd"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        public static bool IsDenyType(string plantcd, string typecd)
        {
            List<string> retv = new List<string>();
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" SELECT typecd
									FROM TmDenyMacType WITH(nolock) 
									WHERE delfg = 0 AND plantcd = @PLANTCD AND typecd = @TYPECD";

                    cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantcd;
                    cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;

                    cmd.CommandText = sql;
                    object obj = cmd.ExecuteScalar();

                    if (obj != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 削除フラグ付きも含め、同一設備CDを持つ装置リストを取得する
        /// </summary>
        /// <returns></returns>
        public static MachineInfo[] GetDupulicateMachines()
        {
            List<MachineInfo> retv = new List<MachineInfo>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                           select macno, plantnm, clasnm, resinchkfg, waferchkfg, plantcd, stockertype, delfg, [lineno] from TmMachine
                            where plantcd in
                            (select plantcd from tmmachine where outline = 0 group by plantcd
                            having count(*) > 1) and outline = 0";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MachineInfo mac = new MachineInfo();

                            mac.MacNo = SQLite.ParseInt(reader["macno"]);
                            mac.MachineName = SQLite.ParseString(reader["plantnm"]);
                            mac.ClassName = SQLite.ParseString(reader["clasnm"]);
                            mac.ResinCheckFg = SQLite.ParseBool(reader["resinchkfg"]);
                            mac.WaferCheckFg = SQLite.ParseBool(reader["waferchkfg"]);
                            mac.NascaPlantCd = SQLite.ParseString(reader["plantcd"]);
                            mac.LineNo = SQLite.ParseString(reader["lineno"]);

                            int stockertype = SQLite.ParseInt(reader["stockertype"]);

                            if (stockertype == STOCKER_TYPE_DOUBLE_FRAMELOADER)
                            {
                                mac.HasDoubleStocker = true;
                            }

                            if (stockertype == STOCKER_TYPE_WAFER_CHANGER)
                            {
                                mac.HasWaferChanger = true;
                            }

                            mac.DelFg = SQLite.ParseBool(reader["delfg"]);

                            // ハイフンの設備番号は無視する
                            if (mac.NascaPlantCd == "-") continue;
                            retv.Add(mac);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("設備マスタ取得エラー", ex);
            }

            return retv.ToArray();
        }

        /// <summary>
        /// EICSの装置タイプ関連を読み込み
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        private static string GetCurrentEICS(MachineInfo mac, string key)
        {
            if (mac == null || string.IsNullOrEmpty(mac.NascaPlantCd) || mac.NascaPlantCd == "-")
            {
                return null;
            }

            try
            {
                if (Config.Settings.QCILXmlFilePath == null) { return null; }

                string retv = null;
                foreach (string qcilConfig in Config.Settings.QCILXmlFilePath)
                {
                    XDocument doc = XDocument.Load(qcilConfig);
                    if (doc == null) continue;

                    IEnumerable<XElement> rootelms = doc.Root.Element("qcil_info").Elements("line_info");
                    foreach (XElement rootelm in rootelms)
                    {
                        var macelmList = rootelm.Element("EquipmentList");
                        var macelms = macelmList.Elements("Equipment");
                        var macelm = macelms.Where(e => e.Attribute("no").Value == mac.NascaPlantCd).FirstOrDefault();
                        if (macelm == null) continue;

                        if (macelm.Attribute(key) != null)
                        {
                            retv = macelm.Attribute(key).Value;
                        }
                    }
                }
                return retv;
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("QCIL装置設定読込異常:" + mac.NascaPlantCd + ":" + ex.ToString());
            }

            return null;
        }

        //2015.11.4改修。タイプ取得の場合はファイルからDB参照に変更。
        //typeCheckModeがfalseならタイプ判定の場合のみ除外。
        public static string GetCurrentEICSTypeCd(MachineInfo mac, bool typeCheckMode)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.Parameters.Add("@DELFG", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = mac.NascaPlantCd;
                    cmd.CommandText = @"
                            SELECT WorkingType_CD 
                            FROM TmLSET with (NOLOCK)
                            WHERE Equipment_NO = @PLANTCD ";

                    if (typeCheckMode == true)
                    {
                        cmd.CommandText += @"AND ArmsTypeNoCheck_FG = 'false' ";
                    }

                    object typeCd = cmd.ExecuteScalar();
                    if (typeCd == null)
                    {
                        return null;
                    }
                    else
                    {
                        return typeCd.ToString().ToUpper().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("QCIL装置設定読込異常:" + mac.NascaPlantCd + ":" + ex.ToString());
            }
            return null;
        }

        public static string GetCurrentEICSTypeCd(MachineInfo mac)
        {
            return GetCurrentEICSTypeCd(mac, false);
        }


        public static bool? GetCurrentEICSBadMarkCountFg(MachineInfo mac)
        {
            string isbadmarkframe = GetCurrentEICS(mac, "BMCountFG");
            if (string.IsNullOrEmpty(isbadmarkframe))
            {
                return null;
            }
            return bool.Parse(isbadmarkframe);
        }

        public static int GetEICSLineNo(string plantCd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@DELFG", SqlDbType.Bit).Value = false;
                cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantCd;
                cmd.CommandText = @"
                        SELECT Inline_CD FROM TmLSET WHERE Equipment_NO = @PLANTCD ";

                object lineNo = cmd.ExecuteScalar();
                if (lineNo == null)
                {
                    Log.SysLog.Info(string.Format("EICSラインNO取得エラー 設備CD:{0}", plantCd));
                    return 99999;
                    //throw new ArmsException(string.Format("EICSラインNO取得エラー 設備CD:{0} 接続先:{1}", plantCd, Config.Settings.QCILConSTR));
                }
                else
                {
                    return Convert.ToInt32(lineNo);
                }
            }
        }

        public static string GetEICSModelNm(string plantCd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantCd;

                cmd.CommandText = @" SELECT Model_NM FROM TmEQUI WITH(nolock) WHERE Del_FG = 0 AND Equipment_NO = @PLANTCD ";

                object modelNM = cmd.ExecuteScalar();
                if (modelNM == null)
                {
                    return null;
                }
                else
                {
                    return modelNM.ToString().Trim();
                }
            }
        }

        /// <summary>
        /// 装置の削除フラグを更新する
        /// </summary>
        /// <param name="mac"></param>
        public static void UpdateDelFg(MachineInfo mac)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = SQLite.SerializeBool(mac.DelFg);
                    cmd.Parameters.Add("@MACNO", SqlDbType.Int).Value = mac.MacNo;
                    cmd.CommandText = @"
                           update TmMachine SET delfg = @DELFG WHERE macno=@MACNO";

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("設備マスタ更新エラー", ex);
            }
        }

        ///// <summary>
        ///// 高効率ライン　仮想マガジン作成指示
        ///// </summary>
        ///// <returns></returns>
        //public string GetMelPDAWorkStartDir()
        //{
        //	return System.IO.Path.Combine(Config.Settings.MelWorkStartBasePath, MacNo.ToString());
        //}

        ///// <summary>
        ///// 高効率ライン　仮想マガジン削除指示
        ///// </summary>
        ///// <returns></returns>
        //public string GetMelPDAWorkCompDir()
        //{
        //	return System.IO.Path.Combine(Config.Settings.MelWorkCompltBasePath, MacNo.ToString());
        //}

        /// <summary>
        /// 通信用のPLCアドレス設定
        /// </summary>
        public Dictionary<string, string> GetNetworkSettings()
        {
            return null;
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return LongName;
        }

        #region GetStockerNo

        /// <summary>
        /// QRコード内容からストッカー番号を取得
        /// QRコードはヘッダー込み
        /// </summary>
        /// <param name="stockerCd"></param>
        /// <returns></returns>
        public static int? GetStockerNo(string stockerCd)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@QRCD", SqlDbType.NVarChar).Value = (object)stockerCd ?? DBNull.Value;

                cmd.CommandText = "SELECT stockerno FROM TmMachineStocker WHERE qrcode=@QRCD";
                object val = cmd.ExecuteScalar();

                if (val == null)
                {
                    return null;
                }

                return Convert.ToInt32(val);
            }
        }
        #endregion

        public static bool IsCutMachine(string clasnm)
        {
            if (clasnm.Contains("ｶｯﾄ") || clasnm.Contains("カット")
                || clasnm.Contains("ﾌﾞﾚｲｸ") || clasnm.Contains("ブレイク")
                || clasnm.Contains("実体顕微鏡"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //20220615 ADD START
        //TmProcessを検索し、finalst=1の場合、trueを返す
        public static bool IsCutMachine(int procno)
        {
            int cutblendfg = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"SELECT cutblendfg
                                          FROM tmprocess
                                         WHERE procno = @PROCNO
                                       ";
                    cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = procno;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cutblendfg = SQLite.ParseInt(reader["cutblendfg"]);
                        }
                    }
                }
                if (cutblendfg == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }
        //20220615 ADD END

        //20220627 ADD START
        public static bool IsFirstSt(string plantcd)
        {

            int firstst = 0;
            
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"SELECT ISNULL(p.firstst, 0) AS firstst
                                          FROM TmProcess p INNER JOIN TmProMac m ON
                                               p.procno  = m.procno INNER JOIN TmMachine t ON
                                               m.macno   = t.macno
                                         WHERE t.plantcd = @PLANTCD
                                           AND p.delfg   = 0
                                       ";
                    cmd.Parameters.Add("@PLANTCD", SqlDbType.NChar).Value = plantcd;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            firstst = SQLite.ParseInt(reader["firstst"]);
                        }
                    }
                }
                if (firstst == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procNo"></param>
        /// <returns></returns>
        public static bool IsLotNoChkProc(string plantcd)
        {
            int lotnochkfg = 0;

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"SELECT ISNULL(p.lotnochkfg, 0) AS lotnochkfg
                                          FROM TmProcess p INNER JOIN TmProMac m ON
                                               p.procno  = m.procno INNER JOIN TmMachine t ON
                                               m.macno   = t.macno
                                         WHERE t.plantcd = @PLANTCD
                                           AND p.delfg   = 0
                                       ";
                    cmd.Parameters.Add("@PLANTCD", SqlDbType.NChar).Value = plantcd;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lotnochkfg = SQLite.ParseInt(reader["lotnochkfg"]);
                        }
                    }
                    if (lotnochkfg == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }
        //20220627 ADD END

        public static bool IsWireBondMachine(string clasnm)
        {
            if (clasnm.Contains("ワイヤーボンダー"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //2015.11.4追加  EICSの設定可能タイプを変更するメソッド
        public static List<string> GetEICSThrowTypeList(string plantModel, string searchCond)
        {

            System.Data.Common.DbDataReader rd = null;
            List<string> typeCDList = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@ModelNM", SqlDbType.NVarChar).Value = plantModel;
                cmd.CommandText = @" SELECT Material_CD
                                FROM TmPLM WITH (nolock) 
                                WHERE Del_FG = 0 AND Model_NM = @ModelNM ";

                if (string.IsNullOrWhiteSpace(searchCond) == false)
                {
                    cmd.CommandText += @"AND Material_CD LIKE @SEARCHCOND ";
                    cmd.Parameters.Add("@SEARCHCOND", SqlDbType.NVarChar).Value = searchCond;
                }

                cmd.CommandText += @"GROUP BY Material_CD 
                                ORDER BY Material_CD ";

                using (rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string typeCD = Convert.ToString(rd["Material_CD"]).Trim();
                        typeCDList.Add(typeCD);
                    }
                }
            }

            return typeCDList;
        }

        //2015.11.4追加  EICSのタイプを変更するメソッド
        public static bool UpdateEICSType(string plantCd, string newType)
        {
            bool retv = false;

            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.Parameters.Add("@NEWTYPE", SqlDbType.NVarChar).Value = newType;
                    cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantCd;
                    cmd.Parameters.Add("@LASTUPDATE", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.CommandText = @"
                           update TmLSET SET WorkingType_CD = @NEWTYPE WHERE Equipment_NO=@PLANTCD";

                    cmd.ExecuteNonQuery();
                }
                retv = true;
            }
            catch (Exception ex)
            {
                throw new ArmsException("設備マスタ更新エラー", ex);
            }
            return retv;
        }

        /// <summary>
        /// 工程取得
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static int GetProcNo(int macno)
        {
            try
            {
                List<int> procNoList = GetProcNoList(macno);
                if (procNoList.Count == 1)
                {
                    return Convert.ToInt32(procNoList[0]);
                }
                else if (procNoList.Count == 0)
                {
                    throw new ApplicationException(string.Format("対象工程が取得できません。設備NO：{0}", macno));
                }
                else
                {
                    throw new ApplicationException(string.Format("設備に複数の工程が紐づいています。設備NO：{0}", macno));
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 工程取得
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static List<int> GetProcNoList(int macno)
        {
            List<int> retv = new List<int>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT 
                          procno                           
                        FROM 
                          tmpromac 
                        WHERE 
                          macno = @MACNO";

                    cmd.Parameters.Add("@MACNO", SqlDbType.NVarChar).Value = macno;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string procno = SQLite.ParseString(reader["procno"]);
                            int i;
                            if (int.TryParse(procno, out i) == true)
                            {
                                retv.Add(i);
                            }
                        }
                    }
                }

                return retv;

            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        public static void UpdateMoveStartMagazineFlag(int macno, bool statusfg)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.Parameters.Add("@STATUSFG", SqlDbType.NVarChar).Value = Convert.ToInt16(statusfg);
                    cmd.Parameters.Add("@MACNO", SqlDbType.NVarChar).Value = macno;
                    cmd.CommandText = @" UPDATE TmMachine SET requestmovestartmagfg = @STATUSFG WHERE macno=@MACNO ";

                    cmd.ExecuteNonQuery();

                    Log.SysLog.Info(string.Format("[開始通知] macno:{1} {0}へ変更", statusfg, macno));
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException("設備マスタ更新エラー", ex);
            }
        }

        /// <summary>
        /// 搬送ロボット、AGVなどマガジン輸送中に使用される設備を取得
        /// </summary>
        /// <param name="macNo"></param>
        /// <returns></returns>
        public static List<int> GetInTransitMachineList()
        {
            List<int> retv = new List<int>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT macno FROM TmAGV with(nolock) ";
                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(Convert.ToInt32(rd["macno"]));
                    }
                }

                sql = @" SELECT macno FROM TmMachine with(nolock) WHERE delfg = 0 AND clasnm = 'ロボットQR読取'";
                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(Convert.ToInt32(rd["macno"]));
                    }
                }
            }

            return retv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isMonitor">監視中</param>
        /// <returns></returns>
        public static List<Operator> GetOperators(bool isMonitor, string showKey)
        {
            List<Operator> retv = new List<Operator>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @" SELECT operationmacgroupcd, operationempcd, warningminutetm, monitorfg, TnMachineOperator.lastupddt, showkey, empname
                                     FROM TnMachineOperator with(nolock) 
                                     INNER JOIN TmEmployee ON TnMachineOperator.operationempcd = TmEmployee.empcode
                                     WHERE 1=1 ";

                if (isMonitor)
                {
                    cmd.Parameters.Add("@MONITORFG", SqlDbType.Bit).Value = isMonitor;
                    cmd.CommandText += " AND TnMachineOperator.monitorfg = @MONITORFG ";
                }

                if (string.IsNullOrEmpty(showKey) == false)
                {
                    cmd.Parameters.Add("@SHOWKEY", SqlDbType.NVarChar).Value = showKey;
                    cmd.CommandText += " AND TnMachineOperator.showkey = @SHOWKEY ";
                }

                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    Operator o = new Operator();
                    o.OperationMacgroupCd = rd["operationmacgroupcd"].ToString().Trim();
                    o.OperationEmpCd = Convert.ToInt32(rd["operationempcd"]);
                    o.OperationEmpNm = rd["empname"].ToString().Trim();
                    o.WarningMinuteTm = Convert.ToInt32(rd["warningminutetm"]);
                    o.MonitorFg = Convert.ToBoolean(rd["monitorfg"]);
                    o.LastUpdDt = Convert.ToDateTime(rd["lastupddt"]);
                    o.ShowKey = rd["showkey"].ToString().Trim();
                    retv.Add(o);
                }
            }

            return retv;
        }

        public static Operator GetOperator(string operationMacGroupCd, bool isMonitor, string showKey)
        {
            List<Operator> list = GetOperators(isMonitor, showKey);

            IEnumerable<Operator> retv = list.Where(l => l.OperationMacgroupCd == operationMacGroupCd);

            if (retv.Count() == 0)
            {
                return null;
            }
            else
            {
                return retv.Single();
            }
        }

        public static int GetOperator(string plantCd)
        {
            //TODO 登録が無い場合どうする??
            int retv = 660;

            MachineInfo m = MachineInfo.GetMachine(plantCd);
            Operator o = GetOperator(m.OperationMacGroupCd, true, Config.Settings.OperationMachineGroupShowKey);
            if (o != null)
            {
                retv = o.OperationEmpCd;
            }
            
            return retv;
        }

        public static bool HasOverWorkOperator(string showKey)
        {
            List<Operator> operatorList = GetOperators(true, showKey);
            foreach(Operator ope in operatorList)
            {
                DateTime limitDt = ope.LastUpdDt.AddMinutes(ope.WarningMinuteTm);
                if (System.DateTime.Now >= limitDt)
                {
                    return true;
                }
            }
            return false;
        }

        public static void RemoveFromOperationMacGroup(string plantCd)
        {
            if (string.IsNullOrEmpty(plantCd))
            {
                throw new ApplicationException("RemoveFromOperationMacGroup関数で引数plantCdが空で引き渡されました。");
            }

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @" UPDATE TmMachine SET operationmacgroupcd = null WHERE plantcd = @PLANTCD ";

                cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantCd;

                cmd.ExecuteNonQuery();
            }
        }

        public static void AddToOperationMacGroup(string plantCd, string operationMacGroupCd)
        {
            if (string.IsNullOrEmpty(plantCd))
            {
                throw new ApplicationException("AddToOperationMacGroup関数で引数plantCdが空で引き渡されました。");
            }

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @" UPDATE TmMachine SET operationmacgroupcd = @OPERATIONMACGROUPCD WHERE plantcd = @PLANTCD ";

                cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantCd;
                cmd.Parameters.Add("@OPERATIONMACGROUPCD", SqlDbType.NVarChar).Value = operationMacGroupCd;

                cmd.ExecuteNonQuery();
            }
        }

        public static void EditMachineOperatorSetting(bool isMonitor, string warningMinuteTm, string operationMacGroupCd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @" UPDATE TnMachineOperator SET warningminutetm = @WARNINGMINUTETM, monitorfg = @MONITORFG 
                                        WHERE operationmacgroupcd = @OPERATIONMACGROUPCD ";

                cmd.Parameters.Add("@MONITORFG", SqlDbType.Bit).Value = isMonitor;
                cmd.Parameters.Add("@WARNINGMINUTETM", SqlDbType.Int).Value = warningMinuteTm;
                cmd.Parameters.Add("@OPERATIONMACGROUPCD", SqlDbType.NVarChar).Value = operationMacGroupCd;

                cmd.ExecuteNonQuery();
            }
        }

        public static void EditMachineOperator(string operationMacGroupCd, int empCd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @" UPDATE TnMachineOperator SET operationempcd = @OPERATIONEMPCD, lastupddt = @LASTUPDDT
                                        WHERE operationmacgroupcd = @OPERATIONMACGROUPCD ";

                cmd.Parameters.Add("@OPERATIONEMPCD", SqlDbType.NVarChar).Value = empCd;
                cmd.Parameters.Add("@OPERATIONMACGROUPCD", SqlDbType.NVarChar).Value = operationMacGroupCd;
                cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = System.DateTime.Now;

                cmd.ExecuteNonQuery();
            }
        }

        public class Eics
        {
            public string LineName { get; set; }

            public string TypeGroupCode { get; set; }

            public string TypeCode { get; set; }

            public string ChipName { get; set; }

            public string ModelName { get; set; }

            public string MachineLogPath { get; set; }

            public string  IPAddressNo { get; set; }

            public static Eics GetSetting(string equipmentNo)
            {
                Eics retv = null;

                using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" SELECT WorkingTypeGroup_CD, Chip_NM, WorkingType_CD, Model_NM, Inline_NM, InputFolder_NM, IPAddress_NO
									FROM TmEQUI WITH(nolock)
                                        INNER JOIN TmLSET WITH(nolock) ON TmEQUI.Equipment_NO = TmLSET.Equipment_NO
                                        INNER JOIN TmLINE WITH(nolock) ON TmLSET.Inline_CD = TmLINE.Inline_CD
									WHERE TmEQUI.Del_FG = 0 AND TmLSET.Del_FG = 0 AND TmLSET.Equipment_NO = @Equipment_NO ";

                    cmd.Parameters.Add("@Equipment_NO", SqlDbType.NVarChar).Value = equipmentNo;

                    cmd.CommandText = sql;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            retv = new Eics();
                            retv.LineName = rd["Inline_NM"].ToString().Trim();
                            retv.TypeCode = rd["WorkingType_CD"].ToString().Trim();
                            retv.TypeGroupCode = rd["WorkingTypeGroup_CD"].ToString().Trim();
                            retv.ChipName = rd["Chip_NM"].ToString().Trim();
                            retv.ModelName = rd["Model_NM"].ToString().Trim();
                            retv.MachineLogPath = rd["InputFolder_NM"].ToString().Trim();
                            retv.IPAddressNo = rd["IPAddress_NO"].ToString().Trim();
                        }
                    }
                }

                return retv;
            }
        }

        public class Operator
        {
            public string OperationMacgroupCd { get; set; }

            public int OperationEmpCd { get; set; }

            public string OperationEmpNm { get; set; }

            public int WarningMinuteTm { get; set; }

            public bool MonitorFg { get; set; }

            public DateTime LastUpdDt { get; set; }

            public string ShowKey { get; set; }
        }


    }

    public class MachineInfoGroup
    {
        /// <summary> 装置種類 </summary>
        public string ClassNm { get; set; }
        /// <summary> ラインNo </summary>
        public string LineNo { get; set; }
        /// <summary> 装置リスト </summary>
        public List<MachineInfo> MacList { get; set; }

        public MachineInfoGroup(string classnm, string lineno)
        {
            this.ClassNm = classnm;
            this.LineNo = lineno;
        }

        public static List<MachineInfoGroup> GetMachineInfoGroupList(List<KeyValuePair<string, string>> kvList)
        {
            List<MachineInfoGroup> retv = new List<MachineInfoGroup>(); 

            foreach (KeyValuePair<string, string> kv in kvList)
            {
                MachineInfoGroup macInfoGroup = new MachineInfoGroup(kv.Value, kv.Key);
                macInfoGroup.MacList = MachineInfo.SearchMachine(null, null, true, false, false, null, kv.Value, kv.Key).OrderBy(m => m.MacNo % 1000).ToList();
                retv.Add(macInfoGroup);
            }

            return retv;
        }
    }
}
