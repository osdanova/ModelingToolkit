using System.Windows.Media.Media3D;

namespace ModelingToolkit.HelixModule
{
    public class HelixUtils
    {
        // Rotations to correclty display models in the viewport
        public static AxisAngleRotation3D RotationX = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90);
        public static AxisAngleRotation3D RotationZ = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 90);
        public static void ApplyRotationForViewport (ModelVisual3D visual)
        {
            // Create a transformation group and add the rotation
            var transformationGroup = new Transform3DGroup();
            transformationGroup.Children.Add(new RotateTransform3D(RotationX));
            transformationGroup.Children.Add(new RotateTransform3D(RotationZ));

            // Apply the transformation to your 3D model
            visual.Transform = transformationGroup;
        }
    }
}
