//AG: TODO
//This class has not been ported. Not sure if it is necessary in .NET.

/*
 * AWTDecoder.java
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

//using System;
//using System.Collections.Generic;
//using System.IO;

//namespace com.flagstone.transform.util.font
//{

//    using com.flagstone.transform.coder;
//    using Bounds = com.flagstone.transform.datatype.Bounds;
//    using CharacterFormat = com.flagstone.transform.font.CharacterFormat;
//    using Shape = com.flagstone.transform.shape.Shape;
//    using Canvas = com.flagstone.transform.util.shape.Canvas;

//    /// <summary>
//    /// AWTDecoder decodes Java AWT Fonts so they can be used in a Flash file.
//    /// </summary>
//    public sealed class AWTDecoder
//    {
//        /// <summary>
//        /// Number of edge points from a PathIterator segment. </summary>
//        private const int SEGMENT_COUNT = 6;
//        /// <summary>
//        /// x-coordinate of the end point of a move or line. </summary>
//        private const int XCOORD = 0;
//        /// <summary>
//        /// y-coordinate of the end point of a move or line. </summary>
//        private const int YCOORD = 1;
//        /// <summary>
//        /// x-coordinate of the control point for a quadratic curve. </summary>
//        private const int QUAD_CTRLX = 0;
//        /// <summary>
//        /// y-coordinate of the control point for a quadratic curve. </summary>
//        private const int QUAD_CTRLY = 1;
//        /// <summary>
//        /// x-coordinate of the anchor point for a quadratic curve. </summary>
//        private const int QUAD_ANCHORX = 2;
//        /// <summary>
//        /// y-coordinate of the anchor point for a quadratic curve. </summary>
//        private const int QUAD_ANCHORY = 3;
//        /// <summary>
//        /// x-coordinate of the first control point for a cubic curve. </summary>
//        private const int CUBE_CTRL1_X = 0;
//        /// <summary>
//        /// y-coordinate of the first control point for a cubic curve. </summary>
//        private const int CUBE_CTRL1_Y = 1;
//        /// <summary>
//        /// x-coordinate of the second control point for a cubic curve. </summary>
//        private const int CUBE_CTRL2_X = 2;
//        /// <summary>
//        /// y-coordinate of the second control point for a cubic curve. </summary>
//        private const int CUBE_CTRL2_Y = 3;
//        /// <summary>
//        /// x-coordinate of the anchor point for a cubic curve. </summary>
//        private const int CUBE_ANCHORX = 4;
//        /// <summary>
//        /// y-coordinate of the anchor point for a cubic curve. </summary>
//        private const int CUBE_ANCHORY = 5;
//        /// <summary>
//        /// Size of the EM-Square in twips. </summary>
//        private const float EM_SQUARE_SIZE = 1024.0f;

//        /// <summary>
//        /// The list of fonts decoded. </summary>

//        private readonly IList<Font> fonts = new List<Font>();

//        /// <summary>
//        /// Decode an AWT Font. </summary>
//        /// <param name="font"> an AWT Font object. </param>
//        /// <exception cref="IOException"> if an error occurs decoding the font data. </exception>
//        /// <exception cref="Exception"> if the font is in a format not supported by
//        /// the decoder. </exception>



//        public void read(java.awt.Font font)
//        {
//            decode(font);
//        }

//        /// <summary>
//        /// Get the list of fonts decoded. </summary>
//        /// <returns> a list of fonts. </returns>
//        public IList<Font> Fonts
//        {
//            get
//            {
//                return fonts;
//            }
//        }

//        /// <summary>
//        /// Decode the AWT font. </summary>
//        /// <param name="aFont"> an AWT Font. </param>


//        private void decode(java.awt.Font aFont)
//        {



//            FontRenderContext fontContext = new FontRenderContext(new AffineTransform(), true, true);
//            java.awt.Font awtFont = aFont.deriveFont(1.0f);



//            Font font = new Font();

//            font.Face = new FontFace(awtFont.Name, awtFont.Bold, awtFont.Italic);
//            font.Encoding = CharacterFormat.UCS2;

//            /*
//			 * The new font scaled to the EM Square must be derived using the size
//			 * as well as the transform used for the glyphs otherwise the advance
//			 * values are not scaled accordingly.
//			 */


//            AffineTransform affine = AffineTransform.getTranslateInstance(0, 0);

//            awtFont = awtFont.deriveFont(affine);
//            awtFont = awtFont.deriveFont(EM_SQUARE_SIZE);



//            int missingGlyph = awtFont.MissingGlyphCode;


//            int count = awtFont.NumGlyphs;

//            font.MissingGlyph = missingGlyph;
//            font.NumberOfGlyphs = count;
//            font.HighestChar = (char)Coder.USHORT_MAX;

//            int index = 0;
//            int code = 0;
//            char character;

//            // create the glyph for the characters that cannot be displayed

//            GlyphVector glyphVector = awtFont.createGlyphVector(fontContext, new int[] { missingGlyph });
//            java.awt.Shape outline = glyphVector.getGlyphOutline(0);
//            int advance = (int)(glyphVector.getGlyphMetrics(0).Advance);

//            font.addGlyph((char)missingGlyph, new Glyph(convertShape(outline), new Bounds(0, 0, 0, 0), advance));

//            index = 1;

//            float ascent = 0.0f;
//            float descent = 0.0f;
//            float leading = 0.0f;

//            /*
//			 * Run through all the unicode character codes looking for a
//			 * corresponding glyph.
//			 */
//            while ((index < count) && (code < Coder.USHORT_MAX))
//            {
//                if (awtFont.canDisplay(code))
//                {
//                    character = (char)code;

//                    glyphVector = awtFont.createGlyphVector(fontContext, new char[] { character });

//                    outline = glyphVector.getGlyphOutline(0);
//                    advance = (int)(glyphVector.getGlyphMetrics(0).Advance);

//                    font.addGlyph(character, new Glyph(convertShape(outline), new Bounds(0, 0, 0, 0), advance));

//                    if (!awtFont.hasUniformLineMetrics())
//                    {


//                        LineMetrics lineMetrics = awtFont.getLineMetrics(new char[] { character }, 0, 1, fontContext);

//                        ascent = Math.Max(lineMetrics.Ascent, ascent);
//                        descent = Math.Max(lineMetrics.Descent, descent);
//                        leading = Math.Max(lineMetrics.Leading, leading);
//                    }
//                    index++;

//                }
//                else
//                {
//                    character = (char)missingGlyph;
//                    font.addMissingGlyph((char)code);
//                }
//                code++;
//            }
//            font.Ascent = (int)ascent;
//            font.Descent = (int)descent;
//            font.Leading = (int)leading;

//            fonts.Add(font);
//        }

//        /// <summary>
//        /// Trace the outline of the glyph. </summary>
//        /// <param name="glyph"> an AWT Shape. </param>
//        /// <returns> a Flash Shape. </returns>


//        private Shape convertShape(java.awt.Shape glyph)
//        {


//            PathIterator pathIter = glyph.getPathIterator(null);


//            Canvas path = new Canvas();



//            double[] coords = new double[SEGMENT_COUNT];

//            while (!pathIter.Done)
//            {
//                switch (pathIter.currentSegment(coords))
//                {
//                    case PathIterator.SEG_MOVETO:
//                        path.close();
//                        path.moveForFont((int)coords[XCOORD], (int)coords[YCOORD]);
//                        break;
//                    case PathIterator.SEG_LINETO:
//                        path.line((int)coords[XCOORD], (int)coords[YCOORD]);
//                        break;
//                    case PathIterator.SEG_QUADTO:
//                        path.curve((int)coords[QUAD_CTRLX], (int)coords[QUAD_CTRLY], (int)coords[QUAD_ANCHORX], (int)coords[QUAD_ANCHORY]);
//                        break;
//                    case PathIterator.SEG_CUBICTO:
//                        path.curve((int)coords[CUBE_CTRL1_X], (int)coords[CUBE_CTRL1_Y], (int)coords[CUBE_CTRL2_X], (int)coords[CUBE_CTRL2_Y], (int)coords[CUBE_ANCHORX], (int)coords[CUBE_ANCHORY]);
//                        break;
//                    case PathIterator.SEG_CLOSE:
//                        path.close();
//                        break;
//                    default:
//                        break;
//                }
//                pathIter.next();
//            }
//            return path.Shape;
//        }
//    }

//}