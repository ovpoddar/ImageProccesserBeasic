using ReadingBmpFileManually.ExtensionMethods;
using ReadingBmpFileManually.Models;


Console.WriteLine("Please Provide a .Bmp File. of 24 bit.");
var inputFile = Console.ReadLine();
if (!File.Exists(inputFile))
    return;

using (var reader = File.OpenRead(inputFile))
{
    byte[] bytesFile = new BmpFileHeader().Tobytes();
    reader.Read(bytesFile, 0, bytesFile.Length);
    BmpFileHeader hex = bytesFile.ToStruct<BmpFileHeader>();
    if (hex.file_type == 0x4D42)
    {
        Console.WriteLine(hex.ToString());
        System.Console.WriteLine(reader.Position);
    }
    Console.WriteLine("header file read success fully");


    byte[] header = new DibFileHeader().Tobytes();
    reader.Read(header, 0, header.Length);
    reader.Seek(hex.offset_data, SeekOrigin.Begin);
    DibFileHeader hex2 = header.ToStruct<DibFileHeader>();
    System.Console.WriteLine("file Header read successfully");

    reader.Position = hex.offset_data;
    Console.WriteLine("Skip the color profile");

    byte[] pixelData = new byte[hex2.Height * hex2.Width * hex2.BitCount / 8];
    if (hex2.Width % 4 == 0)
    {
        reader.Read(pixelData, 0, pixelData.Length);
    }
    else
    {
        int singleRowWidthWithoutPadding = hex2.Width * hex2.BitCount / 8;
        var chunk = new byte[singleRowWidthWithoutPadding];
        int singleRowWidthWithPadding = AddPadding(singleRowWidthWithoutPadding);
        byte[] pixelPaddingData = new byte[singleRowWidthWithPadding - singleRowWidthWithoutPadding];

        for (int i = 0; i < hex2.Height; ++i)
        {
            reader.Read(chunk, 0, singleRowWidthWithoutPadding);
            reader.Read(pixelPaddingData, 0, pixelPaddingData.Length);
            Array.Copy(chunk, 0, pixelData, (i * singleRowWidthWithoutPadding), singleRowWidthWithoutPadding);
        }
    }

    for(var i = 0; i < pixelData.Length; i++)
    {
        if (i % hex2.Width == 0)
            Console.WriteLine();
        Console.Write(pixelData[i] + " ");
    }

}

static int AddPadding(int rowWithoutPadding)
{
    var new_stride = rowWithoutPadding;
    while (new_stride % 4 != 0)
    {
        new_stride++;
    }
    return new_stride;
}