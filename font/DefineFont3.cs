using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;
using com.flagstone.transform.shape;
using com.flagstone.transform.text;

/*
 * DefineFont2.java
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
	/// DefineFont3 provides the same functionality as DefineFont2 but the
	/// coordinates are expressed at 20 times the resolution, i.e. 1/20th of a twip
	/// to increase the quality of the displayed text.
	/// 
	/// DefineFont3 is used in conjunction with FontAlignment zones to support
	/// snapping glyphs to the nearest pixel again to improve display quality.
	/// </summary>


	public sealed class DefineFont3 : DefineTag
	{
		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineFont3: { identifier=%d;" + " encoding=%d; small=%b; italic=%b; bold=%b; language=%s;" + " name=%s; shapes=%s; codes=%s; ascent=%d; descent=%d;" + " leading=%d; advances=%s; bounds=%s; kernings=%s}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// Code for the character encoding used. </summary>
		private int encoding;
		/// <summary>
		/// Is the font small. </summary>
		private bool small;
		/// <summary>
		/// Is the font italicized. </summary>
		private bool italic;
		/// <summary>
		/// Is the font bold. </summary>
		private bool bold;
		/// <summary>
		/// Code representing the spoken language - used for line breaking. </summary>
		private int language;
		/// <summary>
		/// The font name. </summary>
		private string name;
		/// <summary>
		/// The list of font glyphs. </summary>
		private IList<Shape> shapes;
		/// <summary>
		/// The list of character codes that map to each glyph - ascending order. </summary>
		private IList<int?> codes;
		/// <summary>
		/// Height of the font above the baseline. </summary>
		private int ascent;
		/// <summary>
		/// Height of the font below the baseline. </summary>
		private int descent;
		/// <summary>
		/// Spacing between successive lines. </summary>
		private int leading;
		/// <summary>
		/// Advances for each glyph. </summary>
		private IList<int?> advances;
		/// <summary>
		/// Bounding boxes for each glyph. </summary>
		private IList<Bounds> bounds;
		/// <summary>
		/// Kernings for selected pairs of glyphs. </summary>
		private IList<Kerning> kernings;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// Table of offsets to each glyph when encoded. </summary>
		
		private int[] table;
		/// <summary>
		/// Whether offsets are 16-bit (false) or 32-bit (true). </summary>
		
		private bool wideOffsets;
		/// <summary>
		/// Whether character codes are 8-bit (false) or 16-bit (true). </summary>
		
		private bool wideCodes;

		/// <summary>
		/// Creates and initialises a DefineFont3 object using values encoded
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



		 public DefineFont3(SWFDecoder coder, Context context)
		 {
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			shapes = new List<Shape>();
			codes = new List<int?>();
			advances = new List<int?>();
			bounds = new List<Bounds>();
			kernings = new List<Kerning>();



			int bits = coder.readByte();


			bool containsLayout = (bits & Coder.BIT7) != 0;


			int format = (bits >> Coder.TO_LOWER_NIB) & Coder.LOWEST3;

			encoding = 0;

			if (format == 1)
			{
				encoding = 1;
			}
			else if (format == 2)
			{
				small = true;
				// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			}
			else if (format == 4)
			{
				encoding = 2;
			}

			wideOffsets = (bits & Coder.BIT3) != 0;
			wideCodes = (bits & Coder.BIT2) != 0;
			italic = (bits & Coder.BIT1) != 0;
			bold = (bits & Coder.BIT0) != 0;

			if (wideCodes)
			{
				context.put(Context.WIDE_CODES, 1);
			}

			language = coder.readByte();


			int nameLength = coder.readByte();
			name = coder.readString(nameLength);

			if (name.Length > 0)
			{
				while (name[name.Length - 1] == 0)
				{
					name = name.Substring(0, name.Length - 1);
				}
			}



			int glyphCount = coder.readUnsignedShort();


			int[] offset = new int[glyphCount + 1];

			if (wideOffsets)
			{
				for (int i = 0; i < glyphCount; i++)
				{
					offset[i] = coder.readInt();
				}
			}
			else
			{
				for (int i = 0; i < glyphCount; i++)
				{
					offset[i] = coder.readUnsignedShort();
				}
			}

			// A device font may omit the offset to the start of the glyphs
			// when no layout information is included.

			if (coder.bytesRead() < length)
			{
				if (wideOffsets)
				{
					offset[glyphCount] = coder.readInt();
				}
				else
				{
					offset[glyphCount] = coder.readUnsignedShort();
				}
			}

			Shape shape;

			for (int i = 0; i < glyphCount; i++)
			{
				shape = new Shape();
				shape.add(new ShapeData(offset[i + 1] - offset[i], coder));
				shapes.Add(shape);
			}

			if (wideCodes)
			{
				for (int i = 0; i < glyphCount; i++)
				{
					codes.Add(coder.readUnsignedShort());
				}
			}
			else
			{
				for (int i = 0; i < glyphCount; i++)
				{
					codes.Add(coder.readByte());
				}
			}

			if (containsLayout || coder.bytesRead() < length)
			{
				ascent = coder.readSignedShort();
				descent = coder.readSignedShort();
				leading = coder.readSignedShort();

				for (int i = 0; i < glyphCount; i++)
				{
					advances.Add(coder.readSignedShort());
				}

				for (int i = 0; i < glyphCount; i++)
				{
					bounds.Add(new Bounds(coder));
				}



				int kerningCount = coder.readUnsignedShort();

				for (int i = 0; i < kerningCount; i++)
				{
					kernings.Add(new Kerning(coder, context));
				}
			}

			context.remove(Context.WIDE_CODES);
			coder.check(length);
			coder.unmark();
		 }

		/// <summary>
		/// Creates a DefineFont2 object specifying only the name of the font.
		/// 
		/// If none of the remaining attributes are set the Flash Player will load
		/// the font from the system on which it is running or substitute a suitable
		/// font if the specified font cannot be found. This is particularly useful
		/// when defining fonts that will be used to display text in DefineTextField
		/// objects.
		/// 
		/// The font will be defined to use Unicode encoding. The flags which define
		/// the font's face will be set to false. The lists of glyphs which define
		/// the shapes and the code which map the character codes to a particular
		/// glyph will remain empty since the font is loaded from the system on which
		/// it is displayed.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for this font object. </param>
		/// <param name="fontName">
		///            the name of the font. </param>


		public DefineFont3(int uid, string fontName)
		{
			Identifier = uid;
			Name = fontName;

			encoding = 0;
			shapes = new List<Shape>();
			codes = new List<int?>();
			advances = new List<int?>();
			bounds = new List<Bounds>();
			kernings = new List<Kerning>();
		}

		/// <summary>
		/// Creates and initialises a DefineFont3 object using the values copied
		/// from another DefineFont3 object.
		/// </summary>
		/// <param name="object">
		///            a DefineFont3 object from which the values will be
		///            copied. </param>


		public DefineFont3(DefineFont3 @object)
		{
			identifier = @object.identifier;
			encoding = @object.encoding;
			small = @object.small;
			italic = @object.italic;
			bold = @object.bold;
			language = @object.language;
			name = @object.name;
			ascent = @object.ascent;
			descent = @object.descent;
			leading = @object.leading;
			shapes = new List<Shape>(@object.shapes.Count);
			foreach (Shape shape in @object.shapes)
			{
				shapes.Add(shape.copy());
			}
			codes = new List<int?>(@object.codes);
			advances = new List<int?>(@object.advances);
			bounds = new List<Bounds>(@object.bounds);
			kernings = new List<Kerning>(@object.kernings);
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
		/// Add a character code and the corresponding glyph that will be displayed.
		/// Character codes should be added to the font in ascending order.
		/// </summary>
		/// <param name="code">
		///            the character code. Must be in the range 0..65535. </param>
		/// <param name="obj">
		///            the shape that represents the glyph displayed for the
		///            character code. </param>
		/// <returns> this object. </returns>


		public DefineFont3 addGlyph(int code, Shape obj)
		{
			if ((code < 0) || (code > Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, code);
			}
			codes.Add(code);

			if (obj == null)
			{
				throw new ArgumentException();
			}
			shapes.Add(obj);

			return this;
		}

		/// <summary>
		/// Add an advance to the list of advances. The index position of the entry
		/// in the advance list is also used to identify the corresponding glyph and
		/// vice-versa.
		/// </summary>
		/// <param name="anAdvance">
		///            an advance for a glyph. Must be in the range -32768..32767. </param>
		/// <returns> this object. </returns>


		public DefineFont3 addAdvance(int anAdvance)
		{
			if ((anAdvance < Coder.SHORT_MIN) || (anAdvance > Coder.SHORT_MAX))
			{
				throw new IllegalArgumentRangeException(Coder.SHORT_MIN, Coder.SHORT_MAX, anAdvance);
			}
			advances.Add(anAdvance);
			return this;
		}

		/// <summary>
		/// Add a bounds object to the list of bounds for each glyph. The index
		/// position of the entry in the bounds list is also used to identify the
		/// corresponding glyph and vice-versa.
		/// </summary>
		/// <param name="rect">
		///            an Bounds. Must not be null. </param>
		/// <returns> this object. </returns>


		public DefineFont3 add(Bounds rect)
		{
			if (rect == null)
			{
				throw new ArgumentException();
			}
			bounds.Add(rect);
			return this;
		}

		/// <summary>
		/// Add a kerning object to the list of kernings for pairs of glyphs.
		/// </summary>
		/// <param name="anObject">
		///            an Kerning. Must not be null. </param>
		/// <returns> this object. </returns>


		public DefineFont3 add(Kerning anObject)
		{
			if (anObject == null)
			{
				throw new ArgumentException();
			}
			kernings.Add(anObject);
			return this;
		}

		/// <summary>
		/// Returns the encoding scheme used for characters rendered in the font,
		/// either ASCII, SJIS or UCS2.
		/// </summary>
		/// <returns> the encoding used for character codes. </returns>
		public CharacterFormat Encoding
		{
			get
			{
				CharacterFormat value;
				switch (encoding)
				{
				case 0:
					value = CharacterFormat.UCS2;
					break;
				case 1:
					value = CharacterFormat.ANSI;
					break;
				case 2:
					value = CharacterFormat.SJIS;
					break;
				default:
					throw new InvalidOperationException();
				}
				return value;
			}
			set
			{
				switch (value)
				{
				case CharacterFormat.UCS2:
					encoding = 0;
					break;
				case CharacterFormat.ANSI:
					encoding = 1;
					break;
				case CharacterFormat.SJIS:
					encoding = 2;
					break;
				default:
					throw new ArgumentException();
				}
			}
		}

		/// <summary>
		/// Does the font have a small point size. This is used only with a Unicode
		/// font encoding.
		/// </summary>
		/// <returns> a boolean indicating whether the font will be aligned on pixel
		///         boundaries. </returns>
		public bool Small
		{
			get => small;
		    set => small = value;
		}


		// End Flash 7

		/// <summary>
		/// Is the font italicised.
		/// </summary>
		/// <returns> a boolean indicating whether the font is rendered in italics. </returns>
		public bool Italic
		{
			get => italic;
		    set => italic = value;
		}

		/// <summary>
		/// Is the font bold.
		/// </summary>
		/// <returns> a boolean indicating whether the font is rendered in a bold face. </returns>
		public bool Bold
		{
			get => bold;
		    set => bold = value;
		}

		// Flash 6
		/// <summary>
		/// Returns the language code identifying the type of spoken language for the
		/// font.
		/// </summary>
		/// <returns> the Language used to determine how line-breaks are inserted
		///         into text rendered using the font. Returns NONE if the object was
		///         decoded from a movie contains Flash 5 or less. </returns>
		public Language Language
		{
			get => Language.fromInt(language);
		    set => language = value.Value;
		}


		// End Flash 6

		/// <summary>
		/// Returns the name of the font family.
		/// </summary>
		/// <returns> the name of the font. </returns>
		public string Name
		{
			get => name;
		    set
			{
				if (ReferenceEquals(value, null))
				{
					throw new ArgumentException();
				}
				name = value;
			}
		}

		/// <summary>
		/// Returns the list of shapes used to define the outlines of each font
		/// glyph.
		/// </summary>
		/// <returns> a list of Shape objects </returns>
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
		/// Returns the list of codes used to identify each glyph in the font. The
		/// ordinal position of each Integer representing a code identifies a
		/// particular glyph in the shapes list.
		/// </summary>
		/// <returns> a list of Integer objects that contain the character codes for
		///         each glyph in the font. </returns>
		public IList<int?> Codes
		{
			get => codes;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				codes = value;
			}
		}

		/// <summary>
		/// Returns the ascent for the font in twips.
		/// </summary>
		/// <returns> the ascent for the font. </returns>
		public int Ascent
		{
			get => ascent;
		    set
			{
				if ((value < Coder.SHORT_MIN) || (value > Coder.SHORT_MAX))
				{
					throw new IllegalArgumentRangeException(Coder.SHORT_MIN, Coder.SHORT_MAX, value);
				}
				ascent = value;
			}
		}

		/// <summary>
		/// Returns the descent for the font in twips.
		/// </summary>
		/// <returns> the descent for the font. </returns>
		public int Descent
		{
			get => descent;
		    set
			{
				if ((value < Coder.SHORT_MIN) || (value > Coder.SHORT_MAX))
				{
					throw new IllegalArgumentRangeException(Coder.SHORT_MIN, Coder.SHORT_MAX, value);
				}
				descent = value;
			}
		}

		/// <summary>
		/// Returns the leading for the font in twips.
		/// </summary>
		/// <returns> the leading for the font. </returns>
		public int Leading
		{
			get => leading;
		    set
			{
				if ((value < Coder.SHORT_MIN) || (value > Coder.SHORT_MAX))
				{
					throw new IllegalArgumentRangeException(Coder.SHORT_MIN, Coder.SHORT_MAX, value);
				}
				leading = value;
			}
		}

		/// <summary>
		/// Returns the list of advances defined for each glyph in the font.
		/// </summary>
		/// <returns> a list of Integer objects that contain the advance for each
		///         glyph in the font. </returns>
		public IList<int?> Advances
		{
			get => advances;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				advances = value;
			}
		}

		/// <summary>
		/// Returns the list of bounding rectangles defined for each glyph in the
		/// font.
		/// </summary>
		/// <returns> a list of Bounds objects. </returns>
		public IList<Bounds> Bounds
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
		/// Returns the list of kerning records that define the spacing between
		/// glyph pairs.
		/// </summary>
		/// <returns> a list of Kerning objects that define the spacing adjustment
		///         between pairs of glyphs. </returns>
		public IList<Kerning> Kernings
		{
			get => kernings;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				kernings = value;
			}
		}













		/// <summary>
		/// {@inheritDoc} </summary>
		public DefineFont3 copy()
		{
			return new DefineFont3(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, encoding, small, italic, bold, language, name, shapes, codes, ascent, descent, leading, advances, bounds, kernings);
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			wideCodes = (context.get(Context.VERSION) > 5) || encoding != 1;

			context.put(Context.FILL_SIZE, 1);
			context.put(Context.LINE_SIZE, context.contains(Context.POSTSCRIPT) ? 1 : 0);
			if (wideCodes)
			{
				context.put(Context.WIDE_CODES, 1);
			}



			int count = shapes.Count;
			int index = 0;
			int tableEntry;
			int shapeLength;

			if (wideOffsets)
			{
				tableEntry = (count << 2) + 4;
			}
			else
			{
				tableEntry = (count << 1) + 2;
			}

			table = new int[count + 1];

			int glyphLength = 0;

			foreach (Shape shape in shapes)
			{
				table[index++] = tableEntry;
				shapeLength = shape.prepareToEncode(context);
				glyphLength += shapeLength;
				tableEntry += shapeLength;
			}

			table[index++] = tableEntry;

			wideOffsets = (shapes.Count * 2 + glyphLength) > Coder.USHORT_MAX;

			length = 5;
			length += context.strlen(name);
			length += 2;
			length += shapes.Count * (wideOffsets ? 4 : 2);
			length += wideOffsets ? 4 : 2;
			length += glyphLength;
			length += shapes.Count * (wideCodes ? 2 : 1);

			if (containsLayoutInfo())
			{
				length += 6;
				length += advances.Count * 2;

				foreach (Bounds bound in bounds)
				{
					length += bound.prepareToEncode(context);
				}

				length += 2;
				length += kernings.Count * (wideCodes ? 6 : 4);
			}

			context.put(Context.FILL_SIZE, 0);
			context.put(Context.LINE_SIZE, 0);
			context.remove(Context.WIDE_CODES);

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>




		public void encode(SWFEncoder coder, Context context)
		{
			int format;
			if (encoding == 1)
			{
				format = 1;
			}
			else if (small)
			{
				format = 2;
			}
			else if (encoding == 2)
			{
				// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
				format = 4;
			}
			else
			{
				format = 0;
			}

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_FONT_3 << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_FONT_3 << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			context.put(Context.FILL_SIZE, 1);
			context.put(Context.LINE_SIZE, context.contains(Context.POSTSCRIPT) ? 1 : 0);
			if (wideCodes)
			{
				context.put(Context.WIDE_CODES, 1);
			}

			int bits = 0;
			bits |= containsLayoutInfo() ? Coder.BIT7 : 0;
			bits |= format << Coder.TO_UPPER_NIB;
			bits |= wideOffsets ? Coder.BIT3 : 0;
			bits |= wideCodes ? Coder.BIT2 : 0;
			bits |= italic ? Coder.BIT1 : 0;
			bits |= bold ? Coder.BIT0 : 0;
			coder.writeByte(bits);

			coder.writeByte(language);
			coder.writeByte(context.strlen(name));

			coder.writeString(name);
			coder.writeShort(shapes.Count);

			if (wideOffsets)
			{
				for (int i = 0; i < table.Length; i++)
				{
					coder.writeInt(table[i]);
				}
			}
			else
			{
				for (int i = 0; i < table.Length; i++)
				{
					coder.writeShort(table[i]);
				}
			}

			foreach (Shape shape in shapes)
			{
				shape.encode(coder, context);
			}

			if (wideCodes)
			{
				foreach (int? code in codes)
				{
					coder.writeShort(code.Value);
				}
			}
			else
			{
				foreach (int? code in codes)
				{
					coder.writeByte(code.Value);
				}
			}

			if (containsLayoutInfo())
			{
				coder.writeShort(ascent);
				coder.writeShort(descent);
				coder.writeShort(leading);

				foreach (int? advance in advances)
				{
					coder.writeShort(advance.Value);
				}

				foreach (Bounds bound in bounds)
				{
					bound.encode(coder, context);
				}

				coder.writeShort(kernings.Count);

				foreach (Kerning kerning in kernings)
				{
					kerning.encode(coder, context);
				}
			}

			context.put(Context.FILL_SIZE, 0);
			context.put(Context.LINE_SIZE, 0);
			context.remove(Context.WIDE_CODES);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}

		/// <summary>
		/// Does the font contain layout information for the glyphs. </summary>
		/// <returns> true if the font contains layout information, false otherwise. </returns>
		private bool containsLayoutInfo()
		{


			bool layout = (ascent != 0) || (descent != 0) || (leading != 0) || advances.Count > 0 || bounds.Count > 0 || kernings.Count > 0;

			return layout;
		}
	}

}