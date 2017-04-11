using System;
using System.IO;
using com.flagstone.transform.coder;
using com.flagstone.transform.image;

/*
 * PNGDecoder.java
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

namespace com.flagstone.transform.util.image
{
    /// <summary>
	/// PNGDecoder decodes Portable Network Graphics (PNG) format images so they can
	/// be used in a Flash file.
	/// </summary>


	public sealed class PNGDecoder : ImageProvider, ImageDecoder
	{

		/// <summary>
		/// Alpha channel value for opaque colours. </summary>
		private const int OPAQUE = 255;
		/// <summary>
		/// Mask for reading unsigned 8-bit values. </summary>
		private const int UNSIGNED_BYTE = 255;

		/// <summary>
		/// Size of each colour table entry or pixel in a true colour image. </summary>
		private const int RGBA_CHANNELS = 4;
		/// <summary>
		/// Size of each colour table entry or pixel in a true colour image. </summary>
		private const int RGB_CHANNELS = 3;

		/// <summary>
		/// Size of a pixel in a RGB555 true colour image. </summary>
		private const int RGB5_SIZE = 16;
		/// <summary>
		/// Size of a pixel in a RGB8 true colour image. </summary>
		private const int RGB8_SIZE = 24;

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
		/// The image pixels occupy 1 bit. </summary>
		private const int DEPTH_1 = 1;
		/// <summary>
		/// The image pixels occupy 2 bits. </summary>
		private const int DEPTH_2 = 2;
		/// <summary>
		/// The image pixels occupy 4 bits. </summary>
		private const int DEPTH_4 = 4;
		/// <summary>
		/// The image pixels occupy 8 bits. </summary>
		private const int DEPTH_8 = 8;
		/// <summary>
		/// The image pixels occupy 16 bits. </summary>
		private const int DEPTH_16 = 16;


		/// <summary>
		/// Message used to signal that the image cannot be decoded. </summary>
		private const string BAD_FORMAT = "Unsupported format";

		/// <summary>
		/// Table for mapping monochrome images onto a colour palette. </summary>
		private static readonly int[] MONOCHROME = {0, 255};
		/// <summary>
		/// Table for mapping 2-level grey-scale images onto a colour palette. </summary>
		private static readonly int[] GREYCSALE2 = {0, 85, 170, 255};
		/// <summary>
		/// Table for mapping 4-level grey-scale images onto a colour palette. </summary>
		private static readonly int[] GREYCSALE4 = {0, 17, 34, 51, 68, 85, 102, 119, 136, 153, 170, 187, 204, 221, 238, 255};

		/// <summary>
		/// signature identifying a PNG format image. </summary>
		private static readonly int[] SIGNATURE = {137, 80, 78, 71, 13, 10, 26, 10};
		/// <summary>
		/// signature identifying a header block. </summary>
		private const int IHDR = 0x49484452;
		/// <summary>
		/// signature identifying a colour palette block. </summary>
		private const int PLTE = 0x504c5445;
		/// <summary>
		/// signature identifying an image data block. </summary>
		private const int IDAT = 0x49444154;
		/// <summary>
		/// signature identifying an end block. </summary>
		private const int IEND = 0x49454e44;
		/// <summary>
		/// signature identifying a transparency block. </summary>
		private const int TRNS = 0x74524e53;
		/*
		 * private static final int BKGD = 0x624b4744;
		 * private static final int CHRM = 0x6348524d;
		 * private static final int FRAC = 0x66524163;
		 * private static final int GAMA = 0x67414d41;
		 * private static final int GIFG = 0x67494667;
		 * private static final int GIFT = 0x67494674;
		 * private static final int GIFX = 0x67494678;
		 * private static final int HIST = 0x68495354;
		 * private static final int ICCP = 0x69434350;
		 * private static final int ITXT = 0x69545874;
		 * private static final int OFFS = 0x6f464673;
		 * private static final int PCAL = 0x7043414c;
		 * private static final int PHYS = 0x70485973;
		 * private static final int SBIT = 0x73424954;
		 * private static final int SCAL = 0x7343414c;
		 * private static final int SPLT = 0x73504c54;
		 * private static final int SRGB = 0x73524742;
		 * private static final int TEXT = 0x74455874;
		 * private static final int TIME = 0x74494d45;
		 * private static final int ZTXT = 0x7a545874;
		 */
		/// <summary>
		/// colorType value for grey-scale images. </summary>
		private const int GREYSCALE = 0;
		/// <summary>
		/// colorType value for true-colour images. </summary>
		private const int TRUE_COLOUR = 2;
		/// <summary>
		/// colorType value for indexed colour images. </summary>
		private const int INDEXED_COLOUR = 3;
		/// <summary>
		/// colorType value for grey-scale images with transparency. </summary>
		private const int ALPHA_GREYSCALE = 4;
		/// <summary>
		/// colorType value for true-colour images with transparency. </summary>
		private const int ALPHA_TRUECOLOUR = 6;
		/// <summary>
		/// filterMethod value for images with sub-pixel filtering. </summary>
		private const int SUB_FILTER = 1;
		/// <summary>
		/// filterMethod value for images with upper filtering. </summary>
		private const int UP_FILTER = 2;
		/// <summary>
		/// filterMethod value for images with average filtering. </summary>
		private const int AVG_FILTER = 3;
		/// <summary>
		/// filterMethod value for images with Paeth filtering. </summary>
		private const int PAETH_FILTER = 4;
		/// <summary>
		/// starting row for each image block. </summary>
		private static readonly int[] START_ROW = {0, 0, 4, 0, 2, 0, 1};
		/// <summary>
		/// starting column for each image block. </summary>
		private static readonly int[] START_COLUMN = {0, 4, 0, 2, 0, 1, 0};
		/// <summary>
		/// row increment for each image block. </summary>
		private static readonly int[] ROW_STEP = {8, 8, 8, 4, 4, 2, 2};
		/// <summary>
		/// column increment for each image block. </summary>
		private static readonly int[] COLUMN_STEP = {8, 8, 4, 4, 2, 2, 1};

		/// <summary>
		/// The number of bits used to represent each colour component. </summary>
		
		private int bitDepth;
		/// <summary>
		/// The number of colour components in each pixel. </summary>
		
		private int colorComponents;
		/// <summary>
		/// The method used to compress the image. </summary>
	//    private int compression;
		/// <summary>
		/// The method used to encode colours in the image. </summary>
		
		private int colorType;
		/// <summary>
		/// Block filtering method used in the image. </summary>
	//    private int filterMethod;
		/// <summary>
		/// Row interlacing method used in the image. </summary>
		
		private int interlaceMethod;
		/// <summary>
		/// Default value for transparent grey-scale pixels. </summary>
		
		private int transparentGrey;
		/// <summary>
		/// Default value for transparent red pixels. </summary>
		
		private int transparentRed;
		/// <summary>
		/// Default value for transparent green pixels. </summary>
	//    private int transparentGreen;
		/// <summary>
		/// Default value for transparent blue pixels. </summary>
	//    private int transparentBlue;

		/// <summary>
		/// Binary data taken directly from encoded image. </summary>
		
		private byte[] chunkData = new byte[0];

		/// <summary>
		/// The format of the decoded image. </summary>
		
		private ImageFormat format;
		/// <summary>
		/// The width of the image in pixels. </summary>
		
		private int width;
		/// <summary>
		/// The height of the image in pixels. </summary>
		
		private int height;
		/// <summary>
		/// The colour table for indexed images. </summary>
		
		private byte[] table;
		/// <summary>
		/// The image data. </summary>
		
		private byte[] image;

		/// <summary>
		/// {@inheritDoc} </summary>
		public ImageDecoder newDecoder()
		{
			return new PNGDecoder();
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void read(FileInfo file)
		{


			ImageInfo info = new ImageInfo();
		    info.Input = new FileStream(file.FullName, FileMode.Open, FileAccess.Read,
		        FileShare.ReadWrite);
            //info.setDetermineImageNumber(true);

            if (!info.check())
			{
				throw new Exception(BAD_FORMAT);
			}

			read(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public ImageTag defineImage(int identifier)
		{
			ImageTag @object = null;


			ImageFilter filter = new ImageFilter();

			switch (format)
			{
			case ImageFormat.IDX8:
				@object = new DefineImage(identifier, width, height, table.Length / RGBA_CHANNELS, zip(filter.merge(filter.adjustScan(width, height, image), table)));
				break;
			case ImageFormat.IDXA:
				@object = new DefineImage2(identifier, width, height, table.Length / RGBA_CHANNELS, zip(filter.mergeAlpha(filter.adjustScan(width, height, image), table)));
				break;
			case ImageFormat.RGB5:
				@object = new DefineImage(identifier, width, height, zip(filter.packColors(width, height, image)), RGB5_SIZE);
				break;
			case ImageFormat.RGB8:
				filter.orderAlpha(image);
				@object = new DefineImage(identifier, width, height, zip(image), RGB8_SIZE);
				break;
			case ImageFormat.RGBA:
				applyAlpha(image);
				@object = new DefineImage2(identifier, width, height, zip(image));
				break;
			default:
				throw new Exception(BAD_FORMAT);
			}
			return @object;
		}

		/// <summary>
		/// Apply the level for the alpha channel to the red, green and blue colour
		/// channels for encoding the image so it can be added to a Flash movie. </summary>
		/// <param name="img"> the image data. </param>


		public static void applyAlpha(byte[] img)
		{
			int alpha;

			for (int i = 0; i < img.Length; i += RGBA_CHANNELS)
			{
				alpha = img[i + ALPHA] & UNSIGNED_BYTE;

				img[i + ALPHA] = (byte)(((img[i + BLUE] & UNSIGNED_BYTE) * alpha) / OPAQUE);
				img[i + BLUE] = (byte)(((img[i + GREEN] & UNSIGNED_BYTE) * alpha) / OPAQUE);
				img[i + GREEN] = (byte)(((img[i + RED] & UNSIGNED_BYTE) * alpha) / OPAQUE);
				img[i + RED] = (byte) alpha;
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void read(Stream stream)
		{



			BigDecoder coder = new BigDecoder(stream);

			int length = 0;
			int chunkType = 0;
			bool moreChunks = true;

			chunkData = new byte[0];
			transparentGrey = -1;
			transparentRed = -1;

			for (int i = 0; i < 8; i++)
			{
				if (coder.readByte() != SIGNATURE[i])
				{
					throw new Exception(BAD_FORMAT);
				}
			}

			while (moreChunks)
			{
				length = coder.readInt();
				chunkType = coder.readInt();
				coder.mark();
				switch (chunkType)
				{
				case IHDR:
					decodeIHDR(coder);
					break;
				case PLTE:
					decodePLTE(coder, length);
					break;
				case TRNS:
					decodeTRNS(coder, length);
					break;
				case IDAT:
					decodeIDAT(coder, length);
					break;
				case IEND:
					moreChunks = false;
					coder.skip(length + 4);
					break;
				default:
					coder.skip(length + 4);
					break;
				}
			}
			decodeImage();
		}

		/// <summary>
		/// Decode the header, IHDR, block from a PNG image. </summary>
		/// <param name="coder"> the decoder containing the image data. </param>
		/// <exception cref="IOException"> if there is an error decoding the data. </exception>
		/// <exception cref="Exception"> is the image contains an unsupported format. </exception>



		private void decodeIHDR(BigDecoder coder)
		{
			width = coder.readInt();
			height = coder.readInt();
			bitDepth = coder.readByte();
			colorType = coder.readByte();
			/* compression = */	 coder.readByte();
			/* filterMethod = */	 coder.readByte();
			interlaceMethod = coder.readByte();
			/* crc = */	 coder.readInt();
			decodeFormat();
		}

		/// <summary>
		/// Decode the image format. </summary>
		/// <exception cref="Exception"> if the image is in an unsupported format. </exception>


		private void decodeFormat()
		{
			switch (colorType)
			{
			case GREYSCALE:
				format = ImageFormat.RGB8;
				colorComponents = 1;
				break;
			case TRUE_COLOUR:
				format = ImageFormat.RGB8;
				colorComponents = RGB_CHANNELS;
				break;
			case INDEXED_COLOUR:
				format = ImageFormat.IDX8;
				colorComponents = 1;
				break;
			case ALPHA_GREYSCALE:
				format = ImageFormat.RGBA;
				colorComponents = 2;
				break;
			case ALPHA_TRUECOLOUR:
				format = ImageFormat.RGBA;
				colorComponents = RGBA_CHANNELS;
				break;
			default:
				throw new Exception(BAD_FORMAT);
			}

			if (format == ImageFormat.RGB8 && bitDepth <= 5)
			{
				format = ImageFormat.RGB5;
			}
		}

		/// <summary>
		/// Decode the colour palette, PLTE, block from a PNG image. </summary>
		/// <param name="coder"> the decoder containing the image data. </param>
		/// <param name="length"> the length of the block in bytes. </param>
		/// <exception cref="IOException"> if there is an error decoding the data. </exception>



		private void decodePLTE(BigDecoder coder, int length)
		{
			if (colorType == RGB_CHANNELS)
			{


				int paletteSize = length / RGB_CHANNELS;
				int index = 0;

				table = new byte[paletteSize * RGBA_CHANNELS];

				for (int i = 0; i < paletteSize; i++, index += RGBA_CHANNELS)
				{
					table[index + ALPHA] = OPAQUE;
					table[index + BLUE] = (byte) coder.readByte();
					table[index + GREEN] = (byte) coder.readByte();
					table[index + RED] = (byte) coder.readByte();
				}
			}
			else
			{
				coder.skip(length);
			}
			coder.readInt(); // crc
		}

		/// <summary>
		/// Decode the transparency, TRNS, block from a PNG image. </summary>
		/// <param name="coder"> the decoder containing the image data. </param>
		/// <param name="length"> the length of the block in bytes. </param>
		/// <exception cref="IOException"> if there is an error decoding the data. </exception>



		private void decodeTRNS(BigDecoder coder, int length)
		{
			int index = 0;

			switch (colorType)
			{
			case GREYSCALE:
				transparentGrey = coder.readUnsignedShort();
				format = ImageFormat.RGBA;
				break;
			case TRUE_COLOUR:
				transparentRed = coder.readUnsignedShort();
				format = ImageFormat.RGBA;
				/* transparentGreen = */	 coder.readUnsignedShort();
				/* transparentBlue = */	 coder.readUnsignedShort();
				break;
			case INDEXED_COLOUR:
				format = ImageFormat.IDXA;
				for (int i = 0; i < length; i++, index += RGBA_CHANNELS)
				{
					table[index + ALPHA] = (byte) coder.readByte();

					if (table[index + ALPHA] == 0)
					{
						table[index + RED] = 0;
						table[index + GREEN] = 0;
						table[index + BLUE] = 0;
					}
				}
				break;
			default:
				break;
			}
			coder.readInt(); // crc
		}

		/// <summary>
		/// Decode the image data, IDAT, block from a PNG image. </summary>
		/// <param name="coder"> the decoder containing the image data. </param>
		/// <param name="length"> the length of the block in bytes. </param>
		/// <exception cref="IOException"> if there is an error decoding the data. </exception>



		private void decodeIDAT(BigDecoder coder, int length)
		{


			int currentLength = chunkData.Length;


			int newLength = currentLength + length;



			byte[] data = new byte[newLength];

			Array.Copy(chunkData, 0, data, 0, currentLength);

			for (int i = currentLength; i < newLength; i++)
			{
				data[i] = (byte) coder.readByte();
			}

			chunkData = data;

			coder.readInt(); // crc
		}

		/// <summary>
		/// Decode a PNG encoded image. </summary>
		/// <exception cref="IOException"> if there is an error decoding the data. </exception>
		/// <exception cref="Exception"> if the image cannot be decoded. </exception>


		private void decodeImage()
		{

			if ((format == ImageFormat.IDX8) || (format == ImageFormat.IDXA))
			{
				image = new byte[height * width];
			}
			else
			{
				image = new byte[height * width * RGBA_CHANNELS];
			}

			if (interlaceMethod == 1)
			{
				decodeInterlaced();
			}
			else
			{
				decodeProgressive();
			}
		}

		/// <summary>
		/// Decode an interlaced image. </summary>
		/// <exception cref="IOException"> if there is an error reading the image data. </exception>
		/// <exception cref="Exception"> if the image is in an unsupported format. </exception>


		private void decodeInterlaced()
		{



			byte[] encodedImage = unzip(chunkData);


			int bitsPerPixel = bitDepth * colorComponents;


			int bitsPerRow = width * bitsPerPixel;


			int rowWidth = (bitsPerRow + 7) >> 3;


			int bytesPerPixel = (bitsPerPixel < 8) ? 1 : bitsPerPixel / 8;



			byte[] current = new byte[rowWidth];


			byte[] previous = new byte[rowWidth];

			for (int i = 0; i < rowWidth; i++)
			{
				previous[i] = 0;
			}

			int imageIndex = 0;

			int row = 0;
			int col = 0;
			int filter = 0;

			int scanBits = 0;
			int scanLength = 0;

			for (int pass = 0; pass < 7; pass++)
			{
				for (row = START_ROW[pass]; (row < height) && (imageIndex < encodedImage.Length); row += ROW_STEP[pass])
				{
					for (col = START_COLUMN[pass], scanBits = 0; col < width; col += COLUMN_STEP[pass])
					{
						scanBits += bitsPerPixel;
					}

					scanLength = (scanBits + 7) >> 3;
					filter = encodedImage[imageIndex++];

					for (int i = 0; i < scanLength; i++, imageIndex++)
					{
						current[i] = (imageIndex < encodedImage.Length) ? encodedImage[imageIndex] : previous[i];
					}

					defilter(filter, bytesPerPixel, scanLength, current, previous);
					deblock(row, current, START_COLUMN[pass], COLUMN_STEP[pass]);
					Array.Copy(current, 0, previous, 0, scanLength);
				}
			}
		}

		/// <summary>
		/// Decode a progressive-scan image. </summary>
		/// <exception cref="IOException"> if there is an error reading the image data. </exception>
		/// <exception cref="Exception"> if the image is in an unsupported format. </exception>


		private void decodeProgressive()
		{



			byte[] data = unzip(chunkData);


			int bitsPerPixel = bitDepth * colorComponents;


			int bitsPerRow = width * bitsPerPixel;


			int rowWidth = (bitsPerRow + 7) >> 3;


			int bytesPerPixel = (bitsPerPixel < 8) ? 1 : bitsPerPixel / 8;



			byte[] current = new byte[rowWidth];


			byte[] previous = new byte[rowWidth];

			for (int i = 0; i < rowWidth; i++)
			{
				previous[i] = 0;
			}

			int index = 0;
			int row = 0;
			int col = 0;
			int filter = 0;
			int scanBits = 0;
			int scanLength = 0;

			for (row = 0; (row < height) && (index < data.Length); row++)
			{
				for (col = 0, scanBits = 0; col < width; col++)
				{
					 scanBits += bitsPerPixel;
				}

				scanLength = (scanBits + 7) >> 3;
				filter = data[index++];

				for (int i = 0; i < scanLength; i++, index++)
				{
					current[i] = (index < data.Length) ? data[index] : previous[i];
				}

				defilter(filter, bytesPerPixel, scanLength, current, previous);
				deblock(row, current, 0, 1);
				Array.Copy(current, 0, previous, 0, scanLength);
			}
		}

		/// <summary>
		/// Reverse the filter applied to the pixel data. </summary>
		/// <param name="filter"> the filter type. </param>
		/// <param name="size"> the offset in the row to the start of the encoded data. </param>
		/// <param name="scan"> the number of bytes in the block </param>
		/// <param name="current"> the pixel data in the encoded image row. </param>
		/// <param name="previous"> the pixel data from the previous row. </param>


		private void defilter(int filter, int size, int scan, byte[] current, byte[] previous)
		{
			switch (filter)
			{
			case SUB_FILTER:
				subFilter(size, scan, current);
				break;
			case UP_FILTER:
				upFilter(scan, current, previous);
				break;
			case AVG_FILTER:
				averageFilter(size, scan, current, previous);
				break;
			case PAETH_FILTER:
				paethFilter(size, scan, current, previous);
				break;
			default:
				break;
			}
		}

		/// <summary>
		/// Reverse the sub-filter applied to the pixel data. </summary>
		/// <param name="start"> the offset in the row to the start of the encoded data. </param>
		/// <param name="count"> the number of bytes in the block </param>
		/// <param name="current"> the pixel data in the encoded image row. </param>


		private void subFilter(int start, int count, byte[] current)
		{
			for (int i = start, j = 0; i < count; i++, j++)
			{
				current[i] = (byte)(current[i] + current[j]);
			}
		}

		/// <summary>
		/// Reverse the up-filter applied to the pixel data. </summary>
		/// <param name="count"> the number of bytes in the block </param>
		/// <param name="current"> the pixel data in the encoded image row. </param>
		/// <param name="previous"> the pixel data from the previous row. </param>


		private void upFilter(int count, byte[] current, byte[] previous)
		{
			for (int i = 0; i < count; i++)
			{
				current[i] = (byte)(current[i] + previous[i]);
			}
		}

		/// <summary>
		/// Reverse the average filter applied to the pixel data. </summary>
		/// <param name="start"> the offset in the row to the start of the encoded data. </param>
		/// <param name="count"> the number of bytes in the block </param>
		/// <param name="current"> the pixel data in the encoded image row. </param>
		/// <param name="previous"> the pixel data from the previous row. </param>


		private void averageFilter(int start, int count, byte[] current, byte[] previous)
		{

			for (int cindex = 0; cindex < start; cindex++)
			{
				current[cindex] = (byte)(current[cindex] + (0 + (UNSIGNED_BYTE & previous[cindex])) / 2);
			}

			for (int cindex = start, pindex = 0; cindex < count; cindex++, pindex++)
			{
				current[cindex] = (byte)(current[cindex] + ((UNSIGNED_BYTE & current[pindex]) + (UNSIGNED_BYTE & previous[cindex])) / 2);
			}
		}

		/// <summary>
		/// Reverse the Paeth filter applied to the pixel data. </summary>
		/// <param name="start"> the offset in the row to the start of the encoded data. </param>
		/// <param name="count"> the number of bytes in the block </param>
		/// <param name="current"> the pixel data in the encoded image row. </param>
		/// <param name="previous"> the pixel data from the previous row. </param>


		private void paethFilter(int start, int count, byte[] current, byte[] previous)
		{

			for (int cindex = 0; cindex < start; cindex++)
			{
				current[cindex] = (byte)(current[cindex] + paeth(0, previous[cindex], 0));
			}

			for (int cindex = start, pindex = 0; cindex < count; cindex++, pindex++)
			{
				current[cindex] = (byte)(current[cindex] + paeth(current[pindex], previous[cindex], previous[pindex]));
			}
		}

		/// <summary>
		/// Decode a Paeth encoded pixel. </summary>
		/// <param name="lower"> the current pixel. </param>
		/// <param name="upper"> the pixel on the previous row. </param>
		/// <param name="next"> the next pixel in current row. </param>
		/// <returns> the decoded value. </returns>


		private int paeth(byte lower, byte upper, byte next)
		{


			int left = UNSIGNED_BYTE & lower;


			int above = UNSIGNED_BYTE & upper;


			int upperLeft = UNSIGNED_BYTE & next;


			int estimate = left + above - upperLeft;
			int distLeft = estimate - left;

			if (distLeft < 0)
			{
				distLeft = -distLeft;
			}

			int distAbove = estimate - above;

			if (distAbove < 0)
			{
				distAbove = -distAbove;
			}

			int distUpperLeft = estimate - upperLeft;

			if (distUpperLeft < 0)
			{
				distUpperLeft = -distUpperLeft;
			}

			int value;

			if ((distLeft <= distAbove) && (distLeft <= distUpperLeft))
			{
				value = left;
			}
			else if (distAbove <= distUpperLeft)
			{
				value = above;
			}
			else
			{
				value = upperLeft;
			}

			return value;
		}

		/// <summary>
		/// Decode a block of image data. </summary>
		/// <param name="row"> the current row in the decoded image. </param>
		/// <param name="current"> the encoded block data. </param>
		/// <param name="start"> the offset in the image row. </param>
		/// <param name="inc"> the size of each pixel. </param>
		/// <exception cref="IOException"> if there is an error reading the data. </exception>
		/// <exception cref="Exception"> if the image is encoded in an unsupported
		/// format. </exception>



		private void deblock(int row, byte[] current, int start, int inc)
		{



			MemoryStream stream = new MemoryStream(current);


			BigDecoder coder = new BigDecoder(stream);

			for (int col = start; col < width; col += inc)
			{
				switch (colorType)
				{
				case GREYSCALE:
					decodeGreyscale(coder, row, col);
					break;
				case TRUE_COLOUR:
					decodeTrueColour(coder, row, col);
					break;
				case INDEXED_COLOUR:
					decodeIndexedColour(coder, row, col);
					break;
				case ALPHA_GREYSCALE:
					decodeAlphaGreyscale(coder, row, col);
					break;
				case ALPHA_TRUECOLOUR:
					decodeAlphaTrueColour(coder, row, col);
					break;
				default:
					throw new Exception(BAD_FORMAT);
				}
			}
		}

		/// <summary>
		/// Decode a grey-scale pixel with no transparency. </summary>
		/// <param name="coder"> the decode containing the image data. </param>
		/// <param name="row"> the row number of the pixel in the image. </param>
		/// <param name="col"> the column number of the pixel in the image. </param>
		/// <exception cref="IOException"> if there is an error decoding the data. </exception>
		/// <exception cref="Exception"> if the pixel data cannot be decoded. </exception>



		private void decodeGreyscale(BigDecoder coder, int row, int col)
		{
			int pixel = 0;
			byte colour = 0;

			switch (bitDepth)
			{
			case DEPTH_1:
				pixel = coder.readBits(bitDepth, false);
				colour = (byte) MONOCHROME[pixel];
				break;
			case DEPTH_2:
				pixel = coder.readBits(bitDepth, false);
				colour = (byte) GREYCSALE2[pixel];
				break;
			case DEPTH_4:
				pixel = coder.readBits(bitDepth, false);
				colour = (byte) GREYCSALE4[pixel];
				break;
			case DEPTH_8:
				pixel = coder.readByte();
				colour = (byte) pixel;
				break;
			case DEPTH_16:
				pixel = coder.readUnsignedShort();
				colour = (byte)(pixel >> Coder.TO_LOWER_BYTE);
				break;
			default:
				throw new Exception(BAD_FORMAT);
			}

			int index = row * (width << 2) + (col << 2);

			image[index++] = colour;
			image[index++] = colour;
			image[index++] = colour;
			image[index++] = (byte) transparentGrey;
		}

		/// <summary>
		/// Decode a true colour pixel with no transparency. </summary>
		/// <param name="coder"> the decode containing the image data. </param>
		/// <param name="row"> the row number of the pixel in the image. </param>
		/// <param name="col"> the column number of the pixel in the image. </param>
		/// <exception cref="IOException"> if there is an error decoding the data. </exception>
		/// <exception cref="Exception"> if the pixel data cannot be decoded. </exception>



		private void decodeTrueColour(BigDecoder coder, int row, int col)
		{
			int pixel = 0;
			byte colour = 0;



			int index = row * (width << 2) + (col << 2);

			for (int i = 0; i < colorComponents; i++)
			{
				if (bitDepth == DEPTH_8)
				{
					pixel = coder.readByte();
					colour = (byte) pixel;
				}
				else if (bitDepth == DEPTH_16)
				{
					pixel = coder.readUnsignedShort();
					colour = (byte)(pixel >> Coder.TO_LOWER_BYTE);
				}
				else
				{
					throw new Exception(BAD_FORMAT);
				}

				image[index + i] = colour;
			}
			image[index + ALPHA] = (byte) transparentRed;
		}

		/// <summary>
		/// Decode an index colour pixel. </summary>
		/// <param name="coder"> the decode containing the image data. </param>
		/// <param name="row"> the row number of the pixel in the image. </param>
		/// <param name="col"> the column number of the pixel in the image. </param>
		/// <exception cref="IOException"> if there is an error decoding the data. </exception>
		/// <exception cref="Exception"> if the pixel data cannot be decoded. </exception>



		private void decodeIndexedColour(BigDecoder coder, int row, int col)
		{
			int index = 0;

			switch (bitDepth)
			{
			case DEPTH_1:
				index = coder.readBits(bitDepth, false);
				break;
			case DEPTH_2:
				index = coder.readBits(bitDepth, false);
				break;
			case DEPTH_4:
				index = coder.readBits(bitDepth, false);
				break;
			case DEPTH_8:
				index = coder.readByte();
				break;
			case DEPTH_16:
				index = coder.readUnsignedShort();
				break;
			default:
				throw new Exception(BAD_FORMAT);
			}
			image[row * width + col] = (byte) index;
		}

		/// <summary>
		/// Decode a grey-scale pixel with transparency. </summary>
		/// <param name="coder"> the decode containing the image data. </param>
		/// <param name="row"> the row number of the pixel in the image. </param>
		/// <param name="col"> the column number of the pixel in the image. </param>
		/// <exception cref="IOException"> if there is an error decoding the data. </exception>
		/// <exception cref="Exception"> if the pixel data cannot be decoded. </exception>



		private void decodeAlphaGreyscale(BigDecoder coder, int row, int col)
		{
			int pixel = 0;
			byte colour = 0;
			int alpha = 0;

			switch (bitDepth)
			{
			case DEPTH_1:
				pixel = coder.readBits(bitDepth, false);
				colour = (byte) MONOCHROME[pixel];
				alpha = coder.readBits(bitDepth, false);
				break;
			case DEPTH_2:
				pixel = coder.readBits(bitDepth, false);
				colour = (byte) GREYCSALE2[pixel];
				alpha = coder.readBits(bitDepth, false);
				break;
			case DEPTH_4:
				pixel = coder.readBits(bitDepth, false);
				colour = (byte) GREYCSALE4[pixel];
				alpha = coder.readBits(bitDepth, false);
				break;
			case DEPTH_8:
				pixel = coder.readByte();
				colour = (byte) pixel;
				alpha = coder.readByte();
				break;
			case DEPTH_16:
				pixel = coder.readUnsignedShort();
				colour = (byte)(pixel >> Coder.TO_LOWER_BYTE);
				alpha = coder.readUnsignedShort() >> Coder.TO_LOWER_BYTE;
				break;
			default:
				throw new Exception(BAD_FORMAT);
			}

			int index = row * (width << 2) + (col << 2);

			image[index++] = colour;
			image[index++] = colour;
			image[index++] = colour;
			image[index] = (byte) alpha;
		}

		/// <summary>
		/// Decode a true colour pixel with transparency. </summary>
		/// <param name="coder"> the decode containing the image data. </param>
		/// <param name="row"> the row number of the pixel in the image. </param>
		/// <param name="col"> the column number of the pixel in the image. </param>
		/// <exception cref="IOException"> if there is an error decoding the data. </exception>
		/// <exception cref="Exception"> if the pixel data cannot be decoded. </exception>



		private void decodeAlphaTrueColour(BigDecoder coder, int row, int col)
		{
			int pixel = 0;
			byte colour = 0;



			int index = row * (width << 2) + (col << 2);

			for (int i = 0; i < colorComponents; i++)
			{
				if (bitDepth == DEPTH_8)
				{
					pixel = coder.readByte();
					colour = (byte) pixel;
				}
				else if (bitDepth == DEPTH_16)
				{
					pixel = coder.readUnsignedShort();
					colour = (byte)(pixel >> Coder.TO_LOWER_BYTE);
				}
				else
				{
					throw new Exception(BAD_FORMAT);
				}

				image[index + i] = colour;
			}
		}

		/// <summary>
		/// Uncompress the image using the ZIP format. </summary>
		/// <param name="bytes"> the compressed image data. </param>
		/// <returns> the uncompressed image. </returns>
		/// <exception cref="Exception"> if the compressed image is not in the ZIP
		/// format or cannot be uncompressed. </exception>



		private byte[] unzip(byte[] bytes)
		{


			byte[] data = new byte[width * height * 8];
			int count = 0;



			Inflater inflater = new Inflater();
			inflater.Input = bytes;
			count = inflater.inflate(data);



			byte[] uncompressedData = new byte[count];

			Array.Copy(data, 0, uncompressedData, 0, count);

			return uncompressedData;
		}

		/// <summary>
		/// Compress the image using the ZIP format. </summary>
		/// <param name="img"> the image data. </param>
		/// <returns> the compressed image. </returns>


		public static byte[] zip(byte[] img)
		{


			Deflater deflater = new Deflater();
			deflater.Input = img;
			deflater.finish();



			byte[] compressedData = new byte[img.Length * 2];


			int bytesCompressed = deflater.deflate(compressedData);


			byte[] newData = Arrays.copyOf(compressedData, bytesCompressed);

			return newData;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public int Width => width;


	    /// <summary>
		/// {@inheritDoc} </summary>
		public int Height => height;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public byte[] Image
		{
			get
			{
				byte[] copy;
    
				switch (format)
				{
				case ImageFormat.IDX8:
				case ImageFormat.IDXA:
					copy = new byte[image.Length * RGBA_CHANNELS];
    
					int tableIndex;
    
					for (int i = 0, index = 0; i < image.Length; i++)
					{
						tableIndex = image[i] * RGBA_CHANNELS;
						copy[index++] = table[tableIndex + RED];
						copy[index++] = table[tableIndex + GREEN];
						copy[index++] = table[tableIndex + BLUE];
						copy[index++] = table[tableIndex + ALPHA];
					}
					break;
				case ImageFormat.RGB5:
				case ImageFormat.RGB8:
				case ImageFormat.RGBA:
					copy = Arrays.copyOf(image, image.Length);
					break;
				default:
					throw new Exception(BAD_FORMAT);
				}
				return copy;
			}
		}
	}


}