using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * SoundStreamHead2.java
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
	/// SoundStreamHead2 defines the format of a streaming sound, identifying the
	/// encoding scheme, the rate at which the sound will be played and the size of
	/// the decoded samples.
	/// 
	/// <para>
	/// Sounds may be either mono or stereo and encoded using either NATIVE_PCM,
	/// ADPCM, MP3 or NELLYMOSER or SPEEX formats and have sampling rates of 5512,
	/// 11025, 22050 or 44100 Hertz.
	/// </para>
	/// 
	/// <para>
	/// The actual sound is streamed used the SoundStreamBlock class which contains
	/// the data for each frame in a movie.
	/// </para>
	/// 
	/// <para>
	/// When a stream sound is played if the Flash Player cannot render the frames
	/// fast enough to maintain synchronisation with the sound being played then
	/// frames will be skipped. Normally the player will reduce the frame rate so
	/// every frame of a movie is played. The different sets of attributes that
	/// identify how the sound will be played compared to the way it was encoded
	/// allows the Player more control over how the animation is rendered. Reducing
	/// the resolution or playback rate can improve synchronisation with the frames
	/// displayed.
	/// </para>
	/// 
	/// <para>
	/// SoundStreamHead2 allows way the sound is played to differ from the way it is
	/// encoded and streamed to the player. This allows the Player more control over
	/// how the animation is rendered. Reducing the resolution or playback rate can
	/// improve synchronisation with the frames displayed.
	/// </para>
	/// </summary>
	/// <seealso cref= SoundStreamBlock </seealso>
	/// <seealso cref= SoundStreamHead </seealso>
	public sealed class SoundStreamHead2 : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "SoundStreamHead2: { format=%s;" + " playbackRate=%d; playbackChannels=%d; playbackSampleSize=%d;" + " streamRate=%d; streamChannels=%d; streamSampleSize=%d;" + " streamSampleCount=%d; latency=%d}";

		/// <summary>
		/// The code representing the sound format. </summary>
		private int format;
		/// <summary>
		/// The playback rate in KHz. </summary>
		private int playRate;
		/// <summary>
		/// The number of playback channels: 1 = mono, 2 = stereo. </summary>
		private int playChannels;
		/// <summary>
		/// The number of bits in each sample. </summary>
		private int playSampleSize;
		/// <summary>
		/// The sound rate in KHz of the stream. </summary>
		private int streamRate;
		/// <summary>
		/// The number of channels in the stream: 1 = mono, 2 = stereo. </summary>
		private int streamChannels;
		/// <summary>
		/// The number of bits in each stream sample. </summary>
		private int streamSampleSize;
		/// <summary>
		/// The number of samples in the stream. </summary>
		private int streamSampleCount;
		/// <summary>
		/// The latency for MP3 sounds. </summary>
		private int latency;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// The following variable is used to preserve the value of a reserved field
		/// when decoding then encoding an existing Flash file. Macromedia's file
		/// file format specification states that this field is always zero - it is
		/// not, so this is used to preserve the value in case it is implementing an
		/// undocumented feature.
		/// </summary>
		
		private int reserved;

		/// <summary>
		/// Creates and initialises a SoundStreamHead2 object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public SoundStreamHead2(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			int info = coder.readByte();
			reserved = (info & Coder.NIB1) >> Coder.TO_LOWER_NIB;
			playRate = readRate(info & Coder.PAIR1);
			playSampleSize = ((info & Coder.BIT1) >> 1) + 1;
			playChannels = (info & Coder.BIT0) + 1;

			info = coder.readByte();
			format = (info & Coder.NIB1) >> Coder.TO_LOWER_NIB;
			streamRate = readRate(info & Coder.PAIR1);
			streamSampleSize = ((info & Coder.BIT1) >> 1) + 1;
			streamChannels = (info & Coder.BIT0) + 1;
			streamSampleCount = coder.readUnsignedShort();

			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			if ((length == 6) && (format == 2))
			{
				latency = coder.readSignedShort();
			}
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a SoundStreamHead2 object specifying all the parameters required
		/// to define the sound.
		/// </summary>
		/// <param name="encoding">
		///            the compression format for the sound data, either
		///            DefineSound.NATIVE_PCM, DefineSound.ADPCM, DefineSound.MP3,
		///            DefineSound.PCM or DefineSound.NELLYMOSER (Flash 6+ only). </param>
		/// <param name="playbackRate">
		///            the recommended rate for playing the sound, either 5512,
		///            11025, 22050 or 44100 Hz. </param>
		/// <param name="playbackChannels">
		///            The recommended number of playback channels: 1 = mono or 2 =
		///            stereo. </param>
		/// <param name="playSize">
		///            the recommended uncompressed sample size for playing the
		///            sound, either 1 or 2 bytes. </param>
		/// <param name="streamingRate">
		///            the rate at which the sound was sampled, either 5512, 11025,
		///            22050 or 44100 Hz. </param>
		/// <param name="streamingChannels">
		///            the number of channels: 1 = mono or 2 = stereo. </param>
		/// <param name="streamingSize">
		///            the sample size for the sound, either 1 or 2 bytes. </param>
		/// <param name="streamingCount">
		///            the number of samples in each subsequent SoundStreamBlock
		///            object. </param>


		public SoundStreamHead2(SoundFormat encoding, int playbackRate, int playbackChannels, int playSize, int streamingRate, int streamingChannels, int streamingSize, int streamingCount)
		{
			Format = encoding;
			PlayRate = playbackRate;
			PlayChannels = playbackChannels;
			PlaySampleSize = playSize;
			setStreamRate(streamingRate);
			StreamChannels = streamingChannels;
			StreamSampleSize = streamingSize;
			StreamSampleCount = streamingCount;
		}

		/// <summary>
		/// Creates and initialises a SoundStreamHead2 object using the values copied
		/// from another SoundStreamHead2 object.
		/// </summary>
		/// <param name="object">
		///            a SoundStreamHead2 object from which the values will be
		///            copied. </param>


		public SoundStreamHead2(SoundStreamHead2 @object)
		{
			format = @object.format;
			playRate = @object.playRate;
			playChannels = @object.playChannels;
			playSampleSize = @object.playSampleSize;
			streamRate = @object.streamRate;
			streamChannels = @object.streamChannels;
			streamSampleSize = @object.streamSampleSize;
			streamSampleCount = @object.streamSampleCount;
			latency = @object.latency;
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
		/// Returns the recommended playback rate: 5512, 11025, 22050 or 44100 Hz.
		/// </summary>
		/// <returns> the playback rate in Hertz. </returns>
		public int PlayRate
		{
			get => playRate;
		    set
			{
				if ((value != SoundRate.KHZ_5K) && (value != SoundRate.KHZ_11K) && (value != SoundRate.KHZ_22K) && (value != SoundRate.KHZ_44K))
				{
					throw new IllegalArgumentValueException(new[] {SoundRate.KHZ_5K, SoundRate.KHZ_11K, SoundRate.KHZ_22K, SoundRate.KHZ_44K}, value);
				}
				playRate = value;
			}
		}

		/// <summary>
		/// Get the recommended number of playback channels = 1 = mono 2 =
		/// stereo.
		/// </summary>
		/// <returns> the number of channels. </returns>
		public int PlayChannels
		{
			get => playChannels;
		    set
			{
				if ((value < 1) || (value > 2))
				{
					throw new IllegalArgumentRangeException(1, 2, value);
				}
				playChannels = value;
			}
		}

		/// <summary>
		/// Get the recommended playback sample range in bytes: 1 or 2.
		/// </summary>
		/// <returns> the number of bytes in each sample. </returns>
		public int PlaySampleSize
		{
			get => playSampleSize;
		    set
			{
				if ((value < 1) || (value > 2))
				{
					throw new IllegalArgumentRangeException(1, 2, value);
				}
				playSampleSize = value;
			}
		}

		/// <summary>
		/// Get the sample rate: 5512, 11025, 22050 or 44100 Hz in the streaming
		/// sound.
		/// </summary>
		/// <returns> the stream rate in Hertz. </returns>
		public float getStreamRate()
		{
			return streamRate;
		}

		/// <summary>
		/// Get the number of channels, 1 = mono 2 = stereo, in the streaming
		/// sound.
		/// </summary>
		/// <returns> the number of channels defined in the streaming sound. </returns>
		public int StreamChannels
		{
			get => streamChannels;
		    set
			{
				if ((value < 1) || (value > 2))
				{
					throw new IllegalArgumentRangeException(1, 2, value);
				}
				streamChannels = value;
			}
		}

		/// <summary>
		/// Get the sample size in bytes: 1 or 2 in the streaming sound.
		/// </summary>
		/// <returns> the number of bytes per sample in the streaming sound. </returns>
		public int StreamSampleSize
		{
			get => streamSampleSize;
		    set
			{
				if ((value < 1) || (value > 2))
				{
					throw new IllegalArgumentRangeException(1, 2, value);
				}
				streamSampleSize = value;
			}
		}

		/// <summary>
		/// Get the average number of samples in each stream block following.
		/// </summary>
		/// <returns> the number of sample in each stream block. </returns>
		public int StreamSampleCount
		{
			get => streamSampleCount;
		    set
			{
				if (value < 0)
				{
					throw new IllegalArgumentRangeException(0, int.MaxValue, value);
				}
				streamSampleCount = value;
			}
		}




		/// <summary>
		/// Sets the sample rate in Hz for the streaming sound. Must be either: 5512,
		/// 11025, 22050 or 44100.
		/// </summary>
		/// <param name="rate">
		///            the rate at which the streaming sound was sampled. </param>


		public void setStreamRate(int rate)
		{
			if ((rate != SoundRate.KHZ_5K) && (rate != SoundRate.KHZ_11K) && (rate != SoundRate.KHZ_22K) && (rate != SoundRate.KHZ_44K))
			{
				throw new IllegalArgumentValueException(new[] {SoundRate.KHZ_5K, SoundRate.KHZ_11K, SoundRate.KHZ_22K, SoundRate.KHZ_44K}, rate);
			}
			streamRate = rate;
		}




		/// <summary>
		/// For MP3 encoded sounds, returns the number of samples to skip when
		/// starting to play a sound.
		/// </summary>
		/// <returns> the number of samples skipped in an MP3 encoded sound Returns 0
		///         for other sound formats. </returns>
		public int Latency
		{
			get => latency;
		    set => latency = value;
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public SoundStreamHead2 copy()
		{
			return new SoundStreamHead2(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, format, playRate, playChannels, playSampleSize, streamRate, streamChannels, streamSampleSize, streamSampleCount, latency);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			length = 4;

			if ((format == 2) && (latency > 0))
			{
				length += 2;
			}
			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.SOUND_STREAM_HEAD_2 << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.SOUND_STREAM_HEAD_2 << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			int bits = reserved << Coder.TO_UPPER_NIB;
			bits |= writeRate(playRate);
			bits |= (playSampleSize - 1) << 1;
			bits |= playChannels - 1;
			coder.writeByte(bits);

			bits = format << Coder.TO_UPPER_NIB;
			bits |= writeRate(streamRate);
			bits |= (streamSampleSize - 1) << 1;
			bits |= streamChannels - 1;
			coder.writeByte(bits);
			coder.writeShort(streamSampleCount);

			if ((format == 2) && (latency > 0))
			{
				coder.writeShort(latency);
			}
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}

		/// <summary>
		/// Convert the code representing the rate into actual KHz. </summary>
		/// <param name="value"> the code representing the sound rate. </param>
		/// <returns> the actual rate in KHz. </returns>


		private int readRate(int value)
		{
			int rate;
			switch (value)
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
			case Coder.BIT2 | Coder.BIT3:
				rate = SoundRate.KHZ_44K;
				break;
			default:
				rate = 0;
				break;
			}
			return rate;
		}

		/// <summary>
		/// Convert the rate in KHz to the code that represents the rate. </summary>
		/// <param name="rate"> the rate in KHz. </param>
		/// <returns> the code representing the sound rate. </returns>


		private int writeRate(int rate)
		{
			int value;
			switch (rate)
			{
			case SoundRate.KHZ_11K:
				value = Coder.BIT2;
				break;
			case SoundRate.KHZ_22K:
				value = Coder.BIT3;
				break;
			case SoundRate.KHZ_44K:
				value = Coder.BIT2 | Coder.BIT3;
				break;
			default:
				value = 0;
				break;
			}
			return value;
		}
	}

}