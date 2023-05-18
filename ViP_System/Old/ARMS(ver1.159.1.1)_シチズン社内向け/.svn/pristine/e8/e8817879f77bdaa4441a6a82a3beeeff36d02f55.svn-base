using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    public class ECK4MagExchanger : MachineBase
    {
        #region プロパティ

        public string IpAddress { get; set; }
        public int Port { get; set; }

        /// <summary>
        /// 排出モード信号（工程別）
        /// </summary>
        public SortedList<string, string> ProcDischargeAddressList { get; set; }

        /// <summary>
        /// 空マガジン供給要求
        /// </summary>
        public SortedList<int, string> EmpMagLoaderReqBitAddressList { get; set; }

        /// <summary>
        /// 空マガジン供給FromTo
        /// </summary>
        public SortedList<int, string> EmptyMagLoaderPointList { get; set; }


        /// <summary>
        /// 排出マガジン供給要求
        /// </summary>
        public SortedList<int, string> UnloaderReqBitAddressList { get; set; }

        /// <summary>
        /// 排出マガジン供給FromTo
        /// </summary>
        public SortedList<int, string> UnloaderPointList { get; set; }


        /// <summary>
        /// 排出マガジン供給要求 (遠心沈降マガジン空)
        /// </summary>
        public SortedList<int, string> EmpMagUnloaderReqBitAddressList { get; set; }

        /// <summary>
        /// 排出マガジン供給FromTo (遠心沈降マガジン空)
        /// </summary>
        public SortedList<int, string> EmptyMagUnloaderPointList { get; set; }


        /// <summary>
        /// アンローダーリスト
        /// </summary>
        public SortedList<int, Station> UnloaderList { get; set; }

        /// <summary>
        /// 空マガジンローダーリスト
        /// </summary>
        public List<Station> EmpmagLoaderStations { get; set; }

        List<IMachine> moldlist = new List<IMachine>();

        /// <summary>
        /// 合体している遠沈
        /// </summary>
        public int CombinedECKMacNo { get; set; }

        MachinePLC mplc = MachinePLC.GetInstance();

        #endregion

        #region PLC信号定義

        /// <summary>
        /// ﾛﾎﾞｯﾄ待機位置 (B050C)
        /// </summary>
        public const string ROBOT_START_POSITION_BIT_ADDR = "B050C";

        /// <summary>
        /// PC→MD済み次工程沈降機信号 (B0520)
        /// </summary>
        public const string SEND_MOLD_COMPLETE_NEXT_ECK_BIT_ADDR = "B0520";

        /// <summary>
        /// 移載機RB動作禁止信号 (B0523)
        /// </summary>
        public const string EXCHANGE_ROBOT_WORK_FORBIDDEN_BIT_ADDR = "B0523";

        /// <summary>
        /// 移載機RB動作禁止信号(沈降Mag側) (B0524)
        /// </summary>
        public const string EXCHANGE_ROBOT_SIDE_LEFT_WORK_FORBIDDEN_BIT_ADDR = "B0524";

        /// <summary>
        /// 移載機RB動作禁止信号(通常Mag側) (B0525)
        /// </summary>
        public const string EXCHANGE_ROBOT_SIDE_RIGHT_WORK_FORBIDDEN_BIT_ADDR = "B0525";

        /// <summary>
        /// MD済み次工程沈降機信号 (B1020)
        /// </summary>
        public const string MOLD_COMPLETE_NEXT_ECK_BIT_ADDR = "B1020";

        #endregion

        public ECK4MagExchanger() 
            : base()
        {
            this.ProcDischargeAddressList = new SortedList<string, string>();
            this.EmpMagUnloaderReqBitAddressList = new SortedList<int, string>();
            this.EmptyMagUnloaderPointList = new SortedList<int, string>();
            this.UnloaderReqBitAddressList = new SortedList<int, string>();
            this.UnloaderPointList = new SortedList<int, string>();
            this.EmpMagLoaderReqBitAddressList = new SortedList<int, string>();
            this.EmptyMagLoaderPointList = new SortedList<int, string>();

            this.UnloaderList = new SortedList<int, Station>();
            this.UnloaderList.Add(1, Station.Unloader1);
            this.UnloaderList.Add(2, Station.Unloader2);
            this.UnloaderList.Add(3, Station.Unloader3);
            this.UnloaderList.Add(4, Station.Unloader4);

            this.EmpmagLoaderStations = new List<Station>();
            this.EmpmagLoaderStations.Add(Station.EmptyMagazineLoader1);
            this.EmpmagLoaderStations.Add(Station.EmptyMagazineLoader2);
            this.EmpmagLoaderStations.Add(Station.EmptyMagazineLoader3);
            this.EmpmagLoaderStations.Add(Station.EmptyMagazineLoader4);
        }

        protected override void concreteThreadWork()
        {
            //if (base.IsRequireOutputEmptyMagazine() == true)
            //{
                workComplete();
            //}

            //天井ロボット操作
            workRobot();

            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        #region WorkComplete
        
        /// <summary>
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            //移載機ローダーの完了登録処理
            workCompleteExchanger();


            //Unloader1～4の完了登録
            foreach (var st in this.UnloaderList.Values)
            {
                var ulmag = Peek(st);
                if (ulmag != null) continue;

                string magno = getUnloaderMagNo(st);
                if (string.IsNullOrEmpty(magno)) continue;


                string oldmagno = getExchangeLoaderMagNo2(st);
                if (string.IsNullOrEmpty(oldmagno))
                {
                    oldmagno = getExchangeLoaderMagNo1(st);
                }
                VirtualMag oldmag = this.Peek(Station.Loader);
                if (string.IsNullOrEmpty(oldmagno) || oldmag == null) continue;
                if (oldmag.MagazineNo != oldmagno)
                {
                    throw new ApplicationException("移載機仮想マガジン不一致発生 仮想マガジン=" + oldmag.MagazineNo + " : 装置取得=" + oldmagno);
                }

                VirtualMag newmag = new VirtualMag();
                newmag.MagazineNo = magno;
                //newmag.WorkStart = oldmag.WorkStart;
                newmag.WorkStart = DateTime.Now;
                newmag.WorkComplete = DateTime.Now;

                newmag.LastMagazineNo = oldmag.MagazineNo;
                newmag.ProcNo = oldmag.ProcNo;

                //移載機のローダー側はこの時点で仮想マガジン削除
                Dequeue(Station.Loader);

                //移載機アンローダー側は2マガジン統合の場合だけ存在
                //一致確認してから削除
                var ldmag = Peek(Station.Unloader);
                if (ldmag != null && ldmag.MagazineNo == newmag.MagazineNo)
                {
                    Dequeue(Station.Unloader);
                }

                this.Enqueue(newmag, st);

                //本来は開始登録時に行われるマガジンから分割番号を除去する処理
                //この装置は開始登録なしなのでここで行う
                Magazine mag = Magazine.GetCurrent(oldmag.MagazineNo);
                if (mag == null)
                {
                    throw new ApplicationException("マガジン情報が見つかりません:" + oldmag.MagazineNo);
                }
                mag.NewFg = false;
                mag.Update();

                mag.NascaLotNO = Order.MagLotToNascaLot(mag.NascaLotNO);
                mag.NewFg = true;
                mag.Update();


                base.WorkComplete(newmag, this, true);

                if (newmag.NextMachines.Count == 0)
                {
                    //次装置決定
                    Order order = CommonApi.GetWorkEndOrder(newmag, this.MacNo, this.LineNo);
                    ArmsApiResponse res = CommonApi.QueryNextWork(order, newmag);
                    newmag.NextMachines = res.NextMachines;
                }
            }
        }
        #endregion

        #region WorkCompleteExchanger

        /// <summary>
        /// 2マガジン統合の場合の1マガジン目だけは移載機出口（Unloader）で仮想マガジン作成
        /// それ以外のケースはUnloader1~4だけで完了するのでここでは扱わない
        /// </summary>
        private void workCompleteExchanger()
        {
            string newmagno = getExchangeUnloaderMagNo();
            if (string.IsNullOrEmpty(newmagno)) return;

            //2マガジン統合の2マガジン目は扱わない
            VirtualMag newmag = this.Peek(Station.Unloader);
            if (newmag != null) return;

            string oldmagno = getExchangeLoaderMagNo1(Station.Unloader);
            //移載元マガジン番号が取れない場合は移載前なので何もしない
            if (string.IsNullOrEmpty(oldmagno)) return;

            VirtualMag oldmag = this.Peek(Station.Loader);
            if (oldmag == null) return;
            if (oldmag.MagazineNo != oldmagno)
            {
                throw new ApplicationException("移載機仮想マガジン不一致発生 " + oldmag.MagazineNo + " : " + oldmagno);
            }

            Process.MagazineDevideStatus mst = Process.GetMagazineDevideStatus(oldmag.MagazineNo, oldmag.ProcNo.Value);
            if (mst == Process.MagazineDevideStatus.Single)
            { 
                //統合無しの移載の場合はここで完了させず、Unloader1～4で処理する
                return;
            }

            Log.RBLog.Info("分割ロットの1Mag目の処理開始=Unloaderの仮想マガジン作成： " + oldmag.MagazineNo + ", 処理区分=" + mst.ToString());

            newmag = new VirtualMag();
            newmag.MagazineNo = newmagno;

            newmag.LastMagazineNo = oldmag.MagazineNo;

            //本来は開始登録時に行われるマガジンから分割番号を除去する処理
            //この装置は開始登録なしなのでここで行う
            Magazine mag = Magazine.GetCurrent(oldmag.MagazineNo);
            if (mag == null)
            {
                throw new ApplicationException("マガジン情報が見つかりません:" + oldmag.MagazineNo);
            }
            mag.NewFg = false;
            mag.Update();

            mag.NascaLotNO = Order.MagLotToNascaLot(mag.NascaLotNO);
            mag.NewFg = true;
            mag.Update();


            newmag.ProcNo = oldmag.ProcNo;
            //newmag.WorkStart = oldmag.WorkStart;
            newmag.WorkStart = DateTime.Now;
            newmag.WorkComplete = DateTime.Now;

            this.Enqueue(newmag, Station.Unloader);
            this.Dequeue(Station.Loader);



            base.WorkComplete(newmag, this, true);
        }
        #endregion

        #region GetFromToCode

        public override string GetFromToCode(Station station)
        {
            switch (station)
            {
                case Station.EmptyMagazineLoader:
                    foreach (KeyValuePair<int, string> kv in this.EmpMagLoaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            return this.EmptyMagLoaderPointList[kv.Key];
                        }
                    }
                    return this.EmptyMagLoaderPointList[1];

                case Station.EmptyMagazineUnloader:
                    foreach (KeyValuePair<int, string> kv in this.EmpMagUnloaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            return this.EmptyMagUnloaderPointList[kv.Key];
                        }
                    }
                    return this.EmptyMagUnloaderPointList[1];

                case Station.Unloader1:
                    return this.UnloaderPointList[1];

                case Station.Unloader2:
                    return this.UnloaderPointList[2];

                case Station.Unloader3:
                    return this.UnloaderPointList[3];

                case Station.Unloader4:
                    return this.UnloaderPointList[4];
            }

            throw new ApplicationException("定義外のStationのGetFromToCode");
        }
        #endregion

        #region IsRequireOutputEmptyMagazine

        public override bool IsRequireOutputEmptyMagazine()
        {
            //空Mag排出要求信号の確認
            if (this.EmpMagUnloaderReqBitAddressList == null)
            {
                return false;
            }

            if (this.EmpMagUnloaderReqBitAddressList.Count == 0)
            {
                return false;
            }

            foreach (string address in this.EmpMagUnloaderReqBitAddressList.Values)
            {
                //string retv = this.Plc.GetBit(address);
                string retv;
                try
                {
                    retv = this.Plc.GetBit(address);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、空排出要求OFF扱い。アドレス：『{address}』, エラー内容：{ex.Message}");
                    return false;
                }

                if (retv == Mitsubishi.BIT_ON)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region IsRequireEmptyMagazine

        public override bool IsRequireEmptyMagazine()
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            //排出要求信号の確認
            if (this.EmpMagLoaderReqBitAddressList == null)
            {
                return false;
            }

            if (this.EmpMagLoaderReqBitAddressList.Count == 0)
            {
                return false;
            }

            foreach (string address in this.EmpMagLoaderReqBitAddressList.Values)
            {
                //string retv = this.Plc.GetBit(address);
                string retv;
                try
                {
                    retv = this.Plc.GetBit(address);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、空供給要求OFF扱い。アドレス：『{address}』, エラー内容：{ex.Message}");
                    return false;
                }

                if (retv == Mitsubishi.BIT_ON)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 投入要求確認 (バッファ位置指定)
        /// </summary>
        /// <returns>結果</returns>
        public bool IsRequireEmptyMagazine(string loaderPoint)
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            //投入要求信号の確認
            if (this.EmptyMagLoaderPointList == null)
            {
                return false;
            }

            if (this.EmptyMagLoaderPointList.Count == 0)
            {
                return false;
            }
            
            foreach (KeyValuePair<int, string> kv in this.EmptyMagLoaderPointList)
            {
                if (loaderPoint == kv.Value)
                {
                    if (Plc.GetBit(this.EmpMagLoaderReqBitAddressList[kv.Key]) == Mitsubishi.BIT_ON)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        #region IsRequireOutput

        /// <summary>
        /// マガジン排出　可否確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutput()
        {
            //排出要求信号の確認
            if (this.UnloaderReqBitAddressList == null)
            {
                return false;
            }

            if (this.UnloaderReqBitAddressList.Count == 0)
            {
                return false;
            }

            
            //string retv = Plc.GetBit(this.UnloaderReqBitAddressList[1], UnloaderReqBitAddressList.Count);
            string retv;
            try
            {
                retv = this.Plc.GetBit(this.UnloaderReqBitAddressList[1], UnloaderReqBitAddressList.Count);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、排出要求OFF扱い。アドレス：『{this.UnloaderReqBitAddressList[1]}』, エラー内容：{ex.Message}");
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

        #region IsDischargeMode
        
        public override bool IsDischargeMode(VirtualMag mag)
        {
            if (mag == null) return false;

            Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
            if (svrmag == null) return false;

            string proc = svrmag.NowCompProcess.ToString();
            if (string.IsNullOrEmpty(proc)) return false;

            if (ProcDischargeAddressList.Keys.Contains(proc) == false) return false;
            string address = ProcDischargeAddressList[proc];

            if (string.IsNullOrEmpty(address)) return false;

            IPLC iPlc;
            if (this.CarrierPlc != null)
            {
                // 指定PLC(搬送パネルを想定)を使用
                iPlc = this.CarrierPlc;
            }
            else
            {
                // 装置PLCを使用
                iPlc = this.Plc;
            }

            string retv = iPlc.GetBit(address);
            if (retv == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region ResponseEmptyMagazineRequest
        
        /// <summary>
        /// 空マガジン配置
        /// 基板移載機の空マガジンはモールドの空マガジン排出からのみ取得する
        /// 空マガジン投入CVからは拾わない
        /// </summary>
        /// <returns>結果</returns>
        public override bool ResponseEmptyMagazineRequest()
        {
            //供給禁止状態なら処理しない
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            CarrierInfo fromCar = Route.GetReachable(new Location(this.MacNo, Station.Loader));
            if (moldlist.Count == 0)
            {
                // 手が届く範囲のモールド装置からのみ取得する
                foreach (IMachine mac in LineKeeper.Machines)
                {
                    // 反射材仕様のモールド装置は対象から除く
                    if (mac is Machines.Mold && !(mac is Machines.Mold3))
                    {
                        CarrierInfo toCar = Route.GetReachable(new Location(mac.MacNo, Station.Loader));
                        if (fromCar != null && toCar != null & fromCar.CarNo == toCar.CarNo)
                        {
                            moldlist.Add(mac);
                        }
                    }
                }
            }

            if (this.IsRequireEmptyMagazine() == true)
            {
                #region 通常の空マガジンをモールド機の空マガジン排出から取得
                Location to = new Location(this.MacNo, Station.EmptyMagazineLoader);
                Location from = null;

				int eulMagCt = 0;

                foreach (IMachine mac in moldlist)
                {
                    if (mac.IsRequireOutputEmptyMagazine())
                    {
                        VirtualMag mag = mac.Peek(Station.EmptyMagazineUnloader);
                        if (mag == null) continue;

                        Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
                        if (svrmag != null)
                        {
                            //現在稼働中フラグのあるマガジンなら無視する
                            continue;
                        }

						int ct = ((Mold)mac).GetMagazineCount(Station.EmptyMagazineUnloader);
						if (ct >= eulMagCt)
						{
							from = new Location(mac.MacNo, Station.EmptyMagazineUnloader);
							eulMagCt = ct;
						}
					}
                }
                if (from != null)
                {
                    // モールド装置の空マガジン排出から取得
                    LineKeeper.MoveFromTo(from, to, true, true, false);
                    return true;
                }
                #endregion

                //List<LineBridge> bridgeList = LineKeeper.GetReachBridges(this.MacNo);
                List<LineBridge> bridgeList = LineKeeper.Machines.Where(m => m is LineBridge).Select(m => (LineBridge)m).ToList();
                #region ライン連結橋の空マガジンを使用
                foreach (LineBridge bridge in bridgeList)
                {
                    if (bridge.IsRequireOutputEmptyMagazine() == false)
                    {
                        continue;
                    }
                    //先頭が遠心沈降マガジン or アオイ基板マガジンなら処理しない
                    VirtualMag mag = bridge.Peek(bridge.GetUnLoaderLocation().Station);
                    if (mag == null) continue;
                    if (VirtualMag.IsECKMag(mag.MagazineNo)) continue;
                    if (mag.IsAOIMag()) continue;

                    // 【N工場MAP J9・10不具合 修正】
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
                    LineKeeper.MoveFromTo(from, to, true, true, false);
                    return true;
                }
                #endregion

                IMachine conveyor = LineKeeper.GetMachine(Route.GetEmptyMagazineLoadConveyor(this.MacNo));
                #region 空マガジン投入コンベアの状態確認
                if (conveyor.IsRequireOutputEmptyMagazine() == true)
                {
                    /// J9・J10にて空マガジン投入コンベア → 遠心沈降機の間に橋(中間台)が生まれたので
                    /// 共通装置と同様に橋の仮想マガジンをチェックして、空マガジンがあれば、要求しないようにする
                    bool existsEmpMagInBridgeFg = false;
                    CarrierInfo toCar = Route.GetReachable(new Location(conveyor.MacNo, Station.Loader));
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
                            existsEmpMagInBridgeFg = true;
                        }
                    }
                    if (existsEmpMagInBridgeFg == false)
                    {
                        // 橋に空マガジンが無ければ、投入CVから取得
                        from = new Location(conveyor.MacNo, Station.EmptyMagazineUnloader);
                        LineKeeper.MoveFromTo(from, to, false, true, false);
                        return true;
                    }
                }
                #endregion
            }

            #region 遠心沈降用の空マガジンをモールド機の空マガジン供給に移載、要求無しなら排出CV行き
            if (this.IsRequireOutputEmptyMagazine() == true)
            {
                //LDに仮想マガジンが残った状態なら排出要求ONでも空マガジン排出しない
                //センサースレッドが作業完了を検知してマガジン移し替え処理を行った後でLD側はDequeueされる
                VirtualMag ldmag = Peek(Station.Loader);
                if (ldmag == null)
                {
                    //常に排出CV行きで移動開始。
                    //MoveFromTo内でモールド要求に応じてモールド機へ振り替える処理あり（途中投入CV兼用）
                    IMachine conveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
                    Location to = conveyor.GetLoaderLocation();
                    Location from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
                    LineKeeper.MoveFromTo(from, to, false, true, false);
                    return true;
                }
            }
            #endregion

            return false;
        }
        #endregion

        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //空マガジン置き場のローダーは何もしない
            if (station == Station.EmptyMagazineLoader)
            {
                return true;
            }

            return base.Enqueue(mag, station);
        }
        #region GetUnloaderLocation

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Location GetUnLoaderLocation()
        {
            foreach (KeyValuePair<int, string> kv in this.UnloaderReqBitAddressList)
            {
                string retv = Plc.GetBit(kv.Value);

                if (retv == Mitsubishi.BIT_ON)
                {
                    return new Location(this.MacNo, this.UnloaderList[kv.Key]);
                }
            }

            //投入先が無ければnull
            Log.RBLog.Info("オーブンで排出元が無い状態で排出元の問い合わせ発生。");
            return null;
        }
        #endregion

        #region workRobot
        
        private void workRobot()
        {
            ECK4 eck = (ECK4)LineKeeper.GetMachine(this.CombinedECKMacNo);
            if (eck == null)
            {
                throw new ApplicationException("ECK4MagExchanger:遠心沈降機参照エラー");
            }

            if (isRobotReady() == false)
            {
                return;
            }

            //空マガジンを移載機へ
            var ulMag = this.Peek(Station.Unloader);
            if (ulMag == null && isRequireExchangerEmpMag() == true)
            {
                //要求あり+仮想マガジンがUnloaderになしで移動指示
                bool moved = moveEmpMagToExchanger();
                if (moved)
                {
                    return;
                }
            }

            //遠沈済みマガジンで揃っているものを移載機へ
            var ldMag = this.Peek(Station.Loader);
            if (ldMag == null && isRequireExchangerEckMag() == true)
            {
                foreach (var st in eck.UnloaderStations)
                {
                    var mag = eck.Peek(st);
                    if (mag == null || eck.IsULMagazineExists(st) == false) continue;

                    if (canInputExchanger(mag))
                    {
                        moveEckMagToExchanger(st, mag);
                        return;
                    }
                }
            }
        }

        #endregion

        #region 移載機情報取得
        
        private bool isRequireExchangerEmpMag()
        {
            if (mplc.GetBit("B1304", this.IpAddress, this.Port) == MachinePLC.BIT_OFF)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isRequireExchangerEckMag()
        {
            if (mplc.GetBit("B1305", this.IpAddress, this.Port) == MachinePLC.BIT_OFF)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string getExchangeUnloaderMagNo()
        {
            return mplc.GetMagazineNo("W2600", true, this.IpAddress, this.Port, 10);
        }
        
        public bool IsExchangeLoaderMagazineExists()
        {
            string magno = getExchangeLoaderMagNo1(Station.Loader);
            if (string.IsNullOrWhiteSpace(magno))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Unloader1～4内容取得

        private string getUnloaderMagNo(Station st)
        {
            switch (st)
            {
                case Station.Unloader1:
                    return mplc.GetMagazineNo("W2500", true, this.IpAddress, this.Port, 10);

                case Station.Unloader2:
                    return mplc.GetMagazineNo("W2540", true, this.IpAddress, this.Port, 10);

                case Station.Unloader3:
                    return mplc.GetMagazineNo("W2580", true, this.IpAddress, this.Port, 10);

                case Station.Unloader4:
                    return mplc.GetMagazineNo("W25C0", true, this.IpAddress, this.Port, 10);

                default:
                    throw new ApplicationException("定義外のStation:" + st.ToString());
            }
        }

        private string getExchangeLoaderMagNo1(Station st)
        {
            switch (st)
            {
                case Station.Unloader1:
                    return mplc.GetMagazineNo("W250A", true, this.IpAddress, this.Port, 10);

                case Station.Unloader2:
                    return mplc.GetMagazineNo("W254A", true, this.IpAddress, this.Port, 10);

                case Station.Unloader3:
                    return mplc.GetMagazineNo("W258A", true, this.IpAddress, this.Port, 10);

                case Station.Unloader4:
                    return mplc.GetMagazineNo("W25CA", true, this.IpAddress, this.Port, 10);

                case Station.Loader:
                    return mplc.GetMagazineNo("W2640", true, this.IpAddress, this.Port, 10);

                case Station.Unloader:
                    return mplc.GetMagazineNo("W260A", true, this.IpAddress, this.Port, 10);

                default:
                    throw new ApplicationException("定義外のStation:" + st.ToString());
            }
        }

        private string getExchangeLoaderMagNo2(Station st)
        {
            switch (st)
            {
                case Station.Unloader1:
                    return mplc.GetMagazineNo("W2514", true, this.IpAddress, this.Port, 10);

                case Station.Unloader2:
                    return mplc.GetMagazineNo("W2554", true, this.IpAddress, this.Port, 10);

                case Station.Unloader3:
                    return mplc.GetMagazineNo("W2594", true, this.IpAddress, this.Port, 10);

                case Station.Unloader4:
                    return mplc.GetMagazineNo("W25D4", true, this.IpAddress, this.Port, 10);

                default:
                    throw new ApplicationException("定義外のStation:" + st.ToString());
            }
        }
        #endregion

        #region GetRobotMovePointFromStation

        private string GetRobotMovePointFromStation(Station st)
        {
            //この関数の中のUnloader1~7は合体している遠心沈降機のUnloader1～7を指す
            //移載機Unloader1～4はこちらで移動指示をしなくても自動で移るのでこの関数では扱わない
            switch (st)
            {
                case Station.Loader:
                    return "12";

                case Station.Unloader:
                    return "11";

                case Station.Unloader1:
                    return "51";

                case Station.Unloader2:
                    return "52";

                case Station.Unloader3:
                    return "53";

                case Station.Unloader4:
                    return "54";

                case Station.Unloader5:
                    return "55";

                case Station.Unloader6:
                    return "56";

                case Station.Unloader7:
                    return "57";

                case Station.EmptyMagazineLoader1:
                    return "31";

                case Station.EmptyMagazineLoader2:
                    return "32";

                case Station.EmptyMagazineLoader3:
                    return "33";

                case Station.EmptyMagazineLoader4:
                    return "34";

                default:
                    throw new ApplicationException("定義外の移動ステーションです:" + st.ToString());
            }
        }
        #endregion

        #region moveEmpMagToExchanger
        
        private bool moveEmpMagToExchanger() 
        {
            foreach (Station st in this.EmpmagLoaderStations)
            {
                if (isEmpMag(st) == true)
                {
                    MachinePLC mplc = MachinePLC.GetInstance();

                    mplc.SetBit("W24F0", GetRobotMovePointFromStation(st), this.IpAddress, this.Port);
                    mplc.SetBit("W24F1", GetRobotMovePointFromStation(Station.Unloader), this.IpAddress, this.Port);
                    mplc.SetBit("B0F0", "1", this.IpAddress, this.Port);

                    Log.RBLog.Info("ECK4 RB Move order FROM:" + st.ToString() + " TO:" + Station.Unloader.ToString());

                    return true;
                }
            }

            return false;
        }
        #endregion

        #region isEmpMag

        private bool isEmpMag(Station st)
        {
            //物確センサーONかつ、マガジン無い状態が0=空である場合にTrue
            switch (st)
            {
                case Station.EmptyMagazineLoader1:
                    if (mplc.GetBit("B1300", this.IpAddress, this.Port) == MachinePLC.BIT_ON && int.Parse(mplc.GetBit("W2536", this.IpAddress, this.Port)) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case Station.EmptyMagazineLoader2:
                    if (mplc.GetBit("B1301", this.IpAddress, this.Port) == MachinePLC.BIT_ON && int.Parse(mplc.GetBit("W2576", this.IpAddress, this.Port)) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case Station.EmptyMagazineLoader3:
                    if (mplc.GetBit("B1302", this.IpAddress, this.Port) == MachinePLC.BIT_ON && int.Parse(mplc.GetBit("W25B6", this.IpAddress, this.Port)) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case Station.EmptyMagazineLoader4:
                    if (mplc.GetBit("B1303", this.IpAddress, this.Port) == MachinePLC.BIT_ON && int.Parse(mplc.GetBit("W25F6", this.IpAddress, this.Port)) == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                default:
                    throw new ApplicationException("定義外のStation:" + st.ToString());

            }
        }
        #endregion

        #region isRobotReady

        /// <summary>
        /// 天面ロボットのReady信号
        /// </summary>
        /// <returns></returns>
        private bool isRobotReady()
        {
            if (int.Parse(mplc.GetBit("B0E0", this.IpAddress, this.Port)) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region moveEckMagToExchanger
        
        private void moveEckMagToExchanger(Station st, VirtualMag mag)
        {
            MachinePLC mplc = MachinePLC.GetInstance();

            Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
            AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            Process nextproc = Process.GetNextProcess(mag.ProcNo.Value, lot);
            Process.MagazineDevideStatus mst = Process.GetMagazineDevideStatus(mag.MagazineNo, nextproc.ProcNo);

            if (mst == Process.MagazineDevideStatus.DoubleToSingle)
            {
                //段数シフトありアドレスON
                mplc.SetBit("B0FD", "1", this.IpAddress, this.Port);
            }
            else
            {
                //段数シフトありアドレスON
                mplc.SetBit("B0FD", "0", this.IpAddress, this.Port);
            }

            mplc.SetBit("W24F0", GetRobotMovePointFromStation(st), this.IpAddress, this.Port);
            mplc.SetBit("W24F1", GetRobotMovePointFromStation(Station.Loader), this.IpAddress, this.Port);
            mplc.SetBit("B0F0", "1", this.IpAddress, this.Port);

            Log.RBLog.Info("ECK4 RB Move order FROM:" + st.ToString() + " TO:" + Station.Loader.ToString());

            ECK4 eck = (ECK4)LineKeeper.GetMachine(this.CombinedECKMacNo);
            eck.Dequeue(st);
            //this.Dequeue(st);
            //工程を更新
            mag.ProcNo = nextproc.ProcNo;
            this.Enqueue(mag, Station.Loader);
        }
        #endregion

        #region canInputExchanger
        /// <summary>
        /// 移載機へ投入可能か判定
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        private bool canInputExchanger(VirtualMag mag)
        {
            ECK4 eck = (ECK4)LineKeeper.GetMachine(this.CombinedECKMacNo);
            if (eck == null)
            {
                throw new ApplicationException("ECK4MagExchanger:遠心沈降機参照エラー");
            }

            // ロットの警告フラグが立っていない事
            Magazine magInfo = Magazine.GetCurrent(mag.MagazineNo);
            if (mag == null) return false;
            AsmLot lot = AsmLot.GetAsmLot(magInfo.NascaLotNO);
            if (lot == null) return false;
            if (lot.IsWarning == true) return false;

            // 仮想マガジンの工程Noの次工程が「マガジン交換作業」である事
            var nextproc = Process.GetNextProcess(mag.ProcNo.Value, lot);
            if (ECK4.IsMagExchangeWorkCd(nextproc.WorkCd) == false) return false;

            Process.MagazineDevideStatus mst = Process.GetMagazineDevideStatus(mag.MagazineNo, mag.ProcNo.Value);
            if (mst == Process.MagazineDevideStatus.Single)
            {
                //単マガジンの場合 UL側に仮想マガジン無い事=他のセットの処理途中でないこと
                var ulmag = Peek(Station.Unloader);
                if (ulmag == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (mst == Process.MagazineDevideStatus.Double)
            {
                //分割マガジンの場合
                var ulmag = Peek(Station.Unloader);
                if (ulmag != null)
                {
                    if (isSameNascaLot(mag, ulmag) == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    foreach (var st in eck.UnloaderStations)
                    {
                        var othermag = eck.Peek(st);

                        if (othermag == null) continue;
                        if (othermag.MagazineNo == mag.MagazineNo) continue;

                        if (isSameNascaLot(mag, othermag) == true)
                        {
                            // 相方の仮想マガジンの工程Noの次工程が「マガジン交換作業」である事
                            nextproc = Process.GetNextProcess(othermag.ProcNo.Value, lot);
                            if (ECK4.IsMagExchangeWorkCd(nextproc.WorkCd) == false)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }
            else
            {
                throw new ApplicationException("想定外のマガジン分割ステータス:" + mst.ToString());
            }
        }
        #endregion

        #region isSameNascaLot
        private bool isSameNascaLot(VirtualMag mag1, VirtualMag mag2)
        {
            Magazine svrmag1 = Magazine.GetCurrent(mag1.MagazineNo);
            Magazine svrmag2 = Magazine.GetCurrent(mag2.MagazineNo);

            if (svrmag1 == null || svrmag2 == null) return false;

            AsmLot svrlot1 = AsmLot.GetAsmLot(svrmag1.NascaLotNO);
            AsmLot svrlot2 = AsmLot.GetAsmLot(svrmag2.NascaLotNO);

            if (svrlot1.NascaLotNo == svrlot2.NascaLotNo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 装置No取得 (Robot3用 ステーション指定)
        /// </summary>
        /// <returns></returns>
        public override string GetMacPoint(Station station)
        {
            switch (station)
            {
                case Station.Unloader1:
                case Station.Unloader2:
                case Station.Unloader3:
                case Station.Unloader4:
                case Station.EmptyMagazineLoader:
                    return base.GetMacPoint(station);

                case Station.EmptyMagazineUnloader:
                    IMachine mac = LineKeeper.GetMachine(this.CombinedECKMacNo);
                    return mac.GetMacPoint(station);

                default:
                    throw new ApplicationException($"{this.Name}：定義外のStation:{station.ToString()}のGetMacPointの要求を検知しました。");
            }
        }

        #region GetFromBufferCode        
            /// <summary>
            /// 搬送先バッファ位置取得 (Robot3用 ステーション指定)
            /// </summary>
            /// <returns></returns>
        public override string GetFromToBufferCode(Station station)
        {
            this.OutputApiLog($"GetFromToBufferCode開始[Station:{station.ToString()}]");
            switch (station)
            {
                case Station.Unloader1:
                    this.OutputApiLog($"GetFromToBufferCode終了[返値:1]");
                    return "1";
                case Station.Unloader2:
                    this.OutputApiLog($"GetFromToBufferCode終了[返値:2]");
                    return "2";
                case Station.Unloader3:
                    this.OutputApiLog($"GetFromToBufferCode終了[返値:3]");
                    return "3";
                case Station.Unloader4:
                    this.OutputApiLog($"GetFromToBufferCode終了[返値:4]");
                    return "4";

                case Station.EmptyMagazineLoader:
                    foreach (KeyValuePair<int, string> kv in this.EmpMagLoaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            this.OutputApiLog($"GetFromToBufferCode返す[アドレス：{kv.Value}, BIT_ON, 返値：{kv.Key.ToString()}]");
                            return kv.Key.ToString();
                        }
                        this.OutputApiLog($"GetFromToBufferCode信号チェック[アドレス：{kv.Value}, BIT_OFF]");
                    }
                    this.OutputApiLog($"GetFromToBufferCode-BIT_ON信号なし[返値=空文字]");
                    return null;

                case Station.EmptyMagazineUnloader:
                    foreach (KeyValuePair<int, string> kv in this.EmpMagUnloaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            this.OutputApiLog($"GetFromToBufferCode返す[アドレス：{kv.Value}, BIT_ON, 返値：{kv.Key.ToString()}]");
                            return kv.Key.ToString();
                        }
                        this.OutputApiLog($"GetFromToBufferCode信号チェック[アドレス：{kv.Value}, BIT_OFF]");
                    }
                    this.OutputApiLog($"GetFromToBufferCode-BIT_ON信号なし[返値=空文字]");
                    return null;

                default:
                    throw new ApplicationException($"{this.Name}：定義外のStation:{station.ToString()}のGetFromToCodeの要求を検知しました。");
            }

        }
        #endregion

        #region GetRequireBitData
        /// <summary>
        /// 供給要求BITデータ取得 (Robot3用 ステーション指定)
        /// </summary>
        /// <returns></returns>
        public override string GetRequireBitData(Station st, string buffercode)
        {
            int bc;
            if (int.TryParse(buffercode, out bc) == false)
            {
                bc = 1;
            }

            switch (st)
            {
                case Station.Unloader1:
                case Station.Unloader2:
                case Station.Unloader3:
                case Station.Unloader4:
                    return this.Plc.GetBit(this.UnloaderReqBitAddressList[bc]);

                case Station.EmptyMagazineLoader:
                    return this.Plc.GetBit(this.EmpMagLoaderReqBitAddressList[bc]);

                case Station.EmptyMagazineUnloader:
                    return this.Plc.GetBit(this.EmpMagUnloaderReqBitAddressList[bc]);

                default:
                    throw new ApplicationException($"{this.Name}：GetRequireBitDataで定義されていないStation:{st.ToString()}の要求を検知しました。lineconfigを確認して下さい。");
            }
        }
        #endregion
    }
}
