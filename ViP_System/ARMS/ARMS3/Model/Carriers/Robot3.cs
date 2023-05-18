using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.Machines;
using ARMS3.Model.PLC;
using ArmsApi.Model;
using System.Threading;
using ArmsApi;
using System.Windows.Forms;

namespace ARMS3.Model.Carriers
{
    /// <summary>
    /// 搬送ロボット(6軸)  PLC:Keyence  ※Jobショップ用(搬送先の各装置が個別PLC)
    /// </summary>
    public class Robot3 : CarrierBase
    {
        #region メンバ・プロパティ

        /// <summary>
        /// QR読み込み位置 (装置番号)
        /// </summary>
        public string QRReaderMacPoint { get; set; }

        /// <summary>
        /// QR読み込み位置 (位置番号)
        /// </summary>
        public string QRReaderLocationPoint { get; set; }

        /// <summary>
        /// ロボットQR読み込み結果アドレス先頭
        /// </summary>
        public string QRWordAddressStart { get; set; }

        /// <summary>
        /// ロボットQR読込部のWORDアドレス長
        /// 原材料QR読込用に他と長さが違う
        /// </summary>
        public int QRWordAddressLength { get; set; }

        /// <summary>
        /// 自身が届くオーブン(PLC)リスト (オーブン全扉閉チェック用)
        /// </summary>
        protected List<Oven> reachOvens { get; set; }

        /// <summary>
        /// 自身が届く遠心沈降自動機リスト
        /// </summary>
        protected List<ECK4MagExchanger> reachECK4MagExchangers { get; set; }

        /// <summary>
        /// 保持しているマガジン参照アドレスリスト
        /// </summary>
        public List<string> HoldMagazineStartAddressList { get; set; }

        /// <summary>
        /// QRリーダー部のマガジン有無の信号
        /// </summary>
        public SortedList<int, string> MagazineExistQrStageAddressList { get; set; }

        /// <summary>
        /// 保持しているマガジン
        /// </summary>
        public override List<VirtualMag> HoldingMagazines
        {
            get
            {
                List<VirtualMag> retv = new List<VirtualMag>();

                for (int i = 0; i < HoldMagazineStartAddressList.Count; i++)
                {
                    string magNo = this.Plc.GetMagazineNo(HoldMagazineStartAddressList[i], QRWordAddressLength);

                    if (string.IsNullOrEmpty(magNo) == false)
                    {
                        VirtualMag vMag = new VirtualMag();
                        vMag.MagazineNo = magNo;

                        // orderidは投入順番ではなく、マガジンバッファ位置として使用する
                        vMag.orderid = i + 1;
                        retv.Add(vMag);
                    }
                }

                return retv;
            }
        }

        /// <summary>
        /// 搬送時のPLC信号流し込みタスクの停止要求  (TRUE: 停止要求有)
        /// </summary>
        protected bool taskStopRequested { get; set; }

        /// <summary>
        /// 搬送時のPLC信号流し込みタスクの実行状態  (TRUE: 実行中, FALSE: 停止)
        /// </summary>
        protected bool isTaskRunning { get; set; }

        /// <summary>
        /// 最大積載数
        /// </summary>
        protected int HOLDMAGAZINE_MAX_CT = 1;

        object robotMoveLock = new object();

        #endregion

        #region PLCアドレス

        /// <summary>
        /// B0000FF
        /// </summary>
        public virtual string PC_READY() { return "B0000FF"; }

        /// <summary>
        /// ロボット移動要求1 B0000F0
        /// </summary>
        public virtual string REQ_ROBOT_MOVE_ORG2MID() { return "B0000F0"; }

        /// <summary>
        /// ロボット移動2　リニア移動後の最終チェック異常 B0000F7
        /// </summary>
        public virtual string REQ_ROBOT_MOVE2_NASCA_ERROR() { return "B0000F7"; }

        /// <summary>
        /// ロボットBusy信号1 B0000E6
        /// 終了装置→QR読込
        /// </summary>
        public virtual string STAT_ROBOT_BUSY1() { return "B0000E6"; }

        /// <summary>
        /// ロボットBusy信号2 B0000E7
        /// QR読込→開始装置
        /// </summary>
        public virtual string STAT_ROBOT_BUSY2() { return "B0000E7"; }

        /// <summary>
        /// QR読取り部マガジン有 B0000E3
        /// </summary>
        public virtual string BIT_QR_HAS_MAGAZINE() { return "B0000E3"; }

        /// <summary>
        /// FROM設定アドレス (装置番号) W0000F0
        /// </summary>
        public virtual string FROM_MAC_WORD_ADDRESS() { return "W0000F0"; }

        /// <summary>
        /// FROM設定アドレス (位置番号) W0000F1
        /// </summary>
        public virtual string FROM_LOCATION_WORD_ADDRESS() { return "W0000F1"; }

        /// <summary>
        /// FROM設定アドレス (バッファ位置番号) W0000F2
        /// </summary>
        public virtual string FROM_BUFFERLOCATION_WORD_ADDRESS() { return "W0000F2"; }

        /// <summary>
        /// TO設定アドレス (装置番号) W0000F3
        /// </summary>
        public virtual string TO_MAC_WORD_ADDRESS() { return "W0000F3"; }

        /// <summary>
        /// TO設定アドレス (位置番号) W0000F4
        /// </summary>
        public virtual string TO_LOCATION_WORD_ADDRESS() { return "W0000F4"; }

        /// <summary>
        /// TO設定アドレス (バッファ位置番号) W0000F5
        /// </summary>
        public virtual string TO_BUFFERLOCATION_WORD_ADDRESS() { return "W0000F5"; }

        /// <summary>
        /// ロボット移動指令許可 B0000E0
        /// </summary>
        public virtual string STAT_ROBOT_COMMAND_READY() { return "B0000E0"; }

        /// <summary>
        /// ロボットがマガジンを掴んでいるか判定アドレス B0000EE
        /// </summary>
        public virtual string MAGAZINE_EXIST_ROBOT_ARM() { return "B0000EE"; }

        /// <summary>
        /// QR読取部にマガジンがあるかの判定アドレス B0000EF
        /// </summary>
        public virtual string MAGAZINE_EXIST_QR_STAGE1() { return "B0000EF"; }

        /// <summary>
        /// QR読取部にマガジンがあるかの判定アドレス B0000ED
        /// </summary>
        public virtual string MAGAZINE_EXIST_QR_STAGE2() { return "B0000ED"; }

        /// <summary>
        /// 要求失敗、リトライOK B0000E1
        /// ロボットのステータス変化
        /// </summary>
        public virtual string STAT_MOVE_FAULT() { return "B0000E1"; }

        /// <summary>
        /// 操作要求の失敗、緊急停止 B0000E2
        /// チャックミス、QR読取エラー
        /// </summary>
        public virtual string STAT_MOVE_FAULT_EMERGENCY() { return "B0000E2"; }

        /// <summary>
        /// ロボット移動要求2 B0000F6
        /// </summary>
        public virtual string REQ_ROBOT_MOVE2_MID2DST() { return "B0000F6"; }

        /// <summary>
        /// ロボット移動2　リニア移動完了 B0000E5
        /// </summary>
        public virtual string ROBOT_MOVE2_LINER_COMPLETE() { return "B0000E5"; }

        /// <summary>
        /// ロボット移動2　リニア移動後の最終チェック通過→マガジン投入 B0000F5
        /// </summary>
        public virtual string REQ_ROBOT_MOVE2_COMPLETE() { return "B0000F5"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ 連動運転中 B000000
        /// </summary>
        public virtual string SEND_MACHINE_READY() { return "B000000"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ ﾛﾎﾞｯﾄﾁｬｯｸ開中 B000002
        /// </summary>
        public virtual string DOING_OPEN_ROBOT_ARMS() { return "B000002"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ ﾛﾎﾞｯﾄﾁｬｯｸ閉中 B000003
        /// </summary>
        public virtual string DOING_CLOSE_ROBOT_ARMS() { return "B000003"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ Mag供給中 B000004
        /// </summary>
        public virtual string DOING_LOAD_MAGAZINE() { return "B000004"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ Mag排出中 B000005
        /// </summary>
        public virtual string DOING_UNLOAD_MAGAZINE() { return "B000005"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ ﾘﾆｱ停止中 B000008
        /// </summary>
        public virtual string ROBOT_LINER_STOPPING() { return "B000008"; }

        /// <summary>
        /// PC→搬送ﾘﾆｱ移動可能 B000043
        /// </summary>
        public virtual string CAN_MOVE_LINER() { return "B000043"; }

        /// <summary>
        /// PC→Mag供給要求 B000044
        /// </summary>
        public virtual string REQ_LOAD_MAGAZINE() { return "B000044"; }

        /// <summary>
        /// PC→Mag排出要求 B000045
        /// </summary>
        public virtual string REQ_UNLOAD_MAGAZINE() { return "B000045"; }

        /// <summary>
        /// PC→Mag供給・排出可能 B000046
        /// </summary>
        public virtual string CAN_LOAD_UNLOAD_MAGAZINE() { return "B000046"; }

        /// <summary>
        /// PC→Mag受渡し完了 B000047
        /// </summary>
        public virtual string COMPLT_DESTINATION_MAGAZINE() { return "B000047"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ待機位置 B000007
        /// </summary>
        public virtual string ROBOT_START_POSITION() { return "B000007"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ 自動搬送ﾗｲﾝ安全ﾄﾞｱ閉 B000009
        /// </summary>
        public virtual string ROBOT_SAFETY_DOOR_CLOSE() { return "B000009"; }

        /// <summary>
        /// ﾛﾎﾞｯﾄ ﾘﾆｱ移動完了 B00000A
        /// </summary>
        public virtual string ROBOT_LINER_MOVE_COMPLETE() { return "B00000A"; }

        #endregion

        #region コンストラクタ

        public Robot3(string plcAddress, int plcPort, int carNo)
        {
            this.Plc = new Mitsubishi(plcAddress, plcPort);

            //上位リンクモード
            this.Plc.SetBit(PC_READY(), 1, Mitsubishi.BIT_ON);

            carrierMutex = new Mutex(false, string.Format("carrierMutex{0}", carNo));

            //自身が手が届く装置リストを取得
            reachMachines = new List<IMachine>();
            reachECK4MagExchangers = new List<ECK4MagExchanger>();
            List<int> machines = Route.GetMachines(carNo);
            List<Oven> ovens = new List<Oven>();
            foreach (IMachine machine in LineKeeper.Machines)
            {
                if (machines.Exists(m => m == machine.MacNo))
                {
                    reachMachines.Add(machine);

                    if (LineKeeper.GetMachine(machine.MacNo) is Oven)
                    {
                        Oven ovenMac = (Oven)LineKeeper.GetMachine(machine.MacNo);
                        ovens.Add(ovenMac);
                    }
                    // 装置リスト内から遠心沈降自動機用のリストを作成(装置単位)
                    if (LineKeeper.GetMachine(machine.MacNo) is ECK4MagExchanger)
                    {
                        ECK4MagExchanger eck4MagExchanger = LineKeeper.GetMachine(machine.MacNo) as ECK4MagExchanger;
                        reachECK4MagExchangers.Add(eck4MagExchanger);
                    }
                }
            }

            // 装置リスト内からオーブン用のリストを作成(オーブン制御盤 = PLC単位)
            reachOvens = new List<Oven>();
            List<Oven> overLapOvenPlcList = new List<Oven>();
            foreach (Oven oven in ovens)
            {
                List<Oven> samePlcOvenList = reachOvens.Where(r => r.Plc.IPAddress == oven.Plc.IPAddress && r.Plc.Port == oven.Plc.Port).ToList();
                if (samePlcOvenList.Count > 0)
                {
                    if (samePlcOvenList.Exists(o => o.OvenAllDoorCloseBitAddress == oven.OvenAllDoorCloseBitAddress) == true)
                    {
                        // 同じPLC・ポート・オーブン全扉閉BITアドレスが重複する装置はスキップ
                        continue;
                    }
                    //// アドレス領域 (OVN17,18)の追加開放の為、重複チェックを無効にする
                    //else if (overLapOvenPlcList.Exists(o => o.Plc.IPAddress == oven.Plc.IPAddress && o.Plc.Port == oven.Plc.Port) == false)
                    //{
                    //    //// PLC・ポートが同じでBITアドレスが違う装置がある場合、最後に装置リストをSysLogに表示
                    //    //overLapOvenPlcList.Add(oven);
                    //}
                }
                reachOvens.Add(oven);
            }
            // PLC・ポートが同じでBITアドレスが違う装置のリストを警告としてSysLogに表示
            if (overLapOvenPlcList.Count > 0)
            {
                string warningMessage = string.Empty;
                warningMessage += $"【警告】lineconfig.xml上の設定値を見直してください。同じPLCで「OvenAllDoorCloseBitAddress」タグの設定値が違います。";
                foreach (Oven overLapOvenPlc in overLapOvenPlcList)
                {
                    warningMessage += $"\r\n PLCアドレス：{overLapOvenPlc.Plc.IPAddress}, ポート：{overLapOvenPlc.Plc.Port}";
                    List<Oven> overLapOvenList = ovens
                                                    .Where(o => o.Plc.IPAddress == overLapOvenPlc.Plc.IPAddress && o.Plc.Port == overLapOvenPlc.Plc.Port)
                                                    .OrderBy(o => o.OvenAllDoorCloseBitAddress).OrderBy(o => o.MacNo).ToList();
                    foreach (Oven oven in overLapOvenList)
                    {
                        warningMessage += $"\r\n   装置番号={oven.MacNo},号機={oven.Name},設定値={oven.OvenAllDoorCloseBitAddress}";
                    }
                }
                Log.SysLog.Info(warningMessage);
            }

            this.HoldingMagazines = new List<VirtualMag>();
            this.HoldMagazineStartAddressList = new List<string>();

            this.MagazineExistQrStageAddressList = new SortedList<int, string>();
            this.MagazineExistQrStageAddressList.Add(1, MAGAZINE_EXIST_QR_STAGE1());
            this.MagazineExistQrStageAddressList.Add(2, MAGAZINE_EXIST_QR_STAGE2());
            this.isTaskRunning = false;
            this.taskStopRequested = false;
        }

        #endregion

        protected override void concreteThreadWork()
        {
            #region メインスレッド

            //Log.RBLog.Info("[監視周期開始]");

            try
            {
                bool canMove = false;
                Location moveFrom = null;
                Location moveTo = null;

                List<IMachine> machines = reachMachines;

                // 各装置に搬送ﾛﾎﾞｯﾄの連動運転中信号を送信
                string bitData = this.Plc.GetBit(SEND_MACHINE_READY());
                SendReadyBitToReachMachines(machines, bitData);

                #region オーブン全扉閉信号の通信処理 (どれか1つでもオーブンの全扉閉がOFFなら搬送PLCにOFFを渡す)
                bitData = PLC.Common.BIT_ON;
                foreach (Oven oven in reachOvens)
                {
                    if (oven.GetOvenAllDoorCloseBitData() == PLC.Common.BIT_OFF)
                    {
                        bitData = PLC.Common.BIT_OFF;
                        break;
                    }
                }
                this.Plc.SetBit(CAN_MOVE_LINER(), 1, bitData);
                #endregion

                // 各遠心沈降機に必要な情報を通信する
                sendReachECK4Exchangers(null);

                // 連結ラインの場合、お互いの搬送RB処理でロボット処理を二重に行ってしまう為、
                // ここにロックをかける。
                lock (this.robotMoveLock)
                {
                    // 積載マガジンを搬送先へ
                    List<VirtualMag> qrMagList = LineKeeper.GetMachine(QRReaderMacNo).GetMagazines(Station.Unloader);
                    foreach (VirtualMag qrMag in qrMagList)
                    {
                        Log.RBLog.Info(string.Format("[QR読取マガジン取得] MagazineNo:{0}", qrMag.MagazineNo));

                        if (this.HoldingMagazines.Exists(m => m.MagazineNo == qrMag.MagazineNo) == false)
                        {
                            // 搬送PLCに記憶が無いマガジンNoは仮想マガジン削除
                            qrMag.Delete();
                            continue;
                        }
                        //Log.RBLog.Info(string.Format("[QR読取マガジン搬送先供給確認前] MagazineNo:{0}", qrMag.MagazineNo));
                        canMove = LineKeeper.SelectMoveTo(this, qrMag, out moveTo, new Location(this.QRReaderMacNo, Station.Unloader));
                        //Log.RBLog.Info(string.Format("[QR読取マガジン搬送先供給確認後] MagazineNo:{0}", qrMag.MagazineNo));
                        if (canMove)
                        {
                            // QR読取位置 ⇒ 移動先装置への移動
                            Log.RBLog.Info($"[MoveTo]ロボットQR読取部⇒{moveTo.DirectoryPath}");
                            this.MoveTo(moveTo, qrMag, false, true);
                        }
                    }
                }

                // 優先搬送装置から積載
                var priorityList = machines.Select(m => m.Priority).Distinct().OrderBy(i => i);
                foreach (var cp in priorityList)
                {
                    // 同一優先度装置リスト作成
                    var currentMacList = machines.Where(m => m.Priority == cp);

                    canMove = LineKeeper.SelectMoveFrom(currentMacList, out moveFrom, out moveTo, this, this.LastMoveTo);
                    if (canMove)
                    {
                        // 移動元装置 ⇒ QR読取位置への移動
                        Log.RBLog.Info($"[MoveFrom] {moveFrom.DirectoryPath}⇒ロボットQR読取部");
                        //this.MoveFrom(moveFrom, true);
                        // 連結ライン対応：複数ラインのロボットがある環境下では
                        // ラインを超えた空Mag要求が発生すると同じロボットに対して同時に処理が発生してしまう為、
                        /// MoveFromTo直下にlock処理を行い、MoveFromとMoveToを実Mag搬送時にも同時に行うようにする。
                        this.MoveFromTo(moveFrom, moveTo, true, false, false, true);
                        break;
                    }
                    else
                    {
                        //MoveToで空マガジン移動要求あれば処理
                        bool canEmpMagMove = LineKeeper.SelectAndMoveEmptyMagazineRequest(currentMacList);
                        if (canEmpMagMove) break;
                    }
                }

            }
            catch (RobotException ex)
            {
                Log.RBLog.Info("ロボットスレッド例外処理 - 画面の選択ボタン = " + ex.Method.ToString());

                // 排出ボタンを押したときはロボット監視を停止にしない
                if (ex.Method == ErrorHandleMethod.Discharge)
                {
                    Log.RBLog.Info("ロボット監視を維持");
                }
                else
                {
                    Log.RBLog.Info("ロボット監視を停止");
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
                frmErr.ShowDialog();

                if (frmErr.Method == ErrorHandleMethod.None)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            //Log.RBLog.Info("[監視周期完了]");

            #endregion
        }

        /// <summary>
        /// 各装置に搬送ﾛﾎﾞｯﾄの連動運転中信号を送信
        /// </summary>
        /// <param name="machines"></param>
        /// <param name="data"></param>
        protected void SendReadyBitToReachMachines(List<IMachine> machines, string data)
        {
            foreach (IMachine mac in machines)
            {
                try
                {
                    // 連動運転中 (搬送PLC → 装置PLC)
                    mac.SetBitMachineReady(data);
                }
                catch (Exception)
                {
                    // PLC書き込みに失敗しても何もしない
                }
            }
        }

        /// <summary>
        /// 仮想マガジンの移動
        /// </summary>
        /// <param name="from">移動元</param>
        /// <param name="to">移動先</param>
        public override Location MoveFromTo(Location moveFrom, Location moveTo, bool dequeueMoveFrom, bool isEmptyMagazine, bool resetNextProcessIdToCurrentProfileFirstProcNo, bool isCheckQR)
        {
            // 連結ラインの場合、お互いの搬送RB処理でロボット処理を二重に行ってしまう為、
            // ここにロックをかける。
            lock (this.robotMoveLock)
            {
                // この関数の主な用途
                // ① 装置への空マガジン搬送  【呼び出し元 = 各装置のResponseEmptyMagazineRequest()】
                // ② MAP1stダイボンダーへのアオイ基板マガジン搬送 【呼び出し元 = MAP1stDieBonderのResponseEmptyMagazineRequest()】
                Log.RBLog.Info($"[MoveFromTo]{moveFrom.DirectoryPath}⇒{moveTo.DirectoryPath}");
                #region MoveFromTo(空マガジン・アオイ基板マガジンの搬送)

                // 装置へマガジンを取りに行く + ロボットの仮想マガジンに記憶
                MoveFrom(moveFrom, moveTo, dequeueMoveFrom, isEmptyMagazine, resetNextProcessIdToCurrentProfileFirstProcNo, isCheckQR);

                // 記憶直後の仮想マガジンを取得
                VirtualMag mag = LineKeeper.GetMachine(this.QRReaderMacNo).Peek(Station.Unloader);
                if (mag != null)
                {
                    mag.LocationId = (int)Station.Unloader;
                    mag.LocationNm = Station.Unloader.ToString();
                }

                // 装置へマガジンを置きに行く
                if (mag != null)
                {
                    moveTo = MoveTo(moveTo, mag, isEmptyMagazine, isCheckQR);
                }

                // 連続して空マガジン搬送が行われる場合、QR読み取り部に仮想マガジンがたまってしまう為、削除する
                if (mag != null)
                {
                    mag.Dequeue(new Location(this.QRReaderMacNo, Station.Unloader));
                }

                return moveTo;

                #endregion
            }
        }


        /// <summary>
        /// QR読込部へマガジンを持って行き、照合処理。
        /// 成功時はTrue、異常時はFalseを返す
        /// 内部で例外をThrowした場合は仮想マガジン変更せず
        /// </summary>
        /// <returns></returns>
        public bool MoveOrgToQR(Location from, ref VirtualMag mag, out string readedQR, bool isCheckQR)
        {
            #region エラーチェック

            if (this.Plc.GetBit(MAGAZINE_EXIST_ROBOT_ARM()) == Mitsubishi.BIT_ON)
            {
                //仮想マガジンは削除されない。
                throw new RobotStatusException("ロボットがマガジンを持っているので処理を開始できません。", this);
            }
            if (IsInputForbiddenQRReaderStage())
            {
                //仮想マガジンは削除されない。
                throw new RobotStatusException("ロボットの積載マガジン数が上限に達しています。処理を開始できません。", this);
            }
            #endregion

            #region

            System.Threading.SemaphoreSlim sem = null;
            string semName = string.Empty;

            try
            {
                #region 搬送処理

                // PLCクラスのWatchBit関数の引数⇒Action関数とその関数の引数用
                Action actWorkRelayPlcData;

                // ロボット操作Mutex取得
                carrierMutex.WaitOne();

                Log.RBLog.Info(string.Format("[開始]{1} {0}→QR読込", from.DirectoryPath, this.Name));

                // PLCのコマンド受付チェック
                while (IsPLCReadyToCommand(STAT_ROBOT_COMMAND_READY()) == false)
                {
                    Thread.Sleep(1000); //OFFの場合は1秒待機
                }

                // 全オーブンの全扉閉中のPLC信号が1になるまで待つ
                reachOvensWatchBit();
                
                // エラーフラグを一度リセット
                this.Plc.SetBit(STAT_MOVE_FAULT(), 1, Mitsubishi.BIT_OFF);
                this.Plc.SetBit(STAT_MOVE_FAULT_EMERGENCY(), 1, Mitsubishi.BIT_OFF);

                // FROM TO 設定(装置番号、位置番号、バッファ位置番号、搭載マガジンの選択)
                string macpoint = LineKeeper.GetMachine(from.MacNo).GetMacPoint(from.Station);
                string frombuffercode = LineKeeper.GetMachine(from.MacNo).GetFromToBufferCode(from.Station);
                Log.RBLog.Info(string.Format("[移動アドレス設定]{0} MoveFrom：『{1}』バッファ位置：『{3}』(from={4}) → 『{2}』", this.Name, macpoint, this.QRReaderMacPoint, frombuffercode, from.DirectoryPath));
                if (string.IsNullOrWhiteSpace(frombuffercode) == true)
                {
                    // バッファ位置取得失敗
                    throw new RobotChuckMissException($"マガジン取り出し先装置({from.DirectoryPath})のバッファ位置番号を取得できませんでした。", this);
                }
                setFromTo(macpoint, LineKeeper.GetMachine(from.MacNo).GetFromToCode(from.Station), frombuffercode,
                    this.QRReaderMacPoint, this.QRReaderLocationPoint, "1");
                
                // 移動指令1　ON
                this.Plc.SetBit(REQ_ROBOT_MOVE_ORG2MID(), 1, Mitsubishi.BIT_ON.ToString());

                // インターロックをかける
                if (this.SemaphoreSlimList.TryGetValue(macpoint, out semName) == true)
                {
                    if (LineKeeper.SemaphoreSlims.TryGetValue(semName, out sem) == true)
                    {
                        // 取りに行く場合は、無限に待機する
                        Log.RBLog.Info($"[インターロック解除待機]搬送先：『{macpoint}』, ロック名：『{semName}』 {this.Name}");
                        bool got = sem.Wait(-1);
                        Log.RBLog.Info($"[インターロックON]搬送先：『{macpoint}』, ロック名：『{semName}』 {this.Name}");
                    }
                }

                // ﾛﾎﾞｯﾄMag排出中ONになるまで待機
                Log.RBLog.Info(string.Format("[ﾛﾎﾞｯﾄMag排出中ON待機]{0}", this.Name));
                actWorkRelayPlcData = () => { WorkRelayPlcData(from, frombuffercode, false, null); };
                this.Plc.WatchBit(DOING_UNLOAD_MAGAZINE(), 0, Mitsubishi.BIT_ON, actWorkRelayPlcData);
                
                // 移動先がオーブンの場合、扉が開くまで待つ(待機中にPLC信号の中継処理も実施)
                if (LineKeeper.GetMachine(from.MacNo) is Oven)
                {
                    Log.RBLog.Info($"[オーブン扉開待機]{this.Name} -> {from.DirectoryPath}");
                    Oven ovenMac = (Oven)LineKeeper.GetMachine(from.MacNo);
                    actWorkRelayPlcData = () => { WorkRelayPlcData(from, frombuffercode, false, null); };
                    ovenMac.Plc.WatchBit(ovenMac.OvenDoorOpenStatusBitAddress, 0, Mitsubishi.BIT_ON, actWorkRelayPlcData);
                    Log.RBLog.Info($"[ロボットMag供給・排出可能ON]{this.Name}");
                    this.Plc.SetBit(CAN_LOAD_UNLOAD_MAGAZINE(), 1, PLC.Common.BIT_ON);
                }
                else
                {
                    // LineConfigの項目に設定がある場合、マガジンガイドが開くまで待つ (実Mag or 空Mag)
                    // 待機中にPLC信号の中継処理も実施
                    IMachine mac = LineKeeper.GetMachine(from.MacNo);
                    string unloadCanBitAddress = mac.GetCanBitAddress(from.Station);

                    // LineConfigに設定がある装置のみ実施
                    if (string.IsNullOrWhiteSpace(unloadCanBitAddress) == false)
                    {
                        Log.RBLog.Info($"[マガジンガイド開待機]{this.Name} -> {from.DirectoryPath}");
                        actWorkRelayPlcData = () => { WorkRelayPlcData(from, frombuffercode, false, null); };
                        mac.Plc.WatchBit(unloadCanBitAddress, 0, Mitsubishi.BIT_ON, actWorkRelayPlcData);
                        Log.RBLog.Info($"[ロボットMag供給・排出可能ON]{this.Name}");
                        this.Plc.SetBit(CAN_LOAD_UNLOAD_MAGAZINE(), 1, PLC.Common.BIT_ON);
                    }
                }

                // 移動指令OFFになるまで待機(待機中にPLC信号の中継処理も実施)
                Log.RBLog.Info(string.Format("[移動完了待機]{0}", this.Name));
                //this.Plc.WatchBit(REQ_ROBOT_MOVE_ORG2MID, 0, Mitsubishi.BIT_OFF);
                actWorkRelayPlcData = () => { WorkRelayPlcData(from, frombuffercode, false, null); };
                this.Plc.WatchBit(REQ_ROBOT_MOVE_ORG2MID(), 0, Mitsubishi.BIT_OFF, actWorkRelayPlcData);
                Log.RBLog.Info(string.Format("[移動完了]{0}", this.Name));

                // 装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT()) == Mitsubishi.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));

                    // E1が立っている場合はQR読み取り異常と判定
                    readedQR = "読取異常";
                    return false;
                }

                // 装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT_EMERGENCY()) == Mitsubishi.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    // エラーを返して監視ルーチン自体を停止
                    // 仮想マガジンは削除されない。
                    throw new RobotChuckMissException("ロボット動作異常停止", this);
                }

                // マガジン取得先の装置Noを仮想マガジンに記憶(同一リニア優先機能に使用)
                mag.Origin = from.MacNo;
                mag.OriginLocationId = (int)from.Station;

                #endregion

                #region QRコード読取

                // QRコード照合動作
                string qrcode = this.Plc.GetMagazineNo(QRWordAddressStart, QRWordAddressLength);
                //Log.SysLog.Info(string.Format("[QRCODE内容]{0}", qrcode));

                readedQR = qrcode;
                if (qrcode != mag.MagazineNo)
                {
                    if (string.IsNullOrEmpty(mag.MagazineNo) == true || mag.MagazineNo == VirtualMag.UNKNOWN_MAGNO)
                    {
                        // 不明なマガジンを掴んでいる場合
                        mag.MagazineNo = qrcode;

                    }
                    else if (string.IsNullOrEmpty(qrcode))
                    {
                        // E1が立たずに読取内容が空白の場合は、
                        // プラズマ機空マガジン搬送等の、読取をスキップする移動と判断し、
                        // 強制的に照合一致と判定する。
                        readedQR = mag.MagazineNo;
                    }
                    else if (mag.IsAOIMag(false) == true)
                    {
                        // MoveTo内でのアオイマガジンかどうかの判別に使用する為、マガジンNoはそのまま
                        mag.MagazineNo = qrcode;
                        mag.MapAoiMagazineLotNo = VirtualMag.MAP_AOI_MAGAZINE;
                        // アオイマガジンをつかんだ場合はSvrの情報に基づいて行先を更新
                        updateAoiMagazineInfo(mag, qrcode);
                    }
                    else if (isCheckQR == true)
                    {
                        mag.PurgeReason = string.Format("[QR照合不一致]{2} システム：{0} 読み込み：{1}", mag.MagazineNo, readedQR, this.Name);
                        //mag.MagazineNo = readedQR;

                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                #endregion

                // 移動先がオーブンの場合、扉が閉じるまで待つ(待機中にPLC信号の中継処理も実施)
                if (LineKeeper.GetMachine(from.MacNo) is Oven)
                {
                    Oven oven = (Oven)LineKeeper.GetMachine(from.MacNo);
                    Log.RBLog.Info($"[オーブン全扉閉待機]PLCアドレス：{oven.Plc.IPAddress},ポート：{oven.Plc.Port},信号アドレス：{oven.OvenAllDoorCloseBitAddress} -> BIT_ON");
                    actWorkRelayPlcData = () => { WorkRelayPlcData(from, frombuffercode, false, null); };
                    oven.Plc.WatchBit(oven.OvenAllDoorCloseBitAddress, 0, PLC.Common.BIT_ON, actWorkRelayPlcData);
                }

                Log.RBLog.Info($"[MoveOrgToQR完了]{this.Name}");

                return true;
            }
            finally
            {
                // 搬送時にインターロックを掛けていた場合、インターロックを解除する
                if (sem != null)
                {
                    Log.RBLog.Info($"[インターロック解除]ロック名：『{semName}』 {this.Name}");
                    sem.Release();
                }

                carrierMutex.ReleaseMutex();
            }
            #endregion
        }

        /// <summary>
        /// QR読込部から投入先へ移動。
        /// 途中で移動先を変更する場合があるので最終的な移動先を返す。
        /// 内部で例外をThrowした場合は仮想マガジン変更せず
        /// </summary>
        /// <param name="to"></param>
        /// <param name="mag"></param>
        /// <returns></returns>
        public Location MoveQRToDst(Location to, bool isEmptyMagazine, ref VirtualMag mag)
        {
            #region エラーチェック

            if (this.Plc.GetBit(MAGAZINE_EXIST_ROBOT_ARM()) == Mitsubishi.BIT_ON)
            {
                throw new RobotStatusException("ロボットがマガジンを持っているので処理を開始できません。手動で取り出して途中投入コンベヤに入れてください。", this);
            }
            if (IsQRReaderStageMagazineExists() == false)
            {
                throw new RobotStatusException("マガジンがQR読込位置にありません", this);
            }
            #endregion
                        
            #region 排出理由のある、警告ロットマガジンは排出CV(異常品)に置き換え

            if (string.IsNullOrWhiteSpace(mag.PurgeReason) == false)
            {
                int? conveyor = LineKeeper.GetDischargeConveyor(to.MacNo);
                if (conveyor.HasValue == false)
                    return null;

                Log.RBLog.Info($"[搬送先変更]排出理由がある為、排出CVへ搬送します。マガジン：『{mag.MagazineNo}』,排出理由：『{mag.PurgeReason}』");
                to = new Location(conveyor.Value, Station.Loader);
            }

            Magazine svrMag = Magazine.GetCurrent(mag.MagazineNo);
            AsmLot lot = null;
            if (svrMag == null)
            {
                // 空マガジン搬送フラグがOFFの状態でマガジンが見つからない場合は、排出する
                if (isEmptyMagazine == false && string.IsNullOrWhiteSpace(mag.MapAoiMagazineLotNo) == true && VirtualMag.IsECKMag(mag.MagazineNo) == false)
                {
                    int? conveyor = LineKeeper.GetDischargeConveyor(to.MacNo);
                    if (conveyor.HasValue == false)
                        return null;

                    Log.RBLog.Info($"[搬送先変更]稼働中マガジンではない為、排出CVへ搬送します。マガジン：『{mag.MagazineNo}』");
                    to = new Location(conveyor.Value, Station.Loader);
                }
            }
            else
            {
                lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);

                if (lot.IsWarning)
                {
                    if (string.IsNullOrWhiteSpace(mag.PurgeReason))
                    {
                        mag.PurgeReason += "【警告】ロットに警告フラグが立っています。";
                    }

                    int? conveyor = LineKeeper.GetDischargeConveyor(to.MacNo);
                    if (conveyor.HasValue == false)
                        return null;

                    Log.RBLog.Info($"[搬送先変更]ロットに警告フラグが立っている為、排出CVへ搬送します。ロット：『{lot.NascaLotNo}』,マガジン：『{svrMag.MagazineNo}』");
                    to = new Location(conveyor.Value, Station.Loader);
                }
            }

            #endregion

            #region 橋、高効率へのmoveTo置き換え

            CarrierInfo fromCarrier = new CarrierInfo(this.CarNo);

            //To側に手が届かない場合はRoute.GetHandoverLocation関数で行き先の置き換え（ラインブリッジ、排出CV）
            CarrierInfo toCarrier = Route.GetReachable(to);
            if (toCarrier == null)
            {
                //高効率ライン　ラインアウト排出CVへ
                to = new Location(Route.GetAutoLineOutConveyor(mag.MacNo), Station.Unloader);
                toCarrier = Route.GetReachable(to);
            }

            if (fromCarrier.CarNo != toCarrier.CarNo)
            {
                //自動化ライン　橋へ

                //移動指令先が排出CVで仮想マガジン内が排出CVでは無い場合
                //仮想マガジン内を排出CVで上書きする
                if (LineKeeper.GetMachine(to.MacNo) is DischargeConveyor
                    && -1 == mag.NextMachines.FindIndex(n => LineKeeper.GetMachine(n) is DischargeConveyor))
                {
                    mag.NextMachines.Clear();
                    //mag.NextMachines.Add(Route.GetDischargeConveyor(mag.MacNo));
                    // 元々の移動先が橋の向こうのラインの場合、このタイミングで次装置が自ラインの排出CVになってしまい、
                    // 橋の出口で滞留してしまう不具合が発生する為
                    mag.NextMachines.Add(to.MacNo);
                }

                //移動指令先がアオイ排出CVで仮想マガジン内がアオイ排出CVでは無い場合
                //仮想マガジン内をアオイ排出CVで上書きする
                if (LineKeeper.GetMachine(to.MacNo) is MAPBoardDischargeConveyor
                    && -1 == mag.NextMachines.FindIndex(n => LineKeeper.GetMachine(n) is MAPBoardDischargeConveyor))
                {
                    mag.NextMachines.Clear();
                    mag.NextMachines.Add(to.MacNo);
                }

                //移動指令先がラインアウト排出CVで仮想マガジン内がラインアウト排出CVでは無い場合
                //仮想マガジン内をラインアウト排出CVで上書きする
                if (LineKeeper.GetMachine(to.MacNo) is AutoLineOutConveyor
                    && -1 == mag.NextMachines.FindIndex(n => LineKeeper.GetMachine(n) is AutoLineOutConveyor))
                {
                    mag.NextMachines.Clear();
                    mag.NextMachines.Add(to.MacNo);
                }

                //to = Route.GetHandoverLocation(fromCarrier, toCarrier);
                to = null;
                List<Location> lbridgeList = Route.GetHandoverLocationList(fromCarrier, toCarrier);
                // 橋のいずれかが供給ONならその橋へ、OFFなら排出CVへ
                // 条件追加：自ラインに排出CVが存在しない場合は橋の供給OFFでも橋を搬送先にする。
                foreach(Location loc in lbridgeList)
                {
                    IMachine ibridge = LineKeeper.GetMachine(loc.MacNo);
                    if (ibridge.IsRequireInput() == true)
                    {
                        to = loc;
                        break;
                    }
                }
                int dischargeCV = Route.GetDischargeConveyor(mag.MacNo);
                Location locDischargeCV = new Location(dischargeCV, Station.Loader);
                CarrierInfo dischargeCarrier = Route.GetReachable(locDischargeCV);
                // toDischargeCarrierのnullチェックは不要。 nullになるならRoute.GetDischargeConveyor(from.MacNo)でエラー発生している
                if (to != null)
                {
                    //橋のmacnoを先頭に追加(橋に運び終えた後、このmacnoは削除している)
                    mag.NextMachines.Insert(0, to.MacNo);
                }
                else if (fromCarrier.CarNo != dischargeCarrier.CarNo)
                {
                    // 手の届く範囲に完成品排出CVがいる場合は、そちらに搬送する
                    List<int> compltDischargeConveyors = Route.GetCompltDischargeConveyors(mag.MacNo);
                    if (compltDischargeConveyors.Count() >= 1)
                    {
                        int compltDischargeCV = compltDischargeConveyors.First();
                        Location locCompltDischargeCV = new Location(compltDischargeCV, Station.Loader);
                        CarrierInfo compltDischargeCarrier = Route.GetReachable(locCompltDischargeCV);
                        if (fromCarrier.CarNo == compltDischargeCarrier.CarNo)
                        {
                            to = locCompltDischargeCV;
                        }
                    }

                    if (to == null)
                    {
                        // 供給要求ONの橋が無い場合、橋のリストの内、先頭の橋を選択
                        //橋のmacnoを先頭に追加(橋に運び終えた後、このmacnoは削除している)
                        to = lbridgeList.First();
                        mag.NextMachines.Insert(0, to.MacNo);
                    }
                }
                else
                {
                    mag.NextMachines.Clear();
                    mag.NextMachines.Add(dischargeCV);
                    to = locDischargeCV;
                }

                //搬送先がライン連結橋に変更 かつ 空マガジンを識別する為、ライン連結橋に空マガジンを置いた時はフラグをON
                if (LineKeeper.GetMachine(to.MacNo) is LineBridge)
                {
                    LineBridge bridge = (LineBridge)LineKeeper.GetMachine(to.MacNo);
                    if (string.IsNullOrWhiteSpace(bridge.EmptyMagazineInputBitAddress) == false)
                    {
                        if (isEmptyMagazine)
                        {
                            bridge.Plc.SetBit(bridge.EmptyMagazineInputBitAddress, 1, Mitsubishi.BIT_ON);
                        }
                        else
                        {
                            //空マガジンでない場合は、フラグをOFF
                            bridge.Plc.SetBit(bridge.EmptyMagazineInputBitAddress, 1, Mitsubishi.BIT_OFF);
                        }
                    }
                }
            }

            #endregion

            #region

            Location retv;

            System.Threading.SemaphoreSlim sem = null;
            string semName = string.Empty;
            Machines.Oven ovn = LineKeeper.GetMachine(to.MacNo) as Machines.Oven;
            if (Config.Settings.UseOvenProfiler)
            {
                #region オーブンプロファイル照合
                //オーブンへの移動時には操作禁止フラグを操作
                if (ovn != null)
                {
                    int current = ovn.GetCurrentProfile();
                    int magProfile = OvenProfiler.GetProfile(mag);

                    if (current != magProfile)
                    {
                        Log.RBLog.Info($"[マガジンNo：{mag.MagazineNo}]プロファイル不一致のため次回投入可能装置から除外{to.MacNo}(次装置の残り台数={mag.NextMachines.Count()}台)");
                        mag.NextMachines.Remove(to.MacNo);
                        if (mag.NextMachines.Count >= 1)
                        {
                            // 【N工場MAP J9・10不具合 修正】
                            List<int> targetMachines = new List<int>();
                            List<int> targetReachMachines = new List<int>();
                            List<int> targetNonReachSameLineMachines = new List<int>();
                            List<int> targetOtherMachines = new List<int>();

                            List<int> reachMachines = Route.GetMachines(this.CarNo);
                            foreach (int mac in mag.NextMachines)
                            {
                                IMachine iMac = LineKeeper.GetMachine(mac);
                                if (iMac == null) continue;

                                if (reachMachines.Exists(m => m == mac))
                                {
                                    targetReachMachines.Add(mac);
                                }
                                else if (iMac.LineNo == LineKeeper.GetMachine(to.MacNo).LineNo)
                                {
                                    targetNonReachSameLineMachines.Add(mac);
                                }
                                else
                                {
                                    targetOtherMachines.Add(mac);
                                }
                            }
                            // 手の届く装置 -> 同じライン -> その他の装置順にリストを作成
                            targetMachines.AddRange(targetReachMachines);
                            targetMachines.AddRange(targetNonReachSameLineMachines);
                            targetMachines.AddRange(targetOtherMachines);

                            foreach (int machine in targetMachines)
                            {
                                IMachine nextm = LineKeeper.GetMachine(machine);
                                if (nextm.IsReady() == true && nextm.IsRequireInput() == true)
                                {
                                    to = nextm.GetLoaderLocation();
                                    if (to.MacNo == Route.GetDischargeConveyor(nextm.MacNo))
                                    {
                                        continue;
                                    }
                                    retv = to;
                                    Log.RBLog.Info(string.Format("[再移動開始]{0} 対象:{1}", this.Name, to.DirectoryPath));
                                    return MoveQRToDst(to, isEmptyMagazine, ref mag);
                                }
                            }
                        }

                        Log.RBLog.Info(string.Format("[排出]{0}", this.Name));
                        PurgeHandlingMagazine(mag, "一致するオーブンプロファイル無し");
                        return new Location(Route.GetDischargeConveyor(to.MacNo), Station.Loader);
                    }
                    else
                    {
                        ovn.SetInterlock(true);
                    }
                }
                #endregion
            }
            try
            {
                // PLCクラスのWatchBit関数の引数⇒Action関数とその関数の引数用
                Action actWorkRelayPlcData;
                VirtualMag refMag;

                //  ロボット操作Mutex取得
                carrierMutex.WaitOne();

                Log.RBLog.Info(string.Format("[開始]{1} QR読込→{0}", to.DirectoryPath, this.Name));
                Log.RBLog.Info(string.Format("[PLCコマンド受付確認]{0}", this.Name));

                int i = 1;
                while (IsPLCReadyToCommand(STAT_ROBOT_COMMAND_READY()) == false)
                {
                    Log.RBLog.Info(string.Format("[移動指令許可要求待機]{0}回目", i));
                    i++;
                    Thread.Sleep(1000); //OFFの場合は1秒待機
                }

                // 全オーブンの全扉閉中のPLC信号が1になるまで待つ
                reachOvensWatchBit();

                //エラーフラグを一度リセット
                this.Plc.SetBit(STAT_MOVE_FAULT(), 1, Mitsubishi.BIT_OFF);
                this.Plc.SetBit(STAT_MOVE_FAULT_EMERGENCY(), 1, Mitsubishi.BIT_OFF);

                // FROM TO 設定(装置番号、位置番号、搭載マガジンの選択)
                string macpoint = LineKeeper.GetMachine(to.MacNo).GetMacPoint(to.Station);
                //Log.RBLog.Info(string.Format("[移動アドレス設定]{0} MoveTo：『{1}』 → 『{2}』", this.Name, this.QRReaderMacPoint, macpoint));
                string tobuffercode = LineKeeper.GetMachine(to.MacNo).GetFromToBufferCode(to.Station);
                Log.RBLog.Info(string.Format("[移動アドレス設定]{0} MoveTo：『{1}』 → 『{2}』 バッファ位置：『{3}』(to={4})", this.Name, this.QRReaderMacPoint, macpoint, tobuffercode, to.DirectoryPath));
                if (string.IsNullOrWhiteSpace(tobuffercode) == true)
                {
                    // バッファ位置取得失敗
                    mag.PurgeReason = $"マガジン搬送先装置({to.DirectoryPath})のバッファ位置番号を取得できませんでした。";
                    return MoveQRToDst(new Location(LineKeeper.GetDischargeConveyor(to.MacNo).Value, Station.Loader), isEmptyMagazine, ref mag);
                }
                setFromTo(this.QRReaderMacPoint, this.QRReaderLocationPoint, this.GetQRReaderStageBufferCode(mag.MagazineNo),
                    macpoint, LineKeeper.GetMachine(to.MacNo).GetFromToCode(to.Station), tobuffercode);
                
                // 移動指令2　ON
                this.Plc.SetBit(REQ_ROBOT_MOVE2_MID2DST(), 1, Mitsubishi.BIT_ON.ToString());

                // 作業開始登録
                bool canReleaseMagazine = false;
                if (mag.ProcNo == null)
                {
                    //空マガジンの移動の場合、または他ライン製品は開始登録を行わない
                    canReleaseMagazine = true;
                }
                else if (LineKeeper.GetMachine(to.MacNo) is LineBridge)
                {
                    //橋行きは開始登録なし
                    canReleaseMagazine = true;
                }
                else if (LineKeeper.GetMachine(to.MacNo) is DischargeConveyor)
                {
                    //排出コンベヤ行きも開始登録なし
                    canReleaseMagazine = true;
                }
                else if (LineKeeper.GetMachine(to.MacNo) is MAPCompltDischargeConveyor)
                {
                    //完成品排出コンベヤ行きも開始登録なし
                    canReleaseMagazine = true;
                }
                else if (LineKeeper.GetMachine(to.MacNo) is AutoLineOutConveyor)
                {
                    //高効率用排出コンベヤ行きも開始登録なし
                    canReleaseMagazine = true;
                }
                else if (LineKeeper.GetMachine(to.MacNo) is MAPBoardDischargeConveyor)
                {
                    //アオイマガジン排出コンベヤ行きも開始登録なし
                    canReleaseMagazine = true;
                }
                else if (MachineInfo.GetMachine(to.MacNo).ClassName.Trim() == "AGV投入CV")
                {
                    //AGV投入コンベヤ行きも開始登録なし
                    canReleaseMagazine = true;
                }
                // 他の装置
                else
                {
                    //作業開始登録
                    Log.ApiLog.Info(string.Format("[作業開始登録]{0}", this.Name));

                    bool doWorkStart = true;
                    if (LineKeeper.GetMachine(to.MacNo) is MAP1stDieBonder)
                    {
                        #region MAP1stダイボンダーのアンローダーへの空マガジン搬送処理 (プロファイル要求ロット数の上限チェック)

                        // 別々のラインで紐付けプロファイルが同じダイボンダーがある場合に、同タイミングで搬送指示が出る可能性がある為、
                        // 搬送途中で再度、上限ロット数チェックを行う
                        var map1stDieBonder = (MAP1stDieBonder)LineKeeper.GetMachine(to.MacNo);
                        int inputCt, orderCt;
                        if (map1stDieBonder.IsLimitOverProfileLotCount(out inputCt, out orderCt) == true)
                        {
                            Log.ApiLog.Info($"[排出]{this.Name} 理由:搬送先装置のプロファイル上限超過の為 装置={map1stDieBonder.Name} " +
                                            $"プロファイルロット数={orderCt}Lot, 投入済みロット数={inputCt}Lot");
                            doWorkStart = false;
                        }
                        else
                        {
                            Log.RBLog.Info($"{this.Name} 搬送先装置のプロファイル上限未満の為、投入実施 装置={map1stDieBonder.Name} " +
                                            $"プロファイルロット数={orderCt}Lot, 投入済みロット数={inputCt}Lot");
                        }

                        #endregion
                    }
                    if (doWorkStart == true)
                    {
                        ArmsApiResponse res = CommonApi.WorkStart(CommonApi.GetWorkStartOrder(mag, to.MacNo));
                        if (res.IsError)
                        {
                            Log.ApiLog.Info(res.Message);
                            //mag.PurgeReason = res.Message;
                        }
                        else
                        {
                            canReleaseMagazine = true;
                            Log.ApiLog.Info(string.Format("[作業開始登録完了]{0}", this.Name));
                        }
                    }
                }

                // リニア位置到達信号監視(待機中にPLC信号の中継処理も実施)
                Log.RBLog.Info(string.Format("[リニア移動監視開始]{0}", this.Name));
                //this.Plc.WatchBit(ROBOT_MOVE2_LINER_COMPLETE, 0, PLC.Common.BIT_ON);
                refMag = mag;
                actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, true, refMag.MagazineNo); };
                this.Plc.WatchBit(ROBOT_MOVE2_LINER_COMPLETE(), 0, PLC.Common.BIT_ON, actWorkRelayPlcData);
                Log.RBLog.Info(string.Format("[リニア移動監視終了]{0}", this.Name));

                // 装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT()) == Mitsubishi.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    Log.RBLog.Info(string.Format("[排出]{0}", this.Name));

                    mag.PurgeReason = "ロボット動作異常のため排出";
                    return MoveQRToDst(new Location(LineKeeper.GetDischargeConveyor(to.MacNo).Value, Station.Loader), isEmptyMagazine, ref mag);
                }

                // 装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT_EMERGENCY()) == Mitsubishi.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    //エラーを返して監視ルーチン自体を停止

                    mag.PurgeReason = "ロボット動作異常のため排出";
                    return MoveQRToDst(new Location(LineKeeper.GetDischargeConveyor(to.MacNo).Value, Station.Loader), isEmptyMagazine, ref mag);
                }

                // 移動先が遠心沈降自動機の場合、リニア移動後に再度供給要求を確認
                // 装置上の搬送RBが搬送先置場にマガジンを置いている可能性がある為
                if (canReleaseMagazine == true)
                {
                    if (LineKeeper.GetMachine(to.MacNo) is ECK4 || LineKeeper.GetMachine(to.MacNo) is ECK4MagExchanger)
                    {
                        // 搬送先に指定しているローダーポイント(toAddress)の供給要求を再確認
                        string reqBitData = LineKeeper.GetRequireBitData(LineKeeper.GetMachine(to.MacNo), to.Station, tobuffercode, this);

                        // 搬送先指定位置の供給要求が移動中にOFFになっていた場合は、排出CVへ搬送する。
                        if (reqBitData == PLC.Common.BIT_OFF)
                        {
                            Log.RBLog.Info($"[排出]{this.Name}搬送予定先の供給要求が落ちています。搬送先「{to.DirectoryPath}」,搬送先バッファ位置「{tobuffercode}」");
                            canReleaseMagazine = false;
                            //最終の開始実績を削除
                            deleteLastWorkStartRecord(mag);
                        }
                    }
                }

                if (canReleaseMagazine == true)
                {
                    //  移動指令2　ON
                    Log.RBLog.Info(string.Format("[マガジンリリース]{0}", this.Name));

                    LineKeeper.GetMachine(to.MacNo).LoaderConveyorStop(true);

                    // インターロックをかける
                    if (this.SemaphoreSlimList.TryGetValue(macpoint, out semName) == true)
                    {
                        if (LineKeeper.SemaphoreSlims.TryGetValue(semName, out sem) == true)
                        {
                            // 置きに行く場合は、60秒間に待機する
                            Log.RBLog.Info($"[インターロック解除待機]搬送先：『{macpoint}』, ロック名：『{semName}』 {this.Name}");
                            bool got = sem.Wait(60000);
                            if (got == false)
                            {
                                // この関数を抜ける時のfinallty内でロックを解除しないように変数をnullにする
                                sem = null;

                                Log.RBLog.Info($"[排出]{this.Name} インターロック解除時間超過の為");
                                // 【N工場MAP J9・10不具合 修正】
                                this.Plc.SetBit(REQ_ROBOT_MOVE2_NASCA_ERROR(), 1, PLC.Common.BIT_ON);
                                //  移動指令OFFになるまで待機(待機中にPLC信号の中継処理も実施)
                                Log.RBLog.Info(string.Format("[完了待機]{0}", this.Name));
                                refMag = mag;
                                actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, true, refMag.MagazineNo); };
                                this.Plc.WatchBit(REQ_ROBOT_MOVE2_MID2DST(), 0, PLC.Common.BIT_OFF, actWorkRelayPlcData);
                                Log.RBLog.Info(string.Format("[排出]{0} 行き先=排出CV", this.Name));

                                return MoveQRToDst(new Location(LineKeeper.GetDischargeConveyor(to.MacNo).Value, Station.Loader), isEmptyMagazine, ref mag);
                            }
                            Log.RBLog.Info($"[インターロックON]搬送先：『{macpoint}』, ロック名：『{semName}』 {this.Name}");
                        }
                    }

                    this.Plc.SetBit(REQ_ROBOT_MOVE2_COMPLETE(), 1, Mitsubishi.BIT_ON.ToString());
                    retv = to;
                    mag.CurrentLocation = to;
                }
                else
                {
                    Log.RBLog.Info("次回投入可能装置から除外" + to.MacNo);
                    mag.NextMachines.Remove(to.MacNo);

                    //　移動失敗フラグセット
                    Log.RBLog.Info(string.Format("[NASCA返答NGフラグ設定]{0}", this.Name));
                    this.Plc.SetBit(REQ_ROBOT_MOVE2_NASCA_ERROR(), 1, PLC.Common.BIT_ON);
                    //  移動指令OFFになるまで待機(待機中にPLC信号の中継処理も実施)
                    Log.RBLog.Info(string.Format("[完了待機]{0}", this.Name));
                    //this.Plc.WatchBit(REQ_ROBOT_MOVE2_MID2DST, 0, PLC.Common.BIT_OFF);
                    refMag = mag;
                    actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, true, refMag.MagazineNo); };
                    this.Plc.WatchBit(REQ_ROBOT_MOVE2_MID2DST(), 0, PLC.Common.BIT_OFF, actWorkRelayPlcData);

                    if (mag.NextMachines.Count >= 1)
                    {
                        IMachine nextm = LineKeeper.SelectTopPriorityLineMachine(this, mag.NextMachines, mag);
                        if (nextm != null && Route.GetDischargeConveyors(to.MacNo).Contains(nextm.MacNo) == false)
                        {
                            to = nextm.GetLoaderLocation();
                            retv = to;
                            Log.RBLog.Info(string.Format("[再移動開始]{0}", this.Name));
                            return MoveQRToDst(to, isEmptyMagazine, ref mag);
                        }
                    }

                    Log.RBLog.Info(string.Format("[排出]{0}", this.Name));
                    return MoveQRToDst(new Location(LineKeeper.GetDischargeConveyor(to.MacNo).Value, Station.Loader), isEmptyMagazine, ref mag);
                }
                
                // 搬送RB移動後の装置側の扉・マガジンガイド開待機
                if (LineKeeper.GetMachine(to.MacNo) is Oven)
                {
                    // 移動先がオーブンの場合、扉が開くまで待つ(待機中にPLC信号の中継処理も実施)
                    Log.RBLog.Info($"[オーブン扉開待機]{this.Name} -> {to.DirectoryPath}");
                    Oven ovenMac = (Oven)LineKeeper.GetMachine(to.MacNo);
                    //ovenMac.Plc.WatchBit(ovenMac.OvenDoorOpenStatusBitAddress, 0, Mitsubishi.BIT_ON);
                    refMag = mag;
                    actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, true, refMag.MagazineNo); };
                    ovenMac.Plc.WatchBit(ovenMac.OvenDoorOpenStatusBitAddress, 0, Mitsubishi.BIT_ON, actWorkRelayPlcData);

                    Log.RBLog.Info($"[ロボットMag供給・排出可能ON]{this.Name}");
                    this.Plc.SetBit(CAN_LOAD_UNLOAD_MAGAZINE(), 1, PLC.Common.BIT_ON);
                }
                else
                {
                    // モールド装置にも同様の処理が必要になった為、ワイヤボンダー限定の処理から全装置共通の処理に変更

                    // LineConfigの項目に設定がある場合、マガジンガイドが開くまで待つ (実Mag or 空Mag)
                    // 待機中にPLC信号の中継処理も実施
                    IMachine mac = LineKeeper.GetMachine(to.MacNo);
                    string loadCanBitAddress = mac.GetCanBitAddress(to.Station);

                    // LineConfigに設定がある装置のみ実施
                    if (string.IsNullOrWhiteSpace(loadCanBitAddress) == false)
                    {
                        Log.RBLog.Info($"[マガジンガイド開待機]{this.Name} -> {to.DirectoryPath}");
                        refMag = mag;
                        actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, true, refMag.MagazineNo); };
                        mac.Plc.WatchBit(loadCanBitAddress, 0, Mitsubishi.BIT_ON, actWorkRelayPlcData);
                        Log.RBLog.Info($"[ロボットMag供給・排出可能ON]{this.Name}");
                        this.Plc.SetBit(CAN_LOAD_UNLOAD_MAGAZINE(), 1, PLC.Common.BIT_ON);
                    }
                }
                //else if (LineKeeper.GetMachine(to.MacNo) is WireBonder)
                //{
                //    // 移動先がワイヤーボンダーの場合、マガジンガイドが開くまで待つ (実Mag or 空Mag)
                //    // 待機中にPLC信号の中継処理も実施
                //    WireBonder wireMac = (WireBonder)LineKeeper.GetMachine(to.MacNo);
                //    string loadCanBitAddress = string.Empty;
                //    if (isEmptyMagazine == true)
                //    {
                //        // 空Mag供給
                //        Log.RBLog.Info($"[ULD部マガジンガイド開待機]{this.Name} -> {to.DirectoryPath}");
                //        loadCanBitAddress = wireMac.EmpMagLoaderCanBitAddress;
                //    }
                //    else
                //    {
                //        // 実Mag供給
                //        Log.RBLog.Info($"[LD部マガジンガイド開待機]{this.Name} -> {to.DirectoryPath}");
                //        loadCanBitAddress = wireMac.LoaderCanBitAddress;
                //    }
                //    //wireMac.Plc.WatchBit(loadCanBitAddress, 0, Mitsubishi.BIT_ON);
                //    refMag = mag;
                //    actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, true, refMag.MagazineNo); };
                //    wireMac.Plc.WatchBit(loadCanBitAddress, 0, Mitsubishi.BIT_ON, actWorkRelayPlcData);
                //    Log.RBLog.Info($"[ロボットMag供給・排出可能ON]{this.Name}");
                //    this.Plc.SetBit(CAN_LOAD_UNLOAD_MAGAZINE(), 1, PLC.Common.BIT_ON);
                //}

                //  移動指令OFFになるまで待機(待機中にPLC信号の中継処理も実施)
                Log.RBLog.Info(string.Format("[移動完了待機]{0}", this.Name));
                //this.Plc.WatchBit(REQ_ROBOT_MOVE2_MID2DST, 0, PLC.Common.BIT_OFF);
                refMag = mag;
                actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, true, refMag.MagazineNo); };
                this.Plc.WatchBit(REQ_ROBOT_MOVE2_MID2DST(), 0, PLC.Common.BIT_OFF, actWorkRelayPlcData);

                //　装置エラー発生のチェック（リリース指示後）
                if (this.Plc.GetBit(STAT_MOVE_FAULT()) == Mitsubishi.BIT_ON)
                {
                    //NASCAへメッセージ
                    Log.RBLog.Info(string.Format("[ロボットタイムアウト検出]{0}", this.Name));

                    //最終の開始実績を削除
                    //if (canReleaseMagazine) deleteLastWorkStartRecord(mag);
                    deleteLastWorkStartRecord(mag);

                    // 搬送時にインターロックを掛けていた場合、インターロックを解除する
                    if (sem != null)
                    {
                        Log.RBLog.Info($"[インターロック解除]ロック名：『{semName}』 {this.Name}");
                        sem.Release();
                        // この関数を抜ける時のfinallty内でロックを解除しないように変数をnullにする
                        sem = null;
                    }
                    PurgeHandlingMagazine(mag, "ロボットタイムアウトの為排出");
                    return new Location(LineKeeper.GetDischargeConveyor(to.MacNo).Value, Station.Loader);
                }

                //　装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT_EMERGENCY()) == Mitsubishi.BIT_ON)
                {
                    //工程リセットが押された場合にここに入る可能性がある
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    //エラーを返して監視ルーチン自体を停止

                    //最終の開始実績を削除
                    //if (canReleaseMagazine) deleteLastWorkStartRecord(mag);
                    deleteLastWorkStartRecord(mag);

                    throw new RobotStatusException("ロボット動作異常停止\nマガジンを手で取り除いてください", this);
                }

                // 次の条件を満たす場合は、登録した開始実績を削除
                // 仮想マガジンだけがある状態にする。
                bool isDeleteLastWorkStartRecord = false;
                if (LineKeeper.GetMachine(to.MacNo) is Oven && ((Oven)LineKeeper.GetMachine(to.MacNo)).IsUseOvenProcessStatus)
                {
                    // 搬送先がオーブン かつ 硬化信号を使用する場合は、登録した開始実績を削除
                    // のちにオーブン内で硬化が始まったら開始実績登録処理を行う
                    isDeleteLastWorkStartRecord = true;
                }
                if (LineKeeper.GetMachine(to.MacNo).IsConveyor == true)
                {
                    // コンベア有の装置の場合、後でPDAで開始登録する為に削除する
                    isDeleteLastWorkStartRecord = true;
                }

                if (isDeleteLastWorkStartRecord == true)
                {
                    //最終の開始実績を削除
                    deleteLastWorkStartRecord(mag);
                    Log.RBLog.Info($"開始実績削除：{mag.MagazineNo}");
                }

                // 移動先がオーブンの場合、扉が閉じるまで待つ(待機中にPLC信号の中継処理も実施)
                if (LineKeeper.GetMachine(to.MacNo) is Oven)
                {
                    Oven oven = (Oven)LineKeeper.GetMachine(to.MacNo);
                    Log.RBLog.Info($"[オーブン全扉閉待機]PLCアドレス：{oven.Plc.IPAddress},ポート：{oven.Plc.Port},信号アドレス：{oven.OvenAllDoorCloseBitAddress} -> BIT_ON");
                    actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, false, null); };
                    oven.Plc.WatchBit(oven.OvenAllDoorCloseBitAddress, 0, PLC.Common.BIT_ON, actWorkRelayPlcData);
                }


                Log.RBLog.Info(string.Format("[移動完了]{0}", this.Name));

                return retv;
            }
            finally
            {
                // 搬送時にインターロックを掛けていた場合、インターロックを解除する
                if (sem != null)
                {
                    Log.RBLog.Info($"[インターロック解除]ロック名：『{semName}』 {this.Name}");
                    sem.Release();
                }

                if (Config.Settings.UseOvenProfiler)
                {
                    if (ovn != null)
                    {
                        ovn.SetInterlock(false);
                    }
                }

                LineKeeper.GetMachine(to.MacNo).LoaderConveyorStop(false);

                carrierMutex.ReleaseMutex();
            }
            #endregion
        }

        /// <summary>
        /// エラー排出時に既に入っている開始レコードを削除するルーチン
        /// </summary>
        /// <param name="mag"></param>
        protected void deleteLastWorkStartRecord(VirtualMag mag)
        {
            #region 最終開始レコード削除
            try
            {
                //if (mag.ProcNo == null || (mag.Origin.HasValue && mag.Origin != 0))
                //{
                //    //空マガジンの移動の場合、または他ライン製品は開始登録の削除をしない
                //    return;
                //}

                Magazine svmag = Magazine.GetCurrent(mag.MagazineNo);
                if (svmag == null) Log.RBLog.Error("Svrマガジン情報が見つかりません:" + mag.MagazineNo);

                AsmLot lot = AsmLot.GetAsmLot(svmag.NascaLotNO);
                if (lot == null) Log.RBLog.Error("Svrロット情報が見つかりません:" + svmag.NascaLotNO);

                Order order = Order.GetMagazineOrder(svmag.NascaLotNO, mag.ProcNo.Value);

                //指定作業が開始状態であれば削除する
                if (order != null && !order.IsComplete)
                {
                    order.DelFg = true;
                    order.DeleteInsert(order.LotNo);
                }

            }
            catch (Exception ex)
            {
                Log.RBLog.Error("排出ルーチン前　最終開始実績削除失敗:" + ex.ToString());
            }
            #endregion
        }

        /// <summary>
        /// ベーキング作業レコードを照会してアオイマガジンの行先を振り分け
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="qrcode"></param>
        protected void updateAoiMagazineInfo(VirtualMag mag, string qrcode)
        {
            #region アオイ基板マガジンの仮想マガジンを更新
            //Log.SysLog.Info(string.Format("[アオイマガジン行先変更前]nextmachines:{0}", string.Join(",", mag.NextMachines)));

            #region QR内容解析
            //品名,,ロット,,数量,GGコード,,,
            string[] values = qrcode.Split(VirtualMag.MAP_FRAME_SEPERATOR);
            if (values.Length != VirtualMag.MAP_FRAME_ELEMENT_CT)
            {
                mag.PurgeReason = string.Format("MAP基板QRコード内容が不正です。 QRコード内容:{0}", qrcode);
                return;
            }
            string lotno = values[VirtualMag.MAP_FRAME_QR_LOTNO_IDX];
            string ggcode = values[VirtualMag.MAP_FRAME_QR_MATCD_IDX];

            mag.MaxFrameCt = int.Parse(values[VirtualMag.MAP_FRAME_QR_CT_IDX]);
            mag.CurrentFrameCt = mag.MaxFrameCt;
            mag.FrameMatCd = ggcode;
            mag.FrameLotNo = lotno;
            #endregion

            //排出理由が既にある場合は何もしない
            if (!string.IsNullOrEmpty(mag.PurgeReason)) return;
            
            mag.NextMachines.Clear();

            //ダイボンダー行き
            foreach (IMachine m in LineKeeper.Machines)
            {
                if (m is Machines.MAP1stDieBonder)
                {
                    Log.SysLog.Info("DB:" + m.MacNo + " チェック開始");

                    //DBプロファイルと一致判定追加　2012.01.15
                    Profile prof = Profile.GetCurrentDBProfile(m.MacNo);
                    if (prof == null)
                    {
                        //DBプロファイルが存在しない場合はスルー
                        Log.SysLog.Info("DBプロファイルが登録されていない為投入不可 DB:" + m.MacNo);
                        continue;
                    }
                    BOM[] boms = Profile.GetBOM(prof.ProfileId);
                    if (!Array.Exists(boms, b => b.MaterialCd == ggcode))
                    {
                        //BOMと一致しないフレームは投入不可
                        Log.SysLog.Info("BOM不一致の為投入不可 DB:" + m.MacNo + " MatCD:" + ggcode);
                        continue;
                    }

                    //ロボット、橋が保持しているマガジンが存在する場合の判定追加 2018.07.04
                    List<VirtualMag> holdMagazines = GetHoldingMagazines();
                    List<VirtualMag> vmagazines = holdMagazines.Where(d => d.NextMachines.Exists(n => n == m.MacNo)).ToList();
                    if (vmagazines.Any() == true)
                    {
                        string logmsg = string.Join(",", vmagazines.Select(r => r.MacNo + "-" + r.MagazineNo));

                        // ロボット、橋が保持しているマガジンの次装置に、今から搬送しようとしている装置が存在する場合は移動無し。
                        Log.SysLog.Info(string.Format("向こう側の装置への搬送で、ロボット又は橋にその装置へ搬送中のマガジンがある為、スルーします。MagazineNo:{0} 移動先装置:{1} 移動先にあるマガジン情報:{2}",
                            mag.MagazineNo, m.MacNo, logmsg));
                        continue;
                    }

                    mag.NextMachines.Add(m.MacNo);

                    Log.SysLog.Info("DB:" + m.MacNo + " NextMachines:" + string.Join(",", mag.NextMachines) + " チェック完了");
                }
            }

            if (mag.NextMachines.Count == 0)
            {
                mag.NextMachines.Add(Route.GetAoiDischargeConveyor(mag.MacNo));
                Log.SysLog.Info(string.Format("MagazineNo:{0} {1}", mag.MagazineNo, "搬送可能設備がない為"));
            }

            //Log.SysLog.Info(string.Format("[アオイマガジン行先変更後]nextmachines:{0}", string.Join(",", mag.NextMachines)));
            #endregion
        }

        /// <summary>
        /// アーム移動のFrom、To設定 (装置番号、装置位置番号、バッファ位置番号)
        /// </summary>
        /// <returns></returns>
        public void setFromTo(string fromMacAddress, string fromLocationAddress, string fromBufferLocationAddress,
            string toMacAddress, string toLocationAddress, string toBufferLocationAddress)
        {
            #region アーム移動のFrom, To設定を搬送PLCへ送信
            try
            {
                this.Plc.SetWordAsDecimalData(FROM_MAC_WORD_ADDRESS(), int.Parse(fromMacAddress));
                this.Plc.SetWordAsDecimalData(FROM_LOCATION_WORD_ADDRESS(), int.Parse(fromLocationAddress));
                this.Plc.SetWordAsDecimalData(FROM_BUFFERLOCATION_WORD_ADDRESS(), int.Parse(fromBufferLocationAddress));

                this.Plc.SetWordAsDecimalData(TO_MAC_WORD_ADDRESS(), int.Parse(toMacAddress));
                this.Plc.SetWordAsDecimalData(TO_LOCATION_WORD_ADDRESS(), int.Parse(toLocationAddress));
                this.Plc.SetWordAsDecimalData(TO_BUFFERLOCATION_WORD_ADDRESS(), int.Parse(toBufferLocationAddress));
            }
            catch (InvalidCastException ex)
            {
                Log.RBLog.Error("FROM,TOのポイントに数値以外が設定されています", ex);
                throw ex;
            }
            #endregion
        }

        /// <summary>
        /// マガジン排出
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public override void PurgeHandlingMagazine(VirtualMag mag, string reason)
        {
            #region エラーチェック

            if (this.Plc.GetBit(MAGAZINE_EXIST_ROBOT_ARM()) == Mitsubishi.BIT_ON)
            {
                throw new RobotStatusException("ロボットがマガジンを持っているので処理を開始できません。手動で取り出して途中投入コンベヤに入れてください。", this);
            }
            if (IsQRReaderStageMagazineExists() == false)
            {
                //排出が指示されたのにマガジンが無い場合
                //何もせずに終了。
                return;
            }
            #endregion

            #region

            Location to = null;
            System.Threading.SemaphoreSlim sem = null;
            string semName = string.Empty;
            try
            {
                // PLCクラスのWatchBit関数の引数⇒Action関数とその関数の引数用
                Action actWorkRelayPlcData;
                VirtualMag refMag;

                List<int> machineList = Route.GetMachines(this.CarNo);
                IMachine dischargeCV = LineKeeper.GetMachine(LineKeeper.GetDischargeConveyor(machineList[0]).Value);
                to = new Location(dischargeCV.MacNo, Station.Loader);

                //行き先を排出CVへ
                mag.NextMachines.Clear();
                mag.NextMachines.Add(dischargeCV.MacNo);

                #region 橋へのmoveTo置き換え

                CarrierInfo fromCarrier = new CarrierInfo(this.CarNo);
                CarrierInfo toCarrier = Route.GetReachable(to);
                if (this.CarNo != toCarrier.CarNo)
                {
                    ////自動化ライン　橋へ
                    //to = Route.GetHandoverLocation(fromCarrier, toCarrier);

                    ////橋のmacnoを先頭に追加(橋に運び終えた後、このmacnoは削除している)
                    //mag.NextMachines.Insert(0, to.MacNo);

                    //自動化ライン　橋へ
                    to = null;
                    List<Location> lbridgeList = Route.GetHandoverLocationList(fromCarrier, toCarrier);
                    // 橋のいずれかが供給ONならその橋へ、OFFなら排出CVへ
                    // 条件追加：自ラインに排出CVが存在しない場合は橋の供給OFFでも橋を搬送先にする。
                    foreach (Location loc in lbridgeList)
                    {
                        IMachine ibridge = LineKeeper.GetMachine(loc.MacNo);
                        if (ibridge.IsRequireInput() == true)
                        {
                            to = loc;
                            break;
                        }

                    }
                    // 手の届く範囲の完成品排出CVVに置き換え
                    if (to == null)
                    {
                        List<int> compltDischargeConveyors = Route.GetCompltDischargeConveyors(mag.MacNo);
                        if (compltDischargeConveyors.Count() >= 1)
                        {
                            int compltDischargeCV = compltDischargeConveyors.First();
                            Location locCompltDischargeCV = new Location(compltDischargeCV, Station.Loader);
                            CarrierInfo compltDischargeCarrier = Route.GetReachable(locCompltDischargeCV);
                            if (fromCarrier.CarNo == compltDischargeCarrier.CarNo)
                            {
                                to = locCompltDischargeCV;
                            }
                        }
                    }
                    //橋のmacnoを先頭に追加(橋に運び終えた後、このmacnoは削除している)
                    if (to == null)
                    {
                        to = lbridgeList.First();
                    }
                    mag.NextMachines.Insert(0, to.MacNo);
                }

                #endregion

                carrierMutex.WaitOne();
                Log.RBLog.Info("排出処理ルーチン開始:" + reason);

                //エラーフラグを一度リセット
                this.Plc.SetBit(STAT_MOVE_FAULT(), 1, PLC.Common.BIT_OFF);
                this.Plc.SetBit(STAT_MOVE_FAULT_EMERGENCY(), 1, PLC.Common.BIT_OFF);

                //  PLCのコマンド受付チェック
                while (IsPLCReadyToCommand(STAT_ROBOT_COMMAND_READY()) == false)
                {
                    Thread.Sleep(3000); //OFFの場合は3秒待機
                }

                // 全オーブンの全扉閉中のPLC信号が1になるまで待つ
                reachOvensWatchBit();

                //  FROM TO 設定
                string macpoint = LineKeeper.GetMachine(to.MacNo).GetMacPoint(to.Station);
                string tobuffercode = LineKeeper.GetMachine(to.MacNo).GetToBufferCode();
                Log.RBLog.Info(string.Format("[移動アドレス再設定]{0} PurgeHandlingMagazine：『{1}』 → 『{2}』バッファ位置：『{3}』(to={4})", this.Name, this.QRReaderMacPoint, macpoint, tobuffercode, to.DirectoryPath));
                setFromTo(this.QRReaderMacPoint, this.QRReaderLocationPoint, this.GetQRReaderStageBufferCode(mag.MagazineNo),
                    macpoint, LineKeeper.GetMachine(to.MacNo).GetFromToCode(to.Station), tobuffercode);

                //  移動指令2　ON
                this.Plc.SetBit(REQ_ROBOT_MOVE2_MID2DST(), 1, PLC.Common.BIT_ON);

                //  リニア位置到達信号監視(待機中にPLC信号の中継処理も実施)
                Log.RBLog.Info(string.Format("[リニア移動監視開始]{0}", this.Name));
                //this.Plc.WatchBit(ROBOT_MOVE2_LINER_COMPLETE, 0, PLC.Common.BIT_ON);
                refMag = mag;
                actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, true, refMag.MagazineNo); };
                this.Plc.WatchBit(ROBOT_MOVE2_LINER_COMPLETE(), 0, PLC.Common.BIT_ON, actWorkRelayPlcData);
                Log.RBLog.Info(string.Format("[リニア移動監視終了]{0}", this.Name));

                //　装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT()) == PLC.Common.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    throw new RobotTimeoutException("ロボット動作異常停止", this);
                }

                //　装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT_EMERGENCY()) == PLC.Common.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    //エラーを返して監視ルーチン自体を停止
                    throw new RobotChuckMissException("ロボット動作異常停止", this);
                }

                // インターロックをかける
                if (this.SemaphoreSlimList.TryGetValue(macpoint, out semName) == true)
                {
                    if (LineKeeper.SemaphoreSlims.TryGetValue(semName, out sem) == true)
                    {
                        // 置きに行く場合は、60秒間に待機する
                        Log.RBLog.Info($"[インターロック解除待機]搬送先：『{macpoint}』, ロック名：『{semName}』 {this.Name}");
                        bool got = sem.Wait(60000);
                        if (got == false)
                        {
                            // この関数を抜ける時のfinallty内でロックを解除しないように変数をnullにする
                            sem = null;

                            // Todo 搬送先に選択された橋のインターロックが空くまで無限ループ
                            Log.RBLog.Info($"[排出]{this.Name} インターロック解除時間超過の為");
                            PurgeHandlingMagazine(mag, reason);

                            return;
                        }
                        Log.RBLog.Info($"[インターロックON]搬送先：『{macpoint}』, ロック名：『{semName}』 {this.Name}");
                    }
                }

                //  移動指令2　ON
                this.Plc.SetBit(REQ_ROBOT_MOVE2_COMPLETE(), 1, PLC.Common.BIT_ON);
                                
                // 搬送RB移動後の装置側の扉・マガジンガイド開待機
                if (LineKeeper.GetMachine(to.MacNo) is Oven)
                {
                    // 移動先がオーブンの場合、扉が開くまで待つ(待機中にPLC信号の中継処理も実施)
                    Log.RBLog.Info($"[オーブン扉開待機]{this.Name} -> {to.DirectoryPath}");
                    Oven ovenMac = (Oven)LineKeeper.GetMachine(to.MacNo);
                    //ovenMac.Plc.WatchBit(ovenMac.OvenDoorOpenStatusBitAddress, 0, Mitsubishi.BIT_ON);
                    refMag = mag;
                    actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, true, refMag.MagazineNo); };
                    ovenMac.Plc.WatchBit(ovenMac.OvenDoorOpenStatusBitAddress, 0, Mitsubishi.BIT_ON, actWorkRelayPlcData);

                    Log.RBLog.Info($"[ロボットMag供給・排出可能ON]{this.Name}");
                    this.Plc.SetBit(CAN_LOAD_UNLOAD_MAGAZINE(), 1, PLC.Common.BIT_ON);
                }
                else
                {
                    // モールド装置にも同様の処理が必要になった為、ワイヤボンダー限定の処理から全装置共通の処理に変更

                    // LineConfigの項目に設定がある場合、マガジンガイドが開くまで待つ (実Mag or 空Mag)
                    // 待機中にPLC信号の中継処理も実施
                    IMachine mac = LineKeeper.GetMachine(to.MacNo);
                    string loadCanBitAddress = mac.GetCanBitAddress(to.Station);

                    // LineConfigに設定がある装置のみ実施
                    if (string.IsNullOrWhiteSpace(loadCanBitAddress) == false)
                    {
                        Log.RBLog.Info($"[マガジンガイド開待機]{this.Name} -> {to.DirectoryPath}");
                        refMag = mag;
                        actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, true, refMag.MagazineNo); };
                        mac.Plc.WatchBit(loadCanBitAddress, 0, Mitsubishi.BIT_ON, actWorkRelayPlcData);
                        Log.RBLog.Info($"[ロボットMag供給・排出可能ON]{this.Name}");
                        this.Plc.SetBit(CAN_LOAD_UNLOAD_MAGAZINE(), 1, PLC.Common.BIT_ON);
                    }
                }

                Log.RBLog.Info(string.Format("[移動完了待機]{0}", this.Name));
                //  移動指令OFFになるまで待機(待機中にPLC信号の中継処理も実施)
                //this.Plc.WatchBit(REQ_ROBOT_MOVE2_MID2DST, 0, PLC.Common.BIT_OFF);
                refMag = mag;
                actWorkRelayPlcData = () => { WorkRelayPlcData(to, tobuffercode, true, refMag.MagazineNo); };
                this.Plc.WatchBit(REQ_ROBOT_MOVE2_MID2DST(), 0, PLC.Common.BIT_OFF, actWorkRelayPlcData);

                //　装置エラー発生のチェック（リリース指示後）
                if (this.Plc.GetBit(STAT_MOVE_FAULT()) == PLC.Common.BIT_ON
                    || this.Plc.GetBit(STAT_MOVE_FAULT_EMERGENCY()) == PLC.Common.BIT_ON)
                {
                    //工程リセットが押された場合にここに入る可能性がある
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    //エラーを返して監視ルーチン自体を停止
                    throw new RobotStatusException("ロボット動作異常停止\nマガジンを手で取り除いてください", this);
                }

                mag.PurgeReason = reason;

                //最終的な移動先にEnqueue
                LineKeeper.GetMachine(to.MacNo).Enqueue(mag, to.Station);

                Log.RBLog.Info(string.Format("[排出]{0} マガジンを排出：{1}", this.Name, reason));
            }
            finally
            {
                if (sem != null)
                {
                    Log.RBLog.Info($"[インターロック解除]ロック名：『{semName}』 {this.Name}");
                    sem.Release();
                }   

                carrierMutex.ReleaseMutex();
            }
            #endregion
        }

        //ログ用
        public void OutputSysLog(string message)
        {
            Log.SysLog.Info(string.Format("{0} {1}", this.Name, message));
        }


        #region MoveFrom

        public void MoveFrom(Location moveFrom, bool dequeueMoveFrom)
        {
            MoveFrom(moveFrom, null, dequeueMoveFrom, false, false, true);
        }

        public void MoveFrom(Location moveFrom, Location moveTo, bool dequeueMoveFrom, bool isEmptyMagazine, bool resetNextProcessIdToCurrentProfileFirstProcNo, bool isCheckQR)
        {
            #region MoveFromのメイン処理
            if (this.HoldingMagazines.Count >= HOLDMAGAZINE_MAX_CT)
            {
                // 搭載上限数分マガジンを積んでる場合は何もしない
                string errMsg = "";
                errMsg += $"[中断]{this.Name} {moveFrom.DirectoryPath}:QR読取位置  搭載中のマガジン数が上限(={HOLDMAGAZINE_MAX_CT}Mag)に達している為、MoveFrom処理を中断します。";
                errMsg += $"搭載マガジンNo=({string.Join(",", this.HoldingMagazines.Select(h => h.MagazineNo))})";
                Log.RBLog.Info(errMsg);
                return;
            }

            bool acceptQR = false;

            Log.RBLog.Info(string.Format("[MoveStart]{1} {0}:QR読取位置", moveFrom.DirectoryPath, this.Name));

            VirtualMag mag = null;

            if (dequeueMoveFrom == false)
            {
                mag = new VirtualMag();
                mag.CurrentLocation = moveFrom;
            }
            else
            {
                // この時点では仮想マガジンを削除しない
                // 要求フラグが消えないので次の作業完了が発生することを防止
                mag = LineKeeper.GetMachine(moveFrom.MacNo).Peek(moveFrom.Station);
                // 【N工場MAP J9・10不具合 修正】
                if (mag == null)
                {
                    OutputSysLog($"[MoveFrom中断]【return】{moveFrom.DirectoryPath}の仮想マガジンが無くなった。別の搬送で先に持っていかれた可能性有り。");
                    return;
                }
            }

            #region MAP1stダイボンダーの空マガジン搬送監視 (DB装置割り当て中のプロファイルチェック)
            if (resetNextProcessIdToCurrentProfileFirstProcNo == true)
            {
                //先頭工程向けの移動時には次工程IDをCurrentにセット。
                //空マガジン移動時にもNASCA開始登録が正常に動くようにする対策
                Profile prof = Profile.GetCurrentDBProfile(moveTo.MacNo);
                if (prof == null) throw new ApplicationException("使用可能なプロファイルが存在しません");

                //現在のプロファイルからワークフローを取得する。
                //1ワークフロー内にFirstSTが付く工程は1つだけの想定
                Process[] proclist = Process.GetWorkFlow(prof.TypeCd);
                foreach (Process proc in proclist)
                {
                    if (proc.FirstSt == true)
                    {
                        mag.ProcNo = proc.ProcNo;
                        break;
                    }
                }

                if (mag.ProcNo == null) throw new ApplicationException("ワークフロー内に先頭作業が存在しません");
            }
            #endregion

            if (mag.NextMachines.Count == 0 && isEmptyMagazine == false)
            {
                OutputSysLog(string.Format("次装置【受け取り】【return】：{0}：{1}", string.Join(",", mag.NextMachines), mag.MagazineNo));
                return;
            }


            // QR読込
            string readedQR;
            acceptQR = MoveOrgToQR(moveFrom, ref mag, out readedQR, isCheckQR);

            // QR照合の成否に因らずDequeue　否の場合もマガジンはライン外に出る想定
            // MoveOrgToQR内でExceptionが出た場合はキュー変更せず
            if (dequeueMoveFrom == true)
            {
                LineKeeper.GetMachine(moveFrom.MacNo).Dequeue(moveFrom.Station);
            }

            // 移動元の最終排出日時更新
            LineKeeper.GetMachine(moveFrom.MacNo).LastOutputTime = DateTime.Now;


            if (isCheckQR)
            {
                //  QRが一致していた場合は移動先装置へ移動
                if (acceptQR == true)
                {
                    //遠心沈降用マガジンのチェック
                    if (VirtualMag.IsECKMag(mag.MagazineNo))
                    {
                        //空マガジン投入CVへ投入した場合は、稼働中フラグの有無にかかわらず、排出CVへ振り替え (排出理由を入力)
                        if (LineKeeper.GetMachine(moveFrom.MacNo) is EmptyMagazineLoadConveyor)
                        {
                            mag.PurgeReason = $"[排出]マガジン番号:{mag.MagazineNo} 遠心沈降用マガジンが空マガジンCVに投入された為";
                        }
                    }
                }
                else
                {
                    if (moveTo != null && LineKeeper.GetMachine(moveTo.MacNo) is MAPBoardDischargeConveyor)
                    {
                        // MoveTo関数内で移動処理
                        //moveTo = MoveQRToDst(moveTo, isEmptyMagazine, ref mag);
                        mag.PurgeReason = string.Empty;
                    }
                    else
                    {
                        // QR不一致の場合はエラー画面表示
                        throw new QRMissMatchException(
                            string.Format("[QR照合不一致]{2} システム：{0} 読み込み：{1}", mag.MagazineNo, readedQR, this.Name),
                            mag.MagazineNo,
                            readedQR, moveFrom, this);
                    }
                }
            }

            if (isEmptyMagazine == true && mag.NextMachines.Count == 0)
            {
                // 空マガジン搬送の時は引数にて行先を指定しているのでNextMachineに入れる
                mag.NextMachines.Add(moveTo.MacNo);
            }

            LineKeeper.GetMachine(QRReaderMacNo).Enqueue(mag, Station.Unloader);

            #endregion
        }

        #endregion

        #region MoveTo

        public Location MoveTo(Location moveTo, VirtualMag mag, bool isEmptyMagazine, bool isCheckQR)
        {
            #region MoveToのメイン処理

            if (isEmptyMagazine == false)
            {
                // QR読取位置の仮想マガジン内容が途中で書き換わっている可能性があるため取り直す
                mag = VirtualMag.GetVirtualMag(mag.MacNo, mag.LocationId, mag.MagazineNo);
            }

            //アオイマガジンはQR読込時に移動先が更新されているのでMoveToに反映
            #region アオイマガジン行先変更処理
            bool isAoiMagazine = false;
            if (mag.MapAoiMagazineLotNo != null) isAoiMagazine = true;

            if (isAoiMagazine && mag.NextMachines.Count == 0)
            {
                if (LineKeeper.GetMachine(moveTo.MacNo) is MAPBoardDischargeConveyor)
                {
                    // アオイ基板排出マガジンの場合は、このパターンの為
                    mag.NextMachines.Add(moveTo.MacNo);
                }
                else
                {
                    Log.RBLog.Info($"排出ルーチンへ 理由：アオイ基板かつ次投入装置=該当無し");
                    PurgeHandlingMagazine(mag, mag.PurgeReason);
                    Log.RBLog.Info($"[MoveComplete]{this.Name} {moveTo.DirectoryPath}");
                    return LineKeeper.GetMachine(Route.GetAoiDischargeConveyor(this.QRReaderMacNo)).GetLoaderLocation();
                }
            }
            else if (isAoiMagazine && mag.NextMachines.Count >= 1)
            {
                if ((LineKeeper.GetMachine(moveTo.MacNo) is MAPBoardDischargeConveyor) == false || isEmptyMagazine == false)
                {
                    //排出理由が設定されている場合
                    if (!string.IsNullOrEmpty(mag.PurgeReason))
                    {
                        PurgeHandlingMagazine(mag, mag.PurgeReason);
                        Log.RBLog.Info($"[MoveComplete]{this.Name} {moveTo.DirectoryPath}");
                        return LineKeeper.GetMachine(Route.GetAoiDischargeConveyor(this.QRReaderMacNo)).GetLoaderLocation();
                    }

                    IMachine nextm = LineKeeper.SelectTopPriorityLineMachine(this, mag.NextMachines, mag);
                    if (nextm != null)
                    {
                        moveTo = nextm.GetLoaderLocation();
                        mag.NextMachines.Clear();
                        mag.NextMachines.Add(moveTo.MacNo);
                        mag.Updatequeue();
                    }
                    else
                    {
                        PurgeHandlingMagazine(mag, "搬送可能設備がない為");
                        Log.RBLog.Info($"[MoveComplete]{this.Name} {moveTo.DirectoryPath}");
                        return LineKeeper.GetMachine(Route.GetAoiDischargeConveyor(this.QRReaderMacNo)).GetLoaderLocation();
                    }
                }
            }
            #endregion

            //遠心沈降用マガジンは稼働フラグONなら指定の行先へOFFならモールド機アンローダー側へ振り替え
            #region 遠心沈降マガジン行先変更処理
            if (VirtualMag.IsECKMag(mag.MagazineNo))
            {
                // 排出理由がある場合(空マガジン投入CVへ投入した場合)、排出CVへ振り替え
                if (!string.IsNullOrEmpty(mag.PurgeReason))
                {
                    moveTo = new Location(Route.GetDischargeConveyor(mag.MacNo), Station.Loader);
                    Log.ApiLog.Info(mag.PurgeReason);
                }
                else
                {
                    //稼動中フラグのあるマガジンは行先の変更を行わない
                    Magazine svmag = Magazine.GetCurrent(mag.MagazineNo);
                    if (svmag == null)
                    {
                        //空マガジン供給要求有りで空マガジン数が少ないモールド装置のローダーに行先変更
                        //要求無しの場合は現在の行先（排出CV行きで仮想Mgは作られるはず）のまま                                               
                        List<IMachine> empMagInputMolds = LineKeeper.Machines
                            .Where(m => !(m is Mold3))
                            .Where(m => m is Mold)
                            .Where(m => m.IsRequireEmptyMagazine())
                            .Where(m => m.IsInputForbidden() == false)
                            .ToList();

                        if (empMagInputMolds.Count != 0)
                        {
                            // 空マガジン供給部の現在個数順に並び替え
                            empMagInputMolds = empMagInputMolds
                                .OrderBy(m => ((Mold)m).GetMagazineCount(Station.EmptyMagazineLoader)).ToList();

                            // ロボットQR部
                            Location from = new Location(this.QRReaderMacNo, Station.Unloader);

                            // 第1候補ハンドオーバー無しで到達可能
                            int idx = empMagInputMolds.FindIndex(m => Route.IsMyReachable(from, new Location(m.MacNo, Station.EmptyMagazineLoader)) == true);
                            if (idx == -1)
                            {
                                // 第2候補 ロボットと同じLineNoの装置
                                // 【N工場MAP J9・10不具合 修正】  m.LinearNo → m.LineNo
                                idx = empMagInputMolds.FindIndex(m => m.LineNo == this.LineNo);
                            }
                            if (idx == -1)
                            {
                                // 第3候補 リスト内の先頭の装置
                                idx = 0;
                            }

                            //moveTo = new Location(empMagInputMolds.First().MacNo, Station.EmptyMagazineLoader);
                            moveTo = new Location(empMagInputMolds[idx].MacNo, Station.EmptyMagazineLoader);
                            mag.NextMachines.Clear();
                            mag.NextMachines.Add(moveTo.MacNo);
                            mag.Updatequeue();
                        }
                    }
                }
            }
            #endregion

            //空マガジンで稼働中フラグがONの場合は排出CVへ振り替え
            #region 空マガジン行先変更処理
            if (isEmptyMagazine)
            {
                if (Magazine.GetCurrent(mag.MagazineNo) != null)
                {
                    moveTo = new Location(Route.GetDischargeConveyor(this.QRReaderMacNo), Station.Loader);
                    Log.ApiLog.Info($"[排出]マガジン番号:{mag.MagazineNo} 稼働中マガジンが空マガジンCVに投入された為");
                }
            }
            #endregion

            //排出CVの行先変更（排出CV以外の場合は元のMoveTo)
            moveTo = LineKeeper.SelectDischargeConveyor(mag, new Location(mag.MacNo, Station.Unloader), moveTo, isAoiMagazine);

            //MoveTo内部で移動先変更の可能性があるので行き先を取り直す
            moveTo = MoveQRToDst(moveTo, isEmptyMagazine, ref mag);
            if (moveTo == null)
                return null;

            Log.RBLog.Info($"[MoveComplete]{this.Name} {moveTo.DirectoryPath}");

            // 移動先を記録して搬送ロボットの位置(リニア特定)を特定する
            LastMoveTo = moveTo;

            if (LineKeeper.GetMachine(moveTo.MacNo) is LineBridge)
            {
                //ライン連結橋にマガジンを置いた時に橋のmacnoは削除する
                mag.NextMachines.RemoveAt(0);

                //他ラインデータの場合作業データはクリアしない。
                //（他ラインマガジンの移動はBridge出口→装置と装置→Bridge入口しかない前提）
                if (isEmptyMagazine == false)
                {
                    //最終投入日付を更新
                    //LineKeeper.GetMachine(moveTo.MacNo).LastMagazineSetTime = DateTime.Now;
                    this.SaveLastMagazineSetTimeXMLFile(moveTo.MacNo);

                    //他ラインのマガジンは移動時に作業開始時間を上書きしない
                    //装置→返送Bridgeで装置作業データを消さないようにするため
                }
            }
            else
            {
                //自ラインのデータの場合

                // InitializeWorkDataを変更すると影響範囲が広いので、一旦変数に排出理由を格納した後、移動先仮想マガジンに割り当てる。
                string purgeRason = mag.PurgeReason;
                mag.InitializeWorkData();
                if (isEmptyMagazine == false)
                {
                    //装置投入時点で開始時間を仮設定
                    //常温コンベヤや投入センサーの無い装置はこの数値が最終的な開始時間になる。
                    mag.WorkStart = DateTime.Now;

                    mag.PurgeReason = purgeRason;

                    // 最終投入日付を更新
                    OutputSysLog(string.Format("MoveTo"));
                    //LineKeeper.GetMachine(moveTo.MacNo).LastMagazineSetTime = DateTime.Now;
                    this.SaveLastMagazineSetTimeXMLFile(moveTo.MacNo);

                    // 搬送先工程を仮想マガジンに記録(アオイマガジン以外)
                    if (string.IsNullOrWhiteSpace(mag.MapAoiMagazineLotNo) == false)
                    {
                        Magazine svrMag = Magazine.GetCurrent(mag.MagazineNo);
                        if (svrMag != null)
                        {
                            AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);

                            if (Process.GetLastProcess(lot.TypeCd).ProcNo != svrMag.NowCompProcess)
                            {
                                Process nextproc = Process.GetNextProcess(svrMag.NowCompProcess, lot);
                                mag.ProcNo = nextproc.ProcNo;
                            }
                        }
                    }
                }
            }

            // 最終的な移動先にEnqueue
            LineKeeper.GetMachine(moveTo.MacNo).Enqueue(mag, moveTo.Station);

            // QR読み取り位置から仮想マガジンを削除
            mag.Dequeue(new Location(this.QRReaderMacNo, Station.Unloader));

            #endregion

            return moveTo;

        }

        #endregion

        #region QR読み取り位置の信号確認

        /// <summary>
        /// QR読取位置への搭載が禁止状態でないか(上限積載に達している)
        /// </summary>
        /// <returns></returns>
        public bool IsInputForbiddenQRReaderStage()
        {
            if (this.Plc.GetBit(MAGAZINE_EXIST_QR_STAGE1()) == PLC.Common.BIT_ON)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        /// <summary>
        /// QR読取位置に搭載上限(2マガジン)まで積まれているか確認
        /// </summary>
        /// <returns></returns>
        public bool IsQRReaderStageMagazineExists()
        {
            bool retv = false;
            if (this.Plc.GetBit(MAGAZINE_EXIST_QR_STAGE1()) == PLC.Common.BIT_ON)
            {
                retv = true;
            }
            return retv;
        }

        public string GetQRReaderStageBufferCode(string magazineNo)
        {
            foreach (VirtualMag mag in this.HoldingMagazines)
            {
                if (mag.MagazineNo == magazineNo)
                {
                    return mag.orderid.ToString();
                }
            }
            return "0";
        }

        #endregion

        #region 搬送時のロボットPLC⇔装置PLCの信号通信の中継タスク

        /// <summary>
        /// ロボット ⇔ 装置のPLC信号を中継する
        /// </summary>
        protected void WorkRelayPlcData(Location loc, string buffercode, bool isLoader, string magazineNo, int retryCt)
        {
            IMachine mac = LineKeeper.GetMachine(loc.MacNo);
            string bitData;

            try
            {
                #region 各種信号を中継  (搬送PLC → 装置PLC)

                // 連動運転中 (搬送PLC → 装置PLC)
                bitData = this.Plc.GetBit(SEND_MACHINE_READY());
                mac.SetBitMachineReady(bitData);

                // ロボットチャック開 (搬送PLC → 装置PLC)
                bitData = this.Plc.GetBit(DOING_OPEN_ROBOT_ARMS());
                mac.SetBitDoingOpenRobotArm(bitData);

                // ロボットチャック閉 (搬送PLC → 装置PLC)
                bitData = this.Plc.GetBit(DOING_CLOSE_ROBOT_ARMS());
                mac.SetBitDoingCloseRobotArm(bitData);

                // マガジン供給処理
                if (isLoader)
                {
                    // Mag供給中 (搬送PLC → 装置PLC)
                    bitData = this.Plc.GetBit(DOING_LOAD_MAGAZINE());

                    //if (mac is Oven || mac is ECK || mac is ECK2 || mac is ECK3 || mac is ECK4 || mac is ECK4MagExchanger || mac is Mold || mac is Mold2 || mac is Mold3)
                    //{
                    mac.SetAddressDoingLoadMagazine(loc.Station, buffercode, isLoader, bitData);
                    //}

                    // QR読取部のQRコード
                    string org = this.Plc.GetWord(QRWordAddressStart, QRWordAddressLength);
                    Log.RBLog.Info($"搬送MagNo(読み取り時)：{org}");

                    string qrcode;
                    #region 読取データの加工 (末字文字置き換え + 反転) ※Keyenceのみ
                    if (mac.Plc is RelayMachinePLC)
                    {
                        //Null文字, /EXT文字を置換
                        org = org.Replace("\0", "").Replace(((char)0x03).ToString(), "");
                        //取得文字を2文字ずつ反転する (文字数が奇数の場合、末字はそのまま)
                        qrcode = string.Empty;
                        for (int i = 0; i < org.Length - 1; i += 2)
                        {
                            qrcode += org[i + 1];
                            qrcode += org[i];
                        }
                        if (org.Length % 2 != 0)
                        {
                            qrcode += org.Last();
                        }
                    }
                    else
                    {
                        qrcode = org;
                    }
                    #endregion

                    Log.RBLog.Info($"搬送MagNo(書き込み時)：{qrcode}");
                    if (string.IsNullOrWhiteSpace(qrcode) == false)
                    {
                        // 搬送MagNo (搬送PLC → 装置PLC)
                        mac.SetAddressSendMagazineNo(loc.Station, qrcode);
                    }
                }
                // マガジン排出処理
                else
                {
                    // Mag排出中 (搬送PLC → 装置PLC)
                    bitData = this.Plc.GetBit(DOING_UNLOAD_MAGAZINE());
                    mac.SetAddressDoingLoadMagazine(loc.Station, buffercode, isLoader, bitData);
                }

                // 扉開許可 (搬送PLC → 装置PLC)
                bitData = this.Plc.GetBit(ROBOT_LINER_STOPPING());
                mac.SetBitDoorCanOpen(bitData);

                #endregion

                #region 各種信号を中継  (装置PLC → 搬送PLC)

                bitData = LineKeeper.GetRequireBitData(mac, loc.Station, buffercode, this);
                if (isLoader == true)
                {
                    // Mag供給要求 (搬送PLC → 装置PLC)
                    this.Plc.SetBit(REQ_LOAD_MAGAZINE(), 1, bitData);
                }
                else
                {
                    // Mag排出要求 (搬送PLC → 装置PLC)
                    this.Plc.SetBit(REQ_UNLOAD_MAGAZINE(), 1, bitData);
                }

                // Mag受渡し完了
                bitData = mac.GetMagazineDestinationCompltBitData();
                this.Plc.SetBit(COMPLT_DESTINATION_MAGAZINE(), 1, bitData);

                #endregion

                #region オーブン全扉閉信号の通信処理 (どれか1つでもオーブンの全扉閉がOFFなら搬送PLCにOFFを渡す)
                bitData = PLC.Common.BIT_ON;
                foreach (Oven oven in reachOvens)
                {
                    if (oven.GetOvenAllDoorCloseBitData() == PLC.Common.BIT_OFF)
                    {
                        bitData = PLC.Common.BIT_OFF;
                        break;
                    }
                }
                this.Plc.SetBit(CAN_MOVE_LINER(), 1, bitData);
                #endregion

                // 各遠心沈降機に必要な情報を通信する
                sendReachECK4Exchangers(loc);
            }
            catch (Exception)
            {
                Thread.Sleep(200);
                retryCt = retryCt + 1;
                Log.RBLog.Info($"中継失敗{retryCt}回目");
                WorkRelayPlcData(loc, buffercode, isLoader, magazineNo, retryCt);
                return;
            }
        }

        protected void WorkRelayPlcData(Location loc, string buffercode, bool isLoader, string magazineNo)
        {
            WorkRelayPlcData(loc, buffercode, isLoader, magazineNo, 0);
            return;
        }

        #endregion

        /// <summary>
        /// 全オーブンの全扉閉中のPLC信号が1になるまで待つ
        /// </summary>
        protected void reachOvensWatchBit()
        {
            foreach (Oven oven in reachOvens)
            {
                Log.RBLog.Info($"[オーブン全扉閉チェック]PLCアドレス：{oven.Plc.IPAddress},ポート：{oven.Plc.Port},信号アドレス：{oven.OvenAllDoorCloseBitAddress} -> BIT_ON");
                oven.Plc.WatchBit(oven.OvenAllDoorCloseBitAddress, 0, PLC.Common.BIT_ON);
            }
        }

        /// <summary>
        /// 手の届く全ての遠心沈降機と通信を行う
        /// </summary>
        protected void sendReachECK4Exchangers(Location loc)
        {
            foreach (ECK4MagExchanger e in reachECK4MagExchangers)
            {
                try
                {
                    // ロボットの信号取得
                    string bitData00 = this.Plc.GetBit(SEND_MACHINE_READY());
                    string bitData09 = this.Plc.GetBit(ROBOT_SAFETY_DOOR_CLOSE());
                    string bitData07 = this.Plc.GetBit(ROBOT_START_POSITION());
                    string bitData0A = this.Plc.GetBit(ROBOT_LINER_MOVE_COMPLETE());
                    string bitDataF0 = this.Plc.GetBit(REQ_ROBOT_MOVE_ORG2MID());
                    string bitDataF6 = this.Plc.GetBit(REQ_ROBOT_MOVE2_MID2DST());

                    #region  ﾛﾎﾞｯﾄ待機位置
                    e.Plc.SetBit(ECK4MagExchanger.ROBOT_START_POSITION_BIT_ADDR, 1, bitData07);
                    #endregion

                    #region PC→MD済み次工程沈降機信号
                    string bitData = e.Plc.GetBit(ECK4MagExchanger.MOLD_COMPLETE_NEXT_ECK_BIT_ADDR);
                    e.Plc.SetBit(ECK4MagExchanger.SEND_MOLD_COMPLETE_NEXT_ECK_BIT_ADDR, 1, bitData);
                    #endregion

                    #region 移載機RB動作禁止信号
                    bitData = Mitsubishi.BIT_OFF;
                    if (bitData09 == Mitsubishi.BIT_OFF)
                    {
                        bitData = Mitsubishi.BIT_ON;
                    }
                    else if ( (bitData00 == Mitsubishi.BIT_ON) || 
                              (bitData07 == Mitsubishi.BIT_ON && bitData0A == Mitsubishi.BIT_ON) )
                    {
                        // 引数(搬送先Location)の装置番号がチェック対象の遠心沈降機 (沈降部 or 移裁部)の場合
                        if (loc != null && (loc.MacNo == e.MacNo || loc.MacNo == e.CombinedECKMacNo))
                        {
                            if (bitDataF0 == Mitsubishi.BIT_ON || bitDataF6 == Mitsubishi.BIT_ON)
                            {
                                bitData = Mitsubishi.BIT_ON;
                            }
                        }
                    }
                    e.Plc.SetBit(ECK4MagExchanger.EXCHANGE_ROBOT_WORK_FORBIDDEN_BIT_ADDR, 1, bitData);                    
                    #endregion

                    #region 移載機RB動作禁止信号(沈降Mag側)
                    bitData = Mitsubishi.BIT_OFF;
                    // 引数(搬送先Location)の装置番号がチェック対象の遠心沈降機 (沈降部)の場合
                    if (loc != null && bitDataF0 == Mitsubishi.BIT_ON || bitDataF6 == Mitsubishi.BIT_ON)
                    {
                        if (loc.MacNo == e.CombinedECKMacNo && loc.Station == Station.Loader)
                        {
                            bitData = Mitsubishi.BIT_ON;
                        }
                        else if (loc.MacNo == e.MacNo && (loc.Station == Station.EmptyMagazineUnloader))
                        {
                            bitData = Mitsubishi.BIT_ON;
                        }
                    }
                    e.Plc.SetBit(ECK4MagExchanger.EXCHANGE_ROBOT_SIDE_LEFT_WORK_FORBIDDEN_BIT_ADDR, 1, bitData);
                    #endregion

                    #region 移載機RB動作禁止信号(通常Mag側)
                    // 引数(搬送先Location)の装置番号がチェック対象の遠心沈降機 (移裁部)の場合
                    bitData = Mitsubishi.BIT_OFF;
                    if (loc != null && loc.MacNo == e.MacNo && bitDataF0 == Mitsubishi.BIT_ON || bitDataF6 == Mitsubishi.BIT_ON)
                    {
                        switch (loc.Station)
                        {
                            // 搬送先 = 移載部 (空Mag供給) or (実Mag排出)
                            case Station.EmptyMagazineLoader:
                            case Station.Unloader1:
                            case Station.Unloader2:
                            case Station.Unloader3:
                            case Station.Unloader4:
                                bitData = Mitsubishi.BIT_ON;
                                break;
                        }
                    }
                    e.Plc.SetBit(ECK4MagExchanger.EXCHANGE_ROBOT_SIDE_RIGHT_WORK_FORBIDDEN_BIT_ADDR, 1, bitData);
                    #endregion

                }
                catch (Exception)
                {
                    // 通信が繋がらない場合は無視
                }
            }
        }
    }
}
