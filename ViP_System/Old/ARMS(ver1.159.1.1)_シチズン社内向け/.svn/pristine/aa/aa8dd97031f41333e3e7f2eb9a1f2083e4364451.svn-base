using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ArmsApi.Model
{
	public class LotMarkData
	{
		public bool ChangeFg { get; set; }
		public DateTime WorkDt { get; set; }
		public string LotNo { get; set; }
		public int Row { get; set; }
		public int StockerNo { get; set; }
		public decimal MarkData { get; set; }
		public bool ManualFg { get; set; }
        public bool FrameCollectFg { get; set; }
        public bool WaferCollectFg { get; set; }
        public int ProcNo { get; set; }

		/// <summary>
		/// Update/Insert
		/// </summary>
		public void Update()
		{
			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo ?? "";
				cmd.Parameters.Add("@ROW", SqlDbType.Int).Value = Row;
				cmd.Parameters.Add("@STOCKERNO", SqlDbType.Int).Value = StockerNo;
				cmd.Parameters.Add("@WORKDT", SqlDbType.DateTime).Value = this.WorkDt;
				cmd.Parameters.Add("@MARKDATA", SqlDbType.BigInt).Value = MarkData;
                cmd.Parameters.Add("@MANUALFG", SqlDbType.Int).Value = SQLite.SerializeBool(ManualFg);
                cmd.Parameters.Add("@FRAMECOLLECTFG", SqlDbType.Int).Value = SQLite.SerializeBool(FrameCollectFg);
                cmd.Parameters.Add("@WAFERCOLLECTFG", SqlDbType.Int).Value = SQLite.SerializeBool(WaferCollectFg);
                cmd.Parameters.Add("@PROCNO", SqlDbType.Int).Value = ProcNo;

				try
				{
					con.Open();

					//新規Insert
					cmd.CommandText = @"SELECT lotno FROM TnLotmark WHERE lotno=@LOTNO AND row=@ROW";
					object lot = cmd.ExecuteScalar();

					if (lot == null)
					{
						#region Insertコマンド
						cmd.CommandText = @"
                            INSERT INTO TnLotmark 
                              ( 
                                lotno , 
                                row , 
                                stockerno , 
                                markdata ,
                                workdt,
                                manualfg,
                                framecollectfg,
                                wafercollectfg,
                                procno
                              ) 
                            VALUES 
                              ( 
                                @LOTNO , 
                                @ROW , 
                                @STOCKERNO , 
                                @MARKDATA ,
                                @WORKDT ,
                                @MANUALFG ,
                                @FRAMECOLLECTFG,
                                @WAFERCOLLECTFG,
                                @PROCNO
                              )";
						#endregion
					}
					else
					{
						#region Updateコマンド
						cmd.CommandText = @"
                            UPDATE TnLotmark  
                            SET 
                              stockerno = @STOCKERNO , 
                              markdata = @MARKDATA , 
                              workdt = @WORKDT ,
                              manualfg = @MANUALFG,
                              framecollectfg = @FRAMECOLLECTFG,
                              wafercollectfg = @WAFERCOLLECTFG,
                              procno = @PROCNO
                            WHERE 
                              lotno = @LOTNO
                             AND row = @ROW";
						#endregion
					}

					cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					throw new ArmsException("ロットマーキング情報更新エラー:" + this.LotNo + ":" + this.Row.ToString() + ":" + this.MarkData.ToString() + ":" + this.StockerNo.ToString(), ex);
				}
			}

		}

		public static List<LotMarkData> Search(string lotNo, string currentMagazineNo, decimal? markData, int? row, bool? framefg, bool? waferfg)
		{
			List<LotMarkData> retv = new List<LotMarkData>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			{
				con.Open();

				using (SqlCommand cmd = con.CreateCommand())
				{
                    string sql = @" SELECT TnLotmark.lotno, TnLotmark.row, TnLotmark.stockerno, TnLotmark.markdata, TnLotmark.workdt, TnLotmark.manualfg, TnLotMark.framecollectfg, TnLotMark.wafercollectfg, TnLotMark.procno
									FROM TnLotmark WITH(nolock) ";

                    if (currentMagazineNo != null)
                    {
                        sql += " INNER JOIN TnMag WITH(nolock) ON TnLotmark.lotno = TnMag.lotno";
                    }

					sql +=  " WHERE 1=1 ";

					if (!string.IsNullOrEmpty(lotNo))
					{
						sql += " AND TnLotmark.lotno = @LotNo ";
						cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;
					}

					if (!string.IsNullOrEmpty(currentMagazineNo))
					{
						sql += " AND newfg = 1 AND TnMag.magno = @MagazineNo ";
						cmd.Parameters.Add("@MagazineNo", SqlDbType.NVarChar).Value = currentMagazineNo;
					}

					if (markData.HasValue)
					{
						sql += " AND TnLotmark.markdata = @MarkData ";
						cmd.Parameters.Add("@MarkData", SqlDbType.BigInt).Value = markData.Value;
					}

					if (row.HasValue)
					{
						sql += " AND TnLotmark.row = @Row ";
						cmd.Parameters.Add("@Row", SqlDbType.Int).Value = row.Value;
					}

                    if (waferfg.HasValue)
                    {
                        sql += " AND TnLotMark.wafercollectfg = @WAFERCOLLECTFG ";
                        cmd.Parameters.Add("@WAFERCOLLECTFG", SqlDbType.Int).Value = SQLite.SerializeBool(waferfg.Value);
                    }

                    if (framefg.HasValue)
                    {
                        sql += " AND TnLotMark.framecollectfg = @FRAMECOLLECTFG ";
                        cmd.Parameters.Add("@FRAMECOLLECTFG", SqlDbType.Int).Value = SQLite.SerializeBool(framefg.Value);
                    }

                    sql += " GROUP BY TnLotmark.lotno, TnLotmark.row, TnLotmark.stockerno, TnLotmark.markdata, TnLotmark.workdt, TnLotmark.manualfg, TnLotMark.framecollectfg, TnLotMark.wafercollectfg, TnLotMark.procno ";

					cmd.CommandText = sql;
					using (SqlDataReader rd = cmd.ExecuteReader())
					{
						while (rd.Read())
						{
							LotMarkData data = new LotMarkData();

							data.WorkDt = Convert.ToDateTime(rd["workdt"]);
							data.LotNo = rd["lotno"].ToString();
							data.Row = Convert.ToInt32(rd["row"]);
							data.StockerNo = Convert.ToInt32(rd["stockerno"]);
							data.MarkData = Convert.ToDecimal(rd["markdata"]);
							data.ManualFg = SQLite.ParseBool(rd["manualfg"]);
                            data.WaferCollectFg = SQLite.ParseBool(rd["wafercollectfg"]);
                            data.FrameCollectFg = SQLite.ParseBool(rd["framecollectfg"]);
                            data.ProcNo = Convert.ToInt32(rd["procno"]);

							retv.Add(data);
						}
					}
				}
			}

			return retv;
		}
		public static List<LotMarkData> Search(string lotNo, string currentMagazineNo)
		{
			return Search(lotNo, currentMagazineNo, null, null, null, null);
		}
		public static List<LotMarkData> Search(string lotNo, decimal markData)
		{
			return Search(lotNo, null, markData, null, null, null);
		}
		public static List<LotMarkData> Search(string lotNo, int row)
		{
            return Search(lotNo, null, null, row, null, null);
		}

		public static bool HasMarkData(string lotNo, decimal markData)
		{
			List<LotMarkData> markDataList = Search(lotNo, markData);
			if (markDataList.Count == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static bool HasMarkData(string lotNo, int row)
		{
			List<LotMarkData> markDataList = Search(lotNo, row);
			if (markDataList.Count == 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}


        [Obsolete("Updateメソッドを使用すること")]
		public static void InsertUpdate(LotMarkData data)
		{
			if (string.IsNullOrEmpty(data.LotNo) || data.Row == 0)
			{
				throw new ApplicationException("ロット番号、マガジン段数が未記入の為、登録できません。");
			}

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			{
				con.Open();

				using (SqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = @" UPDATE TnLotmark SET
											stockerno = @stockerno,  
											markdata = @markdata, workdt = @workdt, manualfg = @manualfg
										 WHERE lotno = @lotno AND row = @row
										 INSERT INTO TnLotmark (lotno, row, stockerno, markdata, workdt, manualfg)
										 SELECT @lotno, @row, @stockerno, @markdata, @workdt, @manualfg
										 WHERE NOT EXISTS (SELECT * FROM TnLotmark WHERE lotno = @lotno AND row = @row) ";

					cmd.Parameters.Add("@lotno", SqlDbType.NVarChar).Value = data.LotNo;
					cmd.Parameters.Add("@row", SqlDbType.Int).Value = data.Row;
					cmd.Parameters.Add("@stockerno", SqlDbType.Int).Value = data.StockerNo;
					cmd.Parameters.Add("@markdata", SqlDbType.BigInt).Value = data.MarkData;
					cmd.Parameters.Add("@workdt", SqlDbType.DateTime).Value = data.WorkDt;
					cmd.Parameters.Add("@manualfg", SqlDbType.Int).Value = data.ManualFg;

					cmd.ExecuteNonQuery();
				}
			}
		}

        public static void Delete(string lotno)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    string sql = " DELETE FROM TnLotmark WHERE lotno = @LotNo";
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
    }
}
