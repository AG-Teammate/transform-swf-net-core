using System;
using System.Collections.Generic;
using com.flagstone.transform.video;

/*
 * ImageBlock.java
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

namespace com.flagstone.transform.util.image
{
    /// <summary>
	/// ImageBlocker is used to sub-divide an image into a set of blocks so they can
	/// be streamed using Screen Video. Image blocks are compared so only pixel
	/// information for the portions of the image that change are sent.
	/// </summary>
	/// <seealso cref= ImageBlock </seealso>
	public sealed class ImageBlocker
	{

		/// <summary>
		/// Number of colour channels in an RGB pixel. </summary>
		private const int RGB_CHANNELS = 3;

		/// <summary>
		/// Return an image stored in a a file as a list of ImageBlock objects that
		/// can be used when creating ScreenVideo streams.
		/// 
		/// The image is divided by tiling blocks of the specified width and height
		/// across the image. For blocks at the right and bottom edges the size of
		/// the block may be reduced so that it fits the image exactly. In other
		/// words the blocks are not padded with extra pixel information.
		/// </summary>
		/// <param name="blocks">
		///            a list of ImageBlock objects </param>
		/// <param name="blockWidth">
		///            the width of a block in pixels </param>
		/// <param name="blockHeight">
		///            the height of a block in pixels </param>
		/// <param name="imageWidth">
		///            the width of the image in pixels </param>
		/// <param name="imageHeight">
		///            the height of the image in pixels </param>
		/// <param name="image">
		///            the image data </param>


		public void getImageAsBlocks(IList<ImageBlock> blocks, int blockWidth, int blockHeight, int imageWidth, int imageHeight, byte[] image)
		{



			ImageFilter filter = new ImageFilter();
			byte[] img = filter.removeAlpha(image);
			img = filter.invertRGB(img, imageWidth, imageHeight);
			filter.reverseRGB(img);



			int columns = (imageWidth + blockWidth - 1) / blockWidth;


			int rows = (imageHeight + blockHeight - 1) / blockHeight;



			byte[] blockData = new byte[blockHeight * blockWidth * RGB_CHANNELS];

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{


					int xOffset = j * blockWidth;


					int yOffset = i * blockHeight;



					int xSpan = (imageWidth - xOffset > blockWidth) ? blockWidth : imageWidth - xOffset;


					int ySpan = (imageHeight - yOffset > blockHeight) ? blockHeight : imageHeight - yOffset;
					int offset = 0;

					int idx;

					for (int k = 0; k < ySpan; k++)
					{
						for (int l = 0; l < xSpan; l++, offset += RGB_CHANNELS)
						{
							idx = (yOffset + k) * (imageWidth * RGB_CHANNELS) + (xOffset + l) * RGB_CHANNELS;

							blockData[offset] = img[idx];
							blockData[offset + 1] = img[idx + 1];
							blockData[offset + 2] = img[idx + 2];
						}
					}

					blocks.Add(new ImageBlock(xSpan, ySpan, zip(blockData, offset)));
				}
			}
		}

		/// <summary>
		/// Compress the image using the ZIP format. </summary>
		/// <param name="image"> the image data. </param>
		/// <param name="length"> the number of bytes from the image to compress. </param>
		/// <returns> the compressed image. </returns>


		private byte[] zip(byte[] image, int length)
		{


			Deflater deflater = new Deflater();
			deflater.setInput(image, 0, length);
			deflater.finish();



			byte[] compressedData = new byte[image.Length];


			int bytesCompressed = deflater.deflate(compressedData);


			byte[] newData = Arrays.copyOf(compressedData, bytesCompressed);

			return newData;
		}
	}

}