using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using ArmsApi;
using ArmsApi.Model.NASCA;

namespace ArmsApi
{
    public class CommonApi
    {
        //parseFailJobは直接Defect呼び出しに変更
        //parseMAPFrameLoadOrderは廃止
        //WorkStartEndのAPIは廃止

        #region [NEL] 次作業問合せAPI QueryNextWork

        /// <summary>
        /// 次作業問合せAPI
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static ArmsApiResponse QueryNextWork(Order order, VirtualMag vMag)
        {
            Magazine mag = Magazine.GetCurrent(order.OutMagazineNo);
            if (mag == null)
            {
                return ArmsApiResponse.Create(true, "マガジン情報が存在しません", order.ProcNo, order.OutMagazineNo, order.MacNo, order.LineNo);
            }
            AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
            if (lot == null)
            {
                return ArmsApiResponse.Create(true, "マガジンの入力が誤っています:" + mag.MagazineNo, order.ProcNo, order.OutMagazineNo, order.MacNo, order.LineNo);
            }

            ArmsApiResponse retv = new ArmsApiResponse();

            //出荷規制判定（現在完了工程が規制工程なら排出行き）
            string reason;
            if (WorkChecker.IsRestricted(lot.NascaLotNo, mag.NowCompProcess, out reason))
            {
                List<int> machines = new List<int>();
                machines.Add(Route.GetDischargeConveyor(order.MacNo));
                Log.ApiLog.Info(string.Format("[排出]マガジン番号:{0} {1}", mag.MagazineNo, reason));

                retv.ProcNo = order.ProcNo;
                retv.NextMachines = machines;
                retv.Message = reason;
                return retv;
            }

            //現在完了工程が完成品排出CVに搬送する作業の場合
            Process nowCompProcess = Process.GetProcess(mag.NowCompProcess);
            if (nowCompProcess.WorkCd == Config.Settings.MastDoCompltDischargeWorkCd)
            {
                List<int> machines = new List<int>();
                machines.Add(Route.GetCompltDischargeConveyor(order.MacNo));
                Log.ApiLog.Info(string.Format("[完成品排出]マガジン番号:{0}", mag.MagazineNo));

                retv.ProcNo = mag.NowCompProcess;
                retv.NextMachines = machines;
                return retv;
            }

            Process nextproc = Process.GetNextProcess(mag.NowCompProcess, lot);
            // 最終工程の場合
            if (nextproc == null)
            {
                retv.ProcNo = order.ProcNo;
                // 自動搬送ライン装置の場合は、次装置は排出CVから選ぶ
                MachineInfo mac = MachineInfo.GetMachine(retv.MacNo.Value);
                if (mac != null && mac.IsAutoLine)
                {
                    retv.NextMachines.Add(Route.GetDischargeConveyor(order.MacNo));
                }
                return retv;
            }

            //次工程が抜き取り検査工程の場合
            if (nextproc.IsSamplingInspection == true)
            {
                //検査工程の抜き取り指示有無再取得
                Inspection isp = Inspection.GetInspection(lot.NascaLotNo, nextproc.ProcNo);

                //抜き取りフラグがある場合は、検査有無に関わらず検査工程へ
                //作業履歴削除時にも再度検査されるように
                if (isp == null)
                {
                    //抜き取りフラグが無い場合は、ダミー作業を登録
                    Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag);
                    nextproc = Process.GetNextProcess(nextproc.ProcNo, lot);
                }
            }

            // サーバー統合対応 MEL_MAP ⇒ NEL_MAP対応  装置のhighlineフラグも参照する
            MachineInfo macInfo = MachineInfo.GetMachine(order.MacNo);

            // 次工程が移載前待機、MAP高生産性ラインの場合、ダミー登録
            //if (Process.HasMagazineChangeBufferWork(nextproc.WorkCd) && Config.GetLineType == Config.LineTypes.MEL_MAP)
            if (Process.HasMagazineChangeBufferWork(nextproc.WorkCd) 
                && (Config.GetLineType == Config.LineTypes.MEL_MAP || (Config.GetLineType == Config.LineTypes.NEL_MAP && macInfo.IsHighLine == true)))
            {
                Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag);
				nextproc = Process.GetNextProcess(nextproc.ProcNo, lot);
			}

            // 次工程がマガジン交換作業、MAP高生産性ライン、作業順に遠心沈降作業が無い場合、ダミー登録
            //if (Process.HasMagazineChangeWork(nextproc.WorkCd) && Config.GetLineType == Config.LineTypes.MEL_MAP)
            if (Process.HasMagazineChangeWork(nextproc.WorkCd) 
                && (Config.GetLineType == Config.LineTypes.MEL_MAP || (Config.GetLineType == Config.LineTypes.NEL_MAP && macInfo.IsHighLine == true)))
            {
                List<Process> workflow = Process.GetWorkFlow(lot.TypeCd).ToList();
                if (Config.Settings.EckWorkCdList != null && workflow.Exists(w => Config.Settings.EckWorkCdList.Contains(w.WorkCd)) == false)
                {
					Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag);
					nextproc = Process.GetNextProcess(nextproc.ProcNo, lot);
				}
			}

            // 次工程がロットマーキング作業、先行色調品、作業マスタにダミー設備マスタが登録されている場合、ダミー登録
            if (nextproc.IsLotMarking)
            {
                if (nextproc.AutoUpdMachineNo.HasValue)
                {
                    Profile prof = Profile.GetProfile(lot.ProfileId);
                    if (prof != null && prof.ProfileNm.Contains("先行色調"))
                    {
                        Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag);
                        nextproc = Process.GetNextProcess(nextproc.ProcNo, lot);
                    }
                }
            }

            // 次工程がワークフロー上のダミー実績登録可作業の場合、ダミー登録判定OKなら工程を進める (このタイミングでのダミー登録はなし)
            if (Process.GetAutoDummyTran(lot.TypeCd, nextproc.ProcNo) == Process.AutoDummyTran.DummyTran)
            {
                // ダミー判定登録OK
                Process nextproc2 = Process.GetNextProcess(nextproc.ProcNo, lot);
                if (nextproc2 != null && WorkChecker.CanDummyAutoTran(lot, nextproc2.ProcNo) == true)
                {
                    nextproc = nextproc2;
                }
            }

            //ダミー登録後の完了工程が完成品排出CVに搬送する作業の場合
            nowCompProcess = Process.GetProcess(mag.NowCompProcess);
            if (nowCompProcess.WorkCd == Config.Settings.MastDoCompltDischargeWorkCd)
            {
                List<int> machines = new List<int>();
                machines.Add(Route.GetCompltDischargeConveyor(order.MacNo));
                Log.ApiLog.Info(string.Format("[完成品排出]マガジン番号:{0}", mag.MagazineNo));

                retv.ProcNo = mag.NowCompProcess;
                retv.NextMachines = machines;
                return retv;
            }

            //車載内装自動搬送は搭載→DB搬送時にプロファイル切り替わりがある為、事前チェックできない
            if (Config.Settings.IsDBProfileRequireMode == true)
            {
                //if (nextproc == null)
                //{
                //    retv.ProcNo = order.ProcNo;
                //    retv.NextMachines.Add(Route.GetDischargeConveyor(order.MacNo));
                //}
                //else
                //{
                MachineInfo[] nextMachines = MachineInfo.GetAvailableMachines(nextproc.ProcNo, lot.MacGroup);
                retv.ProcNo = nextproc.ProcNo;
                retv.NextMachines = nextMachines.Select(m => m.MacNo).Distinct().ToList();
                //}
            }
            else
            {
                if (nextproc == null)
                {
                    retv.ProcNo = order.ProcNo;
                    retv.NextMachines.AddRange(Route.GetDischargeConveyors(order.MacNo));
                }
                else
                {
                    retv.ProcNo = nextproc.ProcNo;
                    vMag.ProcNo = nextproc.ProcNo;

                    if (string.IsNullOrEmpty(vMag.PurgeReason) == false)
                    {
                        retv.NextMachines.AddRange(Route.GetDischargeConveyors(order.MacNo));
                        return retv;
                    }

                    bool isOtherNextMachine = false;
                    List<MachineInfo> otherNextMachines = new List<MachineInfo>();

                    CarrierInfo fromCarrier = Route.GetReachable(new Location(order.MacNo, Station.Loader));

                    MachineInfo[] nextMachines = MachineInfo.GetAvailableMachines(nextproc.ProcNo, lot.MacGroup);
                    foreach (MachineInfo m in nextMachines)
                    {
                        CarrierInfo toCarrier = Route.GetReachable(new Location(m.MacNo, Station.Loader));
                        if (fromCarrier != null && toCarrier != null)
                        {
                            if (fromCarrier.CarNo != toCarrier.CarNo)
                            {
                                isOtherNextMachine = true;                                
                            }
                        }

                        Order startOrder = CommonApi.GetWorkStartOrder(vMag, m.MacNo);
                        startOrder.LotNo = mag.NascaLotNO;

                        string msg = string.Empty;
                        if (WorkChecker.IsErrorBeforeStartWork(lot, m, startOrder, nextproc, out msg))
                        {
                            string purgeReason = string.Format("【作業開始前照合NGの為、供給対象から除外】装置：{0} マガジン番号:{1} 理由：{2}", m.LongName, mag.MagazineNo, msg);
                            Log.SysLog.Info(purgeReason);

                            continue;
                        }

                        retv.NextMachines.Add(m.MacNo);

                        //要求信号確認に使用(AGV使用品種の場合のみ)
                        if (fromCarrier != null && toCarrier != null)
                        {
                            if (fromCarrier.CarNo != toCarrier.CarNo)
                            {
                                otherNextMachines.Add(m);
                            }
                        }
                    }

                    if (retv.NextMachines.Count == 0)
                    {
                        List<int> agvLoadConveyor = Route.GetAgvLoadConveyors(order.MacNo);
                        if (agvLoadConveyor.Exists(a => a == order.MacNo))
                        {
                            // 搬送元がAGV投入CVの場合、作業開始前チェックで全ての装置がNGは異常品CVに排出する
                            vMag.PurgeReason = "次搬送する全装置で作業開始前チェックがNGだった為(状態検査未完了、型番照合NG等)";
                            retv.NextMachines.AddRange(Route.GetDischargeConveyors(order.MacNo));
                        }

                        if (isOtherNextMachine)
                        {
                            //AGV使用品種の場合、他ラインの装置の要求信号を確認する
                            if (agvLoadConveyor != null && agvLoadConveyor.Count != 0)
                            {
                                // データベースから供給要求リストを取得
                                List<Model.LAMS.MachineRequire> reqAllMachines = Model.LAMS.MachineRequire.GetMachineRequire(otherNextMachines.Select(a => a.NascaPlantCd).ToList()).ToList();

                                if (reqAllMachines.Count == 0)
                                {
                                    //要求が無い場合は排出CVへ
                                    List<int> machines = new List<int>();
                                    machines.Add(Route.GetDischargeConveyor(order.MacNo));
                                    retv.NextMachines = machines;
                                }
                                else
                                {
                                    // 作業開始前判定前の次装置に1つでも他ラインの装置が入っている場合、完成品CVを最終装置として追加
                                    retv.NextMachines.Add(Route.GetAutoLineOutConveyor(order.MacNo));
                                }
                            }
                            else
                            {
                                // 作業開始前判定前の次装置に1つでも他ラインの装置が入っている場合、完成品CVを最終装置として追加
                                retv.NextMachines.Add(Route.GetAutoLineOutConveyor(order.MacNo));
                            }
                        }
                        else
                        {
                            // 作業開始前チェックで全ての装置がNGだった場合、異常品CVへ排出する(状態検査未完了、次装置で型番照合NGなど)
                            vMag.PurgeReason = "次搬送する全装置で作業開始前チェックがNGだった為(状態検査未完了、型番照合NG等)";
                            retv.NextMachines.AddRange(Route.GetDischargeConveyors(order.MacNo));
                        }
                    }
                }
            }
            return retv;
        }
        #endregion

        #region [NEL] 作業開始API　WorkStart

        /// <summary>
        /// 作業開始API
        /// </summary>
        /// <param name="startInfo"></param>
        /// <returns></returns>
        public static ArmsApiResponse WorkStart(Order order)
        {
            // ログ出力（処理開始）
            Log.ApiLog.Info("[api] EditWorkStart start:" + order.InMagazineNo);

            try
            {
                // 各マスタ取得
                MachineInfo machine = MachineInfo.GetMachine(order.MacNo);
                if (machine == null)
                {
                    return ArmsApiResponse.Create(true, "設備マスタに不備があります", order.ProcNo, order.InMagazineNo, order.MacNo, order.LineNo);
                }

                Process process = Process.GetProcess(order.ProcNo);
                if (machine == null)
                {
                    return ArmsApiResponse.Create(true, "工程マスタに不備があります", order.ProcNo, order.InMagazineNo, order.MacNo, order.LineNo);
                }

                #region マガジン内容(インライン装置側の起因で排出)の値があればログ出力を行い処理を終了させる
                if (AsmLot.HasErrorStatus(order))
                {
                    return ArmsApiResponse.Create(true, "装置でエラーが発生して排出されました", order.ProcNo, order.InMagazineNo, order.MacNo, order.LineNo);
                }
                #endregion

                #region 整合チェック

                // マガジンNO
                if (string.IsNullOrEmpty(order.InMagazineNo))
                {
                    return ArmsApiResponse.Create(true, "NW0286:マガジンの入力が誤っています", order.ProcNo, order.InMagazineNo, order.MacNo, order.LineNo);
                }

                // 時間
                if (order.WorkStartDt == DateTime.MinValue)
                {
                    return ArmsApiResponse.Create(true, "NW0123:開始日時が入力されていません", order.ProcNo, order.InMagazineNo, order.MacNo, order.LineNo);
                }

                #endregion

                // 開始作業者が汎用ユーザ(660)、空の場合、装置作業者設定画面で設定した社員番号を記録する
                if (Config.Settings.ActiveMachineOperator == true &&
                    (order.TranStartEmpCd == "660" || string.IsNullOrEmpty(order.TranStartEmpCd) == true))
                {
                    order.TranStartEmpCd = MachineInfo.GetOperator(machine.NascaPlantCd).ToString();
                }

                // 最初の工程（搭載）の場合
                if (process.FirstSt == true)
                {
                    return WorkStartWithFirstOrder(order, false);
                }

                // 現在の指図情報を取得する
                Magazine mag = Magazine.GetCurrent(order.InMagazineNo);
                AsmLot lot = mag != null ? AsmLot.GetAsmLot(mag.NascaLotNO) : null;
                if (mag == null || lot == null)
                {
                    return ArmsApiResponse.Create(true, "ロット情報が見つかりません", order.ProcNo, order.InMagazineNo, order.MacNo, order.LineNo);
                }

                // ロットステータスが「警告」か
                if (lot.IsWarning)
                {
                    // ログ出力（処理終了）
                    Log.ApiLog.Info("[api] EditWorkStart end:" + order.InMagazineNo);
                    return ArmsApiResponse.Create(true, "ロット特性[警告]がONの為、移動できません", lot.NascaLotNo, order.ProcNo, order.InMagazineNo, order.MacNo); // , string.Join(",", lot.MacGroup)を引数から除去 (2015/5/4 nyoshimoto)
                }

                if (string.IsNullOrEmpty(order.LotNo)) order.LotNo = mag.NascaLotNO;

                // ダミー作業実績登録の可・不可の判定
                bool insertDummyTranFg = false;
                int nowprocno = mag.NowCompProcess;
                int dummyprocno = order.ProcNo;
                bool needDummyInsertBeforeWork = WorkChecker.CanDummyAutoTran(lot, order.ProcNo);
                if (needDummyInsertBeforeWork == true)
                {
                    // 登録可の場合、このタイミングでダミー実績登録を実施
                    Process dummyProc = Process.GetPrevProcess(order.ProcNo, lot.TypeCd);
                    Order.RecordDummyWork(lot, dummyProc.ProcNo, dummyProc.AutoUpdMachineNo.Value, mag, order.WorkStartDt);
                    insertDummyTranFg = true;
                }

                // 作業開始登録
                if (process.FinalSt == true && MachineInfo.IsCutMachine(machine.ClassName))
                {
                    // 開始前の各種特性チェック、原材料チェック
                    string msg;
                    bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, process, out msg);
                    if (isError)
                    {
                        // ダミー実績を登録した際は、削除 + 巻き戻し
                        if (insertDummyTranFg == true)
                        {
                            Order.ReturnDummyWork(mag, dummyprocno, nowprocno);
                        }

                        return ArmsApiResponse.Create(true, msg, lot.NascaLotNo, order.ProcNo, order.InMagazineNo, order.MacNo); // , string.Join(",", lot.MacGroup)を引数から除去 (2015/5/4 nyoshimoto)
                    }

                    // FinalStが有効な工程の場合はブレンド
                    CutBlend.InputAsmLot(order);
                }
                else
                {
                    order.LotNo = mag.NascaLotNO;
                    Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, order.ProcNo);
                    if (dst == Process.MagazineDevideStatus.SingleToDouble)
                    {
                        // 分割発生工程の開始は2マガジン分の開始を登録する

                        // マガジンのロット番号を分割番号付きに更新
                        order.DevidedMagazineSeqNo = 1;

                        // 開始前の各種特性チェック、原材料チェック
                        string msg;
                        bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, process, out msg);
                        if (isError)
                        {
                            // ダミー実績を登録した際は、削除 + 巻き戻し
                            if (insertDummyTranFg == true)
                            {
                                Order.ReturnDummyWork(mag, dummyprocno, nowprocno);
                            }
                            return ArmsApiResponse.Create(true, msg, order.LotNo, order.ProcNo, order.InMagazineNo, order.MacNo); // , string.Join(",", lot.MacGroup)を引数から除去 (2015/5/4 nyoshimoto)
                        }

                        // 作業開始登録（1マガジン目）
                        order.DeleteInsert(order.LotNo);
                        mag.NascaLotNO = order.LotNo;
                        mag.Update();

                        // マガジンのロット番号を分割番号付きに更新
                        order.DevidedMagazineSeqNo = 2;

                        // 作業開始登録（２マガジン目）
                        order.DeleteInsert(order.LotNo);
                        mag.NascaLotNO = order.LotNo;
                        mag.Update();

                        // 分割前ロット番号の関連付け解除
                        mag.NascaLotNO = Order.MagLotToNascaLot(order.LotNo);
                        mag.NewFg = false;
                        mag.Update();

                        // magの情報を1マガジン目に戻す（今後、↓に処理を追加する際の保険）
                        order.DevidedMagazineSeqNo = 1;
                        mag.NascaLotNO = order.LotNo;
                        mag.NewFg = true;
                    }
                    else if (dst == Process.MagazineDevideStatus.DoubleToSingle)
                    {
                        // 開始前の各種特性チェック、原材料チェック
                        string msg;
                        bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, process, out msg);
                        if (isError)
                        {
                            // ダミー実績を登録した際は、削除 + 巻き戻し
                            if (insertDummyTranFg == true)
                            {
                                Order.ReturnDummyWork(mag, dummyprocno, nowprocno);
                            }
                            return ArmsApiResponse.Create(true, msg, order.LotNo, order.ProcNo, order.InMagazineNo, order.MacNo); // , string.Join(",", lot.MacGroup)を引数から除去 (2015/5/4 nyoshimoto)
                        }
                        
                        // 元のロット番号の関連付け削除
                        mag.NewFg = false;
                        mag.Update();

                        // 纏め発生工程では作業レコードを一つにする
                        order.DevidedMagazineSeqNo = 0;

                        // マガジンのロット番号を分割番号なしに更新
                        mag.NewFg = true;
                        mag.NascaLotNO = order.LotNo;
                        mag.Update();

                        // 作業開始登録
                        order.DeleteInsert(order.LotNo);
                    }
                    else
                    {
                        // 開始前の各種特性チェック、原材料チェック
                        string msg;
                        bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, process, out msg);
                        if (isError)
                        {
                            // ダミー実績を登録した際は、削除 + 巻き戻し
                            if (insertDummyTranFg == true)
                            {
                                Order.ReturnDummyWork(mag, dummyprocno, nowprocno);
                            }
                            return ArmsApiResponse.Create(true, msg, order.LotNo, order.ProcNo, order.InMagazineNo, order.MacNo); // , string.Join(",", lot.MacGroup)を引数から除去 (2015/5/4 nyoshimoto)
                        }
                        
                        // 作業開始登録
                        order.DeleteInsert(order.LotNo);
                    }

                    // ダイボンド2ロット中、1ロット抜き取り状態検査設定
                    if (Config.Settings.DBOneOfTwoInspectionProcNo != null && Config.Settings.DBOneOfTwoInspectionProcNo.Contains(order.ProcNo))
                    {
                        Inspection.OneOfTwoSampling(order);
                    }

                    // 2015.1.1 3in1改修
                    // ダイシェア抜き取り試験設定
                    if (Config.Settings.DieShearSamplingCheckProcNo.HasValue && Config.Settings.DieShearSamplingCheckProcNo.Value == order.ProcNo)
                    {
                        Inspection.DieShearSampling(order, mag);
                    }

                    ////強度試験設定(完了処理に移動)
                    //if (Config.Settings.SetPSTesterProcNo.HasValue && Config.Settings.SetPSTesterProcNo.Value == order.ProcNo)
                    //{
                    //    Inspection.PSTesterSampling(order, mag);
                    //}
                }

                // FinalStが有効の工程(カットブレンド)の場合マガジンとロット切り離し
                if (process.FinalSt == true && MachineInfo.IsCutMachine(machine.ClassName))
                {
                    Magazine.UpdateNewFgOff(order.InMagazineNo);
                }

                // ログ出力（処理終了）
                Log.ApiLog.Info("[api] EditWorkStart end:" + order.InMagazineNo);
                return ArmsApiResponse.Create(false, process.InlineProNM + "を開始しました", order.LotNo, order.ProcNo, order.InMagazineNo, order.MacNo); // , string.Join(",", lot.MacGroup)を引数から除去 (2015/5/4 nyoshimoto)
            }
            catch (Exception ex)
            {
                Log.ApiLog.Error("作業開始登録エラー", ex);
                throw (ex);
            }
        }
        public static Order GetWorkStartOrder(VirtualMag mag, int macno)
        {
            Order order = new Order();

            order.MacNo = macno;
            order.ProcNo = mag.ProcNo.Value;
            order.ScNo1 = string.Empty;
            order.ScNo2 = string.Empty;
            order.InMagazineNo = mag.MagazineNo;
            order.OutMagazineNo = string.Empty;
            order.WorkStartDt = System.DateTime.Now;

            order.WorkEndDt = null;

            Process p = Process.GetProcess(mag.ProcNo.Value);
            if (p.IsNascaDefect)
            {
                order.IsDefectEnd = false;
            }
            else
            {
                order.IsDefectEnd = true;
            }

            return order;
        }
        #endregion

        #region [NEL] 作業終了登録 WorkEnd
        /// <summary>
        /// 作業終了登録
        /// </summary>
        public static ArmsApiResponse WorkEnd(Order order)
        {
            // ログ出力（処理開始）
            Log.ApiLog.Info("[api] EditWorkEnd start:" + order.OutMagazineNo);

            try
            {
                Process process = Process.GetProcess(order.ProcNo);

                // 指図発行区分=(4:作業終了時に現指図発行)の場合は別メソッドで処理
                if (process.FirstSt == true)
                {
                    return WorkEndWithFirstOrder(order);
                }

                // 各マスタ取得
                MachineInfo machine = MachineInfo.GetMachine(order.MacNo);
                Magazine mag = Magazine.GetCurrent(order.InMagazineNo);
                if (mag == null)
                {
                    return ArmsApiResponse.Create(true,
                        "現在稼働フラグの無いマガジンです:" + order.InMagazineNo,
                        order.ProcNo, order.InMagazineNo, order.MacNo, order.LineNo);
                }

                #region 工程　設備マスタの存在チェック

                // 工程マスタに品目/作業が紐づいているか
                if (process == null || process.WorkCd.Trim() == string.Empty)
                {
                    //if (process.FinalSt == true)
                    //{
                    //    // マガジンとロット切り離し
                    //    Magazine.UpdateNewFgOff(order.InMagazineNo);
                    //}

                    // 工程マスタに品目、作業がない場合は以下の処理を行わず、正常として返す
                    return ArmsApiResponse.Create(false, "", order.ProcNo, order.OutMagazineNo, order.MacNo, order.LineNo);
                }
                else
                {
                    // 設備CD
                    if (machine == null || string.IsNullOrEmpty(machine.NascaPlantCd))
                    {
                        return ArmsApiResponse.Create(true,
                        "NW0036：この設備はマスターに登録されていません",
                        mag.NascaLotNO, order.ProcNo, order.OutMagazineNo, order.MacNo);  // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto)
                    }
                }
                #endregion

                #region 開始・終了日、マガジン入れ替わりチェック

                // マガジン変更チェック
                if (machine.MagazineChgFg == true)
                {
                    if (order.InMagazineNo == order.OutMagazineNo)
                    {
                        return ArmsApiResponse.Create(true,
                        "NW0652：搬入と搬出のマガジンに誤りがあります",
                        mag.NascaLotNO, order.ProcNo, order.OutMagazineNo, order.MacNo);  // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto)
                    }
                }

                // 作業開始時間
                if (order.WorkStartDt == DateTime.MinValue)
                {
                    return ArmsApiResponse.Create(true,
                        "NW0123：開始日時が入力されていません",
                        mag.NascaLotNO, order.ProcNo, order.OutMagazineNo, order.MacNo);  // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto)
                }

                // 作業完了時間
                if (order.WorkEndDt.HasValue == false)
                {
                    return ArmsApiResponse.Create(true,
                        "NW0124：完了日時が入力されていません",
                        mag.NascaLotNO, order.ProcNo, order.OutMagazineNo, order.MacNo);  // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto)
                }

                // 終了時間が開始時間より後かチェック
                if (order.WorkEndDt.Value < order.WorkStartDt)
                {
                    return ArmsApiResponse.Create(true,
                        "開始・完了時間が不正です",
                        order.NascaLotNo, order.ProcNo, order.OutMagazineNo, order.MacNo);  // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto)
                }

                #endregion

                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                order.LotNo = mag.NascaLotNO;

                //分割マガジン工程の場合、初回はマガジンを分割する
                Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, order.ProcNo);
                if (dst == Process.MagazineDevideStatus.SingleToDouble)
                {
                    order.DevidedMagazineSeqNo = 1;

                    Order[] existsOrders = Order.GetOrder(order.NascaLotNo, order.ProcNo);
                    foreach (Order e in existsOrders)
                    {
                        //既にSeqNo=1の完了レコードがある場合
                        if (e.IsComplete && e.OutMagazineNo != order.OutMagazineNo)
                        {
                            order.DevidedMagazineSeqNo = 2;
                        }
                    }
                }

                //作業飛ばしのチェック
                if (dst != Process.MagazineDevideStatus.DoubleToSingle)
                {
                    bool isSkipped = WorkChecker.IsWorkSkiped(order.LotNo, order.ProcNo);
                    if (isSkipped == true)
                        return ArmsApiResponse.Create(true,
                            "前作業の実績が入力されていません",
                            order.LotNo, order.ProcNo, order.OutMagazineNo, order.MacNo); // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto)
                }
                else
                {

                    //1マガジン目の作業飛ばしチェック
                    string preLotNo = Order.NascaLotToMagLot(order.LotNo, 1);
                    bool isSkipped = WorkChecker.IsWorkSkiped(preLotNo, order.ProcNo);
                    if (isSkipped)
                    {
                        //1マガジン目が作業とばしなら2マガジン目チェック
                        preLotNo = Order.NascaLotToMagLot(order.LotNo, 2);
                        isSkipped = WorkChecker.IsWorkSkiped(preLotNo, order.ProcNo);

                        //両方のマガジンが作業飛ばし状態なら異常と判定
                        if (isSkipped == true)
                            return ArmsApiResponse.Create(true,
                                "前作業の実績が入力されていません",
                                order.LotNo, order.ProcNo, order.OutMagazineNo, order.MacNo); // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto) 
                    }
                }

                //既に入力されている開始レコードがある場合は作業者情報などを引き継ぐ
                //開始時間等は、完了側の方が正しい情報なので引き継がない
                Order preInput = Order.GetMagazineOrder(order.NascaLotNo, order.ProcNo);
                if (preInput != null)
                {
                    order.InspectCt = preInput.InspectCt;
                    order.InspectEmpCd = preInput.InspectEmpCd;
                    order.TranStartEmpCd = preInput.TranStartEmpCd;
                    order.Comment = preInput.Comment;

                    order.IsDefectEnd = preInput.IsDefectEnd;
                }
                else
                {
                    if (process.IsNascaDefect) { order.IsDefectEnd = false; }
                    else { order.IsDefectEnd = true; }
                }

                // 完了、検査作業者が汎用ユーザ(660)、空の場合、装置作業者設定画面で設定した社員番号を記録する
                if (Config.Settings.ActiveMachineOperator == true &&
                    (order.TranCompEmpCd == "660" || string.IsNullOrEmpty(order.TranCompEmpCd) == true))
                {
                    order.TranCompEmpCd = MachineInfo.GetOperator(machine.NascaPlantCd).ToString();
                }
                if (Config.Settings.ActiveMachineOperator == true &&
                    (order.InspectEmpCd == "660" || string.IsNullOrEmpty(order.InspectEmpCd) == true))
                {
                    order.InspectEmpCd = order.TranCompEmpCd;
                }

                // NGでも作業は完了させるのでチェック類は全て後で行う
                // 作業終了登録
                order.DeleteInsert(order.LotNo);

                // インラインマガジンロット更新
                mag = Magazine.ApplyMagazineInOut(order, order.LotNo);

                // ロットステータスが「警告」の場合
                if (lot.IsWarning)
                {
                    return ArmsApiResponse.Create(true,
                        "ロット特性[警告]がONの為、移動できません",
                        order.LotNo, order.ProcNo, order.OutMagazineNo, order.MacNo); // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto)
                }

                // 原材料等のチェック
                string msg;
                bool isError = WorkChecker.IsErrorWorkComplete(order, machine, lot, out msg);
                if (isError)
                {
                    //完了エラー時はロットの警告ステータスON
                    lot.IsWarning = true;
                    lot.Update();

                    //作業実績のコメント追記
                    order.Comment += " [完了異常]" + msg;
                    order.DeleteInsert(order.LotNo);

                    return ArmsApiResponse.Create(true, msg, order.LotNo, order.ProcNo, order.OutMagazineNo, order.MacNo); // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto)
                }
                
                //強度試験設定
                if (Config.Settings.SetPSTesterProcNo.HasValue && Config.Settings.SetPSTesterProcNo.Value == order.ProcNo)
                {
                    Inspection.PSTesterSampling(order, mag);
                }

                // ログ出力（処理終了）
                Log.ApiLog.Info("[api] EditWorkEnd end:" + order.InMagazineNo + " >> " + order.OutMagazineNo);
                return ArmsApiResponse.Create(false, process.InlineProNM + "を完了しました", order.LotNo, order.ProcNo, order.OutMagazineNo, order.MacNo); // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto)
            }
            catch (ArmsException aex)
            {
                throw aex;
            }
            catch (Exception ex)
            {
                // ログ出力（処理異常終了）
                return ArmsApiResponse.Create(true,
                    "完了登録時に不明なエラーが発生しました" + ex.ToString(),
                    order.LotNo, order.ProcNo, order.OutMagazineNo, order.MacNo); // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto)
            }
        }
        public static Order GetWorkEndOrder(VirtualMag mag, int macno, string lineNo)
        {
            Order order = new Order();

            order.MacNo = macno;
            order.LineNo = lineNo;
            if (mag.ProcNo != null)
            {
                order.ProcNo = mag.ProcNo.Value;
            }
            order.InMagazineNo = mag.LastMagazineNo;
            order.OutMagazineNo = mag.MagazineNo;
            if (mag.WorkStart != null)
            {
                order.WorkStartDt = mag.WorkStart.Value;
            }
            if (mag.WorkComplete != null)
            {
                order.WorkEndDt = mag.WorkComplete.Value;
            }
            if (mag.Stocker1.HasValue)
            {
                order.ScNo1 = mag.Stocker1.Value.ToString();
            }
            if (mag.Stocker2.HasValue)
            {
                order.ScNo2 = mag.Stocker2.Value.ToString();
            }
            if (mag.StartWafer.HasValue)
            {
                order.StockerStartCol = mag.StartWafer.Value;
                order.ScNo1 = string.Format("{0}-{1}", 0, mag.StartWafer.Value);
            }
            if (mag.EndWafer.HasValue)
            {
                order.StockerEndCol = mag.EndWafer.Value;
                order.ScNo2 = string.Format("{0}-{1}", mag.WaferChangerChangeCount, mag.EndWafer.Value);
            }
            if (mag.WaferChangerChangeCount.HasValue == true)
            {
                order.StockerChangeCt = mag.WaferChangerChangeCount.Value;
            }
            else
            {
                order.StockerChangeCt = 0;
            }

            if (mag.RelatedMaterials.Count != 0)
            {
                order.FrameList = mag.RelatedMaterials.ToArray();
            }

            return order;
        }

        #region [NEL] WorkStartWithFirstOrder

        public static ArmsApiResponse WorkStartWithOutFirstOrder(VirtualMag vMag, int macNo)
        {
            Order order = ArmsApi.CommonApi.GetWorkStartOrder(vMag, macNo);
            return WorkStartWithFirstOrder(order, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startInfo"></param>
        /// <returns></returns>
        private static ArmsApiResponse WorkStartWithFirstOrder(Order startInfo, bool isCheckOnly)
        {
            // 各マスタ取得
            MachineInfo machine = MachineInfo.GetMachine(startInfo.MacNo);
            Process process = Process.GetProcess(startInfo.ProcNo);

            // 開始作業(全工程中)の場合は、マガジンとロットを強制的に切り離しする
            // カット完了を手動で行ったマガジンが別ロットに置き換わって再投入される対策
            Magazine.UpdateNewFgOff(startInfo.InMagazineNo);
            string errMsg;

            if (Config.GetLineType != Config.LineTypes.NEL_MAP && Config.GetLineType != Config.LineTypes.MEL_MAP)
            {
                #region 搭載工程（開始登録を行わないチェックのみ)限定処理

                //開始側ではStocker未確定のため両取得
                Material[] matlist = machine.GetMaterials(startInfo.WorkStartDt, null);

                Profile prof = null;
                prof = Profile.GetCurrentDBProfile(startInfo.MacNo);
                if (prof == null)
                {
                    Log.ApiLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
                    return ArmsApiResponse.Create(true, "使用可能なプロファイルが存在しません", startInfo.ProcNo, startInfo.InMagazineNo, startInfo.MacNo, startInfo.LineNo);
                }

                BOM[] bom = Profile.GetBOM(prof.ProfileId);

                //2012/11/13 sasaki------------------------------------------------------[start]
                AsmLot asmLot = new AsmLot();
                asmLot.TypeCd = prof.TypeCd;
                asmLot.ProfileId = prof.ProfileId;

                if (Config.Settings.IsDBProfileRequireMode == false)
                {
                    bool isBomError = WorkChecker.IsBomError(matlist, bom, out errMsg, asmLot, process.WorkCd, machine.MacNo);

                    //bool isBomError = WorkChecker.IsBomError(matlist, bom, out errMsg);
                    //------------------------------------------------------------------------[end]

                    if (isBomError)
                    {
                        Log.ApiLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
                        return ArmsApiResponse.Create(true, errMsg, "", startInfo.ProcNo, startInfo.InMagazineNo, startInfo.MacNo); // , startInfo.LineNoを引数から除去 (2015/5/4 nyoshimoto)
                    }
                }

                bool isInputWorkError = WorkChecker.CanInputToWorkMaterial(matlist, asmLot.TypeCd, process.WorkCd, out errMsg);
                if (isInputWorkError)
                {
                    Log.ApiLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
                    return ArmsApiResponse.Create(true, errMsg, "", startInfo.ProcNo, startInfo.InMagazineNo, startInfo.MacNo); // , startInfo.LineNoを引数から除去 (2015/5/4 nyoshimoto)
                }

                Log.ApiLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
                return ArmsApiResponse.Create(false, process.InlineProNM + "を開始しました(登録なし)", startInfo.ProcNo, startInfo.InMagazineNo, startInfo.MacNo, startInfo.LineNo);

                //if (!isBomError)
                //{
                //	Log.ApiLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
                //	return ArmsApiResponse.Create(false, process.InlineProNM + "を開始しました(登録なし)", startInfo.ProcNo);
                //}
                //else
                //{
                //	Log.ApiLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
                //	return ArmsApiResponse.Create(true, errMsg, "", startInfo.ProcNo);
                //}

                #endregion
            }

            #region MAPライン　ロット未採番でマガジン番号の作業レコード作成

            // 現在の指図情報を取得する
            Magazine mag = new Magazine();
            mag.MagazineNo = startInfo.InMagazineNo;
            mag.NascaLotNO = mag.MagazineNo;
            mag.NewFg = true;
            mag.NowCompProcess = startInfo.ProcNo;
            mag.Update();

            AsmLot lot = AsmLot.CreateNewAsmLotWithoutLotNumbering(startInfo.WorkStartDt, mag.MagazineNo, true, startInfo.MacNo);
            startInfo.LotNo = lot.NascaLotNo;

            //マガジン番号で不良を持っていた場合はクリア
            Defect magazineDef = Defect.GetDefect(mag.MagazineNo, startInfo.ProcNo);
            if (magazineDef.DefItems.Count >= 1)
            {
                //マガジン番号名義の不良数を削除
                magazineDef.DefItems.Clear();
                magazineDef.DeleteInsert();
            }

            //　開始前の各種特性チェック、原材料チェック
            string msg;
            bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, startInfo, process, out msg);
            if (isError)
            {
                mag.NewFg = false;
                mag.Update();
                return ArmsApiResponse.Create(true, msg, lot.NascaLotNo, startInfo.ProcNo, startInfo.InMagazineNo, startInfo.MacNo); // , string.Join(",", lot.MacGroup)を引数から除去 (2015/5/4 nyoshimoto)
            }

            if (isCheckOnly == true)
            {
                return ArmsApiResponse.Create(false, process.InlineProNM + "の作業前チェックが完了しました。", startInfo.ProcNo, startInfo.LotNo, startInfo.MacNo, string.Empty);
            }

            //作業開始登録
            startInfo.DeleteInsert(lot.NascaLotNo);

            // ログ出力（処理終了）
            Log.ApiLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
            return ArmsApiResponse.Create(false, process.InlineProNM + "を開始しました", startInfo.ProcNo, startInfo.LotNo, startInfo.MacNo, string.Empty); // , string.Join(",", lot.MacGroup)を引数から除去 (2015/5/4 nyoshimoto)  ここで呼んでるCreate()が65箇所から呼ばれてるので最後のstring引数は string.Emptyで渡した
            #endregion
        }

        #endregion

        #region 作業完了+指図発行 workEndWithFirstOrder
        /// <summary>
        /// 作業完了+指図発行
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private static ArmsApiResponse WorkEndWithFirstOrder(Order order)
        {
            MachineInfo machine = MachineInfo.GetMachine(order.MacNo);
            Magazine mag = Magazine.GetCurrent(order.OutMagazineNo);
            Process p = Process.GetProcess(order.ProcNo);

            if (mag != null)
            {
                if (mag.NascaLotNO != mag.MagazineNo)
                {
                    return ArmsApiResponse.Create(true,
                        "NW0650：搬出マガジンに、現在稼動中のロットがあります", mag.NascaLotNO,
                        order.ProcNo, order.OutMagazineNo, order.MacNo);  // , order.LineNo を引数から除去 (2015/5/4 nyoshimoto)
                }
                else
                {
                    //ロット=マガジン番号のマガジンの場合は稼働フラグOFF
                    Magazine.UpdateNewFgOff(mag.MagazineNo);
                }
            }

            //TempLotはNASCA側がロット採番していた名残。現在は必要無し
            AsmLot newlot = AsmLot.CreateNewAsmLot(order.WorkStartDt, order.MacNo, order.LineNo, machine.IsHighLine);

            //搭載機なしラインでD/B投入時点で仮指図発行される場合の処理
            Order magOrder = Order.GetMagazineOrder(order.InMagazineNo, order.ProcNo);
            if (magOrder != null)
            {
                order.InspectCt = magOrder.InspectCt;
                order.InspectEmpCd = magOrder.InspectEmpCd;
                order.IsDefectEnd = magOrder.IsDefectEnd;
                order.IsDefectAutoImportEnd = magOrder.IsDefectAutoImportEnd;
                order.TranStartEmpCd = magOrder.TranStartEmpCd;

                magOrder.WorkEndDt = DateTime.Now;
                magOrder.DelFg = true;
                magOrder.DeleteInsert(magOrder.LotNo);

                AsmLot maglot = AsmLot.GetAsmLot(order.InMagazineNo);
                if (maglot != null && newlot.LotSize != maglot.LotSize)
                {
                    newlot.LotSize = maglot.LotSize;
                }
            }
            else
            {
                //TODO 2014.7.17 作成途中未検証
                if (p.IsNascaDefect) { order.IsDefectEnd = false; }
                else { order.IsDefectEnd = true; }
            }

            // 完了、検査作業者が汎用ユーザ(660)、空の場合、装置作業者設定画面で設定した社員番号を記録する
            if (Config.Settings.ActiveMachineOperator == true &&
                (order.TranCompEmpCd == "660" || string.IsNullOrEmpty(order.TranCompEmpCd) == true))
            {
                order.TranCompEmpCd = MachineInfo.GetOperator(machine.NascaPlantCd).ToString();
            }
            if (Config.Settings.ActiveMachineOperator == true &&
                (order.InspectEmpCd == "660" || string.IsNullOrEmpty(order.InspectEmpCd) == true))
            {
                order.InspectEmpCd = order.TranCompEmpCd;
            }

            order.LotNo = newlot.NascaLotNo;
            order.DeleteInsert(newlot.NascaLotNo);

            newlot = AsmLot.GetAsmLot(newlot.NascaLotNo);

            mag = new Magazine();
            mag.NascaLotNO = newlot.NascaLotNo;
            mag.MagazineNo = order.OutMagazineNo;
            mag.NowCompProcess = order.ProcNo;
            mag.NewFg = true;
            mag.Update();

            //MAP搭載工程の場合は完了時に原材料関連付け
            if (order.FrameList != null && order.FrameList.Count() != 0)
            {
                order.UpdateMaterialRelation(order.FrameList);
            }

            //マガジン番号で不良を持っていた場合はアッセンロットに引き継ぎ
            Defect magazineDef = Defect.GetDefect(mag.MagazineNo, order.ProcNo);
            if (magazineDef.DefItems.Count >= 1)
            {
                //新規採番のロット番号で保存
                magazineDef.LotNo = newlot.NascaLotNo;
                magazineDef.DeleteInsert();

                //マガジン番号名義の不良数を削除
                magazineDef.LotNo = mag.MagazineNo;
                magazineDef.DefItems.Clear();
                magazineDef.DeleteInsert();
            }

            // 2015.4.27 車載自動化の2015.GW限定の快速モード
            // ダイボンドでLotのProfileを更新する為、ここではNASCA連携しない
            if (Config.Settings.IsDBProfileRequireMode == false)
            {
                //期間中の原材料、ロット情報を更新
                Exporter exp = Exporter.GetInstance();
                NASCAResponse res = exp.SendOrderToNasca(newlot, p);
            }

            //NASCA送信時点でOrderが更新されているので再取得
            order = Order.GetMagazineOrder(order.LotNo, order.ProcNo);

            // 原材料でBadMarkフレームを使用していればアッセンロットにフラグを反映
            if (order.GetMaterials().ToList().Exists(m => m.IsBadMarkFrame))
            {
                newlot.IsBadMarkFrame = true;
                newlot.Update();
            }

            // 原材料等のチェック
            string msg;
            bool isError = WorkChecker.IsErrorWorkComplete(order, machine, newlot, out msg);
            if (isError)
            {
                //完了エラー時はロットの警告ステータスON
                newlot.IsWarning = true;
                newlot.Update();

                //作業実績のコメント追記
                order.Comment += " [完了異常]" + msg;
                order.DeleteInsert(newlot.NascaLotNo);

                return ArmsApiResponse.Create(true, msg, newlot.NascaLotNo, order.ProcNo, order.OutMagazineNo, order.MacNo); // , order.LineNoを引数から除去 (2015/5/4 nyoshimoto)
            }

            // ログ出力（処理終了）
            Process process = Process.GetProcess(order.ProcNo);
            Log.ApiLog.Info("[api] EditWorkEnd end:" + order.InMagazineNo + " >> " + order.OutMagazineNo);
            return ArmsApiResponse.Create(false, process.InlineProNM + "を完了しました", newlot.NascaLotNo, order.ProcNo, order.OutMagazineNo, order.MacNo); // , order.LineNo を引数から除去 (2015/5/4 nyoshimoto)
        }
        #endregion

        #endregion

        #region 新規ロット振り替え
        /// <summary>
        /// 新規ロットNo取得 + 実績振替
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static ArmsApiResponse ChangeNewAsmLot(Order order)
        {
            string magazineno = order.OutMagazineNo;
            if (string.IsNullOrWhiteSpace(magazineno) == true)
            {
                magazineno = order.InMagazineNo;
            }
            MachineInfo machine = MachineInfo.GetMachine(order.MacNo);
            Magazine mag = Magazine.GetCurrent(magazineno);
            Process p = Process.GetProcess(order.ProcNo);

            if (mag != null)
            {
                if (mag.NascaLotNO != mag.MagazineNo)
                {
                    return ArmsApiResponse.Create(true,
                        "NW0650：搬出マガジンに、現在稼動中のロットがあります", mag.NascaLotNO,
                        order.ProcNo, order.OutMagazineNo, order.MacNo);  // , order.LineNo を引数から除去 (2015/5/4 nyoshimoto)
                }
                else
                {
                    //ロット=マガジン番号のマガジンの場合は稼働フラグOFF
                    Magazine.UpdateNewFgOff(mag.MagazineNo);
                }
            }

            //TempLotはNASCA側がロット採番していた名残。現在は必要無し
            AsmLot newlot = AsmLot.CreateNewAsmLot(order.WorkStartDt, order.MacNo, order.LineNo, machine.IsHighLine);

            //搭載機なしラインでD/B投入時点で仮指図発行される場合の処理
            Order magOrder = Order.GetMagazineOrder(order.InMagazineNo, order.ProcNo);
            if (magOrder != null)
            {
                order.OutMagazineNo = magOrder.OutMagazineNo;
                // この1行を入れないと、PDA登録前に実績完了として登録されてしまう.
                order.WorkEndDt = magOrder.WorkEndDt;
                order.ScNo1 = string.Empty;
                order.ScNo2 = string.Empty;
                order.TranStartEmpCd = magOrder.TranStartEmpCd;
                order.TranCompEmpCd = magOrder.TranCompEmpCd;

                order.InspectCt = magOrder.InspectCt;
                order.InspectEmpCd = magOrder.InspectEmpCd;
                order.IsDefectEnd = magOrder.IsDefectEnd;
                order.IsDefectAutoImportEnd = magOrder.IsDefectAutoImportEnd;

                // 空マガジンの仮実績を作業完了にする
                magOrder.WorkEndDt = DateTime.Now;
                magOrder.DelFg = true;
                magOrder.DeleteInsert(magOrder.LotNo);

                AsmLot maglot = AsmLot.GetAsmLot(order.InMagazineNo);
                if (maglot != null && newlot.LotSize != maglot.LotSize)
                {
                    newlot.LotSize = maglot.LotSize;
                }
            }
            else
            {
                //TODO 2014.7.17 作成途中未検証
                if (p.IsNascaDefect) { order.IsDefectEnd = false; }
                else { order.IsDefectEnd = true; }
            }

            order.LotNo = newlot.NascaLotNo;
            order.DeleteInsert(newlot.NascaLotNo);

            newlot = AsmLot.GetAsmLot(newlot.NascaLotNo);

            mag = new Magazine();
            mag.NascaLotNO = newlot.NascaLotNo;
            mag.MagazineNo = magazineno;
            mag.NowCompProcess = order.ProcNo;
            mag.NewFg = true;
            mag.Update();
            
            //マガジン番号で不良を持っていた場合はアッセンロットに引き継ぎ
            Defect magazineDef = Defect.GetDefect(mag.MagazineNo, order.ProcNo);
            if (magazineDef.DefItems.Count >= 1)
            {
                //新規採番のロット番号で保存
                magazineDef.LotNo = newlot.NascaLotNo;
                magazineDef.DeleteInsert();

                //マガジン番号名義の不良数を削除
                magazineDef.LotNo = mag.MagazineNo;
                magazineDef.DefItems.Clear();
                magazineDef.DeleteInsert();
            }
            
            // 原材料でBadMarkフレームを使用していればアッセンロットにフラグを反映
            if (order.GetMaterials().ToList().Exists(m => m.IsBadMarkFrame))
            {
                newlot.IsBadMarkFrame = true;
                newlot.Update();
            }

            // ログ出力（処理終了）
            return ArmsApiResponse.Create(false, "新規ロット振り替えを完了しました", newlot.NascaLotNo, order.ProcNo, order.OutMagazineNo, order.MacNo); // , order.LineNo を引数から除去 (2015/5/4 nyoshimoto)
        }
        #endregion


        public static bool IsNextWorkStarted(string magazineNO)
        {
            Magazine mag = Magazine.GetCurrent(magazineNO);
            if (mag == null)
            {
                throw new Exception(string.Format("MagazineNo:{0} 稼働マガジン情報が存在し無い為、次作業開始登録の確認ができませんでした。投入マガジンの稼働状態を確認して下さい。", magazineNO));
            }

            if (WorkChecker.IsNextWorkStarted(mag.NascaLotNO, mag.NowCompProcess))
            {
                return true;
            }
            else
            {
                return false;
            }

            #region MPLマージ前の古いコード
            //try
            //{
            //    Magazine mag = Magazine.GetCurrent(magazineNO);
            //    if (mag == null)
            //    {
            //        return ArmsApiResponse.Create(true, "マガジン情報が存在しません", order.ProcNo, order.OutMagazineNo, order.MacNo, order.LineNo);
            //    }

            //    AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);

            //    if (WorkChecker.IsNextWorkStarted(mag.NascaLotNO, mag.NowCompProcess))
            //    {
            //        return ArmsApiResponse.Create(true, "次の作業が開始されています", mag.NascaLotNO, mag.NowCompProcess, order.OutMagazineNo, order.MacNo); // , order.LineNo を引数から除去 (2015/5/4 nyoshimoto)
            //    }

            //    return QueryNextWork(order, vMag);
            //}
            //catch
            //{
            //    throw;
            //}
            #endregion
        }


        /// <summary>
        /// 文字列が全て半角文字か確認
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsHalfSizeChar(string str)
        {
            int byteCt = Encoding.GetEncoding("Shift_JIS").GetByteCount(str);
            if (byteCt == str.Length)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
