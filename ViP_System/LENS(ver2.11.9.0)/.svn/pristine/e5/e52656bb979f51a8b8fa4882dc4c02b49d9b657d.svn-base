using LENS2.Machine;
using LENS2_Api;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2
{
	class MappingFile
	{
		private static void create(string filepath, Dictionary<int, string> mappingdata, int retryct)
		{


			IOrderedEnumerable<KeyValuePair<int, string>> sortMappingData = mappingdata.OrderBy(m => m.Key);
			string fileValue = string.Join(",", sortMappingData.Select(m => m.Value));

			try
			{
				File.WriteAllText(filepath, fileValue, Encoding.Default);
			}
			catch (IOException)
			{
				if (retryct == Config.Settings.FileAccessRetryCount)
				{
					throw new ApplicationException(string.Format("マッピングファイルの作成に失敗しました。作成ファイル:{0}", filepath));
				}

				create(filepath, mappingdata, retryct++);
			}
		}
		public static void Create(string filepath, Dictionary<int, string> mappingdata)
		{
			create(filepath, mappingdata, 0);
		}

		public static string GetPath(string dirPath, string lotNo, string fileExt)
		{
			List<string> filePathList = LENS2_Api.Directory.GetFiles(dirPath, lotNo, fileExt);

			if (filePathList == null || filePathList.Count == 0)
			{
				return null;
			}
			else if (filePathList.Count == 1)
			{
				return filePathList[0];
			}
			else
			{
				throw new ApplicationException(string.Format("マッピングファイルが複数見つかりました。フォルダ:{0} ロットNo：{1}　拡張子：{2}", dirPath, lotNo, fileExt));
			}
		}

		public static void MoveProcessedFiles()
		{
			string[] wbFilePathArray = System.IO.Directory.GetFiles(Config.Settings.ForAIMappingFileDirPath);
			string[] aiFilePathArray = System.IO.Directory.GetFiles(Config.Settings.ForMDMappingFileDirPath);

			foreach (string wbFilePath in wbFilePathArray)
			{
				FileInfo wbFile = new FileInfo(wbFilePath);

				if (wbFile.CreationTime < DateTime.Now.AddDays(-Config.Settings.MoveMappingFileIntervalDay))
				{
					string wbDateDirName = wbFile.CreationTime.ToString("yyyyMM");
					string wbMoveToDir = Path.Combine(wbFile.DirectoryName, wbDateDirName);

					if (!System.IO.Directory.Exists(wbMoveToDir))
					{
						System.IO.Directory.CreateDirectory(wbMoveToDir);
					}

					try
					{
						wbFile.MoveTo(Path.Combine(wbMoveToDir, wbFile.Name));
					}
					catch (IOException) { }
				}
			}

			foreach (string aiFilePath in aiFilePathArray)
			{
				FileInfo aiFile = new FileInfo(aiFilePath);

				if (aiFile.CreationTime < DateTime.Now.AddDays(-Config.Settings.MoveMappingFileIntervalDay))
				{
					string aiDateDirName = aiFile.CreationTime.ToString("yyyyMM");
					string aiMoveToDir = Path.Combine(aiFile.DirectoryName, aiDateDirName);

					if (!System.IO.Directory.Exists(aiMoveToDir))
					{
						System.IO.Directory.CreateDirectory(aiMoveToDir);
					}

					try
					{
						aiFile.MoveTo(Path.Combine(aiMoveToDir, aiFile.Name));
					}
					catch (IOException) { }
				}
			}
		}

		public static void MoveToBackupDir(string lotNo)
		{		
			//wbmファイルの年月フォルダへの移動
            string wbFilePath = MappingFile.Data.Wirebonder.GetPath(lotNo);
            if (File.Exists(wbFilePath))
            {
                FileInfo wbFile = new FileInfo(wbFilePath);

                string wbDateDirName = wbFile.CreationTime.ToString("yyyyMM");
                string wbMoveToDir = Path.Combine(wbFile.DirectoryName, wbDateDirName);

                if (!System.IO.Directory.Exists(wbMoveToDir))
                {
                    System.IO.Directory.CreateDirectory(wbMoveToDir);
                }

                try
                {
                    wbFile.MoveTo(Path.Combine(wbMoveToDir, wbFile.Name));
                }
                catch (IOException) { }
            }

			//mpdファイルの年月フォルダへの移動
            string aiFilePath = MappingFile.Data.Inspector.GetPath(lotNo);
            if (File.Exists(aiFilePath))
            {
                FileInfo aiFile = new FileInfo(aiFilePath);

                string aiDateDirName = aiFile.CreationTime.ToString("yyyyMM");
                string aiMoveToDir = Path.Combine(aiFile.DirectoryName, aiDateDirName);

                if (!System.IO.Directory.Exists(aiMoveToDir))
                {
                    System.IO.Directory.CreateDirectory(aiMoveToDir);
                }

                try
                {
                    aiFile.MoveTo(Path.Combine(aiMoveToDir, aiFile.Name));
                }
                catch (IOException) { }
            }
		}

		public class Data
		{
			public const string OK_VAL = "0";
			public const string NG_VAL = "1";

			/// <summary>パッケージアドレス</summary>
			public int Address { get; set; }

			/// <summary>マッピング値</summary>
			public string Value { get; set; }

			/// <summary>
			/// 指定したマッピング値で埋めた任意のパッケージ数(サイズ)のマッピングデータを取得
			/// </summary>
			/// <param name="needPackageQty">取得したいマッピングデータのパッケージ数(サイズ)</param>
			/// <param name="mappingValue">マッピング値</param>
			/// <returns></returns>
			public static Dictionary<int, string> GetAnySize(int needPackageQty, string mappingValue)
			{
				Dictionary<int, string> mappingData = new Dictionary<int, string>();

				for (int i = 0; i < needPackageQty; i++)
				{
					mappingData.Add(i+1, mappingValue);
				}

				return mappingData;
			}

			private static Dictionary<int, string> Get(string filepath)
			{
				if (!File.Exists(filepath))
				{
					throw new ApplicationException(string.Format("指定されたマッピングファイルが見つかりません。 FilePath:{0}", filepath));
				}

				Dictionary<int, string> retv = new Dictionary<int, string>();

				string[] elements = File.ReadAllText(filepath, Encoding.Default).Split(',');

				int addressNo = 1;
				foreach (string element in elements)
				{
					if (string.IsNullOrWhiteSpace(element)) { continue; }
					retv.Add(addressNo, element);
					addressNo++;
				}

				return retv;
			}

			/// <summary>
			/// マッピングデータを結合する
			/// </summary>
			/// <param name="mappingList1">マッピングデータ1</param>
			/// <param name="mappingList2">マッピングデータ2</param>
			/// <returns>結合後マッピングデータ</returns>
			public static List<string> Union(List<string> mappingList1, List<string> mappingList2)
			{
				if (mappingList1.Count != 0 && mappingList2.Count == 0)
				{
					return mappingList1;
				}
				else if (mappingList1.Count == 0 && mappingList2.Count != 0)
				{
					return mappingList2;
				}

				if (mappingList1.Count != mappingList2.Count)
				{
					throw new ApplicationException("結合するマッピングデータの数が一致しません");
				}

				for (int i = 0; i < mappingList1.Count; i++)
				{
					if (mappingList2[i] == "0") { continue; }

					mappingList1[i] = mappingList2[i];
				}

				return mappingList1;
			}

			public static Dictionary<int, string> Union(Dictionary<int, string> mappingList1, Dictionary<int, string> mappingList2) 
			{
				if (mappingList1.Count != 0 && mappingList2.Count == 0)
				{
					return mappingList1;
				}
				else if (mappingList1.Count == 0 && mappingList2.Count != 0)
				{
					return mappingList2;
				}

				if (mappingList1.Count != mappingList2.Count)
				{
					throw new ApplicationException("結合するマッピングデータの数が一致しません");
				}

				//for (int i = 0; i < mappingList1.Count; i++)
				//{
				//	if (mappingList2[i] == "0") { continue; }

				//	mappingList1[i] = mappingList2[i];
				//}

				foreach (KeyValuePair<int, string> mappingData in mappingList2)
				{
					if (mappingData.Value == "0") { continue; }

					mappingList1[mappingData.Key] = mappingData.Value;
				}


				return mappingList1;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="mappingData"></param>
			/// <returns>マガジンの上下反転したマッピングデータを返す</returns>
			public static Dictionary<int, string> GetUpsideDownMagazineSequential(Dictionary<int, string> mappingData, int packageQtyX, int packageQtyY, int magazineStepCT, Magazine.Config.LoadStep loadStepCD)
			{
				Dictionary<int, string> reversedData = new Dictionary<int, string>();

				return reversedData;

			}

			/// <summary>
			/// マッピングファイルから不良アドレスに限定したリストを取得する
			/// </summary>
			/// <param name="filePath">マッピングファイルのファイルパス</param>
			/// <param name="hasPassedAI">検査機を通過したかどうか</param>
			/// <param name="isMappingInspection">WBマッピング検査対象かどうか</param>
			/// <returns></returns>
			public static Dictionary<int, string> GetDefectAddressList(Dictionary<int, string> mappingDataList, Lot lot) // , bool hasNeedReverseFramePlacement, Magazine.Config magConfig)
			{
				Dictionary<int, string> defectAddressList = new Dictionary<int, string>();

                // この関数の外Mold.csからこの関数を呼び出す前に、wbm or mpdを読み込んでくるとき、個別に逆転しているのにこの中で元に戻してしまっているため
                // ここの反転はコメントアウト
                //if (hasNeedReverseFramePlacement)
                //{
                //    //マッピングデータを逆転する
                //    mappingDataList = GetReverseData(mappingDataList, magConfig);
                //}

				//検査機通過してない かつWBマッピング対象でも無い場合
				if (lot.HasPassedInspector == false && lot.IsMappingInspection == false)
				{
					MachineBase.CommonHideLog(string.Format("ﾛｯﾄNo:{0} 検査機通過Fg:{1} ﾏｯﾋﾟﾝｸﾞFg:{2}", lot.LotNo, lot.HasPassedInspector ? "true" : "false", lot.IsMappingInspection));
					return new Dictionary<int, string>();
				}

				//Valueが1の項目に絞ったアドレスリストを返す
				if (mappingDataList != null && mappingDataList.Count != 0)
				{
					defectAddressList = mappingDataList.Where(m => m.Value == NG_VAL).ToDictionary(m => m.Key, m => m.Value);	
				}

				MachineBase.CommonHideLog(string.Format("ﾛｯﾄNo:{0} 不良数:{1}", lot.LotNo, defectAddressList.Count));

				return defectAddressList;
			}


			/// <summary>
			/// マガジン内でフレーム反転したデータを取得
			/// </summary>
			/// <param name="mappingDataList">マッピングデータ</param>
			/// <param name="frameInfo">フレーム情報</param>
			/// <returns>マッピングデータ</returns>
			public static Dictionary<int, string> GetReverseData(Dictionary<int, string> mappingDataList, Magazine.Config magConfig)
			{
                mappingDataList = mappingDataList.OrderBy(m => m.Key).ToDictionary(m => m.Key, m => m.Value);
				//フレーム単位のリストを作成する
				List<string> frameList = new List<string>();
				for (int i = 0; i < magConfig.StepNum; i++)
				{
					string packageData = string.Join(",",
									  mappingDataList
									  .Skip(i * magConfig.TotalFramePackage)
									  .Take(magConfig.TotalFramePackage)
									  .Select(m => m.Value)
									  .ToArray()
									  );

					frameList.Add(packageData);
				}

				//反転準備
				switch (magConfig.LoadStepCD)
				{
					case Magazine.Config.LoadStep.Even:
						//(偶数段)
						frameList.Add(frameList[0]);
						frameList.RemoveAt(0);
						break;
					case Magazine.Config.LoadStep.Even_NaturalRev:
						break;
					case Magazine.Config.LoadStep.Odd:
						//(奇数段)
						frameList.Insert(0, frameList.Last());
						frameList.RemoveAt(frameList.Count - 1);
						break;
					case Magazine.Config.LoadStep.Odd_NaturalRev:
						break;
					case Magazine.Config.LoadStep.All:
						break;
					default:
						throw new ApplicationException(string.Format("マガジンへのフレーム搭載段のマスタ設定が正しくありません。"));
				}

				//フレームリストを反転
				frameList.Reverse();

				//アドレスNOを再割当したマッピングリストを作成する
				Dictionary<int, string> rMappingDataList = new Dictionary<int, string>();
				foreach (string frame in frameList)
				{
					List<string> packageList = frame.Split(',').ToList();
					for (int i = 0; i < packageList.Count; i++)
					{
						rMappingDataList.Add(rMappingDataList.Count + 1, packageList[i]);
					}
				}

				return rMappingDataList;
			}

			public static Dictionary<int, string> ConvertToAddressData(List<string> mappingdata)
			{
				Dictionary<int, string> retv = new Dictionary<int, string>();

				for (int i = 0; i < mappingdata.Count; i++)
				{
					retv.Add(i + 1, mappingdata[i]);
				}

				return retv;
			}

			public static Dictionary<int, string> GetAddedManualInputDefect(string lotNo, Magazine.Config magConfig, Dictionary<int, string> mappingData, int fromProcNo, bool isEndTimingRef)
			{
				Dictionary<int, string> toReverseMappingData = MappingFile.Data.GetAnySize(mappingData.Count, MappingFile.Data.OK_VAL);
				bool haveNeedReverseData = false;

				List<MacDefect> defects = MacDefect.GetData(lotNo);
				foreach (MacDefect d in defects)
				{
					if (!mappingData.ContainsKey(d.DefAddressNo))
					{
						throw new ApplicationException(
							string.Format("手動登録の不良箇所を読み込み中、存在しないアドレスへの参照がありました。ロット:{0} アドレス:{1}", lotNo, d.DefAddressNo));
					}

					if (LENS2_Api.ARMS.WorkResult.HasNeedReverseFramePlacement(lotNo, d.ProcNo, fromProcNo, isEndTimingRef))
					{
						toReverseMappingData[d.DefAddressNo] = MappingFile.Data.NG_VAL;
						haveNeedReverseData = true;
					}
					else
					{
						mappingData[d.DefAddressNo] = MappingFile.Data.NG_VAL;
					}
				}

				Dictionary<int, string> reversedMappingData = new Dictionary<int, string>();
				if (haveNeedReverseData)
				{
					reversedMappingData = MappingFile.Data.GetReverseData(toReverseMappingData, magConfig);
					mappingData = MappingFile.Data.Union(mappingData, reversedMappingData);
				}

				return mappingData;
			}

			public class Wirebonder
			{
				public const string AROUNDINSPECTION_POINT = "S";
				public const string NONINSPECTION_POINT = "M";
				public const string INSPECTION_POINT = "1";

				public static string GetPath(string lotNo)
				{
					string filePath = Path.Combine(Config.Settings.ForAIMappingFileDirPath, string.Format("{0}.{1}", lotNo, "wbm"));
					return filePath;
				}

				public static Dictionary<int, string> Get(string lotNo, bool hasNeedReverseFramePlacement)
				{
					string filePath = GetPath(lotNo);

					Dictionary<int, string> mappingData = Data.Get(filePath);

					Lot lot = Lot.GetData(lotNo);

					Magazine.Config magConfig = Magazine.Config.GetData(lot.TypeCd);

					if (hasNeedReverseFramePlacement)
					{
						mappingData = MappingFile.Data.GetReverseData(mappingData, magConfig);
					}

					return mappingData;
				}

				public static Dictionary<int, string> ConvertToInspection(Dictionary<int, string> original)
				{
					Dictionary<int, string> retv = new Dictionary<int, string>();

					Dictionary<string, string> conv = MapConv.GetData("WB_OUT", "AI_IN");

					foreach (KeyValuePair<int, string> data in original)
					{
						retv.Add(data.Key, conv[data.Value]);
					}

					return retv;
				}
				
				public static Dictionary<int, string> ConvertToMold(Dictionary<int, string> original)
				{
					Dictionary<int, string> retv = new Dictionary<int, string>();

					Dictionary<string, string> conv = MapConv.GetData("WB_OUT", "MD_IN");

					foreach (KeyValuePair<int, string> data in original)
					{
						retv.Add(data.Key, conv[data.Value]);
					}

					return retv;
				}

				public static Dictionary<int, string> ConvertToMoldFromMachineFile(Dictionary<int, string> original)
				{
					Dictionary<int, string> retv = new Dictionary<int, string>();

					Dictionary<string, string> conv = MapConv.GetData("WB_MM", "MD_IN");

					foreach (KeyValuePair<int, string> data in original)
					{
						retv.Add(data.Key, conv[data.Value]);
					}

					return retv;
				}

			}

			public class Inspector
			{

				public static string GetPath(string lotNo)
				{
					string filePath = Path.Combine(Config.Settings.ForMDMappingFileDirPath, string.Format("{0}.{1}", lotNo, "mpd"));
					return filePath;
				}

				public static Dictionary<int, string> Get(string lotNo, bool hasNeedReverseFramePlacement)
				{
					string filePath = GetPath(lotNo);

					Dictionary<int, string> mappingData = Data.Get(filePath);

					Lot lot = Lot.GetData(lotNo);

					Magazine.Config magConfig = Magazine.Config.GetData(lot.TypeCd);

					if (hasNeedReverseFramePlacement)
					{
						mappingData = MappingFile.Data.GetReverseData(mappingData, magConfig);
					}

                    return mappingData;

                    // 常に反転せずにデータを返してしまっていた。
                    //return Data.Get(filePath);
				}

				public static Dictionary<int, string> ConvertToMold(Dictionary<int, string> original)
				{
					Dictionary<int, string> retv = new Dictionary<int, string>();

					Dictionary<string, string> conv = MapConv.GetData("AI_OUT", "MD_IN");

					foreach (KeyValuePair<int, string> data in original)
					{
						retv.Add(data.Key, conv[data.Value]);
					}

					return retv;
				}

				public static Dictionary<int, string> ConvertToMarkingMapData(Dictionary<int, string> mappingData, string[] deselectMarkingTargetCd)
				{
					Dictionary<string, string> deselectConv = new Dictionary<string, string>();

					foreach (string targetCd in deselectMarkingTargetCd)
					{
						deselectConv.Add(targetCd, MappingFile.Data.OK_VAL);
					}

					Dictionary<int, string> retv = new Dictionary<int, string>();

					foreach (KeyValuePair<int, string> data in mappingData)
					{
						if (deselectConv.ContainsKey(data.Value))
						{
							retv.Add(data.Key, deselectConv[data.Value]);
						}
						else if(data.Value == MappingFile.Data.OK_VAL)
						{
							retv.Add(data.Key, MappingFile.Data.OK_VAL);
						}
						else
						{
							retv.Add(data.Key, MappingFile.Data.NG_VAL);
						}
					}

					return retv;
				}
			}
		}

		//public class Data 
		//{
		//	public class Convert
		//	{
		//		public static Dictionary<int, string> ToInspectionFromWirebonder(Dictionary<int, string> wiremappingdata)
		//		{
		//			//検査(1)、周辺強度(2)、目視検査(3)、検査無し(4)、部材交換面積(M)は処理無し(0)に変換
		//			//周辺検査(S)は検査(1)に変換
		//			//List<string> convertdata = wiremappingdata.Select(m =>
		//			//	m.Value.Replace(((int)Machine.Wirebonder.MMFile.ResultCode.Inspection).ToString(), "0")
		//			//	.Replace(((int)Machine.Wirebonder.MMFile.ResultCode.Strength).ToString(), "0")
		//			//	.Replace(((int)Machine.Wirebonder.MMFile.ResultCode.Seeing).ToString(), "0")
		//			//	.Replace(((int)Machine.Wirebonder.MMFile.ResultCode.NotInspection).ToString(), "0")
		//			//	.Replace(Machine.Wirebonder.MMFile.MAPPING_PARTSCHANGE_KB, "0")
		//			//	.Replace(Machine.Wirebonder.MMFile.MAPPING_SINSP_KB, "1")
		//			//	).ToList();

		//			Dictionary<int, string> retv = new Dictionary<int, string>();

		//			return wiremappingdata;
		//		}

		//		public static Dictionary<int, string> ToMoldFromWirebonder(Dictionary<int, string> wiremappingdata)

		//		/// <summary>
		//		/// マッピング値変換マスタから理由指定で取得した変換表を基に変換したマッピングデータを返す
		//		/// </summary>
		//		/// <param name="reasonCD"></param>
		//		/// <returns></returns>
		//		public static Dictionary<int, string> ToMappingValue(int reasonCD, Dictionary<int, string> mappingData)
		//		{
		//			Dictionary<int, string> convertedData = new Dictionary<int, string>();

		//			return convertedData;
		//		}
		//	}
		//}

		//public enum ConvertReason
		//{

		//}

		//private static string GetConvertedSequentialMappingData(Dictionary<int, string> targetMappingData)
		//{
		//	return "";
		//}

		//public static string GetFullInspectionMappingData(int totalPackageQtyPerFrame, int magazineStepCT, Magazine.LoadStep loadStepCD)
		//{
		//	string mappingStrPerMagazine = string.Empty;
		//	string mappingStrPerFrame = string.Empty;

		//	for (int packageCt = 0; packageCt < totalPackageQtyPerFrame; packageCt++)
		//	{
		//		mappingStrPerFrame += "1";
		//	}

		//	for (int frameCt = 0; frameCt < magazineStepCT; frameCt++)
		//	{
		//		mappingStrPerMagazine += mappingStrPerFrame;
		//	}

		//	return mappingStrPerMagazine;
		//	//string oddStepMappingStr;
		//	//string evenStepMappingStr;

		//	//switch (loadStepCD)
		//	//{
		//	//	case Magazine.LoadStep.All:
					
		//	//		break;
		//	//	case Magazine.LoadStep.Even:
		//	//		break;
		//	//	case Magazine.LoadStep.Odd:
		//	//		break;
		//	//	default:
		//	//		throw new ApplicationException(string.Format("未定義のフレーム搭載種別(loadStepCD):{0}", loadStepCD.ToString()));
		//	//		break;
		//	//}
		//}

		///// <summary>
		///// アドレス順に並べたマッピングデータを返す
		///// </summary>
		///// <param name="mappingData"></param>
		///// <returns>アドレス順に改行無しカンマ区切りで並べたマッピングデータ</returns>
		//public static string GetSequentialMappingData(Dictionary<int, string> mappingData)
		//{
		//	string sequentialMappingData = string.Empty;
		//	return sequentialMappingData;
		//}

	}
}
