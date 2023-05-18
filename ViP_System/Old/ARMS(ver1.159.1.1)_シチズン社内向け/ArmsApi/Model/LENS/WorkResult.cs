using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.LENS
{
	public class WorkResult
	{
		public string LotNo { get; set; }
		public int ProcNo { get; set; }
		public DateTime MachineLogLastUpdDT { get; set; }
		public string PlantCd { get; set; }
		public DateTime StartDt { get; set; }
		public DateTime? EndDt { get; set; }
		public bool IsMappingCompleted { get; set; }


		public bool IsCompleted { get; set; }
		public bool DelFg { get; set; }
		public string CarrierNo { get; set; }

		public WorkResult()
		{
		}

		public WorkResult(string lotno, int procno, string carrierno)
		{
			this.LotNo = lotno;
			this.ProcNo = procno;
			this.CarrierNo = carrierno;
		}

		private static WorkResult getWorkData(string lotno, int procno)
		{
			List<WorkResult> resultList = getData(lotno, procno);
			if (resultList.Count == 0)
			{
				return null;
				//throw new ArmsException(string.Format("LENSの実績が存在しません。ロット番号:{0} 工程番号:{1}", lotno, procno));
			}
			else
			{
				return resultList.Single();
			}
		}

		private static List<WorkResult> getData(string lotno, int procno)
		{
			List<WorkResult> retv = new List<WorkResult>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT LotNO, ProcNO, PlantCD, StartDT, EndDT, DelFG, IsCompleted, LastupdDT
								FROM TnTran WITH (nolock) 
								WHERE (DelFG = 0) AND LotNO = @LotNO AND ProcNO = @ProcNO ";

				cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotno;
				cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = procno;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					int ordEndDt = rd.GetOrdinal("EndDT");
					while (rd.Read())
					{
						WorkResult r = new WorkResult();
						r.LotNo = rd["LotNO"].ToString().Trim();
						r.ProcNo = Convert.ToInt32(rd["ProcNO"]);
						r.PlantCd = rd["PlantCD"].ToString().Trim();
						r.StartDt = Convert.ToDateTime(rd["StartDT"]);
						if (!rd.IsDBNull(ordEndDt))
						{
							r.EndDt = Convert.ToDateTime(rd[ordEndDt]);
						}
						r.IsMappingCompleted = Convert.ToBoolean(rd["IsCompleted"]);
						retv.Add(r);
					}
				}
			}

			return retv;
		}

		public static bool IsComplete(string lotno, int procno)
		{
			WorkResult result = getWorkData(lotno, procno);
			if (result == null)
			{
				return false;
			}
			else
			{
				return result.IsMappingCompleted;
			}
		}

//		public void UpdateCarrierNo(ref SqlCommand cmd, string newCarrierNo)
//		{
//			string sql = @" UPDATE TnTran 
//								SET CarrierNo = @CarrierNo,
//								LastupdDT = @LastupdDT 
//								WHERE LotNO = @LotNO AND ProcNO = @ProcNO AND CarrierNo = @OldCarrierNo ";


//			cmd.Parameters.Clear();
//			cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = this.LotNo;
//			cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = this.ProcNo;
//			cmd.Parameters.Add("@LastupdDT", SqlDbType.DateTime).Value = System.DateTime.Now;
//			cmd.Parameters.Add("@CarrierNo", SqlDbType.NVarChar).Value = newCarrierNo;
//			cmd.Parameters.Add("@OldCarrierNo", SqlDbType.NVarChar).Value = this.CarrierNo;

//			cmd.CommandText = sql;
//			cmd.ExecuteNonQuery();
//		}

		public static void ChangeCarrier(ref SqlCommand cmd, string lotno, string orgCarrierNo, string newCarrierNo, int procno)
		{
			string sql = @" UPDATE TnTran 
							SET CarrierNO = @NewCarrierNO,
							LastUpdDT = @LastUpdDT
							WHERE LotNO = @LotNO AND CarrierNo = @OrgCarrierNO AND ProcNO = @ProcNO";

			cmd.Parameters.Clear();
			cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotno;
			cmd.Parameters.Add("@OrgCarrierNO", SqlDbType.NVarChar).Value = orgCarrierNo;
			cmd.Parameters.Add("@NewCarrierNO", SqlDbType.NVarChar).Value = newCarrierNo;
			cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = procno;
			cmd.Parameters.Add("@LastUpdDT", SqlDbType.DateTime).Value = DateTime.Now;

			if (string.IsNullOrEmpty(orgCarrierNo) && string.IsNullOrEmpty(newCarrierNo))
			{
				throw new ApplicationException(string.Format(
					"元キャリアNo、先キャリアNoいずれかが未設定で機能が呼び出されました。"));
			}

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
                    string sql = " DELETE FROM TnTran WHERE LotNO = @LotNo";
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

        public void Insert(ref SqlCommand cmd)
		{
			string sql = string.Empty;
			string updSql = @" UPDATE TnTran 
								SET StartDT = @StartDT, 
								EndDT = @EndDT,
								IsCompleted = @IsCompleted, 
								DelFg = @DelFg,
								LastupdDT = @LastupdDT 
								WHERE LotNO = @LotNO AND ProcNO = @ProcNO AND PlantCD = @PlantCD ";

			string insertSql = @" INSERT INTO TnTran (LotNO, ProcNO, PlantCD, StartDT, EndDT, IsCompleted {0}) ";

			string insertSelectSql = @" SELECT @LotNO, @ProcNO, @PlantCD, @StartDT, @EndDT, @IsCompleted ";

			string insertWhereSql =
			@" WHERE NOT EXISTS (SELECT * FROM TnTran WHERE LotNO = @LotNO AND ProcNO = @ProcNO AND PlantCD = @PlantCD {0}) ";

			cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = this.LotNo;
			cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = this.ProcNo;

			cmd.Parameters.Add("@PlantCD", SqlDbType.NVarChar).Value = this.PlantCd;
			cmd.Parameters.Add("@StartDT", SqlDbType.DateTime).Value = this.StartDt;
			cmd.Parameters.Add("@EndDT", SqlDbType.DateTime).Value = this.EndDt ?? (object)DBNull.Value;
			cmd.Parameters.Add("@IsCompleted", SqlDbType.Int).Value = this.IsCompleted;
			cmd.Parameters.Add("@DelFG", SqlDbType.Int).Value = this.DelFg;
			cmd.Parameters.Add("@LastupdDT", SqlDbType.DateTime).Value = System.DateTime.Now;

			if (!string.IsNullOrEmpty(this.CarrierNo))
			{
				updSql += " AND CarrierNo = @CarrierNo ";
				insertSql = string.Format(insertSql, ", CarrierNo");
				insertSelectSql += ", @CarrierNo";
				insertWhereSql = string.Format(insertWhereSql, "AND CarrierNo = @CarrierNo");

				cmd.Parameters.Add("@CarrierNo", SqlDbType.NVarChar).Value = this.CarrierNo;
			}
			else
			{
				insertSql = string.Format(insertSql, "");
				insertWhereSql = string.Format(insertWhereSql, "");
			}

			sql = updSql + insertSql + insertSelectSql + insertWhereSql;

			cmd.CommandText = sql;
			cmd.ExecuteNonQuery();
		}

		public static List<WorkResult> GetData(string lotno, int procno, string carrierNo)
		{
			List<WorkResult> retv = new List<WorkResult>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string selectSql = @" SELECT LotNO, ProcNO, PlantCD, StartDT, EndDT, DelFG, IsCompleted, LastupdDT ";
				string fromSql = @" FROM TnTran WITH (nolock) ";
				string whereSql = @" WHERE (DelFG = 0) and ProcNO = @ProcNO ";

				cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = procno;

				if (!string.IsNullOrEmpty(lotno))
				{
					whereSql += " AND LotNO = @LotNO ";
					cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotno;
				}

				if (!string.IsNullOrEmpty(carrierNo))
				{
					selectSql += ", CarrierNO ";
					whereSql += " AND CarrierNo = @CarrierNo ";
					cmd.Parameters.Add("@CarrierNo", SqlDbType.NVarChar).Value = carrierNo;
				}

				cmd.CommandText = selectSql + fromSql + whereSql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					int ordEndDt = rd.GetOrdinal("EndDT");
					while (rd.Read())
					{
						WorkResult r = new WorkResult();
						r.LotNo = rd["LotNO"].ToString().Trim();
						r.ProcNo = Convert.ToInt32(rd["ProcNO"]);
						r.PlantCd = rd["PlantCD"].ToString().Trim();
						r.StartDt = Convert.ToDateTime(rd["StartDT"]);
						if (!rd.IsDBNull(ordEndDt))
						{
							r.EndDt = Convert.ToDateTime(rd[ordEndDt]);
						}
						r.IsCompleted = Convert.ToBoolean(rd["IsCompleted"]);
						if (!string.IsNullOrEmpty(carrierNo))
						{
							r.CarrierNo = rd["CarrierNO"].ToString().Trim();
						}
						retv.Add(r);
					}
				}
			}

			return retv;
		}
	}
}
