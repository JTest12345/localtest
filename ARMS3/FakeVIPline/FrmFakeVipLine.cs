using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArmsApi;
using ARMS3.Model.PLC;
using System.Data.SqlClient;
using System.Threading;

namespace ARMS3.FakeVIPline
{
    public partial class FrmFakeNline : Form
    {
        // PLC無しで起動する場合はtrueにする
        bool DebugMode = false;

        string CR = "\r\n";
        Mitsubishi Plc = new Mitsubishi("192.168.1.99", 1026);
        Keyence Keylc = new Keyence("192.168.1.100", 8500);
        Dictionary<string, int> initPlcBitData = new Dictionary<string, int>()
        {
            ////テスト用
            //{ "B000000", 1 }, //
            //{ "B000001", 0 }, //
            //{ "B000002", 1 }, //
            //{ "B000003", 0 }, //
            //{ "B000004", 1 }, //
            //ロボット
            { "B0000E0", 1 }, //STAT_ROBOT_COMMAND_READY
            { "B0000F0", 0 }, //REQ_ROBOT_MOVE_ORG2MID
            { "B0000EF", 0 }, //MAGAZINE_EXIST_QR_STAGE
            { "B0000E5", 0 }, //ROBOT_MOVE2_LINER_COMPLETE
            { "B0000F6", 0 }, //REQ_ROBOT_MOVE2_MID2DST
            //オーブンプロファイル
            { "B0000FE", 0 }, //profileChangerReadyBitAddress
            { "B000060", 0 }, //dbOvenChangeInterlockStartAddress
            { "B00006E", 0 }, //moldOvenChangeInterlockStartAddress
            { "B001140", 0 }, //dbOvenAutoModeBitStartAddress
            { "B00114E", 0 }, //moldOvenAutoModeBitStartAddress
            { "B001160", 0 }, //dbOvenProfileChangeCompleteBitStartAddress
            { "B00116E", 0 }, //moldOvenProfileChangeCompleteBitStartAddress
            { "B001180", 0 }, //dbOvenProfileChangeRequestBitStartAddress
            { "B00118E", 0 }, //moldOvenProfileChangeRequestBitStartAddress
            //途中払出コンベア
            { "B0002C8", 0 }, //machineReadyBitAddress
            { "B0002CA", 0 }, //loaderReqBitAddress
            //完成品払出コンベア
            { "B0002E8", 0 }, //machineReadyBitAddress
            { "B0002EA", 1 }, //loaderReqBitAddress
            //アオイマガジン排出コンベア
            { "B0002B8", 0 }, //machineReadyBitAddress
            { "B0002BA", 1 }, //loaderReqBitAddress
            //途中投入コンベヤ
            { "B0002D0", 0 }, //machineReadyBitAddress
            { "B0002D2", 0 }, //unloaderReqBitAddress
            { "B0000E8", 0 }, //magazineArriveBitAddress <!--ロボット　投入C/V ＱＲ読取り部マガジン有-->
            { "B0000EB", 0 }, //missingReservedMagazineBitAddress <!--ロボット　投入C/V  取出部マガジン取除き-->
            { "B0000F2", 0 }, //outputReserveBitAddress <!--PC→投入C/Vマガジン排出許可-->
            //空マガジン投入コンベヤ
            { "B0002C0", 0 }, //machineReadyBitAddress
            { "B0002C2", 0 }, //empMagUnloaderReqBitAddress
            //MAP基板マガジン投入コンベヤ
            { "B0002B0", 0 }, //machineReadyBitAddress
            { "B0002B2", 0 }, //empMagUnloaderReqBitAddress
            //ライン間ブリッジ
            { "B000340", 0 }, //machineReadyBitAddress
            { "B000342", 1 }, //loaderReqBitAddress
            { "B000343", 1 }, //unloaderReqBitAddress
            { "B000344", 0 }, //empMagUnloaderReqBitAddress
            { "B0000FA", 0 }, //empMagInputBitAddress
            { "B00001F", 0 }, //clearMagazinesBitAddress
            //ZDダイボンド1号機
            { "B000101", 0 }, //machineReadyBitAddress
            { "B000103", 0 }, //loaderReqBitAddress
            { "B000106", 0 }, //unloaderReqBitAddress
            { "B0000C0", 0 }, //clearMagazinesBitAddress
            { "B000108", 1 }, //waferBitAddressStart
            { "B000107", 1 }, //startWorkTableSensorAddress
            { "B00011B", 0 }, //waferChangerChangeBitAddress
            { "B000104", 0 }, //empMagUnloaderReqBitAddress
            { "B000105", 0 }, //empMagLoaderReqBitAddress
            { "B000080", 0 }, //inputForbiddenBitAddress
            { "B0000A0", 0 }, //dischargeModeBitAddress
            { "B00011D", 0 }, //secondLoaderReqBitAddress
            { "B00011F", 0 }, //secondEmpMagLoaderReqBitAddress
            //ZDダイボンド3号機
            { "B000141", 0 }, //machineReadyBitAddress
            { "B000143", 0 }, //loaderReqBitAddress
            { "B000146", 0 }, //unloaderReqBitAddress
            { "B0000C2", 0 }, //clearMagazinesBitAddress
            { "B000148", 1 }, //waferBitAddressStart
            { "B000147", 1 }, //startWorkTableSensorAddress
            { "B00015B", 0 }, //waferChangerChangeBitAddress
            { "B000144", 0 }, //empMagUnloaderReqBitAddress
            { "B000145", 0 }, //empMagLoaderReqBitAddress
            { "B000082", 0 }, //inputForbiddenBitAddress
            { "B00015D", 0 }, //secondLoaderReqBitAddress
            { "B00015F", 0 }, //secondEmpMagLoaderReqBitAddress
            //ZDダイボンドオーブン1号機
            { "B001210", 0 },
            { "B0000D1", 0 },
            { "B00008A", 0 },
            { "B0000AA", 0 },
            { "B001212", 0 },
            { "B001213", 0 },
            { "B001215", 0 },
            { "B001216", 0 },
            { "B000630", 0 },
            //ZDダイボンドオーブン7号機
            { "B001240", 0 },
            { "B000017", 0 },
            { "B001242", 0 },
            { "B001243", 0 },
            { "B001245", 0 },
            { "B001246", 0 },
            { "B000636", 0 },
            //ダイボンド4号機
            { "B000161", 0 }, //machineReadyBitAddress
            { "B000163", 0 }, //loaderReqBitAddress
            { "B000166", 0 }, //unloaderReqBitAddress
            { "B0000C3", 0 }, //clearMagazinesBitAddress
            { "B000168", 1 }, //waferBitAddressStart
            { "B000167", 1 }, //startWorkTableSensorAddress
            { "B00017B", 0 }, //waferChangerChangeBitAddress
            { "B000164", 0 }, //empMagUnloaderReqBitAddress
            { "B000165", 0 }, //empMagLoaderReqBitAddress
            { "B000083", 0 }, //inputForbiddenBitAddress
            { "B0000A4", 0 }, //dischargeModeBitAddress
            { "B00017D", 0 }, //secondLoaderReqBitAddress
            { "B00017F", 0 }, //secondEmpMagLoaderReqBitAddress
            //WBプラズマ洗浄機
            { "B000321", 0 }, 　//machineReadyBitAddress
            { "B0000DB", 0 },  //clearMagazinesBitAddress
            { "B00008D", 0 }, //inputForbiddenBitAddress
            { "B0000AD", 0 }, //dischargeModeBitAddress
            { "B000323", 0 }, //loader_lane_key1
            { "B000324", 0 },  //loader_lane_key2
            { "B000325", 0 },  //loader_lane_key3
            { "B000326", 0 },  //empMagUnloader_lane_key1
            { "B000327", 0 },  //empMagUnloader_lane_key2
            { "B000328", 0 },  //empMagUnloader_lane_key3
            { "B000329", 0 },  //empMagLoader_lane_key1
            { "B00032A", 0 }, //empMagLoader_lane_key2
            { "B00032B", 0 },  //empMagLoader_lane_key3
            { "B00032C", 0 },  //unloader_lane_key1
            { "B00032D", 0 }, //unloader_lane_key2
            { "B00032E", 0 },  //unloader_lane_key3
            //ワイヤボンダ―1号機
            { "B000241", 0 }, //machineReadyBitAddress
            { "B000243", 0 }, //loaderReqBitAddress
            { "B000246", 0 }, //unloaderReqBitAddress
            { "B0000CA", 0 }, //clearMagazinesBitAddress
            { "B000244", 0 }, //empMagUnloaderReqBitAddress
            { "B000245", 0 }, //empMagLoaderReqBitAddress
            { "B00008E", 0 }, //inputForbiddenBitAddress
            { "B0000AE", 0 }, //dischargeModeBitAddress
            { "B00024D", 0 }, //secondLoaderReqBitAddress
            { "B00024F", 0 }, //secondEmpMagLoaderReqBitAddress
            //ワイヤボンダ―7号機
            { "B000301", 0 }, //machineReadyBitAddress
            { "B000303", 0 }, //loaderReqBitAddress
            { "B000306", 0 }, //unloaderReqBitAddress
            { "B000011", 0 }, //clearMagazinesBitAddress
            { "B000304", 0 }, //empMagUnloaderReqBitAddress
            { "B000305", 0 }, //empMagLoaderReqBitAddress
            { "B00009C", 0 }, //inputForbiddenBitAddress
            { "B000001", 0 }, //dischargeModeBitAddress
            { "B00030D", 0 }, //secondLoaderReqBitAddress
            { "B00030F", 0 }, //secondEmpMagLoaderReqBitAddress
            //外観検査機1号機
            { "B001601", 0 }, //machineReadyBitAddress
            { "B001603", 0 }, //loaderReqBitAddress
            { "B001604", 0 }, //unloaderReqBitAddress
            { "B000093", 0 }, //inputForbiddenBitAddress
            { "B0000B3", 0 }, //dischargeModeBitAddress
            { "B001605", 0 }, //
            { "B001606", 0 }, //
            //スパッタ
            { "B002100", 0 }, //heartBeatAddress
            { "B002101", 0 }, //loaderQRReadCompleteBitAddress
            { "B002102", 0 }, //loaderMachineSelectCompleteBitAddress
            { "B002103", 0 }, //loaderWorkCompleteBitAddress
            { "B002104", 0 }, //loaderWorkErrorBitAddress
            { "B002105", 0 }, //unloaderQRReadCompleteBitAddress
            { "B002106", 0 }, //unloaderWorkCompleteBitAddress
            { "B002107", 0 }, //unloaderWorkErrorBitAddress
            { "B002108", 0 }, //relateLotTrayFunctionBitAddress
            { "B002109", 0 }, //machineReadyBitAddress
            { "B00210A", 0 }, //loaderReqBitAddress
            { "B00210B", 0 }, //unloaderReqBitAddress
            { "B00210C", 0 }, //inputForbiddenBitAddress
            { "B00210D", 0 }, //dischargeModeBitAddress
            //MD
            { "B001701", 0 }, //machineReadyBitAddress
            { "B001703", 0 }, //loaderReqBitAddress
            { "B001706", 0 }, //unloaderReqBitAddress
            { "B001704", 0 }, //empMagUnloaderReqBitAddress
            { "B001705", 0 }, //empMagLoaderReqBitAddress
            { "B000094", 0 }, //inputForbiddenBitAddress
            { "B0000B4", 0 }, //dischargeModeBitAddress
            //{ "B0000B3", 0 }, //preInputDischargeModeBitAddress
            //ECK
            { "B001901", 0 }, //machineReadyBitAddress
            { "B000096", 0 }, //inputForbiddenBitAddress
            { "B0000B6", 0 }, //dischargeModeBitAddress
            { "B0002E2", 0 }, //dummyMagStationReqBitAddress
            { "B0002E3", 0 }, //dummyMagStationOutputReqBitAddress
            { "B001907", 0 }, //dummyMagReqBitAddress
            { "B001903", 0 }, //loaderReqBitAddress1
            { "B001904", 0 }, //loaderReqBitAddress2
            { "B001908", 0 }, //loaderReqBitAddress3
            { "B001909", 0 }, //loaderReqBitAddress4
            { "B00190C", 0 }, //loaderReqBitAddress5
            { "B00190D", 0 }, //loaderReqBitAddress6
            { "B001905", 0 }, //unloaderReqBitAddress1
            { "B001906", 0 }, //unloaderReqBitAddress2
            { "B00190A", 0 }, //unloaderReqBitAddress3
            { "B00190B", 0 }, //unloaderReqBitAddress4
            { "B00190E", 0 }, //unloaderReqBitAddress5
            { "B00190F", 0 }, //unloaderReqBitAddress6
            //BUFFER(1,2,3)
            { "B0002D8", 0 }, //machineReadyBitAddress
            { "B000020", 0 }, //clearMagazinesBitAddress
            { "B000350", 0 }, //loaderReqBitAddress1
            { "B000351", 0 }, //unloaderReqBitAddress1
            { "B000352", 0 }, //loaderReqBitAddress2
            { "B000353", 0 }, //unloaderReqBitAddress2
            { "B000354", 0 }, //loaderReqBitAddress3
            { "B000355", 0 }, //unloaderReqBitAddress3
            //MAG EX
            { "B0002DC", 0 }, //loaderReqBitAddress
            { "B0002DB", 0 }, //unloaderReqBitAddress
            { "B0000DF", 0 }, //clearMagazinesBitAddress
            { "B0002DD", 0 }, //empMagUnloaderReqBitAddress
            { "B0002DA", 0 }, //empMagUnloaderReqBitAddress
            { "B0000FD", 0 }, //magShiftEnableBitAddress
            { "B0000FC", 0 }, //unloaderMagReverseBitAddress
            { "B0000BA", 0 }, //dischargeModeProcBitAddress_16
            { "B0000BB", 0 }, //dischargeModeProcBitAddress_20
            //MDOVN
            { "B001410", 0 }, //machineReadyBitAddress
            { "B0000D5", 0 }, //clearMagazinesBitAddress
            { "B0000B7", 0 }, //dischargeModeBitAddress
            { "B001412", 0 }, //loaderReqBitAddressList_addKey1
            { "B001413", 0 }, //loaderReqBitAddressList_addKey2
            { "B001415", 0 }, //unloaderReqBitAddressList_addKey1
            { "B001416", 0 }, //unloaderReqBitAddressList_addKey2
            { "B000830", 0 }, //changeProfileInterlockBitAddress
        };
        Dictionary<string, int> initKeylcBitData = new Dictionary<string, int>
        {
            //AVI
            { "B00000", 0 },
            { "DM20040", 1 },
            { "DM20052", 0 },
            { "DM20099", 0 },
            //MD
            { "DM30060", 0 },
            { "B0FF0", 0 },
            { "DM30050", 0 },
            { "DM30051", 0 },
            { "DM30052", 0 },
            { "DM30053", 0 },
            { "DM30054", 0 },
            { "DM30055", 0 },
            { "DM30056", 0 },
            { "DM30057", 0 },
            //ECK
            { "DM32050", 0 },
            { "DM32051", 0 },
            { "DM32052", 0 },
            { "DM32053", 0 }
        };
        Dictionary<string, string> initPlcWordData = new Dictionary<string, string>
        {
            //ロボットQRリーダー
            { "W0000B0", "\0" }, //
            { "W0000B1", "\0" }, //
            { "W0000B2", "\0" }, //
            { "W0000B3", "\0" }, //
            { "W0000B4", "\0" }, //
            { "W0000B5", "\0" }, //
            { "W0000B6", "\0" }, //
            { "W0000B7", "\0" }, //
            { "W0000B8", "\0" }, //
            { "W0000B9", "\0" }, //
            { "W0000BA", "\0" }, //
            { "W0000BB", "\0" }, //
            { "W0000BC", "\0" }, //
            { "W0000BD", "\0" }, //
            { "W0000BE", "\0" }, //
            { "W0000BF", "\0" }, //
            { "W0000C0", "\0" }, //
            { "W0000C1", "\0" }, //
            { "W0000C2", "\0" }, //
            { "W0000C3", "\0" }, //
            { "W0000C4", "\0" }, //
            { "W0000C5", "\0" }, //
            { "W0000C6", "\0" }, //
            { "W0000C7", "\0" }, //
            { "W0000C8", "\0" }, //
            { "W0000C9", "\0" }, //
            { "W0000CA", "\0" }, //
            { "W0000CB", "\0" }, //
        };
        Dictionary<string, int> initPlcDecimalWordData = new Dictionary<string, int>
        {
            //MDマガジンバッファ数
            { "W001721", 2 }, //
            { "W001722", 2 }, //
            { "W001723", 2 }, //
        };
        Dictionary<string, int> plcBitData;
        Dictionary<string, string> plcWordData;
        // ロボット (Carrier)
        Robot Robot = new Robot();
        // ブリッジ (Carrier)
        Bridge Bridge = new Bridge();
        // オーブンプロファイル (Machine)
        OvenProfile OvenProfile = new OvenProfile();
        // 途中払出コンベア
        DischargeConv DCConv_01 = new DischargeConv();
        // 完成品払出コンベア
        MAPCompltDischargeConv MCDCConv_01 = new MAPCompltDischargeConv();
        // アオイマガジン排出コンベア
        MAPBoardDischargeConv AoiDCConv_01 = new MAPBoardDischargeConv();
        // 途中投入コンベヤ
        NichiaConv NicConv_01 = new NichiaConv();
        // 空マガジン投入コンベヤ
        KaraConv KraConv_01 = new KaraConv();
        // MAP基板マガジン投入コンベヤ
        AoiConv AoiConv_01 = new AoiConv();
        // ライン間ブリッジ
        LineBridge LineBridge_01 = new LineBridge()
        {
            //Bit Props
            machineReadyBitAddress = "B000340",
            loaderReqBitAddress = "B000342",
            unloaderReqBitAddress = "B000343",
            empMagUnloaderReqBitAddress = "B000344",
            empMagInputBitAddress = "B0000FA",
            clearMagazinesBitAddress = "B00001F",
            //Word Props
            prioritySettingAddress = "W0000A8",
        };
        // ダイボンド1号機
        DieBonder M1DB_01 = new DieBonder()
        {
            macname = "ダイボンド1号機(ZE用)",
            macno = "101331",
            macLogOriginFolder = "C:\\QCIL\\MacLogOrigin\\DBZ\\",
            //Bit Props
            machineReadyBitAddress = "B000101",
            loaderReqBitAddress = "B000103",
            unloaderReqBitAddress = "B000106",
            clearMagazinesBitAddress = "B0000C0",
            waferBitAddressStart = "B000108",
            startWorkTableSensorAddress = "B000107",
            waferChangerChangeBitAddress = "B00011B",
            empMagUnloaderReqBitAddress = "B000104",
            empMagLoaderReqBitAddress = "B000105",
            inputForbiddenBitAddress = "B000080",
            dischargeModeBitAddress = "B0000A0",
            secondLoaderReqBitAddress = "B00011D",
            secondEmpMagLoaderReqBitAddress = "B00011F",
            //Word Props
            prioritySettingAddress = "W000080",
        };
        // ダイボンド3号機
        DieBonder M1DB_03 = new DieBonder()
        {
            macname = "ダイボンド3号機(ZE用)",
            macno = "101333",
            macLogOriginFolder = "C:\\QCIL\\MacLogOrigin\\DBZ\\",
            //Bit Props
            machineReadyBitAddress = "B000141",
            loaderReqBitAddress = "B000143",
            unloaderReqBitAddress = "B000146",
            clearMagazinesBitAddress = "B0000C2",
            waferBitAddressStart = "B000148",
            startWorkTableSensorAddress = "B000147",
            waferChangerChangeBitAddress = "B00015B",
            empMagUnloaderReqBitAddress = "B000144",
            empMagLoaderReqBitAddress = "B000145",
            inputForbiddenBitAddress = "B000082",
            dischargeModeBitAddress = "B0000A0",
            secondLoaderReqBitAddress = "B00015D",
            secondEmpMagLoaderReqBitAddress = "B00015F",
            //Word Props
            prioritySettingAddress = "W000082",
        };
        Oven DBOVEN_01 = new Oven()
        {
            macname = "ダイボンドオーブン1号機",
            machineReadyBitAddress = "B001210",
            clearMagazinesBitAddress = "B0000D1",
            inputForbiddenBitAddress = "B00008A",
            dischargeModeBitAddress = "B0000AA",
            loaderReqBitAddressList_addKey1 = "B001212",
            loaderReqBitAddressList_addKey2 = "B001213",
            unloaderReqBitAddressList_addKey1 = "B001215",
            unloaderReqBitAddressList_addKey2 = "B001216",
            changeProfileInterlockBitAddress = "B000630",
            //Word Props
            prioritySettingAddress = "W00008A",
            workStartTimeAddress = "W001201",
            workCompleteTimeAddress = "W001207",
            profileStatusAddress = "W001200",
            currentProfileWordAddress = "W0012A0",
            ovenProcessStatusWordAddress = "W00120F"
        };
        Oven DBOVEN_07 = new Oven()
        {
            macname = "ダイボンドオーブン7号機",
            machineReadyBitAddress = "B001240",
            clearMagazinesBitAddress = "B000017",
            inputForbiddenBitAddress = "B00008A",
            dischargeModeBitAddress = "B0000AA",
            loaderReqBitAddressList_addKey1 = "B001242",
            loaderReqBitAddressList_addKey2 = "B001243",
            unloaderReqBitAddressList_addKey1 = "B001245",
            unloaderReqBitAddressList_addKey2 = "B001246",
            changeProfileInterlockBitAddress = "B000636",
            //Word Props
            prioritySettingAddress = "W000004",
            workStartTimeAddress = "W001261",
            workCompleteTimeAddress = "W001267",
            profileStatusAddress = "W001260",
            currentProfileWordAddress = "W0012A6",
            ovenProcessStatusWordAddress = "W00126F"
        };
        // ダイボンド4号機
        DieBonder DB_04 = new DieBonder()
        {
            macname = "ダイボンド4号機(LED用)",
            macno = "104334",
            macLogOriginFolder = "C:\\QCIL\\MacLogOrigin\\DBL\\",
            //Bit Props
            machineReadyBitAddress = "B000161",
            loaderReqBitAddress = "B000163",
            unloaderReqBitAddress = "B000166",
            clearMagazinesBitAddress = "B0000C3",
            waferBitAddressStart = "B000168",
            startWorkTableSensorAddress = "B000167",
            waferChangerChangeBitAddress = "B00017B",
            empMagUnloaderReqBitAddress = "B000164",
            empMagLoaderReqBitAddress = "B000165",
            inputForbiddenBitAddress = "B000083",
            dischargeModeBitAddress = "B0000A4",
            secondLoaderReqBitAddress = "B00017D",
            secondEmpMagLoaderReqBitAddress = "B00017F",
            //Word Props
            prioritySettingAddress = "W000083",
        };
        // WBプラズマ洗浄機
        MultiLoadPlasma WBPL_01 = new MultiLoadPlasma()
        {
            macname = "WBプラズマ洗浄機"
        };
        // ワイヤーボンダ―
        WireBonder WB_01 = new WireBonder()
        {
            macname = "ワイヤーボンド1号機",
            equipment_no = "TSWB007",
            macno = "109321",
            //Bit Props
            machineReadyBitAddress = "B000241",
            loaderReqBitAddress = "B000243",
            unloaderReqBitAddress = "B000246",
            clearMagazinesBitAddress = "B0000CA",
            empMagUnloaderReqBitAddress = "B000244",
            empMagLoaderReqBitAddress = "B000245",
            inputForbiddenBitAddress = "B00008E",
            dischargeModeBitAddress = "B0000AE",
            secondLoaderReqBitAddress = "B00024D",
            secondEmpMagLoaderReqBitAddress = "B00024F",
            inspectProcNo = 10,
            mmFilePath = @"",
            //Word Props
            prioritySettingAddress = "W00008E",
        };
        WireBonder WB_07 = new WireBonder()
        {
            macname = "ワイヤーボンド7号機",
            macno = "109327",
            //Bit Props
            machineReadyBitAddress = "B000301",
            loaderReqBitAddress = "B000303",
            unloaderReqBitAddress = "B000306",
            clearMagazinesBitAddress = "B000011",
            empMagUnloaderReqBitAddress = "B000304",
            empMagLoaderReqBitAddress = "B000305",
            inputForbiddenBitAddress = "B00009C",
            dischargeModeBitAddress = "B000001",
            secondLoaderReqBitAddress = "B00030D",
            secondEmpMagLoaderReqBitAddress = "B00030F",
            inspectProcNo = 10,
            mmFilePath = @"",
            //Word Props
            prioritySettingAddress = "W0000A5",
        };
        //外観検査機1号機
        Inspector AVI_01 = new Inspector()
        {
            macname = "外観検査機1号機",
            macno = "110302",
        };
        Sputter SUP_01 = new Sputter()
        {
            macname = "スパッタ2号機",
            macno = "123301",
        };
        Mold MD_01 = new Mold()
        {
            macname = "モールド1号機",
            macno = "112311",
            ProgramName = "CL-A160-1W9",
            //Bit Props
            machineReadyBitAddress = "B001701",
            loaderReqBitAddress = "B001703",
            unloaderReqBitAddress = "B001706",
            empMagUnloaderReqBitAddress = "B001704",
            empMagLoaderReqBitAddress = "B001705",
            inputForbiddenBitAddress = "B000094",
            dischargeModeBitAddress = "B0000B4",
            preInputDischargeModeBitAddress = "B0000B3",
            //Word Props
            workCompleteTimeAddress = "W001707",
            workStartTimeAddress = "W001701",
            prioritySettingAddress = "W000094",
            lMagazineAddress = "W001718",
            ulMagazineAddress = "W001710",
            eulMagazineAddress = "W001780",
            elMagazineCountAddress = "W001721",
            lMagazineCountAddress = "W001720",
            eulMagazineCountAddress = "W001722",
            ulMagazineCountAddress = "W001723",
        };
        ECK ECK_01 = new ECK()
        {
            macname = "遠心沈降1号機",
            macno = "113302"
        };
        GeneralBuffer GB_01 = new GeneralBuffer()
        {
            macname = "バッファ1",
            prioritySettingAddress = "W000012",
            machineReadyBitAddress = "B0002D8",
            loaderReqBitAddress = "B000350",
            unloaderReqBitAddress = "B000351",
            clearMagazinesBitAddress = "B000020"
        };
        GeneralBuffer GB_02 = new GeneralBuffer()
        {
            macname = "バッファ2",
            prioritySettingAddress = "W000012",
            machineReadyBitAddress = "B0002D8",
            loaderReqBitAddress = "B000352",
            unloaderReqBitAddress = "B000353",
            clearMagazinesBitAddress = "B000020"
        };
        GeneralBuffer GB_03 = new GeneralBuffer()
        {
            macname = "バッファ3",
            prioritySettingAddress = "W000012",
            machineReadyBitAddress = "B0002D8",
            loaderReqBitAddress = "B000354",
            unloaderReqBitAddress = "B000355",
            clearMagazinesBitAddress = "B000020"
        };
        MagExchanger MEX_01 = new MagExchanger();
        Oven MDOVN_01 = new Oven()
        {
            macname = "モールドオーブン1号機",
            machineReadyBitAddress = "B001410",
            clearMagazinesBitAddress = "B0000D5",
            inputForbiddenBitAddress = "B00008A",
            dischargeModeBitAddress = "B0000B7",
            loaderReqBitAddressList_addKey1 = "B001412",
            loaderReqBitAddressList_addKey2 = "B001413",
            unloaderReqBitAddressList_addKey1 = "B001415",
            unloaderReqBitAddressList_addKey2 = "B001416",
            changeProfileInterlockBitAddress = "B000830",
            //Word Props
            prioritySettingAddress = "W000097",
            workStartTimeAddress = "W001401",
            workCompleteTimeAddress = "W001407",
            profileStatusAddress = "W001400",
            currentProfileWordAddress = "W0014A0",
            ovenProcessStatusWordAddress = "W00140F"
        };

        //DBクラス実体
        //Arms
        TnLot tnlot = new TnLot();
        TnTran tntran = new TnTran();
        TnMag tnmag = new TnMag();
        TnLotLog tnlotlog = new TnLotLog();
        TnRestrict tnrestrict = new TnRestrict();
        //Lens
        TnLot_Lens tnlotlens = new TnLot_Lens();
        TnTran_Lens tntranlens = new TnTran_Lens();
        //Qcil
        TnLog_Qcil tnlogqcil = new TnLog_Qcil();
        TnLott_Qcil tnlottqcil = new TnLott_Qcil();
        TnQcnr_Qcil tnqcnrqcil = new TnQcnr_Qcil();
        //自動化
        bool AutoRunLock = false;
        //etc.
        public Dictionary<string, IDebugMac> DebugMacs;
        public Dictionary<string, string> ProcessName;
        string workheader = string.Empty;
        string EicsParam_TypeCode = "CL-A160-1W9-S4";

        public FrmFakeNline()
        {
            plcBitData = new Dictionary<string, int>(initPlcBitData);
            plcWordData = new Dictionary<string, string>(initPlcWordData);

            InitializeComponent();

            //var msg = "";
            //M1DB_03.getMacFolderPath("in", ref msg);
            //ConsoleShow(msg);

            // IDebugMacで装置リスト化
            DebugMacs = new Dictionary<string, IDebugMac>()
            {
                { DCConv_01.macname, DCConv_01 },
                { MCDCConv_01.macname, MCDCConv_01 },
                { AoiDCConv_01.macname, AoiDCConv_01 },
                { NicConv_01.macname, NicConv_01 },
                { KraConv_01.macname, KraConv_01 },
                { AoiConv_01.macname, AoiConv_01 },
                { LineBridge_01.macname, LineBridge_01 },
                { M1DB_01.macname, M1DB_01 },
                { M1DB_03.macname, M1DB_03 },
                { DBOVEN_01.macname, DBOVEN_01 },
                { DBOVEN_07.macname, DBOVEN_07 },
                { DB_04.macname, DB_04 },
                { WBPL_01.macname, WBPL_01 },
                { WB_01.macname, WB_01 },
                { WB_07.macname, WB_07 },
                { AVI_01.macname, AVI_01 },
                { SUP_01.macname, SUP_01 },
                { MD_01.macname, MD_01 },
                { ECK_01.macname, ECK_01 },
                { GB_01.macname, GB_01 },
                { GB_02.macname, GB_02 },
                { GB_03.macname, GB_03 },
                { MEX_01.macname, MEX_01 },
                { MDOVN_01.macname, MDOVN_01 },
            };

            // プロセス定義
            ProcessName = new Dictionary<string, string>
            {
                { "1", "Zeダイボンド@tb_M1DB" },
                { "2", "Zeオーブン@tb_DBOVEN" },
                { "4", "LEDダイボンド@tb_LEDDB" },
                { "5", "硬化前プラズマ@tb_DBPL" },
                { "7", "DBオーブン@tb_DBOVEN" },
                { "8", "WB前プラズマ@tb_WBPL" },
                { "9", "ワイヤーボンド@tb_WB" },
                { "10", "検査機@tb_AVI" },
                { "22", "WBオーブン@tb_WBOVN" },
                { "23", "スパッタ@tb_SUP" },
                { "12", "MD作業@tb_MD" },
                { "13", "遠心沈降作業@tb_ECK" },
                { "18", "マガジン交換前待機_@tb_GBMEX" },
                { "14", "MD硬化作業@tb_MDOVN" },
                { "15", "ダイシング作業@tb_DC" },
                { "24", "ダム/ダム潰し/ﾌﾟﾗｽﾞﾏ@tb_DAM" },
            };

            // コンボボックスcb_naacnameに装置名称追加
            foreach (var macvp in DebugMacs)
            {
                cb_macname.Items.Add(macvp.Key);
            }
            cb_macname.SelectedIndex = (cb_macname.Items.Count - 1);

            // ProcNoコントロール
            cb_procno.SelectedIndex = 0;

            // Tab内オブジェクト初期化
            //Ze
            cb_zedbno.SelectedIndex = 1;
            cmb_zdbpram.SelectedIndex = 0;
            //ovn
            cb_dbovnno.SelectedIndex = 1;
            cb_dbovn_procno.SelectedIndex = 0;
            cb_dbovnProfile.SelectedIndex = 0;
            //led
            cb_leddbno.SelectedIndex = 0;
            cb_leddb_fromDBOVNno.SelectedIndex = 1;
            cb_leddb_fromDBOVNSlot.SelectedIndex = 0;
            cmb_leddbparam.SelectedIndex = 0;
            //dbpl
            //wbpl
            cb_wbpl_fromDBOVNno.SelectedIndex = 1;
            cb_wbpl_fromDBOVNSlot.SelectedIndex = 0;
            //wb
            cb_wbno.SelectedIndex = 1;
            cmb_eicslog.SelectedIndex = 0;
            //avi
            cb_avi_fromwb.SelectedIndex = 1;
            //md
            cb_mb_macno.SelectedIndex = 0;
            //eck
            cb_eck_frommdno.SelectedIndex = 0;
            cb_eck_lrba.SelectedIndex = 0;
            cb_eck_ulrba.SelectedIndex = 0;
            //gb,mex
            cb_gb_lrba_bfno.SelectedIndex = 0;
            cb_gb_ulrba_bfno.SelectedIndex = 0;
            //mdovn
            cb_mdovn_macno.SelectedIndex = 0;
            cb_mdovnProfile.SelectedIndex = 0;
            //dam
            cb_dam_procno.SelectedIndex = 0;
        }

        // Combobox router
        private DieBonder ZeDBComboRouter(string macno)
        {
            switch (macno)
            {
                case "01":
                    return M1DB_01;
                case "03":
                    return M1DB_03;
            }
            return new DieBonder();
        }

        private Oven DBOvnComboRouter(string macno)
        {
            switch (macno)
            {
                case "01":
                    return DBOVEN_01;
                case "07":
                    return DBOVEN_07;
            }
            return new Oven();
        }

        private WireBonder WBComboRouter(string macno)
        {
            switch (macno)
            {
                case "01":
                    return WB_01;
                case "07":
                    return WB_07;
            }
            return new WireBonder();
        }

        private DieBonder LEDDBComboRouter(string macno)
        {
            switch (macno)
            {
                case "04":
                    return DB_04;
            }
            return new DieBonder();
        }

        private Mold MDComboRouter(string macno)
        {
            switch (macno)
            {
                case "01":
                    return MD_01;
            }
            return new Mold();
        }

        private Oven MDOVNComboRouter(string macno)
        {
            switch (macno)
            {
                case "01":
                    return MDOVN_01;
            }
            return new Oven();
        }

        private void FrmFakeNline_Load(object sender, EventArgs e)
        {
            // メモリ初期化
            TestMemAndIni();
            // 全ての設備のmachineReadyBitAddressをON
            Cnt_isReadyOnAll(true);
        }

        //
        // PLCデバイスアクセス
        //

        private bool SetBit(string hexAddressWithDeviceNM, string data, bool fmode = false)
        {
            if (!DebugMode)
            {
                try
                {
                    Plc.SetBit(hexAddressWithDeviceNM, 1, data);
                    var retbit = Plc.GetBit(hexAddressWithDeviceNM);
                    if (retbit != data)
                    {
                        ConsoleShow("PLCへのBIT書き込みができていません");
                        return false;
                    }
                    else
                    {
                        if (!fmode) plcBitData[hexAddressWithDeviceNM] = int.Parse(retbit);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    ConsoleShow("PLCへのBIT書き込み時に例外発生がありました");
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private string GetBit(string hexAddressWithDeviceNM)
        {
            if (!DebugMode)
            {
                try
                {
                    return Plc.GetBit(hexAddressWithDeviceNM);

                }
                catch (Exception e)
                {
                    ConsoleShow("PLCへのBIT読み込み時に例外発生：" + e.ToString());
                    return String.Empty;
                }
            }
            else
            {
                return "-1";
            }
        }

        private bool SetString(string hexAddressWithDeviceNM, string data)
        {
            if (!DebugMode)
            {
                try
                {
                    Plc.SetString(hexAddressWithDeviceNM, data);
                    var retwd = GetWord(hexAddressWithDeviceNM, data.Length);
                    if (retwd.Contains(data))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    ConsoleShow("PLCへのWord読み込み時に例外発生：" + e.ToString());
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private string GetWord(string hexAddressWithDeviceNM, int length)
        {
            if (!DebugMode)
            {
                try
                {
                    if (length > 0)
                        return Plc.GetWord(hexAddressWithDeviceNM, length);
                    else
                        return String.Empty;

                }
                catch (Exception e)
                {
                    ConsoleShow("PLCへのWord読み込み時に例外発生：" + e.ToString());
                    return String.Empty;
                }
            }
            else
            {
                return String.Empty;
            }
        }

        private void SetWordAsDecimalData(string hexAddressWithDeviceNM, int data)
        {
            if (!DebugMode)
            {

                Plc.SetWordAsDecimalData(hexAddressWithDeviceNM, data);
            }
            else
            {
                return;
            }
        }

        //時間書き込み
        public void SetDateTime2Word(string TimeAddress, DateTime dt)
        {
            var plcaddress = Convert.ToInt32(TimeAddress.Replace("W", ""), 16);
            //year
            SetWordAsDecimalData("W00" + (plcaddress).ToString("x4"), dt.Year);
            ////month
            SetWordAsDecimalData("W00" + (plcaddress + 1).ToString("x4"), dt.Month);
            ////day
            SetWordAsDecimalData("W00" + (plcaddress + 2).ToString("x4"), dt.Day);
            ////hour
            SetWordAsDecimalData("W00" + (plcaddress + 3).ToString("x4"), dt.Hour);
            ////minite
            SetWordAsDecimalData("W00" + (plcaddress + 4).ToString("x4"), dt.Minute);
            ////second
            SetWordAsDecimalData("W00" + (plcaddress + 5).ToString("x4"), dt.Second);
        }

        //
        // PLCメモリ強制同期ボタン処理
        //

        private void btn_plcWriteAllBitDev_Click(object sender, EventArgs e)
        {
            ForceWritePlcBitData();
        }

        private void btn_plcReadAllBitDev_Click(object sender, EventArgs e)
        {
            ForceReadPlcBitData();
        }

        private void ForceWritePlcBitData()
        {
            try
            {
                //
                // BitData Force Write PlcBitData
                //
                ConsoleShow("【ビットデータ強制書込】");
                foreach (var pbd in plcBitData)
                {
                    if (!SetBit(pbd.Key, pbd.Value.ToString(), true))
                    {
                        throw new Exception();
                    }
                    var retbit = GetBit(pbd.Key);
                    ConsoleShow(pbd.Key + " " + pbd.Value.ToString() + $"⇒ {retbit}");
                }
            }
            catch (Exception ex)
            {
                ConsoleShow("【Bitデータの強制書込が失敗しました】" + ex);
            }
        }

        private void ForceReadPlcBitData()
        {
            try
            {
                //
                // BitData Force Read PlcBitData
                //
                ConsoleShow("【ビットデータ強制読込】");
                foreach (var pbd in initPlcBitData)
                {
                    var retbit = GetBit(pbd.Key);
                    if (string.IsNullOrEmpty(retbit))
                    {
                        throw new Exception();
                    }
                    plcBitData[pbd.Key] = int.Parse(retbit);
                    ConsoleShow(pbd.Key + " " + $"⇒{retbit}");
                }
            }
            catch (Exception ex)
            {
                ConsoleShow("【Bitデータの強制読込が失敗しました】" + ex);
            }
        }

        // 
        // マガジン投入ボタン処理
        //

        private void bt_conv_aoiMag_Click(object sender, EventArgs e)
        {
            if (!readRbMagQr(txt_rbmagQrr.Text))
            {
                ConsoleShow("ロボットQRが書き込めませんでした");
                return;
            }
            EntConvMagazine("aoi_on", false);
            bt_conv_aoiMag_on.Enabled = false;
            bt_conv_aoiMag_on.Text = "投入中";
        }

        private void bt_conv_nichiaMag_Click(object sender, EventArgs e)
        {
            if (!NicConvQR(txt_NicMagNo.Text))
            {
                ConsoleShow("途中投入QRが書き込めませんでした");
                return;
            }
            EntConvMagazine("nichia_on", false);
            bt_conv_nichiaMag_on.Enabled = false;
            bt_conv_nichiaMag_on.Text = "投入中";
            SetBit(NicConv_01.magazineArriveBitAddress, "1");
        }

        public bool NicConvQR(string magno)
        {
            if (!SetString(NicConv_01.MagNo, magno))
            {
                return false;
            }
            else return true;
        }

        private void bt_conv_empMag_Click(object sender, EventArgs e)
        {
            if (!readRbMagQr(txt_rbmagQrr.Text))
            {
                ConsoleShow("ロボットQRが書き込めませんでした");
                return;
            }
            EntConvMagazine("kara_on", false);
            bt_conv_empMag_on.Enabled = false;
            bt_conv_empMag_on.Text = "投入中";
        }

        private void bt_conv_empMag_off_Click(object sender, EventArgs e)
        {
            EntConvMagazine("kara_off", false);
            bt_conv_empMag_on.Enabled = true;
        }

        private void bt_conv_aoiMag_off_Click(object sender, EventArgs e)
        {
            EntConvMagazine("aoi_off", false);
            bt_conv_aoiMag_on.Enabled = true;
        }

        private void bt_conv_nichiaMag_off_Click(object sender, EventArgs e)
        {
            EntConvMagazine("nichia_off", false);
            bt_conv_nichiaMag_on.Enabled = true;
            SetBit(NicConv_01.magazineArriveBitAddress, "0");
        }

        private void lb_empmag_Click(object sender, EventArgs e)
        {
            ConsoleShow(KraConv_01.empMagUnloaderReqBitAddress);
        }

        private void lb_aoimag_Click(object sender, EventArgs e)
        {
            ConsoleShow(AoiConv_01.empMagUnloaderReqBitAddress);
        }

        private void lb_nichiamag_Click(object sender, EventArgs e)
        {
            ConsoleShow(NicConv_01.unloaderReqBitAddress);
        }

        private void EntConvMagazine(string conv, bool msgoff = true)
        {
            string msg = "";
            if (SetMagazineToConv(conv, ref msg))
            {
                if (!msgoff) ConsoleShow(msg);
            }
            else
            {
                ConsoleShow(msg);
            }
        }

        public bool SetMagazineToConv(string conv, ref string msg)
        {
            try
            {
                switch (conv)
                {
                    case "aoi_on":
                        msg = "アオイいれたよ";
                        if (SetBit(AoiConv_01.empMagUnloaderReqBitAddress, Mitsubishi.BIT_ON))
                        {
                            ConsoleShow($"{AoiConv_01.empMagUnloaderReqBitAddress}: ON>>");
                        }
                        else
                        {
                            throw new Exception();
                        }
                        break;
                    case "kara_on":
                        msg = "空マガいれたよ";
                        if (SetBit(KraConv_01.empMagUnloaderReqBitAddress, Mitsubishi.BIT_ON))
                        {
                            ConsoleShow($"{KraConv_01.empMagUnloaderReqBitAddress}: ON>>");
                        }
                        else
                        {
                            throw new Exception();
                        }
                        break;
                    case "nichia_on":
                        msg = "Nマガいれたよ";
                        if (SetBit(NicConv_01.unloaderReqBitAddress, Mitsubishi.BIT_ON))
                        {
                            ConsoleShow($"{NicConv_01.unloaderReqBitAddress}: ON>>");
                        }
                        else
                        {
                            throw new Exception();
                        }
                        break;
                    case "aoi_off":
                        msg = "アオイ取ったよ";
                        if (SetBit(AoiConv_01.empMagUnloaderReqBitAddress, Mitsubishi.BIT_OFF))
                        {
                            ConsoleShow($"{AoiConv_01.empMagUnloaderReqBitAddress}: OFF>>");
                        }
                        else
                        {
                            throw new Exception();
                        }
                        break;
                    case "kara_off":
                        msg = "空マガ取ったよ";
                        if (SetBit(KraConv_01.empMagUnloaderReqBitAddress, Mitsubishi.BIT_OFF))
                        {
                            ConsoleShow($"{KraConv_01.empMagUnloaderReqBitAddress}: OFF>>");
                        }
                        else
                        {
                            throw new Exception();
                        }
                        break;
                    case "nichia_off":
                        msg = "Nマガ取ったよ";
                        if (SetBit(NicConv_01.unloaderReqBitAddress, Mitsubishi.BIT_OFF))
                        {
                            ConsoleShow($"{NicConv_01.unloaderReqBitAddress}: OFF>>");
                        }
                        else
                        {
                            throw new Exception();
                        }
                        break;
                    default:
                        msg = "だめだめ";
                        return false;
                }
                return true;
            }
            catch (Exception e)
            {
                msg = "Exceptionですよ、シチズンさんっ";
                return false;
            }
        }

        //
        // 全設備machineReadyBitAddress一括処理
        //

        private void Cnt_isReadyOnAll(bool BIT_ON)
        {
            try
            {
                string bitonoff;
                string bitstr;
                if (BIT_ON)
                {
                    bitonoff = "1"; bitstr = "ON";
                }
                else
                {
                    bitonoff = "0"; bitstr = "OFF";
                }
                foreach (var macVp in DebugMacs)
                {
                    SetBit(macVp.Value.machineReadyBitAddress, bitonoff);
                    plcBitData[macVp.Value.machineReadyBitAddress] = int.Parse(bitonoff);
                    ConsoleShow(macVp.Key + $"：isReady{macVp.Value.machineReadyBitAddress}⇒ON");
                }
                if (BIT_ON)
                {
                    btn_mrba_on.Enabled = false;
                    btn_mrba_off.Enabled = true;
                }
                else
                {
                    btn_mrba_on.Enabled = true;
                    btn_mrba_off.Enabled = false;
                }

                ConsoleShow($"【isReadyを全て{bitstr}にしました】");
            }
            catch (Exception ex)
            {
                ConsoleShow("【PLCの動作に異常があります】");
            }

        }


        //
        // メモリ初期化処理
        //

        private void btn_TestMemAndIni_Click(object sender, EventArgs e)
        {
            TestMemAndIni();
        }

        private bool TestMemAndIni()
        {
            try
            {
                //
                // BitData Initial Test
                //
                ConsoleShow("【ビットデータテスト／初期化】");
                foreach (KeyValuePair<string, int> pbd in initPlcBitData)
                {
                    if (!SetBit(pbd.Key, pbd.Value.ToString()))
                    {
                        throw new Exception();
                    }
                    var retbit = GetBit(pbd.Key);
                    ConsoleShow(pbd.Key + " " + pbd.Value + $": {retbit}");
                }

                foreach (KeyValuePair<string, int> pbd in initKeylcBitData)
                {
                    Keylc.SetBit(pbd.Key, 1, pbd.Value.ToString());
                }

                //
                // WordData Initial Test
                //
                //ConsoleShow("【ワードデータテスト／初期化】");
                //foreach (KeyValuePair<string, string> pwd in initPlcWordData)
                //{
                //    if(!SetString(pwd.Key, pwd.Value))
                //    {
                //        throw new Exception();
                //    }
                //    var retword = GetWord(pwd.Key, 1);
                //    ConsoleShow(pwd.Key + " " + pwd.Value + $": {retword}");
                //}

                ////
                //// Magazine No Read/Write Test
                ////
                //// MagNo Sample
                //ConsoleShow("【マガジンNo書き込み・読取テスト】");
                //Plc.SetString("B000000", "C30 M12345678");

                //// string GetMagazineNo(string hexAddressWithDeviceNm);
                //string magno = Plc.GetMagazineNo("B000000");
                //ConsoleShow("B000000 " + $": {magno} ");
                //// string GetMagazineNo(string hexAddressWithDeviceNm, int wordLength);
                //magno = Plc.GetMagazineNo("B000000", 10);
                //ConsoleShow("B000000 " + $": {magno} ");
                //// string GetMagazineNo(string hexAddressWithDeviceNm, bool notDevided);
                ////magno = Plc.GetMagazineNo("W000000", true);
                ////ConsoleShow("W000000 " + $": {magno} ");
                ////magno = Plc.GetMagazineNo("W000000", false);
                ////ConsoleShow("W000000 " + $": {magno} ");
                //// string GetMagazineNo(string[] plcResponseBitArray, bool notDevided);
                ///

                // ロボットマガジンQRを初期化
                ConsoleShow("【ロボットマガジン／初期化】");
                if (!SetString(Robot.robotQRWordAddressStart, ""))
                {
                    throw new Exception();
                }

                // ワードデバイス初期化
                // MDバッファ
                ConsoleShow("【ワードデバイス(Decimal値)初期化】");
                foreach (KeyValuePair<string, int> pbd in initPlcDecimalWordData)
                {
                    SetWordAsDecimalData(pbd.Key, pbd.Value);
                }

                ConsoleShow("【初期化完了】");
                return true;
            }
            catch (Exception ex)
            {
                ConsoleShow("【初期化失敗】" + ex);
                return false;
            }

        }

        //
        // 共通デバイス処理
        //

        private void cb_macname_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            updateCommonFunctionSignalBtns();
        }

        public void updateCommonFunctionSignalBtns()
        {
            var mac = DebugMacs[cb_macname.Text];
            //machineReadyBitAddress
            if (string.IsNullOrEmpty(mac.machineReadyBitAddress))
            {
                btn_mrba_on.Enabled = false;
                btn_mrba_off.Enabled = false;
            }
            else
            {
                btn_mrba_on.Enabled = (plcBitData[MacComboRouter().machineReadyBitAddress] == 0);
                btn_mrba_off.Enabled = (plcBitData[MacComboRouter().machineReadyBitAddress] != 0);
            }
            //loaderReqBitAddress
            if (string.IsNullOrEmpty(mac.loaderReqBitAddress))
            {
                btn_lrba_on.Enabled = false;
                btn_lrba_off.Enabled = false;
            }
            else
            {
                btn_lrba_on.Enabled = (plcBitData[MacComboRouter().loaderReqBitAddress] == 0);
                btn_lrba_off.Enabled = (plcBitData[MacComboRouter().loaderReqBitAddress] != 0);
            }
            //unloaderReqBitAddress
            if (string.IsNullOrEmpty(mac.unloaderReqBitAddress))
            {
                btn_urba_on.Enabled = false;
                btn_urba_off.Enabled = false;
            }
            else
            {
                btn_urba_on.Enabled = (plcBitData[MacComboRouter().unloaderReqBitAddress] == 0);
                btn_urba_off.Enabled = (plcBitData[MacComboRouter().unloaderReqBitAddress] != 0);
            }
            //clearMagazinesBitAddress
            if (string.IsNullOrEmpty(mac.clearMagazinesBitAddress))
            {
                btn_cmba_on.Enabled = false;
                btn_cmba_off.Enabled = false;
            }
            else
            {
                btn_cmba_on.Enabled = (plcBitData[MacComboRouter().clearMagazinesBitAddress] == 0);
                btn_cmba_off.Enabled = (plcBitData[MacComboRouter().clearMagazinesBitAddress] != 0);
            }
            //waferBitAddressStart
            if (string.IsNullOrEmpty(mac.waferBitAddressStart))
            {
                btn_wbas_on.Enabled = false;
                btn_wbas_off.Enabled = false;
            }
            else
            {
                btn_wbas_on.Enabled = (plcBitData[MacComboRouter().waferBitAddressStart] == 0);
                btn_wbas_off.Enabled = (plcBitData[MacComboRouter().waferBitAddressStart] != 0);
            }
            //startWorkTableSensorAddress
            if (string.IsNullOrEmpty(mac.startWorkTableSensorAddress))
            {
                btn_swtsa_on.Enabled = false;
                btn_swtsa_off.Enabled = false;
            }
            else
            {
                btn_swtsa_on.Enabled = (plcBitData[MacComboRouter().startWorkTableSensorAddress] == 0);
                btn_swtsa_off.Enabled = (plcBitData[MacComboRouter().startWorkTableSensorAddress] != 0);
            }
            //waferChangerChangeBitAddress
            if (string.IsNullOrEmpty(mac.waferChangerChangeBitAddress))
            {
                btn_wccba_on.Enabled = false;
                btn_wccba_off.Enabled = false;
            }
            else
            {
                btn_wccba_on.Enabled = (plcBitData[MacComboRouter().waferChangerChangeBitAddress] == 0);
                btn_wccba_off.Enabled = (plcBitData[MacComboRouter().waferChangerChangeBitAddress] != 0);
            }
            //empMagUnloaderReqBitAddress
            if (string.IsNullOrEmpty(mac.empMagUnloaderReqBitAddress))
            {
                btn_emurba_on.Enabled = false;
                btn_emurba_off.Enabled = false;
            }
            else
            {
                btn_emurba_on.Enabled = (plcBitData[MacComboRouter().empMagUnloaderReqBitAddress] == 0);
                btn_emurba_off.Enabled = (plcBitData[MacComboRouter().empMagUnloaderReqBitAddress] != 0);
            }
            //empMagLoaderReqBitAddress
            if (string.IsNullOrEmpty(mac.empMagLoaderReqBitAddress))
            {
                btn_emlrba_on.Enabled = false;
                btn_emlrba_off.Enabled = false;
            }
            else
            {
                btn_emlrba_on.Enabled = (plcBitData[MacComboRouter().empMagLoaderReqBitAddress] == 0);
                btn_emlrba_off.Enabled = (plcBitData[MacComboRouter().empMagLoaderReqBitAddress] != 0);
            }
            //inputForbiddenBitAddress
            if (string.IsNullOrEmpty(mac.inputForbiddenBitAddress))
            {
                btn_ifba_on.Enabled = false;
                btn_ifba_off.Enabled = false;
            }
            else
            {
                btn_ifba_on.Enabled = (plcBitData[MacComboRouter().inputForbiddenBitAddress] == 0);
                btn_ifba_off.Enabled = (plcBitData[MacComboRouter().inputForbiddenBitAddress] != 0);
            }
            //dischargeModeBitAddress
            if (string.IsNullOrEmpty(mac.dischargeModeBitAddress))
            {
                btn_dmba_on.Enabled = false;
                btn_dmba_off.Enabled = false;
            }
            else
            {
                btn_dmba_on.Enabled = (plcBitData[MacComboRouter().dischargeModeBitAddress] == 0);
                btn_dmba_off.Enabled = (plcBitData[MacComboRouter().dischargeModeBitAddress] != 0);
            }
            //secondLoaderReqBitAddress
            if (string.IsNullOrEmpty(mac.secondLoaderReqBitAddress))
            {
                btn_slrba_on.Enabled = false;
                btn_slrba_off.Enabled = false;
            }
            else
            {
                btn_slrba_on.Enabled = (plcBitData[MacComboRouter().secondLoaderReqBitAddress] == 0);
                btn_slrba_off.Enabled = (plcBitData[MacComboRouter().secondLoaderReqBitAddress] != 0);
            }
            //secondEmpMagLoaderReqBitAddress
            if (string.IsNullOrEmpty(mac.secondEmpMagLoaderReqBitAddress))
            {
                btn_semlrba_on.Enabled = false;
                btn_semlrba_off.Enabled = false;
            }
            else
            {
                btn_semlrba_on.Enabled = (plcBitData[MacComboRouter().secondEmpMagLoaderReqBitAddress] == 0);
                btn_semlrba_off.Enabled = (plcBitData[MacComboRouter().secondEmpMagLoaderReqBitAddress] != 0);
            }
        }

        private IDebugMac MacComboRouter()
        {
            //MessageBox.Show(DebugMacs[cb_macname.Text].macname);
            return DebugMacs[cb_macname.Text];
        }

        private void btn_mrba_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().machineReadyBitAddress, "1"))
            {
                plcBitData[MacComboRouter().machineReadyBitAddress] = 1;
                btn_mrba_on.Enabled = false;
                btn_mrba_off.Enabled = true;
                ConsoleShow(MacComboRouter().machineReadyBitAddress + "⇒ON");
            }
        }

        private void btn_mrba_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().machineReadyBitAddress, "0"))
            {
                plcBitData[MacComboRouter().machineReadyBitAddress] = 0;
                btn_mrba_on.Enabled = true;
                btn_mrba_off.Enabled = false;
                ConsoleShow(MacComboRouter().machineReadyBitAddress + "⇒OFF");
            }
        }

        private void btn_lrba_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().loaderReqBitAddress, "1"))
            {
                plcBitData[MacComboRouter().loaderReqBitAddress] = 1;
                btn_lrba_on.Enabled = false;
                btn_lrba_off.Enabled = true;
                ConsoleShow(MacComboRouter().loaderReqBitAddress + "⇒ON");
            }
        }

        private void btn_lrba_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().loaderReqBitAddress, "0"))
            {
                plcBitData[MacComboRouter().loaderReqBitAddress] = 0;
                btn_lrba_on.Enabled = true;
                btn_lrba_off.Enabled = false;
                ConsoleShow(MacComboRouter().loaderReqBitAddress + "⇒OFF");
            }
        }

        private void btn_urba_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().unloaderReqBitAddress, "1"))
            {
                plcBitData[MacComboRouter().unloaderReqBitAddress] = 1;
                btn_urba_on.Enabled = false;
                btn_urba_off.Enabled = true;
                ConsoleShow(MacComboRouter().unloaderReqBitAddress + "⇒ON");
            }
        }

        private void btn_urba_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().unloaderReqBitAddress, "0"))
            {
                plcBitData[MacComboRouter().unloaderReqBitAddress] = 0;
                btn_urba_on.Enabled = true;
                btn_urba_off.Enabled = false;
                ConsoleShow(MacComboRouter().unloaderReqBitAddress + "⇒OFF");
            }
        }

        private void btn_cmba_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().clearMagazinesBitAddress, "1"))
            {
                plcBitData[MacComboRouter().clearMagazinesBitAddress] = 1;
                btn_cmba_on.Enabled = false;
                btn_cmba_off.Enabled = true;
                ConsoleShow(MacComboRouter().clearMagazinesBitAddress + "⇒ON");
            }
        }

        private void btn_cmba_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().clearMagazinesBitAddress, "0"))
            {
                plcBitData[MacComboRouter().clearMagazinesBitAddress] = 0;
                btn_cmba_on.Enabled = true;
                btn_cmba_off.Enabled = false;
                ConsoleShow(MacComboRouter().clearMagazinesBitAddress + "⇒OFF");
            }
        }

        private void btn_wbas_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().waferBitAddressStart, "1"))
            {
                plcBitData[MacComboRouter().waferBitAddressStart] = 1;
                btn_wbas_on.Enabled = false;
                btn_wbas_off.Enabled = true;
                ConsoleShow(MacComboRouter().waferBitAddressStart + "⇒ON");
            }
        }

        private void btn_wbas_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().waferBitAddressStart, "0"))
            {
                plcBitData[MacComboRouter().waferBitAddressStart] = 0;
                btn_wbas_on.Enabled = true;
                btn_wbas_off.Enabled = false;
                ConsoleShow(MacComboRouter().waferBitAddressStart + "⇒OFF");
            }
        }

        private void btn_swtsa_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().startWorkTableSensorAddress, "1"))
            {
                plcBitData[MacComboRouter().startWorkTableSensorAddress] = 1;
                btn_swtsa_on.Enabled = false;
                btn_swtsa_off.Enabled = true;
                ConsoleShow(MacComboRouter().startWorkTableSensorAddress + "⇒ON");
            }
        }

        private void btn_swtsa_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().startWorkTableSensorAddress, "0"))
            {
                plcBitData[MacComboRouter().startWorkTableSensorAddress] = 0;
                btn_swtsa_on.Enabled = true;
                btn_swtsa_off.Enabled = false;
                ConsoleShow(MacComboRouter().startWorkTableSensorAddress + "⇒OFF");
            }
        }

        private void btn_wccba_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().waferChangerChangeBitAddress, "1"))
            {
                plcBitData[MacComboRouter().waferChangerChangeBitAddress] = 1;
                btn_wccba_on.Enabled = false;
                btn_wccba_off.Enabled = true;
                ConsoleShow(MacComboRouter().waferChangerChangeBitAddress + "⇒ON");
            }
        }

        private void btn_wccba_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().waferChangerChangeBitAddress, "0"))
            {
                plcBitData[MacComboRouter().waferChangerChangeBitAddress] = 0;
                btn_wccba_on.Enabled = true;
                btn_wccba_off.Enabled = false;
                ConsoleShow(MacComboRouter().waferChangerChangeBitAddress + "⇒OFF");
            }
        }

        private void btn_emurba_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().empMagUnloaderReqBitAddress, "1"))
            {
                plcBitData[MacComboRouter().empMagUnloaderReqBitAddress] = 1;
                btn_emurba_on.Enabled = false;
                btn_emurba_off.Enabled = true;
                ConsoleShow(MacComboRouter().empMagUnloaderReqBitAddress + "⇒ON");
            }
        }

        private void btn_emurba_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().empMagUnloaderReqBitAddress, "0"))
            {
                plcBitData[MacComboRouter().empMagUnloaderReqBitAddress] = 0;
                btn_emurba_on.Enabled = true;
                btn_emurba_off.Enabled = false;
                ConsoleShow(MacComboRouter().empMagUnloaderReqBitAddress + "⇒OFF");

            }
        }

        private void btn_emlrba_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().empMagLoaderReqBitAddress, "1"))
            {
                plcBitData[MacComboRouter().empMagLoaderReqBitAddress] = 1;
                btn_emlrba_on.Enabled = false;
                btn_emlrba_off.Enabled = true;
                ConsoleShow(MacComboRouter().empMagLoaderReqBitAddress + "⇒ON");
            }
        }

        private void btn_emlrba_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().empMagLoaderReqBitAddress, "0"))
            {
                plcBitData[MacComboRouter().empMagLoaderReqBitAddress] = 0;
                btn_emlrba_on.Enabled = true;
                btn_emlrba_off.Enabled = false;
                ConsoleShow(MacComboRouter().empMagLoaderReqBitAddress + "⇒OFF");
            }
        }

        private void btn_ifba_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().inputForbiddenBitAddress, "1"))
            {
                plcBitData[MacComboRouter().inputForbiddenBitAddress] = 1;
                btn_ifba_on.Enabled = false;
                btn_ifba_off.Enabled = true;
                ConsoleShow(MacComboRouter().inputForbiddenBitAddress + "⇒ON");
            }
        }

        private void btn_ifba_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().inputForbiddenBitAddress, "0"))
            {
                plcBitData[MacComboRouter().inputForbiddenBitAddress] = 0;
                btn_ifba_on.Enabled = true;
                btn_ifba_off.Enabled = false;
                ConsoleShow(MacComboRouter().inputForbiddenBitAddress + "⇒OFF");
            }
        }

        private void btn_dmba_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().dischargeModeBitAddress, "1"))
            {
                plcBitData[MacComboRouter().dischargeModeBitAddress] = 1;
                btn_dmba_on.Enabled = false;
                btn_dmba_off.Enabled = true;
                ConsoleShow(MacComboRouter().dischargeModeBitAddress + "⇒ON");
            }
        }

        private void btn_dmba_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().dischargeModeBitAddress, "0"))
            {
                plcBitData[MacComboRouter().dischargeModeBitAddress] = 0;
                btn_dmba_on.Enabled = true;
                btn_dmba_off.Enabled = false;
                ConsoleShow(MacComboRouter().dischargeModeBitAddress + "⇒OFF");
            }
        }

        private void btn_slrba_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().secondLoaderReqBitAddress, "1"))
            {
                plcBitData[MacComboRouter().secondLoaderReqBitAddress] = 1;
                btn_slrba_on.Enabled = false;
                btn_slrba_off.Enabled = true;
                ConsoleShow(MacComboRouter().secondLoaderReqBitAddress + "⇒ON");
            }
        }

        private void btn_slrba_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().secondLoaderReqBitAddress, "0"))
            {
                plcBitData[MacComboRouter().secondLoaderReqBitAddress] = 0;
                btn_slrba_on.Enabled = true;
                btn_slrba_off.Enabled = false;
                ConsoleShow(MacComboRouter().secondLoaderReqBitAddress + "⇒OFF");
            }
        }

        private void btn_semlrba_on_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().secondEmpMagLoaderReqBitAddress, "1"))
            {
                plcBitData[MacComboRouter().secondEmpMagLoaderReqBitAddress] = 1;
                btn_semlrba_on.Enabled = false;
                btn_semlrba_off.Enabled = true;
                ConsoleShow(MacComboRouter().secondEmpMagLoaderReqBitAddress + "⇒ON");
            }
        }

        private void btn_semlrba_off_Click(object sender, EventArgs e)
        {
            if (SetBit(MacComboRouter().secondEmpMagLoaderReqBitAddress, "0"))
            {
                plcBitData[MacComboRouter().secondEmpMagLoaderReqBitAddress] = 0;
                btn_semlrba_on.Enabled = true;
                btn_semlrba_off.Enabled = false;
                ConsoleShow(MacComboRouter().secondEmpMagLoaderReqBitAddress + "⇒OFF");
            }
        }

        private void lb_mrba_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().machineReadyBitAddress);
        }

        private void lb_lrba_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().loaderReqBitAddress);
        }

        private void lb_ulrba_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().unloaderReqBitAddress);
        }

        private void lb_cmba_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().clearMagazinesBitAddress);
        }

        private void lb_wbas_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().waferBitAddressStart);
        }

        private void lb_swtsa_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().startWorkTableSensorAddress);
        }

        private void lb_wccba_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().waferChangerChangeBitAddress);
        }

        private void lb_emurba_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().empMagUnloaderReqBitAddress);
        }

        private void lb_emlrba_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().empMagLoaderReqBitAddress);
        }

        private void lb_ifba_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().inputForbiddenBitAddress);
        }

        private void lb_dmba_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().dischargeModeBitAddress);
        }

        private void lb_slrba_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().secondLoaderReqBitAddress);
        }

        private void lb_semlrba_Click(object sender, EventArgs e)
        {
            ConsoleShow(MacComboRouter().secondEmpMagLoaderReqBitAddress);
        }

        //
        // 個別設備デバイス処理
        //

        // Ze Die Bonder

        // DBオーブン
        private void btn_lrba_s1_on_Click(object sender, EventArgs e)
        {
            SetBit(DBOvnComboRouter(cb_dbovnno.Text).loaderReqBitAddressList_addKey1, "1");
            btn_lrba_s1_on.Enabled = false;
            btn_lrba_s1_off.Enabled = true;
            ConsoleShow("スロット1供給要求ON");
        }

        private void btn_lrba_s1_off_Click(object sender, EventArgs e)
        {
            SetBit(DBOvnComboRouter(cb_dbovnno.Text).loaderReqBitAddressList_addKey1, "0");
            btn_lrba_s1_on.Enabled = true;
            btn_lrba_s1_off.Enabled = false;
            ConsoleShow("スロット1供給要求OFF");
        }

        private void btn_lrba_s2_on_Click(object sender, EventArgs e)
        {
            SetBit(DBOvnComboRouter(cb_dbovnno.Text).loaderReqBitAddressList_addKey2, "1");
            btn_lrba_s2_on.Enabled = false;
            btn_lrba_s2_off.Enabled = true;
            ConsoleShow("スロット2供給要求ON");
        }

        private void btn_lrba_s2_off_Click(object sender, EventArgs e)
        {
            SetBit(DBOvnComboRouter(cb_dbovnno.Text).loaderReqBitAddressList_addKey2, "0");
            btn_lrba_s2_on.Enabled = true;
            btn_lrba_s2_off.Enabled = false;
            ConsoleShow("スロット2供給要求OFF");
        }

        private void btn_ulrba_s1_on_Click(object sender, EventArgs e)
        {
            SetDataTimeToOvenPlc(DBOvnComboRouter(cb_dbovnno.Text));
            ConsoleShow("開始完了時刻を書き込んでいます:Wait1秒");
            Thread.Sleep(1000);
            SetBit(DBOvnComboRouter(cb_dbovnno.Text).unloaderReqBitAddressList_addKey1, "1");
            btn_ulrba_s1_on.Enabled = false;
            btn_ulrba_s1_off.Enabled = true;
            ConsoleShow("スロット1排出要求ON");
        }

        private void btn_ulrba_s1_off_Click(object sender, EventArgs e)
        {
            SetBit(DBOvnComboRouter(cb_dbovnno.Text).unloaderReqBitAddressList_addKey1, "0");
            btn_ulrba_s1_on.Enabled = true;
            btn_ulrba_s1_off.Enabled = false;
            ConsoleShow("スロット1排出要求OFF");
            SetDataTimeToOvenPlc(DBOVEN_01);
        }

        private void btn_ulrba_s2_on_Click(object sender, EventArgs e)
        {
            SetDataTimeToOvenPlc(DBOvnComboRouter(cb_dbovnno.Text));
            ConsoleShow("開始完了時刻を書き込んでいます:Wait1秒");
            Thread.Sleep(1000);
            SetBit(DBOvnComboRouter(cb_dbovnno.Text).loaderReqBitAddressList_addKey2, "1");
            btn_ulrba_s2_on.Enabled = false;
            btn_ulrba_s2_off.Enabled = true;
            ConsoleShow("スロット2排出要求ON");
        }

        private void btn_ulrba_s2_off_Click(object sender, EventArgs e)
        {
            SetBit(DBOvnComboRouter(cb_dbovnno.Text).loaderReqBitAddressList_addKey2, "0");
            btn_ulrba_s2_on.Enabled = true;
            btn_ulrba_s2_off.Enabled = false;
            ConsoleShow("スロット2排出要求OFF");
        }

        private void cb_dbovnProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetWordAsDecimalData(DBOvnComboRouter(cb_dbovnno.Text).currentProfileWordAddress, int.Parse(cb_dbovnProfile.Text));
        }

        private void lb_dbov_lrba_s1_Click(object sender, EventArgs e)
        {
            ConsoleShow(DBOvnComboRouter(cb_dbovnno.Text).loaderReqBitAddressList_addKey1);
        }

        private void lb_dbov_lrba_s2_Click(object sender, EventArgs e)
        {
            ConsoleShow(DBOvnComboRouter(cb_dbovnno.Text).unloaderReqBitAddressList_addKey1);
        }

        private void lb_dbov_ulrba_s1_Click(object sender, EventArgs e)
        {
            ConsoleShow(DBOvnComboRouter(cb_dbovnno.Text).loaderReqBitAddressList_addKey2);
        }

        private void lb_dbov_ulrba_s2_Click(object sender, EventArgs e)
        {
            ConsoleShow(DBOvnComboRouter(cb_dbovnno.Text).unloaderReqBitAddressList_addKey2);
        }

        public void SetDataTimeToOvenPlc(Oven ovn)
        {
            try
            {
                DateTime dt = DateTime.Now;
                TimeSpan ts = new TimeSpan(0, 0, 0, 10);
                var starttime = dt - ts;
                var endtime = dt;
                var plcaddress = Convert.ToInt32(ovn.workStartTimeAddress.Replace("W", ""), 16);
                //ConsoleShow(plcaddress.ToString("x4"));
                //Plc.SetWordAsDecimalData("W00" + (plcaddress).ToString("x4"), 2021);
                //START TIME
                //year
                SetWordAsDecimalData("W00" + (plcaddress).ToString("x4"), starttime.Year);
                ////month
                SetWordAsDecimalData("W00" + (plcaddress + 1).ToString("x4"), starttime.Month);
                ////day
                SetWordAsDecimalData("W00" + (plcaddress + 2).ToString("x4"), starttime.Day);
                ////hour
                SetWordAsDecimalData("W00" + (plcaddress + 3).ToString("x4"), starttime.Hour);
                ////minite
                SetWordAsDecimalData("W00" + (plcaddress + 4).ToString("x4"), starttime.Minute);
                ////second
                SetWordAsDecimalData("W00" + (plcaddress + 5).ToString("x4"), starttime.Second);
                //END TIME
                //year
                SetWordAsDecimalData("W00" + (plcaddress + 6).ToString("x4"), endtime.Year);
                ////month
                SetWordAsDecimalData("W00" + (plcaddress + 7).ToString("x4"), endtime.Month);
                ////day
                SetWordAsDecimalData("W00" + (plcaddress + 8).ToString("x4"), endtime.Day);
                ////hour
                SetWordAsDecimalData("W00" + (plcaddress + 9).ToString("x4"), endtime.Hour);
                ////minite
                SetWordAsDecimalData("W00" + (plcaddress + 10).ToString("x4"), endtime.Minute);
                ////second
                SetWordAsDecimalData("W00" + (plcaddress + 11).ToString("x4"), endtime.Second);

                ConsoleShow(plcaddress.ToString("START/END時間の設定をしました。"));
            }
            catch (Exception ex)
            {
                return;
            }

        }

        // LED Die Bonder

        // DB PL

        private void btn_dbpl_appendTntran_Click(object sender, EventArgs e)
        {
            var lotno = tx_dbpl_lotno.Text;
            var magno = tx_dbpl_m_auto.Text.Replace("C30 ", "");
            appendDBPLTran(lotno, magno);
        }

        private void btn_dbpl_updateTnMag_Click(object sender, EventArgs e)
        {
            var lotno = tx_dbpl_lotno.Text;
            var magno = tx_dbpl_m_auto.Text.Replace("C30 ", "");
            updateTnMag(5, lotno, magno);
        }

        private void btn_get_lastLotno_Click(object sender, EventArgs e)
        {

            tx_dbpl_lotno.Text = getLastLotNo();

        }

        public bool appendDBPLTran(string lotno, string magno)
        {
            try
            {
                DateTime dt = DateTime.Now;

                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = $"INSERT INTO[dbo].[TnTran] ";
                    cmd.CommandText += "([lotno]";
                    cmd.CommandText += ",[procno]";
                    cmd.CommandText += ",[macno]";
                    cmd.CommandText += ",[inmag]";
                    cmd.CommandText += ",[outmag]";
                    cmd.CommandText += ",[startdt]";
                    cmd.CommandText += ",[enddt]";
                    cmd.CommandText += ",[iscomplt]";
                    cmd.CommandText += ",[stocker1]";
                    cmd.CommandText += ",[stocker2]";
                    cmd.CommandText += ",[comment]";
                    cmd.CommandText += ",[transtartempcd]";
                    cmd.CommandText += ",[trancompempcd]";
                    cmd.CommandText += ",[inspectempcd]";
                    cmd.CommandText += ",[inspectct]";
                    cmd.CommandText += ",[isnascastart]";
                    cmd.CommandText += ",[isnascaend]";
                    cmd.CommandText += ",[isnascadefectend]";
                    cmd.CommandText += ",[isnascacommentend]";
                    cmd.CommandText += ",[delfg]";
                    cmd.CommandText += ",[isdefectend]";
                    cmd.CommandText += ",[isdefectautoimportend]";
                    cmd.CommandText += ",[isnascarunning]";
                    cmd.CommandText += ",[isautoimport]";
                    cmd.CommandText += ",[isresinmixordered])";
                    cmd.CommandText += " VALUES";
                    cmd.CommandText += $"(('{lotno} ')";
                    cmd.CommandText += ",(5)";
                    cmd.CommandText += ",(105301)";
                    cmd.CommandText += $",('{magno}')";
                    cmd.CommandText += $",('{magno}')";
                    cmd.CommandText += $",('{dt.ToString()} ')";
                    cmd.CommandText += $",('{dt.ToString()}')";
                    cmd.CommandText += ",(1)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(660)";
                    cmd.CommandText += ",(660)";
                    cmd.CommandText += ",(660)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(1)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += " ,(0)";
                    cmd.CommandText += " ,(0))";

                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                    con.Close();

                    return true;

                }
            }
            catch (Exception ex)
            {
                ConsoleShow(ex.ToString());
                return false;
            }
        }

        // WBプラズマ
        private void btn_llkey1_on_Click(object sender, EventArgs e)
        {
            SetBit(WBPL_01.loader_lane_key1, "1");
            btn_wbpl_llkey1_on.Enabled = false;
            btn_wbpl_llkey1_off.Enabled = true;
            ConsoleShow("ローダー1実マガジン供給要求ON");
        }

        private void btn_llkey1_off_Click(object sender, EventArgs e)
        {
            SetBit(WBPL_01.loader_lane_key1, "0");
            btn_wbpl_llkey1_on.Enabled = true;
            btn_wbpl_llkey1_off.Enabled = false;
            ConsoleShow("ローダー1実マガジン供給要求OFF");
        }

        private void btn_emlkey1_on_Click(object sender, EventArgs e)
        {
            SetBit(WBPL_01.empMagLoader_lane_key1, "1");
            btn_wbpl_emlkey1_on.Enabled = false;
            btn_wbpl_emlkey1_off.Enabled = true;
            ConsoleShow("空マガジン供給-1要求ON");
        }

        private void btn_emlkey1_off_Click(object sender, EventArgs e)
        {
            SetBit(WBPL_01.empMagLoader_lane_key1, "0");
            btn_wbpl_emlkey1_on.Enabled = true;
            btn_wbpl_emlkey1_off.Enabled = false;
            ConsoleShow("空マガジン供給-1要求OFF");
        }

        private void btn_wbpl__ulkey1_on_Click(object sender, EventArgs e)
        {
            SetBit(WBPL_01.unloader_lane_key1, "1");
            btn_wbpl_ulkey1_on.Enabled = false;
            btn_wbpl_ulkey1_off.Enabled = true;
            ConsoleShow("実マガジン排出-1要求ON");
        }

        private void btn_wbpl__ulkey1_off_Click(object sender, EventArgs e)
        {
            SetBit(WBPL_01.unloader_lane_key1, "0");
            btn_wbpl_ulkey1_on.Enabled = true;
            btn_wbpl_ulkey1_off.Enabled = false;
            ConsoleShow("実マガジン排出-1要求OFF");
        }

        private void lb_wbpl_llkey1_Click(object sender, EventArgs e)
        {
            ConsoleShow(WBPL_01.loader_lane_key1);
        }

        private void lb_wbpl_emllkey1_Click(object sender, EventArgs e)
        {
            ConsoleShow(WBPL_01.empMagLoader_lane_key1);
        }

        private void lb_wbpl_ullkey1_Click(object sender, EventArgs e)
        {
            ConsoleShow(WBPL_01.unloader_lane_key1);
        }

        // WB


        // AVI
        private void btn_avi_readQr_Click(object sender, EventArgs e)
        {
            ReadAviQrCode(txt_avi_qrcode.Text);
        }

        public bool ReadAviQrCode(string qrword)
        {
            if (SetString(AVI_01.ulMagazineAddress, qrword))
            {
                ConsoleShow("排出Magno(AVI)に[" + qrword + "]を書き込みました");
                return true;
            }
            else
            {
                ConsoleShow("QRのデータ書き込みが失敗しました");
            }

            return false;
        }

        //WBOVN
        private void btn_wbovn_get_lastLotno_Click(object sender, EventArgs e)
        {
            txt_wbovn_lotno.Text = getLastLotNo();
        }

        private void btn_wbovn_appendTntran_Click(object sender, EventArgs e)
        {
            var lotno = txt_wbovn_lotno.Text;
            var magno = txt_wbovn_m_auto.Text.Replace("C30 ", "");
            appendWBOVNTran(lotno, magno);
        }

        private void btn_wbovn_updateTnMag_Click(object sender, EventArgs e)
        {
            var lotno = txt_wbovn_lotno.Text;
            var magno = txt_wbovn_m_auto.Text.Replace("C30 ", "");
            updateTnMag(22, lotno, magno);
        }

        public bool appendWBOVNTran(string lotno, string magno)
        {
            try
            {
                DateTime dt = DateTime.Now;

                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = $"INSERT INTO[dbo].[TnTran] ";
                    cmd.CommandText += "([lotno]";
                    cmd.CommandText += ",[procno]";
                    cmd.CommandText += ",[macno]";
                    cmd.CommandText += ",[inmag]";
                    cmd.CommandText += ",[outmag]";
                    cmd.CommandText += ",[startdt]";
                    cmd.CommandText += ",[enddt]";
                    cmd.CommandText += ",[iscomplt]";
                    cmd.CommandText += ",[stocker1]";
                    cmd.CommandText += ",[stocker2]";
                    cmd.CommandText += ",[comment]";
                    cmd.CommandText += ",[transtartempcd]";
                    cmd.CommandText += ",[trancompempcd]";
                    cmd.CommandText += ",[inspectempcd]";
                    cmd.CommandText += ",[inspectct]";
                    cmd.CommandText += ",[isnascastart]";
                    cmd.CommandText += ",[isnascaend]";
                    cmd.CommandText += ",[isnascadefectend]";
                    cmd.CommandText += ",[isnascacommentend]";
                    cmd.CommandText += ",[delfg]";
                    cmd.CommandText += ",[isdefectend]";
                    cmd.CommandText += ",[isdefectautoimportend]";
                    cmd.CommandText += ",[isnascarunning]";
                    cmd.CommandText += ",[isautoimport]";
                    cmd.CommandText += ",[isresinmixordered])";
                    cmd.CommandText += " VALUES";
                    cmd.CommandText += $"(('{lotno} ')";
                    cmd.CommandText += ",(22)";
                    cmd.CommandText += ",(122301)";
                    cmd.CommandText += $",('{magno}')";
                    cmd.CommandText += $",('{magno}')";
                    cmd.CommandText += $",('{dt.ToString()} ')";
                    cmd.CommandText += $",('{dt.ToString()}')";
                    cmd.CommandText += ",(1)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",''";
                    cmd.CommandText += ",(211105)";
                    cmd.CommandText += ",(211105)";
                    cmd.CommandText += ",''";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(1)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += " ,(0)";
                    cmd.CommandText += " ,(0))";

                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                    con.Close();

                    return true;

                }
            }
            catch (Exception ex)
            {
                ConsoleShow(ex.ToString());
                return false;
            }
        }

        // Suputter
        private void btn_sup_Ldcmpl_off_Click(object sender, EventArgs e)
        {
            bsup_Ldcmpl_offk();
        }
        public void bsup_Ldcmpl_offk()
        {
            if (!SetBit(SUP_01.loaderMachineSelectCompleteBitAddress, "0"))
            {
                ConsoleShow("スパッタ(loaderMachineSelectCompleteBit)が書き込めませんでした");
            }
        }

        private void btn_sup_Ulcmpl_on_Click(object sender, EventArgs e)
        {
            var macno = txt_sup_ulno.Text;
            sup_Ulcmpl_on(macno);
        }
        public void sup_Ulcmpl_on(string macno)
        {
            if (!SetString(SUP_01.unloaderMachineNoAddress, macno))
            {
                ConsoleShow("スパッタ(UL)設備Noが書き込めませんでした");
            }
        }

        private void btn_sup_Ldcmpl_on_Click(object sender, EventArgs e)
        {
            var macno = txt_sup_ldno.Text;
            sup_Ldcmpl_on(macno);
        }
        public void sup_Ldcmpl_on(string macno)
        {
            if (!SetString(SUP_01.loaderMachineNoAddress, macno))
            {
                ConsoleShow("スパッタ(LD)設備Noが書き込めませんでした");
            }
        }

        private void btn_sup_Inmag_on_Click(object sender, EventArgs e)
        {
            var magno = txt_sup_inmagno.Text;
            sup_Inmag_on(magno);
        }
        public void sup_Inmag_on(string magno)
        {
            if (!SetString(SUP_01.loaderMagazineAddress, magno))
            {
                ConsoleShow("スパッタのINMAG番号が書き込めませんでした");
            }
            if (!SetBit(SUP_01.loaderQRReadCompleteBitAddress, "1"))
            {
                ConsoleShow("スパッタ(loaderQRReadCompleteBitAddress)が書き込めませんでした");
            }
        }

        private void btn_sup_Outmag_on_Click(object sender, EventArgs e)
        {
            var magno = txt_sup_outmagno.Text;
            sup_Outmag_on(magno);
        }
        public void sup_Outmag_on(string magno)
        {
            if (!SetString(SUP_01.unloaderMagazineAddress, magno))
            {
                ConsoleShow("スパッタのOUTMAG番号が書き込めませんでした");
            }
            if (!SetBit(SUP_01.unloaderQRReadCompleteBitAddress, "1"))
            {
                ConsoleShow("スパッタ(unloaderQRReadCompleteBitAddress)が書き込めませんでした");
            }
        }

        private void btn_sup_Inmag_off_Click(object sender, EventArgs e)
        {
            sup_Inmag_off();
        }
        public void sup_Inmag_off()
        {
            if (!SetBit(SUP_01.loaderQRReadCompleteBitAddress, "0"))
            {
                ConsoleShow("スパッタ(loaderQRReadCompleteBitAddress)が書き込めませんでした");
            }
        }

        private void btn_sup_Outmag_off_Click(object sender, EventArgs e)
        {
            sup_Outmag_off();
        }
        public void sup_Outmag_off()
        {
            if (!SetBit(SUP_01.unloaderQRReadCompleteBitAddress, "0"))
            {
                ConsoleShow("スパッタ(unloaderQRReadCompleteBitAddress)が書き込めませんでした");
            }
        }

        private void btn_sup_lmscba_on_Click(object sender, EventArgs e)
        {
            sup_lmscba_on();
        }
        public void sup_lmscba_on()
        {
            if (!SetBit(SUP_01.loaderMachineSelectCompleteBitAddress, "1"))
            {
                ConsoleShow("スパッタ(loaderMachineSelectCompleteBit)が書き込めませんでした");
            }
        }

        private void btn_sup_lmscba_off_Click(object sender, EventArgs e)
        {
            sup_lmscba_off();
        }
        public void sup_lmscba_off()
        {
            if (!SetBit(SUP_01.loaderMachineSelectCompleteBitAddress, "0"))
            {
                ConsoleShow("スパッタ(loaderMachineSelectCompleteBit)が書き込めませんでした");
            }
        }

        private void lb_sup_ldno_Click(object sender, EventArgs e)
        {
            ConsoleShow(SUP_01.loaderMachineNoAddress);
        }

        private void lb_sup_ulno_Click(object sender, EventArgs e)
        {
            ConsoleShow(SUP_01.unloaderMachineNoAddress);
        }

        private void lb_sup_inmag_Click(object sender, EventArgs e)
        {
            ConsoleShow(SUP_01.loaderMagazineAddress + " " + SUP_01.loaderQRReadCompleteBitAddress);
        }

        private void lb_sup_outmag_Click(object sender, EventArgs e)
        {
            ConsoleShow(SUP_01.unloaderMagazineAddress + " " + SUP_01.unloaderQRReadCompleteBitAddress);
        }

        private void ib_sup_lmscba_Click(object sender, EventArgs e)
        {
            ConsoleShow(SUP_01.loaderMachineSelectCompleteBitAddress);
        }

        private void btn_sup_starttime_Click(object sender, EventArgs e)
        {
            var dt = DateTime.Now;
            SetDateTime2Word(SUP_01.workStartTimeAddress, dt);
        }

        private void btn_sup_comptime_Click(object sender, EventArgs e)
        {
            var dt = DateTime.Now;
            SetDateTime2Word(SUP_01.workCompleteTimeAddress, dt);
        }

        private void lb_sup_stacomptime_Click(object sender, EventArgs e)
        {
            ConsoleShow(SUP_01.workStartTimeAddress + " " + SUP_01.workCompleteTimeAddress);
        }

        private void btn_sup_logout_Click(object sender, EventArgs e)
        {
            var macInstance = SUP_01;
            //
            // log file words
            //
            var MLfolderpath = "";
            if (!macInstance.getMacFolderPath("in", ref MLfolderpath))
            {
                ConsoleShow(MLfolderpath);
                return;
            }
            var logfiledict = new Dictionary<string, string>
            {
                { "@magno", txt_sup_m_auto.Text },
                { "@kishumei", $"{EicsParam_TypeCode}" }
            };
            var msg = "";
            var dt = DateTime.Now;
            var MLSriname = dt.ToString("ddHHmmss");
            var Targetfld = MLfolderpath + $"03_auto\\{MLSriname}";

            if (!Directory.Exists(Targetfld))
            {
                Directory.CreateDirectory(Targetfld);
            }

            if (!CreateMLog(macInstance.macLogOriginFolder + "_03_auto_\\_03_auto_A.CSV", Targetfld + "\\_03_auto_" + MLSriname + "A.CSV", logfiledict, ref msg))
            {
                ConsoleShow(msg);
                return;
            }
            if (!CreateMLog(macInstance.macLogOriginFolder + "_03_auto_\\_03_auto_B.CSV", Targetfld + "\\_03_auto_" + MLSriname + "B.CSV", logfiledict, ref msg))
            {
                ConsoleShow(msg);
                return;
            }
            if (!CreateMLog(macInstance.macLogOriginFolder + "_03_auto_\\_03_auto_C.CSV", Targetfld + "\\_03_auto_" + MLSriname + "C.CSV", logfiledict, ref msg))
            {
                ConsoleShow(msg);
                return;
            }

            // +++ Wait +++
            Thread.Sleep(2000);

            ConsoleShow("■■■マガジン別データ・自動出力データを出力開始■■■");

        }

        // MD

        private void btn_md_readBuff_Click(object sender, EventArgs e)
        {
            var ldvol = int.Parse(txt_md_ldvol.Text);
            var eldvol = int.Parse(txt_md_empldvol.Text);
            var uldvol = int.Parse(txt_md_uldvol.Text);
            var euldvol = int.Parse(txt_md_empuldvol.Text);
            md_readBuff(ldvol, eldvol, uldvol, euldvol);
            ConsoleShow("バッファ数登録をしました");
        }
        public void md_readBuff(int ldvol, int empldvol, int uldvol, int empuldvol)
        {
            SetWordAsDecimalData(MD_01.lMagazineCountAddress, ldvol);
            SetWordAsDecimalData(MD_01.elMagazineCountAddress, empldvol);
            SetWordAsDecimalData(MD_01.ulMagazineCountAddress, uldvol);
            SetWordAsDecimalData(MD_01.eulMagazineCountAddress, empuldvol);
        }

        private void btn_md_magno_Click(object sender, EventArgs e)
        {
            var inmagno = txt_md_inmagno.Text;
            md_inmagno(inmagno);
            ConsoleShow("INマガジンNoを登録しました");
            //開始完了時間メモリ(PLC)に書き込み
            var dt = DateTime.Now;
            SetDateTime2Word(MD_01.workStartTimeAddress, dt);
            ConsoleShow("PLCに開始時間を登録しました");
        }

        private void btn_md_outmagno1_Click(object sender, EventArgs e)
        {
            var outmagno_1 = txt_md_outmagno1.Text;
            md_outmagno(outmagno_1);
            ConsoleShow("OUTマガジンNoを登録しました");
            //開始完了時間メモリ(PLC)に書き込み
            var dt = DateTime.Now;
            SetDateTime2Word(MD_01.workCompleteTimeAddress, dt);
            ConsoleShow("PLCに完了時間を登録しました");
        }

        private void btn_md_outmagno2_Click(object sender, EventArgs e)
        {
            var outmagno_2 = txt_md_outmagno2.Text;
            md_outmagno(outmagno_2);
            ConsoleShow("OUTマガジンNoを登録しました");
            //開始完了時間メモリ(PLC)に書き込み
            var dt = DateTime.Now;
            SetDateTime2Word(MD_01.workCompleteTimeAddress, dt);
            ConsoleShow("PLCに完了時間を登録しました");
        }

        public void md_inmagno(string inmagno)
        {
            SetString(MD_01.lMagazineAddress, inmagno);
        }

        public void md_outmagno(string outmagno)
        {
            SetString(MD_01.ulMagazineAddress, outmagno);
        }

        //ECK
        private void btn_eck_lrba_on_Click(object sender, EventArgs e)
        {
            var slotno = int.Parse(cb_eck_lrba.Text);
            SetEckLrba(slotno, "1");
            ConsoleShow($"供給({slotno})要求をONしました");
        }

        private void btn_eck_lrba_off_Click(object sender, EventArgs e)
        {
            var slotno = int.Parse(cb_eck_lrba.Text);
            SetEckLrba(slotno, "0");
            ConsoleShow($"供給({slotno})要求をOFFしました");
        }

        private void btn_eck_ulrba_on_Click(object sender, EventArgs e)
        {
            var slotno = int.Parse(cb_eck_lrba.Text);
            SetEckUlrba(slotno, "1");
            ConsoleShow($"排出({slotno})要求要求をONしました");
            var outmagno = txt_eck_outmagno.Text;
            SetString(ECK_01.ulMagazineAddress, outmagno);
            ConsoleShow("OUTマガジンNoを登録しました");
            //開始完了時間メモリ(PLC)に書き込み
            var dt = DateTime.Now;
            SetDateTime2Word(ECK_01.workStartTimeAddress, dt);
            SetDateTime2Word(ECK_01.workCompleteTimeAddress, dt);
            ConsoleShow("PLCに完了時間を登録しました");
        }

        private void btn_eck_ulrba_off_Click(object sender, EventArgs e)
        {
            var slotno = int.Parse(cb_eck_lrba.Text);
            SetEckUlrba(slotno, "0");
            ConsoleShow($"排出({slotno})要求要求をOFFしました");
        }

        public void SetEckLrba(int slotno, string onoff)
        {
            switch (slotno)
            {
                case 1:
                    SetBit(ECK_01.loaderReqBitAddress1, onoff);
                    break;
                case 2:
                    SetBit(ECK_01.loaderReqBitAddress2, onoff);
                    break;
                case 3:
                    SetBit(ECK_01.loaderReqBitAddress3, onoff);
                    break;
                case 4:
                    SetBit(ECK_01.loaderReqBitAddress4, onoff);
                    break;
                case 5:
                    SetBit(ECK_01.loaderReqBitAddress5, onoff);
                    break;
                case 6:
                    SetBit(ECK_01.loaderReqBitAddress6, onoff);
                    break;
            }
        }
        public void SetEckUlrba(int slotno, string onoff)
        {
            switch (slotno)
            {
                case 1:
                    SetBit(ECK_01.unloaderReqBitAddress1, onoff);
                    break;
                case 2:
                    SetBit(ECK_01.unloaderReqBitAddress2, onoff);
                    break;
                case 3:
                    SetBit(ECK_01.unloaderReqBitAddress3, onoff);
                    break;
                case 4:
                    SetBit(ECK_01.unloaderReqBitAddress4, onoff);
                    break;
                case 5:
                    SetBit(ECK_01.unloaderReqBitAddress5, onoff);
                    break;
                case 6:
                    SetBit(ECK_01.unloaderReqBitAddress6, onoff);
                    break;
            }
        }

        //GB/MEX
        private void btn_gb_lrba_on_Click(object sender, EventArgs e)
        {
            var slotno = int.Parse(cb_gb_lrba_bfno.Text);
            SetGBLrba(slotno, "1");
            ConsoleShow($"供給({slotno})要求をONしました");
        }

        private void btn_gb_lrba_off_Click(object sender, EventArgs e)
        {
            var slotno = int.Parse(cb_gb_lrba_bfno.Text);
            SetGBLrba(slotno, "0");
            ConsoleShow($"供給({slotno})要求をOFFしました");
        }

        private void btn_gb_ulrba_on_Click(object sender, EventArgs e)
        {
            var slotno = int.Parse(cb_gb_ulrba_bfno.Text);
            SetGBUlrba(slotno, "1");
            ConsoleShow($"排出({slotno})要求をONしました");
        }

        private void btn_gb_ulrba_off_Click(object sender, EventArgs e)
        {
            var slotno = int.Parse(cb_gb_ulrba_bfno.Text);
            SetGBUlrba(slotno, "0");
            ConsoleShow($"排出({slotno})要求をOFFしました");
        }

        public void SetGBLrba(int slotno, string onoff)
        {
            switch (slotno)
            {
                case 1:
                    SetBit(GB_01.loaderReqBitAddress, onoff);
                    break;
                case 2:
                    SetBit(GB_02.loaderReqBitAddress, onoff);
                    break;
                case 3:
                    SetBit(GB_03.loaderReqBitAddress, onoff);
                    break;
            }
        }
        public void SetGBUlrba(int slotno, string onoff)
        {
            switch (slotno)
            {
                case 1:
                    SetBit(GB_01.unloaderReqBitAddress, onoff);
                    break;
                case 2:
                    SetBit(GB_02.unloaderReqBitAddress, onoff);
                    break;
                case 3:
                    SetBit(GB_03.unloaderReqBitAddress, onoff);
                    break;
            }
        }

        private void btn_gb_getmag_Click(object sender, EventArgs e)
        {
            Rb_GetMag();
            SetBit(NicConv_01.magazineArriveBitAddress, "0");
            bt_conv_nichiaMag_on.Enabled = true;

            Thread.Sleep(2000);

            var slotno = int.Parse(cb_gb_lrba_bfno.Text);
            SetGBLrba(slotno, "0");
            ConsoleShow($"供給({slotno})要求をOFFしました");
            SetGBUlrba(slotno, "1");
            ConsoleShow($"排出({slotno})要求をONしました");
        }

        private void btn_gb_getmag2_Click(object sender, EventArgs e)
        {
            Rb_GetMag();

            Thread.Sleep(2000);

            var slotno = int.Parse(cb_gb_lrba_bfno.Text);
            SetGBUlrba(slotno, "0");
            ConsoleShow($"排出({slotno})要求をOFFしました");
        }

        // MD-OVN
        private void btn_mdovn_lrba_s1_on_Click(object sender, EventArgs e)
        {
            SetBit(MDOVNComboRouter(cb_mdovn_macno.Text).loaderReqBitAddressList_addKey1, "1");
            btn_mdovn_lrba_s1_on.Enabled = false;
            btn_mdovn_lrba_s1_off.Enabled = true;
            ConsoleShow("スロット1供給要求ON");
        }

        private void btn_mdovn_lrba_s1_off_Click(object sender, EventArgs e)
        {
            SetBit(MDOVNComboRouter(cb_mdovn_macno.Text).loaderReqBitAddressList_addKey1, "0");
            btn_mdovn_lrba_s1_on.Enabled = true;
            btn_mdovn_lrba_s1_off.Enabled = false;
            ConsoleShow("スロット1供給要求OFF");
        }

        private void btn_mdovn_lrba_s2_on_Click(object sender, EventArgs e)
        {
            SetBit(MDOVNComboRouter(cb_mdovn_macno.Text).loaderReqBitAddressList_addKey2, "1");
            btn_mdovn_lrba_s2_on.Enabled = false;
            btn_mdovn_lrba_s2_off.Enabled = true;
            ConsoleShow("スロット2供給要求ON");
        }

        private void btn_mdovn_lrba_s2_off_Click(object sender, EventArgs e)
        {
            SetBit(MDOVNComboRouter(cb_mdovn_macno.Text).loaderReqBitAddressList_addKey2, "0");
            btn_mdovn_lrba_s2_on.Enabled = true;
            btn_mdovn_lrba_s2_off.Enabled = false;
            ConsoleShow("スロット2供給要求OFF");
        }

        private void btn_mdovn_ulrba_s1_on_Click(object sender, EventArgs e)
        {
            SetDataTimeToOvenPlc(MDOVNComboRouter(cb_mdovn_macno.Text));
            ConsoleShow("開始完了時刻を書き込んでいます:Wait1秒");
            Thread.Sleep(1000);
            SetBit(MDOVNComboRouter(cb_mdovn_macno.Text).unloaderReqBitAddressList_addKey1, "1");
            btn_ulrba_s1_on.Enabled = false;
            btn_ulrba_s1_off.Enabled = true;
            ConsoleShow("スロット1排出要求ON");
        }

        private void btn_mdovn_ulrba_s1_off_Click(object sender, EventArgs e)
        {
            SetBit(MDOVNComboRouter(cb_mdovn_macno.Text).unloaderReqBitAddressList_addKey1, "0");
            btn_ulrba_s1_on.Enabled = true;
            btn_ulrba_s1_off.Enabled = false;
            ConsoleShow("スロット1排出要求OFF");
        }

        private void btn_mdovn_ulrba_s2_on_Click(object sender, EventArgs e)
        {
            SetDataTimeToOvenPlc(MDOVNComboRouter(cb_mdovn_macno.Text));
            ConsoleShow("開始完了時刻を書き込んでいます:Wait1秒");
            Thread.Sleep(1000);
            SetBit(MDOVNComboRouter(cb_mdovn_macno.Text).unloaderReqBitAddressList_addKey2, "1");
            btn_ulrba_s1_on.Enabled = false;
            btn_ulrba_s1_off.Enabled = true;
            ConsoleShow("スロット2排出要求ON");
        }

        private void btn_mdovn_ulrba_s2_off_Click(object sender, EventArgs e)
        {
            SetBit(MDOVNComboRouter(cb_mdovn_macno.Text).unloaderReqBitAddressList_addKey2, "0");
            btn_ulrba_s1_on.Enabled = true;
            btn_ulrba_s1_off.Enabled = false;
            ConsoleShow("スロット2排出要求OFF");
        }

        //DAM

        private void bt_dam_get_lastLotno_Click(object sender, EventArgs e)
        {
            txt_dam_lotno.Text = getLastLotNo();
        }

        private void btn_dam_appendTntran_Click(object sender, EventArgs e)
        {
            var lotno = txt_dam_lotno.Text;
            var magno = txt_dam_m_auto.Text.Replace("C30 ", "");
            var procno = int.Parse(cb_dam_procno.Text);
            var macno = 0;
            if (procno == 24)
            {
                macno = 424001;
            }
            else if (procno == 25)
            {
                macno = 425001;
            }
            else
            {
                macno = 426001;
            }
            appendDAMTran(lotno, magno, procno, macno);
        }

        private void btn_dam_updateTnMag_Click(object sender, EventArgs e)
        {
            var lotno = txt_dam_lotno.Text;
            var magno = txt_dam_m_auto.Text.Replace("C30 ", "");
            var procno = int.Parse(cb_dam_procno.Text);
            updateTnMag(procno, lotno, magno);
        }

        public bool appendDAMTran(string lotno, string magno, int procno, int macno)
        {
            try
            {
                DateTime dt = DateTime.Now;
                //var procno = int.Parse(cb_dam_procno.Text);

                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = $"INSERT INTO[dbo].[TnTran] ";
                    cmd.CommandText += "([lotno]";
                    cmd.CommandText += ",[procno]";
                    cmd.CommandText += ",[macno]";
                    cmd.CommandText += ",[inmag]";
                    cmd.CommandText += ",[outmag]";
                    cmd.CommandText += ",[startdt]";
                    cmd.CommandText += ",[enddt]";
                    cmd.CommandText += ",[iscomplt]";
                    cmd.CommandText += ",[stocker1]";
                    cmd.CommandText += ",[stocker2]";
                    cmd.CommandText += ",[comment]";
                    cmd.CommandText += ",[transtartempcd]";
                    cmd.CommandText += ",[trancompempcd]";
                    cmd.CommandText += ",[inspectempcd]";
                    cmd.CommandText += ",[inspectct]";
                    cmd.CommandText += ",[isnascastart]";
                    cmd.CommandText += ",[isnascaend]";
                    cmd.CommandText += ",[isnascadefectend]";
                    cmd.CommandText += ",[isnascacommentend]";
                    cmd.CommandText += ",[delfg]";
                    cmd.CommandText += ",[isdefectend]";
                    cmd.CommandText += ",[isdefectautoimportend]";
                    cmd.CommandText += ",[isnascarunning]";
                    cmd.CommandText += ",[isautoimport]";
                    cmd.CommandText += ",[isresinmixordered])";
                    cmd.CommandText += " VALUES";
                    cmd.CommandText += $"(('{lotno} ')";
                    cmd.CommandText += $",('{procno}')";
                    cmd.CommandText += $",('{macno}')";
                    cmd.CommandText += $",('{magno}')";
                    cmd.CommandText += $",('{magno}')";
                    cmd.CommandText += $",('{dt.ToString()} ')";
                    cmd.CommandText += $",('{dt.ToString()}')";
                    cmd.CommandText += ",(1)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",''";
                    cmd.CommandText += ",(211105)";
                    cmd.CommandText += ",(211105)";
                    cmd.CommandText += ",''";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(1)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += " ,(0)";
                    cmd.CommandText += " ,(0))";

                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                    con.Close();

                    return true;

                }
            }
            catch (Exception ex)
            {
                ConsoleShow(ex.ToString());
                return false;
            }
        }


        //
        // ロボットマガジンQR書き込み
        //
        private void btn_readRbMagQr_Click(object sender, EventArgs e)
        {
            btn_readRbMagQr.Enabled = false;
            readRbMagQr(txt_rbmagQrr.Text);
            btn_readRbMagQr.Enabled = true;
        }

        private bool readRbMagQr(string qrword)
        {

            //ConsoleShow("QRメモリ領域をクリアしています");
            var allClear = "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0";
            SetString(Robot.robotQRWordAddressStart, allClear);
            if (SetString(Robot.robotQRWordAddressStart, qrword))
            {
                ConsoleShow("ロボットマガジンQRに[" + qrword + "]を書き込みました");
                btn_readRbMagQr.Enabled = true;
                return true;
            }
            else
            {
                ConsoleShow("ロボットマガジンQRのデータ書き込みが失敗しました");
            }

            return false;
        }

        private void btn_clearQrcode_Click(object sender, EventArgs e)
        {
            txt_rbmagQrr.Text = "";
            txt_rbmagQrr.Focus();
        }

        //
        // ロボット動作ボタン
        //
        private void lb_qrrd_Click(object sender, EventArgs e)
        {
            ConsoleShow(Robot.robotQRWordAddressStart);
        }
        private void lb_srcr_Click(object sender, EventArgs e)
        {
            ConsoleShow(Robot.STAT_ROBOT_COMMAND_READY);
        }

        private void lb_rrmo_Click(object sender, EventArgs e)
        {
            ConsoleShow(Robot.REQ_ROBOT_MOVE_ORG2MID);
        }

        private void lb_rmlc_Click(object sender, EventArgs e)
        {
            ConsoleShow(Robot.ROBOT_MOVE2_LINER_COMPLETE);
        }

        private void lb_rrmm_Click(object sender, EventArgs e)
        {
            ConsoleShow(Robot.REQ_ROBOT_MOVE2_MID2DST);
        }

        private void btn_srcr_on_Click(object sender, EventArgs e)
        {
            SetBit(Robot.STAT_ROBOT_COMMAND_READY, "1");
            btn_srcr_on.Enabled = false;
            btn_srcr_off.Enabled = true;
            ConsoleShow("ロボット動作許可しました");
        }

        private void btn_srcr_off_Click(object sender, EventArgs e)
        {
            SetBit(Robot.STAT_ROBOT_COMMAND_READY, "0");
            btn_srcr_on.Enabled = true;
            btn_srcr_off.Enabled = false;
            ConsoleShow("ロボット動作を拒否しました");
        }

        private void btn_rrmo2m_Click(object sender, EventArgs e)
        {
            Rb_GetMag(false);
            SetBit(NicConv_01.magazineArriveBitAddress, "0");
        }

        private void btn_rm2lc_Click(object sender, EventArgs e)
        {
            //ロボットリニア移動完了
            SetBit(Robot.ROBOT_MOVE2_LINER_COMPLETE, "1");
            ConsoleShow("リニア移動完了");
            //ロボットマガジンを置く開始
            SetBit(Robot.REQ_ROBOT_MOVE2_MID2DST, "1");
            ConsoleShow("マガジン置く開始");
        }

        private void btn_rrm2m_Click(object sender, EventArgs e)
        {
            Rb_PutMag();
        }

        private void Rb_GetMag(bool auto=true)
        {
            //ロボットマガジン取る動作
            SetBit(Robot.REQ_ROBOT_MOVE_ORG2MID, "0");
            ConsoleShow("マガジンを取りました");
            SetBit(Robot.MAGAZINE_EXIST_QR_STAGE, "1");
            ConsoleShow("マガジンをQRステージに移動しました");
            //ローダーからマガジン取る
            EntConvMagazine("kara_off");
            if (!auto)
            {
                bt_conv_empMag_on.Enabled = true;
                bt_conv_empMag_on.Text = "投入";
            }
            EntConvMagazine("aoi_off");
            if (!auto)
            {
                bt_conv_aoiMag_on.Enabled = true;
                bt_conv_aoiMag_on.Text = "投入";
            }
            EntConvMagazine("nichia_off");
            if (!auto)
            {
                bt_conv_nichiaMag_on.Enabled = true;
                bt_conv_nichiaMag_on.Text = "投入";
            }
            ConsoleShow("ローダーからマガジンを取りました");
        }

        private void Rb_PutMag()
        {
            //ロボットリニア移動完了
            SetBit(Robot.ROBOT_MOVE2_LINER_COMPLETE, "1");
            ConsoleShow("リニア移動完了");
            //ロボットマガジンを置く開始
            SetBit(Robot.REQ_ROBOT_MOVE2_MID2DST, "1");
            ConsoleShow("マガジン置く開始");
            //ロボットマガジンを置く完了動作
            SetBit(Robot.MAGAZINE_EXIST_QR_STAGE, "1");
            ConsoleShow("マガジンをQRステージから取りました");
            SetBit(Robot.REQ_ROBOT_MOVE2_MID2DST, "0");
            ConsoleShow("マガジンをローダーに置きました");
            Thread.Sleep(1000);
            SetBit(Robot.MAGAZINE_EXIST_QR_STAGE, "0");
        }

        //
        // DBテーブル初期化
        //

        public bool CleanTnVirtualMag(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnVirtualMagのデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnVirtualMag;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnVirtualMagをクリーンしました");

                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        public bool CleanTnTran(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnTranのデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnTran;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnTranをクリーンしました");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        public bool CleanTnMag(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnMagのデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnMag;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnMagをクリーンしました");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        public bool CleanTnLot(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnLotのデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnLot;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnLotをクリーンしました");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        public bool CleanTnLotLog(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnLotLogのデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnLotLog;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnLotLogをクリーンしました");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        public bool CleanTnRestrict(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnRestrictのデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnRestrict;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnRestrictをクリーンしました");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        public bool CleanTnLot_Lens(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnLot(LENS)のデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnLot;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnLotをクリーンしました");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        public bool CleanTnTran_Lens(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnTran(LENS)のデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnTran;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnTranをクリーンしました");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        public bool CleanTnLog_Qcil(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnLog(QCIL)のデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnLOG;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnLog(QCIL)をクリーンしました");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        public bool CleanTnLott_Qcil(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnLOTT(QCIL)のデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnLOTT;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnLOTT(QCIL)をクリーンしました");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        public bool CleanTnQcnr_Qcil(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnQCNR(QCIL)のデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnQCNR;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnQCNR(QCIL)をクリーンしました");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        public bool CleanTnInspection(bool confirm = true)
        {
            try
            {
                if (!confirm || SelectYesNoMessage("TnInspectionのデータを全て削除します") == DialogResult.Yes)
                {
                    using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = "TRUNCATE table TnInspection;";

                        string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                        con.Close();

                        ConsoleShow("TnInspectionをクリーンしました");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SQL FAIL!" + ex.ToString());
                return false;
            }
        }

        //
        // ProcNoコントロール
        //
        private void btn_reset_proc_Click(object sender, EventArgs e)
        {
            // メモリ初期化
            TestMemAndIni();
            // 全ての設備のmachineReadyBitAddressをON
            Cnt_isReadyOnAll(true);

            plcBitData = new Dictionary<string, int>(initPlcBitData);
            plcWordData = new Dictionary<string, string>(initPlcWordData);

            cb_procno.SelectedIndex = 0;
            btn_autoStart.Enabled = true;
        }

        private void btn_inc_proc_Click(object sender, EventArgs e)
        {
            if (cb_procno.SelectedIndex < (cb_procno.Items.Count - 1))
            {
                cb_procno.SelectedIndex += 1;
                btn_autoStart.Enabled = true;
            }
        }

        private void btn_dec_proc_Click(object sender, EventArgs e)
        {
            if (cb_procno.SelectedIndex > 0 )
            {
                cb_procno.SelectedIndex -= 1;
                btn_autoStart.Enabled = true;
            }
        }

        private void btn_autoStart_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                btn_inc_proc.Enabled = true;
                btn_autoStart.Enabled = false;
                AutoRunFunctionRouter(cb_procno.Text);
                
            }
        }

        private void cb_procno_SelectedIndecChenged(object sender, EventArgs e)
        {
            var dictproc = ProcessName[cb_procno.Text].Split('@');
            txt_processname.Text = dictproc[0];
            //btn_inc_proc.Enabled = false;
            TABS.SelectedTab = TABS.TabPages[dictproc[1]];
            if (cb_procno.Text == "2")
            {
                cb_dbovnno.Text = "01";
                cb_dbovn_procno.Text = "2";
                cb_dbovnProfile.Text = "2";
            }
            else if (cb_procno.Text == "7")
            {
                cb_dbovnno.Text = "07";
                cb_dbovn_procno.Text = "7";
                cb_dbovnProfile.Text = "5";
            }
            else if (cb_procno.Text == "4")
            {
                cb_leddb_fromDBOVNno.Text = "01";
            }
        }

        private void AutoRunFunctionRouter(string procno)
        {
            if (procno == "1")
            {
                M1DB_AutoRun();
            }
            else if (procno == "2")
            {
                DVOVN_AutoRun();
            }
            else if (procno == "4")
            {
                LEDDB_AutoRun();
            }
            else if (procno == "5")
            {
                DBPL_AutoRun();
            }
            else if (procno == "7")
            {
                DVOVN_AutoRun();
            }
            else if (procno == "8")
            {
                WBPL_AutoRun();
            }
            else if (procno == "9")
            {
                WB_AutoRun();
            }
            else if (procno == "10")
            {
                AVI_AutoRun();
            }
            else if (procno == "22")
            {
                WBOVN_AutoRun();
            }
            else if (procno == "23")
            {
                SUP_AutoRun();
            }
            else if (procno == "24")
            {
                DAM_AutoRun();
            }
            else if (procno == "12")
            {
                MD_AutoRun();
            }
            else if (procno == "13")
            {
                ECK_AutoRun();
            }
            else if (procno == "18")
            {
                GBMEX_AutoRun();
            }
            else if (procno == "14")
            {
                MDOVN_AutoRun();
            }
        }

        //
        // 共通ファンクション
        //

        // デバックモニタ・デリゲート
        public delegate void ConsoleDelegate(string text, bool cr = true);
        public void ConsoleShow(string text, bool cr = true)
        {
            if (debugMonitor.InvokeRequired)
            {
                ConsoleDelegate d = new ConsoleDelegate(ConsoleShow);
                BeginInvoke(d, new object[] { text, cr });
            }
            else
            {
                if (cr)
                    debugMonitor.AppendText(text + CR);
                else
                    debugMonitor.AppendText(text);
            }
        }

        // 作業開始確認ダイアログ
        public DialogResult SelectYesNoMessage(string msg)
        {
            string message = msg;
            string caption = "CAUTION!";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            var result = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Question);
            return result;
        }

        // ファイル作成
        public bool CreateFile(string FilePath, string Contents, ref string msg, string enccode = "utf-8")
        {
            try
            {
                // Create the file, or overwrite if the file exists.
                if (enccode == "utf-8")
                {
                    using (FileStream fs = File.Create(FilePath))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(Contents.TrimEnd('\r', '\n'));
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }
                else
                {
                    Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                    StreamWriter stw = new StreamWriter(FilePath, false, sjisEnc);
                    stw.WriteLine(Contents.TrimEnd('\r', '\n'));
                    stw.Close();
                }

                return true;
            }

            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        //
        /* テキストファイル全読み込み関数
         * OK: true
         * NG: false
         * 読込内容：contents 
         */
        //
        public bool ReadTextFile(string filepath, ref string contents, string enccode)
        {
            try
            {
                StreamReader sr = new StreamReader(filepath, Encoding.GetEncoding(enccode));
                contents = sr.ReadToEnd();
                //contents = new string(contents.Where(c => !char.IsControl(c)).ToArray());
                sr.Close();

                return true;
            }
            catch (Exception ex)
            {
                contents = ex.Message;
                return false;
            }
        }

        //ファイルコピー
        public bool CopyFile(string FilePath, string MoveToPath)
        {
            try
            {
                File.Copy(FilePath, MoveToPath, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //
        public bool CreateMLog(string OriginPath, string filepath, Dictionary<string, string> logdct, ref string msg, string enccode = "UTF-8")
        {
            try
            {
                var contents = "";
                if (ReadTextFile(OriginPath, ref contents, enccode))
                {
                    foreach (var item in logdct)
                    {
                        contents = contents.Replace(item.Key, item.Value);
                    }
                }
                else return false;
                
                CreateFile(filepath, contents, ref msg, enccode);
                return true;
            }
            catch(Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        // ロボット稼働許可ポーリング
        public bool RBMoveEnableToGetMag()
        {
            if (!DebugMode)
            {
                var cnt = 0;
                while (cnt < 45)
                {
                    Thread.Sleep(2000);
                    if (GetBit(Robot.REQ_ROBOT_MOVE_ORG2MID) == "1")
                    {
                        ConsoleShow("RB移動要求確認");
                        SetBit(Robot.REQ_ROBOT_MOVE_ORG2MID, "0");
                        return true;
                    }
                    ConsoleShow(">", false);
                    cnt++;
                }
                ConsoleShow("RB移動要求タイムアウト");
                return false;
            }
            else
            {
                return true;
            }
        }
        // ロボットMag取る置く動作自動関数
        public bool RBGetPutMagAuto()
        {
            // +++ RB Polling +++
            ConsoleShow("+++ ロボット移動要求確認 +++");
            if (!RBMoveEnableToGetMag())
            {
                ConsoleShow("■■■自動投入停止■■■");
                return false;
            }

            // ロボット動作（Mag取る）
            Rb_GetMag();
            ConsoleShow("Mag取り動作");
            SetBit(NicConv_01.unloaderReqBitAddress, "0");
            SetBit(NicConv_01.magazineArriveBitAddress, "0");

            // +++ Wait +++
            ConsoleShow("+++ Wait:3秒 +++");
            Thread.Sleep(3000);

            // ロボット動作（Mag置く）
            Rb_PutMag();
            ConsoleShow("Mag置く動作");

            // +++ Wait +++
            ConsoleShow("+++ Wait:3秒 +++");
            Thread.Sleep(3000);

            return true;
        }

        public void cleanMacLogFolders()
        {
            var macList = new List<Machine>
            {
                ZeDBComboRouter(cb_zedbno.Text), 
                LEDDBComboRouter(cb_leddbno.Text),
                WBComboRouter(cb_wbno.Text),
                AVI_01,
                SUP_01,
                MDComboRouter(cb_mb_macno.Text),
                ECK_01
            };
            var MLfolderpath = "";
            foreach (var mac in macList)
            {
                if (mac.getMacFolderPath("in", ref MLfolderpath))
                {
                    cleanFolder(MLfolderpath);
                }
            }
        }

        public void cleanFolder(string folderpath)
        {
            foreach (string f in Directory.GetFiles(folderpath, "*.*"))
            {
                File.Delete(f);
            }
        }

        //
        // DB処理
        //

        // テーブル初期化
        private void CleanTbls()
        {
            CleanTnVirtualMag(false);
            CleanTnMag(false);
            CleanTnTran(false);
            CleanTnLot(false);
            CleanTnLotLog(false);
            CleanTnRestrict(false);
            CleanTnLot_Lens(false);
            CleanTnTran_Lens(false);
            CleanTnLog_Qcil(false);
            CleanTnLott_Qcil(false);
            CleanTnQcnr_Qcil(false);
            CleanTnInspection(false);
        }

        // DBダミーデータ処理
        private void btn_dbcnt_exec_Click(object sender, EventArgs e)
        {
            if (cb_procno.SelectedIndex > 0)
            {
                var dblogfpath = "C:\\ARMS\\DB\\";
                var fname_tntran = "tntran.csv";
                var fname_tnmag = "tnmag.csv";
                var fname_tntranlens = "tntran_lens.csv";
                var tntranloglist = new List<string>();
                var tnmagloglist = new List<string>();
                var tntranlensloglist = new List<string>();
                var dbcnt = new DbControls();
                var typecd = txt_dbcnt_typecd.Text;
                var lotno = txt_dbcnt_lotno.Text;
                var dt = DateTime.Now;
                var dtformed = dt.ToString("yyyyMMddHHmmss");
                var msg = string.Empty;
                var curProcno = string.Empty;

                if (string.IsNullOrEmpty(lotno))
                {
                    lotno = "test" + dtformed;
                    txt_dbcnt_lotno.Text = lotno;
                }

                var swapStrings = new Dictionary<string, string>
                {
                    { "@lotno", lotno }
                };

                // TnMag, TnVirtualMag Clean
                if (SelectYesNoMessage("各種トランザクションデータを初期化・DBの途中投入準備します") == DialogResult.Yes)
                {
                    CleanTbls();

                    // DBhistoryフォルダクリーン
                    foreach (string f in Directory.GetFiles(@"C:\ARMS\DBhistory", "*.*"))
                    {
                        File.Delete(f);
                    }
                    // DBhistoryフォーマットコピー
                    foreach (string f in Directory.GetFiles(@"C:\ARMS\DBhistory\format", "*.*"))
                    {
                        CopyFile(f, @"C:\ARMS\DBhistory\" + Path.GetFileName(f));
                    }

                    // TnLot Insert
                    dt = DateTime.Now;
                    dtformed = dt.ToString("yyyy/MM/dd HH:mm:ss");
                    tnlot.lotno = lotno;
                    tnlot.typecd = typecd;
                    tnlot.templotno = lotno;
                    tnlot.lastupddt = dtformed;
                    tnlot.dbthrowdt = dtformed;
                    if (tnlot.insertTnlot(ref msg))
                    {
                        ConsoleShow($"TnLot: Insert OK");
                    }
                    else
                    {
                        ConsoleShow(msg);
                        ConsoleShow($"TnLot: Insert NG");
                    }

                    // TnLot_Lens Insert
                    dt = DateTime.Now;
                    dtformed = dt.ToString("yyyy/MM/dd HH:mm:ss");
                    tnlotlens.LotNO = lotno;
                    tnlotlens.TypeCD = typecd;
                    tnlotlens.LastupdDT = dtformed;
                    if (tnlotlens.InsertTnlot(ref msg))
                    {
                        ConsoleShow($"TnLot_Lens: Insert OK");
                    }
                    else
                    {
                        ConsoleShow(msg);
                        ConsoleShow($"TnLot_Lens: Insert NG");
                    }

                    // トランザクションダミーデータ読込
                    dbcnt.ReadTextFileLine(dblogfpath + fname_tntran, ref tntranloglist);
                    dbcnt.ReadTextFileLine(dblogfpath + fname_tnmag, ref tnmagloglist);
                    dbcnt.ReadTextFileLine(dblogfpath + fname_tntranlens, ref tntranlensloglist);

                    // Procno毎に処理
                    for (int i = 0; i < cb_procno.SelectedIndex; i++)
                    {
                        // TnTran
                        foreach (var log in tntranloglist)
                        {
                            var _log = log;
                            foreach (var item in swapStrings)
                            {
                                _log = _log.Replace(item.Key, item.Value);
                            }
                            var tran = _log.Split(',');
                            var procno = int.Parse(tran[1]);
                            curProcno = cb_procno.Items[i].ToString();
                            dt = DateTime.Now;
                            dtformed = dt.ToString("yyyy/MM/dd HH:mm:ss");

                            if ((procno == int.Parse(curProcno))
                               || (procno == 25 && int.Parse(curProcno) == 24)
                               || (procno == 26 && int.Parse(curProcno) == 24)
                               || (procno == 16 && int.Parse(curProcno) == 18))
                            {
                                tntran.lotno = tran[0];
                                tntran.procno = tran[1];
                                tntran.macno = tran[2];
                                tntran.inmag = tran[3];
                                tntran.outmag = tran[4];
                                tntran.startdt = dtformed;
                                tntran.enddt = dtformed;
                                tntran.iscomplt = tran[7];
                                tntran.stocker1 = tran[8];
                                tntran.stocker2 = tran[9];
                                tntran.comment = tran[10];
                                tntran.transtartempcd = tran[11];
                                tntran.trancompempcd = tran[12];
                                tntran.inspectempcd = tran[13];
                                tntran.inspectct = tran[14];
                                tntran.isnascastart = tran[15];
                                tntran.isnascaend = tran[16];
                                tntran.lastupddt = tran[17];
                                tntran.isnascadefectend = tran[18];
                                tntran.isnascacommentend = tran[19];
                                tntran.delfg = tran[20];
                                tntran.isdefectend = tran[21];
                                tntran.isdefectautoimportend = tran[22];
                                tntran.isnascarunning = tran[23];
                                tntran.isautoimport = tran[24];
                                tntran.isresinmixordered = tran[25];

                                if (tntran.InsertTnTran(ref msg))
                                {
                                    ConsoleShow($"TnTran: ProcNo.{procno}⇒OK");
                                }
                                else
                                {
                                    //ConsoleShow(msg);
                                    ConsoleShow($"TnTran: ProcNo.{procno}⇒NG");
                                }
                            }
                        }
                        //TnMag
                        foreach (var log in tnmagloglist)
                        {
                            var _log = log;
                            foreach (var item in swapStrings)
                            {
                                _log = _log.Replace(item.Key, item.Value);
                            }
                            var mag = _log.Split(',');
                            var procno = int.Parse(mag[2]);
                            if (procno == int.Parse(cb_procno.Items[i].ToString()))
                            {
                                dt = DateTime.Now;
                                dtformed = dt.ToString("yyyy/MM/dd HH:mm:ss");
                                tnmag.lotno = mag[0];
                                tnmag.magno = mag[1];
                                tnmag.inlineprocno = mag[2];
                                tnmag.newfg = mag[3];
                                tnmag.lastupddt = dtformed;

                                if (tnmag.InsertTnMag(ref msg))
                                {
                                    ConsoleShow($"TnMag: ProcNo.{procno}⇒OK");
                                }
                                else
                                {
                                    ConsoleShow(msg);
                                    ConsoleShow($"TnMag: ProcNo.{procno}⇒NG");
                                }
                            }
                        }
                        //TnTran_Lens
                        foreach (var log in tntranlensloglist)
                        {
                            var _log = log;
                            foreach (var item in swapStrings)
                            {
                                _log = _log.Replace(item.Key, item.Value);
                            }
                            var tranlens = _log.Split(',');
                            var procno = int.Parse(tranlens[1]);
                            if (procno == int.Parse(cb_procno.Items[i].ToString()))
                            {
                                dt = DateTime.Now;
                                dtformed = dt.ToString("yyyy/MM/dd HH:mm:ss");
                                tntranlens.LotNO = tranlens[0];
                                tntranlens.ProcNO = tranlens[1];
                                tntranlens.PlantCD = tranlens[2];
                                tntranlens.StartDT = dtformed;
                                tntranlens.EndDT = dtformed;
                                tntranlens.IsCompleted = tranlens[5];
                                tntranlens.LastUpdDT = dtformed;
                                tntranlens.LastUpdDT = dtformed;
                                tntranlens.CarrierNO = tranlens[8];

                                if (tntranlens.InsertTnTran(ref msg))
                                {
                                    ConsoleShow($"TnTran_lens: ProcNo.{procno}⇒OK");
                                }
                                else
                                {
                                    //ConsoleShow(msg);
                                    ConsoleShow($"TnTran_lens: ProcNo.{procno}⇒NG");
                                }
                            }
                        }
                    }
                    // Update TnMag
                    if (curProcno == "12")
                    {
                        if (tnmag.UpdateTnMag(lotno + "_#1", "ME0001", curProcno, ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.ME0001⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.ME0001⇒Updateできませんでした");
                        }
                        if (tnmag.UpdateTnMag(lotno + "_#2", "ME0002", curProcno, ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.ME0002⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.ME0002⇒Updateできませんでした");
                        }
                    }
                    if (curProcno == "13")
                    {
                        if (tnmag.UpdateTnMag(lotno + "_#1", "ME0001", curProcno, ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.ME0001⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.ME0001⇒Updateできませんでした");
                        }
                        if (tnmag.UpdateTnMag(lotno + "_#2", "ME0002", curProcno, ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.ME0002⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.ME0002⇒Updateできませんでした");
                        }
                    }
                    else if (curProcno == "24")
                    {
                        if (tnmag.UpdateTnMag(lotno, tntran.outmag, "26", ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.ME0001⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.{tntran.outmag}⇒Updateできませんでした");
                        }
                    }
                    else if (curProcno == "18")
                    {
                        if (tnmag.UpdateTnMag(lotno, "M00005", "16", ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.M00005⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.M00005⇒Updateできませんでした");
                        }
                    }
                    else
                    {
                        if (tnmag.UpdateTnMag(lotno, tntran.outmag, curProcno, ref msg))
                        {
                            ConsoleShow($"TnMag: Magno.{tntran.outmag}⇒Updated");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnMag: Magno.{tntran.outmag}⇒Updateできませんでした");
                        }
                    }
                    //Insert TnInspection
                    if (curProcno == "9")
                    {
                        var TnInsp = new TnInspection() { lotno = lotno };
                        if (TnInsp.InsertTnMag(ref msg))
                        {
                            ConsoleShow($"TnInspection: Inserted");
                        }
                        else
                        {
                            //ConsoleShow(msg);
                            ConsoleShow($"TnInspection: Insertできませんでした");
                        }
                    }

                    // Log出力
                    cleanMacLogFolders(); //全クリーン

                    if (int.Parse(curProcno) > 8)
                    {
                        //
                        // log file words
                        //
                        var macname = cb_wbno.Text;
                        var MLfolderpath = "";
                        var macInstance = WBComboRouter(macname);
                        if (!macInstance.getMacFolderPath("in", ref MLfolderpath))
                        {
                            ConsoleShow(MLfolderpath);
                            return;
                        }
                        var logfiledict = new Dictionary<string, string>
                    {
                        { "@magno", tx_wb_m_auto.Text }
                    };
                        var Targetfld = "C:\\QCIL\\data\\WB\\MachineLog\\" + lotno + "\\";
                        if (!Directory.Exists(Targetfld))
                        {
                            Directory.CreateDirectory(Targetfld);
                        }
                        // MMファイル
                        dt = DateTime.Now;
                        var mmfilename = "MM0000" + dt.ToString("yyMMddhhmm") + "_" + lotno + "_CL-A160-1W9-S4_9_" + tx_wb_m_auto.Text.Replace("C30 ", "") + ".CSV";
                        if (!CreateMLog("C:\\QCIL\\MacLogOrigin\\mapping\\MM_wb.CSV", Targetfld + mmfilename, logfiledict, ref msg, "shift-jis"))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                        // Mapファイル
                        if (!CopyFile("C:\\QCIL\\MacLogOrigin\\mapping\\map_wb.wbm", "C:\\QCIL\\data\\AI\\Mapping\\" + lotno + ".wbm"))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                    }
                    if (int.Parse(curProcno) > 9)
                    {
                        //
                        // log file words
                        //
                        var MLfolderpath = "";
                        var macInstance = AVI_01;
                        if (!macInstance.getMacFolderPath("in", ref MLfolderpath))
                        {
                            ConsoleShow(MLfolderpath);
                            return;
                        }

                        //var logfiledict = new Dictionary<string, string>
                        //{
                        //    { "@magno", txt_avi_jitumag.Text.Replace("C30 ","") }
                        //};
                        //// MMファイル
                        //var mmfilename = "log000_MM00000000_" + lotno + "_CL-A160-1W9-S4_" + txt_avi_jitumag.Text.Replace("C30 ","") + ".CSV";
                        //if (!CreateMLog("C:\\QCIL\\MacLogOrigin\\mapping\\MM_avi.CSV", Targetfld + mmfilename, logfiledict, ref msg, "shift-jis"))
                        //{
                        //    ConsoleShow(msg);
                        //    return;
                        //}
                        var Targetfld = "C:\\QCIL\\data\\AI\\Mapping\\" + dt.ToString("yyyyMM") + "\\";
                        if (!Directory.Exists(Targetfld))
                        {
                            Directory.CreateDirectory(Targetfld);
                        }
                        // Mapファイル
                        if (!CopyFile("C:\\QCIL\\MacLogOrigin\\mapping\\map_wb.wbm", Targetfld + lotno + ".wbm"))
                        {
                            ConsoleShow(msg);
                            return;
                        }

                        Targetfld = "C:\\QCIL\\data\\AI\\MachineLog\\" + lotno + "\\";
                        if (!Directory.Exists(Targetfld))
                        {
                            Directory.CreateDirectory(Targetfld);
                        }

                        var logfiledict = new Dictionary<string, string>
                        {
                            { "@magno", txt_avi_jitumag.Text}
                        };
                        //Targetfld = MLfolderpath + dt.ToString("yyyyMM") + "\\" + lotno + "\\";
                        if (!Directory.Exists(Targetfld))
                        {
                            Directory.CreateDirectory(Targetfld);
                        }
                        // MMファイル
                        dt = DateTime.Now;
                        var mmfilename = "log000_MM" + dt.ToString("yyMMddhhmm") + "_" + lotno + "_CL-A160-1W9-S4_10_" + txt_avi_jitumag.Text.Replace("C30 ", "") + ".CSV";
                        if (!CreateMLog("C:\\QCIL\\MacLogOrigin\\mapping\\MM_avi.CSV", Targetfld + mmfilename, logfiledict, ref msg, "shift-jis"))
                        {
                            ConsoleShow(msg);
                            return;
                        }

                        // Mapファイル
                        if (!CopyFile("C:\\QCIL\\MacLogOrigin\\mapping\\map_avi.mpd", "C:\\QCIL\\Mapping\\" + lotno + ".mpd"))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                    }
                }
            }
            else
            {
                ConsoleShow("初工程のためDBへのダミー書き込みはできません");
            }
        }

        // RecordTn
        public void RecordTn(string header)
        {
            var msg = string.Empty;
            tnlot.RecordTrData(header, ref msg);
            tntran.RecordTrData(header, ref msg);
            tnlotlog.RecordTrData(header, ref msg);
            tnrestrict.RecordTrData(header, ref msg);
            tnlotlens.RecordTrData(header, ref msg);
            tntranlens.RecordTrData(header, ref msg);
            tnlogqcil.RecordTrData(header, ref msg);
            tnlogqcil.RecordTrData(header, ref msg);
            tnqcnrqcil.RecordTrData(header, ref msg);

        }

        // 最新のロットナンバーをTnTranから取得
        public bool getLastLotNofromTnTran(ref string lotno)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = $"SELECT[lotno] ";
                    cmd.CommandText += $" FROM[dbo].[TnTran] ";
                    cmd.CommandText += $"WHERE startdt = (select MAX(startdt) from[dbo].[TnTran])";

                    lotno = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                    con.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ConsoleShow(ex.ToString());
                return false;
            }
        }
        public string getLastLotNo()
        {
            string lotno = "";
            if (getLastLotNofromTnTran(ref lotno))
            {
                return lotno;
            }
            else
            {
                ConsoleShow("ロットNOの取得が失敗しました");
                return string.Empty;
            }
        }

        // TnMagのアップデート
        public bool updateTnMag(int procno, string lotno, string magno)
        {
            try
            {
                magno = magno.Replace("C30 ", "");
                DateTime dt = DateTime.Now;

                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = $"UPDATE [dbo].[TnMag] ";
                    cmd.CommandText += $"SET [inlineprocno] = ({procno}) ";
                    cmd.CommandText += $"WHERE lotno = '{lotno}' and magno = '{magno}'";

                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                    con.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ConsoleShow(ex.ToString());
                return false;
            }
        }


        //
        // 自動化
        //

        //
        // Ze Die Bonder
        //
        private void btn_M1DB_AutoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                M1DB_AutoRun();
                
            }
        }
        private async void M1DB_AutoRun()
        {
            var procno = "1";
            var procname = "ZE-DB";
            var macname = cb_zedbno.Text;
            string msg = "";
            var karamagno = tx_m1db_empm_auto.Text;
            var aoimagno = tx_m1db_aoim_auto.Text;
            var macparam = cmb_zdbpram.Text;

            //
            // log file words
            //
            var MLfolderpath = "";
            var macInstance = ZeDBComboRouter(macname);
            if (!macInstance.getMacFolderPath("in", ref MLfolderpath))
            {
                ConsoleShow(MLfolderpath);
                return;
            }
            var date = DateTime.Now.ToString("yyyy/MM/dd");
            var time = DateTime.Now.ToString("HH:mm:ss");
            var logfiledict = new Dictionary<string, string>
            {
                { "@kishumei", $"{EicsParam_TypeCode}" },
                { "@date", $"{date}" },
                { "@time", $"{time}" }
            };
            var MLSriname = "0000000.000";
            var dtstr = DateTime.Now.ToString("yyyyMMddhhmmss");

            //
            if (SelectYesNoMessage("各種トランザクションデータを初期化します") == DialogResult.Yes)
            {
                CleanTbls();
            }
            // DBhistoryフォルダクリーン
            foreach (string f in Directory.GetFiles(@"C:\ARMS\DBhistory", "*.*"))
            {
                File.Delete(f);
            }
            // DBhistoryフォーマットコピー
            foreach (string f in Directory.GetFiles(@"C:\ARMS\DBhistory\format", "*.*"))
            {
                CopyFile(f, @"C:\ARMS\DBhistory\" + Path.GetFileName(f));
            }
            // 装置フォルダクリーン
            cleanMacLogFolders(); //全クリーン

            // EICSスタートタイミング開始確認
            workheader = "procno." + procno + ":EICSスタートタイミング開始";
            if (SelectYesNoMessage(workheader + "。EICSは稼働中ですか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□macno={macname}: {macInstance.macname}□□□");
                    // MLフォルダクリーン
                    foreach (string f in Directory.GetFiles(MLfolderpath, "*.*"))
                    {
                        File.Delete(f);
                    }

                    if (macparam != "全パラメータ")
                    {
                        ConsoleShow("■■■O,Pファイルを出力開始■■■");
                        // 設備ログ(O,Pファイル)を吐き出し
                        // Oファイル
                        if (!CreateMLog(macInstance.macLogOriginFolder + "O.csv", MLfolderpath + "O" + MLSriname, logfiledict, ref msg))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                        // Pファイル
                        if (!CreateMLog(macInstance.macLogOriginFolder + "P.csv", MLfolderpath + "P" + MLSriname, logfiledict, ref msg))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                        ConsoleShow("■■■O,Pファイルを出力完了■■■");
                    }
                    else
                    {
                        ConsoleShow("■■■全パラファイルを出力開始■■■");
                        if (!CreateMLog(macInstance.macLogOriginFolder + "ViP-ZD-test_.txt", MLfolderpath + "ViP-ZD-test_" + dtstr + ".txt", logfiledict, ref msg))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                        ConsoleShow("■■■全パラファイルを出力完了■■■");
                    }

                    Thread.Sleep(2000);
                });

                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // マガジン投入開始確認
            workheader = "procno." + procno + ":マガジン投入開始";
            if (SelectYesNoMessage(workheader + "。ARMSは稼働中ですか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    // ビットメモリ初期化
                    if (cb_m1db_exclnbitmem.Checked)
                    {
                        if (!TestMemAndIni())
                        {
                            return;
                        }
                    }
                    Thread.Sleep(1000);

                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    // 空マガ書き込み
                    ConsoleShow("■■■空マガジン投入■■■");
                    if (!readRbMagQr(karamagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    // EmpMagLoaderReq
                    SetBit(ZeDBComboRouter(macname).empMagLoaderReqBitAddress, "1");
                    ConsoleShow("空マガジン供給を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:2秒 +++");
                    Thread.Sleep(2000);

                    // 空マガジン投入
                    EntConvMagazine("kara_on");
                    ConsoleShow("ローダー空マガジン排出を要求しました");

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    // EmpMagLoaderReq
                    SetBit(ZeDBComboRouter(macname).empMagLoaderReqBitAddress, "0");
                    ConsoleShow("空マガジン供給OFFしました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:10秒 +++");
                    Thread.Sleep(10000);

                    ConsoleShow("停止中");
                });

                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }


            // 実マガジン～
            workheader = "procno." + procno + ":実マガジンを投入";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    // 実マガ書き込み
                    ConsoleShow("■■■実マガジン投入■■■");
                    if (!readRbMagQr(aoimagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    // MagLoaderReq ON
                    SetBit(ZeDBComboRouter(macname).loaderReqBitAddress, "1");
                    ConsoleShow("実マガジン供給要求ONしました");
                    Thread.Sleep(1000);

                    // aoiマガジン投入
                    EntConvMagazine("aoi_on");
                    ConsoleShow("ローダー実(aoi)マガジン排出を要求しました");

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    //// waferBitAddressStart
                    //SetBit(ZeDBComboRouter(macname).waferBitAddressStart, "1");
                    //ConsoleShow("waferBitAddressStartをONしました");

                    //// startWorkTableSensorAddress
                    //SetBit(ZeDBComboRouter(macname).startWorkTableSensorAddress, "1");
                    //ConsoleShow("startWorkTableSensorAddressをONしました");

                    // MagLoaderReq OFF
                    SetBit(ZeDBComboRouter(macname).loaderReqBitAddress, "0");
                    ConsoleShow("実マガジン供給要求OFFしました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:10秒 +++");
                    Thread.Sleep(10000);

                    ConsoleShow("停止中");
                });

                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // 工程完了登録：EICSマガジンタイミング
            workheader = "procno." + procno + ":EICSマガジンタイミングを開始";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                ConsoleShow("■■■B,E,H,I,K,L,N,Wファイルを出力開始■■■");

                await Task.Run(() =>
                {
                    if (!CreateMLog(macInstance.macLogOriginFolder + "B.csv", MLfolderpath + "B" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "E.csv", MLfolderpath + "E" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "H.csv", MLfolderpath + "H" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "I.csv", MLfolderpath + "I" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "K.csv", MLfolderpath + "K" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "N.csv", MLfolderpath + "N" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "W.csv", MLfolderpath + "W" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "L.csv", MLfolderpath + "L" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }

                    // +++ Wait +++
                    Thread.Sleep(2000);

                    ConsoleShow("■■■B,E,H,I,K,L,N,Wファイルを出力完了■■■");
                });

                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // 工程完了登録
            workheader = "procno." + procno + ":工程完了開始";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■実マガジン排出開始■■■");

                    // 空マガNoを再度書き込み（ロボットがなぜか持ちだすことがあるっぽい）
                    if (!readRbMagQr(karamagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);
                    ConsoleShow("マガジンNoを再度書き込みました。");

                    //実マガジン排出要求ON
                    SetBit(ZeDBComboRouter(macname).unloaderReqBitAddress, "1");
                    ConsoleShow("実マガジン排出要求ON");

                    //// +++ Wait +++
                    //ConsoleShow("+++ Wait:10秒 +++");
                    //Thread.Sleep(10000);

                    //// waferBitAddressStart
                    //SetBit(ZeDBComboRouter(macname).waferBitAddressStart, "0");
                    //ConsoleShow("waferBitAddressStartをOFFしました");

                    //// startWorkTableSensorAddress
                    //SetBit(ZeDBComboRouter(macname).startWorkTableSensorAddress, "0");
                    //ConsoleShow("startWorkTableSensorAddressをOFFしました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:20秒 +++");
                    Thread.Sleep(20000);

                    ConsoleShow("■■■DB装置アンローダー上で停止■■■");
                });

                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }


        //
        // DB OVEN
        //
        private void btn_DVOVN_AutoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                DVOVN_AutoRun();
                
            }
        }
        private async void DVOVN_AutoRun()
        {
            var procno = cb_dbovn_procno.Text;
            var procname = "DB-OVN";
            var DBOvnNo = cb_dbovnno.Text;
            var leddbmacname = cb_leddbno.Text;
            var ZeDBNo = cb_zedbno.Text;
            var jitumag = tx_dvovn_m_auto.Text;
            var profileno = int.Parse(cb_dbovnProfile.Text);

            // 実マガジン～
            workheader = "procno." + procno + ":DBOVN投入開始";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    if (procno == "2")
                    {
                        ConsoleShow("■■■DB装置から実マガジン投入■■■");

                        ConsoleShow("排出要求ON");
                        if (string.IsNullOrEmpty(ZeDBComboRouter(ZeDBNo).unloaderReqBitAddress))
                        {
                            ConsoleShow("Zeダイボンダーではありません");
                            return;
                        }
                        SetBit(ZeDBComboRouter(ZeDBNo).unloaderReqBitAddress, "1");

                        // プロファイル設定
                        ConsoleShow("オーブンプロファイルを2に設定");
                        SetWordAsDecimalData(DBOvnComboRouter(DBOvnNo).currentProfileWordAddress, 2);
                    }
                    else if (procno == "7")
                    {
                        // 実マガ書き込み
                        ConsoleShow("■■■CV実マガジン投入■■■");
                        if (!readRbMagQr(jitumag))
                        {
                            return;
                        }
                        Thread.Sleep(1000);

                        // 途中投入？
                        if (cb_dbovn_useniccv.Checked)
                        {
                            if (!NicConvQR(jitumag))
                            {
                                return;
                            }
                            Thread.Sleep(1000);
                            // 途中投入排出要求ON
                            SetBit(NicConv_01.unloaderReqBitAddress, "1");
                            SetBit(NicConv_01.magazineArriveBitAddress, "1");
                            ConsoleShow("途中マガジン投入を要求しました");
                        }
                        else
                        {
                            // OVN排出要求ON
                            Thread.Sleep(1000);
                            SetBit(LEDDBComboRouter(leddbmacname).unloaderReqBitAddress, "1");
                            ConsoleShow("DB(LED)の実マガジン排出を要求しました");
                        }

                        // プロファイル設定
                        ConsoleShow("オーブンプロファイルを5に設定");
                        SetWordAsDecimalData(DBOvnComboRouter(DBOvnNo).currentProfileWordAddress, 5);
                    }
                    else
                    {
                        return;
                    }

                    ConsoleShow("供給要求ON");
                    SetBit(DBOvnComboRouter(DBOvnNo).loaderReqBitAddressList_addKey1, "1");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // ブリッジ搬送
                    if (cb_dbovn_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }

                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    if (procno == "2")
                    {
                        // 排出要求OFF
                        ConsoleShow("排出要求OFF");
                        SetBit(ZeDBComboRouter(ZeDBNo).unloaderReqBitAddress, "0");
                    }
                    else
                    {
                        if (cb_dbovn_useniccv.Checked)
                        {
                            SetBit(LEDDBComboRouter(leddbmacname).unloaderReqBitAddress, "0");
                            ConsoleShow("DB(LED)の実マガジン排出をOFFしました");
                        }
                    }

                    // 供給要求OFF
                    SetBit(DBOvnComboRouter(DBOvnNo).loaderReqBitAddressList_addKey1, "0");
                    ConsoleShow("供給要求OFF");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    ConsoleShow("■■■OVN装置上で停止（完了処理前）■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        //
        // LED DB
        //
        private void btn_LEDDB_AutoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                LEDDB_AutoRun();
                
            }
        }
        private async void LEDDB_AutoRun()
        {
            var procno = "4";
            var procname = "LED-DB";
            var macname = cb_leddbno.Text;
            string msg = "";
            var karamagno = tx_leddb_empm_auto.Text;
            var jitumagno = tx_leddb_m_auto.Text;
            var ovnmacno = cb_leddb_fromDBOVNno.Text;
            var macparam = cmb_zdbpram.Text;

            //
            // log file words
            //
            var MLfolderpath = "";
            var macInstance = LEDDBComboRouter(macname);
            if (!macInstance.getMacFolderPath("in", ref MLfolderpath))
            {
                ConsoleShow(MLfolderpath);
                return;
            }
            var date = DateTime.Now.ToString("yyyy/MM/dd");
            var time = DateTime.Now.ToString("HH:mm:ss");
            var logfiledict = new Dictionary<string, string>
            {
                { "@kishumei", $"{EicsParam_TypeCode}" },
                { "@date", $"{date}" },
                { "@time", $"{time}" }
            };
            var MLSriname = "0000000.000";
            var dtstr = DateTime.Now.ToString("yyyyMMddhhmmss");
            //

            // EICSスタートタイミング開始確認
            workheader = "procno." + procno + ":EICSスタートタイミング開始";
            if (SelectYesNoMessage(workheader + "。EICSは稼働中ですか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□macno={macname}: {macInstance.macname}□□□");
                    // MLフォルダクリーン
                    foreach (string f in Directory.GetFiles(MLfolderpath, "*.*"))
                    {
                        File.Delete(f);
                    }

                    if (macparam != "全パラメータ") 
                    {
                        ConsoleShow("■■■O,Pファイルを出力開始■■■");
                        // 設備ログ(O,Pファイル)を吐き出し
                        // Oファイル
                        if (!CreateMLog(macInstance.macLogOriginFolder + "O.csv", MLfolderpath + "O" + MLSriname, logfiledict, ref msg))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                        // Pファイル
                        if (!CreateMLog(macInstance.macLogOriginFolder + "P.csv", MLfolderpath + "P" + MLSriname, logfiledict, ref msg))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                        ConsoleShow("■■■O,Pファイルを出力完了■■■");
                    }
                    else
                    {
                        ConsoleShow("■■■全パラファイルを出力開始■■■");
                        if (!CreateMLog(macInstance.macLogOriginFolder + "ViP-LED-test-2_.txt", MLfolderpath + "ViP-LED-test_" + dtstr + ".txt", logfiledict, ref msg))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                        ConsoleShow("■■■全パラファイルを出力完了■■■");
                    }

                    Thread.Sleep(2000);
                });

                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            // 空マガジン～
            workheader = "procno." + procno + ":空マガジンを投入";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    // 空マガ書き込み
                    ConsoleShow("■■■空マガジン投入■■■");
                    if (!readRbMagQr(karamagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    // EmpMagLoaderReq
                    SetBit(LEDDBComboRouter(macname).empMagLoaderReqBitAddress, "1");
                    ConsoleShow("空マガジン供給を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:2秒 +++");
                    Thread.Sleep(2000);

                    // 空マガジン投入
                    EntConvMagazine("kara_on");
                    ConsoleShow("ローダー空マガジン排出を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // ブリッジ搬送
                    if (cb_leddb_emmoveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");
                        SetBit(LineBridge_01.empMagUnloaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }

                        // 空排出OFF
                        SetBit(LineBridge_01.empMagUnloaderReqBitAddress, "0");
                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    // EmpMagLoaderReq
                    SetBit(LEDDBComboRouter(macname).empMagLoaderReqBitAddress, "0");
                    ConsoleShow("空マガジン供給OFFしました");

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            // 実マガジン～
            workheader = "procno." + procno + ":実マガジンを投入";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    // 実マガ書き込み
                    ConsoleShow("■■■OVN完了／実マガジン投入■■■");
                    if (!readRbMagQr(jitumagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    // MagLoaderReq ON
                    SetBit(LEDDBComboRouter(macname).loaderReqBitAddress, "1");
                    ConsoleShow("実マガジン供給要求ONしました");
                    Thread.Sleep(1000);

                    // 途中投入？
                    if (cb_leddb_useniccv.Checked)
                    {
                        if (!NicConvQR(jitumagno))
                        {
                            return;
                        }
                        Thread.Sleep(1000);
                        // 途中投入排出要求ON
                        SetBit(NicConv_01.unloaderReqBitAddress, "1");
                        SetBit(NicConv_01.magazineArriveBitAddress, "1");
                        ConsoleShow("途中マガジン投入を要求しました");
                    }
                    else
                    {
                        // OVN排出要求ON
                        SetDataTimeToOvenPlc(DBOvnComboRouter(ovnmacno));
                        ConsoleShow("開始完了時刻を書き込んでいます:Wait1秒");
                        Thread.Sleep(1000);
                        SetBit(DBOvnComboRouter(ovnmacno).unloaderReqBitAddressList_addKey1, "1");
                        ConsoleShow("オーブンの実マガジン排出を要求しました");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // ブリッジ搬送
                    if (cb_leddb_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }
                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    //// waferBitAddressStart
                    //SetBit(LEDDBComboRouter(macname).waferBitAddressStart, "1");
                    //ConsoleShow("waferBitAddressStartをONしました");

                    //// startWorkTableSensorAddress
                    //SetBit(LEDDBComboRouter(macname).startWorkTableSensorAddress, "1");
                    //ConsoleShow("startWorkTableSensorAddressをONしました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // MagLoaderReq OFF
                    SetBit(LEDDBComboRouter(macname).loaderReqBitAddress, "0");
                    ConsoleShow("実マガジン供給要求OFFしました");


                    // 途中投入？
                    if (cb_leddb_useniccv.Checked)
                    {
                        SetBit(NicConv_01.unloaderReqBitAddress, "0");
                        ConsoleShow("途中投入をOFFしました");
                    }
                    else
                    {
                        // OVN排出要求OFF
                        SetBit(DBOvnComboRouter(ovnmacno).unloaderReqBitAddressList_addKey1, "0");
                        ConsoleShow("オーブンの実マガジン排出をOFFしました");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:10秒 +++");
                    Thread.Sleep(10000);

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // 工程完了登録：EICSマガジンタイミング
            workheader = "procno." + procno + ":EICSマガジンタイミング開始";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                ConsoleShow("■■■B,E,H,I,K,L,N,Wファイルを出力開始■■■");

                await Task.Run(() =>
                {
                    if (!CreateMLog(macInstance.macLogOriginFolder + "B.csv", MLfolderpath + "B" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "E.csv", MLfolderpath + "E" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "H.csv", MLfolderpath + "H" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "I.csv", MLfolderpath + "I" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "K.csv", MLfolderpath + "K" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "N.csv", MLfolderpath + "N" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "W.csv", MLfolderpath + "W" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "L.csv", MLfolderpath + "L" + MLSriname, logfiledict, ref msg))
                    {
                        ConsoleShow(msg);
                        return;
                    }

                    // +++ Wait +++
                    Thread.Sleep(2000);

                    ConsoleShow("■■■B,E,H,I,K,L,N,Wファイルを出力完了■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // 工程完了登録
            workheader = "procno." + procno + ":工程完了登録開始";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■実マガジン排出開始■■■");

                    // 空マガNoを再度書き込み（ロボットがなぜか持ちだすことがあるっぽい）
                    if (!readRbMagQr(karamagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);
                    ConsoleShow("マガジンNoを再度書き込みました。");

                    //実マガジン排出要求ON
                    SetBit(LEDDBComboRouter(macname).unloaderReqBitAddress, "1");
                    ConsoleShow("実マガジン排出要求ON");

                    //// +++ Wait +++
                    //ConsoleShow("+++ Wait:10秒 +++");
                    //Thread.Sleep(10000);

                    //// waferBitAddressStart
                    //SetBit(LEDDBComboRouter(macname).waferBitAddressStart, "0");
                    //ConsoleShow("waferBitAddressStartをOFFしました");

                    //// startWorkTableSensorAddress
                    //SetBit(LEDDBComboRouter(macname).startWorkTableSensorAddress, "0");
                    //ConsoleShow("startWorkTableSensorAddressをOFFしました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:20秒 +++");
                    Thread.Sleep(20000);

                    ConsoleShow("■■■DB装置アンローダー上で停止■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        //
        // DBPL
        //
        private void btn_DBPL_AutoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                DBPL_AutoRun();
                
            }
        }
        private async void DBPL_AutoRun()
        {
            var procno = "5";
            var procname = "DB-PL";
            string lotno = tx_dbpl_lotno.Text;
            var magno = tx_dbpl_m_auto.Text.Replace("C30 ", "");
            // 実マガジン～
            workheader = "procno." + procno + ":実マガジン排出";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                var DBNo = cb_leddbno.Text;

                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    ConsoleShow("■■■DB装置から実マガジン払出■■■");

                    // Lotno自動取得
                    if (cb_dbpl_getLotno.Checked)
                    {
                        if (!getLastLotNofromTnTran(ref lotno))
                        {
                            ConsoleShow("ロットNoの自動取得が失敗しました");
                            return;
                        }
                        else
                        {

                        }
                    }

                    ConsoleShow("排出要求ON");
                    if (string.IsNullOrEmpty(LEDDBComboRouter(DBNo).unloaderReqBitAddress))
                    {
                        ConsoleShow("Ledダイボンダーではありません");
                        return;
                    }
                    SetBit(LEDDBComboRouter(DBNo).unloaderReqBitAddress, "1");

                    ConsoleShow("供給要求ON");
                    SetBit(DCConv_01.loaderReqBitAddress, "1");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // ブリッジ搬送
                    if (cb_dbpl_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }
                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    // TnTranに開始完了登録
                    if (cb_dbpl_appendTntran.Checked)
                    {
                        if (!appendDBPLTran(lotno, magno))
                        {
                            ConsoleShow("TnTranテーブルへの書き込み時に問題発生しました");
                            return;
                        }
                        if (!updateTnMag(5, lotno, magno))
                        {
                            ConsoleShow("TnMagテーブルへの書き込み時に問題発生しました");
                            return;
                        }
                    }

                    ConsoleShow("■■■ライン外払出で停止■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }


        //
        // WBPL
        //
        private void btn_WBPL_AutoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                WBPL_AutoRun();
                
            }
        }
        private async void WBPL_AutoRun()
        {
            var procno = "8";
            var procname = "WB-PL";
            string msg = "";
            var karamagno = tx_wbpl_empm_auto.Text;
            var jitumagno = tx_wbpl_m_auto.Text;
            var ovnmacno = cb_wbpl_fromDBOVNno.Text; ;

            // 空マガジン～
            workheader = "procno." + procno + ":空マガジン投入";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    // 空マガ書き込み
                    ConsoleShow("■■■空マガジン投入■■■");
                    if (!readRbMagQr(karamagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    // EmpMagLoaderReq
                    SetBit(WBPL_01.empMagLoader_lane_key1, "1");
                    ConsoleShow("空マガジン供給を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:2秒 +++");
                    Thread.Sleep(2000);

                    // 空マガジン投入
                    EntConvMagazine("kara_on");
                    ConsoleShow("ローダー空マガジン排出を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // ブリッジ搬送
                    if (cb_wbpl_emmoveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");
                        // 空排出ON
                        SetBit(LineBridge_01.empMagUnloaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }

                        // 空排出OFF
                        SetBit(LineBridge_01.empMagUnloaderReqBitAddress, "0");
                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    // EmpMagLoaderReq
                    SetBit(WBPL_01.empMagLoader_lane_key1, "0");
                    ConsoleShow("空マガジン供給OFFしました");

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            // 実マガジン～
            workheader = "procno." + procno + ":実マガジン投入";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    // 実マガ書き込み
                    ConsoleShow("■■■実マガジン投入■■■");
                    if (!readRbMagQr(jitumagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    // MagLoaderReq ON
                    SetBit(WBPL_01.loader_lane_key1, "1");
                    ConsoleShow("実マガジン供給要求ONしました");
                    Thread.Sleep(1000);

                    // 途中投入？
                    if (cb_wbpl_useniccv.Checked)
                    {
                        if (!NicConvQR(jitumagno))
                        {
                            return;
                        }
                        Thread.Sleep(1000);
                        // 途中投入排出要求ON
                        SetBit(NicConv_01.unloaderReqBitAddress, "1");
                        SetBit(NicConv_01.magazineArriveBitAddress, "1");
                        ConsoleShow("途中マガジン投入を要求しました");
                    }
                    else
                    {
                        // OVN排出要求ON
                        SetDataTimeToOvenPlc(DBOvnComboRouter(ovnmacno));
                        ConsoleShow("開始完了時刻を書き込んでいます:Wait1秒");
                        Thread.Sleep(1000);
                        SetBit(DBOvnComboRouter(ovnmacno).unloaderReqBitAddressList_addKey1, "1");
                        ConsoleShow("オーブンの実マガジン排出を要求しました");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // ブリッジ搬送
                    if (cb_wbpl_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }
                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    // MagLoaderReq OFF
                    SetBit(WBPL_01.loader_lane_key1, "0");
                    ConsoleShow("実マガジン供給要求OFFしました");

                    // 途中投入？
                    if (cb_wbpl_useniccv.Checked)
                    {
                        SetBit(NicConv_01.unloaderReqBitAddress, "0");
                        ConsoleShow("途中投入をOFFしました");
                    }
                    else
                    {
                        // OVN排出要求OFF
                        SetBit(DBOvnComboRouter(ovnmacno).unloaderReqBitAddressList_addKey1, "0");
                        ConsoleShow("オーブンの実マガジン排出をOFFしました");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:10秒 +++");
                    Thread.Sleep(10000);

                    ConsoleShow("■■■WBプラズマ装置上で停止(完了処理前)■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        //
        // WB
        //
        private void btn_WB_AutoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                WB_AutoRun();
                
            }
        }
        private async void WB_AutoRun()
        {
            var procno = "9";
            var procname = "WB";
            string msg = "";
            var karamagno = tx_wb_empm_auto.Text;
            var jitumagno = tx_wb_m_auto.Text;
            var macname = cb_wbno.Text;
            //
            // log file words
            //
            var MLfolderpath = "";
            var macInstance = WBComboRouter(macname);
            if (!macInstance.getMacFolderPath("in", ref MLfolderpath))
            {
                ConsoleShow(MLfolderpath);
                return;
            }
            var logfld = cmb_eicslog.Text + "\\";
            var date = DateTime.Now.ToString("yyyy/MM/dd");
            var logfiledict = new Dictionary<string, string>
            {
                { "@magno", karamagno },
                { "@macno", macInstance.equipment_no },
                { "@typecd", EicsParam_TypeCode },
                { "@date", $"{date}" }
            };
            DateTime dt = DateTime.Now;
            var MLSriname = "0000" + dt.ToString("yyMMddHHmmss") + ".csv";
            //

            // EICSスタートタイミング開始確認
            workheader = "procno." + procno + ":EICSマガジンタイミング開始";
            if (SelectYesNoMessage(workheader + "。EICSは稼働中ですか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□macno={macname}: {macInstance.macname}□□□");
                    ConsoleShow("■■■SPファイルを出力開始■■■");
                    // MLフォルダクリーン
                    foreach (string f in Directory.GetFiles(MLfolderpath, "*.*"))
                    {
                        File.Delete(f);
                    }
                    // Startフォルダクリーン
                    foreach (string f in Directory.GetFiles(MLfolderpath.Replace("Magazine", "Start"), "*.*"))
                    {
                        File.Delete(f);
                    }

                    // 設備ログ(SPファイル)を吐き出し
                    // SPファイル
                    if (!CreateMLog(macInstance.macLogOriginFolder + logfld + "SP.csv", MLfolderpath.Replace("Magazine", "Start") + "SP" + MLSriname, logfiledict, ref msg, "shift-jis"))
                    {
                        ConsoleShow("SPファイルが正常に出力できませんでした");
                        return;
                    }
                    ConsoleShow("■■■SPファイルを出力完了■■■");

                    Thread.Sleep(2000);
                    RecordTn(workheader);
                });
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            // 空マガジン～
            workheader = "procno." + procno + ":空マガジン投入";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    // 空マガ書き込み
                    ConsoleShow("■■■空マガジン投入■■■");
                    if (!readRbMagQr(karamagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    // EmpMagLoaderReq
                    SetBit(WBComboRouter(macname).empMagLoaderReqBitAddress, "1");
                    ConsoleShow("空マガジン供給を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:2秒 +++");
                    Thread.Sleep(2000);

                    // 空マガジン投入
                    EntConvMagazine("kara_on");
                    ConsoleShow("ローダー空マガジン排出を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // ブリッジ搬送
                    if (cb_wb_emmoveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");
                        SetBit(LineBridge_01.empMagUnloaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }

                        // 空排出OFF
                        SetBit(LineBridge_01.empMagUnloaderReqBitAddress, "0");
                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    // EmpMagLoaderReq
                    SetBit(WBComboRouter(macname).empMagLoaderReqBitAddress, "0");
                    ConsoleShow("空マガジン供給OFFしました");

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            // 実マガジン～
            workheader = "procno." + procno + ":実マガジン投入";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    // 実マガ書き込み
                    ConsoleShow("■■■OVN完了／実マガジン投入■■■");
                    if (!readRbMagQr(jitumagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    // MagLoaderReq ON
                    SetBit(WBComboRouter(macname).loaderReqBitAddress, "1");
                    ConsoleShow("実マガジン供給要求ONしました");
                    Thread.Sleep(1000);

                    // 途中投入？
                    if (cb_wb_useniccv.Checked)
                    {
                        if (!NicConvQR(jitumagno))
                        {
                            return;
                        }
                        Thread.Sleep(1000);
                        // 途中投入排出要求ON
                        SetBit(NicConv_01.unloaderReqBitAddress, "1");
                        SetBit(NicConv_01.magazineArriveBitAddress, "1");
                        ConsoleShow("途中マガジン投入を要求しました");
                    }
                    else
                    {
                        // WBPL排出要求ON
                        SetBit(WBPL_01.unloader_lane_key1, "1");
                        ConsoleShow("WBPLの実マガジン排出を要求しました");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // ブリッジ搬送
                    if (cb_wb_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }
                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    // MagLoaderReq ON
                    SetBit(WBComboRouter(macname).loaderReqBitAddress, "0");
                    ConsoleShow("実マガジン供給要求OFFしました");

                    // 途中投入／WBPL排出要求OFF
                    if (cb_wb_useniccv.Checked)
                    {
                        SetBit(NicConv_01.unloaderReqBitAddress, "0");
                        ConsoleShow("途中投入をOFFしました");
                    }
                    else
                    {
                        SetBit(WBPL_01.unloader_lane_key1, "0");
                        ConsoleShow("WBPLの実マガジン排出をOFFしました");
                    }
                        
                    // +++ Wait +++
                    ConsoleShow("+++ Wait10秒 +++");
                    Thread.Sleep(10000);

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // 工程完了登録：EICSマガジンタイミング
            workheader = "procno." + procno + ":EICSマガジンタイミング開始";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                ConsoleShow("■■■ME,ML,MM,MPファイルを出力開始■■■");

                dt = DateTime.Now;
                MLSriname = "0000" + dt.ToString("yyMMddHHmmss") + ".CSV"; //大文字のCSVでないとEICSは読んでくれない

                await Task.Run(() =>
                {
                    if (!CreateMLog(macInstance.macLogOriginFolder + logfld + "ME.csv", MLfolderpath + "ME" + MLSriname, logfiledict, ref msg, "shift-jis"))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + logfld + "ML.csv", MLfolderpath + "ML" + MLSriname, logfiledict, ref msg, "shift-jis"))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + logfld + "MM.csv", MLfolderpath + "MM" + MLSriname, logfiledict, ref msg, "shift-jis"))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + logfld + "MP.csv", MLfolderpath + "MP" + MLSriname, logfiledict, ref msg, "shift-jis"))
                    {
                        ConsoleShow(msg);
                        return;
                    }

                    // +++ Wait +++
                    Thread.Sleep(2000);

                    ConsoleShow("■■■ME,ML,MM,MPファイルを出力完了■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // 工程完了登録
            workheader = "procno." + procno + ":工程完了登録開始";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■実マガジン排出開始■■■");

                    // MMファイルクリーン、作成
                    //ConsoleShow("MMファイル作成しています");
                    //foreach (string f in Directory.GetFiles(MMfolderpath, "MM*.*"))
                    //{
                    //    File.Delete(f);
                    //}
                    //DateTime dt = DateTime.Now;
                    //var MMfilename = "MM0000" + dt.ToString("yyMMddHHmmss") + ".csv";
                    //CopyFile(MMfileorigin, MMfolderpath + "\\" + MMfilename);

                    // 空マガNoを再度書き込み（ロボットがなぜか持ちだすことがあるっぽい）
                    if (!readRbMagQr(karamagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);
                    ConsoleShow("マガジンNoを再度書き込みました。");

                    //実マガジン排出要求ON
                    SetBit(WBComboRouter(macname).unloaderReqBitAddress, "1");
                    ConsoleShow("実マガジン排出要求ON");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:30秒 +++");
                    Thread.Sleep(30000);

                    //実マガジン排出要求OFF
                    SetBit(WBComboRouter(macname).unloaderReqBitAddress, "0");
                    ConsoleShow("実マガジン排出要求OFF");

                    ConsoleShow("■■■WB装置アンローダー上で停止■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        //
        // AVI
        //
        private void btn_avi_AUTORun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                AVI_AutoRun();
                
            }
        }

        private async void AVI_AutoRun()
        {
            var procno = "10";
            var procname = "AVI";
            string wbmacno = cb_avi_fromwb.Text;
            string jitumagno = txt_avi_jitumag.Text;
            string maccode = txt_avi_maccode.Text;
            string msg = "";
            //
            // log file words
            //
            var MLfolderpath = "";
            var macInstance = AVI_01;
            if (!macInstance.getMacFolderPath("in", ref MLfolderpath))
            {
                ConsoleShow(MLfolderpath);
                return;
            }
            var date = DateTime.Now.ToString("yyyy/MM/dd");
            var logfiledict = new Dictionary<string, string>
            {
                { "@magno", jitumagno},
                { "@date", $"{date}" }
            };
            var maclogprefix = new string[] { "OA", "MM", "SM1", "SM2", "SM3"};
            var maclogprefixAdDict = new Dictionary<string, string> { { "OA", "DM20051" }, { "MM", "DM20052" } , { "SM1", "DM20050" }, {"SM2", "DM20053" }, {"SM3", "DM20054" } };

            workheader = "procno." + procno + ":実マガジン投入";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    // 実マガ書き込み
                    ConsoleShow("■■■WB完了／実マガジン投入■■■");
                    if (!readRbMagQr(jitumagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    // MagLoaderReq ON
                    SetBit(AVI_01.loaderReqBitAddress, "1");
                    ConsoleShow("実マガジン供給要求ONしました");
                    Thread.Sleep(1000);

                    // 途中投入？
                    if (cb_avi_useniccv.Checked)
                    {
                        if (!NicConvQR(jitumagno))
                        {
                            return;
                        }
                        Thread.Sleep(1000);
                        // 途中投入排出要求ON
                        SetBit(NicConv_01.unloaderReqBitAddress, "1");
                        SetBit(NicConv_01.magazineArriveBitAddress, "1");
                        ConsoleShow("途中マガジン投入を要求しました");
                    }
                    else
                    {
                        // WB排出要求ON
                        SetBit(WBComboRouter(wbmacno).unloaderReqBitAddress, "1");
                        ConsoleShow("WBの実マガジン排出を要求しました");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // ブリッジ搬送
                    if (cb_avi_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }
                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    // MagLoaderReq ON
                    SetBit(AVI_01.loaderReqBitAddress, "0");
                    ConsoleShow("実マガジン供給要求OFFしました");

                    // 途中投入／WBPL排出要求ON
                    if (cb_wb_useniccv.Checked)
                    {
                        SetBit(NicConv_01.unloaderReqBitAddress, "0");
                        ConsoleShow("途中投入をOFFしました");
                    }
                    else
                    {
                        SetBit(WBComboRouter(wbmacno).loaderReqBitAddress, "0");
                        ConsoleShow("WBの実マガジン排出をOFFしました");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait5秒 +++");
                    Thread.Sleep(5000);

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // 設備マップ転送・機種名転送～
            workheader = "procno." + procno + ":マップ／機種名転送開始";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■MAP転送要求・機種名転送要求開始■■■");
                    ConsoleShow("MAP転送要求しています");
                    Keylc.SetString(macInstance.LoaderQrdataAddress, jitumagno);
                    Keylc.SetBit(macInstance.OperationalStatusAddress, 1, "1");
                    Keylc.SetBit(macInstance.RequestTypeCdAddress, 1, "1");
                    ConsoleShow("+++ Wait:2秒 +++");
                    Thread.Sleep(2000);

                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }

            // 工程完了・排出：MLogファイル書き込み～
            workheader = "procno." + procno + ":開始完了時間・設備ログファイル処理";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■開始完了登録・ログ処理開始■■■");
                    //開始完了時間登録
                    //開始時間登録
                    var dt = DateTime.Now;
                    ConsoleShow("開始時間登録しています");
                    SetDateTime2Word(AVI_01.workStartTimeAddress, dt);
                    ConsoleShow("+++ Wait1秒 +++");
                    Thread.Sleep(1000);
                    ConsoleShow("完了時間登録しています");
                    dt = DateTime.Now;
                    SetDateTime2Word(AVI_01.workCompleteTimeAddress, dt);

                    // ログファイルクリーン、作成
                    ConsoleShow("ログファイル作成しています");
                    foreach (string f in Directory.GetFiles(MLfolderpath, "*.*"))
                    {
                        File.Delete(f);
                    }

                    foreach (var item in maclogprefix)
                    {
                        var MLfilename = "Log000_" + item + dt.ToString("yyMMddHHmm") + ".csv";
                        if (!CreateMLog(macInstance.macLogOriginFolder + item + ".csv", MLfolderpath + "\\" + MLfilename, logfiledict, ref msg, "shift-jis"))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                        ConsoleShow(item + "ファイルを出力しました");
                        ConsoleShow("+++ Wait:2秒 +++");
                        Thread.Sleep(2000);
                        Keylc.SetBit(maclogprefixAdDict[item], 1, "1");
                        ConsoleShow(item + "ファイル取得要求を立てました");
                        while (Keylc.GetBit(maclogprefixAdDict[item]) == "1") 
                        {
                            var cnt = 0;
                            Thread.Sleep(1000);
                            cnt++;
                            if (cnt > 10) break;
                        }
                    }
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // 工程完了・排出
            workheader = "procno." + procno + ":工程完了・排出";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■実マガジン排出開始■■■");

                    // avi装置へのMagno問い合わせ
                    ReadAviQrCode(jitumagno);
                    ConsoleShow("avi上マガジンNoを書き込みました");

                    // nasファイルクリーン、作成
                    //foreach (string f in Directory.GetFiles(nasfolderpath, "*.nas"))
                    //{
                    //    File.Delete(f);
                    //}
                    //var nasfilename = $"L0000000000_{jitumagno.Replace("C30 ", "")}_{maccode}.nas";
                    //var nasfilepath = txt_avi_nasfilefoder.Text + "\\" + nasfilename;
                    //CreateFile(nasfilepath, nasfilecontents, ref msg);

                    //実マガジン排出要求ON
                    SetBit(AVI_01.unloaderReqBitAddress, "1");
                    ConsoleShow("実マガジン排出要求ON");

                    //途中排出CVをON
                    SetBit(DCConv_01.loaderReqBitAddress, "1");
                    ConsoleShow("途中排出コンベア供給要求ON");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    //実マガジン排出要求OFF
                    SetBit(AVI_01.unloaderReqBitAddress, "0");
                    ConsoleShow("実マガジン排出要求OFF");

                    //途中排出CVをOFF
                    SetBit(DCConv_01.loaderReqBitAddress, "0");
                    ConsoleShow("途中排出コンベア供給要求OFF");

                    ConsoleShow("■■■ライン外排出で停止■■■");

                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        //
        // WBOVN
        //
        private void btn_WBOVN_AutoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                WBOVN_AutoRun();
                
            }
        }
        private async void WBOVN_AutoRun()
        {
            var procno = "22";
            var procname = "WB-OVN";
            string lotno = txt_wbovn_lotno.Text;
            string magno = txt_wbovn_m_auto.Text.Replace("C30 ", "");
            // 実マガジン～
            workheader = "procno." + procno + ":開始完了登録";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    ConsoleShow("■■■TnTran/TnMag操作■■■");

                    // Lotno自動取得
                    if (cb_dbpl_getLotno.Checked)
                    {
                        if (!getLastLotNofromTnTran(ref lotno))
                        {
                            ConsoleShow("ロットNoの自動取得が失敗しました");
                            return;
                        }
                        else
                        {

                        }
                    }

                    // TnTranに開始完了登録
                    if (cb_dbpl_appendTntran.Checked)
                    {
                        if (!appendWBOVNTran(lotno, magno))
                        {
                            ConsoleShow("TnTranテーブルへの書き込み時に問題発生しました");
                            return;
                        }
                        if (!updateTnMag(22, lotno, magno))
                        {
                            ConsoleShow("TnMagテーブルへの書き込み時に問題発生しました");
                            return;
                        }
                    }

                    ConsoleShow("■■■完了■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        //
        // SUP
        //
        private void btn_SUP_AutoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                SUP_AutoRun();
                
            }
        }
        private async void SUP_AutoRun()
        {
            var procno = "23";
            var procname = "SUP";
            string msg = "";
            var magno = txt_sup_m_auto.Text;
            var ldmacno = txt_sup_ldno.Text;
            var ulmacno = txt_sup_ulno.Text;
            var macInstance = SUP_01;
            //
            // log file words
            //
            var MLfolderpath = "";
            if (!macInstance.getMacFolderPath("in", ref MLfolderpath))
            {
                ConsoleShow(MLfolderpath);
                return;
            }
            var date = DateTime.Now.ToString("yyyy/MM/dd");
            var logfiledict = new Dictionary<string, string>
            {
                { "@magno", magno},
                { "@kishumei", $"{EicsParam_TypeCode}" },
                { "@date", $"{date}" }
            };

            // 実マガジン～
            workheader = "procno." + procno + ":実マガジン投入";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    ConsoleShow("■■■実マガジン投入開始■■■");
                    //設備NoをPLCに書き込み
                    ConsoleShow("PLCに設備番号を登録しました");
                    sup_Ldcmpl_on(ldmacno);
                    Thread.Sleep(1000);
                    sup_Ulcmpl_on(ulmacno);
                    Thread.Sleep(1000);

                    //開始完了時間メモリ(PLC)に書き込み
                    ConsoleShow("PLCに開始完了時間を登録しました");
                    var dt = DateTime.Now;
                    SetDateTime2Word(SUP_01.workStartTimeAddress, dt);
                    Thread.Sleep(1000);
                    SetDateTime2Word(SUP_01.workCompleteTimeAddress, dt);
                    Thread.Sleep(1000);

                    // ローダーにマガジンをセット
                    sup_Inmag_on(magno);
                    ConsoleShow("ローダーに実マガジンをセットしました");
                    Thread.Sleep(1000);

                    // ローダー作業完了
                    sup_lmscba_on();
                    ConsoleShow("作業完了信号を立てました");
                    Thread.Sleep(1000);

                    // +++ Wait +++
                    ConsoleShow("+++ Wait15秒 +++");
                    Thread.Sleep(15000);

                    // ローダー関連信号を初期化
                    sup_Inmag_off();
                    sup_lmscba_off();

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // マガジン別データ・自動出力データを出力
            workheader = "procno." + procno + ":マガジン別データ・自動出力データ出力";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                ConsoleShow("■■■マガジン別データ・自動出力データを出力開始■■■");

                var dt = DateTime.Now;
                var MLSriname = dt.ToString("ddHHmmss");
                

                await Task.Run(() =>
                {
                    // 自動出力データ(03)
                    var Targetfld = MLfolderpath + $"03_auto\\{MLSriname}";
                    if (!Directory.Exists(Targetfld))
                    {
                        Directory.CreateDirectory(Targetfld);
                    }

                    if (!CreateMLog(macInstance.macLogOriginFolder + "_03_auto_\\_03_auto_A.CSV", Targetfld + "\\_03_auto_" + MLSriname + "A.CSV", logfiledict, ref msg, "shift-jis"))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "_03_auto_\\_03_auto_B.CSV", Targetfld + "\\_03_auto_" + MLSriname + "B.CSV", logfiledict, ref msg, "shift-jis"))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "_03_auto_\\_03_auto_C.CSV", Targetfld + "\\_03_auto_" + MLSriname + "C.CSV", logfiledict, ref msg, "shift-jis"))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    // マガジン別データ(13)
                    Targetfld = MLfolderpath + $"13_Vdc_kanri\\{MLSriname}";
                    if (!Directory.Exists(Targetfld))
                    {
                        Directory.CreateDirectory(Targetfld);
                    }

                    if (!CreateMLog(macInstance.macLogOriginFolder + "_13_Vdc_kanri_\\_13_Vdc_kanri_D.CSV", Targetfld + "\\_13_Vdc_kanri_" + MLSriname + "D.CSV", logfiledict, ref msg, "shift-jis"))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "_13_Vdc_kanri_\\_13_Vdc_kanri_E.CSV", Targetfld + "\\_13_Vdc_kanri_" + MLSriname + "E.CSV", logfiledict, ref msg, "shift-jis"))
                    {
                        ConsoleShow(msg);
                        return;
                    }
                    if (!CreateMLog(macInstance.macLogOriginFolder + "_13_Vdc_kanri_\\_13_Vdc_kanri_F.CSV", Targetfld + "\\_13_Vdc_kanri_" + MLSriname + "F.CSV", logfiledict, ref msg, "shift-jis"))
                    {
                        ConsoleShow(msg);
                        return;
                    }

                    // +++ Wait +++
                    Thread.Sleep(2000);

                    ConsoleShow("■■■マガジン別データ・自動出力データを出力完了■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }


            // 工程完了登録：MMファイル書き込み～
            workheader = "procno." + procno + ":工程完了登録";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■実マガジン排出開始■■■");

                    //開始完了時間メモリ(PLC)に書き込み
                    ConsoleShow("PLCに開始完了時間を登録しました");
                    var dt = DateTime.Now;
                    SetDateTime2Word(SUP_01.workStartTimeAddress, dt);
                    Thread.Sleep(1000);
                    SetDateTime2Word(SUP_01.workCompleteTimeAddress, dt);
                    Thread.Sleep(1000);

                    // アンローダーにマガジンをセット
                    sup_Outmag_on(magno);
                    ConsoleShow("アンローダーに実マガジンをセットしました");
                    Thread.Sleep(1000);

                    // +++ Wait +++
                    ConsoleShow("+++ Wait15秒 +++");
                    Thread.Sleep(15000);

                    // ローダー関連信号を初期化
                    sup_Outmag_off();
                    ConsoleShow("■■■完了■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        //
        // ﾓｰﾙﾄﾞ
        //
        private void btn_md_autoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                MD_AutoRun();
                
            }
        }
        private async void MD_AutoRun()
        {
            var procno = "12";
            var procname = "MD";
            string msg = "";
            var inmagno = txt_md_inmagno.Text;
            var macno = cb_mb_macno.Text;
            //
            // log file words
            //
            var MLfolderpath = "";
            var macInstance = MDComboRouter(macno);
            if (!macInstance.getMacFolderPath("in", ref MLfolderpath))
            {
                ConsoleShow(MLfolderpath);
                return;
            }
            var date = DateTime.Now.ToString("yyyy/MM/dd");
            var time = DateTime.Now.ToString("HH:mm:ss");
            var logfiledict = new Dictionary<string, string>
            {
                { "@magno", inmagno},
                { "@kishumei", $"CL-A160" },
                { "@date", $"{date}" },
                { "@time", $"{time}" }
            };
            var maclogprefix_st = new string[] { "PR", "OR", "EF", "SF" };
            var maclogprefixAdDict_st = new Dictionary<string, string> { { "PR", "DM30052" }, { "OR", "DM30053" }, { "EF", "DM30055" }, { "SF", "DM30050" } };
            var maclogprefix_mg = new string[] { "SM", "AM", "EM", "SD" };
            var maclogprefixAdDict_mg = new Dictionary<string, string> { { "SM", "DM30051" }, { "AM", "DM30054" }, { "EM", "DM30056" }, { "SD", "DM30057" } };
            //
            // plcStrings
            //
            Keylc.SetBit(macInstance.MoldDirectionInfoAddress, 1, "1");
            Keylc.SetBit(macInstance.SyringeNumberInfoAddress, 2, "3");
            Keylc.SetBit(macInstance.MemoryTypeInfoAddress, 1, "1");
            Keylc.SetBit(macInstance.NO_DIVISION_MAPPING_MODE_ENABLE, 1, "1");
            Keylc.SetString(macInstance.LoaderQrdataAddress, inmagno);

            // 実マガジン～
            workheader = "procno." + procno + ":実マガジン投入";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    ConsoleShow("■■■実マガジン投入開始■■■");
                    if (!readRbMagQr(inmagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    // MagLoaderReq ON
                    SetBit(MDComboRouter(macno).loaderReqBitAddress, "1");
                    ConsoleShow("実マガジン供給要求ONしました");
                    Thread.Sleep(1000);

                    // 途中投入
                    if (!NicConvQR(inmagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);
                    // 途中投入排出要求ON
                    SetBit(NicConv_01.unloaderReqBitAddress, "1");
                    SetBit(NicConv_01.magazineArriveBitAddress, "1" +
                        "");
                    ConsoleShow("途中マガジン投入を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // ブリッジ搬送
                    if (cb_md_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }
                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    //開始完了時間メモリ(PLC)に書き込み
                    ConsoleShow("PLCに開始時間を登録しました");
                    var dt = DateTime.Now;
                    SetDateTime2Word(MDComboRouter(macno).workStartTimeAddress, dt);
                    Thread.Sleep(1000);

                    //MD設備にINマガジンを登録
                    md_inmagno(inmagno);
                    ConsoleShow("設備に実マガジンを登録しました");
                    Thread.Sleep(1000);

                    // MagLoaderReq ON
                    SetBit(MDComboRouter(macno).loaderReqBitAddress, "0");
                    ConsoleShow("実マガジン供給要求OFFしました");

                    // 途中投入排出要求OFF
                    SetBit(NicConv_01.unloaderReqBitAddress, "0");
                    ConsoleShow("途中投入をOFFしました");                 

                    // +++ Wait +++
                    ConsoleShow("+++ Wait5秒 +++");
                    Thread.Sleep(5000);

                    ConsoleShow("■■■MD投入済で停止■■■");
                });

                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // EICSスタートタイミング
            workheader = "procno." + procno + ":EICSスタートTiming";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■PR,OR,EF,SFログ処理開始■■■");

                    var dt = DateTime.Now;

                    // 設備稼働スタート
                    ConsoleShow("機種設定");
                    Keylc.SetString("W0FF0", "CL-A160             ");
                    ConsoleShow("設備稼働ON");
                    Keylc.SetBit("B0FF1", 1, "0");
                    Keylc.SetBit("B0FF2", 1, "0");
                    Keylc.SetBit("B0FF0", 1, "1");

                    // ログファイルクリーン、作成
                    ConsoleShow("ログファイル作成しています");
                    foreach (string f in Directory.GetFiles(MLfolderpath, "*.*"))
                    {
                        File.Delete(f);
                    }

                    foreach (var item in maclogprefix_st)
                    {

                        var MLfilename = "Log000_" + item + dt.ToString("yyMMddHHmm") + ".csv";
                        if (!CreateMLog(macInstance.macLogOriginFolder + item + ".csv", MLfolderpath + "\\" + MLfilename, logfiledict, ref msg, "shift-jis"))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                        ConsoleShow(item + "ファイルを出力しました");
                        ConsoleShow("+++ Wait:2秒 +++");
                        Thread.Sleep(2000);
                        Keylc.SetBit(maclogprefixAdDict_st[item], 1, "1");
                        ConsoleShow(item + "ファイル取得要求を立てました");
                    }

                    ConsoleShow("+++ Wait5秒 +++");
                    Thread.Sleep(5000);

                    //稼働許可確認
                    if (Keylc.GetBit("B0FF1")!="1")
                    {
                        ConsoleShow("設備稼働許可が出ませんでした");
                        return;
                    }
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // LENSマッピングデータ転送
            workheader = "procno." + procno + ":LENSマッピングデータ転送";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■LENSマッピングデータ転送処理開始■■■");

                    var dt = DateTime.Now;

                    // マップ要求発行
                    ConsoleShow("マッピングデータ要求ON");
                    Keylc.SetBit(macInstance.MappingRequestAddress, 1, "1");
                    ConsoleShow("+++ Wait5秒 +++");
                    Thread.Sleep(5000);

                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // EICSマガジンタイミング
            workheader = "procno." + procno + ":EICSマガジンTiming";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■SM, AM, EM, SDログ処理開始■■■");

                    var dt = DateTime.Now;

                    // ログファイルクリーン、作成
                    ConsoleShow("ログファイル作成しています");
                    foreach (string f in Directory.GetFiles(MLfolderpath, "*.*"))
                    {
                        File.Delete(f);
                    }

                    foreach (var item in maclogprefix_mg)
                    {

                        var MLfilename = "Log000_" + item + dt.ToString("yyMMddHHmm") + ".csv";
                        if (!CreateMLog(macInstance.macLogOriginFolder + item + ".csv", MLfolderpath + "\\" + MLfilename, logfiledict, ref msg, "shift-jis"))
                        {
                            ConsoleShow(msg);
                            return;
                        }
                        ConsoleShow(item + "ファイルを出力しました");
                        ConsoleShow("+++ Wait:2秒 +++");
                        Thread.Sleep(2000);
                        Keylc.SetBit(maclogprefixAdDict_mg[item], 1, "1");
                        ConsoleShow(item + "ファイル取得要求を立てました");
                    }

                    ConsoleShow("+++ Wait5秒 +++");
                    Thread.Sleep(5000);
                    ConsoleShow("■■■MD装置上で停止(完了処理前)■■■");

                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        //
        // 遠沈
        //
        private void btn_eck_autoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                ECK_AutoRun();
                
            }
        }
        private async void ECK_AutoRun()
        {
            var procno = "13";
            var procname = "ECK";
            var mdinmagno = txt_eck_mdinmagno.Text;
            var magno = new string[]{ txt_eck_magno1.Text, txt_eck_magno2.Text };
            var mdmacno = cb_eck_frommdno.Text;
            var macInstance = ECK_01;
            var msg = "";
            //
            // log file words
            //
            var MLfolderpath = "";
            var maclogprefix_st = new string[] { "PR", "OR" };
            var maclogprefixAdDict_st = new Dictionary<string, string> { { "PR", "DM32050" }, { "OR", "DM32051" } };
            var date = DateTime.Now.ToString("yyyy/MM/dd");
            var time = DateTime.Now.ToString("HH:mm:ss");
            var logfiledict = new Dictionary<string, string>
            {
                { "@magno", "" },
                { "@typecd", $"{EicsParam_TypeCode}" },
                { "@date", $"{date}" },
                { "@time", $"{time}" }
            };
            if (!macInstance.getMacFolderPath("in", ref MLfolderpath))
            {
                ConsoleShow(MLfolderpath);
                return;
            }

            // 実マガジン～
            workheader = "procno." + procno + ":遠沈マガジン#1を投入します";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    ConsoleShow("■■■遠沈マガジン#1排出開始■■■");
                    if (!readRbMagQr(magno[0]))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    //遠沈実マガジン供給要求をON
                    SetBit(ECK_01.loaderReqBitAddress1, "1");
                    ConsoleShow("遠沈実マガジン供給要求ON");

                    // 途中投入？
                    if (cb_eck_useniccv.Checked)
                    {
                        if (!NicConvQR(magno[0]))
                        {
                            return;
                        }
                        Thread.Sleep(1000);
                        // 途中投入排出要求ON
                        SetBit(NicConv_01.unloaderReqBitAddress, "1");
                        SetBit(NicConv_01.magazineArriveBitAddress, "1");
                        ConsoleShow("途中マガジン投入を要求しました");
                    }
                    else
                    {

                        //MD設備開始時間メモリ(PLC)に書き込み
                        ConsoleShow("PLCに開始時間を登録しました");
                        var dt = DateTime.Now;
                        SetDateTime2Word(MDComboRouter(mdmacno).workStartTimeAddress, dt);
                        Thread.Sleep(1000);

                        //MD設備にINマガジンを登録
                        md_inmagno(mdinmagno);
                        ConsoleShow("設備に実マガジンを登録しました");
                        Thread.Sleep(1000);

                        //開始完了時間メモリ(PLC)に書き込み
                        ConsoleShow("PLCに完了時間を登録しました");
                        dt = DateTime.Now;
                        SetDateTime2Word(MDComboRouter(mdmacno).workCompleteTimeAddress, dt);
                        Thread.Sleep(1000);

                        //MD設備にOUTマガジン(1)を登録
                        md_outmagno(magno[0]);
                        ConsoleShow("設備に実マガジンを登録しました");
                        Thread.Sleep(1000);

                        //実マガジン排出要求ON
                        SetBit(MDComboRouter(mdmacno).unloaderReqBitAddress, "1");
                        ConsoleShow("実マガジン排出要求ON");
                    }

                    // ブリッジ搬送
                    if (cb_eck_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // +++ ロボット動作 +++
                    // +++ RB Polling +++
                    ConsoleShow("+++ ロボット移動要求確認 +++");
                    if (!RBMoveEnableToGetMag())
                    {
                        ConsoleShow("■■■自動投入停止■■■");
                        return;
                    }

                    // ロボット動作（Mag取る）
                    Rb_GetMag();
                    ConsoleShow("Mag取り動作");
                    SetBit(NicConv_01.unloaderReqBitAddress, "0");
                    SetBit(NicConv_01.magazineArriveBitAddress, "0");

                    // 排出OFF
                    if (cb_eck_useniccv.Checked)
                    {
                        SetBit(NicConv_01.unloaderReqBitAddress, "0");
                        ConsoleShow("途中投入をOFFしました");
                    }
                    else
                    {
                        //実マガジン排出要求OFF
                        SetBit(MDComboRouter(mdmacno).unloaderReqBitAddress, "0");
                        ConsoleShow("実マガジン排出要求OFF");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // ロボット動作（Mag置く）
                    Rb_PutMag();
                    ConsoleShow("Mag置く動作");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    //実マガジン供給要求OFF
                    SetBit(ECK_01.loaderReqBitAddress1, "0");
                    ConsoleShow("実マガジン供給要求OFF");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait5秒 +++");
                    Thread.Sleep(5000);

                    ConsoleShow("■■■遠沈マガジン#1で投入完了■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            workheader = "procno." + procno + ":遠沈マガジン#2を投入します";
            if (SelectYesNoMessage(workheader + "。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■遠沈マガジン#2排出開始■■■");
                    if (!readRbMagQr(magno[1]))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    //遠沈実マガジン供給要求をON
                    SetBit(ECK_01.loaderReqBitAddress4, "1");
                    ConsoleShow("遠沈実マガジン供給要求ON");

                    // 途中投入？
                    if (cb_eck_useniccv.Checked)
                    {
                        if (!NicConvQR(magno[1]))
                        {
                            return;
                        }
                        Thread.Sleep(1000);
                        // 途中投入排出要求ON
                        SetBit(NicConv_01.unloaderReqBitAddress, "1");
                        SetBit(NicConv_01.magazineArriveBitAddress, "1");
                        ConsoleShow("途中マガジン投入を要求しました");
                    }
                    else
                    {
                        //開始完了時間メモリ(PLC)に書き込み
                        ConsoleShow("PLCに完了時間を登録しました");
                        var dt = DateTime.Now;
                        SetDateTime2Word(MDComboRouter(mdmacno).workCompleteTimeAddress, dt);
                        Thread.Sleep(1000);

                        //MD設備にOUTマガジン(2)を登録
                        md_outmagno(magno[1]);
                        ConsoleShow("設備に実マガジンを登録しました");
                        Thread.Sleep(1000);

                        //実マガジン排出要求ON
                        SetBit(MDComboRouter(mdmacno).unloaderReqBitAddress, "1");
                        ConsoleShow("実マガジン排出要求ON");
                    }

                    // ブリッジ搬送
                    if (cb_eck_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // +++ ロボット動作 +++
                    // +++ RB Polling +++
                    ConsoleShow("+++ ロボット移動要求確認 +++");
                    if (!RBMoveEnableToGetMag())
                    {
                        ConsoleShow("■■■自動投入停止■■■");
                        return;
                    }

                    // ロボット動作（Mag取る）
                    Rb_GetMag();
                    ConsoleShow("Mag取り動作");
                    SetBit(NicConv_01.unloaderReqBitAddress, "0");
                    SetBit(NicConv_01.magazineArriveBitAddress, "0");

                    // 排出OFF
                    if (cb_avi_useniccv.Checked)
                    {
                        SetBit(NicConv_01.unloaderReqBitAddress, "0");
                        ConsoleShow("途中投入をOFFしました");
                    }
                    else
                    {
                        //実マガジン排出要求OFF
                        SetBit(MDComboRouter(mdmacno).unloaderReqBitAddress, "0");
                        ConsoleShow("実マガジン排出要求OFF");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // ロボット動作（Mag置く）
                    Rb_PutMag();
                    ConsoleShow("Mag置く動作");

                    //実マガジン供給要求OFF
                    SetBit(ECK_01.loaderReqBitAddress4, "0");
                    ConsoleShow("実マガジン供給要求OFF");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait5秒 +++");
                    Thread.Sleep(5000);

                    ConsoleShow("■■■遠沈マガジン#2で投入完了■■■");
                });
                updateCommonFunctionSignalBtns();
                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // EICSスタートタイミング開始確認
            workheader = "procno." + procno + ":EICSスタートタイミング開始";
            if (SelectYesNoMessage(workheader + "。EICSは稼働中ですか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□macno={macInstance.macno}: {macInstance.macname}□□□");
                    ConsoleShow("■■■OR,PRファイルを出力開始■■■");
                    // MLフォルダクリーン
                    foreach (string f in Directory.GetFiles(MLfolderpath, "*.*"))
                    {
                        File.Delete(f);
                    }

                    // 設備ログ(OR,PRファイル)を吐き出し
                    var dt = DateTime.Now;
                    var logno = 1;
                    foreach (var mag in magno)
                    {
                        foreach (var item in maclogprefix_st)
                        {
                            logfiledict["@magno"] = mag;
                            var MLfilename = $"Log00{logno}_" + item + dt.ToString("yyMMddHHmm") + ".csv";
                            if (!CreateMLog(macInstance.macLogOriginFolder + item + ".csv", MLfolderpath + "\\" + MLfilename, logfiledict, ref msg, "shift-jis"))
                            {
                                ConsoleShow(msg);
                                return;
                            }
                            ConsoleShow(item + "ファイルを出力しました");
                            ConsoleShow("+++ Wait:2秒 +++");
                            Thread.Sleep(2000);
                            Keylc.SetBit(maclogprefixAdDict_st[item], 1, "1");
                            ConsoleShow(item + "ファイル取得要求を立てました");
                            ConsoleShow("+++ Wait:2秒 +++");
                            Thread.Sleep(2000);
                        }
                        logno += 1;
                    }

                    ConsoleShow("■■■OR,PRファイルを出力完了■■■");

                    Thread.Sleep(2000);
                });

                RecordTn(workheader);
                ConsoleShow("■■■ECK装置上で停止(完了処理前)■■■");
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        //
        // 移載
        //
        private void btn_gbmex_autoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                GBMEX_AutoRun();
                
            }
        }
        private async void GBMEX_AutoRun()
        {
            var procno = "18,16";
            var procname = "GB/MEX";
            var magno = new string[] { txt_eck_magno1.Text, txt_eck_magno2.Text };
            var magno1 = txt_gbmex_magno1.Text;
            var magno2 = txt_gbmex_magno2.Text;
            var karamagno = txt_gbmex_km_auto.Text;
            var macInstance = ECK_01;
            var msg = "";
            //
            // log file words
            //
            var MLfolderpath = "";
            var maclogprefix_mg = new string[] { "AM", "EM" };
            var maclogprefixAdDict_mg = new Dictionary<string, string> { { "AM", "DM32052" }, { "EM", "DM32053" } };
            var date = DateTime.Now.ToString("yyyy/MM/dd");
            var time = DateTime.Now.ToString("HH:mm:ss");
            var logfiledict = new Dictionary<string, string>
            {
                { "@magno", "" },
                { "@typecd", $"{EicsParam_TypeCode}" },
                { "@date", $"{date}" },
                { "@time", $"{time}" }
            };
            if (!macInstance.getMacFolderPath("in", ref MLfolderpath))
            {
                ConsoleShow(MLfolderpath);
                return;
            }

            // EICSマガジンタイミング開始確認
            workheader = "procno." + procno + ":EICSマガジンタイミング開始";
            if (SelectYesNoMessage(workheader + "。EICSは稼働中ですか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□macno={macInstance.macno}: {macInstance.macname}□□□");
                    ConsoleShow("■■■ECK完了処理開始:EM,AMファイル出力■■■");
                    // MLフォルダクリーン
                    foreach (string f in Directory.GetFiles(MLfolderpath, "*.*"))
                    {
                        File.Delete(f);
                    }

                    //EM,AMファイルを吐き出し
                    var dt = DateTime.Now;
                    var logno = 1;
                    foreach (var mag in magno)
                    {
                        foreach (var item in maclogprefix_mg)
                        {
                            logfiledict["@magno"] = mag;
                            var MLfilename = $"Log00{logno}_" + item + dt.ToString("yyMMddHHmm") + ".csv";
                            if (!CreateMLog(macInstance.macLogOriginFolder + item + ".csv", MLfolderpath + "\\" + MLfilename, logfiledict, ref msg, "shift-jis"))
                            {
                                ConsoleShow(msg);
                                return;
                            }
                            ConsoleShow(item + "ファイルを出力しました");
                            ConsoleShow("+++ Wait:2秒 +++");
                            Thread.Sleep(2000);
                            Keylc.SetBit(maclogprefixAdDict_mg[item], 1, "1");
                            ConsoleShow(item + "ファイル取得要求を立てました");
                            ConsoleShow("+++ Wait:2秒 +++");
                            Thread.Sleep(2000);
                        }
                        logno += 1;
                    }

                    ConsoleShow("■■■EM,AMファイルを出力完了■■■");

                    Thread.Sleep(2000);
                });

                RecordTn(workheader);
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // 実マガジン～
            if (SelectYesNoMessage("遠沈マガジン#1をバッファ1に投入します。") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    ConsoleShow("■■■遠沈マガジン#1排出開始■■■");
                    if (!readRbMagQr(magno1))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    //バッファ実マガジン供給要求をON
                    SetBit(GB_01.loaderReqBitAddress, "1");
                    ConsoleShow("バッファ1実マガジン供給要求ON");

                    // 途中投入？
                    if (cb_gbmex_useniccv.Checked)
                    {
                        if (!NicConvQR(magno1))
                        {
                            return;
                        }
                        Thread.Sleep(1000);
                        // 途中投入排出要求ON
                        SetBit(NicConv_01.unloaderReqBitAddress, "1");
                        SetBit(NicConv_01.magazineArriveBitAddress, "1");
                        ConsoleShow("途中マガジン投入を要求しました");
                    }
                    else
                    {
                        //ECK設備開始時間メモリ(PLC)に書き込み
                        ConsoleShow("PLCに開始時間を登録しました");
                        var dt = DateTime.Now;
                        SetDateTime2Word(ECK_01.workStartTimeAddress, dt);
                        Thread.Sleep(1000);

                        //ECK設備完了時間メモリ(PLC)に書き込み
                        ConsoleShow("PLCに完了時間を登録しました");
                        dt = DateTime.Now;
                        SetDateTime2Word(ECK_01.workCompleteTimeAddress, dt);
                        Thread.Sleep(1000);

                        //ECKアンロードマガジン登録
                        SetString(ECK_01.ulMagazineAddress, magno1);
                        ConsoleShow("OUTマガジンNoを登録しました");

                        //実マガジン排出要求ON
                        SetBit(ECK_01.unloaderReqBitAddress1, "1");
                        ConsoleShow("実マガジン排出要求ON");
                    }

                    // ブリッジ搬送
                    if (cb_gbmex_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // +++ ロボット動作 +++
                    // +++ RB Polling +++
                    ConsoleShow("+++ ロボット移動要求確認 +++");
                    if (!RBMoveEnableToGetMag())
                    {
                        ConsoleShow("■■■自動投入停止■■■");
                        return;
                    }

                    // ロボット動作（Mag取る）
                    Rb_GetMag();
                    ConsoleShow("Mag取り動作");
                    SetBit(NicConv_01.unloaderReqBitAddress, "0");
                    SetBit(NicConv_01.magazineArriveBitAddress, "0");

                    // 排出OFF
                    if (cb_gbmex_useniccv.Checked)
                    {
                        SetBit(NicConv_01.unloaderReqBitAddress, "0");
                        ConsoleShow("途中投入をOFFしました");
                    }
                    else
                    {
                        //実マガジン排出要求OFF
                        SetBit(ECK_01.unloaderReqBitAddress1, "0");
                        ConsoleShow("実マガジン排出要求OFF");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    //実マガジン供給要求OFF
                    SetBit(GB_01.loaderReqBitAddress, "0");
                    ConsoleShow("バッファ実マガジン供給要求OFF");

                    //実マガジン排出要求ON
                    SetBit(GB_01.unloaderReqBitAddress, "1");
                    ConsoleShow("バッファ実マガジン排出要求ON");

                    // ロボット動作（Mag置く）
                    Rb_PutMag();
                    ConsoleShow("Mag置く動作");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait5秒 +++");
                    Thread.Sleep(5000);

                    ConsoleShow("■■■バッファ1・マガジン#1投入完了■■■");
                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            if (SelectYesNoMessage("遠沈マガジン#2をバッファ2に移載投入します。") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■遠沈マガジン#2排出開始■■■");
                    if (!readRbMagQr(magno2))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    //バッファ実マガジン供給要求をON
                    SetBit(GB_02.loaderReqBitAddress, "1");
                    ConsoleShow("バッファ2実マガジン供給要求ON");

                    // 途中投入？
                    if (cb_gbmex_useniccv.Checked)
                    {
                        if (!NicConvQR(magno2))
                        {
                            return;
                        }
                        Thread.Sleep(1000);
                        // 途中投入排出要求ON
                        SetBit(NicConv_01.unloaderReqBitAddress, "1");
                        SetBit(NicConv_01.magazineArriveBitAddress, "1");
                        ConsoleShow("途中マガジン投入を要求しました");
                    }
                    else
                    {
                        //ECK設備開始時間メモリ(PLC)に書き込み
                        ConsoleShow("PLCに開始時間を登録しました");
                        var dt = DateTime.Now;
                        SetDateTime2Word(ECK_01.workStartTimeAddress, dt);
                        Thread.Sleep(1000);

                        //ECK設備完了時間メモリ(PLC)に書き込み
                        ConsoleShow("PLCに完了時間を登録しました");
                        dt = DateTime.Now;
                        SetDateTime2Word(ECK_01.workCompleteTimeAddress, dt);
                        Thread.Sleep(1000);

                        //ECKアンロードマガジン登録
                        SetString(ECK_01.ulMagazineAddress, magno2);
                        ConsoleShow("OUTマガジンNoを登録しました");

                        //実マガジン排出要求ON
                        SetBit(ECK_01.unloaderReqBitAddress4, "1");
                        ConsoleShow("実マガジン排出要求ON");
                    }

                    // ブリッジ搬送
                    if (cb_gbmex_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // +++ ロボット動作 +++
                    // +++ RB Polling +++
                    ConsoleShow("+++ ロボット移動要求確認 +++");
                    if (!RBMoveEnableToGetMag())
                    {
                        ConsoleShow("■■■自動投入停止■■■");
                        return;
                    }

                    // ロボット動作（Mag取る）
                    Rb_GetMag();
                    ConsoleShow("Mag取り動作");
                    SetBit(NicConv_01.unloaderReqBitAddress, "0");
                    SetBit(NicConv_01.magazineArriveBitAddress, "0");

                    // 排出OFF
                    if (cb_gbmex_useniccv.Checked)
                    {
                        SetBit(NicConv_01.unloaderReqBitAddress, "0");
                        ConsoleShow("途中投入をOFFしました");
                    }
                    else
                    {
                        //実マガジン排出要求OFF
                        SetBit(ECK_01.unloaderReqBitAddress4, "0");
                        ConsoleShow("実マガジン排出要求OFF");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    //実マガジン供給要求OFF
                    SetBit(GB_02.loaderReqBitAddress, "0");
                    ConsoleShow("バッファ実マガジン供給要求OFF");

                    //実マガジン排出要求ON
                    SetBit(GB_02.unloaderReqBitAddress, "1");
                    ConsoleShow("バッファ実マガジン排出要求ON");

                    // ロボット動作（Mag置く）
                    Rb_PutMag();
                    ConsoleShow("Mag置く動作");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait5秒 +++");
                    Thread.Sleep(5000);

                    ConsoleShow("■■■バッファ2・マガジン#2投入完了■■■");
                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // 空マガジン～
            if (SelectYesNoMessage("空マガジンを移載機に投入します。") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    // 空マガ書き込み
                    ConsoleShow("■■■空マガジン投入■■■");
                    if (!readRbMagQr(karamagno))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    // EmpMagLoaderReq
                    SetBit(MEX_01.empMagLoaderReqBitAddress, "1");
                    ConsoleShow("空マガジン供給を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:2秒 +++");
                    Thread.Sleep(2000);

                    // 空マガジン投入
                    EntConvMagazine("kara_on");
                    ConsoleShow("ローダー空マガジン排出を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // ブリッジ搬送
                    if (cb_gbmex_emmoveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");
                        SetBit(LineBridge_01.empMagUnloaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }

                        // 空排出OFF
                        SetBit(LineBridge_01.empMagUnloaderReqBitAddress, "0");
                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    // EmpMagLoaderReq
                    SetBit(MEX_01.empMagLoaderReqBitAddress, "0");
                    ConsoleShow("空マガジン供給OFFしました");

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // Mag#2移載
            if (SelectYesNoMessage("遠沈マガジン#2を移載機に投入します。") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    //遠沈マガジン#2読込
                    ConsoleShow("■■■遠沈マガジン#2を移載機に投入■■■");
                    if (!readRbMagQr(magno2))
                    {
                        return;
                    }

                    // 移載機MagLoaderReq
                    SetBit(MEX_01.loaderReqBitAddress, "1");
                    ConsoleShow("実マガジン供給を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:1秒 +++");
                    Thread.Sleep(1000);

                    // バッファunLoaderReq
                    SetBit(GB_02.unloaderReqBitAddress, "1");
                    ConsoleShow("実マガジン排出を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // +++ ロボット動作 +++
                    // +++ RB Polling +++
                    ConsoleShow("+++ ロボット移動要求確認 +++");
                    if (!RBMoveEnableToGetMag())
                    {
                        ConsoleShow("■■■自動投入停止■■■");
                        return;
                    }

                    // ロボット動作（Mag取る）
                    Rb_GetMag();
                    ConsoleShow("Mag取り動作");
                    SetBit(NicConv_01.unloaderReqBitAddress, "0");
                    SetBit(NicConv_01.magazineArriveBitAddress, "0");

                    // 排出OFF
                    SetBit(GB_02.unloaderReqBitAddress, "0");
                    ConsoleShow("実マガジン排出をOFFしました");

                    // 移載機MagLoaderReq
                    SetBit(MEX_01.loaderReqBitAddress, "0");
                    ConsoleShow("実マガジン供給をOFFしました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // ロボット動作（Mag置く）
                    Rb_PutMag();
                    ConsoleShow("Mag置く動作");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // 完了処理EmpUnLoaderReq
                    SetBit(MEX_01.empMagUnloaderReqBitAddress, "1");
                    ConsoleShow("空マガジン排出をONしました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:10秒 +++");
                    Thread.Sleep(10000);

                    // 完了処理EmpUnLoaderReq
                    SetBit(MEX_01.empMagUnloaderReqBitAddress, "0");
                    ConsoleShow("空マガジン排出をOFFしました");

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // Mag#1移載
            if (SelectYesNoMessage("遠沈マガジン#1を移載機に投入します。") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    //遠沈マガジン#1読込
                    ConsoleShow("■■■遠沈マガジン#1を移載機に投入■■■");
                    if (!readRbMagQr(magno1))
                    {
                        return;
                    }

                    // 移載機MagLoaderReq
                    SetBit(MEX_01.loaderReqBitAddress, "1");
                    ConsoleShow("実マガジン供給を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:1秒 +++");
                    Thread.Sleep(1000);

                    // バッファunLoaderReq
                    SetBit(GB_01.unloaderReqBitAddress, "1");
                    ConsoleShow("実マガジン排出を要求しました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // +++ ロボット動作 +++
                    // +++ RB Polling +++
                    ConsoleShow("+++ ロボット移動要求確認 +++");
                    if (!RBMoveEnableToGetMag())
                    {
                        ConsoleShow("■■■自動投入停止■■■");
                        return;
                    }

                    // ロボット動作（Mag取る）
                    Rb_GetMag();
                    ConsoleShow("Mag取り動作");
                    SetBit(NicConv_01.unloaderReqBitAddress, "0");
                    SetBit(NicConv_01.magazineArriveBitAddress, "0");

                    // 排出OFF
                    SetBit(GB_01.unloaderReqBitAddress, "0");
                    ConsoleShow("実マガジン排出をOFFしました");

                    // 移載機MagLoaderReq
                    SetBit(MEX_01.loaderReqBitAddress, "0");
                    ConsoleShow("実マガジン供給をOFFしました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // ロボット動作（Mag置く）
                    Rb_PutMag();
                    ConsoleShow("Mag置く動作");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:3秒 +++");
                    Thread.Sleep(3000);

                    // 完了処理EmpUnLoaderReq
                    SetBit(MEX_01.empMagUnloaderReqBitAddress, "1");
                    ConsoleShow("空マガジン排出をONしました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:10秒 +++");
                    Thread.Sleep(10000);

                    // 完了処理EmpUnLoaderReq
                    SetBit(MEX_01.empMagUnloaderReqBitAddress, "0");
                    ConsoleShow("空マガジン排出をOFFしました");

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
                ConsoleShow("マガジン移載作業完了");
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        //
        // MDオーブン
        //
        private void btn_mdovn_autoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                AutoRunLock = true;
                MDOVN_AutoRun();
            }
        }
        private async void MDOVN_AutoRun()
        {
            var procno = "14";
            var procname = "MD-OVN";
            var MDOvnNo = cb_mdovn_macno.Text;
            var jitumag = txt_mdovn_m_auto.Text;
            var profileno = int.Parse(cb_mdovnProfile.Text);
            // 実マガジン～
            if (SelectYesNoMessage("実マガジンを投入します。") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow($"□□□procno={procno}: {procname}□□□");

                    ConsoleShow("■■■実マガジン投入■■■");

                    if (!readRbMagQr(jitumag))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    //MD-OVEN Slot1の供給をON
                    SetBit(MDOVNComboRouter(MDOvnNo).loaderReqBitAddressList_addKey1, "1");
                    ConsoleShow("スロット１実マガジン供給要求ON");

                    // 途中投入？
                    if (cb_mdovn_useniccv.Checked)
                    {
                        if (!NicConvQR(jitumag))
                        {
                            return;
                        }
                        Thread.Sleep(1000);
                        // 途中投入排出要求ON
                        SetBit(NicConv_01.unloaderReqBitAddress, "1");
                        SetBit(NicConv_01.magazineArriveBitAddress, "1");
                        ConsoleShow("途中マガジン投入を要求しました");
                    }
                    else
                    {
                        //実マガジン排出要求ON
                        SetBit(MEX_01.unloaderReqBitAddress, "1");
                        ConsoleShow("実マガジン排出要求ON");
                    }

                    // プロファイル設定
                    ConsoleShow("オーブンプロファイルを設定");
                    SetWordAsDecimalData(MDOVNComboRouter(MDOvnNo).currentProfileWordAddress, profileno);

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // ブリッジ搬送
                    if (cb_mdovn_moveToBridge.Checked)
                    {
                        ConsoleShow("ブリッジ搬送します");
                        SetBit(LineBridge_01.loaderReqBitAddress, "1");

                        // +++ ロボット動作 +++
                        if (!RBGetPutMagAuto())
                        {
                            ConsoleShow("■■■自動投入停止しました■■■");
                            return;
                        }

                    }

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    // 排出要求OFF
                    ConsoleShow("排出要求OFF");
                    SetBit(MEX_01.unloaderReqBitAddress, "0");

                    // 供給要求OFF
                    SetBit(MDOVNComboRouter(MDOvnNo).loaderReqBitAddressList_addKey1, "0");
                    ConsoleShow("供給要求OFF");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    ConsoleShow("■■■OVN装置上で停止（完了処理前）■■■");
                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }

            // 完成品排出処理
            if (SelectYesNoMessage("完成品マガジンを排出します。") == DialogResult.Yes)
            {

                await Task.Run(() =>
                {

                    ConsoleShow("■■■完成品マガジン排出■■■");

                    if (!readRbMagQr(jitumag))
                    {
                        return;
                    }
                    Thread.Sleep(1000);

                    //MDOVN設備開始時間メモリ(PLC)に書き込み
                    ConsoleShow("PLCに開始時間を登録しました");
                    var dt = DateTime.Now;
                    SetDateTime2Word(MDOVN_01.workStartTimeAddress, dt);
                    Thread.Sleep(1000);

                    //MDOVN設備完了時間メモリ(PLC)に書き込み
                    ConsoleShow("PLCに完了時間を登録しました");
                    dt = DateTime.Now;
                    SetDateTime2Word(MDOVN_01.workCompleteTimeAddress, dt);
                    Thread.Sleep(1000);

                    //MD-OVEN Slot1の供給をON
                    SetBit(MDOVNComboRouter(MDOvnNo).unloaderReqBitAddressList_addKey1, "1");
                    ConsoleShow("スロット１実マガジン排出要求ON");

                    //完成品コンベア供給要求ON
                    SetBit(MCDCConv_01.loaderReqBitAddress, "1");
                    ConsoleShow("完成品コンベア供給要求ON");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    // ブリッジ搬送
                    //if (cb_mdovn_moveToBridge.Checked)
                    //{
                    //    ConsoleShow("ブリッジ搬送します");
                    //    SetBit(LineBridge_01.loaderReqBitAddress, "1");

                    //    // +++ ロボット動作 +++
                    //    if (!RBGetPutMagAuto())
                    //    {
                    //        ConsoleShow("■■■自動投入停止しました■■■");
                    //        return;
                    //    }
                    //}

                    // +++ ロボット動作 +++
                    if (!RBGetPutMagAuto())
                    {
                        ConsoleShow("■■■自動投入停止しました■■■");
                        return;
                    }

                    // 排出要求OFF
                    ConsoleShow("排出要求OFF");
                    SetBit(MDOVNComboRouter(MDOvnNo).unloaderReqBitAddressList_addKey1, "0");

                    // 供給要求OFF
                    SetBit(MCDCConv_01.loaderReqBitAddress, "0");
                    ConsoleShow("供給要求OFF");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);

                    ConsoleShow("■■■完成品払出完了■■■");
                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                AutoRunLock = false;
                return;
            }
            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        //
        // ダム
        //
        private void btn_dam_autoRun_Click(object sender, EventArgs e)
        {
            if (!AutoRunLock)
            {
                DAM_AutoRun();
            }
        }
        private async void DAM_AutoRun()
        {
            var dam1 = new string[] { "24", "DAM-1", "424001" };
            var dam2 = new string[] { "25", "DAM-2", "425001" };
            var dam3 = new string[] { "26", "DAM-3", "426001" };
            var dam = new List<string[]> { dam1, dam2, dam3 };
            string lotno = txt_wbovn_lotno.Text;
            string magno = txt_wbovn_m_auto.Text.Replace("C30 ", "");
            // 開始完了登録のみ
            foreach (var proc in dam)
            {
                if (SelectYesNoMessage($"{proc[1]}始完了登録をします") == DialogResult.Yes)
                {
                    await Task.Run(() =>
                    {
                        ConsoleShow($"□□□procno={proc[0]}: {proc[1]}□□□");

                        ConsoleShow("■■■TnTran/TnMag操作■■■");

                        // Lotno自動取得
                        if (cb_dbpl_getLotno.Checked)
                        {
                            if (!getLastLotNofromTnTran(ref lotno))
                            {
                                ConsoleShow("ロットNoの自動取得が失敗しました");
                                return;
                            }
                            else
                            {

                            }
                        }

                        // TnTranに開始完了登録
                        var macno = int.Parse(proc[2]);
                        if (cb_dam_appendTntran.Checked)
                        {
                            var procno = int.Parse(proc[0]);
                            if (!appendDAMTran(lotno, magno, procno, macno))
                            {
                                ConsoleShow("TnTranテーブルへの書き込み時に問題発生しました");
                                return;
                            }
                            if (!updateTnMag(procno, lotno, magno))
                            {
                                ConsoleShow("TnMagテーブルへの書き込み時に問題発生しました");
                                return;
                            }
                        }

                        ConsoleShow("■■■完了■■■");
                    });
                    updateCommonFunctionSignalBtns();
                }
                else
                {
                    updateCommonFunctionSignalBtns();
                    AutoRunLock = false;
                    return;
                }
            }

            AutoRunLock = false;
            MessageBox.Show("完了しました");
        }

        private void TestBtn_Click(object sender, EventArgs e)
        {
            var dtstr = DateTime.Now.ToString("G");
            var workheader = "任意："+ dtstr;
            RecordTn(workheader);
        }

        private void cmb_zdbpram_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_leddbparam.SelectedIndex = cmb_zdbpram.SelectedIndex;
        }

        private void cmb_leddbparam_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmb_zdbpram.SelectedIndex = cmb_leddbparam.SelectedIndex;
        }

        private void btn_eics_st_Click(object sender, EventArgs e)
        {
            var inmagno = txt_md_inmagno.Text;
            var macno = cb_mb_macno.Text;
            //
            // log file words
            //
            var macInstance = MDComboRouter(macno);
            //
            // plcStrings
            //
            Keylc.SetBit(macInstance.MoldDirectionInfoAddress, 1, "1");
            Keylc.SetBit(macInstance.SyringeNumberInfoAddress, 2, "3");
            Keylc.SetBit(macInstance.MemoryTypeInfoAddress, 1, "1");
            Keylc.SetBit(macInstance.NO_DIVISION_MAPPING_MODE_ENABLE, 1, "1");
            Keylc.SetString(macInstance.LoaderQrdataAddress, inmagno);

            // 設備稼働スタート
            ConsoleShow("機種設定");
            Keylc.SetString("W0FF0", "CL-A160             ");
            ConsoleShow("設備稼働ON");
            Keylc.SetBit("B0FF1", 1, "0");
            Keylc.SetBit("B0FF2", 1, "0");
            Keylc.SetBit("B0FF0", 1, "1");
        }
    }
}
