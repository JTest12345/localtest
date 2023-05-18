using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api.ARMS
{
	public class WorkMagazine
	{
		public const string MAGAZINE_LABEL_IDENTIFIER = "30";
		public const string LOT_LABEL_IDENTIFIER = "13";
		public const string MAGAZINE_DIVIDE_IDENTIFIER = "_#";
		public const int MAGAZINE_DIVIDE_IDENTIFIER_LEN = 3;
		/// <summary>マガジンラベルに最低限必要な要素数</summary>
		public const int LABEL_NEED_ELEMENT_NUM = 2;

		public string MagazineNo { get; set; }
		public string LotNo { get; set; }
		public int CompleteProcNo { get; set; }
		public bool NewFg { get; set; }

		public WorkMagazine() { }

        #region マガジン分割対応

        /// <summary>
        /// string.split用マガジン連番の分割文字
        /// </summary>
        private static string[] LOT_SPLITTER = new string[] { MAGAZINE_DIVIDE_IDENTIFIER };


        /// <summary>
        /// マガジン分割ロット番号を通常ロットに変換
        /// 通常ロットの場合はそのまま
        /// </summary>
        /// <param name="maglot"></param>
        /// <returns></returns>
        public static string MagLotToNascaLot(string maglot)
        {
            if (string.IsNullOrEmpty(maglot)) return maglot;
            string[] splitted = maglot.Split(LOT_SPLITTER, StringSplitOptions.RemoveEmptyEntries);
            return splitted[0];
        }

        #endregion

		public static WorkMagazine getDataFromMagazineNo(string magazineNo)
		{
			List<WorkMagazine> mags = getDatas(magazineNo, null);

			if (mags.Count == 0)
			{
				throw new ApplicationException(string.Format("ARMSに稼働中マガジンが存在しません。マガジンNO:{0}", magazineNo));
			}
			else { return mags[0]; }
			//else { return mags.Single(); } //マガジン分割に対応して無い為削除
		}

		public static WorkMagazine GetDataFromLotNo(string lotNo)
		{
			List<WorkMagazine> mags = getDatas(null, lotNo);
			if (mags.Count == 0) 
			{
				throw new ApplicationException(string.Format("ARMSに稼働中マガジンが存在しません。ロットNO:{0}", lotNo));
			}
            else { return mags.FirstOrDefault(); }
		}

		public static WorkMagazine GetDataFromLabel(string labelString)
		{
			
			WorkMagazine mag;

            labelString = labelString.Replace("\0", "");
            labelString = labelString.Replace("\r", "");

			int labelValueElementIndex = LABEL_NEED_ELEMENT_NUM - 1;

			string[] labelElement = labelString.Split(' ');

			if (labelElement.Length < LABEL_NEED_ELEMENT_NUM)
			{
				throw new ApplicationException(string.Format("ラベル文字列の要素数が不足しています。 ラベル文字:{0} 必要な要素:{1}番目", labelString, LABEL_NEED_ELEMENT_NUM));
			}

			if (labelElement[0] == MAGAZINE_LABEL_IDENTIFIER)
			{
				mag = getDataFromMagazineNo(labelElement[labelValueElementIndex]);
			}
			else if (labelElement[0] == LOT_LABEL_IDENTIFIER)
			{
				mag = WorkMagazine.GetDataFromLotNo(labelElement[labelValueElementIndex]);
			}
			else
			{
				throw new ApplicationException(string.Format("未知のラベル種類です。 ラベル値：{0}", labelString));
			}

			return mag;
		}

		/// <summary>
		/// 作業中マガジンを取得
		/// </summary>
		/// <param name="magazineNo"></param>
		/// <returns></returns>
		private static List<WorkMagazine> getDatas(string magazineNo, string lotNo)
		{
			//2014/10/2 ARMSにある実績系のテーブルはARMS参照

			List<WorkMagazine> retv = new List<WorkMagazine>();

			using (SqlConnection con = new SqlConnection(LENS2_Api.Config.Settings.ArmsConnectionString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT LotNO, MagNO, InlineProcNO, NewFG
									FROM TnMag WITH(nolock) 
									WHERE NewFG = @NewFG ";

				if (string.IsNullOrEmpty(magazineNo) == false)
				{
					sql += @" AND MagNO = @MagNO ";
					cmd.Parameters.Add("@MagNO", System.Data.SqlDbType.NVarChar).Value = magazineNo;
				}

				if (string.IsNullOrEmpty(lotNo) == false)
				{
					sql += @" AND LotNO = @LotNO ";
					cmd.Parameters.Add("@LotNO", System.Data.SqlDbType.NVarChar).Value = lotNo;
				}

				cmd.Parameters.Add("@NewFG", System.Data.SqlDbType.Int).Value = Convert.ToInt16(true);

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						WorkMagazine mag = new WorkMagazine();
						mag.MagazineNo = rd["MagNO"].ToString().Trim();

						string lotno = rd["LotNO"].ToString().Trim();

						if (lotno.Contains(MAGAZINE_DIVIDE_IDENTIFIER))
						{
							lotno = lotno.Substring(0, lotno.Length - MAGAZINE_DIVIDE_IDENTIFIER_LEN);
						}
						mag.LotNo = lotno;

						mag.CompleteProcNo = Convert.ToInt32(rd["InlineProcNO"]);
						mag.NewFg = Convert.ToBoolean(rd["NewFG"]);

						retv.Add(mag);
					}
				}

                if (string.IsNullOrEmpty(magazineNo) == false && magazineNo.Contains(MAGAZINE_DIVIDE_IDENTIFIER) == false)
                {
                    // 自動搬送のQRプレートで分割マガジンデータを検索
                    if (retv.Count == 0)
                    {
                        retv.AddRange(getDatas(string.Format("{0}_#1", magazineNo), (lotNo == null) ? null : lotNo + "_#1" ));
                        retv.AddRange(getDatas(string.Format("{0}_#2", magazineNo), (lotNo == null) ? null : lotNo + "_#2"));
                    }
                }

                if (string.IsNullOrEmpty(lotNo) == false && lotNo.Contains(MAGAZINE_DIVIDE_IDENTIFIER) == false)
                {
                    // 高生産性のQRラベルで分割マガジンデータを検索
                    if (retv.Count == 0)
                    {
                        retv.AddRange(getDatas((magazineNo == null) ? null : magazineNo + "_#1", string.Format("{0}_#1", lotNo)));
                        retv.AddRange(getDatas((magazineNo == null) ? null : magazineNo + "_#2", string.Format("{0}_#2", lotNo)));
                    }
                }
			}

			return retv;
		}
	}
}
