using ModelingToolkit.AssimpModule;
using ModelingToolkit.HelixModule;
using ModelingToolkit.Objects;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ModelingToolkit.Samples
{
    public partial class MainControl : UserControl
    {
        public ViewportController VpController { get; set; }
        public string LoadedFile { get; set; }
        public MtModel LoadedModel { get; set; }
        public MainControl()
        {
            InitializeComponent();
            VpController = new ViewportController(HelixViewport);
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string firstFile = files?.FirstOrDefault();

                if (firstFile.ToLower().EndsWith(".fbx") || firstFile.ToLower().EndsWith(".dae"))
                {
                    LoadedFile = firstFile;
                    loadFile();
                }
            }
        }

        public void loadFile()
        {
            LoadedModel = AssimpImporter.ImportScene(LoadedFile);
            VpController.ClearModels();
            VpController.ClearShapes();
            VpController.AddModel(LoadedModel);
            VpController.Render();
        }

        private void Button_Mesh(object sender, RoutedEventArgs e)
        {
            VpController.SetVisibilityMesh(!VpController.IsMeshVisible);
            VpController.Render();
        }
        private void Button_Wireframe(object sender, RoutedEventArgs e)
        {
            VpController.SetVisibilityWireframe(!VpController.IsWireframeVisible);
            VpController.Render();
        }
        private void Button_skeleton(object sender, RoutedEventArgs e)
        {
            VpController.SetVisibilitySkeleton(!VpController.IsSkeletonVisible);
            VpController.Render();
        }
        private void Button_joints(object sender, RoutedEventArgs e)
        {
            VpController.SetVisibilityJoint(!VpController.IsJointVisible);
            VpController.Render();
        }
        private void Button_boundingBox(object sender, RoutedEventArgs e)
        {
            VpController.SetVisibilityBoundingBox(!VpController.IsBoundingBoxVisible);
            VpController.Render();
        }
        private void Button_reload(object sender, RoutedEventArgs e)
        {
            loadFile();
        }
    }
}
