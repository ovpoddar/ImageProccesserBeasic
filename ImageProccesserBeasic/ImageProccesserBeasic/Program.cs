using ImageProccesserBeasic.Model;
using System;
using System.Collections.Generic;
using System.Data;
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
            var data = await GetImageDataAsync(@"C:\Users\ayanp\Desktop\image1.jpg");

            // resize the photo
            var sampleSize = 50;

            var w = Math.Ceiling((decimal)data.Width / sampleSize);
            var h = Math.Ceiling((decimal)data.Height / sampleSize);

            int width = (int)(w * sampleSize);
            int height = (int)(h * sampleSize);

            // create a blank resized image
            var img = new Bitmap(width, height);


            // make a squere in that place
            for (int i = 0; i < data.Height; i += sampleSize)
            {
                for (int j = 0; j < data.Width; j += sampleSize)
                {
                    //img.SetPixel(j, i, data.Data[i][j]);
                    square(j, i, sampleSize, sampleSize, data.Data[i][j], img);
                }
            }

            // change the output path toos
            img.Save(@"C:\Users\ayanp\Desktop\IMG_20200925_215108.jpg", ImageFormat.Jpeg);
        }

        /// <summary>
        /// draw a squre in a picture
        /// </summary>
        /// <param name="x">x position of the image</param>
        /// <param name="y">y posation of the image</param>
        /// <param name="width">width of the square</param>
        /// <param name="height">height of the square</param>
        /// <param name="color">fill color of the square</param>
        /// <param name="host">image where the square will be drawn</param>
        static void square(int x, int y, int width, int height, Color color, Bitmap host)
        {
            for (int i = x; i < x + height; i++)
            {
                for (int j = y; j < y + width; j++)
                {
                    host.SetPixel(i, j, color);
                }
            }
        }

        /// <summary>
        /// get all pixels data about the image and width and height too
        /// </summary>
        /// <param name="Path"> where the image is stored</param>
        /// <returns></returns>
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
