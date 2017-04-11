using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * DefineButton2.java
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
	/// DefineButton2 defines the appearance and actions of push and menu buttons.
	/// 
	/// <para>
	/// It provides a more sophisticated model for creating buttons than
	/// <seealso cref="DefineButton"/>:
	/// </para>
	/// 
	/// <ul>
	/// <li>Two types of button are supported, <B>Push</B> and <B>Menu</B>.</li>
	/// <li>The number of events that a button can respond to is increased.</li>
	/// <li>Actions can be executed for any button event.</li>
	/// </ul>
	/// 
	/// <para>
	/// Push and Menu buttons behave slightly differently in tracking mouse movements
	/// when the button is clicked. A Push button 'captures' the mouse so if the
	/// cursor is dragged outside of the active area of the button and the mouse
	/// click is released then the Release Outside event is still sent to the button.
	/// A Menu button does not 'capture' the mouse so if the cursor is dragged out of
	/// the active area the button returns to its 'inactive' state.
	/// </para>
	/// 
	/// <para>
	/// A DefineButton2 object must contain at least one ButtonShape. If more than
	/// one button shape is defined for a given button state then each shape will be
	/// displayed by the button. The order in which the shapes are displayed is
	/// determined by the layer assigned to each button record.
	/// </para>
	/// </summary>
	/// <seealso cref= ButtonShape </seealso>
	/// <seealso cref= EventHandler </seealso>
	public sealed class DefineButton2 : DefineTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineButton2: { type=%s;" + " identifier=%d; buttonRecords=%s; handlers=%s}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The button type: push or menu. </summary>
		private int type;
		/// <summary>
		/// The list of shapes used to draw the button. </summary>
		private IList<ButtonShape> shapes;
		/// <summary>
		/// The list of handlers for different button events. </summary>
		private IList<EventHandler> events;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// Offset in bytes from the start of the shapes to the event handlers. </summary>
		
		private int offset;

		/// <summary>
		/// Creates and initialises a DefineButton2 object using values encoded
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




		public DefineButton2(SWFDecoder coder, Context context)
		{
			context.put(Context.TYPE, MovieTypes.DEFINE_BUTTON_2);
			context.put(Context.TRANSPARENT, 1);

			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			type = coder.readByte();
			shapes = new List<ButtonShape>();

			int offsetToNext = coder.readUnsignedShort();

			while (coder.scanByte() != 0)
			{
				shapes.Add(new ButtonShape(coder, context));
			}

			coder.readByte();

			events = new List<EventHandler>();

			if (offsetToNext != 0)
			{
				int size;

				if (type == 1)
				{
					context.put(Context.MENU_BUTTON, 1);
				}

				do
				{
					offsetToNext = coder.readUnsignedShort();

					if (offsetToNext == 0)
					{
						size = length - coder.bytesRead() - 2;
					}
					 else
					 {
						 // CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
						 size = offsetToNext - 4;
					 }
					events.Add(new EventHandler(size, coder, context));

				} while (offsetToNext != 0);

				context.remove(Context.MENU_BUTTON);
			}

			context.remove(Context.TYPE);
			context.remove(Context.TRANSPARENT);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a DefineButton2 object, specifying the unique identifier, the
		/// type of button to be created, the button shapes that describe the
		/// button's appearance and the actions that are performed in response to
		/// each button event.
		/// </summary>
		/// <param name="uid">
		///            a unique identifier for this button. Must be in the range
		///            1..65535. </param>
		/// <param name="buttonType">
		///            the button is a menu button (true) or push button (false). </param>
		/// <param name="buttonShapes">
		///            a list of Button objects. Must not be null. </param>
		/// <param name="handlers">
		///            a list of ButtonEvent objects. Must not be null. </param>


		public DefineButton2(int uid, ButtonType buttonType, IList<ButtonShape> buttonShapes, IList<EventHandler> handlers)
		{
			Identifier = uid;
			Type = buttonType;
			Shapes = buttonShapes;
			Events = handlers;
		}

		/// <summary>
		/// Creates and initialises a DefineButton2 object using the values copied
		/// from another DefineButton2 object.
		/// </summary>
		/// <param name="object">
		///            a DefineButton2 object from which the values will be
		///            copied. </param>


		public DefineButton2(DefineButton2 @object)
		{
			identifier = @object.identifier;
			type = @object.type;
			shapes = new List<ButtonShape>(@object.shapes.Count);
			foreach (ButtonShape shape in @object.shapes)
			{
				shapes.Add(shape.copy());
			}
			events = new List<EventHandler>(@object.events.Count);
			foreach (EventHandler @event in @object.events)
			{
				events.Add(@event.copy());
			}
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
		/// Adds an ButtonShape to the list of button records.
		/// </summary>
		/// <param name="obj">
		///            a button shape object. Must not be null. </param>
		/// <returns> this object. </returns>


		public DefineButton2 add(ButtonShape obj)
		{
			if (obj == null)
			{
				throw new ArgumentException();
			}
			shapes.Add(obj);
			return this;
		}

		/// <summary>
		/// Adds a button event object to the list of button events.
		/// </summary>
		/// <param name="obj">
		///            a button event. Must not be null. </param>
		/// <returns> this object. </returns>


		public DefineButton2 add(EventHandler obj)
		{
			if (obj == null)
			{
				throw new ArgumentException();
			}
			events.Add(obj);
			return this;
		}

		/// <summary>
		/// Get the button type - either PUSH or MENU.
		/// </summary>
		/// <returns> the type that identifies the button. </returns>
		public ButtonType Type
		{
			get
			{
				ButtonType value;
				if (type == 0)
				{
					value = ButtonType.PUSH;
				}
				else
				{
					value = ButtonType.MENU;
				}
				return value;
			}
			set
			{
				if (value == ButtonType.PUSH)
				{
					type = 0;
				}
				else
				{
					type = 1;
				}
			}
		}

		/// <summary>
		/// Get the list of button records defined for this button.
		/// </summary>
		/// <returns> the list of shapes used to draw the button. </returns>
		public IList<ButtonShape> Shapes
		{
			get => shapes;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				shapes = value;
			}
		}

		/// <summary>
		/// Get the list of event handlers defined for this button.
		/// </summary>
		/// <returns> the event handlers for the button. </returns>
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
		/// {@inheritDoc} </summary>
		public DefineButton2 copy()
		{
			return new DefineButton2(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, Type, identifier, shapes, events);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF - Fixed length when encoded.
			context.put(Context.TYPE, MovieTypes.DEFINE_BUTTON_2);
			context.put(Context.TRANSPARENT, 1);

			length = 6;

			foreach (ButtonShape shape in shapes)
			{
				length += shape.prepareToEncode(context);
			}

			if (events.Count == 0)
			{
				offset = 0;
			}
			else
			{
				offset = length - 3;
			}

			EventHandler handler;


			int count = events.Count;

			if (type == 1)
			{
				context.put(Context.MENU_BUTTON, 1);
			}

			for (int i = 0; i < count; i++)
			{
				handler = events[i];
				if (i == count - 1)
				{
					context.put(Context.LAST, 1);
				}
				length += handler.prepareToEncode(context);
			}

			context.remove(Context.TYPE);
			context.remove(Context.TRANSPARENT);
			context.remove(Context.LAST);
			context.remove(Context.MENU_BUTTON);

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			context.put(Context.TYPE, MovieTypes.DEFINE_BUTTON_2);
			context.put(Context.TRANSPARENT, 1);

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_BUTTON_2 << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_BUTTON_2 << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			coder.writeByte(type);
			coder.writeShort(offset);

			foreach (ButtonShape shape in shapes)
			{
				shape.encode(coder, context);
			}
			coder.writeByte(0);

			if (type == 1)
			{
				context.put(Context.MENU_BUTTON, 1);
			}

			foreach (EventHandler handler in events)
			{
				handler.encode(coder, context);
			}

			context.remove(Context.TYPE);
			context.remove(Context.TRANSPARENT);
			context.remove(Context.MENU_BUTTON);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}