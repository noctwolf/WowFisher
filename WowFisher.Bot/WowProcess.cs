using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using static Vanara.PInvoke.User32;

namespace WowFisher.Bot
{
    public static class WowProcess
    {
        private static readonly Random random = new();
        private static readonly List<string> names = new() { "Wow", "WowClassic", "Wow-64", "Notepad" };

        public static bool IsWow(this Process process) => names.Contains(process.ProcessName);

        public static bool IsWowClassic(this Process process) => process.ProcessName.Contains("Classic", StringComparison.OrdinalIgnoreCase);

        public static void KeyPress(this Process process, ConsoleKey consoleKey) => process.Send(new InputBuilder().AddKeyPress(consoleKey));

        /// <summary>
        /// 鼠标右键点击
        /// </summary>
        /// <param name="process"></param>
        /// <param name="point">0到65535的归一化坐标</param>
        public static void MouseRightClick(this Process process, Point point) => process.Send(new InputBuilder().AddMouseRightClick(point.X, point.Y));

        public static uint Send(this Process process, InputBuilder inputs)
        {
            if (!SetForegroundWindow(process.MainWindowHandle)) return 0;
            return SendInput((uint)inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }


        public static Process[] GetWowProcesses() => Process.GetProcesses().AsEnumerable().Where(f => f.IsWow()).ToArray();

    }
}
