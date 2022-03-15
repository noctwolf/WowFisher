using log4net;
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
        ILog log = LogManager.GetLogger(typeof(Fisher));

        public event EventHandler<BobberEventArgs> Bobber;

        public Fisher(Process process)
        {
            Process = process;
            ColorSerial = new(process.IsWowClassic());
        }

        public ColorSerial ColorSerial { get; }

        public Process Process { get; }

        public bool IsRunning { get; private set; }

        public async Task StartAsync(CancellationTokenSource cancellationTokenSource = null)
        {
            log.Debug($"Enter");
            Debug.Assert(!IsRunning);
            IsRunning = true;
            try
            {
                cts = cancellationTokenSource ?? new CancellationTokenSource();
                Process.KeyPress(ConsoleKey.D5);//Rod
                while (true)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    await Task.Run(StartCore);
                    log.Debug($"await");
                }
            }
            catch (OperationCanceledException)
            {
                log.Debug("任务已取消");
            }
            catch (Exception ex)
            {
                log.Debug("Task.Run", ex);
            }
            finally
            {
                IsRunning = false;
            }
            log.Debug($"Exit");
        }

        private void StartCore()
        {
            log.Debug($"Enter");
            cts.Token.ThrowIfCancellationRequested();
            Lure();
            Cast();
            Observe();
            log.Debug($"Exit");
        }

        private void Lure()
        {
            log.Debug("Enter");
            if (DateTime.Now - lureTime > lureDuration)
            {
                log.Debug("Lure");
                lureTime = DateTime.Now;
                Process.KeyPress(ConsoleKey.D4);//Lure
                Task.Delay(250).Wait();
                Process.KeyPress(ConsoleKey.D5);//Rod
                Task.Delay(15000).Wait(cts.Token);
            }
            log.Debug("Exit");
        }

        private void Cast()
        {
            log.Debug($"Enter");
            Process.KeyPress(ConsoleKey.D1);
            Task.Delay(250).Wait();
            log.Debug($"Exit");
        }

        private void Observe()
        {
            log.Debug($"Enter");
            var observeTime = DateTime.Now;
            SortedSet<int> bobber = new();
            Stopwatch stopwatch = new();
            while (DateTime.Now - observeTime < observeDuration)
            {
                log.Debug($"while--{stopwatch.ElapsedMilliseconds}");
                stopwatch.Restart();
                cts.Token.ThrowIfCancellationRequested();
                Bitmap bitmap = Process.GetBitmap();
                log.Debug($"GetBitmap--{stopwatch.ElapsedMilliseconds}");
                stopwatch.Restart();
                cts.Token.ThrowIfCancellationRequested();
                Point point = GetBobber(bitmap);
                log.Debug($"GetBobber--{stopwatch.ElapsedMilliseconds}");
                stopwatch.Restart();
                cts.Token.ThrowIfCancellationRequested();
                Bobber?.Invoke(this, new BobberEventArgs { Image = bitmap, Location = point });
                log.Debug($"Bobber?.Invoke--{stopwatch.ElapsedMilliseconds}");
                if (!point.IsEmpty)
                {
                    if (bobber.Add(point.Y) && bobber.Last() - bobber.First() > bitmap.Height * 0.02)
                    {
                        Loot(bitmap.ClientToScreen(point));
                        break;
                    }
                }
            }
            log.Debug($"Exit");
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
            }).Take(500).ToList();

            return points.SelectMany(f => points, (p1, p2) => new { p1, p2 })
                 .Where(f => Math.Abs(f.p1.X - f.p2.X) < 10 && Math.Abs(f.p1.Y - f.p2.Y) < 10)
                 .GroupBy(f => f.p1)
                 .OrderByDescending(f => f.Count())
                 .Select(f => f.Key)
                 .FirstOrDefault();
        }

        private void Loot(Point point)
        {
            log.Debug($"Enter");
            Task.Delay(1000).Wait(cts.Token);
            //Process.MouseRightClick(point);
            Task.Delay(1000).Wait(cts.Token);
            log.Debug($"Exit");
        }

        public void Stop() => cts?.Cancel();
    }

    public class BobberEventArgs : EventArgs
    {
        public Bitmap Image { get; set; }

        public Point Location { get; set; }
    }
}
