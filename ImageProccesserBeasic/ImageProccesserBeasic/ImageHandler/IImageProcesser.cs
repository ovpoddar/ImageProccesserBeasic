using System.Threading.Tasks;

namespace ImageProccesserBeasic.ImageHandler
{
    public interface IImageProcesser
    {
        Task ProcessImage();
        Task ApplyPixelBlur(int sampleSize);
    }
}
