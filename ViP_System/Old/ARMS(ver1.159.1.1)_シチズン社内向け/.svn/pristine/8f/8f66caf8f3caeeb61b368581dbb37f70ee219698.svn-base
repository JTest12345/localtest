using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ArmsApi;
using ArmsApi.Model;
using System.IO;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 仮想マガジンを保持する機能を持つ製造装置の基底クラス
    /// </summary>
    public abstract class MachineBase : ARMSThreadObject, IMachine
    {
        public MachineBase()
        {
            LoaderConveyorReqBitAddressList = new List<string>();
            UnloaderConveyorReqBitAddressList = new List<string>();
            LoaderConveyorMagazneArriveAddressList = new List<string>();
        }

        public IPLC Plc { get; set; }

        /// <summary>
        /// 搬送RB用のPLC
        /// </summary>
        public IPLC CarrierPlc { get; set; }

        /// <summary>
        /// コンベア用のPLC
        /// </summary>
        public IPLC ConveyorPlc { get; set; }

        /// <summary>
        /// PLC応答不能
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// 最終の応答不能時間
        /// </summary>
        public DateTime LastDisAvailableTime { get; set; }

        /// <summary>
        /// 信号確認画面のタブ名
        /// </summary>
        public string SignalDisplayFlowName { get; set; }

        #region PLC標準アドレス類
        public string MachineReadyBitAddress { get; set; }

        public string LoaderReqBitAddress { get; set; }
        public string UnLoaderReqBitAddress { get; set; }
        public string EmpMagLoaderReqBitAddress { get; set; }
        public string EmpMagUnLoaderReqBitAddress { get; set; }

        public string WorkStartTimeAddress { get; set; }
        public List<string> WorkStartTimeAddressList { get; set; }
        public string WorkCompleteTimeAddress { get; set; }
        public List<string> WorkCompleteTimeAddressList { get; set; }
        public string ClearMagazineBitAddress { get; set; }
        public string InputForbiddenBitAddress { get; set; }
        public string DischargeModeBitAddress { get; set; }
        public string PrioritySettingAddress { get; set; }
        public string SendMachineReadyBitAddress { get; set; }

        //投入前排出
        public string PreInputDischargeModePlcAddress { get; set; }
        public string PreInputDischargeModePlcPort { get; set; }
        public string PreInputDischargeModeBitAddress { get; set; }

        //移動ポイント
        public string LoaderPoint { get; set; }
        public string UnloaderPoint { get; set; }
        public string EmptyMagLoaderPoint { get; set; }
        public string EmptyMagUnloaderPoint { get; set; }

        public string MacPoint { get; set; }

        /// <summary>
        /// リニア番号
        /// </summary>
        public string LinearNo { get; set; }

        /// <summary>
        /// 装置隣接CVマガジン到達信号アドレス
        /// </summary>
        public List<string> LoaderConveyorMagazneArriveAddressList { get; set; }

        /// <summary>
        /// 装置隣接CV(棚)供給信号アドレス
        /// </summary>
        public List<string> LoaderConveyorReqBitAddressList { get; set; }

        /// <summary>
        /// 装置隣接CV(棚)排出信号アドレス
        /// </summary>
        public List<string> UnloaderConveyorReqBitAddressList { get; set; }

        /// <summary>
        /// 装置隣接CV(棚)の存在
        /// </summary>
        public bool IsConveyor { get; set; }

        /// <summary>
        /// マガジンが装置隣接CV(棚)に到達したかの記憶
        /// </summary>
        public bool IsConveyorMagazneArriveMemory { get; set; }


        public Dictionary<string, bool> loaderConveyorRequestBitMemory { get; set; }

        // ロボットチャック開閉中
        public string DoingOpenRobotArmBitAddress { get; set; }
        public string DoingCloseRobotArmBitAddress { get; set; }

        // Mag供給・排出中
        public string LoaderDoBitAddress { get; set; }
        public string UnloaderDoBitAddress { get; set; }
        public string EmpMagLoaderDoBitAddress { get; set; }
        public string EmpMagUnloaderDoBitAddress { get; set; }

        // ロボット⇒装置 マガジンNo転送先
        public string SendLMagazineAddress { get; set; }
        public string SendULMagazineAddress { get; set; }

        /// <summary>
        /// 装置⇒ロボット Mag受渡完了
        /// </summary>
        public string MagazineDestinationCompltBitAddress { get; set; }

        /// <summary>
        // ロボット⇒装置 扉開許可
        /// </summary>
        public string SendDoorCanOpenAddress { get; set; }

        /// <summary>
        /// 全扉閉中 (Robot3の搬送処理で使用)   0：開  1：閉
        /// </summary>
        public string OvenAllDoorCloseBitAddress { get; set; }

        /// <summary>
        /// 実Mag供給可能信号BITアドレス
        /// </summary>
        public string LoaderCanBitAddress { get; set; }
        /// <summary>
        /// 空Mag供給可能信号BITアドレス
        /// </summary>
        public string EmpMagLoaderCanBitAddress { get; set; }
        /// <summary>
        /// 実Mag排出可能信号BITアドレス
        /// </summary>
        public string UnloaderCanBitAddress { get; set; }
        /// <summary>
        /// 空Mag排出可能信号BITアドレス
        /// </summary>
        public string EmpMagUnloaderCanBitAddress { get; set; }

        /// <summary>
        /// 接続要求信号BITアドレス
        /// </summary>
        public string ConnectionReqBitAddress { get; set; }
        /// <summary>
        /// 切断要求信号BITアドレス
        /// </summary>
        public string DisconnectionReqBitAddress { get; set; }
        /// <summary>
        /// 出力要求信号BITアドレス
        /// </summary>
        public string ReadDischargePressureDataReqBitAddress { get; set; }
        /// <summary>
        /// 接続中信号BITアドレス
        /// </summary>
        public string InConnectionBitAddress { get; set; }
        #endregion

        public int Priority { get; set; }

        /// <summary>
        /// 最終のマガジン投入
        /// </summary>
        public DateTime LastMagazineSetTime { get; set; }

        public MachineType MachineType { get; set; }

		///// <summary>
		///// ライン種類(自動化=1、高効率=2)
		///// </summary>
		//public int LineGroupNo { get; set; }

		public bool IsAutoLine { get; set; }
		public bool IsHighLine { get; set; }
		public bool IsOutLine { get; set; }
        public bool IsAgvLine { get; set; }

        /// <summary>
        /// ライン番号
        /// </summary>
        public string LineNo { get; set; }

        /// <summary>
        /// 装置グループ
        /// </summary>
        public List<string> MacGroup { get; set; }

        /// <summary>
        /// 最終のマガジン排出
        /// </summary>
        public DateTime LastOutputTime { get; set; }

        //2016.09.29 フォルダの持ち方変更のため廃止
        //public string WatchingDirectoryPath { get; set; }
        /// <summary>
        /// 傾向管理・マッピングインプットフォルダ
        /// </summary>
        public string LogInputDirectoryPath { get; set; }

        /// <summary>
        /// 傾向管理・マッピングアウトプットフォルダ
        /// </summary>
        public string LogOutputDirectoryPath { get; set; }

        /// <summary>
        /// マッピングインプットフォルダ【※傾向管理とマッピングの出力先が異なる場合のみ使用】
        /// </summary>
        public string AltMapInputDirectoryPath { get; set; }

        /// <summary>
        /// マッピングアウトプットフォルダ【※傾向管理とマッピングの出力先が異なる場合のみ使用】
        /// </summary>
        public string AltMapOutputDirectoryPath { get; set; }

        public string CsvInputDirectoryPath { get; set; }

        /// <summary>
        /// 基板作業処理ありか否かフラグ
        /// </summary>
        public bool IsSubstrateComplete { get; set; }

        public string LoaderConveyorStopAddress { get; set; }

        public string IsUnloaderMagazineExistsAddress { get; set; }

        /// <summary>
        /// CV⇒装置へマガジン移動必要(前回値記憶)
        /// </summary>
        public bool IsMoveStartMagazineMemory { get; set; }

        ///// <summary>
        ///// 作業完了ファイル書き出しフォルダ
        ///// </summary>
        //public string MachineCompleteWorkDir { get; set; }

        public virtual bool Enqueue(VirtualMag mag, Station station)
        {           
            List<VirtualMag> mags = VirtualMag.GetVirtualMag(this.MacNo, ((int)station)).ToList();
            if (mags.Exists(m => m.MagazineNo == mag.MagazineNo)) 
            {
                Log.SysLog.Info(string.Format("既に同一の仮想マガジンが存在する為Enqueue中断:{0}", mag.MagazineNo));
                return false;
            }

            Location loc = new Location(this.MacNo, station);
            mag.Enqueue(mag, loc);
            mag.CurrentLocation = loc;

            return true;
        }

        public virtual VirtualMag Dequeue(Station station)
        {
            Location loc = new Location(this.MacNo, station);
            VirtualMag mag = new VirtualMag();
            mag = mag.Dequeue(loc);

            if (mag == null)
            {
                if (station == Station.EmptyMagazineUnloader)
                {
                    //空マガジン
                    mag = new VirtualMag();
                    mag.CurrentLocation = new Location(this.MacNo, station);
                }
                else
                {
                    //存在しないキューの先頭を取得
                }
            }
            else
            {

            }

            return mag;
        }

        public virtual VirtualMag Peek(Station station)
        {
            Location location = new Location(this.MacNo, station);
            VirtualMag mag = new VirtualMag();
            return mag.Peek(location);
        }

        public bool WorkComplete(VirtualMag mag, IMachine machine, bool isSubmit) 
        {
            if (mag.NextMachines.Count == 0)
            {
                Order order = CommonApi.GetWorkEndOrder(mag, machine.MacNo, machine.LineNo);
                
                //途中投入CVで次作業の開始登録がされている場合は排出CVへ
                if (machine is LoadConveyor)
                {
                    //ArmsApiResponse status = CommonApi.IsNextWorkStarted(mag.MagazineNo, order, mag);
                    //if (status.IsError)
                    if (CommonApi.IsNextWorkStarted(mag.MagazineNo))
                    {
                        //すでに開始時間が入っている場合
                        //Log.ApiLog.Info(status.Message);
                        mag.NextMachines.Clear();
                        mag.NextMachines.Add(Route.GetDischargeConveyor(this.MacNo));
                        mag.PurgeReason = "次の作業が開始されています";
                        mag.Updatequeue();
                        return false;
                    }
                }

                //作業完了登録
                if (isSubmit) 
                {
                    ArmsApiResponse workResponse = CommonApi.WorkEnd(order);
                    if (workResponse.IsError)
                    {
                        if (this.IsAutoLine)
                        {
                            FrmErrHandle frmErr = new FrmErrHandle(workResponse);
                            frmErr.ShowDialog();
                        }
                        else
                        {
                            OutputSysLog(string.Format("完了処理失敗：マガジンNo= {0},  理由= {1}", mag.MagazineNo, workResponse.Message));
                        }
                        return false;
                    }
                }

                //搭載機、途中投入CV、高効率装置の場合、ロットに装置グループ登録
                if (machine is MAP1stDieBonder || machine is FrameLoader || machine is LoadConveyor || machine.IsHighLine || machine is SubstrateLoader)
                {
                    Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
                    AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                    svrlot.MacGroup = machine.MacGroup;
                    svrlot.Update();
                }

                //始業点検CVの場合、自装置のローダーへ
                if (machine is OpeningCheckConveyor)
                {
                    mag.NextMachines.Clear();
                    mag.NextMachines.Add(((OpeningCheckConveyor)machine).ParentMacNo);
                    mag.Updatequeue();
                    return true;
                }


                if (Config.Settings.SetPSTesterProcNo.HasValue &&
                    Config.Settings.CancelRestrictPSTesterFunctionIdList != null &&
                    Config.Settings.CancelRestrictPSTesterFunctionIdList.Count != 0)
                {
                    //途中投入CVの場合、強度試験対象ロットで、Pstesterの実績がNG以外になっていれば規制解除
                    if (machine is LoadConveyor)
                    {
                        try
                        {
                            Restrict.CancelRestrictionPSTester(mag.MagazineNo);
                        }
                        catch (Exception ex)
                        {
                            //ログだけ残してエラーは表示しないようにする
                            OutputSysLog(string.Format("途中投入CVかつ強度試験対象ロットの場合に、Pstesterの実績がNG以外になっていれば規制解除をする処理失敗：マガジンNo= {0},  理由= {1}", mag.MagazineNo, ex.Message + ":" + ex.StackTrace));
                        }
                    }
                }


                //次装置決定
                ArmsApiResponse res = CommonApi.QueryNextWork(order, mag);
                List<int> nextMachines = res.NextMachines;

                //強度試験対象であればAGVユニットへ 2018.08.21
                if (Config.Settings.SetPSTesterProcNo.HasValue && Config.Settings.SetPSTesterProcNo.Value == order.ProcNo)
                {
                    Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
                    if (svrmag != null)
                    {
                        AsmLot checkLot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                        if (checkLot != null)
                        {
                            Process regulationProc = Process.GetNextProcess(Config.Settings.SetPSTesterProcNo.Value, checkLot);
                            if (Restrict.GetPSTesterRestrictLot(svrmag.NascaLotNO, regulationProc.ProcNo, true) != null)
                            {
                                //AGV供給ユニットを指定するのではなく他ラインへ持っていく処理と同じにする
                                nextMachines.Clear();
                                nextMachines.Add(Route.GetAutoLineOutConveyor(order.MacNo));
                                OutputSysLog(string.Format("強度試験対象であるためAGVユニットへ排出：マガジンNo= {0},  ロットNo= {1}, 工程No= {2}", mag.MagazineNo, order.NascaLotNo, order.ProcNo));

                                //AGV供給ユニットへ運ぶためには排出理由が空白の必要がある
                                mag.PurgeReason = "";
                            }
                        }
                    }
                }

                mag.NextMachines = nextMachines;
                mag.ProcNo = res.ProcNo;
                OutputSysLog(string.Format("次装置【r】：{0}", string.Join(",", res.NextMachines)));
                mag.Updatequeue();
            }

			return true;
        }

		public bool WorkStart(VirtualMag mag, IMachine machine) 
		{
			Order order = CommonApi.GetWorkStartOrder(mag, machine.MacNo);

			if (mag.WorkStart.HasValue)
			{
				order.WorkStartDt = mag.WorkStart.Value;
			}

			ArmsApiResponse workResponse = CommonApi.WorkStart(order);
			if (workResponse.IsError)
			{
				Log.ApiLog.Info(workResponse.Message);
				return false;
			}

			return true;
		}

        #region 装置要求確認　IsRequireInput, Output, EmptyMag, IsInputForbidde, IsDischargeMode

        /// <summary>
        /// 投入要求確認
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRequireInput()
        {
            if (string.IsNullOrEmpty(this.LoaderReqBitAddress) == true)
            {
                return false;
            }

            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            //string retv = Plc.GetBit(this.LoaderReqBitAddress);
            string retv;
            try
            {
                retv = Plc.GetBit(this.LoaderReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{this.LoaderReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 投入要求確認(装置隣接CV、棚)
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRequireConveyorInput(IPLC carrierPlc)
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            if (IsConveyor == false)
            {
                // 装置隣接CV(棚)が存在しないラインから呼ばれた場合、OKを返す
                return true;
            }

            IPLC plc = carrierPlc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }

            foreach (string address in this.LoaderConveyorReqBitAddressList)
            {
                //string retv = plc.GetBit(address);
                string retv;
                try
                {
                    retv = plc.GetBit(address);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{address}』, エラー内容：{ex.Message}");
                    retv = Mitsubishi.BIT_OFF;
                }

                if (retv == PLC.Common.BIT_ON)
                {
                    return true;
                }
            }
            return false;
        }

        #region IsDischargeMode
        /// <summary>
        /// 完了後排出モード判定。Mag=nullでも可能
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
		public virtual bool IsDischargeMode(VirtualMag mag)
        {
            if (string.IsNullOrEmpty(this.DischargeModeBitAddress) == true)
            {
                return false;
            }

            string retv = string.Empty;
            if (this.CarrierPlc != null)
            {
                // 指定PLC(搬送パネルを想定)を使用
                retv = this.CarrierPlc.GetBit(this.DischargeModeBitAddress);
            }
            else
            {
                // 装置PLCを使用
                retv = Plc.GetBit(this.DischargeModeBitAddress);
            }

            if (retv == PLC.Common.BIT_ON)
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
        /// 投入前排出モードの判定。　Mag=nullでも可能
        /// モールド投入前に抜き取り検査の検査機排出モード要求確認
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        public virtual bool IsPreInputDischargeMode(VirtualMag mag)
        {
            if (string.IsNullOrEmpty(this.PreInputDischargeModeBitAddress)
                || string.IsNullOrEmpty(this.PreInputDischargeModePlcAddress)
                || string.IsNullOrEmpty(this.PreInputDischargeModePlcPort))
                return false;

            //自搬送機ではなく、lineConfig指定の搬送機アドレスを参照する
            Mitsubishi plc = new Mitsubishi(this.PreInputDischargeModePlcAddress, int.Parse(this.PreInputDischargeModePlcPort));
            string retv = plc.GetBit(this.PreInputDischargeModeBitAddress);
            if (retv == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region IsInputForbidden

        /// <summary>
        /// 供給禁止モード
        /// こちらがONの場合は搬送動作自体が発生しない
        /// </summary>
        /// <returns></returns>
        public bool IsInputForbidden()
        {
            string retv = string.Empty;

            if (string.IsNullOrEmpty(this.InputForbiddenBitAddress) == true)
            {
                return false;
            }

            if (this.CarrierPlc != null)
            {
                // 指定PLC(搬送パネルを想定)を使用
                retv = this.CarrierPlc.GetBit(this.InputForbiddenBitAddress);
            }
            else
            {
                // 装置PLCを使用
                retv = Plc.GetBit(this.InputForbiddenBitAddress);
            }

            if (retv == PLC.Common.BIT_ON)
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
        /// 排出要求確認
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRequireOutput()
        {
            //排出要求信号の確認
            if (string.IsNullOrEmpty(this.UnLoaderReqBitAddress) == true)
            {
                return false;
            }

            //string retv = Plc.GetBit(this.UnLoaderReqBitAddress);
            string retv;
            try
            {
                retv = Plc.GetBit(this.UnLoaderReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{this.UnLoaderReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv != Mitsubishi.BIT_READ_TIMEOUT_VALUE)
            {
                if (retv == Mitsubishi.BIT_ON)
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
                //通信エラー
                return false;
            }
        }

        /// <summary>
        /// 排出要求確認(装置隣接CV、棚)
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRequireConveyorOutput()
        {
            if (IsConveyor == false)
            {
                // 装置隣接CV(棚)が存在しないラインから呼ばれた場合、OKを返す
                return true;
            }

            IPLC plc = this.Plc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }

            foreach (string address in this.UnloaderConveyorReqBitAddressList)
            {
                //string retv = plc.GetBit(address);
                string retv;
                try
                {
                    retv = plc.GetBit(address);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、排出要求OFF扱い。アドレス：『{address}』, エラー内容：{ex.Message}");
                    retv = Mitsubishi.BIT_OFF;
                }

                if (retv == Mitsubishi.BIT_ON)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 空マガジン要求
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRequireEmptyMagazine()
        {
            if (string.IsNullOrEmpty(this.EmpMagLoaderReqBitAddress) == true)
            {
                return false;
            }

            //string retv = Plc.GetBit(this.EmpMagLoaderReqBitAddress);
            string retv;
            try
            {
                retv = Plc.GetBit(this.EmpMagLoaderReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、空供給要求OFF扱い。アドレス：『{this.EmpMagLoaderReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv != Mitsubishi.BIT_READ_TIMEOUT_VALUE)
            {
                if (retv == Mitsubishi.BIT_ON)
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
                //通信エラー
                return false;
            }
        }

        /// <summary>
        /// 空マガジン排出要求
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRequireOutputEmptyMagazine()
        {
            if (string.IsNullOrEmpty(this.EmpMagUnLoaderReqBitAddress) == true)
            {
                return false;
            }

            string retv;
            try
            {
                retv = Plc.GetBit(this.EmpMagUnLoaderReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、空排出要求OFF扱い。アドレス：『{this.EmpMagUnLoaderReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv == Mitsubishi.BIT_READ_TIMEOUT_VALUE)
            {
                return false;
            }
            else
            {
                if (retv == Mitsubishi.BIT_ON)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Ready信号
        /// </summary>
        /// <returns></returns>
        public virtual bool IsReady()
        {
            //string retv = Plc.GetBit(this.MachineReadyBitAddress);
            string retv;
            try
            {
                retv = Plc.GetBit(this.MachineReadyBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、Ready要求OFF扱い。アドレス：『{this.MachineReadyBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv != Mitsubishi.BIT_READ_TIMEOUT_VALUE)
            {
                if (retv == Mitsubishi.BIT_ON)
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
                //通信エラー
				Log.SysLog.Info("通信エラー");
                return false;
            }
        }

        #endregion

        /// <summary>
        /// PLCから優先度設定を取得
        /// </summary>
        /// <param name="plc"></param>
        public void ReloadPriority()
        {
            if (string.IsNullOrEmpty(this.PrioritySettingAddress) == true)
            {
                return;
            }

            if (this.CarrierPlc != null)
            {
                // ロボット用のPLCを参照する
                this.Priority = this.CarrierPlc.GetWordAsDecimalData(this.PrioritySettingAddress);
            }
            else
            {
                // lineConfig指定の搬送機アドレスを参照する
                this.Priority = Plc.GetWordAsDecimalData(this.PrioritySettingAddress);
            }

            Log.RBLog.Info(string.Format("Priority reset {0} : {1}", this.MacNo, this.Priority));
        }

        public virtual Location GetLoaderLocation()
        {
            return new Location(this.MacNo,　Station.Loader);
        }

        public virtual Location GetUnLoaderLocation()
        {
            return new Location(this.MacNo, Station.Unloader);
        }

        public virtual List<VirtualMag> GetMagazines(Station st) 
        {
            VirtualMag[] mags = VirtualMag.GetVirtualMag(this.MacNo, ((int)st));
            return mags.ToList();
        }

        /// <summary>
        /// マガジン指定で供給可能か確認
        /// （内部のNextProcNoを使って判定）
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        public virtual bool CanInput(VirtualMag mag)
        {
            return true;
        }

        #region ResponseEmptyMagazineRequest
        /// <summary>
        /// 空マガジンの配置処理
        /// </summary>
        /// <param name="m"></param>
        public virtual bool ResponseEmptyMagazineRequest()
        {
            //供給禁止状態なら処理しない
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            if (this.IsRequireEmptyMagazine() == true)
            {
                Location from = null;
                
                //List<LineBridge> bridgeList = LineKeeper.GetReachBridges(this.MacNo);
                List<LineBridge> bridgeList = LineKeeper.Machines.Where(m => m is LineBridge).Select(m => (LineBridge)m).ToList();
                bool IsDeleteFromMag = false;

                //自装置の空マガジンを使用
                if (this.IsRequireOutputEmptyMagazine() == true)
                {
                    from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
                    //IsDeleteFromMag = true;
                }
                //ライン連結橋の空マガジンを使用
                else if (bridgeList.Count() > 0)
                {
                    foreach(LineBridge bridge in bridgeList)
                    {
                        if (bridge.IsRequireOutputEmptyMagazine() == false) continue;
                        //先頭が遠心沈降マガジン or アオイ基板マガジンなら処理しない
                        VirtualMag mag = bridge.Peek(bridge.GetUnLoaderLocation().Station);
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
                        IsDeleteFromMag = true;

                        break;
                    }
                }
                //空マガジン投入CVの空マガジンを使用
                if (from == null)
                {
                    //空マガジン投入CVの状態確認
                    List<int> emptyMagazineLoadConveyorMacNoList = Route.GetEmptyMagazineLoadConveyors(this.MacNo);
                    foreach(int macNo in emptyMagazineLoadConveyorMacNoList)
                    {
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
                                if (bridgeMags.Count != 0)
                                {
                                    //if (bridgeMags.Where(m => Magazine.GetCurrent(m.MagazineNo) == null).Count() != 0)
                                    if (bridgeMags.Any(m => Magazine.GetCurrent(m.MagazineNo) == null && VirtualMag.IsECKMag(m.MagazineNo) == false))
                                    {
                                        return false;
                                    }
                                }
                            }

                            from = new Location(empMagLoadConveyor.MacNo, Station.EmptyMagazineUnloader);

                            // 循環式投入CVは空マガジン排出時に仮想マガジンを作成するので
                            // 橋と同じ扱いにする (仮想マガジンを削除する)
                            if (empMagLoadConveyor is HybridLoadConveyor)
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

                Location to = new Location(this.MacNo, Station.EmptyMagazineLoader);
                LineKeeper.MoveFromTo(from, to, IsDeleteFromMag, true, false);

                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 仮想マガジンの削除要求応答

        /// <summary>
        /// 仮想マガジン消去要求への応答
        /// </summary>
        public void ResponseClearMagazineRequest()
        {
            if (string.IsNullOrEmpty(this.ClearMagazineBitAddress) == true)
            {
                return;
            }

            IPLC iPlc;
            if (this.CarrierPlc != null)
            {
                // ロボット用のPLCを参照する
                iPlc = this.CarrierPlc;
            }
            else
            {
                // 装置用PLCを参照する
                iPlc = this.Plc;
            }

            if (iPlc.GetBit(this.ClearMagazineBitAddress) == Mitsubishi.BIT_ON)
            {
                // 【N工場MAP J9・10不具合 修正】
                List<IMachine> targetMacList = new List<IMachine>();
                foreach (IMachine imac in LineKeeper.Machines)
                {
                    // マガジン記憶解除のアドレスが違う装置は対象外
                    if (this.ClearMagazineBitAddress != imac.ClearMagazineBitAddress) continue;

                    // PLCのIPアドレス・ポートが違う装置も対象外 (ロボット用PLC優先)
                    IPLC tPlc;
                    if (imac.CarrierPlc != null) tPlc = imac.CarrierPlc;
                    else tPlc = imac.Plc;

                    if (iPlc.IPAddress != tPlc.IPAddress || iPlc.Port != tPlc.Port) continue;

                    targetMacList.Add(imac);
                }

                foreach (IMachine imac in targetMacList)
                {
                    Log.SysLog.Info(string.Format("[仮想マガジン削除]Start 装置NO:{0} IPAddress:{1} BitAddress:{2}",
                        imac.MacNo, iPlc.IPAddress, imac.ClearMagazineBitAddress));

                    imac.ClearVirtualMagazines();
                }

                //Log.SysLog.Info(string.Format("[仮想マガジン削除]Start 装置NO:{0} IPAddress:{1} BitAddress:{2}",
                //    this.MacNo, this.Plc.IPAddress, this.ClearMagazineBitAddress));

                //ClearVirtualMagazines();
                iPlc.SetBit(this.ClearMagazineBitAddress, 1, Mitsubishi.BIT_OFF);

                Log.SysLog.Info(string.Format("[仮想マガジン削除]End 装置NO:{0}", this.MacNo));
            }
        }

        /// <summary>
        /// 仮想マガジンのクリア
        /// </summary>
        public virtual void ClearVirtualMagazines()
        {
            foreach (Station st in Enum.GetValues(typeof(Station)))
            {
                VirtualMag m = Dequeue(st);
                //全ステーションから削除
                while (m != null)
                {
                    //EmptyMagUnloaderが無限に取得できる装置用
                    //OvenのUNKNOWN番号は正常に存在する仮想ファイルなので通常通り処理
                    if (string.IsNullOrEmpty(m.MagazineNo) == true)
                    {
                        break;
                    }
                    m = Dequeue(st);
                }
            }
        }
        public virtual void ClearVirtualMagazines(Station targetST)
        {
            foreach (Station st in Enum.GetValues(typeof(Station)))
            {
                if (targetST != st) { continue; }

                VirtualMag m = Dequeue(st);
                //全ステーションから削除
                while (m != null)
                {
                    //EmptyMagUnloaderが無限に取得できる装置用
                    //OvenのUNKNOWN番号は正常に存在する仮想ファイルなので通常通り処理
                    if (string.IsNullOrEmpty(m.MagazineNo) == true)
                    {
                        break;
                    }
                    m = Dequeue(st);
                }
            }
        }

        #endregion

        public virtual string GetFromToCode(Station station)
        {
            switch (station)
            {
                case Station.Loader:
                    return LoaderPoint;

                case Station.Unloader:
                    return UnloaderPoint;

                case Station.EmptyMagazineLoader:
                    return EmptyMagLoaderPoint;

                case Station.EmptyMagazineUnloader:
                    return EmptyMagUnloaderPoint;                    
            }

            throw new ApplicationException("定義外のStationのGetFromToCode");
        }

        #region GetBufferCode

        /// <summary>
        /// 排出側バッファ位置取得 (排出信号ONの位置)
        /// </summary>
        /// <returns></returns>
        public string GetFromBufferCode(int retryCt)
        {
            if (retryCt >= 10)
            {
                throw new ApplicationException("装置CVの排出信号OFFの状態でGetFromBufferCode関数が呼び出されました。処理に問題ないか確認して下さい。");
            }

            int? locationId = null;

            if (IsConveyor)
            {
                for (int i = 0; i < this.UnloaderConveyorReqBitAddressList.Count; i++)
                {
                    if (this.Plc.GetBit(this.UnloaderConveyorReqBitAddressList[i]) == PLC.Common.BIT_ON)
                    {
                        locationId = i + 1;
                        break;
                    }
                }
            }
            else
            {
                return "1";
            }

            if (locationId.HasValue == false)
            {
                Thread.Sleep(1000);
                retryCt = retryCt + 1;
                return GetFromBufferCode(retryCt);
            }

            return locationId.Value.ToString();
        }

        public string GetFromBufferCode()
        {
            return GetFromBufferCode(0);
        }

        /// <summary>
        /// 供給側バッファ位置取得 (供給信号ONの位置)
        /// </summary>
        /// <returns></returns>
        public string GetToBufferCode(int retryCt)
        {
            if (retryCt >= 10)
            {
                throw new ApplicationException("装置CVの供給信号OFFの状態でGetFromBufferCode関数が呼び出されました。処理に問題ないか確認して下さい。");
            }

            int? locationId = null;

            if (IsConveyor)
            {
                for (int i = 0; i < this.LoaderConveyorReqBitAddressList.Count; i++)
                {
                    if (this.Plc.GetBit(this.LoaderConveyorReqBitAddressList[i]) == PLC.Common.BIT_ON)
                    {
                        locationId = i + 1;
                        break;
                    }
                }
            }
            else
            {
                if (this.Plc.GetBit(this.LoaderReqBitAddress) == PLC.Common.BIT_ON)
                {
                    locationId = 1;
                }
            }

            if (locationId.HasValue == false)
            {
                Thread.Sleep(1000);
                retryCt = retryCt + 1;
                return GetToBufferCode(retryCt);
            }

            return locationId.Value.ToString();
        }

        public string GetToBufferCode()
        {
            return GetToBufferCode(0);
        }

        /// <summary>
        /// 装置No取得 (Robot3用 ステーション指定)
        /// </summary>
        /// <returns></returns>
        public virtual string GetMacPoint(Station station)
        {
            return this.MacPoint;
        }

        /// <summary>
        /// バッファ位置取得 (Robot3用 ステーション指定)
        /// </summary>
        /// <returns></returns>
        public virtual string GetFromToBufferCode(Station station)
        {
            switch (station)
            {
                case Station.Loader:
                case Station.Unloader:
                case Station.EmptyMagazineLoader:
                case Station.EmptyMagazineUnloader:
                    return "1";
            }
            throw new ApplicationException("定義外のStationのGetToFromBufferCode");
        }


        #endregion

        //#region PDAに完了信号を送る SendCompleteJobData

        ///// <summary>
        ///// 完了マガジンデータをPDAに通知
        ///// </summary>
        ///// <param name="job"></param>
        //public virtual void SendCompleteJobData(VirtualMag mag)
        //{
        //	try
        //	{
        //		ArmsApi.Model.NASCA.NASCAJob job = ArmsApi.Model.NASCA.NASCAJob.GetWorkCompleteJob(mag);
        //		//Log.WriteNASCALog("[FILE-OUT]:" + job.GetCommandText());
        //		string command = job.GetCommandText();

        //		using (StreamWriter sw = new StreamWriter(Path.Combine(this.MachineCompleteWorkDir,  DateTime.Now.Ticks.ToString() + ".csv")))
        //		{
        //			sw.WriteLine(job.GetCommandText());
        //		}
        //	}
        //	catch (Exception ex)
        //	{
        //		Log.ApiLog.Error("NASCA送受信エラー", ex);
        //		throw ex;
        //	}
        //}

        //#endregion

        public void OutputSysLog(string message) 
		{
			Log.SysLog.Info(string.Format("{0} {1}", this.Name, message));
		}

        public void OutputApiLog(string message)
        {
            Log.ApiLog.Info(string.Format("{0} {1}", this.Name, message));
        }

		public void SendInFile(string directoryPath, DateTime fileDate)
		{
			sendFile(directoryPath, fileDate,  "in", "");
		}

		public void SendOutFile(string directoryPath, DateTime fileDate)
		{
			sendFile(directoryPath, fileDate, "out", "");
		}

		private void sendFile(string directoryPath, DateTime? fileDate, string instructionChar, string content)
		{
			string fd = System.DateTime.Now.ToString("yyyyMMddHHmmss");
			if (fileDate.HasValue)
			{
				fd = fileDate.Value.ToString("yyyyMMddHHmmss");
			}

			string fileName = string.Format("{0}.{1}", fd, instructionChar);
			StreamWriter sw = new StreamWriter(Path.Combine(directoryPath, fileName), true, Encoding.UTF8);
			sw.WriteLine(content);
			sw.Close();
			
			OutputSysLog(string.Format("命令ファイルを送信 区分:{0} 送信フォルダ:{1}", instructionChar, directoryPath));
        }

        /// <summary>
		/// 装置隣接CV⇒装置へマガジン移動要求(CVマガジン到達信号ON)
		/// </summary>
		/// <returns></returns>
		private bool isRequireMoveStartMagazine()
        {
            bool retv = false;

            if (this.isLoaderConveyorMagazineArrived())
            {
                return retv = true;
            }

            return retv;
        }

        /// <summary>
		/// 装置隣接CV マガジン到達信号確認
		/// </summary>
		/// <returns></returns>
		private bool isLoaderConveyorMagazineArrived()
        {
            IPLC plc = this.Plc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }

            foreach (string bitAddress in this.LoaderConveyorMagazneArriveAddressList)
            {
                if (plc.GetBit(bitAddress) == PLC.Common.BIT_ON)
                {
                    IsConveyorMagazneArriveMemory = true;
                    return true;
                }
            }

            if (IsConveyorMagazneArriveMemory)
            {
                // 到達後、センサー誤動作でOFFになる可能性があるため、2秒後再確認する
                Thread.Sleep(2000);
                foreach (string bitAddress in this.LoaderConveyorMagazneArriveAddressList)
                {
                    if (plc.GetBit(bitAddress) == PLC.Common.BIT_ON)
                    {
                        IsConveyorMagazneArriveMemory = true;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 装置隣接CV⇒装置へマガジン移動必要/不要の状態を更新
        /// </summary>
        public void StartMagazineMoveStatusUpdate()
        {
            bool fg = this.isRequireMoveStartMagazine();
            if (fg != this.IsMoveStartMagazineMemory)
            {
                MachineInfo.UpdateMoveStartMagazineFlag(this.MacNo, fg);
                this.IsMoveStartMagazineMemory = fg;
            }
        }

        public void CreateWorkStartRecord(VirtualMag mag, DateTime startDt)
        {
            Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
            if (svrmag == null) throw new ApplicationException("[開始登録異常] マガジン情報が見つかりません" + mag.MagazineNo);

            AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

            mag.ProcNo = nextproc.ProcNo;
            mag.WorkStart = startDt;

            Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);

            ArmsApiResponse workResponse = CommonApi.WorkStart(order);
            if (workResponse.IsError)
            {
                OutputSysLog(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo, workResponse.Message));
                throw new ApplicationException(workResponse.Message);
            }

            OutputSysLog(string.Format("[開始登録] 作業記録 MagazineNo:{0} 開始日時:{1} ", mag.MagazineNo, mag.WorkStart.Value));
        }

        public bool IsRequireMagazineOutput()
        {
            if ((this.IsConveyor == true && this.IsRequireConveyorOutput() == true)
                || (this.IsConveyor == false && this.IsRequireOutput() == true))
            {
                VirtualMag mag = this.Peek(Station.Unloader);
                if (mag == null)
                {
                    return false;
                }
                else if (mag.NextMachines.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsRequireConveyorWorkStart()
        {
            // 供給信号がON⇒OFFに変化又はCV到達信号OFF、1度はCV先端に到達済みが条件
            if ((IsChangeLoaderRequest() || isLoaderConveyorMagazineArrived() == false) &&
                IsConveyorMagazneArriveMemory)
            {
                IsConveyorMagazneArriveMemory = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public VirtualMag GetNonStartLoaderMagazine()
        {
            VirtualMag[] ldMagList = VirtualMag.GetVirtualMag(this.MacNo, (int)Station.Loader);
            if (ldMagList.Count() == 0)
            {
                return null;
            }
            ldMagList = ldMagList.OrderBy(l => l.orderid).ToArray();

            foreach (VirtualMag ldMag in ldMagList)
            {
                if (ldMag.WorkStart.HasValue == false)
                {
                    return ldMag;
                }
            }
            return null;
        }

        /// <summary>
        /// 装置隣接棚の供給要求変化を確認
        /// </summary>
        /// <param name="isSave">記憶する場合はTrue</param>
        /// <returns></returns>
        private bool isChangeLoaderRequest(bool isSave)
        {
            if (LoaderConveyorReqBitAddressList.Count == 1)
            {
                // 装置隣接CVの場合は到達信号を見る為、呼ばれた場合は変化無しとして返す
                return false;
            }

            Dictionary<string, bool> loaderConveyorReqBitAddressList = new Dictionary<string, bool>();
            foreach (string bitAddress in this.LoaderConveyorReqBitAddressList)
            {
                if (this.Plc.GetBit(bitAddress) == PLC.Common.BIT_ON)
                {
                    loaderConveyorReqBitAddressList.Add(bitAddress, true);
                }
                else
                {
                    loaderConveyorReqBitAddressList.Add(bitAddress, false);
                }
            }

            if (loaderConveyorRequestBitMemory.Count == 0)
            {
                loaderConveyorRequestBitMemory = loaderConveyorReqBitAddressList;
                OutputSysLog(string.Format("[供給信号変化時記憶] 起動時:{0}", string.Join(",", loaderConveyorRequestBitMemory.Values)));

                return false;
            }
            else
            {
                // 記憶と取得値に変化点がある、供給ONの数が増加した場合はTrueを返す
                // ※変化点は必ずメモリ記憶する
                foreach (KeyValuePair<string, bool> bit in loaderConveyorRequestBitMemory)
                {
                    if (loaderConveyorReqBitAddressList[bit.Key] != bit.Value)
                    {
                        if (loaderConveyorRequestBitMemory.Where(b => b.Value == true).Count() <= loaderConveyorReqBitAddressList.Where(b => b.Value == true).Count())
                        {
                            if (isSave)
                            {
                                loaderConveyorRequestBitMemory = loaderConveyorReqBitAddressList;
                            }
                            OutputSysLog(string.Format("[供給信号変化時記憶] 供給ON増加時:{0}", string.Join(",", loaderConveyorRequestBitMemory.Values)));
                            return true;
                        }
                        else
                        {
                            if (isSave)
                            {
                                loaderConveyorRequestBitMemory = loaderConveyorReqBitAddressList;
                            }
                            OutputSysLog(string.Format("[供給信号変化時記憶] 供給ON減少時:{0}", string.Join(",", loaderConveyorRequestBitMemory.Values)));
                            return false;
                        }
                    }
                }
                return false;
            }
        }

        public bool IsChangeLoaderRequest()
        {
            bool retv = isChangeLoaderRequest(false);
            if (retv == true)
            {
                Thread.Sleep(2000);
                if (isChangeLoaderRequest(true) == retv)
                {
                    // 2秒後、再確認してもマガジン無し(供給ON)の場合は正常
                }
                else
                {
                    // 2秒後、再確認してマガジン有り(供給OFF)の場合は誤検知
                    OutputSysLog("供給信号の誤検知発生");
                    retv = false;
                }
            }
            else
            {
                // マガジンを置いた時は信号の記憶だけ
                isChangeLoaderRequest(true);
            }

            return retv;
        }

        public void LoaderConveyorStop(bool status)
        {
            if (string.IsNullOrWhiteSpace(this.LoaderConveyorStopAddress))
            {
                return;
            }

            IPLC plc = this.Plc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }

            plc.SetBit(this.LoaderConveyorStopAddress, 1, Convert.ToInt32(status).ToString());
        }

        public void LoaderConveyorStop(bool status, bool isEmptyMagazine)
        {
            LoaderConveyorStop(status, isEmptyMagazine, 0);
            return;
        }

        public void LoaderConveyorStop(bool status, bool isEmptyMagazine, int retryCt)
        {
            string address = string.Empty;
            if (isEmptyMagazine)
            {
                if (string.IsNullOrWhiteSpace(this.EmpMagLoaderDoBitAddress))
                {
                    return;
                }
                address = this.EmpMagLoaderDoBitAddress;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(this.LoaderConveyorStopAddress))
                {
                    return;
                }
                address = this.LoaderConveyorStopAddress;
            }

            IPLC plc = this.Plc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }

            if (retryCt >= 3)
            {
                // アクセスファイルの作成に失敗した場合、ファイル未作成のまま信号だけON
                plc.SetBit(address, 1, Convert.ToInt32(status).ToString());
                return;
            }

            if (Directory.Exists(Config.Settings.MachineAccessDirectoryPath) == false)
            {
                Directory.CreateDirectory(Config.Settings.MachineAccessDirectoryPath);
            }

            string filePath = Path.Combine(Config.Settings.MachineAccessDirectoryPath, $"{this.PlantCd}.txt");
            if (status == true)
            {
                // 停止
                try
                {
                    // 装置停止指示後に装置が異常停止した場合、停止指示が残ったままになる。
                    // アクセス中ファイルを作成し、異常停止後のシステム起動時にファイルが存在すれば要求をOFFにする
                    if (File.Exists(filePath) == false)
                    {
                        File.Create(filePath);
                    }
                }
                catch (Exception)
                {
                    // ファイル作成失敗時は3回リトライする
                    Thread.Sleep(500);
                    retryCt = retryCt + 1;
                    LoaderConveyorStop(status, isEmptyMagazine, retryCt);
                    return;
                }
            }
            else
            {
                // 解除
                try
                {
                    if (File.Exists(filePath) == true)
                    {
                        File.Delete(filePath);
                    }
                }
                catch (Exception)
                {
                    // 削除に失敗してもエラーで止まらないように(次の機会に削除でOK)
                }
            }

            plc.SetBit(address, 1, Convert.ToInt32(status).ToString());
        }



        public virtual bool IsUnloaderMagazineExists()
        {
            if (string.IsNullOrWhiteSpace(this.IsUnloaderMagazineExistsAddress))
            {
                return false;
            }

            if (this.Plc.GetBit(this.IsUnloaderMagazineExistsAddress) == PLC.Common.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        /// <summary>
        /// マガジンリリース動作前の処理
        /// </summary>
        /// <returns></returns>
        public virtual bool ReleasingOperationBefore()
        {
            return true;
        }


        /// <summary>
        /// 基板作業開始
        /// </summary>
        public void SubstrateWorkStart()
        {
            List<MachineLog.TriggerFile> trgList = MachineLog.TriggerFile.GetAllFiles(this.LogInputDirectoryPath);
            if (trgList.Count == 0)
            {
                return;
            }

            foreach (MachineLog.TriggerFile trg in trgList)
            {
                OutputSysLog(string.Format("[基板開始処理] 開始 trgファイル取得成功 FileName:{0}", trg.FullName));

                AsmLot lot = AsmLot.GetAsmLot(trg.NascaLotNo);
                OutputSysLog(string.Format("[基板開始処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = ArmsApi.Model.MachineInfo.GetProcNo(this.MacNo);
                OutputSysLog(string.Format("[基板開始処理] 工程取得成功 ProcNo:{0}", procno));

                //trgファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(trg.FullName, lot.NascaLotNo, lot.TypeCd, procno, trg.MagNo, trg.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", trg.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, trg.MagNo);
                OutputSysLog(string.Format("[基板開始処理] ファイル名変更 from:{0} to:{1}", trg.FullName, fileName));

                OutputSysLog(string.Format("[基板開始処理] 完了"));
            }
        }

        /// <summary>
        /// 基板作業完了
        /// </summary>
        public void SubstrateWorkComplete()
        {
            List<MachineLog.FinishedFile6> finList = MachineLog.FinishedFile6.GetAllFiles(this.LogOutputDirectoryPath);
            if (finList.Count == 0)
            {
                return;
            }

            foreach (MachineLog.FinishedFile6 fin in finList)
            {
                OutputSysLog(string.Format("[基板完了処理] 開始 Fin2ファイル取得成功 FileName:{0}", fin.FullName));

                AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
                OutputSysLog(string.Format("[基板完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = ArmsApi.Model.MachineInfo.GetProcNo(this.MacNo);
                OutputSysLog(string.Format("[基板完了処理] 工程取得成功 ProcNo:{0}", procno));

                //fin2ファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(fin.FullName, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);
                OutputSysLog(string.Format("[基板完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName, fileName));

                OutputSysLog(string.Format("[基板完了処理] 完了"));
            }
        }

        /// <summary>
        /// マガジン作業完了
        /// </summary>
        public void MagazineWorkComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }
            VirtualMag oldmag = this.Peek(Station.Loader);
            if (oldmag == null)
            {
                return;
            }

            List<MachineLog.FinishedFile7> finList = MachineLog.FinishedFile7.GetAllFiles(this.LogOutputDirectoryPath, oldmag.MagazineNo);
            if (finList.Count == 0)
            {
                return;
            }
            if (finList.Count >= 2)
            {
                throw new ApplicationException("finファイルが複数存在するため処理できません。");
            }

            foreach (MachineLog.FinishedFile7 fin in finList)
            {
                if (fin.WorkStartDt == null || fin.WorkEndDt == null)
                {
                    throw new ApplicationException(string.Format("finファイル内から開始/完了時間が取得できませんでした。ファイル名：{0}", fin.FullName_Fin));
                }

                VirtualMag newMagazine = this.Peek(Station.Loader);
                newMagazine.WorkStart = fin.WorkStartDt;
                newMagazine.WorkComplete = fin.WorkEndDt;

                OutputSysLog(string.Format("[完了処理] 開始 Finファイル取得成功 FileName:{0}", fin.FullName_Fin));

                //AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
                //OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = ArmsApi.Model.MachineInfo.GetProcNo(this.MacNo);
                OutputSysLog(string.Format("[完了処理] 工程取得成功 ProcNo:{0}", procno));

                //fin及びwedファイル名をリネーム
                //ロット番号_タイプ_procno_マガジンNO.拡張子
                string fileName = string.Format("{0}_{1}_{2}_{3}", fin.NascaLotNo, fin.TypeCd, procno, fin.MagNo);

                //List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, fin.WorkStartDt.Value, fin.WorkEndDt.Value);
                //if (lotFiles.Count == 0)
                //    throw new ApplicationException(string.Format("排出信号を検知しましたが、開始～完了の間に出力された傾向管理ファイルが存在しません。 開始:{0} 完了:{1}", fin.WorkStartDt.Value, fin.WorkEndDt.Value));
                //foreach (string lotFile in lotFiles)
                //{
                //    string fileExtension = Path.GetExtension(lotFile).ToLower();
                //    if (fileExtension != ".mpd")
                //    {
                //        MachineLog.ChangeFileName(lotFile, fin.NascaLotNo, fin.TypeCd, procno, fin.MagNo);
                //        OutputSysLog(string.Format("[完了処理] ファイル名変更 from:{0} to:{1}", lotFile, fileName));
                //    }
                //}

                MachineLog.ChangeFileName(fin.FullName_Wed, fin.NascaLotNo, fin.TypeCd, procno, fin.MagNo);
                OutputSysLog(string.Format("[完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName_Wed, fileName));
                MachineLog.ChangeFileName(fin.FullName_Fin, fin.NascaLotNo, fin.TypeCd, procno, fin.MagNo);
                OutputSysLog(string.Format("[完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName_Fin, fileName));

                //仮想マガジンをunloaderに移動
                this.Enqueue(newMagazine, Station.Unloader);

                OutputSysLog(string.Format("[完了処理] 完了"));
            }
        }

        /// <summary>
        /// 対象ファイルがARMSが既にリネーム済みかどうかをチェックする関数(_区切りで5要素以上あればリネーム済み)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool IsRenamedFile(string filePath)
        {
            //名称変更済かで確認(ロット + タイプ + 工程NO付与)
            string[] nameChar = Path.GetFileNameWithoutExtension(filePath).Split('_');
            if (nameChar.Count() < 5)
            {
                return false;
            }
            else { return true; }
        }

        #region ロボットPLC ⇒ 装置PLC 信号送信
        public void SetBitMachineReady(string data)
        {
            if (string.IsNullOrWhiteSpace(this.SendMachineReadyBitAddress) == true)
                return;

            IPLC plc = this.Plc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }
            plc.SetBit(this.SendMachineReadyBitAddress, 1, data);
        }

        public void SetBitDoingOpenRobotArm(string data)
        {
            if (string.IsNullOrWhiteSpace(this.DoingOpenRobotArmBitAddress) == true)
                return;

            IPLC plc = this.Plc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }
            plc.SetBit(this.DoingOpenRobotArmBitAddress, 1, data);
        }

        public void SetBitDoingCloseRobotArm(string data)
        {
            if (string.IsNullOrWhiteSpace(this.DoingCloseRobotArmBitAddress) == true)
                return;

            IPLC plc = this.Plc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }
            plc.SetBit(this.DoingCloseRobotArmBitAddress, 1, data);
        }

        #region SetAddressDoingLoadMagazine
        //public virtual void SetAddressDoingLoadMagazine(Station st, bool loader, string data)
        public virtual void SetAddressDoingLoadMagazine(Station st, string buffercode, bool loader, string data)
        {
            string loaderDoBitAddress;
            List<string> otherDoBitAddressList = new List<string>();
            switch (st)
            {
                case Station.Loader:
                case Station.Loader1:
                case Station.Loader2:
                case Station.Loader3:
                case Station.Loader4:
                    loaderDoBitAddress = this.LoaderDoBitAddress;
                    otherDoBitAddressList.Add(this.UnloaderDoBitAddress);
                    otherDoBitAddressList.Add(this.EmpMagLoaderDoBitAddress);
                    otherDoBitAddressList.Add(this.EmpMagUnloaderDoBitAddress);
                    break;

                case Station.Unloader:
                case Station.Unloader1:
                case Station.Unloader2:
                case Station.Unloader3:
                case Station.Unloader4:
                case Station.Unloader5:
                case Station.Unloader6:
                case Station.Unloader7:
                    loaderDoBitAddress = this.UnloaderDoBitAddress;
                    otherDoBitAddressList.Add(this.LoaderDoBitAddress);
                    otherDoBitAddressList.Add(this.EmpMagLoaderDoBitAddress);
                    otherDoBitAddressList.Add(this.EmpMagUnloaderDoBitAddress);
                    break;

                case Station.EmptyMagazineLoader:
                case Station.EmptyMagazineLoader1:
                case Station.EmptyMagazineLoader2:
                case Station.EmptyMagazineLoader3:
                case Station.EmptyMagazineLoader4:
                    loaderDoBitAddress = this.EmpMagLoaderDoBitAddress;
                    otherDoBitAddressList.Add(this.LoaderDoBitAddress);
                    otherDoBitAddressList.Add(this.UnloaderDoBitAddress);
                    otherDoBitAddressList.Add(this.EmpMagUnloaderDoBitAddress);
                    break;

                case Station.EmptyMagazineUnloader:
                case Station.EmptyMagazineUnloader1:
                case Station.EmptyMagazineUnloader2:
                case Station.EmptyMagazineUnloader3:
                case Station.EmptyMagazineUnloader4:
                    loaderDoBitAddress = this.EmpMagUnloaderDoBitAddress;
                    otherDoBitAddressList.Add(this.LoaderDoBitAddress);
                    otherDoBitAddressList.Add(this.UnloaderDoBitAddress);
                    otherDoBitAddressList.Add(this.EmpMagLoaderDoBitAddress);
                    break;

                default:
                    throw new ApplicationException($"定義外のStationのSetAddressDoingLoadMagazine：{st.ToString()}");
            }

            if (string.IsNullOrWhiteSpace(loaderDoBitAddress) == false)
            {
                IPLC plc = this.Plc;
                if (this.ConveyorPlc != null)
                {
                    plc = this.ConveyorPlc;
                }
                Plc.SetBit(loaderDoBitAddress, 1, data);
            }

            // 他のdoBitAddressをBIT_OFFにする
            foreach (string doBitAddress in otherDoBitAddressList)
            {
                if (string.IsNullOrWhiteSpace(doBitAddress) == true) continue;

                IPLC plc = this.Plc;
                if (this.ConveyorPlc != null)
                {
                    plc = this.ConveyorPlc;
                }
                Plc.SetBit(doBitAddress, 1, PLC.Common.BIT_OFF);
            }
        }
        #endregion

        #region SetAddressSendMagazineNo
        //public virtual void SetAddressSendMagazineNo(Station st, string magNo)
        public void SetAddressSendMagazineNo(Station st, string magNo)
        {
            string sendMagazineAddress;
            switch (st)
            {
                case Station.Loader:
                case Station.Loader1:
                case Station.Loader2:
                case Station.Loader3:
                case Station.Loader4:
                case Station.EmptyMagazineUnloader:
                case Station.EmptyMagazineUnloader1:
                case Station.EmptyMagazineUnloader2:
                case Station.EmptyMagazineUnloader3:
                case Station.EmptyMagazineUnloader4:
                    sendMagazineAddress = this.SendLMagazineAddress;
                    break;

                case Station.Unloader:
                case Station.Unloader1:
                case Station.Unloader2:
                case Station.Unloader3:
                case Station.Unloader4:
                case Station.Unloader5:
                case Station.Unloader6:
                case Station.Unloader7:
                case Station.EmptyMagazineLoader:
                case Station.EmptyMagazineLoader1:
                case Station.EmptyMagazineLoader2:
                case Station.EmptyMagazineLoader3:
                case Station.EmptyMagazineLoader4:
                    sendMagazineAddress = this.SendULMagazineAddress;
                    break;

                default:
                    throw new ApplicationException($"定義外のStationのSetAddressSendMagazineNo：{st.ToString()}");
            }

            if (string.IsNullOrWhiteSpace(sendMagazineAddress) == false)
            {
                IPLC plc = this.Plc;
                if (this.ConveyorPlc != null)
                {
                    plc = this.ConveyorPlc;
                }
                Plc.SetString(sendMagazineAddress, magNo);
            }
        }
        #endregion

        public void SetBitDoorCanOpen(string data)
        {
            if (string.IsNullOrWhiteSpace(this.SendDoorCanOpenAddress) == true)
                return;

            IPLC plc = this.Plc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }
            plc.SetBit(this.SendDoorCanOpenAddress, 1, data);
        }

        #endregion

        #region GetRequireBitData
        public virtual string GetRequireBitData(Station st, string buffercode)
        {
            switch (st)
            {
                case Station.Loader:
                    return Plc.GetBit(this.LoaderReqBitAddress);

                case Station.EmptyMagazineLoader:
                    return Plc.GetBit(this.EmpMagLoaderReqBitAddress);

                case Station.Unloader:
                    return Plc.GetBit(this.UnLoaderReqBitAddress);

                case Station.EmptyMagazineUnloader:
                    return Plc.GetBit(this.EmpMagUnLoaderReqBitAddress);

                default:
                    throw new ApplicationException($"定義外のStationのGetRequireBitData：{st.ToString()}");
            }
        }
        public string GetRequireConveyorBitData(Station st, IPLC carrierPlc)
        {
            string retv = PLC.Common.BIT_OFF;

            IPLC plc = carrierPlc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }

            switch (st)
            {
                case Station.Loader:
                    foreach (string address in this.LoaderConveyorReqBitAddressList)
                    {
                        retv = plc.GetBit(address);
                        if (retv == PLC.Common.BIT_ON)
                        {
                            break;
                        }
                    }
                    return retv;

                case Station.Unloader:
                    foreach (string address in this.UnloaderConveyorReqBitAddressList)
                    {
                        retv = plc.GetBit(address);
                        if (retv == PLC.Common.BIT_ON)
                        {
                            break;
                        }
                    }
                    return retv;

                default:
                    throw new ApplicationException($"定義外のStationのGetRequireConveyorBitData：{st.ToString()}");
            }
        }
        #endregion

        #region GetMagazineDestinationCompltBitData
        /// <summary>
        /// マガジンの受け渡しが完了しているか
        /// </summary>
        /// <returns></returns>
        public string GetMagazineDestinationCompltBitData()
        {
            //扉開信号の確認
            if (string.IsNullOrWhiteSpace(this.MagazineDestinationCompltBitAddress) == true)
            {
                return Mitsubishi.BIT_OFF;
            }

            IPLC plc = this.Plc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }
            return plc.GetBit(this.MagazineDestinationCompltBitAddress);
        }
        #endregion

        /// <summary>
        /// 【ライン外装置の作業開始処理用】
        /// 作業順マスタと現在工程の関係から、次作業をダミー登録できるかチェック
        /// 登録可の場合、この関数内でダミー登録も実施。
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="mag"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        protected bool dummyTranCheckAndInsert(AsmLot lot, Magazine mag, ref Process p)
        {
            // チェック
            int dummyprocno = p.ProcNo;
            if (WorkChecker.IsNotRelatedMachineAndProcess(this.MacNo, p.ProcNo) == false)
            {
                return false;
            }

            // 作業番号を次に進める
            Process oldProc = p;
            p = Process.GetNextProcess(p.ProcNo, lot);
            if (p == null)
            {
                p = oldProc;
                return false;
            }

            // 進めた作業番号で装置と紐づいているか再度確認。
            if (WorkChecker.IsNotRelatedMachineAndProcess(this.MacNo, p.ProcNo) == true)
            {
                // 紐づいていなければ、そのまま通過 → 後の作業開始規制でエラー判定にする (装置と工程の紐づきエラー)
                p = oldProc;
                return false;
            }

            // ダミー作業実績登録の可・不可の判定
            if (WorkChecker.CanDummyAutoTran(lot, p.ProcNo) == false)
            {
                // 登録不可  そのまま通過 → 後の作業開始規制でエラー判定にする (前作業が完了していません)
                return false;
            }

            // 登録可の場合、このタイミングでダミー実績登録を実施
            Process dummyProc = Process.GetProcess(dummyprocno);
            Order.RecordDummyWork(lot, dummyProc.ProcNo, dummyProc.AutoUpdMachineNo.Value, mag);

            return true;
        }

        #region GetMagazineDestinationCompltBitData
        /// <summary>
        /// オーブンの扉が全て閉じているか
        /// </summary>
        /// <returns></returns>
        public string GetOvenAllDoorCloseBitData()
        {
            //扉開信号の確認
            if (string.IsNullOrWhiteSpace(this.OvenAllDoorCloseBitAddress) == true)
            {
                return Mitsubishi.BIT_OFF;
            }

            IPLC plc = this.Plc;
            if (this.ConveyorPlc != null)
            {
                plc = this.ConveyorPlc;
            }
            return plc.GetBit(this.OvenAllDoorCloseBitAddress);
        }
        #endregion


        /// <summary>
        /// 開始時刻を抽出する
        /// </summary>
        /// <returns></returns>
        protected virtual DateTime GetWorkStartTime()
        {
            if (string.IsNullOrWhiteSpace(this.WorkStartTimeAddress) == false)
            {
                return this.Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
            }
            else if (this.WorkStartTimeAddressList != null)
            {
                int year = this.Plc.GetWordAsDecimalData(this.WorkStartTimeAddressList[0]);
                int month = this.Plc.GetWordAsDecimalData(this.WorkStartTimeAddressList[1]);
                int day = this.Plc.GetWordAsDecimalData(this.WorkStartTimeAddressList[2]);
                int hour = this.Plc.GetWordAsDecimalData(this.WorkStartTimeAddressList[3]);
                int minute = this.Plc.GetWordAsDecimalData(this.WorkStartTimeAddressList[4]);
                int sec = this.Plc.GetWordAsDecimalData(this.WorkStartTimeAddressList[5]);

                return new DateTime(year, month, day, hour, minute, sec);
            }
            else
            {
                throw new Exception("LineConfig.xmlに『WorkStartTimeAddress』or『WorkStartTimeAddressList』が設定されていません。");
            }
        }

        /// <summary>
        /// 開始時刻を抽出する
        /// </summary>
        /// <returns></returns>
        protected virtual DateTime? GetWorkCompleteTime()
        {
            if (string.IsNullOrWhiteSpace(this.WorkCompleteTimeAddress) == false)
            {
                return this.Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
            }
            else if (this.WorkCompleteTimeAddressList != null)
            {
                int year = this.Plc.GetWordAsDecimalData(this.WorkCompleteTimeAddressList[0]);
                int month = this.Plc.GetWordAsDecimalData(this.WorkCompleteTimeAddressList[1]);
                int day = this.Plc.GetWordAsDecimalData(this.WorkCompleteTimeAddressList[2]);
                int hour = this.Plc.GetWordAsDecimalData(this.WorkCompleteTimeAddressList[3]);
                int minute = this.Plc.GetWordAsDecimalData(this.WorkCompleteTimeAddressList[4]);
                int sec = this.Plc.GetWordAsDecimalData(this.WorkCompleteTimeAddressList[5]);

                return new DateTime(year, month, day, hour, minute, sec);
            }
            else
            {
                throw new Exception("LineConfig.xmlに『WorkCompleteTimeAddress』or『WorkCompleteTimeAddressList』が設定されていません。");
            }
        }

        /// <summary>
        /// 供給・排出許可信号を取得
        /// </summary>
        /// <returns></returns>
        public string GetCanBitAddress(Station st)
        {
            switch (st)
            {
                case Station.Loader:
                case Station.Loader1:
                case Station.Loader2:
                case Station.Loader3:
                case Station.Loader4:
                    return this.LoaderCanBitAddress;

                case Station.Unloader:
                case Station.Unloader1:
                case Station.Unloader2:
                case Station.Unloader3:
                case Station.Unloader4:
                case Station.Unloader5:
                case Station.Unloader6:
                case Station.Unloader7:
                    return this.UnloaderCanBitAddress;

                case Station.EmptyMagazineLoader:
                case Station.EmptyMagazineLoader1:
                case Station.EmptyMagazineLoader2:
                case Station.EmptyMagazineLoader3:
                case Station.EmptyMagazineLoader4:
                    return this.EmpMagLoaderCanBitAddress;

                case Station.EmptyMagazineUnloader:
                case Station.EmptyMagazineUnloader1:
                case Station.EmptyMagazineUnloader2:
                case Station.EmptyMagazineUnloader3:
                case Station.EmptyMagazineUnloader4:
                    return this.EmpMagUnloaderCanBitAddress;

                default:
                    throw new ApplicationException($"定義外のStationのSetAddressDoingLoadMagazine：{st.ToString()}");
            }
        }



        /// <summary>
        /// 接続要求
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRequireConnection()
        {
            if (string.IsNullOrEmpty(this.ConnectionReqBitAddress) == true)
            {
                return false;
            }

            string retv;
            try
            {
                retv = Plc.GetBit(this.ConnectionReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、接続要求OFF扱い。アドレス：『{this.ConnectionReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv == Keyence.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 接続要求への応答
        /// </summary>
        public virtual void ConnectionProcess()
        {
            this.OutputApiLog($"接続要求への応答処理開始");

            if (string.IsNullOrEmpty(this.ConnectionReqBitAddress) == true ||
                string.IsNullOrEmpty(this.InConnectionBitAddress) == true)
            {
                return;
            }

            //接続要求信号をOFF
            try
            {
                this.OutputApiLog($"接続要求信号をOFF");
                Plc.SetBit(this.ConnectionReqBitAddress, 1, Keyence.BIT_OFF);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"接続要求への応答失敗。アドレス：『{this.ConnectionReqBitAddress}』, エラー内容：{ex.Message}");
            }

            //接続中信号をON
            try
            {
                this.OutputApiLog($"接続中信号をON");
                Plc.SetBit(this.InConnectionBitAddress, 1, Keyence.BIT_ON);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"接続要求への応答失敗。アドレス：『{this.InConnectionBitAddress}』, エラー内容：{ex.Message}");
            }

            this.OutputApiLog($"接続要求への応答処理完了");
        }

        /// <summary>
        /// 切断要求
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRequireDisconnect()
        {
            if (string.IsNullOrEmpty(this.DisconnectionReqBitAddress) == true)
            {
                return false;
            }

            string retv;
            try
            {
                retv = Plc.GetBit(this.DisconnectionReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、切断要求OFF扱い。アドレス：『{this.DisconnectionReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv == Keyence.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 切断要求への応答
        /// </summary>
        public virtual void DisconnectionProcess()
        {
            this.OutputApiLog($"切断要求への応答処理開始");

            if (string.IsNullOrEmpty(this.DisconnectionReqBitAddress) == true ||
                string.IsNullOrEmpty(this.InConnectionBitAddress) == true)
            {
                return;
            }

            //切断要求信号をOFF
            try
            {
                this.OutputApiLog($"切断要求信号をOFF");
                Plc.SetBit(this.DisconnectionReqBitAddress, 1, Keyence.BIT_OFF);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"切断要求への応答失敗。アドレス：『{this.ConnectionReqBitAddress}』, エラー内容：{ex.Message}");
            }

            //接続中信号をOFF
            try
            {
                this.OutputApiLog($"接続中信号をOFF");
                Plc.SetBit(this.InConnectionBitAddress, 1, Keyence.BIT_OFF);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"切断要求への応答失敗。アドレス：『{this.InConnectionBitAddress}』, エラー内容：{ex.Message}");
            }

            this.OutputApiLog($"切断要求への応答処理完了");
        }

        /// <summary>
        /// 出力要求
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRequireReadDischargePressureData()
        {
            if (string.IsNullOrEmpty(this.ReadDischargePressureDataReqBitAddress) == true)
            {
                return false;
            }

            string retv;
            try
            {
                retv = Plc.GetBit(this.ReadDischargePressureDataReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、出力要求OFF扱い。アドレス：『{this.ReadDischargePressureDataReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv == Keyence.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
