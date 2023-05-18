using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml;
using SLCommonLib.Commons;

namespace GEICS
{
    public class Common
    {
        public static bool notUseTmQdiwFG = false;
        public static int nLineCD = int.MinValue;
        public bool GetMapSetting()
        {
            string sWork;
            bool flg = false;

            System.IO.FileStream fs = new System.IO.FileStream("C:\\QCIL\\SettingFiles\\QCIL.xml", System.IO.FileMode.Open, System.IO.FileAccess.Read);

            XmlDocument doc = new XmlDocument();
            doc.Load((System.IO.Stream)fs);

            try
            {
                sWork = doc.SelectSingleNode("//configuration/qcil_info/Map").Attributes["value"].Value;
            }
            catch
            {//設定がなければ、OFF
                sWork = "OFF";
            }

            fs.Close();

            if (sWork == "ON")
            {
                flg = true;//MAPラインである
            }
            return flg;
        }

        #region データベース処理
        public string GetInlineString(int nLineNO)
        {
            string sLine = "";

            string sqlCmdTxt = @"SELECT Inline_NM FROM TmLINE WITH(NOLOCK) WHERE Inline_CD=@INLINECD AND Del_FG=0";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlParameter pInlinecd = connect.Command.Parameters.Add("@INLINECD", SqlDbType.Int);
                pInlinecd.Value = nLineNO;

                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    while (reader.Read())
                    {
                        sLine = Convert.ToString(reader["Inline_NM"]).Trim();
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return sLine;
        }

        /// <summary>
        /// [設備番号][回収タイミング][パラメータ名]を引数に入れると、TmPRCから"パラメータ連番"を取得して戻す
        /// </summary>
        /// <param name="sEquipmentNO"></param>
        /// <param name="sTimingNM"></param>
        /// <param name="sParameterNM"></param>
        /// <returns>sQcParamNO</returns>
        public string GetTvPRM_Record(string sClassNM, string sParameterNM)
        {
            string sDBInfo = "";

            string BaseSql = "SELECT QcParam_NO,Class_NM,Timing_NM,Parameter_NM,Manage_NM FROM TvPRM WITH(NOLOCK) WHERE Class_NM='{0}' AND Parameter_NM='{1}'";
            string sqlCmdTxt = string.Format(BaseSql,sClassNM,sParameterNM);
            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        sDBInfo = Convert.ToString(reader["QcParam_NO"]).Trim();
                        sDBInfo = sDBInfo + "," + Convert.ToString(reader["Class_NM"]).Trim();
                        sDBInfo = sDBInfo + "," + Convert.ToString(reader["Timing_NM"]).Trim();
                        sDBInfo = sDBInfo + "," + Convert.ToString(reader["Parameter_NM"]).Trim();
                        sDBInfo = sDBInfo + "," + Convert.ToString(reader["Manage_NM"]).Trim();
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();

                }
            }

            return sDBInfo;
        }
        /// <summary>
        /// [パラメータ名][回収タイミング]を引数に入れると、TvPRMから"パラメータ連番"を取得して戻す
        /// </summary>
        /// <param name="sEquipmentNO"></param>
        /// <param name="sTimingNM"></param>
        /// <param name="sParameterNM"></param>
        /// <returns>sQcParamNO</returns>
		public string GetTvPRM_Record(string sClassNM, string sTimingNM, string sParameterNM)
		{
			string sDBInfo = "";

			string BaseSql = "SELECT QcParam_NO,Class_NM,Timing_NM,Parameter_NM,Manage_NM FROM TvPRM WITH(NOLOCK) WHERE Class_NM='{0}' AND Parameter_NM='{1}' AND Timing_NM='{2}'";
			string sqlCmdTxt = string.Format(BaseSql, sClassNM, sParameterNM, sTimingNM);
			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				SqlDataReader reader = null;
				try
				{
					connect.Command.CommandText = sqlCmdTxt;
					reader = connect.Command.ExecuteReader();
					while (reader.Read())
					{
						sDBInfo = Convert.ToString(reader["QcParam_NO"]).Trim();
						sDBInfo = sDBInfo + "," + Convert.ToString(reader["Class_NM"]).Trim();
						sDBInfo = sDBInfo + "," + Convert.ToString(reader["Timing_NM"]).Trim();
						sDBInfo = sDBInfo + "," + Convert.ToString(reader["Parameter_NM"]).Trim();
						sDBInfo = sDBInfo + "," + Convert.ToString(reader["Manage_NM"]).Trim();
					}
				}
				finally
				{
					if (reader != null) reader.Close();
					connect.Close();

				}
			}
			return sDBInfo;
		}
        //Title文字列生成用
        //引数：Plant_CD
        //戻り値：文字列"所有部署 *号機"
		//public string GetMachineSeqNo(string equipmentNo)
		//{
		//    string sMachineSeq="";
		//    //string BaseSql = "Select MachinSeq_NO From NtmEqui Where Plant_CD ='{0}'";
		//    string BaseSql = "Select MachinSeq_NO From TmEQUI WITH(NOLOCK) Where Equipment_NO ='{0}'";
		//    string sqlCmdTxt = string.Format(BaseSql, equipmentNo.Trim());

		//    using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
		//    {
		//        SqlDataReader reader = null;
		//        try
		//        {
		//            connect.Command.CommandText = sqlCmdTxt;
		//            reader = connect.Command.ExecuteReader();
		//            while (reader.Read())
		//            {
		//                sMachineSeq = Convert.ToString(reader["MachinSeq_NO"]).Trim();
		//            }
		//        }
		//        finally
		//        {
		//            if (reader != null) reader.Close();
		//            connect.Close();
		//        }
		//    }
		//    return sMachineSeq;
		//}
        public string GetManageNM(int sQcParamNo)
        {
            string sManageNM = "";
            string BaseSql = "SELECT Manage_NM FROM TmPRM WITH(NOLOCK) Where QcParam_NO={0}";
            string sqlCmdTxt = string.Format(BaseSql, sQcParamNo);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        sManageNM = Convert.ToString(reader["Manage_NM"]).Trim();
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return sManageNM;
        }

        public string GetInspectionNM(int nQcParamNO) 
        {
            string swInspectionNM = "";
            string BaseSql = "";
			if (Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG)
            {
                BaseSql = "SELECT DISTINCT Inspection_NM FROM [TvQDIW_Map]  WITH(NOLOCK) WHERE Process_NO Like '%{0}%'";
            }
            else
            {
                BaseSql = "SELECT DISTINCT Inspection_NM FROM [TvQDIW]  WITH(NOLOCK) WHERE Process_NO Like '%{0}%'";
            }


            string sqlCmdTxt = string.Format(BaseSql, "," + nQcParamNO + ",");

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        swInspectionNM = Convert.ToString(reader["Inspection_NM"]).Trim();
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }

            return swInspectionNM;
        }
        public string GetMachineModel(string equipmentNo)
        {
            string sModelNM = "";
            //string BaseSql = "Select Model_NM From NtmEqui Where Plant_CD ='{0}'";
            string BaseSql = "Select Model_NM From TmEQUI WITH(NOLOCK) Where Equipment_NO ='{0}'";
            string sqlCmdTxt = string.Format(BaseSql, equipmentNo.Trim());

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        sModelNM = Convert.ToString(reader["Model_NM"]).Trim();
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return sModelNM;
        }

        // [傾向管理No][期間]の傾向管理データを入れると、リスト([時間][管理データ])を返す
		//public SortedList<DateTime, string> GetTvQCILString(int nLineNo,string sEquiNo,string sQcParamNo, DateTime dtStart, DateTime dtEnd)
		//{
		//    string BaseSql = "", sqlCmdTxt = "";
		//    SortedList<DateTime,string> retv = new SortedList<DateTime, string>();


		//    BaseSql = "Select Measure_DT,SParameter_VAL From TvQCIL WITH(NOLOCK) Where Inline_CD={0} AND Equipment_NO='{1}'AND QcParam_NO={2} AND (Measure_DT >= '{3}' AND Measure_DT <= '{4}')";
		//    sqlCmdTxt = string.Format(BaseSql, nLineNo,sEquiNo.Trim(), Convert.ToInt32(sQcParamNo.Trim()), dtStart, dtEnd);

		//    using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
		//    {
		//        SqlDataReader reader = null;
		//        try
		//        {
		//            connect.Command.CommandText = sqlCmdTxt;
		//            reader = connect.Command.ExecuteReader();
		//            while (reader.Read())
		//            {
		//                DateTime key = Convert.ToDateTime(reader["Measure_DT"].ToString());
		//                string value = reader["SParameter_VAL"].ToString();
		//                retv.Add(key, value);
		//            }
		//        }
		//        finally
		//        {
		//            if (reader != null) reader.Close();
		//            connect.Close();
		//        }
		//    }
		//    return retv;

		//}

        // [傾向管理No][期間]の傾向管理データを入れると、リスト([時間][管理データ])を返す
		//public SortedList<DateTime, double> GetTvQCILDouble(int nLineNo, string sEquiNo, string sQcParamNo, string sType,DateTime dtStart, DateTime dtEnd)
		//{
		//    string BaseSql = "", sqlCmdTxt = "";
		//    SortedList<DateTime, double> retv = new SortedList<DateTime, double>();

		//    BaseSql = "Select Measure_DT,DParameter_VAL From TvQCIL WITH(NOLOCK) Where Inline_CD={0} AND Equipment_NO='{1}' AND QcParam_NO={2} AND Material_CD='{3}' AND (Measure_DT >= '{4}' AND Measure_DT <= '{5}')";
		//    sqlCmdTxt = string.Format(BaseSql, nLineNo, sEquiNo.Trim(), Convert.ToInt32(sQcParamNo.Trim()),sType, dtStart, dtEnd);

		//    using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
		//    {
		//        SqlDataReader reader = null;
		//        try
		//        {
		//            connect.Command.CommandText = sqlCmdTxt;
		//            reader = connect.Command.ExecuteReader();
		//            while (reader.Read())
		//            {
		//                DateTime key = Convert.ToDateTime(reader["Measure_DT"].ToString());
		//                double value = Convert.ToDouble(reader["DParameter_VAL"]);
		//                retv.Add(key, value);
		//            }
		//        }
		//        finally
		//        {
		//            if (reader != null) reader.Close();
		//            connect.Close();
		//        }
		//    }
		//    return retv;
		//}

        public int GetLineNo(string sLineNM)
        {
            int nlinecd = 0;
            string sqlCmdTxt = "SELECT DISTINCT [Inline_CD] FROM [TmLINE] WITH(NOLOCK) WHERE Del_FG <> '1' AND Inline_NM=@INLINENM";
            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.Parameters.Add("@INLINENM", SqlDbType.Char).Value = sLineNM.Trim();

                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    while (reader.Read())
                    {
                        nlinecd = Convert.ToInt32(reader["Inline_CD"]);
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return nlinecd;
        }

        // [インライン番号]を入れると、設備一覧を返す
		//public string GetTmLSET(string sInlineCD)
		//{
		//    string BaseSql = "", sqlCmdTxt = "";
		//    string sEquipmentNO = "@";

		//    BaseSql = "SELECT Equipment_NO From  TmLSET WITH(NOLOCK) Where Inline_CD={0}";
		//    sqlCmdTxt = string.Format(BaseSql, sInlineCD);
		//    using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
		//    {
		//        SqlDataReader reader = null;
		//        try
		//        {
		//            connect.Command.CommandText = sqlCmdTxt;
		//            reader = connect.Command.ExecuteReader();
		//            while (reader.Read())
		//            {
		//                sEquipmentNO = sEquipmentNO + "," + Convert.ToString(reader["Equipment_NO"]).Trim();
		//            }
		//        }
		//        finally
		//        {
		//            if (reader != null) reader.Close();
		//            connect.Close();
		//        }
		//    }
		//    sEquipmentNO = sEquipmentNO.Replace("@,", "");
		//    return sEquipmentNO;
		//}

        public string GetTmPRM(int nQcparamNO)
        {
            string BaseSql = "", sqlCmdTxt = "";
            string sParamNM="";

            //最新の対応履歴番号取得
            //BaseSql = "Select MAX(Confirm_NO) AS Confirm_NO From TnLOGCnfm Where Inline_CD={0} AND Equipment_NO='{1}' AND Measure_DT='{2}' AND Seq_NO={3} AND QcParam_NO={4} AND Del_FG <> '1'";
            BaseSql = "Select [Parameter_NM] From TmPRM WITH(NOLOCK) Where [QcParam_NO]={0}";
            sqlCmdTxt = string.Format(BaseSql, nQcparamNO);
            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        sParamNM = Convert.ToString(reader["Parameter_NM"]);
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }

            //nConfirmNO += 1;//対応履歴+1
            return sParamNM;
        }

        // [装置型式][傾向管理No][Type]を入れると、文字列"MAX,MIN"を返す
        public string GetTmPLM(string sModelNM, int nQcParamNO,string sMaterialCD)
        {
            string BaseSql = "", sqlCmdTxt = "";
            string sLimit="";

            BaseSql = "Select Parameter_MAX,Parameter_MIN,QcLine_MAX,QcLine_MIN,AimLine_VAL,AimRate_VAL From TmPLM WITH(NOLOCK) Where Model_NM='{0}' AND QcParam_NO={1} AND Material_CD='{2}'";
            sqlCmdTxt = string.Format(BaseSql, sModelNM.Trim(), nQcParamNO, sMaterialCD.Trim());

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        sLimit = Convert.ToString(reader["Parameter_MAX"]);
                        sLimit = sLimit + "," + Convert.ToString(reader["Parameter_MIN"]);

                        if (reader["QcLine_MAX"] != DBNull.Value || reader["QcLine_MIN"] != DBNull.Value)
                        {
                            sLimit = sLimit + "," + Convert.ToString(reader["QcLine_MAX"]);
                            sLimit = sLimit + "," + Convert.ToString(reader["QcLine_MIN"]);
                        }
                        else
                        {
                            sLimit = sLimit + "," + "NULL";
                            sLimit = sLimit + "," + "NULL";
                        }
                        if (reader["AimLine_VAL"] != DBNull.Value || reader["AimRate_VAL"] != DBNull.Value)
                        {
                            sLimit = sLimit + "," + Convert.ToString(reader["AimLine_VAL"]);
                            sLimit = sLimit + "," + Convert.ToString(reader["AimRate_VAL"]);
                        }
                        else
                        {
                            sLimit = sLimit + "," + "NULL";
                            sLimit = sLimit + "," + "NULL";
                        }
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return sLimit;
        }
        public int GetTnQCNRCnfm_CnfmNO(int nInlineCD, int nQcnrNO)
        {
            string BaseSql = "", sqlCmdTxt = "";
            int nConfirmNO = 0;

            //最新の対応履歴番号取得
            //BaseSql = "Select MAX(Confirm_NO) AS Confirm_NO From TnLOGCnfm Where Inline_CD={0} AND Equipment_NO='{1}' AND Measure_DT='{2}' AND Seq_NO={3} AND QcParam_NO={4} AND Del_FG <> '1'";
            BaseSql = "Select MAX(Confirm_NO) AS Confirm_NO From TnQCNRCnfm WITH(NOLOCK) Where Inline_CD={0} AND QCNR_NO={1}";
            sqlCmdTxt = string.Format(BaseSql, nInlineCD, nQcnrNO);
            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["Confirm_NO"] != System.DBNull.Value)
                        {
                            nConfirmNO = Convert.ToInt32(reader["Confirm_NO"]);
                        }
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }

            //nConfirmNO += 1;//対応履歴+1
            return nConfirmNO;
        }

        public int GetTnLOGCnfm_CnfmNO(int nInlineCD, string sEqui, DateTime dtMeasureDT, int nSeqNO, int nQcParamNO)
        {
            string BaseSql = "", sqlCmdTxt = "";
            int nConfirmNO = 0;

            //最新の対応履歴番号取得
            //BaseSql = "Select MAX(Confirm_NO) AS Confirm_NO From TnLOGCnfm Where Inline_CD={0} AND Equipment_NO='{1}' AND Measure_DT='{2}' AND Seq_NO={3} AND QcParam_NO={4} AND Del_FG <> '1'";
            BaseSql = "Select MAX(Confirm_NO) AS Confirm_NO From TnLOGCnfm WITH(NOLOCK) Where Inline_CD={0} AND Equipment_NO='{1}' AND Measure_DT='{2}' AND Seq_NO={3} AND QcParam_NO={4}";
            sqlCmdTxt = string.Format(BaseSql, nInlineCD, sEqui, dtMeasureDT, nSeqNO, nQcParamNO);
            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["Confirm_NO"] != System.DBNull.Value)
                        {
                            nConfirmNO = Convert.ToInt32(reader["Confirm_NO"]);
                        }
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }

            //nConfirmNO += 1;//対応履歴+1
            return nConfirmNO;
        }
        public string AddCommentEquipmentNO(string sEquipmenNO)
        {
            string BaseSql = "", sqlCmdTxt = "";
            string rsEquipmenNO = "";

            BaseSql = "SELECT [MachinSeq_NO]  FROM TmEQUI WITH(NOLOCK) Where [Equipment_NO]='{0}'";
            sqlCmdTxt = string.Format(BaseSql, sEquipmenNO);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        rsEquipmenNO = Convert.ToString(reader["MachinSeq_NO"])+"号機";
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            rsEquipmenNO = rsEquipmenNO + "("+sEquipmenNO+")";

            return rsEquipmenNO;
        }

        public string AddCommentInspectionNM(string sInspectionNM)
        {
            string BaseSql = "", sqlCmdTxt = "";
            string rsInspectionNM="";

            BaseSql = "SELECT [DefItem_JA]  FROM [TmFCNV] WITH(NOLOCK) Where [DefItem_CD]='{0}'";
            sqlCmdTxt = string.Format(BaseSql, sInspectionNM);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        rsInspectionNM = Convert.ToString(reader["DefItem_JA"]);
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            rsInspectionNM = sInspectionNM + "(" + rsInspectionNM + ")";

            return rsInspectionNM;
        }

        public List<int> GetTnQCNRSameNo(int nInlineCD,string sEquipmentNO,int nInspectionNO,DateTime dtMeasureDT,int nCheck)
        {
            string BaseSql = "", sqlCmdTxt = "";
            List<int> ListQCNRNo = new List<int>();

            BaseSql = "SELECT [QCNR_NO]  FROM [TnQCNR] WITH(NOLOCK) " +
                      "Where Inline_CD={0} AND [Equipment_NO] = '{1}' AND  Inspection_NO={2} AND (Measure_DT>='{3}' AND Measure_DT<'{4}') AND Check_NO={5}";
            sqlCmdTxt = string.Format(BaseSql, nInlineCD, sEquipmentNO, nInspectionNO, dtMeasureDT,dtMeasureDT.AddSeconds(1),nCheck);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        ListQCNRNo.Add(Convert.ToInt32(reader["QCNR_NO"]));
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return ListQCNRNo;
        }

        //対応内容登録
        public void InsertTnQCNRCnfm(int nInlineCD, int nQcnrNO, int nCnfmNO,string sCnfmNM, int nflg, string sOpeNM)
        {
            string BaseSql = "", sqlCmdTxt = "";

            BaseSql = "INSERT INTO TnQCNRCnfm(Inline_CD,QCNR_NO,Confirm_NO,Confirm_NM,Product_FG,Operator_NM,Del_FG,LastUpd_DT) "
                     + "VALUES(               {0},      {1},    {2},       '{3}' ,    {4},       '{5}',      0,     '{6}')";

            sqlCmdTxt = string.Format(BaseSql, nInlineCD, nQcnrNO,nCnfmNO, sCnfmNM, nflg, sOpeNM, DateTime.Now);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    connect.Command.ExecuteNonQuery();
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }

        //対応内容削除
        public void DeleteTnQCNRCnfm(int nInlineCD, int nQcnrNO,int nConfmNO)
        {
            string BaseSql = "", sqlCmdTxt = "";

            //BaseSql = "DELETE FROM TnLOGCnfm WHERE (Inline_CD={0} AND Equipment_NO='{1}' AND Measure_DT='{2}' AND Seq_NO={3} AND QcParam_NO={4})";
            //BaseSql = "UPDATE TnQCNRCnfm SET Del_FG='1' "
                     //+ "WHERE (Inline_CD={0} AND Equipment_NO='{1}' AND Measure_DT='{2}' AND Seq_NO={3} AND QcParam_NO={4})";
            BaseSql = "UPDATE TnQCNRCnfm SET Del_FG='1' "
            + "WHERE (Inline_CD={0} AND QCNR_NO={1} AND Confirm_NO={2} )";

            sqlCmdTxt = string.Format(BaseSql, nInlineCD, nQcnrNO, nConfmNO);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    connect.Command.ExecuteNonQuery();
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }

        public void SetTnQCNRCheckFG(int nflg, int nInlineCD, int nCnfmNO)
        {
            string BaseSql = "", sqlCmdTxt = "";
            BaseSql = "UPDATE TnQCNR SET Check_NO={0} Where Inline_CD={1} AND QCNR_NO={2}";
            sqlCmdTxt = string.Format(BaseSql, nflg, nInlineCD, nCnfmNO);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }
        //対応内容登録
        public void InsertTnLOGCnfm(int nInlineCD, string sEqui, DateTime dtMeasureDT, int nSeqNO, int nQcParamNO,int nConfirmNO,string sCnfmNM,int nflg,string sOpeNM)
        {
            string BaseSql = "", sqlCmdTxt = "";

            BaseSql = "INSERT INTO TnLOGCnfm(Inline_CD,Equipment_NO,Measure_DT,Seq_NO,QcParam_NO,Confirm_NO,Confirm_NM,Product_FG,Operator_NM,Del_FG,LastUpd_DT) "
                     + "VALUES({0},'{1}','{2}', {3} ,{4},{5},'{6}',{7},'{8}','0','{9}')";

            sqlCmdTxt = string.Format(BaseSql, nInlineCD, sEqui, dtMeasureDT, nSeqNO, nQcParamNO, nConfirmNO,sCnfmNM, nflg, sOpeNM, DateTime.Now);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    connect.Command.ExecuteNonQuery();
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }

        //対応内容削除
        public void DeleteTnLOGCnfm(int nInlineCD,string sEqui,DateTime dtMeasureDT,int nSeqNO,int nQcParamNO)
        {
            string BaseSql = "", sqlCmdTxt = "";

            //BaseSql = "DELETE FROM TnLOGCnfm WHERE (Inline_CD={0} AND Equipment_NO='{1}' AND Measure_DT='{2}' AND Seq_NO={3} AND QcParam_NO={4})";
            BaseSql = "UPDATE TnLOGCnfm SET Del_FG='1' "
                     +"WHERE (Inline_CD={0} AND Equipment_NO='{1}' AND Measure_DT='{2}' AND Seq_NO={3} AND QcParam_NO={4})";

            sqlCmdTxt = string.Format(BaseSql, nInlineCD, sEqui, dtMeasureDT, nSeqNO, nQcParamNO);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sqlCmdTxt;
                    connect.Command.ExecuteNonQuery();
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
        }

        public void SetTnLOGCheckFG(int nflg, int nInlineCD, string sEqui, DateTime dtMeasureDT, int nSeqNO, int nQcParamNO, DateTime dtLastUpdDT)
        {
            string BaseSql = "", sqlCmdTxt = "";
            DateTime dtFrom,dtTo;

            //同装置で前後15分に同系統で登録されたNGに対して確認済みとする。
            dtFrom=dtLastUpdDT.AddMinutes(-15);
            dtTo=dtLastUpdDT.AddMinutes(15);

            //製品出来栄え/ｼﾘﾝｼﾞ1～5樹脂量測定値
            if (nQcParamNO>=95 && nQcParamNO<=99){
                /* 2010/06/07迄動いていたが、データ数増加によりタイムアウト→Measure_DTを追加する事で速度UP
                BaseSql = "UPDATE TnLOG SET Check_FG='{0}' "
                        + "Where Inline_CD={1} AND Equipment_NO='{2}' AND Message_NM <>'' AND Del_FG <> '1' AND"
                        + "(QcParam_NO>={3} AND QcParam_NO<={4}) AND (LastUpd_DT>='{5}' AND LastUpd_DT<='{6}')";
                sqlCmdTxt = string.Format(BaseSql, nflg, nInlineCD, sEqui,95, 99, dtFrom, dtTo);
                */
                BaseSql = "UPDATE TnLOG SET Check_FG='{0}' "
                        + "Where Inline_CD={1} AND Equipment_NO='{2}' AND Message_NM <>'' AND Del_FG <> '1' AND "
                        + "(QcParam_NO>={3} AND QcParam_NO<={4}) AND (LastUpd_DT>='{5}' AND LastUpd_DT<='{6}') AND "
                        + "(Measure_DT>='{7}' AND Measure_DT<='{8}')";

                sqlCmdTxt = string.Format(BaseSql, nflg, nInlineCD, sEqui, 95, 99, dtFrom, dtTo, dtMeasureDT.AddHours(-12), dtMeasureDT.AddHours(12));

                using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    SqlDataReader reader = null;
                    try
                    {
                        connect.Command.CommandText = sqlCmdTxt;
                        reader = connect.Command.ExecuteReader();
                    }
                    finally
                    {
                        if (reader != null) reader.Close();
                        connect.Close();
                    }
                }
            }
            //装置実行ﾊﾟﾗﾒｰﾀｰ/ｼﾘﾝｼﾞ1～5ﾃﾞｨｽﾍﾟﾝｽﾀｲﾑ
            else if (nQcParamNO >= 100 && nQcParamNO <= 104)
            {
                /* 2010/06/07迄動いていたが、データ数増加によりタイムアウト→Measure_DTを追加する事で速度UP
                 BaseSql = "UPDATE TnLOG SET Check_FG='{0}' "
                                        + "Where Inline_CD={1} AND Equipment_NO='{2}' AND Message_NM <>'' AND Del_FG <> '1' AND"
                                        + "(QcParam_NO>={3} AND QcParam_NO<={4}) AND (LastUpd_DT>='{5}' AND LastUpd_DT<='{6}')";
                                sqlCmdTxt = string.Format(BaseSql, nflg, nInlineCD, sEqui, 100, 104, dtFrom, dtTo);
                                */
                BaseSql = "UPDATE TnLOG SET Check_FG='{0}' "
                        + "Where Inline_CD={1} AND Equipment_NO='{2}' AND Message_NM <>'' AND Del_FG <> '1' AND "
                        + "(QcParam_NO>={3} AND QcParam_NO<={4}) AND (LastUpd_DT>='{5}' AND LastUpd_DT<='{6}') AND "
                        + "(Measure_DT>='{7}' AND Measure_DT<='{8}')";
                sqlCmdTxt = string.Format(BaseSql, nflg, nInlineCD, sEqui, 100, 104, dtFrom, dtTo, dtMeasureDT.AddHours(-12), dtMeasureDT.AddHours(12));

                using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    SqlDataReader reader = null;
                    try
                    {
                        connect.Command.CommandText = sqlCmdTxt;
                        reader = connect.Command.ExecuteReader();
                    }
                    finally
                    {
                        if (reader != null) reader.Close();
                        connect.Close();
                    }
                }
            }
            //装置実行ﾊﾟﾗﾒｰﾀｰ/ｼﾘﾝｼﾞ1～5ﾊﾞｷｭｰﾑ圧
            else if (nQcParamNO >= 105 && nQcParamNO <= 109)
            {
                /* 2010/06/07迄動いていたが、データ数増加によりタイムアウト→Measure_DTを追加する事で速度UP
                BaseSql = "UPDATE TnLOG SET Check_FG='{0}' "
                        + "Where Inline_CD={1} AND Equipment_NO='{2}' AND Message_NM <>'' AND Del_FG <> '1' AND"
                        + "(QcParam_NO>={3} AND QcParam_NO<={4}) AND (LastUpd_DT>='{5}' AND LastUpd_DT<='{6}')";
                sqlCmdTxt = string.Format(BaseSql, nflg, nInlineCD, sEqui, 105, 109, dtFrom, dtTo);
                */
                BaseSql = "UPDATE TnLOG SET Check_FG='{0}' "
                        + "Where Inline_CD={1} AND Equipment_NO='{2}' AND Message_NM <>'' AND Del_FG <> '1' AND "
                        + "(QcParam_NO>={3} AND QcParam_NO<={4}) AND (LastUpd_DT>='{5}' AND LastUpd_DT<='{6}')  AND "
                        + "(Measure_DT>='{7}' AND Measure_DT<='{8}')";
                sqlCmdTxt = string.Format(BaseSql, nflg, nInlineCD, sEqui, 105, 109, dtFrom, dtTo, dtMeasureDT.AddHours(-12), dtMeasureDT.AddHours(12));

                using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    SqlDataReader reader = null;
                    try
                    {
                        connect.Command.CommandText = sqlCmdTxt;
                        reader = connect.Command.ExecuteReader();
                    }
                    finally
                    {
                        if (reader != null) reader.Close();
                        connect.Close();
                    }
                }
            }
            else//ほとんどの管理項目はこっち
            {
                BaseSql = "UPDATE TnLOG SET Check_FG='{0}' "
                        + "Where Inline_CD={1} AND Equipment_NO='{2}' AND Measure_DT='{3}' AND Seq_NO={4} AND QcParam_NO={5} AND Message_NM <>'' AND Del_FG <> '1'";
                sqlCmdTxt = string.Format(BaseSql, nflg, nInlineCD, sEqui, dtMeasureDT, nSeqNO, nQcParamNO);

                using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
                {
                    SqlDataReader reader = null;
                    try
                    {
                        connect.Command.CommandText = sqlCmdTxt;
                        reader = connect.Command.ExecuteReader();
                    }
                    finally
                    {
                        if (reader != null) reader.Close();
                        connect.Close();
                    }
                }
            }
        }

        public static string GetPRMFirstMaterial() 
        {
            string sql = @" SELECT top 1 Material_CD 
                            FROM TmPLM WITH(nolock) ORDER BY Material_CD ";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            { 
                try
                {
                    connect.Command.CommandText = sql;
                    object materialCD = connect.Command.ExecuteScalar();
                    if (materialCD != null)
                    {
                        return Convert.ToString(materialCD).Trim();
                    }
                    else { return ""; }
                }
                finally
                {
                    connect.Close();
                }
            }
        }

        #endregion

        /// <summary>
        /// DataGridView の RowPostPaintイベント　行書き換え時に行番号を付与する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Grd_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView tmpGrd = (DataGridView)sender;

            // 行ヘッダのセル領域を、行番号を描画する長方形とする
            // （ただし右端に4ドットのすき間を空ける）
            Rectangle rect = new Rectangle(
              e.RowBounds.Location.X,
              e.RowBounds.Location.Y,
              tmpGrd.RowHeadersWidth - 4,
              e.RowBounds.Height);

            // 上記の長方形内に行番号を縦方向中央＆右詰めで描画する
            // フォントや前景色は行ヘッダの既定値を使用する
            TextRenderer.DrawText(
              e.Graphics,
              (e.RowIndex + 1).ToString(),
              tmpGrd.RowHeadersDefaultCellStyle.Font,
              rect,
              tmpGrd.RowHeadersDefaultCellStyle.ForeColor,
              TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        /// <summary>
        /// 規格or管理値か?取得
        /// </summary>
        /// <param name="nQcParamNO"></param>
        /// <returns></returns>
        public PrmAddInfo GetTmPRMInfo(int nQcParamNO)
        {
            PrmAddInfo wPrmAddInfo = new PrmAddInfo();

            string BaseSql = @" SELECT Info1_NM, Info2_NM , Info3_NM
                            FROM TmPRMInfo WITH(nolock) 
                            Where QcParam_NO={0} AND Del_FG=0";
            string sql = string.Format(BaseSql, nQcParamNO);
            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
            {
                SqlDataReader reader = null;
                try
                {
                    connect.Command.CommandText = sql;
                    reader = connect.Command.ExecuteReader();
                    while (reader.Read())
                    {
                        wPrmAddInfo.Info1 = Convert.ToString(reader["Info1_NM"]).Trim();
                        wPrmAddInfo.Info2 = Convert.ToString(reader["Info2_NM"]).Trim();
                        wPrmAddInfo.Info3 = Convert.ToString(reader["Info3_NM"]).Trim();
                    }
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return wPrmAddInfo;
        }

        /// <summary>
        /// 装置名からタイミングNOを取得
        /// </summary>
        /// <param name="assetsNM"></param>
        /// <returns></returns>
        public static string GetTimingNO(string assetsNM) 
        {
            string timingNO = "";
            switch (assetsNM)
            {
                case "ﾀﾞｲﾎﾞﾝﾀﾞｰ": timingNO = "1,2";
                    break;
				case "ﾀﾞｲﾎﾞﾝﾀﾞｰKE205": timingNO = "1,2";
					break;
				case "ﾌﾟﾗｽﾞﾏDCom": timingNO = "3,4,11,48";
					break;
                case "ｳｪﾊﾌﾟﾗｽﾞﾏ":  timingNO = "3,4,11,48";
                    break;
                case "ﾜｲﾔｰﾎﾞﾝﾀﾞｰ": timingNO = "5";
                    break;
                case "外観検査機": timingNO = "6";
                    break;
				case "外観検査機9Cam": timingNO = "6";
					break;
				case "外観検査機RAIM": timingNO = "6";
					break;
				case "外観検査機JType": timingNO = "6";
					break;
                case "外観検査機NType": timingNO = "6";
                    break;
                case "ﾓｰﾙﾄﾞ機": timingNO = "7";
                    break;
                case "ｽｸﾘｭｰMD": timingNO = "7";
                    break;
                case "ﾛｯﾄﾏｰｷﾝｸﾞ装置": timingNO = "10";
					break;
				case "ﾌﾘｯﾌﾟﾁｯﾌﾟ": timingNO = "13,14";
					break;
                case "ﾌﾘｯﾌﾟﾁｯﾌﾟﾎﾞﾝﾀﾞｰ": timingNO = "13,14";
                    break;
                case "ZDﾌﾘｯﾌﾟﾁｯﾌﾟ": timingNO = "13";
					break;
				case "LEDﾌﾘｯﾌﾟﾁｯﾌﾟ": timingNO = "14";
					break;
				case "ﾊﾞﾝﾌﾟﾎﾞﾝﾄﾞ": timingNO = "15";
					break;
                case "ﾊﾞﾝﾌﾟﾎﾞﾝﾀﾞｰ2": timingNO = "15";
                    break;
                case "圧縮成型": timingNO = "16";
					break;
				case "ﾌﾞﾚｲｸ": timingNO = "17";
					break;
				case "裏面洗浄": timingNO = "18";
					break;
				case "ﾚｰｻﾞｰｽｸﾗｲﾌﾞ": timingNO = "19";
					break;
				case "基板重量測定": timingNO = "20";
					break;
				case "色調補正": timingNO = "21";
					break;
				case "電着": timingNO = "22";
					break;
				case "白電着": timingNO = "23";
					break;
				case "外観検査機(ﾀﾞｲｽｸﾗｯｸ)": timingNO = "24";
					break;
				case "反射材ﾎﾟｯﾃｨﾝｸﾞ内製": timingNO = "25";
					break;
				case "反射材ﾎﾟｯﾃｨﾝｸﾞQUSPA": timingNO = "25";
					break;
				case "樹脂枠": timingNO = "26";
					break;
				case "外観検査機(反射材塗布後)": timingNO = "27";
					break;
				case "ｲﾝﾅｰMD": timingNO = "28";
					break;
				case "ｱｳﾀｰMD": timingNO = "29";
					break;
                case "ﾓｰﾙﾄﾞ外製2": timingNO = "28,29,30";
                    break;
                case "YAGガラス実装機": timingNO = "35";
                    break;
                case "自動基板マウンタ": timingNO = "49";
                    break;
            }
            return timingNO;
        }

        public static object GetParameterValue(object targetValue)
        {
            if (targetValue == null || targetValue.ToString() == "")
            {
                return DBNull.Value;
            }
            else
            {
                return targetValue;
            }
        }

        /// <summary>
        /// ファイル内容を取得(行配列)
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>内容配列</returns>
        public static string[] GetMachineFileLineValue(string filePath)
        {
            return GetMachineFileLineValue(filePath, System.DateTime.Now);
        }

        public static string[] GetMachineFileLineValue(string filePath, DateTime startDT)
        {
            try
            {
                if (startDT.AddSeconds(10) <= System.DateTime.Now)
                {
                    //10秒の制限時間を超えた場合、エラー
                    throw new Exception(string.Format(Constant.MessageInfo.Message_50, filePath));
                }

                return File.ReadAllLines(filePath, System.Text.Encoding.Default);
            }
            catch (IOException)
            {
                //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("Filelock:{0}", filePath));
                Thread.Sleep(500);
                return GetMachineFileLineValue(filePath, startDT);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public static string GetMultiSql(List<string> list)
        {
            string sql = "( ";
            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0){ sql += ","; }
                sql += "'" + list[i] + "'";
            }

            sql += ") ";

            return sql;
        }
        public static string GetMultiSql(List<int> list)
        {
            string sql = "( ";
            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0){ sql += ","; }
                sql += list[i];
            }

            sql += ") ";

            return sql;
        }

		public static bool IsNotUseQDIW()
		{
			if (Constant.typeGroup == Constant.TypeGroup.MAP || Common.notUseTmQdiwFG)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        public static int ParseBoolToInt(bool val)
        {
            if (val == true) { return 1; }
            else { return 0; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// IEnumerable&lt;T&gt;→SLCommon.SortableBindingList&lt;T&gt;　の変換を行う拡張メソッド
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static SortableBindingList<T> ToSortableBindingList<T>(this IEnumerable<T> source)
        {
            SortableBindingList<T> retV = new SLCommonLib.Commons.SortableBindingList<T>();
            foreach (T tmp in source)
            {
                retV.Add(tmp);
            }
            return retV;
        }
    }
}
