using ARMS3.Model.Carriers;
using ARMS3.Model.Machines;
using ARMS3.Model.PLC;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ARMS3.Model
{
    public class LineConfig
    {
        private const int POINT_UNKNOWN = -1;

        private const string SETTING_FILE_NM = "lineconfig.xml";
        private const string SETTING_FILE_FULLPATH = @"C:\ARMS\Config\lineconfig.xml";


        private const string OUTPUT_FOLDER_NAME = "Output";
		private const string INPUT_FOLDER_NAME = "Input";
		private const string MAPPING_FOLDER_NAME = "Mapping";

        /// <summary>
        /// クラス外からの呼び出し用
        /// </summary>
        public static string LineConfigFilePath => SETTING_FILE_FULLPATH;

        /// <summary>
        /// ネットワークサーバの設定ファイルの更新日時が新しい場合、ローカルにコピーする
        /// </summary>
        public static bool CopySettingFileFromServer(string serverPath, int retryCt)
        {
            string serverFullPath = Path.Combine(serverPath, SETTING_FILE_NM);
            if (File.Exists(serverFullPath) == false)
            {
                throw new ApplicationException(
                    string.Format("ネットワークサーバの設定ファイルが存在しませんでした。設定に間違いがないか確認して下さい。コピー先:{0}、コピー元:{1}", SETTING_FILE_FULLPATH, serverFullPath));
            }

            DateTime localTimeStamp = File.GetLastWriteTime(SETTING_FILE_FULLPATH);
            DateTime serverTimeStamp = File.GetLastWriteTime(serverFullPath);

            if (retryCt >= 5)
            {
                throw new ApplicationException(
                    string.Format("ネットワークサーバの設定ファイルコピーに失敗しました。ファイルが編集可能か確認して下さい。コピー先:{0}、コピー元:{1}", SETTING_FILE_FULLPATH, serverFullPath));
            }

            if (localTimeStamp < serverTimeStamp)
            {
                try
                {
                    File.Copy(serverFullPath, SETTING_FILE_FULLPATH, true);

                    return true;
                }
                catch (IOException)
                {
                    System.Threading.Thread.Sleep(1000);
                    retryCt = retryCt + 1;
                    CopySettingFileFromServer(serverPath, retryCt);
                    return false;
                }
            }

            return false;
        }

        public static IEnumerable<XElement> LoadMachineLineConfig(XDocument doc)
        {            
            IEnumerable<XElement> machines = doc.Elements("settings").Elements("machine")
                .Where(mac => string.IsNullOrEmpty(mac.Element("machineNo").Value) == false);
            return machines;
        }

        public static void LoadLineConfig()
        {
            XDocument doc = XDocument.Load(SETTING_FILE_FULLPATH);
            IEnumerable<XElement> machines = LoadMachineLineConfig(doc);
            foreach (XElement elm in machines)
            {
                IMachine m = GetMachine(elm);
                if (m == null) { continue; }

                //サーバー側の削除フラグ確認
                MachineInfo svrMac = MachineInfo.GetMachine(m.MacNo, false);
                if (svrMac == null) continue;

                m.MacNo = svrMac.MacNo;
                m.PlantCd = svrMac.NascaPlantCd;
                m.Name = svrMac.LongName;
                m.MacGroup = svrMac.MacGroup;
				m.IsAutoLine = svrMac.IsAutoLine;
				m.IsHighLine = svrMac.IsHighLine;
				m.IsOutLine = svrMac.IsOutLine;
				m.LogInputDirectoryPath = svrMac.LogInputDirectoryPath;
                m.LogOutputDirectoryPath = svrMac.LogOutputDirectoryPath;
                m.AltMapInputDirectoryPath = svrMac.AltMapInputDirectoryPath;
                m.AltMapOutputDirectoryPath = svrMac.AltMapOutputDirectoryPath;
                m.LineNo = svrMac.LineNo;
                #region 2016.09.29 フォルダの持ち方変更のため廃止
                //if (!string.IsNullOrWhiteSpace(svrMac.WatchingDirectoryPath))
                //{
                //	MachineType flow = (MachineType)Enum.Parse(typeof(MachineType), elm.Element("flow").Value);
                //	if (flow == MachineType.DieBonder6 || flow == MachineType.DieBonder7)
                //	{
                //		m.LogInputDirectoryPath = Path.Combine(svrMac.WatchingDirectoryPath, MAPPING_FOLDER_NAME);
                //		m.LogOutputDirectoryPath = Path.Combine(svrMac.WatchingDirectoryPath, MAPPING_FOLDER_NAME);
                //	}
                //	else
                //	{
                //		m.LogInputDirectoryPath = Path.Combine(svrMac.WatchingDirectoryPath, INPUT_FOLDER_NAME);
                //		m.LogOutputDirectoryPath = Path.Combine(svrMac.WatchingDirectoryPath, OUTPUT_FOLDER_NAME);
                //	}
                //}
                #endregion
                m.IsSubstrateComplete = svrMac.IsSubstrateComplete;
                m.SignalDisplayFlowName = svrMac.SignalDisplayFlowName;

                LineKeeper.Machines.Add(m);
            }

            IEnumerable<XElement> carriers = doc.Elements("settings").Elements("carrier")
                .Where(car => string.IsNullOrEmpty(car.Element("carrierNo").Value) == false);
            foreach (XElement elm in carriers)
            {
                ICarrier c = GetCarrier(elm);
                if (c == null) { continue; }

                if (c.Plc != null)
                {
                    c.IsPowerON = c.Plc.Ping(c.Plc.IPAddress, 50, 1);
                }

                LineKeeper.Carriers.Add(c);

                // ロック用オブジェクトの追加
                foreach(string semName in c.SemaphoreSlimList.Values)
                {
                    // 同じキーのsemNameのオブジェクトは一つまで
                    if (LineKeeper.SemaphoreSlims.ContainsKey(semName) == false)
                    {
                        System.Threading.SemaphoreSlim sem = new System.Threading.SemaphoreSlim(1, 1);
                        LineKeeper.SemaphoreSlims.Add(semName, sem);
                    }
                }

                if ((c is Bridge) == false)
                {
                    c.LoadLastMagazineSetTimeXMLFile();
                }
            }

            // 装置優先順位を取得
            foreach(IMachine m in LineKeeper.Machines)
            {
                if (m.CarrierPlc == null)
                    continue;

                // 搬送ロボの電源ONなら取得
                if (LineKeeper.Carriers.Where(c => c.IsPowerON && c.Plc.IPAddress == m.CarrierPlc.IPAddress).Any())
                {
                    m.ReloadPriority();
                }
            }

            //オーブンプロファイル自動切り替え
            #region オーブンプロファイル関連アドレス設定

            IEnumerable<XElement> ovenProfileElm = doc.Elements("settings").Elements("ovenProfile");
            if (ovenProfileElm.Count() != 0)
            {
                XElement profile = ovenProfileElm.First();

                LineKeeper.OvenProfileChanger = new OvenProfiler();

                LineKeeper.OvenProfileChanger.DBOvenNumbers = int.Parse(profile.Element("dbOvenNumbers").Value);
                LineKeeper.OvenProfileChanger.MoldOvenNumbers = int.Parse(profile.Element("moldOvenNumbers").Value);

                LineKeeper.OvenProfileChanger.ReadyBitAddress = profile.Element("profileChangerReadyBitAddress").Value;

                LineKeeper.OvenProfileChanger.DBChangeInterlockBit = profile.Element("dbOvenChangeInterlockStartAddress").Value;
                LineKeeper.OvenProfileChanger.MoldChangeInterlockBit = profile.Element("moldOvenChangeInterlockStartAddress").Value;

                LineKeeper.OvenProfileChanger.DBCurrentProfileWord = profile.Element("dbOvenCurrentProfileStartAddress").Value;
                LineKeeper.OvenProfileChanger.MoldCurrentProfileWord = profile.Element("moldOvenCurrentProfileStartAddress").Value;

                LineKeeper.OvenProfileChanger.DBReserveProfileWord = profile.Element("dbOvenReserveProfileStartAddress").Value;
                LineKeeper.OvenProfileChanger.MoldReserveProfileWord = profile.Element("moldOvenReserveProfileStartAddress").Value;

                LineKeeper.OvenProfileChanger.DBAutoModeBit = profile.Element("dbOvenAutoModeBitStartAddress").Value;
                LineKeeper.OvenProfileChanger.MoldAutoModeBit = profile.Element("moldOvenAutoModeBitStartAddress").Value;

                LineKeeper.OvenProfileChanger.DBProfileChangeCompleteBit = profile.Element("dbOvenProfileChangeCompleteBitStartAddress").Value;
                LineKeeper.OvenProfileChanger.MoldProfileChangeCompleteBit = profile.Element("moldOvenProfileChangeCompleteBitStartAddress").Value;

                LineKeeper.OvenProfileChanger.DBProfileChangeRequestBit = profile.Element("dbOvenProfileChangeRequestBitStartAddress").Value;
                LineKeeper.OvenProfileChanger.MoldProfileChangeRequestBit = profile.Element("moldOvenProfileChangeRequestBitStartAddress").Value;

                string dbprocno = profile.Element("dbProfileReserveProcNo").Value;
                if (!string.IsNullOrEmpty(dbprocno))
                {
                    OvenProfiler.DBProfileReserveProcNo = int.Parse(dbprocno);
                }

                string mdprocno = profile.Element("moldProfileReserveProcNo").Value;
                if (!string.IsNullOrEmpty(mdprocno))
                {
                    OvenProfiler.MoldProfileReserveProcNo = int.Parse(mdprocno);
                }
            }
#endregion


            if (LineKeeper.OvenProfileChanger != null)
            {
                foreach (ICarrier carrier in LineKeeper.Carriers)
                {
                    if (carrier is Robot || carrier is Robot3 || carrier is Robot4 || carrier is Robot5)
                    {
                        LineKeeper.OvenProfileChanger.Initialize(((CarrierBase)carrier).Plc);
                    }
                }
            }
        }

        public static IMachine GetMachine(XElement elm)
        {
            try
            {
				MachineBase retv = null; string hostIpAddress = string.Empty;
                MachineType flow = (MachineType)Enum.Parse(typeof(MachineType), elm.Element("flow").Value);
                switch (flow)
                {
                    case MachineType.RobotQRReader:
                        retv = new RobotQRReader();
                        break;

                    case MachineType.DieBonder:
#region

                        retv = new DieBonder();
                        ((DieBonder)retv).WaferBitAddressStart = elm.Element("waferBitAddressStart").Value;
                        ((DieBonder)retv).StartWorkTableSensorAddress = elm.Element("startWorkTableSensorAddress").Value;
                        ((DieBonder)retv).WaferChangerChangeSensorBitAddress = elm.Element("waferChangerChangeBitAddress").Value;
                        ((DieBonder)retv).SecondLoaderReqBitAddress = elm.Element("secondLoaderReqBitAddress").Value;
                        ((DieBonder)retv).SecondEmpMagLoaderReqBitAddress = elm.Element("secondEmpMagLoaderReqBitAddress").Value;
                        XElement magazineTakeoutElm = elm.Element("magazineTakeoutBitAddress");
                        if (magazineTakeoutElm != null)
                        {
                            ((DieBonder)retv).MagazineTakeoutBitAddress = magazineTakeoutElm.Value;
                        }

                        XElement unloaderMoveCompleteDBElm = elm.Element("unloaderMoveCompleteBitAddress");
                        if (unloaderMoveCompleteDBElm != null)
                        {
                            ((DieBonder)retv).UnloaderMoveCompleteBitAddress = unloaderMoveCompleteDBElm.Value;
                        }

#endregion
                        break;

					case MachineType.DieBonder2:
#region

						retv = new DieBonder2();
						((DieBonder2)retv).StartWorkTableSensorAddress = elm.Element("startWorkTableSensorAddress").Value;
						((DieBonder2)retv).SecondLoaderReqBitAddress = elm.Element("secondLoaderReqBitAddress").Value;
						((DieBonder2)retv).SecondEmpMagLoaderReqBitAddress = elm.Element("secondEmpMagLoaderReqBitAddress").Value;
						magazineTakeoutElm = elm.Element("magazineTakeoutBitAddress");
						if (magazineTakeoutElm != null)
						{
							((DieBonder2)retv).MagazineTakeoutBitAddress = magazineTakeoutElm.Value;
						}

						unloaderMoveCompleteDBElm = elm.Element("unloaderMoveCompleteBitAddress");
						if (unloaderMoveCompleteDBElm != null)
						{
							((DieBonder2)retv).UnloaderMoveCompleteBitAddress = unloaderMoveCompleteDBElm.Value;
						}

#endregion
						break;

                    case MachineType.WireBonder:
#region

                        retv = new WireBonder();
                        XElement secondLoaderReqElm = elm.Element("secondLoaderReqBitAddress");
                        if (secondLoaderReqElm != null)
                        {
                            ((WireBonder)retv).SecondLoaderReqBitAddress = secondLoaderReqElm.Value;
                        }

                        XElement secondEmpMagLoaderReqElm = elm.Element("secondEmpMagLoaderReqBitAddress");
                        if (secondEmpMagLoaderReqElm != null)
                        {
                            ((WireBonder)retv).SecondEmpMagLoaderReqBitAddress = secondEmpMagLoaderReqElm.Value;
                        }

                        //抜取り工程
                        XElement elmInpsectProcNo = elm.Element("inspectProcNo");
                        if (elmInpsectProcNo != null)
                        {
                            ((WireBonder)retv).InspectProcNo = int.Parse(elmInpsectProcNo.Value);
                        }

                        XElement magazineWbTakeoutElm = elm.Element("magazineTakeoutBitAddress");
                        if (magazineWbTakeoutElm != null)
                        {
                            ((WireBonder)retv).MagazineTakeoutBitAddress = magazineWbTakeoutElm.Value;
                        }

                        XElement unloaderMoveCompleteWBElm = elm.Element("unloaderMoveCompleteBitAddress");
                        if (unloaderMoveCompleteWBElm != null)
                        {
                            ((WireBonder)retv).UnloaderMoveCompleteBitAddress = unloaderMoveCompleteWBElm.Value;
                        }

                        //XElement isCompletionTriggerFileElm = elm.Element("isCompletionTriggerFile");
                        //if (isCompletionTriggerFileElm != null)
                        //{
                        //    ((WireBonder)retv).IsCompletionTriggerFile = bool.Parse(isCompletionTriggerFileElm.Value);
                        //}

                        #endregion
                        break;

                    case MachineType.Mold:
#region
                        retv = new Mold();
                        ((Mold)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
                        ((Mold)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
                        
                        XElement elmEULMagazineAddress = elm.Element("eulMagazineAddress");
                        if (elmEULMagazineAddress != null)
                        {
                            ((Mold)retv).EULMagazineAddress = elmEULMagazineAddress.Value;
                        }

                        //以下はマガジン数量取得用
                        XElement elmELMagazineCountAddress = elm.Element("elMagazineCountAddress");
                        if (elmELMagazineCountAddress != null)
                        {
                            ((Mold)retv).ELMagazineCountAddress = elmELMagazineCountAddress.Value;
                        }
                        XElement elmLMagazineCountAddress = elm.Element("lMagazineCountAddress");
                        if (elmLMagazineCountAddress != null)
                        {
                            ((Mold)retv).LMagazineCountAddress = elmLMagazineCountAddress.Value;
                        }
                        XElement elmULMagazineCountAddress = elm.Element("ulMagazineCountAddress");
                        if (elmULMagazineCountAddress != null)
                        {
                            ((Mold)retv).ULMagazineCountAddress = elmULMagazineCountAddress.Value;
                        }
                        XElement elmEULMagazineCountAddress = elm.Element("eulMagazineCountAddress");
                        if (elmEULMagazineCountAddress != null)
                        {
                            ((Mold)retv).EULMagazineCountAddress = elmEULMagazineCountAddress.Value;
                        }

                        //以下は高効率で必要
                        XElement elmUnloaderReqBitAddressStart = elm.Element("unloaderReqBitAddressStart");
                        if (elmUnloaderReqBitAddressStart != null)
                        {
                            ((Mold)retv).UnloaderReqBitAddressStart = elmUnloaderReqBitAddressStart.Value;
                        }
                        XElement elmUnloaderMagazineDataAddressStart = elm.Element("unloaderMagazineDataAddressStart");
                        if (elmUnloaderMagazineDataAddressStart != null)
                        {
                            ((Mold)retv).UnloaderMagazineDataAddressStart = elmUnloaderMagazineDataAddressStart.Value;
                        }
                        XElement elmLoaderMagazineAddress = elm.Element("loaderMagazineAddress");
                        if (elmLoaderMagazineAddress != null)
                        {
                            ((Mold)retv).LoaderMagazineAddress = elmLoaderMagazineAddress.Value;
                        }
                        XElement elmNASCA_NG_REASON_WORD = elm.Element("nascaNGReasonWord");
                        if (elmNASCA_NG_REASON_WORD != null)
                        {
                            ((Mold)retv).NASCA_NG_REASON_WORD = elmNASCA_NG_REASON_WORD.Value;
                        }
                        XElement elmNASCA_OK_BIT = elm.Element("nascaOKBit");
                        if (elmNASCA_OK_BIT != null)
                        {
                            ((Mold)retv).NASCA_OK_BIT = elmNASCA_OK_BIT.Value;
                        }
                        XElement elmNASCA_NG_BIT = elm.Element("nascaNGBit");
                        if (elmNASCA_NG_BIT != null)
                        {
                            ((Mold)retv).NASCA_NG_BIT = elmNASCA_NG_BIT.Value;
                        }
                        XElement elmLoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress");
                        if (elmLoaderQRReadCompleteBitAddress != null)
                        {
                            ((Mold)retv).LoaderQRReadCompleteBitAddress = elmLoaderQRReadCompleteBitAddress.Value;
                        }
						XElement elmIsQRReader = elm.Element("isQRReader");
						if (elmIsQRReader != null)
						{
							((Mold)retv).IsQRReader = Convert.ToBoolean(elmIsQRReader.Value);
						}

#endregion
                        break;
                    case MachineType.Mold2:
#region
                        retv = new Mold2();
                        ((Mold2)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
                        ((Mold2)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
                        XElement elm2EULMagazineAddress = elm.Element("eulMagazineAddress");
                        if (elm2EULMagazineAddress != null)
                        {
                            ((Mold2)retv).EULMagazineAddress = elm2EULMagazineAddress.Value;
                        }
#endregion
                        break;
					case MachineType.Mold3:
#region
                        retv = new Mold3();
						((Mold3)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
						((Mold3)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;

						elmEULMagazineAddress = elm.Element("eulMagazineAddress");
						if (elmEULMagazineAddress != null)
						{
							((Mold3)retv).EULMagazineAddress = elmEULMagazineAddress.Value;
						}

						elmIsQRReader = elm.Element("isQRReader");
						if (elmIsQRReader != null)
						{
							((Mold3)retv).IsQRReader = Convert.ToBoolean(elmIsQRReader.Value);
						}

						//以下はマガジン数量取得用
						elmELMagazineCountAddress = elm.Element("elMagazineCountAddress");
						if (elmELMagazineCountAddress != null)
						{
							((Mold3)retv).ELMagazineCountAddress = elmELMagazineCountAddress.Value;
						}
						elmLMagazineCountAddress = elm.Element("lMagazineCountAddress");
						if (elmLMagazineCountAddress != null)
						{
							((Mold3)retv).LMagazineCountAddress = elmLMagazineCountAddress.Value;
						}
						elmULMagazineCountAddress = elm.Element("ulMagazineCountAddress");
						if (elmULMagazineCountAddress != null)
						{
							((Mold3)retv).ULMagazineCountAddress = elmULMagazineCountAddress.Value;
						}
						elmEULMagazineCountAddress = elm.Element("eulMagazineCountAddress");
						if (elmEULMagazineCountAddress != null)
						{
							((Mold3)retv).EULMagazineCountAddress = elmEULMagazineCountAddress.Value;
						}
#endregion
                        break;

                    case MachineType.Cut:
#region
                        retv = new Cut();
                        ((Cut)retv).IsMagReverseWorkBitAddress = elm.Element("isMagReverseWorkBitAddress").Value;
#endregion
                        break;

                    case MachineType.FrameLoader:
#region

                        retv = new FrameLoader();
                        var sub = elm.Element("empMagLoaderReqBitAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((FrameLoader)retv).EmptyMagLoaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub = elm.Element("unloaderReqBitAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((FrameLoader)retv).UnloaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub = elm.Element("workStartTimeAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((FrameLoader)retv).WorkStartTimeAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub = elm.Element("workCompleteTimeAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((FrameLoader)retv).WorkCompleteTimeAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub = elm.Element("stocker1WordAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((FrameLoader)retv).Stocker1WordAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub = elm.Element("stocker2WordAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((FrameLoader)retv).Stocker2WordAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub = elm.Element("empMagLoaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((FrameLoader)retv).EmptyMagLoaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub = elm.Element("unloaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((FrameLoader)retv).UnloaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }

                        //搭載機ロットマーキングデータ読み取りデータ確認用
                        XElement elmFrameLoaderLocalPlcIPAddress = elm.Element("frameLoaderLocalPlcIPAddress");
                        XElement elmFrameLoaderLocalPlcPort = elm.Element("frameLoaderLocalPlcPort");
                        int port;
                        if (elmFrameLoaderLocalPlcIPAddress != null && elmFrameLoaderLocalPlcPort != null &&
                            !string.IsNullOrWhiteSpace(elmFrameLoaderLocalPlcIPAddress.Value) && int.TryParse(elmFrameLoaderLocalPlcPort.Value, out port) == true)
                        {
                            ((FrameLoader)retv).LocalPLC = new Mitsubishi(elmFrameLoaderLocalPlcIPAddress.Value, port);
                        }

                        ((FrameLoader)retv).LotMarkingNeedProgramNo = int.Parse(elm.Element("lotMarkingNeedProgramNo").Value);
                        ((FrameLoader)retv).LotMarkingForbiddenProgramNo = int.Parse(elm.Element("lotMarkingForbiddenProgramNo").Value);

                        //自動搬送個別PLC(Robot3/Robot4)用
                        XElement elmEmpMagLoaderDoBitAddressList = elm.Element("empMagLoaderDoBitAddressList");
                        if (elmEmpMagLoaderDoBitAddressList != null)
                        {
                            sub = elmEmpMagLoaderDoBitAddressList.Elements("address");
                            if (sub != null)
                            {
                                foreach (XElement subelm in sub)
                                {
                                    ((FrameLoader)retv).EmptyMagLoaderDoBitAddressList.Add(
                                    int.Parse(subelm.Attribute("key").Value), subelm.Value);
                                }
                            }
                        }
                        XElement elmUnloaderDoBitAddressList = elm.Element("unloaderDoBitAddressList");
                        if (elmUnloaderDoBitAddressList != null)
                        {
                            sub = elmUnloaderDoBitAddressList.Elements("address");
                            if (sub != null)
                            {
                                foreach (XElement subelm in sub)
                                {
                                    ((FrameLoader)retv).UnloaderDoBitAddressList.Add(
                                    int.Parse(subelm.Attribute("key").Value), subelm.Value);
                                }
                            }
                        }


                        #endregion
                        break;

					case MachineType.FrameLoader2:
#region

						retv = new FrameLoader2();
						sub = elm.Element("empMagLoaderReqBitAddressList").Elements("address");
						foreach (XElement subelm in sub)
						{
							((FrameLoader2)retv).EmptyMagLoaderReqBitAddressList.Add(
							int.Parse(subelm.Attribute("key").Value), subelm.Value);
						}
						sub = elm.Element("unloaderReqBitAddressList").Elements("address");
						foreach (XElement subelm in sub)
						{
							((FrameLoader2)retv).UnloaderReqBitAddressList.Add(
							int.Parse(subelm.Attribute("key").Value), subelm.Value);
						}
						sub = elm.Element("workStartTimeAddressList").Elements("address");
						foreach (XElement subelm in sub)
						{
							((FrameLoader2)retv).WorkStartTimeAddressList.Add(
							int.Parse(subelm.Attribute("key").Value), subelm.Value);
						}
						sub = elm.Element("workCompleteTimeAddressList").Elements("address");
						foreach (XElement subelm in sub)
						{
							((FrameLoader2)retv).WorkCompleteTimeAddressList.Add(
							int.Parse(subelm.Attribute("key").Value), subelm.Value);
						}
						sub = elm.Element("stocker1WordAddressList").Elements("address");
						foreach (XElement subelm in sub)
						{
							((FrameLoader2)retv).Stocker1WordAddressList.Add(
							int.Parse(subelm.Attribute("key").Value), subelm.Value);
						}
						sub = elm.Element("stocker2WordAddressList").Elements("address");
						foreach (XElement subelm in sub)
						{
							((FrameLoader2)retv).Stocker2WordAddressList.Add(
							int.Parse(subelm.Attribute("key").Value), subelm.Value);
						}
						sub = elm.Element("empMagLoaderPointList").Elements("point");
						foreach (XElement subelm in sub)
						{
							((FrameLoader2)retv).EmptyMagLoaderPointList.Add(
							int.Parse(subelm.Attribute("key").Value), subelm.Value);
						}
						sub = elm.Element("unloaderPointList").Elements("point");
						foreach (XElement subelm in sub)
						{
							((FrameLoader2)retv).UnloaderPointList.Add(
							int.Parse(subelm.Attribute("key").Value), subelm.Value);
						}

						//搭載機ロットマーキングデータ読み取りデータ確認用
						elmFrameLoaderLocalPlcIPAddress = elm.Element("frameLoaderLocalPlcIPAddress");
						elmFrameLoaderLocalPlcPort = elm.Element("frameLoaderLocalPlcPort");
						if (elmFrameLoaderLocalPlcIPAddress != null && elmFrameLoaderLocalPlcPort != null &&
							!string.IsNullOrWhiteSpace(elmFrameLoaderLocalPlcIPAddress.Value) && int.TryParse(elmFrameLoaderLocalPlcPort.Value, out port) == true)
						{
							((FrameLoader2)retv).LocalPLC = new Mitsubishi(elmFrameLoaderLocalPlcIPAddress.Value, port);
						}

                        ((FrameLoader)retv).LotMarkingNeedProgramNo = int.Parse(elm.Element("lotMarkingNeedProgramNo").Value);
                        ((FrameLoader)retv).LotMarkingForbiddenProgramNo = int.Parse(elm.Element("lotMarkingForbiddenProgramNo").Value);

#endregion
						break;

                    case MachineType.FrameLoader3:
#region

                        retv = new FrameLoader3();
                        var sub3 = elm.Element("empMagLoaderReqBitAddressList").Elements("address");
                        foreach (XElement subelm in sub3)
                        {
                            ((FrameLoader3)retv).EmptyMagLoaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub3 = elm.Element("unloaderReqBitAddressList").Elements("address");
                        foreach (XElement subelm in sub3)
                        {
                            ((FrameLoader3)retv).UnloaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub3 = elm.Element("workStartTimeAddressList").Elements("address");
                        foreach (XElement subelm in sub3)
                        {
                            ((FrameLoader3)retv).WorkStartTimeAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub3 = elm.Element("workCompleteTimeAddressList").Elements("address");
                        foreach (XElement subelm in sub3)
                        {
                            ((FrameLoader3)retv).WorkCompleteTimeAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub3 = elm.Element("stocker1WordAddressList").Elements("address");
                        foreach (XElement subelm in sub3)
                        {
                            ((FrameLoader)retv).Stocker1WordAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub3 = elm.Element("stocker2WordAddressList").Elements("address");
                        foreach (XElement subelm in sub3)
                        {
                            ((FrameLoader3)retv).Stocker2WordAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub3 = elm.Element("empMagLoaderPointList").Elements("point");
                        foreach (XElement subelm in sub3)
                        {
                            ((FrameLoader3)retv).EmptyMagLoaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }


                        //搭載機ロットマーキングデータ読み取りデータ確認用
                        XElement elmFrameLoaderLocalPlcIPAddress3 = elm.Element("frameLoaderLocalPlcIPAddress");
                        XElement elmFrameLoaderLocalPlcPort3 = elm.Element("frameLoaderLocalPlcPort");
                        int port3;
                        if (elmFrameLoaderLocalPlcIPAddress3 != null && elmFrameLoaderLocalPlcPort3 != null &&
                            !string.IsNullOrWhiteSpace(elmFrameLoaderLocalPlcIPAddress3.Value) && int.TryParse(elmFrameLoaderLocalPlcPort3.Value, out port3) == true)
                        {
                            ((FrameLoader3)retv).LocalPLC = new Mitsubishi(elmFrameLoaderLocalPlcIPAddress3.Value, port3);
                        }

                        ((FrameLoader3)retv).PT1StockerNoAddress = elm.Element("pt1StockerNoAddress").Value;
                        ((FrameLoader3)retv).PT2StockerNoAddress = elm.Element("pt2StockerNoAddress").Value;
                        ((FrameLoader3)retv).PT3StockerNoAddress = elm.Element("pt3StockerNoAddress").Value;
                        ((FrameLoader3)retv).StockerNoRequestBitAddress = elm.Element("stockerNoRequestBitAddress").Value;
                        ((FrameLoader3)retv).StockerNoCommandBitAddress = elm.Element("stockerNoCommandBitAddress").Value;
                        ((FrameLoader3)retv).StockerNoWordAddress = elm.Element("stockerNoWordAddress").Value;
                        ((FrameLoader3)retv).ProgramNoAddress = elm.Element("programNoAddress").Value;
                        ((FrameLoader3)retv).NextBufferMacNo = int.Parse(elm.Element("nextBufferMacNo").Value);
                        ((FrameLoader3)retv).NextBuffer2MacNo = int.Parse(elm.Element("nextBuffer2MacNo").Value);
                        ((FrameLoader3)retv).LocalPLC = new Mitsubishi(elm.Element("localPlcIpAddress").Value, int.Parse(elm.Element("localPlcPort").Value));

#endregion
                        break;

                    case MachineType.FrameLoader3ULBuffer:
#region
                        retv = new FrameLoader3ULBuffer();
                        ((FrameLoader3ULBuffer)retv).PrevBufferMacNo = int.Parse(elm.Element("prevBufferMacNo").Value);
#endregion
                        break;

                    case MachineType.FrameLoader4:
                        retv = new FrameLoader4();
                        ((FrameLoader4)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
                        ((FrameLoader4)retv).WorkStartOKBitAddress = elm.Element("workStartOKBitAddress").Value;
                        ((FrameLoader4)retv).WorkStartNGBitAddress = elm.Element("workStartNGBitAddress").Value;
                        break;

                    case MachineType.Plasma:
#region

                        retv = new Plasma();
                        ((Plasma)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
                        sub = elm.Element("inputForbiddenProcBitAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((Plasma)retv).ProcForbiddenAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }

                        sub = elm.Element("dischargeModeProcBitAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((Plasma)retv).ProcDischargeAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
#endregion
                        break;

					case MachineType.PlasmaMagRotation:
#region

						retv = new PlasmaMagRotation();
						((PlasmaMagRotation)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
						((PlasmaMagRotation)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
						sub = elm.Element("inputForbiddenProcBitAddressList").Elements("address");
						foreach (XElement subelm in sub)
						{
							((PlasmaMagRotation)retv).ProcForbiddenAddressList.Add(
							int.Parse(subelm.Attribute("key").Value), subelm.Value);
						}

						sub = elm.Element("dischargeModeProcBitAddressList").Elements("address");
						foreach (XElement subelm in sub)
						{
							((PlasmaMagRotation)retv).ProcDischargeAddressList.Add(
							int.Parse(subelm.Attribute("key").Value), subelm.Value);
						}

						XElement elmHostIpAddress = elm.Element("hostIpAddress");
						if (elmHostIpAddress != null)
						{
							hostIpAddress = elmHostIpAddress.Value;
						}

						//以下は高生産性の設定
						elmLoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress");
						if (elmLoaderQRReadCompleteBitAddress != null)
						{
							((PlasmaMagRotation)retv).LoaderQRReadCompleteBitAddress = elmLoaderQRReadCompleteBitAddress.Value;
						}
						XElement elmWorkStartOKWordAddress = elm.Element("workStartOKWordAddress");
						if (elmWorkStartOKWordAddress != null)
						{
							((PlasmaMagRotation)retv).WorkStartOKWordAddress = elmWorkStartOKWordAddress.Value;
						}
						XElement elmlMagazineMnfctWordAddress = elm.Element("lMagazineMnfctWordAddress");
						if (elmlMagazineMnfctWordAddress != null)
						{
							((PlasmaMagRotation)retv).lMagazineMnfctWordAddress = elmlMagazineMnfctWordAddress.Value;
						}

						((PlasmaMagRotation)retv).WorkCompleteOKBitAddress = elm.Element("workCompleteOKBitAddress").Value;

#endregion
						break;

                    case MachineType.Inspector:
#region
                        retv = new Inspector();
						((Inspector)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
                        ((Inspector)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
						XElement elmMMFilePath = elm.Element("mmFilePath");
                        if (elmMMFilePath != null)
                        {
                            ((Inspector)retv).MMFilePath = elmMMFilePath.Value;
                        }
#endregion
                        break;

                    case MachineType.Inspector2:
#region
                        retv = new Inspector2();
						((Inspector2)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
                        ((Inspector2)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
                        ((Inspector2)retv).OutputReserveBitAddress = elm.Element("outputReserveBitAddress").Value;
						((Inspector2)retv).LoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress").Value;
						((Inspector2)retv).WorkStartOKBitAddress = elm.Element("workStartOKBitAddress").Value;
						((Inspector2)retv).WorkStartNGBitAddress = elm.Element("workStartNGBitAddress").Value;
						((Inspector2)retv).WorkCompleteNGBitAddress = elm.Element("workCompleteNGBitAddress").Value;
						((Inspector2)retv).IsWorkStartAutoComplete = bool.Parse(elm.Element("isWorkStartAutoComplete").Value);
						((Inspector2)retv).IsWorkEndAutoComplete = bool.Parse(elm.Element("isWorkEndAutoComplete").Value);
						elmMMFilePath = elm.Element("mmFilePath");
                        if (elmMMFilePath != null)
                        {
                            ((Inspector2)retv).MMFilePath = elmMMFilePath.Value;
                        }
#endregion
                        break;

					case MachineType.Inspector3:
						retv = new Inspector3();
						((Inspector3)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
						((Inspector3)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
						((Inspector3)retv).OutputReserveBitAddress = elm.Element("outputReserveBitAddress").Value;
						((Inspector3)retv).LoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress").Value;
						((Inspector3)retv).WorkStartOKBitAddress = elm.Element("workStartOKBitAddress").Value;
						((Inspector3)retv).WorkStartNGBitAddress = elm.Element("workStartNGBitAddress").Value;
						((Inspector3)retv).WorkCompleteNGBitAddress = elm.Element("workCompleteNGBitAddress").Value;
						((Inspector3)retv).IsWorkStartAutoComplete = bool.Parse(elm.Element("isWorkStartAutoComplete").Value);
						((Inspector3)retv).IsWorkEndAutoComplete = bool.Parse(elm.Element("isWorkEndAutoComplete").Value);
						break;

                    case MachineType.Oven:
#region

                        retv = new Oven();
                        ((Oven)retv).ProfileStatusAddress = elm.Element("profileStatusAddress").Value;
                        ((Oven)retv).CurrentProfileWordAddress = elm.Element("currentProfileWordAddress").Value;
                        ((Oven)retv).ChangeProfileInterlockBitAddress = elm.Element("changeProfileInterlockBitAddress").Value;
                        sub = elm.Element("loaderReqBitAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((Oven)retv).LoaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub = elm.Element("unloaderReqBitAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((Oven)retv).UnloaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
                        sub = elm.Element("loaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((Oven)retv).LoaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);

                        }
                        sub = elm.Element("unloaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((Oven)retv).UnloaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);

                        }
                        try
                        {
                            ((Oven)retv).OvenProcessStatusWordAddress = elm.Element("ovenProcessStatusWordAddress").Value;
                            ((Oven)retv).IsUseOvenProcessStatus = bool.Parse(elm.Element("isUseOvenProcessStatus").Value);					
                        }
                        catch
                        {
                            ((Oven)retv).OvenProcessStatusWordAddress = null;
                            ((Oven)retv).IsUseOvenProcessStatus = false;
                        }
                        XElement ovenDoorOpenStatusBitAddressElm = elm.Element("ovenDoorOpenStatusBitAddress");
                        if (ovenDoorOpenStatusBitAddressElm != null)
                        {
                            ((Oven)retv).OvenDoorOpenStatusBitAddress = ovenDoorOpenStatusBitAddressElm.Value;
                        }
                        // MachineBaseに移動
                        //XElement ovenAllDoorCloseBitAddressElm = elm.Element("ovenAllDoorCloseBitAddress");
                        //if (ovenAllDoorCloseBitAddressElm != null)
                        //{
                        //    ((Oven)retv).OvenAllDoorCloseBitAddress = ovenAllDoorCloseBitAddressElm.Value;
                        //}
                        XElement powerCutsBitAddressElm = elm.Element("powerCutsBitAddress");
                        if (powerCutsBitAddressElm != null)
                        {
                            ((Oven)retv).PowerCutsBitAddress = powerCutsBitAddressElm.Value;
                        }

                        XElement workStartTimeListElm = elm.Element("workStartTimeAddressList");
                        if (workStartTimeListElm != null)
                        {
                            sub = workStartTimeListElm.Elements("workStartTimeAddress");
                            ((Oven)retv).WorkStartTimeAddressList = new List<string>();
                            foreach (XElement subelm in sub)
                            {
                                ((Oven)retv).WorkStartTimeAddressList.Add(subelm.Value);
                            }
                            if (((Oven)retv).WorkStartTimeAddressList.Count() != 6) throw new Exception();
                        }
                        XElement workCompleteTimeListElm = elm.Element("workCompleteTimeAddressList");
                        if (workCompleteTimeListElm != null)
                        {
                            sub = workCompleteTimeListElm.Elements("workCompleteTimeAddress");
                            ((Oven)retv).WorkCompleteTimeAddressList = new List<string>();
                            foreach (XElement subelm in sub)
                            {
                                ((Oven)retv).WorkCompleteTimeAddressList.Add(subelm.Value);
                            }
                            if (((Oven)retv).WorkCompleteTimeAddressList.Count() != 6) throw new Exception();
                        }
                        #endregion
                        break;


                    case MachineType.ManualOvenWithProgramWait:
                        retv = new ManualOvenWithProgramWait();
                        break;

                    case MachineType.DischargeConveyor:
                        retv = new DischargeConveyor();
                        break;

                    case MachineType.AutoLineOutConveyor:
                        retv = new AutoLineOutConveyor();
                        break;

                    case MachineType.OpeningCheckConveyor:
                        retv = new OpeningCheckConveyor();
                        ((OpeningCheckConveyor)retv).MagazineExistBitAddress = elm.Element("magazineExistBitAddress").Value;
                        ((OpeningCheckConveyor)retv).OpeningCheckBitAddress = elm.Element("openingCheckBitAddress").Value;
                        ((OpeningCheckConveyor)retv).ParentMacNo = int.Parse(elm.Element("parentMacNo").Value);
                        break;

                    case MachineType.MultiLoadPlasma:
#region

                        retv = new MultiLoadPlasma();
                        sub = elm.Element("loader").Elements("lane");
                        foreach (XElement subelm in sub)
                        {
                            MultiLoadPlasma.Lane ln = new MultiLoadPlasma.Lane();
                            ln.ST = (Station)Enum.Parse(typeof(Station), subelm.Attribute("station").Value);
                            ln.Point = subelm.Attribute("point").Value;
                            ln.ReqBitAddress = subelm.Attribute("reqBit").Value;
                            int key = int.Parse(subelm.Attribute("key").Value);
                            ((MultiLoadPlasma)retv).LoaderLanes.Add(key, ln);
                        }
                        sub = elm.Element("unloader").Elements("lane");
                        foreach (XElement subelm in sub)
                        {
                            MultiLoadPlasma.Lane ln = new MultiLoadPlasma.Lane();
                            ln.ST = (Station)Enum.Parse(typeof(Station), subelm.Attribute("station").Value);
                            ln.Point = subelm.Attribute("point").Value;
                            ln.ReqBitAddress = subelm.Attribute("reqBit").Value;
                            int key = int.Parse(subelm.Attribute("key").Value);
                            ((MultiLoadPlasma)retv).UnloaderLanes.Add(key, ln);
                        }
                        sub = elm.Element("empMagLoader").Elements("lane");
                        foreach (XElement subelm in sub)
                        {
                            MultiLoadPlasma.Lane ln = new MultiLoadPlasma.Lane();
                            ln.ST = (Station)Enum.Parse(typeof(Station), subelm.Attribute("station").Value);
                            ln.Point = subelm.Attribute("point").Value;
                            ln.ReqBitAddress = subelm.Attribute("reqBit").Value;
                            int key = int.Parse(subelm.Attribute("key").Value);
                            ((MultiLoadPlasma)retv).EmpMagLoaderLanes.Add(key, ln);
                        }
                        sub = elm.Element("empMagUnloader").Elements("lane");
                        foreach (XElement subelm in sub)
                        {
                            MultiLoadPlasma.Lane ln = new MultiLoadPlasma.Lane();
                            ln.ST = (Station)Enum.Parse(typeof(Station), subelm.Attribute("station").Value);
                            ln.Point = subelm.Attribute("point").Value;
                            ln.ReqBitAddress = subelm.Attribute("reqBit").Value;
                            int key = int.Parse(subelm.Attribute("key").Value);
                            ((MultiLoadPlasma)retv).EmpMagUnloaderLanes.Add(key, ln);
                        }
                        XElement elms = elm.Element("loaderDo");
                        if (elms != null)
                        {
                            sub = elms.Elements("lane");
                            foreach (XElement subelm in sub)
                            {
                                MultiLoadPlasma.Lane ln = new MultiLoadPlasma.Lane();
                                ln.ST = (Station)Enum.Parse(typeof(Station), subelm.Attribute("station").Value);
                                ln.Point = subelm.Attribute("point").Value;
                                ln.ReqBitAddress = subelm.Attribute("reqBit").Value;
                                int key = int.Parse(subelm.Attribute("key").Value);
                                ((MultiLoadPlasma)retv).LoaderDoLanes.Add(key, ln);
                            }
                        }
                        elms = elm.Element("unloaderDo");
                        if (elms != null)
                        {
                            sub = elms.Elements("lane");
                            foreach (XElement subelm in sub)
                            {
                                MultiLoadPlasma.Lane ln = new MultiLoadPlasma.Lane();
                                ln.ST = (Station)Enum.Parse(typeof(Station), subelm.Attribute("station").Value);
                                ln.Point = subelm.Attribute("point").Value;
                                ln.ReqBitAddress = subelm.Attribute("reqBit").Value;
                                int key = int.Parse(subelm.Attribute("key").Value);
                                ((MultiLoadPlasma)retv).UnloaderDoLanes.Add(key, ln);
                            }
                        }
                        elms = elm.Element("empMagLoaderDo");
                        if (elms != null)
                        {
                            sub = elms.Elements("lane");
                            foreach (XElement subelm in sub)
                            {
                                MultiLoadPlasma.Lane ln = new MultiLoadPlasma.Lane();
                                ln.ST = (Station)Enum.Parse(typeof(Station), subelm.Attribute("station").Value);
                                ln.Point = subelm.Attribute("point").Value;
                                ln.ReqBitAddress = subelm.Attribute("reqBit").Value;
                                int key = int.Parse(subelm.Attribute("key").Value);
                                ((MultiLoadPlasma)retv).EmpMagLoaderDoLanes.Add(key, ln);
                            }
                        }
                        elms = elm.Element("empMagUnloaderDo");
                        if (elms != null)
                        {
                            sub = elms.Elements("lane");
                            foreach (XElement subelm in sub)
                            {
                                MultiLoadPlasma.Lane ln = new MultiLoadPlasma.Lane();
                                ln.ST = (Station)Enum.Parse(typeof(Station), subelm.Attribute("station").Value);
                                ln.Point = subelm.Attribute("point").Value;
                                ln.ReqBitAddress = subelm.Attribute("reqBit").Value;
                                int key = int.Parse(subelm.Attribute("key").Value);
                                ((MultiLoadPlasma)retv).EmpMagUnloaderDoLanes.Add(key, ln);
                            }
                        }
                        #endregion
                        break;

                    case MachineType.LineBridge:
#region

                        retv = new LineBridge();

                        ((LineBridge)retv).EmptyMagazineInputBitAddress = elm.Element("empMagInputBitAddress").Value;

#endregion
                        break;

                    case MachineType.LineBridge2:
#region

                        retv = new LineBridge2();

                        ((LineBridge2)retv).EmptyMagazineInputBitAddress = elm.Element("empMagInputBitAddress").Value;

#endregion
                        break;

                    case MachineType.LineBridgeBuffer:
#region

                        retv = new LineBridgeBuffer();
                        XElement elmBufferCode = elm.Element("bufferCode");
                        if (elmBufferCode != null)
                        {
                            ((LineBridgeBuffer)retv).BufferCode = int.Parse(elmBufferCode.Value);
                        }                        
#endregion
                        break;

                    case MachineType.LoadConveyor:
#region

                        retv = new LoadConveyor();
                        ((LoadConveyor)retv).MagazineArriveBitAddress = elm.Element("magazineArriveBitAddress").Value;
                        ((LoadConveyor)retv).OutputReserveBitAddress = elm.Element("outputReserveBitAddress").Value;
                        ((LoadConveyor)retv).MissingReservedMagazineBitAddress = elm.Element("missingReservedMagazineBitAddress").Value;
                        ((LoadConveyor)retv).QRReaderWordAddress = elm.Element("qrReaderWordAddress").Value;

#endregion
                        break;

                    case MachineType.EmptyMagazineLoadConveyor:
                        retv = new EmptyMagazineLoadConveyor();
                        break;

                    case MachineType.HybridLoadConveyor:
#region
                        retv = new HybridLoadConveyor();
                        ((HybridLoadConveyor)retv).MagazineArriveBitAddress = elm.Element("magazineArriveBitAddress").Value;
                        ((HybridLoadConveyor)retv).OutputReserveBitAddress = elm.Element("outputReserveBitAddress").Value;
                        ((HybridLoadConveyor)retv).MissingReservedMagazineBitAddress = elm.Element("missingReservedMagazineBitAddress").Value;
                        ((HybridLoadConveyor)retv).QRReaderWordAddress = elm.Element("qrReaderWordAddress").Value;
                        ((HybridLoadConveyor)retv).UnLoaderMagazineStatusWordAddress = elm.Element("unLoaderMagazineStatusWordAddress").Value;
                        ((HybridLoadConveyor)retv).UnloaderMagazineDischargeCheckCycleSecond = int.Parse(elm.Element("unloaderMagazineDischargeCheckCycleSecond").Value);

                        #endregion
                        break;

                    case MachineType.LoadConveyorMagRotation:
                        #region
                        retv = new LoadConveyorMagRotation();
                        ((LoadConveyorMagRotation)retv).MagazineArriveBitAddress = elm.Element("magazineArriveBitAddress").Value;
                        ((LoadConveyorMagRotation)retv).OutputReserveBitAddress = elm.Element("outputReserveBitAddress").Value;
                        ((LoadConveyorMagRotation)retv).MissingReservedMagazineBitAddress = elm.Element("missingReservedMagazineBitAddress").Value;
                        ((LoadConveyorMagRotation)retv).QRReaderWordAddress = elm.Element("qrReaderWordAddress").Value;
                        ((LoadConveyorMagRotation)retv).UnLoaderMagazineStatusWordAddress = elm.Element("unLoaderMagazineStatusWordAddress").Value;
                        ((LoadConveyorMagRotation)retv).RotationReqBitAddress = elm.Element("rotationReqBitAddress").Value;
                        ((LoadConveyorMagRotation)retv).MagazineCanInputCheckCycleSecond = int.Parse(elm.Element("magazineCanInputCheckCycleSecond").Value);
                        ((LoadConveyorMagRotation)retv).LoaderMagazineDischargeCheckCycleSecond = int.Parse(elm.Element("loaderMagazineDischargeCheckCycleSecond").Value);
                        ((LoadConveyorMagRotation)retv).UnloaderMagazineDischargeCheckCycleSecond = int.Parse(elm.Element("unloaderMagazineDischargeCheckCycleSecond").Value);
                        ((LoadConveyorMagRotation)retv).UnderStepMagazineExistsBitAddress = elm.Element("underStepMagazineExistsBitAddress").Value;
                        ((LoadConveyorMagRotation)retv).UpperRotaionReqBitAddress = elm.Element("upperRotaionReqBitAddress").Value;
                        #endregion
                        break;

                    case MachineType.MAPBoardDischargeConveyor:
                        retv = new MAPBoardDischargeConveyor();
                        break;

                    case MachineType.MAPBoardMagazineLoadConveyor:
                        retv = new MAPBoardMagazineLoadConveyor();
                        break;

                    case MachineType.MAPCompltDischargeConveyor:
                        retv = new MAPCompltDischargeConveyor();
                        break;

                    case MachineType.MAP1stDieBonder:
#region

                        retv = new MAP1stDieBonder();
                        ((MAP1stDieBonder)retv).WaferBitAddressStart = elm.Element("waferBitAddressStart").Value;
                        ((MAP1stDieBonder)retv).StartWorkTableSensorAddress = elm.Element("startWorkTableSensorAddress").Value;
                        ((MAP1stDieBonder)retv).WaferChangerChangeSensorBitAddress = elm.Element("waferChangerChangeBitAddress").Value;                       
                        ((MAP1stDieBonder)retv).SecondLoaderReqBitAddress = elm.Element("secondLoaderReqBitAddress").Value;
                        ((MAP1stDieBonder)retv).SecondEmpMagLoaderReqBitAddress = elm.Element("secondEmpMagLoaderReqBitAddress").Value;
                        
                        //バッドマーク不良自動登録用
                        ((MAP1stDieBonder)retv).DefectFileBasePath = elm.Element("defectFileBasePath").Value;
                        ((MAP1stDieBonder)retv).BadMarkClassCd = elm.Element("badMarkClassCd").Value;
                        ((MAP1stDieBonder)retv).BadMarkCauseCd = elm.Element("badMarkCauseCd").Value;
                        ((MAP1stDieBonder)retv).BadMarkDefetCd = elm.Element("badMarkDefetCd").Value;

                        //以下高効率で必要
                        XElement elmMagazineTakeoutBitAddress = elm.Element("magazineTakeoutBitAddress");
                        if (elmMagazineTakeoutBitAddress != null)
                        {
                            ((MAP1stDieBonder)retv).MagazineTakeoutBitAddress = elmMagazineTakeoutBitAddress.Value;
                        }
                        XElement elmUnloaderMoveCompleteBitAddress = elm.Element("unloaderMoveCompleteBitAddress");
                        if (elmUnloaderMoveCompleteBitAddress != null)
                        {
                            ((MAP1stDieBonder)retv).UnloaderMoveCompleteBitAddress = elmUnloaderMoveCompleteBitAddress.Value;
                        }

#endregion
                        break;

                    case MachineType.ECK:
#region

                        retv = new ECK();
                        ((ECK)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
                        ((ECK)retv).DummyMagStationReqBitAddress = elm.Element("dummyMagStationReqBitAddress").Value;
                        ((ECK)retv).DummyMagStationOutputReqBitAddress = elm.Element("dummyMagStationOutputReqBitAddress").Value;
                        ((ECK)retv).DummyMagReqBitAddress = elm.Element("dummyMagReqBitAddress").Value;
                        ((ECK)retv).DummyMagStationLoaderPoint = elm.Element("dummyMagStationLoaderPoint").Value;
                        ((ECK)retv).DummyMagStationUnloaderPoint = elm.Element("dummyMagStationUnloaderPoint").Value;

                        sub = elm.Element("unloaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK)retv).UnloaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }

                        sub = elm.Element("unloaderReqBitAddressList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK)retv).UnloaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }

                        sub = elm.Element("loaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK)retv).LoaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }

                        sub = elm.Element("loaderReqBitAddressList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK)retv).LoaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
#endregion
                        break;

                    case MachineType.ECK2:
#region
                        retv = new ECK2();
                        ((ECK2)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
                        ((ECK2)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;

                        //以下は高効率で必要
                        XElement elmEckUnloaderReqBitAddressStart = elm.Element("unloaderReqBitAddressStart");
                        if (elmEckUnloaderReqBitAddressStart != null)
                        {
                            ((ECK2)retv).UnloaderReqBitAddressStart = elmEckUnloaderReqBitAddressStart.Value;
                        }
                        XElement elmEckUnloaderMagazineDataAddressStart = elm.Element("MagazineDataAddressStart");
                        if (elmEckUnloaderMagazineDataAddressStart != null)
                        {
                            ((ECK2)retv).MagazineDataAddressStart = elmEckUnloaderMagazineDataAddressStart.Value;
                        }

#endregion
                        break;
					case MachineType.ECK3:
						retv = new ECK3();

#region 供給側バッファ
						sub = elm.Element("inputBufferLoaderReqBitAddressList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).InputBufferLoaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
						sub = elm.Element("inputBufferUnloaderReqBitAddressList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).InputBufferUnloaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
						sub = elm.Element("inputBufferLoaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).InputBufferLoaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
						sub = elm.Element("inputBufferUnloaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).InputBufferUnloaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
						sub = elm.Element("inputBufferLoaderLocationList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).InputBufferLoaderLocationList.Add(
                            int.Parse(subelm.Attribute("key").Value), (Station)Enum.Parse(typeof(Station), subelm.Value));
                        }
                        sub = elm.Element("inputBufferUnloaderLocationList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).InputBufferUnloaderLocationList.Add(
                            int.Parse(subelm.Attribute("key").Value), (Station)Enum.Parse(typeof(Station), subelm.Value));
                        }

#endregion

#region 本体
						((ECK3)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
						((ECK3)retv).UnloaderReqPlanBitAddress = elm.Element("unloaderReqPlanBitAddress").Value;
						((ECK3)retv).LoaderReqPlanBitAddress = elm.Element("loaderReqPlanBitAddress").Value;

						sub = elm.Element("loaderReqBitAddressList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).LoaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
						sub = elm.Element("unloaderReqBitAddressList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).UnloaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
						sub = elm.Element("loaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).LoaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
						sub = elm.Element("unloaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).UnloaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
#endregion

#region 排出側バッファ
						sub = elm.Element("outputBufferLoaderReqBitAddressList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).OutputBufferLoaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
						sub = elm.Element("outputBufferUnloaderReqBitAddressList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).OutputBufferUnloaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
						sub = elm.Element("outputBufferLoaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).OutputBufferLoaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
						sub = elm.Element("outputBufferUnloaderPointList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).OutputBufferUnloaderPointList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }
						sub = elm.Element("outputBufferLoaderLocationList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).OutputBufferLoaderLocationList.Add(
                            int.Parse(subelm.Attribute("key").Value), (Station)Enum.Parse(typeof(Station), subelm.Value));
                        }
                		sub = elm.Element("outputBufferUnloaderLocationList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK3)retv).OutputBufferUnloaderLocationList.Add(
                            int.Parse(subelm.Attribute("key").Value), (Station)Enum.Parse(typeof(Station), subelm.Value));
                        }

#endregion

#region ダミーマガジン

						((ECK3)retv).DummyMagStationReqBitAddress = elm.Element("dummyMagStationReqBitAddress").Value;
                        ((ECK3)retv).DummyMagStationOutputReqBitAddress = elm.Element("dummyMagStationOutputReqBitAddress").Value;
                        ((ECK3)retv).DummyMagReqBitAddress = elm.Element("dummyMagReqBitAddress").Value;
                        ((ECK3)retv).DummyMagStationLoaderPoint = elm.Element("dummyMagStationLoaderPoint").Value;
                        ((ECK3)retv).DummyMagStationUnloaderPoint = elm.Element("dummyMagStationUnloaderPoint").Value;

#endregion

						break;

                    case MachineType.ECK4:
#region
                        retv = new ECK4();
                        ((ECK4)retv).IpAddress = elm.Element("localPlcIPAddress").Value;
                        ((ECK4)retv).Port = int.Parse(elm.Element("localPlcPort").Value);
                        ((ECK4)retv).CombinedMagExchangerMacNo = int.Parse(elm.Element("CombinedMacNo").Value);
                        sub = elm.Element("loaderReqBitAddressList").Elements("point");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK4)retv).LoaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }

                        sub = elm.Element("loaderPointList").Elements("point");
                        if (sub != null)
                        {
                            foreach (XElement subelm in sub)
                            {
                                ((ECK4)retv).LoaderPointList.Add(
                                int.Parse(subelm.Attribute("key").Value), subelm.Value);
                            }
                        }

                        ((ECK4)retv).MagECKProcessHasWordAddress = elm.Element("magECKProcessHasWordddress").Value;

                        ((ECK4)retv).Pc_Ready();
                        
                        break;
#endregion


                    case MachineType.ECK4MagExchanger:
#region 
                        
                        retv = new ECK4MagExchanger();

                        // 相方の遠心沈降機が通常用とOC用の2種類になるので自動で切り替わる仕様に変更
                        //    指定macnoと同じ設備CD かつ delfg = 0のmacnoに切り替える
                        //((ECK4MagExchanger)retv).CombinedECKMacNo = int.Parse(elm.Element("CombinedMacNo").Value);
                        int configMacNo = int.Parse(elm.Element("CombinedMacNo").Value);
                        MachineInfo oldMac = MachineInfo.GetMachine(configMacNo, true);
                        MachineInfo newMac = MachineInfo.GetMachine(oldMac.NascaPlantCd);
                        ((ECK4MagExchanger)retv).CombinedECKMacNo = newMac.MacNo;

                        ((ECK4MagExchanger)retv).IpAddress = elm.Element("localPlcIPAddress").Value;
                        ((ECK4MagExchanger)retv).Port = int.Parse(elm.Element("localPlcPort").Value);

                        sub = elm.Element("empMagUnLoaderReqBitAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK4MagExchanger)retv).EmpMagUnloaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }

                        sub = elm.Element("empMagUnLoaderPointList").Elements("point");
                        if (sub != null)
                        {
                            foreach (XElement subelm in sub)
                            {
                                ((ECK4MagExchanger)retv).EmptyMagUnloaderPointList.Add(
                                int.Parse(subelm.Attribute("key").Value), subelm.Value);
                            }
                        }

                        sub = elm.Element("empMagLoaderReqBitAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK4MagExchanger)retv).EmpMagLoaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }

                        sub = elm.Element("empMagLoaderPointList").Elements("point");
                        if (sub != null)
                        {
                            foreach (XElement subelm in sub)
                            {
                                ((ECK4MagExchanger)retv).EmptyMagLoaderPointList.Add(
                                int.Parse(subelm.Attribute("key").Value), subelm.Value);
                            }
                        }

                        sub = elm.Element("unloaderReqBitAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK4MagExchanger)retv).UnloaderReqBitAddressList.Add(
                            int.Parse(subelm.Attribute("key").Value), subelm.Value);
                        }

                        sub = elm.Element("unloaderPointList").Elements("point");
                        if (sub != null)
                        {
                            foreach (XElement subelm in sub)
                            {
                                ((ECK4MagExchanger)retv).UnloaderPointList.Add(
                                int.Parse(subelm.Attribute("key").Value), subelm.Value);
                            }
                        }

                        sub = elm.Element("dischargeModeProcBitAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((ECK4MagExchanger)retv).ProcDischargeAddressList.Add(
                            subelm.Attribute("key").Value, subelm.Value);
                        }

                        break;
#endregion

                    case MachineType.ManualECK:
#region

                        retv = new ManualECK();
                        ((ManualECK)retv).StartReqBitAddress = elm.Element("startReqBitAddress").Value;
                        ((ManualECK)retv).ProgramNoAddress = elm.Element("programNoAddress").Value;
                        ((ManualECK)retv).ChamberList = new List<ManualECK.CPMChamber>();
                        foreach (XElement chamber in elm.Elements("chambers").Elements("chamber"))
                        {
                            int no = int.Parse(chamber.Attribute("no").Value);
                            ManualECK.CPMChamber c = new ManualECK.CPMChamber(no);
                            c.StartOKBitAddress = chamber.Element("startOKBitAddress").Value;
                            c.StartNGBitAddress = chamber.Element("startNGBitAddress").Value;
                            c.CompleteOKBitAddress = chamber.Element("completeOKBitAddress").Value;
                            c.CompleteNGBitAddress = chamber.Element("completeNGBitAddress").Value;
                            c.MagazineAddress = chamber.Element("magazineAddress").Value;
                            c.ReqBitAddress = chamber.Element("reqBitAddress").Value;
                            c.LargeSizeLotBitAddress = chamber.Element("largeSizeLotBitAddress").Value;
                            c.MediumSizeLotBitAddress = chamber.Element("mediumSizeLotBitAddress").Value;

                            ((ManualECK)retv).ChamberList.Add(c);
                        }
#endregion
                        break;
                    case MachineType.MoldConveyor:
#region

                        retv = new MoldConveyor();
                        ((MoldConveyor)retv).MagazineArriveBitAddress = elm.Element("magazineArriveBitAddress").Value;
                        ((MoldConveyor)retv).MissingReservedMagazineBitAddress = elm.Element("missingReservedMagazineBitAddress").Value;
                        ((MoldConveyor)retv).OutputReserveBitAddress = elm.Element("outputReserveBitAddress").Value;
#endregion
                        break;

                    case MachineType.MagExchanger:
                        retv = new MagExchanger();
                        ((MagExchanger)retv).MagShiftEnableBitAddress = elm.Element("magShiftEnableBitAddress").Value;
                        ((MagExchanger)retv).UnloaderMagReverseBitAddress = elm.Element("unloaderMagReverseBitAddress").Value;

                        sub = elm.Element("dischargeModeProcBitAddressList").Elements("address");
                        foreach (XElement subelm in sub)
                        {
                            ((MagExchanger)retv).ProcDischargeAddressList.Add(
                            subelm.Attribute("key").Value, subelm.Value);
                        }
                        break;

                    case MachineType.GeneralBuffer:
                        retv = new GeneralBuffer();
                        break;

                    case MachineType.ManualOven:
                        retv = new ManualOven();
                        break;

                    case MachineType.ManualMultiLoadPlasma:
                        retv = new ManualMultiLoadPlasma();

						elmHostIpAddress = elm.Element("hostIpAddress");
						if (elmHostIpAddress != null)
						{
							hostIpAddress = elmHostIpAddress.Value;
						}

                        break;

					case MachineType.Sputter:
						retv = new Sputter();
						((Sputter)retv).LoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress").Value;
						((Sputter)retv).LoaderMagazineAddress = elm.Element("loaderMagazineAddress").Value;
						((Sputter)retv).LoaderMachineSelectCompleteBitAddress = elm.Element("loaderMachineSelectCompleteBitAddress").Value;
						((Sputter)retv).LoaderMachineNoAddress = elm.Element("loaderMachineNoAddress").Value;
						((Sputter)retv).LoaderWorkCompleteBitAddress = elm.Element("loaderWorkCompleteBitAddress").Value;
						((Sputter)retv).LoaderWorkErrorBitAddress = elm.Element("loaderWorkErrorBitAddress").Value;
						((Sputter)retv).UnLoaderQRReadCompleteBitAddress = elm.Element("unloaderQRReadCompleteBitAddress").Value;
						((Sputter)retv).UnLoaderMagazineAddress = elm.Element("unloaderMagazineAddress").Value;
						((Sputter)retv).UnLoaderWorkCompleteBitAddress = elm.Element("unloaderWorkCompleteBitAddress").Value;
						((Sputter)retv).UnLoaderWorkErrorBitAddress = elm.Element("unloaderWorkErrorBitAddress").Value;
						((Sputter)retv).UnLoaderMachineNoAddress = elm.Element("unloaderMachineNoAddress").Value;
						((Sputter)retv).HeartBeatAddress = elm.Element("heartBeatAddress").Value;
                        ((Sputter)retv).LoaderTrayAddress = elm.Element("loaderTrayAddress").Value;
                        ((Sputter)retv).LoaderTrayOrderAddress = elm.Element("loaderTrayOrderAddress").Value;
                        ((Sputter)retv).UnLoaderTrayAddress = elm.Element("unloaderTrayAddress").Value;
                        ((Sputter)retv).UnLoaderTrayOrderAddress = elm.Element("unloaderTrayOrderAddress").Value;
                        ((Sputter)retv).RelateLotTrayFunctionBitAddress = elm.Element("relateLotTrayFunctionBitAddress").Value;

                        break;

                    case MachineType.GeneralLotMarking:
                        retv = new GeneralLotMarking();
                        break;

                    case MachineType.WetBlaster2:
#region
                        retv = new WetBlaster2();

                        ((WetBlaster2)retv).LoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress").Value;
                        ((WetBlaster2)retv).WorkStartLoaderQRAddress = elm.Element("workStartLoaderQRAddress").Value;
                        ((WetBlaster2)retv).WorkCompleteLoaderQRAddress = elm.Element("workCompleteLoaderQRAddress").Value;
                        ((WetBlaster2)retv).WorkCompleteUnloaderQRAddress = elm.Element("workCompleteUnloaderQRAddress").Value;
                        ((WetBlaster2)retv).WorkStartOKBitAddress = elm.Element("workStartOKBitAddress").Value;
                        ((WetBlaster2)retv).WorkStartNGBitAddress = elm.Element("workStartNGBitAddress").Value;
                        ((WetBlaster2)retv).SendCompleteBitAddress = elm.Element("sendCompleteBitAddress").Value;
                        ((WetBlaster2)retv).StartTeachProgramWordAddress = elm.Element("startTeachProguramWordAddress").Value;
                        ((WetBlaster2)retv).HostTeachProgramReqBitAddress = elm.Element("hostTeachProgramReqBitAddress").Value;
                        ((WetBlaster2)retv).HostTeachProgramResultWordAddress = elm.Element("hostTeachProgramResultWordAddress").Value;
                        // 本体通信用
                        XElement hostPlcMakerElm = elm.Element("hostPlcMaker");
                        XElement hostPlcAddressElm = elm.Element("hostPlcAddress");
                        XElement hostPlcPortElm = elm.Element("hostPlcPort");
                        XElement hostPlcSocketElm = elm.Element("hostPlcSocket");
                        ((WetBlaster2)retv).HostPlc = PLC.Common.GetInstance(hostPlcMakerElm.Value, hostPlcAddressElm.Value, int.Parse(hostPlcPortElm.Value), hostIpAddress, hostPlcSocketElm.Value);
#endregion
                        break;

                    #region 車載

                    case MachineType.InspectorMagRotation:
#region
                        retv = new InspectorMagRotation();
                        ((InspectorMagRotation)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
                        ((InspectorMagRotation)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
                        ((InspectorMagRotation)retv).OpeningCheckMagazineInputWordAddress = elm.Element("openingCheckMagazineInputWordAddress").Value;
                        ((InspectorMagRotation)retv).OpeningCheckMagazineOutputWordAddress = elm.Element("openingCheckMagazineOutputWordAddress").Value;
                        elmMMFilePath = elm.Element("mmFilePath");
                        if (elmMMFilePath != null)
                        {
							((InspectorMagRotation)retv).MMFilePath = elmMMFilePath.Value;
                        }

						//以下は高効率で使用
						elmUnloaderReqBitAddressStart = elm.Element("unloaderReqBitAddressStart");
						if (elmUnloaderReqBitAddressStart != null)
						{
							((InspectorMagRotation)retv).UnloaderReqBitAddressStart = elmUnloaderReqBitAddressStart.Value;
						}
						elmUnloaderMagazineDataAddressStart = elm.Element("unloaderMagazineDataAddressStart");
						if (elmUnloaderMagazineDataAddressStart != null)
						{
							((InspectorMagRotation)retv).UnloaderMagazineDataAddressStart = elmUnloaderMagazineDataAddressStart.Value;
						}

                        ((InspectorMagRotation)retv).LogOutputDirectoryPath = elm.Element("logOutputDirectoryPath").Value;

                        XElement elmOutputReserveBitAddress = elm.Element("outputReserveBitAddress");
                        if (elmOutputReserveBitAddress != null)
                        {
                            ((InspectorMagRotation)retv).OutputReserveBitAddress = elmOutputReserveBitAddress.Value;
                        }

                        XElement elmWorkCompleteNGBitAddress = elm.Element("workCompleteNGBitAddress");
                        if (elmWorkCompleteNGBitAddress != null)
                        {
                            ((InspectorMagRotation)retv).WorkCompleteNGBitAddress = elmWorkCompleteNGBitAddress.Value;
                        }

                        elmLoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress");
                        if (elmLoaderQRReadCompleteBitAddress != null)
                        {
                            ((InspectorMagRotation)retv).LoaderQRReadCompleteBitAddress = elmLoaderQRReadCompleteBitAddress.Value;
                        }

                        XElement elmWorkStartOKBitAddress = elm.Element("workStartOKBitAddress");
                        if (elmWorkStartOKBitAddress != null)
                        {
                            ((InspectorMagRotation)retv).WorkStartOKBitAddress = elmWorkStartOKBitAddress.Value;
                        }

                        XElement elmWorkStartNGBitAddress = elm.Element("workStartNGBitAddress");
                        if (elmWorkStartNGBitAddress != null)
                        {
                            ((InspectorMagRotation)retv).WorkStartNGBitAddress = elmWorkStartNGBitAddress.Value;
                        }

                        XElement elmIsWorkStartAutoComplete = elm.Element("isWorkStartAutoComplete");
                        if (elmIsWorkStartAutoComplete != null)
                        {
                            ((InspectorMagRotation)retv).IsWorkStartAutoComplete = bool.Parse(elmIsWorkStartAutoComplete.Value);
                        }
                        XElement elmIsWorkEndAutoComplete = elm.Element("isWorkEndAutoComplete");
                        if (elmIsWorkEndAutoComplete != null)
                        {
                            ((InspectorMagRotation)retv).IsWorkEndAutoComplete = bool.Parse(elmIsWorkEndAutoComplete.Value);
                        }
                                                
#endregion
                        break;
                    case MachineType.LotMarking:
#region 
                        retv = new LotMarking();
                        ((LotMarking)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
                        ((LotMarking)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
						((LotMarking)retv).HandshakeDirPath = elm.Element("handshakeDirPath").Value;

						elmHostIpAddress = elm.Element("hostIpAddress");
						if (elmHostIpAddress != null)
						{
							hostIpAddress = elmHostIpAddress.Value;
						}

						sub = elm.Element("unloaderMagazineList").Elements("magazine");
						foreach(XElement s in sub)
						{
							LotMarking.UnloaderMagazine mag = new LotMarking.UnloaderMagazine();
							mag.ReqBitAddress = s.Element("reqBitAddress").Value;
							mag.MagazineNoAddress = s.Element("magazineNoAddress").Value;
							mag.WorkStartAddress = s.Element("workStartAddress").Value;
							mag.WorkEndAddress = s.Element("workEndAddress").Value;
							((LotMarking)retv).UnloaderMagazineList.Add(mag);
						}

						XElement elmWorkCompleteOKBitAddress = elm.Element("workCompleteOKBitAddress");
						if (elmWorkCompleteOKBitAddress != null)
						{
							((LotMarking)retv).WorkCompleteOKBitAddress = elmWorkCompleteOKBitAddress.Value;
						}

						elmLoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress");
						if (elmLoaderQRReadCompleteBitAddress != null)
						{
							((LotMarking)retv).LoaderQRReadCompleteBitAddress = elmLoaderQRReadCompleteBitAddress.Value;
						}

						((LotMarking)retv).IsWorkStartAutoComplete = bool.Parse(elm.Element("isWorkStartAutoComplete").Value);

						((LotMarking)retv).WorkStartNGBitAddress = elm.Element("workStartNGBitAddress").Value;

#endregion
                        break;

#endregion

#region 19

					case MachineType.FlipChip:
						retv = new FlipChip();
						sub = elm.Element("unloaderReqBitAddressList").Elements("bitAddress");
						foreach (XElement s in sub)
						{
							((FlipChip)retv).UnLoaderReqBitAddressList.Add(s.Value);
						}

						break;

					case MachineType.BumpBonder:
						retv = new BumpBonder();

						break;

					case MachineType.FullAutoCompacting:
						retv = new FullAutoCompacting();
						sub = elm.Element("unloaderReqBitAddressList").Elements("bitAddress");
						foreach (XElement s in sub)
						{
							((FullAutoCompacting)retv).UnLoaderReqBitAddressList.Add(s.Value);
						}

						break;

					case MachineType.SemiAutoCompacting:
						retv = new SemiAutoCompacting();

						break;
						
					case MachineType.ElectroCoat:
						retv = new ElectroCoat();
						((ElectroCoat)retv).TankAvailableBitAddress = elm.Element("tankAvailableBitAddress").Value;

						break;

					case MachineType.WeightMeasurement:
						retv = new WeightMeasurement();
						sub = elm.Element("unloaderReqBitAddressList").Elements("bitAddress");
						foreach (XElement s in sub)
						{
							((WeightMeasurement)retv).UnLoaderReqBitAddressList.Add(s.Value);
						}

						break;

					case MachineType.Mold4:
						retv = new Mold4();

						sub = elm.Element("unloaderReqBitAddressList").Elements("bitAddress");
						foreach (XElement s in sub)
						{
							((Mold4)retv).UnLoaderReqBitAddressList.Add(s.Value);
						}

						break;

					case MachineType.BackSideWashing:
						retv = new BackSideWashing();
						((BackSideWashing)retv).OutputReserveBitAddress = elm.Element("outputReserveBitAddress").Value;
						((BackSideWashing)retv).WorkCompleteNGBitAddress = elm.Element("workCompleteNGBitAddress").Value;


						break;
					case MachineType.LaserScribe:
						retv = new LaserScribe();

						((LaserScribe)retv).OutputReserveBitAddress = elm.Element("outputReserveBitAddress").Value;
						((LaserScribe)retv).WorkCompleteNGBitAddress = elm.Element("workCompleteNGBitAddress").Value;


						break;

					case MachineType.Break:
						retv = new Break();


						break;

					case MachineType.ColorRevision:
						retv = new ColorRevision();

						((ColorRevision)retv).LoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress").Value;
						((ColorRevision)retv).LoaderMagazineAddress = elm.Element("loaderMagazineAddress").Value;
						((ColorRevision)retv).UnLoaderMagazineAddress = elm.Element("unLoaderMagazineAddress").Value;

						((ColorRevision)retv).WorkStartOKBitAddress = elm.Element("workStartOKBitAddress").Value;
						((ColorRevision)retv).WorkStartNGBitAddress = elm.Element("workStartNGBitAddress").Value;

						((ColorRevision)retv).WorkCompleteOKBitAddress = elm.Element("workCompleteOKBitAddress").Value;
						((ColorRevision)retv).WorkCompleteNGBitAddress = elm.Element("workCompleteNGBitAddress").Value;

						((ColorRevision)retv).WorkCompleteNGReasonAddress = elm.Element("workCompleteNGReasonAddress").Value;

						break;
					case MachineType.Dicing:
						retv = new Dicing();

						break;
#endregion

#region MPL/83/385/COB

					case MachineType.DieBonder3:
						retv = new DieBonder3();

						sub = elm.Element("unloaderReqBitAddressList").Elements("bitAddress");
						foreach (XElement s in sub)
						{
							((DieBonder3)retv).UnLoaderReqBitAddressList.Add(s.Value);
						}

						break;

					case MachineType.DieBonder4:
#region

                        retv = new DieBonder4();
                        ((DieBonder4)retv).WaferBitAddressStart = elm.Element("waferBitAddressStart").Value;
                        ((DieBonder4)retv).StartWorkTableSensorAddress = elm.Element("startWorkTableSensorAddress").Value;
                        ((DieBonder4)retv).WaferChangerChangeSensorBitAddress = elm.Element("waferChangerChangeBitAddress").Value;

                        XElement elmIsUniqueDefectImport = elm.Element("isUniqueDefectImport");
                        if (elmIsUniqueDefectImport != null)
                        {
                            ((DieBonder4)retv).IsUniqueDefectImport = bool.Parse(elmIsUniqueDefectImport.Value);
                        }

#endregion

						break;

					case MachineType.DieBonder5:
#region

						retv = new DieBonder5();
						((DieBonder5)retv).StartWorkTableSensorAddress = elm.Element("startWorkTableSensorAddress").Value;

                        elmIsUniqueDefectImport = elm.Element("isUniqueDefectImport");
                        if (elmIsUniqueDefectImport != null)
                        {
                            ((DieBonder5)retv).IsUniqueDefectImport = bool.Parse(elmIsUniqueDefectImport.Value);
                        }

#endregion

                        break;

					case MachineType.ReflectorPotting:
#region 
						retv = new ReflectorPotting();
						((ReflectorPotting)retv).WorkCompleteOKBitAddress = elm.Element("workCompleteOKBitAddress").Value;
						((ReflectorPotting)retv).WorkCompleteNGBitAddress = elm.Element("workCompleteNGBitAddress").Value;

#endregion
						break;

					case MachineType.ReflectorPotting2:
#region
						retv = new ReflectorPotting2();
						sub = elm.Element("unloaderReqBitAddressList").Elements("bitAddress");
						foreach (XElement s in sub)
						{
							((ReflectorPotting2)retv).UnLoaderReqBitAddressList.Add(s.Value);
						}

#endregion
						break;

					case MachineType.Mold5:
#region 
						retv = new Mold5();

						elmHostIpAddress = elm.Element("hostIpAddress");
						if (elmHostIpAddress != null)
						{
							hostIpAddress = elmHostIpAddress.Value;
						}

						((Mold5)retv).WorkCompleteOKBitAddress = elm.Element("workCompleteOKBitAddress").Value;
						((Mold5)retv).WorkCompleteNGBitAddress = elm.Element("workCompleteNGBitAddress").Value;

#endregion
						break;

					case MachineType.LotMarking2:
						retv = new LotMarking2();

						elmHostIpAddress = elm.Element("hostIpAddress");
						if (elmHostIpAddress != null)
						{
							hostIpAddress = elmHostIpAddress.Value;
						}
						((LotMarking2)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
						((LotMarking2)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;

						((LotMarking2)retv).LoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress").Value;

						((LotMarking2)retv).WorkStartOKBitAddress = elm.Element("workStartOKBitAddress").Value;
						((LotMarking2)retv).WorkStartNGBitAddress = elm.Element("workStartNGBitAddress").Value;

						((LotMarking2)retv).WorkCompleteOKBitAddress = elm.Element("workCompleteOKBitAddress").Value;
						((LotMarking2)retv).WorkCompleteNGBitAddress = elm.Element("workCompleteNGBitAddress").Value;

						((LotMarking2)retv).IsWorkStartAutoComplete = bool.Parse(elm.Element("isWorkStartAutoComplete").Value);

						break;

					case MachineType.LotMarking3:
						retv = new LotMarking3();

						break;

					case MachineType.Mold6:
#region
						retv = new Mold6();
						sub = elm.Element("unloaderReqBitAddressList").Elements("bitAddress");
						foreach (XElement s in sub)
						{
							((Mold6)retv).UnLoaderReqBitAddressList.Add(s.Value);
						}

#endregion
						break;
#endregion


#region SIGMA

                    case MachineType.SMTLoader:
                        retv = new SMTLoader();
                        elmHostIpAddress = elm.Element("hostIpAddress");
                        if (elmHostIpAddress != null)
                        {
                            hostIpAddress = elmHostIpAddress.Value;
                        }
                        break;

                    case MachineType.SMTUnloader:
                        retv = new SMTUnloader();
                        elmHostIpAddress = elm.Element("hostIpAddress");
                        if (elmHostIpAddress != null)
                        {
                            hostIpAddress = elmHostIpAddress.Value;
                        }
                        break;

                    case MachineType.Inspector4:
                        retv = new Inspector4();

                        break;

                    case MachineType.DieBonder7:
                        retv = new DieBonder7();

                        ((DieBonder7)retv).WaferBitAddressStart = elm.Element("waferBitAddressStart").Value;
                        ((DieBonder7)retv).WaferChangerChangeSensorBitAddress = elm.Element("waferChangerChangeBitAddress").Value;

                        magazineTakeoutElm = elm.Element("magazineTakeoutBitAddress");
                        if (magazineTakeoutElm != null)
                        {
                            ((DieBonder7)retv).MagazineTakeoutBitAddress = magazineTakeoutElm.Value;
                        }

                        unloaderMoveCompleteDBElm = elm.Element("unloaderMoveCompleteBitAddress");
                        if (unloaderMoveCompleteDBElm != null)
                        {
                            ((DieBonder7)retv).UnloaderMoveCompleteBitAddress = unloaderMoveCompleteDBElm.Value;
                        }
                        break;

                    case MachineType.Mounter:
                        retv = new Mounter();

                        ((Mounter)retv).CsvInputDirectoryPath = elm.Element("csvOutputDirectoryPath").Value;

                        break;

                    case MachineType.Masking:
                        retv = new Masking();

                        break;

                    case MachineType.SubstrateLoader:
                        retv = new SubstrateLoader();

                        elmHostIpAddress = elm.Element("hostIpAddress");
                        if (elmHostIpAddress != null)
                        {
                            hostIpAddress = elmHostIpAddress.Value;
                        }
                        break;

                    case MachineType.LensMounting:
                        retv = new LensMounting();

                        break;

                    case MachineType.Inspector5:
                        retv = new Inspector5();

                        break;

                    case MachineType.Dicing2:
                        retv = new Dicing2();
                        break;

                    case MachineType.TapePeeler:
                        retv = new TapePeeler();

                        elmHostIpAddress = elm.Element("hostIpAddress");
                        if (elmHostIpAddress != null)
                        {
                            hostIpAddress = elmHostIpAddress.Value;
                        }
                        break;

                    case MachineType.WireBonder2:
                        retv = new WireBonder2();

                        break;

#endregion


#region NTSV
                    case MachineType.DieBonder6:
                        retv = new DieBonder6();

                        ((DieBonder6)retv).WaferBitAddressStart = elm.Element("waferBitAddressStart").Value;
                        ((DieBonder6)retv).WaferChangerChangeSensorBitAddress = elm.Element("waferChangerChangeBitAddress").Value;

                        magazineTakeoutElm = elm.Element("magazineTakeoutBitAddress");
                        if (magazineTakeoutElm != null)
                        {
                            ((DieBonder6)retv).MagazineTakeoutBitAddress = magazineTakeoutElm.Value;
                        }

                        unloaderMoveCompleteDBElm = elm.Element("unloaderMoveCompleteBitAddress");
                        if (unloaderMoveCompleteDBElm != null)
                        {
                            ((DieBonder6)retv).UnloaderMoveCompleteBitAddress = unloaderMoveCompleteDBElm.Value;
                        }

                        XElement startWorkTableSensorElm = elm.Element("startWorkTableSensorAddress");
                        if (startWorkTableSensorElm != null)
                        {
                            ((DieBonder6)retv).StartWorkTableSensorAddress = startWorkTableSensorElm.Value;
                        }
                        secondLoaderReqElm = elm.Element("secondLoaderReqBitAddress");
                        if (secondLoaderReqElm != null)
                        {
                            ((DieBonder6)retv).SecondLoaderReqBitAddress = secondLoaderReqElm.Value;
                        }
                        secondEmpMagLoaderReqElm = elm.Element("secondEmpMagLoaderReqBitAddress");
                        if (secondEmpMagLoaderReqElm != null)
                        {
                            ((DieBonder6)retv).SecondEmpMagLoaderReqBitAddress = secondEmpMagLoaderReqElm.Value;
                        }
                        break;

                    case MachineType.SQMachine:
                        retv = new SQMachine();
                        elmHostIpAddress = elm.Element("hostIpAddress");
                        if (elmHostIpAddress != null)
                        {
                            hostIpAddress = elmHostIpAddress.Value;
                        }

                        break;

                    case MachineType.Grinder:
                        retv = new Grinder();
                        elmHostIpAddress = elm.Element("hostIpAddress");

                        ((Grinder)retv).LoaderQRReadCompleteBitAddress = elm.Element("LoaderQRReadCompleteBitAddress").Value;
                        ((Grinder)retv).LoaderRingLabelAddress = elm.Element("LoaderRingLabelAddress").Value;
                        ((Grinder)retv).NotifyRingLabelNGAddress = elm.Element("NotifyRingLabelNGAddress").Value;
                        ((Grinder)retv).UnLoaderQRReadCompleteBitAddress = elm.Element("UnLoaderQRReadCompleteBitAddress").Value;
                                                
                        //if (elmHostIpAddress != null)
                        //{
                        //    hostIpAddress = elmHostIpAddress.Value;
                        //}
                        break;

                    case MachineType.Grinder2:
                        retv = new Grinder2();
                        elmHostIpAddress = elm.Element("hostIpAddress");

                        ((Grinder)retv).LoaderQRReadCompleteBitAddress = elm.Element("LoaderQRReadCompleteBitAddress").Value;
                        ((Grinder)retv).LoaderRingLabelAddress = elm.Element("LoaderRingLabelAddress").Value;
                        ((Grinder)retv).NotifyRingLabelNGAddress = elm.Element("NotifyRingLabelNGAddress").Value;
                        ((Grinder)retv).UnLoaderQRReadCompleteBitAddress = elm.Element("UnLoaderQRReadCompleteBitAddress").Value;

                        break;

                    case MachineType.HotPress:
						retv = new HotPress();
						elmHostIpAddress = elm.Element("hostIpAddress");
						if (elmHostIpAddress != null)
						{
							hostIpAddress = elmHostIpAddress.Value;
						}
						break;

                    case MachineType.DryIceCleaning:
                        retv = new DryIceCleaning();

                        break;

                    case MachineType.Deburring:
                        retv = new Deburring();

                        break;

                    case MachineType.FrameLoader5:
                        retv = new FrameLoader5();
                        break;
                        
                    case MachineType.ColorAutoMeasurer:
                        retv = new ColorAutoMeasurer();
                        break;

                    case MachineType.AutoPaster:
                        retv = new AutoPaster();
                        break;

                    case MachineType.Inspector6:
                        retv = new Inspector6();
                        ((Inspector6)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
                        ((Inspector6)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
                        ((Inspector6)retv).OutputReserveBitAddress = elm.Element("outputReserveBitAddress").Value;
                        ((Inspector6)retv).LoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress").Value;
                        ((Inspector6)retv).WorkStartOKBitAddress = elm.Element("workStartOKBitAddress").Value;
                        ((Inspector6)retv).WorkStartNGBitAddress = elm.Element("workStartNGBitAddress").Value;
                        ((Inspector6)retv).WorkCompleteNGBitAddress = elm.Element("workCompleteNGBitAddress").Value;
                        ((Inspector6)retv).IsWorkStartAutoComplete = bool.Parse(elm.Element("isWorkStartAutoComplete").Value);
                        ((Inspector6)retv).IsWorkEndAutoComplete = bool.Parse(elm.Element("isWorkEndAutoComplete").Value);
                        ((Inspector6)retv).WorkStartNGKindAddress = elm.Element("workStartNGKindAddress").Value;
                        ((Inspector6)retv).WorkCompleteNGKindAddress = elm.Element("workCompleteNGKindAddress").Value;
                        ((Inspector6)retv).WorkCompleteOKBitAddress = elm.Element("workCompleteOKBitAddress").Value;
                        break;
                    #endregion

                    #region DMC
                    case MachineType.IPS://外製
                        retv = new IPS();
                        break;

                    case MachineType.IPS2://内製
                        retv = new IPS2();
                        break;
                    case MachineType.LotMarking4://内製
                        retv = new LotMarking4();

                        XElement tempNotRename = elm.Element("NotRenameTrgFile");
                        if (tempNotRename != null)
                        {
                            ((LotMarking4)retv).NotRenameTrgFile = bool.Parse(tempNotRename.Value);
                        }
                        else
                        {
                            ((LotMarking4)retv).NotRenameTrgFile = false;
                        }
                        
                        XElement inOutMagAddrIsSameFg = elm.Element("InOutMagAddrIsSame");
                        if (inOutMagAddrIsSameFg != null)
                        {
                            ((LotMarking4)retv).inOutMagAddrIsSameFg = bool.Parse(inOutMagAddrIsSameFg.Value);
                        }
                        else
                        {
                            ((LotMarking4)retv).inOutMagAddrIsSameFg = false;
                        }

                        XElement isJudgeDoWork = elm.Element("IsJudgeDoWork");
                        if (isJudgeDoWork != null)
                        {
                            ((LotMarking4)retv).isJudgeDoWork = bool.Parse(isJudgeDoWork.Value);
                        }
                        else
                        {
                            ((LotMarking4)retv).isJudgeDoWork = false;
                        }

                        break;
#endregion

                    case MachineType.ManualComplete:
                        retv = new ManualComplete();

                        break;

					case MachineType.YagGlassBonder:
						retv = new YagGlassBonder();

						sub = elm.Element("unloaderReqBitAddressList").Elements("bitAddress");
						foreach (XElement s in sub)
						{
							((YagGlassBonder)retv).UnLoaderReqBitAddressList.Add(s.Value);
						}

						int checkRequireOutputRetryCt = Convert.ToInt32(elm.Element("checkRequireOutputRetryCt").Value);

						if (checkRequireOutputRetryCt <= 0)
						{
							checkRequireOutputRetryCt = 1;
						}

						((YagGlassBonder)retv).CheckRequireOutputRetryCt = checkRequireOutputRetryCt;

						break;

					case MachineType.Mold7:
						retv = new Mold7();

						((Mold7)retv).LMagazineAddress = elm.Element("lMagazineAddress").Value;
						((Mold7)retv).ULMagazineAddress = elm.Element("ulMagazineAddress").Value;
						((Mold7)retv).EnableWorkRecordAutoRegisterBitAddress = elm.Element("enableWorkRecordAutoRegisterBitAddress").Value;
						((Mold7)retv).WorkStartOKBitAddress = elm.Element("workStartOKBitAddress").Value;
						((Mold7)retv).WorkStartNGBitAddress = elm.Element("workStartNGBitAddress").Value;
						((Mold7)retv).WorkEndOKBitAddress = elm.Element("workEndOKBitAddress").Value;
						((Mold7)retv).WorkEndNGBitAddress = elm.Element("workEndNGBitAddress").Value;
						((Mold7)retv).StartAutoRegisterNGKindAddress = elm.Element("startAutoRegisterNGKindAddress").Value;
						((Mold7)retv).EndAutoRegisterNGKindAddress = elm.Element("endAutoRegisterNGKindAddress").Value;
						((Mold7)retv).WorkStartTrigAddress = elm.Element("workStartTrigAddress").Value;
						((Mold7)retv).WorkEndTrigAddress = elm.Element("workEndTrigAddress").Value;

						elmIsQRReader = elm.Element("isQRReader");
						if (elmIsQRReader != null)
						{
							((Mold)retv).IsQRReader = Convert.ToBoolean(elmIsQRReader.Value);
						}
						//((Mold7)retv).IsWorkStartAutoComplete = bool.Parse(elm.Element("isWorkStartAutoComplete").Value);
						//((Mold7)retv).IsWorkEndAutoComplete = bool.Parse(elm.Element("isWorkEndAutoComplete").Value);

						break;

					case MachineType.WetBlaster:
						retv = new WetBlaster();

						((WetBlaster)retv).LoaderQRReadCompleteBitAddress = elm.Element("LoaderQRReadCompleteBitAddress").Value;
						((WetBlaster)retv).LoaderQRAddress = elm.Element("LoaderQRAddress").Value;
						((WetBlaster)retv).UnloaderQRAddress = elm.Element("UnloaderQRAddress").Value;
						((WetBlaster)retv).WorkStartOKBitAddress = elm.Element("WorkStartOKBitAddress").Value;
						((WetBlaster)retv).WorkStartNGBitAddress = elm.Element("WorkStartNGBitAddress").Value;
						((WetBlaster)retv).RecvCompleteBitAddress = elm.Element("RecvCompleteBitAddress").Value;
						((WetBlaster)retv).WorkCompleteDtStartAddress = elm.Element("WorkCompleteDtStartAddress").Value;
						((WetBlaster)retv).UnloaderDmStartAddress = elm.Element("UnloaderDmStartAddress").Value;
						((WetBlaster)retv).WorkCompleteDtAddressInterval = Convert.ToInt32(elm.Element("WorkCompleteDtAddressInterval").Value);
						((WetBlaster)retv).DmAddressInterval = Convert.ToInt32(elm.Element("DmAddressInterval").Value);

						break;
                    case MachineType.Formatting:
                        retv = new Formatting();

                        break;

                    case MachineType.Sputter2:
                        retv = new Sputter2();

                        ((Sputter2)retv).LoaderQRReadCompleteBitAddress = elm.Element("loaderQRReadCompleteBitAddress").Value;
                        ((Sputter2)retv).LoaderMagazineAddress = elm.Element("loaderMagazineAddress").Value;
                        ((Sputter2)retv).UnLoaderMagazineAddress = elm.Element("unloaderMagazineAddress").Value;
                        ((Sputter2)retv).workStartJudgeAddress = elm.Element("workStartJudgeAddress").Value;

                        break;

                    case MachineType.FormicAcidReflow:
                        retv = new FormicAcidReflow();
                        break;


                    case MachineType.FormicAcidReflow2:
                        retv = new FormicAcidReflow2();
                        break;

                    case MachineType.SheetRingLoader:
                        retv = new SheetRingLoader();
                        break;

                    case MachineType.WaterCleaning:
                        retv = new WaterCleaning();
                        break;

                    case MachineType.ResinMeasurement:
                        retv = new ResinMeasurement();
                        break;
                }

                if (retv == null)
                {
                    return null;
                }
                retv.MacNo = int.Parse(elm.Element("machineNo").Value);
                // データベースに吸い上げ
                //retv.LineNo = elm.Element("lineNo").Value;
                retv.Name = elm.Element("machineName").Value;

                XElement priorityElm = elm.Element("priority");
                if (priorityElm != null && !string.IsNullOrEmpty(priorityElm.Value))
                {
                    retv.Priority = int.Parse(priorityElm.Value);
                }

				XElement plcMakerElm = elm.Element("plcMaker");
				XElement plcAddressElm = elm.Element("plcAddress");
				XElement plcPortElm = elm.Element("plcPort");

				string plcSocket = string.Empty;
				XElement plcSocketElm = elm.Element("plcSocket");
				if (plcSocketElm != null)
				{
					plcSocket = plcSocketElm.Value;
				}
				else 
				{
					plcSocket = Socket.Udp.ToString();
				}

				if (plcMakerElm != null && plcAddressElm != null && plcPortElm != null)
				{
					if (!string.IsNullOrEmpty(plcMakerElm.Value) && !string.IsNullOrEmpty(plcAddressElm.Value) && !string.IsNullOrEmpty(plcPortElm.Value))
					{
						retv.Plc = PLC.Common.GetInstance(plcMakerElm.Value, plcAddressElm.Value, int.Parse(plcPortElm.Value), hostIpAddress, plcSocket);
					}
				}

                //PLC信号アドレス
                retv.MachineReadyBitAddress = elm.Element("machineReadyBitAddress").Value;
                retv.LoaderReqBitAddress = elm.Element("loaderReqBitAddress").Value;
                retv.UnLoaderReqBitAddress = elm.Element("unloaderReqBitAddress").Value;
                retv.EmpMagLoaderReqBitAddress = elm.Element("empMagLoaderReqBitAddress").Value;
                retv.EmpMagUnLoaderReqBitAddress = elm.Element("empMagUnloaderReqBitAddress").Value;
                retv.WorkStartTimeAddress = elm.Element("workStartTimeAddress").Value;
                retv.WorkCompleteTimeAddress = elm.Element("workCompleteTimeAddress").Value;
                retv.ClearMagazineBitAddress = elm.Element("clearMagazinesBitAddress").Value;
                retv.InputForbiddenBitAddress = elm.Element("inputForbiddenBitAddress").Value;
                retv.DischargeModeBitAddress = elm.Element("dischargeModeBitAddress").Value;
                retv.PrioritySettingAddress = elm.Element("prioritySettingAddress").Value;

                //投入前排出
                XElement preInputDischargeModePlcAddressElm = elm.Element("preInputDischargeModePlcAddress");
                if (preInputDischargeModePlcAddressElm != null)
                {
                    retv.PreInputDischargeModePlcAddress = preInputDischargeModePlcAddressElm.Value;
                }
                XElement preInputDischargeModePlcPortElm = elm.Element("preInputDischargeModePlcPort");
                if (preInputDischargeModePlcPortElm != null)
                {
                    retv.PreInputDischargeModePlcPort = preInputDischargeModePlcPortElm.Value;
                }
                XElement preInputDischargeModeBitAddressElm = elm.Element("preInputDischargeModeBitAddress");
                if (preInputDischargeModeBitAddressElm != null)
                {
                    retv.PreInputDischargeModeBitAddress = preInputDischargeModeBitAddressElm.Value;
                }
				
                //FromToアドレス
                retv.LoaderPoint = elm.Element("loaderPoint").Value;
                retv.UnloaderPoint = elm.Element("unloaderPoint").Value;
                retv.EmptyMagLoaderPoint = elm.Element("empMagLoaderPoint").Value;
                retv.EmptyMagUnloaderPoint = elm.Element("empMagUnloaderPoint").Value;

                XElement macPoint = elm.Element("macPoint");
                if (macPoint != null)
                {
                    retv.MacPoint = macPoint.Value;
                }

#region 装置隣接CV(棚)

                XElement elmList = elm.Element("loaderConveyorReqBitAddressList");
                if (elmList != null)
                {
                    var elms = elmList.Elements("loaderConveyorReqBitAddress");
                    foreach (XElement e in elms)
                    {
                        retv.LoaderConveyorReqBitAddressList.Add(e.Value);
                    }
                }

                elmList = elm.Element("unloaderConveyorReqBitAddressList");
                if (elmList != null)
                {
                    var elms = elmList.Elements("unloaderConveyorReqBitAddress");
                    foreach (XElement e in elms)
                    {
                        retv.UnloaderConveyorReqBitAddressList.Add(e.Value);
                    }
                }

                XElement isConveyorElm = elm.Element("isConveyor");
                if (isConveyorElm != null)
                {
                    retv.IsConveyor = bool.Parse(isConveyorElm.Value);
                }

                elmList = elm.Element("loaderConveyorMagazneArriveAddressList");
                if (elmList != null)
                {
                    var elms = elmList.Elements("loaderConveyorMagazneArriveAddress");
                    foreach (XElement e in elms)
                    {
                        retv.LoaderConveyorMagazneArriveAddressList.Add(e.Value);
                    }
                }

                XElement loaderConveyorStopAddressElm = elm.Element("loaderConveyorStopAddress");
                if (loaderConveyorStopAddressElm != null)
                {
                    retv.LoaderConveyorStopAddress = loaderConveyorStopAddressElm.Value;
                }

                XElement isUnloaderMagazineExistsAddressElm = elm.Element("isUnloaderMagazineExistsAddress");
                if (isUnloaderMagazineExistsAddressElm != null)
                {
                    retv.IsUnloaderMagazineExistsAddress = isUnloaderMagazineExistsAddressElm.Value;
                }

#endregion


                XElement linearNoElm = elm.Element("linearNo");
                if (linearNoElm != null)
                {
                    retv.LinearNo = linearNoElm.Value;
                }

                // 連動運転中
                XElement sendMachineReadyBitAddressElm = elm.Element("sendMachineReadyBitAddress");
                if (sendMachineReadyBitAddressElm != null)
                {
                    retv.SendMachineReadyBitAddress = sendMachineReadyBitAddressElm.Value;
                }
                // ロボットチャック開閉中
                XElement doingOpenRobotArmBitAddressElm = elm.Element("doingOpenRobotArmBitAddress");
                if (doingOpenRobotArmBitAddressElm != null)
                {
                    retv.DoingOpenRobotArmBitAddress = doingOpenRobotArmBitAddressElm.Value;
                }
                XElement doingCloseRobotArmBitAddressElm = elm.Element("doingCloseRobotArmBitAddress");
                if (doingCloseRobotArmBitAddressElm != null)
                {
                    retv.DoingCloseRobotArmBitAddress = doingCloseRobotArmBitAddressElm.Value;
                }

                // Mag供給・排出中
                XElement loaderDoBitAddressElm = elm.Element("loaderDoBitAddress");
                if (loaderDoBitAddressElm != null)
                {
                    retv.LoaderDoBitAddress = loaderDoBitAddressElm.Value;
                }
                XElement unloaderDoBitAddressElm = elm.Element("unloaderDoBitAddress");
                if (unloaderDoBitAddressElm != null)
                {
                    retv.UnloaderDoBitAddress = unloaderDoBitAddressElm.Value;
                }
                XElement empMagLoaderDoBitAddressElm = elm.Element("empMagLoaderDoBitAddress");
                if (empMagLoaderDoBitAddressElm != null)
                {
                    retv.EmpMagLoaderDoBitAddress = empMagLoaderDoBitAddressElm.Value;
                }
                XElement empMagUnloaderDoBitAddressElm = elm.Element("empMagUnloaderDoBitAddress");
                if (empMagUnloaderDoBitAddressElm != null)
                {
                    retv.EmpMagUnloaderDoBitAddress = empMagUnloaderDoBitAddressElm.Value;
                }

                // ロボット⇒装置 マガジンNo転送先
                XElement sendLMagazineAddressElm = elm.Element("sendLMagazineAddress");
                if (sendLMagazineAddressElm != null)
                {
                    retv.SendLMagazineAddress = sendLMagazineAddressElm.Value;
                }
                XElement sendULMagazineAddressElm = elm.Element("sendULMagazineAddress");
                if (sendULMagazineAddressElm != null)
                {
                    retv.SendULMagazineAddress = sendULMagazineAddressElm.Value;
                }

                // Mag受渡し完了
                XElement magazineDestinationCompltBitAddressElm = elm.Element("magazineDestinationCompltBitAddress");
                if (magazineDestinationCompltBitAddressElm != null)
                {
                    retv.MagazineDestinationCompltBitAddress = magazineDestinationCompltBitAddressElm.Value;
                }

                // 扉開許可
                XElement sendDoorCanOpenAddressElm = elm.Element("sendDoorCanOpenAddress");
                if (sendDoorCanOpenAddressElm != null)
                {
                    retv.SendDoorCanOpenAddress = sendDoorCanOpenAddressElm.Value;
                }
                // 全扉閉中
                XElement ovenAllDoorCloseBitAddressElm = elm.Element("ovenAllDoorCloseBitAddress");
                if (ovenAllDoorCloseBitAddressElm != null)
                {
                    retv.OvenAllDoorCloseBitAddress = ovenAllDoorCloseBitAddressElm.Value;
                }

                // Robot3用
                XElement loaderCanBitAddressElm = elm.Element("loaderCanBitAddress");
                if (loaderCanBitAddressElm != null)
                {
                    retv.LoaderCanBitAddress = loaderCanBitAddressElm.Value;
                }
                XElement empMagLoaderCanBitAddressElm = elm.Element("empMagLoaderCanBitAddress");
                if (empMagLoaderCanBitAddressElm != null)
                {
                    retv.EmpMagLoaderCanBitAddress = empMagLoaderCanBitAddressElm.Value;
                }
                XElement unloaderCanBitAddressElm = elm.Element("unloaderCanBitAddress");
                if (unloaderCanBitAddressElm != null)
                {
                    retv.UnloaderCanBitAddress = unloaderCanBitAddressElm.Value;
                }
                XElement empMagUnloaderCanBitAddressElm = elm.Element("empMagUnloaderCanBitAddress");
                if (empMagUnloaderCanBitAddressElm != null)
                {
                    retv.EmpMagUnloaderCanBitAddress = empMagUnloaderCanBitAddressElm.Value;
                }

                // ARMSThreadObjectのThreadRoutineWorkのループ時間
                XElement threadRoutineWorkMilliSecondElm = elm.Element("threadRoutineWorkMilliSecond");
                if (threadRoutineWorkMilliSecondElm != null)
                {
                    int sec;
                    if (int.TryParse(threadRoutineWorkMilliSecondElm.Value, out sec) == true)
                    {
                        retv.ThreadRoutineWorkMilliSecond = sec;
                    }
                }

                // ロボット用PLC
                XElement carrierPlcAddressElm = elm.Element("carrierPlcAddress");
                XElement carrierPlcPortElm = elm.Element("carrierPlcPort");
                XElement carrierPlcMakerElm = elm.Element("carrierPlcMaker");
                if (carrierPlcAddressElm != null && carrierPlcPortElm != null)
                {
                    if (!string.IsNullOrEmpty(carrierPlcAddressElm.Value) && !string.IsNullOrEmpty(carrierPlcPortElm.Value))
                    {
                        string plcMaker = Maker.Mitsubishi.ToString();
                        if (carrierPlcMakerElm != null && !string.IsNullOrWhiteSpace(carrierPlcMakerElm.Value))
                        {
                            plcMaker = carrierPlcMakerElm.Value;
                        }
                        string socket = Socket.Udp.ToString();
                        if (plcMaker == Maker.Keyence.ToString())
                        {
                            socket = Socket.Tcp.ToString();
                        }
                        retv.CarrierPlc = PLC.Common.GetInstance(plcMaker, carrierPlcAddressElm.Value, int.Parse(carrierPlcPortElm.Value), hostIpAddress, socket);
                    }
                }

                // コンベア用PLC
                XElement conveyorPlcAddressElm = elm.Element("conveyorPlcAddress");
                XElement conveyorPlcPortElm = elm.Element("conveyorPlcPort");
                XElement conveyorPlcMakerElm = elm.Element("conveyorPlcMaker");
                if (conveyorPlcAddressElm != null && conveyorPlcPortElm != null)
                {
                    if (!string.IsNullOrEmpty(conveyorPlcAddressElm.Value) && !string.IsNullOrEmpty(conveyorPlcPortElm.Value))
                    {
                        string plcMaker = Maker.Mitsubishi.ToString();
                        if (conveyorPlcMakerElm != null && !string.IsNullOrWhiteSpace(conveyorPlcMakerElm.Value))
                        {
                            plcMaker = conveyorPlcMakerElm.Value;
                        }
                        string socket = Socket.Udp.ToString();
                        if (plcMaker == Maker.Keyence.ToString())
                        {
                            socket = Socket.Tcp.ToString();
                        }
                        retv.ConveyorPlc = PLC.Common.GetInstance(plcMaker, conveyorPlcAddressElm.Value, int.Parse(conveyorPlcPortElm.Value), hostIpAddress, socket);
                    }
                }

                // 接続要求信号
                XElement connectionReqBitAddressElm = elm.Element("connectionReqBitAddress");
                if (connectionReqBitAddressElm != null)
                {
                    retv.ConnectionReqBitAddress = connectionReqBitAddressElm.Value;
                }
                //切断要求信号
                XElement disconnectionReqBitAddressElm = elm.Element("disconnectionReqBitAddress");
                if (disconnectionReqBitAddressElm != null)
                {
                    retv.DisconnectionReqBitAddress = disconnectionReqBitAddressElm.Value;
                }
                //出力要求信号
                XElement readDischargePressureDataReqBitAddressElm = elm.Element("readDischargePressureDataReqBitAddress");
                if (readDischargePressureDataReqBitAddressElm != null)
                {
                    retv.ReadDischargePressureDataReqBitAddress = readDischargePressureDataReqBitAddressElm.Value;
                }
                //接続中信号
                XElement inConnectionBitAddressElm = elm.Element("inConnectionBitAddress");
                if (inConnectionBitAddressElm != null)
                {
                    retv.InConnectionBitAddress = inConnectionBitAddressElm.Value;
                }

                return retv;
            }
            catch (Exception ex) 
            {
                return null;
            }
        }

        public static ICarrier GetCarrier(XElement elm) 
        {
            CarrierBase retv = null;
            CarrierType flow = (CarrierType)Enum.Parse(typeof(CarrierType), elm.Element("flow").Value);
            switch (flow)
            {
                case CarrierType.Robot:
#region
                    retv = new Robot(elm.Element("plcAddress").Value, int.Parse(elm.Element("plcPort").Value), int.Parse(elm.Element("carrierNo").Value));

                    //ロボット動作設定      
                    ((Robot)retv).QRWordAddressStart = elm.Element("robotQRWordAddressStart").Value;
                    ((Robot)retv).QRWordAddressLength = int.Parse(elm.Element("robotQRWordAddressLength").Value);
                    ((Robot)retv).QRReaderPoint = elm.Element("robotQRReaderPoint").Value;
					
					XElement movePlanDestPointAddressElm = elm.Element("movePlanDestPointAddress");
					if (movePlanDestPointAddressElm != null)
					{
						((Robot)retv).MovePlanDestPointAddress = movePlanDestPointAddressElm.Value;
					}
#endregion
                    break;

                case CarrierType.Robot2:

#region 実装なし(【2.x】以降のバージョンで実装している)

                    //retv = new Robot2(elm.Element("plcAddress").Value, int.Parse(elm.Element("plcPort").Value), int.Parse(elm.Element("carrierNo").Value));

                    ////ロボット動作設定      
                    //((Robot2)retv).QRWordAddressStart = elm.Element("robotQRWordAddressStart").Value;
                    //((Robot2)retv).QRWordAddressLength = int.Parse(elm.Element("robotQRWordAddressLength").Value);

                    //((Robot2)retv).QRReaderMacPoint = elm.Element("qrReaderMacPoint").Value;
                    //((Robot2)retv).QRReaderLocationPoint = elm.Element("qrReaderLocationPoint").Value;

                    //XElement elmList = elm.Element("holdMagazineStartAddressList");
                    //if (elmList != null)
                    //{
                    //    var elms = elmList.Elements("holdMagazineStartAddress");
                    //    foreach (XElement e in elms)
                    //    {
                    //        ((Robot2)retv).HoldMagazineStartAddressList.Add(e.Value);
                    //    }
                    //}

                    //XElement qrReaderMacNoElm = elm.Element("qrReaderMacNo");
                    //if (qrReaderMacNoElm != null)
                    //{
                    //    ((Robot2)retv).QRReaderMacNo = int.Parse(qrReaderMacNoElm.Value);
                    //}

#endregion

                    break;

                case CarrierType.Robot3:
#region
                    retv = new Robot3(elm.Element("plcAddress").Value, int.Parse(elm.Element("plcPort").Value), int.Parse(elm.Element("carrierNo").Value));

                    //ロボット動作設定      
                    ((Robot3)retv).QRWordAddressStart = elm.Element("robotQRWordAddressStart").Value;
                    ((Robot3)retv).QRWordAddressLength = int.Parse(elm.Element("robotQRWordAddressLength").Value);

                    ((Robot3)retv).QRReaderMacPoint = elm.Element("qrReaderMacPoint").Value;
                    ((Robot3)retv).QRReaderLocationPoint = elm.Element("qrReaderLocationPoint").Value;

                    XElement elmList = elm.Element("holdMagazineStartAddressList");
                    if (elmList != null)
                    {
                        var elms = elmList.Elements("holdMagazineStartAddress");
                        foreach (XElement e in elms)
                        {
                            ((Robot3)retv).HoldMagazineStartAddressList.Add(e.Value);
                        }
                    }

                    XElement qrReaderMacNoElm = elm.Element("qrReaderMacNo");
                    if (qrReaderMacNoElm != null)
                    {
                        ((Robot3)retv).QRReaderMacNo = int.Parse(qrReaderMacNoElm.Value);
                    }

#endregion
                    break;

                case CarrierType.Robot4:
#region
                    retv = new Robot4(elm.Element("plcAddress").Value, int.Parse(elm.Element("plcPort").Value), int.Parse(elm.Element("carrierNo").Value));

                    //ロボット動作設定      
                    ((Robot4)retv).QRWordAddressStart = elm.Element("robotQRWordAddressStart").Value;
                    ((Robot4)retv).QRWordAddressLength = int.Parse(elm.Element("robotQRWordAddressLength").Value);

                    ((Robot4)retv).QRReaderMacPoint = elm.Element("qrReaderMacPoint").Value;
                    ((Robot4)retv).QRReaderLoaderLocationPoint = elm.Element("qrReaderLoaderLocationPoint").Value;
                    ((Robot4)retv).QRReaderUnloaderLocationPoint = elm.Element("qrReaderUnloaderLocationPoint").Value;

                    elmList = elm.Element("holdMagazineStartAddressList");
                    if (elmList != null)
                    {
                        var elms = elmList.Elements("holdMagazineStartAddress");
                        foreach (XElement e in elms)
                        {
                            ((Robot4)retv).HoldMagazineStartAddressList.Add(e.Value);
                        }
                    }

                    qrReaderMacNoElm = elm.Element("qrReaderMacNo");
                    if (qrReaderMacNoElm != null)
                    {
                        ((Robot4)retv).QRReaderMacNo = int.Parse(qrReaderMacNoElm.Value);
                    }
#endregion
                    break;

                case CarrierType.Robot5:
#region
                    retv = new Robot5(elm.Element("plcAddress").Value, int.Parse(elm.Element("plcPort").Value), int.Parse(elm.Element("carrierNo").Value));

                    //ロボット動作設定      
                    ((Robot5)retv).QRWordAddressStart = elm.Element("robotQRWordAddressStart").Value;
                    ((Robot5)retv).QRWordAddressLength = int.Parse(elm.Element("robotQRWordAddressLength").Value);

                    ((Robot5)retv).QRReaderMacPoint = elm.Element("qrReaderMacPoint").Value;
                    ((Robot5)retv).QRReaderLocationPoint = elm.Element("qrReaderLocationPoint").Value;

                    elmList = elm.Element("holdMagazineStartAddressList");
                    if (elmList != null)
                    {
                        var elms = elmList.Elements("holdMagazineStartAddress");
                        foreach (XElement e in elms)
                        {
                            ((Robot5)retv).HoldMagazineStartAddressList.Add(e.Value);
                        }
                    }

                    qrReaderMacNoElm = elm.Element("qrReaderMacNo");
                    if (qrReaderMacNoElm != null)
                    {
                        ((Robot5)retv).QRReaderMacNo = int.Parse(qrReaderMacNoElm.Value);
                    }

                    #endregion
                    break;

                case CarrierType.Bridge:
                    retv = new Bridge();

                    break;
            }


            retv.LineNo = elm.Element("lineNo").Value;
            retv.CarNo = int.Parse(elm.Element("carrierNo").Value);
            retv.Name = elm.Element("carrierName").Value;

            // インターロックオブジェクトの追加
            retv.SemaphoreSlimList = new SortedList<string, string>();
            XElement sub = elm.Element("interLockList");
            if (sub != null)
            {
                var subElements = sub.Elements("interLock");
                foreach (XElement subelm in subElements)
                {
                    string key = subelm.Attribute("key").Value;
                    string semName = subelm.Attribute("name").Value;
                    retv.SemaphoreSlimList.Add(key, semName);
                }
            }

            // ARMSThreadObjectのThreadRoutineWorkのループ時間
            XElement threadRoutineWorkMilliSecondElm = elm.Element("threadRoutineWorkMilliSecond");
            if (threadRoutineWorkMilliSecondElm != null)
            {
                int sec;
                if (int.TryParse(threadRoutineWorkMilliSecondElm.Value, out sec) == true)
                {
                    retv.ThreadRoutineWorkMilliSecond = sec;
                }
            }

            return retv;
        }
    }
}
