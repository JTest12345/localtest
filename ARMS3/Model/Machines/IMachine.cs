using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 装置のマガジン流動形態
    /// </summary>
    public enum MachineType
    {
        RobotQRReader,
        WireBonder,
        DieBonder,
        FrameLoader,
        Mold,
        Mold2,
		Mold3,
        Cut,
        Oven,
        DischargeConveyor,
        LoadConveyor,
        EmptyMagazineLoadConveyor,
        HybridLoadConveyor,
        LoadConveyorMagRotation,
        MoldConveyor,
        Inspector,
        Inspector2,
        MAP1stDieBonder,
        MAPBoardMagazineLoadConveyor,
        MAPBoardDischargeConveyor,
        ECK,
        ECK2,
		ECK3,
        ECK4,
        ECK4MagExchanger,
        ManualECK,
        Plasma,
        MAPPreDBBakingOven,
        MultiLoadPlasma,
        LineBridge,
        LineBridge2,
        MagExchanger,
        GeneralBuffer,
        AutoLineOutConveyor,
        ManualOven,
        ManualOvenWithProgramWait,
        ManualMultiLoadPlasma,
        InspectorMagRotation,
        PlasmaMagRotation,
        LotMarking,
        OpeningCheckConveyor,
        MAPCompltDischargeConveyor,
		Sputter,
		DieBonder2,
		FrameLoader2,
		FlipChip,
		FullAutoCompacting,
		SemiAutoCompacting,
		ElectroCoat,
		BumpBonder,
		Mold4,
		WeightMeasurement,
		BackSideWashing,
		LaserScribe,
		ColorRevision,
		Break,
        FrameLoader3,
        FrameLoader3ULBuffer,
        FrameLoader4,
        FrameLoader5,
        GeneralLotMarking,
		DieBonder3,
		DieBonder4,
		DieBonder5,
		ReflectorPotting,
		ReflectorPotting2,
		Dicing,
		Mold5,
		LotMarking2,
		LotMarking3,
		Mold6,
		Inspector3,
        SMTLoader,
        SMTUnloader,
        Inspector4,
        DieBonder6,
        Mounter,
        SQMachine,
        Masking,
        SubstrateLoader,
        LensMounting,
        Inspector5,
        Dicing2,
        Grinder,
        Grinder2,
        ManualComplete,
        TapePeeler,
        DieBonder7,
        WireBonder2,
		YagGlassBonder,
		Mold7,
		HotPress,
		WetBlaster,
		Formatting,
        IPS,
        IPS2,
        Sputter2,
        LotMarking4,
        FormicAcidReflow,
        FormicAcidReflow2,
        Deburring,
        DryIceCleaning,
        ColorAutoMeasurer,
        AutoPaster,
        LineBridgeBuffer,
        WetBlaster2,
        Inspector6,
        SheetRingLoader,
        WaterCleaning,
        ResinMeasurement,
        //appendbujuni
        CejlLotMarkingPrototype,
    }

	///// <summary>
	///// ライン形態
	///// </summary>
	//public enum LineGroup 
	//{
	//	/// <summary>自動化</summary>
	//	Auto,
	//	/// <summary>高効率</summary>
	//	Manual,
	//}

    /// <summary>
    /// キャリア形態
    /// </summary>
    public enum CarrierType 
    {
        /// <summary>搬送ロボット</summary>
        Robot,
        /// <summary>ライン連結橋</summary>
        Bridge,
        /// <summary>搬送ロボット(三菱スカラ)</summary>
        Robot2,
        /// <summary>搬送ロボット(6軸)  PLC:Mitsubishi  ※Jobショップ用(搬送先の各装置が個別PLC)</summary>
        Robot3,
        /// <summary>搬送ロボット(6軸)  PLC:Mitsubishi  ※NTSV用(搬送先の各装置が個別PLC + QR読取位置バッファ数  2)</summary>
        Robot4,
        /// <summary>搬送ロボット(6軸)  PLC:Mitsubishi  ※Jobショップ用(搬送先の各装置が個別PLC) 同一リニア内の2台目</summary>
        Robot5,
    }

    public interface IMachine
    {
        IPLC Plc { get; set; }

        IPLC CarrierPlc { get; set; }

        /// <summary>
        /// PLC応答不能
        /// </summary>
        bool IsAvailable { get; set; }

        /// <summary>
        /// 最終の応答不能時間
        /// </summary>
        DateTime LastDisAvailableTime { get; set; }

        /// <summary>
        /// スレッド完了
        /// </summary>
        event EventHandler<MachineThreadEventArgs> OnStopWork;

        /// <summary>
        /// 装置種類識別
        /// </summary>
        MachineType MachineType { get; set; }

		///// <summary>
		///// ライン種類(自動化=1、高効率=2)
		///// </summary>
		//int LineGroupNo { get; set; }

		bool IsAutoLine { get; set; }
		bool IsHighLine { get; set; }
		bool IsOutLine { get; set; }
        bool IsAgvLine { get; set; }

        /// <summary>
        /// ライン番号
        /// </summary>
        string LineNo { get; set; }

        /// <summary>
        /// ARMS設備No
        /// </summary>
        int MacNo { get; set; }

        /// <summary>
        /// 装置グループ
        /// </summary>
        List<string> MacGroup { get; set; }

        /// <summary>
        /// NASCA設備コード
        /// </summary>
        string PlantCd { get; set; }

        /// <summary>
        /// 装置日本語名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 優先度
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// スレッド処理通常停止要求
        /// </summary>
        bool StopRequested { get; set; }

        /// <summary>
        /// スレッド処理動作中フラグ
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// 最終のマガジン投入
        /// </summary>
        DateTime LastMagazineSetTime { get; set; }

        /// <summary>
        /// 最終のマガジン排出
        /// </summary>
        DateTime LastOutputTime { get; set; }

        string MacPoint { get; set; }

        // 【N工場MAP J9・10不具合 修正】
        string ClearMagazineBitAddress { get; set; }

        /// <summary>
        /// 信号確認画面のタブ名
        /// </summary>
        string SignalDisplayFlowName { get; set; }

        /// <summary>
        /// スレッド動作状態取得
        /// </summary>
        /// <returns></returns>
        WorkStatus GetWorkStatus();

        /// <summary>
        /// スレッド処理開始
        /// 開始済みの場合は無視
        /// </summary>
        /// <returns></returns>
        void RunWork();

        /// <summary>
        /// 動作中スレッドの強制終了
        /// </summary>
        void AbortThread();

        /// <summary>
        /// 投入要求確認
        /// </summary>
        /// <returns></returns>
        bool IsRequireInput();

        /// <summary>
        /// 投入要求確認(装置隣接CV、棚)
        /// </summary>
        /// <returns></returns>
        bool IsRequireConveyorInput(IPLC carrierPlc);

        /// <summary>
        /// 排出要求確認
        /// </summary>
        /// <returns></returns>
        bool IsRequireOutput();

        /// <summary>
        /// 空マガジン要求
        /// </summary>
        bool IsRequireEmptyMagazine();

        /// <summary>
        /// Ready信号
        /// </summary>
        /// <returns></returns>
        bool IsReady();

        /// <summary>
        /// 空マガジン排出要求
        /// </summary>
        /// <returns></returns>
        bool IsRequireOutputEmptyMagazine();

        bool CanInput(VirtualMag mag);

        string GetMacPoint(Station station);

        string GetFromToCode(Station station);

        string GetFromBufferCode();

        string GetFromToBufferCode(Station station);

        string GetToBufferCode();

        VirtualMag Peek(Station st);

        bool Enqueue(VirtualMag mag, Station station);

        VirtualMag Dequeue(Station station);

        Location GetLoaderLocation();

        Location GetUnLoaderLocation();

        List<VirtualMag> GetMagazines(Station st);

        void ReloadPriority();

        bool ResponseEmptyMagazineRequest();

        bool IsDischargeMode(VirtualMag mag);

        bool IsPreInputDischargeMode(VirtualMag mag);

        void ResponseClearMagazineRequest();

        void ClearVirtualMagazines();

		bool IsInputForbidden();

        bool IsRequireMagazineOutput();

        void LoaderConveyorStop(bool status);

        void LoaderConveyorStop(bool status, bool isEmptyMagazine);

        bool IsUnloaderMagazineExists();

        //2016.09.29 フォルダの持ち方変更のため廃止
        //string WatchingDirectoryPath { get; set; }

        /// <summary>
        /// 傾向管理・マッピングインプットフォルダ
        /// </summary>
        string LogInputDirectoryPath { get; set; }

        /// <summary>
        /// 傾向管理・マッピングアウトプットフォルダ
        /// </summary>
        string LogOutputDirectoryPath { get; set; }

        /// <summary>
        /// マッピングインプットフォルダ【※傾向管理とマッピングの出力先が異なる場合のみ使用】
        /// </summary>
        string AltMapInputDirectoryPath { get; set; }

        /// <summary>
        /// マッピングアウトプットフォルダ【※傾向管理とマッピングの出力先が異なる場合のみ使用】
        /// </summary>
        string AltMapOutputDirectoryPath { get; set; }

        bool IsSubstrateComplete { get; set; }

        /// <summary>
        /// 装置隣接CV(棚)の存在
        /// </summary>
        bool IsConveyor { get; set; }

        string LinearNo { get; set; }

        void SetBitMachineReady(string data);

        void SetBitDoingOpenRobotArm(string data);

        void SetBitDoingCloseRobotArm(string data);

        void SetBitDoorCanOpen(string data);

        void SetAddressDoingLoadMagazine(Station st, string buffercode, bool loader, string data);

        void SetAddressSendMagazineNo(Station st, string magNo);

        string GetRequireBitData(Station st, string buffercode);

        string GetRequireConveyorBitData(Station st, IPLC carrierPlc);

        string GetMagazineDestinationCompltBitData();

        string GetOvenAllDoorCloseBitData();

        string GetCanBitAddress(Station st);
    }
}
