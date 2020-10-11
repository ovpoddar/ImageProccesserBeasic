using System.Collections.Generic;
using System.Drawing;
using ImageProcessorApp.Model;

namespace ImageProcessorApp.Handlers
{
    public class BitmapHandler : IBitmapHandler
    {
        public IImageDataModel GetImageDataAsync(string path)
        {
            using var bmp = new Bitmap(path);

            var imageDataModel = new ImageDataModel(bmp.Height + 1, bmp.Width + 1);

            for (var i = 0; i < bmp.Height; i++)
            {
                var line = new List<Color>();

                for (var n = 0; n < bmp.Width; n++) 
                    line.Add(bmp.GetPixel(i, n));
                
                imageDataModel.Data.Add(line);
            }

            return imageDataModel;
        }
    }
}