using System;
using com.flagstone.transform.coder;

/*
 * SoundStreamBlock.java
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
	/// SoundStreamBlock contains the sound data being streamed to the Flash Player.
	/// 
	/// <para>
	/// Streaming sounds are played in tight synchronisation with one
	/// SoundStreamBlock object defining the sound for each frame displayed in a
	/// movie. When a streaming sound is played if the Flash Player cannot render the
	/// frames fast enough to maintain synchronisation with the sound being played
	/// then frames will be skipped. Normally the player will reduce the frame rate
	/// so every frame of a movie is played.
	/// </para>
	/// </summary>
	/// <seealso cref= SoundStreamHead </seealso>
	/// <seealso cref= SoundStreamHead2 </seealso>
	public sealed class SoundStreamBlock : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "SoundStreamBlock: {" + "sound=byte<%d> ...}";

		/// <summary>
		/// Encoded sound data. </summary>
		private byte[] sound;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a SoundStreamBlock object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public SoundStreamBlock(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			sound = coder.readBytes(new byte[length]);
		}

		/// <summary>
		/// Creates a SoundStreamBlock specifying the sound data in the format
		/// defined by a preceding SoundStreamHead or SoundStreamHead2 object.
		/// </summary>
		/// <param name="bytes">
		///            a list of bytes containing the sound data. Must not be null. </param>


		public SoundStreamBlock(byte[] bytes)
		{
			Sound = bytes;
		}

		/// <summary>
		/// Creates and initialises a SoundStreamBlock object using the values copied
		/// from another SoundStreamBlock object.
		/// </summary>
		/// <param name="object">
		///            a SoundStreamBlock object from which the values will be
		///            copied. </param>


		public SoundStreamBlock(SoundStreamBlock @object)
		{
			sound = @object.sound;
		}

		/// <summary>
		/// Get a copy of the sound data in the format defined by a preceding
		/// SoundStreamHead or SoundStreamHead2 object.
		/// </summary>
		/// <returns> a copy of the sound. </returns>
		public byte[] Sound
		{
			get => Arrays.copyOf(sound, sound.Length);
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				sound = Arrays.copyOf(value, value.Length);
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public SoundStreamBlock copy()
		{
			return new SoundStreamBlock(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, sound.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = sound.Length;
			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.SOUND_STREAM_BLOCK << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.SOUND_STREAM_BLOCK << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeBytes(sound);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}