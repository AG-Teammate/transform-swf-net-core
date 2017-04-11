using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * DefineSound.java
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
	/// DefineSound is used to define a sound that will be played when a given event
	/// occurs.
	/// 
	/// <para>
	/// Three different types of object are used to play an event sound:
	/// </para>
	/// 
	/// <ul>
	/// <li>The DefineSound object that contains the sampled sound.</li>
	/// <li>A SoundInfo object that defines how the sound fades in and out, whether
	/// it repeats and also defines an envelope for more sophisticated control over
	/// how the sound is played.</li>
	/// <li>A StartSound object that signals the Flash Player to begin playing the
	/// sound.</li>
	/// </ul>
	/// 
	/// <para>
	/// Five encoded formats for the sound data are supported: NATIVE_PCM, PCM,
	/// ADPCM, MP3 and NELLYMOSER.
	/// </para>
	/// </summary>
	/// <seealso cref= SoundInfo </seealso>
	/// <seealso cref= StartSound </seealso>
	public sealed class DefineSound : DefineTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineSound: { identifier=%d;" + " format=%s; rate=%d; channelCount=%d; sampleSize=%d;" + " sampleCount=%d}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The code representing the sound format. </summary>
		private int format;
		/// <summary>
		/// The playback rate in KHz. </summary>
		private int rate;
		/// <summary>
		/// The number of channels: 1 = mono, 2 = stereo. </summary>
		private int channelCount;
		/// <summary>
		/// The number of bits in each sample. </summary>
		private int sampleSize;
		/// <summary>
		/// The number of samples. </summary>
		private int sampleCount;
		/// <summary>
		/// The sound data. </summary>
		private byte[] sound;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a DefineSound object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public DefineSound(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();



			int info = coder.readByte();

			format = (info & Coder.NIB1) >> Coder.TO_LOWER_NIB;

			switch (info & Coder.PAIR1)
			{
			case 0:
				rate = SoundRate.KHZ_5K;
				break;
			case Coder.BIT2:
				rate = SoundRate.KHZ_11K;
				break;
			case Coder.BIT3:
				rate = SoundRate.KHZ_22K;
				break;
			case (Coder.BIT2 | Coder.BIT3):
				rate = SoundRate.KHZ_44K;
				break;
			default:
				rate = 0;
				break;
			}

			sampleSize = ((info & 0x02) >> 1) + 1;
			channelCount = (info & 0x01) + 1;
			sampleCount = coder.readInt();

			sound = coder.readBytes(new byte[length - coder.bytesRead()]);
			coder.unmark();
		}

		/// <summary>
		/// Creates a DefineSound object specifying the unique identifier and all the
		/// parameters required to describe the sound.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for this sound. Must be in the range
		///            1..65535. </param>
		/// <param name="aFormat">
		///            the encoding format for the sound. For Flash 1 the formats may
		///            be one of the format: NATIVE_PCM, PCM or ADPCM. For Flash 4 or
		///            later include MP3 and Flash 6 or later include NELLYMOSER. </param>
		/// <param name="playbackRate">
		///            the number of samples per second that the sound is played at ,
		///            either 5512, 11025, 22050 or 44100. </param>
		/// <param name="channels">
		///            the number of channels in the sound, must be either 1 (Mono)
		///            or 2 (Stereo). </param>
		/// <param name="size">
		///            the size of an uncompressed sound sample in bits, must be
		///            either 8 or 16. </param>
		/// <param name="count">
		///            the number of samples in the sound data. </param>
		/// <param name="bytes">
		///            the sound data. </param>


		public DefineSound(int uid, SoundFormat aFormat, int playbackRate, int channels, int size, int count, byte[] bytes)
		{
			Identifier = uid;
			Format = aFormat;
			Rate = playbackRate;
			ChannelCount = channels;
			SampleSize = size;
			SampleCount = count;
			Sound = bytes;
		}

		/// <summary>
		/// Creates and initialises a DefineSound object using the values copied
		/// from another DefineSound object.
		/// </summary>
		/// <param name="object">
		///            a DefineSound object from which the values will be
		///            copied. </param>


		public DefineSound(DefineSound @object)
		{
			identifier = @object.identifier;
			format = @object.format;
			rate = @object.rate;
			channelCount = @object.channelCount;
			sampleSize = @object.sampleSize;
			sampleCount = @object.sampleCount;
			sound = @object.sound;
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
		/// Get the compression format used.
		/// </summary>
		/// <returns> the format for the sound data. </returns>
		public SoundFormat Format
		{
			get => SoundFormat.fromInt(format);
		    set => format = value.Value;
		}

		/// <summary>
		/// Get the rate at which the sound will be played, in Hz: 5512, 11025,
		/// 22050 or 44100.
		/// </summary>
		/// <returns> the playback rate in Hertz. </returns>
		public int Rate
		{
			get => rate;
		    set
			{
				if ((value != SoundRate.KHZ_5K) && (value != SoundRate.KHZ_11K) && (value != SoundRate.KHZ_22K) && (value != SoundRate.KHZ_44K))
				{
					throw new IllegalArgumentValueException(new[] {SoundRate.KHZ_5K, SoundRate.KHZ_11K, SoundRate.KHZ_22K, SoundRate.KHZ_44K}, value);
				}
				rate = value;
			}
		}

		/// <summary>
		/// Get the number of sound channels, 1 (Mono) or 2 (Stereo).
		/// </summary>
		/// <returns> the number of channels. </returns>
		public int ChannelCount
		{
			get => channelCount;
		    set
			{
				if ((value < 1) || (value > 2))
				{
					throw new IllegalArgumentRangeException(1, 2, value);
				}
				channelCount = value;
			}
		}

		/// <summary>
		/// Get the size of an uncompressed sample in bytes.
		/// </summary>
		/// <returns> the number of bytes in each sample. </returns>
		public int SampleSize
		{
			get => sampleSize;
		    set
			{
				if ((value < 1) || (value > 2))
				{
					throw new IllegalArgumentRangeException(1, 2, value);
				}
				sampleSize = value;
			}
		}

		/// <summary>
		/// Get the number of samples in the sound data.
		/// </summary>
		/// <returns> the number of sound samples. </returns>
		public int SampleCount
		{
			get => sampleCount;
		    set
			{
				if (value < 1)
				{
					throw new IllegalArgumentRangeException(1, int.MaxValue, value);
				}
				sampleCount = value;
			}
		}

		/// <summary>
		/// Get a copy of the sound data.
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
		public DefineSound copy()
		{
			return new DefineSound(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, format, rate, channelCount, sampleSize, sampleCount);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			length = 7;
			length += sound.Length;

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_SOUND << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_SOUND << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);

			int bits = format << Coder.TO_UPPER_NIB;

			switch (rate)
			{
			case SoundRate.KHZ_11K:
				bits |= Coder.BIT2;
				break;
			case SoundRate.KHZ_22K:
				bits |= Coder.BIT3;
				break;
			case SoundRate.KHZ_44K:
				bits |= Coder.BIT2;
				bits |= Coder.BIT3;
				break;
			default:
				break;
			}
			bits |= (sampleSize - 1) << 1;
			bits |= channelCount - 1;
			coder.writeByte(bits);
			coder.writeInt(sampleCount);
			coder.writeBytes(sound);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}