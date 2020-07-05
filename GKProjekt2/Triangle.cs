using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GKProjekt2
{
    public class Triangle
    {
        public SimplePoint p1 { get; set; }
        public SimplePoint p2 { get; set; }
        public SimplePoint p3 { get; set; }
        public Canvas canvas { get; set; }
        public TriangleFillingMode fillingMode { get; set; }
        public Random random { get; set; }
        private double Kd;
        private double Ks;
        private int M;
        public Triangle(SimplePoint p1, SimplePoint p2, SimplePoint p3, Canvas canvas, TriangleFillingMode fillingMode)
        {
            this.p1 = p1;            
            this.p2 = p2;
            this.p3 = p3;
            p1.PropertyChanged += RedrawTriangleEventResponder;
            p2.PropertyChanged += RedrawTriangleEventResponder;
            p3.PropertyChanged += RedrawTriangleEventResponder;
            this.canvas = canvas;
            this.fillingMode = fillingMode;
            RenewRand();
        }

        public void RenewRand()
        {
            random = new Random(DateTime.Now.Millisecond * this.GetHashCode());
            Kd = random.NextDouble();
            Ks = 1d - Kd;
            M = random.Next(1, 101);
        }

        public List<SimplePoint> ToList()
        {
            var list = new List<SimplePoint>
            {
                p1,
                p2,
                p3
            };
            return list;
        }

        private void RedrawTriangleEventResponder(object sender, EventArgs eventArgs)
        {
            RedrawTriangle();
            Paint.CopyToWriteableBitmapRect(fillingMode.writeableBitmap, fillingMode.writeableBitmapColor, this.FindRectangle());
        }

        private Int32Rect FindRectangle()
        {
            var points = this.ToList();
            IntPoint[] pointsArray = new IntPoint[points.Count];
            
            int i = 0;
            foreach (var item in points)
            {
                pointsArray[i] = new IntPoint(item);
                i++;
            }
            int top = pointsArray[0].Y, left = pointsArray[0].X, bottom = pointsArray[0].Y, right = pointsArray[0].X;
            for (int k = 1; k < pointsArray.Length; k++)
            {
                if (pointsArray[k].X > right)
                {
                    right = pointsArray[k].X;
                }
                if (pointsArray[k].X < left)
                {
                    left = pointsArray[k].X;
                }
                if (pointsArray[k].Y < top)
                {
                    top = pointsArray[k].Y;
                }
                if (pointsArray[k].Y > bottom)
                {
                    bottom = pointsArray[k].Y;
                }
            }
            return new Int32Rect(left, top, right - left, bottom - top);
        }

        public void RedrawTriangle()
        {
            if (fillingMode.RandomFactors == false)
            {
                Ks = (100d - fillingMode.KdFactor) / 100d;
                Kd = fillingMode.KdFactor / 100d;
                M = fillingMode.MFactor;
            }
            Paint.PaintPolygon(this.ToList(), this.fillingMode, ref Ks, ref Kd, ref M);
        }
    }
}
