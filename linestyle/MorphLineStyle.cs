/*
 * MorphLineStyle.java
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
using com.flagstone.transform.exception;

namespace com.flagstone.transform.linestyle
{
    /// <summary>
	/// MorphLineStyle defines the width and colour of a line drawn for a shape is it
	/// is morphed.
	/// 
	/// <para>
	/// MorphLineStyle specifies the width and colour of the line at the start and
	/// end of the morphing process. The transparency value for the colour should
	/// also be specified. The Flash Player performs the interpolation as the shape
	/// is morphed.
	/// </para>
	/// </summary>
	public sealed class MorphLineStyle : LineStyle
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "MorphSolidLine: { startWidth=%d;" + " endWidth=%d; startColor=%s; endColor=%s}";

		/// <summary>
		/// Width of the line at the start of the morph. </summary>
		private int startWidth;
		/// <summary>
		/// Width of the line at the end of the morph. </summary>
		private int endWidth;
		/// <summary>
		/// Color of the line at the start of the morph. </summary>
		private Color startColor;
		/// <summary>
		/// Color of the line at the end of the morph. </summary>
		private Color endColor;

		/// <summary>
		/// Creates and initialises a MorphLineStyle object using values encoded
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



		public MorphLineStyle(SWFDecoder coder, Context context)
		{
			startWidth = coder.readUnsignedShort();
			endWidth = coder.readUnsignedShort();
			startColor = new Color(coder, context);
			endColor = new Color(coder, context);
		}

		/// <summary>
		/// Creates a MorphLineStyle object specifying the starting and ending widths
		/// and colours.
		/// </summary>
		/// <param name="initialWidth">
		///            the width of the line at the start of the morphing process. </param>
		/// <param name="finalWidth">
		///            the width of the line at the end of the morphing process. </param>
		/// <param name="initialColor">
		///            the colour of the line at the start of the morphing process. </param>
		/// <param name="finalColor">
		///            the colour of the line at the end of the morphing process. </param>


		public MorphLineStyle(int initialWidth, int finalWidth, Color initialColor, Color finalColor)
		{

			StartWidth = initialWidth;
			EndWidth = finalWidth;
			StartColor = initialColor;
			EndColor = finalColor;
		}

		/// <summary>
		/// Creates and initialises a MorphLineStyle object using the values copied
		/// from another MorphLineStyle object.
		/// </summary>
		/// <param name="object">
		///            a MorphLineStyle object from which the values will be
		///            copied. </param>


		public MorphLineStyle(MorphLineStyle @object)
		{
			startWidth = @object.startWidth;
			endWidth = @object.endWidth;
			startColor = @object.startColor;
			endColor = @object.endColor;
		}

		/// <summary>
		/// Get the width of the line at the start of the morphing process.
		/// </summary>
		/// <returns> the starting stroke width. </returns>
		public int StartWidth
		{
			get => startWidth;
		    set
			{
				if ((value < 0) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value);
				}
				startWidth = value;
			}
		}

		/// <summary>
		/// Get the width of the line at the end of the morphing process.
		/// </summary>
		/// <returns> the final stroke width. </returns>
		public int EndWidth
		{
			get => endWidth;
		    set
			{
				if ((value < 0) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value);
				}
				endWidth = value;
			}
		}

		/// <summary>
		/// Get the colour of the line at the start of the morphing process.
		/// </summary>
		/// <returns> the starting stroke colour. </returns>
		public Color StartColor
		{
			get => startColor;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				startColor = value;
			}
		}

		/// <summary>
		/// Returns the colour of the line at the end of the morphing process.
		/// </summary>
		/// <returns> the final stroke colour. </returns>
		public Color EndColor
		{
			get => endColor;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				endColor = value;
			}
		}





		/// <summary>
		/// {@inheritDoc} </summary>
		public MorphLineStyle copy()
		{
			return new MorphLineStyle(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, startWidth, endWidth, startColor, endColor);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 12;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeShort(startWidth);
			coder.writeShort(endWidth);
			startColor.encode(coder, context);
			endColor.encode(coder, context);
		}
	}

}