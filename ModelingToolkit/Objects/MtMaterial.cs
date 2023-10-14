using Assimp;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace ModelingToolkit.Objects
{
    public class MtMaterial
    {
        public string? Name { get; set; }
        public string? DiffuseTextureFileName { get; set; }
        public Bitmap? DiffuseTextureBitmap { get; set; }

        public MtMaterial()
        {
            Name = null;
            DiffuseTextureFileName = null;
            DiffuseTextureBitmap = null;
        }

        public override string ToString()
        {
            return Name + " ["+DiffuseTextureFileName+"]";
        }

        public BitmapImage GetAsBitmapImageFromPath()
        {
            return new BitmapImage(new Uri(DiffuseTextureFileName, UriKind.RelativeOrAbsolute));
        }

        public BitmapImage GetAsBitmapImage()
        {
            // Save the System.Drawing.Bitmap to a MemoryStream
            MemoryStream stream = new MemoryStream();
            DiffuseTextureBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

            // Read the MemoryStream into a BitmapImage
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public void ExportAsPng(string filepath)
        {
            DiffuseTextureBitmap.Save(filepath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
