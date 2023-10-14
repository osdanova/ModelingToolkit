using HelixToolkit.Wpf;
using System.Numerics;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ModelingToolkit.HelixModule
{
    // Creates basic Visual shapes
    public class Visuals
    {
        // DEFAULTS
        public static DiffuseMaterial GetDefaultMaterial()
        {
            LinearGradientBrush myHorizontalGradient = new LinearGradientBrush();
            myHorizontalGradient.StartPoint = new System.Windows.Point(0, 0.5);
            myHorizontalGradient.EndPoint = new System.Windows.Point(1, 0.5);
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Red, 0.25));
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.Blue, 0.75));
            myHorizontalGradient.GradientStops.Add(new GradientStop(Colors.LimeGreen, 1.0));

            return new DiffuseMaterial(myHorizontalGradient);
        }

        public static BoxVisual3D GetOrigin(float size = 1)
        {
            return GetBoxVisual(new Vector3(), size, size, size);
        }

        // VISUAL GETTERS
        public static LinesVisual3D GetLineVisual(Vector3 startPoint, Vector3 endPoint, int thickness = 1)
        {
            LinesVisual3D line = new LinesVisual3D();
            line.Points.Add(new Point3D(startPoint.X, startPoint.Y, startPoint.Z)); // Start point of the bone
            line.Points.Add(new Point3D(endPoint.X, endPoint.Y, endPoint.Z)); // End point of the bone
            line.Thickness = thickness;
            line.Color = Colors.Red; // Set the color of the line
            line.DepthOffset = 1;

            return line;
        }

        
        public static BoundingBoxWireFrameVisual3D GetBoundingBoxVisual(double positionX, double positionY, double positionZ, double sizeX, double sizeY, double sizeZ, Color? color = null, float thickness = 1)
        {
            return GetBoundingBoxVisual(new Rect3D(positionX, positionY, positionZ, sizeX, sizeY, sizeZ), color, thickness);
        }
        public static BoundingBoxWireFrameVisual3D GetBoundingBoxVisual(Rect3D boundingBox, Color? color = null, float thickness = 1)
        {
            boundingBox.X -= boundingBox.SizeX / 2;
            boundingBox.Y -= boundingBox.SizeY / 2;
            boundingBox.Z -= boundingBox.SizeZ / 2;
            return new BoundingBoxWireFrameVisual3D
            {
                BoundingBox = boundingBox,
                Thickness = thickness,
                Color = (color == null) ? Color.FromArgb(255, 255, 204, 0) : color.Value
            };
        }

        public static BoxVisual3D GetBoxVisual(Vector3 position, float height, float width, float depth, Color? color = null)
        {
            return new BoxVisual3D
            {
                Center = new Point3D(position.X, position.Y, position.Z),
                Height = height,
                Width = width,
                Length = depth,
                Material = (color == null) ? GetDefaultMaterial() : new DiffuseMaterial(new SolidColorBrush(color.Value))
            };
        }

        public static EllipsoidVisual3D GetEllipsoidVisual(Vector3 position, float height, float width, float depth, Color? color = null)
        {
            return new EllipsoidVisual3D
            {
                Center = new Point3D(position.X, position.Y, position.Z),
                RadiusX= width,
                RadiusY= height,
                RadiusZ= depth,
                Material = (color == null) ? GetDefaultMaterial() : new DiffuseMaterial(new SolidColorBrush(color.Value))
            };
        }

        public static GridLinesVisual3D GetGridLinesVisual(Vector3 position, float width, float length, float distance = 100, Color? color = null, float thickness = 0.1f)
        {
            return new GridLinesVisual3D
            {
                Center = new Point3D(position.X, position.Y, position.Z),
                Width = width,
                Length = length,
                Thickness = thickness,
                MajorDistance = distance,
                MinorDistance = distance,
                Material = (color == null) ? new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(150, 200, 200, 200))) : new DiffuseMaterial(new SolidColorBrush(color.Value))
            };
        }
    }
}
