using System;
using System.Collections.Generic;
using System.IO;

/*
 * BigDecoder.java
 * Transform
 *
 * Copyright (c) 2010 Flagstone Software Ltd. All rights reserved.
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
	/// BigDecoder wraps an InputStream with a buffer to reduce the amount of
	/// memory required to decode an image or sound and to improve efficiency by
	/// reading data from a file or external source in blocks. Word values are
	/// read in Big-Endian format with the most significant byte decoded first.
	/// </summary>


	public sealed class BigDecoder
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
		/// Left shift to convert number of bits to number of bytes. </summary>
		private const int BITS_TO_BYTES = 3;
		/// <summary>
		/// Right shift to convert number of bits to number of bytes. </summary>
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
		/// Create a new BigDecoder for the underlying InputStream with the
		/// specified buffer size.
		/// </summary>
		/// <param name="streamIn"> the stream from which data will be read. </param>
		/// <param name="length"> the size in bytes of the buffer. </param>


		public BigDecoder(Stream streamIn, int length)
		{
			stream = streamIn;
			buffer = new byte[length];
			locations = new Stack<int?>();
		}

		/// <summary>
		/// Create a new BigDecoder for the underlying InputStream using the
		/// default buffer size.
		/// </summary>
		/// <param name="streamIn"> the stream from which data will be read. </param>


		public BigDecoder(Stream streamIn)
		{
			stream = streamIn;
			buffer = new byte[BUFFER_SIZE];
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
		/// Mark the current position. </summary>
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
		/// Reposition the decoder to the point recorded by the last call to the
		/// mark() method.
		/// </summary>
		/// <exception cref="IOException"> if the internal buffer was filled after mark() was
		/// called. </exception>


		public void reset()
		{
			int location;

			if (locations.Count == 0)
			{
				location = 0;
			}
			else
			{
				location = (int)locations.Peek();
			}
			if (location - pos < 0)
			{
				throw new Exception();
			}
			index = location - pos;
		}

		/// <summary>
		/// Change the position of the decoder within the internal buffer.
		/// </summary>
		/// <param name="location">
		///            the offset in bytes from the start of the internal buffer.
		/// </param>
		/// <exception cref="IOException"> if the internal buffer was filled after mark() was
		/// called. </exception>



		public void move(int location)
		{
			if (size - index == 0)
			{
				fill();
			}
			if (location < 0 || location > size)
			{
				throw new IndexOutOfRangeException();
			}
			index = location;
			offset = 0;
		}

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
		/// Is there any more data to read.
		/// </summary>
		/// <returns> true there is no more data to read from the stream.
		/// </returns>
		/// <exception cref="IOException"> if an error from the underlying input stream. </exception>


		public bool eof()
		{
			if (size - index == 0)
			{
				fill();
			}
			return size - index == 0;
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

				if (read < wanted && index == size)
				{
					fill();
				}
			}
			return bytes;
		}

		/// <summary>
		/// Reads an array of bytes.
		/// </summary>
		/// <param name="bytes"> the array that will contain the bytes read. </param>
		/// <param name="start"> the offset from the start of the array of bytes where
		/// the data will be written. </param>
		/// <param name="length"> the number of bytes to write to the array. </param>
		/// <returns> the array of bytes.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>



		public byte[] readBytes(byte[] bytes, int start, int length)
		{


			int wanted = length;
			int dest = start;
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
			int value = (buffer[index++] & BYTE_MASK) << TO_BYTE1;
			value |= buffer[index++] & BYTE_MASK;
			return value;
		}

		/// <summary>
		/// Read a signed 16-bit integer.
		/// </summary>
		/// <returns> the value read.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>


		public int readShort()
		{
			if (size - index < 2)
			{
				fill();
			}
			if (index + 2 > size)
			{
				throw new IndexOutOfRangeException();
			}
			int value = buffer[index++] << TO_BYTE1;
			value |= buffer[index++] & BYTE_MASK;
			return value;
		}

		/// <summary>
		/// Read an unsigned 32-bit integer.
		/// 
		/// </summary>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>
		/// <returns> the value read. </returns>


		public int scanInt()
		{
			if (size - index < 4)
			{
				fill();
			}
			if (index + 4 > size)
			{
				throw new IndexOutOfRangeException();
			}
			int addr = index;
			int value = (buffer[addr++] & BYTE_MASK) << TO_BYTE3;
			value |= (buffer[addr++] & BYTE_MASK) << TO_BYTE2;
			value |= (buffer[addr++] & BYTE_MASK) << TO_BYTE1;
			value |= buffer[addr] & BYTE_MASK;
			return value;
		}

		/// <summary>
		/// Read an unsigned 32-bit integer.
		/// 
		/// </summary>
		/// <exception cref="IOException"> if an error occurs reading from the underlying
		/// input stream. </exception>
		/// <returns> the value read. </returns>


		public int readInt()
		{
			if (size - index < 2)
			{
				fill();
			}
			if (index + 4 > size)
			{
				throw new IndexOutOfRangeException();
			}
			int value = (buffer[index++] & BYTE_MASK) << TO_BYTE3;
			value |= (buffer[index++] & BYTE_MASK) << TO_BYTE2;
			value |= (buffer[index++] & BYTE_MASK) << TO_BYTE1;
			value |= buffer[index++] & BYTE_MASK;
			return value;
		}
	}

}