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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArmsWorkTransparency
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnNextMachineMonitor_Click(object sender, RoutedEventArgs e)
        {
            V01_MachineMonitor window = new V01_MachineMonitor();
            window.Show();
        }

        private void btnNextLotMonitor_Click(object sender, RoutedEventArgs e)
        {
            V02_LotMonitor window = new V02_LotMonitor();
            window.Show();
        }

    }
}
