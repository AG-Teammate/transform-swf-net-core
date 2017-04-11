using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * FontAlignment.java
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

namespace com.flagstone.transform.font
{
    /// <summary>
	/// FontAlignment provides the alignment information for the glyphs in a font.
	/// </summary>
	public sealed class FontAlignment : MovieTag
	{

		/// <summary>
		/// StrokeWidth is used to provide hints about the thickness of the line
		/// used for rendering glyphs in a font.
		/// </summary>
		public enum StrokeWidth
		{
			/// <summary>
			/// Thin strokes. </summary>
			THIN,
			/// <summary>
			/// Medium thick strokes. </summary>
			MEDIUM,
			/// <summary>
			/// Thick strokes. </summary>
			THICK
		}

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "FontAlignment: { identifier=%d;" + " strokeWidth=%s; zones=%s}";

		/// <summary>
		/// The unique identifier of the font that the alignment applies to. </summary>
		private int identifier;
		/// <summary>
		/// Compound code used for drawing hints. </summary>
		
		private int hints;
		/// <summary>
		/// Alignment zones for snapping areas of glyphs to pixel boundaries. </summary>
		private IList<GlyphAlignment> zones;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a FontAlignment object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public FontAlignment(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			hints = coder.readByte();

			zones = new List<GlyphAlignment>();

			while (coder.bytesRead() < length)
			{
				zones.Add(new GlyphAlignment(coder));
			}
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a new FontAlignment object for the referenced font along with
		/// information on the stroke width used to draw the glyphs and a list of
		/// alignment zones for each glyph. </summary>
		/// <param name="uid"> the unique identifier of the font. </param>
		/// <param name="stroke"> the typical width used when drawing the glyphs. </param>
		/// <param name="list"> a list of alignment boxes used for rendering the glyphs. </param>


		public FontAlignment(int uid, StrokeWidth stroke, IList<GlyphAlignment> list)
		{
			Identifier = uid;
			setStrokeWidth(stroke);
			Zones = list;
		}

		/// <summary>
		/// Creates and initialises a FontAlignment object using the values copied
		/// from another FontAlignment object.
		/// </summary>
		/// <param name="object">
		///            a FontAlignment object from which the values will be
		///            copied. </param>


		public FontAlignment(FontAlignment @object)
		{
			identifier = @object.identifier;
			hints = @object.hints;
			zones = new List<GlyphAlignment>(@object.zones);
		}

		/// <summary>
		/// Get the unique identifier of the font definition that the alignment
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
		/// Get the StrokeWidth that describes how the glyphs in the font are
		/// typically drawn. </summary>
		/// <returns> a StrokeWidth defining how the font is drawn. </returns>
		public StrokeWidth getStrokeWidth()
		{
			StrokeWidth stroke;
			switch (hints)
			{
			case Coder.BIT6:
				stroke = StrokeWidth.MEDIUM;
				break;
			case Coder.BIT7:
				stroke = StrokeWidth.THICK;
				break;
			default:
				stroke = StrokeWidth.THIN;
				break;
			}
			return stroke;
		}

		/// <summary>
		/// Set the StrokeWidth that describes how the glyphs in the font are
		/// typically drawn. </summary>
		/// <param name="stroke"> a StrokeWidth defining how the font is drawn. </param>


		public void setStrokeWidth(StrokeWidth stroke)
		{
			switch (stroke)
			{
			case StrokeWidth.MEDIUM:
				hints = Coder.BIT6;
				break;
			case StrokeWidth.THICK:
				hints = Coder.BIT7;
				break;
			default:
				hints = 0x00;
				break;
			}
		}

		/// <summary>
		/// Get the alignment information for each glyph in the font. </summary>
		/// <returns> a list of GlyphAliment objects that describe the areas in each
		/// glyph which can be snapped to the nearest pixel to improve display
		/// quality. </returns>
		public IList<GlyphAlignment> Zones
		{
			get => zones;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				zones = value;
			}
		}


		/// <summary>
		/// Add the alignment information for a glyph to the list. </summary>
		/// <param name="zone"> the alignment information for a glyph. </param>
		/// <returns> this object. </returns>


		public FontAlignment addZone(GlyphAlignment zone)
		{
			if (zone == null)
			{
				throw new ArgumentException();
			}
			zones.Add(zone);
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public FontAlignment copy()
		{
			return new FontAlignment(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, getStrokeWidth(), zones);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			length = 3;

			foreach (GlyphAlignment zone in zones)
			{
				length += zone.prepareToEncode(context);
			}

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.FONT_ALIGNMENT << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.FONT_ALIGNMENT << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			coder.writeByte(hints);

			foreach (GlyphAlignment zone in zones)
			{
				zone.encode(coder, context);
			}
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}