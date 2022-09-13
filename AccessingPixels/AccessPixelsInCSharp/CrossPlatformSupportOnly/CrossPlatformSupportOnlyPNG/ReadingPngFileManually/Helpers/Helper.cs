using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrossPlatformSupportOnlyPNG.Helpers;
internal static class Helper
{
    [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern int memcmp(IntPtr a1, IntPtr a2, uint count);

    public static unsafe bool Equal(byte[] data1, byte[] data2)
    {
        fixed (byte* p1 = data1, p2 = data2)
            return memcmp((IntPtr)p1, (IntPtr)p2, (uint)data1.Length * sizeof(byte)) == 0;
    }

    internal static byte[] Tobytes(this ValueType @struct)
    {
        var result = new byte[Marshal.SizeOf(@struct)];
        Unsafe.As<byte, ValueType>(ref result[0]) = @struct;
        return result;
    }

    internal static T ToStruct<T>(this byte[] @bytes) where T : struct =>
        Unsafe.As<byte, T>(ref @bytes[0]);
}
