using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.LENS
{
    public class MapResult
    {
        public string LotNo { get; set; }
        public string CarrierNo { get; set; }
        public string NewCarrierNo { get; set; }
        public int ProcNo { get; set; }
        public string ResultValue { get; set; }
        public bool OutputFg { get; set; }
        public string UpdUserCd { get; set; }
		public bool DividedFg { get; set; }

        public static List<MapResult> GetCurrentData(string lotNo, int? procNo, string carrierNo, bool outputFg)
        {
            List<MapResult> retv = new List<MapResult>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

				string sql = @" SELECT LotNo, ProcNo, CarrierNo, OutputFg, ResultValue, DividedFG
								FROM TnMapResult WITH (nolock) 
								WHERE LotNO = @LotNO AND CarrierNO = @CarrierNO 
                                AND OutputFG = @OutputFG AND NewFG = 1 ";

                if (procNo.HasValue)
                {
                    sql += " AND ProcNO = @ProcNO ";
                    cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = procNo;
                }

                cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotNo;
                cmd.Parameters.Add("@CarrierNO", SqlDbType.NVarChar).Value = carrierNo;
                cmd.Parameters.Add("@OutputFG", SqlDbType.Int).Value = SQLite.SerializeBool(outputFg);

                cmd.CommandText = sql;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MapResult r = new MapResult();

                        r.LotNo = reader["LotNo"].ToString().Trim();
                        r.ProcNo = Convert.ToInt32(reader["ProcNo"]);
                        r.CarrierNo = reader["CarrierNo"].ToString().Trim();
                        r.ResultValue = reader["ResultValue"].ToString().Trim();
						r.DividedFg = Convert.ToBoolean(reader["DividedFG"]);

                        retv.Add(r);
                    }
                }
            }

            return retv;
        }
        public static List<MapResult> GetCurrentOutputData(string lotNo, string carrierNo)
        {
            return GetCurrentData(lotNo, null, carrierNo, true);
        }
        public static MapResult GetCurrentProcOutputData(string lotNo, int procNo, string carrierNo)
        {
            List<MapResult> mapList = GetCurrentData(lotNo, procNo, carrierNo, true);
            if (mapList.Count == 0)
            {
                throw new ApplicationException("");
            }
            else if (mapList.Count == 1)
            {
                return mapList[0];
            }
            else
            {
                throw new ApplicationException("");
            }
        }

        public static void Insert(SqlCommand cmd, string lotNo, string carrierNo, int procNo, string resultValue, bool outputFg, bool newFg, bool? dividedFg, string empCD)
        {
			if (dividedFg.HasValue == false)
			{
				MapResult mapResult = GetCurrentProcOutputData(lotNo, procNo, carrierNo);
				dividedFg = mapResult.DividedFg;
			}

            int maxId = getMaxID(cmd, lotNo, carrierNo, procNo);

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@HistoryID", SqlDbType.Int).Value = maxId;
            cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotNo;
            cmd.Parameters.Add("@CarrierNO", SqlDbType.NVarChar).Value = carrierNo;
            cmd.Parameters.Add("@ProcNO", SqlDbType.NVarChar).Value = procNo;
            cmd.Parameters.Add("@ResultValue", SqlDbType.NVarChar).Value = resultValue;
            cmd.Parameters.Add("@OutputFG", SqlDbType.NVarChar).Value = SQLite.ParseInt(outputFg);
			cmd.Parameters.Add("@DividedFG", SqlDbType.Int).Value = SQLite.ParseInt(dividedFg);
			cmd.Parameters.Add("@NewFG", SqlDbType.Int).Value = SQLite.ParseInt(newFg);
			cmd.Parameters.Add("@UpdUserCD", SqlDbType.NVarChar).Value = empCD;
			cmd.Parameters.Add("@LastUpdDT", SqlDbType.DateTime).Value = DateTime.Now;

			string sql = @" INSERT INTO TnMapResult
								(HistoryID,  LotNO,  CarrierNO,  ProcNO,  ResultValue,  OutputFG,  UpdUserCD,  NewFG, ThrowFG, DividedFG, LastUpdDT) 
						 VALUES(@HistoryID, @LotNO, @CarrierNO, @ProcNO, @ResultValue, @OutputFG, @UpdUserCD, @NewFG, 0, @DividedFG, @LastUpdDT) ";
            cmd.CommandText = sql;

            cmd.ExecuteNonQuery();
        }

		public static void Update(SqlCommand cmd, string lotNo, string carrierNo, int procNo, bool outputFg, bool newFg, string empCD)
		{
			cmd.Parameters.Clear();
			cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotNo;
			cmd.Parameters.Add("@CarrierNO", SqlDbType.NVarChar).Value = carrierNo;
			cmd.Parameters.Add("@ProcNO", SqlDbType.NVarChar).Value = procNo;
			cmd.Parameters.Add("@OutputFG", SqlDbType.NVarChar).Value = SQLite.ParseInt(outputFg);
			cmd.Parameters.Add("@NewFG", SqlDbType.Int).Value = SQLite.ParseInt(newFg);
			cmd.Parameters.Add("@UpdUserCD", SqlDbType.NVarChar).Value = empCD;
			cmd.Parameters.Add("@LastUpdDT", SqlDbType.DateTime).Value = DateTime.Now;

			string sql = @" UPDATE TnMapResult SET
								UpdUserCD = @UpdUserCD,
								NewFG = @NewFG,
								LastUpdDT = @LastUpdDT
							WHERE (	LotNO = @LotNO AND
									CarrierNO = @CarrierNO AND
									ProcNO = @ProcNO AND
									OutputFG = @OutputFG AND
									NewFG = 1 ) ";
			cmd.CommandText = sql;

			cmd.ExecuteNonQuery();
		}

        public static void Delete(string lotno)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    string sql = " DELETE FROM TnMapResult WHERE LotNO = @LotNo";
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

        private static int getMaxID(SqlCommand cmd, string lotno, string carrierno, int procno)
        {
            try
            {
                string sql = @" SELECT MAX(HistoryID) AS MaxHistoryID
							FROM TnMapResult WITH (nolock) 
							WHERE LotNO = @LotNO AND ProcNO = @ProcNO AND CarrierNO = @CarrierNO ";

                cmd.Parameters.Clear();
                cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = procno;
                cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotno;
                cmd.Parameters.Add("@CarrierNO", SqlDbType.NVarChar).Value = carrierno;

                cmd.CommandText = sql;

                object maxHistoryID = cmd.ExecuteScalar();

                if (string.IsNullOrEmpty(Convert.ToString(maxHistoryID)))
                {
                    return 1;
                }
                else
                {
                    return Convert.ToInt32(maxHistoryID) + 1;
                }
            }
            catch(Exception ex)
            {
                cmd.Transaction.Rollback();
                return 0;
            }
        }
        private static int getMaxID(string lotno, string carrierno, int procno) 
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                return getMaxID(cmd, lotno, carrierno, procno);
            }
        }
    }
}
