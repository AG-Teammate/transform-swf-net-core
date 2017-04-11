using Ionic.Zlib;

namespace System
{
    internal class Deflater
    {
        public byte[] Input { get; set; }

        internal int deflate(byte[] compressedDataOut)
        {
            var compressed = ZlibStream.CompressBuffer(Input);
            Array.Copy(compressed, compressedDataOut, compressed.Length);
            return compressed.Length;
        }

        public void finish()
        {
        }

        public void setInput(byte[] image, int i, int length)
        {
            Input = new byte[length - i];
            Array.Copy(image, i, Input, 0, length);
        }
    }

    internal class Inflater
    {
        internal int inflate(byte[] dataOut)
        {
            var compressed = ZlibStream.UncompressBuffer(Input);
            Array.Copy(compressed, dataOut, compressed.Length);
            return compressed.Length;
        }

        public byte[] Input { get; set; }

        internal void end()
        {
        }
    }
}