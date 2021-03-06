﻿using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * FontInfo.java
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
	/// FontInfo defines the name and face of a font and maps the codes for a given
	/// character set to the glyphs that are drawn to represent each character.
	/// 
	/// <para>
	/// Three different encoding schemes are supported for the character codes. The
	/// ANSI character set is used for Latin languages, SJIS is used for Japanese
	/// language characters and Unicode is used for any character set. Since Flash 5
	/// Unicode is the preferred encoding scheme.
	/// </para>
	/// 
	/// <para>
	/// The index of each entry in the codes list matches the index in the
	/// corresponding glyph in the shapes list of an DefineFont object, allowing a
	/// given character code to be mapped to a given glyph.
	/// </para>
	/// 
	/// <para>
	/// FontInfo also allows the font associated with a Flash file to be mapped to a
	/// font installed on the device where the Flash Player displaying the file is
	/// hosted. The use of a font from a device is not automatic but is determined by
	/// the HTML tag option <i>deviceFont</i> which is passed to the Flash Player
	/// when it is first started. If a device does not support a given font then the
	/// glyphs in the DefineFont class are used to render the characters.
	/// </para>
	/// 
	/// <para>
	/// An important distinction between the host device to specify the font and
	/// using the glyphs in an DefineFont object is that the device font is not
	/// anti-aliased and the rendering is dependent on the host device. The glyphs in
	/// an DefineFont object are anti-aliased and are guaranteed to look identical on
	/// every device the text is displayed.
	/// </para>
	/// </summary>


	public sealed class FontInfo : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "FontInfo: { identifier=%d;" + " encoding=%s; small=%s; italic=%s; bold=%s;" + " name=%s; codes=%s}";

		/// <summary>
		/// The unique identifier of the font that the info applies to. </summary>
		private int identifier;
		/// <summary>
		/// The font name. </summary>
		private string name;
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
		/// Mapping of glyphs (index) to character codes. </summary>
		private IList<int?> codes;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// Whether character codes are 8-bit (false) or 16-bit (true). </summary>
		
		private bool wideCodes;

		/// <summary>
		/// Creates and initialises an FontInfo object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>




		public FontInfo(SWFDecoder coder)
		{
			codes = new List<int?>();
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();


			int nameLength = coder.readByte();
			name = coder.readString(nameLength);

			if (name.Length > 0)
			{
				while (name[name.Length - 1] == 0)
				{
					name = name.Substring(0, name.Length - 1);
				}
			}



			int bits = coder.readByte();
			small = (bits & Coder.BIT5) != 0;
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			encoding = (bits >> 3) & Coder.LOWEST3;
			italic = (bits & Coder.BIT2) != 0;
			bold = (bits & Coder.BIT1) != 0;
			wideCodes = (bits & Coder.BIT0) != 0;

			if (wideCodes)
			{
				while (coder.bytesRead() < length)
				{
					codes.Add(coder.readUnsignedShort());
				}
			}
			else
			{
				while (coder.bytesRead() < length)
				{
					codes.Add(coder.readByte());
				}
			}
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Constructs a basic FontInfo object specifying only the name and style of
		/// the font.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier of the DefineFont that contains the
		///            glyphs for the font. </param>
		/// <param name="fontName">
		///            the name assigned to the font, identifying the font family. </param>
		/// <param name="isBold">
		///            indicates whether the font weight is bold (true) or normal
		///            (false). </param>
		/// <param name="isItalic">
		///            indicates whether the font style is italic (true) or plain
		///            (false). </param>


		public FontInfo(int uid, string fontName, bool isBold, bool isItalic)
		{
			Identifier = uid;
			Name = fontName;
			Italic = isItalic;
			Bold = isBold;
			small = false;
			encoding = 0;
			codes = new List<int?>();
		}

		/// <summary>
		/// Creates and initialises a FontInfo object using the values copied
		/// from another FontInfo object.
		/// </summary>
		/// <param name="object">
		///            a FontInfo object from which the values will be
		///            copied. </param>


		public FontInfo(FontInfo @object)
		{
			identifier = @object.identifier;
			name = @object.name;
			italic = @object.italic;
			bold = @object.bold;
			small = @object.small;
			encoding = @object.encoding;
			codes = new List<int?>(@object.codes);
		}

		/// <summary>
		/// Get the unique identifier of the font definition that this font
		/// information is for.
		/// </summary>
		/// <returns> the unique identifier of the font. </returns>
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
		/// Get the name of the font family.
		/// </summary>
		/// <returns> the font name. </returns>
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
		/// Get the encoding scheme used for characters rendered in the font,
		/// either ASCII, SJIS or UCS2.
		/// </summary>
		/// <returns> the encoding used for the character codes. </returns>
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
		/// <returns> true if the font is small. </returns>
		public bool Small
		{
			get => small;
		    set => small = value;
		}


		/// <summary>
		/// Is the font style italics.
		/// </summary>
		/// <returns> true if the font is in italics. </returns>
		public bool Italic
		{
			get => italic;
		    set => italic = value;
		}

		/// <summary>
		/// Is the font weight bold.
		/// </summary>
		/// <returns> true if the font weight is bold. </returns>
		public bool Bold
		{
			get => bold;
		    set => bold = value;
		}

		/// <summary>
		/// Get the list of character codes.
		/// </summary>
		/// <returns> the list of character codes defined in the font. </returns>
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
		/// Add a code to the list of codes. The index position of a character code
		/// in the list identifies the index of the corresponding glyph in the
		/// DefineFont object.
		/// </summary>
		/// <param name="aCode">
		///            a code for a glyph. Must be in the range 0..65535. </param>


		public void addCode(int aCode)
		{
			if ((aCode < 0) || (aCode > Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, aCode);
			}
			codes.Add(aCode);
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public FontInfo copy()
		{
			return new FontInfo(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, encoding, small, italic, bold, name, codes);
		}


		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			length = 4;
			length += context.strlen(name);

			wideCodes = false;

			foreach (int? code in codes)
			{
				if (code.Value > 255)
				{
					wideCodes = true;
				}
			}

			length += codes.Count * (wideCodes ? 2 : 1);

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
			// CHECKSTYLE:ON
		}


		/// <summary>
		/// {@inheritDoc} </summary>




		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.FONT_INFO << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.FONT_INFO << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			coder.writeByte(context.strlen(name));
			coder.writeString(name);
			int bits = 0;
			bits |= small ? Coder.BIT5 : 0;
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			bits |= encoding << 3;
			bits |= italic ? Coder.BIT2 : 0;
			bits |= bold ? Coder.BIT1 : 0;
			bits |= wideCodes ? Coder.BIT0 : 0;
			coder.writeByte(bits);

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
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}