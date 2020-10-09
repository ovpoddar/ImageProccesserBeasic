using System.Collections.Generic;
using System.Drawing;

namespace ImageProccesserBeasic.Model
{
    class ImageDataModel
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public List<List<Color>> Data { get; set; }
    }
}
