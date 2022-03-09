using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WowFisher.Bot
{
    public class BotTask
    {
        private CancellationTokenSource cts;

        public BotTask(Process process)
        {
            Process = process;
            ColorSerial = new(process.IsWowClassic());
        }

        public ColorSerial ColorSerial { get; }

        public Process Process { get; }

        public bool IsRunning { get; private set; }

        public async void Start()
        {
            Debug.WriteLine($"Start{Thread.CurrentThread.ManagedThreadId}");
            Debug.Assert(!IsRunning);
            IsRunning = true;
            try
            {
                cts = new CancellationTokenSource();
                Process.KeyPress(ConsoleKey.D1);//Rod
                while (true)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    await Task.Run(StartCore);
                    await Task.Delay(1000, cts.Token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsRunning = false;
            }
        }

        private async Task StartCore()
        {
            Debug.WriteLine($"StartCore{Thread.CurrentThread.ManagedThreadId}");
            cts.Token.ThrowIfCancellationRequested();
            await Lure();
            Process.GetBitmap();
        }

        private async Task Lure()
        {
            Debug.WriteLine($"Lure{Thread.CurrentThread.ManagedThreadId}");
            Process.KeyPress(ConsoleKey.D2);
            await Task.Delay(250);
            Process.KeyPress(ConsoleKey.D3);
            await Task.Delay(15000, cts.Token);
        }

        public void Stop()
        {
            cts?.Cancel();
        }
    }
}
