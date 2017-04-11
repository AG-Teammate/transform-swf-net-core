using System;
using System.Collections.Generic;
using System.Reflection;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;
using com.flagstone.transform.fillstyle;
using com.flagstone.transform.linestyle;

/*
 * DefineMorphShape.java
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
    /// DefineMorphShape defines a shape that will morph from one form into another.
    /// 
    /// <para>
    /// Only the start and end shapes are defined the Flash Player will perform the
    /// interpolation that transforms the shape at each staging in the morphing
    /// process.
    /// </para>
    /// 
    /// <para>
    /// Morphing can be applied to any shape, however there are a few restrictions:
    /// </para>
    /// 
    /// <ul>
    /// <li>The start and end shapes must have the same number of edges (Line and
    /// Curve objects).</li>
    /// <li>The fill style (Solid, Bitmap or Gradient) must be the same in the start
    /// and end shape.</li>
    /// <li>If a bitmap fill style is used then the same image must be used in the
    /// start and end shapes.</li>
    /// <li>If a gradient fill style is used then the gradient must contain the same
    /// number of points in the start and end shape.</li>
    /// <li>Start and end shape must contain the same set of ShapeStyle objects.</li>
    /// </ul>
    /// 
    /// <para>
    /// To perform the morphing of a shape the shape is placed in the display list
    /// using a PlaceObject2 object. The ratio attribute in the PlaceObject2 object
    /// defines the progress of the morphing process. The ratio ranges between 0 and
    /// 65535 where 0 represents the start of the morphing process and 65535, the
    /// end.
    /// </para>
    /// 
    /// <para>
    /// The edges in the shapes may change their type when a shape is morphed.
    /// Straight edges can become curves and vice versa.
    /// </para>
    /// </summary>
    

    public sealed class DefineMorphShape : ShapeTag
    {

        /// <summary>
        /// Reserved length for style counts indicated that the number of line
        /// or fill styles is encoded in the next 16-bit word.
        /// </summary>
        private const int EXTENDED = 255;
        /// <summary>
        /// Number of bytes used to encode the number of styles. </summary>
        private const int EXTENDED_LENGTH = 3;


        /// <summary>
        /// Format string used in toString() method. </summary>
        private const string FORMAT = "DefineMorphShape: { identifier=%d;" + " startBounds=%s; endBounds=%s; fillStyles=%s; lineStyles=%s;" + " startShape=%s; endShape=%s}";

        /// <summary>
        /// The unique identifier for this object. </summary>
        private int identifier;
        /// <summary>
        /// The bounding box for the shape at the start of the morphing process. </summary>
        private Bounds bounds;
        /// <summary>
        /// The bounding box for the shape at the end of the morphing process. </summary>
        private Bounds endBounds;

        /// <summary>
        /// The list of fill styles for the shape. </summary>
        private IList<FillStyle> fillStyles;
        /// <summary>
        /// The list of line styles for the shape. </summary>
        private IList<LineStyle> lineStyles;
        /// <summary>
        /// The shape at the start of the morphing process. </summary>
        private Shape shape;
        /// <summary>
        /// The shape at the end of the morphing process. </summary>
        private Shape endShape;

        /// <summary>
        /// The length of the object, minus the header, when it is encoded. </summary>

        private int length;
        /// <summary>
        /// The number of bits to encode indices into the fill style list. </summary>

        private int fillBits;
        /// <summary>
        /// The number of bits to encode indices into the line style list. </summary>

        private int lineBits;
        /// <summary>
        /// Offset in bytes to the end shape. </summary>

        private int offset;

        /// <summary>
        /// Creates and initialises a DefineMorphShape object using values encoded
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




        public DefineMorphShape(SWFDecoder coder, Context context)
        {
            length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
            if (length == Coder.IS_EXTENDED)
            {
                length = coder.readInt();
            }
            coder.mark();
            coder.mark();
            context.put(Context.TRANSPARENT, 1);
            context.put(Context.ARRAY_EXTENDED, 1);
            context.put(Context.TYPE, MovieTypes.DEFINE_MORPH_SHAPE);

            identifier = coder.readUnsignedShort();

            bounds = new Bounds(coder);
            endBounds = new Bounds(coder);
            fillStyles = new List<FillStyle>();
            lineStyles = new List<LineStyle>();



            int offsetToEnd = coder.readInt();
            coder.mark();

            int fillStyleCount = coder.readByte();

            if (context.contains(Context.ARRAY_EXTENDED) && (fillStyleCount == EXTENDED))
            {
                fillStyleCount = coder.readUnsignedShort();
            }



            SWFFactory<FillStyle> decoder = context.Registry.MorphFillStyleDecoder;

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
                lineStyles.Add(new MorphLineStyle(coder, context));
            }

            int skipLimit;

            if (context.Registry.ShapeDecoder == null)
            {
                skipLimit = 37;

                int size;
                if (offsetToEnd == 0)
                {
                    size = 0;
                }
                else
                {
                    size = offsetToEnd - coder.bytesRead();
                }
                coder.unmark();

                shape = new Shape();
                shape.add(new ShapeData(size, coder));

                if (offsetToEnd == 0)
                {
                    size = 0;
                }
                else
                {
                    size = length - coder.bytesRead();
                }
                coder.unmark();

                endShape = new Shape();
                endShape.add(new ShapeData(size, coder));
            }
            else
            {
                skipLimit = 33;

                shape = new Shape(coder, context);
                endShape = new Shape(coder, context);
                coder.unmark();
                coder.unmark();
            }

            context.remove(Context.TRANSPARENT);
            context.put(Context.ARRAY_EXTENDED, 1);
            context.remove(Context.TYPE);

            // known bug - empty objects may be added to Flash file.
            // CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 2 LINES
            if (length - coder.bytesRead() == skipLimit)
            {
                coder.skip(skipLimit);
            }
            else
            {
                coder.check(length);
            }
            coder.unmark();
        }

        /// <summary>
        /// Creates a DefineMorphShape object.
        /// </summary>
        /// <param name="uid">
        ///            an unique identifier for this object. Must be in the range
        ///            1..65535. </param>
        /// <param name="startRect">
        ///            the bounding rectangle enclosing the start shape. Must not be
        ///            null. </param>
        /// <param name="endRect">
        ///            the bounding rectangle enclosing the end shape. Must not be
        ///            null. </param>
        /// <param name="fills">
        ///            a list of MorphSolidFill, MorphBitmapFill and
        ///            MorphGradientFill objects. Must not be null. </param>
        /// <param name="lines">
        ///            a list of MorphLineStyle objects. Must not be null. </param>
        /// <param name="initialShape">
        ///            the shape at the start of the morphing process. Must not be
        ///            null. </param>
        /// <param name="finalShape">
        ///            the shape at the end of the morphing process. Must not be
        ///            null. </param>


        public DefineMorphShape(int uid, Bounds startRect, Bounds endRect, IList<FillStyle> fills, IList<LineStyle> lines, Shape initialShape, Shape finalShape)
        {
            Identifier = uid;
            Bounds = startRect;
            EndBounds = endRect;
            FillStyles = fills;
            LineStyles = lines;
            Shape = initialShape;
            EndShape = finalShape;
        }

        /// <summary>
        /// Creates and initialises a DefineMorphShape object using the values copied
        /// from another DefineMorphShape object.
        /// </summary>
        /// <param name="object">
        ///            a DefineMorphShape object from which the values will be
        ///            copied. </param>


        public DefineMorphShape(DefineMorphShape @object)
        {
            identifier = @object.identifier;
            bounds = @object.bounds;
            endBounds = @object.endBounds;
            fillStyles = new List<FillStyle>(@object.fillStyles.Count);
            foreach (FillStyle style in @object.fillStyles)
            {
                var method = style.GetType().GetTypeInfo().GetDeclaredMethod("copy");
                var copy = method.Invoke(style, null) as FillStyle;
                fillStyles.Add(copy);
            }
            lineStyles = new List<LineStyle>(@object.lineStyles.Count);
            foreach (LineStyle style in @object.lineStyles)
            {
                var method = style.GetType().GetTypeInfo().GetDeclaredMethod("copy");
                var copy = method.Invoke(style, null) as LineStyle;
                lineStyles.Add(copy);
            }
            shape = @object.shape.copy();
            endShape = @object.endShape.copy();
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
        /// Add a LineStyle object to the list of line styles.
        /// </summary>
        /// <param name="style">
        ///            a MorphLineStyle object. Must not be null. Must be an instance
        ///            of MorphLineStyle.
        /// </param>
        /// <returns> this object. </returns>


        public ShapeTag add(LineStyle style)
        {
            if (!(style is MorphLineStyle))
            {
                throw new ArgumentException();
            }
            lineStyles.Add(style);
            return this;
        }

        /// <summary>
        /// Add the fill style object to the list of fill styles.
        /// </summary>
        /// <param name="aFillStyle">
        ///            an FillStyle object. Must not be null.
        /// </param>
        /// <returns> this object. </returns>


        public ShapeTag add(FillStyle aFillStyle)
        {
            fillStyles.Add(aFillStyle);
            return this;
        }

        /// <summary>
        /// Get the Bounds object that defines the bounding rectangle enclosing
        /// the start shape.
        /// </summary>
        /// <returns> the bounding box for the starting shape. </returns>
        public Bounds Bounds
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
        /// Get the Bounds object that defines the bounding rectangle enclosing
        /// the end shape.
        /// </summary>
        /// <returns> the bounding box for the final shape. </returns>
        public Bounds EndBounds
        {
            get => endBounds;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException();
                }
                endBounds = value;
            }
        }

        /// <summary>
        /// Get the list of fill styles (MorphSolidFill, MorphBitmapFill and
        /// MorphGradientFill objects) for the shapes.
        /// </summary>
        /// <returns> the list of fill styles used in the shape. </returns>
        public IList<FillStyle> FillStyles
        {
            get => fillStyles;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException();
                }
                fillStyles = value;
            }
        }

        /// <summary>
        /// Get the list of line styles (MorphLineStyle objects) for the shapes.
        /// </summary>
        /// <returns> the list of line styles used in the shape. </returns>
        public IList<LineStyle> LineStyles
        {
            get => lineStyles;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException();
                }
                lineStyles = value;
            }
        }

        /// <summary>
        /// Get shape displayed at the start of the morphing process.
        /// </summary>
        /// <returns> the starting shape. </returns>
        public Shape Shape
        {
            get => shape;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException();
                }
                shape = value;
            }
        }

        /// <summary>
        /// Get shape displayed at the end of the morphing process.
        /// </summary>
        /// <returns> the final shape. </returns>
        public Shape EndShape
        {
            get => endShape;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException();
                }
                endShape = value;
            }
        }







        /// <summary>
        /// {@inheritDoc} </summary>
        public DefineMorphShape copy()
        {
            return new DefineMorphShape(this);
        }

        public override string ToString()
        {
            return ObjectExtensions.FormatJava(FORMAT, identifier, bounds, endBounds, fillStyles, lineStyles, shape, endShape);
        }


        /// <summary>
        /// {@inheritDoc} </summary>
        


        public int prepareToEncode(Context context)
        {
            fillBits = Coder.unsignedSize(fillStyles.Count);
            lineBits = Coder.unsignedSize(lineStyles.Count);

            if (context.contains(Context.POSTSCRIPT))
            {
                if (fillBits == 0)
                {
                    fillBits = 1;
                }

                if (lineBits == 0)
                {
                    lineBits = 1;
                }
            }

            context.put(Context.TRANSPARENT, 1);

            // CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
            length = 6 + bounds.prepareToEncode(context);
            length += endBounds.prepareToEncode(context);
            offset = length;

            length += (fillStyles.Count >= EXTENDED) ? EXTENDED_LENGTH : 1;

            foreach (FillStyle style in fillStyles)
            {
                length += style.prepareToEncode(context);
            }

            length += (lineStyles.Count >= EXTENDED) ? EXTENDED_LENGTH : 1;

            foreach (LineStyle style in lineStyles)
            {
                length += style.prepareToEncode(context);
            }

            context.put(Context.ARRAY_EXTENDED, 1);
            context.put(Context.FILL_SIZE, fillBits);
            context.put(Context.LINE_SIZE, lineBits);

            length += shape.prepareToEncode(context);
            offset = length - offset;
            // Number of Fill and Line bits is zero for end shape.
            context.put(Context.FILL_SIZE, 0);
            context.put(Context.LINE_SIZE, 0);

            length += endShape.prepareToEncode(context);

            context.remove(Context.ARRAY_EXTENDED);
            context.remove(Context.TRANSPARENT);

            return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
        }


        /// <summary>
        /// {@inheritDoc} </summary>



        public void encode(SWFEncoder coder, Context context)
        {

            if (length > Coder.HEADER_LIMIT)
            {
                coder.writeShort((MovieTypes.DEFINE_MORPH_SHAPE << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
                coder.writeInt(length);
            }
            else
            {
                coder.writeShort((MovieTypes.DEFINE_MORPH_SHAPE << Coder.LENGTH_FIELD_SIZE) | length);
            }
            if (Constants.DEBUG)
            {
                coder.mark();
            }
            coder.writeShort(identifier);
            context.put(Context.TRANSPARENT, 1);

            bounds.encode(coder, context);
            endBounds.encode(coder, context);
            coder.writeInt(offset);

            if (fillStyles.Count >= EXTENDED)
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

            if (lineStyles.Count >= EXTENDED)
            {
                coder.writeByte(EXTENDED);
                coder.writeShort(lineStyles.Count);
            }
            else
            {
                coder.writeByte(lineStyles.Count);
            }

            foreach (LineStyle style in lineStyles)
            {
                style.encode(coder, context);
            }

            context.put(Context.ARRAY_EXTENDED, 1);
            context.put(Context.FILL_SIZE, fillBits);
            context.put(Context.LINE_SIZE, lineBits);

            shape.encode(coder, context);

            // Number of Fill and Line bits is zero for end shape.
            context.put(Context.FILL_SIZE, 0);
            context.put(Context.LINE_SIZE, 0);

            endShape.encode(coder, context);

            context.remove(Context.ARRAY_EXTENDED);
            context.remove(Context.TRANSPARENT);
            if (Constants.DEBUG)
            {
                coder.check(length);
                coder.unmark();
            }
        }
    }

}