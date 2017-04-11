using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;

/*
 * Place2.java
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

namespace com.flagstone.transform
{
    /// <summary>
	/// Place2 is used to add and manipulate objects (shape, button, etc.) on
	/// the Flash Player's display list.
	/// 
	/// <para>
	/// Place2 supersedes the Place class providing more functionality
	/// and easier manipulation of objects in the display list through the following
	/// operations:
	/// </para>
	/// 
	/// <ul>
	/// <li>Place a new shape on the display list.</li>
	/// <li>Change an existing shape by moving it to new location or changing its
	/// appearance.</li>
	/// <li>Replace an existing shape with a another.</li>
	/// <li>
	/// Define clipping layers to mask objects displayed in front of a shape.</li>
	/// <li>Control the morphing process that changes one shape into another.</li>
	/// <li>Assign names to objects rather than using their identifiers.</li>
	/// <li>Define the sequence of actions that are executed when an event occurs in
	/// movie clip.</li>
	/// </ul>
	/// 
	/// <para>
	/// <b>Clipping Depth</b><br/>
	/// With the introduction of Flash 3 the display list supported a clipping layer.
	/// This allowed the outline of an object to define a clipping path that is used
	/// to mask other objects placed in front of it. The clipping depth can be set to
	/// mask objects between the layer containing the clipping path and a specified
	/// layer.
	/// </para>
	/// 
	/// <para>
	/// <b>Shape Morphing</b><br/>
	/// Shapes that will be morphed are defined using the DefineMorphShape class
	/// which defines a start and end shape. The Flash Player performs the
	/// interpolation that transforms one shape into another. The progress of the
	/// morphing process is controlled by a ratio which ranges from 0.0 to 1.0, where
	/// 0 generates a shape identical to the starting shape in the DefineMorphShape
	/// object and 1.0 generates the shape at the end of the morphing process.
	/// </para>
	/// 
	/// <para>
	/// <b>Event Handlers</b><br/>
	/// With the introduction of Flash 5, movie clips (defined using the
	/// DefineMovieClip class) could specify sequences of actions that would be
	/// performed in response to mouse or keyboard events. The actions are specified
	/// using EventHandler objects and the Place2 class is used to register the
	/// actions in response to a particular event with the Flash player. Multiple
	/// events can be handled by defining an EventHandler for each type of event. For
	/// more information see the EventHandler and Event which defines the set of
	/// events that a movie clip responds to.
	/// </para>
	/// 
	/// <para>
	/// Since only one object can be placed on a given layer an existing object on
	/// the display list can be identified by the layer it is displayed on rather
	/// than its identifier. Therefore Layer is the only required attribute. The
	/// remaining attributes are optional according to the different operation being
	/// performed:
	/// </para>
	/// 
	/// <ul>
	/// <li>If an existing object on the display list is being modified then only the
	/// layer number is required. Previously in the Place class both the
	/// identifier and the layer number were required.</li>
	/// <li>If no coordinate transform is applied to the shape (the default is a
	/// unity transform that does not change the shape) then it is not encoded.</li>
	/// <li>Similarly if no colour transform is applied to the shape (the default is
	/// a unity transform that does not change the shape's colour) then it is not
	/// encoded.</li>
	/// <li>If a shape is not being morphed then the ratio attribute may be left at
	/// its default value (-1.0).</li>
	/// <li>If a shape is not used to define a clipping area then the depth attribute
	/// may be left at its default value (0).</li>
	/// <li>If a name is net assigned to an object the name attribute may be left its
	/// default value (an empty string).</li>
	/// <li>If no events are being defined for a movie clip then the list of
	/// ClipEvent object may be left empty.</li>
	/// </ul>
	/// 
	/// <para>
	/// The Layer class provides a simple API for manipulating objects on the display
	/// list. While it is relatively simple to create instances of Place2
	/// object that perform the same steps the API provided by Player is easier to
	/// use and much more readable.
	/// </para>
	/// </summary>
	/// <seealso cref= com.flagstone.transform.util.movie.Layer </seealso>


	public sealed class Place2 : MovieTag
	{

		/// <summary>
		/// Place a new object on the display list. </summary>
		/// <param name="identifier"> the unique identifier for the object. </param>
		/// <param name="layer"> the layer where it will be displayed. </param>
		/// <param name="xCoord"> the x-coordinate where the object's origin will be. </param>
		/// <param name="yCoord"> the y-coordinate where the object's origin will be. </param>
		/// <returns> the Place2 object to update the display list. </returns>


		public static Place2 show(int identifier, int layer, int xCoord, int yCoord)
		{


			Place2 @object = new Place2();
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


		public static Place2 move(int layer, int xCoord, int yCoord)
		{


			Place2 @object = new Place2();
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


		public static Place2 replace(int identifier, int layer)
		{


			Place2 @object = new Place2();
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


		public static Place2 replace(int identifier, int layer, int xCoord, int yCoord)
		{


			Place2 @object = new Place2();
			@object.Type = PlaceType.REPLACE;
			@object.Layer = layer;
			@object.Identifier = identifier;
			@object.Transform = CoordTransform.translate(xCoord, yCoord);
			return @object;
		}

		/// <summary>
		/// The version of Flash where standard (16-bit) event codes were used. </summary>
		private const int STANDARD_EVENTS = 5;

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Place2: { type=%s; layer=%d;" + " identifier=%d; transform=%s; colorTransform=%s; ratio=%d;" + " clippingDepth=%d; name=%s; clipEvents=%s}";

		/// <summary>
		/// How the display list will be updated. </summary>
		private PlaceType type;
		/// <summary>
		/// The display list layer number. </summary>
		private int layer;
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
		/// The number of layers to clip. </summary>
		private int? depth;
		/// <summary>
		/// The name assigned to an object. </summary>
		private string name;
		/// <summary>
		/// The set of event handlers for movie clips. </summary>
		private IList<EventHandler> events;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a Place2 object using values encoded
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




		public Place2(SWFDecoder coder, Context context)
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

			layer = coder.readUnsignedShort();
			events = new List<EventHandler>();

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

			if (hasEvents)
			{
				int @event;

				coder.readUnsignedShort();

				if (context.get(Context.VERSION) > STANDARD_EVENTS)
				{
					coder.readInt();

					while ((@event = coder.readInt()) != 0)
					{
						events.Add(new EventHandler(@event, coder, context));
					}
				}
				else
				{
					coder.readUnsignedShort();

					while ((@event = coder.readUnsignedShort()) != 0)
					{
						events.Add(new EventHandler(@event, coder, context));
					}
				}
			}
			context.remove(Context.TRANSPARENT);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates an uninitialised Place2 object.
		/// </summary>
		public Place2()
		{
			events = new List<EventHandler>();
		}

		/// <summary>
		/// Creates and initialises a Place2 object using the values copied
		/// from another Place2 object.
		/// </summary>
		/// <param name="object">
		///            a Place2 object from which the values will be
		///            copied. </param>


		public Place2(Place2 @object)
		{
			type = @object.type;
			layer = @object.layer;
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

			events = new List<EventHandler>(@object.events.Count);

			foreach (EventHandler @event in @object.events)
			{
				events.Add(@event.copy());
			}
		}

		/// <summary>
		/// Adds a clip event to the list of clip events.
		/// </summary>
		/// <param name="aClipEvent">
		///            a clip event object.
		/// </param>
		/// <returns> this object. </returns>


		public Place2 add(EventHandler aClipEvent)
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
		/// Get the type of place operation being performed, either adding a new
		/// object, replacing an existing one with another or modifying an existing
		/// object.
		/// </summary>
		/// <returns> the way the object will be placed. </returns>
		public PlaceType Type
		{
			get => type;
		    private set => type = value;
		}

		/// <summary>
		/// Get the Layer on which the object will be displayed in the display
		/// list.
		/// </summary>
		/// <returns> the layer where the object will be displayed. </returns>
		public int Layer
		{
			get => layer;
		    private set => layer = value;
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
		    private set => identifier = value;
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
		    private set => transform = value;
		}

		/// <summary>
		/// Get the colour transform. May be null if no colour transform was
		/// defined.
		/// </summary>
		/// <returns> the colour transform that will be applied to the displayed
		/// object. </returns>
		public ColorTransform ColorTransform => colorTransform;

	    /// <summary>
		/// Get the morph ratio, in the range 0..65535 that defines the progress
		/// in the morphing process performed by the Flash Player from the defined
		/// start and end shapes. A value of 0 indicates the start of the process and
		/// 65535 the end. Returns null if no ratio was specified.
		/// </summary>
		/// <returns> the morphing ratio. </returns>
		public int? Ratio => ratio;

	    /// <summary>
		/// Get the number of layers that will be clipped by the object placed on
		/// the layer specified in this object.
		/// </summary>
		/// <returns> the number of layers to be clipped. </returns>
		public int? Depth => depth;

	    /// <summary>
		/// Get the name of the object. May be null if a name was not assigned to
		/// the object.
		/// </summary>
		/// <returns> the name of the object. </returns>
		public string Name => name;

	    /// <summary>
		/// Sets the type of placement.
		/// </summary>
		/// <param name="aType">
		///            the type of operation to be performed, either New, Modify or
		///            Replace.
		/// </param>
		/// <returns> this object. </returns>


		public Place2 setType(PlaceType aType)
		{
			type = aType;
			return this;
		}

		/// <summary>
		/// Sets the layer at which the object will be placed.
		/// </summary>
		/// <param name="aLayer">
		///            the layer number on which the object is being displayed. Must
		///            be in the range 1..65535.
		/// </param>
		/// <returns> this object. </returns>


		public Place2 setLayer(int aLayer)
		{
			if ((aLayer < 1) || (aLayer > Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, aLayer);
			}
			layer = aLayer;
			return this;
		}

		/// <summary>
		/// Sets the identifier of the object.
		/// </summary>
		/// <param name="uid">
		///            the identifier of a new object to be displayed. Must be in the
		///            range 1..65535.
		/// </param>
		/// <returns> this object. </returns>


		public Place2 setIdentifier(int uid)
		{
			if ((uid < 1) || (uid > Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, uid);
			}
			identifier = uid;
			return this;
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


		public Place2 setTransform(CoordTransform matrix)
		{
			transform = matrix;
			return this;
		}

		/// <summary>
		/// Sets the location where the object will be displayed.
		/// </summary>
		/// <param name="xCoord">
		///            the x-coordinate of the object's origin. </param>
		/// <param name="yCoord">
		///            the x-coordinate of the object's origin. </param>
		/// <returns> this object. </returns>


		public Place2 setLocation(int xCoord, int yCoord)
		{
			transform = CoordTransform.translate(xCoord, yCoord);
			return this;
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


		public Place2 setColorTransform(ColorTransform cxform)
		{
			colorTransform = cxform;
			return this;
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


		public Place2 setRatio(int? aNumber)
		{
			if ((aNumber != null) && ((aNumber < 0) || (aNumber > Coder.USHORT_MAX)))
			{
				throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, aNumber.Value);
			}
			ratio = aNumber;
			return this;
		}

		/// <summary>
		/// Sets the number of layers that this object will mask. May be set to zero
		/// if the shape being placed does not define a clipping area.
		/// </summary>
		/// <param name="aNumber">
		///            the number of layers clipped.
		/// </param>
		/// <returns> this object. </returns>


		public Place2 setDepth(int? aNumber)
		{
			if ((aNumber != null) && ((aNumber < 1) || (aNumber > Coder.USHORT_MAX)))
			{
				 throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, aNumber.Value);
			}
			depth = aNumber;
			return this;
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


		public Place2 setName(string aString)
		{
			name = aString;
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public Place2 copy()
		{
			return new Place2(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, type, layer, identifier, transform, colorTransform, ratio, depth, name, events);
		}


		/// <summary>
		/// {@inheritDoc} </summary>



		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			context.put(Context.TRANSPARENT, 1);

			length = 3;
			length += (type.Equals(PlaceType.NEW) || type.Equals(PlaceType.REPLACE)) ? 2 : 0;
			if (transform != null)
			{
				length += transform.prepareToEncode(context);
			}
			if (colorTransform != null)
			{
				length += colorTransform.prepareToEncode(context);
			}
			length += ratio == null ? 0 : 2;
			length += depth == null ? 0 : 2;
			length += ReferenceEquals(name, null) ? 0 : context.strlen(name);

			if (events.Count > 0)
			{
				int eventSize;

				if (context.get(Context.VERSION) > STANDARD_EVENTS)
				{
					eventSize = 4;
				}
				else
				{
					eventSize = 2;
				}

				length += 2 + eventSize;

				foreach (EventHandler handler in events)
				{
					length += handler.prepareToEncode(context);
				}

				length += eventSize;
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
				coder.writeShort((MovieTypes.PLACE_2 << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.PLACE_2 << Coder.LENGTH_FIELD_SIZE) | length);
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
			coder.writeShort(layer);

			if ((type == PlaceType.NEW) || (type == PlaceType.REPLACE))
			{
				coder.writeShort(identifier);
			}
			if (transform != null)
			{
				transform.encode(coder, context);
			}
			if (colorTransform != null)
			{
				colorTransform.encode(coder, context);
			}
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

			if (events.Count > 0)
			{
				int eventMask = 0;

				foreach (EventHandler handler in events)
				{
					eventMask |= handler.EventCode;
				}

				coder.writeShort(0);

				if (context.get(Context.VERSION) > STANDARD_EVENTS)
				{
					coder.writeInt(eventMask);
					foreach (EventHandler handler in events)
					{
						handler.encode(coder, context);
					}
					coder.writeInt(0);
				}
				else
				{
					coder.writeShort(eventMask);
					foreach (EventHandler handler in events)
					{
						handler.encode(coder, context);
					}
					coder.writeShort(0);
				}
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