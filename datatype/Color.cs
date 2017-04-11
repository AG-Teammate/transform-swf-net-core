using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * Color.java
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
namespace com.flagstone.transform.datatype
{
    /// <summary>
	/// Color is used to represent 32-bit colours in the RGB colour space with 8 bits
	/// per channel and an optional alpha channel.
	/// 
	/// <para>
	/// Whether a colour contains transparency information is determined by the
	/// object that contains the colour. For example the colours in a DefineShape or
	/// DefineShape2 do not use the alpha channel while those in DefineShape3 do. The
	/// Context object, passed to each Color object, when it is encoded or decoded
	/// signals whether the alpha channel should be included.
	/// </para>
	/// </summary>
	public sealed class Color : SWFEncodeable
	{

		/// <summary>
		/// The number of channels in an opaque Color object. Only used within the
		/// framework or when adding a new class.
		/// </summary>
		public const int RGB = 3;
		/// <summary>
		/// The number of channels in a transparent Color object. Only used within
		/// the framework or when adding a new class.
		/// </summary>
		public const int RGBA = 4;

		/// <summary>
		/// Format used by toString() to display object representation. </summary>
		private const string FORMAT = "Color: {" + " red=%d; green=%d; blue=%d; alpha=%d}";

		/// <summary>
		/// The minimum value that can be assigned to a colour channel. </summary>
		public const int MIN_LEVEL = 0;
		/// <summary>
		/// The maximum value that can be assigned to a colour channel. </summary>
		public const int MAX_LEVEL = 255;

		/// <summary>
		/// Holds the value for the red colour channel. </summary>
		
		private readonly int red;
		/// <summary>
		/// Holds the value for the green colour channel. </summary>
		
		private readonly int green;
		/// <summary>
		/// Holds the value for the blue colour channel. </summary>
		
		private readonly int blue;
		/// <summary>
		/// Holds the value for the alpha colour channel. Defaults to MAX_LEVEL
		/// (255) when used in classes such as DefineShape or DefineShape2
		/// that only support opaque colours. 
		/// </summary>
		
		private readonly int alpha;

		/// <summary>
		/// Creates and initialises a Color object using values encoded
		/// in the Flash binary format.
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



		public Color(SWFDecoder coder, Context context)
		{
			red = coder.readByte();
			green = coder.readByte();
			blue = coder.readByte();

			if (context.contains(Context.TRANSPARENT))
			{
				alpha = coder.readByte();
			}
			else
			{
				alpha = MAX_LEVEL;
			}
		}

		/// <summary>
		/// Creates a Color object containing red, green and blue channels. The alpha
		/// channel defaults to the value 255 - defining an opaque colour.
		/// </summary>
		/// <param name="rVal">
		///            value for the red channel, in the range 0..255. </param>
		/// <param name="gVal">
		///            value for the green channel, in the range 0..255. </param>
		/// <param name="bVal">
		///            value for the blue channel, in the range 0..255. </param>


		public Color(int rVal, int gVal, int bVal) : this(rVal, gVal, bVal, MAX_LEVEL)
		{
		}

		/// <summary>
		/// Creates a transparent Color object containing red, green, blue and alpha
		/// channels.
		/// </summary>
		/// <param name="rVal">
		///            value for the red channel, in the range 0..255. </param>
		/// <param name="gVal">
		///            value for the green channel, in the range 0..255. </param>
		/// <param name="bVal">
		///            value for the blue channel, in the range 0..255. </param>
		/// <param name="aVal">
		///            value for the alpha channel, in the range 0..255. </param>


		public Color(int rVal, int gVal, int bVal, int aVal)
		{
			if ((rVal < MIN_LEVEL) || (rVal > MAX_LEVEL))
			{
				throw new IllegalArgumentRangeException(MIN_LEVEL, MAX_LEVEL, rVal);
			}
			if ((gVal < MIN_LEVEL) || (gVal > MAX_LEVEL))
			{
				throw new IllegalArgumentRangeException(MIN_LEVEL, MAX_LEVEL, gVal);
			}
			if ((bVal < MIN_LEVEL) || (bVal > MAX_LEVEL))
			{
				throw new IllegalArgumentRangeException(MIN_LEVEL, MAX_LEVEL, bVal);
			}
			if ((aVal < MIN_LEVEL) || (aVal > MAX_LEVEL))
			{
				throw new IllegalArgumentRangeException(MIN_LEVEL, MAX_LEVEL, aVal);
			}

			red = rVal;
			green = gVal;
			blue = bVal;
			alpha = aVal;
		}

		/// <summary>
		/// Returns the value for the red colour channel.
		/// </summary>
		/// <returns> the level for the red colour channel in the range 0..255. </returns>
		public int Red => red;

	    /// <summary>
		/// Returns the value for the green colour channel.
		/// </summary>
		/// <returns> the level for the green colour channel in the range 0..255. </returns>
		public int Green => green;

	    /// <summary>
		/// Returns the value for the blue colour channel.
		/// </summary>
		/// <returns> the level for the blue colour channel in the range 0..255. </returns>
		public int Blue => blue;

	    /// <summary>
		/// Returns the value for the alpha colour channel.
		/// </summary>
		/// <returns> the level for the alpha channel in the range 0..255. </returns>
		public int Alpha => alpha;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, red, green, blue, alpha);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public override bool Equals(object @object)
		{
			bool result;
			Color color;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is Color)
			{
				color = (Color) @object;
				result = (red == color.red) && (green == color.green) && (blue == color.blue) && (alpha == color.alpha);
			}
			else
			{
				result = false;
			}
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			return ((red * Constants.PRIME + green) * Constants.PRIME + blue) * Constants.PRIME + alpha;
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			int size;
			if (context.contains(Context.TRANSPARENT))
			{
				size = RGBA;
			}
			else
			{
				size = RGB;
			}
			return size;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(red);
			coder.writeByte(green);
			coder.writeByte(blue);

			if (context.contains(Context.TRANSPARENT))
			{
				coder.writeByte(alpha);
			}
		}
	}

}