namespace EICS
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Collections.Specialized;
    using SLCommonLib.Commons;
    using SLCommonLib.DataBase;
    using System.Data.Common;
    using System.Data;
    using System.Drawing;
    using NascaAPI;
    using EICS.Database;
    using System.Linq;

    /// <summary>
    /// データベース接続機能(共通)
    /// </summary>
    public class ConnectDB : IDisposable
    {
        /// <summary>コネクション</summary>
        private SLCommonLib.DataBase.DBConnect connection;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="beginTrans"></param>
        /// <param name="server"></param>
        public ConnectDB(bool beginTrans, Constant.DBConnectGroup dbNM, int lineCD)
        {
            try
            {
                this.connection = SLCommonLib.DataBase.DBConnect.CreateInstance(getConnString(dbNM, lineCD), "System.Data.SqlClient", beginTrans); ;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// デバッグ時の書き込み用DBの接続文字列を取得
        /// </summary>
        /// <param name="connectionKey">接続キー</param>
        /// <returns>接続文字列</returns>
        public static string getConnStringForDebug(Constant.DBConnectGroup dbNM, int lineCD)
        {
#if Debug
            if (dbNM == Constant.DBConnectGroup.EICSDB)
            {
                return @"Server=sla-0040-2\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
            }
            else if (dbNM == Constant.DBConnectGroup.ARMS)
            {
                return @"Server=SLA-0040-2\SQLEXPRESS;Database=ARMS;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
            }
            else
            {
                return "";
            }
#endif
            return "";
        }

        /// <summary>
        /// 接続文字列を取得
        /// </summary>
        /// <param name="connectionKey">接続キー</param>
        /// <returns>接続文字列</returns>
		public static string getConnString(Constant.DBConnectGroup dbNM, int lineCD)
        {
            try
            {
                SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);
                SettingInfo settingInfo = SettingInfo.GetSingleton();

                if (dbNM == Constant.DBConnectGroup.EICSDB)
                {
                    if (settingInfo.DebugEICSServer != null && settingInfo.DebugEICSDatabase != null)
                        return string.Format("Server={0};Database={1};UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];", settingInfo.DebugEICSServer, settingInfo.DebugEICSDatabase);
                }
                else if (dbNM == Constant.DBConnectGroup.ARMS)
                {

                    if (settingInfo.DebugARMSServer != null && settingInfo.DebugARMSDatabase != null)
                        return string.Format("Server={0};Database={1};UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];", settingInfo.DebugARMSServer, settingInfo.DebugARMSDatabase);
                }

                if (dbNM == Constant.DBConnectGroup.ARMS)
                {
                    if (string.IsNullOrEmpty(settingInfoPerLine.ArmsDBPC) == false)
                    {
                        return getConnString(dbNM, settingInfoPerLine.ArmsDBPC);
                    }
                    else
                    {
                        //return getConnString(dbNM, settingInfoPerLine.PackagePC);
                        return getConnStringUsingPackagePc(dbNM, settingInfoPerLine);
                    }
                }

                //return getConnString(dbNM, settingInfoPerLine.PackagePC);
                return getConnStringUsingPackagePc(dbNM, settingInfoPerLine);
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public static string getConnString(Constant.DBConnectGroup dbNM, int serverLineCD, int? lineCD)
        {
            if (lineCD.HasValue)
            {
                SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(lineCD.Value);

                if (settingInfo != null)
                {
                    //return getConnString(dbNM, settingInfo.PackagePC);
                    return getConnStringUsingPackagePc(dbNM, settingInfo);
                }
                else
                {
                    return getConnString(dbNM, Serv.GetDBServerAddr(serverLineCD, lineCD.Value));
                }
            }
            else
            {
                SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(serverLineCD);

                //return getConnString(dbNM, settingInfo.PackagePC);
                return getConnStringUsingPackagePc(dbNM, settingInfo);
            }
        }

        public static string getConnStringUsingPackagePc(Constant.DBConnectGroup dbNM, SettingInfo settingInfo)
        {
            //string constr = string.Empty;
            if (dbNM == Constant.DBConnectGroup.EICSDB)
            {
                //constr = settingInfo.EicsConnectionString;
                return settingInfo.EicsConnectionString;
            }
            else if (dbNM == Constant.DBConnectGroup.ARMS)
            {
                //constr = settingInfo.ArmsConnectionString;
                return settingInfo.ArmsConnectionString;
            }
            else if (dbNM == Constant.DBConnectGroup.LENS)
            {
                //constr = settingInfo.LensConnectionString;
                return settingInfo.LensConnectionString;
            }

            //constr = getConnString(dbNM, settingInfo.PackagePC);
            //return constr;
            return getConnString(dbNM, settingInfo.PackagePC);
        }

        public static string getConnString(Constant.DBConnectGroup dbNM, string packagePC)
        {

            if (dbNM == Constant.DBConnectGroup.EICSDB)
            {
#if Debug
                //MAP
                //return @"Server=sla-0040-2\SQLEXPRESS;Database=QCIL_SIDEVIEW;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=SL5-5672\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=sl5-3135\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //return @"Server=SL5-5672\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return string.Format(Properties.Settings.Default.ConnectionString_QCILDB, "inline", "R28uHta", settingInfo.PackagePC);
                //return string.Format(Properties.Settings.Default.ConnectionString_QCILDB, "inline", "R28uHta", packagePC);
                //return string.Format(Properties.Settings.Default.ConnectionString_QCILDB, "inline", "R28uHta", packagePC);
                //return @"Server=sla-0040-2\SQLEXPRESS;Database=QCIL_SV11;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=sla-0040-2\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return string.Format(Properties.Settings.Default.ConnectionString_QCILDB, "inline", "R28uHta", packagePC);
                //return @"Server=172.16.20.245\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //SIDEVIEW
                return string.Format(Properties.Settings.Default.ConnectionString_QCILDB, "inline", "R28uHta", packagePC);
                //return @"Server=sla-0040-2\SQLEXPRESS;Database=QCIL01_AUTO_AOI;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=sl5-3204\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //アオイ(高効率)SIDEVIEW
                //return @"Server=sla-0040-2\SQLEXPRESS;Database=QCIL01_HIGH_AOI;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //------------------------------

                //return @"Server=sl5-5302.nichia.local\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=172.16.16.247\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=SL5-5671\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=sl5-5275.takatsuki.local\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //アオイ高効率
                //return @"Server=aoihigh1-1.aoi.local\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //return @"Server=sl5-5275.takatsuki.local\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //NMC
                //return @"Server=nmc-para-db1\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=nmc-para-wb1\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=SL5-5172\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
#elif UNIT_TEST
				return @"Server=sla-0040-2\SQLEXPRESS;Database=QCIL_UNITTEST;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システムUNITTEST];";
#else
				return string.Format(Properties.Settings.Default.ConnectionString_QCILDB, "inline", "R28uHta", packagePC);
#endif
            }
            else if (dbNM == Constant.DBConnectGroup.ARMS)
            {
#if Debug
                return string.Format(Properties.Settings.Default.ConnectionString_ARMSDB, "inline", "R28uHta", packagePC);
                //return @"Server=sla-0040-2\SQLEXPRESS;Database=ARMS_MAP;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //return @"Server=sla-0040-2\SQLEXPRESS;Database=ARMS_SV;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //return @"Server=SL5-3204\SQLEXPRESS;Database=ARMS;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=SLA-0040-2\SQLEXPRESS;Database=ARMS3;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return string.Format(Properties.Settings.Default.ConnectionString_ARMSDB, "inline", "R28uHta", packagePC);
                //return string.Format(Properties.Settings.Default.ConnectionString_ARMSDB, "inline", "R28uHta", packagePC);
                //return @"Server=SLA-0040-2\SQLEXPRESS;Database=ARMS3;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=SLA-0040-2\SQLEXPRESS;Database=ARMS2;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return string.Format(Properties.Settings.Default.ConnectionString_ARMSDB, "inline", "R28uHta", packagePC);
                //return @"Server=172.16.20.245\SQLEXPRESS;Database=ARMS;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //return @"Server=sl5-5275.takatsuki.local\SQLEXPRESS;Database=ARMS;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=sl5-5275.takatsuki.local\SQLEXPRESS;Database=ARMS;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=SL5-5671\SQLEXPRESS;Database=ARMS;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=sl5-3135\SQLEXPRESS;Database=ARMS;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
                //return @"Server=172.16.16.247\SQLEXPRESS;Database=ARMS;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //return string.Format(Properties.Settings.Default.ConnectionString_ARMSDB, "inline", "R28uHta", settingInfo.PackagePC);
                //return @"Server=SL5-5672\SQLEXPRESS;Database=ARMS;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";

                //アオイ高効率
                //return @"Server=aoihigh1-1.aoi.local\SQLEXPRESS;Database=QCIL;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];";
#elif UNIT_TEST
				return @"Server=SLA-0040-2\SQLEXPRESS;Database=ARMS_UNITTEST;UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システムUNITTEST];";

#else
				return string.Format(Properties.Settings.Default.ConnectionString_ARMSDB, "inline", "R28uHta", packagePC);
#endif
            }
            else if (dbNM == Constant.DBConnectGroup.NASCA)
            {
                return string.Format(Properties.Settings.Default.ConnectionString_NASCA, "sla05", "ZP3wnIC");
            }
            else if (dbNM == Constant.DBConnectGroup.LENS)
            {
                return string.Format(Properties.Settings.Default.ConnectionString_LENSDB, "inline", "R28uHta", packagePC);
            }
            else
            {
                return "";
            }
        }

        public static string getConnString(string server, string db)
        {
            return string.Format("Server={0};Database={1};UID=inline;PWD=R28uHta;Application Name=EICS[傾向管理システム];", server, db);
        }

        #region IDisposable メンバ

        public void Dispose()
        {
            if (this.connection != null)
            {
                this.connection.Dispose();
                this.connection = null;
            }
        }

        #endregion

        #region QCIL

        /// <summary>
        /// 装置マスタ[TmEQUI]取得
        /// </summary>
        /// <param name="lineCD">ラインNO</param>
        /// <param name="equipmentNO">設備NO</param>
        /// <returns>装置マスタ</returns>
        private static List<EquipmentInfo> GetEquipmentList(int lineCD, string equipmentNO)
        {
            System.Data.Common.DbDataReader rd = null;
            List<EquipmentInfo> equipmentList = new List<EquipmentInfo>();

            try
            {
                SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(lineCD);

                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT TmEQUI.Equipment_NO, TmEQUI.Assets_NM, TmEQUI.MachinSeq_NO, TmEQUI.Model_NM, TmLSET.IPAddress_NO, TmLSET.Port_NO
                                FROM TmLSET WITH (nolock) 
                                INNER JOIN TmEQUI ON TmLSET.Equipment_NO = TmEQUI.Equipment_NO
                                WHERE (TmLSET.Inline_CD = @LineCD) AND (TmLSET.Del_FG = 0) AND (TmEQUI.Del_FG = 0) ";

                    if (!string.IsNullOrEmpty(equipmentNO))
                    {
                        sql += " AND (TmEQUI.Equipment_NO = @EquipmentNO) ";
                        conn.SetParameter("EquipmentNO", SqlDbType.NVarChar, equipmentNO);
                    }

                    if (settingInfo.PcNO != "9999")
                    {
                        sql += " AND (TmLSET.Seq_NO = @PcNO) ";
                        conn.SetParameter("@PcNO", SqlDbType.Int, settingInfo.PcNO);
                    }

                    sql += " ORDER BY Equipment_CD ";

                    conn.SetParameter("@LineCD", SqlDbType.Int, lineCD);

                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            //if (Convert.ToString(rd["Assets_NM"]).Trim() == "ｶｯﾄﾌｫｰﾐﾝｸﾞ機")
                            //{
                            //    continue;
                            //}

                            EquipmentInfo equipInfo = new EquipmentInfo();

                            equipInfo.EquipmentNO = Convert.ToString(rd["Equipment_NO"]).Trim();
                            equipInfo.AssetsNM = Convert.ToString(rd["Assets_NM"]).Trim();
                            equipInfo.MachineNM = Convert.ToString(rd["MachinSeq_NO"]).Trim();
                            equipInfo.ModelNM = Convert.ToString(rd["Model_NM"]).Trim();
                            equipInfo.IPAddressNO = Convert.ToString(rd["IPAddress_NO"]).Trim();
                            equipInfo.PortNO = Convert.ToString(rd["Port_NO"]).Trim();
                            equipInfo.LineNO = lineCD;

                            equipmentList.Add(equipInfo);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, err.Message);
                throw;
            }

            return equipmentList;
        }

        public static List<EquipmentInfo> GetEquipmentList(int lineCD)
        {
            return GetEquipmentList(lineCD, string.Empty);
        }

        public static EquipmentInfo GetEquipmentData(int lineCD, string equipmentNO)
        {
            List<EquipmentInfo> equips = GetEquipmentList(lineCD, equipmentNO);
            if (equips.Count == 0) { return null; }
            else
            {
                return equips.Single();
            }
        }

        /// <summary>
        /// 装置マスタ[TmEQUI](要素)取得
        /// </summary>
        /// <param name="elementNM">戻り値フィールド名</param>
        /// <param name="equipmentNO">設備NO</param>
        /// <returns>フィールド値</returns>
        //        public static object GetEQUIElement(string elementNM, string equipmentNO)
        //        {
        //            object elementVAL;

        //            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
        //            {
        //                string sql = @" SELECT {0}
        //                                FROM TmEQUI WITH (nolock) 
        //                                WHERE Del_FG = 0 AND Equipment_NO = @EquipmentNO";

        //                conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, equipmentNO);

        //                sql = string.Format(sql, elementNM);
        //                elementVAL = conn.ExecuteScalar(sql);
        //            }

        //            return elementVAL;
        //        }

        /// <summary>
        /// 装置情報マスタ[TmLSET]取得
        /// </summary>
        /// <param name="lineCD">ラインNO</param>
        /// <param name="equipmentNO">設備NO</param>
        /// <returns>装置情報マスタ</returns>
        public static LSETInfo GetLSETInfo(int lineCD, string equipmentNO)
        {
            return GetLSETData(lineCD, equipmentNO)[0];
        }
        public static List<LSETInfo> GetLSETData(int lineCD, string equipmentNO)
        {
            return GetLSETData(lineCD, lineCD, equipmentNO);
        }
        public static List<LSETInfo> GetLSETData(int serverLineCD, int? lineCD, string equipmentNO)
        {
            return GetLSETData(getConnString(Constant.DBConnectGroup.EICSDB, serverLineCD), lineCD, equipmentNO);
        }
        public static List<LSETInfo> GetLSETData(string constr, int? lineCD, string equipmentNO)
        {
            System.Data.Common.DbDataReader rd = null;
            List<LSETInfo> lsetList = new List<LSETInfo>();

            using (DBConnect conn = DBConnect.CreateInstance(constr, "System.Data.SqlClient", false))
            {
                string sql = @" SELECT TmLSET.Inline_CD, TmLSET.Equipment_NO, TmLSET.Equipment_CD, TmLSET.Process_CD, TmLSET.Seq_NO, TmLSET.IPAddress_NO, TmLSET.Port_NO, TmLSET.InputFolder_NM, 
                                TmLSET.LoaderAddress_NO, TmLSET.LoaderPlcNode_NO, TmLSET.ThreadGrp_CD, TmLSET.MainThread_FG, TmLSET.EquipPart_ID, TmLSET.ReferMultiServer_FG,
                                TmLSET.Del_FG, TmLSET.UpdUser_CD, TmLSET.LastUpd_DT, TmEQUI.Assets_NM, TmLSET.Seq_NO, TmEQUI.Model_NM, TmEQUI.MachinSeq_NO, TmLSET.WorkingType_CD, TmLSET.EnableResultPriorityJudge_FG, TmLSET.Chip_NM, TmLSET.MachineFolder_NM
                                FROM TmLSET WITH (nolock) 
                                INNER JOIN TmEQUI ON TmLSET.Equipment_NO = TmEQUI.Equipment_NO 
                                WHERE (TmLSET.Del_FG = 0) AND (TmEQUI.Del_FG = 0) ";

                if (lineCD.HasValue)
                {
                    sql += " AND Inline_CD = @LineCD ";
                    conn.SetParameter("@LineCD", SqlDbType.Int, lineCD);
                }

                if (equipmentNO != "")
                {
                    sql += " AND (TmEQUI.Equipment_NO = @EquipmentNO) ";
                    conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, equipmentNO);
                }

                using (rd = conn.GetReader(sql))
                {
                    int ordThreadGrpCD = rd.GetOrdinal("ThreadGrp_CD");
                    int ordMainThreadFG = rd.GetOrdinal("MainThread_FG");
                    int ordEquipPartID = rd.GetOrdinal("EquipPart_ID");
                    int ordEnableResultPriority = rd.GetOrdinal("EnableResultPriorityJudge_FG");
                    int ordMachineFloderNM = rd.GetOrdinal("MachineFolder_NM");

                    while (rd.Read())
                    {
                        LSETInfo lsetInfo = new LSETInfo();

                        lsetInfo.InlineCD = Convert.ToInt32(rd[rd.GetOrdinal("Inline_CD")]);
                        lsetInfo.EquipmentNO = Convert.ToString(rd[rd.GetOrdinal("Equipment_NO")]).Trim();
                        lsetInfo.EquipmentCD = Convert.ToString(rd[rd.GetOrdinal("Equipment_CD")]).Trim();
                        lsetInfo.ProcessCD = Convert.ToInt32(rd[rd.GetOrdinal("Process_CD")]);
                        lsetInfo.SeqCD = Convert.ToString(rd[rd.GetOrdinal("Seq_NO")]).Trim();
                        lsetInfo.IPAddressNO = Convert.ToString(rd[rd.GetOrdinal("IPAddress_NO")]).Trim();

                        int ordPortNO = rd.GetOrdinal("Port_NO");
                        if (!rd.IsDBNull(ordPortNO))
                        {
                            if (!(Convert.ToString(rd[rd.GetOrdinal("Port_NO")]).Trim() == string.Empty))
                            {
                                lsetInfo.PortNO = Convert.ToInt32(rd.GetString(ordPortNO));
                            }
                        }

                        lsetInfo.InputFolderNM = Convert.ToString(rd[rd.GetOrdinal("InputFolder_NM")]).Trim();
                        lsetInfo.DelFG = Convert.ToBoolean(rd[rd.GetOrdinal("Del_FG")]);
                        lsetInfo.UpdUserCD = Convert.ToString(rd[rd.GetOrdinal("UpdUser_CD")]).Trim();
                        lsetInfo.LastUpdDT = Convert.ToString(rd[rd.GetOrdinal("LastUpd_DT")]).Trim();
                        lsetInfo.AssetsNM = Convert.ToString(rd[rd.GetOrdinal("Assets_NM")]).Trim();
                        lsetInfo.MachineNM = Convert.ToString(rd[rd.GetOrdinal("MachinSeq_NO")]).Trim();
                        lsetInfo.ModelNM = Convert.ToString(rd[rd.GetOrdinal("Model_NM")]).Trim();
                        lsetInfo.MachineSeqNO = Convert.ToString(rd[rd.GetOrdinal("MachinSeq_NO")]).Trim();
                        lsetInfo.ReferMultiServerFG = Convert.ToBoolean(rd[rd.GetOrdinal("ReferMultiServer_FG")]);

                        if (!rd.IsDBNull(rd.GetOrdinal("WorkingType_CD")))
                        {
                            lsetInfo.TypeCD = Convert.ToString(rd[rd.GetOrdinal("WorkingType_CD")]).Trim();
                        }


                        if (!rd.IsDBNull(rd.GetOrdinal("LoaderAddress_NO")))
                        {
                            lsetInfo.LoaderAddressNO = Convert.ToString(rd[rd.GetOrdinal("LoaderAddress_NO")]).Trim();
                        }
                        if (!rd.IsDBNull(rd.GetOrdinal("LoaderPlcNode_NO")))
                        {
                            lsetInfo.LoaderPlcNodeNO = Convert.ToByte(Convert.ToString(rd[rd.GetOrdinal("LoaderPlcNode_NO")]).Trim());
                        }

                        if (!rd.IsDBNull(ordThreadGrpCD))
                        {
                            lsetInfo.ThreadGrpCD = rd.GetString(ordThreadGrpCD).Trim();
                        }

                        if (!rd.IsDBNull(ordMainThreadFG))
                        {
                            lsetInfo.MainThreadFG = rd.GetBoolean(ordMainThreadFG);
                        }
                        else
                        {
                            lsetInfo.MainThreadFG = true;
                        }

                        lsetInfo.EnableResultPriorityJudge_FG = Convert.ToBoolean(rd[ordEnableResultPriority]);

                        lsetInfo.EquipPartID = rd.GetString(ordEquipPartID);

                        if (!rd.IsDBNull(rd.GetOrdinal("Chip_NM")))
                        {
                            lsetInfo.ChipNM = Convert.ToString(rd[rd.GetOrdinal("Chip_NM")]).Trim();
                        }

                        if (!rd.IsDBNull(ordMachineFloderNM))
                        {
                            lsetInfo.MachineFolderNM = Convert.ToString(rd[ordMachineFloderNM]).Trim();
                        }

                        lsetList.Add(lsetInfo);
                    }
                }
            }

            return lsetList;
        }

        public static List<LSETInfo> GetLSETDataFromMultipleServer(int serverLineCD, int? lineCD, string equipmentNO)
        {
            List<LSETInfo> list = GetLSETData(serverLineCD, lineCD, equipmentNO);

            // 自サーバで取得出来たら、その時点で返す。
            if (list.Count > 0)
            {
                return list;
            }

            SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

            // 他の指定サーバからもデータを取得する。ライン間移動ロット対応
            foreach (string serverNm in commonSettingInfo.ArmsServerList)
            {
                // 他ラインを想定するので、lineCDの指定はしない
                list.AddRange(GetLSETData(getConnString(Constant.DBConnectGroup.EICSDB, serverNm), null, equipmentNO));
            }

            return list;
        }

        /// <summary>
        /// 装置情報マスタ[TmLSET]取得(DB,WB紐付け機能で使用)
        /// </summary>
        /// <param name="lineCD">ラインNO</param>
        /// <param name="equipmentNO">設備NO</param>
        /// <returns>装置情報マスタ</returns>
        public static List<LSETInfo> GetLSETData_Chain(int lineCD, string equipmentNO)
        {
            SettingInfo settingInfo = SettingInfo.GetSingleton();

            System.Data.Common.DbDataReader rd = null;
            List<LSETInfo> lsetList = new List<LSETInfo>();

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(settingInfo.HonbanServer, settingInfo.HonbanDB), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT TmLSET.Inline_CD, TmLSET.Equipment_NO, TmLSET.Equipment_CD, TmLSET.Process_CD, TmLSET.Seq_NO, TmLSET.IPAddress_NO, TmLSET.Port_NO, TmLSET.InputFolder_NM, 
                                TmLSET.Del_FG, TmLSET.UpdUser_CD, TmLSET.LastUpd_DT, TmEQUI.Assets_NM, TmLSET.Seq_NO, TmLSET.ReferMultiServer_FG, TmEQUI.Model_NM, TmEQUI.MachinSeq_NO  
                                FROM TmLSET WITH (nolock) 
                                INNER JOIN TmEQUI ON TmLSET.Equipment_NO = TmEQUI.Equipment_NO 
                                WHERE (TmLSET.Del_FG = 0) AND (TmEQUI.Del_FG = 0) AND Inline_CD = @LineCD ";

                conn.SetParameter("@LineCD", SqlDbType.Int, lineCD);

                if (equipmentNO != "")
                {
                    sql += " AND (TmEQUI.Equipment_NO = @EquipmentNO) ";
                    conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, equipmentNO);
                }

                using (rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        LSETInfo lsetInfo = new LSETInfo();

                        lsetInfo.InlineCD = Convert.ToInt32(rd["Inline_CD"]);
                        lsetInfo.EquipmentNO = Convert.ToString(rd["Equipment_NO"]).Trim();
                        lsetInfo.EquipmentCD = Convert.ToString(rd["Equipment_CD"]).Trim();
                        lsetInfo.ProcessCD = Convert.ToInt32(rd["Process_CD"]);
                        lsetInfo.SeqCD = Convert.ToString(rd["Seq_NO"]).Trim();
                        lsetInfo.IPAddressNO = Convert.ToString(rd["IPAddress_NO"]).Trim();
                        //lsetInfo.PortNO = Convert.ToInt32(rd["Port_NO"]);
                        int ordPortNO = rd.GetOrdinal("Port_NO");
                        if (!rd.IsDBNull(ordPortNO))
                        {
                            lsetInfo.PortNO = Convert.ToInt32(rd.GetString(ordPortNO));
                        }

                        lsetInfo.InputFolderNM = Convert.ToString(rd["InputFolder_NM"]).Trim();
                        lsetInfo.DelFG = Convert.ToBoolean(rd["Del_FG"]);
                        lsetInfo.UpdUserCD = Convert.ToString(rd["UpdUser_CD"]).Trim();
                        lsetInfo.LastUpdDT = Convert.ToString(rd["LastUpd_DT"]).Trim();
                        lsetInfo.ReferMultiServerFG = Convert.ToBoolean(rd["ReferMultiServer_FG"]);
                        lsetInfo.AssetsNM = Convert.ToString(rd["Assets_NM"]).Trim();
                        lsetInfo.MachineNM = Convert.ToString(rd["Seq_NO"]).Trim();
                        lsetInfo.ModelNM = Convert.ToString(rd["Model_NM"]).Trim();
                        lsetInfo.MachineSeqNO = Convert.ToString(rd["MachinSeq_NO"]).Trim();
                        lsetList.Add(lsetInfo);
                    }
                }
            }

            return lsetList;
        }

        /// <summary>
        /// 装置情報マスタ[TmLSET](要素)取得
        /// </summary>
        /// <param name="elementNM">戻り値フィールド名</param>
        /// <param name="lineCD">ラインNO</param>
        /// <param name="plantCD">設備NO</param>
        /// <returns>フィールド値</returns>
        public static object GetLSETElement(string elementNM, int lineCD, string plantCD)
        {
            object elementVAL;

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT {0}
                                FROM TmLSET WITH (nolock) 
                                WHERE Del_FG = 0 AND InLine_CD = @LineCD AND Equipment_NO = @EquipmentNO ";

                conn.SetParameter("@LineCD", SqlDbType.Int, lineCD);
                conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, plantCD);

                sql = string.Format(sql, elementNM);
                elementVAL = conn.ExecuteScalar(sql);
            }

            return elementVAL;
        }

        /// <summary>
        /// 管理項目マスタ[TmPRM](要素)取得
        /// </summary>
        /// <param name="elementNM">戻り値フィールド名</param>
        /// <param name="qcParamNO">管理NO</param>
        /// <returns>フィールド値</returns>
        public static object GetPRMElement(string elementNM, int qcParamNO, int lineCD)
        {
            object elementVAL;

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT {0} 
                                FROM TmPRM
                                WHERE (Del_FG = 0) AND (QcParam_NO = @QcParamNO)";

                conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNO);

                sql = string.Format(sql, elementNM);
                elementVAL = conn.ExecuteScalar(sql).ToString().Trim();
            }

            return elementVAL;
        }

        /// 閾値マスタ[TmPLM]取得
        /// </summary>
        /// <param name="qcparamNO">管理NO</param>
        /// <param name="modelNM">装置型式</param>
        /// <param name="materialCD">製品型番</param>
        /// <returns>閾値マスタ</returns>
        //public static PLMInfo GetPLMData(int qcparamNO, string modelNM, string materialCD, int lineCD)
        //{
        //	return GetPLMData(qcparamNO, modelNM, string.Empty, materialCD, lineCD, string.Empty);
        //}

        //public static PLMInfo GetPLMData(int qcparamNO, string modelNM, string materialCD, int lineCD, string identChar)
        //{
        //	return GetPLMData(qcparamNO, modelNM, string.Empty, materialCD, lineCD, identChar);
        //}

        public static Plm GetPLMData(int qcparamNO, string modelNM, string materialCD, int lineCD, string identChar)
        {
            if (qcparamNO == 0 || string.IsNullOrEmpty(modelNM) || string.IsNullOrEmpty(materialCD))
            {
                return null;
            }

            return Plm.GetData(lineCD, materialCD, modelNM, qcparamNO, identChar);
        }

        //public static PLMInfo GetPLMData(int qcparamNO, string modelNM, string equipmentNO, string materialCD, int lineCD)
        //{
        //	return GetPLMData(qcparamNO, modelNM, equipmentNO, materialCD, lineCD, string.Empty);
        //}

        /// <summary>
        /// 閾値マスタ[TmPLM]取得
        /// </summary>
        /// <param name="qcparamNO">管理NO</param>
        /// <param name="modelNM">装置型式</param>
        /// <param name="materialCD">製品型番</param>
        /// <returns>閾値マスタ</returns>
        public static Plm GetPLMData(int qcparamNO, string modelNM, string equipmentNO, string materialCD, int lineCD, string identChar)
        {
            System.Data.Common.DbDataReader rd = null;
            Plm plmInfo = null;
            List<Plm> plmInfoList = new List<Plm>();

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT TmPRM.QcParam_NO, TmPRM.Manage_NM, TmPRM.Parameter_NM, TmPRM.Total_KB, TmPRM.Die_KB, TmPLM.Parameter_MAX,
									TmPLM.Parameter_MIN, TmPLM.Parameter_VAL, TmPLM.DS_FG, TmPRM.RefQcParam_NO, TmPLM.Equipment_NO, TmPRM.EquipManage_FG,
                                    TmPLM.InnerUpperLimit, TmPLM.InnerLowerLimit, TmPLM.ParamGetUpperCond, TmPLM.ParamGetLowerCond
                                FROM TmPLM 
                                INNER JOIN TmPRM ON TmPLM.QcParam_NO = TmPRM.QcParam_NO 
                                WHERE (TmPLM.Model_NM = @ModelNM) AND (TmPLM.QcParam_NO = @QcParamNO) 
                                AND (TmPLM.Del_FG = 0) AND (TmPRM.Del_FG = 0) ";

                    if (!string.IsNullOrEmpty(identChar))
                    {
                        sql += " AND (TmPRM.Chip_NM = @IdentChar) ";
                        conn.SetParameter("@IdentChar", SqlDbType.VarChar, identChar);
                    }

                    conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM);
                    conn.SetParameter("@QcParamNO", SqlDbType.Int, qcparamNO);

                    if (materialCD == "")
                    {
                        throw new Exception(string.Format(Constant.MessageInfo.Message_28, materialCD, qcparamNO, ""));
                    }

                    sql += "  AND (TmPLM.Material_CD = @MaterialCD) ";
                    conn.SetParameter("@MaterialCD", SqlDbType.Char, materialCD);


                    sql += " Option(MAXDOP 1) ";

                    using (rd = conn.GetReader(sql))
                    {
                        string ordDieKB = "Die_KB";
                        int ordRefQcParamNO = rd.GetOrdinal("RefQcParam_NO");
                        int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
                        int ordParamGetUpperCond = rd.GetOrdinal("ParamGetUpperCond");
                        int ordParamGetLowerCond = rd.GetOrdinal("ParamGetLowerCond");

                        while (rd.Read())
                        {
                            plmInfo = new Plm();
                            plmInfo.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            plmInfo.ManageNM = Convert.ToString(rd["Manage_NM"]).Trim();
                            plmInfo.ParameterNM = Convert.ToString(rd["Parameter_NM"]).Trim();
                            plmInfo.TotalKB = Convert.ToString(rd["Total_KB"]).Trim();
                            plmInfo.EquipmentNO = rd.GetString(ordEquipmentNO).Trim();

                            if (!rd.IsDBNull(ordRefQcParamNO))
                            {
                                plmInfo.RefQcParamNO = rd.GetInt32(ordRefQcParamNO);
                            }
                            else
                            {
                                plmInfo.RefQcParamNO = null;
                            }

                            if (!rd.IsDBNull(rd.GetOrdinal(ordDieKB)))
                            {
                                plmInfo.DieKB = rd.GetString(rd.GetOrdinal(ordDieKB)).Trim();
                            }


                            plmInfo.ParameterVAL = Convert.ToString(rd["Parameter_VAL"]).Trim();
                            if (rd["Parameter_MAX"] != System.DBNull.Value)
                            {
                                plmInfo.ParameterMAX = Convert.ToDecimal(rd["Parameter_MAX"]);
                            }
                            if (rd["Parameter_MIN"] != System.DBNull.Value)
                            {
                                plmInfo.ParameterMIN = Convert.ToDecimal(rd["Parameter_MIN"]);
                            }
                            plmInfo.DsFG = Convert.ToBoolean(rd["DS_FG"]);

                            if (rd["InnerUpperLimit"] != System.DBNull.Value)
                            {
                                plmInfo.InnerUpperLimit = Convert.ToDecimal(rd["InnerUpperLimit"]);
                            }
                            if (rd["InnerLowerLimit"] != System.DBNull.Value)
                            {
                                plmInfo.InnerLowerLimit = Convert.ToDecimal(rd["InnerLowerLimit"]);
                            }

                            if (rd.IsDBNull(ordParamGetUpperCond))
                            {
                                plmInfo.ParamGetUpperCond = null;
                            }
                            else
                            {
                                plmInfo.ParamGetUpperCond = rd.GetFloat(ordParamGetUpperCond);
                            }

                            if (rd.IsDBNull(ordParamGetLowerCond))
                            {
                                plmInfo.ParamGetLowerCond = null;
                            }
                            else
                            {
                                plmInfo.ParamGetLowerCond = rd.GetFloat(ordParamGetLowerCond);
                            }
                            plmInfoList.Add(plmInfo);
                        }
                    }
                    conn.Command.Connection.Close();
                }
                //Model_NM、Material_CD, QcParam_NO指定で取得したレコードにおいて
                //設備CD指定と無指定のレコードが混在する場合はNG
                if (plmInfoList.Count > 1 && plmInfoList.Exists(p => string.IsNullOrEmpty(p.EquipmentNO)))
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_154, modelNM, materialCD, qcparamNO));
                }

                //取得レコードから設備コードがnullか空白のレコードを抽出。存在しなければ設備コード指定でレコード検索
                if (plmInfoList.Exists(p => string.IsNullOrEmpty(p.EquipmentNO)))
                {
                    return plmInfoList.Single();
                }
                else
                {
                    List<Plm> plmInfoSpecifyEquipList = plmInfoList.Where(p => p.EquipmentNO == equipmentNO).ToList();

                    if (plmInfoSpecifyEquipList.Count == 0)
                    {//設備CDを指定してレコードが取得出来ない場合
                        return null;
                    }

                    return plmInfoSpecifyEquipList.Single();
                }
            }
            catch (Exception err)
            {
                throw;
            }
        }

        /// <summary>
        /// 閾値マスタ[TmPLM]取得
        /// </summary>
        /// <param name="qcparamNO">管理NO</param>
        /// <param name="modelNM">装置型式</param>
        /// <param name="materialCD">製品型番</param>
        /// <returns>閾値マスタ</returns>
        public static List<Plm> GetPLMListData(List<int> qcparamNO, string modelNM, string equipmentNO, string materialCD, int lineCD, string identChar)
        {
            System.Data.Common.DbDataReader rd = null;
            Plm plmInfo = null;
            List<Plm> plmInfoList = new List<Plm>();

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT TmPRM.QcParam_NO, TmPRM.Manage_NM, TmPRM.Parameter_NM, TmPRM.Total_KB, TmPRM.Die_KB, TmPLM.Parameter_MAX,
									TmPLM.Parameter_MIN, TmPLM.Parameter_VAL, TmPLM.DS_FG, TmPRM.RefQcParam_NO, TmPLM.Equipment_NO, TmPRM.EquipManage_FG,
                                    TmPLM.InnerUpperLimit, TmPLM.InnerLowerLimit,  TmPRM.UnManageTrend_FG, TmPRM.WithoutFileFmt_FG, TmPRM.ChangeUnit_VAL
                                FROM TmPLM 
                                INNER JOIN TmPRM ON TmPLM.QcParam_NO = TmPRM.QcParam_NO 
                                WHERE (TmPLM.Model_NM = @ModelNM) 
                                AND (TmPLM.Del_FG = 0) AND (TmPRM.Del_FG = 0) ";

                    if (qcparamNO.Count != 0)
                    {
                        string sQcparamNO = "(" + String.Join(",", qcparamNO.ToArray()) + ")";
                        sql += " AND (TmPLM.QcParam_NO IN " + sQcparamNO + ")";
                        //sql += "AND (TmPLM.QcParam_NO IN @QcParamNO) ";
                        //conn.SetParameter("@QcParamNO", SqlDbType.Int, qcparamNO);
                    }

                    if (!string.IsNullOrEmpty(identChar))
                    {
                        sql += " AND (TmPRM.Chip_NM = @IdentChar) ";
                        conn.SetParameter("@IdentChar", SqlDbType.VarChar, identChar);
                    }

                    conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM);

                    if (materialCD == "")
                    {
                        throw new Exception(string.Format(Constant.MessageInfo.Message_28, materialCD, qcparamNO, ""));
                    }

                    sql += "  AND (TmPLM.Material_CD = @MaterialCD) ";
                    conn.SetParameter("@MaterialCD", SqlDbType.Char, materialCD);


                    sql += " Option(MAXDOP 1) ";

                    using (rd = conn.GetReader(sql))
                    {
                        string ordDieKB = "Die_KB";
                        int ordRefQcParamNO = rd.GetOrdinal("RefQcParam_NO");
                        int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
                        int ordWithoutFmt = rd.GetOrdinal("WithoutFileFmt_FG");
                        int ordUnManage = rd.GetOrdinal("UnManageTrend_FG");
                        int ordChangeUnit = rd.GetOrdinal("ChangeUnit_VAL");

                        while (rd.Read())
                        {
                            plmInfo = new Plm();
                            plmInfo.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            plmInfo.ManageNM = Convert.ToString(rd["Manage_NM"]).Trim();
                            plmInfo.ParameterNM = Convert.ToString(rd["Parameter_NM"]).Trim();
                            plmInfo.TotalKB = Convert.ToString(rd["Total_KB"]).Trim();
                            plmInfo.EquipmentNO = rd.GetString(ordEquipmentNO).Trim();

                            if (!rd.IsDBNull(ordRefQcParamNO))
                            {
                                plmInfo.RefQcParamNO = rd.GetInt32(ordRefQcParamNO);
                            }
                            else
                            {
                                plmInfo.RefQcParamNO = null;
                            }

                            if (!rd.IsDBNull(rd.GetOrdinal(ordDieKB)))
                            {
                                plmInfo.DieKB = rd.GetString(rd.GetOrdinal(ordDieKB)).Trim();
                            }


                            plmInfo.ParameterVAL = Convert.ToString(rd["Parameter_VAL"]).Trim();
                            if (rd["Parameter_MAX"] != System.DBNull.Value)
                            {
                                plmInfo.ParameterMAX = Convert.ToDecimal(rd["Parameter_MAX"]);
                            }
                            if (rd["Parameter_MIN"] != System.DBNull.Value)
                            {
                                plmInfo.ParameterMIN = Convert.ToDecimal(rd["Parameter_MIN"]);
                            }
                            plmInfo.DsFG = Convert.ToBoolean(rd["DS_FG"]);

                            if (rd["InnerUpperLimit"] != System.DBNull.Value)
                            {
                                plmInfo.InnerUpperLimit = Convert.ToDecimal(rd["InnerUpperLimit"]);
                            }
                            if (rd["InnerLowerLimit"] != System.DBNull.Value)
                            {
                                plmInfo.InnerLowerLimit = Convert.ToDecimal(rd["InnerLowerLimit"]);
                            }
                            plmInfo.WithoutFileFmt_FG = rd.GetBoolean(ordWithoutFmt);
                            plmInfo.UnManageTrend_FG = rd.GetBoolean(ordUnManage);
                            plmInfo.ChangeUnitVal = rd.GetString(ordChangeUnit).Trim();

                            plmInfoList.Add(plmInfo);
                        }
                    }
                    conn.Command.Connection.Close();
                }
                return plmInfoList;
            }
            catch (Exception err)
            {
                throw;
            }
        }

        /// <summary>
        /// 閾値マスタ[TmPLM]に存在する型番を取得
        /// </summary>
        /// <param name="modelNM">装置型式</param>
        /// <returns>型番</returns>
        public static List<string> GetPLMTypeData(string modelNM, int lineCD)
        {
            System.Data.Common.DbDataReader rd = null;
            List<string> typeCDList = new List<string>();

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT Material_CD
                                FROM TmPLM WITH (nolock) 
                                WHERE Del_FG = 0 AND Model_NM = @ModelNM 
                                GROUP BY Material_CD 
                                ORDER BY Material_CD ";

                conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM);

                using (rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        string typeCD = Convert.ToString(rd["Material_CD"]).Trim();
                        typeCDList.Add(typeCD);
                    }
                }
            }

            return typeCDList;
        }

        /// <summary>
        /// 管理項目マスタ[TmPRM]に存在するチップを取得
        /// </summary>
        /// <param name="modelNM">装置型式</param>
        /// <returns>チップ</returns>
        //		public static List<string> GetPRMChipData(string modelNM, int lineCD)
        //		{
        //			System.Data.Common.DbDataReader rd = null;
        //			List<string> chipNMList = new List<string>();

        //			using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
        //			{
        //				string sql = @" SELECT Chip_NM
        //                                FROM TmPRM WITH (nolock) 
        //                                WHERE Del_FG = 0 AND Model_NM = @ModelNM 
        //                                GROUP BY Chip_NM ";

        //				conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM);

        //				using (rd = conn.GetReader(sql))
        //				{
        //					while (rd.Read())
        //					{
        //						string chipNM = Convert.ToString(rd["Chip_NM"]).Trim();
        //						chipNMList.Add(chipNM);
        //					}
        //				}
        //			}

        //			return chipNMList;
        //		}

        public static List<string> GetPRMChipData(string modelNM, int lineCD)
        {
            System.Data.Common.DbDataReader rd = null;
            List<string> chipNMList = new List<string>();

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT TmPRM.Chip_NM
								FROM TmPLM WITH(NOLOCK) INNER JOIN TmPRM WITH(NOLOCK) ON TmPRM.QcParam_NO = TmPLM.QcParam_NO
								WHERE (TmPLM.Model_NM = @ModelNM) AND (TmPRM.Chip_NM IS NOT NULL) AND (TmPLM.Del_FG = 0) AND (TmPRM.Del_FG = 0)
								GROUP BY TmPRM.Chip_NM OPTION (MAXDOP 1) ";

                conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM);

                using (rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        string chipNM = Convert.ToString(rd["Chip_NM"]).Trim();
                        chipNMList.Add(chipNM);
                    }
                }
            }

            return chipNMList;
        }


        public static List<FILEFMTInfo> GetFILEFMTData(string prefixNM, LSETInfo lsetInfo, bool startUpFG)
        {
            return GetFILEFMTData(prefixNM, lsetInfo, startUpFG, false);
        }

        /// <summary>
        /// ログファイル紐付けマスタ[TmFILEFMT]取得。
        /// 閾値上位照合時はnoCheckEquipPartID = trueで指定する。(TmPLM側に指定が無いので、TmFILEFMT側もEquipPartIDを無視して照合する)
        /// </summary>
        /// <param name="prefixNM">ファイル種類</param>
        /// <param name="modelNM">装置型式</param>
        /// <returns>紐付けマスタ</returns>
        public static List<FILEFMTInfo> GetFILEFMTData(string prefixNM, LSETInfo lsetInfo, bool startUpFG, bool noCheckEquipPartID)
        //public static List<FILEFMTInfo> GetFILEFMTData(string prefixNM, LSETInfo lsetInfo, bool startUpFG)
        {
            System.Data.Common.DbDataReader rd = null;
            List<FILEFMTInfo> filefmtList = new List<FILEFMTInfo>();

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT TmFILEFMT.QcParam_NO, TmPRM.Parameter_NM, TmFILEFMT.Column_NO, TmFILEFMT.Search_NM, TmFILEFMT.MachinePrefix_NM,
										TmFILEFMT.Prefix_NM, TmPRM.Chip_NM, TmFILEFMT.Header_NM, TmFILEFMT.StartUp_FG, TmFILEFMT.XPath, TmFILEFMT.XPath_SearchNO
									FROM TmFILEFMT WITH(nolock)              
									INNER JOIN TmPRM WITH(nolock) ON TmPRM.QcParam_NO = TmFILEFMT.QcParam_NO
									WHERE (TmFILEFMT.Model_NM = @ModelNM) AND (TmPRM.Del_FG = 0) AND (TmFILEFMT.Del_FG = 0) AND (TmFILEFMT.StartUp_FG = @StartUpFG) ";


                    if (string.IsNullOrEmpty(lsetInfo.EquipPartID) == false && string.IsNullOrEmpty(lsetInfo.ThreadGrpCD) == false && noCheckEquipPartID == false)
                    //if (string.IsNullOrEmpty(lsetInfo.EquipPartID) == false && string.IsNullOrEmpty(lsetInfo.ThreadGrpCD) == false)
                    {
                        sql += " AND (TmFILEFMT.EquipPart_ID = @EquipPartID) ";
                        conn.SetParameter("@EquipPartID", SqlDbType.VarChar, lsetInfo.EquipPartID);
                    }

                    conn.SetParameter("@ModelNM", SqlDbType.VarChar, lsetInfo.ModelNM);
                    conn.SetParameter("@StartUpFG", SqlDbType.Bit, startUpFG);

                    using (rd = conn.GetReader(sql))
                    {
                        int ordParamNo = rd.GetOrdinal("QcParam_NO");
                        int ordParamNm = rd.GetOrdinal("Parameter_NM");
                        int ordColNo = rd.GetOrdinal("Column_NO");
                        int ordSearchNm = rd.GetOrdinal("Search_NM");
                        int ordMacPrefixNm = rd.GetOrdinal("MachinePrefix_NM");
                        int ordPrefixNm = rd.GetOrdinal("Prefix_NM");
                        int ordChipNm = rd.GetOrdinal("Chip_NM");
                        int ordHeaderNm = rd.GetOrdinal("Header_NM");
                        int ordStartUpFG = rd.GetOrdinal("StartUp_FG");
                        int ordXPath = rd.GetOrdinal("XPath");
                        int ordXPath_SearchNO = rd.GetOrdinal("XPath_SearchNO");

                        while (rd.Read())
                        {
                            FILEFMTInfo filefmtInfo = new FILEFMTInfo();
                            filefmtInfo.QCParamNO = rd.GetInt32(ordParamNo);
                            filefmtInfo.ParameterNM = rd.GetString(ordParamNm).Trim();

                            if (rd.IsDBNull(ordColNo) == false)
                            {
                                filefmtInfo.ColumnNO = rd.GetInt32(ordColNo);
                            }
                            else
                            {
                                filefmtInfo.ColumnNO = int.MinValue;
                            }

                            if (rd.IsDBNull(ordSearchNm) == false)
                            {
                                filefmtInfo.SearchNM = rd.GetString(ordSearchNm).Trim();
                            }
                            else
                            {
                                filefmtInfo.SearchNM = null;
                            }

                            if (rd.IsDBNull(ordMacPrefixNm) == false)
                            {
                                filefmtInfo.MachinePrefixNM = rd.GetString(ordMacPrefixNm).Trim();
                            }
                            else
                            {
                                filefmtInfo.MachinePrefixNM = null;
                            }

                            filefmtInfo.PrefixNM = rd.GetString(ordPrefixNm).Trim();

                            if (rd.IsDBNull(ordChipNm) == false)
                            {
                                filefmtInfo.ChipNM = rd.GetString(ordChipNm).Trim();
                            }
                            else
                            {
                                filefmtInfo.ChipNM = string.Empty;
                            }

                            if (rd.IsDBNull(ordHeaderNm) == false)
                            {
                                filefmtInfo.HeaderNM = rd.GetString(ordHeaderNm).Trim();
                            }
                            else
                            {
                                filefmtInfo.HeaderNM = null;
                            }

                            if (rd.IsDBNull(ordStartUpFG))
                            {
                                filefmtInfo.StartUpFG = false;
                            }
                            else
                            {
                                filefmtInfo.StartUpFG = rd.GetBoolean(ordStartUpFG);
                            }

                            if (rd.IsDBNull(ordXPath) == false)
                            {
                                filefmtInfo.XPath = rd.GetString(ordXPath).Trim();
                            }
                            else
                            {
                                filefmtInfo.XPath = string.Empty;
                            }

                            if (rd.IsDBNull(ordXPath_SearchNO) == false)
                            {
                                filefmtInfo.XPath_SearchNO = rd.GetInt32(ordXPath_SearchNO);
                            }
                            else
                            {
                                filefmtInfo.XPath_SearchNO = 0;
                            }

                            filefmtList.Add(filefmtInfo);
                        }
                    }
                }

                List<FILEFMTInfo> selectedFilefmtList = new List<FILEFMTInfo>();

                if (prefixNM != null)
                {
                    selectedFilefmtList = filefmtList.Where(f => f.PrefixNM == prefixNM).ToList();
                }
                else
                {
                    selectedFilefmtList = filefmtList;
                }

                if (lsetInfo.ChipNM != "" && lsetInfo.ChipNM != null)
                {
                    return selectedFilefmtList.Where(f => f.ChipNM == lsetInfo.ChipNM).ToList();
                }
                else
                {
                    return selectedFilefmtList;
                }

            }
            catch (Exception err)
            {
                throw;
            }
        }

        /// <summary>
        /// ログファイル紐付けマスタ[TmFILEFMT]取得
        /// </summary>
        /// <param name="prefixNM">ファイル種類</param>
        /// <param name="modelNM">装置型式</param>
        /// <returns>紐付けマスタ</returns>
        public static List<FILEFMTInfo> GetFILEFMTData(string prefixNM, LSETInfo lsetInfo, string typeCD)
        {
            System.Data.Common.DbDataReader rd = null;
            List<FILEFMTInfo> filefmtList = new List<FILEFMTInfo>();

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT TmFILEFMT.QcParam_NO, TmFILEFMT.Header_NM, TmPRM.Parameter_NM, TmFILEFMT.Column_NO, TmFILEFMT.Search_NM, TmFILEFMT.MachinePrefix_NM, TmFILEFMT.Prefix_NM, TmPRM.Chip_NM
                                FROM TmFILEFMT WITH(nolock)              
                                INNER JOIN TmPRM WITH(nolock) ON TmPRM.QcParam_NO = TmFILEFMT.QcParam_NO
                                WHERE (TmFILEFMT.Model_NM = @ModelNM) AND (TmFILEFMT.Del_FG = 0) AND (TmFILEFMT.QcParam_NO <> @NoChkParamNo) ";

                    //if (prefixNM != "")
                    //{
                    //    sql += " AND (TmFILEFMT.Prefix_NM = @PrefixNM) ";
                    //    conn.SetParameter("@PrefixNM", SqlDbType.VarChar, prefixNM);
                    //}

                    //if (lsetInfo.ChipNM != "" && lsetInfo.ChipNM != null)
                    //{
                    //    sql += " AND (TmPRM.Chip_NM = @ChipNM) ";
                    //    conn.SetParameter("@ChipNM", SqlDbType.VarChar, lsetInfo.ChipNM);
                    //}

                    conn.SetParameter("@ModelNM", SqlDbType.VarChar, lsetInfo.ModelNM);
                    conn.SetParameter("@NoChkParamNo", SqlDbType.Int, Constant.NO_CHK_QCPARAM_NO);

                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            FILEFMTInfo filefmtInfo = new FILEFMTInfo();
                            filefmtInfo.QCParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            filefmtInfo.ParameterNM = Convert.ToString(rd["Parameter_NM"]).Trim();
                            filefmtInfo.ColumnNO = Convert.ToInt32(rd["Column_NO"]);
                            filefmtInfo.HeaderNM = Convert.ToString(rd["Header_NM"]).Trim();
                            filefmtInfo.SearchNM = Convert.ToString(rd["Search_NM"]).Trim();
                            filefmtInfo.MachinePrefixNM = Convert.ToString(rd["MachinePrefix_NM"]).Trim();
                            filefmtInfo.PrefixNM = Convert.ToString(rd["Prefix_NM"]).Trim();
                            filefmtInfo.ChipNM = Convert.ToString(rd["Chip_NM"]).Trim();
                            filefmtList.Add(filefmtInfo);
                        }
                    }
                }

                List<FILEFMTInfo> selectedFilefmtList = filefmtList.Where(f => f.PrefixNM == prefixNM).ToList();

                if (lsetInfo.ChipNM != "" && lsetInfo.ChipNM != null)
                {
                    return selectedFilefmtList.Where(f => f.ChipNM == lsetInfo.ChipNM).ToList();
                }
                else
                {
                    return selectedFilefmtList;
                }

            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// ログファイル紐付けマスタ[TmFILEFMT]取得
        /// </summary>
        /// <param name="prefixNM">ファイル種類</param>
        /// <param name="modelNM">装置型式</param>
        /// <returns>紐付けマスタ</returns>
        public static List<FILEFMTInfo> GetFILEFMTData(string prefixNM, LSETInfo lsetInfo, string typeCD, string filePrefix)
        {
            System.Data.Common.DbDataReader rd = null;
            List<FILEFMTInfo> filefmtList = new List<FILEFMTInfo>();

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT TmFILEFMT.QcParam_NO, TmPRM.Parameter_NM, TmFILEFMT.Column_NO, TmFILEFMT.Search_NM, TmFILEFMT.MachinePrefix_NM,
										TmFILEFMT.Prefix_NM, TmPRM.Chip_NM
									FROM TmFILEFMT WITH(nolock)              
									INNER JOIN TmPRM WITH(nolock) ON TmPRM.QcParam_NO = TmFILEFMT.QcParam_NO
									WHERE (TmFILEFMT.Model_NM = @ModelNM) AND (TmFILEFMT.Del_FG = 0) ";

                    //if (prefixNM != "")
                    //{
                    //    sql += " AND (TmFILEFMT.Prefix_NM = @PrefixNM) ";
                    //    conn.SetParameter("@PrefixNM", SqlDbType.VarChar, prefixNM);
                    //}

                    conn.SetParameter("@ModelNM", SqlDbType.VarChar, lsetInfo.ModelNM);

                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            FILEFMTInfo filefmtInfo = new FILEFMTInfo();
                            filefmtInfo.QCParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            filefmtInfo.ParameterNM = Convert.ToString(rd["Parameter_NM"]).Trim();
                            filefmtInfo.ColumnNO = Convert.ToInt32(rd["Column_NO"]);
                            filefmtInfo.SearchNM = Convert.ToString(rd["Search_NM"]).Trim();
                            filefmtInfo.MachinePrefixNM = Convert.ToString(rd["MachinePrefix_NM"]).Trim();
                            filefmtInfo.PrefixNM = Convert.ToString(rd["Prefix_NM"]).Trim();
                            filefmtInfo.ChipNM = Convert.ToString(rd["Chip_NM"]).Trim();

                            filefmtList.Add(filefmtInfo);
                        }
                    }
                }

                //白樹脂対応に当たって、下記のチェックに問題が出てきた為、機能除去 SGA41と話合い済み 2015/1/15
                //if (filefmtList.Count != 0)
                //{
                //    List<Plm> plmList = Plm.GetDatas(lsetInfo.InlineCD, typeCD, lsetInfo.ModelNM, false);
                //    foreach (Plm p in plmList)
                //    {
                //        if (!filefmtList.Exists(f => f.QCParamNO == p.QcParamNO))
                //        {
                //            AlertLog log = AlertLog.GetInstance();
                //            log.logMessageQue.Enqueue(
                //                string.Format("紐付けマスタに存在しない閾値があります。QcParamNo:{0} ParameterNm:{1}", p.QcParamNO, p.ParameterNM));

                //            throw new ApplicationException("紐付けマスタ不具合の為、装置監視を停止しました。");
                //        }
                //        System.Threading.Thread.Sleep(0);
                //    }
                //}

                List<FILEFMTInfo> selectedFilefmtList = filefmtList.Where(f => f.PrefixNM == prefixNM).ToList();

                if (lsetInfo.ChipNM != "" && lsetInfo.ChipNM != null)
                {
                    return selectedFilefmtList.Where(f => f.ChipNM == lsetInfo.ChipNM).ToList();
                }
                else
                {
                    return selectedFilefmtList;
                }

            }
            catch (Exception err)
            {
                throw;
            }
        }

        /// <summary>
        /// ログファイル紐付けWBマスタ[TmFILEFMT_WB]を取得
        /// </summary>
        /// <param name="prefixNM">ファイル種類</param>
        /// <param name="modelNM">装置型式</param>
        /// <returns>紐付けWBマスタ</returns>
        public static List<FILEFMTWBInfo> GetFILEFMTWBData(string prefixNM, string typeCD, string modelNM, int lineCD)
        {
            AlertLog alertLog = AlertLog.GetInstance();

            List<FILEFMTWBInfo> filefmtList = new List<FILEFMTWBInfo>();

            Database.LENS.Type type = Database.LENS.Type.Get(typeCD, lineCD);
            if (type == null)
            {
                throw new ApplicationException(
                    string.Format("該当する製品型番情報が「WBファイル内紐付け方法 製品型番分類マスタ」に存在しませんでした。製品型番:{0}", typeCD));
            }


            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT TmFILEFMT_WB.Prefix_NM, TmFILEFMT_WB.QcParam_NO, TmFILEFMT_WB.Function_NO, 
								  TmFILEFMT_WB.Search_NM, TmFILEFMT_WB.Search_NO, TmFILEFMT_WB.Comma_NO, TmPRM.Parameter_NM
								FROM TmFILEFMT_WB WITH (nolock) 
                                    INNER JOIN TmPRM WITH (nolock) ON TmPRM.QcParam_NO = TmFILEFMT_WB.QcParam_NO
                                WHERE (TmFILEFMT_WB.Del_FG = 0) AND (TmPRM.Del_FG = 0) AND (TmFILEFMT_WB.Model_NM = @ModelNM) AND (TmFILEFMT_WB.FileFmt_NO = @FileFmtNO)";

                conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM);
                conn.SetParameter("@FileFmtNO", SqlDbType.Int, type.FileFmtNO);
                //[LENS2へ機能移行]
                //conn.SetParameter("@TypeCD", SqlDbType.Char, typeNM);

                using (System.Data.Common.DbDataReader rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        FILEFMTWBInfo filefmtInfo = new FILEFMTWBInfo();

                        //[LENS2へ機能移行]
                        //filefmtInfo.TypeCD = Convert.ToString(rd["Type_CD"]).Trim();
                        filefmtInfo.PrefixNM = Convert.ToString(rd["Prefix_NM"]);
                        filefmtInfo.ParameterNM = Convert.ToString(rd["Parameter_NM"]);
                        filefmtInfo.QCParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                        filefmtInfo.FunctionNO = Convert.ToInt32(rd["Function_NO"]);
                        filefmtInfo.SearchNM = Convert.ToString(rd["Search_NM"]);
                        filefmtInfo.SearchNO = Convert.ToInt32(rd["Search_NO"]);
                        filefmtInfo.Comma_NO = Convert.ToInt32(rd["Comma_NO"]);

                        filefmtList.Add(filefmtInfo);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(prefixNM))
            {
                return filefmtList.ToList();
            }
            else
            {
                return filefmtList.Where(f => f.PrefixNM == prefixNM).ToList();
            }
        }

        /// <summary>
        /// メッセージ紐付けマスタ[TmMSGFMT]を取得
        /// </summary>
        /// <param name="typeNM">製品型番</param>
        /// <param name="modelNM">装置型式</param>
        /// <returns>紐付けマスタ</returns>
        public static List<MSGFMTInfo> GetMSGFMTData(string msgTypeCD, string typeNM, string modelNM, int lineCD)
        {
            System.Data.Common.DbDataReader rd = null;
            List<MSGFMTInfo> fmtList = new List<MSGFMTInfo>();

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT TmMSGFMT.MsgType_CD, TmMSGFMT.Model_NM, TmMSGFMT.MsgFmt_NO, TmMSGFMT.QcParam_NO, TmMSGFMT.SearchParam_NO, TmMSGFMT.SearchGrpParam_NO,
                                    TmMSGFMT.SearchParam_NM, TmMSGFMT.SearchValue_NO, TmPRM.Parameter_NM 
                                FROM TmMSGFMT WITH (nolock) 
                                    INNER JOIN TmPRM WITH (nolock) ON TmMSGFMT.QcParam_NO = TmPRM.QcParam_NO 
                                    INNER JOIN TmMSGFMTTYPE WITH (nolock) ON TmMSGFMT.MsgFmt_NO = TmMSGFMTTYPE.MsgFmt_NO 
                                WHERE (TmMSGFMTTYPE.Type_CD = @TypeCD) AND (TmMSGFMT.Model_NM = @ModelNM) 
                                    AND (TmPRM.Del_FG = 0) AND (TmMSGFMT.Del_FG = 0) AND (TmMSGFMTTYPE.Del_FG = 0) ";

                conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM);
                conn.SetParameter("@TypeCD", SqlDbType.Char, typeNM);

                using (rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        MSGFMTInfo fmtInfo = new MSGFMTInfo();
                        fmtInfo.MsgTypeCD = rd.GetString(rd.GetOrdinal("MsgType_CD")).Trim();
                        fmtInfo.QcParamNO = rd.GetInt32(rd.GetOrdinal("QcParam_NO"));
                        fmtInfo.ParameterNM = rd.GetString(rd.GetOrdinal("Parameter_NM")).Trim();

                        fmtInfo.SearchParamNO = rd.GetInt32(rd.GetOrdinal("SearchParam_NO"));
                        fmtInfo.SearchGrpParamNO = rd.GetInt32(rd.GetOrdinal("SearchGrpParam_NO"));

                        int ordSearchParamNM = rd.GetOrdinal("SearchParam_NM");
                        if (!rd.IsDBNull(ordSearchParamNM))
                        {
                            fmtInfo.SearchParamNM = rd.GetString(ordSearchParamNM).Trim();
                        }

                        fmtInfo.SearchValueNO = rd.GetInt32(rd.GetOrdinal("SearchValue_NO"));
                        fmtList.Add(fmtInfo);
                    }
                }
            }

            //白樹脂対応に当たって、下記のチェックに問題が出てきた為、機能除去 SGA41と話合い済み 2015/1/15
            //if (fmtList.Count != 0)
            //{
            //    List<Plm> plmList = Plm.GetDatas(lineCD, typeNM, modelNM, false);
            //    foreach (Plm p in plmList)
            //    {
            //        if (!fmtList.Exists(f => f.QcParamNO == p.QcParamNO))
            //        {
            //            AlertLog log = AlertLog.GetInstance();
            //            log.logMessageQue.Enqueue(
            //                string.Format("紐付けマスタに存在しない閾値があります。QcParamNo:{0} ParameterNm:{1}", p.QcParamNO, p.ParameterNM));

            //            throw new ApplicationException("紐付けマスタ不具合の為、装置監視を停止しました。");
            //        }
            //    }
            //}

            return fmtList.Where(f => f.MsgTypeCD == msgTypeCD).ToList();
        }

        /// <summary>
        /// 外観検査機エラー番号変換マスタ[TmErrConv]取得
        /// </summary>
        /// <param name="plantCD">設備NO</param>
        /// <param name="qcParamNO">管理NO</param>
        /// <returns>外観検査機エラー番号変換マスタ</returns>
        public static ErrConvInfo GetErrConvInfo(string plantCD, int qcParamNO, int lineCD)
        {
            System.Data.Common.DbDataReader rd = null;
            ErrConvInfo errConvInfo = null;

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT Equipment_NO, EquiErr_NO, QcParam_NO, NascaErr_NO, DefCause_CD, DefClass_CD
                                FROM TmErrConv
                                WHERE (QcParam_NO = @QcParamNO) AND (Equipment_NO = @EquipmentNO) ";

                    conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNO);
                    conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, plantCD);

                    using (rd = conn.GetReader(sql))
                    {
                        if (!rd.HasRows)
                        {
                            string sMessage = "[管理番号:" + qcParamNO + "]ErrConvマスタに設定されていません。システム担当者に連絡して下さい。";
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                            throw new Exception(sMessage);
                        }

                        while (rd.Read())
                        {
                            errConvInfo = new ErrConvInfo();
                            errConvInfo.EquipmentNO = Convert.ToString(rd["Equipment_NO"]).Trim();
                            errConvInfo.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            errConvInfo.EquiErrNO = Convert.ToString(rd["EquiErr_NO"]).Trim();
                            errConvInfo.NascaErrNO = Convert.ToString(rd["NascaErr_NO"]).Trim();
                        }
                    }
                }

                return errConvInfo;
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public static Dictionary<string, string> GetMachineFilePrefix(LSETInfo lsetInfo, int timingNO)
        {
            return GetMachineFilePrefix(lsetInfo, timingNO, null, null, null);
        }

        public static Dictionary<string, string> GetMachineFilePrefix(LSETInfo lsetInfo, int timingNO, bool? isStartUp)
        {
            return GetMachineFilePrefix(lsetInfo, timingNO, isStartUp, null, null);
        }

        /// <summary>
        /// 設備ファイル識別文字を取得(Database.FILEFMT.csの仕様を推奨
        /// </summary>
        /// <param name="modelNM">型式</param>
        /// <param name="timingNO">タイミングNO</param>
        /// <returns>ファイル識別, 設備識別</returns>
        public static Dictionary<string, string> GetMachineFilePrefix(LSETInfo lsetInfo, int timingNO, bool? isStartUp, string prefixNm, string equipPartNo)
        {
            System.Data.Common.DbDataReader rd = null;
            Dictionary<string, string> prefixList = new Dictionary<string, string>();

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
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

                    if (string.IsNullOrEmpty(prefixNm) == false)
                    {
                        sqlWhere += " AND (TmFILEFMT.Prefix_NM = @PrefixNM) ";
                        conn.SetParameter("@PrefixNM", SqlDbType.VarChar, prefixNm);
                    }

                    if (string.IsNullOrEmpty(equipPartNo) == false)
                    {
                        sqlWhere += " AND (TmFILEFMT.EquipPart_ID = @EquipPartID) ";
                        conn.SetParameter("@EquipPartID", SqlDbType.NVarChar, equipPartNo);
                    }

                    if (lsetInfo.AssetsNM == Constant.ASSETS_WB_NM)
                    {
                        sql = @" SELECT TmFILEFMT.Prefix_NM, '' as MachinePrefix_NM
                                FROM TmFILEFMT_WB AS TmFILEFMT WITH(nolock)
                                INNER JOIN TmPRM ON TmFILEFMT.QcParam_NO = TmPRM.QcParam_NO
                                WHERE (TmPRM.Del_FG = 0) AND (TmFILEFMT.Del_FG = 0) AND (TmFILEFMT.XPath IS NULL)" + sqlWhere;
                        sql += " GROUP BY TmFILEFMT.Prefix_NM ";
                    }
                    else
                    {
                        sql = @" SELECT TmFILEFMT.Prefix_NM, TmFILEFMT.MachinePrefix_NM
                                FROM TmFILEFMT WITH(nolock)
                                INNER JOIN TmPRM ON TmFILEFMT.QcParam_NO = TmPRM.QcParam_NO
                                WHERE (TmPRM.Del_FG = 0) AND (TmFILEFMT.Del_FG = 0)  AND (TmFILEFMT.XPath IS NULL)
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
                //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, 
                //    string.Format("設備番号{0} タイミングNO{1} 設備ファイル識別文字の取得に失敗", lsetInfo.EquipmentNO, timingNO));
                throw;
            }

            return prefixList;
        }

        /// <summary>
        /// 樹脂少量不良データ[TnDEFECTRESIN]を取得
        /// </summary>
        /// <param name="lotNO">ロット番号</param>
        /// <returns>樹脂少量不良データ</returns>
        public static List<DEFECTRESINInfo> GetDEFECTRESINData(string lotNO, int lineCD)
        {
            List<DEFECTRESINInfo> defectResinList = new List<DEFECTRESINInfo>();

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT Address_NO
                                    FROM dbo.TnDEFECTRESIN AS TnDEFECTRESIN WITH(nolock)
                                    WHERE (Del_FG = 0) AND (Lot_NO = @LotNO)";

                    conn.SetParameter("@LotNO", SqlDbType.VarChar, lotNO);

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            DEFECTRESINInfo defectResinInfo = new DEFECTRESINInfo();
                            defectResinInfo.AddressNO = rd.GetInt32(rd.GetOrdinal("Address_NO"));
                            defectResinList.Add(defectResinInfo);
                        }
                    }
                }

                return defectResinList;
            }
            catch (Exception err)
            {
                throw;
            }
        }



        /// <summary>
        /// マッピング順序並び替えマスタ[TmMPGORDER]を取得
        /// </summary>
        /// <param name="modelNM">装置型式</param>
        public static List<MPGORDERInfo> GetMPGORDERData(int lineCD, string modelNM, int timingNO)
        {
            List<MPGORDERInfo> mpgOrderInfoList = new List<MPGORDERInfo>();

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT Model_NM, Proc_NO, UpdUser_CD, LastUpd_DT
                                FROM TmMPGORDER WITH(nolock) WHERE (del_fg = 0) 
                                AND (Model_NM = @ModelNM) AND (Timing_NO = @TimingNO)";

                conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM);
                conn.SetParameter("@TimingNO", SqlDbType.Int, timingNO);

                using (DbDataReader rd = conn.GetReader(sql))
                {
                    //if (!rd.HasRows)
                    //{
                    //    return null;
                    //}

                    while (rd.Read())
                    {
                        MPGORDERInfo mpgOrderInfo = new MPGORDERInfo();
                        mpgOrderInfo.ProcNO = rd.GetInt64(rd.GetOrdinal("Proc_NO"));

                        mpgOrderInfoList.Add(mpgOrderInfo);
                    }
                }
            }

            return mpgOrderInfoList;
        }

        /// <summary>
        /// ウェハーログ[TnDBWF]を登録
        /// </summary>
        /// <param name="plantcd">設備CD</param>
        /// <param name="dt"></param>
        /// <param name="log"></param>
        public void InsertDBWF(int lineCD, string plantcd, DateTime dt, string log)
        {
            int macno = ConnectDB.GetMacNo(plantcd, lineCD);

            this.Connection.SetParameter("@PLANTCD", System.Data.SqlDbType.NVarChar, macno);
            this.Connection.SetParameter("@DT", System.Data.SqlDbType.DateTime, dt);

            string sql = "SELECT plant_cd FROM TnDBWF WHERE plant_cd=@PLANTCD AND Log_DT = @DT";

            //記録済みのログは保存しない
            if (this.Connection.ExecuteScalar(sql) != null)
            {
                return;
            }

            this.Connection.SetParameter("@MSG", System.Data.SqlDbType.NVarChar, log);
            sql = "INSERT TnDBWF(plant_cd, log_dt, message) VALUES(@PLANTCD, @DT, @MSG)";
            this.Connection.ExecuteNonQuery(sql);
        }

        #endregion

        #region ARMS

        /// <summary>
        /// ロットNOから製品型番を取得
        /// </summary>
        /// <param name="lotNO">ロットNO</param>
        /// <returns>製品型番</returns>
        public static string GetARMSLotType(int lineCD, string lotNO)
        {
            string sql = @" SELECT t.typecd 
                        FROM tnlot t With(NOLOCK) 
                        WHERE t.lotno = @LOTNO ";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {
                if (lotNO.Contains("_#"))
                {
                    lotNO = lotNO.Substring(0, lotNO.IndexOf("_#"));
                }
                conn.SetParameter("@LOTNO", SqlDbType.NVarChar, lotNO);

                object typeCD = conn.ExecuteScalar(sql);
                if (typeCD == null || typeCD == System.DBNull.Value)
                {
                    throw new Exception(string.Format(Constant.MessageInfo.Message_65, lotNO));
                }
                else
                {
                    return typeCD.ToString();
                }
            }
        }
        /// <summary>
        /// ARMSのTypeとLotを抜く。※マッピング関連のフラグは取得しないので注意。
        /// </summary>
        /// <param name="lineCD"></param>
        /// <param name="magNo"></param>
        /// <returns></returns>
        public static ARMSLotInfo GetARMSLotInfoFromMag(int lineCD, string magNo)
        {
            ARMSLotInfo lotInfo = new ARMSLotInfo();

            string sql = @" SELECT l.typecd, l.lotno
                        FROM TnLot AS l WITH(nolock) 
                        INNER JOIN TnMag WITH(nolock) ON (l.lotno = TnMag.lotno OR l.lotno = TnMag.lotno + '_#1' OR l.lotno = TnMag.lotno + '_#2') 
                        WHERE (TnMag.newfg = 1) and (TnMag.magno LIKE @MAG)";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {
                conn.SetParameter("@MAG", SqlDbType.NVarChar, magNo + "%");

                using (DbDataReader rd = conn.GetReader(sql))
                {
                    if (!rd.HasRows)
                    {
                        throw new Exception(string.Format(Constant.MessageInfo.Message_85, magNo));
                    }
                    else
                    {
                        if (rd.Read())
                        {
                            lotInfo.TypeCD = rd.GetString(rd.GetOrdinal("typecd")).Trim();
                            lotInfo.LotNO = rd.GetString(rd.GetOrdinal("lotno")).Trim();
                        }
                        return lotInfo;
                    }
                }
            }
        }

        /// <summary>
        /// ロット情報[TnLot]取得
        /// </summary>
        /// <param name="lotNO">ロット番号</param>
        /// <returns>ARMSロット情報</returns>
        public static ARMSLotInfo GetARMSLotInfo(int lineCD, string lotNO)
        {
            ARMSLotInfo lotInfo = new ARMSLotInfo();

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT lotno, typecd, isfullsizeinspection, ismappinginspection, ischangepointlot 
                                    FROM tnlot WITH(NOLOCK)
                                    WHERE lotno = @LotNO ";

                    if (lotNO.Contains("_#"))
                    {
                        lotNO = lotNO.Substring(0, lotNO.IndexOf("_#"));
                    }
                    conn.SetParameter("@LotNO", SqlDbType.NVarChar, lotNO);

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        if (!rd.HasRows)
                        {
                            throw new Exception(string.Format(Constant.MessageInfo.Message_55, lotNO));
                        }

                        if (rd.Read())
                        {
                            lotInfo.TypeCD = rd.GetString(rd.GetOrdinal("typecd")).Trim();
                            lotInfo.LotNO = rd.GetString(rd.GetOrdinal("lotno")).Trim();

                            int ordFullsizeinspection = rd.GetOrdinal("isfullsizeinspection");
                            if (!rd.IsDBNull(ordFullsizeinspection))
                            {
                                lotInfo.FullSizeInspFG = Convert.ToBoolean(rd.GetInt32(ordFullsizeinspection));
                            }

                            int ordMappinginspection = rd.GetOrdinal("ismappinginspection");
                            if (!rd.IsDBNull(ordMappinginspection))
                            {
                                lotInfo.MappingInspFG = Convert.ToBoolean(rd.GetInt32(ordMappinginspection));
                            }

                            int ordChangepointlotFG = rd.GetOrdinal("ischangepointlot");
                            if (!rd.IsDBNull(ordChangepointlotFG))
                            {
                                lotInfo.ChangepointlotFG = Convert.ToBoolean(rd.GetInt32(ordChangepointlotFG));
                            }
                        }
                    }
                }

                return lotInfo;
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public static List<ArmsLotInfo> GetARMSLotData(int lineCD, string plantCD, string magazineNO, string targetDT)
        {
            List<ArmsLotInfo> armsLotInfoList = GetARMSLotData(lineCD, plantCD, magazineNO, targetDT, false);

            return armsLotInfoList;
        }

        public static ArmsLotInfo GetLotNo_EckMag(int lineCD, string plantCD, string magazineNO, string targetDT, bool isStartedAndUnCompLot)
        {
            List<ArmsLotInfo> armsLotList = new List<ArmsLotInfo>();
            System.Data.Common.DbDataReader rd = null;

            // 2015/12/26 n.yoshimoto
            // 古川Sコードチェックで問題箇所指摘（遠心沈降マガジン取得時にECKからの呼び出し時の引数で日付指定出来て無いので出鱈目なロットを掴む危険性が指摘された）
            string sql = @" SELECT t.lotno, t.startdt, t.enddt,  t.inmag, t.procno
										FROM TnTran AS t WITH(nolock)
										INNER JOIN TmMachine AS m WITH(nolock) ON t.macno = m.macno 
										WHERE (t.delfg = 0) AND m.plantcd = @PlantCD AND t.inmag = @INMAG ";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {
                if (isStartedAndUnCompLot)
                {
                    //sql += @" AND t.startdt is not NULL And t.outmag = '' ";
                    sql += @" AND t.startdt IS NOT NULL AND t.outmag = '' AND t.enddt IS NULL 
								ORDER BY t.startdt DESC ";
                }
                else
                {
                    sql += @" AND ( (t.startdt <= @START2ENDDT AND @START2ENDDT <= t.enddt) OR (t.startdt <= @START2ENDDT AND t.enddt IS NULL) ) ";
                }

                conn.SetParameter("@PlantCD", SqlDbType.NVarChar, plantCD);
                conn.SetParameter("@INMAG", SqlDbType.NVarChar, magazineNO);
                conn.SetParameter("@START2ENDDT", SqlDbType.DateTime, targetDT);

                using (rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        ArmsLotInfo armsLotInfo = new ArmsLotInfo();

                        int ordStartdt = rd.GetOrdinal("startdt");
                        if (!rd.IsDBNull(ordStartdt))
                        {
                            armsLotInfo.StartDT = rd.GetDateTime(ordStartdt).ToString("yyyy/MM/dd HH:mm:ss.fff");
                        }
                        else { armsLotInfo.StartDT = ""; }

                        int ordEnddt = rd.GetOrdinal("enddt");
                        if (!rd.IsDBNull(ordEnddt))
                        {
                            armsLotInfo.EndDT = rd.GetDateTime(ordEnddt).ToString("yyyy/MM/dd HH:mm:ss.fff");
                        }
                        else { armsLotInfo.EndDT = ""; }

                        string lotNO = rd.GetString(rd.GetOrdinal("lotno")).Trim();
                        if (lotNO.Contains("_#"))
                        {
                            lotNO = lotNO.Substring(0, lotNO.IndexOf("_#"));
                        }
                        armsLotInfo.LotNO = lotNO;

                        armsLotInfo.TypeCD = ConnectDB.GetARMSLotType(lineCD, armsLotInfo.LotNO);
                        armsLotInfo.InMag = rd.GetString(rd.GetOrdinal("inmag")).Trim();

                        armsLotInfo.ProcNO = rd.GetInt64(rd.GetOrdinal("procno"));

                        armsLotList.Add(armsLotInfo);
                    }
                }
            }

            if (armsLotList.Count > 0)
            {
                return armsLotList[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 稼動履歴データ[TnTran]取得
        /// </summary>
        /// <param name="plantCD">設備NO</param>
        /// <param name="magazineNO">マガジンNO</param>
        /// <param name="targetDT">対象日時</param>
        /// <returns>稼動履歴データ</returns>
        public static List<ArmsLotInfo> GetARMSLotData(int lineCD, string plantCD, string magazineNO, string targetDT, bool isStartedAndUnCompLot)
        {
            List<ArmsLotInfo> armsLotList = new List<ArmsLotInfo>();
            System.Data.Common.DbDataReader rd = null;

            // 2015/12/26 n.yoshimoto
            // 古川Sコードチェックで問題箇所指摘（遠心沈降マガジン取得時にECKからの呼び出し時の引数で日付指定出来て無いので出鱈目なロットを掴む危険性が指摘された）
            //			string sql = @" SELECT t.lotno, t.startdt, t.enddt,  t.inmag, t.procno
            //							FROM TnTran AS t WITH(nolock)
            //							INNER JOIN TmMachine AS m WITH(nolock) ON t.macno = m.macno ";

            //			if (!string.IsNullOrEmpty(magazineNO) && isECKMag == false)
            //			{
            //				sql += @" INNER JOIN TnMag WITH(nolock) ON t.lotno = TnMag.lotno AND t.inmag = TnMag.magno ";
            //			}

            //			sql += @" WHERE (t.delfg = 0) ";

      //      string sql = @" SELECT t.lotno, t.startdt, t.enddt,  t.inmag, t.procno 
      //                  FROM TnTran AS t WITH(nolock) 
      //                  INNER JOIN TmMachine AS m WITH(nolock) ON t.macno = m.macno 
      //                  INNER JOIN TnMag WITH(nolock) ON (t.lotno = TnMag.lotno OR t.lotno = TnMag.lotno + '_#1' OR t.lotno = TnMag.lotno + '_#2') 
						//AND t.inmag = TnMag.magno 
      //                  WHERE (t.delfg = 0) and TnMag.newfg = 1 ";

            // QRプレートを使った時のマガジンNo (TnMag.magno =「Mxxxxx_#2」)に対応
            string sql = @" SELECT t.lotno, t.startdt, t.enddt,  t.inmag, t.procno 
                        FROM TnTran AS t WITH(nolock) 
                        INNER JOIN TmMachine AS m WITH(nolock) ON t.macno = m.macno 
                        INNER JOIN TnMag WITH(nolock) ON (t.lotno = TnMag.lotno OR t.lotno = TnMag.lotno + '_#1' OR t.lotno = TnMag.lotno + '_#2') 
						AND (t.inmag = TnMag.magno OR t.inmag + '_#1' = TnMag.magno OR t.inmag + '_#2' = TnMag.magno)
                        WHERE (t.delfg = 0) and TnMag.newfg = 1 ";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {
                if (!string.IsNullOrEmpty(plantCD))
                {
                    sql += " AND m.plantcd = @PlantCD ";
                    conn.SetParameter("@PlantCD", SqlDbType.NVarChar, plantCD);
                }
#if Debug
                if (!string.IsNullOrEmpty(magazineNO))
                {
                    sql += " AND t.inmag = @INMAG ";
                    conn.SetParameter("@INMAG", SqlDbType.NVarChar, magazineNO);
                }
                if (!string.IsNullOrEmpty(magazineNO) && string.IsNullOrEmpty(targetDT))
                {
                    sql += " AND t.enddt IS NULL ";
                }
#else
                if (!string.IsNullOrEmpty(magazineNO))
                {
					sql += " AND t.inmag = @INMAG AND t.enddt IS NULL ";

					// 2015/12/26 n.yoshimoto
					// 遠心沈降マガジンは本関数からは取得しない様に修正するので、下記はコメントアウト

					//if (isECKMag == false)
					//{
					//	sql += "AND TnMag.newfg = 1 ";
					//}

                    conn.SetParameter("@INMAG", SqlDbType.NVarChar, magazineNO);
                }
#endif
                if (!string.IsNullOrEmpty(targetDT))
                {
                    sql += @" AND
                          (
                            (t.startdt <= @START2ENDDT AND @START2ENDDT <= t.enddt) 
							OR (t.startdt <= @START2ENDDT AND t.enddt IS NULL) 
						  ) ";

                    conn.SetParameter("@START2ENDDT", SqlDbType.DateTime, targetDT);
                }
                else if (string.IsNullOrEmpty(targetDT) && isStartedAndUnCompLot)
                {
                    sql += @" AND t.startdt is not NULL And t.outmag = '' ";
                }

                using (rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        ArmsLotInfo armsLotInfo = new ArmsLotInfo();

                        int ordStartdt = rd.GetOrdinal("startdt");
                        if (!rd.IsDBNull(ordStartdt))
                        {
                            armsLotInfo.StartDT = rd.GetDateTime(ordStartdt).ToString("yyyy/MM/dd HH:mm:ss.fff");
                        }
                        else { armsLotInfo.StartDT = ""; }

                        int ordEnddt = rd.GetOrdinal("enddt");
                        if (!rd.IsDBNull(ordEnddt))
                        {
                            armsLotInfo.EndDT = rd.GetDateTime(ordEnddt).ToString("yyyy/MM/dd HH:mm:ss.fff");
                        }
                        else { armsLotInfo.EndDT = ""; }

                        string lotNO = rd.GetString(rd.GetOrdinal("lotno")).Trim();
                        if (lotNO.Contains("_#"))
                        {
                            lotNO = lotNO.Substring(0, lotNO.IndexOf("_#"));
                        }
                        armsLotInfo.LotNO = lotNO;

                        armsLotInfo.TypeCD = ConnectDB.GetARMSLotType(lineCD, armsLotInfo.LotNO);
                        armsLotInfo.InMag = rd.GetString(rd.GetOrdinal("inmag")).Trim();

                        armsLotInfo.ProcNO = rd.GetInt64(rd.GetOrdinal("procno"));

                        armsLotList.Add(armsLotInfo);
                    }
                }
            }

            // 2015/12/26 n.yoshimoto
            // 遠心沈降マガジンは本関数からは取得しない様に修正するので、下記はコメントアウト

            //if ((armsLotList == null || armsLotList.Count == 0) && isECKMag == false)
            //{
            //	return GetARMSLotData(lineCD, true, plantCD, magazineNO, targetDT, isStartedAndUnCompLot);
            //}
            //else if((armsLotList == null || armsLotList.Count == 0) && magazineNO.Contains("_E") == false && isECKMag == true)
            //{
            //	return GetARMSLotData(lineCD, true, plantCD, string.Format("{0}_E", magazineNO), targetDT, isStartedAndUnCompLot);
            //}
            //else if ((armsLotList == null || armsLotList.Count == 0) && magazineNO.Contains("_#1") == false && isECKMag == false)
            //{
            //	return GetARMSLotData(lineCD, false, plantCD, string.Format("{0}_#1", magazineNO), targetDT, isStartedAndUnCompLot);
            //}

            return armsLotList;
        }

        public static bool IsDefectEnd(int lineCD, string lotNO, long procNO)
        {
            bool retV = false;

            try
            {
                string sql = @" SELECT t.isdefectend
							FROM TnTran AS t WITH(nolock)
							WHERE (t.delfg = 0) and t.procno = @ProcNO and t.lotno = @LotNO ";

                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
                {
                    conn.SetParameter("@ProcNO", SqlDbType.BigInt, procNO);
                    conn.SetParameter("@LotNO", SqlDbType.NVarChar, lotNO);

                    System.Data.Common.DbDataReader rd = null;

                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            int ordIsDefectEnd = rd.GetOrdinal("isdefectend");

                            retV = Convert.ToBoolean(rd.GetInt32(ordIsDefectEnd));
                        }
                    }
                }

                return retV;
            }
            catch (Exception err)
            {
                throw new Exception(string.Format("Message：{0}\r\nStackTrace：{1}", err.Message, err.StackTrace));
            }
        }

        public static void UpdateIsDefectEnd(int lineCD, string lotNO, long procNO, bool value)
        {
            try
            {
                string sql = @" UPDATE TnTran SET isdefectend = @IsDefectEnd
                                    WHERE lotno = @LotNO and procno = @ProcNO ";

                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
                {
                    conn.SetParameter("@ProcNO", SqlDbType.BigInt, procNO);
                    conn.SetParameter("@LotNO", SqlDbType.NVarChar, lotNO);

                    conn.SetParameter("@IsDefectEnd", SqlDbType.Bit, value);

                    conn.ExecuteNonQuery(sql);
                }
            }
            catch (Exception err)
            {
                throw new Exception(string.Format("Message：{0}\r\nStackTrace：{1}", err.Message, err.StackTrace));
            }
        }

        public static List<ArmsLotInfo> GetARMSLotData(int lineCD, string plantCD, string magazineNO, string targetDT, bool isStartedAndUnCompLot, bool isIncludeUnCompLot)
        {
            return GetARMSLotData(lineCD, plantCD, magazineNO, targetDT, isStartedAndUnCompLot, isIncludeUnCompLot, false);
        }

        public static List<ArmsLotInfo> GetARMSLotData(int lineCD, string plantCD, string magazineNO, string targetDT, bool isStartedAndUnCompLot, bool isIncludeUnCompLot, bool isNewerThanTargetDt)
        {
            return GetARMSLotData(lineCD, plantCD, magazineNO, targetDT, isStartedAndUnCompLot, isIncludeUnCompLot, isNewerThanTargetDt, null, null);
        }

        /// <summary>
        /// 稼動履歴データ[TnTran]取得
        /// </summary>
        /// <param name="plantCD">設備NO</param>
        /// <param name="magazineNO">マガジンNO</param>
        /// <param name="targetDT">対象日時</param>
        /// <returns>稼動履歴データ</returns>
        public static List<ArmsLotInfo> GetARMSLotData(int lineCD, string plantCD, string magazineNO, string targetDT, bool isStartedAndUnCompLot, bool isIncludeUnCompLot, bool isNewerThanTargetDt, string lotNo, int? procNo)
        {
            List<ArmsLotInfo> armsLotList = new List<ArmsLotInfo>();
            try
            {

                System.Data.Common.DbDataReader rd = null;

                string sql = @" SELECT t.lotno, t.startdt, t.enddt, t.inmag, t.outmag, t.procno
                        FROM TnTran AS t WITH(nolock)
                        INNER JOIN TmMachine AS m WITH(nolock) ON t.macno = m.macno 
						WHERE (t.delfg = 0)";

                if (!string.IsNullOrEmpty(magazineNO))
                {
                    sql = @" SELECT t.lotno, t.startdt, t.enddt, t.inmag, t.outmag, t.procno
                        FROM TnTran AS t WITH(nolock)
                        INNER JOIN TmMachine AS m WITH(nolock) ON t.macno = m.macno 
						INNER JOIN TnMag WITH(nolock) ON t.lotno = TnMag.lotno AND t.inmag = TnMag.magno 
						WHERE (t.delfg = 0)
							AND t.inmag = @INMAG AND t.enddt IS NULL AND TnMag.newfg = 1";
                }


                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
                {
                    if (string.IsNullOrEmpty(lotNo) == false)
                    {
                        sql += " AND t.lotno = @LotNo ";
                        conn.SetParameter("@LotNo", SqlDbType.NVarChar, lotNo);
                    }

                    if (procNo.HasValue)
                    {
                        sql += " AND t.procno = @ProcNo ";
                        conn.SetParameter("@ProcNo", SqlDbType.Int, procNo.Value);
                    }

                    if (!string.IsNullOrEmpty(plantCD))
                    {
                        sql += " AND m.plantcd = @PlantCD ";
                        conn.SetParameter("@PlantCD", SqlDbType.NVarChar, plantCD);
                    }

                    if (isNewerThanTargetDt)
                    {
                        sql += " AND t.startdt > @targetDt AND t.enddt is not NULL ";
                        conn.SetParameter("@targetDT", SqlDbType.DateTime, targetDT);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(magazineNO))
                        {
                            conn.SetParameter("@INMAG", SqlDbType.NVarChar, magazineNO);
                        }

                        if (!string.IsNullOrEmpty(targetDT))
                        {
                            sql += @" AND
                          (
                            (t.startdt <= @START2ENDDT AND @START2ENDDT <= t.enddt) ";

                            if (isIncludeUnCompLot)
                            {
                                sql += @" OR (t.startdt <= @START2ENDDT AND t.enddt IS NULL) ";
                            }

                            sql += @" ) ";

                            conn.SetParameter("@START2ENDDT", SqlDbType.DateTime, targetDT);
                        }
                        else if (string.IsNullOrEmpty(targetDT) && isStartedAndUnCompLot)
                        {
                            sql += @" AND t.startdt is not NULL And t.outmag = '' ";
                        }
                    }

                    using (rd = conn.GetReader(sql))
                    {
                        int ordOutMag = rd.GetOrdinal("outmag");

                        while (rd.Read())
                        {
                            ArmsLotInfo armsLotInfo = new ArmsLotInfo();

                            int ordStartdt = rd.GetOrdinal("startdt");
                            if (!rd.IsDBNull(ordStartdt))
                            {
                                armsLotInfo.StartDT = rd.GetDateTime(ordStartdt).ToString("yyyy/MM/dd HH:mm:ss.fff");
                            }
                            else { armsLotInfo.StartDT = ""; }

                            int ordEnddt = rd.GetOrdinal("enddt");
                            if (!rd.IsDBNull(ordEnddt))
                            {
                                armsLotInfo.EndDT = rd.GetDateTime(ordEnddt).ToString("yyyy/MM/dd HH:mm:ss.fff");
                            }
                            else { armsLotInfo.EndDT = ""; }

                            string lotNO = rd.GetString(rd.GetOrdinal("lotno")).Trim();
                            if (lotNO.Contains("_#"))
                            {
                                lotNO = lotNO.Substring(0, lotNO.IndexOf("_#"));
                            }
                            armsLotInfo.LotNO = lotNO;

                            armsLotInfo.TypeCD = ConnectDB.GetARMSLotType(lineCD, armsLotInfo.LotNO);
                            armsLotInfo.InMag = rd.GetString(rd.GetOrdinal("inmag")).Trim();
                            armsLotInfo.OutMag = rd.GetString(ordOutMag).Trim();
                            armsLotInfo.ProcNO = rd.GetInt64(rd.GetOrdinal("procno"));

                            armsLotList.Add(armsLotInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string sMessage = "[インライン番号]" + lineCD + "\r\n" +
                            ex.Message + "☆PlantCD=[" + plantCD + "]/" + "start2EndDt=[" + targetDT + "]で例外が発生しました。";

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage + "\r\n" + ex.StackTrace);

                armsLotList = null;
            }

            // 検索条件に合う実績がなかった
            if (armsLotList == null || armsLotList.Count == 0)
            {
#if UNIT_TEST
#else
                DateTime s2eDt;
                if (DateTime.TryParse(targetDT, out s2eDt))
                {
                    TimeSpan ts = DateTime.Now - s2eDt;
                    if (ts.TotalSeconds % (300) < 10) //ﾛｸﾞ数が多いので300秒おきに10秒間の出力タイミングのみでログ出力する
                    {
                        string sMessage = string.Format(Constant.MessageInfo.Message_24, plantCD, targetDT);
                        //sMessage = "投入されていないか時刻がずれています。" + plantCD + "/" + dtStart2EndDt;
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                    }
                }
#endif
            }

            // マガジンロット情報取得
            return armsLotList;
        }


        /// <summary>
        /// 稼動履歴データ[TnCutBlend](ダイサー)取得
        /// </summary>
        /// <param name="sEquiNo">設備NO</param>
        /// <param name="magazineNO">マガジンNO</param>
        /// <returns>ロット情報</returns>
        public static ArmsLotInfo GetInlineMagazineLotDC(int lineCD, string sEquiNo, string magazineNO)
        {
            int nMacNo = ConnectDB.GetMacNo(sEquiNo, lineCD);//設備番号(S*****)をmacnoに変換

            // パッケージ化DB情報
            ArmsLotInfo rtnArmsLotInfo = null;

            string sql = @"SELECT lotno,startdt,enddt
                                FROM TnCutBlend t With(NOLOCK)
                                Where 
                                t.magno=@MAGAZINENO AND
                                t.macno=@MACNO AND
                                t.enddt is null AND
                                t.delfg =0 option(MAXDOP 1)";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {
                System.Data.Common.DbDataReader rd = null;
                try
                {
                    conn.SetParameter("@MAGAZINENO", SqlDbType.Char, magazineNO);
                    conn.SetParameter("@MACNO", SqlDbType.Char, nMacNo);

                    using (rd = conn.GetReader(sql))
                    {

                        while (rd.Read())
                        {
                            ArmsLotInfo o = new ArmsLotInfo();
                            o.LotNO = Convert.ToString(rd["lotno"]).Trim();
                            try
                            {
                                o.StartDT = Convert.ToString(rd["startdt"]);
                            }
                            catch
                            {
                                o.StartDT = "";
                            }
                            try
                            {
                                o.EndDT = Convert.ToString(rd["enddt"]);
                            }
                            catch
                            {
                                o.EndDT = "";
                            }

                            o.TypeCD = ConnectDB.GetARMSLotType(lineCD, o.LotNO);

                            rtnArmsLotInfo = o;
                        }
                    }
                }
                catch (Exception ex)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.ToString());
                    throw ex;
                }

            }
            return rtnArmsLotInfo;
        }

        /// <summary>
        /// 作業順序データ[TmWorkFrow]取得
        /// </summary>
        /// <param name="typeCD">製品型番</param>
        /// <param name="procNO">作業NO</param>
        /// <returns>作業順序データ</returns>
        public static List<WorkFrowInfo> GetWorkFrowData(int lineCD, string typeCD, long procNO)
        {
            List<WorkFrowInfo> workFrowList = new List<WorkFrowInfo>();

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT typecd, workorder, procno, ordermove, magdevidestatus
                       FROM TmWorkFlow WITH (nolock)
                       WHERE (delfg = 0) AND (typecd = @TypeCD) ";

                conn.SetParameter("@TypeCD", SqlDbType.NVarChar, typeCD);

                if (procNO != long.MinValue)
                {
                    sql += " AND procno = @ProcNO ";
                    conn.SetParameter("@ProcNO", SqlDbType.BigInt, procNO);
                }

                using (DbDataReader rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        WorkFrowInfo workFrowInfo = new WorkFrowInfo();
                        workFrowInfo.TypeCD = rd.GetString(rd.GetOrdinal("typecd"));
                        workFrowInfo.WorkOrder = rd.GetInt64(rd.GetOrdinal("workorder"));
                        workFrowInfo.ProcNO = rd.GetInt64(rd.GetOrdinal("procno"));
                        workFrowInfo.OrderMove = rd.GetInt64(rd.GetOrdinal("ordermove"));
                        workFrowInfo.MagDevideStatus = rd.GetInt32(rd.GetOrdinal("magdevidestatus"));
                        workFrowList.Add(workFrowInfo);
                    }
                }
            }

            return workFrowList;
        }
        //public static List<WorkFrowInfo> GetWorkFrowData(string typeCD) 
        //{
        //    return GetWorkFrowData(typeCD, long.MinValue);
        //}

        /// <summary>
        /// TnLot情報(ARMS)マッピングFGを変更する
        /// </summary>
        public static void UpdateARMSLotInfo_Mapping(int lineCD, string lotNO, bool mappingFG)
        {
            try
            {
                if (string.IsNullOrEmpty(lotNO))
                {
                    throw new Exception(Constant.MessageInfo.Message_56);
                }
#if Debug
                using (DBConnect conn = DBConnect.CreateInstance(getConnStringForDebug(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
#else
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
#endif
                {
                    string sql = @" UPDATE tnlot SET ismappinginspection = @MappingFG
                                    WHERE lotno = @LotNO ";

                    if (lotNO.Contains("_#"))
                    {
                        lotNO = lotNO.Substring(0, lotNO.IndexOf("_#"));
                    }
                    conn.SetParameter("@LotNO", SqlDbType.NVarChar, lotNO);
                    conn.SetParameter("@MappingFG", SqlDbType.Int, Convert.ToInt16(mappingFG));

                    conn.ExecuteNonQuery(sql);
                }
            }
            catch (Exception err)
            {
                throw;
            }
        }

        /// <summary>
        /// 外観検査機入った(1)、入っていない(2)、ｴﾗｰ(-1)を教えてくれる
        /// </summary>
        /// <param name="sLot"></param>
        /// <returns></returns>
        public static int GetLotCharInfo(int lineCD, string lotno, int procno)
        {
            int nrtn = 0;
            string sCheck = "";
            System.Data.Common.DbDataReader rd = null;

            string sql = @" SELECT t.lotno FROM TnInspection t With(NOLOCK) WHERE t.lotno = @LOTNO AND t.procno = @procno option(MAXDOP 1)";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {
                conn.SetParameter("@procno", SqlDbType.BigInt, procno);

                try
                {
                    if (lotno.Contains("_#"))
                    {
                        lotno = lotno.Substring(0, lotno.IndexOf("_#"));
                    }
                    conn.SetParameter("@LOTNO", SqlDbType.Char, lotno);

                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            sCheck = Convert.ToString(rd["lotno"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    nrtn = -1;
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
                    throw ex;
                }
            }

            if (nrtn == -1)
            {
                return nrtn;
            }

            if (sCheck == "")
            {
                nrtn = 2;//sCheckにLotが入ってなければ、外観検査機を通っていない=マッピングを行わない。
            }
            else
            {
                nrtn = 1;//sCheckにLotが入っていれば、外観検査機を通った=マッピングを行う。
            }

            return nrtn;
        }

        /// <summary>
        /// 外観検査機の通過確認
        /// </summary>
        /// <param name="lotNO">ロット番号</param>
        /// <param name="procNO">工程CD</param>
        /// <returns>外観検査機通過フラグ</returns>
        public static bool IsThroughAI(int lineCD, string lotNO, int procNO)
        {
            System.Data.Common.DbDataReader rd = null;

            try
            {
                string sql = @" SELECT t.lotno
                                FROM TnInspection t With(NOLOCK)
                                WHERE t.lotno = @LotNO AND t.procno = @ProcNO ";

                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
                {
                    conn.SetParameter("@ProcNO", SqlDbType.BigInt, procNO);

                    if (lotNO.Contains("_#"))
                    {
                        lotNO = lotNO.Substring(0, lotNO.IndexOf("_#"));
                    }
                    conn.SetParameter("@LotNO", SqlDbType.Char, lotNO);

                    using (rd = conn.GetReader(sql))
                    {
                        if (rd.HasRows)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw;
            }
        }

        /// <summary>
        /// 現在流動中の作業CDを取得
        /// </summary>
        /// <param name="sLot">ロットNO</param>
        /// <returns>作業CD</returns>
        public static string GetCurrentWork(int lineCD, string sLot)
        {
            string retv = string.Empty;

            string sql = @" SELECT p.workcd
                            FROM TnTran AS t WITH (nolock) 
                            INNER JOIN TmProcess AS p WITH (nolock) ON t.procno = p.procno
							INNER JOIN TnMag WITH(nolock) ON t.lotno = TnMag.lotno AND t.inmag = TnMag.magno
                            WHERE (t.lotno = @LOTNO) AND (t.inmag IS NOT NULL) AND (t.enddt IS NULL) AND (TnMag.newfg = 1) 
                            AND (t.delfg = 0) OPTION (MAXDOP 1) ";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {
                System.Data.Common.DbDataReader rd = null;
                conn.SetParameter("@LOTNO", SqlDbType.Char, sLot);

                using (rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        retv = rd["workcd"].ToString().Trim();
                    }
                }
            }

            return retv;
        }

        /// <summary>
        /// 装置NOを取得
        /// </summary>
        /// <param name="sPlancd">設備NO</param>
        /// <returns>装置NO</returns>
        public static int GetMacNo(string sPlancd, int lineCD)
        {
            int nmacno = 0;


            string sql = @" SELECT t.macno FROM TmMachine t With(NOLOCK) WHERE t.plantcd = @PLANTCD AND delfg = 0 AND outline = 0 ";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {
                System.Data.Common.DbDataReader rd = null;
                try
                {
                    conn.SetParameter("@PLANTCD", SqlDbType.Char, sPlancd);


                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            nmacno = Convert.ToInt32(rd["macno"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.ToString());
                    throw ex;
                }

            }
            return nmacno;
        }

        /// <summary>
        /// TODO 旧コードなので整理してGetARMSLotDataに組み込む
        /// </summary>
        /// <param name="procno"></param>
        /// <param name="macno"></param>
        /// <param name="start2EndDt"></param>
        /// <returns></returns>
        public static List<ArmsLotInfo> GetLotNo_EquiTime(int lineCD, string plantCD, string dtStart2EndDt)
        {
            string sMessage = "";
            //long lStart2EndDt = 0;

            List<ArmsLotInfo> rtnArmsLotInfo = new List<ArmsLotInfo>();

            //lStart2EndDt = Convert.ToInt64(sqlite.SerializeDate(dtStart2EndDt));

            //API1 確認OK
            try
            {
                rtnArmsLotInfo = ConnectDB.GetARMSLotData(lineCD, plantCD, "", dtStart2EndDt);
            }
            catch (Exception ex)
            {
                sMessage = "[インライン番号]" + lineCD + "\r\n" +
                            ex.Message + "☆PlantCD=[" + plantCD + "]/" + "start2EndDt=[" + dtStart2EndDt + "]でGetLotNo_EquiTimeが落ちました。";

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                rtnArmsLotInfo = null;
            }

            // 検索条件に合う実績がなかった
            if (rtnArmsLotInfo == null || rtnArmsLotInfo.Count == 0)
            {
                DateTime s2eDt;
                if (DateTime.TryParse(dtStart2EndDt, out s2eDt))
                {
                    TimeSpan ts = DateTime.Now - s2eDt;
                    if (ts.TotalSeconds % (300) < 10) //ﾛｸﾞ数が多いので300秒おきに10秒間の出力タイミングのみでログ出力する
                    {
                        sMessage = string.Format(Constant.MessageInfo.Message_24, plantCD, dtStart2EndDt);
                        //sMessage = "投入されていないか時刻がずれています。" + plantCD + "/" + dtStart2EndDt;
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                    }
                }
            }

            // マガジンロット情報取得
            return rtnArmsLotInfo;
        }

        /// <summary>
        /// TODO 旧コードなので整理してGetARMSLotDataに組み込む
        /// </summary>
        /// <param name="procno"></param>
        /// <param name="macno"></param>
        /// <param name="start2EndDt"></param>
        /// <returns></returns>
        public static List<ArmsLotInfo> GetLotNoSingleInfo(int lineCD, string plantCD, string magazineNO, bool isStartedAndUnCompLot)
        {
            string sMessage = "";
            List<ArmsLotInfo> rtnArmsLotInfo = new List<ArmsLotInfo>();

            try
            {
                rtnArmsLotInfo = ConnectDB.GetARMSLotData(lineCD, plantCD, magazineNO, null, isStartedAndUnCompLot);
            }
            catch (Exception ex)
            {
                sMessage = "[インライン番号]" + lineCD + "\r\n" +
                            ex.Message + "☆PlantCD=[" + plantCD + "]/" + "magazineNO=[" + magazineNO + "]でGetLotNoSingleInfoが落ちました。";

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                rtnArmsLotInfo = null;
            }

            // 検索条件に合う実績がなかった
            if (rtnArmsLotInfo == null || rtnArmsLotInfo.Count == 0)
            {
                sMessage = string.Format(Constant.MessageInfo.Message_130, plantCD, magazineNO, isStartedAndUnCompLot);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
            }

            // マガジンロット情報取得
            return rtnArmsLotInfo;
        }

        //TODO 旧コードなので整理してGetARMSLotDataに組み込む
        ///引数1:設備番号,引数2:時間
        ///戻り値:NppResultsFacilityInfo[] Aとすると
        ///　　　A[0].TypeCd;   //型番
        ///　　　A[0].LotNo     //ロットNO
        ///　　　nullが返った場合は、ログイン異常、情報なしの何れか。
        public static NppResultsFacilityInfo[] GetLotNo_EquiTimeNasca(int lineCD, string plantCD, string start2EndDt)
        {
            LoginInfo loginInfo = new LoginInfo();
            loginInfo.EmployeeID = Constant.APILogin_UID;
            loginInfo.Password = Constant.APILogin_PWD;
            loginInfo.SectionNM = SettingInfo.GetSectionCD();

            ConnectAPI api = new ConnectAPI(loginInfo);
            NppResultsFacilityInfo[] rtnResultsFacilityInfo = null;

            //Nasca公開APIログイン
            string sMessage = "[Login]" + plantCD;
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

            if (!api.Login())           //<--Session Start
            {
                return rtnResultsFacilityInfo;
            }

            try
            {
                rtnResultsFacilityInfo = api.GetInlineMagazineLot(plantCD, start2EndDt);
            }
            catch (Exception ex)
            {
                sMessage = "[インライン番号]" + lineCD + "\r\n" +
                            ex.Message + "☆PlantCD=[" + plantCD + "]/" + "start2EndDt=[" + start2EndDt + "]でGetInlineMagazineLotが落ちました。";

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                rtnResultsFacilityInfo = null;
            }

            //Nasca公開APIログオフ
            api.Logoff();               //-->Session End
            sMessage = "[Logoff]";
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

            // 検索条件に合う実績がなかった
            if (rtnResultsFacilityInfo == null)
            {
                DateTime s2eDt;
                if (DateTime.TryParse(start2EndDt, out s2eDt))
                {
                    TimeSpan ts = DateTime.Now - s2eDt;
                    if (ts.TotalSeconds % (300) < 10) //ﾛｸﾞ数が多いので300秒おきに10秒間の出力タイミングのみでログ出力する
                    {
                        sMessage = string.Format(Constant.MessageInfo.Message_24, plantCD, start2EndDt);
                        //sMessage = "投入されていないか時刻がずれています。" + plantCD + "/" + start2EndDt;
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                    }
                }
            }

            // マガジンロット情報取得
            return rtnResultsFacilityInfo;
        }

        #endregion

        #region NASCA

        //<--後工程合理化/エラー集計
        public static NascaTranInfo GetNascaTran(int linecd, string sql, string lotno)
        {
            var nascatraninfo = new NascaTranInfo();
            int cnt = 0;
            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.NASCA, linecd), "System.Data.SqlClient", false))
                {
                    System.Data.Common.DbDataReader rd = null;

                    conn.SetParameter("@lotno", SqlDbType.Char, lotno);


                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            nascatraninfo.lotno = Convert.ToString(rd["lot_no"]).Trim();
                            nascatraninfo.startdt = Convert.ToDateTime(rd["start_dt"]);
                            nascatraninfo.compltdt = Convert.ToDateTime(rd["complt_dt"]);
                            cnt++;
                        }
                    }

                    //複数のロットと紐付く場合は、ロットが取得出来ない場合と同じ挙動とする。
                    if (cnt > 1)
                    {
                        nascatraninfo = new NascaTranInfo();
                    }
                    return nascatraninfo;
                }
            }
            catch (Exception err)
            {
                throw;
            }
        }

        //この関数はSTエラー集計専用なのでTmErrConvのprocno列は参照しない。（発行検査作業しか無いため）
        //他装置に転用する場合は注意すること。
        public static int GetFileErrCntInfo(int linecd, string modelnm, int paramno)
        {
            int errno = int.MinValue;

            System.Data.Common.DbDataReader rd = null;

            string sql;
            string sMessageNM = "";

            sql = @"SELECT dbo.TmErrConv.EquiErr_NO
                    FROM dbo.TmPLM WITH (nolock) INNER JOIN
                        dbo.TmErrConv WITH (nolock) ON dbo.TmPLM.Model_NM = dbo.TmErrConv.Equipment_NO AND dbo.TmPLM.QcParam_NO = dbo.TmErrConv.QcParam_NO
                    WHERE(dbo.TmPLM.Model_NM = @modelnm) AND(dbo.TmPLM.QcParam_NO = @paramno)";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, linecd), "System.Data.SqlClient", false))
            {
                conn.SetParameter("@modelnm", SqlDbType.Char, modelnm);
                conn.SetParameter("@paramno", SqlDbType.Int, paramno);

                using (rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        errno = Convert.ToInt32(rd["EquiErr_NO"]);
                    }
                }

            }
            return errno;
        }
        //-->後工程合理化/エラー集計


        /// <summary>
        /// ロットNOから製品型番を取得(NASCA)
        /// </summary>
        /// <param name="lotNO">ロットNO</param>
        public static string GetTypeCD(int lineCD, string lotNO)
        {
            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.NASCA, lineCD), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT RvmMCONV.mtralbase_cd
                            FROM dbo.NttSSHJ AS NttSSHJ WITH (nolock) 
                            INNER JOIN dbo.RvtORDH AS RvtORDH WITH (nolock) ON NttSSHJ.MnfctInst_NO = RvtORDH.mnfctinst_no 
                            INNER JOIN dbo.RvmMCONV AS RvmMCONV WITH (nolock) ON RvtORDH.material_cd = RvmMCONV.material_cd
                            WHERE (NttSSHJ.Del_FG = 0) AND (RvmMCONV.del_fg = '0') AND (RvtORDH.del_fg = '0') 
                             AND (NttSSHJ.lot_no = @LotNO) OPTION (MAXDOP 1) ";

                    conn.SetParameter("@LotNO", SqlDbType.Char, lotNO);

                    object typeCD = conn.ExecuteScalar(sql);
                    if (typeCD == null)
                    {
                        throw new Exception(string.Format(Constant.MessageInfo.Message_65, lotNO));
                    }
                    else
                    {
                        return typeCD.ToString().Trim();
                    }
                }
            }
            catch (Exception err)
            {
                throw;
            }
        }

        /// <summary>
        /// 梱包指図書Noから投入予定ロットのロットNo及びポットNo一覧取得(NASCA)
        /// </summary>
        /// <param name="lotNO">ロットNO</param>
        public static List<InputScheduleInfo> GetInputSchedule(int lineCD, string sashizuno)
        {
            var listInputScheduleInfo = new List<InputScheduleInfo>();
            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.NASCA, lineCD), "System.Data.SqlClient", false))
                {
                    System.Data.Common.DbDataReader rd = null;

                    string sql = @" SELECT dbo.NttLTNA.NascaLot_NO, dbo.NttLTRS.Pot_NO
                                    FROM   dbo.RvtORDH INNER JOIN
                                           dbo.RvtORDMAT ON dbo.RvtORDH.mnfctinst_no = dbo.RvtORDMAT.mnfctinst_no INNER JOIN
                                           dbo.NttLTRS ON dbo.RvtORDMAT.lot_no = dbo.NttLTRS.RootsLot_NO AND dbo.RvtORDMAT.mtralitem_cd = dbo.NttLTRS.Material_CD INNER JOIN 
                                           dbo.NttLTNA ON dbo.NttLTRS.NascaLot_ID = dbo.NttLTNA.NascaLot_ID
                                    WHERE  (dbo.RvtORDH.mnfctinst_no = @sashizuno) AND (dbo.RvtORDMAT.del_fg = '0') AND (dbo.RvtORDH.del_fg = '0')";

                    conn.SetParameter("@sashizuno", SqlDbType.Char, sashizuno);


                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            InputScheduleInfo inputscheduleinfo = new InputScheduleInfo();

                            inputscheduleinfo.lotno = Convert.ToString(rd["NascaLot_NO"]).Trim();
                            inputscheduleinfo.potno = Convert.ToInt32(rd["Pot_NO"]);
                            listInputScheduleInfo.Add(inputscheduleinfo);
                        }
                    }

                    return listInputScheduleInfo;
                }
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public static NascaLotCharInfo GetNascaLotCharInfo(int linecd, string lotno, string typecd, string lotcharcd)
        {
            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.NASCA, linecd), "System.Data.SqlClient", false))
                {
                    System.Data.Common.DbDataReader rd = null;

                    conn.SetParameter("@lotno", SqlDbType.Char, lotno);

                    string sql = @"SELECT NttLTTS.LotChar_VAL
                            FROM dbo.NttLTNA AS NttLTNA WITH (nolock) 
                            INNER JOIN dbo.NttLTTS AS NttLTTS WITH (nolock) ON NttLTNA.NascaLot_ID = NttLTTS.NascaLot_ID
                            WHERE (NttLTTS.LotChar_CD = @LOTCHARCD) 
                            AND (NttLTNA.NascaLot_NO = @LOTNO)";

                    conn.SetParameter("@LOTNO", System.Data.SqlDbType.VarChar, lotno);
                    conn.SetParameter("@LOTCHARCD", System.Data.SqlDbType.Char, lotcharcd);
                    if (string.IsNullOrWhiteSpace(typecd) == false)
                    {
                        sql += @" AND (NttLTNA.Type_CD = @TYPECD) ";
                        conn.SetParameter("@TYPECD", System.Data.SqlDbType.VarChar, typecd);
                    }
                    sql += " OPTION(MAXDOP 1) ";
                    
                    NascaLotCharInfo nascalotcharinfo = new NascaLotCharInfo();
                    nascalotcharinfo.LotCharCd = lotcharcd;
                    nascalotcharinfo.LotCharVal = (conn.ExecuteScalar(sql) ?? "").ToString().Trim();                            
                 
                    return nascalotcharinfo;
                }
            }
            catch (Exception err)
            {
                throw;
            }            
        }

        #endregion

        #region 前ｼｽﾃﾑから転用

        /// <summary>最新の連番を取得</summary>
        /// <returns>連番ID</returns>
        public int GetLOGNewSeqNO()
        {
            string sql = " SELECT MAX(Seq_NO) AS Max_CT FROM TnLOG ";

            try
            {
                object maxCT = this.connection.ExecuteScalar(sql);
                return Convert.ToInt32(maxCT) + 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
                return 0;
            }
        }

        /// <summary>
        /// DB/WBのデータ保存先リスト取得
        /// </summary>
        /// <param name="nLinecd"></param>
        /// <returns></returns>
        public static List<UnchainInfo> GetListUnchainInfo(int nLinecd)
        {
            List<UnchainInfo> ListUnchainInfo = new List<UnchainInfo>();

            string sql = @"SELECT [Equipment_NO],[InputFolder_NM]
                                FROM [TmLSET] With(NOLOCK)
                                Where   Inline_CD=@INLINECD AND 
                                        Process_CD in (1,5) AND
                                        Del_FG = 0
                                option(MAXDOP 1)";

            SettingInfo settingInfo = SettingInfo.GetSingleton();

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(settingInfo.HonbanServer, settingInfo.HonbanDB), "System.Data.SqlClient", false))
            {
                System.Data.Common.DbDataReader rd = null;
                try
                {
                    conn.SetParameter("@INLINECD", SqlDbType.Int, nLinecd);

                    using (rd = conn.GetReader(sql))
                    {

                        while (rd.Read())
                        {
                            UnchainInfo unchainInfo = new UnchainInfo();
                            unchainInfo.Equi = Convert.ToString(rd["Equipment_NO"]);
                            unchainInfo.Dir = Convert.ToString(rd["InputFolder_NM"]);
                            ListUnchainInfo.Add(unchainInfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
                    throw ex;
                }

            }

            return ListUnchainInfo;
        }

        /// <summary>
        /// [パラメータ名][回収タイミング]を引数に入れると、TvPRMから"パラメータ連番""管理方法"を取得して戻す
        /// </summary>
        /// <param name="sEquipmentNO"></param>
        /// <param name="sTimingNM"></param>
        /// <param name="sParameterNM"></param>
        /// <returns>sQcParamNO</returns>
        public static ParamInfo GetTvPRM_QcParamNO(int lineCD, int qcParamNO, string sTimingNM)
        {
            System.Data.Common.DbDataReader rd = null;
            ParamInfo wParamInfo = new ParamInfo();

            //string BaseSql = "SELECT QcParam_NO,Parameter_NM,Manage_NM FROM TvPRM WITH(NOLOCK) WHERE Parameter_NM='{0}' AND Timing_NM='{1}'";
            //string sql = string.Format(BaseSql, sParameterNM, sTimingNM);
            string sql = @" SELECT QcParam_NO,Parameter_NM,Manage_NM,Total_KB, ChangeUnit_VAL
                            FROM TvPRM WITH(NOLOCK) 
                            WHERE QcParam_NO=@QcParamNO AND Timing_NM=@TimingNM ";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNO);
                conn.SetParameter("@TimingNM", SqlDbType.VarChar, sTimingNM);

                try
                {
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            wParamInfo.nQcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            wParamInfo.sManageNM = Convert.ToString(rd["Manage_NM"]).Trim();
                            wParamInfo.sParamNM = Convert.ToString(rd["Parameter_NM"]).Trim();
                            wParamInfo.sTotalKB = Convert.ToString(rd["Total_KB"]).Trim();
                            wParamInfo.ChangeUnitVAL = Convert.ToString(rd["ChangeUnit_VAL"]).Trim();
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = lineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);

                }
            }
            return wParamInfo;
        }

        /// <summary>
        /// [パラメータ名][回収タイミング]を引数に入れると、TvPRMから"パラメータ連番""管理方法"を取得して戻す
        /// </summary>
        /// <param name="sEquipmentNO"></param>
        /// <param name="sTimingNM"></param>
        /// <param name="sParameterNM"></param>
        /// <returns>sQcParamNO</returns>
        public static ParamInfo GetTvPRM_QcParamNO(int lineCD, string chipNM, string qcParamNM, string sTimingNM)
        {
            System.Data.Common.DbDataReader rd = null;
            ParamInfo wParamInfo = new ParamInfo();

            //string BaseSql = "SELECT QcParam_NO,Parameter_NM,Manage_NM FROM TvPRM WITH(NOLOCK) WHERE Parameter_NM='{0}' AND Timing_NM='{1}'";
            //string sql = string.Format(BaseSql, sParameterNM, sTimingNM);
            string sql = @" SELECT QcParam_NO,Parameter_NM,Manage_NM,Total_KB,ChangeUnit_VAL
                            FROM TvPRM WITH(NOLOCK) 
                            WHERE Parameter_NM=@ParameterNM AND Timing_NM=@TimingNM ";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                conn.SetParameter("@ParameterNM", SqlDbType.VarChar, qcParamNM);
                conn.SetParameter("@TimingNM", SqlDbType.VarChar, sTimingNM);

                if (!string.IsNullOrEmpty(chipNM))
                {
                    sql += @" AND Chip_NM = @ChipNM ";
                    conn.SetParameter("@ChipNM", SqlDbType.VarChar, chipNM);
                }


                try
                {
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            wParamInfo.nQcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            wParamInfo.sManageNM = Convert.ToString(rd["Manage_NM"]).Trim();
                            wParamInfo.sParamNM = Convert.ToString(rd["Parameter_NM"]).Trim();
                            wParamInfo.sTotalKB = Convert.ToString(rd["Total_KB"]).Trim();
                            wParamInfo.ChangeUnitVAL = Convert.ToString(rd["ChangeUnit_VAL"]).Trim();
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = lineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);

                }
            }
            return wParamInfo;
        }

        //「インライン番号/設備番号/計測日時/パラメータ管理番号/遡るﾏｶﾞｼﾞﾝ数」を入れると、計測日時より以前で「遡るﾏｶﾞｼﾞﾝ」目のパラメータ値を返す
        public static decimal GetTnLOG_DParam(int nInlineCD, string sEquipmentNO, string dtMeasureDT, int nQcParamNO, int nBeforeMag)
        {
            int nCnt = 0;
            decimal dDParam = decimal.MinValue;

            string sql = string.Format(" SELECT TOP {0} DParameter_VAL FROM TnLOG WITH(NOLOCK) WHERE Inline_CD =@InlineCD AND Equipment_NO=@EquipmentNO AND Measure_DT<@MeasureDT AND QcParam_NO=@QcParamNO ORDER by Measure_DT DESC ",
                nBeforeMag);

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, nInlineCD), "System.Data.SqlClient", false))
            {
                //conn.SetParameter("@RecordNO", SqlDbType.Int, nBeforeMag);
                conn.SetParameter("@InlineCD", SqlDbType.Int, nInlineCD);
                conn.SetParameter("@EquipmentNO", SqlDbType.Char, sEquipmentNO);
                conn.SetParameter("@MeasureDT", SqlDbType.DateTime, dtMeasureDT);
                conn.SetParameter("@QcParamNO", SqlDbType.Int, nQcParamNO);

                using (System.Data.Common.DbDataReader rd = conn.GetReader(sql))
                {
                    while (rd.Read())
                    {
                        nCnt = nCnt + 1;
                        if (nCnt == nBeforeMag)
                        {
                            if (rd["DParameter_VAL"] != DBNull.Value)
                            {
                                dDParam = Convert.ToDecimal(rd["DParameter_VAL"]);
                            }
                        }
                    }
                }
            }

            return dDParam;
        }

        /// <summary>
        /// NG判定を行う。 (TnLOGへの追加・メッセージ表示は一切なし)
        /// エラーの場合、trueを返す
        /// </summary>
        public static bool NGJudge(Plm plmInfo, string sValue)
        {
            bool fErr = false; //ｴﾗｰあるなし

            // パラメータが空の場合、合格とする
            if (sValue == "")
            {
                return false;
            }

            //NG判定してメッセージ入れ込み
            if (plmInfo != null && plmInfo.ParameterVAL == "")//0:数値で、管理限界情報を取得出来ている場合
            {

            }
            else//1:文字列か、管理限界情報を取得出来ていない場合
            {
                if (plmInfo.ParameterVAL.ToUpper() != sValue.ToUpper() && plmInfo.ParameterVAL.ToUpper() != Constant.sOKStrings)
                {
                    fErr = true;
                }
            }
            return fErr;
        }

        /// <summary>
        /// NG判定を行う。 (TnLOGへの追加・メッセージ表示は一切なし)
        /// </summary>
        public static bool NGJudge(Plm plmInfo, decimal dValue)
        {
            bool fErr = false; //ｴﾗｰあるなし

            //// パラメータを数値型に変換
            //dValue = decimal.Round(dValue, 4, MidpointRounding.AwayFromZero);

            //NG判定してメッセージ入れ込み
            if (plmInfo != null && plmInfo.ParameterVAL == "")//0:数値で、管理限界情報を取得出来ている場合
            {
                // MAX-MIN
                if (plmInfo.ManageNM == Constant.sMAXMIN)
                {
                    if (plmInfo.ParameterMAX < dValue)
                    {
                        fErr = true;
                    }
                    else if (plmInfo.ParameterMIN > dValue)
                    {
                        fErr = true;
                    }
                }
                // MAX
                else if (plmInfo.ManageNM == Constant.sMAX)
                {
                    if (plmInfo.ParameterMAX < dValue)
                    {
                        fErr = true;
                    }
                }
            }
            else//1:文字列か、管理限界情報を取得出来ていない場合
            {
                
            }
            return fErr;
        }


        //<--NASCA不良のエラー判定実施
        public static bool InsertTnLOG(LSETInfo lsetInfo, int LineCD, int QcParamNO, string typeCD, string magazineNo, string lotNo, string sValue, string dtMeasureDT, ref List<ErrMessageInfo> errMessageList)
        {
            MagInfo magInfo = new MagInfo();
            magInfo.sNascaLotNO = lotNo;
            magInfo.sMagazineNO = magazineNo;
            magInfo.sMaterialCD = typeCD;
            //NASCA不良を取得するためにはtrue
            Plm plmInfo = Plm.GetData(LineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, QcParamNO, true, lsetInfo.ChipNM);
            if (plmInfo == null)
            {
                plmInfo = Plm.GetData(LineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, QcParamNO, true, null);
            }

            return InsertTnLOG(lsetInfo, plmInfo, magInfo, sValue, dtMeasureDT, ref errMessageList);
        }
        //-->NASCA不良のエラー判定実施

        /// <summary>
        /// NG判定を行いエラー出力する・TnLOGへレコード追加する。
        /// </summary>
        /// <param name="sQcParamNO"></param>
        /// <param name="sLotNO"></param>
        /// <param name="sParameterVAL"></param>
        /// <param name="dtLastUpdDT"></param>
        public static bool InsertTnLOG(LSETInfo EquiInfo, Plm plmInfo, MagInfo MagInfo, string sValue, string dtMeasureDT, ref List<ErrMessageInfo> errMessageList)
        {
            int nSeqNO = 0;
            string sql;
            string sMessage = "";
            string sMessageInnerLimit = "";
            Decimal? dValue = 0;
            bool fErr = false; //ｴﾗｰあるなし
            bool fErrInnerLimit = false;

            //Inline_CD,Equipment_NO,NascaLot_NO,QcParam_NOが同じものがあれば、Delete
            //→メンテの為、一旦マガジン取り出し(ファイル1出力)、再投入した(ファイル2出力)場合
            //  同一Lotで2ファイル出力された状態となる。この場合、最初のデータは削除し、後のファイルのみ登録する。=Delete&Insert
            //<-- Package 一時的にコメント
            //DeleteSameLot(EquiInfo, ParamInfo, MagInfo, dtMeasureDT);
            //--> Package 一時的にコメント

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, EquiInfo.InlineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    sql = @" SELECT Measure_DT FROM TnLOG WITH(NOLOCK) 
								 WHERE Inline_CD = @InlineCD AND Equipment_NO = @EquipmentNO AND Measure_DT = @MeasureDT AND Magazine_NO = @MagazineNO
								 AND NascaLot_NO = @NascaLotNO AND QcParam_NO= @QcParamNO AND DParameter_VAL = @DParamVal AND SParameter_VAL = @SParamVal ";

                    conn.SetParameter("@InlineCD", SqlDbType.Int, EquiInfo.InlineCD);
                    conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, EquiInfo.EquipmentNO);
                    conn.SetParameter("@MeasureDT", SqlDbType.DateTime, dtMeasureDT);

                    if (MagInfo != null && string.IsNullOrEmpty(MagInfo.sMagazineNO) == false)
                    {
                        conn.SetParameter("@MagazineNO", SqlDbType.NVarChar, MagInfo.sMagazineNO);
                    }
                    else
                    {
                        conn.SetParameter("@MagazineNO", SqlDbType.NVarChar, string.Empty);
                    }

                    if (MagInfo != null && string.IsNullOrEmpty(MagInfo.sNascaLotNO) == false)
                    {
                        conn.SetParameter("@NascaLotNO", SqlDbType.NVarChar, MagInfo.sNascaLotNO);
                    }
                    else
                    {
                        conn.SetParameter("@NascaLotNO", SqlDbType.NVarChar, string.Empty);
                    }

                    conn.SetParameter("@QcParamNO", SqlDbType.Int, plmInfo.QcParamNO);

                    string logMsg;
                    Log.ParameterSet paramSet = Log.GetParameter(plmInfo, sValue, out logMsg);

                    if (!string.IsNullOrEmpty(logMsg))
                    {
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{0} 測定日時:{1} {2}", EquiInfo.EquipmentNO, dtMeasureDT, logMsg));
                    }

                    if (paramSet.DParameterVAL.HasValue)
                    {
                        conn.SetParameter("@DParamVal", SqlDbType.Decimal, paramSet.DParameterVAL);
                    }
                    else
                    {
                        conn.SetParameter("@DParamVal", SqlDbType.Decimal, DBNull.Value);
                    }

                    if (string.IsNullOrEmpty(paramSet.SParameterVAL))
                    {
                        conn.SetParameter("@SParamVal", SqlDbType.NVarChar, DBNull.Value);
                    }
                    else
                    {
                        conn.SetParameter("@SParamVal", SqlDbType.NVarChar, paramSet.SParameterVAL);
                    }

                    dValue = paramSet.DParameterVAL;

                    //NG判定してメッセージ入れ込み
                    if (plmInfo != null && plmInfo.ParameterVAL == "")//0:数値で、管理限界情報を取得出来ている場合
                    {
                        if (Plm.HasDecimalLimit(plmInfo))
                        {
                            if (dValue.HasValue == false)
                            {
                                string nullValErrFormat = string.Format("[{0}/{1}号機][管理番号:{5}/{2}]閾値が設定されていますがﾌｧｲﾙからの取得値がnull値です。Lot={3},Linecd={4}");

                                F01_MachineWatch.sp.PlayLooping();

                                sMessage = string.Format(nullValErrFormat, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);
                                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                                errMessageList.Add(errMessageInfo);

                                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                                return false;
                            }

                            if (plmInfo.ManageNM == Constant.sMAXMIN)
                            {
                                if (plmInfo.ParameterMAX < dValue)
                                {
                                    F01_MachineWatch.sp.PlayLooping();

                                    sMessage = string.Format(Constant.MessageInfo.Message_22, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, "MAX", dValue, plmInfo.ParameterMAX, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);
                                    ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                                    errMessageList.Add(errMessageInfo);

                                    fErr = true;
                                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                                }
                                else if (plmInfo.ParameterMIN > dValue)
                                {
                                    F01_MachineWatch.sp.PlayLooping();

                                    //sMessage = "[{0}/{1}号機/{2}]が管理限界値(MIN)を越えました。\r\n取得値={3},閾値MIN={4}";
                                    sMessage = string.Format(Constant.MessageInfo.Message_22, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, "MIN", dValue, plmInfo.ParameterMIN, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);
                                    ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                                    errMessageList.Add(errMessageInfo);

                                    fErr = true;
                                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                                }
                            }
                            else if (plmInfo.ManageNM.ToUpper() == Constant.sMAX)
                            {
                                if (plmInfo.ParameterMAX < dValue)
                                {
                                    F01_MachineWatch.sp.PlayLooping();

                                    //sMessage = "[{0}/{1}号機/{2}]が管理限界値(MAX)を越えました。\r\n取得値={3},閾値MAX={4}";
                                    sMessage = string.Format(Constant.MessageInfo.Message_22, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, "MAX", dValue, plmInfo.ParameterMAX, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);
                                    ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                                    errMessageList.Add(errMessageInfo);

                                    fErr = true;
                                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                                }
                            }
                        }
                        if (!fErr)
                        {
                            //管理限界値超えが発生していない時は、工程狙い値超えの確認をし、超えていればエラーメッセージを表示
                            sMessageInnerLimit = ParameterInfo.CheckInnerLimit(plmInfo, dValue.ToString(), EquiInfo, MagInfo.sNascaLotNO);
                            if (!string.IsNullOrEmpty(sMessageInnerLimit))
                            {
                                F01_MachineWatch.sp.PlayLooping();

                                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessageInnerLimit, Color.Blue);
                                errMessageList.Add(errMessageInfo);

                                fErrInnerLimit = true;
                            }
                        }
                    }
                    else//1:文字列か、管理限界情報を取得出来ていない場合
                    {
                        if (plmInfo.ParameterVAL.ToUpper() != sValue.ToUpper() && plmInfo.ParameterVAL.ToUpper() != Constant.sOKStrings)
                        {
                            F01_MachineWatch.sp.PlayLooping();
                            //sMessage = "[{0}/{1}号機/{2}]の設定値に誤りがあります。\r\n取得値={3},閾値={4}";
                            sMessage = string.Format(Constant.MessageInfo.Message_23, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, sValue, plmInfo.ParameterVAL, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);
                            ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                            errMessageList.Add(errMessageInfo);

                            fErr = true;
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                        }
                    }

                    if ((fErr || fErrInnerLimit) && SettingInfo.GetBatchModeFG() == false)
                    {
                        if (ErrorCommunicator.IsMustSendToOtherLineError(EquiInfo.InlineCD, plmInfo.QcParamNO))
                        {
                            if (string.IsNullOrEmpty(MagInfo.sNascaLotNO) == true)
                            {
                                ErrMessageInfo errMessageInfo = new ErrMessageInfo(string.Format("【遠隔エラー通知不可】マガジンNO：{0} / パラメータ名：{1} / ロット紐付け不良の為、通知不可", MagInfo.sMagazineNO, plmInfo.ParameterNM), Color.Red);
                                errMessageList.Add(errMessageInfo);
                            }
                            else
                            {
                                string message = string.Empty;
                                if (fErr) { message = sMessage; }
                                else { message = sMessageInnerLimit; }
#if DEBUG
#else
                        
								//（2014.3.14 n.yoshimotoコードレビューにて修正候補浮上)ErrorMessageInfoに入れて、この関数外（自ライン通知時）で他ラインへメッセージを飛ばす仕組みに
								ErrorCommunicator.SendError(ErrorCommunicator.GetContactError(EquiInfo.InlineCD, DateTime.Now, EquiInfo, MagInfo.sNascaLotNO, MagInfo.sMagazineNO, plmInfo.QcParamNO, message));
#endif
                            }
                        }
                    }

                    //文字数多の為、ログ除外 (2015/2/14 n.yoshimoto)
                    //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, EquiInfo.EquipmentNO + "/" + plmInfo.QcParamNO + "/" + dtMeasureDT + "/▲[SQL]/" + sql);
                    //重複確認
                    if (conn.ExecuteScalar(sql) == null)
                    {
                        //重複なければ、INSERT

                        //但し、事前に同じ「インライン番号/設備番号/計測日時/パラメータ管理番号」の数をカウントして、カウント+1をSeq_NOに決定する。
                        nSeqNO = GetTnLOG_SeqNO(EquiInfo.InlineCD, EquiInfo.EquipmentNO, dtMeasureDT, plmInfo.QcParamNO);

                        sql = @" INSERT INTO TnLog(Inline_CD, Equipment_NO, Measure_DT, Seq_NO, QcParam_NO, Material_CD, Magazine_NO, NascaLot_NO, DParameter_VAL, SParameter_VAL, Message_NM, Error_FG, Check_FG, Del_FG, UpdUser_CD, LastUpd_DT)
									 VALUES(@InlineCD, @EquipmentNO, @MeasureDT, @SeqNO , @QcParamNO, @MaterialCD, @MagazineNO, @NascaLotNO, @DParamVal, @SParamVal , @MessageNM, @ErrorFG , @CheckFG, @DelFG, @UpdUserCD, @LastUpdDT)";

                        conn.SetParameter("@SeqNO", SqlDbType.Int, nSeqNO);
                        conn.SetParameter("@MaterialCD", SqlDbType.NVarChar, MagInfo.sMaterialCD);
                        conn.SetParameter("@MessageNM", SqlDbType.NVarChar, sMessage);
                        conn.SetParameter("@ErrorFG", SqlDbType.Bit, Convert.ToInt16(fErr));
                        conn.SetParameter("@CheckFG", SqlDbType.Bit, Constant.bitEmp);
                        conn.SetParameter("@DelFG", SqlDbType.Bit, Constant.bitEmp);
                        conn.SetParameter("@UpdUserCD", SqlDbType.Char, Constant.sUser);
                        conn.SetParameter("@LastUpdDT", SqlDbType.DateTime, DateTime.Now);

#if Debug || TEST
                        //using (DBConnect DebConn = DBConnect.CreateInstance(getConnStringForDebug(Constant.DBConnectGroup.EICSDB, 0), "System.Data.SqlClient", false))
                        //DebConn.ExecuteNonQuery(sql);
#else
                        conn.ExecuteNonQuery(sql);
#endif

                    }
                }
                catch (Exception ex)
                {
                    string funcNm = "InsertTnLOG(LSETInfo, Plm, MagInfo, string, string, ref List<ErrMessageInfo>)";
                    string sMsg = string.Format("lineCD:{0} / plantCD:{1} / funcNm:{2} / QcParamNo:{3} / LotNo:{4} / MagNo:{5} / measureDt:{6} / sValue:{7} / [原因]:{8} / ネットワーク停止<--Start"
                        , EquiInfo.InlineCD, EquiInfo.EquipmentNO, funcNm, plmInfo.QcParamNO, MagInfo.sNascaLotNO, MagInfo.sMagazineNO, dtMeasureDT, sValue, ex.Message);

                    throw new ApplicationException(sMsg, ex);
                }
            }

            return fErr;
        }

        public static void InsertTnLOG(Log log, LSETInfo lsetInfo)
        {
            InsertTnLOG(log, lsetInfo, null);
        }

        public static void InsertTnLOG(Log log, LSETInfo lsetInfo, string resinGroup)
        {
            Plm plmInfo = new Plm();
            MagInfo magInfo = new MagInfo();

            magInfo.sMaterialCD = log.MaterialCD;
            magInfo.sMagazineNO = log.MagazineNO;
            magInfo.sNascaLotNO = log.NascaLotNO;

            //樹脂グループ指定の時だけ閾値取得ルーチンを分岐
            if(string.IsNullOrWhiteSpace(resinGroup) == true)
            {
                plmInfo = Plm.GetData(lsetInfo.InlineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, log.QcParamNO, lsetInfo.EquipmentNO, false, lsetInfo.ChipNM);
            }
            else
            {
                List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);
                List<FILEFMTInfo> fileFmtList = ConnectDB.GetFILEFMTData(null, lsetInfo, false);
                List<FileFmtWithPlm> allFileFmtWithPlmList = FileFmtWithPlm.GetData(lsetInfo, false, plmPerTypeModelChipList, fileFmtList, resinGroup);

                FileFmtWithPlm qcParamData = allFileFmtWithPlmList.Where(w => w.Plm.QcParamNO == log.QcParamNO).FirstOrDefault();

                if(qcParamData != null)
                {
                    plmInfo = qcParamData.Plm;
                }
            }

            string sValue = string.Empty;
            if (log.DParameterVAL.HasValue)
            {
                sValue = log.DParameterVAL.Value.ToString();
            }
            else
            {
                sValue = log.SParameterVAL;
            }

            InsertTnLOG(lsetInfo, plmInfo, magInfo, sValue, log.MeasureDT.Value.ToString("yyyy/MM/dd HH:mm:ss"), log.MessageNM, log.ErrorFG, log.CheckFG);
        }


        public static void InsertTnLOG(LSETInfo lsetInfo, Plm plmInfo, MagInfo magInfo, string sValue, string dtMeasureDT, string sMessage)
        {
            InsertTnLOG(lsetInfo, plmInfo, magInfo, sValue, dtMeasureDT, sMessage, null, null);
        }

        /// <summary>
        /// NG判定を行わず、TnLogへレコード追加する。
        /// </summary>
        /// <param name="sQcParamNO"></param>
        /// <param name="sLotNO"></param>
        /// <param name="sParameterVAL"></param>
        /// <param name="dtLastUpdDT"></param>
		public static void InsertTnLOG(LSETInfo EquiInfo, Plm plmInfo, MagInfo MagInfo, string sValue, string dtMeasureDT, string sMessage, bool? errFg, bool? chkFg)
        {
            int nSeqNO = 0;
            string sql;
            Decimal dValue = 0;

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, EquiInfo.InlineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    sql = @" SELECT Measure_DT FROM TnLOG WITH(NOLOCK) 
										 WHERE Inline_CD = @InlineCD AND Equipment_NO = @EquipmentNO AND Measure_DT = @MeasureDT AND Magazine_NO = @MagazineNO
										 AND NascaLot_NO = @NascaLotNO AND QcParam_NO= @QcParamNO AND DParameter_VAL = @DParamVal AND SParameter_VAL = @SParamVal ";

                    conn.SetParameter("@InlineCD", SqlDbType.Int, EquiInfo.InlineCD);
                    conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, EquiInfo.EquipmentNO);
                    conn.SetParameter("@MeasureDT", SqlDbType.DateTime, dtMeasureDT);
                    conn.SetParameter("@MagazineNO", SqlDbType.NVarChar, MagInfo.sMagazineNO);
                    conn.SetParameter("@NascaLotNO", SqlDbType.NVarChar, MagInfo.sNascaLotNO);
                    conn.SetParameter("@QcParamNO", SqlDbType.Int, plmInfo.QcParamNO);

                    string logMsg;
                    Log.ParameterSet paramSet = Log.GetParameter(plmInfo, sValue, out logMsg);

                    if (!string.IsNullOrEmpty(logMsg))
                    {
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{0} 測定日時:{1} {2}", EquiInfo.EquipmentNO, dtMeasureDT, logMsg));
                    }

                    if (paramSet.DParameterVAL.HasValue)
                    {
                        conn.SetParameter("@DParamVal", SqlDbType.Decimal, paramSet.DParameterVAL);
                    }
                    else
                    {
                        conn.SetParameter("@DParamVal", SqlDbType.Decimal, DBNull.Value);
                    }

                    if (string.IsNullOrEmpty(paramSet.SParameterVAL))
                    {
                        conn.SetParameter("@SParamVal", SqlDbType.NVarChar, DBNull.Value);
                    }
                    else
                    {
                        conn.SetParameter("@SParamVal", SqlDbType.NVarChar, paramSet.SParameterVAL);
                    }

                    //文字数多の為、ログ除外 (2015/2/14 n.yoshimoto)
                    //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, EquiInfo.EquipmentNO + "/" + plmInfo.QcParamNO + "/" + dtMeasureDT + "/▲[SQL]/" + sql);
                    //重複確認
                    if (conn.ExecuteScalar(sql) == null)
                    {
                        //重複なければ、INSERT
                        //但し、事前に同じ「インライン番号/設備番号/計測日時/パラメータ管理番号」の数をカウントして、カウント+1をSeq_NOに決定する。
                        nSeqNO = GetTnLOG_SeqNO(EquiInfo.InlineCD, EquiInfo.EquipmentNO, dtMeasureDT, plmInfo.QcParamNO);

                        sql = @" INSERT INTO TnLog(Inline_CD, Equipment_NO, Measure_DT, Seq_NO, QcParam_NO, Material_CD, Magazine_NO, NascaLot_NO, DParameter_VAL, SParameter_VAL, Message_NM, Error_FG, Check_FG, Del_FG, UpdUser_CD, LastUpd_DT)
									 VALUES(@InlineCD, @EquipmentNO, @MeasureDT, @SeqNO , @QcParamNO, @MaterialCD, @MagazineNO, @NascaLotNO, @DParamVal, @SParamVal , @MessageNM, @ErrorFG , @CheckFG, @DelFG, @UpdUserCD, @LastUpdDT)";

                        conn.SetParameter("@SeqNO", SqlDbType.Int, nSeqNO);
                        conn.SetParameter("@MaterialCD", SqlDbType.NVarChar, MagInfo.sMaterialCD);
                        conn.SetParameter("@MessageNM", SqlDbType.NVarChar, sMessage);

                        if (errFg.HasValue)
                        {
                            conn.SetParameter("@ErrorFG", SqlDbType.Bit, errFg.Value ? 1 : 0);
                        }
                        else
                        {
                            conn.SetParameter("@ErrorFG", SqlDbType.Bit, Constant.bitEmp);
                        }

                        if (chkFg.HasValue)
                        {
                            conn.SetParameter("@CheckFG", SqlDbType.Bit, chkFg.Value ? 1 : 0);
                        }
                        else
                        {
                            conn.SetParameter("@CheckFG", SqlDbType.Bit, Constant.bitEmp);
                        }

                        conn.SetParameter("@DelFG", SqlDbType.Bit, Constant.bitEmp);
                        conn.SetParameter("@UpdUserCD", SqlDbType.Char, Constant.sUser);
                        conn.SetParameter("@LastUpdDT", SqlDbType.DateTime, DateTime.Now);

                        //文字数多の為、ログ除外 (2015/2/14 n.yoshimoto)
                        //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, EquiInfo.EquipmentNO + "/" + plmInfo.QcParamNO + "/" + dtMeasureDT + "/▲[SQL]/" + sql);

//#if Debug
//                        using (DBConnect DebConn = DBConnect.CreateInstance(getConnStringForDebug(Constant.DBConnectGroup.EICSDB, 0), "System.Data.SqlClient", false))
//                        {
//                            DebConn.ExecuteNonQuery(sql);
//                        }
//#else
                        conn.ExecuteNonQuery(sql);
//#endif
                    }
                }
                catch (Exception ex)
                {
                    string funcNm = "InsertTnLOG(LSETInfo, PLMInfo, MagInfo, string, string, string)";
                    //string sMsg = EquiInfo.InlineCD + "/" + funcNm + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    string sMsg = string.Format("lineCD:{0} / plantCD:{1} / funcNm:{2} / QcParamNo:{3} / LotNo:{4} / MagNo:{5} / measureDt:{6} / sValue:{7} / sMessage:{8} / [原因]:{9} / ネットワーク停止<--Start"
                        , EquiInfo.InlineCD, EquiInfo.EquipmentNO, funcNm, plmInfo.QcParamNO, MagInfo.sNascaLotNO, MagInfo.sMagazineNO, dtMeasureDT, sValue, sMessage, ex.Message);

                    throw new ApplicationException(sMsg, ex);
                }
            }
        }

        /// <summary>
        /// NG判定を行うがエラー出力しない・TnLOGへレコード追加する。
        /// </summary>
        /// <param name="sQcParamNO"></param>
        /// <param name="sLotNO"></param>
        /// <param name="sParameterVAL"></param>
        /// <param name="dtLastUpdDT"></param>
        public static bool InsertTnLOG_NotOutputErr(LSETInfo EquiInfo, Plm plmInfo, MagInfo MagInfo, string sValue, string dtMeasureDT)
        {
            int nSeqNO = 0;
            string sql;
            string sMessage = "";
            Decimal? dValue = 0;
            bool fErr = false;//ｴﾗｰあるなし

            //Inline_CD,Equipment_NO,NascaLot_NO,QcParam_NOが同じものがあれば、Delete
            //→メンテの為、一旦マガジン取り出し(ファイル1出力)、再投入した(ファイル2出力)場合
            //  同一Lotで2ファイル出力された状態となる。この場合、最初のデータは削除し、後のファイルのみ登録する。=Delete&Insert
            //DeleteSameLot(EquiInfo, ParamInfo, MagInfo, dtMeasureDT);

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, EquiInfo.InlineCD), "System.Data.SqlClient", false))
            {
                try
                {

                    sql = @" SELECT Measure_DT FROM TnLOG WITH(NOLOCK) 
												 WHERE Inline_CD = @InlineCD AND Equipment_NO = @EquipmentNO AND Measure_DT = @MeasureDT AND Magazine_NO = @MagazineNO
												 AND NascaLot_NO = @NascaLotNO AND QcParam_NO = @QcParamNO AND DParameter_VAL = @DParamVal AND SParameter_VAL = @SParamVal ";

                    conn.SetParameter("@InlineCD", SqlDbType.Int, EquiInfo.InlineCD);
                    conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, EquiInfo.EquipmentNO);
                    conn.SetParameter("@MeasureDT", SqlDbType.DateTime, dtMeasureDT);
                    conn.SetParameter("@MagazineNO", SqlDbType.NVarChar, MagInfo.sMagazineNO);
                    conn.SetParameter("@NascaLotNO", SqlDbType.NVarChar, MagInfo.sNascaLotNO);
                    conn.SetParameter("@QcParamNO", SqlDbType.Int, plmInfo.QcParamNO);

                    string logMsg;
                    Log.ParameterSet paramSet = Log.GetParameter(plmInfo, sValue, out logMsg);

                    if (!string.IsNullOrEmpty(logMsg))
                    {
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{0} 測定日時:{1} {2}", EquiInfo.EquipmentNO, dtMeasureDT, logMsg));
                    }

                    if (paramSet.DParameterVAL.HasValue)
                    {
                        conn.SetParameter("@DParamVal", SqlDbType.Decimal, paramSet.DParameterVAL);
                    }
                    else
                    {
                        conn.SetParameter("@DParamVal", SqlDbType.Decimal, DBNull.Value);
                    }

                    if (string.IsNullOrEmpty(paramSet.SParameterVAL))
                    {
                        conn.SetParameter("@SParamVal", SqlDbType.NVarChar, DBNull.Value);
                    }
                    else
                    {
                        conn.SetParameter("@SParamVal", SqlDbType.NVarChar, paramSet.SParameterVAL);
                    }

                    dValue = paramSet.DParameterVAL;

                    //NG判定してメッセージ入れ込み
                    if (plmInfo != null && plmInfo.ParameterVAL != "") //1:文字列か、管理限界情報を取得出来ていない場合
                    {
                        if (plmInfo.ParameterVAL.ToUpper() != sValue.ToUpper() && plmInfo.ParameterVAL.ToUpper() != Constant.sOKStrings)
                        {
                            //sMessage = "[{0}/{1}号機/{2}]の設定値に誤りがあります。\r\n取得値={3},閾値={4}";
                            sMessage = string.Format(Constant.MessageInfo.Message_23, EquiInfo.AssetsNM, EquiInfo.MachineSeqNO, plmInfo.ParameterNM, sValue, plmInfo.ParameterVAL, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);
                            //ShowSubForm(sMessage);エラー出力なし

                            fErr = true;
                            //Log.Logger.Info(sMessage);
                        }
                    }
                    else //0:数値で、管理限界情報を取得出来ている場合
                    {
                        if (Plm.HasDecimalLimit(plmInfo))
                        {
                            if (plmInfo.ManageNM == Constant.sMAXMIN)
                            {
                                if (plmInfo.ParameterMAX < dValue)
                                {
                                    sMessage = string.Format(Constant.MessageInfo.Message_22, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, "MAX", dValue, plmInfo.ParameterMAX, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);

                                    fErr = true;
                                }
                                else if (plmInfo.ParameterMIN > dValue)
                                {
                                    sMessage = string.Format(Constant.MessageInfo.Message_22, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, "MIN", dValue, plmInfo.ParameterMAX, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);

                                    fErr = true;
                                }
                            }
                            else if (plmInfo.ManageNM == Constant.sMAX)
                            {
                                if (plmInfo.ParameterMAX < dValue)
                                {
                                    sMessage = string.Format(Constant.MessageInfo.Message_22, EquiInfo.AssetsNM, EquiInfo.MachineNM, plmInfo.ParameterNM, "MAX", dValue, plmInfo.ParameterMAX, MagInfo.sNascaLotNO, EquiInfo.InlineCD, plmInfo.QcParamNO);

                                    fErr = true;
                                }
                            }
                        }
                    }

                    //重複確認
                    if (conn.ExecuteScalar(sql) == null)
                    {
                        //重複なければ、INSERT

                        //但し、事前に同じ「インライン番号/設備番号/計測日時/パラメータ管理番号」の数をカウントして、カウント+1をSeq_NOに決定する。
                        nSeqNO = GetTnLOG_SeqNO(EquiInfo.InlineCD, EquiInfo.EquipmentNO, dtMeasureDT, plmInfo.QcParamNO);

                        sql = @" INSERT INTO TnLog(Inline_CD, Equipment_NO, Measure_DT, Seq_NO, QcParam_NO, Material_CD, Magazine_NO, NascaLot_NO, DParameter_VAL, SParameter_VAL, Message_NM, Error_FG, Check_FG, Del_FG, UpdUser_CD, LastUpd_DT)
									 VALUES(@InlineCD, @EquipmentNO, @MeasureDT, @SeqNO , @QcParamNO, @MaterialCD, @MagazineNO, @NascaLotNO, @DParamVal, @SParamVal , @MessageNM, @ErrorFG , @CheckFG, @DelFG, @UpdUserCD, @LastUpdDT)";

                        conn.SetParameter("@SeqNO", SqlDbType.Int, nSeqNO);
                        conn.SetParameter("@MaterialCD", SqlDbType.NVarChar, MagInfo.sMaterialCD);

                        conn.SetParameter("@MessageNM", SqlDbType.NVarChar, sMessage);
                        //@ErrorFG , @CheckFG, @DelFG, @UpdUserCD, @LastUpdDT)";
                        conn.SetParameter("@ErrorFG", SqlDbType.Bit, Constant.bitEmp);
                        conn.SetParameter("@CheckFG", SqlDbType.Bit, Constant.bitEmp);
                        conn.SetParameter("@DelFG", SqlDbType.Bit, Constant.bitEmp);
                        conn.SetParameter("@UpdUserCD", SqlDbType.Char, Constant.sUser);
                        conn.SetParameter("@LastUpdDT", SqlDbType.DateTime, DateTime.Now);

#if Debug
                        using (DBConnect DebConn = DBConnect.CreateInstance(getConnStringForDebug(Constant.DBConnectGroup.EICSDB, 0), "System.Data.SqlClient", false))
                        {
                            DebConn.ExecuteNonQuery(sql);
                        }
#else
                        conn.ExecuteNonQuery(sql);
#endif

                    }
                }
                catch (Exception ex)
                {
                    string funcNm = "InsertTnLOG_NotOutputErr(LSETInfo, Plm, MagInfo, string, string)";
                    string sMsg = string.Format("lineCD:{0} / plantCD:{1} / funcNm:{2} / QcParamNo:{3} / LotNo:{4} / MagNo:{5} / measureDt:{6} / sValue:{7} / sMessage:{8} / [原因]:{9} / ネットワーク停止<--Start"
                            , EquiInfo.InlineCD, EquiInfo.EquipmentNO, funcNm, plmInfo.QcParamNO, MagInfo.sNascaLotNO, MagInfo.sMagazineNO, dtMeasureDT, sValue, sMessage, ex.Message);

                    throw new ApplicationException(sMsg, ex);
                }
            }
            return fErr;
        }

        //「インライン番号/設備番号/計測日時/パラメータ管理番号」の数をカウントして、カウント+1をSeq_NOに決定する。
        public static int GetTnLOG_SeqNO(int nInlineCD, string sEquipmentNO, string dtMeasureDT, int nQcParamNO)
        {
            System.Data.Common.DbDataReader rd = null;

            int nCnt = 0;
            string BaseSql = "SELECT Measure_DT FROM TnLOG WITH(NOLOCK) WHERE Inline_CD ={0} AND Equipment_NO='{1}' AND Measure_DT='{2}' AND QcParam_NO={3}";
            string sql = string.Format(BaseSql, nInlineCD, sEquipmentNO, dtMeasureDT, nQcParamNO);
            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, nInlineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            nCnt = nCnt + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = nInlineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }
            }

            return nCnt + 1;
        }

        /// <summary>
        /// nMode=0で実行すると単数号機で監視する。
        /// nMode=1で実行すると、複数号機で監視する(設備番号をWhere句に入れない)。
        /// </summary>
        /// <param name="nLineCD"></param>
        /// <param name="sEquiNO"></param>
        /// <param name="wInspData"></param>
        /// <param name="nMode"></param>
        /// <returns></returns>
        public static SortedList<int, QCLogData> GetQCItem(LSETInfo EquiInfo, InspData wInspData, string sType, int nMode, int nQcprmNO)
        {
            System.Data.Common.DbDataReader rd = null;
            int nCnt = 0;

            //設定されている遡る個数
            //int nBackCnt = GetQCBackCnt(wInspData.InspectionNO);
            int nBackCnt = 10;

            string sWhereSql = "";
            SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();
            //QCLogData wQCLogData = new QCLogData();こっちで宣言すると全て同じ値となる

            //現在時間から設定個数分の管理番号データを取得する
            //単数号機監視
            if (nMode == 0)
            {
                sWhereSql = " AND Equipment_NO='" + EquiInfo.EquipmentNO + "'";
            }
            else
            {//複数号機監視
                sWhereSql = GetSQLRelationEqui(EquiInfo);
            }

            string BaseSql = @"SELECT TOP {0} NascaLot_NO,DParameter_VAL,Equipment_NO,Measure_DT,Message_NM 
							   FROM TnLOG WITH(NOLOCK) 
							   WHERE Inline_CD ={1}
								 AND Measure_DT>'{2}'
								 AND Measure_DT<='{3}'
								 AND NascaLot_No <> ''
								 AND Material_CD='{4}'
								 AND QcParam_NO={5} "
                             + sWhereSql + " ORDER BY Measure_DT DESC";//新しい方から取ってくる

            string sql = string.Format(BaseSql, nBackCnt, EquiInfo.InlineCD, DateTime.Now.AddDays(-10), DateTime.Now, sType, nQcprmNO);

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, EquiInfo.InlineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            QCLogData wQCLogData = new QCLogData();                               //こっちが正解

                            wQCLogData.EquiNO = Convert.ToString(rd["Equipment_NO"]).Trim();  //設備番号
                            wQCLogData.LotNO = Convert.ToString(rd["NascaLot_NO"]).Trim();    //Lot
                            wQCLogData.TypeCD = sType;                                            //Type
                            wQCLogData.MeasureDT = Convert.ToDateTime(rd["Measure_DT"]);      //計測日時
                            wQCLogData.Data = Convert.ToDouble(rd["DParameter_VAL"]);         //data
                            wQCLogData.Defect = wInspData.Defect;                                 //監視項目No
                            wQCLogData.InspectionNO = wInspData.InspectionNO;                     //監視番号
                            wQCLogData.QcprmNO = nQcprmNO;                                        //動作番号
                            wQCLogData.MessageNM = Convert.ToString(rd["Message_NM"]).Trim(); //メッセージ
                            cndDataItem.Add(nCnt, wQCLogData);
                            nCnt = nCnt + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = EquiInfo.InlineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }
            }
            return cndDataItem;
        }

        /// <summary>
        /// nMode=0で実行すると単数号機で監視する。
        /// nMode=1で実行すると、複数号機で監視する(設備番号をWhere句に入れない)。
        /// </summary>
        /// <param name="nLineCD"></param>
        /// <param name="sEquiNO"></param>
        /// <param name="wInspData"></param>
        /// <param name="nMode"></param>
        /// <returns></returns>
        public static SortedList<int, QCLogData> GetQCItem(LSETInfo EquiInfo, InspData wInspData, string sType, int nMode, int nQcprmNO, List<string> LotList)
        {
            System.Data.Common.DbDataReader rd = null;
            int nCnt = 0;

            //設定されている遡る個数
            //int nBackCnt = GetQCBackCnt(wInspData.InspectionNO);
            int nBackCnt = 10;

            string sWhereSql = "";
            SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();
            //QCLogData wQCLogData = new QCLogData();こっちで宣言すると全て同じ値となる

            //現在時間から設定個数分の管理番号データを取得する
            //単数号機監視
            if (nMode == 0)
            {
                sWhereSql = " AND Equipment_NO='" + EquiInfo.EquipmentNO + "'";
            }
            else
            {//複数号機監視
                sWhereSql = GetSQLRelationEqui(EquiInfo);
            }

            ////文字数多の為、ログ除外(2015/2/14 n.yoshimoto)
            ////log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, EquiInfo.EquipmentNO + "/▲[SQL]GetQCItem/" + sql);

            string BaseSql = @"SELECT TOP {0} NascaLot_NO,DParameter_VAL,Equipment_NO,Measure_DT,Message_NM 
							   FROM TnLOG WITH(NOLOCK) 
							   WHERE Inline_CD ={1}
								 AND Measure_DT>'{2}'
								 AND Measure_DT<='{3}'
								 AND NascaLot_No <> ''
								 AND Material_CD='{4}'
								 AND QcParam_NO={5} "
                 + sWhereSql + " ORDER BY Measure_DT DESC";//新しい方から取ってくる

            string sql = string.Format(BaseSql, nBackCnt, EquiInfo.InlineCD, DateTime.Now.AddDays(-10), DateTime.Now, sType, nQcprmNO);


            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, EquiInfo.InlineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            QCLogData wQCLogData = new QCLogData();                               //こっちが正解

                            wQCLogData.EquiNO = Convert.ToString(rd["Equipment_NO"]).Trim();  //設備番号
                            wQCLogData.LotNO = Convert.ToString(rd["NascaLot_NO"]).Trim();    //Lot
                            wQCLogData.TypeCD = sType;                                            //Type
                            wQCLogData.MeasureDT = Convert.ToDateTime(rd["Measure_DT"]);      //計測日時
                            wQCLogData.Data = Convert.ToDouble(rd["DParameter_VAL"]);         //data
                            wQCLogData.Defect = wInspData.Defect;                                 //監視項目No
                            wQCLogData.InspectionNO = wInspData.InspectionNO;                     //監視番号
                            wQCLogData.QcprmNO = nQcprmNO;                                        //動作番号
                            wQCLogData.MessageNM = Convert.ToString(rd["Message_NM"]).Trim(); //メッセージ
                            cndDataItem.Add(nCnt, wQCLogData);
                            nCnt = nCnt + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = EquiInfo.InlineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }

            }
            return cndDataItem;
        }

        //そのLineに組み込まれている同装置のListを返す。
        public static string GetSQLRelationEqui(LSETInfo EquiInfo)
        {
            System.Data.Common.DbDataReader rd = null;
            string sWhereSQL = "@AND (";

            string BaseSql = "Select Equipment_NO From TvLSET WITH(NOLOCK) Where Inline_CD={0} AND Assets_NM='{1}'";
            string sql = string.Format(BaseSql, EquiInfo.InlineCD, EquiInfo.AssetsNM);

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, EquiInfo.InlineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            sWhereSQL = sWhereSQL.Replace("@", " ");
                            sWhereSQL = sWhereSQL + "Equipment_NO= '" + Convert.ToString(rd["Equipment_NO"]).Trim() + "' OR@";  //設備番号
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = EquiInfo.InlineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }
            }

            sWhereSQL = sWhereSQL.Replace("OR@", "");
            sWhereSQL = sWhereSQL + ")";
            return sWhereSQL;
        }

        public static bool GetQCSettingSwitch(int lineCD, string sType, int nInspNO, int nNum)
        {
            System.Data.Common.DbDataReader rd = null;
            bool bSwitch = false;

            string BaseSql = "SELECT USE_FG FROM TmQCST WITH(NOLOCK) WHERE Material_CD=@MATECD AND Inspection_NO=@INSPECTIONNO AND QCnum_NO =@QCNUMNO ";

            string sql = BaseSql;

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    conn.SetParameter("@MATECD", SqlDbType.Char, sType);
                    conn.SetParameter("@INSPECTIONNO", SqlDbType.Int, nInspNO);
                    conn.SetParameter("@QCNUMNO", SqlDbType.Char, nNum);

                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            bSwitch = Convert.ToBoolean(rd["USE_FG"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = lineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }

            }
            return bSwitch;
        }

        public static string GetQcNum(int lineCD, string sType, int nInspNO, int nNum)
        {
            System.Data.Common.DbDataReader rd = null;
            string sQcNum = "";

            string BaseSql = "Select QCnum_VAL From TmQCST WITH(NOLOCK) Where Material_CD=@MATECD AND Qcnum_NO=@QCNUMNO AND Inspection_NO=@INSPCTNO";
            string sql = BaseSql;
            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    conn.SetParameter("@QCNUMNO", SqlDbType.Int, nNum);
                    conn.SetParameter("@MATECD", SqlDbType.Char, sType);
                    conn.SetParameter("@INSPCTNO", SqlDbType.Int, nInspNO);

                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            sQcNum = Convert.ToString(rd["QCnum_VAL"]).Trim();  //設定数字(カンマ区切り)取得
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = lineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }

            }
            return sQcNum;
        }

        public static string GetQcLine(int lineCD, string sType, int nQcprmNO)
        {
            System.Data.Common.DbDataReader rd = null;
            string sVal = "";
            string BaseSql = "SELECT [QcLine_MAX],[QcLine_MIN],[AimLine_VAL],[AimRate_VAL],[Parameter_MAX],[Parameter_MIN] " +
                             "FROM [TmPLM]  WITH(NOLOCK) " +
                             "Where Material_CD='{0}' AND QcParam_NO={1}";
            string sql = string.Format(BaseSql, sType, nQcprmNO);
            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            sVal = Convert.ToString(rd["QcLine_MAX"]).Trim();
                            sVal = sVal + "," + Convert.ToString(rd["QcLine_MIN"]).Trim();
                            sVal = sVal + "," + Convert.ToString(rd["AimLine_VAL"]).Trim();
                            sVal = sVal + "," + Convert.ToString(rd["AimRate_VAL"]).Trim();
                            sVal = sVal + "," + Convert.ToString(rd["Parameter_MAX"]).Trim();
                            sVal = sVal + "," + Convert.ToString(rd["Parameter_MIN"]).Trim();

                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = lineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }

            }
            return sVal;
        }

        /// 装置情報と外観検査機ｴﾗｰNoを入れるとNascaErrNoを返す
        public static string GetTmErrConv_NascaErrNO(LSETInfo lsetInfo, int nErrNo)
        {
            try
            {
                string sNascaErrNo = "", sDefCause = "", sDefClass = "";
                string sRtn = "";

                string BaseSql = "SELECT NascaErr_NO,DefCause_CD,DefClass_CD FROM TmErrConv  WITH(NOLOCK) WHERE Equipment_NO = '{0}' AND EquiErr_NO={1}";
                string sql = string.Format(BaseSql, lsetInfo.EquipmentNO, nErrNo);

                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
                {
                    System.Data.Common.DbDataReader rd = null;
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            sNascaErrNo = Convert.ToString(rd["NascaErr_NO"]).Trim();
                            sDefCause = Convert.ToString(rd["DefCause_CD"]).Trim();
                            sDefClass = Convert.ToString(rd["DefClass_CD"]).Trim();
                        }
                    }
                }
                sRtn = sNascaErrNo + "," + sDefCause + "," + sDefClass;
                return sRtn;
            }
            catch (Exception err)
            {
                throw;
            }
        }

        /// <summary>
        /// 外観検査機エラー番号変換マスタ[TmErrConv]取得
        /// </summary>
        /// <param name="plantCD">設備NO</param>
        /// <param name="qcParamNO">管理NO</param>
        /// <returns>外観検査機エラー番号変換マスタ</returns>
        public static ErrConvInfo GetErrConvInfo(LSETInfo lsetInfo, int qcParamNO)
        {
            System.Data.Common.DbDataReader rd = null;
            ErrConvInfo errConvInfo = null;

            try
            {
                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT Model_NM, EquiErr_NO, QcParam_NO, NascaErr_NO, DefCause_CD, DefClass_CD
                                FROM TmErrConv
                                WHERE (QcParam_NO = @QcParamNO) AND (Equipment_NO = @EquipmentNO) ";

                    conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNO);
                    conn.SetParameter("@ModelNM", SqlDbType.NVarChar, lsetInfo.ModelNM);

                    using (rd = conn.GetReader(sql))
                    {
                        if (!rd.HasRows)
                        {
                            string sMessage = "[管理番号:" + qcParamNO + "]ErrConvマスタに設定されていません。システム担当者に連絡して下さい。";
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                            throw new Exception(sMessage);
                        }

                        while (rd.Read())
                        {
                            errConvInfo = new ErrConvInfo();
                            errConvInfo.EquipmentNO = Convert.ToString(rd["Equipment_NO"]).Trim();
                            errConvInfo.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
                            errConvInfo.EquiErrNO = Convert.ToString(rd["EquiErr_NO"]).Trim();
                            errConvInfo.NascaErrNO = Convert.ToString(rd["NascaErr_NO"]).Trim();
                        }
                    }
                }

                return errConvInfo;
            }
            catch (Exception err)
            {
                throw;
            }
        }

        /// 装置情報と装置ｴﾗｰNoを入れるとNascaErrNoを返す
        public static string GetNascaErrByPlantErr(LSETInfo lsetInfo, int plantErrNo)
        {
            try
            {
                string sNascaErrNo = "", sDefCause = "", sDefClass = "";
                string sRtn = "";

                string sql = " SELECT NascaErr_NO,DefCause_CD,DefClass_CD FROM TmERCV WITH(NOLOCK) WHERE Model_NM = @ModelNM AND EquiErr_NO = @EquiErrNO ";

                using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
                {
                    conn.SetParameter("@ModelNM", SqlDbType.NVarChar, lsetInfo.ModelNM);
                    conn.SetParameter("@EquiErrNO", SqlDbType.NVarChar, plantErrNo);

                    System.Data.Common.DbDataReader rd = null;

                    using (rd = conn.GetReader(sql))
                    {
                        int ordNascaErrNO = rd.GetOrdinal("NascaErr_NO");
                        int ordDefCauseCD = rd.GetOrdinal("DefCause_CD");
                        int ordDefClassCD = rd.GetOrdinal("DefClass_CD");

                        while (rd.Read())
                        {
                            sNascaErrNo = rd.GetString(ordNascaErrNO);
                            sDefCause = rd.GetString(ordDefCauseCD);
                            sDefClass = rd.GetString(ordDefClassCD);
                        }
                    }
                }
                sRtn = sNascaErrNo + "," + sDefCause + "," + sDefClass;
                return sRtn;
            }
            catch (Exception err)
            {
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="text"></param>
        public static void RecordNotify(SortedList<int, QCLogData> cndDataItem, int nMultiNO, int i, string text, int lineCD)
        {
            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    conn.SetParameter("@QCNRNO", SqlDbType.Int, -1);
                    conn.SetParameter("@EQUIPMENTNO", SqlDbType.Char, cndDataItem[i].EquiNO);
                    conn.SetParameter("@INLINECD", SqlDbType.Int, lineCD);
                    conn.SetParameter("@DEFECT", SqlDbType.Int, cndDataItem[i].Defect);
                    conn.SetParameter("@INSPECTIONNO", SqlDbType.Int, cndDataItem[i].InspectionNO);
                    conn.SetParameter("@MULTINO", SqlDbType.Int, nMultiNO);
                    conn.SetParameter("@NASCALOTNO", SqlDbType.Char, cndDataItem[i].LotNO);
                    conn.SetParameter("@MEASUREDT", SqlDbType.DateTime, cndDataItem[i].MeasureDT);
                    conn.SetParameter("@MESSAGE", SqlDbType.VarChar, text);
                    conn.SetParameter("@TYPECD", SqlDbType.VarChar, cndDataItem[i].TypeCD);
                    //int nQCBackCnt = ConnectDB.GetQCBackCnt(cndDataItem[i].InspectionNO);
                    conn.SetParameter("@BACKNUMNO", SqlDbType.Int, 10);
                    conn.SetParameter("@CHECKNO", SqlDbType.Int, 0);
                    conn.SetParameter("@UPDUSERCD", SqlDbType.Char, "9999");
                    conn.SetParameter("@LASTUPDDT", SqlDbType.DateTime, DateTime.Now);

                    string sql = @"
                        SELECT
                        Equipment_NO
                        FROM TnQCNR  WITH(NOLOCK)
                        WHERE Inline_CD =@INLINECD 
                        AND Equipment_NO = @EQUIPMENTNO
                        AND Defect_NO = @DEFECT
                        AND Inspection_NO = @INSPECTIONNO
                        AND Multi_NO = @MULTINO
                        AND NascaLot_NO = @NASCALOTNO
                        AND Measure_DT = @MEASUREDT
                        AND Message = @MESSAGE";

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "TnQCNR /▲[SQL SELECT・INSERT]/" + cndDataItem[i].EquiNO + "/" + cndDataItem[i].Defect + "/" + cndDataItem[i].InspectionNO + "/" + nMultiNO + "/" +
                        cndDataItem[i].LotNO + "/" + cndDataItem[i].MeasureDT + "/" + text + "/" + cndDataItem[i].TypeCD + "/" + 10 + "/" + DateTime.Now);

                    if (conn.ExecuteScalar(sql) == null)
                    {
                        sql = @"
                    INSERT
                     TnQCNR(
                          Inline_CD
                        , Equipment_NO
                        , Defect_NO
                        , Inspection_NO
                        , Multi_NO
                        , NascaLot_NO
                        , Measure_DT
	                    , Message
	                    , Type_CD
                        , BackNum_NO
                        , Check_NO
	                    , UpdUser_CD
	                    , LastUpd_DT)
                    VALUES(
                          @INLINECD
                        , @EQUIPMENTNO
                        , @DEFECT
                        , @INSPECTIONNO
                        , @MULTINO
                        , @NASCALOTNO
	                    , @MEASUREDT
	                    , @MESSAGE
	                    , @TYPECD
	                    , @BACKNUMNO
                        , @CHECKNO
	                    , @UPDUSERCD
	                    , @LASTUPDDT)";

#if Debug || TEST
                        //using (DBConnect DebConn = DBConnect.CreateInstance(getConnStringForDebug(Constant.DBConnectGroup.EICSDB, 0), "System.Data.SqlClient", false))
                        //{
                        //	DebConn.ExecuteNonQuery(sql);
                        //}
#else
                        conn.ExecuteNonQuery(sql);
#endif
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = lineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }
            }
        }

        public static List<string> GetLotList(LSETInfo EquiInfo, int nTiming, InspData wInspData, string sType, int nMode)
        {
            System.Data.Common.DbDataReader rd = null;
            int nQcParamNo = 0;
            List<string> ListLotNo = new List<string>();

            //設定されている遡る個数
            int nBackCnt = 10;//ConnectDB.GetQCBackCnt(wInspData.InspectionNO);

            //現在時間から設定個数分の管理番号データを取得する
            //単数号機監視
            string sWhereSql = "";
            if (nMode == 0)
            {
                sWhereSql = " AND Equipment_NO='" + EquiInfo.EquipmentNO + "'";
            }
            else
            {//複数号機監視
                sWhereSql = GetSQLRelationEqui(EquiInfo);
            }

            //決め打ちでLotListを作成する。
            if (nTiming == 1)//ﾀﾞｲﾎﾞﾝﾀﾞｰ
            {
                nQcParamNo = 11;    //[11:ｺﾚｯﾄ打点回数]を決め打ちしてLotListを作成する。
            }
            else if (nTiming == 2)//ﾜｲﾎﾞﾝﾀﾞｰ
            {
                nQcParamNo = 39;      //[39:キャピラリ－打点数]を決め打ちしてLotListを作成する。
            }
            else if (nTiming == 3)//外観検査機
            {
                nQcParamNo = 43;      //[43:ダイスＹズレAve]を決め打ちしてLotListを作成する。
            }
            else if (nTiming == 4)//ﾓｰﾙﾄﾞ機
            {
                nQcParamNo = 176;   //[176:全ｼﾘﾝｼﾞ樹脂量測定値(マガジン終了:σ)]を決め打ちしてLotListを作成する。
            }

            string BaseSql = "SELECT TOP {0} NascaLot_NO " +
                             "FROM TnLOG WITH(NOLOCK) WHERE Inline_CD ={1} AND QcParam_NO={2} AND Measure_DT<'{3}' AND NascaLot_No <> '' AND Material_CD='{4}' " + sWhereSql +
                             " ORDER BY Measure_DT DESC";
            string sql = string.Format(BaseSql, nBackCnt, EquiInfo.InlineCD, nQcParamNo, DateTime.Now, sType);

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, EquiInfo.InlineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            ListLotNo.Add(Convert.ToString(rd["NascaLot_NO"]).Trim());    //Lot);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = EquiInfo.InlineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }
            }

            return ListLotNo;
        }

        //同一フレームでタイミング違いでエラーを出さないようにチェックする。
        public static bool CheckNG(LSETInfo lsetInfo, int nQcParamNO, string dtMeasureDT)
        {
            System.Data.Common.DbDataReader rd = null;

            string BaseSql, sql;
            string sMessageNM = "";

            BaseSql = "SELECT Message_NM FROM TnLOG WITH(NOLOCK) WHERE Equipment_NO='{0}' AND Measure_DT='{1}' AND QcParam_NO={2}";
            sql = string.Format(BaseSql, lsetInfo.EquipmentNO, dtMeasureDT, nQcParamNO);
            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
            {
                try
                {
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            sMessageNM = Convert.ToString(rd["Message_NM"]).Trim();
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = lsetInfo.InlineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }

            }
            //エラーなし
            if (sMessageNM == "")
            {
                return false;
            }
            else
            {//エラーあり
                return true;
            }
        }

        public static SortedList<int, InspData> GetInspectionList(int nTiming, int lineCD)
        {
            System.Data.Common.DbDataReader rd = null;

            string[] textArray = new string[] { };
            string sWork = "";

            SortedList<int, InspData> wSLInspection = new SortedList<int, InspData>();

            //string BaseSql = "SELECT Defect_NO,Process_NO,Inspection_NO,Inspection_NM,Multi_NO FROM TvQDIW WHERE Timing_NO ={0} AND Watch_NO={1}";
            string BaseSql = "SELECT Defect_NO,Process_NO,Inspection_NO,Inspection_NM FROM TvQDIW WITH(NOLOCK) WHERE Timing_NO ={0} AND Watch_NO={1}";
            string sql = string.Format(BaseSql, nTiming, 2);//2が監視項目

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                int nCnt = 0;
                try
                {
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            InspData inspdata = new InspData();

                            inspdata.Defect = Convert.ToInt32(rd["Defect_NO"]);

                            //inspdata.Number = Convert.ToInt32(reader["Process_NO"]);
                            sWork = Convert.ToString(rd["Process_NO"]);
                            textArray = sWork.Split(',');
                            for (int i = 1; i < Convert.ToInt32(textArray.Length) - 1; i++)
                            {
                                int paramNo = Convert.ToInt32(textArray[i]);
                                Prm prm = Prm.GetData(lineCD, paramNo, null, null);
                                InspData.QcParamInfo paramInfo = new InspData.QcParamInfo(paramNo, prm.UnManageTrendFG, prm.WithoutFileFmtFG);
                                inspdata.ParamInfo.Add(paramInfo);
                            }
                            inspdata.InspectionNO = Convert.ToInt32(rd["Inspection_NO"]);
                            inspdata.Param = Convert.ToString(rd["Inspection_NM"]).Trim();
                            //inspdata.Multi = Convert.ToInt32(reader["Multi_NO"]);

                            wSLInspection.Add(nCnt, inspdata);
                            nCnt = nCnt + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = lineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }

            }
            return wSLInspection;
        }

        public static SortedList<int, InspData> GetMAPInspectionList(int nTiming, int lineCD)
        {
            System.Data.Common.DbDataReader rd = null;

            string[] textArray = new string[] { };
            string sWork = "";

            SortedList<int, InspData> wSLInspection = new SortedList<int, InspData>();

            //string BaseSql = "SELECT Defect_NO,Process_NO,Inspection_NO,Inspection_NM,Multi_NO FROM TvQDIW WHERE Timing_NO ={0} AND Watch_NO={1}";
            string BaseSql = "SELECT Process_NO,Inspection_NO,Inspection_NM FROM TvQDIW_Map WITH(NOLOCK) WHERE Del_FG = 0 and Timing_NO ={0} ";
            string sql = string.Format(BaseSql, nTiming);//2が監視項目

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                int nCnt = 0;
                try
                {
                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            InspData inspdata = new InspData();

                            inspdata.Defect = 0;//MAPはなし

                            //inspdata.Number = Convert.ToInt32(reader["Process_NO"]);
                            sWork = Convert.ToString(rd["Process_NO"]);
                            textArray = sWork.Split(',');
                            for (int i = 0; i < textArray.Length; i++)
                            {
                                if (string.IsNullOrEmpty(textArray[i]))
                                {
                                    continue;
                                }

                                int paramNo = Convert.ToInt32(textArray[i]);
                                Prm prm = Prm.GetData(lineCD, paramNo, null, null);
                                if (prm == null)
                                {
                                    throw new ApplicationException($"TmPRMに管理番号『{paramNo}』のデータが存在しません。");
                                }

                                InspData.QcParamInfo paramInfo = new InspData.QcParamInfo(paramNo, prm.UnManageTrendFG, prm.WithoutFileFmtFG);

                                inspdata.ParamInfo.Add(paramInfo);
                            }
                            inspdata.InspectionNO = Convert.ToInt32(rd["Inspection_NO"]);
                            inspdata.Param = Convert.ToString(rd["Inspection_NM"]).Trim();
                            //inspdata.Multi = Convert.ToInt32(reader["Multi_NO"]);

                            wSLInspection.Add(nCnt, inspdata);
                            nCnt = nCnt + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    AlertLog alertLog = AlertLog.GetInstance();
                    alertLog.logMessageQue.Enqueue($"GEICSへのデータ登録処理が途中終了しました。システム担当者へ連絡して下さい。理由:{ex.ToString()}");
                    //string sMsg = lineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }
            }
            return wSLInspection;
        }

        //QcParam_NOからInspectionNoを抜くだけのメソッド ※スパッタ用　2015.8.24湯浅
        public static int GetMAPInspectionNo(int nTiming, int qcParamNo, int lineCD)
        {

            SortedList<int, InspData> inspList = GetMAPInspectionList(nTiming, lineCD);

            int retv = 0;

            for (int i = 0; i < inspList.Count(); i++)
            {
                for (int j = 0; j < inspList[i].ParamInfo.Count(); j++)
                {
                    if (qcParamNo == inspList[i].ParamInfo[j].No)
                    {
                        retv = inspList[i].InspectionNO;
                    }
                }
            }

            return retv;
        }




        /// <summary>
        /// Bumpあり/なし
        /// </summary>
        /// <param name="sMate"></param>
        /// <returns></returns>
        //public static bool GetBumpUse(string sMate, int lineCD)
        //{
        //    bool fBump = false;

        //    string BaseSql = "SELECT Bump_FG FROM TmDIECT WITH(NOLOCK) WHERE Material_CD=@MATERIALCD AND Del_FG=0";
        //    string sql = BaseSql;

        //    using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
        //    {
        //        System.Data.Common.DbDataReader rd = null;
        //        try
        //        {
        //            conn.SetParameter("@MATERIALCD", SqlDbType.Char, sMate);

        //            using (rd = conn.GetReader(sql))
        //            {
        //                while (rd.Read())
        //                {
        //                    fBump = Convert.ToBoolean(rd["Bump_FG"]);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            string sMsg = Constant.inlineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
        //            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
        //        }
        //    }
        //    return fBump;
        //}

        /// <summary>
        /// プラズマの設定値を取得する
        /// </summary>
        /// <param name="sModel">装置型式 >プラズマの場合は同一装置でも「PLADB」「PLAWB」で分ける</param>
        /// <param name="sMate">製品タイプ</param>
        /// <param name="sParam">管理項目名</param>
        /// <returns></returns>
        public static int GetSettingParam(string sModel, string sMate, string paramNM, string workCd, int lineCD)
        {
            //2015.2.12 車載3次 TmPRM.Chip_NMにARMSのWorkCDが入るようになり、DB,WB,MDの識別をChip_NMでする
            List<Plm> plmList = Plm.GetData(lineCD, sMate, sModel, workCd);
            plmList = plmList.Where(p => p.ParameterNM.ToUpper().Contains(paramNM.ToUpper())).ToList();

            if (plmList.Count == 0 || plmList.Count >= 2)
            {
                //閾値設定が無かったり、複数ヒットした場合はエラー
                //設定されていない場合、装置処理停止
                string message = string.Format(Constant.MessageInfo.Message_28, sMate, 0, paramNM);
                throw new Exception(message);
            }

            return Convert.ToInt32(plmList.Single().ParameterMAX);
        }

        public static List<LotPrevInfo> GetLotPrevInfo(string lotno, int lineCD)
        {
            List<LotPrevInfo> listlotprevinfo = new List<LotPrevInfo>();

            string sql = "SELECT dbo.TnTran.lotno, dbo.TmProcess.procnm, dbo.TmMachine.clasnm, dbo.TmMachine.plantnm " +
                        "FROM  dbo.TnTran WITH(NOLOCK) INNER JOIN " +
                        "dbo.TmMachine WITH(NOLOCK) ON dbo.TnTran.macno = dbo.TmMachine.macno INNER JOIN " +
                        "dbo.TmProcess WITH(NOLOCK) ON dbo.TnTran.procno = dbo.TmProcess.procno " +
                        "WHERE (dbo.TnTran.lotno LIKE '" + @lotno + "%') AND (dbo.TnTran.delfg = 0) " +
                        "ORDER BY dbo.TnTran.startdt ";

            using (DBConnect conn = DBConnect.CreateInstance(getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {
                System.Data.Common.DbDataReader rd = null;
                try
                {
                    conn.SetParameter("@lotno", SqlDbType.Char, lotno);

                    using (rd = conn.GetReader(sql))
                    {
                        while (rd.Read())
                        {
                            LotPrevInfo lotprevinfo = new LotPrevInfo();

                            lotprevinfo.lotno = Convert.ToString(rd["lotno"]);      //ロット番号
                            lotprevinfo.procnm = Convert.ToString(rd["procnm"]);    //作業名
                            lotprevinfo.clasnm = Convert.ToString(rd["clasnm"]);    //装置名
                            lotprevinfo.plantnm = Convert.ToString(rd["plantnm"]);  //号機

                            listlotprevinfo.Add(lotprevinfo);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string sMsg = lineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
                }
            }

            return listlotprevinfo;
        }
        #endregion

        #region プロパティ

        /// <summary>
        /// コネクション
        /// </summary>
        public SLCommonLib.DataBase.DBConnect Connection
        {
            get { return this.connection; }
        }

        #endregion
    }
}
