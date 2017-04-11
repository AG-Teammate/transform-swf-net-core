using System;
using System.Collections.Generic;
using System.IO;

/*
 * SWFDecoder.java
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
	/// SWFDecoder wraps an InputStream with a buffer to reduce the amount of
	/// memory required to decode a movie and to improve efficiency by reading
	/// data from a file or external source in blocks.
	/// </summary>


	public sealed class SWFDecoder
	{
		/// <summary>
		/// The default size, in bytes, for the internal buffer. </summary>
		public const int BUFFER_SIZE = 4096;

		/// <summary>
		/// The default size, in bytes, for the reading strings. </summary>
		private const int STR_BUFFER_SIZE = 1024;
		/// <summary>
		/// Bit mask applied to bytes when converting to unsigned integers. </summary>
		private const int BYTE_MASK = 255;
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
		/// Number of bits in an int. </summary>
		private const int BITS_PER_INT = 32;
		/// <summary>
		/// Number of bits in a byte. </summary>
		private const int BITS_PER_BYTE = 8;
		/// <summary>
		/// Right shift to convert number of bits to number of bytes. </summary>
		private const int BITS_TO_BYTES = 3;
		/// <summary>
		/// Left shift to convert number of bytes to number of bits. </summary>
		private const int BYTES_TO_BITS = 3;

		/// <summary>
		/// The underlying input stream. </summary>
		
		private readonly Stream stream;
		/// <summary>
		/// The buffer for data read from the stream. </summary>
		
		private readonly byte[] buffer;
		/// <summary>
		/// A buffer used for reading null terminated strings. </summary>
		
		private byte[] stringBuffer;
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
		/// The position from the start of the buffer. </summary>
		
		private int index;
		/// <summary>
		/// The offset in bits in the current buffer location. </summary>
		
		private int offset;
		/// <summary>
		/// The number of bytes available in the current buffer. </summary>
		
		private int size;
		/// <summary>
		/// The starting location from the last check-point. </summary>
		
		private int location;
		/// <summary>
		/// The expected number number of bytes to be decoded. </summary>
		
		private int expected;
		/// <summary>
		/// The difference from the expected number. </summary>
		
		private int delta;

		/// <summary>
		/// Create a new SWFDecoder for the underlying InputStream with the
		/// specified buffer size.
		/// </summary>
		/// <param name="streamIn"> the stream from which data will be read. </param>
		/// <param name="length"> the size in bytes of the buffer. </param>


		public SWFDecoder(Stream streamIn, int length)
		{
			stream = streamIn;
			buffer = new byte[length];
			stringBuffer = new byte[STR_BUFFER_SIZE];
			encoding = CharacterEncoding.UTF8.Encoding;
			locations = new Stack<int?>();
		}

		/// <summary>
		/// Create a new SWFDecoder for the underlying InputStream using the
		/// default buffer size.
		/// </summary>
		/// <param name="streamIn"> the stream from which data will be read. </param>


		public SWFDecoder(Stream streamIn)
		{
			stream = streamIn;
			buffer = new byte[BUFFER_SIZE];
			stringBuffer = new byte[BUFFER_SIZE];
			encoding = CharacterEncoding.UTF8.Encoding;
			locations = new Stack<int?>();
		}

		/// <summary>
		/// Fill the internal buffer. Any unread bytes are copied to the start of
		/// the buffer and the remaining space is filled with data from the
		/// underlying stream.
		/// </summary>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>


		public void fill()
		{


			int diff = size - index;
			pos += index;

			if (index < size)
			{
				for (int i = 0; i < diff; i++)
				{
					buffer[i] = buffer[index++];
				}
			}

			int bytesRead = 0;
			int bytesToRead = buffer.Length - diff;

			index = diff;
			size = diff;

			do
			{
				bytesRead = stream.Read(buffer, index, bytesToRead);
				if (bytesRead == 0)
				{
					bytesToRead = 0;
				}
				else
				{
					index += bytesRead;
					size += bytesRead;
					bytesToRead -= bytesRead;
				}
			} while (bytesToRead > 0);

			index = 0;
		}

		/// <summary>
		/// Remember the current position. </summary>
		/// <returns> the current position. </returns>
		public int mark() //AG unsure
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
		/// Reposition the decoder to the point recorded by the last call to the
		/// mark() method.
		/// </summary>
		/// <exception cref="IOException"> if the internal buffer was filled after mark() was
		/// called. </exception>


		public void reset()
		{
			int last;

			if (locations.Count == 0)
			{
				last = 0;
			}
			else
			{
				last = (int)locations.Peek();
			}
			if (last - pos < 0)
			{
				throw new IOException();
			}
			index = last - pos;
		}

		/// <summary>
		/// Compare the number of bytes read since the last saved position and
		/// record any difference.
		/// </summary>
		/// <param name="count"> the expected number of bytes read. </param>


		public void check(int count)
		{
			expected = count;
			location = (int)locations.Peek();
			delta = count - ((pos + index) - location);
		}

		/// <summary>
		/// Get the location recorded for the last call to check(). </summary>
		/// <returns> the position in the buffer of the call to mark() used by
		/// check(). </returns>
		public int Location => location;

	    /// <summary>
		/// Get the expected number of bytes from the last call to check(). </summary>
		/// <returns> the difference from the expected number of bytes decoded. </returns>
		public int Expected => expected;

	    /// <summary>
		/// Get the difference from the expected number of bytes from the last call
		/// to check(). </summary>
		/// <returns> the difference from the expected number of bytes decoded. </returns>
		public int Delta => delta;

	    /// <summary>
		/// Get the number of bytes read from the last saved position.
		/// </summary>
		/// <returns> the number of bytes read since the mark() method was last called. </returns>
		public int bytesRead()
		{
			return (pos + index) - (int)locations.Peek();
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
		/// Skips over and discards n bytes of data.
		/// </summary>
		/// <param name="count"> the number of bytes to skip.
		/// </param>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>



		public void skip(int count)
		{
			if (size - index == 0)
			{
				fill();
			}
			if (count < size - index)
			{
				index += count;
			}
			else
			{
				int toSkip = count;
				int diff;
				while (toSkip > 0)
				{
					diff = size - index;
					if (toSkip <= diff)
					{
						index += toSkip;
						toSkip = 0;
					}
					else
					{
						index += diff;
						toSkip -= diff;
						fill();
						if (size - index == 0)
						{
							throw new IndexOutOfRangeException();
						}
					}
				}
			}
		}

		/// <summary>
		/// Read a bit field.
		/// </summary>
		/// <param name="numberOfBits">
		///            the number of bits to read.
		/// </param>
		/// <param name="signed">
		///            indicates whether the integer value read is signed.
		/// </param>
		/// <returns> the value read.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>



		public int readBits(int numberOfBits, bool signed)
		{

			int pointer = (index << BYTES_TO_BITS) + offset;

			if (((size << BYTES_TO_BITS) - pointer) < numberOfBits)
			{
				fill();
				pointer = (index << BYTES_TO_BITS) + offset;
			}

			int value = 0;

			if (numberOfBits > 0)
			{

				if (pointer + numberOfBits > (size << BYTES_TO_BITS))
				{
					throw new IndexOutOfRangeException();
				}

				for (int i = BITS_PER_INT; (i > 0) && (index < buffer.Length); i -= BITS_PER_BYTE)
				{
					value |= (buffer[index++] & BYTE_MASK) << (i - BITS_PER_BYTE);
				}

				value <<= offset;

				if (signed)
				{
					value >>= BITS_PER_INT - numberOfBits;
				}
				else
				{
					value = (int)((uint)value >> BITS_PER_INT - numberOfBits);
				}

				pointer += numberOfBits;
				index = (int)((uint)pointer >> BITS_TO_BYTES);
				offset = pointer & Coder.LOWEST3;
			}

			return value;
		}

		/// <summary>
		/// Read-ahead a bit field.
		/// </summary>
		/// <param name="numberOfBits">
		///            the number of bits to read.
		/// </param>
		/// <param name="signed">
		///            indicates whether the integer value read is signed.
		/// </param>
		/// <returns> the value read.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>



		public int scanBits(int numberOfBits, bool signed)
		{

			int pointer = (index << BYTES_TO_BITS) + offset;

			if (((size << BYTES_TO_BITS) - pointer) < numberOfBits)
			{
				fill();
				pointer = (index << BYTES_TO_BITS) + offset;
			}

			int value = 0;

			if (numberOfBits > 0)
			{

				if (pointer + numberOfBits > (size << BYTES_TO_BITS))
				{
					throw new IndexOutOfRangeException();
				}

				for (int i = BITS_PER_INT; (i > 0) && (index < buffer.Length); i -= BITS_PER_BYTE)
				{
					value |= (buffer[index++] & BYTE_MASK) << (i - BITS_PER_BYTE);
				}

				value <<= offset;

				if (signed)
				{
					value >>= BITS_PER_INT - numberOfBits;
				}
				else
				{
					value = (int)((uint)value >> BITS_PER_INT - numberOfBits);
				}

				index = (int)((uint)pointer >> BITS_TO_BYTES);
				offset = pointer & Coder.LOWEST3;
			}

			return value;
		}

		/// <summary>
		/// Read an unsigned byte but do not advance the internal pointer.
		/// </summary>
		/// <returns> an 8-bit unsigned value.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>


		public int scanByte()
		{
			if (size - index < 1)
			{
				fill();
			}
			if (index + 1 > size)
			{
				throw new IndexOutOfRangeException();
			}
			return buffer[index] & BYTE_MASK;
		}

		/// <summary>
		/// Read an unsigned byte.
		/// </summary>
		/// <returns> an 8-bit unsigned value.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>


		public int readByte()
		{
			if (size - index < 1)
			{
				fill();
			}
			if (index + 1 > size)
			{
				throw new IndexOutOfRangeException();
			}
			return buffer[index++] & BYTE_MASK;
		}

		/// <summary>
		/// Reads an array of bytes.
		/// </summary>
		/// <param name="bytes">
		///            the array that will contain the bytes read.
		/// </param>
		/// <returns> the array of bytes.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>



		public byte[] readBytes(byte[] bytes)
		{


			int wanted = bytes.Length;
			int dest = 0;
			int read = 0;

			int available;
			int remaining;

			while (read < wanted)
			{
				available = size - index;
				remaining = wanted - read;
				if (available > remaining)
				{
					available = remaining;
				}
				Array.Copy(buffer, index, bytes, dest, available);
				read += available;
				index += available;
				dest += available;

				if (index == size)
				{
					fill();
				}
			}
			return bytes;
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
		/// Read a string using the default character set defined in the decoder.
		/// </summary>
		/// <param name="length">
		///            the number of bytes to read.
		/// </param>
		/// <returns> the decoded string.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>



		public string readString(int length)
		{


			byte[] bytes = new byte[length];
			readBytes(bytes);
			int len;
			if (bytes[length - 1] == 0)
			{
				len = length - 1;
			}
			else
			{
				len = length;
			}
			return StringHelperClass.NewString(bytes, 0, len, encoding);
		}

		/// <summary>
		/// Read a null-terminated string using the default character set defined in
		/// the decoder.
		/// </summary>
		/// <returns> the decoded string.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>


		public string readString()
		{
			int start = index;
			int length = 0;
			int available;
			int dest = 0;
			bool finished = false;
			int count;

			while (!finished)
			{
				available = size - index;
				if (available == 0)
				{
					fill();
					available = size - index;
				}
				start = index;
				count = 0;
				for (int i = 0; i < available; i++)
				{
					if (buffer[index++] == 0)
					{
						finished = true;
						break;
					}
				    length++;
				    count++;
				}
				if (stringBuffer.Length < length)
				{
					stringBuffer = Arrays.copyOf(stringBuffer, length << 2);
				}
				Array.Copy(buffer, start, stringBuffer, dest, count);
				dest += length;
			}
			return StringHelperClass.NewString(stringBuffer, 0, length, encoding);
		}

		/// <summary>
		/// Read an unsigned 16-bit integer.
		/// </summary>
		/// <returns> the value read.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>


		public int scanUnsignedShort()
		{
			if (size - index < 2)
			{
				fill();
			}
			if (index + 2 > size)
			{
				throw new IndexOutOfRangeException();
			}
			int value = buffer[index] & BYTE_MASK;
			value |= (buffer[index + 1] & BYTE_MASK) << TO_BYTE1;
			return value;
		}

		/// <summary>
		/// Read an unsigned 16-bit integer.
		/// </summary>
		/// <returns> the value read.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>


		public int readUnsignedShort()
		{
			if (size - index < 2)
			{
				fill();
			}
			if (index + 2 > size)
			{
				throw new IndexOutOfRangeException();
			}
			int value = buffer[index++] & BYTE_MASK;
			value |= (buffer[index++] & BYTE_MASK) << TO_BYTE1;
			return value;
		}

		/// <summary>
		/// Read an unsigned 16-bit integer.
		/// </summary>
		/// <returns> the value read.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>


		public int readSignedShort()
		{
			if (size - index < 2)
			{
				fill();
			}
			if (index + 2 > size)
			{
				throw new IndexOutOfRangeException();
			}
			int value = buffer[index++] & BYTE_MASK;
			value |= buffer[index++] << TO_BYTE1;
			return value;
		}

		/// <summary>
		/// Read an unsigned 32-bit integer.
		/// </summary>
		/// <returns> the value read.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>


		public int readInt()
		{
			if (size - index < 4)
			{
				fill();
			}
			if (index + 4 > size)
			{
				throw new IndexOutOfRangeException();
			}
			int value = buffer[index++] & BYTE_MASK;
			value |= (buffer[index++] & BYTE_MASK) << TO_BYTE1;
			value |= (buffer[index++] & BYTE_MASK) << TO_BYTE2;
			value |= (buffer[index++] & BYTE_MASK) << TO_BYTE3;
			return value;
		}

		/// <summary>
		/// Read a 32-bit unsigned integer, encoded using a variable number of bytes.
		/// </summary>
		/// <returns> the value read.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>


		public int readVarInt()
		{

			if (size - index < 5)
			{
				fill();
			}

			int value = buffer[index++] & BYTE_MASK;
			const int mask = -1;
			int test = Coder.BIT7;
			int step = Coder.VAR_INT_SHIFT;

			while ((value & test) != 0)
			{
                value = ((buffer[index++] & BYTE_MASK) << step) + (value & (int)(unchecked((uint)mask) >> (32 - step)));
				test <<= Coder.VAR_INT_SHIFT;
				step += Coder.VAR_INT_SHIFT;
			}
			return value;
		}

		/// <summary>
		/// Number of bits to shift to obtain the sign in a half-precision
		/// float-point value.
		/// </summary>
		private const int HALF_SIGN_SHIFT = 15;
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
		/// Number of bits to shift to obtain the sign in a single-precision
		/// float-point value.
		/// </summary>
		private const int SIGN_SHIFT = 31;
		/// <summary>
		/// Number of bits to shift to obtain the exponent in a singlr-precision
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
		/// The bit pattern used to represent Infinity in a single-precision
		/// floating-point value.
		/// </summary>
		private const int INFINITY = 0x7f800000;

		/// <summary>
		/// Read a single-precision floating point number.
		/// </summary>
		/// <returns> the value.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>


		public float readHalf()
		{


			int bits = readUnsignedShort();


			int sign = (bits >> HALF_SIGN_SHIFT) & Coder.BIT0;
			int exp = (bits >> HALF_EXP_SHIFT) & Coder.LOWEST5;
			int mantissa = bits & Coder.LOWEST10;
			float value;

			if (exp == 0)
			{
				if (mantissa == 0)
				{ // Plus or minus zero
					value = Float.intBitsToFloat(sign << SIGN_SHIFT);
				}
				else
				{ // Denormalized number -- renormalize it
					while ((mantissa & Coder.BIT10) == 0)
					{
						mantissa <<= 1;
						exp -= 1;
					}
					exp += 1;
					exp = exp + (EXP_MAX - HALF_EXP_OFFSET);
					mantissa &= ~Coder.BIT10;
					mantissa = mantissa << MANT_SHIFT;
					value = Float.intBitsToFloat((sign << SIGN_SHIFT) | (exp << EXP_SHIFT) | mantissa);
				}
			}
			else if (exp == HALF_EXP_MAX)
			{
				if (mantissa == 0)
				{ // Inf
					value = Float.intBitsToFloat((sign << SIGN_SHIFT) | INFINITY);
				}
				else
				{ // NaN
					value = Float.intBitsToFloat((sign << SIGN_SHIFT) | INFINITY | (mantissa << MANT_SHIFT));
				}
			}
			else
			{
				exp = exp + (EXP_MAX - HALF_EXP_OFFSET);
				mantissa = mantissa << MANT_SHIFT;
				value = Float.intBitsToFloat((sign << SIGN_SHIFT) | (exp << EXP_SHIFT) | mantissa);
			}
			return value;
		}
	}

}