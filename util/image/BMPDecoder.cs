using System;
using System.IO;
using com.flagstone.transform.coder;
using com.flagstone.transform.image;

/*
 * BMPDecoder.java
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
	/// BMPDecoder decodes Bitmap images (BMP) so they can be used in a Flash file.
	/// </summary>


	public sealed class BMPDecoder : ImageProvider, ImageDecoder
	{

		/// <summary>
		/// Level used to indicate an opaque colour. </summary>
		private const int OPAQUE = 255;
		/// <summary>
		/// Message used to signal that the image cannot be decoded. </summary>
		private const string BAD_FORMAT = "Unsupported Format";
		/// <summary>
		/// The signature identifying BMP files. </summary>
		private static readonly int[] SIGNATURE = {66, 77};
		/// <summary>
		/// An uncompressed indexed image. </summary>
		private const int BI_RGB = 0;
		/// <summary>
		/// A run-length compressed image with 8 bits per pixel. </summary>
		private const int BI_RLE8 = 1;
		/// <summary>
		/// A run-length compressed image with 4 bits per pixel. </summary>
		private const int BI_RLE4 = 2;
		/// <summary>
		/// A true-colour image. </summary>
		private const int BI_BITFIELDS = 3;

		/// <summary>
		/// The size of the header for an uncompressed image. </summary>
		private const int UNZIPPED_LENGTH = 12;
		/// <summary>
		/// The size of the header for an compressed image. </summary>
		private const int ZIPPED_LENGTH = 40;

		/// <summary>
		/// Size of each colour table entry or pixel in a true colour image. </summary>
		private const int COLOUR_CHANNELS = 4;
		/// <summary>
		/// Size of a pixel in an indexed image with 1 bit per pixel. </summary>
		private const int IDX_1 = 1;
		/// <summary>
		/// Size of a pixel in an indexed image with 2 bits per pixel. </summary>
		private const int IDX_2 = 2;
		/// <summary>
		/// Size of a pixel in an indexed image with 4 bits per pixel. </summary>
		private const int IDX_4 = 4;
		/// <summary>
		/// Size of a pixel in an indexed image with 8 bits per pixel. </summary>
		private const int IDX_8 = 8;
	   /// <summary>
	   /// Size of a pixel in a RGB555 true colour image. </summary>
		private const int RGB5_SIZE = 16;
		/// <summary>
		/// Size of a pixel in a RGB8 true colour image. </summary>
		private const int RGB8_SIZE = 24;
		/// <summary>
		/// Size of a pixel in a RGB8 true colour image. </summary>
		private const int RGBA_SIZE = 32;

		/// <summary>
		/// Number of bits for each colour channel in a RGB555 pixel. </summary>
		private const int RGB5_DEPTH = 5;
		/// <summary>
		/// Number of bits for each colour channel in a RGB8 pixel. </summary>
		private const int RGB8_DEPTH = 8;

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
		/// Mask used to extract red channel from a 16-bit RGB555 pixel. </summary>
		private const int R5_MASK = 0x7C00;
		/// <summary>
		/// Mask used to extract green channel from a 16-bit RGB555 pixel. </summary>
		private const int G5_MASK = 0x03E0;
		/// <summary>
		/// Mask used to extract blue channel from a 16-bit RGB555 pixel. </summary>
		private const int B5_MASK = 0x001F;
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
		/// Mask used to extract red channel from a 16-bit RGB565 pixel. </summary>
		private const int R6_MASK = 0x7C00;
		/// <summary>
		/// Mask used to extract green channel from a 16-bit RGB565 pixel. </summary>
		private const int G6_MASK = 0x03E0;
		/// <summary>
		/// Mask used to extract blue channel from a 16-bit RGB565 pixel. </summary>
		private const int B6_MASK = 0x001F;
		/// <summary>
		/// Shift used to align the RGB565 red channel to a 8-bit pixel. </summary>
		private const int R6_SHIFT = 8;
		/// <summary>
		/// Shift used to align the RGB565 green channel to a 8-bit pixel. </summary>
		private const int G6_SHIFT = 3;
		/// <summary>
		/// Shift used to align the RGB565 blue channel to a 8-bit pixel. </summary>
		private const int B6_SHIFT = 3;

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
		/// The number of bits per pixel. </summary>
		
		private int bitDepth;
		/// <summary>
		/// The method used to compress the image. </summary>
		
		private int compressionMethod;
		/// <summary>
		/// The bit mask used to extract the red channel from the pixel word. </summary>
		
		private int redMask;
		/// <summary>
		/// Shift for the red pixel to convert to an 8-bit colour. </summary>
		
		private int redShift;
		/// <summary>
		/// The bit mask used to extract the green channel from the pixel word. </summary>
		
		private int greenMask;
		/// <summary>
		/// Shift for the green pixel to convert to an 8-bit colour. </summary>
		
		private int greenShift;
		/// <summary>
		/// The bit mask used to extract the blue channel from the pixel word. </summary>
		
		private int blueMask;
		/// <summary>
		/// Shift for the blue pixel to convert to an 8-bit colour. </summary>
		
		private int blueShift;
		/// <summary>
		/// Size of a pixel in bits. </summary>
		
		private int bitsPerPixel;
		/// <summary>
		/// Number of colours used in each pixel. </summary>
		
		private int coloursUsed;

		/// <summary>
		/// {@inheritDoc} </summary>



		public void read(FileInfo file)
		{
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
				@object = new DefineImage(identifier, width, height, table.Length / COLOUR_CHANNELS, zip(filter.merge(filter.adjustScan(width, height, image), table)));
				break;
			case ImageFormat.IDXA:
				@object = new DefineImage2(identifier, width, height, table.Length / COLOUR_CHANNELS, zip(filter.mergeAlpha(filter.adjustScan(width, height, image), table)));
				break;
			case ImageFormat.RGB5:
				@object = new DefineImage(identifier, width, height, zip(filter.packColors(width, height, image)), RGB5_SIZE);
				break;
			case ImageFormat.RGB8:
				filter.orderAlpha(image);
				@object = new DefineImage(identifier, width, height, zip(image), RGB8_SIZE);
				break;
			case ImageFormat.RGBA:
				filter.applyAlpha(image);
				@object = new DefineImage2(identifier, width, height, zip(image));
				break;
			default:
				throw new Exception(BAD_FORMAT);
			}
			return @object;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public ImageDecoder newDecoder()
		{
			return new BMPDecoder();
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
					copy = new byte[image.Length * COLOUR_CHANNELS];
    
					int tableIndex;
    
					for (int i = 0, index = 0; i < image.Length; i++)
					{
						tableIndex = image[i] * COLOUR_CHANNELS;
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

		/// <summary>
		/// {@inheritDoc} </summary>



		public void read(Stream stream)
		{



			LittleDecoder coder = new LittleDecoder(stream);
			coder.mark();

			for (int i = 0; i < 2; i++)
			{
				if (coder.readByte() != SIGNATURE[i])
				{
					throw new Exception(BAD_FORMAT);
				}
			}

			coder.readInt(); // fileSize
			coder.readInt(); // reserved



			int offset = coder.readInt();


			int headerSize = coder.readInt();

			if (headerSize == ZIPPED_LENGTH)
			{
				decodeCompressedHeader(coder);
			}
			else
			{
				decodeHeader(coder);
			}

			decodeFormat(bitsPerPixel);

			if (format == ImageFormat.IDX8)
			{
				coloursUsed = 1 << bitsPerPixel;

				if (headerSize == UNZIPPED_LENGTH)
				{
					decodeTable(coloursUsed, coder);
				}
				else
				{
					decodeTableWithAlpha(coloursUsed, coder);
				}

				coder.skip(offset - coder.bytesRead());
				decodeIndexedImage(coder);
			}
			else
			{
				coder.skip(offset - coder.bytesRead());
				decodeColourImage(coder);
			}
		}

		/// <summary>
		/// Decode the header record for an uncompressed image. </summary>
		/// <param name="coder"> the Decoder containing the data. </param>
		/// <exception cref="IOException"> if an error occurs during decoding. </exception>



		private void decodeHeader(LittleDecoder coder)
		{
			width = coder.readUnsignedShort();
			height = coder.readUnsignedShort();
			coder.readUnsignedShort(); // bitPlanes
			bitsPerPixel = coder.readUnsignedShort();
			coloursUsed = 0;
		}

		/// <summary>
		/// Decode the header record for an compressed image. </summary>
		/// <param name="coder"> the Decoder containing the data. </param>
		/// <exception cref="IOException"> if an error occurs during decoding. </exception>



		private void decodeCompressedHeader(LittleDecoder coder)
		{

			width = coder.readInt();
			height = coder.readInt();
			coder.readUnsignedShort(); // bitPlanes
			bitsPerPixel = coder.readUnsignedShort();
			compressionMethod = coder.readInt();
			coder.readInt(); // imageSize
			coder.readInt(); // horizontalResolution
			coder.readInt(); // verticalResolution
			coloursUsed = coder.readInt();
			coder.readInt(); // importantColours

			if (compressionMethod == BI_BITFIELDS)
			{
				decodeMasks(coder);
			}
		}

		/// <summary>
		/// Decode the bit masks to extra colour channels from a compressed image. </summary>
		/// <param name="coder"> the Decoder containing the data. </param>
		/// <exception cref="IOException"> if an error occurs during decoding. </exception>



		private void decodeMasks(LittleDecoder coder)
		{
			redMask = coder.readInt();
			greenMask = coder.readInt();
			blueMask = coder.readInt();

			if (redMask == R5_MASK)
			{
				redShift = R5_SHIFT;
			}
			else if (redMask == R6_MASK)
			{
				redShift = R6_SHIFT;
			}

			if (greenMask == G5_MASK)
			{
				greenShift = G5_SHIFT;
			}
			else if (greenMask == G6_MASK)
			{
				greenShift = G6_SHIFT;
			}

			if (blueMask == B5_MASK)
			{
				blueShift = B5_SHIFT;
			}
			else if (blueMask == B6_MASK)
			{
				blueShift = B6_SHIFT;
			}
		}

		/// <summary>
		/// Set the ImageFormat inferred from the pixel size. </summary>
		/// <param name="pixelSize"> the number of bits in each pixel. </param>
		/// <exception cref="Exception"> if the pixel size is not supported. </exception>



		private void decodeFormat(int pixelSize)
		{
			switch (pixelSize)
			{
			case IDX_1:
				format = ImageFormat.IDX8;
				bitDepth = pixelSize;
				break;
			case IDX_2:
				format = ImageFormat.IDX8;
				bitDepth = pixelSize;
				break;
			case IDX_4:
				format = ImageFormat.IDX8;
				bitDepth = pixelSize;
				break;
			case IDX_8:
				format = ImageFormat.IDX8;
				bitDepth = pixelSize;
				break;
			case RGB5_SIZE:
				format = ImageFormat.RGB5;
				bitDepth = RGB5_DEPTH;
				break;
			case RGB8_SIZE:
				format = ImageFormat.RGB8;
				bitDepth = RGB8_DEPTH;
				break;
			case RGBA_SIZE:
				format = ImageFormat.RGBA;
				bitDepth = RGB8_DEPTH;
				break;
			default:
				throw new Exception(BAD_FORMAT);
			}
		}

		/// <summary>
		/// Decode the colour palette with opaque colours.
		/// </summary>
		/// <param name="numColours"> the number of entries in the table. </param>
		/// <param name="coder"> the decoder containing the table data. </param>
		/// <exception cref="IOException"> if an error occurs while decoding the table. </exception>



		private void decodeTable(int numColours, LittleDecoder coder)
		{
			int index = 0;
			table = new byte[numColours * COLOUR_CHANNELS];

			for (int i = 0; i < numColours; i++)
			{
				table[index + ALPHA] = OPAQUE;
				table[index + BLUE] = (byte) coder.readByte();
				table[index + GREEN] = (byte) coder.readByte();
				table[index + RED] = (byte) coder.readByte();
				index += COLOUR_CHANNELS;
			}
		}

		/// <summary>
		/// Decode the colour palette with transparent colours.
		/// </summary>
		/// <param name="numColours"> the number of entries in the table. </param>
		/// <param name="coder"> the decoder containing the table data. </param>
		/// <exception cref="IOException"> if an error occurs while decoding the table. </exception>



		private void decodeTableWithAlpha(int numColours, LittleDecoder coder)
		{
			int index = 0;
			table = new byte[numColours * COLOUR_CHANNELS];

			for (int i = 0; i < numColours; i++)
			{
				table[index + RED] = (byte) coder.readByte();
				table[index + GREEN] = (byte) coder.readByte();
				table[index + BLUE] = (byte) coder.readByte();
				table[index + ALPHA] = (byte) coder.readByte();
				index += COLOUR_CHANNELS;
			}
		}

		/// <summary>
		/// Decode an indexed image. </summary>
		/// <param name="coder"> LittleDecoder object containing the encoded image data. </param>
		/// <exception cref="IOException"> if an error occurs reading the image data. </exception>
		/// <exception cref="Exception"> if the image is encoded in an unsupported
		/// format. </exception>



		private void decodeIndexedImage(LittleDecoder coder)
		{

			image = new byte[height * width];

			switch (compressionMethod)
			{
			case BI_RGB:
				decodeIDX8(coder);
				break;
			case BI_RLE8:
				decodeRLE8(coder);
				break;
			case BI_RLE4:
				decodeRLE4(coder);
				break;
			default:
				throw new Exception(BAD_FORMAT);
			}
		}

		/// <summary>
		/// Decode a true-colour image. </summary>
		/// <param name="coder"> LittleDecoder object containing the encoded image data. </param>
		/// <exception cref="IOException"> if an error occurs reading the image data. </exception>
		/// <exception cref="Exception"> if the image is encoded in an unsupported
		/// format. </exception>



		private void decodeColourImage(LittleDecoder coder)
		{

			image = new byte[height * width * COLOUR_CHANNELS];

			switch (format)
			{
			case ImageFormat.RGB5:
				decodeRGB5(coder);
				break;
			case ImageFormat.RGB8:
				decodeRGB8(coder);
				break;
			case ImageFormat.RGBA:
				decodeRGBA(coder);
				break;
			default:
				throw new Exception(BAD_FORMAT);
			}
		}

		/// <summary>
		/// Decode the indexed image data block (IDX8). </summary>
		/// <param name="coder"> the decoder containing the image data. </param>
		/// <exception cref="IOException"> is there is an error decoding the data. </exception>



		private void decodeIDX8(LittleDecoder coder)
		{
			int bitsRead;
			int index = 0;

			for (int row = height - 1; row > 0; row--)
			{
				bitsRead = 0;
				index = row * width;

				for (int col = 0; col < width; col++)
				{
					image[index++] = (byte) coder.readBits(bitDepth, false);
					bitsRead += bitDepth;
				}

				if (bitsRead % 32 > 0)
				{
					coder.readBits(32 - (bitsRead % 32), false);
				}
			}
		}

		/// <summary>
		/// Decode the run length encoded image data block (RLE4). </summary>
		/// <param name="coder"> the decoder containing the image data. </param>
		/// <exception cref="IOException"> is there is an error decoding the data. </exception>



		private void decodeRLE4(LittleDecoder coder)
		{
			int row = height - 1;
			int col = 0;
			int index = 0;
			int value;

			bool hasMore = true;
			int code;
			int count;

			while (hasMore)
			{
				count = coder.readByte();
				if (count == 0)
				{
					code = coder.readByte();

					switch (code)
					{
					case 0:
						col = 0;
						row--;
						break;
					case 1:
						hasMore = false;
						break;
					case 2:
						col += coder.readUnsignedShort();
						row -= coder.readUnsignedShort();
						decodeRLE4Pixels(code, coder, row, col);
						break;
					default:
						decodeRLE4Pixels(code, coder, row, col);
						break;
					}
				}
				else
				{
					value = coder.readByte();


					byte indexA = (byte)((int)((uint)value >> Coder.TO_LOWER_NIB));


					byte indexB = (byte)(value & Coder.NIB0);
					index = row * width + col;

					for (int i = 0; (i < count) && (col < width); i++, col++)
					{
						image[index++] = (i % 2 > 0) ? indexB : indexA;
					}
				}
			}
		}

		/// <summary>
		/// Decode a block of run-length encoded pixels where each pixel occupies
		/// 4 bits. </summary>
		/// <param name="code"> the number of pixels to decode. </param>
		/// <param name="coder"> a LittleEncoder object containing the encoded image. </param>
		/// <param name="row"> the row number for the starting pixel. </param>
		/// <param name="col"> the column number for the starting pixel. </param>
		/// <exception cref="IOException"> if there is an error reading the image data. </exception>



		private void decodeRLE4Pixels(int code, LittleDecoder coder, int row, int col)
		{
			int index = row * width + col;
			int value;
			for (int i = 0; i < code; i += 2)
			{
				value = coder.readByte();
				image[index++] = (byte)((int)((uint)value >> Coder.TO_LOWER_NIB));
				image[index++] = (byte)(value & Coder.NIB0);
			}

			if ((code & 2) == 2)
			{
				coder.readByte();
			}
		}


		/// <summary>
		/// Decode the run length encoded image data block (RLE8). </summary>
		/// <param name="coder"> the decoder containing the image data. </param>
		/// <exception cref="IOException"> is there is an error decoding the data. </exception>



		private void decodeRLE8(LittleDecoder coder)
		{
			int row = height - 1;
			int col = 0;

			bool hasMore = true;
			int code;
			int count;

			while (hasMore)
			{
				count = coder.readByte();

				if (count == 0)
				{
					code = coder.readByte();

					switch (code)
					{
					case 0:
						col = 0;
						row--;
						break;
					case 1:
						hasMore = false;
						break;
					case 2:
						col += coder.readUnsignedShort();
						row -= coder.readUnsignedShort();
						decodeRLE8Pixels(code, coder, row, col);
						break;
					default:
						decodeRLE8Pixels(code, coder, row, col);
						break;
					}
				}
				else
				{
					decodeRLE8Run(count, row, col, (byte) coder.readByte());
				}
			}
		}

		/// <summary>
		/// Decode a block of run-length encoded pixels where each pixel occupies
		/// 8 bits. </summary>
		/// <param name="code"> the number of pixels to decode. </param>
		/// <param name="coder"> a LittleEncoder object containing the encoded image. </param>
		/// <param name="row"> the row number for the starting pixel. </param>
		/// <param name="col"> the column number for the starting pixel. </param>
		/// <exception cref="IOException"> if there is an error reading the image data. </exception>



		private void decodeRLE8Pixels(int code, LittleDecoder coder, int row, int col)
		{
			int index = row * width + col;
			for (int i = 0; i < code; i++)
			{
				image[index++] = (byte) coder.readByte();
			}

			if ((code & 1) == 1)
			{
				coder.readByte();
			}
		}

		/// <summary>
		/// Decode a series of pixels where each pixel occupies 8 bits and has the
		/// same value. </summary>
		/// <param name="count"> the number of pixels to decode. </param>
		/// <param name="row"> the row number for the starting pixel. </param>
		/// <param name="col"> the column number for the starting pixel. </param>
		/// <param name="value"> value for each pixel. </param>


		private void decodeRLE8Run(int count, int row, int col, byte value)
		{
			int index = row * width + col;

			for (int i = 0; i < count; i++)
			{
				image[index++] = value;
			}
		}

		/// <summary>
		/// Decode the true colour image with each colour channel taking 5-bits. </summary>
		/// <param name="coder"> the decoder containing the image data. </param>
		/// <exception cref="IOException"> is there is an error decoding the data. </exception>



		private void decodeRGB5(LittleDecoder coder)
		{
			int index = 0;
			int colour;

			for (int row = height - 1; row > 0; row--)
			{
				coder.mark();
				for (int col = 0; col < width; col++)
				{
					colour = coder.readUnsignedShort();
					image[index + RED] = (byte)((colour & redMask) >> redShift);
					image[index + GREEN] = (byte)((colour & greenMask) >> greenShift);
					image[index + BLUE] = (byte)((colour & blueMask) << blueShift);
					image[index + ALPHA] = OPAQUE;
					index += COLOUR_CHANNELS;
				}
				coder.alignToWord();
				coder.unmark();
			}
		}

		/// <summary>
		/// Decode the true colour image with each colour channel taking 8-bits. </summary>
		/// <param name="coder"> the decoder containing the image data. </param>
		/// <exception cref="IOException"> is there is an error decoding the data. </exception>



		private void decodeRGB8(LittleDecoder coder)
		{
			int bytesRead;
			int index = 0;
			for (int row = height - 1; row > 0; row--)
			{
				bytesRead = 0;
				for (int col = 0; col < width; col++)
				{
					image[index + RED] = (byte) coder.readByte();
					image[index + GREEN] = (byte) coder.readByte();
					image[index + BLUE] = (byte) coder.readByte();
					image[index + ALPHA] = OPAQUE;
					index += COLOUR_CHANNELS;
					bytesRead += 3;
				}
				if (bytesRead % 4 > 0)
				{
					coder.readBytes(new byte[4 - (bytesRead % 4)]);
				}
			}
		}

		/// <summary>
		/// Decode the true colour image with each colour channel and alpha taking
		/// 8-bits. </summary>
		/// <param name="coder"> the decoder containing the image data. </param>
		/// <exception cref="IOException"> is there is an error decoding the data. </exception>



		private void decodeRGBA(LittleDecoder coder)
		{
			int index = 0;

			for (int row = height - 1; row > 0; row--)
			{
				for (int col = 0; col < width; col++)
				{
					image[index + BLUE] = (byte) coder.readByte();
					image[index + GREEN] = (byte) coder.readByte();
					image[index + RED] = (byte) coder.readByte();
					// force alpha channel to be opaque
					image[index + ALPHA] = (byte) coder.readByte();
					image[index + ALPHA] = OPAQUE;
					index += COLOUR_CHANNELS;
				}
			}
		}

		/// <summary>
		/// Compress the image using the ZIP format. </summary>
		/// <param name="img"> the image data. </param>
		/// <returns> the compressed image. </returns>


		private byte[] zip(byte[] img)
		{


			Deflater deflater = new Deflater();
			deflater.Input = img;
			deflater.finish();



			byte[] compressedData = new byte[img.Length * 2];


			int bytesCompressed = deflater.deflate(compressedData);


			byte[] newData = Arrays.copyOf(compressedData, bytesCompressed);

			return newData;
		}
	}

}