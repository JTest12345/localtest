using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using System.Collections.Specialized;

namespace GEICS
{
    public class ConnectNASCA
    {
		//public const string CONNECT_USER_ID = "sla04";
		//public const string CONNECT_USER_PASS = "uB9k36K";
		public const string CONNECT_USER_ID = "sla05";
		public const string CONNECT_USER_PASS = "ZP3wnIC";

        /// <summary>
        /// 接続状態を確認
        /// </summary>
        /// <returns></returns>
        public static bool CheckConnect() 
        {
            bool status = true;
            IConnection connect = null;

            try
            {
                connect = NascaConnection.CreateInstance(Constant.StrNASCA, false);
                if (connect.Connection.State == ConnectionState.Closed) 
                {
                    status = false;
                }
            }
            finally
            {
                connect.Dispose();
            }

            return status;
        }

        ///// <summary>
        ///// 資材情報取得
        ///// </summary>
        ///// <param name="sWhereSql"></param>
        ///// <returns></returns>
        //public static List<StructureDrawList> GetTblMaterialStb(string sWhereSql)
        //{
        //    int nCnt = 0;
        //    List<StructureDrawList> drawListList = new List<StructureDrawList>();
        //    SqlDataReader reader = null;

        //    string BaseSql = ""
        //    + " SELECT DISTINCT \r\n"
        //    + "     NttSSHJ.Lot_NO, \r\n"
        //    + "     NtmHMGP.MateGroup_CD, \r\n"
        //    + "     NtmHMGP.MateGroup_JA, \r\n"
        //    + "     RttTRANMAT.mtralitem_cd , \r\n"
        //    + "     RtmMAT.material_ja, \r\n"
        //    + "     RttTRANMAT.lot_no As MateLot_NO, \r\n"
        //    + "     NttSJSB.Plant_CD, \r\n"
        //    + "     NtmHMGK.Material_CD, \r\n"
        //    + "     RttORDH.material_cd As material_cd2 , \r\n"
        //    + "     RTTTRANH.complt_dt \r\n"
        //    + " FROM \r\n"
        //    + "     dbo.NtmHMGP NtmHMGP WITH(NOLOCK) INNER JOIN   \r\n"
        //    + "     dbo.NtmHMGK NtmHMGK WITH(NOLOCK) ON NtmHMGP.MateGroup_CD = NtmHMGK.MateGroup_CD INNER JOIN   \r\n"
        //    + "     dbo.NttSSHJ NttSSHJ WITH(NOLOCK) INNER JOIN   \r\n"
        //    + "     dbo.RvtORDH RttORDH WITH(NOLOCK) ON NttSSHJ.MnfctInst_NO = RttORDH.mnfctinst_no INNER JOIN   \r\n"
        //    + "     dbo.RvtTRANH RttTRANH WITH(NOLOCK) ON NttSSHJ.MnfctInst_NO = RttTRANH.mnfctinst_no INNER JOIN   \r\n"
        //    + "     dbo.RvtTRANMAT RttTRANMAT WITH(NOLOCK) ON RttTRANH.mnfctrsl_no = RttTRANMAT.mnfctrsl_no INNER JOIN   \r\n"
        //    + "     dbo.RvmMAT RtmMAT WITH(NOLOCK) ON RttTRANMAT.mtralitem_cd = RtmMAT.material_cd INNER JOIN   \r\n"
        //    + "     dbo.NttSJSB NttSJSB WITH(NOLOCK) ON RttTRANH.mnfctrsl_no = NttSJSB.MnfctRsl_NO  \r\n"
        //    + "     ON NtmHMGK.Material_CD = RttTRANMAT.mtralitem_cd \r\n"
        //    + " WHERE \r\n"
        //    + "     RttORDH.del_fg = '0' AND \r\n"
        //    + "     NttSSHJ.Del_FG = 0 AND \r\n"
        //    + "     RttTRANH.del_fg = '0' AND \r\n"
        //    + "     RttTRANMAT.del_fg = '0' AND \r\n"
        //    + "     (" + sWhereSql + ") \r\n"
        //    + " OPTION(MAXDOP 1) \r\n";

        //    using (IConnection connect = NascaConnection.CreateInstance(Constant.StrNASCA, false))
        //    {
        //        try
        //        {
        //            connect.Command.CommandText = BaseSql;
        //            reader = connect.Command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                nCnt = nCnt + 1;

        //                StructureDrawList aDrawList= new StructureDrawList()
        //                {
        //                    P_LotNO = Convert.ToString(reader["Lot_NO"]).Trim(),
        //                    C_MateGroupCD = Convert.ToString(reader["MateGroup_CD"]).Trim(),
        //                    C_MateGroupNM = Convert.ToString(reader["MateGroup_JA"]).Trim(),
        //                    C_MaterialCD = Convert.ToString(reader["mtralitem_cd"]).Trim(),
        //                    C_MaterialNM = Convert.ToString(reader["material_ja"]).Trim(),
        //                    C_LotNO = Convert.ToString(reader["MateLot_NO"]).Trim(),
        //                    P_MaterialCD = Convert.ToString(reader["material_cd"]).Trim(),
        //                    P_PlantCD = Convert.ToString(reader["Plant_CD"]).Trim(),
        //                    complt_dt = ((reader["complt_dt"] == DBNull.Value) ? "" : Convert.ToDateTime(reader["complt_dt"]).ToString())
        //                };
        //                //aDrawList.MateChangeDT = this.GetChangeTiming(aDrawList);

        //                drawListList.Add(aDrawList);

        //                //<--Start 2010.03.09 応答なし回避
        //                // メッセージ・キューにあるWindowsメッセージをすべて処理する
        //                Application.DoEvents();
        //                //-->End 2010.03.09 応答なし回避
        //            }
        //            return drawListList;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.ToString());
        //            //MessageBox.Show("失敗しました。もう一度取得して下さい。\r\n\r\n [詳細]=" + ex.ToString());
        //            MessageBox.Show(Constant.MessageInfo.Message_45 + ex.ToString());
        //        }
        //        finally
        //        {
        //            if (reader != null) reader.Close();
        //            connect.Close();
        //        }
        //    }
        //    return drawListList;

        //}

        /// <summary>
        /// NASCA情報(ダイボンド)取得
        /// </summary>
        /// <param name="sWhereEqui"></param>
        /// <returns></returns>
        public static SortedList<int, QCLogData> GetQCItemNASCADB(string sWhereEqui, string sInspectionNM, List<string> lotList, string typeCD, int processNO)
        {
            SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();

            string BaseSql = ""
            + " SELECT  \r\n"
            + "      RvtORDH.material_cd,  \r\n"
            + "      NttSSHJ.Lot_NO,  \r\n"
            + "      NttSJSB.Plant_CD,  \r\n"
            + "      NtmFRKN.DefCause_JA,  \r\n"
            + "      NtmFRBR.DefClass_JA,  \r\n"
            + "      NtmFRKM.DefItem_JA,  \r\n"
            + "      ISNULL(NttSJFR.Defect_CT,0) Defect_CT ,  \r\n"
            + "      RvtTRANH.complt_dt  \r\n"
            + "  FROM  \r\n"
            + "      #LOT INNER JOIN   \r\n"
            + "      dbo.NttSSHJ NttSSHJ WITH(NOLOCK) ON NttSSHJ.Lot_NO = #LOT.Lot_NO INNER JOIN   \r\n"
            + "      dbo.RvtORDH RvtORDH WITH(NOLOCK) ON NttSSHJ.MnfctInst_NO = RvtORDH.mnfctinst_no INNER JOIN   \r\n"
            + "      dbo.RvtTRANH RvtTRANH WITH(NOLOCK) ON  NttSSHJ.MnfctInst_NO = RvtTRANH.mnfctinst_no INNER JOIN   \r\n"
            + "      dbo.NttSJSB NttSJSB WITH(NOLOCK) ON RvtTRANH.mnfctrsl_no = NttSJSB.MnfctRsl_NO LEFT OUTER JOIN  \r\n"
            + "   \r\n"
            + "      (dbo.NttSJFR NttSJFR WITH(NOLOCK) INNER JOIN   \r\n"
            + "       dbo.NtmFRBR NtmFRBR WITH(NOLOCK) ON NttSJFR.DefClass_CD = NtmFRBR.DefClass_CD INNER JOIN   \r\n"
            + "       dbo.NtmFRKM NtmFRKM WITH(NOLOCK) ON  \r\n"
            + "          NttSJFR.DefItem_CD = NtmFRKM.DefItem_CD AND  \r\n"
            + "          NttSJFR.DefItem_CD = @INSPECTIONNM AND NttSJFR.DefClass_CD = @INSPECTIONNM2 INNER JOIN  \r\n"
            + "       dbo.NtmFRKN NtmFRKN WITH(NOLOCK) ON NttSJFR.DefCause_CD = NtmFRKN.DefCause_CD) ON RvtTRANH.mnfctrsl_no = NttSJFR.MnfctRsl_NO \r\n"
            + "  WHERE  \r\n"
            + "      RvtORDH.material_cd = @TYPE AND  \r\n"
            + "      {0}    \r\n"
            + "  OPTION(MAXDOP 1) \r\n";

            string sqlCmdTxt = string.Format(BaseSql, sWhereEqui);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrNASCA, false))
            {
#if TEST
#else
                connect.Command.CommandText = "CREATE TABLE #LOT(Lot_NO varchar(64))";
                connect.Command.ExecuteNonQuery();

                foreach (string tmpLot in lotList)
                {
                    connect.Command.CommandText = "INSERT INTO #LOT VALUES('" + tmpLot + "');";
                    connect.Command.ExecuteNonQuery();
                }
#endif
                SqlDataReader reader = null;
                try
                {
                    if (Common.nLineCD > 9000 && Common.nLineCD < 10000)
                    {
                        connect.Command.Parameters.Add("@TYPE", SqlDbType.Char).Value = typeCD.Trim() + ".DBC";
                    }
                    else
                    {
                        connect.Command.Parameters.Add("@TYPE", SqlDbType.Char).Value = typeCD.Trim() + ".DBA";
                    }
                    string sInspectionNM2 = "";
                    if (sInspectionNM.Substring(0, 1) == "F")            //F0109_B001(チップ欠け)
                    {
                        //B001不良分類CD
                        sInspectionNM2 = sInspectionNM.Substring(sInspectionNM.IndexOf("_") + 1, sInspectionNM.IndexOf("(")-(sInspectionNM.IndexOf("_")+1) );
                        //F0109不良項目CD
                        sInspectionNM = sInspectionNM.Substring(0, sInspectionNM.IndexOf("_"));
                    }
                    connect.Command.Parameters.Add("@INSPECTIONNM", SqlDbType.Char).Value = sInspectionNM;
                    connect.Command.Parameters.Add("@INSPECTIONNM2", SqlDbType.Char).Value = sInspectionNM2;

                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    List<QCLogData> tmpQCLogDataList = new List<QCLogData>();
                    while (reader.Read())
                    {
                        QCLogData wQCLogData = new QCLogData();                             //こっちが正解
                        wQCLogData.EquiNO = Convert.ToString(reader["Plant_CD"]).Trim();    //設備番号
                        wQCLogData.LotNO = Convert.ToString(reader["Lot_NO"]).Trim();       //Lot
                        wQCLogData.TypeCD = typeCD.Trim();                           //Type
                        try
                        {
                            wQCLogData.MeasureDT = Convert.ToDateTime(reader["complt_dt"]);     //計測日時
                        }
                        catch
                        {
                            wQCLogData.MeasureDT = Convert.ToDateTime("9999/01/01");
                        }
                        wQCLogData.Data = Convert.ToDouble(reader["Defect_CT"]);     //"DParameter_VAL"]);       //data
                        wQCLogData.QcprmNO = processNO; 
                        tmpQCLogDataList.Add(wQCLogData);
                    }
                    tmpQCLogDataList.Sort((x, y) => { return (int)((TimeSpan)(x.MeasureDT - y.MeasureDT)).TotalSeconds; });
                    for (int i = 0; i < tmpQCLogDataList.Count; i++)
                    {
                        cndDataItem.Add(i, tmpQCLogDataList[i].Clone());
                    }

                    if (reader != null) reader.Close();
#if TEST
#else
                    connect.Command.CommandText = "DROP TABLE #LOT";
                    connect.Command.ExecuteNonQuery();
#endif
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }

            return cndDataItem;
        }

        /// <summary>
        /// NASCA情報(ワイヤーボンド)取得
        /// </summary>
        /// <param name="sWhereEqui"></param>
        /// <returns></returns>
        public static SortedList<int, QCLogData> GetQCItemNASCAWB(string sWhereEqui, string sInspectionNM, List<string> lotList, string typeCD, int processCD, int defectNO)
        {
            SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();

            string BaseSql = ""
            + " SELECT  \r\n"
            + "      RvtORDH.material_cd,  \r\n"
            + "      NttSSHJ.Lot_NO,  \r\n"
            + "      NttSJSB.Plant_CD,  \r\n"
            + "      NtmFRKN.DefCause_JA,  \r\n"
            + "      NtmFRBR.DefClass_JA,  \r\n"
            + "      NtmFRKM.DefItem_JA,  \r\n"
            + "      ISNULL(NttSJFR.Defect_CT,0) Defect_CT ,  \r\n"
            + "      RvtTRANH.complt_dt  \r\n"
            + "  FROM  \r\n"
            + "      #LOT INNER JOIN   \r\n"
            + "      dbo.NttSSHJ NttSSHJ WITH(NOLOCK) ON NttSSHJ.Lot_NO = #LOT.Lot_NO INNER JOIN   \r\n"
            + "      dbo.RvtORDH RvtORDH WITH(NOLOCK) ON NttSSHJ.MnfctInst_NO = RvtORDH.mnfctinst_no INNER JOIN   \r\n"
            + "      dbo.RvtTRANH RvtTRANH WITH(NOLOCK) ON  NttSSHJ.MnfctInst_NO = RvtTRANH.mnfctinst_no INNER JOIN   \r\n"
            + "      dbo.NttSJSB NttSJSB WITH(NOLOCK) ON RvtTRANH.mnfctrsl_no = NttSJSB.MnfctRsl_NO LEFT OUTER JOIN  \r\n"
            + "   \r\n"
            + "      (dbo.NttSJFR NttSJFR WITH(NOLOCK) INNER JOIN   \r\n"
            + "       dbo.NtmFRBR NtmFRBR WITH(NOLOCK) ON NttSJFR.DefClass_CD = NtmFRBR.DefClass_CD INNER JOIN   \r\n"
            + "       dbo.NtmFRKM NtmFRKM WITH(NOLOCK) ON  \r\n"
            + "          NttSJFR.DefItem_CD = NtmFRKM.DefItem_CD AND  \r\n"
            + "          NttSJFR.DefItem_CD = @INSPECTIONNM AND NttSJFR.DefClass_CD = @INSPECTIONNM2 INNER JOIN  \r\n"
            + "       dbo.NtmFRKN NtmFRKN WITH(NOLOCK) ON NttSJFR.DefCause_CD = NtmFRKN.DefCause_CD) ON RvtTRANH.mnfctrsl_no = NttSJFR.MnfctRsl_NO \r\n"
            + "  WHERE  \r\n"
            + "      RvtORDH.material_cd = @TYPE AND  \r\n"
            + "      {0}    \r\n"
            + "  OPTION(MAXDOP 1) \r\n";

            string sqlCmdTxt = string.Format(BaseSql, sWhereEqui);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrNASCA, false))
            {
#if TEST
#else
                connect.Command.CommandText = "CREATE TABLE #LOT(Lot_NO varchar(64))";
                connect.Command.ExecuteNonQuery();

                foreach (string tmpLot in lotList)
                {
                    connect.Command.CommandText = "INSERT INTO #LOT VALUES('" + tmpLot + "');";
                    connect.Command.ExecuteNonQuery();
                }
#endif
                SqlDataReader reader = null;
                try
                {
                    if (Common.nLineCD > 9000 && Common.nLineCD < 10000)
                    {
                        connect.Command.Parameters.Add("@TYPE", SqlDbType.Char).Value = typeCD.Trim() + ".WBC";
                    }
                    else
                    {
                        connect.Command.Parameters.Add("@TYPE", SqlDbType.Char).Value = typeCD.Trim() + ".WBA";
                    }

                    string sInspectionNM2 = "";
                    if (sInspectionNM.Substring(0, 1) == "F")            //F0109_B001(チップ欠け)
                    {
                        //B001不良分類CD
                        sInspectionNM2 = sInspectionNM.Substring(sInspectionNM.IndexOf("_") + 1, sInspectionNM.IndexOf("(") - (sInspectionNM.IndexOf("_") + 1));
                        //F0109不良項目CD
                        sInspectionNM = sInspectionNM.Substring(0, sInspectionNM.IndexOf("_"));
                    }
                    connect.Command.Parameters.Add("@INSPECTIONNM", SqlDbType.Char).Value = sInspectionNM;
                    connect.Command.Parameters.Add("@INSPECTIONNM2", SqlDbType.Char).Value = sInspectionNM2;
                    
                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    List<QCLogData> tmpQCLogDataList = new List<QCLogData>();
                    while (reader.Read())
                    {
                        QCLogData wQCLogData = new QCLogData();                             //こっちが正解
                        wQCLogData.EquiNO = Convert.ToString(reader["Plant_CD"]).Trim();    //設備番号
                        wQCLogData.LotNO = Convert.ToString(reader["Lot_NO"]).Trim();       //Lot
                        wQCLogData.TypeCD = typeCD.Trim();                           //Type
                        //wQCLogData.MeasureDT = Convert.ToDateTime(reader["complt_dt"]);     //計測日時
                        try
                        {
                            wQCLogData.MeasureDT = Convert.ToDateTime(reader["complt_dt"]);     //計測日時
                        }
                        catch
                        {
                            wQCLogData.MeasureDT = Convert.ToDateTime("9999/01/01");
                        }
                        wQCLogData.Data = Convert.ToDouble(reader["Defect_CT"]);     //"DParameter_VAL"]);       //data
                        wQCLogData.Defect = defectNO;              //監視項目No
                        wQCLogData.QcprmNO = processCD;
                        tmpQCLogDataList.Add(wQCLogData);
                    }
                    tmpQCLogDataList.Sort((x, y) => { return (int)((TimeSpan)(x.MeasureDT - y.MeasureDT)).TotalSeconds; });
                    for (int i = 0; i < tmpQCLogDataList.Count; i++)
                    {
                        cndDataItem.Add(i, tmpQCLogDataList[i].Clone());
                    }

                    if (reader != null) reader.Close();
#if TEST
#else
                    connect.Command.CommandText = "DROP TABLE #LOT";
                    connect.Command.ExecuteNonQuery();
#endif
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return cndDataItem;
        }

        /// <summary>
        /// NASCA情報(モールド)取得
        /// </summary>
        /// <param name="sWhereEqui"></param>
        /// <returns></returns>
        public static SortedList<int, QCLogData> GetQCItemNASCAMD(string sWhereEqui, string sInspectionNM, List<string> lotList, string typeCD, int processNO, int defectNO)
        {
            SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();

            string BaseSql = ""
            + " SELECT  \r\n"
            + "      RvtORDH.material_cd,  \r\n"
            + "      NttSSHJ.Lot_NO,  \r\n"
            + "      NttSJSB.Plant_CD,  \r\n"
            + "      NtmFRKN.DefCause_JA,  \r\n"
            + "      NtmFRBR.DefClass_JA,  \r\n"
            + "      NtmFRKM.DefItem_JA,  \r\n"
            + "      ISNULL(NttSJFR.Defect_CT,0) Defect_CT ,  \r\n"
            + "      RvtTRANH.complt_dt  \r\n"
            + "  FROM  \r\n"
            + "      #LOT INNER JOIN   \r\n"
            + "      dbo.NttSSHJ NttSSHJ WITH(NOLOCK) ON NttSSHJ.Lot_NO = #LOT.Lot_NO INNER JOIN   \r\n"
            + "      dbo.RvtORDH RvtORDH WITH(NOLOCK) ON NttSSHJ.MnfctInst_NO = RvtORDH.mnfctinst_no INNER JOIN   \r\n"
            + "      dbo.RvtTRANH RvtTRANH WITH(NOLOCK) ON  NttSSHJ.MnfctInst_NO = RvtTRANH.mnfctinst_no INNER JOIN   \r\n"
            + "      dbo.NttSJSB NttSJSB WITH(NOLOCK) ON RvtTRANH.mnfctrsl_no = NttSJSB.MnfctRsl_NO LEFT OUTER JOIN  \r\n"
            + "   \r\n"
            + "      (dbo.NttSJFR NttSJFR WITH(NOLOCK) INNER JOIN   \r\n"
            + "       dbo.NtmFRBR NtmFRBR WITH(NOLOCK) ON NttSJFR.DefClass_CD = NtmFRBR.DefClass_CD INNER JOIN   \r\n"
            + "       dbo.NtmFRKM NtmFRKM WITH(NOLOCK) ON  \r\n"
            + "          NttSJFR.DefItem_CD = NtmFRKM.DefItem_CD AND  \r\n"
            + "          NttSJFR.DefItem_CD = @INSPECTIONNM AND NttSJFR.DefClass_CD = @INSPECTIONNM2 INNER JOIN  \r\n"
            + "       dbo.NtmFRKN NtmFRKN WITH(NOLOCK) ON NttSJFR.DefCause_CD = NtmFRKN.DefCause_CD) ON RvtTRANH.mnfctrsl_no = NttSJFR.MnfctRsl_NO \r\n"
            + "  WHERE  \r\n"
            + "      RvtORDH.material_cd = @TYPE AND  \r\n"
            + "      {0}    \r\n"
            + "  OPTION(MAXDOP 1) \r\n";

            string sqlCmdTxt = string.Format(BaseSql, sWhereEqui);

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrNASCA, false))
            {
#if TEST
#else
                connect.Command.CommandText = "CREATE TABLE #LOT(Lot_NO varchar(64))";
                connect.Command.ExecuteNonQuery();

                foreach (string tmpLot in lotList)
                {
                    connect.Command.CommandText = "INSERT INTO #LOT VALUES('" + tmpLot + "');";
                    connect.Command.ExecuteNonQuery();
                }
#endif
                SqlDataReader reader = null;
                try
                {
                    if (Common.nLineCD > 9000 && Common.nLineCD < 10000)
                    {
                        connect.Command.Parameters.Add("@TYPE", SqlDbType.Char).Value = typeCD.Trim() + ".MDC";
                    }
                    else
                    {
                        connect.Command.Parameters.Add("@TYPE", SqlDbType.Char).Value = typeCD.Trim() + ".MDA";
                    }

                    string sInspectionNM2 = "";
                    if (sInspectionNM.Substring(0, 1) == "F")            //F0109_B001(チップ欠け)
                    {
                        //B001不良分類CD
                        sInspectionNM2 = sInspectionNM.Substring(sInspectionNM.IndexOf("_") + 1, sInspectionNM.IndexOf("(") - (sInspectionNM.IndexOf("_") + 1));
                        //F0109不良項目CD
                        sInspectionNM = sInspectionNM.Substring(0, sInspectionNM.IndexOf("_"));
                    }
                    connect.Command.Parameters.Add("@INSPECTIONNM", SqlDbType.Char).Value = sInspectionNM;
                    connect.Command.Parameters.Add("@INSPECTIONNM2", SqlDbType.Char).Value = sInspectionNM2;

                    connect.Command.CommandText = sqlCmdTxt;
                    reader = connect.Command.ExecuteReader();

                    List<QCLogData> tmpQCLogDataList = new List<QCLogData>();
                    while (reader.Read())
                    {
                        QCLogData wQCLogData = new QCLogData();                             //こっちが正解
                        wQCLogData.EquiNO = Convert.ToString(reader["Plant_CD"]).Trim();    //設備番号
                        wQCLogData.LotNO = Convert.ToString(reader["Lot_NO"]).Trim();       //Lot
                        wQCLogData.TypeCD = typeCD.Trim();                           //Type
                        //wQCLogData.MeasureDT = Convert.ToDateTime(reader["complt_dt"]);     //計測日時
                        try
                        {
                            wQCLogData.MeasureDT = Convert.ToDateTime(reader["complt_dt"]);     //計測日時
                        }
                        catch
                        {
                            wQCLogData.MeasureDT = Convert.ToDateTime("9999/01/01");
                        }
                        wQCLogData.Data = Convert.ToDouble(reader["Defect_CT"]);     //"DParameter_VAL"]);       //data
                        wQCLogData.Defect = defectNO;              //監視項目No
                        wQCLogData.QcprmNO = processNO;
                        tmpQCLogDataList.Add(wQCLogData);
                    }
                    tmpQCLogDataList.Sort((x, y) => { return (int)((TimeSpan)(x.MeasureDT - y.MeasureDT)).TotalSeconds; });
                    for (int i = 0; i < tmpQCLogDataList.Count; i++)
                    {
                        cndDataItem.Add(i, tmpQCLogDataList[i].Clone());
                    }

                    if (reader != null) reader.Close();
#if TEST
#else
                    connect.Command.CommandText = "DROP TABLE #LOT";
                    connect.Command.ExecuteNonQuery();
#endif
                }
                finally
                {
                    if (reader != null) reader.Close();
                    connect.Close();
                }
            }
            return cndDataItem;
        }

        private static string GETWhereSQLProcess(List<int> wProcess)
        {
            string sWhereSql = "@";

            foreach (int nProcess in wProcess)
            {
                sWhereSql = sWhereSql + " OR QcParam_NO = " + nProcess;
            }

            sWhereSql = sWhereSql.Replace("@ OR", "AND (");
            sWhereSql = sWhereSql + ")";

            if (sWhereSql == "@)")
            {
                sWhereSql = "";
            }

            return sWhereSql;
        }

        /// <summary>
        /// 樹脂情報を取得
        /// </summary>
        /// <param name="resultID"></param>
        /// <returns></returns>
        public static List<MaterialStbInfo> GetResinData(string resultID)
        {
            SqlDataReader rd = null;
            List<MaterialStbInfo> resultList = new List<MaterialStbInfo>();

            string sql = @" SELECT JttJTMS.Result_ID, JttJTMS.Material_CD, RvmMAT.material_ja, NtmHMGP.MateGroup_CD, NtmHMGP.MateGroup_JA,
                            JttJTMS.Lot_NO
                            FROM dbo.JttJTMS AS JttJTMS 
                            INNER JOIN dbo.JttJSTG AS JttJSTG ON JttJTMS.Result_ID = JttJSTG.Result_ID 
                            INNER JOIN dbo.RvmMAT AS RvmMAT ON JttJTMS.Material_CD = RvmMAT.material_cd 
                            INNER JOIN dbo.NtmHMGK AS NtmHMGK ON JttJTMS.Material_CD = NtmHMGK.Material_CD 
                            INNER JOIN dbo.NtmHMGP AS NtmHMGP ON NtmHMGK.MateGroup_CD = NtmHMGP.MateGroup_CD 
                            WHERE (JttJSTG.del_fg = 0) AND (JttJTMS.del_fg = 0) 
                            AND (JttJTMS.result_id = @ResultID) ";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrNASCA, false))
            {
                SqlParameter param = new SqlParameter("@ResultID", SqlDbType.Int);
                param.Value = resultID;
                connect.Command.Parameters.Add(param);

                connect.Command.CommandText = sql;

                try
                {
                    rd = connect.Command.ExecuteReader();
                    while (rd.Read())
                    {
                        MaterialStbInfo resultInfo = new MaterialStbInfo();

                        resultInfo.MixResultID = resultID;
                        resultInfo.MateGroupCD = Convert.ToString(rd["MateGroup_CD"]).Trim();
                        resultInfo.MateGroupNM = Convert.ToString(rd["MateGroup_JA"]).Trim();
                        resultInfo.MaterialCD = Convert.ToString(rd["Material_CD"]).Trim();
                        resultInfo.MaterialNM = Convert.ToString(rd["material_ja"]).Trim();
                        resultInfo.LotNO = Convert.ToString(rd["Lot_NO"]).Trim();

                        resultList.Add(resultInfo);
                    }
                }
                finally
                {
                    if (!rd.IsClosed) { rd.Close(); }
                }
            }

            return resultList;

        }

        /// <summary>
        /// 品目グループ情報を取得
        /// </summary>
        /// <param name="materialCD">品目CD</param>
        /// <returns>品目グループ情報</returns>
        public static MateGroupInfo GetMateGroup(string materialCD)
        {
            SqlDataReader rd = null;
            MateGroupInfo mateGroupInfo = new MateGroupInfo();

            string sql = @" SELECT NtmHMGP.MateGroup_CD, NtmHMGP.MateGroup_JA
                        FROM dbo.NtmHMGP AS NtmHMGP WITH(nolock) 
                        INNER JOIN dbo.NtmHMGK AS NtmHMGK WITH(nolock) ON NtmHMGP.MateGroup_CD = NtmHMGK.MateGroup_CD
                        WHERE (NtmHMGP.Del_FG = 0) AND (NtmHMGK.Del_FG = 0) AND (NtmHMGK.material_cd = @MaterialCD) ";

#if MEASURE_TIME
			DateTime baseTime = DateTime.Now;
#endif

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrNASCA, false))
            {
                SqlParameter param = new SqlParameter("@MaterialCD", SqlDbType.Char);
                param.Value = materialCD;
                connect.Command.Parameters.Add(param);

                connect.Command.CommandText = sql;
                
                try
                {
                    rd = connect.Command.ExecuteReader();
                    while (rd.Read())
                    {
                        mateGroupInfo.MateGroupCD = Convert.ToString(rd["MateGroup_CD"]).Trim();
                        mateGroupInfo.MateGroupNM = Convert.ToString(rd["MateGroup_JA"]).Trim();
                    }
                }
                finally
                {
                    if (!rd.IsClosed) { rd.Close(); }
                }
            }

#if MEASURE_TIME
			Console.WriteLine("GetMateGroup() connect(ms) / " + (DateTime.Now - baseTime).TotalMilliseconds);
#endif

            return mateGroupInfo;
        }

        public static NameValueCollection GetEmplpyee(string empCD, string empNM) 
        {
            SqlDataReader rd = null;
            NameValueCollection nvc = new NameValueCollection();

            string sql = @" SELECT empcode, empname_ja
                            FROM ROOTSDB.dbo.RTMEMPLOYEE
                            WHERE (void_p = 0) ";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrNASCA, false))
            {
                if (empCD != "")
                {
                    sql += " AND (empcode = @EmpCD) ";
                    SqlParameter param = new SqlParameter("@EmpCD", SqlDbType.Char);
                    param.Value = empCD;
                    connect.Command.Parameters.Add(param);
                }
                if (empNM != "")
                {
                    sql += " AND (empname_ja Like @EmpNM) ";
                    SqlParameter param = new SqlParameter("@EmpNM", SqlDbType.NVarChar);
                    param.Value = "%" + empNM + "%";
                    connect.Command.Parameters.Add(param);
                }

                connect.Command.CommandText = sql;

                try
                {
                    using (rd = connect.Command.ExecuteReader())
                    {
                        if (!rd.HasRows) 
                        {
                            throw new Exception(Constant.MessageInfo.Message_67);
                        }
                        while (rd.Read())
                        {
                            nvc.Add(rd.GetString(rd.GetOrdinal("empcode")), rd.GetString(rd.GetOrdinal("empname_ja")));
                        }
                    }

                    return nvc;
                }
                catch (Exception err)
                {
                    throw err;
                }
            }
        }

        /// <summary>
        /// 作業情報を取得
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public static List<WorkInfo> GetNascaWork(string workcd, string worknm, bool includedelfg)
        {
            var retv = new List<WorkInfo>();

            SqlDataReader rd = null;

            string sql = @" SELECT Work_CD, Work_JA
                        FROM dbo.RvmWork WITH(nolock) 
                        WHERE 1 = 1 ";

            using (IConnection connect = NascaConnection.CreateInstance(Constant.StrNASCA, false))
            {
                if (!includedelfg)
                {
                    sql += " AND (Del_FG = '0') ";
                }
                if (string.IsNullOrEmpty(workcd) == false)
                {
                    sql += " AND (Work_CD like @WORKCD) ";
                    connect.Command.Parameters.Add("@WORKCD", SqlDbType.Char).Value = "%" + workcd + "%";
                }
                if (string.IsNullOrEmpty(worknm) == false)
                {
                    sql += " AND (Work_JA like @WORKNM) ";
                    connect.Command.Parameters.Add("@WORKNM", SqlDbType.NVarChar).Value = "%" + worknm + "%";
                }

                sql += " ORDER BY Work_CD ";

                connect.Command.CommandText = sql;

                try
                {
                    rd = connect.Command.ExecuteReader();
                    while (rd.Read())
                    {
                        string workCd = Convert.ToString(rd["Work_CD"]).Trim();
                        string workNm = Convert.ToString(rd["Work_JA"]).Trim();                        

                        if (retv.Any(l => l.WorkCD == workcd) == false)
                        {
                            var item = new WorkInfo();
                            item.WorkCD = workCd;
                            item.WorkNM = workNm;
                            retv.Add(item);
                        }
                    }
                }
                finally
                {
                    if (!rd.IsClosed) { rd.Close(); }
                }
            }

            return retv;
        }

    }
    /// <summary>
    /// 品目グループ
    /// </summary>
    public class MateGroupInfo
    {
        /// <summary>品目グループCD</summary>
        public string MateGroupCD { get; set; }

        /// <summary>品目グループ名</summary>
        public string MateGroupNM { get; set; }
    }
    /// <summary>
    /// 作業
    /// </summary>
    public class WorkInfo
    {
        /// <summary>画面上のチェックボックスにチェックを付けたかどうか</summary>
        public bool SelectFG { get; set; }

        /// <summary>作業CD</summary>
        public string WorkCD { get; set; }

        /// <summary>作業名</summary>
        public string WorkNM { get; set; }

        /// <summary> 作業名【作業CD】 </summary>
        public string WorkNMWithCD
        {
            get
            {
                if (string.IsNullOrEmpty(WorkCD) || string.IsNullOrEmpty(WorkNM))
                {
                    return string.Empty;
                }
                else
                {
                    return WorkNM + "【" + WorkCD + "】";                    
                }
            }
        }
    }
}
