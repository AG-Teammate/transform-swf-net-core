using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;
using com.flagstone.transform.font;
using com.flagstone.transform.shape;

/*
 * Font.java
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
	/// <para>
	/// Font is used to add embedded fonts to a movie.
	/// </para>
	/// 
	/// <para>
	/// Flash supports two types of font definition: embedded fonts where the Flash
	/// file contains the glyphs that are drawn to represents the text characters and
	/// device fonts where the font is provided by the Flash Player showing the
	/// movie. Embedded fonts are preferred since the movie will always look the same
	/// regardless of where it is played - if a Flash Player does not contain a
	/// device font it will substitute it with another.
	/// </para>
	/// 
	/// <para>
	/// Device fonts can be added to a movie by simply creating a DefineFont or
	/// DefineFont2 object which contain the name of the font. An embedded font must
	/// contain all the information to draw and layout the glyphs representing the
	/// text to be displayed. The Font class hides all this detail and makes it easy
	/// to add embedded fonts to a movie.
	/// </para>
	/// <para>
	/// 
	/// </para>
	/// <para>
	/// The Font class can be used to create embedded fonts in three ways:
	/// </para>
	/// 
	/// <ol>
	/// <li>Using TrueType or OpenType font definition stored in a file.</li>
	/// <li>Using an existing font definition from a flash file.</li>
	/// <li>Using a given Java AWT font as a template.</li>
	/// </ol>
	/// 
	/// <P>
	/// For OpenType or TrueType fonts, files with the extensions ".otf" or ".ttf"
	/// may be used. Files containing collections of fonts ".otc" are not currently
	/// supported.
	/// </p>
	/// 
	/// <para>
	/// Using an existing Flash font definition is the most interesting. Fonts can
	/// initially be created using AWT Font objects or TrueType files and all the
	/// visible characters included. If the generated Flash definition is saved to a
	/// file it can easily and quickly be loaded. Indeed the overhead of parsing an
	/// AWT or TrueType font is significant (sometimes several seconds) so creating
	/// libraries of "pre-parsed" flash fonts is the preferred way of use fonts.
	/// </para>
	/// </summary>
	public sealed class Font
	{

		/// <summary>
		/// The face describing the font. </summary>
		private FontFace face;
		/// <summary>
		/// The encoding used for character codes. </summary>
		private CharacterFormat encoding;
		/// <summary>
		/// The height of the font above the baseline. </summary>
		private int ascent;
		/// <summary>
		/// The height of the font below the baseline. </summary>
		private int descent;
		/// <summary>
		/// The spacing between lines. </summary>
		private int leading;

		/// <summary>
		/// Table mapping character codes to glyphs. </summary>
		
		private int[] charToGlyph;
		/// <summary>
		/// Table mapping glyphs to character codes. </summary>
		
		private int[] glyphToChar;
		/// <summary>
		/// Table of glyphs. </summary>
		
		private Glyph[] glyphTable;

		/// <summary>
		/// The current glyph. </summary>
		
		private int glyphIndex;
		/// <summary>
		/// The number of glyphs in the font. </summary>
		
		private int glyphCount;
		/// <summary>
		/// The index of the glyph used to represent undisplayable characters. </summary>
		
		private int missingGlyph;
		/// <summary>
		/// The highest character code. </summary>
		
		private char highestChar;
		/// <summary>
		/// List of kernings for selected pairs of characters. </summary>
		
		private readonly IList<Kerning> kernings = new List<Kerning>();

		/// <summary>
		/// Get the FontFace that contains the font name and style. </summary>
		/// <returns> the FontFace. </returns>
		public FontFace Face
		{
			get => face;
		    set => face = value;
		}


		/// <summary>
		/// Get the encoding scheme used for the character codes, either UCS2,
		/// ANSI or SJIS.
		/// </summary>
		/// <returns> the encoding used for the character codes. </returns>
		public CharacterFormat Encoding
		{
			get => encoding;
		    set => encoding = value;
		}


		/// <summary>
		/// Get the ascent for the font in twips.
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
		/// Get the descent for the font in twips.
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
		/// Get the number of glyphs defined in the font.
		/// </summary>
		/// <returns> the number of glyphs. </returns>
		public int NumberOfGlyphs
		{
			get => glyphCount;
		    set
			{
		//        glyphTable = new Glyph[Coder.USHORT_MAX + 1];
		//        glyphToChar = new int[Coder.USHORT_MAX + 1];
				glyphTable = new Glyph[value];
				glyphToChar = new int[value];
				glyphIndex = 0;
			}
		}


		/// <summary>
		/// Get the highest character code defined in the font.
		/// </summary>
		/// <returns> the character with the highest character code. </returns>
		public char HighestChar
		{
			get => highestChar;
		    set
			{
				highestChar = value;
				//charToGlyph = new int[Coder.USHORT_MAX + 1];
				charToGlyph = new int[value + 1];
			}
		}


		/// <summary>
		/// Get the index of the glyph used to represent characters that are not
		/// supported int the font. </summary>
		/// <returns> the index of the glyph for unsupported characters. </returns>
		public int MissingGlyph
		{
			get => missingGlyph;
		    set => missingGlyph = value;
		}


		/// <summary>
		/// Get the glyph from the specified position in the table. </summary>
		/// <param name="index"> the index of the glyph. </param>
		/// <returns> the corresponding glyph. </returns>


		public Glyph getGlyph(int index)
		{
			return glyphTable[index];
		}

		/// <summary>
		/// Add a character and the corresponding glyph that is displayed to the
		/// font. </summary>
		/// <param name="code"> the character </param>
		/// <param name="glyph"> the glyph displayed for the character. </param>


		public void addGlyph(char code, Glyph glyph)
		{
			glyphTable[glyphIndex] = glyph;
			glyphToChar[glyphIndex] = code;
			charToGlyph[code] = glyphIndex;
			glyphIndex++;
		}

		/// <summary>
		/// Add a character to the font where the missing glyph will be displayed. </summary>
		/// <param name="code"> the character where there is no corresponding glyph. </param>


		public void addMissingGlyph(char code)
		{
			charToGlyph[code] = missingGlyph;
		}

		/// <summary>
		/// Create and return a DefineFont2 object that contains information to
		/// display a set of characters.
		/// </summary>
		/// <param name="identifier">
		///            the unique identifier that will be used to reference the font
		///            definition in the flash file.
		/// </param>
		/// <param name="characters">
		///            the set of characters that the font must contain glyphs and
		///            layout information for,
		/// </param>
		/// <returns> a font definition that contains information for all the glyphs in
		///         the set of characters. </returns>


		public DefineFont2 defineFont(int identifier, IList<char?> characters)
		{

			DefineFont2 fontDefinition = null;


			int count = characters.Count;



			List<Shape> glyphsArray = new List<Shape>(count);


			List<int?> codesArray = new List<int?>(count);


			List<int?> advancesArray = new List<int?>(count);


			List<Bounds> boundsArray = new List<Bounds>(count);

			foreach (char? character in characters)
			{


				Glyph glyph = glyphTable[charToGlyph[(int)character]];

				glyphsArray.Add(glyph.Shape);
				codesArray.Add((int) character);
				advancesArray.Add(glyph.Advance);

				if (glyph.Bounds != null)
				{
					boundsArray.Add(glyph.Bounds);
				}
			}

			fontDefinition = new DefineFont2(identifier, face.Name);

			fontDefinition.Encoding = encoding;
			fontDefinition.Italic = face.Italic;
			fontDefinition.Bold = face.Bold;
			fontDefinition.Ascent = ascent;
			fontDefinition.Descent = descent;
			fontDefinition.Leading = leading;
			fontDefinition.Shapes = glyphsArray;
			fontDefinition.Codes = codesArray;
			fontDefinition.Advances = advancesArray;
			fontDefinition.Bounds = boundsArray;
			fontDefinition.Kernings = kernings;

			return fontDefinition;
		}

		/// <summary>
		/// Tests whether the font can display all the characters in a string.
		/// </summary>
		/// <param name="aString">
		///            the string to be displayed.
		/// </param>
		/// <returns> the index of the character that cannot be displayed or -1 if all
		///         characters have corresponding glyphs. </returns>


		public int canDisplay(string aString)
		{
			int firstMissingChar = -1;

			for (int i = 0; i < aString.Length; i++)
			{
				if (!canDisplay(aString[i]))
				{
					firstMissingChar = i;
					break;
				}
			}
			return firstMissingChar;
		}

		/// <summary>
		/// Tests whether a character can be displayed by the font or whether the
		/// "missing" character glyph (usually an empty box) will be displayed
		/// instead.
		/// </summary>
		/// <param name="character">
		///            the character to be displayed.
		/// </param>
		/// <returns> true if the font contains a glyph for character or false if there
		///         is no corresponding glyph and the missing character glyph will be
		///         displayed. </returns>


		public bool canDisplay(char character)
		{
			bool canDisplay;

			if ((character < charToGlyph.Length) && ((character == ' ') || (charToGlyph[character] != 0)))
			{
				canDisplay = true;
			}
			else
			{
				canDisplay = false;
			}

			return canDisplay;
		}

		/// <summary>
		/// Returns the glyph for the specified character.
		/// </summary>
		/// <param name="character">
		///            the character.
		/// </param>
		/// <returns> the Glyph object which contains the layout information. </returns>


		public int glyphForCharacter(char character)
		{
			return charToGlyph[character];
		}

		/// <summary>
		/// Returns the characters code for the glyph at a specified index in the
		/// font. This method is used for extracting the strings displayed by static
		/// text (DefineText, DefineText2) fields.
		/// </summary>
		/// <param name="index">
		///            the index of the font.
		/// </param>
		/// <returns> the character code for the glyph. </returns>


		public char characterForGlyph(int index)
		{
			return (char) glyphToChar[index];
		}

		/// <summary>
		/// Returns the default advance for the font as defined in the EM Square -
		/// conceptually a font with a point size of 1024. The number returned needs
		/// to be scaled to the correct size in order to calculate the advance to the
		/// next character.
		/// </summary>
		/// <param name="character">
		///            the character code. </param>
		/// <returns> the advance in twips to the next character. </returns>


		public int advanceForCharacter(char character)
		{
			return glyphTable[charToGlyph[character]].Advance;
		}
	}

}