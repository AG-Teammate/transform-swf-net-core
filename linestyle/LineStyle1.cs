/*
 * LineStyle.java
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
	/// LineStyle1 defines the width and colour of a line that is used when drawing
	/// the outline of a shape.
	/// 
	/// <para>
	/// All lines are drawn with rounded corners and end caps. Different join and
	/// line end styles can be created by drawing line segments as a sequence of
	/// filled shapes. With 1 twip equal to 1/20th of a pixel this technique can
	/// easily be used to draw the narrowest of visible lines. Note that specific
	/// join and cap styles can be specified with the <seealso cref="LineStyle2"/> class.
	/// </para>
	/// 
	/// <para>
	/// Whether the alpha channel in the colour is used is determined by the class
	/// used to define the shape. Transparent colours are only supported from Flash 3
	/// onwards. Simply specifying the level of transparency in the Color object is
	/// not sufficient.
	/// </para>
	/// 
	/// <para>
	/// Flash only supports contiguous lines. Dashed line styles can be created by
	/// drawing the line as a series of short line segments by interspersing
	/// ShapeStyle objects to move the current point in between the Line objects that
	/// draw the line segments.
	/// </para>
	/// </summary>
	/// <seealso cref= LineStyle2 </seealso>
	public sealed class LineStyle1 : LineStyle
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "LineStyle : { width=%d; color=%s}";

		/// <summary>
		/// Width of the line in twips. </summary>
		private int width;
		/// <summary>
		/// Colour used to draw the line. </summary>
		private Color color;

		/// <summary>
		/// Creates and initialises a LineStyle object using values encoded
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



		public LineStyle1(SWFDecoder coder, Context context)
		{
			width = coder.readUnsignedShort();
			color = new Color(coder, context);
		}

		/// <summary>
		/// Creates a LineStyle, specifying the width and colour of the line.
		/// </summary>
		/// <param name="aWidth">
		///            the width of the line. Must be in the range 0..65535. </param>
		/// <param name="aColor">
		///            the colour of the line. Must not be null. </param>


		public LineStyle1(int aWidth, Color aColor)
		{
			Width = aWidth;
			Color = aColor;
		}

		/// <summary>
		/// Creates and initialises a LineStyle object using the values copied
		/// from another LineStyle object.
		/// </summary>
		/// <param name="object">
		///            a LineStyle object from which the values will be
		///            copied. </param>


		public LineStyle1(LineStyle1 @object)
		{
			width = @object.width;
			color = @object.color;
		}

		/// <summary>
		/// Get the width of the line.
		/// </summary>
		/// <returns> the stroke width. </returns>
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
		/// Get the colour of the line.
		/// </summary>
		/// <returns> the line colour. </returns>
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
		public LineStyle1 copy()
		{
			return new LineStyle1(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, width, color);
		}


		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 2 + (context.contains(Context.TRANSPARENT) ? 4 : 3);
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeShort(width);
			color.encode(coder, context);
		}
	}

}