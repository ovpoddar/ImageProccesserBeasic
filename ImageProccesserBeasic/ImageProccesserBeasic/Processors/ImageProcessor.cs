using System.Drawing.Imaging;
using ImageProcessorApp.Extensions;
using ImageProcessorApp.Handlers;

namespace ImageProcessorApp.Processors
{
    public class ImageProcessor : IImageProcessor
    {
        private readonly IBitmapHandler _bitmapHandler;

        public ImageProcessor(IBitmapHandler bitmapHandler) =>
            _bitmapHandler = bitmapHandler;

        public void ProcessBitmap(string filePath, string savePath)
        {
            var imageDataModel = _bitmapHandler.GetImageDataAsync(filePath);

            var bitmap = imageDataModel.BuildBitmap();

            bitmap.SetBorder();

           bitmap.Save(savePath, ImageFormat.Jpeg);
        }
    }
}
