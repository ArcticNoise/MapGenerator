using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace MapGenerator.Extensions
{
    public static class BitmapExtensions
    {
        public static BitmapImage ToImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                var bImage = new BitmapImage();
                bImage.BeginInit();
                bImage.StreamSource = memory;
                bImage.CacheOption = BitmapCacheOption.OnLoad;
                bImage.EndInit();

                bImage.Freeze();

                return bImage;
            }
        }
    }
}
