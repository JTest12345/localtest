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
using ArmsApi.Model;
using System.Windows.Threading;
using ArmsWorkTransparency.Views;

namespace ArmsWorkTransparency
{
    /// <summary>
    /// W01_MachineMonitor.xaml の相互作用ロジック
    /// </summary>
    public partial class V01_MachineMonitor : Window
    {

        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        ViewModels.MachineMonitorViewModel model = null;

        public V01_MachineMonitor()
        {
            InitializeComponent();           

            model = new ViewModels.MachineMonitorViewModel();
            this.DataContext = model;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.TabletMode)
            {
                // タブレットモード時は
                // 実行周期(1.2秒)、1回の処理時間(500mmsec) を想定
                timer.Interval = new TimeSpan(0, 0, 0, 1, 200);

                rtSavingPeople.Visibility = Visibility.Hidden;
                lbSavingPeople.Visibility = Visibility.Hidden;

                rtExclamation.Visibility = Visibility.Hidden;
                lbExclamation.Visibility = Visibility.Hidden;

                rtWarning.Visibility = Visibility.Hidden;
                lbWarning.Visibility = Visibility.Hidden;

                lbExclamation.Visibility = Visibility.Hidden;

                lbCautionMagazineFreeCt.Visibility = Visibility.Hidden;
                lbWarningMagazineFreeCt.Visibility = Visibility.Hidden;

                rtExclamation.Visibility = Visibility.Visible;
                lbStop.Visibility = Visibility.Visible;
            }
            else
            {
                timer.Interval = new TimeSpan(0, Properties.Settings.Default.MachineMonitorUpdateMinutes, 0);

                rtSavingPeople.Visibility = Visibility.Visible;
                lbSavingPeople.Visibility = Visibility.Visible;

                rtExclamation.Visibility = Visibility.Visible;
                lbExclamation.Visibility = Visibility.Visible;

                rtWarning.Visibility = Visibility.Visible;
                lbWarning.Visibility = Visibility.Visible;

                lbExclamation.Visibility = Visibility.Visible;

                lbCautionMagazineFreeCt.Visibility = Visibility.Visible;
                lbWarningMagazineFreeCt.Visibility = Visibility.Visible;

                //rtExclamation.Visibility = Visibility.Hidden;
                lbStop.Visibility = Visibility.Hidden;
            }

            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

            #region 表示列設定
            for (int j = 0; j < grdItems.Columns.Count; j++)
            {
                switch (grdItems.Columns[j].Header.ToString())
                {
                    case "空数":
                        if (Properties.Settings.Default.LoaderMagazineFreeCountVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "ﾏｶﾞｼﾞﾝ投入予測":
                        if (Properties.Settings.Default.LoadMagazinePlanTimeVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "ﾁｯﾌﾟ交換予測":
                        if (Properties.Settings.Default.ChipChangePlanTimeVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "ﾁｯﾌﾟ数量":
                        if (Properties.Settings.Default.ChipCtVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "DB樹脂交換予測":
                        if (Properties.Settings.Default.DbResinChangePlanTimeVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "右ｽﾀﾝﾌﾟﾋﾟﾝ交換予測":
                    case "左ｽﾀﾝﾌﾟﾋﾟﾝ交換予測":
                        if (Properties.Settings.Default.StampPinChangePlanTimeVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "ｽﾀﾝﾌﾟﾋﾟﾝ交換予測":
                        if (Properties.Settings.Default.StampPinChangePlanTimeVisible2 == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "ｺﾚｯﾄ交換予測":
                        if (Properties.Settings.Default.EjectPinChangePlanTimeVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "ｲｼﾞｪｸﾄﾋﾟﾝ交換予測":
                        if (Properties.Settings.Default.EjectPinChangePlanTimeVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "金線交換予測":
                        if (Properties.Settings.Default.GoldWireChangePlanTimeVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "ｷｬﾋﾟﾗﾘ交換予測":
                        if (Properties.Settings.Default.CapillaryChangePlanTimeVisible == false)
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

                    case "資材有効期限":
                        if (Properties.Settings.Default.MaterialLimitVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "樹脂ｸﾞﾙｰﾌﾟ":
                        if (Properties.Settings.Default.ResinGroupVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "狙いﾗﾝｸ":
                        if (Properties.Settings.Default.AimRankVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "蛍光体ﾌﾞﾛｯｸ":
                        if (Properties.Settings.Default.PhosphorSheetMatCdVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                    case "蛍光体ﾌﾞﾛｯｸ樹脂ｸﾞﾙｰﾌﾟ":
                        if (Properties.Settings.Default.PhosphorSheetResinGroupCdVisible == false)
                            grdItems.Columns[j].Visibility = Visibility.Hidden;
                        break;

                }
            }
            #endregion

            lbCautionMagazineFreeCt.Content = Properties.Settings.Default.CautionMagazineFreeCt + "マガジン以上";
            lbWarningMagazineFreeCt.Content = Properties.Settings.Default.WarningMagazineFreeCt + "マガジン以上";

            grdItems.Visibility = Visibility.Hidden; 
        }

        void timer_Tick(object sender, EventArgs e)
        {
            model.Update(this);
            grdItems.Visibility = Visibility.Visible;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            timer.Stop();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            grdItems.Visibility = Visibility.Visible;
        }

        private void btnCondition_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();

            V03_MachineMonitorSetting machineMonitorSetting = new V03_MachineMonitorSetting();
            machineMonitorSetting.ShowDialog();

            timer.Start();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Hyperlink link = (Hyperlink)e.OriginalSource;

                MachineInfo.Eics eicsMachine = ArmsApi.Model.MachineInfo.Eics.GetSetting(link.NavigateUri.OriginalString);
                if (eicsMachine == null)
                    throw new Exception($"IPAddressの設定がEICS設備マスタに存在しません。設備CD:{link.NavigateUri.OriginalString}");

                //if
                //    string vncString = @"C:\Program Files\UltraVNC\vncviewer.ex

                System.Diagnostics.Process.Start(@"C:\Program Files\UltraVNC\vncviewer.exe", $"/server {eicsMachine.IPAddressNo} /password 0");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
