using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data;
using System.Data.Common;

namespace EICS.Database.LENS
{
    public class Type
    {
        public string TypeCD { get; set; }
        public int FileFmtNO { get; set; }
        public int MagazineID { get; set; }
        
        public static Type Get(string typeCd, int hostLineCd)
        {
            List<Type> retv = GetData(typeCd, hostLineCd);
            if (retv.Count == 0)
            {
                return null;
            }
            else
            {
                return retv.Single();
            }
        }
        public static List<Type> GetData(string typeCd, int hostLineCd)
        {
            List<Type> retv = new List<Type>();

            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.LENS, hostLineCd), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT TypeCD, FileFmtNO, MagazineID
                                FROM TmType WITH(nolock) WHERE DelFG = 0 ";

                if (!string.IsNullOrEmpty(typeCd)) 
                {
                    sql += " AND (TypeCD = @TypeCD) ";
                    conn.SetParameter("@TypeCD", SqlDbType.Char, typeCd);
                }

                conn.Command.CommandText = sql;

                using (DbDataReader rd = conn.GetReader(sql)) 
                {
                    while (rd.Read()) 
                    {
                        Type t = new Type();
                        t.TypeCD = rd["TypeCD"].ToString().Trim();
                        t.FileFmtNO = Convert.ToInt32(rd["FileFmtNO"]);
                        t.MagazineID = Convert.ToInt32(rd["MagazineID"]);
                        retv.Add(t);
                    }
                }
            }

            return retv;
        }
    }
}