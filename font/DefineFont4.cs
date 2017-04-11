using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * DefineFont4.java
 * Transform
 *
 * Copyright (c) 2010 Flagstone Software Ltd. All rights reserved.
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
	/// DefineFont4 is used to pass OpenType font data directly to the text rendering
	/// engine added in Flash Player 10.
	/// </summary>
	public sealed class DefineFont4 : DefineTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineFont4: { identifier=%d;" + " italic=%b; bold=%b; name=%s; data=byte<%d> ...}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// Is the font italicized. </summary>
		private bool italic;
		/// <summary>
		/// Is the font bold. </summary>
		private bool bold;
		/// <summary>
		/// The font name. </summary>
		private string name;
		/// <summary>
		/// The OpenType font data. </summary>
		private byte[] data;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a DefineFont4 object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public DefineFont4(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();



			int bits = coder.readByte();

			bold = (bits & 0x00000001) == 1;
			italic = (bits & 0x00000002) == 2;
			name = coder.readString();
			data = coder.readBytes(new byte[length - coder.bytesRead()]);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a new DefineFont4 with the specified identifier, font name and
		/// style and OpenType font data. </summary>
		/// <param name="uid"> the unique identifier for the font. </param>
		/// <param name="fontName"> the name of the font. </param>
		/// <param name="isItalic"> does the font use italics </param>
		/// <param name="isBold"> is the font bold. </param>
		/// <param name="font"> the OpenType font data. </param>


		public DefineFont4(int uid, string fontName, bool isItalic, bool isBold, byte[] font)
		{
			Identifier = uid;
			Italic = isItalic;
			Bold = isBold;
			Name = fontName;
			Data = font;
		}

		/// <summary>
		/// Creates and initialises a DefineFont4 object using the values copied
		/// from another DefineFont3 object.
		/// </summary>
		/// <param name="object">
		///            a DefineFont4 object from which the values will be
		///            copied. </param>


		public DefineFont4(DefineFont4 @object)
		{
			identifier = @object.identifier;
			italic = @object.italic;
			bold = @object.bold;
			name = @object.name;
			data = @object.data;
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
		/// Get the OpenType font definition data. </summary>
		/// <returns> a copy of the binary data containing the definition of the font. </returns>
		public byte[] Data
		{
			get => Arrays.copyOf(data, data.Length);
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				data = Arrays.copyOf(value, value.Length);
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public DefineFont4 copy()
		{
			return new DefineFont4(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, italic, bold, name, data.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			length = 3 + context.strlen(name) + data.Length;
			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>




		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_FONT_4 << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_FONT_4 << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			int bits = 0;
			bits |= data.Length > 0 ? Coder.BIT2 : 0;
			bits |= italic ? Coder.BIT1 : 0;
			bits |= bold ? Coder.BIT0 : 0;
			coder.writeByte(bits);
			coder.writeString(name);
			coder.writeBytes(data);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}