using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace ModelingToolkit.Objects
{
    public class MtMesh
    {
        public string Name { get; set; }
        public List<string> Labels { get; set; }
        public int? MaterialId { get; set; }
        public List<MtVertex> Vertices { get; set; }
        public List<MtFace> Faces { get; set; }
        public List<MtTriangleStrip> TriangleStrips { get; set; }
        public bool IsVisible { get; set; } // For the viewport
        public bool IsMeshVisible { get; set; } // For the viewport
        public bool IsWireframeVisible { get; set; } // For the viewport

        public MtMesh()
        {
            Name = null;
            Labels = new List<string>();
            MaterialId = null;
            Vertices = new List<MtVertex>();
            TriangleStrips = new List<MtTriangleStrip>();
            Faces = new List<MtFace>();
            IsVisible = true;
            IsMeshVisible = true;
            IsWireframeVisible = true;
        }

        public Rect3D GetBoundingBox()
        {
            float minX = 0;
            float minY = 0;
            float minZ = 0;
            float maxX = 0;
            float maxY = 0;
            float maxZ = 0;
            foreach (MtVertex vertex in Vertices)
            {
                if (minX > vertex.AbsolutePosition.Value.X) minX = vertex.AbsolutePosition.Value.X;
                if (minY > vertex.AbsolutePosition.Value.Y) minY = vertex.AbsolutePosition.Value.Y;
                if (minZ > vertex.AbsolutePosition.Value.Z) minZ = vertex.AbsolutePosition.Value.Z;
                if (maxX < vertex.AbsolutePosition.Value.X) maxX = vertex.AbsolutePosition.Value.X;
                if (maxY < vertex.AbsolutePosition.Value.Y) maxY = vertex.AbsolutePosition.Value.Y;
                if (maxZ < vertex.AbsolutePosition.Value.Z) maxZ = vertex.AbsolutePosition.Value.Z;
            }

            Rect3D boundingBox = new Rect3D();
            boundingBox.SizeX = maxX - minX;
            boundingBox.SizeY = maxY - minY;
            boundingBox.SizeZ = maxZ - minZ;
            boundingBox.Offset(maxX - (boundingBox.SizeX / 2), maxY - (boundingBox.SizeY / 2), maxZ - (boundingBox.SizeZ / 2));

            return boundingBox;
        }

        public void SetFaceVertices()
        {
            foreach(MtFace face in Faces)
            {
                face.Vertices.Add(Vertices[face.VertexIndices[0]]);
                face.Vertices.Add(Vertices[face.VertexIndices[1]]);
                face.Vertices.Add(Vertices[face.VertexIndices[2]]);
            }
        }

        public void BuildTriangleStrips()
        {
            TriangleStrips.Clear();
            SetFaceVertices();

            for (int i = 0; i < Faces.Count; i++)
            {
                MtFace iFace = Faces[i];

                // If there are no strips, create the first one
                if(TriangleStrips.Count == 0) {
                    TriangleStrips.Add(new MtTriangleStrip());
                }
                MtTriangleStrip triStrip = TriangleStrips[TriangleStrips.Count - 1];

                bool faceAdded = triStrip.AddFace(iFace);
                if(faceAdded){
                    continue;
                }
                else
                {
                    TriangleStrips.Add(new MtTriangleStrip());
                    triStrip = TriangleStrips[TriangleStrips.Count - 1];
                    bool faceAdded2 = triStrip.AddFace(iFace);
                    if (!faceAdded2) {
                        throw new System.Exception("Couldn't add a face to a tri strip. The tri strip algorythm is wrong");
                    }
                }
            }
        }
    }
}
