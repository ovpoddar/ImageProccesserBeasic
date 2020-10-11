namespace ImageProcessorApp.Processors
{
    public interface IImageProcessor
    {
        void ProcessBitmap(string filePath, string savePath);
    }
}