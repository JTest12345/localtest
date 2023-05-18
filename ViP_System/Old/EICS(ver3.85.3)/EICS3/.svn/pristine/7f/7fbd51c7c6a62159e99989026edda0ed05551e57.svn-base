using EICS.Database;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine
{
    /// <summary>
    /// ASSETS_NM:ﾁｯﾌﾟﾀｲﾌﾟﾛｯﾄﾏｰｷﾝｸﾞ装置2
    /// </summary>
    class LMMachineInfoDAIRYU : MachineBase
    {
        const string TEACH_Send_LM_MarkingNO = "W2100";
        const string TRIG_ADDR = "MR1006";
        const string MAGAZINENO_ADDR = "W1000";
        const string SendMarkingNoModePlcAddress = "LR213";

        public IPlc plc;

		public LMMachineInfoDAIRYU(LSETInfo lsetInfo)
		{

		}

		public override void CheckFile(LSETInfo lsetInfo)
		{


			string logMsg = string.Empty;
			string markingStr = string.Empty;
			string lotMarkingStr = string.Empty;
			//string typeCD = string.Empty;
			string printOutTime1 = string.Empty;
			string printOutTime2 = string.Empty;
			List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList = new List<PLCDDGBasedMachine.ExtractExclusion>();
			RunningLog runLog = RunningLog.GetInstance();

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
			SettingInfo commonSettingInfo = SettingInfo.GetSingleton();
			string plcType = settingInfoPerLine.GetPLCType(lsetInfo.EquipmentNO);

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

			string trg = plc.GetBit(TRIG_ADDR);

			//印字切り替え要求
			if (plc.GetBit(SendMarkingNoModePlcAddress) == PLC.BIT_ON && int.Parse(trg) == 1)
			{
				string sMagazineNO = plc.GetString(MAGAZINENO_ADDR, 10, false, false);

				if (string.IsNullOrEmpty(sMagazineNO))
                {
                    string msg = string.Format("マガジン番号が空白 {0}", sMagazineNO);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, msg);

                    return;
                } 

				MagInfo magInfo = GetMagazineInfo(lsetInfo, sMagazineNO);

				if (string.IsNullOrEmpty(magInfo.sNascaLotNO))
                {
                    throw new Exception(string.Format(Constant.MessageInfo.Message_129, lsetInfo.EquipmentNO, DateTime.Now.ToString()));
                }

					//// ロットNoからロットマーキングNoを取得
                LotMark lotMarkData = Common.GetLotMarkingNO(lsetInfo.InlineCD, magInfo.sNascaLotNO
					, (Constant.TypeGroup)Enum.Parse(typeof(Constant.TypeGroup), settingInfoPerLine.TypeGroup)
					, (Constant.LineType)Enum.Parse(typeof(Constant.LineType), settingInfoPerLine.LineType)); //車載の処理をハードコーディング（今後の方針についても考えておく） //この行のコメントアウトは元に戻す必要あり(2013/9/12 n.yoshimoto

                logMsg = string.Format("{0} ライン:{1}/{2}/{3}/[印字文字取得]印字文字：{4} ロットNo：{5}", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lotMarkData.MarkNo, magInfo.sNascaLotNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
				runLog.logMessageQue.Enqueue(logMsg);

                bool isInsertEnd = LotMark.CancelableInsert(lsetInfo.InlineCD, lotMarkData, (Constant.TypeGroup)Enum.Parse(typeof(Constant.TypeGroup), settingInfoPerLine.TypeGroup));

				if (isInsertEnd == false)
				{
					throw new ApplicationException("ロットNo、印字文字のデータベース登録時に異常が確認され、作業者にて処理が取り消されました。");
				}

				//// 装置へロットマーキングNoを送信
				//plc.SendString(Constant.TEACH_Send_LM_MarkingNO, lotMarkingStr); //PLC処理を三菱経由からOMRON直としたので本行はコメントアウト 2015/5/9 nyoshimoto

				//※三菱、オムロンどちらでも使えるように変更する
				bool verifyLotMarkingStrResult = false;
				string recvLotMarkingStr;
				int tryCt = 1;

				do
				{
                    plc.SetString(TEACH_Send_LM_MarkingNO, lotMarkData.MarkNo);
					//plc.SetString(GetPLCMemAddress(settingInfoPerLine, Constant.TEACH_Send_LM_MarkingNO), lotMarkingStr);

                    recvLotMarkingStr = plc.GetString(TEACH_Send_LM_MarkingNO, lotMarkData.MarkNo.Length, true, false);

                    logMsg = string.Format("{0} ライン:{1}/{2}/{3}/[送信印字文字と装置受信文字の照合] 送信文字：{4} 受信文字：{5}", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lotMarkData.MarkNo, recvLotMarkingStr);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

                    if (lotMarkData.MarkNo == recvLotMarkingStr)
					{
						verifyLotMarkingStrResult = true;
					}

				} while (verifyLotMarkingStrResult == false && tryCt++ < 3);

				if (verifyLotMarkingStrResult == false)
				{
					throw new ApplicationException(
                        string.Format("ロットマーキング文字送信時異常終了：送信した印字文字({0})と装置が認識した印字文字({1})が異なるため", lotMarkData.MarkNo, recvLotMarkingStr));
				}

				logMsg = string.Format("{0} ライン:{1}/{2}/{3}/[印字文字送信完了]", DateTime.Now, lsetInfo.InlineCD, lsetInfo.ModelNM, lsetInfo.EquipmentNO);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
				runLog.logMessageQue.Enqueue(logMsg);

				//印字書き込み
				plc.SetBit(TRIG_ADDR, 1, "0");
			}

            #region 装置運転開始時のパラメータ管理

            // QCIL.xml内の設備毎のタグ「PlcParamCheckFG」がTrueの時 + PLCタイプ = キーエンスのみ、処理を行う。
            if (settingInfoPerLine.GetFullParameterFG(lsetInfo.EquipmentNO) && plcType == PLC_Keyence.PLC_TYPE)
            {
                CheckPlcParameter(lsetInfo, ref errorMessageList);
            }

            #endregion

            System.Threading.Thread.Sleep(10);
		}

        /// <summary>
        /// 装置のPLCから取得したデータと閾値を判定する
        /// </summary>
        /// <param name="moldPatternID"></param>
        /// <returns</returns>
        public void CheckPlcParameter(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
        {
            bool IsOKPrm = true;

            // PLCをインスタンス
            PLC_Keyence plcKeyence = (PLC_Keyence)this.plc;
            //PLC_Keyence plcKeyence = new PLC_Keyence(lsetInfo.IPAddressNO, lsetInfo.PortNO);
            if (plcKeyence == null)
            {
                string sMessage = $"[{lsetInfo.AssetsNM}/{lsetInfo.MachineNM}号機] PLCへの接続に失敗しました。"
                    + $"PLCタイプ = キーエンス, PLCアドレス = {lsetInfo.IPAddressNO}, Port = {lsetInfo.PortNO}";

                throw new ApplicationException(sMessage);
            }

            #region 全パラ要求アドレスのマスタを取得

            //対象ファイル種類のフラグ用PLCアドレス情報を取得
            PlcAddrInfo plcAddrInfo = PlcAddrInfo.GetData(lsetInfo.InlineCD, lsetInfo.ModelNM, Constant.PREFIX_PLCPARAM_NM);
            if (plcAddrInfo == null)
            {
                return;
            }

            // 各マスタの列名の有無を確認 
            if (string.IsNullOrEmpty(plcAddrInfo.StartONADDR))
            {
                string sMessage = $"[{lsetInfo.AssetsNM}/{lsetInfo.MachineNM}号機] PLCアドレスマスタ「運転開始要求フラグ取得アドレス」が未設定です。値 = {plcAddrInfo.StartONADDR}";
                throw new ApplicationException(sMessage);
            }
            if (string.IsNullOrEmpty(plcAddrInfo.PrgNmADDR))
            {
                string sMessage = $"[{lsetInfo.AssetsNM}/{lsetInfo.MachineNM}号機] PLCアドレスマスタ「マガジンNo取得アドレス」が未設定です。値 = {plcAddrInfo.PrgNmADDR}";
                throw new ApplicationException(sMessage);
            }
            if (plcAddrInfo.PrgNmLEN <= 0)
            {
                string sMessage = $"[{lsetInfo.AssetsNM}/{lsetInfo.MachineNM}号機] PLCアドレスマスタ「マガジンNo取得文字数」が不正です。値 = {plcAddrInfo.PrgNmLEN}";
                throw new ApplicationException(sMessage);
            }
            #endregion

            #region 全パラチェック要求信号取得

            // 処理開始フラグ取得要求
            try
            {
                if (plcKeyence.GetDataAsString(plcAddrInfo.StartONADDR, 1, PLC.DT_BINARY) != PLC.BIT_ON)
                {
                    // 運転開始ではない為、処理中断
                    return;
                }
            }
            catch(Exception ex)
            {
                string sMessage = $"パラメータ照合要求信号の取得に失敗： {ex.Message}";
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                return;
                //throw new Exception(sMessage);
            }
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                $"■設備CD={lsetInfo.EquipmentNO}：LM装置パラメータチェック-要求処理フラグONを確認");

            #endregion

            #region 投入ロット情報取得

            // マガジンNo -> 製品タイプ取得
            string sMagazineNO = plcKeyence.GetStringWithoutReplace(MAGAZINENO_ADDR, 10, false, false);
            if (string.IsNullOrEmpty(sMagazineNO))
            {
                throw new ApplicationException($"■設備CD={ lsetInfo.EquipmentNO}：LM装置パラメータチェック-マガジン番号が空白 {sMagazineNO}");
            }

            MagInfo lotInfo = GetLotInfo(lsetInfo, sMagazineNO);
            if (string.IsNullOrEmpty(lotInfo.sNascaLotNO))
            {
                throw new Exception($"■設備CD={ lsetInfo.EquipmentNO}：LM装置パラメータチェック-{string.Format(Constant.MessageInfo.Message_129, lsetInfo.EquipmentNO, DateTime.Now.ToString())}");
            }

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                $"■設備CD={ lsetInfo.EquipmentNO}：LM装置パラメータチェック-投入ロット = ロットNo:{lotInfo.sNascaLotNO}, "
                + $"タイプ：{lotInfo.sMaterialCD}");

            #endregion

            #region 各マスタの取得

            // 装置のパラメータ管理リスト(取得PLCアドレス + 閾値)
            List<PlcFileConv> plcfileconvList = Database.PlcFileConv.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, Constant.PREFIX_PLCPARAM_NM);
            if (plcfileconvList == null || plcfileconvList.Count == 0)
            {
                string sMessage = $"[{lsetInfo.AssetsNM}/{lsetInfo.MachineNM}号機]パラメータ取得用PLCアドレスマスタ(TmPlcFileConv)が設定されていません。";

                // 判定NG → 装置へのアクションは無し
                throw new Exception(sMessage);
            }

            // パラメータ管理リスト内の管理番号がNullのレコードがあるかチェック
            List<PlcFileConv> noHasQcparamList = plcfileconvList.Where(p => p.QcParamNO.HasValue == false).ToList();
            if (noHasQcparamList.Count > 0)
            {
                throw new ApplicationException(
                    $"■設備CD={lsetInfo.EquipmentNO}：LM装置パラメータチェック-パラメータ取得用PLCアドレスマスタ異常 QcParamNoがNULLです。"
                    + $"メモリAddr:{string.Join(",", noHasQcparamList.Select(n => n.PlcADDR))} ");
            }
            List<int> qcparamNOList = plcfileconvList.Select(q => q.QcParamNO.Value).ToList();

            // 投入ロットのタイプと取得したパラメータ管理リストの管理Noをキーに閾値リストを取得
            List<Plm> plmList = ConnectDB.GetPLMListData(qcparamNOList, lsetInfo.ModelNM, lsetInfo.EquipmentNO,
                lotInfo.sMaterialCD, lsetInfo.InlineCD, lsetInfo.ChipNM);

            // 取得データの確認
            foreach (PlcFileConv pfcInfo in plcfileconvList)
            {
                // 閾値の取得
                Plm plmInfo = plmList.Where(p => p.QcParamNO == pfcInfo.QcParamNO).Where(p => p.EquipmentNO == lsetInfo.EquipmentNO).FirstOrDefault();
                if (plmInfo == null)
                {
                    plmInfo = plmList.Where(p => p.QcParamNO == pfcInfo.QcParamNO).FirstOrDefault();
                }
                // 装置CDが空欄の閾値レコードもない場合、エラーと判断して、処理中断
                if (plmInfo == null)
                {
                    object pNM = ConnectDB.GetPRMElement("Parameter_NM", pfcInfo.QcParamNO.Value, lsetInfo.InlineCD);
                    if (pNM == null)
                    {
                        // 判定NG → 装置へのアクションは無し
                        throw new Exception($"[{lsetInfo.AssetsNM}/{lsetInfo.MachineNM}号機] "
                            + $"閾値名マスタ(TmPRM)に管理No[{pfcInfo.QcParamNO}]のマスタが登録されていません。");
                    }
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                         $"■設備CD={lsetInfo.EquipmentNO}：LM装置パラメータチェック-閾値マスタ(TmPLM)がありません。, "
                         + $"管理NO = {pfcInfo.QcParamNO}, 管理名 = {pNM.ToString()}, プログラム名={lotInfo.sMaterialCD}");

                    // 判定NG → 装置へのアクションは無し
                    throw new Exception($"[{lsetInfo.AssetsNM}/{lsetInfo.MachineNM}号機]"
                        + $"[管理番号:{pfcInfo.QcParamNO}/{pNM.ToString()}]の閾値(TmPLM)が設定されていません。"
                        + $"プログラム名={lotInfo.sMaterialCD}");
                }

                pfcInfo.Plm = plmInfo;
            }

            #endregion

            #region 装置PLCから各パラメータ取得

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                $"■設備CD={lsetInfo.EquipmentNO}：LM装置パラメータチェック-各PLCアドレスからデータを取得");

            // パラメータ管理リストから各アドレスの値を格納
            List<PLC_Address> tmpAdrList = new List<PLC_Address>();
            foreach (PlcFileConv tmpData in plcfileconvList)
            {
                tmpAdrList.Add(new PLC_Address(tmpData.PlcADDR, tmpData.DataTypeCD, tmpData.DataLen));
            }

            // アドレス種類毎の先頭・最後尾アドレスを算出 (リストを作成)
            Dictionary<string, PLC_Device> plcdevList = PLC_Device.GetPLCDeviceList(lsetInfo, tmpAdrList);

            // 各アドレスの値をDirectoryで取得する
            Dictionary<string, decimal> AddressValue = PLC_Device.GetAdrValList(plcKeyence, plcdevList);

            #endregion

            #region NG判定

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：LM装置パラメータチェック-NG判定開始", lsetInfo.EquipmentNO));
            
            // 取得データ ⇔ 閾値の判定
            
            foreach (PlcFileConv pfcInfo in plcfileconvList)
            {
                string sValue = "";
                string sMessage = "";

                Plm plmInfo = pfcInfo.Plm;

                if (pfcInfo.DataTypeCD == PLC.DT_STR)
                {
                    // データが文字の場合は、このタイミングでPLCからデータを取得する。
                    sValue = plcKeyence.GetString_AS_ShiftJIS(pfcInfo.PlcADDR, pfcInfo.DataLen);

                    // NG判定を行う
                    if (ConnectDB.NGJudge(plmInfo, sValue))
                    {
                        sMessage = $"[{lsetInfo.AssetsNM}/{lsetInfo.MachineNM}号機]"
                            + $"[管理番号:{plmInfo.QcParamNO}/{plmInfo.ParameterNM}]の設定値に誤りがあります。"
                            + $"タイプ={lotInfo.sMaterialCD}, 取得値={sValue}, 閾値={plmInfo.ParameterVAL}";
                        ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                        errMessageList.Add(errMessageInfo);

                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"■{sMessage}");

                        IsOKPrm = false;
                    }
                }
                else
                {

                    // NG判定を行う
                    decimal calcValue = Convert.ToDecimal(CalcChangeUnit(pfcInfo.Plm.ChangeUnitVal, Convert.ToDouble(AddressValue[pfcInfo.PlcADDR])));
                    if (ConnectDB.NGJudge(plmInfo, calcValue))
                    {
                        sValue = calcValue.ToString();
                        
                        sMessage = $"[{lsetInfo.AssetsNM}/{lsetInfo.MachineNM}号機]"
                            + $"[管理番号:{plmInfo.QcParamNO}/{plmInfo.ParameterNM}]が管理限界値を越えました。"
                            + $"タイプ={lotInfo.sMaterialCD}, 取得値={sValue}, ";
                        
                        if (plmInfo.ManageNM == Constant.sMAXMIN)
                        {
                            sMessage += $"{plmInfo.ParameterMIN} <= 閾値 <= {plmInfo.ParameterMAX}";
                        }
                        else if (plmInfo.ManageNM == Constant.sMAX)
                        {
                            sMessage += $"閾値 = {plmInfo.ParameterMAX}";
                        }
                        ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                        errMessageList.Add(errMessageInfo);

                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"■{sMessage}");

                        IsOKPrm = false;
                    }
                }
            }

            #endregion

            if (IsOKPrm == true)
            {
                // 処理終了 → 処理開始フラグを下げる
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                    $"■設備CD={lsetInfo.EquipmentNO}：LM装置パラメータチェック-装置へOK信号を送信");
                plcKeyence.SetBit2(plcAddrInfo.StartONADDR, PLC.BIT_OFF);
            }
            else
            {
                // 処理終了 EICS画面上にerrMessageListのエラーメッセージを表示
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                    $"■設備CD={lsetInfo.EquipmentNO}：LM装置パラメータチェック-装置へNG信号を送信");

                // エラーメッセージを表示して停止する
                throw new ApplicationException("LM装置パラメータチェック不一致発生 - 装置へNG信号を送信");
            }
        }

        public MagInfo GetLotInfo(LSETInfo lsetInfo, string qrValue)
        {
            MagInfo retv = new MagInfo();

            SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);

            if (string.IsNullOrEmpty(qrValue))
            {
                throw new Exception(string.Format("マガジン番号の形式が正しくありません 読込番号:{0}", qrValue));
            }
            string[] qrValueChars = qrValue.Split('\r')[0].Split(' ');
            if (qrValueChars == null || string.IsNullOrEmpty(qrValueChars[0]))
            {
                throw new Exception(string.Format("マガジン番号の形式が正しくありません 読込番号:{0}", qrValue));
            }

            string magazineNo = "";
            if (qrValueChars.Count() == 1)
            {
                magazineNo = qrValueChars[0];
            }
            else
            {
                magazineNo = qrValueChars[1];
            }

            ArmsLotInfo lot = GetLotNo_Mag(lsetInfo.InlineCD, magazineNo);
            if (lot == null)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "マガジンNoからロット情報取得");
                retv.sMagazineNO = magazineNo;
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("マガジンNoの識別子判定： 識別子={0}", qrValueChars[0]));
                if ((qrValueChars[0] == "13") || (qrValueChars[0] == "11"))
                {
                    retv.sNascaLotNO = magazineNo;

                    Arms.AsmLot armsLot = Arms.AsmLot.GetAsmLot(lsetInfo.InlineCD, retv.sNascaLotNO);
                    retv.sMaterialCD = armsLot.TypeCd;
                }
                else
                {
                    retv.sNascaLotNO = null;
                    retv.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);
                }
            }
            else
            {
                retv.sMagazineNO = magazineNo;
                retv.sNascaLotNO = lot.LotNO;
                retv.sMaterialCD = lot.TypeCD;
                retv.ProcNO = lot.ProcNO;
            }

            return retv;
        }
    }
}
