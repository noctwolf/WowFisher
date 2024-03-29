﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WowFisher.Wpf.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.VisualAll().OfType<Button>().Where(f => (string)f.Content == "开始").ToList().ForEach(f => f.OnClick());
        }
    }
}
