using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WowFisher.Bot
{
    public class Fisher
    {
        private CancellationTokenSource cts;
        private DateTime lureTime;
        private readonly TimeSpan lureDuration = TimeSpan.FromMinutes(10);

        public event EventHandler<BobberEventArgs> Bobber;

        public Fisher(Process process)
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
                Process.KeyPress(ConsoleKey.D5);//Rod
                while (true)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    await StartCore();
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
            await Cast();
            await Observe();
        }

        private async Task Lure()
        {
            Debug.WriteLine($"Lure{Thread.CurrentThread.ManagedThreadId}");
            if (DateTime.Now - lureTime > lureDuration)
            {
                Debug.WriteLine($"Lure");
                lureTime = DateTime.Now;
                Process.KeyPress(ConsoleKey.D4);//Lure
                await Task.Delay(250);
                Process.KeyPress(ConsoleKey.D5);//Rod
                await Task.Delay(15000, cts.Token);
            }
        }

        private async Task Cast()
        {
            Debug.WriteLine($"Cast{Thread.CurrentThread.ManagedThreadId}");
            Process.KeyPress(ConsoleKey.D1);
            await Task.Delay(250);
        }

        private async Task Observe()
        {
            Debug.WriteLine($"Observe{Thread.CurrentThread.ManagedThreadId}");
            Bitmap bitmap = Process.GetBitmap();
            var bobber = GetBobber(bitmap);
            Bobber?.Invoke(this, new BobberEventArgs { Image = bitmap, Location = bobber });
            await Task.Delay(0);
        }

        private Point GetBobber(Bitmap bitmap)
        {
            var points = Enumerable.Range(0, bitmap.Height)
                .SelectMany(f => Enumerable.Range(0, bitmap.Width), (y, x) => new Point(x, y))
                .ToList();

            points = points.Where(f =>
            {
                bool isMatch = ColorSerial.IsMatch(bitmap.GetPixel(f.X, f.Y));
                if (isMatch) bitmap.SetPixel(f.X, f.Y, ColorSerial.IsRed ? Color.Red : Color.Blue);
                return isMatch;
            }).ToList();

            var p = points.SelectMany(f => points, (p1, p2) => new { p1, p2 })
                .Where(f => Math.Abs(f.p1.X - f.p2.X) < 10 && Math.Abs(f.p1.Y - f.p2.Y) < 10)
                .GroupBy(f => f.p1).OrderByDescending(f => f.Count()).Select(f => new { f.Key, Count = f.Count() }).ToList();
            points = p.Select(f => f.Key).ToList();
            Debug.WriteLine($"Bobber：{points.First()}");
            return points.First();
        }

        public void Stop() => cts?.Cancel();
    }

    public class BobberEventArgs : EventArgs
    {
        public Bitmap Image { get; set; }

        public Point Location { get; set; }
    }
}
