using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ArmsApi;
using System.Data;

namespace ARMS3.FakeVSPline
{
    //
    // ARMS装置、キャリア
    //

    //装置インターフェース
    public interface IDebugMac
    {
        string macname { get; set; }
        string macno { get; set; }
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

    public class Machine
    {
        public string macno { get; set; }

        public bool getMacFolderPath(string inout, ref string msg)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.Parameters.Add("@macno", SqlDbType.NVarChar).Value = macno;
                    if (inout == "in")
                    {
                        cmd.CommandText = "SELECT TOP 1 loginputdirectorypath  FROM TmMachine WHERE macno = @macno";
                    }
                    else
                    {
                        cmd.CommandText = "SELECT TOP 1 logoutputdirectorypath FROM TmMachine WHERE macno = @macno";
                    }
                    msg = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();
                }
                return true;
            }
            catch (Exception ex)
            {
                msg = "SQL FAIL!: " + ex.ToString();
                return false;
            }
        }
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
    public class NichiaConv : Machine, IDebugMac
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
    public class DischargeConv : Machine, IDebugMac
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
    public class MAPCompltDischargeConv : Machine, IDebugMac
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

    //Machine , IDebugMac
    public class MAPBoardDischargeConv : Machine, IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "AOI排出コンベヤ";
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
    public class KaraConv : Machine, IDebugMac
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
    public class AoiConv : Machine, IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "アオイ基板投入コンベヤ";
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
    public class LineBridge : Machine, IDebugMac
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

    //マガジン交換機
    public class MagExchanger : Machine, IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "マガジン交換機";
        //Bit Props
        public string machineReadyBitAddress { get; set; } = "B0002D8";
        public string loaderReqBitAddress { get; set; } = "B0002DC";
        public string unloaderReqBitAddress { get; set; } = "B0002DB";
        public string clearMagazinesBitAddress { get; set; } = "B0000DF";
        public string waferBitAddressStart { get; set; }
        public string startWorkTableSensorAddress { get; set; }
        public string waferChangerChangeBitAddress { get; set; }
        public string empMagUnloaderReqBitAddress { get; set; } = "B0002DD";
        public string empMagLoaderReqBitAddress { get; set; } = "B0002DA";
        public string inputForbiddenBitAddress { get; set; }
        public string dischargeModeBitAddress { get; set; }
        public string secondLoaderReqBitAddress { get; set; }
        public string secondEmpMagLoaderReqBitAddress { get; set; }
        public string magShiftEnableBitAddress { get; set; } = "B0000FD";
        public string unloaderMagReverseBitAddress { get; set; } = "B0000FC";
        public string dischargeModeProcBitAddress_16 { get; set; } = "B0000BA";
        public string dischargeModeProcBitAddress_20 { get; set; } = "B0000BB";
        //Word Props
        public string prioritySettingAddress { get; set; }
    }

    //ライン内バッファ
    public class GeneralBuffer : Machine, IDebugMac
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
    public class DieBonder : Machine, IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "ダイボンダー";
        public string macLogOriginFolder { get; set; }
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
    public class Oven : Machine, IDebugMac
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
    public class MultiLoadPlasma : Machine, IDebugMac
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
    public class WireBonder : Machine, IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "ワイヤボンダー";
        public string equipment_no { get; set; }
        public string macLogOriginFolder { get; set; } = "C:\\QCIL\\MacLogOrigin\\WB\\";
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
    public class Inspector : Machine, IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "外観検査機";
        public string macLogOriginFolder { get; set; } = "C:\\QCIL\\MacLogOrigin\\AVI\\";
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
        public string mLogFileRecieveReady { get; set; } = "B001605";
        public string mLogFileRecieveRequest { get; set; } = "B001606";
        //Word Props
        public string prioritySettingAddress { get; set; } = "W000093";
        public string workCompleteTimeAddress { get; set; } = "W001607";
        public string workStartTimeAddress { get; set; } = "W001601";
        public string ulMagazineAddress { get; set; } = "W001610";
        //for LENS/EICS
        public string OperationalStatusAddress { get; set; } = "B00000";
        public string RequestTypeCdAddress { get; set; } = "DM20099";
        public string ResponseTypeCdAddress { get; set; } = "DM20100";
        public string CompleteOutputMachineLogAddress { get; set; } = "DM20052";
        public string MappingFunctionAddress { get; set; } = "DM20040";
        public string ChangePointAddress { get; set; } = "DM20070";
        public string LoaderQrdataAddress { get; set; } = "DM20120";
        public string MappingStartAddress { get; set; } = "DM00000";
        public string MissingInspectionAddress { get; set; } = "DM20021";
        public string MagazineInspectionStepAddress { get; set; } = "DM20200";
        public string StartableProcessAddress { get; set; } = "B0022";
    }

    //スパッタ
    public class Sputter : Machine, IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "スパッタ";
        public string macLogOriginFolder { get; set; } = "C:\\QCIL\\MacLogOrigin\\SUP\\";
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
    public class Mold : Machine, IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "モールド";
        public string macLogOriginFolder { get; set; } = "C:\\QCIL\\MacLogOrigin\\MD\\";
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
        // for LENS
        public string MappingRequestAddress { get; set; } = "DM30060";
        public string MoldDirectionInfoAddress { get; set; } = "DM30065";
        public string SyringeNumberInfoAddress { get; set; } = "DM30066";
        public string MemoryTypeInfoAddress { get; set; } = "DM30067";
        public string LoaderQrdataAddress { get; set; } = "DM31000";
        public string CompleteOutputMachineLogAddress { get; set; } = "DM30057";
        public string NO_DIVISION_MAPPING_MODE_ENABLE { get; set; } = "DM30073";
        // for PLC Param check
        public string ProgramName { get; set; }
    }

    //遠沈（自動搬送）
    public class ECK : Machine, IDebugMac
    {
        //Mac名称
        public string macname { get; set; } = "遠沈";
        public string macLogOriginFolder { get; set; } = "C:\\QCIL\\MacLogOrigin\\ECK\\";
        //Bit Props
        public string machineReadyBitAddress { get; set; } = "B001901";
        public string inputForbiddenBitAddress { get; set; } = "B000096";
        public string dischargeModeBitAddress { get; set; } = "B0000B6";
        public string dummyMagStationReqBitAddress { get; set; } = "B0002E2";
        public string dummyMagStationOutputReqBitAddress { get; set; } = "B0002E3";
        public string dummyMagReqBitAddress { get; set; } = "B001907";
        public string loaderReqBitAddress { get; set; }
        public string unloaderReqBitAddress { get; set; }
        public string loaderReqBitAddress1 { get; set; } = "B001903";
        public string loaderReqBitAddress2 { get; set; } = "B001904";
        public string loaderReqBitAddress3 { get; set; } = "B001908";
        public string loaderReqBitAddress4 { get; set; } = "B001909";
        public string loaderReqBitAddress5 { get; set; } = "B00190C";
        public string loaderReqBitAddress6 { get; set; } = "B00190D";
        public string unloaderReqBitAddress1 { get; set; } = "B001905";
        public string unloaderReqBitAddress2 { get; set; } = "B001906";
        public string unloaderReqBitAddress3 { get; set; } = "B00190A";
        public string unloaderReqBitAddress4 { get; set; } = "B00190B";
        public string unloaderReqBitAddress5 { get; set; } = "B00190E";
        public string unloaderReqBitAddress6 { get; set; } = "B001903F";
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


    //
    // ARMS DB
    //
    public class TnLot
    {
        public string lotno { get; set; }
        public string typecd { get; set; }
        public string warningfg { get; set; } = "0";
        public string restrictfg { get; set; } = "0";
        public string profileid { get; set; } = "60000";
        public string mnfctplanno { get; set; } = null;
        public string blendcd { get; set; } = "BC";
        public string resingpcd { get; set; } = "RC";
        public string cutblendcd { get; set; } = "CC";
        public string lifetestfg { get; set; } = "0";
        public string khltestfg { get; set; } = "0";
        public string colortestfg { get; set; } = "0";
        public string templotno { get; set; }
        public string istemp { get; set; } = "0";
        public string isnascalotcharend { get; set; } = "0";
        public string lastupddt { get; set; }
        public string lotsize { get; set; } = null;
        public string isfullsizeinspection { get; set; } = null;
        public string ismappinginspection { get; set; } = null;
        public string ischangepointlot { get; set; } = null;
        public string macgroup { get; set; } = "2";
        public string isbadmarkframe { get; set; } = "0";
        public string dbthrowdt { get; set; }
        public string reflowtestfg { get; set; } = "0";
        public string reflowtestwirebondfg { get; set; } = "0";
        public string elasticitytestfg { get; set; } = "0";
        public string diesheartestfg { get; set; } = "0";
        public string movestockct { get; set; } = "0";
        public string beforelifetestcondcd { get; set; } = "";
        public string tempcutblendid { get; set; } = "0";
        public string tempcutblendno { get; set; } = "";
        public string mnggrid { get; set; } = null;
        public string mnggrnm { get; set; } = null;
        public string limitsheartestfg { get; set; } = "0";
        public string resingpcd2 { get; set; } = "";

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnlot.csv";

        public bool insertTnlot(ref string msg)
        {
            DateTime dt = DateTime.Now;
            lastupddt = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO[dbo].[TnLot]" +
            "([lotno]" +
            ",[typecd]" +
            ",[warningfg]" +
            ",[restrictfg]" +
            ",[profileid]" +
            ",[mnfctplanno]" +
            ",[blendcd]" +
            ",[resingpcd]" +
            ",[cutblendcd]" +
            ",[lifetestfg]" +
            ",[khltestfg]" +
            ",[colortestfg]" +
            ",[templotno]" +
            ",[istemp]" +
            ",[isnascalotcharend]" +
            ",[lastupddt]" +
            ",[lotsize]" +
            ",[isfullsizeinspection]" +
            ",[ismappinginspection]" +
            ",[ischangepointlot]" +
            ",[macgroup]" +
            ",[isbadmarkframe]" +
            ",[dbthrowdt]" +
            ",[reflowtestfg]" +
            ",[reflowtestwirebondfg]" +
            ",[elasticitytestfg]" +
            ",[diesheartestfg]" +
            ",[movestockct]" +
            ",[beforelifetestcondcd]" +
            ",[tempcutblendid]" +
            ",[tempcutblendno]" +
            ",[mnggrid]" +
            ",[mnggrnm]" +
            ",[limitsheartestfg]" +
            ",[resingpcd2])" +
         " VALUES " +
            $"('{lotno}'" +
            $",'{typecd}'" +
            $",'{warningfg}'" +
            $",'{restrictfg}'" +
            $",'{profileid}'" +
            $",'{mnfctplanno}'" +
            $",'{blendcd}'" +
            $",'{resingpcd}'" +
            $",'{cutblendcd}'" +
            $",'{lifetestfg}'" +
            $",'{khltestfg}'" +
            $",'{colortestfg}'" +
            $",'{templotno}'" +
            $",'{istemp}'" +
            $",'{isnascalotcharend}'" +
            $",'{lastupddt}'" +
            $",'{lotsize}'" +
            $",'{isfullsizeinspection}'" +
            $",'{ismappinginspection}'" +
            $",'{ischangepointlot}'" +
            $",'{macgroup}'" +
            $",'{isbadmarkframe}'" +
            $",'{dbthrowdt}'" +
            $",'{reflowtestfg}'" +
            $",'{reflowtestwirebondfg}'" +
            $",'{elasticitytestfg}'" +
            $",'{diesheartestfg}'" +
            $",'{movestockct}'" +
            $",'{beforelifetestcondcd}'" +
            $",'{tempcutblendid}'" +
            $",'{tempcutblendno}'" +
            $",'{mnggrid}'" +
            $",'{mnggrnm}'" +
            $",'{limitsheartestfg}'" +
            $",'{resingpcd2}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);

        }
        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [lotno]" +
                            ",[typecd]" +
                            ",[warningfg]" +
                            ",[restrictfg]" +
                            ",[profileid]" +
                            ",[mnfctplanno]" +
                            ",[blendcd]" +
                            ",[resingpcd]" +
                            ",[cutblendcd]" +
                            ",[lifetestfg]" +
                            ",[khltestfg]" +
                            ",[colortestfg]" +
                            ",[templotno]" +
                            ",[istemp]" +
                            ",[isnascalotcharend]" +
                            ",[lastupddt]" +
                            ",[lotsize]" +
                            ",[isfullsizeinspection]" +
                            ",[ismappinginspection]" +
                            ",[ischangepointlot]" +
                            ",[macgroup]" +
                            ",[isbadmarkframe]" +
                            ",[dbthrowdt]" +
                            ",[reflowtestfg]" +
                            ",[reflowtestwirebondfg]" +
                            ",[elasticitytestfg]" +
                            ",[diesheartestfg]" +
                            ",[movestockct]" +
                            ",[beforelifetestcondcd]" +
                            ",[tempcutblendid]" +
                            ",[tempcutblendno]" +
                            ",[mnggrid]" +
                            ",[mnggrnm]" +
                            ",[limitsheartestfg]" +
                            ",[resingpcd2] " +
                            "FROM[ARMS].[dbo].[TnLot]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["lotno"]+
                            "," + reader["typecd"]+
                            "," + reader["warningfg"]+
                            "," + reader["restrictfg"]+
                            "," + reader["profileid"]+
                            "," + reader["mnfctplanno"]+
                            "," + reader["blendcd"]+
                            "," + reader["resingpcd"]+
                            "," + reader["cutblendcd"]+
                            "," + reader["lifetestfg"]+
                            "," + reader["khltestfg"]+
                            "," + reader["colortestfg"]+
                            "," + reader["templotno"]+
                            "," + reader["istemp"]+
                            "," + reader["isnascalotcharend"]+
                            "," + reader["lastupddt"]+
                            "," + reader["lotsize"]+
                            "," + reader["isfullsizeinspection"]+
                            "," + reader["ismappinginspection"]+
                            "," + reader["ischangepointlot"]+
                            "," + reader["macgroup"]+
                            "," + reader["isbadmarkframe"]+
                            "," + reader["dbthrowdt"]+
                            "," + reader["reflowtestfg"]+
                            "," + reader["reflowtestwirebondfg"]+
                            "," + reader["elasticitytestfg"]+
                            "," + reader["diesheartestfg"]+
                            "," + reader["movestockct"]+
                            "," + reader["beforelifetestcondcd"]+
                            "," + reader["tempcutblendid"]+
                            "," + reader["tempcutblendno"]+
                            "," + reader["mnggrid"]+
                            "," + reader["mnggrnm"]+
                            "," + reader["limitsheartestfg"]+
                            "," + reader["resingpcd2"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }
    public class TnMag
    {
        public string lotno { get; set; }
        public string magno { get; set; }
        public string inlineprocno { get; set; }
        public string newfg { get; set; }
        public string lastupddt { get; set; }

        public bool InsertTnMag(ref string msg)
        {
            DateTime dt = DateTime.Now;
            lastupddt = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO[dbo].[TnMag]" +
                    "([lotno]" +
                    ",[magno]" +
                    ",[inlineprocno]" +
                    ",[newfg]" +
                    ",[lastupddt])" +
                    " VALUES " +
                    $"('{lotno}'" +
                    $",'{magno}'" +
                    $",'{inlineprocno}'" +
                    $",'{newfg}'" +
                    $",'{lastupddt}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }
        public bool UpdateTnMag(string lotno, string magno, string procno, ref string msg)
        {
            var SqlStrings = $"UPDATE [dbo].[TnMag] " +
                            $"SET [inlineprocno] = '{procno}', [newfg] = 1 " +
                            $"WHERE lotno = '{lotno}' and magno = '{magno}'";
            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }
    }
    public class TnTran
    {
        public string lotno { get; set; }
        public string procno { get; set; }
        public string macno { get; set; }
        public string inmag { get; set; }
        public string outmag { get; set; }
        public string startdt { get; set; }
        public string enddt { get; set; }
        public string iscomplt { get; set; }
        public string stocker1 { get; set; }
        public string stocker2 { get; set; }
        public string comment { get; set; }
        public string transtartempcd { get; set; }
        public string trancompempcd { get; set; }
        public string inspectempcd { get; set; }
        public string inspectct { get; set; }
        public string isnascastart { get; set; }
        public string isnascaend { get; set; }
        public string lastupddt { get; set; }
        public string isnascadefectend { get; set; }
        public string isnascacommentend { get; set; }
        public string delfg { get; set; }
        public string isdefectend { get; set; }
        public string isdefectautoimportend { get; set; }
        public string isnascarunning { get; set; }
        public string isautoimport { get; set; }
        public string isresinmixordered { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tntran.csv";

        public bool InsertTnTran(ref string msg)
        {
            DateTime dt = DateTime.Now;
            lastupddt = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO [dbo].[TnTran]" +
                            "([lotno]" +
                            ",[procno]" +
                            ",[macno]" +
                            ",[inmag]" +
                            ",[outmag]" +
                            ",[startdt]" +
                            ",[enddt]" +
                            ",[iscomplt]" +
                            ",[stocker1]" +
                            ",[stocker2]" +
                            ",[comment]" +
                            ",[transtartempcd]" +
                            ",[trancompempcd]" +
                            ",[inspectempcd]" +
                            ",[inspectct]" +
                            ",[isnascastart]" +
                            ",[isnascaend]" +
                            ",[lastupddt]" +
                            ",[isnascadefectend]" +
                            ",[isnascacommentend]" +
                            ",[delfg]" +
                            ",[isdefectend]" +
                            ",[isdefectautoimportend]" +
                            ",[isnascarunning]" +
                            ",[isautoimport]" +
                            ",[isresinmixordered])" +
                        " VALUES " +
                            $"('{lotno}'" +
                            $",'{procno}'" +
                            $",'{macno}'" +
                            $",'{inmag}'" +
                            $",'{outmag}'" +
                            $",'{startdt}'" +
                            $",'{enddt}'" +
                            $",'{iscomplt}'" +
                            $",'{stocker1}'" +
                            $",'{stocker2}'" +
                            $",'{comment}'" +
                            $",'{transtartempcd}'" +
                            $",'{trancompempcd}'" +
                            $",'{inspectempcd}'" +
                            $",'{inspectct}'" +
                            $",'{isnascastart}'" +
                            $",'{isnascaend}'" +
                            $",'{lastupddt}'" +
                            $",'{isnascadefectend}'" +
                            $",'{isnascacommentend}'" +
                            $",'{delfg}'" +
                            $",'{isdefectend}'" +
                            $",'{isdefectautoimportend}'" +
                            $",'{isnascarunning}'" +
                            $",'{isautoimport}'" +
                            $",'{isresinmixordered}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }
        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [lotno] " +
                              ",[procno] " +
                              ",[macno] " +
                              ",[inmag] " +
                              ",[outmag] " +
                              ",[startdt] " +
                              ",[enddt] " +
                              ",[iscomplt] " +
                              ",[stocker1] " +
                              ",[stocker2] " +
                              ",[comment] " +
                              ",[transtartempcd] " +
                              ",[trancompempcd] " +
                              ",[inspectempcd] " +
                              ",[inspectct] " +
                              ",[isnascastart] " +
                              ",[isnascaend] " +
                              ",[lastupddt] " +
                              ",[isnascadefectend] " +
                              ",[isnascacommentend] " +
                              ",[delfg] " +
                              ",[isdefectend] " +
                              ",[isdefectautoimportend] " +
                              ",[isnascarunning] " +
                              ",[isautoimport] " +
                              ",[isresinmixordered] " +
                              "FROM[ARMS].[dbo].[TnTran]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["lotno"] +
                              "," + reader["procno"] +
                              "," + reader["macno"] +
                              "," + reader["inmag"] +
                              "," + reader["outmag"] +
                              "," + reader["startdt"] +
                              "," + reader["enddt"] +
                              "," + reader["iscomplt"] +
                              "," + reader["stocker1"] +
                              "," + reader["stocker2"] +
                              "," + reader["comment"] +
                              "," + reader["transtartempcd"] +
                              "," + reader["trancompempcd"] +
                              "," + reader["inspectempcd"] +
                              "," + reader["inspectct"] +
                              "," + reader["isnascastart"] +
                              "," + reader["isnascaend"] +
                              "," + reader["lastupddt"] +
                              "," + reader["isnascadefectend"] +
                              "," + reader["isnascacommentend"] +
                              "," + reader["delfg"] +
                              "," + reader["isdefectend"] +
                              "," + reader["isdefectautoimportend"] +
                              "," + reader["isnascarunning"] +
                              "," + reader["isautoimport"] +
                              "," + reader["isresinmixordered"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }
    public class TnVirtualMag
    {
        public string macno { get; set; }
        public string locationid { get; set; }
        public string orderid { get; set; }
        public string magno { get; set; }
        public string lastmagno { get; set; }
        public string procno { get; set; }
        public string workstartdt { get; set; }
        public string workenddt { get; set; }
        public string nextmachines { get; set; }
        public string relatedmaterials { get; set; }
        public string framematerialcd { get; set; }
        public string framelotno { get; set; }
        public string framecurrentct { get; set; }
        public string framemaxct { get; set; }
        public string stockerstartno { get; set; }
        public string stockerendno { get; set; }
        public string stockerchangect { get; set; }
        public string stockerlastupddt { get; set; }
        public string stocker1no { get; set; }
        public string stocker2no { get; set; }
        public string waferstartno { get; set; }
        public string waferendno { get; set; }
        public string waferchangect { get; set; }
        public string waferlastupddt { get; set; }
        public string mapaoimaglotno { get; set; }
        public string purgereason { get; set; }
        public string originno { get; set; }
        public string programtotalminutes { get; set; }
        public string lastupddt { get; set; }
        public string priorityfg { get; set; }
        public string originlocationid { get; set; }

        public bool InsertVirtualMag(ref string msg)
        {
            DateTime dt = DateTime.Now;
            lastupddt = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO [dbo].[TnVirtualMag] " +
                           "([macno] " +
                           ",[locationid] " +
                           ",[orderid] " +
                           ",[magno] " +
                           ",[lastmagno] " +
                           ",[procno] " +
                           ",[workstartdt] " +
                           ",[workenddt] " +
                           ",[nextmachines] " +
                           ",[relatedmaterials] " +
                           ",[framematerialcd] " +
                           ",[framelotno] " +
                           ",[framecurrentct] " +
                           ",[framemaxct] " +
                           ",[stockerstartno] " +
                           ",[stockerendno] " +
                           ",[stockerchangect] " +
                           ",[stockerlastupddt] " +
                           ",[stocker1no] " +
                           ",[stocker2no] " +
                           ",[waferstartno] " +
                           ",[waferendno] " +
                           ",[waferchangect] " +
                           ",[waferlastupddt] " +
                           ",[mapaoimaglotno] " +
                           ",[purgereason] " +
                           ",[originno] " +
                           ",[programtotalminutes] " +
                           ",[lastupddt] " +
                           ",[priorityfg] " +
                           ",[originlocationid]) " +
                     " VALUES  " +
                           $"('{macno}' " +
                           $",'{locationid}' " +
                           $",'{orderid}' " +
                           $",'{magno}'" +
                           $",'{lastmagno}' " +
                           $",'{procno}'" +
                           $",'{workstartdt}' " +
                           $",'{workenddt}' " +
                           $",'{nextmachines}' " +
                           $",'{relatedmaterials}' " +
                           $",'{framematerialcd}'" +
                           $",'{framelotno}'" +
                           $",'{framecurrentct}'" +
                           $",'{framemaxct}'" +
                           $",'{stockerstartno}'" +
                           $",'{stockerendno}'" +
                           $",'{stockerchangect}' " +
                           $",'{stockerlastupddt}' " +
                           $",'{stocker1no}'" +
                           $",'{stocker2no}' " +
                           $",'{waferstartno}' " +
                           $",'{waferendno}'" +
                           $",'{waferchangect}' " +
                           $",'{waferlastupddt}' " +
                           $",'{mapaoimaglotno}'" +
                           $",'{purgereason}'" +
                           $",'{originno}'" +
                           $",'{programtotalminutes}' " +
                           $",'{lastupddt}'" +
                           $",'{priorityfg}' " +
                           $",'{originlocationid}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }
    }

    public class TnLotLog
    {
        public string lotno { get; set; }
        public string indt { get; set; }
        public string msg { get; set; }
        public string magno { get; set; }
        public string errorfg { get; set; }
        public string line { get; set; }
        public string updusercd { get; set; }
        public string plantcd { get; set; }
        public string moniterviewfg { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnlotlog.csv";

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [lotno] " +
                              ",[indt] " +
                              ",[msg] " +
                              ",[magno] " +
                              ",[errorfg] " +
                              ",[line] " +
                              ",[updusercd] " +
                              ",[plantcd] " +
                              ",[moniterviewfg] " +
                              "FROM[ARMS].[dbo].[TnLotLog]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["lotno"] +
                              "," + reader["indt"] +
                              "," + reader["msg"] +
                              "," + reader["magno"] +
                              "," + reader["errorfg"] +
                              "," + reader["line"] +
                              "," + reader["updusercd"] +
                              "," + reader["plantcd"] +
                              "," + reader["moniterviewfg"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }

    public class TnRestrict
    {
        public string lotno { get; set; }
        public string procno { get; set; }
        public string reason { get; set; }
        public string delfg { get; set; }
        public string lastupddt { get; set; }
        public string reasonkb { get; set; }
        public string updusercd { get; set; }
        public string restrictreleasefg { get; set; }
        public string representativelotno { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnrestrict.csv";

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [lotno] " +
                              ",[procno] " +
                              ",[reason] " +
                              ",[delfg] " +
                              ",[lastupddt] " +
                              ",[reasonkb] " +
                              ",[updusercd] " +
                              ",[restrictreleasefg] " +
                              ",[representativelotno] " +
                              "FROM[ARMS].[dbo].[TnRestrict]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["lotno"] +
                              "," + reader["procno"] +
                              "," + reader["reason"] +
                              "," + reader["delfg"] +
                              "," + reader["lastupddt"] +
                              "," + reader["reasonkb"] +
                              "," + reader["updusercd"] +
                              "," + reader["restrictreleasefg"] +
                              "," + reader["representativelotno"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }
    public class TnInspection
    {
        public string lotno { get; set; }
        public string procno { get; set; } = "10";
        public string sgyfg { get; set; } = "0";
        public string lastupddt { get; set; }

        public bool InsertTnMag(ref string msg)
        {
            DateTime dt = DateTime.Now;
            lastupddt = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO[dbo].[TnInspection]" +
                    "([lotno]" +
                    ",[procno]" +
                    ",[sgyfg]" +
                    ",[lastupddt])" +
                    " VALUES " +
                    $"('{lotno}'" +
                    $",'{procno}'" +
                    $",'{sgyfg}'" +
                    $",'{lastupddt}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand(SqlStrings, ref msg);
        }
    }

    //
    // LENS DB
    //
    public class TnLot_Lens
    {
        public string LotNO { get; set; }
        public string TypeCD { get; set; }
        public string IsFullsizeInspection { get; set; } = "0";
        public string IsMappingInspection { get; set; } = "1";
        public string IsChangePoint { get; set; } = "0";
        public string LastupdDT { get; set; }
        public string InspectionResultCD { get; set; } = "0";

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnlot_lens.csv";

        public bool InsertTnlot(ref string msg)
        {
            DateTime dt = DateTime.Now;
            LastupdDT = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO [dbo].[TnLot] " +
                           "([LotNO] " +
                           ",[TypeCD] " +
                           ",[IsFullsizeInspection] " +
                           ",[IsMappingInspection] " +
                           ",[IsChangePoint] " +
                           ",[LastupdDT] " +
                           ",[InspectionResultCD]) " +
                        " VALUES " +
                           $"('{LotNO}' " +
                           $",'{TypeCD}' " +
                           $",'{IsFullsizeInspection}' " +
                           $",'{IsMappingInspection}' " +
                           $",'{IsChangePoint}' " +
                           $",'{LastupdDT}'" +
                           $",'{InspectionResultCD}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand_Lens(SqlStrings, ref msg);
        }
        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [lotno] " +
                              ",[TypeCD] " +
                              ",[IsFullsizeInspection] " +
                              ",[IsMappingInspection] " +
                              ",[IsChangePoint] " +
                              ",[LastupdDT] " +
                              ",[InspectionResultCD] " +
                              "FROM[LENS].[dbo].[TnLot]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LENSConSTR))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["lotno"] +
                              "," + reader["TypeCD"] +
                              "," + reader["IsFullsizeInspection"] +
                              "," + reader["IsMappingInspection"] +
                              "," + reader["IsChangePoint"] +
                              "," + reader["LastupdDT"] +
                              "," + reader["InspectionResultCD"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }
    public class TnTran_Lens
    {
        public string LotNO { get; set; }
        public string ProcNO { get; set; }
        public string PlantCD { get; set; }
        public string StartDT { get; set; }
        public string EndDT { get; set; }
        public string IsCompleted { get; set; }
        public string DelFG { get; set; } = "0";
        public string LastUpdDT { get; set; }
        public string CarrierNO { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tntran_lens.csv";

        public bool InsertTnTran(ref string msg)
        {
            DateTime dt = DateTime.Now;
            LastUpdDT = dt.ToString("yyyy/MM/dd HH:mm:ss");
            var SqlStrings = "INSERT INTO [dbo].[TnTran]" +
                          " ([LotNO]" +
                          " ,[ProcNO]" +
                          " ,[PlantCD]" +
                          " ,[StartDT]" +
                          " ,[EndDT]" +
                          " ,[IsCompleted]" +
                          " ,[DelFG]" +
                          " ,[LastUpdDT]" +
                          " ,[CarrierNO])" +
                          " VALUES" +
                          $" ('{LotNO}'" +
                          $" ,'{ProcNO}'" +
                          $" ,'{PlantCD}'" +
                          $" ,'{StartDT}'" +
                          $" ,'{EndDT}'" +
                          $" ,'{IsCompleted}'" +
                          $" ,'{DelFG}'" +
                          $" ,'{LastUpdDT}'" +
                          $" ,'{CarrierNO}')";

            var dbcnt = new DbControls();
            return dbcnt.execSqlCommand_Lens(SqlStrings, ref msg);
        }
        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [LotNO] " +
                              ",[ProcNO] " +
                              ",[PlantCD] " +
                              ",[StartDT] " +
                              ",[EndDT] " +
                              ",[IsCompleted] " +
                              ",[DelFG] " +
                              ",[LastUpdDT] " +
                              ",[CarrierNO] " +
                              "FROM[LENS].[dbo].[TnTran]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.LENSConSTR))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["LotNO"] +
                              "," + reader["ProcNO"] +
                              "," + reader["PlantCD"] +
                              "," + reader["StartDT"] +
                              "," + reader["EndDT"] +
                              "," + reader["IsCompleted"] +
                              "," + reader["DelFG"] +
                              "," + reader["LastUpdDT"] +
                              "," + reader["CarrierNO"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }

    //
    // QCIL DB
    //
    public class TnLog_Qcil
    {
        public string Inline_CD { get; set; }
        public string Equipment_NO { get; set; }
        public string Measure_DT { get; set; }
        public string Seq_NO { get; set; }
        public string QcParam_NO { get; set; }
        public string Material_CD { get; set; }
        public string Magazine_NO { get; set; }
        public string NascaLot_NO { get; set; }
        public string DParameter_VAL { get; set; }
        public string SParameter_VAL { get; set; }
        public string Message_NM { get; set; }
        public string Error_FG { get; set; }
        public string Check_FG { get; set; }
        public string Del_FG { get; set; }
        public string UpdUser_CD { get; set; }
        public string LastUpd_DT { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnlog_qcil.csv";

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [Inline_CD] " +
                              ",[Equipment_NO] " +
                              ",[Measure_DT] " +
                              ",[Seq_NO] " +
                              ",[QcParam_NO] " +
                              ",[Material_CD] " +
                              ",[Magazine_NO] " +
                              ",[NascaLot_NO] " +
                              ",[DParameter_VAL] " +
                              ",[SParameter_VAL] " +
                              ",[Message_NM] " +
                              ",[Error_FG] " +
                              ",[Check_FG] " +
                              ",[Del_FG] " +
                              ",[UpdUser_CD] " +
                              ",[LastUpd_DT] " +
                              "FROM[QCIL].[dbo].[TnLOG]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.QCILConSTR))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["Inline_CD"] +
                              "," + reader["Equipment_NO"] +
                              "," + reader["Measure_DT"] +
                              "," + reader["Seq_NO"] +
                              "," + reader["QcParam_NO"] +
                              "," + reader["Material_CD"] +
                              "," + reader["Magazine_NO"] +
                              "," + reader["NascaLot_NO"] +
                              "," + reader["DParameter_VAL"] +
                              "," + reader["SParameter_VAL"] +
                              "," + reader["Message_NM"] +
                              "," + reader["Error_FG"] +
                              "," + reader["Check_FG"] +
                              "," + reader["Del_FG"] +
                              "," + reader["UpdUser_CD"] +
                              "," + reader["LastUpd_DT"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }
    }

    public class TnLott_Qcil
    {
        public string Inline_CD { get; set; }
        public string NascaLot_NO { get; set; }
        public string Equipment_NO { get; set; }
        public string Measure_DT { get; set; }
        public string Assets_NM { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnlott_qcil.csv";

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [Inline_CD] " +
                                ",[NascaLot_NO] " +
                                ",[Equipment_NO] " +
                                ",[Measure_DT] " +
                                ",[Assets_NM] " +
                                "FROM[QCIL].[dbo].[TnLOTT]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.QCILConSTR))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["Inline_CD"] +
                                "," + reader["NascaLot_NO"] +
                                "," + reader["Equipment_NO"] +
                                "," + reader["Measure_DT"] +
                                "," + reader["Assets_NM"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }

    }

    public class TnQcnr_Qcil
    {
        public string QCNR_NO { get; set; }
        public string Inline_CD { get; set; }
        public string Equipment_NO { get; set; }
        public string Defect_NO { get; set; }
        public string Inspection_NO { get; set; }
        public string Multi_NO { get; set; }
        public string NascaLot_NO { get; set; }
        public string Measure_DT { get; set; }
        public string Message { get; set; }
        public string Type_CD { get; set; }
        public string BackNum_NO { get; set; }
        public string Check_NO { get; set; }
        public string UpdUser_CD { get; set; }
        public string LastUpd_DT { get; set; }

        public string historyfile { get; set; } = @"C:\ARMS\DBhistory\tnqcnr_qcil.csv";

        public bool RecordTrData(string header, ref string msg)
        {
            var sqlstring = "SELECT [QCNR_NO] " +
                                ",[Inline_CD] " +
                                ",[Equipment_NO] " +
                                ",[Defect_NO] " +
                                ",[Inspection_NO] " +
                                ",[Multi_NO] " +
                                ",[NascaLot_NO] " +
                                ",[Measure_DT] " +
                                ",[Message] " +
                                ",[Type_CD] " +
                                ",[BackNum_NO] " +
                                ",[Check_NO] " +
                                ",[UpdUser_CD] " +
                                ",[LastUpd_DT] " +
                                "FROM[QCIL].[dbo].[TnLOG]";
            try
            {
                using (SqlConnection conn = new SqlConnection(Config.Settings.QCILConSTR))
                using (SqlCommand cmd = new SqlCommand(sqlstring))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        File.AppendAllText(historyfile, header + "\r\n", System.Text.Encoding.GetEncoding("Shift-Jis"));
                        while (reader.Read())
                        {
                            var ret = reader["QCNR_NO"] +
                                "," + reader["Inline_CD"] +
                                "," + reader["Equipment_NO"] +
                                "," + reader["Defect_NO"] +
                                "," + reader["Inspection_NO"] +
                                "," + reader["Multi_NO"] +
                                "," + reader["NascaLot_NO"] +
                                "," + reader["Measure_DT"] +
                                "," + reader["Message"] +
                                "," + reader["Type_CD"] +
                                "," + reader["BackNum_NO"] +
                                "," + reader["Check_NO"] +
                                "," + reader["UpdUser_CD"] +
                                "," + reader["LastUpd_DT"] + "\r\n";
                            File.AppendAllText(historyfile, ret);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            return true;
        }

    }


    //
    // 共通DBクラス
    //
    public class DbControls
    {
        public bool execSqlCommand(string SqlStrings, ref string msg)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = SqlStrings;
                    con.Open();
                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();
                    con.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        public bool execSqlCommand_Lens(string SqlStrings, ref string msg)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = SqlStrings;
                    con.Open();
                    string Result = (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();
                    con.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        //
        /* テキストファイル一行づつ読み込み関数
         * OK: true
         * NG: false
         * 読込内容(配列)：contents 
         */
        //
        public bool ReadTextFileLine(string filepath, ref List<string> contents, string enccode = "UTF-8")
        {
            try
            {
                StreamReader sr = new StreamReader(filepath, Encoding.GetEncoding(enccode));
                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();
                    line = new string(line.Where(c => !char.IsControl(c)).ToArray());
                    contents.Add(line);
                }
                sr.Close();

                contents.RemoveAt(0);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
    
}
