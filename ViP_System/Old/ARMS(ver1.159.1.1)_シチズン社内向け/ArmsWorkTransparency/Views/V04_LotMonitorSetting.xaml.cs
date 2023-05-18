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
using ArmsApi.Model;
using System.Text.RegularExpressions;

namespace ArmsWorkTransparency.Views
{
	/// <summary>
	/// V04_LotMonitorSetting.xaml の相互作用ロジック
	/// </summary>
	public partial class V04_LotMonitorSetting : Window
	{
		public LotMonitorSettingViewModel vm { get; set; }
		public V04_LotMonitorSetting()
		{
			InitializeComponent();

			vm = new LotMonitorSettingViewModel();
			this.DataContext = vm;

			vm.LoadSetting();
		}

		private void btnSaveSetting_Click(object sender, RoutedEventArgs e)
		{
            #region SaveSetting関数の引数をプロパティ化
            vm.TypeSelectedList = this.lbType.Items;
            vm.CurrentProcSelectedList = this.lbCurrentProc.SelectedItems;
            vm.NextProcSelectedList = this.lbNextProc.SelectedItems;
            int updateinterval;
            if (int.TryParse(this.tbUpdateInterval.Text, out updateinterval) == false)
            {
                MessageBox.Show("更新時間(分)には数字を入力して下さい。");
                return;
            }
            vm.UpdateInterval = updateinterval;

            vm.ResinGroupVisible = this.cbResingroupVisible.IsChecked.Value;
            vm.SupplyIdVisible = this.cbSupplyIdVisible.IsChecked.Value;
            vm.BlendCdVisible = this.cbBlendCdVisible.IsChecked.Value;
            vm.MagazineNoVisibleVisible = this.cbMagazineNVisible.IsChecked.Value;
            vm.DBThrowDtVisible = this.cbDBThrowDtVisible.IsChecked.Value;
            vm.AimRankVisible = this.cbAimRankVisible.IsChecked.Value;
            vm.PhosphorSheetMatCdVisible = this.cbPhosphorSheetMatCdVisible.IsChecked.Value;
            vm.NextLineVisible = this.cbNextLineVisible.IsChecked.Value;
            vm.ReachingDateVisible = this.cbReachingDateVisible.IsChecked.Value;
            vm.ReachingMinutesVisible = this.cbReachingMinutesVisible.IsChecked.Value;
            #endregion

            //vm.SaveSetting(this.lbType.Items, this.lbCurrentProc.SelectedItems, this.lbNextProc.SelectedItems, this.tbUpdateInterval.Text, cbSupplyIdVisible.IsChecked.Value, cbBlendCdVisible.IsChecked.Value);
            vm.SaveSetting();
		}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbUpdateInterval.Text = Properties.Settings.Default.LotMonitorUpdateMinutes.ToString();

            if (Properties.Settings.Default.TargetTypeList != null)
            {
                foreach (string type in Properties.Settings.Default.TargetTypeList)
                {
                    lbType.Items.Add(type);
                }
            }

            cbResingroupVisible.IsChecked = Properties.Settings.Default.ResinGroupVisible;
            cbSupplyIdVisible.IsChecked = Properties.Settings.Default.SupplyIdVisible;
            cbBlendCdVisible.IsChecked = Properties.Settings.Default.BlendCdVisible;
            cbMagazineNVisible.IsChecked = Properties.Settings.Default.MagazineNoVisibleVisible;
            cbDBThrowDtVisible.IsChecked = Properties.Settings.Default.DBThrowDtVisible;
            cbAimRankVisible.IsChecked = Properties.Settings.Default.AimRankVisible;
            cbPhosphorSheetMatCdVisible.IsChecked = Properties.Settings.Default.PhosphorSheetMatCdVisible;
            cbNextLineVisible.IsChecked = Properties.Settings.Default.NextLineVisible;
            cbReachingDateVisible.IsChecked = Properties.Settings.Default.ReachingDateVisible;
            cbReachingMinutesVisible.IsChecked = Properties.Settings.Default.ReachingMinutesVisible;
        }

        private void tbSearchType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string[] typeList = Process.GetWorkFlowTypeList();
                typeList = typeList.Where(t => Regex.IsMatch(t, $"^.*{tbSearchType.Text}.*$")).ToArray();
                lbSearchType.ItemsSource = typeList;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            foreach (string type in lbSearchType.SelectedItems)
            {
                if (lbType.Items.IndexOf(type)== -1)
                {
                    lbType.Items.Add(type);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectTypeList = new List<string>();
            foreach (string type in lbType.SelectedItems)
            {
                selectTypeList.Add(type);
            }
            
            foreach (string type in selectTypeList)
            {
                lbType.Items.Remove(type);            
            }
        }

        private void btnAllSelect_Click(object sender, RoutedEventArgs e)
        {
            lbSearchType.SelectAll();
        }

        private void btnAllDeSelect_Click(object sender, RoutedEventArgs e)
        {
            lbSearchType.UnselectAll();
        }
    }
}
