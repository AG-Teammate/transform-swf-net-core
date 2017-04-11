using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;
using com.flagstone.transform.shape;

/*
 * DefineFont.java
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

namespace com.flagstone.transform.font
{
    /// <summary>
	/// DefineFont defines the glyphs that are drawn when text characters are
	/// rendered in a particular font.
	/// 
	/// <para>
	/// A complete definition of a font is created using the DefineFont object for
	/// the glyphs along with an FontInfo or FontInfo2 object which contains the name
	/// of the font, whether the font face is bold or italics and a table that maps
	/// character codes to the glyphs that is drawn to represent the character.
	/// </para> </summary>
	/// <seealso cref= FontInfo </seealso>
	/// <seealso cref= FontInfo2 </seealso>
	public sealed class DefineFont : DefineTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineFont: { identifier=%d;" + " shapes=%s}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The set of glyphs for this font. </summary>
		private IList<Shape> shapes;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// The table of offsets to each glyph. </summary>
		
		private int[] table;


		/// <summary>
		/// Creates and initialises a DefineFont object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public DefineFont(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			shapes = new List<Shape>();



			int first = coder.readUnsignedShort();


			int count = first >> 1;
			table = new int[count + 1];

			table[0] = first;
			for (int i = 1; i < count; i++)
			{
				table[i] = coder.readUnsignedShort();
			}

			table[count] = length - 2;

			Shape shape;
			for (int i = 0; i < count; i++)
			{
				shape = new Shape();
				shape.add(new ShapeData(table[i + 1] - table[i], coder));
				shapes.Add(shape);
			}
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a DefineFont object setting the unique identifier for the object
		/// and the list of glyphs used to render the characters used from the font.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for this object. </param>
		/// <param name="list">
		///            a list of Shape objects that define the outlines for each
		///            glyph in the font. </param>


		public DefineFont(int uid, IList<Shape> list)
		{
			Identifier = uid;
			Shapes = list;
		}

		/// <summary>
		/// Creates and initialises a DefineFont object using the values copied
		/// from another DefineFont object.
		/// </summary>
		/// <param name="object">
		///            a DefineFont object from which the values will be
		///            copied. </param>


		public DefineFont(DefineFont @object)
		{
			identifier = @object.identifier;
			shapes = new List<Shape>(@object.shapes.Count);
			foreach (Shape shape in @object.shapes)
			{
				shapes.Add(shape.copy());
			}
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
		/// Add a shape to the list of shapes that represent the glyphs for the
		/// font.
		/// </summary>
		/// <param name="obj">
		///            a shape which must not be null. </param>
		/// <returns> this object. </returns>


		public DefineFont add(Shape obj)
		{
			if (obj == null)
			{
				throw new ArgumentException();
			}
			shapes.Add(obj);
			return this;
		}

		/// <summary>
		/// Get the list of shapes that define the outline for each glyph.
		/// </summary>
		/// <returns> the glyphs for this font. </returns>
		public IList<Shape> Shapes
		{
			get => shapes;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				shapes = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public DefineFont copy()
		{
			return new DefineFont(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, shapes);
		}


		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 2;

			context.put(Context.FILL_SIZE, 1);
			context.put(Context.LINE_SIZE, context.contains(Context.POSTSCRIPT) ? 1 : 0);



			int count = shapes.Count;
			int index = 0;
			int shapeLength;
			int tableEntry = count << 1;
			table = new int[count + 1];

			length += count << 1;

			table[index++] = tableEntry;

			foreach (Shape shape in shapes)
			{
				shapeLength = shape.prepareToEncode(context);
				tableEntry += shapeLength;
				table[index++] = tableEntry;
				length += shapeLength;
			}

			context.put(Context.FILL_SIZE, 0);
			context.put(Context.LINE_SIZE, 0);

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}


		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_FONT << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_FONT << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);

			context.put(Context.FILL_SIZE, 1);
			context.put(Context.LINE_SIZE, context.contains(Context.POSTSCRIPT) ? 1 : 0);

			for (int i = 0; i < table.Length - 1; i++)
			{
				coder.writeShort(table[i]);
			}

			foreach (Shape shape in shapes)
			{
				shape.encode(coder, context);
			}

			context.put(Context.FILL_SIZE, 0);
			context.put(Context.LINE_SIZE, 0);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}