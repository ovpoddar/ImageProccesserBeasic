using ImageProccesserBeasic.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageProccesserBeasic
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // change the path to a valid image
            var data = await GetImageDataAsync(@"C:\Users\ayanp\Desktop\IMG_20200925_215107.jpg");

            var img = new Bitmap(data.Width, data.Height);
            for (int i = 0; i < data.Height - 1; i++)
            {
                for (int j = 0; j < data.Width - 1; j++)
                {
                    img.SetPixel(j, i, data.Data[i][j]);
                }
            }
            for (int i = 1000; i < 3000; i++)
            {
                for (int j = 2000; j < 3000; j++)
                {
                    img.SetPixel(j, i, Color.White);
                }
            }
            // change the output path too
            img.Save(@"C:\Users\ayanp\Desktop\IMG_20200925_215108.jpg", ImageFormat.Jpeg);
        }

        static async Task<ImageDataModel> GetImageDataAsync(string Path)
        {
            return await Task.Run(() =>
            {
                using (Bitmap bmp = new Bitmap(Path))
                {
                    var width = bmp.Width - 1;
                    var height = bmp.Height - 1;
                    var pixels = new List<List<Color>>();

                    for (var j = 0; j < height; j++)
                    {
                        var linesOfPixels = new List<Color>();
                        for (var i = 0; i < width; i++)
                        {
                            Color clr = bmp.GetPixel(i, j);
                            linesOfPixels.Add(clr);
                        }
                        pixels.Add(linesOfPixels);
                    }
                    return new ImageDataModel()
                    {
                        Data = pixels,
                        Height = height + 1,
                        Width = width + 1
                    };
                }
            });

        }
    }
}
