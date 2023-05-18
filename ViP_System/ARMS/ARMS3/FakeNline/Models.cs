using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.FakeNline
{
    //装置インターフェース
    public interface IDebugMac
    {
        string macname { get; set; }
        //Bit Prop 共通
        string machineReadyBitAddress { get; set; }
        string loaderReqBitAddress { get; set; }
        string unloaderReqBitAddress { get; set; }
        string clearMagazinesBitAddress { get; set; }
        string waferBitAddressStart { get; set; }
        string startWorkTableSensorAddress { get; set; }
        string waferChangerChangeBitAddress { get; set; }
        string empMagUnloaderReqBitAddress { get; set; }
        string empMagLoaderReqBitAddress { get; set; }
        string inputForbiddenBitAddress { get; set; }
        string dischargeModeBitAddress { get; set; }
        string secondLoaderReqBitAddress { get; set; }
        string secondEmpMagLoaderReqBitAddress { get; set; }
    }

    //ロボット
    public class Robot
    {
        //Bit Prop
        public string STAT_ROBOT_COMMAND_READY { get; set; } = "B0000E0";
        public string REQ_ROBOT_MOVE_ORG2MID { get; set; } = "B0000F0";
        public string MAGAZINE_EXIST_QR_STAGE { get; set; } = "B0000EF";
        public string ROBOT_MOVE2_LINER_COMPLETE { get; set; } = "B0000E5";
        public string REQ_ROBOT_MOVE2_MID2DST { get; set; } = "B0000F6";
        // Word Prop
        public string robotQRWordAddressStart { get; set; } = "W0000B0";
    }

    //橋
    public class Bridge
    {
        public string robotQRWordAddressStart { get; set; } = "W0000B0";
    }

    //オーブンプロファイル自動変更
    public class OvenProfile
    {
        //Bit Prop
        public string profileChangerReadyBitAddress { get; set; } = "B0000FE";
        public string dbOvenChangeInterlockStartAddress { get; set; } = "B000060";
        public string moldOvenChangeInterlockStartAddress { get; set; } = "B00006E";
        public string dbOvenAutoModeBitStartAddress { get; set; } = "B001140";
        public string moldOvenAutoModeBitStartAddress { get; set; } = "B00114E";
        public string dbOvenProfileChangeCompleteBitStartAddress { get; set; } = "B001160";
        public string moldOvenProfileChangeCompleteBitStartAddress { get; set; } = "B00116E";
        public string dbOvenProfileChangeRequestBitStartAddress { get; set; } = "B001180";
        public string moldOvenProfileChangeRequestBitStartAddress { get; set; } = "B00118E";
        // Word Prop
        public string dbOvenCurrentProfileStartAddress { get; set; } = "W000040";
        public string moldOvenCurrentProfileStartAddress { get; set; } = "W00004E";
        public string dbOvenReserveProfileStartAddress { get; set; } = "W000060";
        public string moldOvenReserveProfileStartAddress { get; set; } = "W00006E";
    }

    //途中投入コンベヤ
    public class NichiaConv : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "途中投入コンベヤ";
        //Bit Prop 共通
        public string machineReadyBitAddress { get; set; } = "B0002D0";
        public string unloaderReqBitAddress { get; set; } = "B0002D2";
        public string magazineArriveBitAddress { get; set; } = "B0000E8";
        public string missingReservedMagazineBitAddress { get; set; } = "B0000EB";
        public string outputReserveBitAddress { get; set; } = "B0000F2";
        public string loaderReqBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string prioritySettingAddress { get; set; } = "W0000A0";
        public string MagNo { get; set; } = "W0000D0";
    }

    //途中払出コンベヤ
    public class DischargeConv : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "途中払出コンベヤ";
        //Bit Prop 共通
        public string machineReadyBitAddress { get; set; } = "B0002C8";
        public string loaderReqBitAddress { get; set; } = "B0002CA";
        public string unloaderReqBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string prioritySettingAddress { get; set; } = "W00009F";
    }

    //完成品払出コンベヤ
    public class MAPCompltDischargeConv : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "完成品払出コンベヤ";
        //Bit Props
        public string machineReadyBitAddress { get; set; } = "B0002E8";
        public string loaderReqBitAddress { get; set; } = "B0002EA";
        public string unloaderReqBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string prioritySettingAddress { get; set; } = "W0000A9";
    }

    //AOI払出コンベヤ
    public class MAPBoardDischargeConv : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "AOI払出コンベヤ";
        //Bit Props
        public string machineReadyBitAddress { get; set; } = "B0002B8";
        public string loaderReqBitAddress { get; set; } = "B0002BA";
        public string unloaderReqBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string prioritySettingAddress { get; set; } = "W00009D";
    }

    //空マガジン投入コンベヤ
    public class KaraConv : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "空マガジン投入コンベヤ";
        //Bit Props
        public string machineReadyBitAddress { get; set; } = "B0002C0";
        public string empMagUnloaderReqBitAddress { get; set; } = "B0002C2";
        public string loaderReqBitAddress { get; set; }
        public string unloaderReqBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string prioritySettingAddress { get; set; } = "W00009E";
    }

    //MAP基板マガジン(AOI)投入コンベヤ
    public class AoiConv : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "MAP基板(AOI)マガジン投入コンベヤ";
        //Bit Props
        public string machineReadyBitAddress { get; set; } = "B0002B0";
        public string empMagUnloaderReqBitAddress { get; set; } = "B0002B2";
        public string loaderReqBitAddress { get; set; }
        public string unloaderReqBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string prioritySettingAddress { get; set; } = "W00009C";
    }

    //ライン間ブリッジ
    public class LineBridge : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "ライン間ブリッジ";
        //Bit Props
        public string machineReadyBitAddress { get; set; }
        public string loaderReqBitAddress { get; set; }
        public string unloaderReqBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagInputBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string prioritySettingAddress { get; set; }
    }

    //ライン内バッファ
    public class GeneralBuffer : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "ライン内バッファ";
        //Bit Props
        public string machineReadyBitAddress { get; set; }
        public string loaderReqBitAddress { get; set; }
        public string unloaderReqBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string prioritySettingAddress { get; set; }
    }


    //ダイボンダー
    public class DieBonder : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "ダイボンダー";
        //Bit Props
        public string machineReadyBitAddress { get; set; }
        public string loaderReqBitAddress { get; set; }
        public string unloaderReqBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string prioritySettingAddress { get; set; }
    }

    //DB OVEN
    public class Oven : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "DB OVEN";
        //Bit Props
        public string machineReadyBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string loaderReqBitAddressList_addKey1 { get; set; }
        public string loaderReqBitAddressList_addKey2 { get; set; }
        public string unloaderReqBitAddressList_addKey1 { get; set; }
        public string unloaderReqBitAddressList_addKey2 { get; set; }
        public string changeProfileInterlockBitAddress { get; set; }
        public string loaderReqBitAddress { get; set; }
        public string unloaderReqBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }

        //Word Props
        public string prioritySettingAddress { get; set; }
        public string workStartTimeAddress { get; set; }
        public string workCompleteTimeAddress { get; set; }
        public string profileStatusAddress { get; set; }
        public string currentProfileWordAddress { get; set; }
        public string ovenProcessStatusWordAddress { get; set; }
    }

    //プラズマ（自動搬送）
    public class MultiLoadPlasma : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "プラズマ";
        //Bit Props
        public string machineReadyBitAddress { get; set; } = "B000321";
        public string clearMagazinesBitAddress { get; set; } = "B0000DB";
        public string inputForbiddenBitAddress { get; set; } = "B00008D";
        public string dischargeModeBitAddress { get; set; } = "B0000AD";
        public string loader_lane_key1 { get; set; } = "B000323";
        public string loader_lane_key2 { get; set; } = "B000324";
        public string loader_lane_key3 { get; set; } = "B000325";
        public string empMagUnloader_lane_key1 { get; set; } = "B000326";
        public string empMagUnloader_lane_key2 { get; set; } = "B000327";
        public string empMagUnloader_lane_key3 { get; set; } = "B000328";
        public string empMagLoader_lane_key1 { get; set; } = "B000329";
        public string empMagLoader_lane_key2 { get; set; } = "B00032A";
        public string empMagLoader_lane_key3 { get; set; } = "B00032B";
        public string unloader_lane_key1 { get; set; } = "B00032C";
        public string unloader_lane_key2 { get; set; } = "B00032D";
        public string unloader_lane_key3 { get; set; } = "B00032E";
        public string loaderReqBitAddress { get; set; }
        public string unloaderReqBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }

        //Word Props
        public string prioritySettingAddress { get; set; } = "W00008D";
    }

    //ワイヤボンダー
    public class WireBonder : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "ワイヤボンダー";
        //Bit Props
        public string machineReadyBitAddress { get; set; }
        public string loaderReqBitAddress { get; set; }
        public string unloaderReqBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public int inspectProcNo { get; set; }
        public string mmFilePath { get; set; }
        //Word Props
        public string prioritySettingAddress { get; set; }
    }

    //外観検査機
    public class Inspector : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "外観検査機";
        //Bit Props
        public string machineReadyBitAddress { get; set; } = "B001601";
        public string loaderReqBitAddress { get; set; } = "B001603";
        public string unloaderReqBitAddress { get; set; } = "B001604";
        public string inputForbiddenBitAddress { get; set; } = "B000093";
        public string dischargeModeBitAddress { get; set; } = "B0000B3";
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string prioritySettingAddress { get; set; } = "W000093";
        public string workCompleteTimeAddress { get; set; } = "W001607";
        public string workStartTimeAddress { get; set; } = "W001601";
        public string ulMagazineAddress { get; set; } = "W001610";
    }

    //スパッタ
    public class Sputter : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "スパッタ";
        //Bit Props
        public string heartBeatAddress { get; set; } = "B002100";
        public string loaderQRReadCompleteBitAddress { get; set; } = "B002101";
        public string loaderMachineSelectCompleteBitAddress { get; set; } = "B002102";
        public string loaderWorkCompleteBitAddress { get; set; } = "B002103";
        public string loaderWorkErrorBitAddress { get; set; } = "B002104";
        public string unloaderQRReadCompleteBitAddress { get; set; } = "B002105";
        public string unloaderWorkCompleteBitAddress { get; set; } = "B002106";
        public string unloaderWorkErrorBitAddress { get; set; } = "B002107";
        public string relateLotTrayFunctionBitAddress { get; set; } = "B002108";
        public string machineReadyBitAddress { get; set; } = "";
        public string loaderReqBitAddress { get; set; } = "";
        public string unloaderReqBitAddress { get; set; } = "";
        public string inputForbiddenBitAddress { get; set; } = "";
        public string dischargeModeBitAddress { get; set; } = "";
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string workStartTimeAddress { get; set; } = "W002100";
        public string loaderMachineNoAddress { get; set; } = "W002108";
        public string loaderMagazineAddress { get; set; } = "W002110";
        public string loaderTrayAddress { get; set; } = "W002118";
        public string loaderTrayOrderAddress { get; set; } = "W002120";
        public string workCompleteTimeAddress { get; set; } = "W002128";
        public string unloaderMachineNoAddress { get; set; } = "W002130";
        public string unloaderMagazineAddress { get; set; } = "W002138";
        public string unloaderTrayAddress { get; set; } = "W002140";
        public string unloaderTrayOrderAddress { get; set; } = "W002148";
    }

    //モールド
    public class Mold : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "モールド";
        //Bit Props
        public string machineReadyBitAddress { get; set; }
        public string loaderReqBitAddress { get; set; }
        public string unloaderReqBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string preInputDischargeModeBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        //Word Props
        public string workCompleteTimeAddress { get; set; }
        public string workStartTimeAddress { get; set; }
        public string prioritySettingAddress { get; set; }
        public string lMagazineAddress { get; set; }
        public string ulMagazineAddress { get; set; }
        public string eulMagazineAddress { get; set; }
        public string elMagazineCountAddress { get; set; }
        public string lMagazineCountAddress { get; set; }
        public string eulMagazineCountAddress { get; set; }
        public string ulMagazineCountAddress { get; set; }
    }

    //遠沈（自動搬送）
    public class ECK : IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "遠沈";
        //Bit Props
        public string machineReadyBitAddress { get; set; } = "B001901";
        public string inputForbiddenBitAddress { get; set; } = "B000096";
        public string dischargeModeBitAddress { get; set; } = "B0000B6";
        public string dummyMagStationReqBitAddress { get; set; } = "B0002E2";
        public string dummyMagStationOutputReqBitAddress { get; set; } = "B0002E3";
        public string dummyMagReqBitAddress { get; set; } = "B001907";
        public string loaderReqBitAddress { get; set; }
        public string unloaderReqBitAddress { get; set; }
        public string clearMagazinesBitAddress { get; set; }
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; }
        public string empMagLoaderReqBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }

        //Word Props    
        public string workCompleteTimeAddress { get; set; } = "W001907";
        public string workStartTimeAddress { get; set; } = "W001901";
        public string prioritySettingAddress { get; set; } = "W000096";
        public string ulMagazineAddress { get; set; } = "W001910";
    }

}
