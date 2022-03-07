using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using WowFisher.Bot;
using static Vanara.PInvoke.User32;

namespace WowFisher.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var wow = WowProcess.GetWowProcesses();
            wow[0].KeyPress(ConsoleKey.D1);
            wow[0].KeyPress(ConsoleKey.D2);
            wow[0].KeyPress(ConsoleKey.D3);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var wow = WowProcess.GetWowProcesses();
            wow[0].MouseRightClick(new System.Drawing.Point(0, 0));

        }
    }
}
