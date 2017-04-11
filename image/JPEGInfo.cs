

/*
 * JPEGInfo.java
 * Transform
 *
 * Copyright (c) 2009-2010 Flagstone Software Ltd. All rights reserved.
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

using com.flagstone.transform.coder;

namespace com.flagstone.transform.image
{
    /// <summary>
	/// JPEGInfo is used to extract the width and height from a JPEG encoded image.
	/// </summary>
	public sealed class JPEGInfo
	{

		/// <summary>
		/// Bit mask for the least significant byte. </summary>
		private const int BYTE_MASK = 255;

		/// <summary>
		/// Marks the start of an image. </summary>
		public const int SOI = 0xFFD8;
		/// <summary>
		/// Marks the end of an image. </summary>
		public const int EOI = 0xFFD9;
		/// <summary>
		/// Marks the start of a frame - baseline DCT. </summary>
		public const int SOF0 = 0xFFC0;
		/// <summary>
		/// Marks the start of a frame - progressive DCT. </summary>
		public const int SOF2 = 0xFFC2;
		/// <summary>
		/// Marks the start of a JPG block. </summary>
		public const int JPG = 0xFFC8;
		/// <summary>
		/// Marks the start of a JPG block. </summary>
		public const int SOFF = 0xFFCF;
		/// <summary>
		/// Marks the Huffman table. </summary>
		public const int DHT = 0xFFC4;
		/// <summary>
		/// Marks the quantization table. </summary>
		public const int DQT = 0xFFDB;
		/// <summary>
		/// Marks the restart interval. </summary>
		public const int DRI = 0xFFDD;
		/// <summary>
		/// Marks the start of scan. </summary>
		public const int SOS = 0xFFDA;
		/// <summary>
		/// Marks a restart. </summary>
		public const int RST = 0xFFD0;
		/// <summary>
		/// Marks the start of an application specific block. </summary>
		public const int APP = 0xFFE0;
		/// <summary>
		/// Marks the start of an comment block. </summary>
		public const int COM = 0xFFFE;

		/// <summary>
		/// The width of the image. </summary>
		
		private int width;
		/// <summary>
		/// The height of the image. </summary>
		
		private int height;

		/// <summary>
		/// Get the width of the image in pixels, not twips.
		/// </summary>
		/// <returns> the width of the image </returns>
		public int Width => width;

	    /// <summary>
		/// Return the height of the image in pixels, not twips.
		/// </summary>
		/// <returns> the height of the image </returns>
		public int Height => height;

	    /// <summary>
		/// Decode a JPEG encoded image.
		/// </summary>
		/// <param name="image"> the image data. </param>


		public void decode(byte[] image)
		{


			int limit = image.Length - 2;
			int marker;
			int length;
			int index = 0;

			while (index < limit)
			{
				marker = ((image[index++] & BYTE_MASK) << Coder.TO_UPPER_BYTE) | (image[index++] & BYTE_MASK);

				if (marker == SOI || marker == EOI)
				{
					continue;
				}

				length = ((image[index++] & BYTE_MASK) << Coder.TO_UPPER_BYTE) | (image[index++] & BYTE_MASK);

				if (marker >= SOF0 && marker <= SOFF && marker != DHT && marker != JPG)
				{
					index++;
					height = ((image[index++] & BYTE_MASK) << Coder.TO_UPPER_BYTE) | (image[index++] & BYTE_MASK);
					width = ((image[index++] & BYTE_MASK) << Coder.TO_UPPER_BYTE) | (image[index++] & BYTE_MASK);
					break;
				}
			    index += length - 2;
			}
		}
	}

}