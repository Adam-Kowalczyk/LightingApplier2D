using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingApplier2D
{
    public class FillingTriangle
    {
        public FillingTriangle(DragablePoint p1, DragablePoint p2, DragablePoint p3, Random generator)
        {
            Points.Add(p1);
            Points.Add(p2);
            Points.Add(p3);
            Kd = generator.NextDouble();
            Ks = generator.NextDouble();
            M = generator.Next(1, 100);
        }
        public ObservableCollection<DragablePoint> Points { get; set; } = new ObservableCollection<DragablePoint>();

        public double Kd { get; set; }
        public double Ks { get; set; }
        public int M { get; set; }

    }
}
