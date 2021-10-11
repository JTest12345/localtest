using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.Machines;
using ARMS3.Model.Carriers;
using System.Threading;

namespace ARMS3.Model
{
    public class LineKeeper
    {
        #region Singleton
        private static LineKeeper instance;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private LineKeeper() 
        {
            LineKeeper.Machines = new List<IMachine>();
            LineKeeper.Carriers = new List<ICarrier>();

            LineKeeper.SemaphoreSlims = new Dictionary<string, SemaphoreSlim>();

            //lineconfig.xml読み込み
            LineConfig.LoadLineConfig();

            //最小スレッド数の設定
            //TODO 装置数から動的に算出
            int min, completetation;
            ThreadPool.GetMinThreads(out min, out completetation);
            if (min <= 200)
            {
                ThreadPool.SetMinThreads(200, 10);
            }
        }

        void isp_OnStopWork(object sender, MachineThreadEventArgs e)
        {
            if (e.Exception != null)
            {      
                System.Windows.Forms.MessageBox.Show(e.Exception.Message);
            }
            if (e.Status == MachineThreadEventArgs.ExitStatus.ThreadAlreadyRun)
            {
                System.Diagnostics.Debug.WriteLine("thread already run");
            }
        }

        public static LineKeeper GetInstance()
        {
            if (instance == null)
            {
                instance = new LineKeeper();
            }
            return instance;
        }
        #endregion

        /// <summary>
        /// 装置リスト
        /// </summary>
        public static List<IMachine> Machines;

        /// <summary>
        /// 搬送設備リスト
        /// </summary>
        public static List<ICarrier> Carriers;

        /// <summary>
        /// オーブンプロファイル切替
        /// </summary>
        public static OvenProfiler OvenProfileChanger;

        /// <summary>
        /// 最終のプロファイル予約更新時間
        /// </summary>
        public static DateTime LastProfileReserve;

        /// <summary>
        /// インターロックリスト (中間台搬送用)
        /// </summary>
        public static Dictionary<string, SemaphoreSlim> SemaphoreSlims;

        /// <summary>
        /// 指定装置の処理開始
        /// </summary>
        /// <param name="plantcd"></param>
        public void Run(string plantcd)
        {
            IMachine mb = Machines.Where(m => m.PlantCd == plantcd).FirstOrDefault();
            if (mb != null) mb.RunWork();
        }

        /// <summary>
        /// 全装置の通常停止要求ON
        /// </summary>
        public void StopAll()
        {
            foreach (IMachine m in Machines)
            {
                m.StopRequested = true;
            }
        }

        public static IMachine GetMachine(int macno)
        {
            IMachine machine = LineKeeper.Machines.Find(m => m.MacNo == macno);
            if (machine == null)
            {
                throw new ApplicationException(string.Format("lineconfigに設定されていない または 設備マスタが有効ではない装置です。装置NO:{0}", macno));
            }
            return machine;
        }

        public static ICarrier GetCarrier(int carno)
        {
            ICarrier carrier = LineKeeper.Carriers.Find(c => c.CarNo == carno);
            if (carrier == null) 
            {
                throw new ApplicationException(string.Format("lineconfigに設定されていない搬送機です。装置NO:{0}", carno));
            }
            return carrier;
        }

        public static LineBridge GetReachBridge(int macno)
        {
            int thisCarno = Route.GetReachable(new Location(macno, Station.Loader)).CarNo;
            List<IMachine> bridges = LineKeeper.Machines.Where(m => m is LineBridge).ToList();

            int index = bridges.FindIndex(b => Route.GetReachable(b.GetLoaderLocation()).CarNo == thisCarno);
            if (index == -1)
            {
                return null;
            }
            else
            {
                return (LineBridge)bridges[index];
            }
        }

        public static List<LineBridge> GetReachBridges(int macno)
        {
            List<LineBridge> retv = new List<LineBridge>();

            int thisCarno = Route.GetReachable(new Location(macno, Station.Loader)).CarNo;
            List<IMachine> bridges = LineKeeper.Machines.Where(m => m is LineBridge).ToList();

            foreach(IMachine bridge in bridges)
            {
                if (Route.GetReachable(bridge.GetLoaderLocation()).CarNo == thisCarno)
                {
                    retv.Add((LineBridge)bridge);
                }
            }

            return retv;

            //int index = bridges.FindIndex(b => Route.GetReachable(b.GetLoaderLocation()).CarNo == thisCarno);
            //if (index == -1)
            //{
            //    return null;
            //}
            //else
            //{
            //    return (LineBridge)bridges[index];
            //}
        }


        /// <summary>
        /// 排出CVで供給信号がONのCVを返す
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static int? GetDischargeConveyor(int macno)
        {
            List<int> conveyors = Route.GetDischargeConveyors(macno);
            foreach (int conveyor in conveyors)
            {
                if (LineKeeper.GetMachine(conveyor).IsRequireInput())
                    return conveyor;
            }

            return null;
            // 全て供給信号OFFの場合はリストの1つ目を返す　※Robotクラスで再度供給確認し、排出できない場合は待機する
            //return conveyors.FirstOrDefault();
        }

        /// <summary>
        /// 完成品CVで供給信号がONのCVを返す
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static int? GetAutoLineOutConveyors(int macno)
        {
            List<int> conveyors = Route.GetAutoLineOutConveyors(macno);
            foreach (int conveyor in conveyors)
            {
                if (LineKeeper.GetMachine(conveyor).IsRequireInput())
                    return conveyor;
            }

            return null;
            // 全て供給信号OFFの場合はリストの1つ目を返す　※Robotクラスで再度供給確認し、排出できない場合は待機する
            //return conveyors.FirstOrDefault();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="macno">装置番号</param>
        /// <returns></returns>
        public static OvenProfiler SetDBOvenProfileReserve(int macno, int profileno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="macno">装置番号</param>
        /// <returns></returns>
        public static OvenProfiler SetMDOvenProfileReserve(int macno, int profileno)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// キャリアの特定をした後、移動処理へ
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void MoveFromTo(Location from, Location to, bool dequeueMoveFrom, bool isEmptyMagazine, bool resetNextProcessIdToCurrentProfileFirstProcNo, bool isCheckQR)
        {
            CarrierInfo carrier = Route.GetReachable(from);

            ICarrier currentCar = Carriers.Find(c => c.CarNo == carrier.CarNo);
                        
            currentCar.MoveFromTo(from, to, dequeueMoveFrom, isEmptyMagazine, resetNextProcessIdToCurrentProfileFirstProcNo, isCheckQR);
        }
        public static void MoveFromTo(Location from, Location to, bool dequeueMoveFrom, bool isEmptyMagazine, bool resetNextProcessIdToCurrentProfileFirstProcNo) 
        {
            MoveFromTo(from, to, dequeueMoveFrom, isEmptyMagazine, resetNextProcessIdToCurrentProfileFirstProcNo, true);
        }

        /// <summary>
        /// 与えられたリストでRequireInputが立っている最優先装置を取得
        /// 供給可能が一つも無い場合はnull
        /// </summary>
        /// <param name="maclist"></param>
        /// <returns></returns>
        public static IMachine SelectTopPriorityMachine(List<int> maclist, VirtualMag mag, ICarrier carrier)
        {
            IMachine tmpNext = null;
            
            foreach (int macno in maclist)
            {
                IMachine machine = LineKeeper.GetMachine(macno);

                if (machine.IsAutoLine || machine.IsAgvLine)
                {
                    //供給要求ONかつ、Mag指定での投入判定OK
                    // 装置隣接CV(棚)の有無に対して、供給要求先を使い分ける
                    bool isRequire = false;
                    if(machine.IsConveyor == false)
                    {
                        if (machine.IsRequireInput() == true && machine.CanInput(mag))
                        {
                            isRequire = true;
                        }
                    }
                    else
                    {
                        if (machine.IsRequireConveyorInput(carrier.Plc) == true && machine.CanInput(mag))
                        {
                            isRequire = true;
                        }
                    }
                    //if (machine.IsRequireInput() == true && machine.CanInput(mag))
                    if (isRequire == true)
                    {
                        #region 車載自動化のロボットが運転前に作業開始前チェックをするモード
                        if (Config.Settings.IsBeforeDriveWorkStartCheckerMode)
						{
							Magazine svrMag = Magazine.GetCurrent(mag.MagazineNo);
							if (svrMag != null)
							{
                                if (machine is DischargeConveyor || machine is AutoLineOutConveyor || machine is LineBridge)
                                {
                                    //排出CV, ラインアウト排出CV, 橋の場合、作業開始前照合はしない
                                }
                                else
                                {
                                    AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);

                                    Order order = CommonApi.GetWorkStartOrder(mag, machine.MacNo);
                                    order.LotNo = svrMag.NascaLotNO;

                                    Process p = Process.GetProcess(mag.ProcNo.Value);
                                    MachineInfo m = MachineInfo.GetMachine(order.MacNo);

                                    string msg = string.Empty;
                                    if (WorkChecker.IsErrorBeforeStartWork(lot, m, order, p, out msg))
                                    {
                                        Log.RBLog.Info(string.Format("【作業開始前照合NGの為、供給対象から除外】装置：{0} マガジン番号:{1} 理由：{2}", machine.Name, mag.MagazineNo, msg));

                                        int dischargeCV = Route.GetDischargeConveyor(mag.MacNo);
                                        if (mag.NextMachines.Exists(n => n == dischargeCV) == false)
                                        {
                                            mag.NextMachines.Add(dischargeCV);
                                            mag.Updatequeue();
                                        }

                                        continue;
                                    }
                                }
							}
						}
                        #endregion

                        // 遠心沈降へ供給可能な場合は、ダミーマガジンの投入タイマを一時停止する
                        Machines.ECK eckm = machine as Machines.ECK;
                        if (eckm != null) eckm.SetDummyMagTimerOff();

                        // オーブンの場合はLoader1が埋まった装置が見つかった時点で終了
                        if (machine is Machines.Oven)
                        {
                            VirtualMag ld1 = machine.Peek(Station.Loader1);
                            if (ld1 != null)
                            {
								tmpNext = machine;
								break;
                            }
                        }

						// 常温待機CVの場合は他の常温待機CVで、同じ待機時間のマガジンがあって、空きがあればそちらを優先する
						// 同じ待機時間、空きがあるかの判定はMoldConveyor.CanInput側で判定
						if (machine is Machines.MoldConveyor) 
						{
							if (machine.Peek(Station.Loader) != null) 
							{
								tmpNext = machine;
								break;
							}
						}

						// マガジンを供給した最古の装置に搬送する
						if (tmpNext != null)
						{	
							if (tmpNext.LastMagazineSetTime <= machine.LastMagazineSetTime)
							{
								continue;
							}
                        }

                        if (machine is DischargeConveyor && tmpNext != null)
                        {
                            continue;
                        }

                        tmpNext = machine;
                    }
                }
                else
                {
                    Log.RBLog.Info($"【高生産性装置決定:{machine.MacNo}");

                    //高効率装置の場合、無条件で格納
                    tmpNext = machine;
                }
            }

            if (tmpNext is DischargeConveyor && string.IsNullOrEmpty(mag.PurgeReason))
            {
                // 遠沈の空マガジンの場合は、排出理由を記載しない
                bool isEmpECKMag = false;
                if (VirtualMag.IsECKMag(mag.MagazineNo))
                {
                    Magazine svrMag = Magazine.GetCurrent(mag.MagazineNo);
                    if (svrMag == null) isEmpECKMag = true;
                }

                if (isEmpECKMag == false)
                {
                    mag.PurgeReason = "【製造条件不一致】装置が供給状態で、BOM照合、ブレンドコード照合などに一致した設備が見つかりません。理由：" + mag.PurgeReason;
                    if (mag.PurgeReason.Length >= 200) mag.PurgeReason = mag.PurgeReason.Substring(0, 200);
                    mag.Updatequeue();
                }
            }

            return tmpNext;
        }
        
        /// <summary>
        /// SelectTopPriorityMachineを自ライン優先で取得
        /// </summary>
        /// <param name="maclist"></param>
        /// <param name="mag"></param>
        /// <returns></returns>
        public static IMachine SelectTopPriorityLineMachine(ICarrier carrier, List<int> maclist, VirtualMag mag) 
        {
            // 【N工場MAP J9・10不具合 修正】
            List<int> targetReachMachines = new List<int>();
            List<int> targetOtherMachines = new List<int>();
            List<int> targetNonReachSameLineMachines = new List<int>();
            List<int> targetNonReachOtherLineMachines = new List<int>();

            List<int> reachMachines = Route.GetMachines(carrier.CarNo);
            foreach(int mac in maclist)
            {
                IMachine iMac = LineKeeper.GetMachine(mac);
                if (iMac == null) continue;

                if (reachMachines.Exists(m => m == mac))
                {
                    targetReachMachines.Add(mac);
                }
                else if (iMac.LineNo == carrier.LineNo)
                {
                    targetNonReachSameLineMachines.Add(mac);
                }
                else
                {
                    targetNonReachOtherLineMachines.Add(mac);
                }
            }
            targetOtherMachines.AddRange(targetNonReachSameLineMachines);
            targetOtherMachines.AddRange(targetNonReachOtherLineMachines);

            IMachine rMachine = SelectTopPriorityMachine(targetReachMachines, mag, carrier);
            if (rMachine == null)
            {
                if (LineKeeper.GetMachine(mag.MacNo).IsAgvLine)
                {
                    if (Config.Settings.RelayOtherLineIfReachMachineIsBusy == false)
                    {
                        if (targetOtherMachines.Count != 0 && targetReachMachines.Count == 0)
                        {
                            Log.RBLog.Info("【移動先変更】別ライン装置への搬送の為、完成品CV(AGV受渡装置含む)へ");
                            // 別ライン装置への搬送の場合、排出CV(完成品)へ　※全て供給OFFなら排出CV(異常)へ
                            int? conveyor = LineKeeper.GetAutoLineOutConveyors(mag.MacNo);
                            if (conveyor.HasValue == false)
                            {
                                conveyor = LineKeeper.GetDischargeConveyor(mag.MacNo);
                                mag.PurgeReason = "【移動先変更】完成品CV(AGV受渡装置含む)が満杯の為、移動先を異常CVへ変更します。";
                                mag.Updatequeue();
                            }

                            return GetMachine(conveyor.Value);
                        }
                    }
                    else
                    {
                        if (targetOtherMachines.Count != 0)
                        {
                            Log.RBLog.Info("【移動先変更】別ライン装置への搬送の為、完成品CV(AGV受渡装置含む)へ");

                            // 別ライン装置への搬送の場合、排出CV(完成品)へ　※全て供給OFFなら排出CV(異常)へ
                            int? conveyor = LineKeeper.GetAutoLineOutConveyors(mag.MacNo);
                            if (conveyor.HasValue == false)
                            {
                                conveyor = LineKeeper.GetDischargeConveyor(mag.MacNo);
                                mag.PurgeReason = "【移動先変更】完成品CV(AGV受渡装置含む)が満杯の為、移動先を異常CVへ変更します。";
                                mag.Updatequeue();
                            }

                            return GetMachine(conveyor.Value);
                        }
                    }
                }
                else
                {
                    return SelectTopPriorityMachine(targetOtherMachines, mag, carrier);
                }
            }
            return rMachine;
        }

        #region SelectMoveFrom

        /// <summary>
        /// 与えられた同優先度装置の内、装置の実空マガジン処理排出要求がある装置を見つけた場合、
        /// 再優先のものを返す
        /// </summary>
        /// <param name="maclist"></param>
        /// <param name="moveFrom"></param>
        /// <param name="moveTo"></param>
        /// <returns></returns>
        public static bool SelectMoveFrom(IEnumerable<IMachine> samePriorityMaclist, out Location moveFrom, out Location moveTo, ICarrier carrier, Location lastMoveTo)
        {
            bool canMove = false;
            moveTo = null;
            moveFrom = null;

            // 作業完了日時が古いマガジンを保持している装置から順に搬送する
            Dictionary<IMachine, DateTime> finishedMacList = new Dictionary<IMachine, DateTime>();
            foreach (IMachine m in samePriorityMaclist)
            {
                if (IsRequireOutput(m) == true)
                {
                    VirtualMag ulmag = m.Peek(m.GetUnLoaderLocation().Station);
                    if (ulmag == null)
                    {
                        //アンローダーに仮想マガジンが存在しない装置の搬送は最後に
                        finishedMacList.Add(m, DateTime.MaxValue);
                    }
                    else
                    {
                        if (ulmag.WorkComplete.HasValue)
                        {
                            finishedMacList.Add(m, ulmag.WorkComplete.Value);
                        }
                        else
                        {
                            //作業完了日時の存在しない仮想マガジンを保持している装置の搬送は最後に
                            finishedMacList.Add(m, DateTime.MaxValue);
                        }
                    }
                }
            }

            if (finishedMacList.Count == 0)
            {
                return false;
            }

            List<IMachine> finishedOrderMacList = new List<IMachine>();
            if (lastMoveTo != null)
            {

                IMachine lastMoveMac = LineKeeper.GetMachine(lastMoveTo.MacNo);

                // 最終移動先記憶がある場合、同一リニア、作業完了日時順に並び替え
                finishedOrderMacList.AddRange(finishedMacList
                    .Where(m => m.Key.LinearNo == lastMoveMac.LinearNo)
                    .OrderBy(m => m.Value).Select(m => m.Key));

                finishedOrderMacList.AddRange(finishedMacList
                    .Where(m => m.Key.LinearNo != lastMoveMac.LinearNo)
                    .OrderBy(m => m.Value).Select(m => m.Key));
            }
            else
            {
                // 作業完了日時順に並び替え
                finishedOrderMacList = finishedMacList.OrderBy(m => m.Value).Select(m => m.Key).ToList();
            }

            foreach (IMachine m in finishedOrderMacList)
            {
                Location tmpMoveTo = null;
                bool tmpCanMove = HasMoveTo(m.GetUnLoaderLocation(), out tmpMoveTo, carrier);

                //移動可能な場合はFromToを保持
                if (tmpCanMove == true)
                {
                    canMove = true;
                    moveFrom = m.GetUnLoaderLocation();
                    moveTo = tmpMoveTo;
                    break;
                }
            }

            return canMove;
        }

        public static bool SelectMoveFrom(IEnumerable<IMachine> samePriorityMaclist, out Location moveFrom, out Location moveTo, ICarrier carrier)
        {
            return SelectMoveFrom(samePriorityMaclist, out moveFrom, out moveTo, carrier, null);
        }

        ///// <summary>
        ///// 与えられた同優先度装置の内、実空マガジン処理排出要求がある装置を見つけた場合、
        ///// 再優先のものを返す
        ///// </summary>
        ///// <param name="maclist"></param>
        ///// <param name="moveFrom"></param>
        ///// <param name="moveTo"></param>
        ///// <returns></returns>
        //public static bool SelectMoveFrom(IEnumerable<IMachine> samePriorityMaclist, out Location moveFrom, out Location moveTo, ICarrier carrier)
        //{
        //    bool canMove = false;
        //    moveTo = null;
        //    moveFrom = null;

        //    //作業完了日時が古いマガジンを保持している装置から順に搬送する

        //    Dictionary<IMachine, DateTime> orderMachines = new Dictionary<IMachine, DateTime>();
        //    foreach (IMachine m in samePriorityMaclist)
        //    {
        //        if (m.IsRequireOutput() == true)
        //        {
        //            VirtualMag ulmag = m.Peek(m.GetUnLoaderLocation().Station);
        //            if (ulmag == null)
        //            {
        //                //アンローダーに仮想マガジンが存在しない装置の搬送は最後に
        //                orderMachines.Add(m, DateTime.MaxValue);
        //            }
        //            else
        //            {
        //                if (ulmag.WorkComplete.HasValue)
        //                {
        //                    orderMachines.Add(m, ulmag.WorkComplete.Value);
        //                }
        //                else
        //                {
        //                    //作業完了日時の存在しない仮想マガジンを保持している装置の搬送は最後に
        //                    orderMachines.Add(m, DateTime.MaxValue);
        //                }
        //            }
        //        }
        //    }
        //    List<IMachine> askMachines = orderMachines.OrderBy(m => m.Value).Select(m => m.Key).ToList();
        //    foreach(IMachine m in askMachines)
        //    {
        //        Location tmpMoveTo = null;
        //        bool tmpCanMove = HasMoveTo(m.GetUnLoaderLocation(), out tmpMoveTo, carrier);

        //        //移動可能な場合はFromToを保持
        //        if (tmpCanMove == true)
        //        {
        //            canMove = true;
        //            moveFrom = m.GetUnLoaderLocation();
        //            moveTo = tmpMoveTo;
        //            break;
        //        }
        //    }

        //    return canMove;
        //}

        #endregion

        #region SelectMoveTo   

        public static bool SelectMoveTo(ICarrier carrier, VirtualMag mag, out Location moveTo, Location qrStage)
        {
            bool canMove = false;
            moveTo = null;

            Location tmpMoveTo = null;

            Station st = (Station)Enum.Parse(typeof(Station), mag.LocationId.ToString());
            bool tmpCanMove = HasMoveTo(new Location(mag.Origin.Value,  st), out tmpMoveTo, carrier, qrStage);


            //移動可能な場合はFromToを保持
            if (tmpCanMove == true)
            {
                canMove = true;
                //moveFrom = m.GetUnLoaderLocation();
                moveTo = tmpMoveTo;
                //break;
            }

            return canMove;
        }

        #endregion

        /// <summary>
        /// 与えられた同優先度装置の内、空マガジン処理要求がある装置を見つけた場合、
        /// 1つだけ処理してTrueを返す
        /// </summary>
        /// <param name="maclist"></param>
        /// <returns></returns>
        public static bool SelectAndMoveEmptyMagazineRequest(IEnumerable<IMachine> samePriorityMaclist)
        {
            foreach (IMachine m in samePriorityMaclist)
            {
                bool empMagMoved = m.ResponseEmptyMagazineRequest();
                if (empMagMoved)
                {
                    return true;
                }
            }

            return false;
        }

        #region HasMoveTo

        public static bool HasMoveTo(Location from, out Location to, ICarrier carrier, Location qrStage)
        {
            to = null;

            VirtualMag mag = null;

            if (qrStage == null)
            {
                mag = LineKeeper.GetMachine(from.MacNo).Peek(from.Station);
            }
            else
            {
                mag = LineKeeper.GetMachine(qrStage.MacNo).Peek(qrStage.Station);
            }

            //
            // debug code by juni
            //
            //if (from.MacNo == 112311)
            //{
            //    var a = "welcome";
            //}
            // code end

            if (mag != null)
            {
                // 移動元装置が排出モードの場合は排出コンベヤを確認
                if (LineKeeper.GetMachine(from.MacNo).IsDischargeMode(mag) == true)
                {
                    if (mag.NextMachines.Count >= 1)
                    {
                        int? conveyor = LineKeeper.GetDischargeConveyor(from.MacNo);
                        if (conveyor.HasValue)
                        {
                            to = LineKeeper.GetMachine(conveyor.Value).GetLoaderLocation();
                            mag.PurgeReason = "抜き取りモード排出";
							mag.Updatequeue();

							return true;
						}
						else
						{
							// 抜き取りCVに行けない場合は移動無し。
							return false;
						}
                    }
                }

                ////行き先不明のマガジンは排出CV行き
                //if (mag.NextMachines.Count == 0 && mag.ProcNo.HasValue)
                //{
                //    IMachine dischargeCV = LineKeeper.GetMachine(Route.GetDischargeConveyor(from.MacNo));
                //    if (dischargeCV.IsRequireInput() == true)
                //    {
                //        to = dischargeCV.GetLoaderLocation();
                //        return true;
                //    }
                //    else
                //    {
                //        //排出CVに行けない場合は移動無し。
                //        return false;
                //    }
                //}

                // 到達可能な装置順に並び変える
                mag.NextMachines = Route.GetSortReachableMachine(carrier.CarNo, mag.NextMachines);

                // 次装置の内、供給要求が立っている最優先を選択
                IMachine tmpNext = SelectTopPriorityLineMachine(carrier, mag.NextMachines, mag);
                if (tmpNext != null)
                {
                    // 投入側装置の投入位置を取得
                    to = tmpNext.GetLoaderLocation();

					// 橋経由で向こう側のラインに搬送する時だけ
					if (Route.IsMyReachable(from, to) == false)
					{
						List<VirtualMag> holdMagazines = CarrierBase.GetHoldingMagazines();
						if (holdMagazines.Exists(m => m.NextMachines.Exists(n => n == tmpNext.MacNo && m.MagazineNo != mag.MagazineNo)))
						{
							// ロボット、橋が保持しているマガジンの次装置に、今から搬送しようとしている装置が存在する場合は移動無し。
							Log.SysLog.Info(
								string.Format("向こう側の装置への搬送で、ロボット又は橋にその装置へ搬送中のマガジンがある為、待機します。MagazineNo:{0} 移動元装置:{1} 移動先装置:{2}",
								mag.MagazineNo, from.MacNo, to.MacNo));
							return false;
						}
					}
                }
            }

            if (to != null)
            {
                //TODO
                // 移動先装置の供給前排出モードを確認してONなら排出
                // 他ライン判定等はFrom側の排出モード判定で終わっているので、抜き取りCVへの振り替えのみでOK。
                // ただし、途中投入CVからの投入の場合は無視して運ぶ
                if ((LineKeeper.GetMachine(from.MacNo) is Machines.LoadConveyor) == false
                    && (LineKeeper.GetMachine(from.MacNo) is Machines.LineBridge) == false
                    && LineKeeper.GetMachine(to.MacNo).IsPreInputDischargeMode(mag) == true)
                {
                    int? conveyor = LineKeeper.GetDischargeConveyor(from.MacNo);
                    if (conveyor.HasValue)
                    {
                        to = LineKeeper.GetMachine(conveyor.Value).GetLoaderLocation();
                    }
                    else
                    {
                        // 抜き取りCVに行けない場合は移動しない。
                        to = null;
                        return false;
                    }
                }
                return true;
            }
            else
            {
				// 次工程が常温待機CVの場合、全てのCV(供給禁止装置は除外)で投入するマガジンとは違う待機時間のマガジンが入っていて、供給ONのCVがあれば排出CVへ
				//                           全てのCV(供給禁止装置は除外)で投入するマガジンと一致する待機時間のマガジンは存在するが、供給OFFのCVがあれば待機
				//if (mag != null && mag.ProcNo == Process.GetProcess("MD0072").ProcNo)
				//{
				//	to = LineKeeper.GetMachine(Route.GetDischargeConveyor(from.MacNo)).GetLoaderLocation();

				//	List<int> mdConveyors = mag.NextMachines.Where(m => LineKeeper.GetMachine(m) is MoldConveyor).ToList();
				//	mdConveyors = mdConveyors.Where(cv => LineKeeper.GetMachine(cv).IsInputForbidden() == false).ToList();

				//	foreach (int mdCv in mdConveyors) 
				//	{
				//		if (LineKeeper.GetMachine(mdCv).IsRequireInput() == false && LineKeeper.GetMachine(mdCv).CanInput(mag) == true) 
				//		{
				//			to = null;
				//			return false;
				//		}
				//	}

				//	if (to != null)
				//	{
				//		Log.SysLog.Info(string.Format("常温待機CVへの搬送で、全てのCVで待機時間の違うマガジンが投入されているか、供給禁止になっている為、排出 MagazineNo:{0}", mag.MagazineNo));
				//		return true;
				//	}
				//}

				return false;
            }
        }

        public static bool HasMoveTo(Location from, out Location to, ICarrier carrier)
        {
            return HasMoveTo(from, out to, carrier, null);
        }

        #endregion

        #region SelectDischargeConveyor

        /// <summary>
        /// 排出CVの行先を正しく選択。
        /// MoveToが排出CV行き以外で呼び出されても元々のMoveToを返す。
        /// </summary>
        /// <param name="moveFrom"></param>
        /// <param name="moveTo"></param>
        /// <param name="isEmptyMagazine"></param>
        /// <returns></returns>
        public static Location SelectDischargeConveyor(VirtualMag mag, Location moveFrom, Location moveTo, bool isAoiMagazine)
        {
            //アオイマガジンは別の判定が既にあるので判定しない。
            if (isAoiMagazine) return moveTo;

            //MoveFrom,Toの何れかがnullは判定不可
            if (moveFrom == null || moveTo == null || moveFrom.MacNo == 0 || moveTo.MacNo == 0)
            {
                return moveTo;
            }

            //排出CV行き以外は置き換えない
            //抜取りCVが排出と別にあるラインで、抜き取りCV行きの場合は置き換えない
            if (Route.GetDischargeConveyors(moveFrom.MacNo).Contains(moveTo.MacNo) == false)
            {
                return moveTo;
            }

            //TODO
            ////Fromのステーションが空マガジン関連排出関連だった場合は空マガジン排出CVへ
            //switch (moveFrom.Station)
            //{
            //    case Station.EmptyMagazineUnloader:
            //    case Station.EmptyMagazineUnloader1:
            //    case Station.EmptyMagazineUnloader2:
            //    case Station.EmptyMagazineUnloader3:
            //        return EmptyMagazineDischargeConveyor.GetLoaderLocation();
            //}

            //排出理由に記載がある場合（NG排出）はコンベヤは置き換えない
            if (string.IsNullOrEmpty(mag.PurgeReason) == false)
            {
                return moveTo;
            }

            //TODO
            //工程4(プラズマ）決め打ちでCVを置換え（仕様：四宮2012.02.22）
            //if (GetMachine(moveFrom.MacNo) is Machines.Oven && Process.GetProcess(mag.ProcNo).WorkCd == "WB0002")
            //{
            //    return DBSamplingConveyor.GetLoaderLocation();
            //}

            return moveTo;
        }
        #endregion

        /// <summary>
        /// 一致するIPアドレスを持った装置を無効化する。
        /// </summary>
        /// <param name="host"></param>
        public static void DisavailableHost(string host)
        {
            foreach (IMachine m in LineKeeper.Machines)
            {
                if (m.Plc == null) { continue; }
                if (m.Plc.IPAddress == host)
                {
                    m.IsAvailable = false;
                    m.LastDisAvailableTime = DateTime.Now;
                    Log.SysLog.Info("disavailable:" + m.MacNo);
                }
            }
        }

        public static bool IsRequireOutput(IMachine mac)
        {
            // 装置クラス毎のIsRequireMagazineOutputの切り分け方法が分からない為、
            // LineConfigのIsConveyorで、まず分ける
            if (mac.IsConveyor == true)
            {
                return mac.IsRequireMagazineOutput();
            }
            else
            {
                return mac.IsRequireOutput();
            }
        }

        public static string GetRequireBitData(IMachine mac, Station st, string buffercode, ICarrier carrier)
        {
            if (mac.IsConveyor == true)
            {
                return mac.GetRequireConveyorBitData(st, carrier.Plc);
            }
            else
            {
                return mac.GetRequireBitData(st, buffercode);
            }
        }
    }
}
