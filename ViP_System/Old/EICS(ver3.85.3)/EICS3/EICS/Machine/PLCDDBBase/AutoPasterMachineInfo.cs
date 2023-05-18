using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EICS.Database;
using ArmsApi.Model;

namespace EICS.Machine
{
    /// <summary>
    /// 蛍光体シート貼り合わせ自動機
    /// </summary>
    class AutoPasterMachineInfo : PLCDDGBasedMachine
    {
        #region PLCアドレス定義
        
        private const string PLC_MEMORY_ADDR_MACHINE_READY = "EM50070";

        private const string PLC_MEMORY_ADDR_PROGRAMNAME_REQ = "EM50054";
        private const string PLC_MEMORY_ADDR_PROGRAMNAME_SEND = "EM50027";
        private const string PLC_MEMORY_ADDR_PROGRAMNAME_RESULT_SEND = "EM50026";
        private const string PLC_MEMORY_ADDR_PROGRAMNAME_SEND_COMPLETE = "EM50023";

        private const string PLC_MEMORY_ADDR_PARAMCHECK_REQ = "EM50050";
        private const string PLC_MEMORY_ADDR_PARAMERTERCHECK_RESULT_SEND = "EM50020";

        private const string PLC_MEMORY_ADDR_COMPLETE_REQ = "EM50052";

        private string[] PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST()
        { return new string[] { "EM51400", "EM51410", "EM51420", "EM51430", "EM51440", "EM51450", "EM51460", "EM51470" }; }

        private const int MAGAZINENO_LENGTH = 10;

        private List<string> PLC_MEMORY_ADDR_LEFTTRAYORDER_SEND
        {
            get { return new List<string> { "EM50100", "EM50101", "EM50102", "EM50103", "EM50104", "EM50105", "EM50106", "EM50107" }; }
        }

        private List<string> PLC_MEMORY_ADDR_RIGHTTRAYORDER_SEND
        { 
            get { return new List<string> { "EM50110", "EM50111", "EM50112", "EM50113", "EM50114", "EM50115", "EM50116", "EM50117" }; } 
        }

        public List<string> PLC_MEMORY_ADDR_LEFTSHEETORDER_SEND
        {
            // トレイ毎の1枚目のアドレスを取得可能
            // 1トレイ分(12枚)を取得する場合はgetLeftSheetOrderAddress関数を使用する

            get { return new List<string> { "EM50300", "EM50340", "EM50380", "EM50420", "EM50460", "EM50500", "EM50540", "EM50580" }; }
        }
        public List<string> getLeftSheetOrderAddress(int orderNo)
        {
            List<string> retv = new List<string>();

            string firstAddress = PLC_MEMORY_ADDR_LEFTSHEETORDER_SEND[orderNo];
            retv.Add(firstAddress);

            int count = int.Parse(firstAddress.Substring(firstAddress.Length - 4, 4));
            for (int i = 0; i < SheetMaxCount-1; i++)
            {
                retv.Add(firstAddress.Substring(0, 3) + (count + (i+1)).ToString("0000"));
            }

            return retv;
        }

        public List<string> PLC_MEMORY_ADDR_RIGHTSHEETORDER_SEND
        {
            // トレイ毎の1枚目のアドレスを取得可能
            // 1トレイ分(12枚)を取得する場合はgetRightSheetOrderAddress関数を使用する

            get { return new List<string> { "EM50320", "EM50360", "EM50400", "EM50440", "EM50480", "EM50520", "EM50560", "EM50600" }; }
        }
        public List<string> getRightSheetOrderAddress(int orderNo)
        {
            List<string> retv = new List<string>();

            string firstAddress = PLC_MEMORY_ADDR_RIGHTSHEETORDER_SEND[orderNo];
            retv.Add(firstAddress);

            int count = int.Parse(firstAddress.Substring(firstAddress.Length - 4, 4));
            for (int i = 0; i < SheetMaxCount-1; i++)
            {
                retv.Add(firstAddress.Substring(0, 3) + (count + (i+1)).ToString("0000"));
            }

            return retv;
        }

        #endregion

        public int TrayMaxCount
        {
            get
            {
                if (PLC_MEMORY_ADDR_LEFTTRAYORDER_SEND.Count != PLC_MEMORY_ADDR_RIGHTTRAYORDER_SEND.Count)
                {
                    throw new ApplicationException("左右トレイの最大段数が違う為、トレイ最大段数が分かりません。");
                }

                return PLC_MEMORY_ADDR_LEFTTRAYORDER_SEND.Count;
            }
        }

        public int SheetMaxCount
        {
            get
            {
                return 12;
            }
        }

        #region 定数

        /// <summary>
        /// トレイデータが空の時に装置が出力する出来栄えデータの値
        /// </summary>
        private const string NULL_VALUE = "-9999";

        #endregion

        public LSETInfo LsetInfo { get; set; }
        
        protected override int GetTimingNo(string chipNm)
        {
            return Constant.TIM_AUTOPASTER;
        }
        public AutoPasterMachineInfo(LSETInfo lsetInfo)
        {
            this.LsetInfo = lsetInfo;

            // 設定ファイルPLC設定を基ににPLCインスタンス初期化
            InitPLC(lsetInfo);
        }

        public override void InitFirstLoop(LSETInfo lsetInfo)
        {
        }
        public override void CheckFile(LSETInfo lsetInfo)
        {
            // 設定ファイル読み出し
            InitPropAtLoop(lsetInfo);
            this.LsetInfo = lsetInfo;

            string errMsg;

            base.machineStatus = Constant.MachineStatus.Runtime;

            if (readyMachine() == true)
            {
                if (requestProgramName())
                {
                    string typeCd;
                    if (checkTypeBlend(lsetInfo.InlineCD, out typeCd, out errMsg))
                    {
                        string programName = getProgramName(lsetInfo.InlineCD, lsetInfo.ModelNM, typeCd);

                        responseCombinationOrder();

                        responseTypeBlendCheckOKAndProgramName(programName);
                    }
                    else
                    {
                        responseTypeBlendCheckNG();
                        throw new ApplicationException(errMsg);
                    }
                }

                if (requestParameterCheck())
                {
                    this.LsetInfo.TypeCD = GetTypeFromPlcLotInfo();
                    if (checkParameter())
                    {
                        responseParameterCheckOk();
                    }
                    else
                    {
                        responseParameterCheckNg();
                    }

                    //このルーチンだとOK、NGファイルは不要だがEndingProcessの処理上作成してしまうので後で消す。
                    List<string> startFilePathList = Common.GetFiles(StartFileDir, ".*\\." + Structure.CIFS.EXT_OK_FILE);
                    startFilePathList.AddRange(Common.GetFiles(StartFileDir, ".*\\." + Structure.CIFS.EXT_NG_FILE));
                    foreach (string filePath in startFilePathList)
                    {
                        File.Delete(filePath);
                    }
                }

                if (requestWorkComplete())
                {
                    CreateFileProcess(lsetInfo, false);
                }
            }

            int timingNo = GetTimingNo(this.LsetInfo.ChipNM);
            EndingProcess(this.LsetInfo, timingNo);

            //このルーチンだとOK、NGファイルは不要だがEndingProcessの処理上作成してしまうので後で消す。
            List<string> endFilePathList = Common.GetFiles(EndFileDir, ".*\\." + Structure.CIFS.EXT_OK_FILE);
            endFilePathList.AddRange(Common.GetFiles(EndFileDir, ".*\\." + Structure.CIFS.EXT_NG_FILE));
            foreach (string filePath in endFilePathList)
            {
                File.Delete(filePath);
            }
        }

        private bool readyMachine()
        {
            if (plc.GetBit(PLC_MEMORY_ADDR_MACHINE_READY) == PLC.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool requestProgramName()
        {
            if (plc.GetBit(PLC_MEMORY_ADDR_PROGRAMNAME_REQ) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム 品種要求確認 [アドレス:{PLC_MEMORY_ADDR_PROGRAMNAME_REQ}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// プログラム名送信 (型番混在チェックOK)
        /// </summary>
        /// <param name="programName"></param>
        private void responseTypeBlendCheckOKAndProgramName(string programName)
        {
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PROGRAMNAME_SEND, Convert.ToInt32(programName));
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PROGRAMNAME_SEND_COMPLETE, 0);
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PROGRAMNAME_REQ, 0);

            outputLog($"装置 << システム プログラム名:{programName}送信 [アドレス:{PLC_MEMORY_ADDR_PROGRAMNAME_SEND}]");
            outputLog($"装置 << システム 装置停止要求(0：継続)送信 [アドレス:{PLC_MEMORY_ADDR_PROGRAMNAME_SEND_COMPLETE}]");
        }

        /// <summary>
        /// プログラム名送信 (型番混在チェックNG)
        /// </summary>
        /// <param name="programName"></param>
        private void responseTypeBlendCheckNG()
        {
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PROGRAMNAME_RESULT_SEND, 1);
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PROGRAMNAME_REQ, 0);

            outputLog($"装置 << システム 装置停止要求(1：停止)送信 [アドレス:{PLC_MEMORY_ADDR_PROGRAMNAME_SEND}]");
        }

        private bool requestParameterCheck()
        {
            if (plc.GetBit(PLC_MEMORY_ADDR_PARAMCHECK_REQ) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム パラメータチェック要求確認 [アドレス:{PLC_MEMORY_ADDR_PARAMCHECK_REQ}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void responseParameterCheckOk()
        {
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PARAMERTERCHECK_RESULT_SEND, 0);
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PARAMCHECK_REQ, 0);

            outputLog($"装置 << システム パラメータ照合結果(0：OK)送信 [アドレス:{PLC_MEMORY_ADDR_PARAMERTERCHECK_RESULT_SEND}]");
        }

        private void responseParameterCheckNg()
        {
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PARAMERTERCHECK_RESULT_SEND, 1);
            plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_PARAMCHECK_REQ, 0);

            outputLog($"装置 << システム パラメータ照合(1：NG)送信 [アドレス:{PLC_MEMORY_ADDR_PARAMERTERCHECK_RESULT_SEND}]");
        }

        private bool requestWorkComplete()
        {
            if (plc.GetBit(PLC_MEMORY_ADDR_COMPLETE_REQ) == PLC.BIT_ON)
            {
                outputLog($"装置 >> システム 完了処理要求確認 [アドレス:{PLC_MEMORY_ADDR_COMPLETE_REQ}]");
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool checkTypeBlend(int lineCd, out string typeCd, out string errMsg)
        {
            typeCd = "";
            errMsg = "";

            // -------- マガジンNo 取得 -> アッセンロット情報取得
            for (int i = 0; i < PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST().Length; i++)
            {
                string address = PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST()[i];
                string lotNo = plc.GetString(address, MAGAZINENO_LENGTH, false, false).Replace("\r", "").Replace("\0", "");
                if (string.IsNullOrEmpty(lotNo)) continue;

                string[] lotNoArray = lotNo.Split(' ');

                if (lotNoArray.Length >= 2)
                {
                    lotNo = lotNoArray[1];
                }

                string lotTypeCd = GetLotTypeCd(lotNo);
                if (string.IsNullOrEmpty(lotTypeCd))
                {
                    errMsg = $"読み取み込んだロットのNASCA指図から型番情報が取得できません。[段数：[{i + 1}段],アドレス：{address},ロットNo：{lotNo}]";
                    return false;
                }

                // -------- 型番混在チェック
                if (string.IsNullOrWhiteSpace(typeCd) == true)
                {
                    // 1ロット目
                    typeCd = lotTypeCd;
                }
                else
                {
                    // 2ロット目：型番混在チェック
                    if (typeCd != lotTypeCd)
                    {
                        errMsg = $"読込ロットの型番が他のロットと違います。[段数：[{i + 1}段],ロットNo：{lotNo},型番：{lotTypeCd}],[型番(他ロット)：{typeCd}]";
                        return false;
                    }
                }
            }
            if (string.IsNullOrEmpty(typeCd))
            {
                errMsg = "全てのトラベルシート情報からロット情報を取得できませんでした。QR読み取りに失敗した思われます。再度読み直して下さい。";
                return false;
            }

            return true;
        }

        private string getProgramName(int lineCd, string modelNm, string typecd)
        {
            List<string> lotList = getWorkingLots();

            string workCd = string.Empty;
            LotInfoReferring reference = GetLotInfoReferring(this.LsetInfo.EquipmentNO, this.LsetInfo.InlineCD);
            if (reference == LotInfoReferring.ARMS)
            {
                Arms.AsmLot lot = Arms.AsmLot.GetAsmLot(lineCd, lotList.First());
                Arms.Magazine svrMag = Arms.Magazine.GetCurrent(lineCd, lotList.First());
                Arms.Process p = Arms.Process.GetNextProcess(lineCd, svrMag.NowCompProcess, lot);
                workCd = p.WorkCd;
            }

            outputLog($"プログラム名取得 <条件> 製品型番:{typecd} 作業CD:{workCd}");
            string programName = Database.Plm.GetProgramName(typecd, lineCd, modelNm, workCd, this.Code);
            return programName;
        }

        /// <summary>
        /// パラメータチェック
        /// </summary>
        /// <returns></returns>
        private bool checkParameter()
        {
            CreateFileProcess(this.LsetInfo, true);

            List<ErrMessageInfo> errMessageList = new List<ErrMessageInfo>();
            StartingProcess(this.LsetInfo, out errMessageList);

            if (errMessageList.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void outputLog(string logText)
        {
            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
                $"設備:{this.LsetInfo.ModelNM}/{this.LsetInfo.EquipmentNO}/{this.LsetInfo.MachineSeqNO}号機 {logText}");
        }

        private string GetTypeFromPlcLotInfo()
        {
            for (int i = 0; i < PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST().Length; i++)
            {
                string address = PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST()[i];
                string lotNo = plc.GetString(address, MAGAZINENO_LENGTH, false, false).Replace("\r", "").Replace("\0", "");
                if (string.IsNullOrEmpty(lotNo)) continue;

                string[] lotNoArray = lotNo.Split(' ');

                if (lotNoArray.Length >= 2)
                {
                    lotNo = lotNoArray[1];
                }

                return GetLotTypeCd(lotNo);
            }
            return null;
        }

        /// <summary>
        /// CheckMachineFile_SpeedUp関数内で使用。取得データからバカ値「-9999」を除く為、オーバーライト
        /// </summary>
        protected override string[] getWithOutValues()
        {
            // ファイルから取得したデータを加工する。オーバーライド先で修正
            return new string[] { "-9999" };
        }

        /// <summary>
        /// 最適なトレイ、シートの組み合わせ順を装置に送信
        /// </summary>
        private void responseCombinationOrder()
        {
            // 左トレイ(クリア+KSF)は設定測定値が高い(濃い)順、右トレイ(βサイアロン)は測定値が低い(薄い)順に組み合わせる

            // TODO 色調Y管理No 後で設定ファイルに出す
            int qcParamColorYNo = 1000003;

            var rightTrayData = Tray.GetTraySummaryMeasureValue(this.plc, this.LineCD, qcParamColorYNo, Tray.Position.Right)
                .OrderBy(t => t.MeasureAveVAL).ToArray();
            var leftTrayData = Tray.GetTraySummaryMeasureValue(this.plc, this.LineCD, qcParamColorYNo, Tray.Position.Left)
                .OrderByDescending(t => t.MeasureAveVAL).ToArray();

            if (rightTrayData.Count() == 0 || leftTrayData.Count() == 0 || rightTrayData.First().IsKsfSheet)
            {
                outputLog($"装置 << システム KSF+クリア貼付、又は色調測定データが存在しない為、デフォルト組み合わせ順送信開始");

                // デフォルト順序を送信　左トレイがクリア、右トレイがKFSの貼付を想定
                for (int i = 0; i < Tray.GetTrayNo(this.plc, Tray.Position.Left).Count; i++)
                {
                    this.plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_LEFTTRAYORDER_SEND[i], i + 1);
                    outputLog($"装置 << システム 左トレイ順{i + 1}送信  指定段数:{i + 1} [PLCアドレス:{PLC_MEMORY_ADDR_LEFTTRAYORDER_SEND[i]}]");

                    this.plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_RIGHTTRAYORDER_SEND[i], i + 1);
                    outputLog($"装置 << システム 右トレイ順{i + 1}送信  指定段数:{i + 1} [PLCアドレス:{PLC_MEMORY_ADDR_RIGHTTRAYORDER_SEND[i]}]");

                    List<string> rightAddressList = getRightSheetOrderAddress(i);
                    List<string> leftAddressList = getLeftSheetOrderAddress(i);
                    for (int j = 0; j < rightAddressList.Count; j++)
                    {
                        this.plc.SetWordAsDecimalData(leftAddressList[j], j+1);
                        outputLog($"装置 << システム 左シート順{i + 1}送信  指定シート位置:{j+1} [PLCアドレス:{leftAddressList[j]}]");

                        this.plc.SetWordAsDecimalData(rightAddressList[j], j+1);
                        outputLog($"装置 << システム 右シート順{i + 1}送信  指定シート位置:{j+1} [PLCアドレス:{rightAddressList[j]}]");
                    }
                }

                outputLog($"装置 << システム KSF+クリア貼付、又は色調測定データが存在しない為、デフォルト組み合わせ順送信完了");

                return;
            }

            // 最適な組み合わせを送信　左トレイがクリア+KSF、右トレイがβサイアロンの貼付を想定
            outputLog($"装置 << システム βサイアロン貼付の為、最適組み合わせ順送信開始");

            // トレイ順を送信
            if (leftTrayData.Count() == rightTrayData.Count())
            {
                for (int i = 0; i < PLC_MEMORY_ADDR_LEFTTRAYORDER_SEND.Count; i++)
                {
                    if (leftTrayData.Count() <= i) continue;

                    this.plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_LEFTTRAYORDER_SEND[i], leftTrayData[i].RowNo);
                    outputLog($"装置 << システム 左トレイ順{i + 1}送信 トレイNo:{leftTrayData[i].TrayNo} 指定段数:{leftTrayData[i].RowNo} [PLCアドレス:{PLC_MEMORY_ADDR_LEFTTRAYORDER_SEND[i]}]");
                }

                for (int i = 0; i < PLC_MEMORY_ADDR_RIGHTTRAYORDER_SEND.Count; i++)
                {
                    if (rightTrayData.Count() <= i) continue;

                    this.plc.SetWordAsDecimalData(PLC_MEMORY_ADDR_RIGHTTRAYORDER_SEND[i], rightTrayData[i].RowNo);
                    outputLog($"装置 << システム 右トレイ順{i + 1}送信 トレイNo:{rightTrayData[i].TrayNo} 指定段数:{rightTrayData[i].RowNo} [PLCアドレス:{PLC_MEMORY_ADDR_RIGHTTRAYORDER_SEND[i]}]");
                }
            }
            else
            {
                throw new ApplicationException($"トレイ左右の色調測定結果が全て保存されていないので、最適なシート組み合わせを算出できません。色調測定結果が正しいか確認して下さい。");
            }

            // シート順を送信
            for (int i = 0; i < leftTrayData.Count(); i++)
            {
                // 先で決定したトレイ順を元にトレイ内で最適なシート順を作成する
                var rightSheetData = PsMeasureResult.SheetSummary.GetCurrentAverage(rightTrayData[i].TrayNo, qcParamColorYNo, this.LineCD)
                    .OrderBy(s => s.MeasureAveVAL).ToArray();
                var leftSheetData = PsMeasureResult.SheetSummary.GetCurrentAverage(leftTrayData[i].TrayNo, qcParamColorYNo, this.LineCD)
                    .OrderByDescending(s => s.MeasureAveVAL).ToArray();
                
                List<string> addressList = getLeftSheetOrderAddress(i);
                for (int j = 0; j < addressList.Count; j++)
                {
                    this.plc.SetWordAsDecimalData(addressList[j], leftSheetData[j].SheetNo);
                    outputLog($"装置 << システム 左シート順{i + 1}送信 トレイNo:{leftTrayData[i].TrayNo} 指定シート位置:{leftSheetData[j].SheetNo} [PLCアドレス:{addressList[j]}]");
                }

                addressList = getRightSheetOrderAddress(i);
                for (int j = 0; j < addressList.Count; j++)
                {
                    this.plc.SetWordAsDecimalData(addressList[j], rightSheetData[j].SheetNo);
                    outputLog($"装置 << システム 右シート順{i + 1}送信 トレイNo:{rightTrayData[i].TrayNo} 指定シート位置:{rightSheetData[j].SheetNo} [PLCアドレス:{addressList[j]}]");
                }

                // データベースに組み合わせ結果保存
                savePastedResult(rightSheetData, leftSheetData);
            }
            outputLog($"装置 << システム βサイアロン貼付の為、最適組み合わせ順送信完了");
        }

        private void savePastedResult(PsMeasureResult.SheetSummary[] rightSheetOrders, PsMeasureResult.SheetSummary[] leftSheetOrders)
        {
            using (DataContext.EICSDataContext db = new DataContext.EICSDataContext(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, this.LineCD)))
            {
                for (int i = 0; i < rightSheetOrders.Count(); i++)
                {
                    var registeredDataList = db.TnPsPastedResult
                        .Where(p => p.Combi1Lot_NO == leftSheetOrders[i].LotNo && p.Combi1Tray_NO == leftSheetOrders[i].TrayNo && p.Combi1Sheet_NO == leftSheetOrders[i].SheetNo
                        && p.Combi2Lot_NO == rightSheetOrders[i].LotNo && p.Combi2Tray_NO == rightSheetOrders[i].TrayNo && p.Combi2Sheet_NO == rightSheetOrders[i].SheetNo);
                    if (registeredDataList.Any())
                    {
                        // 登録済の場合は除外
                        continue;
                    }

                    DataContext.TnPsPastedResult d = new DataContext.TnPsPastedResult();

                    d.Combi1Lot_NO = leftSheetOrders[i].LotNo;
                    d.Combi1Tray_NO = leftSheetOrders[i].TrayNo;
                    d.Combi1Sheet_NO = leftSheetOrders[i].SheetNo;

                    d.Combi2Lot_NO = rightSheetOrders[i].LotNo;
                    d.Combi2Tray_NO = rightSheetOrders[i].TrayNo;
                    d.Combi2Sheet_NO = rightSheetOrders[i].SheetNo;

                    string dicingLotNo = LotCarrier.GetLotNo(leftSheetOrders[i].TrayNo, true).SingleOrDefault();
                    if (string.IsNullOrEmpty(dicingLotNo))
                    {
                        d.PastedLot_NO = "";
                        outputLog($"▲警告 トレイNo:{leftSheetOrders[i].TrayNo}はLotCarrier(トレイ-ロット紐づけ)にダイシングロットの紐づけが存在しない為、TnPsPastedResult(貼り合わせ結果)のダイシングロットは空白で登録しました。");
                    }
                    else
                    {
                        d.PastedLot_NO = dicingLotNo;
                    }

                    d.LastUpd_DT = System.DateTime.Now;
                    db.TnPsPastedResult.InsertOnSubmit(d);
                }

                db.SubmitChanges();
            }
        }

        public class Tray
        {
            public enum Position
            {
                Left, Right
            }

            public static List<string> LeftTrayNoPlcAddress
            {
                get { return new List<string> { "EM51800", "EM51810", "EM51820", "EM51830", "EM51840", "EM51850", "EM51860", "EM51870" }; }
            }

            public static List<string> RightTrayNoPlcAddress
            {
                get { return new List<string> { "EM51880", "EM51890", "EM51900", "EM51910", "EM51920", "EM51930", "EM51940", "EM51950" }; }
            }

            public static List<string> GetTrayNo(EICS.Structure.IPlc plc, Position position)
            {
                List<string> retv = new List<string>();

                List<string> trayNoAddressList = new List<string>();
                if (position == Position.Left)
                {
                    trayNoAddressList = LeftTrayNoPlcAddress;
                }
                else
                {
                    trayNoAddressList = RightTrayNoPlcAddress;
                }

                foreach (string address in trayNoAddressList)
                {
                    string trayNo = plc.GetString(address, MAGAZINENO_LENGTH, false, false);
                    if (string.IsNullOrEmpty(trayNo))
                        continue;

                    retv.Add(trayNo);
                }

                return retv;
            }

            /// <summary>
            /// トレイ毎に色調測定の平均値を集計したリストを取得
            /// </summary>
            /// <returns></returns>
            public static List<PsMeasureResult.TraySummary> GetTraySummaryMeasureValue(EICS.Structure.IPlc plc, int hostLineCd, int qcparamNo, Position position)
            {
                List<PsMeasureResult.TraySummary> retv = new List<PsMeasureResult.TraySummary>();

                List<string> trayNoList = GetTrayNo(plc, position);
                for (int i = 0; i < trayNoList.Count; i++)
                {
                    if (string.IsNullOrEmpty(trayNoList[i]) == true)
                        continue;

                    PsMeasureResult.TraySummary trayData = Database.PsMeasureResult.TraySummary.GetCurrentAverage(trayNoList[i], qcparamNo, hostLineCd);
                    if (trayData == null)
                        continue;

                    trayData.RowNo = i + 1;

                    retv.Add(trayData);
                }

                return retv;
            }
        }

        private string GetLotTypeCd(string lotNo)
        {
            LotInfoReferring reference = GetLotInfoReferring(this.LsetInfo.EquipmentNO, this.LsetInfo.InlineCD);
            if (reference == LotInfoReferring.ARMS)
            {
                Arms.AsmLot lot = Arms.AsmLot.GetAsmLot(this.LsetInfo.InlineCD, lotNo);
                return lot.TypeCd;
            }
            else
            {
                return ConnectDB.GetTypeCD(this.LsetInfo.InlineCD, lotNo);
            }
        }

        /// <summary>
        /// 装置から作業中のロットNoを取得
        /// </summary>
        /// <returns></returns>
        private List<string> getWorkingLots()
        {
            List<string> lotList = new List<string>();
           
            for (int i = 0; i < PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST().Length; i++)
            {
                string address = PLC_MEMORY_ADDR_START_LOTNO_ADDR_LIST()[i];
                string lotNo = plc.GetString(address, MAGAZINENO_LENGTH, false, false).Replace("\r", "").Replace("\0", "");
                if (string.IsNullOrEmpty(lotNo)) continue;

                string[] lotNoArray = lotNo.Split(' ');
                if (lotNoArray.Length >= 2)
                {
                    lotList.Add(lotNoArray[1]);
                }
            }

            return lotList;
        }
    }
}
