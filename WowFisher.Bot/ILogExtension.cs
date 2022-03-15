using log4net;
using System;
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

            public Disposable(ILog log, string memberName)
            {
                this.log = log;
                this.memberName = memberName;
            }

            public void Dispose() => log.Info($"{memberName} - Exit");
        }
    }
}
