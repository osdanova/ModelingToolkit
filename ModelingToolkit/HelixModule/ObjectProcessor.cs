using HelixToolkit.Wpf;
using ModelingToolkit.Objects;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ModelingToolkit.HelixModule
{
    // Processes Object data and turns it into visuals for the viewport
    public class ObjectProcessor
    {
        public static ModelVisual3D GetVisualFromModel(MtModel model)
        {
            Model3DGroup meshGroup = new Model3DGroup();

            for (int i = 0; i < model.Meshes.Count; i++)
            {
                MtMesh mesh = model.Meshes[i];

                if (!mesh.IsVisible || !mesh.IsMeshVisible) {
                    continue;
                }

                // MESH
                GeometryModel3D myGeometryModel = new GeometryModel3D();

                // GEOMETRY
                MeshGeometry3D myMeshGeometry3D = new MeshGeometry3D();
                Point3DCollection myPositionCollection = new Point3DCollection();
                PointCollection myTextureCoordinatesCollection = new PointCollection();
                Int32Collection myTriangleIndicesCollection = new Int32Collection();


                foreach (MtVertex vertex in mesh.Vertices)
                {
                    myPositionCollection.Add(new Point3D(vertex.AbsolutePosition.Value.X, vertex.AbsolutePosition.Value.Y, vertex.AbsolutePosition.Value.Z));
                    float x = vertex.TextureCoordinates.Value.X;
                    float y = 1f - vertex.TextureCoordinates.Value.Y;
                    if (x < 0 || x > 1) {
                        x = x - (int)x;
                    }
                    if (y < 0 || y > 1) {
                        y = y - (int)y;
                    }
                    myTextureCoordinatesCollection.Add(new System.Windows.Point(x, y));
                }

                foreach (MtFace face in mesh.Faces)
                {
                    if (face.Clockwise)
                    {
                        myTriangleIndicesCollection.Add(face.VertexIndices[0]);
                        myTriangleIndicesCollection.Add(face.VertexIndices[1]);
                        myTriangleIndicesCollection.Add(face.VertexIndices[2]);
                    }
                    else
                    {
                        myTriangleIndicesCollection.Add(face.VertexIndices[0]);
                        myTriangleIndicesCollection.Add(face.VertexIndices[2]);
                        myTriangleIndicesCollection.Add(face.VertexIndices[1]);
                    }
                }

                myMeshGeometry3D.Positions = myPositionCollection;
                myMeshGeometry3D.TextureCoordinates = myTextureCoordinatesCollection;
                myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

                myGeometryModel.Geometry = myMeshGeometry3D;

                DiffuseMaterial myMaterial = Visuals.GetDefaultMaterial();
                try
                {
                    if(model.Materials[mesh.MaterialId.Value].DiffuseTextureBitmap != null)
                    {
                        myMaterial = new DiffuseMaterial
                        {
                            Brush = new ImageBrush(model.Materials[mesh.MaterialId.Value].GetAsBitmapImage()),
                        };
                    }
                    else
                    {
                        myMaterial = Visuals.GetDefaultMaterial();
                    }
                }
                catch (Exception e)
                {
                    myMaterial = Visuals.GetDefaultMaterial();
                }
                myGeometryModel.Material = myMaterial;

                // Magic vertices so that the UV mapping goes from 0 to 1 instead of scaling to maximum existing
                myPositionCollection.Add(new Point3D(0, 0, 0));
                myPositionCollection.Add(new Point3D(0, 0, 0));
                myTextureCoordinatesCollection.Add(new System.Windows.Point(0, 0));
                myTextureCoordinatesCollection.Add(new System.Windows.Point(1, 1));

                meshGroup.Children.Add(myGeometryModel);
            }

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = meshGroup;
            HelixUtils.ApplyRotationForViewport(visual);
            return visual;
        }

        public static List<LinesVisual3D> GetWireframeFromModel(MtModel model)
        {
            List<LinesVisual3D> wireframe = new List<LinesVisual3D>();
            for(int i = 0; i < model.Meshes.Count; i++)
            {
                MtMesh mesh = model.Meshes[i];
                if (mesh.IsVisible && mesh.IsWireframeVisible)
                {
                    wireframe.AddRange(GetWireframeFromMesh(mesh));
                }
            }

            return wireframe;
        }

        public static List<LinesVisual3D> GetWireframeFromMesh(MtMesh mesh)
        {
            List<LinesVisual3D> wireframe = new List<LinesVisual3D>();
            for(int i = 0; i < mesh.Faces.Count; i++)
            {
                MtFace face = mesh.Faces[i];
                List<Vector3> vertexPositions = new List<Vector3>();
                foreach (int vertexIndex in face.VertexIndices)
                {
                    vertexPositions.Add(mesh.Vertices[vertexIndex].AbsolutePosition.Value);
                }

                LinesVisual3D line = new LinesVisual3D();
                line.Points.Add(new Point3D(vertexPositions[0].X, vertexPositions[0].Y, vertexPositions[0].Z));
                line.Points.Add(new Point3D(vertexPositions[1].X, vertexPositions[1].Y, vertexPositions[1].Z));
                line.Points.Add(new Point3D(vertexPositions[1].X, vertexPositions[1].Y, vertexPositions[1].Z));
                line.Points.Add(new Point3D(vertexPositions[2].X, vertexPositions[2].Y, vertexPositions[2].Z));
                line.Points.Add(new Point3D(vertexPositions[2].X, vertexPositions[2].Y, vertexPositions[2].Z));
                line.Points.Add(new Point3D(vertexPositions[0].X, vertexPositions[0].Y, vertexPositions[0].Z));
                line.Color = Colors.Blue;
                line.Thickness = 2;

                HelixUtils.ApplyRotationForViewport(line);

                wireframe.Add(line);
            }

            return wireframe;
        }

        public static List<LinesVisual3D> GetBonesFromModel(MtModel model)
        {
            List<LinesVisual3D> bones = new List<LinesVisual3D>();
            for (int i = 0; i < model.Joints.Count; i++)
            {
                MtJoint joint = model.Joints[i];
                if (!joint.IsVisible || joint.ParentId == null) {
                    continue;
                }

                Vector3 translation = (Vector3)joint.AbsoluteTranslation;
                Vector3 parentTranslation = (Vector3)model.Joints[(int)joint.ParentId].AbsoluteTranslation;

                LinesVisual3D bone = Visuals.GetLineVisual(translation, parentTranslation, 2);

                HelixUtils.ApplyRotationForViewport(bone);

                bones.Add(bone);
            }

            return bones;
        }

        public static List<BillboardVisual3D> GetJointsFromModel(MtModel model)
        {
            List<BillboardVisual3D> bones = new List<BillboardVisual3D>();
            for (int i = 0; i < model.Joints.Count; i++)
            {
                MtJoint joint = model.Joints[i];

                if (!joint.IsVisible) {
                    continue;
                }

                RectangleVisual3D rectVis = new RectangleVisual3D
                {
                    Width = 1,
                    Length = 1,
                    Fill = new SolidColorBrush(Colors.White),
                };


                BillboardVisual3D jointBB = new BillboardVisual3D();
                jointBB.Position = new Point3D(joint.AbsoluteTranslation.Value.X, joint.AbsoluteTranslation.Value.Y, joint.AbsoluteTranslation.Value.Z);
                jointBB.Width = 10;
                jointBB.Height = 10;
                jointBB.Material = new DiffuseMaterial(new SolidColorBrush(Colors.White));
                jointBB.DepthOffset = 0.1;
                
                HelixUtils.ApplyRotationForViewport(jointBB);
                bones.Add(jointBB);
            }

            return bones;
        }
    }
}
