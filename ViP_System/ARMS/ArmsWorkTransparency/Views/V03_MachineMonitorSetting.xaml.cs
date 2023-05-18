using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ArmsWorkTransparency.ViewModels;
using System.Text.RegularExpressions;
using ArmsApi.Model;
using static ArmsWorkTransparency.ViewModels.MachineMonitorSettingViewModel;

namespace ArmsWorkTransparency.Views
{
	/// <summary>
	/// V03_LotMoniterSetting.xaml の相互作用ロジック
	/// </summary>
	public partial class V03_MachineMonitorSetting : Window
	{
        private MachineMonitorSettingViewModel vm { get; set; }

        public V03_MachineMonitorSetting()
		{
			InitializeComponent();

            vm = new MachineMonitorSettingViewModel();
            this.DataContext = vm;

            vm.LoadSetting();
        }

        private void btnSaveSetting_Click(object sender, RoutedEventArgs e)
        {
            #region SaveSettingの引数をプロパティ化
            vm.MachineSelectedList = this.lbMachine.Items;
            int updateInterval;
            if (int.TryParse(this.tbUpdateInterval.Text, out updateInterval) == false)
            {
                MessageBox.Show("更新時間(分)には数字を入力して下さい。");
                return;
            }
            vm.UpdateInterval = updateInterval;
            vm.SupplyIdVisible = cbSupplyIdVisible.IsChecked.Value;
            vm.BlendCdVisible = cbBlendCdVisible.IsChecked.Value;
            vm.ChipCtVisible = cbChipCtVisible.IsChecked.Value;
            vm.LoadMagazinePlanTimeVisible = this.cbLoadMagazinePlanTimeVisible.IsChecked.Value;
            vm.LoaderMagazineFreeCountVisible = this.cbLoaderMagazineFreeCtVisible.IsChecked.Value;
            vm.ChipChangePlanTimeVisible = this.cbChipChangePlanTimeVisible.IsChecked.Value;
            vm.StampPinChangePlanTimeVisible = this.cbStampPinChangePlanTimeVisible.IsChecked.Value;
            vm.ColletChangePlanTimeVisible = this.cbColletChangePlanTimeVisible.IsChecked.Value;
            vm.EjectPinChangePlanTimeVisible = this.cbEjectPinChangePlanTimeVisible.IsChecked.Value;
            vm.DbResinChangePlanTimeVisible = this.cbDbResinChangePlanTimeVisible.IsChecked.Value;
            vm.GoldWireChangePlanTimeVisible = this.cbGoldWirePlanTimeVisible.IsChecked.Value;
            vm.CapillaryChangePlanTimeVisible = this.cbCapillaryPlanTimeVisible.IsChecked.Value;
            int cautionMagazineFreeCt;
            if (int.TryParse(tbCautionMagazineFreeCt.Text, out cautionMagazineFreeCt) == false)
            {
                MessageBox.Show("注意(個)には数字を入力して下さい。");
                return;
            }
            vm.CautionMagazineFreeCt = cautionMagazineFreeCt;
            int warningMagazineFreeCt;
            if (int.TryParse(tbWarningMagazineFreeCt.Text, out warningMagazineFreeCt) == false)
            {
                MessageBox.Show("警告(個)には数字を入力して下さい。");
                return;
            }
            vm.WarningMagazineFreeCt = warningMagazineFreeCt;
            vm.UseMachineReady = this.cbUseMachineReady.IsChecked.Value;
            vm.TabletMode = this.cbTabletMode.IsChecked.Value;
            vm.MaterialLimitVisible = this.cbMaterialLimit.IsChecked.Value;
            vm.ResinGroupVisible = this.cbResinGroupVisible.IsChecked.Value;
            vm.StampPinChangePlanTimeVisible2 = this.cbStampPinChangePlanTimeVisible2.IsChecked.Value;
            vm.AimRankVisible = this.cbAimRankVisible.IsChecked.Value;
            vm.PhosphorSheetMatCdVisible = this.cbPhosphorSheetMatCdVisible.IsChecked.Value;
            vm.PhosphorSheetResinGroupCdVisible = this.cbPhosphorSheetResinGroupCdVisible.IsChecked.Value;
            #endregion

            //vm.SaveSetting(this.lbMachine.Items, this.tbUpdateInterval.Text, cbSupplyIdVisible.IsChecked.Value, cbBlendCdVisible.IsChecked.Value,
            //    cbChipCtVisible.IsChecked.Value, this.cbLoadMagazinePlanTimeVisible.IsChecked.Value, this.cbLoaderMagazineFreeCtVisible.IsChecked.Value, this.cbChipChangePlanTimeVisible.IsChecked.Value, this.cbStampPinChangePlanTimeVisible.IsChecked.Value, this.cbColletChangePlanTimeVisible.IsChecked.Value, this.cbEjectPinChangePlanTimeVisible.IsChecked.Value, this.cbDbResinChangePlanTimeVisible.IsChecked.Value,
            //    this.cbGoldWirePlanTimeVisible.IsChecked.Value, this.cbCapillaryPlanTimeVisible.IsChecked.Value, tbWarningMagazineFreeCt.Text, tbCautionMagazineFreeCt.Text,
            //    this.cbUseMachineReady.IsChecked.Value, this.cbTabletMode.IsChecked.Value, this.cbMaterialLimit.IsChecked.Value, this.cbResinGroupVisible.IsChecked.Value,
            //    this.cbStampPinChangePlanTimeVisible2.IsChecked.Value);
            vm.SaveSetting();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbUpdateInterval.Text = Properties.Settings.Default.MachineMonitorUpdateMinutes.ToString();

            if (Properties.Settings.Default.TargetMachineList != null)
            {
                foreach (string machine in Properties.Settings.Default.TargetMachineList)
                {
                    MachineInfo mInfo = MachineInfo.GetMachine(int.Parse(machine));
                    Machine m = new Machine(mInfo.MacNo, mInfo.LongName, false);
                    lbMachine.Items.Add(m);
                }
            }

            cbSupplyIdVisible.IsChecked = Properties.Settings.Default.SupplyIdVisible;
            cbBlendCdVisible.IsChecked = Properties.Settings.Default.BlendCdVisible;
            cbChipCtVisible.IsChecked = Properties.Settings.Default.ChipCtVisible;
            cbLoaderMagazineFreeCtVisible.IsChecked = Properties.Settings.Default.LoaderMagazineFreeCountVisible;

            cbLoadMagazinePlanTimeVisible.IsChecked = Properties.Settings.Default.LoadMagazinePlanTimeVisible;
            cbLoaderMagazineFreeCtVisible.IsChecked = Properties.Settings.Default.LoaderMagazineFreeCountVisible;
            cbChipChangePlanTimeVisible.IsChecked = Properties.Settings.Default.ChipChangePlanTimeVisible;
            cbStampPinChangePlanTimeVisible.IsChecked = Properties.Settings.Default.StampPinChangePlanTimeVisible;
            cbColletChangePlanTimeVisible.IsChecked = Properties.Settings.Default.ColletChangePlanTimeVisible;
            cbEjectPinChangePlanTimeVisible.IsChecked = Properties.Settings.Default.EjectPinChangePlanTimeVisible;
            cbDbResinChangePlanTimeVisible.IsChecked = Properties.Settings.Default.DbResinChangePlanTimeVisible;
            cbGoldWirePlanTimeVisible.IsChecked = Properties.Settings.Default.GoldWireChangePlanTimeVisible;
            cbCapillaryPlanTimeVisible.IsChecked = Properties.Settings.Default.CapillaryChangePlanTimeVisible;
            cbUseMachineReady.IsChecked = Properties.Settings.Default.UseMachineReady;
            cbTabletMode.IsChecked = Properties.Settings.Default.TabletMode;
            cbMaterialLimit.IsChecked = Properties.Settings.Default.MaterialLimitVisible;
            tbCautionMagazineFreeCt.Text = Properties.Settings.Default.CautionMagazineFreeCt.ToString();
            tbWarningMagazineFreeCt.Text = Properties.Settings.Default.WarningMagazineFreeCt.ToString();
            cbResinGroupVisible.IsChecked = Properties.Settings.Default.MaterialLimitVisible;
            cbStampPinChangePlanTimeVisible2.IsChecked = Properties.Settings.Default.StampPinChangePlanTimeVisible2;

            cbAimRankVisible.IsChecked = Properties.Settings.Default.AimRankVisible;
            cbPhosphorSheetMatCdVisible.IsChecked = Properties.Settings.Default.PhosphorSheetMatCdVisible;
            cbPhosphorSheetResinGroupCdVisible.IsChecked = Properties.Settings.Default.PhosphorSheetResinGroupCdVisible;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            foreach (Machine machine in lbSearchMachine.SelectedItems)
            {
                if (lbMachine.Items.IndexOf(machine) == -1)
                {
                    lbMachine.Items.Add(machine);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<Machine> selectMachineList = new List<Machine>();
            foreach (Machine machine in lbMachine.SelectedItems)
            {
                selectMachineList.Add(machine);
            }

            foreach (Machine machine in selectMachineList)
            {
                lbMachine.Items.Remove(machine);
            }
        }

        private void btnAllSelect_Click(object sender, RoutedEventArgs e)
        {
            lbSearchMachine.SelectAll();
        }

        private void btnAllDeSelect_Click(object sender, RoutedEventArgs e)
        {
            lbSearchMachine.UnselectAll();
        }

        private void tbSearchType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                IEnumerable<MachineInfo> machineList = MachineInfo.GetMachineList(true).Where(m => m.NascaPlantCd != "-")?.OrderBy(m => m.MacNo);
                List<MachineInfo> targetMachineList = machineList.Where(t => Regex.IsMatch(string.Join(",", t.MacGroup), $"^{tbSearchMacGroup.Text}$")).ToList();
                targetMachineList.AddRange(machineList.Where(t => Regex.IsMatch(string.Join(",", t.MacGroup), $"^{tbSearchMacGroup.Text},.*$")).ToList());

                List<Machine> searchMachineList = new List<Machine>();
                foreach (MachineInfo machine in targetMachineList)
                {
                    searchMachineList.Add(new Machine(machine.MacNo, machine.LongName, false));
                }

                lbSearchMachine.ItemsSource = searchMachineList;
            }
        }

        private void btnAllSelect2_Click(object sender, RoutedEventArgs e)
        {
            lbMachine.SelectAll();
        }

        private void btnAllDeSelect2_Click(object sender, RoutedEventArgs e)
        {
            lbMachine.UnselectAll();
        }
    }
}
