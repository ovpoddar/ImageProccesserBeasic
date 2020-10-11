using ImageProcessorApp.Handlers;
using ImageProcessorApp.Processors;
using System.Threading.Tasks;

namespace ImageProcessorApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bitmapHandler = new BitmapHandler();
            var imageProcessor = new ImageProcessor(bitmapHandler);

            imageProcessor.ProcessBitmap("", "");
        }
    }
}
