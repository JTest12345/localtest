using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EICS.Machine;
using System.Drawing;
using EICS.Database;

namespace EICS
{
    /// <summary>
    /// マッピングファイル情報
    /// </summary>
    public class MappingFile
    {
        /// <summary>アドレスNO</summary>
        public int AddressNO { get; set; }

        /// <summary>マッピングCD</summary>
        public string MappingCD { get; set; }

        public MappingFile(int addressNO, string mappingCD) 
        {
            this.AddressNO = addressNO;
            this.MappingCD = mappingCD;
        }

        /// <summary>
        /// 任意範囲のマッピングファイル内容を取得
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public static List<MappingFile> GetUnMappingData(int range)
        {
            List<MappingFile> mappingDataList = new List<MappingFile>();
            for (int i = 0; i < range; i++)
            {
                MappingFile mappingDataInfo = new MappingFile(i + 1, "0");
                mappingDataList.Add(mappingDataInfo);
            }
            return mappingDataList;
        }

        /// <summary>
        /// マッピングデータの反転対象か確認する
        /// </summary>
        /// <param name="modelNM">装置型式</param>
        /// <param name="typeCD">製品型番</param>
        /// <returns>反転対象フラグ</returns>
        public static bool IsReverse(int lineCD, string modelNM, int timingNO, string typeCD)
        {
            //反転対象の作業CDを取得
            List<MPGORDERInfo> mpgOrderInfoList = ConnectDB.GetMPGORDERData(lineCD, modelNM, timingNO);
            if (mpgOrderInfoList.Count == 0)
            {
                return false;
            }

            //対象作業CDが作業フローに含まれているか確認
			List<WorkFrowInfo> workFrowList = new List<WorkFrowInfo>();

			foreach (MPGORDERInfo mpgOrderInfo in mpgOrderInfoList)
			{
				 workFrowList.AddRange(ConnectDB.GetWorkFrowData(lineCD, typeCD, mpgOrderInfo.ProcNO));
			}

            if (workFrowList.Count % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// マッピングファイル内容からアドレス付き内容を取得
        /// </summary>
        /// <param name="mappingValueList">マッピングデータ</param>
        /// <returns>マッピングデータ</returns>
        public static List<MappingFile> GetAddressData(List<string> mappingValueList)
        {
            List<MappingFile> mappingDataList = new List<MappingFile>();

            for (int i = 0; i < mappingValueList.Count; i++)
            {
                MappingFile mappingDataInfo
                    = new MappingFile(i + 1, mappingValueList[i]);
                mappingDataList.Add(mappingDataInfo);
            }

            return mappingDataList;
        }

        /// <summary>
        /// アドレス無しのリストを取得
        /// </summary>
        /// <param name="mappingDataList">マッピングデータ</param>
        /// <returns>マッピングファイル内容</returns>
        public static List<string> GetUnAddressData(List<MappingFile> mappingDataList)
        {
            List<string> mappingList = new List<string>();

            foreach (MappingFile mappingData in mappingDataList)
            {
                mappingList.Add(mappingData.MappingCD);
            }

            return mappingList;
        }

        /// <summary>
        /// アドレス無しのリストを取得
        /// </summary>
        /// <param name="dirPath">マッピングファイルのディレクトリパス</param>
        /// <param name="fileNM">マッピングファイルのファイル名</param>
        /// <param name="extNM">拡張子名</param>
        /// <returns>マッピングファイル内容</returns>
        public static List<string> GetUnAddressData(string dirPath, string fileNM, string extNM) 
        {
            List<MappingFile> mappingDataList = GetData(dirPath, fileNM, extNM);

            List<string> mappingDataUnList = new List<string>();
            foreach (MappingFile mappingData in mappingDataList)
            {
                mappingDataUnList.Add(mappingData.MappingCD);
            }

            return mappingDataUnList;
        }

		public static List<string> GetFullSizeMappingData(int size, string data)
		{
			try
			{
				List<string> mappinglist = new List<string>();

				//フレーム情報からマッピングデータを生成
				for (int i = 0; i < size; i++)
				{
					mappinglist.Add(data);
				}

				return mappinglist;
			}
			catch (Exception err)
			{
				throw;
			}
		}

        /// <summary>マッピングファイル内容を取得</summary>
        /// <param name="dirPath">マッピングファイルのディレクトリパス</param>
        /// <param name="fileNM">マッピングファイルのファイル名</param>
        /// <param name="extNM">拡張子名</param>
        /// <returns>マッピングファイル内容</returns>
        public static List<MappingFile> GetData(string dirPath, string fileNM, string extNM)
        {
            string filePath = string.Format("{0}.{1}", Path.Combine(dirPath, fileNM), extNM);

            if (!File.Exists(filePath))
            {
                throw new Exception(string.Format(Constant.MessageInfo.Message_54, fileNM, filePath));
            }

            List<MappingFile> mappingFileInfoList = new List<MappingFile>();
            
            //マッピングファイルからマッピング情報を取得
            List<string> mappingDataList = File.ReadAllText(filePath, System.Text.Encoding.Default).Split(',').ToList();

            for (int i = 0; i < mappingDataList.Count; i++ )
            {
                if (mappingDataList[i] == "") { continue; }
                MappingFile mappingFileInfo = new MappingFile(i + 1, mappingDataList[i]);
                mappingFileInfoList.Add(mappingFileInfo);
            }

            return mappingFileInfoList;
        }

		public static List<MappingFile> GetCombineMappingFile(List<MappingFile> baseData, List<MappingFile> addData)
		{
			List<MappingFile> combineData = new List<MappingFile>();

			List<MappingFile> ngMapDatas = addData.Where(a => a.MappingCD != "0").ToList();

			foreach (MappingFile ngMapData in ngMapDatas)
			{
				int index = baseData.FindIndex(b => b.AddressNO == ngMapData.AddressNO);

				baseData[index].MappingCD = ngMapData.MappingCD;
			}

			return baseData;
		}

		public static bool IsCheckMappingData(List<MappingFile> checkTarget1List, List<MappingFile> checkTarget2List, ref List<ErrMessageInfo> errMessageList)
		{
			if(checkTarget1List.Count != checkTarget2List.Count)
			{
				throw new ApplicationException(string.Format("マッピングデータのサイズが異なります。マッピングデータサイズ:{0} and {1} ", checkTarget1List.Count, checkTarget2List.Count));
			}

			for (int index = 0; index < checkTarget1List.Count; index++)
			{
				if (checkTarget1List[index].AddressNO != checkTarget2List[index].AddressNO || checkTarget1List[index].MappingCD != checkTarget2List[index].MappingCD)
				{
					ErrMessageInfo errMessageInfo = new ErrMessageInfo(string.Format("マッピングデータの内容が異なります。 アドレス：{0}", checkTarget1List[index].AddressNO), Color.Red);
					errMessageList.Add(errMessageInfo);
				}
			}
			//foreach (MappingFile checkTarget1 in checkTarget1List)
			//{//forでインデックス指定により、それぞれのリストの要素同士を比較するよう変更
			//    if (checkTarget2List.Where(c => c.AddressNO == checkTarget1.AddressNO).Single().MappingCD != checkTarget1.MappingCD)
			//    {
			//        ErrMessageInfo errMessageInfo = new ErrMessageInfo(string.Format("マッピングデータの内容が異なります。 アドレス：{0}", checkTarget1.AddressNO), Color.Red);
			//        errMessageList.Add(errMessageInfo);
			//    }
			//}

			if (errMessageList.Count > 0)
			{
				return false;
			}

			return true;

		}

		public static List<string> GetInspectUnneededStepZeroMappingCDList(int startStepIndex, int magazineStepMaxCT, int framePkgGT, List<string> targetDataList)
		{
			//偶数設定されてる場合：1,3,5…
			//奇数設定されてる場合：2,4,6…最大段数までのフレームで繰り返し
			for (int stepIndex = startStepIndex; stepIndex < magazineStepMaxCT; stepIndex += 2)
			{
				for (int pkgIndexInFrame = 0; pkgIndexInFrame < framePkgGT; pkgIndexInFrame++)
				{
					int pkgAddr = framePkgGT * stepIndex + pkgIndexInFrame;

					targetDataList[pkgAddr] = "0";
				}
			}

			return targetDataList;
		}

		public static List<MappingFile> GetInspectUnneededStepZeroMappingCDList(int startStepIndex, int magazineStepMaxCT, int framePkgGT, List<MappingFile> targetDataList)
		{
			//偶数設定されてる場合：1,3,5…
			//奇数設定されてる場合：2,4,6…最大段数までのフレームで繰り返し
			for (int stepIndex = startStepIndex; stepIndex < magazineStepMaxCT; stepIndex += 2)
			{
				for (int pkgIndexInFrame = 1; pkgIndexInFrame <= framePkgGT; pkgIndexInFrame++)
				{
					int mappingAddr = framePkgGT * stepIndex + pkgIndexInFrame;

					int mappingFileIndex = targetDataList.FindIndex(t => t.AddressNO == mappingAddr);
					targetDataList[mappingFileIndex].MappingCD = "0";
				}
			}

			return targetDataList;
		}

		/// <summary>
		/// TnDefectに存在する異常箇所を1（MDにおける樹脂少箇所）としたマッピングファイルを取得
		/// </summary>
		/// <param name="baseDataList">TnDefectの異常情報を反映するベースになるマップデータ</param>
		/// <param name="lineCD"></param>
		/// <param name="equipmentNO"></param>
		/// <param name="sNascaLotNO"></param>
		/// <returns></returns>
		public static List<MappingFile> GetCompleteMappingFile(int dbServerLineCD, List<MappingFile> baseDataList, int lineCD, string equipmentNO, string sNascaLotNO)
		{
			List<Defect> defectList = Defect.GetDefectData(dbServerLineCD, lineCD, equipmentNO, sNascaLotNO, string.Empty, string.Empty, string.Empty);
				
			foreach (Defect defect in defectList)
			{
				int targetIndex = baseDataList.FindIndex(b => b.AddressNO == defect.DefAddressNO);

				baseDataList[targetIndex].MappingCD = "1";
			}

			return baseDataList;
		}

    }
}
