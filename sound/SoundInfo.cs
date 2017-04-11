/*
 * SoundInfo.java
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

using System;
using System.IO;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

namespace com.flagstone.transform.sound
{
    /// <summary>
	/// SoundInfo identifies a sound (previously defined using The DefineSound class)
	/// and controls how it is played.
	/// 
	/// <para>
	/// SoundInfo defines how the sound fades in and out, whether it is repeated as
	/// well as specifying an envelope that provides a finer degree of control over
	/// the levels at which the sound is played.
	/// </para>
	/// 
	/// <para>
	/// The in and out point specify the sample number which marks the point in time
	/// at which the sound stops increasing or starts decreasing in volume
	/// respectively. Sounds are played by the Flash player at 44.1KHz so the sample
	/// number also indicates the time when the total number of samples in the sound
	/// is taken into account.
	/// </para>
	/// 
	/// <para>
	/// Not all the attributes are required to play a sound. Only the identifier and
	/// the mode is required. The other attributes are optional and may be added as a
	/// greater degree of control is required. The inPoint and outPoint attributes
	/// may be set to zero if the sound does not fade in or out respectively. The
	/// loopCount may be set to zero if a sound is being stopped. The envelopes
	/// may be left empty if no envelope is defined for the sound. The class provides
	/// different constructors to specify different sets of attributes.
	/// </para>
	/// </summary>
	/// <seealso cref= DefineSound </seealso>
	public sealed class SoundInfo : SWFEncodeable
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "SoundInfo: { identifier=%d; mode=%s;" + " inPoint=%d; outPoint=%d; loopCount=%d; envelopes=%s}";

		/// <summary>
		/// Mode describes how the sound is controlled. </summary>
		public enum Mode
		{
			/// <summary>
			/// Start playing the sound. </summary>
			START,
			/// <summary>
			/// Start playing the sound or continues if it is already playing. </summary>
			CONTINUE,
			/// <summary>
			/// Stop playing the sound. </summary>
			STOP
		}

		/// <summary>
		/// The unique identifier of the sound that this info applies to. </summary>
		private int identifier;
		/// <summary>
		/// Controls whether the sound starts or stops. </summary>
		private int mode;
		/// <summary>
		/// The number of samples to fade the sound in. </summary>
		private int? inPoint;
		/// <summary>
		/// The number of samples to fade the sound out. </summary>
		private int? outPoint;
		/// <summary>
		/// The number of time the sound will be repeated. </summary>
		private int? loopCount;
		/// <summary>
		/// The envelope that controls how the sound is played. </summary>
		private Envelope envelope;

		/// <summary>
		/// Creates and initialises a SoundInfo object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="uid"> the unique identifier for the sound definition - decoded by
		/// the parent object.
		/// </param>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public SoundInfo(int uid, SWFDecoder coder)
		{
			identifier = uid;



			int info = coder.readByte();
			mode = info & Coder.NIB1;

			if ((info & Coder.BIT0) != 0)
			{
				inPoint = coder.readInt();
			}

			if ((info & Coder.BIT1) != 0)
			{
				outPoint = coder.readInt();
			}

			if ((info & Coder.BIT2) != 0)
			{
				loopCount = coder.readUnsignedShort();
			}

			if ((info & Coder.BIT3) != 0)
			{
				envelope = new Envelope(coder);
			}
		}

		/// <summary>
		/// Creates a Sound object specifying how the sound is played and the number
		/// of times the sound is repeated.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier of the object that contains the sound
		///            data. </param>
		/// <param name="aMode">
		///            how the sound is synchronised when the frames are displayed:
		///            Play - do not play the sound if it is already playing and Stop
		///            - stop playing the sound. </param>
		/// <param name="aCount">
		///            the number of times the sound is repeated. May be set to zero
		///            if the sound will not be repeated. </param>
		/// <param name="anEnvelope">
		///            the Envelope that control the levels the sound is played. </param>


		public SoundInfo(int uid, Mode aMode, int aCount, Envelope anEnvelope)
		{
			Identifier = uid;
			setMode(aMode);
			LoopCount = aCount;
			Envelope = anEnvelope;
		}

		/// <summary>
		/// Creates and initialises a SoundInfo object using the values copied
		/// from another SoundInfo object.
		/// </summary>
		/// <param name="object">
		///            a SoundInfo object from which the values will be
		///            copied. </param>


		public SoundInfo(SoundInfo @object)
		{
			identifier = @object.identifier;
			mode = @object.mode;
			loopCount = @object.loopCount;
			inPoint = @object.inPoint;
			outPoint = @object.outPoint;

			if (@object.envelope != null)
			{
				envelope = @object.envelope.copy();
			}
		}

		/// <summary>
		/// Get the identifier of the sound to the played.
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
		/// Get the synchronisation mode: START - start playing the sound,
		/// CONTINUE - do not play the sound if it is already playing and STOP - stop
		/// playing the sound.
		/// </summary>
		/// <returns> the sound synchronisation mode. </returns>
		public Mode getMode()
		{
			Mode value;
			switch (mode)
			{
			case 0:
				value = Mode.START;
				break;
			case Coder.BIT4:
				value = Mode.CONTINUE;
				break;
			case Coder.BIT5:
				value = Mode.STOP;
				break;
			default:
				throw new InvalidOperationException();
			}
			return value;
		}

		/// <summary>
		/// Get the sample number at which the sound reaches full volume when
		/// fading in.
		/// </summary>
		/// <returns> the fade in point. </returns>
		public int? InPoint
		{
			get => inPoint;
		    set
			{
				if ((value != null) && ((value < 0) || (value > Coder.USHORT_MAX)))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value.Value);
				}
				inPoint = value;
			}
		}

		/// <summary>
		/// Get the sample number at which the sound starts to fade.
		/// </summary>
		/// <returns> the fade out point. </returns>
		public int? OutPoint
		{
			get => outPoint;
		    set
			{
				if ((value != null) && ((value < 0) || (value > Coder.USHORT_MAX)))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value.Value);
				}
				outPoint = value;
			}
		}

		/// <summary>
		/// Get the number of times the sound will be repeated.
		/// </summary>
		/// <returns> the number of loops. </returns>
		public int? LoopCount
		{
			get => loopCount;
		    set
			{
				if ((value != null) && ((value < 0) || (value > Coder.USHORT_MAX)))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value.Value);
				}
				loopCount = value;
			}
		}

		/// <summary>
		/// Get the Envelope that control the levels the sound is played.
		/// </summary>
		/// <returns> the sound envelope. </returns>
		public Envelope Envelope
		{
			get => envelope;
		    set => envelope = value;
		}


		/// <summary>
		/// Sets how the sound is synchronised when the frames are displayed: START -
		/// start playing the sound, CONTINUE - do not play the sound if it is
		/// already playing and STOP - stop playing the sound.
		/// </summary>
		/// <param name="soundMode">
		///            how the sound is played. </param>


		public void setMode(Mode soundMode)
		{
			switch (soundMode)
			{
			case Mode.START:
				mode = 0;
				break;
			case Mode.CONTINUE:
				mode = Coder.BIT4;
				break;
			case Mode.STOP:
				mode = Coder.BIT5;
				break;
			default:
				throw new ArgumentException();
			}
		}





		/// <summary>
		/// {@inheritDoc} </summary>
		public SoundInfo copy()
		{
			return new SoundInfo(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, mode, inPoint, outPoint, loopCount, envelope);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			int length = 3;
			if (inPoint != null)
			{
				length += 4;
			}
			if (outPoint != null)
			{
				length += 4;
			}
			if (loopCount != null)
			{
				length += 2;
			}
			if (envelope != null)
			{
				length += envelope.prepareToEncode(context);
			}
			return length;
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>




		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeShort(identifier);

			int bits = mode;
			bits |= envelope == null ? 0 : Coder.BIT3;
			bits |= loopCount == null ? 0 : Coder.BIT2;
			bits |= outPoint == null ? 0 : Coder.BIT1;
			bits |= inPoint == null ? 0 : Coder.BIT0;
			coder.writeByte(bits);

			if (inPoint != null)
			{
				coder.writeInt(inPoint.Value);
			}
			if (outPoint != null)
			{
				coder.writeInt(outPoint.Value);
			}
			if (loopCount != null)
			{
				coder.writeShort(loopCount.Value);
			}
		    envelope?.encode(coder, context);
		}
	}

}