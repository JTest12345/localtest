using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data.Common;
using System.Data;

namespace EICS.Database
{
    /// <summary>
    /// 閾値マスタ
    /// </summary>
    public class Plm
    {
        public int QcParamNO { get; set; }
        public int? RefQcParamNO { get; set; }
		public string MaterialCD { get; set; }
        public string EquipmentNO { get; set; }
        public string ResinGroupCD { get; set; }
        public string ManageNM { get; set; }
		public string ModelNM { get; set; }
        public string ParameterNM { get; set; }
        public string TotalKB { get; set; }
        public string DieKB { get; set; }

        public decimal? ParameterMAX { get; set; }
        public decimal? ParameterMIN { get; set; }
        public string ParameterVAL
        {
            get { return parameterVAL; }
            set { this.parameterVAL = value; }
        }
        private string parameterVAL = "";

        public decimal QcLineMAX { get; set; }
        public decimal QcLineMIN { get; set; }
        public decimal AimLineVAL { get; set; }
        public decimal AimRateVAL { get; set; }
        public bool DsFG { get; set; }

        public decimal? InnerUpperLimit { get; set; }
        public decimal? InnerLowerLimit { get; set; }

		public float? ParamGetUpperCond { get; set; }
		public float? ParamGetLowerCond { get; set; }

		public int EquipManageFG { get; set; }
		public bool UnManageTrend_FG { get; set; }
		public bool WithoutFileFmt_FG { get; set; }

        public string ChangeUnitVal { get; set; }

        public bool ProgramNmFG { get; set; }
        public int ResinGroupManageFG { get; set; }

        public string ChipNM { get; set; }

        public static List<Plm> GetData(int lineCd, string typeCd, string modelNm, string chipNm)
        {
            if (string.IsNullOrEmpty(typeCd) || string.IsNullOrEmpty(modelNm))
            {
                throw new ApplicationException(
                    string.Format("Plm.GetData()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1}", typeCd, modelNm));
            }

            return GetDatas(lineCd, typeCd, modelNm, null, true, chipNm);
        }

		public static Plm GetData(int lineCd, string typeCd, string modelNm, int qcParamNo, bool isIncludeNascaParam)
        {
            if (string.IsNullOrEmpty(typeCd) || string.IsNullOrEmpty(modelNm) || qcParamNo == 0)
            {
                throw new ApplicationException(
                    string.Format("Plm.GetData()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1} QcParamNo:{2}", typeCd, modelNm, qcParamNo));
            }

			List<Plm> plmList = GetDatas(lineCd, typeCd, modelNm, qcParamNo, isIncludeNascaParam, string.Empty);
            if (plmList.Count == 0)
            {
                return null;
            }
            else
            {
                return plmList.Single();
            }
        }

		public static Plm GetData(int lineCd, string typeCd, string modelNm, int qcParamNo, bool isIncludeNascaParam, string chipNM)
		{
			if (string.IsNullOrEmpty(typeCd) || string.IsNullOrEmpty(modelNm) || qcParamNo == 0)
			{
				throw new ApplicationException(
					string.Format("Plm.GetData()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1} QcParamNo:{2}", typeCd, modelNm, qcParamNo));
			}

			List<Plm> plmList = GetDatas(lineCd, typeCd, modelNm, qcParamNo, isIncludeNascaParam, chipNM);
			if (plmList.Count == 0)
			{
				return null;
			}
			else
			{
				return plmList.Single();
			}
		}

		public static Plm GetData(int lineCd, string typeCd, string modelNm, int qcParamNo, string chipNm)
		{
			if (string.IsNullOrEmpty(typeCd) || string.IsNullOrEmpty(modelNm) || qcParamNo == 0)
			{
				throw new ApplicationException(
					string.Format("Plm.GetData()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1} QcParamNo:{2}", typeCd, modelNm, qcParamNo));
			}

			List<Plm> plmList = GetDatas(lineCd, typeCd, modelNm, qcParamNo, false, chipNm);
			if (plmList.Count == 0)
			{
				return null;
			}
			else
			{
				return plmList.Single();
			}

		}

		public static Plm GetData(int lineCD, List<Plm> plmList, string typeCd, string modelNm, string equipmentNo, int qcParamNo)
		{
			plmList = plmList.Where(p => p.MaterialCD == typeCd && p.QcParamNO == qcParamNo).ToList();

			if (plmList.Count == 0)
			{
				return null;
			}
			else
			{
				string errMsg = string.Empty;

				if (HasProblem(lineCD, plmList, ref errMsg))
				{
					throw new ApplicationException(errMsg);
				}

				//Model_NM、Material_CD, QcParam_NO指定で取得したレコードにおいて
				//設備CD指定と無指定のレコードが混在する場合はNG
				if (plmList.Count > 1 && plmList.Exists(p => string.IsNullOrEmpty(p.EquipmentNO)))
				{
					throw new ApplicationException(string.Format(Constant.MessageInfo.Message_154, modelNm, typeCd, qcParamNo));
				}

				//取得レコードから設備コードがnullか空白のレコードを抽出。存在しなければ設備コード指定でレコード検索
				if (plmList.Exists(p => string.IsNullOrEmpty(p.EquipmentNO)))
				{
					return plmList.Single();
				}
				else
				{
					List<Plm> plmInfoSpecifyEquipList = plmList.Where(p => p.EquipmentNO == equipmentNo).ToList();

					if (plmInfoSpecifyEquipList.Count == 0)
					{//設備CDを指定してレコードが取得出来ない場合
						Prm prm = Prm.GetData(lineCD, qcParamNo, modelNm, string.Empty);
						throw new ApplicationException(string.Format(Constant.MessageInfo.Message_28, typeCd, qcParamNo, prm.ParamNM));
					}

					return plmInfoSpecifyEquipList.Single();
				}
			}
		}

		public static Plm GetData(int lineCD, string typeCd, string modelNm, int qcParamNo, string equipmentNo, bool isIncludeNascaParam)
		{
			return GetData(lineCD, typeCd, modelNm, qcParamNo, equipmentNo, isIncludeNascaParam, string.Empty);
		}

		public static Plm GetData(int lineCD, string typeCd, string modelNm, int qcParamNo, string equipmentNo, bool isIncludeNascaParam, string chipNm) 
        {
            if (string.IsNullOrEmpty(typeCd) || string.IsNullOrEmpty(modelNm) || qcParamNo == 0)
            {
                throw new ApplicationException(
                    string.Format("Plm.GetData()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1} QcParamNo:{2}", typeCd, modelNm, qcParamNo));
            }

			List<Plm> plmList = GetDatas(lineCD, typeCd, modelNm, qcParamNo, isIncludeNascaParam, chipNm);

			return GetData(lineCD, plmList, typeCd, modelNm, equipmentNo, qcParamNo);
		
        }

        public static List<Plm> GetDatas(int lineCd, string typeCd, string modelNm, bool isIncludeNascaParam) 
        {
            if (string.IsNullOrEmpty(typeCd) || string.IsNullOrEmpty(modelNm))
            {
                throw new ApplicationException(
                    string.Format("Plm.GetDatas()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1} ", typeCd, modelNm));
            }

            return GetDatas(lineCd, typeCd, modelNm, null, isIncludeNascaParam, string.Empty);
        }

        public static List<Plm> GetDatas(int lineCd, string typeCd, string modelNm, int? qcParamNo, bool isIncludeNascaParam, string chipNm)
		{
			bool? withoutFileFmt = null;
			return GetDatas(lineCd, typeCd, modelNm, qcParamNo, isIncludeNascaParam, chipNm, withoutFileFmt);
		}

		public static List<Plm> GetDatas(int lineCd, string typeCd, string modelNm, int? qcParamNo, bool isIncludeNascaParam, string chipNm, bool? withoutFileFmt)
        {
            return GetDatas(lineCd, typeCd, modelNm, qcParamNo, isIncludeNascaParam, chipNm, ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCd), withoutFileFmt);
        }

		/// <summary>
		/// （本関数呼び出し時の注意事項!!!）
		/// この関数は必ずmodelNmかqcParamNoのどちらかを指定して呼び出す事
		/// どちらもnull、空白の場合は例外を発生させて処理停止させているので要注意!!!
		/// </summary>
		/// <param name="lineCd"></param>
		/// <param name="typeCd"></param>
		/// <param name="modelNm"></param>
		/// <param name="qcParamNo"></param>
		/// <param name="isIncludeNascaParam"></param>
		/// <param name="chipNm"></param>
		/// <returns></returns>
		public static List<Plm> GetDatas(int lineCd, string typeCd, string modelNm, int? qcParamNo, bool isIncludeNascaParam, string chipNm, string constr)
		{
			return GetDatas(lineCd, typeCd, modelNm, qcParamNo, isIncludeNascaParam, chipNm, constr, null);
		}

        /// <summary>
        /// Plmからのデータ取得をする。Plmに存在するNasca不良管理用の項目を含みたい場合はisIncludeNascaParamをtrueに、
        /// FileFmtが存在しなくていいPlmレコードを取得対象にする場合、withoutFileFmtをtrue、
        /// FileFmtが必要なPlmレコードを取得対象にする場合、withoutFileFmtをfalseに
        /// FileFmt要・不要を検索条件に含めたくない場合、withoutFileFmtをnullにする
        /// </summary>
        /// <param name="lineCd"></param>
        /// <param name="typeCd"></param>
        /// <param name="modelNm"></param>
        /// <param name="qcParamNo"></param>
        /// <param name="isIncludeNascaParam"></param>
        /// <param name="chipNm"></param>
        /// <param name="constr"></param>
        /// <param name="withoutFileFmt"></param>
        /// <returns></returns>
        public static List<Plm> GetDatas(int lineCd, string typeCd, string modelNm, int? qcParamNo, bool isIncludeNascaParam, string chipNm, string constr, bool? withoutFileFmt) 
        {
            List<Plm> retv = new List<Plm>();

            using (DBConnect conn = DBConnect.CreateInstance(constr, "System.Data.SqlClient", false))
            {
                string sql = @" SELECT TmPRM.QcParam_NO, TmPRM.Manage_NM, TmPRM.Parameter_NM, TmPRM.Total_KB, TmPRM.Die_KB, TmPLM.Parameter_MAX, 
                                        TmPLM.Parameter_MIN, TmPLM.Parameter_VAL, TmPLM.DS_FG, TmPRM.RefQcParam_NO, TmPLM.Equipment_NO, TmPRM.EquipManage_FG, 
                                        TmPLM.InnerUpperLimit, TmPLM.InnerLowerLimit, TmPLM.Material_CD, TmPRM.UnManageTrend_FG, TmPRM.WithoutFileFmt_FG, TmPRM.ProgramNm_FG, 
                                        TmPRM.ResinGroupManage_FG, TmPLM.ResinGroup_CD, TmPLM.Model_NM, TmPRM.Chip_NM
                                        FROM TmPLM WITH(nolock) 
                                        INNER JOIN TmPRM WITH(nolock) ON TmPLM.QcParam_NO = TmPRM.QcParam_NO 
                                        WHERE (TmPLM.Material_CD = @MaterialCD) 
                                        AND (TmPLM.Del_FG = 0) AND (TmPRM.Del_FG = 0) ";

                if (string.IsNullOrEmpty(modelNm) == false)
				{
					sql += " AND (TmPLM.Model_NM = @ModelNM) ";
					conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNm);
				}
				else if(qcParamNo.HasValue)
				{
				}
				else
				{
					throw new ApplicationException(string.Format(
						"許可されていない関数呼び出しが発生しました modelNm、qcParamNoの何れも無指定での関数呼び出しは禁止しています。ｼｽﾃﾑ担当者へ連絡して下さい。"));
				}

                if (qcParamNo.HasValue)
                {
                    sql +=  " AND (TmPLM.QcParam_NO = @QcParamNO) ";
					conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNo.Value);
                }

				if (isIncludeNascaParam == false)
				{
					//sql += " AND ((TmPLM.QcParam_NO < 10000) OR ((TmPLM.QcParam_NO >= 200000) AND (TmPLM.QcParam_NO < 300000))) ";
					sql += " AND (TmPRM.UnManageTrend_FG = 0) ";
				}

                if (!string.IsNullOrEmpty(chipNm))
                {
                    sql += " AND (TmPRM.Chip_NM = @ChipNM) ";
					conn.SetParameter("@ChipNM", SqlDbType.VarChar, chipNm);
                }

				if(withoutFileFmt.HasValue)
				{
					sql += " AND (TmPRM.WithoutFileFmt_FG = @WithoutFileFmtFG) ";
					conn.SetParameter("@WithoutFileFmtFG", SqlDbType.Bit, withoutFileFmt.Value);
				}
                conn.SetParameter("@MaterialCD", SqlDbType.Char, typeCd);                

                using (DbDataReader rd = conn.GetReader(sql))
                {
                    string ordDieKB = "Die_KB";
                    int ordRefQcParamNO = rd.GetOrdinal("RefQcParam_NO");
                    int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
					int ordEquipManageFG = rd.GetOrdinal("EquipManage_FG");
					int ordMaterialCD = rd.GetOrdinal("Material_CD");
					int ordWithoutFmt = rd.GetOrdinal("WithoutFileFmt_FG");
					int ordUnManage = rd.GetOrdinal("UnManageTrend_FG");
                    int ordProgramNmFG = rd.GetOrdinal("ProgramNm_FG");
                    int ordResinGroupManageFG = rd.GetOrdinal("ResinGroupManage_FG");
                    int ordResinGroupCD = rd.GetOrdinal("ResinGroup_CD");
                    int ordModelNM = rd.GetOrdinal("Model_NM");
                    int ordChipNM = rd.GetOrdinal("Chip_NM");

                    while (rd.Read())
                    {
                        Plm p = new Plm();
                        p.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                        p.ManageNM = Convert.ToString(rd["Manage_NM"]).Trim();
                        p.ParameterNM = Convert.ToString(rd["Parameter_NM"]).Trim();
                        p.TotalKB = Convert.ToString(rd["Total_KB"]).Trim();
                        p.EquipmentNO = rd.GetString(ordEquipmentNO).Trim();
						p.EquipManageFG = rd.GetInt32(ordEquipManageFG);
						p.MaterialCD = rd.GetString(ordMaterialCD).Trim();
						p.WithoutFileFmt_FG = rd.GetBoolean(ordWithoutFmt);
						p.UnManageTrend_FG = rd.GetBoolean(ordUnManage);
                        p.ResinGroupCD = rd.GetString(ordResinGroupCD).Trim();
                        p.ResinGroupManageFG = rd.GetInt32(ordResinGroupManageFG);
                        p.ModelNM = rd.GetString(ordModelNM).Trim();
                        if (!rd.IsDBNull(ordRefQcParamNO))
                        {
                            p.RefQcParamNO = rd.GetInt32(ordRefQcParamNO);
                        }
                        else
                        {
                            p.RefQcParamNO = null;
                        }

                        if (!rd.IsDBNull(rd.GetOrdinal(ordDieKB)))
                        {
                            p.DieKB = rd.GetString(rd.GetOrdinal(ordDieKB)).Trim();
                        }
                        
                        p.ParameterVAL = Convert.ToString(rd["Parameter_VAL"]).Trim();
						if (rd["Parameter_MAX"] != System.DBNull.Value)
						{
							p.ParameterMAX = Convert.ToDecimal(rd["Parameter_MAX"]);
						}
						else
						{
							p.ParameterMAX = null;
						}

						if (rd["Parameter_MIN"] != System.DBNull.Value)
						{
							p.ParameterMIN = Convert.ToDecimal(rd["Parameter_MIN"]);
						}
						else
						{
							p.ParameterMIN = null;
						}

                        p.DsFG = Convert.ToBoolean(rd["DS_FG"]);

						if (rd["InnerUpperLimit"] != System.DBNull.Value)
						{
							p.InnerUpperLimit = Convert.ToDecimal(rd["InnerUpperLimit"]);
						}
						else
						{
							p.InnerUpperLimit = null;
						}

						if (rd["InnerLowerLimit"] != System.DBNull.Value)
						{
							p.InnerLowerLimit = Convert.ToDecimal(rd["InnerLowerLimit"]);
						}
						else
						{
							p.InnerLowerLimit = null;
						}

                        p.ProgramNmFG = rd.GetBoolean(ordProgramNmFG);
                        if (rd.IsDBNull(ordChipNM) == false)
                        {
                            p.ChipNM = rd.GetString(ordChipNM).Trim();
                        }
                        retv.Add(p);
                    }
                }
            }

            return retv;
        }

		public static bool HasDecimalLimit(Plm plmInfo)
		{
			if (plmInfo.ParameterMAX.HasValue)
			{
				return true;
			}
			else if (plmInfo.ParameterMIN.HasValue)
			{
				return true;
			}

			return false;
		}

		public static Plm GetDataOld(List<Plm> plmList, int qcParamNo, string typeCd, string equipCd)
		{
			List<Plm> targetPlmList = plmList.FindAll(p => p.QcParamNO == qcParamNo);

			if(targetPlmList.Count == 0)
			{
				return null;
			}

			Plm targetPlm = targetPlmList[0];

			if (Convert.ToBoolean(targetPlm.EquipManageFG))
			{
				try
				{
					targetPlm = targetPlmList.Single(p => p.EquipmentNO == equipCd);
				}
				catch(Exception err)
				{
					return null;
				}

				if (string.IsNullOrEmpty(equipCd) == false && targetPlm.EquipmentNO == equipCd)
				{
					return targetPlm;
				}
				else
				{
					return null;
				}
			}
			else
			{
				try
				{
					targetPlm = targetPlmList.Single();
				}
				catch (Exception err)
				{
					AlertLog alertLog = AlertLog.GetInstance();
					alertLog.logMessageQue.Enqueue(string.Format("設備:{0} ﾀｲﾌﾟ:{1} 管理番号:{2}で閾値を1件に限定出来ませんでした。左記条件で閾値が全く存在しないか、複数存在する事がｴﾗｰ原因です。"));

					return null;
				}
				return targetPlm;
			}
		}

        // Ver 3.70.0 引数追加 → string resingroupcd (樹脂グループ) 
        public static Plm GetData(int inlineCD, List<Plm> plmList, int qcParamNo, string typeCD, string equipCD, string modelNM, string identifierCD, string resingroupcd)
		{
            //QcParamNo、MaterialCD、ModelNm、チップ/作業で絞り込む
            List<Plm> targetPlmList = plmList.FindAll(p => p.QcParamNO == qcParamNo && p.MaterialCD == typeCD);
            string errMsg = $"ﾀｲﾌﾟ:『{typeCD}』 管理番号:『{qcParamNo}』 チップ/作業:『{identifierCD}』";

			if (targetPlmList.Count == 0)
			{
				return null;
			}

			Plm targetPlm = targetPlmList[0];
            #region
            // 樹脂グループの絞込み
            if (Convert.ToBoolean(targetPlm.ResinGroupManageFG))
            {
                // 更に樹脂グループで絞り込む
                targetPlmList = GetParameterFromResinGroup(targetPlmList, resingroupcd.Split(','));
                if (targetPlmList.Any() == false)
                {
                    throw new ApplicationException($"投入ロットの製品型番:{typeCD}、管理番号:{qcParamNo}、樹脂グループ:{resingroupcd}で閾値マスタを検索しましたが、一致する閾値が見つかりませんでした。");
                }

                errMsg = $"樹脂Gr:『{resingroupcd}』 {errMsg}";
            }
         
            #endregion
            if (Convert.ToBoolean(targetPlm.EquipManageFG))
            {
                //QcParamNo、MaterialCD、ModelNm、チップ/作業で絞った結果を設備でさらに絞り込む
                targetPlmList = targetPlmList.Where(p => p.EquipmentNO == equipCD).ToList();
                if (targetPlmList.Count() == 1 && string.IsNullOrWhiteSpace(equipCD) == false)
                {
                    return targetPlmList.SingleOrDefault();
                }
                else if (targetPlmList.Count > 1)
                {
                    errMsg += string.Format(
                        "設備:『{0}』 ﾀｲﾌﾟ:『{1}』 管理番号:『{2}』 チップ/作業:『{3}』で閾値を1件に限定出来ませんでした。左記条件で、閾値が複数存在する事がｴﾗｰ原因です。"
                        , equipCD, typeCD, qcParamNo, identifierCD);

                    throw new ApplicationException(errMsg);
                }
                else
                {
                    //設備で絞ってレコードが無くなった場合
                    return null;
                }
            }
            else
            {
                if (targetPlmList.Count == 1)
                {
                    return targetPlmList.Single();
                }
                else if (targetPlmList.Count > 1)
                {
                    errMsg += $"ﾀｲﾌﾟ:『{typeCD}』 管理番号:『{qcParamNo}』 チップ/作業:『{identifierCD}』で閾値を1件に限定出来ませんでした。左記条件で、閾値が複数存在する事がｴﾗｰ原因です。";

                    throw new ApplicationException(errMsg);
                }
                else
                {
                    return null;
                }
            }
        }

		public static List<Plm> GetData(int lineCd, string typeCd, string modelNm, string equipCd, string chipNm, string constr)
		{
			return GetData(lineCd, typeCd, modelNm, equipCd, chipNm, null, constr);
		}

		public static List<Plm> GetData(int lineCd, string typeCd, string modelNm, string equipCd, string chipNm, int? timingNo, string constr)
		{
			List<Plm> retv = new List<Plm>();

			using (DBConnect conn = DBConnect.CreateInstance(constr, "System.Data.SqlClient", false))
			{
				string sql = @" SELECT TmPRM.QcParam_NO, TmPRM.Manage_NM, TmPRM.Parameter_NM, TmPRM.Total_KB, TmPRM.Die_KB, TmPLM.Parameter_MAX, 
									TmPLM.Parameter_MIN, TmPLM.Parameter_VAL, TmPLM.DS_FG, TmPRM.RefQcParam_NO, TmPLM.Equipment_NO, TmPRM.EquipManage_FG, 
                                    TmPLM.InnerUpperLimit, TmPLM.InnerLowerLimit, TmPLM.Material_CD, TmPLM.ParamGetUpperCond, TmPLM.ParamGetLowerCond
                                FROM TmPLM WITH(nolock) 
                                INNER JOIN TmPRM WITH(nolock) ON TmPLM.QcParam_NO = TmPRM.QcParam_NO 
                                WHERE (TmPLM.Material_CD = @MaterialCD) 
                                AND (TmPLM.Del_FG = 0) AND (TmPRM.Del_FG = 0) ";

				if (string.IsNullOrEmpty(modelNm) == false)
				{
					sql += " AND (TmPLM.Model_NM = @ModelNM) ";
					conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNm);
				}

				if (string.IsNullOrEmpty(chipNm) == false)
				{
					sql += " AND (TmPRM.Chip_NM = @ChipNM) ";
					conn.SetParameter("@ChipNM", SqlDbType.VarChar, chipNm);
				}

				if (string.IsNullOrEmpty(equipCd) == false)
				{
					sql += " AND (TmPLM.Equipment_NO = @EquipCD) ";
					conn.SetParameter("@EquipCD", SqlDbType.VarChar, equipCd);
				}

				if (timingNo.HasValue)
				{
					sql += " AND (TmPRM.Timing_NO = @Timing_NO) ";
					conn.SetParameter("@Timing_NO", SqlDbType.Int, timingNo.Value);
				}

				conn.SetParameter("@MaterialCD", SqlDbType.Char, typeCd);

				using (DbDataReader rd = conn.GetReader(sql))
				{
					string ordDieKB = "Die_KB";
					int ordRefQcParamNO = rd.GetOrdinal("RefQcParam_NO");
					int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
					int ordEquipManageFG = rd.GetOrdinal("EquipManage_FG");
					int ordMaterialCD = rd.GetOrdinal("Material_CD");
					int ordParamGetUpperCond = rd.GetOrdinal("ParamGetUpperCond");
					int ordParamGetLowerCond = rd.GetOrdinal("ParamGetLowerCond");

					while (rd.Read())
					{
						Plm p = new Plm();
						p.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
						p.ManageNM = Convert.ToString(rd["Manage_NM"]).Trim();
						p.ParameterNM = Convert.ToString(rd["Parameter_NM"]).Trim();
						p.TotalKB = Convert.ToString(rd["Total_KB"]).Trim();
						p.EquipmentNO = rd.GetString(ordEquipmentNO).Trim();
						p.EquipManageFG = rd.GetInt32(ordEquipManageFG);
						p.MaterialCD = rd.GetString(ordMaterialCD).Trim();

						if (!rd.IsDBNull(ordRefQcParamNO))
						{
							p.RefQcParamNO = rd.GetInt32(ordRefQcParamNO);
						}
						else
						{
							p.RefQcParamNO = null;
						}

						if (!rd.IsDBNull(rd.GetOrdinal(ordDieKB)))
						{
							p.DieKB = rd.GetString(rd.GetOrdinal(ordDieKB)).Trim();
						}

						p.ParameterVAL = Convert.ToString(rd["Parameter_VAL"]).Trim();
						if (rd["Parameter_MAX"] != System.DBNull.Value)
						{
							p.ParameterMAX = Convert.ToDecimal(rd["Parameter_MAX"]);
						}
						if (rd["Parameter_MIN"] != System.DBNull.Value)
						{
							p.ParameterMIN = Convert.ToDecimal(rd["Parameter_MIN"]);
						}
						p.DsFG = Convert.ToBoolean(rd["DS_FG"]);

						if (rd["InnerUpperLimit"] != System.DBNull.Value)
						{
							p.InnerUpperLimit = Convert.ToDecimal(rd["InnerUpperLimit"]);
						}
						if (rd["InnerLowerLimit"] != System.DBNull.Value)
						{
							p.InnerLowerLimit = Convert.ToDecimal(rd["InnerLowerLimit"]);
						}

						if (rd.IsDBNull(ordParamGetUpperCond))
						{
							p.ParamGetUpperCond = null;
						}
						else
						{
							p.ParamGetUpperCond = rd.GetFloat(ordParamGetUpperCond);
						}

						if (rd.IsDBNull(ordParamGetLowerCond))
						{
							p.ParamGetLowerCond = null;
						}
						else
						{
							p.ParamGetLowerCond = rd.GetFloat(ordParamGetLowerCond);
						}

						retv.Add(p);
					}
				}
			}

			return retv;
		}

		public static List<int> ParamGetTargetNoList(string modelNm, string chipNm, string typeCd, int? timingNo, string constr)
		{
			List<int> retv = new List<int>();

			using (DBConnect conn = DBConnect.CreateInstance(constr, "System.Data.SqlClient", false))
			{
				string sql = @" SELECT TmPRM.QcParam_NO, TmPRM.Manage_NM, TmPRM.Parameter_NM, TmPRM.Total_KB, TmPRM.Die_KB, TmPLM.Parameter_MAX, 
									TmPLM.Parameter_MIN, TmPLM.Parameter_VAL, TmPLM.DS_FG, TmPRM.RefQcParam_NO, TmPLM.Equipment_NO, TmPRM.EquipManage_FG, 
                                    TmPLM.InnerUpperLimit, TmPLM.InnerLowerLimit, TmPLM.Material_CD, TmPLM.ParamGetUpperCond, TmPLM.ParamGetLowerCond
                                FROM TmPLM WITH(nolock) 
                                INNER JOIN TmPRM WITH(nolock) ON TmPLM.QcParam_NO = TmPRM.QcParam_NO 
                                WHERE (TmPLM.Material_CD = @MaterialCD) 
                                AND (TmPLM.Del_FG = 0) AND (TmPRM.Del_FG = 0) ";

				if (string.IsNullOrEmpty(modelNm) == false)
				{
					sql += " AND (TmPLM.Model_NM = @ModelNM) ";
					conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNm);
				}

				if (string.IsNullOrEmpty(chipNm) == false)
				{
					sql += " AND (TmPRM.Chip_NM = @ChipNM) ";
					conn.SetParameter("@ChipNM", SqlDbType.VarChar, chipNm);
				}

				if (timingNo.HasValue)
				{
					sql += " AND (TmPRM.Timing_NO = @Timing_NO) ";
					conn.SetParameter("@Timing_NO", SqlDbType.Int, timingNo.Value);
				}

				conn.SetParameter("@MaterialCD", SqlDbType.Char, typeCd);

				using (DbDataReader rd = conn.GetReader(sql))
				{
					while (rd.Read())
					{
						int paramNo = Convert.ToInt32(rd["QcParam_NO"]);

						retv.Add(paramNo);
					}
				}
			}
			return retv;
		}

		public static void CheckEquipManageParam(List<Plm> plmList, ref string errMsg)
		{
			string localErrMsg = string.Empty;

			//設備CD別管理の仕様上許可されない問題が発生していないかをチェック

			#region 設備CDを管理する状況で空白レコードがあるかどうかのチェック
			List<Plm> equipManagePlmList = plmList.Where(p => p.EquipManageFG != 0).ToList();
			List<string> typeList = equipManagePlmList.Select(e => e.MaterialCD).Distinct().ToList();
			List<int> paramNoList = equipManagePlmList.Select(e => e.QcParamNO).Distinct().ToList();

			//設備CD管理のパラメタは空白を一切許可しない。空白レコードがあったらエラー
			foreach (int qcParamNo in paramNoList)
			{
				string paramNm = equipManagePlmList.Where(e => e.QcParamNO == qcParamNo).Select(e => e.ParameterNM).Distinct().Single();
				//品種、パラメタNo毎の全装置分のパラメタリスト
				List<Plm> equipManagePlmPerParam = equipManagePlmList.Where(e => e.QcParamNO == qcParamNo).ToList();

				foreach (string type in typeList)
				{
					int equipEmptyRecordCt = equipManagePlmPerParam.Where(e => e.MaterialCD == type && string.IsNullOrEmpty(e.EquipmentNO)).Count();

					if (equipEmptyRecordCt > 0)
					{
						if (string.IsNullOrEmpty(localErrMsg))
						{
							localErrMsg += "設備CD別管理が【必要】なパラメータで【設備CD指定の無い】規格が存在します。\r\n";
						}
						localErrMsg += string.Format("\tタイプ:{0} / パラメータ番号:{1} / パラメータ名:{2}\r\n", type, qcParamNo, paramNm);
					}
				}
			}
			#endregion

			#region 設備CDを管理しない状況で設備CD指定レコードがあるかどうかのチェック
			List<Plm> nonEquipManagePlmList = plmList.Where(p => p.EquipManageFG == 0).ToList();
			typeList = nonEquipManagePlmList.Select(e => e.MaterialCD).Distinct().ToList();
			paramNoList = nonEquipManagePlmList.Select(e => e.QcParamNO).Distinct().ToList();

			bool addedErrMsg = false;

			//設備CD管理をしないパラメタは空白でなければならない。設備CD指定のレコードがあったらエラー
			foreach (int qcParamNo in paramNoList)
			{
				string paramNm = nonEquipManagePlmList.Where(e => e.QcParamNO == qcParamNo).Select(e => e.ParameterNM).Distinct().Single();

				List<Plm> nonEquipManagePlmPerParam = nonEquipManagePlmList.Where(e => e.QcParamNO == qcParamNo).ToList();

				foreach (string type in typeList)
				{
					int equipManageRecordCt = nonEquipManagePlmList.Where(e => e.MaterialCD == type &&
						string.IsNullOrEmpty(e.EquipmentNO) == false).Count();

					if (equipManageRecordCt > 0)
					{
						if (addedErrMsg == false)
						{
							localErrMsg += "設備CD別管理が【不要】なパラメータで【設備CD指定されている】規格が存在します。\r\n";
							addedErrMsg = true;
						}
						localErrMsg += string.Format("\tタイプ:{0} / パラメータ番号:{1} / パラメータ名:{2}\r\n", type, qcParamNo, paramNm);
					}
				}
			}
			#endregion

			errMsg += localErrMsg;
		}

		/// <summary>
		/// TmPLMの設備CDの値がTmEquiに存在するものか確認する関数。存在しない場合メッセージだけで処理の停止はしない
		/// </summary>
		/// <param name="plmList"></param>
		/// <param name="errMsg"></param>
		public static void ExistEquipOnParam(int lineCD, List<Plm> plmList, ref string errMsg)
		{
			string localErrMsg = string.Empty;

			foreach (Plm plmInfo in plmList)
			{
				if (string.IsNullOrEmpty(plmInfo.EquipmentNO.Trim()))
				{
					continue;
				}

				List<Equi> equiList = Equi.GetEquipmentInfo(lineCD, plmInfo.EquipmentNO, plmInfo.ModelNM, false);

				if (equiList.Count == 0)
				{
					if (string.IsNullOrEmpty(localErrMsg))
					{
						localErrMsg += ("設備マスタに未追加の装置が設定されています。設備マスタ担当者へ確認して下さい。\r\n");
					}

					localErrMsg += string.Format("    タイプ：{0}　パラメタ番号：{1}　パラメタ名：{2}　設備CD：{3}\r\n", plmInfo.MaterialCD, plmInfo.QcParamNO, plmInfo.ParameterNM, plmInfo.EquipmentNO);
				}
			}

			if (string.IsNullOrEmpty(localErrMsg))
			{
			}
			else
			{
				AlertLog alertLog = AlertLog.GetInstance();
				alertLog.logMessageQue.Enqueue(localErrMsg);
				//errMsg += localErrMsg;
			}
		}

		public static bool HasProblem(int lineCD, Plm plm, ref string errMsg)
		{
			List<Plm> plmList = new List<Plm>();

			plmList.Add(plm);

			return HasProblem(lineCD, plmList, ref errMsg);

		}

		/// <summary>
		/// Plmの内容に問題が無いかチェックし問題が有ればTrueを返す
		/// </summary>
		/// <param name="plmList"></param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public static bool HasProblem(int lineCD, List<Plm> plmList, ref string errMsg)
		{
			CheckEquipManageParam(plmList, ref errMsg);

			ExistEquipOnParam(lineCD, plmList, ref errMsg);

			if (string.IsNullOrEmpty(errMsg))
			{
				//Prmは問題を抱えて無いのでfalse
				return false;
			}
			else
			{
				//問題がある為、true
				return true;
			}
		}

        public static Plm GetCurrentFromMultipleServer(int lineCD, string typeCd, string modelNm, int qcParamNo, string equipmentNo, bool isIncludeNascaParam, string chipNm, bool referMultiServerFG)
        {
            if (string.IsNullOrEmpty(typeCd) || qcParamNo == 0)
            {
                throw new ApplicationException(
                    string.Format("Plm.GetData()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1} QcParamNo:{2}", typeCd, modelNm, qcParamNo));
            }

            List<Plm> plmList = GetDatas(lineCD, typeCd, modelNm, qcParamNo, isIncludeNascaParam, chipNm);

            if (plmList == null || plmList.Count() == 0)
            {
                if (referMultiServerFG)
                {

                    SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

                    foreach (string serverNm in commonSettingInfo.ArmsServerList)
                    {
                        List<Plm> plmAddList = GetDatas(lineCD, typeCd, modelNm, qcParamNo, isIncludeNascaParam, chipNm, ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, serverNm));
                        if (plmAddList.Count > 0)
                        {
                            plmList.AddRange(plmAddList);
                            break;
                        }
                    }
                }
            }

            return GetData(lineCD, plmList, typeCd, modelNm, equipmentNo, qcParamNo);
        }
        
        public static string GetProgramName(string typeCd, int lineCd, string modelNm)
        {
            return GetProgramName(typeCd, lineCd, modelNm, null, null);
        }

        public static string GetProgramName(string typeCd, int lineCd, string modelNm, string workCd, string plantCd)
        {
            List<Plm> plmList = GetData(lineCd, typeCd, modelNm, string.Empty);

            plmList = plmList.Where(p => p.ProgramNmFG == true).ToList();

            if (string.IsNullOrEmpty(workCd) == false && plmList.Exists(p => string.IsNullOrEmpty(p.ChipNM) == false))
            {
                plmList = plmList.Where(p => p.ChipNM == workCd).ToList();
            }

            if (string.IsNullOrEmpty(plantCd) == false && plmList.Exists(p => p.EquipManageFG == 1))
            {
                if (plmList.Exists(p => p.EquipManageFG == 1 && string.IsNullOrEmpty(p.EquipmentNO) == true))
                {
                    throw new ApplicationException($"プログラム名の取得で号機管理対象(TmPRM.EquipManageFG=true)だが、閾値マスタで設備毎にプログラム名の設定がされていません。製品型番:{typeCd}");
                }
                
                plmList = plmList.Where(p => p.EquipmentNO == plantCd).ToList();
            }
            
            if (plmList.Count >= 2)
            {
                throw new ApplicationException($"閾値設定で複数のプログラムNoが設定されています。閾値マスタ設定者に問い合わせ、修正して下さい。対象型式:{modelNm} 型番:{typeCd}");
            }
            else if (plmList.Count == 0)
            {
                throw new ApplicationException($"閾値設定でプログラムNoが未設定です。閾値マスタ設定者に問い合わせ、設定して下さい。対象型式:{modelNm} 型番:{typeCd}");
            }
            else
            {
                return plmList.Single().ParameterVAL;
            }
        }

        /// <summary>
        /// Plmリストを指定の樹脂グループリストと一致するものに絞る
        /// </summary>
        /// <param name="plmList">Plmリスト</param>
        /// <param name="searchResinGroupList">指定の樹脂グループリスト</param>
        /// <returns></returns>
        public static List<Plm> GetParameterFromResinGroup(List<Plm> plmList, string[] searchResinGroupList)
        {
            List<Plm> retv = new List<Plm>();
            foreach(Plm p in plmList)
            {
                if (string.IsNullOrEmpty(p.ResinGroupCD))
                    continue;

                int[] list; int[] searchList;
                try
                {
                    list = p.ResinGroupCD.Split(',').Select(r => int.Parse(r)).ToArray();
                    searchList = searchResinGroupList.Select(r => int.Parse(r)).ToArray();
                }
                catch (Exception)
                {
                    throw new ApplicationException($@"閾値マスタの「樹脂Gr」列に数値でない文字が含まれています。閾値マスタを修正して下さい。 型式:{p.ModelNM} 管理番号:{p.QcParamNO} 製品型番:{p.MaterialCD} NASCA特性値:{string.Join(",", searchResinGroupList)} 閾値マスタ:{p.ResinGroupCD}");
                }

                var orderList = list.OrderBy(l => l).ToArray();
                var orderSearchList = searchList.OrderBy(l => l).ToArray();

                if (orderList.SequenceEqual(orderSearchList))
                {
                    retv.Add(p);
                }
            }
            return retv;
        }
    }
}
