using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;
using com.flagstone.transform.filter;

/*
 * Button.java
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

namespace com.flagstone.transform.button
{
    /// <summary>
    /// <para>
    /// ButtonShape identifies the shape that is drawn when a button is in a
    /// particular state. Shapes can be drawn for each of three button states, Over,
    /// Up and Down allowing simple animations to be created when a button is
    /// clicked.
    /// </para>
    /// 
    /// <para>
    /// A shape is also used to define active area of the button. When defining the
    /// active area the outline of the shape defines the boundary of the area, the
    /// shape itself is not displayed. The button will only respond to mouse events
    /// when the cursor is placed inside the active area.
    /// </para>
    /// 
    /// <para>
    /// The order in which shapes are displayed is controlled by the layer number. As
    /// with the Flash Player's display list shapes on a layer with a higher number
    /// are displayed in front of ones on a layer with a lower number. A coordinate
    /// and color transform can also be applied to each shape to change its
    /// appearance when it is displayed when the button enters the specified state.
    /// </para>
    /// </summary>
    /// <seealso cref= DefineButton </seealso>
    /// <seealso cref= DefineButton2 </seealso>
    public sealed class ButtonShape : SWFEncodeable
    {

        /// <summary>
        /// Format string used in toString() method. </summary>
        private const string FORMAT = "ButtonShape: { state=%d;" + " identifier=%d; layer=%d; transform=%s; colorTransform=%s;" + " blend=%s; filters=%s}";

        /// <summary>
        /// The button state that the shape represents. </summary>
        private int state;
        /// <summary>
        /// The unique identifier of the shape that will be displayed. </summary>
        private int identifier;
        /// <summary>
        /// The layer on which the shape is displayed. </summary>
        private int layer;
        /// <summary>
        /// The coordinate transform used to position the shape. </summary>
        private CoordTransform transform;
        /// <summary>
        /// The colour transform applied to the shape. </summary>
        private ColorTransform colorTransform;
        /// <summary>
        /// The set of filters applied to the shape. </summary>
        private IList<Filter> filters;
        /// <summary>
        /// The mode used to blend the shape with its background. </summary>
        private int? blend;

        /// <summary>
        /// Flag used when encoded to identify whether the blend is set. </summary>

        private bool hasBlend;
        /// <summary>
        /// Flag used when encoded to identify whether filters are defined. </summary>

        private bool hasFilters;

        /// <summary>
        /// Creates and initialises a ButtonShape object using values encoded
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



        public ButtonShape(SWFDecoder coder, Context context)
        {



            int bits = coder.readByte();
            hasBlend = (bits & Coder.BIT5) != 0;
            hasFilters = (bits & Coder.BIT4) != 0;
            state = bits & Coder.NIB0;

            identifier = coder.readUnsignedShort();
            layer = coder.readUnsignedShort();
            transform = new CoordTransform(coder);

            if (context.get(Context.TYPE) != null && context.get(Context.TYPE) == MovieTypes.DEFINE_BUTTON_2)
            {
                colorTransform = new ColorTransform(coder, context);
            }

            if (hasFilters)
            {


                SWFFactory<Filter> decoder = context.Registry.FilterDecoder;


                int count = coder.readByte();
                filters = new List<Filter>(count);
                for (int i = 0; i < count; i++)
                {
                    decoder.getObject(filters, coder, context);
                }
            }
            else
            {
                filters = new List<Filter>();
            }

            if (hasBlend)
            {
                blend = coder.readByte();
                if (blend == 0)
                {
                    blend = 1;
                }
            }
            else
            {
                blend = 0;
            }
        }

        /// <summary>
        /// Creates am uninitialised ButtonShape object.
        /// </summary>
        public ButtonShape()
        {
            transform = CoordTransform.translate(0, 0);
            colorTransform = new ColorTransform(0, 0, 0, 0);
            filters = new List<Filter>();
            blend = 0;
        }

        /// <summary>
        /// Creates and initialises a ButtonShape object using the values copied
        /// from another ButtonShape object.
        /// </summary>
        /// <param name="object">
        ///            a ButtonShape object from which the values will be
        ///            copied. </param>


        public ButtonShape(ButtonShape @object)
        {
            state = @object.state;
            identifier = @object.identifier;
            layer = @object.layer;
            transform = @object.transform;
            colorTransform = @object.colorTransform;
            filters = new List<Filter>(@object.filters);
            blend = @object.blend;
        }

        /// <summary>
        /// Get the list of states that the shape is displayed for. </summary>
        /// <returns> the list of button states that define when the shape is
        /// displayed. </returns>
        public ISet<ButtonState> State
        {
            get
            {


                ISet<ButtonState> set = new HashSet<ButtonState>();

                if ((state & Coder.BIT0) != 0)
                {
                    set.Add(ButtonState.UP);
                }
                if ((state & Coder.BIT1) != 0)
                {
                    set.Add(ButtonState.OVER);
                }
                if ((state & Coder.BIT2) != 0)
                {
                    set.Add(ButtonState.DOWN);
                }
                if ((state & Coder.BIT3) != 0)
                {
                    set.Add(ButtonState.ACTIVE);
                }
                return set;
            }
        }

        /// <summary>
        /// Set the list of states that the shape is displayed for. </summary>
        /// <param name="states"> the list of button states that define when the shape is
        /// displayed. </param>
        /// <returns> this object. </returns>


        public ButtonShape setState(ISet<ButtonState> states)
        {
            foreach (ButtonState buttonState in states)
            {
                switch (buttonState)
                {
                    case ButtonState.UP:
                        state |= Coder.BIT0;
                        break;
                    case ButtonState.OVER:
                        state |= Coder.BIT1;
                        break;
                    case ButtonState.DOWN:
                        state |= Coder.BIT2;
                        break;
                    case ButtonState.ACTIVE:
                        state |= Coder.BIT3;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            return this;
        }

        /// <summary>
        /// Add the state to the list of states that the shape is displayed for. </summary>
        /// <param name="buttonState"> the state that defines when the shape is displayed. </param>
        /// <returns> this object. </returns>


        public ButtonShape addState(ButtonState buttonState)
        {
            switch (buttonState)
            {
                case ButtonState.UP:
                    state |= Coder.BIT0;
                    break;
                case ButtonState.OVER:
                    state |= Coder.BIT1;
                    break;
                case ButtonState.DOWN:
                    state |= Coder.BIT2;
                    break;
                case ButtonState.ACTIVE:
                    state |= Coder.BIT3;
                    break;
                default:
                    throw new ArgumentException();
            }
            return this;
        }

        /// <summary>
        /// Get the unique identifier of the shape that this Button applies to.
        /// </summary>
        /// <returns> the unique identifier of the shape. </returns>
        public int Identifier => identifier;

        /// <summary>
        /// Sets the unique identifier of the DefineShape, DefineShape2 or
        /// DefineShape3 object that defines the appearance of the button when it is
        /// in the specified state(s).
        /// </summary>
        /// <param name="uid">
        ///            the unique identifier of the shape object that defines the
        ///            shape's appearance. Must be in the range 1..65535. </param>
        /// <returns> this object. </returns>


        public ButtonShape setIdentifier(int uid)
        {
            if ((uid < 1) || (uid > Coder.USHORT_MAX))
            {
                throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, uid);
            }
            identifier = uid;
            return this;
        }

        /// <summary>
        /// Get the layer that the button will be displayed on.
        /// </summary>
        /// <returns> the layer that the shape is displayed on. </returns>
        public int Layer => layer;

        /// <summary>
        /// Sets the layer in the display list that the shape will be displayed on.
        /// </summary>
        /// <param name="aNumber">
        ///            the number of the layer in the display list where the shape is
        ///            drawn. Must be in the range 1..65535. </param>
        /// <returns> this object. </returns>


        public ButtonShape setLayer(int aNumber)
        {
            if ((aNumber < 1) || (aNumber > Coder.USHORT_MAX))
            {
                throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, aNumber);
            }
            layer = aNumber;
            return this;
        }

        /// <summary>
        /// Get the coordinate transform that will be applied to the button.
        /// </summary>
        /// <returns> the coordinate transform that is applied to the shape. </returns>
        public CoordTransform Transform => transform;

        /// <summary>
        /// Sets the coordinate transform that will be applied to the shape to change
        /// it's appearance.
        /// </summary>
        /// <param name="matrix">
        ///            a CoordTransform object that will be applied to the shape.
        ///            Must not be null. </param>
        /// <returns> this object. </returns>


        public ButtonShape setTransform(CoordTransform matrix)
        {
            if (matrix == null)
            {
                throw new ArgumentException();
            }
            transform = matrix;
            return this;
        }

        /// <summary>
        /// Get the colour transform that will be applied to the button.
        /// 
        /// Note that the colour transform will only be used if the ButtonShape is
        /// added to a DefineButton2 object.
        /// </summary>
        /// <returns> the colour transform that is applied to the shape. </returns>
        public ColorTransform ColorTransform => colorTransform;

        /// <summary>
        /// Sets the colour transform that will be applied to the shape to change
        /// it's colour.
        /// 
        /// IMPORTANT: The colour transform is only used in DefineButton2 objects.
        /// </summary>
        /// <param name="cxform">
        ///            a ColorTransform object that will be applied to the shape.
        ///            Must not be null, even if the ButtonShape will be added to a
        ///            DefineButton object. </param>
        /// <returns> this object. </returns>


        public ButtonShape setColorTransform(ColorTransform cxform)
        {
            if (cxform == null)
            {
                throw new ArgumentException();
            }
            colorTransform = cxform;
            return this;
        }

        /// <summary>
        /// Add a Filter to the list of Filters that will be applied to the shape. </summary>
        /// <param name="filter"> a Filter to apply to the button shape. </param>
        /// <returns> this object. </returns>


        public ButtonShape add(Filter filter)
        {
            if (filter == null)
            {
                throw new ArgumentException();
            }
            filters.Add(filter);
            return this;
        }

        /// <summary>
        /// Get the list of Filters that will be applied to the shape. </summary>
        /// <returns> the list of filters. </returns>
        public IList<Filter> Filters => filters;

        /// <summary>
        /// Set the list of Filters that will be applied to the shape. </summary>
        /// <param name="list"> a list of Filter objects. </param>
        /// <returns> this object. </returns>


        public ButtonShape setFilters(IList<Filter> list)
        {
            if (list == null)
            {
                throw new ArgumentException();
            }
            filters = list;
            return this;
        }

        /// <summary>
        /// Get the Blend that defines how the shape is blended with background
        /// shapes that make up the button. </summary>
        /// <returns> the Blend mode. </returns>
        public Blend Blend => Blend.fromInt((int)blend);

        /// <summary>
        /// Set the Blend that defines how the shape is blended with background
        /// shapes that make up the button. </summary>
        /// <param name="mode"> the Blend mode for this shape. </param>
        /// <returns> this object. </returns>


        public ButtonShape setBlend(Blend mode)
        {
            blend = mode.Value;
            return this;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public ButtonShape copy()
        {
            return new ButtonShape(this);
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public override string ToString()
        {
            return ObjectExtensions.FormatJava(FORMAT, state, identifier, layer, transform, colorTransform, blend, filters);
        }

        /// <summary>
        /// {@inheritDoc} </summary>


        public int prepareToEncode(Context context)
        {

            hasBlend = blend != Blend.NULL.Value;
            hasFilters = true ^ filters.Count == 0;

            // CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
            int length = 5 + transform.prepareToEncode(context);

            if (context.get(Context.TYPE) == MovieTypes.DEFINE_BUTTON_2)
            {
                length += colorTransform.prepareToEncode(context);
            }

            if (hasFilters)
            {
                length += 1;
                foreach (Filter filter in filters)
                {
                    length += filter.prepareToEncode(context);
                }
            }

            if (hasBlend)
            {
                length += 1;
            }

            return length;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        



        public void encode(SWFEncoder coder, Context context)
        {
            int bits = 0;
            bits |= hasBlend ? Coder.BIT5 : 0;
            bits |= hasFilters ? Coder.BIT4 : 0;
            bits |= state;
            coder.writeByte(bits);
            coder.writeShort(identifier);
            coder.writeShort(layer);
            transform.encode(coder, context);

            if (context.get(Context.TYPE) != null && context.get(Context.TYPE) == MovieTypes.DEFINE_BUTTON_2)
            {
                colorTransform.encode(coder, context);
            }

            if (hasFilters)
            {
                coder.writeByte(filters.Count);
                foreach (Filter filter in filters)
                {
                    filter.encode(coder, context);
                }
            }

            if (hasBlend)
            {
                coder.writeByte(blend.Value);
            }
        }
    }

}