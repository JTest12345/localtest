using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Drawing;
using System.Threading;
using EICS.Database;
using EICS.Structure;

namespace EICS.Machine
{
	class AIMachineInfoTSAVI : AIMachineInfo
	{
        protected TcpClient tcp = null;
        protected NetworkStream ns = null;

		private const int FILE_OA_OPENINGCHECKFG_COL = 69;
		/// <summary>ファイルOA内容列(傾向管理フラグ)</summary>
		private const int FILE_OA_KEIKOUFG_COL = 4;

		public const int TimingNO = 61;
        private const string FILE_OA_REQ_ADDR = "EM40051";


        public AIMachineInfoTSAVI(LSETInfo lset) : base(lset)
		{
			lsetInfo = lset;
			this.LineCD = lset.InlineCD;
		}

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

                //タイプ情報送信
				string sType = GetType(ref tcp, ref ns, lsetInfo);
				if (string.IsNullOrEmpty(sType) == false)
				{
					SendTypeProcess(ref tcp, ref ns, lsetInfo.IPAddressNO, sType);
				}                

                System.Threading.Thread.Sleep(100);//0.1秒ready信号をON

                CheckPlcParameter(lsetInfo, ref this.errorMessageList, true);

                //リネームされたOAファイルがフォルダにあるならバックアップに移動
                BackupOAFile(lsetInfo);

                //「OAファイルは処理しないが無駄に信号が立つので毎ループ0を書き込む」
                resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, FILE_OA_REQ_ADDR, 0, 1, Constant.ssuffix_U);
                if (resMsg == "Error")
                {
                    base.machineStatus = Constant.MachineStatus.Stop;
                    return;
                }

                //「受付準備OKをOFF」
                resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
                if (resMsg == "Error")
                {
                    base.machineStatus = Constant.MachineStatus.Stop;
                    return;
                }
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

        protected void BackupOAFile(LSETInfo lsetInfo)
        {
            string[] fileList = Directory.GetFiles(lsetInfo.InputFolderNM, "*_OA*.csv");
            BackupMachineFileByFileNm(fileList, 3);
        }

        /// <summary>
        /// 装置のPLCから取得したデータと閾値を判定する
        /// </summary>
        /// <param name="moldPatternID"></param>
        /// <returns</returns>
        public virtual void CheckPlcParameter(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList, bool precedingJudgeResult)
        {
            const string Message_158 = "[{0}/{1}号機][管理番号:{8}/{2}]が管理限界値({3})を越えました。取得値={4},閾値{3}={5},プログラム名={6},Linecd={7}";
            const string Message_159 = "[{0}/{1}号機][管理番号:{7}/{2}]の設定値に誤りがあります。取得値={3},閾値={4},プログラム名={5},Linecd={6}";
            const string Message_160 = "[{0}/{1}号機][管理番号:{5}/{2}]の閾値が設定されていません。プログラム名={3},Linecd={4}";
            const string Message_161 = "[{0}/{1}号機]パラメータ取得用PLCアドレスマスタが設定されていません。";

            bool IsOKPrm = true;

            // PLCをインスタンス
            PLC_Keyence plc = new PLC_Keyence(lsetInfo.IPAddressNO, lsetInfo.PortNO);
            if (plc == null)
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCへの接続に失敗しました。 PLC = {2}, Port = {3}", lsetInfo.AssetsNM, lsetInfo.MachineNM, lsetInfo.IPAddressNO, lsetInfo.PortNO);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：外観検査機パラメータチェック-PLC接続失敗, PLC = {1}, Port = {2}, ", lsetInfo.EquipmentNO, lsetInfo.IPAddressNO, lsetInfo.PortNO));

                return;
            }

            //対象ファイル種類のフラグ用PLCアドレス情報を取得
            PlcAddrInfo plcAddrInfo = PlcAddrInfo.GetData(lsetInfo.InlineCD, lsetInfo.ModelNM, Constant.PREFIX_PLCPARAM_NM);
            if (plcAddrInfo == null)
            {
                return;
            }

            #region 各マスタの列名の有無を確認

            // 各マスタの列名の有無を確認 
            if (string.IsNullOrEmpty(plcAddrInfo.StartONADDR))
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCアドレスマスタ「{2}」が未設定です。値 = {3}", lsetInfo.AssetsNM, lsetInfo.MachineNM, "運転開始要求フラグ取得アドレス", plcAddrInfo.StartONADDR);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-PLCアドレスマスタ「{1}」未設定。値 = {2}", lsetInfo.EquipmentNO, "運転開始要求フラグ取得アドレス", plcAddrInfo.StartONADDR));
            }
            if (string.IsNullOrEmpty(plcAddrInfo.PrgNmADDR))
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCアドレスマスタ「{2}」が未設定です。", lsetInfo.AssetsNM, lsetInfo.MachineNM, "運転開始要求フラグ取得アドレス", plcAddrInfo.PrgNmADDR);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-PLCアドレスマスタ「{1}」未設定。値 = {2}", lsetInfo.EquipmentNO, "運転開始要求フラグ取得アドレス", plcAddrInfo.PrgNmADDR));
            }
            if (plcAddrInfo.PrgNmLEN <= 0)
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCアドレスマスタ「{2}」が不正です。値 = {3}", lsetInfo.AssetsNM, lsetInfo.MachineNM, "プログラム名取得文字数", plcAddrInfo.PrgNmLEN);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-PLCアドレスマスタ「{1}」不正。値 = {2}", lsetInfo.EquipmentNO, "運転開始要求フラグ取得アドレス", plcAddrInfo.PrgNmLEN));
            }
            if (string.IsNullOrEmpty(plcAddrInfo.PrmOKADDR))
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCアドレスマスタ「{2}」が未設定です。値 = {3}", lsetInfo.AssetsNM, lsetInfo.MachineNM, "OK信号-送信用アドレス", plcAddrInfo.PrmOKADDR);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-PLCアドレスマスタ「{1}」未設定。値 = {2}", lsetInfo.EquipmentNO, "OK信号-送信用アドレス", plcAddrInfo.PrmOKADDR));
            }
            if (string.IsNullOrEmpty(plcAddrInfo.PrmNGADDR))
            {
                string sMessage = string.Format("[{0}/{1}号機] PLCアドレスマスタ「{2}」が未設定です。値 = {3}", lsetInfo.AssetsNM, lsetInfo.MachineNM, "NG信号-送信用アドレス", plcAddrInfo.PrmNGADDR);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                errMessageList.Add(errMessageInfo); log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：MD装置パラメータチェック-PLCアドレスマスタ「{1}」未設定。値 = {2}", lsetInfo.EquipmentNO, "NG信号-送信用アドレス", plcAddrInfo.PrmNGADDR));
            }
            #endregion

            // 運転開始フラグ取得要求
            try
            {
                if (plc.GetDataAsString(plcAddrInfo.StartONADDR, 1, PLC.DT_UDEC_16BIT) == PLC.BIT_ON)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：外観検査機パラメータチェック-運転開始フラグONを確認", lsetInfo.EquipmentNO));
                }
                else
                {
                    // 運転開始ではない為、処理中断
                    return;
                }
            }
            catch (Exception err)
            {
                // 運転開始フラグ取得失敗  ⇒  メッセージ出力
                throw;
            }

            // 装置のパラメータ管理リスト(取得PLCアドレス + 閾値)
            List<PlcFileConv> plcfileconvList = Database.PlcFileConv.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, Constant.PREFIX_PLCPARAM_NM);
            if (plcfileconvList == null || plcfileconvList.Count == 0)
            {
                string sMessage = string.Format(Message_161, lsetInfo.AssetsNM, lsetInfo.MachineNM);
                ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                //errMessageList.Add(errMessageInfo); 
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                string.Format("■設備CD={0}：外観検査機パラメータチェック-パラメータ取得用PLCアドレスマスタ無, ", lsetInfo.EquipmentNO));

                // 装置へNG信号を返す
                SendOKNG(false, plc, plcAddrInfo, lsetInfo);

                throw new Exception(errMessageInfo.MessageVAL);

                //return;
            }

            List<int> qcparamNOList = new List<int>();
            foreach (PlcFileConv pfc in plcfileconvList)
            {
                if (pfc.QcParamNO.HasValue)
                {
                    qcparamNOList.Add(pfc.QcParamNO.Value);
                }
                else
                {
                    throw new ApplicationException(string.Format("■設備CD={0}：外観検査機パラメータチェック-パラメータ取得用PLCアドレスマスタ異常 QcParamNoがNULLです。ﾒﾓﾘAddr:{1} ", lsetInfo.EquipmentNO, pfc.PlcADDR));
                }
            }

            // プログラム名取得
            string ProgramNM = plc.GetString_AS_ShiftJIS(plcAddrInfo.PrgNmADDR, plcAddrInfo.PrgNmLEN);
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：外観検査機パラメータチェック-プログラム名 = {1}", lsetInfo.EquipmentNO, ProgramNM));

            // プログラム名と取得したパラメータ管理リストの管理Noをキーに閾値リストを取得
            List<Plm> plmList = ConnectDB.GetPLMListData(qcparamNOList, lsetInfo.ModelNM, lsetInfo.EquipmentNO, ProgramNM, lsetInfo.InlineCD, null);

            string rMessage = "";
            //string lMessage = "";

            // 取得データの確認
            foreach (PlcFileConv pfcInfo in plcfileconvList)
            {
                string sMessage = "";

                // 閾値の取得
                Plm plmInfo = plmList.Where(p => p.QcParamNO == pfcInfo.QcParamNO).Where(p => p.EquipmentNO == lsetInfo.EquipmentNO).FirstOrDefault();
                if (plmInfo == null)
                {
                    plmInfo = plmList.Where(p => p.QcParamNO == pfcInfo.QcParamNO).FirstOrDefault();

                    // 装置CDが空欄の閾値レコードもない場合、エラーと判断して、処理中断
                    if (plmInfo == null)
                    {
                        string pNM;
                        try
                        {
                            if (pfcInfo.QcParamNO.HasValue)
                            {
                                pNM = ConnectDB.GetPRMElement("Parameter_NM", pfcInfo.QcParamNO.Value, lsetInfo.InlineCD).ToString();
                            }
                            else
                            {
                                throw new ApplicationException(
                                    string.Format("■設備CD={0}：外観検査機パラメータチェック-パラメータ取得用PLCアドレスマスタ異常 QcParamNoがNULLです。ﾒﾓﾘAddr:{1} ", lsetInfo.EquipmentNO, pfcInfo.PlcADDR));
                            }
                        }
                        catch (Exception err)
                        {
                            sMessage = string.Format("[{0}/{1}号機] 閾値名マスタ(TmPRM)に管理No[{2}]のマスタが登録されていません。", lsetInfo.AssetsNM, lsetInfo.MachineNM, pfcInfo.QcParamNO);
                            rMessage += sMessage + "\n";

                            //IsOKPrm = false;

                            //continue;

                            // 装置へNG信号を返す
                            SendOKNG(false, plc, plcAddrInfo, lsetInfo);

                            throw new Exception(sMessage);

                        }
                        sMessage = string.Format(Message_160, lsetInfo.AssetsNM, lsetInfo.MachineNM, pNM, ProgramNM, lsetInfo.InlineCD, pfcInfo.QcParamNO);
                        //tMessage += sMessage + "\r\n";
                        //ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                        //errMessageList.Add(errMessageInfo); 
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                        string.Format("■設備CD={0}：外観検査機パラメータチェック-閾値マスタ無, 管理NO = {1}, 管理名 = {2}, ", lsetInfo.EquipmentNO, pfcInfo.QcParamNO, pNM));

                        //IsOKPrm = false;

                        //continule;

                        // 装置へNG信号を返す
                        SendOKNG(false, plc, plcAddrInfo, lsetInfo);

                        throw new Exception(sMessage);

                    }
                }

                pfcInfo.Plm = plmInfo;
            }
            if (!IsOKPrm)
            {
                // 装置へNG信号を返す
                SendOKNG(false, plc, plcAddrInfo, lsetInfo);
                throw new Exception(rMessage);

                //return;
            }

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：外観検査機パラメータチェック-各PLCアドレスからデータを取得 = {1}", lsetInfo.EquipmentNO, ProgramNM));

            // パラメータ管理リストから各アドレスの値を格納
            List<PLC_Address> tmpAdrList = new List<PLC_Address>();
            foreach (PlcFileConv tmpData in plcfileconvList)
            {
                tmpAdrList.Add(new PLC_Address(tmpData.PlcADDR, tmpData.DataTypeCD, tmpData.DataLen));
            }

            // アドレス種類毎の先頭・最後尾アドレスを算出 (リストを作成)
            Dictionary<string, PLC_Device> plcdevList = PLC_Device.GetPLCDeviceList(lsetInfo, tmpAdrList);

            // 各アドレスの値をDirectoryで取得する
            Dictionary<string, decimal> AddressValue = PLC_Device.GetAdrValList(plc, plcdevList);


            #region NG判定

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：外観検査機パラメータチェック-NG判定開始", lsetInfo.EquipmentNO));

            // 取得データ ⇔ 閾値の判定
            foreach (PlcFileConv pfcInfo in plcfileconvList)
            {
                string sValue = "";
                string sMessage = "";

                Plm plmInfo = pfcInfo.Plm;

                if (pfcInfo.DataTypeCD == PLC.DT_STR)
                {
                    // データが文字の場合は、このタイミングでPLCからデータを取得する。
                    sValue = plc.GetString_AS_ShiftJIS(pfcInfo.PlcADDR, pfcInfo.DataLen);
                    // NG判定を行う
                    if (ConnectDB.NGJudge(plmInfo, sValue))
                    {
                        sMessage = string.Format(Message_159, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo.ParameterNM, sValue, plmInfo.ParameterVAL, ProgramNM, lsetInfo.InlineCD, plmInfo.QcParamNO);
                        ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                        errMessageList.Add(errMessageInfo);
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                        string.Format("■設備CD={0}：外観検査機パラメータチェック-NG検出, 管理NO = {1}, 管理名 = {2}, 装置PLC取得値 = {3}, 閾値 = {4}", lsetInfo.EquipmentNO, plmInfo.QcParamNO, plmInfo.ParameterNM, sValue, plmInfo.ParameterVAL));

                        IsOKPrm = false;
                    }
                }
                else
                {
                    // NG判定を行う
                    decimal ChangeUnitValue = Convert.ToDecimal(CalcChangeUnit(pfcInfo.Plm.ChangeUnitVal, Convert.ToDouble(AddressValue[pfcInfo.PlcADDR])));
                    if (ConnectDB.NGJudge(plmInfo, ChangeUnitValue))
                    {
                        sValue = ChangeUnitValue.ToString();

                        if (plmInfo.ManageNM == Constant.sMAXMIN)
                        {
                            sMessage = string.Format(Message_158, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo.ParameterNM, "MAX-MIN", AddressValue[pfcInfo.PlcADDR], plmInfo.ParameterMAX + "-" + plmInfo.ParameterMIN, ProgramNM, lsetInfo.InlineCD, plmInfo.QcParamNO);
                            ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                            errMessageList.Add(errMessageInfo);

                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                                string.Format("■設備CD={0}：外観検査機パラメータチェック-NG検出, 管理NO = {1}, 管理名 = {2}, 装置PLC取得値 = {3}, {4} <= 閾値 <= {5}", lsetInfo.EquipmentNO, plmInfo.QcParamNO, plmInfo.ParameterNM, AddressValue[pfcInfo.PlcADDR], plmInfo.ParameterMIN, plmInfo.ParameterMAX));
                        }
                        else if (plmInfo.ManageNM == Constant.sMAX)
                        {
                            sMessage = string.Format(Message_158, lsetInfo.AssetsNM, lsetInfo.MachineNM, plmInfo.ParameterNM, "MAX", AddressValue[pfcInfo.PlcADDR], plmInfo.ParameterMAX, ProgramNM, lsetInfo.InlineCD, plmInfo.QcParamNO);
                            ErrMessageInfo errMessageInfo = new ErrMessageInfo(sMessage, Color.Red);
                            errMessageList.Add(errMessageInfo);

                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                                string.Format("■設備CD={0}：外観検査機パラメータチェック-NG検出, 管理NO = {1}, 管理名 = {2}, 装置PLC取得値 = {3}, 閾値 <= {4}", lsetInfo.EquipmentNO, plmInfo.QcParamNO, plmInfo.ParameterNM, AddressValue[pfcInfo.PlcADDR], plmInfo.ParameterMAX));
                        }

                        IsOKPrm = false;
                    }
                }
            }

            #endregion

            bool chkResult = precedingJudgeResult & IsOKPrm;

            SendOKNG(chkResult, plc, plcAddrInfo, lsetInfo);
        }

        ///<summary>
        /// OK信号またはNG信号を返す。
        ///</summary>
        protected void SendOKNG(bool OKFg, PLC_Keyence plc, PlcAddrInfo plcAddrInfo, LSETInfo lsetInfo)
        {
            if (OKFg)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：外観検査機パラメータチェック-装置へOK信号を送信", lsetInfo.EquipmentNO));

                // 装置へOK信号を返す
                plc.SetBit2(plcAddrInfo.PrmOKADDR, PLC.BIT_ON);

            }
            else
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("■設備CD={0}：外観検査機パラメータチェック-装置へNG信号を送信", lsetInfo.EquipmentNO));

                // 装置へNG信号を返す
                plc.SetBit2(plcAddrInfo.PrmNGADDR, PLC.BIT_ON);
            }


            // 運転開始フラグ取得要求
            plc.SetBit2(plcAddrInfo.StartONADDR, PLC.BIT_OFF);
        }
    }
}
