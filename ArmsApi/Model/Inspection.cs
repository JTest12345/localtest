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
    public class Inspection
    {
        /// <summary>
        /// ダイシェア試験抜取グループ「不要」
        /// </summary>
        private const string DIE_SHARE_UNNECESSARY = "不要";

        /// <summary>
        /// ダイシェア試験抜取グループ「必須」
        /// </summary>
        private const string DIE_SHARE_NECESSARY = "必須";

        private string _lotno;
        /// <summary>
        /// ロット番号
        /// </summary>
        public string LotNo { get { return _lotno; } set { _lotno = Order.MagLotToNascaLot(value); } }

        /// <summary>
        /// 工程
        /// </summary>
        public int ProcNo { get; set; }

        private string procnm;
        public string ProcNm
        {
            get
            {
                if (string.IsNullOrEmpty(procnm))
                {
                    Process p = Process.GetProcess(ProcNo);
                    if (p != null)
                    {
                        this.procnm = p.InlineProNM;
                    }
                }
                return procnm;
            }
        }

        /// <summary>
        /// 検査済みフラグ
        /// </summary>
        public bool IsInspected { get; set; }

        #region GetInspection

        /// <summary>
        /// 検査必要工程情報取得
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <returns></returns>
        public static Inspection GetInspection(string lotno, int procno)
        {
            Inspection[] list = GetInspections(lotno);

            foreach (Inspection isp in list)
            {
                if (isp.ProcNo == procno)
                {
                    return isp;
                }
            }

            return null;
        }
        #endregion

        #region GetInspections


        public static Inspection[] GetInspections(string lotno)
        {
            //マガジン分割対応
            lotno = Order.MagLotToNascaLot(lotno);

            List<Inspection> retv = new List<Inspection>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {


                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;

                try
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT 
                          lotno , 
                          procno , 
                          sgyfg 
                        FROM 
                          TnInspection
                        WHERE 
                          lotno = @LOTNO";

                    cmd.CommandText = cmd.CommandText.Replace("\r\n", "");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            Inspection i = new Inspection();
                            i.LotNo = SQLite.ParseString(reader["lotno"]);
                            i.ProcNo = SQLite.ParseInt(reader["procno"]);
                            i.IsInspected = SQLite.ParseBool(reader["sgyfg"]);

                            retv.Add(i);
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

        #region 原材料抜き取り設定

        private class SamplingSetting
        {
            public string HMGP { get; set; }
            public int ProcNo { get; set; }
            /// <summary>
            /// ロット単位の抜き取りフラグ
            /// Falseの場合は同一ロットでも交換時・装置単位で抜き取り
            /// </summary>
            public bool IsEveryLot { get; set; }

			/// <summary>
			/// 検査が免除される工程NO
			/// </summary>
			public int? ExemptProcNo { get; set; }
        }

        /// <summary>
        /// 抜き取り検査フラグ操作
        /// </summary>
        /// <param name="ord"></param>
        /// <param name="lotno"></param>
        public static void Sampling(Order ord, string lotno)
        {
            if (!ord.IsComplete) return;

			Material[] matlist = new List<Material>().ToArray(); MachineInfo mac = null;
			if (ord.IsAutoImport)
			{
				// 高生産性ラインの取り込みで生成された実績は装置情報が無い為、
				// 取り込み時にTnMatRelationに資材を追加した後、それをここで取得する。
				matlist = ord.GetRelatedMaterials();
			}
			else
			{
				mac = MachineInfo.GetMachine(ord.MacNo);
				if (mac == null)
				{
					return;
				}

				matlist = mac.GetMaterials(ord.WorkStartDt, ord.WorkEndDt);

				// 2015.9.4 リードフレーム成型金型管理　(MAP)
				List<Material> macMatList = matlist.ToList();
				macMatList.AddRange(ord.GetRelatedMaterials());
				matlist = macMatList.ToArray();
			}

            SamplingSetting[] ss = getSettings();

            foreach (Material mat in matlist)
            {
                IEnumerable<SamplingSetting> found = ss.Where(s => s.HMGP == mat.HMGroup);
                if (found != null && found.Count() >= 1)
                {
                    AsmLot lot = AsmLot.GetAsmLot(lotno);
                    List<Process> workFlow = Process.GetWorkFlow(lot.TypeCd).ToList();

                    if (mat.IsFrame)
                    {
                        #region フレームのストッカー判定

                        //フレームの場合、混載は抜き取り対象にしないため、片方のストッカーの指図のみ対象
                        if (ord.ScNo1 == "1" && ord.ScNo2 == "1")
                        {
                            continue;
                        }
                        if (mat.StockerNo == 1)
                        {
                            if (ord.ScNo1 == "0") continue;
                        }
                        if (mat.StockerNo == 2)
                        {
                            if (ord.ScNo2 == "0") continue;
                        }

                        #endregion
                    }

                    if (mat.IsWafer)
                    {
                        throw new ArmsException("ウェハー抜き取り未対応");
                    }

                    bool isTimeSampled = false;
                    bool isLotSampled = false;

                    foreach (SamplingSetting setting in found)
                    {
                        if (!workFlow.Exists(w => w.ProcNo == setting.ProcNo))
                        {
                            //WorkFlowに登録されていない作業の場合は次へ
                            continue;
                        }

						// 2015.4.18 リードフレーム成型日管理機能追加
						if (hasFrameMoldedInspection(mat.MaterialCd, mat.FrameMoldedClass, mat.FrameMoldedNm, mat.FrameMoldedDt) == false) 
						{
							continue;
						}
						else
						{
							if (setting.IsEveryLot)
							{
								if (mat.IsLotSampled == true) continue;
								Inspection isp = new Inspection();
								isp.LotNo = lotno;
								isp.ProcNo = setting.ProcNo;
								isp.DeleteInsert();

								isLotSampled = true;

								lot = AsmLot.GetAsmLot(lotno);
								lot.IsChangePointLot = true;
								lot.Update();

								Log.SysLog.Info("[原材料検査抜き取り]" + lotno + ":" + mat.MaterialCd + ":" + mat.LotNo);
							}
							else
							{
								if (mat.IsTimeSampled == true) continue;
								Inspection isp = new Inspection();
								isp.LotNo = lotno;
								isp.ProcNo = setting.ProcNo;
								isp.DeleteInsert();

								isTimeSampled = true;

								lot = AsmLot.GetAsmLot(lotno);
								lot.IsChangePointLot = true;
								lot.Update();

								//Log.SysLog.Info("[原材料検査抜き取り]" + lotno + ":" + mat.MaterialCd + ":" + mat.LotNo + ":" + mac.MacNo + ":" + mat.InputDt);
								Log.SysLog.Info("[原材料検査抜き取り]" + lotno + ":" + mat.MaterialCd + ":" + mat.LotNo + ":" + mat.InputDt);
							}

							// 2015.9.4 リードフレーム成型金型管理　(MAP)
							// 免除される検査設定がされている場合、その工程を検査済みへ
							if (setting.ExemptProcNo.HasValue && ord.ProcNo == setting.ExemptProcNo.Value)
							{
								Inspection isp = Inspection.GetInspection(lotno, ord.ProcNo);
								if (isp != null)
								{
									isp.IsInspected = true;
									isp.DeleteInsert();
								}
							}
						}
                    }
					
                    if (mac != null && isTimeSampled)
                    {
                        mat.IsTimeSampled = true;
                        mac.DeleteInsertMacMat(mat);
                    }

                    if (isLotSampled)
                    {
                        UpdateLotSampled(mat.MaterialCd, mat.LotNo);

						// 2015.4.18 リードフレーム成型日管理機能追加
						if (mat.FrameMoldedDt.Count == 1 && mat.FrameMoldedClass == "1")
						{
							InsertFrameMoldedInspectionResult(mat.MaterialCd, mat.FrameMoldedNm, mat.FrameMoldedDt.Single());
						}
                    }
                }
            }
        }

        /// <summary>
        /// 抜き取り設定取得
        /// </summary>
        /// <returns></returns>
        private static SamplingSetting[] getSettings()
        {
            List<SamplingSetting> retv = new List<SamplingSetting>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
				cmd.CommandText = @" SELECT hmgroup, procno, everylotfg, exemptprocno
                                     FROM TmMatSampling with(nolock) ";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
					int ordExemptProcNo = rd.GetOrdinal("exemptprocno");
                    while (rd.Read())
                    {
                        SamplingSetting s = new SamplingSetting();
                        s.HMGP = SQLite.ParseString(rd["hmgroup"]);
                        s.ProcNo = SQLite.ParseInt(rd["procno"]);
                        s.IsEveryLot = SQLite.ParseBool(rd["everylotfg"]);

						if (rd.IsDBNull(ordExemptProcNo) == false) 
						{
							s.ExemptProcNo = Convert.ToInt32(rd[ordExemptProcNo]);
							//s.ExemptProcNo = rd.GetInt32(ordExemptProcNo);
						}				

                        retv.Add(s);
                    }
                }
            }

            return retv.ToArray();
        }

        public static void UpdateLotSampled(string matcd, string lotno)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.Parameters.Add("@MATCD", System.Data.SqlDbType.NVarChar).Value = matcd;
                    cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.NVarChar).Value = lotno;

                    cmd.CommandText = "UPDATE TnMaterials SET issampled=1 WHERE materialcd=@MATCD AND lotno=@LOTNO";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("抜き取り検査フラグ更新失敗:" + matcd + ":" + lotno, ex);
                }
            }
        }

		public static bool hasFrameMoldedInspection(string matcd, string framemoldedclass, string framemoldednm, List<DateTime> framemoldeddt)
		{
			if (string.IsNullOrEmpty(framemoldednm) || framemoldeddt.Count == 0
				|| Config.Settings.FrameMoldedInspectionClass.Exists(c => c == framemoldedclass) == false)
			{
				//管理対象外の場合、検査履歴は未参照のまま検査へ
				return true;
			}

			List<bool> result = new List<bool>();

			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

                SqlParameter paramMatCd = cmd.Parameters.Add("@MaterialCd", SqlDbType.NVarChar);
                SqlParameter paramMoldedNm = cmd.Parameters.Add("@FrameMoldedNm", SqlDbType.NVarChar);
                SqlParameter paramMoldedDt = cmd.Parameters.Add("@FrameMoldedDt", SqlDbType.DateTime);

				foreach (DateTime dt in framemoldeddt)
				{
					cmd.CommandText = @" SELECT * FROM TnFrameInspectResult WITH(nolock) 
							WHERE materialcd = @MaterialCd AND framemoldednm = @FrameMoldedNm AND framemoldeddt = @FrameMoldedDt ";

					paramMatCd.Value = matcd;
					paramMoldedNm.Value = framemoldednm;
					paramMoldedDt.Value = dt.ToString("yyyy/MM/dd");

					if (cmd.ExecuteScalar() == null)
					{
						result.Add(true);
					}
					else 
					{
						result.Add(false);
					}
				}
			}

			//全ての成型日で検査履歴が無かった場合のみ未検査(false)
			if (result.Count(r => r == false) == result.Count)
			{
				return false;
			}
			else 
			{
				return true;
			}
		}

		public static void InsertFrameMoldedInspectionResult(string matcd, string framemoldednm, DateTime framemoldeddt) 
		{
			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				cmd.CommandText = @" INSERT INTO TnFrameInspectResult(materialcd, framemoldednm, framemoldeddt) VALUES
										(@MaterialCd, @FrameMoldedNm, @FrameMoldedDt) ";

				cmd.Parameters.Add("@MaterialCd", SqlDbType.NVarChar).Value = matcd;
				cmd.Parameters.Add("@FrameMoldedNm", SqlDbType.NVarChar).Value = framemoldednm;
				cmd.Parameters.Add("@FrameMoldedDt", SqlDbType.DateTime).Value = framemoldeddt.ToString("yyyy/MM/dd");

				cmd.ExecuteNonQuery();
			}
		}

        #endregion

        #region 2ロット中、1ロット抜き取り設定

        public static void OneOfTwoSampling(Order order)
        {
            Order lastOrder = Order.GetLastOrderInMachine(order.MacNo, order.ProcNo);
            if (lastOrder == null)
            {
                //最終完成ロットが存在しない場合は、検査設定
                Inspection isp = new Inspection();
                isp.LotNo = order.LotNo;
                isp.ProcNo = order.ProcNo;
                isp.DeleteInsert();
            }
            else 
            {
                Inspection lastOrderIsp = Inspection.GetInspection(lastOrder.LotNo, lastOrder.ProcNo);
                if (lastOrderIsp == null) 
                {
                    //最終完成ロットの検査設定が存在しない場合は、現ロットに検査設定
                    Inspection isp = new Inspection();
                    isp.LotNo = order.LotNo;
                    isp.ProcNo = order.ProcNo;
                    isp.DeleteInsert();
                }
            }
        }

        #endregion

		#region ダイシェア抜き取り設定

		/// <summary>
		/// ダイシェア抜き取り設定
		/// </summary>
		/// <param name="order"></param>
		public static bool DieShearSampling(Order order, Magazine mag) 
		{
			AsmLot checklot = AsmLot.GetAsmLot(order.NascaLotNo);

			// ロットがダイシェア抜き取り対象の型番か確認
			List<string> typeList = new List<string>();
			if (Config.Settings.IsDieShearSamplingFromPsTesterType)
			{
				typeList = GnlMaster.GetDieshearSampleType().Select(t => t.Code).ToList();
			}
			else 
			{
				typeList = Process.GetWorkFlowTypeCd().ToList();
			}

			if (typeList.Count == 0) 
			{
				return false;
			}
			if (typeList.Where(t => t == checklot.TypeCd).Count() == 0) 
			{
				return false;
			}

            string dieShearSamplingJudgeWorkCd = getDieShearSamplingJudgeWorkCd(checklot);
            Process[] judgeProcList = Process.GetProcessFromWorkCd(dieShearSamplingJudgeWorkCd);

			Order judgeOrder = null;
			foreach (Process judgeProc in judgeProcList)
			{
				// ロットのダイシェア抜き取り判定材料にする実績を取得
				Order o = Order.GetMagazineOrder(order.LotNo, judgeProc.ProcNo);
				if (o == null) continue;
				if (judgeOrder != null && judgeOrder.WorkEndDt < o.WorkEndDt) continue;

				judgeOrder = o;
			}
			if (judgeOrder == null)
			{
				throw new Exception(
					string.Format("ダイシェア抜き取り設定中、判定に必要な工程実績の取得に失敗。LotNo:{0} 実績作業CD:{1}", order.LotNo, dieShearSamplingJudgeWorkCd));
			}

            if (Config.Settings.IsDieShearSamplingEachMachineDay)
            {
                //抜取頻度：ZD(RED)ダイボンダー1台・1日1ロット＋仕様変更毎に1ロット

                DateTime form = Convert.ToDateTime(judgeOrder.WorkStartDt.ToShortDateString());

                List<AsmLot> lotList = new List<AsmLot>();
                Order[] hisMacOrders = Order.GetMachineOrderStart(judgeOrder.MacNo, form, form.AddDays(1).AddMilliseconds(-1));

                List<Process> proclist = Process.SearchProcess(null, dieShearSamplingJudgeWorkCd, null, false).ToList();

                foreach (Order his in hisMacOrders)
                {
                    if (!proclist.Exists(p => p.ProcNo == his.ProcNo))
                    {
                        continue;
                    }

                    AsmLot targetlot = AsmLot.GetAsmLot(his.NascaLotNo);
                    lotList.Add(targetlot);
                }

                if (checklot.DieShareSamplingPriority == DIE_SHARE_NECESSARY)
                {
                    //ダイシェア試験抜取グループに”必須”を持っている。 ⇒ 全て試験必要
                }
                else
                {
                    //ダイシェア試験抜取グループに”不要”を持っている。 ⇒ 判定対象外としてスルー。
                    if (checklot.DieShareSamplingPriority == DIE_SHARE_UNNECESSARY)
                    {
                        return false;
                    }

                    if (!string.IsNullOrWhiteSpace(checklot.DieShareSamplingPriority))
                    {
                        //ダイシェア試験抜取グループに”不要”以外の特性を持っている。
                        //⇒抜取グループでロット一覧を絞り込み、既に規制ずみのロットが無ければ現ロットを対象とする。
                        lotList = lotList.FindAll(l => l.DieShareSamplingPriority == checklot.DieShareSamplingPriority);

                        foreach (AsmLot his in lotList)
                        {
                            // ダイシェア代表ロットフラグがOnが1ロットでもあればダイシェア代表ロットフラグ付与対象を探す必要なし
                            if (his.IsDieShearLot)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //ダイシェア試験抜取グループを持っていない。
                        //⇒型番でロット一覧を絞り込み、既に規制ずみのロットが無ければ現ロットを対象とする。
                        foreach (AsmLot his in lotList)
                        {
                            if (checklot.TypeCd != his.TypeCd) continue;

                            // ダイシェア代表ロットフラグがOnが1ロットでもあればダイシェア代表ロットフラグ付与対象を探す必要なし
                            if (his.IsDieShearLot)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                //抜取頻度：オーブン1バッチ1ロット

                // 判定材料にする実績から±1分の同型番、同装置の実績を取得する
                Order[] hisMacOrders = Order.GetMachineOrder(judgeOrder.MacNo, judgeOrder.WorkEndDt.Value.AddMinutes(-1), judgeOrder.WorkEndDt.Value.AddMinutes(1));
                foreach (Order his in hisMacOrders)
                {
                    AsmLot targetlot = AsmLot.GetAsmLot(his.NascaLotNo);

                    if (Config.Settings.IsDieShearSamplingFromPsTesterType)
                    {
                        if (checklot.TypeCd != targetlot.TypeCd) continue;
                    }

                    // ダイシェア代表ロットフラグがOnが1ロットでもあればダイシェア代表ロットフラグ付与対象を探す必要なし
                    if (targetlot.IsDieShearLot)
                    {
                        return false;
                    }
                }
            }

            // ここまで処理が流れてきて全ロットで前述フラグOffなら、前述フラグOnとPSTester用ファイル出力、移動規制、ダイシェア対象ロットのメッセージ出力

            //ダイシェア代表ロット対象フラグ付与
            checklot.IsDieShearLot = true;
            checklot.Update();

            //移動規制
            Process restrictProc = Process.GetNextProcess(order.ProcNo, checklot);
            //Process restrictProc = Process.GetProcess(Restrict.INSPECT_WORKCD);
            SetRistrict(restrictProc.ProcNo, checklot.NascaLotNo, Restrict.RESTRICT_REASON_DIESHEARSAMPLE);

            //PSTester用ファイル出力 現仕様Magazineからでないと呼び出せないので見直した方がいいか… n.yoshimoto 2015/1/15
            mag.WritePSTesterFile();

			return true;
		}

        #endregion

        #region 強度試験設定
        public static void PSTesterSampling(Order order, Magazine mag)
        {
            AsmLot checkLot = AsmLot.GetAsmLot(order.NascaLotNo);
            MachineInfo m = MachineInfo.GetMachine(order.MacNo);

            // 設定時間
            decimal? settingtime = PSTeserMaster.GetSettingTime(m.NascaPlantCd);
            if (settingtime.HasValue == false)
            {
                //設定時間がマスタ登録されていない場合は、0時を設定時間とする
                settingtime = 0;
            }

            Process regulationProc = Process.GetNextProcess(Config.Settings.SetPSTesterProcNo.Value, checkLot);

            //既に規制がかかっている場合は何もしない
            if (Restrict.GetPSTesterRestrictLot(order.NascaLotNo, regulationProc.ProcNo, false) != null ||
                Restrict.GetPSTesterSpreadLot(order.NascaLotNo, regulationProc.ProcNo) != null)
            {
                return;
            }

            //既に強度試験対象に選ばれている場合は何もしない(装置問わず)
            if (AGVPSTester.ExistTestLot(order.NascaLotNo, null) == true)
            {
                return;
            }

            //製造区分が量産以外は何もしない
            Profile prof = Profile.GetProfile(checkLot.ProfileId);
            if (prof == null ||
                string.IsNullOrWhiteSpace(prof.MnfctKb) == true ||
                prof.MnfctKb.Trim() == "初期流動品" ||
                prof.MnfctKb.Trim() == "量産試作" ||
                prof.MnfctKb.Trim() == "初期流動" ||
                prof.MnfctKb.Trim() == "試作")
            {
                Log.SysLog.Info($"[強度試験設定処理] ロット：{checkLot.NascaLotNo}の製造区分は量産ではないため設定対象外。製造区分：{prof.MnfctKb}");
                return;
            }

            // 当日に対象装置での試験対象があるか確認
            List<string> todayTestLot = AGVPSTester.GetTestLot(m.NascaPlantCd, DateTime.Today, DateTime.Today.AddDays(1));

            // 前日に対象装置での試験対象があるか確認
            List<string> previousdayTestLot = AGVPSTester.GetTestLot(m.NascaPlantCd, DateTime.Today.AddDays(-1), DateTime.Today);
            if (previousdayTestLot.Count == 0)
            {
                //前日に対象装置への投入がなかった場合、翌日の先頭ロットが強度対象となる
                settingtime = 0;
            }

            DateTime testStartDt = DateTime.Today.AddHours((double)settingtime);

            if (todayTestLot.Count == 0 && order.WorkStartDt >= testStartDt)
            {
                Log.SysLog.Info($"[強度試験対象ロット] ロット：{checkLot.NascaLotNo}, テスト開始時間：{testStartDt.ToString()}");

                // 強度試験対象ロットの為、流動規制
                Restrict.RegulationPSTester(checkLot.NascaLotNo, checkLot.NascaLotNo, regulationProc.ProcNo, false);

                //強度試験情報テーブル（TnPSTester）へのレコードを追加
                AGVPSTester.Insert(m.NascaPlantCd, order.WorkStartDt, checkLot.NascaLotNo);

                //強度データ出力
                mag.WritePSTesterFile();
            }
            else
            {
                //最新の代表ロットを取得
                string recentlyTestLot = AGVPSTester.GetTestLot(m.NascaPlantCd);
                if (string.IsNullOrWhiteSpace(recentlyTestLot) == false)
                {
                    // 開始登録するロットが波及ロットの場合
                    Restrict restrictLot = Restrict.GetPSTesterRestrictLot(recentlyTestLot, regulationProc.ProcNo, false);
                    if (restrictLot == null ||
                        (restrictLot != null && restrictLot.DelFg))
                    {
                        // 波及ロットだが、既に代表ロットが解除済みの場合は解除状態で追加
                        Restrict.SpreadLotPSTester(checkLot.NascaLotNo, recentlyTestLot, regulationProc.ProcNo, true);
                    }
                    else
                    {
                        Log.SysLog.Info($"[強度試験波及ロット] ロット：{checkLot.NascaLotNo}, 試験対象ロット：{recentlyTestLot}");

                        // 波及ロットの為、流動規制
                        Restrict.SpreadLotPSTester(checkLot.NascaLotNo, recentlyTestLot, regulationProc.ProcNo, false);
                    }
                }
            }
        }
        #endregion

        // // 3in1対応 n.yoshimoto 2015/1/16
        // WB.MMファイルクラスの中にあり、そのインスタンスのLotNoを暗黙で使用するようになっていたので
        // 引数渡し出来る関数にして、WB.MMファイルクラスから出した…出した際に移動先が分からなかったのでInspection.csにしてしまったが
        // 適切な場所は追々という事でWB.MMの中でもとりあえずは良かったかも…
        /// <summary>
        /// 移動規制
        /// </summary>
        /// <param name="procno"></param>
        /// <param name="lotno"></param>
        /// <param name="reason"></param>
        public static void SetRistrict(int procno, string lotno, string reason)
		{
			Restrict r = new Restrict();
			r.LotNo = lotno;
			r.ProcNo = procno;
			r.Reason = reason;
			r.DelFg = false;
			r.Save();

			Log.ApiLog.Info(string.Format("流動規制設定:{0}:{1} 理由:{2}", lotno, procno.ToString(), reason));
		}

		public void DeleteInsert()
        {
            DeleteInsert(SQLite.ConStr);
        }

        public void DeleteInsert(string constr)
        {
            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                    cmd.Parameters.Add("@PROC", SqlDbType.BigInt).Value = this.ProcNo;
                    cmd.Parameters.Add("@SGYFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IsInspected);
                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;


                    //前履歴は削除
                    cmd.CommandText = "DELETE FROM TnInspection WHERE lotno=@LOTNO AND procno=@PROC";
                    cmd.ExecuteNonQuery();

                    //新規Insert
                    cmd.CommandText = @"
                            INSERT
                             INTO TnInspection(lotno
	                            , procno
	                            , sgyfg
	                            , lastupddt)
                            values(@LOTNO
	                            , @PROC
                                , @SGYFG
	                            , @UPDDT )";

                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("抜き取り検査設定エラー:" + this.LotNo, ex);
                }
            }
        }


        /// <summary>
        /// 抜取り検査設定自体を削除
        /// 通常は完了フラグONで更新を行うので、
        /// このメソッドは検査機の投入フラグ自体を消す必要がある場合のみ。
        /// </summary>
        public void Delete()
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                    cmd.Parameters.Add("@PROC", SqlDbType.BigInt).Value = this.ProcNo;

                    cmd.CommandText = "DELETE FROM TnInspection WHERE lotno=@LOTNO AND procno=@PROC";
                    cmd.ExecuteNonQuery();

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new ArmsException("抜き取り検査設定エラー:" + this.LotNo, ex);
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
            Inspection[] inspections = Inspection.GetInspections(lotno);
            
            string sql = " DELETE FROM TnInspection WHERE lotno Like @LOTNO ";
            cmd.CommandText = sql;

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno + "%";

            try
            {
                cmd.ExecuteNonQuery();

                foreach(Inspection inspection in inspections)
                {
                    Log.DelLog.Info(string.Format("[TnInspection] {0}\t{1}\t{2}\t{3}",
                        inspection.LotNo, inspection.ProcNo, inspection.IsInspected, System.DateTime.Now));
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("マガジン削除エラー:" + lotno, ex);
            }
        }

        public static string getDieShearSamplingJudgeWorkCd(AsmLot lot)
        {
            List<Process> workflow = Process.GetWorkFlow(lot.TypeCd).ToList();

            // 取得① タイプ - ダイシェア抜き取り判定工程マスタから取得
            ArmsApi.Model.DieShearSampling dss = ArmsApi.Model.DieShearSampling.getDieShearSampling(lot.TypeCd);
            if(dss != null && dss.TypeCd == lot.TypeCd)
            {
                // マスタにタイプのレコードがあれば、それを返す。
                if (workflow.Exists(w => w.WorkCd == dss.JudgeWorkCd) == false)
                {
                    throw new Exception($"作業フローにダイシェア抜き取り判定工程がありません。タイプ:{lot.TypeCd}, TmDieShearSampling登録工程:{dss.JudgeWorkCd}");
                }
                return dss.JudgeWorkCd;
            }

            // 取得② 全タイプ共通の作業CDの優先度リストから取得
            GnlMaster[] dieshearSampleJudgeWorkCdPriorytyList = GnlMaster.GetDieshearSampleJudgeWorkCd();
            int priority;
            // TmGeneralのCodeに文字が含まれているレコードは無視する
            foreach (GnlMaster m in dieshearSampleJudgeWorkCdPriorytyList
                                        .Where(d => int.TryParse(d.Code, out priority))
                                        .OrderBy(d => int.Parse(d.Code)))
            {
                // 作業フローに存在する作業CDがあれば、それVal2に指定した作業CDを返す。
                if(workflow.Exists(w => w.WorkCd == m.Val))
                {
                    if(workflow.Exists(w => w.WorkCd == m.Val2) == false)
                    {
                        throw new Exception($"作業フローにダイシェア抜き取り判定工程がありません。タイプ:{lot.TypeCd}, 作業フローに含む工程：{m.Val}, 汎用区分：{GnlMaster.TID_DIESHEARSAMPLEJUDGEWORKCD}, 汎用マスタ登録工程:{m.Val2}");
                    }
                    return m.Val2;
                }
            }            

            // ここまでの処理で該当なしの場合は、これまで通りArmsConfigの作業CDを返す
            return Config.Settings.DieShearSamplingJudgeWorkCd;
        }
    }
}
