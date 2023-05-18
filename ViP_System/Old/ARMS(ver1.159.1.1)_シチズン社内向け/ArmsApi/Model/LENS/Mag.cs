using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ArmsApi.Model.LENS
{
    public class Mag
    {
        public int PackageQtyX { get; set; }
        public int PackageQtyY { get; set; }


        public static int? GetMagStep(string type)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @"SELECT m.Step
                        From TmType t
                        INNER JOIN TmMag m
                        on t.MagazineID = m.MagazineID
                        where t.TypeCD = @TYPECD";
                cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = type;

                return SQLite.ParseNullableInt(cmd.ExecuteScalar());
            }
        }

        public static int? GetMagStepCd(string type)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @"SELECT m.LoadStepCD
                        From TmType t
                        INNER JOIN TmMag m
                        on t.MagazineID = m.MagazineID
                        where t.TypeCD = @TYPECD";
                cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = type;

                return SQLite.ParseNullableInt(cmd.ExecuteScalar());
            }
        }


        /// <summary>
        /// マガジン構成を取得
        /// </summary>
        /// <param name="typeCD"></param>
        public static Mag GetData(string typecd)
        {
            List<Mag> mags = getDatas(typecd);
            if (mags.Count == 0) { return null; }
            else
            {
                return mags.Single();
            }
        }

        private static List<Mag> getDatas(string typecd)
        {
            List<Mag> retv = new List<Mag>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT FrameXPackage, FrameYPackage
									FROM TmMag WITH (nolock) 
									INNER JOIN TmType WITH (nolock) ON TmMag.MagazineID = TmType.MagazineID 
									WHERE (TmType.DelFG = 0) AND (TmMag.DelFG = 0) ";

                if (!string.IsNullOrEmpty(typecd))
                {
                    sql += " AND (TmType.TypeCD = @TypeCD) ";
                    cmd.Parameters.Add("@TypeCD", System.Data.SqlDbType.Char).Value = typecd;
                }

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        Mag m = new Mag();

                        m.PackageQtyX = Convert.ToInt32(rd["FrameXPackage"]);
                        m.PackageQtyY = Convert.ToInt32(rd["FrameYPackage"]);

                        retv.Add(m);
                    }
                }
            }
            return retv;
        }
    }
}
