using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ArmsApi;
using ArmsApi.Model;
using static ArmsApi.Model.Material;

//#if DEBUG
//using ArmsApi.local.nichia.naweb_dev;
//#else
using ArmsApi.local.nichia.naweb;
//#endif

namespace ArmsNascaBridge3
{
  
    public class lotCharDataList //LTTSの複数列を情報をまとめて格納するためのクラス
    {
        /// <summary>
        /// ロット特性値
        /// </summary>
        public string LotCharVal { get; set; }

        /// <summary>
        /// 最終更新日
        /// </summary>
        public DateTime Lastupddt { get; set; }
    }

    public class Material
    {
        public string MaterialCd { get; set; }

        /// <summary>
        /// ロット
        /// </summary>
        public string LotNo { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string MaterialNm { get; private set; }

        /// <summary>
        /// 品目グループコード
        /// </summary>
        public string Hmgp { get; set; }

        /// <summary>
        /// 使用期限
        /// </summary>
        public DateTime LimitDt { get; set; }

        /// <summary>
        /// 保管場所コード
        /// </summary>
        public string BinCd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastUpddt { get; set; }

		public string DiceWaferKb { get; set; }

		public string TypeCd { get; set; }

        public decimal StockCt { get; set; }

        public DateTime StockLastUpdDt { get; set; }

        public static void Import()
        {
			try
			{
				DateTime updfrom = DateTime.Now.AddMonths(-2);

				string[] ggcodes = getNascaGGCodeList(updfrom);

				int i = 1;
				foreach (string ggcode in ggcodes)
				{
					Console.WriteLine(DateTime.Now.ToString() + ":" + i.ToString() + "/" + ggcodes.Length.ToString() + " start " + ggcode);
					Material[] matlist = getNascaMaterials(ggcode, updfrom);

					string hmgp = GetHMGP(ggcode);

					foreach (Material mat in matlist)
					{
                        try
						{
							lotCharDataList blendCdData = new lotCharDataList();
                            string waferSupplyId = null;
                            List<string> waferWorkCdList = null;
                            float? waferThicknessAve = null;
                            string ringId = null;

							bool badmarkframeFG = IsBadMarkFrame(mat.MaterialCd, mat.LotNo);
                            bool waferFG = GetWaferFG(mat.MaterialCd);

							if (waferFG)
							{
								waferSupplyId = GetWaferSupplyId(mat.LotNo, mat.MaterialCd);

								if (IsInHouseDice(mat.DiceWaferKb))
								{
									//社内チップはブレンドコード有が取り込み対象

									blendCdData = GetBlendCdForWafer(mat.MaterialCd, mat.LotNo, null);

									if (blendCdData.LotCharVal == null || blendCdData.Lastupddt == null)
									{
										continue;
									}
									else
									{
                                        if (mat.LastUpddt <= blendCdData.Lastupddt)
										    mat.LastUpddt = blendCdData.Lastupddt;
									}

									if (blendCdData.LotCharVal != null && waferSupplyId != null)
									{
										waferWorkCdList = GetBlendWork(mat.MaterialCd, blendCdData.LotCharVal, waferSupplyId);
									}

                                    //厚み平均値※必要なラインのみ　2016/12/2 湯浅
                                    if (Config.Settings.ImportMatThickness)
                                    {
                                        waferThicknessAve = ArmsApi.Model.NASCA.Importer.GetThicknessAve(mat.LotNo);
                                    }
                                }
								else
								{
									//社外チップ、チップ加工品は無条件で取り込み
								}

                                if (Config.Settings.ImportMatRingId)
                                {
                                    // 19ラインウェハーリングID　※必要なラインのみ
                                    ringId = ArmsApi.Model.NASCA.Importer.GetRingID(mat.LotNo);
                                }

                                BinLot binLot = ArmsApi.Model.Material.GetStockCount(mat.MaterialCd, mat.LotNo);
                                if (binLot != null)
                                {
                                    mat.StockCt = binLot.StockCt;
                                    mat.StockLastUpdDt = binLot.StockLastUpdDt;
                                }

                                updateSQLite(mat, hmgp, true, badmarkframeFG, blendCdData.LotCharVal, waferWorkCdList, waferSupplyId, waferThicknessAve, ringId);
							}
						}
						catch (Exception ex)
						{
							Log.SysLog.Info("[ArmsNascaBridge3] Wafer Error:" + ex.ToString());
						}
					}
					Console.WriteLine(DateTime.Now.ToString() + ":" + i.ToString() + "/" + ggcodes.Length.ToString() + " end");
					i++;
				}
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge3] Wafer Error:" + err.ToString());
			}
        }

		private static void updateSQLite(Material mat, string hmgp, bool iswafer, bool isbadmarkframe, string moldedNm, string moldedClass, List<string> moldedDateList, string waferBlendCd, List<string> waferWorkCdList, string waferSupplyId, float? waferThicknessAve, string ringId)
        {
			bool isframe = false;

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar).Value = mat.MaterialCd;
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = mat.LotNo;
                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@LIMITDT", SqlDbType.DateTime).Value = mat.LimitDt;
                cmd.Parameters.Add("@BINCD", SqlDbType.NVarChar).Value = mat.BinCd;
                cmd.Parameters.Add("@MATNM", SqlDbType.NVarChar).Value = mat.MaterialNm;
                cmd.Parameters.Add("@HMGP", SqlDbType.NVarChar).Value = hmgp;
                cmd.Parameters.Add("@ISFRAME", SqlDbType.Int).Value = SQLite.SerializeBool(isframe);
                cmd.Parameters.Add("@ISWAFER", SqlDbType.Int).Value = SQLite.SerializeBool(iswafer);
                cmd.Parameters.Add("@ISBADMARKFRAME", SqlDbType.Int).Value = SQLite.SerializeBool(isbadmarkframe);
				cmd.Parameters.Add("@MOLDEDNM", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(moldedNm);
				cmd.Parameters.Add("@MOLDEDCLASS", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(moldedClass);
				if (moldedDateList == null || moldedDateList.Count == 0)
				{
					cmd.Parameters.Add("@MOLDEDDATE", SqlDbType.NVarChar).Value = System.DBNull.Value;
				}
				else
				{
					cmd.Parameters.Add("@MOLDEDDATE", SqlDbType.NVarChar).Value = string.Join(",", moldedDateList);
				}

                //2015.7.13 NASCAからのウェハデータ吸い上げ機能追加

                cmd.Parameters.Add("@BLENDCD", SqlDbType.NVarChar).Value = waferBlendCd ?? string.Empty;

                if (waferWorkCdList == null || waferWorkCdList.Count == 0)
				{
                    cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = string.Empty;
				}
				else
				{
                    cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = string.Join(",", waferWorkCdList);
				}

                if (waferThicknessAve.HasValue)
                {
                    cmd.Parameters.Add("@THICKNESSAVE", SqlDbType.Real).Value = waferThicknessAve;
                }
                else
                {
                    cmd.Parameters.Add("@THICKNESSAVE", SqlDbType.NVarChar).Value = System.DBNull.Value;
                }
                
                cmd.Parameters.Add("@SUPPLYID", SqlDbType.NVarChar).Value = waferSupplyId ?? string.Empty;
				cmd.Parameters.Add("@DICEWAFERKB", SqlDbType.NVarChar).Value = mat.DiceWaferKb ?? string.Empty;

                cmd.Parameters.Add("@STOCKCT", SqlDbType.Decimal).Value = mat.StockCt;

                cmd.Parameters.Add("@RINGID", SqlDbType.NVarChar).Value = ringId ?? string.Empty;

                cmd.CommandText = @"
                    SELECT lastupddt FROM TnMaterials
                    WHERE materialcd=@MATCD AND lotno=@LOTNO";

                object objlastupd = cmd.ExecuteScalar();

                if (objlastupd == null)
                {
                    if (Config.Settings.ImportMatRingId && string.IsNullOrEmpty(ringId) == false)
                    {
                        cmd.CommandText = @" UPDATE TnMaterials SET ringid = NULL WHERE ringid = @RINGID ";
                        cmd.ExecuteNonQuery();
                    }

					cmd.CommandText = @"
                        INSERT INTO TnMaterials(materialcd, lotno, materialnm, limitdt, bincd, hmgroup, iswafer, isframe, delfg, lastupddt, 
                                                isbadmarkframe, framemoldednm, framemoldedclass, framemoldeddt, blendcd, workcd, supplyId, 
                                                dicewaferkb, thicknessave, stockct, ringid)
                        VALUES (@MATCD, @LOTNO, @MATNM, @LIMITDT, @BINCD, @HMGP, @ISWAFER, @ISFRAME,  0, @UPDDT, @ISBADMARKFRAME, @MOLDEDNM, 
                                @MOLDEDCLASS, @MOLDEDDATE, @BLENDCD, @WORKCD, @SUPPLYID, @DICEWAFERKB, @THICKNESSAVE, @STOCKCT, @RINGID);";

                    cmd.ExecuteNonQuery();
                    return;
                }
                else
                {
					DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
					if (mat.LastUpddt > current)
					{
						cmd.CommandText = @"
                            UPDATE TnMaterials SET limitdt=@LIMITDT, bincd=@BINCD, hmgroup=@HMGP, iswafer=@ISWAFER, isframe=@ISFRAME, 
                            isbadmarkframe = @ISBADMARKFRAME, lastupddt=@UPDDT, framemoldednm = @MOLDEDNM, framemoldedclass = @MOLDEDCLASS, 
                            framemoldeddt = @MOLDEDDATE, blendcd=@BLENDCD, workcd = @WORKCD, supplyId = @SUPPLYID, dicewaferkb = @DICEWAFERKB, 
                            thicknessave = @THICKNESSAVE, ringid = @RINGID
                            WHERE materialcd=@MATCD AND lotno=@LOTNO";

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void updateSQLite(Material mat, string hmgp, bool iswafer, bool isbadmarkframe, string moldedNm, string moldedClass, List<string> moldedDateList)
        {
            updateSQLite(mat, hmgp, iswafer, isbadmarkframe, moldedNm, moldedClass, moldedDateList, null, null, null, null, null);
        }

		private static void updateSQLite(Material mat, string hmgp, bool iswafer, bool isbadmarkframe)
		{
			updateSQLite(mat, hmgp, iswafer, isbadmarkframe, null, null, null, null, null, null, null, null);
		}

		private static void updateSQLite(Material mat, string hmgp, bool iswafer, bool isbadmarkframe, string waferBlendCd, List<string> waferWorkCdList, string waferSupplyId)
        {
            updateSQLite(mat, hmgp, iswafer, isbadmarkframe, null, null, null, waferBlendCd, waferWorkCdList, waferSupplyId, null, null);
        }

        private static void updateSQLite(Material mat, string hmgp, bool iswafer, bool isbadmarkframe, string waferBlendCd, List<string> waferWorkCdList, string waferSupplyId, float? waferThicknessAve)
        {
            updateSQLite(mat, hmgp, iswafer, isbadmarkframe, null, null, null, waferBlendCd, waferWorkCdList, waferSupplyId, waferThicknessAve, null);
        }

        private static void updateSQLite(Material mat, string hmgp, bool iswafer, bool isbadmarkframe, string waferBlendCd, List<string> waferWorkCdList, string waferSupplyId, float? waferThicknessAve, string ringId)
        {
            updateSQLite(mat, hmgp, iswafer, isbadmarkframe, null, null, null, waferBlendCd, waferWorkCdList, waferSupplyId, waferThicknessAve, ringId);
        }

        private static string GetHMGP(string ggcode)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {

                con.Open();
                cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = ggcode;

                cmd.CommandText = @"
                    select
                     mategroup_cd
                    from ntmhmgk
                    where del_fg=0 AND material_cd = @MATCD";

                object o = cmd.ExecuteScalar() ?? string.Empty;
                return o.ToString().Trim();
            }
        }


        /// <summary>
        /// ウェハー品目であるかどうかを調べる
        /// </summary>
        /// <param name="materialcd"></param>
        /// <param name="lotno"></param>
        private static bool GetWaferFG(string materialCd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                bool retv = false;
                con.Open();
                cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = materialCd;

                cmd.CommandText = @"
                  SELECT dicewafer_kb
                  FROM   dbo.RvmMAT WITH (NOLOCK)
                  WHERE (material_cd = @MATCD) AND (del_fg = '0')
                  OPTION (MAXDOP 1)";

                object o = cmd.ExecuteScalar() ?? string.Empty;
                string s = o.ToString();
				//2=社内チップ, 8=社外チップ, 16=チップ加工品
                if (s == "2" || s == "8" || s == "16")
                {
                    retv = true;
                }
                
                return retv;
            }
        }

		/// <summary>
		/// リードフレームかどうか
		/// </summary>
		/// <returns></returns>
		private static bool IsFrame(Material mat) 
		{
			if (mat.Hmgp == "MATE001")
			{
				return true;
			}
			else 
			{
				return false;
			}
		}

		/// <summary>
		/// 社内チップか
		/// </summary>
		/// <param name="diceWaferKb"></param>
		/// <returns></returns>
		private static bool IsInHouseDice(string diceWaferKb) 
		{
			if (diceWaferKb == "2")
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        /// <summary>
        /// ブレンドコード上の作業CDをチェックし、存在すれば取り込む
        /// </summary>
        /// <param name="materialcd"></param>
        /// <param name="lotno"></param>
        private static List<string> GetBlendWork(string materialCd, string blendCd, string supplyId)
        {
            List<string> retv = new List<string>();
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = materialCd;
                cmd.Parameters.Add("@BLENDCD", SqlDbType.Char).Value = blendCd;
                cmd.Parameters.Add("@SUPPLYID", SqlDbType.Char).Value = supplyId;

                cmd.CommandText = @"
                  SELECT DISTINCT Work_CD
                  FROM   dbo.NtmBDSS WITH (NOLOCK) 
                  WHERE  (Blend_CD = @BLENDCD) AND (Del_FG = 0) AND (WfrMate_CD = @MATCD) AND (Supply_ID = @SUPPLYID)
                  OPTION (MAXDOP 1)";
                    

                using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
							retv.Add(SQLite.ParseString(reader["Work_CD"]).Trim());
						}
                    }

                return retv;
            }
        }

#region GGコード取得
        

        public static string[] getNascaGGCodeList(DateTime from)
        {
            List<string> retv = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {

                con.Open();

                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = from;
                cmd.Parameters.Add("@BINJA", SqlDbType.VarChar).Value = Config.Settings.MaterialsImportBinJaLike;
                cmd.Parameters.Add("@SECTIONCD", SqlDbType.VarChar).Value = Config.Settings.SectionCd + "%";

                cmd.CommandText = @"
                        select
                         distinct(t.material_cd)
                        from rvtbinlot(nolock) t
                        where t.lastupd_dt >= @UPDDT
                        and exists (
	                        select
	                         *
	                        from rvmbin(nolock)
	                        where rvmbin.bin_cd = t.bin_cd
	                        and bin_cd LIKE @SECTIONCD
	                        and (bin_ja like @BINJA or bin_ja like '%自動化%'))
                     and exists(
	                    select * from rvmmat m
                        where m.material_cd = t.material_cd and (dicewafer_kb in (2, 8, 16)))   
                        OPTION (MAXDOP 1)";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(rd[0].ToString().Trim());
                    }
                }
            }

            return retv.ToArray();
        }

#endregion


        public static Material[] getNascaMaterials(string ggcode, DateTime from)
        {
            List<Material> retv = new List<Material>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = ggcode;
                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = from;
                cmd.Parameters.Add("@BINJA", SqlDbType.VarChar).Value = Config.Settings.MaterialsImportBinJaLike;
                cmd.Parameters.Add("@SECTIONCD", SqlDbType.VarChar).Value = Config.Settings.SectionCd + "%";

                //A-受入を除くA-xxxxから原材料取得
                cmd.CommandText = @"
                        select
                         t.material_cd
                        , t.lot_no
                        , m.material_ja
                        , l.qultymnte_dt
                        , t.bin_cd
                        , t.lastupd_dt
						, dicewafer_kb
						, mtralbase_cd
                        from rvtbinlot(nolock) t
                        INNER JOIN rvmMat(nolock) m
                        ON m.material_cd = t.material_cd
                        INNER JOIN rvtlot(nolock) l
                        ON l.material_cd = t.material_cd
                        and l.lot_no = t.lot_no
						INNER JOIN dbo.RvmMCONV ON m.material_cd = dbo.RvmMCONV.material_cd
                        where  t.lastupd_dt >= @UPDDT
                        and t.material_cd = @MATCD
                        and l.material_cd = @MATCD
                        and l.material_cd = @MATCD
                        and exists (
	                        select
	                         *
	                        from rvmbin(nolock)
	                        where rvmbin.bin_cd = t.bin_cd
	                        and bin_cd LIKE @SECTIONCD
	                        and (bin_ja like @BINJA or bin_ja like '%自動化%')  and bin_cd <> '5306')
                        and t.del_fg = 0
                        and m.del_fg = 0
                        and l.del_Fg = 0
						and dbo.RvmMCONV.del_fg = 0 option (maxdop 1)";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        Material m = new Material();
                        m.MaterialCd = rd["material_cd"].ToString().Trim();
                        m.LotNo = rd["lot_no"].ToString().Trim();
                        m.MaterialNm = rd["material_ja"].ToString().Trim();
                        m.LimitDt = Convert.ToDateTime(rd["qultymnte_dt"]);
                        m.BinCd = rd["bin_cd"].ToString().Trim();
                        m.LastUpddt = Convert.ToDateTime(rd["lastupd_dt"]);
						m.DiceWaferKb = rd["dicewafer_kb"].ToString().Trim();
						m.TypeCd = rd["mtralbase_cd"].ToString().Trim();
                        retv.Add(m);
                    }
                }
            }

            return retv.ToArray();
        }
        
        /// <summary>
        /// ロット特性 フレーム識別(M0000013)を保持しているか調べる
        /// </summary>
        /// <param name="materialcd"></param>
        /// <param name="lotno"></param>
        private static bool IsBadMarkFrame(string materialcd, string lotno, int? nascalotid)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                
                string sql = @" SELECT NttLTTS.CharVal_CD
                                     FROM dbo.NttLTNA AS NttLTNA WITH (nolock) 
                                     INNER JOIN dbo.NttLTTS AS NttLTTS WITH (nolock) ON NttLTNA.NascaLot_ID = NttLTTS.NascaLot_ID 
                                     INNER JOIN dbo.NttLTRS AS NttLTRS WITH (nolock) ON NttLTNA.NascaLot_ID = NttLTRS.NascaLot_ID
                                     WHERE (NttLTTS.LotChar_CD = 'M0000013') 
                                        AND (NttLTTS.CharVal_CD = '23') ";
                if (!string.IsNullOrEmpty(materialcd) && !string.IsNullOrEmpty(lotno))
                {
                    sql += " AND (NttLTRS.Material_CD = @MATCD) AND (NttLTRS.RootsLot_NO = @LOTNO) ";
                    cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = materialcd;
                    cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;
                }
                if (nascalotid.HasValue)
                {
                    sql += " AND (NttLTNA.nascalot_id = @LOTID) ";
                    cmd.Parameters.Add("@LOTID", SqlDbType.Int).Value = nascalotid.Value;
                }

                cmd.CommandText =sql;

                object val = cmd.ExecuteScalar();
                if (val == null || val == System.DBNull.Value)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public static bool IsBadMarkFrame(string materialcd, string lotno)
        {
            return IsBadMarkFrame(materialcd, lotno, null);
        }
        public static bool IsBadMarkFrame(int nascalotid)
        {
            return IsBadMarkFrame(string.Empty, string.Empty, nascalotid);
        }

		/// <summary>
		/// 成型金型取得
		/// </summary>
		/// <param name="lotNo"></param>
		/// <param name="TypeCd"></param>
		/// <returns></returns>
		private static string getMoldedName(string lotNo, string typeCd)
		{
			ArmsApi.Model.NASCA.NascaPubApi api = ArmsApi.Model.NASCA.NascaPubApi.GetInstance();
			NppLotCharInfo[] lotChars = api.SearchLotChar(lotNo, typeCd, "M0000009", false);
			if (lotChars == null || lotChars.Count() == 0)
			{
				return null;
			}
			else 
			{
				return lotChars.Single().LotCharValue;
			}
		}

		/// <summary>
		/// 成型区分取得
		/// </summary>
		/// <param name="lotNo"></param>
		/// <param name="typeCd"></param>
		/// <returns></returns>
		private static string getMoldedClass(string lotNo, string typeCd)
		{
			ArmsApi.Model.NASCA.NascaPubApi api = ArmsApi.Model.NASCA.NascaPubApi.GetInstance();
			NppLotCharInfo[] lotChars = api.SearchLotChar(lotNo, typeCd, "P0000202", false);
			if (lotChars == null || lotChars.Count() == 0)
			{
				return null;
			}
			else 
			{
				return lotChars.Single().CharValCD.Trim();
			}
		}

		/// <summary>
		/// 成型日取得
		/// </summary>
		/// <param name="lotNo"></param>
		/// <param name="typeCd"></param>
		/// <returns></returns>
		private static List<string> getMoldedDate(string lotNo, string typeCd)
		{
			List<string> retv = new List<string>();

			ArmsApi.Model.NASCA.NascaPubApi api = ArmsApi.Model.NASCA.NascaPubApi.GetInstance();
			NppLotCharInfo[] lotChars = api.SearchLotChar(lotNo, typeCd, "M0000116", false);
			if (lotChars != null && lotChars.Count() != 0)
			{
				foreach (NppLotCharInfo lotChar in lotChars)
				{
					DateTime date;
					if (DateTime.TryParse(lotChar.LotCharValue, out date))
					{
						retv.Add(date.ToString("yyyy/MM/dd"));
					}
				}
			}

			return retv;
		}

        /// <summary>
        /// ブレンドコード特性の取得　(※非API方式)
        /// </summary>
        /// <param name="materialcd"></param>
        /// <param name="lotno"></param>
        private static lotCharDataList GetBlendCdForWafer(string materialcd, string lotno, int? nascalotid)
        {
            lotCharDataList retv = new lotCharDataList();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT NttLTTS.LotChar_VAL, NttLTTS.LastUpd_DT
                                FROM dbo.NttLTRS AS NttLTRS WITH (nolock)
                                INNER JOIN dbo.NttLTTS AS NttLTTS WITH (nolock) ON NttLTRS.NascaLot_ID = NttLTTS.NascaLot_ID
                                WHERE (NttLTTS.LotChar_CD = 'P0000207') "; 

                if (!string.IsNullOrEmpty(materialcd) && !string.IsNullOrEmpty(lotno))
                {
                    sql += " AND (NttLTRS.Material_CD = @MATCD) AND (NttLTRS.RootsLot_NO = @LOTNO) ";
                    cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = materialcd;
                    cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;
                }
                if (nascalotid.HasValue)
                {
                    sql += " AND (NttLTNA.nascalot_id = @LOTID) ";
                    cmd.Parameters.Add("@LOTID", SqlDbType.Int).Value = nascalotid.Value;
                }

                sql += " OPTION (MAXDOP 1) ";

                cmd.CommandText = sql;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        retv.LotCharVal = reader["LotChar_VAL"].ToString().Trim();
                        retv.Lastupddt = Convert.ToDateTime(reader["LastUpd_DT"]);
					}
                }

                return retv;
                
            }
        }

        /// <summary>
        /// 供給ID特性の取得
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="typeCd"></param>
        /// <returns></returns>
        public static string GetWaferSupplyId(string lotNo, string materialCd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT NttLTTS.LotChar_VAL, NttLTTS.LastUpd_DT
                                FROM dbo.NttLTRS AS NttLTRS WITH (nolock)
                                INNER JOIN dbo.NttLTTS AS NttLTTS WITH (nolock) ON NttLTRS.NascaLot_ID = NttLTTS.NascaLot_ID
                                WHERE (NttLTTS.LotChar_CD = '21400097') ";

                if (!string.IsNullOrEmpty(materialCd) && !string.IsNullOrEmpty(lotNo))
                {
                    sql += " AND (NttLTRS.Material_CD = @MATCD) AND (NttLTRS.RootsLot_NO = @LOTNO) ";
                    cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = materialCd;
                    cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotNo;
                }
                sql += " OPTION (MAXDOP 1) ";

                cmd.CommandText = sql;

                object supplyId = cmd.ExecuteScalar();
                if (supplyId == null)
                    return null;
                else
                    return supplyId.ToString().Trim();
            }
        }
    }
}
