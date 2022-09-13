using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossPlatformSupportOnlyPNG.Models;
internal struct PngChunk
{
    public uint Length { get; set; }
    public char[] Signature { get; set; }
    public byte[] Data { get; set; }
    public byte[] CRC { get; set; }
    public PngChunk(Stream stream)
    {
        var reader = new BinaryReader(stream);

        var len = new byte[4];
        stream.Read(len, 0, len.Length);
        Array.Reverse(len);

        this.Length = BitConverter.ToUInt32(len, 0);
        this.Signature = reader.ReadChars(4);
        this.Data = reader.ReadBytes((int)this.Length);
        this.CRC = reader.ReadBytes(4);
    }
}
