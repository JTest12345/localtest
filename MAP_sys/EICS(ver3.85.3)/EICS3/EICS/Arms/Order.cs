using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace EICS.Arms
{
	class Order
	{
		#region マガジン分割対応


		/// <summary>
		/// ロット番号とマガジン連番の結合文字
		/// </summary>
		private const string DEVIDED_MAGAZINE_LOTNO_CONNECTOR = "_#";

		/// <summary>
		/// string.split用マガジン連番の分割文字
		/// </summary>
		private static string[] LOT_SPLITTER = new string[] { DEVIDED_MAGAZINE_LOTNO_CONNECTOR };

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

		/// <summary>
		/// マガジン連番付きロット番号を生成
		/// lotnoに連番部分が含まれていた場合は取り除かれる
		/// </summary>
		/// <param name="lotno"></param>
		/// <param name="magSeqNo"></param>
		/// <returns></returns>
		public static string NascaLotToMagLot(string lotno, int magSeqNo)
		{
			return MagLotToNascaLot(lotno) + DEVIDED_MAGAZINE_LOTNO_CONNECTOR + magSeqNo.ToString();
		}


		/// <summary>
		/// ロット番号からマガジン連番を取得
		/// マガジン連番無しのロットなら0を返す
		/// </summary>
		/// <param name="rawLotNo"></param>
		/// <returns></returns>
		public static int ParseMagSeqNo(string rawLotNo)
		{
			if (string.IsNullOrEmpty(rawLotNo) || rawLotNo.Contains(DEVIDED_MAGAZINE_LOTNO_CONNECTOR) == false)
			{
				return 0;
			}

			try
			{
				string[] spl = rawLotNo.Split(LOT_SPLITTER, StringSplitOptions.RemoveEmptyEntries);
				return int.Parse(spl[1]);
			}
			catch
			{
				throw new ApplicationException("ロット番号フォーマット異常：" + rawLotNo);
			}
		}
		#endregion

		#region プロパティ
		private string _lotno;
		/// <summary>
		/// ロット情報
		/// </summary>
		public string LotNo
		{
			get { return _lotno; }
			set
			{
				int newSeq = ParseMagSeqNo(value);
				if (newSeq == 0 && value.Contains(DEVIDED_MAGAZINE_LOTNO_CONNECTOR))
				{
					//マガジン連番0の場合　連番があれば取り除く
					_lotno = MagLotToNascaLot(value);
				}
				else
				{
					_lotno = value;
				}

				if (newSeq != DevidedMagazineSeqNo) DevidedMagazineSeqNo = newSeq;
			}
		}

		/// <summary>
		/// マガジン連番があった場合は取り除いた形のロット番号
		/// </summary>
		public string NascaLotNo
		{
			get { return MagLotToNascaLot(LotNo); }
		}


		private int _devidedMagSeqNo;
		/// <summary>
		/// マガジン分割連番
		/// </summary>
		public int DevidedMagazineSeqNo
		{
			get { return _devidedMagSeqNo; }
			set
			{
				_devidedMagSeqNo = value;
				int currentSeq = ParseMagSeqNo(LotNo);
				if (currentSeq != value)
				{
					if (value == 0)
					{
						LotNo = MagLotToNascaLot(LotNo);
					}
					else
					{
						LotNo = NascaLotToMagLot(MagLotToNascaLot(LotNo), value);
					}
				}
			}
		}

		/// <summary>
		/// 作業ID
		/// </summary>
		public string WorkId { get; set; }

		/// <summary>
		/// 搬入マガジンNO
		/// </summary>
		public string InMagazineNo { get; set; }

		/// <summary>
		/// 搬出マガジンNo
		/// </summary>
		public string OutMagazineNo { get; set; }

		/// <summary>
		/// 装置NO
		/// </summary>
		public int MacNo { get; set; }

		/// <summary>
		/// 作業開始時間
		/// </summary>
		public DateTime WorkStartDt { get; set; }

		/// <summary>
		/// 作業完了時間
		/// </summary>
		public DateTime? WorkEndDt { get; set; }

		/// <summary>
		/// NG登録を明示したい場合の処理用
		/// </summary>
		public string MagazineSt { get; set; }

		/// <summary>
		/// 工程ID
		/// </summary>
		public int ProcNo { get; set; }

		public string ScNo1 { get; set; }

		public string ScNo2 { get; set; }

		public bool IsComplete
		{
			get { return this.WorkEndDt.HasValue; }
		}


		/// <summary>
		/// 排出マガジンに番号が入っているまたは完了時間が入力されている場合はTrue
		/// </summary>
		public bool IsDevidedMagComplete
		{
			get
			{
				if (string.IsNullOrEmpty(OutMagazineNo) == false || this.IsComplete)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// 検査数
		/// </summary>
		public int InspectCt { get; set; }

		/// <summary>
		/// 工程作業者
		/// </summary>
		public string TranStartEmpCd { get; set; }

		/// <summary>
		/// 完了作業者
		/// </summary>
		public string TranCompEmpCd { get; set; }

		/// <summary>
		/// 検査作業者
		/// </summary>
		public string InspectEmpCd { get; set; }


		/// <summary>
		/// 作業コメント
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// NASCA開始連携済みフラグ
		/// </summary>
		public bool IsNascaStart { get; set; }

		/// <summary>
		/// NASCA完了連携済みフラグ
		/// </summary>
		public bool IsNascaEnd { get; set; }

		/// <summary>
		/// NASCA不良状況連携完了フラグ
		/// </summary>
		public bool IsNascaDefectEnd { get; set; }


		/// <summary>
		/// NASCAコメント情報連携完了フラグ
		/// </summary>
		public bool IsNascaCommentEnd { get; set; }

		public bool DelFg { get; set; }

		/// <summary>
		/// マガジン分割で連携用にダミーデータを作成した時の連番
		/// </summary>
		public int NascaDummmyOrderNo { get; set; }

        #endregion

        #region GetOrder

        /// <summary>
        /// 指定ロットの全作業レコード取得
        /// </summary>
        /// <param name="nascalotNo"></param>
        /// <returns></returns>
        public static Order[] GetOrder(int lineCD, string nascalotNo)
        {
            return SearchOrder(lineCD, nascalotNo, null, null, false, false);
        }

        /// <summary>
        /// 指定工程のOrder情報取得
        /// WorkIDは登録時に使うものに再構成
        /// </summary>
        /// <param name="nascalotNo"></param>
        /// <param name="processNo"></param>
        /// <returns></returns>
        public static Order[] GetOrder(int lineCD, string nascalotNo, long procNo)
		{
			Order[] orders = SearchOrder(lineCD, nascalotNo, procNo, null, false, false);
			return orders;
		}

		public static Order[] SearchOrder(int lineCD, string nascalotNo, long? procNo, int? macno, bool onlyWorking, bool onlyNascaUnsync)
		{
			return SearchOrder(lineCD, nascalotNo, null, procNo, macno, onlyWorking, onlyNascaUnsync, null, null, null, null);
		}


		public static Order[] SearchOrder(int lineCD, string nascalotNo, int? magSeqNo, long? procNo, int? macno, bool onlyWorking, bool onlyNascaUnsync,
			DateTime? startfrom, DateTime? startto, DateTime? endfrom, DateTime? endto)
		{
			List<Order> retv = new List<Order>();

			//マガジン分割対応
			nascalotNo = MagLotToNascaLot(nascalotNo);

			using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
			using (SqlCommand cmd = con.CreateCommand())
			{
				try
				{
					con.Open();
					cmd.CommandText = @"
                        SELECT 
                          t.lotno , 
                          t.procno , 
                          t.macno , 
                          t.inmag , 
                          t.outmag , 
                          t.startdt , 
                          t.enddt , 
                          t.iscomplt , 
                          t.stocker1 , 
                          t.stocker2 ,
                          t.transtartempcd,
                          t.trancompempcd,
                          t.inspectempcd,
                          t.inspectct,
                          t.isnascastart ,
                          t.isnascaend ,
                          t.isnascacommentend ,
                          t.isnascadefectend ,
                          t.comment
                        FROM 
                          tntran t with(nolock)
                        WHERE 
                          delfg = 0";

					if (nascalotNo != null)
					{
						cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = nascalotNo + "%";
						cmd.CommandText += " AND t.lotno like @LOTNO";
					}

					if (procNo.HasValue)
					{
						cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procNo;
						cmd.CommandText += " AND t.procno = @PROCNO";
					}

					if (macno.HasValue)
					{
						cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno;
						cmd.CommandText += " AND t.macno = @MACNO";
					}

					if (onlyWorking == true)
					{
						cmd.CommandText += " AND t.enddt IS NULL";
					}

					if (onlyNascaUnsync == true)
					{
						cmd.CommandText += " AND (t.isnascaend <> 1)";
					}

					if (startfrom.HasValue)
					{
						cmd.Parameters.Add("@STARTFROM", SqlDbType.DateTime).Value = startfrom;
						cmd.CommandText += " AND startdt >= @STARTFROM";
					}

					if (startto.HasValue)
					{
						cmd.Parameters.Add("@STARTTO", SqlDbType.DateTime).Value = startto;
						cmd.CommandText += " AND startdt <= @STARTTO";
					}

					if (endfrom.HasValue)
					{
						cmd.Parameters.Add("@ENDFROM", SqlDbType.DateTime).Value = endfrom;
						cmd.CommandText += " AND enddt >= @ENDFROM";
					}

					if (endto.HasValue)
					{
						cmd.Parameters.Add("@ENDTO", SqlDbType.DateTime).Value = endto;
						cmd.CommandText += " AND enddt <= @ENDTO";
					}


					using (SqlDataReader reader = cmd.ExecuteReader())
					{

						while (reader.Read())
						{
							Order o = new Order();
							o.LotNo = SQLite.ParseString(reader["lotno"]);
							//マガジン連番はLotNoのSetプロパティで自動設定される
							o.MacNo = SQLite.ParseInt(reader["macno"]);
							o.ProcNo = SQLite.ParseInt(reader["procno"]);
							o.InMagazineNo = SQLite.ParseString(reader["inmag"]);
							o.OutMagazineNo = SQLite.ParseString(reader["outmag"]);
							o.WorkStartDt = SQLite.ParseDate(reader["startdt"]) ?? DateTime.MinValue;
							o.WorkEndDt = SQLite.ParseDate(reader["enddt"]);
							o.ScNo1 = SQLite.ParseString(reader["stocker1"]);
							o.ScNo2 = SQLite.ParseString(reader["stocker2"]);
							o.IsNascaStart = SQLite.ParseBool(reader["isnascastart"]);
							o.IsNascaEnd = SQLite.ParseBool(reader["isnascaend"]);
							o.IsNascaCommentEnd = SQLite.ParseBool(reader["isnascacommentend"]);
							o.IsNascaDefectEnd = SQLite.ParseBool(reader["isnascadefectend"]);
							o.Comment = SQLite.ParseString(reader["comment"]);
							o.TranStartEmpCd = SQLite.ParseString(reader["transtartempcd"]);
							o.TranCompEmpCd = SQLite.ParseString(reader["trancompempcd"]);
							o.InspectEmpCd = SQLite.ParseString(reader["inspectempcd"]);
							o.InspectCt = SQLite.ParseInt(reader["inspectct"]);
							o.DelFg = false;


							string st1 = string.IsNullOrEmpty(o.ScNo1) ? "0" : o.ScNo1;
							string st2 = string.IsNullOrEmpty(o.ScNo2) ? "0" : o.ScNo2;

							if (st1.Contains("-"))
							{
								o.WorkId = "012"; //DB工程完了
							}
							else if (st1 == "1" || st2 == "1")
							{
								o.WorkId = "011"; //搭載工程
							}
							else
							{
								o.WorkId = "014"; //その他工程
							}

							//マガジン分割対応
							if (!magSeqNo.HasValue || o.DevidedMagazineSeqNo == magSeqNo || o.DevidedMagazineSeqNo == 0)
							{
								retv.Add(o);
							}

						}
					}

					return retv.ToArray();
				}
				catch (Exception ex)
				{
					//log.Info(ex.ToString());
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.ToString());
					throw ex;
				}
			}
		}

        #endregion

        public static Order GetLastOrder(int lineCd, int macno) 
        {
            Order[] orders = SearchOrder(lineCd, null, null, null, macno, false, false, DateTime.Now.AddHours(-8), null, null, null);
            return orders.OrderByDescending(o => o.WorkStartDt).FirstOrDefault();
        }

        public static int GetLastProcNoFromLotNo(int lineCD, string lotNo)
        {
            Order[] orders = Order.GetOrder(lineCD, lotNo);
            if (orders.Length == 0)
            {
                throw new ApplicationException("作業開始レコードが見つかりません mag:" + lotNo);
            }
            else if (orders.Length == 1)
            {
                return orders[0].ProcNo;
            }
            else
            {
                //複数回同じ装置で作業されている場合は
                //開始日が一番新しいものを返す
                return orders.OrderByDescending(o => o.WorkStartDt).First().ProcNo;
            }
        }
    }
}
