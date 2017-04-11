using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;
using com.flagstone.transform.fillstyle;
using com.flagstone.transform.linestyle;

/*
 * DefineShape4.java
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

namespace com.flagstone.transform.shape
{
    /// <summary>
	/// DefineShape4 extends DefienShape3 by specifying the bounding box for the
	/// edges of the shape (the outline without taking the stroke thickness into
	/// account) as well as hints for improving the way shapes are drawn.
	/// </summary>
	public sealed class DefineShape4 : ShapeTag
	{

		/// <summary>
		/// Reserved length for style counts indicated that the number of line
		/// or fill styles is encoded in the next 16-bit word.
		/// </summary>
		private const int EXTENDED = 255;
		/// <summary>
		/// Number of bytes used to encode the number of styles. </summary>
		private const int EXTENDED_LENGTH = 3;

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineShape4: { identifier=%d;" + " shapeBounds=%s; edgeBounds=%s; fillStyles=%s; lineStyles=%s;" + " winding=%b; shape=%s}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The bounding box for the shape at the start of the morph. </summary>
		private Bounds bounds;
		/// <summary>
		/// The bounding box for the shape, excluding line widths at the start. </summary>
		private Bounds edgeBounds;
		/// <summary>
		/// The list of fill styles for the shape. </summary>
		private IList<FillStyle> fillStyles;
		/// <summary>
		/// The list of line styles for the shape. </summary>
		private IList<LineStyle> lineStyles;
		/// <summary>
		/// The shape. </summary>
		private Shape shape;

		/// <summary>
		/// Indicates whether fill winding is used. </summary>
		
		private int winding;
		/// <summary>
		/// Indicates whether any of the line styles contain scaling strokes. </summary>
		
		private int scaling;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// The number of bits to encode indices into the fill style list. </summary>
		
		private int fillBits;
		/// <summary>
		/// The number of bits to encode indices into the line style list. </summary>
		
		private int lineBits;

		/// <summary>
		/// Creates and initialises a DefineShape4 object using values encoded
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



		public DefineShape4(SWFDecoder coder, Context context)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			context.put(Context.TRANSPARENT, 1);
			context.put(Context.TYPE, MovieTypes.DEFINE_SHAPE_4);

			bounds = new Bounds(coder);
			edgeBounds = new Bounds(coder);

			// scaling hints are implied by the line styles used
			winding = coder.readByte() & Coder.BIT2;

			int fillStyleCount = coder.readByte();

			if (fillStyleCount == EXTENDED)
			{
				fillStyleCount = coder.readUnsignedShort();
			}
			fillStyles = new List<FillStyle>();
			lineStyles = new List<LineStyle>();



			SWFFactory<FillStyle> decoder = context.Registry.FillStyleDecoder;

			for (int i = 0; i < fillStyleCount; i++)
			{
				decoder.getObject(fillStyles, coder, context);
			}

			int lineStyleCount = coder.readByte();

			if (lineStyleCount == EXTENDED)
			{
				lineStyleCount = coder.readUnsignedShort();
			}

			for (int i = 0; i < lineStyleCount; i++)
			{
				lineStyles.Add(new LineStyle2(coder, context));
			}

			context.put(Context.ARRAY_EXTENDED, 1);

			if (context.Registry.ShapeDecoder == null)
			{
				shape = new Shape();
				shape.add(new ShapeData(length - coder.bytesRead(), coder));
			}
			else
			{
				shape = new Shape(coder, context);
			}

			context.remove(Context.TRANSPARENT);
			context.remove(Context.ARRAY_EXTENDED);
			context.remove(Context.TYPE);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a DefineShape3 object.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for the shape in the range 1..65535. </param>
		/// <param name="rect">
		///            the bounding rectangle for the shape including the width of
		///            the border lines. Must not be null. </param>
		/// <param name="edges">
		///            the bounding rectangle for the shape excluding the line used
		///            to draw the border. Must not be null. </param>
		/// <param name="fills">
		///            the list of fill styles used in the shape. Must not be null. </param>
		/// <param name="lines">
		///            the list of line styles used in the shape. Must not be null. </param>
		/// <param name="aShape">
		///            the shape to be drawn. Must not be null. </param>


		public DefineShape4(int uid, Bounds rect, Bounds edges, IList<FillStyle> fills, IList<LineStyle> lines, Shape aShape)
		{
			Identifier = uid;
			Bounds = rect;
			EdgeBounds = edges;
			FillStyles = fills;
			LineStyles = lines;
			Shape = aShape;
		}

		/// <summary>
		/// Creates and initialises a DefineShape4 object using the values copied
		/// from another DefineShape4 object.
		/// </summary>
		/// <param name="object">
		///            a DefineShape4 object from which the values will be
		///            copied. </param>


		public DefineShape4(DefineShape4 @object)
		{
			identifier = @object.identifier;
			bounds = @object.bounds;
			edgeBounds = @object.edgeBounds;
			fillStyles = new List<FillStyle>(@object.fillStyles.Count);
            foreach (FillStyle style in @object.fillStyles)
            {
                fillStyles.Add(style.copy());
            }
            lineStyles = new List<LineStyle>(@object.lineStyles.Count);
            foreach (LineStyle style in @object.lineStyles)
            {
                lineStyles.Add(style.copy());
            }
            shape = @object.shape.copy();
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
		/// Get the bounding rectangle that completely enclosed the shape.
		/// </summary>
		/// <returns> the Bounds that encloses the shape </returns>
		public Bounds Bounds
		{
			get => bounds;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				bounds = value;
			}
		}


		/// <summary>
		/// Get the bounding rectangle that covers the outline of the shape.
		/// </summary>
		/// <returns> the Bounds that encloses the shape outline, excluding any
		/// lines drawn. </returns>
		public Bounds EdgeBounds
		{
			get => edgeBounds;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				edgeBounds = value;
			}
		}


		/// <summary>
		/// Add a LineStyle to the list of line styles.
		/// </summary>
		/// <param name="style">
		///            a LineStyle2 object. Must not be null or an instance of
		///            LineStyle1.
		/// </param>
		/// <returns> this object. </returns>


		public ShapeTag add(LineStyle style)
		{
			if (style == null || style is LineStyle1)
			{
				throw new ArgumentException();
			}
			lineStyles.Add(style);
			return this;
		}

		/// <summary>
		/// Add the fill style to the list of fill styles.
		/// </summary>
		/// <param name="style">
		///            and FillStyle object. Must not be null.
		/// </param>
		/// <returns> this object. </returns>


		public ShapeTag add(FillStyle style)
		{
			if (style == null)
			{
				throw new ArgumentException();
			}
			fillStyles.Add(style);
			return this;
		}

		/// <summary>
		/// Get the list fill styles.
		/// </summary>
		/// <returns> the list of fill styles used in the shape. </returns>
		public IList<FillStyle> FillStyles
		{
			get => fillStyles;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				fillStyles = value;
			}
		}

		/// <summary>
		/// Get the list line styles.
		/// </summary>
		/// <returns> the list of line styles used in the shape. </returns>
		public IList<LineStyle> LineStyles
		{
			get => lineStyles;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				lineStyles = value;
			}
		}

		/// <summary>
		/// Get the shape.
		/// </summary>
		/// <returns> the shape. </returns>
		public Shape Shape
		{
			get => shape;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				shape = value;
			}
		}




		/// <summary>
		/// Does the shape use fill winding. </summary>
		/// <returns> true if fill winding is used, false otherwise. </returns>
		public bool useWinding()
		{
			return winding != 0;
		}

		/// <summary>
		/// Indicates whether the shape uses fill winding. </summary>
		/// <param name="use"> true if fill winding is used, false otherwise. </param>


		public bool Winding
		{
			set
			{
				if (value)
				{
					winding = Coder.BIT2;
				}
				else
				{
					winding = 0;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public DefineShape4 copy()
		{
			return new DefineShape4(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, bounds, edgeBounds, fillStyles, lineStyles, useWinding(), shape);
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public int prepareToEncode(Context context)
		{
			fillBits = Coder.unsignedSize(fillStyles.Count);
			lineBits = Coder.unsignedSize(lineStyles.Count);

			if (context.contains(Context.POSTSCRIPT))
			{
				if (fillBits == 0)
				{
					fillBits = 1;
				}

				if (lineBits == 0)
				{
					lineBits = 1;
				}
			}

			context.put(Context.TRANSPARENT, 1);
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			length = 3;
			length += bounds.prepareToEncode(context);
			length += edgeBounds.prepareToEncode(context);

			length += (fillStyles.Count >= EXTENDED) ? EXTENDED_LENGTH : 1;

			foreach (FillStyle style in fillStyles)
			{
				length += style.prepareToEncode(context);
			}

			context.put(Context.SCALING_STROKE, 0);

			length += (lineStyles.Count >= EXTENDED) ? EXTENDED_LENGTH : 1;

			foreach (LineStyle style in lineStyles)
			{
				length += style.prepareToEncode(context);
			}

			scaling = context.get(Context.SCALING_STROKE);

			context.put(Context.ARRAY_EXTENDED, 1);
			context.put(Context.FILL_SIZE, fillBits);
			context.put(Context.LINE_SIZE, lineBits);

			length += shape.prepareToEncode(context);

			context.remove(Context.ARRAY_EXTENDED);
			context.put(Context.FILL_SIZE, 0);
			context.put(Context.LINE_SIZE, 0);
			context.remove(Context.TRANSPARENT);
			context.remove(Context.SCALING_STROKE);

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			context.put(Context.TRANSPARENT, 1);

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_SHAPE_4 << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_SHAPE_4 << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);

			bounds.encode(coder, context);
			edgeBounds.encode(coder, context);

			coder.writeByte(winding | scaling);

			if (fillStyles.Count >= EXTENDED)
			{
				coder.writeByte(EXTENDED);
				coder.writeShort(fillStyles.Count);
			}
			else
			{
				coder.writeByte(fillStyles.Count);
			}

			foreach (FillStyle style in fillStyles)
			{
				style.encode(coder, context);
			}

			if (lineStyles.Count >= EXTENDED)
			{
				coder.writeByte(EXTENDED);
				coder.writeShort(lineStyles.Count);
			}
			else
			{
				coder.writeByte(lineStyles.Count);
			}

			foreach (LineStyle style in lineStyles)
			{
				style.encode(coder, context);
			}

			context.put(Context.ARRAY_EXTENDED, 1);
			context.put(Context.FILL_SIZE, fillBits);
			context.put(Context.LINE_SIZE, lineBits);

			shape.encode(coder, context);

			context.remove(Context.ARRAY_EXTENDED);
			context.put(Context.FILL_SIZE, 0);
			context.put(Context.LINE_SIZE, 0);
			context.remove(Context.TRANSPARENT);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}