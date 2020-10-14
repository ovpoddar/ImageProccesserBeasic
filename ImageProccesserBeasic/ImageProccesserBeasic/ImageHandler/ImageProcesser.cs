using ImageProccesserBeasic.EffectHandler;
using ImageProccesserBeasic.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageProccesserBeasic.ImageHandler
{
    public class ImageProcesser : IImageProcesser
    {
        private readonly string _image;
        private readonly string _output;
        private ImageDataModel _processData;

        public ImageProcesser(string Image, string outputpath)
        {
            _image = Image;
            _output = outputpath;
        }
        public async Task ApplyPixelBlur(int sampleSize)
        {
            await Task.Run(() =>
            {
                if (sampleSize >= _processData.Height || sampleSize >= _processData.Width)
                    throw new Exception("Invalid Ammount");

                // resize the photo
                var w = Math.Ceiling((decimal)_processData.Width / sampleSize);
                var h = Math.Ceiling((decimal)_processData.Height / sampleSize);

                int width = (int)(w * sampleSize);
                int height = (int)(h * sampleSize);

                // create a blank resized image
                var img = new Bitmap(width, height);


                // make a squere in that place
                for (int i = 0; i < _processData.Height; i += sampleSize)
                {
                    for (int j = 0; j < _processData.Width; j += sampleSize)
                        ApplyPixelBlurHandler.Square(j, i, sampleSize, sampleSize, _processData.Data[i][j], img);
                }

                // change the output path toos
                img.Save(_output, ImageFormat.Jpeg);
            });
        }


        /// <summary>
        /// get all pixels data about the image and width and height too
        /// </summary>
        /// <param name="Path"> where the image is stored</param>
        /// <returns>return ImageDataModel</returns>
        public async Task ProcessImage()
        {
            await Task.Run(() =>
            {
                using (Bitmap bmp = new Bitmap(_image))
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
                    _processData = new ImageDataModel()
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
