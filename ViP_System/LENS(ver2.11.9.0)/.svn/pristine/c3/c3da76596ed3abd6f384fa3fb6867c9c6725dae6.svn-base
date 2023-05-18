using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api
{
	public class Lot
	{
		public string LotNo { get; set; }
		public string TypeCd { get; set; }

		/// <summary>全数検査対象フラグ</summary>
		public bool IsFullSizeInspection { get; set; }

		/// <summary>マッピング検査フラグ</summary>
		public bool IsMappingInspection { get; set; }

		/// <summary>変化点フラグ</summary>
		public bool IsChangePoint { get; set; }

		/// <summary>過去に外観検査機を通過している</summary>
		public bool HasPassedInspector { get; set; }

		/// <summary>検査後照合の結果　0：検査結果未判定　1：検査NG　2：検査OK</summary>
		public int InspectionResultCD { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="magazineNo">マガジン番号(ロット番号)</param>
		/// <returns></returns>
		public static Lot GetData(string magazineNo)
		{
			Lot retv = null;

			string lotNo = string.Empty;

			string[] magChars = magazineNo.Split(' ');
			if (magChars.Count() == 1)
			{
				//ロット番号が渡された事を想定
				lotNo = magazineNo;
			}
			else if (magChars[0] == Magazine.MAGAZINE_LABEL_IDENTIFIER)
			{
				//マガジン番号(QRコード形式)が渡された事を想定
				throw new NotImplementedException();
			}
			else 
			{
				//ロット番号(QRコード形式)が渡された事を想定
				lotNo = magChars[1];		
			}

			using (SqlConnection con = new SqlConnection(Config.Settings.LensConnectionString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT LotNO, TypeCD, IsFullsizeInspection, IsMappingInspection, IsChangePoint, InspectionResultCD
								FROM TnLot WITH(nolock) 
								WHERE LotNO = @LotNO ";

				cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotNo;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						retv = new Lot();
						retv.LotNo = rd["LotNO"].ToString().Trim();
						retv.TypeCd = rd["TypeCD"].ToString().Trim();
						retv.IsFullSizeInspection = Convert.ToBoolean(rd["IsFullsizeInspection"]);
						retv.IsMappingInspection = Convert.ToBoolean(rd["IsMappingInspection"]);
						retv.IsChangePoint = Convert.ToBoolean(rd["IsChangePoint"]);
						retv.InspectionResultCD = Convert.ToInt32(rd["InspectionResultCD"]);
						retv.HasPassedInspector = ARMS.WorkResult.HasPassedProcess(retv.LotNo, Config.Settings.InspectProcNo, null);
					}
				}
			}

			return retv;
		}

		public void InsertUpdate() 
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.LensConnectionString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" UPDATE TnLot 
									SET TypeCD = @TypeCD, 
									IsFullsizeInspection = @IsFullsizeInspection, 
									IsMappingInspection = @IsMappingInspection,
									IsChangePoint = @IsChangePoint, 
									LastupdDT = @LastupdDT,
									InspectionResultCD = @InspectionResultCD 
									WHERE LotNO = @LotNO 
								INSERT INTO TnLot (LotNO, TypeCD, IsFullsizeInspection, IsMappingInspection, IsChangePoint, InspectionResultCD)
									SELECT @LotNO, @TypeCD, @IsFullsizeInspection, @IsMappingInspection, @IsChangePoint, @InspectionResultCD
									WHERE NOT EXISTS (SELECT * FROM TnLot WHERE LotNO = @LotNO) ";

				cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = this.LotNo;
				cmd.Parameters.Add("@TypeCD", SqlDbType.NVarChar).Value = this.TypeCd;
				cmd.Parameters.Add("@IsFullsizeInspection", SqlDbType.Int).Value = this.IsFullSizeInspection;
				cmd.Parameters.Add("@IsMappingInspection", SqlDbType.Int).Value = this.IsMappingInspection;
				cmd.Parameters.Add("@IsChangePoint", SqlDbType.Int).Value = this.IsChangePoint;
				cmd.Parameters.Add("@LastupdDT", SqlDbType.DateTime).Value = System.DateTime.Now;
				cmd.Parameters.Add("@InspectionResultCD", SqlDbType.Int).Value = this.InspectionResultCD;

				cmd.CommandText = sql;
				cmd.ExecuteNonQuery();
			}			
		}

		public static bool IsExecuteInspectionResultJudgment(string magazineNo)
		{
			if (GetData(magazineNo).InspectionResultCD == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
