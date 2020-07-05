using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKProjekt2
{
    public class IntPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public IntPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
        public IntPoint(double x, double y)
        {
            X = (int)Math.Round(x);
            Y = (int)Math.Round(y);
        }

        public IntPoint(SimplePoint p)
        {
            X = (int)Math.Round(p.X);
            Y = (int)Math.Round(p.Y);
        }
        public static double Length(IntPoint p1, IntPoint p2)
        {
            int x = p1.X - p2.X;
            int y = p1.Y - p2.Y;
            return Math.Sqrt(x * x + y * y);
        }

        public int SquareLength(ref int X, ref int Y)
        {
            int x = this.X - X;
            int y = this.Y - Y;
            return x * x + y * y;
        }

        public static int CalculateTriangleField(IntPoint[] points)
        {
            if (points.Length != 3)
                return -1;
            return Math.Abs(((points[1].X - points[0].X) * (points[2].Y - points[0].Y)) - ((points[1].Y - points[0].Y) * (points[2].X - points[0].X)));
        }

        public static int CalculateTriangleField(IntPoint p0, IntPoint p1, IntPoint p2)
        {
            return Math.Abs(((p1.X - p0.X) * (p2.Y - p0.Y)) - ((p1.Y - p0.Y) * (p2.X - p0.X)));
        }

        public static int CalculateTriangleField(ref int p0X, ref int p0Y, IntPoint p1, IntPoint p2)
        {
            return Math.Abs(((p1.X - p0X) * (p2.Y - p0Y)) - ((p1.Y - p0Y) * (p2.X - p0X)));
        }
    }
}
