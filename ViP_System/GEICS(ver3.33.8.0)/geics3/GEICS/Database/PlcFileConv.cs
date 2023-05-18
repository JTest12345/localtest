using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace GEICS.Database
{
    public class DataTypeMasterClass
    {
        public string CD { get; set; }
        public string NM { get; set; }

        public DataTypeMasterClass(string C, string N)
        {
            this.CD = C;
            this.NM = N;
        }
    }

    public class PlcFileConv
    {
        #region パラメータ

        public bool IsCheck { get; set; }

        public int QcParamNO { get; set; }

        public string QcModelNM { get; set; }
        public string ClassNM { get; set; }
        public string ParameterNM { get; set; }

        public string ModelNM { get; set; }
        public string PrefixNM { get; set; }
        public string HeaderNM { get; set; }
        public int OrderNO { get; set; }
        public string PlcADDR { get; set; }
        public int DataLEN { get; set; }
        public string DataTypeCD { get; set; }
        public bool DelFG { get; set; }
        public DateTime LastUpdDT { get; set; }
        public string IdentifyCD { get; set; }       

        #endregion

        public static List<DataTypeMasterClass> DataTypeMaster = new List<DataTypeMasterClass>()
        {
            new DataTypeMasterClass("STR","文字列"),
            new DataTypeMasterClass("BIN","2進数16BIT"),
            new DataTypeMasterClass("DEC_32BIT","±10進数32BIT"),
            new DataTypeMasterClass("DEC_16BIT","±10進数16BIT"),
            new DataTypeMasterClass("UDEC_32BIT","10進数32BIT"),
            new DataTypeMasterClass("UDEC_16BIT","10進数16BIT"),
            new DataTypeMasterClass("HEX_32BIT","16進数32BIT"),
            new DataTypeMasterClass("HEX_16BIT","16進数16BIT"),
            new DataTypeMasterClass("BOOL","0か1"),
            new DataTypeMasterClass("FLOAT","32bit浮動少数点"),
            new DataTypeMasterClass("FLOAT16BIT","16bit浮動小数点"),
            new DataTypeMasterClass("BCD32BIT","32bitBCD値")
        };

        public static string GetDataType_NM(string DataType_CD)
        {
            List<DataTypeMasterClass> dtmList = DataTypeMaster.Where(d => d.CD == DataType_CD).ToList();
            if (dtmList.Count == 1)
            {
                return dtmList.Single().NM;
            }
            else
            {
                return Constant.VOID_STRING;
            }
        }
        public static string GetDataType_CD(string DataType_NM)
        {
            List<DataTypeMasterClass> dtmList = DataTypeMaster.Where(d => d.NM == DataType_NM).ToList();
            if (dtmList.Count == 1)
            {
                return dtmList.Single().CD;
            }
            else
            {
                return Constant.VOID_STRING;
            }
        }


        public static List<PlcFileConv> GetData(string modelNM, List<int> qcParamNOList, bool chkDelFG)
        {

            List<PlcFileConv> plcFileConvList = new List<PlcFileConv>();

            using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
            {
                string sql = @" SELECT Model_NM, QcParam_NO, Prefix_NM, Header_NM, Order_NO, Plc_ADDR, Data_LEN, DataType_CD, Del_FG, LastUpd_DT, Identify_CD
                                FROM TmPlcFileConv WITH(NOLOCK)
                                WHERE 1=1 ";

                if (!string.IsNullOrEmpty(modelNM))
                {
                    sql += " AND (Model_NM like @ModelNM) ";
                    conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM + "%");
                }
                if (qcParamNOList != null && qcParamNOList.Count > 0)
                {
                    if (qcParamNOList.Count == 1)
                    {
                        sql += " AND (QcParam_NO = @QcParamNO) ";
                        conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNOList.Single());
                    }
                    else
                    {
                        sql += " AND (QcParam_NO IN (" + Common.GetMultiSql(qcParamNOList)  + ")) ";
                    }
                }
                if (chkDelFG == false)
                {
                    sql += " AND (Del_FG = 0) ";
                }

                using (DbDataReader rd = conn.GetReader(sql))
                {
                    int ordModelNM = rd.GetOrdinal("Model_NM");
                    int ordQcParamNO = rd.GetOrdinal("QcParam_NO");
                    int ordPrefixNM = rd.GetOrdinal("Prefix_NM");
                    int ordHeaderNM = rd.GetOrdinal("Header_NM");
                    int ordOrderNO = rd.GetOrdinal("Order_NO");
                    int ordPlcADDR = rd.GetOrdinal("Plc_ADDR");
                    int ordDataLEN = rd.GetOrdinal("Data_LEN");
                    int ordDataTypeCD = rd.GetOrdinal("DataType_CD");
                    int ordDelFG = rd.GetOrdinal("Del_FG");
                    int ordLastUpdDT = rd.GetOrdinal("LastUpd_DT");
                    int ordIdenfifyCD = rd.GetOrdinal("Identify_CD");

                    while (rd.Read())
                    {
                        PlcFileConv pfc = new PlcFileConv();

                        pfc.QcParamNO = rd.GetInt32(ordQcParamNO);
                        pfc.ModelNM = rd.GetString(ordModelNM);
                        pfc.PrefixNM = rd.GetString(ordPrefixNM);
                        pfc.HeaderNM = rd.GetString(ordHeaderNM);
                        pfc.OrderNO = rd.GetInt32(ordOrderNO);
                        pfc.PlcADDR = rd.GetString(ordPlcADDR);
                        pfc.DataLEN = rd.GetInt32(ordDataLEN);
                        pfc.DataTypeCD = rd.GetString(ordDataTypeCD);
                        pfc.DelFG = rd.GetBoolean(ordDelFG);
                        pfc.LastUpdDT = rd.GetDateTime(ordLastUpdDT);
                        pfc.IdentifyCD = rd.GetString(ordIdenfifyCD);

                        plcFileConvList.Add(pfc);
                    }
                }
            }

            return plcFileConvList;
        }

        public static List<PlcFileConv> GetDatawithPRM(string modelNM, List<int> qcParamNOList, string prefixNM, bool chkDelFG)
        {

            List<PlcFileConv> plcFileConvList = new List<PlcFileConv>();

            using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
            {
                string sql = @" SELECT p.QcParam_NO, p.Model_NM AS QcModel_NM, p.Class_NM, p.Parameter_NM,
                                        pfc.Model_NM, pfc.Prefix_NM, pfc.Header_NM, pfc.Order_NO, 
                                        pfc.Plc_ADDR, pfc.Data_LEN, pfc.DataType_CD, pfc.Del_FG, pfc.LastUpd_DT, pfc.Identify_CD
                                FROM TmPlcFileConv AS pfc WITH(NOLOCK)
                                INNER JOIN TmPRM AS p WITH(NOLOCK)
                                ON pfc.QcParam_NO = p.QcParam_NO
                                WHERE 1=1 ";

                if (!string.IsNullOrEmpty(modelNM))
                {
                    sql += " AND (pfc.Model_NM like @ModelNM) ";
                    conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM + "%");
                }
                if (qcParamNOList != null && qcParamNOList.Count > 0)
                {
                    if (qcParamNOList.Count == 1)
                    {
                        sql += " AND (pfc.QcParam_NO = @QcParamNO) ";
                        conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNOList.Single());
                    }
                    else
                    {
                        sql += " AND (pfc.QcParam_NO IN (" + Common.GetMultiSql(qcParamNOList)  + ")) ";
                    }
                }
                if (!string.IsNullOrEmpty(prefixNM))
                {
                    sql += " AND (pfc.Prefix_NM like @PrefixNM) ";
                    conn.SetParameter("@PrefixNM", SqlDbType.VarChar, prefixNM + "%");
                }
                if (chkDelFG == false)
                {
                    sql += " AND (pfc.Del_FG = 0) ";
                }

                using (DbDataReader rd = conn.GetReader(sql))
                {
                    int ordQcParamNO = rd.GetOrdinal("QcParam_NO");
                    int ordQcModelNM = rd.GetOrdinal("QcModel_NM");
                    int ordClassNM = rd.GetOrdinal("Class_NM");
                    int ordParameterNM = rd.GetOrdinal("Parameter_NM");
                    int ordModelNM = rd.GetOrdinal("Model_NM");
                    int ordPrefixNM = rd.GetOrdinal("Prefix_NM");
                    int ordHeaderNM = rd.GetOrdinal("Header_NM");
                    int ordOrderNO = rd.GetOrdinal("Order_NO");
                    int ordPlcADDR = rd.GetOrdinal("Plc_ADDR");
                    int ordDataLEN = rd.GetOrdinal("Data_LEN");
                    int ordDataTypeCD = rd.GetOrdinal("DataType_CD");
                    int ordDelFG = rd.GetOrdinal("Del_FG");
                    int ordLastUpdDT = rd.GetOrdinal("LastUpd_DT");
                    int ordIdenfifyCD = rd.GetOrdinal("Identify_CD");

                    while (rd.Read())
                    {
                        PlcFileConv pfc = new PlcFileConv();

                        
                        pfc.QcParamNO = rd.GetInt32(ordQcParamNO);
                        pfc.QcModelNM = rd.GetString(ordQcModelNM);
                        pfc.ClassNM = rd.GetString(ordClassNM);
                        pfc.ParameterNM = rd.GetString(ordParameterNM);
                        pfc.ModelNM = rd.GetString(ordModelNM);
                        pfc.PrefixNM = rd.GetString(ordPrefixNM);
                        pfc.HeaderNM = rd.GetString(ordHeaderNM);
                        pfc.OrderNO = rd.GetInt32(ordOrderNO);
                        pfc.PlcADDR = rd.GetString(ordPlcADDR);
                        pfc.DataLEN = rd.GetInt32(ordDataLEN);
                        pfc.DataTypeCD = rd.GetString(ordDataTypeCD);
                        pfc.DelFG = rd.GetBoolean(ordDelFG);
                        pfc.LastUpdDT = rd.GetDateTime(ordLastUpdDT);
                        if (rd[ordIdenfifyCD] != System.DBNull.Value)
                        {
                            pfc.IdentifyCD = rd.GetString(ordIdenfifyCD);
                        }
                        plcFileConvList.Add(pfc);
                    }
                }
            }

            return plcFileConvList;
        }

        /// <summary>
        /// データベースへの登録(更新 + 挿入)
        /// </summary>
        /// <param name="pfc"></param>
        public static void InsertUpdate(List<PlcFileConv> pfcList)
        {
            foreach (PlcFileConv p in pfcList)
            {
                InsertUpdate(p);
            }
        }


        /// <summary>
        /// データベースへの登録(更新 + 挿入)
        /// </summary>
        /// <param name="pfc"></param>
        public static void InsertUpdate(PlcFileConv pfc)
        {
            using (SqlConnection con = new SqlConnection(Constant.StrQCIL))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" UPDATE TmPlcFileConv
                                SET Header_NM = @HeaderNM, Plc_ADDR = @PlcADDR, Data_LEN = @DataLEN, DataType_CD = @DataTypeCD, 
                                    Del_FG = @DelFG, LastUpd_DT = @LastUpdDT, Identify_CD = @IdentifyCD
                                WHERE Model_NM = @ModelNM AND QcParam_NO = @QcParamNO AND Prefix_NM = @PrefixNM AND Order_NO = @OrderNO
                                INSERT INTO TmPlcFileConv (Model_NM, QcParam_NO, Prefix_NM, Header_NM, Order_NO, Plc_ADDR, Data_LEN, DataType_CD,
                                                           Del_FG, LastUpd_DT, Identify_CD)
                                SELECT @ModelNM, @QcParamNO, @PrefixNM, @HeaderNM, @OrderNO, @PlcADDR, @DataLEN, @DataTypeCD,
                                       @DelFG, @LastUpdDT, @IdentifyCD
                                WHERE NOT EXISTS 
                                (SELECT * FROM TmPlcFileConv
                                 WHERE Model_NM = @ModelNM AND QcParam_NO = @QcParamNO AND Prefix_NM = @PrefixNM AND Order_NO = @OrderNO) ";
                cmd.CommandText = sql;

                cmd.Parameters.Add("@ModelNM", SqlDbType.VarChar).Value = pfc.ModelNM;
                cmd.Parameters.Add("@QcParamNO", SqlDbType.Int).Value = pfc.QcParamNO;
                cmd.Parameters.Add("@PrefixNM", SqlDbType.VarChar).Value = pfc.PrefixNM;
                cmd.Parameters.Add("@HeaderNM", SqlDbType.VarChar).Value = pfc.HeaderNM ?? Constant.VOID_STRING;
                cmd.Parameters.Add("@OrderNO", SqlDbType.Int).Value = pfc.OrderNO;
                cmd.Parameters.Add("@PlcADDR", SqlDbType.VarChar).Value = pfc.PlcADDR;
                cmd.Parameters.Add("@DataLEN", SqlDbType.Int).Value = pfc.DataLEN;
                cmd.Parameters.Add("@DataTypeCD", SqlDbType.VarChar).Value = pfc.DataTypeCD;
                cmd.Parameters.Add("@DelFG", SqlDbType.Bit).Value = pfc.DelFG;
                cmd.Parameters.Add("@LastUpdDT", SqlDbType.DateTime).Value = System.DateTime.Now;
                cmd.Parameters.Add("@IdentifyCD", SqlDbType.VarChar).Value = pfc.IdentifyCD ?? (object)DBNull.Value; ;

                cmd.ExecuteNonQuery();
            }
        }

    }
}
