using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;
using com.flagstone.transform.fillstyle;
using com.flagstone.transform.linestyle;

/*
 * DefineShape3.java
 * Transform
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
	/// DefineShape3 defines a transparent shape to be displayed. It extends the
	/// <seealso cref="DefineShape2"/> class by encoding the alpha channel in any Color objects
	/// included in the line and fill styles.
	/// </summary>
	public sealed class DefineShape3 : ShapeTag
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
		private const string FORMAT = "DefineShape3: { identifier=%d;" + " bounds=%s; fillStyles=%s; lineStyles=%s; shape=%s}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The bounding box that encloses the shape. </summary>
		private Bounds bounds;
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
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// The number of bits to encode indices into the fill style list. </summary>
		
		private int fillBits;
		/// <summary>
		/// The number of bits to encode indices into the line style list. </summary>
		
		private int lineBits;

		/// <summary>
		/// Creates and initialises a DefineShape3 object using values encoded
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



		public DefineShape3(SWFDecoder coder, Context context)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			context.put(Context.TRANSPARENT, 1);
			context.put(Context.TYPE, MovieTypes.DEFINE_SHAPE_3);

			bounds = new Bounds(coder);

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
				lineStyles.Add(new LineStyle1(coder, context));
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
		///            the bounding rectangle for the shape. Must not be null. </param>
		/// <param name="fills">
		///            the list of fill styles used in the shape. Must not be null. </param>
		/// <param name="lines">
		///            the list of line styles used in the shape. Must not be null. </param>
		/// <param name="aShape">
		///            the shape to be drawn. Must not be null. </param>


		public DefineShape3(int uid, Bounds rect, IList<FillStyle> fills, IList<LineStyle> lines, Shape aShape)
		{
			Identifier = uid;
			Bounds = rect;
			FillStyles = fills;
			LineStyles = lines;
			Shape = aShape;
		}

		/// <summary>
		/// Creates and initialises a DefineShape3 object using the values copied
		/// from another DefineShape3 object.
		/// </summary>
		/// <param name="object">
		///            a DefineShape3 object from which the values will be
		///            copied. </param>


		public DefineShape3(DefineShape3 @object)
		{
			identifier = @object.identifier;
			bounds = @object.bounds;
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
		/// Add a LineStyle1 to the list of line styles.
		/// </summary>
		/// <param name="style">
		///            and LineStyle1 object. Must not be null or an instance of
		///            LineStyle2.
		/// </param>
		/// <returns> this object. </returns>


		public ShapeTag add(LineStyle style)
		{
			if (style == null || style is LineStyle2)
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
		/// Get the bounding rectangle for the shape.
		/// </summary>
		/// <returns> the Bounds that encloses the shape. </returns>
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
		/// {@inheritDoc} </summary>
		public DefineShape3 copy()
		{
			return new DefineShape3(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, bounds, fillStyles, lineStyles, shape);
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

			length = 2 + bounds.prepareToEncode(context);

			length += (fillStyles.Count >= EXTENDED) ? EXTENDED_LENGTH : 1;

			foreach (FillStyle style in fillStyles)
			{
				length += style.prepareToEncode(context);
			}

			length += (lineStyles.Count >= EXTENDED) ? EXTENDED_LENGTH : 1;

			foreach (LineStyle style in lineStyles)
			{
				length += style.prepareToEncode(context);
			}

			context.put(Context.ARRAY_EXTENDED, 1);
			context.put(Context.FILL_SIZE, fillBits);
			context.put(Context.LINE_SIZE, lineBits);

			length += shape.prepareToEncode(context);

			context.remove(Context.ARRAY_EXTENDED);
			context.put(Context.FILL_SIZE, 0);
			context.put(Context.LINE_SIZE, 0);
			context.remove(Context.TRANSPARENT);

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_SHAPE_3 << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_SHAPE_3 << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			context.put(Context.TRANSPARENT, 1);

			bounds.encode(coder, context);

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