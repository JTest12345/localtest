using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class LotChar
    {
		/// <summary>
		/// 先行ライフ試験条件
		/// </summary>
		public const string BEFORELIFETESTCONDCD_LOTCHARCD = "P0000205";

		/// <summary>
		/// ライフ試験結果
		/// </summary>
		public const string LIFETESTRESULTCONDCD_LOTCHARCD = "T0000008";

		/// <summary>
		/// ライフ試験数
		/// </summary>
		public const string LIFETESTCT_LOTCHARCD = "T0000001";

		/// <summary>
		/// 自動搬送硬化条件
		/// </summary>
		public const string AUTOLINEOVENPROF_LOTCHARCD = "P0000208";

		/// <summary>
		/// WB外観検査(1=無2=有3=全数)
		/// </summary>
		public const string WB_INSPECTION_LOTCHARCD = "P0000144";

        /// <summary>
        /// ダイシェア試験抜取グループ
        /// </summary>
        public const string DIE_SHARE_SAMPLING_PRIORITY_LOTCHARCD = "P0000212";

        /// <summary>
        /// 予定選別規格
        /// </summary>
        public const string SCHEDULE_SELECTION_STANDARD = "P0000051";

        /// <summary>
        /// 色調先行品
        /// </summary>
        public const string PRECOLORTONECONDCD_LOTCHARCD = "P0000031";


        public string LotNo { get; set; }
        public string LotCharCd { get; set; }
        public string LotCharVal { get; set; }
        public string ListVal { get; set; }

        public static LotChar[] GetLotChar(string lotno) 
        {
            return GetLotChar(lotno, null);
        }
        public static LotChar[] GetLotChar(string lotno, string lotcharcd) 
        {
            List<LotChar> retv = new List<LotChar>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;

                    string sql = @" SELECT lotno, lotcharcd, lotcharval, listval
                                         FROM TnLotCond WITH (nolock) 
                                         WHERE lotno = @LOTNO ";

                    if (!string.IsNullOrEmpty(lotcharcd))
                    {
                        sql += " AND lotcharcd = @LOTCHARCD ";
                        cmd.Parameters.Add("@LOTCHARCD", SqlDbType.NVarChar).Value = lotcharcd;
                    }

                    cmd.CommandText = sql;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LotChar lotchar = new LotChar();

                            lotchar.LotNo = SQLite.ParseString(reader["lotno"]).Trim();
                            lotchar.LotCharCd = SQLite.ParseString(reader["lotcharcd"]).Trim();
                            lotchar.LotCharVal = SQLite.ParseString(reader["lotcharval"]).Trim();
                            lotchar.ListVal = SQLite.ParseString(reader["listval"]).Trim();

                            retv.Add(lotchar);
                        }
                    }

                    return retv.ToArray();
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }
        }

		public static LotChar GetLifeTestResult(string lotno)
		{
			LotChar[] lotCharList = GetLotChar(lotno, LIFETESTRESULTCONDCD_LOTCHARCD);
			if (lotCharList.Count() == 0)
			{
				return null;
			}
			else 
			{
				return lotCharList.First();
			}
		}

		public static LotChar GetLifeTestCt(string lotno)
		{
			LotChar[] lotCharList = GetLotChar(lotno, LIFETESTCT_LOTCHARCD);
			if (lotCharList.Count() == 0)
			{
				return null;
			}
			else
			{
				return lotCharList.First();
			}
		}

        #region Delete

        public static void Delete(string lotno, string lotcharcd)
        {
            Delete(lotno, lotcharcd, SQLite.ConStr);
        }

        public static void Delete(string lotno, string lotcharcd, string constr)
        {
            //ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno ?? "";
                cmd.Parameters.Add("@CD", SqlDbType.NVarChar).Value = lotcharcd ?? "";

                try
                {
                    con.Open();
                    cmd.CommandText = "DELETE FROM TnLotCond WHERE lotno=@LOTNO AND lotcharcd=@CD";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("LotCond更新エラー", ex);
                }
            }
        }

        public static void Delete(SqlCommand cmd, string lotno)
        {
            if (string.IsNullOrEmpty(lotno)) 
            {
                return;
            }

            //削除ログ出力用
            LotChar[] lotchars = LotChar.GetLotChar(lotno);
            
            string sql = " DELETE FROM TnLotCond WHERE lotno Like @LOTNO ";
            cmd.CommandText = sql;

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";

            try
            {
                cmd.ExecuteNonQuery();

                foreach (LotChar lotchar in lotchars) 
                {
                    Log.DelLog.Info(string.Format("[TnLotCond] {0}\t{1}\t{2}\t{3}\t{4}",
                        lotchar.LotNo, lotchar.LotCharCd, lotchar.LotCharVal, lotchar.ListVal, System.DateTime.Now));
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("LotCond削除エラー:" + lotno, ex);
            }
        }

        #endregion

        #region DeleteInsert

        public void DeleteInsert(string lotno)
        {
            DeleteInsert(lotno, SQLite.ConStr);
        }

        public void DeleteInsert(string lotno, string constr)
        {
            DeleteInsert(lotno, SQLite.ConStr, true);
        }

        public void DeleteInsert(string lotno, string constr, bool logOutput)
        {
            //ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {

                if (this.LotCharVal == null)
                {
                    this.LotCharVal = this.ListVal;
                }

                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                cmd.Parameters.Add("@CD", SqlDbType.NVarChar).Value = this.LotCharCd ?? "";
                cmd.Parameters.Add("@VAL", SqlDbType.NVarChar).Value = this.LotCharVal ?? "";
                cmd.Parameters.Add("@LISTVAL", SqlDbType.NVarChar).Value = this.ListVal ?? "";
                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    //前履歴は削除
                    cmd.CommandText = "DELETE FROM TnLotCond WHERE lotno=@LOTNO AND lotcharcd=@CD";
                    cmd.ExecuteNonQuery();

                    //新規Insert
                    cmd.CommandText = @"
                            INSERT
                             INTO TnLotCond(lotno
	                            , lotcharcd
	                            , lotcharval
	                            , listval
	                            , lastupddt)
                            values(@LOTNO
	                            , @CD
	                            , @VAL
	                            , @LISTVAl
	                            , @UPDDT)";
                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                    if (logOutput == true)
                    {
                        Log.SysLog.Info("UPDATE LOTCHAR" + lotno + ":" + this.LotCharCd + ":" + this.LotCharVal + ":" + this.ListVal);
                    }
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("LotCond更新エラー", ex);
                }
            }
        }

        #endregion
    }

    public class MatChar
    {
		public string MaterialCd { get; set; }
		public string LotNo { get; set; }
		public string LotCharCd { get; set; }
		public string LotCharVal { get; set; }

        public DateTime LastupdDt { get; set; }

        /// <summary>
        /// 水洗浄日時
        /// </summary>
        public const string LOTCHARCD_WASHED = "P0000158";

        public const string LOTCHARCD_MATERIALOPEN = "P0000092";

        public static MatChar GetMaterialOpen(string materialcd, string matlotno)
        {
            MatChar[] list = GetMatChar(materialcd, matlotno, LOTCHARCD_MATERIALOPEN);
            if (list.Count() >= 2)
            {
                Log.SysLog.Info(string.Format("複数の開封日時がデータベース登録されています。不正データの為、削除してください。品目：{0} ロット：{1}", materialcd, matlotno));
                return list.First();
            }
            else if (list.Count() == 1)
            {
                return list.Single();
            }
            else
            {
                return null;
            }
        }

		public static MatChar[] GetMatChar(string materialcd, string matlotno)
		{
			return GetMatChar(materialcd, matlotno, null);
		}
        
		public static MatChar[] GetMatChar(string materialcd, string matlotno, string lotcharcd)
		{
			List<MatChar> retv = new List<MatChar>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				cmd.Parameters.Add("@MATERIALCD", SqlDbType.NVarChar).Value = materialcd;
				cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = matlotno;

				string sql = @" SELECT  materialcd, lotno, lotcharcd, lotcharval, lastupddt
                                        FROM TnMatCond WITH (nolock) 
                                        WHERE delfg = 0 AND lotno = @LOTNO AND materialcd = @MATERIALCD ";

				if (!string.IsNullOrEmpty(lotcharcd))
				{
					sql += " AND lotcharcd = @LOTCHARCD ";
					cmd.Parameters.Add("@LOTCHARCD", SqlDbType.NVarChar).Value = lotcharcd;
				}

				cmd.CommandText = sql;

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						MatChar matChar = new MatChar();

						matChar.MaterialCd = SQLite.ParseString(reader["materialcd"]).Trim();
						matChar.LotNo = SQLite.ParseString(reader["lotno"]).Trim();
						matChar.LotCharCd = SQLite.ParseString(reader["lotcharcd"]).Trim();
						matChar.LotCharVal = SQLite.ParseString(reader["lotcharval"]).Trim();
						matChar.LastupdDt = SQLite.ParseDate(reader["lastupddt"]).Value;
						retv.Add(matChar);
					}
				}

				return retv.ToArray();
			}
		}

		/// <summary>
		/// 水洗浄した最終日時を取得
		/// </summary>
		/// <param name="materialcd"></param>
		/// <param name="matlotno"></param>
		/// <param name="lotcharcd"></param>
		/// <returns></returns>
		public static DateTime? GetWashedLastDate(string materialcd, string matlotno)
		{
			DateTime? retv = null;

			MatChar[] matChars = GetMatChar(materialcd, matlotno, LOTCHARCD_WASHED);
			foreach (MatChar c in matChars)
			{
				DateTime washedDt;
				if (DateTime.TryParse(c.LotCharVal, out washedDt) == false)
				{
					throw new ApplicationException(string.Format("水洗浄時間が不正です。 ウェハロット番号:{0} 水洗浄時間:{1}", matlotno, c.LotCharVal));
				}

				if (retv.HasValue == false || retv.Value < washedDt)
				{
					retv = washedDt;
				}
			}

			return retv;
		}
        
		public static void InsertUpdate(string materialcd, string lotno, string lotcharcd, string lotcharval, bool delfg, DateTime lastupddt)
		{
			if (string.IsNullOrEmpty(materialcd) || string.IsNullOrEmpty(lotno) || string.IsNullOrEmpty(lotcharcd) || string.IsNullOrEmpty(lotcharval))
			{
				throw new ApplicationException("TnMatCondの更新に必要なキー情報が不足しています。");
			}

			using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = conn.CreateCommand())
			{
				conn.Open();

                string sql = @" SELECT lotno FROM TnMatCond WITH(nolock) 
                                WHERE materialcd = @MaterialCd and lotno = @LotNo and lotcharcd = @LotCharCd AND lotcharval = @LotCharVal ";

                cmd.CommandText = sql;

                cmd.Parameters.Add("@MaterialCd", SqlDbType.NVarChar).Value = materialcd;
                cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotno;
                cmd.Parameters.Add("@LotCharCd", SqlDbType.NVarChar).Value = lotcharcd;
                cmd.Parameters.Add("@LotCharVal", SqlDbType.NVarChar).Value = lotcharval;
                cmd.Parameters.Add("@DelFG", SqlDbType.Int).Value = SQLite.ParseInt(delfg);
                cmd.Parameters.Add("@LastUpdDt", SqlDbType.DateTime).Value = lastupddt;

                object exists = cmd.ExecuteScalar();
                if (exists != null)
                {
                    // 登録済みの場合は未処理
                    return;
                }

                sql = @" UPDATE TnMatCond SET
                                    delfg = @DelFG, lastupddt = getdate()
                                WHERE materialcd = @MaterialCd AND lotno = @LotNo AND lotcharcd = @LotCharCd AND lotcharval = @LotCharVal
                                INSERT INTO TnMatCond (materialcd, lotno, lotcharcd, lotcharval, lastupddt)
                                    SELECT @MaterialCd, @LotNo, @LotCharCd, @LotCharVal, @LastUpdDt
                                    WHERE NOT EXISTS (SELECT * FROM TnMatCond WHERE MaterialCd = @MaterialCd AND lotno = @LotNo AND lotcharcd = @LotCharCd AND lotcharval = @LotCharVal) ";              

                cmd.CommandText = sql;
				cmd.ExecuteNonQuery();
			}
		}

        public static void DeleteInsert(string materialcd, string lotno, string lotcharcd, string lotcharval, DateTime lastupdDt)
        {
            if (string.IsNullOrEmpty(materialcd) || string.IsNullOrEmpty(lotno) || string.IsNullOrEmpty(lotcharcd))
            {
                throw new ApplicationException("TnMatCondの更新に必要なキー情報が不足しています。");
            }

            using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

                string sql = @" DELETE FROM TnMatCond
                                WHERE materialcd = @MaterialCd and lotno = @LotNo and lotcharcd = @LotCharCd ";
                cmd.CommandText = sql;

                cmd.Parameters.Add("@MaterialCd", SqlDbType.NVarChar).Value = materialcd;
                cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotno;
                cmd.Parameters.Add("@LotCharCd", SqlDbType.NVarChar).Value = lotcharcd;
                cmd.Parameters.Add("@LotCharVal", SqlDbType.NVarChar).Value = lotcharval;
                cmd.Parameters.Add("@LastupdDt", SqlDbType.DateTime).Value = lastupdDt;

                cmd.ExecuteNonQuery();

                sql = @" INSERT INTO TnMatCond (materialcd, lotno, lotcharcd, lotcharval, lastupddt)
                         VALUES(@MaterialCd, @LotNo, @LotCharCd, @LotCharVal, @LastupdDt) ";

                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        public static bool Exists(string materialcd, string lotno, string lotcharcd)
        {
            MatChar[] list = GetMatChar(materialcd, lotno, lotcharcd);
            if (list.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #region "FJH ADD 20221006"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="materialcd"></param>
        /// <param name="lotno"></param>
        /// <param name="lotcharcd"></param>
        /// <param name="lotcharval"></param>
        /// <param name="delfg"></param>
        /// <param name="lastupddt"></param>
        public static void InsertUpdateMatCond(string materialcd, string lotno, string lotcharcd, string lotcharval, bool delfg, DateTime lastupddt)
        {
            if (string.IsNullOrEmpty(materialcd) || string.IsNullOrEmpty(lotno) || string.IsNullOrEmpty(lotcharcd) || string.IsNullOrEmpty(lotcharval))
            {
                throw new ApplicationException("TnMatCondの更新に必要なキー情報が不足しています。");
            }

            using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

                string sql = @" SELECT lotcharval FROM TnMatCond WITH(nolock) 
                                WHERE materialcd = @MaterialCd and lotno = @LotNo and lotcharcd = @LotCharCd ";

                cmd.CommandText = sql;

                cmd.Parameters.Add("@MaterialCd", SqlDbType.NVarChar).Value = materialcd;
                cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotno;
                cmd.Parameters.Add("@LotCharCd", SqlDbType.NVarChar).Value = lotcharcd;
                cmd.Parameters.Add("@LotCharVal", SqlDbType.NVarChar).Value = lotcharval;
                cmd.Parameters.Add("@DelFG", SqlDbType.Int).Value = SQLite.ParseInt(delfg);
                cmd.Parameters.Add("@LastUpdDt", SqlDbType.DateTime).Value = lastupddt;

                object objLotCharVal = cmd.ExecuteScalar();
                if (objLotCharVal == null)
                {
                    // 存在しない場合は登録
                    sql = @" INSERT INTO TnMatCond (materialcd, lotno, lotcharcd, lotcharval, lastupddt)
                             SELECT @MaterialCd, @LotNo, @LotCharCd, @LotCharVal, @LastUpdDt ";
                }
                else if (delfg)
                {
                    // 削除FLGがtrueの場合の処理
                    sql = @" UPDATE TnMatCond SET
                                     delfg = @DelFG, lastupddt = @LastUpdDt
                                WHERE materialcd = @MaterialCd AND lotno = @LotNo AND lotcharcd = @LotCharCd ";
                }
                else if (objLotCharVal.ToString() != lotcharval)
                {
                    // 開封日が異なる場合は更新
                    sql = @" UPDATE TnMatCond SET
                                     lotcharval = @LotCharVal, delfg = @DelFG, lastupddt = @LastUpdDt
                                WHERE materialcd = @MaterialCd AND lotno = @LotNo AND lotcharcd = @LotCharCd ";
                }

                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }
        #endregion
    }


    public class WorkCondition
    {
        #region 遠心沈降機基板サイズ

        public enum MapFrameSize : int
        {
            None,
            Large,
            Medium
        }

        /// <summary>
		/// [Nasca取得を廃止する為、CompareECKProgramへ]遠心沈降のプログラム照合
        /// </summary>
        /// <param name="typecd"></param>
        /// <param name="macno"></param>
        /// <param name="progno"></param>
        /// <returns></returns>
        public static bool CompareECKProgramToNasca(string typecd, int macno, string progno)
        {
            MachineInfo mac = MachineInfo.GetMachine(macno);
            if (mac == null) throw new ApplicationException("設備情報が見つかりません:" + macno);

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = typecd + ".%";
                cmd.Parameters.Add("@PLANTCD", SqlDbType.Char).Value = mac.NascaPlantCd;

                cmd.CommandText = @"
                    select general_val from ntmpdjk WITH(nolock)
                        where material_cd like @MATCD
                        and del_Fg = 0
                        and condition_kb = 28 OPTION (MAXDOP 1) ";

                //and condition_kb = 8
                //and plant_cd = @PLANTCD

                object obj = cmd.ExecuteScalar();
                if (obj == null)
                {
                    Log.ApiLog.Info(string.Format("ECKプログラム照合不一致 [装置:{0} マスタ:{1}]", progno.Trim(), "存在しません"));
                    return false;
                }
                else if (obj.ToString().Trim() == progno.Trim())
                {
                    return true;
                }
                else
                {
                    Log.ApiLog.Info(string.Format("ECKプログラム照合不一致 [装置:{0} マスタ:{1}]", progno.Trim(), obj.ToString().Trim()));
                    return false;
                }
            }
        }

		/// <summary>
		/// 遠心沈降のプログラム照合
		/// </summary>
		/// <param name="typecd"></param>
		/// <param name="macno"></param>
		/// <param name="progno"></param>
		/// <returns></returns>
		public static bool CompareECKProgram(string typecd, int macno, string progno)
		{
			MachineInfo mac = MachineInfo.GetMachine(macno);
			if (mac == null) throw new ApplicationException("設備情報が見つかりません:" + macno);

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;

				cmd.CommandText = @"
                    SELECT condval
					FROM TmTypeCond WITH (nolock) 
					WHERE delfg = 0 AND typecd = @TYPECD AND condcd = '28' ";

				object obj = cmd.ExecuteScalar();
				if (obj == null)
				{
					Log.ApiLog.Info(string.Format("ECKプログラム照合不一致 [装置:{0} マスタ:{1}]", progno.Trim(), "存在しません"));
					return false;
				}
				else if (obj.ToString().Trim() == progno.Trim())
				{
					return true;
				}
				else
				{
					Log.ApiLog.Info(string.Format("ECKプログラム照合不一致 [装置:{0} マスタ:{1}]", progno.Trim(), obj.ToString().Trim()));
					return false;
				}
			}
		}

        /// <summary>
        /// TmTypeCondの情報を取得
        /// </summary>
        /// <param name="typecd"></param>
        /// <param name="macno"></param>
        /// <param name="progno"></param>
        /// <returns></returns>
        public static string GetTypeCondVal(string typecd, string condcd, string workcd)
        {
            string retV = string.Empty;

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
                cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar).Value = condcd;

                string sql = @"
                    SELECT condval
					FROM TmTypeCond WITH (nolock) 
					WHERE delfg = 0 AND typecd = @TYPECD AND condcd = @CONDCD ";

                if (string.IsNullOrWhiteSpace(workcd) == false)
                {
                    sql += " AND workcd = @WORKCD ";
                    cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = workcd;
                }

                cmd.CommandText = sql;

                object obj = cmd.ExecuteScalar();
                if (obj == null)
                {
                    return string.Empty;
                }
                else
                {
                    return obj.ToString().Trim();
                }
            }
        }

        public static MapFrameSize GetMapFrameSize(string ggcode)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@GGCODE", SqlDbType.VarChar).Value = ggcode;
                cmd.CommandText = @"
                        select
                          h.Substrate_KB
                        from ntmhmhj h (nolock)
                        where h.material_cd =@GGCODE
                        and h.substrate_kb <> '0'
                        and h.substrate_kb is not null";

                object obj = cmd.ExecuteScalar();

                if (obj == null)
                {
                    return MapFrameSize.None;
                }
                else if (obj.ToString().Trim() == "1")
                {
                    //substrate_kb = 1は中判（仕様書記載）
                    return MapFrameSize.Medium;
                }
                else if (obj.ToString().Trim() == "2")
                {
                    //substrate_kb = 2は大判（仕様書記載）
                    return MapFrameSize.Large;
                }
                else
                {
                    return MapFrameSize.None;
                }
            }
        }

        /// <summary>
        /// NASCA参照サーバーからアウトラインロットのフレームサイズを取得
        /// 高効率ラインで遠心沈降機をアウトラインと共用するための暫定措置
        /// 将来は廃止すること
        /// lotno引数はヘッダー付き、フッター付きでも受け付け可能
        /// </summary>
        /// <param name="lotno"></param>
        /// <returns></returns>
        public static MapFrameSize GetMapFrameSizeForOutlineLot(string lotno)
        {

            if (lotno.StartsWith(AsmLot.PREFIX_INLINE_LOT)
                || lotno.StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE)
                || lotno.StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
            {
                lotno = lotno.Split(' ')[1];
            }

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;
                cmd.CommandText = @"
                        select
                          h.Substrate_KB
                        from nttsshj s (nolock)
                        inner join rvttranh t (nolock)
                        on s.mnfctinst_no = t.mnfctinst_no
                        inner join rvttranmat m (nolock)
                        on t.mnfctrsl_no = m.mnfctrsl_no
                        inner join ntmhmhj h (nolock)
                        on h.material_cd = mtralitem_cd
                        where s.lot_no = @LOTNO
                        and h.substrate_kb <> '0'
                        and h.substrate_kb is not null
                        and t.del_fg = 0
                        and m.del_fg = 0
                        and s.del_fg = 0";


                object obj = cmd.ExecuteScalar();

                if (obj == null)
                {
                    Log.ApiLog.Info("NASCA 大判/中判判定失敗: " + lotno);
                    //マスタ設定なしの場合はNoneを返し開始NGとする
                    return MapFrameSize.None;
                }
                else if (obj.ToString().Trim() == "1")
                {
                    //substrate_kb = 1は中判（仕様書記載）
                    return MapFrameSize.Medium;
                }
                else if (obj.ToString().Trim() == "2")
                {
                    //substrate_kb = 2は大判（仕様書記載）
                    return MapFrameSize.Large;
                }
                else
                {
                    Log.ApiLog.Info("NASCA 大判/中判判定失敗: " + lotno);
                    //マスタに1,2以外の値が入っている場合は仕様外なのでNoneを返して装置停止
                    return MapFrameSize.None;
                }
            }
        }

        #endregion

        #region Equals実装

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            WorkCondition cnd = (WorkCondition)obj;
            if (this.CondCd == cnd.CondCd && this.CondVal == cnd.CondVal)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        public string CondCd { get; set; }
        public string CondName { get; set; }
        public string CondVal { get; set; }
        public string BarcodeHeaderText { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime? EndDt { get; set; }
        public string WebString
        {
            get
            {
                return CondName + " 設定値：" + CondVal;
            }
        }
		public string workCd { get; set; }
		public string plantCd { get; set; }

		public const string SHIZAI_TOUNYU_JOUKEN_KB = "26";

        public static WorkCondition GetCondition(string condcd)
        {
            WorkCondition[] cl = SearchCondition(condcd, null, null, false);

            if (cl.Length == 0)
            {
                return null;
            }
            else
            {
                return cl[0];
            }
        }

        public static WorkCondition GetConditoinFromHeader(string headertext)
        {
            WorkCondition[] cl = SearchCondition(null, headertext, null, false);

            if (cl.Length == 0)
            {
                return null;
            }
            else
            {
                return cl[0];
            }
        }


        #region SearchCondition

        public static WorkCondition[] SearchCondition(string condcd, string barcodeHeaderText, string name, bool isNamePartialMatch)
        {
            List<WorkCondition> retv = new List<WorkCondition>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT
                        condcd
                        , condnm
                        , headertxt
                        FROM TmWorkCond
                        WHERE  delfg=0";

                    if (condcd != null)
                    {
                        cmd.CommandText += " AND condcd=@CONDCD";
                        cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar).Value = condcd;
                    }

                    if (barcodeHeaderText != null)
                    {
                        cmd.CommandText += " AND headertxt=@BCHEAD";
                        cmd.Parameters.Add("@BCHEAD", SqlDbType.NVarChar).Value = barcodeHeaderText;
                    }

                    if (name != null)
                    {
                        if (isNamePartialMatch)
                        {
                            cmd.CommandText += " AND condnm LIKE @CNDNM";
                            cmd.Parameters.Add("@CNDNM", SqlDbType.NVarChar).Value = "%" + name + "%";
                        }
                        else
                        {
                            cmd.CommandText += " AND condnm = @CNDNM";
                            cmd.Parameters.Add("@CNDNM", SqlDbType.NVarChar).Value = name;
                        }
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            WorkCondition c = new WorkCondition();
                            c.CondCd = SQLite.ParseString(reader["condcd"]);
                            c.CondName = SQLite.ParseString(reader["condnm"]);
                            c.BarcodeHeaderText = SQLite.ParseString(reader["headertxt"]);

                            retv.Add(c);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new ArmsException("製造条件マスタ取得エラー", ex);
            }

            return retv.ToArray();
        }
        #endregion

        #region GetCheckCondCd

		/// <summary>
		/// 工程に関連づいた確認必要特性取得
		/// </summary>
		/// <param name="procno"></param>
		/// <returns></returns>
		public static string[] GetCheckCondCd(int procno, MachineInfo mac)
		{
			List<string> retv = new List<string>();

			try
			{
				using (SqlConnection con = new SqlConnection(SQLite.ConStr))
				using (SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					cmd.CommandText = @"
                        SELECT
                        procno
                        , condcd
                        FROM TmProcCondCheck
                        WHERE procno=@PROCNO AND delfg=0";

					//2014.08.17 41移管2次で検証中
					if (mac.IsAutoLine)
					{
						cmd.CommandText += " AND autoline = 1 ";
					}
					if (mac.IsHighLine)
					{
						cmd.CommandText += " AND highline = 1 ";
					}

					cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno;

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							retv.Add(SQLite.ParseString(reader["condcd"]));
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new ArmsException("工程製造条件マスタ取得エラー", ex);
			}

			return retv.ToArray();
		}


        #endregion

        #region GetCondValsFromMachine

        /// <summary>
        /// 装置に割り付いている製造条件取得
        /// </summary>
        /// <param name="macno"></param>
        /// <param name="condcd"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string[] GetCondValsFromMachine(int macno, string condcd, DateTime date)
        {
            MachineInfo m = MachineInfo.GetMachine(macno);

            if (m != null)
            {
                WorkCondition[] conds = m.GetWorkConditions(condcd, date, date);
                return conds.Select(c => c.CondVal).ToArray();
            }

            throw new ArmsException("装置マスタ異常:" + macno);

        }
        #endregion

        /// <summary>
        /// プログラム運転時間を取得
        /// 設定が無い場合はnull
        /// </summary>
        /// <param name="cond"></param>
        /// <returns></returns>
        public static int? GetProgramMinutes(WorkCondition cond)
        {
            GnlMaster[] gnls = GnlMaster.Search("PROGRAMMIN", cond.CondCd, cond.CondVal, null);

            if (gnls.Length == 0)
            {
                return null;
            }
            else
            {
                return int.Parse(gnls[0].Val2);
            }
        }

        #region GetCondValsFromType

		/// <summary>
		/// ロット（タイプ）に関連づいた製造条件値取得
		/// </summary>
		/// <param name="lot"></param>
		/// <param name="condcd"></param>
		/// <returns></returns>
		public static List<WorkCondition> GetCondValsFromType(AsmLot lot, string condcd)
		{
			List<WorkCondition> retv = new List<WorkCondition>();

			try
			{
				using (SqlConnection con = new SqlConnection(SQLite.ConStr))
				using (SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					cmd.CommandText = @"
                        SELECT
                        typecd
                        , condcd
                        , condval
						, workcd
						, plantcd
                        FROM TmTypeCond
                        WHERE typecd=@TYPECD AND condcd=@CONDCD AND delfg=0";

					cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = lot.TypeCd;
					cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar).Value = condcd ?? "";

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							WorkCondition c = new WorkCondition();
							c.CondCd = condcd;
							c.CondVal = SQLite.ParseString(reader["condval"]);
							c.workCd = reader["workcd"].ToString().Trim();
							c.plantCd = reader["plantcd"].ToString().Trim();
							retv.Add(c);
						}
					}
				}

				return retv;
			}
			catch (Exception ex)
			{
				throw new ArmsException("タイプ製造条件マスタ取得エラー", ex);
			}
		}
        #endregion

		#region GetWorkCDFromType

		// SV3in1対応 n.yoshimoto 2015/1/13
		/// <summary>
		/// ロット（タイプ）と資材に関連づいた製造条件値(作業CD)取得
		/// </summary>
		/// <param name="lot"></param>
		/// <param name="condcd"></param>
		/// <returns></returns>
		public static string[] GetWorkCDFromType(string typecd, string matcd)
		{
			List<string> retv = new List<string>();

			try
			{
				using (SqlConnection con = new SqlConnection(SQLite.ConStr))
				using (SqlCommand cmd = con.CreateCommand())
				{
					con.Open();

					cmd.CommandText = @"
                        SELECT
                        typecd
                        , condcd
                        , condval
						, materialcd
						, workcd
                        FROM TmTypeCond
                        WHERE typecd=@TYPECD AND materialcd = @MATCD AND delfg=0";

					cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
					cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar).Value = SHIZAI_TOUNYU_JOUKEN_KB;
					cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar).Value = matcd ?? "";

					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							retv.Add(SQLite.ParseString(reader["workcd"]));
						}
					}
				}

			}
			catch (Exception ex)
			{
				throw new ArmsException("タイプ製造条件マスタ取得エラー", ex);
			}

			return retv.ToArray();
		}
		#endregion

		#region GetLotCharFromLot

		/// <summary>
        /// ロット（タイプ）に関連づいた製造条件値取得
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="condcd"></param>
        /// <returns></returns>
		public static LotChar[] GetLotCharFromLot(string lotno)
        {
            List<LotChar> retv = new List<LotChar>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {

                    con.Open();

                    cmd.CommandText = @"
                        SELECT
                        lotcharcd
                        , lotcharval
                        , listval
                        FROM TnLotCond with(nolock)
                        WHERE lotno=@LOTNO";

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
					
					//if (!string.IsNullOrEmpty(lotcharcd))
					//{
					//	cmd.CommandText += " AND lotcharcd = @LOTCHARCD ";
					//	cmd.Parameters.Add("@LOTCHARCD", SqlDbType.NVarChar).Value = lotcharcd;
					//}

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            LotChar lc = new LotChar();

                            lc.LotCharCd = SQLite.ParseString(rd["lotcharcd"]);
                            lc.LotCharVal = SQLite.ParseString(rd["lotcharval"]);
                            lc.ListVal = SQLite.ParseString(rd["listval"]);

                            retv.Add(lc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("タイプ製造条件マスタ取得エラー", ex);
            }

            return retv.ToArray();
        }

        //public static LotChar[] GetLotCharFromLot(string lotno)
        //{
        //	return GetLotCharFromLot(lotno, string.Empty);
        //}

        #endregion

        public static string GetSupplyId(string blendCd, string workCd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = " SELECT supplyid FROM TmBlend WITH(nolock) WHERE blendcd = @BLENDCD AND workcd = @WORKCD ";

                cmd.Parameters.Add("@BLENDCD", SqlDbType.NVarChar).Value = blendCd;
                cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = workCd;

                cmd.CommandText = sql;

                object supplyId = cmd.ExecuteScalar();
                if (supplyId == null)
                    return "";
                else
                    return supplyId.ToString();
            }
        }

        public static string GetBlendConditionResinGroupCd(string blendCd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = " SELECT ResinGp_CD FROM NtmBDCD WITH(nolock) WHERE Blend_CD = @BLENDCD ";

                cmd.Parameters.Add("@BLENDCD", SqlDbType.VarChar).Value = blendCd;

                cmd.CommandText = sql;

                object resinGroupCd = cmd.ExecuteScalar();
                if (resinGroupCd == null)
                    return "";
                else
                    return resinGroupCd.ToString();
            }
        }
    }
}
