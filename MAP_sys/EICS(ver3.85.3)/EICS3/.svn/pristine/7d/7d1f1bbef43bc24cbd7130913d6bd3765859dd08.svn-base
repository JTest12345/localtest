using SLCommonLib.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;

namespace EICS.Database
{
    class FileFmt
    {
        public string ModelNM { get; set; }
        public int QcParamNO { get; set; }
        public string PrefixNM { get; set; }
        public int? ColumnNO { get; set; }
        public string SearchNM { get; set; }
        public string MachinePrefixNM { get; set; }
        public bool DelFG { get; set; }
        public string UpdUserCD { get; set; }
        public DateTime LastUpdDT { get; set; }
		public string HeaderNM { get; set; }
		public bool StartUpFG { get; set; }
		public string EquipPartID { get; set; }
		public string XPath { get; set; }
        public int XPath_SearchNO { get; set; }

        //public static List<FileFmt> GetData(LSETInfo lsetInfo, int timingNO)
        //{
        //	return GetData(lsetInfo, timingNO, null);
        //}

		public static List<FileFmt> GetData(LSETInfo lsetInfo, bool? isStartUp, string prefixNM, int? qcParamNO)
		{
			return GetData(lsetInfo.InlineCD, lsetInfo.ModelNM, isStartUp, prefixNM, qcParamNO);
		}

        /// <summary>
        /// 設備ファイル識別文字を取得
        /// </summary>
        /// <param name="modelNM">型式</param>
        /// <param name="timingNO">タイミングNO</param>
        /// <returns>ファイル識別, 設備識別</returns>
        public static List<FileFmt> GetData(int inlineCD, string modelNM, bool? isStartUp, string prefixNM, int? qcParamNO)
        {
            System.Data.Common.DbDataReader rd = null;
            List<FileFmt> fileFmtList = new List<FileFmt>();

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, inlineCD), "System.Data.SqlClient", false))
                {
                    string sql = "";


					sql = @" SELECT TmFILEFMT.Model_NM, TmFILEFMT.QcParam_NO, TmFILEFMT.Prefix_NM, TmFILEFMT.Column_NO, TmFILEFMT.Search_NM, TmFILEFMT.XPath, TmFILEFMT.XPath_SearchNO,
										TmFILEFMT.MachinePrefix_NM, TmFILEFMT.Del_FG, TmFILEFMT.UpdUser_CD, TmFILEFMT.LastUpd_DT, TmFILEFMT.Header_NM, TmFILEFMT.StartUp_FG, TmFILEFMT.EquipPart_ID
									FROM TmFILEFMT WITH(nolock)
									WHERE (TmFILEFMT.Del_FG = 0) 
									AND (TmFILEFMT.Model_NM = @ModelNM) ";

                    //sql += " GROUP BY TmFILEFMT.Prefix_NM, MachinePrefix_NM ";
                    conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM);

                    if (isStartUp.HasValue)
                    {
                        sql += " AND (TmFILEFMT.StartUp_FG = @StartUpFG) ";
                        conn.SetParameter("@StartUpFG", SqlDbType.Bit, isStartUp.Value);
                    }

                    if (string.IsNullOrEmpty(prefixNM) == false)
                    {
                        sql += " AND (TmFILEFMT.Prefix_NM = @PrefixNM) ";
                        conn.SetParameter("@PrefixNM", SqlDbType.VarChar, prefixNM);
                    }

                    if (qcParamNO.HasValue)
                    {
                        sql += " AND (TmFILEFMT.QcParam_NO = @QcParamNO) ";
                        conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNO.Value);
                    }

                    using (rd = conn.GetReader(sql))
                    {
                        int ordModelNM = rd.GetOrdinal("Model_NM");
                        int ordQcParamNO = rd.GetOrdinal("QcParam_NO");
                        int ordPrefixNM = rd.GetOrdinal("Prefix_NM");
                        int ordColumnNO = rd.GetOrdinal("Column_NO");
                        int ordSearchNM = rd.GetOrdinal("Search_NM");
                        int ordMacPrefixNM = rd.GetOrdinal("MachinePrefix_NM");
                        int ordDelFG = rd.GetOrdinal("Del_FG");
                        int ordUpdUserCD = rd.GetOrdinal("UpdUser_CD");
                        int ordLastUpdDT = rd.GetOrdinal("LastUpd_DT");
						int ordHeaderNM = rd.GetOrdinal("Header_NM");
						int ordStartUpFG = rd.GetOrdinal("StartUp_FG");
						int ordEquipPartID = rd.GetOrdinal("EquipPart_ID");
						int ordXPath = rd.GetOrdinal("XPath");
                        int ordXPath_SearchNO = rd.GetOrdinal("XPath_SearchNO");

						while (rd.Read())
                        {
                            FileFmt fileFmt = new FileFmt();

							fileFmt.ModelNM = rd.GetString(ordModelNM).Trim();
                            fileFmt.QcParamNO = rd.GetInt32(ordQcParamNO);
							fileFmt.PrefixNM = rd.GetString(ordPrefixNM).Trim();

                            if (rd.IsDBNull(ordColumnNO) == false)
                            {
                                fileFmt.ColumnNO = rd.GetInt32(ordColumnNO);
                            }
                            else
                            {
                                fileFmt.ColumnNO = null;
                            }

                            if (rd.IsDBNull(ordSearchNM) == false)
                            {
                                fileFmt.SearchNM = rd.GetString(ordSearchNM);
                            }
                            else
                            {
                                fileFmt.SearchNM = null;
                            }

                            if (rd.IsDBNull(ordMacPrefixNM) == false)
                            {
								fileFmt.MachinePrefixNM = rd.GetString(ordMacPrefixNM).Trim();
                            }
                            else
                            {
                                fileFmt.MachinePrefixNM = null;
                            }

                            fileFmt.DelFG = rd.GetBoolean(ordDelFG);
                            fileFmt.UpdUserCD = rd.GetString(ordUpdUserCD);
                            fileFmt.LastUpdDT = rd.GetDateTime(ordLastUpdDT);

							if (rd.IsDBNull(ordHeaderNM) == false)
							{
								fileFmt.HeaderNM = rd.GetString(ordHeaderNM).Trim();
							}
							else
							{
								fileFmt.HeaderNM = null;
							}

							fileFmt.StartUpFG = rd.GetBoolean(ordStartUpFG);
							fileFmt.EquipPartID = rd.GetString(ordEquipPartID).Trim();

							if (rd.IsDBNull(ordXPath) == false)
							{
								fileFmt.XPath = rd.GetString(ordXPath).Trim();
							}
							else
							{
								fileFmt.XPath = string.Empty;
							}

                            if (rd.IsDBNull(ordXPath_SearchNO) == false)
                            {
                                fileFmt.XPath_SearchNO = rd.GetInt32(ordXPath_SearchNO);
                            }
                            else
                            {
                                fileFmt.XPath_SearchNO = 0;
                            }

							fileFmtList.Add(fileFmt);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw;
            }

            return fileFmtList;
        }

        /// <summary>
        /// 設備ファイル識別文字を取得(Database.FILEFMT.csの仕様を推奨
        /// </summary>
        /// <param name="modelNM">型式</param>
        /// <param name="timingNO">タイミングNO</param>
        /// <returns>ファイル識別, 設備識別</returns>
        public static Dictionary<string, string> GetMachineFilePrefix(LSETInfo lsetInfo, int timingNO, bool? isStartUp, bool? isTargetXPathSetting)
        {
            System.Data.Common.DbDataReader rd = null;
            Dictionary<string, string> prefixList = new Dictionary<string, string>();

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
                {
                    string sql = "";
                    string sqlWhere = "";

                    if (timingNO != 0)
                    {
                        sqlWhere += " AND (TmPRM.Timing_NO = @TimingNO) ";
                        conn.SetParameter("@TimingNO", SqlDbType.Int, timingNO);
                    }

                    if (isStartUp.HasValue)
                    {
                        sqlWhere += " AND (TmFILEFMT.StartUp_FG = @StartUpFG) ";
                        conn.SetParameter("@StartUpFG", SqlDbType.Bit, isStartUp.Value);
                    }

					if (isTargetXPathSetting.HasValue)
					{
						if (isTargetXPathSetting.Value)
						{
							sqlWhere += " AND (TmFILEFMT.XPath IS NOT NULL) ";
						}
						else
						{
							sqlWhere += " AND (TmFILEFMT.XPath IS NULL) ";
						}
					}

                    if (lsetInfo.AssetsNM == Constant.ASSETS_WB_NM)
                    {
                        sql = @" SELECT TmFILEFMT.Prefix_NM, '' as MachinePrefix_NM
                                FROM TmFILEFMT_WB AS TmFILEFMT WITH(nolock)
                                INNER JOIN TmPRM ON TmFILEFMT.QcParam_NO = TmPRM.QcParam_NO
                                WHERE (TmPRM.Del_FG = 0) AND (TmFILEFMT.Del_FG = 0) " + sqlWhere;
                        sql += " GROUP BY TmFILEFMT.Prefix_NM ";
                    }
                    else
                    {
                        sql = @" SELECT TmFILEFMT.Prefix_NM, TmFILEFMT.MachinePrefix_NM
                                FROM TmFILEFMT WITH(nolock)
                                INNER JOIN TmPRM ON TmFILEFMT.QcParam_NO = TmPRM.QcParam_NO
                                WHERE (TmPRM.Del_FG = 0) AND (TmFILEFMT.Del_FG = 0) 
                                AND (TmFILEFMT.Model_NM = @ModelNM) " + sqlWhere;
                        sql += " GROUP BY TmFILEFMT.Prefix_NM, MachinePrefix_NM ";
                        conn.SetParameter("@ModelNM", SqlDbType.VarChar, lsetInfo.ModelNM);
                    }

                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            prefixList.Add(Convert.ToString(rd["Prefix_NM"]), Convert.ToString(rd["MachinePrefix_NM"]));
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw;
            }

            return prefixList;
        }

        public static string GetMachinePrefixNM(List<FileFmt> fileFmtList, string prefixNM)
        {
            return fileFmtList.First(f => f.PrefixNM == prefixNM && string.IsNullOrEmpty(f.MachinePrefixNM) == false).MachinePrefixNM;
        }

        public static string GetMachinePrefixNM(FileFmt fileFmt, string prefixNM)
        {
            if (fileFmt.PrefixNM == prefixNM && string.IsNullOrEmpty(fileFmt.MachinePrefixNM) == false)
            {
                return fileFmt.MachinePrefixNM;
            }

            return null;
        }

        public static bool IsStartFile(LSETInfo lsetInfo, string prefixNm)
        {
            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT StartUp_FG FROM TmFILEFMT WITH(nolock)
                                WHERE Model_NM = @ModelNM AND Prefix_NM = @PrefixNM ";

                conn.SetParameter("@ModelNM", SqlDbType.VarChar, lsetInfo.ModelNM);
                conn.SetParameter("@PrefixNM", SqlDbType.VarChar, prefixNm);

                object flag = conn.ExecuteScalar(sql);
                if (flag == null)
                {
                    throw new ApplicationException(
                        string.Format("管理されていないファイルが出力されている為、開始時に出力されるファイルか特定できませんでした。ファイル識別文字：{0}", prefixNm));
                }

                if (Convert.ToBoolean(flag))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="fileFmt"></param>
		/// <param name="plm"></param>
		/// <returns></returns>
		public static FileData GetFileData(XElement xmlDoc, FILEFMTInfo fileFmt, Plm plm, string targetFilePath)
		{
            if(string.IsNullOrWhiteSpace(fileFmt.XPath))
            {
                throw new ApplicationException($"XPathが設定されていません。管理項目『{plm.QcParamNO}/{plm.ManageNM}』");
            }

            if (fileFmt.XPath_SearchNO == 0)
            {
                throw new ApplicationException($"XPath_SearchNOが設定されていないか、0になっています。管理項目『{plm.QcParamNO}/{plm.ManageNM}』");
            }

            FileData retv = new FileData();

            List<string> xTags = fileFmt.XPath.Split(',').ToList();

            //XPathへの記載順でタグを順番に絞り込みする。
            var targetRecord = xmlDoc.Elements().Where(x => x.Name.LocalName == xTags[0]);
            for(int i = 1; i < xTags.Count; i ++)
            {
                string tag = xTags[i];
                targetRecord = targetRecord.Elements().Where(x => x.Name.LocalName == tag);
            }

            //データが複数取りえるので、foreachで回してXPath_SearchNOと一致する回数目のデータを取得。
            int readCt = 1;
            string targetStr = string.Empty;

            foreach (var tag in targetRecord)
            {
                if (readCt == fileFmt.XPath_SearchNO)
                {
                    targetStr = tag.Value;
                    break;
                }
                readCt++;
            }

			retv.TypeCD = plm.MaterialCD;

			if (plm.ManageNM == Constant.sOKNG)
			{
				retv.StrValue = targetStr;
				retv.RuleValue = plm.ParameterVAL;

				retv.DecValue = null;
				retv.LowerLimit = null;
				retv.UpperLimit = null;
			}
			else if (plm.ManageNM == Constant.sMAXMIN)
			{
				decimal decValue;
				if (decimal.TryParse(targetStr, out decValue) == false)
				{
					string errMsg = string.Format("次の条件でﾌｧｲﾙから取得したﾊﾟﾗﾒｰﾀが数値変換出来ない文字です。変換対象文字:{0} 管理番号:{1} XPathTags:{2} 検索順:{3} ファイルパス:{4}",
						targetStr, plm.QcParamNO, fileFmt.XPath, fileFmt.XPath_SearchNO, targetFilePath);
					throw new ApplicationException(errMsg);
				}

				retv.DecValue = decValue;

				if (plm.ParameterMIN.HasValue == false && plm.ParameterMAX.HasValue)
				{
					string errMsg = string.Format(
						"閾値の判定方法が『{0}』で以外で設定されていますが、ﾏｽﾀの上下限値が未設定です。管理番号:{1}", Constant.sOKNG, plm.QcParamNO);
					throw new ApplicationException(errMsg);
				}

				retv.UpperLimit = plm.ParameterMAX.Value;
				retv.LowerLimit = plm.ParameterMIN.Value;

				retv.StrValue = null;
				retv.RuleValue = null;
			}
			else if (plm.ManageNM == Constant.sMAX)
			{
				decimal decValue;
				if (decimal.TryParse(targetStr, out decValue) == false)
				{
                    string errMsg = string.Format("次の条件でﾌｧｲﾙから取得したﾊﾟﾗﾒｰﾀが数値変換出来ない文字です。変換対象文字:{0} 管理番号:{1} XPathTags:{2} 検索順:{3} ファイルパス：{4}",
                        targetStr, plm.QcParamNO, fileFmt.XPath, fileFmt.XPath_SearchNO, targetFilePath);
                    throw new ApplicationException(errMsg);
				}

				retv.DecValue = decValue;

				if (plm.ParameterMAX.HasValue == false)
				{
					string errMsg = string.Format(
						"閾値の判定方法が『{0}』で設定されていますが、ﾏｽﾀの上限値が未設定です。管理番号:{1}", Constant.sMAX, plm.QcParamNO);
					throw new ApplicationException(errMsg);
				}

				retv.UpperLimit = plm.ParameterMAX.Value;
				retv.LowerLimit = null;

				retv.StrValue = null;
				retv.RuleValue = null;
			}

			return retv;
		}
    }


	public class FileFmtWithPlm
	{
		public FILEFMTInfo FileFmt { get; set; }
		public Plm Plm { get; set; }

		public static void CheckAllFileFmtFromParamMaster(List<Plm> plmPerTypeModelChipList, LSETInfo lsetInfo, bool containWbFmt)
		{
			List<FILEFMTInfo> fileFmtList = ConnectDB.GetFILEFMTData(null, lsetInfo, true, true);
			fileFmtList.AddRange(ConnectDB.GetFILEFMTData(null, lsetInfo, false, true));

			List<FILEFMTWBInfo> fileFmtWbList = ConnectDB.GetFILEFMTWBData(null, lsetInfo.TypeCD, lsetInfo.ModelNM, lsetInfo.InlineCD);

			CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, containWbFmt, fileFmtList, fileFmtWbList);
		}

		public static void CheckAllFileFmtFromParamMaster(List<Plm> plmPerTypeModelChipList, LSETInfo lsetInfo, bool containWbFmt, List<FILEFMTInfo> fileFmtList, List<FILEFMTWBInfo> fileFmtWbList)
		{
			int masterMissingCount = 0;
			int paramMasterMissingCount = 0;
			AlertLog alertLog = AlertLog.GetInstance();

			SettingInfo settingInfo = SettingInfo.GetSingleton();

			if (containWbFmt && fileFmtWbList == null)
			{
				throw new ApplicationException($"システム設計の問題の為、システム担当者へ連絡して下さい。FileFmtWBをチェック対象に含んでいますが、チェック対象のFileFmtWBのリストが空で関数呼び出しされました。");
			}

			foreach (Plm plm in plmPerTypeModelChipList)
			{
				if (existsInFileFmt(plm.QcParamNO, fileFmtList, fileFmtWbList, containWbFmt) == false)
				{
					string errMsg = string.Format(
						"マスタ設定の問題の為、システム担当者へ連絡して下さい。閾値マスタ(TmPlm、TmPrm）にある管理番号がTmFileFmt、TmFileFmtWBいずれにも存在しません。" +
						"管理番号:『{0}』 ﾀｲﾌﾟ:『{1}』 Model:『{2}』 ﾁｯﾌﾟ/作業:『{3}』 設備:『{4}』"
						, plm.QcParamNO, plm.MaterialCD, lsetInfo.ModelNM, lsetInfo.ChipNM, lsetInfo.EquipmentNO);

					if (masterMissingCount < settingInfo.MasterMissDisplayCount)
					{
						alertLog.logMessageQue.Enqueue(errMsg);
					}
					else if (masterMissingCount == settingInfo.MasterMissDisplayCount)
					{
						alertLog.logMessageQue.Enqueue(errMsg);

						errMsg = string.Format("マスタ設定に関する同様の問題が{0}件発生した為、ログファイルにのみエラーログ出力します。", settingInfo.MasterMissDisplayCount);
						alertLog.logMessageQue.Enqueue(errMsg);
					}
					else
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, errMsg);
					}

					masterMissingCount++;
				}
			}

			List<Plm> plmListForChkUnManage = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, true, lsetInfo.ChipNM
					, ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), false);

			foreach (Plm plm in plmListForChkUnManage.FindAll(p => p.UnManageTrend_FG == true))
			{
				string errMsg = "マスタの設定に問題がある為、システム担当者へ連絡して下さい。\r\n"
					+ "パラメータマスタ(TmPrm)でUnManageTrend_FG=true、WithoutFileFmt_FG=falseの設定は禁止されています。\r\n"
					+ $"管理番号:『{plm.QcParamNO}』";

				if (paramMasterMissingCount < settingInfo.MasterMissDisplayCount)
				{
					alertLog.logMessageQue.Enqueue(errMsg);
				}
				else if (paramMasterMissingCount == settingInfo.MasterMissDisplayCount)
				{
					alertLog.logMessageQue.Enqueue(errMsg);

					errMsg = string.Format("マスタ設定に関する同様の問題が{0}件発生した為、ログファイルにのみエラーログ出力します。", settingInfo.MasterMissDisplayCount);
					alertLog.logMessageQue.Enqueue(errMsg);
				}
				else
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, errMsg);
				}

				paramMasterMissingCount++;
			}

			if (masterMissingCount > 0 || paramMasterMissingCount > 0)
			{
				throw new ApplicationException("マスタ設定ミスがあった為、監視を停止します。");
			}
		}

		private static bool existsInFileFmt(int qcParamNo, List<FILEFMTInfo> fileFmtList, List<FILEFMTWBInfo> fileFmtWbList, bool containWbFmt)
		{
			if (fileFmtList.Exists(f => f.QCParamNO == qcParamNo) == false)
			{
				if (containWbFmt && fileFmtWbList.Exists(f => f.QCParamNO == qcParamNo))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return true;
			}
		}

        // Ver 3.70.0 引数追加 → string resingroupcd (樹脂グループ) 
        public static List<FileFmtWithPlm> GetData(LSETInfo lsetInfo, bool isStartUpFG, List<Plm> plmList, List<FILEFMTInfo> fileFmtList, string resingroupcd)
		{
			List<FileFmtWithPlm> fileFmtWithPlmList = new List<FileFmtWithPlm>();

			foreach (FILEFMTInfo fileFmt in fileFmtList)
			{
				FileFmtWithPlm fileFmtWithPlm = new FileFmtWithPlm();

				Plm plmInfo = new Plm();

				if (lsetInfo.ReferMultiServerFG)
				{
					plmInfo = Plm.GetCurrentFromMultipleServer(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, fileFmt.QCParamNO, lsetInfo.EquipmentNO, false, lsetInfo.ChipNM, lsetInfo.ReferMultiServerFG);
				}
				else
				{
					plmInfo = Plm.GetData(lsetInfo.InlineCD, plmList, fileFmt.QCParamNO, lsetInfo.TypeCD, lsetInfo.EquipmentNO, lsetInfo.ModelNM, lsetInfo.ChipNM, resingroupcd);
				}

				if (plmInfo != null && (plmInfo.ParameterMAX != null || plmInfo.ParameterMIN != null || plmInfo.ParameterVAL != null))
				{
					fileFmtWithPlm.FileFmt = fileFmt;
					fileFmtWithPlm.Plm = plmInfo;

					fileFmtWithPlmList.Add(fileFmtWithPlm);
				}
			}

			return fileFmtWithPlmList;
		}
	}

	public class FileData
	{
		public int QcParamNO { get; set; }
		public string TypeCD { get; set; }
		public string StrValue { get; set; }
		public string RuleValue { get; set; }
		public decimal? DecValue { get; set; }
		public decimal? UpperLimit { get; set; }
		public decimal? LowerLimit { get; set; }
		private bool? Result { get; set; }

		/// <summary>
		/// true:判定OK/false:判定NG
		/// </summary>
		/// <returns></returns>
		public bool GetResult()
		{
			if (this.Result.HasValue)
			{
				return this.Result.Value;
			}
			else
			{
				judge();
				return this.Result.Value;
			}
		}

		private void judge()
		{
			if (string.IsNullOrEmpty(this.StrValue) && string.IsNullOrEmpty(RuleValue))
			{
				if (this.LowerLimit.HasValue)
				{
					if (this.DecValue >= this.LowerLimit && this.DecValue <= this.UpperLimit)
					{
						this.Result = true;
					}
					else
					{
						this.Result = false;
					}
				}
				else
				{
					if (this.DecValue <= this.UpperLimit)
					{
						this.Result = true;
					}
					else
					{
						this.Result = false;
					}
				}
			}
			else
			{
				if (this.StrValue.Trim().ToUpper() == this.RuleValue.Trim().ToUpper())
				{
					this.Result = true;
				}
				else
				{
					this.Result = false;
				}
			}
		}
	}


}
