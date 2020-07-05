using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GKProjekt2
{
    public class SimplePoint
    {
        private double _x;
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                NotifyPropertyChanged();
            }
        }
        private double _y;
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                NotifyPropertyChanged();
            }
        }

        private bool IsDraggingOn = false;

        public event EventHandler PropertyChanged;

        public Canvas canvas { get; set; }

        public Rectangle rect { get; set; }

        public SimplePoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public SimplePoint(double x, double y, Canvas canvas, bool clickable = true)
        {
            X = x;
            Y = y;
            this.canvas = canvas;
            if (clickable == true)
            {
                Draw();
            }
        }

        private void Draw()
        {
            rect = new Rectangle
            {
                Height = Globals.PointSize,
                Width = Globals.PointSize,
                Fill = new SolidColorBrush(Globals.PointColor)
            };
            rect.MouseLeftButtonDown += Rect_MouseLeftButtonDown;
            rect.MouseLeftButtonUp += Rect_MouseLeftButtonUp;
            rect.MouseMove += Rect_MouseMove;
            rect.MouseEnter += Rect_MouseEnter;
            rect.MouseLeave += Rect_MouseLeave;
            canvas.MouseMove += Rect_MouseMove;
            canvas.MouseLeftButtonUp += Rect_MouseLeftButtonUp;
            Canvas.SetLeft(rect, X - Globals.PointSize / 2);
            Canvas.SetTop(rect, Y - Globals.PointSize / 2);
            Panel.SetZIndex(rect, Globals.PointZIndex);
            canvas.Children.Add(rect);
        }

        private void MovePoint(double x, double y)
        {
            X = x;
            Y = y;
            Canvas.SetLeft(rect, X - Globals.PointSize / 2);
            Canvas.SetTop(rect, Y - Globals.PointSize / 2);
        }

        private void NotifyPropertyChanged()
        {
            PropertyChanged?.Invoke(this, null);
        }
        private void Rect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Cursor = Cursors.Hand;
            IsDraggingOn = true;
        }
        private void Rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            canvas.Cursor = Cursors.Arrow;
            IsDraggingOn = false;
        }
        private void Rect_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton != MouseButtonState.Pressed)
            {
                IsDraggingOn = false;
                return;
            }
            if (IsDraggingOn == true)
            {
                canvas.Cursor = Cursors.Hand;
                Point p = e.GetPosition(canvas);
                if (p.X < 0 || p.X > canvas.ActualWidth || p.Y < 0 || p.Y > canvas.ActualHeight)
                    return;
                MovePoint(p.X, p.Y);
            }
        }
        private void Rect_MouseLeave(object sender, MouseEventArgs e)
        {
            canvas.Cursor = Cursors.Arrow;
        }
        private void Rect_MouseEnter(object sender, MouseEventArgs e)
        {
            canvas.Cursor = Cursors.Hand;
        }

        public override string ToString()
        {
            return $"({X};{Y})";
        }

        public SimplePoint Copy()
        {
            return new SimplePoint(X, Y);
        }
    }
}
