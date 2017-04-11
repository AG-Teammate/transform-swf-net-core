using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;

/*
 * TextSpan.java
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
    /// TextSpan is used to display a group of characters with a selected font and
    /// colour. TextSpan objects are used in <seealso cref="DefineText"/> and
    /// <seealso cref="DefineText2"/> to display a line or block of text.
    /// 
    /// <para>
    /// TextSpan contains a list of Character objects which identify the glyphs
    /// that will be displayed along with style information that sets the colour of
    /// the text, the size of the font and the relative placement of the line within
    /// a block of text.
    /// </para>
    /// 
    /// <para>
    /// Whether the alpha channel in the colour needs to be specified depends on the
    /// class the Text is added to. The DefineText2 class supports transparent text
    /// while DefineText class does not.
    /// </para>
    /// 
    /// <para>
    /// The x and y offsets are used to control how several TextSpan objects are laid
    /// out to create a block of text. The y offset is specified relative to the
    /// bottom edge of the bounding rectangle, which is actually closer to the top of
    /// the screen as the direction of the y-axis is from the top to the bottom of
    /// the screen. In this respect Flash is counter-intuitive. Lines with higher
    /// offset values are displayed below lines with lower offsets.
    /// </para>
    /// 
    /// <para>
    /// The x and y offsets are also optional and may be set to the constant
    /// VALUE_NOT_SET when more than one TextSpan object is created. If the y offset
    /// is not specified then a TextSpan object is displayed on the same line as the
    /// previous TextSpan. If the x offset is not specified then the TextSpan is
    /// displayed after the previous TextSpan. This makes it easy to lay text out on
    /// a single line.
    /// </para>
    /// 
    /// <para>
    /// Similarly the font and colour information is optional. The values from a
    /// previous TextSpan object will be used if they are not set.
    /// </para>
    /// 
    /// <para>
    /// The creation and layout of the glyphs to create the text is too onerous to
    /// perform from scratch. It is easier and more convenient to use the
    /// <seealso cref="com.flagstone.transform.util.text.TextTable"/> class to create the
    /// TextSpan objects.
    /// </para>
    /// </summary>
    /// <seealso cref= DefineText </seealso>
    /// <seealso cref= DefineText2 </seealso>
    /// <seealso cref= com.flagstone.transform.util.text.TextTable </seealso>
    /// <seealso cref= com.flagstone.transform.util.font.Font </seealso>
    

    public sealed class TextSpan : SWFEncodeable
    {

        /// <summary>
        /// Format string used in toString() method. </summary>
        private const string FORMAT = "TextSpan: { identifier=%d; color=%s;" + " offsetX=%d; offsetY=%d; height=%d; characters=%s}";

        /// <summary>
        /// The colour used to draw the text. </summary>
        private Color color;
        /// <summary>
        /// The offset on the x-axis of the text span, within the text block. </summary>
        private int? offsetX;
        /// <summary>
        /// The offset on the y-axis of the text span, within the text block. </summary>
        private int? offsetY;
        /// <summary>
        /// The unique identifier of the font used to render the text. </summary>
        private int? identifier;
        /// <summary>
        /// The height of the text in twips. </summary>
        private int? height;

        /// <summary>
        /// The list of characters to be displayed. </summary>
        private IList<GlyphIndex> characters;

        /// <summary>
        /// Indicates whether the text contains font or layout information. </summary>

        private bool hasStyle;
        /// <summary>
        /// Indicate that a font is specified. </summary>

        private bool hasFont;
        /// <summary>
        /// Indicate that a colour is specified. </summary>

        private bool hasColor;
        /// <summary>
        /// Indicate that an x offset is specified. </summary>

        private bool hasX;
        /// <summary>
        /// Indicate that an y offset is specified. </summary>

        private bool hasY;

        /// <summary>
        /// Creates and initialises a TextSpan object using values encoded
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



        public TextSpan(SWFDecoder coder, Context context)
        {



            int bits = coder.readByte();
            hasFont = (bits & Coder.BIT3) != 0;
            hasColor = (bits & Coder.BIT2) != 0;
            hasY = (bits & Coder.BIT1) != 0;
            hasX = (bits & Coder.BIT0) != 0;

            if (hasFont)
            {
                identifier = coder.readUnsignedShort();
            }
            if (hasColor)
            {
                color = new Color(coder, context);
            }
            if (hasX)
            {
                offsetX = coder.readSignedShort();
            }
            if (hasY)
            {
                offsetY = coder.readSignedShort();
            }
            if (hasFont)
            {
                height = coder.readSignedShort();
            }



            int charCount = coder.readByte();

            characters = new List<GlyphIndex>(charCount);

            for (int i = 0; i < charCount; i++)
            {
                characters.Add(new GlyphIndex(coder, context));
            }

            coder.alignToByte();
        }

        /// <summary>
        /// Creates a Text object, specifying the colour and position of the
        /// following Text.
        /// </summary>
        /// <param name="uid">
        ///            the identifier of the font that the text will be rendered in.
        ///            Must be in the range 1..65535. </param>
        /// <param name="aHeight">
        ///            the height of the text in the chosen font. Must be in the
        ///            range 1..65535. </param>
        /// <param name="aColor">
        ///            the colour of the text. </param>
        /// <param name="xOffset">
        ///            the location of the text relative to the left edge of the
        ///            bounding rectangle enclosing the text. </param>
        /// <param name="yOffset">
        ///            the location of the text relative to the bottom edge of the
        ///            bounding rectangle enclosing the text. </param>
        /// <param name="list">
        ///            a list of Character objects. Must not be null. </param>


        public TextSpan(int? uid, int? aHeight, Color aColor, int? xOffset, int? yOffset, IList<GlyphIndex> list)
        {
            Identifier = uid;
            Height = aHeight;
            Color = aColor;
            OffsetX = xOffset;
            OffsetY = yOffset;
            Characters = list;
        }

        /// <summary>
        /// Creates and initialises a TextSpan object using the values copied
        /// from another TextSpan object.
        /// </summary>
        /// <param name="object">
        ///            a TextSpan object from which the values will be
        ///            copied. </param>


        public TextSpan(TextSpan @object)
        {
            identifier = @object.identifier;
            color = @object.color;
            offsetX = @object.offsetX;
            offsetY = @object.offsetY;
            height = @object.height;
            characters = new List<GlyphIndex>(@object.characters);
        }

        /// <summary>
        /// Get the identifier of the font in which the text will be displayed.
        /// </summary>
        /// <returns> the unique identifier of the font. </returns>
        public int? Identifier
        {
            get => identifier;
            set
            {
                if ((value != null) && ((value < 1) || (value > Coder.USHORT_MAX)))
                {
                    throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, value.Value);
                }
                identifier = value;
            }
        }

        /// <summary>
        /// Get the colour used to display the text.
        /// </summary>
        /// <returns> the text colour. </returns>
        public Color Color
        {
            get => color;
            set => color = value;
        }

        /// <summary>
        /// Get the location of the start of the text relative to the left edge
        /// of the bounding rectangle in twips.
        /// </summary>
        /// <returns> the left offset. </returns>
        public int? OffsetX
        {
            get => offsetX;
            set
            {
                if ((value != null) && ((value < Coder.SHORT_MIN) || (value > Coder.SHORT_MAX)))
                {
                    throw new IllegalArgumentRangeException(Coder.SHORT_MIN, Coder.SHORT_MAX, value.Value);
                }
                offsetX = value;
            }
        }

        /// <summary>
        /// Get the location of the start of the text relative to the bottom edge
        /// of the bounding rectangle in twips.
        /// </summary>
        /// <returns> the top offset. </returns>
        public int? OffsetY
        {
            get => offsetY;
            set
            {
                if ((value != null) && ((value < Coder.SHORT_MIN) || (value > Coder.SHORT_MAX)))
                {
                    throw new IllegalArgumentRangeException(Coder.SHORT_MIN, Coder.SHORT_MAX, value.Value);
                }
                offsetY = value;
            }
        }

        /// <summary>
        /// Get the height of the text.
        /// </summary>
        /// <returns> the size, in twips, of the font used to display the text. </returns>
        public int? Height
        {
            get => height;
            set
            {
                if ((value < 0) || (value > Coder.USHORT_MAX))
                {
                    throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value.Value);
                }
                height = value;
            }
        }






        /// <summary>
        /// Adds an Character object to the list of characters.
        /// </summary>
        /// <param name="aCharacter">
        ///            an Character object. Must not be null. </param>
        /// <returns> this object. </returns>


        public TextSpan add(GlyphIndex aCharacter)
        {
            characters.Add(aCharacter);
            return this;
        }

        /// <summary>
        /// Returns the list of characters to be displayed.
        /// </summary>
        /// <returns> the list of Character objects. </returns>
        public IList<GlyphIndex> Characters
        {
            get => characters;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException();
                }
                characters = value;
            }
        }


        /// <summary>
        /// {@inheritDoc} </summary>
        public TextSpan copy()
        {
            return new TextSpan(this);
        }

        public override string ToString()
        {
            return ObjectExtensions.FormatJava(FORMAT, identifier, color, offsetX, offsetY, height, characters);
        }


        /// <summary>
        /// {@inheritDoc} </summary>
        


        public int prepareToEncode(Context context)
        {
            // CHECKSTYLE:OFF
            hasFont = (identifier != null) && (height != null);
            hasColor = color != null;
            hasX = offsetX != null;
            hasY = offsetY != null;
            hasStyle = hasFont || hasColor || hasX || hasY;

            int length = 1;
            if (hasStyle)
            {
                length += (hasFont) ? 2 : 0;
                length += (hasColor) ? (context.contains(Context.TRANSPARENT) ? 4 : 3) : 0;
                length += (hasY) ? 2 : 0;
                length += (hasX) ? 2 : 0;
                length += (hasFont) ? 2 : 0;
            }

            length += 1;

            if (characters.Count > 0)
            {


                int glyphSize = context.get(Context.GLYPH_SIZE);


                int advanceSize = context.get(Context.ADVANCE_SIZE);

                int numberOfBits = (glyphSize + advanceSize) * characters.Count;
                numberOfBits += (numberOfBits % 8 > 0) ? 8 - (numberOfBits % 8) : 0;

                length += numberOfBits >> 3;
            }
            return length;
            // CHECKSTYLE:ON
        }


        /// <summary>
        /// {@inheritDoc} </summary>
        



        public void encode(SWFEncoder coder, Context context)
        {

            int bits = Coder.BIT7;
            bits |= hasFont ? Coder.BIT3 : 0;
            bits |= hasColor ? Coder.BIT2 : 0;
            bits |= hasY ? Coder.BIT1 : 0;
            bits |= hasX ? Coder.BIT0 : 0;
            coder.writeByte(bits);

            if (hasStyle)
            {
                if (hasFont)
                {
                    coder.writeShort(identifier.Value);
                }

                if (hasColor)
                {
                    color.encode(coder, context);
                }

                if (hasX)
                {
                    coder.writeShort(offsetX.Value);
                }

                if (hasY)
                {
                    coder.writeShort(offsetY.Value);
                }

                if (hasFont)
                {
                    coder.writeShort(height.Value);
                }
            }

            coder.writeByte(characters.Count);

            foreach (GlyphIndex index in characters)
            {
                index.encode(coder, context);
            }

            coder.alignToByte();
        }

        /// <summary>
        /// The number of bits used to encode the glyph indices. </summary>
        /// <returns> the number of bits used to encode each glyph index. </returns>
        internal int glyphBits()
        {
            int numberOfBits = 0;

            foreach (GlyphIndex index in characters)
            {
                numberOfBits = Math.Max(numberOfBits, Coder.unsignedSize(index.getGlyphIndex()));
            }

            return numberOfBits;
        }

        /// <summary>
        /// The number of bits used to encode the advances. </summary>
        /// <returns> the number of bits used to encode each advance. </returns>
        internal int advanceBits()
        {
            int numberOfBits = 0;

            foreach (GlyphIndex index in characters)
            {
                numberOfBits = Math.Max(numberOfBits, Coder.size(index.Advance));
            }

            return numberOfBits;
        }
    }

}