using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace FileIf
{
    public class _WorkStartModel
    {
        public _WorkStartModel(string plantcd)
        {
            MagList = new List<Magazine>();
            BlendLotList = new Dictionary<string, AsmLot>();
            CutBlendList = new List<CutBlend>();
            RecommendList = new List<string>();

            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(plantcd);

            this.DieShearSamplingLotList = new List<string>();
            this.NeedInspectionWhenStartLotList = new List<string>();
        }

        /// <summary>
        /// 装置
        /// </summary>
        public string PlantCd { get; set; }

        /// <summary>
        /// 登録作業者
        /// </summary>
        public string EmpCd { get; set; } = "testsan";

        public MachineInfo Mac { get; set; }

        public List<ArmsApi.Model.Magazine> MagList { get; set; }

        public Dictionary<string, AsmLot> BlendLotList { get; set; }

        public List<ArmsApi.Model.ReservedOrder> ReserveList { get; set; }

        public List<CutBlend> CutBlendList { get; set; }

        public List<string> RecommendList { get; set; }

        /// <summary>
        /// すでに作業中のマガジン
        /// </summary>
        public List<string> WorkingMags { get; set; }

        public List<string> DieShearSamplingLotList { get; set; }

        public List<string> NeedInspectionWhenStartLotList { get; set; }

        public ReservedOrder[] GetReserveList()
        {
            if (this.Mac != null)
            {
                return ReservedOrder.GetReserveList(this.Mac.MacNo);
            }
            else
            {
                return new ReservedOrder[0];
            }
        }


        public bool WorkStart(out string errMsg)
        {
            int startoffsetsec = 0;
            foreach (ArmsApi.Model.Magazine mag in MagList)
            {
                //念のため最新情報取得
                ArmsApi.Model.Magazine mag2 = Magazine.GetCurrent(mag.MagazineNo);

                if (mag2 == null)
                {
                    // TmWorkflow上の第1作業の装置の時、ロットNo = マガジンNoの仮ロットを作成する
                    int firstprocno;
                    if (IsFirstProcess(out firstprocno) == true)
                    {
                        // マガジンNo = ロットNoでTnLot, TnMagのレコードを作成 + 装置に紐づいているプロファイル情報でBOMチェック
                        VirtualMag vMag = new VirtualMag();
                        vMag.MagazineNo = mag.MagazineNo;
                        vMag.ProcNo = firstprocno;
                        ArmsApiResponse api = ArmsApi.CommonApi.WorkStartWithOutFirstOrder(vMag, this.Mac.MacNo);
                        if (api.IsError)
                        {
                            errMsg = api.Message;
                            return false;
                        }
                        mag2 = ArmsApi.Model.Magazine.GetCurrent(mag.MagazineNo);

                        // ロットNo = マガジンNoの場合、実績の使いまわしがある為、同一のロットNo,作業の実績が残っている場合、削除する
                        // ※削除しなかった場合は、作業開始時に次の作業扱いとして開始登録してしまう
                        Order oldOrder = Order.GetMagazineOrder(mag.MagazineNo, firstprocno);
                        if (oldOrder != null && oldOrder.IsComplete == true)
                        {
                            oldOrder.DelFg = true;
                            oldOrder.DeleteInsert(mag.MagazineNo);
                        }
                    }

                    //ブレンドされているロット、かつ最終工程以降の工程の開始の場合
                    //mag2には子ロットが入る
                    if (this.BlendLotList.ContainsKey(mag.MagazineNo) == true)
                    {
                        mag2 = mag;
                    }
                }

                if (mag2 == null)
                {
                    errMsg = "マガジン情報が見つかりません:" + mag.MagazineNo;
                    return false;
                }

                bool isSuccecc = startMag(mag2, startoffsetsec, false, out errMsg);
                if (!isSuccecc)
                {
                    return false;
                }
                startoffsetsec++;
            }

            //開始予約があった場合は削除（高効率のオーブン用機能）
            ReservedOrder.ClearReserveList(this.Mac.MacNo);

            errMsg = "";
            return true;
        }

        /// <summary>
        /// 開始前チェックを実施。
        /// 実際の登録は行わない
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckBeforeStart(Magazine mag, out string msg)
        {
            return startMag(mag, 0, true, out msg);
        }


        /// <summary>
        /// 遠心沈降中架空マガジンNOの識別文字
        /// </summary>
        public string WORKMAGAZINE_PREFIX = "_E";


        /// <summary>
        /// 遠心沈降自動機内部の備え付けマガジンへの入れ替え
        /// 自動化用の遠心沈降をアウトで使う暫定対応
        /// </summary>
        private void exchangeECKDummyMag(Order order, string typecd, Magazine mag)
        {
            // 供給後、フレームのみ装置内に入り、排出で別マガジンへ格納される為、TnMag.magnoを架空マガジンに振替える。
            // (架空マガジンNO：マガジンNO+識別文字(M******_E))

            //ApplyMagazineInOutは現在完了工程を更新してしまうので、一つ巻き戻してから呼び出す
            int originalProcNo = order.ProcNo;
            order.ProcNo = Process.GetPrevProcess(order.ProcNo, typecd).ProcNo;

            order.InMagazineNo = mag.MagazineNo;
            order.OutMagazineNo = string.Format("{0}{1}", mag.MagazineNo, WORKMAGAZINE_PREFIX);

            Magazine.ApplyMagazineInOut(order, order.LotNo);


        }

        /// <summary>
        /// 作業開始前チェック
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool startMag(Magazine mag, int startoffsetsec, bool isCheckOnly, out string msg)
        {
            try
            {
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                if (lot == null)
                {
                    msg = "ロット情報が存在しません";
                    return false;
                }

                //ブレンドロットの場合
                if (this.BlendLotList.ContainsKey(mag.MagazineNo) == true)
                {
                    //既に開始レコードがある場合、同じブレンドロットかどうかチェックする
                    Order[] orders = Order.GetCurrentWorkingOrderInMachine(Mac.MacNo);
                    foreach (Order o in orders)
                    {
                        if (this.BlendLotList[mag.MagazineNo].NascaLotNo != o.LotNo)
                        {
                            msg = $"対象装置で別のブレンドロットが開始されているため、本マガジンは開始できません。開始済みブレンドロット：{o.LotNo} 開始対象ブレンドロット：{this.BlendLotList[mag.MagazineNo].NascaLotNo} 開始対象マガジン：{mag.MagazineNo}";
                            return false;
                        }
                    }

                    if (orders.Length >= 1)
                    {
                        //すでに開始レコードがある場合、何もしない
                        msg = "";
                        return true;
                    }
                }

                // 完了工程 = 新規指図発行 + その作業が未完了の場合、完了工程の実績を対象に完了登録を実施。
                Process p = Process.GetProcess(mag.NowCompProcess);
                if (p == null)
                {
                    msg = "完了工程の工程情報が存在しません";
                    return false;
                }
                bool firstProcessFg = false;
                Process firstProc = Process.GetFirstProcess(lot.TypeCd);
                if (p.ProcNo == firstProc.ProcNo)
                {
                    Order firstOrder = Order.GetMagazineOrder(mag.NascaLotNO, p.ProcNo);
                    if (firstOrder == null || firstOrder.IsComplete == false)
                    {
                        firstProcessFg = true;
                    }
                }
                if (firstProcessFg == false)
                {
                    p = Process.GetNextProcess(mag.NowCompProcess, lot);
                }
                //Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
                if (p == null)
                {
                    msg = "工程情報が存在しません";
                    return false;
                }

                // ダミー作業実績登録の可・不可の判定
                bool insertDummyTranFg = false;
                int nowprocno = mag.NowCompProcess;
                int dummyprocno = p.ProcNo;
                if (WorkChecker.IsNotRelatedMachineAndProcess(this.Mac.MacNo, p.ProcNo) == true)
                {
                    Process oldProc = p;
                    p = Process.GetNextProcess(p.ProcNo, lot);
                    if (p != null && WorkChecker.IsNotRelatedMachineAndProcess(this.Mac.MacNo, p.ProcNo) == false)
                    {
                        bool needDummyInsertBeforeWork = WorkChecker.CanDummyAutoTran(lot, p.ProcNo);
                        if (needDummyInsertBeforeWork == true)
                        {
                            if (!isCheckOnly)
                            {
                                // 登録可の場合、このタイミングでダミー実績登録を実施
                                Process dummyProc = Process.GetPrevProcess(p.ProcNo, lot.TypeCd);
                                Order.RecordDummyWork(lot, dummyProc.ProcNo, dummyProc.AutoUpdMachineNo.Value, mag);
                                insertDummyTranFg = true;
                            }
                        }
                    }
                    else
                    {
                        p = oldProc;
                    }
                }

                Order order = new Order();
                order.LotNo = mag.NascaLotNO;
                //ブレンドされているロット、かつ最終工程以降の工程の場合、
                //子ロットを読み込み開始登録処理をすることで、
                //ARMS上は、親ロットNoで開始実績が登録される仕様となる
                if (this.BlendLotList.ContainsKey(mag.MagazineNo) == true)
                {
                    order.LotNo = this.BlendLotList[mag.MagazineNo].NascaLotNo;
                }
                order.ProcNo = p.ProcNo;
                order.InMagazineNo = mag.MagazineNo;
                if (this.BlendLotList.ContainsKey(mag.MagazineNo) == true)
                {
                    order.OutMagazineNo = mag.MagazineNo;
                }
                order.MacNo = this.Mac.MacNo;
                order.WorkStartDt = DateTime.Now.AddSeconds(startoffsetsec);
                order.WorkEndDt = null;
                order.TranStartEmpCd = this.EmpCd;
                order.TranCompEmpCd = this.EmpCd;

                if (p.IsNascaDefect)
                {
                    order.IsDefectEnd = false;
                }
                else
                {
                    order.IsDefectEnd = true;
                }

                string errMsg;

                bool isError = false;
                //ブレンドされているロット、かつ最終工程以降の工程の場合
                //引数に子ロット(mag.NascaLotNO)を追加する
                if (this.BlendLotList.ContainsKey(mag.MagazineNo) == true)
                {
                    isError = WorkChecker.IsErrorBeforeStartWork(lot, Mac, order, mag.NascaLotNO, p, out errMsg);
                }
                else
                {
                    isError = WorkChecker.IsErrorBeforeStartWork(lot, Mac, order, p, out errMsg);
                }
                if (isError)
                {
                    // ダミー実績を登録した際は、削除 + 巻き戻し
                    if (insertDummyTranFg == true)
                    {
                        Order.ReturnDummyWork(mag, dummyprocno, nowprocno);
                    }
                    msg = errMsg;
                    return false;
                }
                else if (!isCheckOnly)
                {
                    WorkCondition[] conds = this.Mac.GetWorkConditions(order.WorkStartDt, null);
                    int? programMinutes = null;
                    foreach (WorkCondition cond in conds)
                    {
                        programMinutes = WorkCondition.GetProgramMinutes(cond);
                        if (programMinutes.HasValue) break;
                    }

                    VirtualMag vmag = new VirtualMag();
                    vmag.MagazineNo = mag.MagazineNo;

                    //遠心沈降自動機をアウトラインで扱うための暫定対応
                    if (Mac.ClassName == "遠心沈降自動機")
                    {
                        vmag.MagazineNo = string.Format("{0}{1}", vmag.MagazineNo, WORKMAGAZINE_PREFIX);
                        exchangeECKDummyMag(order, lot.TypeCd, mag);
                        //exchangeが変更するので戻す
                        order.ProcNo = p.ProcNo;
                    }
                    vmag.WorkStart = order.WorkStartDt;
                    vmag.ProcNo = p.ProcNo;
                    if (programMinutes.HasValue)
                    {
                        vmag.ProgramTotalMinutes = programMinutes;
                    }

                    //Map1stDiebonderの場合はEnqueue先を切り替え
                    if (this.Mac.OuterMagFg == true)
                    {
                        VirtualMag[] vmags = VirtualMag.GetVirtualMag(this.Mac.MacNo, ((int)Station.EmptyMagazineLoader));
                        if (vmags.Where(v => v.MagazineNo == vmag.MagazineNo).Count() == 0)
                        {
                            vmag.Enqueue(vmag, new Location(this.Mac.MacNo, Station.EmptyMagazineLoader));
                        }
                    }
                    else
                    {
                        if (this.Mac.IsHighLine && MachineInfo.IsCutMachine(this.Mac.ClassName) &&
                            this.BlendLotList.ContainsKey(mag.MagazineNo) == false)
                        {
                            //高効率のカット機はLoaderに仮想マガジンを生成しない
                            //※ただしブレンドされているロット、かつ最終工程以降の工程の開始の場合は除く
                        }
                        else if (this.BlendLotList.ContainsKey(mag.MagazineNo) == true)
                        {
                            //ブレンドされているロット、かつ最終工程以降の工程の開始の場合はUnloaderに仮想マガジンを作成
                            VirtualMag[] vmags = VirtualMag.GetVirtualMag(this.Mac.MacNo, ((int)Station.Unloader));
                            if (vmags.Where(v => v.MagazineNo == vmag.MagazineNo).Count() == 0)
                            {
                                vmag.LastMagazineNo = vmag.MagazineNo;
                                vmag.Enqueue(vmag, new Location(this.Mac.MacNo, Station.Unloader));
                            }
                        }
                        else
                        {
                            //TODO Loader以外にEnqueueが必要な場合の処理記載必要
                            //MachineClassにGetLoaderLocationを配置する
                            VirtualMag[] vmags = VirtualMag.GetVirtualMag(this.Mac.MacNo, ((int)Station.Loader));
                            if (vmags.Where(v => v.MagazineNo == vmag.MagazineNo).Count() == 0)
                            {
                                //非連携装置で完了登録を個別に登録できるよう、isnoconnected=1の場合はUnloaderに生成する。
                                if (this.Mac.IsNoConnected == true)
                                {
                                    vmag.LastMagazineNo = vmag.MagazineNo;
                                    vmag.Enqueue(vmag, new Location(this.Mac.MacNo, Station.Unloader));
                                }
                                else
                                {
                                    vmag.Enqueue(vmag, new Location(this.Mac.MacNo, Station.Loader));
                                }
                            }
                        }
                    }

                    if (
                        (ArmsApi.Config.GetLineType == ArmsApi.Config.LineTypes.MEL_SV
                            || ArmsApi.Config.GetLineType == ArmsApi.Config.LineTypes.NEL_SV
                            || ArmsApi.Config.GetLineType == ArmsApi.Config.LineTypes.NEL_MAP
                            || ArmsApi.Config.GetLineType == ArmsApi.Config.LineTypes.MEL_MAP
                            || ArmsApi.Config.GetLineType == ArmsApi.Config.LineTypes.MEL_GAM
                            || ArmsApi.Config.GetLineType == ArmsApi.Config.LineTypes.NEL_GAM
                            || ArmsApi.Config.GetLineType == ArmsApi.Config.LineTypes.MEL_19
                            || ArmsApi.Config.GetLineType == ArmsApi.Config.LineTypes.MEL_NTSV)
                        && p.FinalSt == true)
                    {
                        CutBlend.InputAsmLot(order);
                    }
                    else
                    {
                        Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, p.ProcNo);
                        if (dst == Process.MagazineDevideStatus.SingleToDouble)
                        {
                            mag.NewFg = false;
                            mag.Update();

                            order.DevidedMagazineSeqNo = 1;
                            order.DeleteInsert(order.LotNo);

                            mag.NascaLotNO = order.LotNo;
                            string orgMag = mag.MagazineNo;
                            mag.MagazineNo = orgMag + "_#1";
                            mag.NewFg = true;
                            mag.Update();

                            order.DevidedMagazineSeqNo = 2;
                            order.DeleteInsert(order.LotNo);

                            mag.NascaLotNO = order.LotNo;
                            mag.MagazineNo = orgMag + "_#2";
                            mag.Update();
                        }
                        else
                        {
                            order.DeleteInsert(order.LotNo);
                        }

                        //2015.1.1 3in1改修
                        //ダイシェア抜き取り試験設定
                        if (ArmsApi.Config.Settings.DieShearSamplingCheckProcNo.HasValue
                            && ArmsApi.Config.Settings.DieShearSamplingCheckProcNo == order.ProcNo)
                        {
                            if (Inspection.DieShearSampling(order, mag) == true)
                            {
                                this.DieShearSamplingLotList.Add(order.LotNo);
                            }
                        }

                        //状態検査必要フラグ
                        Inspection isp = Inspection.GetInspection(order.NascaLotNo, order.ProcNo);
                        if (isp != null && isp.IsInspected == false) this.NeedInspectionWhenStartLotList.Add(order.NascaLotNo);


                        ////強度試験設定(不要のため削除)
                        //if (ArmsApi.Config.Settings.SetPSTesterProcNo.HasValue 
                        //    && ArmsApi.Config.Settings.SetPSTesterProcNo.Value == order.ProcNo)
                        //{
                        //    Inspection.PSTesterSampling(order, mag);
                        //}
                    }
                }

                //カットブレンド判定処理 
                //→高生産の場合は、複数マガジンを一括登録するので下記チェック処理が必要！！
                if (p.IsCutBlend)
                {
                    List<AsmLot> blend = new List<AsmLot>();
                    foreach (Magazine existmag in MagList)
                    {
                        AsmLot existlot = AsmLot.GetAsmLot(existmag.NascaLotNO);
                        if (!blend.Exists(b => b.NascaLotNo == existlot.NascaLotNo))
                        {
                            blend.Add(existlot);
                        }
                    }
                    blend.Add(lot);

                    isError = WorkChecker.IsCutBlendError(blend.ToArray(), out errMsg);
                    if (isError)
                    {
                        msg = errMsg;
                        return false;
                    }
                }

                msg = "";
                return true;
            }
            catch (Exception ex)
            {
                msg = "エラーが発生したため処理を中断しました " + ex.ToString();
                return false;
            }
        }

        // 次の条件を満たす場合、Trueを返す
        // ①装置にプロファイルが紐づいている
        // ②装置に紐づいている作業とプロファイルのタイプのフロー上の最初の作業番号が一致する。
        public bool IsFirstProcess(out int firstprocno)
        {
            firstprocno = int.MinValue;
            try
            {
                if (this.Mac == null) return false;

                Profile prof = Profile.GetCurrentDBProfile(this.Mac.MacNo);
                if (prof == null) return false;

                Process firstProcess = Process.GetFirstProcess(prof.TypeCd);
                List<string> macGroup = MachineInfo.GetMachine(this.Mac.MacNo).MacGroup;

                // ダイボンダーが最初の作業に紐づいている装置かどうか
                MachineInfo[] availableMachines = MachineInfo.GetAvailableMachines(firstProcess.ProcNo, macGroup);
                bool found = false;
                foreach (MachineInfo a in availableMachines)
                {
                    if (a.MacNo == this.Mac.MacNo)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }

                firstprocno = firstProcess.ProcNo;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
