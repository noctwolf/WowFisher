using System;
using System.Collections.Generic;
using static Vanara.PInvoke.User32;

namespace WowFisher.Bot
{
    public class InputBuilder : List<INPUT>
    {
        public InputBuilder AddKeyDown(ConsoleKey consoleKey)
        {
            Add(new INPUT(0, (ushort)consoleKey));
            return this;
        }

        public InputBuilder AddKeyUp(ConsoleKey consoleKey)
        {
            Add(new INPUT(KEYEVENTF.KEYEVENTF_KEYUP, (ushort)consoleKey));
            return this;
        }

        public InputBuilder AddKeyPress(ConsoleKey consoleKey) => AddKeyDown(consoleKey).AddKeyUp(consoleKey);

        public InputBuilder AddMouseMove(int x, int y)
        {
            Add(new INPUT(MOUSEEVENTF.MOUSEEVENTF_MOVE | MOUSEEVENTF.MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF.MOUSEEVENTF_VIRTUALDESK, 0, x, y));
            return this;
        }

        public InputBuilder AddMouseRightClick()
        {
            Add(new INPUT(MOUSEEVENTF.MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF.MOUSEEVENTF_RIGHTUP));
            return this;
        }

        public InputBuilder AddMouseRightClick(int x, int y) => AddMouseMove(x, y).AddMouseRightClick();
    }
}
