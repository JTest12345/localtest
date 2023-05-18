using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api
{
	public class WorkResult
	{
		public string LotNo { get; set; }
		public int ProcNo { get; set; }
		public string PlantCD { get; set; }
		public DateTime StartDt { get; set; }
		public DateTime? EndDt { get; set; }
		public bool IsCompleted { get; set; }
		public bool DelFg { get; set; }

		public static WorkResult GetData(string lotno, int procno) 
		{
			List<WorkResult> resultList = GetDatas(lotno, procno, null, false);
			if (resultList.Count == 0)
			{
				return null;
			}
			else 
			{
				return resultList.Single();
			}
		}
		public static List<WorkResult> GetDatas(string lotno) 
		{
			return GetDatas(lotno, null, null, false);
		}
		public static List<WorkResult> GetDatas(string lotno, int? procno, string plantcd, bool isTargetOnlyComplete)
		{
			List<WorkResult> retv = new List<WorkResult>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LensConnectionString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT LotNO, ProcNO, PlantCD, StartDT, EndDT, DelFG, IsCompleted, LastupdDT
								FROM TnTran WITH (nolock) 
								WHERE (DelFG = 0) ";

				if (!string.IsNullOrEmpty(lotno))
				{
					sql += " AND LotNO = @LotNO ";
					cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotno;
				}

				if (procno.HasValue)
				{
					sql += " AND ProcNO = @ProcNO ";
					cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = procno;
				}

				if (!string.IsNullOrWhiteSpace(plantcd))
				{
					sql += " AND PlantCD = @PlantCD ";
					cmd.Parameters.Add("@PlantCD", SqlDbType.NVarChar).Value = plantcd;
				}

				if (isTargetOnlyComplete)
				{
					sql += " AND EndDT IS NOT NULL ";
				}


				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					int ordEndDt = rd.GetOrdinal("EndDT");
					while (rd.Read())
					{
						WorkResult r = new WorkResult();
						r.LotNo = rd["LotNO"].ToString().Trim();
						r.ProcNo = Convert.ToInt32(rd["ProcNO"]);
						r.PlantCD = rd["PlantCD"].ToString().Trim();
						r.StartDt = Convert.ToDateTime(rd["StartDT"]);
						if (!rd.IsDBNull(ordEndDt))
						{
							r.EndDt = Convert.ToDateTime(rd[ordEndDt]);
						}
						r.IsCompleted = Convert.ToBoolean(rd["IsCompleted"]);
						retv.Add(r);
					}
				}
			}

			return retv;
		}

		public static bool IsCompleteStartProcess(string lotno, int procno) 
		{
			WorkResult result = GetData(lotno, procno);
			if (result == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool IsCompleteEndProcess(string lotno, int procno)
		{
			WorkResult result = GetData(lotno, procno);
			if (result == null)
			{
				return false;
			}
			else
			{
				return result.IsCompleted;
			}
		}

		public static void CompleteStartProcess(string lotno, int procno, string plantcd) 
		{
			WorkResult r = new WorkResult();
			r.LotNo = lotno;
			r.ProcNo = procno;
			r.PlantCD = plantcd;
			r.StartDt = System.DateTime.Now;
			r.EndDt = null;
			r.IsCompleted = false;

			r.InsertUpdate();
		}

		public static void CompleteEndProcess(string lotno, int procno, string plantcd)
		{
			WorkResult r = WorkResult.GetData(lotno, procno);
			if (r == null)
			{
				r = new WorkResult();
				r.LotNo = lotno;
				r.ProcNo = procno;
				r.PlantCD = plantcd;
				r.StartDt = System.DateTime.Now;
			}

			r.EndDt = System.DateTime.Now;
			r.IsCompleted = true;

			r.InsertUpdate();
		}

		private void InsertUpdate()
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.LensConnectionString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" UPDATE TnTran 
									SET PlantCD = @PlantCD, 
									StartDT = @StartDT, 
									EndDT = @EndDT,
									IsCompleted = @IsCompleted, 
									DelFg = @DelFg,
									LastupdDT = @LastupdDT 
									WHERE LotNO = @LotNO AND ProcNO = @ProcNO 
								INSERT INTO TnTran (LotNO, ProcNO, PlantCD, StartDT, EndDT, IsCompleted)
									SELECT @LotNO, @ProcNO, @PlantCD, @StartDT, @EndDT, @IsCompleted
									WHERE NOT EXISTS (SELECT * FROM TnTran WHERE LotNO = @LotNO AND ProcNO = @ProcNO) ";

				cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = this.LotNo;
				cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = this.ProcNo;
				cmd.Parameters.Add("@PlantCD", SqlDbType.NVarChar).Value = this.PlantCD;
				cmd.Parameters.Add("@StartDT", SqlDbType.DateTime).Value = this.StartDt;
				cmd.Parameters.Add("@EndDT", SqlDbType.DateTime).Value = this.EndDt ?? (object)DBNull.Value;
				cmd.Parameters.Add("@IsCompleted", SqlDbType.Int).Value = this.IsCompleted;
				cmd.Parameters.Add("@DelFG", SqlDbType.Int).Value = this.DelFg;
				cmd.Parameters.Add("@LastupdDT", SqlDbType.DateTime).Value = System.DateTime.Now;

				cmd.CommandText = sql;
				cmd.ExecuteNonQuery();
			}
		}

		//public static void SetCompleteMapping(string lotno, int procno) 
		//{
		//	// 2014/10/2 ARMS実績系テーブルはARMS

		//	using (SqlConnection con = new SqlConnection(Config.Settings.NamiConnectionString))
		//	using (SqlCommand cmd = con.CreateCommand())
		//	{
		//		con.Open();

		//		string sql = " UPDATE TnTran SET IsMappingCompleted = 1 WHERE (lotno = @LotNO AND procno = @ProcNO) ";

		//		cmd.Parameters.Add("LotNO", SqlDbType.NVarChar).Value = lotno;
		//		cmd.Parameters.Add("ProcNO", SqlDbType.BigInt).Value = procno;

		//		cmd.CommandText = sql;

		//		try
		//		{
		//			cmd.ExecuteNonQuery();
		//		}
		//		catch(Exception err)
		//		{
		//			throw new ApplicationException(
		//				string.Format("マッピング処理完了フラグの更新に失敗しました。ロット番号:{0} 作業番号:{1}", lotno, procno), err);
		//		}
		//	}
		//}
	}
}
