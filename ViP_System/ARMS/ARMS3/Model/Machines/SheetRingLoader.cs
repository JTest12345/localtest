using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.local.nichia.naweb;
using ArmsApi.Model;
using ArmsApi.Model.NASCA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 蛍光体シートリング搭載自動機
    /// </summary>
    public class SheetRingLoader : MachineBase
    {
        /// <summary>
        /// 運転中Bit
        /// </summary>
        private const string MACHINE_READY_BIT_ADDR = "EM50070";

        /// <summary>
        /// 開始処理要求Bit
        /// </summary>
        public const string START_REQ_BIT_ADDR = "EM50051";

        /// <summary>
        /// 開始処理結果Word (OK = 1, NG = 2)
        /// </summary>
        public const string START_RESULT_WORD_ADDR = "EM50021";

        /// <summary>
        /// 完了処理要求Bit
        /// </summary>
        public const string COMPLETE_REQ_BIT_ADDR = "EM50052";

        /// <summary>
        /// 開始処理時トラベルシートロットNo取得用(各段の開始アドレスのリスト)
        /// </summary>
        private string[] START_LOTNO_WORD_ADDRNOLIST_START
        {
            get { return new string[] { "EM51400", "EM51410", "EM51420", "EM51430", "EM51440", "EM51450", "EM51460", "EM51470" }; }
        }

        /// <summary>
        /// 完了処理時マガジン収納情報取得用(各段の開始アドレスのリスト)
        /// </summary>
        private string[] END_MAGAZINEINFO_WORD_ADDRNOLIST_START
        {
            get { return new string[] { "EM60000", "EM60200", "EM60400", "EM60600", "EM60800", "EM61000", "EM61200", "EM61400", "EM61600", "EM61800", "EM62000", "EM62200", "EM62400", "EM62600", "EM62800", "EM63000", "EM63200", "EM63400", "EM63600", "EM63800", "EM64000", "EM64200", "EM64400", "EM64600" }; }
        }

        /// <summary>
        /// 完了ロットNo
        /// 全段同ロットが搭載される想定なので、1段目のトラベルシート情報から取得
        /// </summary>
        private const string COMPLETE_LOTNO_WORD_ADDR = "EM60040";  

        /// <summary>
        /// 現在搭載中のロットNo
        /// </summary>
        private const string NOWLOADING_LOTNO_WORD_ADDR = "EM51580";

        /// <summary>
        /// 現在搭載中のトレイNo
        /// </summary>
        private const string NOWLOADING_TRAYNO_WORD_ADDR = "EM51540";

        /// <summary>
        /// 搭載を開始した時間
        /// </summary>
        private const string MAGAZINE_STARTDATE_WORD_ADDR = "EM65080";

        /// <summary>
        /// 搭載を完了した時間(最終リングに貼り付け終わった時間)
        /// </summary>
        private const string MAGAZINE_ENDDATE_WORD_ADDR = "EM65090";
        
        /// <summary>
        /// マガジンの総段数
        /// </summary>
        public int MagazineMaxStages
        {
            get { return END_MAGAZINEINFO_WORD_ADDRNOLIST_START.Count(); }
        }

        private const int LOTNO_WORD_ADDR_LENGTH = 10;
        private const int TRAYNO_WORD_ADDR_LENGTH = 10;
        private const int RINGNO_WORD_ADDR_LENGTH = 10;

        protected override void concreteThreadWork()
        {
            try
            {
                if ((Convert.ToInt32(Plc.GetBit(MACHINE_READY_BIT_ADDR))) == Convert.ToInt32(PLC.Common.BIT_ON))
                {
                    if ((Convert.ToInt32(Plc.GetBit(COMPLETE_REQ_BIT_ADDR))) == Convert.ToInt32(PLC.Common.BIT_ON))
                    {
                        workComplete();
                    }

                    if ((Convert.ToInt32(Plc.GetBit(START_REQ_BIT_ADDR))) == Convert.ToInt32(PLC.Common.BIT_ON))
                    {
                        workStart();
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
        private void workStart()
        {
            try
            {       
                this.OutputSysLog("[開始処理] 開始");

                // 読み込んだロットNo(最大8ロット)を全てチェックする
                SortedList<int, string>lotList = getTravelSheetLots();
                foreach (KeyValuePair<int, string> stepAndLotNo in lotList)
                {
                    AsmLot lot = new AsmLot();
                    lot.NascaLotNo = stepAndLotNo.Value;

                    this.OutputSysLog($"[開始処理] チェック開始 [ロットNo:『{lot.NascaLotNo}』]");

                    #region NASCA上の前作業・次作業の実績チェック 

                    List<NascaTranData> tranList = Importer.GetLotTranData(lot.NascaLotNo);

                    bool tranOK = false;
                    string inputworkcd = string.Empty;
                    foreach (string workcd in ArmsApi.Config.Settings.SheetRingLoaderWorkCdList)
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
                        inputworkcd = workcd;
                        tranOK = true;
                        break;
                    }
                    if (tranOK == false)
                    {
                        throw new ApplicationException($"NASCA実績上で前作業が未完了 または 開始実績が登録済みです。" +
                            $"NASCA実績をメンテしてからマガジンを再投入して下さい。ロットNo：『{lot.NascaLotNo}』," +
                            $"作業CD：『{string.Join(",", ArmsApi.Config.Settings.AutoPasterWorkCdList)}』");
                    }

                    #endregion
                }

                string nowLotNo = getNowLoadingLotNo();
                string nowTrayNo = getNowLoadingTrayNo();

                if (lotTrayEquals(nowLotNo, nowTrayNo) == false)
                {
                    throw new ApplicationException($"現在作業中のトラベルシート情報ロットNo:{nowLotNo}とトレイNo:{nowTrayNo}に紐づくロットNoが一致しません。");
                }

                //判定OKをPLCに書き込み
                Plc.SetWordAsDecimalData(START_RESULT_WORD_ADDR, 0);

                this.OutputSysLog("[開始処理] 正常完了");
            }
            catch (Exception ex)
            {
                //判定NGをPLCに書き込み
                Plc.SetWordAsDecimalData(START_RESULT_WORD_ADDR, 1);
                throw new Exception($"{this.Name} [開始処理異常] 理由：{ex.Message}");
            }
            finally
            {
                Plc.SetBit(START_REQ_BIT_ADDR, 1, Common.BIT_OFF);
                this.OutputSysLog("[開始処理] 作業許可判定要求OFF");
            }
        }

        private void workComplete()
        {
            try
            {
                this.OutputSysLog("[完了処理] 開始");

                // NASCA実績登録
                string qrCode = Plc.GetString(COMPLETE_LOTNO_WORD_ADDR, LOTNO_WORD_ADDR_LENGTH);
                string lotNo = PLC.Common.GetMagazineNo(qrCode);
                if (lotNo == null)
                {
                    throw new ApplicationException($"ロットNoの取得に失敗しました。[アドレス:『{COMPLETE_LOTNO_WORD_ADDR}』]");
                }
                this.OutputSysLog($"[完了処理] ロットNoの取得成功 [ロットNo:『{lotNo}』]");

                DateTime workStartDt = getMagazineStartDate();
                DateTime workCompleteDt = getMagazineCompleteDate();

                Process nowProc = Process.GetNowProcessFromNasca(lotNo, Config.Settings.SheetRingLoaderWorkCdList);
                string magTypeCd = Importer.GetTypeFromLotNo(lotNo);
                if (string.IsNullOrWhiteSpace(magTypeCd))
                {
                    throw new ApplicationException($"NASCA指図から型番の取得に失敗しました。[ロットNo:『{lotNo}』]");
                }
                this.OutputSysLog($"[完了処理] 型番の取得成功 [型番:『{magTypeCd}』]");
                
                NASCAResponse res = new NASCAResponse();
                res = NASCAResponse.GetNGResponse("アッセンロット連携異常");
                string mnfctString;
                NppNelMnfctParamInfo mnfctInfo = NascaPubApi.GetMnfctParamInfo(lotNo,
                    "", //マガジン連番0なら空白
                    this.PlantCd, workStartDt, workCompleteDt, null, null, null, null, null, null, null, null, 0, out mnfctString);
                this.OutputSysLog($"[完了処理] NASCA SEND:『{mnfctString}』");
                NascaPubApi api = NascaPubApi.GetInstance();
                res = api.WriteMnfctResult(mnfctInfo, true);
                this.OutputSysLog($"[完了処理] NASCA RECEIVE:『{res.OriginalMessage}』");

                // ロットとリングの紐づけ
                foreach (KeyValuePair<int, string> ring in getPastedRings())
                {
                    // 過去に同リングの紐づけがある場合、解除する
                    LotCarrier.CancelCarrier(ring.Value);
                    LotCarrier newCarrier = new LotCarrier(lotNo, ring.Value);
                    newCarrier.Insert();
                    this.OutputSysLog($"[完了処理] ロット-リング紐付け完了 [ロットNo:『{lotNo}』, リング：[{ring.Value}]");
                }

                // 実績要求OFFをPLCに書き込み
                Plc.SetBit(COMPLETE_REQ_BIT_ADDR, 1, Common.BIT_OFF);

                // NASCAの実績登録を失敗した時に手動登録のメッセージを表示する。
                if (res != null && res.Status != NASCAStatus.OK)
                {
                    string errMsg = $"【重要！】NASCA実績登録に失敗しました。理由:『{res.OriginalMessage}』\r\n " +
                        $"■作業指示：NASCAの実績登録画面で手動で実績登録して下さい。\r\n" +
                        $"  ロットNo：『{lotNo}]』\r\n" +
                        $"  作業CD：『{nowProc.WorkCd}』\r\n" +
                        $"  設備CD：『{this.PlantCd}』\r\n" +
                        $"  作業開始時刻：『{workStartDt}』\r\n" +
                        $"  作業完了時刻：『{workCompleteDt}』\r\n";
                    throw new ApplicationException(errMsg);
                }

                this.OutputSysLog($"[完了処理] 完了 [ロットNo:『{lotNo}』]");
            }
            catch (Exception ex)
            {
                throw new Exception($"{this.Name} [完了処理異常] 理由：{ex.Message}");
            }
        }

        /// <summary>
        /// 作業許可判定時のトラベルシート情報(EM51400～EM51479)からロットNoを取得  1ロット = 10アドレス × 最大8ロット
        /// </summary>
        /// <returns></returns>
        private SortedList<int, string> getTravelSheetLots()
        {
            SortedList<int, string> retv = new SortedList<int, string>();
            string[] addressList = START_LOTNO_WORD_ADDRNOLIST_START;
            for (int i = 0; i < addressList.Length; i++)
            {
                string qrCode = Plc.GetString(addressList[i], LOTNO_WORD_ADDR_LENGTH);
                string lotNo = PLC.Common.GetMagazineNo(qrCode);
                if (string.IsNullOrWhiteSpace(lotNo) == true)
                {
                    continue;
                }
                // 読み込んだ8ロット内で重複があれば、対象外とする (1と2段目, 3と4段目が同じロットNoなので重複チェックができない)
                if (retv.Any(l => l.Value == lotNo) == true)
                {
                    continue;
                }
                this.OutputSysLog($"[開始処理] トラベルシートのロットNoの取得成功 [段数: {i + 1}段目 ロットNo:『{lotNo}』]");

                retv.Add(i+1, lotNo);
            }

            return retv;
        }

        private SortedList<int, string> getPastedRings()
        {
            SortedList<int, string> retv = new SortedList<int, string>();

            for (int i = 0; i < MagazineMaxStages; i++)
            {
                string ringCd = getRingCode(i + 1);
                if (string.IsNullOrEmpty(ringCd))
                    continue;

                retv.Add(i+1, ringCd);
            }

            return retv;
        }

        private string getNowLoadingLotNo()
        {
            string qrCode = Plc.GetString(NOWLOADING_LOTNO_WORD_ADDR, LOTNO_WORD_ADDR_LENGTH);
            string lotNo = PLC.Common.GetMagazineNo(qrCode);

            return lotNo;
        }

        private string getNowLoadingTrayNo()
        {
            string qrCode = Plc.GetString(NOWLOADING_TRAYNO_WORD_ADDR, TRAYNO_WORD_ADDR_LENGTH, true);
            string trayNo = qrCode.Replace("\r", "").Replace("\0", "");
            
            return trayNo;
        }

        /// <summary>
        /// トラベルシートのロットとトレイに紐付いているロットを照合する
        /// </summary>
        /// <param name="travalSheetLotNo"></param>
        /// <param name="trayNo"></param>
        /// <returns></returns>
        private bool lotTrayEquals(string travalSheetLotNo, string trayNo)
        {
            string[] trayLotNoArray = LotCarrier.GetLotNo(trayNo, true);
            if (trayLotNoArray.Length == 0)
            {
                throw new ApplicationException($"トレイQRに紐づいているロットが存在しません。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                    $"[トレイQR(キャリアNo)：『{trayNo}』]");
            }
            else if (trayLotNoArray.Length >= 2)
            {
                throw new ApplicationException($"トレイQRに紐づいているロットが複数存在します。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                    $"[トレイQR(キャリアNo)：『{trayNo}』]");
            }

            if (travalSheetLotNo == trayLotNoArray.Single())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private DateTime getMagazineStartDate()
        {
            return this.Plc.GetWordsAsDateTime(MAGAZINE_STARTDATE_WORD_ADDR);
        }

        private DateTime getMagazineCompleteDate()
        {
            return this.Plc.GetWordsAsDateTime(MAGAZINE_ENDDATE_WORD_ADDR);
        }

        private string getRingCode(int magazineStage)
        {
            if (END_MAGAZINEINFO_WORD_ADDRNOLIST_START.Count() < magazineStage)
            {
                throw new ApplicationException("最大段数より多い段数が指定されています。");
            }

            string ringStartAddress = END_MAGAZINEINFO_WORD_ADDRNOLIST_START[magazineStage - 1];
            string idStartAddress = ringStartAddress.Substring(0, 5) + "20";

            return this.Plc.GetString(idStartAddress, RINGNO_WORD_ADDR_LENGTH).Replace("\r", "").Replace("\0", "");
        } 
    }
}
