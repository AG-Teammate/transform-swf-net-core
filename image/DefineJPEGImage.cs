using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * DefineJPEGImage.java
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
	/// DefineJPEGImage is used to define a JPEG encoded image.
	/// 
	/// <para>
	/// DefineJPEGImage objects only contain the image data, the encoding table for
	/// the image is defined in a JPEGEncodingTable object. All images using a shared
	/// JPEGEncodingTable object to represent the encoding table have the same
	/// compression ratio.
	/// </para>
	/// 
	/// <para>
	/// Although the DefineJPEGImage class is supposed to be used with the
	/// JPEGEncodingTable class which defines the encoding table for the images it is
	/// not essential. If an JPEGEncodingTable object is created with an empty
	/// encoding table then the Flash Player will still display the JPEG image
	/// correctly if the encoding table is included in the image data.
	/// </para>
	/// </summary>
	/// <seealso cref= JPEGEncodingTable </seealso>
	/// <seealso cref= DefineJPEGImage2 </seealso>
	/// <seealso cref= DefineJPEGImage3 </seealso>
	public sealed class DefineJPEGImage : ImageTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineJPEGImage: { identifier=%d;" + " image=byte<%d> ...}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The JPEG encoded image. </summary>
		private byte[] image;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// The width of the image in pixels. </summary>
		
		private int width;
		/// <summary>
		/// The height of the image in pixels. </summary>
		
		private int height;

		/// <summary>
		/// Creates and initialises a DefineJPEGImage object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public DefineJPEGImage(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			image = coder.readBytes(new byte[length - 2]);
			decodeInfo();
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a DefineJPEGImage object with the identifier and JPEG data.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for this object. Must be in the range
		///            1..65535. </param>
		/// <param name="bytes">
		///            the JPEG encoded image data. Must not be null. </param>


		public DefineJPEGImage(int uid, byte[] bytes)
		{
			Identifier = uid;
			Image = bytes;
		}

		/// <summary>
		/// Creates and initialises a DefineJPEGImage object using the values copied
		/// from another DefineJPEGImage object.
		/// </summary>
		/// <param name="object">
		///            a DefineJPEGImage object from which the values will be
		///            copied. </param>


		public DefineJPEGImage(DefineJPEGImage @object)
		{
			identifier = @object.identifier;
			width = @object.width;
			height = @object.height;
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
		/// Get the width of the image in pixels.
		/// </summary>
		/// <returns> the image width. </returns>
		public int Width => width;

	    /// <summary>
		/// Get the height of the image in pixels (not twips).
		/// </summary>
		/// <returns> the height of the image. </returns>
		public int Height => height;

	    /// <summary>
		/// Get a copy of the image.
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
				decodeInfo();
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public DefineJPEGImage copy()
		{
			return new DefineJPEGImage(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, image.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 2 + image.Length;

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_JPEG_IMAGE << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_JPEG_IMAGE << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			coder.writeBytes(image);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}

		/// <summary>
		/// Decode the JPEG image to get the width and height.
		/// </summary>
		private void decodeInfo()
		{


			JPEGInfo info = new JPEGInfo();
			info.decode(image);
			width = info.Width;
			height = info.Height;
		}
	}

}