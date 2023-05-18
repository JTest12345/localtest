using EICS.Database;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EICS.Machine
{
    //PLCDDG(PLC Direct Data Get)
    public class PLCDDGBasedMachine : CIFSBasedMachine
    {
        public struct ExtractExclusion
        {
            public string ModelNm;
            public string DataType;
            public string ExceptionValue;
        }

        protected virtual string LF() { return "\r\n"; }
        protected virtual string[] PLC_MEMORY_ADDR_HEART_BEAT() { return new string[] { "" }; }
        protected virtual string PLC_MEMORY_ADDR_MACHINE_STOP() { return ""; }
        protected virtual string DATA_SPLITTER() { return ","; }

        public PLCDDGBasedMachine()
        {
        }

        public PLCDDGBasedMachine(LSETInfo lsetInfo)
        {
            InitPropAtLoop(lsetInfo);
            InitPLC(lsetInfo);
        }

        public IPlc plc;

        public void InitPLC(LSETInfo lsetInfo)
        {
            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
            SettingInfo commonSettingInfo = SettingInfo.GetSingleton();
            string plcType = settingInfoPerLine.GetPLCType(lsetInfo.EquipmentNO);

            List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList
                = commonSettingInfo.PlcExtractExclusionList.Where(p => p.ModelNm == lsetInfo.ModelNM).ToList();

            if (plc == null)
            {
                if (plcType == PLC_Keyence.PLC_TYPE)
                {
                    plc = new PLC_Keyence(lsetInfo.IPAddressNO, lsetInfo.PortNO);
                    //plcGetStringSwapFG = false;
                }
                else if (plcType == PLC_Omron.PLC_TYPE)
                {
                    //オムロンPLCを使用する場合
                    plc = new PLC_Omron(commonSettingInfo.LocalHostIP, lsetInfo.IPAddressNO, Convert.ToByte(PLC_Omron.GetNodeAddress(lsetInfo.IPAddressNO)), lsetInfo.LoaderPlcNodeNO, commonSettingInfo.PLCReceiveTimeout);
                    //plcGetStringSwapFG = true;
                }
                else if (plcType == PLC_Melsec.PLC_TYPE)
                {
                    //三菱PLCを使用する場合
                    plc = new PLC_Melsec(lsetInfo.IPAddressNO, lsetInfo.PortNO, exclusionList);
                    //plcGetStringSwapFG = false;
                }
                else if (plcType == PLC_MelsecUDP.PLC_TYPE)
                {
                    plc = new PLC_MelsecUDP(lsetInfo.IPAddressNO, lsetInfo.PortNO, exclusionList);
                }
                else
                {
                    throw new NotImplementedException("PLCタイプ未定義");
                }
            }
        }

        protected void InitPropAtLoop(LSETInfo lsetInfo)
        {
            try
            {
                SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

                DateIndex = settingInfoPerLine.GetDateStrIndex(lsetInfo.EquipmentNO);
                DateLen = settingInfoPerLine.GetDateStrLen(lsetInfo.EquipmentNO);

                EndFileIdLen = settingInfoPerLine.GetEndIdLen(lsetInfo.EquipmentNO);

                StartFileDir = Path.Combine(lsetInfo.InputFolderNM, settingInfoPerLine.GetStartFileDirNm(lsetInfo.EquipmentNO));
                EndFileDir = Path.Combine(lsetInfo.InputFolderNM, settingInfoPerLine.GetEndFileDirNm(lsetInfo.EquipmentNO));

                AvailableAddress = settingInfoPerLine.GetAvailableAddress(lsetInfo.EquipmentNO);
                EncodeStr = settingInfoPerLine.GetEncodeStr(lsetInfo.EquipmentNO);

            }
            catch (Exception err)
            {
                throw;
            }
        }

        public override void CheckFile(LSETInfo lsetInfo)
        {
#if TEST
			lsetInfo.InputFolderNM = @"C:\qcil\data\test";
			lsetInfo.IPAddressNO = "172.21.56.53";
#endif
            try
            {
                base.machineStatus = Constant.MachineStatus.Runtime;

                InitPropAtLoop(lsetInfo);
                InitPLC(lsetInfo);
                //問題発生時は装置停止
#if TEST
#else
                //ハートビート Hレベル
                SendHeartBeat(PLC_MEMORY_ADDR_HEART_BEAT(), true);

                if (lsetInfo.MainThreadFG)
                {
                    CreateFileProcess(lsetInfo, true);
                    CreateFileProcess(lsetInfo, false);
                }

                StartingProcess(lsetInfo);

                MachineStopProcess(lsetInfo, true);
#endif
                int timingNo = GetTimingNo(lsetInfo.ChipNM);

                EndingProcess(lsetInfo, timingNo);
#if TEST
#else
                MachineStopProcess(lsetInfo, false);

                ResponseOKFile(true, lsetInfo);
                ResponseOKFile(false, lsetInfo);

                //ハートビート Lレベル
                SendHeartBeat(PLC_MEMORY_ADDR_HEART_BEAT(), false);
#endif
            }
            catch (Exception err)
            {
                //装置停止処理
                SendMachineStop(PLC_MEMORY_ADDR_MACHINE_STOP());
                throw;
            }
        }

        public virtual void MachineStopProcess(LSETInfo lsetInfo, bool isStartUp)
        {
            //バックアップしている実態が関数名から認知できないので、関数名を修正する
            if (CheckForMachineStopFile(isStartUp))
            {
                //開始の時のみエラーで止める。
                if (isStartUp)
                {
                    base.machineStatus = Constant.MachineStatus.Stop;
                    SendMachineStop(PLC_MEMORY_ADDR_MACHINE_STOP());
                }

                //存在する場合はデータ取得要求フラグを落とす
                Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, isStartUp, null, null);
                foreach (KeyValuePair<string, string> prefixAddr in prefixList)
                {
#if DEBUG
					continue;
#endif
                    ResetDataRecvReq(prefixAddr.Key, prefixAddr.Value);
                }
            }
        }
        /// <summary>
        /// 本来のMachineStopProcessはNGファイルの存在をチェックしてNG処置するが、
        /// この関数はファイル有無に関連せず、呼び出したら即NG通知をする。
        /// </summary>
        /// <param name="isStartUp"></param>
        public void MachineStopProcessWithoutFile(LSETInfo lsetInfo, bool isStartUp)
        {
            base.machineStatus = Constant.MachineStatus.Stop;
            SendMachineStop(PLC_MEMORY_ADDR_MACHINE_STOP());

            //存在する場合はデータ取得要求フラグを落とす
            Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, isStartUp, null, null);
            foreach (KeyValuePair<string, string> prefixAddr in prefixList)
            {
                ResetDataRecvReq(prefixAddr.Key, prefixAddr.Value);
                continue;
            }
        }

        protected virtual void ResetDataRecvReq(string prefixNm, string plcMemAddr)
        {
#if DEBUG || TEST
			return;
#endif

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
                        "【ﾃﾞｰﾀ取得要求OFF】prefx:{0} addr:{1}", prefixNm, plcMemAddr));

            plc.SetWordAsDecimalData(plcMemAddr, 0);
        }

        protected virtual void SendHeartBeat(string[] plcMemAddrArray, bool bitVal)
        {
#if DEBUG
			return;
#endif
            foreach (string plcMemAddr in plcMemAddrArray)
            {
                if (bitVal)
                {
                    plc.SetWordAsDecimalData(plcMemAddr, 1);
                }
                else
                {
                    plc.SetWordAsDecimalData(plcMemAddr, 0);
                }
            }
        }

        public bool CheckForMachineStopFile(bool isStartUp)
        {
            List<string> filePathList = new List<string>();

            if (isStartUp == true)
            {
                //開始フォルダに.NGファイルリスト追加
                filePathList.AddRange(Common.GetFiles(StartFileDir, ".*\\." + CIFS.EXT_NG_FILE));
                //開始フォルダに.STOPファイルリスト追加
                filePathList.AddRange(Common.GetFiles(StartFileDir, ".*\\." + CIFS.EXT_STOP_FILE));

                if (filePathList.Count() > 0)
                {
                    CIFS.BackupDoneFiles(filePathList, StartFileDir, string.Empty, DateTime.Now);
                    return true;
                }
            }
            else
            {
                //完了フォルダに.NGファイルリスト追加
                filePathList.AddRange(Common.GetFiles(EndFileDir, ".*\\." + CIFS.EXT_NG_FILE));
                //完了フォルダに.STOPファイルリスト追加
                filePathList.AddRange(Common.GetFiles(EndFileDir, ".*\\." + CIFS.EXT_STOP_FILE));

                if (filePathList.Count() > 0)
                {
                    CIFS.BackupDoneFiles(filePathList, EndFileDir, string.Empty, DateTime.Now);
                    return true;
                }
            }
            return false;
        }

        protected virtual void ResponseOKFile(bool isStartUp, LSETInfo lset)
        {
            List<string> chkDirList = new List<string>();
            List<string> chkTargetFileList = new List<string>();
            List<KeyValuePair<string, List<string>>> moveTargetFileList = new List<KeyValuePair<string, List<string>>>();

            if (isStartUp)
            {
                chkDirList.Add(StartFileDir);
            }
            else
            {
                chkDirList.Add(EndFileDir);
            }

            chkDirList.AddRange(GetSubGrpOutputDir(lset, isStartUp, string.Empty));

            //移動対象を抽出
            foreach (string chkDir in chkDirList)
            {
                //OKファイルがあるかチェック
                chkTargetFileList = Common.GetFiles(chkDir, EICS.Structure.CIFS.EXT_OK_FILE);
                if (chkTargetFileList.Count == 0) return;

                KeyValuePair<string, List<string>> moveInfo = new KeyValuePair<string, List<string>>(chkDir, chkTargetFileList);

                moveTargetFileList.Add(moveInfo);
            }

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
                "設備:{0}/{1}/{2}号機【OKﾌｧｲﾙ確認の為、ﾃﾞｰﾀ取得要求OFFと出力ﾌｧｲﾙ移動を開始】 isStartUpFg:{3}", lset.ModelNM, lset.EquipmentNO, lset.MachineSeqNO, isStartUp));

            //存在する場合はデータ取得要求フラグを落とす
            Dictionary<string, string> prefixList = GetPrefixList(lset, isStartUp, null);
            foreach (KeyValuePair<string, string> prefixAddr in prefixList)
            {
                ResetDataRecvReq(prefixAddr.Key, prefixAddr.Value);
            }

            //OKファイルを退避(サブ装置スレッドのファイルも全て移動
            foreach (KeyValuePair<string, List<string>> moveTarget in moveTargetFileList)
            {
                EICS.Structure.CIFS.BackupDoneFiles(moveTarget.Value, moveTarget.Key, string.Empty, DateTime.Now);
            }
        }

        public void SendMachineStop(string memAddr)
        {
#if DEBUG || TEST
			return;
#else
            plc.SetWordAsDecimalData(memAddr, 1);
#endif
        }

        /// <summary>
        /// PLCからデータを取得してファイル出力する（開始時、終了時どちらのデータかはisStartUpFileによって識別）
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="isStartUpFile"></param>
        public virtual void CreateFileProcess(LSETInfo lsetInfo, bool isStartUpFile, string prefixNm, string equipPartNo)
        {
            try
            {
                if (lsetInfo.MainThreadFG)
                {
                    //データ取得要求チェック
                    bool fileCreatedFg = false;
                    string dtStr = string.Empty;
                    string createFileNm = string.Empty;
                    string outputDir;
                    string ext = string.Empty;

                    List<string> subGrpOutputDirList = new List<string>();

                    SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

                    if (isStartUpFile)
                    {
                        outputDir = StartFileDir;
                        ext = EXT_START_FILE;
                    }
                    else
                    {
                        outputDir = EndFileDir;
                        ext = EXT_END_FILE();
                    }

                    if (Directory.Exists(outputDir) == false)
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    Dictionary<string, string> prefixList = GetPrefixList(lsetInfo, isStartUpFile, prefixNm, equipPartNo);

                    fileCreatedFg = CreateFileFromPLCData(lsetInfo, prefixList, outputDir, ext, isStartUpFile, out dtStr, out createFileNm, out subGrpOutputDirList);

                    //ファイル出力されたならfinファイル出力
                    if (fileCreatedFg && string.IsNullOrEmpty(createFileNm) == false)
                    {
                        string finFilePath = Path.Combine(outputDir, string.Join(".", dtStr, EXT_TRIG_FILE()));

                        //File.Create(finFilePath);

                        StreamWriter sw = new StreamWriter(finFilePath);
                        sw.Close();

                        if (subGrpOutputDirList != null)
                        {
                            foreach (string subGrpOutpuDir in subGrpOutputDirList)
                            {
                                string subGrpFilePath = Path.Combine(subGrpOutpuDir, Path.GetFileName(finFilePath));
                                //File.Create(subGrpFilePath);
                                StreamWriter subSw = new StreamWriter(subGrpFilePath);
                                subSw.Close();
                            }
                        }

                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
                            "[{1} {2}:{3}]【finﾌｧｲﾙ作成完了】ﾌｧｲﾙﾊﾟｽ:{0}", finFilePath, lsetInfo.AssetsNM, lsetInfo.EquipmentCD, lsetInfo.EquipmentNO));
                    }
                }
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public virtual void CreateFileProcess(LSETInfo lsetInfo, bool isStartUpFile, string prefixNm)
        {
            CreateFileProcess(lsetInfo, isStartUpFile, prefixNm, null);
        }

        public virtual void CreateFileProcess(LSETInfo lsetInfo, bool isStartUpFile)
        {
            CreateFileProcess(lsetInfo, isStartUpFile, null);
        }

        protected virtual Dictionary<string, string> GetPrefixList(LSETInfo lsetInfo, bool isStartUp, string prefixNm)
        {
            return ConnectDB.GetMachineFilePrefix(lsetInfo, 0, isStartUp, prefixNm, null);
        }

        protected virtual Dictionary<string, string> GetPrefixList(LSETInfo lsetInfo, bool isStartUp, string prefixNm, string equipPartNo)
        {
            return ConnectDB.GetMachineFilePrefix(lsetInfo, 0, isStartUp, prefixNm, equipPartNo);
        }

        /// <summary>
        /// PLCﾒﾓﾘから取得した値をファイル出力する
        /// </summary>
        /// <param name="prefixList"></param>
        /// <param name="outputDir"></param>
        /// <param name="dtStr"></param>
        /// <param name="createFileNm"></param>
        /// <param name="ext"></param>
        /// <param name="isStartUp"></param>
        /// <param name="subGrpOutputDirList"></param>
        /// <returns>ファイルが作成された場合、true</returns>
        protected virtual bool CreateFileFromPLCData(LSETInfo lsetInfo, Dictionary<string, string> prefixList, string outputDir, string ext, bool isStartUp, out string dtStr, out string createFileNm, out List<string> subGrpOutputDirList)
        {
            bool fileCreatedFg = false;
            dtStr = string.Empty;
            createFileNm = string.Empty;
            subGrpOutputDirList = new List<string>();

            //取得要求の出ているPrefixに関連するPLCメモリアドレスリストを取得
            foreach (KeyValuePair<string, string> prefixAddr in prefixList)
            {
                //データ取得要求チェック
                if (IsGetableData(plc, prefixAddr.Value) == false)
                {
                    continue;
                }

                if (IsFinishedFile(outputDir))
                {
                    continue;
                }

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
                    "[{2} {3}:{4}]【ﾃﾞｰﾀ取得要求確認】prefx:{0} addr:{1}", prefixAddr.Key, prefixAddr.Value, lsetInfo.AssetsNM, lsetInfo.EquipmentCD, lsetInfo.EquipmentNO));

                List<PlcFileConv> plcFileConvList = PlcFileConv.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, prefixAddr.Key);
                //PLCメモリアドレスリストからファイル出力する際の配列の大きさを計算し、配列を動的生成する。Shift_JIS
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{0} PLCから取得する必要のあるデータの一覧取得", lsetInfo.EquipmentNO));

                dtStr = DateTime.Now.ToString("yyyyMMddHHmmss");
                createFileNm = string.Format("{0}_{1}.{2}", dtStr, prefixAddr.Key, ext); ;
                string filePath = Path.Combine(outputDir, createFileNm);

                subGrpOutputDirList = GetSubGrpOutputDir(lsetInfo, isStartUp, dtStr);

                //ファイルへ出力(カンマ区切り)
                string[] fileTxtArray = ConvertPLCDataToFile(filePath, plcFileConvList, lsetInfo.EquipmentNO);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{0} ConvertPLCDataToFile()完了", lsetInfo.EquipmentNO));
                foreach (string subGrpOutpuDir in subGrpOutputDirList)
                {
                    if (Directory.Exists(subGrpOutpuDir) == false)
                    {
                        Directory.CreateDirectory(subGrpOutpuDir);
                    }

                    string subGrpFilePath = Path.Combine(subGrpOutpuDir, createFileNm);
                    File.WriteAllLines(subGrpFilePath, fileTxtArray, Encoding.GetEncoding("Shift_JIS"));
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{0} File.WriteAllLines()完了", lsetInfo.EquipmentNO));
                }

                fileCreatedFg = true;

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
                    "[{1} {2}:{3}]【ﾌｧｲﾙ作成完了】prefx:{0}", prefixAddr.Key, lsetInfo.AssetsNM, lsetInfo.EquipmentCD, lsetInfo.EquipmentNO));
            }

            return fileCreatedFg;
        }
        
        protected virtual List<string> GetSubGrpOutputDir(LSETInfo lsetInfo, bool isStartUpFile, string fileIdentity)
        {
            List<string> subGrpOutputDirList = new List<string>();
            List<LSETInfo> subGrpEquipList = new List<LSETInfo>();

            try
            {
                //サブ装置スレッド用のディレクトリ取得
                List<LSETInfo> lsetList = ConnectDB.GetLSETData(lsetInfo.InlineCD, string.Empty);
                SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

                if (string.IsNullOrEmpty(lsetInfo.ThreadGrpCD) == false)
                {
                    if (lsetList.Count > 0)
                    {
                        subGrpEquipList = lsetList.Where(l => l.ThreadGrpCD == lsetInfo.ThreadGrpCD && l.MainThreadFG == false).ToList();
                    }
                    else
                    {
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.WARN, "subGrpEquip.EquipmentNOがNULL");
                    }
                }

                if (isStartUpFile)
                {
                    foreach (LSETInfo subGrpEquip in subGrpEquipList)
                    {
                        if (string.IsNullOrEmpty(subGrpEquip.EquipmentNO) || string.IsNullOrEmpty(subGrpEquip.InputFolderNM))
                        {
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.WARN, "subGrpEquip.EquipmentNOがNULL(isStartUp=1)");
                            continue;
                        }

                        string outputSubDir = string.Empty;
                        outputSubDir = settingInfoPerLine.GetStartFileDirNm(subGrpEquip.EquipmentNO);
                        subGrpOutputDirList.Add(Path.Combine(subGrpEquip.InputFolderNM, outputSubDir));
                    }
                }
                else
                {
                    foreach (LSETInfo subGrpEquip in subGrpEquipList)
                    {
                        if (string.IsNullOrEmpty(subGrpEquip.EquipmentNO) || string.IsNullOrEmpty(subGrpEquip.InputFolderNM))
                        {
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.WARN, "subGrpEquip.EquipmentNOがNULL(isStartUp=0)");
                            continue;
                        }

                        string outputSubDir = string.Empty;
                        outputSubDir = settingInfoPerLine.GetEndFileDirNm(subGrpEquip.EquipmentNO);
                        subGrpOutputDirList.Add(Path.Combine(subGrpEquip.InputFolderNM, outputSubDir));
                    }
                }

            }
            catch (Exception err)
            {
                throw;
            }

            return subGrpOutputDirList;
        }


        //protected void JudgementProcess(LSETInfo lsetInfo)
        //{
        //	try
        //	{
        //		FileScan fileScan = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, prefixAddr.Key).Single();

        //		List<ErrMessageInfo> fileErrMsgList = new List<ErrMessageInfo>();

        //		string chipNM = settingInfoPerLine.GetChipNM(lsetInfo.EquipmentNO);
        //		if (lotNO == null || magNO == null || typeCD == null || dt.HasValue == false)
        //		{
        //			GetInfo(lsetInfo, plcFileConvList, txtArray, out lotNO, out magNO, out typeCD, out dt);
        //		}
        //		//ファイルの判定処理
        //		if (MachineFile.CheckMachineFile(filePath, LF(), typeCD, lotNO, magNO, dt.Value, fileScan, lsetInfo, chipNM, ref fileErrMsgList) == false)
        //		{
        //			continue;
        //		}

        //		//ファイルの移動(未実装)

        //		//取得要求を落とす
        //	}
        //	catch (Exception err)
        //	{
        //		throw;
        //	}
        //}

        protected void GetInfo(LSETInfo lsetInfo, List<PlcFileConv> plcFileConvList, string[] fileTxtArray, out string lotNO, out string magNO, out string typeCD, out DateTime? dt)
        {
            lotNO = GetStringData(lsetInfo.InlineCD, plcFileConvList, fileTxtArray, PlcFileConv.IDENTCD_LOTNO);

            magNO = GetStringData(lsetInfo.InlineCD, plcFileConvList, fileTxtArray, PlcFileConv.IDENTCD_MAGNO);

            typeCD = GetStringData(lsetInfo.InlineCD, plcFileConvList, fileTxtArray, PlcFileConv.IDENTCD_TYPE);

            if (string.IsNullOrEmpty(typeCD))
            {
                typeCD = lsetInfo.TypeCD;
            }

            string dtStr = GetStringData(lsetInfo.InlineCD, plcFileConvList, fileTxtArray, PlcFileConv.IDENTCD_MEASUREDT);

            DateTime tempDt;
            if (DateTime.TryParse(dtStr, out tempDt) == false)
            {
                dt = DateTime.Now;
            }
            else
            {
                dt = tempDt;
            }
        }

        protected virtual bool IsGetableData(IPlc plc, string plcMemAddr)
        {
//#if DEBUG
//			return false;
//#endif
            try
            {
                int retv = Convert.ToInt32(plc.GetBit(plcMemAddr));

                //int retv = plc.GetWordAsDecimalData(plcMemAddr, 1);

                if (retv == 0)
                {
                    return false;
                }
                else if (retv == 1)
                {
                    return true;
                }
                else
                {
                    throw new ApplicationException(string.Format("PLCから予期せぬ応答を受信しました。ﾒﾓﾘｱﾄﾞﾚｽ:{0} 応答:{1} 想定する応答:{2}", plcMemAddr, retv, "0 or 1"));
                }
            }
            catch
            {
                throw;
            }
        }

        public string GetStringData(int lineCD, List<PlcFileConv> plcFileConvList, string[] txtArray, string getTargetCD)
        {
            try
            {
                List<PlcFileConv> tempPlcFileConvList = plcFileConvList.Where(p => p.IdentifyCD == PlcFileConv.GetGeneralNm(lineCD, getTargetCD)).ToList();

                string header = string.Empty;

                if (tempPlcFileConvList.Count > 0)
                {
                    header = tempPlcFileConvList.Select(p => p.HeaderNM).First();
                }

                string strData = MachineFile.GetData(txtArray, header, true, false);

                return strData;
            }
            catch (Exception err)
            {
                throw;
            }
        }

        /// <summary>
        /// PLCのメモリアドレス内のデータをファイルに出力。ファイル出力した同じ内容を戻り値として返す
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="plcFileConvList"></param>
        /// <returns></returns>
        public virtual string[] ConvertPLCDataToFile(string filePath, List<PlcFileConv> plcFileConvList, string EquipCD)
        {
            try
            {
                int rowCt = plcFileConvList.Select(p => p.OrderNO).Max() + 1;

                List<string> retv = new List<string>();

                List<string> headerNmList = plcFileConvList.Select(p => p.HeaderNM).Distinct().OrderBy(h => h).ToList();

                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("Shift-JIS")))
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{0} PLCからのデータ書き込み用ファイル生成", EquipCD));
                    //ヘッダ列作成とファイル書き込み
                    string headerLineTxt = string.Join(DATA_SPLITTER(), headerNmList);
                    sw.WriteLine(headerLineTxt);
                    retv.Add(headerLineTxt);

                    for (int lineNo = 1; lineNo < rowCt; lineNo++)
                    {
                        List<PlcFileConv> plcFileConvPerLine = plcFileConvList.Where(p => p.OrderNO == lineNo).ToList();

                        List<string> lineTxtElement = new List<string>();
                        //ファイルに書き出す行毎に、各列のデータをPLCから取得
                        foreach (string headerNm in headerNmList)
                        {
                            int headerCt = plcFileConvPerLine.Where(p => p.HeaderNM == headerNm).Count();
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{0} ﾍｯﾀﾞ数取得", EquipCD));
                            if (headerCt > 1)
                            {
                                throw new ApplicationException(string.Format(
                                    "TmPlcFileConvのﾃﾞｰﾀにﾍｯﾀﾞ名の重複があります。同一のOrderNoでﾍｯﾀﾞ名は一意の必要があります。 OrderNo:{0} ﾍｯﾀﾞ名:{1}",
                                    lineNo, headerNm));
                            }
                            else if (headerCt == 0)
                            {
                                throw new ApplicationException(string.Format(
                                    "TmPlcFileConvにﾍｯﾀﾞが存在しません。右記条件のレコードが存在する必要があります。 OrderNo:{0} ﾍｯﾀﾞ名:{1}",
                                    lineNo, headerNm));
                            }

                            PlcFileConv plcFileConv = plcFileConvPerLine.Where(p => p.HeaderNM == headerNm).Single();
                            //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{0} plcFileConv特定", EquipCD));
                            //GetDataAsStringは未実装(2015/7/22 nyoshimoto) (7/25 nyoshimoto:三菱実装済み,KEYENCEとOMRONが未実装)
                            string elemData = plc.GetDataAsString(plcFileConv.PlcADDR, plcFileConv.DataLen, plcFileConv.DataTypeCD);//PLCから文字列形式でデータ取得
                                                                                                                                    //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{1} PLCからデータ取得完了 addr:{0}", plcFileConv.PlcADDR, EquipCD));
                            elemData = elemData.Trim('\r', '\n', '\0', ' ');
                            //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{1} PLCﾃﾞｰﾀ取得完 addr:{0}, len:{3}, Data:{2}", plcFileConv.PlcADDR, EquipCD, elemData, plcFileConv.DataLen.ToString()));

                            lineTxtElement.Add(elemData);

                            //Thread.Sleep(100);
                        }

                        string lineTxt = string.Join(DATA_SPLITTER(), lineTxtElement);
                        sw.WriteLine(lineTxt);

                        retv.Add(lineTxt);
                    }
                }

                return retv.ToArray();
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public bool IsAvailable(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return true;
            }

            if (plc.GetBit(address, 1) == PLC.BIT_ON)
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
