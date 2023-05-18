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
    /// 循環式途中投入CV (実Mag・空Mag両方投入可)
    /// </summary>
    public class LoadConveyorMagRotation : HybridLoadConveyor
    {
        #region 列挙型-装置モード (MachineMode)
        protected enum MachineMode
        {
            Normal,
            MagazineCheckRotation,
            SelectMagazineUnloader,
        }
        protected string MachineModeToString(MachineMode mode)
        {
            switch(mode)
            {
                case MachineMode.Normal:
                    return "通常モード";
                case MachineMode.MagazineCheckRotation:
                    return "棚卸しチェックモード";
                case MachineMode.SelectMagazineUnloader:
                    return "指定マガジン排出モード";
                default:
                    return string.Empty;
            }
        }
        #endregion

        #region プロパティ・定数
        
        /// <summary>
        /// QR読み取り位置のマガジンを循環させる(下段に送る)動作をコンベアに要求する信号 (PC → 装置)
        /// </summary>
        public string RotationReqBitAddress { get; set; }

        /// <summary>
        /// 棚卸しチェックの頻度 (秒)
        /// </summary>
        public int MagazineCanInputCheckCycleSecond { get; set; }

        /// <summary>
        /// Loaderの仮想マガジンを排出扱いにするまでの時間 (秒)
        /// </summary>
        public int LoaderMagazineDischargeCheckCycleSecond { get; set; }

        /// <summary>
        /// UnLoaderの仮想マガジンを排出扱いにするまでの時間 (秒)
        /// </summary>
        public int UnloaderMagazineDischargeCheckCycleSecond { get; set; }

        /// <summary>
        /// 下段の先頭位置にマガジンがある場合に立つ信号
        /// </summary>
        public string UnderStepMagazineExistsBitAddress { get; set; }

        /// <summary>
        /// 下段の先頭マガジンを循環させる(上段に送る)動作をコンベアに要求する信号 (PC → 装置)
        /// </summary>
        public string UpperRotaionReqBitAddress { get; set; }

        /// <summary>
        /// QRリーダーで読み込んだマガジンNoリスト (棚卸チェック済み)
        /// </summary>
        protected List<string> readMagazineQRList { get; set; }

        /// <summary>
        /// 次装置へ搬送可能となったマガジンNo
        /// </summary>
        protected string canInputNextMachineMagNo { get; set; }

        /// <summary>
        /// 最後に棚卸しチェックした時刻
        /// </summary>
        protected DateTime canInputCheckLastDateTime { get; set; }

        protected int MAGAZINE_STATUS = 1;
        protected int EMPTY_MAGAZINE_STATUS = 2;
        
        protected MachineMode macMode { get; set; }

        #endregion

        #region コンストラクタ

        public LoadConveyorMagRotation()
        {
            this.readMagazineQRList = new List<string>();

            this.initProperty(true, true);
        }

        #endregion

        /// <summary>
        /// プロパティを初期化して、通常モードに戻す
        /// </summary>
        protected void initProperty(bool initMagNo, bool initCheckLastDt)
        {
            #region

            if (initMagNo == true)
            {
                this.canInputNextMachineMagNo = string.Empty;
            }
            if (initCheckLastDt == true)
            {
                this.canInputCheckLastDateTime = DateTime.Now;
            }

            this.readMagazineQRList.Clear();
            this.modeChange(MachineMode.Normal);

            #endregion
        }

        protected override void concreteThreadWork()
        {
            // 装置状態毎の処理
            switch (this.macMode)
            {
                case MachineMode.Normal:
                    processNormalMode();
                    break;

                case MachineMode.MagazineCheckRotation:
                    processCheckRotationMode();
                    break;

                case MachineMode.SelectMagazineUnloader:
                    processSelectUnloaderMode();
                    break;
            }

            if (this.isMissingReservedMagazine() == true)
            {
                this.Dequeue(Station.Unloader);
                this.Dequeue(Station.EmptyMagazineUnloader);
                this.Plc.SetBit(MissingReservedMagazineBitAddress, 1, Mitsubishi.BIT_OFF);
            }
        }
        
        /// <summary>
        /// ログ表示 (現在の装置モードも表示)
        /// </summary>
        /// <param name="message"></param>
        protected void OutputSysLogWithModeName(string message)
        {
            this.OutputSysLog($"[{MachineModeToString(this.macMode)}]{message}");
        }

        /// <summary>
        /// 装置状態の変更 + 変更ログ表示
        /// </summary>
        /// <param name="mode"></param>
        protected void modeChange(MachineMode mode)
        {
            #region

            MachineMode oldMode = this.macMode;
            this.macMode = mode;

            string logMsg = $"装置状態変更：{MachineModeToString(oldMode)} ⇒ {MachineModeToString(mode)}";
            if (mode == MachineMode.SelectMagazineUnloader)
            {                
                logMsg += $" [指定マガジンNo:「{this.canInputNextMachineMagNo}」]";
            }
            this.OutputSysLog(logMsg);

            #endregion
        }

        /// <summary>
        /// 通常動作
        /// </summary>
        protected void processNormalMode()
        {
            #region

            // 排出位置から一定時間経過したマガジンを排出CV行きにする
            this.checkLoaderMagazine();
            this.checkEmpLoaderMagazine();
            this.checkUnLoaderMagazine();
            this.checkEmpUnloaderMagazine();

            // リフトアップ中または循環(QR読み取り位置→下段)中ではない時の処理
            if (this.Plc.GetBit(this.OutputReserveBitAddress) == PLC.Common.BIT_OFF)
            {
                if (base.IsRequireOutput() == false)
                {
                    // 次装置へ搬送可能な仮想マガジンがあるかをチェックして、
                    // マガジンの有無によりモード変更
                    this.canInputNextMachineMagNo = getMagazineNoCanInputNextMachine();
                    if (string.IsNullOrWhiteSpace(canInputNextMachineMagNo) == false)
                    {
                        // 投入可能マガジン有り ⇒ 指定マガジン排出モードへ変更
                        this.modeChange(MachineMode.SelectMagazineUnloader);
                    }
                    else if((DateTime.Now - this.canInputCheckLastDateTime).Seconds > MagazineCanInputCheckCycleSecond)
                    {                   
                        // 一定時間経過したら、棚卸しチェックを行う
                        
                        // 棚卸しモードへ変更
                        this.modeChange(MachineMode.MagazineCheckRotation);
                    }
                }

                if (this.macMode == MachineMode.Normal && this.isMagazineArrived() == true)
                {
                    // 入ってきたマガジンの(Loader or EmptyMagazineLoader)仮想マガジン作成 + 次装置リストの作成
                    // 次装置の供給要求が一つ以上ある場合にUnLoader(or EmptyMagazineUnLoader)に移してリフトアップする
                    this.processNormalMagazineArrived();
                }
            }
            else
            {
                // [this.Plc.GetBit(this.OutputReserveBitAddress) == PLC.Common.BIT_ON]
                if (this.Plc.GetBit(this.RotationReqBitAddress) == PLC.Common.BIT_ON)
                {
                    // CV循環指令を送信後、一定時間内に信号がOFFにならなければ、
                    // 下段が満杯と判断して、下段の先頭マガジンを上段に循環させる
                    DateTime checkStartDt = DateTime.Now;
                    while (true)
                    {
                        if (this.Plc.GetBit(this.OutputReserveBitAddress) == PLC.Common.BIT_OFF
                            && this.Plc.GetBit(this.RotationReqBitAddress) == PLC.Common.BIT_OFF)
                        {
                            break;
                        }

                        if ((DateTime.Now - checkStartDt).TotalSeconds > 60)
                        {
                            if (IsUnderStepMagazineExists() == true)
                            {
                                this.OutputSysLogWithModeName("一定時間(60秒)以内に循環処理が完了しなかった為、循環指令(下段先頭マガジン→上段)を送信");
                                this.sendPlcOrderUpperRotation();
                            }
                            break;
                        }
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 棚卸しチェックモードの処理
        /// </summary>
        protected void processCheckRotationMode()
        {
            #region

            // 開始後に初めて読み込んだマガジンを起点にして、全てのマガジンを読み込むまで
            // CV内のマガジンを読み込んで搬送可否をチェックする

            if (this.isMagazineArrived() == true)
            {
                // マガジンがQR読込位置に来た時の処理
                processCheckRotationMagazineArrived();
            }

            // 下段の先頭位置にマガジンがいる場合、下段先頭位置→上段へマガジンを循環させる
            if (this.IsUnderStepMagazineExists() == true)
            {
                this.sendPlcOrderUpperRotation();
            } 

            #endregion
        }

        /// <summary>
        /// 指定マガジン排出モード動作
        /// </summary>
        protected void processSelectUnloaderMode()
        {
            #region

            // 開始後に初めて読み込んだマガジンを起点にして、
            // 全てのマガジンを読み込むか指定のマガジンを読むまで
            // CV内のマガジンを読み込んで搬送可否をチェックする 
            //    指定のマガジンがきた場合は、次装置の供給要求を再チェックして
            //    要求ONならUnloaderに仮想マガジンを移す + 通常モードへ戻す

            if (this.isMagazineArrived() == true)
            {
                // マガジンがQR読込位置に来た時の処理
                this.processSelectUnloaderMagazineArrived();
            }

            // 下段の先頭位置にマガジンがいる場合、下段先頭位置→上段へマガジンを循環させる
            if (this.IsUnderStepMagazineExists() == true)
            {
                this.sendPlcOrderUpperRotation();
            }

            #endregion
        }

        #region マガジン到達時の処理 (processXXXXXMagazineArrived)

        /// <summary>
        /// 通常モードにおけるマガジン到達時の処理(排出前の持ち上げ動作と次作業の問い合わせジョブ)
        /// </summary>
        protected void processNormalMagazineArrived()
        {
            #region

            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            VirtualMag eulMagazine = this.Peek(Station.EmptyMagazineUnloader);
            if (ulMagazine != null || eulMagazine != null)
            {
                return;
            }

            string magazineNo = Plc.GetMagazineNo(QRReaderWordAddress);
            if (magazineNo == string.Empty)
            {
                return;
            }
          
            this.OutputSysLogWithModeName($"{this.Name} 到達マガジンの取得：『{magazineNo}』");

            VirtualMag vMag;
            List<VirtualMag> vMags = VirtualMag.GetVirtualMag(this.MacNo).ToList();
            if (vMags.Exists(m => m.MagazineNo == magazineNo))
            {
                vMag = vMags.Where(m => m.MagazineNo == magazineNo).OrderBy(m => m.LastUpdDt).First();
            }
            else
            {
                vMag = this.enqueueLoaderMagazine(magazineNo);
            }

            bool emptyMagazineFg;
            if (this.checkCanInputNextMachineAndMoveVirtualMag(vMag, out emptyMagazineFg) == true)
            {
                // CVの次の動作 = リフトアップ
                this.sendPlcOrderLiftUp(emptyMagazineFg);
            }
            else
            {
                // CVの次の動作 = CV奥へマガジンを循環
                this.sendPlcOrderRotation();
            }
            
            #endregion
        }

        /// <summary>
        /// 棚卸しチェックモードにおけるマガジン到達時の処理(不足仮想マガジンの追加と過剰仮想マガジンの削除)
        /// </summary>
        protected void processCheckRotationMagazineArrived()
        {
            #region

            // 装置へ処理信号を送った直後は処理しない
            if (this.Plc.GetBit(this.OutputReserveBitAddress) == PLC.Common.BIT_ON)
            {
                return;
            }

            // マガジンNoを取得
            string magazineNo = this.Plc.GetMagazineNo(QRReaderWordAddress);
            if (magazineNo == string.Empty)
            {
                return;
            }
            this.OutputSysLogWithModeName($"到達マガジンNo:「{magazineNo}」");

            // 棚卸チェック済みのマガジンNoリストに登録済み(最初のマガジンに戻ってきた) ⇒ 棚卸しチェック終了
            if (this.readMagazineQRList.Contains(magazineNo) == true)
            {
                // 仮想マガジンの内、QR読込リストに存在しない仮想マガジンを削除する                
                foreach (VirtualMag vMag in VirtualMag.GetVirtualMag(this.MacNo))
                {
                    if (this.readMagazineQRList.Contains(vMag.MagazineNo) == false)
                    {
                        vMag.Delete();
                    }
                }

                // プロパティを初期化 + 通常モードに戻す (棚卸し実施時刻更新)
                this.initProperty(false, true);

                return;
            }

            // Loader + EmptyMagazineLoaderに仮想マガジンが無い場合、追加する
            List<VirtualMag> vMags = VirtualMag.GetVirtualMag(this.MacNo, (int)Station.Loader).ToList();
            vMags.AddRange(VirtualMag.GetVirtualMag(this.MacNo, (int)Station.EmptyMagazineLoader).ToList());
            if (vMags.Exists(m => m.MagazineNo == magazineNo) == false)
            {
                // 仮想マガジンを追加
                this.enqueueLoaderMagazine(magazineNo);
            }

            // マガジンを循環 (QR読み取り位置→下段)
            this.sendPlcOrderRotation();

            // 読込済みマガジンQRリストに追加
            this.readMagazineQRList.Add(magazineNo);

            #endregion
        }

        /// <summary>
        /// 指定マガジン排出モードにおけるマガジン到達時の処理(不足仮想マガジンの追加と過剰仮想マガジンの削除)
        /// </summary>
        protected void processSelectUnloaderMagazineArrived()
        {
            #region

            // 装置へ処理信号を送った直後は処理しない
            if (this.Plc.GetBit(this.OutputReserveBitAddress) == PLC.Common.BIT_ON)
            {
                return;
            }

            // マガジンNoを取得
            string magazineNo = this.Plc.GetMagazineNo(QRReaderWordAddress);
            if (magazineNo == string.Empty)
            {
                return;
            }
            
            // 指定マガジン排出モード以降に読込済みのマガジンNoリストに登録済み ⇒ 処理中断
            if (this.readMagazineQRList.Contains(magazineNo) == true)
            {
                this.OutputSysLogWithModeName($"到達マガジン『{magazineNo}』 = 最初のマガジン ⇒ 処理中断(通常モードに戻す)");

                // 指定マガジンを読み込めずに最初のマガジンに戻ってきたという事は、
                // 存在しないマガジンを選んだ事になる。

                // 次装置投入可能なマガジンが、読み込み済みマガジンNoリストにない場合、
                // 指定マガジンNoの仮想マガジンを削除する
                IEnumerable<VirtualMag> vMags = VirtualMag.GetVirtualMag(this.MacNo).Where(m => m.MagazineNo == this.canInputNextMachineMagNo);
                foreach (VirtualMag vMag in vMags)
                {
                    vMag.Delete();
                }

                // プロパティを初期化 + 通常モードに戻す ( + 指定マガジンNo初期化)
                this.initProperty(true, false);

                return;
            }

            // 指定マガジンNoが流れてきた
            if (magazineNo == this.canInputNextMachineMagNo)
            {
                this.OutputSysLogWithModeName($"到達マガジン『{magazineNo}』 = 指定マガジン『{this.canInputNextMachineMagNo}』 ⇒ 排出処理");

                List<VirtualMag> vMags = VirtualMag.GetVirtualMag(this.MacNo, (int)Station.Loader).ToList();
                vMags.AddRange(VirtualMag.GetVirtualMag(this.MacNo, (int)Station.EmptyMagazineLoader).ToList());

                VirtualMag vMag = vMags.Where(m => m.MagazineNo == magazineNo).OrderBy(m => m.LastUpdDt).FirstOrDefault();
                if (vMag == null)
                {
                    // 仮想マガジンが残っていなければ、仮想マガジンを新規作成する
                    this.OutputSysLogWithModeName($"マガジンNo:「{magazineNo}」の仮想マガジンが無い為、仮想マガジンを新規作成");
                    vMag = this.enqueueLoaderMagazine(magazineNo);
                }

                // 仮想マガジンの位置を変更する (次装置の供給要求を再チェックをクリアできれば、排出)
                bool emptyMagazineFg = false;
                if (this.checkCanInputNextMachineAndMoveVirtualMag(vMag, out emptyMagazineFg) == true)
                {
                    // 排出
                    this.sendPlcOrderLiftUp(emptyMagazineFg);
                }
                else
                {
                    // CV内、循環
                    this.sendPlcOrderRotation();
                }

                // プロパティを初期化 + 通常モードに戻す ( + 指定マガジンNo初期化)
                this.initProperty(true, false);

                return;
            }

            this.OutputSysLogWithModeName($"到達マガジン『{magazineNo}』 ≠ 指定マガジン『{this.canInputNextMachineMagNo}』 ⇒ CV内循環処理");

            // マガジンを循環 (QR読み取り位置→下段)
            this.sendPlcOrderRotation();

            // 読込済みマガジンQRリストに追加
            this.readMagazineQRList.Add(magazineNo);

            #endregion
        }

        #endregion

        #region PLC要求確認 (IsRequire)

        /// <summary>
        /// Robotクラスが使用するCV上にマガジンがあるかどうか
        /// </summary>
        /// <returns></returns>
        public override bool IsUnloaderMagazineExists()
        {

            // 上段か下段にマガジンがいれば、あるものと判定する
            if (base.IsUnloaderMagazineExists() == true
                || this.IsUnderStepMagazineExists() == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 下段の先頭位置にマガジンがあるかどうか
        /// </summary>
        /// <returns></returns>
        public bool IsUnderStepMagazineExists()
        {
            if (string.IsNullOrWhiteSpace(this.UnderStepMagazineExistsBitAddress) == true)
            {
                return false;
            }

            try
            {
                string retv = this.Plc.GetBit(this.UnderStepMagazineExistsBitAddress);
                if (retv == PLC.Common.BIT_ON)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                // PLC通信異常時は、要求なし
                return false;
            }
        }

        #endregion

        #region 次装置投入可のマガジンNoを取得 (getMagazineNoCanInputNextMachine)

        /// <summary>
        /// Loader・EmptyMagazineLoader全マガジンの内、
        ///   次装置の供給要求が一つ以上ある仮想マガジンの中で更新日時が最も古いマガジンを返す
        /// </summary>
        protected string getMagazineNoCanInputNextMachine()
        {
            #region

            // 次装置への投入可マガジンの記憶用
            List<VirtualMag> canInputMags = new List<VirtualMag>();

            var lMagazines = VirtualMag.GetVirtualMag(this.MacNo, (int)Station.Loader).OrderBy(m => m.LastUpdDt);

            // 空マガジンは、更新日時が最も古い仮想マガジンを対象にする
            VirtualMag elMagazine = VirtualMag.GetVirtualMag(this.MacNo, (int)Station.EmptyMagazineLoader).OrderBy(m => m.LastUpdDt).FirstOrDefault();
            
            // Loader・EmptyMagazineLoaderの仮想マガジンが無い場合は、処理しない
            if (lMagazines.Any() == false && elMagazine == null) return string.Empty;
            
            // Loaderマガジンの確認 (次装置の内、供給要求が立っている装置が1台以上あれば、投入可とする)
            foreach (VirtualMag lMag in lMagazines)
            {
                if (this.canInputNextMachine(lMag) == true)
                {
                    // 投入可マガジンリストに追加
                    canInputMags.Add(lMag);
                    break;
                }
            }

            // EmptyeMagazineLoaderマガジンの確認 (全ての装置の内、空mag供給要求が立っている装置が1台以上あれば、投入可とする)
            if (elMagazine != null && this.canInputNextMachineEmptyMagazine() == true)
            {
                // 投入可マガジンリストに追加
                canInputMags.Add(elMagazine);
            }

            // 投入可のマガジンが無い場合は、処理をやめる
            if (canInputMags.Any() == false) return string.Empty;

            // 投入可のマガジンの内、更新日時が最も古いマガジンのマガジンNoを返す
            return canInputMags.OrderBy(m => m.LastUpdDt).First().MagazineNo;

            #endregion
        }

        #endregion

        #region 次装置への投入可否判定(canInputNextMachine)

        /// <summary>
        /// 実Magに対して次装置への投入が可能か判定する
        /// </summary>
        /// <param name="vMag"></param>
        /// <returns></returns>
        protected bool canInputNextMachine(VirtualMag vMag)
        {
            #region

            // 次装置が無い場合は、排出CVを入れる
            if (vMag.NextMachines == null || vMag.NextMachines.Any() == false)
            {
                vMag.NextMachines.AddRange(Route.GetDischargeConveyors(this.MacNo));
                vMag.Updatequeue();
            }

            // 次装置の投入要求確認   要求ONの装置を抽出
            IEnumerable<IMachine> macEnum = vMag.NextMachines.Select(m => LineKeeper.GetMachine(m));
            foreach(IMachine mac in macEnum)
            {
                if (mac.IsRequireInput() == true && mac.CanInput(vMag) == true)
                {
                    return true;
                }
            }

            // 実Mag投入要求ONの装置の有 or 無 →  投入可 or 不可
            return false;

            #endregion
        }

        /// <summary>
        /// 空Magに対して次装置への投入が可能か判定する
        /// </summary>
        /// <param name="vMag"></param>
        /// <returns></returns>
        protected bool canInputNextMachineEmptyMagazine()
        {
            #region

            CarrierInfo carInfo = Route.GetReachable(this.GetUnLoaderLocation());
            if (carInfo == null) return false;

            // 対象装置 = 同じライン内 + 到達可能
            List<int> macNoList = LineKeeper.Machines.Where(m => m.LineNo == this.LineNo).Select(m => m.MacNo).ToList();
            macNoList = Route.GetSortReachableMachine(carInfo.CarNo, macNoList);

            // 空Mag投入要求確認  要求ONの装置を抽出
            IEnumerable<IMachine> macEnum = macNoList.Select(m => LineKeeper.GetMachine(m));
            macEnum = macEnum.Where(m => m.IsRequireEmptyMagazine() == true && m.IsInputForbidden() == false);

            if (macEnum.Any() == false) return false;

            // ロボット(橋を含む)の仮想マガジンの内、空Mag供給要求ONの装置行きのマガジンがある場合は、投入可としない
            List<VirtualMag> carMags = new List<VirtualMag>();
            foreach(Carriers.ICarrier car in LineKeeper.Carriers)
            {
                if (car is Carriers.Bridge || car is Carriers.Robot)
                {
                    carMags.AddRange(car.HoldingMagazines);
                }
                else
                {
                    carMags.AddRange(VirtualMag.GetVirtualMag(car.QRReaderMacNo));
                }
            }
            carMags = carMags.Where(m => Magazine.GetCurrent(m.MagazineNo) == null).ToList();

            List<IMachine> macList = new List<IMachine>();
            foreach (IMachine mac in macEnum)
            {
                if (carMags.Exists(m => m.NextMachines.Exists(n => n == mac.MacNo)))
                {
                    continue;
                }
                macList.Add(mac);
            }
            // 空Mag投入要求ONの装置の有 or 無 →  投入可 or 不可
            return macList.Any();

            #endregion
        }

        #endregion

        #region Loader仮想マガジンの次装置投入可否を判定してUnloader仮想マガジンに移動する (checkCanInputNextMachineAndMoveVirtualMag)

        /// <summary>
        /// Loader仮想マガジンの次装置投入可否を判定してUnloader仮想マガジンに移動する
        /// 返値：Unloader稼働マガジンに移動したらtrueを返す
        /// </summary>
        /// <param name="vMag"></param>
        /// <returns></returns>
        protected bool checkCanInputNextMachineAndMoveVirtualMag(VirtualMag vMag, out bool emptyMagazineFg)
        {
            #region

            emptyMagazineFg = false;
            VirtualMag oldMagazine = VirtualMag.GetVirtualMag(this.MacNo, vMag.LocationId, vMag.MagazineNo);
            Station st = (Station)Enum.Parse(typeof(Station), vMag.LocationId.ToString());
            switch (st)
            {
                case Station.Loader:    // 実マガジン

                    // 次装置への投入可否判定
                    if (this.canInputNextMachine(vMag) == true)
                    {
                        // 投入可能の場合は、Loader ⇒ Unloaderへ仮想マガジンを移動する
                        this.Enqueue(vMag, Station.Unloader);
                        if (oldMagazine != null)
                        {
                            oldMagazine.Delete();
                        }

                        return true;
                    }
                    break;

                case Station.EmptyMagazineLoader:   // 空マガジン

                    emptyMagazineFg = true;

                    // 各装置への空マガジンの投入可否判定
                    if (this.canInputNextMachineEmptyMagazine() == true)
                    {
                        this.Enqueue(vMag, Station.EmptyMagazineUnloader);
                        if (oldMagazine != null)
                        {
                            oldMagazine.Delete();
                        }

                        return true;
                    }
                    break;
            }

            return false;

            #endregion
        }

        #endregion

        #region 読み込んだマガジンを仮想マガジンに追加(enqueueVirtualMagazine)

        /// <summary>
        /// 読み込んだマガジンNoから実Mag or 空Magのどちらであるかを判定して「Loader」 or 「EmptyMagazineLoader」の仮想マガジンに追加する
        /// 返値： 追加した仮想マガジン
        /// </summary>
        /// <param name="magno"></param>
        /// <returns></returns>
        protected VirtualMag enqueueLoaderMagazine(string magno)
        {
            #region

            Magazine svmag = Magazine.GetCurrent(magno);
            VirtualMag newMagazine = new VirtualMag();
            newMagazine.MacNo = this.MacNo;
            newMagazine.MagazineNo = magno;
            bool isEmptyMagazine = false;

            // 実マガジン or 空マガジンの判定
            if (VirtualMag.IsECKMag(magno))
            {
                //稼動中フラグのあるマガジンは行先の変更を行わない
                if (svmag == null)
                {
                    newMagazine.NextMachines.Add(Route.GetDischargeConveyor(this.MacNo));
                }
            }
            else if (svmag != null)
            {
                // 実マガジン
            }
            else
            {
                // 空マガジン
                isEmptyMagazine = true;
            }
            
            if (isEmptyMagazine == false)
            {
                // 実マガジン

                // Loader側の仮想マガジンとして一時保存 (以降、CV内にいる間は、常に次装置の供給要求を確認)
                Station st = Station.Loader;
                this.Enqueue(newMagazine, st);
                newMagazine.LocationId = (int)st;

                // 次装置を追加して、仮想マガジンを更新
                this.WorkComplete(newMagazine, this, false);
            }
            else
            {
                // 空マガジン
                Station st = Station.EmptyMagazineLoader;
                this.Enqueue(newMagazine, st);
                newMagazine.LocationId = (int)st;
            }

            return newMagazine;

            #endregion
        }

        #endregion

        #region 装置への指示(sendPlcOrder)
        protected void sendPlcOrderLiftUp(bool emptyMagazine)
        {
            int magSt = MAGAZINE_STATUS;
            if (emptyMagazine == true)
            {
                magSt = EMPTY_MAGAZINE_STATUS;
            }
            this.OutputSysLogWithModeName($"PC→装置 リフトアップ命令送信 [実({MAGAZINE_STATUS}):空({EMPTY_MAGAZINE_STATUS}) = {magSt}]))");
            this.Plc.SetWordAsDecimalData(this.UnLoaderMagazineStatusWordAddress, magSt);
            this.Plc.SetBit(this.RotationReqBitAddress, 1, Mitsubishi.BIT_OFF);
            this.Plc.SetBit(this.OutputReserveBitAddress, 1, Mitsubishi.BIT_ON);
        }

        protected void sendPlcOrderRotation()
        {
            this.OutputSysLogWithModeName("PC→装置 コンベア循環命令(QR読み取り位置→下段)送信");
            this.Plc.SetBit(this.RotationReqBitAddress, 1, Mitsubishi.BIT_ON);
            this.Plc.SetBit(this.OutputReserveBitAddress, 1, Mitsubishi.BIT_ON);
        }
        protected void sendPlcOrderUpperRotation()
        {
            if (this.Plc.GetBit(this.UpperRotaionReqBitAddress) == PLC.Common.BIT_ON)
            {
                // 装置へ指示済みの場合は、改めて送信はしない
                return;
            }

            this.OutputSysLogWithModeName("PC→装置 コンベア循環命令(下段先頭位置→上段)送信");
            this.Plc.SetBit(this.UpperRotaionReqBitAddress, 1, Mitsubishi.BIT_ON);
        }

        #endregion

        #region 仮想マガジン作成時刻から一定時間経過した仮想マガジンを更新する(checkLoaderMagazine)

        /// <summary>
        /// 仮想マガジン作成時刻から一定時間経過したLoaderマガジンを排出CV行きにする
        /// </summary>
        protected void checkLoaderMagazine()
        {
            #region

            VirtualMag[] lMagazines = VirtualMag.GetVirtualMag(this.MacNo, (int)Station.Loader);
            foreach (VirtualMag lMagazine in lMagazines)
            {
                PurgeVirtualMagazine(lMagazine);                
            }

            #endregion
        }

        /// <summary>
        /// 仮想マガジン生成から一定時間経過した空マガジンを排出CV行きにする
        /// </summary>
        protected void checkEmpLoaderMagazine()
        {
            #region

            VirtualMag[] elMagazines = VirtualMag.GetVirtualMag(this.MacNo, (int)Station.EmptyMagazineLoader);
            foreach (VirtualMag elMagazine in elMagazines)
            {
                try
                {
                    // 一定時間が経過していない場合は、処理しない
                    if ((DateTime.Now - elMagazine.LastUpdDt).TotalSeconds < LoaderMagazineDischargeCheckCycleSecond)
                    {
                        continue;
                    }                

                    // 次装置をクリア + 排出CVを追加
                    this.OutputSysLogWithModeName($"時間超過の為、空マガジンの次装置を排出CVに変更してLoader仮想マガジンに移動[マガジンNo:「{elMagazine.MagazineNo}」]");
                    elMagazine.Delete();

                    elMagazine.NextMachines.Clear();
                    elMagazine.NextMachines.AddRange(Route.GetDischargeConveyors(this.MacNo));
                    // EmptyeMagazineLoader ⇒ Loaderへ仮想マガジンを移動
                    this.Enqueue(elMagazine, Station.Loader);
                }
                catch(Exception ex)
                {
                    this.OutputSysLogWithModeName($"空マガジン排出行き振替時に異常発生した為、処理をスキップ[マガジンNo:{elMagazine.MagazineNo}]理由：{ex.ToString()}");
                }
            }
            #endregion
        }

        /// <summary>
        /// 仮想マガジン作成時刻から一定時間経過したUnLoader実マガジンを排出CV行きにする
        /// </summary>
        protected void checkUnLoaderMagazine()
        {
            #region

            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if(ulMagazine != null)
            {
                ulMagazine.LocationId = (int)Station.Unloader;
                PurgeVirtualMagazine(ulMagazine);
            }

            #endregion
        }

        /// <summary>
        /// 仮想マガジン作成時刻から一定時間経過した排出空マガジンを排出CV行きにする
        /// </summary>
        protected void checkEmpUnloaderMagazine()
        {
            #region

            VirtualMag eulMagazine = this.Peek(Station.EmptyMagazineUnloader);
            if(eulMagazine != null)
            {
                eulMagazine.LocationId = (int)Station.EmptyMagazineUnloader;
                PurgeVirtualMagazine(eulMagazine);
            }
            #endregion
        }

        /// <summary>
        /// 引数の仮想マガジンを排出CV行きにする
        /// </summary>
        private void PurgeVirtualMagazine(VirtualMag mag)
        {
            #region

            try
            {
                int checkCycleSecond = this.LoaderMagazineDischargeCheckCycleSecond;
                if (mag.LocationId == (int)Station.Unloader || mag.LocationId == (int)Station.EmptyMagazineUnloader)
                {
                    checkCycleSecond = this.UnloaderMagazineDischargeCheckCycleSecond;
                }


                // 一定時間が経過していない場合は、処理しない
                if ((DateTime.Now - mag.LastUpdDt).TotalSeconds < checkCycleSecond)
                {
                    return;
                }

                // 処理が不要かどうかを判定 (既に排出CV行きになっているか)
                List<IMachine> macList = mag.NextMachines.Select(m => LineKeeper.GetMachine(m)).ToList();
                bool processfg = false;

                if (macList.Any() == false)
                {
                    // 行先なし ⇒ 処理が必要
                    processfg = true;
                }
                else
                {
                    foreach (IMachine mac in macList)
                    {
                        if ((mac is DischargeConveyor) == false)
                        {
                            // 行先に排出CV以外を含んでいる ⇒ 処理対象
                            processfg = true;
                            break;
                        }
                    }
                }
                if (processfg == false)
                {
                    return;
                }

                // 次装置をクリア + 排出CVを追加 + 更新
                this.OutputSysLogWithModeName($"時間超過の為、仮想マガジンの次装置を排出CVに変更[位置：「{(Station)Enum.Parse(typeof(Station), mag.LocationId.ToString())}」マガジンNo:「{mag.MagazineNo}」]");
                mag.NextMachines.Clear();
                mag.NextMachines.AddRange(Route.GetDischargeConveyors(this.MacNo));
                mag.Updatequeue();
            }
            catch (Exception ex)
            {
                this.OutputSysLogWithModeName($"実マガジン排出行き振替時に異常発生した為、処理をスキップ[マガジンNo:{mag.MagazineNo}]理由：{ex.ToString()}");
            }

            #endregion
        }

        #endregion

        #region 空マガジン搬送制御 ResponseEmptyMagazineRequest

        /// <summary>
        /// 空マガジンの配置処理
        /// </summary>
        /// <param name="m"></param>
        public override bool ResponseEmptyMagazineRequest()
        {
            #region

            if (IsRequireOutputEmptyMagazine() == false)
            {
                return false;
            }

            VirtualMag mag = this.Peek(Station.EmptyMagazineUnloader);
            if (mag == null) return false;

            // 排出行の空マガジン かつ 手の届く範囲に空Mag供給の装置が無い場合、
            // この関数内で排出CVへの搬送指令を出す
            Location from = new Location(this.MacNo, Station.EmptyMagazineUnloader);

            foreach(int macno in mag.NextMachines)
            {
                if ((LineKeeper.GetMachine(macno) is DischargeConveyor) == false)
                {
                    continue;
                }

                // 次装置 = 排出CV かつ 排出CVの供給要求ONのある時は、この関数内で排出CVへの搬送命令を出す。
                IMachine dischargecv = LineKeeper.GetMachine(Route.GetDischargeConveyor(macno));
                if (dischargecv.IsRequireInput() == false)
                {
                    continue;
                }

                Location locDischargecv = dischargecv.GetLoaderLocation();
                this.OutputSysLog($"空Mag排出ONの状態で排出CV行きの為、排出します。-> {locDischargecv.DirectoryPath}");
                LineKeeper.MoveFromTo(from, locDischargecv, true, true, false);
                return true;
            }

            return false;

            #endregion
        }
        #endregion
    }
}
