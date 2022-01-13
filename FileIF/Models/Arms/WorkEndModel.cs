using System;
using System.Collections.Generic;
using System.Linq;
using ArmsApi.Model;
using ArmsApi;
using ArmsApi.Model.NASCA;

namespace FileIf
{
    public class _WorkEndAltModel
    {
        /// <summary>
        /// 移載先マガジン基板数量
        /// 2021.09.06 JunichiWatanabe 追加
        /// </summary>
        public int NewMagFrameQty { get; set; }


        public _WorkEndAltModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(plantcd);
            this.MagList = new List<Magazine>();
            this.BlendLotList = new Dictionary<string, AsmLot>();
            this.VirtualMags = new Dictionary<string, VirtualMag>();
            this.NeedInspectionWhenCompleteLotList = new List<string>();
            this.NewMagFrameQty = 0;
        }

        public void AddMagazine(ArmsApi.Model.Magazine mag, ArmsApi.Model.VirtualMag vmag)
        {
            ArmsApi.Model.AsmLot lot = ArmsApi.Model.AsmLot.GetAsmLot(mag.NascaLotNO);
            ArmsApi.Model.Process p = ArmsApi.Model.Process.GetNextProcess(mag.NowCompProcess, lot);
            if (this.Mac.MagazineChgFg == true)
            {
                this.IsNeedMagazineChange = true;
            }

            //高生産ラインのマガジン分割工程で自動搬送プレートを投入した場合は1ロットずつ完成させてUL側プレートの読み込みが必要
            Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, p.ProcNo);
            if (dst == Process.MagazineDevideStatus.SingleToDouble)
            {
                if (mag.MagazineNo != mag.NascaLotNO)
                {
                    this.IsNeedMagazineChange = true;
                }
            }


            this.MagList.Add(mag);


            //仮想マガジン記憶
            this.VirtualMags.Add(mag.NascaLotNO, vmag);
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

        /// <summary>
        /// 仮想マガジンリスト　キー=NascaLotNo
        /// </summary>
        public Dictionary<string, VirtualMag> VirtualMags { get; set; }

        /// <summary>
        /// 1:1完成時のみ
        /// UL側マガジンプレート情報
        /// </summary>
        public string UnloaderMagNo { get; set; }

        public bool IsNeedMagazineChange { get; set; }

        public List<string> NeedInspectionWhenCompleteLotList { get; set; }

        public bool NeedAutoInspectionNextProc { get; set; }

        public bool IsCompleteUnloaderMagazine { get; set; }

        public string Comment { get; set; }

        /// <summary>
        /// 全マガジンを完了
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool WorkEnd(out List<string> errMsg)
        {
            bool isSuccess = true;
            errMsg = new List<string>();
            Comment = "";

            foreach (Magazine mag in this.MagList)
            {
                //念のため最新情報取得
                ArmsApi.Model.Magazine mag2 = Magazine.GetCurrent(mag.MagazineNo);
                if (mag2 == null)
                {
                    //ブレンドされているロット、かつ最終工程以降の工程の完了の場合
                    //mag2には子ロットが入る
                    if (this.BlendLotList.ContainsKey(mag.MagazineNo) == true)
                    {
                        mag2 = mag;
                    }
                }

                if (mag2 == null)
                {
                    // commentout by juniwatanabe
                    //errMsg.Add("マガジン情報が見つかりません:" + mag.MagazineNo);
                    //isSuccess = false;
                    //continue;

                    // add code juniwatanabe
                    break;
                }

                string lotlog;
                bool lotSuccecc = endMag(mag2, out lotlog);
                if (!lotSuccecc)
                {
                    isSuccess = false;
                    errMsg.Add(lotlog);
                }

                if (string.IsNullOrWhiteSpace(Comment) == false) Comment += "\r\n";
                string outMagNo;
                if (this.IsNeedMagazineChange)
                {
                    outMagNo = this.UnloaderMagNo;
                }
                else
                {
                    outMagNo = mag.MagazineNo;
                }
                //完了後のマガジン情報を取得
                ArmsApi.Model.Magazine mag3 = Magazine.GetCurrent(outMagNo);
                //ブレンドされているロット、かつ最終工程以降の工程の完了の場合
                //mag2には子ロットが入る
                string lotno = "";
                string typeCd = "";
                if (this.BlendLotList.ContainsKey(mag.MagazineNo) == true)
                {
                    mag3 = new Magazine();
                    lotno = this.BlendLotList[mag.MagazineNo].NascaLotNo;
                    mag3.NascaLotNO = mag.MagazineNo;
                    mag3.MagazineNo = mag.MagazineNo;
                    mag3.NowCompProcess = Order.GetLastProcNoFromLotNo(mag3.NascaLotNO);
                    mag3.FrameQty = this.NewMagFrameQty;
                    typeCd = AsmLot.GetAsmLot(Order.MagLotToNascaLot(mag.MagazineNo)).TypeCd;
                }
                else
                {
                    lotno = mag3.NascaLotNO;
                    typeCd = AsmLot.GetAsmLot(Order.MagLotToNascaLot(mag3.NascaLotNO)).TypeCd;
                }
                Comment += $"マガジン：{mag3.MagazineNo}\r\n";
                Comment += $"ロット番号：{lotno}\r\n";
                Comment += $"タイプ：{typeCd}\r\n";
                string msg;
                string nextline = Magazine.GetNextLine(mag3, out msg);
                Comment += $"次ライン：{nextline}\r\n";
                if (string.IsNullOrWhiteSpace(msg) == false)
                {
                    Comment += $"          {msg}\r\n";
                }


                //////////////////////////////////////////////
                /// 2021.07.29 Junichi Watanabe 追加コード
                /// 
                MachineInfo mc = MachineInfo.GetMachine(this.PlantCd);
                List<VirtualMag> vmags = VirtualMag.GetVirtualMag(mc.MacNo, ((int)Station.EmptyMagazineLoader)).ToList();
                if (vmags.Count > 0) vmags[0].Delete();
                // ここまで
                //////////////////////////////////////////////

            }

            return isSuccess;
        }

        public List<Magazine> getUnloaderMag(string plantcd)
        {
            List<Magazine> retv = new List<Magazine>();

            MachineInfo m = MachineInfo.GetMachine(plantcd);
            List<VirtualMag> vmags = VirtualMag.GetVirtualMag(m.MacNo, ((int)Station.Loader)).ToList();

            /////////////////////////////////////////////////
            // 2021.07.28 Junichi Watanabe 追加コード
            if (vmags.Count == 1)
            {
                var vm = vmags[0];
                vm.CurrentLocation = new Location(m.MacNo, Station.Loader);
                vm.Dequeue(vm.CurrentLocation);
                vm.CurrentLocation.Station = Station.Unloader;
                vm.LastMagazineNo = vm.MagazineNo;
                vm.Enqueue(vm, vm.CurrentLocation);
            }
            // 追加コードここまで
            ////////////////////////////////////////////////


            vmags = VirtualMag.GetVirtualMag(m.MacNo, ((int)Station.Unloader)).ToList();

            foreach (VirtualMag vmag in vmags)
            {
                Magazine svrmag = Magazine.GetCurrent(vmag.MagazineNo);

                //ブレンドされているロット、かつ最終工程以降の工程の完了の場合
                CutBlend[] cbs = CutBlend.GetData(vmag.MagazineNo);
                if (cbs.Length > 0)
                {
                    AsmLot lot = AsmLot.GetAsmLot(cbs.First().LotNo);

                    int lastprocno = Order.GetLastProcNoFromLotNo(cbs.First().BlendLotNo);
                    Process prevprocess = Process.GetPrevProcess(lastprocno, lot.TypeCd);
                    Process nextprocess = Process.GetNextProcess(prevprocess.ProcNo, lot);

                    if (Process.IsFinalStAfterProcess(nextprocess, lot.TypeCd) == true)
                    {
                        svrmag = new Magazine();
                        svrmag.NascaLotNO = cbs.First().BlendLotNo;
                        svrmag.MagazineNo = vmag.MagazineNo;
                    }
                }
                if (svrmag == null) continue;

                retv.Add(svrmag);
            }

            return retv;
        }

        /// <summary>
        /// 作業完了前チェック
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private bool endMag(Magazine mag, out string msg)
        {
            try
            {

                string lotno = Order.MagLotToNascaLot(mag.NascaLotNO);
                //AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                AsmLot lot = AsmLot.GetAsmLot(lotno);

                if (lot == null)
                {
                    msg = "ロット情報が存在しません";
                    return false;
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
                    Order firstOrder = Order.GetMagazineOrder(lotno, p.ProcNo);
                    if (firstOrder.IsComplete == false)
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

                Order order = Order.GetMagazineOrder(mag.NascaLotNO, p.ProcNo);
                if (this.BlendLotList.ContainsKey(mag.MagazineNo) == true)
                {
                    order = Order.GetMagazineOrder(this.BlendLotList[mag.MagazineNo].NascaLotNo, p.ProcNo);
                }
                if (order == null)
                {
                    msg = "作業開始データが存在しません";
                    return false;
                }

                if (this.IsNeedMagazineChange)
                {
                    order.OutMagazineNo = this.UnloaderMagNo;
                }
                else
                {
                    order.OutMagazineNo = mag.MagazineNo;
                }

                order.MacNo = this.Mac.MacNo;
                VirtualMag vmag = VirtualMags[mag.NascaLotNO];
                if (vmag != null)
                {
                    order.WorkStartDt = vmag.WorkStart.Value;
                    order.WorkEndDt = vmag.WorkComplete;

                    if (vmag.WaferChangerChangeCount.HasValue)
                    {
                        order.StockerChangeCt = vmag.WaferChangerChangeCount.Value;
                    }
                    if (order.WorkEndDt.HasValue == false)
                    {
                        order.WorkEndDt = System.DateTime.Now;
                    }
                    if (this.Mac.HasDoubleStocker)
                    {
                        order.ScNo1 = vmag.Stocker1.ToString();
                        order.ScNo2 = vmag.Stocker2.ToString();
                    }
                    else if (this.Mac.HasWaferChanger)
                    {
                        if (vmag.StartWafer == null || vmag.EndWafer == null || vmag.WaferChangerChangeCount == null)
                        {
                            msg = "ウェハー開始/終了段数、チェンジャー交換回数の何れかが不正です";
                            return false;
                        }

                        order.ScNo1 = string.Format("0-{0}", vmag.StartWafer.Value);
                        order.ScNo2 = string.Format("{0}-{1}", vmag.WaferChangerChangeCount.Value, vmag.EndWafer.Value);
                    }
                }
                else
                {
                    order.WorkEndDt = DateTime.Now;
                }
                order.TranCompEmpCd = this.EmpCd;

                #region マガジンの入れ替わりチェック NGの場合は仮想マガジン、作業データは更新しない
                // マガジン変更チェック
                if (this.Mac.MagazineChgFg == true)
                {
                    if (order.InMagazineNo == order.OutMagazineNo)
                    {
                        throw new ApplicationException("搬入と搬出のマガジンに誤りがあります");
                    }

                    Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, order.ProcNo);
                    Magazine newmag = Magazine.GetCurrent(order.OutMagazineNo);
                    if (newmag != null && dst != Process.MagazineDevideStatus.DoubleToSingle)
                    {
                        throw new ApplicationException("搬出マガジンに、現在稼動中のロットがあります");
                    }
                }
                #endregion

                //完了時チェック前に資材割付を更新
                if (vmag != null && vmag.RelatedMaterials != null)
                {
                    order.UpdateMaterialRelation(vmag.RelatedMaterials.ToArray());
                }

                string errMsg;
                bool isError = WorkChecker.IsErrorWorkComplete(order, Mac, lot, out errMsg);

                if (isError)
                {
                    msg = errMsg + " ロットは完了しましたが警告状態になっています。";
                    lot.IsWarning = true;
                    lot.Update();
                    order.Comment += errMsg;
                    order.DeleteInsert(order.LotNo);

                    // インラインマガジンロット更新
                    mag = Magazine.ApplyMagazineInOut(order, order.LotNo);

                    if (vmag != null) vmag.Delete();

                    //状態検査必要フラグ
                    Inspection isp = Inspection.GetInspection(order.NascaLotNo, order.ProcNo);
                    if (isp != null && isp.IsInspected == false) NeedInspectionWhenCompleteLotList.Add(order.NascaLotNo);

                    Process nextproc = Process.GetNextProcess(mag.NowCompProcess, lot);
                    if (nextproc != null && nextproc.IsSamplingInspection)
                    {
                        //検査工程の抜き取り指示有無再取得
                        isp = Inspection.GetInspection(mag.NascaLotNO, nextproc.ProcNo);
                        if (isp != null)
                        {
                            this.NeedAutoInspectionNextProc = true;
                        }
                    }

                    //TODO ワイヤー号機の判定がワイヤーの文字で行われているLineConfigをDB化必要
                    if (this.Mac.ClassName.StartsWith("ワイヤー"))
                    {
                        VirtualMag evmag = new VirtualMag();
                        evmag.MagazineNo = order.OutMagazineNo;
                        evmag.LastMagazineNo = order.InMagazineNo;
                        evmag.ProcNo = order.ProcNo;

                        evmag.WorkStart = order.WorkStartDt;
                        evmag.WorkComplete = order.WorkEndDt;

                        //TODO 空マガジンローダーを使って擬似的にマッピングデータの集計を行っている。別の仕組みが必要
                        evmag.Enqueue(evmag, new Location(this.Mac.MacNo, Station.EmptyMagazineLoader));
                    }

                    if (firstProcessFg == true)
                    {
                        // ダイボンドでLotのProfileを更新する為、ここではNASCA連携しない
                        if (Config.Settings.IsDBProfileRequireMode == false)
                        {
                            int orderMove = Process.GetOrderMove(lot.TypeCd, firstProc.ProcNo);
                            Order firstOrder = Order.GetMagazineOrder(lotno, firstProc.ProcNo);
                            if (orderMove == 1 && firstOrder.IsNascaStart == false)
                            {
                                //期間中の原材料、ロット情報を更新
                                Exporter exp = Exporter.GetInstance();
                                NASCAResponse res = exp.SendOrderToNasca(lot, firstProc);
                            }
                        }
                    }

                    return false;
                }
                else
                {
                    order.DeleteInsert(order.LotNo);

                    if (this.BlendLotList.ContainsKey(mag.MagazineNo) == false)
                    {
                        // インラインマガジンロット更新
                        mag = Magazine.ApplyMagazineInOut(order, order.LotNo);
                    }
                    msg = "";

                    Process nextproc = Process.GetNextProcess(order.ProcNo, lot);
                    //次工程が抜き取り検査工程の場合
                    if (nextproc != null && nextproc.IsSamplingInspection == true)
                    {
                        //検査工程の抜き取り指示有無再取得
                        Inspection isp = Inspection.GetInspection(lot.NascaLotNo, nextproc.ProcNo);
                        if (isp == null)
                        {
                            //抜き取りフラグが無い場合は、ダミー作業を登録
                            Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag);
                        }
                        else
                        {
                            this.NeedAutoInspectionNextProc = true;
                        }
                    }

                    // FILEIF統合対応 MEL_MAP ⇒ NEL_MAP対応  装置のhighlineフラグも参照する
                    MachineInfo macInfo = MachineInfo.GetMachine(this.Mac.MacNo);

                    // 本工程が分割工程、MAP高生産性ライン、作業順に遠心沈降作業が無い場合、#2のレコードをダミー登録
                    Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, order.ProcNo);
                    //if (nextproc != null && dst == Process.MagazineDevideStatus.SingleToDouble && Config.GetLineType == Config.LineTypes.MEL_MAP)
                    if (nextproc != null && dst == Process.MagazineDevideStatus.SingleToDouble
                        && (Config.GetLineType == Config.LineTypes.MEL_MAP || (Config.GetLineType == Config.LineTypes.NEL_MAP && macInfo.IsHighLine == true)))
                    {
                        // 遠心沈降作業が無い品種のみ対応
                        List<Process> workflow = Process.GetWorkFlow(lot.TypeCd).ToList();
                        if (Config.Settings.EckWorkCdList != null && workflow.Exists(w => Config.Settings.EckWorkCdList.Contains(w.WorkCd)) == false)
                        {
                            // #2のデータを取得(TnTran)
                            Order doubleOrder = order;
                            doubleOrder.DevidedMagazineSeqNo = 2;

                            // #2のデータを取得(TnMag)
                            Magazine[] doubleMags = Magazine.GetMagazine(doubleOrder.LotNo, true);
                            Magazine doubleMag = null;
                            if (doubleMags != null && doubleMags.Length != 0)
                            {
                                doubleMag = doubleMags[0];
                            }

                            // 高生産性用プレート使用の場合は、TnTran.outmagを「ロット_#1」⇒「ロット_#2」に変更
                            // 自動搬送用プレートの場合は、「Mxxxxx」なので変更不要
                            if (!this.IsNeedMagazineChange)
                            {
                                doubleOrder.OutMagazineNo = doubleMag.MagazineNo;
                            }

                            // #2の稼働中マガジンがある場合のみダミー登録実施
                            if (doubleMag != null)
                            {
                                // #2のMD作業実績を作業中 ⇒ 作業完了にする
                                doubleOrder.DeleteInsert(doubleOrder.LotNo);
                                // 自動搬送用プレート使用： magno = 「Mxxxx_#2」のレコードの稼働中フラグを外す。
                                doubleMag.NowCompProcess = doubleOrder.ProcNo;
                                doubleMag.NewFg = false;
                                doubleMag.Update();
                                // 自動搬送用プレート使用： magno = 「Mxxxx」のレコードを作成
                                doubleMag.MagazineNo = doubleOrder.OutMagazineNo;
                                doubleMag.NewFg = true;
                                doubleMag.Update();

                                // 次工程がマガジン交換前待機作業の場合、ダミー登録 (#1, #2両方とも)
                                if (nextproc != null && Process.HasMagazineChangeBufferWork(nextproc.WorkCd))
                                {
                                    Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag);
                                    Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, doubleMag);
                                    nextproc = Process.GetNextProcess(nextproc.ProcNo, lot);
                                }

                                // 次工程がマガジン交換作業の場合、ダミー登録  ※マガジン情報も統合後にする
                                if (nextproc != null && Process.HasMagazineChangeWork(nextproc.WorkCd))
                                {
                                    Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag, doubleMag);
                                    nextproc = Process.GetNextProcess(nextproc.ProcNo, lot);
                                }
                            }
                        }
                    }

                    // 次工程が移載前待機、MAP高生産性ラインの場合、ダミー登録
                    //if (nextproc != null && Process.HasMagazineChangeBufferWork(nextproc.WorkCd) && Config.GetLineType == Config.LineTypes.MEL_MAP)
                    if (nextproc != null && Process.HasMagazineChangeBufferWork(nextproc.WorkCd)
                        && (Config.GetLineType == Config.LineTypes.MEL_MAP || (Config.GetLineType == Config.LineTypes.NEL_MAP && macInfo.IsHighLine == true)))
                    {
                        Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag);
                        nextproc = Process.GetNextProcess(nextproc.ProcNo, lot);
                    }

                    // 次工程がマガジン交換作業、MAP高生産性ライン、作業順に遠心沈降作業が無い場合、ダミー登録
                    //if (nextproc != null && Process.HasMagazineChangeWork(nextproc.WorkCd) && Config.GetLineType == Config.LineTypes.MEL_MAP)
                    if (nextproc != null && Process.HasMagazineChangeWork(nextproc.WorkCd)
                        && (Config.GetLineType == Config.LineTypes.MEL_MAP || (Config.GetLineType == Config.LineTypes.NEL_MAP && macInfo.IsHighLine == true)))
                    {
                        List<Process> workflow = Process.GetWorkFlow(lot.TypeCd).ToList();
                        if (Config.Settings.EckWorkCdList != null && workflow.Exists(w => Config.Settings.EckWorkCdList.Contains(w.WorkCd)) == false)
                        {
                            Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag);
                        }
                    }

                    // 次工程がロットマーキング作業、先行色調品、作業マスタにダミー設備マスタが登録されている場合、ダミー登録
                    if (nextproc != null && nextproc.IsLotMarking)
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

                    if (vmag != null) vmag.Delete();

                    //TODO ワイヤー号機の判定がワイヤーの文字で行われているLineConfigをDB化必要
                    if (this.Mac.ClassName.StartsWith("ワイヤー"))
                    {
                        VirtualMag evmag = new VirtualMag();
                        evmag.MagazineNo = order.OutMagazineNo;
                        evmag.LastMagazineNo = order.InMagazineNo;
                        evmag.ProcNo = order.ProcNo;

                        evmag.WorkStart = order.WorkStartDt;
                        evmag.WorkComplete = order.WorkEndDt;

                        //TODO 空マガジンローダーを使って擬似的にマッピングデータの集計を行っている。別の仕組みが必要
                        evmag.Enqueue(evmag, new Location(this.Mac.MacNo, Station.EmptyMagazineLoader));
                    }
                    //状態検査必要フラグ
                    Inspection isp2 = Inspection.GetInspection(order.NascaLotNo, order.ProcNo);
                    if (isp2 != null && isp2.IsInspected == false) this.NeedInspectionWhenCompleteLotList.Add(order.NascaLotNo);

                    //SLS1の圧縮成型機(セミオート)を想定 仮想マガジンを作成して、本体の完了処理のトリガとする
                    if (this.Mac.RequestStartEndFg)
                    {
                        VirtualMag eulmag = new VirtualMag();
                        eulmag.MagazineNo = order.OutMagazineNo;
                        eulmag.LastMagazineNo = order.InMagazineNo;
                        eulmag.WorkStart = order.WorkStartDt;
                        eulmag.ProcNo = order.ProcNo;

                        eulmag.Enqueue(eulmag, new Location(this.Mac.MacNo, Station.EmptyMagazineUnloader));
                    }

                    if (firstProcessFg == true)
                    {
                        // ダイボンドでLotのProfileを更新する為、ここではNASCA連携しない
                        if (Config.Settings.IsDBProfileRequireMode == false)
                        {
                            int orderMove = Process.GetOrderMove(lot.TypeCd, firstProc.ProcNo);
                            Order firstOrder = Order.GetMagazineOrder(lotno, firstProc.ProcNo);
                            if (orderMove == 1 && firstOrder.IsNascaStart == false)
                            {
                                //期間中の原材料、ロット情報を更新
                                Exporter exp = Exporter.GetInstance();
                                NASCAResponse res = exp.SendOrderToNasca(lot, firstProc);
                            }
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = "エラーが発生したため処理を中断しました " + ex.ToString();
                return false;
            }
        }
    }
}
