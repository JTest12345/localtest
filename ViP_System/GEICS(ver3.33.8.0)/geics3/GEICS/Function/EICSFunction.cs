using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEICS.Function
{
	class EICSFunction
	{
		public static void UpdateDefect(List<DefectInfo> updateTargetList, string updUserCD)
		{
			using (ConnectQCIL conn = new ConnectQCIL(true, Constant.StrQCIL))
			{
				//不良情報を更新
				foreach (DefectInfo defectInfo in updateTargetList)
				{
					defectInfo.UpdUserCD = updUserCD;

					if (string.IsNullOrEmpty(defectInfo.UpdateDefAddressNO) == false)
					{
						CheckIntParsable(defectInfo.UpdateDefAddressNO);
						if(ExistMappingAddress(defectInfo.LotNO, int.Parse(defectInfo.UpdateDefAddressNO)) == false)
						{
							string typeCD = ConnectARMS.GetTypeCD(defectInfo.LotNO);
							FRAMEInfo frameInfo = ConnectQCIL.GetFRAMEData(typeCD);
							throw new ApplicationException(string.Format("存在しないアドレスです。\nタイプ:{0} 最大アドレス：{1} 入力アドレス：{2}", typeCD, frameInfo.MagazinPackageMAXCT, defectInfo.UpdateDefAddressNO));
						}
					}
					conn.UpdateDefectInfo(defectInfo);
				}

				////マッピングファイル作成
				//string mappingDir
				//    = string.Format(MAPPINGFILE_DEPOSITORY_AI, ((ServInfo)cmbServer.SelectedItem).ServerCD);
				//MachineFileInfo.CreateMappingFile(mappingDir, targetList[0].LotNO, mappingDataList);

				conn.Connection.Commit();
			}
		}

		public static bool ExistMappingAddress(FRAMEInfo frameInfo, int chkAddr)
		{
			return ExistMappingAddress(frameInfo.MagazinPackageMAXCT, chkAddr);
		}

		public static bool ExistMappingAddress(string lotNO, int chkAddr)
		{
			//フレーム情報を取得
			string typeCD = ConnectARMS.GetTypeCD(lotNO);
			FRAMEInfo frameInfo = ConnectQCIL.GetFRAMEData(typeCD);
			return ExistMappingAddress(frameInfo.MagazinPackageMAXCT, chkAddr);
		}

		public static bool ExistMappingAddress(int maxAddr, int chkAddr)
		{
			if (chkAddr > 0 && chkAddr <= maxAddr)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static void CheckIntParsable(string targetStr)
		{
			int testNum;
			if (int.TryParse(targetStr, out testNum) == false)
			{
				throw new ApplicationException(string.Format("数値に変換出来る文字列である必要があります。変換対象:{0}", targetStr));
			}
		}
	}
}
