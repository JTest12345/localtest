using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class History
    {
        #region プロパティ

        string _lotno;
        /// <summary>
        /// ロット番号
        /// </summary>
        public string LotNo { get { return _lotno; } set { _lotno = Order.MagLotToNascaLot(value); } }

        /// <summary>
        /// 確認フラグ
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// メンテナンス作業日
        /// </summary>
        public DateTime EditDt { get; set; }

        /// <summary>
        /// 旧マガジン番号
        /// </summary>
        public string OldMagNo { get; set; }

        /// <summary>
        /// 新マガジン番号
        /// </summary>
        public string NewMagNo { get; set; }


        /// <summary>
        /// メンテナンス工程番号
        /// </summary>
        public int? OldProcNo { get; set; }

        /// <summary>
        /// メンテナンス工程番号
        /// </summary>
        public int? NewProcNo { get; set; }

        private Process _newprocess;
        /// <summary>
        /// 対象工程(FlyWeight)
        /// </summary>
        public string NewProcNm
        {
            get
            {
                if (_newprocess == null && NewProcNo.HasValue)
                {
                    _newprocess = Process.GetProcess(NewProcNo.Value);
                }

                return _newprocess != null ? _newprocess.InlineProNM : "";
            }
        }

        private Process _oldprocess;
        /// <summary>
        /// 対象工程(FlyWeight)
        /// </summary>
        public string OldProcNm
        {
            get
            {
                if (_oldprocess == null && OldProcNo.HasValue)
                {
                    _oldprocess = Process.GetProcess(OldProcNo.Value);
                }

                return _oldprocess != null ? _oldprocess.InlineProNM : "";
            }
        }

        /// <summary>
        /// メンテナンス内容
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// メンテナンス作業者
        /// </summary>
        public string EmpCd { get; set; }

        /// <summary>
        /// 装置番号
        /// </summary>
        public int? NewMacNo { get; set; }

        /// <summary>
        /// 作業装置（FlyWeight）
        /// </summary>
        private MachineInfo _newmachine;
        public string NewMacNm
        {
            get
            {
                if (_newmachine == null && NewMacNo.HasValue)
                {
                    _newmachine = MachineInfo.GetMachine(NewMacNo.Value);
                }

                return _newmachine != null ? _newmachine.LongName : "";
            }
        }

        /// <summary>
        /// 装置番号
        /// </summary>
        public int? OldMacNo { get; set; }

        /// <summary>
        /// 作業装置（FlyWeight）
        /// </summary>
        private MachineInfo _oldmachine;
        public string OldMacNm
        {
            get
            {
                if (_oldmachine == null && OldMacNo.HasValue)
                {
                    _oldmachine = MachineInfo.GetMachine(OldMacNo.Value);
                }

                return _oldmachine != null ? _oldmachine.LongName : "";
            }
        }

        /// <summary>
        /// 作業開始時間
        /// </summary>
        public DateTime? NewStartDt { get; set; }

        /// <summary>
        /// 作業完了時間
        /// </summary>
        public DateTime? NewEndDt { get; set; }

        /// <summary>
        /// 作業開始時間
        /// </summary>
        public DateTime? OldStartDt { get; set; }

        /// <summary>
        /// 作業完了時間
        /// </summary>
        public DateTime? OldEndDt { get; set; }

        #endregion

        #region Search
        /// <summary>
        /// メンテナンス履歴検索
        /// </summary>
        /// <param name="lotno"></param>
        /// <returns></returns>
        public static History[] Search(string lotno, DateTime? from, DateTime? to)
        {
            List<History> retv = new List<History>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.CommandText = @"
                        SELECT lotno, editdt, checkedfg, oldmagno, newmagno, newprocno, oldprocno, empcd, note, newmacno, oldmacno,
                        newstartdt, oldstartdt, newenddt, oldenddt FROM TnHistory h
                        WHERE 1=1";

                    if (lotno != null)
                    {
                        cmd.CommandText += " AND h.lotno = @LOTNO";
                        cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                    }

                    if (from.HasValue)
                    {
                        cmd.CommandText += " AND h.editdt >= @FROM";
                        cmd.Parameters.Add("@FROM", SqlDbType.DateTime).Value = from;
                    }

                    if (to.HasValue)
                    {
                        cmd.CommandText += " AND h.editdt <= @TO";
                        cmd.Parameters.Add("@TO", SqlDbType.DateTime).Value = to;
                    }

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            History h = new History();
                            h.LotNo = SQLite.ParseString(rd["lotno"]);
                            h.EditDt = SQLite.ParseDate(rd["editdt"]) ?? DateTime.MinValue;
                            h.OldMagNo = SQLite.ParseString(rd["oldmagno"]);
                            h.NewMagNo = SQLite.ParseString(rd["newmagno"]);
                            h.OldProcNo = rd["oldprocno"] == DBNull.Value ? (int?)null : SQLite.ParseInt(rd["oldprocno"]);
                            h.NewProcNo = rd["newprocno"] == DBNull.Value ? (int?)null : SQLite.ParseInt(rd["newprocno"]);
                            h.EmpCd = SQLite.ParseString(rd["empcd"]);
                            h.Note = SQLite.ParseString(rd["note"]);
                            h.NewMacNo = rd["newmacno"] == DBNull.Value ? (int?)null : SQLite.ParseInt(rd["newmacno"]);
                            h.OldMacNo = rd["oldmacno"] == DBNull.Value ? (int?)null : SQLite.ParseInt(rd["oldmacno"]);
                            h.NewStartDt = SQLite.ParseDate(rd["newstartdt"]);
                            h.OldStartDt = SQLite.ParseDate(rd["oldstartdt"]);
                            h.NewEndDt = SQLite.ParseDate(rd["newenddt"]);
                            h.OldEndDt = SQLite.ParseDate(rd["oldenddt"]);
                            h.Checked = SQLite.ParseBool(rd["checkedfg"]);

                            retv.Add(h);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException("データメンテナンス履歴検索エラー:" + lotno, ex);
                }
            }
            return retv.ToArray();
        }

        public static History[] Search(string lotno)
        {
            return Search(lotno, null, null);
        }

        #endregion

        #region Insert
        /// <summary>
        /// 新規履歴保存　既存履歴の削除等は行わない
        /// </summary>
        public void Insert()
        {
            Insert(true, SQLite.ConStr);
        }



        public void Insert(bool canretry, string constr)
        {
            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                    cmd.Parameters.Add("@EDITDT", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@CHECKED", SqlDbType.Int).Value = SQLite.SerializeBool(this.Checked);
                    cmd.Parameters.Add("@OLDPROCNO", SqlDbType.BigInt).Value = this.OldProcNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@NEWPROCNO", SqlDbType.BigInt).Value = this.NewProcNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@NOTE", SqlDbType.NVarChar).Value = this.Note ?? string.Empty;
                    cmd.Parameters.Add("@EMPCD", SqlDbType.NVarChar).Value = this.EmpCd ?? string.Empty;

                    cmd.Parameters.Add("@OLDMAGNO", SqlDbType.NVarChar).Value = this.OldMagNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@NEWMAGNO", SqlDbType.NVarChar).Value = this.NewMagNo ?? (object)DBNull.Value;

                    cmd.Parameters.Add("@OLDMACNO", SqlDbType.BigInt).Value = this.OldMacNo ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@NEWMACNO", SqlDbType.BigInt).Value = this.NewMacNo ?? (object)DBNull.Value;

                    cmd.Parameters.Add("@OLDSTARTDT", SqlDbType.DateTime).Value = this.OldStartDt ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@NEWSTARTDT", SqlDbType.DateTime).Value = this.NewStartDt ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@OLDENDDT", SqlDbType.DateTime).Value = this.OldEndDt ?? (object)DBNull.Value;
                    cmd.Parameters.Add("@NEWENDDT", SqlDbType.DateTime).Value = this.NewEndDt ?? (object)DBNull.Value;


                    //新規Insert
                    cmd.CommandText = @"
                        INSERT
                         INTO TnHistory(lotno
	                        , editdt
                            , checkedfg
	                        , oldprocno
	                        , newprocno
	                        , note
	                        , empcd
	                        , oldmagno
	                        , newmagno
	                        , oldmacno
	                        , newmacno
	                        , oldstartdt
	                        , newstartdt
	                        , oldenddt
	                        , newenddt )
                        values(@LOTNO
	                        , @EDITDT
                            , @CHECKED
	                        , @OLDPROCNO
                            , @NEWPROCNO
	                        , @NOTE
	                        , @EMPCD
	                        , @OLDMAGNO
	                        , @NEWMAGNO
	                        , @OLDMACNO
	                        , @NEWMACNO
	                        , @OLDSTARTDT
	                        , @NEWSTARTDT
	                        , @OLDENDDT
	                        , @NEWENDDT)";

                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    //日付が主キーのため、エラー時は1秒まって1度だけリトライ
                    if (canretry)
                    {
                        System.Threading.Thread.Sleep(1000);
                        Insert(false, constr);
                    }
                    else
                    {
                        throw new ArmsException("データメンテナンス履歴保存エラー:" + this.LotNo, ex);
                    }
                }
            }
        }


        #endregion

        /// <summary>
        /// 確認済みフラグ更新
        /// </summary>
        /// <param name="checkedfg"></param>
        public void UpdateChecked(bool checkedfg)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                    cmd.Parameters.Add("@EDITDT", SqlDbType.DateTime).Value = this.EditDt;
                    cmd.Parameters.Add("@CHECKED", SqlDbType.Int).Value = SQLite.SerializeBool(checkedfg);

                    //新規Insert
                    cmd.CommandText = @"
                        UPDATE TnHistory SET checkedfg = @CHECKED
                        WHERE lotno = @LOTNO AND editdt = @EDITDT";

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("データメンテナンス確認フラグ更新エラー:" + this.LotNo, ex);
                }
            }
        }

        public static void Delete(SqlCommand cmd, string lotno) 
        {
            if (string.IsNullOrEmpty(lotno)) 
            {
                return;
            }

            //削除ログ出力用
            History[] historys = History.Search(lotno);
            
            string sql = " DELETE FROM TnHistory WHERE lotno = @LOTNO ";
            cmd.CommandText = sql;

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;

            try
            {
                cmd.ExecuteNonQuery();

                foreach (History history in historys)
                {
                    Log.DelLog.Info(string.Format("[TnHistory] {0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}",
                        history.LotNo, history.EditDt, history.Checked, history.OldProcNo, history.NewProcNo, history.Note, history.EmpCd, history.OldMagNo, history.NewMagNo, history.OldMacNo, history.NewMacNo, history.OldStartDt, history.NewStartDt, history.OldEndDt, history.NewEndDt));
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("メンテナンス履歴削除エラー:" + lotno, ex);
            }

        }

        #region HasUncheckedHistory
        /// <summary>
        /// 未確認のメンテナンス履歴を一つでも持っていればTrue
        /// </summary>
        /// <param name="lotno"></param>
        /// <returns></returns>
        public static bool HasUncheckedHistory(string lotno)
        {
            History[] lst = Search(lotno, null, null);
            foreach (History h in lst)
            {
                List<int> hisNoChkProcList = Config.Settings.HistoryNoCheckProcList;

                if (hisNoChkProcList != null)
                {
                    // 設定ファイルで指定していない作業番号の履歴だけをチェック対象にする
                    // 履歴に作業番号が無い場合はチェック不要とする

                    // 履歴の工程[前]が空でない かつ 設定ファイルで指定していない作業 の場合はチェック対象
                    bool isOldProc = true;
                    if (h.OldProcNo.HasValue && hisNoChkProcList.Contains(h.OldProcNo.Value) == false)
                    {
                        isOldProc = false;
                    }

                    // 履歴の工程[後]が空でない かつ 設定ファイルで指定していない作業 の場合はチェック対象
                    bool isNewProc = true;
                    if (h.NewProcNo.HasValue && hisNoChkProcList.Contains(h.NewProcNo.Value) == false)
                    {
                        isNewProc = false;
                    }

                    // 両方とも(設定ファイルで指定している作業 or 作業番号無し)の場合、チェック対象外
                    if (isOldProc && isNewProc)
                    {
                        continue;
                    }
                }

                if (h.Checked == false)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
