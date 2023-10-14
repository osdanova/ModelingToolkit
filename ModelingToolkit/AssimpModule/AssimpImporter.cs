using ModelingToolkit.Objects;
using System.Collections.Generic;
using System.IO;

namespace ModelingToolkit.AssimpModule
{
    public class AssimpImporter
    {
        public static MtModel ImportScene(string filePath)
        {
            string directory = Path.GetDirectoryName(filePath);

            Assimp.AssimpContext assimp = new Assimp.AssimpContext();
            Assimp.Scene scene = assimp.ImportFile(filePath);

            MtModel model = new MtModel();

            // SKELETON
            List<Assimp.Node> nodeList = AssimpUtils.GetNodeList(scene);

            // Get joint data. Assimp stores it in relative-to-parent format
            List<MtJoint> joints = new List<MtJoint>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                Assimp.Node node = nodeList[i];
                MtJoint joint = new MtJoint();
                joint.Name = node.Name;
                joint.RelativeTransformationMatrix = AssimpUtils.ToNumerics(node.Transform);
                if (node.Parent != null && nodeList.IndexOf(node.Parent) != -1)
                {
                    joint.ParentId = nodeList.IndexOf(node.Parent);
                }
                joint.Decompose();
                joints.Add(joint);
            }

            // Calculate absolute data as well
            model.Joints = joints;
            model.CalculateJointAbsoluteMatrices();

            // MATERIALS / TEXTURES
            for (int i = 0; i < scene.Materials.Count; i++)
            {
                MtMaterial material = new MtMaterial();
                material.Name = scene.Materials[i].Name;
                string textureFilepath = scene.Materials[i].TextureDiffuse.FilePath;
                string[] pathSplit = textureFilepath.Split("\\");
                textureFilepath = pathSplit[pathSplit.Length - 1];
                textureFilepath = Path.Combine(directory, textureFilepath);
                textureFilepath += ".png";

                material.DiffuseTextureFileName = textureFilepath;
                material.DiffuseTextureBitmap = new System.Drawing.Bitmap(textureFilepath);

                model.Materials.Add(material);
            }

            // MESHES
            for (int i = 0; i < scene.Meshes.Count; i++)
            {
                MtMesh mesh = new MtMesh();
                mesh.Name = scene.Meshes[i].Name;
                mesh.MaterialId = scene.Meshes[i].MaterialIndex;

                // Vertices
                for(int j = 0; j < scene.Meshes[i].Vertices.Count; j++)
                {
                    MtVertex vertex = new MtVertex();

                    // Position
                    vertex.AbsolutePosition = AssimpUtils.ToNumerics(scene.Meshes[i].Vertices[j]);
                    // UV
                    if(scene.Meshes[i].TextureCoordinateChannels[0].Count > 0)
                    {
                        vertex.TextureCoordinates = AssimpUtils.ToNumerics(scene.Meshes[i].TextureCoordinateChannels[0][j]);
                    }
                    // Color
                    if (scene.Meshes[i].VertexColorChannels[0].Count > 0) {
                        vertex.Color = AssimpUtils.ToNumericsColor(scene.Meshes[i].VertexColorChannels[0][j]);
                    }
                    // Normal
                    if (scene.Meshes[i].Normals.Count > 0) {
                        vertex.Normal = AssimpUtils.ToNumerics(scene.Meshes[i].Normals[j]);
                    }

                    mesh.Vertices.Add(vertex);
                }
                // Weights
                for (int j = 0; j < scene.Meshes[i].Bones.Count; j++)
                {
                    Assimp.Bone bone = scene.Meshes[i].Bones[j];

                    int boneIndex = -1;
                    for (int k = 0; k < model.Joints.Count; k++)
                    {
                        if (model.Joints[k].Name == bone.Name) {
                            boneIndex = k;
                            break;
                        }
                    }

                    for (int k = 0; k < bone.VertexWeights.Count; k++)
                    {
                        Assimp.VertexWeight weight = bone.VertexWeights[k];

                        MtWeightPosition weightPosition = new MtWeightPosition();
                        weightPosition.JointIndex = boneIndex;
                        weightPosition.Weight = weight.Weight;

                        mesh.Vertices[weight.VertexID].Weights.Add(weightPosition);
                    }
                }

                // Faces
                for (int j = 0; j < scene.Meshes[i].Faces.Count; j++)
                {
                    Assimp.Face iFace = scene.Meshes[i].Faces[j];
                    if(iFace.Indices.Count > 3) {
                        throw new System.Exception("Sorry! We only work with triangles and you have a face with more than 3 vertices!");
                    }
                    else if (iFace.Indices.Count < 3) {
                        throw new System.Exception("It seems like one of your faces consist of less than 3 vertices. That doesn't sound right.");
                    }

                    MtFace face = new MtFace();
                    face.VertexIndices = iFace.Indices;
                    face.Clockwise = true;

                    mesh.Faces.Add(face);
                }

                mesh.BuildTriangleStrips();
                model.Meshes.Add(mesh);
            }

            return model;
        }
    }
}
