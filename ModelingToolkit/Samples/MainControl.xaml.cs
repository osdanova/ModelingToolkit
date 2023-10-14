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
        public ViewportService VpService { get; set; }
        public string LoadedFile { get; set; }
        public MtModel LoadedModel { get; set; }
        public MainControl()
        {
            InitializeComponent();
            VpService = new ViewportService(HelixViewport);
            VpService.ShowMesh = true;
            VpService.ShowWireframe = true;
            VpService.ShowSkeleton = true;
            VpService.ShowJoints = true;
            VpService.ShowBoundingBox = true;
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
            VpService.LoadNewModel(LoadedModel);
        }

        private void Button_Mesh(object sender, RoutedEventArgs e)
        {
            VpService.ShowMesh = !VpService.ShowMesh;
            VpService.Render();
        }
        private void Button_Wireframe(object sender, RoutedEventArgs e)
        {
            VpService.ShowWireframe = !VpService.ShowWireframe;
            VpService.Render();
        }
        private void Button_skeleton(object sender, RoutedEventArgs e)
        {
            VpService.ShowSkeleton = !VpService.ShowSkeleton;
            VpService.Render();
        }
        private void Button_joints(object sender, RoutedEventArgs e)
        {
            VpService.ShowJoints = !VpService.ShowJoints;
            VpService.Render();
        }
        private void Button_boundingBox(object sender, RoutedEventArgs e)
        {
            VpService.ShowBoundingBox = !VpService.ShowBoundingBox;
            VpService.Render();
        }
        private void Button_reload(object sender, RoutedEventArgs e)
        {
            loadFile();
        }
    }
}
