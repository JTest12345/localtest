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
    /// SLN1 自動貼付装置
    /// </summary>
    public class AutoPaster : MachineBase
    {

        #region PLCアドレス定義

        private const int LOTNO_WORD_ADDR_LENGTH = 10;
        private const int DATAMATRIX_WORD_LENGTH = 10;
        private const int TRAYNO_WORD_ADDR_LENGTH = 10;

        /// <summary>
        /// 連動運転中BIT
        /// </summary>
        private const string MACHINE_READY_BIT_ADDR = "EM50070";

        /// <summary>
        /// トラベルシートのバーコードWord(作業許可判定時)
        /// </summary>
        private string[] START_LOTNO_WORD_ADDRNOLIST_START()
        { return new string[] { "EM51400", "EM51410", "EM51420", "EM51430", "EM51440", "EM51450", "EM51460", "EM51470" }; }

        private const string START_TRAY_QRCODE_LEFT_WORD_ADDR = "EM51540";
        private const string START_TRAY_QRCODE_RIGHT_WORD_ADDR = "EM51560";
        
        private const string COMPLETE_LOTNO_WORD_ADDR = "EM62640";
        private const string COMPLETE_STARTDT_WORD_ADDR = "EM62680";
        private const string COMPLETE_ENDDT_WORD_ADDR = "EM62690";

        private const string COMPLETE_TRAY_QRCODE_LEFT_WORD_ADDR = "EM62600";
        private const string COMPLETE_TRAY_QRCODE_RIGHT_WORD_ADDR = "EM62620";

        private const string COMPLETE_TRAY_REMAIN_COUNT_WORD_ADDR = "EM62400";

        private const string COMPLETE_TRAY_COMBINATIONLEFTNO_WORD_ADDR = "EM60000";
        private const string COMPLETE_TRAY_COMBINATIONRIGHTNO_WORD_ADDR = "EM60020";

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
        
        private const string COMPLETE_TRAY_QRCODE_WORD_ADDR_START = "EM60000";

        /// <summary>
        /// 1トレイで使用したシート枚数の取得用アドレス
        /// </summary>
        private const string COMPLETE_TRAY_USEDSHEETCT_WOED_ADDR = "EM62500";

        #endregion

        #region 定数

        private const string SIDE_NAME_LEFT = "左側";
        private const string SIDE_NAME_RIGHT = "右側";

        private enum TrayPosition
        {
            Left, 
            Right,
        }

        #endregion

        #region プロパティ
        
        #endregion
        
        protected override void concreteThreadWork()
        {
            try
            {
                if ((Convert.ToInt32(Plc.GetBit(MACHINE_READY_BIT_ADDR))) == Convert.ToInt32(PLC.Common.BIT_ON))
                {
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

                #region トラベルシート情報(EM51400～EM51479)からロットNoを取得  1ロット = 10アドレス × 最大8ロット

                SortedList<string, int> lotList = new SortedList<string, int>();
                string[] addressList = START_LOTNO_WORD_ADDRNOLIST_START();
                for (int i = 0; i < addressList.Length; i++)
                {
                    string qrcode = Plc.GetString(addressList[i], LOTNO_WORD_ADDR_LENGTH);
                    string lotno = PLC.Common.GetMagazineNo(qrcode);
                    if (string.IsNullOrWhiteSpace(lotno) == true)
                    {
                        continue;
                    }
                    // 読み込んだ8ロット内で重複があれば、対象外とする (1と2段目, 3と4段目が同じロットNoなので重複チェックができない)
                    if (lotList.Any(l => l.Key == lotno) == true)
                    {
                        continue;
                    }
                    this.OutputSysLog($"[開始処理] トラベルシートのロットNoの取得成功 [段数: {i + 1}段目 ロットNo:『{lotno}』]");

                    lotList.Add(lotno, i);
                }

                #endregion

                // 読み込んだロットNo(最大8ロット)を全てチェックする
                foreach (KeyValuePair<string, int> lotnoAndStep in lotList)
                {
                    AsmLot lot = new AsmLot();
                    lot.NascaLotNo = lotnoAndStep.Key;

                    this.OutputSysLog($"[開始処理] チェック開始 [ロットNo:『{lot.NascaLotNo}』]");

                    #region NASCA上の前作業・次作業の実績チェック 

                    List<NascaTranData> tranList = Importer.GetLotTranData(lot.NascaLotNo);

                    bool tranOK = false;
                    string inputworkcd = string.Empty;
                    foreach (string workcd in ArmsApi.Config.Settings.AutoPasterWorkCdList)
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

                    #region 樹脂Gr一致チェック

                    // ①マガジン(トラベルシート)のロット取得
                    lot.TypeCd = Importer.GetTypeFromLotNo(lot.NascaLotNo);
                    if (string.IsNullOrWhiteSpace(lot.TypeCd))
                    {
                        throw new ApplicationException($"NASCA指図から型番の取得に失敗しました。ロットNo：『{lot.TypeCd}』");
                    }
                    this.OutputSysLog($"[開始処理] 型番取得成功 [型番:『{lot.TypeCd}』]");
                    
                    // ②トレイ(左側 + 右側)の情報を取得
                    AsmLot trayLotLeft = getLotInfoFromMachinePlcAndData(0);
                    AsmLot trayLotRight = getLotInfoFromMachinePlcAndData(1);

                    // ③トレイの各ロットとマガジン(トラベルシート)のロットの樹脂グループの不一致チェック
                    string errMsg;
                    if (checkTrayResinGroup(lot, trayLotLeft, out errMsg) == false)
                    {
                        throw new ApplicationException(errMsg);
                    }
                    this.OutputSysLog($"[開始処理] ({SIDE_NAME_LEFT}) 樹脂グループの一致チェック成功 [({lot.NascaLotNo} == {trayLotLeft.NascaLotNo})]");
                    if (checkTrayResinGroup(lot, trayLotRight, out errMsg) == false)
                    {
                        throw new ApplicationException(errMsg);
                    }
                    this.OutputSysLog($"[開始処理] ({SIDE_NAME_RIGHT}) 樹脂グループの一致チェック成功 [({lot.NascaLotNo} == {trayLotRight.NascaLotNo})]");

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

                // トレイ(左側 + 右側)の情報を取得
                AsmLot trayLotLeft = getLotInfoFromMachinePlcAndData(0);
                AsmLot trayLotRight = getLotInfoFromMachinePlcAndData(1);

                // BHTで資材割付しない為、(装置でトレイ割付し、作業完了時に割付たトレイからTnMaterialの資材を使用資材として登録)
                // 作業開始時にTnMaterialに取り込まれているか確認しておく(前工程の保管場所払い出しができているかの確認)
                // また、貼り合わせ2の作業ではトレイにDCロットが紐づく為、対象から除外する
                if (lots.Contains(trayLotLeft.NascaLotNo) == false && Material.IsImported(trayLotLeft.NascaLotNo) == false)
                {
                    throw new ApplicationException($"左側トレイの資材「{trayLotLeft.NascaLotNo}」が取り込まれていません。NASCAで前工程から保管場所払い出しができているか確認して下さい。");
                }
                if (lots.Contains(trayLotRight.NascaLotNo) == false && Material.IsImported(trayLotRight.NascaLotNo) == false)
                {
                    throw new ApplicationException($"右側トレイの資材「{trayLotRight.NascaLotNo}」が取り込まれていません。NASCAで前工程から保管場所払い出しができているか確認して下さい。");
                }

                for (int i = 0; i < lots.Count; i++)
                {
                    Magazine svrMag = Magazine.GetCurrent(lots[i]);
                    AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                    Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);

                    // トレイの各ロットとマガジン(トラベルシート)のロットの樹脂グループの不一致チェック
                    string errMsg;
                    if (checkTrayResinGroup(lot, trayLotLeft, out errMsg) == false)
                    {
                        throw new ApplicationException(errMsg);
                    }
                    this.OutputSysLog($"[開始処理] ({SIDE_NAME_LEFT}) 樹脂グループの一致チェック成功 [({lot.NascaLotNo} == {trayLotLeft.NascaLotNo})]");
                    if (checkTrayResinGroup(lot, trayLotRight, out errMsg) == false)
                    {
                        throw new ApplicationException(errMsg);
                    }
                    this.OutputSysLog($"[開始処理] ({SIDE_NAME_RIGHT}) 樹脂グループの一致チェック成功 [({lot.NascaLotNo} == {trayLotRight.NascaLotNo})]");

                    VirtualMag mag = new VirtualMag();
                    mag.MagazineNo = svrMag.MagazineNo;
                    mag.ProcNo = p.ProcNo;
                    Order startOrder = CommonApi.GetWorkStartOrder(mag, this.MacNo);
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
                #region 装置PLCから開始時間を取得 (過去ログファイル検出用)
                DateTime workStartFromMachinePlc;
                try
                {
                    workStartFromMachinePlc = Plc.GetWordsAsDateTime(COMPLETE_STARTDT_WORD_ADDR);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"開始日時の取得に失敗しました。[アドレス:『{COMPLETE_STARTDT_WORD_ADDR}』]");
                }
                #endregion

                #region リネーム対象のファイルの有無をチェック → なければ、処理しない
                // 未リネームのファイルを取得
                List<string> lotFiles = MachineLog.GetFiles(this.LogOutputDirectoryPath);
                string finFilePath = string.Empty;
                // finファイルを取得
                foreach (string lotFile in lotFiles.OrderByDescending(l => l))
                {
                    string filename = System.IO.Path.GetFileName(lotFile);
                    if (MachineLog.IsLotFromFileName(filename) == true) continue;
                    if (filename.Contains(".fin") == false) continue;

                    DateTime? lastUpdDtFromFileName = MachineLog.GetDateTimeFromFileName(filename);
                    if (lastUpdDtFromFileName < workStartFromMachinePlc)
                    {
                        throw new ApplicationException($"装置メモリの開始日時より更新日が古いログファイルがあります。ファイルを削除して下さい。" +
                            $"[ファイル置場：『{this.LogOutputDirectoryPath}』ファイル名:『{System.IO.Path.GetFileName(lotFile)}』]");
                    }

                    finFilePath = lotFile;
                    break;
                }
                // 未リネームのfinファイルがなければ、処理しない。
                if (string.IsNullOrWhiteSpace(finFilePath) == true)
                {
                    return;
                }
                #endregion

                this.OutputSysLog($"[完了処理] 開始 [対象finファイル：『{finFilePath}』]");

                #region PLCからデータ収集

                // トラベルシートのロット情報を取得
                string qrcode = Plc.GetString(COMPLETE_LOTNO_WORD_ADDR, LOTNO_WORD_ADDR_LENGTH);
                string lotno = PLC.Common.GetMagazineNo(qrcode);
                if (lotno == null)
                {
                    throw new ApplicationException($"ロットNoの取得に失敗しました。[アドレス:『{COMPLETE_LOTNO_WORD_ADDR}』]");
                }
                this.OutputSysLog($"[完了処理] ロットNoの取得成功 [ロットNo:『{lotno}』]");
                
                // NASCA作業実績から現在の作業CDを取得 (対象はArmsConfigの項目『AutoPasterWorkCdList』に指定している作業CD)
                string workcd = null;
                List<NascaTranData> tranList = Importer.GetLotTranData(lotno);
                foreach (NascaTranData tran in tranList)
                {
                    if (Config.Settings.AutoPasterWorkCdList.Contains(tran.WorkCd) == false)
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
                    throw new ApplicationException($"NASCA実績から自工程の作業CDの取得に失敗しました。[ロットNo:『{lotno}』]");
                }
                Process p = Process.GetProcess(workcd);
                if (p == null)
                {
                    throw new ApplicationException($"自工程の作業CDがTmProcessに登録されていません。[作業CD：『{workcd}』]");
                }

                // NASCA指図から型番を取得
                string magTypeCd = Importer.GetTypeFromLotNo(lotno);
                if (string.IsNullOrWhiteSpace(magTypeCd))
                {
                    throw new ApplicationException($"NASCA指図から型番の取得に失敗しました。[ロットNo:『{lotno}』]");
                }
                this.OutputSysLog($"[完了処理] 型番の取得成功 [型番:『{magTypeCd}』]");

                // 開始時刻の取得  (仮想マガジン『Loader』のレコードがあれば、その開始時刻とする。なければ、装置PLCから取得 + 仮想マガジン『Loader』を登録)
                DateTime WorkStart;
                VirtualMag[] lMags = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Loader).ToString(), lotno, p.ProcNo);
                if (lMags.Count() == 0)
                {
                    // 仮想マガジンのデータが無い → 初回トレイ：装置PLCから時間を取得
                    WorkStart = workStartFromMachinePlc;

                    // 仮想マガジンの新規登録
                    VirtualMag lMag = new VirtualMag();
                    lMag.MacNo = this.MacNo;
                    lMag.MagazineNo = lotno;
                    lMag.ProcNo = p.ProcNo;
                    lMag.WorkStart = WorkStart;
                    this.Enqueue(lMag, Station.Loader);
                }
                else
                {
                    // 仮想マガジンのデータがある → 2個目以降のトレイ：仮想マガジンの開始時刻を参照
                    WorkStart = lMags.OrderBy(m => m.orderid).First().WorkStart.Value;
                }
                this.OutputSysLog($"[完了処理] 開始日時取得成功 [日時:『{WorkStart}』]");

                DateTime WorkComplete;
                try
                {
                    WorkComplete = Plc.GetWordsAsDateTime(COMPLETE_ENDDT_WORD_ADDR);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"完了日時の取得に失敗しました。[アドレス:『{COMPLETE_ENDDT_WORD_ADDR}』]");
                }
                this.OutputSysLog($"[完了処理] 完了日時取得成功 [日時:『{WorkComplete}』]");

                // トラベルシートに対する残りトレイ数を取得
                int trayRemainCt;
                try
                {
                    trayRemainCt = Plc.GetWordAsDecimalData(COMPLETE_TRAY_REMAIN_COUNT_WORD_ADDR);
                }
                catch (Exception)
                {
                    throw new ApplicationException($"残りトレイ数の取得に失敗しました。[アドレス:『{COMPLETE_TRAY_REMAIN_COUNT_WORD_ADDR}』]");
                }
                this.OutputSysLog($"[完了処理] 残りトレイ数取得成功 [残りトレイ数:『{trayRemainCt}』]");
                
                // 収納基板(左側)情報取得
                string trayQRLeft = Plc.GetString(COMPLETE_TRAY_QRCODE_LEFT_WORD_ADDR, DATAMATRIX_WORD_LENGTH, true);
                if (string.IsNullOrWhiteSpace(trayQRLeft) == true)
                {
                    throw new ApplicationException($"装置から収納基板情報が取得できません。[取得アドレス(開始)：『{COMPLETE_TRAY_QRCODE_LEFT_WORD_ADDR}』]");
                }
                trayQRLeft = trayQRLeft.Replace("\r", "").Replace("\0", "");
                string[] trayLotNoArray = LotCarrier.GetLotNo(trayQRLeft, true);
                if (trayLotNoArray.Length == 0)
                {
                    throw new ApplicationException($"トレイQRに紐づいているロットが存在しません。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                        $"[トレイQR(キャリアNo)：『{trayQRLeft}』]");
                }
                else if (trayLotNoArray.Length > 1)
                {
                    throw new ApplicationException($"トレイQRに紐づいているロットが複数存在します。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                        $"[トレイQR(キャリアNo)：『{trayQRLeft}』]");
                }
                string trayLeftLotNo = trayLotNoArray[0];
                string trayLeftMaterialCd = Importer.GetMaterialCdFromLotNo(trayLeftLotNo);
                if (string.IsNullOrWhiteSpace(trayLeftMaterialCd))
                {
                    throw new ApplicationException($"NASCA指図から型番の取得に失敗しました。[トレイQR：『{trayQRLeft}』]");
                }

                // 収納基板(右側)情報取得
                string trayQRRight = Plc.GetString(COMPLETE_TRAY_QRCODE_RIGHT_WORD_ADDR, DATAMATRIX_WORD_LENGTH, true);
                if (string.IsNullOrWhiteSpace(trayQRRight) == true)
                {
                    throw new ApplicationException($"装置から収納基板情報が取得できません。[取得アドレス(開始)：『{COMPLETE_TRAY_QRCODE_RIGHT_WORD_ADDR}』]");
                }
                trayQRRight = trayQRRight.Replace("\r", "").Replace("\0", "");
                trayLotNoArray = LotCarrier.GetLotNo(trayQRRight, true);
                if (trayLotNoArray.Length == 0)
                {
                    throw new ApplicationException($"トレイQRに紐づいているロットが存在しません。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                        $"[トレイQR(キャリアNo)：『{trayQRRight}』]");
                }
                else if (trayLotNoArray.Length > 1)
                {
                    throw new ApplicationException($"トレイQRに紐づいているロットが複数存在します。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                        $"[トレイQR(キャリアNo)：『{trayQRRight}』]");
                }
                string trayRightLotNo = trayLotNoArray[0];
                string trayRightMatreialCd = Importer.GetMaterialCdFromLotNo(trayRightLotNo);
                if (string.IsNullOrWhiteSpace(trayRightMatreialCd))
                {
                    throw new ApplicationException($"NASCA指図から型番の取得に失敗しました。[トレイQR：『{trayQRRight}』]");
                }

                // トレイ(左側+右側)の資材リストを作成 + TnMatRelationに登録
                Order o = new Order();
                o.LotNo = lotno;
                o.ProcNo = p.ProcNo;

                List<Material> matList = new List<Material>();
                // TnMatreialsを参照しない方法でTnMatRelationの情報を取得する
                MatRelation[] matRelateArray = Order.GetRelatedMaterials(lotno);
                foreach(MatRelation matRelate in matRelateArray.Where(m => m.ProcNo == p.ProcNo))
                {
                    Material mat = new Material();
                    mat.LotNo = matRelate.MatLotNo;
                    mat.MaterialCd = matRelate.MaterialCd;
                    mat.InputCt = matRelate.inputCt;
                    matList.Add(mat);
                    OutputSysLog($"[完了処理] TnMatRelationから一時保存資材を取得 LotNo:{mat.LotNo} MaterialCd:{mat.MaterialCd}");
                }

                // トラベルシートのロットNoと同じロットは資材扱いにしない (2回目の貼り付け作業の時に再現される)
                if (lotno != trayLeftLotNo)
                {
                    int leftmatid = matList.FindIndex(m => m.LotNo == trayLeftLotNo && m.MaterialCd == trayLeftMaterialCd);
                    if (leftmatid == -1)
                    {
                        // リストに無い → リストに新規追加
                        Material mat = new Material();
                        mat.LotNo = trayLeftLotNo;
                        mat.MaterialCd = trayLeftMaterialCd;
                        mat.InputCt = getTrayUsedSheetCount();
                        OutputSysLog($"[完了処理] 左トレイ:{trayQRLeft} ロット:{trayLeftLotNo}を使用資材に追加");
                        matList.Add(mat);
                    }
                    else
                    {
                        // リストにある → 投入数を1増やす
                        matList[leftmatid].InputCt++;
                    }
                }

                // トラベルシートのロットNoと同じロットは資材扱いにしない (2回目の貼り付け作業の時に再現される)
                if (lotno != trayRightLotNo)
                {
                    int rightmatid = matList.FindIndex(m => m.LotNo == trayRightLotNo && m.MaterialCd == trayRightMatreialCd);
                    if (rightmatid == -1)
                    {
                        // リストに無い → リストに新規追加
                        Material mat = new Material();
                        mat.LotNo = trayRightLotNo;
                        mat.MaterialCd = trayRightMatreialCd;
                        mat.InputCt = getTrayUsedSheetCount();
                        OutputSysLog($"[完了処理] 右トレイ:{trayQRRight} ロット:{trayRightLotNo}を使用資材に追加");
                        matList.Add(mat);
                    }
                    else
                    {
                        // リストにある → 投入数を1増やす
                        matList[rightmatid].InputCt++;
                    }
                }

                #endregion

                // トラベルシートに対する残りトレイ数が0枚の時のみ、実績登録をする
                NASCAResponse res = new NASCAResponse();
                if (trayRemainCt == 0)
                { 
                    #region NASCA実績登録

                    res = NASCAResponse.GetNGResponse("アッセンロット連携異常");
                    string mnfctString;
                    NppNelMnfctParamInfo mnfctInfo = NascaPubApi.GetMnfctParamInfo(lotno,
                        "", //マガジン連番0なら空白
                        this.PlantCd, WorkStart, WorkComplete, null, null, matList.ToArray(), null, null, null, null, null, 0, out mnfctString);

                    this.OutputSysLog($"[完了処理] NASCA SEND:『{mnfctString}』");
                    NascaPubApi api = NascaPubApi.GetInstance();
                    res = api.WriteMnfctResult(mnfctInfo, true);
                    this.OutputSysLog($"[完了処理] NASCA RECEIVE:『{res.OriginalMessage}』");
                    //if (res.Status != NASCAStatus.OK)
                    //{
                    //    throw new ApplicationException($"NASCA実績登録に失敗しました。理由:『{res.OriginalMessage}』");
                    //}
                    #endregion

                    // 仮想マガジンの削除
                    VirtualMag[] deleteMags = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Loader).ToString(), lotno, p.ProcNo);
                    foreach(VirtualMag mag in deleteMags)
                    {
                        mag.Delete();
                    }
                }

                #region トレイ情報をTnMatRelationに登録

                // TnMatRelation登録
                o.UpdateMaterialRelation(matList.ToArray());

                #endregion

                #region ファイルのリネーム
                string dataMatrix = System.IO.Path.GetFileNameWithoutExtension(finFilePath);
                lotFiles = MachineLog.GetFiles(this.LogOutputDirectoryPath);
                foreach (string lotFile in lotFiles)
                {
                    string filename = System.IO.Path.GetFileName(lotFile);

                    // この関数の初めで取得したfinファイルと同じ時刻ファイル (未リネーム)のみが対象
                    if (filename.StartsWith(dataMatrix) == false) continue;
                    if (MachineLog.IsLotFromFileName(filename) == true) continue;
                    
                    MachineLog.ChangeFileName(lotFile, lotno, magTypeCd, p.ProcNo, lotno);
                    this.OutputSysLog($"[完了処理] ロットファイル名称変更 [ファイル名:『{lotFile}』]");
                }
                #endregion

                #region TnLotCarrierの紐付け紐付け解除
                
                //TnLotCarrier登録&前データがある場合フラグOFF
                LotCarrier oldLotCarrier = LotCarrier.GetData(trayQRLeft, true, false);
                if (oldLotCarrier != null && oldLotCarrier.LotNo != lotno)
                {
                    LotCarrier.UpdateOperateFg(oldLotCarrier.LotNo, oldLotCarrier.CarrierNo, false);
                    this.OutputSysLog($"[完了処理] ロット-トレイ紐付け解除 [ロットNo:『{oldLotCarrier.LotNo}』, トレイ(左側)：[{oldLotCarrier.CarrierNo}]");
                }
                oldLotCarrier = LotCarrier.GetData(trayQRRight, true, false);
                if (oldLotCarrier != null && oldLotCarrier.LotNo != lotno)
                {
                    LotCarrier.UpdateOperateFg(oldLotCarrier.LotNo, oldLotCarrier.CarrierNo, false);
                    this.OutputSysLog($"[完了処理] ロット-トレイ紐付け解除 [ロットNo:『{oldLotCarrier.LotNo}』, トレイ(右側)：[{oldLotCarrier.CarrierNo}]");
                }

                LotCarrier svrData = LotCarrier.GetData(trayQRLeft, lotno, false);
                if (svrData == null)
                {
                    LotCarrier lotcarrier = new LotCarrier(lotno, trayQRLeft, "660");
                    lotcarrier.Insert();
                }
                else
                {
                    LotCarrier.UpdateOperateFg(lotno, trayQRLeft, true);
                }
                this.OutputSysLog($"[完了処理] ロット-トレイ紐付け完了 [ロットNo:『{lotno}』, トレイ(左側)：[{trayQRLeft}]");

                #endregion

                // KFS+クリアの貼り合わせ後はKFSの測定結果を取得するにクリアのトレイNoから取得しないといけないので
                // クリアのトレイNoをKFSのトレイNoに更新しておく
                updateTray();

                // 完了処理が終わったので排出要求をOFFへ
                Plc.SetBit(COMPLETE_REQ_BIT_ADDR, 1, Common.BIT_OFF);
                Log.SysLog.Info("[完了処理] 排出信号OFFへ");

                #region NASCAの実績登録を失敗した時に手動登録のメッセージを表示する。
                if (trayRemainCt == 0 && res != null && res.Status != NASCAStatus.OK)
                {
                    string errMsg = $"【重要！】NASCA実績登録に失敗しました。理由:『{res.OriginalMessage}』\r\n " +
                        $"■作業指示：NASCAの実績登録画面で手動で実績登録して下さい。\r\n" +
                        $"  ロットNo：『{lotno}]』\r\n" +
                        $"  作業CD：『{workcd}』\r\n" +
                        $"  設備CD：『{this.PlantCd}』\r\n" +
                        $"  作業開始時刻：『{WorkStart}』\r\n" +
                        $"  作業完了時刻：『{WorkComplete}』\r\n";
                    int i = 1;
                    foreach(Material mat in matList)
                    {
                        errMsg += $"  資材ロット({i})[ロットNo：{mat.LotNo}, 型番：{mat.MaterialCd}, 投入数：{(int)mat.InputCt}]\r\n";
                        i++;
                    }
                    throw new ApplicationException(errMsg);
                }
                #endregion

                this.OutputSysLog($"[完了処理] 作業登録完了 [ロットNo:『{lotno}』]");
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
                DateTime workStartDt = getWorkStartDate();
                
                #region リネーム対象のファイルの有無をチェック → なければ、処理しない
                // 未リネームのファイルを取得
                List<string> lotFiles = MachineLog.GetFiles(this.LogOutputDirectoryPath);
                string finFilePath = string.Empty;
                // finファイルを取得
                foreach (string lotFile in lotFiles.OrderByDescending(l => l))
                {
                    string filename = System.IO.Path.GetFileName(lotFile);
                    if (MachineLog.IsLotFromFileName(filename) == true) continue;
                    if (filename.Contains(".fin") == false) continue;

                    DateTime? lastUpdDtFromFileName = MachineLog.GetDateTimeFromFileName(filename);
                    if (lastUpdDtFromFileName < workStartDt)
                    {
                        throw new ApplicationException($"装置メモリの開始日時より更新日が古いログファイルがあります。ファイルを削除して下さい。" +
                            $"[ファイル置場：『{this.LogOutputDirectoryPath}』ファイル名:『{System.IO.Path.GetFileName(lotFile)}』]");
                    }

                    finFilePath = lotFile;
                    break;
                }
                // 未リネームのfinファイルがなければ、処理しない。
                if (string.IsNullOrWhiteSpace(finFilePath) == true)
                {
                    return;
                }
                #endregion

                this.OutputSysLog($"[完了処理] 開始 [対象finファイル：『{finFilePath}』]");

                string completeLotNo = getCompleteLotNo();
                this.OutputSysLog($"[完了処理] ロットNoの取得成功 [ロットNo:『{completeLotNo}』]");

                Magazine svrMag = Magazine.GetCurrent(completeLotNo);
                AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                Process p = Process.GetNextProcess(svrMag.NowCompProcess, lot);
                
                VirtualMag[] lMags = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Loader).ToString(), completeLotNo, p.ProcNo);
                if (lMags.Count() == 0)
                {
                    // 仮想マガジンのデータが無い → 初回トレイ：装置PLCから時間を取得

                    // 仮想マガジンの新規登録
                    VirtualMag lMag = new VirtualMag();
                    lMag.MacNo = this.MacNo;
                    lMag.MagazineNo = completeLotNo;
                    lMag.ProcNo = p.ProcNo;
                    lMag.WorkStart = workStartDt;
                    this.Enqueue(lMag, Station.Loader);
                }
                else
                {
                    // 仮想マガジンのデータがある → 2個目以降のトレイ：仮想マガジンの開始時刻を参照
                    workStartDt = lMags.OrderBy(m => m.orderid).First().WorkStart.Value;
                }
                this.OutputSysLog($"[完了処理] 開始日時取得成功 [日時:『{workStartDt}』]");

                DateTime workCompleteDt = getWorkCompleteDate();

                int trayRemainCt = getRemainingTrayCount();
                this.OutputSysLog($"[完了処理] 残りトレイ数取得成功 [残りトレイ数:『{trayRemainCt}』]");

                string leftTrayNo = getTrayNo(TrayPosition.Left);
                string[] trayLotNoArray = LotCarrier.GetLotNo(leftTrayNo, true);
                if (trayLotNoArray.Length == 0)
                {
                    throw new ApplicationException($"トレイQRに紐づいているロットが存在しません。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                        $"[トレイQR(キャリアNo)：『{leftTrayNo}』]");
                }
                else if (trayLotNoArray.Length > 1)
                {
                    throw new ApplicationException($"トレイQRに紐づいているロットが複数存在します。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                        $"[トレイQR(キャリアNo)：『{leftTrayNo}』]");
                }
                string trayLeftLotNo = trayLotNoArray[0];
                string trayLeftMaterialCd = Material.GetMaterials(trayLeftLotNo, false).SingleOrDefault()?.MaterialCd;

                string rightTrayNo = getTrayNo(TrayPosition.Right);
                trayLotNoArray = LotCarrier.GetLotNo(rightTrayNo, true);
                if (trayLotNoArray.Length == 0)
                {
                    throw new ApplicationException($"トレイQRに紐づいているロットが存在しません。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                        $"[トレイQR(キャリアNo)：『{rightTrayNo}』]");
                }
                else if (trayLotNoArray.Length > 1)
                {
                    throw new ApplicationException($"トレイQRに紐づいているロットが複数存在します。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                        $"[トレイQR(キャリアNo)：『{rightTrayNo}』]");
                }
                string trayRightLotNo = trayLotNoArray[0];
                string trayRightMaterialCd = Material.GetMaterials(trayRightLotNo, false).SingleOrDefault()?.MaterialCd;

                // トレイ(左側+右側)の資材リストを作成 + TnMatRelationに登録
                Order o = new Order();
                o.LotNo = lot.NascaLotNo;
                o.ProcNo = p.ProcNo;

                List<Material> matList = new List<Material>();
                // TnMatreialsを参照しない方法でTnMatRelationの情報を取得する
                MatRelation[] matRelateArray = Order.GetRelatedMaterials(lot.NascaLotNo);
                foreach (MatRelation matRelate in matRelateArray.Where(m => m.ProcNo == p.ProcNo))
                {
                    Material mat = new Material();
                    mat.LotNo = matRelate.MatLotNo;
                    mat.MaterialCd = matRelate.MaterialCd;
                    mat.InputCt = matRelate.inputCt;
                    matList.Add(mat);
                }

                // トラベルシートのロットNoと同じロットは資材扱いにしない (2回目の貼り付け作業の時に再現される)
                if (lot.NascaLotNo != trayLeftLotNo)
                {
                    int leftmatid = matList.FindIndex(m => m.LotNo == trayLeftLotNo && m.MaterialCd == trayLeftMaterialCd);
                    if (leftmatid == -1)
                    {
                        // リストに無い → リストに新規追加
                        Material mat = new Material();
                        mat.LotNo = trayLeftLotNo;
                        mat.MaterialCd = trayLeftMaterialCd;
                        mat.InputCt = getTrayUsedSheetCount();
                        matList.Add(mat);
                    }
                    else
                    {
                        // リストにある → 投入数を1増やす
                        matList[leftmatid].InputCt++;
                    }
                }

                // トラベルシートのロットNoと同じロットは資材扱いにしない (2回目の貼り付け作業の時に再現される)
                if (lot.NascaLotNo != trayRightLotNo)
                {
                    int rightmatid = matList.FindIndex(m => m.LotNo == trayRightLotNo && m.MaterialCd == trayRightMaterialCd);
                    if (rightmatid == -1)
                    {
                        // リストに無い → リストに新規追加
                        Material mat = new Material();
                        mat.LotNo = trayRightLotNo;
                        mat.MaterialCd = trayRightMaterialCd;
                        mat.InputCt = getTrayUsedSheetCount();
                        matList.Add(mat);
                    }
                    else
                    {
                        // リストにある → 投入数を1増やす
                        matList[rightmatid].InputCt++;
                    }
                }

                // 残りトレイ数が0枚の時のみ、実績登録をする
                ArmsApiResponse res = null;
                if (trayRemainCt == 0)
                {
                    VirtualMag mag = new VirtualMag();
                    mag.MagazineNo = completeLotNo;
                    mag.LastMagazineNo = completeLotNo;
                    mag.ProcNo = p.ProcNo;
                    mag.WorkStart = workStartDt;
                    mag.WorkComplete = workCompleteDt;

                    Order endOrder = CommonApi.GetWorkEndOrder(mag, this.MacNo, this.LineNo);
                    res = CommonApi.WorkEnd(endOrder);

                    // 仮想マガジンの削除
                    VirtualMag[] deleteMags = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Loader).ToString(), completeLotNo, p.ProcNo);
                    foreach (VirtualMag delMag in deleteMags)
                    {
                        delMag.Delete();
                    }
                }

                // TnMatRelation登録
                o.UpdateMaterialRelation(matList.ToArray());

                #region ファイルのリネーム
                string dataMatrix = System.IO.Path.GetFileNameWithoutExtension(finFilePath);
                lotFiles = MachineLog.GetFiles(this.LogOutputDirectoryPath);
                foreach (string lotFile in lotFiles)
                {
                    string filename = System.IO.Path.GetFileName(lotFile);

                    // この関数の初めで取得したfinファイルと同じ時刻ファイル (未リネーム)のみが対象
                    if (filename.StartsWith(dataMatrix) == false) continue;
                    if (MachineLog.IsLotFromFileName(filename) == true) continue;

                    MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, p.ProcNo, lot.NascaLotNo);
                    this.OutputSysLog($"[完了処理] ロットファイル名称変更 [ファイル名:『{lotFile}』]");
                }
                #endregion

                #region TnLotCarrierの紐付け紐付け解除

                //TnLotCarrier登録&前データがある場合フラグOFF
                LotCarrier oldLotCarrier = LotCarrier.GetData(leftTrayNo, true, false);
                if (oldLotCarrier != null && oldLotCarrier.LotNo != completeLotNo)
                {
                    LotCarrier.UpdateOperateFg(oldLotCarrier.LotNo, oldLotCarrier.CarrierNo, false);
                    this.OutputSysLog($"[完了処理] ロット-トレイ紐付け解除 [ロットNo:『{oldLotCarrier.LotNo}』, トレイ(左側)：[{oldLotCarrier.CarrierNo}]");
                }
                oldLotCarrier = LotCarrier.GetData(rightTrayNo, true, false);
                if (oldLotCarrier != null && oldLotCarrier.LotNo != completeLotNo)
                {
                    LotCarrier.UpdateOperateFg(oldLotCarrier.LotNo, oldLotCarrier.CarrierNo, false);
                    this.OutputSysLog($"[完了処理] ロット-トレイ紐付け解除 [ロットNo:『{oldLotCarrier.LotNo}』, トレイ(右側)：[{oldLotCarrier.CarrierNo}]");
                }

                LotCarrier svrData = LotCarrier.GetData(leftTrayNo, completeLotNo, false);
                if (svrData == null)
                {
                    LotCarrier lotcarrier = new LotCarrier(completeLotNo, leftTrayNo, "660");
                    lotcarrier.Insert();
                }
                else
                {
                    LotCarrier.UpdateOperateFg(completeLotNo, leftTrayNo, true);
                }
                this.OutputSysLog($"[完了処理] ロット-トレイ紐付け完了 [ロットNo:『{completeLotNo}』, トレイ(左側)：[{leftTrayNo}]");

                #endregion

                // KFS+クリアの貼り合わせ後はKFSの測定結果を取得するにクリアのトレイNoから取得しないといけないので
                // クリアのトレイNoをKFSのトレイNoに更新しておく
                updateTray();

                // 完了処理が終わったので排出要求をOFFへ
                Plc.SetBit(COMPLETE_REQ_BIT_ADDR, 1, Common.BIT_OFF);
                Log.SysLog.Info("[完了処理] 排出信号OFFへ");

                #region 実績登録を失敗した時に手動登録のメッセージを表示する。
                if (res != null && res.IsError)
                {
                    string errMsg = $"【重要！】実績登録に失敗しました。理由:『{res.Message}』\r\n " +
                        $"■作業指示：NASCAの実績登録画面で手動で実績登録して下さい。\r\n" +
                        $"  ロットNo：『{lot.NascaLotNo}]』\r\n" +
                        $"  工程：『{p.InlineProNM}』\r\n" +
                        $"  設備CD：『{this.PlantCd}』\r\n" +
                        $"  作業開始時刻：『{workStartDt}』\r\n" +
                        $"  作業完了時刻：『{workCompleteDt}』\r\n";
                    throw new ApplicationException(errMsg);
                }
                #endregion
                
                this.OutputSysLog($"[完了処理] 作業登録完了 [ロットNo:『{completeLotNo}』]");
            }
            catch (Exception ex)
            {
                throw new Exception($"{this.Name} [完了処理異常] 理由：{ex.Message}");
            }
        }

        #region [開始処理] 装置PLC + NASCAからトレイの紐付けロット情報 (ロットNo, タイプ, 樹脂Gr)を取得

        private AsmLot getLotInfoFromMachinePlcAndData(int sideIndex)
        {
            string sidename;
            string addressTrayQR;
            if (sideIndex == 0)
            {
                sidename = SIDE_NAME_LEFT;
                addressTrayQR = START_TRAY_QRCODE_LEFT_WORD_ADDR;
            }
            else
            {
                sidename = SIDE_NAME_RIGHT;
                addressTrayQR = START_TRAY_QRCODE_RIGHT_WORD_ADDR;
            }
            
            // トレイQRのデータ取得
            string trayQR = Plc.GetString(addressTrayQR, DATAMATRIX_WORD_LENGTH, true);
            if (string.IsNullOrWhiteSpace(trayQR) == true)
            {
                throw new ApplicationException($"装置からトレイQRコード({sidename})が取得できません。取得アドレス：『{addressTrayQR}』");
            }
            trayQR = trayQR.Replace("\r", "").Replace("\0", "");
            this.OutputSysLog($"[開始処理] トレイQRコード({sidename})の取得成功 [QRコード:『{trayQR}』]");

            string[] lotNoArray = LotCarrier.GetLotNo(trayQR, true);
            if (lotNoArray.Length == 0)
            {
                throw new ApplicationException($"トレイQRコード({sidename})をキャリアNo扱いとして紐づいているロットが存在しません。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                    $"[トレイQRコード({sidename})：『{trayQR}』]");
            }
            else if (lotNoArray.Length > 1)
            {
                throw new ApplicationException($"トレイQRコード({sidename})をキャリアNo扱いとして紐づいているロットが複数存在します。ARMSのキャリア紐付け情報(TnLotCarrier)を確認して下さい。" +
                    $"[トレイQRコード({sidename})：『{trayQR}』]");
            }

            string lotno = lotNoArray.Single();
            this.OutputSysLog($"[開始処理] トレイQRコード({sidename})の紐付けロット取得成功 [ロットNo({sidename})：{lotno}』]");

            //string typecd = Importer.GetTypeFromLotNo(lotno);
            //if (string.IsNullOrWhiteSpace(typecd))
            //{
            //    throw new ApplicationException($"NASCA指図から型番の取得に失敗しました。[ロットNo({sidename})：『{lotno}』]");
            //}

            //this.OutputSysLog($"[開始処理] トレイQRコード({sidename})の型番取得成功 [型番：{typecd}』]");
            AsmLot asmlot = new AsmLot();
            if (this.IsOutLine)
            {
                asmlot.NascaLotNo = lotno;
                asmlot.TypeCd = getLotTypeCd(lotno);
            }
            else
            {
                asmlot = AsmLot.GetAsmLot(lotno);
            }
            return asmlot;
        }

        #endregion

        #region ロットとトレイQRの樹脂Grチェック  (トレイQRの型番を出力)
        private bool checkTrayResinGroup(AsmLot lot, AsmLot trayLot, out string errMsg)
        {
            errMsg = "";
            string[] lotResinGroup;
            string[] trayResinGroup;
            if (this.IsOutLine)
            {
                // 樹脂グループの取得：関数GetLotCharValは必ずLotCharクラスを返すから、直接『LotCharVal』プロパティを参照できる
                string lotCharVal = Importer.GetLotCharVal(lot.NascaLotNo, lot.TypeCd, Config.RESINGROUP_LOTCHARCD).LotCharVal;
                if (string.IsNullOrWhiteSpace(lotCharVal))
                {
                    throw new ApplicationException($"NASCA指図からロット特性『樹脂グループ』の取得に失敗しました。ロットNo：『{lot.NascaLotNo}』, 型番：『{lot.TypeCd}』, ロット特性Cd：『{Config.RESINGROUP_LOTCHARCD}』");
                }
                lotResinGroup = lotCharVal.Split(',');
                this.OutputSysLog($"[開始処理] 樹脂Gr取得成功 [樹脂Gr:『{lotCharVal}』]");

                lotCharVal = Importer.GetLotCharVal(trayLot.NascaLotNo, trayLot.TypeCd, Config.RESINGROUP_LOTCHARCD).LotCharVal;
                trayResinGroup = lotCharVal.Split(',');
            }
            else
            {
                lotResinGroup = lot.ResinGpCd.ToArray();
                trayResinGroup = trayLot.ResinGpCd.ToArray();
            }

            // 樹脂グループチェック → 親が持っていない樹脂Grを、子の樹脂Grに含まれていたらNG
            string[] magLotResinGroupArray = lotResinGroup;
            string[] trayResinGroupArray = trayResinGroup;
            List<string> errResinGroup = new List<string>();
            foreach (string resingroup in trayResinGroupArray)
            {
                if (lotResinGroup.Contains(resingroup) == false)
                {
                    errResinGroup.Add(resingroup);
                }
            }
            if (errResinGroup.Count() > 0)
            {
                errMsg = $"トラベルシート側が持っていない樹脂GrをトレイQR側の樹脂Grが持っています。[ロットNo(トラベルシート)：『{lot.NascaLotNo}』[ロットNo(トレイQR)：『{trayLot.NascaLotNo}』,樹脂グループ(トレイQR)：『{string.Join(",", errResinGroup.Distinct())}』]";
                return false;
            }

            return true;
        }

        #endregion

        private string getCombinationLeftTrayNo()
        {
            return this.Plc.GetString(COMPLETE_TRAY_COMBINATIONLEFTNO_WORD_ADDR, TRAYNO_WORD_ADDR_LENGTH, true).Replace("\r", "").Replace("\0", "");
        }
        private string getCombinationRightTrayNo()
        {
            return this.Plc.GetString(COMPLETE_TRAY_COMBINATIONRIGHTNO_WORD_ADDR, TRAYNO_WORD_ADDR_LENGTH, true).Replace("\r", "").Replace("\0", "");
        }

        /// <summary>
        /// 組み合わせ元のトレイNoを組み合わせ後のトレイNoに更新
        /// </summary>
        private void updateTray()
        {
            //組み合わせ元トレイNo
            string rightTrayNo = getCombinationRightTrayNo();

            //組み合わせ後トレイNo
            string leftTrayNo = getCombinationLeftTrayNo();

            OutputSysLog($"[完了処理] 色調測定結果のトレイNo更新開始 前No:{rightTrayNo} ⇒ 後No:{leftTrayNo}");

            using (var db = new ArmsApi.Model.DataContext.EICSDataContext(Config.Settings.QCILConSTR))
            {
                var lTrayData = db.TnPsMeasureResult.Where(p => p.Tray_NO == leftTrayNo && p.New_FG == true);
                foreach (var d in lTrayData)
                {
                    d.New_FG = false;
                }
                var rTrayData = db.TnPsMeasureResult.Where(p => p.Tray_NO == rightTrayNo && p.New_FG == true);
                foreach (var d in rTrayData)
                {
                    d.New_FG = false;

                    ArmsApi.Model.DataContext.TnPsMeasureResult p = new ArmsApi.Model.DataContext.TnPsMeasureResult();
                    p.Tray_NO = leftTrayNo;
                    p.Sheet_NO = d.Sheet_NO;
                    p.QcParam_NO = d.QcParam_NO;
                    p.Measure_DT = d.Measure_DT;
                    p.Plant_CD = d.Plant_CD;
                    p.Lot_NO = d.Lot_NO;
                    p.Type_CD = d.Type_CD;
                    p.New_FG = true;
                    p.MeasureAve_VAL = d.MeasureAve_VAL;
                    p.LastUpd_DT = System.DateTime.Now;

                    OutputSysLog($"[完了処理] 色調測定結果の追加情報　トレイNo:{p.Tray_NO} シートNo:{p.Sheet_NO} 管理番号:{p.QcParam_NO} 測定日時:{p.Measure_DT} 設備番号:{p.Plant_CD}");

                    db.TnPsMeasureResult.InsertOnSubmit(p);
                }

                db.SubmitChanges();
                OutputSysLog($"[完了処理] 色調測定結果のトレイNo更新完了 前No:{rightTrayNo} ⇒ 後No:{leftTrayNo}");
            }
        }

        private List<string> getTravelSheetLots()
        {
            SortedList<string, int> lotList = new SortedList<string, int>();

            string[] addressList = START_LOTNO_WORD_ADDRNOLIST_START();
            for (int i = 0; i < addressList.Length; i++)
            {
                string qrcode = Plc.GetString(addressList[i], LOTNO_WORD_ADDR_LENGTH);
                string lotno = PLC.Common.GetMagazineNo(qrcode);
                if (string.IsNullOrWhiteSpace(lotno) == true)
                {
                    continue;
                }

                // 読み込んだ8ロット内で重複があれば、対象外とする (1と2段目, 3と4段目が同じロットNoなので重複チェックができない)
                if (lotList.Any(l => l.Key == lotno) == true)
                {
                    continue;
                }
                this.OutputSysLog($"[開始処理] トラベルシートのロットNoの取得成功 [段数: {i + 1}段目 ロットNo:『{lotno}』]");

                lotList.Add(lotno, i);
            }

            return lotList.Keys.ToList();
        }

        /// <summary>
        /// 装置PLCから作業開始日時を取得
        /// </summary>
        /// <returns></returns>
        private DateTime getWorkStartDate()
        {
            try
            {
                return Plc.GetWordsAsDateTime(COMPLETE_STARTDT_WORD_ADDR);
            }
            catch (Exception)
            {
                throw new ApplicationException($"作業開始日時の取得に失敗しました。[アドレス:『{COMPLETE_STARTDT_WORD_ADDR}』]");
            }
        }

        /// <summary>
        /// 装置PLCから作業完了日時を取得
        /// </summary>
        /// <returns></returns>
        private DateTime getWorkCompleteDate()
        {
            try
            {
                return Plc.GetWordsAsDateTime(COMPLETE_ENDDT_WORD_ADDR);
            }
            catch (Exception)
            {
                throw new ApplicationException($"作業完了日時の取得に失敗しました。[アドレス:『{COMPLETE_ENDDT_WORD_ADDR}』]");
            }
        }

        /// <summary>
        /// 装置PLCから完了ロットNoを取得
        /// </summary>
        /// <returns></returns>
        private string getCompleteLotNo()
        {
            string qrcode = Plc.GetString(COMPLETE_LOTNO_WORD_ADDR, LOTNO_WORD_ADDR_LENGTH);
            string lotno = PLC.Common.GetMagazineNo(qrcode);
            if (lotno == null)
            {
                throw new ApplicationException($"ロットNoの取得に失敗しました。[アドレス:『{COMPLETE_LOTNO_WORD_ADDR}』]");
            }

            return lotno;
        }

        /// <summary>
        /// トレイ残数を取得
        /// </summary>
        /// <returns></returns>
        private int getRemainingTrayCount()
        {
            try
            {
                return Plc.GetWordAsDecimalData(COMPLETE_TRAY_REMAIN_COUNT_WORD_ADDR);
            }
            catch (Exception)
            {
                throw new ApplicationException($"残りトレイ数の取得に失敗しました。[アドレス:『{COMPLETE_TRAY_REMAIN_COUNT_WORD_ADDR}』]");
            }
        }

        private string getTrayNo(TrayPosition position)
        {
            string trayQR = string.Empty;
            string addressNo = string.Empty;

            if (position == TrayPosition.Left)
            {
                addressNo = COMPLETE_TRAY_QRCODE_LEFT_WORD_ADDR;
            }
            else
            {
                addressNo = COMPLETE_TRAY_QRCODE_RIGHT_WORD_ADDR;
            }
            trayQR = Plc.GetString(addressNo, DATAMATRIX_WORD_LENGTH, true);

            if (string.IsNullOrWhiteSpace(trayQR) == true)
            {
                throw new ApplicationException($"装置から収納基板情報が取得できません。[取得アドレス(開始)：『{COMPLETE_TRAY_QRCODE_LEFT_WORD_ADDR}』]");
            }
            trayQR = trayQR.Replace("\r", "").Replace("\0", "");

            return trayQR;
        }

        /// <summary>
        /// トレイで使用したシート枚数を取得
        /// </summary>
        /// <returns></returns>
        private int getTrayUsedSheetCount()
        {
            try
            {
                return Plc.GetWordAsDecimalData(COMPLETE_TRAY_USEDSHEETCT_WOED_ADDR);
            }
            catch (Exception)
            {
                throw new ApplicationException($"使用したシート数の取得に失敗しました。[アドレス:『{COMPLETE_TRAY_USEDSHEETCT_WOED_ADDR}』]");
            }
        }

        private string getLotTypeCd(string lotNo)
        {
            if (this.IsOutLine)
            {
                return Importer.GetTypeFromLotNo(lotNo);
            }
            else
            {
                AsmLot lot = AsmLot.GetAsmLot(lotNo);
                return lot.TypeCd;
            }
        }
    }
}
