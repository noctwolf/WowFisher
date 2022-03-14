using DevExpress.Mvvm.CodeGenerators;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using WowFisher.Bot;

namespace WowFisher.Wpf.ViewModel
{
    [GenerateViewModel]
    public partial class MainViewModel
    {
        Process process = WowProcess.GetWowProcesses()[0];

        Fisher fisher;

        [GenerateProperty(SetterAccessModifier = AccessModifier.Private)]
        System.Windows.Media.ImageSource imageSource;

        [GenerateCommand]
        async Task StartAsync() => await fisher.StartAsync(StartAsyncCommand.CancellationTokenSource);

        public MainViewModel()
        {
            fisher = new(process);
            fisher.Bobber += Fisher_Bobber;
        }

        private void Fisher_Bobber(object sender, BobberEventArgs e)
        {
            Graphics.FromImage(e.Image).DrawRectangle(new Pen(Color.White, 2), e.Location.X - 10, e.Location.Y - 10, 21, 21);
            ImageSource = e.Image.ToBitmapImage();
        }
    }
}
