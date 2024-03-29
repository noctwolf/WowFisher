﻿using log4net;
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
        private readonly ILog log = LogManager.GetLogger(typeof(Fisher));

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
            using IDisposable _ = log.Method();
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
                    log.Info($"await");
                }
            }
            catch (OperationCanceledException)
            {
                log.Info("任务已取消");
            }
            catch (Exception ex)
            {
                log.Info("StartAsync", ex);
            }
            finally
            {
                IsRunning = false;
            }
        }

        private void StartCore()
        {
            using IDisposable _ = log.Method();
            cts.Token.ThrowIfCancellationRequested();
            Lure();
            Cast();
            Observe();
        }

        private void Lure()
        {
            using IDisposable _ = log.Method();
            if (DateTime.Now - lureTime > lureDuration)
            {
                log.Info("Lure");
                lureTime = DateTime.Now;
                Process.KeyPress(ConsoleKey.D4);//Lure
                Task.Delay(250).Wait();
                Process.KeyPress(ConsoleKey.D5);//Rod
                Task.Delay(15000).Wait(cts.Token);
            }
        }

        private void Cast()
        {
            using IDisposable _ = log.Method();
            Process.KeyPress(ConsoleKey.D1);
            Task.Delay(1000).Wait(cts.Token);
        }

        private void Observe()
        {
            using IDisposable _ = log.Method();
            var observeTime = DateTime.Now;
            SortedSet<int> bobber = null;
            Point point = Point.Empty;
            while (DateTime.Now - observeTime < observeDuration)
            {
                cts.Token.ThrowIfCancellationRequested();
                Bitmap bitmap = Process.GetBitmap();
                cts.Token.ThrowIfCancellationRequested();
                point = GetBobber(bitmap, point);
                cts.Token.ThrowIfCancellationRequested();
                Bobber?.Invoke(this, new BobberEventArgs { Image = bitmap, Location = point });
                if (!point.IsEmpty)
                {
                    //log.Debug($"point={point}");
                    if (bobber == null) { bobber = new(); continue; }//忽略掉第一次识别的点
                    if (bobber.Add(point.Y) && bobber.Last() - bobber.First() > bitmap.Height * 0.015)
                    {
                        log.Debug(bobber);
                        Loot(bitmap.ClientToScreen(point));
                        break;
                    }
                }
            }
        }

        private Point GetBobber(Bitmap bitmap, Point point)
        {
            //using IDisposable _ = log.Method();
            Rectangle searchRect = new(Point.Empty, bitmap.Size);
            if (!point.IsEmpty)//缩小搜索范围，提高帧率
            {
                Rectangle pointRect = new(point.X - 20, point.Y - 20, 41, 41);
                if (searchRect.Contains(pointRect)) searchRect = pointRect;
            }
            var points = Enumerable.Range(searchRect.Y, searchRect.Height)
                .SelectMany(f => Enumerable.Range(searchRect.X, searchRect.Width), (y, x) => new Point(x, y));

            points = points.Where(f =>
            {
                bool isMatch = ColorSerial.IsMatch(bitmap.GetPixel(f.X, f.Y));
                if (isMatch) bitmap.SetPixel(f.X, f.Y, ColorSerial.IsRed ? Color.Red : Color.Blue);
                return isMatch;
            }).Take(500).ToList();

            return points.SelectMany(f => points, (p1, p2) => new { p1, p2 })
                .Where(f => Math.Abs(f.p1.X - f.p2.X) < 10 && Math.Abs(f.p1.Y - f.p2.Y) < 10)
                .GroupBy(f => f.p1)
                .Where(f => f.Count() > 10)
                .OrderByDescending(f => f.Count())
                .Select(f => f.Key)
                .FirstOrDefault();
        }

        private void Loot(Point point)
        {
            using IDisposable _ = log.Method();
            Task.Delay(1000).Wait(cts.Token);
            log.Info($"Loot:{point}");
            Process.MouseRightClick(point);
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
