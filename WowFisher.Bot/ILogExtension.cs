using log4net;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WowFisher.Bot
{
    public static class ILogExtension
    {
        public static IDisposable Method(this ILog log, [CallerMemberName] string memberName = "")
        {
            log.Info($"{memberName} - Enter");
            return new Disposable(log, memberName);
        }

        internal class Disposable : IDisposable
        {
            private readonly string memberName;
            private readonly ILog log;
            private readonly Stopwatch stopwatch = new();

            public Disposable(ILog log, string memberName)
            {
                stopwatch.Start();
                this.log = log;
                this.memberName = memberName;
            }

            public void Dispose() => log.Info($"{memberName} - Exit - {stopwatch.ElapsedMilliseconds}ms");
        }
    }
}
