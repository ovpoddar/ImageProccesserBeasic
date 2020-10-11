using ImageProcessorApp.Model;
using System.Collections.Generic;
using System.Drawing;

namespace ImageProcessorApp.Handlers
{
    public class BitmapHandler : IBitmapHandler
    {
        public IImageDataModel GetImageDataAsync(string path)
        {
            using var bmp = new Bitmap(path);

            var imageDataModel = new ImageDataModel(bmp.Height, bmp.Width);

            for (var y = 0; y < bmp.Height-1; y++)
            {
                var colors = new List<Color>();

                for (var x = 0; x < bmp.Width-1; x++) 
                    colors.Add(bmp.GetPixel(x, y));
                
                imageDataModel.Data.Add(colors);
            }

            return imageDataModel;
        }
    }
}