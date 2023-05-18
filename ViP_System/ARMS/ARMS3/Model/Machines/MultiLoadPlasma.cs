using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// MultiLoaderプラズマ
    /// </summary>
    public class MultiLoadPlasma : MachineBase
    {
        public class Lane
        {
            public string ReqBitAddress { get; set; }
            public string Point { get; set; }
            public Station ST { get; set; }
        }

        /// <summary>
        /// ローダー
        /// </summary>
        public SortedList<int, Lane> LoaderLanes { get; set; }

        /// <summary>
        /// アンローダー
        /// </summary>
        public SortedList<int, Lane> UnloaderLanes { get; set; }

        /// <summary>
        /// 空マガジン投入（アンローダー側）
        /// </summary>
        public SortedList<int, Lane> EmpMagLoaderLanes { get; set; }

        /// <summary>
        /// 空マガジン排出（ローダー側）
        /// </summary>
        public SortedList<int, Lane> EmpMagUnloaderLanes { get; set; }

        /// <summary>
        /// ローダー供給中
        /// </summary>
        public SortedList<int, Lane> LoaderDoLanes { get; set; }

        /// <summary>
        /// アンローダー排出中
        /// </summary>
        public SortedList<int, Lane> UnloaderDoLanes { get; set; }

        /// <summary>
        /// 空マガジン供給中（アンローダー側）
        /// </summary>
        public SortedList<int, Lane> EmpMagLoaderDoLanes { get; set; }

        /// <summary>
        /// 空マガジン排出中（ローダー側）
        /// </summary>
        public SortedList<int, Lane> EmpMagUnloaderDoLanes { get; set; }

        public MultiLoadPlasma()
            : base()
        {
            this.LoaderLanes = new SortedList<int, Lane>();
            this.UnloaderLanes = new SortedList<int, Lane>();
            this.EmpMagLoaderLanes = new SortedList<int, Lane>();
            this.EmpMagUnloaderLanes = new SortedList<int, Lane>();
            this.LoaderDoLanes = new SortedList<int, Lane>();
            this.UnloaderDoLanes = new SortedList<int, Lane>();
            this.EmpMagLoaderDoLanes = new SortedList<int, Lane>();
            this.EmpMagUnloaderDoLanes = new SortedList<int, Lane>();
        }

        protected override void concreteThreadWork()
        {
            if (IsRequireOutput() == true)
            {
                workComplete();
            }

            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        /// <summary>
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            try
            {
                foreach (KeyValuePair<int, Lane> kv in this.UnloaderLanes)
                {
                    //既にULに仮想マガジンがある場合は処理しない
                    VirtualMag ulMagazine = this.Peek(this.UnloaderLanes[kv.Key].ST);
                    if (ulMagazine != null) return;

                    string retv = Plc.GetBit(kv.Value.ReqBitAddress);
                    if (retv == Mitsubishi.BIT_ON)
                    {
                        VirtualMag newmag = this.Peek(this.EmpMagLoaderLanes[kv.Key].ST);
                        VirtualMag oldmag = this.Peek(this.LoaderLanes[kv.Key].ST);
                        if (oldmag != null)
                        {
                            if (newmag == null) return;

                            //作業開始完了時間取得
                            newmag.LastMagazineNo = oldmag.MagazineNo;
                            newmag.WorkComplete = DateTime.Now;
                            newmag.WorkStart = oldmag.WorkStart;
                            newmag.ProcNo = oldmag.ProcNo;
                            newmag.Origin = oldmag.Origin;
                            newmag.OriginLocationId = oldmag.OriginLocationId;

                            oldmag.InitializeWorkData();
                            oldmag.ProcNo = null;

                            this.Enqueue(newmag, kv.Value.ST);
                            this.Enqueue(oldmag, this.EmpMagUnloaderLanes[kv.Key].ST);

                            this.Dequeue(this.EmpMagLoaderLanes[kv.Key].ST);
                            this.Dequeue(this.LoaderLanes[kv.Key].ST);

                            this.WorkComplete(newmag, this, true);
                        }
                        else
                        {
                            if (newmag == null) return;

                            //アンローダー側は空マガジンであっても一旦排出する
                            newmag.PurgeReason = "プラズマ機からマガジン排出";
                            newmag.NextMachines.Clear();
                            newmag.NextMachines.Add(Route.GetDischargeConveyor(this.MacNo));

                            this.Enqueue(newmag, kv.Value.ST);
                            this.Dequeue(this.EmpMagLoaderLanes[kv.Key].ST);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.RBLog.Info("", ex);
            }
        }

        /// <summary>
        /// 排出要求　可否確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutput()
        {
            bool ret = isRequire(Plc, UnloaderLanes);
            if (ret == false)
            {
                //排出要求OFFかつ空マガジン供給要求もOFF = UL側が稼働位置に居る。　（手前側ならどちらからが必ずON)
                //かつ、LD側の空MG排出要求ONなら、最終フレーム処理中の状態と仮定して予約動作実施。
                if (IsRequireEmptyMagazine() == false && IsRequireOutputEmptyMagazine() == true)
                {
                    return true;
                }
            }

            return ret;
        }

        /// <summary>
        /// 供給要求　可否確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireInput()
        {
            if(this.IsInputForbidden() == true)
            {
                return false;
            }
            return isRequire(Plc, LoaderLanes);
        }

        /// <summary>
        /// 空マガジン供給　可否確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireEmptyMagazine()
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }
            return isRequire(Plc, EmpMagLoaderLanes);
        }

        /// <summary>
        /// 空マガジン排出　可否確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutputEmptyMagazine()
        {
            return isRequire(Plc, EmpMagUnloaderLanes);
        }

        #region isRequire(Lane)

        private bool isRequire(IPLC plc, SortedList<int, Lane> lanes)
        {
            //排出要求信号の確認
            if (lanes == null || lanes.Count == 0)
            {
                return false;
            }

            //string retv = plc.GetBit(lanes[1].ReqBitAddress, lanes.Count);
            string retv;
            try
            {
                retv = plc.GetBit(lanes[1].ReqBitAddress, lanes.Count);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、要求OFF扱い。アドレス：『{lanes[1].ReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            foreach (char bit in retv)
            {
                if (bit.ToString() == Mitsubishi.BIT_ON)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
        
        /// <summary>
        /// 空マガジン供給
        /// </summary>
        /// <returns></returns>
        public override bool ResponseEmptyMagazineRequest()
        {

            //供給禁止状態なら処理しない
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            if (this.IsRequireEmptyMagazine() == true)
            {
                Location from = null;
                List<LineBridge> bridgeList = LineKeeper.Machines.Where(m => m is LineBridge).Select(m => (LineBridge)m).ToList();
                bool IsDeleteFromMag = false;

                //自装置の空マガジンを使用
                if (this.IsRequireOutputEmptyMagazine() == true)
                {
                    from = GetEmpMagUnloaderLoacation();
                    IsDeleteFromMag = true;
                }
                //ライン連結橋の空マガジンを使用
                else if (bridgeList.Count() > 0)
                {
                    foreach (LineBridge bridge in bridgeList)
                    {
                        if (bridge.IsRequireOutputEmptyMagazine() == false) continue;
                        //先頭が遠心沈降マガジン or アオイ基板マガジンなら処理しない
                        VirtualMag mag = bridge.Peek(bridge.GetUnLoaderLocation().Station);
                        if (VirtualMag.IsECKMag(mag.MagazineNo)) continue;
                        if (mag.IsAOIMag()) continue;

                        if (mag.NextMachines.Any() == true)
                        {
                            // 空マガジンの仮想マガジンの次装置(A)が入力されており、(A)の装置Noと自装置と違う場合、
                            // (A)装置の空Mag要求がONの場合は、そのマガジンは(A)装置専用とする為、自装置は要求しない
                            IMachine nmac = LineKeeper.GetMachine(mag.NextMachines.First());
                            if (nmac != null && nmac.MacNo != this.MacNo && nmac.IsRequireEmptyMagazine() == true)
                            {
                                continue;
                            }
                        }

                        from = bridge.GetUnLoaderLocation();
                        IsDeleteFromMag = true;

                        break;
                    }
                }
                //空マガジン投入CVの空マガジンを使用
                if (from == null)
                {
                    //空マガジン投入CVの状態確認
                    List<int> emptyMagazineLoadConveyorMacNoList = Route.GetEmptyMagazineLoadConveyors(this.MacNo);
                    foreach (int macNo in emptyMagazineLoadConveyorMacNoList)
                    {
                        //空マガジン投入CVの状態確認
                        IMachine empMagLoadConveyor = LineKeeper.GetMachine(macNo);
                        if (empMagLoadConveyor.IsRequireOutputEmptyMagazine() == true)
                        {
                            CarrierInfo fromCar = Route.GetReachable(new Location(this.MacNo, Station.Loader));
                            CarrierInfo toCar = Route.GetReachable(new Location(empMagLoadConveyor.MacNo, Station.Loader));
                            if (fromCar.CarNo != toCar.CarNo)
                            {
                                //空マガジン投入CVが自ラインでは無い場合、橋に空マガジンが無いか確認し、有れば搬送しないようにする
                                List<VirtualMag> bridgeMags = new List<VirtualMag>();

                                List<IMachine> bridges = LineKeeper.Machines.Where(m => m is LineBridge || m is RobotQRReader).ToList();
                                foreach (IMachine b in bridges)
                                {
                                    //橋内のすべての仮想マガジンを取得
                                    bridgeMags.AddRange(VirtualMag.GetVirtualMag(b.MacNo));
                                }
                                if (bridgeMags.Any(m => Magazine.GetCurrent(m.MagazineNo) == null &&
                                                        VirtualMag.IsECKMag(m.MagazineNo) == false &&
                                                        m.IsAOIMag() == false))
                                {
                                    return false;
                                }
                            }

                            from = new Location(empMagLoadConveyor.MacNo, Station.EmptyMagazineUnloader);

                            // 循環式投入CVは空マガジン排出時に仮想マガジンを作成するので
                            // 橋と同じ扱いにする (仮想マガジンを削除する)
                            if (empMagLoadConveyor is LoadConveyorMagRotation)
                            {
                                IsDeleteFromMag = true;
                            }
                        }
                    }
                    if (from == null)
                    {
                        //空マガジン投入CVにマガジンが無い場合
                        return false;
                    }
                }

                Location to = GetEmpMagLoaderLoacation();
                LineKeeper.MoveFromTo(from, to, IsDeleteFromMag, true, false);

                return true;
            }
            else
            {
                return false;
            }
        }

        #region GetEmpMagLoaderLoacation

        public Location GetEmpMagLoaderLoacation()
        {
            foreach (KeyValuePair<int, Lane> kv in this.EmpMagLoaderLanes)
            {
                string retv = Plc.GetBit(kv.Value.ReqBitAddress);

                if (retv == Mitsubishi.BIT_ON)
                {
                    return new Location(this.MacNo, kv.Value.ST);
                }
            }

            //投入先が無ければ排出コンベヤを返す
            Log.RBLog.Info("投入先が埋まった状態で投入場所の問い合わせ発生。排出コンベヤをセット");

            IMachine dischargeConveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
            return dischargeConveyor.GetLoaderLocation();
        }

        #endregion

        #region GetEmpMagtUnloaderLocation

        public Location GetEmpMagUnloaderLoacation()
        {
            foreach (KeyValuePair<int, Lane> kv in this.EmpMagUnloaderLanes)
            {
                string retv = Plc.GetBit(kv.Value.ReqBitAddress);

                if (retv == Mitsubishi.BIT_ON)
                {
                    return new Location(this.MacNo, kv.Value.ST);
                }
            }

            //投入先が無ければ排出コンベヤを返す
            Log.RBLog.Info("投入先が埋まった状態で投入場所の問い合わせ発生。排出コンベヤをセット");
            IMachine dischargeConveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
            return dischargeConveyor.GetLoaderLocation();
        }

        #endregion

        #region GetLoaderLocation

        public override Location GetLoaderLocation()
        {
            foreach (KeyValuePair<int, Lane> kv in this.LoaderLanes)
            {
                string retv = Plc.GetBit(kv.Value.ReqBitAddress);

                if (retv == Mitsubishi.BIT_ON)
                {
                    return new Location(this.MacNo, kv.Value.ST);
                }
            }

            //投入先が無ければ排出コンベヤを返す
            Log.RBLog.Info("投入先が埋まった状態で投入場所の問い合わせ発生。排出コンベヤをセット");
            IMachine dischargeConveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
            return dischargeConveyor.GetLoaderLocation();
        }

        #endregion

        #region GetUnloaderLocation

        public override Location GetUnLoaderLocation()
        {
            foreach (KeyValuePair<int, Lane> kv in this.UnloaderLanes)
            {
                string retv = Plc.GetBit(kv.Value.ReqBitAddress);

                if (retv == Mitsubishi.BIT_ON)
                {
                    return new Location(this.MacNo, kv.Value.ST);
                }
            }

            //投入先が無ければ排出コンベヤを返す
            Log.RBLog.Info("投入先が埋まった状態で投入場所の問い合わせ発生。排出コンベヤをセット");
            IMachine dischargeConveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
            return dischargeConveyor.GetLoaderLocation();
        }
        #endregion

        #region GetFromToCode

        public override string GetFromToCode(Station station)
        {
            switch (station)
            {
                case Station.EmptyMagazineLoader1:
                case Station.EmptyMagazineLoader2:
                case Station.EmptyMagazineLoader3:
                    return getStationPoint(this.EmpMagLoaderLanes, station);

                case Station.Loader1:
                case Station.Loader2:
                case Station.Loader3:
                    return getStationPoint(this.LoaderLanes, station);


                case Station.Unloader1:
                case Station.Unloader2:
                case Station.Unloader3:
                    return getStationPoint(this.UnloaderLanes, station);


                case Station.EmptyMagazineUnloader1:
                case Station.EmptyMagazineUnloader2:
                case Station.EmptyMagazineUnloader3:
                    return getStationPoint(this.EmpMagUnloaderLanes, station);

            }

            throw new ApplicationException("定義外のStationのGetFromToCode");
        }

        private string getStationPoint(SortedList<int, Lane> lanes, Station st)
        {
            foreach (Lane l in lanes.Values)
            {
                if (l.ST == st)
                {
                    return l.Point;
                }
            }

            throw new ApplicationException("定義外のStationのGetFromToCode");
        }

        #endregion

        #region GetFromBufferCode

        /// <summary>
        /// バッファ位置取得 (Robot3用 ステーション指定)
        /// </summary>
        /// <returns></returns>
        public override string GetFromToBufferCode(Station station)
        {
            switch (station)
            {
                case Station.EmptyMagazineLoader1:
                case Station.Loader1:
                case Station.Unloader1:
                case Station.EmptyMagazineUnloader1:
                    return "1";

                case Station.EmptyMagazineLoader2:
                case Station.Loader2:
                case Station.Unloader2:
                case Station.EmptyMagazineUnloader2:
                    return "2";

                case Station.EmptyMagazineLoader3:
                case Station.Loader3:
                case Station.Unloader3:
                case Station.EmptyMagazineUnloader3:
                    return "3";
            }

            throw new ApplicationException("定義外のStationのGetFromToCode");
        }

        #endregion

        #region GetRequireBitData
        public override string GetRequireBitData(Station st, string buffercode)
        {
            foreach (Lane l in this.LoaderLanes.Values)
            {
                if (l.ST == st)
                {
                    return Plc.GetBit(l.ReqBitAddress);
                }
            }
            foreach (Lane l in this.UnloaderLanes.Values)
            {
                if (l.ST == st)
                {
                    return Plc.GetBit(l.ReqBitAddress);
                }
            }
            foreach (Lane l in this.EmpMagLoaderLanes.Values)
            {
                if (l.ST == st)
                {
                    return Plc.GetBit(l.ReqBitAddress);
                }
            }
            foreach (Lane l in this.EmpMagUnloaderLanes.Values)
            {
                if (l.ST == st)
                {
                    return Plc.GetBit(l.ReqBitAddress);
                }
            }

            throw new ApplicationException($"定義外のStationのGetRequireBitData：{st.ToString()}");
        }
        #endregion

        #region SetAddressDoingLoadMagazine
        public override void SetAddressDoingLoadMagazine(Station st, string buffercode, bool loader, string data)
        {
            string tgtAddress = null;

            List<Lane> AllLanes = new List<Lane>();
            AllLanes.AddRange(this.LoaderDoLanes.Values);
            AllLanes.AddRange(this.UnloaderDoLanes.Values);
            AllLanes.AddRange(this.EmpMagLoaderDoLanes.Values);
            AllLanes.AddRange(this.EmpMagUnloaderDoLanes.Values);
            foreach (Lane l in AllLanes)
            {
                if (l.ST == st)
                {
                    tgtAddress = l.ReqBitAddress;
                    Plc.SetBit(tgtAddress, 1, data);
                    //break;
                }
                else
                {
                    // 目的のアドレス以外はBIT_OFFにする
                    Plc.SetBit(l.ReqBitAddress, 1, PLC.Common.BIT_OFF);
                }
            }

            if (tgtAddress == null)
                throw new ApplicationException($"{this.Name}：定義外のStationのSetAddressDoingLoadMagazine：{st.ToString()}");
        }
        #endregion
    }
}
