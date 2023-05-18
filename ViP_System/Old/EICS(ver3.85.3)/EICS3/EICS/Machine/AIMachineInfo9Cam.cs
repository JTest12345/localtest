using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using EICS.Database;
using EICS.Structure;

namespace EICS.Machine
{
	class AIMachineInfo9Cam : AIMachineInfo
	{
        protected TcpClient tcp = null;
        protected NetworkStream ns = null;

		private const int FILE_OA_OPENINGCHECKFG_COL = 69;
		/// <summary>ファイルOA内容列(傾向管理フラグ)</summary>
		private const int FILE_OA_KEIKOUFG_COL = 4;

		public const int TimingNO = 6;

		public AIMachineInfo9Cam(LSETInfo lset) : base(lset)
		{
			lsetInfo = lset;
			this.LineCD = lset.InlineCD;
		}

        //public AIMachineInfo9Cam(int lineCD) : base(lineCD)
        //{
        //	this.LineCD = lineCD;
        //}

        /// <summary>
        /// ファイルチェック
        /// </summary>
        /// <param name="lsetInfo"></param>
        /// <param name="errMessageList"></param>
        /// <returns></returns>
        public override void CheckFile(LSETInfo lsetInfo)
		{

			try
			{
				CheckDirectory(lsetInfo);

				base.machineStatus = Constant.MachineStatus.Runtime;

                AlertLog alertLog = AlertLog.GetInstance();

                SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(this.LineCD);
				SettingInfo settingInfoCommon = SettingInfo.GetSingleton();
#if Debug
                string sLot = "";
#else
				//AI,MD,PLA,ECKはBlackJumbDogの起動確認
				if (settingInfoPerLine.BlackJumboDogFG == "ON")
				{
					if (!KLinkInfo.CheckBlackJumboDog())
					{
						throw new Exception(string.Format(Constant.MessageInfo.Message_14, this.LineCD));
					}
				}

                //マッピング有効ラインの場合、LENS2起動確認
                if (settingInfoCommon.LensFG)
                {
                    if (!KLinkInfo.CheckLENS())
                    {
                        throw new ApplicationException(string.Format("LENS2を起動して下さい。", this.LineCD));
                    }
                }

                string resMsg = string.Empty;
				KLinkInfo kLinkInfo = new KLinkInfo();

                //「受付準備OKをON」
                if (settingInfoCommon.IsMappingMode == false)
                {
                    resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 1, 11, Constant.ssuffix_U);
                    if (resMsg == "Error")
                    {
                        base.machineStatus = Constant.MachineStatus.Stop;
                        return;
                    }
                    System.Threading.Thread.Sleep(100);//0.1秒ready信号をON
                }
#endif
				if (settingInfoCommon.IsMappingMode == false && settingInfoPerLine.WBMappingFG == false)
				{
					string sType = GetType(ref tcp, ref ns, lsetInfo);

					if (string.IsNullOrEmpty(sType) == false)
					{
						if (settingInfoPerLine.IsNotSendZeroMapping(lsetInfo.EquipmentNO))
						{
						}
						else
						{
							ForcedZeroMappingProcess(ref tcp, ref ns, lsetInfo, sType);
						}

						SendTypeProcess(ref tcp, ref ns, lsetInfo.IPAddressNO, sType);
					}
				}

                System.Threading.Thread.Sleep(100);//0.1秒ready信号をON

				//対象ファイル種類を取得
				Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0);
				foreach (KeyValuePair<string, string> prefix in prefixList)
				{
#if Debug
                    //ファイルの情報登録・異常あれば通知
					//FileDistribution(ref tcp, ref ns, lsetInfo, prefix.Key, ref base.errorMessageList);
#else
					//ファイル取得要求
					resMsg = kLinkInfo.KLINK_GetKV_RD(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefix.Value, Constant.ssuffix_U);
					if (resMsg == "Error")
					{
						base.machineStatus = Constant.MachineStatus.Stop;
						return;
					}
					if (Convert.ToInt16(resMsg.ToString().Trim()) == 1)//ONの場合
					{
                        //「受付準備OKをOFF」
                        resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
                        if (resMsg == "Error")
                        {
                            base.machineStatus = Constant.MachineStatus.Stop;
                            return;
                        }

                        if (!FileFmt.IsStartFile(lsetInfo, prefix.Key))
                        {
                            FileInfo file = MachineFile.Search(lsetInfo.InputFolderNM, prefix.Key);

                            //ファイルアクセスに何らかの理由(ファイルロックや存在しない、etc）で失敗した場合はファイル処理を中止して処理を最初からやり直す
                            if (file == null)
                            {
                                return;
                            }

                            if (!MachineFile.IsThereLotNoInFileName(file.FullName))
                            {
                                return;
                            }

                            if (MachineFile.IsUnknownFile(file) == false)
                            {
                                if (settingInfoCommon.IsMappingMode && prefix.Value =="MM")
                                {
                                    MMFile mmFile = new MMFile(file);
                                    if (Database.LENS.WorkResult.IsComplete(mmFile.LotNo, mmFile.ProcNo, this.LineCD) == false)
                                    {
                                        //LENSの処理が完了していない間は待機
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                alertLog.logMessageQue.Enqueue(
                                    string.Format("ARMSによりロット特定されなかった為、傾向管理処理が行えませんでした。対象ﾌｧｲﾙ:{0}", file.FullName));
                            }
                        }

						//ファイルの情報登録・異常あれば通知
						FileDistribution(ref tcp, ref ns, prefix.Value, lsetInfo, prefix.Key, ref base.errorMessageList);

						//ファイル取得要求 立ち下げ
						resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefix.Value, 0, 1, Constant.ssuffix_U);
						if (resMsg == "Error")
						{
							base.machineStatus = Constant.MachineStatus.Stop;
							return;
						}
					}
#endif
				}
#if Debug
#else
                if (settingInfoCommon.IsMappingMode == false)
                {
                    //「受付準備OKをOFF」
                    resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
                    if (resMsg == "Error")
                    {
                        base.machineStatus = Constant.MachineStatus.Stop;
                        return;
                    }
                }
#endif
			}
			catch (Exception err)
			{
				throw;
			}
			finally
			{
				if (ns != null) { ns.Close(); }
				if (tcp != null) { tcp.Close(); }
			}
		}

		private void FileDistribution(ref TcpClient tcp, ref NetworkStream ns, string fileGetReqFgAddr, LSETInfo lsetInfo, string sTargetFile, ref List<ErrMessageInfo> errMessageList)
		{
			try
			{
				string sfname = "";
				string sWork = "";
				string sFileType = "";
				string[] textArray = new string[] { };
				string spath1 = "", spath2 = "";
				string sMessage = "";

				List<string> fileList = GetMachineLogPathList(lsetInfo, ref tcp, ref ns, fileGetReqFgAddr, lsetInfo.InputFolderNM, sTargetFile + ".*");

				int nSameTargetFileNum = fileList.Count;

				string[] sortedCreateTime = new string[nSameTargetFileNum];
				int i = 0;

				//ターゲットファイルが複数あった場合
				if (nSameTargetFileNum > 1)
				{
					i = 0;
					sMessage = lsetInfo.AssetsNM + "/TargetFile=" + sTargetFile + "/[" + nSameTargetFileNum + "]ありました。";
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

					//ファイル名から日付文字列取得
					foreach (string swfname in fileList)
					{
						sfname = swfname.Substring(lsetInfo.InputFolderNM.Length, swfname.Length - lsetInfo.InputFolderNM.Length);      //ファイル名取得
						sortedCreateTime[i] = sfname.Substring(9, 10);//ﾌｧｲﾙ名に付加されている日付文字列取得
						i = i + 1;
					}
					Array.Sort(sortedCreateTime);

					for (i = 0; i < nSameTargetFileNum - 1; i++)
					{
						foreach (string swfname in Common.GetFiles(lsetInfo.InputFolderNM, sTargetFile + sortedCreateTime[i] + ".*"))
						{
							//ファイル名に付いている日付を確認して、最新ファイル以外は未登録場所へ移動する。
							sfname = swfname.Substring(lsetInfo.InputFolderNM.Length, swfname.Length - lsetInfo.InputFolderNM.Length);      //ファイル名取得
							spath1 = lsetInfo.InputFolderNM + "reserve";
							spath2 = spath1 + "\\" + sfname;

							if (!Directory.Exists(spath1))
							{
								Directory.CreateDirectory(spath1);
							}
							else
							{
								if (File.Exists(spath2))
								{
									//登録済みﾌｧｲﾙは削除して次へ
									File.Delete(swfname);
									continue;
								}
							}
							//<--人搬送量試時にモールド機が夜間に止まった対応2 2010/09/03 Y.Matsushima
							bool flg;
							for (int j = 0; j < 5; j++)
							{
								flg = GetFileInfo(swfname, lsetInfo.InputFolderNM, ref sfname, ref sFileType, ref sWork);
								//成功したら出る
								if (flg == true)
								{
									break;
								}
								//失敗経験あればLogへ出力
								if (j > 1)
								{
									sMessage = Convert.ToString(j) + "回 GetFileInfoで失敗がありました。";
									log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
								}
							}
							//-->人搬送量試時にモールド機が夜間に止まった対応2 2010/09/03 Y.Matsushima

							if (File.Exists(spath2) == false)
							{
								File.Move(swfname, spath2);
							}
							else//既にある場合、delete
							{
								File.Delete(swfname);
							}
						}
					}
				}

				//-->マッピング不一致対策 発生していないが可能性がある為

				//「LogファイルデータをDB登録
				List<string> machineFileList = MachineFile.GetPathList(lsetInfo.InputFolderNM, ".*_" + sTargetFile);
				foreach (string swfname in machineFileList)
				{
					FileInfo fileInfo = new FileInfo(swfname);

					//<--人搬送量試時にモールド機が夜間に止まった対応 2010/07/28 Y.Matsushima
					bool flg;
					for (int j = 0; j < 5; j++)
					{
						flg = GetFileInfo(fileInfo.FullName, lsetInfo.InputFolderNM, ref sfname, ref sFileType, ref sWork);
						//成功したら出る
						if (flg == true)
						{
							break;
						}
						//失敗経験あればLogへ出力
						if (j > 1)
						{
							sMessage = Convert.ToString(j) + "回 GetFileInfoで失敗がありました。";
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

						}
					}
					//-->人搬送量試時にモールド機が夜間に止まった対応 2010/07/28 Y.Matsushima

					textArray = sWork.Split('\n');
					//項目のみの空ﾌｧｲﾙの場合削除
					if (textArray[2] == "")
					{
						File.Delete(fileInfo.FullName);
						continue;
					}

					string lotNo = string.Empty;

					//ﾌｧｲﾙﾀｲﾌﾟ毎にﾃﾞｰﾀﾍﾞｰｽ登録
					switch (sFileType)
					{
						case "MM":/*3*/
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}", lsetInfo.EquipmentNO, "[START]AI:MM File"));

                            if (fileInfo.Name.Contains("OPENINGCHECK"))
                            {
                                MoveOpeningCheckFile(fileInfo.FullName);
                                break;
                            }
                            
                            MMFile9Cam mmFile = new MMFile9Cam(this);
							mmFile.Run(lsetInfo, textArray, ref errMessageList, TimingNO);
							lotNo = mmFile.LotNo;
                            OpeningFileProcess(mmFile.OpeningCheckFileType, fileInfo.FullName);
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}", lsetInfo.EquipmentNO, "[END]AI:MM File"));
							break;
						case "OA":/*2*/
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}", lsetInfo.EquipmentNO, "[START]AI:OA File"));
							OAFile9Cam oaFile = new OAFile9Cam(lsetInfo.InlineCD);
							oaFile.Run(lsetInfo, textArray, ref errMessageList, FILE_OA_KEIKOUFG_COL, FILE_OA_OPENINGCHECKFG_COL);
							lotNo = oaFile.LotNo;

							OpeningFileProcess(oaFile.OpeningCheckFileType, fileInfo.FullName);
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}", lsetInfo.EquipmentNO, "[END]AI:OA File"));
							break;
						case "SM":/*1*/
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}", lsetInfo.EquipmentNO, "[START]AI:SM File"));

                            if (fileInfo.Name.Contains("OPENINGCHECK"))
                            {
                                MoveOpeningCheckFile(fileInfo.FullName);
                                break;
                            }

							SMFile9Cam smFile = new SMFile9Cam(lsetInfo.InlineCD);
							smFile.Run(lsetInfo, textArray, ref errMessageList, "SM", SMFile9Cam.FILE_SM_KEIKOUFG, SMFile9Cam.COL_OPENINGCHECKFG, null);
							lotNo = smFile.LotNo;

							OpeningFileProcess(smFile.OpeningCheckFileType, fileInfo.FullName);
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("設備：{0}/ロット：{1}", lsetInfo.EquipmentNO, "[END]AI:SM File"));
							break;
					}

					//処理済みファイルを保管フォルダへ移動
					MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, lotNo, "");
				}
			}
			catch (Exception err)
			{
				throw;
			}
		}
	}

	class MMFile9Cam : MMFile
	{
		/// <summary>ファイルMMアドレス列(傾向管理フラグ)[9カム出力仕様対応]()</summary>
		private const int COL_ADDRESS = -1;//未使用

		/// <summary>ファイルMM内容列(傾向管理フラグ)[9カム出力仕様対応]</summary>
		private const int COL_KEIKOUFG = 4;

		/// <summary>ファイルMMログデータ開始列(傾向管理フラグ)[9カム出力仕様対応]</summary>
		private const int COL_DATASTART = 5;
		
		private const int COL_OPENINGCHECKFG = 23;

		private const int COL_CHANGEFG = 24;

		private const int CAM_NUMBER = 9;
		public Constant.OpeningCheckFileType OpeningCheckFileType { get; set; }

		public MMFile9Cam(AIMachineInfo instance) : base(instance)
        {
             aiMachineInfo = instance;
			 OpeningCheckFileType = Constant.OpeningCheckFileType.UnDefined;
             this.LineCD = aiMachineInfo.LineCD;
        }
        /// <summary>
        /// ログファイル処理
        /// </summary>
        /// <param name="lsetInfo">設備情報</param>
        /// <param name="fileLineValueList">ファイル内容</param>
        /// <param name="errMessageList">エラー情報</param>
        public void Run(LSETInfo lsetInfo, string[] fileLineValueList, ref List<ErrMessageInfo> errMessageList, int timingNO)
        {
			AlertLog alertLog = AlertLog.GetInstance();
            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            //MMファイル情報取得
			List<MMFile> allDataList = GetData(fileLineValueList.ToList());
            
			if (OpeningCheckFileType != Constant.OpeningCheckFileType.NotOpeningCheckFile)
			{
				return;
			}

            MagInfo magInfo = GetMagazineInfo(lsetInfo, fileLineValueList, FILE_MAGAZINEROW, FILE_MAGAZINECOL);

            this.LotNo = magInfo.sNascaLotNO;

			if (allDataList.Count == 0)
			{
				throw new ApplicationException(string.Format("MMファイル内からデータ行を取得できませんでした。設備：{0}", lsetInfo.EquipmentCD));
			}
			else if (allDataList.Last().MeasureDT.HasValue == false)
			{
				throw new ApplicationException(string.Format("MMファイル内から測定日時を取得できませんでした。設備：{0}", lsetInfo.EquipmentCD));
			}

			DateTime lastMeasureDT = allDataList.Last().MeasureDT.Value;

			List<MMFile> dataList = GetNgInspectionData(allDataList);

            if (string.IsNullOrEmpty(magInfo.sNascaLotNO))
            {
                throw new Exception(string.Format(Constant.MessageInfo.Message_85, magInfo.sMagazineNO));
            }

            //閾値判定+NASCA不良判定
            LimitCheck(lsetInfo, magInfo, dataList, lastMeasureDT, ref errMessageList, timingNO);
        }

		public string GetLotNoFromFile(string[] fileLineValueList)
		{
			MagInfo magInfo = MachineBase.GetMagazineInfo(lsetInfo, fileLineValueList, FILE_MAGAZINEROW, FILE_MAGAZINECOL);
			return magInfo.sNascaLotNO;
		}

		/// ファイル情報を取得
		/// </summary>
		/// <param name="fileValue">ファイル内容</param>
		/// <returns>ファイル情報</returns>
		public List<MMFile> GetData(List<string> fileValue)
		{
			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(aiMachineInfo.LineCD);

			List<MMFile> mmDataList = new List<MMFile>();
			int openingCheckRecordCt = 0;
			int recordCt = 0;

			for (int i = ROW_DATASTART; i < fileValue.Count; i++)
			{
				if (fileValue[i] == "")
				{
					//空白行の場合、次へ
					continue;
				}
				//空白行以外の行数カウント
				recordCt++;

				string[] fileLineValue = fileValue[i].Split(',');

				if (fileLineValue[COL_OPENINGCHECKFG].Trim() == AIMachineInfo.OPENINGCHECKFG_ON)
				{
					//オープニングチェックフラグOnの行数カウント
					openingCheckRecordCt++;
					continue;
				}

				if (fileLineValue[COL_KEIKOUFG].Trim() == AIMachineInfo.KEIKOUFG_OFF)
				{
					//傾向管理しないデータの場合、次へ
					continue;
				}

				for (int colAddress = COL_DATASTART; colAddress < 2 * CAM_NUMBER + COL_DATASTART; colAddress += 2)
				{
					MMFile mmData = new MMFile(aiMachineInfo);
					mmData.MeasureDT = Convert.ToDateTime(fileLineValue[COL_DAY] + " " + fileLineValue[COL_TIME]);
					mmData.AddressNO = Convert.ToInt32(fileLineValue[colAddress].Trim());
					mmData.TranCD = fileLineValue[colAddress+1].Trim();

					if (fileLineValue[COL_CHANGEFG].Trim() == AIMachineInfo.CHANGEFG_ON)//settingInfo.WBMappingFG==trueもand条件で存在したが、四宮氏と相談し不要との事で削除 (2014/1/31 n.yoshimoto)
					{
                        //変化点フラグONの場合
						mmData.ChangePointFG = true;
					}

					int targetIndex = mmDataList.FindIndex(m => m.AddressNO == mmData.AddressNO);
					if (targetIndex == -1)
					{
						mmDataList.Add(mmData);
					}
					else
					{
						//同一アドレスデータがある場合、最後尾を優先する
						mmDataList[targetIndex] = mmData;
					}
				}
			}

			if ( openingCheckRecordCt == recordCt && openingCheckRecordCt > 0)
			{
				this.OpeningCheckFileType = Constant.OpeningCheckFileType.OpeningCheckFile;
			}
			else if (openingCheckRecordCt != recordCt && openingCheckRecordCt > 0)
			{
				this.OpeningCheckFileType = Constant.OpeningCheckFileType.ErrorFile;
			}
			else if (openingCheckRecordCt == 0)
			{
				this.OpeningCheckFileType = Constant.OpeningCheckFileType.NotOpeningCheckFile;
			}

			return mmDataList;
		}
	}

	class OAFile9Cam
	{
		/// <summary>ファイル内容(日付)</summary>
		private const int FILE_DAY = 1;

		/// <summary>ファイル内容(時間)</summary>
		private const int FILE_TIME = 2;

		/// <summary>ファイル内容マガジンNO行</summary>
		private const int FILE_MAGAZINEROW = 2;

		/// <summary>ファイル内容マガジンNO列</summary>
		private const int FILE_MAGAZINECOL = 3;

		/// <summary>ファイル内容開始行</summary>
		private const int FILE_DATASTARTROW = 2;


		/// <summary>ファイルOA内容列(変化点フラグ)</summary>
		private const int FILE_OA_CHANGEFG = 8;

		/// <summary>ファイル内容(傾向管理フラグOFF)</summary>
		private const string KEIKOUFG_OFF = "0";

		/// <summary>ファイル内容(変化点フラグOFF)</summary>
		private const string CHANGEFG_OFF = "0";

        public string LotNo { get; set; }

		//private AIMachineInfo9Cam aiMachineInfo;

		public int LineCD { get; set; }
		public string MeasureDT { get; set; }
		public double TargetVAL { get; set; }
		public bool SubmitFG { get; set; }
		public Constant.OpeningCheckFileType OpeningCheckFileType { get; set; }

		public OAFile9Cam(int lineCD)
        {
			 OpeningCheckFileType = Constant.OpeningCheckFileType.UnDefined;
             this.LineCD = lineCD;
        }

	    //[OA:装置実行ﾊﾟﾗﾒｰﾀ]
        public void Run(LSETInfo lsetInfo, string[] fileLineValueList, ref List<ErrMessageInfo> errMessageList, int keikouFgColIndex, int openingChkColIndex)
        {
			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            OAFile9Cam oaFile = GetData(fileLineValueList.ToList(), keikouFgColIndex, openingChkColIndex);
            if (OpeningCheckFileType != Constant.OpeningCheckFileType.NotOpeningCheckFile)
            {
                return;
            }

            MagInfo magInfo = MachineBase.GetMagazineInfo(lsetInfo, fileLineValueList, FILE_MAGAZINEROW, FILE_MAGAZINECOL);

            this.LotNo = magInfo.sNascaLotNO;

            //OAファイルの紐付けマスタ情報(TmFILEFMT)を取得
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(AIMachineInfo.FileType.OA), lsetInfo, magInfo.sMaterialCD);
            if (filefmtList.Count == 0)
            {
                //設定されていない場合、装置処理停止
				string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, Convert.ToString(AIMachineInfo.FileType.OA));
                throw new Exception(message);
            }

            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                //閾値マスタ情報(TmPLM)取得
				Plm plmInfo = Plm.GetData(this.LineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

                if (plmInfo == null)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                    throw new Exception(message);
                }

				oaFile = GetData(fileLineValueList.ToList(), filefmtInfo, keikouFgColIndex, openingChkColIndex);

                //異常判定+DB登録
                ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, Convert.ToString(oaFile.TargetVAL), oaFile.MeasureDT, ref errMessageList);
            }
        }

        /// <summary>
        /// ファイル情報を取得
        /// </summary>
        /// <param name="fileValue">ファイル内容</param>
        /// <returns>ファイル情報</returns>
		public OAFile9Cam GetData(List<string> fileValue, FILEFMTInfo filefmtInfo, int keikouFgColIndex, int openingChkColIndex)
		{
			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);
			OAFile9Cam oaFile = new OAFile9Cam(this.LineCD);
			int openingCheckRecordCt = 0;
			int recordCt = 0;

			for (int i = FILE_DATASTARTROW; i < fileValue.Count; i++)
			{
				if (fileValue[i] == "")
				{
					//空白行の場合、次へ
					continue;
				}
                //空白行以外の行数カウント
                recordCt++;

				string[] fileLineValue = fileValue[i].Split(',');

				if (fileLineValue[openingChkColIndex].Trim() == AIMachineInfo.OPENINGCHECKFG_ON)
				{
					//オープニングチェックフラグOnの行数カウント
					openingCheckRecordCt++;
					continue;
				}

				if (fileLineValue[keikouFgColIndex].Trim() == KEIKOUFG_OFF)
				{
					//傾向管理しないデータの場合、次へ
					continue;
				}
				//車載では全数検査の為使用しない（OAFileクラスをどこかのタイミングで作成する場合、このコメントアウトは解除して作成する）
				//if (settingInfo.WBMappingFG)
				//{
				//    if (fileLineValue[FILE_OA_CHANGEFG].Trim() == CHANGEFG_OFF)
				//    {
				//        //変化点フラグOFFの場合、次へ
				//        continue;
				//    }
				//}
                               
                oaFile.MeasureDT = Convert.ToString(Convert.ToDateTime(fileLineValue[FILE_DAY] + " " + fileLineValue[FILE_TIME]));

                if (filefmtInfo != null)
                {
					oaFile.TargetVAL = MachineFile.CalcChangeUnit(this.LineCD, filefmtInfo.QCParamNO, Convert.ToDouble(fileLineValue[filefmtInfo.ColumnNO]));
                }

                oaFile.SubmitFG = true;
			}

			if (openingCheckRecordCt == recordCt && openingCheckRecordCt > 0)
			{
				this.OpeningCheckFileType = Constant.OpeningCheckFileType.OpeningCheckFile;
			}
			else if (openingCheckRecordCt != recordCt && openingCheckRecordCt > 0)
			{
				this.OpeningCheckFileType = Constant.OpeningCheckFileType.ErrorFile;
			}
			else if (openingCheckRecordCt == 0)
			{
				this.OpeningCheckFileType = Constant.OpeningCheckFileType.NotOpeningCheckFile;
			}

			return oaFile;
		}
        public OAFile9Cam GetData(List<string> fileValue, int keikouFgColIndex, int openingChkColIndex) 
        {
            return GetData(fileValue, null, keikouFgColIndex, openingChkColIndex);
        }
	}
     
	class SMFile9Cam
	{
		/// <summary>ファイル内容(日付)</summary>
		private const int FILE_DAY = 1;

		/// <summary>ファイル内容(時間)</summary>
		private const int FILE_TIME = 2;

		/// <summary>ファイル内容マガジンNO行</summary>
		private const int FILE_MAGAZINEROW = 2;

		/// <summary>ファイル内容マガジンNO列</summary>
		private const int FILE_MAGAZINECOL = 3;

		/// <summary>ファイル内容開始行</summary>
		private const int FILE_DATASTARTROW = 2;

		/// <summary>ファイルSM内容列(傾向管理フラグ)</summary>
		public const int FILE_SM_KEIKOUFG = 4;

		/// <summary>ファイルSM内容列(変化点フラグ)</summary>
		private const int FILE_SM_CHANGEFG = 55;

		public const int COL_OPENINGCHECKFG = 27;

		/// <summary>ファイル内容(傾向管理フラグOFF)</summary>
		private const string KEIKOUFG_OFF = "0";

		/// <summary>ファイル内容(変化点フラグOFF)</summary>
		private const string CHANGEFG_OFF = "0";

		/// <summary>ファイル内容異常値</summary>
		private const double FILE_ERRORVAL = -999000;

        public string LotNo { get; set; }

		public int LineCD { get; set; }
		List<double> valueList = new List<double>();
		public string MeasureDT { get; set; }
		public double TargetVAL { get; set; }
		public Constant.OpeningCheckFileType OpeningCheckFileType { get; set; }

		public SMFile9Cam(int lineCD)
        {
			 OpeningCheckFileType = Constant.OpeningCheckFileType.UnDefined;
             this.LineCD = lineCD;
        }

        //[SM:製品出来栄え]
        public void Run(LSETInfo lsetInfo, string[] fileLineValueList, ref List<ErrMessageInfo> errMessageList, string prefixNM, int keikouFgColIndex, int openingChkColIndex, int? changeFgColIndex)
        {
			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			SMFile smFile = SMFile.GetData(fileLineValueList.ToList(), settingInfo.WBMappingFG, openingChkColIndex, keikouFgColIndex, changeFgColIndex);
            OpeningCheckFileType = smFile.OpeningCheckFileType;

            if (OpeningCheckFileType != Constant.OpeningCheckFileType.NotOpeningCheckFile)
            {
                return;
            }

			MagInfo magInfo = MachineBase.GetMagazineInfo(lsetInfo, fileLineValueList, FILE_MAGAZINEROW, FILE_MAGAZINECOL);

            this.LotNo = magInfo.sNascaLotNO;

            //SMファイルの紐付けマスタ情報(TmFILEFMT)を取得
			List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(prefixNM, lsetInfo, magInfo.sMaterialCD);
            if (filefmtList.Count == 0)
            {
				List<FileFmt> noChkFileFmt = FileFmt.GetData(lsetInfo, null, prefixNM, Constant.NO_CHK_QCPARAM_NO);

				if (noChkFileFmt.Count > 0)
				{
					return;
				}

                //設定されていない場合、装置処理停止
				string message = string.Format(Constant.MessageInfo.Message_27, magInfo.sMaterialCD, prefixNM);
                throw new Exception(message);
            }
            foreach (FILEFMTInfo filefmtInfo in filefmtList)
            {
                //閾値マスタ情報(TmPLM)取得
				Plm plmInfo = Plm.GetData(this.LineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

                if (plmInfo == null)
                {
                    //設定されていない場合、装置処理停止
                    string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
                    throw new Exception(message);
                }

                FileValueInfo fileValueInfo = new FileValueInfo();

                Database.LENS.Magazine magConfig = Database.LENS.Magazine.GetData(magInfo.sMaterialCD, this.LineCD);

				//ファイル内容取得
				smFile = SMFile.GetData(fileLineValueList.ToList(), filefmtInfo.ColumnNO, magConfig.SamplingModeID, magConfig.SamplingCT, settingInfo.WBMappingFG, openingChkColIndex, keikouFgColIndex, changeFgColIndex);

				OpeningCheckFileType = smFile.OpeningCheckFileType;

                if (smFile.ValueList.Count == 0) 
                {
                    continue;
                }

                double rValue = 0;

				string calcDataStr = MachineFile.GetCalculatedResult(plmInfo, smFile.ValueList.ToArray(), lsetInfo.InlineCD);

                //異常判定+DB登録
				ConnectDB.InsertTnLOG(lsetInfo, plmInfo, magInfo, calcDataStr, smFile.MeasureDT, ref errMessageList);
            }
        }
	}
}
