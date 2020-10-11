using ImageProcessorApp.Handlers;
using ImageProcessorApp.Processors;
using System.Threading.Tasks;

namespace ImageProcessorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var bitmapHandler = new BitmapHandler();
            var imageProcessor = new ImageProcessor(bitmapHandler);

            imageProcessor.ProcessBitmap(@"D:\Test\Helium8af84bc6-e79c-4962-b48c-5ba7166efc83.jpg", @"D:\Test\Helium.jpg");
        }
    }
}
