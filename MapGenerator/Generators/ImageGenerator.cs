using System.Drawing;
using System.Threading;
using System.Windows.Media.Imaging;
using MapGenerator.Bitmaps;
using MapGenerator.Extensions;

namespace MapGenerator.Generators
{
    public static class ImageGenerator
    {
        public static BitmapImage GenerateGreyscale(float[,] map, CancellationToken token)
        {
            var size = map.GetLength(0);
            var directBitmap = new DirectBitmap(size, size);
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var height = map[x, y];
                    var pixelColorValue = (int)height;
                    var pixelColor = Color.FromArgb(pixelColorValue, pixelColorValue, pixelColorValue);
                    directBitmap.SetPixel(x, y, pixelColor);

                    token.ThrowIfCancellationRequested();
                }

                token.ThrowIfCancellationRequested();
            }

            var bitmap = directBitmap.Bitmap;
            return bitmap.ToImage();
        }
    }
}
