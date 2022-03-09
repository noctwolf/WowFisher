using System.Drawing;

namespace WowFisher.Bot
{
    public class ColorSerial
    {
        public ColorSerial(bool isWowClassic)
        {
            if (isWowClassic)
            {
                BigSmallRatio = 1;
                CloseRatio = 1;
            }
            else
            {
                BigSmallRatio = 0.5;
                CloseRatio = 0.5;
            }
        }

        public double BigSmallRatio { get; set; }

        public double CloseRatio { get; set; }

        public bool IsRed { get; set; } = true;

        public bool IsMatch(Color color) => IsMatch(color.R, color.G, color.B);

        public bool IsMatch(byte r, byte g, byte b)
        {
            if (!IsRed) r = Exchange(ref b, r);
            return IsBig(r, g) && IsBig(r, b) && IsClose(g, b);
        }

        private bool IsBig(byte big, byte small) => small / big < BigSmallRatio;

        private bool IsClose(byte min, byte max)
        {
            if (min > max) min = Exchange(ref max, min);
            return min / (max - 20) > CloseRatio;
        }

        private static byte Exchange(ref byte location, byte value)
        {
            byte temp = location;
            location = value;
            return temp;
        }
    }
}
