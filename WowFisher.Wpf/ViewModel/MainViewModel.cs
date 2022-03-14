using DevExpress.Mvvm.CodeGenerators;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media;
using WowFisher.Bot;

namespace WowFisher.Wpf.ViewModel
{
    [GenerateViewModel]
    public partial class MainViewModel
    {
        Process process = WowProcess.GetWowProcesses()[0];

        Fisher fisher;

        [GenerateProperty(SetterAccessModifier = AccessModifier.Private)]
        ImageSource imageSource;

        [GenerateCommand]
        async Task StartAsync() => await fisher.StartAsync(StartAsyncCommand.CancellationTokenSource);

        public MainViewModel()
        {
            fisher = new(process);
            fisher.Bobber += Fisher_Bobber;
        }

        private void Fisher_Bobber(object sender, BobberEventArgs e)
        {
            ImageSource = e.Image.ToBitmapImage();
        }
    }
}
