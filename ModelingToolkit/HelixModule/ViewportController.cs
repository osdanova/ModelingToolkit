using HelixToolkit.Wpf;
using ModelingToolkit.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ModelingToolkit.HelixModule
{
    public class ViewportController
    {
        public HelixViewport3D Viewport { get; set; }

        // Contents
        public Color BackgroundColor { get; set; }
        public ModelVisual3D Lightning { get; set; }
        public Light LightingLight { get; set; }
        public Dictionary<MtModel, ModelVisual3D> ModelVisuals { get; private set; }
        public Dictionary<MtModel, List<LinesVisual3D>> ModelSkeleton { get; private set; }
        public Dictionary<MtModel, List<BillboardVisual3D>> ModelJoints { get; private set; }
        public Dictionary<MtModel, List<LinesVisual3D>> ModelWireframe { get; private set; }
        public Dictionary<MtModel, ModelVisual3D> ModelBoundingBox { get; private set; }
        public Dictionary<MtShape, ModelVisual3D> ShapeVisuals { get; private set; }
        public BoxVisual3D Origin { get; set; }
        public GridLinesVisual3D GridLines { get; set; }

        // Visibility Options
        public bool IsMeshVisible { get; private set; }
        public bool IsSkeletonVisible { get; private set; }
        public bool IsJointVisible { get; private set; }
        public bool IsWireframeVisible { get; private set; }
        public bool IsBoundingBoxVisible { get; private set; }
        public bool IsOriginVisible { get; private set; }
        public bool IsGridVisible { get; private set; }

        public ViewportController(HelixViewport3D viewport)
        {
            Viewport = viewport;
            ModelVisuals = new Dictionary<MtModel, ModelVisual3D>();
            ModelSkeleton = new Dictionary<MtModel, List<LinesVisual3D>>();
            ModelJoints = new Dictionary<MtModel, List<BillboardVisual3D>>();
            ModelWireframe = new Dictionary<MtModel, List<LinesVisual3D>>();
            ModelBoundingBox = new Dictionary<MtModel, ModelVisual3D>();
            ShapeVisuals = new Dictionary<MtShape, ModelVisual3D>();
            IsMeshVisible = true;
            IsWireframeVisible = false;
            IsSkeletonVisible = false;
            IsJointVisible = false;
            IsBoundingBoxVisible = false;
            IsOriginVisible = true;
            IsGridVisible = true;
            LightingLight = new AmbientLight(Brushes.White.Color);
            BackgroundColor = Color.FromArgb(255, 10, 10, 10);

            CreateBase();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // VIEWPORT OPTIONS
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetLightning(Light newLight)
        {
            LightingLight = newLight;
            Lightning.Content = LightingLight;
        }
        public void SetBackgroundColor(Color newColor)
        {
            BackgroundColor = newColor;
            Viewport.Background = new SolidColorBrush(BackgroundColor);
        }

        public void SetVisibilityMesh(bool isVisible)
        {
            IsMeshVisible = isVisible;
        }

        public void SetVisibilitySkeleton(bool isVisible)
        {
            IsSkeletonVisible = isVisible;
        }

        public void SetVisibilityJoint(bool isVisible)
        {
            IsJointVisible = isVisible;
        }

        public void SetVisibilityWireframe(bool isVisible)
        {
            IsWireframeVisible = isVisible;
        }

        public void SetVisibilityBoundingBox(bool isVisible)
        {
            IsBoundingBoxVisible = isVisible;
        }

        public void SetVisibilityOrigin(bool isVisible)
        {
            if (IsOriginVisible && !isVisible)
            {
                Viewport.Children.Remove(Origin);
            }
            else if (!IsOriginVisible && isVisible)
            {
                Viewport.Children.Add(Origin);
            }
            IsOriginVisible = isVisible;
        }

        public void SetVisibilityGrid(bool isVisible)
        {
            if (IsGridVisible && !isVisible)
            {
                Viewport.Children.Remove(GridLines);
            }
            else if (!IsGridVisible && isVisible)
            {
                Viewport.Children.Add(GridLines);
            }
            IsGridVisible = isVisible;
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // ADD / REMOVE OBJECTS
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddModel(MtModel model, bool loadMesh = true, bool loadSkeleton = true, bool loadJoints = true, bool loadWireframe = true, bool loadBoundingBox = true)
        {
            if (ModelVisuals.Keys.Contains(model))
            {
                throw new System.Exception("Model already exists in current Viewport");
            }

            if (loadMesh)
            {
                ModelVisuals.Add(model, ObjectProcessor.GetVisualFromModel(model));
            }
            else
            {
                ModelVisuals.Add(model, null);
            }

            if (loadSkeleton)
            {
                ModelSkeleton.Add(model, ObjectProcessor.GetBonesFromModel(model));
            }
            else
            {
                ModelSkeleton.Add(model, null);
            }

            if (loadJoints)
            {
                ModelJoints.Add(model, ObjectProcessor.GetJointsFromModel(model));
            }
            else
            {
                ModelJoints.Add(model, null);
            }

            if (loadWireframe)
            {
                List<LinesVisual3D> wireframe = new List<LinesVisual3D>();
                foreach (MtMesh mesh in model.Meshes)
                {
                    wireframe.AddRange(ObjectProcessor.GetWireframeFromMesh(mesh));
                }
                ModelWireframe.Add(model, wireframe);
            }
            else
            {
                ModelWireframe.Add(model, null);
            }

            if (loadBoundingBox)
            {
                Rect3D boundingBox = MtHelper.GetBoundingBox(model);
                ModelVisual3D bb = Visuals.GetBoundingBoxVisual(boundingBox);
                HelixUtils.ApplyRotationForViewport(bb);
                ModelBoundingBox.Add(model, bb);
            }
            else
            {
                ModelBoundingBox.Add(model, null);
            }
        }

        public void RemoveModel(MtModel model)
        {
            if (ModelVisuals.Keys.Contains(model))
            {
                if (ModelVisuals[model] != null)
                {
                    Viewport.Children.Remove(ModelVisuals[model]);
                }
                ModelVisuals.Remove(model);

                if (ModelSkeleton[model] != null)
                {
                    foreach (LinesVisual3D bone in ModelSkeleton[model])
                    {
                        Viewport.Children.Remove(bone);
                    }
                }
                ModelSkeleton.Remove(model);

                if (ModelJoints[model] != null)
                {
                    foreach (BillboardVisual3D joint in ModelJoints[model])
                    {
                        Viewport.Children.Remove(joint);
                    }
                }
                ModelJoints.Remove(model);

                if (ModelWireframe[model] != null)
                {
                    foreach (LinesVisual3D edge in ModelWireframe[model])
                    {
                        Viewport.Children.Remove(edge);
                    }
                }
                ModelWireframe.Remove(model);

                if (ModelBoundingBox[model] != null)
                {
                    Viewport.Children.Remove(ModelBoundingBox[model]);
                    ModelBoundingBox.Remove(model);
                }
            }
        }

        public void AddShape(MtShape shape)
        {
            // TODO


            //if (ShapeVisuals.Keys.Contains(shape))
            //{
            //    throw new System.Exception("Shape already exists in current Viewport");
            //}
            //
            //ShapeVisuals.Add(shape, shape.GetVisual());
            //if (shape.IsVisible)
            //{
            //    Viewport.Children.Add(ShapeVisuals[shape]);
            //}
        }

        public void RemoveShape(MtShape shape)
        {
            if (ShapeVisuals.Keys.Contains(shape))
            {
                Viewport.Children.Remove(ShapeVisuals[shape]);
                ShapeVisuals.Remove(shape);
            }
        }

        public void ClearModels()
        {
            foreach (MtModel model in ModelVisuals.Keys)
            {
                RemoveModel(model);
                ModelVisuals.Remove(model);
                ModelSkeleton.Remove(model);
                ModelJoints.Remove(model);
                ModelWireframe.Remove(model);
                ModelBoundingBox.Remove(model);
            }
        }
        public void ClearShapes()
        {
            foreach (MtShape shape in ShapeVisuals.Keys)
            {
                RemoveShape(shape);
                ShapeVisuals.Remove(shape);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // RENDERING
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CreateBase()
        {
            Lightning = new ModelVisual3D { Content = LightingLight };
            Viewport.Children.Add(Lightning);

            Viewport.Background = new SolidColorBrush(BackgroundColor);

            Origin = Visuals.GetOrigin();
            if (IsOriginVisible)
            {
                Viewport.Children.Add(Origin);
            }

            GridLines = Visuals.GetGridLinesVisual(new System.Numerics.Vector3(), 1000, 1000);
            if (IsGridVisible)
            {
                Viewport.Children.Add(GridLines);
            }
        }

        public void Render()
        {
            foreach (MtModel model in ModelVisuals.Keys)
            {
                if (MtHelper.MetadataIsTrue(model.Metadata, MtHelper.KEY_VISIBLE, true))
                {
                    if (IsMeshVisible)
                    {
                        if (!Viewport.Children.Contains(ModelVisuals[model]))
                        {
                            if (ModelVisuals[model] == null)
                            {
                                ModelVisuals[model] = ObjectProcessor.GetVisualFromModel(model);
                            }
                            Viewport.Children.Add(ModelVisuals[model]);
                        }
                    }
                    else if (ModelVisuals[model] != null)
                    {
                        Viewport.Children.Remove(ModelVisuals[model]);
                    }

                    if (IsSkeletonVisible)
                    {
                        if (ModelSkeleton[model] == null)
                        {
                            ModelSkeleton[model] = ObjectProcessor.GetBonesFromModel(model);
                        }
                        foreach (LinesVisual3D bone in ModelSkeleton[model])
                        {
                            if (!Viewport.Children.Contains(bone))
                            {
                                Viewport.Children.Add(bone);
                            }
                        }
                    }
                    else if (ModelSkeleton[model] != null)
                    {
                        foreach (LinesVisual3D bone in ModelSkeleton[model])
                        {
                            Viewport.Children.Remove(bone);
                        }
                    }


                    if (IsJointVisible)
                    {
                        if (ModelJoints[model] == null)
                        {
                            ModelJoints[model] = ObjectProcessor.GetJointsFromModel(model);
                        }
                        foreach (BillboardVisual3D joint in ModelJoints[model])
                        {
                            if (!Viewport.Children.Contains(joint))
                            {
                                Viewport.Children.Add(joint);
                            }
                        }
                    }
                    else if (ModelJoints[model] != null)
                    {
                        foreach (BillboardVisual3D joint in ModelJoints[model])
                        {
                            Viewport.Children.Remove(joint);
                        }
                    }

                    if (IsWireframeVisible)
                    {
                        if (ModelWireframe[model] == null)
                        {
                            List<LinesVisual3D> wireframe = new List<LinesVisual3D>();
                            foreach (MtMesh mesh in model.Meshes)
                            {
                                wireframe.AddRange(ObjectProcessor.GetWireframeFromMesh(mesh));
                            }
                            ModelWireframe[model] = wireframe;
                        }
                        foreach (LinesVisual3D edge in ModelWireframe[model])
                        {
                            if (!Viewport.Children.Contains(edge))
                            {
                                Viewport.Children.Add(edge);
                            }
                        }
                    }
                    else if (ModelWireframe[model] != null)
                    {
                        foreach (LinesVisual3D edge in ModelWireframe[model])
                        {
                            Viewport.Children.Remove(edge);
                        }
                    }

                    if (IsBoundingBoxVisible)
                    {
                        if (ModelBoundingBox[model] == null)
                        {
                            Rect3D boundingBox = MtHelper.GetBoundingBox(model);
                            ModelVisual3D bb = Visuals.GetBoundingBoxVisual(boundingBox);
                            HelixUtils.ApplyRotationForViewport(bb);
                            ModelBoundingBox[model] = bb;
                        }
                        if (!Viewport.Children.Contains(ModelBoundingBox[model]))
                        {
                            Viewport.Children.Add(ModelBoundingBox[model]);
                        }
                    }
                    else if (ModelBoundingBox[model] != null)
                    {
                        Viewport.Children.Remove(ModelBoundingBox[model]);
                    }
                }
                else
                {
                    if (ModelBoundingBox[model] != null)
                    {
                        Viewport.Children.Remove(ModelVisuals[model]);
                    }
                    if (ModelSkeleton[model] != null)
                    {
                        foreach (LinesVisual3D bone in ModelSkeleton[model])
                        {
                            Viewport.Children.Remove(bone);
                        }
                    }
                    if (ModelJoints[model] != null)
                    {
                        foreach (BillboardVisual3D joint in ModelJoints[model])
                        {
                            Viewport.Children.Remove(joint);
                        }
                    }
                    if (ModelWireframe[model] != null)
                    {
                        foreach (LinesVisual3D edge in ModelWireframe[model])
                        {
                            Viewport.Children.Remove(edge);
                        }
                    }
                    if (ModelBoundingBox[model] != null)
                    {
                        Viewport.Children.Remove(ModelBoundingBox[model]);
                    }
                }
            }

            foreach (MtShape shape in ShapeVisuals.Keys)
            {
                if (MtHelper.MetadataIsTrue(shape.Metadata, MtHelper.KEY_VISIBLE))
                {
                    if (!Viewport.Children.Contains(ShapeVisuals[shape]))
                    {
                        Viewport.Children.Add(ShapeVisuals[shape]);
                    }
                }
                else
                {
                    Viewport.Children.Remove(ShapeVisuals[shape]);
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // FUNCTIONS
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Resets the camera to its starting position
        public void ResetCamera()
        {
            if (ModelVisuals.Count > 0 && ModelVisuals.Keys.ToArray()[0].Meshes.Count > 0)
            {
                Rect3D boundingBox = MtHelper.GetBoundingBox(ModelVisuals.Keys.ToArray()[0]);
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

        // Finds the models with a specific name. Case insensitive.
        public List<MtModel> FindModelsByName(string searchString, bool exactMatch = true)
        {
            List<MtModel> searchResult = new List<MtModel>();
            foreach (MtModel model in ModelVisuals.Keys)
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
            foreach (MtModel model in ModelVisuals.Keys)
            {
                foreach (string label in model.Metadata.Keys)
                {
                    if (exactMatch)
                    {
                        if (label.ToLower() == searchString.ToLower())
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
            foreach (MtShape shape in ShapeVisuals.Keys)
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
            foreach (MtShape shape in ShapeVisuals.Keys)
            {
                foreach (string label in shape.Metadata.Keys)
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
