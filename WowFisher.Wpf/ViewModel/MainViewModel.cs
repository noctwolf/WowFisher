using DevExpress.Mvvm.CodeGenerators;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using WowFisher.Bot;

namespace WowFisher.Wpf.ViewModel
{
    [GenerateViewModel]
    public partial class MainViewModel
    {
        private ObservableCollection<Process> processes = new();

        [GenerateProperty(SetterAccessModifier = AccessModifier.Private)]
        private ObservableCollection<BotViewModel> botViewModelList = new();

        public MainViewModel() => Refresh();

        [GenerateCommand]
        private void Refresh()
        {
            var newlist = WowProcess.GetWowProcesses().ToList();
            botViewModelList.Select(f => f.Process.Id).Except(newlist.Select(f => f.Id)).ToList()
                .ForEach(f => botViewModelList.Remove(botViewModelList.Single(ff => ff.Process.Id == f)));
            newlist.Select(f => f.Id).Except(botViewModelList.Select(f => f.Process.Id)).ToList()
                .ForEach(f => botViewModelList.Add(new BotViewModel(newlist.Single(ff => ff.Id == f))));
        }
    }
}
