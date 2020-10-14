using System.Drawing;

namespace ImageProccesserBeasic.EffectHandler
{
    public static class ApplyPixelBlurHandler
    {
        /// <summary>
        /// draw a squre in a picture
        /// </summary>
        /// <param name="x">x position of the image</param>
        /// <param name="y">y posation of the image</param>
        /// <param name="width">width of the square</param>
        /// <param name="height">height of the square</param>
        /// <param name="color">fill color of the square</param>
        /// <param name="host">image where the square will be drawn</param>
        public static void Square(int x, int y, int width, int height, Color color, Bitmap host)
        {
            for (int i = x; i < x + height; i++)
            {
                for (int j = y; j < y + width; j++)
                {
                    host.SetPixel(i, j, color);
                }
            }
        }
    }
}
