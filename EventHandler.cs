using System;
using System.Collections.Generic;
using System.IO;
using com.flagstone.transform.action;
using com.flagstone.transform.coder;
using Action = com.flagstone.transform.action.Action;

/*
 * Event.java
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
	/// <para>
	/// EventHandler is used to define the actions that a movie clip or button will
	/// execute in response to a particular event. Handlers for movie clips are
	/// defined when the movie clip is added to the display list using Place2 or
	/// Place3 objects while handlers for buttons are added when the button is
	/// created.
	/// </para>
	/// </summary>
	/// <seealso cref= Event </seealso>


	public sealed class EventHandler : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "EventHandler: { events=%s;" + " key=%s; actions=%s}";

		/// <summary>
		/// Version of Flash that supports the extended event model. </summary>
		private const int EVENTS_VERSION = 6;

		/// <summary>
		/// Number of bits to shift key code for encoding with event flags. </summary>
		private const int KEY_OFFSET = 9;
		/// <summary>
		/// Bit mask for key field. </summary>
		private const int KEY_MASK = 0xFE00;
		/// <summary>
		/// Bit mask for key field. </summary>
		private const int EVENT_MASK = 0x01FF;
		/// <summary>
		/// The number of different types of event supported by buttons. </summary>
		private const int NUM_BUTTON_EVENTS = 9;
		/// <summary>
		/// The number of different types of event supported by movie clips. </summary>
		private const int NUM_CLIP_EVENTS = 19;
		/// <summary>
		/// Bit mask for accessing bit 0 of the composite event code. </summary>
		private const int BIT0 = 1;
		/// <summary>
		/// Bit mask for accessing bit 1 of the composite event code. </summary>
		private const int BIT1 = 2;
		/// <summary>
		/// Bit mask for accessing bit 2 of the composite event code. </summary>
		private const int BIT2 = 4;
		/// <summary>
		/// Bit mask for accessing bit 3 of the composite event code. </summary>
		private const int BIT3 = 8;
		/// <summary>
		/// Bit mask for accessing bit 4 of the composite event code. </summary>
		private const int BIT4 = 16;
		/// <summary>
		/// Bit mask for accessing bit 5 of the composite event code. </summary>
		private const int BIT5 = 32;
		/// <summary>
		/// Bit mask for accessing bit 6 of the composite event code. </summary>
		private const int BIT6 = 64;
		/// <summary>
		/// Bit mask for accessing bit 7 of the composite event code. </summary>
		private const int BIT7 = 128;
		/// <summary>
		/// Bit mask for accessing bit 8 of the composite event code. </summary>
		private const int BIT8 = 256;
		/// <summary>
		/// Bit mask for accessing bit 9 of the composite event code. </summary>
		private const int BIT9 = 512;
		/// <summary>
		/// Bit mask for accessing bit 10 of the composite event code. </summary>
		private const int BIT10 = 1024;
		/// <summary>
		/// Bit mask for accessing bit 11 of the composite event code. </summary>
		private const int BIT11 = 2048;
		/// <summary>
		/// Bit mask for accessing bit 12 of the composite event code. </summary>
		private const int BIT12 = 4096;
		/// <summary>
		/// Bit mask for accessing bit 13 of the composite event code. </summary>
		private const int BIT13 = 8192;
		/// <summary>
		/// Bit mask for accessing bit 14 of the composite event code. </summary>
		private const int BIT14 = 16384;
		/// <summary>
		/// Bit mask for accessing bit 15 of the composite event code. </summary>
		private const int BIT15 = 32768;
		/// <summary>
		/// Bit mask for accessing bit 16 of the composite event code. </summary>
		private const int BIT16 = 65536;
		/// <summary>
		/// Bit mask for accessing bit 17 of the composite event code. </summary>
		private const int BIT17 = 131072;
		/// <summary>
		/// Bit mask for accessing bit 18 of the composite event code. </summary>
		private const int BIT18 = 262144;

		/// <summary>
		/// Table mapping a movie event to a code. </summary>
		private static readonly IDictionary<Event, int?> CLIP_CODES;
		/// <summary>
		/// Table mapping a push button event to a code. </summary>
		private static readonly IDictionary<Event, int?> BUTTON_CODES;
		/// <summary>
		/// Table mapping a menu button event to a code. </summary>
		private static readonly IDictionary<Event, int?> MENU_CODES;

		/// <summary>
		/// Table mapping a code to a movie event. </summary>
		private static readonly IDictionary<int?, Event> CLIP_EVENTS;
		/// <summary>
		/// Table mapping a code to a push button event. </summary>
		private static readonly IDictionary<int?, Event> BUTTON_EVENTS;
		/// <summary>
		/// Table mapping a code to a menu button event. </summary>
		private static readonly IDictionary<int?, Event> MENU_EVENTS;

		static EventHandler()
		{
			CLIP_CODES = new Dictionary<Event, int?>();
			CLIP_CODES[Event.LOAD] = BIT0;
			CLIP_CODES[Event.ENTER_FRAME] = BIT1;
			CLIP_CODES[Event.UNLOAD] = BIT2;
			CLIP_CODES[Event.MOUSE_MOVE] = BIT3;
			CLIP_CODES[Event.MOUSE_DOWN] = BIT4;
			CLIP_CODES[Event.MOUSE_UP] = BIT5;
			CLIP_CODES[Event.KEY_DOWN] = BIT6;
			CLIP_CODES[Event.KEY_UP] = BIT7;
			CLIP_CODES[Event.DATA] = BIT8;
			CLIP_CODES[Event.INITIALIZE] = BIT9;
			CLIP_CODES[Event.PRESS] = BIT10;
			CLIP_CODES[Event.RELEASE] = BIT11;
			CLIP_CODES[Event.RELEASE_OUT] = BIT12;
			CLIP_CODES[Event.ROLL_OVER] = BIT13;
			CLIP_CODES[Event.ROLL_OUT] = BIT14;
			CLIP_CODES[Event.DRAG_OVER] = BIT15;
			CLIP_CODES[Event.DRAG_OUT] = BIT16;
			CLIP_CODES[Event.KEY_PRESS] = BIT17;
			CLIP_CODES[Event.CONSTRUCT] = BIT18;

			CLIP_EVENTS = new Dictionary<int?, Event>();
			CLIP_EVENTS[BIT0] = Event.LOAD;
			CLIP_EVENTS[BIT1] = Event.ENTER_FRAME;
			CLIP_EVENTS[BIT2] = Event.UNLOAD;
			CLIP_EVENTS[BIT3] = Event.MOUSE_MOVE;
			CLIP_EVENTS[BIT4] = Event.MOUSE_DOWN;
			CLIP_EVENTS[BIT5] = Event.MOUSE_UP;
			CLIP_EVENTS[BIT6] = Event.KEY_DOWN;
			CLIP_EVENTS[BIT7] = Event.KEY_UP;
			CLIP_EVENTS[BIT8] = Event.DATA;
			CLIP_EVENTS[BIT9] = Event.INITIALIZE;
			CLIP_EVENTS[BIT10] = Event.PRESS;
			CLIP_EVENTS[BIT11] = Event.RELEASE;
			CLIP_EVENTS[BIT12] = Event.RELEASE_OUT;
			CLIP_EVENTS[BIT13] = Event.ROLL_OVER;
			CLIP_EVENTS[BIT14] = Event.ROLL_OUT;
			CLIP_EVENTS[BIT15] = Event.DRAG_OVER;
			CLIP_EVENTS[BIT16] = Event.DRAG_OUT;
			CLIP_EVENTS[BIT17] = Event.KEY_PRESS;
			CLIP_EVENTS[BIT18] = Event.CONSTRUCT;

			BUTTON_CODES = new Dictionary<Event, int?>();
			BUTTON_CODES[Event.ROLL_OVER] = BIT0;
			BUTTON_CODES[Event.ROLL_OUT] = BIT1;
			BUTTON_CODES[Event.PRESS] = BIT2;
			BUTTON_CODES[Event.RELEASE] = BIT3;
			BUTTON_CODES[Event.DRAG_OUT] = BIT4;
			BUTTON_CODES[Event.DRAG_OVER] = BIT5;
			BUTTON_CODES[Event.RELEASE_OUT] = BIT6;

			BUTTON_EVENTS = new Dictionary<int?, Event>();
			BUTTON_EVENTS[BIT0] = Event.ROLL_OVER;
			BUTTON_EVENTS[BIT1] = Event.ROLL_OUT;
			BUTTON_EVENTS[BIT2] = Event.PRESS;
			BUTTON_EVENTS[BIT3] = Event.RELEASE;
			BUTTON_EVENTS[BIT4] = Event.DRAG_OUT;
			BUTTON_EVENTS[BIT5] = Event.DRAG_OVER;
			BUTTON_EVENTS[BIT6] = Event.RELEASE_OUT;

			MENU_CODES = new Dictionary<Event, int?>();
			MENU_CODES[Event.ROLL_OVER] = BIT0;
			MENU_CODES[Event.ROLL_OUT] = BIT1;
			MENU_CODES[Event.PRESS] = BIT2;
			MENU_CODES[Event.RELEASE] = BIT3;
			MENU_CODES[Event.RELEASE_OUT] = BIT4;
			MENU_CODES[Event.DRAG_OVER] = BIT7;
			MENU_CODES[Event.DRAG_OUT] = BIT8;

			MENU_EVENTS = new Dictionary<int?, Event>();
			MENU_EVENTS[BIT0] = Event.ROLL_OVER;
			MENU_EVENTS[BIT1] = Event.ROLL_OUT;
			MENU_EVENTS[BIT2] = Event.PRESS;
			MENU_EVENTS[BIT3] = Event.RELEASE;
			MENU_EVENTS[BIT4] = Event.RELEASE_OUT;
			MENU_EVENTS[BIT7] = Event.DRAG_OVER;
			MENU_EVENTS[BIT8] = Event.DRAG_OUT;
		}

		/// <summary>
		/// The events that the handler responds to. </summary>
		private ISet<Event> events;
		/// <summary>
		/// The code representing keyboard shortcut for the handler. </summary>
		private int key;
		/// <summary>
		/// The actions executed by the handler when the event occurs. </summary>
		private IList<Action> actions;

		/// <summary>
		/// The composite event code for all events this handler responds to. </summary>
		
		private int eventCode;
		/// <summary>
		/// The number of bytes used to encode the handler. </summary>
		
		private int length;
		/// <summary>
		/// The offset in bytes to the next handler, if any, to be decoded. </summary>
		
		private int offset;

		/// <summary>
		/// Creates and initialises a EventHandler object using values
		/// encoded in the Flash binary format.
		/// </summary>
		/// <param name="value">
		///            is decoded by and it is dependent on the parent object. If
		///            it is a Place2 or Place3 object then the event handler is for
		///            a movie clip and the value represents the the set of events
		///            that the handler responds to. If the parent object is a
		///            button then the value is the length in bytes of the encoded
		///            actions executed by the handler.
		/// </param>
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



		public EventHandler(int value, SWFDecoder coder, Context context)
		{

			int field;

		    events = new HashSet<Event>();

			if (context.contains(Context.TYPE) && context.get(Context.TYPE) == MovieTypes.DEFINE_BUTTON_2)
			{
				length = value;


				int eventKey = coder.readUnsignedShort();
				eventCode = eventKey & EVENT_MASK;
				key = (eventKey & KEY_MASK) >> KEY_OFFSET;

				if (context.contains(Context.MENU_BUTTON))
				{
					for (int i = 0; i < NUM_BUTTON_EVENTS; i++)
					{
						field = eventCode & (1 << i);
						if (MENU_EVENTS.ContainsKey(field))
						{
							events.Add(MENU_EVENTS[field]);
						}
					}
				}
				else
				{
					for (int i = 0; i < NUM_BUTTON_EVENTS; i++)
					{
						field = eventCode & (1 << i);
						if (field != 0 && BUTTON_EVENTS.ContainsKey(field))
						{
							events.Add(BUTTON_EVENTS[field]);
						}
					}
				}
			}
			 else
			 {
				eventCode = value;
				length = coder.readInt();
				if ((eventCode & CLIP_CODES[Event.KEY_PRESS]) != 0)
				{
					key = coder.readByte();
					length -= 1;
				}
				for (int i = 0; i < NUM_CLIP_EVENTS; i++)
				{
					field = eventCode & (1 << i);
					if (field != 0 && CLIP_EVENTS.ContainsKey(field))
					{
						events.Add(CLIP_EVENTS[field]);
					}
				}
			 }

			actions = new List<Action>();



			SWFFactory<Action> decoder = context.Registry.ActionDecoder;

			if (decoder == null)
			{
				if (length != 0)
				{
					actions.Add(new ActionData(coder.readBytes(new byte[length])));
				}
			}
			else
			{
				coder.mark();
				while (coder.bytesRead() < length)
				{
					decoder.getObject(actions, coder, context);
				}
				coder.unmark();
			}
		}

		/// <summary>
		/// Creates a ClipEvent object that with a list of actions that will be
		/// executed when a particular event occurs.
		/// </summary>
		/// <param name="event">
		///            the set of Events that the handler will respond to. </param>
		/// <param name="list">
		///            the list of actions that will be executed when the specified
		///            event occurs. </param>


		public EventHandler(ISet<Event> @event, IList<Action> list)
		{
			Events = @event;
			Actions = list;
		}

		/// <summary>
		/// Creates an EventHandler object that defines the list of actions that
		/// will be executed when a particular event occurs or when the specified
		/// key is pressed.
		/// </summary>
		/// <param name="event">
		///            the set of Events that the handler will respond to. </param>
		/// <param name="character">
		///            the ASCII code for the key pressed on the keyboard. </param>
		/// <param name="list">
		///            the list of actions that will be executed when the specified
		///            event occurs. Must not be null. </param>


		public EventHandler(ISet<Event> @event, int character, IList<Action> list)
		{
			Events = @event;
			Key = character;
			Actions = list;
		}

		/// <summary>
		/// Creates and initialises a EventHandler object using the values
		/// copied from another EventHandler object.
		/// </summary>
		/// <param name="object">
		///            a EventHandler object from which the values will be
		///            copied. </param>


		public EventHandler(EventHandler @object)
		{
			events = @object.events;
			key = @object.key;
			actions = new List<Action>(@object.actions);
		}

		/// <summary>
		/// Get the value that is encoded to represent the set of events that the
		/// handler responds to.
		/// 
		/// NOTE: This method is only used by Place2 and Place3 objects to encode
		/// EventHandlers for movie clips. It should not be used.
		/// </summary>
		/// <returns> the value representing the set of encoded events. </returns>
		public int EventCode => eventCode;

	    /// <summary>
		/// Get the set of events that the handler responds to. </summary>
		/// <returns> a set of Events. </returns>
		public ISet<Event> Events
		{
			get => events;
	        set => events = value;
	    }


		/// <summary>
		/// Get the code for the key that triggers the event when pressed. The
		/// code is typically the ASCII code for standard western keyboards.
		/// </summary>
		/// <returns> the ASCII code for the key that triggers the event. </returns>
		public int Key
		{
			get => key;
		    set => key = value;
		}


		/// <summary>
		/// Get the list of actions that are executed by the movie clip.
		/// </summary>
		/// <returns> the actions executed by the handler. </returns>
		public IList<Action> Actions
		{
			get => actions;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				actions = value;
			}
		}


		/// <summary>
		/// Adds an action to the list of actions.
		/// </summary>
		/// <param name="anAction">
		///            an action object. Must not be null. </param>
		/// <returns> this object. </returns>


		public EventHandler add(Action anAction)
		{
			if (anAction == null)
			{
				throw new ArgumentException();
			}
			actions.Add(anAction);
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public EventHandler copy()
		{
			return new EventHandler(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, events, key, actions);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			//CHECKSTYLE:OFF
			eventCode = 0;

			if (context.contains(Context.TYPE) && context.get(Context.TYPE) == MovieTypes.DEFINE_BUTTON_2)
			{
				if (context.contains(Context.MENU_BUTTON))
				{
					foreach (Event @event in events)
					{
						eventCode |= (int)MENU_CODES[@event];
					}
				}
				else
				{
					foreach (Event @event in events)
					{
						eventCode |= (int)BUTTON_CODES[@event];
					}
				}

				length = 4;
				foreach (Action action in actions)
				{
					length += action.prepareToEncode(context);
				}
				if (context.contains(Context.LAST))
				{
					offset = -2;
				}
				else
				{
					offset = length - 2;
				}
			}
			else
			{
				foreach (Event @event in events)
				{
					eventCode |= (int)CLIP_CODES[@event];
				}

				if (context.get(Context.VERSION) >= EVENTS_VERSION)
				{
					length = 8;
				}
				else
				{
					length = 6;
				}
				offset = (eventCode & CLIP_CODES[Event.KEY_PRESS]) == 0 ? 0 : 1;

				foreach (Action action in actions)
				{
					offset += action.prepareToEncode(context);
				}

				length += offset;
			}
			return length;
			//CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			if (context.contains(Context.TYPE) && context.get(Context.TYPE) == MovieTypes.DEFINE_BUTTON_2)
			{
				coder.writeShort(offset + 2);
				coder.writeShort((key << KEY_OFFSET) | eventCode);
			}
			else
			{
				if (context.get(Context.VERSION) >= EVENTS_VERSION)
				{
					coder.writeInt(eventCode);
				}
				else
				{
					coder.writeShort(eventCode);
				}

				coder.writeInt(offset);

				if ((eventCode & CLIP_CODES[Event.KEY_PRESS]) != 0)
				{
					coder.writeByte(key);
				}
			}

			foreach (Action action in actions)
			{
				action.encode(coder, context);
			}
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}