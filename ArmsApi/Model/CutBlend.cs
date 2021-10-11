﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class CutBlend
    {
        public const string PRESSDIE_WORKCOND_CD = "PRESSDIE";

        public int MacNo { get; set; }
        public string LotNo { get; set; }
        public string BlendLotNo { get; set; }
        public string MagNo { get; set; }
        public bool IsComplete
        {
            get
            {
                return EndDt.HasValue;
            }
        }

        public DateTime StartDt { get; set; }
        public DateTime? EndDt { get; set; }

        public bool IsNascaStart { get; set; }
        public bool IsNascaEnd { get; set; }
        public bool DelFg { get; set; }


        #region InputAsmLot

        /// <summary>
        /// アッセンロットを追加
        /// </summary>
        /// <param name="order"></param>
        public static void InputAsmLot(Order order)
        {

            Magazine mag = Magazine.GetCurrent(order.InMagazineNo);

            CutBlend[] exists = SearchBlendRecord(mag.NascaLotNO, null, null, false, false);
            if (exists.Length > 1)
            {
                foreach (CutBlend blend in exists)
                {
                    //別の装置に投入中のアッセンロットの場合
                    if (blend.IsComplete == false && blend.MacNo != order.MacNo)
                    {
                        throw new ArmsException("このロットは別の装置でブレンド中です:" + blend.MacNo);
                    }

                    //完成済みの別ブレンドロットにロットが含まれている場合
                    if (blend.IsComplete == true)
                    {
                        throw new ArmsException("このロットは別のブレンドロットに含まれています:" + blend.BlendLotNo);
                    }
                }
            }

            CutBlend newrec = new CutBlend();
            newrec.MacNo = order.MacNo;
            newrec.MagNo = order.InMagazineNo;
            newrec.StartDt = order.WorkStartDt;
            newrec.EndDt = null;
            newrec.BlendLotNo = "";
            newrec.LotNo = mag.NascaLotNO;
            newrec.IsNascaEnd = false;
            newrec.IsNascaStart = false;
            newrec.DeleteInsert();
        }
        #endregion

        #region DeleteInsert

        /// <summary>
        /// ブレンド情報更新
        /// 装置・ロット主キーでDeleteInsert
        /// </summary>
        public void DeleteInsert()
        {
            Log.SysLog.Info("カットブレンド情報更新" + this.LotNo);
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo ?? "";
                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.MacNo;
                    cmd.Parameters.Add("@MAGNO", SqlDbType.NVarChar).Value = this.MagNo ?? "";
                    cmd.Parameters.Add("@BLENDLOT", SqlDbType.NVarChar).Value = this.BlendLotNo ?? "";

                    cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = this.StartDt;
                    cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = (object)this.EndDt ?? DBNull.Value;

                    cmd.Parameters.Add("@NASCASTARTFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsNascaStart);
                    cmd.Parameters.Add("@NASCAENDFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsNascaEnd);
                    cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.DelFg);

                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                    //前履歴は削除
                    cmd.CommandText = "DELETE FROM TnCutBlend WHERE lotno=@LOTNO AND macno=@MACNO";
                    cmd.ExecuteNonQuery();

                    //新規Insert
                    cmd.CommandText = @"
                            INSERT
                             INTO TnCutBlend(lotno
	                            , macno
                                , blendlotno
	                            , startdt
                                , enddt
	                            , magno
	                            , nascastartfg
                                , nascaendfg
                                , delfg
	                            , lastupddt )
                            values(@LOTNO
	                            , @MACNO
	                            , @BLENDLOT
	                            , @STARTDT
                                , @ENDDT
	                            , @MAGNO
	                            , @NASCASTARTFG
                                , @NASCAENDFG
                                , @DELFG
	                            , @UPDDT)";

                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("カットブレンド情報更新エラー", ex);
                }
            }
        }

        #endregion

        public static void Delete(SqlCommand cmd, string lotNo, string blendLotNo) 
        {
            if (string.IsNullOrEmpty(lotNo) && string.IsNullOrEmpty(blendLotNo))
            {
                return;
            }

            //物理削除に変更
            /*string sql = " UPDATE TnCutBlend SET delfg = 1 WHERE 1=1 ";*/
            string sql = " DELETE TnCutBlend WHERE 1=1 ";

            cmd.Parameters.Clear();
            if (string.IsNullOrEmpty(lotNo) == false)
            {
                sql += " AND lotno Like @LOTNO ";
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotNo;
            }

            if (string.IsNullOrEmpty(blendLotNo) == false)
            {
                sql += " AND blendlotno = @BLENDLOTNO ";
                cmd.Parameters.Add("@BLENDLOTNO", SqlDbType.NVarChar).Value = blendLotNo;
            }
            cmd.CommandText = sql;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new ArmsException("カット作業履歴削除エラー:" + lotNo, ex);
            }
        }

        public static void Delete(SqlCommand cmd, string lotNo) 
        {
            Delete(cmd, lotNo, string.Empty);
        }

		public static void Delete(string blendLotNo)
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				Delete(cmd, string.Empty, blendLotNo);
			}
		}

        /// <summary>
        /// 現在アクティブなブレンドレコード取得
        /// </summary>
        /// <param name="inlineMachineNo"></param>
        /// <returns></returns>
        public static CutBlend[] GetCurrentBlendItems(int inlineMachineNo)
        {
            return SearchBlendRecord(null, null, inlineMachineNo, true, false);
        }

		public static CutBlend[] GetData(int macNo, DateTime startTo, DateTime endFrom) 
		{
			return SearchBlendRecord(null, null, null, false, false, null, startTo, endFrom, null);
		}

		public static CutBlend[] GetData(string lotNo)
		{
			return SearchBlendRecord(lotNo, null, null, false, false, null, null, null, null);
		}

        #region Search

        public static CutBlend[] SearchBlendRecord(string lotNo, string blendlotNo, int? inlineMacNo, bool isActiveOnly, bool isNascaNotEndOnly)
        {
            return SearchBlendRecord(lotNo, blendlotNo, inlineMacNo, isActiveOnly, isNascaNotEndOnly, null, null, null, null);
        }

        public static CutBlend[] SearchBlendRecord(string lotNo, string blendlotNo, int? inlineMacNo, bool isActiveOnly, bool isNascaNotEndOnly,
            DateTime? startfrom, DateTime? startto, DateTime? endfrom, DateTime? endto)
        {
            return SearchBlendRecord(lotNo, blendlotNo, inlineMacNo, isActiveOnly, isNascaNotEndOnly, startfrom, startto, endfrom, endto, false);
        }

        /// <summary>
        /// null許容の汎用検索
        /// </summary>
        /// <param name="nascalotNo"></param>
        /// <param name="inlineMacNo"></param>
        /// <param name="isActiveOnly"></param>
        /// <returns></returns>
        public static CutBlend[] SearchBlendRecord(string lotNo, string blendlotNo, int? inlineMacNo, bool isActiveOnly, bool isNascaNotEndOnly,
            DateTime? startfrom, DateTime? startto, DateTime? endfrom, DateTime? endto, bool blendlotlike)
        {
            List<CutBlend> retv = new List<CutBlend>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {


                try
                {
                    con.Open();
                    cmd.CommandText = @"
                        SELECT 
                          lotno
                          , macno
                          , blendlotno
                          , startdt
                          , enddt
                          , magno
                          , nascastartfg
                          , nascaendfg
                        FROM 
                          TnCutBlend 
                        WHERE 
                          delfg=0";

                    if (string.IsNullOrEmpty(lotNo) == false)
                    {
                        cmd.CommandText += " AND lotno = @LOTNO";
                        cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotNo;
                    }

                    if (string.IsNullOrEmpty(blendlotNo) == false)
                    {
                        if (blendlotlike)
                        {
                            cmd.CommandText += " AND blendlotno LIKE @BLENDLOTNO";
                            cmd.Parameters.Add("@BLENDLOTNO", SqlDbType.NVarChar).Value = blendlotNo + "%";
                        }
                        else
                        {
                            cmd.CommandText += " AND blendlotno = @BLENDLOTNO";
                            cmd.Parameters.Add("@BLENDLOTNO", SqlDbType.NVarChar).Value = blendlotNo;
                        }
                    }

                    if (inlineMacNo.HasValue == true)
                    {
                        cmd.CommandText += " AND macno = @MACNO";
                        cmd.Parameters.Add("@MACNO", SqlDbType.NVarChar).Value = inlineMacNo;
                    }

                    if (isActiveOnly == true)
                    {
                        cmd.CommandText += " AND enddt IS NULL";
                    }

                    if (isNascaNotEndOnly == true)
                    {
                        cmd.CommandText += " AND nascaendfg <> 1";
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
                            CutBlend b = new CutBlend();

                            b.MacNo = SQLite.ParseInt(reader["macno"]);
                            b.LotNo = SQLite.ParseString(reader["lotno"]);
                            b.BlendLotNo = SQLite.ParseString(reader["blendlotno"]);
                            b.MagNo = SQLite.ParseString(reader["magno"]);
                            b.StartDt = SQLite.ParseDate(reader["startdt"]) ?? DateTime.MinValue;
                            b.EndDt = SQLite.ParseDate(reader["enddt"]);
                            b.IsNascaStart = SQLite.ParseBool(reader["nascastartfg"]);
                            b.IsNascaEnd = SQLite.ParseBool(reader["nascaendfg"]);

                            retv.Add(b);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }

            return retv.ToArray();
        }

        #endregion

        #region CompleteBlend

        public static string CompleteBlend(DateTime enddt, CutBlend[] blendlist, string lineno, string empCd, bool isAutoLine)
        {
            string blendlotno = String.Empty;
            AsmLot listfirstlot = AsmLot.GetAsmLot(blendlist[0].LotNo);
            string tempblendlotno = listfirstlot.TempCutBlendNo;

            if (tempblendlotno != String.Empty) //2015.10.7 車載CT特採対応。仮ブレンドでロットを取得しているならばそちらを使う。
            {
                blendlotno = tempblendlotno;
            }
            else if (blendlist.Length == 0)
            {
                return blendlotno;
            }
            else if (blendlist.Length == 1)
            {
                blendlotno = blendlist[0].LotNo;
            }
            else
            {
                blendlotno = Numbering.GetNewCutBlendLotNo(enddt, lineno, isAutoLine);
            }

            if (blendlotno.StartsWith(Config.Settings.CutBlend12)) //2015.10.15 車載特採対応。事前にブレンドロットを取得するようになったため、ブレンドした場合はブレンドロットの重複エラーを実施。
            {
                Order[] readyBlendLot = Order.SearchOrder(blendlotno, null, null, false, false);
                if (readyBlendLot.Count() > 0)
                {
                    throw new ArmsException("仮登録されたブレンドロットが既に別ロットでブレンドされています　：" + blendlotno);
                }

            }
            
            Log.SysLog.Info("カット完了処理開始:" + blendlotno);

            DateTime start = enddt;
            #region 全ロット特性引き継ぎ

            try
            {
                //試験数関連は構成ロットの最大値を引き継ぐように修正
                List<LotChar> AllLotsCharValues = new List<LotChar>();
                List<Profile> AllLotsProfiles = new List<Profile>();
                foreach (CutBlend lot in blendlist)
                {
                    AsmLot svrLot = AsmLot.GetAsmLot(lot.LotNo);
                    AllLotsCharValues.AddRange(LotChar.GetLotChar(lot.LotNo).Where(l => string.IsNullOrWhiteSpace(l.LotCharVal)== false).ToList());
                    AllLotsProfiles.Add(Profile.GetProfile(svrLot.ProfileId));
                }

                GnlMaster[] testLotChars = GnlMaster.GetTestSampleLotChar();
                foreach (GnlMaster lotChar in testLotChars)
                {
                    LotChar lotCharMax = AllLotsCharValues.Where(l => l.LotCharCd == lotChar.Code)
                                                          .OrderByDescending(l => Convert.ToInt32(l.LotCharVal))
                                                          .FirstOrDefault();
                    if (lotCharMax == null) continue;

                    if (lotChar.Code == Config.HV_TEST_CT_LOTCHAR_CD)
                    {
                        if(AllLotsProfiles.Any(p=>p.HasHvTest == true))
                        {
                            if(Convert.ToInt32(lotCharMax.LotCharVal) < Config.HV_TEST_CT)
                            {
                                lotCharMax.LotCharVal = Config.HV_TEST_CT.ToString();
                            }
                        }
                    }
                    else if(lotChar.Code == Config.DELTA_L_TEST_CT_LOTCHAR_CD)
                    {
                        if (AllLotsProfiles.Any(p => p.HasDeltaLTest == true))
                        {
                            if (Convert.ToInt32(lotCharMax.LotCharVal) < 1)
                            {
                                lotCharMax.LotCharVal = "1";
                            }
                        }
                    }
                    lotCharMax.DeleteInsert(blendlotno);
                }


                foreach (CutBlend blend in blendlist)
                {
                    LotChar[] lclist = WorkCondition.GetLotCharFromLot(blend.LotNo);
                    foreach (LotChar lc in lclist)
                    {
                        //ライフ試験以外は最大ロット値でなく最終ロットの特性値しか引き継がない理由が特に無いので
                        //試験特性は外に出してブレンドロット内で最大の値を常に引き継ぐように変更する。 2018/6/11 湯浅/四宮
                        //※試験数以外はとりあえずそのまま。多分NASCA処理と違うので見直しが必要…
                        //if (lc.LotCharCd == LotChar.LIFETESTCT_LOTCHARCD)
                        //{
                        //    // ライフ試験数特性(T0000001)のみ、構成されるアッセンロットの中で数量が多い特性を挿入する
                        //    if (lifeCt >= int.Parse(lc.LotCharVal))
                        //        continue;

                        //    lifeCt = int.Parse(lc.LotCharVal);
                        //}

                        //試験数は先に処理しているのでスキップ
                        if (testLotChars.Any(t => t.Code == lc.LotCharCd) == true) continue; 

                        lc.DeleteInsert(blendlotno);
                    }

                    AsmLot lot = AsmLot.GetAsmLot(blend.LotNo);
                    Profile prof = AllLotsProfiles.Where(p=>p.ProfileId == lot.ProfileId).First();
                    LotChar lca = new LotChar();

                    //HV試験数の特性をプロファイルが持っている場合（0でも20にする）
                    if (prof.HasHvTest)
                    {
                        //試験数は先に処理しているのでスルー
                        //lca.LotCharCd = Config.HV_TEST_CT_LOTCHAR_CD;
                        //lca.LotCharVal = Config.HV_TEST_CT.ToString();
                        //lca.ListVal = "";
                        //lca.DeleteInsert(blendlotno);

                        lca.LotCharCd = Config.HV_TEST_RESULT_LOTCHAR_CD;
                        lca.LotCharVal = "";
                        lca.ListVal = Config.HV_TEST_RESULT_LIST_VAL;
                        lca.DeleteInsert(blendlotno);
                    }

                    //⊿L試験数の特性をプロファイルが持っている場合（0でも1にする）
                    if (prof.HasDeltaLTest)
                    {
                        //試験数は先に処理しているのでスルー
                        //lca.LotCharCd = Config.DELTA_L_TEST_CT_LOTCHAR_CD;
                        //lca.LotCharVal = "1";
                        //lca.ListVal = "";
                        //lca.DeleteInsert(blendlotno);

                        lca.LotCharCd = Config.DELTA_L_TEST_RESULT_WAIT_LOTCHAR_CD;
                        lca.LotCharVal = "";
                        lca.ListVal = "";
                        lca.DeleteInsert(blendlotno);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("カットブレンド完成後　特性引き継ぎ保存異常" + blendlotno + ex.ToString());
            }
            #endregion

            #region 結果待ち付与（先行ライフ品対応）

            //2015.10.15 車載CT先行ライフ対応。対象ロットが「先行ライフ品(tempblendlotnoが空でない)」「色調先行品」「ライフ試験結果を持っていない」を全て
            //満たす場合に「ライフ試験結果：結果待ち」を付与する。
            //先行ライフ品はライフOK後にカットブレンドをするため、基本的にライフ試験結果特性を持てない。（ライフ結果が出た後にカットブレンドをすると
            //結果待ちと良の両方を持ってしまう）
            //しかし先行色調品はカットを待たないため、結果待ちを付与しておかないと選別でブレンドされたときに結果待ちを含むか後工程でわからなくなる。
            if (listfirstlot.IsColorTest == true && listfirstlot.IsBeforeLifeTest)
            {
                LotChar lifeResult = LotChar.GetLifeTestResult(blendlotno);
                if (lifeResult == null)
                {
                    LotChar lc = new LotChar();

                    lc.LotCharCd = LotChar.LIFETESTRESULTCONDCD_LOTCHARCD;
                    lc.ListVal = "3"; // 3 = 結果待ち。仕様上これ以外関係ないので決め打ち。                    
                    lc.LotCharVal = "3";

                    lc.DeleteInsert(blendlotno);
                }
            }
            #endregion


            try
            {
                foreach (CutBlend blend in blendlist)
                {
                    if (start >= blend.StartDt) start = blend.StartDt;
                }


                Order ord = new Order();
                ord.WorkStartDt = start;
                ord.WorkEndDt = enddt;
                ord.InspectCt = 0;
                ord.MacNo = blendlist[0].MacNo;
                ord.LotNo = blendlotno;
				ord.TranStartEmpCd = empCd;
				ord.TranCompEmpCd = empCd;
                string typecd = AsmLot.GetAsmLot(blendlist[0].LotNo).TypeCd;
                Process final = Process.GetWorkFlow(typecd).Where(p => p.FinalSt == true).FirstOrDefault();
                ord.ProcNo = final.ProcNo;
                ord.DeleteInsert(blendlotno);
            }
            catch (Exception ex)
            {
                throw new ArmsException("カットブレンド完成後　Tran保存異常" + blendlotno + ex.ToString());
            }

            foreach (CutBlend blend in blendlist)
            {
                blend.BlendLotNo = blendlotno;
                blend.EndDt = enddt;
                blend.DeleteInsert();
            }

            Log.SysLog.Info("カット完了処理完了:" + blendlotno);

            return blendlotno;
        }
        #endregion

        #region CalcTotal 完成数計算

        public static void CalcTotal(CutBlend[] blendlist, string blendlotNo, out int total, out int deftotal, out int testtotal)
        {
            CalcTotal(blendlist, blendlotNo, false, out total, out deftotal, out testtotal);
        }

        public static void CalcTotal(CutBlend[] blendlist, string blendlotNo, bool afterFinalStFg, out int total, out int deftotal, out int testtotal)
        {
            total = 0;
            deftotal = 0;
            testtotal = 0;
			int movetotal = 0;

            bool isSingleLot = false;
            if (blendlist.Length == 1) isSingleLot = true;

            //アッセンロット情報集計
            foreach (CutBlend bln in blendlist)
            {
                AsmLot lot = AsmLot.GetAsmLot(bln.LotNo);
                if (lot == null) throw new ArmsException("ロット情報がありません" + bln.LotNo);

                Profile prof = Profile.GetProfile(lot.ProfileId);
                if (prof == null) throw new ArmsException("プロファイル情報がありません" + lot.ProfileId.ToString());

                Process[] proclist = Process.GetWorkFlow(lot.TypeCd);
                Process finalprocess = proclist.Where(r => r.FinalSt == true).FirstOrDefault();
                if (afterFinalStFg == true && finalprocess == null)
                {
                    throw new ApplicationException($"FinalSt=trueの工程が取得できませんでした。型番:{lot.TypeCd}");
                }

                //1ロット数量加算
                total += prof.LotSize;

				//2015.2.5 車載3次 部門間移動数加算
				movetotal += lot.MoveStockCt;

                //不良数計算
                Defect def = Defect.GetDefect(lot.NascaLotNo, null);
                foreach (DefItem di in def.DefItems)
                {
                    //ブレンドロットの場合カット中に不良入力した数をアッセンロット、カット工程で登録する為、その分を差し引く
                    if (isSingleLot == false && Process.GetProcess(di.ProcNo).FinalSt == true)
                    {
                        continue;
                    }

                    Process p = proclist.Where(r => r.ProcNo == di.ProcNo).SingleOrDefault();
                    if (p == null)
                    {
                        throw new ApplicationException($"作業マスタに存在しない工程です。ロットNO：{lot.NascaLotNo} 型番:{lot.TypeCd} 工程NO：{di.ProcNo}");
                    }

                    //ブレンドロットかつ最終作業以降の作業の場合、ブレンドロットで加算するのでスルー
                    if (afterFinalStFg == true &&  p.WorkOrder >= finalprocess.WorkOrder && isSingleLot == false)
                    {
                        continue;
                    }

                    deftotal += di.DefectCt;
                }
            }

            if (isSingleLot == false)
            {
                //カットロット情報
                Defect cbdef = Defect.GetDefect(blendlotNo, null);
                foreach (DefItem di in cbdef.DefItems)
                {
                    deftotal += di.DefectCt;
                }
            }

            testtotal = calcTestCt(blendlotNo, blendlist);
            
			// 2015.2.5 車載3次 部門間移動数を減らす
			//total = total - testtotal - deftotal;
			total = total - testtotal - deftotal - movetotal;
        }
        #endregion

        #region IsEICSDicingEnd / GetEICSDicingEndDt

        /// <summary>
        /// EICS上でダイシングの完了データが存在するかの確認
        /// </summary>
        /// <returns></returns>
        public static bool IsEICSDicingEnd(string plantcd, string lotno)
        {
            DateTime? enddt = GetEICSDicingEndDt(plantcd, lotno);
            if (enddt.HasValue) return true;
            else return false;
        }

        /// <summary>
        /// EICS上でダイシングの完了データが存在するかの確認
        /// </summary>
        /// <returns></returns>
        public static DateTime? GetEICSDicingEndDt(string plantcd, string lotno)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@PLANTCD", SqlDbType.Char).Value = plantcd;
                cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;

                // 消耗品  ブレード摩耗量(Z2) のデータが存在するかを確認
                cmd.CommandText = @"
                    SELECT [Measure_DT]
                    FROM [QCIL].[dbo].[TnLOG] With(NOLOCK)
                    Where Equipment_NO= @PLANTCD AND
                    QcParam_NO=902 AND
                    NascaLot_NO= @LOTNO AND
                    Del_FG=0  
                    ORDER BY Measure_DT DESC option(MAXDOP 1)";

                object o = cmd.ExecuteScalar();

                if (o != null)
                {
                    return Convert.ToDateTime(o);
                }

                return null;
            }
        }

        #endregion

        private static int calcTestCt(string cutBlendlotNo, CutBlend[] blendlist)
        {
			int retv = 0;

			GnlMaster[] lotChars = GnlMaster.GetTestSampleLotChar();
			foreach (GnlMaster lotChar in lotChars)
			{
				LotChar[] lotCharValues = LotChar.GetLotChar(cutBlendlotNo, lotChar.Code);
				foreach (LotChar lotCharValue in lotCharValues)
				{
					int i;
					if (int.TryParse(lotCharValue.LotCharVal, out i))
					{
						retv += i;
					}
					break;
				}
			}

            return retv;
        }


        #region カットラベル照合
        

        /// <summary>
        /// カットラベル照合時に何分以上経過したら警告を出すかの閾値
        /// 今後一度でもこの値が変更される場合は汎用マスタに切り出すこと
        /// </summary>
        private const int TIME_THRESHOLD_MINUTES = 120;

        public static bool CompareCutLabel(string cutLabel, int macno, out string errMsg)
        {
            string blendlotno;
            string typecd;

            try
            {
                string[] elm = cutLabel.Split(',');
                blendlotno = elm[0];
                typecd = elm[1];
            }
            catch
            {
                errMsg = "ラベルフォーマットが不正です";
                return false;
            }

            CutBlend[] lst = CutBlend.SearchBlendRecord(null, blendlotno, macno, false, false);

            if (lst.Length == 0)
            {
                errMsg = "該当装置で完成履歴が見つかりません:" + blendlotno;
                UpdateCutLabelCompareInfo(blendlotno, macno, false);
                return false;
            }

            AsmLot lot = AsmLot.GetAsmLot(lst[0].LotNo);

            if (lot == null || lot.TypeCd != typecd)
            {
                errMsg = "タイプ不整合:" + typecd;
                UpdateCutLabelCompareInfo(blendlotno, macno, false);
                return false;
            }
            else if (lst[0].EndDt < DateTime.Now.AddMinutes(-1 * TIME_THRESHOLD_MINUTES))
            {
                errMsg = "完成から" + TIME_THRESHOLD_MINUTES.ToString() + "分以上経過しています:" + blendlotno;
                UpdateCutLabelCompareInfo(blendlotno, macno, true);
                return false;
            }
            else
            {
                errMsg = "照合OK:" + blendlotno;
                UpdateCutLabelCompareInfo(blendlotno, macno, true);
                return true;
            }
        }

        public static bool IsCutLabelCompareOK(string blendlotno)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@BLENDLOTNO", SqlDbType.NVarChar).Value = blendlotno;

                try
                {

                    cmd.CommandText = "SELECT ISOK FROM TnCutBlendCompare WHERE blendlotno = @BLENDLOTNO";
                    bool isok = SQLite.ParseBool(cmd.ExecuteScalar());
                    return isok;
                }
                catch
                {
                    throw;
                }
            }
        }

        public static void UpdateCutLabelCompareInfo(string blendlotno, int macno, bool isOK)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@BLENDLOTNO", SqlDbType.NVarChar).Value = blendlotno;
                cmd.Parameters.Add("@MACNO", SqlDbType.Int).Value = macno;
                cmd.Parameters.Add("@ISOK", SqlDbType.Int).Value = SQLite.SerializeBool(isOK);
                cmd.Parameters.Add("@DATE", SqlDbType.DateTime).Value = DateTime.Now;

                try{

                    cmd.Transaction = con.BeginTransaction();
                    cmd.CommandText = "DELETE FROM TnCutBlendCompare WHERE blendlotno = @BLENDLOTNO";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                            INSERT INTO [dbo].[TnCutBlendCompare]
                                       ([blendlotno]
                                       ,[enddt]
                                       ,[macno]
                                       ,[isok]
                                       ,[lastupddt]
                                       ,[delfg])
                                 VALUES
                                       (@BLENDLOTNO
                                       ,@DATE
                                       ,@MACNO
                                       ,@ISOK
                                       ,@DATE
                                       ,0)";
					cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
            }

        }

        #endregion


        #region PrintMapPreCutLabel

        /// <summary>
        /// 
        /// </summary>
        /// <param name="macno"></param>
        /// <param name="lotno"></param>
        /// <param name="magno"></param>
        public static void PrintMapPreCutLabel(int macno, string lotno, string magno)
        {
            AsmLot lot = AsmLot.GetAsmLot(lotno);
            MachineInfo mac = MachineInfo.GetMachine(macno);
            CutBlend[] blend = CutBlend.GetCurrentBlendItems(macno);

            //ロットが見つからない場合は印字しない
            if (lot == null) return;
            Log.SysLog.Info("MAPカット前ラベル印字開始:" + lotno);

            try
            {
                Profile p = Profile.GetProfile(lot.ProfileId);

                //ロットNO,N1196MC0101,型番,NESW157A21L04,受入日,2011/06/06 13:30,数量,M000093,,OFF,95号機,,1,
                StringBuilder sb = new StringBuilder();
                sb.Append("ロットNO,");
                sb.Append(lotno);
                sb.Append(",型番,");
                sb.Append(lot.TypeCd);
                sb.Append(",受入日,");
                sb.Append(DateTime.Now);
                sb.Append(",数量,");
                sb.Append(magno);
                sb.Append(",,OFF");
                sb.Append("," + mac.MachineName);
                sb.Append(",," + blend.Length.ToString() + ",");

				string labelDir = Config.Settings.CutPreLabelDir;

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(labelDir, lotno + ".dat"), false, Encoding.UTF8))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("MAPカット前ラベル印字失敗:" + lotno + ex.ToString());
				throw ex;
            }
            Log.SysLog.Info("MAPカット前ラベル印字完了:" + lotno);
        }
        #endregion

		/// <summary>
		/// 車載高のカット前にライフ試験数ラベルを出力する機能
		/// </summary>
		public static void PrintGaPreCutLabel(AsmLot[] lots, string tempCutBlendLot) 
		{
			if (lots.Count() == 0) return;

			try
			{
				Log.SysLog.Info("GAカット前ライフラベル印字開始:" + tempCutBlendLot); //2015.10.7 車載CT特採対応。カットブレンドロットを事前に取得し、そのラベルを印刷する。

                //LotChar lifeCt = LotChar.GetLifeTestCt(lots[0].NascaLotNo);
                //if (lifeCt == null)
                //{
                //    throw new ApplicationException(string.Format("ライフ試験数のロット特性が存在しません。LotNo:{0}", lots[0].NascaLotNo));
                //}

                //全資材のロットを舐めてひとつもライフ試験が無ければエラーになるように変更。
                LotChar lifeCt = new LotChar();
                foreach (AsmLot lot in lots)
                {
                    LotChar tempLifeCt = LotChar.GetLifeTestCt(lot.NascaLotNo);
                    if (tempLifeCt != null || string.IsNullOrWhiteSpace(tempLifeCt.LotCharVal) == false)
                    {
                        lifeCt = tempLifeCt;
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(lifeCt.LotCharVal) == true)
                {
                    throw new ApplicationException(string.Format("ライフ試験数のロット特性が存在しません。LotNo:{0}", string.Join(",", lots.Select(l=>l.NascaLotNo))));
                }

                StringBuilder sb = new StringBuilder();

                //sb.Append("ロットNO,");
                //sb.Append(lots[0].NascaLotNo);
                //sb.Append(",型番,");
                //sb.Append(lots[0].TypeCd);
                //sb.Append(",受入日,");
                //sb.Append(DateTime.Now);
                //sb.Append(",ﾗｲﾌ試験数,");
                //sb.Append(lifeCt.LotCharVal);
                //sb.Append(string.Format(",,{0}\r\n", lifeCt.LotCharCd));

                List<LotChar> AllLotsCharValues = new List<LotChar>();
                List<Profile> AllLotsProfiles = new List<Profile>();
                foreach (AsmLot lot in lots)
                {
                    AllLotsCharValues.AddRange(LotChar.GetLotChar(lot.NascaLotNo).Where(l=>string.IsNullOrWhiteSpace(l.LotCharVal) == false).ToList());
                    AllLotsProfiles.Add(Profile.GetProfile(lot.ProfileId));
                }

                GnlMaster[] testLotChars = GnlMaster.GetTestSampleLotChar();
                foreach (GnlMaster lotChar in testLotChars)
                {
                    LotChar lotCharMax = AllLotsCharValues.Where(l => l.LotCharCd == lotChar.Code)
                                                          .OrderByDescending(l => Convert.ToInt32(l.LotCharVal))
                                                          .FirstOrDefault();
                    if (lotCharMax == null) continue;

                    if (lotChar.Code == Config.HV_TEST_CT_LOTCHAR_CD)
                    {
                        if(AllLotsProfiles.Any(p=>p.HasHvTest == true))
                        {
                            if(Convert.ToInt32(lotCharMax.LotCharVal) < Config.HV_TEST_CT)
                            {
                                lotCharMax.LotCharVal = Config.HV_TEST_CT.ToString();
                            }
                        }
                    }
                    else if(lotChar.Code == Config.DELTA_L_TEST_CT_LOTCHAR_CD)
                    {
                        if (AllLotsProfiles.Any(p => p.HasDeltaLTest == true))
                        {
                            if (Convert.ToInt32(lotCharMax.LotCharVal) < 1)
                            {
                                lotCharMax.LotCharVal = "1";
                            }
                        }
                    }

                    sb.Append(string.Format("ロットNO,{0},型番,{1},受入日,{2},{3},{4},,{5}\r\n",
                        tempCutBlendLot, lots[0].TypeCd,
                        DateTime.Now, lotChar.Val, lotCharMax.LotCharVal, lotChar.Code));
                }


				sb.Append("ロットNO,");
                sb.Append(tempCutBlendLot);
				sb.Append(",型番,");
				sb.Append(lots[0].TypeCd);
				sb.Append(",受入日,");
				sb.Append(DateTime.Now);
				sb.Append(",抜取残,,\r\n");

				foreach (AsmLot lot in lots) //2015.10.13 車載CT特殊対応。保管用ラべルもCTブレンドロットで印字。
				{
					sb.Append("ロットNO,");
                    sb.Append(tempCutBlendLot);
					sb.Append(",型番,");
					sb.Append(lot.TypeCd);
					sb.Append(",受入日,");
					sb.Append(DateTime.Now);
					sb.Append(",保管用,,\r\n");
				}

				string labelDir = CutLabelFile.GetDefaultOutputPathSetting().OutputPath;
				if (Directory.Exists(labelDir) == false)
				{
					Directory.CreateDirectory(labelDir);
				}

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(labelDir, tempCutBlendLot + ".dat"), false, Encoding.UTF8))
				{
					sw.Write(sb.ToString());
				}

				Log.SysLog.Info("GAカット前ライフラベル印字完了:" + lots[0]);
			}
			catch (Exception ex)
			{
				Log.SysLog.Error("GAカット前ライフラベル印字失敗:" + lots[0] + ex.ToString());
				throw ex;
			}
		}

        #region PrintCutLabel

        public static void PrintCutLabel(CutBlend[] blendlist, string blendlotno)
        {
            PrintCutLabel(blendlist, blendlotno, false);
        }

        /// <summary>
        /// カットラベル印字
        /// </summary>
        /// <param name="blendlist"></param>
        /// <param name="blendlotno"></param>
        public static void PrintCutLabel(CutBlend[] blendlist, string blendlotno, bool afterFinalStFg)
        {
            Log.SysLog.Info("カットブレンドラベル印字開始:" + blendlotno);

            try
            {
                int total, deftotal, testtotal;

                CalcTotal(blendlist, blendlotno, afterFinalStFg, out total, out deftotal, out testtotal);

                DateTime indt = blendlist.OrderBy(b => b.StartDt).FirstOrDefault().StartDt;
                AsmLot firstLot = AsmLot.GetAsmLot(blendlist[0].LotNo);

                string colortest = "OFF";
                if (firstLot.IsColorTest)
                {
                    colortest = "ON";
                }

                string typecd = firstLot.TypeCd;

                Profile p = Profile.GetProfile(firstLot.ProfileId);

                StringBuilder sb = new StringBuilder();

				//試験数ラベル
				GnlMaster[] lotChars = GnlMaster.GetTestSampleLotChar();
				foreach(GnlMaster lotChar in lotChars)
				{
					LotChar[] lotCharValues = LotChar.GetLotChar(blendlotno, lotChar.Code);
					foreach (LotChar lotCharValue in lotCharValues)
					{
						sb.Append(string.Format("ロットNO,{0},型番,{1},受入日,{2},{3},{4},,{5}\r\n",
							blendlotno, typecd, 
							indt.ToShortDateString(), lotChar.Val, lotCharValue.LotCharVal, lotChar.Code));
					}
				}

                sb.Append("ロットNO,");
                sb.Append(blendlotno);
                sb.Append(",型番,");
                sb.Append(typecd);
                sb.Append(",受入日,");
                sb.Append(indt.ToShortDateString());
                sb.Append(",数量,");
                sb.Append(total.ToString());
                sb.Append(",," + colortest);
                sb.Append("," + p.MnfctKb);
                sb.Append("," + p.TrialNo);

                //狙いランク内の半角カンマは全角に置換
                string aimrank = p.AimRank ?? "";
                aimrank = aimrank.Replace(',', '，');
                sb.Append("," + aimrank + ",");

                if (hasPrecedingLifeTest(firstLot.BeforeLifeTestCondCd, firstLot.IsColorTest))
                {
                    sb.Append(",1");
                }

                sb.Append("\r\n");

                sb.Append("ロットNO,");
                sb.Append(blendlotno);
                sb.Append(",型番,");
                sb.Append(typecd);
                sb.Append(",受入日,");
                sb.Append(indt.ToShortDateString());
                sb.Append(",抜取残,,");


                string labelDir;

				//2015.2.9 車載3次 出力先をマスタ設定場所へ(設定が無い場合、デフォルトへ)
				CutLabelFile cutLabelFile = CutLabelFile.GetOutputPathSetting(blendlist.First().MacNo);
				if (cutLabelFile == null)
				{
					labelDir = CutLabelFile.GetDefaultOutputPathSetting().OutputPath;
				}
				else
				{
					labelDir = cutLabelFile.OutputPath;
				}

				if (Directory.Exists(labelDir) == false)
				{
					Directory.CreateDirectory(labelDir);
				}

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(labelDir, blendlotno + ".dat"), false, Encoding.UTF8))
                {
                    sw.Write(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("カットブレンドラベル印字失敗:" + blendlotno + ex.ToString());
				throw ex;
            }
            Log.SysLog.Info("カットブレンドラベル印字完了:" + blendlotno);
        }

        /// <summary>
        /// 先行ライフ試験対象か確認
        /// </summary>
        private static bool hasPrecedingLifeTest(string preLifeTestValue, bool isPreColorTest)
        {
            // P0000205:先行ライフ試験条件の値　0,空白,NULL以外の値を持つ場合対象
            if (preLifeTestValue == "0" || string.IsNullOrEmpty(preLifeTestValue))
            {
                return false;
            }

            // P0000031:色調先行品の値　OFFと特性無しが対象
            if (isPreColorTest)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
    }
}
