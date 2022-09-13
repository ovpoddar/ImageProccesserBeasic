namespace CrossPlatformSupportOnlyPNG.Models;
/// <summary>
/// All the data blocks which exist a png file
/// </summary>
internal enum DataBlocks
{
    IHDR,
    cHRM,
    gAMA,
    sBIT,
    PLTE,
    bKGD,
    hIST,
    tRNS,
    oFFs,
    pHYs,
    sCAL,
    IDAT,
    tIME,
    tEXt,
    zTXt,
    fRAc,
    gIFg,
    gIFt,
    gIFx,
    /// <summary>
    /// End block of the png file
    /// </summary>
    IEND
}
