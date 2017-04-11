using System.Collections.Generic;
using com.flagstone.transform.datatype;
using com.flagstone.transform.font;
using com.flagstone.transform.text;

/*
 * TextTable.java
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

namespace com.flagstone.transform.util.text
{
    /// <summary>
	/// TextTable is used to generate test definitions for a specific (fixed) point
	/// size of a font.
	/// 
	/// Each instance caches a table of GlyphIndex objects with predefined advances
	/// allowing the objects for each character used to be shared amongst the
	/// TextSpan objects that are used to display text.
	/// </summary>
	public sealed class TextTable
    {

        /// <summary>
        /// Size in twips of the EM Square used for glyph coordinates. </summary>
        private const float EMSQUARE = 1024.0f;

        /// <summary>
        /// The size, in twips, of the font. </summary>

        private readonly int size;
        /// <summary>
        /// The height, in twips of the font above the baseline. </summary>

        private readonly int ascent;
        /// <summary>
        /// The height, in twips of the font below the baseline. </summary>

        private readonly int descent;
        /// <summary>
        /// The unique identifier of the font. </summary>

        private readonly int identifier;

        /// <summary>
        /// A table of characters and the corresponding GlyphIndex with the
        /// pre-calculated advance for the font.
        /// </summary>

        private readonly IDictionary<char?, GlyphIndex> characters;

        /// <summary>
        /// Creates a TextTable for the specified font size. </summary>
        /// <param name="font"> the font definition. </param>
        /// <param name="fontSize"> the size of the font in twips. </param>


        public TextTable(DefineFont2 font, int fontSize)
        {

            identifier = font.Identifier;
            size = fontSize;
            characters = new Dictionary<char?, GlyphIndex>();



            IList<int?> codes = font.Codes;


            IList<int?> advances = font.Advances;


            float scale = fontSize / EMSQUARE;


            int count = codes.Count;

            ascent = (int)(font.Ascent * scale);
            descent = (int)(font.Descent * scale);

            for (int i = 0; i < count; i++)
            {
                characters[(char)codes[i].Value] = new GlyphIndex(i, (int)(advances[i] * scale));
            }
        }

        /// <summary>
        /// Create a bound box that encloses the line of text when rendered using the
        /// specified font and size.
        /// </summary>
        /// <param name="text">
        ///            the string to be displayed.
        /// </param>
        /// <returns> the bounding box that completely encloses the text. </returns>


        public Bounds boundsForText(string text)
        {
            int total = 0;
            for (int i = 0; i < text.Length; i++)
            {
                total += characters[text[i]].Advance;
            }
            return new Bounds(0, -ascent, total, descent);
        }

        /// <summary>
        /// Create a list of characters that can be added to a text span.
        /// </summary>
        /// <param name="text">
        ///            the string to be displayed.
        /// </param>
        /// <returns> a TextSpan object that can be added to a DefineText or
        ///         DefineText2 object. </returns>


        public IList<GlyphIndex> charactersForText(string text)
        {


            IList<GlyphIndex> list = new List<GlyphIndex>(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                list.Add(characters[text[i]]);
            }
            return list;
        }

        /// <summary>
        /// Create a span of text that can be added to a static text field.
        /// </summary>
        /// <param name="text">
        ///            the string to be displayed.
        /// </param>
        /// <param name="color">
        ///            the colour used to display the text.
        /// </param>
        /// <param name="xCoord">
        ///            the x-coordinate for the origin of the text span.
        /// </param>
        /// <param name="yCoord">
        ///            the y-coordinate for the origin of the text span.
        /// </param>
        /// <returns> a TextSpan object that can be added to a DefineText or
        ///         DefineText2 object. </returns>


        public TextSpan defineSpan(string text, Color color, int xCoord, int yCoord)
        {
            return new TextSpan(identifier, size, color, xCoord, yCoord, charactersForText(text));
        }

        /// <summary>
        /// Create a definition for a static text field that displays a single line
        /// of text in the specified font.
        /// </summary>
        /// <param name="uid">
        ///            the unique identifier that will be used to reference the text
        ///            field in a flash file.
        /// </param>
        /// <param name="text">
        ///            the string to be displayed.
        /// </param>
        /// <param name="color">
        ///            the colour used to display the text.
        /// </param>
        /// <returns> a DefineText2 object that can be added to a Flash file. </returns>


        public DefineText2 defineText(int uid, string text, Color color)
        {


            CoordTransform transform = CoordTransform.translate(0, 0);


            List<TextSpan> spans = new List<TextSpan>();
            spans.Add(defineSpan(text, color, 0, 0));
            return new DefineText2(uid, boundsForText(text), transform, spans);
        }

        /// <summary>
        /// Create a definition for a static text field that displays a block of text
        /// in the specified font.
        /// </summary>
        /// <param name="uid">
        ///            the unique identifier that will be used to reference the text
        ///            field in a flash file.
        /// </param>
        /// <param name="lines">
        ///            the list of strings to be displayed.
        /// </param>
        /// <param name="color">
        ///            the colour used to display the text.
        /// </param>
        /// <param name="lineSpacing">
        ///            the spearation between successive lines of text.
        /// </param>
        /// <returns> a DefineText2 object that can be added to a Flash file. </returns>


        public DefineText2 defineTextBlock(int uid, IList<string> lines, Color color, int lineSpacing)
        {


            CoordTransform transform = CoordTransform.translate(0, 0);

            int xMin = 0;
            int yMin = 0;
            int xMax = 0;
            int yMax = 0;

            int yOffset = ascent;



            List<TextSpan> spans = new List<TextSpan>();
            string text;

            int lineNumber = 0;

            IEnumerator<string> i;

            for (i = lines.GetEnumerator(); i.MoveNext(); yOffset += lineSpacing, lineNumber++)
            {

                text = i.Current;

                spans.Add(new TextSpan(identifier, size, color, 0, yOffset, charactersForText(text)));



                Bounds bounds = boundsForText(text);

                if (lineNumber == 0)
                {
                    yMin = bounds.MinY;
                    yMax = bounds.MaxY;
                }
                else
                {
                    yMax += lineSpacing;
                }

                if (lineNumber == lines.Count - 1)
                {
                    yMax += bounds.Height;
                }

                xMin = (xMin < bounds.MinX) ? xMin : bounds.MinX;
                xMax = (xMax > bounds.MaxX) ? xMax : bounds.MaxX;
            }

            i.Dispose();

            return new DefineText2(uid, new Bounds(xMin, yMin, xMax, yMax), transform, spans);
        }
    }

}