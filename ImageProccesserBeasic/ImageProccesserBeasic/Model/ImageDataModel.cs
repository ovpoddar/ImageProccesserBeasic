using System.Collections.Generic;
using System.Drawing;

namespace ImageProcessorApp.Model
{
    public class ImageDataModel : IImageDataModel
    {
        public int Height { get; }
        public int Width { get; }
        public List<List<Color>> Data { get; set; } = new List<List<Color>>();

        public ImageDataModel(int height, int width)
        {
            Height = height;
            Width = width;
        }

        public Bitmap BuildBitmap()
        {
            var bitmap = new Bitmap(Width, Height);

            for (var i = 0; i < Height - 1; i++)
            {
                for (var j = 0; j < Width - 1; j++)
                    bitmap.SetPixel(j, i, Data[i][j]);
            }

            return bitmap;
        }
    }
}