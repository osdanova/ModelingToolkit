using HelixToolkit.Wpf;
using ModelingToolkit.Objects;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ModelingToolkit.HelixModule
{
    public class ViewportController
    {
        public HelixViewport3D Viewport { get; set; }

        // Contents
        public List<MtModel> Models { get; set; }
        public List<MtShape> Shapes { get; set; }
        public Light Lighting { get; set; }
        public Color BackgroundColor { get; set; }

        // Visibility Options
        public bool ShowMesh { get; set; }
        public bool ShowWireframe { get; set; }
        public bool ShowSkeleton { get; set; }
        public bool ShowJoints { get; set; }
        public bool ShowBoundingBox { get; set; }
        public bool ShowOrigin { get; set; }
        public bool ShowGrid { get; set; }

        public ViewportController(HelixViewport3D viewport)
        {
            Viewport = viewport;
            Models = new List<MtModel>();
            Shapes = new List<MtShape>();
            ShowMesh = true;
            ShowWireframe = false;
            ShowSkeleton = false;
            ShowJoints = false;
            ShowBoundingBox = false;
            ShowOrigin = true;
            ShowGrid = true;
            Lighting = new AmbientLight(Brushes.White.Color);
            BackgroundColor = Color.FromArgb(255, 10, 10, 10);

            RenderBase();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // ADD / REMOVE OBJECTS
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void LoadNewModel(MtModel model)
        {
            LoadNewModels(new List<MtModel> { model });
        }
        public void LoadNewModels(List<MtModel> models)
        {
            Models.Clear();
            Models.AddRange(models);
            Render();
            ResetCamera();
        }

        public void AddModel(MtModel model)
        {
            Models.Add(model);
            Render();
        }

        public void RemoveModel(MtModel model)
        {
            Models.Remove(model);
            Render();
        }

        public void AddShape(MtShape shape)
        {
            Shapes.Add(shape);
            Render();
        }

        public void RemoveShape(MtShape shape)
        {
            Shapes.Remove(shape);
            Render();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // RENDERING
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Render()
        {
            Viewport.Children.Clear();

            RenderBase();

            foreach (MtModel model in Models)
            {
                if (!model.IsVisible) {
                    continue;
                }

                if (ShowSkeleton)
                {
                    List<LinesVisual3D> skeleton = ObjectProcessor.GetBonesFromModel(model);
                    foreach (LinesVisual3D bone in skeleton)
                    {
                        Viewport.Children.Add(bone);
                    }
                }

                if (ShowJoints)
                {
                    List<BillboardVisual3D> skeleton = ObjectProcessor.GetJointsFromModel(model);
                    foreach (BillboardVisual3D bone in skeleton)
                    {
                        Viewport.Children.Add(bone);
                    }
                }

                if (ShowWireframe)
                {
                    List<LinesVisual3D> wireframe = ObjectProcessor.GetWireframeFromModel(model);
                    foreach (LinesVisual3D bone in wireframe)
                    {
                        Viewport.Children.Add(bone);
                    }
                }

                if (ShowBoundingBox)
                {
                    Rect3D boundingBox = model.GetBoundingBox();
                    ModelVisual3D bb = Visuals.GetBoundingBoxVisual(boundingBox);
                    HelixUtils.ApplyRotationForViewport(bb);
                    Viewport.Children.Add(bb);
                }

                if (ShowMesh)
                {
                    ModelVisual3D modelVisual = ObjectProcessor.GetVisualFromModel(model);
                    Viewport.Children.Add(modelVisual);
                }
            }

            foreach (MtShape shape in Shapes)
            {
                if (!shape.IsVisible) {
                    continue;
                }

                ModelVisual3D shapeVisual = shape.GetVisual();
                Viewport.Children.Add(shapeVisual);
            }
        }

        public void RenderBase()
        {
            ResetLighting();

            Viewport.Background = new SolidColorBrush(BackgroundColor);

            if (ShowGrid)
            {
                Viewport.Children.Add(Visuals.GetGridLinesVisual(new System.Numerics.Vector3(), 1000, 1000));
            }

            if (ShowOrigin)
            {
                Viewport.Children.Add(Visuals.GetOrigin());
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // FUNCTIONS
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Resets the camera to its starting position
        public void ResetCamera()
        {
            if(Models.Count > 0 && Models[0].Meshes.Count > 0)
            {
                Rect3D boundingBox = Models[0].GetBoundingBox();
                double greatestSize = (boundingBox.SizeY > boundingBox.SizeZ) ? boundingBox.SizeY : boundingBox.SizeZ;
                Viewport.CameraController.CameraPosition = new Point3D(boundingBox.X + greatestSize * 3, boundingBox.Z, boundingBox.Y);
                Viewport.CameraController.CameraTarget = new Point3D(boundingBox.X, boundingBox.Z, boundingBox.Y);
                Viewport.CameraController.CameraUpDirection = new Vector3D(0, 0, 1);
            }
            else
            {
                Viewport.CameraController.ResetCamera();
            }
        }
        // Resets the lighting to default
        public void ResetLighting()
        {
            ModelVisual3D lightning = new ModelVisual3D();
            lightning.Content = Lighting;
            Viewport.Children.Add(lightning);
        }

        // Finds the models with a specific name. Case insensitive.
        public List<MtModel> FindModelsByName(string searchString, bool exactMatch = true)
        {
            List<MtModel> searchResult = new List<MtModel>();
            foreach (MtModel model in Models)
            {
                if (exactMatch)
                {
                    if (model.Name.ToLower() == searchString.ToLower())
                    {
                        searchResult.Add(model);
                        continue;
                    }
                }
                else
                {
                    if (model.Name.ToLower().Contains(searchString.ToLower()))
                    {
                        searchResult.Add(model);
                        continue;
                    }
                }
            }
            return searchResult;
        }

        // Finds the models with a specific label. Case insensitive.
        public List<MtModel> FindModelsByLabel(string searchString, bool exactMatch = true)
        {
            List<MtModel> searchResult = new List<MtModel>();
            foreach(MtModel model in Models)
            {
                foreach(string label in model.Labels)
                {
                    if (exactMatch)
                    {
                        if(label.ToLower() == searchString.ToLower())
                        {
                            searchResult.Add(model);
                            break;
                        }
                    }
                    else
                    {
                        if (label.ToLower().Contains(searchString.ToLower()))
                        {
                            searchResult.Add(model);
                            break;
                        }
                    }
                }
            }
            return searchResult;
        }

        // Finds the models with a specific name. Case insensitive.
        public List<MtShape> FindShapesByName(string searchString, bool exactMatch = true)
        {
            List<MtShape> searchResult = new List<MtShape>();
            foreach (MtShape shape in Shapes)
            {
                if (exactMatch)
                {
                    if (shape.Name.ToLower() == searchString.ToLower())
                    {
                        searchResult.Add(shape);
                        continue;
                    }
                }
                else
                {
                    if (shape.Name.ToLower().Contains(searchString.ToLower()))
                    {
                        searchResult.Add(shape);
                        continue;
                    }
                }
            }
            return searchResult;
        }

        // Finds the shapes with a specific label. Case insensitive.
        public List<MtShape> FindShapesByLabel(string searchString, bool exactMatch = true)
        {
            List<MtShape> searchResult = new List<MtShape>();
            foreach (MtShape shape in Shapes)
            {
                foreach (string label in shape.Labels)
                {
                    if (exactMatch)
                    {
                        if (label.ToLower() == searchString.ToLower())
                        {
                            searchResult.Add(shape);
                            break;
                        }
                    }
                    else
                    {
                        if (label.ToLower().Contains(searchString.ToLower()))
                        {
                            searchResult.Add(shape);
                            break;
                        }
                    }
                }
            }
            return searchResult;
        }
    }
}
