using EICS.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.PLCDDBBase
{
	/// <summary>
	/// ウェットブラスト装置(マコー)
	/// </summary>
	class WTBTMachineInfo : PLCDDGBasedMachine
	{
		private const string PLC_MEMORY_ADDR_OK_SEND = "M0005E7";
		private const string PLC_MEMORY_ADDR_NG_SEND = "M0005E8";
		private const string PLC_MEMORY_ADDR_NG_REASON = "ZR001BBC";
        private const string PLC_MEMORY_ADDR_RESULT_READ = "M0005DD";
        private const string NG_REASON_FOR_PLC_SEND = "EICS Parameter Error";


        private const string PLC_MEMORY_ADDR_ERROR_WRITE = "M0005E0";
        private const string PLC_MEMORY_ADDR_ERROR_READ = "M0005E1";
        private const string PLC_MEMORY_ADDR_ERROR_CODE = "ZR002AFA";
        private const string PLC_MEMORY_ADDR_ERROR_YEARMONTH = "ZR002AFB";
        private const string PLC_MEMORY_ADDR_ERROR_DAYHOUR = "ZR002AFC";
        private const string PLC_MEMORY_ADDR_ERROR_MINSEC = "ZR002AFD";

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
				if (lsetInfo.MainThreadFG)
				{
					CreateFileProcess(lsetInfo, true);
                    CreateFileProcess(lsetInfo, false);
                    LoggingErrorLog(lsetInfo);
                }

				StartingProcess(lsetInfo);

                int timingNo = GetTimingNo(lsetInfo.ChipNM);
                EndingProcess(lsetInfo, timingNo);

                ResponseProcess(lsetInfo);
#endif
			}
			catch (Exception err)
			{
				//装置停止処理
				//SendMachineStop(PLC_MEMORY_ADDR_MACHINE_STOP());
				throw;
			}
		}

		private void ResponseProcess(LSETInfo lset)
		{
			RunningLog runLog = RunningLog.GetInstance();
			List<string> okFileList = new List<string>();
			List<string> ngFileList = new List<string>();

			//移動対象を抽出
			//OKファイルがあるかチェック
			okFileList = Common.GetFiles(StartFileDir, string.Format("[.]{0}$", EICS.Structure.CIFS.EXT_OK_FILE));
			ngFileList = Common.GetFiles(StartFileDir, string.Format("[.]{0}$", EICS.Structure.CIFS.EXT_NG_FILE));

			DirectoryInfo di = new DirectoryInfo(StartFileDir);


			if (ngFileList.Count > 0)
			{

				string errMsg = NG_REASON_FOR_PLC_SEND;

				plc.SetString(PLC_MEMORY_ADDR_NG_REASON, NG_REASON_FOR_PLC_SEND);

				//もしくはSetBitを使う
				plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_NG_SEND, 1);

				runLog.logMessageQue.Enqueue(string.Format(
					"設備:{0}/{1}/{2}号機【NG信号送信】理由:{3}", lset.ModelNM, lset.EquipmentNO, lset.MachineSeqNO, errMsg));

			}
			else if (okFileList.Count > 0)
			{
				plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_OK_SEND, 1);
				runLog.logMessageQue.Enqueue(string.Format(
						"設備:{0}/{1}/{2}号機【OK信号送信】", lset.ModelNM, lset.EquipmentNO, lset.MachineSeqNO));
			}
			else
			{
				return;
			}
            	
			//OKファイルを退避(サブ装置スレッドのファイルも全て移動
			EICS.Structure.CIFS.BackupDoneFiles(okFileList, StartFileDir, string.Empty, DateTime.Now);
			EICS.Structure.CIFS.BackupDoneFiles(ngFileList, StartFileDir, string.Empty, DateTime.Now);

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
				"設備:{0}/{1}/{2}号機【判定結果ファイルバックアップ完了】", lset.ModelNM, lset.EquipmentNO, lset.MachineSeqNO));
		}

        /// <summary>
        /// 装置実行パラメータ取得の際に、読取フラグを一旦こちらがONにする必要があるため
        /// ブラスト装置用にオーバーライド。
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="isStartUpFile"></param>
        public override void CreateFileProcess(LSETInfo lsetInfo, bool isStartUpFile, string prefixNm)
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

                    Dictionary<string, string> prefixList = GetPrefixList(lsetInfo, isStartUpFile, null);

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

                        //装置実行パラメータの処理の場合、読み取り信号を一旦入れてから落とす。
                        if(isStartUpFile == false)
                        {
                            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_RESULT_READ, 1);
                            System.Threading.Thread.Sleep(1000);                      
                            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_RESULT_READ, 0);

                            //装置が要求信号を落とす前にファイルを再生成しない様、念のため1秒スリープ
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw;
            }
        }

        /// <summary>
        /// ブラスト装置は実行パラメータは基板毎に出るがリネーム・移動はマガジン完了時にまとめてするので
        /// finが既にあったら処理しないという処理を除外するため、オーバーライドする。
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="prefixList"></param>
        /// <param name="outputDir"></param>
        /// <param name="ext"></param>
        /// <param name="isStartUp"></param>
        /// <param name="dtStr"></param>
        /// <param name="createFileNm"></param>
        /// <param name="subGrpOutputDirList"></param>
        /// <returns></returns>
        protected override bool CreateFileFromPLCData(LSETInfo lsetInfo, Dictionary<string, string> prefixList, string outputDir, string ext, bool isStartUp, out string dtStr, out string createFileNm, out List<string> subGrpOutputDirList)
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

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
                    "[{2} {3}:{4}]【ﾃﾞｰﾀ取得要求確認】prefx:{0} addr:{1}", prefixAddr.Key, prefixAddr.Value, lsetInfo.AssetsNM, lsetInfo.EquipmentCD, lsetInfo.EquipmentNO));

                List<PlcFileConv> plcFileConvList = PlcFileConv.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, prefixAddr.Key);
                //PLCメモリアドレスリストからファイル出力する際の配列の大きさを計算し、配列を動的生成する。Shift_JIS
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備:{0} PLCから取得する必要のあるデータの一覧取得", lsetInfo.EquipmentNO));

                dtStr = DateTime.Now.ToString("yyyyMMddHHmmss");
                createFileNm = string.Format("{0}_{1}.{2}", dtStr, prefixAddr.Key, ext);
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



        /// <summary>
        /// 装置のエラーログを吸い上げてロギングする関数。マコーウェットブラスト装置限定処理多し。
        /// </summary>
        /// <param name="lsetIndf"></param>
        private void LoggingErrorLog(LSETInfo lsetInfo)
        {

            if (IsGetableData(plc, PLC_MEMORY_ADDR_ERROR_WRITE) == false)
            {
                return;
            }
                
            string errorCode = plc.GetDataAsString(PLC_MEMORY_ADDR_ERROR_CODE, 1, PLC.DT_DEC_16BIT);

            if (String.IsNullOrWhiteSpace(errorCode) == true)
            {
                string errMsg = $"エラーログ書込み信号が出力されましたがPLCからデータが取得できません。号機：{lsetInfo.EquipmentNO}";
                throw new ApplicationException(errMsg);
            }


            string dtStr = string.Empty;
            string createFileNm = string.Empty;
            string dataTemp;

            dtStr = DateTime.Now.ToString("yyyyMMddHHmmss");
            createFileNm = string.Format("{0}.err", dtStr);

            string loggingDir = Path.Combine(EndFileDir, "ErrLog", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));
            string filePath = Path.Combine(loggingDir, createFileNm);

               
            dataTemp = plc.GetDataAsString(PLC_MEMORY_ADDR_ERROR_YEARMONTH, 1, PLC.DT_BCD16BIT).PadLeft(4, '0');
            string errYear = dataTemp.Substring(0, 2);
            string errMonth = dataTemp.Substring(2, 2);

            dataTemp = plc.GetDataAsString(PLC_MEMORY_ADDR_ERROR_DAYHOUR, 1, PLC.DT_BCD16BIT).PadLeft(4, '0');
            string errDay = dataTemp.Substring(0, 2);
            string errHour = dataTemp.Substring(2, 2);

            dataTemp = plc.GetDataAsString(PLC_MEMORY_ADDR_ERROR_MINSEC, 1, PLC.DT_BCD16BIT).PadLeft(4, '0');
            string errMin = dataTemp.Substring(0, 2);
            string errSec = dataTemp.Substring(2, 2);

            string logStr = string.Format("発生日時：{0}/{1}/{2} {3}:{4}:{5}, エラーコード：{6}\n", errYear, errMonth, errDay, errHour, errMin, errSec, errorCode);

            if (Directory.Exists(loggingDir) == false)
            {
                Directory.CreateDirectory(loggingDir);
            }

            File.WriteAllText(filePath, logStr, Encoding.UTF8);

            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_ERROR_READ, 1);
            System.Threading.Thread.Sleep(1000);
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_ERROR_READ, 0);

            //装置が要求信号を落とす前にファイルを再生成しない様、念のため1秒スリープ
            System.Threading.Thread.Sleep(1000);

        }
    }
   
}
