namespace Tools.Core.Models
{
    public struct SvgDetails
    {
        public SvgDetails(
            int x1,
            int x2,
            int y1,
            int y2,
            byte r,
            byte g,
            byte b)
        {
            X1 = x1;
            X2 = x2;
            Y1 = y1;
            Y2 = y2;
            R = r;
            G = g;
            B = b;
        }
        public int X1 { get; set; }

        public int X2 { get; set; }

        public int Y1 { get; set; }

        public int Y2 { get; set; }

        public byte R { get; set; }

        public byte G { get; set; }

        public byte B { get; set; }
    }
}
