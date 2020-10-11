using System.Drawing;

namespace ImageProcessorApp.Extensions
{
    public static class BitmapExtensions
    {
        public static void SetBorder(this Bitmap bitmap)
        {
            for (var i = 1000; i < 3000; i++)
            {
                for (var j = 2000; j < 3000; j++)
                    bitmap.SetPixel(j, i, Color.White);
            }
        }
    }
}
