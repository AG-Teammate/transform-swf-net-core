using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;
using com.flagstone.transform.filter;

/*
 * Place3.java
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

namespace com.flagstone.transform
{
    /// <summary>
	/// Place3 is used to update the display list. It extends Place2 by specifying
	/// a Blend which controls how an object is composited with its background and
	/// also Filters which can be used to create special effects such as drop shadows
	/// etc.
	/// </summary>
	/// <seealso cref= Place2 </seealso>


	public sealed class Place3 : MovieTag
	{

		/// <summary>
		/// Place a new object on the display list. </summary>
		/// <param name="identifier"> the unique identifier for the object. </param>
		/// <param name="layer"> the layer where it will be displayed. </param>
		/// <param name="xCoord"> the x-coordinate where the object's origin will be. </param>
		/// <param name="yCoord"> the y-coordinate where the object's origin will be. </param>
		/// <returns> the Place3 object to update the display list. </returns>


		public static Place3 show(int identifier, int layer, int xCoord, int yCoord)
		{


			Place3 @object = new Place3();
			@object.Type = PlaceType.NEW;
			@object.Layer = layer;
			@object.Identifier = identifier;
			@object.Transform = CoordTransform.translate(xCoord, yCoord);
			return @object;
		}

		/// <summary>
		/// Change the position of a displayed object.
		/// </summary>
		/// <param name="layer"> the display list layer where the object is displayed. </param>
		/// <param name="xCoord"> the x-coordinate where the object's origin will be moved. </param>
		/// <param name="yCoord"> the y-coordinate where the object's origin will be moved. </param>
		/// <returns> the Place3 object to change the position of the object. </returns>


		public static Place3 move(int layer, int xCoord, int yCoord)
		{


			Place3 @object = new Place3();
			@object.Type = PlaceType.MODIFY;
			@object.Layer = layer;
			@object.Transform = CoordTransform.translate(xCoord, yCoord);
			return @object;
		}

		/// <summary>
		/// Replace an existing object with another.
		/// </summary>
		/// <param name="identifier"> the unique identifier of the new object. </param>
		/// <param name="layer"> the display list layer of the existing object. </param>
		/// <returns> the Place3 object to update the display list. </returns>


		public static Place3 replace(int identifier, int layer)
		{


			Place3 @object = new Place3();
			@object.Type = PlaceType.REPLACE;
			@object.Layer = layer;
			@object.Identifier = identifier;
			return @object;
		}

		/// <summary>
		/// Replace an existing object with another.
		/// </summary>
		/// <param name="identifier"> the unique identifier of the new object. </param>
		/// <param name="layer"> the display list layer of the existing object. </param>
		/// <param name="xCoord"> the x-coordinate where the new object's origin will be. </param>
		/// <param name="yCoord"> the y-coordinate where the new object's origin will be. </param>
		/// <returns> the Place3 object to update the display list. </returns>


		public static Place3 replace(int identifier, int layer, int xCoord, int yCoord)
		{


			Place3 @object = new Place3();
			@object.Type = PlaceType.REPLACE;
			@object.Layer = layer;
			@object.Identifier = identifier;
			@object.Transform = CoordTransform.translate(xCoord, yCoord);
			return @object;
		}

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "PlaceObject3: { type=%s; layer=%d;" + " bitmapCache=%d; identifier=%d; transform=%s;" + " colorTransform=%s; ratio=%d; clippingDepth=%d;" + " name=%s; className=%s;" + " filters=%s; blend=%s; clipEvents=%s}";

		/// <summary>
		/// How the display list will be updated. </summary>
		private PlaceType type;
		/// <summary>
		/// The display list layer number. </summary>
		private int layer;
		/// <summary>
		/// The actionscript 3 class that will render the object to be displayed. </summary>
		private string className;
		/// <summary>
		/// Whether the displayed object will be cached as a bitmap. </summary>
		private int? bitmapCache;
		/// <summary>
		/// The unique identifier of the object that will be displayed. </summary>
		private int identifier;
		/// <summary>
		/// The coordinate transform applied to the displayed object. </summary>
		private CoordTransform transform;
		/// <summary>
		/// The color transform applied to the displayed object. </summary>
		private ColorTransform colorTransform;
		/// <summary>
		/// The progression of the morphing process. </summary>
		private int? ratio;
		/// <summary>
		/// The name assigned to an object. </summary>
		private string name;
		/// <summary>
		/// The number of layers to clip. </summary>
		private int? depth;
		/// <summary>
		/// The set of effects filters applied to the object. </summary>
		private IList<Filter> filters;
		/// <summary>
		/// How the object is blended with its background. </summary>
		private int? blend;
		/// <summary>
		/// The set of event handlers for movie clips. </summary>
		private IList<EventHandler> events;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// Indicates whether the encoded object contains a blend. </summary>
		
		private bool hasBlend;
		/// <summary>
		/// Indicates whether the encoded object contains filters. </summary>
		
		private bool hasFilters;
		/// <summary>
		/// Indicates whether the encoded object contains an image. </summary>
		
		private bool hasImage;

		/// <summary>
		/// Creates and initialises a Place3 object using values encoded
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




		public Place3(SWFDecoder coder, Context context)
		{
			context.put(Context.TRANSPARENT, 1);
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			int bits = coder.readByte();


			bool hasEvents = (bits & Coder.BIT7) != 0;


			bool hasDepth = (bits & Coder.BIT6) != 0;


			bool hasName = (bits & Coder.BIT5) != 0;


			bool hasRatio = (bits & Coder.BIT4) != 0;


			bool hasColorTransform = (bits & Coder.BIT3) != 0;


			bool hasTransform = (bits & Coder.BIT2) != 0;

			switch (bits & Coder.PAIR0)
			{
			case 0:
				type = PlaceType.MODIFY;
				break;
			case 1:
				type = PlaceType.MODIFY;
				break;
			case 2:
				type = PlaceType.NEW;
				break;
			default:
				type = PlaceType.REPLACE;
				break;
			}

			bits = coder.readByte();
			hasImage = (bits & Coder.BIT4) != 0;


			bool hasClassName = (bits & Coder.BIT3) != 0;


			bool hasBitmapCache = (bits & Coder.BIT2) != 0;
			hasBlend = (bits & Coder.BIT1) != 0;
			hasFilters = (bits & Coder.BIT0) != 0;

			layer = coder.readUnsignedShort();

			/* The following line implements the logic as described in the SWF 9
			 * specification but it appears to be incorrect. The class name is not
			 * given when hasImage is set.
			 *
			 * if (hasClassName || ((type == PlaceType.NEW
			 * || type == PlaceType.REPLACE) && hasImage)) {
			 */
			if (hasClassName)
			{
				className = coder.readString();
			}

			if ((type == PlaceType.NEW) || (type == PlaceType.REPLACE))
			{
				identifier = coder.readUnsignedShort();
			}

			if (hasTransform)
			{
				transform = new CoordTransform(coder);
			}

			if (hasColorTransform)
			{
				colorTransform = new ColorTransform(coder, context);
			}

			if (hasRatio)
			{
				ratio = coder.readUnsignedShort();
			}

			if (hasName)
			{
				name = coder.readString();
			}

			if (hasDepth)
			{
				depth = coder.readUnsignedShort();
			}

			filters = new List<Filter>();

			if (hasFilters)
			{


				SWFFactory<Filter> decoder = context.Registry.FilterDecoder;



				int count = coder.readByte();

				for (int i = 0; i < count; i++)
				{
					decoder.getObject(filters, coder, context);
				}
			}

			if (hasBlend)
			{
				blend = coder.readByte();
			}

			if (hasBitmapCache)
			{
				bitmapCache = coder.readByte();
			}

			events = new List<EventHandler>();

			if (hasEvents)
			{
				int @event;

				coder.readUnsignedShort();
				coder.readInt();

				while ((@event = coder.readInt()) != 0)
				{
					events.Add(new EventHandler(@event, coder, context));
				}
			}
			context.remove(Context.TRANSPARENT);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates an uninitialised Place3 object.
		/// </summary>
		public Place3()
		{
			filters = new List<Filter>();
			events = new List<EventHandler>();
		}

		/// <summary>
		/// Creates and initialises a Place3 object using the values copied
		/// from another Place3 object.
		/// </summary>
		/// <param name="object">
		///            a Place3 object from which the values will be
		///            copied. </param>


		public Place3(Place3 @object)
		{
			type = @object.type;
			layer = @object.layer;
			bitmapCache = @object.bitmapCache;
			className = @object.className;
			identifier = @object.identifier;
			if (@object.transform != null)
			{
				transform = @object.transform;
			}
			if (@object.colorTransform != null)
			{
				colorTransform = @object.colorTransform;
			}
			ratio = @object.ratio;
			depth = @object.depth;
			name = @object.name;

			filters = new List<Filter>(@object.filters);
			blend = @object.blend;
			events = new List<EventHandler>(@object.events.Count);

			foreach (EventHandler @event in @object.events)
			{
				events.Add(@event.copy());
			}

		}

		/// <summary>
		/// Get the type of place operation being performed, either adding a new
		/// object, replacing an existing one with another or modifying an existing
		/// object.
		/// </summary>
		/// <returns> the way the object will be placed. </returns>
		public PlaceType Type
		{
			get => type;
		    set => type = value;
		}

		/// <summary>
		/// Sets the type of placement.
		/// </summary>
		/// <param name="aType">
		///            the type of operation to be performed, either New, Modify or
		///            Replace.
		/// </param>
		/// <returns> this object. </returns>


		public Place3 setType(PlaceType aType)
		{
			type = aType;
			return this;
		}

		/// <summary>
		/// Get the Layer on which the object will be displayed in the display
		/// list.
		/// </summary>
		/// <returns> the layer where the object will be displayed. </returns>
		public int Layer
		{
			get => layer;
		    set => layer = value;
		}

		/// <summary>
		/// Sets the layer at which the object will be placed.
		/// </summary>
		/// <param name="aLayer">
		///            the layer number on which the object is being displayed. Must
		///            be in the range 1..65535.
		/// </param>
		/// <returns> this object. </returns>


		public Place3 setLayer(int aLayer)
		{
			if ((aLayer < 1) || (aLayer > Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, aLayer);
			}
			layer = aLayer;
			return this;
		}

		/// <summary>
		/// Get the identifier of the object to be placed. This is only required
		/// when placing an object for the first time. Subsequent references to the
		/// object on this layer can simply use the layer number.
		/// </summary>
		/// <returns> the unique identifier of the object to be displayed. </returns>
		public int Identifier
		{
			get => identifier;
		    set => identifier = value;
		}

		/// <summary>
		/// Sets the identifier of the object.
		/// </summary>
		/// <param name="uid">
		///            the identifier of a new object to be displayed. Must be in the
		///            range 1..65535.
		/// </param>
		/// <returns> this object. </returns>


		public Place3 setIdentifier(int uid)
		{
			if ((uid < 1) || (uid > Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, uid);
			}
			identifier = uid;
			return this;
		}

		/// <summary>
		/// Get the coordinate transform. May be null if no coordinate transform
		/// was defined.
		/// </summary>
		/// <returns> the coordinate transform that will be applied to the displayed
		/// object. </returns>
		public CoordTransform Transform
		{
			get => transform;
		    set => transform = value;
		}

		/// <summary>
		/// Sets the coordinate transform that defines the position where the object
		/// will be displayed. The argument may be null if the location of the object
		/// is not being changed.
		/// </summary>
		/// <param name="matrix">
		///            an CoordTransform object that will be applied to the object
		///            displayed.
		/// </param>
		/// <returns> this object. </returns>


		public Place3 setTransform(CoordTransform matrix)
		{
			transform = matrix;
			return this;
		}

		/// <summary>
		/// Get the colour transform. May be null if no colour transform was
		/// defined.
		/// </summary>
		/// <returns> the colour transform that will be applied to the displayed
		/// object. </returns>
		public ColorTransform ColorTransform
		{
			get => colorTransform;
		    set => colorTransform = value;
		}

		/// <summary>
		/// Sets the colour transform that defines the colour effects applied to the
		/// object. The argument may be null if the color of the object is not being
		/// changed.
		/// </summary>
		/// <param name="cxform">
		///            an ColorTransform object that will be applied to the object
		///            displayed.
		/// </param>
		/// <returns> this object. </returns>


		public Place3 setColorTransform(ColorTransform cxform)
		{
			colorTransform = cxform;
			return this;
		}

		/// <summary>
		/// Get the morph ratio, in the range 0..65535 that defines the progress
		/// in the morphing process performed by the Flash Player from the defined
		/// start and end shapes. A value of 0 indicates the start of the process and
		/// 65535 the end. Returns null if no ratio was specified.
		/// </summary>
		/// <returns> the morphing ratio. </returns>
		public int? Ratio
		{
			get => ratio;
		    set => ratio = value;
		}

		/// <summary>
		/// Sets point of the morphing process for a morph shape in the range
		/// 0..65535. May be set to null if the shape being placed is not being
		/// morphed.
		/// </summary>
		/// <param name="aNumber">
		///            the progress in the morphing process.
		/// </param>
		/// <returns> this object. </returns>


		public Place3 setRatio(int? aNumber)
		{
			if ((aNumber != null) && ((aNumber < 0) || (aNumber > Coder.USHORT_MAX)))
			{
				throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, aNumber.Value);
			}
			ratio = aNumber;
			return this;
		}

		/// <summary>
		/// Get the number of layers that will be clipped by the object placed on
		/// the layer specified in this object.
		/// </summary>
		/// <returns> the number of layers to be clipped. </returns>
		public int? Depth
		{
			get => depth;
		    set => setDepth(value);
		}

		/// <summary>
		/// Sets the number of layers that this object will mask. May be set to zero
		/// if the shape being placed does not define a clipping area.
		/// </summary>
		/// <param name="aNumber">
		///            the number of layers clipped.
		/// </param>
		/// <returns> this object. </returns>


		public Place3 setDepth(int? aNumber)
		{
			if ((aNumber != null) && ((aNumber < 1) || (aNumber > Coder.USHORT_MAX)))
			{
				 throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, aNumber.Value);
			}
			depth = aNumber;
			return this;
		}

		/// <summary>
		/// Get the name of the object. May be null if a name was not assigned to
		/// the object.
		/// </summary>
		/// <returns> the name of the object. </returns>
		public string Name
		{
			get => name;
		    set => setName(value);
		}

		/// <summary>
		/// Set the name of an object to be displayed. If a shape is not being
		/// assigned a name then setting the argument to null will omit the attribute
		/// when the object is encoded.
		/// </summary>
		/// <param name="aString">
		///            the name assigned to the object.
		/// </param>
		/// <returns> this object. </returns>


		public Place3 setName(string aString)
		{
			name = aString;
			return this;
		}

		/// <summary>
		/// Get the value indicating whether the display object will be cached as a
		/// bitmap (non-zero) or not cached (zero).
		/// </summary>
		/// <returns> a non-zero value if the object is cached or zer oif not cached. </returns>
		public int? BitmapCache => bitmapCache;

	    /// <summary>
		/// Set whether the displayed object should be cached as a bitmap.
		/// </summary>
		/// <param name="cache"> set to a non-zero value if the object should be cached as
		/// a bitmap or to zero to disable caching.
		/// </param>
		/// <returns> this object. </returns>


		public Place3 setBitmapCache(int? cache)
		{
			bitmapCache = cache;
			return this;
		}

		/// <summary>
		/// Get the name of the Actionscript 3 class which will be used to render
		/// the object to be displayed.
		/// </summary>
		/// <returns> the name of the Actionscript class. </returns>
		public string ClassName => className;

	    /// <summary>
		/// Set the name of the Actionscript 3 class which will be used to render
		/// the object to be displayed.
		/// </summary>
		/// <param name="aName"> the name of the Actionscript class. </param>
		/// <returns> this object. </returns>


		public Place3 setClassName(string aName)
		{
			className = aName;
			return this;
		}

		/// <summary>
		/// Get the list of filters that will be applied to the object when it is
		/// displayed as a bitmap. </summary>
		/// <returns> the list of bitmap filters. </returns>
		public IList<Filter> Filters
		{
			get => filters;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				filters = value;
			}
		}


		/// <summary>
		/// Get the blend that describes how the object will be rendered in relation
		/// to the background. </summary>
		/// <returns> the Blend that describes how the object is composited. </returns>
		public Blend Blend => Blend.fromInt((int)blend);

	    /// <summary>
		/// Set the blend that describes how the object will be rendered in relation
		/// to the background. </summary>
		/// <param name="mode"> the Blend that describes how the object is composited. </param>
		/// <returns> this object. </returns>


		public Place3 setBlend(Blend mode)
		{
			blend = mode.Value;
			return this;
		}

		/// <summary>
		/// Adds a clip event to the list of clip events.
		/// </summary>
		/// <param name="aClipEvent">
		///            a clip event object.
		/// </param>
		/// <returns> this object. </returns>


		public Place3 add(EventHandler aClipEvent)
		{
			if (aClipEvent == null)
			{
				throw new ArgumentException();
			}
			events.Add(aClipEvent);
			return this;
		}

		/// <summary>
		/// Get the list of event handlers that define the actions that will
		/// be executed in response to events that occur in the movie clip being
		/// placed.
		/// </summary>
		/// <returns> the set of event handlers for the movie clip. </returns>
		public IList<EventHandler> Events
		{
			get => events;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				events = value;
			}
		}


		/// <summary>
		/// Adds a Filter to the list of filters.
		/// </summary>
		/// <param name="filter">
		///            a Filter object.
		/// </param>
		/// <returns> this object. </returns>


		public Place3 add(Filter filter)
		{
			if (filter == null)
			{
				throw new ArgumentException();
			}
			filters.Add(filter);
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public Place3 copy()
		{
			return new Place3(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, type, layer, bitmapCache, identifier, transform, colorTransform, ratio, depth, name, className, filters, blend, events);
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			context.put(Context.TRANSPARENT, 1);

			hasBlend = blend != null;
			hasFilters = true ^ filters.Count == 0;

			length = 4;
			length += ((type == PlaceType.NEW) || (type == PlaceType.REPLACE)) ? 2 : 0;
			length += transform == null ? 0 : transform.prepareToEncode(context);
			length += colorTransform == null ? 0 : colorTransform.prepareToEncode(context);
			length += ratio == null ? 0 : 2;
			length += depth == null ? 0 : 2;
			length += ReferenceEquals(name, null) ? 0 : context.strlen(name);
			length += ReferenceEquals(className, null) ? 0 : context.strlen(className);

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

			if (bitmapCache != null)
			{
				length += 1;
			}

			if (events.Count > 0)
			{
				length += 6;

				foreach (EventHandler handler in events)
				{
					length += handler.prepareToEncode(context);
				}

				length += 4;
			}

			context.remove(Context.TRANSPARENT);

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>




		public void encode(SWFEncoder coder, Context context)
		{
			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.PLACE_3 << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.PLACE_3 << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}

			context.put(Context.TRANSPARENT, 1);
			int bits = 0;
			bits |= events.Count == 0 ? 0 : Coder.BIT7;
			bits |= depth == null ? 0 : Coder.BIT6;
			bits |= ReferenceEquals(name, null) ? 0 : Coder.BIT5;
			bits |= ratio == null ? 0 : Coder.BIT4;
			bits |= colorTransform == null ? 0 : Coder.BIT3;
			bits |= transform == null ? 0 : Coder.BIT2;

			switch (type)
			{
			case PlaceType.MODIFY:
				bits |= Coder.BIT0;
				break;
			case PlaceType.NEW:
				bits |= Coder.BIT1;
				break;
			default:
				bits |= Coder.BIT0;
				bits |= Coder.BIT1;
				break;
			}
			coder.writeByte(bits);

			bits = 0;
			bits |= hasImage ? Coder.BIT4 : 0;
			bits |= ReferenceEquals(className, null) ? 0 : Coder.BIT3;
			bits |= bitmapCache == null ? 0 : Coder.BIT2;
			bits |= hasBlend ? Coder.BIT1 : 0;
			bits |= hasFilters ? Coder.BIT0 : 0;
			coder.writeByte(bits);
			coder.writeShort(layer);

			if (!ReferenceEquals(className, null))
			{
				coder.writeString(className);
			}
			if ((type == PlaceType.NEW) || (type == PlaceType.REPLACE))
			{
				coder.writeShort(identifier);
			}
			if (transform != null)
			{
				transform.encode(coder, context);
			}
		    colorTransform?.encode(coder, context);
		    if (ratio != null)
			{
				coder.writeShort(ratio.Value);
			}
			if (!ReferenceEquals(name, null))
			{
				coder.writeString(name);
			}

			if (depth != null)
			{
				coder.writeShort(depth.Value);
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

			if (bitmapCache != null)
			{
				coder.writeByte(bitmapCache.Value);
			}

			if (events.Count > 0)
			{
				int eventMask = 0;

				coder.writeShort(0);

				foreach (EventHandler handler in events)
				{
					eventMask |= handler.EventCode;
				}

				coder.writeInt(eventMask);

				foreach (EventHandler handler in events)
				{
					handler.encode(coder, context);
				}

				coder.writeInt(0);
			}
			context.remove(Context.TRANSPARENT);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}