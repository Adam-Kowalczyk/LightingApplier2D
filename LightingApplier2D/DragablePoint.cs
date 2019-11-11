using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingApplier2D
{
    public class DragablePoint:Observable
    {
        public DragablePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                //OnPropertyChanged(nameof(X));
            }
        }
        int x;

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                //OnPropertyChanged(nameof(Y));
            }
        }
        int y;

        public bool IsHit(double xPos, double yPos, int margin)
        {
            return Math.Sqrt((xPos - X) * (xPos - X) + (yPos - Y) * (yPos - Y)) < margin;
        }

        public double Distance(double xPos, double yPos)
        {
            return Math.Sqrt((xPos - X) * (xPos - X) + (yPos - Y) * (yPos - Y));
        }
    }
}
