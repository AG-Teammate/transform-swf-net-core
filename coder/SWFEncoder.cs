using System;
using System.Collections.Generic;
using System.IO;

/*
 * SWFEncoder.java
 * Transform
 *
 * Copyright (c) 2001-2010 Flagstone Software Ltd. All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *  * Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *  * Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *  * Neither the name of Flagstone Software Ltd. nor the names of its
 *    contributors may be used to endorse or promote products derived from this
 *    software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

namespace com.flagstone.transform.coder
{

    /// <summary>
    /// SWFEncoder wraps an OutputStream with a buffer to reduce the amount of
    /// memory required to encode a movie and to improve efficiency by writing
    /// data to a file or external source in blocks.
    /// </summary>
    

    public sealed class SWFEncoder
    {
        /// <summary>
        /// The default size, in bytes, for the internal buffer. </summary>
        public const int BUFFER_SIZE = 4096;

        /// <summary>
        /// Bit mask applied to bytes when converting to unsigned integers. </summary>
        private const int BYTE_MASK = 255;
        /// <summary>
        /// Number of bits in an int. </summary>
        private const int BITS_PER_INT = 32;
        /// <summary>
        /// Number of bits in a byte. </summary>
        private const int BITS_PER_BYTE = 8;
        /// <summary>
        /// Offset to add to number of bits when calculating number of bytes. </summary>
        private const int ROUND_TO_BYTES = 7;
        /// <summary>
        /// Right shift to convert number of bits to number of bytes. </summary>
        private const int BITS_TO_BYTES = 3;
        /// <summary>
        /// Left shift to convert number of bytes to number of bits. </summary>
        private const int BYTES_TO_BITS = 3;
        /// <summary>
        /// Number of bits to shift when aligning a value to the second byte. </summary>
        private const int TO_BYTE1 = 8;
        /// <summary>
        /// Number of bits to shift when aligning a value to the third byte. </summary>
        private const int TO_BYTE2 = 16;
        /// <summary>
        /// Number of bits to shift when aligning a value to the fourth byte. </summary>
        private const int TO_BYTE3 = 24;


        /// <summary>
        /// The underlying input stream. </summary>

        private readonly Stream stream;
        /// <summary>
        /// The buffer for data read from the stream. </summary>

        private readonly byte[] buffer;
        /// <summary>
        /// The index in bytes to the current location in the buffer. </summary>

        private int index;
        /// <summary>
        /// The offset in bits to the location in the current byte. </summary>

        private int offset;
        /// <summary>
        /// The character encoding used for strings. </summary>

        private string encoding;
        /// <summary>
        /// Stack for storing file locations. </summary>

        private readonly Stack<int?> locations;
        /// <summary>
        /// The position of the buffer relative to the start of the stream. </summary>

        private int pos;

        /// <summary>
        /// Create a new SWFEncoder for the underlying InputStream with the
        /// specified buffer size.
        /// </summary>
        /// <param name="streamOut"> the stream from which data will be written. </param>
        /// <param name="length"> the size in bytes of the buffer. </param>


        public SWFEncoder(Stream streamOut, int length)
        {
            stream = streamOut;
            buffer = new byte[length];
            encoding = CharacterEncoding.UTF8.Encoding;
            locations = new Stack<int?>();
        }

        /// <summary>
        /// Create a new SWFEncoder for the underlying InputStream using the
        /// default buffer size.
        /// </summary>
        /// <param name="streamOut"> the stream from which data will be written. </param>


        public SWFEncoder(Stream streamOut)
        {
            stream = streamOut;
            buffer = new byte[BUFFER_SIZE];
            encoding = CharacterEncoding.UTF8.Encoding;
            locations = new Stack<int?>();
        }

        /// <summary>
        /// Sets the character encoding scheme used when encoding or decoding
        /// strings.
        /// </summary>
        /// <param name="enc">
        ///            the CharacterEncoding that identifies how strings are encoded. </param>


        public CharacterEncoding Encoding
        {
            set => encoding = value.Encoding;
        }

        /// <summary>
        /// Remember the current position. </summary>
        /// <returns> the current position. </returns>
        public int mark()
        {
            locations.Push(pos + index);
            return locations.Peek().Value;
        }

        /// <summary>
        /// Discard the last saved position.
        /// </summary>
        public void unmark()
        {
            locations.Pop();
        }

        /// <summary>
        /// Compare the number of bytes read with the expected number and throw an
        /// exception if there is a difference.
        /// </summary>
        /// <param name="expected"> the expected number of bytes read.
        /// </param>
        /// <exception cref="CoderException"> if the number of bytes read is different from the
        /// expected number. </exception>



        public void check(int expected)
        {


            int actual = (pos + index) - (int)locations.Peek();
            if (actual != expected)
            {
                throw new CoderException((int)locations.Peek(), expected, actual - expected);
            }
        }

        /// <summary>
        /// Changes the location to the next byte boundary.
        /// </summary>
        public void alignToByte()
        {
            if (offset > 0)
            {
                index += 1;
                offset = 0;
            }
        }

        /// <summary>
        /// Write the data currently stored in the buffer to the underlying stream. </summary>
        /// <exception cref="IOException"> if an error occurs while writing the data to the
        /// stream. </exception>


        public void flush()
        {
            stream.Write(buffer, 0, index);
            stream.Flush();

            int diff;
            if (offset == 0)
            {
                diff = 0;
            }
            else
            {
                diff = 1;
                buffer[0] = buffer[index];
            }

            for (int i = diff; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }

            pos += index;
            index = 0;
        }

        /// <summary>
        /// Write a value to bit field.
        /// </summary>
        /// <param name="value">
        ///            the value. </param>
        /// <param name="numberOfBits">
        ///            the (least significant) number of bits that will be written. </param>
        /// <exception cref="IOException"> if there is an error writing data to the underlying
        /// stream. </exception>



        public void writeBits(int value, int numberOfBits)
        {



            int ptr = (index << BYTES_TO_BITS) + offset + numberOfBits;

            if (ptr >= (buffer.Length << BYTES_TO_BITS))
            {
                flush();
            }



            int val = ((int)((uint)(value << (BITS_PER_INT - numberOfBits)) >> offset)) | (buffer[index] << TO_BYTE3);
            int @base = BITS_PER_INT - (((int)((uint)(offset + numberOfBits + ROUND_TO_BYTES) >> BITS_TO_BYTES)) << BYTES_TO_BITS);
            @base = @base < 0 ? 0 : @base;

            int pointer = (index << BYTES_TO_BITS) + offset;

            for (int i = 24; i >= @base; i -= BITS_PER_BYTE)
            {
                buffer[index++] = (byte)((int)((uint)val >> i));
            }

            if (offset + numberOfBits > BITS_PER_INT)
            {
                buffer[index] = (byte)(value << (BITS_PER_BYTE - (offset + numberOfBits - BITS_PER_INT)));
            }

            pointer += numberOfBits;
            index = (int)((uint)pointer >> BITS_TO_BYTES);
            offset = pointer & Coder.LOWEST3;
        }

        /// <summary>
        /// Write a byte.
        /// </summary>
        /// <param name="value">
        ///            the value to be written - only the least significant byte will
        ///            be written. </param>
        /// <exception cref="IOException"> if there is an error writing data to the underlying
        /// stream. </exception>



        public void writeByte(int value)
        {
            if (index == buffer.Length)
            {
                flush();
            }
            buffer[index++] = (byte)value;
        }

        /// <summary>
        /// Write an array of bytes.
        /// </summary>
        /// <param name="bytes">
        ///            the array to be written.
        /// </param>
        /// <returns> the number of bytes written. </returns>
        /// <exception cref="IOException"> if there is an error reading data from the underlying
        /// stream. </exception>



        public int writeBytes(byte[] bytes)
        {
            if (index + bytes.Length < buffer.Length)
            {
                Array.Copy(bytes, 0, buffer, index, bytes.Length);
                index += bytes.Length;
            }
            else
            {
                flush();
                stream.Write(bytes, 0, bytes.Length);
                pos += bytes.Length;
            }
            return bytes.Length;
        }

        /// <summary>
        /// Write a string using the default character set defined in the encoder.
        /// </summary>
        /// <param name="str">
        ///            the string. </param>
        /// <exception cref="IOException"> if there is an error reading data from the underlying
        /// stream. </exception>



        public void writeString(string str)
        {
            try
            {
                writeBytes(str.GetBytes(encoding));
                buffer[index++] = 0;
            }


            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Write a 16-bit integer.
        /// </summary>
        /// <param name="value">
        ///            an integer containing the value to be written. </param>
        /// <exception cref="IOException"> if there is an error reading data from the underlying
        /// stream. </exception>



        public void writeShort(int value)
        {
            if (index + 2 > buffer.Length)
            {
                flush();
            }
            buffer[index++] = (byte)value;
            buffer[index++] = (byte)((int)((uint)value >> TO_BYTE1));
        }

        /// <summary>
        /// Write a 32-bit integer.
        /// </summary>
        /// <param name="value">
        ///            an integer containing the value to be written. </param>
        /// <exception cref="IOException"> if there is an error reading data from the underlying
        /// stream. </exception>



        public void writeInt(int value)
        {
            if (index + 4 > buffer.Length)
            {
                flush();
            }
            buffer[index++] = (byte)value;
            buffer[index++] = (byte)((int)((uint)value >> TO_BYTE1));
            buffer[index++] = (byte)((int)((uint)value >> TO_BYTE2));
            buffer[index++] = (byte)((int)((uint)value >> TO_BYTE3));
        }

        /// <summary>
        /// Write a 32-bit unsigned integer, encoded in a variable number of bytes.
        /// </summary>
        /// <param name="value">
        ///            an integer containing the value to be written. </param>
        /// <exception cref="IOException"> if there is an error reading data from the underlying
        /// stream. </exception>



        public void writeVarInt(int value)
        {
            if (index + 5 > buffer.Length)
            {
                flush();
            }

            int val = value;
            while (val > Coder.VAR_INT_MAX)
            {
                buffer[index++] = (byte)((val & Coder.LOWEST7) | Coder.BIT7);
                val = (int)((uint)val >> Coder.VAR_INT_SHIFT);
            }
            buffer[index++] = (byte)(val & Coder.LOWEST7);
        }

        /// <summary>
        /// Number of bits to shift to obtain the exponent in a half-precision
        /// floating-point value.
        /// </summary>
        private const int HALF_EXP_SHIFT = 10;
        /// <summary>
        /// The offset to apply to the exponent in a half-precision
        /// floating-point value.
        /// </summary>
        private const int HALF_EXP_OFFSET = 15;
        /// <summary>
        /// The maximum value of the exponent in a half-precision
        /// floating-point value.
        /// </summary>
        private const int HALF_EXP_MAX = 31;
        /// <summary>
        /// The bit pattern used to represent Infinity in a half-precision
        /// floating-point value.
        /// </summary>
        private const int HALF_INF = 0x7C00;

        /// <summary>
        /// Number of bits to shift to obtain the exponent in a single-precision
        /// floating-point value.
        /// </summary>
        private const int EXP_SHIFT = 23;
        /// <summary>
        /// The maximum value of the exponent in a single-precision
        /// floating-point value.
        /// </summary>
        private const int EXP_MAX = 127;
        /// <summary>
        /// Number of bits to shift to obtain the mantissa in a single-precision
        /// float-point value.
        /// </summary>
        private const int MANT_SHIFT = 13;
        /// <summary>
        /// Mask to obtain the mantissa in a single-precision floating-point value.
        /// </summary>
        private const int LOWEST23 = 0x007fffff;
        /// <summary>
        /// Mask to obtain the most significant bit of the mantissa in a
        /// single-precision floating-point value.
        /// </summary>
        private const int BIT23 = 0x00800000;

        /// <summary>
        /// Write a single-precision floating point number.
        /// </summary>
        /// <param name="value">
        ///            the value to be written. </param>
        /// <exception cref="IOException"> if there is an error reading data from the underlying
        /// stream. </exception>



        public void writeHalf(float value)
        {


            int intValue = Float.floatToIntBits(value);


            int sign = (intValue >> Coder.ALIGN_BYTE2) & Coder.BIT15;


            int exponent = ((intValue >> EXP_SHIFT) & BYTE_MASK) - (EXP_MAX - HALF_EXP_OFFSET);
            int mantissa = intValue & LOWEST23;

            if (exponent <= 0)
            {
                if (exponent < -10)
                {
                    writeShort(0);
                }
                else
                {
                    mantissa = (mantissa | BIT23) >> (1 - exponent);
                    writeShort((sign | (mantissa >> MANT_SHIFT)));
                }
            }
            else if (exponent == 0xff - (EXP_MAX - HALF_EXP_OFFSET))
            {
                if (mantissa == 0)
                { // Inf
                    writeShort(sign | HALF_INF);
                }
                else
                { // NAN
                    mantissa >>= MANT_SHIFT;
                    writeShort((sign | HALF_INF | mantissa | ((mantissa == 0) ? 1 : 0)));
                }
            }
            else
            {
                if (exponent >= HALF_EXP_MAX)
                { // Overflow
                    writeShort((sign | HALF_INF));
                }
                else
                {
                    writeShort((sign | (exponent << HALF_EXP_SHIFT) | (mantissa >> MANT_SHIFT)));
                }
            }
        }
    }

}