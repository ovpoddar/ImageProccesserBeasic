using System;
using ImageProcessorApp.Handlers;
using ImageProcessorApp.Processors;

namespace ImageProcessorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var bitmapHandler = new BitmapHandler();
            var imageProcessor = new ImageProcessor(bitmapHandler);

            imageProcessor.ProcessBitmap(@"D:\Test\testimage.jpeg", @"D:\Test\output.jpg");

            Console.WriteLine("Complete");
            Console.Read();
        }
    }
}
