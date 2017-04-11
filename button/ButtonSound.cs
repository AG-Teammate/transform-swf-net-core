using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;
using com.flagstone.transform.sound;

/*
 * ButtonSound.java
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
	/// ButtonSound defines the sounds that are played when an event occurs in a
	/// button. Sounds are only played for the RollOver, RollOut, Press and Release
	/// events.
	/// 
	/// <para>
	/// For each event a <seealso cref="SoundInfo"/> object identifies the sound and controls
	/// how it is played. For events where no sound should be played simply specify a
	/// null value instead of a SoundInfo object.
	/// </para>
	/// </summary>
	/// <seealso cref= DefineButton </seealso>
	/// <seealso cref= DefineButton2 </seealso>
	public sealed class ButtonSound : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "ButtonSound: { identifier=%d;" + " table=%s}";

		/// <summary>
		/// The set of button events that support sounds. </summary>
		//private static readonly EnumSet<Event> EVENTS = EnumSet.of(Event.ROLL_OUT, Event.ROLL_OVER, Event.PRESS, Event.RELEASE);
        static Event[] EVENTS = { Event.ROLL_OUT, Event.ROLL_OVER, Event.PRESS, Event.RELEASE };

		/// <summary>
		/// The unique identifier of the button. </summary>
		private int identifier;
		/// <summary>
		/// Table of sounds played for different button events. </summary>
		
		private readonly IDictionary<Event, SoundInfo> table;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a ButtonSound object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public ButtonSound(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			table = new Dictionary<Event, SoundInfo>();
			decodeInfo(Event.ROLL_OUT, coder);
			decodeInfo(Event.ROLL_OVER, coder);
			decodeInfo(Event.PRESS, coder);
			decodeInfo(Event.RELEASE, coder);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Decoder the optional sound for each button event. </summary>
		/// <param name="event"> the button event. </param>
		/// <param name="coder"> the SWFDecoder containing the encoded data. </param>
		/// <exception cref="IOException"> if an error occurs decoding the sound. </exception>



		private void decodeInfo(Event @event, SWFDecoder coder)
		{
			if (coder.bytesRead() < length)
			{


				int uid = coder.readUnsignedShort();
				if (uid != 0)
				{
					table[@event] = new SoundInfo(uid, coder);
				}
			}
		}

		/// <summary>
		/// Creates a ButtonSound object that defines the sound played for a single
		/// button event.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier of the DefineButton or DefineButton2
		///            object that defines the button. Must be in the range 1..65535. </param>
		/// <param name="eventCode">
		///            the event that identifies when the sound id played, must be
		///            either Event.EventType.rollOver,
		///            Event.EventType.rollOut, Event.EventType.press or
		///            Event.EventType.release. </param>
		/// <param name="aSound">
		///            an SoundInfo object that identifies a sound and controls how
		///            it is played. </param>


		public ButtonSound(int uid, Event eventCode, SoundInfo aSound)
		{
			table = new Dictionary<Event, SoundInfo>();
			Identifier = uid;
			setSoundInfo(eventCode, aSound);
		}

		/// <summary>
		/// Creates and initialises a ButtonSound object using the values copied
		/// from another ButtonSound object.
		/// </summary>
		/// <param name="object">
		///            a ButtonSound object from which the values will be
		///            copied. </param>


		public ButtonSound(ButtonSound @object)
		{

			identifier = @object.identifier;
			table = new Dictionary<Event, SoundInfo>();

			foreach (Event @event in @object.table.Keys)
			{
				table[@event] = @object.table[@event].copy();
			}
		}

		/// <summary>
		/// Get the unique identifier of the button that this object applies to.
		/// </summary>
		/// <returns> the unique identifier of the sound. </returns>
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
		/// Returns the SoundInfo object for the specified event. Null is returned if
		/// there is no SoundInfo object defined for the event code.
		/// </summary>
		/// <param name="event">
		///            The button event, must be one of Event.ROLL_OVER,
		///            Event.ROLL_OUT, Event.PRESS, Event.RELEASE. </param>
		/// <returns> the SoundInfo that identifies and controls the sound that will be
		///            played for the event or null if not SoundInfo is defined for
		///            the event. </returns>


		public SoundInfo getSoundInfo(Event @event)
		{
			 return table[@event];
		}


		/// <summary>
		/// Sets the SoundInfo object for the specified button event. The argument
		/// may be null allowing the SoundInfo object for a given event to be
		/// deleted.
		/// </summary>
		/// <param name="event">
		///            the code representing the button event, must be either
		///            Event.EventType.RollOver, Event.EventType.RollOut,
		///            Event.EventType.Press or Event.EventType.Release. </param>
		/// <param name="info">
		///            an SoundInfo object that identifies and controls how the sound
		///            is played. </param>


		public void setSoundInfo(Event @event, SoundInfo info)
		{
			table[@event] = info;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public ButtonSound copy()
		{
			return new ButtonSound(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, table.ToString());
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 2;

			foreach (Event @event in EVENTS)
			{
				if (table.ContainsKey(@event))
				{
					length += table[@event].prepareToEncode(context);
				}
				else
				{
					length += 2;
				}
			}
			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.BUTTON_SOUND << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.BUTTON_SOUND << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);

			foreach (Event @event in EVENTS)
			{
				if (table.ContainsKey(@event))
				{
					table[@event].encode(coder, context);
				}
				else
				{
					coder.writeShort(0);
				}
			}
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}