using System;
using System.Collections.Generic;
using System.IO;
using com.flagstone.transform.datatype;
using com.flagstone.transform.font;
using com.flagstone.transform.shape;

/*
 * SWFFontDecoder.java
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

namespace com.flagstone.transform.util.font
{
    /// <summary>
	/// SWFFontDecoder decodes one or more existing font definitions from a Flash
	/// file. The definitions may be either DefineFont/FontInfo pairs, DefineFont2
	/// or DefineFont2. Files containing DefineFont4 definitions are not supported
	/// since they contain font encoded using the OpenType format.
	/// </summary>
	public sealed class SWFFontDecoder : FontProvider, FontDecoder
	{

		/// <summary>
		/// The table of fonts, indexed by unique identifier. </summary>
		
		private readonly IDictionary<int?, Font> fonts = new Dictionary<int?, Font>();

		/// <summary>
		/// The list of glyphs from a DefineFont object. </summary>
		
		private IList<Shape> glyphs;

		/// <summary>
		/// {@inheritDoc} </summary>
		public FontDecoder newDecoder()
		{
			return new SWFFontDecoder();
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void read(FileInfo file)
		{


			Movie movie = new Movie();
			movie.decodeFromFile(file);
			decode(movie);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public IList<Font> Fonts => new List<Font>(fonts.Values);

	    /// <summary>
		/// Decode a font from a stream. </summary>
		/// <param name="movie"> the Flash movie containing the encoded font. </param>
		/// <exception cref="IOException"> if an error occurs while decoding the font data. </exception>
		/// <exception cref="Exception"> if the font is not in a supported format. </exception>



		private void decode(Movie movie)
		{

			fonts.Clear();

			foreach (MovieTag obj in movie.Objects)
			{
				if (obj is DefineFont)
				{
					decode((DefineFont) obj);
				}
				else if (obj is DefineFont2)
				{
					decode((DefineFont2) obj);
				}
				else if (obj is FontInfo)
				{
					decode((FontInfo) obj);
				}
				else if (obj is FontInfo2)
				{
					decode((FontInfo2) obj);
				}
			}
		}

		/// <summary>
		/// Initialise this object with the information from a flash font definition.
		/// </summary>
		/// <param name="definition">
		///            a DefineFont object which contains the definition of the
		///            glyphs. </param>


		public void decode(DefineFont definition)
		{
			glyphs = definition.Shapes;
			fonts[definition.Identifier] = new Font();
		}

		/// <summary>
		/// Initialise this object with the information from a flash font definition.
		/// </summary>
		/// <param name="info">
		///            a FontInfo object that contains information on the font name,
		///            weight, style and character codes. </param>


		public void decode(FontInfo info)
		{



			Font font = fonts[info.Identifier];

			font.Face = new FontFace(info.Name, info.Bold, info.Italic);

			font.Encoding = info.Encoding;
			font.Ascent = 0;
			font.Descent = 0;
			font.Leading = 0;



			int codeCount = info.Codes.Count;


			int highest = info.Codes[codeCount - 1].Value;

			font.HighestChar = (char) highest;

			if (glyphs.Count > 0)
			{
				foreach (int code in info.Codes)
				{
					font.addGlyph((char) code, new Glyph(glyphs[code]));
				}
			}
		}

		/// <summary>
		/// Initialise this object with the information from a flash font definition.
		/// </summary>
		/// <param name="info">
		///            a FontInfo2 object that contains information on the font name,
		///            weight, style and character codes. </param>


		public void decode(FontInfo2 info)
		{



			Font font = fonts[info.Identifier];

			font.Face = new FontFace(info.Name, info.Bold, info.Italic);

			font.Encoding = info.Encoding;
			font.Ascent = 0;
			font.Descent = 0;
			font.Leading = 0;



			int codeCount = info.Codes.Count;


			int highest = info.Codes[codeCount - 1].Value;

			font.HighestChar = (char) highest;

			if (glyphs.Count > 0)
			{
				foreach (int code in info.Codes)
				{
					font.addGlyph((char) code, new Glyph(glyphs[code]));
				}
			}
		}

		/// <summary>
		/// Initialise this object with the information from a flash font definition.
		/// </summary>
		/// <param name="object">
		///            a DefineFont2 object that contains information on the font
		///            name, weight, style and character codes as well as the glyph
		///            definitions. </param>


		public void decode(DefineFont2 @object)
		{



			Font font = new Font();

			font.Face = new FontFace(@object.Name, @object.Bold, @object.Italic);

			font.Encoding = @object.Encoding;
			font.Ascent = @object.Ascent;
			font.Descent = @object.Descent;
			font.Leading = @object.Leading;



			int glyphCount = @object.Shapes.Count;


			int codeCount = @object.Codes.Count;


			int highest = @object.Codes[codeCount - 1].Value;

			font.MissingGlyph = 0;
			font.NumberOfGlyphs = glyphCount;
			font.HighestChar = (char) highest;

			if (glyphCount > 0)
			{

				Shape shape;
				Bounds bounds = null;
				int advance;
				int code;

				for (int i = 0; i < glyphCount; i++)
				{
					shape = @object.Shapes[i];

					if (@object.Bounds != null)
					{
						 bounds = @object.Bounds[i];
					}
					if (@object.Advances == null)
					{
						advance = 0;
					}
					else
					{
						advance = @object.Advances[i].Value;
					}
					code = @object.Codes[i].Value;

					font.addGlyph((char) code, new Glyph(shape, bounds, advance));
				}
			}

			fonts[@object.Identifier] = font;
		}

		/// <summary>
		/// Initialise this object with the information from a flash font definition.
		/// </summary>
		/// <param name="object">
		///            a DefineFont3 object that contains information on the font
		///            name, weight, style and character codes as well as the glyph
		///            definitions. </param>


		public void decode(DefineFont3 @object)
		{



			Font font = new Font();

			font.Face = new FontFace(@object.Name, @object.Bold, @object.Italic);

			font.Encoding = @object.Encoding;
			font.Ascent = @object.Ascent;
			font.Descent = @object.Descent;
			font.Leading = @object.Leading;



			int glyphCount = @object.Shapes.Count;


			int highest = @object.Codes[glyphCount - 1].Value;

			font.MissingGlyph = 0;
			font.NumberOfGlyphs = glyphCount;
			font.HighestChar = (char) highest;

			if (glyphCount > 0)
			{

				Shape shape;
				Bounds bounds = null;
				int advance;
				int code;

				for (int i = 0; i < glyphCount; i++)
				{
					shape = @object.Shapes[i];

					if (@object.Bounds != null)
					{
						 bounds = @object.Bounds[i];
					}
					if (@object.Advances == null)
					{
						advance = 0;
					}
					else
					{
						advance = @object.Advances[i].Value;
					}
					code = @object.Codes[i].Value;

					font.addGlyph((char) code, new Glyph(shape, bounds, advance));
				}
			}

			fonts[@object.Identifier] = font;
		}
	}

}