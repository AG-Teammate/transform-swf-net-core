/*
 * ImageUtils.java
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

using com.flagstone.transform.coder;

namespace com.flagstone.transform.util.image
{
    /// <summary>
	/// ImageFilter contains a set of convenience methods for processing the
	/// pixels in an image.
	/// </summary>


	public sealed class ImageFilter
	{

		/// <summary>
		/// Shift used to align the RGB555 red channel to a 8-bit pixel. </summary>
		private const int RGB5_MSB_MASK = 0x00F8;
		/// <summary>
		/// Shift used to align the RGB555 red channel to a 8-bit pixel. </summary>
		private const int R5_SHIFT = 7;
		/// <summary>
		/// Shift used to align the RGB555 green channel to a 8-bit pixel. </summary>
		private const int G5_SHIFT = 2;
		/// <summary>
		/// Shift used to align the RGB555 blue channel to a 8-bit pixel. </summary>
		private const int B5_SHIFT = 3;

		/// <summary>
		/// Level used to indicate an opaque colour. </summary>
		private const int OPAQUE = 255;

		/// <summary>
		/// Byte offset to red channel. </summary>
		private const int RED = 0;
		/// <summary>
		/// Byte offset to red channel. </summary>
		private const int GREEN = 1;
		/// <summary>
		/// Byte offset to blue channel. </summary>
		private const int BLUE = 2;
		/// <summary>
		/// Byte offset to alpha channel. </summary>
		private const int ALPHA = 3;

		/// <summary>
		/// Number of colour channels in a 32-bit pixel. </summary>
		private const int RGBA_CHANNELS = 4;
		/// <summary>
		/// Number of colour channels in a 24-bit pixel. </summary>
		private const int RGB_CHANNELS = 3;

		/// <summary>
		/// Filter out the alpha channel from a 32-bit image. </summary>
		/// <param name="image"> the image containing 32-bit pixels in RGBA format. </param>
		/// <returns> the image data containing only the RGB channels. </returns>


		public byte[] removeAlpha(byte[] image)
		{


			byte[] @out = new byte[(image.Length / RGBA_CHANNELS) * RGB_CHANNELS];

			int dst = 0;

			for (int i = 0; i < image.Length; i += RGBA_CHANNELS)
			{
				@out[dst++] = image[i];
				@out[dst++] = image[i + 1];
				@out[dst++] = image[i + 2];
			}
			return @out;
		}

		/// <summary>
		/// Flip a 24-bit image along the horizontal axis so the order of the rows
		/// are reversed. </summary>
		/// <param name="image"> the image containing 24-bit pixels in RGB format. </param>
		/// <param name="width"> the number pixels in each row. </param>
		/// <param name="height"> the number of rows in the image. </param>
		/// <returns> the image reversed vertically. </returns>


		public byte[] invertRGB(byte[] image, int width, int height)
		{



			byte[] @out = new byte[image.Length];

			int dst = 0;
			int src = 0;

			for (int row = height - 1; row >= 0; row--)
			{
				src = row * width;

				for (int col = 0; col < width; col++, src += RGB_CHANNELS)
				{
					@out[dst++] = image[src];
					@out[dst++] = image[src + 1];
					@out[dst++] = image[src + 2];
				}
			}

			return @out;
		}

		/// <summary>
		/// Remap the colour channels in a 24-bit image from RGB to BGR. </summary>
		/// <param name="image"> the image containing 24-bit pixels in RGB format. </param>


		public void reverseRGB(byte[] image)
		{
			byte swap;
			for (int i = 0; i < image.Length; i += RGB_CHANNELS)
			{
				swap = image[i];
				image[i] = image[i + 2];
				image[i + 2] = swap;
			}
		}

		/// <summary>
		/// Remap the colour channels in a 32-bit image from RGBA to ABGR. </summary>
		/// <param name="image"> the image containing 32-bit pixels in RGBA format. </param>


		public void reverseRGBA(byte[] image)
		{
			byte alpha;
			for (int i = 0; i < image.Length; i += RGBA_CHANNELS)
			{
				alpha = image[i + ALPHA];
				image[i + RED] = alpha;
				image[i + GREEN] = image[i + RED];
				image[i + BLUE] = image[i + GREEN];
				image[i + ALPHA] = image[i + BLUE];
			}
		}

		/// <summary>
		/// Convert an image with 32-bits for the red, green, blue and alpha channels
		/// to one where the channels each take 5-bits in a 16-bit word. </summary>
		/// <param name="imgWidth"> the width of the image in pixels. </param>
		/// <param name="imgHeight"> the height of the image in pixels. </param>
		/// <param name="img"> the image data. </param>
		/// <returns> the image data with the red, green and blue channels packed into
		/// 16-bit words. Alpha is discarded. </returns>


		public byte[] packColors(int imgWidth, int imgHeight, byte[] img)
		{
			int src = 0;
			int dst = 0;
			int row;
			int col;



			int scan = imgWidth + (imgWidth & 1);


			byte[] formattedImage = new byte[scan * imgHeight * 2];

			for (row = 0; row < imgHeight; row++)
			{
				for (col = 0; col < imgWidth; col++, src++)
				{


					int red = (img[src++] & RGB5_MSB_MASK) << R5_SHIFT;


					int green = (img[src++] & RGB5_MSB_MASK) << G5_SHIFT;


					int blue = (img[src++] & RGB5_MSB_MASK) >> B5_SHIFT;


					int colour = (red | green | blue) & Coder.LOWEST15;

					formattedImage[dst++] = (byte)(colour >> 8);
					formattedImage[dst++] = (byte) colour;
				}

				while (col < scan)
				{
					formattedImage[dst++] = 0;
					formattedImage[dst++] = 0;
					col++;
				}
			}
			return formattedImage;
		}

		/// <summary>
		/// Adjust the width of each row in an image so the data is aligned to a
		/// 16-bit word boundary when loaded in memory. The additional bytes are
		/// all set to zero and will not be displayed in the image.
		/// </summary>
		/// <param name="imgWidth"> the width of the image in pixels. </param>
		/// <param name="imgHeight"> the height of the image in pixels. </param>
		/// <param name="img"> the image data. </param>
		/// <returns> the image data with each row aligned to a 16-bit boundary. </returns>


		public byte[] adjustScan(int imgWidth, int imgHeight, byte[] img)
		{
			int src = 0;
			int dst = 0;
			int row;
			int col;

			int scan = 0;
			byte[] formattedImage = null;

			scan = (imgWidth + 3) & ~3;
			formattedImage = new byte[scan * imgHeight];

			for (row = 0; row < imgHeight; row++)
			{
				for (col = 0; col < imgWidth; col++)
				{
					formattedImage[dst++] = img[src++];
				}

				while (col++ < scan)
				{
					formattedImage[dst++] = 0;
				}
			}

			return formattedImage;
		}

		/// <summary>
		/// Reorder the image pixels from RGBA to ARGB.
		/// </summary>
		/// <param name="img"> the image data. </param>


		public void orderAlpha(byte[] img)
		{
			byte alpha;

			for (int i = 0; i < img.Length; i += RGBA_CHANNELS)
			{
				alpha = img[i + ALPHA];
				img[i + ALPHA] = img[i + BLUE];
				img[i + BLUE] = img[i + GREEN];
				img[i + GREEN] = img[i + RED];
				img[i + RED] = alpha;
			}
		}

		/// <summary>
		/// Reorder the image pixels from RGBA to ABGR.
		/// </summary>
		/// <param name="img"> the image data. </param>


		public void orderABGR(byte[] img)
		{
			byte swap;

			for (int i = 0; i < img.Length; i += RGBA_CHANNELS)
			{
				swap = img[i + ALPHA];
				img[i + ALPHA] = img[i + RED];
				img[i + RED] = swap;
				swap = img[i + BLUE];
				img[i + BLUE] = img[i + GREEN];
				img[i + GREEN] = swap;
			}
		}

		/// <summary>
		/// Apply the level for the alpha channel to the red, green and blue colour
		/// channels for encoding the image so it can be added to a Flash movie. </summary>
		/// <param name="img"> the image data. </param>


		public void applyAlpha(byte[] img)
		{
			int alpha;

			for (int i = 0; i < img.Length; i += RGBA_CHANNELS)
			{
				alpha = img[i + ALPHA] & OPAQUE;

				img[i + RED] = (byte)(((img[i + RED] & OPAQUE) * alpha) / OPAQUE);
				img[i + GREEN] = (byte)(((img[i + GREEN] & OPAQUE) * alpha) / OPAQUE);
				img[i + BLUE] = (byte)(((img[i + BLUE] & OPAQUE) * alpha) / OPAQUE);
			}
		}

	   /// <summary>
	   /// Concatenate the colour table and the image data together. </summary>
	   /// <param name="img"> the image data. </param>
	   /// <param name="colors"> the colour table. </param>
	   /// <returns> a single array containing the red, green and blue (not alpha)
	   /// entries from the colour table followed by the red, green, blue and
	   /// alpha channels from the image. The alpha defaults to 255 for an opaque
	   /// image. </returns>


		public byte[] merge(byte[] img, byte[] colors)
		{


			int entries = colors.Length / RGBA_CHANNELS;


			byte[] merged = new byte[entries * (RGBA_CHANNELS - 1) + img.Length];
			int dst = 0;

			// Remap RGBA colours from table to BGR in encoded image
			for (int i = 0; i < colors.Length; i += RGBA_CHANNELS)
			{
				merged[dst++] = colors[i + BLUE];
				merged[dst++] = colors[i + GREEN];
				merged[dst++] = colors[i + RED];
			}

			foreach (byte element in img)
			{
				merged[dst++] = element;
			}

			return merged;
		}

		/// <summary>
		/// Concatenate the colour table and the image data together. </summary>
		/// <param name="img"> the image data. </param>
		/// <param name="colors"> the colour table. </param>
		/// <returns> a single array containing entries from the colour table followed
		/// by the image. </returns>


		public byte[] mergeAlpha(byte[] img, byte[] colors)
		{


			byte[] merged = new byte[colors.Length + img.Length];
			int dst = 0;

			foreach (byte element in colors)
			{
				merged[dst++] = element;
			}

			foreach (byte element in img)
			{
				merged[dst++] = element;
			}
			return merged;
		}

	}

}