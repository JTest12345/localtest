using EICS.Database;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine
{
	//スパッタ前移載機
	class EXCMachineInfo : MachineBase
	{
		//EICSの照合OK信号送信
		private const string SET_CHECK_OK = "B001212";

		private const string SET_CHECK_NG = "B001221";

		private const string GET_TRIGGER = "B001211";
		private const int MAGAZINENO_LEN = 10;
		private const string GET_MAGNO_ADDR = "W001240";

		PLC_Melsec machinePLC;

		private void connectPLC(string ipAddress, int portNO, List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList)
		{
			if (machinePLC == null)
			{
				machinePLC = new PLC_Melsec(ipAddress, portNO, exclusionList);
			}
			else
			{
				if (machinePLC.ConnectedPLC() == false)
				{
					machinePLC.ConnectPLC();
				}
			}
		}

		public override void CheckFile(LSETInfo lsetInfo)
		{
			SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

			List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList
				= commonSettingInfo.PlcExtractExclusionList.Where(p => p.ModelNm == lsetInfo.ModelNM).ToList();

            connectPLC(lsetInfo.IPAddressNO, lsetInfo.PortNO, exclusionList);

			//ﾄﾘｶﾞが有ったら下記のブロックを実行
			if (checkTrigger())
			{
                try
                {
                    string sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ﾄﾘｶﾞ確認", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                    //装置からﾏｶﾞｼﾞﾝNoを取得、ﾏｶﾞｼﾞﾝNoが全て装置から貰える
                    string magNo = getMagNo(GET_MAGNO_ADDR);

                    sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ﾏｶﾞｼﾞﾝNo取得:{2}", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, magNo);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                    List<string> lotNoList = getLotNoList(lsetInfo, magNo);

                    sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ﾛｯﾄNoﾘｽﾄ取得:{2}", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, string.Join(", ", lotNoList));
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);


                    //ﾛｯﾄ一覧からﾀｲﾌﾟ一覧を取得Arms.AsmLotにGetType()を作ったのでそれを呼ぶ
                    List<string> typeList = getTypeList(lsetInfo, lotNoList);

                    sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ﾀｲﾌﾟﾘｽﾄ取得:{2}", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, string.Join(", ", typeList));
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                    //ﾀｲﾌﾟ一覧を使ってEICSのTmPLMからﾌﾟﾛｸﾞﾗﾑ一覧を取得
                    List<Plm> spatterProgramList = SUPMachineInfo.GetSpatterProgramList(lsetInfo, typeList);

                    sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ｽﾊﾟｯﾀﾌﾟﾛｸﾞﾗﾑﾘｽﾄ取得:{2}", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, string.Join(", ", spatterProgramList.Select(s => s.ParameterVAL)));
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                    //ﾌﾟﾛｸﾞﾗﾑが単一となるかどうかをチェック、単一で無ければｴﾗｰで移載機の段階で止めてしまう。
                    if (SUPMachineInfo.IsUniqueProgram(spatterProgramList))
                    {
                        sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ｽﾊﾟｯﾀﾌﾟﾛｸﾞﾗﾑﾁｪｯｸOK", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO);
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                        //ﾌﾟﾛｸﾞﾗﾑが単一なので、EICS・OK信号を返す
                        sendCheckOK();
                    }
                    else
                    {
                        sendCheckNG();
                        throw new ApplicationException(string.Format(
                            "設備号機]={0} /[設備CD]={1} ﾌﾟﾛｸﾞﾗﾑ不一致(qcParamNo:{2})の為、装置停止信号送信"
                            , lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, SettingInfo.GetSingleton().SUP_ProgramParamNo));
                    }
                }
                catch (ApplicationException ex)
                {
                    sendCheckNG();
                    throw ex;
                }
			}
		}

		private string getMagNo(string getMagNoPLCAddr)
		{
            //■マガジンNo取得"30 M000001"→"M000001"
            string sMagazineNo = machinePLC.GetMagazineNo(getMagNoPLCAddr, MAGAZINENO_LEN);
            string[] sMagazineNoItem = sMagazineNo.Split(' ');
            if (sMagazineNoItem.Count() < 2)
            {
                throw new ApplicationException(string.Format("装置から取得したﾄﾚｲNoのﾌｫｰﾏｯﾄが正しくありません。ﾏｶﾞｼﾞﾝNo:{0}", sMagazineNoItem));
            }
            return sMagazineNoItem[1];

		}

		//private string getMagazineNoList(string getMagNoPLCAddr)
		//{
		//	string magNo = string.Empty;

		//	foreach (string getMagNoPLCAddr in getMagNoPLCAddrArray)
		//	{
		//		magNoList.Add(getMagNo(getMagNoPLCAddr));
		//	}

		//	return magNoList;
		//}

        private List<string> getLotNoList(LSETInfo lsetInfo, string magNo)
		{
			List<string> lotNoList = new List<string>();

            string refServerNm = string.Join(",", SettingInfo.GetReferServerNm(lsetInfo));

            Arms.Magazine[] magList = Arms.Magazine.GetCurrentFromMultipleServer(lsetInfo.InlineCD, magNo, lsetInfo.ReferMultiServerFG);
            if (magList == null )
            {
                throw new ApplicationException(string.Format("ﾏｶﾞｼﾞﾝNoに紐つくﾛｯﾄNoが取得できませんでした。ﾏｶﾞｼﾞﾝNo:{0}, 参照したDBｻｰﾊﾞ:{1}", magNo, refServerNm));

            }
            else if (magList.Length != 1)
            {
                throw new ApplicationException(string.Format("ﾏｶﾞｼﾞﾝNoに紐つくﾛｯﾄNoが複数存在しています。ﾏｶﾞｼﾞﾝNo:{0}, 参照したDBｻｰﾊﾞ:{1}, ﾛｯﾄNo：{2}", magNo, refServerNm, string.Join(", ", magList.Select(l => l.NascaLotNO))));

            }
            string lotNo = magList[0].NascaLotNO;


            List<Arms.LotTray> lotTrayList = Arms.LotTray.GetCurrentFromMultipleServer(lsetInfo.InlineCD, lotNo, null, null, lsetInfo.ReferMultiServerFG);
			if (lotTrayList.Count != 1)
            {
				throw new ApplicationException(string.Format(
                    "ﾛｯﾄNoから紐つくｽﾊﾟｯﾀ用移載後ﾄﾚｲNoが1つに限定して取得できませんでした。ﾛｯﾄNo:{0}/ﾄﾚｲNo:{1}, 参照したDBｻｰﾊﾞ:{2}"
                    , lotNo, string.Join(", ", lotTrayList.Select(l => l.TrayNo)), refServerNm));
			
            }
            Arms.LotTray lotTray = lotTrayList.Single();

            // トレイNoの連番が照合対象ロットの連番未満のロットを取得
            for (int orderNo = 1; orderNo < lotTray.OrderNo; orderNo++)
            {
                List<Arms.LotTray> lotTrayAddList = Arms.LotTray.GetCurrentFromMultipleServer(lsetInfo.InlineCD, null, lotTray.TrayNo, orderNo, lsetInfo.ReferMultiServerFG);
                if (lotTrayAddList.Count == 1)
                {
                    lotTrayList.Add(lotTrayAddList.Single());
                }
                else if (lotTrayAddList.Count != 1)
                {
                    throw new ApplicationException(string.Format(
                        "投入中のｽﾊﾟｯﾀ用移載後ﾄﾚｲNoの連番 = 「{2}」の割当てﾛｯﾄが1つに限定して取得できませんでした。ﾄﾚｲNo:{0}, ﾛｯﾄNo：{1}, 参照したDBｻｰﾊﾞ:{2}"
                        , lotTray.TrayNo, string.Join(", ", lotTrayAddList.Select(l => l.LotNo)), orderNo, refServerNm));

                }
            }

			return lotTrayList.Select(l => l.LotNo).ToList();
		}

		public static List<string> getTypeList(LSETInfo lsetInfo, List<string> lotNoList)
		{
            List<string> typeList = new List<string>();

			foreach (string lotNo in lotNoList)
			{
                Arms.AsmLot lotInfo = Arms.AsmLot.GetAsmLotFromMultipleServer(lsetInfo.InlineCD, lotNo, lsetInfo.ReferMultiServerFG);

				if (lotInfo == null)
				{
                    string refServerNm = string.Join(",", SettingInfo.GetReferServerNm(lsetInfo));

					throw new ApplicationException(string.Format(
						"ﾛｯﾄNoからﾀｲﾌﾟを取得できませんでした。 ﾛｯﾄNo:{0} 参照したDBｻｰﾊﾞ:{1}", lotNo, refServerNm));
				}

				typeList.Add(lotInfo.TypeCd);
				
			}

            List<string> retV = new List<string>(typeList.Distinct());

            return retV;
		}

		private bool checkTrigger()
		{
			string trigBitStr = machinePLC.GetBit(GET_TRIGGER);

			if (trigBitStr == PLC.BIT_ON)
			{
				return true;
			}
			else if (trigBitStr == PLC.BIT_OFF)
			{
				return false;
			}
			else
			{
				throw new ApplicationException(string.Format("装置PLC不正応答 ﾄﾘｶﾞを参照しましたが0or1以外の応答がありました。取得内容:{0}", trigBitStr));
			}
		}

		private void sendCheckOK()
		{
			machinePLC.SetBit(SET_CHECK_OK, 1, "1");
		}

		private void sendCheckNG()
		{
            //machinePLC.SetBit(GET_TRIGGER, 1, "0");
			machinePLC.SetBit(SET_CHECK_NG, 1, "1");
		}
	}
}
