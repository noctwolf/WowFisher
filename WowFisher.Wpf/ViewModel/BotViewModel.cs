using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.CodeGenerators;
using WowFisher.Bot;

namespace WowFisher.Wpf.ViewModel
{
    [GenerateViewModel]
    public partial class BotViewModel : IDisposable
    {
        public Process Process { get; }

        public BotViewModel(Process process)
        {
            Process = process;
            fisher = new(process);
            fisher.Bobber += Fisher_Bobber;
        }

        private void Fisher_Bobber(object sender, BobberEventArgs e)
        {
            Graphics.FromImage(e.Image).DrawRectangle(new Pen(Color.White, 2), e.Location.X - 10, e.Location.Y - 10, 21, 21);
            ImageSource = e.Image.ToBitmapImage();
        }

        private readonly Fisher fisher;

        [GenerateProperty(SetterAccessModifier = AccessModifier.Private)]
        private System.Windows.Media.ImageSource imageSource;

        [GenerateCommand]
        async Task StartAsync() => await fisher.StartAsync(StartAsyncCommand.CancellationTokenSource);

        public void Dispose() => StartAsyncCommand.CancelCommand.Execute(null);
    }
}
