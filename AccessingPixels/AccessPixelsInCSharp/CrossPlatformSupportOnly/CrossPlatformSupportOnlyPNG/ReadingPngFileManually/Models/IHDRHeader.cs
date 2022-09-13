using System;
using System.Runtime.InteropServices;

namespace CrossPlatformSupportOnlyPNG.Models;

internal struct IHDRHeader
{
    public UInt32 Width { get; set; }
    public UInt32 Height { get; set; }
    public byte BitDepth { get; set; }
    public byte ColoeType { get; set; }
    public byte CompressionMethod { get; set; }
    public byte FilterMethod { get; set; }
    public byte InterlaceMethod { get; set; }

    public IHDRHeader(Span<byte> span)
    {
        if (span.Length != 13)
            throw new ArgumentException("invalid data stream");

        var _width = span.Slice(0, 4);
        _width.Reverse();
        var _height = span.Slice(4, 4);
        _height.Reverse();

        this.Width = BitConverter.ToUInt32(_width);
        this.Height = BitConverter.ToUInt32(_height);
        this.BitDepth = span[8];
        this.ColoeType = span[9];
        this.CompressionMethod = span[10];
        this.FilterMethod = span[11];
        this.InterlaceMethod = span[12];
    }
}
