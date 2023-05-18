using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EICS.Structure;
using System.Threading;

namespace EICS.Machine
{
    /// <summary>
    /// スパッタ装置(DMC)
    /// </summary>
    public class SUPMachineInfo2 : SUPMachineInfo
    {
        PLC_MelsecUDP machinePLC;


        //レシピ書込み完了信号。SUP無印の関数を継承しているが落とす信号が違うので名前と実体が異なる。
        protected override string QRCODE_SETTING_OK_ADDR { get { return "L002EE1"; } }

        protected override string SET_JUDGE_FG { get { return "ZR01D556"; } }
        protected override string QRCODE_ADDR { get { return "ZR01D4CA"; } }
        protected override int QRCODE_PLCDEVICE_LEN { get { return 18; } }

        ~SUPMachineInfo2()
        {
            if (machinePLC != null)
                machinePLC.Dispose();
        }

        public override void CheckFile(LSETInfo lsetInfo)
        {
            SettingInfo commonSettingInfo = SettingInfo.GetSingleton();
            List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList
                = commonSettingInfo.PlcExtractExclusionList.Where(p => p.ModelNm == lsetInfo.ModelNM).ToList();

            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            ConnectPLC(lsetInfo.IPAddressNO, lsetInfo.PortNO, exclusionList);
            startProcess(lsetInfo);

        }

        private void startProcess(LSETInfo lsetInfo)
        {
            base.machineStatus = Constant.MachineStatus.Runtime;

            SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            //QR書込み完了フラグ監視
            int nw = int.MinValue;
            nw = Convert.ToInt32(machinePLC.GetBit(QRCODE_SETTING_OK_ADDR));

            //☆装置通信--------

            //■SUPのマガジンロード時の動作
            //マガジンNo書込み完了フラグON
            if (nw > 0)
            {
                MagInfo magInfo = GetLotInfo(lsetInfo);

                //調査用ログ。量産開始後トラブルなければ廃止可。2017.04.19
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, 
                    $"ｽﾊﾟｯﾀ装置:{lsetInfo.EquipmentNO} ロット情報取得 マガジン：{magInfo.sMagazineNO}、ロット：{magInfo.sNascaLotNO}、タイプ：{magInfo.sMaterialCD}");


                LSETInfo lsetInfoForWstJudge = (LSETInfo)lsetInfo.Clone();
                lsetInfoForWstJudge.TypeCD = magInfo.sMaterialCD;

                JudgeProcess(lsetInfoForWstJudge);
            }
        }

        protected MagInfo GetLotInfo(LSETInfo lsetInfo)
        {
            MagInfo magInfo = new MagInfo();
            SettingInfo commonSetting = SettingInfo.GetSingleton();

            string sMagazineNo = machinePLC.GetMagazineNo(QRCODE_ADDR, QRCODE_PLCDEVICE_LEN);

            if (string.IsNullOrWhiteSpace(sMagazineNo) == true)
            {
                throw new ApplicationException($"ｽﾊﾟｯﾀ装置：{lsetInfo.EquipmentCD}  装置からマガジン番号が取得できませんでした。");
            }


            //■マガジンNoからLot/Type取得
            ARMSLotInfo rtnArmsLotInfo = ConnectDB.GetARMSLotInfoFromMag(lsetInfo.InlineCD, sMagazineNo);

            if (string.IsNullOrWhiteSpace(rtnArmsLotInfo.TypeCD) == true)
            {
                throw new ApplicationException($"マガジンNoから現在流動中のロットを取得出来ません。MagazineNo:{ sMagazineNo }");
            }
            
            magInfo.sMagazineNO = sMagazineNo;
            magInfo.sNascaLotNO = rtnArmsLotInfo.LotNO;
            magInfo.sMaterialCD = rtnArmsLotInfo.TypeCD;
            
            return magInfo;
        }

        /// <summary>
        /// N工場スパッタと異なりタイプを送信しないのでそこをオミットしたものでオーバーライド
        /// タイプの切り替え待ちする箇所を削除。
        /// </summary>
        /// <param name="lsetInfo"></param>
        protected override void JudgeProcess(LSETInfo lsetInfo)
        {

            PLCDDGBasedMachine plcDDGBasedMac = new PLCDDGBasedMachine(lsetInfo);

            plcDDGBasedMac.CreateFileProcess(lsetInfo, true);

            List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();
            plcDDGBasedMac.StartingProcess(lsetInfo, out errMsgList);

            if (errMsgList.Count > 0)
            {
                AlertLog alertLog = AlertLog.GetInstance();
                foreach (ErrMessageInfo errMsg in errMsgList)
                {
                    alertLog.logMessageQue.Enqueue(errMsg.MessageVAL);
                }
            }

            //plcDDGBasedMac.SendResultProcess(lsetInfo, true);			

            JudgeProcess(plcDDGBasedMac, lsetInfo, true);
        }
        protected override void ConnectPLC(string ipAddress, int portNO, List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList)
        {
            if (machinePLC == null)
            {
                machinePLC = new PLC_MelsecUDP(ipAddress, portNO, exclusionList);
            }
            //else
            //{
            //    if (machinePLC  ConnectedPLC() == false)
            //    {
            //        machinePLC.ConnectPLC();
            //    }
            //}
        }

        protected override void JudgeProcess(PLCDDGBasedMachine plcDDGBasedMac, LSETInfo lsetInfo, bool isStartUp)
        {
            string sMessage;

            try
            {
                if (plcDDGBasedMac.CheckForMachineStopFile(isStartUp))
                {
                    sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ﾚｼﾋﾟ判定NG送信 {2} = 2", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, SET_JUDGE_FG);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                    //判定結果NG送信
                    machinePLC.SetWordAsDecimalData(SET_JUDGE_FG, 2);
                }
                else if (IsThereOKFile(lsetInfo))
                {
                    sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ﾚｼﾋﾟ判定OK送信 {2} = 1", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, SET_JUDGE_FG);
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                    //判定結果OK送信
                    machinePLC.SetWordAsDecimalData(SET_JUDGE_FG, 1);
                }
            }
            catch (Exception err)
            {
                //判定結果NG送信
                machinePLC.SetWordAsDecimalData(SET_JUDGE_FG, 2);
                Thread.Sleep(100);
                machinePLC.SetBit(QRCODE_SETTING_OK_ADDR, 1, "0");

                throw new Exception(err.ToString(), err);
            }

            Thread.Sleep(100);

            sMessage = string.Format("[設備号機]={0} /[設備CD]={1} ﾏｶﾞｼﾞﾝNo書込みOFF {2} = 0", lsetInfo.EquipmentCD, lsetInfo.EquipmentNO, QRCODE_SETTING_OK_ADDR);
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

            machinePLC.SetBit(QRCODE_SETTING_OK_ADDR, 1, "0");
        }
    }
}
