using SLCommonLib.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace EICS.Database
{
	class LotNoConv
	{
        public int MarkingIdListNo { get; set; }
        public string MarkingIdListChar { get; set; }

		public static string GetConvertedLotNo(int lineCD, string convertID, string beforeConvLotNo)
		{
			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
			{
				string afterConvLotNo = string.Empty;

				string sql = @" SELECT ConvertID, BeforeConvVAL, AfterConvVAL 
								FROM TmLotNoConv WITH(NOLOCK) 
								WHERE (ConvertID = @ConvertID) AND (BeforeConvVAL = @BeforeConvVAL) OPTION(MAXDOP 1) ";

				conn.SetParameter("@ConvertID", SqlDbType.Char, convertID);
				conn.SetParameter("@BeforeConvVAL", SqlDbType.NVarChar, beforeConvLotNo);

				System.Data.Common.DbDataReader rd = null;

				using (rd = conn.GetReader(sql))
				{
					int ordAfterConvVAL = rd.GetOrdinal("AfterConvVAL");

					while (rd.Read())
					{
						if (rd.IsDBNull(ordAfterConvVAL) == false)
						{
							afterConvLotNo = rd.GetString(ordAfterConvVAL);
						}
					}
				}

				return afterConvLotNo;
			}
		}

        public static LotMark GetFullSerialNoMarikingChar(string lotno, Constant.TypeGroup typeGrp, int lineCD, int markingDigit)
        {
            LotMark retv = new LotMark();

            retv.SerialNo = GetLatestMarkingId(lotno, typeGrp, lineCD);
            retv.MarkNo = IntToN(retv.SerialNo, typeGrp, lineCD, markingDigit);
            retv.LotNo = lotno;

            return retv;
            
        }

        /// <summary>
        /// 2016.3.16 データベースからN進数の文字列を取得する。(noVerは1ラインで複数のN進数を使い分ける際に使用)
        /// </summary>
        /// <param name="typeGrp"></param>
        /// <returns></returns>
        private static List<LotNoConv> GetConvertCharList(Constant.TypeGroup typeGrp, int LineCD)
        {
            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, LineCD, null), "System.Data.SqlClient", false))
            {
                string afterConvLotNo = string.Empty;
                List<LotNoConv> retv = new List<LotNoConv>();

                string sql = @" SELECT decimalno, marktext 
								FROM TmMarkingConv WITH(NOLOCK) 
								WHERE (verno = @VerNo)
                                ORDER BY decimalno";

                conn.SetParameter("@VerNo", SqlDbType.NVarChar, typeGrp.ToString());


                using (System.Data.Common.DbDataReader rd = conn.GetReader(sql))
                {
                    int ordDecimalno = rd.GetOrdinal("decimalno");
                    int ordMarktext = rd.GetOrdinal("marktext");

                    while (rd.Read())
                    {
                        LotNoConv tempLotNoConv = new LotNoConv();
                        tempLotNoConv.MarkingIdListNo = rd.GetInt32(ordDecimalno);
                        tempLotNoConv.MarkingIdListChar = rd.GetString(ordMarktext);

                        retv.Add(tempLotNoConv);
                    }
                }

                if (retv == null || retv.Count() < 1)
                {
                    throw new ApplicationException("マーキング文字連番マスタが見つかりません。：" + typeGrp.ToString());
                }

                // マスタのdecimalnoが0から1ずつインクリメントしているかチェック。※飛び番も不可
                // marktextの重複はデータベース側のキー設定でフォロー。 2016.3.23 湯浅
                for (int i = 0; i < retv.Count(); i++)
                {
                    if (retv[i].MarkingIdListNo != i)
                    {
                        throw new ApplicationException("マーキング用連番定義が0から順に設定されていません。マスタ管理者に確認してください。:" + typeGrp.ToString());
                    }
                }

                return retv;
            }
        }

        /// <summary>
        /// 2016.3.16 連番取得テーブルを参照。既に該当ロットが取得済みならその番号を、取得済みで無いなら
        /// lotnoがnullで最も数が小さいものを取得。
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="typeGrp"></param>
        /// <returns></returns>
        private static long GetLatestMarkingId(string lotno, Constant.TypeGroup typeGrp, int lineCd)
        {

			using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCd, null)))
			using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                    cmd.Parameters.Add("@CATEGORY", SqlDbType.NVarChar).Value = typeGrp.ToString();

                    string afterConvLotNo = string.Empty;
                    long retv = 0;

                    cmd.CommandText = @" SELECT serialno 
								         FROM TnMarkingID 
							       	     WHERE (lotno = @LOTNO)
                                         AND (category = @CATEGORY) ";

                    object LatestSerialNo = cmd.ExecuteScalar();

                    //該当ロットが既に取得済みならそれを返して終わり
                    if (LatestSerialNo != null)
                    {
                        retv = (long)LatestSerialNo;
                        cmd.Transaction.Rollback();
                        return retv;
                    }

                    //未取得ならlotno=nullのレコードで一番小さい値を取得
                    cmd.CommandText = @" SELECT TOP (1) serialno, revision
								         FROM TnMarkingID WITH(TABLOCKX)
							       	     WHERE (lotno is NULL)
                                         AND (category = @CATEGORY) 
                                         ORDER BY revision, serialNo ";

                    long serialNo = 0;
                    int revison = 0;
                   
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        int ordSerialNo = rd.GetOrdinal("SerialNo");
                        int ordReviton = rd.GetOrdinal("Revision");
                        
                        while (rd.Read())
                        {
                            serialNo = rd.GetInt64(ordSerialNo);
                            revison = rd.GetInt32(ordReviton);
                        }
                    }

                    //seiral=0にしていたが連番は0になる可能性があるので判断をrevisionに変更。5.18
                    if (revison == 0)
                    {
                        throw new ApplicationException("マーキング用連番の取得に失敗しました。連番に空きがあるか確認してください。:" + lotno);
                    }

                    retv = serialNo;
                    cmd.Parameters.Add("@SERIALNO", SqlDbType.BigInt).Value = serialNo;
                    cmd.Parameters.Add("@REVISION", SqlDbType.Int).Value = revison;
                    cmd.Parameters.Add("@DATE", SqlDbType.Date).Value = DateTime.Now;

                    //最終更新日の更新を追加 5.18
                    cmd.CommandText = @" UPDATE TnMarkingID
                                         SET lotno = @LOTNO, lastupddt = @DATE
							       	     WHERE (serialNo = @SERIALNO)
                                         AND (category = @CATEGORY) 
                                         AND (revision = @REVISION) ";

                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();

                    return retv;
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ApplicationException("マーキング番号取得時に不明なエラーが発生。連番に空きがあるか確認し、空きがある場合はシステム管理者に確認してください。:" + lotno, ex);
                }
            }
        }


        /// <summary>
        /// ロット・連番を指定して、そのロットで使用済みの連番が別ロットに使用可能かどうかをチェックする関数 2016.3.22 湯浅
        /// ロット・連番でTnMarkingIDを検索してRevisionを取得。該当連番の最新のRevisionを取得し、
        /// ロット・連番でヒットしたRevisionが最新のものより古ければOK。（一週まわって再使用している）
        /// 最新のRevisionと一致する場合はNG。Revisionが取得できない※場合は例外エラー停止。
        /// (※TnLotMarkにデータがあるがTnMarkingIDにレコードが無いのはおかしいのでとりあえず止める)
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="serialNo"></param>
        /// <param name="typeGrp"></param>
        /// <returns></returns>
        public static bool CheckMarkingIdUsable(string lotno, long serialNo, Constant.TypeGroup typeGroup, int lineCd)
        {

            using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCd, null)))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                    cmd.Parameters.Add("@SERIALNO", SqlDbType.BigInt).Value = serialNo;
                    cmd.Parameters.Add("@CATEGORY", SqlDbType.NVarChar).Value = typeGroup.ToString();
                    
                    //DRBFM
                    cmd.CommandText = @" SELECT Revision
								         FROM TnMarkingID
							       	     WHERE　(serialno = @SERIALNO)
                                         AND (lotno = @LOTNO)
                                         AND (category = @CATEGORY) ";

                    object revision = cmd.ExecuteScalar();

                    if (revision == null || revision == System.DBNull.Value)
                    {
                        throw new ApplicationException("印字履歴があるが連番テーブルに取得履歴が存在しないIDがあります。システム管理者に状況の確認をおこなってください。:" + lotno);
                    }

                    cmd.CommandText = @" SELECT Revision
								         FROM TnMarkingID
							       	     WHERE (serialno = @SERIALNO)
                                         AND (category = @CATEGORY)
                                         Order By revision DESC";

                    object latestRevision = cmd.ExecuteScalar();

                    if ((int)latestRevision == (int)revision)
                    {
                        return false;
                    }
                    
                    return true;

                }
                catch (Exception ex)
                {
                    throw new Exception("マーキング番号重複チェック時に不明なエラーが発生:" + lotno, ex);
                }
            }
        }

        /// <summary>
        /// 2016.3.15 INT型を任意のn進数に変換する。（n進数はデータベースから取得）
        /// </summary>
        /// <param name="serialno">連番</param>
        /// <param name="typeGrp">カテゴリ</param>
        /// <param name="LineCD"></param>
        /// <param name="digit">マーキング文字桁</param>
        /// <returns></returns>
        private static string IntToN(long serialno, Constant.TypeGroup typeGrp, int LineCD, int markingDigit)
        {
            if (markingDigit < 1)
            {
                throw new ApplicationException("マーキング桁数が不適切な値となっています。桁数：" + markingDigit);
            }

            List<LotNoConv> ConvertCharList = GetConvertCharList(typeGrp, LineCD);
            int maxId = ConvertCharList.Count();
            long num = serialno;

            if (maxId == 0 || maxId == 1)
            {
                throw new ApplicationException("n進数の設定が不正です。nには2以上の値を指定する必要があります。システム管理者に連絡を行って下さい。n："+ maxId.ToString());
            }

            string result = string.Empty;

            long amari = 0;
            bool calcEndFg = false;
            while (calcEndFg == false)
            {
                if (num < maxId)
                {
                    result = ConvertCharList[(int)num].MarkingIdListChar + result;
                    calcEndFg = true;
                }
                else
                {
                    amari = num % maxId;
                    num = num / maxId;  //※numが整数型なので小数点以下は切り捨て
                    result = ConvertCharList[(int)amari].MarkingIdListChar + result;
                }
            }

            //桁が足りない場合は指定桁まで左側に最小値文字(10進数で言うところの0)で埋める
            result = result.PadLeft(markingDigit, ConvertCharList[0].MarkingIdListChar[0]);

            if (result.Length > markingDigit)
            {
                throw new ApplicationException("マーキング文字が指定文字長を超過しています。マスタ管理者に確認を行って下さい。印字文字:" + result + " 指定文字長:" + markingDigit.ToString() + " 連番:" + serialno.ToString() );
            }

            return result;
        }

    }
}
