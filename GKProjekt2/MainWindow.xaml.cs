using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GKProjekt2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private SimplePoint[,] PointsArray;
        private List<Edge> EdgesList;
        private List<Triangle> TrianglesList;

        //grid size
        private int _GridRows;
        public int GridRows
        {
            get
            {
                return _GridRows;
            }
            set
            {
                _GridRows = value;
                NotifyPropertyChanged();
            }
        }
        private int _GridColumns;
        public int GridColumns
        {
            get
            {
                return _GridColumns;
            }
            set
            {
                _GridColumns = value;
                NotifyPropertyChanged();
            }
        }
        //vectorL - TODO timer
        public bool IsDefaultVectorL
        {
            get
            {
                return triangleFillingMode.IsDefaultVectorL;
            }
            set
            {
                triangleFillingMode.IsDefaultVectorL = value;
                if (value == true)
                {
                    timer.Stop();
                    TimerPathIndex = 0;
                    triangleFillingMode.VectorL = Globals.DefaultVectorL.Normalize();
                    RedrawTriangles();
                }
                else
                {
                    TimerPathIndex = 0;
                    timer.Start();
                }
                NotifyPropertyChanged();
            }
        }

        public int TimerPathIndex;
        public List<Vector3> TimerPathList;
        public System.Windows.Threading.DispatcherTimer timer;
        public Vector3 VectorLProperty
        {
            get
            {
                return triangleFillingMode.VectorL;
            }
            set
            {
                triangleFillingMode.VectorL = value;
                RedrawTriangles();
            }
        }

        //vectorN
        public VectorNMode IsTexturedVectorN
        {
            get
            {
                return triangleFillingMode.vectorNMode;
            }
            set
            {
                VectorNPreviousValue = triangleFillingMode.vectorNMode;
                triangleFillingMode.vectorNMode = value;
                if (value == VectorNMode.Default)
                {
                    if (HasChooseFileReturnedNull == true)
                    {
                        HasChooseFileReturnedNull = false;
                    }
                    else
                    {
                        SetNormalVectorBitmap();
                        RedrawTriangles();
                    }
                }
                NotifyPropertyChanged();
            }
        }
        private VectorNMode VectorNPreviousValue;
        private string VectorNFileName;
        //light color
        public bool IsDefaultLightColor
        {
            get
            {
                return triangleFillingMode.IsDefaulLightColor;
            }
            set
            {
                triangleFillingMode.IsDefaulLightColor = value;
                if (value == true)
                {
                    triangleFillingMode.LightVector = new SimpleColor(Globals.LightColor).ConvertColorToVector();
                }
                else
                {
                    var colInfo = LightColorComboBox.SelectedItem as ColorInfo;
                    triangleFillingMode.LightVector = new SimpleColor(colInfo.color).ConvertColorToVector();
                }
                RedrawTriangles();
                NotifyPropertyChanged();
            }
        }
        //grid fill
        public bool IsTextureFill
        {
            get
            {
                return triangleFillingMode.IsTextureFill;
            }
            set
            {
                IsTextureFillPreviousValue = triangleFillingMode.IsTextureFill;
                triangleFillingMode.IsTextureFill = value;
                if (value == false)
                {
                    if (HasChooseFileReturnedNull == true)
                    {
                        HasChooseFileReturnedNull = false;
                    }
                    else
                    {
                        SetTextureBitmap();
                        RedrawTriangles();
                    }
                }
                NotifyPropertyChanged();
            }
        }
        private bool HasChooseFileReturnedNull;
        private bool IsTextureFillPreviousValue;
        private string TextureFileName;
        //filling mode
        public FillingMode fillingMode
        {
            get
            {
                return triangleFillingMode.fillingMode;
            }
            set
            {
                triangleFillingMode.fillingMode = value;
                RedrawTriangles();
                NotifyPropertyChanged();
            }
        }
        public TriangleFillingMode triangleFillingMode { get; set; }
        //factors
        public bool RandomFactors
        {
            get
            {
                return triangleFillingMode.RandomFactors;
            }
            set
            {
                triangleFillingMode.RandomFactors = value;
                RenewTrianglesRand();
                RedrawTriangles();
                NotifyPropertyChanged();
            }
        }
        public double KdFactor //(range 0-1 , should be divided by 100)
        {
            get
            {
                return triangleFillingMode.KdFactor;
            }
            set
            {
                triangleFillingMode.KdFactor = value;
                RedrawTriangles();
                NotifyPropertyChanged("KsFactor");
                NotifyPropertyChanged("KdFactor");
            }
        }
        public double KsFactor //(range 0-1 , should be divided by 100)
        {
            get
            {
                return 100d - triangleFillingMode.KdFactor;
            }
            set
            {
                triangleFillingMode.KdFactor = 100d - value;
                RedrawTriangles();
                NotifyPropertyChanged("KsFactor");
                NotifyPropertyChanged("KdFactor");
            }
        }
        public int MFactor //(range 1-100)
        {
            get
            {
                return triangleFillingMode.MFactor;
            }
            set
            {
                triangleFillingMode.MFactor = value;
                RedrawTriangles();
                NotifyPropertyChanged();
            }
        }
        

        //bitmaps
        private WriteableBitmap writeableBitmap;
        private WriteableBitmap PictureWriteableBitmap;
        private WriteableBitmap NormalWriteableBitmap;
        private Image image;
        private int widthBitmap;
        private int heightBitmap;
        //color bitmap arrays
        private SimpleColor[,] writeableBitmapColor;
        private SimpleColor[,] PictureBitmapColor;
        private Vector3[,] NormalBitmapVector;
        //color list
        public List<ColorInfo> ColorList { get; set; }
        //bubble
        private int BubbleSquareRadius;


        //property changed
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            SetVariables();
            InitializeComponent();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetVariables()
        {
            triangleFillingMode = new TriangleFillingMode(FillingMode.Direct, true, true, VectorNMode.NormalMap, true, true);
            GridRows = Globals.Rows;
            GridColumns = Globals.Columns;
            TextureFileName = Globals.BitmapSource;
            VectorNFileName = Globals.NormalMapSource;
            VectorNPreviousValue = VectorNMode.Default;
            IsTextureFillPreviousValue = true;
            HasChooseFileReturnedNull = false;
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(VectorLTimer_Elapsed);
            timer.Interval = new TimeSpan(0, 0, 0, 0, Globals.VectorLTimerIntervalMS);
            //timer.Stop();
            TimerPathIndex = 0;

            BubbleSquareRadius = Globals.BubbleRadius * Globals.BubbleRadius;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var props = typeof(Colors).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            var colorInfos = props.Select(prop =>
            {
                var propColor = (Color)prop.GetValue(null, null);
                return new ColorInfo()
                {
                    Name = prop.Name,
                    color = propColor
                };
            });
            LightColorComboBox.ItemsSource = colorInfos;
            LightColorComboBox.SelectedIndex = 0;
            FillColorComboBox.ItemsSource = colorInfos;
            FillColorComboBox.SelectedIndex = 0;
        }
        private void MainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            var PictureBitmap = new BitmapImage(new Uri(Globals.BitmapSource, UriKind.Relative));
            var NormalBitmap = new BitmapImage(new Uri(Globals.NormalMapSource, UriKind.Relative));
            var PWB = new WriteableBitmap(PictureBitmap);
            NormalWriteableBitmap = new WriteableBitmap(NormalBitmap);

            SetTextureBitmap(PWB);
            RedrawTriangles();
        }

        private void GenerateGrid(Canvas canvas)
        {
            PointsArray = new SimplePoint[GridRows + 1, GridColumns + 1];
            EdgesList = new List<Edge>();
            TrianglesList = new List<Triangle>();
            double widthStep = canvas.ActualWidth / (double)GridColumns;
            double heightStep = canvas.ActualHeight / (double)GridRows;
            double pointX = widthStep;
            double pointY = 0d;
            //frame
            for (int i = 0; i < PointsArray.GetLength(0); i++)
            {
                PointsArray[i, 0] = new SimplePoint(0d, pointY, canvas, false);
                PointsArray[i, PointsArray.GetLength(1) - 1] = new SimplePoint(canvas.ActualWidth, pointY, canvas, false);
                pointY += heightStep;
            }
            for (int j = 1; j < PointsArray.GetLength(1) - 1; j++)
            {
                PointsArray[0, j] = new SimplePoint(pointX, 0d, canvas, false);
                PointsArray[PointsArray.GetLength(0) - 1, j] = new SimplePoint(pointX, canvas.ActualHeight, canvas, false);
                pointX += widthStep;
            }
            //middle
            pointX = widthStep;
            pointY = heightStep;
            for (int i = 1; i < PointsArray.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < PointsArray.GetLength(1) - 1; j++)
                {
                    PointsArray[i, j] = new SimplePoint(pointX, pointY, canvas);
                    pointX += widthStep;
                }
                pointX = widthStep;
                pointY += heightStep;
            }
            //edges and triangles
            for (int i = 0; i < PointsArray.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < PointsArray.GetLength(1) - 1; j++)
                {
                    TrianglesList.Add(new Triangle(PointsArray[i, j], PointsArray[i + 1, j], PointsArray[i, j + 1], canvas, triangleFillingMode));
                    TrianglesList.Add(new Triangle(PointsArray[i + 1, j + 1], PointsArray[i, j + 1], PointsArray[i + 1, j], canvas, triangleFillingMode));
                    EdgesList.Add(new Edge(PointsArray[i, j], PointsArray[i, j + 1], canvas));
                    EdgesList.Add(new Edge(PointsArray[i, j], PointsArray[i + 1, j], canvas));
                    EdgesList.Add(new Edge(PointsArray[i + 1, j], PointsArray[i, j + 1], canvas));
                }
            }
            //right and down of frame
            for (int i = 0; i < PointsArray.GetLength(0) - 1; i++)
            {
                EdgesList.Add(new Edge(PointsArray[i, PointsArray.GetLength(1) - 1], PointsArray[i + 1, PointsArray.GetLength(1) - 1], canvas));
            }
            for (int j = 0; j < PointsArray.GetLength(1) - 1; j++)
            {
                EdgesList.Add(new Edge(PointsArray[PointsArray.GetLength(0) - 1, j], PointsArray[PointsArray.GetLength(0) - 1, j + 1], canvas));
            }
        }
        private void RemoveGrid(Canvas canvas)
        {
            canvas.Children.Clear();
            EdgesList?.Clear();
            TrianglesList?.Clear();
        }

        private void GenerateTimerPathList()
        {
            TimerPathList = new List<Vector3>();
            double t = 1;
            int Z = 100;
            int deltaZ = 50;
            
            for (int i = 0; i < 300; i++)
            {
                double X = widthBitmap * Math.Sin(t + 1 * Math.PI /2) + widthBitmap ;
                double Y = heightBitmap * Math.Sin(3 * t) + heightBitmap;
                X /= 2;
                Y /= 2;
                t += 0.05;
                
                Z += deltaZ;
                if (Z == 500)
                    deltaZ *= -1;
                else if (Z == 50)
                    deltaZ *= -1;
                Vector3 v = new Vector3(X, Y, Z).Normalize();
                TimerPathList.Add(v);
            }
            ;
        }

        private void RedrawTriangles()
        {
            Parallel.ForEach(TrianglesList, t =>
             {
                 t.RedrawTriangle();
             });
            //foreach (var t in TrianglesList)
            //{
            //    t.RedrawTriangle();
            //}
            Paint.CopyToWriteableBitmap(writeableBitmap, writeableBitmapColor);
        }
        private void SetNormalVectorBitmap(WriteableBitmap vectorNbitmap = null, IntPoint mousePosition = null)
        {
            //if (timer.IsEnabled == true)
            //{
            //    IsDefaultVectorL = true;
            //}
            if (IsTexturedVectorN == VectorNMode.Default)
            {
                NormalBitmapVector = new Vector3[heightBitmap, widthBitmap];
                for (int i = 0; i < NormalBitmapVector.GetLength(0); i++)
                {
                    for (int j = 0; j < NormalBitmapVector.GetLength(1); j++)
                    {
                        NormalBitmapVector[i, j] = Globals.DefaultVectorN;
                    }
                }
                NormalWriteableBitmap = null;
            }
            else if(IsTexturedVectorN == VectorNMode.NormalMap)
            {
                NormalWriteableBitmap = vectorNbitmap;
                var NormalBitmapColor = new SimpleColor[heightBitmap, widthBitmap];
                if (vectorNbitmap.PixelHeight != heightBitmap || vectorNbitmap.PixelWidth != widthBitmap)
                {
                    var TMPBitmapColor = new SimpleColor[vectorNbitmap.PixelHeight, vectorNbitmap.PixelWidth];
                    Paint.ReadColorsFromBitmap(vectorNbitmap, TMPBitmapColor);
                    for (int y = 0; y < heightBitmap; y++)
                    {
                        for (int x = 0; x < widthBitmap; x++)
                        {
                            NormalBitmapColor[y, x] = TMPBitmapColor[y % vectorNbitmap.PixelHeight, x % vectorNbitmap.PixelWidth];
                        }
                    }
                }
                else
                {
                    Paint.ReadColorsFromBitmap(vectorNbitmap, NormalBitmapColor);
                }
                NormalBitmapVector = SimpleColor.ConvertColorsToVectorsN(NormalBitmapColor);
            }
            else
            {
                //assumming Vector Table is ProperSize
                for (int y = 0; y < NormalBitmapVector.GetLength(0); y++)
                {
                    for (int x = 0; x < NormalBitmapVector.GetLength(1); x++)
                    {
                        if(mousePosition.SquareLength(ref x, ref y) <= BubbleSquareRadius)
                        {
                            int dx = x - mousePosition.X;
                            int dy = mousePosition.Y - y;
                            double h = Math.Sqrt(dx * dx + dy * dy + BubbleSquareRadius);
                            NormalBitmapVector[y, x].X = dx;
                            NormalBitmapVector[y, x].Y = dy;
                            NormalBitmapVector[y, x].Z = h;
                            NormalBitmapVector[y, x].NormalizeInPlace();
                        }
                        else
                        {
                            NormalBitmapVector[y, x].X = 0;
                            NormalBitmapVector[y, x].Y = 0;
                            NormalBitmapVector[y, x].Z = 1;
                        }
                    }
                }
            }
            triangleFillingMode.NormalBitmapVector = NormalBitmapVector;
        }
        private void SetTextureBitmap(WriteableBitmap textureBitmap = null)
        {
            if (timer.IsEnabled == true)
            {
                IsDefaultVectorL = true;
            }
            if (IsTextureFill == false)
            {
                //AnotherTextureFillRadioButtonClick = false;
                widthBitmap = Globals.DefaultBitmapWidth;
                heightBitmap = Globals.DefaultBitmapHeight;
                PictureWriteableBitmap = new WriteableBitmap(widthBitmap, heightBitmap, 96, 96, PixelFormats.Bgra32, null);

                var colInfo = FillColorComboBox.SelectedItem as ColorInfo;
                Paint.FillBitmap(PictureWriteableBitmap, colInfo.color);
            }
            else
            {
                //AnotherTextureFillRadioButtonClick = true;
                PictureWriteableBitmap = textureBitmap;
                widthBitmap = PictureWriteableBitmap.PixelWidth;
                heightBitmap = PictureWriteableBitmap.PixelHeight;
            }
            RemoveGrid(MainCanvas);

            writeableBitmap = new WriteableBitmap(PictureWriteableBitmap);

            writeableBitmapColor = new SimpleColor[heightBitmap, widthBitmap];
            PictureBitmapColor = new SimpleColor[heightBitmap, widthBitmap];
            Paint.ReadColorsFromBitmap(PictureWriteableBitmap, PictureBitmapColor);
            Paint.ReadColorsFromBitmap(writeableBitmap, writeableBitmapColor);

            triangleFillingMode.writeableBitmap = writeableBitmap;
            triangleFillingMode.PictureBitmapColor = PictureBitmapColor;
            triangleFillingMode.writeableBitmapColor = writeableBitmapColor;

            MainCanvas.Width = widthBitmap;
            MainCanvas.Height = heightBitmap;
            this.MaxHeight = heightBitmap + 4 * Globals.GridMargin;


            image = new Image
            {
                Source = writeableBitmap,
                IsHitTestVisible = false,
                Width = widthBitmap,
                Height = heightBitmap
            };
            Panel.SetZIndex(image, Globals.BitmapZIndex);
            MainCanvas.Children.Add(image);
            MainCanvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            MainCanvas.Arrange(new Rect(0, 0, MainCanvas.DesiredSize.Width, MainCanvas.DesiredSize.Height));

            SetNormalVectorBitmap(NormalWriteableBitmap);

            
            GenerateGrid(MainCanvas);

            GenerateTimerPathList();
        }
        private WriteableBitmap ChooseBitmapFile(bool IsNormalMap)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmaps (*.bmp)|*.bmp|All files (*.*)|*.*";
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (IsNormalMap == true)
            { 
                string path = System.IO.Path.GetFullPath(Globals.NormalMapSource);
                string file = System.IO.Path.GetFileName(Globals.NormalMapSource);
                path = path.Substring(0, path.Length - file.Length);
                openFileDialog.InitialDirectory = path;
            }
            else
            {
                string path = System.IO.Path.GetFullPath(Globals.BitmapSource);
                string file = System.IO.Path.GetFileName(Globals.BitmapSource);
                path = path.Substring(0, path.Length - file.Length);
                openFileDialog.InitialDirectory = path;
            }
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage OpenedBitmap;
                string filename;
                try
                {
                    filename = openFileDialog.FileName;
                    OpenedBitmap = new BitmapImage(new Uri(filename, UriKind.Absolute));
                }
                catch(Exception)
                {
                    MessageBox.Show("Could not open file!", Globals.WindowName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return null;
                }

                if(IsNormalMap == true)
                {
                    VectorNFileName = filename;
                }
                else
                {
                    TextureFileName = filename;
                }

                var openedWriteableBitmap = new WriteableBitmap(OpenedBitmap);

                return openedWriteableBitmap;
            }
            return null;
        }
        private WriteableBitmap OpenBitmapFromFile(string path)
        {
            BitmapImage OpenedBitmap;
            try
            {
                OpenedBitmap = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            }
            catch(Exception)
            {
                return null;
            }
            return new WriteableBitmap(OpenedBitmap);
        }
        private void RenewTrianglesRand()
        {
            foreach (var t in TrianglesList)
            {
                t.RenewRand();
            }
        }
        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            RenewTrianglesRand();
            var starttime = DateTime.Now;
            RedrawTriangles();
            var endtime = DateTime.Now;
            Debug.WriteLine($"Time: {endtime - starttime}");
        }

        private void SetGridSizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (GridRows > 0 && GridColumns > 0)
            {
                RemoveGrid(MainCanvas);
                MainCanvas.Children.Add(image);
                GenerateGrid(MainCanvas);              
                RedrawTriangles();
            }
        }
        private void VectorNFileButton_Click(object sender, RoutedEventArgs e)
        {
            var vectorNbitmap = ChooseBitmapFile(true);
            if (vectorNbitmap == null)
            {
                if (VectorNPreviousValue != VectorNMode.NormalMap)
                {
                    HasChooseFileReturnedNull = true;
                    IsTexturedVectorN = VectorNPreviousValue;
                }
                return;
            }
            HasChooseFileReturnedNull = false;
            SetNormalVectorBitmap(vectorNbitmap);
            RedrawTriangles();
        }
        private void VectorNFileRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if(VectorNPreviousValue != VectorNMode.NormalMap)
            {
                var vectorNbitmap = OpenBitmapFromFile(VectorNFileName);
                while(vectorNbitmap == null)
                {
                    vectorNbitmap = ChooseBitmapFile(true);
                }
                SetNormalVectorBitmap(vectorNbitmap);
                RedrawTriangles();
            }
        }
        private void GridTextureFileButton_Click(object sender, RoutedEventArgs e)
        {
            var textureBitmap = ChooseBitmapFile(false);
            if (textureBitmap == null)
            {
                if (IsTextureFillPreviousValue == false)
                {
                    HasChooseFileReturnedNull = true;
                    IsTextureFill = false;
                }
                return;
            }
            HasChooseFileReturnedNull = false;
            SetTextureBitmap(textureBitmap);
            RedrawTriangles();
        }
        private void GridTextureFileRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsTextureFillPreviousValue == false)
            {
                var texturebitmap = OpenBitmapFromFile(TextureFileName);
                while (texturebitmap == null)
                {
                    texturebitmap = ChooseBitmapFile(false);
                }
                SetTextureBitmap(texturebitmap);
                RedrawTriangles();
            }
        }
        private void FillColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsTextureFill == false)
            {
                SetTextureBitmap();
                RedrawTriangles();
            }
        }
        private void LightColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsDefaultLightColor == false)
            {
                var colInfo = LightColorComboBox.SelectedItem as ColorInfo;
                triangleFillingMode.LightVector = new SimpleColor(colInfo.color).ConvertColorToVector();
                RedrawTriangles();
            }
        }

        private void VectorLTimer_Elapsed(object sender, EventArgs e)
        {
            //Debug.WriteLine($"Light vector [{TimerPathList[TimerPathIndex].X},{TimerPathList[TimerPathIndex].Y},{TimerPathList[TimerPathIndex].Z}]");
            VectorLProperty = TimerPathList[TimerPathIndex];
            TimerPathIndex++;
            TimerPathIndex %= TimerPathList.Count;
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsTexturedVectorN == VectorNMode.Bubble)
            {
                Point pos = e.GetPosition(MainCanvas);
                IntPoint mousePos = new IntPoint(pos.X, pos.Y);
                SetNormalVectorBitmap(null, mousePos);
                if (timer.IsEnabled == false)
                {
                    RedrawTriangles();
                }
            }
        }
    }
}
