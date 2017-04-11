using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * DefineImage2.java
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

namespace com.flagstone.transform.image
{
    /// <summary>
	/// DefineImage2 is used to define a transparent image compressed using the
	/// lossless zlib compression algorithm.
	/// 
	/// <para>
	/// The class supports colour-mapped images where the image data contains an
	/// index into a colour table or direct-mapped images where the colour is
	/// specified directly. It extends DefineImage by including alpha channel
	/// information for the colour table and pixels in the image.
	/// </para>
	/// 
	/// <para>
	/// For colour-mapped images the colour table contains up to 256, 32-bit colours.
	/// The image contains one byte for each pixel which is an index into the table
	/// to specify the colour for that pixel. The colour table and the image data are
	/// compressed as a single block, with the colour table placed before the image.
	/// </para>
	/// 
	/// <para>
	/// For images where the colour is specified directly, the image data contains 32
	/// bit colour values.
	/// </para>
	/// 
	/// <para>
	/// The image data is stored in zlib compressed form within the object. For
	/// colour-mapped images the compressed data contains the colour table followed
	/// by the image data.
	/// </para>
	/// </summary>
	/// <seealso cref= DefineImage </seealso>
	public sealed class DefineImage2 : ImageTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineImage2: { identifier=%d;" + " width=%d; height=%d; pixelSize=%d; tableSize=%d;" + " image=byte<%d> ...}";

		/// <summary>
		/// Identifies an indexed image. </summary>
		private const int IDX_FORMAT = 3;
		/// <summary>
		/// Identifies a true-color image with 32-bit pixels. </summary>
		private const int RGBA_FORMAT = 5;

		/// <summary>
		/// Size of a pixel in an indexed image. </summary>
		private const int IDX_SIZE = 8;
		/// <summary>
		/// Size of a pixel in an RGBA image. </summary>
		private const int RGBA_SIZE = 32;
		/// <summary>
		/// Number of entries in the colour table of an indexed image. </summary>
		private const int TABLE_SIZE = 256;

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The width of the image in pixels. </summary>
		private int width;
		/// <summary>
		/// The height of the image in pixels. </summary>
		private int height;
		/// <summary>
		/// The number of bits in a pixel. </summary>
		private int pixelSize;
		/// <summary>
		/// The number of entries in the colour table. </summary>
		private int tableSize;
		/// <summary>
		/// The compressed colour table and image data. </summary>
		private byte[] image;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises an DefineImage2 object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public DefineImage2(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();

			if (coder.readByte() == IDX_FORMAT)
			{
				pixelSize = IDX_SIZE;
			}
			else
			{
				pixelSize = RGBA_SIZE;
			}

			width = coder.readUnsignedShort();
			height = coder.readUnsignedShort();

			if (pixelSize == IDX_SIZE)
			{
				tableSize = coder.readByte() + 1;
				// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
				image = coder.readBytes(new byte[length - 8]);
			}
			else
			{
				// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
				image = coder.readBytes(new byte[length - 7]);
			}
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a DefineImage2 object defining a colour-mapped image.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for this object. Must be in the range
		///            1..65535. </param>
		/// <param name="imgWidth">
		///            the width of the image. Must be in the range 0..65535. </param>
		/// <param name="imgHeight">
		///            the height of the image. Must be in the range 0..65535. </param>
		/// <param name="size">
		///            the number of entries in the colour table in the compressed
		///            data. Each entry is 32 bits. Must be in the range 1..256. </param>
		/// <param name="data">
		///            the zlib compressed colour table and image data. Must not be
		///            null. </param>


		public DefineImage2(int uid, int imgWidth, int imgHeight, int size, byte[] data)
		{
			Identifier = uid;
			Width = imgWidth;
			Height = imgHeight;
			PixelSize = IDX_SIZE;
			TableSize = size;
			Image = data;
		}

		/// <summary>
		/// Creates a DefineImage object defining a true-colour image. Each pixel in
		/// the image is 32 bits - 8 bits for the red, green, blue and alpha colour
		/// channels.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for this object. Must be in the range
		///            1..65535. </param>
		/// <param name="imgWidth">
		///            the width of the image. Must be in the range 0..65535. </param>
		/// <param name="imgHeight">
		///            the height of the image. Must be in the range 0..65535. </param>
		/// <param name="data">
		///            the zlib compressed image data. Must not be null. </param>


		public DefineImage2(int uid, int imgWidth, int imgHeight, byte[] data)
		{
			Identifier = uid;
			Width = imgWidth;
			Height = imgHeight;
			PixelSize = RGBA_SIZE;
			tableSize = 0;
			Image = data;
		}

		/// <summary>
		/// Creates and initialises a DefineImage2 object using the values copied
		/// from another DefineImage2 object.
		/// </summary>
		/// <param name="object">
		///            a DefineImage2 object from which the values will be
		///            copied. </param>


		public DefineImage2(DefineImage2 @object)
		{
			identifier = @object.identifier;
			width = @object.width;
			height = @object.height;
			pixelSize = @object.pixelSize;
			tableSize = @object.tableSize;
			image = @object.image;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public int Identifier
		{
			get => identifier;
		    set
			{
				if ((value < 1) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, value);
				}
				identifier = value;
			}
		}


		/// <summary>
		/// Get the width of the image in pixels (not twips).
		/// </summary>
		/// <returns> the width of the image. </returns>
		public int Width
		{
			get => width;
		    set
			{
				if ((value < 0) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value);
				}
				width = value;
			}
		}

		/// <summary>
		/// Get the height of the image in pixels (not twips).
		/// </summary>
		/// <returns> the height of the image. </returns>
		public int Height
		{
			get => height;
		    set
			{
				if ((value < 0) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value);
				}
				height = value;
			}
		}

		/// <summary>
		/// Get the number of bits used to represent each pixel. Either 8 or 32
		/// bits. The pixel size is 8-bits for colour-mapped images and 32 bits for
		/// images where the colour is specified directly.
		/// </summary>
		/// <returns> the number of bits for each pixel. </returns>
		public int PixelSize
		{
			get => pixelSize;
		    set
			{
				if ((value != IDX_SIZE) && (value != RGBA_SIZE))
				{
					throw new ArgumentException("Pixel size must be either 8 or 32 bits.");
				}
				pixelSize = value;
			}
		}

		/// <summary>
		/// Get the number of entries in the colour table encoded the compressed
		/// image. For images where the colour is specified directly in the image
		/// then the table size is zero.
		/// </summary>
		/// <returns> the number of entries in the colour table. </returns>
		public int TableSize
		{
			get => tableSize;
		    set
			{
				if ((value < 1) || (value > TABLE_SIZE))
				{
					throw new ArgumentException("Colour table size must be in the range 1..256.");
				}
				tableSize = value;
			}
		}

		/// <summary>
		/// Get a copy of the compressed colour table and image.
		/// </summary>
		/// <returns>  a copy of the data. </returns>
		public byte[] Image
		{
			get => Arrays.copyOf(image, image.Length);
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				image = Arrays.copyOf(value, value.Length);
			}
		}






		/// <summary>
		/// {@inheritDoc} </summary>
		public DefineImage2 copy()
		{
			return new DefineImage2(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, width, height, pixelSize, tableSize, image.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			length = 7;
			length += (pixelSize == IDX_SIZE) ? 1 : 0;
			length += image.Length;

			return Coder.LONG_HEADER + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			coder.writeShort((MovieTypes.DEFINE_IMAGE_2 << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
			coder.writeInt(length);
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);

			if (pixelSize == IDX_SIZE)
			{
				coder.writeByte(IDX_FORMAT);
			}
			else
			{ // 32
				coder.writeByte(RGBA_FORMAT);
			}

			coder.writeShort(width);
			coder.writeShort(height);

			if (pixelSize == IDX_SIZE)
			{
				coder.writeByte(tableSize - 1);
			}

			coder.writeBytes(image);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}