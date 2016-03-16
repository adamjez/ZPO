using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZPO.Core
{
    public static class BufferExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this byte[] buffer, int index = 0)
        {
            return buffer[index + 3] << 24 | (buffer[index + 2] << 16) | (buffer[index + 1] << 8) | (buffer[index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToArray(this int resultColor, byte[] resultBuffer, int index)
        {
            resultBuffer[index] = (byte)resultColor;
            resultBuffer[index + 1] = (byte)(resultColor >> 8);
            resultBuffer[index + 2] = (byte)(resultColor >> 16);
            resultBuffer[index + 3] = (byte)(resultColor >> 24);
        }

    }
}
