using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ArmsApi.Model
{

    #region 不良項目 DefItem

    /// <summary>
    /// 不良項目
    /// </summary>
    public class DefItem : IComparable
    {
        public DefItem() { }
        public DefItem(int procNO, string classCD, string causeCD, string defectCD, int defectCT)
        {
			this.ProcNo = procNO;
            this.CauseCd = causeCD;
            this.ClassCd = classCD;
            this.DefectCd = defectCD;
            this.DefectCt = defectCT;
        }

        /// <summary>
        /// 表示順
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// 起因CD
        /// </summary>
        public string CauseCd { get; set; }

        /// <summary>
        /// 起因名称
        /// </summary>
        public string CauseName { get; set; }

        /// <summary>
        /// 分類CD
        /// </summary>
        public string ClassCd { get; set; }

        /// <summary>
        /// 分類名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 不良CD
        /// </summary>
        public string DefectCd { get; set; }

        /// <summary>
        /// 不良名称
        /// </summary>
        public string DefectName { get; set; }

        /// <summary>
        /// 不良数
        /// </summary>
        public int DefectCt { get; set; }

		/// <summary>
		/// EICS画面にエラー表示するか
		/// </summary>
		public bool IsDisplayedEICS { get; set; }

		/// <summary>
		/// 工程NO
		/// </summary>
		public int ProcNo { get; set; }

        #region IComparable メンバ

        public int CompareTo(object obj)
        {
            //nullより大きい
            if (obj == null)
            {
                return 1;
            }

            //違う型とは比較できない
            if (this.GetType() != obj.GetType())
            {
                throw new ArgumentException("別の型とは比較できません。", "obj");
            }
            DefItem def = (DefItem)obj;
            return this.GetHashCode().CompareTo(def.GetHashCode());
        }

        #endregion
    }
    #endregion

	public class DefectJudge
	{
		public string DefItemCd { get; set; }
		public decimal JudgDefectCt { get; set; }
	}

    /// <summary>
    /// 不良情報クラス
    /// </summary>
    public class Defect
    {
        public Defect() { }
        public Defect(string lotNO, int procNO, List<DefItem> defItems) 
        {
            this.LotNo = lotNO;
            this.ProcNo = procNO;
            this.DefItems = defItems;
        }

        /// <summary>
        /// 吸湿保管点灯試験で除外する工程
        /// </summary>
        private const int KHLTEST_EXCLUDE_PROCNO = 1;

        /// <summary>
        /// 吸湿保管点灯試験の判定に使う不良コード
        /// </summary>
        private const string KHLTEST_CAUSE_DEFECT_CD = "F0021";

        /// <summary>
        /// 吸湿保管点灯試験必要閾値
        /// </summary>
        private const int KHLTEST_THRESHOLD = 1;

        /// <summary>
        /// DB-BM免責
        /// </summary>
        public const string DB_BADMARK_DEFECT_CD = "F1106";

        /// <summary>
        /// DB-樹脂不良
        /// </summary>
        public const string DB_RESIN_DEFECT_CD = "F0019";

        #region プロパティ

        public string LotNo { get; set; }

        public int ProcNo { get; set; }

        public List<DefItem> DefItems { get; set; }

        #region FJH ADD
        public string MagazineNo { get; set; }
        #endregion

        #endregion

        /// <summary>
        /// 指定タイプ、工程の不良項目一覧を取得
        /// FJH 引数追加
        /// </summary>
        /// <param name="typecd"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        //public static DefItem[] GetDefectMaster(string typecd, int? procno)
        public static DefItem[] GetDefectMaster(string typecd, int? procno, string mode = "")
        {
            List<DefItem> retv = new List<DefItem>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd ?? "";

                    con.Open();

                    //新規Insert
                    cmd.CommandText = @"
                            SELECT
                                itemno,
                                causecd,
                                classcd,
                                itemcd,
                                itemnm,
                                causenm,
                                classnm
                            FROM
                                TmDefect
                            WHERE typecd = @TYPECD
                            AND delfg = 0";

					if (procno.HasValue)
					{
						//工程マスタ取得
						Process proc = Process.GetProcess(procno.Value);
						cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = proc.WorkCd ?? "";
						cmd.CommandText += " AND workcd = @WORKCD ";                            
					}

                    //FJH ADD START
                    cmd.Parameters.Add("@CLASSCD", SqlDbType.Char).Value = ArmsApi.Config.SUBST_DEFECT_CLASSCD;
                    if (mode == "")
                    {
                        cmd.CommandText += " AND classcd <> @CLASSCD";
                    }
                    else
                    {
                        cmd.CommandText += " AND classcd = @CLASSCD";
                    }
                    //FJH ADD END

                    cmd.CommandText = cmd.CommandText.Replace("\r\n", "");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DefItem item = new DefItem();
                            item.CauseCd = SQLite.ParseString(reader["causecd"]);
                            item.CauseName = SQLite.ParseString(reader["causenm"]);
                            item.ClassCd = SQLite.ParseString(reader["classcd"]);
                            item.ClassName = SQLite.ParseString(reader["classnm"]);
                            item.DefectCd = SQLite.ParseString(reader["itemcd"]);
                            item.DefectName = SQLite.ParseString(reader["itemnm"]);
                            item.OrderNo = SQLite.ParseInt(reader["itemno"]);

                            retv.Add(item);
                        }
                    }
                }

                return retv.ToArray();
            }
            catch (Exception ex)
            {
                Log.SysLog.Info(string.Format("{0} 型番:{1} 工程NO:{2}", ex.ToString(), typecd, procno));
                return retv.ToArray();
            }
        }

		public static DefItem[] GetDefectMaster(string typecd)
		{
			return GetDefectMaster(typecd, null);
		}

        /// <summary>
        /// 特定不良発生の抜き取り検査判定
        /// </summary>
        private void JudgeDefIsp()
        {
            foreach (DefItem item in this.DefItems)
            {
                GnlMaster[] procs = GnlMaster.Search("DEFISP_PROC", item.DefectCd, this.ProcNo.ToString(), null);

                if (procs.Length >= 1)
                {
                    GnlMaster[] thresholds = GnlMaster.Search("DEFISP_THRESHOLD", item.DefectCd, null, this.ProcNo.ToString());
                    if (thresholds.Length == 0)
                    {
                        Log.SysLog.Error("特定不良判定の閾値に異常があります:" + item.DefectCd + ":DEFISP_THRESHOLD");
                        continue;
                    }

                    int th;
                    if (int.TryParse(thresholds[0].Val, out th) == false)
                    {
                        Log.SysLog.Error("特定不良判定の閾値に異常があります:" + item.DefectCd + ":DEFISP_THRESHOLD");
                        continue;
                    }

                    if (item.DefectCt < th) continue;

                    foreach (GnlMaster proc in procs)
                    {
                        int procno;
                        if (int.TryParse(proc.Val2, out procno) == false)
                        {
                            Log.SysLog.Error("特定不良判定の工程NOに異常があります:" + item.DefectCd + ":DEFISP_PROC");
                            continue;
                        }

                        Inspection isp = new Inspection();
                        isp.LotNo = this.LotNo;
                        isp.ProcNo = procno;
                        isp.DeleteInsert();
                    }
                }
            }
        }

        #region GetDefect

        /// <summary>
        /// マスタ上存在する全ての不良項目を返す。
        /// 記録されている不良以外は数量0
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="typecd"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        public static DefItem[] GetAllDefect(string lotno, string typecd, int procno)
        {
            //マガジン分割対応
            lotno = Order.MagLotToNascaLot(lotno);

            Defect record = GetDefect(lotno, procno);

            DefItem[] master = GetDefectMaster(typecd, procno);

            foreach (DefItem d in record.DefItems)
            {

                bool found = false;
                foreach (DefItem m in master)
                {

                    if (d.CauseCd == m.CauseCd && d.ClassCd == m.ClassCd && d.DefectCd == m.DefectCd)
                    {
                        m.DefectCt = d.DefectCt;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new ArmsException("不良マスタに存在しません:" + d.DefectCd);
                }
            }

            return master;
        }

        /// <summary>
        /// マスタ上存在する全ての不良項目を返す。
        /// 記録されている不良以外は数量0
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        public static DefItem[] GetAllDefect(AsmLot lot, int procno)
        {
            return GetAllDefect(lot.NascaLotNo, lot.TypeCd, procno);
        }

        public static Defect GetDefect(string lotno) 
        {
            return GetDefect(lotno, null);
        }

		public static int GetPreCutDefect(string asmlotno)
		{
			int retv = 0;

			//CT
			Process ctproc = Process.GetProcess("TC0003");
			if (ctproc != null)
			{
				Defect ctdef = GetDefect(asmlotno, ctproc.ProcNo);
				if (ctdef.DefItems.Count != 0)
				{
					retv += ctdef.DefItems.Sum(d => d.DefectCt);
				}
			}

			//DC
			Process dcproc = Process.GetProcess("TC0005");
			if (dcproc != null)
			{
				Defect dcdef = GetDefect(asmlotno, dcproc.ProcNo);
				if (dcdef.DefItems.Count != 0)
				{
					retv += dcdef.DefItems.Sum(d => d.DefectCt);
				}
			}

			return retv;
		}

        /// <summary>
        /// 指定ロット、工程の不良全取得
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        public static Defect GetDefect(string lotno, int? procno)
        {
            //マガジン分割対応
            lotno = Order.MagLotToNascaLot(lotno);

            List<DefItem> items = new List<DefItem>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno ?? "";

                try
                {
                    con.Open();

                    //新規Insert
                    cmd.CommandText = @"
                        SELECT 
                          procno, 
                          causecd , 
                          classcd , 
                          defectcd , 
                          defectct 
                        FROM 
                          tndefect 
                        WHERE 
                          lotno = @LOTNO";

                    if (procno.HasValue)
                    {
                        cmd.CommandText += " AND procno = @PROCNO";
                        cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno.Value;
                    }

                    //FJH ADD START
                    cmd.Parameters.Add("@CLASSCD", SqlDbType.Char).Value = ArmsApi.Config.SUBST_DEFECT_CLASSCD;
                    cmd.CommandText += " AND classcd <> @CLASSCD";
                    //FJH ADD END

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DefItem item = new DefItem();
                            item.ProcNo = SQLite.ParseInt(reader["procno"]);
                            item.CauseCd = SQLite.ParseString(reader["causecd"]);
                            item.ClassCd = SQLite.ParseString(reader["classcd"]);
                            item.DefectCd = SQLite.ParseString(reader["defectcd"]);
                            item.DefectCt = SQLite.ParseInt(reader["defectct"]);

                            items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException("不良情報取得エラー:" + lotno, ex);
                }
            }

            Defect retv = new Defect();
            retv.LotNo = lotno;
            if (procno.HasValue)
            {
                retv.ProcNo = procno.Value;
            }
            retv.DefItems = items;
            return retv;
        }

        #endregion

        public void DeleteInsert()
        {
            DeleteInsert(SQLite.ConStr);
        }

        /// <summary>
        /// 該当proc,lotの不良を全削除した上で登録
        /// </summary>
        /// <param name="procno"></param>
        /// <param name="defects"></param>
        public void DeleteInsert(string constr)
        {
            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            Log.SysLog.Info("不良情報更新" + this.LotNo);

            // データベース登録前にマスタに存在するかの確認
            #region 登録対象の不良項目のマスタ有無チェック
            AsmLot lot = AsmLot.GetAsmLot(this.LotNo);
            if (lot != null)
            {
                DefItem[] master = GetDefectMaster(lot.TypeCd, this.ProcNo);
                foreach (DefItem d in this.DefItems)
                {
                    List<DefItem> foundList = master.Where(m => m.CauseCd == d.CauseCd && m.ClassCd == d.ClassCd && m.DefectCd == d.DefectCd).ToList();
                    if (foundList.Count == 0)
                    {
                        throw new ArmsException("不良マスタに存在しません: " + lot.TypeCd + "- (" + d.DefectCd + ", " + d.CauseCd + ", " + d.ClassCd + ")");
                    }
                }
            }
            #endregion

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo ?? "";
                    cmd.Parameters.Add("@PROC", SqlDbType.BigInt).Value = this.ProcNo;
                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                    //前履歴は削除
                    cmd.CommandText = "DELETE FROM TnDefect WHERE lotno=@LOTNO AND procno=@PROC";

                    //FJH ADD START
                    cmd.Parameters.Add("@CLASSCD", SqlDbType.Char).Value = ArmsApi.Config.SUBST_DEFECT_CLASSCD;
                    cmd.CommandText += " AND classcd <> @CLASSCD";
                    //FJH ADD END

                    cmd.ExecuteNonQuery();

                    SqlParameter prmCauseCd = cmd.Parameters.Add("@CAUSECD", SqlDbType.NVarChar);
                    SqlParameter prmClassCd = cmd.Parameters.Add("@CLASSCD", SqlDbType.NVarChar);
                    SqlParameter prmDefectCd = cmd.Parameters.Add("@DEFECTCD", SqlDbType.NVarChar);
                    SqlParameter prmDefectCt = cmd.Parameters.Add("@DEFECTCT", SqlDbType.BigInt);

                    //新規Insert
                    cmd.CommandText = @"
                            INSERT
                             INTO TnDefect(lotno
	                            , procno
	                            , causecd
	                            , classcd
	                            , defectcd
	                            , defectct
	                            , lastupddt
                                , delfg)
                            values(@LOTNO
	                            , @PROC
	                            , @CAUSECD
	                            , @CLASSCD
	                            , @DEFECTCD
	                            , @DEFECTCT
	                            , @UPDDT
                                , 0)";

                    IEnumerable<string> defectNotIncludeList = GnlMaster.GetDefectNotInclude()
                        .Select(d => d.Code);

                    foreach (DefItem d in this.DefItems)
                    {
                        // 不良数0は対象外
                        if (d.DefectCt == 0) continue;

                        // 計上済前工程不良は登録しない
                        if (defectNotIncludeList.Contains(d.DefectCd)) continue;
                            
                        prmCauseCd.Value = d.CauseCd ?? "";
                        prmClassCd.Value = d.ClassCd ?? "";
                        prmDefectCd.Value = d.DefectCd ?? "";
                        prmDefectCt.Value = d.DefectCt;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("不良情報更新エラー", ex);
                }
            }

            //ライン間受渡しの場合は処理を行わない
            if (constr == SQLite.ConStr)
            {
				if (Config.GetLineType != Config.LineTypes.NEL_GAM && Config.GetLineType != Config.LineTypes.MEL_GAM)
				{
					KHLTestCheckAndUpdateLot(this.LotNo, this.ProcNo, this.DefItems);
				}

                //特定工程・不良発生時の抜き取り検査フラグ追加判定
                JudgeDefIsp();

                //QCIL更新
                lot = AsmLot.GetAsmLot(this.LotNo);
                if (lot != null)
                {
                    DefItem[] updateList = GetAllDefect(lot, this.ProcNo);

					List<DefItem> displayList = this.DefItems.Where(d => d.IsDisplayedEICS).ToList();

					UpdateEICS(lot, this.ProcNo, updateList, displayList);
				}
            }
        }

		/// <summary>
		/// 不良の全削除 + 登録処理はしない。EICSへの傾向管理ログのアップデートのみ行う 2015.9.2 永尾追加
		/// </summary>
		/// <param name="procno"></param>
		/// <param name="defects"></param>
		public void DeleteInsert2(string constr)
		{
			//ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要

			//ライン間受渡しの場合は処理を行わない
			if (constr == SQLite.ConStr)
			{
				KHLTestCheckAndUpdateLot(this.LotNo, this.ProcNo, this.DefItems);

				//特定工程・不良発生時の抜き取り検査フラグ追加判定
				JudgeDefIsp();

				//QCIL更新
				AsmLot lot = AsmLot.GetAsmLot(this.LotNo);
				if (lot != null)
				{
					//DefItem[] updateList = GetAllDefect(lot, this.ProcNo);

					DefItem[] updateList = this.DefItems.ToArray();

					List<DefItem> displayList = this.DefItems.Where(d => d.IsDisplayedEICS).ToList();

                    //Log.ApiLog.Info("TnLOGOutSideにBM差異をアップデート:" + this.LotNo);
					UpdateEICS(lot, this.ProcNo, updateList, displayList);
				}
			}
		}

        public static void Delete(SqlCommand cmd, string lotno) 
        {
            if (string.IsNullOrEmpty(lotno))
            {
                return;
            }

            string sql = " UPDATE TnDefect SET delfg = 1 WHERE lotno Like @LOTNO ";
            cmd.CommandText = sql;

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new ArmsException("不良削除エラー:" + lotno, ex);
            }
        }

        /// <summary>
        /// デフォルト検査数マスタ取得
        /// </summary>
        /// <param name="typecd"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        public static int? GetDefaultInspectCtMaster(string typecd, int procno)
        {
            GnlMaster[] mst = GnlMaster.Search("DEFCT", typecd, procno.ToString(), null);

            if (mst.Length == 0) return null;

            else
            {
                int ct;
                if (int.TryParse(mst[0].Val2, out ct) == true)
                {
                    return ct;
                }
                else
                {
                    return null;
                }
            }
        }

		/// <summary>
		/// 不良判定マスタ取得
		/// 41移管2次で検証中
		/// </summary>
		/// <param name="typecd"></param>
		/// <param name="procno"></param>
		/// <returns></returns>
		public static List<DefectJudge> GetDefectJudgeMaster(string typecd, int procno)
		{
			List<DefectJudge> retv = new List<DefectJudge>();

			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT typecd, procno, defitemcd, judgdefectct 
								FROM TmDefectJudge WITH (nolock)
								WHERE (delfg = 0) AND (typecd = @TYPECD) AND (procno = @PROCNO) ";

				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
				cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno;

				cmd.CommandText = sql;

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						DefectJudge d = new DefectJudge();
						d.DefItemCd = rd["defitemcd"].ToString().Trim();
						d.JudgDefectCt = Convert.ToDecimal(rd["judgdefectct"]);
						retv.Add(d);
					}
				}
			}

			return retv;
		}

        #region UpdateEICS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <param name="defList"></param>
		public static void UpdateEICS(AsmLot lot, int procno, DefItem[] defList, List<DefItem> displayList)
        {
            try
            {
                Order[] magOrdList = Order.GetOrder(lot.NascaLotNo, procno);
                if (magOrdList.Length == 0)
                {
                    return;
                }
                Order ord = magOrdList[0];

                MachineInfo mac = MachineInfo.GetMachine(ord.MacNo);
                if (mac == null)
                {
                    return;
                }

                int lineNo = MachineInfo.GetEICSLineNo(mac.NascaPlantCd);
				//if (string.IsNullOrEmpty(lineNo)) 
				//{
				//	return;
				//}

				string modelNM = MachineInfo.GetEICSModelNm(mac.NascaPlantCd);
				if (string.IsNullOrEmpty(modelNM)) 
				{
					return;
				}

                using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(Config.Settings.QCILConSTR))
                using (System.Data.SqlClient.SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    DateTime inspectdt = DateTime.Now;
                    if (ord.WorkEndDt.HasValue)
                    {
                        inspectdt = ord.WorkEndDt.Value;
                    }

                    #region パラメータ設定

                    cmd.Parameters.Add("@INLINECD", SqlDbType.Int).Value = lineNo;
                    cmd.Parameters.Add("@Equipment_NO", SqlDbType.Char).Value = mac.NascaPlantCd;
                    cmd.Parameters.Add("@Measure_DT", SqlDbType.DateTime).Value = inspectdt;
                    cmd.Parameters.Add("@NascaLot_NO", SqlDbType.VarChar).Value = lot.NascaLotNo;
                    cmd.Parameters.Add("@Material_CD", SqlDbType.Char).Value = lot.TypeCd;
                    cmd.Parameters.Add("@Magazine_NO", SqlDbType.Char).Value = ord.InMagazineNo ?? "";
                    cmd.Parameters.Add("@Seq_NO", SqlDbType.Int).Value = 1;
                    cmd.Parameters.Add("@SParameter_VAL", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@Message_NM", SqlDbType.VarChar).Value = "";
                    cmd.Parameters.Add("@Check_FG", SqlDbType.Bit).Value = 0;
                    cmd.Parameters.Add("@Del_FG", SqlDbType.Bit).Value = 0;
                    cmd.Parameters.Add("@UpdUser_CD", SqlDbType.Char).Value = "660";
                    cmd.Parameters.Add("@LastUpd_DT", SqlDbType.DateTime).Value = DateTime.Now;
                    #endregion


                    System.Data.SqlClient.SqlParameter prmDefCd = cmd.Parameters.Add("@DEFECTCD", SqlDbType.VarChar);
                    System.Data.SqlClient.SqlParameter prmPrmNo = cmd.Parameters.Add("@QcParam_NO", SqlDbType.Int);
                    System.Data.SqlClient.SqlParameter prmDVal = cmd.Parameters.Add("@DParameter_VAL", SqlDbType.Decimal);

					System.Data.SqlClient.SqlParameter prmTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.VarChar);
					prmTypeCd.Value = lot.TypeCd;

					System.Data.SqlClient.SqlParameter prmModelNm = cmd.Parameters.Add("@MODELNM", SqlDbType.VarChar);
					prmModelNm.Value = modelNM;

                    // <!-- 処理高速化
                    prmPrmNo.Value = 0;
                    prmDVal.Value = 0;
                    prmDefCd.Value = "";
                    List<string> defCdList = defList.Select(def => def.DefectCd + "_" + def.ClassCd).ToList();
                    cmd.CommandText = "SELECT TmPRM.QcParam_NO, Parameter_NM FROM TmPRM WITH(nolock) INNER JOIN TmPLM WITH(nolock) ON TmPRM.QcParam_NO = TmPLM.QcParam_NO " +
                                      $" Where TmPRM.Del_FG = 0 AND TmPLM.Del_FG = 0 AND Parameter_NM IN ('{ string.Join("','", defCdList)}') AND TmPLM.Material_CD = @TYPECD AND TmPLM.Model_NM = @MODELNM ";
                    Dictionary<string, int> prmDict = new Dictionary<string, int>();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            string paramNm = rd["Parameter_NM"].ToString();
                            int paramNo = Convert.ToInt32(rd["QcParam_NO"]);
                            if (prmDict.Keys.Contains(paramNm) == true)
                            {
                                continue;
                            }
                            prmDict.Add(paramNm, paramNo);
                        }
                    }
                    // --> 処理高速化

                    foreach (DefItem def in defList)
                    {

                        prmPrmNo.Value = 0;
                        prmDVal.Value = def.DefectCt;
                        prmDefCd.Value = def.DefectCd + "_" + def.ClassCd;

                        // <!-- 処理高速化
                        //             cmd.CommandText = @"SELECT TmPRM.QcParam_NO FROM TmPRM WITH(nolock) INNER JOIN TmPLM WITH(nolock) ON TmPRM.QcParam_NO = TmPLM.QcParam_NO
                        //Where TmPRM.Del_FG = 0 AND TmPLM.Del_FG = 0 AND Parameter_NM = @DEFECTCD AND TmPLM.Material_CD = @TYPECD AND TmPLM.Model_NM = @MODELNM ";

                        //             object prmNo = cmd.ExecuteScalar();
                        //             if (prmNo == null)
                        //             {
                        //                 continue;
                        //             }
                        //prmPrmNo.Value = Convert.ToInt32(prmNo);

                        int iPrmNo;
                        if (prmDict.TryGetValue(prmDefCd.Value.ToString(), out iPrmNo) == false)
                        {
                            continue;
                        }
                        prmPrmNo.Value = iPrmNo;
                        // --> 処理高速化

                        // TnLogWaitingQueue
                        cmd.CommandText = @"
                            SELECT DParameter_VAL FROM TnLogWaitingQueue WITH(nolock) WHERE Inline_CD=@INLINECD AND Equipment_NO=@Equipment_No
                            AND QcParam_NO=@QcParam_NO AND NascaLot_NO=@NascaLot_NO";

                        object objLastVal = cmd.ExecuteScalar();
                        if (objLastVal != null)
                        {
                            if (Convert.ToInt32(objLastVal) != def.DefectCt)
                            {
                                cmd.CommandText = @"
                                  UPDATE TnLogWaitingQueue SET  Measure_DT=@Measure_DT, DParameter_VAL=@DParameter_VAL, LastUpd_DT=@LastUpd_DT
                                     WHERE Inline_CD=@INLINECD AND Equipment_NO=@Equipment_No
                                       AND QcParam_NO=@QcParam_NO AND NascaLot_NO=@NascaLot_NO";

                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            cmd.CommandText = @"
                                  INSERT INTO TnLogWaitingQueue(Inline_CD,Equipment_NO,Measure_DT,Seq_NO,QcParam_NO,Material_CD,Magazine_NO,NascaLot_NO,DParameter_VAL,SParameter_VAL,Message_NM,Check_FG,Del_FG,UpdUser_CD,LastUpd_DT)
                                    VALUES(@INLINECD,@Equipment_NO,@Measure_DT,@Seq_NO,@QcParam_NO,@Material_CD,@Magazine_NO,@NascaLot_NO,@DParameter_VAL,@SParameter_VAL,@Message_NM,@Check_FG,@Del_FG,@UpdUser_CD,@LastUpd_DT)";

                            cmd.ExecuteNonQuery();
                        }

						if (displayList != null && displayList.Exists(d => d.DefectCd == def.DefectCd && d.ClassCd == def.ClassCd && d.CauseCd == def.CauseCd
							&& d.IsDisplayedEICS == true))
						{
							// TnLOGOutSide
							cmd.CommandText = @"
                            SELECT FinishedDisplay_FG FROM TnLOGOutSide WITH(nolock) WHERE Inline_CD=@INLINECD AND Equipment_NO=@Equipment_No
                            AND QcParam_NO=@QcParam_NO AND NascaLot_NO=@NascaLot_NO";

							objLastVal = cmd.ExecuteScalar();
							if (objLastVal != null)
							{
								cmd.CommandText = @"
                                  UPDATE TnLOGOutSide SET Measure_DT=@Measure_DT, LastUpd_DT=@LastUpd_DT, FinishedDisplay_FG=0
                                     WHERE Inline_CD=@INLINECD AND Equipment_NO=@Equipment_NO
                                       AND QcParam_NO=@QcParam_NO AND NascaLot_NO=@NascaLot_NO";

								cmd.ExecuteNonQuery();
							}
							else
							{
								cmd.CommandText = @"
                                  INSERT INTO TnLOGOutSide(Inline_CD,Equipment_NO,Measure_DT,Seq_NO,QcParam_NO,NascaLot_NO,FinishedDisplay_FG)
                                    VALUES(@INLINECD,@Equipment_NO,@Measure_DT,@Seq_NO,@QcParam_NO,@NascaLot_NO,0)";

								cmd.ExecuteNonQuery();
							}
							//-------------------------
						}
					}
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("UPDATE EICS ERROR:" + ex.ToString());
            }
        }

		public static void UpdateEICS(AsmLot lot, int procno, DefItem[] defList)
		{
			UpdateEICS(lot, procno, defList, null);
		}

        #endregion

		#region UpdateEICSWireBondAddress
		
		/// <summary>
        /// 
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <param name="defList"></param>
        public static void UpdateEICSWireBondAddress(string magno, AsmLot lot, DefItem def, string address, string unit, string empcd)
        {
            if (def.DefectCt == 0)
            {
                throw new ApplicationException("不良数0でアドレス登録は出来ません");
            }

            try
            {
                Magazine mag = Magazine.GetCurrent(magno);
                if (mag == null) throw new ApplicationException("マガジン情報が見つかりません:" + magno);

                //次工程が開始されていたら次工程。無ければ現在完了工程
                Order ord = Order.GetNextMagazineOrder(mag.NascaLotNO, mag.NowCompProcess);
                if (ord == null)
                {
                    ord = Order.GetMagazineOrder(mag.NascaLotNO, mag.NowCompProcess);
                    if (ord == null) throw new ApplicationException("作業中の指図がありません:" + magno);
                }

                Process proc = Process.GetProcess(ord.ProcNo);

                MachineInfo mac = MachineInfo.GetMachine(ord.MacNo);
                if (mac == null) throw new ApplicationException("装置マスタ情報がありません:" + ord.MacNo);

                using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(Config.Settings.QCILConSTR))
                using (System.Data.SqlClient.SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    //ライン番号をEICSから取得
                    int lineNo = MachineInfo.GetEICSLineNo(mac.NascaPlantCd);

                    #region パラメータ設定
                    
                    cmd.Parameters.Add("@LINENO", SqlDbType.Int).Value = lineNo;
                    cmd.Parameters.Add("@EQNO", SqlDbType.Char).Value = mac.NascaPlantCd;
                    cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lot.NascaLotNo;
                    cmd.Parameters.Add("@ADDRESS", SqlDbType.VarChar).Value = address ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@UNIT", SqlDbType.VarChar).Value = unit ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@TGTDT", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@WORKCD", SqlDbType.Char).Value = proc.WorkCd;
                    cmd.Parameters.Add("@DEFITEMCD", SqlDbType.Char).Value = def.DefectCd;
                    cmd.Parameters.Add("@DEFITEMNM", SqlDbType.NVarChar).Value = def.DefectName;
                    cmd.Parameters.Add("@DEFCAUSECD", SqlDbType.Char).Value = def.CauseCd;
                    cmd.Parameters.Add("@DEFCAUSENM", SqlDbType.NVarChar).Value = def.CauseName;
                    cmd.Parameters.Add("@DEFCLASSCD", SqlDbType.Char).Value = def.ClassCd;
                    cmd.Parameters.Add("@DEFCLASSNM", SqlDbType.NVarChar).Value = def.ClassName;
                    cmd.Parameters.Add("@EMPCD", SqlDbType.Char).Value = empcd ?? "9999";
                    cmd.Parameters.Add("@DELFG", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                    #endregion

                    cmd.CommandText = @"
                            SELECT LOT_NO
                            FROM TnDEFECT
                            WHERE Line_CD=@LINENO AND Plant_CD=@EQNO AND Lot_NO=@LOTNO 
                            AND DefAddress_NO=@ADDRESS AND DefUnit_NO=@UNIT AND WORK_CD=@WORKCD";

                    object exists = cmd.ExecuteScalar();
                    if (exists == null)
                    {
                        cmd.CommandText = @"
                            INSERT INTO TnDEFECT(
                            Line_Cd, Plant_CD, Lot_NO, DefAddress_NO, DefUnit_NO, Target_DT, Work_CD,
                            DefItem_CD, DefItem_NM, DefCause_CD, DefCause_NM, DefClass_CD, DefClass_NM, UpdUser_CD, Del_FG, LastUpd_DT)
                            VALUES (@LINENO, @EQNO, @LOTNO, @ADDRESS, @UNIT, @TGTDT, @WORKCD, 
                            @DEFITEMCD, @DEFITEMNM, @DEFCAUSECD, @DEFCAUSENM, @DEFCLASSCD, @DEFCLASSNM, @EMPCD, @DELFG, @LASTUPDDT)";

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.CommandText = @"
                            UPDATE TnDEFECT SET
                                DefItem_CD=@DEFITEMCD, DefItem_NM=@DEFITEMNM, DefCause_CD=@DEFCAUSECD, DefCause_NM=@DEFCAUSENM, DefClass_CD=@DEFCLASSCD,
                                DefClass_NM=@DEFCLASSNM, Del_FG=@DELFG, LastUpd_DT=@LASTUPDDT
                            WHERE Line_CD=@LINENO AND Plant_CD=@EQNO AND Lot_NO=@LOTNO 
                            AND DefAddress_NO=@ADDRESS AND DefUnit_NO=@UNIT AND WORK_CD=@WORKCD";

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("UPDATE EICS ERROR:" + ex.ToString());
                throw ex;
            }
        }



        /// <summary>
        /// ワイヤーボンダーで区分0以外に設定されたエラーアドレス
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <param name="defList"></param>
        public static void UpdateEICSWireBondMappingAddress(string magno, string address, string empcd)
        {
            try
            {
                Magazine mag = Magazine.GetCurrent(magno);
                if (mag == null) throw new ApplicationException("マガジン情報が見つかりません:" + magno);

                using (System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(Config.Settings.QCILConSTR))
                using (System.Data.SqlClient.SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    #region パラメータ設定
                    cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = mag.NascaLotNO;
                    cmd.Parameters.Add("@ADDRESS", SqlDbType.VarChar).Value = address ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@DELFG", SqlDbType.Bit).Value = false;
                    cmd.Parameters.Add("@EMPCD", SqlDbType.Char).Value = empcd ?? "9999";
                    cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                    #endregion

                    cmd.CommandText = @"
                            SELECT LOT_NO
                            FROM TnDEFECTRESIN
                            WHERE Lot_NO=@LOTNO 
                            AND Address_NO=@ADDRESS";

                    object exists = cmd.ExecuteScalar();
                    if (exists == null)
                    {
                        cmd.CommandText = @"
                            INSERT INTO TnDEFECTRESIN(
                            Lot_NO, Address_NO, UpdUser_CD, Del_FG, LastUpd_DT)
                            VALUES (@LOTNO, @ADDRESS, 
                            @EMPCD, @DELFG, @LASTUPDDT)";

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.CommandText = @"
                            UPDATE TnDEFECTRESIN SET
                                Del_FG=@DELFG, LastUpd_DT=@LASTUPDDT
                            WHERE Lot_NO=@LOTNO AND Address_NO=@ADDRESS";

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("UPDATE EICS ERROR:" + ex.ToString());
                throw ex;
            }
        }
        #endregion

        #region KHLTestCheckAndUpdateLot



        /// <summary>
        /// 吸湿保管点灯試験のフラグ更新処理
        /// </summary>
        public static void KHLTestCheckAndUpdateLot(string lotno, int procno, List<DefItem> defList)
        {
            //ダイボンドで発見した場合は何もしない
            if (procno == KHLTEST_EXCLUDE_PROCNO) return;

            AsmLot lot = AsmLot.GetAsmLot(lotno);
            if (lot == null)
            {
                return;
            }

            foreach (DefItem def in defList)
            {
                if (def.DefectCd == KHLTEST_CAUSE_DEFECT_CD && def.DefectCt >= KHLTEST_THRESHOLD)
                {
                    Log.SysLog.Info("[吸湿保管点灯試験]ON:" + lotno);
                    lot.IsKHLTest = true;
                    lot.Update();
                }
            }
        }
        #endregion

        /// <summary>
        /// WB工程NGマッピング用MMファイルのスキップエラーに該当するか
        /// </summary>
        /// <param name="errCode"></param>
        /// <returns></returns>
        public static bool IsWireMMFileSkipErrorCode(string errCode, GnlMaster[] skipErrorList)
        {
            if (skipErrorList.Count() == 0)
            {
                return false;
            }

            errCode = errCode.ToLower();

            if (skipErrorList.Select(s => s.Code).Contains(errCode))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		/// <summary>
		/// WB工程NGマッピング用MMファイルのバッドマークエラーに該当するか
		/// </summary>
		/// <param name="errCode"></param>
		/// <returns></returns>
		public static bool IsWireMMFileBadmarkErrorCode(string errCode)
		{
			if (Config.Settings.WireMMFileBadmarkErrorCodeList == null)
			{
				return false;
			}

			string[] codeList = Config.Settings.WireMMFileBadmarkErrorCodeList;
			errCode = errCode.ToLower();

			if (codeList.Contains(errCode))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool InsertWireMMFileBadmarkError(string magno, int errct, int procno)
		{
			Magazine mag = Magazine.GetCurrent(magno);
			if (mag == null) throw new ApplicationException("svrマガジン情報が見つかりません:" + magno);

			AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
			if (lot == null) throw new ApplicationException("svrロット情報が見つかりません:" + mag.NascaLotNO);

			DefItem di = Config.Settings.WireMMFileBadmarkErrorDefItem;
			if (di == null) throw new ApplicationException("WBバッドマーク不良の設定がArmsConfig.xmlにありません");

			int badmarkCt = GetDefectCountOfPassedProcess(lot, procno, di.DefectCd);

			DefItem[] defs = Defect.GetAllDefect(lot, procno);

			bool found = false;
			foreach (DefItem item in defs)
			{
				if (di.CauseCd == item.CauseCd && di.ClassCd == item.ClassCd && di.DefectCd == item.DefectCd)
				{
					item.DefectCt = Math.Abs(errct - badmarkCt);
					item.IsDisplayedEICS = true;

					Defect d = new Defect();
					d.LotNo = lot.NascaLotNo;
					d.DefItems = new List<DefItem>();
					d.DefItems.Add(item);
					d.ProcNo = procno;
					d.DeleteInsert2(SQLite.ConStr);

					found = true;
				}
			}

			if (found == false)
			{
				if (lot == null) throw new ApplicationException("svr不良明細が見つかりません:" + mag.NascaLotNO);
			}

			return true;
		}

        public static bool InsertWireMMFileSkipError(string magno, int errct, int procno)
        {
            Magazine mag = Magazine.GetCurrent(magno);
            if (mag == null) throw new ApplicationException("svrマガジン情報が見つかりません:" + magno);
            
			AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
            if (lot == null) throw new ApplicationException("svrロット情報が見つかりません:" + mag.NascaLotNO);

            DefItem di = Config.Settings.WireMMFileSkipErrorDefItem;
            if (di == null) throw new ApplicationException("WBスキップ不良の設定がArmsConfig.xmlにありません");

			// DB不良(WBまでの不良)をWBスキップ数から差し引いて、正確なスキップ数を算出する
			int dbDefectCt = GetDefectCountOfPassedProcess(lot, procno);

			// 2015.6.15 スキップ数傾向管理
			// WBスキップ数に部材交換免責を含んでいる為、差し引いて正確なスキップ数を算出する
			int matChangeCt = Defect.GetMaterialChangeCt(lot.NascaLotNo, procno);

			if (errct < dbDefectCt + matChangeCt)
			{
				return false;
			}

			//errct = errct - dbDefectCt;
			errct = errct - dbDefectCt - matChangeCt;
			//-------------------------------------------------------------------------------

            DefItem[] defs = Defect.GetAllDefect(lot, procno);

            bool found = false;
            foreach (DefItem item in defs)
            {
                if (di.CauseCd == item.CauseCd && di.ClassCd == item.ClassCd && di.DefectCd == item.DefectCd)
                {
                    item.DefectCt = errct;
					item.IsDisplayedEICS = true;

                    found = true;
                }
            }

            if (found == false)
            {
                if (lot == null) throw new ApplicationException("svr不良明細が見つかりません:" + mag.NascaLotNO);
            }

            Defect d = new Defect();
            d.LotNo = lot.NascaLotNo;
            d.DefItems = new List<DefItem>(defs);
            d.ProcNo = procno;
            d.DeleteInsert();

			return true;
        }

        /// <summary>
        /// EICSが作成したNasca不良ファイル処理
        /// </summary>
        /// <param name="macNo"></param>
        /// <param name="plantCd"></param>
        public static void ImportNascaDefectFile(int macNo, string plantCd, out string errMessage)
        {
            ImportNascaDefectFile(macNo, plantCd, false, out errMessage);
        }
        public static void ImportNascaDefectFile(int macNo, string plantCd, bool bySubstrate)
        {
            string errMessage = string.Empty;
            ImportNascaDefectFile(macNo, plantCd, bySubstrate, out errMessage);
        }

        public static void ImportNascaDefectFile(int macNo, string plantCd)
        {
            string errMessage = string.Empty;
            ImportNascaDefectFile(macNo, plantCd, false, out errMessage);
        }

        /// <summary>
        /// EICSが作成したNasca不良ファイル処理
        /// </summary>
        /// <param name="magazineNo"></param>
        /// <param name="plantCd"></param>
        /// <param name="procNo"></param>
        /// <param name="bySubstrate">基板毎に不良ファイルが作成される場合：true そうでなければ：false</param>
        public static void ImportNascaDefectFile(int macNo, string plantCd, bool bySubstrate, out string errMessage)
		{
			//List<string> files = Directory.GetFiles(Config.Settings.NascaDefectFilePath, string.Format("*_{0}.nas", plantCd)).ToList();
			List<string> files = DirectoryHelper.GetFiles(Config.Settings.NascaDefectFilePath, string.Format(@".*_{0}\.nas$", plantCd)).ToList();
			files.AddRange(DirectoryHelper.GetFiles(Config.Settings.NascaDefectFilePath, string.Format(@".*_{0}_.*\.nas$", plantCd)).ToList());
            errMessage = string.Empty;

			if (files.Count == 0) 
			{
				return;
			}

			List<FileInfo> fiList = new List<FileInfo>();
			foreach (string file in files)
			{
				FileInfo fi = new FileInfo(file);
				fiList.Add(fi);
			}

            
			//本日0時以降に作られたデータが対象 ←目的が不明なので削除
			//fiList = fiList
			//	.Where(f => DateTime.Now.Date < f.LastWriteTime)
			//	.OrderByDescending(f => f.LastWriteTime).ToList();

			if (fiList.Count == 0)
			{
				return;
			}

            if (Directory.Exists(Config.Settings.NascaDefectFileDonePath) == false)
            {
                Directory.CreateDirectory(Config.Settings.NascaDefectFileDonePath);
            }


            foreach (FileInfo fi in fiList)
			{
				string[] fileNameChars = fi.Name.Split('_');
				string lotNo = fileNameChars[0];
				string magazineNo = fileNameChars[1];

                string carrierNo = string.Empty;
                if (bySubstrate == true && fileNameChars.Length >= 4)
                {
                    carrierNo = fileNameChars[3];
                }


				int procNo = Order.GetLastProcNoFromLot(macNo, lotNo);

                string destPath = Path.Combine(Config.Settings.NascaDefectFileDonePath, Path.GetFileName(fi.Name));

                //同マガジン番号(マガジン単位)、同キャリア番号(キャリア単位)のファイルが他にあれば例外エラーで停止
                foreach (string file in files)
                {
                    if (file == fi.FullName) continue;

                    FileInfo checkfi = new FileInfo(file);
                    string[] checkFileNameChars = checkfi.Name.Split('_');

                    if (bySubstrate == false)
                    {
                        string checkMagazineNo = checkFileNameChars[1];
                        if (magazineNo == checkMagazineNo)
                        {
                            throw new ApplicationException($"NASCA不良データ登録時に同マガジン・設備で別ファイルが見つかりました。ファイルを確認してください。magazine:{magazineNo} NASファイル:{fi.FullName} \r\n");
                        }
                    }
                    else
                    {
                        if (checkFileNameChars.Length >= 4)
                        {
                            string checkCarrierNo = checkFileNameChars[3];
                            if (carrierNo == checkCarrierNo)
                            {
                                throw new ApplicationException($"NASCA不良データ登録時に同キャリア・設備で別ファイルが見つかりました。ファイルを確認してください。carrier:{carrierNo} NASファイル:{fi.FullName} \r\n");
                            }
                        }

                    }
                }

                //不良登録完了フラグON
                Order order = Order.GetOrder(lotNo, procNo).Single();
				//if (order.IsDefectAutoImportEnd)
                string defectLogMsg = string.Empty;

                if (order.IsDefectAutoImportEnd && !bySubstrate)
                {
                    throw new ApplicationException(
                        string.Format("Nasca不良ファイルの重複登録が発生しました。MacNo:{0} LotNo:{1} ProcNo:{2}", macNo, lotNo, procNo));
                }
                else if(isDefectAutoImportEnd(lotNo, procNo))
                {
                    defectLogMsg += string.Format("Nasca不良の追加登録を行います。MacNo:{0} LotNo:{1} ProcNo:{2} file:{3}", macNo, lotNo, procNo, fi.FullName);

                    Defect svrDef = Defect.GetDefect(lotNo, procNo);

                    string svrDefStr = "DB登録済ﾃﾞｰﾀ\r\nDefectCd,CauseCd,ClassCd,qty\r\n";
                    foreach (DefItem defItem in svrDef.DefItems)
                    {
                        svrDefStr += string.Format("{0},{1},{2},{3}\r\n", defItem.DefectCd, defItem.CauseCd, defItem.ClassCd, defItem.DefectCt);
                    }

                    string fileDefStr = "nas内容\r\nDefectCd,CauseCd,ClassCd,qty\r\n";
                    using (StreamReader sr = new StreamReader(fi.FullName))
                    {
                        while (sr.Peek() >= 0)
                        {
                            string line = sr.ReadLine();
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                continue;
                            }
                            fileDefStr += line;
                        }
                    }

                    defectLogMsg += svrDefStr + fileDefStr;
                }

                try
                {
                    InsertNascaDefect(fi.FullName, lotNo, procNo, out errMessage);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(
                        string.Format("NASCA不良データ登録でエラー発生 plant:{0} magazine:{1} NASファイル:{2} \r\n" + ex.Message, plantCd, magazineNo, fi.Name));
                }

                if (string.IsNullOrEmpty(defectLogMsg) == false)
                {
                    Log.ApiLog.Info(defectLogMsg);
                }

				try
				{
                    order = Order.GetOrder(lotNo, procNo).Single();
                    order.IsDefectEnd = true;
					order.IsDefectAutoImportEnd = true;
					order.DeleteInsert(order.NascaLotNo);

					//完了ファイルの移動処理
				
					if (File.Exists(destPath) == true)
					{
						//Doneフォルダ内に同名ファイルがあれば削除
						File.Delete(destPath);
					}
					fi.MoveTo(destPath);
				}
				catch(IOException)
				{
					Log.ApiLog.Info(string.Format("NASCA不良ファイルの完了フォルダへの移動時にエラー発生。\nNASCA登録は正常完了しているため処理続行。\n{0}", fi.FullName));
					//ファイル移動に関するエラーは握りつぶす
				}

            }
		}

		/// <summary>
		/// Nasca不良ファイル登録
		/// </summary>
		/// <param name="inspectionDataPath"></param>
		public static void InsertNascaDefect(string inspectionDataPath, string lotNo, int procNo, out string errMessage)
		{
			List<DefItem> defectList = new List<DefItem>();
            errMessage = string.Empty;

		    using (StreamReader sr = new StreamReader(inspectionDataPath))
			{
				while (sr.Peek() >= 0)
				{
					string line = sr.ReadLine();
					if (string.IsNullOrEmpty(line))
					{
						continue;
					}

					//各行は4項目
					string[] data = line.Trim().Split(',');
					if (data.Length != 4)
					{
						Log.SysLog.Info("不正な検査データです:" + inspectionDataPath);
						continue;
					}

					//4列目は数字
					int count;
					if (int.TryParse(data[3], out count) == false)
					{
						Log.SysLog.Info("不正な検査データです:" + inspectionDataPath);
						continue;
					}

					DefItem defect = new DefItem(procNo, data[2], data[1], data[0], count);
					defectList.Add(defect);
				}
			}

			Defect svrDef = Defect.GetDefect(lotNo, procNo);


            bool autoInspectionSkippedFg = false;

            foreach (DefItem fileDef in defectList)
			{

                if (Config.Settings.AutoInspectionSkipErrorDefItem != null)
				{
					// 検査機マーキング不良の場合、前工程までの不良を差し引いて、WBスキップ数を算出登録
					// 選別の不明数を減らす対応
					if (hasInspectionSkipError(fileDef.ProcNo, fileDef.DefectCd, fileDef.CauseCd, fileDef.ClassCd))
					{
						AsmLot lot = AsmLot.GetAsmLot(lotNo);

                        //民生・車載混載ラインの不具合対応。抜取検査の場合は本機能が正常動作しないので常に0を返す。2016.9.9 湯浅/石口
                        //マッピングOFFの時は全数フラグ自体が使用されないので強制的に計上する。
                        if (lot.IsFullSizeInspection == false && Config.Settings.IsMappingMode == true)
                        {
                            fileDef.DefectCt = 0;
                            continue;
                        }

						int beforeDefCt = GetDefectCountOfPassedProcess(lot, procNo);

						int skipCt = fileDef.DefectCt - beforeDefCt;
						if (skipCt < 0)
						{
							// 前工程までの不良を差し引いてマイナス数になった場合、流動規制をかける
							setRistrictToInspectionSkipError(lotNo, Process.GetNextProcess(procNo, lot).ProcNo, skipCt, out errMessage);
                            autoInspectionSkippedFg = true;
                            continue;
						}
						fileDef.DefectCt = skipCt;

                        autoInspectionSkippedFg = true;
					}
				}

				int index = svrDef.DefItems.FindIndex(d => d.DefectCd == fileDef.DefectCd && d.CauseCd == fileDef.CauseCd && d.ClassCd == fileDef.ClassCd);
				if (index == -1)
				{
					svrDef.DefItems.Add(fileDef);
				}
				else
				{
					svrDef.DefItems[index].DefectCt += fileDef.DefectCt;
				}
			}

            if (Config.Settings.AutoInspectionSkipErrorDefItem != null && Config.Settings.AutoInspectionSkipErrorDefItem.ProcNo == procNo && autoInspectionSkippedFg == false)
            {
                AsmLot asmlot = AsmLot.GetAsmLot(lotNo);
                if (asmlot.IsFullSizeInspection == false && Config.Settings.IsMappingMode == true)
                {
                    //マッピングフラグONで且つ全数フラグOFFの時はバッドマーク計上しないので対象外
                }
                else
                {
                    int beforeDefCt = GetDefectCountOfPassedProcess(asmlot, procNo);
                    if (beforeDefCt > 0)
                    {
                        //バッドマーク処理をしていない＆バッドマーク処理工程である＆バッドマーク計上対象外でない＆
                        //前工程不良が1個以上ある場合は流動規制を掛ける。
                        setRistrictToInspectionSkipError(lotNo, Process.GetNextProcess(procNo, asmlot).ProcNo, beforeDefCt * -1, out errMessage);
                    }
                }
            }


			svrDef.DeleteInsert();
		}

		/// <summary>
		/// 指定作業までに登録されている不良数を取得
		/// (※指定作業の不良は含まない)
		/// </summary>
		/// <returns></returns>
		public static int GetDefectCountOfPassedProcess(AsmLot lot, int procNo, string defCd) 
		{
			int retv = 0;

			Process proc = Process.GetProcess(procNo);
			if (proc == null) throw new ApplicationException("作業情報が見つかりません:" + procNo);

			Process[] workflows = Process.GetWorkFlow(lot.TypeCd);
            //workflows = workflows.Where(w => w.WorkOrder < proc.WorkOrder).ToArray(); procのWorkOrderはGetProcess()では取得出来ないので修正 2014/10/21 n.yoshimoto

            workflows = workflows.OrderBy(w => w.WorkOrder).ToArray();

			foreach (Process w in workflows)
			{
                if (w.ProcNo == proc.ProcNo)
                {
                    break;
                }

				Defect d = Defect.GetDefect(lot.NascaLotNo, w.ProcNo);

				if (string.IsNullOrEmpty(defCd) == false)
				{
					d.DefItems = d.DefItems.Where(di => di.DefectCd == defCd).ToList();
				}

				retv += d.DefItems.Sum(s => s.DefectCt);
			}

			return retv;
		}
		/// <summary>
		/// 指定作業までに登録されている不良数を取得
		/// (※指定作業の不良は含まない)
		/// </summary>
		/// <returns></returns>
		public static int GetDefectCountOfPassedProcess(AsmLot lot, int procNo)
		{
			return GetDefectCountOfPassedProcess(lot, procNo, null);
		}

		/// <summary>
		/// 部材交換免責(立ち上げ免責含む)の不良数取得
		/// </summary>
		/// <param name="lotNo"></param>
		/// <param name="procNo"></param>
		/// <returns></returns>
		public static int GetMaterialChangeCt(string lotNo, int procNo)
		{
			int retv = 0;
			foreach (string defCd in Config.Settings.MaterialChangeDefectCode)
			{
				Defect d = GetDefect(lotNo, procNo);
				foreach (DefItem item in d.DefItems)
				{
					if (item.DefectCd != defCd) continue;

					retv += item.DefectCt;
				}
			}

			return retv;
		}

		private static bool hasInspectionSkipError(int procNo, string defectCd, string causeCd, string classCd) 
		{
			if (Config.Settings.AutoInspectionSkipErrorDefItem.ProcNo == procNo && Config.Settings.AutoInspectionSkipErrorDefItem.DefectCd == defectCd
				&& Config.Settings.AutoInspectionSkipErrorDefItem.CauseCd == causeCd && Config.Settings.AutoInspectionSkipErrorDefItem.ClassCd == classCd)
			{
				return true;
			}
			else 
			{
				return false;
			}
		}

		private static void setRistrictToInspectionSkipError(string lotNo, int procNo, int skipCt, out string errMessage)
		{
			Process proc = Process.GetProcess(procNo);

			string message = string.Format("[流動規制設定] ロットNO:{0} 工程名:{1} 規制理由:{2} SKIP:{3}",
				lotNo, proc.InlineProNM, "検査機のマーキングNGが前工程不良合計より少なくなっています。", skipCt);
            
			Restrict r = new Restrict();
			r.LotNo = lotNo;
			r.ProcNo = procNo;
			r.Reason = message;
			r.DelFg = false;
			r.Save();

			Log.SysLog.Info(message);
            errMessage = message;
        }


        /// <summary>
        /// 不良データ取り込み済みかどうか
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        private static bool isDefectAutoImportEnd(string lotno, int procno)
        {
            //マガジン分割対応
            lotno = Order.MagLotToNascaLot(lotno);

            List<DefItem> items = new List<DefItem>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno ?? "";
                cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno;

                try
                {
                    con.Open();

                    //新規Insert
                    cmd.CommandText = @"
                        SELECT 
                          causecd , 
                          classcd , 
                          defectcd , 
                          defectct 
                        FROM 
                          tndefect 
                        WHERE 
                          lotno = @LOTNO 
                          AND procno = @PROCNO";

                    //FJH ADD START
                    cmd.Parameters.Add("@CLASSCD", SqlDbType.Char).Value = ArmsApi.Config.SUBST_DEFECT_CLASSCD;
                    cmd.CommandText += " AND classcd <> @CLASSCD";
                    //FJH ADD END

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DefItem item = new DefItem();
                            item.CauseCd = SQLite.ParseString(reader["causecd"]);
                            item.ClassCd = SQLite.ParseString(reader["classcd"]);
                            item.DefectCd = SQLite.ParseString(reader["defectcd"]);
                            item.DefectCt = SQLite.ParseInt(reader["defectct"]);

                            items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException("不良情報取得エラー:" + lotno, ex);
                }
            }

            if (items.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public static DefItem GetErrConv(string plantCd, string typecd, string workcd, string errno)
        {
            DefItem def = new DefItem();

            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT NascaErr_NO, DefCause_CD, DefClass_CD
								FROM TmErrConv WITH(nolock)
								WHERE 1=1 ";

                if (!string.IsNullOrWhiteSpace(plantCd))
                {
                    sql += " AND (Equipment_NO = @Equipment_NO) ";
                    cmd.Parameters.Add("@Equipment_NO", System.Data.SqlDbType.NVarChar).Value = plantCd;
                }

                if (!string.IsNullOrWhiteSpace(errno))
                {
                    sql += " AND (EquiErr_NO = @EquiErr_NO) ";
                    cmd.Parameters.Add("@EquiErr_NO", System.Data.SqlDbType.NVarChar).Value = errno;
                }

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        def.DefectCd = rd["NascaErr_NO"].ToString().Trim();
                        def.CauseCd = rd["DefCause_CD"].ToString().Trim();
                        def.ClassCd = rd["DefClass_CD"].ToString().Trim();
                    }
                }
            }

            return def;
        }

        public static void GetDefectNm(string typecd, string workcd, DefItem def, ref string nascaErrNm, ref string defCauseNm, ref string defClassNm)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT itemnm, causenm, classnm
                            FROM TmDefect
                            WHERE (typecd = @TypeCD) 
                            AND (workcd = @WorkCD) 
                            AND (itemcd = @ItemCD) 
                            AND (causecd = @CauseCD) 
                            AND (classcd = @ClassCD) ";

                cmd.Parameters.Add("@TypeCD", SqlDbType.NVarChar).Value = typecd;
                cmd.Parameters.Add("@WorkCD", SqlDbType.NVarChar).Value = workcd;
                cmd.Parameters.Add("@ItemCD", SqlDbType.NVarChar).Value = def.DefectCd;
                cmd.Parameters.Add("@CauseCD", SqlDbType.NVarChar).Value = def.CauseCd;
                cmd.Parameters.Add("@ClassCD", SqlDbType.NVarChar).Value = def.ClassCd;

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        nascaErrNm = rd["itemnm"].ToString().Trim();
                        defCauseNm = rd["causenm"].ToString().Trim();
                        defClassNm = rd["classnm"].ToString().Trim();
                    }
                }
            }
        }

        #region FJH ADD
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="typecd"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        public static DefItem[] GetAllDefectSubSt(string lotno, string typecd, int procno)
        {
            //マガジン分割対応
            lotno = Order.MagLotToNascaLot(lotno);

            Defect record = GetDefectSubSt(lotno, procno);

            DefItem[] master = GetDefectMaster(typecd, procno, "SUBST");

            foreach (DefItem d in record.DefItems)
            {
                bool found = false;
                foreach (DefItem m in master)
                {
                    if (d.CauseCd == m.CauseCd && d.ClassCd == m.ClassCd && d.DefectCd == m.DefectCd)
                    {
                        m.DefectCt = d.DefectCt;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new ArmsException("不良マスタに存在しません:" + d.DefectCd);
                }
            }
            return master;
        }

        /// <summary>
        /// 指定ロット、工程の全不良取得
        /// 不良数は枚数で算出
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        public static Defect GetDefectSubSt(string lotno, int? procno)
        {
            //マガジン分割対応
            lotno = Order.MagLotToNascaLot(lotno);

            List<DefItem> items = new List<DefItem>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno ?? "";
                cmd.Parameters.Add("@CLASSCD", SqlDbType.Char).Value = ArmsApi.Config.SUBST_DEFECT_CLASSCD;

                try
                {
                    con.Open();

                    //新規Select
                    cmd.CommandText = @"
                        SELECT d.procno
                             , d.causecd
                             , d.classcd
                             , d.defectcd
                             , CASE
                                 WHEN l.limitsheartestfg IS NULL OR
                                      l.limitsheartestfg = 0 THEN d.defectct
                                 ELSE d.defectct / l.limitsheartestfg
                               END AS defectct 
                          FROM tndefect d INNER JOIN tnlot l ON
                               d.lotno   = l.lotno
                         WHERE d.lotno   = @LOTNO
                           AND d.classcd = @CLASSCD";

                    if (procno.HasValue)
                    {
                        cmd.CommandText += " AND d.procno = @PROCNO";
                        cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno.Value;
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DefItem item = new DefItem();
                            item.ProcNo = SQLite.ParseInt(reader["procno"]);
                            item.CauseCd = SQLite.ParseString(reader["causecd"]);
                            item.ClassCd = SQLite.ParseString(reader["classcd"]);
                            item.DefectCd = SQLite.ParseString(reader["defectcd"]);
                            item.DefectCt = SQLite.ParseInt(reader["defectct"]);

                            items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException("不良情報取得エラー:" + lotno, ex);
                }
            }

            Defect retv = new Defect();
            retv.LotNo = lotno;
            if (procno.HasValue)
            {
                retv.ProcNo = procno.Value;
            }
            retv.DefItems = items;
            return retv;
        }

        /// <summary>
        /// 不良枚数登録
        /// </summary>
        /// <param name="frameqty"></param>
        public void DeleteInsertSubSt(int? frameqty)
        {
            DeleteInsertSubSt(SQLite.ConStr, frameqty);
        }

        /// <summary>
        /// 入力不良枚数チェック
        /// </summary>
        /// <param name="frameqty"></param>
        /// <returns></returns>
        public bool CheckDefectSubSt(int? frameqty)
        {
            return CheckDefectSubSt(SQLite.ConStr, frameqty);
        }

        /// <summary>
        /// 入力不良枚数チェック 
        /// </summary>
        /// <param name="constr"></param>
        /// <param name="frameqty"></param>
        /// <returns></returns>
        public bool CheckDefectSubSt(string constr, int? frameqty)
        {
            int iFrameQty = 0;
            int iDefectCt = 0;

            try
            {
                if (frameqty.HasValue)
                {
                    Log.SysLog.Info("不良枚数チェック" + this.LotNo);

                    iFrameQty = (int)frameqty;
                }
                else
                {
                    //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
                    Log.SysLog.Info("不良枚数チェック" + this.LotNo + " : " + this.MagazineNo);
                    using (SqlConnection con = new SqlConnection(constr))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();

                        cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo ?? "";
                        cmd.Parameters.Add("@MAGAZINENO", SqlDbType.NVarChar).Value = this.MagazineNo;
                        cmd.CommandText = @"
                            SELECT frameqty
                                FROM TnMag
                                WHERE lotno  = @LOTNO
                                AND magno  = @MAGAZINENO";
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                iFrameQty = SQLite.ParseInt(reader["frameqty"]);
                            }
                        }
                    }
                }
                IEnumerable<string> defectNotIncludeList = GnlMaster.GetDefectNotInclude()
                    .Select(d => d.Code);
                foreach (DefItem d in this.DefItems)
                {
                    // 不良数0は対象外
                    if (d.DefectCt == 0) continue;
                    // 計上済前工程不良は登録しない
                    if (defectNotIncludeList.Contains(d.DefectCd)) continue;
                    // 不良枚数加算
                    iDefectCt += d.DefectCt;
                }

                //加算した枚数＞TnMag.frameqtyの場合は、エラーとする
                if (iDefectCt > iFrameQty)
                {
                    return false;
                    throw new ArmsException("不良枚数チェックエラー");
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw new ArmsException("不良枚数チェックエラー", ex);
            }
        }

        /// <summary>
        /// 不良枚数取得
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        public int GetDefectCtSubSt(string lotno, int procno)
        {
            return GetDefectCtSubSt(lotno, procno, SQLite.ConStr);
        }

        /// <summary>
        /// 不良枚数取得
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <param name="constr"></param>
        /// <returns></returns>
        public int GetDefectCtSubSt(string lotno, int procno, string constr)
        {
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno ?? "";
                    cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = procno;
                    cmd.Parameters.Add("@CLASSCD", SqlDbType.Char).Value = ArmsApi.Config.SUBST_DEFECT_CLASSCD;

                    //
                    cmd.CommandText = @"
                        SELECT CASE
                                 WHEN b.limitsheartestfg IS NULL OR
                                      b.limitsheartestfg = 0 THEN SUM(a.defectct)
                                 ELSE SUM(a.defectct) / b.limitsheartestfg
                               END AS defectct 
                          FROM TnDefect a INNER JOIN TnLot b ON
                               a.lotno = b.lotno
                         GROUP BY a.lotno
                                , a.procno
                                , a.classcd
                                , a.delfg
                                , b.limitsheartestfg
                        HAVING a.lotno   = @LOTNO
                           AND a.procno  = @PROCNO
                           AND a.classcd = @CLASSCD
                           AND a.delfg   = 0";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                return SQLite.ParseInt(reader["defectct"]);
                            }
                        }
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    return 0;
                    throw new ArmsException("不良枚数取得エラー", ex);
                }
            }
        }

        /// <summary>
        /// 不良枚数登録
        /// </summary>
        /// <param name="constr"></param>
        /// <param name="iFrameQty"></param>
        public void DeleteInsertSubSt(string constr, int? iFrameQty)
        {
            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            Log.SysLog.Info("不良情報更新" + this.LotNo);

            // データベース登録前にマスタに存在するかの確認
            #region 登録対象の不良項目のマスタ有無チェック
            AsmLot lot = AsmLot.GetAsmLot(this.LotNo);
            if (lot != null)
            {
                DefItem[] master = GetDefectMaster(lot.TypeCd, this.ProcNo, "SUBST");
                foreach (DefItem d in this.DefItems)
                {
                    List<DefItem> foundList = master.Where(m => m.CauseCd == d.CauseCd && m.ClassCd == d.ClassCd && m.DefectCd == d.DefectCd).ToList();
                    if (foundList.Count == 0)
                    {
                        throw new ArmsException("不良マスタに存在しません: " + lot.TypeCd + "- (" + d.DefectCd + ", " + d.CauseCd + ", " + d.ClassCd + ")");
                    }
                }
            }
            #endregion

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo ?? "";
                    cmd.Parameters.Add("@PROC", SqlDbType.BigInt).Value = this.ProcNo;
                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@MAGAZINENO", SqlDbType.NVarChar).Value = this.MagazineNo;
                    cmd.Parameters.Add("@CLASSCD1", SqlDbType.NVarChar).Value = ArmsApi.Config.SUBST_DEFECT_CLASSCD;

                    //前履歴は削除
                    cmd.CommandText = @"DELETE FROM TnDefect
                                         WHERE lotno   = @LOTNO
                                           AND procno  = @PROC
                                           AND classcd = @CLASSCD1";
                    cmd.ExecuteNonQuery();

                    SqlParameter prmCauseCd = cmd.Parameters.Add("@CAUSECD", SqlDbType.NVarChar);
                    SqlParameter prmClassCd = cmd.Parameters.Add("@CLASSCD", SqlDbType.NVarChar);
                    SqlParameter prmDefectCd = cmd.Parameters.Add("@DEFECTCD", SqlDbType.NVarChar);
                    SqlParameter prmDefectCt = cmd.Parameters.Add("@DEFECTCT", SqlDbType.BigInt);

                    //新規Insert
                    cmd.CommandText = @"
                            INSERT INTO TnDefect(
                                   lotno
	                             , procno
	                             , causecd
	                             , classcd
	                             , defectcd
	                             , defectct
	                             , lastupddt
                                 , delfg)
                            SELECT @LOTNO
	                             , @PROC
	                             , @CAUSECD
	                             , @CLASSCD
	                             , @DEFECTCD
                                 , CASE
                                     WHEN l.limitsheartestfg IS NULL OR
                                          l.limitsheartestfg = 0 THEN @DEFECTCT
                                     ELSE @DEFECTCT * l.limitsheartestfg
                                   END 
	                             , @UPDDT
                                 , 0
                              FROM tnlot l
                             WHERE l.lotno = @LOTNO";
                    IEnumerable<string> defectNotIncludeList = GnlMaster.GetDefectNotInclude()
                        .Select(d => d.Code);

                    int defectct = 0;
                    foreach (DefItem d in this.DefItems)
                    {
                        // 不良数0は対象外
                        if (d.DefectCt == 0) continue;

                        // 計上済前工程不良は登録しない
                        if (defectNotIncludeList.Contains(d.DefectCd)) continue;

                        prmCauseCd.Value = d.CauseCd ?? "";
                        prmClassCd.Value = d.ClassCd ?? "";
                        prmDefectCd.Value = d.DefectCd ?? "";
                        prmDefectCt.Value = d.DefectCt;
                        defectct += d.DefectCt;

                        cmd.ExecuteNonQuery();
                    }
                    if (iFrameQty.HasValue)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo ?? "";
                        cmd.Parameters.Add("@FRAMEQTY", SqlDbType.Int).Value = iFrameQty - defectct;
                        cmd.CommandText = @"UPDATE TnMag
                                               SET frameqty = @FRAMEQTY
                                             WHERE lotno    = @LOTNO
                                               AND newfg    = 1";
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("不良情報更新エラー", ex);
                }
            }

            //ライン間受渡しの場合は処理を行わない
            if (constr == SQLite.ConStr)
            {
                if (Config.GetLineType != Config.LineTypes.NEL_GAM && Config.GetLineType != Config.LineTypes.MEL_GAM)
                {
                    KHLTestCheckAndUpdateLot(this.LotNo, this.ProcNo, this.DefItems);
                }

                //特定工程・不良発生時の抜き取り検査フラグ追加判定
                JudgeDefIsp();

                //QCIL更新
                lot = AsmLot.GetAsmLot(this.LotNo);
                if (lot != null)
                {
                    DefItem[] updateList = GetAllDefect(lot, this.ProcNo);

                    List<DefItem> displayList = this.DefItems.Where(d => d.IsDisplayedEICS).ToList();

                    UpdateEICS(lot, this.ProcNo, updateList, displayList);
                }
            }
        }
        #endregion
    }
}
