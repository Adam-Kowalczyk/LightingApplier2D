using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LightingApplier2D
{
    public class LightSource
    {

        public LightSource(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }
        public Vector3 Position { get; set; }
        public Vector3 UnitToLight(float x, float y)
        {
            var cal = Vector3.Subtract(Position, new Vector3(x, y, 0));
            return Vector3.Normalize(cal);
        }

        public Color Color { get; set; }
    }
}
