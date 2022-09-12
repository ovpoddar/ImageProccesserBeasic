using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ReadingBmpFileManually.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct BmpFileHeader
    {
        public ushort file_type { get; set; }
        public uint file_size { get; set; }
        public ushort reserved1 { get; set; }
        public ushort reserved2 { get; set; }
        public uint offset_data { get; set; }
    }

}