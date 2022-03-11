using System;
using System.Collections.Generic;
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
        private readonly TimeSpan observeDuration = TimeSpan.FromSeconds(25);

        public event EventHandler<BobberEventArgs> Bobber;

        public Fisher(Process process)
        {
            Process = process;
            ColorSerial = new(process.IsWowClassic());
        }

        public ColorSerial ColorSerial { get; }

        public Process Process { get; }

        public bool IsRunning { get; private set; }

        public async Task StartAsync()
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
                    await Task.Run(StartCore);
                    Debug.WriteLine($"await");
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("任务已取消");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.GetType()}={ex.Message}");
            }
            finally
            {
                IsRunning = false;
            }
        }

        private void StartCore()
        {
            Debug.WriteLine($"StartCore{Thread.CurrentThread.ManagedThreadId}");
            cts.Token.ThrowIfCancellationRequested();
            Lure();
            Cast();
            Observe();
        }

        private void Lure()
        {
            Debug.WriteLine($"Lure{Thread.CurrentThread.ManagedThreadId}");
            if (DateTime.Now - lureTime > lureDuration)
            {
                Debug.WriteLine($"Lure");
                lureTime = DateTime.Now;
                Process.KeyPress(ConsoleKey.D4);//Lure
                Task.Delay(250).Wait();
                Process.KeyPress(ConsoleKey.D5);//Rod
                Task.Delay(15000).Wait(cts.Token);
            }
        }

        private void Cast()
        {
            Debug.WriteLine($"Cast{Thread.CurrentThread.ManagedThreadId}");
            Process.KeyPress(ConsoleKey.D1);
            Task.Delay(250).Wait();
        }

        private void Observe()
        {
            Debug.WriteLine($"Observe{Thread.CurrentThread.ManagedThreadId}");
            var observeTime = DateTime.Now;
            SortedSet<int> bobber = new();
            while (DateTime.Now - observeTime < observeDuration)
            {
                Bitmap bitmap = Process.GetBitmap();
                Point point = GetBobber(bitmap);
                if (!point.IsEmpty)
                {
                    Bobber?.Invoke(this, new BobberEventArgs { Image = bitmap, Location = point });
                    if (bobber.Add(point.Y) && bobber.Last() - bobber.First() > 5)
                    {
                        Loot(bitmap.ClientToScreen(point));
                        break;
                    }
                }
            }
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

            return points.SelectMany(f => points, (p1, p2) => new { p1, p2 })
                 .Where(f => Math.Abs(f.p1.X - f.p2.X) < 10 && Math.Abs(f.p1.Y - f.p2.Y) < 10)
                 .GroupBy(f => f.p1)
                 .OrderByDescending(f => f.Count())
                 .Select(f => f.Key)
                 .FirstOrDefault();
        }

        private void Loot(Point point)
        {
            Debug.WriteLine($"Loot{Thread.CurrentThread.ManagedThreadId}");
            Task.Delay(1000).Wait(cts.Token);
            //Process.MouseRightClick(point);
            Task.Delay(1000).Wait(cts.Token);
        }

        public void Stop() => cts?.Cancel();
    }

    public class BobberEventArgs : EventArgs
    {
        public Bitmap Image { get; set; }

        public Point Location { get; set; }
    }
}
