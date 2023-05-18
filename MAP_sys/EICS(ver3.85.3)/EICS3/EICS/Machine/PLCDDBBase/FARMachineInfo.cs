using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EICS.Database;
using EICS.Structure;

namespace EICS.Machine.CIFSBase
{
    //NTSV用ギ酸リフロー装置（PINK製)
    public class FARMachineInfo : PLCDDGBasedMachine
    {
        public LSETInfo LsetInfo { get; set; }
        private string sectionCd { get; set; }

        private const string REQUEST_WORKSTART_ADDRESS = "B000201";

        private const string RESPONSE_WORKSTART_ADDRESS = "B000222";

        private const string TRGFILE_EXTENTION = ".trg";

        //硬化完了予測時間から実際にチェックするまでの待機時間
        //装置がファイルに書き込むまでのラグ対策
        private const int CHECK_WAIT_MINUTES = 10;

        //管理するグループ名とdatファイル内の列位置のリスト。
        //装置仕様的に今後管理項目が増えることは無いということで完全に決め打ち。
        private readonly Dictionary<string, int> manageGroup = new Dictionary<string, int>()
        {
            {"加熱炉熱電対1",16 },
            {"加熱炉熱電対2",17 },
            {"加熱炉熱電対3",18 },
            {"加熱炉熱電対4",19 }
        };
        

        private string connectOvenDBStr { get; set; }

        protected override int GetTimingNo(string chipNm)
        {
            return Constant.TIM_REFLOW;
        }

		public FARMachineInfo(LSETInfo lsetInfo)
		{
            this.LsetInfo = lsetInfo;

            SettingInfo settingInfo = SettingInfo.GetSingleton();
            this.connectOvenDBStr = settingInfo.OvenTemparatureControlConnectionString;
            this.sectionCd = Database.NASCA.Section.GetSectionCd(settingInfo.SectionCD, lsetInfo.InlineCD);

        }

		public override void CheckFile(LSETInfo lsetInfo)
        {

            base.machineStatus = Constant.MachineStatus.Runtime;

            bool done = StartingProcess(lsetInfo);

            int timingNo = GetTimingNo(lsetInfo.ChipNM);

            // 完了時処理関数
            CheckProfileProcess(lsetInfo, timingNo);

        }
        public override void InitFirstLoop(LSETInfo lsetInfo)
        {
            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            StartFileDir = Path.Combine(lsetInfo.InputFolderNM, settingInfoPerLine.GetStartFileDirNm(lsetInfo.EquipmentNO));

            if (Directory.Exists(StartFileDir) == false)
            {
                Directory.CreateDirectory(StartFileDir);
            }

            EndFileDir = Path.Combine(lsetInfo.InputFolderNM, settingInfoPerLine.GetEndFileDirNm(lsetInfo.EquipmentNO));

            if (Directory.Exists(EndFileDir) == false)
            {
                Directory.CreateDirectory(EndFileDir);
            }

            EncodeStr = settingInfoPerLine.GetEncodeStr(lsetInfo.EquipmentNO);

            // 設定ファイルPLC設定を基にPLCインスタンス初期化
            InitPLC(lsetInfo);
        }


        //装置信号を受け取ってtrgファイルを作成するルーチン。パラメータ出力機能が装置に無いのでパラチェックはなし。
        public override bool StartingProcess(LSETInfo lsetInfo)
        {
            try
            {
                if (requestWorkStart() == false) return false;

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号：{lsetInfo.EquipmentNO}, リフロー開始処理実行。");

                //ARMSから作業中のロット・マガジン情報を取得
                ArmsApi.Model.MachineInfo macInfo = ArmsApi.Model.MachineInfo.GetMachine(lsetInfo.EquipmentNO);
                if (macInfo == null) throw new ApplicationException($"装置より開始要求信号が検知されましたが、ARMS設備情報が取得できませんでした。");

                ArmsApi.Model.Order order = ArmsApi.Model.Order.GetCurrentWorkingOrderInMachine(macInfo.MacNo)
                                                                 ?.OrderByDescending(o => o.WorkStartDt)
                                                                 .FirstOrDefault();
                if (order == null) throw new ApplicationException($"装置より開始要求信号が検知されましたが、作業中の実績が取得できませんでした。");

                ArmsApi.Model.AsmLot lot = ArmsApi.Model.AsmLot.GetAsmLot(order.LotNo);
                if (lot == null) throw new ApplicationException($"装置より開始要求信号が検知されましたが、アッセン情報の取得に失敗しました。ロット:{order.LotNo}");

                ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetMagazine(order.LotNo);
                if (mag == null) throw new ApplicationException($"装置より開始要求信号が検知されましたが、マガジン情報の取得に失敗しました。ロット:{order.LotNo}");


                //ファイル生成
                if (Directory.Exists(this.StartFileDir) == false)
                {
                    Directory.CreateDirectory(this.StartFileDir);
                }

                string destPath = Path.Combine(this.StartFileDir, $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{lot.NascaLotNo}_{lot.TypeCd}_{order.ProcNo}_{mag.MagazineNo}" + TRGFILE_EXTENTION);
                File.Create(destPath);

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"設備番号：{lsetInfo.EquipmentNO}, 開始ファイル生成：{destPath}");

                responseWorkStartReq();

                //応答信号ONからPLCの要求信号OFFまでに間があったら嫌なので2秒待機。
                //この関数以外、EICSがハンドシェイクの応答速度を要求される箇所はないので待機の影響はなし。
                System.Threading.Thread.Sleep(2000);

                return true;
                
            }
            catch (Exception err)
            {
                throw new ApplicationException($"開始作業時に予期せぬエラーが発生しました。{err.ToString()}");
            }
        }


        /// <summary>
        /// 投入から一定時間経過したらプロファイルチェックをする関数。
        /// 傾向管理ではなくプロファイルのチェックのため、TnLOGにデータは残さない。（警告表示のみ）
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="timingNo"></param>
        public void CheckProfileProcess(LSETInfo lsetInfo, int timingNo)
        {

            string workFolder = Path.Combine(lsetInfo.InputFolderNM, "WORK");
            if(Directory.Exists(workFolder) == false)
            {
                Directory.CreateDirectory(workFolder);
            }                

            List<OvenTrgFileInfo> targetTrgList = new List<OvenTrgFileInfo>();
            if (isExistCompletedTrgFile(out targetTrgList) == false) return;

            Dictionary<string, string> errList = new Dictionary<string, string>();

            Dictionary<string, List<OvenTemperatureInfo>> workDataList
                        = new Dictionary<string, List<OvenTemperatureInfo>>();

            string errMsgStr = string.Empty;

            foreach (OvenTrgFileInfo file in targetTrgList)
            {
                GetWorkData(file, workFolder, out workDataList);


                foreach(KeyValuePair<string, int> group in manageGroup)
                {
                    string message  = OvenTemperatureCheckReporter.reportAction(lsetInfo, group.Key, workDataList[group.Key], file, this.sectionCd, this.connectOvenDBStr);
                    if(string.IsNullOrWhiteSpace(message) == false)
                    {
                        if(string.IsNullOrWhiteSpace(errMsgStr) == true)
                        {
                            errMsgStr = "温度プロファイル監視異常";
                        }
                        errMsgStr = errMsgStr + $":{group.Key}:{message}\r\n";
                    }
                }
                BackUpDataFile(file.FilePath, true);
                if (string.IsNullOrWhiteSpace(errMsgStr) == false)
                {
                    //本来は従来のエラー表示処理で表示すべきだが、他の処理が通常の装置と異なり過ぎて
                    //データ作るのが手間なのと、目的（異常時の装置停止・目視調査）のためには監視が止まったほうが
                    //都合がいいので例外で投げてしまう。
                    throw new ApplicationException(errMsgStr);
                }
            }


        }

        private bool requestWorkStart()
        {
            if (plc.GetBit(REQUEST_WORKSTART_ADDRESS) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム トレイ作業開始確認 [アドレス:{REQUEST_WORKSTART_ADDRESS}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void responseWorkStartReq()
        {
            plc.SetBit(RESPONSE_WORKSTART_ADDRESS, 1, "1");

            outputLog($"装置 << システム トレイ作業開始確認完了送信 [アドレス:{RESPONSE_WORKSTART_ADDRESS}]");
        }

        private void outputLog(string logText)
        {
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                $"設備:{this.LsetInfo.ModelNM}/{this.LsetInfo.EquipmentNO}/{this.LsetInfo.MachineSeqNO}号機 {logText}");
        }

        /// <summary>
        /// 規定時間超過したtrgファイルが存在するかチェック
        /// </summary>
        /// <param name="trgFileList"></param>
        /// <returns></returns>
        private bool isExistCompletedTrgFile(out List<OvenTrgFileInfo> trgFileList)
        {
            trgFileList = new List<OvenTrgFileInfo>();

            string[] fileList = Directory.GetFiles(StartFileDir, "*" + TRGFILE_EXTENTION);

            foreach(string file in fileList)
            {
                string[] fileElm = Path.GetFileNameWithoutExtension(file).Split('_');
                if (fileElm.Count() < 4) continue;

                ArmsApi.Model.Process work = ArmsApi.Model.Process.GetProcess(Convert.ToInt32(fileElm[3]));

                int totalCureTime = OvenTemperatureCheckReporter.GetCompleteMinutes(fileElm[2], work.WorkCd, this.connectOvenDBStr);
                DateTime startDt;
                if (DateTime.TryParseExact(fileElm[0], "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out startDt) == false)
                {
                    throw new ApplicationException($"ファイル名から日付が取得できません。{file}");
                }

                if (startDt.AddMinutes(totalCureTime + CHECK_WAIT_MINUTES) > DateTime.Now) continue;

                trgFileList.Add(new OvenTrgFileInfo()
                {
                    TypeCd = fileElm[2],
                    LotNo = fileElm[1],
                    StartDt = startDt,
                    EndDt = startDt.AddMinutes(totalCureTime),
                    FilePath = file,
                    WorkCd = work.WorkCd
                });
            }

            if(trgFileList.Any() == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 傾向管理ファイルをコピーして対象範囲のレコードを抽出する
        /// </summary>
        /// <param name="startDt"></param>
        /// <param name="cureTime"></param>
        /// <returns></returns>
        private void GetWorkData(OvenTrgFileInfo trgFile, string workFolderPath, 
            out Dictionary<string, List<OvenTemperatureInfo>> workDataList)
        {
            workDataList = new Dictionary<string, List<OvenTemperatureInfo>>();

            foreach (KeyValuePair<string, int> group in manageGroup)
            {
                workDataList.Add(group.Key, new List<OvenTemperatureInfo>());
            }

            //一時作業フォルダをクリア。コピーファイルを一時置きするフォルダなので誤削除でも致命的なことにはならない。
            Common.ClearDirectory(workFolderPath);

            SearchAndCopyDataFile(trgFile, workFolderPath);

            string[] targetFileList = Directory.GetFiles(workFolderPath);
            DateTime revTime = DateTime.MinValue;

            foreach (string file in targetFileList)
            {
                string[] allDatas = File.ReadAllLines(file);
                
                foreach (string data in allDatas)
                {
                    string[] dataElm = data.Split(';');
                    //熱電対4の列位置が20番目なのでとりあえずそれで規制。特に今後変更の予定なし。
                    if (dataElm.Count() < 20) continue;

                    DateTime lineDate;
                    if(DateTime.TryParseExact(dataElm[0], "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, 
                        System.Globalization.DateTimeStyles.None, out lineDate) == false)
                    {
                        continue;
                    }
                    if (lineDate < trgFile.StartDt || lineDate > trgFile.EndDt) continue;

                    //元の秒単位データは細かすぎるので従来のオーブンログ同様に10秒おきのデータにして集計。
                    //3分以上データ欠けがあればエラー停止(NAOR側の仕様に準じる)
                    if (revTime != DateTime.MinValue &&  lineDate >= revTime.AddMinutes(3))
                    {
                        throw new ApplicationException($"3分以上のデータ欠落が見つかったため処理停止:ファイル:{file}");
                    }
                    else if(lineDate >= revTime.AddSeconds(10))
                    {
                        revTime = lineDate;

                        foreach(KeyValuePair<string, List<OvenTemperatureInfo>> workdata in workDataList)
                        {
                            workdata.Value.Add(new OvenTemperatureInfo() { LogDT = lineDate, PresentVal = Convert.ToDecimal(dataElm[manageGroup[workdata.Key]]) });
                        }
                    }

                }
                BackUpDataFile(file, false);
            }

            //最終データと完了までの間の欠落もチェック
            if (revTime.AddMinutes(3) <= trgFile.EndDt)
            {
                throw new ApplicationException($"最終データからから完了時刻までに3分以上欠落が有るため処理停止:ファイル:{trgFile.FilePath}");
            }

        }


        /// <summary>
        /// 開始時刻～完了時間までのファイルを収集する。
        /// リフローは1時間程度なので3ファイル以上(3日間)にまたがることは考慮しない。
        /// </summary>
        /// <param name="startDt"></param>
        /// <param name="workFolderPath"></param>
        private void SearchAndCopyDataFile(OvenTrgFileInfo file, string workFolderPath)
        {
            string searchStrToStarFile = file.StartDt.ToString("yyyy-MM-dd");
            string searchStrToEndFile = file.EndDt.ToString("yyyy-MM-dd");

            string startFileDir = Path.Combine(this.LsetInfo.MachineFolderNM, file.StartDt.ToString("yyyy"), file.StartDt.ToString("MM"));
            if (Directory.Exists(StartFileDir) == false)
            {
                throw new ApplicationException($"傾向管理フォルダが見つかりません。パス：{startFileDir}");
            }

            List<string> targetFilelist = Directory.GetFiles(startFileDir, $"*{searchStrToStarFile}*").ToList();
            if (targetFilelist.Any() == false)
            {
                throw new ApplicationException($"傾向管理ファイルが見つかりません。パス：{startFileDir}, 検索文字列:{searchStrToStarFile}");
            }

            //開始から完了までの間で日をまたいでいる場合は2ファイル必要
            if (searchStrToStarFile != searchStrToEndFile)
            {
                string latestFileDir = Path.Combine(this.LsetInfo.MachineFolderNM, file.EndDt.ToString("yyyy"), file.EndDt.ToString("MM"));

                if (Directory.Exists(latestFileDir) == false)
                {
                    throw new ApplicationException($"傾向管理フォルダが見つかりません。パス：{latestFileDir}");
                }

                string[] targetFilelist2 = Directory.GetFiles(latestFileDir, $"*{searchStrToEndFile}*");
                if (targetFilelist2.Any() == false)
                {
                    throw new ApplicationException($"傾向管理ファイルが見つかりません。パス：{latestFileDir}, 検索文字列:{searchStrToEndFile}");
                }
                targetFilelist.AddRange(targetFilelist2);
            }

            foreach (string dataFile in targetFilelist)
            {
                if (Path.GetExtension(dataFile).ToUpper() == ".ZIP")
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(dataFile, workFolderPath);
                }
                else
                {
                    File.Copy(dataFile, Path.Combine(workFolderPath, Path.GetFileName(dataFile)));
                }
            }
        }

        private void BackUpDataFile(string file, bool isStart)
        {
            DateTime fileDate = File.GetLastAccessTime(file);
            string destPath;

            if (isStart)
            {
                destPath = Path.Combine(Path.Combine(this.LsetInfo.InputFolderNM, this.StartFileDir) , fileDate.ToString("yyyy"), fileDate.ToString("MM"));
            }
            else
            {
                destPath = Path.Combine(this.LsetInfo.InputFolderNM, fileDate.ToString("yyyy"), fileDate.ToString("MM"));
            }

            if(Directory.Exists(destPath) == false)
            {
                Directory.CreateDirectory(destPath);
            }

            string destfilePath = Path.Combine(destPath, Path.Combine(destPath, Path.GetFileName(file)));

            //バックアップ先に既に存在する場合は上書き
            File.Copy(file, Path.Combine(destPath, Path.GetFileName(file)), true);
            File.Delete(file);

        }
    }
}
