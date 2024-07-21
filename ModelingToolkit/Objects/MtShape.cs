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
        public System.Windows.Media.Color ShapeColor { get; set; }
        public bool IsVisible { get; set; } // For the viewport


        public enum ShapeType
        {
            cube,
            box,
            column,
            sphere,
            ellipsoid,
            boundingBox
        }

        public ModelVisual3D GetVisual()
        {
            if (Type == ShapeType.cube)
            {
                return Visuals.GetBoxVisual(Position, Height, Width, Depth, ShapeColor);
            }
            else if (Type == ShapeType.box)
            {
                return Visuals.GetBoxVisual(Position, Height, Height, Height, ShapeColor);
            }
            else if (Type == ShapeType.column)
            {
                return Visuals.GetColumnVisual(Position, Height, Width, ShapeColor);
            }
            else if (Type == ShapeType.sphere)
            {
                return Visuals.GetEllipsoidVisual(Position, Height, Height, Height, ShapeColor);
            }
            else if (Type == ShapeType.ellipsoid)
            {
                return Visuals.GetEllipsoidVisual(Position, Height, Width, Depth, ShapeColor);
            }
            else if (Type == ShapeType.boundingBox)
            {
                return Visuals.GetBoundingBoxVisual(Position.X, Position.Y, Position.Z, Height, Width, Depth, ShapeColor);
            }
            else
                throw new System.Exception("[MtShape.GetVisual] INVALID TYPE");
        }

        public static MtShape CreateCube(Vector3 position, float size)
        {
            MtShape shape = new MtShape();
            shape.Type = ShapeType.cube;
            shape.Position = position;
            shape.Height = size;
            shape.IsVisible = true;

            return shape;
        }

        public static MtShape CreateBox(Vector3 position, float height, float width, float depth)
        {
            MtShape shape = new MtShape();
            shape.Type = ShapeType.box;
            shape.Position = position;
            shape.Height = height;
            shape.Width = width;
            shape.Depth = depth;
            shape.IsVisible = true;

            return shape;
        }

        public static MtShape CreateColumn(Vector3 position, float height, float width)
        {
            MtShape shape = new MtShape();
            shape.Type = ShapeType.column;
            shape.Position = position;
            shape.Height = height;
            shape.Width = width;
            shape.IsVisible = true;

            return shape;
        }

        public static MtShape CreateSphere(Vector3 position, float diameter)
        {
            MtShape shape = new MtShape();
            shape.Type = ShapeType.sphere;
            shape.Position = position;
            shape.Height = diameter;
            shape.IsVisible = true;

            return shape;
        }

        public static MtShape CreateEllipsoid(Vector3 position, float height, float width, float depth)
        {
            MtShape shape = new MtShape();
            shape.Type = ShapeType.ellipsoid;
            shape.Position = position;
            shape.Height = height;
            shape.Width = width;
            shape.Depth = depth;
            shape.IsVisible = true;

            return shape;
        }

        public static MtShape CreateBoundingBox(Vector3 position, float height, float width, float depth)
        {
            MtShape shape = new MtShape();
            shape.Type = ShapeType.boundingBox;
            shape.Position = position;
            shape.Height = height;
            shape.Width = width;
            shape.Depth = depth;
            shape.IsVisible = true;

            return shape;
        }
    }
}
