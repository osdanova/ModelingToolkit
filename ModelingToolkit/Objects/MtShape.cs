using ModelingToolkit.HelixModule;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Media.Media3D;

namespace ModelingToolkit.Objects
{
    public class MtShape
    {
        public string Name { get; set; }
        public List<string> Labels { get; set; }
        public ShapeType Type { get; set; }
        public Vector3 Position { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }
        public float Subdivisions { get; set; } // Unused currently

        public enum ShapeType
        {
            cube,
            box,
            sphere,
            ellipsoid
        }

        public ModelVisual3D GetVisual(System.Windows.Media.Color? color = null)
        {
            if (Type == ShapeType.cube || Type == ShapeType.box)
            {
                return Visuals.GetBoxVisual(Position, Height, Width, Depth, color);
            }
            else if (Type == ShapeType.sphere || Type == ShapeType.ellipsoid)
            {
                return Visuals.GetEllipsoidVisual(Position, Height, Width, Depth, color);
            }
            else throw new System.Exception("[MtShape.GetVisual] INVALID TYPE");
        }
    }
}
