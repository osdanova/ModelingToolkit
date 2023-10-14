using System.Collections.Generic;
using System.Numerics;
using System.Windows.Media.Media3D;

namespace ModelingToolkit.Objects
{
    public class MtModel
    {
        public string Name { get; set; }
        public List<string> Labels { get; set; }
        public List<MtJoint> Joints { get; set; }
        public List<MtMesh> Meshes { get; set; }
        public List<MtMaterial> Materials { get; set; }
        public bool IsVisible { get; set; } // For the viewport

        public MtModel()
        {
            Name = null;
            Labels = new List<string>();
            Joints = new List<MtJoint>();
            Meshes = new List<MtMesh>();
            Materials = new List<MtMaterial>();
            IsVisible = true;
        }

        public void CalculateJointAbsoluteMatrices()
        {
            for (int i = 0; i < Joints.Count; i++)
            {
                MtJoint joint = Joints[i];
                Matrix4x4 absoluteMatrix = (Matrix4x4)joint.RelativeTransformationMatrix;
                if (joint.ParentId != null) {
                    absoluteMatrix *= (Matrix4x4)Joints[(int)joint.ParentId].AbsoluteTransformationMatrix;
                }

                joint.AbsoluteTransformationMatrix = absoluteMatrix;
                joint.Decompose();
            }
        }

        public Rect3D GetBoundingBox()
        {
            float minX = 0;
            float minY = 0;
            float minZ = 0;
            float maxX = 0;
            float maxY = 0;
            float maxZ = 0;
            foreach(MtMesh mesh in Meshes)
            {
                foreach (MtVertex vertex in mesh.Vertices)
                {
                    if (minX > vertex.AbsolutePosition.Value.X) minX = vertex.AbsolutePosition.Value.X;
                    if (minY > vertex.AbsolutePosition.Value.Y) minY = vertex.AbsolutePosition.Value.Y;
                    if (minZ > vertex.AbsolutePosition.Value.Z) minZ = vertex.AbsolutePosition.Value.Z;
                    if (maxX < vertex.AbsolutePosition.Value.X) maxX = vertex.AbsolutePosition.Value.X;
                    if (maxY < vertex.AbsolutePosition.Value.Y) maxY = vertex.AbsolutePosition.Value.Y;
                    if (maxZ < vertex.AbsolutePosition.Value.Z) maxZ = vertex.AbsolutePosition.Value.Z;
                }
            }

            Rect3D boundingBox = new Rect3D();
            boundingBox.SizeX = maxX - minX;
            boundingBox.SizeY = maxY - minY;
            boundingBox.SizeZ = maxZ - minZ;
            double halfSizeX = boundingBox.SizeX / 2;
            double halfSizeY = boundingBox.SizeY / 2;
            double halfSizeZ = boundingBox.SizeZ / 2;
            boundingBox.X = maxX - halfSizeX;
            boundingBox.Y = maxY - halfSizeY;
            boundingBox.Z = maxZ - halfSizeZ;

            return boundingBox;
        }
    }
}
