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
            VpController.ShowMesh = true;
            VpController.ShowWireframe = true;
            VpController.ShowSkeleton = true;
            VpController.ShowJoints = true;
            VpController.ShowBoundingBox = true;
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
            VpController.LoadNewModel(LoadedModel);
        }

        private void Button_Mesh(object sender, RoutedEventArgs e)
        {
            VpController.ShowMesh = !VpController.ShowMesh;
            VpController.Render();
        }
        private void Button_Wireframe(object sender, RoutedEventArgs e)
        {
            VpController.ShowWireframe = !VpController.ShowWireframe;
            VpController.Render();
        }
        private void Button_skeleton(object sender, RoutedEventArgs e)
        {
            VpController.ShowSkeleton = !VpController.ShowSkeleton;
            VpController.Render();
        }
        private void Button_joints(object sender, RoutedEventArgs e)
        {
            VpController.ShowJoints = !VpController.ShowJoints;
            VpController.Render();
        }
        private void Button_boundingBox(object sender, RoutedEventArgs e)
        {
            VpController.ShowBoundingBox = !VpController.ShowBoundingBox;
            VpController.Render();
        }
        private void Button_reload(object sender, RoutedEventArgs e)
        {
            loadFile();
        }
    }
}
