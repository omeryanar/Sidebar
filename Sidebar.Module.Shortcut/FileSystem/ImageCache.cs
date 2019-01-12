using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sidebar.Module.Shortcut.FileSystem
{
    public class ImageCache
    {
        public static Size GetSize(IconSizeType iconSizeType)
        {
            Size itemSize = new Size(128, 128);

            switch (iconSizeType)
            {
                case IconSizeType.Small:
                    itemSize = new Size(16, 16);
                    break;

                case IconSizeType.Medium:
                    itemSize = new Size(32, 32);
                    break;

                case IconSizeType.Large:
                    itemSize = new Size(128, 128);
                    break;

                case IconSizeType.ExtraLarge:
                    itemSize = new Size(256, 256);
                    break;
            }

            return itemSize;
        }

        public static ImageSource GetImageSource(string path, IconSizeType iconSizeType)
        {
            string key = GetKey(path, iconSizeType);
            if (key != null && Cache.ContainsKey(key))
                return Cache[key];

            Size size = GetSize(iconSizeType);
            Icon icon = ImageHelper.GetFileIcon(path, iconSizeType, size);

            MemoryStream memoryStream = new MemoryStream();
            icon.ToBitmap().Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();

            if (key != null)
                Cache[key] = bitmapImage;

            return bitmapImage;
        }

        public static void Clear()
        {
            Cache.Clear();
        }

        protected static string GetKey(string path,  IconSizeType iconSizeType)
        {
            string key = String.Format("{0}_{1}", path, iconSizeType);

            if (Directory.Exists(path))
                return key;

            string extension = Path.GetExtension(path);
            
            if (String.IsNullOrEmpty(extension)) 
                return key;
            if (String.Equals(extension, ".exe", StringComparison.Ordinal))
                return key;
            if (String.Equals(extension, ".lnk", StringComparison.Ordinal)) 
                return key;

            key = String.Format("{0}_{1}", extension, iconSizeType);
            
            return key;
        }

        protected static Dictionary<String, ImageSource> Cache = new Dictionary<String, ImageSource>();
    }
}
