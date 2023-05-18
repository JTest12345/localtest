using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GEICS.Database;
using SLCommonLib.DataBase;
using System.Data.Common;

namespace GEICS.Function
{
	class MasterChk
	{
		private static List<string> NgMsgList = new List<string>();

		private const string MSG_CheckSkip_TYPE_PARAMNO_MODEL_FMTNO = "{0}のチェックをスキップしました。,Type:{1},QcParamNo:{2},Model:{3},FileFmtNo:{4}";
		private const string MSG_NoRecord_TYPE_PARAMNO_MODEL_FMTNO = "{0}にレコードが存在しません。,Type:{1},QcParamNo:{2},Model:{3},FileFmtNo:{4}";
		private const string MSG_EXCEPTION_BY_GETSINGLE = "{0}からの取得値を1レコードに絞り込んで取得する処理で例外発生,type:{1},qcParamNo:{2},Model:{3},FileFmtNo:{4},例外Msg:{5},ｽﾀｯｸﾄﾚｰｽ:{6}";

		private const string TABLE_PRM = "TmPRM";
		private const string TABLE_PLM = "TmPLM";
		private const string TABLE_PRM_OR_PLM = "TmPRMかTmPLM";
		private const string TABLE_FILEFMTWB = "TmFileFmtWB";
		private const string TABLE_FILEFMT = "TmFileFmt";
		private const string TABLE_FILEFMTTYPE = "TmFileFmtType";

		private const string NONE_TERM = "指定無";

		public MasterChk()
		{
		}

		public static bool IsThereDB(string targetServ, string targetDB)
		{
			#region 指定サーバの指定DBの存在チェック
			using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(
				string.Format(Properties.Settings.Default.ConStrServ, targetServ, "master", ConnectQCIL.CONNECT_INLINE_USER_ID, ConnectQCIL.CONNECT_INLINE_USER_PASS), "System.Data.SqlClient", false))
			{
			    string sql = @" SELECT COUNT(name) FROM sysdatabases WHERE name = @DBNM ";

				conn.SetParameter("@DBNM", System.Data.DbType.String, targetDB);

				object ct = conn.ExecuteScalar(sql);

			    if((int)ct == 1)
			    {
			        return true;
			    }
			    else
			    {
			        return false;
			    }
			}
			#endregion
		}

		/// <summary>
		/// TmPLMを起点にしたWBパラメータ関連のマスタチェック機能
		/// </summary>
		/// <param name="targetServ"></param>
		/// <param name="plmList"></param>
		/// <returns>OK:true / NG:false</returns>
		public static bool ChkParamRelatedMasterFromPLM_WB(string targetServ, List<PlmInfo> plmList)
		{
			NgMsgList = new List<string>();

			bool isThereLENS = IsThereDB(targetServ, Constant.LENS_DB_NM);

			#region Plmを基にして足りて無いマスタが無いかチェック
			foreach (PlmInfo plm in plmList)
			{
				FileFmtType fileFmtType = GetFileFmtTypeWithCheck(isThereLENS, plm.MaterialCD);

				if (fileFmtType == null)
				{
					NgMsgList.Add(string.Format(MSG_CheckSkip_TYPE_PARAMNO_MODEL_FMTNO, "TmFILEFMT、TmPRM", plm.MaterialCD, NONE_TERM, NONE_TERM, NONE_TERM));
					continue;
				}

				MachineLogFormat.WirebonderFile fileFmtWb = GetFileFmtWBWithCheck(plm.ModelNM, plm.QcParamNO, fileFmtType.FileFmtNo);

				//後ろでfileFmtWbを使用していないのでスキップする必要無しの為、下記削除
				//if (fileFmtWb == null)
				//{
				//    NgMsgList.Add(string.Format(MSG_CheckSkip_TYPE_PARAMNO_MODEL_FMTNO, "TmPRM", , plm.QcParamNO, plm.ModelNM, fileFmtType.FileFmtNo));
				//    continue;
				//}

				Prm prm = GetPrmWithCheck(plm.MaterialCD, plm.QcParamNO);
			}
			#endregion

			string msg = string.Format("マスタチェック完了 ﾒｯｾｰｼﾞ:{0}件\r\n", NgMsgList.Count);
			msg += string.Join("\r\n", NgMsgList.ToArray());

			F100_MsgBox.Show(msg);

			if (NgMsgList.Count > 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static List<PlmInfo> GetExclusionWbFmt(List<PlmInfo> plmList)
		{
			List<string> wbModelList = MachineLogFormat.WirebonderFile.GetModelNm(null, null, null, null);

			foreach (PlmInfo plm in plmList)
			{
				Console.Write(plm.QcParamNO.ToString());
			}

			List<PlmInfo> retv = plmList;

			foreach(string wbModel in wbModelList)
			{
				retv = retv.Where(l => l.ModelNM != wbModel).ToList();
			}

			return retv;
		}

		public static bool ChkParamRelatedMasterFromPLM(string targetServ, List<PlmInfo> plmList)
		{
			NgMsgList = new List<string>();

			bool isThereLENS = IsThereDB(targetServ, Constant.LENS_DB_NM);

			plmList = GetExclusionWbFmt(plmList);

			#region Plmを基にして足りて無いマスタが無いかチェック
			foreach (PlmInfo plm in plmList)
			{
				FileFmtType fileFmtType = GetFileFmtTypeWithCheck(isThereLENS, plm.MaterialCD);

				if (fileFmtType == null)
				{
					NgMsgList.Add(string.Format(MSG_CheckSkip_TYPE_PARAMNO_MODEL_FMTNO, "TmFILEFMT、TmPRM", plm.MaterialCD, NONE_TERM, NONE_TERM, NONE_TERM));
					continue;
				}

				GetFileFmtWithCheck(plm.ModelNM, plm.QcParamNO, null);

				Prm prm = GetPrmWithCheck(plm.MaterialCD, plm.QcParamNO);
			}
			#endregion

			string msg = string.Format("マスタチェック完了 チェックNG:{0}件\r\n", NgMsgList.Count);
			msg += string.Join("\r\n", NgMsgList.ToArray());

			F100_MsgBox.Show(msg);

			if (NgMsgList.Count > 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		//public bool Checkhogehoge(string targetServ, string typeCD)
		//{
		//    bool isThereLENS = IsThereDB(targetServ, Constant.LENS_DB_NM);

		//    FileFmtType fileFmtType = GetFileFmtTypeWithCheck(isThereLENS, typeCD);

		//    List<MachineLogFormat.WirebonderFile> fileFmtWBList = GetFileFmtWBWithCheck(fileFmtType.FileFmtNo);

		//    using(DBConnect conn = DBConnect.CreateInstance(Constant.StrQCIL, false))
		//    {
		//        foreach (MachineLogFormat.WirebonderFile fileFmtWB in fileFmtWBList)
		//        {
		//        }
		//    }
		//}

		//public static List<MachineLogFormat.WirebonderFile> GetFileFmtWBWithCheck(int? fmtNo)
		//{
		//    // FileFmtWBからレコード取得
		//    List<MachineLogFormat.WirebonderFile> fileFmtWbList = MachineLogFormat.WirebonderFile.GetData(null, null, null, fmtNo);

		//    #region FileFmtWBのレコード数チェック
		//    if (fileFmtWbList.Count == 0)
		//    {
		//        NgMsgList.Add(string.Format(MSG_NoRecord_TYPE_PARAMNO_MODEL_FMTNO, TABLE_FILEFMTWB, NONE_TERM, qcParamNo, modelNM, fmtNo));
		//    }
		//    #endregion

		//    return fileFmtWbList;
		//}

		public static void GetFileFmtWithCheck(string modelNM, int qcParamNo, string prefix)
		{
			// FileFmtWBからレコード取得
			List<MachineLogFormat.FileFormat> fileFmtList = MachineLogFormat.FileFormat.GetData(qcParamNo, modelNM, null);

			#region FileFmtのレコード数チェック
			if (fileFmtList.Count == 0)
			{
				NgMsgList.Add(string.Format(MSG_NoRecord_TYPE_PARAMNO_MODEL_FMTNO, TABLE_FILEFMT, NONE_TERM, qcParamNo, modelNM, NONE_TERM));
				return;
			}
			#endregion

			#region FileFmtの1レコード絞り込み取得
			try
			{
				foreach (MachineLogFormat.FileFormat fileFmt in fileFmtList)
				{
					if (fileFmt.ColumnNO.HasValue == false && string.IsNullOrEmpty(fileFmt.HeaderNM) && string.IsNullOrEmpty(fileFmt.SearchNM))
					{
						NgMsgList.Add(string.Format("TmFileFmtにおいてﾌｧｲﾙからのﾃﾞｰﾀ抽出に必要なﾌｨｰﾙﾄﾞが全て未設定です。Type:{1},QcParamNo:{2},Model:{3},FileFmtNo:{4}", string.Empty, qcParamNo, modelNM, string.Empty));
					}
				}
			}
			catch (Exception err)
			{
				NgMsgList.Add(string.Format(MSG_EXCEPTION_BY_GETSINGLE, TABLE_FILEFMT, modelNM, qcParamNo, string.Empty, err.Message, err.StackTrace));
				return;
			}
			#endregion
		}

		public static MachineLogFormat.WirebonderFile GetFileFmtWBWithCheck(string modelNM, int qcParamNo, int? fmtNo)
		{
			MachineLogFormat.WirebonderFile fileFmtWb = new MachineLogFormat.WirebonderFile();

			// FileFmtWBからレコード取得
			List<MachineLogFormat.WirebonderFile> fileFmtWbList = MachineLogFormat.WirebonderFile.GetData(qcParamNo, modelNM, null, fmtNo);

			#region FileFmtWBのレコード数チェック
			if (fileFmtWbList.Count == 0)
			{
				NgMsgList.Add(string.Format(MSG_NoRecord_TYPE_PARAMNO_MODEL_FMTNO, TABLE_FILEFMTWB, NONE_TERM, qcParamNo, modelNM, fmtNo));
				return null;
			}
			#endregion

			#region FileFmtWBの1レコード絞り込み取得
			try
			{
				fileFmtWb = fileFmtWbList.Single();
			}
			catch (Exception err)
			{
				NgMsgList.Add(string.Format(MSG_EXCEPTION_BY_GETSINGLE, TABLE_FILEFMTWB, modelNM, qcParamNo, fmtNo, err.Message, err.StackTrace));
				return null;
			}
			#endregion

			return fileFmtWb;
		}

		public static Prm GetPrmWithCheck(string typeCD, int qcParamNo)
		{
			#region 変数初期化
			List<Prm> prmList = Prm.GetDataFromParamNo(string.Empty, Constant.StrQCIL, qcParamNo);
			Prm prm = new Prm();
			#endregion

			#region Prmのレコード数チェック
			if (prmList.Count == 0)
			{
				NgMsgList.Add(string.Format(MSG_NoRecord_TYPE_PARAMNO_MODEL_FMTNO, TABLE_PRM, typeCD, qcParamNo, NONE_TERM, NONE_TERM));
			}
			#endregion

			#region Prmの1レコード絞り込み取得
			try
			{
				prm = prmList.Single();
			}
			catch (Exception err)
			{
				NgMsgList.Add(string.Format(MSG_EXCEPTION_BY_GETSINGLE, TABLE_PRM, typeCD, qcParamNo, NONE_TERM, NONE_TERM, err.Message, err.StackTrace));
			}
			#endregion

			return prm;
		}

		public static FileFmtType GetFileFmtTypeWithCheck(bool isThereLENS, string typeCD)
		{
			#region 変数初期化
			string strDBAndTable = string.Empty;
			List<FileFmtType> fileFmtTypeList = new List<FileFmtType>();
			FileFmtType fileFmtType = new FileFmtType();
			#endregion

			#region LENSのDB有無によりLENS.TmTypeかQCIL.TmFILEFMTTYPEを切り替えてレコード取得
			if (isThereLENS)
			{
				fileFmtTypeList = FileFmtType.GetFromLENS(typeCD, null, null);
			}
			else
			{
				fileFmtTypeList = FileFmtType.GetFromQCIL(typeCD, null, null);
			}
			#endregion

			#region fileFmtTypeのレコード数チェック
			if (fileFmtTypeList.Count == 0)
			{
				if (isThereLENS)
				{
					strDBAndTable = "LENS.TmType";
				}
				else
				{
					strDBAndTable = "QCIL.TmFILEFMTTYPE";
				}

				NgMsgList.Add(string.Format(MSG_NoRecord_TYPE_PARAMNO_MODEL_FMTNO, strDBAndTable, typeCD, NONE_TERM, NONE_TERM, NONE_TERM));
				return null;
			}
			#endregion

			#region fileFmtTypeの1レコード絞り込み取得
			try
			{
				fileFmtType = fileFmtTypeList.Single();
			}
			catch (Exception err)
			{
				NgMsgList.Add(string.Format(MSG_EXCEPTION_BY_GETSINGLE, strDBAndTable, typeCD, NONE_TERM, NONE_TERM, NONE_TERM, err.Message, err.StackTrace));
				return null;
			}
			#endregion

			return fileFmtType;
		}

		//public Plm GetPlmWithCheck(int qcParamNo, string typeCD)
		//{
		//    List<Plm> plmList = new List<Plm>();

		//    using (DBConnect conn = DBConnect.CreateInstance(Constant.StrQCIL, false))
		//    {
		//        plmList = Plm.GetDatas(conn, typeCD, null, qcParamNo, false, null);
		//    }

		//    if (plmList.Count == 0)
		//    {
		//        NgMsgList.Add(string.Format(MSG_NoRecord_TYPE_PARAMNO_MODEL_FMTNO, TABLE_PRM_OR_PLM, typeCD, qcParamNo, string.Empty, string.Empty));
		//    }
		//}
	}
}
