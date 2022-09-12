using ReadingBmpFileManually.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ReadingBmpFileManually.ExtensionMethods;
internal static class LowLeveleConversionExtensition
{
    internal static byte[] Tobytes(this ValueType @struct)
    {
        var result = new byte[Marshal.SizeOf(@struct)];
        Unsafe.As<byte, ValueType>(ref result[0]) = @struct;
        return result;
    }

    internal static T ToStruct<T>(this byte[] @bytes) where T : struct =>
        Unsafe.As<byte, T>(ref @bytes[0]);
}
