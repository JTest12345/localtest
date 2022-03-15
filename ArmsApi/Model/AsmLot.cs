using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class AsmLot
    {
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            AsmLot lot = (AsmLot)obj;
            if (this.NascaLotNo == lot.NascaLotNo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 自動化用　マガジンバーコード
        /// </summary>
        // 富士情報　開始
        // public const string PREFIX_INLINE_MAGAZINE = "30 ";
        public const string PREFIX_INLINE_MAGAZINE = "C30 ";
        // 富士情報　終了始
        // JuncihiWatanabe 照明合理化工程マガジン用
        // public const string PREFIX_INLINE_MAGAZINE = "A ";

        /// <summary>
        /// 高効率用　ロット番号バーコード
        /// </summary>
        public const string PREFIX_INLINE_LOT = "11 ";

        /// <summary>
        /// 高効率用　マガジン分割対応ロット番号バーコード
        /// </summary>
        public const string PREFIX_DEVIDED_INLINE_LOT = "13 ";

        /// <summary>
        /// エラーマガジン番号
        /// </summary>
        public const string MAGAZINE_ERROR = "ERROR";

        #region プロパティ

        /// <summary>
        /// NASCAロットNO
        /// マガジン連番ロットを入れた場合は自動変換
        /// </summary>
        public string NascaLotNo { get { return _nascaLotNo; } set { _nascaLotNo = Order.MagLotToNascaLot(value); } }
        private string _nascaLotNo;

        /// <summary>
        /// 指図タイプ名
        /// </summary>
        public string TypeCd { get; set; }

        /// <summary>
        /// プロファイルID
        /// </summary>
        public int ProfileId { get; set; }

        /// <summary>
        /// 樹脂グループコード
        /// </summary>
        public List<string> ResinGpCd { get; set; }

        /// <summary>
        /// 樹脂グループコード2
        /// </summary>
        public List<string> ResinGpCd2 { get; set; }

        /// <summary>
        /// ブレンドコード
        /// </summary>
        public string BlendCd { get; set; }

        /// <summary>
        /// カットブレンド判定コード
        /// </summary>
        public string CutBlendCd { get; set; }

        /// <summary>
        /// ロットステータス警告
        /// </summary>
        public bool IsWarning { get; set; }

        /// <summary>
        /// 出荷規制(TnRestrictに切り出したため使用禁止)
        /// </summary>
        public bool IsRestricted { get; set; }

        /// <summary>
        /// ライフ試験対象フラグ
        /// </summary>
        public bool IsLifeTest { get; set; }

        /// <summary>
        /// 先行色調
        /// </summary>
        public bool IsColorTest { get; set; }

        /// <summary>
        /// 吸湿保管点灯試験対象フラグ
        /// </summary>
        public bool IsKHLTest { get; set; }

        /// <summary>
        /// リフローパス試験数フラグ
        /// </summary>
        public bool IsReflowTest { get; set; }

        /// <summary>
        /// リフローパス試験(WB)数フラグ
        /// </summary>
        public bool IsReflowTestWirebond { get; set; }

        /// <summary>
        /// 弾性率試験フラグ
        /// </summary>
        public bool IsElasticityTest { get; set; }

        /// <summary>
        /// 仮採番ロット番号
        /// </summary>
        public string TempLotNo { get; set; }

        /// <summary>
        /// 仮採番状態
        /// </summary>
        public bool IsTemp { get; set; }

        /// <summary>
        /// NASCAロット特性情報連携完了フラグ
        /// </summary>
        public bool IsNascaLotCharEnd { get; set; }

        /// <summary>
        /// 指図数量
        /// </summary>
        public int LotSize { get; set; }

        /// <summary>
        /// マッピング検査実施フラグ
        /// </summary>
        public bool IsMappingInspection { get; set; }

        /// <summary>
        /// 全数検査を行うかのフラグ
        /// </summary>
        public bool IsFullSizeInspection { get; set; }

        /// <summary>
        /// 抜取り検査対象ロットフラグ
        /// </summary>
        public bool IsChangePointLot { get; set; }

        /// <summary>
        /// 装置グループ
        /// </summary>
        public List<string> MacGroup { get; set; }

        /// <summary>
        /// BadMarkフレームを使用している
        /// </summary>
        public bool IsBadMarkFrame { get; set; }

        /// <summary>
        /// カットブレンド判定コード
        /// </summary>
        public string DBThrowDT { get; set; }

        /// <summary>
        /// ダイシェア抜き取り対象ロットフラグ 
        /// </summary>
        public bool IsDieShearLot { get; set; }

        /// <summary>
        /// 2015.2.5 車載3次 部門間移動数
        /// </summary>
        public int MoveStockCt { get; set; }

        /// <summary>
        /// 先行ライフ試験条件
        /// </summary>
        public string BeforeLifeTestCondCd { get; set; }

        /// <summary>
        /// ダイシェア試験抜取グループ
        /// </summary>
        public string DieShareSamplingPriority { get; set; }

        /// <summary>
        /// ループ条件変更フラグ
        /// </summary>
        public bool IsLoopCondChange { get; set; }

        /// <summary>
        /// 先行ライフ試験対象
        /// </summary>
        public bool IsBeforeLifeTest
        {
            get
            {
                if (string.IsNullOrEmpty(this.BeforeLifeTestCondCd) || this.BeforeLifeTestCondCd == "0")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        /// <summary>
        /// 仮カットブレンドID
        /// </summary>
        public int TempCutBlendId { get; set; }


        /// <summary>
        /// 仮カットブレンドNo
        /// </summary>
        public string TempCutBlendNo { get; set; }

        /// <summary>
        /// 予定選別規格
        /// </summary>
        public string ScheduleSelectionStandard { get; set; }

        /// <summary>
        /// スペックボックスID
        /// </summary>
        public int? MnggrId { get; set; }

        /// <summary>
        /// スペックボックス名
        /// </summary>
        public string MnggrNm { get; set; }


        #endregion

        #region HasErrorStatus

        /// <summary>
        /// マガジンステータスの内容を確認
        /// </summary>
        /// <param name="paramInfo">作業開始コマンド</param>
        /// <param name="msg">マガジン内容に該当するメッセージ</param>
        /// <param name="dataupdflg">データ更新フラグ</param>
        /// <returns>True=Inline Error,False=Inline Normal</returns>
        public static bool HasErrorStatus(Order paramInfo)
        {
            try
            {
                // マガジンSTが設定されていない場合は制御なし
                if (string.IsNullOrEmpty(paramInfo.MagazineSt))
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            catch
            {
                throw;
            }
        }

        #endregion

        public static AsmLot CreateNewAsmLot(DateTime dt, int? macno, string lineno)
        {
            return CreateNewAsmLot(dt, macno, lineno, false);
        }
        public static AsmLot CreateNewAsmLot(DateTime dt, int? macno, string lineno, bool highline)
        {
            AsmLot retv = CreateNewAsmLotWithoutLotNumbering(dt, "", false, macno);
            // MAPサーバー統合対応 (アウト・高生産性用定義のロットNoを取得)
            if (highline == true)
            {
                // Armsデータメンテナンスの指図手動発行画面で登録する場合は、ラインNoが本来の番号の末字2文字になっている為、再取得する。
                if (macno.HasValue)
                {
                    MachineInfo svrmac = MachineInfo.GetMachine(macno.Value);
                    if (svrmac != null) lineno = svrmac.LineNo;
                }                
                retv.TempLotNo = Numbering.GetNewAsmLotNoHigh(dt, lineno);
            }
            else
            {
                retv.TempLotNo = Numbering.GetNewAsmLotNo(dt, lineno);
            }
            retv.NascaLotNo = retv.TempLotNo;
            retv.Update();
            Log.SysLog.Info("新規ロット発行");

            try
            {
                Profile prof = null;

                //TODO
                //if (Config.Settings.UseDBProfile == false)
                //{
                //    prof = Profile.GetCurrentProfile();
                //}
                //else
                //{
                if (macno.HasValue)
                {
                    prof = Profile.GetCurrentDBProfile(macno.Value);
                }
                //}

                if (prof == null) throw new ArmsException("投入可能なプロファイルが存在しません");

                //WB後自動外観検査機投入が全数かどうかの設定
                retv.IsFullSizeInspection = prof.FullInspectionFg;
                retv.Update();

                //プロファイル時点の抜き取り検査設定
                foreach (int proc in prof.InspectionProcs)
                {
                    Inspection isp = new Inspection();
                    isp.LotNo = retv.NascaLotNo;
                    isp.ProcNo = proc;
                    isp.DeleteInsert();
                    if (retv.IsChangePointLot == false)
                    {
                        //抜取り検査フラグON
                        retv.IsChangePointLot = true;
                        retv.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("ロット発行時抜き取り検査設定異常" + ex.ToString());
            }

            return retv;
        }

        public static AsmLot CreateNewAsmLotWithoutLotNumbering(DateTime dt, string tempLot, bool recordDb, int? macno)
        {
            AsmLot retv = new AsmLot();

            Profile prof = null;

            if (macno.HasValue)
            {
                prof = Profile.GetCurrentDBProfile(macno.Value);
            }

            if (prof == null) throw new ArmsException("投入可能なプロファイルが存在しません");

            retv.TempLotNo = tempLot;
            retv.NascaLotNo = tempLot;

            retv.TypeCd = prof.TypeCd;
            retv.ProfileId = prof.ProfileId;
            retv.LotSize = prof.LotSize;

            retv.ResinGpCd = new List<string>();
            if (prof.ResinGpCd != null)
            {
                if (prof.ResinGpCd.Count != 0)
                {
                    retv.ResinGpCd.AddRange(prof.ResinGpCd);
                }
            }

            retv.ResinGpCd2 = new List<string>();
            if (prof.ResinGpCd2 != null)
            {
                if (prof.ResinGpCd2.Count != 0)
                {
                    retv.ResinGpCd2.AddRange(prof.ResinGpCd2);
                }
            }

            retv.BlendCd = prof.BlendCd;
            retv.CutBlendCd = prof.CutBlendCd;
            retv.DBThrowDT = prof.DBThrowDt;
            retv.BeforeLifeTestCondCd = prof.BeforeLifeTestCondCd;
            retv.DieShareSamplingPriority = prof.DieShearSamplingPriority;
            // Ver1.99.0 予定選別規格 追加
            retv.ScheduleSelectionStandard = prof.ScheduleSelectionStandard;

            if (recordDb)
            {
                retv.Update();
            }

            return retv;
        }

        #region Update ロット情報更新

        public void Update()
        {
            Update(SQLite.ConStr);
        }

        public void Update(string constr)
        {
            //ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要

            Log.SysLog.Info("ロット情報更新" + this.NascaLotNo);
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.NascaLotNo;
                cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = this.TypeCd ?? "";
                cmd.Parameters.Add("@WARNINGFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsWarning);
                cmd.Parameters.Add("@RESTRICTFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsRestricted);
                cmd.Parameters.Add("@PROFILEID", SqlDbType.BigInt).Value = this.ProfileId;
                if (this.ResinGpCd == null || this.ResinGpCd.Count == 0)
                {
                    cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = System.DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = string.Join(",", this.ResinGpCd);
                }

                if (this.ResinGpCd2 == null || this.ResinGpCd2.Count == 0)
                {
                    cmd.Parameters.Add("@RESINGPCD2", SqlDbType.NVarChar).Value = System.DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("@RESINGPCD2", SqlDbType.NVarChar).Value = string.Join(",", this.ResinGpCd2);
                }

                cmd.Parameters.Add("@BLENDCD", SqlDbType.NVarChar).Value = this.BlendCd ?? "";
                cmd.Parameters.Add("@CUTBLENDCD", SqlDbType.NVarChar).Value = this.CutBlendCd ?? "";
                cmd.Parameters.Add("@COLORTESTFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsColorTest);
                cmd.Parameters.Add("@TEMPLOTNO", SqlDbType.NVarChar).Value = this.TempLotNo ?? "";
                cmd.Parameters.Add("@ISTEMP", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsTemp);
                cmd.Parameters.Add("@ISNASCALOTCHAREND", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsNascaLotCharEnd);
                cmd.Parameters.Add("@ISBADMARKFRAME", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsBadMarkFrame);
                cmd.Parameters.Add("@DBTHROWDT", SqlDbType.NVarChar).Value = this.DBThrowDT ?? "";
                cmd.Parameters.Add("@DIESHEARTESTFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsDieShearLot);
                cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                if (this.MacGroup == null || this.MacGroup.Count == 0)
                {
                    cmd.Parameters.Add("@MACGROUP", SqlDbType.NVarChar).Value = System.DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("@MACGROUP", SqlDbType.NVarChar).Value = string.Join(",", this.MacGroup);
                }

                cmd.Parameters.Add("@MOVESTOCKCT", SqlDbType.BigInt).Value = SQLite.ParseInt(this.MoveStockCt);

                // 2015.6.16 車載高 先行ライフ試験対応
                cmd.Parameters.Add("@BEFORELIFETESTCONDCD", SqlDbType.NVarChar).Value = this.BeforeLifeTestCondCd ?? "";
                cmd.Parameters.Add("@TEMPCUTBLENDNO", SqlDbType.NVarChar).Value = this.TempCutBlendNo ?? "";
                // -----------------------------------

                cmd.Parameters.Add("@MNGGRID", SqlDbType.Int).Value = SQLite.GetParameterValue(this.MnggrId);
                cmd.Parameters.Add("@MNGGRNM", SqlDbType.NVarChar).Value = SQLite.GetParameterValue(this.MnggrNm);

                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    //新規Insert
                    cmd.CommandText = @"SELECT lotno FROM TnLot WHERE lotno=@LOTNO";
                    object lot = cmd.ExecuteScalar();

                    if (lot == null)
                    {
                        #region Insertコマンド

                        cmd.CommandText = @"
                            INSERT INTO tnlot 
                              ( 
                                lotno , 
                                typecd , 
                                blendcd , 
                                warningfg , 
                                restrictfg ,
                                profileid, 
                                resingpcd,
                                cutblendcd,
                                colortestfg,
                                templotno,
                                istemp,
                                isnascalotcharend,
                                lastupddt,
                                macgroup,
                                isbadmarkframe,
                                dbthrowdt,
								diesheartestfg,
							    movestockct,
                                beforelifetestcondcd,
                                tempcutblendno,
                                mnggrid,
                                mnggrnm,
                                resingpcd2
                              ) 
                            VALUES 
                              ( 
                                @LOTNO , 
                                @TYPECD , 
                                @BLENDCD , 
                                @WARNINGFG , 
                                @RESTRICTFG ,
                                @PROFILEID ,
                                @RESINGPCD ,
                                @CUTBLENDCD,
                                @COLORTESTFG,
                                @TEMPLOTNO,
                                @ISTEMP,
                                @ISNASCALOTCHAREND,
                                @LASTUPDDT,
                                @MACGROUP,
                                @ISBADMARKFRAME,
                                @DBTHROWDT,
								@DIESHEARTESTFG,
								@MOVESTOCKCT,
                                @BEFORELIFETESTCONDCD,
								@TEMPCUTBLENDNO,
                                @MNGGRID,
                                @MNGGRNM,
                                @RESINGPCD2
                              )";


                        #endregion
                    }
                    else
                    {
                        #region Updateコマンド

                        cmd.CommandText = @"
                            UPDATE tnlot 
                            SET 
                              typecd = @TYPECD , 
                              blendcd = @BLENDCD , 
                              warningfg = @WARNINGFG , 
                              restrictfg = @RESTRICTFG , 
                              profileid = @PROFILEID,
                              resingpcd = @RESINGPCD,
                              cutblendcd = @CUTBLENDCD, 
                              colortestfg = @COLORTESTFG,
                              templotno = @TEMPLOTNO,
                              istemp = @ISTEMP,
                              isnascalotcharend = @ISNASCALOTCHAREND,
                              lastupddt = @LASTUPDDT,
                              macgroup = @MACGROUP,
                              isbadmarkframe = @ISBADMARKFRAME,
                              dbthrowdt = @DBTHROWDT,
							  diesheartestfg = @DIESHEARTESTFG,
                              movestockct = @MOVESTOCKCT,
                              beforelifetestcondcd = @BEFORELIFETESTCONDCD,
							  tempcutblendno = @TEMPCUTBLENDNO,
                              mnggrid = @MNGGRID,
                              mnggrnm = @MNGGRNM,
                              resingpcd2 = @RESINGPCD2
                            WHERE 
                              lotno = @LOTNO";

                        #endregion
                    }

                    cmd.ExecuteNonQuery();

                    //ロット特性情報の連動更新
                    updateLotChars(constr);

                    //LENSのロット情報更新
                    updateLensLot(this.NascaLotNo);

                    //これ以降の更新処理等の関数追加は厳禁
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("ロット情報更新エラー:" + this.NascaLotNo, ex);
                }
            }
        }

        /// <summary>
        /// ロット特性リスト
        /// </summary>
        private void updateLotChars(string constr)
        {
            //ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
            try
            {
                if (this.IsColorTest)
                {
                    LotChar lc = new LotChar();
                    lc.LotCharCd = Config.COLOR_TEST_LOTCHAR_CD;
                    lc.ListVal = "1"; //ON
                    lc.DeleteInsert(this.NascaLotNo, constr);
                }
                else
                {
                    LotChar.Delete(this.NascaLotNo, Config.COLOR_TEST_LOTCHAR_CD);
                }


                if (this.IsKHLTest)
                {
                    LotChar lc = new LotChar();
                    lc.LotCharCd = Config.KHL_TEST_CT_LOTCHAR_CD;

                    if (Config.GetLineType == Config.LineTypes.MEL_COB)
                    {
                        lc.LotCharVal = Config.KHL_COB_TEST_CT.ToString();
                    }
                    else
                    {

                        lc.LotCharVal = Config.KHL_TEST_CT.ToString();
                    }
                    lc.DeleteInsert(this.NascaLotNo, constr);

                    lc.LotCharCd = Config.KHL_TEST_RESULT_LOTCHAR_CD;
                    lc.ListVal = "3";
                    lc.DeleteInsert(this.NascaLotNo, constr);
                }
                else
                {
                    LotChar.Delete(this.NascaLotNo, Config.KHL_TEST_RESULT_LOTCHAR_CD, constr);
                    LotChar.Delete(this.NascaLotNo, Config.KHL_TEST_CT_LOTCHAR_CD, constr);
                }


                if (this.IsLifeTest)
                {
                }
                else
                {
                    LotChar.Delete(this.NascaLotNo, Config.LIFE_TEST_CT_LOTCHAR_CD, constr);
                }

                if (this.IsReflowTest)
                {
                }
                else
                {
                    LotChar.Delete(this.NascaLotNo, Config.REFLOW_TEST_CT_LOTCHAR_CD, constr);
                }

                if (this.IsReflowTestWirebond)
                {
                }
                else
                {
                    LotChar.Delete(this.NascaLotNo, Config.REFLOW_TEST_CT_WB_LOTCHAR_CD, constr);
                }

                if (this.IsElasticityTest)
                {
                }
                else
                {
                    LotChar.Delete(this.NascaLotNo, Config.ELASTICITY_TEST_CT_LOTCHAR_CD, constr);
                }

                if (!string.IsNullOrWhiteSpace(this.DieShareSamplingPriority))
                {
                    LotChar lc = new LotChar();
                    lc.LotCharCd = Config.DIE_SHARE_SAMPLING_PRIORITY_LOTCHARCD;
                    lc.LotCharVal = this.DieShareSamplingPriority;
                    lc.DeleteInsert(this.NascaLotNo, constr);
                }
                else
                {
                    LotChar.Delete(this.NascaLotNo, Config.DIE_SHARE_SAMPLING_PRIORITY_LOTCHARCD, constr);
                }

                if (this.IsLoopCondChange)
                {
                    LotChar lc = new LotChar();
                    lc.LotCharCd = Config.LOOP_CHECK_SAMPLE_CT_LOTCHAR_CD;
                    lc.LotCharVal = "2"; // 2pcs抜取
                    lc.DeleteInsert(this.NascaLotNo, constr);
                }
                else
                {
                    LotChar.Delete(this.NascaLotNo, Config.LOOP_CHECK_SAMPLE_CT_LOTCHAR_CD);
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("ロット更新時の特性連動更新異常:" + this.NascaLotNo + ":" + ex.ToString());
            }
        }

        /// <summary>
        /// LENS2
        /// </summary>
        /// <param name="lotNo"></param>
        private void updateLensLot(string lotNo)
        {
            LENS.Lot lensLot = LENS.Lot.GetData(lotNo);
            if (lensLot == null)
            {
                lensLot = new LENS.Lot(lotNo);
                lensLot.TypeCd = this.TypeCd;
            }
            lensLot.IsMappingInspection = this.IsMappingInspection;
            lensLot.IsChangePoint = this.IsChangePointLot;
            lensLot.IsFullSizeInspection = this.IsFullSizeInspection;
            lensLot.InsertUpdate();
        }

        public void updateLensLot(string lotNo,string typecd)
        {
            LENS.Lot lensLot = LENS.Lot.GetData(lotNo);
            if (lensLot == null)
            {
                lensLot = new LENS.Lot(lotNo);
                lensLot.TypeCd = this.TypeCd;
            }
            lensLot.LotNo = lotNo;
            lensLot.TypeCd = typecd;
            lensLot.IsMappingInspection = this.IsMappingInspection;
            lensLot.IsChangePoint = this.IsChangePointLot;
            lensLot.IsFullSizeInspection = this.IsFullSizeInspection;
            lensLot.InsertUpdate();
        }

        #endregion

        /// <summary>
        /// ロット情報削除
        /// </summary>
        /// <param name="lotno"></param>
        public void Delete(SqlCommand cmd)
        {
            if (string.IsNullOrEmpty(this.NascaLotNo))
            {
                return;
            }

            string sql = " DELETE FROM TnLot WHERE (lotno = @LOTNO) ";

            cmd.CommandText = sql;

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.NascaLotNo;

            try
            {
                cmd.ExecuteNonQuery();

                Log.DelLog.Info(string.Format("[TnLot] {0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}",
                    this.NascaLotNo, this.TypeCd, this.IsWarning, this.IsRestricted, this.ProfileId, "NULL", this.BlendCd, this.ResinGpCd, this.CutBlendCd, this.IsLifeTest, this.IsKHLTest, this.IsColorTest, this.TempLotNo, this.IsTemp, this.IsNascaLotCharEnd, System.DateTime.Now, this.LotSize, this.IsFullSizeInspection, this.IsMappingInspection, this.IsChangePointLot, this.IsDieShearLot));
            }
            catch (Exception ex)
            {
                throw new ArmsException("ロット情報削除エラー:" + this.NascaLotNo, ex);
            }
        }

        #region SearchAsmLot

        public static AsmLot[] SearchAsmLot(string lotno, bool onlyTempLot, bool onlyNascaNotEnd, int? profileId, string tempCutBlendNo)
        {
            //マガジン分割対応
            lotno = Order.MagLotToNascaLot(lotno);

            List<AsmLot> retv = new List<AsmLot>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT 
                          t.lotno , 
                          t.typecd ,
                          t.profileid, 
                          t.blendcd , 
                          t.resingpcd,
                          t.cutblendcd,
                          t.warningfg, 
                          t.colortestfg,
                          t.templotno,
                          t.istemp,
                          t.isnascalotcharend,
                          t.restrictfg,
                          t.lotsize,
                          t.macgroup,
                          t.isbadmarkframe,
                          t.dbthrowdt,
						  t.diesheartestfg,
						  t.movestockct,
						  t.beforelifetestcondcd,
                          t.tempcutblendno,
                          t.mnggrid,
                          t.mnggrnm, 
                          t.resingpcd2
                        FROM
                          tnlot t with(nolock)
                        WHERE
                          1=1 ";

                    if (string.IsNullOrEmpty(lotno) == false)
                    {
                        cmd.CommandText += " AND t.lotno = @LOTNO";
                        cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                    }

                    if (onlyTempLot)
                    {
                        cmd.CommandText += " AND istemp = 1";
                    }

                    if (onlyNascaNotEnd)
                    {
                        cmd.CommandText += " AND isnascalotcharend <> 1";
                    }

                    if (profileId.HasValue)
                    {
                        cmd.CommandText += " AND profileid = @PROFILEID ";
                        cmd.Parameters.Add("@PROFILEID", SqlDbType.BigInt).Value = profileId.Value;
                    }

                    //先行ライフ対応 2015.8.11湯浅
                    if (tempCutBlendNo != String.Empty)
                    {
                        cmd.CommandText += " AND tempcutblendno = @TEMPCUTBLENDNO ";
                        cmd.Parameters.Add("@TEMPCUTBLENDNO", SqlDbType.NVarChar).Value = tempCutBlendNo;
                    }

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        int ordMacGroup = rd.GetOrdinal("macgroup");
                        int ordResingpCd = rd.GetOrdinal("resingpcd");
                        int ordResingpCd2 = rd.GetOrdinal("resingpcd2");

                        while (rd.Read())
                        {
                            AsmLot lot = new AsmLot();
                            lot.NascaLotNo = SQLite.ParseString(rd["lotno"]);
                            lot.TypeCd = SQLite.ParseString(rd["typecd"]);
                            lot.IsWarning = SQLite.ParseBool(rd["warningfg"]);
                            lot.IsRestricted = SQLite.ParseBool(rd["restrictfg"]);
                            lot.ProfileId = SQLite.ParseInt(rd["profileid"]);

                            //2015.2.4 複数樹脂グループ投入対応
                            lot.ResinGpCd = new List<string>();
                            if (!rd.IsDBNull(ordResingpCd))
                            {
                                lot.ResinGpCd.AddRange(rd.GetString(ordResingpCd).Split(','));
                            }

                            lot.ResinGpCd2 = new List<string>();
                            if (!rd.IsDBNull(ordResingpCd2))
                            {
                                lot.ResinGpCd2.AddRange(rd.GetString(ordResingpCd2).Split(','));
                            }

                            lot.BlendCd = SQLite.ParseString(rd["blendcd"]);
                            lot.IsColorTest = SQLite.ParseBool(rd["colortestfg"]);
                            lot.CutBlendCd = SQLite.ParseString(rd["cutblendcd"]);
                            lot.TempLotNo = SQLite.ParseString(rd["templotno"]);
                            lot.IsNascaLotCharEnd = SQLite.ParseBool(rd["isnascalotcharend"]);
                            lot.IsTemp = SQLite.ParseBool(rd["istemp"]);
                            lot.LotSize = SQLite.ParseInt(rd["lotsize"]);
                            lot.MacGroup = new List<string>();
                            if (!rd.IsDBNull(ordMacGroup))
                            {
                                lot.MacGroup.AddRange(rd.GetString(ordMacGroup).Split(','));
                            }
                            lot.IsBadMarkFrame = SQLite.ParseBool(rd["isbadmarkframe"]);
                            lot.DBThrowDT = SQLite.ParseString(rd["dbthrowdt"]);
                            lot.IsDieShearLot = SQLite.ParseBool(rd["diesheartestfg"]);
                            lot.MoveStockCt = SQLite.ParseInt(rd["movestockct"]);

                            // 2015.6.16 車載高 先行ライフ試験対応
                            lot.BeforeLifeTestCondCd = SQLite.ParseString(rd["beforelifetestcondcd"]);
                            lot.TempCutBlendNo = SQLite.ParseString(rd["tempcutblendno"]);
                            // -----------------------------------

                            List<LotChar> lotChars = LotChar.GetLotChar(lot.NascaLotNo).ToList();
                            if (lotChars.Exists(lc => lc.LotCharCd == Config.DIE_SHARE_SAMPLING_PRIORITY_LOTCHARCD))
                            {
                                lot.DieShareSamplingPriority = lotChars.Find(lc => lc.LotCharCd == Config.DIE_SHARE_SAMPLING_PRIORITY_LOTCHARCD).LotCharVal;
                            }

                            //<--Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応 
                            Profile prof = Profile.GetProfile(lot.ProfileId);
                            if (prof != null) { 
                                lot.ScheduleSelectionStandard = prof.ScheduleSelectionStandard;
                            }
                            //-->Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応 

                            lotChars = lotChars.Where(lc => lc.LotCharVal != "0").ToList();
                            if (lotChars.Count != 0)
                            {
                                if (lotChars.Exists(lc => lc.LotCharCd == Config.LIFE_TEST_CT_LOTCHAR_CD))
                                {
                                    lot.IsLifeTest = true;
                                }
                                if (lotChars.Exists(lc => lc.LotCharCd == Config.KHL_TEST_CT_LOTCHAR_CD))
                                {
                                    lot.IsKHLTest = true;
                                }
                                if (lotChars.Exists(lc => lc.LotCharCd == Config.REFLOW_TEST_CT_LOTCHAR_CD))
                                {
                                    lot.IsReflowTest = true;
                                }
                                if (lotChars.Exists(lc => lc.LotCharCd == Config.REFLOW_TEST_CT_WB_LOTCHAR_CD))
                                {
                                    lot.IsReflowTestWirebond = true;
                                }
                                if (lotChars.Exists(lc => lc.LotCharCd == Config.ELASTICITY_TEST_CT_LOTCHAR_CD))
                                {
                                    lot.IsElasticityTest = true;
                                }
                                if (lotChars.Exists(lc => lc.LotCharCd == Config.LOOP_CHECK_SAMPLE_CT_LOTCHAR_CD))
                                {
                                    lot.IsLoopCondChange = true;
                                }
                            }
                            lot.MnggrId = SQLite.ParseNullableInt(rd["mnggrid"]);
                            lot.MnggrNm = SQLite.ParseString(rd["mnggrnm"]);
                            retv.Add(lot);
                        }
                    }

                    //1.88.0からフラグに関わらずLENSロットを生成するようになっているので取得を除外する理由が無いため、解除
                    //if (Config.Settings.IsMappingMode)
                    //{
                    foreach (AsmLot alot in retv)
                    {
                        LENS.Lot nLot = LENS.Lot.GetData(alot.NascaLotNo);
                        if (nLot == null)
                        {
                            throw new Exception(string.Format("LENSにロット情報が存在しない為、正しい情報を表示できません。ロットNO：{0}", lotno));
                        }

                        alot.IsChangePointLot = nLot.IsChangePoint;
                        alot.IsMappingInspection = nLot.IsMappingInspection;
                        alot.IsFullSizeInspection = nLot.IsFullSizeInspection;
                    }
                    //}
                }
                catch (Exception ex)
                {
                    throw new ArmsException("ロット情報取得時エラー:" + lotno, ex);
                }
            }

            return retv.ToArray();
        }

        public static AsmLot[] SearchAsmLot(int profileId)
        {
            return SearchAsmLot(null, false, false, profileId, String.Empty);
        }

        public static AsmLot[] SearchAsmLot(string lotno, bool onlyTempLot, bool onlyNascaNotEnd, int? profileId)
        {
            return SearchAsmLot(lotno, onlyTempLot, onlyNascaNotEnd, profileId, String.Empty);
        }

        #endregion

        #region GetAsmLot
        /// <summary>
        /// インラインマガジンロット情報取得
        /// </summary>
        /// <param name="schParam">検索条件</param>
        /// <returns>インラインマガジンロット情報構造体</returns>
        public static AsmLot GetAsmLot(string lotno)
        {
            //マガジン分割対応
            lotno = Order.MagLotToNascaLot(lotno);

            AsmLot[] list = SearchAsmLot(lotno, false, false, null);
            if (list.Length == 0)
            {
                return null;
            }
            else
            {
                return list[0];
            }
        }
        #endregion

        #region GetLotLog

        public static SortedList<DateTime, string> GetLotLog(string lotno)
        {
            SortedList<DateTime, string> retv = new SortedList<DateTime, string>();

            List<AsmLotLog> logs = GetLotLog(lotno, null, null, string.Empty, string.Empty, false, string.Empty, false, false);
            foreach (AsmLotLog log in logs)
            {
                retv.Add(log.InDt, log.Message);
            }

            return retv;
        }

        public static List<AsmLotLog> GetLotLog(string lineNo, bool isError, bool isTop10)
        {
            List<AsmLotLog> logs;
            if (isError)
            {
                logs = GetLotLog(null, null, null, string.Empty, string.Empty, true, lineNo, isTop10, false);
            }
            else
            {
                logs = GetLotLog(null, null, null, string.Empty, string.Empty, false, lineNo, isTop10, true);
            }
            return logs;
        }

        public static List<AsmLotLog> GetLotLog(string lotno, DateTime? fromdt, DateTime? todt, string message, string magazineno, bool iserror, string lineno)
        {
            List<AsmLotLog> logs = GetLotLog(lotno, null, null, string.Empty, string.Empty, false, string.Empty, false, false);
            return logs;
        }

        public static List<AsmLotLog> GetLotLog(string lotno, DateTime? fromdt, DateTime? todt, string message, string magazineno, bool iserror, string lineno, bool isTop10, bool isnormal)
        {
            List<AsmLotLog> retv = new List<AsmLotLog>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = " SELECT ";

                    if (isTop10)
                    {
                        sql += " Top 10 ";
                    }

                    cmd.CommandText = sql += @" lotno, indt, msg, procno, magno, errorfg, line
										FROM TnLotLog with(nolock) WHERE 1=1 ";

                    if (!string.IsNullOrEmpty(lotno))
                    {
                        cmd.CommandText += " AND lotno=@LOTNO ";
                        cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                    }
                    if (fromdt.HasValue && todt.HasValue)
                    {
                        cmd.CommandText += " AND indt>=@INFROMDT AND indt<@INTODT";
                        cmd.Parameters.Add("@INFROMDT", SqlDbType.DateTime).Value = fromdt.Value;
                        cmd.Parameters.Add("@INTODT", SqlDbType.DateTime).Value = todt.Value;
                    }
                    if (!string.IsNullOrEmpty(message))
                    {
                        cmd.CommandText += " AND msg Like @MSG ";
                        cmd.Parameters.Add("@MSG", SqlDbType.NVarChar).Value = message + "%";
                    }
                    if (!string.IsNullOrEmpty(magazineno))
                    {
                        cmd.CommandText += " AND magno=@MAGNO ";
                        cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = magazineno;
                    }
                    if (iserror)
                    {
                        cmd.CommandText += " AND errorfg=@ERRFG ";
                        cmd.Parameters.Add("@ERRFG", SqlDbType.Int).Value = SQLite.SerializeBool(iserror);
                    }
                    if (isnormal)
                    {
                        cmd.CommandText += " AND errorfg=@ERRFG ";
                        cmd.Parameters.Add("@ERRFG", SqlDbType.Int).Value = SQLite.SerializeBool(false);
                    }

                    if (!string.IsNullOrEmpty(lineno))
                    {
                        cmd.CommandText += " AND line=@LINENO ";
                        cmd.Parameters.Add("@LINENO", SqlDbType.NVarChar).Value = lineno;
                    }

                    if (isTop10)
                    {
                        cmd.CommandText += " ORDER BY indt DESC ";
                    }

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            AsmLotLog log = new AsmLotLog();

                            log.LotNo = rd["lotno"].ToString().Trim();
                            log.InDt = SQLite.ParseDate(rd["indt"]) ?? DateTime.MinValue;
                            log.Message = SQLite.ParseString(rd["msg"]).Trim();
                            log.ProcNo = SQLite.ParseInt(rd["procno"]);

                            Process p = Process.GetProcess(log.ProcNo);
                            log.ProcNm = p.InlineProNM;

                            log.MagazineNo = rd["magno"].ToString().Trim();
                            log.IsError = SQLite.ParseBool(rd["errorfg"]);
                            log.LineNo = rd["line"].ToString().Trim();
                            retv.Add(log);
                        }
                    }
                }

                return retv;
            }
            catch (Exception ex)
            {
                throw new ArmsException(ex.Message);
            }
        }

        #endregion

        #region InsertLotLog
        //ロット単位の応答ログ保存
        public static void InsertLotLog(string lotno, DateTime indt, string msg, int procno, string magno, bool iserror, string lineno, string updusercd)
        {
            if (string.IsNullOrEmpty(msg))
            {
                msg = "不明";
            }

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                cmd.Parameters.Add("@INDT", SqlDbType.DateTime).Value = indt;
                cmd.Parameters.Add("@MSG", SqlDbType.NVarChar).Value = msg;
                cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno;
                cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = magno;
                cmd.Parameters.Add("ERRFG", SqlDbType.Int).Value = SQLite.SerializeBool(iserror);
                cmd.Parameters.Add("@LINENO", SqlDbType.NVarChar).Value = lineno;
                cmd.Parameters.Add("@UPDUSERCD", SqlDbType.NVarChar).Value = updusercd ?? (object)System.DBNull.Value;

                try
                {
                    con.Open();

                    //DELETE
                    cmd.CommandText = @"DELETE FROM TnLotLog WHERE lotno=@LOTNO AND indt=@INDT";
                    cmd.ExecuteNonQuery();

                    //INSERT
                    cmd.CommandText = @"
                            INSERT INTO TnLotLog (lotno, indt, msg, procno, magno, errorfg, line, updusercd) 
                            VALUES (@LOTNO, @INDT, @MSG, @PROCNO, @MAGNO, @ERRFG, @LINENO, @UPDUSERCD)";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException(ex.Message);
                }
            }
        }

        public static void InsertLotLog(string lotno, DateTime indt, string msg, int procno, string magno, bool iserror, string lineno)
        {
            InsertLotLog(lotno, indt, msg, procno, magno, iserror, lineno, string.Empty);
        }
        #endregion

        public static void DeleteLotLog(SqlCommand cmd, string lotno)
        {
            if (string.IsNullOrEmpty(lotno))
            {
                return;
            }

            //削除ログ出力用
            SortedList<DateTime, string> lotLogs = AsmLot.GetLotLog(lotno);

            string sql = " DELETE FROM TnLotLog WHERE lotno Like @LOTNO ";
            cmd.CommandText = sql;

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";

            try
            {
                cmd.ExecuteNonQuery();

                foreach (KeyValuePair<DateTime, string> lotLog in lotLogs)
                {
                    Log.DelLog.Info(string.Format("[TnLotLog] {0}\t{1}\t{2}",
                        lotno, lotLog.Key, lotLog.Value));
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException(ex.Message);
            }
        }

        public static void DeleteLotTray(string lotno)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    string sql = " DELETE FROM TnLotTray WHERE lotno = @LotNo";
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
        /// <summary>
        /// NASCAから供給IDを取得
        /// </summary>
        /// <param name="blendcd"></param>
        /// <returns></returns>
        public static string GetNascaWaferShipId(string blendcd)
        {
            List<string> idlist = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                #region SQL

                string sql = @"
                     select Supply_ID from ntmbdss(nolock)
                     where blend_cd = @BLENDCD";
                #endregion

                cmd.CommandText = sql;
                cmd.Parameters.Add("@BLENDCD", System.Data.SqlDbType.VarChar).Value = blendcd;

                con.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string id = rd["Supply_id"].ToString().Trim();
                        if (idlist.Contains(id) == false)
                        {
                            idlist.Add(id);
                        }
                    }
                }

                string retv = "";
                bool first = true;
                foreach (string s in idlist)
                {
                    if (!first) retv += ":";
                    retv += s;
                    first = false;
                }

                return retv;
            }
        }

        public static bool UpdatePreCutBlendNo(string[] lotNoList) //2015.10.7 SLP1特採対応。仮ブレンド時にカットブレンドロットを取得。
        {
            using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
            {
                string tempCutBlendNo = String.Empty;

                if (lotNoList.Length == 0)
                {
                    return false;
                }
                else if (lotNoList.Length == 1)
                {
                    tempCutBlendNo = lotNoList[0];
                }
                else
                {
                    tempCutBlendNo = Numbering.GetNewCutBlendLotNo(DateTime.Now, "01", false); //仮ブレンド時のlinenoは01固定。(2015.10.8小林LLに確認済み！)
                }

                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();

                    try
                    {

                        cmd.CommandText = " UPDATE TnLot SET tempcutblendno = @TempCutBlendNo WHERE lotno = @LotNo ";

                        SqlParameter pTempCutBlendNo = cmd.Parameters.Add("@TempCutBlendNo", SqlDbType.NVarChar);
                        SqlParameter pLotNo = cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar);

                        foreach (string lotNo in lotNoList)
                        {
                            pTempCutBlendNo.Value = tempCutBlendNo;
                            pLotNo.Value = lotNo;

                            cmd.ExecuteNonQuery();
                        }

                        cmd.Transaction.Commit();
                    }
                    catch (Exception)
                    {
                        cmd.Transaction.Rollback();
                        return false;
                    }
                }
            }

            return true;
        }

        public static int GetLedDiceCount(string typeCd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = " SELECT Die_CT FROM TmDIECT WITH(nolock) WHERE Material_CD = @TypeCd";

                cmd.Parameters.Add("@TypeCd", SqlDbType.Char).Value = typeCd;

                cmd.CommandText = sql;

                object dieCt = cmd.ExecuteScalar();
                if (dieCt == null)
                {
                    throw new ApplicationException(string.Format("この型番のチップ数量はマスタに設定されていません。型番:{0}", typeCd));
                }
                else
                {
                    return Convert.ToInt32(dieCt);
                }
            }
        }

        /// <summary>
        /// ロットとトレイの関連付け (※現状スパッタ装置専用)
        /// </summary>
        public static void RelateTray(string lotNo, string trayNo, int orderNo)
        {
            UpdateRelateTray(lotNo, trayNo, orderNo, true, false);
        }

        /// <summary>
        /// ロットとトレイの関連付け (※現状スパッタ装置専用)
        /// </summary>
        public static void UpdateRelateTray(string lotNo, string trayNo, int orderNo, bool newFg, bool temprelateFg)
        {
            if (string.IsNullOrWhiteSpace(lotNo) && string.IsNullOrWhiteSpace(trayNo))
            {
                throw new ApplicationException(string.Format("UpdateRelateTray関数の引数が不正です。"));
            }

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    string sql = @" SELECT * FROM TnLotTray  
                            WHERE lotno = @LotNo AND trayno = @TrayNo ";

                    cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;
                    cmd.Parameters.Add("@TrayNo", SqlDbType.NVarChar).Value = trayNo;
                    cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = orderNo;
                    cmd.Parameters.Add("@NewFg", SqlDbType.Int).Value = SQLite.SerializeBool(newFg);
                    cmd.Parameters.Add("@TempRelateFg", SqlDbType.Int).Value = SQLite.SerializeBool(temprelateFg);
                    cmd.Parameters.Add("@LastUpdDt", SqlDbType.DateTime).Value = DateTime.Now;

                    cmd.CommandText = sql;
                    object data = cmd.ExecuteScalar();

                    if (data == null)
                    {
                        sql = @" INSERT INTO TnLotTray(trayno, lotno, orderno, newfg, temprelatefg, lastupddt) 
                             VALUES(@TrayNo, @LotNo, @OrderNo, @NewFg, @TempRelateFg, @LastUpdDt)";
                    }
                    else
                    {
                        sql = @" UPDATE TnLotTray SET orderno = @OrderNo, newfg = @NewFg, temprelatefg = @TempRelateFg, lastupddt = @LastUpdDt
                             WHERE lotno = @LotNo AND trayno = @TrayNo ";
                    }

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    //これ以降の更新処理等の関数追加は厳禁
                    cmd.Transaction.Commit();

                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw new ApplicationException(string.Format("ロット:{0}とトレイ:{1}の関連付けに失敗しました。", lotNo, trayNo));
                }
            }
        }

		public class LotTray
		{
			public string TrayNo { get; set; }
			public string LotNo { get; set; }
			public int OrderNo { get; set; }
            public bool NewFg { get; set; }
            public bool TempRelateFg { get; set; }
            public DateTime LastUpdDt { get; set; }
		}
		public static List<LotTray> GetRelateTray(string lotNo)
		{
            return GetRelateTray(null, null, lotNo, true, false);
        }
        public static List<LotTray> GetRelateTray(string trayNo, int? orderNo, string lotNo, bool onlyRelating, bool tempRelating)
        {
			List<LotTray> lotTrayList = new List<LotTray>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
				con.Open();

				string sql = @" SELECT trayno, lotno, orderno, newfg, temprelatefg, lastupddt
								FROM TnLotTray WITH(nolock)
								WHERE 1 = 1 ";

				if (string.IsNullOrEmpty(lotNo) == false)
				{
					sql += " AND lotno = @LotNo ";
                    cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;
				}
                if (string.IsNullOrEmpty(trayNo) == false)
                {
                    sql += " AND trayno = @TrayNo ";
                    cmd.Parameters.Add("@TrayNo", SqlDbType.NVarChar).Value = trayNo;
                }
                if (orderNo.HasValue)
                {
                    sql += " AND orderno = @OrderNo ";
                    cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = orderNo;
                }
                if (onlyRelating)
                {
                    sql += " AND newfg = 1 ";
                }

                if (tempRelating)
                {
                    sql += " AND temprelatefg = 1 ";
                }

                cmd.CommandText = sql;
				SqlDataReader rd = cmd.ExecuteReader();
				while(rd.Read())
				{
					LotTray tray = new LotTray();

					tray.TrayNo = rd["trayno"].ToString().Trim();
					tray.LotNo = rd["lotno"].ToString().Trim();
					tray.OrderNo = Convert.ToInt32(rd["orderno"]);
                    tray.NewFg = SQLite.ParseBool(rd["newfg"]);
                    tray.TempRelateFg = SQLite.ParseBool(rd["temprelatefg"]);
                    tray.LastUpdDt = SQLite.ParseDate(rd["lastupddt"]) ?? DateTime.MinValue;

					lotTrayList.Add(tray);
				}
            }

			return lotTrayList;
		}

        /// <summary>
        /// ロットとトレイの関連付けを解除する (※現状スパッタ装置専用)
        /// </summary>
        public static void DissolveTray(string lotNo, bool tempReleateFg)
        {
            DissolveTray(lotNo, null, null, tempReleateFg);
        }

        /// <summary>
        /// ロットとトレイの関連付けを解除する (※現状スパッタ装置専用)
        /// </summary>
        public static void DissolveTray(string trayNo, int? orderNo, bool tempReleateFg)
        {
            DissolveTray(null, trayNo, orderNo, tempReleateFg);
        }

        /// <summary>
        /// ロットとトレイの関連付けを解除する (※現状スパッタ装置専用)
        /// </summary>
        public static void DissolveTray(string lotNo, string trayNo, int? orderNo, bool tempReleateFg)
        {
            if (string.IsNullOrWhiteSpace(lotNo) && string.IsNullOrWhiteSpace(trayNo))
            {
                throw new ApplicationException(string.Format("DissolveTray関数の引数が不正です。"));
            }

            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" UPDATE TnLotTray SET lastupddt = @LastUpdDt, newfg = 0 "; 

                    cmd.Parameters.Add("@LastUpdDt", SqlDbType.DateTime).Value = DateTime.Now;

                    if (tempReleateFg == true)
                    {
                        sql += " , temprelatefg = 1 ";
                    }
                    else
                    {
                        sql += " , temprelatefg = 0 ";
                    }

                    sql += " WHERE 1 = 1 ";

                    if (string.IsNullOrEmpty(trayNo) == false)
                    {
                        sql += " AND trayno = @TrayNo ";
                        cmd.Parameters.Add("@TrayNo", SqlDbType.NVarChar).Value = trayNo;
                    }

                    if (orderNo.HasValue)
                    {
                        sql += " AND orderno = @OrderNo ";
                        cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = orderNo.Value;
                    }

                    if (string.IsNullOrEmpty(lotNo) == false)
                    {
                        sql += " AND lotno = @LotNo ";
                        cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;
                    }

                    if (tempReleateFg == true)
                    {
                        sql += " AND newfg = 1 ";
                    }

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                throw new ApplicationException(string.Format("ロット:{0}とトレイの関連付けの解除に失敗しました。", lotNo));
            }
        }
    }
    public class AsmLotLog
    {
        public string LotNo{ get; set; }
        public DateTime InDt { get; set; }
        public string Message { get; set; }
		public int ProcNo { get; set; }
		public string ProcNm { get; set; }
		public string MagazineNo { get; set; }
		public bool IsError { get; set; }
		public string LineNo { get; set; }
    }
}
