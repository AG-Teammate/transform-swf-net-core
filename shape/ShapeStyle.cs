using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;
using com.flagstone.transform.fillstyle;
using com.flagstone.transform.linestyle;

/*
 * ShapeStyle.java
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

namespace com.flagstone.transform.shape
{
    /// <summary>
    /// ShapeStyle is used to change the drawing environment when a shape is drawn.
    /// Three operations can be performed:
    /// 
    /// <ul>
    /// <li>Select a line style or fill style.</li>
    /// <li>Move the current drawing point.</li>
    /// <li>Define a new set of line and fill styles.</li>
    /// </ul>
    /// 
    /// <para>
    /// An ShapeStyle object can specify one or more of the operations rather than
    /// specifying them in separate ShapeStyle objects - compacting the size of the
    /// binary data when the object is encoded. Conversely if an operation is not
    /// defined then the values may be omitted.
    /// </para>
    /// 
    /// <para>
    /// Line and Fill styles are selected by the index position, starting at 1, of
    /// the style in a list of styles. An index of zero means that no style is
    /// used. Two types of fill style are supported: fillStyle is used where a
    /// shape does not contain overlapping areas and altFillStyle is used where areas
    /// overlap. This differs from graphics environments that only support one fill
    /// style as the overlapping area would form a hole in the shape and not be
    /// filled.
    /// </para>
    /// 
    /// <para>
    /// A new drawing point is specified using the absolute x and y coordinates. If
    /// an ShapeStyle object is the first in a shape then the current drawing point
    /// is the origin of the shape (0,0). As with the line and fill styles,
    /// specifying a move is optional.
    /// </para>
    /// 
    /// <para>
    /// Finally the line or fill style lists may left empty if no new styles are
    /// being specified.
    /// </para>
    /// </summary>
    

    public sealed class ShapeStyle : ShapeRecord
    {

        /// <summary>
        /// Reserved length for style counts indicated that the number of line
        /// or fill styles is encoded in the next 16-bit word.
        /// </summary>
        private const int EXTENDED = 255;

        /// <summary>
        /// Format string used in toString() method. </summary>
        private const string FORMAT = "ShapeStyle: { move=(%d, %d);" + " fill=%d; alt=%d; line=%d; fillStyles=%s; lineStyles=%s}";

        /// <summary>
        /// Relative move along the x-axis. </summary>
        private int? moveX;
        /// <summary>
        /// Relative move along the y-axis. </summary>
        private int? moveY;
        /// <summary>
        /// Selected fill style. </summary>
        private int? fillStyle;
        /// <summary>
        /// Selected alternate fill style. </summary>
        private int? altFillStyle;
        /// <summary>
        /// Selected line style. </summary>
        private int? lineStyle;
        /// <summary>
        /// List of fill styles. </summary>
        private IList<FillStyle> fillStyles;
        /// <summary>
        /// List of line styles. </summary>
        private IList<LineStyle1> lineStyles;

        /// <summary>
        /// Indicates whether new line or fill styles are specified. </summary>

        private bool hasStyles;
        /// <summary>
        /// Indicates whether a line is specified. </summary>

        private bool hasLine;
        /// <summary>
        /// Indicates whether an alternate fill style is specified. </summary>

        private bool hasAlt;
        /// <summary>
        /// Indicates whether an fill style is specified. </summary>

        private bool hasFill;
        /// <summary>
        /// Indicates whether a relative move is specified. </summary>

        private bool hasMove;

        /// <summary>
        /// Creates and initialises a ShapeStyle object using values encoded
        /// in the Flash binary format.
        /// </summary>
        /// <param name="flags">
        ///            contains fields identifying which fields are optionally
        ///            encoded in the data - decoded by parent object. </param>
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




        public ShapeStyle(int flags, SWFDecoder coder, Context context)
        {
            int numberOfFillBits = context.get(Context.FILL_SIZE);
            int numberOfLineBits = context.get(Context.LINE_SIZE);

            hasStyles = (flags & Coder.BIT4) != 0;
            hasLine = (flags & Coder.BIT3) != 0;
            hasAlt = (flags & Coder.BIT2) != 0;
            hasFill = (flags & Coder.BIT1) != 0;
            hasMove = (flags & Coder.BIT0) != 0;

            if (hasMove)
            {


                int moveFieldSize = coder.readBits(5, false);
                moveX = coder.readBits(moveFieldSize, true);
                moveY = coder.readBits(moveFieldSize, true);
            }
            fillStyles = new List<FillStyle>();
            lineStyles = new List<LineStyle1>();

            if (hasFill)
            {
                fillStyle = coder.readBits(numberOfFillBits, false);
            }
            if (hasAlt)
            {
                altFillStyle = coder.readBits(numberOfFillBits, false);
            }
            if (hasLine)
            {
                lineStyle = coder.readBits(numberOfLineBits, false);
            }

            if (hasStyles)
            {
                coder.alignToByte();

                int fillStyleCount = coder.readByte();

                if (context.contains(Context.ARRAY_EXTENDED) && (fillStyleCount == EXTENDED))
                {
                    fillStyleCount = coder.readUnsignedShort();
                }



                SWFFactory<FillStyle> decoder = context.Registry.FillStyleDecoder;

                for (int i = 0; i < fillStyleCount; i++)
                {
                    decoder.getObject(fillStyles, coder, context);
                }

                int lineStyleCount = coder.readByte();

                if (context.contains(Context.ARRAY_EXTENDED) && (lineStyleCount == EXTENDED))
                {
                    lineStyleCount = coder.readUnsignedShort();
                }

                for (int i = 0; i < lineStyleCount; i++)
                {
                    lineStyles.Add(new LineStyle1(coder, context));
                }



                int sizes = coder.readByte();
                numberOfFillBits = (sizes & Coder.NIB1) >> Coder.TO_LOWER_NIB;
                numberOfLineBits = sizes & Coder.NIB0;

                context.put(Context.FILL_SIZE, numberOfFillBits);
                context.put(Context.LINE_SIZE, numberOfLineBits);
            }
        }

        /// <summary>
        /// Creates an uninitialised ShapeStyle object.
        /// </summary>
        public ShapeStyle()
        {
            fillStyles = new List<FillStyle>();
            lineStyles = new List<LineStyle1>();
        }

        /// <summary>
        /// Creates and initialises a ShapeStyle object using the values copied
        /// from another ShapeStyle object.
        /// </summary>
        /// <param name="object">
        ///            a ShapeStyle object from which the values will be
        ///            copied. </param>


        public ShapeStyle(ShapeStyle @object)
        {
            moveX = @object.moveX;
            moveY = @object.moveY;
            lineStyle = @object.lineStyle;
            fillStyle = @object.fillStyle;
            altFillStyle = @object.altFillStyle;

            lineStyles = new List<LineStyle1>(@object.lineStyles.Count);

            foreach (LineStyle1 style in @object.lineStyles)
            {
                lineStyles.Add(style.copy());
            }

            fillStyles = new List<FillStyle>(@object.fillStyles.Count);

            foreach (FillStyle style in @object.fillStyles)
            {
                fillStyles.Add(style.copy());
            }
        }

        /// <summary>
        /// Add a LineStyle object to the list of line styles.
        /// </summary>
        /// <param name="style">
        ///            and LineStyle object. Must not be null. </param>
        /// <returns> this object. </returns>


        public ShapeStyle add(LineStyle1 style)
        {
            if (style == null)
            {
                throw new ArgumentException();
            }
            lineStyles.Add(style);
            return this;
        }

        /// <summary>
        /// Add the fill style object to the list of fill styles.
        /// </summary>
        /// <param name="style">
        ///            and FillStyle object. Must not be null. </param>
        /// <returns> this object. </returns>


        public ShapeStyle add(FillStyle style)
        {
            if (style == null)
            {
                throw new ArgumentException();
            }
            fillStyles.Add(style);
            return this;
        }

        /// <summary>
        /// Get the x-coordinate of any relative move or null if no move is
        /// specified.
        /// </summary>
        /// <returns> the relative move in the x direction. </returns>
        public int? MoveX => moveX;

        /// <summary>
        /// Get the y-coordinate of any relative move or null if no move is
        /// specified.
        /// </summary>
        /// <returns> the relative move in the y direction. </returns>
        public int? MoveY => moveY;

        /// <summary>
        /// Get the index of the line style that will be applied to any line
        /// drawn. Returns null if no line style is defined.
        /// </summary>
        /// <returns> the selected line style. </returns>
        public int? LineStyle
        {
            get => lineStyle;
            set => lineStyle = value;
        }

        /// <summary>
        /// Get the index of the fill style that will be applied to any area
        /// filled. Returns null if no fill style is defined.
        /// </summary>
        /// <returns> the selected fill style. </returns>
        public int? FillStyle
        {
            get => fillStyle;
            set => fillStyle = value;
        }

        /// <summary>
        /// Get the index of the fill style that will be applied to any
        /// overlapping area filled. Returns null if no alternate fill style is
        /// defined.
        /// </summary>
        /// <returns> the selected alternate fill style. </returns>
        public int? AltFillStyle => altFillStyle;

        /// <summary>
        /// Get the list of new line styles.
        /// </summary>
        /// <returns> the list of line styles. </returns>
        public IList<LineStyle1> LineStyles => lineStyles;

        /// <summary>
        /// Returns the list of new fill styles.
        /// </summary>
        /// <returns> the list of fill styles. </returns>
        public IList<FillStyle> FillStyles => fillStyles;

        /// <summary>
        /// Sets the x-coordinate of any relative move.
        /// </summary>
        /// <param name="coord">
        ///            move the current point by aNumber in the x direction. Must be
        ///            in the range -65535..65535. </param>
        /// <returns> this object. </returns>


        public ShapeStyle setMoveX(int? coord)
        {
            if ((coord != null) && ((coord < Shape.MIN_COORD) || (coord > Shape.MAX_COORD)))
            {
                throw new IllegalArgumentRangeException(Shape.MIN_COORD, Shape.MAX_COORD, coord.Value);
            }
            moveX = coord;
            return this;
        }

        /// <summary>
        /// Sets the x-coordinate of any relative move.
        /// </summary>
        /// <param name="coord">
        ///            move the current point by aNumber in the x direction. Must be
        ///            in the range -65535..65535. </param>
        /// <returns> this object. </returns>


        public ShapeStyle setMoveY(int? coord)
        {
            if ((coord != null) && ((coord < Shape.MIN_COORD) || (coord > Shape.MAX_COORD)))
            {
                throw new IllegalArgumentRangeException(Shape.MIN_COORD, Shape.MAX_COORD, coord.Value);
            }
            moveY = coord;
            return this;
        }

        /// <summary>
        /// Sets the coordinates of any relative move.
        /// </summary>
        /// <param name="xCoord">
        ///            move the current point by aNumber in the x direction. Must be
        ///            in the range -65535..65535.
        /// </param>
        /// <param name="yCoord">
        ///            move the current point by aNumber in the y direction. Must be
        ///            in the range -65535..65535. </param>
        /// <returns> this object. </returns>


        public ShapeStyle setMove(int? xCoord, int? yCoord)
        {
            if (((xCoord == null) && (yCoord != null)) || ((xCoord != null) && (yCoord == null)))
            {
                throw new ArgumentException();
            }
            if ((xCoord != null) && ((xCoord < Shape.MIN_COORD) || (xCoord > Shape.MAX_COORD)))
            {
                throw new IllegalArgumentRangeException(Shape.MIN_COORD, Shape.MAX_COORD, xCoord.Value);
            }
            if ((yCoord != null) && ((yCoord < Shape.MIN_COORD) || (yCoord > Shape.MAX_COORD)))
            {
                throw new IllegalArgumentRangeException(Shape.MIN_COORD, Shape.MAX_COORD, yCoord.Value);
            }
            moveX = xCoord;
            moveY = yCoord;
            return this;
        }

        /// <summary>
        /// Sets the index of the fill style that will be applied to any area filled.
        /// May be set to zero if no style is selected or null if the line style
        /// remains unchanged.
        /// </summary>
        /// <param name="anIndex">
        ///            selects the fill style at anIndex in the fill styles list of
        ///            the parent Shape object. </param>
        /// <returns> this object. </returns>


        public ShapeStyle setFillStyle(int? anIndex)
        {
            fillStyle = anIndex;
            return this;
        }

        /// <summary>
        /// Sets the index of the fill style that will be applied to any overlapping
        /// area filled. May be set to zero if no style is selected or null if the ~
        /// line style remains unchanged.
        /// </summary>
        /// <param name="anIndex">
        ///            selects the alternate fill style at anIndex in the fill styles
        ///            list of the parent Shape object. </param>
        /// <returns> this object. </returns>


        public ShapeStyle setAltFillStyle(int? anIndex)
        {
            altFillStyle = anIndex;
            return this;
        }

        /// <summary>
        /// Sets the index of the line style that will be applied to any line drawn.
        /// May be set to zero if no style is selected or null if the line style
        /// remains unchanged.
        /// </summary>
        /// <param name="anIndex">
        ///            selects the line style at anIndex in the line styles list of
        ///            the parent Shape object. </param>
        /// <returns> this object. </returns>


        public ShapeStyle setLineStyle(int? anIndex)
        {
            lineStyle = anIndex;
            return this;
        }

        /// <summary>
        /// Sets the list of new line styles. May be set to null if no styles are
        /// being defined.
        /// </summary>
        /// <param name="list">
        ///            a list of LineStyle objects. Must not be null. </param>
        /// <returns> this object. </returns>


        public ShapeStyle setLineStyles(IList<LineStyle1> list)
        {
            if (list == null)
            {
                throw new ArgumentException();
            }
            lineStyles = list;
            return this;
        }

        /// <summary>
        /// Sets the list of new fill styles. May be set to null if no styles are
        /// being defined.
        /// </summary>
        /// <param name="list">
        ///            a list of fill style objects. Must not be null. </param>
        /// <returns> this object. </returns>


        public ShapeStyle setFillStyles(IList<FillStyle> list)
        {
            if (list == null)
            {
                throw new ArgumentException();
            }
            fillStyles = list;
            return this;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public ShapeStyle copy()
        {
            return new ShapeStyle(this);
        }

        public override string ToString()
        {
            return ObjectExtensions.FormatJava(FORMAT, moveX, moveY, fillStyle, altFillStyle, lineStyle, fillStyles, lineStyles);
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        


        public int prepareToEncode(Context context)
        {
            // CHECKSTYLE:OFF
            hasLine = lineStyle != null;
            hasFill = fillStyle != null;
            hasAlt = altFillStyle != null;
            hasMove = (moveX != null) && (moveY != null);
            hasStyles = lineStyles.Count > 0 || fillStyles.Count > 0;

            int numberOfBits = 6;

            if (hasMove)
            {


                int fieldSize = Math.Max(Coder.size(moveX.Value), Coder.size(moveY.Value));
                numberOfBits += 5 + fieldSize * 2;
            }

            numberOfBits += hasFill ? context.get(Context.FILL_SIZE) : 0;
            numberOfBits += hasAlt ? context.get(Context.FILL_SIZE) : 0;
            numberOfBits += (hasLine) ? context.get(Context.LINE_SIZE) : 0;

            context.put(Context.SHAPE_SIZE, context.get(Context.SHAPE_SIZE) + numberOfBits);

            if (hasStyles)
            {
                int numberOfFillBits = Coder.unsignedSize(fillStyles.Count);
                int numberOfLineBits = Coder.unsignedSize(lineStyles.Count);

                if ((numberOfFillBits == 0) && context.contains(Context.POSTSCRIPT))
                {
                    numberOfFillBits = 1;
                }

                if ((numberOfLineBits == 0) && context.contains(Context.POSTSCRIPT))
                {
                    numberOfLineBits = 1;
                }



                bool countExtended = context.contains(Context.ARRAY_EXTENDED);

                int numberOfStyleBits = 0;


                int flushBits = context.get(Context.SHAPE_SIZE);

                numberOfStyleBits += (flushBits % 8 > 0) ? 8 - (flushBits % 8) : 0;
                numberOfStyleBits += (countExtended && (fillStyles.Count >= EXTENDED)) ? 24 : 8;

                foreach (FillStyle style in fillStyles)
                {
                    numberOfStyleBits += style.prepareToEncode(context) << 3;
                }

                numberOfStyleBits += (countExtended && (lineStyles.Count >= EXTENDED)) ? 24 : 8;

                foreach (LineStyle1 style in lineStyles)
                {
                    numberOfStyleBits += style.prepareToEncode(context) << 3;
                }

                numberOfStyleBits += 8;

                context.put(Context.FILL_SIZE, numberOfFillBits);
                context.put(Context.LINE_SIZE, numberOfLineBits);
                context.put(Context.SHAPE_SIZE, context.get(Context.SHAPE_SIZE) + numberOfStyleBits);

                numberOfBits += numberOfStyleBits;
            }
            return numberOfBits;
            // CHECKSTYLE:ON
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        



        public void encode(SWFEncoder coder, Context context)
        {
            coder.writeBits(0, 1);
            coder.writeBits(hasStyles ? 1 : 0, 1);
            coder.writeBits(hasLine ? 1 : 0, 1);
            coder.writeBits(hasAlt ? 1 : 0, 1);
            coder.writeBits(hasFill ? 1 : 0, 1);
            coder.writeBits(hasMove ? 1 : 0, 1);

            if (hasMove)
            {


                int fieldSize = Math.Max(Coder.size(moveX.Value), Coder.size(moveY.Value));

                // CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
                coder.writeBits(fieldSize, 5);
                coder.writeBits(moveX.Value, fieldSize);
                coder.writeBits(moveY.Value, fieldSize);
            }

            if (hasFill)
            {
                coder.writeBits(fillStyle.Value, context.get(Context.FILL_SIZE));
            }

            if (hasAlt)
            {
                coder.writeBits(altFillStyle.Value, context.get(Context.FILL_SIZE));
            }

            if (hasLine)
            {
                coder.writeBits(lineStyle.Value, context.get(Context.LINE_SIZE));
            }

            if (hasStyles)
            {


                bool countExtended = context.contains(Context.ARRAY_EXTENDED);

                coder.alignToByte();

                if (countExtended && (fillStyles.Count >= EXTENDED))
                {
                    coder.writeByte(EXTENDED);
                    coder.writeShort(fillStyles.Count);
                }
                else
                {
                    coder.writeByte(fillStyles.Count);
                }

                foreach (FillStyle style in fillStyles)
                {
                    style.encode(coder, context);
                }

                if (countExtended && (lineStyles.Count >= EXTENDED))
                {
                    coder.writeByte(EXTENDED);
                    coder.writeShort(lineStyles.Count);
                }
                else
                {
                    coder.writeByte(lineStyles.Count);
                }

                foreach (LineStyle1 style in lineStyles)
                {
                    style.encode(coder, context);
                }

                int numberOfFillBits = Coder.unsignedSize(fillStyles.Count);
                int numberOfLineBits = Coder.unsignedSize(lineStyles.Count);

                if (context.contains(Context.POSTSCRIPT))
                {
                    if (numberOfFillBits == 0)
                    {
                        numberOfFillBits = 1;
                    }

                    if (numberOfLineBits == 0)
                    {
                        numberOfLineBits = 1;
                    }
                }

                coder.writeByte((numberOfFillBits << Coder.TO_UPPER_NIB) | numberOfLineBits);

                // Update the stream with the new numbers of line and fill bits
                context.put(Context.FILL_SIZE, numberOfFillBits);
                context.put(Context.LINE_SIZE, numberOfLineBits);
            }
        }
    }

}