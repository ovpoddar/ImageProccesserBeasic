using CrossPlatformSupportOnlyPNG.Helpers;
using CrossPlatformSupportOnlyPNG.Models;

Console.WriteLine("Please Provide a .PNG File.");
var inputFile = Console.ReadLine();
if (!File.Exists(inputFile))
    return;

using (var reader = File.OpenRead(inputFile))
{
    var pngSignature = new byte[8]
    {
        137, 80, 78, 71, 13,10 , 26, 10
    };


    var header = new byte[8];
    reader.Read(header, 0, header.Length);

    if (!Helper.Equal(pngSignature, header))
        Console.WriteLine("invalid file");
    else
    {
        Console.WriteLine("validate sin=gnature successfully");
        while (true)
        {
            var a = new PngChunk(reader);
            if (a.Signature.AsSpan().ToString() == DataBlocks.IEND.ToString())
                break;
            if (a.Signature.AsSpan().ToString() == DataBlocks.IHDR.ToString())
                ReadHeader(a.Data);
        }

        Console.Read();
    }
}

void ReadHeader(byte[] data)
{
    var d = new IHDRHeader(data);
    Console.WriteLine("found header successfully.");
}