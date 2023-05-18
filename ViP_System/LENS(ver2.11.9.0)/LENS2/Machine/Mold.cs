using LENS2_Api;
using PLC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LENS2.Machine
{
	/// <summary>
	/// モールド
	/// </summary>
	class Mold : MachineBase, IMachine
	{
		//定数とか増えてきたらpartialで他ファイルに逃がしても良いかも
		private const string NO_DIVISION_MAPPING_MODE_ENABLE = "EM40073";
		public const string MAPPING_START_ADDRESS = "ZF80000";
        public const string BADMARK_MAPPING_START_ADDRESS = "ZF120000";
        public const string BADMARK_MAPPING_REQUEST_ADDRESS = "EM40074";

        public string HeartBeatAddress { get; set; }
		public string MachineStopAddress { get; set; }
		public string WrongMappingAddress { get; set; }
		public string MappingRequestAddress { get; set; }
		public string MoldDirectionInfoAddress { get; set; }
		public string SyringeNumberInfoAddress { get; set; }
		public string MemoryTypeInfoAddress { get; set; }
		
		public string LoaderQrdataAddress { get; set; }
		public string CompleteOutputMachineLogAddress { get; set; }
        
        //public string BadMarkMappingRequestAddress { get; set; }

        public List<string> StartAddressPerSyringe { get; set; }
		
		public List<string> LRFrontSideStartAddressPerSyringe { get; set; }
		public List<string> LRBackSideStartAddressPerSyringe { get; set; }
		public List<MainteTargetMachineAddress> mainteTargetMachineAddress { get; set; }

		public List<string> PlantListForSendPerSyringe { get; set; }

		public enum MappingDataKind
		{
			InspectedMapping,
			WireBondedMapping,
		}
        //<--SGA-IM0000007627 MD装置SDカード内不要データ抽出
        // ファイル内のマガジン番号取得
        public const int SD_MAGNO_COL_INDEX = 3;
        public const int SD_KEIKOUFG_COL_INDEX = 18;
        //-->SGA-IM0000007627 MD装置SDカード内不要データ抽出
        struct MachineSpec
		{
			/// <summary>樹脂塗布方向CD</summary>
			public int MoldDirectionCD { get; set; }
			/// <summary>シリンジ本数CD</summary>
			public int SyringeNumberCD { get; set; }
			public int SyringeQty { get; set; }
			/// <summary>メモリ種類CD</summary>
			public int MemoryTypeCD { get; set; }

			/// <summary>樹脂の塗布方向 1:フレーム搬送方向と同じ 2:フレーム搬送方向に対して垂直</summary>
			public const int HORIZONTAL_DIRECTION_CD = 1;
			public const int VERTICAL_DIRECTION_CD = 2;

			/// <summary>シリンジ本数 1:5本仕様 2:4本仕様</summary>
			public const int FIVE_SYRINGE_CD = 1;
			public const int FOUR_SYRINGE_CD = 2;
			public const int EIGHT_SYRINGE_CD = 3;

			/// <summary>メモリ種類 1:LRメモリ 2:ZFメモリ</summary>
			public const int LR_MEMORY_CD = 1;
			public const int ZF_MEMORY_CD = 2;


			/// <summary>モールド動作パターン(横)進行方向</summary>
			public enum MoldMovement
			{
				/// <summary>右</summary>
				Right,
				/// <summary>左</summary>
				Left,
			}

			public static int GetSyringeQuantity(int syringeNumberCD)
			{
				if (syringeNumberCD == FIVE_SYRINGE_CD)
				{
					return 5;
				}
				else if (syringeNumberCD == FOUR_SYRINGE_CD)
				{
					return 4;
				}
				else if (syringeNumberCD == EIGHT_SYRINGE_CD)
				{
					return 8;
				}
				else
				{
					throw new ApplicationException(string.Format("装置から取得したシリンジ本数CDが正しくありません。シリンジ本数CD：{0}", syringeNumberCD));
				}
			}
		}

		/// <summary>
		/// MDディスペンスタイム情報
		/// </summary>
		public class MdDispenseInfo
		{
			/// <summary>シリンジNO</summary>
			public int SyringeNO { get; set; }

			/// <summary>段数</summary>
			public int StepNO { get; set; }

			/// <summary>モールド打順</summary>
			public int MoldOrder { get; set; }
		}

		// 共通化出来る処理は共通のルーチンへ!!!
		// 外部システム・装置との連携部の処理にはきめ細かなログ出力を!!!

		public Mold(string plcIpAddress, int plcPort) { }

		public void MainWork()
		{
			try
			{
				// ハートビートON
				setHeartBeat(true);

				string magazineNo = string.Empty;

				try
				{
					// マッピングデータ要求監視
					if (haveRequest())
					{
						magazineNo = base.GetHoldMagazineLotNo(this.LoaderQrdataAddress);
						// 装置のQRデータからロット情報取得
						Lot lot = Lot.GetData(magazineNo);

                        // モールド作業の工程No = ロット番号 or ロット番号_#2の作業中実績の工程No 
                        int mdProcNo = LENS2_Api.ARMS.WorkResult.GetCurrentWorkingProcNo(lot.LotNo, 2);

                        InfoLog(string.Format("[開始時] マッピング処理開始(工程No = {0})", mdProcNo), lot.LotNo);

						// マッピングデータ送信
                        mappingDataSendProcess(lot, mdProcNo);
                        InfoLog(string.Format("マッピングデータ送信完了(工程No = {0})", mdProcNo), lot.LotNo);

						clearRequest();

						// NAMI処理済フラグON
                        WorkResult.CompleteStartProcess(lot.LotNo, mdProcNo, this.NascaPlantCD);
                        //WorkResult.CompleteStartProcess(lot.LotNo, Config.Settings.MoldProcNo, this.NascaPlantCD);
                        InfoLog(string.Format("[開始時] マッピング処理完了(工程No = {0})", mdProcNo), lot.LotNo);
					}
				}
				catch (Exception err)
				{
					throw new Exception(string.Format(
						"正しくマッピングされない可能性がある為、装置から作業開始しようとしているマガジン({0})を取り除き、再投入して下さい。\r\n\r\n{1}", magazineNo, err.Message));
				}

				// SDファイル監視

				string sdFilePath = string.Empty;
                Lot sdFileLot = null;

				try
				{
					if (isMachineLogOutputCompletion())
					{
						// ハートビートOFF
						setHeartBeat(false);

						FileInfo sd = MachineLog.GetLatest(this.WatchingDirectoryPath, SDFile.FILE_KIND, SDFile.DATESTARTINDEX_FROM_SD_ON_FILENAME);
						
						if (sd == null)
						{
							return;

							//↓↓↓下2行追加 2016/1/5 nyoshimoto
							//ファイル取得要求があり、ファイルが存在しない場合は
							//LENSで対処せず装置のサイクルタイムアウトに任せる
							//↑↑↑2016/1/5 nyoshimoto//////////////////////////
						}

						sdFilePath = sd.FullName;

						if (!SDFile.IsFileNameAddLot(sd))
						{
							// ARMS処理待ち(ファイル名にロット情報付与)
							return;
						}

						SDFile sdFile = new SDFile(sd, this);
						if (SDFile.IsUnknownFileName(sdFile))
						{
							// SDファイル内容がヘッダのみの空ファイルを想定　処理をスルーしてEICSに削除処理を任せる
							return;
						}
                        sdFileLot = Lot.GetData(sdFile.LotNo);

                        // モールド作業の工程No = ロット番号 or ロット番号_#2の作業中実績の工程No 
                        int mdProcNo = LENS2_Api.ARMS.WorkResult.GetCurrentWorkingProcNo(sdFile.LotNo, 2);

                        if (LENS2_Api.WorkResult.IsCompleteEndProcess(sdFile.LotNo, sdFile.ProcNo))
                        {
							// LENS処理済み
							return;
						}

                        if (IsMoldMappingProcess(mdProcNo))
						{
							InfoLog("[完了時] マッピング処理開始", sdFile.LotNo);
							checkMappingAfterMold(sdFile, mdProcNo);

                            MappingFile.MoveToBackupDir(sdFile.LotNo);
						}

						// LENS処理済フラグON(ARMS, EICS処理開始)
                        LENS2_Api.WorkResult.CompleteEndProcess(sdFile.LotNo, sdFile.ProcNo, this.NascaPlantCD);
                        InfoLog("[完了時] マッピング処理完了", sdFile.LotNo);
						
					}
				}
				catch (Exception err)
				{
                    // SDファイルからロットNoを取得した上でのエラーの場合は、ARMS流動規制を掛ける
                    if(sdFileLot != null)
                    {
                        int mdProcNo = LENS2_Api.ARMS.WorkResult.GetCurrentWorkingProcNo(sdFileLot.LotNo, 2);
                        InsertRestrictMappingNgAfterMold(sdFileLot, mdProcNo);
                    }
					throw new Exception(string.Format(
                        "【マッピング照合不一致】正しくMD後照合が行えていない為、装置から排出しようとしているマガジンの樹脂少箇所が正しいか確認して下さい。\r\nSDファイル{0}\r\n\r\n{1}", sdFilePath, err.Message));
				}
				// ハートビートOFF
				setHeartBeat(false);
			}
			catch(Exception err)
			{
				throw new MachineException(this.ClassNM, this.MachineNM, err);
			}
		}

        // マッピングデータ送信
        private void mappingDataSendProcess(Lot lot, int mdProcNo)
        {
            Dictionary<int, string> mappingData = new Dictionary<int, string>();
            
            // マガジンを構成するパッケージ数やフレームのパッケージ数等の取得
            Magazine.Config magConfig = Magazine.Config.GetData(lot.TypeCd);

            // 装置から情報取得（メモリ種類（ZF）、シリンジ数、打ち方（縦横））
            MachineSpec machineSpec = getMachineSpec();

            if (IsMoldMappingProcess(mdProcNo))
            {
                InfoLog("マッピングデータ使用モールド", lot.LotNo);
                // MD用マッピングデータ取得
                mappingData = getMappingDataForMold(lot, magConfig, mdProcNo);

                // MD前マッピングデータ照合
                verifyMappingBeforeMold(lot, magConfig, mappingData, mdProcNo);
            }
            else
            {
                InfoLog("マッピングデータ不使用モールド（全点0マッピング）", lot.LotNo);
                // 0埋めマッピングデータ取得		
                mappingData = MappingFile.Data.GetAnySize(magConfig.TotalMagazinePackage, MappingFile.Data.OK_VAL);
            }
            
            if (haveBadMarkMappingRequest() == true)
            {
                InfoLog("MD装置のBMマッピング要求ON マッピングデータの分解実施あり", lot.LotNo);
            }
            else
            {
                InfoLog("MD装置のBMマッピング要求OFF マッピングデータの分解実施なし", lot.LotNo);
            }
            sendMapping(magConfig, machineSpec, mappingData);
        }

        private static Dictionary<int, string> ConvertToSendMappingData1(Dictionary<int, string> original)
        {
            return ConvertToMappingData(original, "MD_IN", "MD_IN1");
        }

        private static Dictionary<int, string> ConvertToSendMappingData2(Dictionary<int, string> original)
        {
            return ConvertToMappingData(original, "MD_IN", "MD_IN2");
        }

        private static Dictionary<int, string> ConvertToMappingData(Dictionary<int, string> original, string originalcd, string convertcd)
        {
            Dictionary<int, string> retv = new Dictionary<int, string>();

            Dictionary<string, string> conv = MapConv.GetData(originalcd, convertcd);

            foreach (KeyValuePair<int, string> data in original)
            {
                retv.Add(data.Key, conv[data.Value]);
            }

            return retv;
        }

        #region 設備仕様取得処理

        //8連シリンジMDの検証完了後に削除可(2016/3/28 nyoshimoto)
        //private bool isSendPerSyringeMode(string plantCD)
        //{
        //	if (this.PlantListForSendPerSyringe == null || this.PlantListForSendPerSyringe.Count == 0)
        //	{
        //		return false;
        //	}

        //	return this.PlantListForSendPerSyringe.Contains(plantCD);
        //}

        private MachineSpec getMachineSpec()
		{
			MachineSpec machineSpec = new MachineSpec();

			machineSpec.MoldDirectionCD = getMoldDirectionCD();
			machineSpec.SyringeNumberCD = getSyringeNumberCD();
			machineSpec.SyringeQty = MachineSpec.GetSyringeQuantity(machineSpec.SyringeNumberCD);
			machineSpec.MemoryTypeCD = getMemoryTypeCD();

			return machineSpec;
		}

		/// <summary>MD装置から樹脂塗布方向のCDを取得</summary>
		/// <returns>1:フレーム搬送方向 2:フレーム搬送方向に対し垂直の方向</returns>
		private int getMoldDirectionCD()
		{
			string strMoldDirectionCD = Plc.GetData(MoldDirectionInfoAddress, 1);

			int moldDirectionCD;

			if (int.TryParse(strMoldDirectionCD, out moldDirectionCD) == false)
			{
				throw new ApplicationException(string.Format("装置から正しいデータが取得できません。塗布方向CD(={0})", strMoldDirectionCD));
			}

			if (moldDirectionCD == MachineSpec.HORIZONTAL_DIRECTION_CD)
			{
				return MachineSpec.HORIZONTAL_DIRECTION_CD;
			}
			else if (moldDirectionCD == MachineSpec.VERTICAL_DIRECTION_CD)
			{
				return MachineSpec.VERTICAL_DIRECTION_CD;
			}
			else
			{
				throw new ApplicationException(string.Format("装置から正しいデータが取得できません。塗布方向CD(={0})", strMoldDirectionCD));
			}
		}

		/// <summary>MD装置からシリンジ本数のCDを取得</summary>
		/// <returns>1:5本 2:4本</returns>
		private int getSyringeNumberCD()
		{
			string strSyringeNumberCD = Plc.GetData(SyringeNumberInfoAddress, 1);

			int syringeNumberCD;

			if (int.TryParse(strSyringeNumberCD, out syringeNumberCD) == false)
			{
				throw new ApplicationException(string.Format("装置から正しいデータが取得できません。シリンジ本数CD(={0})", strSyringeNumberCD));
			}

			if (syringeNumberCD == MachineSpec.FIVE_SYRINGE_CD)
			{
				return MachineSpec.FIVE_SYRINGE_CD;
			}
			else if (syringeNumberCD == MachineSpec.FOUR_SYRINGE_CD)
			{
				return MachineSpec.FOUR_SYRINGE_CD;
			}
			else if (syringeNumberCD == MachineSpec.EIGHT_SYRINGE_CD)
			{
				return MachineSpec.EIGHT_SYRINGE_CD;
			}
			else
			{
				throw new ApplicationException(string.Format("装置から正しいデータが取得できません。シリンジ本数CD(={0})", strSyringeNumberCD));
			}
		}

		/// <summary>MD装置からメモリ種類のCDを取得</summary>
		/// <returns>1:LR 2:ZF</returns>
		private int getMemoryTypeCD()
		{
			string strMemoryTypeCD = Plc.GetData(MemoryTypeInfoAddress, 1);

			int memoryTypeCD;

			if (int.TryParse(strMemoryTypeCD, out memoryTypeCD) == false)
			{
				throw new ApplicationException(string.Format("装置から正しいデータが取得できません。メモリタイプCD(={0})", strMemoryTypeCD));
			}

			if (memoryTypeCD == MachineSpec.LR_MEMORY_CD)
			{
				return MachineSpec.LR_MEMORY_CD;
			}
			else if (memoryTypeCD == MachineSpec.ZF_MEMORY_CD)
			{
				return MachineSpec.ZF_MEMORY_CD;
			}
			else
			{
				throw new ApplicationException(string.Format("装置から正しいデータが取得できません。メモリタイプCD(={0})", strMemoryTypeCD));
			}
		}

		#endregion

		internal Dictionary<int, string> getMappingDataForMold(Lot lot, LENS2_Api.Magazine.Config magConfig, int mdProcNo)
		{
			Dictionary<int ,string> mappingData = new Dictionary<int,string>();

			// 外観検査機を通す必要あり
			if (lot.HasPassedInspector)
			{
				InfoLog("AI通過ロット判定", lot.LotNo);
				//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(MAP) 外観検査機通過パターン"), lsetInfo.EquipmentNO);

				// MDからmpdデータ参照するにあたっての上下反転の必要性取得
                bool hasNeedReverseFramePlacement = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo
                    , Config.Settings.InspectProcNo, mdProcNo, false);
                HideLog(string.Format("【MD用マッピング取得】マッピングデータ(AI通過)の上下反転有無：{0} / ロットNo:{1} AI-Proc(Config):{2} MD-Proc(Config):{3}"
                    , hasNeedReverseFramePlacement, lot.LotNo, Config.Settings.InspectProcNo, mdProcNo));
                
                ////bool hasNeedReverseFramePlacement = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo
                ////    , Config.Settings.InspectProcNo, Config.Settings.MoldProcNo, false);
                //HideLog(string.Format("【MD用マッピング取得】マッピングデータ(AI通過)の上下反転有無：{0} / ロットNo:{1} AI-Proc(Config):{2} MD-Proc(Config):{3}"
                //    , hasNeedReverseFramePlacement, lot.LotNo, Config.Settings.InspectProcNo, Config.Settings.MoldProcNo));

				mappingData = MappingFile.Data.Inspector.Get(lot.LotNo, hasNeedReverseFramePlacement);

				mappingData = MappingFile.Data.Inspector.ConvertToMold(mappingData);

				//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(MAP) マッピングファイル発見:{0} ", mappingFileInfo.FullName), lsetInfo.EquipmentNO);
			}
			// 外観検査機を通す必要なし
			else
			{
				InfoLog("AI非通過ロット判定", lot.LotNo);
				//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マッピング情報提供(MAP) 外観検査機無しパターン"), lsetInfo.EquipmentNO);

				if (lot.IsMappingInspection)
				{
					InfoLog("WBマッピングフラグONロット", lot.LotNo);
					// MDからwbmデータ参照するにあたっての上下反転の必要性取得
					bool hasNeedReverseFramePlacement = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo,
						Config.Settings.WirebondProcNo, mdProcNo, false);
                    //bool hasNeedReverseFramePlacement = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo,
                    //    Config.Settings.WirebondProcNo, Config.Settings.MoldProcNo, false);

                    HideLog(string.Format("【MD用マッピング取得】マッピングデータ(AIスルー)の上下反転有無：{0} / ロットNo:{1} WB-Proc(Config):{2} MD-Proc(Config):{3}"
                        , hasNeedReverseFramePlacement, lot.LotNo, Config.Settings.WirebondProcNo, mdProcNo));

                    //HideLog(string.Format("【MD用マッピング取得】マッピングデータ(AIスルー)の上下反転有無：{0} / ロットNo:{1} WB-Proc(Config):{2} MD-Proc(Config):{3}"
                    //    , hasNeedReverseFramePlacement, lot.LotNo, Config.Settings.WirebondProcNo, Config.Settings.MoldProcNo));

					mappingData = MappingFile.Data.Wirebonder.Get(lot.LotNo, hasNeedReverseFramePlacement);

					// Wirebond後マッピングデータをMold用マッピングデータに処理CDを変換
					mappingData = MappingFile.Data.Wirebonder.ConvertToMold(mappingData);
				}
				else
				{
					InfoLog("WBマッピングフラグOFFロット", lot.LotNo);
					mappingData = MappingFile.Data.GetAnySize(magConfig.TotalMagazinePackage, MappingFile.Data.OK_VAL);
				}
			}

			return mappingData;
		}

        private void sendMapping(Magazine.Config magConfig, MachineSpec machineSpec, Dictionary<int, string> mappingData)
        {
			try
			{
                bool haveBadMarkMappingRequestFg = haveBadMarkMappingRequest();

                //設定ファイルでシリンジ毎にマッピング送信する必要があるのかどうかの管理をやめて、装置からモード情報を取得するようにしたので下記コメントアウト
                //2016/3/25 n.yoshimoto このコメント部は検証後に削除可能
                //if (isSendPerSyringeMode(this.NascaPlantCD))
                //{
                //	sendMappingPerSyringe(magConfig, machineSpec, mappingData);
                //}
                //else
                //{
                //	sendMappingWithNoDivision(mappingData);
                //}
                if (isNoDivisionMappingModeEnable())
				{
                    // 1： 樹脂少箇所, 2：樹脂吐出無し・樹脂量測定無し箇所
                    Dictionary<int, string> mappingData1 = ConvertToSendMappingData1(mappingData);
                    sendMappingWithNoDivision(mappingData1);
                    // BMマッピング要求がONの時、マッピングデータを分解する
                    if (haveBadMarkMappingRequestFg == true)
                    {
                        Dictionary<int, string> mappingData2 = ConvertToSendMappingData2(mappingData);
                        sendMappingWithNoDivision(mappingData2, BADMARK_MAPPING_START_ADDRESS);
                    }
                }
				else
				{
                    // 現在のMD装置は isNoDivisionMappingModeEnableがFalseになる事は無い為、
                    // こっち用の送信先アドレスはMD装置では使用されていない為、BMマッピング送信処理は
                    // こちらには実装しない。
                    // ただし、もしこっちに来た時の為に、BMマッピング箇所 → 樹脂少箇所への変換処理を記載しておく。
                    if (haveBadMarkMappingRequest())
                    {
                        string logMsg = $"MDデータ送信時にアドレス『{NO_DIVISION_MAPPING_MODE_ENABLE}』がOFFの状態で";
                        logMsg += $"BMマッピング要求アドレス『{BADMARK_MAPPING_REQUEST_ADDRESS}』がONになった為、";
                        logMsg += $"BMマッピング箇所を樹脂少箇所に変換します。";
                        InfoLog(logMsg);

                        // 照合用マッピングデータを(MD_IN - MD_IN1)と(MD_IN - MD_IN2)の変換データの和集合に変換
                        Dictionary<int, string> mappingDataList1 = ConvertToSendMappingData1(mappingData);
                        Dictionary<int, string> mappingDataList2 = ConvertToSendMappingData2(mappingData);
                        mappingData = MappingFile.Data.Union(mappingDataList1, mappingDataList2);
                    }

                    sendMappingPerSyringe(magConfig, machineSpec, mappingData);
				}

			}
            catch (Exception)
			{
				// 停止要求
				stopMachine();

				// マッピング不一致
				notifyWrongMapping();
				
				throw;
			}
		}

        //private void sendMapping(Magazine.Config magConfig, MachineSpec machineSpec, Dictionary<int, string> mappingData)
        //{
        //    try
        //    {
        //        //設定ファイルでシリンジ毎にマッピング送信する必要があるのかどうかの管理をやめて、装置からモード情報を取得するようにしたので下記コメントアウト
        //        //2016/3/25 n.yoshimoto このコメント部は検証後に削除可能
        //        //if (isSendPerSyringeMode(this.NascaPlantCD))
        //        //{
        //        //	sendMappingPerSyringe(magConfig, machineSpec, mappingData);
        //        //}
        //        //else
        //        //{
        //        //	sendMappingWithNoDivision(mappingData);
        //        //}
        //        if (isNoDivisionMappingModeEnable())
        //        {
        //            sendMappingWithNoDivision(mappingData);
        //        }
        //        else
        //        {
        //            sendMappingPerSyringe(magConfig, machineSpec, mappingData);
        //        }

        //    }
        //    catch (Exception err)
        //    {
        //        // 停止要求
        //        stopMachine();

        //        // マッピング不一致
        //        notifyWrongMapping();

        //        throw;
        //    }
        //}

        private void sendMappingPerSyringe(Magazine.Config magConfig, MachineSpec machineSpec, Dictionary<int, string> mappingData)
		{
			HideLog(string.Format("【装置仕様】シリンジ:{0}, メモリ:{1}, 塗布方向:{2}", machineSpec.SyringeNumberCD, machineSpec.MemoryTypeCD, machineSpec.MoldDirectionCD));

			// シリンジ当たりの総パッケージ数
			int totalPkgCtPerSyringe = magConfig.TotalMagazinePackage / machineSpec.SyringeQty;

			string[][] mappingDataPerSyringe = new string[machineSpec.SyringeQty][];

			// モールドパターン（横打ちか縦打ちかで処理を切り替える	
			if (machineSpec.MoldDirectionCD == MachineSpec.HORIZONTAL_DIRECTION_CD) //動作パターン:1(横打ち)/使用メモリ：2(ZF)
			{
				for (int syringeIndex = 0; syringeIndex < machineSpec.SyringeQty; syringeIndex++)
				{
					// 横打ち
					mappingDataPerSyringe[syringeIndex] = GetHorizontalPatternMappingPerSyringe(mappingData, machineSpec.SyringeQty, syringeIndex, totalPkgCtPerSyringe, magConfig);
				}
			}
			else if (machineSpec.MoldDirectionCD == MachineSpec.VERTICAL_DIRECTION_CD) //動作パターン:2(縦打ち)/使用メモリ：2(ZF)
			{
				for (int syringeIndex = 0; syringeIndex < machineSpec.SyringeQty; syringeIndex++)
				{
					// 縦打ち
					mappingDataPerSyringe[syringeIndex] = GetVerticalPatternMappingPerSyringe(mappingData, machineSpec.SyringeQty, syringeIndex, totalPkgCtPerSyringe, magConfig);
				}
			}
			else
			{
				// それ以外(未使用)
				throw new ApplicationException("[使用シリンジ数]=" + machineSpec.SyringeNumberCD + "/[動作パターン]" + machineSpec.MoldDirectionCD + "/[使用メモリ]" + machineSpec.MemoryTypeCD + "のマッピング条件はプログラムが対応していない為、マッピング出来ません。");
			}

			if (machineSpec.MemoryTypeCD == MachineSpec.ZF_MEMORY_CD)
			{
				sendMappingToZFMemory(machineSpec.SyringeQty, mappingDataPerSyringe);
			}
			else if (machineSpec.MemoryTypeCD == MachineSpec.LR_MEMORY_CD)
			{
				sendMappingToLRMemory(machineSpec.SyringeQty, mappingDataPerSyringe);
			}
			else
			{
				throw new ApplicationException("[使用シリンジ数]=" + machineSpec.SyringeNumberCD + "/[動作パターン]" + machineSpec.MoldDirectionCD + "/[使用メモリ]" + machineSpec.MemoryTypeCD + "のマッピング条件はプログラムが対応していない為、マッピング出来ません。");
			}
		}

        private void sendMappingWithNoDivision(Dictionary<int, string> mappingData)
        {
            sendMappingWithNoDivision(mappingData, MAPPING_START_ADDRESS);
        }

        private void sendMappingWithNoDivision(Dictionary<int, string> mappingData, string address)
		{
			string[] mappingArray = mappingData.OrderBy(m => m.Key).Select(m => m.Value).ToArray();

			int[] mappingDataArray = Convert2ByteBinaryArrayToDecimalArray(mappingArray);

			string logMsg = string.Format("書き込み開始アドレス：{0} / データ：{1}", address, string.Join(" ", mappingDataArray));
			HideLog(logMsg);

			Plc.SendMultiValue(address, mappingDataArray, Keyence.Suffix_U);

			Thread.Sleep(100);
		}

        //private void sendMappingWithNoDivision(Dictionary<int, string> mappingData)
        //{
        //    string[] mappingArray = mappingData.OrderBy(m => m.Key).Select(m => m.Value).ToArray();

        //    int[] mappingDataArray = Convert2ByteBinaryArrayToDecimalArray(mappingArray);

        //    string logMsg = string.Format("書き込み開始アドレス：{0} / データ：{1}", MAPPING_START_ADDRESS, string.Join(" ", mappingDataArray));
        //    HideLog(logMsg);

        //    Plc.SendMultiValue(MAPPING_START_ADDRESS, mappingDataArray, Keyence.Suffix_U);

        //    Thread.Sleep(100);
        //}

        private void sendMappingToZFMemory(int syringeQty, string[][] mappingDataPerSyringe)
		{
			string[] startAddrPerSyringe = getStartAddressAllSyringe(syringeQty);

			// 書き込み用フォーマットに変換
			int[][] decMappingDataPerSyringe = new int[syringeQty][];

			for (int syringeIndex = 0; syringeIndex < syringeQty; syringeIndex++)
			{
				decMappingDataPerSyringe[syringeIndex] = Convert2ByteBinaryArrayToDecimalArray(mappingDataPerSyringe[syringeIndex]);

				sendMappingDataPerSyringe(startAddrPerSyringe[syringeIndex], decMappingDataPerSyringe[syringeIndex]);
			}
		}

		private void sendMappingToLRMemory(int syringeQty, string[][] mappingDataPerSyringe)
		{	
			for (int syringeIndex = 0; syringeIndex < syringeQty; syringeIndex++)
			{
				string[] frontSideMappingData = mappingDataPerSyringe[syringeIndex].Take(Config.Settings.LRMappingAddressFrontSideSize).ToArray();
				string[] backSideMappingData = mappingDataPerSyringe[syringeIndex].Skip(Config.Settings.LRMappingAddressFrontSideSize).Take(Config.Settings.LRMappingAddressBackSideSize).ToArray();

				sendMappingDataPerSyringe(LRFrontSideStartAddressPerSyringe[syringeIndex], frontSideMappingData);

				sendMappingDataPerSyringe(LRBackSideStartAddressPerSyringe[syringeIndex], backSideMappingData);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pkgIndex">0から始まるパッケージを示すインデックス</param>
		/// <param name="xPkgMaxCt">X方向のパッケージ最大数</param>
		/// <returns></returns>
		private static bool isOddRowInFrame(int pkgIndex, int frameIndex, int framePkgCt, int xPkgMaxCt)
		{
			int pkgIndexinFrame = pkgIndex - frameIndex * framePkgCt;
			int yIndex = pkgIndexinFrame / xPkgMaxCt;

			if (yIndex % 2 == 0)
			{
				return true;
			}
			return false;
		}

		private static bool isOddColInFrame(int pkgIndex, int frameIndex, int framePkgCt, int yPkgMaxCt)
		{
			int pkgIndexinFrame = pkgIndex - frameIndex * framePkgCt;
			int xIndex = pkgIndexinFrame / yPkgMaxCt;

			if (xIndex % 2 == 0)
			{
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// フレーム長手方向(フレームの搬送方向)でつづら折りにモールドするパターンに変換したシリンジ毎のマッピングデータを取得
		/// </summary>
		/// <param name="mappingData">マッピング元データ</param>
		/// <param name="syringeMaxNum">シリンジの最大本数</param>
		/// <param name="syringeIndex">データ取得対象のシリンジのインデックス(0始まり)</param>
		/// <param name="totalPkgCt">マッピングデータ上に有るはずの総パッケージ数</param>
		/// <param name="framePkgCt">フレーム上の総パッケージ数</param>
		/// <param name="xPkgMaxCt">フレーム内</param>
		/// <param name="yPkgMaxCt"></param>
		/// <returns></returns>
		public static string[] GetHorizontalPatternMappingPerSyringe(Dictionary<int, string> mappingData, int syringeMaxNum, int syringeIndex, int totalPkgCtPerSyringe, Magazine.Config magConfig)
		{
#if DEBUG
			for (int i = 0; i < mappingData.Count; i++)
			{
				mappingData[i + 1] = (i + 1).ToString();
			}
#endif

			// フレーム内におけるシリンジ当たりのパッケージ数
			int framePkgCtPerSyringe = magConfig.TotalFramePackage / syringeMaxNum;

			int xPkgMaxCtPerSyringe = (magConfig.PackageQtyX / syringeMaxNum);
			// フレーム内におけるX方向の最大インデックス
			int xMaxIndexPerSyringe = xPkgMaxCtPerSyringe - 1;
			int xIndex;

			int syringeNo = syringeIndex + 1;

			// シリンジ分のインデックスのオフセット
			int indexSyringeOffset = (syringeMaxNum - syringeNo) * framePkgCtPerSyringe;

			// シリンジ毎のマッピングデータを格納
			string[] mappingDataPerSyringe = new string[totalPkgCtPerSyringe];

			for (int index = 0; index < totalPkgCtPerSyringe; index++)
			{
				// 何フレーム目かのインデックス(0始まり)
				int frameIndex = index / framePkgCtPerSyringe;
				// フレーム内におけるY方向のインデックス(0始まり)
				int yIndex = (index / xPkgMaxCtPerSyringe) % magConfig.PackageQtyY;
				// フレーム内におけるY方向のアドレス（1始まり)
				int yAddress = yIndex + 1;

				int indexPerFrame = index % framePkgCtPerSyringe;

				if (isOddRowInFrame(index, frameIndex, framePkgCtPerSyringe, xPkgMaxCtPerSyringe))
				{
					xIndex = index % xPkgMaxCtPerSyringe;
				}
				else
				{
					xIndex = (yAddress * xPkgMaxCtPerSyringe - indexPerFrame + xMaxIndexPerSyringe) % xPkgMaxCtPerSyringe;
				}
				// シリンジ内でのアドレス
				int addressPerSyringe = xIndex * magConfig.PackageQtyY + yAddress + frameIndex * magConfig.TotalFramePackage;

				int addressOnMappingData = (addressPerSyringe) + indexSyringeOffset;


				mappingDataPerSyringe[index] = mappingData[addressOnMappingData];
			}

			return mappingDataPerSyringe;
		}

		public static string[] GetVerticalPatternMappingPerSyringe(Dictionary<int, string> mappingData, int syringeMaxNum, int syringeIndex, int totalPkgCtPerSyringe, Magazine.Config magConfig)
		{
#if DEBUG
			for (int i = 0; i < mappingData.Count; i++)
			{
				mappingData[i + 1] = (i + 1).ToString();
			}
#endif

			// フレーム内におけるシリンジ当たりのパッケージ数
			int framePkgCtPerSyringe = magConfig.TotalFramePackage / syringeMaxNum;
			int xPkgMaxCtPerSyringe = (magConfig.PackageQtyX / syringeMaxNum);
			int syringeNo = syringeIndex + 1;

			int indexSyringeOffset = (syringeMaxNum - syringeNo) * framePkgCtPerSyringe;

			int indexInSyringe;

			// シリンジ毎のマッピングデータを格納
			string[] mappingDataPerSyringe = new string[totalPkgCtPerSyringe];

			for (int index = 0; index < totalPkgCtPerSyringe; index++)
			{
				int yIndex;
				int xIndex = (index / magConfig.PackageQtyY) % xPkgMaxCtPerSyringe;
				int xAddressInSyringe = xIndex + 1;

				// 何フレーム目かのインデックス(0始まり)
				int frameIndex = index / framePkgCtPerSyringe;
				// フレーム分のインデックスのオフセット
				int indexFrameOffset = frameIndex * magConfig.TotalFramePackage;

				// シリンジ内のインデックスを計算
				if (isOddColInFrame(index, frameIndex, framePkgCtPerSyringe, magConfig.PackageQtyY))
				{
					yIndex = index % magConfig.PackageQtyY;
					indexInSyringe = (xAddressInSyringe - 1) * magConfig.PackageQtyY + yIndex + indexFrameOffset;
					
				}
				else
				{
					yIndex = index % magConfig.PackageQtyY;
					int yAddress = yIndex + 1;
					indexInSyringe = xAddressInSyringe * magConfig.PackageQtyY - yAddress + indexFrameOffset;
				}

				int indexOnMappingData = indexInSyringe + indexSyringeOffset;

				int addressOnMappingData = indexOnMappingData + 1;

				mappingDataPerSyringe[index] = mappingData[addressOnMappingData];
			}

			return mappingDataPerSyringe;
		}

		private string[] getStartAddressAllSyringe(int allSyringeNum)
		{
			string[] startAddressPerSyringe = new string[allSyringeNum];

			for (int syringeNum = 1; syringeNum <= allSyringeNum; syringeNum++)
			{
				int syringeIndex = syringeNum - 1;

				// 各シリンジの先頭アドレス取得
				string startAddress = Plc.GetData(StartAddressPerSyringe[syringeIndex], 5, Keyence.Suffix_H, true)
                    .Replace("\0", "");

				startAddressPerSyringe[syringeIndex] = startAddress;
			}

			return startAddressPerSyringe;
		}

		private void verifyMappingBeforeMold(Lot lot, Magazine.Config magConfig, Dictionary<int, string> mdInputData, int mdProcNo)
		{
			try
			{
				InfoLog("MD前マッピング照合開始", lot.LotNo);

				Dictionary<int, string> mdReferenceData = getPreVerifyReferenceData(lot, magConfig, mdProcNo);

				Dictionary<int, string> differenceAddressList = getDifferenceMappingAddress(mdInputData, mdReferenceData);

				string differenceAddrLog = string.Empty;
				foreach (KeyValuePair<int, string> differenceAddress in differenceAddressList)
				{
					differenceAddrLog += string.Format("MD入力データ アドレス{0} / 値{1} 照合基準データ 値{2}\r\n", differenceAddress.Key, differenceAddress.Value, mdReferenceData[differenceAddress.Key]);
				}

				if (differenceAddrLog != string.Empty)
				{
					ExclamationLog("MD前マッピング照合で不整合\r\n" + differenceAddrLog, lot.LotNo);
				}

				InfoLog("MD前マッピング照合終了", lot.LotNo);
			}
			catch (ApplicationException err)
			{
				throw new ApplicationException(string.Format("モールド前照合にて予期せぬエラーが発生しました。ロット：{0} {1}", lot.LotNo, err.Message));
			}
		}

		private Dictionary<int, string> getDifferenceMappingAddress(Dictionary<int, string> targetDataList, Dictionary<int, string> referenceDataList)
		{

			Dictionary<int, string> differenceAddressList = new Dictionary<int, string>();

			foreach (KeyValuePair<int, string> referenceData in referenceDataList)
			{
				int key = referenceData.Key;

				//Console.WriteLine(referenceData.Key + "\t" + referenceData.Value + "\t" + targetDataList[referenceData.Key]);

				if (Config.Settings.DebugLogOutFg)
				{
					HideLog("照合基準 アドレス:" + referenceData.Key + " 値：" + referenceData.Value + " MD使用 値：" + targetDataList[referenceData.Key]);
				}

				if (referenceData.Value != targetDataList[key])
				{
					differenceAddressList.Add(key, targetDataList[key]);
				}
			}

			return differenceAddressList;
		}

		private Dictionary<int, string> getPreVerifyReferenceData(Lot lot, Magazine.Config magConfig, int mdProcNo)
		{
			Dictionary<int, string> wbReferenceData = new Dictionary<int, string>();
			Dictionary<int, string> aiReferenceData = new Dictionary<int, string>();
			
			if (lot.IsMappingInspection)
			{
				try
				{
					FileInfo wbMMFileInfo = Wirebonder.MMFile.GetCompletedFile(lot.LotNo);
					Wirebonder.MMFile wbMMFile = new Wirebonder.MMFile(wbMMFileInfo, this);
					wbMMFile.Contents = wbMMFile.GetData();

					Dictionary<int, string> wbRawMapping = wbMMFile.GetMappingDataForAI(wbMMFile.Contents, magConfig);

					wbReferenceData = MappingFile.Data.Wirebonder.ConvertToMold(wbRawMapping);
				}
				catch (Exception err)
				{
                    ExclamationLog(string.Format("MMファイルからの照合用マッピングデータの作成に失敗した為、全パッケージOKの照合用マッピングデータを作成(WB)。 ロットNo:{0}\r\n失敗理由：{1}", lot.LotNo, err.Message));
					wbReferenceData = MappingFile.Data.GetAnySize(magConfig.TotalMagazinePackage, MappingFile.Data.OK_VAL);
				}
			}
			else
			{
                InfoLog(string.Format("WBマッピングフラグ:OFFの為、全パッケージOKの照合用マッピングデータを作成(WB)。 ロットNo:{0}", lot.LotNo));
				wbReferenceData = MappingFile.Data.GetAnySize(magConfig.TotalMagazinePackage, MappingFile.Data.OK_VAL);
			}

			List<MacDefect> defectList = MacDefect.GetData(lot.LotNo);

			foreach (MacDefect d in defectList)
			{
				if (!wbReferenceData.ContainsKey(d.DefAddressNo))
				{
					throw new ApplicationException(
						string.Format("手動登録された不良を取得中, 存在しないアドレスへの参照が発生。 ロットNo:{0} アドレス:{1}", lot.LotNo, d.DefAddressNo));
				}

				wbReferenceData[d.DefAddressNo] = MappingFile.Data.NG_VAL;
			}

            // WB作業がある時のみ
            LENS2_Api.ARMS.WorkResult WireResults = LENS2_Api.ARMS.WorkResult.GetData(lot.LotNo, Config.Settings.WirebondProcNo);
            if (WireResults != null)
            {
                // WBの照合用基準データの上下反転
                bool wbReverseUpDownFg = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo, Config.Settings.WirebondProcNo, mdProcNo, false);
                HideLog(string.Format("【MD前照合】照合用基準データ(WB)の上下反転有無：{0} / ロットNo:{1} WB-Proc(Config):{2} MD-Proc(Config):{3}", wbReverseUpDownFg, lot.LotNo, Config.Settings.WirebondProcNo, mdProcNo));
                //bool wbReverseUpDownFg = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo, Config.Settings.WirebondProcNo, Config.Settings.MoldProcNo, false);
                //HideLog(string.Format("【MD前照合】照合用基準データ(WB)の上下反転有無：{0} / ロットNo:{1} WB-Proc(Config):{2} MD-Proc(Config):{3}", wbReverseUpDownFg, lot.LotNo, Config.Settings.WirebondProcNo, Config.Settings.MoldProcNo));

                if (wbReverseUpDownFg)
                {
                    wbReferenceData = MappingFile.Data.GetReverseData(wbReferenceData, magConfig);
                }
            }

			if (lot.IsChangePoint || lot.HasPassedInspector)
			{
				try
				{
					Inspector.MMFile aiMMFile = Inspector.MMFile.GetCompletedFile(lot.LotNo, this);
					Dictionary<int, string> mmContents = aiMMFile.GetData();
					aiReferenceData = aiMMFile.GetMappingDataForMD(mmContents, magConfig, lot.IsMappingInspection);
					//aiReferenceData = aiMMFile.GetData();

                    // AIの照合用基準データの上下反転
                    bool aiReverseUpDownFg = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo, Config.Settings.InspectProcNo, mdProcNo, false);
                    HideLog(string.Format("【MD前照合】照合用基準データ(AI)の上下反転有無：{0} / ロットNo:{1} AI-Proc(Config):{2} MD-Proc(Config):{3}", aiReverseUpDownFg, lot.LotNo, Config.Settings.InspectProcNo, mdProcNo));
                    //bool aiReverseUpDownFg = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo, Config.Settings.InspectProcNo, Config.Settings.MoldProcNo, false);
                    //HideLog(string.Format("【MD前照合】照合用基準データ(AI)の上下反転有無：{0} / ロットNo:{1} AI-Proc(Config):{2} MD-Proc(Config):{3}", aiReverseUpDownFg, lot.LotNo, Config.Settings.InspectProcNo, Config.Settings.MoldProcNo));

                    if (aiReverseUpDownFg)
                    {
                        aiReferenceData = MappingFile.Data.GetReverseData(aiReferenceData, magConfig);
                    }
				}
				catch (Exception err)
				{
                    ExclamationLog(string.Format("MMファイルからの照合用マッピングデータの作成に失敗した為、全パッケージOKの照合用マッピングデータを作成(AI)。 ロットNo:{0}\r\n失敗理由：{1}", lot.LotNo, err.Message));
					aiReferenceData = MappingFile.Data.GetAnySize(magConfig.TotalMagazinePackage, MappingFile.Data.OK_VAL);
				}
			}
			else
			{
                InfoLog(string.Format("変化点フラグ:OFFかつ検査機非通過の為、全パッケージOKの照合用マッピングデータを作成(AI)。 ロットNo:{0}", lot.LotNo));
				aiReferenceData = MappingFile.Data.GetAnySize(magConfig.TotalMagazinePackage, MappingFile.Data.OK_VAL);
			}

			// WBとAIの基準データを合わせてMDの照合用基準データを生成
			Dictionary<int, string> mdReferenceData = MappingFile.Data.Union(wbReferenceData, aiReferenceData);

			return mdReferenceData;

		}

		private void checkMappingAfterMold(SDFile sdFile, int mdProcNo)
		{
            bool isNoDivisionMappingModeEnableFg = isNoDivisionMappingModeEnable();

            sdFile.GetData(isNoDivisionMappingModeEnableFg);

			Dictionary<int, string> mappingDataList;

			Lot lot = Lot.GetData(sdFile.LotNo);
			
			InfoLog("MD後マッピング照合開始", lot.LotNo);

			// マガジン構成情報取得
			Magazine.Config magConfig = Magazine.Config.GetData(lot.TypeCd);

			bool hasNeedReverseFramePlacement;

			if (lot.IsMappingInspection)
			{
				if (lot.HasPassedInspector)
				{
                    hasNeedReverseFramePlacement = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo, Config.Settings.InspectProcNo, mdProcNo, false);
                    HideLog(string.Format("【MD後照合】照合用基準データ(AI通過)の上下反転有無：{0} / ロットNo:{1} AI-Proc(Config):{2} MD-Proc(Config):{3}", hasNeedReverseFramePlacement, lot.LotNo, Config.Settings.InspectProcNo, mdProcNo));
                    //hasNeedReverseFramePlacement = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo, Config.Settings.InspectProcNo, Config.Settings.MoldProcNo, false);
                    //HideLog(string.Format("【MD後照合】照合用基準データ(AI通過)の上下反転有無：{0} / ロットNo:{1} AI-Proc(Config):{2} MD-Proc(Config):{3}", hasNeedReverseFramePlacement, lot.LotNo, Config.Settings.InspectProcNo, Config.Settings.MoldProcNo));

					mappingDataList = MappingFile.Data.Inspector.Get(lot.LotNo, hasNeedReverseFramePlacement);
					mappingDataList = MappingFile.Data.Inspector.ConvertToMold(mappingDataList);
				}
				else
				{
                    hasNeedReverseFramePlacement = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo, Config.Settings.WirebondProcNo, mdProcNo, false);
                    HideLog(string.Format("【MD後照合】照合用基準データ(AIスルー)の上下反転有無：{0} / ロットNo:{1} WB-Proc(Config):{2} MD-Proc(Config):{3}", hasNeedReverseFramePlacement, lot.LotNo, Config.Settings.WirebondProcNo, mdProcNo));
                    //hasNeedReverseFramePlacement = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo, Config.Settings.WirebondProcNo, Config.Settings.MoldProcNo, false);
                    //HideLog(string.Format("【MD後照合】照合用基準データ(AIスルー)の上下反転有無：{0} / ロットNo:{1} WB-Proc(Config):{2} MD-Proc(Config):{3}", hasNeedReverseFramePlacement, lot.LotNo, Config.Settings.WirebondProcNo, Config.Settings.MoldProcNo));

					mappingDataList = MappingFile.Data.Wirebonder.Get(lot.LotNo, hasNeedReverseFramePlacement);
					mappingDataList = MappingFile.Data.Wirebonder.ConvertToMoldFromMachineFile(mappingDataList);
				}

                // BMマッピング要求がONの時、照合用マッピングデータを適用する。
                if (haveBadMarkMappingRequest())
                {
                    // 照合用マッピングデータを(MD_IN - MD_IN1)と(MD_IN - MD_IN2)の変換データの和集合に変換
                    Dictionary<int, string> mappingDataList1 = ConvertToSendMappingData1(mappingDataList);
                    Dictionary<int, string> mappingDataList2 = ConvertToSendMappingData2(mappingDataList);
                    mappingDataList = MappingFile.Data.Union(mappingDataList1, mappingDataList2);
                }
                else
                {
                    // 照合用マッピングデータを(MD_IN - MD_IN1)の変換データに変換 ※BMマッピング箇所は良品扱いにする
                    mappingDataList = ConvertToSendMappingData1(mappingDataList);
                }
            }
			else
			{
				ExclamationLog("対象ﾛｯﾄのWBﾏｯﾋﾟﾝｸﾞﾌﾗｸﾞ=OFFの為、全pcs良品としてMD後照合を行います。", lot.LotNo);
				mappingDataList = MappingFile.Data.GetAnySize(magConfig.TotalMagazinePackage, "0");
			}
			// 2014/10/2 全工程分の手動登録を取得して樹脂少扱いにする必要が無いか確認　する必要ある場合、各工程で上下反転の必要性チェックが必要
			// (AI or WB)不良アドレスを取得
            mappingDataList = MappingFile.Data.GetAddedManualInputDefect(sdFile.LotNo, magConfig, mappingDataList, mdProcNo, false);
            
            Dictionary<int, string> defectAddressList = MappingFile.Data.GetDefectAddressList(mappingDataList, lot); //, hasNeedReverseFramePlacement, magConfig);

			int checkNgCt = 0;

            List<string> afterMDNgList = new List<string>();

			foreach (KeyValuePair<int, string> defectInfo in defectAddressList)
			{
				bool isMappingCheckNg = false;

				//8連ｼﾘﾝｼﾞMDからの仕様であるｼｽﾃﾑで加工せずに1ｱﾄﾞﾚｽ目から分割無しでｼｰｹﾝｼｬﾙなﾏｯﾋﾟﾝｸﾞﾃﾞｰﾀを送信するﾓｰﾄﾞかﾁｪｯｸ
                if (isNoDivisionMappingModeEnableFg)
				{
					if (sdFile.IsDispenceDefect(sdFile.PottingContents, defectInfo.Key) == false)
					{
						isMappingCheckNg = true;
					}
				}
				else
				{
					// ファイル内容を確認する為の情報を取得
					MdDispenseInfo mdDispenseInfo = sdFile.getDispenseInfo(sdFile.Contents, defectInfo.Key, magConfig.TotalFramePackage, magConfig.PackageQtyY);
					
					if (sdFile.IsDispenceDefect(mdDispenseInfo) == false)
					{
						isMappingCheckNg = true;
					}
				}

				if (isMappingCheckNg)
				{
					if (checkNgCt == 0)
					{
						afterMDNgList.Add("【マッピング照合不一致】MD後マッピング照合で不整合");
					}
					checkNgCt++;

					afterMDNgList.Add(string.Format("アドレス{0} / 値{1}", defectInfo.Key, defectInfo.Value));
				}

				HideLog(string.Format("addr:{0},SendVal:{1},result:{2}", defectInfo.Key, defectInfo.Value, isMappingCheckNg ? "照合NG" : "照合OK"));
			}

			if (checkNgCt > 0)
			{
                // 流動規制付与
                InsertRestrictMappingNgAfterMold(lot, sdFile.ProcNo);

                ErrorLog(string.Join("\r\n", afterMDNgList));
				// 装置停止要求
                stopMachine();
			}

			InfoLog("MD後マッピング照合終了", lot.LotNo);

		}

		/// <summary>
		/// シリンジ本数を取得
		/// </summary>
		/// <param name="syringeID">シリンジID</param>
		/// <returns>シリンジ本数</returns>
		private static int convertIdToSyringeCt(int syringeId)
		{
			try
			{
				int syringeCt = 0;
				switch (syringeId)
				{
					case 1:
						syringeCt = 5;
						break;
					case 2:
						syringeCt = 4;
						break;
					default:
						throw new ApplicationException(string.Format("管理されていないシリンジです。シリンジNO:{0}", syringeId));
				}
				return syringeCt;
			}
			catch (Exception err)
			{
				throw err;
			}
		}

		///// <summary>
		///// モールド動作パターンを取得
		///// </summary>
		///// <param name="moldPatternID">モールド動作パターンID</param>
		///// <returns>モールド動作パターン</returns>
		//private static MachineSpec.MoldPattern convertIdToMoldPattern(int moldPatternId)
		//{
		//	try
		//	{
		//		switch (moldPatternId)
		//		{
		//			case 1:
		//				return MachineSpec.MoldPattern.Side;
		//			case 2:
		//				return MachineSpec.MoldPattern.Length;
		//			default:
		//				throw new ApplicationException(string.Format("管理されていない動作パターンです。動作パターンNO:{0}", moldPatternId));
		//		}
		//	}
		//	catch (Exception err)
		//	{
		//		throw err;
		//	}
		//}

		/// <summary>
		/// 開始アドレスから任意パッケージ個数の打順(Y方向)マッピングデータを取得
		/// </summary>
		/// <param name="startAddress">開始アドレス</param>
		/// <param name="nextCount">パッケージ個数</param>
		/// <param name="yPackageCT">フレームY数</param>
		private static List<int> getOrderYMappingData(int startAddress, int nextCount, int packageQtyY)
		{
			List<int> orderMappingList = new List<int>();

			try
			{
				int targetAddress = startAddress;
				for (int i = 0; i <= nextCount; i++)
				{
					orderMappingList.Add(targetAddress);

					// 対象アドレスNOとフレーム縦NOを除算した余剰
					int remainderVAL = Convert.ToInt32(decimal.Remainder(targetAddress, packageQtyY));

					// 対象アドレスNOとフレーム縦NOを除算した結果を切り捨て
					double resultVAL = Math.Truncate(Convert.ToDouble(targetAddress / packageQtyY));
					if (resultVAL % 2 == 0)
					{
						// 結果が偶数(奇数列)
						switch (remainderVAL)
						{
							case 0:
								targetAddress -= 1;
								break;
							default:
								targetAddress += 1;
								break;
						}
					}
					else
					{
						// 結果が奇数(偶数列)
						switch (remainderVAL)
						{
							case 0:
							case 1:
								targetAddress += packageQtyY;
								break;
							default:
								targetAddress -= 1;
								break;
						}
					}
				}

				return orderMappingList;
			}
			catch (Exception err)
			{
				throw err;
			}
		}

		/// <summary>
		/// 開始アドレスから任意パッケージ個数の打順(X方向)マッピングデータを取得
		/// </summary>
		/// <param name="mappingData">マッピングデータ</param>
		/// <param name="startAddress">開始アドレス</param>
		/// <param name="nextCount">パッケージ個数</param>
		/// <param name="yPackageCT">フレームY数</param>
		private static List<int> getOrderXMappingData(int startAddress, int nextCount, int packageQtyY)
		{
			List<int> orderMappingList = new List<int>();

			try
			{
				// 折り返し(フレーム右)アドレスリストを取得
				int turnStartAddress = startAddress;
				int turnEndAddress = startAddress + (packageQtyY - 1);

				List<int> rTurnAddressList = new List<int>();
				for (int i = turnStartAddress; i <= turnEndAddress; i++)
				{
					rTurnAddressList.Add(i);
				}

				// 折り返し(フレーム左)アドレスリストを取得
				turnStartAddress = startAddress + nextCount - (packageQtyY - 1);
				turnEndAddress = startAddress + nextCount;

				List<int> lTurnAddressList = new List<int>();
				for (int i = turnStartAddress; i <= turnEndAddress; i++)
				{
					lTurnAddressList.Add(i);
				}

				MachineSpec.MoldMovement moldMove = MachineSpec.MoldMovement.Left;
				bool turnCompltFG = false;

				int targetAddress = startAddress;
				for (int i = 0; i <= nextCount; i++)
				{
					orderMappingList.Add(targetAddress);

					if (targetAddress == startAddress)
					{
						targetAddress += packageQtyY;
					}
					else if (lTurnAddressList.Exists(l => l == targetAddress) && !turnCompltFG)
					{
						targetAddress += 1;
						moldMove = MachineSpec.MoldMovement.Right;
						turnCompltFG = true;
					}
					else if (rTurnAddressList.Exists(r => r == targetAddress) && !turnCompltFG)
					{
						targetAddress += 1;
						moldMove = MachineSpec.MoldMovement.Left;
						turnCompltFG = true;
					}
					else
					{
						if (moldMove == MachineSpec.MoldMovement.Left) { targetAddress += packageQtyY; }
						else { targetAddress -= packageQtyY; }
						turnCompltFG = false;
					}
				}

				return orderMappingList;
			}
			catch (Exception err)
			{
				throw err;
			}
		}

		private void setHeartBeat(bool value)
		{
            int addrSize = 11;

            int[] valueArray = new int[addrSize];
            for (int i = 0; i < addrSize; i++)
            {
                valueArray[i] = Convert.ToInt16(value);
            }


            Plc.SendMultiValue(this.HeartBeatAddress, valueArray, Keyence.Suffix_U);
		}

		private bool isMachineLogOutputCompletion()
		{
			return Plc.GetBitForAI_MD(this.CompleteOutputMachineLogAddress);
		}

		private bool isNoDivisionMappingModeEnable()
		{
			return Plc.GetBit(NO_DIVISION_MAPPING_MODE_ENABLE);
		}

        /// <summary>
        /// 流動規制を付与
        /// </summary>
        private void InsertRestrictMappingNgAfterMold(Lot lot, int procNo)
        {
            LENS2_Api.ARMS.Process[] wf = LENS2_Api.ARMS.Process.GetWorkFlow(lot.TypeCd);
            List<LENS2_Api.ARMS.Process> restrictProcInfo = new List<LENS2_Api.ARMS.Process>();
            List<KeyValuePair<int, string>> settingRestrictWorkCd = Config.Settings.MdMappingNgRestrictWorkCd;

            // 規制工程を設定ファイルの設定値から決定
            foreach (KeyValuePair<int, string> workcd in settingRestrictWorkCd.OrderBy(s => s.Key))
            {
                restrictProcInfo = wf.Where(w => w.WorkCd == workcd.Value).ToList();
                if (restrictProcInfo.Count() > 0)
                {
                    break;
                }
            }

            // 規制工程が見つからない場合、SDファイル名の作業番号(モールド作業)の工程に流動規制を付与する
            if (restrictProcInfo.Count() == 0)
            {
                LENS2_Api.ARMS.Process p = wf.Where(w => w.ProcNo == procNo).FirstOrDefault();
                restrictProcInfo.Add(p);
                ErrorLog(string.Format("投入ロット「{0}」のタイプ「{1}」の作業フローに設定ファイルで指定されている規制作業が存在しない為、現在工程「{2}」に対して流動規制を付与します。",
                    lot.LotNo, lot.TypeCd, p.InlineProNM));
            }

            // 一つのタイプのワークフローに同一作業CDがある場合を想定
            foreach (LENS2_Api.ARMS.Process p in restrictProcInfo)
            {
                LENS2_Api.ARMS.Restrict r = new LENS2_Api.ARMS.Restrict();
                r.LotNo = lot.LotNo;
                r.ProcNo = p.ProcNo;
                r.Reason = LENS2_Api.ARMS.Restrict.RESTRICT_REASON_MD_MAPPING_NG;
                r.DelFg = false;
                r.LastUpdDt = DateTime.Now;
                r.ReasonKb = LENS2_Api.ARMS.Restrict.RESTRICT_REASON_KB_NOTCHECK_WORK_START;

                r.Save();
                ErrorLog(string.Format("投入ロット「{0}」に対して流動規制を付与します。規制工程：「{1}」", lot.LotNo, p.InlineProNM));
            }
        }
        
		/// <summary>
		/// 装置停止
		/// </summary>
		private void stopMachine()
		{
			InfoLog("装置停止");
			Plc.SetBit(this.MachineStopAddress, true);
		}

		private void notifyWrongMapping()
		{
            HideLog(string.Format("マッピングデータ不正を装置へ通知(アドレス:{0})", this.WrongMappingAddress));
			Plc.SetBit(this.WrongMappingAddress, true);
		}

		private bool haveRequest()
		{
			return Plc.GetBitForAI_MD(this.MappingRequestAddress);
		}

        private bool haveBadMarkMappingRequest()
        {
            return Plc.GetBitForAI_MD(BADMARK_MAPPING_REQUEST_ADDRESS);
        }

        private void clearRequest()
		{
            HideLog(string.Format("マッピングデータ要求(アドレス:{0}):立下げ", this.MappingRequestAddress));
			Plc.SetBit(this.MappingRequestAddress, false);
		}

		private void sendMappingDataPerSyringe(string startAddrPerSyringe, int[] decMappingDataPerSyringe)
		{
            //現状ZFメモリに書き込みする時に使用する想定(引数のSuffixはU)
            HideLog(string.Format("【各シリンジマッピングデータ送信】アドレス:{0} データ:{1}", startAddrPerSyringe, string.Join(" ", decMappingDataPerSyringe)));
			Plc.SendMultiValue(startAddrPerSyringe, decMappingDataPerSyringe, Keyence.Suffix_U);
		}

		private void sendMappingDataPerSyringe(string startAddrPerSyringe, string[] mappingDataPerSyringe)
		{
            //現状LRメモリに書き込みする時に使用する想定(引数のSuffixは無し)
            HideLog(string.Format("【各シリンジマッピングデータ送信】アドレス:{0} データ:{1}", startAddrPerSyringe, string.Join(" ", mappingDataPerSyringe)));
			Plc.SendMultiValue(startAddrPerSyringe, mappingDataPerSyringe, "");
		}

        private bool IsMoldMappingProcess(int moldProcNo)
        {
            // 設定項目「moldProcNo」 = マッピング処理有りのMD作業の工程No
            List<int> moldProcNoList = Config.Settings.MoldProcNoList;
            // 設定項目「PreMoldProcNo」 = マッピング処理無しのMD作業の工程No
            List<int> preMoldProcNoList = Config.Settings.PreMoldProcNoList;

            if (preMoldProcNoList != null && preMoldProcNoList.Contains(moldProcNo))
            {
                return false;
            }
            else if (moldProcNoList != null && moldProcNoList.Contains(moldProcNo))
            {
                return true;
            }
            else
            {
                // どちらの項目にも設定されていない場合、エラー表示
                throw new ApplicationException(string.Format("投入中作業の工程No：「{0}」が設定ファイル(LensConfig.xml)の項目「MoldProcNo」「PreMoldProcNo」のいずれにも設定されていません。", 
                    moldProcNo.ToString()));
            }
        }

		public class SDFile : MachineLog
		{
			public const string FILE_KIND = "SD";
            public const string FILE_UNKNOWN = "UNKNOWN";

			public const int DATESTARTINDEX_FROM_SD_ON_FILENAME = 2;
			public const int CONTENTS_HEADER_ROW = 1;
			public const int CONTENTS_HEADEREND_ROW = 2;
			public const int CONTENTS_MAGAZINE_COL = 3;
			public const int DATASTART_NO_ROW = 0;
			public const int MAGAZINE_NO_ROW = 0;
			public const int STEP_NO_COL = 4;
			public const int MOLD_ORDER_COL = 5;
			public const int SYRINGE_NO_COL = 17;
			public const int MOLDPATTERN_NO_COL = 16;
			/// <summary>ファイルSD内容ｼﾘﾝｼﾞ1樹脂小フラグ列</summary>
			public const int SYRINGE1_FG_COL = 11;
			/// <summary>ファイルSD内容ｼﾘﾝｼﾞ2樹脂小フラグ列</summary>
			public const int SYRINGE2_FG_COL = 12;
			/// <summary>ファイルSD内容ｼﾘﾝｼﾞ3樹脂小フラグ列</summary>
			public const int SYRINGE3_FG_COL = 13;
			/// <summary>ファイルSD内容ｼﾘﾝｼﾞ5樹脂小フラグ列</summary>
			public const int SYRINGE4_FG_COL = 14;
			/// <summary>ファイルSD内容ｼﾘﾝｼﾞ5樹脂小フラグ列</summary>
			public const int SYRINGE5_FG_COL = 15;

			private const string POTTING_ADDRESS_COLUMN_NAME = "NGﾏｯﾋﾟﾝｸﾞ 位置";
			private const string POTTING_RESULT_COLUMN_NAME = "NGﾏｯﾋﾟﾝｸﾞ 判定(NG:1_OK:0)";

			public FileInfo Info { get; set; }
			public IMachine WorkMachine { get; set; }

			//public string MagazineNo { get; set; }
			public string TypeCd { get; set; }
			public string LotNo { get; set; }
			public int ProcNo { get; set; }

			public int PottingAddrColIndex { get; set; }
			public int PottingResultColIndex { get; set; }

			public string[] Contents { get; set; }
			public Dictionary<int, int> PottingContents { get; set; }

			public SDFile(FileInfo sdfile, IMachine workMachine)
			{
				this.Info = sdfile;
				this.WorkMachine = workMachine;
				this.PottingContents = new Dictionary<int, int>();

				getLotFromFileName();
			}

			private void getLotFromFileName()
			{
				if (SDFile.IsFileNameAddLot(this.Info) == false)
				{
					throw new ApplicationException(
						string.Format("SDファイル名にロット情報がありません。ファイルパス:{0}", this.Info.FullName));
				}

				string[] nameChar = Path.GetFileNameWithoutExtension(this.Info.Name).Split('_');
				this.LotNo = nameChar[2].Trim();
				this.TypeCd = nameChar[3].Trim();
				this.ProcNo = int.Parse(nameChar[4].Trim());
			}

			public static bool IsFileNameAddLot(FileInfo file)
			{
				string[] nameChar = Path.GetFileNameWithoutExtension(file.Name).Split('_');
				if (nameChar.Count() < 5)
				{
					return false;
				}
				else { return true; }
			}

            public static bool IsUnknownFileName(SDFile sdFile)
            {
                if (sdFile.LotNo == FILE_UNKNOWN)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void GetData(bool isNoDivisionMappingModeEnableFg)
			{
				string[] fileValue = FileWrapper.ReadAllLines(this.Info.FullName);
                string[] headerArray = fileValue[CONTENTS_HEADER_ROW].Split(',');

				// ヘッダ部除去
				fileValue = fileValue.Skip(CONTENTS_HEADEREND_ROW).ToArray();
                //<--SGA-IM0000007627 MD装置SDカード内不要データ抽出
                List<string> fileValuelist = new List<string>();
                for (int i = 0; i < fileValue.Count(); i++)
                {
                    string[] dataArray = fileValue[i].Split(',');

                    if (dataArray.Count() < SD_KEIKOUFG_COL_INDEX)
                    {
                        continue;
                    }

                    if (i > 0)
                    {
                        string[] dataArraybefor = fileValue[i - 1].Split(',');
                        if (dataArray[SD_MAGNO_COL_INDEX] != dataArraybefor[SD_MAGNO_COL_INDEX])
                        {
                            fileValuelist.Clear();//ロットが途中で変われば、それまでのデータは削除。
                        }
                    }

                    if (Convert.ToInt32(dataArray[SD_KEIKOUFG_COL_INDEX]) == 1)
                    {
                        fileValuelist.Add(fileValue[i]);
                    }
                }

                fileValue = fileValuelist.ToArray();
                //-->SGA-IM0000007627 MD装置SDカード内不要データ抽出
                if (fileValue.Count() == 0)
				{
					// ヘッダ部のみの空ファイルの場合、処理停止
					throw new ApplicationException(
						string.Format("空のSDファイルが読み込まれました。出力ファイルを確認して下さい。出力ファイル:{0}", Info.FullName));
				}

                //<--SGA-IM0000007627 MD装置SDカード内不要データ抽出
                //string magazineQR = getMagazineQR(fileValue, CONTENTS_MAGAZINE_COL);
                //-->SGA-IM0000007627 MD装置SDカード内不要データ抽出

				// 検査結果取得
				this.Contents = fileValue;

                if (isNoDivisionMappingModeEnableFg)
                {
                    setPottingContents(this.Contents, headerArray);
                }
			}

			private void setPottingContents(string[] fileContents, string[] headerArray)
			{
				// ヘッダ部から樹脂塗布アドレスと樹脂塗布結果列のインデックス取得
				for (int i = 0; i < headerArray.Count(); i++)
				{
					headerArray[i] = headerArray[i].Trim();
					headerArray[i] = headerArray[i].Replace("\"", "");

					if (headerArray[i] == POTTING_ADDRESS_COLUMN_NAME)
					{
						this.PottingAddrColIndex = i;
					}

					if (headerArray[i] == POTTING_RESULT_COLUMN_NAME)
					{
						this.PottingResultColIndex = i;
					}
				}

				if (this.PottingAddrColIndex < 0 || this.PottingResultColIndex < 0)
				{
					throw new ApplicationException(
						string.Format("SDファイルに「{0}」「{1}」のどちらかの列が存在しません", POTTING_ADDRESS_COLUMN_NAME, POTTING_RESULT_COLUMN_NAME));
				}

				// ファイルから樹脂塗布アドレス、樹脂塗布結果を取得
				foreach (string lineContents in fileContents)
				{
					string[] contentItem = lineContents.Split(',');

					if (contentItem.Length <= this.PottingAddrColIndex || contentItem.Length <= this.PottingResultColIndex)
					{
						continue;
					}

					int pottingAddr, pottingResult;
					string pottingAddrStr = contentItem[this.PottingAddrColIndex];
					string pottingResultStr = contentItem[this.PottingResultColIndex];

					if (int.TryParse(pottingAddrStr, out pottingAddr) == false)
					{
						throw new ApplicationException(
							string.Format("SDﾌｧｲﾙから樹脂塗布ｱﾄﾞﾚｽの情報が数値変換できませんでした。数値変換対象:{0}/取得列ｲﾝﾃﾞｸｽ:{1}/取得列:{2}/取得ﾌｧｲﾙ:{3}"
							, pottingAddrStr, this.PottingAddrColIndex, POTTING_ADDRESS_COLUMN_NAME, Info.FullName));
					}

					if (int.TryParse(pottingResultStr, out pottingResult) == false)
					{
						throw new ApplicationException(
							string.Format("SDﾌｧｲﾙから樹脂塗布結果の情報が数値変換できませんでした。数値変換対象:{0}/取得列ｲﾝﾃﾞｸｽ:{1}/取得列:{2}/取得ﾌｧｲﾙ:{3}"
							, pottingResultStr, this.PottingResultColIndex, POTTING_RESULT_COLUMN_NAME, Info.FullName));
					}

					if (this.PottingContents.ContainsKey(pottingAddr))
					{
						throw new ApplicationException(
							string.Format("SDﾌｧｲﾙに樹脂塗布ｱﾄﾞﾚｽの情報が重複して存在します。重複していたｱﾄﾞﾚｽ:{0}/取得列ｲﾝﾃﾞｸｽ:{1}/取得列:{2}/取得ﾌｧｲﾙ:{3}"
							, pottingAddrStr, this.PottingAddrColIndex, POTTING_ADDRESS_COLUMN_NAME, Info.FullName));
					}
					else
					{
						this.PottingContents.Add(pottingAddr, pottingResult);
					}
				}
			}

			/// <summary>
			/// SDファイル内容の樹脂小フラグを取得
			/// </summary>
			/// <param name="mdDispenseInfo">ディスペンスタイム情報</param>
			/// <param name="fileLineValueList">ファイル内容</param>
			/// <returns>樹脂小フラグ</returns>
			public bool IsDispenceDefect(MdDispenseInfo mdDispenseInfo)
			{
				try
				{
					for (int i = DATASTART_NO_ROW; i < this.Contents.Length; i++)
					{
						if (this.Contents[i] == "")
						{
							// 空白行の場合、次へ
							continue;
						}

						List<string> fileValue = this.Contents[i].Split(',').ToList();

						int fileStepNO = Convert.ToInt32(fileValue[STEP_NO_COL]);
						int fileMoldNO = Convert.ToInt32(fileValue[MOLD_ORDER_COL]); ;
						if (fileStepNO != mdDispenseInfo.StepNO || fileMoldNO != mdDispenseInfo.MoldOrder)
						{
							continue;
						}

						int columnIndex = getSDFileSyringeColumn(mdDispenseInfo.SyringeNO);

						int defectFG = int.MinValue;
						if (!int.TryParse(fileValue[columnIndex], out defectFG))
						{
							throw new ApplicationException(string.Format("SDファイル内容に問題があります。製造のメンテGに連絡して下さい。ファイル内容:{0} 行:{1}", fileValue[columnIndex], i));
						}

						return Convert.ToBoolean(defectFG);
					}

					return false;
				}
				catch (Exception err)
				{
					throw err;
				}
			}

			public bool IsDispenceDefect(Dictionary<int, int> pottingContents, int pottingAddress)
			{
				if (pottingContents.Count == 0)
				{
					throw new ApplicationException(string.Format("SDﾌｧｲﾙから樹脂塗布ｱﾄﾞﾚｽ、結果の情報を読み取り出来ていません。"));
				}

				if (pottingContents.ContainsKey(pottingAddress))
				{
					if (pottingContents[pottingAddress] == 1)
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

			/// <summary>
			/// SDファイル内容樹脂小フラグ列番号を取得
			/// </summary>
			/// <param name="syringeNO">シリンジNO</param>
			/// <returns>樹脂小フラグ列番号</returns>
			private static int getSDFileSyringeColumn(int syringeNO)
			{
				int targetColumnIndex = int.MinValue;

				try
				{
					switch (syringeNO)
					{
						case 1:
							targetColumnIndex = SYRINGE1_FG_COL;
							break;
						case 2:
							targetColumnIndex = SYRINGE2_FG_COL;
							break;
						case 3:
							targetColumnIndex = SYRINGE3_FG_COL;
							break;
						case 4:
							targetColumnIndex = SYRINGE4_FG_COL;
							break;
						case 5:
							targetColumnIndex = SYRINGE5_FG_COL;
							break;
						default:
							throw new ApplicationException(string.Format("シリンジ番号が正しくありません。シリンジ数:{0} ", syringeNO));
					}

					return targetColumnIndex;
				}
				catch (Exception err)
				{
					throw err;
				}
			}

			/// <summary>
			/// SDファイル内容を確認する為の情報を取得
			/// </summary>
			/// <param name="addressNO">アドレス</param>
			/// <param name="magInfo">マガジン情報</param>
			/// <param name="fileLineValueList">ファイル内容</param>
			/// <returns>ディスペンスタイム情報</returns>
			public MdDispenseInfo getDispenseInfo(string[] contents, int addressNO, int totalFramePackage, int packageQtyY)
			{
				MdDispenseInfo mdDispenseInfo = new MdDispenseInfo();

				try
				{
					// 段数を取得
					mdDispenseInfo.StepNO = Convert.ToInt32(Math.Ceiling((decimal)addressNO / (decimal)totalFramePackage));

					// シリンジ本数を取得
					int syringeId = Convert.ToInt32(contents[MAGAZINE_NO_ROW].Split(',')[SYRINGE_NO_COL]);
					int syringeCt = convertIdToSyringeCt(syringeId);

					// モールド動作パターン(縦or横)を取得
					int moldPatternId = Convert.ToInt32(contents[MAGAZINE_NO_ROW].Split(',')[MOLDPATTERN_NO_COL]);


					// 2014/10/2 リファクタリング対象
					// 対象シリンジを取得
					int targetStartAddressNO = 0; int targetEndAddressNO = 0;
					for (int i = 1; i <= syringeCt; i++)
					{
						int frameStartAddressNO = mdDispenseInfo.StepNO * totalFramePackage - totalFramePackage + 1;
						int syringeRange = (totalFramePackage / syringeCt);
						int syringeStartAddressNO = frameStartAddressNO + (syringeRange * i) - syringeRange;
						int syringeEndAddressNO = frameStartAddressNO + (syringeRange * i) - 1;
						// syringeEndAddressNO - syringeStartAddressNO の計算結果 = - 1 + syringeRange
						if (addressNO >= syringeStartAddressNO && addressNO <= syringeEndAddressNO)
						{
							mdDispenseInfo.SyringeNO = syringeCt - (i - 1);
							targetStartAddressNO = syringeStartAddressNO;
							targetEndAddressNO = syringeEndAddressNO;
							break;
						}
					}
					
					// モールドNOを取得(ｼﾘﾝｼﾞの打順NO)
					List<int> orderMappingList = new List<int>();
					if (moldPatternId == MachineSpec.VERTICAL_DIRECTION_CD)
					{
						// モールド動作パターン(縦)
						orderMappingList = getOrderYMappingData(targetStartAddressNO, targetEndAddressNO - targetStartAddressNO, packageQtyY);
					}
					else if (moldPatternId == MachineSpec.HORIZONTAL_DIRECTION_CD)
					{
						// モールド動作パターン(横)
						orderMappingList = getOrderXMappingData(targetStartAddressNO, targetEndAddressNO - targetStartAddressNO, packageQtyY);
					}
					else
					{
						throw new ApplicationException(string.Format("SDファイルから得たモールド動作CDが正しくありません。列：{0},行：{1},モールド動作CD：{2}", SYRINGE_NO_COL, MAGAZINE_NO_ROW, moldPatternId));
					}

					mdDispenseInfo.MoldOrder = orderMappingList.FindIndex(m => m == addressNO) + 1;

					return mdDispenseInfo;
				}
				catch (Exception err)
				{
					throw err;
				}
			}
		}
	}
}
