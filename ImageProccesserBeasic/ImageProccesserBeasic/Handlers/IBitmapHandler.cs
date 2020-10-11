using ImageProcessorApp.Model;

namespace ImageProcessorApp.Handlers
{
    public interface IBitmapHandler
    {
        IImageDataModel GetImageDataAsync(string path);
    }
}