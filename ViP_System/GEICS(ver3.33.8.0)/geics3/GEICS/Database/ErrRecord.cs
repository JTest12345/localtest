using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace GEICS.Database
{
	class ErrRecord
	{
		public int QCNR_NO { get; set; }
		public int Inline_CD { get; set; }
		public string Inline_NM { get; set; }
		public string Equipment_NO { get; set; }
		public string NascaLot_NO { get; set; }
		public int Defect_NO { get; set; }
		public string Defect_NM { get; set; }
		public int Inspection_NO { get; set; }
		public string Inspection_NM { get; set; }
		public string Process_NO { get; set; }
		public string Timing_NO { get; set; }
		public string Timing_NM { get; set; }
		public int Multi_NO { get; set; }
		public DateTime Measure_DT { get; set; }
		public string Message { get; set; }
		public string Type_CD { get; set; }
		public int BackNum_NO { get; set; }
		public int Check_NO { get; set; }
		public string UpdUser_CD { get; set; }
		public DateTime LastUpd_DT { get; set; }
		public string Confirm_NM { get; set; }
		public string Assets_NM { get; set; }
		public string Magazine_NO { get; set; }
		public string DBMachine_NM { get; set; }
		public string WBMachine_NM { get; set; }
	
		public static List<ErrRecord> GetData(DateTime startDt, DateTime endDt, bool getAllDataFg, bool getDBWBMachineNmFg)
		{
			Common com = new Common();
			List<ErrRecord> dataList = new List<ErrRecord>();

			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				try
				{
					string sql = @" SELECT {0} TvQCNR.QCNR_NO, TvQCNR.Inline_CD, TmLINE.Inline_NM, TvQCNR.Equipment_NO, TvQCNR.NascaLot_NO, TvQCNR.Defect_NO, TvQCNR.Inspection_NO, TvQCNR.Inspection_NM,
                                TvQCNR.Process_NO, TvQCNR.Timing_NO, TvQCNR.Timing_NM, TvQCNR.Multi_NO, TvQCNR.Measure_DT, TvQCNR.Message, TvQCNR.Type_CD, TvQCNR.BackNum_NO, TvQCNR.Check_NO, TnQCNRCnfm.Confirm_NM, TvQCNR.UpdUser_CD, TvQCNR.LastUpd_DT, TmEQUI.Assets_NM 
                                FROM dbo.TvQCNR_Map AS TvQCNR WITH(NOLOCK) 
                                INNER JOIN TmLINE WITH(NOLOCK) ON TvQCNR.Inline_CD = TmLINE.Inline_CD
                                LEFT OUTER JOIN TmEQUI WITH(NOLOCK) ON TvQCNR.Equipment_NO = TmEQUI.Equipment_NO
                                LEFT OUTER JOIN 
                                    (SELECT Inline_CD, QCNR_NO, MAX(CONVERT(int, Confirm_NO)) AS Confirm_NO, Confirm_NM, Product_FG, Operator_NM 
                                        FROM dbo.TnQCNRCnfm AS TnQCNRCnfm WITH(NOLOCK) 
                                        WHERE (Del_FG = 0) 
                                        GROUP BY Inline_CD, QCNR_NO, Confirm_NM, Product_FG, Operator_NM) AS TnQCNRCnfm 
                                ON TvQCNR.QCNR_NO = TnQCNRCnfm.QCNR_NO 
                                AND TvQCNR.Inline_CD = TnQCNRCnfm.Inline_CD 
                                WHERE (TvQCNR.Inline_CD = @LINECD)";

					if (getAllDataFg)
					{
						sql = string.Format(sql, "");
						sql += " AND (Measure_DT >= @FROMDT AND Measure_DT <= @TODT AND Check_NO = 1) OR (TvQCNR.Inline_CD = @LINECD AND Check_NO = 0) ";
						connect.Command.Parameters.Add("@CHKNO", SqlDbType.Int).Value = 0;
					}
					else
					{
						sql = string.Format(sql, "TOP 1000");
						sql += " AND (Check_NO = 0) ";
						connect.Command.Parameters.Add("@CHKNO", SqlDbType.Int).Value = 0;
					}

					connect.Command.Parameters.Add("@FROMDT", SqlDbType.DateTime).Value = startDt;
					connect.Command.Parameters.Add("@TODT", SqlDbType.DateTime).Value = endDt;
					connect.Command.Parameters.Add("@LINECD", SqlDbType.Int).Value = Common.nLineCD;

					connect.Command.CommandText = sql;
					using (SqlDataReader rd = connect.Command.ExecuteReader())
					{
						int ordQcnrNo = rd.GetOrdinal("QCNR_NO");
						int ordLineCd = rd.GetOrdinal("Inline_CD");
						int ordLineNm = rd.GetOrdinal("Inline_NM");
						int ordEquipNo = rd.GetOrdinal("Equipment_NO");
						int ordLotNo = rd.GetOrdinal("NascaLot_NO");
						int ordDefNo = rd.GetOrdinal("Defect_NO");
						int ordInspectNo = rd.GetOrdinal("Inspection_NO");
						int ordInspectNm = rd.GetOrdinal("Inspection_NM");
						int ordProcNo = rd.GetOrdinal("Process_NO");
						int ordTimNo = rd.GetOrdinal("Timing_NO");
						int ordTimNm = rd.GetOrdinal("Timing_NM");
						int ordMultiNo = rd.GetOrdinal("Multi_NO");
						int ordMeasureDt = rd.GetOrdinal("Measure_DT");
						int ordMsg = rd.GetOrdinal("Message");
						int ordTypeCd = rd.GetOrdinal("Type_CD");
						int ordBackNumNo = rd.GetOrdinal("BackNum_NO");
						int ordChkNo = rd.GetOrdinal("Check_NO");
						int ordUpdUserCd = rd.GetOrdinal("UpdUser_CD");
						int ordLastUpdDt = rd.GetOrdinal("LastUpd_DT");
						int ordConfirmNm = rd.GetOrdinal("Confirm_NM");
						int ordAssetsNm = rd.GetOrdinal("Assets_NM");

						while (rd.Read())
						{
							string sInspectionNM = rd.GetString(ordInspectNm).Trim();

							if (sInspectionNM.Substring(0, 1) == "F")
							{
								sInspectionNM = com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「不具合A(F*****)」の表記に変更
							}

							string sEquipmentNO = rd.GetString(ordEquipNo).Trim();
							sEquipmentNO = com.AddCommentEquipmentNO(sEquipmentNO);

							ErrRecord data = new ErrRecord();

							data.QCNR_NO = rd.GetInt32(ordQcnrNo);
							data.Inline_CD = rd.GetInt32(ordLineCd);
							data.Inline_NM = rd.GetString(ordLineNm).Trim();
							data.Equipment_NO = sEquipmentNO;
							data.NascaLot_NO = rd.GetString(ordLotNo).Trim();
							data.Defect_NO = rd.GetInt32(ordDefNo);
							data.Defect_NM = "";
							data.Inspection_NO = rd.GetInt32(ordInspectNo);
							data.Inspection_NM = sInspectionNM;
							data.Process_NO = rd.GetString(ordProcNo).Trim();
							data.Timing_NO = rd.GetString(ordTimNo).Trim();
							data.Timing_NM = rd.GetString(ordTimNm).Trim();
							data.Multi_NO = rd.GetInt32(ordMultiNo);
							data.Measure_DT = rd.GetDateTime(ordMeasureDt);
							data.Message = rd.GetString(ordMsg).Replace("\r\n", "。").Trim();
							data.Type_CD = rd.GetString(ordTypeCd).Trim();
							data.BackNum_NO = rd.GetInt32(ordBackNumNo);
							data.Check_NO = rd.GetInt32(ordChkNo);
							data.UpdUser_CD = rd.GetString(ordUpdUserCd).Trim();
							data.LastUpd_DT = rd.GetDateTime(ordLastUpdDt);

							if (!rd.IsDBNull(ordConfirmNm))
							{
								data.Confirm_NM = rd.GetString(ordConfirmNm).Replace("\r\n", "。").Trim();
							}
							else
							{
								data.Confirm_NM = string.Empty;
							}

							if (!rd.IsDBNull(ordAssetsNm))
							{
								data.Assets_NM = rd.GetString(ordAssetsNm).Trim();
							}
							else
							{
								data.Assets_NM = string.Empty;
							}

							data.Magazine_NO = Database.Log.GetMagazineNO(data.NascaLot_NO, sEquipmentNO, data.Inline_CD, startDt, endDt);
                            if (getDBWBMachineNmFg)
                            {
                                data.DBMachine_NM = Database.Lott.GetDBMachineNames(data.Inline_CD, data.NascaLot_NO);
                                data.WBMachine_NM = Database.Lott.GetWBMachineNames(data.Inline_CD, data.NascaLot_NO);
                            }

                            dataList.Add(data);
						}
					}
				}
				catch (Exception ex)
				{
					//Console.WriteLine(ex.ToString());
					//MessageBox.Show(Constant.MessageInfo.Message_47 + ex.ToString());
				}
				finally
				{
					connect.Close();

					//<--Start 2010.03.09 応答なし回避
					//fDrawComplete = true;
					//-->End 2010.03.09 応答なし回避
				}
			}

			return dataList;
		}

        // 2016.01.25 永尾修正 2015.12.09の修正で想定漏れがあった為、各パラメータ指定で取得する。。
        public static List<ErrRecord> GetData(int? inline_CD, string equipment_NO, string nascaLot_NO, int? defect_NO, int? inspection_NO,
                                                string process_NO, string timing_NO, int? multi_NO, string Message, DateTime startDt, DateTime endDt, int? Check_NO, bool getDBWBMachineNmFg)
        {
            Common com = new Common();
            List<ErrRecord> dataList = new List<ErrRecord>();

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                try
                {
                    string sql = @" SELECT {0} TvQCNR.QCNR_NO, TvQCNR.Inline_CD, TmLINE.Inline_NM, TvQCNR.Equipment_NO, TvQCNR.NascaLot_NO, TvQCNR.Defect_NO, TvQCNR.Inspection_NO, TvQCNR.Inspection_NM,
                                TvQCNR.Process_NO, TvQCNR.Timing_NO, TvQCNR.Timing_NM, TvQCNR.Multi_NO, TvQCNR.Measure_DT, TvQCNR.Message, TvQCNR.Type_CD, TvQCNR.BackNum_NO, TvQCNR.Check_NO, TnQCNRCnfm.Confirm_NM, TvQCNR.UpdUser_CD, TvQCNR.LastUpd_DT, TmEQUI.Assets_NM 
                                FROM dbo.TvQCNR_Map AS TvQCNR WITH(NOLOCK) 
                                INNER JOIN TmLINE WITH(NOLOCK) ON TvQCNR.Inline_CD = TmLINE.Inline_CD
                                LEFT OUTER JOIN TmEQUI WITH(NOLOCK) ON TvQCNR.Equipment_NO = TmEQUI.Equipment_NO
                                LEFT OUTER JOIN 
                                    (SELECT Inline_CD, QCNR_NO, MAX(CONVERT(int, Confirm_NO)) AS Confirm_NO, Confirm_NM, Product_FG, Operator_NM 
                                        FROM dbo.TnQCNRCnfm AS TnQCNRCnfm WITH(NOLOCK) 
                                        WHERE (Del_FG = 0) 
                                        GROUP BY Inline_CD, QCNR_NO, Confirm_NM, Product_FG, Operator_NM) AS TnQCNRCnfm 
                                ON TvQCNR.QCNR_NO = TnQCNRCnfm.QCNR_NO 
                                AND TvQCNR.Inline_CD = TnQCNRCnfm.Inline_CD 
                                WHERE TvQCNR.Inline_CD = @LINECD
                                AND Measure_DT >= @FROMDT AND Measure_DT <= @TODT ";

                    if (inline_CD != null)
                    {
                        connect.Command.Parameters.Add("@LINECD", SqlDbType.Int).Value = inline_CD;
                    }
                    else
                    {
                        connect.Command.Parameters.Add("@LINECD", SqlDbType.Int).Value = Common.nLineCD;
                    }

                    if (!string.IsNullOrEmpty(equipment_NO))
                    {
                        sql += " AND TvQCNR.Equipment_NO = @EquiNO ";
                        connect.Command.Parameters.Add("@EquiNO", SqlDbType.VarChar).Value = equipment_NO;
                    }

                    if (!string.IsNullOrEmpty(nascaLot_NO))
                    {
                        sql += " AND TvQCNR.NascaLot_NO = @LotNO ";
                        connect.Command.Parameters.Add("@LotNO", SqlDbType.VarChar).Value = nascaLot_NO;
                    }

                    if (defect_NO != null)
                    {
                        sql += " AND TvQCNR.Defect_NO = @DefNO ";
                        connect.Command.Parameters.Add("@DefNO", SqlDbType.Int).Value = defect_NO;
                    }

                    if (inspection_NO != null)
                    {
                        sql += " AND TvQCNR.Inspection_NO = @InsNO ";
                        connect.Command.Parameters.Add("@InsNO", SqlDbType.Int).Value = inspection_NO;
                    }

                    if (!string.IsNullOrEmpty(process_NO))
                    {
                        sql += " AND TvQCNR.Process_NO = @ProcNO ";
                        connect.Command.Parameters.Add("@ProcNO", SqlDbType.VarChar).Value = process_NO;
                    }

                    if (!string.IsNullOrEmpty(timing_NO))
                    {
                        sql += " AND TvQCNR.Timing_NO = @TimNO ";
                        connect.Command.Parameters.Add("@TimNO", SqlDbType.VarChar).Value = timing_NO;
                    }

                    if (multi_NO != null)
                    {
                        sql += " AND TvQCNR.Multi_NO = @MultiNO ";
                        connect.Command.Parameters.Add("@MultiNO", SqlDbType.Int).Value = multi_NO;
                    }

                    if (!string.IsNullOrEmpty(Message))
                    {
                        sql += " AND TvQCNR.Message = @Message ";
                        connect.Command.Parameters.Add("@Message", SqlDbType.VarChar).Value = Message;
                    }

                    if (Check_NO != null)
                    {
                        sql += " AND TvQCNR.Check_NO = @CheckNO ";
                        connect.Command.Parameters.Add("@CheckNO", SqlDbType.Int).Value = Check_NO;
                    }

                    connect.Command.Parameters.Add("@FROMDT", SqlDbType.DateTime).Value = startDt;
                    connect.Command.Parameters.Add("@TODT", SqlDbType.DateTime).Value = endDt;

                    if (string.IsNullOrEmpty(nascaLot_NO))
                    {
                        sql = string.Format(sql, "TOP 1000");
                    }
                    else
                    {
                        sql = string.Format(sql, "");
                    }

                    connect.Command.CommandText = sql;
                    using (SqlDataReader rd = connect.Command.ExecuteReader())
                    {
                        int ordQcnrNo = rd.GetOrdinal("QCNR_NO");
                        int ordLineCd = rd.GetOrdinal("Inline_CD");
                        int ordLineNm = rd.GetOrdinal("Inline_NM");
                        int ordEquipNo = rd.GetOrdinal("Equipment_NO");
                        int ordLotNo = rd.GetOrdinal("NascaLot_NO");
                        int ordDefNo = rd.GetOrdinal("Defect_NO");
                        int ordInspectNo = rd.GetOrdinal("Inspection_NO");
                        int ordInspectNm = rd.GetOrdinal("Inspection_NM");
                        int ordProcNo = rd.GetOrdinal("Process_NO");
                        int ordTimNo = rd.GetOrdinal("Timing_NO");
                        int ordTimNm = rd.GetOrdinal("Timing_NM");
                        int ordMultiNo = rd.GetOrdinal("Multi_NO");
                        int ordMeasureDt = rd.GetOrdinal("Measure_DT");
                        int ordMsg = rd.GetOrdinal("Message");
                        int ordTypeCd = rd.GetOrdinal("Type_CD");
                        int ordBackNumNo = rd.GetOrdinal("BackNum_NO");
                        int ordChkNo = rd.GetOrdinal("Check_NO");
                        int ordUpdUserCd = rd.GetOrdinal("UpdUser_CD");
                        int ordLastUpdDt = rd.GetOrdinal("LastUpd_DT");
                        int ordConfirmNm = rd.GetOrdinal("Confirm_NM");
                        int ordAssetsNm = rd.GetOrdinal("Assets_NM");

                        while (rd.Read())
                        {
                            string sInspectionNM = rd.GetString(ordInspectNm).Trim();

                            if (sInspectionNM.Substring(0, 1) == "F")
                            {
                                sInspectionNM = com.AddCommentInspectionNM(sInspectionNM);//「F*****」→「不具合A(F*****)」の表記に変更
                            }

                            string sEquipmentNO = rd.GetString(ordEquipNo).Trim();
                            sEquipmentNO = com.AddCommentEquipmentNO(sEquipmentNO);

                            ErrRecord data = new ErrRecord();

                            data.QCNR_NO = rd.GetInt32(ordQcnrNo);
                            data.Inline_CD = rd.GetInt32(ordLineCd);
                            data.Inline_NM = rd.GetString(ordLineNm).Trim();
                            data.Equipment_NO = sEquipmentNO;
                            data.NascaLot_NO = rd.GetString(ordLotNo).Trim();
                            data.Defect_NO = rd.GetInt32(ordDefNo);
                            data.Defect_NM = "";
                            data.Inspection_NO = rd.GetInt32(ordInspectNo);
                            data.Inspection_NM = sInspectionNM;
                            data.Process_NO = rd.GetString(ordProcNo).Trim();
                            data.Timing_NO = rd.GetString(ordTimNo).Trim();
                            data.Timing_NM = rd.GetString(ordTimNm).Trim();
                            data.Multi_NO = rd.GetInt32(ordMultiNo);
                            data.Measure_DT = rd.GetDateTime(ordMeasureDt);
                            data.Message = rd.GetString(ordMsg).Replace("\r\n", "。").Trim();
                            data.Type_CD = rd.GetString(ordTypeCd).Trim();
                            data.BackNum_NO = rd.GetInt32(ordBackNumNo);
                            data.Check_NO = rd.GetInt32(ordChkNo);
                            data.UpdUser_CD = rd.GetString(ordUpdUserCd).Trim();
                            data.LastUpd_DT = rd.GetDateTime(ordLastUpdDt);

                            if (!rd.IsDBNull(ordConfirmNm))
                            {
                                data.Confirm_NM = rd.GetString(ordConfirmNm).Replace("\r\n", "。").Trim();
                            }
                            else
                            {
                                data.Confirm_NM = string.Empty;
                            }

                            if (!rd.IsDBNull(ordAssetsNm))
                            {
                                data.Assets_NM = rd.GetString(ordAssetsNm).Trim();
                            }
                            else
                            {
                                data.Assets_NM = string.Empty;
                            }

                            data.Magazine_NO = Database.Log.GetMagazineNO(data.NascaLot_NO, sEquipmentNO, data.Inline_CD, startDt, endDt);
                            if (getDBWBMachineNmFg)
                            {
                                data.DBMachine_NM = Database.Lott.GetDBMachineNames(data.Inline_CD, data.NascaLot_NO);
                                data.WBMachine_NM = Database.Lott.GetWBMachineNames(data.Inline_CD, data.NascaLot_NO);
                            }

                            dataList.Add(data);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                    //MessageBox.Show(Constant.MessageInfo.Message_47 + ex.ToString());
                }
                finally
                {
                    connect.Close();

                    //<--Start 2010.03.09 応答なし回避
                    //fDrawComplete = true;
                    //-->End 2010.03.09 応答なし回避
                }
            }

            return dataList;
        }
	}
}
