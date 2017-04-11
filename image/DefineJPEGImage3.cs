﻿using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * DefineJPEGImage3.java
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
	/// DefineJPEGImage3 is used to define a transparent JPEG encoded image.
	/// 
	/// <para>
	/// It extends the DefineJPEGImage2 class by including a separate zlib compressed
	/// table of alpha channel values. This allows the transparency of existing JPEG
	/// encoded images to be changed without re-encoding the original image.
	/// </para>
	/// </summary>
	/// <seealso cref= DefineJPEGImage2 </seealso>
	public sealed class DefineJPEGImage3 : ImageTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineJPEGImage3: { identifier=%d;" + "image=byte<%d> ...; alpha=byte<%d> ...}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The JPEG encoded image. </summary>
		private byte[] image;
		/// <summary>
		/// The zlib compressed transparency values for the image. </summary>
		private byte[] alpha;

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
		/// Creates and initialises a DefineJPEGImage3 object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public DefineJPEGImage3(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();


			int offset = coder.readInt();
			image = coder.readBytes(new byte[offset]);
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			alpha = coder.readBytes(new byte[length - offset - 6]);
			decodeInfo();
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a DefineJPEGImage3 object with the specified image data, and
		/// alpha channel data.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for this object. Must be in the range
		///            1..65535. </param>
		/// <param name="img">
		///            the JPEG encoded image data. Must not be null. </param>
		/// <param name="transparency">
		///            byte array containing the zlib compressed alpha channel data.
		///            Must not be null. </param>


		public DefineJPEGImage3(int uid, byte[] img, byte[] transparency)
		{
			Identifier = uid;
			Image = img;
			Alpha = transparency;
		}

		/// <summary>
		/// Creates and initialises a DefineJPEGImage3 object using the values copied
		/// from another DefineJPEGImage3 object.
		/// </summary>
		/// <param name="object">
		///            a DefineJPEGImage3 object from which the values will be
		///            copied. </param>


		public DefineJPEGImage3(DefineJPEGImage3 @object)
		{
			identifier = @object.identifier;
			width = @object.width;
			height = @object.height;
			image = @object.image;
			alpha = @object.alpha;
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
				image = Arrays.copyOf(value, value.Length);
				decodeInfo();
			}
		}

		/// <summary>
		/// Get a copy of the alpha channel.
		/// </summary>
		/// <returns>  a copy of the data. </returns>
		public byte[] Alpha
		{
			get => Arrays.copyOf(alpha, alpha.Length);
		    set => alpha = Arrays.copyOf(value, value.Length);
		}



		/// <summary>
		/// {@inheritDoc} </summary>
		public DefineJPEGImage3 copy()
		{
			return new DefineJPEGImage3(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, image.Length, alpha.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			length = 6;
			length += image.Length;
			length += alpha.Length;

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_JPEG_IMAGE_3 << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_JPEG_IMAGE_3 << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			coder.writeInt(image.Length);
			coder.writeBytes(image);
			coder.writeBytes(alpha);
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