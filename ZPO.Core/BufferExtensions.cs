using System.Runtime.CompilerServices;

namespace ZPO.Core
{
    public static class BufferExtensions
    {
        /// <summary>
        /// Converts byte buffer to int
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt(this byte[] buffer, int index = 0)
        {
            return buffer[index + 3] << 24 | (buffer[index + 2] << 16) | (buffer[index + 1] << 8) | (buffer[index]);
        }

        /// <summary>
        /// Converts int to byte buffer
        /// </summary>
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
