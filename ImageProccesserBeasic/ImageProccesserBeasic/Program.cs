using ImageProccesserBeasic.ImageHandler;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ImageProccesserBeasic
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Image Path:-");
            var inputPath = Path.GetFullPath(Console.ReadLine());
            try
            {
                if (".jpg" != Path.GetExtension(inputPath) && ".jpeg" != Path.GetExtension(inputPath) && ".png" != Path.GetExtension(inputPath))
                    throw new Exception("not a valid format");
                var outputPath = Path.Combine(Path.GetDirectoryName(inputPath), new Guid() + Path.GetExtension(inputPath));
                var ImageProcesser = new ImageProcesser(inputPath, outputPath);
                await ImageProcesser.ProcessImage();
                Console.WriteLine("select the ammount:-");
                await ImageProcesser.ApplyPixelBlur(Convert.ToInt32(Console.ReadLine()));
                Console.WriteLine("Complete..." + outputPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
