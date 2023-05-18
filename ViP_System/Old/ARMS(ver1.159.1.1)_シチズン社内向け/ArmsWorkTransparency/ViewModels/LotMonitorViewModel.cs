using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ArmsWorkTransparency.Common;
using ArmsWorkTransparency.Views;
using ArmsApi.Model;
using ArmsWorkTransparency.Model;
using System.Windows.Forms;

namespace ArmsWorkTransparency.ViewModels
{
    public class LotMonitorViewModel : INotifyPropertyChanged
    {
		public class LotProgress
		{
			public string TypeCd { get; set; }
			public string LotNo { get; set; }
			public string ResinGrp { get; set; }
			public string SupplyId { get; set; }
			public string BlendCd { get; set; }
            public string MagazineNo { get; set; }
            public string DBThrowDt { get; set; }
            public string AimRank { get; set; }
            public string PhosphorSheetMatCd { get; set; }
            public string NextLine { get; set; }
            public string CurrentProcessNm { get; set; }
			public string NextProcessNm { get; set; }
			public DateTime? ScheduledArrivalDt { get; set; }
            public double ScheduledArrivalMinutes { get; set; }

            public bool IsWarning
            {
                get
                {
                    if (System.DateTime.Now > ScheduledArrivalDt)
                        return true;
                    else
                        return false;
                }
            }
            public bool IsCaution
            {
                get
                {
                    if (System.DateTime.Now.AddMinutes(60) > ScheduledArrivalDt
                        && System.DateTime.Now < ScheduledArrivalDt)
                        return true;
                    else
                        return false;
                }
            }
        }

        private List<LotProgress> lotProgList;
		public List<LotProgress> LotProgList
		{
			get
			{
				return lotProgList = lotProgList ?? new List<LotProgress>();
			}
			set
			{
				this.lotProgList = value;
				NotifyPropertyChanged(nameof(LotProgList));
			}
		}
        
		private RelayCommand settingCommand;
		public RelayCommand SettingCommand
		{
			get { return settingCommand = settingCommand ?? new RelayCommand(Setting); }
		}
        public LotMonitorViewModel()
        {           	
		}

        public void Setting(object obj)
		{
			V04_LotMonitorSetting lotMonitorSetting = new V04_LotMonitorSetting();
			lotMonitorSetting.ShowDialog();
		}

		private RelayCommand updateCommand;
		public RelayCommand UpdateCommand
		{
			get { return updateCommand = updateCommand ?? new RelayCommand(Update); }
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

        public void Update(object obj)
        {
            try
            {
                List<LotProgress> localLotProgList = new List<LotProgress>();

                IEnumerable<Magazine> mags = Magazine.GetMagazine(true)
                    .Where(m => m.LastUpdDt >= Convert.ToDateTime("2017/01/01") && m.NowCompProcess != 0);

                foreach (Magazine mag in mags)
                {
                    //if (mag.NascaLotNO != "B175NS27800")
                    //    continue;

                    // 先に現在作業を取得し、設定している現在工程以外は省く ※StringCollection.Containsは完全一致検索
                    Process currentProc = Process.GetNowProcess(mag.NascaLotNO);
                    if (Properties.Settings.Default.TargetCurrentProcessList.Contains(currentProc.ProcNo.ToString()) == false)
                        continue;

                    AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                    if (lot == null) continue;

                    Profile prof = Profile.GetProfile(lot.ProfileId);
                    if (prof == null) continue;

                    BOM[] bomList = Profile.GetBOM(lot.ProfileId);

                    Restrict[] restrict = Restrict.SearchRestrict(lot.NascaLotNo, null, true);
                    if (restrict.Count() != 0)
                        continue;

                    // 設定している型番以外は省く
                    if (Properties.Settings.Default.TargetTypeList.Contains(lot.TypeCd) == false)
                        continue;

                    //Order currentOrder = Order.GetMagazineOrder(mag.MagazineNo, currentProc.ProcNo);
                    //QRプレート対応
                    Order currentOrder = Order.GetMagazineOrder(mag.NascaLotNO, currentProc.ProcNo);
                    List<string> typeGroupCd = TypeGroup.GetGroupCode(lot.TypeCd);

                    foreach (string nextProcNo in Properties.Settings.Default.TargetNextProcessList)
                    {
                        Process nextProc = Process.GetProcess(int.Parse(nextProcNo));
                        if (nextProc == null)
                            continue;

                        VirtualMag nextProclMag = VirtualMag.GetVirtualMag((int)Station.Loader, lot.NascaLotNo, int.Parse(nextProcNo));
                        if (nextProclMag != null)
                            continue;

                        LotProgress lotProg = new LotProgress();

                        lotProg.LotNo = mag.NascaLotNO;
                        lotProg.TypeCd = lot.TypeCd;
                        lotProg.ResinGrp = string.Join(",", lot.ResinGpCd);
                        lotProg.BlendCd = lot.BlendCd;

                        lotProg.MagazineNo = mag.MagazineNo;
                        lotProg.DBThrowDt = lot.DBThrowDT;
                        lotProg.AimRank = prof.AimRank;

                        List<string> phosphorSheetBomList = bomList.Where(b => b.LotCharCd == ArmsApi.Config.PHOSPHORSHEET_LOTCHARCD).Select(b => b.MaterialCd).ToList();
                        if (phosphorSheetBomList.Count > 0)
                        {
                            lotProg.PhosphorSheetMatCd = string.Join(",", phosphorSheetBomList);
                        }
                        else
                        {
                            lotProg.PhosphorSheetMatCd = string.Empty;
                        }

                        string msg;
                        lotProg.NextLine = Magazine.GetNextLine(mag, out msg);
                                               
                        lotProg.CurrentProcessNm = currentProc.InlineProNM;
                        lotProg.NextProcessNm = nextProc.InlineProNM;

                        lotProg.SupplyId = WorkCondition.GetSupplyId(lot.BlendCd, nextProc.WorkCd);

                        typeGroupCd.Add(lot.TypeCd);
                        int? workTm = Model.MachineWork.GetWorkTime(currentProc, nextProc, typeGroupCd, lot.TypeCd);
                        if (workTm.HasValue == false)
                            continue;

                        lotProg.ScheduledArrivalDt = getScheduledArrivalDt(currentOrder, workTm.Value);
                        if (lotProg.ScheduledArrivalDt.HasValue && Properties.Settings.Default.ReachingMinutesVisible)
                        {
                            TimeSpan span = lotProg.ScheduledArrivalDt.Value - System.DateTime.Now;
                            lotProg.ScheduledArrivalMinutes = Math.Round(span.TotalMinutes, MidpointRounding.AwayFromZero);
                        }

                        localLotProgList.Add(lotProg);
                    }
                }
                
                LotProgList = localLotProgList
                    .OrderBy(l => l.TypeCd)
                    .ThenBy(l => l.ResinGrp)
                    .ThenBy(l => l.ScheduledArrivalDt).ToList();
                

                LastUpdate = System.DateTime.Now.ToString("MM/dd HH:mm");
                NextUpdate = System.DateTime.Now.AddMinutes(Properties.Settings.Default.LotMonitorUpdateMinutes).ToString("MM/dd HH:mm");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DateTime? getScheduledArrivalDt(Order o, int workTm)
        {
            if (workTm == 0)
                // 装置作業時間ﾏｽﾀが未設定か、現在工程と受け取り工程が同じ場合
                return null;

            return o.WorkStartDt.AddMinutes(workTm);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
