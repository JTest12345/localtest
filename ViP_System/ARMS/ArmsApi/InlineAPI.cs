using ArmsApi.Model;
using ArmsApi.Model.NASCA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi
{
    public class InlineAPI
    {
        #region NelInput ARMSからの入力受付

        /// <summary>
        /// ARMSからの入力受付
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public ArmsApiResponse NelInput(string command)
        {
            Log.SysLog.Info("in:" + command);

            string[] cmd = command.Split(',');

            try
            {
                if (cmd.Length == 6 && (cmd[1] == "FAIL" || cmd[1] == "FAIL2"))
                {
                    ArmsApiResponse info = parseFailJob(cmd);
                    return info;
                }
                else if (cmd[1] == "MAPDB")
                {

                    MAPFrameLoadOrder ord = parseMAPFrameLoadOrder(cmd);

                    ArmsApiResponse info = NelCommonProcess(ord);
                    Log.SysLog.Info("out:" + info.ToString());
                    return info;
                }
                else
                {
                    Order ord = parseNormalStartEnd(cmd);

                    //CarrierInfo carrier = Route.GetReachable(new Location(ord.MacNo, Station.Loader));
                    //ord.LineNo = carrier.CarNo.ToString().Substring(carrier.CarNo.ToString().Length - 2, 2);

                    // Job-Shopライン対応, データベース上の『lineno』列の末2文字を採用
                    MachineInfo svrmac = MachineInfo.GetMachine(ord.MacNo);
                    ord.LineNo = svrmac.LineNo.Substring(svrmac.LineNo.Length -2, 2);

                    ArmsApiResponse info = NelCommonProcess(ord);
                    Log.SysLog.Info("out:" + info.ToString());
                    return info;
                }
            }
            catch (ArmsException aex)
            {
                return ArmsApiResponse.Create(true, aex.Message, 0, "", 0, "");
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("コマンド処理中エラー発生 不明エラー応答" + ex.ToString());
                return ArmsApiResponse.Create(true, ex.Message, 0, "", 0, "");
            }
        }
        #endregion
   
        #region parseFailJob

        /// <summary>
        /// 不良登録JOB
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private ArmsApiResponse parseFailJob(string[] cmd)
        {
            try
            {
                int procno = int.Parse(cmd[2]);
                string magno = cmd[3].Replace(AsmLot.PREFIX_INLINE_MAGAZINE, string.Empty);
                Magazine mag = Magazine.GetCurrent(magno);
                if (mag == null)
                {
                    throw new ArmsException("マガジン情報が不正です:" + magno);
                }
                string orgfailstring = cmd[5];

                if (cmd[1] == "FAIL")
                {
                    #region 該当ロットの不良全クリアして新規登録
                    
                    Defect defect = new Defect();
                    defect.LotNo = mag.NascaLotNO;
                    defect.ProcNo = procno;
                    defect.DefItems = new List<DefItem>();

                    string[] deflines = orgfailstring.Split(':');
                    foreach (string line in deflines)
                    {
                        string[] def = line.Split('@');
                        if (def.Length != 4)
                        {
                            throw new ArmsException("不良登録ファイルの内容が不正です");
                        }

                        DefItem item = new DefItem();
                        item.CauseCd = def[0];
                        item.ClassCd = def[1];
                        item.DefectCd = def[2];
                        item.DefectCt = int.Parse(def[3]);
                        defect.DefItems.Add(item);
                    }

                    Log.SysLog.Info("不良登録:" + mag.NascaLotNO + ":" + procno.ToString());
                    defect.DeleteInsert();

                    return ArmsApiResponse.Create(false,
                        "不良登録完了:" + mag.NascaLotNO + ":" + procno.ToString(), procno, "", 0, "");
                    #endregion
                }
                else if (cmd[1] == "FAIL2")
                {
                    #region 該当ロットの不良取得して加算

                    Defect defdata = Defect.GetDefect(mag.NascaLotNO, procno);

                    string[] deflines = orgfailstring.Split(':');
                    foreach (string line in deflines)
                    {
                        string[] def = line.Split('@');
                        if (def.Length != 4)
                        {
                            throw new ArmsException("不良登録ファイルの内容が不正です");
                        }

                        DefItem item = new DefItem();
                        item.CauseCd = def[0];
                        item.ClassCd = def[1];
                        item.DefectCd = def[2];
                        item.DefectCt = int.Parse(def[3]);

                        bool exists = false;
                        foreach (DefItem exd in defdata.DefItems)
                        {
                            if (exd.CauseCd == item.CauseCd && exd.ClassCd == item.ClassCd && exd.DefectCd == item.DefectCd)
                            {
                                exd.DefectCt += item.DefectCt;
                                exists = true;
                            }
                        }
                        if (!exists)
                        {
                            defdata.DefItems.Add(item);
                        }
                    }

                    Log.SysLog.Info("不良登録:" + mag.NascaLotNO + ":" + procno.ToString());
                    defdata.DeleteInsert();

                    return ArmsApiResponse.Create(false,
                        "不良登録完了:" + mag.NascaLotNO + ":" + procno.ToString(), procno, "", 0, "");
                    #endregion
                }

                throw new ArmsException("不良登録失敗　区分が不正です:" + cmd[1]);
            }
            catch (ArmsException ex)
            {
                return ArmsApiResponse.Create(true, ex.Message,  0, "", 0, "");
            }
            catch (Exception ex)
            {
                throw new ArmsException("不良登録中に不明なエラーが発生しました" + ex.Message, ex);
            }
        }
        #endregion

        #region parseMAPFrameLoadOrder

        private MAPFrameLoadOrder parseMAPFrameLoadOrder(string[] cmd)
        {
            try
            {
                MAPFrameLoadOrder ord = new MAPFrameLoadOrder();
                ord.WorkId = cmd[2].Trim();
                ord.InMagazineNo = cmd[5].Trim();

                if (ord.WorkId != Config.WORKID_QUERY && ord.WorkId != Config.WORKID_NEXT_PROCESS)
                {
                    ord.ProcNo = int.Parse(cmd[3].Trim());
                    ord.MacNo = int.Parse(cmd[4].Trim());
                    ord.OutMagazineNo = cmd[6].Trim();
                    ord.WorkStartDt = DateTime.Parse(cmd[7].Trim());

                    if (string.IsNullOrEmpty(cmd[8].Trim()) != true)
                    {
                        ord.WorkEndDt = DateTime.Parse(cmd[8].Trim());
                    }
                    else
                    {
                        ord.WorkEndDt = null;
                    }

                    ord.ScNo1 = cmd[9].Trim();
                    ord.ScNo2 = cmd[10].Trim();

                    List<Material> frames = new List<Material>();

                    string rawFrame = cmd[13].Trim();

                    if (!string.IsNullOrEmpty(rawFrame))
                    {
                        string[] deflines = rawFrame.Split(':');

                        foreach (string line in deflines)
                        {
                            string[] data = line.Split('@');
                            if (data.Length != 2)
                            {
                                throw new ArmsException("MAP搭載完了ファイルの内容が不正です");
                            }

                            string matcd = data[0];
                            string lotno = data[1];

                            Material mat = Material.GetMaterial(matcd, lotno);
                            frames.Add(mat);
                        }
                    }

                    if (cmd.Length >= 15)
                    {
                        if (cmd[14] == "1")
                        {
                            ord.IsFrameBakingTimeOver = true;
                        }
                    }

                    ord.FrameList = frames.ToArray();
                }

                return ord;
            }
            catch (Exception ex)
            {
                throw new ArmsException("DataSpiderコマンドエラー", ex);
            }
        }
        #endregion

        #region parseNormalStartEnd

        private Order parseNormalStartEnd(string[] cmd)
        {
            try
            {
                Order ord = new Order();
                ord.WorkId = cmd[2].Trim();
                ord.InMagazineNo = cmd[5].Trim();

                if (ord.WorkId != Config.WORKID_QUERY && ord.WorkId != Config.WORKID_NEXT_PROCESS)
                {
                    ord.ProcNo = int.Parse(cmd[3].Trim());
                    ord.MacNo = int.Parse(cmd[4].Trim());
                    ord.OutMagazineNo = cmd[6].Trim();
                    ord.WorkStartDt = DateTime.Parse(cmd[7].Trim());

                    if (string.IsNullOrEmpty(cmd[8].Trim()) != true)
                    {
                        ord.WorkEndDt = DateTime.Parse(cmd[8].Trim());
                    }
                    else
                    {
                        ord.WorkEndDt = null;
                    }

                    ord.ScNo1 = cmd[9].Trim();
                    ord.ScNo2 = cmd[10].Trim();
                }

                return ord;
            }
            catch (Exception ex)
            {
                throw new ArmsException("DataSpiderコマンドエラー", ex);
            }
        }
        #endregion

        #region [NEL]共通処理メソッド
        /// <summary>
        /// [NEL]共通処理メソッド
        /// </summary>
        /// <param name="paramInfo">条件</param>
        /// <param name="valInfo"></param>
        /// <returns>戻り用構造体</returns>
        public ArmsApiResponse NelCommonProcess(Order paramInfo)
        {
            // プレフィックス(30)があれば削除
            if (string.IsNullOrEmpty(paramInfo.InMagazineNo) == false)
            {
                paramInfo.InMagazineNo = paramInfo.InMagazineNo.Replace(AsmLot.PREFIX_INLINE_MAGAZINE, string.Empty);
            }
            if (string.IsNullOrEmpty(paramInfo.OutMagazineNo) == false)
            {
                paramInfo.OutMagazineNo = paramInfo.OutMagazineNo.Replace(AsmLot.PREFIX_INLINE_MAGAZINE, string.Empty);
            }

            switch (paramInfo.WorkId)
            {
                case Config.WORKID_WORK_START:		// [001]作業開始
                    //return WorkStart(paramInfo);


                //case Config.WORKID_NEXT_PROCESS:		// [002]途中品
                //    return CheckNextProcess(paramInfo.InMagazineNo, paramInfo.WorkId, paramInfo);


                case Config.WORKID_FRAME:				// [011]フレーム搭載作業
                case Config.WORKID_RESULT_NO_TIME:	// [014]時間管理無の実績登録
                case Config.WORKID_DIE_BONDING:		// [012]ダイボンド作業
                    return CommonApi.WorkEnd(paramInfo);
                    //return WorkEnd(paramInfo);


                //case Config.WORKID_QUERY:				// [051]作業問合せ
                //    return QueryNextWork(paramInfo);

                case Config.WORKID_START_COMPLT:		// [021]作業開始完了
                    //return WorkStartEnd(paramInfo);

                default:
                    throw new ArmsException("NW0008 引数に無効な値があります");
            }
        }
        #endregion

        #region [NEL]インライン用作業開始/完了を使って作業開始完了
        /// <summary>
        /// [NEL]インライン用作業開始/完了を使って作業開始完了
        /// </summary>
        /// <param name="nelParamInfo"></param>
        /// <param name="valInfo"></param>
        /// <returns></returns>
        //public ArmsSvrReturnInfo WorkStartEnd(Order workinfo)
        //{
        //    // 作業開始
        //    workinfo.WorkId = Config.WORKID_WORK_START;
        //    WorkStart(workinfo);

        //    // 作業終了
        //    workinfo.WorkId = Config.WORKID_RESULT_NO_TIME;
        //    ArmsSvrReturnInfo ret = WorkEnd(workinfo);

        //    workinfo.WorkId = Config.WORKID_START_COMPLT;

        //    return ret;
        //}
        #endregion

        //#region [NEL] 次作業問合せAPI QueryNextWork

        ///// <summary>
        ///// 次作業問合せAPI
        ///// </summary>
        ///// <param name="order"></param>
        ///// <returns></returns>
        //public ArmsSvrReturnInfo QueryNextWork(Order order)
        //{
        //    Magazine mag = Magazine.GetCurrent(order.InMagazineNo);
        //    if (mag == null)
        //    {
        //        return ArmsSvrReturnInfo.GetNgReturnData("マガジン情報が存在しません", "", order.ProcNo);
        //    }
        //    AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
        //    if (lot == null)
        //    {
        //        //ロットが見つからない場合はNG
        //        throw new ArmsException("マガジンの入力が誤っています:" + mag.MagazineNo);
        //    }

        //    ArmsSvrReturnInfo retv = new ArmsSvrReturnInfo();

        //    //出荷規制判定（現在完了工程が規制工程なら排出行き）
        //    string reason;
        //    if (WorkChecker.IsRestricted(lot.NascaLotNo, mag.NowCompProcess, out reason))
        //    {
        //        retv.ProcNo = order.ProcNo;
        //        retv.NextMachines = new MachineInfo[0];
        //        retv.Message = reason;
        //        return retv;
        //    }


        //    Process nextproc = Process.GetNextProcess(mag.NowCompProcess, lot);
        //    //次工程が抜き取り検査工程の場合
        //    if (nextproc.IsSamplingInspection == true)
        //    {
        //        //検査工程の抜き取り指示有無再取得
        //        Inspection isp = Inspection.GetInspection(lot.NascaLotNo, nextproc.ProcNo);

        //        //抜き取りフラグがある場合は、検査有無に関わらず検査工程へ
        //        //作業履歴削除時にも再度検査されるように
        //        if (isp == null)
        //        {
        //            //抜き取りフラグが無い場合は、ダミー作業を登録
        //            Order.RecordDummyWork(lot, nextproc.ProcNo, nextproc.AutoUpdMachineNo.Value, mag);
        //            nextproc = Process.GetNextProcess(nextproc.ProcNo, lot);
        //        }
        //    }

        //    if (nextproc == null)
        //    {
        //        retv.ProcNo = order.ProcNo;
        //        retv.NextMachines = new MachineInfo[0];
        //    }
        //    else
        //    {
        //        MachineInfo[] nextMachines = MachineInfo.GetAvailableMachines(nextproc.ProcNo);
        //        retv.ProcNo = nextproc.ProcNo;
        //        retv.NextMachines = nextMachines;
        //    }

        //    return retv;
        //}
        //#endregion

        #region [NEL] 作業開始API　WorkStart

        /// <summary>
        /// 作業開始API
        /// </summary>
        /// <param name="startInfo"></param>
        /// <returns></returns>
        //public ArmsSvrReturnInfo WorkStart(Order startInfo)
        //{
        //    // ログ出力（処理開始）
        //    Log.SysLog.Info("[api] EditWorkStart start:" + startInfo.InMagazineNo);

        //    try
        //    {
        //        // 各マスタ取得
        //        MachineInfo machine = MachineInfo.GetMachine(startInfo.MacNo);
        //        Process process = Process.GetProcess(startInfo.ProcNo);

        //        #region マガジン内容(インライン装置側の起因で排出)の値があればログ出力を行い処理を終了させる
        //        if (AsmLot.HasErrorStatus(startInfo))
        //        {
        //            Log.SysLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
        //            return ArmsSvrReturnInfo.GetReturnData(ResStatus.OK, "装置でエラーが発生して排出されました", startInfo.ProcNo);
        //        }
        //        #endregion

        //        #region 整合チェック

        //        // マガジンNO
        //        if (string.IsNullOrEmpty(startInfo.InMagazineNo))
        //        {
        //            throw new ArmsException("NW0286:マガジンの入力が誤っています");
        //        }

        //        // 時間
        //        if (startInfo.WorkStartDt == DateTime.MinValue)
        //        {
        //            throw new ArmsException("NW0123:開始日時が入力されていません");
        //        }

        //        // 工程マスタに品目と作業が紐づいているか
        //        if (machine == null || process == null)
        //        {
        //            return ArmsSvrReturnInfo.GetNgReturnData("設備、工程マスタに不備があります", "", startInfo.ProcNo);
        //        }


        //        #endregion


        //        // 最初の工程（搭載）の場合
        //        if (process.FirstSt == true)
        //        {
        //            return workStartWithFirstOrder(startInfo);
        //        }


        //        // 現在の指図情報を取得する
        //        Magazine mag = Magazine.GetCurrent(startInfo.InMagazineNo);
        //        AsmLot lot = mag != null ? AsmLot.GetAsmLot(mag.NascaLotNO) : null;
        //        if (mag == null || lot == null)
        //        {
        //            return ArmsSvrReturnInfo.GetNgReturnData("ロット情報が見つかりません", "", startInfo.ProcNo);
        //        }

        //        // ロットステータスが「警告」か
        //        if (lot.IsWarning)
        //        {
        //            // ログ出力（処理終了）
        //            Log.SysLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
        //            return ArmsSvrReturnInfo.GetNgReturnData("このロットは移動できません", lot.NascaLotNo, startInfo.ProcNo);
        //        }

        //        if (string.IsNullOrEmpty(startInfo.LotNo)) startInfo.LotNo = mag.NascaLotNO;



        //        //作業開始登録
        //        if (process.FinalSt == true)
        //        {
        //            //　開始前の各種特性チェック、原材料チェック
        //            string msg;
        //            bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, startInfo, process, out msg);
        //            if (isError)
        //            {
        //                return ArmsSvrReturnInfo.GetNgReturnData(msg, lot.NascaLotNo, startInfo.ProcNo);
        //            }

        //            //最終工程の場合はブレンド
        //            CutBlend.InputAsmLot(startInfo);
        //        }
        //        else
        //        {
        //            startInfo.LotNo = mag.NascaLotNO;
        //            Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, startInfo.ProcNo);
        //            if (dst == Process.MagazineDevideStatus.SingleToDouble)
        //            {
        //                //分割発生工程の開始は2マガジン分の開始を登録する

        //                //マガジンのロット番号を分割番号付きに更新
        //                startInfo.DevidedMagazineSeqNo = 1;

        //                //　開始前の各種特性チェック、原材料チェック
        //                string msg;
        //                bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, startInfo, process, out msg);
        //                if (isError)
        //                {
        //                    return ArmsSvrReturnInfo.GetNgReturnData(msg, lot.NascaLotNo, startInfo.ProcNo);
        //                }

        //                //作業開始登録（1マガジン目）
        //                startInfo.DeleteInsert(startInfo.LotNo);
        //                mag.NascaLotNO = startInfo.LotNo;
        //                mag.Update();

        //                //マガジンのロット番号を分割番号付きに更新
        //                startInfo.DevidedMagazineSeqNo = 2;

        //                //作業開始登録（２マガジン目）
        //                startInfo.DeleteInsert(startInfo.LotNo);
        //                mag.NascaLotNO = startInfo.LotNo;
        //                mag.Update();

        //                //分割前ロット番号の関連付け解除
        //                mag.NascaLotNO = Order.MagLotToNascaLot(startInfo.LotNo);
        //                mag.NewFg = false;
        //                mag.Update();

        //                //magの情報を1マガジン目に戻す（今後、↓に処理を追加する際の保険）
        //                startInfo.DevidedMagazineSeqNo = 1;
        //                mag.NascaLotNO = startInfo.LotNo;
        //                mag.NewFg = true;
        //            }
        //            else if (dst == Process.MagazineDevideStatus.DoubleToSingle)
        //            {
        //                //開始前の各種特性チェック、原材料チェック
        //                string msg;
        //                bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, startInfo, process, out msg);
        //                if (isError)
        //                {
        //                    return ArmsSvrReturnInfo.GetNgReturnData(msg, lot.NascaLotNo, startInfo.ProcNo);
        //                }

        //                //元のロット番号の関連付け削除
        //                mag.NewFg = false;
        //                mag.Update();

        //                //纏め発生工程では作業レコードを一つにする
        //                startInfo.DevidedMagazineSeqNo = 0;

        //                //マガジンのロット番号を分割番号なしに更新
        //                mag.NewFg = true;
        //                mag.NascaLotNO = startInfo.LotNo;
        //                mag.Update();

        //                //作業開始登録
        //                startInfo.DeleteInsert(startInfo.LotNo);
        //            }
        //            else
        //            {
        //                //　開始前の各種特性チェック、原材料チェック
        //                string msg;
        //                bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, startInfo, process, out msg);
        //                if (isError)
        //                {
        //                    return ArmsSvrReturnInfo.GetNgReturnData(msg, lot.NascaLotNo, startInfo.ProcNo);
        //                }

        //                //作業開始登録
        //                startInfo.DeleteInsert(startInfo.LotNo);
        //            }

        //        }

        //        // 最終工程の場合マガジンとロット切り離し
        //        if (process.FinalSt == true)
        //        {
        //            Magazine.UpdateNewFgOff(startInfo.InMagazineNo);
        //        }

        //        // ログ出力（処理終了）
        //        Log.SysLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
        //        return ArmsSvrReturnInfo.GetReturnData(ResStatus.OK, process.InlineProNM + "を開始しました", lot.NascaLotNo, startInfo.ProcNo); ;

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.SysLog.Error("作業開始登録エラー " + ex.ToString());
        //        throw (ex);
        //    }

        //}

        #region workStartWithFirstOrder

        private ArmsSvrReturnInfo workStartWithFirstOrder(Order startInfo)
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

                if (Config.Settings.UseDBProfile == false)
                {
                    prof = Profile.GetCurrentProfile();
                }
                else
                {
                    prof = Profile.GetCurrentDBProfile(startInfo.MacNo);
                }


                if (prof == null)
                {
                    Log.SysLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
                    return ArmsSvrReturnInfo.GetNgReturnData("使用可能なプロファイルが存在しません", "", startInfo.ProcNo);
                }

                BOM[] bom = Profile.GetBOM(prof.ProfileId);

                //2012/11/13 sasaki------------------------------------------------------[start]
                AsmLot asmLot = new AsmLot();
                asmLot.TypeCd = prof.TypeCd;
                asmLot.ProfileId = prof.ProfileId;

                bool isBomError = WorkChecker.IsBomError(matlist, bom, out errMsg, asmLot, process.WorkCd, machine.MacNo);

                //bool isBomError = WorkChecker.IsBomError(matlist, bom, out errMsg);
                //------------------------------------------------------------------------[end]

                if (!isBomError)
                {
                    Log.SysLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
                    return ArmsSvrReturnInfo.GetReturnData(ResStatus.OK, process.InlineProNM + "を開始しました(登録なし)", startInfo.ProcNo);
                }
                else
                {
                    Log.SysLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
                    return ArmsSvrReturnInfo.GetNgReturnData(errMsg, "", startInfo.ProcNo);
                }
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
                return ArmsSvrReturnInfo.GetNgReturnData(msg, lot.NascaLotNo, startInfo.ProcNo);
            }

            //作業開始登録
            startInfo.DeleteInsert(lot.NascaLotNo);



            // ログ出力（処理終了）
            Log.SysLog.Info("[api] EditWorkStart end:" + startInfo.InMagazineNo);
            return ArmsSvrReturnInfo.GetReturnData(ResStatus.OK, process.InlineProNM + "を開始しました", lot.NascaLotNo, startInfo.ProcNo); ;
            #endregion

        }

        #endregion



        #endregion

        #region [NEL] 作業終了登録 WorkEnd
        /// <summary>
        /// 作業終了登録
        /// </summary>
        public ArmsSvrReturnInfo WorkEnd(Order order)
        {
            // ログ出力（処理開始）
            Log.SysLog.Info("[api] EditWorkEnd start:" + order.InMagazineNo);

            try
            {
                Process process = Process.GetProcess(order.ProcNo);

                // 指図発行区分=(4:作業終了時に現指図発行)の場合は別メソッドで処理
                if (process.FirstSt == true)
                {
                    return workEndWithFirstOrder(order);
                }

                // 各マスタ取得
                MachineInfo machine = MachineInfo.GetMachine(order.MacNo);
                Magazine mag = Magazine.GetCurrent(order.InMagazineNo);
                if (mag == null)
                {
                    throw new ArmsException("現在稼働フラグの無いマガジンです:" + order.InMagazineNo);
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
                    return ArmsSvrReturnInfo.GetReturnData(ResStatus.OK, "", order.ProcNo);
                }
                else
                {
                    // 設備CD

                    if (machine == null || string.IsNullOrEmpty(machine.NascaPlantCd))
                    {
                        throw new ArmsException("NW0036：この設備はマスターに登録されていません");
                    }
                }
                #endregion

                #region 開始・終了日、マガジン入れ替わりチェック


                // マガジン変更チェック
                if (machine.MagazineChgFg == true)
                {
                    if (order.InMagazineNo == order.OutMagazineNo)
                    {
                        throw new ArmsException("NW0652：搬入と搬出のマガジンに誤りがあります");
                    }
                }

                // 作業開始時間
                if (order.WorkStartDt == DateTime.MinValue)
                {
                    throw new ArmsException("NW0123：開始日時が入力されていません");
                }

                // 作業完了時間
                if (order.WorkEndDt.HasValue == false)
                {
                    throw new ArmsException("NW0124：完了日時が入力されていません");
                }

                //終了時間が開始時間より後かチェック
                if (order.WorkEndDt.Value < order.WorkStartDt)
                {
                    throw new ArmsException("開始・完了時間が不正です");
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
                    if (isSkipped == true) throw new ArmsException("前作業の実績が入力されていません");
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
                        if (isSkipped == true) throw new ArmsException("前作業の実績が入力されていません");
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
                }

                // NGでも作業は完了させるのでチェック類は全て後で行う
                // 作業終了登録
                order.DeleteInsert(order.LotNo);

                // インラインマガジンロット更新
                mag = Magazine.ApplyMagazineInOut(order, order.LotNo);

                // ロットステータスが「警告」の場合
                if (lot.IsWarning)
                {
                    return ArmsSvrReturnInfo.GetNgReturnData("このロットは移動できません", lot.NascaLotNo, order.ProcNo);
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

                    return ArmsSvrReturnInfo.GetNgReturnData(msg, lot.NascaLotNo, order.ProcNo);
                }

                // ログ出力（処理終了）
                Log.SysLog.Info("[api] EditWorkEnd end:" + order.InMagazineNo + " >> " + order.OutMagazineNo);
                return ArmsSvrReturnInfo.GetReturnData(ResStatus.OK, process.InlineProNM + "を完了しました", order.ProcNo);
            }
            catch (ArmsException aex)
            {
                throw aex;
            }
            catch (Exception ex)
            {
                // ログ出力（処理異常終了）
                throw new ArmsException("完了登録時に不明なエラーが発生しました" + ex.ToString());
            }

        }


        #region 作業完了+指図発行 workEndWithFirstOrder
        /// <summary>
        /// 作業完了+指図発行
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private ArmsSvrReturnInfo workEndWithFirstOrder(Order order)
        {
            MachineInfo machine = MachineInfo.GetMachine(order.MacNo);
            Magazine mag = Magazine.GetCurrent(order.OutMagazineNo);
            if (mag != null)
            {
                if (mag.NascaLotNO != mag.MagazineNo)
                {
                    throw new ArmsException("NW0650：搬出マガジンに、現在稼動中のロットがあります");
                }
                else
                {
                    //ロット=マガジン番号のマガジンの場合は稼働フラグOFF
                    Magazine.UpdateNewFgOff(mag.MagazineNo);
                }
            }


            //TempLotはNASCA側がロット採番していた名残。現在は必要無し
            AsmLot newlot = AsmLot.CreateNewAsmLot(order.WorkStartDt, order.MacNo, order.LineNo);

            //搭載機なしラインでD/B投入時点で仮指図発行される場合の処理
            Order magOrder = Order.GetMagazineOrder(order.InMagazineNo, order.ProcNo);
            if (magOrder != null)
            {
                order.InspectCt = magOrder.InspectCt;
                order.InspectEmpCd = magOrder.InspectEmpCd;
                magOrder.WorkEndDt = DateTime.Now;
                magOrder.DeleteInsert(magOrder.LotNo);
                AsmLot maglot = AsmLot.GetAsmLot(order.InMagazineNo);
                if (maglot != null && newlot.LotSize != maglot.LotSize)
                {
                    newlot.LotSize = maglot.LotSize;
                }
            }

            order.LotNo = newlot.NascaLotNo;
            order.DeleteInsert(newlot.NascaLotNo);

            mag = new Magazine();
            mag.NascaLotNO = newlot.NascaLotNo;
            mag.MagazineNo = order.OutMagazineNo;
            mag.NowCompProcess = order.ProcNo;
            mag.NewFg = true;
            mag.Update();

            Process p = Process.GetProcess(order.ProcNo);

            //MAP搭載工程の場合は完了時に原材料関連付け
            if (order is MAPFrameLoadOrder)
            {
                Material[] frames = ((MAPFrameLoadOrder)order).FrameList;
                order.UpdateMaterialRelation(frames);

                if (((MAPFrameLoadOrder)order).IsFrameBakingTimeOver == true)
                {
                    //完了エラー時はロットの警告ステータスON
                    newlot.IsWarning = true;
                    newlot.Update();

                    //作業実績のコメント追記
                    order.Comment += " [完了異常] DB前ベーキング時間超過";
                    order.DeleteInsert(newlot.NascaLotNo);
                }
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

            //期間中の原材料、ロット情報を更新
            Exporter exp = Exporter.GetInstance();
            NASCAResponse res = exp.SendOrderToNasca(newlot, p);

            //NASCA送信時点でOrderが更新されているので再取得
            order = Order.GetMagazineOrder(order.LotNo, order.ProcNo);

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

                return ArmsSvrReturnInfo.GetNgReturnData(msg, newlot.NascaLotNo, order.ProcNo);
            }

            // ログ出力（処理終了）
            Process process = Process.GetProcess(order.ProcNo);
            Log.SysLog.Info("[api] EditWorkEnd end:" + order.InMagazineNo + " >> " + order.OutMagazineNo);
            return ArmsSvrReturnInfo.GetReturnData(ResStatus.OK, process.InlineProNM + "を完了しました", order.ProcNo);
        }
        #endregion

        #endregion

        //#region CheckNextProcess
        ///// <summary>
        ///// 次作業開始の判定
        ///// </summary>
        ///// <param name="inlineNO">インラインNO</param>
        ///// <param name="magazineNO">マガジンNO</param>
        ///// <param name="workId">作業ID</param>
        ///// <returns>戻り用構造体</returns>
        //public ArmsSvrReturnInfo CheckNextProcess(string magazineNO, string workId, Order order)
        //{
        //    try
        //    {
        //        Magazine mag = Magazine.GetCurrent(magazineNO);
        //        if (mag == null)
        //        {
        //            return ArmsSvrReturnInfo.GetNgReturnData("マガジン情報が存在しません", "", order.ProcNo);
        //        }
        //        AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);

        //        if (WorkChecker.IsNextWorkStarted(mag.NascaLotNO, mag.NowCompProcess))
        //        {
        //            return ArmsSvrReturnInfo.GetNgReturnData("次の作業が開始されています", lot.NascaLotNo, mag.NowCompProcess);
        //        }

        //        return QueryNextWork(order);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        //#endregion
    }
}
