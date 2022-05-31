using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace ArmsApi.Model
{
    public class WorkChecker
    {
        #region 作業完了時データ登録後の事後チェック
        /// <summary>
        /// 作業完了時データ登録後の事後チェック
        /// </summary>
        /// <param name="paramInfo">作業終了構造体</param>
        /// <param name="matOrdInfo">資材情報</param>
        /// <param name="retInfo">戻り値のデフォルト値</param>
        /// <returns></returns>
        public static bool IsErrorWorkComplete(Order order, MachineInfo machine, AsmLot lot, bool isManualInputout, out string errMsg)
        {
            try
            {
                bool isError = false;

                //装置投入原材料取得
                //MAP指図の場合、先行でOrderと関連付いている前提
                Material[] matlist = order.GetMaterials();

                //工程情報
                Process proc = Process.GetProcess(order.ProcNo);

                //原材料の使用期限切れチェック
                isError = WorkChecker.HasExpiredMaterial(machine, order, lot, out errMsg);
                if (isError) return true;

                //車載自動搬送の多品種流動モード(DB振替えモード）がONかつProcNo=0の場合はBOMチェックしない
                if (Config.Settings.IsDBProfileRequireMode == false || order.ProcNo != 0)
                {
                    //原材料のBOMチェック
                    BOM[] bom = Profile.GetBOM(lot.ProfileId);
                    isError = WorkChecker.IsBomError(matlist, bom, out errMsg, lot, proc.WorkCd, order.MacNo);
                    if (isError) return true;
                }

				//原材料の投入可能作業チェック
				isError = WorkChecker.CanInputToWorkMaterial(matlist, lot.TypeCd, proc.WorkCd, out errMsg);
				if (isError) return true;

                //対象工程がCHECK側になっている時間管理チェック
                if (lot.NascaLotNo != order.LotNo)
                {
                    //※ブレンドロットの場合、lot=子ロット、order.LotNo=ブレンドロットが入力されている
                    lot.NascaLotNo = order.LotNo;
                }
                LimitCheckResult[] limit = TimeLimit.CheckProcExpired(lot.TypeCd, lot, order.ProcNo, order);
                if (limit.Length >= 1)
                {
                    errMsg = "タイムチャートオーバー発生";
                    return true;
                }

                //モールド樹脂照合 (調合有効期限、樹脂グループ)
                if (machine.ResinCheckFg == true)
                {
                    isError = WorkChecker.IsResinError(machine, order, lot, out errMsg);
                    if (isError == true) return true;
                }

                //一つ前の工程の抜き取り状態検査済み確認
                Process prevProcess = Process.GetPrevProcess(proc.ProcNo, lot.TypeCd);
                if (prevProcess != null)
                {
                    //抜き取り検査情報取得
                    Inspection isp = Inspection.GetInspection(lot.NascaLotNo, prevProcess.ProcNo);
                    if (isp != null)
                    {
                        //状態検査必要で未検査の場合は排出工程を返す
                        if (isp.IsInspected == false)
                        {
                            errMsg = "前工程の状態検査が終了していません";
                            return true;
                        }
                    }
                }

                //ウェハー引き当てチェック(ArmsMaintenanceからの呼び出しは無効)
                if (!isManualInputout)
                {
                    if (machine.WaferCheckFg == true)
                    {
                        Material[] wafers = machine.GetWafers(order.WorkStartDt, order.WorkEndDt.Value);
                        isError = IsWaferChangeError(wafers, order, out errMsg);
                        if (isError == true) { return true; }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("作業完了後判定で不明なエラーが発生しました:" + ex.ToString());
            }
            errMsg = "";
            return false;
        }

        public static bool IsErrorWorkComplete(Order order, MachineInfo machine, AsmLot lot, out string errMsg)
        {
            return IsErrorWorkComplete(order, machine, lot, false, out errMsg);
        }

        #endregion

        #region 作業開始前チェック

        /// <summary>
        /// 作業開始前チェック
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="machine"></param>
        /// <param name="order"></param>
        /// <param name="process"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool IsErrorBeforeStartWork(AsmLot lot, MachineInfo machine, Order order, Process process, out string errMsg)
        {
            if (order.LotNo == null) throw new ApplicationException("Orderロット番号未設定");
            return IsErrorBeforeStartWork(lot, machine, order, null, process, false, out errMsg);
        }

        /// <summary>
        /// 作業開始前チェック
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="machine"></param>
        /// <param name="order"></param>
        /// <param name="assemblyLotNo"></param>
        /// <param name="process"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool IsErrorBeforeStartWork(AsmLot lot, MachineInfo machine, Order order, string assemblyLotNo, Process process, out string errMsg)
        {
            if (order.LotNo == null) throw new ApplicationException("Orderロット番号未設定");
            return IsErrorBeforeStartWork(lot, machine, order, assemblyLotNo, process, false, out errMsg);
        }

        /// <summary>
        /// 作業開始前チェック 
        /// </summary>
        /// <param name="equipInfo">設備情報</param>
        /// <param name="proInfo">工程情報</param>
        /// <param name="matOrderInfo">指図情報</param>
        /// <param name="lotCharInfo">ロット特性</param>
        public static bool IsErrorBeforeStartWork(AsmLot lot, MachineInfo machine, Order order, string assemblyLotNo, Process process, bool isManualInput,
            out string errMsg)
        {
            if (order.LotNo == null) throw new ApplicationException("Orderロット番号未設定");

            //※ブレンドされているロット、かつ最終工程以降の工程の開始の場合、lot=子ロット、order.LotNo=ブレンドロット、assemblyLotNo=子ロットが入力されている
            string lotno = order.LotNo;

            try
            {
                bool isError = false;

                //EICS設定ファイルとの照合
                //isManualInput=画面からの手入力の際は判定しない
                if (!isManualInput)
                {
                    //型番照合
                    //type=null or emptyの場合はEICSに設定なし（オーブン等）のため判定しない
                    //2015.11.4改修。タイプ取得をDBに変更した為、型番チェックするかどうかの判定フラグ※を追加
                    //※型番規制したくないので敢えて設定ファイルを指定しない、というパターンを再現するためのフラグ。
                    //string type = MachineInfo.GetCurrentEICSTypeCd(machine, true);
                    //if (string.IsNullOrEmpty(type) == false && type != lot.TypeCd)
                    //{
                    //    errMsg = string.Format("EICSの装置設定タイプと異なるため投入できません。{0}", machine.LongName);
                    //    return true;
                    //}
                    if (isMismatchBetweenLotAndMacProductType(lot, machine, out errMsg))
                    {
                        return true;
                    }

                    if (Config.Settings.QCILXmlFilePath != null)
                    {
                        //バッドマークカウント照合
                        //isbadmarkframe=null or emptyの場合はEICSに設定なし（オーブン等）のため判定しない
                        bool? isbadmarkframe = MachineInfo.GetCurrentEICSBadMarkCountFg(machine);
                        if (lot.IsBadMarkFrame && isbadmarkframe.HasValue) 
                        {
                            if (lot.IsBadMarkFrame != isbadmarkframe) 
                            {
                                errMsg = "EICSの装置設定バッドマークカウントフラグと異なるため投入できません";
                                return true;
                            }
                        }
                    }
                }

                if (lot.IsWarning)
                {
                    errMsg = "ロットの警告フラグがONになっています";
                    return true;
                }
                if (string.IsNullOrWhiteSpace(assemblyLotNo) == false)
                {
                    //ブレンドされているロット、かつ最終工程以降の工程の開始の場合、子ロットすべての警告フラグを確認する
                    CutBlend[] cbs = CutBlend.SearchBlendRecord(null, order.LotNo, null, false, false);
                    foreach (CutBlend cb in cbs)
                    {
                        AsmLot cutlot = AsmLot.GetAsmLot(cb.LotNo);
                        if (cutlot.IsWarning)
                        {
                            errMsg = $"ロットの警告フラグがONになっています 対象ロット：『{cb.LotNo}』";
                            return true;
                        }
                    }
                }

                //作業飛ばしのチェック
                if (string.IsNullOrWhiteSpace(assemblyLotNo) == false)
                {
                    isError = WorkChecker.IsWorkSkiped(lotno, lot.TypeCd, order.ProcNo);
                }
                else
                {
                    isError = WorkChecker.IsWorkSkiped(lotno, order.ProcNo);
                }
                if (isError == true)
                {
                    // ダミー実績登録可能の場合は、エラーをスキップ。この関数の外でDummy実績登録を実施する
                    bool needDummyInsertBeforeWork = WorkChecker.CanDummyAutoTran(lot, order.ProcNo);
                    if (needDummyInsertBeforeWork == false)
                    {
                        errMsg = "前作業の実績が入力されていません";
                        return true;
                    }
                }

                //次工程開始済みの場合は前の開始不可（完了はOK）
                isError = WorkChecker.IsNextWorkStarted(lotno, assemblyLotNo, order.ProcNo);
                if (isError == true)
                {
                    errMsg = "既に次の作業が開始されています";
                    return true;
                }

                string nascaLotNo = lot.NascaLotNo;
                if (string.IsNullOrWhiteSpace(assemblyLotNo) == false)
                {
                    //ブレンドされているロット、かつ最終工程以降の工程の開始の場合、lot.NascaLotNo=子ロットなので、lotno=親ロットに置き換え
                    nascaLotNo = lotno;
                }
                if (machine.Canredoprocessfg == true && Order.GetOrder(nascaLotNo, order.ProcNo).Any())
                {
                    errMsg = $"既に作業開始されています。製品状態を確認し、再投入する場合は開始実績を削除して下さい。ロットNo:{nascaLotNo} 作業名:{process.InlineProNM}";
                    return true;
                }

                //List<string> macGroup = MachineInfo.GetMachine(order.MacNo).MacGroup;

                ////投入可能装置判定
                //MachineInfo[] availableMachines = MachineInfo.GetAvailableMachines(process.ProcNo, macGroup);
                //bool found = false;
                //foreach (MachineInfo a in availableMachines)
                //{
                //    if (a.MacNo == machine.MacNo)
                //    {
                //        found = true;
                //        break;
                //    }
                //}
                //if (!found)
                isError = WorkChecker.IsNotRelatedMachineAndProcess(order.MacNo, process.ProcNo);
                if (isError == true)
                {
                    string magno = string.Empty;
                    if (order != null && string.IsNullOrWhiteSpace(order.InMagazineNo) == false) magno = order.InMagazineNo;
                    errMsg = $"この装置は現在の工程で作業できません。作業設備マスタ(TmProMac)に登録されていません。[ロットNo:『{lot.NascaLotNo}』,"
                           + $"マガジンNo:『{magno}』,型番:『{lot.TypeCd}』,作業:『{process.ProcNo}/{process.InlineProNM}』"
                           + $"装置:『{machine.MacNo}/{machine.LongName}』]";
                    return true;
                }

				//2014.8.6 41移管2次で検証中
				//投入作業装置チェック
				if (IsInputWorkMachineError(machine, order.ProcNo, lot.TypeCd, out errMsg))
				{
                    return true;
				}

                //出荷規制判定
                string procAndReason;
				if (IsRestrictedInPreviousProcess(nascaLotNo, lot.TypeCd, order.ProcNo, out procAndReason))
				{
					errMsg = $"以下の理由で流動規制されています。ロット:『{nascaLotNo}』,{procAndReason}";
					return true;
				}
                if (string.IsNullOrWhiteSpace(assemblyLotNo) == false)
                {
                    //ブレンドされているロット、かつ最終工程以降の工程の開始の場合、子ロットすべての出荷規制を確認する
                    CutBlend[] cbs = CutBlend.SearchBlendRecord(null, order.LotNo, null, false, false);
                    foreach (CutBlend cb in cbs)
                    {
                        if (IsRestrictedInPreviousProcess(cb.LotNo, lot.TypeCd, order.ProcNo, out procAndReason))
                        {
                            errMsg = $"以下の理由で流動規制されています。ロット:『{cb.LotNo}』,{procAndReason}";
                            return true;
                        }
                    }
                }

                //モールド樹脂照合 (調合有効期限、樹脂グループ)
                if (machine.ResinCheckFg == true)
                {
                    isError = WorkChecker.IsResinError(machine, order, lot, out errMsg);
                    if (isError == true) return true;
                }

                //投入樹脂グループ判定
                foreach (string resingroupcd in lot.ResinGpCd)
                {
                    if (Process.IsDenyProcResin(order.ProcNo, resingroupcd) == true)
                    {
                        errMsg = string.Format("【投入禁止樹脂Gr規制】樹脂Gr[{0}]は作業[{1}]に投入できません。",
                            resingroupcd, Process.GetProcess(order.ProcNo).InlineProNM);
                        return true;
                    }
                }

                //装置投入原材料取得（全て）
                //Material[] matlist = machine.GetMaterials(order.WorkStartDt, order.WorkEndDt);
                Material[] matlist = order.GetMaterials(false, true);

                //原材料のBOMチェック
                BOM[] bom = Profile.GetBOM(lot.ProfileId);
                isError = WorkChecker.IsBomError(matlist, bom, out errMsg, lot, process.WorkCd, order.MacNo);
                if (isError) return true;

				//原材料の投入可能作業チェック
				isError = WorkChecker.CanInputToWorkMaterial(matlist, lot.TypeCd, process.WorkCd, out errMsg);
				if (isError) return true;

                //資材の樹脂グループとの照合チェック 2016.11.5 湯浅
                foreach (Material mat in matlist)
                {
                    if (ArmsApi.Model.Material.IsCompareResinMatGr(mat.HMGroup) == true && IsMatchResinGroup(mat, lot, order.ProcNo, out errMsg) == false)
                    {
                        isError = true;
                        errMsg += String.Format("資材ロット:{0}, アッセンロット:{1}", mat.LotNo, lot.NascaLotNo);
                        return true;
                    }
                }

                //装置投入原材料取得
                matlist = machine.GetMaterials(order.WorkStartDt, order.WorkEndDt, machine, order.ScNo1, order.ScNo2);

                //原材料の有効期限チェック
                isError = WorkChecker.HasExpiredMaterial(machine, order, lot, out errMsg);
                if (isError == true) return true;

                //作業時間判定
                isError = TimeLimit.IsErrorStartWork(nascaLotNo, lot.TypeCd, order.ProcNo, order);
                if (isError == true)
                {
                    errMsg = "時間監視判定NG";
                    return true;
                }

                //カット工程のブレンド判定
                if (process.IsCutBlend)
                {
                    //NEL_SVラインの場合、データメンテ履歴のあるマガジンはライン外へ排出
                    if (Config.GetLineType == Config.LineTypes.NEL_SV)
                    {
                        if (History.HasUncheckedHistory(nascaLotNo) == true)
                        {
                            errMsg = "未確認のデータメンテナンス履歴が存在します";
                            return true;
                        }
                    }

                    List<AsmLot> blend = new List<AsmLot>();
                    CutBlend[] cb = CutBlend.GetCurrentBlendItems(machine.MacNo);

                    foreach (CutBlend b in cb)
                    {
                        AsmLot existlot = AsmLot.GetAsmLot(b.LotNo);
                        blend.Add(existlot);
                    }
                    blend.Add(lot);

                    //if (Config.Settings.IsCutBlendErrorCheck)
                    //{
                    isError = IsCutBlendError(blend.ToArray(), out errMsg);
                    if (isError == true) return true;
                    //}
                }

                // ウェハーのブレンドコード、作業コードチェック
                if (machine.WaferCheckFg == true)
                {
                    //開始時はストッカー未確定のためストッカー内の全ウェハー取得
                    Material[] wafers = machine.GetWafers(order.WorkStartDt);

                    //富士情報 mod start
                    //製品、素子のコードから波長ランクのリストを取得する
                    string[] okrnks = SosiCheck.GetHchRnkLstFronShn(lot.TypeCd, wafers[0].MaterialCd);

                    if (okrnks.Length == 0)
                    {
                        errMsg = string.Format("製品素子波長ランクマスタが未登録です 機種:{0} 素子:{1}", lot.TypeCd, wafers[0].MaterialCd);
                        return true;
                    }
                    if (okrnks[0] != "")
                    //波長ランクチェック対象の素子の場合はチェックする
                    {
                        //投入素子のリストからCEJ機種CDリスト取得
                        IEnumerable<string> cejkshcds = wafers.Select(o => o.BlendCd.ToUpper()).Distinct();
                        //投入素子のCEJ機種CDリストから波長ランクCEJ機種CDリスト取得
                        SosiCheck.HchRnkCejKsh[] rnkkshs = SosiCheck.GetHchRnkCejKshList(cejkshcds.ToList());

                        //投入素子のCEJ機種CDリストから波長ランクCEJ機種CDリスト取得できなかったリスト作成
                        IEnumerable<string> exrnk = cejkshcds.Except<string>(rnkkshs.Select(o => o.CejKshCd));
                        if (exrnk.Count() > 0)
                        {
                            errMsg = string.Format("CEJ機種波長ランクマスタが未登録です CEJ機種:{0}", string.Join(",", exrnk));
                            return true;
                        }

                        if (rnkkshs.Select(o => o.CejHchRnk).Distinct().Count() > 1)
                        {
                            errMsg = "複数の波長ランクの素子が投入されています";
                            return true;
                        }

                         //製品素子の波長ランクと一致するか
                        if (!okrnks.Contains(rnkkshs[0].CejHchRnk))
                        {
                            errMsg = string.Format("製品に投入可能な波長ランクではありません 製品:{0} 素子波長ランク:{1}", lot.TypeCd, rnkkshs[0].CejKshCd);
                            return true;
                        }
                    }

                    //              //現作業コード取得
                    //              string workcd = Process.GetProcess(order.ProcNo).WorkCd;

                    //              foreach (Material wafer in wafers)
                    //              {
                    //if (Material.IsInHouseDice(wafer.DiceWaferKb))
                    //{
                    //	// 社内チップはブレンドコードチェック
                    //	isError = IsDbWaferBlendCodeError(wafer, lot, workcd, out errMsg);
                    //	if (isError) return true;
                    //}
                    //else
                    //{
                    //	// 社外チップは供給ID、品目チェック
                    //	isError = IsDBWaferSupplyIdError(wafer, lot, workcd, out errMsg);
                    //	if (isError) return true;
                    //}

                    //// DBウェハーの水洗浄後、有効期限チェック
                    //// 2015.5.14 3in1高効率ラインのシステム変更依頼1(水洗浄)
                    //isError = IsDbWaferWashedLimitError(wafer, lot.TypeCd, out errMsg);
                    //if (isError) return true;
                    //              }
                    //富士情報 mod end

                }

                //製造条件判定（NASCAのPDA条件マスタ）
                isError = WorkChecker.IsCondError(lot, order, machine, out errMsg);
                if (isError == true) return true;


                //開始工程より前の工程の抜き取り状態検査済み確認
                Process[] flow = Process.GetWorkFlow(lot.TypeCd);
                foreach (Process p in flow)
                {
                    //現プロセスより先の工程は無視
                    if (p.ProcNo == process.ProcNo) break;

                    //抜き取り検査情報取得
                    Inspection isp = Inspection.GetInspection(nascaLotNo, p.ProcNo);
                    if (isp != null)
                    {
                        //状態検査必要で未検査の場合は排出工程を返す
                        if (isp.IsInspected == false)
                        {
                            errMsg = "前工程の状態検査が終了していません";
                            return true;
                        }
                    }
                }

                //富士情報　start
                //帳票システムで前の工程がクローズしていること
                    if (!ArmsApi.Model.FORMS.ProccessForms.isClosedPrevProcess(lot.TypeCd, order.LotNo, order.ProcNo, out errMsg))
                    {
                        return true;
                    }
                //富士情報　end


                //エラー無し
                return false;
            }
            catch (ArmsException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ArmsException("作業開始前チェックで不明なエラーが発生しました", ex);
            }
        }
        #endregion

        #region ロット-装置製品型番チェック
        public static bool isMismatchBetweenLotAndMacProductType(AsmLot lot, MachineInfo machine, out string message)
        {
            using (var eicsDB = new DataContext.EICSDataContext(Config.Settings.QCILConSTR))
            {
                var lsetInfoList = eicsDB.TmLSET.Where(t => t.Equipment_NO == machine.NascaPlantCd && t.ArmsTypeNoCheck_FG == false && t.Del_FG == false);
                DataContext.TmLSET lsetInfo = lsetInfoList.FirstOrDefault();

                if (lsetInfo != null)
                {
                    string typeGrCD = lsetInfo.WorkingTypeGroup_CD;
                    string chipNM = lsetInfo.Chip_NM;

                    string machineModel = eicsDB.TmEQUI.Where(t => t.Equipment_NO == machine.NascaPlantCd && t.Del_FG == false)
                        .Select(t => t.Model_NM).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(machineModel))
                    {
                        throw new ApplicationException($"設備マスタから設備型式が取得できません。設備:{machine.NascaPlantCd}/{machine.MachineName}");
                    }

                    var typeGrMasterList = eicsDB.TmTypeGroup.Where(t => t.Model_NM == machineModel && t.TypeGroup_CD == typeGrCD && t.Chip_NM == chipNM && t.Del_FG == false);

                    if (typeGrMasterList != null && typeGrMasterList.Select(t => t.Type_CD).Contains(lot.TypeCd))
                    {
                        message = $"EICSの装置設定タイプグループに含まれない型番のため投入できません。投入ロットの型番：『{lot.TypeCd}』, EICSの装置情報[設備：『{machine.NascaPlantCd}/{machine.MachineName}』,設定タイプグループ：『{typeGrCD}』]";
                        return false;
                    }
                }
            }

            string type = MachineInfo.GetCurrentEICSTypeCd(machine, true);
            if (string.IsNullOrEmpty(type) == false && type != lot.TypeCd)
            {
                message = $"EICSの装置設定タイプと異なるため投入できません。投入ロットの型番：『{lot.TypeCd}』, EICSの装置情報[設備：『{machine.NascaPlantCd}/{machine.MachineName}』,設定タイプ：『{type}』]";
                return true;
            }

            message = string.Empty;
            return false;
        }
        #endregion

        #region モールド樹脂投入前チェック

        public static bool IsErrorBeforeInputResin(Resin r, MachineInfo machine, out string msg)
        {
            return IsErrorBeforeInputResin(r, machine, out msg, false);
        }

        public static bool IsErrorBeforeInputResin(Resin r, MachineInfo machine, out string msg, bool removeFg)
        {
            try
            {
                if (machine.ResinCheckFg == false)
                {
                    msg = "調合樹脂投入対象の設備ではありません";
                    return true;
                }

                Order[] orders;

                if (removeFg == false)
                {
                    //作業中のロット情報取得
                    orders = Order.GetCurrentWorkingOrderInMachine(machine.MacNo);
                }
                else
                {
                    orders = Order.SearchOrder(null, null, null, machine.MacNo, false, false, null, r.RemoveDt, r.InputDt, null);
                    Order[] ordersCorrent = Order.SearchOrder(null, null, null, machine.MacNo, true, false, null, r.RemoveDt, null, null);

                    if (ordersCorrent.Count() > 0)
                    {
                        orders = orders.Concat(ordersCorrent).ToArray();
                    }
                }

                //モールド樹脂照合 (調合有効期限、樹脂グループ)
                foreach (Order order in orders)
                {
                    AsmLot lot = AsmLot.GetAsmLot(order.LotNo);
                    bool isExpiredResinMix = IsExpiredUseLimit(r.LimitDt, r.RemoveDt, order.WorkStartDt, order.WorkEndDt);

                    if (isExpiredResinMix)
                    {
                        msg = "調合後の有効期限を超過しています";
                        return true;
                    }

					// 2015.2.4 複数樹脂グループ投入対応
                    if (lot.ResinGpCd.Contains(r.ResinGroupCd) == false)
                    {
                        msg = "投入樹脂の樹脂グループコードが一致しません";
                        return true;
                    }

					Process p = Process.GetProcess(order.ProcNo);
					if (p.MixTypeCd.Contains(r.MixTypeCd) == false)
					{
						msg = string.Format("投入樹脂の調合タイプが一致しません");
						return true;
					}
                }

				//割り付け済み樹脂取得
				Resin[] registeredResinArray = machine.GetResins();

				long targetResinResultID;
				if (long.TryParse(r.MixResultId, out targetResinResultID) == false)
				{
					throw new ApplicationException($"数値変換出来ない調合結果IDです。調合結果ID：『{r.MixResultId}』");
				}

				if (IsMixMaterialMatchError(targetResinResultID, registeredResinArray, out msg) == true)
				{
					return true;
				}

                if (removeFg == false && r.LimitDt <= DateTime.Now)
                {
                    msg = "調合後の有効期限を超過しています";
                    return true;
                }

                // 脱泡後有効期限の超過チェック
                if(removeFg == false && r.StirringLimitDt.HasValue)
                {
                    // 有効期限がNULLの場合は規制チェックしない
                    if(DateTime.Now > r.StirringLimitDt.Value)
                    {
                        msg = $"脱泡後の有効期限を超過しています。調合結果ID『{r.MixResultId}』,脱泡後有効期限『{r.StirringLimitDt.Value}』";
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = "不明なエラー:" + ex.ToString();
                return true;
            }

            msg = "";
            return false;

        }
        #endregion

        #region IsErrorBeforeInputMat

        /// <summary>
        /// 原材料の投入前照合
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="lot"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool IsErrorBeforeInputMat(Material mat, AsmLot lot, out string errMsg)
        {
            return IsErrorBeforeInputMat(mat, new AsmLot[] { lot }, null, string.Empty, out errMsg);
        }

        /// <summary>
        /// 原材料の投入前照合
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="lot"></param>
        /// <param name="workCd">作業実績入力画面から選択作業を引き継ぎ</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool IsErrorBeforeInputMat(Material mat, AsmLot lot, string selectWorkCd, out string errMsg)
        {
            return IsErrorBeforeInputMat(mat, new AsmLot[] { lot }, null, selectWorkCd, out errMsg);           
        }

        public static bool IsErrorBeforeInputMat(Material mat, AsmLot[] lotlist, int macno, out string errMsg)
        {
            return IsErrorBeforeInputMat(mat, lotlist, macno, string.Empty, out errMsg);
        }

        /// <summary>
        /// 原材料の投入前照合
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="matlist">先に投入されている原材料</param>
        /// <param name="lotlist"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool IsErrorBeforeInputMat(Material mat, AsmLot[] lotlist, int? macno, string selectWorkCd, out string errMsg)
        {
            //macnoとselectWorkCDの指定から矛盾が発生しないか事前チェック
            if (macno.HasValue == true && selectWorkCd != string.Empty)
            {
                foreach (AsmLot lot in lotlist)
                {
                    //string workCd = Process.GetProcess(Order.GetLastProcNoFromLot(macno.Value, lot.NascaLotNo)).WorkCd;

                    // カット装置の照合の場合、TnTranではなく、TnCutBlendに作業開始レコードがある為、
                    // 必ず、ArmsExceptionを飛ばしてしまう。
                    // 対策として、ArmsExceptionが飛んできた場合は、装置に割り付いているprocnoの作業CDと照合する
                    string workCd = string.Empty;
                    try
                    {
                        workCd = Process.GetProcess(Order.GetLastProcNoFromLot(macno.Value, lot.NascaLotNo)).WorkCd;
                    }
                    catch(ArmsException)
                    {
                        List<int> procNoList = MachineInfo.GetProcNoList(macno.Value);
                        foreach (int procno in procNoList)
                        {
                            Process p = Process.GetProcess(procno);
                            if (p != null && selectWorkCd == p.WorkCd)
                            {
                                workCd = p.WorkCd;
                            }
                        }
                    }

                    if (workCd != selectWorkCd)
                    {
                        throw new ApplicationException("IsErrorBeforeInputMat()呼び出し異常：macnoとselectWorkCDの指定に矛盾");
                    }
                }
            }

            if (mat.InputDt.HasValue == false)
            {
                errMsg = "投入日時が設定されていません";
                return true;
            }

            try
            {
                bool isError = false;

                //資材BOM
                foreach (AsmLot lot in lotlist)
                {
                    //原材料のBOMチェック
                    BOM[] bom = Profile.GetBOM(lot.ProfileId);
                    isError = WorkChecker.IsBomError(new Material[] { mat }, bom, out errMsg, null, "", null);
                    if (isError) return true;

					//原材料の投入可能作業チェック
					//原材料の投入可能作業チェック
					string workCd = string.Empty;
					if (!string.IsNullOrEmpty(selectWorkCd))
					{
						workCd = selectWorkCd;
					}
					else
					{
                        workCd = Process.GetProcess(Order.GetLastProcNoFromLot(macno.Value, lot.NascaLotNo)).WorkCd;
                    }
					isError = WorkChecker.CanInputToWorkMaterial(new Material[] { mat }, lot.TypeCd, workCd, out errMsg);
					if (isError) return true;
                }

                //ウェハーブレンドコード、作業コード
                if (mat.IsWafer)
                {
                    //富士情報　更新　start
                    if (lotlist.Length > 0)
                    //設備で開始しているロットがある場合、製品が確定しているので波長ランクのチェックをする
                    {
                        //製品、素子のコードから波長ランクのリストを取得する
                        string[] okrnks = SosiCheck.GetHchRnkLstFronShn(lotlist[0].TypeCd, mat.MaterialCd);

                        //波長ランクチェック用のマスタが存在するか
                        if (okrnks.Length == 0)
                        {
                            errMsg = string.Format("製品素子波長ランクマスタが未登録です 機種:{0} 素子:{1}", lotlist[0].TypeCd, mat.MaterialCd);
                            return true;
                        }
                        if (okrnks[0] != "")
                        //波長ランクチェック対象の素子の場合はチェックする
                        {
                            //投入素子のCEJ機種CDから波長ランク取得
                            string matrnk = SosiCheck.GetHchRnkFromCejkscd(mat.BlendCd);
                            if (matrnk == "")
                            {
                                errMsg = string.Format("CEJ機種波長ランクマスタが未登録です CEJ機種:{0}", mat.BlendCd);
                                return true;
                            }
                            //投入素子波長ランクがマスタで許可する波長ランクに含まれているか
                            if (!okrnks.Contains(matrnk))
                            {
                                errMsg = string.Format("製品に投入可能な波長ランクではありません 投入波長ランク:{0} 投入CEJ機種:{1}", matrnk, mat.BlendCd);
                                return true;
                            }

                            //投入済みの素子と素子ランクが一致しているか
                            if (macno.HasValue)
                            {
                                //設備に投入済みの素子情報を取得
                                Material[] macmats = SosiCheck.GetMatCejKshLst(macno, mat.MaterialCd, mat.InputDt, mat.InputDt, false);

                                if (macmats.Length > 0)
                                {
                                    //投入済み素子の波長ランク取得
                                    string macrnk = SosiCheck.GetHchRnkFromCejkscd(macmats[0].BlendCd);

                                   if (matrnk != macrnk)
                                    {
                                        errMsg = string.Format("投入済みの素子の波長ランクと一致しません　投入済み波長ランク:{0} 投入波長ランク:{1} 投入CEJ機種:{2}", macrnk, matrnk, mat.BlendCd);
                                        return true;
                                    }
                                }
                            }
                        }
                    }


      //              foreach (AsmLot lot in lotlist)
      //              {
      //                  string workCd = string.Empty;
      //                  if (macno.HasValue)
      //                  {
						//	workCd = Process.GetProcess(Order.GetLastProcNoFromLot(macno.Value, lot.NascaLotNo)).WorkCd;
      //                  }
      //                  else if (macno.HasValue == false && selectWorkCd == string.Empty)
      //                  {
      //                      workCd = Process.GetNowProcess(lot).WorkCd;
      //                  }
      //                  else
      //                  {
      //                      workCd = selectWorkCd;
      //                  }

						//if (Material.IsInHouseDice(mat.DiceWaferKb))
						//{
						//	// 社内チップはブレンドコードチェック
						//	isError = IsDbWaferBlendCodeError(mat, lot, workCd, out errMsg);
						//	if (isError) return true;
						//}
						//else
						//{
						//	// 社外チップは供給ID、品目チェック
						//	isError = IsDBWaferSupplyIdError(mat, lot, workCd, out errMsg);
						//	if (isError) return true;
						//}

						////// 2015.5.13 [変更]ブレンド詳細マスタ複数作業CD取り込み
						//////if (string.IsNullOrEmpty(mat.WorkCd))
						////if (mat.WorkCd.Count == 0)
						////{
						////	if (lot.BlendCd != mat.BlendCd)
						////	{
						////		errMsg = string.Format("ブレンドマスタと一致していません TnLot.blendcd:{0} = TnMaterials.blendcd:{1}"
						////			, lot.BlendCd, mat.BlendCd);
						////		return true;
						////	}
						////}
						////else
						////{
						////	// 2015.5.13 [変更]ブレンド詳細マスタ複数作業CD取り込み
						////	//if (lot.BlendCd != mat.BlendCd || workcd != mat.WorkCd)
						////	if (lot.BlendCd != mat.BlendCd || mat.WorkCd.Exists(w => w == workCd) == false)
						////	{
						////		errMsg = string.Format("ブレンドマスタと一致していません TnLot.blendcd:{0} = TnMaterials.blendcd:{1}, TnLot.workcd:{2} = TnMaterials.workcd:{3}"
						////			, lot.BlendCd, mat.BlendCd, workCd, mat.WorkCd);
						////		return true;
						////	}
						////}

						//// DBウェハーの水洗浄後、有効期限チェック
						//// 2015.5.14 3in1高効率ラインのシステム変更依頼1(水洗浄)
						//isError = IsDbWaferWashedLimitError(mat, lot.TypeCd, out errMsg);
						//if (isError) return true;
      //              }
                    //富士情報　更新　end
                }

                //有効期限判定
                if (mat.LimitDt < mat.InputDt)
                {
                    errMsg = "資材の有効期限を超過しています";
                    return true;
                }

                //有効期限判定(残り時間が*分切った場合も使用できないようにする) 2016.05.06
                if (Config.Settings.RemainingTimeOfLimit != null)
                {
                    if (mat.LimitDt.AddMinutes(-1 * Config.Settings.RemainingTimeOfLimit.Value) < mat.InputDt)
                    {
                        errMsg = string.Format("資材の有効期限が{0}分を下回っています", Config.Settings.RemainingTimeOfLimit.Value);
                        return true;
                    }
                }

                //DB樹脂の解凍→投入までの有効期限判定
                if (lotlist == null || lotlist.Length == 0)
                {
                    // 引数にアッセンロット指定が無い場合は、タイプを指定せずに「使用可能まで(分)」の時間チェックを行う。
                    bool isExpired = Material.IsFobiddenDBResinWhenInput(null, mat.MaterialCd, mat.LotNo, mat.InputDt.Value);
                    if (isExpired == true)
                    {
                        errMsg = "解凍後の使用禁止期間を経過していません";
                        return true;
                    }
                }
                foreach (AsmLot lot in lotlist)
                {
                    bool isExpired = Material.IsExpiredDBResinWhenInput(lot.TypeCd, mat.MaterialCd, mat.LotNo, mat.InputDt.Value, true);
                    if (isExpired == true)
                    {
                        errMsg = "解凍後の有効期限を超過しています";
                        return true;
                    }

                    //有効期限判定(残り時間が*分切った場合も使用できないようにする) 2016.05.06
                    if (Config.Settings.RemainingTimeOfLimit != null)
                    {
                        isExpired = Material.IsExpiredDBResinWhenInput(lot.TypeCd, mat.MaterialCd, mat.LotNo, mat.InputDt.Value, true, -1 * Config.Settings.RemainingTimeOfLimit.Value);
                        if (isExpired == true)
                        {
                            errMsg = string.Format("解凍後の有効期限が{0}分を下回っています", Config.Settings.RemainingTimeOfLimit.Value);
                            return true;
                        }
                    }

                    // 開封時刻から「使用可能まで(分)」の時間を経過していない場合も使用できないようにする。
                    isExpired = Material.IsFobiddenDBResinWhenInput(lot.TypeCd, mat.MaterialCd, mat.LotNo, mat.InputDt.Value);
                    if (isExpired == true)
                    {
                        errMsg = "開封後の使用禁止期間を経過していません";
                        return true;
                    }
                }

                //樹脂照合対象の品目グループならアッセンと一致するかを照合
                if (ArmsApi.Model.Material.IsCompareResinMatGr(mat.HMGroup) == true)
                {
                    foreach (AsmLot lot in lotlist)
                    {
                        Process currentProc = Process.GetProcess(Order.GetLastProcNoFromLot(macno.Value, lot.NascaLotNo));
                        if (IsMatchResinGroup(mat, lot, currentProc.ProcNo, out errMsg) == false)
                        {
                            errMsg += string.Format("ロット：{0} 資材：{1} ",lot.NascaLotNo, mat.LotNo);
                            return true;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                errMsg = "不明なエラー:" + ex.ToString();
                return true;
            }

            errMsg = "";
            return false;
        }

        /// <summary>
        /// 原材料投入前チェック（指定時間ロットの作業中ロットと照合）
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="machine"></param>
        /// <param name="order"></param>
        /// <param name="process"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool IsErrorBeforeInputMat(Material mat, MachineInfo machine, out string errMsg)
        {
            try
            {
                //作業中のロット情報取得
                List<Order> orders = new List<Order>();

                //取付→取外し範囲の資材は無条件で照合対象
                orders.AddRange(Order.SearchOrder(null, null, null, machine.MacNo, false, false, null, mat.RemoveDt, mat.InputDt, null));

                //現在作業中の指図一覧取得
                Order[] active = Order.SearchOrder(null, null, machine.MacNo, true, false);
                foreach (Order a in active)
                {
                    //取外し日以降の投入ロットは無視
                    if (mat.RemoveDt.HasValue && a.WorkStartDt >= mat.RemoveDt.Value) continue;
                    orders.Add(a);
                }

                List<AsmLot> lotlist = new List<AsmLot>();
                foreach (Order ord in orders)
                {
                    lotlist.Add(AsmLot.GetAsmLot(ord.LotNo));
                }

                // カット作業 (TmProcess.finalst = 1) の場合、TmCutBlendからも作業中ロットを取得する
                string selectWorkCd = string.Empty;
                List<int> procNoList = MachineInfo.GetProcNoList(machine.MacNo);
                bool isFinalSt = false;
                foreach(int procno in procNoList)
                {
                    Process p = Process.GetProcess(procno);
                    if (p == null) continue;
                    if (p.FinalSt == true)
                    {
                        isFinalSt = true;
                        // カット作業のみ作業CDを指定 (TnTranに開始レコードが無い為)
                        selectWorkCd = p.WorkCd;
                        break;
                    }
                }
                if (isFinalSt == true)
                {
                    // TmCutBlendからも作業中ロットの情報を取得する
                    CutBlend[] cutblends = CutBlend.GetCurrentBlendItems(machine.MacNo);
                    foreach(CutBlend cutblend in cutblends)
                    {
                        AsmLot lot = AsmLot.GetAsmLot(cutblend.LotNo);
                        if (lotlist.Any(l => l.NascaLotNo == lot.NascaLotNo) == false)
                        {
                            // まだlotlistにないロット情報を追加する
                            lotlist.Add(lot);
                        }
                    }
                }

                //return IsErrorBeforeInputMat(mat, lotlist.ToArray(), machine.MacNo, string.Empty, out errMsg);
                return IsErrorBeforeInputMat(mat, lotlist.ToArray(), machine.MacNo, selectWorkCd, out errMsg);
            }
            catch (Exception ex)
            {
                errMsg = "不明なエラー:" + ex.ToString();
                return true;
            }
        }
        #endregion

        #region checkWorkSkip

        /// <summary>
        /// 作業とばし判定
        /// 
        /// </summary>
        /// <param name="workinfo"></param>
        /// <returns></returns>
        public static bool IsWorkSkiped(string magazineLotNo, int newProcNo)
        {
            Process current = Process.GetProcess(newProcNo);
            AsmLot lot = AsmLot.GetAsmLot(magazineLotNo);
            Process prev = Process.GetPrevProcess(current.ProcNo, lot.TypeCd);

            if (prev != null)
            {
                Order prevOrder = Order.GetPrevMagazineOrder(magazineLotNo, current.ProcNo);

                if (prevOrder == null)
                {
                    return true;
                }
                else if (prevOrder.IsComplete == false)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 作業とばし判定(型番を引数に追加したもの)
        /// </summary>
        /// <param name="magazineLotNo"></param>
        /// <param name="typeCd"></param>
        /// <param name="newProcNo"></param>
        /// <returns></returns>
        public static bool IsWorkSkiped(string magazineLotNo, string typeCd, int newProcNo)
        {
            Process current = Process.GetProcess(newProcNo);
            Process prev = Process.GetPrevProcess(current.ProcNo, typeCd);

            if (prev != null)
            {
                Order prevOrder = Order.GetPrevMagazineOrder(magazineLotNo, typeCd, current.ProcNo);

                if (prevOrder == null)
                {
                    return true;
                }
                else if (prevOrder.IsComplete == false)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region IsNextWorkStarted

        /// <summary>
        /// 次作業開始済み判定
        /// 現工程が単マガジンで次工程が分割の場合　→　判定あり
        /// 現工程が分割マガジンで次工程が単マガジン　→　判定なし（常にFalse)
        /// 現工程が分割で次工程も分割の場合　→　マガジン連番が同一のもののみ判定
        /// </summary>
        /// <param name="workinfo"></param>
        /// <returns></returns>
        public static bool IsNextWorkStarted(string magazineLotNo, int inlineProcNo)
        {
            return IsNextWorkStarted(magazineLotNo, null, inlineProcNo);
        }

        /// <summary>
        /// 次作業開始済み判定
        /// 現工程が単マガジンで次工程が分割の場合　→　判定あり
        /// 現工程が分割マガジンで次工程が単マガジン　→　判定なし（常にFalse)
        /// 現工程が分割で次工程も分割の場合　→　マガジン連番が同一のもののみ判定
        /// </summary>
        /// <param name="workinfo"></param>
        /// <returns></returns>
        public static bool IsNextWorkStarted(string magazineLotNo, string assemblyLotNo, int inlineProcNo)
        {
            Process current = Process.GetProcess(inlineProcNo);
            AsmLot lot = AsmLot.GetAsmLot(magazineLotNo);
            string lotno = "";
            if (string.IsNullOrWhiteSpace(assemblyLotNo) == false)
            {
                //ブレンドロットの場合、TnLotにレコードがないので子ロットでTnLotにレコードを取得
                lot = AsmLot.GetAsmLot(assemblyLotNo);
                lotno = magazineLotNo;
            }
            else
            {
                lotno = lot.NascaLotNo;
            }

            Process next = Process.GetNextProcess(current.ProcNo, lot);

            if (next != null)
            {
                Order[] nextOrders = Order.GetOrder(lotno, next.ProcNo);

                if (nextOrders.Length == 0)
                {
                    return false;
                }

                int magSeqNo = Order.ParseMagSeqNo(magazineLotNo);

                if (magSeqNo == 0)
                {
                    if (nextOrders.Length >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //複→単の工程では次開始判定をしない　後から来るマガジンが弾かれるため
                    if (nextOrders.Where(o => (o.DevidedMagazineSeqNo == magSeqNo)).Count() >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }
        #endregion

		public static bool IsMixMaterialMatchError(long targetResinResultID, Resin[] registeredResinArray, out string msg)
		{
			msg = string.Empty;
			using (var armsDB = new DataContext.ARMSDataContext(ArmsApi.Config.Settings.LocalConnString))
			{
				var targetResinMixMat = armsDB.TnResinMixMat.Where(t => t.mixresultid == targetResinResultID);

				foreach (Resin registeredResin in registeredResinArray)
				{
					long registeredResinResultID;

					if (long.TryParse(registeredResin.MixResultId, out registeredResinResultID) == false)
					{
						throw new ApplicationException($"数値変換出来ない調合結果IDです。調合結果ID：『{registeredResin.MixResultId}』");
					}

					var registeredResinMixMat = armsDB.TnResinMixMat.Where(t => t.mixresultid == registeredResinResultID);

					//登録対象樹脂の資材中に登録済み樹脂の資材が存在するかチェック
					foreach (DataContext.TnResinMixMat registeredMixMat in registeredResinMixMat)
					{
						if (targetResinMixMat.Count(t => t.materialcd == registeredMixMat.materialcd && t.lotno == registeredMixMat.lotno) == 0)
						{
							msg = $"装置に割りついている調合樹脂の資材と一致しません。"
								+ $"品目CD：『{registeredMixMat.materialcd}』ロット：『{registeredMixMat.lotno}』";
							return true;
						}
					}

					//登録済み樹脂の資材中に登録対象樹脂の資材が存在するかチェック
					foreach (DataContext.TnResinMixMat targetMixMat in targetResinMixMat)
					{
						if(registeredResinMixMat.Count(r => r.materialcd == targetMixMat.materialcd && r.lotno == targetMixMat.lotno) == 0)
						{
							msg = $"装置に割りついている調合樹脂の資材と一致しません。"
								+ $"品目CD：『{targetMixMat.materialcd}』ロット：『{targetMixMat.lotno}』";
							return true;
						}
					}
				}
			}

			return false;
		}

        #region IsResinError

        public static bool IsResinError(MachineInfo m, Order order, AsmLot lot, out string msg)
        {
            Resin[] resins = m.GetResins(order.WorkStartDt, order.WorkEndDt);　//Order.GetResinではなく装置割り付け分のみ

            if (resins == null || resins.Count() == 0)
            {
                msg = $"[{m.MacNo}/{m.LongName}] 設備に樹脂が登録されていません。";
                return true;
            }

			Process p = Process.GetProcess(order.ProcNo);

            foreach (Resin r in resins)
            {
                bool isExpiredResinMix = IsExpiredUseLimit(r.LimitDt, r.RemoveDt, order.WorkStartDt, order.WorkEndDt);

                if (isExpiredResinMix)
                {
                    msg = $"[{m.MacNo}/{m.LongName}] 投入樹脂は調合後の有効期限を超過しています "
                        + $"投入樹脂[樹脂ID『{r.MixResultId}』/有効期限『{r.LimitDt.ToString()}』] "
                        + $"比較時間『{(order.WorkEndDt ?? order.WorkStartDt).ToString()}』";
                    return true;
                }

				// 2015.2.4 複数樹脂グループ投入対応
				if (lot.ResinGpCd.Contains(r.ResinGroupCd) == false) 
				{
					msg = $"[{m.MacNo}/{m.LongName}] 投入樹脂の樹脂グループコードが一致しません "
                        + $"投入可能樹脂グループコード:『{string.Join(",",lot.ResinGpCd)}』 "
                        + $"投入樹脂:[樹脂ID『{r.MixResultId}』/樹脂グループコード『{r.ResinGroupCd}』]";

                    return true;
				}


                // 20220112 Juniwatanabe
                // 複数樹脂対応実験
                // OriginalCode
                //if (p.MixTypeCd.Contains(r.MixTypeCd) == false)
                //{
                //    msg = $"[{m.MacNo}/{m.LongName}] 投入樹脂の調合タイプが一致しません "
                //    + $"投入調合タイプ:『{r.MixTypeCd}/{Resin.GetMixTypeNm(r.MixTypeCd)}』 "
                //    + $"投入樹脂:[樹脂ID『{r.MixResultId}』/樹脂グループコード『{r.ResinGroupCd}』]";
                //
                //    return true;
                //}

                var mixTypeCdArr = r.MixTypeCd.Split(',');

                foreach (var mixtypeCode in mixTypeCdArr)
                {
                    if (p.MixTypeCd.Contains(mixtypeCode) == false)
                    {
                        msg = $"[{m.MacNo}/{m.LongName}] 投入樹脂の調合タイプが一致しません "

                        + $"投入調合タイプ:『{r.MixTypeCd}/{Resin.GetMixTypeNm(r.MixTypeCd)}』 "
                        + $"投入樹脂:[樹脂ID『{r.MixResultId}』/樹脂グループコード『{r.ResinGroupCd}』]";

                        return true;
                    }
                }
            }

            msg = "";
            return false;
        }

        #endregion

        #region IsCutBlendError

		public static bool IsCutBlendError(AsmLot[] lotlist, out string msg) 
		{
			return IsCutBlendError(lotlist, out msg, true);
		}
        public static bool IsCutBlendError(AsmLot[] lotlist, out string msg, bool isTempCutBlendCheck)
        {
            string typeCd = null;
            string cutBlendCd = null;
            List<string> resinGroupCd = null;
            List<string> resinGroupCd2 = null;
            string blendCd = null;
            string scheduleSelectionStandard = null;
            int profileId = 0;
            string aimRank = null;
            int? mnggrid = null;
            string mnggrnm = null;
            //<--Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応
            string lot1st = null;
            //-->Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応

            bool isCutBlendCd = false;

            bool iscolortest = false;

            foreach (AsmLot lot in lotlist)
            {
                //富士情報　追加
                // ブレンドCD＝"NA"の場合ブレンド不可
                //登録ボタン押下後に2度この処理を行い、2度目はlotlistに同じロットが2件存在するため必ずエラーになる
                //そのため重複するロットをカウントしない
                if (lot.BlendCd == ArmsApi.Config.BlendCD_Forbidden && lotlist.Select(o => o.NascaLotNo).Distinct().Count() >= 2)
                {
                    msg = $"ブレンド禁止のロットです :「{lot.NascaLotNo}」 ブレンドCD:「{lot.BlendCd}」";
                    return true;
                }
                //富士情報　追加

                if (lot.IsKHLTest == true && lotlist.Length >= 2)
                {
                    msg = $"吸湿保管点灯試験の対象ロットはブレンドできません ロット:「{lot.NascaLotNo}」";
                    return true;
                }

                if (lot.IsReflowTestWirebond == true && lotlist.Length >= 2)
                {
                    msg = $"リフローパス試験(WB)数の対象ロットはブレンドできません ロット:「{lot.NascaLotNo}」";
                    return true;
                }
                
                if (!isCutBlendCd)
                {
                    typeCd = lot.TypeCd;
                    cutBlendCd = lot.CutBlendCd;
                    resinGroupCd = lot.ResinGpCd;
                    resinGroupCd2 = lot.ResinGpCd2;
                    blendCd = lot.BlendCd;
                    scheduleSelectionStandard = lot.ScheduleSelectionStandard;
                    profileId = lot.ProfileId;
                    Profile prof = Profile.GetProfile(profileId);
                    if (prof != null) aimRank = prof.AimRank;
                    iscolortest = lot.IsColorTest;
                    mnggrid = lot.MnggrId;
                    mnggrnm = lot.MnggrNm;
                    //<--Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応
                    lot1st = lot.NascaLotNo;//1つ目のロット
                    //-->Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応

                    isCutBlendCd = true;
                }
                else 
                {
                    if (typeCd != lot.TypeCd)
                    {
                        msg = $"製品型番が一致しません ロット：「{lot.NascaLotNo}」 製品型番：「{lot.TypeCd}」 読込済み：「{typeCd}」";
                        return true;
                    }

                    if (isDenyBlended(lot.TypeCd))
                    {
                        msg = $"ブレンド規制マスタ(TmDenyCutBlendType)で規制されている製品型番の為、ブレンドできません。  ロット：「{lot.NascaLotNo}」 製品型番：「{lot.TypeCd}」";
                        return true;
                    }

                    if (cutBlendCd != lot.CutBlendCd)
                    {
                        msg = $"カットブレンドグループが一致しません ロット：「{lot.NascaLotNo}」 カットブレンドグループ：「{lot.CutBlendCd}」 読込済み：「{cutBlendCd}」";
                        return true;
                    }

                    if (IsExactResinGp(lot, resinGroupCd) == false)
                    {
                        msg = $"樹脂グループが一致しません ロット:「{lot.NascaLotNo}」 樹脂G:「{string.Join(",", lot.ResinGpCd)}」 読込済み：「{string.Join(",", resinGroupCd)}」";
                        return true;
                    }

                    //resingrcd2追加時に照合するために書いたがNTSVでカットブレンド予定が無いので未検証。
                    //NTSVでカットブレンドする際には検証が必要。2018/5/29 湯浅/川口
                    //if (IsExactResinGp2(lot, resinGroupCd2) == false)
                    //{
                    //    msg = $"樹脂グループ(PB実装2)が一致しません ロット:「{lot.NascaLotNo}」 樹脂G:「{string.Join(",", lot.ResinGpCd2)}」 読込済み：「{string.Join(",", resinGroupCd2)}」";
                    //    return true;
                    //}

                    string lAimRank;
                    if (IsExactAimRank(lot, profileId, aimRank, out lAimRank) == false)
                    {
                        msg = $"狙いランクが一致しません ロット:「{lot.NascaLotNo}」 狙いランク:「{lAimRank}」 読込済み：「{aimRank}」";
                        return true;
                    }

                    if (iscolortest != lot.IsColorTest)
                    {
                        msg = $"先行色調確認特性が一致しません ロット:「{lot.NascaLotNo}」　先行色調確認：「{lot.IsColorTest.ToString()}」 読込済み：「{iscolortest.ToString()}」";
                        return true;
                    }

                    // ①ブレンドCDが同じかチェック
                    if (blendCd != lot.BlendCd)
                    {
                        msg = $"ブレンドCDが一致しません ロット:「{lot.NascaLotNo}」 ブレンドCD:「{lot.BlendCd}」 読込済み：「{blendCd}」";
                        return true;
                    }

                    // ②予定選別規格が同じかチェック
                    //<--Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応
                    //scheduleSelectionStandard    →1つ目のロットのデータ
                    //lot.ScheduleSelectionStandard→2つ目以降のロットのデータ
                    string[] elements1st = scheduleSelectionStandard.Split(',');
                    string[] elementsother = lot.ScheduleSelectionStandard.Split(',');
                    //個数違いチェック
                    if (elements1st.Length != elementsother.Length)
                    {
                        msg = $"予定選別規格の数が一致しません ロット1:「{lot1st}」 ロット2:「{lot.NascaLotNo}」";
                        return true;
                    }
                    //個数は同じだが、違う予定選別規格が設定されていたらNGへ。
                    int okcnt = 0;
                    foreach (string element1st in elements1st)
                    {
                        if (elementsother.Contains(element1st) == true)
                        {
                            okcnt = okcnt + 1;
                        }
                        else
                        {
                            msg = $"予定選別規格が一致しません ロット:「{lot.NascaLotNo}」 予定選別規格:「{lot.ScheduleSelectionStandard}」 読込済み：「{scheduleSelectionStandard}」";
                            return true;
                        }
                    }
                    if (elements1st.Length != okcnt)
                    {
                        msg = $"予定選別規格が全て一致しません ロット:「{lot.NascaLotNo}」 予定選別規格:「{lot.ScheduleSelectionStandard}」 読込済み：「{scheduleSelectionStandard}」";
                        return true;
                    }
                    //--> Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応

                    if (mnggrid != lot.MnggrId)
                    {
                        msg = $"SpecBoxが一致しません ロット:「{lot.NascaLotNo}」 スペックボックス：「ID={lot.MnggrId},名={lot.MnggrNm}」 読込済み：「ID={mnggrid},名={mnggrnm}」";
                        return true;
                    }
                }

                //TG試験特性有の場合はブレンド禁止。応急対応　2015.08.04
                //TODO 次回改修時にブレンド禁止特性コードと閾値を設定ファイルに切り出す改修必要　
                LotChar[] chars = LotChar.GetLotChar(lot.NascaLotNo);
                var c = chars.Where(ch => ch.LotCharCd == "T0000003").FirstOrDefault();
                if (c != null && lotlist.Length >= 2)
                {
                    try
                    {
                        int testct = int.Parse(c.LotCharVal);
                        msg = $"Tg試験対象の対象ロットはブレンドできません ロット:「{lot.NascaLotNo}」";
                        if (testct >= 1) return true;
                    }
                    catch
                    {
                        //特性値を数値変換失敗した場合は無視する
                    }
                }
            }

            if (IsFirstArticleError(lotlist)) 
            {
                msg = "初品識別特性が一致しません:" + string.Join(",", lotlist.Select(l => l.NascaLotNo));
                return true;
            }

			// 2015.6.16 車載高 先行ライフ試験対応
			if (isTempCutBlendCheck)
			{
				if (IsTempCutBlendError(lotlist, out msg))
				{
					return true;
				}
			}

            msg = "";
            return false;
        }

		public static bool IsExactResinGp(AsmLot lot, List<string> resingroupcd) 
		{
            if(resingroupcd.Distinct().Count() != lot.ResinGpCd.Distinct().Count())
            {
                // 同じ樹脂Gが登録されている可能性を考慮する
                return false;
            }
            
			foreach (string rg in lot.ResinGpCd.Distinct())
			{
				if (resingroupcd.Contains(rg) == false)
				{
					return false;	
				}
			}

			return true;
		}

        //resingrcd2追加時に照合するために書いたがNTSVでカットブレンド予定が無いので未検証。
        //NTSVでカットブレンドする際には検証が必要。2018/5/29 湯浅/川口
        //public static bool IsExactResinGp2(AsmLot lot, List<string> resingroupcd)
        //{
        //    if (resingroupcd.Distinct().Count() != lot.ResinGpCd2.Distinct().Count())
        //    {
        //        // 同じ樹脂Gが登録されている可能性を考慮する
        //        return false;
        //    }

        //    foreach (string rg in lot.ResinGpCd2.Distinct())
        //    {
        //        if (resingroupcd.Contains(rg) == false)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        public static bool IsExactAimRank(AsmLot lot, int profileid, string aimrank, out string getAimRank)
        {
            getAimRank = string.Empty;

            if (profileid == lot.ProfileId)
            {
                // プロファイルが同じなら狙いランクも同じとする
                return true;
            }
            
            Profile prof = Profile.GetProfile(lot.ProfileId);
            if (prof != null)
            {
                getAimRank = prof.AimRank;
            }

            if (aimrank != getAimRank)
            {
                return false;
            }

            return true;
        }

        #endregion


        #region HasExpiredMaterial

        public static bool HasExpiredMaterial(MachineInfo m, Order o, AsmLot lot, out string errMsg)
        {
            Material[] materials;
            if (o.IsComplete)
            {
                materials = o.GetMaterials();
            }
            else
            {
                materials = m.GetMaterials(o.WorkStartDt, o.WorkEndDt);
            }

            //資材の有効期限チェック
            foreach (Material mat in materials)
            {
                bool isExpired = WorkChecker.IsExpiredUseLimit(mat.LimitDt, mat.RemoveDt, o.WorkStartDt, o.WorkEndDt);
                errMsg = "資材の有効期限を超過しています";
                if (isExpired) return true;

                //残り時間が*分切った場合も使用できないようにする 2016.05.06
                if (Config.Settings.RemainingTimeOfLimit != null)
                {
                    isExpired = WorkChecker.IsExpiredUseLimit(mat.LimitDt, mat.RemoveDt, o.WorkStartDt, o.WorkEndDt, -1 * Config.Settings.RemainingTimeOfLimit.Value);
                    errMsg = string.Format("資材の有効期限が{0}分を下回っています", Config.Settings.RemainingTimeOfLimit.Value);
                    if (isExpired) return true;
                }
            }


            //各投入資材の投入後有効期限チェック（主にダイボンド樹脂）
            foreach (Material mat in materials)
            {
                if (mat.InputDt.HasValue)
                {
                    DateTime dbResinLimit = Material.GetDBResinLimitDt(lot.TypeCd, mat.MaterialCd, mat.LotNo, mat.InputDt.Value, true);

                    bool isExpired = IsExpiredUseLimit(dbResinLimit, mat.RemoveDt, o.WorkStartDt, o.WorkEndDt);
                    errMsg = "資材の投入後有効期限を超過しています";
                    if (isExpired) return true;

                    //残り時間が*分切った場合も使用できないようにする 2016.05.06
                    if (Config.Settings.RemainingTimeOfLimit != null)
                    {
                        isExpired = IsExpiredUseLimit(dbResinLimit, mat.RemoveDt, o.WorkStartDt, o.WorkEndDt, -1 * Config.Settings.RemainingTimeOfLimit.Value);
                        errMsg = string.Format("資材の有効期限が{0}分を下回っています", Config.Settings.RemainingTimeOfLimit.Value);
                        if (isExpired) return true;
                    }

                    //DB樹脂の解凍→投入までの有効期限判定（本来は資材投入時だが、タイプ取得がここしかできないため）
                    isExpired = Material.IsExpiredDBResinWhenInput(lot.TypeCd, mat.MaterialCd, mat.LotNo, mat.InputDt.Value, true);
                    if (isExpired == true)
                    {
                        errMsg = "DB樹脂解凍後の有効期限を超過しています";
                        return true;
                    }

                    //残り時間が*分切った場合も使用できないようにする 2016.05.06
                    if (Config.Settings.RemainingTimeOfLimit != null)
                    {
                        isExpired = Material.IsExpiredDBResinWhenInput(lot.TypeCd, mat.MaterialCd, mat.LotNo, mat.InputDt.Value, true, -1 * Config.Settings.RemainingTimeOfLimit.Value);
                        if (isExpired == true)
                        {
                            errMsg = string.Format("DB樹脂解凍後の有効期限が{0}分を下回っています", Config.Settings.RemainingTimeOfLimit.Value);
                            return true;
                        }
                    }
                }
            }

            errMsg = "";
            return false;
        }
        #endregion

        /// <summary>
        /// 製造条件（PDA条件ロット特性）の開始前照合
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="order"></param>
        /// <param name="mac"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool IsCondError(AsmLot lot, Order order, MachineInfo mac, out string errMsg)
        {
			string[] condCdList = WorkCondition.GetCheckCondCd(order.ProcNo, mac);
			Process p = Process.GetProcess(order.ProcNo);

			foreach (string condcd in condCdList)
			{
				List<WorkCondition> lotCondVals = WorkCondition.GetCondValsFromType(lot, condcd);
				string[] macCondVals = WorkCondition.GetCondValsFromMachine(mac.MacNo, condcd, order.WorkStartDt);

                if (lotCondVals.Any() == false || macCondVals.Any() == false)
                {
                    WorkCondition cond = WorkCondition.SearchCondition(condcd, null, null, false).FirstOrDefault();
                    string condnm = cond != null ? cond.CondName : string.Empty;
                    if (lotCondVals.Count == 0)
                    {
                        errMsg = $"特性チェックマスタが見つかりません: 型番『{lot.TypeCd}』,ロット『{lot.NascaLotNo}』,条件CD『{condcd}』,条件名『{condnm}』";
                        return true;
                    }

                    if (macCondVals.Length == 0)
                    {
                        errMsg = $"装置に製造条件が登録されていません: 装置No『{mac.MacNo}』,装置名『{mac.LongName}』:条件CD『{condcd}』,条件名『{condnm}』";
                        return true;
                    }
                }

				//2015.1.10 3in1改修 ロット条件と装置で作業、設備が違う場合エラー判定する
				//但し、LotCondにNullでない行が有った場合だけチェックをする
				foreach (string macval in macCondVals)
				{
					//作業、設備、値の判定
					if (lotCondVals.Exists(lc => string.IsNullOrWhiteSpace(lc.workCd) == false)
						&& lotCondVals.Exists(lc => string.IsNullOrWhiteSpace(lc.plantCd) == false))
					{
						if (!lotCondVals.Exists(lc => lc.CondVal == macval && lc.workCd == p.WorkCd && lc.plantCd == mac.NascaPlantCd))
						{
							errMsg = $"製造条件(条件CD『{condcd}』)が一致しません:[作業、設備、値の判定NG] 設備設定値[作業CD:『{p.WorkCd}』 設備番号:『{mac.NascaPlantCd}』 値:『{macval}』]"
                                   + $"型番『{lot.TypeCd}』のマスタ設定値[{string.Join(",", lotCondVals.Select(l => $"(作業CD:『{l.workCd}』 設備番号:『{l.plantCd}』 値:『{l.CondVal}』)"))}]";
                            return true;
						}
					}
					//作業、値の判定
					else if (lotCondVals.Exists(lc => string.IsNullOrWhiteSpace(lc.workCd) == false)) 
					{
						if (!lotCondVals.Exists(lc => lc.CondVal == macval && lc.workCd == p.WorkCd))
						{
							errMsg = $"製造条件(条件CD『{condcd}』)が一致しません:[作業、値の判定NG] 設備設定値[作業CD:『{p.WorkCd}』 値:『{macval}』]"
                                   + $"型番『{lot.TypeCd}』のマスタ設定値[{string.Join(",", lotCondVals.Select(l => $"(作業CD:『{l.workCd}』 値:『{l.CondVal}』)"))}]";
                            return true;
						}
					}
					//設備、値の判定
					else if (lotCondVals.Exists(lc => string.IsNullOrWhiteSpace(lc.plantCd) == false))
					{
						if (!lotCondVals.Exists(lc => lc.CondVal == macval && lc.plantCd == mac.NascaPlantCd))
						{
							errMsg = $"製造条件(条件CD『{condcd}』)が一致しません:[設備、値の判定NG] 設備番号:{mac.NascaPlantCd} 値:{macval}"
                                   + $"型番『{lot.TypeCd}』のマスタ設定値[{string.Join(",", lotCondVals.Select(l => $"(設備番号:『{l.plantCd}』 値:『{l.CondVal}』)"))}]";
                            return true;
						}
					}
					//値の判定
					else
					{
						if (!lotCondVals.Exists(lc => lc.CondVal == macval))
						{
							errMsg = $"製造条件が一致しません:[値の判定NG]{macval}"
                                   + $"型番『{lot.TypeCd}』のマスタ設定値[{string.Join(",", lotCondVals.Select(l => $"(値:『{l.CondVal}』)"))}]";
                            return true;
						}
					}
				}
			}

			errMsg = "";
			return false;
		}

        /// <summary>
        /// BOM情報と原材料を照合してBOMに無い資材投入を判定
        /// </summary>
        /// <param name="matlist"></param>
        /// <param name="bom"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool IsBomError(Material[] matlist, BOM[] bom, out string errMsg, AsmLot lot, string workCd, int? macno)
        {
            //必須資材情報確認 2012/11/13 SGA32.sasaki--------------------------------------------[start]
            if (lot != null)
            {
                if (Config.Settings.IsDBProfileRequireMode != true || workCd != "DB0009")
                {
                    RequireMaterial[] reqmats = Profile.GetRequiredMaterials(lot, workCd);
                    bool isError = WorkChecker.IsRequireMaterialError(matlist, reqmats, out errMsg);
                    if (isError)
                    {
                        return true;
                    }
                }
            }
            //----------------------------------------------------------------------------------[end]

            foreach (Material mat in matlist)
            {
                bool found = false;

                foreach (BOM bomitem in bom)
                {
                    if (mat.MaterialCd == bomitem.MaterialCd)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    if (macno.HasValue)
                    {
                        MachineInfo machine = MachineInfo.GetMachine(macno.Value, true);
                        errMsg = $"[{machine.MacNo}/{machine.LongName}]  ロット特性に含まれていない資材が割り当てられています。割当済み資材=";
                    }
                    else
                    {
                        errMsg = "ロット特性に含まれていない資材を割り当てようとしています。割当て資材=";
                    }
                    errMsg += $"[ロットNo{mat.LotNo}/品目CD{mat.MaterialCd}/品目名{mat.MaterialNm}] ";                    
                    if (lot != null)
                    {
                        errMsg += $"ロット特性情報=ロットNo:{lot.NascaLotNo}/プロファイルNo:{lot.ProfileId}/";
                    }
                    errMsg += $"投入可能資材:[{string.Join(",", bom.Select(b => $"品目CD:{b.MaterialCd}/品目名:{b.MaterialName})"))}]";
                    
                    return true;
                }				
            }

            errMsg = "";
            return false;
        }

		/// <summary>
		/// 原材料が投入可能作業か判定　3in1対応 n.yoshimoto 2015/1/13
		/// </summary>
		/// <param name="matlist"></param>
		/// <param name="typeCd"></param>
		/// <returns></returns>
		public static bool CanInputToWorkMaterial(Material[] matlist, string typeCd, string workCd, out string errMsg) 
		{
			foreach (Material mat in matlist)
			{	
				string[] workCdCond = WorkCondition.GetWorkCDFromType(typeCd, mat.MaterialCd);
				if (workCdCond.Length > 0 && workCdCond.Contains(workCd) == false)
				{
					errMsg = string.Format("投入可能作業に含まれていない資材です。資材ロット：{0} 投入作業:{1} 投入可能作業:{2}", mat.LotNo, workCd, string.Join(", ", workCdCond));
					return true;
				}
			}

			errMsg = "";
			return false;
		}

        /// <summary>
        /// 実績ロット特性で指定された資材の投入チェック
        /// </summary>
        /// <param name="matlist"></param>
        /// <param name="reqlist"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool IsRequireMaterialError(Material[] matlist, RequireMaterial[] reqlist, out string errMsg)
        {
            List<RequireMaterial> notFoundList = new List<RequireMaterial>();
            foreach (RequireMaterial req in reqlist)
            {
                bool found = false;

                foreach (string reqmat in req.Materials)
                {
                    foreach (Material mat in matlist)
                    {
                        if (mat.MaterialCd == reqmat)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found == true) break;
                }

                if (found == false)
                {
                    //errMsg = "ロット特性に登録されている資材が使用されていません";
                    //return true;
                    notFoundList.Add(req);
                }
            }

            if (notFoundList.Any() == true)
            {
                errMsg = "ロット特性に登録されている資材が使用されていません\r\n";
                for (int i = 0; i < notFoundList.Count(); i++)
                {
                    List<string> matNmList = new List<string>();
                    foreach(string s in notFoundList[i].Materials.Distinct())
                    {
                        try
                        {
                            matNmList.Add($"[{(new BOM(string.Empty, s)).MaterialName}]");
                        }
                        catch(Exception)
                        {
                            // 資材品目名取得時にエラーが発生した場合は、資材品目CDを格納
                            matNmList.Add($"[{s}]");
                        }
                    }
                    errMsg += $"  要求資材({i + 1}):{string.Join(" or ", matNmList)}\r\n";
                }
                errMsg += $"  投入資材:[資材品目名,ロットNo] = {string.Join(",", matlist.Select(l => "[" + l.MaterialNm + "," + l.LotNo + "]"))}";
                return true;
            }

            errMsg = "";
            return false;
        }

        #region IsExpiredUseLimit

        /// <summary>
        /// 汎用　使用期限判定
        /// </summary>
        /// <param name="limitdt"></param>
        /// <param name="removeDt"></param>
        /// <param name="workstart"></param>
        /// <param name="workCompleteDt"></param>
        /// <returns></returns>
        public static bool IsExpiredUseLimit(DateTime limitdt, DateTime? removeDt, DateTime workstart, DateTime? workCompleteDt)
        {
            return IsExpiredUseLimit(limitdt, removeDt, workstart, workCompleteDt, 0);
        }

        /// <summary>
        /// 汎用　使用期限判定
        /// </summary>
        /// <param name="limitdt"></param>
        /// <param name="removeDt"></param>
        /// <param name="workstart"></param>
        /// <param name="workCompleteDt"></param>
        /// <param name="addMinutes"></param>
        /// <returns></returns>
        public static bool IsExpiredUseLimit(DateTime limitdt, DateTime? removeDt, DateTime workstart, DateTime? workCompleteDt, int addMinutes)
        {
            DateTime compDate = workstart;
            if (workCompleteDt.HasValue)
            {
                compDate = workCompleteDt.Value;
            }

            if (removeDt.HasValue)
            {
                if (compDate >= removeDt.Value)
                {
                    compDate = removeDt.Value;
                    if (compDate < workstart) { return false; } // EndDTが引数workStartDT（作業開始日）より前の場合は無視。
                }
            }

            if (compDate > limitdt.AddMinutes(addMinutes))
            {
                // 設備資材の使用期限を過ぎています。
                // ログ出力（処理終了）
                return true;
            }

            return false;
        }

        #endregion

        #region IsRestricted

        /// <summary>
        /// 出荷規制（ローカル機能）
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static bool IsRestricted(string lotno, int procno, out string reason)
        {
            Restrict[] rlist = Restrict.SearchRestrict(lotno, procno, true);
            if (rlist.Length >= 1)
            {
                reason = rlist[0].Reason;
                return true;
            }
            else
            {
                reason = "";
                return false;
            }
        }

		public static bool IsRestrictedInPreviousProcess(string lotno, string typecd, int procno, out string reason)
		{
			bool isRestrict = false;
			reason = string.Empty;

			Restrict[] rList = Restrict.SearchRestrict(lotno, null, true);

			Process[] procList = Process.GetWorkFlow(typecd);

			int? currentProcOrderNo = procList.Where(p => p.ProcNo == procno)?.OrderBy(p => p.WorkOrder).Last().WorkOrder;

			if(currentProcOrderNo.HasValue == false)
			{
				throw new ApplicationException($"指定した工程No『{procno}』で作業情報が見つかりませんでした。");
			}

			IEnumerable<Process> previousProcList = procList.Where(p => p.WorkOrder <= currentProcOrderNo);

			if(previousProcList.Count() == 0)
			{
				return false;
			}

			foreach (Restrict restrict in rList)
			{
                //規制情報の工程が投入工程 かつ 理由区分 = 「作業開始規制チェック対象外」の場合
                if (restrict.ProcNo == procno && restrict.ReasonKb == Restrict.RESTRICT_REASON_KB_NOTCHECK_WORK_START)
                {
                    //規制チェックを行わない
                    continue;
                }

                //規制情報の工程Noが前工程情報リスト内に存在するか
                if (previousProcList.Count(p => p.ProcNo == restrict.ProcNo) > 0)
				{
                    reason += $"工程:{restrict.ProcNm}[{restrict.ProcNo}]/理由:{restrict.Reason}\r\n";
                    isRestrict = true;
				}
			}

			return isRestrict;
		}

        #endregion

        /// <summary>
        /// ウェハー引き当てチェック
        /// </summary>
        /// <returns></returns>
        public static bool IsWaferChangeError(Material[] waferlist, Order order, out string errMsg)
        {
            //チェック① ウェハー交換をしているのに(waferlistが2件以上)、ウェハ交換回数が0の場合は異常
            bool isChangeStocker = false;
            foreach (Material wafer in waferlist)
            {
				if (wafer.StockerNo == 0) continue;

                int stockerCount = waferlist.ToList().Where(w => w.StockerNo == wafer.StockerNo).Count();
                if (stockerCount >= 2) 
                {
                    isChangeStocker = true;
                    break;
                }
            }
            
            if (isChangeStocker && order.StockerChangeCt == 0) 
            {
                errMsg = string.Format("ウェハー交換がされていません。交換回数:{1}", 
                    string.Join(",", waferlist.Select(w => w.MaterialCd)), order.StockerChangeCt);
                return true;
            }

            //チェック② ウェハー段数の巻き戻しが起こって、最終段数まで戻らずそのままの状態は異常
			//if (!isChangeStocker)
			//{
			//	DBWaferLog[] waferlogs = DBWaferLog.GetQCILWaferLog(MachineInfo.GetMachine(order.MacNo).NascaPlantCd, order.WorkStartDt, order.WorkEndDt.Value);
			//	if (waferlogs.Count() == 0)
			//	{
			//		//データが存在しない場合は30秒後、もう一度再取得
			//		Thread.Sleep(30000);
			//		waferlogs = DBWaferLog.GetQCILWaferLog(MachineInfo.GetMachine(order.MacNo).NascaPlantCd, order.WorkStartDt, order.WorkEndDt.Value);
			//	}
			//	waferlogs = waferlogs.ToList().Where(w => !string.IsNullOrEmpty(w.SheetNO)).ToArray();
			//	if (waferlogs.Count() == 0)
			//	{
			//		Log.SysLog.Info(string.Format("ウェハー履歴が存在しません。 設備番号:{0} 開始日時:{1} 完了日時:{2}",
			//			MachineInfo.GetMachine(order.MacNo).NascaPlantCd, order.WorkStartDt, order.WorkEndDt.Value));
			//		errMsg = "";
			//		return false;
			//	}

			//	int maxSheetNO = waferlogs.Max(w => int.Parse(w.SheetNO));
			//	int lastSheetNO = int.Parse(waferlogs.OrderBy(w => w.LogDT).Last().SheetNO);

			//	if (maxSheetNO > lastSheetNO)
			//	{
			//		errMsg = string.Format("ウェハー段数の巻き戻しが発生しています。交換最大段数:{0} 最終交換段数{1}", maxSheetNO, lastSheetNO);
			//		return true;
			//	}
			//}

            errMsg = "";
            return false;
        }

        /// <summary>
        /// 初品識別チェック
        /// </summary>
        /// <param name="lots"></param>
        /// <returns></returns>
        public static bool IsFirstArticleError(AsmLot[] lots) 
        {
            foreach(AsmLot lot in lots)
            {
                LotChar[] lotchars = LotChar.GetLotChar(lot.NascaLotNo, Config.FIRST_ARTICLE_LOTCHAR_CD);

                foreach (AsmLot lot2 in lots) 
                {
                    if (lot.NascaLotNo == lot2.NascaLotNo) { continue; }

                    LotChar[] lotchars2 = LotChar.GetLotChar(lot2.NascaLotNo, Config.FIRST_ARTICLE_LOTCHAR_CD);

                    //特性値をすべて別ロットが含んでいるかチェック
                    foreach (LotChar lotchar in lotchars) 
                    {
                        if (!lotchars2.ToList().Exists(l => l.ListVal == lotchar.ListVal))
                        {
                            return true;
                        }
                    }

                    //特性無しと有りの相違チェック
                    if (lotchars.Count() == 0 && lotchars2.Count() != 0) 
                    {
                        return true;
                    }
                }
            }

            return false;
        }

		/// <summary>
		/// 先行ライフ試験対象ロットの仮カットブレンドチェック
		/// </summary>
		/// <param name="lots"></param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public static bool IsTempCutBlendError(AsmLot[] lots, out string errMsg) 
		{
			bool? isBeforeLifeTestMode = null;
			string isTempCutBlendNo = String.Empty;

			foreach(AsmLot lot in lots)
			{
				if (lot.IsBeforeLifeTest && lot.TempCutBlendNo == String.Empty && !lot.IsColorTest) //チェック条件から先行色調品を除外 2015.9.23 湯浅
				{
					errMsg = string.Format("先行ライフ試験対象のロットの為、仮カットブレンドの登録が必要です。LotNo:{0}", lot.NascaLotNo);
					return true;
				}

				if (isBeforeLifeTestMode.HasValue && lot.IsBeforeLifeTest != isBeforeLifeTestMode) 
				{
					errMsg = string.Format("先行ライフ試験対象のロットと未対象のロットはブレンドできません。LotNo:{0}", string.Join(",", lots.Select(l => l.NascaLotNo)));
					return true;	
				}
				isBeforeLifeTestMode = lot.IsBeforeLifeTest;

				if (isTempCutBlendNo != String.Empty && lot.TempCutBlendNo != isTempCutBlendNo)
				{
					errMsg = string.Format("仮カットブレンドIDが一致し無い為、ブレンドできません。LotNo:{0}", string.Join(",", lots.Select(l => l.NascaLotNo)));
					return true;
				}
				isTempCutBlendNo = lot.TempCutBlendNo;

				if (lot.IsBeforeLifeTest && !lot.IsColorTest) //チェック条件から先行色調品を除外
				{
					List<GnlMaster> lifeTestGoodResultList = GnlMaster.GetLifeTestGoodResult(lot.BeforeLifeTestCondCd).ToList();

                    //同仮ブレンドNoの全ロットを取得して全体で特性をチェックするように変更 2015.8.11 湯浅
                    AsmLot[] tempCutBlendLotList = AsmLot.SearchAsmLot(null, false, false, null, lot.TempCutBlendNo);

                    bool isLifeTestNg = true;

                    if (tempCutBlendLotList.Length != 0)
                    {
                        foreach (AsmLot tempLot in tempCutBlendLotList)
                        {
                            LotChar result = LotChar.GetLifeTestResult(tempLot.NascaLotNo);


                            if (result != null && result.ListVal != null && lifeTestGoodResultList.Exists(l => l.Val == result.ListVal) == true)
                            {
                                isLifeTestNg = false;
                            }
                        }
                    }

                    //仮ブレンドで組んだカットブレンドロットの特性も取得するように変更 2015.10.7 湯浅
                    LotChar blendresult = LotChar.GetLifeTestResult(lot.TempCutBlendNo);
                    if (blendresult != null)
                    {
                        if (lifeTestGoodResultList.Exists(l => l.Val == blendresult.ListVal) == true)
                        {
                            isLifeTestNg = false;
                        }
                    }
                    if (isLifeTestNg)
					{
						errMsg = string.Format("ライフ試験結果が「良」では無い為、ブレンドできません。LotNo:{0}", lot.NascaLotNo);
						return true;
					}
				}
			}

			errMsg = "";
			return false;
		}

		/// <summary>
		/// 投入作業装置チェック
		/// </summary>
		public static bool IsInputWorkMachineError(MachineInfo mac, int procNo, string typeCd, out string message)
		{
            message = string.Empty;

            //投入禁止装置判定
            if (MachineInfo.IsDenyType(mac.NascaPlantCd, typeCd) == true)
            {
                message = $"この装置『{mac.MacNo}/{mac.LongName}』は『{typeCd}』を投入できません。作業禁止設備マスタ(TmDenyType)に登録されています。";
                return true;
            }

			List<string> types = MachineInfo.GetAvailableTypes(mac.NascaPlantCd, null);
			if (types.Count == 0)
			{
				//規制の無い設備、作業はOK
				return false;
			}
			types = MachineInfo.GetAvailableTypes(mac.NascaPlantCd, procNo);

			if (types.Exists(t => t == typeCd))
			{
				return false;
			}
			else
			{
                Process proc = Process.GetProcess(procNo);
                string procNm = proc != null ? proc.InlineProNM : string.Empty;
                message = $"この装置『{mac.MacNo}/{mac.LongName}』は『{typeCd}』を投入できません。" 
                        + $"作業設備マスタ(TmWorkMacCond)に登録されていません。作業『{procNo}/{procNm}』";
                return true;
			}
		}

		/// <summary>
		/// DBウェハーの水洗浄後、有効期限チェック
		/// 2015.5.14 3in1高効率ラインのシステム変更依頼1(水洗浄)
		/// </summary>
		public static bool IsDbWaferWashedLimitError(Material mat, string typeCd, out string errMsg)
		{
			Material.WaferWashedLife life = Material.GetWaferWashedLife(typeCd, mat.TypeCd);
			if (life == null) 
			{
				// 有効期限の無いウェハーは対象外の為、チェックしない。
				errMsg = "";
				return false;
				//errMsg = string.Format("水洗浄の有効期限が存在しません。 製品型番:{0} ウェハ型番:{1}", typeCd, mat.MaterialCd);
				//return true;
			}

			DateTime? washedDt = MatChar.GetWashedLastDate(mat.MaterialCd, mat.LotNo);
			if (washedDt.HasValue == false)
			{
				errMsg = string.Format("水洗浄をした履歴が存在しません。 品目Cd:{0} LotNo:{1}", mat.MaterialCd, mat.LotNo);
				return true;
			}

			DateTime limitDt = Material.GetDBWaferWashedLimitDt(life, washedDt.Value);
			if (limitDt <= mat.InputDt)
			{
				errMsg = string.Format("水洗浄後、有効期限を超過しています。有効期限:{0}", limitDt);
				return true;
			}
			else
			{
				errMsg = "";
				return false;
			}
		}

		/// <summary>
		/// DBウェハーのブレンドコードチェック
		/// </summary>
		/// <param name="wafer"></param>
		/// <param name="lot"></param>
		/// <param name="workcd"></param>
		/// <param name="errMsg"></param>
		/// <returns></returns>
		public static bool IsDbWaferBlendCodeError(Material wafer, AsmLot lot, string workcd, out string errMsg) 
		{
			// 2015.5.13 [変更]ブレンド詳細マスタ複数作業CD取り込み
			//if (string.IsNullOrEmpty(wafer.WorkCd))
			if (wafer.WorkCd.Count == 0)
			{
				//ブレンドコード照合(作業コードの設定無し)
				if (lot.BlendCd != wafer.BlendCd)
				{
					errMsg = string.Format("ブレンドマスタと一致していません TnLot.blendcd:{0} = TnMaterials.blendcd:{1}"
						, lot.BlendCd, wafer.BlendCd);
					return true;
				}
			}
			else
			{
				//ブレンドコード、作業コード照合

				// 2015.5.13 [変更]ブレンド詳細マスタ複数作業CD取り込み
				//if (lot.BlendCd != wafer.BlendCd || workcd != wafer.WorkCd)
				if (lot.BlendCd != wafer.BlendCd || wafer.WorkCd.Exists(w => w == workcd) == false)
				{
					errMsg = string.Format("ブレンドマスタと一致していません TnLot.blendcd:{0} = TnMaterials.blendcd:{1}, TnLot.workcd:{2} = TnMaterials.workcd:{3}"
						, lot.BlendCd, wafer.BlendCd, workcd, wafer.WorkCd);
					return true;
				}
			}

			errMsg = "";
			return false;
		}

		public static bool IsDBWaferSupplyIdError(Material wafer, AsmLot lot, string workcd, out string errMsg) 
		{
			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT blendcd, workcd, SupplyId FROM TmBlend WITH(nolock) 
								WHERE wafermaterialcd = @WaferMaterialCd 
								AND blendcd = @BlendCd ";
				//supplyid = @SupplyId AND

				//cmd.Parameters.Add("@SupplyId", System.Data.SqlDbType.NVarChar).Value = wafer.Supplyid;
				cmd.Parameters.Add("@WaferMaterialCd", System.Data.SqlDbType.NVarChar).Value = wafer.MaterialCd;
				cmd.Parameters.Add("@BlendCd", System.Data.SqlDbType.NVarChar).Value = lot.BlendCd;

				cmd.CommandText = sql;
				bool isData = false;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while(rd.Read())
					{
						string wcd = rd["workcd"].ToString().Trim();
						if (string.IsNullOrWhiteSpace(wcd) == false)
						{
							if (wcd == workcd)
							{
								isData = true;
							}
							else 
							{
								isData = false;
                                //break;
                                continue;
							}
						}
						else
						{
							isData = true;
						}

						string sid = rd["SupplyId"].ToString().Trim();
						if (string.IsNullOrWhiteSpace(sid) == false)
						{
							if (sid == wafer.Supplyid)
							{
								isData = true;
							}
							else
							{
								isData = false;
                                //break;
                                continue;
							}
						}
						else
						{
							isData = true;
						}

						if (isData) break;
					}
				}

				if (isData)
				{
					errMsg = "";
					return false;
				}
				else 
				{
					errMsg = string.Format("ブレンドマスタと一致していません 供給ID:{0} ウェハ品目CD:{1} ロットブレンドCD:{2} 作業CD:{3}",
						wafer.Supplyid, wafer.MaterialCd, lot.BlendCd, workcd);
					return true;
				}
			}
		}


        /// <summary>
        /// 資材の樹脂照合 2016.11.5 湯浅
        /// </summary>
        /// <param name="wafer"></param>
        /// <param name="lot"></param>
        /// <param name="workcd"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool IsMatchResinGroup(Material mat, AsmLot lot, int procno, out string errMsg)
        {
            MatChar[] matResingGr = MatChar.GetMatChar(mat.MaterialCd, mat.LotNo, Config.RESINGROUP_LOTCHARCD);
            string[] matResinGrList;
            errMsg = String.Empty;

            //資材・アッセンロットが樹脂グループを持っていなければNG。
            if (matResingGr.Count() != 1)
            {
                errMsg = "資材が樹脂グループを持っていないか、複数所持しています。";
                return false;
            }
            else
            {
                matResinGrList = matResingGr[0].LotCharVal.Trim().Split(',');
            }
            
            if (lot.ResinGpCd.Count() == 0)
            {
                errMsg = "アッセンロットに樹脂グループが指定されていません。";
                return false;
            }

            List<string> lotResinGr = new List<string>();

            //アッセンロット2が樹脂グループを持っている場合は作業別の処理に移行。
            if(lot.ResinGpCd2 != null && lot.ResinGpCd2.Any() == true && string.IsNullOrWhiteSpace(lot.ResinGpCd2[0]) == false)
            {
                ArmsApi.Model.GnlMaster[] workList = ArmsApi.Model.GnlMaster.GetPBMixingWorkList();
                Process proc = Process.GetProcess(procno);
                workList = workList.Where(w => w.Code == proc.WorkCd).ToArray();

                if(workList.Any() == false)
                {
                    errMsg = $"樹脂グループ(PB実装2)が設定されていますが作業CDがマスタ(TmGeneral,Tid={Model.GnlMaster.TID_PBMIXINGWORKLIST})に存在しません。作業CD『{proc.WorkCd}』";
                    return false;
                }

                //作業コードで複数行データが取れるとシステムが破たんするのでその場合は仕様見直し必要。
                if (workList.Single().Val == "P0000007") //樹脂グループコード
                {
                    lotResinGr = lot.ResinGpCd;
                }
                else if (workList.Single().Val == "P0000264") //樹脂グループコード(PB実装2)
                {
                    lotResinGr = lot.ResinGpCd2;
                }
                else
                {
                    errMsg = $"樹脂グループ(PB実装2)が設定されていますが作業CDマスタ(TmGeneral,Tid={Model.GnlMaster.TID_PBMIXINGWORKLIST})の特性が不正です。作業CD『{proc.WorkCd}』";
                }
            }
            else
            {
                lotResinGr = lot.ResinGpCd;
            }

            //資材の樹脂グループすべてがロットの樹脂グループに含まれていなければNG。
            foreach (string matResin in matResinGrList)
            {
                if (lotResinGr.Contains(matResin) == false)
                {
                    errMsg = $"資材と樹脂グループが一致しません。資材：『{string.Join(",", matResinGrList)}』、ロット：『{string.Join(",",lotResinGr)}』";
                    return false;
                }
            }
            return true;
        }

        #region IsDummyAutoTran

         /// <summary>
        /// 前作業のダミー実績登録判定
        /// </summary>
        /// <param name="lot">ロット</param>
        /// <param name="ChkProcNo">規制チェック対象の作業</param>
        /// <returns>true：ダミー実績登録可能</returns>
        public static bool CanDummyAutoTran(AsmLot lot, int chkProcNo)
        {
            List<Process> wfList = Process.GetWorkFlow(lot.TypeCd).ToList();

            Process chkProc = wfList.Find(w => w.ProcNo == chkProcNo);
            if (chkProc == null) return false;
            
            // -------- 条件① 規制チェック対象の作業がTmWorkFlow上のレコードにおいて「autodummytran」列が1になっている
            if (chkProc.AutoDummyTranKb != Process.AutoDummyTran.TimeLimitChkWorkEnd) return false;

            // -------- 条件② ダミー実績登録対象作業がTmWorkFlow上のレコードにおいて「autodummytran」列が2になっている
            Process dummyProc = wfList.Where(w => w.WorkOrder < chkProc.WorkOrder).OrderBy(w => w.WorkOrder).Last();
            if (dummyProc.AutoDummyTranKb != Process.AutoDummyTran.DummyTran) return false;

            // -------- 条件③ 規制チェック対象作業からTmWorkFlowを遡って「autodummytran」が1 ⇒ 2 ⇒ 3 or 4と登録されている
            Process tgtProc = null;
            // 前作業より更に過去の工程リストをWorkOrderの逆順に抽出
            List<Process> decendProcListBeforePrev = wfList.Where(w => w.WorkOrder < dummyProc.WorkOrder).OrderByDescending(w => w.WorkOrder).ToList();
            foreach (Process p in decendProcListBeforePrev)
            {
                // 「autodummytran」が1 ⇒ 2 ⇒ 1 の場合は登録不可
                if (p.AutoDummyTranKb == Process.AutoDummyTran.DummyTran)
                {
                    return false;
                }

                if (p.AutoDummyTranKb == Process.AutoDummyTran.TimeLimitTgtWorkStart ||
                    p.AutoDummyTranKb == Process.AutoDummyTran.TimeLimitTgtWorkEnd)
                {
                    tgtProc = p;
                    break;
                }
            }
            if (tgtProc == null) return false;

            // -------- 条件④ 「autodummytran」の1 と 3 or 4の組み合わせのTmTimeLimitのレコードが存在する
            TimeLimit.JudgeKb tgtKb = TimeLimit.JudgeKb.Start;
            if (tgtProc.AutoDummyTranKb == Process.AutoDummyTran.TimeLimitTgtWorkEnd)
            {
                tgtKb = TimeLimit.JudgeKb.End;
            }
            TimeLimit.JudgeKb chkKb = TimeLimit.JudgeKb.Start;

            List<TimeLimit> limitList;
            try
            {
                limitList = TimeLimit.GetLimits(lot.TypeCd, tgtProc.WorkCd, tgtKb, chkProc.WorkCd, chkKb).ToList();
            }
            catch(Exception)
            {
                // TmTimeLimitのレコードが無ければ、TimeLimit.GetLimits関数内からここに飛んでくる
                return false;
            }
            // マスタが複数ある場合は、警告時間が正 かつ 警告時間が最も短いマスタを採用
            TimeLimit limit = limitList.Where(l => l.EffectLimit >= 0).OrderBy(l => l.EffectLimit).FirstOrDefault();
            // 警告時間が正のレコードが無い場合は、NG
            if (limit == null) return false;

            // -------- 条件⑤ 「autodummytran」の2の工程の作業実績が存在しない
            Order[] dummyProcOrderArray = Order.SearchOrder(lot.NascaLotNo, dummyProc.ProcNo, null, false, false);
            if (dummyProcOrderArray.Length > 0) return false;

            // -------- 条件⑥ 「autodummytran」の2の工程のTmProcess.autoupdmachinenoに値が入っていて、TmMachineに存在するmacnoである
            if (dummyProc.AutoUpdMachineNo.HasValue == false) return false;

            // -------- 条件⑦ 現在時刻が時間監視マスタの有効時間を超過していない
            List<Order> tgtOrderList = Order.SearchOrder(lot.NascaLotNo, tgtProc.ProcNo, null, false, false).ToList();
            if (tgtOrderList.Count() == 0) return false;

            DateTime from;
            if (tgtProc.AutoDummyTranKb == Process.AutoDummyTran.TimeLimitTgtWorkStart)
            {
                from = tgtOrderList.OrderBy(o => o.WorkStartDt).First().WorkStartDt;
            }
            else
            {
                from = tgtOrderList.Where(o => o.WorkEndDt.HasValue).OrderBy(o => o.WorkEndDt).Last().WorkEndDt.Value;
            }

            DateTime ExpireDt = from.AddMinutes(limit.EffectLimit);
            if (DateTime.Now > ExpireDt) return false;

            return true;
        }

        #endregion

        /// <summary>
        /// 装置と作業が紐づいていないか判定
        /// </summary>
        /// <param name="macno">装置番号</param>
        /// <param name="procno">作業番号</param>
        /// <returns>true：装置と作業が紐づいていない</returns>
        public static bool IsNotRelatedMachineAndProcess(int macno, int procno)
        {
            List<string> macGroup = MachineInfo.GetMachine(macno).MacGroup;

            //投入可能装置判定
            MachineInfo[] availableMachines = MachineInfo.GetAvailableMachines(procno, macGroup);
            foreach (MachineInfo a in availableMachines)
            {
                if (a.MacNo == macno)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool isDenyBlended(string typecd)
        {
            using (DataContext.ARMSDataContext ent = new DataContext.ARMSDataContext(Config.Settings.LocalConnString))
            {
                using (var t = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                {
                    if (ent.TmDenyCutBlendType.Where(c => c.delfg == false && c.typecd == typecd).Any())
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
    }
}
