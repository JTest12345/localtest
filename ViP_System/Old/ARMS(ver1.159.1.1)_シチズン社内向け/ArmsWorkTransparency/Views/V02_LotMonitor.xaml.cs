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
using System.Windows.Threading;
using ArmsWorkTransparency.ViewModels;

namespace ArmsWorkTransparency
{
    /// <summary>
    /// W02_LotMonitor.xaml の相互作用ロジック
    /// </summary>
    public partial class V02_LotMonitor : Window
    {
		private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
		public LotMonitorViewModel vm { get; set; }
		public V02_LotMonitor()
        {
            InitializeComponent();

			timer.Interval = new TimeSpan(0, Properties.Settings.Default.LotMonitorUpdateMinutes, 0);
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();

            //if (Properties.Settings.Default.SupplyIdVisible == false)
            //    grdItems.Columns[3].Visibility = Visibility.Hidden;
            //if (Properties.Settings.Default.BlendCdVisible == false)
            //    grdItems.Columns[4].Visibility = Visibility.Hidden;
            #region 表示列設定
            for (int j = 0; j < grdItems.Columns.Count; j++)
            {
                switch (grdItems.Columns[j].Header.ToString())
                {
                    case "樹脂ｸﾞﾙｰﾌﾟ":
                        if (Properties.Settings.Default.ResinGroupVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "供給ID":
                        if (Properties.Settings.Default.SupplyIdVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "ﾌﾞﾚﾝﾄﾞCD":
                        if (Properties.Settings.Default.BlendCdVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "ﾏｶﾞｼﾞﾝ":
                        if (Properties.Settings.Default.MagazineNoVisibleVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "ﾀﾞｲﾎﾞﾝﾄﾞ投入日":
                        if (Properties.Settings.Default.DBThrowDtVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "狙いﾗﾝｸ":
                        if (Properties.Settings.Default.AimRankVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "蛍光体ﾌﾞﾛｯｸ品目":
                        if (Properties.Settings.Default.PhosphorSheetMatCdVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "次ﾗｲﾝ":
                        if (Properties.Settings.Default.NextLineVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "到達日時":
                        if (Properties.Settings.Default.ReachingDateVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "到達時間(分)":
                        if (Properties.Settings.Default.ReachingMinutesVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                }
            }
            #endregion

            vm = new LotMonitorViewModel();
			this.DataContext = vm;
		}
		void timer_Tick(object sender, EventArgs e)
		{
			vm.Update(null);
		}

        private void grdItems_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            lbSelectCt.Content = grdItems.SelectedItems.Count;
        }
    }
}
