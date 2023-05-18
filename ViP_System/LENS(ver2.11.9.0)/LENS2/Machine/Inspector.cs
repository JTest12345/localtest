using PLC;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using LENS2_Api;
using System.Collections.ObjectModel;

namespace LENS2.Machine
{
	/// <summary>
	/// 外観検査機
	/// </summary>
	public class Inspector : MachineBase, IMachine
	{
        public const string ADDR_MODE = "ADDR";
        public const string QTY_MODE = "QTY";
		
		public const string RequestMarkingDataModeAddress = "";

		private const string MARKING_MAPPING_START_ADDRESS = "ZF100000";
		private const int MARKING_MAPPING_ADDRESS_LENGTH = 10000;
		private const string MARKING_ADDRESS_HEADER_NAME = "ﾏｰｷﾝｸﾞｱﾄﾞﾚｽ";
		private const string MARKING_MODE_ADDRESS = "EM40060";

        private const string CAMERA_NO_HEADER_NAME = "高倍/低倍検査中";

        public static ReadOnlyCollection<string> DESELECT_MARKING_TARGET_CD = Array.AsReadOnly(new string[] { "S" });

		public bool IsMappingFunction { get; set; }

		public string HeartBeatAddress { get; set; }
		public string RequestTypeCdAddress { get; set; }
		public string ResponseTypeCdAddress { get; set; }
		public string CompleteOutputMachineLogAddress { get; set; }
		public string MappingFunctionAddress { get; set; }
		public string ChangePointAddress { get; set; }
		public string LoaderQrdataAddress { get; set; }
		public string MappingStartAddress { get; set; }
		public string MissingInspectionAddress { get; set; }
		public string OperationalStatusAddress { get; set; }

		public string MagazineInspectionStepAddress { get; set; }
		public List<FrameInspectionSetting> frameInspectionSetting { get; set; }
		public List<MainteTargetMachineAddress> mainteTargetMachineAddress { get; set; }
        public string ChangeInspAddrOrQtyMode { get; set; }
		
		//共通化出来る処理は共通のルーチンへ!!!
		//外部システム・装置との連携部の処理にはきめ細かなログ出力を!!!

		public Inspector() {}

		public void MainWork()
		{
			try
			{
				// ハートビートON
				setHeartBeat(true);

#if DEBUG
                HideLog("debug mode");
#endif

				// 2014/10/2 型番送信はNAMIから除去できないか検討
				// 装置からの型番要求確認
				if (isRequestTypeCD() && isOperationalStatus())
				{
					// 装置のQRデータからロット情報取得
					string qrStr = base.GetHoldMagazineLotNo(this.LoaderQrdataAddress);

					Lot lot = null;

					lot = Lot.GetData(qrStr);

					if(lot == null)
					{
						Thread.Sleep(500);
						return;
					}

					InfoLog("[開始時] マッピング処理開始", lot.LotNo);
										
					if (hasWorkStartResult(lot.LotNo) == false) 
					{
						throw new ApplicationException(string.Format("ARMS開始実績が存在しません。ロットNO:{0}", lot.LotNo));
					}
					
					// マッピングデータ送信処理
					mappingDataSendProcess(lot);
					InfoLog("マッピングデータ送信完了", lot.LotNo);

					// 変化点フラグ送信(※True, Falseどちらでも必ず送信する )
					changePointSendProces(lot.IsChangePoint);
					InfoLog(string.Format("変化点フラグ送信完了: {0}", lot.IsChangePoint), lot.LotNo);

					// 製品型番送信(検査機はこれをトリガに検査を始める)
					typeCDSendProcess(lot.TypeCd);
					InfoLog(string.Format("製品型番送信完了: {0}", lot.TypeCd), lot.LotNo);

					// NAMI処理済フラグON
					WorkResult.CompleteStartProcess(lot.LotNo, Config.Settings.InspectProcNo, this.NascaPlantCD);
					InfoLog("[開始時] マッピング処理完了", lot.LotNo);
				}

				// MMファイル出力確認
				if (isMachineLogOutputCompletion())
				{

					// ハートビートOFF
					setHeartBeat(false);

					FileInfo mm = MachineLog.GetLatest(this.WatchingDirectoryPath, MMFile.FILE_KIND, MMFile.DATESTARTINDEX_FROM_MM_ON_FILENAME);
					if (mm == null) 
					{
						if (isMachineLogOutputCompletion())//MMファイル取得中にタイムラグで移動される場合がある為、出力完了信号を再確認
						{
							throw new ApplicationException(
									string.Format(CommonProcess.NOT_FOUND_MACHINELOG_MSG +
									"\r\nMMファイルが処理されていない為、検査機へ再投入して下さい。\r\n" +
									"MMファイルを処理せず流動した場合、MD装置へ検査NGのマッピングアドレスが伝わらず\r\n" +
									"不良品流出の危険性がある為、注意して下さい。\r\n\r\n" + CommonProcess.HOW_TO_MACHINELOG_MAINTE_MSG,
									"MM", this.WatchingDirectoryPath));
						}
						else
						{
							Machine.MachineBase.CommonHideLog(string.Format("{0}ファイル出力信号確認後、ファイルが移動されました", MMFile.FILE_KIND));
							return;
						}
					}

					if (!MMFile.IsFileNameAddLot(mm))
					{
						// ARMS処理待ち(ファイル名にロット情報付与)
						return;
					}

					MMFile mmFile = new MMFile(mm, this);
                    if (MMFile.IsUnknownFileName(mmFile))
                    {
                        // MMファイル内容がヘッダのみの空ファイルを想定　処理をスルーしてEICSに削除処理を任せる
                        return;
                    }

					if (WorkResult.IsCompleteEndProcess(mmFile.LotNo, mmFile.ProcNo))
					{
						// NAMI処理済み
						return;
					}

					// MMファイル処理(検査後照合、マッピングファイル作成)
					InfoLog("[完了時] マッピング処理開始", mmFile.LotNo);

					Lot lot = Lot.GetData(mmFile.LotNo);
					Magazine.Config magConfig = Magazine.Config.GetData(lot.TypeCd);

					Dictionary<int, string> inspectionMacSetting = new Dictionary<int, string>();

					// 検査機の検査設定箇所を取得
					if (lot.IsFullSizeInspection)
					{
						
					}
					else
					{
						inspectionMacSetting = getInspectionMachineSetting(magConfig.PackageQtyY, magConfig.TotalFramePackage, magConfig.StepNum, lot.IsChangePoint);
					}

					// 検査機がマーキング指示データ要求モードかチェック
					// 要求モードの情報をmmFile.Run()に引数として与えてRun()内でマーキング結果照合するのが良いかもしれない 2016/2/24 n.yoshi

					bool isEnableMarkingRequest = isEnableMarkingRequestMode();

                    if (isEnableMarkingRequest)
                    {
                        InfoLog("装置:マーキングアドレス要求モードON", mmFile.LotNo, mmFile.MagazineNo);
                    }
                    else
                    {
                        InfoLog("装置:マーキングアドレス要求モードOFF", mmFile.LotNo, mmFile.MagazineNo);
                    }
					mmFile.Run(inspectionMacSetting, lot, isEnableMarkingRequest);

					if (mmFile.IsSuccessInspection == false)
					{
						string strDifferentAddr = string.Empty;

						foreach (int addr in mmFile.DifferentAddressList)
						{
							int lastRow = (int)(strDifferentAddr.Count() / Log.CellStrLenPerRow);
							strDifferentAddr += addr.ToString() + ", ";

							if ((int)(strDifferentAddr.Count() / Log.CellStrLenPerRow) > lastRow)
							{
								strDifferentAddr += "\r\n";
							}
						}

						ExclamationLog(string.Format("【検査内容不整合】検査必要箇所と検査箇所(MMファイル)不一致\r\n{0}", strDifferentAddr), mmFile.LotNo, mmFile.MagazineNo);

						if (isEnableMarkingRequest)
						{
                            if (mmFile.IsSuccessMarking == false)
                            {
                                ExclamationLog(string.Format("{0}", mmFile.MarkingVerifyErrMsg), mmFile.LotNo, mmFile.MagazineNo);
                            }
                            else
                            {
                                InfoLog("マーキング箇所照合OK", mmFile.LotNo, mmFile.MagazineNo);
                            }
						}

						SendResultAfterInspection(false);
					}
					else
					{
						// 検査NG箇所、マーキング指示箇所が全てマーキングされているか照合 2016/2/24 n.yoshi
						//ここで照合結果を確認
						if (isEnableMarkingRequest)
						{
							if(mmFile.IsSuccessMarking == false)
							{
								ExclamationLog(string.Format("{0}", mmFile.MarkingVerifyErrMsg), mmFile.LotNo, mmFile.MagazineNo);
								SendResultAfterInspection(false);
							}
                            else
                            {
                                InfoLog("マーキング箇所照合OK", mmFile.LotNo, mmFile.MagazineNo);
                            }
						}

						SendResultAfterInspection(true);
					}

					// NAMI処理済フラグON(EICS処理開始)
					WorkResult.CompleteEndProcess(mmFile.LotNo, mmFile.ProcNo, this.NascaPlantCD);
					InfoLog("[完了時] マッピング処理完了", mmFile.LotNo);
				}

				// ハートビートOFF
				setHeartBeat(false);
			}
			catch(ApplicationException err)
			{
				throw new MachineException(this.ClassNM, this.MachineNM, err);
			}
		}

		private void setHeartBeat(bool value)
		{
			int addrSize = mainteTargetMachineAddress.Count;

            int[] valueArray = new int[addrSize];
            for (int i = 0; i < addrSize; i++)
            {
                valueArray[i] = Convert.ToInt16(value);
            }


			Plc.SendMultiValue(this.HeartBeatAddress, valueArray, Keyence.Suffix_U);
		}

		private bool isEnableMarkingRequestMode()
		{
			bool isEnableMarkingRequestMode = Plc.GetBit(MARKING_MODE_ADDRESS);

			return isEnableMarkingRequestMode;

			//if (isEnableMarkingRequestModeStr == PLC.Keyence.BIT_ON)
			//{
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}
		}

		private void setMapping(Dictionary<int, string> mappingData) 
		{
			string[] mappingArray = mappingData.OrderBy(m => m.Key).Select(m => m.Value).ToArray();

			int[] mappingDataArray = Convert2ByteBinaryArrayToDecimalArray(mappingArray);
		
            string logMsg = string.Format("書き込み開始アドレス：{0} / データ：{1}", this.MappingStartAddress, string.Join(" ", mappingDataArray));
            HideLog(logMsg);

            Plc.SendMultiValue(this.MappingStartAddress, mappingDataArray, Keyence.Suffix_U);

            Thread.Sleep(100);
		}

		private void setMarkingMapping(Dictionary<int, string> mappingData)
		{
			string[] mappingArray = mappingData.OrderBy(m => m.Key).Select(m => m.Value).ToArray();

			int[] mappingDataArray = Convert2ByteBinaryArrayToDecimalArray(mappingArray);

			string logMsg = string.Format("書き込み開始アドレス：{0} / データ：{1}", MARKING_MAPPING_START_ADDRESS, string.Join(" ", mappingDataArray));
			HideLog(logMsg);

			Plc.SendMultiValue(MARKING_MAPPING_START_ADDRESS, mappingDataArray, Keyence.Suffix_U);

			Thread.Sleep(100);
		}

		private bool isRequestTypeCD()
		{
			return Plc.GetBitForAI_MD(this.RequestTypeCdAddress);
		}

		private bool isOperationalStatus()
		{
			return Plc.GetBitForAI_MD(this.OperationalStatusAddress);
		}

		private bool isRequestMarkingDataMode()
		{
			return Plc.GetBit(RequestMarkingDataModeAddress);
		}

		private void typeCDSendProcess(string typeCD)
		{
			// 1.送信
			Plc.SendString(this.ResponseTypeCdAddress, typeCD);

			// 2.立ち下げ
			Plc.SetBit(this.RequestTypeCdAddress, false);
		}

		/// <summary>
		/// 変化点フラグの送信
		/// </summary>
		/// <param name="isChangePoint"></param>
		private void changePointSendProces(bool isChangePoint) 
		{
			Plc.SetBit(this.ChangePointAddress, isChangePoint);
		}

		// 装置のファイル出力完了信号のチェック
		private bool isMachineLogOutputCompletion() 
		{
			return Plc.GetBitForAI_MD(this.CompleteOutputMachineLogAddress);
		}

		//// 装置への検査ミスの通知
		//private void sendMissingInspection()
		//{
		//	HideLog(string.Format("装置へ検査ミス通知 / 通知アドレス：{0}", this.MissingInspectionAddress));

		//	Plc.SetBit(this.MissingInspectionAddress, true);
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lsetInfo"></param>
		/// <param name="result">合否の結果(true=合格/false=不合格)</param>
		protected void SendResultAfterInspection(bool result)
		{

			int sendData = 0;

			if (Config.Settings.IsMappingResultInterlockMode)
			{
				if (result)
				{
					//検査後照合結果：合格
					sendData = 2;
				}
				else
				{
					//検査後照合結果：不合格
					sendData = 1;
				}

				HideLog(string.Format("装置へ検査照合結果通知 / 通知アドレス：{0}　送信値：{1}　照合結果通知モード：{2}"
					, this.MissingInspectionAddress, sendData, Config.Settings.IsMappingResultInterlockMode));

				Plc.Send1ByteData(this.MissingInspectionAddress, sendData);
			}
			else
			{
				HideLog(string.Format("装置へ検査照合NG通知モード / 照合結果通知モード：OFFの為"));
				if (result == false)
				{
					HideLog(string.Format("装置へ検査照合結果通知 / 通知アドレス：{0}　送信値：{1}　照合結果通知モード：{2}"
						, this.MissingInspectionAddress, true, Config.Settings.IsMappingResultInterlockMode));

					Plc.SetBit(this.MissingInspectionAddress, true);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lot"></param>
		private void markingDataSendProcess(Lot lot)
		{
			if (isRequestMarkingDataMode())
			{
				InfoLog(string.Format("装置側マーキングアドレス要求モード:ONを確認 addr:{0}", RequestMarkingDataModeAddress));

				//前工程のNGアドレスを取得
			}
			else
			{
				InfoLog(string.Format("装置側マーキングアドレス要求モード:OFFを確認 addr:{0}", RequestMarkingDataModeAddress));
				

			}
		}

		/// <summary>
		/// マッピングデータを装置へ送信する処理
		/// </summary>
		private void mappingDataSendProcess(Lot lot)
		{
			// EICSと装置のマッピング機能ON/OFF照合
			checkMatchMappingFunction();

			// 検査対象か調べる
			if (isTargetInspection(lot.IsFullSizeInspection, lot.IsMappingInspection, lot.IsChangePoint) == false)
			{
				throw new ApplicationException(string.Format("[ロット番号 {0}]検査機対象外です", lot.LotNo));
			}

			Dictionary<int, string> markingMapData = new Dictionary<int, string>();

			// マッピングデータ取得（マッピングデータの変換等に関する処理はこの中で）(マーキングアドレスの取得も行う）
			Dictionary<int, string> mappingData = GetMappingDataForInspection(lot, out markingMapData);

			// 変化点しかない時のマーキングデータ送信、検査箇所送信が必要な気がする 変化点しかないときは装置側で勝手に検査する箇所はこっちで指示必要ない事を確認

			// マッピングデータ送信
			setMapping(mappingData);

			// マーキングアドレス指示データ送信
			setMarkingMapping(markingMapData);
		}

		/// <summary>
		/// EICSと装置のマッピング機能ON/OFF照合
		/// </summary>
		/// <returns></returns>
		private void checkMatchMappingFunction()
		{
			bool retv = Plc.GetBit(this.MappingFunctionAddress);
			if (retv == false)
			{
				throw new ApplicationException(string.Format("外観検査機とLENSでWBマッピング機能(有効/無効)に相違があります。外観検査機:{0} LENS:{1}", 
					retv, "true"));
			}
		}

		/// <summary>
		/// 検査対象か調べる
		/// </summary>
		/// <param name="isFullSizeInspection"></param>
		/// <param name="isInspectionTarget"></param>
		/// <param name="isChangePoint"></param>
		/// <returns>検査対象であればtrue</returns>
		internal bool isTargetInspection(bool isFullSizeInspection, bool isMappingInspection, bool isChangePoint)
		{
			if (isFullSizeInspection == false && isMappingInspection == false && isChangePoint == false)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 全数検査の為のオール1のマッピングデータを取得
		/// </summary>
		/// <param name="packageXNum"></param>
		/// <param name="packageYNum"></param>
		/// <param name="FrameNum"></param>
		/// <returns></returns>
		private static Dictionary<int, string> getAllInspectionMappingData(int totalMagazinePackage)
		{
			Dictionary<int, string> retv= new Dictionary<int, string>();

			//全数検査マッピングデータを返す
			retv = MappingFile.Data.GetAnySize(totalMagazinePackage, "1");

			return retv;
		}

		/// <summary>
		/// 変化点検査の為のオール0のマッピングデータを取得（この関数を呼び出す場合、対象ﾛｯﾄが必ず変化点フラグ1で有ることを保証しなければならない）
		/// </summary>
		/// <param name="changePointLotFg">ロットの変化点フラグ</param>
		/// <param name="totalMagazinePackage">ﾛｯﾄのマガジン総パッケージ数(マッピングファイル当たりの総パッケージ数)</param>
		/// <returns></returns>
		private static Dictionary<int, string> getAllZeroInspectionMappingData(bool changePointLotFg, int totalMagazinePackage)
		{
			if (changePointLotFg)
			{
				Dictionary<int, string> retv = new Dictionary<int, string>();

				//全数検査マッピングデータを返す
				retv = MappingFile.Data.GetAnySize(totalMagazinePackage, "0");

				return retv;
			}
			else
			{
				throw new ApplicationException("変化点検査の対象で無い(変化点Fg=0)ﾛｯﾄに対してﾏｯﾋﾟﾝｸﾞｵｰﾙ0(WBﾏｯﾋﾟﾝｸﾞ検査箇所無し)のﾏｯﾋﾟﾝｸﾞﾃﾞｰﾀの生成をしようとした為、ﾌｪｰﾙｾｰﾌの為ｴﾗｰ停止します。");
			}
		}

		internal Dictionary<int, string> GetMappingDataForInspection(Lot lot, out Dictionary<int, string> markingMapData)
		{
			Dictionary<int, string> retv = new Dictionary<int, string>();
			markingMapData = new Dictionary<int, string>();

			bool hasNeedReverseFramePlacement = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(
				lot.LotNo, Config.Settings.WirebondProcNo, Config.Settings.InspectProcNo, false);

			if (lot.IsFullSizeInspection)
			{
				Magazine.Config magConfig = Magazine.Config.GetData(lot.TypeCd);
				retv = getAllInspectionMappingData(magConfig.TotalMagazinePackage);
			}
			else if (lot.IsMappingInspection)
			{
				Dictionary<int, string> mappingData = MappingFile.Data.Wirebonder.Get(lot.LotNo, hasNeedReverseFramePlacement);
				markingMapData = MappingFile.Data.Inspector.ConvertToMarkingMapData(mappingData, DESELECT_MARKING_TARGET_CD.ToArray());

				HideLog(string.Format("【AI用マッピング取得】WBマッピングデータの上下反転有無：{0} / ロットNo:{1} WB-Proc(Config):{2} MD-Proc(Config):{3}"
					, hasNeedReverseFramePlacement, lot.LotNo, Config.Settings.WirebondProcNo, Config.Settings.InspectProcNo));

				retv = MappingFile.Data.Wirebonder.ConvertToInspection(mappingData);
			}
			else
			{
				Magazine.Config magConfig = Magazine.Config.GetData(lot.TypeCd);
				retv = getAllZeroInspectionMappingData(lot.IsChangePoint, magConfig.TotalMagazinePackage);
			}

			return retv;
		}



		/// <summary>
		/// 外観検査機の検査設定箇所データからマッピングデータを作成。(wbmファイルと同じデータの並び）
		/// </summary>
		/// <param name="totalFramePackage"></param>
		/// <param name="magazineStep"></param>
		/// <param name="isChangePoint"></param>
		/// <returns></returns>
		private Dictionary<int, string> getInspectionMachineSetting(int packageQtyY, int totalFramePackage, int magazineStep, bool isChangePoint)
		{
			List<string> inspectedFrameMapping = new List<string>();
			List<string> unInspectedFrameMapping = new List<string>();

			for (int pkgIndex = 0; pkgIndex < totalFramePackage; pkgIndex++)
			{
				unInspectedFrameMapping.Add("0");// 検査無しの段のフレームのマッピングデータの設定
				inspectedFrameMapping.Add("0");// 検査ありの段のフレームのマッピングデータを0埋め初期化
			}

			int inspectEndMAPAddressOld = 0;

			if (frameInspectionSetting == null || frameInspectionSetting.Count == 0)
			{
				return null;
			}

			// 最大4エリア分の各エリアの検査範囲の開始アドレスと終了アドレスを取得する。
			foreach(FrameInspectionSetting frameArea in frameInspectionSetting)
			{
				string sInspectStartMAPAddress = Plc.GetData(frameArea.AreaStartAddress, 1);
				string sInspectEndMAPAddress = Plc.GetData(frameArea.AreaEndAddress, 1);

                HideLog(string.Format("【検査後照合】検査エリア:{0} 開始アドレス{1},完了アドレス{2}", frameArea.AreaNo, sInspectStartMAPAddress, sInspectEndMAPAddress));

				int inspectStartMAPAddress;
				int inspectEndMAPAddress;

				// 検査開始アドレスの数値変換
				if (!int.TryParse(sInspectStartMAPAddress, out inspectStartMAPAddress))
				{
					throw new ApplicationException(
						string.Format("検査開始/完了アドレスデータが数値ではありません。 取得データ：{0}", sInspectStartMAPAddress));
				}
				// 検査完了アドレスの数値変換
				if (!int.TryParse(sInspectEndMAPAddress, out inspectEndMAPAddress))
				{
					throw new Exception(
						string.Format("検査開始/完了アドレスデータが数値ではありません。 取得データ：{0}", sInspectEndMAPAddress));
				}

				// 検査開始アドレス、完了アドレスが共に0の場合、当該エリアの処理は飛ばす
				if (inspectStartMAPAddress == 0 && inspectEndMAPAddress == 0)
				{
					continue;
				}

				// 検査開始アドレス、完了アドレスが1～フレーム内のパッケージ数の範囲内に収まっているか
				if (inspectStartMAPAddress < 0 || inspectEndMAPAddress < 0 || inspectStartMAPAddress > totalFramePackage || inspectEndMAPAddress > totalFramePackage)
				{
					throw new Exception(
						string.Format("検査開始・完了アドレス不正（検査開始アドレスもしくは検査完了アドレスが1～{2}の範囲になっていません。）\r\n検査開始アドレス：{0}/検査完了アドレス：{1}", inspectStartMAPAddress, inspectEndMAPAddress, totalFramePackage));
				}
				// 検査開始アドレスよりも検査完了アドレスが前に来ていないかのチェック
				if (inspectStartMAPAddress > inspectEndMAPAddress)
				{
					throw new Exception(
						string.Format("検査開始・完了アドレス不正（開始・完了アドレス大小逆）開始アドレス：{0}/完了アドレス：{1}", inspectStartMAPAddress, inspectEndMAPAddress));
				}
				// ひとつ前のエリアの検査完了アドレスよりも検査開始アドレスが前に来ていないかのチェック
				if (inspectStartMAPAddress <= inspectEndMAPAddressOld)
				{
					throw new Exception(
						string.Format("検査開始・完了アドレス不正（検査エリア{2}の開始アドレスと検査エリア{3}の完了アドレスの大小逆）開始アドレス：{0}/完了アドレス：{1}", inspectStartMAPAddress, inspectEndMAPAddressOld, frameArea.AreaStartAddress, frameArea.AreaEndAddress));
				}

				// 検査開始アドレス～検査終了アドレスまでの範囲に検査済みを示す1を設定（開始/完了アドレスは1始まりの為、-1のオフセット加えて処理）
				// 装置のつづら折り動作に対応して無いので廃止
				//for (int pkgIndex = inspectStartMAPAddress - 1; pkgIndex <= inspectEndMAPAddress - 1; pkgIndex++)
				//{
				//	inspectedFrameMapping[pkgIndex] = "1";
				//}

				// 装置のつづら折り動作に対応する為以下を追加
                List<int> areaIndexList = getInspectedIndexBetweenPoints(inspectStartMAPAddress, inspectEndMAPAddress, packageQtyY);

				foreach (int pkgIndex in areaIndexList)
				{
					inspectedFrameMapping[pkgIndex] = "1";
				}

				inspectEndMAPAddressOld = inspectEndMAPAddress;
			}

			List<bool> inspectedStepFlag = getInspectedFlagPerMagazineStepList(magazineStep);

            //下記ログはエラーが出たから一旦消します ←　修正済み　n.yoshimoto
            HideLog(string.Format("【検査後照合】M'g各段検査ステータス:{0}", string.Join(" ", inspectedStepFlag.ConvertAll(i => Convert.ToInt16(i)))));

			List<string> inspectedMappingDataList = new List<string>();

			for (int stepIndex = 0; stepIndex < inspectedStepFlag.Count; stepIndex++)
			{
				// stepIndexで指定した段が検査してない段の場合、その段は全て未検査と言う事で0埋め
				if (inspectedStepFlag[stepIndex] == true)
				{
					inspectedMappingDataList.AddRange(inspectedFrameMapping);
				}
				else
				{
					inspectedMappingDataList.AddRange(unInspectedFrameMapping);
				}
			}

			// 変化点フラグ=1なのに、抜き取り検査箇所が無い場合、エラー出力
			if (inspectedMappingDataList.Exists(i => i == "1") == false && isChangePoint == true)
			{
				throw new ApplicationException("検査箇所未設定[変化点フラグ=1(抜き取り検査有り)に対して検査機の検査設定箇所が無し]");
			}

			return MappingFile.Data.ConvertToAddressData(inspectedMappingDataList);
		}

		/// <summary>
		/// 検査機が指定の2点間を検査する際のマッピングデータのインデックス(0始まり)のリストを返す
		/// </summary>
		/// <param name="startAddress"></param>
		/// <param name="endAddressValue"></param>
		/// <param name="packageQtyY"></param>
		/// <param name="inspectedFrame"></param>
		/// <returns></returns>
		private List<int> getInspectedIndexBetweenPoints(int startAddress, int endAddressValue, int packageQtyY)
		{

            // ADDRモードの場合、開始アドレスと完了アドレスをつづら折りにした時(実際に検査した順番)のアドレスに変更する。
            if (this.ChangeInspAddrOrQtyMode.ToUpper() == ADDR_MODE)
            {
                startAddress = getWindingAddress(startAddress, packageQtyY);

                endAddressValue = getWindingAddress(endAddressValue, packageQtyY);

                HideLog(string.Format("【検査後照合】    マッピングデータ取得開始 開始アドレス{0}, 完了アドレス{1}", startAddress.ToString(), endAddressValue.ToString()));
            }
            
			int targetAddress = startAddress;
			List<int> indexList = new List<int>();
            int pkgCt = 0;

            while (isLoopGetInspectedIndex(targetAddress, pkgCt, endAddressValue))
			{
				int colOnFrame = (int)Math.Ceiling(targetAddress / (double)packageQtyY);

				int additionalVal;

				if (colOnFrame % 2 == 1)
				{
					additionalVal = 1;
				}
				else
				{
					additionalVal = -1;
				}
		
				int targetIndex = targetAddress - 1;
				indexList.Add(targetIndex);

				if ((targetAddress % packageQtyY == 0) && additionalVal == 1)
				{
					targetAddress += packageQtyY;
				}
                else if ((targetAddress % packageQtyY == 1) && additionalVal == -1)
                {
                    targetAddress += packageQtyY;
                }
                else
                {
                    targetAddress += additionalVal;
                }

                pkgCt++;
			}

            if (this.ChangeInspAddrOrQtyMode.ToUpper() == ADDR_MODE)
            {
                int endIndex = endAddressValue - 1;
                indexList.Add(endIndex);
            }

			return indexList;
		}

        /// <summary>
		/// 指定のアドレスを打点番号に変換する
		/// </summary>
		/// <param name="startAddress"></param>
		/// <param name="endAddressValue"></param>
		/// <param name="packageQtyY"></param>
		/// <param name="inspectedFrame"></param>
		/// <returns></returns>
        private int getWindingAddress(int tmpAddress, int packageQtyY)
        {
            // 返り値
            int retV = 0;

            // 対象アドレスが0の場合、0を返す。
            if (tmpAddress == 0) return 0;

            // 対象アドレスの フレーム上における 行・列番号を算出
            int colOnFrame = (int)Math.Ceiling(tmpAddress / (double)packageQtyY);
            int rowOnFrame = tmpAddress - ((colOnFrame - 1) * packageQtyY);

            // 奇数列 (打点方向 ： ↓)
            if (colOnFrame % 2 == 1)
            {
                // アドレスの行Noに変更なし
            }
            // 偶数列 (打点方向 ： ↑)
            else
            {
                // 行Noを上下反転にする
                rowOnFrame = packageQtyY - rowOnFrame + 1;
            }
            
            // 打点番号 = 変更後の行No + (直前までの列数 × 縦pcs数)
            retV = rowOnFrame + ((colOnFrame - 1) * packageQtyY);

            return retV;
        }

        private bool isLoopGetInspectedIndex(int targetAddr, int pkgCt, int endCondValue)
        {
            if (this.ChangeInspAddrOrQtyMode.ToUpper() == ADDR_MODE)
            {
                if (targetAddr != endCondValue)
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
                if (pkgCt < endCondValue)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

		/// <summary>
		/// マガジンの段毎の検査済みフラグのリストを取得
		/// </summary>
		/// <param name="lsetInfo"></param>
		/// <param name="frameInfo"></param>
		/// <returns>マガジンの段毎の検査済みフラグのリスト</returns>
		private List<bool> getInspectedFlagPerMagazineStepList(int magazineStep)
		{
			string sInspectedFG = Plc.GetData(MagazineInspectionStepAddress, magazineStep);

			List<bool> inspectedFlagList = new List<bool>();

			foreach (string bitStr in sInspectedFG.Split(' '))
			{
				int bitData;
				if (!int.TryParse(bitStr, out bitData))
				{
					throw new Exception(string.Format("検査済み段のフラグ情報が数値ではありません。変換対象データ：{0}", bitStr));
				}

				if (bitData == 1)
					inspectedFlagList.Add(true);
				else if (bitData == 0)
					inspectedFlagList.Add(false);
				else
					throw new Exception(string.Format("【外観検査機 不正応答】 受信データ：{0}", bitStr));
			}

			return inspectedFlagList;
		}

		/// <summary>
		/// ARMSの開始実績が存在するか確認する　存在なしの場合、30
		/// </summary>
		/// <param name="lotNo"></param>
		/// <param name="startDt"></param>
		/// <returns></returns>
		private bool hasWorkStartResult(string lotNo, int retryCt)
		{
			if (retryCt >= Config.Settings.DatabaseAccessRetryCount)
			{
				return false;
			}

			LENS2_Api.ARMS.WorkResult inspectResult = LENS2_Api.ARMS.WorkResult.GetData(lotNo, Config.Settings.InspectProcNo);
			if (inspectResult == null)
			{
				Thread.Sleep(3000);
				retryCt = retryCt + 1;
				if (retryCt >= 2)
				{
					InfoLog(string.Format("ARMS開始実績登録待ち リトライ回数:{0}", retryCt), lotNo);
				}

				return hasWorkStartResult(lotNo, retryCt);
			}
			else
			{
				return true;
			}
		}
		private bool hasWorkStartResult(string lotNo) 
		{
			return hasWorkStartResult(lotNo, 0);
		}

		public class MMFile : MachineLog
		{
			public const string FILE_KIND = "MM";
            public const string FILE_UNKNOWN = "UNKNOWN";

			public const int CONTENTS_HEADER_ROW = 1;
			public const int CONTENTS_HEADEREND_ROW = 2;

			/// <summary>内容(マガジン番号)</summary>
			public const int CONTENTS_MAGAZINE_COL = 3;

			/// <summary>内容(ロギングアドレス)</summary>
			public const int CONTENTS_ADDRESS_COL = 4;

			/// <summary>内容(検査結果CD)</summary>
			public const int CONTENTS_RESULT_COL = 5;

			/// <summary>内容(傾向管理フラグ)</summary>
			public const int CONTENTS_KEIKOUFG_COL = 6;

			/// <summary>内容(変化点フラグ)</summary>
			public const int CONTENTS_CHANGEFG_COL = 7;

			public const string CONTENTS_KEIKOUFG_ON = "1";
			public const string CONTENTS_KEIKOUFG_OFF = "0";

			/// <summary>
			/// 検査後照合の状態値
			/// NONE = 0, NG = 1, OK = 2 なので並べ替え不可
			/// </summary>
			public enum InspectionResultCD
			{
				NONE,
				NG,
				OK,
			}


			public const int DATESTARTINDEX_FROM_MM_ON_FILENAME = 2;

			public FileInfo Info { get; set; }

			public Dictionary<int, string> Contents { get; set; }

			public string MagazineNo { get; set; }

			public string LotNo { get; set; }

			public string TypeCd { get; set; }

			public int ProcNo { get; set; }

			public IMachine WorkMachine { get; set; }

			// 検査後照合の結果
			public bool IsSuccessInspection { get; set; }

			// マーキング箇所照合の結果
			public bool IsSuccessMarking { get; set; }

            // 検査後照合の不一致アドレスのリスト 
            public List<int> DifferentAddressList { get; set; }

			// マーキング箇所照合の不一致アドレスのリスト 
			public List<int> DifferentMarkingAddressList { get; set; }

			public string MarkingVerifyErrMsg { get; set; }

            // 「高倍/低倍検査中」の列番号
            private int camNoColIdx { get; set; }

			public MMFile(FileInfo mmfile, IMachine workMachine) 
			{
                DifferentAddressList = new List<int>();
				DifferentMarkingAddressList = new List<int>();
				this.Info = mmfile;
				this.WorkMachine = workMachine;

				getLotFromFileName();

				this.IsSuccessMarking = false;
				this.IsSuccessInspection = false;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="lotNo"></param>
			/// <returns></returns>
			public static MMFile GetCompletedFile(string lotNo, IMachine workMachine)
			{
				// 2014/10/2 ロット毎のフォルダへのバックアップはやめる
				string lotNoDirPath = Path.Combine(Config.Settings.ForAICompareMachineLogDirPath, lotNo);
				if (!System.IO.Directory.Exists(lotNoDirPath))
				{
					throw new ApplicationException(
						string.Format("MMファイルの保管フォルダが見つかりません。フォルダパス:{0}", lotNoDirPath));
				}

				FileInfo mmFile = GetLatest(lotNoDirPath, MMFile.FILE_KIND, MMFile.DATESTARTINDEX_FROM_MM_ON_FILENAME);
				if (mmFile == null)
				{
					throw new ApplicationException(
						string.Format("MMファイルが見つかりません。フォルダパス:{0}", lotNoDirPath));
				}
				else
				{
					return new MMFile(mmFile, workMachine);
				}
			}

			private void getLotFromFileName()
			{
				if (MMFile.IsFileNameAddLot(this.Info) == false)
				{
					throw new ApplicationException(
						string.Format("MMファイル名にロット情報がありません。ファイルパス:{0}", this.Info.FullName));
				}

				string[] nameChar = Path.GetFileNameWithoutExtension(this.Info.Name).Split('_');
				this.LotNo = nameChar[2].Trim();
				this.TypeCd = nameChar[3].Trim();
				this.ProcNo = int.Parse(nameChar[4].Trim());
			}

			public static bool IsFileNameAddLot(FileInfo file)
			{
				string[] nameChar = Path.GetFileNameWithoutExtension(file.FullName).Split('_');
				if (nameChar.Count() < 5)
				{
					return false;
				}
				else { return true; }
			}

            public static bool IsUnknownFileName(MMFile mmFile) 
            {
                if (mmFile.LotNo == FILE_UNKNOWN)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

			public Dictionary<int, string> GetData()
			{
				List<int> markingAddress = new List<int>();
				return GetData(false, out markingAddress);
			}

			public Dictionary<int, string> GetData(bool isEnableMarkingRequest, out List<int> markingAddress) 
			{
				markingAddress = new List<int>();
				int markingAddressHeaderColIndex;

				string[] fileValue = FileWrapper.ReadAllLines(this.Info.FullName);
				
				if (isEnableMarkingRequest)
				{
					markingAddressHeaderColIndex = getMarkingAddressHeaderColIndex(fileValue);
					// マーキングアドレスを取得
					markingAddress = getMarkingAddress(fileValue, markingAddressHeaderColIndex);
				}

                camNoColIdx = getCameraNoHeaderColIndex(fileValue);

				// ヘッダ部除去
				fileValue = fileValue.Skip(CONTENTS_HEADEREND_ROW).ToArray();
				if (fileValue.Count() == 0)
				{
					// ヘッダ部のみの空ファイルの場合、処理停止
					throw new ApplicationException(
						string.Format("空のMMファイルが読み込まれました。出力ファイルを確認して下さい。出力ファイル:{0}", Info.FullName));
				}

				// ファイル内のマガジン番号取得
				string magazineQR = getMagazineQR(fileValue, CONTENTS_MAGAZINE_COL);

				return getInspectionResult(fileValue, magazineQR);
			}

			/// <summary>
			/// MMファイル中のマーキングアドレス列のインデックスを取得
			/// </summary>
			/// <param name="fileValue"></param>
			/// <returns></returns>
			private int getMarkingAddressHeaderColIndex(string[] fileValue)
			{
				int colIndex = -1;

				string[] headerArray = fileValue[CONTENTS_HEADER_ROW].Split(',');

				for (int i = 0; i < headerArray.Length; i++)
				{
					if (headerArray[i].Contains(MARKING_ADDRESS_HEADER_NAME))
					{
						colIndex = i;
						break;
					}
				}

				return colIndex;
			}

			private  List<int> getMarkingAddress(string[] fileValue, int markingAddressColIndex)
			{
				List<int> markingAddressList = new List<int>();

				fileValue = fileValue.Skip(CONTENTS_HEADEREND_ROW).ToArray();

				for (int i = 0; i < fileValue.Count(); i++)
				{
					if (string.IsNullOrWhiteSpace(fileValue[i]))
					{
						// 空白行の場合、次へ
						continue;
					}
					string[] fileLineValue = fileValue[i].Split(',');

					int address = Convert.ToInt32(fileLineValue[markingAddressColIndex].Trim());

					if (address == 0)
					{
						continue;
					}

					if (markingAddressList.Count(m => m == address) == 0)
					{
						markingAddressList.Add(address);
					}
				}

				return markingAddressList;
			}

            /// <summary>
            /// MMファイル中のカメラNo列のインデックスを取得
            /// </summary>
            /// <param name="fileValue"></param>
            /// <returns></returns>
            private int getCameraNoHeaderColIndex(string[] fileValue)
            {
                int colIndex = -1;

                string[] headerArray = fileValue[CONTENTS_HEADER_ROW].Split(',');

                for (int i = 0; i < headerArray.Length; i++)
                {
                    if (headerArray[i].Contains(CAMERA_NO_HEADER_NAME))
                    {
                        colIndex = i;
                        break;
                    }
                }

                return colIndex;
            }

			private Dictionary<int, string> getInspectionResult(string[] fileValue, string magazineQR)
			{
				Dictionary<int, string> retv = new Dictionary<int, string>();
                List<MMFileRowData> rowList = new List<MMFileRowData>();
                for (int i = 0; i < fileValue.Count(); i++)
				{
					if (string.IsNullOrWhiteSpace(fileValue[i]))
					{
						// 空白行の場合、次へ
						continue;
					}
					string[] fileLineValue = fileValue[i].Split(',');

					if (fileLineValue[CONTENTS_MAGAZINE_COL].Trim().Replace("\"", "").Replace("\r", "").Equals(magazineQR) == false)
					{
						// 指定したマガジンQR値と等しくない場合、次へ
						continue;
					}

					if (fileLineValue[CONTENTS_KEIKOUFG_COL].Trim() == CONTENTS_KEIKOUFG_OFF)
					{
						// 傾向管理しないデータの場合、次へ
						continue;
					}

					int address = Convert.ToInt32(fileLineValue[CONTENTS_ADDRESS_COL].Trim());

                    if (address == 0)
                    {
                        continue;
                    }

                    // 【高倍/低倍検査中】列の値を取得   列無し   
                    int camerano = 0;
                    if (camNoColIdx > -1)
                    {
                        string sCameraNo = fileLineValue[camNoColIdx].Trim();
                        if (int.TryParse(sCameraNo, out camerano) == false)
                        {
                            throw new ApplicationException(string.Format("{0}行目の【高倍/低倍検査中】列の値が数字ではありません。値=「{1}」", (i + 1).ToString(), sCameraNo));
                        }
                        camerano = Convert.ToInt32(fileLineValue[camNoColIdx].Trim());
                    }


					string result = fileLineValue[CONTENTS_RESULT_COL].Trim();
                    // 同じアドレス・カメラNoの古い要素を削除
                    rowList.RemoveAll(r => r.Address == address && r.CameraNo == camerano);

                    // 新しいアドレス・カメラNoの
                    MMFileRowData newItem = new MMFileRowData();
                    newItem.Address = address;
                    newItem.CameraNo = camerano;
                    newItem.Result = result;
                    rowList.Add(newItem);
                }

                // 取得データから各アドレス毎のNGデータを出力 (NGが無い場合は、配列の先頭のエラーCDを返す)
                Dictionary<string, string> convMaster = MapConv.GetData("AI_MM", "AI_OUT");
                foreach (int address in rowList.Select(r => r.Address).Distinct())
                {
                    foreach (MMFileRowData rowData in rowList.FindAll(r => r.Address == address))
                    {
                        bool isNG = false;

                        // NGであるかの確認 (変換マスタに存在しないCD または 変換後の値が0以外 ⇒ NG扱い)
                        if (!convMaster.ContainsKey(rowData.Result) || convMaster[rowData.Result] != "0")
                        {
                            isNG = true;
                        }

                        // マッピングデータの配列に対象アドレスのデータが無い場合、(ループ1回目)
                        if (retv.Keys.Count(k => k == address) == 0)
                        {
                            retv.Add(address, rowData.Result);
                        }
                        else if (isNG)
                        {
                            // NGであるなら、上書きをしてループを抜ける。
                            retv[address] = rowData.Result;
                            break;
                        }
                        else
                        {
                            // 処理なし。次の「高倍/低倍検査中」の行を見に行く。
                        }

                    }
                }

                return retv;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="inspectPointSetting">検査機の検査設定箇所</param>
			public void Run(Dictionary<int, string> inspectionMacSetting, Lot lot, bool isEnableMarkingRequest)
			{
				List<int> markingAddressList = new List<int>();

				Magazine.Config magConfig = Magazine.Config.GetData(lot.TypeCd);

				// 検査結果取得
				this.Contents = GetData(isEnableMarkingRequest, out markingAddressList);
				
				// WBマッピングデータがあれば検査設定と合わせる為、搭載段の上下反転が必要か情報取得しておく
				bool hasNeedReverseFramePlacement = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lot.LotNo, Config.Settings.WirebondProcNo, Config.Settings.InspectProcNo, true);

				// 検査後照合
				if (lot.IsFullSizeInspection)
				{
					inspectionMacSetting = getAllInspectionMappingData(magConfig.TotalMagazinePackage);
				}
				else if (lot.IsMappingInspection) 
				{
					// MMファイルのクラスでMachineBaseを継承してないので、下記のログ出力関数を使えないので次回、この周辺を改修する際に修正
					//HideLog(string.Format("【AI後照合】WBマッピングデータの上下反転有無：{0} / ロットNo:{1} WB-Proc(Config):{2} AI-Proc(Config):{3}", hasNeedReverseFramePlacement, lot.LotNo, Config.Settings.WirebondProcNo, Config.Settings.InspectProcNo));

					Dictionary<int, string> wireMappingData = MappingFile.Data.Wirebonder.Get(lot.LotNo, hasNeedReverseFramePlacement);

					wireMappingData = MappingFile.Data.Wirebonder.ConvertToInspection(wireMappingData);

					if (inspectionMacSetting != null && inspectionMacSetting.Count > 0)
					{
						inspectionMacSetting = MappingFile.Data.Union(inspectionMacSetting, wireMappingData);
					}
				}

				if (inspectionMacSetting != null && inspectionMacSetting.Count > 0)
				{
					this.IsSuccessInspection = getInspectionVerification(inspectionMacSetting, this.Contents);
				}
				else
				{
					this.IsSuccessInspection = true;
				}

				if (isEnableMarkingRequest)
				{
					Dictionary<int, string> markingTargetData = new Dictionary<int, string>();

					Dictionary<int, string> wireMappingData = MappingFile.Data.Wirebonder.Get(lot.LotNo, hasNeedReverseFramePlacement);
					markingTargetData = MappingFile.Data.Inspector.ConvertToMarkingMapData(wireMappingData, DESELECT_MARKING_TARGET_CD.ToArray());

					//マーキング箇所照合
					string markingVerifyErrMsg = string.Empty;
					this.IsSuccessMarking = getMarkingVerificationResult(this.Contents, markingTargetData, markingAddressList, magConfig.TotalMagazinePackage);
				}

				// MDマッピング用データ取得
				Dictionary<int, string> mappingData = GetMappingDataForMD(this.Contents, magConfig, lot.IsMappingInspection);

				// MDマッピング用ファイル作成
				createMappingFile(lot.LotNo, mappingData);

				// MMファイルのコピーファイル作成(MD前照合用)
				keepMachineLog(this.Info.FullName, lot.LotNo);

				if (this.IsSuccessInspection)
				{
					lot.InspectionResultCD = (int)MMFile.InspectionResultCD.OK;
				}
				else
				{
					lot.InspectionResultCD = (int)MMFile.InspectionResultCD.NG;
				}

				if (isEnableMarkingRequest)
				{
					if (this.IsSuccessMarking)
					{
						lot.InspectionResultCD = (int)MMFile.InspectionResultCD.OK;
					}
					else
					{
						lot.InspectionResultCD = (int)MMFile.InspectionResultCD.NG;
					}
				}

				lot.InsertUpdate();
			}

			/// <summary>
			/// マッピングデータ同士の照合
			/// </summary>
			/// <param name="referenceData">基準となる方のデータ</param>
			/// <param name="verifyTargetData">間違えている可能性がある方のデータ</param>
			/// <param name="differentAddressList">照合の結果、異なっているアドレスとverifyTargetData側の値</param>
			/// <returns></returns>
			private bool getMappingDataVerificationResult(bool isMustBePerfectMatching, Dictionary<int, string> referenceDataList, Dictionary<int, string> verifyTargetData, out Dictionary<int, string> differentAddressList)
			{
				differentAddressList = new Dictionary<int, string>();

				if (isMustBePerfectMatching)
				{
					foreach (KeyValuePair<int, string> referenceData in referenceDataList)
					{
						if (verifyTargetData.ContainsKey(referenceData.Key) == false)
						{
							differentAddressList.Add(referenceData.Key, referenceData.Value);
						}
						else
						{
							if (referenceData.Value != verifyTargetData[referenceData.Key])
							{
								differentAddressList.Add(referenceData.Key, referenceData.Value);
							}
						}
					}
				}
				else // 完全一致でなくても良い（リファレンス側がOKなら余分にNG処置しているケースは問わない）
				{
					foreach (KeyValuePair<int, string> referenceData in referenceDataList)
					{
						if (referenceData.Value == "0") continue;

						if (verifyTargetData.ContainsKey(referenceData.Key) == false)
						{
							differentAddressList.Add(referenceData.Key, referenceData.Value);
						}
					}
				}

				if (differentAddressList.Count == 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			/// <summary>
			/// 検査後照合（AI装置が検査すべき箇所を検査し異常箇所をMMファイルに出力出来ているかチェック）
			/// </summary>
			/// <param name="setting"></param>
			/// <param name="result">true:チェックOK/false:チェックNG</param>
			private bool getInspectionVerification(Dictionary<int, string> setting, Dictionary<int, string> result)
			{
				foreach(KeyValuePair<int, string> s in setting)
				{
					if (s.Value == "0") continue;

                    if (result.ContainsKey(s.Key) == false)
                    {
                        DifferentAddressList.Add(s.Key);
                    }
				}

                if (DifferentAddressList.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
			}

			private bool getMarkingVerificationResult(Dictionary<int, string> inspectionResult, Dictionary<int, string> markingTargetDataFromWirebond, List<int> markingAddressList, int totalMagazinePackageCt)
			{
				//Dictionary<int, string> inspectionOkNgData = convertToInspectorMarking(inspectionResult, totalMagazinePackageCt);

				//Dictionary<int, string> totalMarkingTargetData = MappingFile.Data.Union(inspectionOkNgData, markingTargetDataFromWirebond);

				//List<int> needMarkingAddressList = totalMarkingTargetData.Where(t => t.Value == MappingFile.Data.NG_VAL).Select(t => t.Key).ToList();
				List<int> needMarkingAddressList = markingTargetDataFromWirebond.Where(t => t.Value == MappingFile.Data.NG_VAL).Select(t => t.Key).ToList();

				// マーキング指示したアドレスがマーキング結果に存在するかチェック
				foreach (int needMarkingAddress in needMarkingAddressList)
				{
					if (markingAddressList.Contains(needMarkingAddress) == false)
					{
						DifferentMarkingAddressList.Add(needMarkingAddress);
					}
				}

				// マーキング結果のアドレスがマーキング指示に存在するかチェック
				//foreach (int markingAddress in markingAddressList)
				//{
				//	if (needMarkingAddressList.Contains(markingAddress) == false)
				//	{
				//		DifferentMarkingAddressList.Add(markingAddress);
				//	}
				//}

				if (DifferentMarkingAddressList.Count > 0)
				{
					string strDifferentAddr = "ｱﾄﾞﾚｽ:指示内容\r\n";

					foreach (int addr in DifferentMarkingAddressList)
					{
						int lastRow = (int)(strDifferentAddr.Count() / Log.CellStrLenPerRow);
						strDifferentAddr += string.Format("{0}:{1}, ", addr.ToString(), markingTargetDataFromWirebond.Where(m => m.Key == addr).Select(m => m.Value).Single());

						if ((int)(strDifferentAddr.Count() / Log.CellStrLenPerRow) > lastRow)
						{
							strDifferentAddr += "\r\n";
						}
					}

					this.MarkingVerifyErrMsg = string.Format("【ﾏｰｷﾝｸﾞ不整合】ﾏｰｷﾝｸﾞ指示箇所(WBのNG確定箇所, AI検査NG箇所)とﾏｰｷﾝｸﾞ箇所(MMファイル)不一致\r\n{0}", strDifferentAddr);
					
					return false;
				}

				return true;
			}

			/// <summary>
			/// 検査後MD用マッピングデータ取得
			/// </summary>
			/// <returns></returns>
			public Dictionary<int, string> GetMappingDataForMD(Dictionary<int, string> mmContents, Magazine.Config magConfig, bool isMappingInspection)
			{
				//2014/10/2 戻り値で
				// 内容をMDマッピング用データに変換
				Dictionary<int, string> inspectionMapping = getInspectionMappingDataConvertedToMold(mmContents, magConfig.TotalMagazinePackage);

				Dictionary<int, string> moldMapping = new Dictionary<int,string>();
				// WBマッピングデータを読み込む
				if (isMappingInspection)
				{
					Dictionary<int, string> wirebondMapping = getWirebondMappingDataConvertedToMold(this.LotNo);
					moldMapping = MappingFile.Data.Union(wirebondMapping, inspectionMapping);
				}
				else
				{
					moldMapping = inspectionMapping;
				}
				
				// 手入力不良データを読み込む
				moldMapping = MappingFile.Data.GetAddedManualInputDefect(this.LotNo, magConfig, moldMapping, Config.Settings.InspectProcNo, true);

				return moldMapping;
			}

			private Dictionary<int, string> convertToMold(Dictionary<int, string> mappingData)
			{
				Dictionary<int, string> retv = new Dictionary<int, string>();

				Dictionary<string, string> convMaster = MapConv.GetData("AI_MM", "AI_OUT");
				foreach(KeyValuePair<int, string> mapping in mappingData)
				{
                    if (!convMaster.ContainsKey(mapping.Value))
                    {
                        retv.Add(mapping.Key, "1");
                        //throw new ApplicationException(
                        //    string.Format("MMファイルからMD受け渡し用マッピングデータに変換中、対応していないMMファイルの処理CDがありました。ファイルパス:{0} 処理CD:{1}", this.Info.FullName, mapping.Key));
                    }
                    else
                    {
                        retv.Add(mapping.Key, convMaster[mapping.Value]);
                    }
				}

				return retv;
			}

			private Dictionary<int, string> convertToInspectorMarking(Dictionary<int, string> mappingData, int totalMagazinePackageCt)
			{
				Dictionary<int, string> retv = new Dictionary<int, string>();

				Dictionary<string, string> convMaster = MapConv.GetData("AI_MM", "AI_OUT");
				foreach (KeyValuePair<int, string> mapping in mappingData)
				{
					if (!convMaster.ContainsKey(mapping.Value))
					{
						retv.Add(mapping.Key, MappingFile.Data.NG_VAL);
					}
					else
					{
						retv.Add(mapping.Key, convMaster[mapping.Value]);
					}
				}

				for (int i = 1; i <= totalMagazinePackageCt; i++)
				{
					if (!retv.ContainsKey(i))
					{
						retv.Add(i, MappingFile.Data.OK_VAL);
					}
				}



				return retv;
			}

			private Dictionary<int, string> getInspectionMappingDataConvertedToMold(Dictionary<int, string> mmContents, int totalMagazinePackage) 
			{
				Dictionary<int, string> retv = MappingFile.Data.GetAnySize(totalMagazinePackage, "0");

				Dictionary<int, string> convData = convertToMold(mmContents);
				foreach(KeyValuePair<int, string> conv in convData)
				{
					if (conv.Key == 0)
					{
						continue;
					}

					if (conv.Value == MappingFile.Data.OK_VAL)
					{
						continue;
					}
					retv[conv.Key] = conv.Value;
				}

				return retv;
			}

			private Dictionary<int, string> getWirebondMappingDataConvertedToMold(string lotNo) 
			{
				bool hasNeedReverseFramePlacement = LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(LotNo
					, Config.Settings.WirebondProcNo, Config.Settings.InspectProcNo, true);

                //MMファイルのクラスでMachineBaseを継承してないので、下記のログ出力関数を使えないので次回、この周辺を改修する際に修正
                //HideLog(string.Format("【】WBマッピングデータの上下反転有無：{0} / ロットNo:{1} WB-Proc(Config):{2} AI-Proc(Config):{3}", hasNeedReverseFramePlacement, lot.LotNo, Config.Settings.WirebondProcNo, Config.Settings.InspectProcNo));


				Dictionary<int, string> wireMappingData = MappingFile.Data.Wirebonder.Get(lotNo, hasNeedReverseFramePlacement);
				
				wireMappingData = MappingFile.Data.Wirebonder.ConvertToMold(wireMappingData);

				return wireMappingData;
			}

			private void createMappingFile(string lotNo, Dictionary<int, string> mappingData)
			{
				string filePath = Path.Combine(Config.Settings.ForMDMappingFileDirPath, lotNo + ".mpd");

				MappingFile.Create(filePath, mappingData);
			}

			private void keepMachineLog(string filePath, string lotNo)
			{
				MachineLog.CopyKeepRecord(filePath, Config.Settings.ForAICompareMachineLogDirPath, lotNo);
				this.WorkMachine.InfoLog("照合用MMファイル複製完了", lotNo);
			}

            /// <summary>
            /// MMファイル1行分のデータ
            /// </summary>
            private class MMFileRowData
            {
                /// <summary>ﾛｷﾞﾝｸﾞｱﾄﾞﾚｽ（1～100000）</summary>
                public int Address { get; set; }

                /// <summary>生ﾛｷﾞﾝｸﾞﾏｯﾋﾟﾝｸﾞﾃﾞｰﾀ</summary>
                public string Result { get; set; }

                /// <summary>高倍/低倍検査中</summary>
                public int CameraNo { get; set; }
            }
		}

		/// <summary>
		/// 1フレームの検査設定
		/// </summary>
		public class FrameInspectionSetting
		{
			public int AreaNo { get; set; }

			/// <summary>エリア開始位置アドレス</summary>
			public string AreaStartAddress { get; set; }

			/// <summary>エリア完了位置アドレス</summary>
			public string AreaEndAddress { get; set; }
		}
		
		//public void Save()
		//{
		//	this.frameInspectionSetting = new List<FrameInspectionSetting>();
		//	FrameInspectionSetting test = new FrameInspectionSetting();
		//	test.AreaNo = 1;
		//	test.AreaStartAddress = "E00000";
		//	test.AreaEndAddress = "E99999";
		//	frameInspectionSetting.Add(test);

		//	FrameInspectionSetting test2 = new FrameInspectionSetting();
		//	test2.AreaNo = 2;
		//	test2.AreaStartAddress = "E00000";
		//	test2.AreaEndAddress = "E99999";
		//	frameInspectionSetting.Add(test);

		//	string raw = JsonConvert.SerializeObject(this, Formatting.Indented);
		//	try
		//	{
		//		File.WriteAllText(Path.Combine(Properties.Settings.Default.ConfigFilePath, "Inspector.xml"), raw);
		//	}
		//	catch (Exception err)
		//	{
		//		//Log.SysLog.Error("Config保存失敗 " + ex.ToString());
		//		//throw new ApplicationException("Config保存失敗");
		//	}
		//}
    }
}
