using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GKProjekt2
{
    public enum FillingMode
    {
        Direct,
        Fast,
        Hybrid
    }

    public enum VectorNMode
    {
        Default,
        NormalMap,
        Bubble
    }

    public class TriangleFillingMode
    {
        public FillingMode fillingMode { get; set; }
        //vectorL
        public bool IsDefaultVectorL { get; set; }
        public Vector3 VectorL { get; set; } //should be normalized
        //vectorN
        public VectorNMode vectorNMode { get; set; }
        //public Vector3 NonTexturedVectorN { get; set; } //should be normalized
        public Vector3[,] NormalBitmapVector { get; set; } //should be normalized
        //background
        public bool IsTextureFill { get; set; }
        public SimpleColor NonTexturedFillColor { get; set; }
        public SimpleColor[,] PictureBitmapColor { get; set; }
        //view vector
        public Vector3 ViewVector { get; set; }
        //light color
        public bool IsDefaulLightColor { get; set; }
        public Vector3 LightVector { get; set; }
        //factors
        public bool RandomFactors { get; set; }
        public double KdFactor { get; set; } //(range 0-1 , should be divided by 100)
        public int MFactor { get; set; } //(range 1-100)
        //writeable bitmap color
        public WriteableBitmap writeableBitmap { get; set; }
        public SimpleColor[,] writeableBitmapColor { get; set; }
        public TriangleFillingMode(FillingMode fm, bool randomfactors, bool vectorL, VectorNMode vectorN, bool textureFill, bool lightcolor)
        {
            this.fillingMode = fm;
            this.RandomFactors = randomfactors;
            this.IsDefaultVectorL = vectorL;
            this.vectorNMode = vectorN;
            this.IsTextureFill = textureFill;
            this.IsDefaulLightColor = lightcolor;
            VectorL = Globals.DefaultVectorL.Normalize();
            //NonTexturedVectorN = Globals.DefaultVectorN;
            LightVector = new SimpleColor(Globals.LightColor).ConvertColorToVector();
            ViewVector = Globals.ViewVector;
            KdFactor = 100d;
            MFactor = 1;
        }
    }
}
