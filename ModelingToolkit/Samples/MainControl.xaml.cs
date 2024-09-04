using Assimp;
using Microsoft.Win32;
using ModelingToolkit.Core;
using ModelingToolkit.Formats;
using ModelingToolkit.Formats.MtAssimp;
using ModelingToolkit.HelixModule;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ModelingToolkit.Samples
{
    public partial class MainControl : UserControl
    {
        public ViewportController VpController { get; set; }
        public string LoadedFile { get; set; }
        public MtScene Scene { get; set; }

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
            Scene = AssimpImporter.ImportScene(LoadedFile);
            VpController.ClearModels();
            VpController.ClearShapes();
            VpController.AddModel(Scene.Models[0].Model);
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

        private void MenuItem_ExportGltf(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd;
            sfd = new SaveFileDialog();
            sfd.Title = "Export model";
            sfd.FileName = Path.GetFileNameWithoutExtension(LoadedFile) + ".gltf";
            if (sfd.ShowDialog() == true)
            {
                string dirPath = Path.GetDirectoryName(sfd.FileName);

                if (!Directory.Exists(dirPath))
                    return;
                dirPath += "\\";

                MtPorter.ExportScene(Scene, sfd.FileName, MtPorter.FileType.GLTF); //.SaveGLB(dirPath + sfd.FileName + ".glb");
            }
        }

        private void MenuItem_ExportGlb(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd;
            sfd = new SaveFileDialog();
            sfd.Title = "Export model";
            sfd.FileName = Path.GetFileNameWithoutExtension(LoadedFile) + ".glb";
            if (sfd.ShowDialog() == true)
            {
                string dirPath = Path.GetDirectoryName(sfd.FileName);

                if (!Directory.Exists(dirPath))
                    return;
                dirPath += "\\";

                MtPorter.ExportScene(Scene, sfd.FileName, MtPorter.FileType.GLB);
            }
        }

        private void MenuItem_ExportFbx(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd;
            sfd = new SaveFileDialog();
            sfd.Title = "Export model";
            sfd.FileName = Path.GetFileNameWithoutExtension(LoadedFile) + ".fbx";
            if (sfd.ShowDialog() == true)
            {
                string dirPath = Path.GetDirectoryName(sfd.FileName);

                if (!Directory.Exists(dirPath))
                    return;
                dirPath += "\\";

                MtPorter.ExportScene(Scene, sfd.FileName, MtPorter.FileType.FBX);
            }
        }
    }
}
