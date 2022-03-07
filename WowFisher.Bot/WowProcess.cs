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

        public static void KeyPress(this Process process, ConsoleKey key)
        {
            if (!SetForegroundWindow(process.MainWindowHandle)) return;
            INPUT[] inputs = new INPUT[2];
            inputs[0] = new INPUT(0, (ushort)key);
            inputs[1] = new INPUT(KEYEVENTF.KEYEVENTF_KEYUP, (ushort)key);

            if (SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT))) != inputs.Length)
            {
            }
        }

        public static void MouseRightClick(this Process process, Point point)
        {
            if (!SetForegroundWindow(process.MainWindowHandle)) return;
            if (!SetCursorPos(point.X, point.Y)) return;

            INPUT[] inputs = new INPUT[1];
            inputs[0] = new INPUT(MOUSEEVENTF.MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF.MOUSEEVENTF_RIGHTUP);

            if (SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT))) != inputs.Length)
            {
            }
        }

        public static Process[] GetWowProcesses() => Process.GetProcesses().AsEnumerable().Where(f => f.IsWow()).ToArray();

    }
}
