using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Drawing;
using EICS.NascaAPI;
using System.Diagnostics;
using EICS.Database;
using EICS.Machine.CIFSBase;
using EICS.Structure;
using EICS.Machine.PLCDDBBase;
using EICS.Machine.Base;

namespace EICS.Machine
{
    public abstract class MachineBase : IDisposable, IMachine
    {
        protected const string BACKUP_BIND_DIR_NM = "Bind";
        protected const string BACKUP_UNCHAIN_DIR_NM = "unchain";

        /// <summary>ファイル種類</summary>
        public enum FileType
        {
            EF, OR, PR, SF, EM, SM, AM, SD, O, P, W, M, L, H, I, B, SP, MP, ML, S, E
        }

        public bool IsErrorPR { get; set; }
        public bool IsErrorOR { get; set; }
        //public bool IsErrorAM { get; set; }
        //public bool IsErrorEM { get; set; }
        //public bool IsErrorEF { get; set; }
        //public bool IsErrorSD { get; set; }
        //public bool IsErrorSF { get; set; }
        //public bool IsErrorSM { get; set; }

        public string Name { get; set; }//
        public string AssetsNM { get; set; }
        public string Code { get; set; }
        public int LineCD { get; set; }

        public ConnectHSMS HSMS { get; set; }
        public LSETInfo lsetInfo { get; set; }

        protected Constant.MachineStatus machineStatus { get; set; }
        protected Constant.FrameSupplyStatus frameSupplyStatus { get; set; }
        protected List<ErrMessageInfo> errorMessageList;
        public RunningLog runningLog;
        protected bool StopMachineFG { get; set; }
        protected bool WaitForRenameByArmsFG { get; set; }

        protected int LastTimeOutSecond = 0;

        /// <summary>
        /// ロット情報の参照元
        /// </summary>
        public enum LotInfoReferring
        {
            ARMS,
            NASCA
        }
        //protected abstract void JudgeProcess(ReceiveMessageInfo receiveInfo);

        public MachineBase()
        {
            machineStatus = Constant.MachineStatus.Wait;
            runningLog = RunningLog.GetInstance();
        }

        public void InitHSMS(LSETInfo mLsetInfo)
        {
            lsetInfo = mLsetInfo;

            if (HSMS == null)
            {
                HSMS = new ConnectHSMS(lsetInfo.IPAddressNO, lsetInfo.PortNO);
            }
        }

        public void Dispose()
        {
            HSMS.Dispose();
        }

        protected void NetUse(string toConnect)
        {
            System.Diagnostics.ProcessStartInfo psi =
                        new System.Diagnostics.ProcessStartInfo();
            //ComSpecのパスを取得する
            psi.FileName = SettingInfo.GetNetExePath();// System.Environment.GetEnvironmentVariable("ComSpec");

            //出力を読み取れるようにする
            psi.RedirectStandardInput = false;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            //ウィンドウを表示しないようにする
            psi.CreateNoWindow = true;

            string cmnd = "";
            Process p = null;

            //文字列最後に続く\の文字を全て除去する。
            while (toConnect.Substring(toConnect.Length - 1, 1) == @"\")
            {
                toConnect = toConnect.Remove(toConnect.Length - 1);
            }

            //リモートＰＣと接続
            cmnd = "USE " + toConnect + " /user:inline inline";
            psi.Arguments = cmnd;
            p = Process.Start(psi);
            //出力を読み取る
            string results = p.StandardOutput.ReadToEnd();
            //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, cmnd + "実行");
            p.WaitForExit();
            //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, cmnd + "完了");
        }

        protected void CheckDirectory(LSETInfo lsetInfo)
        {
            DirectoryInfo directoryInfo;
            switch (lsetInfo.AssetsNM)
            {
                case Constant.ASSETS_DB_NM:
                    NetUse(lsetInfo.InputFolderNM);
                    Thread.Sleep(500);
                    break;
                case Constant.ASSETS_DC_NM:
                    NetUse(lsetInfo.InputFolderNM);
                    Thread.Sleep(500);
                    break;
                default:
                    break;
            }

            if (!Directory.Exists(lsetInfo.InputFolderNM))
            {
                //LSETのInputFolderNMフォルダが無い場合、作成
                directoryInfo = Directory.CreateDirectory(lsetInfo.InputFolderNM);
                Thread.Sleep(100);
                if (directoryInfo.Exists == false)
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_141, directoryInfo.FullName));
                }
            }
        }

        public virtual void InitFirstLoop(LSETInfo lsetInfo)
        {

        }

        /// <summary>
        /// 全ての装置スレッドで最初に呼び出されるメインルーチン
        /// ファイル処理だけでなく、装置と通信でデータ受信する装置も出てきた為
        /// いずれ関数名はCheckDataにしたい
        /// </summary>
        /// <param name="lsetInfo"></param>
        public virtual void CheckFile(LSETInfo lsetInfo)
        {
        }

        //<--NASCA不良のエラー判定実施:TnLogWaitingQueue(未判定)⇒判定⇒TnLogへ
        public virtual void CheckNascaError(LSETInfo lsetInfo)
        {
            using (var eicsDB = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, LineCD)))
            {
                //TnLogWaitingQueueから同一設備番号のレコードリスト取得
                var tnLogWaitingQueueList = eicsDB.TnLogWaitingQueue.Where(t => t.Equipment_NO == lsetInfo.EquipmentNO);

                foreach (DataContext.TnLogWaitingQueue tnLogWaitingQueue in tnLogWaitingQueueList)
                {
                    //List<ErrMessageInfo> errMessageList = new List<ErrMessageInfo>();
                    //TnLogにエラー判定してInsert
                    ConnectDB.InsertTnLOG(lsetInfo, LineCD, tnLogWaitingQueue.QcParam_NO, tnLogWaitingQueue.Material_CD.Trim(), tnLogWaitingQueue.Magazine_NO.Trim(),
                        tnLogWaitingQueue.NascaLot_NO.Trim(), Convert.ToString(tnLogWaitingQueue.DParameter_VAL), Convert.ToString(tnLogWaitingQueue.Measure_DT), ref this.errorMessageList);
                    //TnLogWaitingQueueからDelete
                    eicsDB.TnLogWaitingQueue.DeleteOnSubmit(tnLogWaitingQueue);
                    eicsDB.SubmitChanges();
                }
            }
        }
        //-->NASCA不良のエラー判定実施

        protected virtual void FileDistribution(ref TcpClient tcp, ref NetworkStream ns, string fileGetReqFgAddr, LSETInfo lsetInfo, string sTargetFile, ref List<ErrMessageInfo> errMessageList)
        {
        }

        /// <summary>
        /// CheckFileの中身を開始時処理と完了時処理に分けたいので、随時下記にまとめていく
        /// </summary>
        /// <param name="lsetInfo"></param>
        //public virtual void StartingProcess(LSETInfo lsetInfo)
        //{
        //}

        /// <summary>
        /// CheckFileの中身を開始時処理と完了時処理に分けたいので、随時下記にまとめていく
        /// </summary>
        /// <param name="lsetInfo"></param>
        public virtual void EndingProcess(LSETInfo lsetInfo)
        {

        }

        protected bool IsStopTargetFileKind(string prefix)
        {
            if (prefix == FileType.PR.ToString() && IsErrorPR)
            {
                return true;
            }
            else if (prefix == FileType.OR.ToString() && IsErrorOR)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// ファイル内容を取得(行配列)
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>内容配列</returns>
        public static string[] GetMachineFileLineValue(string filePath)
        {
            return GetMachineFileLineValue(filePath, System.DateTime.Now, 0);
        }

        public static string[] GetMachineFileLineValue(string filePath, int skipCT)
        {
            return GetMachineFileLineValue(filePath, System.DateTime.Now, skipCT);
        }

        public static string[] GetMachineFileLineValue(string filePath, DateTime startDT, int skipCT)
        {
            try
            {
                if (startDT.AddSeconds(10) <= System.DateTime.Now)
                {
                    //10秒の制限時間を超えた場合、エラー
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_50, filePath));
                }

                return File.ReadAllLines(filePath, System.Text.Encoding.Default).Skip(skipCT).ToArray();
            }
            catch (IOException)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("Filelock:{0}", filePath));
                Thread.Sleep(500);
                return GetMachineFileLineValue(filePath, startDT, skipCT);
            }
        }

        /// <summary>
        /// searchPatternで取得されるファイルの中身を結合して返り値として返す
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="searchPattern">正規表現による検索条件</param>
        /// <returns></returns>
        public IEnumerable<string> GetJoinedMachineFiles(string dirPath, string searchPattern)
        {
            List<string> fileContents = new List<string>();

            string[] pathArray = Directory.GetFiles(dirPath, "*.*");

            Regex regex = new Regex(searchPattern);

            IEnumerable<string> targetPathList = pathArray.Where(p => regex.IsMatch(p));

            foreach (string targetPath in targetPathList)
            {
                fileContents.AddRange(File.ReadAllLines(targetPath, System.Text.Encoding.UTF8));
            }

            return fileContents;
        }

        /// <summary>
        /// ファイル内容を取得
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>内容</returns>
        public string GetMachineFileValue(string filePath)
        {
            return GetMachineFileValue(filePath, System.DateTime.Now);
        }

        public string GetMachineFileValue(string filePath, DateTime startDT)
        {
            try
            {
                if (startDT.AddSeconds(10) <= System.DateTime.Now)
                {
                    //10秒の制限時間を超えた場合、エラー
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_50, filePath));
                }

                return File.ReadAllText(filePath, System.Text.Encoding.Default);
            }
            catch (IOException)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("Filelock:{0}", filePath));
                Thread.Sleep(500);
                return GetMachineFileValue(filePath, startDT);
            }
        }

        public List<string> GetMachineFileTxtList(string filePath, DateTime startDT)
        {
            try
            {
                if (startDT.AddSeconds(10) <= System.DateTime.Now)
                {
                    //10秒の制限時間を超えた場合、エラー
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_50, filePath));
                }

                return File.ReadAllLines(filePath, System.Text.Encoding.Default).ToList();
            }
            catch (IOException)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("Filelock:{0}", filePath));
                Thread.Sleep(500);
                return GetMachineFileTxtList(filePath, startDT);
            }
        }

        public List<ErrMessageInfo> GetErrorMessageList()
        {
            if (errorMessageList == null)
            {
                errorMessageList = new List<ErrMessageInfo>();
            }
            return errorMessageList;
        }

        public void InitErrorMessageList()
        {
            errorMessageList = new List<ErrMessageInfo>();
        }

        public Constant.MachineStatus GetMachineStatus()
        {
            return machineStatus;
        }

        #region NMC

        /// <summary>
        /// 設備ファイル処理(NMC)
        /// </summary>
        /// <param name="path">設備ファイル保存先</param>
        /// <param name="equipmentInfo">設備情報</param>
        /// <returns></returns>
        public void CheckMachineFileNMC(LSETInfo lsetInfo, Constant.MachineStatus machineStatus, ref List<ErrMessageInfo> errMessageList, Constant.FrameSupplyStatus frameSupplyStatus)
        {
            //↓↓↓↓↓インスタンスによって処理が勝手に分岐するような作りに変更する。

            ////実行された装置によって処理を分岐
            //switch (lsetInfo.AssetsNM)
            //{
            //    case Constant.NMC_ASSETS_DB_NM:

            //        DBMachineInfoNMC.CheckFile(lsetInfo, ref machineStatus, ref errMessageList);

            //        break;

            //    case Constant.NMC_ASSETS_WB_NM:

            //        WBMachineInfoNMC.CheckFile(lsetInfo, ref errMessageList, ref frameSupplyStatus);

            //        break;
            //}
        }

        /// <summary>
        /// 処理済みファイルを保管場所へ移動する(NMC)
        /// </summary>
        /// <param name="moveFromPath">移動元ファイルパス</param>
        /// <param name="moveToPath">移動先ファイルパス</param>
        public static void MoveMachineFileNMC(string fromPath, string toPath)
        {
            MoveMachineFileNMC(fromPath, toPath, 0, System.DateTime.Now);
        }

        public static void MoveMachineFileNMC(string fromPath, string toPath, int incrementNO, DateTime startDT)
        {
            try
            {
                if (startDT.AddSeconds(10) <= System.DateTime.Now)
                {
                    //10秒の制限時間を超えた場合、エラー
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_50, fromPath));
                }

                string moveToPath = toPath;
                if (incrementNO != 0)
                {
                    //保管場所に同名ファイルが存在するのでインクリメントした番号をファイル名に付ける。
                    FileInfo changeFileInfo = new FileInfo(toPath);
                    if (changeFileInfo.Extension != string.Empty)
                    {
                        moveToPath = Path.Combine(changeFileInfo.DirectoryName, changeFileInfo.Name.Replace(changeFileInfo.Extension, ""));
                    }
                    moveToPath = moveToPath + "_" + incrementNO + changeFileInfo.Extension;
                }

                if (File.Exists(moveToPath))
                {
                    //保管場所に同名ファイルが存在するので、番号をインクリメントして再帰
                    incrementNO++;

                    MoveMachineFileNMC(fromPath, toPath, incrementNO, startDT);
                }
                else
                {
                    FileInfo moveToInfo = new FileInfo(moveToPath);
                    if (!Directory.Exists(moveToInfo.DirectoryName))
                    {
                        //移動先フォルダが無い場合作成
                        Directory.CreateDirectory(moveToInfo.DirectoryName);
                    }
                    File.Move(fromPath, moveToInfo.FullName);
                }
            }
            catch (IOException)
            {
                //移動元ファイルが読み取り専用の場合、再帰
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("Filelock:{0}", fromPath));
                Thread.Sleep(500);
                MoveMachineFileNMC(fromPath, toPath, incrementNO, startDT);
            }
        }

        /// <summary>
        /// 判定ファイルの作成
        /// </summary>
        /// <param name="targetDirPath">作成場所ディレクトリ</param>
        public void CreateJudgeFile(string targetDirPath, int statusFG)
        {

            if (!Directory.Exists(targetDirPath))
            {
                throw new Exception(Constant.MessageInfo.Message_10);
            }

            string judgeDirPath = Path.Combine(targetDirPath, "judge");
            if (!Directory.Exists(judgeDirPath))
            {
                Directory.CreateDirectory(judgeDirPath);
            }

            string createFileNM
                = statusFG
                + System.DateTime.Now.ToString("dd")
                + ConvertNumberToAlphabet(Convert.ToInt16(System.DateTime.Now.Hour))
                + System.DateTime.Now.ToString("mmss")
                + "." + System.DateTime.Now.Millisecond;

            FileInfo fileInfo = new FileInfo(Path.Combine(judgeDirPath, createFileNM));
            FileStream fs = fileInfo.Create();
            fs.Dispose();
        }

        #endregion

        /// <summary>
        /// マッピングファイルの作成
        /// </summary>
        /// <param name="targetDirPath">作成場所ディレクトリ</param>
        /// <param name="fileName">作成ファイル名(ロット番号)</param>
        /// <param name="mappingList">作成ファイル内容</param>
        public static void CreateMappingFile(string targetDirPath, string fileName, List<MappingFile> mappingList)
        {
            if (!Directory.Exists(targetDirPath))
            {
                Directory.CreateDirectory(targetDirPath);
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(targetDirPath, fileName), false, Encoding.Default))
            {
                foreach (MappingFile mappingInfo in mappingList)
                {
#if Debug
						sw.Write("[" + mappingInfo.AddressNO + "]" + mappingInfo.MappingCD + ",");
						//sw.Write(mappingInfo.MappingCD + ",");
						Console.WriteLine(string.Format("{0} : {1}", mappingInfo.AddressNO, mappingInfo.MappingCD));
#else
                    sw.Write(mappingInfo.MappingCD + ",");
#endif
                }
            }
        }

        /// <summary>
        /// マッピングファイルを検索
        /// </summary>
        /// <param name="targetDirPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static FileInfo SearchMappingFile(string targetDirPath, string fileName)
        {
            if (!Directory.Exists(targetDirPath))
            {
                return null;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            if (fileName.Contains("_#"))
            {
                fileName = fileName.Substring(0, fileName.IndexOf("_#"));
            }
            //string[] fileList = Directory.GetFiles(targetDirPath, fileName + "*");
            List<string> fileList = Common.GetFiles(targetDirPath, fileName + ".*");

            if (fileList.Count == 0)
            {
                return null;
            }

            return new FileInfo(fileList[0]);
        }

        /// <summary>
        /// スタートファイルの取得
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="equipmentInfo">設備情報</param>
        /// <returns>スタートファイルリスト</returns>
        public static List<string> GetMachineStartFile(string machineDirPath, LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
        {
            List<string> startFileList = new List<string>();

            //スタートファイル識別文字を取得
            Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, (int)Constant.MachineFileType.Start);
            if (prefixList.Count == 0)
            {
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(Constant.MessageInfo.Message_36, Color.Red);
                errMessageList.Add(errMessageInfo);
                return startFileList;
            }

            string[] machineFileList = Directory.GetFiles(machineDirPath);
            foreach (string machineFile in machineFileList)
            {
                if (!File.Exists(machineFile))
                {
                    continue;
                }

                FileInfo fileInfo = new FileInfo(machineFile);

                //ARMSが処理するファイルの為、移動しない
                //if (fileInfo.Name.Substring(0, 1) == "B")
                //{
                //    continue;
                //}

                if (fileInfo.Name.Substring(0, 1) == "_")
                {
                    continue;
                }

                //0KBファイルを削除
                if (fileInfo.Length == 0)
                {
                    fileInfo.Delete();
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(Constant.MessageInfo.Message_44, fileInfo.FullName));
                    continue;
                }
                //ファイルが転送中かの確認
                if (!MachineFile.CheckFileAccess(fileInfo.FullName))
                {
                    continue;
                }

                foreach (string prefix in prefixList.Keys)
                {
                    if (fileInfo.Name.Contains(prefix))
                    {
                        startFileList.Add(fileInfo.FullName);
                        break;
                    }
                }
            }

            startFileList.Sort();

            return startFileList;
        }

        /// <summary>
        /// スタートファイルの取得
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="equipmentInfo">設備情報</param>
        /// <returns>スタートファイルリスト</returns>
        public static List<string> GetMachineStartFile(string targetDirPath, string magazineID)
        {
            List<string> startFileList = new List<string>();

            //string[] machineFileList = Directory.GetFiles(targetDirPath, "*." + magazineID);
            List<string> machineFileList = Common.GetFiles(targetDirPath, ".*", magazineID);

            foreach (string machineFile in machineFileList)
            {
                if (!File.Exists(machineFile))
                {
                    continue;
                }

                FileInfo fileInfo = new FileInfo(machineFile);

                ////ARMSが処理するファイルの為、移動しない
                //if (fileInfo.Name.Substring(0, 1) == "B")
                //{
                //    continue;
                //}

                if (fileInfo.Name.Substring(0, 1) == "_")
                {
                    continue;
                }

                //2012/06/27修正 T.Sasaki　Mファイルの条件追加（NMC用）
                if (!Regex.IsMatch(fileInfo.Name, "^O.*$") && !Regex.IsMatch(fileInfo.Name, "^P.*$") && !Regex.IsMatch(fileInfo.Name, "^M.*$"))
                {
                    continue;
                }

                //0KBファイルを削除
                if (fileInfo.Length == 0)
                {
                    fileInfo.Delete();
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(Constant.MessageInfo.Message_44, fileInfo.FullName));
                    continue;
                }
                //ファイルが転送中かの確認
                if (!MachineFile.CheckFileAccess(fileInfo.FullName))
                {
                    continue;
                }

                startFileList.Add(fileInfo.FullName);
            }

            return startFileList;
        }



        /// <summary>
        /// ファイル名からファイル識別文字を取得
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns>ファイル識別文字</returns>
        public static string GetMachineFileChar(string path, string assetsNM)
        {
            string fileChar = "";

            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Name.Substring(0, 1) == "_")
            {
                fileChar = fileInfo.Name.Substring(1, fileInfo.Name.Length - 1);
            }
            else
            {
                fileChar = fileInfo.Name;
            }

            if (assetsNM == Constant.ASSETS_DB_NM || assetsNM == Constant.NMC_ASSETS_DB_NM)
            {
                fileChar = fileChar.Substring(0, 1);
            }
            else if (assetsNM == Constant.ASSETS_DC_NM)
            {
                fileChar = fileChar.Substring(8, 2);
            }
            else
            {
                fileChar = fileChar.Substring(0, 2);
            }

            return fileChar;
        }

        protected void MoveExceptLatestFile(string sNascaLotNO, List<string> sFileList, string fileStampDT, LSETInfo lsetInfo)
        {
            string path = null;
            //最新の更新日付のデータのみ対象
            DateTime lastWrite = DateTime.MinValue;

            foreach (string sFilePath in sFileList)
            {
                FileInfo fi = new FileInfo(sFilePath);
                if (lastWrite >= fi.LastWriteTime)
                {
                    continue;
                }
                path = sFilePath;
                lastWrite = fi.LastWriteTime;
            }

            foreach (string sFilePath in sFileList)
            {
                if (sFilePath != path)
                {
                    FileInfo fileInfo = new FileInfo(sFilePath);
                    //File.Move(sFilePath, toMovePath + "\\" + fileInfo.Name);
                    MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, sNascaLotNO, fileStampDT);
                }
            }
        }



        /// <summary>
        /// 処理済みのファイルを移動する(※MoveCompleteMachineFile(FileInfo logFile)に移行していく)
        /// </summary>
        /// <param name="moveFilePath">処理済み移動ファイル名</param>
        public static string MoveCompleteMachineFile(string targetFilePath, LSETInfo lsetInfo, string lotNO, string fileStampDT)
        {
            string moveFolderPath = string.Empty;
            if (!File.Exists(targetFilePath))
            {
                return moveFolderPath;
            }

            FileInfo fileInfo = new FileInfo(targetFilePath);

            string sCreateFileYM = File.GetLastWriteTime(fileInfo.FullName).ToString("yyyyMM");

            if (lsetInfo.AssetsNM == Constant.ASSETS_MD_NM || lsetInfo.AssetsNM == Constant.ASSETS_ECK_NM || lsetInfo.AssetsNM == Constant.ASSETS_CR_NM ||
                lsetInfo.AssetsNM == Constant.ASSETS_RP_NAISEI_NM || lsetInfo.AssetsNM == Constant.ASSETS_CR2_NM || lsetInfo.AssetsNM == Constant.ASSETS_PLADCOM_NM
                || lsetInfo.AssetsNM == Constant.ASSETS_SCREW_MD_NM)
            {
                moveFolderPath = Path.Combine(fileInfo.Directory.FullName, sCreateFileYM);
            }
            else if (lsetInfo.AssetsNM == Constant.ASSETS_DB_NM || lsetInfo.AssetsNM == Constant.ASSETS_WB_NM || lsetInfo.AssetsNM == Constant.ASSETS_BB_NM)
            {
                if (lotNO == "" || lotNO == null)
                {
                    moveFolderPath = Path.Combine(lsetInfo.InputFolderNM, sCreateFileYM, BACKUP_UNCHAIN_DIR_NM, fileStampDT);
                }
                else
                {
                    moveFolderPath = Path.Combine(lsetInfo.InputFolderNM, sCreateFileYM, BACKUP_BIND_DIR_NM, lotNO);
                }
            }
            else if (lsetInfo.AssetsNM == Constant.ASSETS_AI_NM || lsetInfo.AssetsNM == Constant.ASSETS_AI9CAM_NM ||
                lsetInfo.AssetsNM == Constant.ASSETS_AIJTYPE_NM || lsetInfo.AssetsNM == Constant.ASSETS_AINTYPE_NM || lsetInfo.AssetsNM == Constant.ASSETS_AIRAIM_NM)// 2013/10/16 n.yoshimoto EICS3の車載・ハイブリッドのマージ失敗による緊急改修（MDマッピング照合機能）
            {
                moveFolderPath = Path.Combine(lsetInfo.InputFolderNM, sCreateFileYM, lotNO);
            }
            else
            {
                moveFolderPath = Path.Combine(Directory.GetParent(fileInfo.Directory.FullName).FullName, sCreateFileYM);
            }

            string moveFilePath = Path.Combine(moveFolderPath, fileInfo.Name);

            //ﾌｧｲﾙ情報をデータベースへ登録したので、保管フォルダへファイル移動
            //保管フォルダが無ければ作成
            if (Directory.Exists(moveFolderPath) == false)
            {
                Directory.CreateDirectory(moveFolderPath);
            }
            else
            {
                if (File.Exists(moveFilePath))
                {
                    //登録済みﾌｧｲﾙは削除して次へ
                    File.Delete(fileInfo.FullName);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(Constant.MessageInfo.Message_46, fileInfo.FullName));
                    return moveFolderPath;
                }
            }

            if (File.Exists(moveFilePath) == false)
            {
                try
                {
                    File.Move(fileInfo.FullName, moveFilePath);
                }
                catch
                {
                    //読み取り専用になる為、2秒後もう一度試す。
                    Thread.Sleep(2000);
                    File.Move(fileInfo.FullName, moveFilePath);
                }
            }
            else//既にある場合、delete
            {
                File.Delete(fileInfo.FullName);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("移動先に既に同名ファイルが存在する為、削除 削除対象:{0}", fileInfo.FullName));
            }

            return moveFolderPath;
        }

        /// <summary>
        /// ファイル名からロット番号を抽出して現在フォルダ\年月\ロットNoのフォルダへ移動する関数。
        /// 引数にアンダースコア区切りで何要素目がロット番号かを指定する。
        /// </summary>
        protected void BackupMachineFileByFileNm(string[] filePathList, int lotNoColumn)
        {
            foreach (string filePath in filePathList)
            {
                string[] fileElm = Path.GetFileNameWithoutExtension(filePath).Split('_');
                if (fileElm.Count() < lotNoColumn) continue;

                string lotNo = fileElm[lotNoColumn - 1];

                FileInfo fileInfo = new FileInfo(filePath);

                DateTime lastWrite = fileInfo.LastWriteTime;

                string destPath = Path.Combine(fileInfo.DirectoryName,
                                               fileInfo.LastWriteTime.ToString("yyyy"),
                                               fileInfo.LastWriteTime.ToString("MM"),
                                               lotNo);
                MoveFile(fileInfo, destPath);
            }
        }

        protected void BackupMpd(LSETInfo lsetInfo)
        {
            BackupMpd(lsetInfo, null);
        }

        protected void BackupMpd(LSETInfo lsetInfo, string targetDirNm)
        {
            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            string endDirNm;

            if (string.IsNullOrEmpty(targetDirNm))
            {
                endDirNm = settingInfoPerLine.GetEndFileDirNm(lsetInfo.EquipmentNO);
            }
            else
            {
                endDirNm = targetDirNm;
            }

            string targetDir = string.Empty;

            if (string.IsNullOrEmpty(endDirNm) == false)
            {
                targetDir = Path.Combine(lsetInfo.InputFolderNM, endDirNm);
                if (Directory.Exists(targetDir) == false)
                {
                    Directory.CreateDirectory(targetDir);
                }
            }
            else
            {
                return;
            }

            List<string> finFiles = Common.GetFiles(targetDir, string.Format(".*_.*_.*_.*_.*[.]{0}", CIFS.EXT_FIN_FILE));

            //AD838L改修時にAD830の改修依頼忘れらしく、finで出てこないので .outにも対応　その他の型式で同様の事が無いように 2016/6/6 n.yoshi
            finFiles.AddRange(Common.GetFiles(targetDir, string.Format(".*_.*_.*_.*_.*[.]{0}", CIFS.EXT_AD830_FIN_FILE)));

            foreach (string finPath in finFiles)
            {
                if (File.Exists(finPath) == false)
                {
                    continue;
                }

                CIFSBasedMachine.FileNameAdditionInfo fileNmAddInfo = CIFSBasedMachine.GetInfoFromFileNm(finPath, CIFS.FILE_NM_SPLITTER
                    , CIFSBasedMachine.IDENTITY_INDEX_IN_FILENM, CIFSBasedMachine.TYPECD_INDEX_IN_FILENM, CIFSBasedMachine.LOTNO_INDEX_IN_FILENM
                    , CIFSBasedMachine.MAGNO_INDEX_IN_FILENM, CIFSBasedMachine.PROC_INDEX_IN_FILENM);

                if (Database.LENS.WorkResult.IsComplete(
                    fileNmAddInfo.LotNo, fileNmAddInfo.ProcNo, lsetInfo.InlineCD, fileNmAddInfo.Identity, lsetInfo.EquipmentNO) == false)
                {
                    continue;
                }

                DateTime backupDt = File.GetLastWriteTime(finPath);

                string backupDir = Path.Combine(Path.GetDirectoryName(finPath)
                    , backupDt.ToString("yyyyMM"), BACKUP_BIND_DIR_NM, fileNmAddInfo.LotNo);

                if (Directory.Exists(backupDir) == false)
                {
                    Directory.CreateDirectory(backupDir);
                }

                List<string> mpdFiles = Common.GetFiles(targetDir, string.Format("^{0}[.]{1}", fileNmAddInfo.Identity, CIFSBasedMachine.EXT_MPD_FILE));

                foreach (string mpdPath in mpdFiles)
                {
                    CIFSBasedMachineAddMPD.CountDefect(mpdPath, lsetInfo.InlineCD, fileNmAddInfo.LotNo, fileNmAddInfo.MagNo, lsetInfo.EquipmentNO, fileNmAddInfo.ProcNo, fileNmAddInfo.Identity, lsetInfo.EquipInfo.ErrConvWithProcNo);

                    string mpdBkPath = Path.Combine(backupDir, Path.GetFileName(mpdPath));
                    string finBkPath = Path.Combine(backupDir, Path.GetFileName(finPath));

                    if (File.Exists(mpdBkPath))
                    {
                        throw new ApplicationException(
                            string.Format("ﾊﾞｯｸｱｯﾌﾟ先に既に同名mpdﾌｧｲﾙが存在する為、ﾊﾞｯｸｱｯﾌﾟ処理を中断します。問題のﾌｧｲﾙﾊﾟｽ：{0}", mpdBkPath));
                    }

                    if (File.Exists(finBkPath))
                    {
                        throw new ApplicationException(
                            string.Format("ﾊﾞｯｸｱｯﾌﾟ先に既に同名finﾌｧｲﾙが存在する為、ﾊﾞｯｸｱｯﾌﾟ処理を中断します。問題のﾌｧｲﾙﾊﾟｽ：{0}", finBkPath));
                    }

                    File.Move(mpdPath, mpdBkPath);
                    File.Move(finPath, finBkPath);
                }
            }
        }

        /// <summary>
        /// 完了フォルダ(日付フォルダ)へログファイルを移動
        /// </summary>
        /// <param name="logFile">ログファイル</param>
        public static void MoveCompleteMachineFile(FileInfo logFile, DateTime backupDirDT)
        {
            MoveCompleteMachineFile(logFile, backupDirDT, System.DateTime.Now);
        }

        /// <summary>
        /// ﾌｧｲﾙ情報と移動開始日時を指定して呼び出すとﾌｧｲﾙをﾌｧｲﾙが有った階層の./年フォルダ/月フォルダに移動
        /// 移動開始から5分以上経過するとファイルがロックされている可能性がある旨をエラー出力する
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="moveStartDT"></param>
        public static void MoveCompleteMachineFile(FileInfo logFile, DateTime backupDirDT, DateTime moveStartDT)
        {
            if (moveStartDT <= System.DateTime.Now.AddMinutes(-1))
            {
                throw new ApplicationException(string.Format(Constant.MessageInfo.Message_50, logFile.FullName));
            }

            string yearDirNM = string.Format("{0}", backupDirDT.Year);
            string monthDirNM = string.Format("{0}", backupDirDT.Month.ToString("00"));
            string compDirNM = Path.Combine(logFile.Directory.FullName, yearDirNM, monthDirNM);

            if (!Directory.Exists(compDirNM))
            {
                Directory.CreateDirectory(compDirNM);
            }

            try
            {
                logFile.MoveTo(Path.Combine(compDirNM, logFile.Name));
            }
            catch (Exception)
            {
                Thread.Sleep(1000);
                MoveCompleteMachineFile(logFile, backupDirDT, moveStartDT);
            }
        }

        /// <summary>
        /// 処理済みのファイル名を変更する
        /// </summary>
        /// <param name="targetFilePath"></param>
        public static void ChangeCompleteMachineFile(string targetFilePath, ref List<ErrMessageInfo> errMessageList)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(targetFilePath);

                string changeFilePath = Path.Combine(fileInfo.Directory.FullName, "_" + fileInfo.Name);
                if (!File.Exists(changeFilePath))
                {
                    try
                    {
                        File.Move(fileInfo.FullName, changeFilePath);
                    }
                    catch (Exception err)
                    {
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
                            "日付フォルダへの移動に失敗した為、3秒後再移動します。対象ﾌｧｲﾙ:{0} 例外詳細:{1}", fileInfo.FullName, err.Message));
                        //読み取り専用になる為、3秒後もう一度試す。
                        Thread.Sleep(3000);
                        File.Move(fileInfo.FullName, changeFilePath);
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception err)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, err.Message);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(Constant.MessageInfo.Message_37, Color.Red);
                errMessageList.Add(errMessageInfo);
                throw;
            }
        }

        /// <summary>
        /// 処理をするファイルを処理中フォルダに移動する(Runフォルダ)
        /// </summary>
        public static void MoveRunFile(string pFilePath)
        {
            FileInfo pFileInfo = new FileInfo(pFilePath);

            //Runフォルダがない場合作成
            string runFolderPath = Path.Combine(pFileInfo.Directory.FullName, "Run");
            if (!Directory.Exists(runFolderPath))
            {
                Directory.CreateDirectory(runFolderPath);
            }

            //基点ファイルが存在するフォルダ内のファイルをループ
            string[] fileList = Directory.GetFiles(pFileInfo.Directory.FullName);
            foreach (string file in fileList)
            {
                if (!File.Exists(file))
                {
                    continue;
                }

                FileInfo tFileInfo = new FileInfo(file);

                ////ARMSが処理するファイルの為、移動しない
                //if (tFileInfo.Name.Substring(0, 1) == "B")
                //{
                //    continue;
                //}

                //基点ファイルより対象ファイルの更新日時が古い場合→Run(処理をするフォルダ)にファイル移動
                if (pFileInfo.LastWriteTime >= tFileInfo.LastWriteTime)
                {
                    try
                    {
                        tFileInfo.MoveTo(Path.Combine(runFolderPath, tFileInfo.Name));
                    }
                    catch
                    {
                        //読み取り専用になる為、3秒後もう一度試す。
                        Thread.Sleep(3000);
                        tFileInfo.MoveTo(Path.Combine(runFolderPath, tFileInfo.Name));
                    }
                }
            }
        }

        /// <summary>
        /// 処理中フォルダ(Run)に移動する
        /// </summary>
        /// <param name="targetDirPath"></param>
        /// <param name="magazineID"></param>
        public static void MoveRunFile(string targetDirPath, string magazineID, List<string> moveTargetFileList)
        {
            MoveRunFile(targetDirPath, magazineID, System.DateTime.Now, moveTargetFileList);
        }

        public static void MoveRunFile(string targetDirPath, string magazineID, DateTime startDT, List<string> moveTargetFileList)
        {
            try
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"[MoveRunFile]ファイル移動開始時刻：{startDT.ToString()}");

                if (startDT.AddSeconds(10) <= System.DateTime.Now)
                {
                    //10秒の制限時間を超えた場合、エラー
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_50, targetDirPath));
                }

                //Runフォルダがない場合作成
                string runDirPath = Path.Combine(targetDirPath, "Run");
                if (!Directory.Exists(runDirPath))
                {
                    Directory.CreateDirectory(runDirPath);
                }



                //対象ファイルをRunフォルダに移動
                //string[] files = Directory.GetFiles(targetDirPath, "*" + magazineID + ".???");
                List<string> files = new List<string>();

                if (moveTargetFileList != null && moveTargetFileList.Count > 0)
                {
                    files = moveTargetFileList;
                }
                else
                {
                    files = Common.GetFiles(targetDirPath, ".*" + magazineID, ".*");
                }

                int filecount = 0;
                foreach (string targetFile in files)
                {
                    ////ARMSが処理するファイルの為、移動しない
                    //string fileNM = Path.GetFileNameWithoutExtension(targetFile);
                    //if (Regex.IsMatch(fileNM, "^B.*$"))
                    //{
                    //    continue;
                    //}

                    filecount++;
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"移動対象ファイル：{targetFile} 総ファイル数：{files.Count} 現在の処理ファイルNO：{filecount}");

                    File.Move(targetFile,
                        Path.Combine(runDirPath, Path.GetFileName(targetFile)));
                }
            }
            catch (IOException ex)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.ERROR, "[MoveRunFile]エラー発生のためリトライ " + ex.Message);

                Thread.Sleep(500);
                MoveRunFile(targetDirPath, magazineID, startDT, moveTargetFileList);
            }
        }


        /// <summary>
        /// 処理しないファイルを移動する(Reserveフォルダ)
        /// </summary>
        /// <param name="targetFilePath"></param>
        /// <param name="assetsNM"></param>
        public static void MoveReserveFile(string targetFilePath, string assetsNM)
        {
            FileInfo targetFileInfo = new FileInfo(targetFilePath);

            string moveToFolderPath = "";
            if (assetsNM == Constant.ASSETS_PLA_NM)
            {
                moveToFolderPath = targetFileInfo.Directory.Parent.FullName;
            }
            else
            {
                moveToFolderPath = targetFileInfo.Directory.FullName;
            }

            string reserveFolderPath = Path.Combine(moveToFolderPath, "reserve");
            if (!Directory.Exists(reserveFolderPath))
            {
                Directory.CreateDirectory(reserveFolderPath);
            }

            try
            {
                targetFileInfo.MoveTo(Path.Combine(reserveFolderPath, targetFileInfo.Name));
            }
            catch
            {
                //読み取り専用になる為、3秒後もう一度試す。
                Thread.Sleep(3000);
                targetFileInfo.MoveTo(Path.Combine(reserveFolderPath, targetFileInfo.Name));
            }
        }

        /// <summary>
        /// 処理しないフォルダ内ファイルを移動する(Reserveフォルダ)
        /// </summary>
        /// <param name="targetFolderPath"></param>
        public static void MoveReserveFolderFiles(string targetFolderPath)
        {
            string reserveFolderPath = Path.Combine(targetFolderPath, "reserve");
            if (!Directory.Exists(reserveFolderPath))
            {
                Directory.CreateDirectory(reserveFolderPath);
            }

            string[] files = Directory.GetFiles(targetFolderPath);
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);

                try
                {
                    fileInfo.MoveTo(Path.Combine(reserveFolderPath, fileInfo.Name));
                }
                catch
                {
                    //読み取り専用になる為、3秒後もう一度試す。
                    Thread.Sleep(3000);
                    fileInfo.MoveTo(Path.Combine(reserveFolderPath, fileInfo.Name));
                }
            }
        }

        /// <summary>
        /// フォルダ内の最新ファイルを取得
        /// </summary>
        public static FileInfo GetLatestFile(string targetFolderPath)
        {
            List<FileInfo> fileList = new List<FileInfo>();

            string[] files = Directory.GetFiles(targetFolderPath);
            foreach (string file in files)
            {
                fileList.Add(new FileInfo(file));
            }

            //最新ファイルの更新日時を取得
            return fileList.FindAll(f => f.LastAccessTime == fileList.Max(f2 => f2.LastAccessTime))[0];
        }

        /// <summary>
        /// 処理済みか確認する
        /// </summary>
        /// <param name="targetFilePath"></param>
        /// <param name="assetsNM"></param>
        /// <returns></returns>
        public static bool CheckEndFile(string targetFilePath, string targetFolderPath, string sFileStamp, string lotNO)
        {
            bool endFG = false;

            if (lotNO == string.Empty)
            {
                return endFG;
            }

            FileInfo tFile = new FileInfo(targetFilePath);

            //ファイルの更新日時から年月フォルダを特定
            string dtFolderPath = Convert.ToString(tFile.LastWriteTime.Year) + tFile.LastWriteTime.Month.ToString("00");
            dtFolderPath = Path.Combine(targetFolderPath, dtFolderPath);

            //保存先フォルダを検索(unchain)
            string mFolder = Path.Combine(dtFolderPath, BACKUP_UNCHAIN_DIR_NM);
            mFolder = Path.Combine(mFolder, sFileStamp);
            if (Directory.Exists(mFolder))
            {
                if (File.Exists(Path.Combine(mFolder, tFile.Name)))
                {
                    endFG = true;
                }
            }
            //保存先フォルダを検索(Bind)
            mFolder = Path.Combine(dtFolderPath, BACKUP_BIND_DIR_NM);
            mFolder = Path.Combine(mFolder, lotNO);
            if (Directory.Exists(mFolder))
            {
                if (File.Exists(Path.Combine(mFolder, tFile.Name)))
                {
                    endFG = true;
                }
            }

            return endFG;
        }

        public static bool OutputErr(LSETInfo lsetInfo, Plm plmInfo, MagInfo MagInfo, string sValue, string dtMeasureDT, ref List<ErrMessageInfo> errMessageList)
        {
            List<Log> logList = new List<Log>();

            return OutputErr(lsetInfo, plmInfo, MagInfo, sValue, dtMeasureDT, ref errMessageList, ref logList, true);
        }

        public class JudgeResult
        {
            public List<ErrMessageInfo> ErrMessageList { get; set; }
            public List<Log> LogList { get; set; }
        }

        public static bool OutputErr(LSETInfo lsetInfo, Plm plmInfo, string typeCD, string magNo, string lotNo, string sValue, string dtMeasureDT, ref List<ErrMessageInfo> errMessageList, ref List<Log> logList)
        {
            MagInfo magInfo = new MagInfo();

            magInfo.sNascaLotNO = lotNo;
            magInfo.sMagazineNO = magNo;
            magInfo.sMaterialCD = typeCD;

            return OutputErr(lsetInfo, plmInfo, magInfo, sValue, dtMeasureDT, ref errMessageList, ref logList, true);
        }

        public static JudgeResult JudgeParam(LSETInfo lsetInfo, Plm plmInfo, string typeCD, string magNo, string lotNo, string sValue, string dtMeasureDT, List<ErrMessageInfo> errMessageList, List<Log> logList)
        {
            return JudgeParam(lsetInfo, plmInfo, typeCD, magNo, lotNo, sValue, dtMeasureDT, errMessageList, logList, false, true);
        }

        public static JudgeResult JudgeParam(LSETInfo lsetInfo, Plm plmInfo, string typeCD, string magNo, string lotNo, string sValue, string dtMeasureDT, List<ErrMessageInfo> errMessageList, List<Log> logList, bool needRegisterData, bool isErrorSound)
        {
            JudgeResult jr = new JudgeResult();

            jr.LogList = new List<Log>();

            MagInfo magInfo = new MagInfo();

            magInfo.sNascaLotNO = lotNo;
            magInfo.sMagazineNO = magNo;
            magInfo.sMaterialCD = typeCD;

            OutputErr(lsetInfo, plmInfo, magInfo, sValue, dtMeasureDT, ref errMessageList, ref logList, isErrorSound);

            jr.ErrMessageList = errMessageList;

            if (needRegisterData)
            {
                jr.LogList = logList;
            }

            return jr;
        }

        /// <summary>
        /// NG判定を行い、エラーメッセージを表示する。(データベース登録はしない)
        /// </summary>
        /// <param name="sQcParamNO"></param>
        /// <param name="sLotNO"></param>
        /// <param name="sParameterVAL"></param>
        /// <param name="dtLastUpdDT"></param>
        //public static bool OutputErr(LSETInfo lsetInfo, ParamInfo ParamInfo, PLMInfo plmInfo, MagInfo MagInfo, string sValue, string dtMeasureDT, ref List<ErrMessageInfo> errMessageList)
        public static bool OutputErr(LSETInfo lsetInfo, Plm plmInfo, MagInfo MagInfo, string sValue, string dtMeasureDT, ref List<ErrMessageInfo> errMessageList, ref List<Log> logList, bool isPlaySound)
        {
            string sMessage = "";
            Decimal? dValue;
            bool fErr = false;//ｴﾗｰあるなし
            Log log = new Log();

            string logMsg;
            Log.ParameterSet paramSet = Log.GetParameter(plmInfo, sValue, out logMsg);

            if (!string.IsNullOrEmpty(logMsg))
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{0} 測定日時:{1} {2}", lsetInfo.EquipmentNO, dtMeasureDT, logMsg));
            }

            dValue = paramSet.DParameterVAL;

            //NG判定してメッセージ入れ込み
            if (plmInfo != null && plmInfo.ParameterVAL == "")//0:数値で、管理限界情報を取得出来ている場合
            {
                if (Plm.HasDecimalLimit(plmInfo))
                {
                    if (dValue.HasValue == false)
                    {
                        string nullValErrFormat = string.Format("[{0}/{1}号機][管理番号:{5}/{2}]閾値が設定されていますがﾌｧｲﾙからの取得値がnull値です。Lot={3},Linecd={4}");
                        if (isPlaySound)
                        {
                            F01_MachineWatch.sp.PlayLooping();
                        }
                        sMessage = string.Format(nullValErrFormat, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO, plmInfo.ParameterNM, MagInfo.sNascaLotNO, lsetInfo.InlineCD, plmInfo.QcParamNO);
                        ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                        errMessageList.Add(errMessageInfo);

                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                        return false;
                    }

                    if (plmInfo.ManageNM == Constant.sMAXMIN)
                    {
                        if (plmInfo.ParameterMAX < dValue)
                        {
                            sMessage = "[{0}/{1}号機][管理番号:{2}/{3}]が管理限界値(MAX)を越えました。\r\n取得値={4},閾値MAX={5}"; //2015.10.21修正。表示内容に管理番号追加。
                            sMessage = string.Format(sMessage, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO, plmInfo.QcParamNO, plmInfo.ParameterNM, dValue, plmInfo.ParameterMAX);

                            if (isPlaySound)
                            {
                                F01_MachineWatch.sp.PlayLooping();
                            }
                            ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                            errMessageList.Add(errMessageInfo);

                            fErr = true;
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                        }
                        else if (plmInfo.ParameterMIN > dValue)
                        {
                            sMessage = "[{0}/{1}号機][管理番号:{2}/{3}]が管理限界値(MIN)を越えました。\r\n取得値={4},閾値MIN={5}"; //2015.10.21修正。表示内容に管理番号追加。
                            sMessage = string.Format(sMessage, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO, plmInfo.QcParamNO, plmInfo.ParameterNM, dValue, plmInfo.ParameterMIN);
                            if (isPlaySound)
                            {
                                F01_MachineWatch.sp.PlayLooping();
                            }
                            ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                            errMessageList.Add(errMessageInfo);

                            fErr = true;
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                        }
                    }
                    else if (plmInfo.ManageNM == Constant.sMAX)
                    {
                        if (plmInfo.ParameterMAX < dValue)
                        {
                            sMessage = "[{0}/{1}号機][管理番号:{2}/{3}]が管理限界値(MAX)を越えました。\r\n取得値={4},閾値MAX={5}"; //2015.10.21修正。表示内容に管理番号追加。
                            sMessage = string.Format(sMessage, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO, plmInfo.QcParamNO, plmInfo.ParameterNM, dValue, plmInfo.ParameterMAX);
                            if (isPlaySound)
                            {
                                F01_MachineWatch.sp.PlayLooping();
                            }
                            ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                            errMessageList.Add(errMessageInfo);

                            fErr = true;
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                        }
                    }
                    log.DParameterVAL = dValue;
                    log.SParameterVAL = null;
                }
            }
            else//1:文字列か、管理限界情報を取得出来ていない場合
            {
                if (plmInfo.ParameterVAL != sValue && plmInfo.ParameterVAL != Constant.sOKStrings)
                {
                    sMessage = "[{0}/{1}号機][管理番号:{2}/{3}]の設定値に誤りがあります。\r\n取得値={4},閾値={5}"; //2015.10.21修正。表示内容に管理番号追加。
                    sMessage = string.Format(sMessage, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO, plmInfo.QcParamNO, plmInfo.ParameterNM, sValue, plmInfo.ParameterVAL);
                    //sMessage = string.Format(Constant.MessageInfo.Message_23, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO, plmInfo.ParameterNM, sValue, plmInfo.ParameterVAL);
                    if (isPlaySound)
                    {
                        F01_MachineWatch.sp.PlayLooping();
                    }
                    ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                    errMessageList.Add(errMessageInfo);

                    fErr = true;
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                }

                log.SParameterVAL = sValue;
                log.DParameterVAL = null;
            }

            log.EquipmentNO = lsetInfo.EquipmentNO;
            log.ErrorFG = fErr;
            log.LineCD = lsetInfo.InlineCD;
            log.MagazineNO = MagInfo.sMagazineNO;
            log.NascaLotNO = MagInfo.sNascaLotNO;
            log.QcParamNO = plmInfo.QcParamNO;
            log.MaterialCD = MagInfo.sMaterialCD;
            log.MeasureDT = DateTime.Parse(dtMeasureDT);
            log.MessageNM = sMessage;
            log.UpdUserCD = "660";
            log.CheckFG = false;
            log.SeqNO = 1;

            logList.Add(log);

            if (fErr && SettingInfo.GetBatchModeFG() == false)
            {
                if (ErrorCommunicator.IsMustSendToOtherLineError(lsetInfo.InlineCD, plmInfo.QcParamNO))
                {
                    if (string.IsNullOrEmpty(MagInfo.sNascaLotNO) == true)
                    {
                        ErrMessageInfo errMessageInfo = new ErrMessageInfo(string.Format("【遠隔エラー通知不可】マガジンNO：{0} / パラメータ名：{1} / ロット紐付け不良の為、通知不可", MagInfo.sMagazineNO, plmInfo.ParameterNM), Color.Red);
                        errMessageList.Add(errMessageInfo);
                    }
                    else
                    {
#if DEBUG
#else
                        ErrorCommunicator.SendError(ErrorCommunicator.GetContactError(lsetInfo.InlineCD, DateTime.Now, lsetInfo, MagInfo.sNascaLotNO, MagInfo.sMagazineNO, plmInfo.QcParamNO, sMessage));
#endif
                    }
                }
            }

            return fErr;
        }

        /// <summary>
        /// 別プロセス使用中対策
        /// 別プロセスがファイルを掴んでいたら300msSleepする
        /// </summary>
        /// <param name="swfname"></param>
        /// <param name="sInputFolder"></param>
        /// <param name="sfname"></param>
        /// <param name="sFileType"></param>
        /// <param name="sWork"></param>
        /// <returns></returns>
        public static bool GetFileInfo(string swfname, string sInputFolder, ref string sfname, ref string sFileType, ref string sWork)
        {
            bool flg = true;//正常終了

            try
            {
                if (sInputFolder.EndsWith("\\") == false)
                {
                    sInputFolder += "\\";
                }
                using (System.IO.StreamReader textFile = new System.IO.StreamReader(swfname, System.Text.Encoding.Default))
                {
                    sfname = swfname.Substring(sInputFolder.Length, swfname.Length - sInputFolder.Length);      //ファイル名取得
                                                                                                                //プラズマ機のファイルフォーマットは三菱仕様
                    if (sInputFolder.Contains("PLA"))
                    {
                        sFileType = sfname.Substring(4, 2);                                                         //ファイルタイプ取得
                    }//SGA内製のファイルフォーマットはKEYENCE仕様
                    else
                    {
                        sFileType = sfname.Substring(7, 2);                                                         //ファイルタイプ取得
                    }

                    sWork = textFile.ReadToEnd();

                    textFile.Close();
                }
            }
            catch
            {
                flg = false;//異常終了(別プロセス(blackJumboDog)が掴んでいる)
                System.Threading.Thread.Sleep(300);
            }

            return flg;
        }

        public static bool GetFileInfo(string swfname, string sInputFolder, ref string sfname, ref string sWork)
        {
            string sFileType = string.Empty;
            return GetFileInfo(swfname, sInputFolder, ref sfname, ref sFileType, ref sWork);
        }

        public static MachineFileInfo GetFileInfo(string swfname, string sInputFolder)
        {
            return GetFileInfo(swfname, sInputFolder, 1);
        }

        /// <summary>
        /// GetFileInfo()の引数の参照渡し廃止版 どこかのタイミングで切換える(2013/11/15 n.yoshimto)
        /// 他のプロセスが掴んでいたらsleepする処理になっているが、そのあと関数内で再処理するようにした方が良い。今は関数の呼び出しもとでやってる
        /// </summary>
        /// <param name="swfname"></param>
        /// <param name="sInputFolder"></param>
        /// <param name="sfname"></param>
        /// <param name="sFileType"></param>
        /// <param name="sWork"></param>
        /// <returns></returns>
        public static MachineFileInfo GetFileInfo(string swfname, string sInputFolder, int callCount)
        {
            MachineFileInfo machineFileInfo = new MachineFileInfo();

            try
            {
                using (System.IO.StreamReader textFile = new System.IO.StreamReader(swfname, System.Text.Encoding.Default))
                {
                    machineFileInfo.Name = swfname.Substring(sInputFolder.Length, swfname.Length - sInputFolder.Length);      //ファイル名取得

                    machineFileInfo.Content = textFile.ReadToEnd();

                    textFile.Close();
                }
            }
            catch
            {
                System.Threading.Thread.Sleep(300);

                if (callCount <= 5)
                {
                    return GetFileInfo(swfname, sInputFolder, callCount++);
                }

                string sMessage = Convert.ToString(callCount) + "回 GetFileInfoで失敗がありました。";
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
            }

            return machineFileInfo;
        }


        /// <summary>
        /// タイプとLotを入れると、外観検査機入った(1)、入っていない(2)、ｴﾗｰ(-1)を教えてくれる
        /// </summary>
        /// <param name="plantCD"></param>
        /// <param name="start2EndDt"></param>
        /// <returns></returns>
        public int GetLotInfo(string sLotNo, int procno)
        {
            int nret = -1;  //-1=ｴﾗｰ
                            //1=マッピング必要
                            //2=マッピング不要なので外観検査機のメモリを0クリア

            string sMessage = "";
            //int nCD = 32;//外観検査
            try
            {
                nret = ConnectDB.GetLotCharInfo(this.LineCD, sLotNo, procno);
            }
            catch (Exception ex)
            {
                sMessage = "[インライン番号]" + this.LineCD + "\r\n" +
                            ex.Message + "☆LotNO=[" + sLotNo + "]" + "でGetLotCharInfoが落ちました。";

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                nret = -1;
            }

            // 検索条件に合う実績がなかった
            if (nret == -1)
            {
                sMessage = "Lot特性を取得出来ませんでした。" + "LotNO=[" + sLotNo + "]";
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
            }

            //外観検査機入った(1)、入っていない(2)、エラー(-1)
            return nret;
        }

        /// <summary>
        /// 数値データのNG判定のみ行う
        /// </summary>
        /// <param name="sQcParamNO"></param>
        /// <param name="sLotNO"></param>
        /// <param name="sParameterVAL"></param>
        /// <param name="dtLastUpdDT"></param>
        public static bool JudgeParam(LSETInfo lsetInfo, ParamInfo ParamInfo, Plm plmInfo, MagInfo MagInfo, string sValue, string dtMeasureDT)
        {
            string sMessage = "";
            Decimal dValue = 0;
            bool bflg = false;
            //<--Start 2010/03/04 樹脂測定値がスタート直後1F・マガジン完了後で重複したエラーが出てしまう対応
            int n1FQcParamNO = 0;
            //-->End 2010/03/04 樹脂測定値がスタート直後1F・マガジン完了後で重複したエラーが出てしまう対応

            if (sValue != "")
            {
                if (decimal.TryParse(sValue, out dValue) == false)//sValueが0に近い値の場合、失敗する為その対応
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + ParamInfo.nQcParamNO + dtMeasureDT + ":sValueの変換に失敗した為、0を格納します。");
                }
                //dValue = Convert.ToDecimal(sValue);
                dValue = decimal.Round(dValue, 4, MidpointRounding.AwayFromZero);
            }

            if (ParamInfo.sManageNM == Constant.sMAXMIN)
            {
                if (plmInfo.ParameterMAX < dValue)
                {
                    //<--Start 2010/03/04 樹脂測定値がスタート直後1F・マガジン完了後で重複したエラーが出てしまう対応
                    //1マガジン完了後はエラーであれば、90～94で重複していたらエラー判定・装置停止しない。
                    if (ParamInfo.nQcParamNO >= 95 && ParamInfo.nQcParamNO <= 99)
                    {
                        n1FQcParamNO = ParamInfo.nQcParamNO - 5;//-5するとスタート直後1FのQcParam_NOとなる
                        if (ConnectDB.CheckNG(lsetInfo, n1FQcParamNO, dtMeasureDT))
                        {
                            sMessage = "";
                            bflg = false;
                        }
                    }
                    //-->End 2010/03/04 樹脂測定値がスタート直後1F・マガジン完了後で重複したエラーが出てしまう対応

                    //xSP.Play();
                    sMessage = "[{0}/{1}号機][{2}]が管理限界値(MAX)を越えました。\r\n取得値={3},閾値MAX={4}";
                    sMessage = string.Format(sMessage, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO, ParamInfo.sParamNM, dValue, plmInfo.ParameterMAX);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                    //ShowSubForm(sMessage);

                    bflg = true;
                }
                else if (plmInfo.ParameterMIN > dValue)
                {
                    //<--Start 2010/03/04 樹脂測定値がスタート直後1F・マガジン完了後で重複したエラーが出てしまう対応
                    //1マガジン完了後はエラーであれば、90～94で重複していたらエラー判定・装置停止しない。
                    if (ParamInfo.nQcParamNO >= 95 && ParamInfo.nQcParamNO <= 99)
                    {
                        n1FQcParamNO = ParamInfo.nQcParamNO - 5;//-5するとスタート直後1FのQcParam_NOとなる
                        if (ConnectDB.CheckNG(lsetInfo, n1FQcParamNO, dtMeasureDT))
                        {
                            sMessage = "";
                            bflg = false;
                        }
                    }
                    //-->End 2010/03/04 樹脂測定値がスタート直後1F・マガジン完了後で重複したエラーが出てしまう対応

                    //xSP.Play();
                    sMessage = "[{0}/{1}号機][{2}]が管理限界値(MIN)を越えました。\r\n取得値={3},閾値MIN={4}";
                    sMessage = string.Format(sMessage, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO, ParamInfo.sParamNM, dValue, plmInfo.ParameterMIN);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                    //ShowSubForm(sMessage);
                    bflg = true;
                }
            }
            else if (ParamInfo.sManageNM == Constant.sMAX)
            {
                if (plmInfo.ParameterMAX < dValue)
                {
                    //xSP.Play();
                    sMessage = "[{0}/{1}号機][{2}]が管理限界値(MAX)を越えました。\r\n取得値={3},閾値MAX={4}";
                    sMessage = string.Format(sMessage, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO, ParamInfo.sParamNM, dValue, plmInfo.ParameterMAX);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                    //ShowSubForm(sMessage);
                    bflg = true;
                }
            }
            return bflg;
        }

        //LotNo文字列を確認して、問題あれば"Error"を返す
        public static string CheckLotNo(LSETInfo lsetInfo, string sLotNo)
        {
            string sMessage = "";
            string swLotNo = sLotNo;

            SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            swLotNo = swLotNo.Replace(" 1", "");        //余計な文字削除
            swLotNo = swLotNo.Replace(" 2", "");        //余計な文字削除
            swLotNo = swLotNo.Replace(" 3", "");        //余計な文字削除
            swLotNo = swLotNo.Replace("\r", "");        //余計な文字削除
            swLotNo = swLotNo.Replace("\"", "");        //余計な文字削除
            swLotNo = swLotNo.Replace("\0", "");        //余計な文字削除

            //高効率、アウトラインの場合
            if ((settingInfo.LineType == Constant.LineType.High.ToString()) || (settingInfo.LineType == Constant.LineType.Out.ToString()))
            {
                swLotNo = swLotNo.Replace("11 ", "").Trim();//余計な文字削除    //人搬送用
                swLotNo = swLotNo.Replace("13 ", "").Trim();
            }
            else//自動化の場合
            {
                swLotNo = swLotNo.Replace("13 ", "").Trim();//余計な文字削除  //インライン用
            }

            if (swLotNo.Length == Constant.LOT_NO_LENGTH)
            {
                return swLotNo;
            }
            else
            {
                sMessage = string.Format(Constant.MessageInfo.Message_106, lsetInfo.AssetsNM, lsetInfo.MachineNM, swLotNo, Constant.LOT_NO_LENGTH);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                throw new Exception(sMessage);
            }
        }

        //マガジン番号文字列を確認して、問題あれば"Error"を返す
        public string CheckMagNo(string sMagazineNo)
        {
            string sType = string.Empty;
            string swMagazineNo = "";

            swMagazineNo = sMagazineNo;
            swMagazineNo = swMagazineNo.Replace("\r", "");//余計な文字削除
            swMagazineNo = swMagazineNo.Replace("\"", "");//余計な文字削除
            swMagazineNo = swMagazineNo.Replace("30 ", "").Trim();//余計な文字削除

            //<--マガジン桁数判定自体不要になった為削除。湯浅/松島 2015/02/25
            return swMagazineNo;

            /*
            SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            int magazinLength = 0;
            if (settingInfo.MapFG || settingInfo.TypeGroup == Constant.TypeGroup.Map.ToString())
            {
                magazinLength = (int)Constant.MagazineLength.Map;
                sType = Enum.GetName(typeof(Constant.MagazineLength), Constant.MagazineLength.Map);
            }
            else
            {
                magazinLength = (int)Constant.MagazineLength.Sideview;
                sType = Enum.GetName(typeof(Constant.MagazineLength), Constant.MagazineLength.Sideview);
            }

            if (settingInfo.TypeGroup == Constant.TypeGroup.AutoMotive.ToString() && settingInfo.LineType == Constant.LineType.Auto.ToString())
            {
                magazinLength = (int)Constant.MagazineLength.Automotive;
                sType = Constant.TypeGroup.AutoMotive.ToString();
            }

            if (swMagazineNo.Length == magazinLength)
            {
                return swMagazineNo;
            }
            else
            {
				// マガジンNoの文字列内にDUMMYの文字を含む場合は異常無し
				if (swMagazineNo.ToUpper().Contains(Constant.DUMMY_MAG_CONTAIN_STR))
				{
					return swMagazineNo;
				}
                throw new Exception(string.Format(Constant.MessageInfo.Message_107, this.LineCD, this.Code, sMagazineNo, sType, magazinLength));
            }
            */
            //-->マガジン桁数判定自体不要になった為削除。湯浅/松島 2015/02/25

        }


        public static ArmsLotInfo GetLotNo_Mag(int lineCD, string magazineNO)
        {
            string sMessage = "";
            ArmsLotInfo rtnArmsLotInfo = null;

            //API1 確認OK
            try
            {
                rtnArmsLotInfo = ConnectDB.GetARMSLotData(lineCD, "", magazineNO, "")[0];//Lot,startdt,enddt取得
                                                                                         //rtnArmsLotInfo.Type = armsinfo.GetLotType(rtnArmsLotInfo.Lot);
            }
            catch (Exception ex)
            {
                sMessage = "[インライン番号]" + lineCD + "\r\n" +
                            ex.Message + "☆マガジンNo=[" + magazineNO + "]でGetARMSLotDataが落ちました。";

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                rtnArmsLotInfo = null;
            }

            if (rtnArmsLotInfo == null)
            {
                sMessage = "流動中でないかロット情報がありません。" + magazineNO;
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
            }

            // マガジンロット情報取得
            return rtnArmsLotInfo;
        }

        public ArmsLotInfo GetLotNo_Mag(string magazineNO, string targetTime)
        {
            string sMessage = "";
            ArmsLotInfo rtnArmsLotInfo = null;

            //API1 確認OK
            try
            {
                rtnArmsLotInfo = ConnectDB.GetARMSLotData(this.LineCD, "", magazineNO, targetTime)[0];//Lot,startdt,enddt取得
                                                                                                      //rtnArmsLotInfo.Type = armsinfo.GetLotType(rtnArmsLotInfo.Lot);
            }
            catch (Exception ex)
            {
                sMessage = "[インライン番号]" + this.LineCD + "\r\n" +
                            ex.Message + "☆マガジンNo=[" + magazineNO + "]でGetARMSLotDataが落ちました。";

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                rtnArmsLotInfo = null;
            }

            if (rtnArmsLotInfo == null)
            {
                sMessage = "流動中でないかロット情報がありません。" + magazineNO;
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
            }

            // マガジンロット情報取得
            return rtnArmsLotInfo;
        }


        public MagInfo SetMagInfo(MagInfo wMagInfo, string sEquipmentNO, string sLotNo)
        {
            SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            if (sLotNo == "Error")
            {
                wMagInfo.sMagazineNO = null;
                wMagInfo.sNascaLotNO = null;
                wMagInfo.sMaterialCD = settingInfo.GetMaterialCD(sEquipmentNO);
            }
            else
            {
                wMagInfo.sMagazineNO = null;
                wMagInfo.sNascaLotNO = sLotNo;
                //<--Package 古川さん待ち
                //wMagInfo.sMaterialCD = GetLotType(sLotNo);
                //#if Debug
                //                wMagInfo.sMaterialCD = "NSSW156A2-100";
                //#else
                wMagInfo.sMaterialCD = ConnectDB.GetARMSLotType(this.LineCD, sLotNo);
                //#endif
                //-->Package 古川さん待ち
            }
            return wMagInfo;
        }


        public void CheckQC(LSETInfo lsetInfo, int timingNO, string sType)
        {
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"[CheckQC開始] 設備CD:{lsetInfo.EquipmentNO} 製品型番:{sType} TimingNo:{timingNO}");

            SortedList<int, InspData> SLInspection = new SortedList<int, InspData>();

            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            //処理方法と管理項目のリスト取得
            if (settingInfoPerLine.MapFG || settingInfoPerLine.TypeGroup == Constant.TypeGroup.Map.ToString() || settingInfoPerLine.NotUseTmQDIWFG)
            {
                SLInspection = ConnectDB.GetMAPInspectionList(timingNO, lsetInfo.InlineCD);
            }
            else
            {
                SLInspection = ConnectDB.GetInspectionList(timingNO, lsetInfo.InlineCD);
            }

            //List<SortedList<int, QCLogData>> cndData = new List<SortedList<int, QCLogData>>();

            //NASCA情報のX軸(LotNo)を書く為に、必要
            List<string> ListLotNo = new List<string>();

            string[] textArray = new string[] { };
           
            //単数号機監視 必須(Multi_NOにかかわらず全て)-------------------------------------------------------------------------------
            for (int i = 0; i < SLInspection.Count; i++)
            {
                SortedList<int, QCLogData> cndDataItem = new SortedList<int, QCLogData>();

#if Debug
                //if (SLInspection[i].InspectionNO != 7236)
                //{
                //    continue;
                //}
#endif

                //装置情報連携システムのデータを使用して傾向管理
                //if (SLInspection[i].Number[0] < 10000 ||
                //	((SLInspection[i].Number[0] >= 200000) && (SLInspection[i].Number[0] < 300000)))
                if (SLInspection[i].ParamInfo[0].UnManageTrendFG == false)
                {
#if Debug
                    //if (SLInspection[i].Number[0] == 179)
                    //{
                    //    return;
                    //}
#endif
                    for (int j = 0; j < SLInspection[i].ParamInfo.Count; j++)
                    {
                        //現在時間から設定個数分の管理番号データを取得する
                        cndDataItem = ConnectDB.GetQCItem(lsetInfo, SLInspection[i], sType, 0, SLInspection[i].ParamInfo[j].No);//ここは0でOK!
                        Notifier n = new Notifier(cndDataItem, 0, lsetInfo.InlineCD);
                        n.Notify();
                    }
                }
                else//NASCAデータを使用して傾向管理(AIはここには入らない)
                {
                    ListLotNo = ConnectDB.GetLotList(lsetInfo, timingNO, SLInspection[i], sType, 0);//ここは0でOK!
                                                                                                    //<--Package 2011/03/17
                    if (ListLotNo.Count > 0)
                    {
                        for (int j = 0; j < SLInspection[i].ParamInfo.Count; j++)
                        {
                            //cndDataItem = GetQCItem_NASCA(nLineCD, EquiInfo, SLInspection[i], sType, 0, ListLotNo, SLInspection[i].Number[j]);//ここは0でOK!
                            cndDataItem = ConnectDB.GetQCItem(lsetInfo, SLInspection[i], sType, 0, SLInspection[i].ParamInfo[j].No, ListLotNo);//ここは0でOK!
                            Notifier n = new Notifier(cndDataItem, 0, lsetInfo.InlineCD);//ここは0でOK!
                            n.Notify();
                        }
                    }
                    //-->Package 2011/03/17
                }
            }

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"[CheckQC完了] 設備CD:{lsetInfo.EquipmentNO} 製品型番:{sType} TimingNo:{timingNO}");
        }

        /// <summary>
        /// 算出情報を基にファイル値を計算
        /// </summary>
        /// <returns></returns>
        public double CalcChangeUnit(int qcParamNO, double targetVAL)
        {
            double rChangeUnitVAL = 0;

            string unitVAL = Convert.ToString(ConnectDB.GetPRMElement("ChangeUnit_VAL", qcParamNO, this.LineCD));
            if (unitVAL == "" || unitVAL == "0" || unitVAL == "1")
            {
                return targetVAL;
            }

            int calcVAL = Convert.ToInt32(unitVAL.Substring(1, unitVAL.Length - 1));

            if (unitVAL.Substring(0, 1) == "/")
            {
                rChangeUnitVAL = targetVAL / calcVAL;
            }
            else if (unitVAL.Substring(0, 1) == "*")
            {
                rChangeUnitVAL = targetVAL * calcVAL;
            }
            else
            {
                throw new Exception(string.Format(Constant.MessageInfo.Message_122, this.LineCD, qcParamNO, unitVAL));
            }

            return rChangeUnitVAL;
        }

        /// <summary>
        /// 算出情報を基にファイル値を計算(データベースからの算出情報取得は無し)
        /// </summary>
        /// <returns></returns>
        public double CalcChangeUnit(string unitVAL, double targetVAL)
        {
            double rChangeUnitVAL = 0;

            if (unitVAL == "" || unitVAL == "0" || unitVAL == "1")
            {
                return targetVAL;
            }

            int calcVAL = Convert.ToInt32(unitVAL.Substring(1, unitVAL.Length - 1));

            if (unitVAL.Substring(0, 1) == "/")
            {
                rChangeUnitVAL = targetVAL / calcVAL;
            }
            else if (unitVAL.Substring(0, 1) == "*")
            {
                rChangeUnitVAL = targetVAL * calcVAL;
            }
            else
            {
                throw new Exception($"ChangeUnitValueのマスタ設定値が不正です。ライン：{this.LineCD} ChangeUnitVal：{unitVAL}");
            }

            return rChangeUnitVAL;
        }

        /// <summary>
		/// 最大値を計算
		/// </summary>
		/// <param name="wList">値リスト</param>
		/// <returns>最大値</returns>
		public static double CalcMax(List<double> wList)
        {
            double wdrtn = double.MinValue;

            foreach (double wd in wList)
            {
                if (wdrtn < wd)
                {
                    wdrtn = wd;
                }
            }
            return wdrtn;
        }

        /// <summary>
        /// 最小値を計算
        /// </summary>
        /// <param name="wList">値リスト</param>
        /// <returns>最小値</returns>
        public static double CalcMin(List<double> wList)
        {
            double wdrtn = double.MaxValue;

            foreach (double wd in wList)
            {
                if (wdrtn > wd)
                {
                    wdrtn = wd;
                }
            }
            return wdrtn;
        }

        /// <summary>
        /// 合計値を計算
        /// </summary>
        /// <param name="wList">値リスト</param>
        /// <returns>合計値</returns>
        public static double CalcSum(List<double> wList)
        {
            double wdrtn = double.MinValue;

            foreach (double wb in wList)
            {
                wdrtn = wdrtn + wb;
            }

            return wdrtn;
        }

        /// <summary>
        /// 最頻値を計算
        /// </summary>
        /// <param name="wList">値リスト</param>
        /// <returns>最頻値</returns>
        public static double CalcMode(List<double> wList)
        {
            Dictionary<double, int> groupList = new Dictionary<double, int>();
            foreach (double wValue in wList)
            {
                if (groupList.ContainsKey(wValue))
                {
                    groupList[wValue] += 1;
                }
                else
                {
                    groupList.Add(wValue, 1);
                }
            }

            double maxCount = groupList.Max(g => g.Value);
            List<KeyValuePair<double, int>> maxCountList = groupList.Where(g => g.Value == maxCount).ToList();

            double rValue = 0;
            foreach (KeyValuePair<double, int> maxCountValue in maxCountList)
            {
                rValue += maxCountValue.Key;
            }
            return rValue / maxCountList.Count;
        }

        /// <summary>
        /// σ値を計算(ExcelのStDevAと計算方法は同じ)
        /// </summary>
        /// <param name="wList">値リスト</param>
        /// <param name="avg">平均値</param>
        /// <returns>σ値</returns>
        public static double calcSigma(List<double> wList, double avg)
        {
            double xsum = 0;
            double x2sum = 0;
            double dCnt = wList.Count;
            //  念のためのエラーチェック
            if (dCnt == 0) return 0.0;

            foreach (double wd in wList)
            {
                xsum += wd;
                x2sum += wd * wd;
            }

            //  Excel STDEVA()の定義
            double sigma = Math.Sqrt((dCnt * x2sum - (xsum * xsum)) / (dCnt * (dCnt - 1)));

            return sigma;
        }

        /// <summary>
        /// 平均値を計算
        /// </summary>
        /// <param name="wList">値リスト</param>
        /// <returns>平均値</returns>
        public static double calcAvg(List<double> wList)
        {
            double retv = 0.0;
            double dCnt = wList.Count;
            //  念のためのエラーチェック
            if (dCnt == 0) return 0.0;

            foreach (double wd in wList)
            {
                retv += wd;
            }

            retv = retv / dCnt;

            return retv;
        }

        /// <summary>
        /// 数値をアルファベット文字に変換
        /// </summary>
        /// <param name="number">数値</param>
        /// <returns>アルファベット文字</returns>
        public static string ConvertNumberToAlphabet(int number)
        {
            //定義　1=A,2=B,...

            char c = 'A';
            c = (char)((int)c + number - 1);
            return c.ToString();

            //int charNO = number + 64;   //65=A
            //return Convert.ToChar(charNO).ToString();
        }

        public static string GetQRStrFromFile(string[] fileLineValueList, int dataStartRow, int targetCol)
        {
            string targetValue = string.Empty;

            //BTS.2147にてマガジンNOを最終行の方から取得するように改修。
            //ファイル最終行からデータ開始行（非ヘッダ行）までマガジンNoが見つかるまでサーチ
            for (int targetMagRow = fileLineValueList.Count() - 1; targetMagRow >= dataStartRow; targetMagRow--)
            {
                if (string.IsNullOrEmpty(fileLineValueList[targetMagRow]))
                {//空白行の場合、手前の行をサーチする
                    continue;
                }

                string[] fileLineValue = fileLineValueList[targetMagRow].Split(',');
                targetValue = fileLineValue[targetCol].Trim();

                if (string.IsNullOrEmpty(targetValue))
                {//マガジン・ロットNo列がnullか空白なら手前の行をサーチする
                    continue;
                }
                else
                {//何らかの文字列が入っていた場合、サーチ終了
                    break;
                }
            }

            return targetValue;
        }

        public static DateTime GetDateTimeFromFile(string[] fileLineValueList, int dataStartRow, int dateCol, int timeCol)
        {
            string targetValue = string.Empty;

            for (int targetMagRow = fileLineValueList.Count() - 1; targetMagRow >= dataStartRow; targetMagRow--)
            {
                if (string.IsNullOrEmpty(fileLineValueList[targetMagRow]))
                {//空白行の場合、手前の行をサーチする
                    continue;
                }

                string[] fileLineValue = fileLineValueList[targetMagRow].Split(',');
                targetValue = fileLineValue[dateCol] + " " + fileLineValue[timeCol];

                if (string.IsNullOrEmpty(targetValue))
                {//マガジン・ロットNo列がnullか空白なら手前の行をサーチする
                    continue;
                }
                else
                {//何らかの文字列が入っていた場合、サーチ終了
                    break;
                }
            }

            DateTime targetDt = new DateTime();

            if (DateTime.TryParseExact(targetValue, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.AllowInnerWhite, out targetDt) == false)
            {
                throw new ApplicationException(string.Format("文字列を日時型に変換出来ません。変換対象:{0} ﾃﾞｰﾀ開始行:{1} 日付列:{2} 時間列:{3}", targetValue, dataStartRow, dateCol, timeCol));
            }

            return targetDt;
        }

        public int GetFileColLength(string rowData, char delimiter)
        {
            string[] colArray = rowData.Split(delimiter);

            return colArray.Length;
        }


        public static MagInfo GetMagazineInfo(LSETInfo lsetInfo, string[] fileLineValueList, int dataStartRow, int targetCol)
        {
            return GetMagazineInfo(lsetInfo, fileLineValueList, dataStartRow, targetCol, false, 0, 0);
        }

        /// <summary>
        /// マガジン情報を取得(ログファイル)
        /// </summary>
        /// <param name="lsetInfo">設備情報</param>
        /// <param name="fileLineValueList">ファイル内容</param>
        /// <param name="targetRow">ファイル内容対象行</param>
        /// <param name="targetCol">ファイル内容対象列</param>
        /// <param name="fromNascaFg">実績情報をNascaから取得するかどうか(true:Nascaから取得する)</param>
        /// <returns>マガジン情報</returns>
        public static MagInfo GetMagazineInfo(LSETInfo lsetInfo, string[] fileLineValueList, int dataStartRow, int targetCol, bool fromNascaFg, int dateCol, int timeCol)
        {
            MagInfo retv = new MagInfo();

            SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            //GetLotNo_Mag(targetValue, string.Format("{0} {1}", fileLineValue[targetDateCol].Trim(), fileLineValue[targetTimeCol].Trim()

            string qrValue = GetQRStrFromFile(fileLineValueList, dataStartRow, targetCol);
            if (string.IsNullOrEmpty(qrValue))
            {
                throw new Exception(string.Format("マガジン番号の形式が正しくありません 読込番号:{0}", qrValue));
            }
            string[] qrValueChars = qrValue.Replace("\r", "").Replace("\"", "").Replace("\0", "").Split(' ');
            if (qrValueChars == null || string.IsNullOrEmpty(qrValueChars[0]))
            {
                throw new Exception(string.Format("マガジン番号の形式が正しくありません 読込番号:{0}", qrValue));
            }

            string magazineNo = "";
            if (qrValueChars.Count() == 1)
            {
                magazineNo = qrValueChars[0];
            }
            else
            {
                magazineNo = qrValueChars[1];
            }

            //ロット情報を取得する際に、NASCAとARMSからを切り替えれるようにする 2015/8/19 nyoshimoto
            if (fromNascaFg)
            {
                DateTime targetDt = GetDateTimeFromFile(fileLineValueList, dataStartRow, dateCol, timeCol);

                retv = GetMagazineInfo(lsetInfo.InlineCD, lsetInfo.EquipmentNO, magazineNo, targetDt.ToString("yyyy/MM/dd HH:mm:ss"));
                //アウト(ロットNO)
                if ((qrValueChars[0] == "11" || qrValueChars[0] == "13") && string.IsNullOrEmpty(retv.sNascaLotNO))
                {
                    retv.sNascaLotNO = CheckLotNo(lsetInfo, magazineNo);
                }

                if (string.IsNullOrEmpty(retv.sMagazineNO))
                {
                    retv.sMaterialCD = ConnectDB.GetTypeCD(lsetInfo.InlineCD, retv.sNascaLotNO);
                }
            }
            else
            {
                ArmsLotInfo lot = GetLotNo_Mag(lsetInfo.InlineCD, magazineNo);
                if (lot == null)
                {
                    retv.sMagazineNO = magazineNo;
                    if ((qrValueChars[0] == "13") || (qrValueChars[0] == "11"))
                    {
                        retv.sNascaLotNO = magazineNo;

                        Arms.AsmLot armsLot = Arms.AsmLot.GetAsmLot(lsetInfo.InlineCD, retv.sNascaLotNO);
                        retv.sMaterialCD = armsLot.TypeCd;
                    }
                    else
                    {
                        retv.sNascaLotNO = null;
                        retv.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);
                    }
                }
                else
                {
                    retv.sMagazineNO = magazineNo;
                    retv.sNascaLotNO = lot.LotNO;
                    retv.sMaterialCD = lot.TypeCD;
                    retv.ProcNO = lot.ProcNO;

                    if (retv.MeasureDT == null)
                    {
                        DateTime dt;
                        if (DateTime.TryParseExact(lot.StartDT, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dt) == false)
                        {
                            retv.MeasureDT = dt;
                        }
                    }
                }
            }

            #region commentout
            //string targetValue = string.Empty;

            //string qrValue = GetQRStrFromFile(fileLineValueList, dataStartRow, targetCol);

            //if (settingInfo.LineType == Constant.LineType.High.ToString())
            //{
            //    //高効率(ロットNO)
            //    targetValue = CheckLotNo(lsetInfo, qrValue);

            //    magInfo = SetMagInfo(magInfo, lsetInfo.EquipmentNO, targetValue);
            //    magInfo.sMagazineNO = magInfo.sNascaLotNO;
            //}
            //else if (settingInfo.LineType == Constant.LineType.Out.ToString())
            //{
            //    //if (settingInfo.TypeGroup == Constant.TypeGroup.AutoMotive.ToString())
            //    //{
            //    targetValue = CheckLotNo(lsetInfo, qrValue);

            //    magInfo.sNascaLotNO = targetValue;
            //    magInfo.sMaterialCD = ConnectDB.GetTypeCD(lsetInfo.InlineCD, targetValue);					

            //    //}
            //    //else
            //    //{
            //    //    targetValue = CheckMagNo(qrValue);

            //    //    ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(targetValue);
            //    //    if (rtnArmsLotInfo == null)
            //    //    {
            //    //        magInfo = GetMagazineInfo(lsetInfo.EquipmentNO, DateTime.Now.ToString());
            //    //        magInfo.sMagazineNO = targetValue;
            //    //        //magInfo.sNascaLotNO = null;
            //    //        magInfo.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);
            //    //    }
            //    //    else
            //    //    {
            //    //        magInfo.sMagazineNO = targetValue;
            //    //        magInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;
            //    //        magInfo.sMaterialCD = rtnArmsLotInfo.TypeCD;
            //    //    }
            //    //}
            //    //アウト(ロットNO)
            //    //targetValue = CheckLotNo(lsetInfo, targetValue);

            //    //magInfo.sNascaLotNO = targetValue;
            //    //magInfo.sMaterialCD = ConnectDB.GetTypeCD(this.LineCD, targetValue);
            //}
            //else
            //{
            //    //自動化 or ハイブリッド(マガジンNO)
            //    targetValue = CheckMagNo(qrValue);

            //    ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(targetValue);

            //    if (rtnArmsLotInfo == null)
            //    {
            //        magInfo.sMagazineNO = targetValue;
            //        magInfo.sNascaLotNO = null;
            //        magInfo.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);
            //    }
            //    else
            //    {
            //        magInfo.sMagazineNO = targetValue;
            //        magInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;
            //        magInfo.sMaterialCD = rtnArmsLotInfo.TypeCD;
            //        magInfo.ProcNO = rtnArmsLotInfo.ProcNO;
            //    }
            //}
            #endregion

            if (string.IsNullOrEmpty(retv.sMaterialCD) == false)
            {
                Database.LENS.Magazine magConfig = Database.LENS.Magazine.GetData(retv.sMaterialCD, lsetInfo.InlineCD);

                if (magConfig == null)
                {
                    throw new ApplicationException(string.Format(
                        "LENSのﾀｲﾌﾟﾏｽﾀ、ﾏｶﾞｼﾞﾝ構成ﾏｽﾀから情報取得出来ませんでした。ﾏｽﾀ担当者へ連絡して下さい。ﾀｲﾌﾟ:{0}", retv.sMaterialCD));
                }

                retv.FrameXPackageCT = magConfig.PackageQtyX;
                retv.FrameYPackageCT = magConfig.PackageQtyY;
                retv.FramePackageCT = magConfig.TotalFramePackage;
                retv.MagazineMaxStepCT = magConfig.StepNum;
                retv.MagazineStepCT = magConfig.StepNum;
                retv.MagazinePackageCT = magConfig.TotalMagazinePackage;
            }

            return retv;
        }

        public MagInfo GetMagazineInfo(LSETInfo lsetInfo, string qrValue)
        {
            MagInfo retv = new MagInfo();

            SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            if (string.IsNullOrEmpty(qrValue))
            {
                throw new Exception(string.Format("マガジン番号の形式が正しくありません 読込番号:{0}", qrValue));
            }
            string[] qrValueChars = qrValue.Replace("\r", "").Replace("\"", "").Replace("\0", "").Split(' ');
            if (qrValueChars == null || string.IsNullOrEmpty(qrValueChars[0]))
            {
                throw new Exception(string.Format("マガジン番号の形式が正しくありません 読込番号:{0}", qrValue));
            }

            string magazineNo = "";
            if (qrValueChars.Count() == 1)
            {
                magazineNo = qrValueChars[0];
            }
            else
            {
                magazineNo = qrValueChars[1];
            }

            ArmsLotInfo lot = GetLotNo_Mag(lsetInfo.InlineCD, magazineNo);
            if (lot == null)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "マガジンNoからロット情報取得");
                retv.sMagazineNO = magazineNo;
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マガジンNoの識別子判定： 識別子={0}", qrValueChars[0]));
                if ((qrValueChars[0] == "13") || (qrValueChars[0] == "11"))
                {
                    retv.sNascaLotNO = magazineNo;

                    Arms.AsmLot armsLot = Arms.AsmLot.GetAsmLot(lsetInfo.InlineCD, retv.sNascaLotNO);
                    retv.sMaterialCD = armsLot.TypeCd;
                }
                else
                {
                    retv.sNascaLotNO = null;
                    retv.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);
                }
            }
            else
            {
                retv.sMagazineNO = magazineNo;
                retv.sNascaLotNO = lot.LotNO;
                retv.sMaterialCD = lot.TypeCD;
                retv.ProcNO = lot.ProcNO;
            }

            if (string.IsNullOrEmpty(retv.sMaterialCD) == false)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "パッケージ数情報取得");
                Database.LENS.Magazine magConfig = Database.LENS.Magazine.GetData(retv.sMaterialCD, this.LineCD);

                retv.FrameXPackageCT = magConfig.PackageQtyX;
                retv.FrameYPackageCT = magConfig.PackageQtyY;
                retv.FramePackageCT = magConfig.TotalFramePackage;
                retv.MagazineMaxStepCT = magConfig.StepNum;
                retv.MagazineStepCT = magConfig.StepNum;
                retv.MagazinePackageCT = magConfig.TotalMagazinePackage;
            }

            return retv;
        }

        #region 削除
        ///// <summary>
        ///// マガジン情報を取得(ログファイル)
        ///// </summary>
        ///// <param name="lsetInfo">設備情報</param>
        ///// <param name="fileLineValueList">ファイル内容</param>
        ///// <param name="targetRow">ファイル内容対象行</param>
        ///// <param name="targetCol">ファイル内容対象列</param>
        ///// <returns>マガジン情報</returns>
        //public MagInfo GetMagazineInfo(LSETInfo lsetInfo, string[] fileLineValueList, int dataStartRow, int targetCol, int targetDateCol, int targetTimeCol, ref string qrValue)
        //{
        //    MagInfo magInfo = new MagInfo();

        //    SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

        //    try
        //    {
        //        string[] fileLineValue = fileLineValueList[dataStartRow].Split(',');
        //        //string targetValue = fileLineValue[targetCol].Trim();

        //        string targetValue = string.Empty;

        //        //BTS.2147にてマガジンNOを最終行の方から取得するように改修。
        //        //ファイル最終行からデータ開始行（非ヘッダ行）までマガジンNoが見つかるまでサーチ
        //        for (int targetMagRow = fileLineValueList.Count()-1; targetMagRow >= dataStartRow; targetMagRow--)
        //        {
        //            if (string.IsNullOrEmpty(fileLineValueList[targetMagRow]))
        //            {//空白行の場合、手前の行をサーチする
        //                continue;
        //            }

        //            fileLineValue = fileLineValueList[targetMagRow].Split(',');
        //            targetValue = fileLineValue[targetCol].Trim();

        //            if (string.IsNullOrEmpty(targetValue))
        //            {//マガジン・ロットNo列がnullか空白なら手前の行をサーチする
        //                continue;
        //            }
        //            else
        //            {//何らかの文字列が入っていた場合、サーチ終了
        //                break;
        //            }
        //        }

        //        qrValue = targetValue;


        //        if (settingInfo.LineType == Constant.LineType.High.ToString())
        //        {
        //            //高効率(ロットNO)
        //            targetValue = CheckLotNo(lsetInfo, targetValue);

        //            magInfo = SetMagInfo(magInfo, lsetInfo.EquipmentNO, targetValue);
        //        }
        //        else if (settingInfo.LineType == Constant.LineType.Out.ToString())
        //        {
        //            //アウト(ロットNO)
        //            targetValue = CheckLotNo(lsetInfo, targetValue);

        //            magInfo.sNascaLotNO = targetValue;
        //            magInfo.sMaterialCD = ConnectDB.GetTypeCD(this.LineCD, targetValue);
        //        }
        //        else
        //        {
        //            //自動化 or ハイブリッド(マガジンNO)
        //            targetValue = CheckMagNo(targetValue);

        //            ArmsLotInfo rtnArmsLotInfo = GetLotNo_Mag(targetValue, string.Format("{0} {1}", fileLineValue[targetDateCol].Trim(), fileLineValue[targetTimeCol].Trim()));

        //            if (rtnArmsLotInfo == null)
        //            {
        //                magInfo.sMagazineNO = targetValue;
        //                magInfo.sNascaLotNO = null;
        //                magInfo.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);
        //            }
        //            else
        //            {
        //                magInfo.sMagazineNO = targetValue;
        //                magInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;
        //                magInfo.sMaterialCD = rtnArmsLotInfo.TypeCD;
        //            }
        //        }

        //        //FRAMEInfo frameInfo = ConnectDB.GetFRAMEData(magInfo.sMaterialCD, this.LineCD);

        //        //magInfo.FrameXPackageCT = frameInfo.XPackageCT;
        //        //magInfo.FrameYPackageCT = frameInfo.YPackageCT;
        //        //magInfo.FramePackageCT = frameInfo.FramePackageCT;
        //        //magInfo.MagazineMaxStepCT = frameInfo.MagazineStepMAXCT;
        //        //magInfo.MagazineStepCT = frameInfo.MagazineStepCT;
        //        //magInfo.MagazinePackageCT = frameInfo.MagazinPackageCT;

        //        Database.LENS.Magazine magConfig = Database.LENS.Magazine.GetData(magInfo.sMaterialCD, this.LineCD);

        //        magInfo.FrameXPackageCT = magConfig.PackageQtyX;
        //        magInfo.FrameYPackageCT = magConfig.PackageQtyY;
        //        magInfo.FramePackageCT = magConfig.TotalFramePackage;
        //        magInfo.MagazineMaxStepCT = magConfig.StepNum;
        //        magInfo.MagazineStepCT = magConfig.StepNum;
        //        magInfo.MagazinePackageCT = magConfig.TotalMagazinePackage;


        //        //#if Debug
        //        //                magInfo.sNascaLotNO = "N12CAE00700";
        //        //#endif

        //        return magInfo;
        //    }
        //    catch (Exception err)
        //    {
        //        throw;
        //    }
        //}
        #endregion

        public MagInfo GetMagazineInfo(int lineCD, string equipmentNO, string updateDT)
        {
            return GetMagazineInfo(lineCD, equipmentNO, null, updateDT);
        }

        /// <summary>
        /// マガジン情報を取得(ARMS or NASCA)
        /// </summary>
        /// <param name="equipmentNO"></param>
        /// <param name="updateDT"></param>
        /// <returns></returns>
        public static MagInfo GetMagazineInfo(int lineCD, string equipmentNO, string magazineNO, string updateDT)
        {
            MagInfo magInfo = new MagInfo();

            if (string.IsNullOrEmpty(magazineNO))
            {
                magInfo.sMagazineNO = string.Empty;
            }
            else
            {
                magInfo.sMagazineNO = magazineNO;
            }

            magInfo.MeasureDT = Convert.ToDateTime(updateDT);

            SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(lineCD);

            if (settingInfo.LineType == Constant.LineType.Out.ToString())
            {
                NppResultsFacilityInfo[] nascaLotInfo = ConnectDB.GetLotNo_EquiTimeNasca(lineCD, equipmentNO, updateDT);
                if (nascaLotInfo == null || nascaLotInfo.Length == 0)
                {
                    magInfo.sNascaLotNO = null;
                    magInfo.sMaterialCD = settingInfo.GetMaterialCD(equipmentNO);
                }
                else
                {
                    ////開始時間が早い方のindexを採用する
                    //int key = KLinkInfo.GetEarlyLotIndex(rtnArmsLotInfo);
                    magInfo.sNascaLotNO = nascaLotInfo[0].LotNo;  //最初のLot
                    magInfo.sMaterialCD = nascaLotInfo[0].TypeCd; //最初のLot
                }
            }
            else
            {
                List<ArmsLotInfo> rtnArmsLotInfo = ConnectDB.GetLotNo_EquiTime(lineCD, equipmentNO, updateDT);
                if (rtnArmsLotInfo == null || rtnArmsLotInfo.Count == 0)
                {
                    magInfo.sNascaLotNO = null;
                    magInfo.sMaterialCD = settingInfo.GetMaterialCD(equipmentNO);
                }
                else//Lotが複数Hitした場合
                {
                    //開始時間が早い方のindexを採用する
                    int key = KLinkInfo.GetEarlyLotIndex(rtnArmsLotInfo);

                    magInfo.sNascaLotNO = rtnArmsLotInfo[key].LotNO;  //最初のLot
                    magInfo.sMaterialCD = rtnArmsLotInfo[key].TypeCD; //最初のLot
                }
            }

            return magInfo;
        }

        /// <summary>
        /// マガジン情報を取得(ARMS) ロット情報が一件に絞れる場合のみ情報取得
        /// </summary>
        /// <param name="equipmentNO"></param>
        /// <param name="updateDT"></param>
        /// <returns></returns>
        public MagInfo GetMagazineSingleInfo(string equipmentNO, string magazineNO, string updateDT, bool isStartedAndUnCompLot)
        {
            MagInfo magInfo = new MagInfo();

            magInfo.MeasureDT = Convert.ToDateTime(updateDT);

            SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            if (settingInfo.LineType == Constant.LineType.Out.ToString())
            {
                throw new Exception(string.Format(Constant.MessageInfo.Message_132));
            }
            else
            {
                List<ArmsLotInfo> rtnArmsLotInfo = ConnectDB.GetLotNoSingleInfo(LineCD, equipmentNO, magazineNO, isStartedAndUnCompLot);
                if (rtnArmsLotInfo == null || rtnArmsLotInfo.Count == 0)
                {
                    throw new Exception(string.Format(Constant.MessageInfo.Message_130, LineCD, equipmentNO, magazineNO, isStartedAndUnCompLot));
                }
                else if (rtnArmsLotInfo.Count > 1)//Lotが複数Hitした場合
                {
                    throw new Exception(string.Format(Constant.MessageInfo.Message_131, LineCD, equipmentNO, magazineNO, isStartedAndUnCompLot));
                }
                else//ロット情報が１件に絞れるときのみ、ロット情報を取得する。
                {
                    magInfo.sMagazineNO = rtnArmsLotInfo[0].InMag;
                    magInfo.sNascaLotNO = rtnArmsLotInfo[0].LotNO;
                    magInfo.sMaterialCD = rtnArmsLotInfo[0].TypeCD;
                }
            }

            return magInfo;
        }

        /// <summary>
        /// マガジン情報を取得(WB MMファイル名から取得)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static MagInfo GetMagazineInfo(string fileName)
        {
            MagInfo magInfo = new MagInfo();

            if (fileName.Substring(0, 1) == "_")
            {
                throw new Exception(Constant.MessageInfo.Message_51);
            }

            //string idstr = @"^.*_(?<lotNO>.*?)_(?<typeCD>.*?)\.CSV.*$";
            string idstr = @"^.*_(?<lotNO>.*?)_(?<typeCD>.*?)_(?<procNO>.*?)_(?<magNO>.*?)\.CSV.*$";
            System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(idstr);
            System.Text.RegularExpressions.Match m = re.Match(fileName);
            magInfo.sMaterialCD = m.Groups["typeCD"].Value;
            magInfo.sNascaLotNO = m.Groups["lotNO"].Value;
            magInfo.ProcNO = long.Parse(m.Groups["procNO"].Value);
            magInfo.sMagazineNO = m.Groups["magNO"].Value;

            return magInfo;
        }

        public void CommunicationHSMS(ref bool firstContactFG)
        {
            SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            //			lsetInfo.TypeCD = settingInfo.GetMaterialCD(this.Code);

            //SFを受信
            ReceiveMessageInfo receiveInfo = HSMS.GetReceiveMessage();

            if (HSMS.GetMustEndConnectFG())
            {
                HSMS.DisconnectHSMS();
                HSMS = null;
                throw new ApplicationException(string.Format(Constant.MessageInfo.Message_82));
            }

            if (HSMS.IsCleanLinkTestRsp(receiveInfo) == false)
            {
                HSMS.DisconnectHSMS();
                HSMS = null;
                throw new ApplicationException(string.Format("一定期間、装置からLinkTest応答がない為、通信切断"));
                //tcpコネクションを切断してHSMSも終了
            }

            if (receiveInfo == null)
            {
                if (HSMS.GetIntervalLinkTestReqFG() && HSMS.IsIntervalLinkTestRspWaitingState() == false)
                {
                    TimeSpan ts = DateTime.Now - HSMS.GetLastLinkTestRequestDT();
                    if (ts.TotalMilliseconds >= 5000)
                    {
                        HSMS.SendLinkTestRequest();
                    }
                }
                Thread.Sleep(1000);
                return;
            }

            if (receiveInfo.ReceiveSF != "LinkTest")
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備CD:{0} / 受信：{1}", lsetInfo.EquipmentNO, receiveInfo.ReceiveSF), "SECS通信ログ");
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備CD:{0} / 受信【生データ】：{1}", lsetInfo.EquipmentNO, receiveInfo.AllMessage), "SECS通信ログ");
            }

            //受信SFに対応したSF送信
            receiveInfo = HSMS.SendMessage(receiveInfo, ref firstContactFG);

            if (receiveInfo.SendSF != "LinkTest")
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備CD:{0} / 送信：{1}", lsetInfo.EquipmentNO, receiveInfo.SendSF), "SECS通信ログ");
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備CD:{0} / 送信【生データ】：{1}", lsetInfo.EquipmentNO, receiveInfo.SendAllMessage), "SECS通信ログ");
            }

            if (receiveInfo.LinkTestRequestFG)
            {
                //HSMS通信作動確認送信
                byte[] sendWordStr = HSMS.GetRequestLinkTestWord();
                HSMS.SendWord(sendWordStr);

                HSMS.SetIntervalLinkTestReqFG();
                //HSMS.StartLinkTestRequester();
            }
            else if (receiveInfo.MessageJudgeFG)
            {
                //JudgeProcess(receiveInfo);
            }
        }

        /// <summary>
        /// ログファイル処理
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="machineStatus"></param>
        /// <param name="errMessageList"></param>
        /// <param name="frameSupplyStatus"></param>
        public void CheckMachineFile(LSETInfo lsetInfo)
        {

            //List<ErrMessageInfo> errMessageList = new List<ErrMessageInfo>();
            //lsetInfo.TypeCD = Constant.SettingInfo.GetMaterialCD(lsetInfo.EquipmentNO);

            //外部(PDA)から設定ファイルの変更があった場合、ツリーをリフレッシュ
            //ChangeTreeTextInvoke(lsetInfo.MachineNM, lsetInfo.EquipmentNO, lsetInfo.TypeCD, lsetInfo.ChipNM);

            //if (Constant.fNmc)
            //{
            //    //NMCの処理
            //    CheckMachineFileNMC(lsetInfo, machineStatus, ref errMessageList, frameSupplyStatus);
            //}
            //else
            //{
            //    //N工場や外注の処理
            //    machineStatus = CheckMachineFile(lsetInfo, ref errMessageList);
            //}

            ////エラーメッセージの表示
            //foreach (ErrMessageInfo errMessageInfo in errMessageList)
            //{
            //    string equipmentMessage = string.Format(Constant.MessageInfo.Message_42, lsetInfo.AssetsNM, lsetInfo.MachineSeqNO);
            //    if (!errMessageInfo.MessageVAL.Contains(equipmentMessage))
            //    {
            //        //設備の情報が無い場合付け足す
            //        errMessageInfo.MessageVAL = equipmentMessage + errMessageInfo.MessageVAL;
            //    }

            //    AddMessageInvoke(errMessageInfo.MessageVAL, errMessageInfo.ShowColor);
            //}

            ////通信異常時
            //if (machineStatus == Constant.MachineStatus.Stop)
            //{
            //    ChangePatLampInvoke(lsetInfo.EquipmentNO, Constant.PatLamp.Red);
            //    spMachine.Play();
            //    break;
            //}

            //Thread.Sleep(1000);
        }

        /// <summary>
        /// 各装置インスタンスを作成し返り値として返す
        /// </summary>
        /// <param name="equipmentInfo"></param>
        /// <returns></returns>
        public static MachineBase GetMachineInfo(EquipmentInfo equipmentInfo)
        {
            MachineBase machineInfo;

            LSETInfo lsetInfo = ConnectDB.GetLSETInfo(equipmentInfo.LineNO, equipmentInfo.EquipmentNO);

            switch (equipmentInfo.AssetsNM)
            {
                case Constant.ASSETS_AI_NM:
                    machineInfo = new AIMachineInfo(lsetInfo);
                    break;
                case Constant.ASSETS_AI9CAM_NM:
                    machineInfo = new AIMachineInfo9Cam(lsetInfo);
                    break;
                case Constant.ASSETS_AIJTYPE_NM:
                    machineInfo = new AIMachineInfoJType(lsetInfo);
                    break;
                case Constant.ASSETS_AINTYPE_NM:
                    machineInfo = new AIMachineInfoNType(lsetInfo);
                    break;
                case Constant.ASSETS_AIRAIM_NM:
                    machineInfo = new AIMachineInfoRAIM(lsetInfo);
                    break;
                case Constant.ASSETS_AIRAIM2_NM:
                    machineInfo = new AIMachineInfoRAIM2(lsetInfo);
                    break;
                case Constant.ASSETS_AITSAVI_NM:
                    machineInfo = new AIMachineInfoTSAVI(lsetInfo);
                    break;
                case Constant.ASSETS_BB_NM:
                    machineInfo = new BBMachineInfo(equipmentInfo.LineNO, equipmentInfo.EquipmentNO);
                    break;
                case Constant.ASSETS_BB2_NM:
                    machineInfo = new BBMachineInfo2(lsetInfo);
                    break;
                case Constant.ASSETS_BK_NM:
                    machineInfo = new BKMachineInfo();
                    break;
                case Constant.ASSETS_BC_NM:
                    machineInfo = new BCMachineInfo();
                    break;
                case Constant.ASSETS_BWM_NM:
                    machineInfo = new BWMMachineInfo();
                    break;
                case Constant.ASSETS_CO_SEMI_NM:
                    machineInfo = new COMachineInfo();
                    break;
                case Constant.ASSETS_CO_FULL_NM:
                    machineInfo = new COMachineInfo();
                    break;
                case Constant.ASSETS_CF_NM:
                    machineInfo = new CFMachineInfo();
                    break;
                case Constant.ASSETS_CR_NM:
                    machineInfo = new CRMDMachineInfo();
                    break;
                case Constant.ASSETS_CR2_NM:
                    machineInfo = new CRMDMachineInfo2();
                    break;
                case Constant.ASSETS_DB_NM:
                    machineInfo = new DBMachineInfo();
                    break;
                case Constant.ASSETS_DB_KOSAKA_KE205_NM:
                    machineInfo = new DBMachineInfoKOSAKA_KE205();
                    break;
                case Constant.ASSETS_DC_NM:
                    machineInfo = new DCMachineInfo();
                    break;
                case Constant.ASSETS_ECK_NM:
                    machineInfo = new ECKMachineInfo();
                    break;
                case Constant.ASSETS_EDP_NM:
                    machineInfo = new EDPMachineInfo();
                    break;
                case Constant.ASSETS_FC_NM:
                    machineInfo = new FCMachineInfo();
                    break;
                case Constant.ASSETS_LM_NM:
                    machineInfo = new LMMachineInfo();
                    break;
                case Constant.ASSETS_LMCHIPTYPE_NM:
                    machineInfo = new LMMachineInfoChipType();
                    break;
                case Constant.ASSETS_LS_NM:
                    machineInfo = new LSMachineInfo();
                    break;
                case Constant.ASSETS_MD_NM:
                    machineInfo = new MDMachineInfo();
                    break;
                case Constant.ASSETS_MD_YAMAMOTO_NM:
                    machineInfo = new MDMachineInfoYAMAMOTO();
                    break;
                case Constant.ASSETS_MDTDKMDM20_NM:
                    machineInfo = new MDMachineInfoTDK();
                    break;
                case Constant.ASSETS_MDTDKMDM50_NM:
                    machineInfo = new MDMachineInfoTDK();
                    break;
                case Constant.ASSETS_RP_QUSPA_NM:
                    machineInfo = new RPMachineInfoSINWA_QUSPA();
                    break;
                case Constant.ASSETS_MD_QUSPA_NM:
                    machineInfo = new MDMachineInfoSINWA_QUSPA();
                    break;
                case Constant.ASSETS_MD2_QUSPA_NM:
                    machineInfo = new MD2MachineInfoSINWA_QUSPA();
                    break;
                case Constant.ASSETS_RP_NAISEI_NM:
                    machineInfo = new RPMachineInfo();
                    break;
                case Constant.ASSETS_PLA_NM:
                    machineInfo = new PLAMachineInfo();
                    break;
                case Constant.ASSETS_PLADCOM_NM:
                    machineInfo = new PLAMachineInfoDirectlyCom(equipmentInfo.LineNO, equipmentInfo.EquipmentNO);
                    break;
                case Constant.ASSETS_WB_NM:
                    machineInfo = new WBMachineInfo(equipmentInfo.LineNO, equipmentInfo.EquipmentNO);
                    break;
                case Constant.ASSETS_SUP_NM:
                    machineInfo = new SUPMachineInfo();
                    break;
                case Constant.ASSETS_SUPTRS_NM:
                    machineInfo = new EXCMachineInfo();
                    break;
                case Constant.ASSETS_PS_NM:
                    machineInfo = new PSMachineInfoHIRANO_TMMC();
                    break;
                case Constant.ASSETS_LM_DAIRYU_NM:
                    machineInfo = new LMMachineInfoDAIRYU(lsetInfo);
                    break;
                case Constant.ASSETS_AI_KOHYOUNG_NM:
                    machineInfo = new AIMachineInfoKOHYOUNG();
                    break;
                case Constant.ASSETS_MT_YAMAHA_NM:
                    machineInfo = new MTMachineInfoYAMAHA();
                    break;
                case Constant.ASSETS_LASTMT_YAMAHA_NM:
                    machineInfo = new LastMTMachineInfoYAMAHA();
                    break;
                case Constant.ASSETS_RP_MUSASHI_NM:
                    machineInfo = new RPMachineInfoMUSASHI();
                    break;
                case Constant.ASSETS_BBSQ_NM:
                    machineInfo = new BBSQMachineInfo();
                    break;
                case Constant.ASSETS_LHA_NM:
                    machineInfo = new LHAMachineInfo();
                    break;
                case Constant.ASSETS_YB_NM:
                    machineInfo = new YBMachineInfoPFSC();
                    break;
                case Constant.ASSETS_SCREW_MD_NM:
                    machineInfo = new ScrewMDMachineInfo();
                    break;
                case Constant.ASSETS_HP_NM:
                    machineInfo = new HPMachineInfo();
                    break;
                case Constant.ASSETS_GRD_NM:
                    machineInfo = new GRDMachineInfo(lsetInfo);
                    break;
                case Constant.ASSETS_WTBT_NM:
                    machineInfo = new WTBTMachineInfo();
                    break;
                case Constant.ASSETS_IPS_NM:
                    machineInfo = new IPSMachineInfoMTS(lsetInfo);
                    break;
                case Constant.ASSETS_IPS2_NM:
                    machineInfo = new IPSMachineInfo(lsetInfo);
                    break;
                case Constant.ASSETS_CO2_NM:
                    machineInfo = new CO2MachineInfo();
                    break;
                case Constant.ASSETS_SUP2_NM:
                    machineInfo = new SUPMachineInfo2();
                    break;
                case Constant.ASSETS_LM2_NM:
                    machineInfo = new LM2MachineInfo(lsetInfo);
                    break;
                case Constant.ASSETS_ST_NM:
                    machineInfo = new STMachineInfo(lsetInfo);
                    break;
                case Constant.ASSETS_TP_NM:
                    machineInfo = new TPMachineInfo(lsetInfo);
                    break;
                //<--後工程合理化/エラー集計
                case Constant.ASSETS_ST2_NM:
                    machineInfo = new STMachineInfo2(lsetInfo);
                    break;
                case Constant.ASSETS_TP2_NM:
                    machineInfo = new TPMachineInfo2(lsetInfo);
                    break;
                //-->後工程合理化/エラー集計
                case Constant.ASSETS_DRYICECN_NM:
                    machineInfo = new DryIceCleaningMachineInfo(lsetInfo);
                    break;
                case Constant.ASSETS_DF_NM:
                    machineInfo = new DeburringMachineInfo(lsetInfo);
                    break;
                case Constant.ASSETS_COLORAUTOMEASIRER_NM:
                    machineInfo = new ColorAutoMeasurerMachineInfo(lsetInfo);
                    break;
                case Constant.ASSETS_AUTOPASTER_NM:
                    machineInfo = new AutoPasterMachineInfo(lsetInfo);
                    break;
                case Constant.ASSETS_FAR_NM:
                    machineInfo = new FARMachineInfo(lsetInfo);
                    break;
                case Constant.ASSETS_FAR2_NM:
                    machineInfo = new FAR2MachineInfo();
                    break;
                case Constant.ASSETS_SRL_NM:
                    machineInfo = new SheetRingLoader(lsetInfo);
                    break;
                case Constant.ASSETS_WC_NM:
                    machineInfo = new WaterCleaning(lsetInfo);
                    break;
                default:

                    //#if Debug
                    //#else
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_95, equipmentInfo.EquipmentNO, equipmentInfo.ModelNM));
                    //#endif
            }

            machineInfo.AssetsNM = equipmentInfo.AssetsNM;
            machineInfo.Code = equipmentInfo.EquipmentNO;
            machineInfo.Name = equipmentInfo.MachineNM;
            machineInfo.LineCD = equipmentInfo.LineNO;

            return machineInfo;
        }

        public bool IsMustStopMachine()
        {
            return StopMachineFG;
        }

        public void SetMustStopMachineFG(bool stopFlag)
        {
            StopMachineFG = stopFlag;
        }

        public virtual void SendStopSignalToMachine(LSETInfo lsetInfo, string plcMemAddr)
        {
        }

        public List<MagInfo> GetMagInfoListFromFileDT(LSETInfo lsetInfo, string sFileStampDT)
        {
            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            //ARMSからロット情報取得
            List<MagInfo> magInfoList = null;

            if (settingInfoPerLine.LineType == Constant.LineType.Out.ToString())
            {
                NppResultsFacilityInfo[] nascaLotInfoArray = ConnectDB.GetLotNo_EquiTimeNasca(this.LineCD, lsetInfo.EquipmentNO, sFileStampDT);

                if (nascaLotInfoArray == null || nascaLotInfoArray.Length == 0)
                {
                    return null;
                }
                else
                {
                    magInfoList = new List<MagInfo>();
                    foreach (NppResultsFacilityInfo nascaLotInfo in nascaLotInfoArray)
                    {
                        MagInfo magInfo = new MagInfo();
                        //開始時間が早い方のindexを採用する
                        //int key = KLinkInfo.GetEarlyLotIndex(rtnArmsLotInfo);
                        magInfo.sNascaLotNO = nascaLotInfo.LotNo;  //最初のLot
                        magInfo.sMaterialCD = nascaLotInfo.TypeCd; //最初のLot

                        if (string.IsNullOrEmpty(nascaLotInfo.StartDt) == false)
                        {
                            magInfo.StartDT = Convert.ToDateTime(nascaLotInfo.StartDt);
                        }
                        if (string.IsNullOrEmpty(nascaLotInfo.CompltDt) == false)
                        {
                            magInfo.EndDT = Convert.ToDateTime(nascaLotInfo.CompltDt);
                        }

                        magInfoList.Add(magInfo);
                    }
                }
            }
            else
            {
                List<ArmsLotInfo> rtnArmsLotInfo = ConnectDB.GetARMSLotData(lsetInfo.InlineCD, lsetInfo.EquipmentNO, "", sFileStampDT, false, true);

                if (rtnArmsLotInfo == null || rtnArmsLotInfo.Count == 0)
                {
                    return null;
                }
                else//Lotが複数Hitした場合
                {
                    magInfoList = new List<MagInfo>();
                    foreach (ArmsLotInfo armsLotInfo in rtnArmsLotInfo)
                    {
                        MagInfo magInfo = new MagInfo();
                        magInfo.sMagazineNO = armsLotInfo.OutMag;
                        magInfo.sNascaLotNO = armsLotInfo.LotNO;  //最初のLot
                        magInfo.sMaterialCD = armsLotInfo.TypeCD; //最初のLot

                        magInfo.ProcNO = armsLotInfo.ProcNO;

                        magInfo.StartDT = Convert.ToDateTime(armsLotInfo.StartDT);

                        if (string.IsNullOrEmpty(armsLotInfo.EndDT) == false)
                        {
                            magInfo.EndDT = Convert.ToDateTime(armsLotInfo.EndDT);
                        }

                        magInfoList.Add(magInfo);
                    }
                }
            }

            return magInfoList;

        }

        public MagInfo GetMagInfo(LSETInfo lsetInfo, string sFileStampDT, bool isIncludeUnCompLot)
        {
            return GetMagInfoFromFileDT(lsetInfo, sFileStampDT, 0, isIncludeUnCompLot);
        }

        public MagInfo GetMagInfoFromFileDT(LSETInfo lsetInfo, string sFileStampDT, int callFuncCT, bool isIncludeUnCompLot)
        {
            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            //ARMSからロット情報取得
            MagInfo magInfo = null;

            if (settingInfoPerLine.LineType == Constant.LineType.Out.ToString())
            {
                NppResultsFacilityInfo[] nascaLotInfo = ConnectDB.GetLotNo_EquiTimeNasca(this.LineCD, lsetInfo.EquipmentNO, sFileStampDT);
                if (nascaLotInfo == null || nascaLotInfo.Length == 0)
                {
                    //magInfo = new MagInfo();
                    //magInfo.sNascaLotNO = null;
                    //magInfo.sMaterialCD = settingInfoPerLine.GetMaterialCD(lsetInfo.EquipmentNO);
                }
                else
                {
                    magInfo = new MagInfo();
                    ////開始時間が早い方のindexを採用する
                    //int key = KLinkInfo.GetEarlyLotIndex(rtnArmsLotInfo);
                    magInfo.sNascaLotNO = nascaLotInfo[0].LotNo;  //最初のLot
                    magInfo.sMaterialCD = nascaLotInfo[0].TypeCd; //最初のLot
                }
            }
            else
            {
                List<ArmsLotInfo> rtnArmsLotInfo = ConnectDB.GetARMSLotData(lsetInfo.InlineCD, lsetInfo.EquipmentNO, "", sFileStampDT, false, isIncludeUnCompLot);

                if (rtnArmsLotInfo == null || rtnArmsLotInfo.Count == 0)
                {
                    return magInfo; //2014.7.31 n.yoshimoto ARMSからロット情報が取得できなかった場合にnullを返すようにする場合はここのコメントを解除
                }
                else//Lotが複数Hitした場合
                {
                    int key = 0;
                    if ((settingInfoPerLine.MapFG || settingInfoPerLine.TypeGroup == Constant.TypeGroup.Map.ToString()) && (settingInfoPerLine.LineType != Constant.LineType.High.ToString()))
                    {
                        //取得したLotリストの中からLotとMagazineが違うものがあれば、そのindexを採用する
                        key = KLinkInfo.GetDBLotIndex(rtnArmsLotInfo);
                        if (key == -1)
                        {
                            //マガジン、ロットNOの両フィールドにマガジンNOが入っている場合は待機。

                            if (callFuncCT < 2)
                            {
                                Thread.Sleep(300000);//5分まって、再試行する。
                                return GetMagInfoFromFileDT(lsetInfo, sFileStampDT, callFuncCT + 1, isIncludeUnCompLot);
                            }
                            string sMessage = Convert.ToString(callFuncCT) + "回 完了済み実績(条件:lotno!=magno)の取得で失敗がありました。";
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                            throw new ApplicationException(sMessage);
                        }
                    }
                    else
                    {
                        //開始時間が早い方のindexを採用する
                        key = KLinkInfo.GetEarlyLotIndex(rtnArmsLotInfo);
                    }

                    magInfo = new MagInfo();
                    magInfo.sMagazineNO = rtnArmsLotInfo[key].OutMag;
                    magInfo.sNascaLotNO = rtnArmsLotInfo[key].LotNO;  //最初のLot
                    magInfo.sMaterialCD = rtnArmsLotInfo[key].TypeCD; //最初のLot

                    magInfo.ProcNO = rtnArmsLotInfo[key].ProcNO;

                    magInfo.StartDT = Convert.ToDateTime(rtnArmsLotInfo[key].StartDT);

                    if (string.IsNullOrEmpty(rtnArmsLotInfo[key].EndDT) == false)
                    {
                        magInfo.EndDT = Convert.ToDateTime(rtnArmsLotInfo[key].EndDT);
                    }
                }
            }

            return magInfo;
        }

        public DateTime GetDTFromLogFile(string filePath, int dateCol, int timeCol)
        {
            string fileContents;

            using (System.IO.StreamReader textFile = new System.IO.StreamReader(filePath, System.Text.Encoding.Default))
            {
                fileContents = textFile.ReadToEnd();
            }
            string[] fileLineValue = fileContents.Split('\n');

            if (fileLineValue.Count() == 0)
            {
                throw new ApplicationException(string.Format(Constant.MessageInfo.Message_144, filePath));
            }

            int index = 1;//fileLineValueの末尾から空白で無いデータが得られるまで
            while (string.IsNullOrEmpty(fileLineValue[fileLineValue.Length - index].Trim()))
            {
                index++;
            }

            string[] fileColValue = fileLineValue[fileLineValue.Length - index].Split(',');

            string dateStr = fileColValue[dateCol].ToString();
            string timeStr = fileColValue[timeCol].ToString();
            DateTime dateTime;

            if (DateTime.TryParse(string.Format("{0} {1}", dateStr, timeStr), out dateTime))
            {
                return dateTime;
            }
            else
            {
                throw new ApplicationException(string.Format(Constant.MessageInfo.Message_145, filePath, dateStr, timeStr));
            }
        }

        /// <summary>
        /// runDirPathに存在するファイルのunchain移動処理
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="runDirPath"></param>
        /// <param name="targetFileType"></param>
        protected void UnchainFileProcess(LSETInfo lsetInfo, string runDirPath, FileType targetFileType)
        {
            //マガジン完了タイミングでRunフォルダに既にファイルが居る場合
            if (Directory.GetFiles(runDirPath).Length != 0)
            {
                string movedDirFromRun = string.Empty;

                //1マガジン単位ファイルのファイルスタンプ取得
                string dtFileStamp = KLinkInfo.GetFileStampDT(runDirPath, Convert.ToString(targetFileType));

                dtFileStamp = Convert.ToString(Convert.ToDateTime(dtFileStamp).AddMinutes(-5));//装置の時間として使用(ファイルスタンプ-5分)
                string sFileStamp = Convert.ToDateTime(dtFileStamp).ToString("yyyyMMddHHmmss");

                List<string> mFileList = MachineFile.GetPathList(runDirPath, ".*");
                foreach (string mFilePath in mFileList)
                {
                    movedDirFromRun = MoveCompleteMachineFile(mFilePath, lsetInfo, string.Empty, sFileStamp);
                }

                string msg = string.Format("ロットに紐付かない傾向管理ファイルがRunフォルダに存在した為、unchainフォルダへ移動しました。(パス：{0})", movedDirFromRun);

                throw new ApplicationException(msg);
            }
        }

        protected bool IsLegacyFile(LSETInfo lsetInfo, string strfileStampDt)
        {
            List<ArmsLotInfo> lotInfo = ConnectDB.GetARMSLotData(lsetInfo.InlineCD, lsetInfo.EquipmentNO, "", strfileStampDt, false, false, true);

            if (lotInfo == null)
            {
                return false;
            }

            if (lotInfo.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected List<string> GetMachineLogPathList(LSETInfo lsetInfo, ref TcpClient tcp, ref NetworkStream ns, string fgAddr, string inputFolderNM, string sTargetFile)
        {
            List<string> fileList = GetMachineLogPathList(inputFolderNM, ".*_" + sTargetFile, 0);

            if (fileList == null || fileList.Count == 0)
            {
                string sMessage = lsetInfo.AssetsNM + "/" + lsetInfo.MachineSeqNO + "号機" + "/出力ファイル=" + sTargetFile + "ファイルが装置側から中間サーバーに転送されませんでした\r\n\r\n"
                    + "BlackJumboDogが起動していない場合→直ちにBlackJumboDogを起動して下さい。\r\n"
                    + "BlackJumboDogが起動している場合  →傾向管理ファイル転送ミス\r\n"
                    + " 装置のSDカードを交換してください。\r\n\r\n"
                    + " 設備保全担当者へ連絡してください。";

                AlertLog alertLog = AlertLog.GetInstance();
                alertLog.logMessageQue.Enqueue(sMessage);

                //ファイル取得要求のクリア
                ClearFileGetReqFg(lsetInfo, ref tcp, ref ns, fgAddr);
                throw new ApplicationException(sMessage);
            }

            return fileList;
        }

        protected List<string> GetMachineLogPathList(string inputFolderNM, string sTargetFile, int retryCt)
        {
            List<string> fileList = Common.GetFiles(inputFolderNM, sTargetFile + ".*");

            if (fileList.Count <= 0 && retryCt < 3)
            {
                SettingInfo settingInfo = SettingInfo.GetSingleton();
                Thread.Sleep(settingInfo.MachineLogOutWaitmSec);
                retryCt = retryCt + 1;
                fileList = GetMachineLogPathList(inputFolderNM, sTargetFile, retryCt);
            }
            else if (fileList.Count <= 0 && retryCt >= 3)
            {
                //装置からファイル取得指示が来たのにそのターゲットファイルが無い場合
                return null;
            }

            return fileList;
        }

        protected void ClearFileGetReqFg(LSETInfo lsetInfo, ref TcpClient tcp, ref NetworkStream ns, string fgAddr)
        {
            KLinkInfo kLinkInfo = new KLinkInfo();

            //ファイル取得要求 立ち下げ
            kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, fgAddr, 0, 1, Constant.ssuffix_U);
        }

        protected bool IsNeedMoveFileFromCommonPath(string filePath, string machineSeqNo, bool isOutputCommonPath, int machineSeqStartIndex, int machineSeqLen)
        {
            string fileNm = Path.GetFileName(filePath);

            // 共通出力場所に出るかどうかのフラグが有効ならファイル名に号機が含まれるかどうかをチェック
            if (isOutputCommonPath)
            {
                string machineSeqNoOnFileNm;
                try
                {
                    machineSeqNoOnFileNm = fileNm.Substring(machineSeqStartIndex, machineSeqLen);
                }
                catch (ArgumentOutOfRangeException err)
                {
                    // 文字列中に存在しない範囲を参照した場合（仕様上は無いはず）
                    // DRBFMで何かメッセージを出すかどうか対応を確認

                    return false;
                }

                // マスタから取得する号機は先頭0埋め無し、ファイル名の方は先頭の0埋め有なので
                // マスタ取得値の桁数不足分は先頭0埋めし比較する 湯浅さん仕様  2015/4/23 n.yoshimoto
                if (machineSeqNoOnFileNm == machineSeqNo.PadLeft(machineSeqLen, '0'))
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
                return false;
            }
        }

        // 装置によるファイルのロック状態についてどうするか
        protected void MoveOwnFileFromCommonPath(string commonPath, string destPath, string machineSeqNo, bool isOutputCommonPath, int machineSeqStartIndex, int machineSeqLen)
        {
            if (isOutputCommonPath == false)
            {
                return;
            }

            string[] filePaths = Directory.GetFiles(commonPath);

            List<FileInfo> moveTargets = new List<FileInfo>();

            foreach (string filePath in filePaths)
            {
                if (IsNeedMoveFileFromCommonPath(filePath, machineSeqNo, isOutputCommonPath, machineSeqStartIndex, machineSeqLen))
                {
                    //現在のfilePathのファイルが移動対象なので移動する
                    moveTargets.Add(new FileInfo(filePath));
                }
            }

            // ファイルを更新日時順に並べて、古い順に移動する
            moveTargets = moveTargets.OrderBy(m => m.LastWriteTime).ToList();

            foreach (FileInfo moveTarget in moveTargets)
            {
                int retryMax = 10;
                for (int tryCt = 1; tryCt <= retryMax; tryCt++)
                {
                    try
                    {
                        moveTarget.MoveTo(Path.Combine(destPath, moveTarget.Name));
                        break;
                    }
                    catch (Exception err)
                    {
                        //移動を規定回数失敗したファイルが発生した場合、それ以降の処理も抜ける
                        if (tryCt == retryMax)
                        {
                            return;
                        }
                        Thread.Sleep(3000);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 指定ファイルを指定フォルダに移動する。フォルダが無ければ生成する。
        /// </summary>
        public static void MoveFile(FileInfo currentFile, string targetFolderPath)
        {

            if (!Directory.Exists(targetFolderPath))
            {
                Directory.CreateDirectory(targetFolderPath);
            }

            try
            {
                currentFile.MoveTo(Path.Combine(targetFolderPath, currentFile.Name));
            }
            catch
            {
                //読み取り専用になる為、3秒後もう一度試す。
                Thread.Sleep(3000);
                currentFile.MoveTo(Path.Combine(targetFolderPath, currentFile.Name));
            }
        }

        /// <summary>
        /// 元フォルダ内をチェックし、ファイルが指定数以上存在する場合、超過分を古い順に先フォルダに移動する。
        /// </summary>
        public static void CheckFileCountAndMoveFile(string checkFolderPath, string moveFolderPath, int lotCt)
        {
            DirectoryInfo dir = new DirectoryInfo(checkFolderPath);

            var files = dir.GetFiles();
            if (files.Count() >= lotCt)
            {
                files = files.OrderBy(l => l.LastWriteTime).ToArray();

                for (int i = 0; i < (files.Count() - lotCt); i++)
                {
                    FileInfo currentFile = new FileInfo(files[i].FullName);
                    MoveFile(currentFile, moveFolderPath);
                }
            }
        }

        public static void OutputEicsEndTrigFile(string outputDirPath)
        {
            string fileName = string.Format("EICSEND{0}.CSV", DateTime.Now.ToString("yyyyMMddHHmmss"));
            StreamWriter sw = new StreamWriter(Path.Combine(outputDirPath, fileName), true, Encoding.UTF8);

            sw.Close();
        }

        public static bool IsArmsEnd(string chkDirPath, int retryCt, int retryIntervalMilliSec)
        {
            List<string> eicsEndFiles = Common.GetFiles(chkDirPath, "^EICSEND.+");

            if (eicsEndFiles == null || eicsEndFiles.Count == 0)
            {
                throw new ApplicationException("EICSENDﾌｧｲﾙが未出力です。");
            }

            string eicsEndTrigFilePath = eicsEndFiles[0];

            string armsEndTrigFileNm = Path.GetFileName(eicsEndTrigFilePath).Replace("EICSEND", "ARMSEND");
            string armsEndTrigFilePath = Path.Combine(chkDirPath, armsEndTrigFileNm);

            if (File.Exists(armsEndTrigFilePath))
            {
                //ファイルを削除するタイミングを装置との通信完了時に変更。2016.5.25 湯浅
                //Thread.Sleep(100);
                //File.Delete(eicsEndTrigFilePath);
                //File.Delete(armsEndTrigFilePath);

                return true;
            }
            else
            {
                int tryCt = 0;
                while (File.Exists(armsEndTrigFilePath) == false)
                {
                    tryCt++;

                    if (tryCt >= retryCt)
                    {
                        return false;
                    }
                    else
                    {
                        //2016.5.24 LMタイムアウト調査用の臨時ログ(後で削除すること)
                        string logMsg = string.Format("{0} パス:{1}：ARMSENDファイル取得リトライ{2}回目実施", DateTime.Now, chkDirPath, tryCt.ToString());
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

                        Thread.Sleep(retryIntervalMilliSec);
                    }
                }

                //ファイルを削除するタイミングを装置との通信完了時に変更。2016.5.25 湯浅
                //Thread.Sleep(100);
                //File.Delete(eicsEndTrigFilePath);
                //File.Delete(armsEndTrigFilePath);

                return true;
            }
        }

        public static void CheckFileFmtFromParamWhenInit(LSETInfo lsetInfo, bool containWbFmt)
        {
            RunningLog runLog = RunningLog.GetInstance();
            try
            {
                string runMsg = string.Format("設備:{0}[{1}:{2}号機]タイプ:『{3}』チップ/作業:『{4}』【監視開始時】閾値・ファイル参照マスタ取得テスト開始"
                    , lsetInfo.EquipmentNO, lsetInfo.ModelNM, lsetInfo.MachineSeqNO, lsetInfo.TypeCD, lsetInfo.ChipNM);
                runLog.logMessageQue.Enqueue(runMsg);

                List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM
                    , ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), false);

                FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, containWbFmt);

                runMsg = string.Format("設備:{0}[{1}:{2}号機]タイプ:『{3}』チップ/作業:『{4}』【監視開始時】閾値・ファイル参照マスタ取得テスト完了"
                    , lsetInfo.EquipmentNO, lsetInfo.ModelNM, lsetInfo.MachineSeqNO, lsetInfo.TypeCD, lsetInfo.ChipNM);
                runLog.logMessageQue.Enqueue(runMsg);
            }
            catch (Exception err)
            {
                string runMsg = string.Format("設備:{0}[{1}:{2}号機]タイプ:『{3}』チップ/作業:『{4}』【監視開始時】閾値・ファイル参照マスタ取得テスト エラー終了"
                    , lsetInfo.EquipmentNO, lsetInfo.ModelNM, lsetInfo.MachineSeqNO, lsetInfo.TypeCD, lsetInfo.ChipNM);
                runLog.logMessageQue.Enqueue(runMsg);

                throw new ApplicationException(err.ToString());
            }
        }

        /// <summary>
        /// ロット情報の参照先を取得
        /// </summary>
        /// <param name="plantCd"></param>
        /// <returns></returns>
        public static LotInfoReferring GetLotInfoReferring(string plantCd, int lineCd)
        {
            Arms.Machine m = Arms.Machine.GetMachine(lineCd, plantCd);
            if (m.IsOutLine)
            {
                return LotInfoReferring.NASCA;
            }
            else
            {
                return LotInfoReferring.ARMS;
            }
        }
    }
}
