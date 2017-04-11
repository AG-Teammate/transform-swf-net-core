using System;

namespace com.flagstone.transform.coder
{
    internal class Float
    {
        public static float intBitsToFloat(int bits)
        {
            byte[] bytes = BitConverter.GetBytes(bits);
            return BitConverter.ToSingle(bytes, 0);
        }

        internal static int floatToIntBits(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }
    }
}
