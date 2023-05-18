using EICS.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EICS.Machine
{
	class CFMachineInfo : MachineBase
	{
		/// <summary>カットフォーミング装置名</summary>
		public const string ASSETS_NM = "ｶｯﾄﾌｫｰﾐﾝｸﾞ機";

		/// <summary>Prefix_NM</summary>
		protected enum FileType
		{
			PLC
		}

		PLC_Keyence plc;//本番では三菱？という話なので、量試後に修正必要!!
		//PLC plcM;

		public CFMachineInfo()
		{
		}

		~CFMachineInfo()
		{
			if(plc != null)
				plc.Dispose();
		}

        public void ConnectPLC(string ipAddress, int portNO)
        {
            if (plc == null)
            {
                plc = new PLC_Keyence(ipAddress, portNO);
            }
            else
            {
                if (plc.ConnectedPLC() == false)
                {
                    plc.ConnectPLC();
                }
            }
        }

		/// <summary>
        /// ファイルチェック
        /// </summary>
        /// <param name="lsetInfo">装置情報</param>
        /// <returns>装置処理状態ステータス</returns>
		public override void CheckFile(LSETInfo lsetInfo)
		{
            ConnectPLC(lsetInfo.IPAddressNO, lsetInfo.PortNO);

			MagInfo magInfo;
			string sHexData;
            string sHexDataU;
            string sHexDataL;

			string parameterVAL;

			if (plc.GetBit(Constant.TRG_Res_CF_END) == "1")
			{
				magInfo = GetMagazineInfo(lsetInfo.InlineCD, lsetInfo.EquipmentNO, DateTime.Now.ToString());

				//マスタ情報(TmFILEFMT)を取得
				List<FILEFMTInfo> filefmtList = ConnectDB.GetFILEFMTData(Convert.ToString(FileType.PLC), lsetInfo, magInfo.sMaterialCD);
				if (filefmtList.Count == 0)
				{
					//設定されていない場合、装置処理停止
					string message = string.Format(Constant.MessageInfo.Message_114, magInfo.sMaterialCD, Convert.ToString(FileType.PLC));
					throw new Exception(message);
				}

				foreach (FILEFMTInfo filefmtInfo in filefmtList)
				{
					if (string.IsNullOrEmpty(filefmtInfo.MachinePrefixNM))
					{
						//設定されていない場合、装置処理停止
						string message = string.Format(Constant.MessageInfo.Message_115, magInfo.sMaterialCD, Convert.ToString(FileType.PLC), filefmtInfo.QCParamNO);
						throw new Exception(message);
					}
					
					sHexData = plc.GetData(filefmtInfo.MachinePrefixNM, 2, Constant.ssuffix_H, false).Replace(" ", "");

                    //上位4ビットと下位4ビットを入れ替え（エンディアンの都合）
                    sHexDataL = sHexData.Substring(4, 4);
                    sHexDataU = sHexData.Substring(0, 4);
                    sHexData = sHexDataL + sHexDataU;

					parameterVAL = (Convert.ToInt32(sHexData, 16)).ToString();
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("{0} {1}/[ショット数]：{2}", DateTime.Now, lsetInfo.EquipmentNO, parameterVAL));


					//閾値マスタ情報(TmPLM)取得
					//PLMInfo plmInfo = ConnectDB.GetPLMData(filefmtInfo.QCParamNO, lsetInfo.ModelNM, magInfo.sMaterialCD, this.LineCD);
					Plm plmInfo = Plm.GetData(this.LineCD, magInfo.sMaterialCD, lsetInfo.ModelNM, filefmtInfo.QCParamNO, false);

					if (plmInfo == null)
					{
						//設定されていない場合、装置処理停止
						string message = string.Format(Constant.MessageInfo.Message_28, magInfo.sMaterialCD, filefmtInfo.QCParamNO, filefmtInfo.ParameterNM);
						throw new Exception(message);
					}

					//履歴保存
                    Database.Log log = new Database.Log(this.LineCD);
                    log.GetInsertData(lsetInfo, magInfo, plmInfo, parameterVAL, string.Empty, System.DateTime.Now);
                    log.Insert();
				}
			}
		}

		//protected override void JudgeProcess(ReceiveMessageInfo receiveInfo)
		//{
		//	throw new Exception(string.Format(Constant.MessageInfo.Message_86, lsetInfo.AssetsNM));
		//}
	}
}
