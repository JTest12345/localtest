using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model.NASCA;
//#if DEBUG
//using ArmsApi.local.nichia.naweb_dev;
//#else
//using ArmsApi.local.nichia.naweb;
//#endif
using ArmsApi.local.nichia.naweb;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// SLN1 色調自動測定器
    /// </summary>
    public class ColorAutoMeasurer : MachineBase
    {
        #region PLCアドレス定義

        private string[] START_LOTNO_WORD_ADDRNOLIST_START()
        { return new string[] { "EM51000", "EM51010", "EM51020", "EM51030", "EM51040", "EM51050", "EM51060", "EM51070" }; }
        private const int LOTNO_WORD_ADDR_LENGTH = 10;

        private const string COMPLETE_TRAY_QRCODE_WORD_ADDR = "EM62620";
        private const int DATAMATRIX_WORD_LENGTH = 10;

        private const string COMPLETE_LOTNO_WORD_ADDR = "EM62640";
        private const string COMPLETE_STARTDT_WORD_ADDR = "EM62680";
        private const string COMPLETE_ENDDT_WORD_ADDR = "EM62690";
        
        /// <summary>
        /// 開始登録要求BIT
        /// </summary>
        public const string START_REQ_BIT_ADDR = "EM50051";

        /// <summary>
        /// 開始登録結果Word (OK = 1, NG = 2)
        /// </summary>
        public const string START_RESULT_WORD_ADDR = "EM50021";

        /// <summary>
        /// 完了登録要求BIT
        /// </summary>
        public const string COMPLETE_REQ_BIT_ADDR = "EM50052";

        #endregion
        
        #region 定数


        #endregion
        
        protected override void concreteThreadWork()
        {
            try
            {
                // 作業完了
                if ((Convert.ToInt32(Plc.GetBit(COMPLETE_REQ_BIT_ADDR))) == Convert.ToInt32(PLC.Common.BIT_ON))
                {
                    if (this.IsOutLine)
                    {
                        // NASCAへ作業完了登録
                        workCompleteToNasca();
                    }
                    else
                    {
                        // ARMSへ作業完了登録
                        workCompleteToArms();
                    }
                }
                
                // 作業開始
                if ((Convert.ToInt32(Plc.GetBit(START_REQ_BIT_ADDR))) == Convert.ToInt32(PLC.Common.BIT_ON))
                {
                    if (this.IsOutLine)
                    {
                        // NASCAの作業開始前照合
                        beforeWorkStartNascaConfirm();
                    }
                    else
                    {
                        // ARMSへ作業開始登録
                        workStartToArms();
                    }
                }
            }
            catch (Exception ex)
            {
                FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
                frmErr.ShowDialog();

                if (frmErr.Method == ErrorHandleMethod.None)
                {
                    throw;
                }
            }
        }

        private void beforeWorkStartNascaConfirm()
        {
            try
            {
                this.OutputSysLog("[開始処理] 開始");

                #region トラベルシート情報(EM51000～EM51079)からロットNoを取得  1ロット = 10アドレス × 最大8ロット

                SortedList<string, int> lotList = new SortedList<string, int>();
                string[] addressList = START_LOTNO_WORD_ADDRNOLIST_START();
                string orgresingroupcd = "";
                for (int i = 0; i < addressList.Length; i++)
                {
                    string qrcode = Plc.GetString(addressList[i], LOTNO_WORD_ADDR_LENGTH).Replace("\r","").Replace("\0","");
                    string lotno = PLC.Common.GetMagazineNo(qrcode);
                    if (string.IsNullOrWhiteSpace(lotno) == true)
                    {
                        continue;
                    }
                    // 読み込んだ8ロット内で重複があれば、エラー扱いとする
                    if (lotList.Any(l => l.Key == lotno) == true)
                    {
                        throw new ApplicationException($"読み込んだトラベルシートの『{lotList[lotno]}』段と『{i+1}』のロットNoが重複しています。");
                    }

                    string typecd = Importer.GetTypeFromLotNo(lotno);
                    string resingroup = Importer.GetLotCharVal(lotno, typecd, Config.RESINGROUP_LOTCHARCD).LotCharVal;
                    // 樹脂グループの一致チェック
                    if (string.IsNullOrWhiteSpace(orgresingroupcd) == true)
                    {
                        // 1ロット目
                        orgresingroupcd = resingroup;
                    }
                    else
                    {
                        // 2ロット目：樹脂GrCD混在チェック (カンマ区切りを含めて一つの文字列して扱う)
                        if (resingroup != orgresingroupcd)
                        {
                            throw new ApplicationException($"読込ロットの樹脂Grが他のロットと違います。" +
                                $"[段数：[{i + 1}段],ロットNo：{lotno},型番：{typecd}],樹脂Gr：{resingroup}],[樹脂Gr(他ロット)：{orgresingroupcd}]");
                        }
                    }

                    lotList.Add(lotno, i);
                }
                
                #endregion

                // 読み込んだロットNo(最大8ロット)を全てチェックする
                foreach (string lotno in lotList.Keys)
                {
                    #region NASCA上の前作業・次作業の実績チェック
                    List<NascaTranData> tranList = Importer.GetLotTranData(lotno);

                    bool tranOK = false;
                    foreach (string workcd in ArmsApi.Config.Settings.ColorAutoMeasurerWorkCdList)
                    {
                        // 対象作業CDの実績検索 (実績が無い場合は、判定NG)
                        int workId = tranList.FindIndex(t => t.WorkCd == workcd);
                        if (workId == -1)
                        {
                            continue;
                        }

                        // 自工程の開始実績チェック (開始日時があれば、判定NG)
                        if (tranList[workId].StartDt.HasValue == true)
                        {
                            continue;
                        }

                        // 前工程の完了実績チェック (完了日時がなければ、判定NG) ※初回工程の場合は、チェックしない
                        if (workId >= 1 && tranList[workId - 1].CompltDt.HasValue == false)
                        {
                            continue;
                        }

                        // 判定結果 = OK
                        tranOK = true;
                        break;
                    }
                    if (tranOK == false)
                    {
                        throw new ApplicationException($"NASCA実績上で前作業が未完了 または 開始実績が登録済みです。NASCA実績をメンテしてからマガジンを再投入して下さい。ロットNo：『{lotno}』,作業CD：『{string.Join(",", ArmsApi.Config.Settings.ColorAutoMeasurerWorkCdList)}』");
                    }

                    #endregion
                }

                #region PLCへ返信

                //判定結果をPLCに書き込み
                //現在は固定で「0」を書き込み
                Plc.SetWordAsDecimalData(START_RESULT_WORD_ADDR, 0);

                #endregion

                this.OutputSysLog("[開始処理] 完了");
            }
            catch (Exception ex)
            {
                //判定結果をPLCに書き込み
                //現在は固定で「1」を書き込み
                Plc.SetWordAsDecimalData(START_RESULT_WORD_ADDR, 1);

                throw new Exception($"{this.Name} [開始処理異常] 理由：{ex.Message}");
            }
            finally
            {
                //開始登録要求OFFをPLCに書き込み
                Plc.SetBit(START_REQ_BIT_ADDR, 1, Common.BIT_OFF);
            }
        }

        private void workStartToArms()
        {
            try
            {
                this.OutputSysLog("[開始処理] 開始");

                List<string> lots = getTravelSheetLots();

                string errMessage = string.Empty;
                if (isResinGroupEqusls(lots, out errMessage) == false)
                {
                    throw new ApplicationException(errMessage);
                }
                
                for (int i = 0; i < lots.Count; i++)
                {
                    Magazine svrMag = Magazine.GetCurrent(lots[i]);
                    AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                    Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);

                    if (Order.GetOrder(lot.NascaLotNo, p.ProcNo).Any())
                        continue;

                    VirtualMag mag = new VirtualMag();
                    mag.MagazineNo = svrMag.MagazineNo;
                    mag.ProcNo = p.ProcNo;
                    Order startOrder =CommonApi.GetWorkStartOrder(mag, this.MacNo);
                    ArmsApiResponse res = CommonApi.WorkStart(startOrder);
                    if (res.IsError)
                    {
                        throw new ApplicationException(res.Message);
                    }
                }

                //判定結果をPLCに書き込み
                //現在は固定で「0」を書き込み
                Plc.SetWordAsDecimalData(START_RESULT_WORD_ADDR, 0);

                this.OutputSysLog("[開始処理] 完了");
            }
            catch (Exception ex)
            {
                //判定結果をPLCに書き込み
                //現在は固定で「1」を書き込み
                Plc.SetWordAsDecimalData(START_RESULT_WORD_ADDR, 1);

                throw new Exception($"{this.Name} [開始処理異常] 理由：{ex.Message}");
            }
            finally
            {
                //開始登録要求OFFをPLCに書き込み
                Plc.SetBit(START_REQ_BIT_ADDR, 1, Common.BIT_OFF);
            }
        }

        private void workCompleteToNasca()
        {
            try
            {
                #region PLCからデータ収集
                string qrcode = Plc.GetString(COMPLETE_LOTNO_WORD_ADDR, LOTNO_WORD_ADDR_LENGTH, true);
                string lotno = PLC.Common.GetMagazineNo(qrcode);
                if (lotno == null)
                {
                    throw new ApplicationException($"ロットNoの取得に失敗しました。アドレス:『{COMPLETE_LOTNO_WORD_ADDR}』");
                }
                this.OutputSysLog($"[完了処理] ロットNoの取得成功 ロットNo:『{lotno}』");

                DateTime WorkStart;
                try
                {
                    WorkStart = Plc.GetWordsAsDateTime(COMPLETE_STARTDT_WORD_ADDR);
                }
                catch(Exception)
                {
                    throw new ApplicationException($"開始日時の取得に失敗しました。アドレス:『{COMPLETE_STARTDT_WORD_ADDR}』");
                }
                this.OutputSysLog($"[完了処理] 開始日時取得成功 StartDt:『{WorkStart}』");

                DateTime WorkComplete;
                try
                {
                    WorkComplete = Plc.GetWordsAsDateTime(COMPLETE_ENDDT_WORD_ADDR);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"完了日時の取得に失敗しました。アドレス:『{COMPLETE_ENDDT_WORD_ADDR}』");
                }
                this.OutputSysLog($"[完了処理] 完了日時取得成功 EndDt:『{WorkComplete}』");

                string workcd = null;
                List<NascaTranData> tranList = Importer.GetLotTranData(lotno);
                foreach(NascaTranData tran in tranList)
                {
                    if (Config.Settings.ColorAutoMeasurerWorkCdList.Contains(tran.WorkCd) == false)
                    {
                        continue;
                    }
                    if (tran.StartDt.HasValue == false)
                    {
                        workcd = tran.WorkCd;
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(workcd))
                {
                    throw new ApplicationException($"NASCA実績から自工程の作業CDの取得に失敗しました。ロットNo:『{lotno}』");
                }
                Process p = Process.GetProcess(workcd);
                if (p == null)
                {
                    throw new ApplicationException($"自工程の作業CDがTmProcessに登録されていません。作業CD：『{workcd}』");
                }

                string typecd = Importer.GetTypeFromLotNo(lotno);
                if (string.IsNullOrWhiteSpace(typecd))
                {
                    throw new ApplicationException($"NASCA指図から型番の取得に失敗しました。ロットNo:『{lotno}』");
                }

                string trayQR = Plc.GetString(COMPLETE_TRAY_QRCODE_WORD_ADDR, DATAMATRIX_WORD_LENGTH, true);
                if (string.IsNullOrWhiteSpace(trayQR) == true)
                {
                    throw new ApplicationException($"装置から収納基板情報が取得できません。取得アドレス(開始)：『{COMPLETE_TRAY_QRCODE_WORD_ADDR}』");
                }
                trayQR = trayQR.Replace("\r", "");

                #endregion

                #region ファイルのリネーム

                List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, WorkStart.AddMinutes(-5), WorkComplete);
                foreach (string lotFile in lotFiles)
                {
                    MachineLog.ChangeFileName(lotFile, lotno, typecd, p.ProcNo, lotno);
                    this.OutputSysLog($"[完了処理] ロットファイル名称変更 FileName:『{lotFile}』");
                }

                #endregion
                
                #region 収納基板情報取得 + TnLotCarrierに紐付け

                //TnLotCarrier登録&前データがある場合フラグOFF
                LotCarrier oldLotCarrier = LotCarrier.GetData(trayQR, true, false);
                if (oldLotCarrier != null && oldLotCarrier.LotNo != lotno)
                {
                    LotCarrier.UpdateOperateFg(oldLotCarrier.LotNo, oldLotCarrier.CarrierNo, false);
                }
                
                LotCarrier svrData = LotCarrier.GetData(trayQR, lotno, false);
                if(svrData == null)
                {
                    LotCarrier lotcarrier = new LotCarrier(lotno, trayQR, "660");
                    lotcarrier.Insert();
                }
                else
                {
                    LotCarrier.UpdateOperateFg(lotno, trayQR, true);
                }



                #endregion

                #region NASCA実績登録

                NASCAResponse res = NASCAResponse.GetNGResponse("アッセンロット連携異常");
                string mnfctString;
                NppNelMnfctParamInfo mnfctInfo = NascaPubApi.GetMnfctParamInfo(lotno,
                    "", //マガジン連番0なら空白
                    this.PlantCd, WorkStart, WorkComplete, null, null, null, null, null, null, null, null, 0, out mnfctString);

                this.OutputSysLog($"[完了処理] NASCA SEND:『{mnfctString}』");
                NascaPubApi api = NascaPubApi.GetInstance();
                res = api.WriteMnfctResult(mnfctInfo, true);
                this.OutputSysLog($"[完了処理] NASCA RECEIVE:『{res.OriginalMessage}』");

                #endregion

                #region PLCへ返信

                //排出要求OFFをPLCに書き込み
                Plc.SetBit(COMPLETE_REQ_BIT_ADDR, 1, Common.BIT_OFF);

                #endregion

                #region NASCAの実績登録を失敗した時に手動登録のメッセージを表示する。
                if (res.Status != NASCAStatus.OK)
                {
                    string errMsg = $"【重要！】NASCA実績登録に失敗しました。理由:『{res.OriginalMessage}』\r\n " +
                        $"■作業指示：NASCAの実績登録画面で手動で実績登録して下さい。\r\n" +
                        $"  ロットNo：『{lotno}]』\r\n" +
                        $"  作業CD：『{workcd}』\r\n" +
                        $"  設備CD：『{this.PlantCd}』\r\n" +
                        $"  作業開始時刻：『{WorkStart}』\r\n" +
                        $"  作業完了時刻：『{WorkComplete}』\r\n";
                    throw new ApplicationException(errMsg);
                }
                #endregion

                this.OutputSysLog($"[完了処理] 完了 ロットNo:『{lotno}』");
            }
            catch (Exception ex)
            {
                throw new Exception($"{this.Name} [完了処理異常] 理由：{ex.Message}");
            }
        }

        private void workCompleteToArms()
        {
            try
            {
                this.OutputSysLog("[完了処理] 開始");

                #region PLCからデータ収集
                string qrcode = Plc.GetString(COMPLETE_LOTNO_WORD_ADDR, LOTNO_WORD_ADDR_LENGTH, true);
                string lotno = PLC.Common.GetMagazineNo(qrcode);
                if (lotno == null)
                {
                    throw new ApplicationException($"ロットNoの取得に失敗しました。アドレス:『{COMPLETE_LOTNO_WORD_ADDR}』");
                }
                this.OutputSysLog($"[完了処理] ロットNoの取得成功 ロットNo:『{lotno}』");

                DateTime workStartDt;
                try
                {
                    workStartDt = Plc.GetWordsAsDateTime(COMPLETE_STARTDT_WORD_ADDR);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"開始日時の取得に失敗しました。アドレス:『{COMPLETE_STARTDT_WORD_ADDR}』");
                }
                this.OutputSysLog($"[完了処理] 開始日時取得成功 StartDt:『{workStartDt}』");

                DateTime workCompleteDt;
                try
                {
                    workCompleteDt = Plc.GetWordsAsDateTime(COMPLETE_ENDDT_WORD_ADDR);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"完了日時の取得に失敗しました。アドレス:『{COMPLETE_ENDDT_WORD_ADDR}』");
                }
                this.OutputSysLog($"[完了処理] 完了日時取得成功 EndDt:『{workCompleteDt}』");

                string trayQR = Plc.GetString(COMPLETE_TRAY_QRCODE_WORD_ADDR, DATAMATRIX_WORD_LENGTH, true);
                if (string.IsNullOrWhiteSpace(trayQR) == true)
                {
                    throw new ApplicationException($"装置から収納基板情報が取得できません。取得アドレス(開始)：『{COMPLETE_TRAY_QRCODE_WORD_ADDR}』");
                }
                trayQR = trayQR.Replace("\r", "");

                #endregion

                Magazine svrMag = Magazine.GetCurrent(lotno);
                AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);

                #region ファイルのリネーム

                List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, workStartDt.AddMinutes(-5), workCompleteDt);
                foreach (string lotFile in lotFiles)
                {
                    MachineLog.ChangeFileName(lotFile, lotno, lot.TypeCd, p.ProcNo, lotno);
                    this.OutputSysLog($"[完了処理] ロットファイル名称変更 FileName:『{lotFile}』");
                }

                #endregion

                #region 収納基板情報取得 + TnLotCarrierに紐付け

                //TnLotCarrier登録&前データがある場合フラグOFF
                LotCarrier oldLotCarrier = LotCarrier.GetData(trayQR, true, false);
                if (oldLotCarrier != null && oldLotCarrier.LotNo != lotno)
                {
                    LotCarrier.UpdateOperateFg(oldLotCarrier.LotNo, oldLotCarrier.CarrierNo, false);
                }

                LotCarrier svrData = LotCarrier.GetData(trayQR, lotno, false);
                if (svrData == null)
                {
                    LotCarrier lotcarrier = new LotCarrier(lotno, trayQR, "660");
                    lotcarrier.Insert();
                }
                else
                {
                    LotCarrier.UpdateOperateFg(lotno, trayQR, true);
                }

                #endregion

                VirtualMag mag = new VirtualMag();
                mag.MagazineNo = lotno;
                mag.LastMagazineNo = lotno;
                mag.ProcNo = p.ProcNo;
                mag.WorkStart = workStartDt;
                mag.WorkComplete = workCompleteDt;

                Order endOrder = CommonApi.GetWorkEndOrder(mag, this.MacNo, this.LineNo);
                ArmsApiResponse res = CommonApi.WorkEnd(endOrder);
                if (res.IsError)
                {
                    string errMsg = $"【重要！】実績登録に失敗しました。理由:『{res.Message}』\r\n " +
                        $"■作業指示：ARMSデータメンテナンスで実績登録して下さい。\r\n" +
                        $"  ロットNo：『{lotno}]』\r\n" +
                        $"  作業：『{p.InlineProNM}』\r\n" +
                        $"  設備CD：『{this.PlantCd}』\r\n" +
                        $"  作業開始時刻：『{workStartDt}』\r\n" +
                        $"  作業完了時刻：『{workCompleteDt}』\r\n";
                    throw new ApplicationException(errMsg);
                }

                //排出要求OFFをPLCに書き込み
                Plc.SetBit(COMPLETE_REQ_BIT_ADDR, 1, Common.BIT_OFF);

                this.OutputSysLog("[完了処理] 完了");
            }
            catch (Exception ex)
            {
                throw new Exception($"{this.Name} [完了処理異常] 理由：{ex.Message}");
            }
        }

        private List<string> getTravelSheetLots()
        {
            SortedList<string, int> lotList = new SortedList<string, int>();

            string[] addressList = START_LOTNO_WORD_ADDRNOLIST_START();
            for (int i = 0; i < addressList.Length; i++)
            {
                string qrcode = Plc.GetString(addressList[i], LOTNO_WORD_ADDR_LENGTH).Replace("\r", "").Replace("\0", "");
                string lotno = PLC.Common.GetMagazineNo(qrcode);
                if (string.IsNullOrWhiteSpace(lotno) == true)
                {
                    continue;
                }

                // 読み込んだ8ロット内で重複があれば、エラー扱いとする
                if (lotList.Any(l => l.Key == lotno) == true)
                {
                    throw new ApplicationException($"読み込んだトラベルシートの『{lotList[lotno]}』段と『{i + 1}』のロットNoが重複しています。");
                }               

                lotList.Add(lotno, i);
            }

            return lotList.Keys.ToList();
        }

        /// <summary>
        /// 対象ロットの樹脂グループが対象ロットリストの樹脂グループと同一か確認
        /// </summary>
        /// <param name="lots"></param>
        /// <returns></returns>
        private bool isResinGroupEqusls(List<string> lots, out string errMessage)
        {
            errMessage = "";

            if (lots.Count == 0)
                throw new ApplicationException("対象ロットリストが空の為、樹脂グループが同一か照合できません。");
            
            if (lots.Count == 1)
                return true;

            string firstLotNo = lots.First();
            List<string> firstLotResinGroup = AsmLot.GetAsmLot(firstLotNo).ResinGpCd;

            foreach(string lotno in lots)
            {
                if (lotno == firstLotNo) continue;

                AsmLot lot = AsmLot.GetAsmLot(lotno);
                foreach (string resingroup in firstLotResinGroup)
                {
                    if (lot.ResinGpCd.Where(l => l == resingroup).Any() == false)
                    {
                        // 一致する樹脂グループが無かった時点で関数終了(false)
                        errMessage = $"読込ロットの樹脂Grが違うロットが存在します。 ロットNo:{firstLotNo} 樹脂Gr:{ string.Join(",", firstLotResinGroup)}";
                        return false;
                    }
                }
            }

            return true;
        }


    }
}
