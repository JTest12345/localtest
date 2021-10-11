using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Order
    {
        #region プロパティ
        //WorkIdは廃止
        //IsNascaStart、IsNascaDefectEnd、IsNascaCommentEndフラグは廃止
        //IsNascaStartはカットブレンドの指図発行有無
        //IsNascaDefectEndはカットブレンドの登録完了フラグに利用。別のフラグを作成。

        /// <summary>
        /// ライン番号
        /// </summary>
        public string LineNo { get; set; }

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
        /// 分割連番
        /// </summary>
        public int SeqNo { get; set; }

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

        /// <summary>
        /// ストッカーの開始段数
        /// </summary>
        public int StockerStartCol { get; set; }

        /// <summary>
        /// ストッカーの終了段数
        /// </summary>
        public int StockerEndCol { get; set; }

        /// <summary>
        /// ストッカーの交換回数
        /// </summary>
        public int StockerChangeCt { get; set; }


        public bool IsComplete
        {
            get { return this.WorkEndDt.HasValue; }
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

        public bool IsNascaStart { get; set; }

        /// <summary>
        /// NASCA連携済みフラグ
        /// </summary>
        public bool IsNascaEnd { get; set; }

        /// <summary>
        /// NASCA連携中フラグ
        /// </summary>
        public bool IsNascaRunning { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool DelFg { get; set; }

        /// <summary>
        /// 不良登録完了フラグ(Trueの場合NASCA連携可能)
        /// </summary>
        public bool IsDefectEnd { get; set; }

        /// <summary>
        /// 不良登録完了フラグ(Nasca不良ファイル取り込み時にTrue)
        /// </summary>
        public bool IsDefectAutoImportEnd { get; set; }

        /// <summary>
        /// マガジン分割で連携用にダミーデータを作成した時の連番(追加)
        /// </summary>
        public int NascaDummmyOrderNo { get; set; }

        public string ScNo1 { get; set; }

        public string ScNo2 { get; set; }

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
        /// MAP1stDiebonder基板登録用
        /// </summary>
        public Material[] FrameList { get; set; }

        /// <summary>
        /// 取り込みの実績かどうか
        /// </summary>
        public bool IsAutoImport { get; set; }

        public bool IsResinMixOrdered { get; set; }

        #endregion

        #region GetOrder

        /// <summary>
        /// 指定ロットの全作業レコード取得
        /// </summary>
        /// <param name="nascalotNo"></param>
        /// <returns></returns>
        public static Order[] GetOrder(string nascalotNo)
        {
            return SearchOrder(nascalotNo, null, null, false, false);
        }

        /// <summary>
        /// 指定工程のOrder情報取得
        /// WorkIDは登録時に使うものに再構成
        /// </summary>
        /// <param name="nascalotNo"></param>
        /// <param name="processNo"></param>
        /// <returns></returns>
        public static Order[] GetOrder(string nascalotNo, int procNo)
        {
            Order[] orders = SearchOrder(nascalotNo, procNo, null, false, false);
            return orders;
        }

        /// <summary>
        /// 指定工程のOrder情報取得 (複数ロット指定)
        /// </summary>
        /// <param name="nascalotNoList"></param>
        /// <param name="procNo"></param>
        /// <returns></returns>
        public static Order[] GetOrder(IEnumerable<string> nascalotNoList, int procNo)
        {
            if (nascalotNoList == null || nascalotNoList.Any() == false) return new Order[] { };

            Order[] orders = SearchOrder(null, nascalotNoList, null, procNo, null, false, false, null, null, null, null, SQLite.ConStr);
            return orders;
        }


        #region GetMagazineOrder

        /// <summary>
        /// マガジン分割対応版 指図取得
        /// </summary>
        /// <param name="nascalotNo"></param>
        /// <param name="procNo"></param>
        /// <returns></returns>
        public static Order GetMagazineOrder(string magazineLotNo, int procNo)
        {
            string nascalot = MagLotToNascaLot(magazineLotNo);
            int seqno = ParseMagSeqNo(magazineLotNo);
            return GetMagazineOrder(nascalot, seqno, procNo);
        }

        /// <summary>
        /// マガジン分割対応版 指図取得
        /// </summary>
        /// <param name="nascaLotNo"></param>
        /// <param name="magSeqNo"></param>
        /// <param name="procNo"></param>
        /// <returns></returns>
        public static Order GetMagazineOrder(string nascaLotNo, int magSeqNo, int procNo)
        {
            Order[] orders = SearchOrder(nascaLotNo, magSeqNo, procNo, null, false, false, null, null, null, null);
            Order ret = orders.Where(o => o.DevidedMagazineSeqNo == magSeqNo || o.DevidedMagazineSeqNo == 0).FirstOrDefault();

            return ret;
        }

        #endregion

        #region GetNextMagazineOrder
        /// <summary>
        /// 指定工程の次工程指図を返す
        /// 未開始の場合やロットが無い場合、次工程が無い場合はnull
        /// 未分割→分割工程の場合、次工程の完了前作業があればそれを優先して返す
        /// </summary>
        /// <param name="magazineLotNo"></param>
        /// <param name="currentProcNo"></param>
        /// <returns></returns>
        public static Order GetNextMagazineOrder(string magazineLotNo, int currentProcNo)
        {
            string nascalot = MagLotToNascaLot(magazineLotNo);
            int seqno = ParseMagSeqNo(magazineLotNo);

            return GetNextMagazineOrder(nascalot, seqno, currentProcNo);
        }

        /// <summary>
        /// 指定工程の次工程指図を返す
        /// 未開始の場合やロットが無い場合、次工程が無い場合はnull
        /// </summary>
        /// <param name="nascaLotNo"></param>
        /// <param name="magSeqNo"></param>
        /// <param name="currentProcNo"></param>
        /// <returns></returns>
        public static Order GetNextMagazineOrder(string nascaLotNo, int magSeqNo, int currentProcNo)
        {
            AsmLot lot = AsmLot.GetAsmLot(nascaLotNo);
            if (lot == null) return null;

            Process next = Process.GetNextProcess(currentProcNo, lot);
            if (next == null) return null;

            Order[] orders = GetOrder(nascaLotNo, next.ProcNo);

            foreach (Order o in orders)
            {
                if (o.DevidedMagazineSeqNo == 0 || o.DevidedMagazineSeqNo == magSeqNo)
                {
                    return o;
                }
            }

            //現在工程が分割前で次工程が分割工程の場合
            //1指図だけ未完了ならそれが次の指図
            if (magSeqNo == 0 && orders.Where(o => o.IsComplete == false).Count() == 1)
            {
                return orders.Where(o => o.IsComplete == false).FirstOrDefault();
            }
            else if (magSeqNo == 0 && orders.Length >= 1)
            {
                //2つ以上の完了済み指図がある場合は先頭指図
                //今後の仕様変更時に注意必要
                return orders[0];
            }

            return null;
        }
        #endregion

        /// <summary>
        /// 指定工程の前工程指図を返す
        /// 未開始の場合やロットが無い場合、前無い場合はnull
        /// </summary>
        /// <param name="magazineLotNo"></param>
        /// <param name="currentProcNo"></param>
        /// <returns></returns>
        public static Order GetPrevMagazineOrder(string magazineLotNo, int currentProcNo)
        {
            string nascalot = MagLotToNascaLot(magazineLotNo);
            int seqno = ParseMagSeqNo(magazineLotNo);

            AsmLot lot = AsmLot.GetAsmLot(nascalot);
            if (lot == null) return null;

            return GetPrevMagazineOrder(nascalot, lot.TypeCd, seqno, currentProcNo);
        }

        /// <summary>
        /// 指定工程の前工程指図を返す(型番を引数に追加したもの)
        /// 未開始の場合やロットが無い場合、前無い場合はnull
        /// </summary>
        /// <param name="magazineLotNo"></param>
        /// <param name="typeCd"></param>
        /// <param name="currentProcNo"></param>
        /// <returns></returns>
        public static Order GetPrevMagazineOrder(string magazineLotNo, string typeCd, int currentProcNo)
        {
            string nascalot = MagLotToNascaLot(magazineLotNo);
            int seqno = ParseMagSeqNo(magazineLotNo);

            return GetPrevMagazineOrder(nascalot, typeCd, seqno, currentProcNo);
        }

        /// <summary>
        /// 指定工程の前工程指図を返す
        /// 未開始の場合やロットが無い場合、前工程が無い場合はnull
        /// </summary>
        /// <param name="nascaLotNo"></param>
        /// <param name="magSeqNo"></param>
        /// <param name="currentProcNo"></param>
        /// <returns></returns>
        public static Order GetPrevMagazineOrder(string nascaLotNo, string typeCd, int magSeqNo, int currentProcNo)
        {
            Process prev = Process.GetPrevProcess(currentProcNo, typeCd);
            if (prev == null) return null;

            Order[] orders = GetOrder(nascaLotNo, prev.ProcNo);
            foreach (Order o in orders)
            {
                if (o.DevidedMagazineSeqNo == 0 || o.DevidedMagazineSeqNo == magSeqNo)
                {
                    return o;
                }
            }

            return null;
        }

        /// <summary>
        /// 指定装置で現在稼働中の指図一覧取得
        /// </summary>
        /// <param name="inlineMacno"></param>
        /// <returns></returns>
        public static Order[] GetCurrentWorkingOrderInMachine(int inlineMacno)
        {
            return SearchOrder(null, null, inlineMacno, true, false);
        }

        public static Order GetLastOrderInMachine(int macno, int procno)
        {
            Order[] orders = SearchOrder(null, procno, macno, false, false);
            if (orders.Count() == 0)
            {
                return null;
            }
            if (orders.Count() == 1)
            {
                return orders[0];
            }
            else
            {
                return orders.OrderByDescending(o => o.WorkStartDt).ToList()[0];
            }
        }

        public static Order GetLastCompOrderInMachine(int macno, int procno, DateTime endto)
        {
            Order[] orders = SearchOrder(null, null, procno, macno, false, false, null, null, null, endto);
            if (orders.Count() == 0)
            {
                return null;
            }
            if (orders.Count() == 1)
            {
                return orders[0];
            }
            else
            {
                return orders.OrderByDescending(o => o.WorkStartDt).ToList()[0];
            }
        }

        /// <summary>
        /// 装置ログとロットの紐付け用
        /// </summary>
        /// <param name="macno"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Order GetMachineOrder(int macno, DateTime target)
        {
            //先に完了登録済みの実績がないか確認
            Order[] orders = SearchOrder(null, null, null, macno, false, false, null, target, target, null);
            if (orders.Count() == 0)
            {
                //完了登録済み実績が無い場合は、現在作業中の実績の内、開始時間が早い実績を取得
                orders = SearchOrder(null, null, null, macno, true, false, null, target, null, null);
                if (orders.Count() == 0)
                {
                    return null;
                }
                else
                {
                    orders = orders.OrderBy(o => o.WorkStartDt).ToArray();
                    return orders.First();
                }
            }
            else
            {
                //完了登録済みの実績に紐付く場合はそれを返す
                return orders.Single();
            }
        }

        public static Order[] GetMachineOrderStart(int macno, DateTime from, DateTime to)
        {
            return SearchOrder(null, null, null, macno, false, false, from, to, null, null);
        }

        public static Order[] GetMachineOrder(int macno, DateTime from, DateTime to)
        {
            return SearchOrder(null, null, null, macno, false, false, null, null, from, to);
        }

        public static Order GetMachineOrder(int macno, string lotno)
        {
            Order[] orders = SearchOrder(lotno, null, null, macno, false, false, null, null, null, null);
            if (orders.Count() == 0)
            {
                return null;
            }
            else if (orders.Count() >= 2)
            {
                throw new ApplicationException(
                    string.Format("複数の実績が存在する為、取得に失敗しました。MacNo:{0} LotNo:{1}", macno, lotno));
            }
            else
            {
                return orders.Single();
            }
        }


        public static Order GetMachineOrder(int macno,string lotno,int procno)
        {
            VirtualMag[] mags = VirtualMag.GetVirtualMag(macno.ToString(),((int)Station.Unloader).ToString(), lotno, procno);
            mags = mags.OrderBy(m => m.orderid).ToArray();

            if (mags.Length == 0) return null;


            Order[] orders = SearchOrder(mags.First().MagazineNo, null, procno, macno, false, false, null, null, null, null);
            if (orders.Count() == 0) return null;

            orders = orders.Where(o => o.IsComplete == false).ToArray();
            if (orders.Count() == 0)
            {
                return null;
            }

            if (orders.Count() >= 2)
            {
                throw new ApplicationException(string.Format("同一作業・ロットで装置実績が複数存在します。装置:{0}", macno));
            }

            return orders.Single();
        }

        public static Order GetMachineStartOrder(int macno)
		{
            VirtualMag[] mags = VirtualMag.GetVirtualMag(macno, ((int)Station.Loader));
            mags = mags.OrderBy(m => m.orderid).ToArray();

            if (mags == null) return null;

            Order[] orders = SearchOrder(mags.First().MagazineNo, null, null, macno, false, false, null, null, null, null);
			if (orders.Count() == 0) return null;

            orders = orders.Where(o => o.IsComplete == false).ToArray();
			if (orders.Count() == 0) 
			{
				return null;
			}

			if (orders.Count() >= 2) 
			{
				throw new ApplicationException(string.Format("装置開始実績が複数存在します。装置:{0}", macno));
			}

			return orders.Single();
		}

        public static Order[] SearchOrder(string nascalotNo, int? procNo, int? macno, bool onlyWorking, bool onlyNascaUnsync)
        {
            return SearchOrder(nascalotNo, null, procNo, macno, onlyWorking, onlyNascaUnsync, null, null, null, null);
        }

        public static Order[] SearchOrder(string nascalotNo, int? magSeqNo, int? procNo, int? macno, bool onlyWorking, bool onlyNascaUnsync,
            DateTime? startfrom, DateTime? startto, DateTime? endfrom, DateTime? endto)
        {
            return SearchOrder(nascalotNo, null, magSeqNo, procNo, macno, onlyWorking, onlyNascaUnsync, startfrom, startto, endfrom, endto, SQLite.ConStr);
        }

        public static Order[] SearchOrder(string nascalotNo, IEnumerable<string> nascalots, int? magSeqNo, int? procNo, int? macno, bool onlyWorking, 
            bool onlyNascaUnsync, DateTime? startfrom, DateTime? startto, DateTime? endfrom, DateTime? endto, string ConStr)
        {
            List<Order> retv = new List<Order>();

            //マガジン分割対応
            nascalotNo = MagLotToNascaLot(nascalotNo);

            using (SqlConnection con = new SqlConnection(ConStr))
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
                          t.isnascastart,
                          t.isnascaend,
                          t.comment,
						  t.isdefectend,
						  t.isdefectautoimportend,
						  t.isnascarunning,
				          t.isautoimport,
                          t.isresinmixordered
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

                    if (nascalots != null && nascalots.Any() == true)
                    {
                        cmd.CommandText += $" AND lotno in ('{string.Join("','", nascalots)}') ";
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
                            o.Comment = SQLite.ParseString(reader["comment"]);
                            o.TranStartEmpCd = SQLite.ParseString(reader["transtartempcd"]);
                            o.TranCompEmpCd = SQLite.ParseString(reader["trancompempcd"]);
                            o.InspectEmpCd = SQLite.ParseString(reader["inspectempcd"]);
                            o.InspectCt = SQLite.ParseInt(reader["inspectct"]);
                            o.DelFg = false;
                            o.IsDefectEnd = SQLite.ParseBool(reader["isdefectend"]);
                            o.IsDefectAutoImportEnd = SQLite.ParseBool(reader["isdefectautoimportend"]);
                            o.IsNascaRunning = SQLite.ParseBool(reader["isnascarunning"]);
                            if (!string.IsNullOrEmpty(o.ScNo2))
                            {
                                o.StockerChangeCt = int.Parse(o.ScNo2.Split('-')[0]);
                            }

                            string st1 = string.IsNullOrEmpty(o.ScNo1) ? "0" : o.ScNo1;
                            string st2 = string.IsNullOrEmpty(o.ScNo2) ? "0" : o.ScNo2;

                            //マガジン分割対応
                            if (!magSeqNo.HasValue || o.DevidedMagazineSeqNo == magSeqNo || o.DevidedMagazineSeqNo == 0)
                            {
                                retv.Add(o);
                            }
                            o.IsAutoImport = SQLite.ParseBool(reader["isAutoImport"]);
                            o.IsResinMixOrdered = SQLite.ParseBool(reader["isresinmixordered"]);
                        }
                    }

                    return retv.ToArray();
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }
        }

        #endregion

        public static Order CreateNewOrder(int procNo)
        {
            Order retv = new Order();
            retv.WorkStartDt = DateTime.Now;
            retv.ProcNo = procNo;

            return retv;
        }

        #region RecordDummyWork

        /// <summary>
        /// ダミー作業を登録
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <param name="dummymacno"></param>
        /// <param name="mag"></param>
        public static void RecordDummyWork(AsmLot lot, int procno, int dummymacno, Magazine mag)
        {
            DateTime? trandt = null;
            RecordDummyWork(lot, procno, dummymacno, mag, trandt);
        }

        /// <summary>
        /// ダミー作業を登録 (時間指定)
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <param name="dummymacno"></param>
        /// <param name="mag"></param>
        public static void RecordDummyWork(AsmLot lot, int procno, int dummymacno, Magazine mag, DateTime? trandt)
        {
            Order prevOrder = Order.GetPrevMagazineOrder(mag.NascaLotNO, procno);
            if (prevOrder == null || prevOrder.IsComplete == false) return;

            Order ord = new Order();
            ord.InMagazineNo = mag.MagazineNo;
            ord.OutMagazineNo = mag.MagazineNo;
            ord.ProcNo = procno;
            ord.WorkStartDt = trandt ?? DateTime.Now;
            ord.WorkEndDt = trandt ?? DateTime.Now;
            ord.MacNo = dummymacno;
            ord.IsDefectEnd = true;
            ord.DeleteInsert(mag.NascaLotNO);
            Log.SysLog.Info(string.Format("[ダミー作業完了登録] 完了 PC:{0} MagazineNo:{1} ProcNo:{2} MacNo:{3}", Dns.GetHostName(), mag.MagazineNo, procno, dummymacno));

            mag.NowCompProcess = procno;
            mag.Update();
        }

        /// <summary>
        /// ダミー作業を登録 (マガジン統合作業)
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <param name="dummymacno"></param>
        /// <param name="mag"></param>
        public static void RecordDummyWork(AsmLot lot, int procno, int dummymacno, Magazine mag, Magazine mag2)
        {
            Order prevOrder = Order.GetPrevMagazineOrder(mag.NascaLotNO, procno);
            if (prevOrder == null || prevOrder.IsComplete == false) return;

            mag.NowCompProcess = procno;
            mag.NewFg = false;
            mag.Update();

            mag2.NowCompProcess = procno;
            mag2.NewFg = false;
            mag2.Update();

            // #なしロット番号に変換
            mag.NascaLotNO = MagLotToNascaLot(mag.NascaLotNO);
            mag.MagazineNo = MagLotToNascaLot(mag.MagazineNo);

            Order ord = new Order();
            ord.InMagazineNo = mag.MagazineNo;
            ord.OutMagazineNo = mag.MagazineNo;
            ord.ProcNo = procno;
            ord.WorkStartDt = DateTime.Now;
            ord.WorkEndDt = DateTime.Now;
            ord.MacNo = dummymacno;
            ord.IsDefectEnd = true;
            ord.DeleteInsert(mag.NascaLotNO);
            Log.SysLog.Info(string.Format("[ダミー作業完了登録(統合)] 完了 PC:{0} MagazineNo:{1}+{4} ProcNo:{2} MacNo:{3}", Dns.GetHostName(), mag.MagazineNo, procno, dummymacno, mag2.MagazineNo));

            mag.NewFg = true;
            mag.Update();
        }
        #endregion

        public static void ReturnDummyWork(Magazine mag, int dummyprocno, int nowcompprocessno)
        {
            Order[] dummyOrders = Order.SearchOrder(mag.NascaLotNO, dummyprocno, null, false, false);
            foreach (Order o in dummyOrders)
            {
                o.DelFg = true;
                o.DeleteInsert(o.LotNo);
            }
            mag = Magazine.GetCurrent(mag.MagazineNo);
            mag.NowCompProcess = nowcompprocessno;
            mag.Update();
            Log.SysLog.Info(string.Format("[ダミー作業削除] 完了 PC:{0} LotNo:{3} MagazineNo:{1} ProcNo:{2}", Dns.GetHostName(), mag.MagazineNo, dummyprocno, mag.NascaLotNO));
        }

        #region updateDb プロパティ内容でDBを更新


        public void DeleteInsert(string lotno)
        {
            DeleteInsert(lotno, false);
        }

        public void DeleteInsert(string lotno, bool isManualEdit)
        {
            DeleteInsert(lotno, isManualEdit, SQLite.ConStr);
        }

        /// <summary>
        /// 作業履歴の更新
        /// 内部では整合確認無しでDeleteInsert
        /// </summary>
        /// <param name="wsInfo"></param>
        public void DeleteInsert(string lotno, bool isManualEdit, string constr)
        {
            //TODO 2016.06.20 SIGMAでは下記処理はコメントアウトしてるけどどうする？
            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            if (!isManualEdit)
            {
                //原材料の投入除外対策
                //事前に完了時間が入っている指図は同一開始時間、装置であれば、事前データの終了日を優先する
                //PDAで終了時間が書き換わっている想定
                //ライン間受渡し時は必ずisManualEdit=Trueの想定なので必要なし
                Order prevData = Order.GetMagazineOrder(lotno, this.ProcNo);
                if (prevData != null)
                {
                    if (prevData.WorkStartDt == this.WorkStartDt && prevData.MacNo == this.MacNo && prevData.WorkEndDt != null)
                    {
                        this.WorkEndDt = prevData.WorkEndDt;
                    }
                }
            }
			

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                //マガジン分割対応
                this.LotNo = lotno;
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;

                cmd.Parameters.Add("@PROC", SqlDbType.BigInt).Value = this.ProcNo;
                cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.MacNo;

                cmd.Parameters.Add("@INMAG", SqlDbType.NVarChar).Value = this.InMagazineNo ?? "";
                cmd.Parameters.Add("@OUTMAG", SqlDbType.NVarChar).Value = this.OutMagazineNo ?? "";

                cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = this.WorkStartDt;
                cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = (object)this.WorkEndDt ?? DBNull.Value;

                cmd.Parameters.Add("@ISCOMPLT", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsComplete);

                cmd.Parameters.Add("@ST1", SqlDbType.NVarChar).Value = (object)this.ScNo1 ?? DBNull.Value;
                cmd.Parameters.Add("@ST2", SqlDbType.NVarChar).Value = (object)this.ScNo2 ?? DBNull.Value;

                cmd.Parameters.Add("@COMMENT", SqlDbType.NVarChar).Value = this.Comment ?? "";

                cmd.Parameters.Add("@INSPECTCT", SqlDbType.BigInt).Value = this.InspectCt;
                cmd.Parameters.Add("@INSPECTEMPCD", SqlDbType.NVarChar).Value = this.InspectEmpCd ?? "";
                cmd.Parameters.Add("@TRANSTARTEMPCD", SqlDbType.NVarChar).Value = this.TranStartEmpCd ?? "";
                cmd.Parameters.Add("@TRANCOMPEMPCD", SqlDbType.NVarChar).Value = this.TranCompEmpCd ?? "";

                cmd.Parameters.Add("@ISNASCASTART", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsNascaStart);
                cmd.Parameters.Add("@ISNASCAEND", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsNascaEnd);
				cmd.Parameters.Add("@ISNASCARUNNING", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsNascaRunning);

                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.DelFg);

				cmd.Parameters.Add("@ISDEFECTEND", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsDefectEnd);
				cmd.Parameters.Add("@ISDEFECTAUTOIMPORTEND", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsDefectAutoImportEnd);

				cmd.Parameters.Add("@ISAUTOIMPORT", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsAutoImport);

                cmd.Parameters.Add("@ISRESINMIXORDERED", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsResinMixOrdered);

                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    //前履歴は削除
                    cmd.CommandText = "DELETE FROM TnTran WHERE lotno=@LOTNO AND procno=@PROC";
                    cmd.ExecuteNonQuery();

                    //新規Insert
					cmd.CommandText = @"
                            INSERT
                             INTO TnTran(lotno
	                            , procno
	                            , macno
	                            , inmag
	                            , outmag
	                            , startdt
	                            , enddt
	                            , iscomplt
	                            , stocker1
	                            , stocker2
                                , comment
                                , inspectct
                                , inspectempcd
                                , transtartempcd
                                , trancompempcd
                                , isnascastart
                                , isnascaend
							    , isnascarunning
	                            , lastupddt
	                            , delfg
								, isdefectend
								, isdefectautoimportend
                                , isautoimport
                                , isresinmixordered)
                            values(@LOTNO
	                            , @PROC
	                            , @MACNO
	                            , @INMAG
	                            , @OUTMAG
	                            , @STARTDT
	                            , @ENDDT
	                            , @ISCOMPLT
	                            , @ST1
	                            , @ST2
                                , @COMMENT
                                , @INSPECTCT
                                , @INSPECTEMPCD
                                , @TRANSTARTEMPCD
                                , @TRANCOMPEMPCD
                                , @ISNASCASTART
                                , @ISNASCAEND
								, @ISNASCARUNNING
	                            , @UPDDT
	                            , @DELFG
								, @ISDEFECTEND
								, @ISDEFECTAUTOIMPORTEND
                                , @ISAUTOIMPORT
                                , @ISRESINMIXORDERED)";

                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();

					if (IsComplete)
					{
						Log.SysLog.Info(string.Format("[作業完了登録] 完了 PC:{0} LotNo:{1} MacNo:{2} ProcNo:{3}", Dns.GetHostName(), this.LotNo, this.MacNo, this.ProcNo));
					}
					else 
					{
						Log.SysLog.Info(string.Format("[作業開始登録、更新] 完了 PC:{0} LotNo:{1} MacNo:{2} ProcNo:{3} 完了日時:{4}", Dns.GetHostName(), this.LotNo, this.MacNo, this.ProcNo, (object)this.WorkEndDt ?? ""));
					}
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    cmd.Transaction.Rollback();
                    throw ex;
                }
            }

            //指図削除時は不良も全削除
            //ライン間受渡しではdelfg=trueは発生しない
            if (this.DelFg == true)
            {
                Defect d = Defect.GetDefect(this.LotNo, this.ProcNo);
                foreach (DefItem item in d.DefItems)
                {
                    item.DefectCt = 0;
                }
                d.DeleteInsert();
            }

            //抜き取り検査工程の場合は抜き取りフラグON
            //ライン間受渡しの場合(SQLite.ConStr!=constr))はこの判定を行わない
            if (this.IsComplete && SQLite.ConStr == constr)
            {
                //原材料起因の抜き取り検査設定
                Inspection.Sampling(this, lotno);

                //抜き取り工程終了のフラグ設定
                Process p = Process.GetProcess(this.ProcNo);
                if (p != null && p.IsSamplingInspection)
                {
                    Inspection isp = Inspection.GetInspection(lotno, p.ProcNo);
                    if (isp != null)
                    {
                        isp.IsInspected = true;
                        isp.DeleteInsert();
                    }

                    Restrict[] reslist = Restrict.SearchRestrict(lotno, p.ProcNo, false);
                    foreach (Restrict res in reslist)
                    {
                        //周辺強度による規制理由と同じか確認
                        if (res.Reason == Restrict.RESTRICT_REASON_WIRE_2)
                        {
                            //規制を有効化
                            res.DelFg = false;
                            res.Save();
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 作業履歴の削除
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="lotno"></param>
        public static void Delete(SqlCommand cmd, string lotno)
        {
            if (string.IsNullOrEmpty(lotno)) 
            {
                return;
            }

            string sql = " UPDATE TnTran SET delfg = 1 WHERE lotno Like @LOTNO ";
            cmd.CommandText = sql;

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new ArmsException("作業履歴削除エラー:" + lotno, ex);
            }
        }

        /// <summary>
        /// Lotno削除
        /// </summary>
        /// <returns></returns>
        public static void Delete(string lotno)
        {
            if (string.IsNullOrEmpty(lotno))
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    string sql = " Delete TnTran WHERE lotno Like @LOTNO";
                    cmd.CommandText = sql;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("作業履歴削除エラー:" + lotno, ex);
                }
            }
        }

        /// <summary>
        /// Lotno削除
        /// </summary>
        /// <returns></returns>
        public static void Delete(string lotno,string macno)
        {
            if (string.IsNullOrEmpty(lotno))
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    string sql = " Delete TnTran WHERE lotno Like @LOTNO AND macno=@MACNO";
                    cmd.CommandText = sql;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";
                    cmd.Parameters.Add("@MACNO", SqlDbType.NVarChar).Value = macno;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("作業履歴削除エラー:" + lotno, ex);
                }
            }
        }
        /// <summary>
        /// 使用原材料の一覧取得（装置割り付け分含む）
        /// </summary>
        /// <returns></returns>
        public Material[] GetMaterials(bool isOrderComplete, bool isWorkStart)
        {
            List<Material> retv = new List<Material>();

            if (isOrderComplete)
            {
                if (this.IsComplete == false)
                {
                    return retv.ToArray();
                }
            }

            MachineInfo machine = MachineInfo.GetMachine(this.MacNo);
            if (machine != null)
            {
                if (isWorkStart)
                {
                    //開始登録時は問答無用でこの処理に
                    retv.AddRange(machine.GetMaterials(WorkStartDt, WorkEndDt));

                    //仮想マガジンのフレームを取得
                    retv.AddRange(GetFrameMaterial(new Location(machine.MacNo, Station.Loader)));
                }
                else
                {
                    if (machine.HasDoubleStocker)
                    {
                        retv.AddRange(machine.GetMaterialsFrameLoader(WorkStartDt, WorkEndDt, ScNo1, ScNo2));
                    }
                    else if (machine.HasWaferChanger)
                    {
                        retv.AddRange(machine.GetMaterialsDieBond(WorkStartDt, WorkEndDt, ScNo1, ScNo2));
                    }
                    else
                    {
                        retv.AddRange(machine.GetMaterials(WorkStartDt, WorkEndDt));
                    }
                }
            }

            retv.AddRange(GetRelatedMaterials());

            return retv.ToArray();
        }
        public Material[] GetMaterials() 
        {
            return GetMaterials(true, false);
        }

        /// <summary>
        /// 使用樹脂一覧取得（装置割り付け分取得）
        /// </summary>
        /// <returns></returns>
        public Resin[] GetResins()
        {
            List<Resin> retv = new List<Resin>();
            if (this.IsComplete == false)
            {
                return retv.ToArray();
            }

            MachineInfo machine = MachineInfo.GetMachine(this.MacNo);
            if (machine != null)
            {
                retv.AddRange(machine.GetResins(WorkStartDt, WorkEndDt));
            }
            retv.AddRange(GetRelatedResins());
            return retv.ToArray();
        }

        public Material[] GetRelatedMaterials()
        {
            List<Material> retv = new List<Material>();
            if (string.IsNullOrEmpty(this.LotNo))
            {
                return retv.ToArray();
            }

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.CommandText = @"
                       select lotno, procno, materialcd, matlotno, inputct from TnMatRelation
                       where lotno = @LOTNO AND procno = @PROCNO AND resinmixresultid IS NULL";

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                    cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = this.ProcNo;
                    
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        int ordInputCt = rd.GetOrdinal("inputct");

                        while (rd.Read())
                        {
                            string matcd = rd["materialcd"].ToString();
                            string lotno = rd["matlotno"].ToString();

                            Material mat = Material.GetMaterial(matcd, lotno);
                            if (mat != null)
                            {
                                if(!rd.IsDBNull(ordInputCt))
                                {
                                    mat.InputCt = rd.GetDecimal(ordInputCt);
                                }
                                retv.Add(mat);
                            }
                        }
                    }

                    return retv.ToArray();
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }
        }

        public static MatRelation[] GetRelatedMaterials(string lotno) 
        {
            List<MatRelation> retv = new List<MatRelation>();
            if (string.IsNullOrEmpty(lotno))
            {
                return retv.ToArray();
            }

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.CommandText = @"
                       select procno, materialcd, matlotno, resinmixresultid, inputct
                       from TnMatRelation WITH(nolock)
                       where lotno Like @LOTNO ";

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        int ordInputCt = rd.GetOrdinal("inputct");

                        while (rd.Read())
                        {
                            MatRelation matRelate = new MatRelation();
                            matRelate.LotNo = lotno;
                            matRelate.ProcNo = SQLite.ParseInt(rd["procno"]);
                            matRelate.MaterialCd = SQLite.ParseString(rd["materialcd"]);
                            matRelate.MatLotNo = SQLite.ParseString(rd["matlotno"]);
                            matRelate.ResinmixResultId = SQLite.ParseInt(rd["resinmixresultid"]);

                            if (!rd.IsDBNull(ordInputCt))
                            {
                                matRelate.inputCt = rd.GetDecimal(ordInputCt);
                            }

                            retv.Add(matRelate);
                        }
                    }

                    return retv.ToArray();
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }
        }

        public Material[] GetFrameMaterial(Location loc)
        {
            List<Material> retv = new List<Material>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.CommandText = @"
                       SELECT framematerialcd, framelotno FROM TnVirtualMag WITH(nolock)
                       where macno = @MACNO AND locationid = @LOCID ";

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = loc.MacNo;
                    cmd.Parameters.Add("@LOCID", SqlDbType.Int).Value = (int)loc.Station;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        int ordFramematerialcd = rd.GetOrdinal("framematerialcd");
                        int ordFramelotno = rd.GetOrdinal("framelotno");

                        while (rd.Read())
                        {
                            string matcd = string.Empty;
                            if (!rd.IsDBNull(ordFramematerialcd))
                            {
                                matcd = rd.GetString(ordFramematerialcd).Trim();
                            }

                            string lotno = string.Empty;
                            if (!rd.IsDBNull(ordFramelotno))
                            {
                                lotno = rd.GetString(ordFramelotno).Trim();
                            }

                            if (string.IsNullOrEmpty(matcd) || string.IsNullOrEmpty(lotno)) 
                            {
                                continue;
                            }

                            Material mat = Material.GetMaterial(matcd, lotno);
                            if (mat != null)
                            {
                                retv.Add(mat);
                            }
                        }
                    }

                    return retv.ToArray();
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 関連付いている樹脂を取得
        /// </summary>
        /// <returns></returns>
        public Resin[] GetRelatedResins()
        {
            List<Resin> retv = new List<Resin>();
            if (string.IsNullOrEmpty(this.LotNo))
            {
                return retv.ToArray();
            }

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.CommandText = @"
                       select lotno, procno, resinmixresultid from TnMatRelation
                       where lotno = @LOTNO AND procno = @PROCNO AND resinmixresultid IS NOT NULL";

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                    cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = this.ProcNo;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            int mixresultid = SQLite.ParseInt(rd["resinmixresultid"]);

                            Resin r = Resin.GetResin(mixresultid);
                            if (r != null)
                            {
                                retv.Add(r);
                            }
                        }
                    }

                    return retv.ToArray();
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }
        }

        #region UpdateMaterialRelation ロットに直接紐づける原材料情報の更新

        /// <summary>
        /// ロットに直接紐づける原材料情報の更新
        /// </summary>
        public void UpdateMaterialRelation(Material[] matlist)
        {
            UpdateMaterialRelation(matlist, SQLite.ConStr);
        }

        /// <summary>
        /// ロットに直接紐づける原材料情報の更新
        /// </summary>
        public void UpdateMaterialRelation(Material[] matlist, string constr)
        {
            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            Log.SysLog.Info("原材料割り付け情報更新" + this.LotNo);

            if (string.IsNullOrEmpty(this.LotNo)) return;

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo ?? "";
                    cmd.Parameters.Add("@PROC", SqlDbType.BigInt).Value = this.ProcNo;
                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                    //前履歴は削除
                    //樹脂調合結果ありのレコードは消さない！
                    cmd.CommandText = "DELETE FROM TnMatRelation WHERE lotno=@LOTNO AND procno=@PROC AND resinmixresultid IS NULL";
                    cmd.ExecuteNonQuery();

                    SqlParameter prmMatrialCd = cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar);
                    SqlParameter prmMatLotNo = cmd.Parameters.Add("@MATLOTNO", SqlDbType.NVarChar);
                    SqlParameter prmInputCt= cmd.Parameters.Add("@INPUTCT", SqlDbType.Decimal);

                    //新規Insert
                    cmd.CommandText = @"
                            INSERT
                             INTO TnMatRelation(lotno
	                            , procno
	                            , materialcd
	                            , matlotno
                                , lastupddt
                                , inputct)
                            values(@LOTNO
	                            , @PROC
	                            , @MATCD
	                            , @MATLOTNO
	                            , @UPDDT
                                , @INPUTCT)";

                    List<Material> exists = new List<Material>();
                    foreach (Material m in matlist)
                    {
                        //foreach (Material exist in exists)
                        //{
                        //    //重複排除
                        //    if (exist.MaterialCd == m.MaterialCd && exist.LotNo == m.LotNo) continue;
                        //}
                        //重複排除
                        if (exists.Exists(exist => exist.MaterialCd == m.MaterialCd && exist.LotNo == m.LotNo)) continue;

                        if (m == null) continue;

                        prmMatrialCd.Value = m.MaterialCd;
                        prmMatLotNo.Value = m.LotNo;
                        prmInputCt.Value = m.InputCt ?? (object)DBNull.Value;

                        cmd.ExecuteNonQuery();
                        exists.Add(m);
                    }

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("原材料割り付け情報更新エラー", ex);
                }
            }
        }
        #endregion

        #region UpdateResinRelation ロットに直接紐づける調合結果ID情報の更新

        /// <summary>
        /// ロットに直接紐づける原材料情報の更新
        /// </summary>
        public void UpdateResinRelation(Resin[] resinlist)
        {
            UpdateResinRelation(resinlist, SQLite.ConStr);
        }

        /// <summary>
        /// ロットに直接紐づける原材料情報の更新
        /// </summary>
        public void UpdateResinRelation(Resin[] resinlist, string constr)
        {
            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            Log.SysLog.Info("樹脂割り付け情報更新" + this.LotNo);

            if (string.IsNullOrEmpty(this.LotNo)) return;

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo ?? "";
                    cmd.Parameters.Add("@PROC", SqlDbType.BigInt).Value = this.ProcNo;
                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                    //前履歴は削除
                    //樹脂調合結果ありのレコードしか消さない！
                    cmd.CommandText = "DELETE FROM TnMatRelation WHERE lotno=@LOTNO AND procno=@PROC AND resinmixresultid IS NOT NULL";
                    cmd.ExecuteNonQuery();

                    SqlParameter prmResinMixResultId = cmd.Parameters.Add("@MIXID", SqlDbType.BigInt);

                    //新規Insert
                    cmd.CommandText = @"
                            INSERT
                             INTO TnMatRelation(lotno
	                            , procno
	                            , resinmixresultid
                                , lastupddt)
                            values(@LOTNO
	                            , @PROC
	                            , @MIXID
	                            , @UPDDT)";

                    List<Resin> exists = new List<Resin>();
                    foreach (Resin r in resinlist)
                    {
                        if (r == null) continue;

                        foreach (Resin exist in exists)
                        {
                            //重複排除
                            if (exist.MixResultId == r.MixResultId) continue;
                        }

                        prmResinMixResultId.Value = r.MixResultId;
                        cmd.ExecuteNonQuery();
                        exists.Add(r);
                    }

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("樹脂割り付け情報更新エラー", ex);
                }
            }
        }
        #endregion

        public static void DeleteMaterialRelation(SqlCommand cmd, string lotno) 
        {
            if (string.IsNullOrEmpty(lotno))
            {
                return;
            }

            //削除ログ出力用
            MatRelation[] matRelates = GetRelatedMaterials(lotno);

            string sql = " DELETE FROM TnMatRelation WHERE lotno Like @LOTNO ";
            cmd.CommandText = sql;

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";

            try
            {
                cmd.ExecuteNonQuery();

                foreach (MatRelation matRelate in matRelates)
                {
                    Log.DelLog.Info(string.Format("[TnMatRelation] {0}\t{1}\t{2}\t{3}\t{4}\t{5}",
                        matRelate.LotNo, matRelate.ProcNo, matRelate.MaterialCd, matRelate.MatLotNo, matRelate.ResinmixResultId, System.DateTime.Now));
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("MatRelation削除エラー:" + lotno, ex);
            }
        }

        /// <summary>
        /// 該当マガジンを対象装置で作業した一番最後のレコードから工程を取得
        /// ARMSのWorkComplete内で仮想マガジンのProcNo取得用
        /// </summary>
        /// <param name="macno"></param>
        /// <param name="magno"></param>
        /// <returns></returns>
        public static int GetLastProcNo(int? macno, string magno)
        {
            Magazine mag = Magazine.GetCurrent(magno);
            if (mag == null)
            {
                throw new ArmsException("マガジン情報が存在しません " + magno);
            }
            AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
            if (lot == null)
            {
                throw new ArmsException("ロット情報が存在しません " + magno);
            }

            //該当装置のレコード全取得（OnlyWorkingはFalse　完了上書きが有り得る為）
            Order[] orders = Order.SearchOrder(lot.NascaLotNo, null, macno, false, false);
            if (orders.Length == 0)
            {
                throw new ArmsException("作業開始レコードが見つかりません mag:" + magno + " 装置:" + macno.ToString());
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

        public static int GetLastProcNo(string magno)
        {
            return GetLastProcNo(null, magno);
        }

        public static int GetLastProcNoFromLotNo(string lotNo)
        {
            Order[] orders = Order.GetOrder(lotNo);
            if (orders.Length == 0)
            {
                throw new ArmsException("作業開始レコードが見つかりません mag:" + lotNo);
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

        /// <summary>
        /// 該当マガジンを対象装置で作業した一番最後のレコードから工程を取得
        /// ARMSのWorkComplete内で仮想マガジンのProcNo取得用
        /// 分割マガジン対応
        /// </summary>
        /// <param name="macno"></param>
        /// <param name="magno"></param>
        /// <returns></returns>
        public static int GetLastProcNoWithDevidedMag(int macno, string magno)
        {
            Magazine mag = Magazine.GetCurrent(magno);
            if (mag == null)
            {
                throw new ArmsException("マガジン情報が存在しません " + magno);
            }
            AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
            if (lot == null)
            {
                throw new ArmsException("ロット情報が存在しません " + magno);
            }

            //該当装置のレコード全取得（OnlyWorkingはFalse　完了上書きが有り得る為）
            int seqno = Order.ParseMagSeqNo(mag.NascaLotNO);
            Order[] orders = Order.SearchOrder(lot.NascaLotNo, seqno, null, macno, false, false, null, null, null, null);
            if (orders.Length == 0)
            {
                throw new ArmsException("作業開始レコードが見つかりません mag:" + magno + " 装置:" + macno.ToString());
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



		public static int GetLastProcNoFromLot(int macno, string lotno)
		{
			AsmLot lot = AsmLot.GetAsmLot(lotno);
			if (lot == null)
			{
				throw new ArmsException("ロット情報が存在しません " + lotno);
			}

			//該当装置のレコード全取得（OnlyWorkingはFalse　完了上書きが有り得る為）
			Order[] orders = Order.SearchOrder(lot.NascaLotNo, null, macno, false, false);
			if (orders.Length == 0)
			{
				throw new ArmsException("作業開始レコードが見つかりません LotNo:" + lotno + " 装置:" + macno.ToString());
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

		///// <summary>
		///// 任意の日時から紐付く工程を実績データから取得
		///// [注意]一意になる想定だが、キー列を指定していない為にならないケース有り
		///// </summary>
		///// <param name="macno"></param>
		///// <param name="lotno"></param>
		///// <param name="targetdt"></param>
		///// <returns></returns>
		//public static int GetDatePointProcNo(int macno, string lotno, DateTime targetdt)
		//{
		//	Order[] orders =  Order.SearchOrder(lotno, null, null, macno, false, false, null, targetdt, targetdt, null);
		//	if (orders.Length == 0) 
		//	{
		//		throw new ArmsException(string.Format("完了実績が見つかりません。 MacNo:{0} LotNo:{1} DatePoint:{2}", 
		//			macno, lotno, targetdt));
		//	}
		//	if (orders.Length >= 2) 
		//	{
		//		throw new ArmsException(string.Format("完了実績が複数存在します。 MacNo:{0} LotNo:{1} DatePoint:{2}",
		//			macno, lotno, targetdt));
		//	}
		//	return orders.Single().ProcNo;
		//}
		
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

        /// <summary>
        /// 過去の実績ロット番号を取得する
        /// </summary>
        /// <param name="fromDt">対象設備</param>
        /// <param name="fromDt">遡る起点となる日付</param>
        /// <param name="recordCount">遡るレコード数</param>
        /// <returns></returns>
        public static List<Order> GetOrderRecordList(int macno, DateTime fromDt, DateTime? toDt, int recordCount)
        {
            List<Order> retv = new List<Order>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = $@" SELECT TOP {recordCount} TnTran.lotno, TnTran.procno FROM TnTran WITH(nolock)
                                        INNER JOIN TnMag ON TnTran.lotno = TnMag.lotno
                                        WHERE macno = @MacNo AND enddt < @FromDt AND newfg = 1 ";
                
                if (toDt.HasValue)
                {
                    cmd.CommandText += " AND enddt >= @ToDt ";
                    cmd.Parameters.Add("@ToDt", SqlDbType.DateTime).Value = toDt.Value;
                }

                cmd.CommandText += " ORDER BY TnTran.enddt DESC ";
                
                cmd.Parameters.Add("@MacNo", SqlDbType.Int).Value = macno;
                cmd.Parameters.Add("@FromDt", SqlDbType.DateTime).Value = fromDt;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        Order[] o = Order.GetOrder(rd["lotno"].ToString().Trim(), Convert.ToInt32(rd["procno"]));
                        retv.AddRange(o);
                    }
                }
            }

            return retv;
        }        

        /// <summary>
        /// 過去ロット1件目だけ取得
        /// </summary>
        /// <param name="macno"></param>
        /// <param name="fromDt"></param>
        /// <param name="toDt"></param>
        /// <returns></returns>
        public static Order GetOrderOneBeforeRecord(int macno, DateTime fromDt, DateTime? toDt)
        {
            List<Order> lotList = GetOrderRecordList(macno, fromDt, toDt, 1);
            return lotList.FirstOrDefault();
        }
    }

    public class MatRelation 
    {
        public string LotNo { get; set; }
        public int ProcNo { get; set; }
        public string MaterialCd { get; set; }
        public string MatLotNo { get; set; }
        public int ResinmixResultId { get; set; }
        public Decimal? inputCt  { get; set; }
    }

    public class MAPFrameLoadOrder : Order
    {
        public Material[] FrameList { get; set; }
        public bool IsFrameBakingTimeOver { get; set; }
    }
    
}
