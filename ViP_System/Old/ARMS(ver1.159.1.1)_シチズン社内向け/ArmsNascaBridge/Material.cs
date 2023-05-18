using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ArmsApi;
using ArmsApi.Model;
#if DEBUG
using ArmsApi.local.nichia.naweb_dev;
#else
using ArmsApi.local.nichia.naweb;
#endif
using System.IO;

namespace ArmsNascaBridge
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

	public class MetallicMolding
	{
		public MetallicMolding() 
		{
			MoldedDateList = new List<string>();
		}

		public string LotNo { get; set; }
		public string LotCharCd { get; set; }
		public string MoldedName { get; set; }
		public string MoldedClass { get; set; }
		public List<string> MoldedDateList { get; set; }
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

        /// <summary>
        /// 在庫数
        /// </summary>
        public decimal StockCt { get; set; }

        /// <summary>
        /// 在庫数を更新した日時
        /// </summary>
        public DateTime? StockLastUpdDt { get; set; }

        /// <summary>
        /// 金型名特性CD
        /// </summary>
        public const string MOLDEDNAME_LOTCHARCD = "M0000009";

		/// <summary>
		/// 成形区分特性CD
		/// </summary>
		public const string MOLDEDCLASS_LOTCHARCD = "P0000202";

		/// <summary>
		/// 成形日特性CD
		/// </summary>
		public const string MOLDEDDATE_LOTCHARCD = "M0000116";

        /// <summary>
        /// 資材開封日時ファイルを取り込む対象期間(14日間分は取り込む　14日以前は対象外)
        /// </summary>
        public const int MATOPENFILE_TARGETDAY = 14;

		/// <summary>
		/// 資材の取り込み(ウェハ以外)
		/// </summary>
        public static void Import()
        {
			try
			{
                DateTime updfrom = DateTime.Now.AddMonths(-2);

                //資材(ウェハ以外)の品目CD一覧を取得
                string[] ggcodes = getNascaGGCodeList(updfrom);

                int i = 1;
                foreach (string ggcode in ggcodes)
                {
                    Console.WriteLine(DateTime.Now.ToString() + ":" + i.ToString() + "/" + ggcodes.Length.ToString() + " start " + ggcode);
                    Material[] matlist = getNascaMaterials(ggcode, updfrom);

                    string hmgp = GetHMGP(ggcode);
                    
//#if DEBUG
//                    if (hmgp != "MATE016") continue;
//#endif 

                    foreach (Material mat in matlist)
                    {
                        try
                        {
                            mat.Hmgp = hmgp;

                            bool badmarkframeFG = IsBadMarkFrame(mat.MaterialCd, mat.LotNo);
                            bool waferFG = GetWaferFG(mat.MaterialCd);

                            if (waferFG)
                            {
                                //ArmsNascaBridge3へ処理移動
                            }
                            else if (ArmsApi.Model.Material.IsCompareResinMatGr(mat.Hmgp) == true) //樹脂グループ取込対象品について特性を取り込む機能を追加。
                            {
                                string resinGroupCd = GetResinGroupCd(mat);

                                if (string.IsNullOrWhiteSpace(resinGroupCd))
                                {
                                    Log.SysLog.Info("[ArmsNascaBridge] Material Error:樹脂Gr照合対象品に樹脂グル―プが付与されてないか、複数付与されています。ロット：" + mat.LotNo);
                                    continue;
                                }

                                ArmsApi.Model.MatChar.InsertUpdate(mat.MaterialCd, mat.LotNo, ArmsApi.Config.RESINGROUP_LOTCHARCD, resinGroupCd, false, DateTime.Now);
                                updateSQLite(mat, hmgp, false, badmarkframeFG);
                            }
                            else if (ArmsApi.Model.Material.IsCompareGoldWireMatGr(mat.Hmgp) == true)
                            {
                                mat.StockCt = getGoldWireStockCtFromMaterialName(mat.MaterialNm);
                                updateSQLite(mat, hmgp, false, badmarkframeFG);
                            }
                            else
                            {
                                if (Config.Settings.IsFrameMoldedImport && IsFrame(mat))
                                {
                                    MetallicMolding m = getMetallicMoldingData(mat.LotNo, mat.TypeCd);

                                    updateSQLite(mat, hmgp, false, badmarkframeFG, m.MoldedName, m.MoldedClass, m.MoldedDateList);
                                }
                                else
                                {
                                    updateSQLite(mat, hmgp, false, badmarkframeFG);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.SysLog.Info("[ArmsNascaBridge] Material Error:" + ex.ToString());
                        }
                    }
                    Console.WriteLine(DateTime.Now.ToString() + ":" + i.ToString() + "/" + ggcodes.Length.ToString() + " end");
                    i++;
                }                
                importMaterialOpenData();
            }
            catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge] Material Error:" + err.ToString());
			}
        }

        /// <summary>
        /// 資材開封日時ファイル取り込み
        /// </summary>
        private static void importMaterialOpenData()
        {
            try
            {
                List<string> files = GetMaterialOpenFiles(Config.Settings.MaterialOpenFilePath);
                foreach (string file in files)
                {
                    MatChar[] mcList = getMaterialOpenDateList(file);
                    foreach (MatChar c in mcList)
                    {
                        // データベース登録データで開封日時の違う同じ資材が登録されていた場合、
                        // 日付が新しい方を登録する
                        if (MatChar.Exists(c.MaterialCd, c.LotNo, c.LotCharCd))
                        {
                            MatChar dbData = MatChar.GetMaterialOpen(c.MaterialCd, c.LotNo);

                            if (dbData.LastupdDt < c.LastupdDt)
                            {
                                MatChar.DeleteInsert(c.MaterialCd, c.LotNo, c.LotCharCd, c.LotCharVal, c.LastupdDt);
                            }
                        }
                        else
                        {
                            MatChar.InsertUpdate(c.MaterialCd, c.LotNo, c.LotCharCd, c.LotCharVal, false, c.LastupdDt);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                Log.SysLog.Error("[ArmsNascaBridge] MaterialOpen Error:" + err.ToString());
            }
        }

        public static MatChar[] getMaterialOpenDateList(string file)
        {
            DateTime fileDt = File.GetLastWriteTime(file);

            List<MatChar> retv = new List<MatChar>();
            using (StreamReader sr = new StreamReader(file))
            {
                while (sr.Peek() > 0)
                {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] data = line.Split(',');

                        if (data.Length != 4)
                        {
                            throw new Exception(string.Format("資材開封ファイルフォーマットが不正です。ファイル:{0}", file));
                        }

                        MatChar c = new MatChar();

                        c.LotNo = data[0].Trim();
                        c.MaterialCd = data[1].Trim();
                        c.LotCharCd = data[2].Trim();
                        c.LotCharVal = data[3].Trim();
                        c.LastupdDt = fileDt;

                        if (ArmsApi.Model.Material.IsImported(c.MaterialCd, c.LotNo))
                        { 
                            retv.Add(c);
                        }
                    }
                }
            }

            return retv.ToArray();
        }

		private static void updateSQLite(Material mat, string hmgp, bool iswafer, bool isbadmarkframe, string moldedNm, string moldedClass, List<string> moldedDateList, string waferBlendCd, List<string> waferWorkCdList, string waferSupplyId)
        {
            bool isframe = false;
            if (hmgp == "MATE001" || hmgp == "MATE039")
            {
                isframe = true;
            }

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

				cmd.Parameters.Add("@SUPPLYID", SqlDbType.NVarChar).Value = waferSupplyId ?? string.Empty;
				cmd.Parameters.Add("@DICEWAFERKB", SqlDbType.NVarChar).Value = mat.DiceWaferKb ?? string.Empty;

                cmd.Parameters.Add("@STOCKCT", SqlDbType.Decimal).Value = mat.StockCt;

                cmd.CommandText = @"
                    SELECT lastupddt FROM TnMaterials
                    WHERE materialcd=@MATCD AND lotno=@LOTNO";

                object objlastupd = cmd.ExecuteScalar();

                if (objlastupd == null)
                {
					cmd.CommandText = @"
                        INSERT INTO TnMaterials(materialcd, lotno, materialnm, limitdt, bincd, hmgroup, iswafer, isframe, delfg, lastupddt, isbadmarkframe, framemoldednm, framemoldedclass, framemoldeddt, blendcd, workcd, supplyId, dicewaferkb, stockct)
                        VALUES (@MATCD, @LOTNO, @MATNM, @LIMITDT, @BINCD, @HMGP, @ISWAFER, @ISFRAME,  0, @UPDDT, @ISBADMARKFRAME, @MOLDEDNM, @MOLDEDCLASS, @MOLDEDDATE, @BLENDCD, @WORKCD, @SUPPLYID, @DICEWAFERKB, @STOCKCT);";

                    cmd.ExecuteNonQuery();
                    return;
                }
                else
                {
					DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
					if (mat.LastUpddt > current)
					{
						cmd.CommandText = @"
                            UPDATE TnMaterials SET limitdt=@LIMITDT, bincd=@BINCD, hmgroup=@HMGP, iswafer=@ISWAFER, isframe=@ISFRAME, isbadmarkframe = @ISBADMARKFRAME, lastupddt=@UPDDT, framemoldednm = @MOLDEDNM, framemoldedclass = @MOLDEDCLASS, framemoldeddt = @MOLDEDDATE, blendcd=@BLENDCD, workcd = @WORKCD, supplyId = @SUPPLYID, dicewaferkb = @DICEWAFERKB, stockct = @STOCKCT
                            WHERE materialcd=@MATCD AND lotno=@LOTNO";

                        cmd.ExecuteNonQuery();
					}
                }
            }
        }

        private static void updateSQLite(Material mat, string hmgp, bool iswafer, bool isbadmarkframe, string moldedNm, string moldedClass, List<string> moldedDateList)
        {
            updateSQLite(mat, hmgp, iswafer, isbadmarkframe, moldedNm, moldedClass, moldedDateList, null, null, null);
        }

		private static void updateSQLite(Material mat, string hmgp, bool iswafer, bool isbadmarkframe)
		{
			updateSQLite(mat, hmgp, iswafer, isbadmarkframe, null, null, null, null, null, null);
		}

		private static void updateSQLite(Material mat, string hmgp, bool iswafer, bool isbadmarkframe, string waferBlendCd, List<string> waferWorkCdList, string waferSupplyId)
        {
            updateSQLite(mat, hmgp, iswafer, isbadmarkframe, null, null, null, waferBlendCd, waferWorkCdList, waferSupplyId);
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
                //if (Config.GetLineType == Config.LineTypes.NEL_MAP || Config.GetLineType == Config.LineTypes.MEL_MAP)
                //{
                //    cmd.Parameters.Add("@SECTIONCD", SqlDbType.VarChar).Value = SLE_SECTION_CD + "%";
                //}
                //else
                //{
                //    cmd.Parameters.Add("@SECTIONCD", SqlDbType.VarChar).Value = SLC_SECTION_CD + "%";
                //}
                //cmd.Parameters.Add("@SECTIONCD", SqlDbType.VarChar).Value = Config.SectionCd + "%";

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
                     and not exists(
	                    select * from rvmmat m
                        where m.material_cd = t.material_cd and (dicewafer_kb in (4, 2, 8, 16))) 
                        OPTION (MAXDOP 1)";

				//2, 8, 16 = ウェハ
				//4 = 加工品
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
                        , stock_ct
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
                        m.StockCt = Convert.ToDecimal(rd["stock_ct"]);
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

		///// <summary>
		///// 成型金型取得
		///// </summary>
		///// <param name="lotNo"></param>
		///// <param name="TypeCd"></param>
		///// <returns></returns>
		//private static string getMoldedName(string lotNo, string typeCd)
		//{
		//	ArmsApi.Model.NASCA.NascaPubApi api = ArmsApi.Model.NASCA.NascaPubApi.GetInstance();
		//	NppLotCharInfo[] lotChars = api.SearchLotChar(lotNo, typeCd, "M0000009", false);
		//	if (lotChars == null || lotChars.Count() == 0)
		//	{
		//		return null;
		//	}
		//	else 
		//	{
		//		return lotChars.Single().LotCharValue;
		//	}
		//}

		///// <summary>
		///// 成型区分取得　//2015.11.17 複数持つ場合はnull値を返すよう修正。（データがnullだと強制的に検査対象になる）
		///// </summary>
		///// <param name="lotNo"></param>
		///// <param name="typeCd"></param>
		///// <returns></returns>
		//private static string getMoldedClass(string lotNo, string typeCd)
		//{
		//	ArmsApi.Model.NASCA.NascaPubApi api = ArmsApi.Model.NASCA.NascaPubApi.GetInstance();
		//	NppLotCharInfo[] lotChars = api.SearchLotChar(lotNo, typeCd, "P0000202", false);
		//	if (lotChars == null)
		//	{
		//		return null;
		//	}
		//	else if (lotChars.Count() == 0 || lotChars.Count() > 1)
		//	{
		//		return null;
		//	}
		//	else
		//	{
		//		return lotChars.Single().CharValCD.Trim();
		//	}
		//}

		///// <summary>
		///// 成型日取得
		///// </summary>
		///// <param name="lotNo"></param>
		///// <param name="typeCd"></param>
		///// <returns></returns>
		//private static List<string> getMoldedDate(string lotNo, string typeCd)
		//{
		//	List<string> retv = new List<string>();

		//	ArmsApi.Model.NASCA.NascaPubApi api = ArmsApi.Model.NASCA.NascaPubApi.GetInstance();
		//	NppLotCharInfo[] lotChars = api.SearchLotChar(lotNo, typeCd, "M0000116", false);
		//	if (lotChars != null && lotChars.Count() != 0)
		//	{
		//		foreach (NppLotCharInfo lotChar in lotChars)
		//		{
		//			DateTime date;
		//			if (DateTime.TryParse(lotChar.LotCharValue, out date))
		//			{
		//				retv.Add(date.ToString("yyyy/MM/dd"));
		//			}
		//		}
		//	}

		//	return retv;
		//}

		/// <summary>
		/// リードフレーム成形情報取得(金型名、成形区分、成形日)
		/// </summary>
		/// <param name="lotNo"></param>
		/// <param name="typeCd"></param>
		/// <returns></returns>
		private static MetallicMolding getMetallicMoldingData(string lotNo, string typeCd) 
		{
			MetallicMolding m = new MetallicMolding();

			List<string> moldedClassList = new List<string>();
			List<string> moldedDateList = new List<string>();

			using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = string.Format(@" SELECT NttLTNA.NascaLot_NO, NttLTTS.LotChar_CD, NttLTTS.LotChar_VAL, NttLTTS.CharVal_CD
							FROM dbo.NttLTTS AS NttLTTS WITH (nolock) 
							INNER JOIN dbo.NttLTNA AS NttLTNA WITH (nolock) ON NttLTTS.NascaLot_ID = NttLTNA.NascaLot_ID
							WHERE (NttLTNA.NascaLot_NO = @LotNo) AND (NttLTNA.Type_CD = @TypeCd) 
							AND (NttLTTS.LotChar_CD IN ('{0}', '{1}', '{2}')) OPTION (MAXDOP 1) ",
								MOLDEDNAME_LOTCHARCD, MOLDEDDATE_LOTCHARCD, MOLDEDCLASS_LOTCHARCD);

				cmd.Parameters.Add("@LotNo", SqlDbType.VarChar).Value = lotNo;
				cmd.Parameters.Add("@TypeCd", SqlDbType.Char).Value = typeCd;

				cmd.CommandText = sql;

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					int ordLotNo = reader.GetOrdinal("NascaLot_NO");
					int ordLotCharCd = reader.GetOrdinal("LotChar_CD");
					int ordLotCharVal = reader.GetOrdinal("LotChar_VAL");
					int ordLotCharValCd = reader.GetOrdinal("CharVal_CD");

					while (reader.Read())
					{
						m.LotNo = reader.GetString(ordLotNo);
						m.LotCharCd = reader.GetString(ordLotCharCd);

						// 金型名
						if (reader.IsDBNull(ordLotCharVal) == false && m.LotCharCd == MOLDEDNAME_LOTCHARCD)
						{
							m.MoldedName = reader.GetString(ordLotCharVal).Trim();
						}
						// 成形区分
						if (reader.IsDBNull(ordLotCharValCd) == false && m.LotCharCd == MOLDEDCLASS_LOTCHARCD)
						{
							moldedClassList.Add(reader.GetString(ordLotCharValCd).Trim());
						}
						// 成形日
						if (reader.IsDBNull(ordLotCharVal) == false && m.LotCharCd == MOLDEDDATE_LOTCHARCD) 
						{
							moldedDateList.Add(reader.GetString(ordLotCharVal).Trim());
						}						
					}
				}

				if (moldedClassList.Count == 1)
				{
					m.MoldedClass = moldedClassList.Single();

					if (Config.Settings.FrameMoldedImportClass != null && Config.Settings.FrameMoldedImportClass.Exists(c => c == m.MoldedClass))
					{
						// 対象の成形区分の場合、成形日を取得
						m.MoldedDateList = moldedDateList;
					}
				}
				else
				{
					// 成形区分を複数持つ場合はnull値を返す。(nullだと強制的に検査対象になる為)
					m.MoldedClass = null;
				}
			}

			return m;
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
        
        /// <summary>
        /// 樹脂グループ特性の取得
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="typeCd"></param>
        /// <returns></returns>
        public static string GetResinGroupCd(Material mat)
        {
            string resinGroupCd = string.Empty;
            List<string> resinGroupTemp = new List<string>();


            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = string.Format(@" SELECT NttLTNA.NascaLot_NO, NttLTTS.LotChar_CD, NttLTTS.LotChar_VAL, NttLTTS.CharVal_CD
							FROM dbo.NttLTTS AS NttLTTS WITH (nolock) 
							INNER JOIN dbo.NttLTNA AS NttLTNA WITH (nolock) ON NttLTTS.NascaLot_ID = NttLTNA.NascaLot_ID
							WHERE (NttLTNA.NascaLot_NO = @LotNo) AND (NttLTNA.Type_CD = @TypeCd) 
							AND (NttLTTS.LotChar_CD = @LorCharCd )");

                cmd.Parameters.Add("@LotNo", SqlDbType.VarChar).Value = mat.LotNo;
                cmd.Parameters.Add("@TypeCd", SqlDbType.Char).Value = mat.TypeCd;
                cmd.Parameters.Add("@LorCharCd", SqlDbType.Char).Value = ArmsApi.Config.RESINGROUP_LOTCHARCD;

                cmd.CommandText = sql;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    int ordLotCharVal = reader.GetOrdinal("LotChar_VAL");

                    while (reader.Read())
                    {

                        resinGroupCd = reader.GetString(ordLotCharVal).Trim();
                        resinGroupTemp.Add(resinGroupCd);

                    }
                }
            }
            if (resinGroupTemp.Count() != 1)
            {
                return null;
            }

            return resinGroupCd;
        }
    

        /// <summary>
        /// 資材開封日時ファイルの対象分を返す(資材取り込み済、ファイル日付が14日以内)
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static List<string> GetMaterialOpenFiles(string dirPath)
        {
            if (Directory.Exists(dirPath) == false)
            {
                throw new ApplicationException(string.Format("指定されたパスは存在しない為、資材開封日時ファイルの取り込みを行いませんでした。フォルダパス:{0}", dirPath));
            }
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);

            List<string> retv = new List<string>();

            FileInfo[] fileInfoList = dirInfo.GetFiles();
            foreach (FileInfo fileInfo in fileInfoList)
            {
                string dateDir = Path.Combine(Path.Combine(dirPath, fileInfo.LastWriteTime.Year.ToString()), fileInfo.LastWriteTime.Month.ToString());
                if (Directory.Exists(dateDir) == false)
                {
                    Directory.CreateDirectory(dateDir);
                }

                try
                {
                    File.Move(fileInfo.FullName, Path.Combine(dateDir, fileInfo.Name));
                }
                catch (IOException)
                {
                    // ファイルロック等で移動できない場合は次回に
                    continue;
                }
            }

            // 当月フォルダと前月フォルダ内のファイルを対象に
            string presentMonthDir = Path.Combine(dirPath, Path.Combine(System.DateTime.Now.Year.ToString(), System.DateTime.Now.Month.ToString()));
            if (Directory.Exists(presentMonthDir) == false)
            {
                Directory.CreateDirectory(presentMonthDir);
            }
            DirectoryInfo presentMonthDirInfo = new DirectoryInfo(presentMonthDir);

            DateTime pastMonthDate = System.DateTime.Now.AddMonths(-1);
            string pastMonthDir = Path.Combine(dirPath, Path.Combine(pastMonthDate.Year.ToString(), pastMonthDate.Month.ToString()));
            if (Directory.Exists(pastMonthDir) == false)
            {
                Directory.CreateDirectory(pastMonthDir);
            }
            DirectoryInfo pastMonthDirInfo = new DirectoryInfo(pastMonthDir);

            List<FileInfo> fileList = presentMonthDirInfo.GetFiles().ToList();
            fileList.AddRange(pastMonthDirInfo.GetFiles());

            // 14日前のファイルを対象から除去
            fileList = fileList.Where(f => f.LastWriteTime < System.DateTime.Now.AddDays(MATOPENFILE_TARGETDAY)).ToList();
            foreach (FileInfo file in fileList)
            {
                try
                {
                    string[] fileContents = File.ReadAllLines(file.FullName);
                    if (fileContents.Count() == 0)
                    {
                        // ファイル内が空の場合は放置
                        Log.SysLog.Error(string.Format("資材開封日時ファイルの内容が空です。ファイル名：{0}", file));
                        continue;
                    }

                    foreach (string fileLineValue in fileContents)
                    {
                        string[] fileLineChar = fileLineValue.Split(',');
                        if (ArmsApi.Model.Material.IsImported(fileLineChar[1].Trim(), fileLineChar[0].Trim()))
                        {
                            retv.Add(file.FullName);
                            break;
                        }
                    }
                }
                catch (Exception err)
                {
                    Log.SysLog.Error(string.Format("資材開封日時ファイル取り込みエラー:{0}　ファイルパス:{1}", err.Message, file.FullName));
                }
            }

            return retv;
        }       

        /// <summary>
        /// 金線MAXｍ数を品名から抜き出す
        /// </summary>
        /// <returns></returns>
        private static int getGoldWireStockCtFromMaterialName(string materialNm)
        {
            string text = new String(materialNm.Skip(materialNm.IndexOf("(") + 1).ToArray());
            text = new String(text.Take(text.IndexOf("m")).ToArray());
            
            int retv = 0;
            if (int.TryParse(text, out retv) == false)
            {
                Log.SysLog.Info($"金線名:{materialNm}を数値型に変換出来なかった為、在庫数0で保存します。");
                return 0;
            }

            return retv;
        }
    }
}
