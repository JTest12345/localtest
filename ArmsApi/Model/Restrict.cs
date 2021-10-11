using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class Restrict
    {
        /// <summary>
        /// 周辺強度試験のための排出
        /// </summary>
        public const string RESTRICT_REASON_WIRE_2 = "WBエラーに対する周辺強度実施の為";
		public const string RESTRICT_REASON_DIESHEARSAMPLE = "ダイシェア抜き取り対象ロットの為";

		public const string TESTMODE_DIESHEAR = "DIESHEAR";
		public const string TESTMODE_PULL = "PULL";
		public const string TESTMODE_SHEAR = "SHEAR";

		public const string INSPECT_WORKCD = "WB0004";

        public const string RESTRICT_REASON_KB_NOTCHECK_WORK_START = "作業開始規制チェック対象外";

        public const string RESTRICT_REASON_PSTESTER = "強度対象ロットの為、流動禁止";
        public const string RESTRICT_REASON_KB_PSTESTER = "強度対象ロット";
        public const string RESTRICT_REASON_PSTESTER_SPREAD = "強度対象ロットの試験が未完了の為、流動禁止";
        public const string RESTRICT_REASON_KB_PSTESTER_SPREAD = "強度未完了装置での完成ロット";

        

        string _lotno;
        /// <summary>
        /// ロット番号
        /// </summary>
        public string LotNo { get { return _lotno; } set { _lotno = Order.MagLotToNascaLot(value); } }

        /// <summary>
        /// 代表ロット番号
        /// </summary>
        public string RepresentativeLotNo { get; set; }

        /// <summary>
        /// 投入規制工程
        /// </summary>
        public int ProcNo { get; set; }

        private string _procnm;
        /// <summary>
        /// 工程名称
        /// </summary>
        public string ProcNm
        {
            get
            {
                if (_procnm == null)
                {
                    Process p = Process.GetProcess(ProcNo);
                    if (p != null) _procnm = p.InlineProNM;
                }

                return _procnm ?? "";
            }
        }

        /// <summary>
        /// 規制理由
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool DelFg { get; set; }

        /// <summary>
        /// 最終更新日
        /// </summary>
        public DateTime LastUpdDt { get; set; }

        /// <summary>
        /// 規制理由区分
        /// </summary>
        public string ReasonKb { get; set; }

        /// <summary>
        /// 更新者CD
        /// </summary>
        public string UpdUserCd { get; set; }

        /// <summary>
        /// 社員番号での解除制限が有りの時にOnになるフラグ
        /// </summary>
        public bool RestrictReleaseFg { get; set; }

        #region SearchRestrict

        /// <summary>
        /// 流動規制情報検索
        /// </summary>
        /// <param name="lotno">null許可</param>
        /// <param name="procno">工程ID</param>
        /// <param name="onlyActive">DelFg除くフラグ</param>
        /// <returns></returns>
        public static Restrict[] SearchRestrict(string lotno, int? procno, bool onlyActive)
        {
            return SearchRestrict(lotno, procno, onlyActive, SQLite.ConStr, null, null);
        }

        /// <summary>
        /// 流動規制情報検索
        /// </summary>
        /// <param name="lotno">null許可</param>
        /// <param name="procno">工程ID</param>
        /// <param name="onlyActive">DelFg除くフラグ</param>
        /// <returns></returns>
        public static Restrict[] SearchRestrict(string lotno, int? procno, bool onlyActive, string constr, string representativelotno, string reason)
        {
            //マガジン分割対応
            lotno = Order.MagLotToNascaLot(lotno);

            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            List<Restrict> retv = new List<Restrict>();

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT lotno, procno, reason, delfg, lastupddt, reasonkb, updusercd, restrictreleasefg, representativelotno FROM TnRestrict WHERE 1=1 ";

                if (onlyActive)
                {
                    cmd.CommandText += " AND delfg=0";
                }

                if (lotno != null)
                {
                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                    cmd.CommandText += " AND lotno = @LOTNO";
                }

                if (procno.HasValue)
                {
                    cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno;
                    cmd.CommandText += " AND procno=@PROCNO";
                }

                if (representativelotno != null)
                {
                    cmd.Parameters.Add("@RLOTNO", SqlDbType.NVarChar).Value = representativelotno;
                    cmd.CommandText += " AND representativelotno = @RLOTNO";
                }

                if (string.IsNullOrEmpty(reason) == false)
                {
                    cmd.Parameters.Add("@REASON", SqlDbType.NVarChar).Value = reason;
                    cmd.CommandText += " AND reason = @REASON";
                }

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        Restrict r = new Restrict();
                        r.LotNo = SQLite.ParseString(rd["lotno"]);
                        r.ProcNo = SQLite.ParseInt(rd["procno"]);
                        r.Reason = SQLite.ParseString(rd["reason"]);
                        r.DelFg = SQLite.ParseBool(rd["delfg"]);
                        r.LastUpdDt = SQLite.ParseDate(rd["lastupddt"]) ?? DateTime.MinValue;
                        r.ReasonKb = SQLite.ParseString(rd["reasonkb"]);
                        r.UpdUserCd = SQLite.ParseString(rd["updusercd"]);
                        r.RestrictReleaseFg = SQLite.ParseBool(rd["restrictreleasefg"]);
                        r.RepresentativeLotNo = SQLite.ParseString(rd["representativelotno"]);

                        retv.Add(r);
                    }
                }

            }

            return retv.ToArray();
        }

        public static Restrict[] SearchRestrict(bool onlyActive, string reason)
        {
            return SearchRestrict(null, null, onlyActive, SQLite.ConStr, null, reason);
        }
        #endregion

        public void Save()
        {
            Save(SQLite.ConStr);
        }

        /// <summary>
        /// Insert or Update
        /// </summary>
        public void Save(string constr)
        {
            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            if (string.IsNullOrEmpty(LotNo)) return;

            Restrict[] exists = SearchRestrict(LotNo, ProcNo, false, constr, null, null);
            foreach (Restrict r in exists)
            {
                //規制済みデータに対して同じ条件の規制をしないようにdelfgチェック
                if (r.Reason == this.Reason)
                {
                    if (string.IsNullOrEmpty(r.UpdUserCd) == false && r.DelFg == this.DelFg) 
                    {
                        //既存規制を別の作業者で上書きしないようにチェック
                        throw new ApplicationException("既に同じ理由で規制されています。");
                    }
                    //規制理由まで全く同じものが見つかった場合はUPDATEして終了
                    r.LastUpdDt = DateTime.Now;
                    r.DelFg = this.DelFg;
                    r.ReasonKb = this.ReasonKb;
                    r.UpdUserCd = this.UpdUserCd;
                    r.RestrictReleaseFg = this.RestrictReleaseFg;
                    r.RepresentativeLotNo = this.RepresentativeLotNo;
                    r.updateDB(constr);
                    return;
                }
            }

            //既存レコードが無ければ新規Insert
            this.insertDB(constr);
        }


        #region Insert/Update


        /// <summary>
        /// Update
        /// </summary>
        private void updateDB(string constr)
        {
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @"
                        UPDATE TnRestrict SET lastupddt=@UPDDT, delfg=@DELFG, reasonkb=@REASONKB, updusercd=@UPDUSERCD,
                                              restrictreleasefg=@RESTRICTRELEASEFG, representativelotno=@REPRESENTATIVELOTNO
                        WHERE lotno=@LOTNO AND procno=@PROCNO AND reason=@REASON";

                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = this.ProcNo;
                cmd.Parameters.Add("@REASON", SqlDbType.NVarChar).Value = this.Reason;
                cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.DelFg);
                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@REASONKB", SqlDbType.NVarChar).Value = (object)this.ReasonKb ?? DBNull.Value;
                cmd.Parameters.Add("@UPDUSERCD", SqlDbType.NVarChar).Value = (object)this.UpdUserCd ?? DBNull.Value;
                cmd.Parameters.Add("@RESTRICTRELEASEFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.RestrictReleaseFg);
                cmd.Parameters.Add("@REPRESENTATIVELOTNO", SqlDbType.NVarChar).Value = (object)this.RepresentativeLotNo ?? DBNull.Value;

                cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Insert
        /// </summary>
        private void insertDB(string constr)
        {
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @"
                        INSERT TnRestrict(lotno, procno, reason, delfg, lastupddt, reasonkb, updusercd, restrictreleasefg, representativelotno)
                        VALUES (@LOTNO, @PROCNO, @REASON, @DELFG, @UPDDT, @REASONKB, @UPDUSERCD, @RESTRICTRELEASEFG, @REPRESENTATIVELOTNO)";

                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = this.ProcNo;
                cmd.Parameters.Add("@REASON", SqlDbType.NVarChar).Value = this.Reason;
                cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.DelFg);
                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@REASONKB", SqlDbType.NVarChar).Value = (object)this.ReasonKb ?? DBNull.Value;
                cmd.Parameters.Add("@UPDUSERCD", SqlDbType.NVarChar).Value = (object)this.UpdUserCd ?? DBNull.Value;
                cmd.Parameters.Add("@RESTRICTRELEASEFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.RestrictReleaseFg);
                cmd.Parameters.Add("@REPRESENTATIVELOTNO", SqlDbType.NVarChar).Value = (object)this.RepresentativeLotNo ?? DBNull.Value;

                cmd.ExecuteNonQuery();
            }

            if (DelFg == false)
            {
                AsmLot.InsertLotLog(this.LotNo, DateTime.Now, "流動規制：" + Reason, this.ProcNo, string.Empty, false, "", this.UpdUserCd);
            }
            else
            {
                AsmLot.InsertLotLog(this.LotNo, DateTime.Now, "流動規制解除：" + Reason, this.ProcNo, string.Empty, false, "", this.UpdUserCd);
            }

        }

        #endregion

        public static void Delete(SqlCommand cmd, string lotno) 
        {
            if (string.IsNullOrEmpty(lotno))
            {
                return;
            }

            string sql = " UPDATE TnRestrict SET delfg = 1 WHERE lotno Like @LOTNO ";
            cmd.CommandText = sql;

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new ArmsException("流動規制削除エラー:" + lotno, ex);
            }
        }

		// 3in1対応 n.yoshimoto 2015/1/16
		/// <summary>
		/// ダイシェアサンプリング理由の移動規制を解除可能かチェックする
		/// →16版仕様からPSTesterLinkInfoのキーに入るprocNo全ての実績をチェックしてある場合に削除の仕様に変更(メール:From湯浅氏)
		/// →procNo全ての実績チェックは無しでダイシェア規制がかかって無ければ、PSTesterログは放置、規制あれば削除の仕様に変更（電話：From湯浅氏）
		/// </summary>
		/// <returns>解除可否</returns>
		public static void CancelDieShaerRestrict()
		{
			List<KeyValuePair<int, string>?>cfglist = Config.Settings.PSTesterLinkInfo;

			if (cfglist == null || cfglist.Count == 0) 
            {
                return;
            }
            
            IEnumerable<KeyValuePair<int, string>?> wbcfglist = cfglist.Where(c => !string.IsNullOrEmpty(c.Value.Value));
            if (wbcfglist == null || wbcfglist.Count() == 0)
            {
                return;
            }
            KeyValuePair<int, string>? wbcfg = wbcfglist.First();
            
            //PSTesterログ出力先ディレクトリ
            string basepath = wbcfg.Value.Value;

			string chkDirPath = Path.Combine(basepath, "toarms");

			string[] testerLogPathList = Directory.GetFiles(chkDirPath);
			
			if (testerLogPathList == null || testerLogPathList.Count() <= 0)
			{
				return;
			}

			foreach (string testerLogPath in testerLogPathList)
			{
				string lotno;
				string testMode;

				string fileNm = Path.GetFileNameWithoutExtension(testerLogPath);

				string[] fileNmItem = fileNm.Split('_');

				lotno = fileNmItem[0];
				testMode = fileNmItem[1];

				AsmLot lot = AsmLot.GetAsmLot(lotno);

                if (lot == null) continue;

				if(testMode == TESTMODE_DIESHEAR)
				{
					Process restrictProc = Process.GetProcess(Restrict.INSPECT_WORKCD);
					Restrict[] targetLotReslist = Restrict.SearchRestrict(lot.NascaLotNo, restrictProc.ProcNo, true);

					foreach (Restrict res in targetLotReslist)
					{
						//ダイシェア代表ロットサンプリングによる規制理由と同じか確認
						if (res.Reason == Restrict.RESTRICT_REASON_DIESHEARSAMPLE)
						{
							//規制を解除
							res.DelFg = true;
							res.Save();							

							Log.ApiLog.Info(string.Format("ダイシェア代表ロット 規制解除 工程：{0} lot:{1}", res.ProcNm, lot.NascaLotNo));

							if (File.Exists(testerLogPath))
							{
								//ダイシェア完了ファイルの削除
								File.Delete(testerLogPath);
							}

							Log.ApiLog.Info(string.Format("ダイシェア代表ロット 試験完了ファイル削除 lot:{0} 削除ファイル:{1}", lot.NascaLotNo, testerLogPath));
						}
					}
				}
			}

		}

        /// <summary>
        /// ダイシェア試験対象で流動規制がかかっているロットを抽出
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDieshearRestrictLot()
        {
            List<string> retv = new List<string>();

            Restrict[] list = SearchRestrict(true, RESTRICT_REASON_DIESHEARSAMPLE);
            if (list.Count() == 0)
                return retv;

            foreach (Restrict restrict in list)
            {
                Magazine[] mag = Magazine.GetMagazine(restrict.LotNo, true);
                if (mag.Count() > 0)
                {
                    retv.Add(restrict.LotNo);
                }
            }

            retv = list.Select(l => l.LotNo).ToList();

            return retv;
        }


        /// <summary>
        /// 規制を解除する
        /// </summary>
        /// <param name="lotno"></param>
        public static void Cancel(string lotno, string reason)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = " UPDATE TnRestrict SET delfg = 1 WHERE lotno Like @LOTNO ";

                    if (string.IsNullOrEmpty(reason) == false)
                    {
                        cmd.Parameters.Add("@REASON", SqlDbType.NVarChar).Value = reason;
                        sql += " AND reason = @REASON";
                    }

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";
                    cmd.CommandText = sql;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("流動規制解除エラー:" + lotno, ex);
            }
        }

        /// <summary>
        /// 強度試験対象に流動規制をかける
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="representativelotno"></param>
        /// <param name="procno"></param>
        /// <param name="IsUndefines"></param>
        public static void RegulationPSTester(string lotno, string representativelotno, int procno, bool IsUndefines)
        {
            Restrict r = new Restrict();

            r.LotNo = lotno;
            r.RepresentativeLotNo = representativelotno;
            r.Reason = RESTRICT_REASON_PSTESTER;
            r.ReasonKb = RESTRICT_REASON_KB_PSTESTER;
            r.ProcNo = procno;
            r.DelFg = IsUndefines;

            r.Save();
        }

        /// <summary>
        /// 強度試験実施待ちの流動規制をかける
        /// </summary>
        /// <param name="lotno">ロット</param>
        /// <param name="representativelotno">代表ロット</param>
        /// <param name="procno">規制する作業NO</param>
        /// <param name="IsUndefines">解除済み規制をする</param>
        public static void SpreadLotPSTester(string lotno, string representativelotno, int procno, bool IsUndefines)
        {
            Restrict r = new Restrict();

            r.LotNo = lotno;
            r.RepresentativeLotNo = representativelotno;
            r.Reason = RESTRICT_REASON_PSTESTER_SPREAD;
            r.ReasonKb = RESTRICT_REASON_KB_PSTESTER_SPREAD;
            r.ProcNo = procno;
            r.DelFg = IsUndefines;

            r.Save();
        }

        public static Restrict GetPSTesterRestrictLot(string lotno, int procno, bool onlyActive)
        {
            Restrict[] list = SearchRestrict(lotno, procno, onlyActive, SQLite.ConStr, null, RESTRICT_REASON_PSTESTER);
            if (list.Count() == 0)
                return null;

            return list.Single();
        }

        public static Restrict GetPSTesterSpreadLot(string lotno, int procno)
        {
            Restrict[] list = SearchRestrict(lotno, procno, false, SQLite.ConStr, null, RESTRICT_REASON_PSTESTER_SPREAD);
            if (list.Count() == 0)
                return null;

            return list.Single();
        }

        public static List<Restrict> GetRestrictFromRepresentativeLotNo(string representativelotno)
        {
            return Restrict.SearchRestrict(null, null, true, SQLite.ConStr, representativelotno, null).ToList();
        }

        public static void CancelRestrictionPSTester(string magazineNo)
        {
            Magazine svrmag = Magazine.GetCurrent(magazineNo);
            if (svrmag != null)
            {
                AsmLot checkLot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                
                Process regulationProc = Process.GetNextProcess(Config.Settings.SetPSTesterProcNo.Value, checkLot);

                Restrict r = GetPSTesterRestrictLot(svrmag.NascaLotNO, regulationProc.ProcNo, true);

                if (r != null)
                {
                    bool isResultPassing = true;
                    foreach (int functionId in Config.Settings.CancelRestrictPSTesterFunctionIdList)
                    {
                        if (ArmsApi.Model.PSTESTER.WorkResult.IsResultPassing(svrmag.NascaLotNO, functionId) == false)
                        {
                            isResultPassing = false;
                            break;
                        }
                    }

                    if (isResultPassing)
                    {
                        r.DelFg = true;
                        r.UpdUserCd = "660";
                        r.Save();

                        Log.ApiLog.Info(string.Format("[流動規制解除] LotNo:{0} 規制内容:{1} 解除理由:{2}",
                                svrmag.NascaLotNO, RESTRICT_REASON_PSTESTER, "強度測定結果で合格判定された為"));


                        //波及ロットも解除
                        List<Restrict> spreadList = Restrict.GetRestrictFromRepresentativeLotNo(svrmag.NascaLotNO);
                        foreach (Restrict spread in spreadList)
                        {
                            if (svrmag.NascaLotNO != spread.LotNo)
                            {
                                spread.DelFg = true;
                                spread.UpdUserCd = "660";
                                spread.Save();

                                Log.ApiLog.Info(string.Format("[流動規制解除] LotNo:{0} 試験代表LotNo:{1} 規制内容:{2} 解除理由:{3}",
                                        spread.LotNo, svrmag.NascaLotNO, RESTRICT_REASON_PSTESTER_SPREAD, "代表ロットの強度測定結果で合格判定された為"));
                            }
                        }
                    }
                }
            }
        }
    }
}
