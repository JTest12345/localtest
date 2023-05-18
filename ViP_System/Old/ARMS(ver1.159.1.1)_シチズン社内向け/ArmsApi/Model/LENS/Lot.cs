using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.LENS
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

		public Lot(string lotno)
		{
			this.LotNo = lotno;
		}

		public static Lot GetData(string lotno)
		{
			Lot retv = null;

			using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT LotNO, TypeCD, IsFullsizeInspection, IsMappingInspection, IsChangePoint
								FROM TnLot WITH(nolock) 
								WHERE LotNO = @LotNO ";

				cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotno;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						retv = new Lot(lotno);
						retv.TypeCd = rd["TypeCD"].ToString().Trim();
						retv.IsFullSizeInspection = Convert.ToBoolean(rd["IsFullsizeInspection"]);
						retv.IsMappingInspection = Convert.ToBoolean(rd["IsMappingInspection"]);
						retv.IsChangePoint = Convert.ToBoolean(rd["IsChangePoint"]);
					}
				}
			}

			return retv;
		}
        public static void Delete(string lotno)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    string sql = " DELETE FROM TnLot WHERE LotNO = @LotNo";
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
        public void InsertUpdate()
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();
				try
				{
					string sql = @" UPDATE TnLot 
										SET TypeCD =@TypeCD,
                                            IsFullsizeInspection = @IsFullsizeInspection, 
											IsMappingInspection = @IsMappingInspection,
											IsChangePoint = @IsChangePoint, 
											LastupdDT = @LastupdDT 
										WHERE LotNO = @LotNO 
									INSERT INTO TnLot (LotNO, TypeCD, IsFullsizeInspection, IsMappingInspection, IsChangePoint)
										SELECT @LotNO, @TypeCD, @IsFullsizeInspection, @IsMappingInspection, @IsChangePoint
										WHERE NOT EXISTS (SELECT * FROM TnLot WHERE LotNO = @LotNO) ";

					cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = this.LotNo;
					cmd.Parameters.Add("@TypeCD", SqlDbType.NVarChar).Value = this.TypeCd;
					cmd.Parameters.Add("@IsFullsizeInspection", SqlDbType.Int).Value = this.IsFullSizeInspection;
					cmd.Parameters.Add("@IsMappingInspection", SqlDbType.Int).Value = this.IsMappingInspection;
					cmd.Parameters.Add("@IsChangePoint", SqlDbType.Int).Value = this.IsChangePoint;
					cmd.Parameters.Add("@LastupdDT", SqlDbType.DateTime).Value = System.DateTime.Now;

					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
				catch (Exception err)
				{
					throw new ArmsException(string.Format("LENSのロット情報更新失敗。ロット番号:{0}", this.LotNo), err);
				}
			}
		}
	}
}
