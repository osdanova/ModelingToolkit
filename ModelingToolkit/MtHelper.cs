using ModelingToolkit.Core;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace ModelingToolkit
{
    public class MtHelper
    {
        public static readonly string KEY_VISIBLE = "MT_VISIBLE";
        public static readonly string KEY_WIREFRAME_VISIBLE = "MT_WIREFRAME_VISIBLE";

        public static bool MetadataIsTrue (Dictionary<string, string> metadata, string key, bool defaultValue = false)
        {
            if (!metadata.Keys.Contains(key))
            {
                metadata.Add(key, defaultValue.ToString());
                return defaultValue;
            }
            else return metadata[key] == "True";
        }

        public static ImageSource GetImageSourceFromMaterial(MtMaterial material)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Save the bitmap to the memory stream in a format (e.g., PNG)
                material.DiffuseTextureBitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Create a BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // To make it cross-thread accessible

                return bitmapImage;
            }
        }

        public static Rect3D GetBoundingBox(MtModel model)
        {
            if (model.BoundingBox == null)
            {
                model.GenerateBoundingBox();
            }
            MtShape modelBB = model.BoundingBox;
            Rect3D bb = new Rect3D();
            bb.Location = new Point3D(modelBB.Position.X, modelBB.Position.Y, modelBB.Position.Z);
            bb.Size = new Size3D(modelBB.Width, modelBB.Height, modelBB.Depth);
            return bb;
        }
    }
}
