using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;
using com.flagstone.transform.fillstyle;

/*
 * LineStyle2.java
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

namespace com.flagstone.transform.linestyle
{
    /// <summary>
	/// LineStyle2 extends LineStyle1 by supporting different styles for line joins
	/// and line ends, a fill style for the stroke and whether the stroke thickness
	/// is scaled if an object is resized.
	/// </summary>


	public sealed class LineStyle2 : LineStyle
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "LineStyle2: { width=%d; color=%s;" + " fillStyle=%s; startCap=%s; endCap=%s; joinStyle=%s;" + " scaledHorizontally=%b; scaledVertically=%b;" + " pixelAligned=%b; lineClosed=%b; miterLimit=%d}";

		/// <summary>
		/// Width of the line in twips. </summary>
		private int width;
		/// <summary>
		/// Colour used to draw the line. </summary>
		private Color color;

		/// <summary>
		/// Code for the cap style used for the start of the line. </summary>
		private int startCap;
		/// <summary>
		/// Code for the cap style used for the end of the line. </summary>
		private int endCap;
		/// <summary>
		/// Code for the style used to join two line together. </summary>
		private int joinStyle;
		/// <summary>
		/// Fill style used to draw the stroke. </summary>
		private FillStyle fillStyle;

		/// <summary>
		/// Does the line allow scaling horizontally. </summary>
		private bool horizontal;
		/// <summary>
		/// Does the line allow scaling vertically. </summary>
		private bool vertical;
		/// <summary>
		/// Is the line drawn along pixel boundaries. </summary>
		private bool pixelAligned;
		/// <summary>
		/// Should the line be closed if the start and end points coincide. </summary>
		private bool lineClosed;
		/// <summary>
		/// Parameter controlling the mitering when joining two lines. </summary>
		private int miterLimit;

		/// <summary>
		/// Indicates the style contains a fill style. </summary>
		
		private bool hasFillStyle;
		/// <summary>
		/// Indicates the style contains a mitering limit. </summary>
		
		private bool hasMiter;

		/// <summary>
		/// Creates and initialises a LineStyle2 object using values encoded
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



		public LineStyle2(SWFDecoder coder, Context context)
		{

			width = coder.readUnsignedShort();

			int bits = coder.readByte();
			if ((bits & Coder.BIT6) > 0)
			{
				startCap = 1;
			}
			else if ((bits & Coder.BIT7) > 0)
			{
				startCap = 2;
			}
			else
			{
				startCap = 0;
			}

			if ((bits & Coder.BIT4) > 0)
			{
				joinStyle = 1;
				hasMiter = false;
			}
			else if ((bits & Coder.BIT5) > 0)
			{
				joinStyle = 2;
				hasMiter = true;
			}
			else
			{
				joinStyle = 0;
				hasMiter = false;
			}

			hasFillStyle = (bits & Coder.BIT3) != 0;
			horizontal = (bits & Coder.BIT2) == 0;
			vertical = (bits & Coder.BIT1) == 0;
			pixelAligned = (bits & Coder.BIT0) != 0;

			bits = coder.readByte();
			lineClosed = (bits & Coder.BIT2) == 0;
			endCap = bits & Coder.PAIR0;

			if (hasMiter)
			{
				coder.readUnsignedShort();
			}

			if (hasFillStyle)
			{


				SWFFactory<FillStyle> decoder = context.Registry.FillStyleDecoder;


				IList<FillStyle> styles = new List<FillStyle>();
				decoder.getObject(styles, coder, context);
				fillStyle = styles[0];
			}
			else
			{
				color = new Color(coder, context);
			}
		}

		/// <summary>
		/// Create a new LineStyle2 object with the stroke thickness and color. </summary>
		/// <param name="lineWidth"> the width of the line. </param>
		/// <param name="lineColor"> the colour used to draw the line. </param>


		public LineStyle2(int lineWidth, Color lineColor)
		{

			Width = lineWidth;
			Color = lineColor;

			vertical = true;
			vertical = true;
			lineClosed = true;
		}

		/// <summary>
		/// Create a new LineStyle2 object with the stroke thickness and fill style. </summary>
		/// <param name="lineWidth"> the width of the line. </param>
		/// <param name="style"> the fill style used to draw the line. </param>


		public LineStyle2(int lineWidth, FillStyle style)
		{

			Width = lineWidth;
			FillStyle = style;

			vertical = true;
			vertical = true;
			lineClosed = true;
		}

		 /// <summary>
		 /// Creates and initialises a LineStyle2 object using the values copied
		 /// from another LineStyle2 object.
		 /// </summary>
		 /// <param name="object">
		 ///            a LineStyle2 object from which the values will be
		 ///            copied. </param>


		public LineStyle2(LineStyle2 @object)
		{
			width = @object.width;
			color = @object.color;

			if (@object.fillStyle != null)
			{
				fillStyle = @object.fillStyle.copy();
			}

			startCap = @object.startCap;
			endCap = @object.endCap;
			joinStyle = @object.joinStyle;

			horizontal = @object.horizontal;
			vertical = @object.vertical;
			pixelAligned = @object.pixelAligned;
			lineClosed = @object.lineClosed;
			miterLimit = @object.miterLimit;
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
		/// Get the CapStyle used for the start of the line. </summary>
		/// <returns> the CapStyle that specifies how the start of the line is drawn. </returns>
		public CapStyle StartCap
		{
			get
			{
				CapStyle style;
				if (startCap == 1)
				{
					style = CapStyle.NONE;
				}
				else if (startCap == 2)
				{
					style = CapStyle.SQUARE;
				}
				else
				{
					style = CapStyle.ROUND;
				}
				return style;
			}
			set
			{
				switch (value)
				{
				case CapStyle.NONE:
					startCap = 1;
					break;
				case CapStyle.SQUARE:
					startCap = 2;
					break;
				default:
					startCap = 0;
					break;
				}
			}
		}


		/// <summary>
		/// Get the CapStyle used for the end of the line. </summary>
		/// <returns> the CapStyle that specifies how the end of the line is drawn. </returns>
	   public CapStyle EndCap
	   {
		   get
		   {
				CapStyle style;
				if (endCap == 1)
				{
					style = CapStyle.NONE;
				}
				else if (endCap == 2)
				{
					style = CapStyle.SQUARE;
				}
				else
				{
					style = CapStyle.ROUND;
				}
				return style;
		   }
		   set
		   {
				switch (value)
				{
				case CapStyle.NONE:
					endCap = 1;
					break;
				case CapStyle.SQUARE:
					endCap = 2;
					break;
				default:
					endCap = 0;
					break;
				}
		   }
	   }


		/// <summary>
		/// Get the JoinStyle used when joining with another line or curve. </summary>
		/// <returns> the JoinStyle used to connect with another line or curve. </returns>
		public JoinStyle JoinStyle
		{
			get
			{
				JoinStyle style;
				if (endCap == 1)
				{
					style = JoinStyle.BEVEL;
				}
				else if (endCap == 2)
				{
					style = JoinStyle.MITER;
				}
				else
				{
					style = JoinStyle.ROUND;
				}
				return style;
			}
			set
			{
				switch (value)
				{
				case JoinStyle.BEVEL:
					joinStyle = 1;
					break;
				case JoinStyle.MITER:
					joinStyle = 2;
					break;
				default:
					joinStyle = 0;
					break;
				}
			}
		}


		/// <summary>
		/// Is the stroke scaled horizontally if the shape is redrawn. </summary>
		/// <returns> true if the stroke is scaled horizontally, false if the stroke
		/// thickness does not change. </returns>
		public bool Horizontal
		{
			get => horizontal;
		    set => horizontal = value;
		}


		/// <summary>
		/// Is the stroke scaled vertically if the shape is redrawn. </summary>
		/// <returns> true if the stroke is scaled vertically, false if the stroke
		/// thickness does not change. </returns>
		public bool Vertical
		{
			get => vertical;
		    set => vertical = value;
		}


		/// <summary>
		/// Are the end points of the line aligned to pixel boundaries. </summary>
		/// <returns> true if the end points are aligned to full pixels, false
		/// otherwise. </returns>
		public bool PixelAligned
		{
			get => pixelAligned;
		    set => pixelAligned = value;
		}


		/// <summary>
		/// Is the path closed if the end point matches the starting point. If true
		/// then the line will be joined, otherwise an end cap is drawn. </summary>
		/// <returns> true if the line will be closed, false if the path remains open. </returns>
		public bool LineClosed
		{
			get => lineClosed;
		    set => lineClosed = value;
		}


		/// <summary>
		/// Get the limit for drawing miter joins. </summary>
		/// <returns> the value controlling how miter joins are drawn. </returns>
		public int MiterLimit
		{
			get => miterLimit;
		    set
			{
				if ((value < 0) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value);
				}
				miterLimit = value;
			}
		}


		/// <summary>
		/// Get the FillStyle used for the line stroke. </summary>
		/// <returns> the FillStyle used to draw the line. </returns>
		public FillStyle FillStyle
		{
			get => fillStyle;
		    set => fillStyle = value;
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public LineStyle2 copy()
		{
			return new LineStyle2(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, width, color, fillStyle, startCap, endCap, joinStyle, horizontal, vertical, pixelAligned, lineClosed, miterLimit);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			hasFillStyle = fillStyle != null;
			hasMiter = joinStyle == 2;

			int length = 4;

			if (hasMiter)
			{
				length += 2;
			}

			if (hasFillStyle)
			{
				length += fillStyle.prepareToEncode(context);
			}
			else
			{
				length += 4;
			}

			int scaling = context.get(Context.SCALING_STROKE);

			if (horizontal || vertical)
			{
				scaling |= Coder.BIT1;
			}
			else
			{
				scaling |= Coder.BIT0;
			}
			context.put(Context.SCALING_STROKE, scaling);

			return length;
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>




		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeShort(width);

			int value = 0;

			if (startCap == 1)
			{
				value |= Coder.BIT6;
			}
			else if (startCap == 2)
			{
				value |= Coder.BIT7;
			}

			if (joinStyle == 1)
			{
				value |= Coder.BIT4;
			}
			else if (joinStyle == 2)
			{
				value |= Coder.BIT5;
			}

			value |= fillStyle == null ? 0 : Coder.BIT3;
			value |= horizontal ? 0 : Coder.BIT2;
			value |= vertical ? 0 : Coder.BIT1;
			value |= pixelAligned ? Coder.BIT0 : 0;

			coder.writeByte(value);

			value = lineClosed ? 0 : Coder.BIT2;
			value |= endCap;
			coder.writeByte(value);

			if (hasMiter)
			{
				coder.writeShort(miterLimit);
			}

			if (hasFillStyle)
			{
				fillStyle.encode(coder, context);
			}
			else
			{
				color.encode(coder, context);
			}
		}
	}

}