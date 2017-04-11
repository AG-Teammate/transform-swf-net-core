using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;

/*
 * DefineText.java
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

namespace com.flagstone.transform.text
{
    /// <summary>
	/// DefineText defines one or more lines of text.
	/// 
	/// <para>
	/// The characters, style and layout information is defined using {@link
	/// TextSpan} objects. The DefineText class acts as a container for the text,
	/// defining the bounding rectangle that encloses the text along with a
	/// coordinate transform that can be used to change the size and orientation of
	/// the text when it is displayed.
	/// </para>
	/// 
	/// <para>
	/// The bounding rectangle and transform controls how the text is laid out. Each
	/// Text object in the list of TextSpans specifies an offset from the left and
	/// bottom edges of the bounding rectangle, allowing successive lines of text to
	/// be arranged as a block or paragraph. The coordinate transform can be used to
	/// control the size and orientation of the text when it is displayed.
	/// </para>
	/// </summary>
	/// <seealso cref= TextSpan </seealso>
	public sealed class DefineText : StaticTextTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineText: { identifier=%d;" + " bounds=%s; transform=%s; objects=%s}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The bounding box that encloses the text. </summary>
		private Bounds bounds;
		/// <summary>
		/// The position and orientation of the text. </summary>
		private CoordTransform transform;
		/// <summary>
		/// The segments of text. </summary>
		private IList<TextSpan> spans;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// Number of bits used to encode each glyph index. </summary>
		
		private int glyphBits;
		/// <summary>
		/// Number of bits used to encode each advance. </summary>
		
		private int advanceBits;

		/// <summary>
		/// Creates and initialises a DefineText object using values encoded
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




		public DefineText(SWFDecoder coder, Context context)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			bounds = new Bounds(coder);

			/*
			 * This code is used to get round a bug in Flash - sometimes 16, 8-bit
			 * zeroes are written out before the transform. The root cause in Flash
			 * is unknown but seems to be related to the bounds not being set - all
			 * values are zero.
			 */

			coder.fill();
			coder.mark();
			int sum = 0;
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			for (int i = 0; i < 16; i++)
			{
				sum += coder.readByte();
			}
			if (sum != 0)
			{
				coder.reset();
			}
			coder.unmark();

			transform = new CoordTransform(coder);

			glyphBits = coder.readByte();
			advanceBits = coder.readByte();

			context.put(Context.GLYPH_SIZE, glyphBits);
			context.put(Context.ADVANCE_SIZE, advanceBits);

			spans = new List<TextSpan>();

			while (coder.scanByte() != 0)
			{
				spans.Add(new TextSpan(coder, context));
			}

			coder.readByte();

			context.put(Context.GLYPH_SIZE, 0);
			context.put(Context.ADVANCE_SIZE, 0);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a DefineText object with the specified bounding rectangle,
		/// coordinate transform and text records.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for this object. Must be in the range
		///            1..65535 </param>
		/// <param name="rect">
		///            the bounding rectangle enclosing the text. Must not be null. </param>
		/// <param name="matrix">
		///            an CoordTransform to change the size and orientation of the
		///            text. Must not be null. </param>
		/// <param name="list">
		///            a list of Text objects that define the text to be displayed.
		///            Must not be null. </param>


		public DefineText(int uid, Bounds rect, CoordTransform matrix, IList<TextSpan> list)
		{
			Identifier = uid;
			Bounds = rect;
			Transform = matrix;
			Spans = list;
		}

		/// <summary>
		/// Creates and initialises a DefineText object using the values copied
		/// from another DefineText object.
		/// </summary>
		/// <param name="object">
		///            a DefineText object from which the values will be
		///            copied. </param>


		public DefineText(DefineText @object)
		{
			identifier = @object.identifier;
			bounds = @object.bounds;
			transform = @object.transform;
			spans = new List<TextSpan>(@object.spans.Count);
			foreach (TextSpan span in @object.spans)
			{
				spans.Add(span.copy());
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
		/// Add a TextSpan object to the list of text spans.
		/// </summary>
		/// <param name="obj">
		///            an TextSpan object. Must not be null. </param>
		/// <returns> this object. </returns>


		public DefineText add(TextSpan obj)
		{
			if (obj == null)
			{
				throw new ArgumentException();
			}
			spans.Add(obj);
			return this;
		}

		/// <summary>
		/// Get the bounding rectangle that completely encloses the text to be
		/// displayed.
		/// </summary>
		/// <returns> the bounding box that encloses the text. </returns>
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
		/// Get the coordinate transform that controls the size, location and
		/// orientation of the text when it is displayed.
		/// </summary>
		/// <returns> the coordinate transform used to position the text. </returns>
		public CoordTransform Transform
		{
			get => transform;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				transform = value;
			}
		}

		/// <summary>
		/// Get the list of text spans that define the text to be displayed.
		/// </summary>
		/// <returns> the list of text blocks. </returns>
		public IList<TextSpan> Spans
		{
			get => spans;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				spans = value;
			}
		}




		/// <summary>
		/// {@inheritDoc} </summary>
		public DefineText copy()
		{
			return new DefineText(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, bounds, transform, spans);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			glyphBits = calculateSizeForGlyphs();
			advanceBits = calculateSizeForAdvances();

			context.put(Context.GLYPH_SIZE, glyphBits);
			context.put(Context.ADVANCE_SIZE, advanceBits);

			length = 2 + bounds.prepareToEncode(context);
			length += transform.prepareToEncode(context);
			length += 2;

			foreach (TextSpan span in spans)
			{
				length += span.prepareToEncode(context);
			}

			length += 1;

			context.put(Context.GLYPH_SIZE, 0);
			context.put(Context.ADVANCE_SIZE, 0);

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}


		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_TEXT << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_TEXT << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);

			context.put(Context.GLYPH_SIZE, glyphBits);
			context.put(Context.ADVANCE_SIZE, advanceBits);

			bounds.encode(coder, context);
			transform.encode(coder, context);

			coder.writeByte(glyphBits);
			coder.writeByte(advanceBits);

			foreach (TextSpan span in spans)
			{
				span.encode(coder, context);
			}

			coder.writeByte(0);

			context.put(Context.GLYPH_SIZE, 0);
			context.put(Context.ADVANCE_SIZE, 0);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}

		/// <summary>
		/// The number of bits used to encode the glyph indices. </summary>
		/// <returns> the number of bits used to encode each glyph index. </returns>
		private int calculateSizeForGlyphs()
		{
			int total = 0;
			int size;

			foreach (TextSpan span in spans)
			{
				size = span.glyphBits();

				if (size > total)
				{
					total = size;
				}
			}

			return total;
		}

		/// <summary>
		/// The number of bits used to encode the advances. </summary>
		/// <returns> the number of bits used to encode each advance. </returns>
		private int calculateSizeForAdvances()
		{
			int total = 0;
			int size;

			foreach (TextSpan span in spans)
			{
				size = span.advanceBits();

				if (size > total)
				{
					total = size;
				}
			}

			return total;
		}
	}

}