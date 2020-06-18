using System.Drawing;
using System.Windows.Media.Imaging;
using MapGenerator.Extensions;

namespace MapGenerator.Generators
{
    public static class ImageGenerator
    {
        public static BitmapImage GenerateGreyscale(float[,] map)
        {
            var size = map.GetLength(0);
            var bitmap = new Bitmap(size, size);
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var height = map[x, y];
                    var pixelColorValue = (int)height;
                    var pixelColor = Color.FromArgb(pixelColorValue, pixelColorValue, pixelColorValue);
                    bitmap.SetPixel(x, y, pixelColor);
                }
            }

            return bitmap.ToImage();
        }
    }
}
