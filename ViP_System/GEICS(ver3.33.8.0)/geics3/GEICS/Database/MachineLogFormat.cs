using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data;
using System.Data.Common;

namespace GEICS.Database
{
    public class MachineLogFormat
    {
		public class FileFormat
		{
			public string ModelNM { get; set; }
			public int QcParamNO { get; set; }
			public string PrefixNM { get; set; }
			public int? ColumnNO { get; set; }
			public string HeaderNM { get; set; }
			public string SearchNM { get; set; }
			public string MachinePrefixNM { get; set; }
			public bool? StartUpFG { get; set; }
			public bool DelFG { get; set; }
			public string UpdUserCD { get; set; }
			public DateTime LastUpdDT { get; set; }
			public string EquipPartID { get; set; }

			public static List<FileFormat> GetData(int? qcParamNo, string modelNm, string prefixNm)
			{
				List<FileFormat> retv = new List<FileFormat>();

				using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
				{
					string sql = @" SELECT Model_NM, Prefix_NM, QcParam_NO, Column_NO, Header_NM, Search_NM, MachinePrefix_NM, StartUp_FG, Del_FG, UpdUser_CD
										, LastUpd_DT, EquipPart_ID
		                            FROM dbo.TmFILEFMT WITH(nolock) WHERE 1=1 AND Del_FG = 0 ";

					if (qcParamNo.HasValue)
					{
						sql += " AND (QcParam_NO = @QcParamNO) ";
						conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNo.Value);
					}

					if (!string.IsNullOrEmpty(modelNm))
					{
						sql += " AND (Model_NM = @ModelNM) ";
						conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNm);
					}

					if (!string.IsNullOrEmpty(prefixNm))
					{
						sql += " AND (Prefix_NM = @PrefixNM) ";
						conn.SetParameter("@PrefixNM", SqlDbType.VarChar, prefixNm);
					}

					using (DbDataReader rd = conn.GetReader(sql))
					{
						int ordColumn = rd.GetOrdinal("Column_NO");
						int ordHeader = rd.GetOrdinal("Header_NM");
						int ordSearch = rd.GetOrdinal("Search_NM");
						int ordMachinePrefix = rd.GetOrdinal("MachinePrefix_NM");
						int ordStartUp = rd.GetOrdinal("StartUp_FG");

						while (rd.Read())
						{
							FileFormat f = new FileFormat();
							f.ModelNM = Convert.ToString(rd["Model_NM"]).Trim();
							f.PrefixNM = Convert.ToString(rd["Prefix_NM"]).Trim();
							f.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);

							if (rd.IsDBNull(ordColumn) == false)
							{
								f.ColumnNO = rd.GetInt32(ordColumn);
							}
							else
							{
								f.ColumnNO = null;
							}

							if (rd.IsDBNull(ordHeader) == false)
							{
								f.HeaderNM = rd.GetString(ordHeader).Trim();
							}
							else
							{
								f.HeaderNM = null;
							}

							if (rd.IsDBNull(ordSearch) == false)
							{
								f.SearchNM = rd.GetString(ordSearch).Trim();
							}
							else
							{
								f.SearchNM = null;
							}

							if (rd.IsDBNull(ordMachinePrefix) == false)
							{
								f.MachinePrefixNM = rd.GetString(ordMachinePrefix).Trim();
							}
							else
							{
								f.MachinePrefixNM = null;
							}

							if (rd.IsDBNull(ordStartUp) == false)
							{
								f.StartUpFG = rd.GetBoolean(ordStartUp);
							}
							else
							{
								f.StartUpFG = null;
							}

							f.DelFG = Convert.ToBoolean(rd["Del_FG"]);
							f.UpdUserCD = Convert.ToString(rd["UpdUser_CD"]).Trim();
							f.LastUpdDT = Convert.ToDateTime(rd["LastUpd_DT"]);
							f.EquipPartID = Convert.ToString(rd["EquipPart_ID"]).Trim();

							retv.Add(f);
						}
					}
				}
				return retv;
			}
		}

        public class WirebonderFile
        {
            public bool ChangeFG { get; set; }

            public string PrefixNM { get; set; }
            public int QcParamNO { get; set; }
            public int FileFmtNO { get; set; }
            public string ModelNM { get; set; }

            public string OldPrefixNM { get; set; }
            public int OldQcParamNO { get; set; }
            public int OldFileFmtNO { get; set; }
            public string OldModelNM { get; set; }

            public int FunctionNO { get; set; }
            public string SearchNM { get; set; }
            public int SearchNO { get; set; }
            public int CommaNO { get; set; }

            public bool DelFG { get; set; }
            public string UpdUserCD { get; set; }
            public DateTime LastUpdDT { get; set; }

			public static List<WirebonderFile> GetData(int? qcParamNo, string modelNm)
			{
				return GetData(qcParamNo, modelNm, null, null);
			}

			public static List<string> GetModelNm(int? qcParamNo, string modelNm, string prefixNm, int? fileFmtNo)
			{
				List<string> retv = new List<string>();

				using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
				{
					string sql = @" SELECT Model_NM FROM dbo.TmFILEFMT_WB WITH(nolock) WHERE 1=1 ";

					if (qcParamNo.HasValue)
					{
						sql += " AND (QcParam_NO = @QcParamNO) ";
						conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNo.Value);
					}

					if (!string.IsNullOrEmpty(modelNm))
					{
						sql += " AND (Model_NM = @ModelNM) ";
						conn.SetParameter("@ModelNM", SqlDbType.NVarChar, modelNm);
					}

					if (fileFmtNo.HasValue)
					{
						sql += " AND (FileFmt_NO = @FileFmt_NO) ";
						conn.SetParameter("@FileFmt_NO", SqlDbType.Int, fileFmtNo.Value);
					}

					sql += " GROUP BY Model_NM ";

					using (DbDataReader rd = conn.GetReader(sql))
					{
						while (rd.Read())
						{
							string model = rd["Model_NM"].ToString().Trim();

							retv.Add(model);
						}
					}
				}
				return retv;
			}

            public static List<WirebonderFile> GetData(int? qcParamNo, string modelNm, string prefixNm, int? fileFmtNo)
            {
                List<WirebonderFile> retv = new List<WirebonderFile>();

                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT Prefix_NM, QcParam_NO, FileFmt_NO, Model_NM, Function_NO, Search_NM, Search_NO, Comma_NO, Del_FG, UpdUser_CD, LastUpd_DT
                            FROM dbo.TmFILEFMT_WB WITH(nolock) WHERE 1=1 ";

                    if (qcParamNo.HasValue)
                    {
                        sql += " AND (QcParam_NO = @QcParamNO) ";
                        conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNo.Value);
                    }

                    if (!string.IsNullOrEmpty(modelNm))
                    {
                        sql += " AND (Model_NM = @ModelNM) ";
                        conn.SetParameter("@ModelNM", SqlDbType.NVarChar, modelNm);
                    }

					if (fileFmtNo.HasValue)
					{
						sql += " AND (FileFmt_NO = @FileFmt_NO) ";
						conn.SetParameter("@FileFmt_NO", SqlDbType.Int, fileFmtNo.Value);
					}

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            WirebonderFile f = new WirebonderFile();
                            f.PrefixNM = rd["Prefix_NM"].ToString().Trim();
                            f.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            f.FileFmtNO = Convert.ToInt32(rd["FileFmt_NO"]);
                            f.ModelNM = rd["Model_NM"].ToString().Trim();
                            f.FunctionNO = Convert.ToInt32(rd["Function_NO"]);
                            f.SearchNM = rd["Search_NM"].ToString().Trim();
                            f.SearchNO = Convert.ToInt32(rd["Search_NO"]);
                            f.CommaNO = Convert.ToInt32(rd["Comma_NO"]);
                            f.DelFG = Convert.ToBoolean(rd["Del_FG"]);
                            f.UpdUserCD = rd["UpdUser_CD"].ToString().Trim();
                            f.LastUpdDT = Convert.ToDateTime(rd["LastUpd_DT"]);

                            f.OldPrefixNM = rd["Prefix_NM"].ToString().Trim();
                            f.OldQcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            f.OldFileFmtNO = Convert.ToInt32(rd["FileFmt_NO"]);
                            f.OldModelNM = rd["Model_NM"].ToString().Trim();

                            retv.Add(f);
                        }
                    }
                }
                return retv;
            }

            public static List<WirebonderFile> GetData(List<string> qcParamNoList, List<string> modelNmList, string prefixNm, List<string> fileFmtNoList, List<string> searchNmList)
            {
                List<WirebonderFile> retv = new List<WirebonderFile>();

                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT Prefix_NM, QcParam_NO, FileFmt_NO, Model_NM, Function_NO, Search_NM, Search_NO, Comma_NO, Del_FG, UpdUser_CD, LastUpd_DT
                            FROM dbo.TmFILEFMT_WB WITH(nolock) WHERE 1=1 ";

                    if (qcParamNoList.Count > 0)
                    {
                        string InQcParam = string.Join(",", qcParamNoList.ToArray());
                        sql += " AND (QcParam_NO IN (" + InQcParam + ")) ";
                    }

                    if (modelNmList.Count > 0)
                    {
                        string InModelNm = string.Join("','", modelNmList.ToArray());
                        sql += " AND (Model_NM IN ('" + InModelNm + "')) ";
                    }

                    if (fileFmtNoList.Count > 0)
                    {
                        string InFileFmt = string.Join(",", fileFmtNoList.ToArray());
                        sql += " AND (FileFmt_NO IN (" + InFileFmt + ")) ";
                    }

                    if (searchNmList.Count > 0)
                    {
                        string InSearchNm = string.Join("','", searchNmList.ToArray());
                        sql += " AND (Search_NM IN ('" + InSearchNm + "')) ";
                    }

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            WirebonderFile f = new WirebonderFile();
                            f.PrefixNM = rd["Prefix_NM"].ToString().Trim();
                            f.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            f.FileFmtNO = Convert.ToInt32(rd["FileFmt_NO"]);
                            f.ModelNM = rd["Model_NM"].ToString().Trim();
                            f.FunctionNO = Convert.ToInt32(rd["Function_NO"]);
                            f.SearchNM = rd["Search_NM"].ToString().Trim();
                            f.SearchNO = Convert.ToInt32(rd["Search_NO"]);
                            f.CommaNO = Convert.ToInt32(rd["Comma_NO"]);
                            f.DelFG = Convert.ToBoolean(rd["Del_FG"]);
                            f.UpdUserCD = rd["UpdUser_CD"].ToString().Trim();
                            f.LastUpdDT = Convert.ToDateTime(rd["LastUpd_DT"]);

                            f.OldPrefixNM = rd["Prefix_NM"].ToString().Trim();
                            f.OldQcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            f.OldFileFmtNO = Convert.ToInt32(rd["FileFmt_NO"]);
                            f.OldModelNM = rd["Model_NM"].ToString().Trim();

                            retv.Add(f);
                        }
                    }
                }
                return retv;
            }

            public static void InsertUpdate(WirebonderFile data) 
            {
                if (string.IsNullOrEmpty(data.PrefixNM) || string.IsNullOrEmpty(data.ModelNM) || data.QcParamNO == 0 || data.FileFmtNO == 0)
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_92, data.PrefixNM));
                }

                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" UPDATE TmFILEFMT_WB SET
                                        Prefix_NM = @PrefixNM, QcParam_NO = @QcParamNO, FileFmt_NO = @FileFmtNO, Model_NM = @ModelNM, 
                                        Function_NO = @FunctionNO, Search_NM = @SearchNM, Search_NO = @SearchNO, Comma_NO = @CommaNO,  
                                        Del_FG = @DelFG, UpdUser_CD = @UpdUserCD, LastUpd_DT = @LastUpdDT 
                                    WHERE Prefix_NM = @OldPrefixNM AND QcParam_NO = @OldQcParamNO AND FileFmt_NO = @OldFileFmtNO AND Model_NM = @OldModelNM
 
                                    INSERT INTO TmFILEFMT_WB (Prefix_NM, QcParam_NO, FileFmt_NO, Model_NM, Function_NO, Search_NM, Search_NO, Comma_NO, UpdUser_CD)
                                        SELECT @PrefixNM, @QcParamNO, @FileFmtNO, @ModelNM, @FunctionNO, @SearchNM, @SearchNO, @CommaNO, @UpdUserCD
                                        WHERE NOT EXISTS (SELECT * FROM TmFILEFMT_WB 
                                                            WHERE Prefix_NM = @PrefixNM AND QcParam_NO = @QcParamNO AND FileFmt_NO = @FileFmtNO AND Model_NM = @ModelNM) ";

                    conn.SetParameter("@PrefixNM", SqlDbType.VarChar, Common.GetParameterValue(data.PrefixNM));
                    conn.SetParameter("@QcParamNO", SqlDbType.Int, Common.GetParameterValue(data.QcParamNO));
                    conn.SetParameter("@FileFmtNO", SqlDbType.Int, Common.GetParameterValue(data.FileFmtNO));
                    conn.SetParameter("@ModelNM", SqlDbType.NVarChar, Common.GetParameterValue(data.ModelNM));

                    conn.SetParameter("@OldPrefixNM", SqlDbType.VarChar, Common.GetParameterValue(data.OldPrefixNM));
                    conn.SetParameter("@OldQcParamNO", SqlDbType.Int, Common.GetParameterValue(data.OldQcParamNO));
                    conn.SetParameter("@OldFileFmtNO", SqlDbType.Int, Common.GetParameterValue(data.OldFileFmtNO));
                    conn.SetParameter("@OldModelNM", SqlDbType.NVarChar, Common.GetParameterValue(data.OldModelNM));

                    conn.SetParameter("@FunctionNO", SqlDbType.Int, Common.GetParameterValue(data.FunctionNO));
                    conn.SetParameter("@SearchNM", SqlDbType.VarChar, Common.GetParameterValue(data.SearchNM));
                    conn.SetParameter("@SearchNO", SqlDbType.Int, Common.GetParameterValue(data.SearchNO));
                    conn.SetParameter("@CommaNO", SqlDbType.Int, Common.GetParameterValue(data.CommaNO));

                    conn.SetParameter("@DelFG", SqlDbType.Bit, data.DelFG);
                    conn.SetParameter("@UpdUserCD", SqlDbType.Char, Constant.EmployeeInfo.EmployeeCD);
                    conn.SetParameter("@LastUpdDT", SqlDbType.DateTime, System.DateTime.Now);

                    conn.ExecuteNonQuery(sql);
                }
            }

            /// <summary>
            /// 物理削除
            /// </summary>
            public static void Delete(WirebonderFile data)
            {
                if (string.IsNullOrEmpty(data.PrefixNM) || string.IsNullOrEmpty(data.ModelNM) || data.QcParamNO == 0 || data.FileFmtNO == 0)
                {
                    throw new ApplicationException(string.Format("削除処理をするにはキーの情報が不適切です。"));
                }

                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" DELETE
                                    FROM TmFILEFMT_WB
                                    WHERE Prefix_NM = @PrefixNM AND QcParam_NO = @QcParamNO AND FileFmt_NO = @FileFmtNO AND Model_NM = @ModelNM ";

                    conn.SetParameter("@PrefixNM", SqlDbType.VarChar, Common.GetParameterValue(data.PrefixNM));
                    conn.SetParameter("@QcParamNO", SqlDbType.Int, Common.GetParameterValue(data.QcParamNO));
                    conn.SetParameter("@FileFmtNO", SqlDbType.Int, Common.GetParameterValue(data.FileFmtNO));
                    conn.SetParameter("@ModelNM", SqlDbType.NVarChar, Common.GetParameterValue(data.ModelNM));

                    conn.ExecuteNonQuery(sql);
                }
            }

            /// <summary>
            /// TmFILEFMTTYPE取得
            /// </summary>
            /// <param name="typeCd"></param>
            /// <returns></returns>
            public static List<FileFmtTypeInfo> GetTypeClassData(string typeCd) 
            {
                return ConnectQCIL.GetFILEFMTTYPEData(typeCd);
            }

            /// <summary>
            /// TmFILEFMTTYPE追加更新
            /// </summary>
            /// <param name="plmInfo"></param>
            public static void InsertUpdateTypeClass(FileFmtTypeInfo fileFmtTypeInfo)
            {
                if (string.IsNullOrEmpty(fileFmtTypeInfo.TypeCD))
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_92, fileFmtTypeInfo.TypeCD));
                }

                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" UPDATE TmFILEFMTTYPE SET
                                    Type_CD = @TypeCD, FileFmt_NO = @FileFmtNO, Frame_NO = @FrameNO,  
                                    Del_FG = @DelFG, UpdUser_CD = @UpdUserCD, LastUpd_DT = @LastUpdDT 
                                WHERE Type_CD = @OldTypeCD
                                INSERT INTO TmFILEFMTTYPE (Type_CD, FileFmt_NO, Frame_NO, UpdUser_CD)
                                    SELECT @TypeCD, @FileFmtNO, @FrameNO, @UpdUserCD
                                    WHERE NOT EXISTS (SELECT * FROM TmFILEFMTTYPE WHERE Type_CD = @TypeCD) ";

                    conn.SetParameter("@TypeCD", SqlDbType.VarChar, Common.GetParameterValue(fileFmtTypeInfo.TypeCD));
                    conn.SetParameter("@OldTypeCD", SqlDbType.VarChar, Common.GetParameterValue(fileFmtTypeInfo.OldTypeCD));

                    conn.SetParameter("@FileFmtNO", SqlDbType.Int, Common.GetParameterValue(fileFmtTypeInfo.FileFmtNO));
                    conn.SetParameter("@FrameNO", SqlDbType.Char, Common.GetParameterValue(fileFmtTypeInfo.FrameNO));
                    conn.SetParameter("@DelFG", SqlDbType.Bit, fileFmtTypeInfo.DelFG);
                    conn.SetParameter("@UpdUserCD", SqlDbType.Char, Constant.EmployeeInfo.EmployeeCD);
                    conn.SetParameter("@LastUpdDT", SqlDbType.DateTime, System.DateTime.Now);

                    conn.ExecuteNonQuery(sql);
                }
            }
        }

        public class WirebonderSecs
        {
            public bool ChangeFG { get; set; }

            public string FmtTypeCD { get; set; }
            public int FmtNO { get; set; }
            public string ModelNM { get; set; }
            public int QcParamNO { get; set; }

            public string OldFmtTypeCD { get; set; }
            public int OldFmtNO { get; set; }
            public string OldModelNM { get; set; }
            public int OldQcParamNO { get; set; }

            public int SearchGrpParamNO { get; set; }
            public int SearchParamNO { get; set; }
            public string SearchParamNM { get; set; }
            public int SearchValueNO { get; set; }

            public bool DelFG { get; set; }
            public string UpdUserCD { get; set; }
            public DateTime LastUpdDT { get; set; }

            /// <summary>
            /// TmMSGFMT取得
            /// </summary>
            /// <param name="typeCd"></param>
            /// <returns></returns>
            public static List<WirebonderSecs> GetData(string modelNm, int? qcparamNo)
            {
                List<WirebonderSecs> retv = new List<WirebonderSecs>();

                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT MsgType_CD, MsgFmt_NO, Model_NM, QcParam_NO, SearchParam_NO, SearchParam_NM, SearchValue_NO, Del_FG, UpdUser_CD, LastUpd_DT, SearchGrpParam_NO
                                    FROM TmMSGFMT WITH(nolock)
                                    WHERE 1=1 ";

                    if (!string.IsNullOrEmpty(modelNm))
                    {
                        sql += " AND (Model_NM = @ModelNM) ";
                        conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNm);
                    }

                    if (qcparamNo.HasValue)
                    {
                        sql += " AND QcParam_NO = @QcParamNO ";
                        conn.SetParameter("@QcParamNO", SqlDbType.Int, qcparamNo);
                    }

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            WirebonderSecs f = new WirebonderSecs();
                            f.FmtTypeCD = rd["MsgType_CD"].ToString().Trim();
                            f.FmtNO = Convert.ToInt32(rd["MsgFmt_NO"]);
                            f.ModelNM = rd["Model_NM"].ToString().Trim();
                            f.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            f.DelFG = Convert.ToBoolean(rd["Del_FG"]);
                            f.UpdUserCD = rd["UpdUser_CD"].ToString().Trim();
                            f.LastUpdDT = Convert.ToDateTime(rd["LastUpd_DT"]);
                            f.SearchGrpParamNO = Convert.ToInt32(rd["SearchGrpParam_NO"]);

                            f.FmtTypeCD = f.FmtTypeCD;
                            f.FmtNO = f.FmtNO;
                            f.ModelNM = f.ModelNM;
                            f.QcParamNO = f.QcParamNO;

                            retv.Add(f);
                        }
                    }
                }

                return retv;
            }

            /// <summary>
            /// TmMSGFMT追加更新
            /// </summary>
            /// <param name="plmInfo"></param>
            public static void InsertUpdate(WirebonderSecs data)
            {
                //if (string.IsNullOrEmpty(data.TypeCD))
                //{
                //    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_92, data.TypeCD));
                //}

                using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                {
                    string sql = @" UPDATE TmMSGFMT SET
                                    MsgType_CD = @MsgTypeCD, MsgFmt_NO = @MsgFmtNO,  Model_NM = @ModelNM, QcParam_NO = @QcParamNO,
                                    SearchParam_NO = @SearchParamNO, SearchParam_NM = @SearchParamNM, SearchValue_NO = @SearchValueNO,
                                    SearchGrpParam_NO = @SearchGrpParamNO
                                    Del_FG = @DelFG, UpdUser_CD = @UpdUserCD, LastUpd_DT = @LastUpdDT 
                                    WHERE MsgType_CD = @OldMsgTypeCD AND MsgFmt_NO = @OldMsgFmtNO AND Model_NM = @OldModelNM AND QcParam_NO = @OldQcParamNO
                                    INSERT INTO TmMSGFMT (MsgType_CD, MsgFmt_NO, Model_NM, QcParam_NO, SearchParam_NO, SearchParam_NM, SearchValue_NO, SearchGrpParam_NO, UpdUser_CD)
                                    SELECT @MsgTypeCD, @MsgFmtNO, @ModelNM, @QcParamNO, @SearchParamNO, @SearchParamNM, @SearchValueNO, @SearchGrpParamNO, @UpdUserCD
                                    WHERE NOT EXISTS (SELECT * FROM TmMSGFMT WHERE MsgType_CD = @OldMsgTypeCD AND MsgFmt_NO = @OldMsgFmtNO AND Model_NM = @OldModelNM AND QcParam_NO = @OldQcParamNO) ";

                    conn.SetParameter("@MsgTypeCD", SqlDbType.Char, Common.GetParameterValue(data.FmtTypeCD));
                    conn.SetParameter("@OldMsgTypeCD", SqlDbType.Char, Common.GetParameterValue(data.OldFmtTypeCD));

                    conn.SetParameter("@MsgFmtNO", SqlDbType.Int, Common.GetParameterValue(data.FmtTypeCD));
                    conn.SetParameter("@OldMsgFmtNO", SqlDbType.Int, Common.GetParameterValue(data.OldFmtTypeCD));

                    conn.SetParameter("@ModelNM", SqlDbType.VarChar, Common.GetParameterValue(data.FmtTypeCD));
                    conn.SetParameter("@OldModelNM", SqlDbType.VarChar, Common.GetParameterValue(data.OldFmtTypeCD));

                    conn.SetParameter("@QcParamNO", SqlDbType.Int, Common.GetParameterValue(data.FmtTypeCD));
                    conn.SetParameter("@OldQcParamNO", SqlDbType.Int, Common.GetParameterValue(data.OldFmtTypeCD));



                    conn.SetParameter("@MsgFmtNO", SqlDbType.Int, Common.GetParameterValue(data.FmtNO));
                    conn.SetParameter("@DelFG", SqlDbType.Bit, data.DelFG);
                    conn.SetParameter("@UpdUserCD", SqlDbType.Char, Constant.EmployeeInfo.EmployeeCD);
                    conn.SetParameter("@LastUpdDT", SqlDbType.DateTime, System.DateTime.Now);

                    conn.ExecuteNonQuery(sql);
                }
            }

            public class FormatType
            {
                public bool ChangeFG { get; set; }

                public string TypeCD { get; set; }

                public string OldTypeCD { get; set; }

                public int FmtNO { get; set; }
                public bool DelFG { get; set; }
                public string UpdUserCD { get; set; }
                public DateTime LastUpdDT { get; set; }

                /// <summary>
                /// TmMSGFMTTYPE取得
                /// </summary>
                /// <param name="typeCd"></param>
                /// <returns></returns>
                public static List<FormatType> GetData(string typeCd)
                {
                    List<FormatType> retv = new List<FormatType>();

                    using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                    {
                        string sql = @" SELECT Type_CD, MsgFmt_NO, Del_FG, UpdUser_CD, LastUpd_DT
                                        FROM TmMSGFMTTYPE WITH (nolock) WHERE 1=1";

                        if (!string.IsNullOrEmpty(typeCd))
                        {
                            sql += " AND Type_CD = @TypeCD ";
                            conn.SetParameter("@TypeCD", SqlDbType.Char, typeCd);
                        }

                        using (DbDataReader rd = conn.GetReader(sql)) 
                        {
                            while (rd.Read()) 
                            {
                                FormatType t = new FormatType();
                                t.TypeCD = rd["Type_CD"].ToString().Trim();
                                t.FmtNO = Convert.ToInt32(rd["MsgFmt_NO"]);
                                t.DelFG = Convert.ToBoolean(rd["Del_FG"]);
                                t.UpdUserCD = rd["UpdUser_CD"].ToString().Trim();
                                t.LastUpdDT = Convert.ToDateTime(rd["LastUpd_DT"]);

                                t.OldTypeCD = t.TypeCD;

                                retv.Add(t);
                            }
                        }
                    }

                    return retv;
                }

                /// <summary>
                /// TmMSGFMTTYPE追加更新
                /// </summary>
                /// <param name="plmInfo"></param>
                public static void InsertUpdate(FormatType data)
                {
                    if (string.IsNullOrEmpty(data.TypeCD))
                    {
                        throw new ApplicationException(string.Format(Constant.MessageInfo.Message_92, data.TypeCD));
                    }

                    using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
                    {
                        string sql = @" UPDATE TmMSGFMTTYPE SET
                                    Type_CD = @TypeCD, MsgFmt_NO = @MsgFmtNO,  
                                    Del_FG = @DelFG, UpdUser_CD = @UpdUserCD, LastUpd_DT = @LastUpdDT 
                                    WHERE Type_CD = @OldTypeCD
                                    INSERT INTO TmMSGFMTTYPE (Type_CD, MsgFmt_NO, UpdUser_CD)
                                    SELECT @TypeCD, @MsgFmtNO, @UpdUserCD
                                    WHERE NOT EXISTS (SELECT * FROM TmMSGFMTTYPE WHERE Type_CD = @OldTypeCD) ";

                        conn.SetParameter("@TypeCD", SqlDbType.VarChar, Common.GetParameterValue(data.TypeCD));
                        conn.SetParameter("@OldTypeCD", SqlDbType.VarChar, Common.GetParameterValue(data.OldTypeCD));

                        conn.SetParameter("@MsgFmtNO", SqlDbType.Int, Common.GetParameterValue(data.FmtNO));
                        conn.SetParameter("@DelFG", SqlDbType.Bit, data.DelFG);
                        conn.SetParameter("@UpdUserCD", SqlDbType.Char, Constant.EmployeeInfo.EmployeeCD);
                        conn.SetParameter("@LastUpdDT", SqlDbType.DateTime, System.DateTime.Now);

                        conn.ExecuteNonQuery(sql);
                    }
                }
            }
        }
    }
}
