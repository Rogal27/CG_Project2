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
    public static class Paint
    {
        //paint polygon
        public static void PaintPolygon(List<SimplePoint> points, TriangleFillingMode fillingMode, ref double KdFactor, ref double KsFactor, ref int MFactor)
        {
            IntPoint[] pointsArray = new IntPoint[points.Count];
            int[] indexes = new int[points.Count];
            int i = 0;

            //fast
            (byte R, byte G, byte B) p0Fast = (0, 0, 0);
            (byte R, byte G, byte B) p1Fast = (0, 0, 0);
            (byte R, byte G, byte B) p2Fast = (0, 0, 0);
            int triangleField = 0;
            //hybrid
            SimpleColor c0Hybrid = new SimpleColor();
            SimpleColor c1Hybrid = new SimpleColor();
            SimpleColor c2Hybrid = new SimpleColor();
            Vector3 v0Hybrid = new Vector3();
            Vector3 v1Hybrid = new Vector3();
            Vector3 v2Hybrid = new Vector3();

            foreach (var item in points)
            {
                pointsArray[i] = new IntPoint(item.X, item.Y);
                if (pointsArray[i].X >= fillingMode.PictureBitmapColor.GetLength(1))
                    pointsArray[i].X = fillingMode.PictureBitmapColor.GetLength(1) - 1;
                if (pointsArray[i].Y >= fillingMode.PictureBitmapColor.GetLength(0))
                    pointsArray[i].Y = fillingMode.PictureBitmapColor.GetLength(0) - 1;
                indexes[i] = i;
                i++;
            }

            if(fillingMode.fillingMode == FillingMode.Fast)
            {
                p0Fast = CalculateResultColorInPoint(pointsArray[0].X, pointsArray[0].Y, fillingMode, ref KdFactor, ref KsFactor, ref MFactor);
                p1Fast = CalculateResultColorInPoint(pointsArray[1].X, pointsArray[1].Y, fillingMode, ref KdFactor, ref KsFactor, ref MFactor);
                p2Fast = CalculateResultColorInPoint(pointsArray[2].X, pointsArray[2].Y, fillingMode, ref KdFactor, ref KsFactor, ref MFactor);
                triangleField = IntPoint.CalculateTriangleField(pointsArray);
            }
            else if(fillingMode.fillingMode == FillingMode.Hybrid)
            {
                c0Hybrid = fillingMode.PictureBitmapColor[pointsArray[0].Y, pointsArray[0].X];
                c1Hybrid = fillingMode.PictureBitmapColor[pointsArray[1].Y, pointsArray[1].X];
                c2Hybrid = fillingMode.PictureBitmapColor[pointsArray[2].Y, pointsArray[2].X];
                v0Hybrid = fillingMode.NormalBitmapVector[pointsArray[0].Y, pointsArray[0].X];
                v1Hybrid = fillingMode.NormalBitmapVector[pointsArray[1].Y, pointsArray[1].X];
                v2Hybrid = fillingMode.NormalBitmapVector[pointsArray[2].Y, pointsArray[2].X];
                triangleField = IntPoint.CalculateTriangleField(pointsArray);
            }

            Array.Sort(indexes, (p1, p2) =>
            {
                if (pointsArray[p1].Y > pointsArray[p2].Y)
                    return 1;
                else if (pointsArray[p1].Y < pointsArray[p2].Y)
                    return -1;
                else if (pointsArray[p1].X > pointsArray[p2].X)
                    return 1;
                else if (pointsArray[p1].X < pointsArray[p2].X)
                    return -1;
                return 0;
            });

            List<(int ymax, double x, double m)> AET = new List<(int, double, double)>();

            int ymin = pointsArray[indexes[0]].Y;
            int ymax = pointsArray[indexes[indexes.Length - 1]].Y;
            int startingIndex = 0;
            bool removeEdgesFromAET = false;

            for (int scanLineY = ymin; scanLineY <= ymax; scanLineY++)
            {
                for (int j = startingIndex; j < indexes.Length; j++)
                {
                    if (pointsArray[indexes[j]].Y > scanLineY - 1)
                        break;
                    if (pointsArray[indexes[j]].Y == scanLineY - 1)
                    {
                        startingIndex++;
                        int index = indexes[j];
                        int previousIndex = (indexes[j] - 1 + pointsArray.Length) % pointsArray.Length;
                        int nextIndex = (indexes[j] + 1) % pointsArray.Length;
                        if (pointsArray[previousIndex].Y >= pointsArray[indexes[j]].Y)
                        {
                            //dodaj krawedz Pi-1 Pi do AET
                            int x = pointsArray[index].X;
                            if (pointsArray[index].Y != pointsArray[previousIndex].Y)
                            {
                                double m = (double)(pointsArray[index].X - pointsArray[previousIndex].X) / (double)(pointsArray[index].Y - pointsArray[previousIndex].Y);
                                AET.Add((pointsArray[previousIndex].Y, x, m));
                            }
                        }
                        else
                        {
                            //usun krawedz Pi-1 Pi z AET
                            removeEdgesFromAET = true;
                        }
                        if (pointsArray[nextIndex].Y >= pointsArray[indexes[j]].Y)
                        {
                            //dodaj krawedz Pi+1 Pi do AET
                            int x = pointsArray[index].X;
                            if (pointsArray[nextIndex].Y != pointsArray[index].Y)
                            {
                                double m = (double)(pointsArray[nextIndex].X - pointsArray[index].X) / (double)(pointsArray[nextIndex].Y - pointsArray[index].Y);
                                AET.Add((pointsArray[nextIndex].Y, x, m));
                            }
                        }
                        else
                        {
                            //usun krawedz Pi+1 Pi z AET
                            removeEdgesFromAET = true;
                        }
                        if(removeEdgesFromAET == true)
                        {
                            AET.RemoveAll((aet) =>
                            {
                                return aet.ymax == scanLineY - 1;
                            });
                            removeEdgesFromAET = false;
                        }
                    }
                }
                //uaktualnij AET
                //posortuj
                AET.Sort((p1, p2) =>
                {
                    if (p1.x > p2.x)
                    {
                        return 1;
                    }
                    else if (p1.x < p2.x)
                    {
                        return -1;
                    }
                    else
                        return 0;
                });
                //wypelnij


                if (fillingMode.fillingMode == FillingMode.Direct)
                {
                    FillScanLineDirect(scanLineY, AET, fillingMode, ref KdFactor, ref KsFactor, ref MFactor);
                }
                else if(fillingMode.fillingMode == FillingMode.Fast)
                {
                    //policz kolory w wierzcholkach trojkat
                    FillScanLineFast(scanLineY, AET, fillingMode, ref triangleField, pointsArray, ref p0Fast, ref p1Fast, ref p2Fast);
                }
                else//hybrid mode
                {
                    FillScanLineHybrid(scanLineY, AET, fillingMode, ref triangleField, pointsArray, ref v0Hybrid, ref c0Hybrid, ref v1Hybrid, ref c1Hybrid, ref v2Hybrid, ref c2Hybrid, ref KdFactor, ref KsFactor, ref MFactor);
                }
                //uaktualnij x
                for (int k = 0; k < AET.Count; k++)
                {
                    var aet = AET[k];
                    AET[k] = (aet.ymax, aet.x += aet.m, aet.m);
                }
            }
        }
        //fill scan lines
        private static void FillScanLineDirect(int scanLineY, List<(int ymax, double x, double m)> AET, TriangleFillingMode fillingMode, ref double KdFactor, ref double KsFactor, ref int MFactor)
        {
            for (int i = 0; i < AET.Count; i += 2)
            {
                for (int j = (int)Math.Ceiling(AET[i].x); j <= (int)Math.Floor(AET[i + 1].x); j++)
                {
                    if (j >= fillingMode.PictureBitmapColor.GetLength(1) || scanLineY >= fillingMode.PictureBitmapColor.GetLength(0))
                        continue;
                    (byte R, byte G, byte B) = CalculateResultColorInPoint(ref j, ref scanLineY, fillingMode, ref KdFactor, ref KsFactor, ref MFactor);
                    fillingMode.writeableBitmapColor[scanLineY, j].R = R;
                    fillingMode.writeableBitmapColor[scanLineY, j].G = G;
                    fillingMode.writeableBitmapColor[scanLineY, j].B = B;
                }
            }
        }
        private static void FillScanLineFast(int scanLineY, List<(int ymax, double x, double m)> AET, TriangleFillingMode fillingMode, ref int triangleField, IntPoint[] points, ref (byte R, byte G, byte B) p0, ref (byte R, byte G, byte B) p1, ref (byte R, byte G, byte B) p2)
        {
            for (int i = 0; i < AET.Count; i += 2)
            {
                for (int j = (int)Math.Ceiling(AET[i].x); j <= (int)Math.Floor(AET[i + 1].x); j++)
                {
                    //x = j
                    //y = scanLineY
                    //result color -> srednia wazona
                    if (j >= fillingMode.PictureBitmapColor.GetLength(1) || scanLineY >= fillingMode.PictureBitmapColor.GetLength(0) || j < 0 || scanLineY < 0)
                        continue;
                    (byte R, byte G, byte B) = CalculateWeightAverageColorFast(ref j, ref scanLineY, ref triangleField, points, ref p0, ref p1, ref p2);
                    fillingMode.writeableBitmapColor[scanLineY, j].R = R;
                    fillingMode.writeableBitmapColor[scanLineY, j].G = G;
                    fillingMode.writeableBitmapColor[scanLineY, j].B = B;
                }
            }
        }
        private static void FillScanLineHybrid(int scanLineY, List<(int ymax, double x, double m)> AET, TriangleFillingMode fillingMode, ref int triangleField, IntPoint[] points, ref Vector3 v0, ref SimpleColor c0, ref Vector3 v1, ref SimpleColor c1, ref Vector3 v2, ref SimpleColor c2, ref double KdFactor, ref double KsFactor, ref int MFactor)
        {
            for (int i = 0; i < AET.Count; i += 2)
            {
                for (int j = (int)Math.Ceiling(AET[i].x); j <= (int)Math.Floor(AET[i + 1].x); j++)
                {
                    //x = j
                    //y = scanLineY
                    //result color -> srednia wazona
                    if (j >= fillingMode.PictureBitmapColor.GetLength(1) || scanLineY >= fillingMode.PictureBitmapColor.GetLength(0) || j < 0 || scanLineY < 0)
                        continue;
                    var (vectorN, color) = CalculateWeightAverageColorAndVectorHybrid(ref j, ref scanLineY, ref triangleField, points, ref v0, ref c0, ref v1, ref c1, ref v2, ref c2);
                    (byte R, byte G, byte B) = CalculateResultColorInPointHybrid(ref j, ref scanLineY, fillingMode, ref vectorN, ref color, ref KdFactor, ref KsFactor, ref MFactor);
                    fillingMode.writeableBitmapColor[scanLineY, j].R = R; 
                    fillingMode.writeableBitmapColor[scanLineY, j].G = G; 
                    fillingMode.writeableBitmapColor[scanLineY, j].B = B; 
                }
            }
        }
        //calculating colors in points
        private static (byte R, byte G, byte B) CalculateResultColorInPoint(int x, int y, TriangleFillingMode fillingMode, ref double KdFactor, ref double KsFactor, ref int MFactor)
        {
            double cosNL = Vector3.Cosinus(fillingMode.NormalBitmapVector[y, x], fillingMode.VectorL);

            if (cosNL < 0)
                cosNL = 0;

            double vectorRX = 2 * cosNL * fillingMode.NormalBitmapVector[y, x].X - fillingMode.VectorL.X;
            double vectorRY = 2 * cosNL * fillingMode.NormalBitmapVector[y, x].Y - fillingMode.VectorL.Y;
            double vectorRZ = 2 * cosNL * fillingMode.NormalBitmapVector[y, x].Z - fillingMode.VectorL.Z;

            double cosVR = fillingMode.ViewVector.X * vectorRX + fillingMode.ViewVector.Y * vectorRY + fillingMode.ViewVector.Z * vectorRZ;

            if (cosVR < 0)
                cosVR = 0;

            double mPowerCosVR = Math.Pow(cosVR, MFactor);

            double resultX = fillingMode.LightVector.X * fillingMode.PictureBitmapColor[y, x].R;
            double resultY = fillingMode.LightVector.Y * fillingMode.PictureBitmapColor[y, x].G;
            double resultZ = fillingMode.LightVector.Z * fillingMode.PictureBitmapColor[y, x].B;

            double tmp1 = KdFactor * cosNL;
            double tmp2 = KsFactor * mPowerCosVR;
            tmp1 += tmp2;

            resultX = (tmp1 * resultX);
            resultY = (tmp1 * resultY);
            resultZ = (tmp1 * resultZ);
            byte R = (byte)MathExtension.Clamp(resultX, 0, 255);
            byte G = (byte)MathExtension.Clamp(resultY, 0, 255);
            byte B = (byte)MathExtension.Clamp(resultZ, 0, 255);

            return (R, G, B);
        }
        private static (byte R, byte G, byte B) CalculateResultColorInPoint(ref int x, ref int y, TriangleFillingMode fillingMode, ref double KdFactor, ref double KsFactor, ref int MFactor)
        {
            double cosNL = Vector3.Cosinus(fillingMode.NormalBitmapVector[y, x], fillingMode.VectorL);

            if (cosNL < 0)
                cosNL = 0;

            double vectorRX = 2 * cosNL * fillingMode.NormalBitmapVector[y, x].X - fillingMode.VectorL.X;
            double vectorRY = 2 * cosNL * fillingMode.NormalBitmapVector[y, x].Y - fillingMode.VectorL.Y;
            double vectorRZ = 2 * cosNL * fillingMode.NormalBitmapVector[y, x].Z - fillingMode.VectorL.Z;

            double cosVR = fillingMode.ViewVector.X * vectorRX + fillingMode.ViewVector.Y * vectorRY + fillingMode.ViewVector.Z * vectorRZ;

            if (cosVR < 0)
                cosVR = 0;

            double mPowerCosVR = Math.Pow(cosVR, MFactor);

            double resultX = fillingMode.LightVector.X * fillingMode.PictureBitmapColor[y, x].R;
            double resultY = fillingMode.LightVector.Y * fillingMode.PictureBitmapColor[y, x].G;
            double resultZ = fillingMode.LightVector.Z * fillingMode.PictureBitmapColor[y, x].B;

            double tmp1 = KdFactor * cosNL;
            double tmp2 = KsFactor * mPowerCosVR;
            tmp1 += tmp2;

            resultX = (tmp1 * resultX);
            resultY = (tmp1 * resultY);
            resultZ = (tmp1 * resultZ);
            byte R = (byte)MathExtension.Clamp(resultX, 0, 255);
            byte G = (byte)MathExtension.Clamp(resultY, 0, 255);
            byte B = (byte)MathExtension.Clamp(resultZ, 0, 255);

            return (R, G, B);
        }
        
        private static (byte R, byte G, byte B) CalculateWeightAverageColorFast(ref int x, ref int y, ref int triangleField, IntPoint[] points, ref (byte R, byte G, byte B) p0, ref (byte R, byte G, byte B) p1, ref (byte R, byte G, byte B) p2)
        {
            int alfa0 = IntPoint.CalculateTriangleField(ref x, ref y, points[1], points[2]);
            int alfa1 = IntPoint.CalculateTriangleField(ref x, ref y, points[0], points[2]);
            int alfa2 = IntPoint.CalculateTriangleField(ref x, ref y, points[0], points[1]);
            //Vector3 resultVector = (alfa0 * p0) + (alfa1 * p1) + (alfa2 * p2);
            double resultR = alfa0 * p0.R + alfa1 * p1.R + alfa2 * p2.R;
            double resultG = alfa0 * p0.G + alfa1 * p1.G + alfa2 * p2.G;
            double resultB = alfa0 * p0.B + alfa1 * p1.B + alfa2 * p2.B;
            resultR /= (double)triangleField;
            resultG /= (double)triangleField;
            resultB /= (double)triangleField;
            byte R = (byte)MathExtension.Clamp(resultR, 0, 255);
            byte G = (byte)MathExtension.Clamp(resultG, 0, 255);
            byte B = (byte)MathExtension.Clamp(resultB, 0, 255);
            return (R, G, B);
        }

        private static (byte R, byte G, byte B) CalculateResultColorInPointHybrid(ref int x, ref int y, TriangleFillingMode fillingMode, ref (double X, double Y, double Z) vectorN, ref (byte R, byte G, byte B) pictureColorVector, ref double KdFactor, ref double KsFactor, ref int MFactor)
        {
            double cosNL = vectorN.X * fillingMode.VectorL.X + vectorN.Y * fillingMode.VectorL.Y + vectorN.Z * fillingMode.VectorL.Z;

            if (cosNL < 0)
                cosNL = 0;

            double vectorRX = 2 * cosNL * vectorN.X - fillingMode.VectorL.X;
            double vectorRY = 2 * cosNL * vectorN.Y - fillingMode.VectorL.Y;
            double vectorRZ = 2 * cosNL * vectorN.Z - fillingMode.VectorL.Z;

            double cosVR = fillingMode.ViewVector.X * vectorRX + fillingMode.ViewVector.Y * vectorRY + fillingMode.ViewVector.Z * vectorRZ;

            if (cosVR < 0)
                cosVR = 0;

            double mPowerCosVR = Math.Pow(cosVR, MFactor);

            double resultX = fillingMode.LightVector.X * pictureColorVector.R;
            double resultY = fillingMode.LightVector.Y * pictureColorVector.G;
            double resultZ = fillingMode.LightVector.Z * pictureColorVector.B;

            double tmp1 = KdFactor * cosNL;
            double tmp2 = KsFactor * mPowerCosVR;
            tmp1 += tmp2;

            resultX = (tmp1 * resultX);
            resultY = (tmp1 * resultY);
            resultZ = (tmp1 * resultZ);
            byte R = (byte)MathExtension.Clamp(resultX, 0, 255);
            byte G = (byte)MathExtension.Clamp(resultY, 0, 255);
            byte B = (byte)MathExtension.Clamp(resultZ, 0, 255);

            return (R, G, B);
        }
        private static ((double X, double Y, double Z) vectorN, (byte R, byte G, byte B) color) CalculateWeightAverageColorAndVectorHybrid(ref int x, ref int y, ref int triangleField, IntPoint[] points, ref Vector3 v0, ref SimpleColor c0, ref Vector3 v1, ref SimpleColor c1, ref Vector3 v2, ref SimpleColor c2)
        {
            int alfa0 = IntPoint.CalculateTriangleField(ref x, ref y, points[1], points[2]);
            int alfa1 = IntPoint.CalculateTriangleField(ref x, ref y, points[0], points[2]);
            int alfa2 = IntPoint.CalculateTriangleField(ref x, ref y, points[0], points[1]);

            double vectorX = alfa0 * v0.X + alfa1 * v1.X + alfa2 * v2.X;
            double vectorY = alfa0 * v0.Y + alfa1 * v1.Y + alfa2 * v2.Y;
            double vectorZ = alfa0 * v0.Z + alfa1 * v1.Z + alfa2 * v2.Z;
            vectorX /= (double)triangleField;
            vectorY /= (double)triangleField;
            vectorZ /= (double)triangleField;

            double colorR = alfa0 * c0.R + alfa1 * c1.R + alfa2 * c2.R;
            double colorG = alfa0 * c0.G + alfa1 * c1.G + alfa2 * c2.G;
            double colorB = alfa0 * c0.B + alfa1 * c1.B + alfa2 * c2.B;
            colorR /= (double)triangleField;
            colorG /= (double)triangleField;
            colorB /= (double)triangleField;
            byte R = (byte)MathExtension.Clamp(colorR, 0, 255);
            byte G = (byte)MathExtension.Clamp(colorG, 0, 255);
            byte B = (byte)MathExtension.Clamp(colorB, 0, 255);
            return ((vectorX, vectorY, vectorZ), (R, G, B));
        }


        //copying and retrieving from writable bitmaps

        public static void CopyToWriteableBitmap(WriteableBitmap writeableBitmap, SimpleColor[,] pixels)
        {
            writeableBitmap.Lock();
            unsafe
            {
                int writeablebpp = writeableBitmap.Format.BitsPerPixel / 8;
                int writeableBuffer = (int)writeableBitmap.BackBuffer;
                int bufferstride = writeableBitmap.BackBufferStride;
                Parallel.For(0, pixels.GetLength(0), y =>
                {
                    int place = writeableBuffer + y * bufferstride;
                    for (int x = 0; x < pixels.GetLength(1); x++)
                    {
                        *((int*)place) = pixels[y, x].ToInt();
                        place += writeablebpp;
                    }
                });
            }
            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight));
            writeableBitmap.Unlock();
        }
        public static void CopyToWriteableBitmapRect(WriteableBitmap writeableBitmap, SimpleColor[,] pixels, Int32Rect rect)
        {
            int top = rect.Y;
            int bottom = rect.Y + rect.Height;
            if (bottom >= writeableBitmap.PixelHeight)
                bottom = writeableBitmap.PixelHeight - 1;
            int left = rect.X;
            int right = rect.X + rect.Width;
            if (right >= writeableBitmap.PixelWidth)
                right = writeableBitmap.PixelWidth - 1;
            writeableBitmap.Lock();
            unsafe
            {
                int writeablebpp = writeableBitmap.Format.BitsPerPixel / 8;
                int writeableBuffer = (int)writeableBitmap.BackBuffer;
                int bufferstride = writeableBitmap.BackBufferStride;
                Parallel.For(top, bottom + 1, y =>
                  {
                      int place = writeableBuffer + y * bufferstride;
                      place += left * writeablebpp;
                      for (int x = left; x < right + 1; x++)
                      {
                          *((int*)place) = pixels[y, x].ToInt();
                          place += writeablebpp;
                      }
                  });
            }
            writeableBitmap.AddDirtyRect(rect);
            writeableBitmap.Unlock();
        }
        public static void ReadColorsFromBitmap(WriteableBitmap writeableBitmap, SimpleColor[,] pixels)
        {
            writeableBitmap.Lock();
            unsafe
            {
                int writeablebpp = writeableBitmap.Format.BitsPerPixel / 8;
                int writeableBuffer = (int)writeableBitmap.BackBuffer;
                int bufferstride = writeableBitmap.BackBufferStride;
                for (int y = 0; y < pixels.GetLength(0); y++)
                {
                    for (int x = 0; x < pixels.GetLength(1); x++)
                    {
                        int col = *((int*)writeableBuffer);
                        //SimpleColor sc = new SimpleColor(col);
                        pixels[y, x].ConvertFromInt(col);
                        writeableBuffer += writeablebpp;
                    }
                }
            }
            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight));
            writeableBitmap.Unlock();
        }

        //filling not using pointers
        public static void FillBitmap(WriteableBitmap bitmap, Color color)
        {
            int stride = bitmap.PixelWidth * (bitmap.Format.BitsPerPixel / 8);
            int arraySize = stride * bitmap.PixelHeight;
            byte[] pixels = new byte[arraySize];
            int j = 0;
            for (int i = 0; i < pixels.Length / 4; i++)
            {
                pixels[j] = color.B;//blue
                pixels[j + 1] = color.G;//green
                pixels[j + 2] = color.R;//red             
                pixels[j + 3] = color.A;
                j += 4;
            }
            Int32Rect rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            bitmap.WritePixels(rect, pixels, stride, 0);
        }
        public static void FillBitmap(WriteableBitmap bitmap, SimpleColor[,] colors)
        {
            int stride = bitmap.PixelWidth * (bitmap.Format.BitsPerPixel / 8);
            int arraySize = stride * bitmap.PixelHeight;
            byte[] pixels = new byte[arraySize];
            int j = 0;
            int x = -1;
            int y = 0;
            for (int i = 0; i < pixels.Length / 4; i++)
            {
                if (i % colors.GetLength(1) == 0)
                {
                    y++;
                    x = 0;
                }
                pixels[j] = colors[y, x].B;//blue
                pixels[j + 1] = colors[y, x].G;//green
                pixels[j + 2] = colors[y, x].R;//red             
                pixels[j + 3] = 255;
                j += 4;
                x++;
            }
            Int32Rect rect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            bitmap.WritePixels(rect, pixels, stride, 0);
        }
    }
}
