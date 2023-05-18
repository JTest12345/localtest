using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api
{
    public class MacDefect
    {
		public bool ChangeFg { get; set; }
		public string PlantCd { get; set; }
		public string LotNo { get; set; }
		public int DefAddressNo { get; set; }
		public int DefUnitNo { get; set; }
		public DateTime TargetDt { get; set; }
		public int ProcNo { get; set; }
		public string ProcNm { get; set; }
		public string DefItemCd { get; set; }
		public string DefItemNm { get; set; }
		public string DefCauseCd { get; set; }
		public string DefCauseNm { get; set; }
		public string DefClassCd { get; set; }
		public string DefClassNm { get; set; }
		public string TranCd { get; set; }
		public string UpdateDefAddressNo { get; set; }
		public string UpdateTranCd { get; set; }
		public string UpdUserCd { get; set; }
		public bool DelFg { get; set; }
		public DateTime LastUpdDt { get; set; }

		public static List<MacDefect> GetData(string plantCd, string lotNo, int? procNo, string defItemCd, string defItemNm, DateTime? fromDt, DateTime? toDt, bool includeDelete) 
		{
			List<MacDefect> retv = new List<MacDefect>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LensConnectionString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT PlantCD, LotNO, DefAddressNO, DefUnitNO, TargetDT, ProcNO, ProcNM, DefItemCD, DefItemNM, DefCauseCD, DefCauseNM, DefClassCD, DefClassNM, TranCD, 
									UpdateTranCD, DelFG, UpdUserCD, LastUpdDT
								FROM TnMacDefect WITH(nolock,INDEX(IX_LotNO))
								WHERE 1=1 ";

				if (!includeDelete)
				{
					sql += " AND DelFG = 0 ";
				}
				if (!string.IsNullOrWhiteSpace(plantCd))
				{
					sql += " AND (PlantCD = @PlantCD) ";
					cmd.Parameters.Add("@PlantCD", System.Data.SqlDbType.Char).Value = plantCd;
				}
				if (!string.IsNullOrWhiteSpace(lotNo))
				{
					sql += " AND (LotNO = @LotNO) ";
					cmd.Parameters.Add("@LotNO", System.Data.SqlDbType.VarChar).Value = lotNo;
				}

				if (procNo.HasValue) 
				{
					sql += " AND (ProcNO = @ProcNO) ";
					cmd.Parameters.Add("@ProcNO", System.Data.SqlDbType.Int).Value = procNo.Value;
				}

				if (!string.IsNullOrWhiteSpace(defItemCd))
				{
					sql += " AND (DefItemCD = @DefItemCD) ";
					cmd.Parameters.Add("@DefItemCD", System.Data.SqlDbType.Char).Value = defItemCd;
				}

				if (!string.IsNullOrWhiteSpace(defItemNm))
				{
					sql += " AND (DefItemNM Like @DefItemNM) ";
					cmd.Parameters.Add("@DefItemNM", System.Data.SqlDbType.NVarChar).Value = defItemNm + "%";
				}

				if (fromDt.HasValue)
				{
					sql += " AND (TargetDT >= @FromTargetDT) ";
					cmd.Parameters.Add("@FromTargetDT", System.Data.SqlDbType.DateTime).Value = fromDt.Value;
				}

				if (toDt.HasValue)
				{
					sql += " AND (TargetDT <= @ToTargetDT) ";
					cmd.Parameters.Add("@ToTargetDT", System.Data.SqlDbType.DateTime).Value = toDt.Value;
				}

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						MacDefect def = new MacDefect();

						def.PlantCd = rd["PlantCD"].ToString().Trim();
						def.LotNo = rd["LotNO"].ToString().Trim();
						def.DefAddressNo = Convert.ToInt32(rd["DefAddressNO"]);
						def.DefUnitNo = Convert.ToInt32(rd["DefUnitNO"]);
						def.TargetDt = Convert.ToDateTime(rd["TargetDT"]);
						def.ProcNo = Convert.ToInt32(rd["ProcNO"]);
						def.ProcNm = rd["ProcNM"].ToString().Trim();
						def.DefItemCd = rd["DefItemCD"].ToString().Trim();
						def.DefItemNm = rd["DefItemNM"].ToString().Trim();
						def.DefCauseCd = rd["DefCauseCD"].ToString().Trim();
						def.DefCauseNm = rd["DefCauseNM"].ToString().Trim();
						def.DefClassCd = rd["DefClassCD"].ToString().Trim();
						def.DefClassNm = rd["DefClassNM"].ToString().Trim();
						def.TranCd = rd["TranCD"].ToString().Trim();
						def.UpdateTranCd = rd["UpdateTranCD"].ToString().Trim();
						def.DelFg = Convert.ToBoolean(rd["DelFG"]);
						def.UpdUserCd = rd["UpdUserCD"].ToString().Trim();
						def.LastUpdDt = Convert.ToDateTime(rd["LastUpdDT"]);

						retv.Add(def);
					}
				}
			}

			return retv;
		}

		public static List<MacDefect> GetData(string lotNo)
		{
			return GetData(string.Empty, lotNo, null, string.Empty, string.Empty, null, null, false);
		}

		public static List<MacDefect> GetData(string lotNo, int procNo) 
		{
			return GetData(string.Empty, lotNo, procNo, string.Empty, string.Empty, null, null, false);
		}

		public static List<MacDefect> GetData(string lotNo, string defItemCd, string defItemNm) 
		{
			return GetData(string.Empty, lotNo, null, defItemCd, defItemNm, null, null, false);
		}

		/// <summary>
		/// 外観検査機で検査不要の不良を取得
		/// </summary>
		/// <returns></returns>
		public static List<MacDefect> GetNonInspection(string lotNo) 
		{
			List<MacDefect> retv = new List<MacDefect>(); 

			Dictionary<string, string> defItems = General.GetNonInspectionDefectItem();
			foreach(string defItemCd in defItems.Keys)
			{
				List<MacDefect> defectList = GetData(lotNo, defItemCd, string.Empty);
				retv.AddRange(defectList);
			}
			
			return retv;
		}

		public static void UpdateDefect(List<MacDefect> updateTargetList, string updUserCD)
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.LensConnectionString))
			{
				con.Open();
				using (SqlTransaction tran = con.BeginTransaction())
				{
					//using (ConnectQCIL conn = new ConnectQCIL(true, Constant.StrQCIL))
					//{
					try
					{
						//不良情報を更新
						foreach (MacDefect defectInfo in updateTargetList)
						{
							defectInfo.UpdUserCd = updUserCD;

							if (string.IsNullOrEmpty(defectInfo.UpdateDefAddressNo) == false)
							{
								CheckIntParsable(defectInfo.UpdateDefAddressNo);
								if (ExistMappingAddress(defectInfo.LotNo, int.Parse(defectInfo.UpdateDefAddressNo)) == false)
								{
									Lot lot = Lot.GetData(defectInfo.LotNo);
									Magazine.Config magConfig = Magazine.Config.GetData(lot.TypeCd);
									throw new ApplicationException(string.Format("存在しないアドレスです。\nタイプ:{0} 最大アドレス：{1} 入力アドレス：{2}", lot.LotNo, magConfig.TotalMagazinePackage, defectInfo.UpdateDefAddressNo));
								}
							}

							UpdateDefectInfo(con, tran, defectInfo);
						}
						tran.Commit();
					}
					catch
					{
						tran.Rollback();
						throw;
					}
				}
				////マッピングファイル作成
				//string mappingDir
				//    = string.Format(MAPPINGFILE_DEPOSITORY_AI, ((ServInfo)cmbServer.SelectedItem).ServerCD);
				//MachineFileInfo.CreateMappingFile(mappingDir, targetList[0].LotNO, mappingDataList);

				
			}
		}

		/// <summary>
		/// TnDEFECT(不良履歴)を更新
		/// </summary>
		/// <param name="defectInfo"></param>
		public static void UpdateDefectInfo(SqlConnection con, SqlTransaction tran, MacDefect defectInfo)
		{
			try
			{
				using (SqlCommand cmd = con.CreateCommand())
				{
					string sql = @" UPDATE TnMacDefect SET DelFG = @DelFG, UpdUserCD = @UpdUserCD, LastUpdDT = @LastUpdDT ";

					cmd.Parameters.Add("@DelFG", SqlDbType.Bit).Value = defectInfo.DelFg;
					cmd.Parameters.Add("@UpdUserCD", SqlDbType.Char).Value = defectInfo.UpdUserCd;
					cmd.Parameters.Add("@LastUpdDT", SqlDbType.DateTime).Value = DateTime.Now;

					if (string.IsNullOrEmpty(defectInfo.TranCd) == false)
					{
						sql += @" , TranCD = @TranCD ";
						cmd.Parameters.Add("@TranCD", SqlDbType.Char).Value = defectInfo.TranCd;
					}

					if (string.IsNullOrEmpty(defectInfo.UpdateTranCd) == false)
					{
						sql = @" , UpdateTranCD = @UpdateTranCD ";
						cmd.Parameters.Add("@UpdateTranCD", SqlDbType.Char).Value = defectInfo.UpdateTranCd;
					}

					if (string.IsNullOrEmpty(defectInfo.UpdateDefAddressNo) == false)
					{
						sql += @" , DefAddressNO = @UpdateDefAddressNO ";
						cmd.Parameters.Add("@UpdateDefAddressNO", SqlDbType.VarChar).Value = defectInfo.UpdateDefAddressNo;
					}

//					string conditionSql = @" WHERE (Line_CD = @Line_CD) AND (Plant_CD = @Plant_CD) 
//										   AND (Lot_NO = @Lot_NO) AND (DefAddress_NO = @DefAddress_NO) AND (DefUnit_NO = @DefUnit_NO) ";
					string conditionSql = @" WHERE (PlantCD = @PlantCD) AND (LotNO = @LotNO) AND (DefAddressNO = @DefAddressNO) AND (DefUnitNO = @DefUnitNO) ";



					//cmd.Parameters.Add("@Line_CD", SqlDbType.Int).Value = defectInfo.LineCD;
					cmd.Parameters.Add("@PlantCD", SqlDbType.Char).Value = defectInfo.PlantCd;
					cmd.Parameters.Add("@LotNO", SqlDbType.VarChar).Value = defectInfo.LotNo;
					cmd.Parameters.Add("@DefAddressNO", SqlDbType.VarChar).Value = defectInfo.DefAddressNo;

					cmd.Parameters.Add("@DefUnitNO", SqlDbType.VarChar).Value = defectInfo.DefUnitNo;

					sql += conditionSql;
#if TEST
				this.connection.ExecuteNonQuery(sql);
#else
					if (tran != null)
					{
						cmd.Transaction = tran;
					}
					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
#endif
			}
			catch (Exception err)
			{
				throw err;
			}
		}

		public static bool ExistMappingAddress(Magazine.Config magConfig, int chkAddr)
		{
			return ExistMappingAddress(magConfig.TotalMagazinePackage, chkAddr);
		}

		public static bool ExistMappingAddress(string lotNO, int chkAddr)
		{
			//フレーム情報を取得
			Lot lot = Lot.GetData(lotNO);
			Magazine.Config magConfig = Magazine.Config.GetData(lot.TypeCd);
			return ExistMappingAddress(magConfig, chkAddr);
		}

		public static bool ExistMappingAddress(int maxAddr, int chkAddr)
		{
			if (chkAddr > 0 && chkAddr <= maxAddr)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void CheckIntParsable(string targetStr)
		{
			int testNum;
			if (int.TryParse(targetStr, out testNum) == false)
			{
				throw new ApplicationException(string.Format("数値に変換出来る文字列である必要があります。変換対象:{0}", targetStr));
			}
		}
    }

//	public class Defect 
//	{
//		public Defect() 
//		{
//			this.DefItems = new List<DefItem>();
//		}

//		public string LotNo { get; set; }
//		public int ProcNo { get; set; }
//		public List<DefItem> DefItems { get; set; }

//		public class DefItem 
//		{
//			public string CauseCd { get; set; }
//			public string ClassCd { get; set; }
//			public string DefectCd { get; set; }
//			public int DefectCt { get; set; }
//		}

//		public void DeleteInsert()
//		{
//			using (SqlConnection con = new SqlConnection(Config.Settings.NamiConnectionString))
//			using (SqlCommand cmd = con.CreateCommand())
//			{
//				try
//				{
//					con.Open();
//					cmd.Transaction = con.BeginTransaction();

//					cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = this.LotNo;
//					cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = this.ProcNo;

//					SqlParameter paramCauseCd = cmd.Parameters.Add("@CauseCD", SqlDbType.NVarChar);
//					SqlParameter paramClassCd = cmd.Parameters.Add("@ClassCD", SqlDbType.NVarChar);
//					SqlParameter paramDefectCd = cmd.Parameters.Add("@DefectCD", SqlDbType.NVarChar);
//					SqlParameter paramDefectCt = cmd.Parameters.Add("@DefectCT", SqlDbType.BigInt);

//					foreach (DefItem defItem in this.DefItems)
//					{
//						paramCauseCd.Value = defItem.CauseCd;
//						paramClassCd.Value = defItem.ClassCd;
//						paramDefectCd.Value = defItem.DefectCd;
//						paramDefectCt.Value = defItem.DefectCt;

//						//前履歴Delete
//						cmd.CommandText = @" DELETE FROM TnDefect 
//										WHERE (LotNO = @LotNO) AND (ProcNO = @ProcNO) 
//										AND (CauseCD = @CauseCD) AND (ClassCD = @ClassCD) AND (DefectCD = @DefectCD) ";

//						cmd.ExecuteNonQuery();

//						//新規Insert
//						cmd.CommandText = @"
//                            INSERT
//                             INTO TnDefect(LotNO
//	                            , ProcNO
//	                            , CauseCD
//	                            , ClassCD
//	                            , DefectCD
//	                            , DefectCT)
//                            values(@LotNO
//	                            , @ProcNO
//	                            , @CauseCD
//	                            , @ClassCD
//	                            , @DefectCD
//	                            , @DefectCT) ";

//						cmd.ExecuteNonQuery();
//					}

//					cmd.Transaction.Commit();
//				}
//				catch (Exception)
//				{
//					cmd.Transaction.Rollback();
//				}
//			}
//		}

//		public List<Defect> GetMasterData(string typeCd, string workCd)
//		{
//			List<Defect> retv = new List<Defect>();

//			using (SqlConnection con = new SqlConnection(""))
//			using (SqlCommand cmd = con.CreateCommand())
//			{
//				con.Open();

//				string sql = " SELECT *** FROM TmFrame WITH(nolock) WHERE TypeCD = @TypeCD AND WorkCD = @WorkCD ";

//				cmd.Parameters.Add("@TypeCD", System.Data.SqlDbType.NVarChar).Value = typeCd;
//				cmd.Parameters.Add("@WorkCD", System.Data.SqlDbType.NVarChar).Value = workCd;

//				cmd.CommandText = sql;
//				using (SqlDataReader rd = cmd.ExecuteReader())
//				{
//					while (rd.Read())
//					{
//						Defect def = new Defect();

//						retv.Add(def);
//					}
//				}
//			}
//			return retv;
//		}
//	}
}
