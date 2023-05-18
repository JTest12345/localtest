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
    public class Magazine
    {
        #region プロパティ

        /// <summary>
        /// マガジン番号
        /// </summary>
        public string MagazineNo { get; set; }

        /// <summary>
        /// NASCAロットNO(分割時は分割ロット番号）
        /// </summary>
        public string NascaLotNO { get; set; }

        private List<string> resinGpCd = new List<string>();
        /// <summary>
        /// 樹脂グループ
        /// </summary>
        public List<string> ResinGr
        {
            get
            {
                if (resinGpCd == null || resinGpCd.Count == 0)
                {
					return resinGpCd;
				}
				else
				{
                    AsmLot lot = AsmLot.GetAsmLot(this.NascaLotNO);
                    if (lot == null) return null;
                    this.resinGpCd = lot.ResinGpCd;

					return resinGpCd;
                }
            }
        }

        private List<string> resinGpCd2 = new List<string>();
        /// <summary>
        /// 樹脂グループ2
        /// </summary>
        public List<string> ResinGr2
        {
            get
            {
                if (resinGpCd2== null || resinGpCd2.Count == 0)
                {
                    return resinGpCd2;
                }
                else
                {
                    AsmLot lot = AsmLot.GetAsmLot(this.NascaLotNO);
                    if (lot == null) return null;
                    this.resinGpCd2 = lot.ResinGpCd2;

                    return resinGpCd2;
                }
            }
        }

        /// <summary>
        /// 現在完了工程
        /// </summary>
        public int NowCompProcess { get; set; }

        /// <summary>
        /// フレーム搭載段数区分
        /// </summary>
        public enum LoadStep : int
        {
            Even = 1,
            Odd = 2,
            All = 3,
        }

        public string NowCompProcessNm
        {
            get
            {
                return Process.GetProcess(NowCompProcess).InlineProNM;
            }
        }

        /// <summary>
        /// 現在稼働中フラグ
        /// </summary>
        public bool NewFg { get; set; }

        public DateTime LastUpdDt { get; set; }

        #endregion

        #region UpdateNewFgOff

        /// <summary>
        /// 現在稼働中フラグを外す
        /// </summary>
        /// <param name="magazineNo"></param>
        public static void UpdateNewFgOff(string magazineNo)
        {
            Magazine mag = Magazine.GetCurrent(magazineNo);

            if (mag == null)
            {
                return;
            }

            mag.NewFg = false;
            mag.Update();
        }
        #endregion

        public static Magazine GetCurrent(string magazineNo)
        {
            if (string.IsNullOrEmpty(magazineNo)) return null;

            Magazine[] list = GetMagazine(magazineNo, null, true, null);


            if (list == null)
            {
                return null;
            }
            else if (list.Count() == 0)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }

        #region GetMagazine

        public static Magazine[] GetMagazine(string magazineNo, string lotno, bool newFg, string resingpcd)
        {
            return GetMagazine(magazineNo, lotno, newFg, resingpcd, SQLite.ConStr, null, null);
        }

        public static Magazine[] GetMagazine(string lotno, bool newFg) 
        {
            return GetMagazine(null, lotno, newFg, null, SQLite.ConStr, null, null);
        }

        public static Magazine[] GetMagazine(bool newFg) 
        {
            return GetMagazine(null, null, newFg, null, SQLite.ConStr, null, null);
        }

        public static Magazine[] GetMagazine(string magazineNo, string lotno, bool newFg, string resingpcd, int? inlineProcNo)
        {
            return GetMagazine(magazineNo, lotno, newFg, resingpcd, SQLite.ConStr, inlineProcNo, null);
        }

        public static Magazine GetMagazine(string lotno)
        {
            Magazine[] list = GetMagazine(null, lotno, true, null, SQLite.ConStr, null, null);
            if (list.Count() == 0)
            {
                return null;
            }
            else
            {
                return list.Single();
            }
        }

        public static Magazine[] GetMagazine(bool newfg, string typeCd)
        {
            return GetMagazine(null, null, newfg, null, SQLite.ConStr, null, typeCd);
        } 
        
        /// <summary>
        /// マガジン検索(汎用 パラメータnull許容)
        /// </summary>
        /// <param name="magazineNo"></param>
        /// <param name="lotno"></param>
        /// <param name="newFg"></param>
        /// <returns></returns>
        public static Magazine[] GetMagazine(string magazineNo, string lotno, bool newFg, string resingpcd, string constr, int? inlineProcNo, string typeCd)
        {
            //ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
            List<Magazine> retv = new List<Magazine>();

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {

                try
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT 
                          t.lotno , 
                          t.magno , 
                          t.inlineprocno , 
                          t.newfg,
                          t.lastupddt
                        FROM
                          tnmag t";

                    if (string.IsNullOrEmpty(typeCd) == false || string.IsNullOrEmpty(resingpcd) == false)
                    {
                        cmd.CommandText += @" INNER JOIN TnLot WITH(nolock)
                                ON TnLot.lotno + '_#1' = t.lotno OR TnLot.lotno + '_#2' = t.lotno OR TnLot.lotno = t.lotno ";
                    }

                    cmd.CommandText += " WHERE 1 = 1";
                   
                    if (newFg == true)
                    {
                        cmd.CommandText += " AND t.newfg = 1";
                    }

                    if (string.IsNullOrEmpty(magazineNo) == false)
                    {
                        cmd.CommandText += " AND t.magno = @MAGNO";
                        cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = magazineNo;
                    }

                    if (string.IsNullOrEmpty(lotno) == false)
                    {
                        cmd.CommandText += " AND t.lotno LIKE @LOTNO";
                        cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";
                    }

                    if (inlineProcNo.HasValue)
                    {
                        cmd.CommandText += " AND t.inlineprocno = @PROCNO";
                        cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = inlineProcNo;
                    }

                    if (string.IsNullOrEmpty(resingpcd) == false)
                    {
                        cmd.CommandText += @" AND TnLot.resingpcd LIKE @RESINGPCD ";
                        cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = "%" + resingpcd + "%";
                    }

                    if (string.IsNullOrEmpty(typeCd) == false)
                    {
                        cmd.CommandText += @" AND TnLot.typecd = @TYPECD ";
                        cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typeCd;
                    }

                    cmd.CommandText = cmd.CommandText.Replace("\r\n", "");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Magazine mag = new Magazine();
                            mag.MagazineNo = SQLite.ParseString(reader["magno"]);
                            mag.NascaLotNO = SQLite.ParseString(reader["lotno"]);
                            mag.NowCompProcess = SQLite.ParseInt(reader["inlineprocno"]);
                            mag.NewFg = SQLite.ParseBool(reader["newfg"]);
                            mag.LastUpdDt = Convert.ToDateTime(reader["lastupddt"]);

                            retv.Add(mag);
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

        #region (作業終了登録)インラインマガジンロット更新

        public static Magazine ApplyMagazineInOut(Order workinfo)
        {
            return ApplyMagazineInOut(workinfo, null);
        }



        /// <summary>
        /// インラインマガジンロット更新
        /// </summary>
        /// <param name="workinfo"></param>
        /// <param name="duplicateMag">Trueの場合、IN側マガジンの稼働フラグを残したままにする</param>
        /// <param name="newMagLotNo">nullの場合はIn側マガジンのロット番号を継承</param>
        /// <returns></returns>
        public static Magazine ApplyMagazineInOut(Order workinfo, string newMagLotNo)
        {
            // インラインマガジンロット情報取得
            Magazine mag = GetCurrent(workinfo.InMagazineNo);
            if (mag == null)
            {
                mag = ArmsApi.Model.Magazine.GetCurrent(workinfo.OutMagazineNo);
                if (mag == null)
                {
                    // ---- MAP高生産性ラインMDの自動搬送プレート使用時の対応
                    // ロットに紐づいたマガジンNoが「Mxxxxx_#1」「Mxxxxx_#2」なので、InMagazineNoとOutMagazineNoでは、ヒットしない
                    Magazine[] mags = GetMagazine(workinfo.LotNo, true);
                    if (mags != null && mags.Length != 0)
                    {
                        mag = mags[0];
                    }
                }
                if (mag == null)
                {
                    throw new ArmsException(string.Format("ロット情報が存在しません。ロットNo:{0}", workinfo.InMagazineNo));
                }
            }

            if (workinfo.InMagazineNo == workinfo.OutMagazineNo && workinfo.OutMagazineNo == mag.MagazineNo && mag.NascaLotNO == newMagLotNo)
            {
                mag.NowCompProcess = Convert.ToInt32(workinfo.ProcNo);
                mag.Update();
                return mag;
            }
            else
            {
                // 搬入マガジンNOと搬出マガジンNOが異なる場合
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, workinfo.ProcNo);

                // ----- 搬入マガジンのデータを更新 -----
                // 流動フラグ
                mag.NewFg = false;
                mag.NowCompProcess = Convert.ToInt32(workinfo.ProcNo);
                mag.Update();

                // ----- 搬出マガジンのデータを編集 -----
                // 異なったロットで、同じマガジンを使用しているデータが存在するか
                Magazine newmag = GetCurrent(workinfo.OutMagazineNo);
                if (newmag != null && dst != Process.MagazineDevideStatus.DoubleToSingle)
                {
                    throw new ArmsException("NW0650：搬出マガジンに、現在稼動中のロットがあります");
                }

                //新しいロット番号と関連付け
                if (!string.IsNullOrEmpty(newMagLotNo))
                {
                    mag.NascaLotNO = newMagLotNo;
                }

                // マガジンNO
                mag.MagazineNo = workinfo.OutMagazineNo;
                mag.NowCompProcess = workinfo.ProcNo;
                mag.NewFg = true;

                // マガジン交換作業の処理の時には、分割ロットの稼働中フラグが全て消えるまで
                // 統合後のマガジンを稼働中にしない。
                // (統合後のマガジンNoが先に稼働中になると残りの未処理の分割マガジンの代わりに紐づいてしまう為)
                if (dst == Process.MagazineDevideStatus.DoubleToSingle && Config.GetLineType == Config.LineTypes.MEL_MAP)
                {
                    string SingleLotNo = Order.MagLotToNascaLot(mag.NascaLotNO);
                    Magazine[] mags = GetMagazine(SingleLotNo, true);
                    if (mags.Length == 0)
                    {
                        mag.Update();
                    }
                }
                else
                {
                    mag.Update();
                }
                return mag;
            }
        }
        #endregion

        /// <summary>
        /// ロット番号からTnMagのレコードが1件に特定できるか確認する
        /// </summary>
        /// <param name="lotno"></param>
        public static void CheckIdentifyData(string lotno)
        {
            Magazine[] magList = Magazine.GetMagazine(lotno, true);

            if (magList.Length == 0)
            {
                throw new ApplicationException(string.Format("稼働中ではないロットです。LotNo:{0}", lotno));
            }
            else if (magList.Length > 1)
            {
                throw new ApplicationException(string.Format("稼働中で同一ロットのマガジン情報が複数件存在します。LotNo:{0}", lotno));
            }
        }
        #region Update

        public void Update()
        {
            Update(SQLite.ConStr);
        }

        public void Update(string constr)
        {
            //ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
            if (this.MagazineNo == AsmLot.MAGAZINE_ERROR)
            {
                throw new ArmsException("ERROR MAGAZINE");
            }

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {

                cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = this.MagazineNo;
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.NascaLotNO ?? "";
                cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = this.NowCompProcess;
                cmd.Parameters.Add("@NEWFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.NewFg);
                cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                try
                {
                    con.Open();

                    //新規Insert
                    cmd.CommandText = @"SELECT magno FROM TnMag WHERE lotno=@LOTNO AND magno=@MAGNO";

                    object mag = cmd.ExecuteScalar();

                    if (mag == null)
                    {
                        #region Insertコマンド
                        cmd.CommandText = @"
                            INSERT INTO tnmag
                              ( 
                                lotno , 
                                magno , 
                                inlineprocno , 
                                newfg , 
                                lastupddt 
                              ) 
                            VALUES 
                              ( 
                                @LOTNO , 
                                @MAGNO , 
                                @PROCNO , 
                                @NEWFG , 
                                @LASTUPDDT 
                              )";
                        #endregion
                    }
                    else
                    {
                        #region Updateコマンド

                        cmd.CommandText = @"
                            UPDATE tnmag
                            SET 
                              inlineprocno = @PROCNO, 
                              newfg = @NEWFG, 
                              lastupddt = @LASTUPDDT 
                            WHERE 
                              lotno = @LOTNO AND magno = @MAGNO";
                        #endregion
                    }

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("マガジン情報更新エラー:" + ex.ToString());
                }
            }
        }
        #endregion

        public static void Delete(SqlCommand cmd, string lotno)
        {
            if (string.IsNullOrEmpty(lotno)) 
            {
                return;
            }

            //削除ログ出力用
            Magazine[] magazines = Magazine.GetMagazine(lotno, false);
            
            string sql = " DELETE FROM TnMag WHERE lotno Like @LOTNO ";
            cmd.CommandText = sql;

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";

            try
            {
                cmd.ExecuteNonQuery();

                foreach (Magazine magazine in magazines)
                {
                    Log.DelLog.Info(string.Format("[TnMag] {0}\t{1}\t{2}\t{3}\t{4}",
                        magazine.NascaLotNO, magazine.MagazineNo, magazine.NowCompProcess, magazine.NewFg, System.DateTime.Now));
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("マガジン削除エラー:" + lotno, ex);
            }
        }

        /// <summary>
        /// 強度試験用ファイル出力
        /// SvrConfigへの設定なしの場合は何もせずに終了
        /// </summary>
        public void WriteTesterFile(List<KeyValuePair<int, string>?> cfglist)
        {
            //出力形態："ラインCD"+"_"+"型番"+"："+"ロットNo"+"_"【"作業CD_設備番号"_"号機"】
            //1013_NESW455B31_N1236CS1302_【WB0002_S00455_455号機】
            string lotFormat = "{0}_{1}_{2}";
            string macFormat = "_【{0}_{1}_{2}】";

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
            
            //出力先ディレクトリ
            string basepath = wbcfg.Value.Value;
           
            AsmLot lot = AsmLot.GetAsmLot(this.NascaLotNO);
            if (lot == null) throw new ArmsException("AsmLotが見つかりません mag:" + this.MagazineNo);

            Order wborder = Order.GetMagazineOrder(this.NascaLotNO, wbcfg.Value.Key);
            if (wborder == null) throw new ArmsException("WB工程の作業実績が存在しません mag:" + this.MagazineNo);

            MachineInfo wbmac = MachineInfo.GetMachine(wborder.MacNo);
            if (wbmac == null) throw new ArmsException("WB装置マスタが存在しません mac:" + wborder.MacNo.ToString());

            int lineno = MachineInfo.GetEICSLineNo(wbmac.NascaPlantCd);

            string filenm = string.Format(lotFormat, lineno, lot.TypeCd, lot.NascaLotNo);
            foreach (KeyValuePair<int, string> cfg in cfglist) 
            {
                Process proc = Process.GetProcess(cfg.Key);
                if (proc == null) throw new ArmsException("工程マスタが存在しません procno:" + cfg.Key);

                Order order = Order.GetMagazineOrder(this.NascaLotNO, cfg.Key);
                if (order == null) throw new ArmsException(string.Format("作業実績が存在しません mag:{0} proc:{1}", this.MagazineNo, proc.InlineProNM));

                MachineInfo mac = MachineInfo.GetMachine(order.MacNo);
                if (mac == null) throw new ArmsException("装置マスタが存在しません mac:" + order.MacNo.ToString());

                filenm += string.Format(macFormat, proc.WorkCd, mac.NascaPlantCd, mac.MachineName) ;
            }

            string fullpath = Path.Combine(basepath, filenm);

            if (File.Exists(fullpath))
            {
                //既にまったく同名のファイルがあれば何もしない
                return;
            }

            using (StreamWriter sw = new StreamWriter(fullpath))
            {
                sw.WriteLine(DateTime.Now.ToString());
            }
        }
        public void WritePSTesterFile() 
        {
            WriteTesterFile(Config.Settings.PSTesterLinkInfo);
        }
        public void WriteBDTesterFile() 
        {
            WriteTesterFile(Config.Settings.BDTesterLinkInfo);
        }
        public void WriteFilmTesterFile()
        {
            WriteTesterFile(Config.Settings.FilmTesterLinkInfo);
        }

        public static int GetFrameCt(string magazineNO)
        {
            int retv = 0;

            string nascaLotNO = Magazine.GetCurrent(magazineNO).NascaLotNO;

            string typeCD = AsmLot.GetAsmLot(nascaLotNO).TypeCd;

            using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

//				string sql = @" SELECT TmFRAME.MagazineStepMAX_CT, TmFrame.LoadStep_CD
//                                FROM TmFILEFMTTYPE WITH(nolock)INNER JOIN
//                                TmFRAME WITH(nolock) ON TmFILEFMTTYPE.Frame_NO = TmFRAME.Frame_NO
//                                WHERE (TmFILEFMTTYPE.Del_FG = 0) AND (TmFRAME.Del_FG = 0) 
//                                AND (TmFILEFMTTYPE.Type_CD = @TypeCD) ";

				//string sql = @" SELECT TmMag.Step, TmMag.LoadStepCD
				//				FROM TmMag WITH (nolock) 
				//				INNER JOIN TmType WITH (nolock) ON TmMag.MagazineID = TmType.MagazineID
				//				WHERE (TmMag.DelFG = 0) AND (TmType.DelFG = 0) AND (TmType.TypeCD = @TypeCD) ";

                string sql = @" SELECT TmType.MagazineID, TmMag.MagazineID AS 'MagRecord', TmMag.Step, TmMag.LoadStepCD
								FROM TmType WITH(NOLOCK)
                                LEFT OUTER JOIN TmMag WITH(NOLOCK) ON TmType.MagazineID = TmMag.MagazineID
                                                                    AND TmMag.DelFG = 0
                                WHERE (TmType.TypeCD = @TypeCD) AND (TmType.DelFG = 0) ";

                cmd.Parameters.Add("@TypeCD", SqlDbType.Char).Value = typeCD;

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.HasRows)
                    {
                        throw new Exception($"[型番={typeCD}]のフレーム紐づけ情報(LENS.TmType)にマスタが存在しません。");
                    }

                    if (rd.Read())
                    {
                        int frameid = rd.GetInt32(rd.GetOrdinal("MagazineID"));
                        if (rd["MagRecord"] == DBNull.Value)
                        {
                            throw new Exception($"[型番={typeCD}] の[紐づけフレームID={frameid.ToString()}]のマスタがフレーム情報マスタ(LENS.TmMag)に存在しません。");
                        }

						//retv = rd.GetInt32(rd.GetOrdinal("MagazineStepMAX_CT"));
						//int loadStepCD = rd.GetInt32(rd.GetOrdinal("LoadStep_CD"));

						retv = rd.GetInt32(rd.GetOrdinal("Step"));
						int loadStepCD = rd.GetInt32(rd.GetOrdinal("LoadStepCD"));

                        switch(loadStepCD)
                        {
                            case (int)LoadStep.Even:
                            case (int)LoadStep.Odd:
                                retv = retv / 2;
                                break;

                            case (int)LoadStep.All:
                                break;

                            default:
                                throw new ApplicationException($"[型番={typeCD}] の[紐づけフレームID={frameid.ToString()}]のマスタの[LoadStepCD={loadStepCD}]がARMSシステム内で未定義の設定値です。");
                        }
                    }
                }
            }

            return retv;
        }



        private const int LOAD_STEP_CD_FULL = 3;
        private const int LOAD_STEP_CD_EVEN = 1;
        private const int LOAD_STEP_CD_ODD = 2;

        /// <summary>
        /// LENSのTmMagの情報を元に反転したマガジン内段数（RowNo）を返す
        /// </summary>
        /// <param name="rowNo">反転前の現在段数</param>
        /// <param name="step">マガジンの総段数（LENS2.TmMag.Step)</param>
        /// <param name="loadStepCd">マガジンの格納方法CD（LENS2.TmMag.LoadStepCd)</param>
        /// <returns></returns>
        public static int ReverseRow(int rowNo, int step, int loadStepCd)
        {
            switch (loadStepCd)
            {
                case LOAD_STEP_CD_FULL:
                    return step - rowNo + 1;

                case LOAD_STEP_CD_EVEN:
                    return step - rowNo + 2;

                case LOAD_STEP_CD_ODD:
                    return step - rowNo;

                default:
                    throw new ApplicationException("定義外のloadStepCdです:" + loadStepCd.ToString());
            }

        }

        /// <summary>
        /// TmMagの情報を搬送可能な次ラインを返す
        /// </summary>
        public static string GetNextLine(Magazine mag, out string msg)
        {
            msg = "";

            if (mag == null) return null;

            AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
            if (lot == null) return null;

            Process nowp = Process.GetProcess(mag.NowCompProcess);
            if (nowp == null) return null;

            Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
            if (p == null) return null;
            
            // TmGeneralからmacgroup, ライン名, キャリアNoリストを取得
            GnlMaster[] gnlMacGroupList = GnlMaster.GetMacGroup();

            Dictionary<GnlMaster, int> stationAvailableMachine = new Dictionary<GnlMaster, int>();
            Dictionary<GnlMaster, int> stationAllMachine = new Dictionary<GnlMaster, int>();
            // TmProMacで次工程のみに絞って、全装置を取得
            MachineInfo[] availableAllMachines = MachineInfo.GetAvailableMachines(p.ProcNo, new List<string>());

            // データベースから供給要求リストを取得
            List<LAMS.MachineRequire> reqAllMachines = LAMS.MachineRequire.GetMachineRequire(availableAllMachines.Select(a => a.NascaPlantCd).ToList()).ToList();

            foreach (GnlMaster gm in gnlMacGroupList)
            {
                List<string> carrierNoList = new List<string>();
                carrierNoList.AddRange(gm.Val2.Split(','));

                MachineInfo[] availableMachines = availableAllMachines.Where(a => a.MacGroup.Contains(gm.Code)).ToArray();
                List<MachineInfo> allMachines = new List<MachineInfo>();


                int vmagCount = 0;
                int reqCount = 0;
                foreach (MachineInfo availableMachine in availableMachines)
                {
                    //TmRoute見る (装置とキャリアが紐づいている → その装置がライン内にいるかどうか)
                    Location loc = new Location(availableMachine.MacNo, Station.Loader);
                    bool isReachable = false;
                    foreach (string carrierno in carrierNoList)
                    {
                        int iCarrierno;
                        if (int.TryParse(carrierno, out iCarrierno) == false) continue;
                        CarrierInfo carrier = new CarrierInfo(iCarrierno);
                        if (Route.IsReachable(loc, carrier) == true)
                        {
                            isReachable = true;
                            break;
                        }
                    }
                    if (isReachable == false)
                        continue;

                    allMachines.Add(availableMachine);
                    
                    // 供給要求を確認 → LAMSのテーブルから取得する処理を追記する ********************************/
                    // 供給要求 と 供給禁止モードの両方を見る
                    bool reqOk = true;
                    LAMS.MachineRequire machineRequire = reqAllMachines.Where(r => r.PlantCd == availableMachine.NascaPlantCd).FirstOrDefault();
                    if (machineRequire == null || 
                        machineRequire.IsRequireInput == false || 
                        machineRequire.IsInputForbidden == true)
                    {
                        // レコードなし or 供給要求OFF or 供給禁止モード ON の場合、供給要求OFFとする
                        reqOk = false;
                    }

                    if (reqOk)
                    {
                        // EICSの装置設定タイプと一致するかを確認する
                        string errMsg;
                        if (ArmsApi.Model.WorkChecker.isMismatchBetweenLotAndMacProductType(lot, availableMachine, out errMsg) == true)
                            continue;
                        
                        reqCount += availableMachine.LoaderMagazineMaxCt;

                        //仮想マガジン数をチェック
                        VirtualMag[] magsLoader = VirtualMag.GetVirtualMag(availableMachine.MacNo, (int)Station.Loader);
                        VirtualMag[] magsLoader1 = VirtualMag.GetVirtualMag(availableMachine.MacNo, (int)Station.Loader1);
                        VirtualMag[] magsLoader2 = VirtualMag.GetVirtualMag(availableMachine.MacNo, (int)Station.Loader2);
                        VirtualMag[] magsLoader3 = VirtualMag.GetVirtualMag(availableMachine.MacNo, (int)Station.Loader3);
                        VirtualMag[] magsLoader4 = VirtualMag.GetVirtualMag(availableMachine.MacNo, (int)Station.Loader4);

                        int tmpVmagCount = magsLoader.Length + magsLoader1.Length + magsLoader2.Length + magsLoader3.Length + magsLoader4.Length;
                        vmagCount += tmpVmagCount;
                    }
                }

                if (reqCount > 0)
                {
                    // AGVのgetNextStation関数では、更に受渡しユニットのマガジン数も減算しているが、
                    // この関数を使う場合は、投入CV経由と考えられるため、受け渡しユニットは考慮しない
                    int remainingCount = reqCount - vmagCount;

                    stationAvailableMachine.Add(gm, remainingCount);
                }

                if (allMachines.Count != 0)
                    stationAllMachine.Add(gm, allMachines.Count);
            }

            if (stationAllMachine.Count != 0)
            {
                // ライン内のLoader側のマガジン置場の空き数
                string lineName = string.Empty;
                int canloadmagazinecount = 0;

                if (stationAvailableMachine.Count > 0)
                {
                    KeyValuePair<GnlMaster, int> kv = stationAvailableMachine.OrderByDescending(r => r.Value).First();
                    lineName = kv.Key.Val;
                    canloadmagazinecount = kv.Value;
                }
                
                // バッファの空き装置なし
                if (canloadmagazinecount == 0)
                {
                    msg = "投入装置空き無し";
                }

                return lineName;
            }
            else
            {
                // AGVのgetNextStation関数では、ここで集荷装置の要求を見に行っているが、
                // この関数を使う場合は、ライン外と考えられるため、受け渡しユニットは考慮しない

                // 搬送可能ラインなし or ライン外装置行き
                msg = "該当ラインなし または ライン外";
                return null;
            }
        }

        public void RecordAGVPSTester()
        {
            if (Config.Settings.SetPSTesterProcNo.HasValue == false)
            {
                return;
            }

            //強度試験情報テーブル（TnPSTester）へのレコードを追加

            Order order = Order.GetMagazineOrder(this.NascaLotNO, Config.Settings.SetPSTesterProcNo.Value);
            if (order == null)
            {
                throw new ApplicationException($"強度試験対象工程の実績がないため強度試験情報テーブル（TnPSTester）へのレコードを追加ができませんでした。ロット番号：{this.NascaLotNO} 工程NO：{Config.Settings.SetPSTesterProcNo.Value}");
            }

            MachineInfo mac = MachineInfo.GetMachine(order.MacNo);

            if (AGVPSTester.ExistTestLot(this.NascaLotNO, mac.NascaPlantCd) == false)
            {
                AGVPSTester.Insert(mac.NascaPlantCd, DateTime.Now, this.NascaLotNO);
                Log.SysLog.Info($"[強度試験対象ロット手動] ロット：{this.NascaLotNO}");
            }
        }
    }
}
