using ImageProcessorApp.Handlers;
using ImageProcessorApp.Processors;
using System.Threading.Tasks;

namespace ImageProcessorApp
{
    class Program
    {
        static Task Main(string[] args)
        {
            var bitmapHandler = new BitmapHandler();
            var imageProcessor = new ImageProcessor(bitmapHandler);

            imageProcessor.ProcessBitmap("", "");
        }
    }
}
