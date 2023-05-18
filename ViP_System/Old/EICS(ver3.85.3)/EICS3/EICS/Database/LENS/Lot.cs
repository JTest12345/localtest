using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using SLCommonLib.DataBase;
using System.Data.Common;

namespace EICS.Database.LENS
{
	class Lot
	{
		public string LotNo { get; set; }
		public string TypeCd { get; set; }

		/// <summary>マッピング検査フラグ</summary>
		public bool IsMappingInspection { get; set; }

		/// <summary>検査後照合の結果　0：検査結果未判定　1：検査NG　2：検査OK</summary>
		public int InspectionResultCD { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="magazineNo">マガジン番号(ロット番号)</param>
		/// <returns></returns>
		public static Lot GetData(string magazineNo, int hostLineCd)
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

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.LENS, hostLineCd), "System.Data.SqlClient", false))
			{

				string sql = @" SELECT LotNO, TypeCD, IsMappingInspection, InspectionResultCD
								FROM TnLot WITH(nolock) 
								WHERE LotNO = @LotNO ";

				conn.SetParameter("@LotNO", SqlDbType.NVarChar, lotNo);

				conn.Command.CommandText = sql;
				using (DbDataReader rd = conn.GetReader(sql))
				{
					while (rd.Read())
					{
						retv = new Lot();
						retv.LotNo = rd["LotNO"].ToString().Trim();
						retv.TypeCd = rd["TypeCD"].ToString().Trim();
						retv.IsMappingInspection = Convert.ToBoolean(rd["IsMappingInspection"]);
						retv.InspectionResultCD = Convert.ToInt32(rd["InspectionResultCD"]);
					}
				}
			}

			return retv;
		}

		public static bool IsExecuteInspectionResultJudgment(string magazineNo, int hostLineCd)
		{
			if (GetData(magazineNo, hostLineCd).InspectionResultCD == 0)
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
