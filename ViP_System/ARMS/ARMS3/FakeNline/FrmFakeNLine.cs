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

namespace ARMS3.FakeNline
{
    public partial class FrmFakeNline : Form
    {
        // PLC無しで起動する場合はtrueにする
        bool DebugMode = false;

        string CR = "\r\n";
        Mitsubishi Plc = new Mitsubishi("192.168.1.99", 1026);
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
            { "B000108", 0 }, //waferBitAddressStart
            { "B000107", 0 }, //startWorkTableSensorAddress
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
            { "B000148", 0 }, //waferBitAddressStart
            { "B000147", 0 }, //startWorkTableSensorAddress
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
            { "B000168", 0 }, //waferBitAddressStart
            { "B000167", 0 }, //startWorkTableSensorAddress
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
        };
        Sputter SUP_01 = new Sputter()
        {
            macname = "スパッタ2号機",
        };
        public Dictionary<string, IDebugMac> DebugMacs;
        public Dictionary<string, string> ProcessName;

        public FrmFakeNline()
        {
            plcBitData = new Dictionary<string, int>(initPlcBitData);
            plcWordData = new Dictionary<string, string>(initPlcWordData);

            InitializeComponent();

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
            //ovn
            cb_dbovnno.SelectedIndex = 1;
            cb_dbovn_procno.SelectedIndex = 0;
            cb_dbovnProfile.SelectedIndex = 0;
            //led
            cb_leddbno.SelectedIndex = 0;
            cb_leddb_fromDBOVNno.SelectedIndex = 1;
            cb_leddb_fromDBOVNSlot.SelectedIndex = 0;
            //dbpl

            //wbpl
            cb_wbpl_fromDBOVNno.SelectedIndex = 1;
            cb_wbpl_fromDBOVNSlot.SelectedIndex = 0;
            //wb
            cb_wbno.SelectedIndex = 1;
            //avi
            cb_avi_fromwb.SelectedIndex = 1;
        }

        // Combobox router
        private DieBonder ZeDBComboRouter(string macno)
        {
            string macname = "";

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
                //Plc.SetString("B000000", "30 M12345678");

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
            SetDataTimeToOvenPlc(DBOVEN_01);
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

        // Suputter
        private void btn_sup_Ldcmpl_off_Click(object sender, EventArgs e)
        {
            if (!SetBit(SUP_01.loaderMachineSelectCompleteBitAddress, "0"))
            {
                ConsoleShow("スパッタ(loaderMachineSelectCompleteBit)が書き込めませんでした");
            }
        }

        private void btn_sup_Ulcmpl_on_Click(object sender, EventArgs e)
        {
            if (!SetString(SUP_01.unloaderMachineNoAddress, txt_sup_ulno.Text))
            {
                ConsoleShow("スパッタ(UL)設備Noが書き込めませんでした");
            }
        }

        private void btn_sup_Ldcmpl_on_Click(object sender, EventArgs e)
        {

            if (!SetString(SUP_01.loaderMachineNoAddress, txt_sup_ldno.Text))
            {
                ConsoleShow("スパッタ(LD)設備Noが書き込めませんでした");
            }
        }

        private void btn_sup_Inmag_on_Click(object sender, EventArgs e)
        {
            if (!SetString(SUP_01.loaderMagazineAddress, txt_sup_inmagno.Text))
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
            if (!SetString(SUP_01.unloaderMagazineAddress, txt_sup_outmagno.Text))
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
            if (!SetBit(SUP_01.loaderQRReadCompleteBitAddress, "0"))
            {
                ConsoleShow("スパッタ(loaderQRReadCompleteBitAddress)が書き込めませんでした");
            }
        }

        private void btn_sup_Outmag_off_Click(object sender, EventArgs e)
        {
            if (!SetBit(SUP_01.unloaderQRReadCompleteBitAddress, "0"))
            {
                ConsoleShow("スパッタ(unloaderQRReadCompleteBitAddress)が書き込めませんでした");
            }
        }
        private void btn_sup_lmscba_on_Click(object sender, EventArgs e)
        {
            if (!SetBit(SUP_01.loaderMachineSelectCompleteBitAddress, "1"))
            {
                ConsoleShow("スパッタ(loaderMachineSelectCompleteBit)が書き込めませんでした");
            }
        }

        private void btn_sup_lmscba_off_Click(object sender, EventArgs e)
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

        private void btn_cln_TnVirtualMag_Click(object sender, EventArgs e)
        {
            CleanTnVirtualMag();
        }

        private void btn_cln_TnTranMag_Click(object sender, EventArgs e)
        {
            CleanTnTran();
        }

        private void btn_cln_TnMag_Click(object sender, EventArgs e)
        {
            CleanTnMag();
        }

        private void btn_cln_TnLot_Click(object sender, EventArgs e)
        {
            CleanTnLot();
        }

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

        //
        // ProcNoコントロール
        //
        private void btn_reset_proc_Click(object sender, EventArgs e)
        {
            cb_procno.SelectedIndex = 0;
            btn_autoStart.Enabled = true;
        }

        private void btn_inc_proc_Click(object sender, EventArgs e)
        {
            if (cb_procno.SelectedIndex < (cb_procno.MaxDropDownItems - 1))
            {
                cb_procno.SelectedIndex += 1;
                btn_inc_proc.Enabled = false;
                btn_autoStart.Enabled = true;
            }
        }

        private void btn_autoStart_Click(object sender, EventArgs e)
        {
            btn_inc_proc.Enabled = true;
            btn_autoStart.Enabled = false;
            AutoRunFunctionRouter(cb_procno.Text);
        }

        private void cb_procno_SelectedIndecChenged(object sender, EventArgs e)
        {
            var dictproc = ProcessName[cb_procno.Text].Split('@');
            txt_processname.Text = dictproc[0];
            btn_inc_proc.Enabled = false;
            TABBOX.SelectedTab = TABBOX.TabPages[dictproc[1]];
            if (cb_procno.Text == "7")
            {
                cb_dbovn_procno.Text = "7";
                cb_dbovnProfile.Text = "5";
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
        public bool CreateFile(string FilePath, string Contents, ref string msg)
        {
            try
            {
                // Create the file, or overwrite if the file exists.
                using (FileStream fs = File.Create(FilePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(Contents);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }

                return true;
            }

            catch (Exception ex)
            {
                msg = ex.ToString();
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

        //
        // 自動化
        //

        //
        // Ze Die Bonder
        //
        private void btn_M1DB_AutoRun_Click(object sender, EventArgs e)
        {
            M1DB_AutoRun();
        }
        private async void M1DB_AutoRun()
        {
            string msg = "";
            var karamagno = tx_m1db_empm_auto.Text;
            var aoimagno = tx_m1db_aoim_auto.Text;
            var Lfolderpath = txt_m1db_lfolderpath.Text;
            var Lfilepath = Lfolderpath + "\\" + txt_m1db_Lfilename.Text;
            var LfileContents = txt_zedb_lfilecontents.Text;
            var macname = cb_zedbno.Text;

            // 開始確認
            if (SelectYesNoMessage("ARMSは稼働中ですか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    // DBクリーン
                    if (cb_m1db_exclndb.Checked)
                    {
                        if (!CleanTnVirtualMag(false)) return;
                        if (!CleanTnTran(false)) return;
                        if (!CleanTnMag(false)) return;
                        if (!CleanTnLot(false)) return;
                    }
                    Thread.Sleep(1000);

                    // ビットメモリ初期化
                    if (cb_m1db_exclnbitmem.Checked)
                    {
                        if (!TestMemAndIni())
                        {
                            return;
                        }
                    }
                    Thread.Sleep(1000);

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
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }
            // 実マガジン～
            if (SelectYesNoMessage("実マガジンを投入します。続行しますか？") == DialogResult.Yes)
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

                    // waferBitAddressStart
                    SetBit(ZeDBComboRouter(macname).waferBitAddressStart, "1");
                    ConsoleShow("waferBitAddressStartをONしました");

                    // startWorkTableSensorAddress
                    SetBit(ZeDBComboRouter(macname).startWorkTableSensorAddress, "1");
                    ConsoleShow("startWorkTableSensorAddressをONしました");

                    // MagLoaderReq OFF
                    SetBit(ZeDBComboRouter(macname).loaderReqBitAddress, "0");
                    ConsoleShow("実マガジン供給要求OFFしました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:10秒 +++");
                    Thread.Sleep(10000);

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }
            // 工程完了登録：Lファイル書き込み～
            if (SelectYesNoMessage("工程完了登録します。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■実マガジン排出開始■■■");

                    // Lファイルクリーン、作成
                    foreach (string f in Directory.GetFiles(Lfolderpath, "L*.*"))
                    {
                        File.Delete(f);
                    }
                    CreateFile(Lfilepath, LfileContents, ref msg);

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
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }

            MessageBox.Show("完了しました");
        }


        //
        // DB OVEN
        //
        private void btn_DVOVN_AutoRun_Click(object sender, EventArgs e)
        {
            DVOVN_AutoRun();
        }
        private async void DVOVN_AutoRun()
        {
            // 実マガジン～
            if (SelectYesNoMessage("実マガジンを投入します。") == DialogResult.Yes)
            {
                var procno = cb_dbovn_procno.Text;
                var DBOvnNo = cb_dbovnno.Text;
                var ZeDBNo = cb_zedbno.Text;
                var jitumag = tx_dvovn_m_auto.Text;
                var profileno = int.Parse(cb_dbovnProfile.Text);

                await Task.Run(() =>
                {
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

                        // 実マガジンCV投入
                        if (!SetString(NicConv_01.MagNo, jitumag))
                        {
                            ConsoleShow("途中投入QRが書き込めませんでした");
                            return;
                        }
                        EntConvMagazine("nichia_on");
                        SetBit(NicConv_01.magazineArriveBitAddress, "1");
                        ConsoleShow("CVローダー実マガジン排出を要求しました");

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

                    // 排出要求OFF
                    ConsoleShow("排出要求OFF");
                    SetBit(ZeDBComboRouter(ZeDBNo).unloaderReqBitAddress, "0");

                    // 供給要求OFF
                    SetBit(DBOvnComboRouter(DBOvnNo).loaderReqBitAddressList_addKey1, "0");
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
                return;
            }

            MessageBox.Show("完了しました");
        }

        //
        // LED DB
        //
        private void btn_LEDDB_AutoRun_Click(object sender, EventArgs e)
        {
            LEDDB_AutoRun();
        }
        private async void LEDDB_AutoRun()
        {
            string msg = "";
            var karamagno = tx_leddb_empm_auto.Text;
            var jitumagno = tx_leddb_m_auto.Text;
            var ovnmacno = cb_leddb_fromDBOVNno.Text;
            var Lfolderpath = txt_leddb_lfolderpath.Text;
            var Lfilepath = Lfolderpath + "\\" + txt_leddb_Lfilename.Text;
            var LfileContents = txt_leddb_lfilecontents.Text;
            var macname = cb_leddbno.Text;

            // 空マガジン～
            if (SelectYesNoMessage("空マガジンを投入します。") == DialogResult.Yes)
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
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }
            // 実マガジン～
            if (SelectYesNoMessage("実マガジンを投入します。続行しますか？") == DialogResult.Yes)
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

                    // waferBitAddressStart
                    SetBit(LEDDBComboRouter(macname).waferBitAddressStart, "1");
                    ConsoleShow("waferBitAddressStartをONしました");

                    // startWorkTableSensorAddress
                    SetBit(LEDDBComboRouter(macname).startWorkTableSensorAddress, "1");
                    ConsoleShow("startWorkTableSensorAddressをONしました");

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
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }
            // 工程完了登録：Lファイル書き込み～
            if (SelectYesNoMessage("工程完了登録します。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■実マガジン排出開始■■■");
                    // Lファイルクリーン、作成
                    foreach (string f in Directory.GetFiles(Lfolderpath, "L*.*"))
                    {
                        File.Delete(f);
                    }
                    CreateFile(Lfilepath, LfileContents, ref msg);

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
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }

            MessageBox.Show("完了しました");
        }

        //
        // DBPL
        //
        private void btn_DBPL_AutoRun_Click(object sender, EventArgs e)
        {
            DBPL_AutoRun();
        }
        private async void DBPL_AutoRun()
        {
            // 実マガジン～
            if (SelectYesNoMessage("実マガジンをライン外に排出します") == DialogResult.Yes)
            {
                var DBNo = cb_leddbno.Text;

                await Task.Run(() =>
                {
                    ConsoleShow("■■■DB装置から実マガジン払出■■■");

                    // Lotno自動取得
                    string lotno = "";
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
                        if (!appendDBPLTran(lotno))
                        {
                            ConsoleShow("TnTranテーブルへの書き込み時に問題発生しました");
                            return;
                        }
                        if (!updateDBTnMag(5, lotno))
                        {
                            ConsoleShow("TnMagテーブルへの書き込み時に問題発生しました");
                            return;
                        }
                    }

                    ConsoleShow("■■■ライン外払出で停止■■■");
                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }

            MessageBox.Show("完了しました");
        }

        private void btn_dbpl_appendTntran_Click(object sender, EventArgs e)
        {
            appendDBPLTran();
        }

        private void btn_dbpl_updateTnMag_Click(object sender, EventArgs e)
        {
            updateDBTnMag(5);
        }

        private void btn_get_lastLotno_Click(object sender, EventArgs e)
        {
            string lotno = "";
            if (getLastLotNofromTnTran(ref lotno))
            {
                tx_dbp_lotno.Text = lotno;
            }
            else
            {
                ConsoleShow("ロットNOの取得が失敗しました");
            }
        }

        public bool appendDBPLTran(string lotno = "")
        {
            try
            {
                if (string.IsNullOrEmpty(lotno)) lotno = tx_dbp_lotno.Text;
                var karamag = tx_dbp_empm_auto.Text.Replace("30 ", "");
                //var jitumag = tx_dbp_m_auto.Text.Replace("30 ", "");
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
                    cmd.CommandText += $",('{karamag}')";
                    cmd.CommandText += $",('{karamag}')";
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
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(null)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(1)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += ",(0)";
                    cmd.CommandText += " ,(0)";
                    cmd.CommandText += " ,(0))";

                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                    return true;

                }
            }
            catch (Exception ex)
            {
                ConsoleShow(ex.ToString());
                return false;
            }
        }

        public bool updateDBTnMag(int procno, string lotno = "")
        {
            try
            {
                if (string.IsNullOrEmpty(lotno)) lotno = tx_dbp_lotno.Text;
                var karamag = tx_dbp_empm_auto.Text.Replace("30 ", "");
                //var jitumag = tx_dbp_m_auto.Text.Replace("30 ", "");
                DateTime dt = DateTime.Now;

                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = $"UPDATE [dbo].[TnMag] ";
                    cmd.CommandText += $"SET [inlineprocno] = ({procno}) ";
                    cmd.CommandText += $"WHERE lotno = '{lotno}' and magno = '{karamag}'";

                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();

                    return true;
                }
            }
            catch (Exception ex)
            {
                ConsoleShow(ex.ToString());
                return false;
            }
        }

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
        // WBPL
        //
        private void btn_WBPL_AutoRun_Click(object sender, EventArgs e)
        {
            WBPL_AutoRun();
        }
        private async void WBPL_AutoRun()
        {
            string msg = "";
            var karamagno = tx_wbpl_empm_auto.Text;
            var jitumagno = tx_wbpl_m_auto.Text;
            var ovnmacno = cb_wbpl_fromDBOVNno.Text; ;

            // 空マガジン～
            if (SelectYesNoMessage("空マガジンを投入します。") == DialogResult.Yes)
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
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }
            // 実マガジン～
            if (SelectYesNoMessage("実マガジンを投入します。続行しますか？") == DialogResult.Yes)
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

                    // OVN排出要求ON
                    SetDataTimeToOvenPlc(DBOvnComboRouter(ovnmacno));
                    ConsoleShow("開始完了時刻を書き込んでいます:Wait1秒");
                    Thread.Sleep(1000);
                    SetBit(DBOvnComboRouter(ovnmacno).unloaderReqBitAddressList_addKey1, "1");
                    ConsoleShow("オーブンの実マガジン排出を要求しました");

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

                    // OVN排出要求OFF
                    SetBit(DBOvnComboRouter(ovnmacno).unloaderReqBitAddressList_addKey1, "0");
                    ConsoleShow("オーブンの実マガジン排出をOFFしました");

                    // +++ Wait +++
                    ConsoleShow("+++ Wait:10秒 +++");
                    Thread.Sleep(10000);

                    ConsoleShow("■■■WBプラズマ装置上で停止(完了処理前)■■■");
                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }

            MessageBox.Show("完了しました");
        }

        //
        // WBPL
        //
        private void btn_WB_AutoRun_Click(object sender, EventArgs e)
        {
            WB_AutoRun();
        }
        private async void WB_AutoRun()
        {
            string msg = "";
            var karamagno = tx_wb_empm_auto.Text;
            var jitumagno = tx_wb_m_auto.Text;
            var macname = cb_wbno.Text;
            var MMfileorigin = tx_mmfileorigin.Text;
            var MMfolderpath = tx_mmfolderpath.Text;

            // 空マガジン～
            if (SelectYesNoMessage("空マガジンを投入します。") == DialogResult.Yes)
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
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }
            // 実マガジン～
            if (SelectYesNoMessage("実マガジンを投入します。続行しますか？") == DialogResult.Yes)
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

                    // 途中投入／WBPL排出要求ON
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
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }
            // 工程完了登録：MMファイル書き込み～
            if (SelectYesNoMessage("工程完了登録します。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■実マガジン排出開始■■■");

                    // MMファイルクリーン、作成
                    ConsoleShow("MMファイル作成しています");
                    foreach (string f in Directory.GetFiles(MMfolderpath, "MM*.*"))
                    {
                        File.Delete(f);
                    }
                    DateTime dt = DateTime.Now;
                    var MMfilename = "MM0000" + dt.ToString("yyMMddHHmmss") + ".csv";
                    CopyFile(MMfileorigin, MMfolderpath + "\\" + MMfilename);

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
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }

            MessageBox.Show("完了しました");
        }

        private void btn_avi_AUTORun_Click(object sender, EventArgs e)
        {
            AVI_AutoRun();
        }

        private async void AVI_AutoRun()
        {
            string wbmacno = cb_avi_fromwb.Text;
            string nasfolderpath = txt_avi_nasfilefoder.Text;
            string jitumagno = txt_avi_jitumag.Text;
            string maccode = txt_avi_maccode.Text;
            string nasfilecontents = txt_avi_nasfilecontents.Text;
            string msg = "";
            var MMfileorigin = tx_avi_mmfileorigin.Text;
            var MMfolderpath = tx_avi_mmfolderpath.Text;

            if (SelectYesNoMessage("実マガジンを投入します") == DialogResult.Yes)
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
                        ConsoleShow("WBPLの実マガジン排出をOFFしました");
                    }

                    // +++ Wait +++
                    ConsoleShow("+++ Wait5秒 +++");
                    Thread.Sleep(5000);

                    ConsoleShow("停止中");
                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }
            // 工程完了・排出：nasファイル書き込み～
            if (SelectYesNoMessage("MMファイル処理をします。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■実マガジン排出開始■■■");

                    // MMファイルクリーン、作成
                    ConsoleShow("MMファイル作成しています");
                    foreach (string f in Directory.GetFiles(MMfolderpath, "MM*.*"))
                    {
                        File.Delete(f);
                    }
                    DateTime dt = DateTime.Now;
                    var MMfilename = "Log000_MM" + dt.ToString("yyMMddHHmm") + ".csv";
                    CopyFile(MMfileorigin, MMfolderpath + "\\" + MMfilename);
                    ConsoleShow("+++ Wait:5秒 +++");
                    Thread.Sleep(5000);
                    ConsoleShow("MMファイルを出力しました");
                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }
            // 工程完了・排出：nasファイル書き込み～
            if (SelectYesNoMessage("工程完了・排出します。続行しますか？") == DialogResult.Yes)
            {
                await Task.Run(() =>
                {
                    ConsoleShow("■■■実マガジン排出開始■■■");

                    // avi装置へのMagno問い合わせ
                    ReadAviQrCode(jitumagno);
                    ConsoleShow("avi上マガジンNoを書き込みました");

                    // nasファイルクリーン、作成
                    foreach (string f in Directory.GetFiles(nasfolderpath, "*.nas"))
                    {
                        File.Delete(f);
                    }
                    var nasfilename = $"L0000000000_{jitumagno.Replace("30 ", "")}_{maccode}.nas";
                    var nasfilepath = txt_avi_nasfilefoder.Text + "\\" + nasfilename;
                    CreateFile(nasfilepath, nasfilecontents, ref msg);

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

                    //途中排出CVをON
                    SetBit(DCConv_01.loaderReqBitAddress, "0");
                    ConsoleShow("途中排出コンベア供給要求OFF");

                    ConsoleShow("■■■ライン外排出で停止■■■");

                });
                updateCommonFunctionSignalBtns();
            }
            else
            {
                updateCommonFunctionSignalBtns();
                return;
            }

            MessageBox.Show("完了しました");
        }


    }
}
