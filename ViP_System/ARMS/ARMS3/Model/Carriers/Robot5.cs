using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Carriers
{
    /// <summary>
    /// 搬送ロボット(6軸)  PLC:Keyence  ※Jobショップ用(搬送先の各装置が個別PLC) 1リニア上での2台目
    /// </summary>
    public class Robot5 : Robot3
    {
        #region PLCアドレス

        /// <summary>
        /// 
        /// </summary>
        public override string PC_READY() { return "B0010FF"; }

        /// <summary>
        /// ロボット移動要求1
        /// </summary>
        public override string REQ_ROBOT_MOVE_ORG2MID() { return "B0010F0"; }

        /// <summary>
        /// ロボット移動2　リニア移動後の最終チェック異常
        /// </summary>
        public override string REQ_ROBOT_MOVE2_NASCA_ERROR() { return "B0010F7"; }

        /// <summary>
        /// ロボットBusy信号1
        /// 終了装置→QR読込
        /// </summary>
        public override string STAT_ROBOT_BUSY1() { return "B0010E6"; }

        /// <summary>
        /// ロボットBusy信号2
        /// QR読込→開始装置
        /// </summary>
        public override string STAT_ROBOT_BUSY2() { return "B0010E7"; }

        /// <summary>
        /// QR読取り部マガジン有
        /// </summary>
        public override string BIT_QR_HAS_MAGAZINE() { return "B0010E3"; }

        /// <summary>
        /// FROM設定アドレス (装置番号)
        /// </summary>
        public override string FROM_MAC_WORD_ADDRESS() { return "W0010F0"; }

        /// <summary>
        /// FROM設定アドレス (位置番号)
        /// </summary>
        public override string FROM_LOCATION_WORD_ADDRESS() { return "W0010F1"; }

        /// <summary>
        /// FROM設定アドレス (バッファ位置番号)
        /// </summary>
        public override string FROM_BUFFERLOCATION_WORD_ADDRESS() { return "W0010F2"; }

        /// <summary>
        /// TO設定アドレス (装置番号)
        /// </summary>
        public override string TO_MAC_WORD_ADDRESS() { return "W0010F3"; }

        /// <summary>
        /// TO設定アドレス (位置番号)
        /// </summary>
        public override string TO_LOCATION_WORD_ADDRESS() { return "W0010F4"; }

        /// <summary>
        /// TO設定アドレス (バッファ位置番号)
        /// </summary>
        public override string TO_BUFFERLOCATION_WORD_ADDRESS() { return "W0010F5"; }

        /// <summary>
        /// ロボット移動指令許可
        /// </summary>
        public override string STAT_ROBOT_COMMAND_READY() { return "B0010E0"; }

        /// <summary>
        /// ロボットがマガジンを掴んでいるか判定アドレス
        /// </summary>
        public override string MAGAZINE_EXIST_ROBOT_ARM() { return "B0010EE"; }

        /// <summary>
        /// QR読取部にマガジンがあるかの判定アドレス
        /// </summary>
        public override string MAGAZINE_EXIST_QR_STAGE1() { return "B0010EF"; }

        /// <summary>
        /// QR読取部にマガジンがあるかの判定アドレス
        /// </summary>
        public override string MAGAZINE_EXIST_QR_STAGE2() { return "B0010ED"; }

        /// <summary>
        /// 要求失敗、リトライOK
        /// ロボットのステータス変化
        /// </summary>
        public override string STAT_MOVE_FAULT() { return "B0010E1"; }

        /// <summary>
        /// 操作要求の失敗、緊急停止
        /// チャックミス、QR読取エラー
        /// </summary>
        public override string STAT_MOVE_FAULT_EMERGENCY() { return "B0010E2"; }

        /// <summary>
        /// ロボット移動要求2
        /// </summary>
        public override string REQ_ROBOT_MOVE2_MID2DST() { return "B0010F6"; }

        /// <summary>
        /// ロボット移動2　リニア移動完了
        /// </summary>
        public override string ROBOT_MOVE2_LINER_COMPLETE() { return "B0010E5"; }

        /// <summary>
        /// ロボット移動2　リニア移動後の最終チェック通過→マガジン投入
        /// </summary>
        public override string REQ_ROBOT_MOVE2_COMPLETE() { return "B0010F5"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ 連動運転中
        /// </summary>
        public override string SEND_MACHINE_READY() { return "B001000"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ ﾛﾎﾞｯﾄﾁｬｯｸ開中
        /// </summary>
        public override string DOING_OPEN_ROBOT_ARMS() { return "B001002"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ ﾛﾎﾞｯﾄﾁｬｯｸ閉中
        /// </summary>
        public override string DOING_CLOSE_ROBOT_ARMS() { return "B001003"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ Mag供給中
        /// </summary>
        public override string DOING_LOAD_MAGAZINE() { return "B001004"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ Mag排出中
        /// </summary>
        public override string DOING_UNLOAD_MAGAZINE() { return "B001005"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ ﾘﾆｱ停止中
        /// </summary>
        public override string ROBOT_LINER_STOPPING() { return "B001008"; }

        /// <summary>
        /// PC→搬送ﾘﾆｱ移動可能
        /// </summary>
        public override string CAN_MOVE_LINER() { return "B001043"; }

        /// <summary>
        /// PC→Mag供給要求
        /// </summary>
        public override string REQ_LOAD_MAGAZINE() { return "B001044"; }

        /// <summary>
        /// PC→Mag排出要求
        /// </summary>
        public override string REQ_UNLOAD_MAGAZINE() { return "B001045"; }

        /// <summary>
        /// PC→Mag供給・排出可能
        /// </summary>
        public override string CAN_LOAD_UNLOAD_MAGAZINE() { return "B001046"; }

        /// <summary>
        /// PC→Mag受渡し完了
        /// </summary>
        public override string COMPLT_DESTINATION_MAGAZINE() { return "B001047"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ待機位置 B001007
        /// </summary>
        public override string ROBOT_START_POSITION() { return "B001007"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ 自動搬送ﾗｲﾝ安全ﾄﾞｱ閉 B001009
        /// </summary>
        public override string ROBOT_SAFETY_DOOR_CLOSE() { return "B001009"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ ﾘﾆｱ移動完了 B00100A
        /// </summary>
        public override string ROBOT_LINER_MOVE_COMPLETE() { return "B00100A"; }


        #endregion

        #region コンストラクタ

        public Robot5(string plcAddress, int plcPort, int carNo) : base(plcAddress, plcPort, carNo)
        {

        }

        #endregion
    }
}
