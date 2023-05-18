using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class OvenProfile
    {
        /// <summary>
        /// オーブンのプロファイル取得
        /// </summary>
        /// <param name="magazineNo"></param>
        /// <returns></returns>
        public static int GetOvenProfileId(string magazineNo)
        {
            Magazine mag = Magazine.GetCurrent(magazineNo);
            AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
            if (mag == null) throw new ArmsException("マガジン情報が見つかりません:" + magazineNo);
            if (lot == null) throw new ArmsException("ロット情報が見つかりません：" + mag.NascaLotNO);
            Process next = Process.GetNextProcess(mag.NowCompProcess, lot);
            if (next == null) throw new ArmsException("次工程情報が見つかりません:" + mag.MagazineNo);

            return GetOvenProfileId(magazineNo, next.ProcNo);
        }

//		/// <summary>
//		/// オーブンのプロファイル取得
//		/// 2015.7.28 オーブン硬化条件取得方法変更
//		/// </summary>
//		/// <param name="magazineNo"></param>
//		/// <param name="procno"></param>
//		/// <returns></returns>
//		public static int GetOvenProfileId(string magazineNo, int procno)
//		{
//			Magazine mag = Magazine.GetCurrent(magazineNo);
//			AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
//			if (mag == null) throw new ArmsException("マガジン情報が見つかりません:" + magazineNo);
//			if (lot == null) throw new ArmsException("ロット情報が見つかりません：" + mag.NascaLotNO);

//			Process p = Process.GetProcess(procno);

//			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
//			using (SqlCommand cmd = con.CreateCommand())
//			{

//				con.Open();
//				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = lot.TypeCd;
//				cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = p.WorkCd;
//				cmd.CommandText = @" SELECT condval
//									FROM TmTypeCond WITH (nolock)
//									WHERE (typecd = @TYPECD) AND (condcd = 'OVENPROF') AND (workcd = @WORKCD) AND (delfg = 0) ";

//				object retv = cmd.ExecuteScalar();

//				if (retv == null)
//				{
//					throw new ArmsException("オーブンプロファイルが見つかりません:" + lot.TypeCd + ":" + procno.ToString());
//				}

//				return Convert.ToInt32(retv);
//			}
//		}

		/// <summary>
		/// オーブンのプロファイル取得
		/// </summary>
		/// <param name="magazineNo"></param>
		/// <param name="procno"></param>
		/// <returns></returns>
		public static int GetOvenProfileId(string magazineNo, int procno)
		{
			Magazine mag = Magazine.GetCurrent(magazineNo);
			AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
			if (mag == null) throw new ArmsException("マガジン情報が見つかりません:" + magazineNo);
			if (lot == null) throw new ArmsException("ロット情報が見つかりません：" + mag.NascaLotNO);

			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();
				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = lot.TypeCd;
				cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno;

				cmd.CommandText = "SELECT profileid FROM TmOvenProf WHERE typecd=@TYPECD AND procno=@PROCNO AND delfg=0";

				object retv = cmd.ExecuteScalar();

				if (retv == null)
				{
					throw new ArmsException("オーブンプロファイルが見つかりません:" + lot.TypeCd + ":" + procno.ToString());
				}

				return Convert.ToInt32(retv);
			}
		}
    }
}
