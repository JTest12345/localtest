using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ArmsWorkTransparency.Common;
using System.ComponentModel;
using ArmsApi.Model;
using System.Collections.Specialized;
using ArmsWorkTransparency.Views;
using ArmsWorkTransparency.Model;
using System.Xml.Linq;
using ArmsWorkTransparency.Model.PLC;
using System.Windows.Forms;
using System.Data.SqlClient;
using ArmsApi;
using System.Data;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace ArmsWorkTransparency.ViewModels
{
    public class MachineMonitorViewModel : INotifyPropertyChanged
    {

        private System.Media.SoundPlayer sp = new System.Media.SoundPlayer("alert.wav");
        
        private List<LineConfig> lineConfigList = new List<LineConfig>();
                  
        public MachineMonitorViewModel()
        {
            try
            {
                foreach (string path in Properties.Settings.Default.LineConfigPathList)
                {
                    LineConfig lineConfig = LineConfig.LoadLineConfig(path);
                    lineConfigList.Add(lineConfig);
                }

                foreach(string zone in Properties.Settings.Default.UnattendedTimeZoneList)
                {
                    string[] zoneElement = zone.Split(',');
                    UnattendedTimeZone z = new UnattendedTimeZone();
                    z.StartDt = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy/MM/dd ") + zoneElement[0]);
                    z.EndDt = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy/MM/dd ") + zoneElement[1]);
                    unattendedTimeZoneList.Add(z);
                }

                foreach (string no in Properties.Settings.Default.UseHitCtQcParamNoList)
                {
                    string[] noElement = no.Split(',');
                    UseHitCtQcparamNo q = new UseHitCtQcparamNo();
                    q.ProcNo = Convert.ToInt32(noElement[0]);
                    if (noElement.Count() == 3)
                    {
                        q.StampCd = noElement[1];
                        q.QcParamNo = Convert.ToInt16(noElement[2]);
                    }
                    else
                    {
                        q.QcParamNo = Convert.ToInt32(noElement[1]);
                    }
                    useHitCtQcparamNoList.Add(q);
                }

                List<Machine> mList = new List<Machine>();
                IEnumerable<MachineInfo> machineList = MachineInfo.GetMachineList(true).Where(m => m.NascaPlantCd != "-");
                if (Properties.Settings.Default.TargetMachineList != null)
                {
                    // 設定で指定した号機のみに絞る
                    machineList = machineList.Where(m => Properties.Settings.Default.TargetMachineList.Contains(m.MacNo.ToString()));
                }

                foreach (MachineInfo machine in machineList)
                {
                    Machine m = new Machine();
                    m.MacNo = machine.MacNo;
                    m.PlantCd = machine.NascaPlantCd;
                    m.LoaderMagazineMaxCt = machine.LoaderMagazineMaxCt;
                    LineConfig.MachineSetting setting = LineConfig.GetMachineSetting(this.lineConfigList, machine.MacNo);
                    m.Plc = setting.Plc;
                    //m.RobotPlc = setting.RobotPlc;
                    m.RequreInputAddressList = setting.RequreInputAddressList;
                    m.InputForbiddenBitAddress = setting.InputForbiddenBitAddress;

                    mList.Add(m);
                }

                MachineList = mList;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private string lastUpdate;
        public string LastUpdate
        {
            get { return lastUpdate; }
            set
            {
                this.lastUpdate = value;
                NotifyPropertyChanged(nameof(LastUpdate));
            }
        }

        private string nextUpdate;
        public string NextUpdate
        {
            get { return nextUpdate; }
            set
            {
                this.nextUpdate = value;
                NotifyPropertyChanged(nameof(NextUpdate));
            }
        }

        public class Machine
        {
            public string LineName { get; set; }

            public int MacNo { get; set; }

            public string PlantCd { get; set; }

            public int LoaderMagazineMaxCt { get; set; }

            public string MachineName { get; set; }

            public string ClassName { get; set; }

            public string TypeCode { get; set; }

            public string ChipName { get; set; }

            public string LoaderMagazineFreeCount { get; set; } = "-";

            public string RemainingTime { get; set; } = "-";

            public string SupplyId { get; set; }

            public string BlendCode { get; set; }

            public string ResinGroupCode { get; set; }

            public DateTime? MaterialLimitTime { get; set; }

            public string IPAddress { get; set; }

            public string Port { get; set; }

            public IPLC Plc { get; set; }

            public IPLC RobotPlc { get; set; }

            public List<string> RequreInputAddressList { get; set; }

            public string InputForbiddenBitAddress { get; set; }
            
            /// <summary>
            /// 警告　資材有効期限10分以内
            /// </summary>
            public bool IsWarning
            {
                get
                {
                    if (string.IsNullOrEmpty(LoaderMagazineFreeCount) == true)
                        return false;

                    if (LoaderMagazineFreeCount != "-" 
                        && int.Parse(LoaderMagazineFreeCount) >= Properties.Settings.Default.WarningMagazineFreeCt)
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
            /// 注意　資材有効期限60分前
            /// </summary>
            public bool IsCaution
            {
                get
                {
                    if (string.IsNullOrEmpty(LoaderMagazineFreeCount) == true)
                        return false;

                    if (LoaderMagazineFreeCount != "-" 
                        && int.Parse(LoaderMagazineFreeCount) >= Properties.Settings.Default.CautionMagazineFreeCt 
                        && int.Parse(LoaderMagazineFreeCount) < Properties.Settings.Default.WarningMagazineFreeCt)
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
            /// 供給信号
            /// </summary>
            public bool IsRequreInput
            {
                get
                {
                    try
                    {
#if DEBUG
                        return true;
#else
                        //if (this.RobotPlc.GetBit(this.InputForbiddenBitAddress) == Model.PLC.Common.BIT_ON)
                        //{
                        //    return false;
                        //}

                        //foreach (string address in this.RequreInputAddressList)
                        //{
                        //    if (this.Plc.GetBit(address) == Model.PLC.Common.BIT_ON)
                        //    {
                        //        return true;
                        //    }
                        //}
                        //return false;
                        return true;
#endif
                    }
                    catch(Exception)
                    {
                        return false;
                    }
                }
            }

            /// <summary>
            /// 運転信号(データベース記録値表示)
            /// </summary>
            public bool IsReady { get; set; } = true;

            /// <summary>
            /// 無人時間帯対象
            /// </summary>
            public bool IsUnattendedTimeZone { get; set; }          

            /// <summary>
            /// マガジン投入予測
            /// </summary>
            public string LoadMagazinePlanTime { get; set; } = "-";

            /// <summary>
            /// DB樹脂交換予測
            /// </summary>
            public string ResinChangePlanTime { get; set; } = "-";

            /// <summary>
            /// ﾁｯﾌﾟ残数量
            /// </summary>
            public string ChipCt { get; set; } = "-";

            /// <summary>
            /// ﾁｯﾌﾟ交換予測
            /// </summary>
            public string ChipChangePlanTime { get; set; } = "-";

            public string StampPinChangePlanTime { get; set; } = "-";

            public string LeftStampPinChangePlanTime { get; set; } = "-";

            public string RightStampPinChangePlanTime { get; set; } = "-";

            public string ColletChangePlanTime { get; set; } = "-";

            public string EjectPinChangePlanTime { get; set; } = "-";

            public string GoldWireChangePlanTime { get; set; } = "-";

            public string CapillaryChangePlanTime { get; set; } = "-";

            public string MachineLogOutputPath { get; set; } = "-";

            public string AimRank { get; set; }

            public string PhosphorSheetMatCd { get; set; }

            public string PhosphorSheetResinGroupCd { get; set; }
        }

        public class Eics
        {
            public string TypeGroupCode { get; set; }

            public string TypeCode { get; set; }

            public string ChipName { get; set; }

            public string ModelName { get; set; }

            public static Eics GetSetting(string equipmentNo)
            {
                Eics retv = null;

                using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" SELECT WorkingTypeGroup_CD, Chip_NM, WorkingType_CD, Model_NM
									FROM TmEQUI 
                                        INNER JOIN TmLSET ON TmEQUI.Equipment_NO = TmLSET.Equipment_NO
									WHERE TmEQUI.Del_FG = 0 AND TmLSET.Del_FG = 0 AND TmLSET.Equipment_NO = @Equipment_NO ";

                    cmd.Parameters.Add("@Equipment_NO", SqlDbType.NVarChar).Value = equipmentNo;

                    cmd.CommandText = sql;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            retv = new Eics();
                            retv.TypeCode = rd["WorkingType_CD"].ToString().Trim();
                            retv.TypeGroupCode = rd["WorkingTypeGroup_CD"].ToString().Trim();
                            retv.ChipName = rd["Chip_NM"].ToString().Trim();
                            retv.ModelName = rd["Model_NM"].ToString().Trim();
                        }
                    }
                }

                return retv;
            }
        }

        /// <summary>
        /// 無人時間帯
        /// </summary>
        private class UnattendedTimeZone
        {
            public DateTime StartDt { get; set; }
            public DateTime EndDt { get; set; }
        }
        private List<UnattendedTimeZone> unattendedTimeZoneList = new List<UnattendedTimeZone>();

        /// <summary>
        /// 使用した打点数を傾向管理ログから取得する際の管理番号
        /// </summary>
        private class UseHitCtQcparamNo
        {
            public int ProcNo { get; set; }

            public string StampCd { get; set; }

            public int QcParamNo { get; set; }
            
        }
        private List<UseHitCtQcparamNo> useHitCtQcparamNoList = new List<UseHitCtQcparamNo>();

        private List<Machine> machineList;
        public List<Machine> MachineList
        {
            get
            {
                if (machineList == null)
                {
                    machineList = new List<Machine>();
                }
                return machineList;
            }
            set
            {
                this.machineList = value;
                NotifyPropertyChanged(nameof(MachineList));
            }
        }

        private RelayCommand settingCommand;
        public RelayCommand SettingCommand
        {
            get { return settingCommand = settingCommand ?? new RelayCommand(Setting); }
        }

        public void Setting(object obj)
        {
        }

        private RelayCommand updateCommand;
        public RelayCommand UpdateCommand
        {
            get { return updateCommand = updateCommand ?? new RelayCommand(Update); }
        }

        public void Update(object obj)
        {
            DateTime updStartDt = System.DateTime.Now;

            try
            {
                List<Machine> mList = new List<Machine>();

                List<int> inTransitMacNoList = null;
                if (Properties.Settings.Default.TabletMode != true)
                {
                    inTransitMacNoList = MachineInfo.GetInTransitMachineList();
                }
                Dictionary<string, bool> machineReadyList = MachineSignal.GetReadyList();

                List<string> PhosphorSheetMateGroupCdList = new List<string>();
                foreach(GnlMaster gen in GnlMaster.GetPhosphorSheetMateGroupCd())
                {
                    if (string.IsNullOrWhiteSpace(gen.Val) == false)
                        PhosphorSheetMateGroupCdList.Add(gen.Val);
                }

                foreach (Machine m in this.MachineList)
                {
                    MachineInfo mInfo = MachineInfo.GetMachine(m.PlantCd);
                    m.MachineName = mInfo.MachineName;
                    m.ClassName = mInfo.ClassName;

                    MachineInfo.Eics eicsSetting = ArmsApi.Model.MachineInfo.Eics.GetSetting(mInfo.NascaPlantCd);
                    if (eicsSetting != null)
                    {
                        if (string.IsNullOrEmpty(eicsSetting.TypeGroupCode))
                            m.TypeCode = eicsSetting.TypeCode;
                        else
                            m.TypeCode = eicsSetting.TypeGroupCode;
                        m.ChipName = eicsSetting.ChipName;
                        m.LineName = eicsSetting.LineName;
                        m.MachineLogOutputPath = eicsSetting.MachineLogPath;
                    }

                    // 運転状態表示(背景色灰色)
                    if (machineReadyList.Where(k => k.Key == m.PlantCd).Count() != 0) 
                        m.IsReady = machineReadyList[m.PlantCd];

                    if (Properties.Settings.Default.TabletMode)
                    {
                        mList.Add(m);
                        continue;
                    }
                    
                    // 空数
                    int? loaderMagazineFreeCount = getLoaderMagazineFreeCt(mInfo, inTransitMacNoList);
                    if (loaderMagazineFreeCount.HasValue)
                        m.LoaderMagazineFreeCount = loaderMagazineFreeCount.Value.ToString();

                    Material[] matList = new List<Material>().ToArray();
                    if (mInfo.ResinCheckFg == true)
                    {
                        Resin[] resinList = mInfo.GetResins(System.DateTime.Now, System.DateTime.Now);
                        m.ResinGroupCode = string.Join(",", resinList.Select(r => r.ResinGroupCd));

                        // 一番短い樹脂の有効期限
                        m.MaterialLimitTime = resinList.OrderBy(r => r.LimitDt).FirstOrDefault()?.LimitDt;
                    }
                    else
                    {
                        matList = mInfo.GetMaterials(System.DateTime.Now, System.DateTime.Now);

                        // 一番短い資材の有効期限
                        m.MaterialLimitTime = getMaterialFirstLimitDt(matList, mInfo);

                        if (Properties.Settings.Default.BlendCdVisible)
                        {
                            m.BlendCode = string.Join(",", matList.Where(mat => mat.IsWafer).Select(mat => mat.BlendCd).Distinct());
                            m.ResinGroupCode = WorkCondition.GetBlendConditionResinGroupCd(m.BlendCode);
                        }

                        if (Properties.Settings.Default.SupplyIdVisible)
                            m.SupplyId = string.Join(",", matList.Where(mat => mat.IsWafer).Select(mat => mat.Supplyid).Distinct());

                        // DB樹脂交換予測
                        if (eicsSetting != null && string.IsNullOrWhiteSpace(eicsSetting.TypeCode) == false)
                        {
                            string resinChangePlanTime = getResinChangePlanTime(mInfo, matList, eicsSetting.TypeCode);
                            if (string.IsNullOrEmpty(resinChangePlanTime) == false)
                                m.ResinChangePlanTime = resinChangePlanTime;
                        }
                    }

                    if (Properties.Settings.Default.PhosphorSheetMatCdVisible || Properties.Settings.Default.PhosphorSheetResinGroupCdVisible)
                    {
                        List<Material> phosphorSheetList = mInfo.GetMaterials().Where(mat => PhosphorSheetMateGroupCdList.Contains(mat.HMGroup)).ToList();
                        m.PhosphorSheetMatCd = string.Join(",", phosphorSheetList.Select(p => p.MaterialCd).Distinct());
                        List<int> matResinGroupCdList = new List<int>();
                        foreach(Material phosphorSheet in phosphorSheetList)
                        {
                            MatChar[] mcList = MatChar.GetMatChar(phosphorSheet.MaterialCd, phosphorSheet.LotNo, ArmsApi.Config.RESINGROUP_LOTCHARCD);
                            foreach(MatChar mc in mcList.Where(mc => string.IsNullOrWhiteSpace(mc.LotCharVal) == false))
                            {
                                string[] matResinGroup = mc.LotCharVal.Split(',');
                                foreach(string mrg in matResinGroup)
                                {
                                    int iMatResinGroup;
                                    if (int.TryParse(mrg, out iMatResinGroup) == false) continue;
                                    if (matResinGroupCdList.Contains(iMatResinGroup) == false)
                                        matResinGroupCdList.Add(iMatResinGroup);
                                }
                            }
                        }
                        m.PhosphorSheetResinGroupCd = string.Join(",", matResinGroupCdList.OrderBy(mrg => mrg));
                    }

                    // 設備に投入中のロット一覧取得
                    Order[] workOrderList = Order.GetCurrentWorkingOrderInMachine(m.MacNo);
                    if (workOrderList.Count() == 0)
                    {
                        m.LoadMagazinePlanTime = System.DateTime.Now.ToString();
                    }
                    else
                    {
                        // 先頭の投入中ロット情報取得
                        Order firstOrder = workOrderList.OrderBy(w => w.WorkStartDt).FirstOrDefault();
                        AsmLot firstLot = AsmLot.GetAsmLot(firstOrder.NascaLotNo);
                        Profile firstLotProfile = Profile.GetProfile(firstLot.ProfileId);
                        if (firstLotProfile != null)
                        {
                            m.AimRank = firstLotProfile.AimRank;
                        }

                        float uph = ArmsApi.Model.LAMS.Work.GetMachineUph(m.PlantCd, firstLot.TypeCd, Properties.Settings.Default.UphSampleLotCt, firstOrder.ProcNo);

                        DateTime startDt = firstOrder.WorkStartDt;
                        ArmsApi.Model.LAMS.Work oneBeforeLot = ArmsApi.Model.LAMS.Work.GetMachineFirstRecord(m.PlantCd, firstLot.TypeCd);                        
                        if (oneBeforeLot != null && startDt < oneBeforeLot.LotEndDt)
                        {
                            // 自動搬送ラインでは実績の開始時間はロボット搬送した時間となっている為、(実際の開始は随分後)
                            // 前ロットの完了時間が投入中ロットの開始時間を超えている場合は前ロットの完了時間を採用する。
                            startDt = oneBeforeLot.LotEndDt;
                        }

                        DateTime? magPlanTime = getLoadMagazinePlanTime(mInfo, workOrderList, startDt, uph);
                        if (magPlanTime.HasValue)
                            m.LoadMagazinePlanTime = magPlanTime.ToString();

                        // ﾀﾞｲﾎﾞﾝﾀﾞｰ
                        if (mInfo.WaferCheckFg)
                        {
                            //int qcParamNo = GetUseHitCtQcParamNo(firstOrder.ProcNo, "L");

                            int qcParamNo = 0;

                            string modelNm = MachineInfo.GetEICSModelNm(mInfo.NascaPlantCd);
                            if (modelNm.Contains("AD"))
                            {
                                // MAP
                                //if (firstOrder.ProcNo == 4) qcParamNo = 146;
                                //else qcParamNo = 33;

                                //MPL
                                if (firstOrder.ProcNo == 1) qcParamNo = 68;
                                else qcParamNo = 32;

                                string stampPinPlanTime = getDiebondStampPinPlanTime(mInfo, eicsSetting.ModelName, startDt, uph, firstLot.TypeCd, firstOrder.ProcNo, qcParamNo);
                                if (string.IsNullOrEmpty(stampPinPlanTime) == false)
                                    m.LeftStampPinChangePlanTime = stampPinPlanTime;

                                // MAP
                                //if (firstOrder.ProcNo == 4) qcParamNo = 147;
                                //else qcParamNo = 34;

                                // MPL
                                if (firstOrder.ProcNo == 1) qcParamNo = 69;
                                else qcParamNo = 33;

                                //qcParamNo = GetUseHitCtQcParamNo(firstOrder.ProcNo, "R");

                                stampPinPlanTime = getDiebondStampPinPlanTime(mInfo, eicsSetting.ModelName, startDt, uph, firstLot.TypeCd, firstOrder.ProcNo, qcParamNo);
                                if (string.IsNullOrEmpty(stampPinPlanTime) == false)
                                    m.RightStampPinChangePlanTime = stampPinPlanTime;
                            }
                            else
                            {
                                if (firstOrder.ProcNo == 1) qcParamNo = 106;
                                else qcParamNo = 79;

                                string stampPinPlanTime = getDiebondStampPinPlanTime(mInfo, eicsSetting.ModelName, startDt, uph, firstLot.TypeCd, firstOrder.ProcNo, qcParamNo);
                                if (string.IsNullOrEmpty(stampPinPlanTime) == false)
                                    m.StampPinChangePlanTime = stampPinPlanTime;
                            }

                            string colletPlanTime = getDiebondColletPlanTime(mInfo, eicsSetting.ModelName, startDt, uph, firstLot.TypeCd, firstOrder.ProcNo);
                            if (string.IsNullOrEmpty(colletPlanTime) == false)
                                m.ColletChangePlanTime = colletPlanTime;

                            string ejectPinPlanTime = getDiebondEjectPinPlanTime(mInfo, eicsSetting.ModelName, startDt, uph, firstLot.TypeCd, firstOrder.ProcNo);
                            if (string.IsNullOrEmpty(ejectPinPlanTime) == false)
                                m.EjectPinChangePlanTime = ejectPinPlanTime;

                            if (firstLotProfile != null)
                            {
                                string chipCt = getChipCt(mInfo, workOrderList, matList.Where(mat => mat.IsWafer).ToArray(), firstOrder.WorkStartDt, uph, m.MachineLogOutputPath, firstLotProfile.LotSize, firstLot.TypeCd, firstOrder.ProcNo);
                                if (string.IsNullOrEmpty(chipCt) == false)
                                {
                                    m.ChipCt = chipCt;

                                    string chipChangePlanTime = getChipChangePlanTime(mInfo, int.Parse(chipCt), firstOrder.WorkStartDt, uph, firstLot.TypeCd, firstOrder.ProcNo);
                                    if (string.IsNullOrEmpty(chipChangePlanTime) == false)
                                        m.ChipChangePlanTime = chipChangePlanTime;
                                }
                            }
                        }

                        //ﾜｲﾔｰﾎﾞﾝﾀﾞｰ
                        if (MachineInfo.IsWireBondMachine(mInfo.ClassName))
                        {
                            string capilaryPlanTime = getWirebondCapillaryPlanTime(mInfo, eicsSetting.ModelName, startDt, uph, firstLot.TypeCd, firstOrder.ProcNo);
                            if (string.IsNullOrEmpty(capilaryPlanTime) == false)
                                m.CapillaryChangePlanTime = capilaryPlanTime.ToString();

                            string goldWirePlanTime = getWirebondGoldWirePlanTime(mInfo, matList, eicsSetting.ModelName, startDt, uph, firstLot.TypeCd, firstOrder.ProcNo, eicsSetting.MachineLogPath);
                            if (string.IsNullOrEmpty(goldWirePlanTime) == false)
                                m.GoldWireChangePlanTime = goldWirePlanTime.ToString();
                        }
                    }

                    if (HasUnattendedTimeZone(m.LoadMagazinePlanTime) || HasUnattendedTimeZone(m.ChipChangePlanTime)
                        || HasUnattendedTimeZone(m.LeftStampPinChangePlanTime) || HasUnattendedTimeZone(m.RightStampPinChangePlanTime)
                        || HasUnattendedTimeZone(m.ColletChangePlanTime) || HasUnattendedTimeZone(m.EjectPinChangePlanTime)
                        || HasUnattendedTimeZone(m.GoldWireChangePlanTime) || HasUnattendedTimeZone(m.CapillaryChangePlanTime))

                    {
                        m.IsUnattendedTimeZone = true;
                    }

                    mList.Add(m);
                }

                MachineList = mList;

                if (mList.Exists(mac => mac.IsReady == false))
                {
                    Task.Run(() =>
                    {
                        sp.PlayLooping();
                        Thread.Sleep(1000);
                        sp.Stop();
                    });
                }
                
                LastUpdate = System.DateTime.Now.ToString();
                NextUpdate = System.DateTime.Now.AddMinutes(Properties.Settings.Default.MachineMonitorUpdateMinutes).ToString();

                DateTime updEndDt = System.DateTime.Now;
                TimeSpan span = updEndDt - updStartDt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 装置ローダーマガジンの空数を取得
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="inTransitMacNoList"></param>
        /// <returns></returns>
        private int? getLoaderMagazineFreeCt(MachineInfo machine, List<int> inTransitMacNoList)
        {
            if (Properties.Settings.Default.LoaderMagazineFreeCountVisible == false)
                return null;

            // 空き数 = 装置ローダー最大積載数 - 装置隣接CV、装置ローダー上(ローダー仮想マガジン)のマガジン数量 - AGV、搬送ロボットの搬送中数量(自動搬送ラインのみ)
            int loaderMagazineCt = 0;
            if (Config.GetLineType == Config.LineTypes.MEL_MPL)
            {
                // TODO LineTypeでの切り分けは暫定対応 仮想マガジンが作成されない装置を考慮して切り分け(内製モールドなど)
                VirtualMag[] lMagazineList = VirtualMag.GetVirtualMag(machine.MacNo, (int)Station.Loader);
                loaderMagazineCt = lMagazineList.Count();
            }
            else
            {
                Order[] startOrderList = Order.GetCurrentWorkingOrderInMachine(machine.MacNo);
                //分割マガジンを考慮して、同マガジンの実績は纏める
                loaderMagazineCt = startOrderList.Select(s => s.InMagazineNo).Distinct().Count();
            }

            int inTransitMagazineCt = 0;
            if (machine.IsAgvLine || machine.IsAutoLine)
            {
                inTransitMagazineCt = VirtualMag.GetInTransitMagazineCt(inTransitMacNoList, machine.MacNo);
            }

            if (machine.LoaderMagazineMaxCt == 0)
                return null;

            return machine.LoaderMagazineMaxCt - loaderMagazineCt - inTransitMagazineCt;
        }

        /// <summary>
        /// マガジン投入予測時刻を取得
        /// </summary>
        /// <returns></returns>
        private DateTime? getLoadMagazinePlanTime(MachineInfo machine, Order[] workOrderList, DateTime startDt, float uph)
        {
            if (Properties.Settings.Default.LoadMagazinePlanTimeVisible == false)
                return null;

            if (workOrderList.Count() == 0)
                // 投入中ロットが無い場合は現在時刻が投入予測時刻
                return System.DateTime.Now;

            // 先頭の投入中ロットで装置停止した時間を取得
            double stopHour = ArmsApi.Model.LAMS.HeartBeat.GetMachineStopHour(machine.NascaPlantCd, startDt, System.DateTime.Now);

            // 投入中ロットの予測作業時間を足していく
            DateTime orderCompltDt = startDt;
            foreach (Order workOrder in workOrderList)
            {
                AsmLot lot = AsmLot.GetAsmLot(workOrder.NascaLotNo);
                Profile profile = Profile.GetProfile(lot.ProfileId);
                int defectCt = Defect.GetDefectCountOfPassedProcess(lot, workOrder.ProcNo);

                float lotTime = (profile.LotSize - defectCt) / uph;
                orderCompltDt = orderCompltDt.AddHours(lotTime);
            }

            // 投入中ロットの作業時間+装置停止時間を返す
            orderCompltDt = orderCompltDt.AddHours(stopHour);
            return orderCompltDt;
        }
        
        #region Diebonder

        /// <summary>
        /// ﾁｯﾌﾟ数取得
        /// </summary>
        /// <param name="waferList"></param>
        /// <returns></returns>
        private string getChipCt(MachineInfo machine, Order[] workOrderList, Material[] waferList, DateTime startDt, float uph, string machineLogOutputPath, int lotSizeCt, string typeCd, int procNo)
        {
            try
            {
                if (Properties.Settings.Default.ChipCtVisible == false)
                    return null;

                if (waferList.FirstOrDefault() == null)
                    return null;

                DateTime inputDt = waferList.FirstOrDefault().InputDt.Value;
                DateTime stockUpdDt = waferList.FirstOrDefault().StockLastUpdDt.Value;
                if (inputDt < stockUpdDt)
                {
                    inputDt = stockUpdDt;
                }

                int usedChipCt = 0;
                List<Material> calcWaferList = waferList.ToList();

                Order order = Order.GetOrderOneBeforeRecord(machine.MacNo, System.DateTime.Now, inputDt);
                if (order != null)
                {
                    // 資材割付～現在までに完了したロットが有る(前ロットの使用在庫数を現在庫数に反映させるため更新)

#if DEBUG
                    machineLogOutputPath = @"C:\QCIL\data\DB";
#endif
                    string dateFolderName = Path.Combine(order.WorkEndDt.Value.ToString("yyyyMM"), "Bind");

                    if (Directory.Exists(dateFolderName))
                    {
                        string[] lotDir = Directory.GetDirectories(Path.Combine(machineLogOutputPath, dateFolderName), order.NascaLotNo);
                        if (lotDir.Count() == 0)
                        {
                            usedChipCt = lotSizeCt * workOrderList.Count() * 1;
                        }
                        else
                        {
                            string[] file = DirectoryHelper.GetFiles(lotDir.FirstOrDefault(), "^I.*$");
                            if (file.Count() == 0)
                            {
                                usedChipCt = lotSizeCt * workOrderList.Count() * 1;
                            }
                            string[] fileLines = File.ReadAllLines(file.FirstOrDefault(), Encoding.Default)
                                .Where(f => f.Split(',').Count() >= 15).ToArray();

                            usedChipCt = fileLines.Count();
                        }
                    }
                    else
                    {
                        usedChipCt = lotSizeCt * workOrderList.Count() * 1;
                    }

                    // ﾁｯﾌﾟ在庫から使用数を差し引いた更新用ﾁｯﾌﾟ在庫リスト作成
                    List<Material> updWaferList = new List<Material>();
                    for (int i = 0; i < waferList.Count(); i++)
                    {
                        Material m = new Material();
                        m.MaterialCd = waferList[i].MaterialCd;
                        m.LotNo = waferList[i].LotNo;
                        m.StockCt = waferList[i].StockCt - usedChipCt;
                        if (m.StockCt < 0) m.StockCt = 0;

                        usedChipCt = usedChipCt - (int)waferList[i].StockCt;
                        if (usedChipCt < 0) usedChipCt = 0;

                        updWaferList.Add(m);
                    }
#if !DEBUG
                    foreach (Material wafer in updWaferList)
                    {
                        Material.UpdateStockCount(wafer.MaterialCd, wafer.LotNo, wafer.StockCt, null);
                    }
#endif
                    calcWaferList = updWaferList;
                }

                // 先頭の投入中ロットで装置停止した時間を取得
                double stopHour = ArmsApi.Model.LAMS.HeartBeat.GetMachineStopHour(machine.NascaPlantCd, startDt, System.DateTime.Now);

                ArmsApi.Model.EICS.Bonding bondSetting = ArmsApi.Model.EICS.Bonding.GetSetting(typeCd, procNo);
                if (bondSetting == null)
                    throw new ApplicationException($"[算出不可] 型番:{typeCd} 作業:{procNo} ﾁｯﾌﾟ搭載数ﾏｽﾀ無し");

                TimeSpan firstLotProgressSpan = DateTime.Now - startDt;
                float firstLotWorkHour = Convert.ToSingle(firstLotProgressSpan.TotalHours - stopHour);

                usedChipCt = Convert.ToInt32(uph * firstLotWorkHour * bondSetting.ChipBondCt);
                int retvCt = Convert.ToInt32(calcWaferList.Sum(w => w.StockCt) - usedChipCt);
                if (retvCt < 0) retvCt = 0;

                return retvCt.ToString();
            }
            catch (ApplicationException err)
            {
                return err.Message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// ﾁｯﾌﾟ交換予測時刻を取得
        /// </summary>
        /// <param name="chipCt"></param>
        /// <param name="uph"></param>
        /// <returns></returns>
        private string getChipChangePlanTime(MachineInfo machine, int chipCt, DateTime startDt, float uph, string typeCd, int procNo)
        {
            try
            {
                if (chipCt == 0)
                    return System.DateTime.Now.ToString();

                ArmsApi.Model.EICS.Bonding bondSetting = ArmsApi.Model.EICS.Bonding.GetSetting(typeCd, procNo);
                if (bondSetting == null)
                    throw new ApplicationException($"[算出不可] 型番:{typeCd} 作業:{procNo} ﾁｯﾌﾟ搭載数ﾏｽﾀ無し");

                // 先頭の投入中ロットで装置停止した時間を取得
                double stopHour = ArmsApi.Model.LAMS.HeartBeat.GetMachineStopHour(machine.NascaPlantCd, startDt, System.DateTime.Now);

                double outOfStockHour = Convert.ToDouble(chipCt / (uph * bondSetting.ChipBondCt));
                return System.DateTime.Now.AddHours(outOfStockHour + stopHour).ToString();
            }
            catch (ApplicationException err)
            {
                return err.Message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// スタンプピン交換予測日時
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="workOrderList"></param>
        /// <param name="startDt"></param>
        /// <param name="uph"></param>
        /// <returns></returns>
        private string getDiebondStampPinPlanTime(MachineInfo machine, string eicsModelNm, DateTime startDt, float uph, string typeCd, int procNo, int qcParamNo)
        {
#if DEBUG
            //if (machine.MachineName == "371")
            //    Console.WriteLine("");
#endif

            try
            {
                if (Properties.Settings.Default.StampPinChangePlanTimeVisible == false)
                    return null;              

                DateTime startPointDt = startDt;

                decimal? limit = ArmsApi.Model.EICS.LimitManagement.GetUpperLimit(eicsModelNm, qcParamNo, typeCd);
                if (limit.HasValue == false)
                {
                    throw new ApplicationException($"[算出不可] 管理番号:{qcParamNo} 閾値登録無し");
                }
                decimal lotHitCt = lotHitCountFromLamsRecord(machine, typeCd, qcParamNo, "MATE043", procNo);

                // 投入中ﾛｯﾄでｽﾀﾝﾌﾟﾋﾟﾝの交換があるか調べる
                Material[] changeMateriaList = machine.GetMaterials()
                    ?.Where(m => m.InputDt >= startDt && m.InputDt <= System.DateTime.Now)
                    ?.Where(m => m.HMGroup == "MATE043").ToArray();

                decimal? oneBeforeLotHitCt = null;  ArmsApi.Model.LAMS.Work oneBeforeLotWork = null;
                if (changeMateriaList.Count() != 0)
                {
                    // ｽﾀﾝﾌﾟﾋﾟﾝの交換を行っている場合、打点数は0で計算、起点日時(startPointDt)も交換した日時に変更
                    oneBeforeLotHitCt = 0;
                    startPointDt = changeMateriaList
                        .OrderByDescending(m => m.InputDt).FirstOrDefault().InputDt.Value;
                }
                else
                {
                    oneBeforeLotWork = ArmsApi.Model.LAMS.Work.GetMachineFirstRecord(machine.NascaPlantCd, typeCd);
                    if (oneBeforeLotWork == null)
                    {
                        throw new ApplicationException($"[算出不可] 過去ロットLAMS実績無し");
                    }
                    int lineNo = MachineInfo.GetEICSLineNo(machine.NascaPlantCd);
                    oneBeforeLotHitCt = ArmsApi.Model.EICS.MachineLog.GetNumericValue(lineNo, machine.NascaPlantCd, qcParamNo, oneBeforeLotWork.LotNo);
                    if (oneBeforeLotHitCt.HasValue == false)
                    {
                        throw new ApplicationException($"[算出不可] 過去ロット:{oneBeforeLotWork.LotNo} 打点数取得失敗");
                    }
                }

                AsmLot lot = AsmLot.GetAsmLot(oneBeforeLotWork.LotNo);
                int defectCt = Defect.GetDefectCountOfPassedProcess(lot, procNo);

                // 先頭の投入中ロットで装置停止した時間を取得
                double stopHour = ArmsApi.Model.LAMS.HeartBeat.GetMachineStopHour(machine.NascaPlantCd, startDt, System.DateTime.Now);

                decimal pcsHitCt = lotHitCt / (oneBeforeLotWork.LotSizeCt - defectCt);
                decimal remainingHitCt = limit.Value - oneBeforeLotHitCt.Value;
                decimal remainingPcs = remainingHitCt / pcsHitCt;
                decimal remainingHour = remainingPcs / (decimal)uph;
                return startPointDt.AddHours((double)remainingHour + stopHour).ToString();
            }
            catch (ApplicationException err)
            {
                return err.Message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// コレット交換予測日時
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="workOrderList"></param>
        /// <param name="startDt"></param>
        /// <param name="uph"></param>
        /// <returns></returns>
        private string getDiebondColletPlanTime(MachineInfo machine, string eicsModelNm, DateTime startDt, float uph, string typeCd, int procNo)
        {
            try
            {
                if (Properties.Settings.Default.ColletChangePlanTimeVisible == false)
                    return null;

                int qcParamNo = 0;

                // MAP
                //if (procNo == 4) qcParamNo = 145;
                //else qcParamNo = 32;

                // MPL
                if (procNo == 1) qcParamNo = 67;
                else qcParamNo = 31;

                //int qcParamNo = GetUseHitCtQcParamNo(procNo);

                DateTime startPointDt = startDt;

                decimal? limit = ArmsApi.Model.EICS.LimitManagement.GetUpperLimit(eicsModelNm, qcParamNo, typeCd);
                if (limit.HasValue == false)
                {
                    throw new ApplicationException($"[算出不可] 管理番号:{qcParamNo} 閾値登録無し");
                }
                decimal lotHitCt = lotHitCountFromLamsRecord(machine, typeCd, qcParamNo, "MATE042", procNo);

                // 投入中ﾛｯﾄでｺﾚｯﾄの交換があるか調べる
                Material[] changeMateriaList = machine.GetMaterials()
                    ?.Where(m => m.InputDt >= startDt && m.InputDt <= System.DateTime.Now)
                    ?.Where(m => m.HMGroup == "MATE042").ToArray();

                decimal? oneBeforeLotHitCt = null; ArmsApi.Model.LAMS.Work oneBeforeLotWork = null;
                if (changeMateriaList.Count() != 0)
                {
                    // ｺﾚｯﾄの交換を行っている場合、打点数は0で計算、起点日時(startPointDt)も交換した日時に変更
                    oneBeforeLotHitCt = 0;
                    startPointDt = changeMateriaList
                        .OrderByDescending(m => m.InputDt).FirstOrDefault().InputDt.Value;
                }
                else
                {
                    oneBeforeLotWork = ArmsApi.Model.LAMS.Work.GetMachineFirstRecord(machine.NascaPlantCd, typeCd);
                    if (oneBeforeLotWork == null)
                    {
                        throw new ApplicationException($"[算出不可] 過去ロットLAMS実績無し");
                    }
                    int lineNo = MachineInfo.GetEICSLineNo(machine.NascaPlantCd);
                    oneBeforeLotHitCt = ArmsApi.Model.EICS.MachineLog.GetNumericValue(lineNo, machine.NascaPlantCd, qcParamNo, oneBeforeLotWork.LotNo);
                    if (oneBeforeLotHitCt.HasValue == false)
                    {
                        throw new ApplicationException($"[算出不可] 過去ロット:{oneBeforeLotWork.LotNo} 打点数取得失敗");
                    }
                }

                AsmLot lot = AsmLot.GetAsmLot(oneBeforeLotWork.LotNo);
                int defectCt = Defect.GetDefectCountOfPassedProcess(lot, procNo);

                // 先頭の投入中ロットで装置停止した時間を取得
                double stopHour = ArmsApi.Model.LAMS.HeartBeat.GetMachineStopHour(machine.NascaPlantCd, startDt, System.DateTime.Now);

                decimal pcsHitCt = lotHitCt / (oneBeforeLotWork.LotSizeCt - defectCt);
                decimal remainingHitCt = limit.Value - oneBeforeLotHitCt.Value;
                decimal remainingPcs = remainingHitCt / pcsHitCt;
                decimal remainingHour = remainingPcs / (decimal)uph;
                return startPointDt.AddHours((double)remainingHour + stopHour).ToString();
            }
            catch (ApplicationException err)
            {
                return err.Message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// イジェクトピン交換予測日時
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="workOrderList"></param>
        /// <param name="startDt"></param>
        /// <param name="uph"></param>
        /// <returns></returns>
        private string getDiebondEjectPinPlanTime(MachineInfo machine, string eicsModelNm, DateTime startDt, float uph, string typeCd, int procNo)
        {
            try
            {
                if (Properties.Settings.Default.EjectPinChangePlanTimeVisible == false)
                    return null;

                int qcParamNo = 0;

                // MAP
                //if (procNo == 4) qcParamNo = 148;
                //else qcParamNo = 35;

                // MPL
                if (procNo == 1) qcParamNo = 70;
                else qcParamNo = 34;

                DateTime startPointDt = startDt;

                decimal? limit = ArmsApi.Model.EICS.LimitManagement.GetUpperLimit(eicsModelNm, qcParamNo, typeCd);
                if (limit.HasValue == false)
                {
                    throw new ApplicationException($"[算出不可] 管理番号:{qcParamNo} 閾値登録無し");
                }
                decimal lotHitCt = lotHitCountFromLamsRecord(machine, typeCd, qcParamNo, "MATE044", procNo);

                // 投入中ﾛｯﾄでｲｼﾞｪｸﾄの交換があるか調べる
                Material[] changeMateriaList = machine.GetMaterials()
                    ?.Where(m => m.InputDt >= startDt && m.InputDt <= System.DateTime.Now)
                    ?.Where(m => m.HMGroup == "MATE044").ToArray();

                decimal? oneBeforeLotHitCt = null; ArmsApi.Model.LAMS.Work oneBeforeLotWork = null;
                if (changeMateriaList.Count() != 0)
                {
                    // ｲｼﾞｪｸﾄの交換を行っている場合、打点数は0で計算、起点日時(startPointDt)も交換した日時に変更
                    oneBeforeLotHitCt = 0;
                    startPointDt = changeMateriaList
                        .OrderByDescending(m => m.InputDt).FirstOrDefault().InputDt.Value;
                }
                else
                {
                    oneBeforeLotWork = ArmsApi.Model.LAMS.Work.GetMachineFirstRecord(machine.NascaPlantCd, typeCd);
                    if (oneBeforeLotWork == null)
                    {
                        throw new ApplicationException($"[算出不可] 過去ロットLAMS実績無し");
                    }
                    int lineNo = MachineInfo.GetEICSLineNo(machine.NascaPlantCd);
                    oneBeforeLotHitCt = ArmsApi.Model.EICS.MachineLog.GetNumericValue(lineNo, machine.NascaPlantCd, qcParamNo, oneBeforeLotWork.LotNo);
                    if (oneBeforeLotHitCt.HasValue == false)
                    {
                        throw new ApplicationException($"[算出不可] 過去ロット:{oneBeforeLotWork.LotNo} 打点数取得失敗");
                    }
                }

                AsmLot lot = AsmLot.GetAsmLot(oneBeforeLotWork.LotNo);
                int defectCt = Defect.GetDefectCountOfPassedProcess(lot, procNo);

                // 先頭の投入中ロットで装置停止した時間を取得
                double stopHour = ArmsApi.Model.LAMS.HeartBeat.GetMachineStopHour(machine.NascaPlantCd, startDt, System.DateTime.Now);

                decimal pcsHitCt = lotHitCt / (oneBeforeLotWork.LotSizeCt - defectCt);
                decimal remainingHitCt = limit.Value - oneBeforeLotHitCt.Value;
                decimal remainingPcs = remainingHitCt / pcsHitCt;
                decimal remainingHour = remainingPcs / (decimal)uph;
                return startPointDt.AddHours((double)remainingHour + stopHour).ToString();
            }
            catch (ApplicationException err)
            {
                return err.Message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DB樹脂交換予測時刻を取得
        /// </summary>
        /// <param name="machine"></param>
        /// <returns></returns>
        private string getResinChangePlanTime(MachineInfo machine, Material[] matList, string typeCd)
        {
            if (Properties.Settings.Default.DbResinChangePlanTimeVisible == false)
                return null;

            if (machine.WaferCheckFg == false)
                return null;
            
            if (matList.Count() == 0)
                return null;

            Material dbResin = matList.Where(m => m.HMGroup == "MATE015").FirstOrDefault();
            if (dbResin == null)
                return null;

            DateTime limitdt = dbResin.LimitDt;

            DateTime dbResinLimit = Material.GetDBResinLimitDt(typeCd, dbResin.MaterialCd, dbResin.LotNo, dbResin.InputDt.Value, true);
            if (limitdt >= dbResinLimit) limitdt = dbResinLimit;

            // 時間監視と同じ固定値
            int attensionMinutes = 60;

            return limitdt.AddMinutes(-attensionMinutes).ToString();
        }

#endregion

#region Wirebonder
        /// <summary>
        /// キャピラリ交換予測日時
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="workOrderList"></param>
        /// <param name="startDt"></param>
        /// <param name="uph"></param>
        /// <returns></returns>
        private string getWirebondCapillaryPlanTime(MachineInfo machine, string eicsModelNm, DateTime startDt, float uph, string typeCd, int procNo)
        {
            try
            {
                if (Properties.Settings.Default.CapillaryChangePlanTimeVisible == false)
                    return null;

                // MAP
                //int qcParamNo = 434;

                // MPL
                int qcParamNo = 136;

                DateTime startPointDt = startDt;

                decimal? limit = ArmsApi.Model.EICS.LimitManagement.GetUpperLimit(eicsModelNm, qcParamNo, typeCd);
                if (limit.HasValue == false)
                {
                    throw new ApplicationException($"[算出不可] 管理番号:{qcParamNo} 閾値登録無し");
                }
                decimal lotHitCt = lotHitCountFromLamsRecord(machine, typeCd, qcParamNo, "MATE024", procNo);

                ArmsApi.Model.LAMS.Work oneBeforeLotWork = ArmsApi.Model.LAMS.Work.GetMachineFirstRecord(machine.NascaPlantCd, typeCd);
                if (oneBeforeLotWork == null)
                {
                    throw new ApplicationException($"[算出不可] 過去ロットLAMS実績無し");
                }

                // 投入中ﾛｯﾄでキャピラリの交換があるか調べる
                Material[] changeMateriaList = machine.GetMaterials()
                    ?.Where(m => m.InputDt >= startDt && m.InputDt <= System.DateTime.Now)
                    ?.Where(m => m.HMGroup == "MATE024").ToArray();
                
                decimal? oneBeforeLotHitCt = null;
                if (changeMateriaList.Count() != 0)
                {
                    // キャピラリの交換を行っている場合、打点数は0で計算、起点日時(startPointDt)も交換した日時に変更
                    oneBeforeLotHitCt = 0;
                    startPointDt = changeMateriaList
                        .OrderByDescending(m => m.InputDt).FirstOrDefault().InputDt.Value;
                }
                else
                {
                    int lineNo = MachineInfo.GetEICSLineNo(machine.NascaPlantCd);
                    oneBeforeLotHitCt = ArmsApi.Model.EICS.MachineLog.GetNumericValue(lineNo, machine.NascaPlantCd, qcParamNo, oneBeforeLotWork.LotNo);
                    if (oneBeforeLotHitCt.HasValue == false)
                    {
                        throw new ApplicationException($"[算出不可] 過去ロット:{oneBeforeLotWork.LotNo} 打点数取得失敗");
                    }
                }

                AsmLot lot = AsmLot.GetAsmLot(oneBeforeLotWork.LotNo);
                int defectCt = Defect.GetDefectCountOfPassedProcess(lot, procNo);

                // 先頭の投入中ロットで装置停止した時間を取得
                double stopHour = ArmsApi.Model.LAMS.HeartBeat.GetMachineStopHour(machine.NascaPlantCd, startDt, System.DateTime.Now);

                decimal pcsHitCt = lotHitCt / (oneBeforeLotWork.LotSizeCt - defectCt);
                decimal remainingHitCt = limit.Value - oneBeforeLotHitCt.Value;

                decimal remainingPcs = remainingHitCt / pcsHitCt;
                decimal remainingHour = remainingPcs / (decimal)uph;
                return startPointDt.AddHours((double)remainingHour + stopHour).ToString();
            }
            catch (ApplicationException err)
            {
                return err.Message;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string getWirebondGoldWirePlanTime(MachineInfo machine, Material[] matList, string eicsModelNm, DateTime startDt, float uph, string typeCd, int procNo, string machineLogOutputPath)
        {
            try
            {
                if (Properties.Settings.Default.GoldWireChangePlanTimeVisible == false)
                    return null;

                DateTime startPointDt = startDt;

                decimal? limit = matList.Where(m => m.HMGroup == "MATE016").FirstOrDefault().StockCt;
                if (limit.HasValue == false || limit.Value == 0)
                {
                    throw new ApplicationException($"[算出不可] 在庫数取得不可");
                }

                //decimal? limit = ArmsApi.Model.EICS.LimitManagement.GetUpperLimit(eicsModelNm, qcParamNo, typeCd);
                //if (limit.HasValue == false)
                //{
                //    throw new ApplicationException($"[算出不可] 管理番号:{qcParamNo} 閾値登録無し");
                //}
                //decimal lotHitCt = lotHitCountFromLamsRecord(machine, typeCd, qcParamNo, "MATE016", procNo);
                decimal lotGoldWireCt = goldWireUseCtFromMpFile(machine, typeCd, "MATE016", procNo, machineLogOutputPath);

                // 投入中ﾛｯﾄで金線の交換があるか調べる
                Material[] changeMateriaList = machine.GetMaterials()
                    ?.Where(m => m.InputDt >= startDt && m.InputDt <= System.DateTime.Now)
                    ?.Where(m => m.HMGroup == "MATE016").ToArray();

                decimal? oneBeforeLotGoldWireCt = null; ArmsApi.Model.LAMS.Work oneBeforeLotWork = null;

                oneBeforeLotWork = ArmsApi.Model.LAMS.Work.GetMachineFirstRecord(machine.NascaPlantCd, typeCd);
                if (oneBeforeLotWork == null)
                {
                    throw new ApplicationException($"[算出不可] 過去ロットLAMS実績無し");
                }

                if (changeMateriaList.Count() != 0)
                {
                    // 金線の交換を行っている場合、金線使用数は0で計算、起点日時(startPointDt)も交換した日時に変更
                    oneBeforeLotGoldWireCt = 0;
                    startPointDt = changeMateriaList
                        .OrderByDescending(m => m.InputDt).FirstOrDefault().InputDt.Value;
                }
                else
                {
                    //oneBeforeLotGoldWireCt = ArmsApi.Model.EICS.MachineLog.GetNumericValue(machine.NascaPlantCd, qcParamNo, oneBeforeLotWork.LotNo);
                    oneBeforeLotGoldWireCt = ArmsApi.Model.EICS.MachineLog.GetGoldWireUseCount(machineLogOutputPath, oneBeforeLotWork.LotEndDt, oneBeforeLotWork.LotNo);
                    if (oneBeforeLotGoldWireCt.HasValue == false)
                    {
                        throw new ApplicationException($"[算出不可] 過去ロット:{oneBeforeLotWork.LotNo} 打点数取得失敗");
                    }
                }

                AsmLot lot = AsmLot.GetAsmLot(oneBeforeLotWork.LotNo);
                int defectCt = Defect.GetDefectCountOfPassedProcess(lot, procNo);

                // 先頭の投入中ロットで装置停止した時間を取得
                double stopHour = ArmsApi.Model.LAMS.HeartBeat.GetMachineStopHour(machine.NascaPlantCd, startDt, System.DateTime.Now);

                decimal pcsUseCt = lotGoldWireCt / (oneBeforeLotWork.LotSizeCt - defectCt);
                decimal remainingCt = limit.Value - oneBeforeLotGoldWireCt.Value;
                decimal remainingPcs = remainingCt / pcsUseCt;
                decimal remainingHour = remainingPcs / (decimal)uph;
                return startPointDt.AddHours((double)remainingHour + stopHour).ToString();
            }
            catch (ApplicationException err)
            {
                return err.Message;
            }
            catch (Exception)
            {
                throw;
            }



            //            try
            //            {
            //                if (Properties.Settings.Default.GoldWireChangePlanTimeVisible == false)
            //                    return null;

            //                DateTime inputDt = wireList.FirstOrDefault().InputDt.Value;
            //                DateTime stockUpdDt = wireList.FirstOrDefault().StockLastUpdDt.Value;
            //                if (inputDt < stockUpdDt)
            //                {
            //                    inputDt = stockUpdDt;
            //                }

            //                double usedWireCt = 0;
            //                List<Material> calcWireList = wireList.ToList();

            //                decimal lotUseCt = goldWireUseCtFromMpFile(machine, typeCd, "MATE016", procNo, machineLogPath);

            //                Order order = Order.GetOrderOneBeforeRecord(machine.MacNo, System.DateTime.Now, inputDt);
            //                if (order != null)
            //                {
            //                    // 資材割付～現在までに完了したロットが有る(前ロットの使用在庫数を現在庫数に反映させるため更新)

            //                    //string dateFolderName = Path.Combine(order.WorkEndDt.Value.ToString("yyyyMM"), "Bind");

            //                    //string[] lotDir = Directory.GetDirectories(Path.Combine(machineLogOutputPath, dateFolderName), order.NascaLotNo);
            //                    //if (lotDir.Count() == 0)
            //                    //{
            //                    //    usedWireCt = lotSizeCt * workOrderList.Count() * 1;
            //                    //}
            //                    //else
            //                    //{
            //                        //string[] file = DirectoryHelper.GetFiles(lotDir.FirstOrDefault(), "^MP.*$");
            //                        //if (file.Count() == 0)
            //                        //{
            //                        //    usedWireCt = lotSizeCt * workOrderList.Count() * 1;
            //                        //}

            //                        //string[] fileLines = File.ReadAllLines(file.FirstOrDefault(), Encoding.Default);
            //                        //IEnumerable<string> line = fileLines.Where(f => Regex.IsMatch(f, @"^.*ワイヤ消費量.*$"));
            //                        //if (line.Count() == 0)
            //                        //    throw new ApplicationException("[算出不可] ファイル内容「ワイヤ消費量」取得失敗");

            //                        //if (double.TryParse(line.FirstOrDefault().Split(',')[4], out usedWireCt) == false)
            //                        //    throw new ApplicationException("[算出不可] ファイル内容「ワイヤ消費量」数値取得失敗");
            //                    //}

            //                    // 金線在庫から使用数を差し引いた更新用ﾁｯﾌﾟ在庫リスト作成
            //                    List<Material> updWireList = new List<Material>();
            //                    for (int i = 0; i < wireList.Count(); i++)
            //                    {
            //                        Material m = new Material();
            //                        m.MaterialCd = wireList[i].MaterialCd;
            //                        m.LotNo = wireList[i].LotNo;
            //                        m.StockCt = wireList[i].StockCt - (decimal)usedWireCt;
            //                        if (m.StockCt < 0) m.StockCt = 0;

            //                        usedWireCt = usedWireCt - (int)wireList[i].StockCt;
            //                        if (usedWireCt < 0) usedWireCt = 0;

            //                        updWireList.Add(m);
            //                    }
            //#if DEBUG
            //                    foreach (Material wire in updWireList)
            //                    {
            //                        Material.UpdateStockCount(wire.MaterialCd, wire.LotNo, wire.StockCt);
            //                    }
            //#endif
            //                    calcWireList = updWireList;
            //                }

            //                // 先頭の投入中ロットで装置停止した時間を取得
            //                double stopHour = ArmsApi.Model.LAMS.HeartBeat.GetMachineStopHour(machine.NascaPlantCd, startDt, System.DateTime.Now);

            //                ArmsApi.Model.EICS.Bonding bondSetting = ArmsApi.Model.EICS.Bonding.GetSetting(typeCd, procNo);
            //                if (bondSetting == null)
            //                    throw new ApplicationException($"[算出不可] 型番:{typeCd} 作業:{procNo} ﾁｯﾌﾟ搭載数ﾏｽﾀ無し");

            //                TimeSpan firstLotProgressSpan = DateTime.Now - startDt;
            //                double firstLotWorkHour = firstLotProgressSpan.TotalHours - stopHour;

            //                usedWireCt = uph * firstLotWorkHour * bondSetting.ChipBondCt;
            //                double nowWireCt = (double)calcWireList.Sum(w => w.StockCt) - usedWireCt;
            //                if (nowWireCt < 0) nowWireCt = 0;

            //                if (nowWireCt == 0)
            //                    return System.DateTime.Now.ToString();

            //                double outOfStockHour = nowWireCt / (uph * bondSetting.ChipBondCt);
            //                return System.DateTime.Now.AddHours(outOfStockHour).ToString();
            //            }
            //            catch (ApplicationException err)
            //            {
            //                return err.Message;
            //            }
            //            catch (Exception)
            //            {
            //                throw;
            //            }
        }

#endregion

        /// <summary>
        /// 指定された資材リスト内で一番短い有効期限を返す
        /// </summary>
        /// <param name="materialList"></param>
        /// <param name="machineInfo"></param>
        /// <returns></returns>
        private DateTime? getMaterialFirstLimitDt(Material[] materialList, MachineInfo machineInfo)
        {
            DateTime? retv = null;

            foreach (Material mat in materialList)
            {
                string typeCd = MachineInfo.GetCurrentEICSTypeCd(machineInfo);

                if (mat.IsWafer == true || mat.IsFrame == true) continue;
                if (mat.LimitDt >= DateTime.Parse("9000/1/1")) continue;

                // 原材料使用期限
                DateTime limitdt = mat.LimitDt;

                // タイプ名が取得できた場合は、解凍・開封有効期限を使用
                if (!string.IsNullOrEmpty(typeCd))
                {
                    DateTime dbResinLimit = Material.GetDBResinLimitDt(typeCd, mat.MaterialCd, mat.LotNo, mat.InputDt.Value, true);
                    if (limitdt >= dbResinLimit) limitdt = dbResinLimit;
                }

                if (retv == null || retv > limitdt)
                {
                    retv = limitdt;
                }
            }

            return retv;
        }

        /// <summary>
        /// 過去ロットの実績(装置、型番グループが同じ)から1ロットの打点数を取得する
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="typeCd"></param>
        /// <param name="qcParamNo"></param>
        private decimal lotHitCountFromLamsRecord(MachineInfo machine, string typeCd, int qcParamNo, string matGroupCd, int procNo)
        {
            ArmsApi.Model.LAMS.Work oneBeforeWork = null; ArmsApi.Model.LAMS.Work twoBeforeWork = null;
            decimal? oneBeforeHitCt = null; decimal? twoBeforeHitCt = null;

            // 1ロットの打点数算出の為、設備、まとめ型番の同じ過去2ロットを取得 (※条件に一致するロットが見つからなかった場合の遡る限界時間は過去24hr)
            DateTime recordDt = System.DateTime.Now;
            while (recordDt >= System.DateTime.Now.AddHours(-24))
            {
                List<ArmsApi.Model.LAMS.Work> list = ArmsApi.Model.LAMS.Work.GetMachineRecord(machine.NascaPlantCd, typeCd, 2, recordDt);

                if (list.Count() < 2)
                    // 過去ロットが2件以上無い場合は算出不可
                    throw new ApplicationException("[算出不可] 過去ロット2件以上無し");

                oneBeforeWork = list[0];
                twoBeforeWork = list[1];

                List<ArmsApi.Model.LAMS.Work> exists = ArmsApi.Model.LAMS.Work.GetMachineRecord(machine.NascaPlantCd, oneBeforeWork.LotStartDt, twoBeforeWork.LotEndDt);
                if (exists.Count != 0)
                {
                    // ロット間で違う型番ロットの実績有り　1ロットの使用数算出不可の為、さらに過去に遡って連続ロットを探す
                    recordDt = list[0].LotStartDt;
                    continue;
                }

                // 投入中ﾛｯﾄで資材交換があるか調べる
                Material[] changeMateriaList = machine.GetMaterials(twoBeforeWork.LotStartDt, oneBeforeWork.LotEndDt)
                    ?.Where(m => m.InputDt >= twoBeforeWork.LotStartDt && m.InputDt <= oneBeforeWork.LotEndDt)
                    ?.Where(m => m.HMGroup == matGroupCd).ToArray();
                if (changeMateriaList.Count() != 0)
                {
                    // ロット間で資材交換している場合、1ロットの使用数算出不可の為さらに過去に遡って連続ロットを探す
                    recordDt = list[0].LotStartDt;
                    continue;
                }

                int lineNo = MachineInfo.GetEICSLineNo(machine.NascaPlantCd);

                oneBeforeHitCt = ArmsApi.Model.EICS.MachineLog.GetNumericValue(lineNo, machine.NascaPlantCd, qcParamNo, oneBeforeWork.LotNo);
                twoBeforeHitCt = ArmsApi.Model.EICS.MachineLog.GetNumericValue(lineNo, machine.NascaPlantCd, qcParamNo, twoBeforeWork.LotNo);

                if (oneBeforeHitCt.HasValue == false && twoBeforeHitCt.HasValue == true)
                {
                    // EICSログから打点数が取得できない　1ロットの使用数算出不可の為、さらに過去に遡って連続ロットを探す
                    recordDt = list[0].LotStartDt;
                    continue;
                }
                else if (twoBeforeHitCt.HasValue == false)
                {
                    // EICSログから打点数が取得できない　1ロットの使用数算出不可の為、さらに過去に遡って連続ロットを探す
                    recordDt = list[1].LotStartDt;
                    continue;
                }
                
                break;
            }

            decimal? lotHitCt = 0;
            if (oneBeforeHitCt.HasValue == true && twoBeforeHitCt.HasValue == true)
            {
                lotHitCt = oneBeforeHitCt.Value - twoBeforeHitCt.Value;
            }
            else
            {
                lotHitCt = Lot.GetUseCount(machine.MacNo, procNo, typeCd, matGroupCd);
            }

            if (lotHitCt.HasValue == false || lotHitCt.Value <= 0)
                throw new ApplicationException("[算出不可] 過去実績ロットから打点数取得が可能なデータが取得できなかった為");

            Lot.UpdateUseCount(machine.MacNo, procNo, typeCd, matGroupCd, lotHitCt.Value);
            return lotHitCt.Value;
        }

        private decimal goldWireUseCtFromMpFile(MachineInfo machine, string typeCd, string matGroupCd, int procNo, string machineLogPath)
        {
            ArmsApi.Model.LAMS.Work oneBeforeWork = null; ArmsApi.Model.LAMS.Work twoBeforeWork = null;
            decimal? oneBeforeUseCt = null; decimal? twoBeforeUseCt = null;

            // 1ロットの打点数算出の為、設備、まとめ型番の同じ過去2ロットを取得 (※条件に一致するロットが見つからなかった場合の遡る限界時間は過去24hr)
            DateTime recordDt = System.DateTime.Now;
            while (recordDt >= System.DateTime.Now.AddHours(-24))
            {
                List<ArmsApi.Model.LAMS.Work> list = ArmsApi.Model.LAMS.Work.GetMachineRecord(machine.NascaPlantCd, typeCd, 2, recordDt);

                if (list.Count() < 2)
                    // 過去ロットが2件以上無い場合は算出不可
                    throw new ApplicationException("[算出不可] 過去実績ロット2件以上無し");

                oneBeforeWork = list[0];
                twoBeforeWork = list[1];

                List<ArmsApi.Model.LAMS.Work> exists = ArmsApi.Model.LAMS.Work.GetMachineRecord(machine.NascaPlantCd, oneBeforeWork.LotStartDt, twoBeforeWork.LotEndDt);
                if (exists.Count != 0)
                {
                    // ロット間で違う型番ロットの実績有り　1ロットの使用数算出不可の為、さらに過去に遡って連続ロットを探す
                    recordDt = list[0].LotStartDt;
                    continue;
                }

                // 投入中ﾛｯﾄで資材交換があるか調べる
                Material[] changeMateriaList = machine.GetMaterials(twoBeforeWork.LotStartDt, oneBeforeWork.LotEndDt)
                    ?.Where(m => m.InputDt >= twoBeforeWork.LotStartDt && m.InputDt <= oneBeforeWork.LotEndDt)
                    ?.Where(m => m.HMGroup == matGroupCd).ToArray();
                if (changeMateriaList.Count() != 0)
                {
                    // ロット間で資材交換している場合、1ロットの使用数算出不可の為さらに過去に遡って連続ロットを探す
                    recordDt = list[0].LotStartDt;
                    continue;
                }

                oneBeforeUseCt = ArmsApi.Model.EICS.MachineLog.GetGoldWireUseCount(machineLogPath, oneBeforeWork.LotEndDt, oneBeforeWork.LotNo);
                twoBeforeUseCt = ArmsApi.Model.EICS.MachineLog.GetGoldWireUseCount(machineLogPath, twoBeforeWork.LotEndDt, twoBeforeWork.LotNo);

                if (oneBeforeUseCt.HasValue == false && twoBeforeUseCt.HasValue == true)
                {
                    // EICSログから打点数が取得できない　1ロットの使用数算出不可の為、さらに過去に遡って連続ロットを探す
                    recordDt = list[0].LotStartDt;
                    continue;
                }
                else if (twoBeforeUseCt.HasValue == false)
                {
                    // EICSログから打点数が取得できない　1ロットの使用数算出不可の為、さらに過去に遡って連続ロットを探す
                    recordDt = list[1].LotStartDt;
                    continue;
                }

                break;
            }

            decimal? lotUseCt = 0;
            if (oneBeforeUseCt.HasValue == true && twoBeforeUseCt.HasValue == true)
            {
                lotUseCt = oneBeforeUseCt.Value - twoBeforeUseCt.Value;
            }
            else
            {
                lotUseCt = Lot.GetUseCount(machine.MacNo, procNo, typeCd, matGroupCd);
            }

            if (lotUseCt.HasValue == false || lotUseCt.Value <= 0)
                throw new ApplicationException("[算出不可] 装置ログ内「ワイヤ消費量」不正");

            Lot.UpdateUseCount(machine.MacNo, procNo, typeCd, matGroupCd, lotUseCt.Value);
            return lotUseCt.Value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        /// <summary>
        /// 無人時間帯対象か調べる
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool HasUnattendedTimeZone(string time)
        {
            if (string.IsNullOrEmpty(time) || time == "-" || time.Contains("算出不可"))
            {
                return false;
            }

            DateTime date = Convert.ToDateTime(time);

            if (unattendedTimeZoneList.Where(l => l.StartDt <= date && l.EndDt >= date).Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 使用した打点数を取得するための傾向管理番号を取得
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private int GetUseHitCtQcParamNo(int procNo, string stampCd)
        {
            List<UseHitCtQcparamNo> list = useHitCtQcparamNoList.Where(l => l.ProcNo == procNo).ToList();
            if (list.Count == 0)
                throw new ApplicationException($"設定ファイルに工程No:{procNo}の管理場号無し");

            if (string.IsNullOrEmpty(stampCd) == false)
            {
                list = list.Where(l => l.StampCd == stampCd).ToList();
            }

            return list.FirstOrDefault().QcParamNo;
        }

        private int GetUseHitCtQcParamNo(int procNo)
        {
            return GetUseHitCtQcParamNo(procNo);
        }
    }
}
