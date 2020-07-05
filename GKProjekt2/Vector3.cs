using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GKProjekt2
{
    public struct Vector3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public SimpleColor ConvertToColor()
        {
            byte R = (byte)MathExtension.Clamp(X * 128 + 127, 0, 255);
            byte G = (byte)MathExtension.Clamp(Y * 128 + 127, 0, 255);
            byte B = (byte)MathExtension.Clamp(Z * 128 + 127, 0, 255);
            return new SimpleColor(R, G, B);
        }

        public SimpleColor SimpleConvertToColor()
        {
            byte R = (byte)MathExtension.Clamp(X, 0, 255);
            byte G = (byte)MathExtension.Clamp(Y, 0, 255);
            byte B = (byte)MathExtension.Clamp(Z, 0, 255);
            return new SimpleColor(R, G, B);
        }

        public Vector3 Normalize()
        {
            double div = Math.Sqrt(X * X + Y * Y + Z * Z);
            if (div == 0)
                return new Vector3(0, 0, 1);
            return new Vector3(X / div, Y / div, Z / div);
        }

        public void NormalizeInPlace()
        {
            double div = Math.Sqrt(X * X + Y * Y + Z * Z);
            if (div == 0)
            {
                this.X = 0;
                this.Y = 0;
                this.Z = 1;
                return;
            }
            this.X /= div;
            this.Y /= div;
            this.Z /= div;
        }

        public static double Cosinus(Vector3 A, Vector3 B)
        {
            return A.X * B.X + A.Y * B.Y + A.Z * B.Z;
        }

        public static double Cosinus(ref Vector3 A, ref Vector3 B)
        {
            return A.X * B.X + A.Y * B.Y + A.Z * B.Z;
        }

        public static Vector3 operator *(int a, Vector3 v)
        {
            return new Vector3(v.X * (double)a, v.Y * (double)a, v.Z * (double)a);
        }

        public static Vector3 operator *(double a, Vector3 v)
        {
            return new Vector3(v.X * a, v.Y * a, v.Z * a);
        }

        public static Vector3 operator +(Vector3 A, Vector3 B)
        {
            return new Vector3(A.X + B.X, A.Y + B.Y, A.Z + B.Z);
        }
        public static Vector3 operator -(Vector3 A, Vector3 B)
        {
            return new Vector3(A.X - B.X, A.Y - B.Y, A.Z - B.Z);
        }

        public static Vector3 MultiplyDot(Vector3 A, Vector3 B)
        {
            return new Vector3(A.X * B.X, A.Y * B.Y, A.Z * B.Z);
        }
    }
}
