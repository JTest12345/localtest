using SLCommonLib.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EICS.Database
{
	public class LotMark
	{
		/// <summary>サイレン(閾値越えｴﾗｰ)</summary>
		public static System.IO.Stream strm = Properties.Resources.Alarm0010;
		public static System.Media.SoundPlayer sp = new System.Media.SoundPlayer(strm);

		public string LotNo { get; set; }
        public string MarkNo { get; set; }
        public long SerialNo { get; set; }

        /// <summary>
        /// ロットとマーキングNoでマーキング実績テーブルを検索する。
        /// 2016.4.9 取得先をARMSのTnLotMarkに変更。
        /// </summary>
        /// <param name="lineCD"></param>
        /// <param name="lotNo"></param>
        /// <param name="markNo"></param>
        /// <returns></returns>
		public static LotMark GetLotMarkInfo(int lineCD, string lotNo, string markNo)
		{
			LotMark lotMark = null;

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
			{
				string afterConvLotNo = string.Empty;

				string sql = @" SELECT lotNo, markChar 
								FROM TnLotMark WITH(NOLOCK) 
								WHERE row = 0 "; //マガジン単位のマーキングしか想定していないのでrow=0で固定

				if (string.IsNullOrEmpty(lotNo) == false)
				{
					sql += @" AND lotNo = @lotNo ";
					conn.SetParameter("@lotNo", SqlDbType.NVarChar, lotNo);
				}

				if (string.IsNullOrEmpty(markNo) == false)
				{
					sql += @" AND markChar = @markNo ";
					conn.SetParameter("@markNo", SqlDbType.NVarChar, markNo);
				}

				using (System.Data.Common.DbDataReader rd = conn.GetReader(sql))
				{
					int ordLotNo = rd.GetOrdinal("lotno");
					int ordMarkNo = rd.GetOrdinal("markChar");

					while (rd.Read())
					{
						if (rd.IsDBNull(ordLotNo) == false)
						{
							lotMark = new LotMark();
							lotMark.LotNo = rd.GetString(ordLotNo);
							lotMark.MarkNo = rd.GetString(ordMarkNo);
						}
					}
				}

				return lotMark;
			}
		}

        /// <summary>
        /// ロットは一致しているがマーキングNOが一致していないレコードを取得。2016.4.9 湯浅
        /// </summary>
        /// <param name="lineCD"></param>
        /// <param name="lotNo"></param>
        /// <param name="markNo"></param>
        /// <returns></returns>
        public static List<LotMark> GetMissmatchMarkingNoList(int lineCD, string lotNo, string markNo)
        {
            List<LotMark> LotMarkDataList = new List<LotMark>();


            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {

                string sql = @" SELECT lotno, markchar, markdata
								FROM TnLotMark WITH(NOLOCK) 
								WHERE (lotno = @lotNo) ";

                conn.SetParameter("@lotNo", SqlDbType.NVarChar, lotNo);

                using (System.Data.Common.DbDataReader rd = conn.GetReader(sql))
                {
                    int ordLotNo = rd.GetOrdinal("lotno");
                    int ordMarkNo = rd.GetOrdinal("markchar");
                    int ordMarkdata = rd.GetOrdinal("markdata");

                    while (rd.Read())
                    {
                        LotMark tempData = new LotMark();
                        tempData.LotNo = rd.GetString(ordLotNo);
                        tempData.MarkNo = rd.GetString(ordMarkNo);
                        tempData.SerialNo = rd.GetInt64(ordMarkdata);

                        if (tempData.MarkNo == markNo) continue;

                        LotMarkDataList.Add(tempData);
                    }
                }

                return LotMarkDataList;
            }
        }

        /// <summary>
        /// マーキングNOは一致しているがロットが一致していないレコードを取得。2016.4.9 湯浅
        /// </summary>
        /// <param name="lineCD"></param>
        /// <param name="lotNo"></param>
        /// <param name="markNo"></param>
        /// <returns></returns>
        public static List<LotMark> GetMissmatchMarkingLotList(int lineCD, string lotNo, string markNo)
        {
            List<LotMark> LotMarkDataList = new List<LotMark>();


            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {

                string sql = @" SELECT lotno, markchar, markdata
								FROM TnLotMark WITH(NOLOCK) 
                                WHERE (markchar = @markno)";

                conn.SetParameter("@markno", SqlDbType.NVarChar, markNo);

                //DRBFM
                using (System.Data.Common.DbDataReader rd = conn.GetReader(sql))
                {
                    int ordLotNo = rd.GetOrdinal("lotno");
                    int ordMarkNo = rd.GetOrdinal("markchar");
                    int ordMarkdata = rd.GetOrdinal("markdata");

                    while (rd.Read())
                    {
                        LotMark tempData = new LotMark();
                        tempData.LotNo = rd.GetString(ordLotNo);
                        tempData.MarkNo = rd.GetString(ordMarkNo);
                        tempData.SerialNo = rd.GetInt64(ordMarkdata);

                        if (tempData.LotNo == lotNo) continue;

                        LotMarkDataList.Add(tempData);
                    }
                }

                return LotMarkDataList;
            }
        }


		/// <summary>
		/// DB登録
		/// </summary>
		/// <param name="lineCD"></param>
		/// <param name="lotNo"></param>
		/// <param name="markNo"></param>
		/// <returns>途中で取り消された場合はfalse、正常終了時はtrue</returns>
		public static bool CancelableInsert(int lineCD, LotMark lotMarkData, Constant.TypeGroup typeGroup, bool fullSerialNoModeFG)
		{
            lotMarkData.LotNo = lotMarkData.LotNo.Trim();
			lotMarkData.MarkNo = lotMarkData.MarkNo.Trim();

			DialogResult dialogResult = new DialogResult();
			string msg = string.Empty;

            //fullSerialNoModeFGで分岐。ModeOFFならmarkNoとlotNoで不一致データを検索して何れかでヒットしたらNG。
            //ModeONなら不一致検索後、LOTのみ不一致の場合はTnMarkingIDのRevisionを確認し、
            //該当IDの最新Revisionのレコードでなければエラーなし。
            //lotNo同じでmarkNoが違うものは無条件でエラー。　2016.5.8 湯浅

            List<LotMark> markNoMissMatchList = GetMissmatchMarkingNoList(lineCD, lotMarkData.LotNo, lotMarkData.MarkNo);

            //LotMarkClassに版数と不一致フラグを追加して、追加関数なしでこの関数内で判定が完結するように変更する。
            　　//⇒再確認したが上記の関数の検索対象はTnLotMarkではRevisionが取れないので対応できない。元のままにしておく。5.18湯浅

            //markingNoズレの場合はどうせINSERTでこけるので、確認ウィンドウではなく即例外NGを返す。
            if (markNoMissMatchList != null && markNoMissMatchList.Count() > 0)
            {
                throw new ApplicationException("このロット番号は別IDで印字した履歴があります。システム担当に確認を行って下さい。 LotNo:" + lotMarkData.LotNo);
            }

            List<LotMark> lotNoMissMatchList = GetMissmatchMarkingLotList(lineCD, lotMarkData.LotNo, lotMarkData.MarkNo);

            if (lotNoMissMatchList != null && lotNoMissMatchList.Count() > 0)
            {
                int i = 0;
                bool overlapNgFg = true;

                //完全連番モードがOFFなら無条件でエラー、連番モードONなら使用可能かチェックして、不可の場合のみエラー。
                if (fullSerialNoModeFG)
                {
                    for (i = 0; i < lotNoMissMatchList.Count(); i++)
                    {
                        overlapNgFg = false;

                        if (LotNoConv.CheckMarkingIdUsable(lotNoMissMatchList[i].LotNo, lotNoMissMatchList[i].SerialNo, typeGroup, lineCD) == false)
                        {
                            overlapNgFg = true;
                            break;
                        }
                    }
                }

                if (overlapNgFg)
                {
                    sp.PlayLooping();
				    msg = string.Format("ﾏｰｷﾝｸﾞNo:{1}は別ﾛｯﾄ:{0}で過去に印字した履歴があります。 \r\n" +
				    "このまま作業を続けた場合、異なるロットに重複した印字を行ってしまう可能性があります。\r\n" +
				    "別ﾛｯﾄについて製品の状況を確認し、重複印刷になっていないかを確認してください。\r\n" +
				    "処理を一度停止させる場合、キャンセルを押して下さい。\r\n" +
				    "問題無い事を確認後、処理を続行する場合はOKを押して下さい。", markNoMissMatchList[i].LotNo, markNoMissMatchList[i].MarkNo);

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.WARN, msg);

                    dialogResult = MustConfirmMsgBox.Show(msg);
                }
            }
            
			sp.Stop();

			if (dialogResult == DialogResult.Cancel)
			{
				return false;
			}

            //2016.3.17 登録先をEICSからARMSに変更。マガジン毎のロットマーキングで使用していない項目は全て0で登録。 湯浅
            //テーブルにmarkCharを追加。（印刷文字列のフィールド）

            //INSERT失敗を想定してlotnoとrow=0で検索してヒットしたら処理しないよう変更。(5.18)

            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
            {
                conn.SetParameter("@lotNo", SqlDbType.NVarChar, lotMarkData.LotNo);
                conn.SetParameter("@row", SqlDbType.Int, 0);

                string sql = @" SELECT lotno
                                FROM TnLotMark WIHT(NOLOCK)
                                WHERE lotno = @lotno AND row = 0 ";

                object readyRecord = conn.ExecuteScalar(sql);

                if (readyRecord != null)
                {
                    return true;
                }
                
                sql = @" INSERT INTO TnLotMark
                                (lotno, row, stockerno, markdata, workdt, manualfg, procno, framecollectfg, wafercollectfg, markChar)
                                VALUES (@lotNo, @row, @StockerNo, @MarkData, @Workdt, @ManualFg, @ProcNo, @FramecollectFg, @WaferCollectFg, @Markchar) ";

                conn.SetParameter("@lotNo", SqlDbType.NVarChar, lotMarkData.LotNo);
                conn.SetParameter("@row", SqlDbType.Int, 0);
                conn.SetParameter("@StockerNo", SqlDbType.NVarChar, "0");
                conn.SetParameter("@MarkData", SqlDbType.BigInt, lotMarkData.SerialNo);
                conn.SetParameter("@Workdt", SqlDbType.DateTime, DateTime.Now);
                conn.SetParameter("@ManualFg", SqlDbType.Int, 0);
                conn.SetParameter("@ProcNo", SqlDbType.Int, 0);
                conn.SetParameter("@FramecollectFg", SqlDbType.Int, 0);
                conn.SetParameter("@WaferCollectFg", SqlDbType.Int, 0);
                conn.SetParameter("@Markchar", SqlDbType.NVarChar, lotMarkData.MarkNo);

                conn.ExecuteNonQuery(sql);

                return true;
            }
		}

        public static bool CancelableInsert(int lineCD, LotMark lotMarkData, Constant.TypeGroup typeGroup)
        {
            return CancelableInsert(lineCD, lotMarkData, typeGroup, false);
        }
	}
}
