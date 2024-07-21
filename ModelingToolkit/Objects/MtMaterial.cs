using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace ModelingToolkit.Objects
{
    // Represents a diffuse texture
    // Palette size is 256
    public class MtMaterial
    {
        public string Name;
        public byte[] Data;
        public byte[] Clut; // Color palette
        public int Width;
        public int Height;
        public byte ColorSize; // 1, 2 or 4
        public bool PixelHasAlpha;
        public string DiffuseTextureFileName;
        public Bitmap DiffuseTextureBitmap;

        public override string ToString()
        {
            return Name + " ["+DiffuseTextureFileName+"]";
        }
        public void GenerateBitmap()
        {
            if (Data == null || Clut == null || Width <= 0 || Height <= 0)
            {
                throw new ArgumentException("Can't create bitmap");
            }

            if (Data.Length != Width * Height)
            {
                throw new ArgumentException("Image data length or CLUT length is invalid");
            }

            DiffuseTextureBitmap = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (MemoryStream stream = new MemoryStream(Data))
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        byte pixelIndex = (byte)stream.ReadByte();
                        Color pixelColor = GetColorFromClutRgba(pixelIndex);
                        DiffuseTextureBitmap.SetPixel(x, y, pixelColor);
                    }
                }
            }
        }

        private Color GetColorFromClutRgba(int index)
        {
            int start = index * 4 * ColorSize;
            int red = 0;
            int green = 0;
            int blue = 0;
            int alpha = 0;
            using (Stream clutStream = new MemoryStream(Clut))
            {
                clutStream.Position = start;
                BinaryReader reader = new BinaryReader(clutStream);
                if(ColorSize == 1)
                {
                    red = reader.ReadByte();
                    green = reader.ReadByte();
                    blue = reader.ReadByte();
                    if(PixelHasAlpha) alpha = reader.ReadByte();
                }
                else if (ColorSize == 2)
                {
                    red = reader.ReadInt16();
                    green = reader.ReadInt16();
                    blue = reader.ReadInt16();
                    if (PixelHasAlpha) alpha = reader.ReadInt16();
                }
                else if (ColorSize == 4)
                {
                    red = reader.ReadInt32();
                    green = reader.ReadInt32();
                    blue = reader.ReadInt32();
                    if (PixelHasAlpha) alpha = reader.ReadInt32();
                }
                else
                {
                    throw new Exception("Color size not set or invalid");
                }
            }

            return Color.FromArgb(alpha, red, green, blue);
        }

        public void BitmapToDataClut(int clutSize = 256)
        {
            Data = new byte[Width * Height];
            List<int> clutList = new List<int>();
            clutList.Add(0); // No color

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Color pixelColor = DiffuseTextureBitmap.GetPixel(x, y);
                    int intColor = pixelColor.ToArgb();
                    int colorIndexInt = clutList.IndexOf(intColor);
                    if (colorIndexInt == -1)
                    {
                        if (clutList.Count == clutSize)
                        {
                            throw new Exception("The image has more than " + clutSize + " colors");
                        }

                        colorIndexInt = (byte)clutList.Count;
                        clutList.Add(intColor);
                    }
                    Data[(Width * y) + x] = (byte)colorIndexInt;
                }
            }

            Clut = new byte[clutSize * 4];
            for(int i = 0; i < clutList.Count; i++)
            {
                byte[] argbBytes = BitConverter.GetBytes(clutList[i]);
                Clut[4 * i] = argbBytes[2];
                Clut[4 * i + 1] = argbBytes[1];
                Clut[4 * i + 2] = argbBytes[0];
                Clut[4 * i + 3] = argbBytes[3];
            }
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
