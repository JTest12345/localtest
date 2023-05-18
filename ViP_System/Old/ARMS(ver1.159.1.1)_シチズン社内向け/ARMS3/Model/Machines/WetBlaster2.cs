using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// ウェットブラスト装置(SMD3in1高効率用)
    /// WetBlasterとの相違点： 装置からの読み取り内容を変更 (基板データマトリクス → マガジンQR)
    /// </summary>
    public class WetBlaster2 : MachineBase
    {
        private const int MAGAZINE_NO_WORD_LENGTH = 10;
        /// <summary>
        /// ローダー側マガジンNo読取完了アドレス
        /// </summary>
        public string LoaderQRReadCompleteBitAddress { get; set; }

        /// <summary>
        /// 【作業開始時】ローダー側読み取り済みマガジンQR格納アドレス
        /// </summary>
        public string WorkStartLoaderQRAddress { get; set; }

        /// <summary>
        /// 【作業完了時】ローダー側読み取り済みマガジンQR格納アドレス
        /// </summary>
        public string WorkCompleteLoaderQRAddress { get; set; }

        /// <summary>
        /// 【作業完了時】アンローダー側読み取り済みマガジンQR格納アドレス
        /// </summary>
        public string WorkCompleteUnloaderQRAddress { get; set; }

        /// <summary>
        /// 開始登録OKBit
        /// </summary>
        public string WorkStartOKBitAddress { get; set; }

        /// <summary>
        /// 開始登録NGBit
        /// </summary>
        public string WorkStartNGBitAddress { get; set; }

        /// <summary>
        /// 完了登録OKBit
        /// </summary>
        public string SendCompleteBitAddress { get; set; }

        /// <summary>
        /// 開始登録時にPC → 装置(LD/ULD)へ送る品種プログラムの番号格納アドレス
        /// </summary>
        public string StartTeachProgramWordAddress { get; set; }

        /// <summary>
        /// 開始登録時にPC ⇔ 装置(LD/ULD)間で通信する本体レシピ送信要求信号
        /// </summary>
        public string HostTeachProgramReqBitAddress { get; set; }

        /// <summary>
        /// 開始登録時にPC → 装置(LD/ULD)へ送る本体レシピ切替通信の結果
        /// </summary>
        public string HostTeachProgramResultWordAddress { get; set; }

        /// <summary>
        /// 本体用PLC
        /// </summary>
        public IPLC HostPlc { get; set; }

        #region PLCアドレス定義(本体用信号 - メーカー指定)

        private const string START_TEACH_PROGRAM_WORD_ADDR = "ZR001B76";
        private const string PROGRAM_CHANGE_REQ_BIT_ADDR = "M0005FA";
        private const string PROGRAM_CHANGING_BIT_ADDR = "M0005FB";
        private const string PROGRAM_CHANGE_NG_BIT_ADDR = "M0005FC";

        private const int RESPONSE_OK = 1;
        private const int RESPONSE_NG = 2;

        /// <summary>
        /// 本体とのレシピ変更の通信処理におけるタイムアウト時間(秒) PLC.WatchBit関数の引数
        /// </summary>
        private const int HOST_RESEIVE_TIMEOUTE_SECOND = 60;

        #endregion

        protected override void concreteThreadWork()
        {
            // 作業完了登録
            if (this.IsRequireOutput() == true)
            {
                // 前ロットの受信完了が装置側で未処理の間は次ロットの完了登録を処理しない
                if (this.Plc.GetBit(this.SendCompleteBitAddress) == PLC.Common.BIT_OFF)
                {
                    workComplete();
                }
            }

            // 作業開始登録
            if (this.Plc.GetBit(LoaderQRReadCompleteBitAddress) == PLC.Common.BIT_ON)
            {
                workStart();
            }

            // 本体PLCへレシピ送信
            if (this.Plc.GetBit(HostTeachProgramReqBitAddress) == PLC.Common.BIT_ON)
            {
                SendProgram();
            }
        }

        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //実マガジンのアンローダー以外は何もしない
            if (station != Station.Unloader)
            {
                return true;
            }

            return base.Enqueue(mag, station);
        }

        public void workStart()
        {
            try
            {
                VirtualMag mag = new VirtualMag();
                string magno = getMagazineNo(this.WorkStartLoaderQRAddress, "開始", "投入");

                Magazine svrMag = Magazine.GetCurrent(magno);
                if (svrMag == null)
                {
                    throw new ApplicationException($"稼働中マガジンが存在しません。マガジンNo:『{magno}』");
                }

                AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                if (lot == null)
                {
                    throw new ApplicationException($"ロット情報が存在しません。ロットNo:『{lot.NascaLotNo}』");
                }

                Process nextproc = Process.GetNextProcess(svrMag.NowCompProcess, lot);
                if (nextproc == null)
                {
                    throw new ApplicationException($"次作業が存在しません。ロットNo:『{lot.NascaLotNo}』,現在完了工程No：『{svrMag.NowCompProcess}』");
                }

                mag.MagazineNo = magno;
                mag.ProcNo = nextproc.ProcNo;

                List<Order> orderList = ArmsApi.Model.Order.SearchOrder(lot.NascaLotNo, mag.ProcNo, null, true, false).ToList();
                foreach (Order o in orderList)
                {
                    if (o.MacNo != this.MacNo)
                    {
                        throw new ApplicationException($"他の装置での開始実績が既に存在します。ロットNo:『{lot.NascaLotNo}』,作業No：『{ mag.ProcNo}』,装置番号:{o.MacNo}");
                    }
                    else
                    {
                        // 同じ装置の作業実績が登録済みの場合でも、『UnLoader』の仮想マガジンが登録済みの場合は、NG扱いとする。
                        VirtualMag unloadermag = VirtualMag.GetVirtualMag(this.MacNo, (int)Station.Unloader, magno);
                        if (unloadermag != null)
                        {
                            throw new ApplicationException($"この装置の完了通知ロットに含まれている為、開始登録できません。ロットNo:『{lot.NascaLotNo}』,作業No：『{ mag.ProcNo}』");
                        }
                    }
                }

                try
                {
                    // 作業開始時間取得
                    mag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
                }
                catch
                {
                    throw new ApplicationException($"LD/ULD部からの作業開始時間の取得に失敗。取得先アドレス：『{this.WorkStartTimeAddress}』");
                }

                #region データベースから装置へ送るプログラム番号を取得 (60mm：1　47mm：2)
                // レコードが無い場合は、関数内のthrow new ApplicationExceptionのコードに行く
                int programNo;
                try
                {
                    programNo = OvenProfile.GetOvenProfileId(mag.MagazineNo, mag.ProcNo.Value);
                }
                catch(Exception)
                {
                    throw new Exception($"ARMSデータベースからレシピ番号の取得に失敗しました。レシピ番号(オーブンプロファイル番号)が登録されているか確認して下さい。[タイプ：{lot.TypeCd}][工程No：{mag.ProcNo.Value.ToString()}]");
                }

                #endregion

                Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);
                order.LotNo = lot.NascaLotNo;
                order.TranStartEmpCd = "660";

                OutputSysLog($"[開始処理] 開始 LD部マガジンNo:{magno}");

                ArmsApiResponse workResponse = CommonApi.WorkStart(order);
                if (workResponse.IsError)
                {
                    throw new ApplicationException(workResponse.Message);
                }
                else
                {
                    // 品種プログラム送信 (60mm：1　47mm：2)
                    this.Plc.SetWordAsDecimalData(this.StartTeachProgramWordAddress, programNo);
                    OutputSysLog($"[開始処理] 品種プログラム送信完了 プログラム番号:{programNo}");
                    this.Plc.SetBit(this.WorkStartOKBitAddress, 1, Mitsubishi.BIT_ON);
                    OutputSysLog($"[開始処理] 完了 LD部マガジンNo:{mag.MagazineNo}");
                }
            }
            catch(Exception ex)
            {
                this.Plc.SetBit(this.WorkStartNGBitAddress, 1, Mitsubishi.BIT_ON);
                Log.ApiLog.Info($"装置:{this.Name} [開始登録異常] 理由:{ex.Message}");
                throw ex;
            }
        }

        public void workComplete()
        {
            try
            {
                VirtualMag mag = new VirtualMag();

                //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
                string lMagno = getMagazineNo(this.WorkCompleteLoaderQRAddress, "完了", "投入");
                string ulMagno = getMagazineNo(this.WorkCompleteUnloaderQRAddress, "完了", "排出");

                Magazine svrMag = Magazine.GetCurrent(lMagno);
                if (svrMag == null)
                {
                    throw new ApplicationException($"稼働中マガジンが存在しません。マガジンNo:『{lMagno}』");
                }

                AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                if (lot == null)
                {
                    throw new ApplicationException($"ロット情報が存在しません。ロットNo:『{lot.NascaLotNo}』");
                }

                OutputSysLog($"[完了登録処理] 開始 ロットNo:『{lot.NascaLotNo}』, マガジンNo:『{lMagno}』→『{ulMagno}』");

                Process nextproc = Process.GetNextProcess(svrMag.NowCompProcess, lot);
                if (nextproc == null)
                {
                    throw new ApplicationException($"次作業が存在しません。ロットNo:『{lot.NascaLotNo}』,現在完了工程No：『{svrMag.NowCompProcess}』");
                }

                mag.ProcNo = nextproc.ProcNo;
                mag.MagazineNo = ulMagno;
                mag.LastMagazineNo = lMagno;

                Order startOrder = Order.GetMachineOrder(this.MacNo, svrMag.NascaLotNO);
                if (startOrder == null)
                {
                    throw new ApplicationException($"この装置での開始実績が存在しません。手動で開始登録を行った後、装置監視を再開して下さい。ロットNo:『{lot.NascaLotNo}』,工程No：{svrMag.NowCompProcess}");
                }
                mag.WorkStart = startOrder.WorkStartDt;

                //既にキュー内に存在するかを確認
                bool found = false;

                VirtualMag[] mags = VirtualMag.GetVirtualMag(this.MacNo, ((int)Station.Unloader));

                foreach (VirtualMag exist in mags)
                {
                    if (exist.MagazineNo == ulMagno && exist.LastMagazineNo == lMagno)
                    {
                        found = true;
                        mag.WorkComplete = exist.WorkComplete;
                    }
                }
                //既存キュー内に存在しない場合のみ
                if (found == false)
                {
                    try
                    {
                        // 作業完了時間取得
                        mag.WorkComplete = this.Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
                    }
                    catch
                    {
                        throw new ApplicationException($"LD/ULD部からの作業完了時間の取得に失敗。取得先アドレス：『{this.WorkCompleteTimeAddress}』");
                    }
                }

                if (renameLotFiles(svrMag, mag) == false)
                {
                    Log.SysLog.Info($"[完了登録中断] WetBlaster装置 傾向管理ファイルが見つからないため完了処理スキップ。傾向管理ファイル置場：『{this.LogOutputDirectoryPath}』");
                    return;
                }
                Log.SysLog.Info("WetBlaster装置 ファイルリネーム完了:" + this.MacNo);

                //既存キュー内に存在しない場合のみ、Enqueue
                if (found == false)
                {
                    //完了登録にUnloaderマガジンが必要なので先に作成
                    this.Enqueue(mag, Station.Unloader);
                }

                Order order = CommonApi.GetWorkEndOrder(mag, this.MacNo, this.LineNo);
                ArmsApiResponse workResponse = CommonApi.WorkEnd(order);

                this.Dequeue(Station.Unloader);

                if (workResponse.IsError)
                {
                    throw new ApplicationException(workResponse.Message);
                }

                this.Plc.SetBit(this.SendCompleteBitAddress, 1, PLC.Common.BIT_ON);
                OutputSysLog($"[完了処理] 完了 ロットNo:『{lot.NascaLotNo}』, マガジンNo:『{lMagno}』→『{ulMagno}』");
            }
            catch (Exception ex)
            {
                Log.ApiLog.Info($"装置:{this.Name} [完了登録異常] 理由:{ex.Message}");
                throw ex;
            }
        }

        private string getMagazineNo(string address, string timingName, string locationName)
        {
            string magazineQR = this.Plc.GetWord(address, MAGAZINE_NO_WORD_LENGTH);
            //Null文字を置換
            magazineQR = magazineQR.Replace("\0", "");
            if (string.IsNullOrWhiteSpace(magazineQR) == true)
            {
                throw new ApplicationException($"{locationName}マガジンNOの取得に失敗。\nウェットブラスト装置{timingName}位置のマガジンは装置に記録がありません。\n手動で取り除いてください。");
            }

            string magno = magazineQR.Trim();
            string[] magStr = magazineQR.Split(new[] { ' ', '\r' });
            if (magStr.Length >= 2)
            {
                //識別文字を取り除き
                magno = magStr[1].Trim();
            }

            // 識別文字取り除き後にマガジンNoを再チェック
            if (string.IsNullOrWhiteSpace(magno) == true)
            {
                throw new ApplicationException($"{locationName}マガジンNOの取得に失敗。読取QR内容：『{magazineQR}』\nウェットブラスト装置{timingName}位置のマガジンは装置に記録がありません。\n手動で取り除いてください。");
            }

            return magno;
        }

        private void SendProgram()
        {
            try
            {
                OutputSysLog($"[品種切替送信] 開始 ");

                // LD/ULDのレシピ番号取得
                int programNo;
                try
                {
                    programNo = this.Plc.GetWordAsDecimalData(this.StartTeachProgramWordAddress);
                }
                catch(Exception)
                {
                    throw new ApplicationException($"LD/ULD部からレシピ番号の取得に失敗しました。取得アドレス：『{this.StartTeachProgramWordAddress}』");
                }

                // EICSの監視タイプを投入マガジンのタイプに変更する
                string magno = getMagazineNo(this.WorkStartLoaderQRAddress, "開始", "投入");
                Magazine svrMag = Magazine.GetCurrent(magno);
                if (svrMag == null)
                {
                    throw new ApplicationException($"投入マガジンが稼働中マガジンが存在しません。マガジンNo:『{magno}』");
                }
                AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                if (lot == null)
                {
                    throw new ApplicationException($"ロット情報が存在しません。ロットNo:『{lot.NascaLotNo}』");
                }
                if (MachineInfo.UpdateEICSType(this.PlantCd, lot.TypeCd) == true)
                {
                    OutputSysLog($"[EICS装置マスタ設定変更] 監視タイプ変更 新タイプ：『{lot.TypeCd}』 ");
                }
                else
                {
                    throw new ApplicationException($"EICS装置の監視タイプの変更に失敗しました。ArmsConfigファイルのEICS接続文字列の設定を見直してください。:『{Config.Settings.QCILConSTR}』");
                }
                
                // 本体に取得したレシピ番号と切替要求を送信
                try
                {
                    this.HostPlc.SetWordAsDecimalData(START_TEACH_PROGRAM_WORD_ADDR, programNo);
                    OutputSysLog($"[品種切替送信] 本体PLCへの切替番号の送信完了 送信先アドレス：『{START_TEACH_PROGRAM_WORD_ADDR}』 番号：『{programNo}』 ");
                }
                catch (Exception)
                {
                    throw new ApplicationException($"本体PLCへの切替番号({START_TEACH_PROGRAM_WORD_ADDR})=({programNo})の送信に失敗しました。");
                }
                try
                {
                    this.HostPlc.SetBit(PROGRAM_CHANGE_REQ_BIT_ADDR, 1, Common.BIT_ON);
                    OutputSysLog($"[品種切替送信] 本体への切替要求信号({PROGRAM_CHANGE_REQ_BIT_ADDR})=(ON：1)の送信完了");
                }
                catch (Exception)
                {
                    throw new ApplicationException($"本体PLCへの切替要求信号({PROGRAM_CHANGE_REQ_BIT_ADDR})=(ON：1)の送信に失敗しました。");
                }

                OutputSysLog($"[品種切替送信] 信号待機。本体の品種切替中信号({PROGRAM_CHANGING_BIT_ADDR})：OFF→ON");
                if (this.HostPlc.WatchBit(PROGRAM_CHANGING_BIT_ADDR, HOST_RESEIVE_TIMEOUTE_SECOND, PLC.Common.BIT_ON) == false)
                {
                    throw new ApplicationException($"一定時間内(={HOST_RESEIVE_TIMEOUTE_SECOND}秒)に品種切替中信号({PROGRAM_CHANGING_BIT_ADDR})がONになりませんでした。");
                }

                //本体の切り替え要求をOFFにする
                this.HostPlc.SetBit(PROGRAM_CHANGE_REQ_BIT_ADDR, 1, PLC.Common.BIT_OFF);
                OutputSysLog($"[品種切替送信] 本体への切替要求信号({PROGRAM_CHANGE_REQ_BIT_ADDR})=(OFF：0)の送信F完了 ");
                
                //一定時間 品種切替中信号が0になるのを待つ。超えたらタイムアウトを返す
                OutputSysLog($"[品種切替送信] 信号待機。本体の品種切替中信号({PROGRAM_CHANGING_BIT_ADDR})：ON→OFF ");
                if (this.HostPlc.WatchBit(PROGRAM_CHANGING_BIT_ADDR, HOST_RESEIVE_TIMEOUTE_SECOND, PLC.Common.BIT_OFF) == false)
                {
                    throw new ApplicationException($"一定時間内(={HOST_RESEIVE_TIMEOUTE_SECOND}秒)に品種切替中信号({PROGRAM_CHANGING_BIT_ADDR})がOFFになりませんでした。");
                }
                OutputSysLog($"[品種切替送信] 信号確認。本体の品種切替中信号({PROGRAM_CHANGING_BIT_ADDR})：ON→OFF");

                // 品種切替中信号がOFFになった時に、品種切替不可信号がONの場合、LD/ULD部に切り替え結果 = NG(2)を送信する
                if (this.HostPlc.GetBit(PROGRAM_CHANGE_NG_BIT_ADDR) == PLC.Common.BIT_ON)
                {
                    throw new ApplicationException($"装置本体側で品種切替処理異常が発生しました。本体PLCの品種切替不可信号({PROGRAM_CHANGE_NG_BIT_ADDR})：ONを確認。");
                }

                // LD/ULD部に切り替え結果 = OK(1)を送信 + 要求信号をOFFにする
                this.Plc.SetWordAsDecimalData(this.HostTeachProgramResultWordAddress, RESPONSE_OK);
                this.Plc.SetBit(this.HostTeachProgramReqBitAddress, 1, PLC.Common.BIT_OFF);
                OutputSysLog($"[品種切替送信] LD/ULD部への切替結果信号({this.HostTeachProgramResultWordAddress})=(OK:1)と" + 
                    $"品種切替要求信号({this.HostTeachProgramReqBitAddress})=(OFF：0)を送信完了 ");
            }
            catch (Exception ex)
            {
                // LD/ULD部に切り替え結果 = NG(2)を送信 + 要求信号をOFFにする
                this.Plc.SetWordAsDecimalData(this.HostTeachProgramResultWordAddress, RESPONSE_NG);
                this.Plc.SetBit(this.HostTeachProgramReqBitAddress, 1, PLC.Common.BIT_OFF);

                string errLog = $"装置:{this.Name} [品種切替送信異常] 理由:{ex.Message}";
                Log.ApiLog.Info(errLog);
                throw ex;
            }
        }

        /// <summary>
        /// EICSが作成したファイル名のリネーム
        /// </summary>
        private bool renameLotFiles(Magazine svmag, VirtualMag mag)
        {
            AsmLot lot = AsmLot.GetAsmLot(svmag.NascaLotNO);

            Order[] orders = Order.SearchOrder(svmag.NascaLotNO, null, null, this.MacNo, false, false, null, null, null, null);
            if (orders.Length == 0)
            {
                return false;
            }

            Order order = orders.FirstOrDefault();

            List<string> lotFilestemp = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, order.WorkStartDt, mag.WorkComplete.Value);

            if (lotFilestemp.Count == 0)
            {
                return false;
            }

            List<string> lotFiles = new List<string>();

            foreach (string file in lotFilestemp)
            {
                if (file.Contains(".OK") == true || file.Contains(".NG") == true)
                {
                    continue;
                }
                lotFiles.Add(file);
            }

            if (lotFiles.Count == 0)
            {
                return false;
            }

            foreach (string lotFile in lotFiles)
            {

                string dataMatrix = System.IO.Path.GetFileNameWithoutExtension(lotFile);

                //リネイム済みは除外
                if (dataMatrix.Split('_').Length >= MachineLog.FINISHED_RENAME_ELEMENT_NUM)
                {
                    continue;
                }

                MachineLog.ChangeFileName(lotFile, svmag.NascaLotNO, lot.TypeCd, order.ProcNo, svmag.MagazineNo);
                OutputSysLog($"[完了処理] ファイル名称変更 FileName:{lotFile}");
            }

            return true;
        }
    }
}
