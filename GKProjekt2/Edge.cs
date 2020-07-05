using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GKProjekt2
{
    public class Edge
    {
        public SimplePoint First { get; set; }
        public SimplePoint Second { get; set; }
        public Canvas canvas { get; set; }
        public Line line { get; set; }

        public Edge(SimplePoint first,SimplePoint second, Canvas canvas)
        {
            First = first;
            Second = second;
            this.canvas = canvas;
            Draw();
            First.PropertyChanged += MoveWithPoints;
            Second.PropertyChanged += MoveWithPoints;
        }

        private void Draw()
        {
            line = new Line
            {
                X1 = First.X,
                Y1 = First.Y,
                X2 = Second.X,
                Y2 = Second.Y,
                StrokeThickness = Globals.LineThickness,
                Stroke = new SolidColorBrush(Globals.LineColor)
            };
            Panel.SetZIndex(line, Globals.LineZIndex);
            canvas.Children.Add(line);            
        }

        private void MoveWithPoints(object sender, EventArgs eventArgs)
        {
            line.X1 = First.X;
            line.Y1 = First.Y;
            line.X2 = Second.X;
            line.Y2 = Second.Y;
        }
    }
}
