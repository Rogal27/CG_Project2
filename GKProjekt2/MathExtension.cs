using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKProjekt2
{
    public static class MathExtension
    {
        public static byte Clamp(double value, byte minValue, byte maxValue)
        {
            if (value < minValue)
                return minValue;
            else if (value > maxValue)
                return maxValue;
            return (byte)value;
        }
    }
}
