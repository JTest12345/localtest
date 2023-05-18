using ArmsApi.Model;
using ArmsWorkTransparency.Model.PLC;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArmsWorkTransparency.ViewModels
{
	public class MachineMonitorSettingViewModel : INotifyPropertyChanged
    {
        #region プロパティ

        public System.Collections.IList MachineSelectedList { get; set; }
        public int UpdateInterval { get; set; }
        public bool SupplyIdVisible { get; set; }
        public bool BlendCdVisible { get; set; }
        public bool ChipCtVisible { get; set; }
        public bool LoadMagazinePlanTimeVisible { get; set; }
        public bool LoaderMagazineFreeCountVisible { get; set; }
        public bool ChipChangePlanTimeVisible { get; set; }
        public bool StampPinChangePlanTimeVisible { get; set; }
        public bool ColletChangePlanTimeVisible { get; set; }
        public bool EjectPinChangePlanTimeVisible { get; set; }
        public bool DbResinChangePlanTimeVisible { get; set; }
        public bool GoldWireChangePlanTimeVisible { get; set; }
        public bool CapillaryChangePlanTimeVisible { get; set; }
        public int CautionMagazineFreeCt { get; set; }
        public int WarningMagazineFreeCt { get; set; }
        public bool UseMachineReady { get; set; }
        public bool TabletMode { get; set; }
        public bool MaterialLimitVisible { get; set; }
        public bool ResinGroupVisible { get; set; }
        public bool StampPinChangePlanTimeVisible2 { get; set; }
        public bool AimRankVisible { get; set; }
        public bool PhosphorSheetMatCdVisible { get; set; }
        public bool PhosphorSheetResinGroupCdVisible { get; set; }

        #endregion

        public class Machine
        {
            public int No { get; set; }
            public string Name { get; set; }
            public bool IsSelected { get; set; }

            public Machine(int no, string name, bool isSelected)
            {
                this.No = no;
                this.Name = name;
                this.IsSelected = isSelected;
            }
        }

        private List<Machine> machineList;
        public List<Machine> MachineList
        {
            get { return machineList = machineList ?? new List<Machine>(); }
            set
            {
                machineList = value;
                NotifyPropertyChanged(nameof(MachineList));
            }
        }

        private List<Machine> searchMachineList;
        public List<Machine> SearchMachineList
        {
            get { return searchMachineList = searchMachineList ?? new List<Machine>(); }
            set
            {
                searchMachineList = value;
                NotifyPropertyChanged(nameof(SearchMachineList));
            }
        }

        public void LoadSetting()
        {
            LoadMachineList();
           
        }

        private void LoadMachineList()
        {
            IEnumerable<MachineInfo> machines = MachineInfo.GetMachineList(true).Where(m => m.NascaPlantCd != "-")?.OrderBy(m => m.MacNo);

            searchMachineList = new List<Machine>();

            foreach (MachineInfo machine in machines)
            {
                searchMachineList.Add(new Machine(machine.MacNo, machine.LongName, false));
            }
        }

        //public void SaveSetting(System.Collections.IList machineSelectedList, string updateInterval,
        //    bool supplyIdVisible, bool blendCdVisible, bool ChipCtVisible,
        //    bool loadMagazinePlanTimeVisible, bool loaderMagazineFreeCountVisible,
        //    bool chipChangePlanTimeVisible, bool stampPinChangePlanTimeVisible, bool colletChangePlanTimeVisible, bool ejectPinChangePlanTimeVisible, bool dbResinChangePlanTimeVisible,
        //    bool goldWireChangePlanTimeVisible, bool capillaryChangePlanTimeVisible,
        //    string warningMagazineFreeCt, string cautionMagazineFreeCt,
        //    bool useMachineReady, bool tabletMode, bool materialLimitVisible, bool resinGroupVisible,
        //    bool stampPinChangePlanTimeVisible2)
        public void SaveSetting()
        {
            List<string> machineList = new List<string>();
            StringCollection sc = new StringCollection();

            //foreach (Machine currentMachine in machineSelectedList)
            foreach (Machine currentMachine in this.MachineSelectedList)
            {
                sc.Add(currentMachine.No.ToString());
            }

            Properties.Settings.Default.TargetMachineList = sc;

            //Properties.Settings.Default.MachineMonitorUpdateMinutes = int.Parse(updateInterval);

            //Properties.Settings.Default.SupplyIdVisible = supplyIdVisible;
            //Properties.Settings.Default.BlendCdVisible = blendCdVisible;
            //Properties.Settings.Default.ChipCtVisible = ChipCtVisible;
            //Properties.Settings.Default.LoadMagazinePlanTimeVisible = loadMagazinePlanTimeVisible;
            //Properties.Settings.Default.LoaderMagazineFreeCountVisible = loaderMagazineFreeCountVisible;
            //Properties.Settings.Default.ChipChangePlanTimeVisible = chipChangePlanTimeVisible;
            //Properties.Settings.Default.StampPinChangePlanTimeVisible = stampPinChangePlanTimeVisible;
            //Properties.Settings.Default.ColletChangePlanTimeVisible = colletChangePlanTimeVisible;
            //Properties.Settings.Default.EjectPinChangePlanTimeVisible = ejectPinChangePlanTimeVisible;
            //Properties.Settings.Default.DbResinChangePlanTimeVisible = dbResinChangePlanTimeVisible;
            //Properties.Settings.Default.GoldWireChangePlanTimeVisible = goldWireChangePlanTimeVisible;
            //Properties.Settings.Default.CapillaryChangePlanTimeVisible = capillaryChangePlanTimeVisible;

            //Properties.Settings.Default.WarningMagazineFreeCt = int.Parse(warningMagazineFreeCt);
            //Properties.Settings.Default.CautionMagazineFreeCt = int.Parse(cautionMagazineFreeCt);
            //Properties.Settings.Default.UseMachineReady = useMachineReady;
            //Properties.Settings.Default.TabletMode = tabletMode;
            //Properties.Settings.Default.MaterialLimitVisible = materialLimitVisible;
            //Properties.Settings.Default.ResinGroupVisible = resinGroupVisible;
            //Properties.Settings.Default.StampPinChangePlanTimeVisible2 = stampPinChangePlanTimeVisible2;

            Properties.Settings.Default.MachineMonitorUpdateMinutes = this.UpdateInterval;

            Properties.Settings.Default.SupplyIdVisible = this.SupplyIdVisible;
            Properties.Settings.Default.BlendCdVisible = this.BlendCdVisible;
            Properties.Settings.Default.ChipCtVisible = this.ChipCtVisible;
            Properties.Settings.Default.LoadMagazinePlanTimeVisible = this.LoadMagazinePlanTimeVisible;
            Properties.Settings.Default.LoaderMagazineFreeCountVisible = this.LoaderMagazineFreeCountVisible;
            Properties.Settings.Default.ChipChangePlanTimeVisible = this.ChipChangePlanTimeVisible;
            Properties.Settings.Default.StampPinChangePlanTimeVisible = this.StampPinChangePlanTimeVisible;
            Properties.Settings.Default.ColletChangePlanTimeVisible = this.ColletChangePlanTimeVisible;
            Properties.Settings.Default.EjectPinChangePlanTimeVisible = this.EjectPinChangePlanTimeVisible;
            Properties.Settings.Default.DbResinChangePlanTimeVisible = this.DbResinChangePlanTimeVisible;
            Properties.Settings.Default.GoldWireChangePlanTimeVisible = this.GoldWireChangePlanTimeVisible;
            Properties.Settings.Default.CapillaryChangePlanTimeVisible = this.CapillaryChangePlanTimeVisible;

            Properties.Settings.Default.CautionMagazineFreeCt = this.CautionMagazineFreeCt;
            Properties.Settings.Default.WarningMagazineFreeCt = this.WarningMagazineFreeCt;
            Properties.Settings.Default.UseMachineReady = this.UseMachineReady;
            Properties.Settings.Default.TabletMode = this.TabletMode;
            Properties.Settings.Default.MaterialLimitVisible = this.MaterialLimitVisible;
            Properties.Settings.Default.ResinGroupVisible = this.ResinGroupVisible;
            Properties.Settings.Default.StampPinChangePlanTimeVisible2 = this.StampPinChangePlanTimeVisible2;

            Properties.Settings.Default.AimRankVisible = this.AimRankVisible;
            Properties.Settings.Default.PhosphorSheetMatCdVisible = this.PhosphorSheetMatCdVisible;
            Properties.Settings.Default.PhosphorSheetResinGroupCdVisible = this.PhosphorSheetResinGroupCdVisible;

            Properties.Settings.Default.Save();
            MessageBox.Show("設定の保存を完了しました。");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
