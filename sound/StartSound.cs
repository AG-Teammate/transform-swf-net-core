using System;
using com.flagstone.transform.coder;

/*
 * StartSound.java
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

namespace com.flagstone.transform.sound
{
    /// <summary>
	/// StartSound instructs the player to start or stop playing a sound defined
	/// using the DefineSound class.
	/// 
	/// <para>
	/// StartSound contains a SoundInfo object that defines how the sound fades in
	/// and out, whether it is repeated as well as specifying an envelope that
	/// provides a finer degree of control over the levels at which the sound is
	/// played.
	/// </para>
	/// </summary>
	/// <seealso cref= DefineSound </seealso>
	/// <seealso cref= SoundInfo </seealso>
	public sealed class StartSound : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "StartSound: { sound=%s}";

		/// <summary>
		/// Describes how the sound is played. </summary>
		private SoundInfo sound;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a StartSound object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public StartSound(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			sound = new SoundInfo(coder.readUnsignedShort(), coder);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a StartSound object with an Sound object that identifies the
		/// sound and controls how it is played.
		/// </summary>
		/// <param name="aSound">
		///            the Sound object. Must not be null. </param>


		public StartSound(SoundInfo aSound)
		{
			Sound = aSound;
		}

		/// <summary>
		/// Creates and initialises a StartSound object using the values copied
		/// from another StartSound object.
		/// </summary>
		/// <param name="object">
		///            a StartSound object from which the values will be
		///            copied. </param>


		public StartSound(StartSound @object)
		{
			sound = @object.sound.copy();
		}

		/// <summary>
		/// Get the Sound object describing how the sound will be played.
		/// </summary>
		/// <returns> the SoundInfo object that controls the playback. </returns>
		public SoundInfo Sound
		{
			get => sound;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				sound = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public StartSound copy()
		{
			return new StartSound(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, sound);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = sound.prepareToEncode(context);
			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.START_SOUND << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.START_SOUND << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			sound.encode(coder, context);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}