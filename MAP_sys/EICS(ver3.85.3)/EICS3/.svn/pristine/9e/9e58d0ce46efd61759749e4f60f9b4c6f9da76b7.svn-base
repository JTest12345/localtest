using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data;
using System.Data.Common;

namespace EICS.Database
{
    /// <summary>
    /// 判定履歴情報[TnLOG]
    /// </summary>
    public class Log
    {
        public int LineCD { get; set; }
        public string EquipmentNO { get; set; }
        public DateTime? MeasureDT { get; set; }
        public int SeqNO { get; set; }
        public int QcParamNO { get; set; }
        public string  MaterialCD { get; set; }
        public string MagazineNO { get; set; }
        public string NascaLotNO { get; set; }
        public decimal? DParameterVAL { get; set; }
        public string SParameterVAL { get; set; }
        public string MessageNM { get; set; }
        public bool ErrorFG { get; set; }
        public bool CheckFG { get; set; }
        public string UpdUserCD { get; set; }

        private DBConnect Connection;

		public struct ParameterSet
		{
			public decimal? DParameterVAL { get; set; }
			public string SParameterVAL { get; set; }
		}

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="conn"></param>
		public Log()
		{
		}

        public Log(DBConnect conn) 
        {
            this.Connection = conn;
        }

        public Log(int lineCD) 
        {
            this.Connection 
                = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false);
        }

        /// <summary>
        /// 登録用判定履歴を取得
        /// </summary>
        /// <param name="lsetInfo">設備情報</param>
        /// <param name="magInfo">マガジン情報</param>
        /// <param name="plmInfo">閾値情報</param>
        /// <param name="parameterVAL">パラメータ値</param>
        /// <param name="errMessageVAL">異常内容</param>
        public void GetInsertData(LSETInfo lsetInfo, MagInfo magInfo, Plm plmInfo, string parameterVAL, string errMessageVAL, DateTime measureDT)
        {
            this.LineCD = lsetInfo.InlineCD;
            this.EquipmentNO = lsetInfo.EquipmentNO;
            this.MeasureDT = magInfo.MeasureDT;
            this.SeqNO = 1;
            this.QcParamNO = plmInfo.QcParamNO;
            this.MaterialCD = magInfo.sMaterialCD;
            this.MagazineNO = magInfo.sMagazineNO;
            this.NascaLotNO = magInfo.sNascaLotNO;
			this.MeasureDT = measureDT;

            if (plmInfo.ManageNM == ParameterInfo.MANAGE_OKNG)
            {
                this.SParameterVAL = parameterVAL;
            }
            else 
            {
				decimal decimalData;

				if (!decimal.TryParse(parameterVAL, out decimalData))
				{
					throw new Exception(string.Format(Constant.MessageInfo.Message_121, parameterVAL));
				}


				this.DParameterVAL = decimalData;
            }

            this.MessageNM = errMessageVAL;

            if (errMessageVAL != string.Empty)
            {
                this.ErrorFG = true;
            }

            this.CheckFG = false;
            this.UpdUserCD = Constant.sUser;
        }

        /// <summary>
        /// DB登録
        /// </summary>
        public void Insert()
        {
            string sql = @" INSERT INTO TnLOG
                            (Inline_CD, Equipment_NO, Measure_DT, Seq_NO, QcParam_NO, 
                            Material_CD, Magazine_NO, NascaLot_NO, DParameter_VAL, SParameter_VAL, Message_NM, 
                            Error_FG, Check_FG, UpdUser_CD)
                            VALUES (@LineCD, @EquipmentNO, @MeasureDT, @SeqNO, @QcParamNO,
                                @MaterialCD, @MagazineNO, @NascaLotNO, @DParameterVAL, @SParameterVAL, @MessageNM,
                                @ErrorFG, @CheckFG, @UpdUserCD) ";

            this.Connection.SetParameter("@LineCD", SqlDbType.Int, Common.GetParameterValue(this.LineCD));
            this.Connection.SetParameter("@EquipmentNO", SqlDbType.Char, Common.GetParameterValue(this.EquipmentNO));
            this.Connection.SetParameter("@MeasureDT", SqlDbType.DateTime, Common.GetParameterValue(this.MeasureDT));
            this.Connection.SetParameter("@SeqNO", SqlDbType.Int, Common.GetParameterValue(this.SeqNO));
            this.Connection.SetParameter("@QcParamNO", SqlDbType.Int, Common.GetParameterValue(this.QcParamNO));
            this.Connection.SetParameter("@MaterialCD", SqlDbType.Char, Common.GetParameterValue(this.MaterialCD));
            this.Connection.SetParameter("@MagazineNO", SqlDbType.Char, Common.GetParameterValue(this.MagazineNO));
            this.Connection.SetParameter("@NascaLotNO", SqlDbType.VarChar, Common.GetParameterValue(this.NascaLotNO));
            this.Connection.SetParameter("@DParameterVAL", SqlDbType.Decimal, Common.GetParameterValue(this.DParameterVAL));
            this.Connection.SetParameter("@SParameterVAL", SqlDbType.VarChar, Common.GetParameterValue(this.SParameterVAL));
            this.Connection.SetParameter("@MessageNM", SqlDbType.VarChar, Common.GetParameterValue(this.MessageNM));

            this.Connection.SetParameter("@ErrorFG", SqlDbType.Bit, Common.GetParameterValue(this.ErrorFG));
            this.Connection.SetParameter("@CheckFG", SqlDbType.Bit, Common.GetParameterValue(this.CheckFG));
            this.Connection.SetParameter("@UpdUserCD", SqlDbType.Char, Common.GetParameterValue(this.UpdUserCD));

            this.Connection.ExecuteNonQuery(sql);
        }

		public static void ErrorUpdate(int lineCd, string equipmentNo, int qcParamNo, string lotNo, string message)
		{
			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCd), "System.Data.SqlClient", false))
			{
				string sql = @" UPDATE TnLog SET Message_NM = @MessageNM, Error_FG = 1
                            WHERE Inline_CD = @LineCD AND Equipment_NO = @EquipmentNO AND QcParam_NO = @QcParamNO AND NascaLot_NO = @NascaLotNO ";

				conn.SetParameter("@LineCD", SqlDbType.Int, lineCd);
				conn.SetParameter("@EquipmentNO", SqlDbType.Char, equipmentNo);
				conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNo);
				conn.SetParameter("@NascaLotNO", SqlDbType.VarChar, lotNo);
				conn.SetParameter("@MessageNM", SqlDbType.VarChar, message);

				conn.ExecuteNonQuery(sql);
			}
		}

		/// <summary>
		/// 未だエラー表示していないデータを取得
		/// </summary>
		/// <returns></returns>
		public static List<Log> GetNotYetDisplayedData(int lineCd, string equipmentNo)
		{
			List<Log> logs = new List<Log>();

			string connStr = ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCd);
			if (string.IsNullOrEmpty(connStr))
			{
				return logs;
			}

			using (DBConnect conn = DBConnect.CreateInstance(connStr, "System.Data.SqlClient", false))
			{
				string sql = @" SELECT QcParam_NO, NascaLot_NO FROM TnLOGOutSide WITH(nolock) 
                                WHERE Inline_CD = @LineCd AND FinishedDisplay_FG = 0 AND Equipment_NO = @Equipment_NO AND Measure_DT >= @Measure_DT ";

				conn.SetParameter("@LineCd", SqlDbType.Int, lineCd);
				conn.SetParameter("@Equipment_NO", SqlDbType.Char, equipmentNo);
				conn.SetParameter("@Measure_DT", SqlDbType.DateTime, System.DateTime.Now.AddMonths(-1));

				using (DbDataReader rd = conn.GetReader(sql))
				{
					while (rd.Read())
					{
						Log l = new Log(conn);

						l.LineCD = lineCd;
						l.EquipmentNO = equipmentNo;
						l.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
						l.NascaLotNO = rd["NascaLot_NO"].ToString().Trim();

						logs.Add(l);
					}
				}
			}

			List<Log> retv = new List<Log>();

			foreach (Log log in logs)
			{
				Log l = GetData(log.LineCD, log.EquipmentNO, log.QcParamNO, log.NascaLotNO);
				if (l == null) continue;

				retv.Add(l);
			}

			return retv;

		}

		/// <summary>
		/// エラー表示したデータの完了処理
		/// </summary>
		/// <param name="lineCd"></param>
		/// <param name="equipmentNo"></param>
		/// <param name="qcParamNo"></param>
		/// <param name="lotNo"></param>
		public static void CompleteDisplayedData(int lineCd, string equipmentNo, int qcParamNo, string lotNo)
		{
			if (string.IsNullOrEmpty(equipmentNo) || qcParamNo == 0 || string.IsNullOrEmpty(lotNo))
			{
				throw new ApplicationException("必須条件が不正の為、CompleteDisplayedDataの処理に失敗しました。");
			}

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCd), "System.Data.SqlClient", false))
			{
				string sql = @" UPDATE TnLOGOutSide SET FinishedDisplay_FG = 1
                                WHERE (Inline_CD = @LineCD 
                                AND Equipment_NO = @EquipmentNO AND QcParam_NO = @QcParamNO AND NascaLot_NO = @NascaLotNO) ";

				conn.SetParameter("@LineCD", SqlDbType.Int, lineCd);
				conn.SetParameter("@EquipmentNO", SqlDbType.Char, equipmentNo);
				conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNo);
				conn.SetParameter("@NascaLotNO", SqlDbType.VarChar, lotNo);

				conn.ExecuteNonQuery(sql);
			}
		}

		public static List<Log> GetData(int lineCd, string equipmentNo, int? qcParamNo, string lotNo, bool isError)
		{
			List<Log> retv = new List<Log>();

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCd), "System.Data.SqlClient", false))
			{
				string sql = @" SELECT Equipment_NO, Measure_DT, Seq_NO, QcParam_NO, Material_CD, Magazine_NO, NascaLot_NO, DParameter_VAL, SParameter_VAL,
								Message_NM, Check_FG, UpdUser_CD
								FROM TnLog WITH(nolock) WHERE (Del_FG = 0) AND (Inline_CD = @LineCd)";

				conn.SetParameter("@LineCd", SqlDbType.Int, lineCd);

				if (string.IsNullOrEmpty(equipmentNo) == false)
				{
					sql += " AND Equipment_NO = @EquipmentNO ";
					conn.SetParameter("@EquipmentNO", SqlDbType.Char, equipmentNo);
				}

				if (string.IsNullOrEmpty(lotNo) == false)
				{
					sql += " AND NascaLot_NO = @NascaLotNO ";
					conn.SetParameter("@NascaLotNO", SqlDbType.VarChar, lotNo);
				}

				if (qcParamNo.HasValue)
				{
					sql += " AND QcParam_NO = @QcParamNO ";
					conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNo.Value);
				}

				if (isError)
				{
					sql += " AND Error_FG = @ErrorFG ";
					conn.SetParameter("@ErrorFG", SqlDbType.Bit, true);
				}

				using (DbDataReader rd = conn.GetReader(sql))
				{
					while (rd.Read())
					{
						Log l = new Log(conn);
						l.EquipmentNO = rd["Equipment_NO"].ToString().Trim();
						l.MeasureDT = Convert.ToDateTime(rd["Measure_DT"]);
						l.SeqNO = Convert.ToInt32(rd["Seq_NO"]);
						l.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
						l.MaterialCD = rd["Material_CD"].ToString().Trim();
						l.MagazineNO = rd["Magazine_NO"].ToString().Trim();
						l.NascaLotNO = rd["NascaLot_NO"].ToString().Trim();
						l.DParameterVAL = Convert.ToDecimal(rd["DParameter_VAL"]);
						l.SParameterVAL = rd["SParameter_VAL"].ToString().Trim();
						l.MessageNM = rd["Message_NM"].ToString().Trim();
						l.CheckFG = Convert.ToBoolean(rd["Check_FG"]);
						l.UpdUserCD = rd["UpdUser_CD"].ToString().Trim();
						retv.Add(l);
					}
				}
			}

			return retv;
		}

		public static Log GetData(int lineCd, string equipmentNo, int qcParamNo, string lotNo)
		{
			List<Log> logs = GetData(lineCd, equipmentNo, qcParamNo, lotNo, false);
			if (logs.Count == 0)
			{
				return null;
			}
			else
			{
				return logs.OrderByDescending(l => l.MeasureDT).First();
			}
		}

		/// <summary>
		/// 文字、数値の取得値の内、存在する方を返す
		/// </summary>
		/// <param name="stringParameter"></param>
		/// <param name="decimalParameter"></param>
		/// <returns></returns>
		public static string GetParameterValue(string stringParameter, decimal? decimalParameter)
		{
			if (string.IsNullOrEmpty(stringParameter) == false)
			{
				return stringParameter;
			}
			else if (decimalParameter.HasValue)
			{
				return decimalParameter.Value.ToString();
			}
			else
			{
				return "文字・数値いずれの取得値も空です。";
			}
		}

		public static ParameterSet GetParameter(Plm plmInfo,string strParam, out string logMsg)
		{
			ParameterSet paramSet = new ParameterSet();
			decimal dValue;

			logMsg = string.Empty;
			strParam = strParam.ToUpper();

			//NG判定してメッセージ入れ込み
			if (plmInfo != null && string.IsNullOrEmpty(plmInfo.ParameterVAL))//0:数値で、管理限界情報を取得出来ている場合
			{
				switch (strParam)
				{
					case "":
						if (plmInfo.ParameterMAX.HasValue)
						{
							paramSet.DParameterVAL = plmInfo.ParameterMAX.Value + 1;

							logMsg = string.Format("管理番号:{0} :ファイルから値が取得出来なかった為、MAX閾値+1の値を代入しました。", plmInfo.QcParamNO);
						}
						else if (plmInfo.ParameterMIN.HasValue)
						{
							paramSet.DParameterVAL = plmInfo.ParameterMIN.Value - 1;

							logMsg = string.Format("管理番号:{0} :ファイルから値が取得出来なかった為、MIN閾値-1の値を代入しました。", plmInfo.QcParamNO);
						}
						else
						{
							paramSet.DParameterVAL = null;
						}
						break;

					case Structure.MachineFile.NULL_Str:
						paramSet.DParameterVAL = null;
						break;
					default:
						if (decimal.TryParse(strParam, out dValue) == false)//sValueが0に近い値の場合、失敗する為その対応
						{
							logMsg = string.Format("ﾊﾟﾗﾒﾀ番号:{0} :ファイルから取得した値の数値変換に失敗した為、0を格納します。変換対象:{1}", plmInfo.QcParamNO, strParam);
						}

						paramSet.DParameterVAL = decimal.Round(dValue, 4, MidpointRounding.AwayFromZero);				
						break;
				}

				paramSet.SParameterVAL = null;
			}
			else//1:文字列か、管理限界情報を取得出来ていない場合
			{
				paramSet.DParameterVAL = null;
				paramSet.SParameterVAL = strParam;
			}

			return paramSet;
		}
    }
}
	//switch(plmInfo.ManageNM)
	//{
	//	case Constant.sMAXMIN:
				
	//		if (plmInfo.ParameterMAX < dValue)
	//		{
	//			F01_MachineWatch.sp.PlayLooping();

	//			sMessage = string.Format(Constant.MessageInfo.Message_22, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, "MAX", dValue, plmInfo.ParameterMAX, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);
	//			ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
	//			errMessageList.Add(errMessageInfo);

	//			fErr = true;
	//			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
	//		}
	//		else if (plmInfo.ParameterMIN > dValue)
	//		{
	//			F01_MachineWatch.sp.PlayLooping();

	//			//sMessage = "[{0}/{1}号機/{2}]が管理限界値(MIN)を越えました。\r\n取得値={3},閾値MIN={4}";
	//			sMessage = string.Format(Constant.MessageInfo.Message_22, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, "MIN", dValue, plmInfo.ParameterMIN, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);
	//			ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
	//			errMessageList.Add(errMessageInfo);

	//			fErr = true;
	//			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
	//		}

	//		break;

	//	case Constant.sMAX:
				
	//		if (plmInfo.ParameterMAX < dValue)
	//		{
	//			F01_MachineWatch.sp.PlayLooping();

	//			//sMessage = "[{0}/{1}号機/{2}]が管理限界値(MAX)を越えました。\r\n取得値={3},閾値MAX={4}";
	//			sMessage = string.Format(Constant.MessageInfo.Message_22, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, "MAX", dValue, plmInfo.ParameterMAX, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);
	//			ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
	//			errMessageList.Add(errMessageInfo);

	//			fErr = true;
	//			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
	//		}

	//		break;

	//	default:

	//		string errMsg = string.Format("未定義のTmPLM.Manage_Nmが設定されいます。設定値:{0}", plmInfo.ManageNM);
	//		throw new ApplicationException(errMsg);
	//		break;

	//}

	//if (!fErr)
	//{
	//	//管理限界値超えが発生していない時は、工程狙い値超えの確認をし、超えていればエラーメッセージを表示
	//	sMessageInnerLimit = ParameterInfo.CheckInnerLimit(plmInfo, dValue.ToString(), EquiInfo, MagInfo.sNascaLotNO);
	//	if (!string.IsNullOrEmpty(sMessageInnerLimit))
	//	{
	//		F01_MachineWatch.sp.PlayLooping();

	//		ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessageInnerLimit, Color.Blue);
	//		errMessageList.Add(errMessageInfo);

	//		fErrInnerLimit = true;
	//	}
	//}



	//if (plmInfo.ParameterVAL.ToUpper() != strParam.ToUpper() && plmInfo.ParameterVAL.ToUpper() != Constant.sOKStrings)
	//{
	//	F01_MachineWatch.sp.PlayLooping();
	//	//sMessage = "[{0}/{1}号機/{2}]の設定値に誤りがあります。\r\n取得値={3},閾値={4}";
	//	sMessage = string.Format(Constant.MessageInfo.Message_23, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, strParam, plmInfo.ParameterVAL, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);
	//	ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
	//	errMessageList.Add(errMessageInfo);

	//	fErr = true;
	//	log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
	//}
