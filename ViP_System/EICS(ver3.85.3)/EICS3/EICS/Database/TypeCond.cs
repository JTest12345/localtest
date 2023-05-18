using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data.Common;

namespace EICS.Database
{
    class TypeCond
    {
        public string ModelNM { get; set; }
        public string MaterialCD { get; set; }
        public int CondCD { get; set; }
        public string CondVAL { get; set; }
        public bool DelFG { get; set; }
        public string UpdUserCD { get; set; }
        //public DateTime LastUpdDT { get; set; }

        public static List<TypeCond> GetTypeCond(int lineCD, string modelNM, string materialCD, int condCD)
        {
            List<TypeCond> typeCondList = new List<TypeCond>();

            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT Model_NM, Material_CD, Cond_CD, Cond_VAL, Del_FG, UpdUser_CD, LastUpd_DT
                                FROM TmTypeCond with(nolock) 
                                WHERE Model_NM = @ModelNM and Material_CD = @MaterialCD and Cond_CD = @CondCD 
								OPTION (MAXDOP 1) ";

                conn.SetParameter("@ModelNM", System.Data.SqlDbType.NVarChar, modelNM);
                conn.SetParameter("@MaterialCD", System.Data.SqlDbType.Char, materialCD);
                conn.SetParameter("@CondCD", System.Data.SqlDbType.Int, condCD);

                using (DbDataReader rd = conn.GetReader(sql))
                {
                    int ordModelNM = rd.GetOrdinal("Model_NM");
                    int ordMaterialCD = rd.GetOrdinal("Material_CD");
                    int ordCondCD = rd.GetOrdinal("Cond_CD");
                    int ordCondVAL = rd.GetOrdinal("Cond_VAL");
                    int ordDelFG = rd.GetOrdinal("Del_FG");
                    int ordUpdUserCD = rd.GetOrdinal("UpdUser_CD");
                    int ordLastUpdDT = rd.GetOrdinal("LastUpd_DT");

                    while (rd.Read())
                    {
                        TypeCond typeCond = new TypeCond();

                        typeCond.ModelNM = rd.GetString(ordModelNM);
                        typeCond.MaterialCD = rd.GetString(ordMaterialCD);
                        typeCond.CondCD = rd.GetInt32(ordCondCD);
                        typeCond.CondVAL = rd.GetString(ordCondVAL);
                        typeCond.DelFG = rd.GetBoolean(ordDelFG);
                        typeCond.UpdUserCD = rd.GetString(ordUpdUserCD);

                        typeCondList.Add(typeCond);
                    }
                }                
            }

            return typeCondList;
        }

    }
}
