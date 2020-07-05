using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GKProjekt2
{
    public static class Globals
    {
        //window
        public static string WindowName = "GK Project 2";
        public static int GridMargin = 30;
        public static int DefaultBitmapWidth = 1024;
        public static int DefaultBitmapHeight = 1024;
        //triangle grid
        public static int Rows = 5;
        public static int Columns = 5;
        //point
        public static int PointSize = 10;
        public static int PointZIndex = 15;
        public static Color PointColor = Colors.Transparent;
        //line
        public static int LineThickness = 1;
        public static Color LineColor = Color.FromRgb(0, 0, 0);
        public static int LineZIndex = 10;
        //bitmaps
        public static string BitmapSource = "..\\..\\Bitmaps\\porsche.bmp";
        public static string NormalMapSource = "..\\..\\NormalMaps\\normal_map_hd.bmp";
        public static int BitmapZIndex = 5;
        //vectorN
        public static Vector3 DefaultVectorN = new Vector3(0, 0, 1).Normalize();
        //vectorL
        public static Vector3 DefaultVectorL = new Vector3(0, 0, 1).Normalize();
        //timer
        public static int VectorLTimerIntervalMS = 100;
        //light color
        public static Color LightColor = Color.FromRgb(255, 255, 255);
        //view vector
        public static Vector3 ViewVector = new Vector3(0, 0, 1);
        //bubble 
        public static int BubbleRadius = 100;
    }
}
