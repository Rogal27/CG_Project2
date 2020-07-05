using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GKProjekt2
{
    public struct SimpleColor
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public SimpleColor(ref byte r,ref byte g,ref byte b)
        {
            R = r;
            G = g;
            B = b;
        }
        public SimpleColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
        public SimpleColor((byte r, byte g, byte b) color)
        {
            R = color.r;
            G = color.g;
            B = color.b;
        }
        public SimpleColor(int col)
        {
            int mask = 255;
            R = (byte)((col & (mask << 16)) >> 16);
            G = (byte)((col & (mask << 8)) >> 8);
            B = (byte)(col & mask);
        }
        public SimpleColor(Color c)
        {
            R = c.R;
            G = c.G;
            B = c.B;
        }

        public void ConvertFromInt(int col)
        {
            int mask = 255;
            R = (byte)((col & (mask << 16)) >> 16);
            G = (byte)((col & (mask << 8)) >> 8);
            B = (byte)(col & mask);
        }

        public int ToInt()
        {
            int colorData = 0;
            colorData |= 255 << 24;
            colorData |= R << 16; // R
            colorData |= G << 8; // G
            colorData |= B; //B
            return colorData;
        }
        public Vector3 ConvertColorToVectorN()
        {
            double x = (double)(R - 127) / 128d;
            double y = (double)(G - 127) / 128d;
            double z = (double)B / 255d;
            return new Vector3(x, y, z);
        }

        public Vector3 ConvertColorToVector()
        {
            double x = (double)(R - 127) / 128d;
            double y = (double)(G - 127) / 128d;
            double z = (double)(B - 127) / 128d;
            return new Vector3(x, y, z);
        }
        public Vector3 SimpleConvertToVector()
        {
            return new Vector3(R, G, B);
        }
        public static Vector3[,] ConvertColorsToVectorsN(SimpleColor[,] color)
        {
            var array = new Vector3[color.GetLength(0), color.GetLength(1)];
            Parallel.For(0, color.GetLength(0), i =>
            {
                for (int j = 0; j < color.GetLength(1); j++)
                {
                    var vector = color[i,j].ConvertColorToVectorN();
                    vector = vector.Normalize();
                    array[i, j] = vector;
                }
            });
            return array;
        }
    }
}
