using System;
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
            var width = map.GetLength(0);
            var height = map.GetLength(1);
            var directBitmap = new DirectBitmap(width, height);
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var value = map[x, y];
                    var pixelColorValue = Math.Clamp((int)(value * 255), 0, 255);
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
