/*
 * Background.java
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

using System;
using System.IO;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;

namespace com.flagstone.transform
{
    /// <summary>
	/// Background sets the background colour displayed in every frame in the movie.
	/// 
	/// <para>
	/// Although the colour is specified using an Color object the colour displayed
	/// is completely opaque - the alpha channel information in the object is
	/// ignored.
	/// </para>
	/// 
	/// <para>
	/// The background colour must be set before the first frame is displayed
	/// otherwise the background colour defaults to white. This is typically the
	/// first object in a coder. If more than one Background object is added to a
	/// movie then only first one sets the background colour. Subsequent objects are
	/// ignored.
	/// </para>
	/// </summary>
	/// <seealso cref= Color </seealso>
	public sealed class Background : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Background: { color=%s}";

		/// <summary>
		/// The colour that will be displayed on the screen background. </summary>
		private Color color;

		/// <summary>
		/// Creates and initialises a Background object using values encoded in the
		/// Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <param name="context">
		///            a Context object used to manage the decoders for different
		///            type of object and to pass information on how objects are
		///            decoded.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Background(SWFDecoder coder, Context context)
		{
			int length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			color = new Color(coder, context);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a Background object with a the specified colour.
		/// </summary>
		/// <param name="aColor">
		///            the colour for the background. Must not be null. </param>


		public Background(Color aColor)
		{
			Color = aColor;
		}

		/// <summary>
		/// Creates and initialises an Background object using the values
		/// copied from another Background object.
		/// </summary>
		/// <param name="object">
		///            a Background object from which the values will be
		///            copied. </param>


		public Background(Background @object)
		{
			color = @object.color;
		}

		/// <summary>
		/// Get the colour for the movie background.
		/// </summary>
		/// <returns> the Color for the background of the Flash Player screen. </returns>
		public Color Color
		{
			get => color;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				color = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public Background copy()
		{
			return new Background(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, color.ToString());
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			return 2 + Color.RGB;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeShort((MovieTypes.SET_BACKGROUND_COLOR << Coder.LENGTH_FIELD_SIZE) | Color.RGB);
			color.encode(coder, context);
		}
	}

}