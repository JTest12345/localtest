using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.LAMS
{
    public class Work
    {
        public string TypeCd { get; set; }

        public string LotNo { get; set; }

        public DateTime LotStartDt { get; set; }

        public DateTime LotEndDt { get; set; }

        public int LotSizeCt { get; set; }

        public float WorkTm { get; set; }

        /// <summary>
        /// 指定装置の作業実績を取得
        /// </summary>
        /// <param name="plantCd">設備番号</param>
        /// <param name="typeCd">製品型番</param>
        /// <param name="recordCt">指定レコード数</param>
        /// 
        /// <returns></returns>
        public static List<Work> GetMachineRecord(string plantCd, string typeCd, int recordCt, DateTime? fromDt, DateTime? toDt)
        {
            List<Work> retv = new List<Work>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LAMSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@PlantCd", SqlDbType.NVarChar).Value = plantCd;

                string sql = "";
                if (string.IsNullOrEmpty(typeCd) == false)
                {
                    //社内型番の引数指定が有る場合、実績参照する前にまずまとめ型番を取得
                    SqlParameter paramTypeCd = cmd.Parameters.Add("@TypeCd", SqlDbType.NVarChar);

                    sql = " SELECT armstypecd FROM TmTypeGroupConv WITH(nolock) WHERE delfg = 0 AND @TypeCd Like armstypecd ";
                    cmd.CommandText = sql;

                    paramTypeCd.Value = typeCd;
                    object likeTypecd = cmd.ExecuteScalar();

                    paramTypeCd.Value = likeTypecd;
                }

                sql = @" SELECT ";
                if (recordCt != 0)
                {
                    sql += $" TOP {recordCt} ";
                }

                sql += @" typecd, lotno, lotsize, worktime, lotstart, lotend
                         FROM TnWork WITH(nolock)
                         WHERE(plantcd = @PlantCd) AND (delfg = 0) ";

                if (string.IsNullOrEmpty(typeCd) == false)
                {
                    // 社内型番の引数指定が有る場合、先で取得したまとめ型番で実績取得する
                    sql += " AND (typecd Like @TypeCd) ";
                }

                if (fromDt.HasValue)
                {
                    sql += " AND lotend < @FromDt ";
                    cmd.Parameters.Add("@FromDt", SqlDbType.DateTime).Value = fromDt.Value;
                }
                if (toDt.HasValue)
                {
                    sql += " AND lotend > @ToDt ";
                    cmd.Parameters.Add("@ToDt", SqlDbType.DateTime).Value = toDt.Value;
                }
                sql += " ORDER BY lotend DESC ";

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        Work w = new Work();
                        w.TypeCd = rd["typecd"].ToString().Trim();
                        w.LotNo = rd["lotno"].ToString().Trim();
                        w.LotSizeCt = Convert.ToInt32(rd["lotsize"]);
                        w.WorkTm = Convert.ToSingle(rd["worktime"]);
                        w.LotStartDt = Convert.ToDateTime(rd["lotstart"]);
                        w.LotEndDt = Convert.ToDateTime(rd["lotend"]);

                        retv.Add(w);
                    }
                }
            }

            return retv;
        }

        public static Work GetMachineFirstRecord(string plantCd, string typeCd)
        {
            return GetMachineRecord(plantCd, typeCd, 1).FirstOrDefault();
        }

        /// <summary>
        /// 期間内の作業実績を取得
        /// </summary>
        /// <param name="plantCd"></param>
        /// <param name="fromDt"></param>
        /// <param name="toDt"></param>
        /// <returns></returns>
        public static List<Work> GetMachineRecord(string plantCd, DateTime fromDt, DateTime toDt)
        {
            return GetMachineRecord(plantCd, null, 0, fromDt, toDt);
        }

        public static List<Work> GetMachineRecord(string plantCd, string typeCd, int recordCt)
        {
            return GetMachineRecord(plantCd, typeCd, recordCt, null, null);
        }

        public static List<Work> GetMachineRecord(string plantCd, string typeCd, int recordCt, DateTime fromDt)
        {
            return GetMachineRecord(plantCd, typeCd, recordCt, fromDt, null);
        }

        /// <summary>
        /// 装置の実績からUPH(1hr当たりの処理pcs)を取得 ※現状過去3ロット固定
        /// </summary>
        /// <param name="plantCd"></param>
        /// <param name="typeCd"></param>
        /// <returns>UPH</returns>
        public static float GetMachineUph(string plantCd, string typeCd, int sampleLotCt, int procNo)
        {
            List<Work> workList = GetMachineRecord(plantCd, typeCd, sampleLotCt);
            if (workList.Count == 0)
            {
                return GetMachineUphDefault(plantCd, procNo);
            }

            int sumLotSizeCt = workList.Sum(w => w.LotSizeCt);
            float sumWorkTm = workList.Sum(w => w.WorkTm);

            return sumLotSizeCt / (sumWorkTm / 3600);
        }

        public static float GetMachineUphDefault(string plantCd, int procNo)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LAMSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT uph 
                                FROM TmUphDefault WITH(nolock) 
                                INNER JOIN TmMachine ON TmUphDefault.model = TmMachine.model WHERE plantCd = @PlantCd AND procno = @ProcNo ";

                cmd.Parameters.Add("@PlantCd", SqlDbType.NVarChar).Value = plantCd;
                cmd.Parameters.Add("@ProcNo", SqlDbType.BigInt).Value = procNo;

                cmd.CommandText = sql;
                object uph = cmd.ExecuteScalar();
                if (uph == null)
                {
                    throw new ApplicationException($"マスタにUPHの設定無し 設備:{plantCd} 工程:{procNo}");
                }
                else
                {
                    return Convert.ToSingle(uph);
                }
            }
        }
        public static void Delete(string lotno)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LAMSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    string sql = " DELETE FROM TnWork WHERE LotNO = @LotNo";
                    cmd.CommandText = sql;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@LotNo", System.Data.SqlDbType.NVarChar).Value = lotno;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
