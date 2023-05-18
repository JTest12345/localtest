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
namespace ARMS3.Model.Carriers
{
    /// <summary>
    /// 搬送ロボット
    /// </summary>
    public class Robot : CarrierBase
    {
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
        /// QR読み込み位置
        /// </summary>
        public string QRReaderPoint { get; set; }

		/// <summary>
		/// 移動予定先位置
		/// </summary>
		public string MovePlanDestPointAddress { get; set; }

        /// <summary>
        /// 最終の移動先装置
        /// </summary>
        public Location lastMoveTo { get; set; }

        object robotMoveLock = new object();

        public Robot(string plcAddress, int plcPort, int carNo) 
        {
            this.Plc = new Mitsubishi(plcAddress, plcPort);

            //上位リンクモード
            this.Plc.SetBit(PC_READY, 1, Mitsubishi.BIT_ON);

            carrierMutex = new Mutex(false, string.Format("carrierMutex{0}", carNo));

			//自身が手が届く装置リストを取得
			reachMachines = new List<IMachine>();
			List<int> machines = Route.GetMachines(carNo);
			foreach (IMachine machine in LineKeeper.Machines)
			{
				if (machines.Exists(m => m == machine.MacNo))
				{
					reachMachines.Add(machine);
				}
			}

			this.HoldingMagazines = new List<VirtualMag>();
        }

        #region PLCアドレス

        /// <summary>
        /// 
        /// </summary>
        public const string PC_READY = "B0000FF";

        /// <summary>
        /// ロボット移動要求1
        /// </summary>
        public const string REQ_ROBOT_MOVE_ORG2MID = "B0000F0";

        /// <summary>
        /// ロボット移動2　リニア移動後の最終チェック異常
        /// </summary>
        public const string REQ_ROBOT_MOVE2_NASCA_ERROR = "B0000F7";

        /// <summary>
        /// ロボットBusy信号1
        /// 終了装置→QR読込
        /// </summary>
        public const string STAT_ROBOT_BUSY1 = "B0000E6";

        /// <summary>
        /// ロボットBusy信号2
        /// QR読込→開始装置
        /// </summary>
        public const string STAT_ROBOT_BUSY2 = "B0000E7";

        /// <summary>
        /// 搬送中のE1エラー詳細確認用
        /// </summary>
        public const string BIT_QR_HAS_MAGAZINE = "B0000E3";

        /// <summary>
        /// FROM設定アドレス
        /// </summary>
        public const string FROM_WORD_ADDRESS = "W0000F0";

        /// <summary>
        /// TO設定アドレス
        /// </summary>
        public const string TO_WORD_ADDRESS = "W0000F1";

        /// <summary>
        /// ロボット移動指令許可
        /// </summary>
        public const string STAT_ROBOT_COMMAND_READY = "B0000E0";

        /// <summary>
        /// ロボットがマガジンを掴んでいるか判定アドレス
        /// </summary>
        public const string MAGAZINE_EXIST_ROBOT_ARM = "B0000EE";

        /// <summary>
        /// QR読取部にマガジンがあるかの判定アドレス
        /// </summary>
        public const string MAGAZINE_EXIST_QR_STAGE = "B0000EF";

        /// <summary>
        /// 要求失敗、リトライOK
        /// ロボットのステータス変化
        /// </summary>
        public const string STAT_MOVE_FAULT = "B0000E1";

        /// <summary>
        /// 操作要求の失敗、緊急停止
        /// チャックミス、QR読取エラー
        /// </summary>
        public const string STAT_MOVE_FAULT_EMERGENCY = "B0000E2";

        /// <summary>
        /// ロボット移動要求2
        /// </summary>
        public const string REQ_ROBOT_MOVE2_MID2DST = "B0000F6";

        /// <summary>
        /// ロボット移動2　リニア移動完了
        /// </summary>
        public const string ROBOT_MOVE2_LINER_COMPLETE = "B0000E5";

        /// <summary>
        /// ロボット移動2　リニア移動後の最終チェック通過→マガジン投入
        /// </summary>
        public const string REQ_ROBOT_MOVE2_COMPLETE = "B0000F5";

        #endregion

        ///// <summary>
        ///// 異常発生
        ///// </summary>
        //public class RobotException : ApplicationException
        //{
        //    public ErrorHandleMethod Method { get; set; }

        //    public RobotException(string message) : base(message) 
        //    {

        //    }
        //}

        ///// <summary>
        ///// QR照合異常発生
        ///// </summary>
        //public class QRMissMatchException : RobotException
        //{
        //    public string VirtualMag { get; set; }
        //    public string RealMag { get; set; }
        //    public Location From { get; set; }

        //    public QRMissMatchException(string message, string virtualMag, string realMag, Location from, Robot robot)
        //        : base(message)
        //    {
        //        VirtualMag = virtualMag;
        //        RealMag = realMag;
        //        From = from;

        //        FrmErrHandle frm = new FrmErrHandle(robot, this);
        //        frm.ShowDialog();

        //        this.Method = frm.Method;
        //    }
        //}

        //public class RobotStatusException : RobotException
        //{
        //    public RobotStatusException(string message, Robot robot) : base(message) 
        //    {
        //        FrmErrHandle frm = new FrmErrHandle(robot, message);
        //        frm.ShowDialog();

        //        this.Method = frm.Method;
        //    }
        //}

        ///// <summary>
        ///// タイムアウト発生
        ///// </summary>
        //public class RobotTimeoutException : RobotException
        //{
        //    public RobotTimeoutException(string message, Robot robot) : base(message) 
        //    {
        //        FrmErrHandle frm = new FrmErrHandle(robot, message);
        //        frm.ShowDialog();

        //        this.Method = frm.Method;
        //    }
        //}

        ///// <summary>
        ///// チャックミス発生
        ///// </summary>
        //public class RobotChuckMissException : RobotException
        //{
        //    public RobotChuckMissException(string message, Robot robut) : base(message) 
        //    {
        //        FrmErrHandle frm = new FrmErrHandle(robut, message);
        //        frm.ShowDialog();

        //        this.Method = frm.Method;
        //    }
        //}

        protected override void concreteThreadWork()
        {
            try
            {
                bool canMove = false;
                Location moveFrom = null;
                Location moveTo = null;

                List<IMachine> machines = new List<IMachine>();

                //マガジン優先搬送装置取得
                //排出信号がON、アンローダーに仮想マガジン有の装置で優先フラグがONがあれば優先して搬送する
                //List<IMachine> ulMachines = reachMachines
                //	.Where(m => m.IsRequireOutput())
                //	.Where(m => m.Peek(m.GetUnLoaderLocation().Station) != null).ToList();
                //List<IMachine> magPriMachines = ulMachines.Where(m => m.Peek(m.GetUnLoaderLocation().Station).PriorityFg).ToList();
                //if (magPriMachines.Count != 0)
                //{
                //	machines = magPriMachines;
                //}
                //else 
                //{
                machines = reachMachines;
                //}

                //装置優先搬送装置取得
                var priorityList = machines.Select(m => m.Priority).Distinct().OrderBy(i => i);
                foreach (var cp in priorityList)
                {
                    //最終の移動先装置がある場合、同一優先度のリストに先立って要求処理
                    if (lastMoveTo != null && cp == LineKeeper.GetMachine(lastMoveTo.MacNo).Priority)
                    {
                        var lastMac = machines.Where(m => m.MacNo == lastMoveTo.MacNo);
                        canMove = LineKeeper.SelectMoveFrom(lastMac, out moveFrom, out moveTo, this);
                        if (canMove)
                        {
                            lastMoveTo = MoveFromTo(moveFrom, moveTo, true, false, false);
                            break;
                        }
                        else
                        {
                            bool canEmpMagMove = LineKeeper.SelectAndMoveEmptyMagazineRequest(lastMac);
                            if (canEmpMagMove) break;
                        }
                    }

                    //同一優先度装置リスト作成
                    var currentMacList = machines.Where(m => m.Priority == cp);
                    canMove = LineKeeper.SelectMoveFrom(currentMacList, out moveFrom, out moveTo, this);
                    if (canMove)
                    {
                        lastMoveTo = this.MoveFromTo(moveFrom, moveTo, true, false, false);
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
        }

        /// <summary>
        /// 仮想マガジンの移動
        /// </summary>
        /// <param name="from">移動元</param>
        /// <param name="to">移動先</param>
        public override Location MoveFromTo(Location moveFrom, Location moveTo, bool dequeueMoveFrom, bool isEmptyMagazine, bool resetNextProcessIdToCurrentProfileFirstProcNo, bool isCheckQR)
        {
            lock (this.robotMoveLock)
            {
                bool acceptQR = false;
                bool isAoiMagazine = false;

                Log.RBLog.Info(string.Format("[MoveStart]{2} {0}:{1}", moveFrom.DirectoryPath, moveTo.DirectoryPath, this.Name));

                VirtualMag mag = null;

                if (dequeueMoveFrom == false)
                {
                    mag = new VirtualMag();
                    mag.CurrentLocation = moveFrom;
                }
                else
                {
                    //この時点では仮想マガジンを削除しない
                    //要求フラグが消えないので次の作業完了が発生することを防止
                    mag = LineKeeper.GetMachine(moveFrom.MacNo).Peek(moveFrom.Station);
                }

                //アオイマガジン判定
                if (mag.MagazineNo == VirtualMag.MAP_AOI_MAGAZINE) isAoiMagazine = true;

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

                //QR読込
                string readedQR;
                acceptQR = MoveOrgToQR(moveFrom, moveTo, ref mag, out readedQR);

                //QR照合の成否に因らずDequeue　否の場合もマガジンはライン外に出る想定
                //MoveOrgToQR内でExceptionが出た場合はキュー変更せず
                if (dequeueMoveFrom == true)
                {
                    LineKeeper.GetMachine(moveFrom.MacNo).Dequeue(moveFrom.Station);
                }
				AddHoldingMagazine(mag);

                //移動元の最終排出日時更新
                if (isEmptyMagazine == false)
                {
                    LineKeeper.GetMachine(moveFrom.MacNo).LastOutputTime = DateTime.Now;
                }

                if (isCheckQR)
                {
                    //  QRが一致していた場合は移動先装置へ移動
                    if (acceptQR == true)
                    {
                        //アオイマガジンはQR読込時に移動先が更新されているのでMoveToに反映
                        #region アオイマガジン行先変更処理
                        if (isAoiMagazine && mag.NextMachines.Count >= 1)
                        {
                            //排出理由が設定されている場合
                            if (!string.IsNullOrEmpty(mag.PurgeReason))
                            {
                                PurgeHandlingMagazine(mag, mag.PurgeReason);
                                Log.RBLog.Info(string.Format("[MoveComplete]{2} {0}:{1}", moveFrom.DirectoryPath, moveTo.DirectoryPath, this.Name));
                                return LineKeeper.GetMachine(Route.GetAoiDischargeConveyor(moveFrom.MacNo)).GetLoaderLocation();
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
                                Log.RBLog.Info(string.Format("[MoveComplete]{2} {0}:{1}", moveFrom.DirectoryPath, moveTo.DirectoryPath, this.Name));
                                return LineKeeper.GetMachine(Route.GetAoiDischargeConveyor(moveFrom.MacNo)).GetLoaderLocation();
                            }
                        }
                        else if (isAoiMagazine && mag.NextMachines.Count == 0)
                        {
                            PurgeHandlingMagazine(mag, mag.PurgeReason);
                            Log.RBLog.Info(string.Format("[MoveComplete]{2} {0}:{1}", moveFrom.DirectoryPath, moveTo.DirectoryPath, this.Name));
                            return LineKeeper.GetMachine(Route.GetAoiDischargeConveyor(moveFrom.MacNo)).GetLoaderLocation();
                        }
                        #endregion

                        //遠心沈降用マガジンは稼働フラグONなら指定の行先へOFFならモールド機アンローダー側へ振り替え
                        #region 遠心沈降マガジン行先変更処理
                        if (VirtualMag.IsECKMag(mag.MagazineNo))
                        {
                            //空マガジン投入CVへ投入した場合は、稼働中フラグの有無にかかわらず、排出CVへ振り替え
                            if (LineKeeper.GetMachine(moveFrom.MacNo) is EmptyMagazineLoadConveyor)
                            {
                                moveTo = new Location(Route.GetDischargeConveyor(moveFrom.MacNo), Station.Loader);
                                Log.ApiLog.Info(string.Format("[排出]マガジン番号:{0} 遠心沈降用マガジンが空マガジンCVに投入された為", mag.MagazineNo));
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
                                        .ToList();

                                    if (empMagInputMolds.Count != 0)
                                    {
                                        empMagInputMolds = empMagInputMolds
                                            .OrderBy(m => ((Mold)m).GetMagazineCount(Station.EmptyMagazineLoader)).ToList();

                                        moveTo = new Location(empMagInputMolds.First().MacNo, Station.EmptyMagazineLoader);
                                        mag.NextMachines.Clear();
                                        mag.NextMachines.Add(moveTo.MacNo);
                                        mag.Updatequeue();
                                    }
                                }
                            }
                        }
                        #endregion

                        //空マガジンで稼働中フラグがONの場合は排出CVへ振り替え
						if (isEmptyMagazine)
						{
							if (Magazine.GetCurrent(mag.MagazineNo) != null)
							{
								moveTo = new Location(Route.GetDischargeConveyor(moveFrom.MacNo), Station.Loader);
								Log.ApiLog.Info(string.Format("[排出]マガジン番号:{0} 稼働中マガジンが空マガジンCVに投入された為", mag.MagazineNo));
							}
						}

                        //排出CVの行先変更（排出CV以外の場合は元のMoveTo)
                        moveTo = LineKeeper.SelectDischargeConveyor(mag, moveFrom, moveTo, isAoiMagazine);

                        if (Config.GetLineType == Config.LineTypes.NEL_MAP)
                        {
                            //途中投入CV⇒ダイサーに移動する場合、反転ユニット通過済設定用アドレスをfalse
                            if (LineKeeper.GetMachine(moveFrom.MacNo) is Machines.LoadConveyor && LineKeeper.GetMachine(moveTo.MacNo) is Machines.Cut)
                            {
                                ((Machines.Cut)LineKeeper.GetMachine(moveTo.MacNo)).SetMagReverseWorkBitAddress(false);
                            }
                        }

                        //MoveFromTo内部で移動先変更の可能性があるので行き先を取り直す
                        moveTo = MoveQRToDst(moveFrom, moveTo, isEmptyMagazine, ref mag);
                    }
                    else
                    {
                        if (LineKeeper.GetMachine(moveTo.MacNo) is MAPBoardDischargeConveyor)
                        {
                            moveTo = MoveQRToDst(moveFrom, moveTo, isEmptyMagazine, ref mag);
                        }
                        else
                        {
                            throw new QRMissMatchException(
                                string.Format("[QR照合不一致]{2} システム：{0} 読み込み：{1}", mag.MagazineNo, readedQR, this.Name),
                                mag.MagazineNo,
                                readedQR, moveFrom, this);
                        }
                    }
                }
                else
                {
                    //MoveFromTo内部で移動先変更の可能性があるので行き先を取り直す
                    moveTo = MoveQRToDst(moveFrom, moveTo, isEmptyMagazine, ref mag);
                }

                Log.RBLog.Info(string.Format("[MoveComplete]{2} {0}:{1}", moveFrom.DirectoryPath, moveTo.DirectoryPath, this.Name));

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

					//InitializeWorkDataを変更すると影響範囲が広いので、一旦変数に排出理由を格納した後、移動先仮想マガジンに割り当てる。
					string purgeRason = mag.PurgeReason;

                    mag.InitializeWorkData();
                    if (isEmptyMagazine == false)
                    {
                        //装置投入時点で開始時間を仮設定
                        //常温コンベヤや投入センサーの無い装置はこの数値が最終的な開始時間になる。
                        mag.WorkStart = DateTime.Now;

						mag.PurgeReason = purgeRason;

                        //最終投入日付を更新
                        //LineKeeper.GetMachine(moveTo.MacNo).LastMagazineSetTime = DateTime.Now;
                        this.SaveLastMagazineSetTimeXMLFile(moveTo.MacNo);
                    }
                }

                //最終的な移動先にEnqueue
                LineKeeper.GetMachine(moveTo.MacNo).Enqueue(mag, moveTo.Station);
				RemoveHoldingMagazine();

                return moveTo;
            }
        }
        public Location MoveFromTo(Location moveFrom, Location moveTo, bool dequeueMoveFrom, bool isEmptyMagazine, bool resetNextProcessIdToCurrentProfileFirstProcNo)
        {
            return MoveFromTo(moveFrom, moveTo, dequeueMoveFrom, isEmptyMagazine, resetNextProcessIdToCurrentProfileFirstProcNo, true);
        }

        /// <summary>
        /// QR読込部から投入先へ移動。
        /// 途中で移動先を変更する場合があるので最終的な移動先を返す。
        /// 内部で例外をThrowした場合は仮想マガジン変更せず
        /// </summary>
        /// <param name="to"></param>
        /// <param name="mag"></param>
        /// <returns></returns>
        public Location MoveQRToDst(Location from, Location to, bool isEmptyMagazine, ref VirtualMag mag)
        {
            #region エラーチェック

            if (this.Plc.GetBit(MAGAZINE_EXIST_ROBOT_ARM) == Mitsubishi.BIT_ON)
            {
                throw new RobotStatusException("ロボットがマガジンを持っているので処理を開始できません。手動で取り出して途中投入コンベヤに入れてください。", this);
            }
            if (this.Plc.GetBit(MAGAZINE_EXIST_QR_STAGE) == Mitsubishi.BIT_OFF)
            {
                throw new RobotStatusException("マガジンがQR読込位置にありません", this);
            }
            #endregion

            #region 橋、高効率へのmoveTo置き換え

			CarrierInfo fromCarrier = new CarrierInfo(this.CarNo);

            //To側に手が届かない場合はRoute.GetHandoverLocation関数で行き先の置き換え（ラインブリッジ、排出CV）
            CarrierInfo toCarrier = Route.GetReachable(to);
            if (toCarrier == null)
            {
                //高効率ライン　ラインアウト排出CVへ
                to = new Location(Route.GetAutoLineOutConveyor(from.MacNo), Station.Unloader);
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
                    //mag.NextMachines.Add(Route.GetDischargeConveyor(from.MacNo));
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
								
				to = Route.GetHandoverLocation(fromCarrier, toCarrier);

                //橋の供給ONなら橋へ、OFFなら排出CVへ
                // 条件追加：自ラインに排出CVが存在しない場合は橋の供給OFFでも橋を搬送先にする。
                IMachine bridge = LineKeeper.GetMachine(to.MacNo);
                int dischargeCV = Route.GetDischargeConveyor(from.MacNo);
                Location locDischargeCV = new Location(dischargeCV, Station.Loader);
                CarrierInfo dischargeCarrier = Route.GetReachable(locDischargeCV);
                // toDischargeCarrierのnullチェックは不要。 nullになるならRoute.GetDischargeConveyor(from.MacNo)でエラー発生している
                if (bridge.IsRequireInput() || fromCarrier.CarNo != dischargeCarrier.CarNo)
				{
					//橋のmacnoを先頭に追加(橋に運び終えた後、このmacnoは削除している)
					mag.NextMachines.Insert(0, to.MacNo);
				}
				else 
				{
					mag.NextMachines.Clear();
					mag.NextMachines.Add(dischargeCV);
                    to = locDischargeCV;
                }

                //空マガジンを識別する為、ライン連結橋に空マガジンを置いた時はフラグをON
                if (isEmptyMagazine)
                {
                    this.Plc.SetBit(((LineBridge)LineKeeper.GetMachine(to.MacNo)).EmptyMagazineInputBitAddress, 1, Mitsubishi.BIT_ON);
                }
            }
            #endregion
            
            #region

            Location retv;

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
                        // 【連結ラインの場合、現在位置に関係なく、若いラインのオーブンを優先してしまう不具合 修正】
                        Log.RBLog.Info($"[マガジンNo：{mag.MagazineNo}]プロファイル不一致のため次回投入可能装置から除外{to.MacNo}(次装置の残り台数={mag.NextMachines.Count()}台)");
                        mag.NextMachines.Remove(to.MacNo);
                        if (mag.NextMachines.Count >= 1)
                        {
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
                                    Log.RBLog.Info(string.Format("[再移動開始]{0}", this.Name));
                                    return MoveQRToDst(from, to, isEmptyMagazine, ref mag);
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
                //  ロボット操作Mutex取得
                carrierMutex.WaitOne();

                Log.RBLog.Info(string.Format("[開始]{1} QR読込→{0}", to.DirectoryPath, this.Name));
                Log.RBLog.Info(string.Format("[PLCコマンド受付確認]{0}", this.Name));
                while (IsPLCReadyToCommand(STAT_ROBOT_COMMAND_READY) == false)
                {
                    Thread.Sleep(3000); //OFFの場合は3秒待機
                }

                //エラーフラグを一度リセット
                this.Plc.SetBit(STAT_MOVE_FAULT, 1, Mitsubishi.BIT_OFF);
                this.Plc.SetBit(STAT_MOVE_FAULT_EMERGENCY, 1, Mitsubishi.BIT_OFF);

                Log.RBLog.Info(string.Format("[移動アドレス設定]{0}", this.Name));
                //  FROM TO 設定
                string toAddress = LineKeeper.GetMachine(to.MacNo).GetFromToCode(to.Station);
                setFromTo(this.QRReaderPoint, toAddress);
                //setFromTo(this.QRReaderPoint, LineKeeper.GetMachine(to.MacNo).GetFromToCode(to.Station));

                //  移動指令2　ON
                this.Plc.SetBit(REQ_ROBOT_MOVE2_MID2DST, 1, Mitsubishi.BIT_ON.ToString());

                //  NASCAへ開始登録
                bool canReleaseMagazine = false;
                if (mag.ProcNo == null || (mag.Origin.HasValue && mag.Origin != 0))
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

                    //ArmsApiResponse res = CommonApi.WorkStart(CommonApi.GetWorkStartOrder(mag, to.MacNo));
                    //if (res.IsError)
                    //{
                    //    Log.ApiLog.Info(res.Message);
                    //}
                    //else
                    //{
                    //    canReleaseMagazine = true;
                    //    Log.ApiLog.Info(string.Format("[作業開始登録完了]{0}", this.Name));
                    //}
                }
                
                // リニア位置到達信号監視
                Log.RBLog.Info(string.Format("[リニア移動監視開始]{0}", this.Name));
                this.Plc.WatchBit(ROBOT_MOVE2_LINER_COMPLETE, 0, Mitsubishi.BIT_ON);
                Log.RBLog.Info(string.Format("[リニア移動監視終了]{0}", this.Name));

                // 装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT) == Mitsubishi.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    Log.RBLog.Info(string.Format("[排出]{0}", this.Name));

                    //最終の開始実績を削除
                    if (canReleaseMagazine) deleteLastWorkStartRecord(mag);

                    PurgeHandlingMagazine(mag, "ロボット動作異常のため排出");
                    return new Location(Route.GetDischargeConveyor(to.MacNo), Station.Loader);
                }

                // 装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT_EMERGENCY) == Mitsubishi.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    //エラーを返して監視ルーチン自体を停止

                    //最終の開始実績を削除
                    if (canReleaseMagazine) deleteLastWorkStartRecord(mag);

                    throw new RobotChuckMissException("ロボット動作異常のため停止", this);
                }

                // 移動先が遠心沈降自動機の場合、リニア移動後に再度供給要求を確認
                // 装置上の搬送RBが搬送先置場にマガジンを置いている可能性がある為
                if (canReleaseMagazine == true)
                {
                    if (LineKeeper.GetMachine(to.MacNo) is ECK4 || LineKeeper.GetMachine(to.MacNo) is ECK4MagExchanger)
                    {
                        bool isRequireInput = false;
                        // 搬送先に指定しているローダーポイント(toAddress)の供給要求を再確認
                        if (LineKeeper.GetMachine(to.MacNo) is ECK4)
                        {
                            isRequireInput = ((ECK4)LineKeeper.GetMachine(to.MacNo)).IsRequireInput(toAddress);
                        }
                        if (LineKeeper.GetMachine(to.MacNo) is ECK4MagExchanger)
                        {
                            isRequireInput = ((ECK4MagExchanger)LineKeeper.GetMachine(to.MacNo)).IsRequireEmptyMagazine(toAddress);
                        }

                        // 搬送先指定位置の供給要求が移動中にOFFになっていた場合は、排出CVへ搬送する。
                        if (isRequireInput == false)
                        {
                            Log.RBLog.Info($"[排出]{this.Name}搬送予定先の供給要求が落ちています。搬送先「{to.DirectoryPath}」,搬送先ポイント「{toAddress}」");
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

                    this.Plc.SetBit(REQ_ROBOT_MOVE2_COMPLETE, 1, Mitsubishi.BIT_ON.ToString());
                    retv = to;
                    mag.CurrentLocation = to;
                }
                else
                {
                    Log.RBLog.Info("次回投入可能装置から除外" + to.MacNo);
                    mag.NextMachines.Remove(to.MacNo);

                    //　移動失敗フラグセット
                    Log.RBLog.Info(string.Format("[NASCA返答NGフラグ設定]{0}", this.Name));
                    this.Plc.SetBit(REQ_ROBOT_MOVE2_NASCA_ERROR, 1, Mitsubishi.BIT_ON.ToString());
                    //  移動指令OFFになるまで待機
                    Log.RBLog.Info(string.Format("[完了待機]{0}", this.Name));
                    this.Plc.WatchBit(REQ_ROBOT_MOVE2_MID2DST, 0, Mitsubishi.BIT_OFF);

                    if (mag.NextMachines.Count >= 1)
                    {
                        IMachine nextm = LineKeeper.SelectTopPriorityLineMachine(this, mag.NextMachines, mag);
                        if (nextm != null && nextm.MacNo != Route.GetDischargeConveyor(to.MacNo))
                        {
                            to = nextm.GetLoaderLocation();
                            retv = to;
                            Log.RBLog.Info(string.Format("[再移動開始]{0}", this.Name));
                            return MoveQRToDst(from, to, isEmptyMagazine, ref mag);
                        }
                    }

                    //次工程が移載機(反転)だった為、反転=ONにしているので運ぶ装置が変わったこのタイミングでアンローダマガジン反転フラグをfalseに変更
                    if (mag.ProcNo.HasValue == true)
                    {
                        Process currentProc = Process.GetProcess(mag.ProcNo.Value);
                        if (currentProc.WorkCd == Config.Settings.MagExchangerReverseWorkCd)
                        {
                            Machines.MagExchanger magExchanger = (Machines.MagExchanger)LineKeeper.Machines.Find(m => m is Machines.MagExchanger);
                            this.Plc.SetBit(magExchanger.UnloaderMagReverseBitAddress, 1, Mitsubishi.BIT_OFF);
                        }
                    }

                    Log.RBLog.Info(string.Format("[排出]{0}", this.Name));
                    PurgeHandlingMagazine(mag, "NASCA開始NGで投入可能装置無し");
                    return new Location(Route.GetDischargeConveyor(to.MacNo), Station.Loader);
                }

                //  移動指令OFFになるまで待機
                Log.RBLog.Info(string.Format("[移動完了待機]{0}", this.Name));
                this.Plc.WatchBit(REQ_ROBOT_MOVE2_MID2DST, 0, Mitsubishi.BIT_OFF);

                //　装置エラー発生のチェック（リリース指示後）
                if (this.Plc.GetBit(STAT_MOVE_FAULT) == Mitsubishi.BIT_ON)
                {
                    //NASCAへメッセージ
                    Log.RBLog.Info(string.Format("[ロボットタイムアウト検出]{0}", this.Name));

                    //最終の開始実績を削除
                    if (canReleaseMagazine) deleteLastWorkStartRecord(mag);

                    PurgeHandlingMagazine(mag, "ロボットタイムアウトの為排出");
                    return new Location(Route.GetDischargeConveyor(to.MacNo), Station.Loader);
                }

                //　装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT_EMERGENCY) == Mitsubishi.BIT_ON)
                {
                    //工程リセットが押された場合にここに入る可能性がある
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    //エラーを返して監視ルーチン自体を停止

                    //最終の開始実績を削除
                    if (canReleaseMagazine) deleteLastWorkStartRecord(mag);

                    throw new RobotStatusException("ロボット動作異常停止\nマガジンを手で取り除いてください", this);
                }

                // 搬送先がオーブン かつ 硬化信号を使用する場合は、登録した開始実績を削除
                // のちにオーブン内で硬化が始まったら開始実績登録処理を行う
                if (LineKeeper.GetMachine(to.MacNo) is Oven && ((Oven)LineKeeper.GetMachine(to.MacNo)).IsUseOvenProcessStatus)
                {
                    //最終の開始実績を削除
                    deleteLastWorkStartRecord(mag);
                    Log.RBLog.Info($"オーブン開始実績削除：{mag.MagazineNo}");
                }

                Log.RBLog.Info(string.Format("[移動完了]{0}", this.Name));

                return retv;
            }
            finally
            {
				if (Config.Settings.UseOvenProfiler)
                {
                    if (ovn != null)
                    {
                        ovn.SetInterlock(false);
                    }
                }
                carrierMutex.ReleaseMutex();
            }
            #endregion
        }

        /// <summary>
        /// QR読込部へマガジンを持って行き、照合処理。
        /// 成功時はTrue、異常時はFalseを返す
        /// 内部で例外をThrowした場合は仮想マガジン変更せず
        /// </summary>
        /// <returns></returns>
        public bool MoveOrgToQR(Location from, Location to, ref VirtualMag mag, out string readedQR)
        {
            #region エラーチェック

            if (this.Plc.GetBit(MAGAZINE_EXIST_ROBOT_ARM) == Mitsubishi.BIT_ON)
            {
                //仮想マガジンは削除されない。
                throw new RobotStatusException("ロボットがマガジンを持っているので処理を開始できません。手動で取り出して途中投入コンベヤに入れてください。", this);
            }
            if (this.Plc.GetBit(MAGAZINE_EXIST_QR_STAGE) == Mitsubishi.BIT_ON)
            {
                //仮想マガジンは削除されない。
                throw new RobotStatusException("ロボットがマガジンを持っているので処理を開始できません。手動で取り出して途中投入コンベヤに入れてください。", this);
            }
            #endregion

            #region

            try
            {
                // ロボット操作Mutex取得
                carrierMutex.WaitOne();

                Log.RBLog.Info(string.Format("[開始]{1} {0}→QR読込", from.DirectoryPath, this.Name));

                // PLCのコマンド受付チェック
                while (IsPLCReadyToCommand(STAT_ROBOT_COMMAND_READY) == false)
                {
                    Thread.Sleep(1000); //OFFの場合は3秒待機
                }

                // エラーフラグを一度リセット
                this.Plc.SetBit(STAT_MOVE_FAULT, 1, Mitsubishi.BIT_OFF);
                this.Plc.SetBit(STAT_MOVE_FAULT_EMERGENCY, 1, Mitsubishi.BIT_OFF);

				// 移動予定先 設定
				if (VirtualMag.IsECKMag(mag.MagazineNo))
				{
					//遠心沈降用空マガジンの移動は排出CVをToに設定している為、排出CVに移動し始めてしまうので"0"を書き込んでQRを読むまで移動しなくする。
					setPlanTo(this.MovePlanDestPointAddress, "0");
				}
				else
				{
					setPlanTo(this.MovePlanDestPointAddress, LineKeeper.GetMachine(to.MacNo).GetFromToCode(to.Station));
				}

                // FROM TO 設定
                setFromTo(LineKeeper.GetMachine(from.MacNo).GetFromToCode(from.Station), this.QRReaderPoint);

                // 移動指令1　ON
                this.Plc.SetBit(REQ_ROBOT_MOVE_ORG2MID, 1, Mitsubishi.BIT_ON.ToString());

                // 移動指令OFFになるまで待機
                Log.RBLog.Info(string.Format("[移動完了待機]{0}", this.Name));
                this.Plc.WatchBit(REQ_ROBOT_MOVE_ORG2MID, 0, Mitsubishi.BIT_OFF);
                Log.RBLog.Info(string.Format("[移動完了]{0}", this.Name));

                // 装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT) == Mitsubishi.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));

                    if (this.Plc.GetBit(MAGAZINE_EXIST_ROBOT_ARM) == Mitsubishi.BIT_ON ||
                        this.Plc.GetBit(BIT_QR_HAS_MAGAZINE) == Mitsubishi.BIT_ON)
                    {
                        // ロボットがマガジンを掴んでいてE1が立っている場合はQR読み取り異常と判定
                        readedQR = "読取異常";
                        return false;
                    }
                    else
                    {
                        // ロボットがマガジンを掴む前の通常停止なのでタイムアウトの挙動
                        throw new RobotTimeoutException("ロボット動作タイムアウト", this);
                    }
                }

                // 装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT_EMERGENCY) == Mitsubishi.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    // エラーを返して監視ルーチン自体を停止
                    // 仮想マガジンは削除されない。
                    throw new RobotChuckMissException("ロボット動作異常停止", this);
                }

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
                    else if (mag.MagazineNo == VirtualMag.MAP_AOI_MAGAZINE)
                    {
                        mag.MagazineNo = qrcode;
                        // アオイマガジンをつかんだ場合はSvrの情報に基づいて行先を更新
                        updateAoiMagazineInfo(mag, qrcode);
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            finally
            {
                carrierMutex.ReleaseMutex();
            }
            #endregion
        }

        /// <summary>
        /// エラー排出時に既に入っている開始レコードを削除するルーチン
        /// </summary>
        /// <param name="mag"></param>
        private void deleteLastWorkStartRecord(VirtualMag mag)
        {
            try
            {
                if (mag.ProcNo == null || (mag.Origin.HasValue && mag.Origin != 0))
                {
                    //空マガジンの移動の場合、または他ライン製品は開始登録の削除をしない
                    return;
                }

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
        }

        /// <summary>
        /// マガジン排出
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public override void PurgeHandlingMagazine(VirtualMag mag, string reason)
        {
            #region エラーチェック

            if (this.Plc.GetBit(MAGAZINE_EXIST_ROBOT_ARM) == Mitsubishi.BIT_ON)
            {
                throw new RobotStatusException("ロボットがマガジンを持っているので処理を開始できません。手動で取り出して途中投入コンベヤに入れてください。", this);
            }
            if (this.Plc.GetBit(MAGAZINE_EXIST_QR_STAGE) == Mitsubishi.BIT_OFF)
            {
                //排出が指示されたのにマガジンが無い場合
                //何もせずに終了。
                return;
            }
            #endregion

            #region

            try
            {
                List<int> machineList = Route.GetMachines(this.CarNo);
                IMachine dischargeCV = LineKeeper.GetMachine(Route.GetDischargeConveyor(machineList[0]));
                Location to = new Location(dischargeCV.MacNo, Station.Loader);

                //行き先を排出CVへ
                mag.NextMachines.Clear();
                mag.NextMachines.Add(dischargeCV.MacNo);

                #region 橋へのmoveTo置き換え

                CarrierInfo fromCarrier = new CarrierInfo(this.CarNo);
                CarrierInfo toCarrier = Route.GetReachable(to);
                if (this.CarNo != toCarrier.CarNo)
                {
                    //自動化ライン　橋へ
                    to = Route.GetHandoverLocation(fromCarrier, toCarrier);

                    //橋のmacnoを先頭に追加(橋に運び終えた後、このmacnoは削除している)
                    mag.NextMachines.Insert(0, to.MacNo);
                }

                #endregion

                carrierMutex.WaitOne();
                Log.RBLog.Info("排出処理ルーチン開始:" + reason);

                //エラーフラグを一度リセット
                this.Plc.SetBit(STAT_MOVE_FAULT, 1, Mitsubishi.BIT_OFF);
                this.Plc.SetBit(STAT_MOVE_FAULT_EMERGENCY, 1, Mitsubishi.BIT_OFF);

                //  PLCのコマンド受付チェック
                while (IsPLCReadyToCommand(STAT_ROBOT_COMMAND_READY) == false)
                {
                    Thread.Sleep(3000); //OFFの場合は3秒待機
                }
                                
                //  FROM TO 設定
                Log.RBLog.Info(string.Format("[移動アドレス再設定]{0}", this.Name));
                setFromTo(this.QRReaderPoint, LineKeeper.GetMachine(to.MacNo).GetFromToCode(to.Station));

                //  移動指令2　ON
                this.Plc.SetBit(REQ_ROBOT_MOVE2_MID2DST, 1, Mitsubishi.BIT_ON.ToString());

                //  リニア位置到達信号監視
                Log.RBLog.Info(string.Format("[リニア移動監視開始]{0}", this.Name));
                this.Plc.WatchBit(ROBOT_MOVE2_LINER_COMPLETE, 0, Mitsubishi.BIT_ON);
                Log.RBLog.Info(string.Format("[リニア移動監視終了]{0}", this.Name));

                //　装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT) == Mitsubishi.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    throw new RobotTimeoutException("ロボット動作異常停止", this);
                }

                //　装置エラー発生のチェック
                if (this.Plc.GetBit(STAT_MOVE_FAULT_EMERGENCY) == Mitsubishi.BIT_ON)
                {
                    Log.RBLog.Info(string.Format("[ロボット異常検出]{0}", this.Name));
                    //エラーを返して監視ルーチン自体を停止
                    throw new RobotChuckMissException("ロボット動作異常停止", this);
                }

                //  移動指令2　ON
                this.Plc.SetBit(REQ_ROBOT_MOVE2_COMPLETE, 1, Mitsubishi.BIT_ON.ToString());

                Log.RBLog.Info(string.Format("[完了待機]{0}", this.Name));
                //  移動指令OFFになるまで待機
                this.Plc.WatchBit(REQ_ROBOT_MOVE2_MID2DST, 0, Mitsubishi.BIT_OFF);

                //　装置エラー発生のチェック（リリース指示後）
                if (this.Plc.GetBit(STAT_MOVE_FAULT) == Mitsubishi.BIT_ON
                    || this.Plc.GetBit(STAT_MOVE_FAULT_EMERGENCY) == Mitsubishi.BIT_ON)
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
                carrierMutex.ReleaseMutex();
            }
            #endregion
        }

        /// <summary>
        /// ベーキング作業レコードを照会してアオイマガジンの行先を振り分け
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="qrcode"></param>
        private void updateAoiMagazineInfo(VirtualMag mag, string qrcode)
        {
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
        }

		public void AddHoldingMagazine(VirtualMag mag)
		{
			this.HoldingMagazines.Clear();
			this.HoldingMagazines.Add(mag);
		}

		public void RemoveHoldingMagazine()
		{
			this.HoldingMagazines.Clear();
        }

        /// <summary>
        /// アーム移動のFrom、To設定
        /// </summary>
        /// <returns></returns>
        public void setFromTo(string fromAddress, string toAddress)
        {
            try
            {
                this.Plc.SetWordAsDecimalData(FROM_WORD_ADDRESS, int.Parse(fromAddress));
                this.Plc.SetWordAsDecimalData(TO_WORD_ADDRESS, int.Parse(toAddress));
            }
            catch (InvalidCastException ex)
            {
                Log.RBLog.Error("FROM,TOのポイントに数値以外が設定されています", ex);
                throw ex;
            }
        }
    }
}
