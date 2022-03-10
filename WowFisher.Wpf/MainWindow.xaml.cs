using System;
using System.Windows;
using WowFisher.Bot;

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
            WowProcess.GetWowProcesses()[0].MouseRightClick(new System.Drawing.Point(
                (int)(330 * 65535 / SystemParameters.VirtualScreenWidth),
                (int)(330 * 65535 / SystemParameters.VirtualScreenHeight)));
        }

        Fisher botTask = new(WowProcess.GetWowProcesses()[0]);
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            botTask.Start();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            botTask.Stop();
        }
    }
}
