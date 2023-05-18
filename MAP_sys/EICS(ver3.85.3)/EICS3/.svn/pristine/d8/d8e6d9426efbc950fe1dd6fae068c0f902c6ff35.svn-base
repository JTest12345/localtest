using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;

namespace EICS.Arms
{
	class Machine
	{
		public const int STOCKER_TYPE_DOUBLE_FRAMELOADER = 1;
		public const int STOCKER_TYPE_WAFER_CHANGER = 2;

		#region プロパティ

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

		public bool DelFg { get; set; }

		#endregion


		#region インライン設備マスタ情報取得

		/// <summary>
		/// アウトライン含めて全装置を返す
		/// </summary>
		/// <returns></returns>
		//public static Machine[] GetMachineList()
		//{
		//    return SearchMachine(null, null, false, false);
		//}

		/// <summary>
		/// インライン設備マスタ情報取得
		/// </summary>
		/// <param name="schParam">検索条件</param>
		/// <returns>インライン設備マスタ構造体</returns>
		public static Machine GetMachine(int lineCD, int inlineMachineNo)
		{
			return GetMachine(lineCD, inlineMachineNo, false);
		}

		public static Machine GetMachine(int lineCD, int inlineMachineNo, bool includeDelFg)
		{
			Machine[] maclist = SearchMachine(lineCD, inlineMachineNo, null, false, false, includeDelFg);

			foreach (Machine m in maclist)
			{
				if (m.MacNo == inlineMachineNo)
				{
					return m;
				}
			}

			return null;
		}
        public static Machine GetMachine(int lineCD, string plantCD) 
        {
            Machine[] machines = SearchMachine(lineCD, null, plantCD, false, false, false);
            if (machines.Count() == 0) 
            {
                throw new ApplicationException(string.Format("対象設備が存在しません。 設備番号:{0}", plantCD));
            }
            return machines[0];
        }

		//public static Machine GetMachine(string nascaPlantCd)
		//{
		//    Machine[] maclist = SearchMachine(null, nascaPlantCd, false, false);

		//    foreach (Machine m in maclist)
		//    {
		//        if (m.NascaPlantCd == nascaPlantCd)
		//        {
		//            return m;
		//        }
		//    }

		//    log.Error("装置が見つかりません" + nascaPlantCd);
		//    return null;
		//}

        public static Machine[] SearchMachine(int lineCD, int? macno, string plantcd, bool onlyInline, bool onlyOutline, bool includeDelFg)
        {
			return SearchMachine(macno, plantcd, onlyInline, onlyOutline, includeDelFg, ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD));
        }

        public static Machine[] SearchMachine(int? macno, string plantcd, bool onlyInline, bool onlyOutline, bool includeDelFg, string constr)
        {
            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            List<Machine> retv = new List<Machine>();

            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        select
                        macno
                        , plantcd
                        , plantnm
                        , clasnm
                        , resinchkfg
                        , waferchkfg
                        , stockertype
                        , outline
                        FROM TmMachine
                        WHERE 1=1";

                    if (includeDelFg == false)
                    {
                        cmd.CommandText += "  AND delfg=0";
                    }

                    if (macno.HasValue == true)
                    {
                        cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno.Value;
                        cmd.CommandText += " AND macno=@MACNO";
                    }

                    if (string.IsNullOrEmpty(plantcd) == false)
                    {
                        cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantcd;
                        cmd.CommandText += " AND plantcd=@PLANTCD";
                    }

                    if (onlyInline)
                    {
                        cmd.CommandText += " AND outline=0";
                    }

                    if (onlyOutline)
                    {
                        cmd.CommandText += " AND outline=1";
                    }


                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Machine mac = new Machine();

                            mac.MacNo = SQLite.ParseInt(reader["macno"]);
                            mac.MachineName = SQLite.ParseString(reader["plantnm"]);
                            mac.ClassName = SQLite.ParseString(reader["clasnm"]);
                            mac.ResinCheckFg = SQLite.ParseBool(reader["resinchkfg"]);
                            mac.WaferCheckFg = SQLite.ParseBool(reader["waferchkfg"]);
                            mac.NascaPlantCd = SQLite.ParseString(reader["plantcd"]);
                            mac.IsOutLine = SQLite.ParseBool(reader["outline"]);

                            int stockertype = SQLite.ParseInt(reader["stockertype"]);

                            if (stockertype == STOCKER_TYPE_DOUBLE_FRAMELOADER)
                            {
                                mac.HasDoubleStocker = true;
                            }

                            if (stockertype == STOCKER_TYPE_WAFER_CHANGER)
                            {
                                mac.HasWaferChanger = true;
                            }

                            retv.Add(mac);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("設備マスタ取得エラー", ex);
            }

            return retv.ToArray();
        }


		//public static Machine[] SearchMachine(int? macno, string plantcd, bool onlyInline, bool onlyOutline)
		//{
		//    return SearchMachine(macno, plantcd, onlyInline, onlyOutline, false);
		//}

		#endregion


		#region 期間中に割り付いていた製造条件特性リスト取得 GetWorkConditions
		//public WorkCondition[] GetWorkConditions(DateTime startdt, DateTime? enddt)
		//{
		//    return GetWorkConditions(null, startdt, enddt);
		//}

		/// <summary>
		/// 指定期間に割り付いていた特性リスト取得
		/// </summary>
		/// <param name="macno"></param>
		/// <param name="condcd"></param>
		/// <param name="startdt"></param>
		/// <param name="enddt"></param>
		/// <returns></returns>
//        public WorkCondition[] GetWorkConditions(string condcd, DateTime startdt, DateTime? enddt)
//        {
//            DateTime compDate = DateTime.Now;
//            if (enddt.HasValue)
//            {
//                compDate = enddt.Value;
//            }

//            List<WorkCondition> retv = new List<WorkCondition>();

//            try
//            {
//                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//                using (SqlCommand cmd = con.CreateCommand())
//                {
//                    con.Open();

//                    cmd.CommandText = @"
//                        SELECT
//                        macno
//                        , condcd
//                        , condval
//                        , startdt
//                        , enddt
//                        FROM TnMacCond
//                        WHERE macno=@MACNO
//                        AND startdt<=@ENDDT AND (enddt IS NULL OR enddt >= @STARTDT)";

//                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.MacNo;
//                    cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = startdt;
//                    cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = compDate;
//                    if (string.IsNullOrEmpty(condcd) == false)
//                    {
//                        cmd.CommandText += " AND condcd=@CONDCD";
//                        cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar).Value = condcd;
//                    }

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            string code = SQLite.ParseString(reader["condcd"]);
//                            WorkCondition c = WorkCondition.GetCondition(code);
//                            c.CondVal = SQLite.ParseString(reader["condval"]);
//                            c.StartDt = SQLite.ParseDate(reader["startdt"]) ?? DateTime.MinValue;
//                            c.EndDt = SQLite.ParseDate(reader["enddt"]);

//                            retv.Add(c);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new ArmsException("装置製造条件取得エラー", ex);
//            }

//            return retv.ToArray();
//        }
		#endregion




		#region 設備投入資材の更新関連 DeleteInsertMacMat
//        public void DeleteInsertMacMat(Material mat)
//        {
//            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//            using (SqlCommand cmd = con.CreateCommand())
//            {

//                try
//                {
//                    con.Open();

//                    cmd.Transaction = con.BeginTransaction();

//                    cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.NVarChar).Value = this.MacNo;
//                    cmd.Parameters.Add("@MATCD", System.Data.SqlDbType.NVarChar).Value = mat.MaterialCd ?? "";
//                    cmd.Parameters.Add("@STOCKERNO", SqlDbType.BigInt).Value = mat.StockerNo;
//                    cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.NVarChar).Value = mat.LotNo ?? "";
//                    cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = (object)mat.InputDt ?? DateTime.Now;
//                    cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = (object)mat.RemoveDt ?? DBNull.Value;
//                    cmd.Parameters.Add("@ISSAMPLED", SqlDbType.Int).Value = SQLite.SerializeBool(mat.IsTimeSampled);
//                    cmd.Parameters.Add("@ISNASCASTART", SqlDbType.Int).Value = SQLite.SerializeBool(mat.IsNascaStart);
//                    cmd.Parameters.Add("@ISNASCAEND", SqlDbType.Int).Value = SQLite.SerializeBool(mat.IsNascaEnd);

//                    cmd.CommandText = @"
//                        DELETE FROM TnMacMat 
//                        WHERE macno=@MACNO AND materialcd=@MATCD AND lotno=@LOTNO
//                        AND stockerno=@STOCKERNO AND startdt=@STARTDT";
//                    cmd.ExecuteNonQuery();

//                    cmd.CommandText = @"
//                        INSERT INTO TnMacMat(macno, stockerno, materialcd, lotno, startdt, enddt, issampled, isnascastart, isnascaend)
//                        VALUES (@MACNO, @STOCKERNO, @MATCD, @LOTNO, @STARTDT, @ENDDT, @ISSAMPLED, @ISNASCASTART, @ISNASCAEND)";

//                    cmd.ExecuteNonQuery();
//                    cmd.Transaction.Commit();
//                }
//                catch (Exception ex)
//                {
//                    cmd.Transaction.Rollback();
//                    throw new ArmsException("投入資材更新エラー", ex);
//                }
//            }
//        }
		#endregion


		#region 設備投入樹脂の更新関連 DeleteInsertMacResin
//        public void DeleteInsertMacResin(Resin resin)
//        {
//            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//            using (SqlCommand cmd = con.CreateCommand())
//            {

//                try
//                {
//                    con.Open();

//                    cmd.Transaction = con.BeginTransaction();

//                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.MacNo;
//                    cmd.Parameters.Add("@MIXRESID", SqlDbType.BigInt).Value = resin.MixResultId;
//                    cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = (object)resin.InputDt ?? DBNull.Value;
//                    cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = (object)resin.RemoveDt ?? DBNull.Value;
//                    cmd.Parameters.Add("@ISNASCASTART", SqlDbType.Int).Value = SQLite.SerializeBool(resin.IsNascaStart);
//                    cmd.Parameters.Add("@ISNASCAEND", SqlDbType.Int).Value = SQLite.SerializeBool(resin.IsNascaEnd);

//                    cmd.CommandText = @"
//                        DELETE FROM TnMacResin WHERE macno=@MACNO AND mixresultid=@MIXRESID AND startdt=@STARTDT";
//                    cmd.ExecuteNonQuery();

//                    cmd.CommandText = @"
//                        INSERT INTO TnMacResin(macno, mixresultid, startdt, enddt, isnascastart, isnascaend)
//                        VALUES (@MACNO, @MIXRESID, @STARTDT, @ENDDT, @ISNASCASTART, @ISNASCAEND)";

//                    cmd.ExecuteNonQuery();
//                    cmd.Transaction.Commit();
//                }
//                catch (Exception ex)
//                {
//                    cmd.Transaction.Rollback();
//                    throw new ArmsException("投入資材更新エラー", ex);
//                }
//            }
//        }
		#endregion


		#region 製造条件の更新関連 DeleteInsertWorkCond

		/// <summary>
		/// 新規製造条件割り付け
		/// </summary>
		/// <param name="cnd"></param>
//        public void DeleteInsertWorkCond(WorkCondition cnd)
//        {
//            log.Info("製造条件割り付け" + this.MacNo + ":" + cnd.CondCd + ":" + cnd.CondVal);
//            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//            using (SqlCommand cmd = con.CreateCommand())
//            {

//                try
//                {
//                    con.Open();

//                    cmd.Transaction = con.BeginTransaction();
//                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.MacNo;
//                    cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar).Value = cnd.CondCd ?? "";
//                    cmd.Parameters.Add("@CONDVAL", SqlDbType.NVarChar).Value = cnd.CondVal ?? "";
//                    cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = cnd.StartDt;
//                    cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = (object)cnd.EndDt ?? DBNull.Value;

//                    cmd.CommandText = @"
//                        DELETE FROM TnMacCond WHERE macno=@MACNO AND condcd=@CONDCD AND startdt=@STARTDT";
//                    cmd.ExecuteNonQuery();

//                    cmd.CommandText = @"
//                        INSERT INTO TnMacCond(macno, condcd, condval, startdt, enddt)
//                        VALUES (@MACNO, @CONDCD, @CONDVAL, @STARTDT, @ENDDT)";

//                    cmd.ExecuteNonQuery();
//                    cmd.Transaction.Commit();
//                }
//                catch (Exception ex)
//                {
//                    cmd.Transaction.Rollback();
//                    throw new ArmsException("製造条件割り付け更新エラー", ex);
//                }
//            }
//        }
		#endregion

		#region 期間中に投入されていた樹脂一覧を取得 GetResins

		/// <summary>
		/// 期間中に装置に投入されていた樹脂一覧を取得
		/// ！！ロットに直接紐づいているものは取得できないので注意！！
		/// </summary>
		/// <param name="fromdt"></param>
		/// <param name="todt"></param>
		/// <returns></returns>
		//public Resin[] GetResins(DateTime fromdt, DateTime? todt)
		//{

		//    return GetResins(fromdt, todt, false);

		//}

		/// <summary>
		/// 期間中に装置に投入されていた樹脂一覧を取得
		/// ！！ロットに直接紐づいているものは取得できないので注意！！
		/// </summary>
		/// <param name="fromdt"></param>
		/// <param name="todt"></param>
		/// <returns></returns>
//        public Resin[] GetResins(DateTime? fromdt, DateTime? todt, bool notNascaEndOnly)
//        {
//            DateTime compDate = DateTime.Now;
//            if (todt.HasValue)
//            {
//                compDate = todt.Value;
//            }

//            List<Resin> retv = new List<Resin>();

//            try
//            {
//                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//                using (SqlCommand cmd = con.CreateCommand())
//                {
//                    con.Open();

//                    cmd.CommandText = @"
//                        SELECT macno, mixresultid, startdt, enddt, isnascastart, isnascaend
//                        FROM TnMacResin 
//                        WHERE 
//                          macno     = @MACNO";

//                    cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.NVarChar).Value = this.MacNo;

//                    if (fromdt.HasValue)
//                    {
//                        cmd.CommandText += " AND (enddt is null OR enddt >= @STARTDT)";
//                        cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = fromdt;
//                    }

//                    if (todt.HasValue)
//                    {
//                        cmd.CommandText += " AND startdt <= @ENDDT";
//                        cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = compDate;
//                    }

//                    if (notNascaEndOnly)
//                    {
//                        cmd.CommandText += " AND isnascaend=0";
//                    }


//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            int mixresultid = SQLite.ParseInt(reader["mixresultid"]);

//                            Resin r = Resin.GetResin(mixresultid);
//                            r.InputDt = SQLite.ParseDate(reader["startdt"]) ?? DateTime.MinValue;
//                            r.RemoveDt = SQLite.ParseDate(reader["enddt"]);
//                            r.IsNascaStart = SQLite.ParseBool(reader["isnascastart"]);
//                            r.IsNascaEnd = SQLite.ParseBool(reader["isnascaend"]);


//                            retv.Add(r);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LogManager.GetCurrentClassLogger().Error(ex);
//                throw ex;
//            }

//            return retv.ToArray();
//        }

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
		//public Material[] GetWafers(DateTime lotStart)
		//{

		//    //※このメソッドはストッカーの段数無関係で装置セット中の全ウェハーを返す

		//    List<Material> retv = new List<Material>();

		//    Material[] matlist = GetMaterials(lotStart, lotStart);

		//    foreach (Material mat in matlist)
		//    {
		//        if (!mat.IsWafer) continue;
		//        retv.Add(mat);
		//    }


		//    return retv.ToArray();
		//}
		#endregion

		#region 期間中に投入されていた原材料の一覧を取得 GetMaterials

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
		//public Material[] GetMaterials(DateTime fromdt, DateTime? todt, Machine machine, string stocker1, string stocker2)
		//{
		//    //注意！
		//    //MAP基板など、ロットに直接紐づく原材料は取得しない
		//    //Order.GetMaterials()を使うこと

		//    Material[] matlist;

		//    if (machine.HasDoubleStocker)
		//    {
		//        matlist = machine.GetMaterialsFrameLoader(fromdt, todt, stocker1, stocker2);
		//    }
		//    else if (machine.HasWaferChanger)
		//    {
		//        matlist = machine.GetMaterialsDieBond(fromdt, todt, stocker1, stocker2);
		//    }
		//    else
		//    {
		//        matlist = machine.GetMaterials(fromdt, todt);
		//    }

		//    return matlist;

		//}

		/// <summary>
		/// このメソッドはストッカー無関係に全ての原材料を返す
		/// </summary>
		/// <param name="fromdt"></param>
		/// <param name="todt"></param>
		/// <returns></returns>
		//public Material[] GetMaterials(DateTime fromdt, DateTime? todt)
		//{
		//    return GetMaterials(fromdt, todt, false);
		//}

		/// <summary>
		/// 期間中に投入されていた原材料の一覧を取得
		/// ストッカーの状況は無視
		/// </summary>
		/// <param name="fromdt"></param>
		/// <param name="todt"></param>
		/// <returns></returns>
//        public Material[] GetMaterials(DateTime? fromdt, DateTime? todt, bool notNascaEndOnly)
//        {
//            DateTime compDate = DateTime.Now;
//            if (todt.HasValue)
//            {
//                compDate = todt.Value;
//            }

//            List<Material> retv = new List<Material>();

//            try
//            {
//                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//                using (SqlCommand cmd = con.CreateCommand())
//                {
//                    con.Open();

//                    cmd.CommandText = @"
//                        SELECT macno, stockerno, materialcd, lotno, startdt, enddt, issampled, isnascastart, isnascaend
//                        FROM TnMacMat
//                        WHERE 
//                         macno = @MACNO";

//                    cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.NVarChar).Value = this.MacNo;

//                    if (fromdt.HasValue)
//                    {
//                        cmd.CommandText += " AND (enddt is null OR enddt >= @STARTDT)";
//                        cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = fromdt;
//                    }

//                    if (todt.HasValue)
//                    {
//                        cmd.CommandText += " AND startdt <= @ENDDT";
//                        cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = compDate;
//                    }

//                    if (notNascaEndOnly)
//                    {
//                        cmd.CommandText += " AND isnascaend=0";
//                    }


//                    using (SqlDataReader rd = cmd.ExecuteReader())
//                    {
//                        while (rd.Read())
//                        {
//                            string materialcd = SQLite.ParseString(rd["materialcd"]);
//                            string lotno = SQLite.ParseString(rd["lotno"]);

//                            Material m = Material.GetMaterial(materialcd, lotno);
//                            if (m == null)
//                            {
//                                throw new ArmsException("原材料ロット情報が存在しません:" + lotno);
//                            }
//                            m.InputDt = SQLite.ParseDate(rd["startdt"]) ?? DateTime.MinValue;
//                            m.RemoveDt = SQLite.ParseDate(rd["enddt"]);
//                            m.StockerNo = SQLite.ParseInt(rd["stockerno"]);
//                            m.IsNascaStart = SQLite.ParseBool(rd["isnascastart"]);
//                            m.IsNascaEnd = SQLite.ParseBool(rd["isnascaend"]);
//                            m.IsTimeSampled = SQLite.ParseBool(rd["issampled"]);

//                            retv.Add(m);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new ArmsException("投入済み原材料取得エラー", ex);
//            }
//            return retv.ToArray();
//        }

		#region 投入されたウェハーをカセット情報から取得

		/// <summary>
		/// 期間中に投入されていたウェハー、ダイボンド樹脂の一覧を取得
		/// </summary>
		/// <param name="fromdt"></param>
		/// <param name="todt"></param>
		/// <returns></returns>
		//public Material[] GetMaterialsDieBond(DateTime fromdt, DateTime? todt, string stocker1, string stocker2)
		//{

		//    try
		//    {

		//        int from = 0;
		//        if (string.IsNullOrEmpty(stocker1) == false)
		//        {
		//            from = int.Parse(stocker1.Split('-')[1]);
		//        }

		//        int to = 0;
		//        int change = 0;

		//        if (string.IsNullOrEmpty(stocker2) == false)
		//        {
		//            to = int.Parse(stocker2.Split('-')[1]);
		//            change = int.Parse(stocker2.Split('-')[0]);

		//        }
		//        List<Material> retv = new List<Material>();

		//        Material[] matlist = GetMaterials(fromdt, todt);
		//        if (matlist.Length == 0)
		//        {
		//            return matlist;
		//        }

		//        matlist = matlist.OrderBy(m => m.StockerNo).OrderBy(m => m.InputDt).ToArray();

		//        int prevStocker = 0;
		//        int currentBox = 0;


		//        //チェンジャー交換回数より実際の交換回数が少ない場合は、
		//        //チェンジャー交換回数を回数に合わせる　（メンテ対策）
		//        foreach (Material m in matlist)
		//        {
		//            if (m.StockerNo == 0) continue;

		//            if (m.StockerNo < prevStocker)
		//            {
		//                //ストッカー番号が若返った場合は箱が変わったと判定
		//                currentBox += 1;
		//            }
		//            prevStocker = m.StockerNo;
		//        }
		//        if (currentBox < change) change = currentBox;



		//        currentBox = 0;
		//        prevStocker = 1;
		//        foreach (Material m in matlist)
		//        {
		//            //StockerNo=0は通常原材料
		//            if (m.StockerNo == 0)
		//            {
		//                retv.Add(m);
		//                continue;
		//            }


		//            if (m.StockerNo < prevStocker)
		//            {
		//                //ストッカー番号が若返った場合は箱が変わったと判定
		//                currentBox += 1;
		//            }
		//            prevStocker = m.StockerNo;

		//            //最初のカセットはFROM以上の段数を原材料に採用
		//            if (currentBox == 0)
		//            {
		//                if (m.StockerNo >= from)
		//                {
		//                    if (change == 0)
		//                    {
		//                        //カセット交換回数が0の場合はTOより若い段数のみ
		//                        if (m.StockerNo <= to)
		//                        {
		//                            retv.Add(m);
		//                        }
		//                    }
		//                    else
		//                    {
		//                        retv.Add(m);
		//                    }
		//                }
		//            }
		//            //2個目以降のカセットで交換回数に満たないカセットは全投入
		//            else if (currentBox < change)
		//            {
		//                retv.Add(m);
		//            }
		//            //交換回数と一致しているカセット（最終カセット）はTO以前の段数を採用
		//            else if (currentBox == change)
		//            {
		//                if (m.StockerNo <= to)
		//                {
		//                    retv.Add(m);
		//                }
		//            }
		//        }

		//        return retv.ToArray();
		//    }
		//    catch (Exception ex)
		//    {
		//        throw new ArmsException("ウェハー段数取得エラー", ex);
		//    }

		//}
		#endregion

		#region 投入されたフレームをストッカー情報から取得

		/// <summary>
		/// 期間中に投入されていたフレームの一覧を取得
		/// </summary>
		/// <param name="fromdt"></param>
		/// <param name="todt"></param>
		/// <returns></returns>
		//public Material[] GetMaterialsFrameLoader(DateTime fromdt, DateTime? todt, string stocker1, string stocker2)
		//{
		//    List<Material> retv = new List<Material>();

		//    Material[] matlist = GetMaterials(fromdt, todt);

		//    if (!string.IsNullOrEmpty(stocker1) && stocker1 != "0")
		//    {
		//        Material[] org = matlist.Where(m => m.StockerNo == 1).ToArray();

		//        foreach (Material m in org)
		//        {
		//            retv.Add(m);
		//        }
		//    }


		//    if (!string.IsNullOrEmpty(stocker2) && stocker2 != "0")
		//    {
		//        Material[] org = matlist.Where(m => m.StockerNo == 2).ToArray();
		//        foreach (Material m in org)
		//        {
		//            retv.Add(m);
		//        }
		//    }

		//    //Stocker0番はそのまま返す
		//    Material[] org0 = matlist.Where(m => m.StockerNo == 0).ToArray();

		//    foreach (Material m in org0)
		//    {
		//        retv.Add(m);
		//    }

		//    return retv.ToArray();
		//}
		#endregion

		#endregion


		#region GetAvailableMachines その工程で投入可能な設備一覧を取得

		/// <summary>
		/// その工程で投入可能な設備一覧を取得する
		/// </summary>
		/// <param name="inlineNo"></param>
		/// <param name="procNo"></param>
		/// <returns></returns>
//        public static Machine[] GetAvailableMachines(int procNo)
//        {
//            List<Machine> retv = new List<Machine>();

//            try
//            {
//                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//                using (SqlCommand cmd = con.CreateCommand())
//                {
//                    con.Open();

//                    cmd.CommandText = @"
//                        SELECT 
//                          procno , 
//                          macno 
//                        FROM 
//                          tmpromac 
//                        WHERE 
//                          procno = @PROCNO";

//                    cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procNo;

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            int macno = SQLite.ParseInt(reader["macno"]);
//                            Machine mac = Machine.GetMachine(macno);
//                            if (mac != null)
//                            {
//                                retv.Add(mac);
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LogManager.GetCurrentClassLogger().Error(ex);
//                throw ex;
//            }

//            return retv.ToArray();
//        }
		#endregion

		#region GetStockerNo

		/// <summary>
		/// QRコード内容からストッカー番号を取得
		/// QRコードはヘッダー込み
		/// </summary>
		/// <param name="stockerCd"></param>
		/// <returns></returns>
		//public static int? GetStockerNo(string stockerCd)
		//{
		//    using (SqlConnection con = new SqlConnection(SQLite.ConStr))
		//    using (SqlCommand cmd = con.CreateCommand())
		//    {
		//        con.Open();

		//        cmd.Parameters.Add("@QRCD", SqlDbType.NVarChar).Value = (object)stockerCd ?? DBNull.Value;

		//        cmd.CommandText = "SELECT stockerno FROM TmMachineStocker WHERE qrcode=@QRCD";
		//        object val = cmd.ExecuteScalar();

		//        if (val == null)
		//        {
		//            return null;
		//        }

		//        return Convert.ToInt32(val);
		//    }
		//}
		#endregion



		/// <summary>
		/// 削除フラグ付きも含め、同一設備CDを持つ装置リストを取得する
		/// </summary>
		/// <returns></returns>
//        public static Machine[] GetDupulicateMachines()
//        {
//            List<Machine> retv = new List<Machine>();

//            try
//            {
//                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//                using (SqlCommand cmd = con.CreateCommand())
//                {
//                    con.Open();

//                    cmd.CommandText = @"
//                           select macno, plantnm, clasnm, resinchkfg, waferchkfg, plantcd, stockertype, delfg from TmMachine
//                            where plantcd in
//                            (select plantcd from tmmachine where outline = 0 group by plantcd
//                            having count(*) > 1) and outline = 0";

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            Machine mac = new Machine();

//                            mac.MacNo = SQLite.ParseInt(reader["macno"]);
//                            mac.MachineName = SQLite.ParseString(reader["plantnm"]);
//                            mac.ClassName = SQLite.ParseString(reader["clasnm"]);
//                            mac.ResinCheckFg = SQLite.ParseBool(reader["resinchkfg"]);
//                            mac.WaferCheckFg = SQLite.ParseBool(reader["waferchkfg"]);
//                            mac.NascaPlantCd = SQLite.ParseString(reader["plantcd"]);

//                            int stockertype = SQLite.ParseInt(reader["stockertype"]);

//                            if (stockertype == STOCKER_TYPE_DOUBLE_FRAMELOADER)
//                            {
//                                mac.HasDoubleStocker = true;
//                            }

//                            if (stockertype == STOCKER_TYPE_WAFER_CHANGER)
//                            {
//                                mac.HasWaferChanger = true;
//                            }

//                            mac.DelFg = SQLite.ParseBool(reader["delfg"]);

//                            // ハイフンの設備番号は無視する
//                            if (mac.NascaPlantCd == "-") continue;
//                            retv.Add(mac);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new ArmsException("設備マスタ取得エラー", ex);
//            }

//            return retv.ToArray();
//        }


		/// <summary>
		/// EICSの装置タイプ関連を読み込み
		/// </summary>
		/// <param name="mac"></param>
		/// <returns></returns>
		//public static string GetCurrentEICSTypeCd(Machine mac)
		//{
		//    if (mac == null || string.IsNullOrEmpty(mac.NascaPlantCd) || mac.NascaPlantCd == "-")
		//    {
		//        return null;
		//    }

		//    try
		//    {
		//        if (string.IsNullOrEmpty(Config.QCILXmlFilePath)) return null;
		//        XDocument doc = XDocument.Load(Config.QCILXmlFilePath);
		//        if (doc == null) return null;

		//        string typecd = null;
		//        var rootelm = doc.Root.Element("qcil_info").Element("EquipmentList");
		//        if (rootelm != null)
		//        {
		//            //新EICS用
		//            var macelms = rootelm.Elements("Equipment");
		//            var macelm = macelms.Where(e => e.Attribute("no").Value == mac.NascaPlantCd).FirstOrDefault();
		//            if (macelm == null) return null;
		//            typecd = macelm.Attribute("value").Value;
		//        }
		//        else
		//        {
		//            //旧EICS用
		//            string oldelmnm = mac.NascaPlantCd + "_Material_cd";
		//            var macelm = doc.Root.Element("qcil_info").Element(oldelmnm);
		//            if (macelm == null) return null;
		//            typecd = macelm.Attribute("value").Value;
		//        }

		//        if (typecd == null) return null;
		//        return typecd.ToUpper().Trim();
		//    }
		//    catch (Exception ex)
		//    {
		//        Log.Error("QCIL装置設定読込異常:" + mac.NascaPlantCd + ":" + ex.ToString());
		//    }

		//    return null;
		//}


		/// <summary>
		/// 装置の削除フラグを更新する
		/// </summary>
		/// <param name="mac"></param>
//        public static void UpdateDelFg(Machine mac)
//        {

//            try
//            {
//                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//                using (SqlCommand cmd = con.CreateCommand())
//                {
//                    con.Open();
//                    cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = SQLite.SerializeBool(mac.DelFg);
//                    cmd.Parameters.Add("@MACNO", SqlDbType.Int).Value = mac.MacNo;
//                    cmd.CommandText = @"
//                           update TmMachine SET delfg = @DELFG WHERE macno=@MACNO";

//                    cmd.ExecuteNonQuery();
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new ArmsException("設備マスタ更新エラー", ex);
//            }

//        }

		/// <summary>
		/// 高効率ライン　仮想マガジン作成指示
		/// </summary>
		/// <returns></returns>
		//public string GetMelPDAWorkStartDir()
		//{
		//    return System.IO.Path.Combine(Config.MelWorkStartBasePath, MacNo.ToString());
		//}

		/// <summary>
		/// 高効率ライン　仮想マガジン削除指示
		/// </summary>
		/// <returns></returns>
		//public string GetMelPDAWorkCompDir()
		//{
		//    return System.IO.Path.Combine(Config.MelWorkCompltBasePath, MacNo.ToString());
		//}
		//public override string ToString()
		//{
		//    return LongName;
		//}
	}
}
