using System;
using System.Collections.Generic;

/*
 * ImageEncoding.java
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
	/// ImageEncoding describes the different image formats that can be decoded and
	/// added to a Flash movie.
	/// </summary>
	public sealed class ImageEncoding
	{
		/// <summary>
		/// Windows Bitmap images. </summary>
		public static readonly ImageEncoding BMP = new ImageEncoding("BMP", InnerEnum.BMP, "image/bmp", new BMPDecoder());
		/// <summary>
		/// Joint Photographic Experts Group format images. </summary>
		public static readonly ImageEncoding JPEG = new ImageEncoding("JPEG", InnerEnum.JPEG, "image/jpeg", new JPGDecoder());
		/// <summary>
		/// Portable Network Graphics images. </summary>
		public static readonly ImageEncoding PNG = new ImageEncoding("PNG", InnerEnum.PNG, "image/png", new PNGDecoder());

		private static readonly IList<ImageEncoding> valueList = new List<ImageEncoding>();

		static ImageEncoding()
		{
			valueList.Add(BMP);
			valueList.Add(JPEG);
			valueList.Add(PNG);
		}

		public enum InnerEnum
		{
			BMP,
			JPEG,
			PNG
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal;

		/// <summary>
		/// The MIME type used to identify the image format. </summary>
		private readonly string mimeType;
		/// <summary>
		/// The ImageProvider that can be used to decode the image format. </summary>
		private readonly ImageProvider provider;

		/// <summary>
		/// Private constructor for the enum.
		/// </summary>
		/// <param name="type"> the string representing the mime-type. </param>
		/// <param name="imageProvider"> the ImageProvider that can be used to decode the
		/// image format. </param>


		private ImageEncoding(string name, InnerEnum innerEnum, string type, ImageProvider imageProvider)
		{
			mimeType = type;
			provider = imageProvider;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Get the mime-type used to represent the image format.
		/// </summary>
		/// <returns> the string identifying the image format. </returns>
		public string MimeType => mimeType;

	    /// <summary>
		/// Get the ImageProvider that can be registered in the ImageRegistry to
		/// decode the image.
		/// </summary>
		/// <returns> the ImageProvider that can be used to decode images of the given
		/// mime-type. </returns>
		public ImageProvider Provider => provider;

	    public static IList<ImageEncoding> values()
		{
			return valueList;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static ImageEncoding valueOf(string name)
		{
			foreach (ImageEncoding enumInstance in valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new ArgumentException(name);
		}
	}

}