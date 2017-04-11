using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * Envelope.java
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
	/// Envelope is a container class for Level objects that describe how the volume
	/// of a sound changes over time.
	/// </summary>
	public sealed class Envelope : SWFEncodeable
	{
		/// <summary>
		/// Level describes the sound levels for stereo sound.
		/// 
		/// <para>
		/// Each Level object contains a sample number in the audio <b>when it is
		/// played</b> where the envelope will be applied along with the sound levels
		/// for the left and right channels.
		/// </para>
		/// 
		/// <para>
		/// The Flash Player plays sounds at a fixed rate of 44.1KHz, therefore
		/// sounds sampled at a lower frequency are interpolated with each sample
		/// repeated to generated the 44.1Khz playback rate. For example each sample
		/// in a sound sampled at 22KHz is played twice to generated the 44.1Khz
		/// playback rate.
		/// </para>
		/// 
		/// <para>
		/// The Level defines the sample number (and hence the time) in the
		/// playback data stream where the level information applies and <b>not</b>
		/// the sample number in the original sound data. For example to set the
		/// level 0.1 seconds into a sound that plays for 1 second the value for the
		/// mark attribute in the envelope object would be 44100 * 0.1/1.0 = 4410.
		/// </para>
		/// </summary>
		/// <seealso cref= SoundInfo </seealso>
		public sealed class Level : SWFEncodeable
		{

			/// <summary>
			/// Format string used in toString() method. </summary>
			internal const string FORMAT = "Envelope: { mark=%d; left=%d;" + " right=%d}";

			/// <summary>
			/// The sample number (at 44KHz) where the levels apply. </summary>
			
			internal readonly int mark;
			/// <summary>
			/// The sound level for the left channel. </summary>
			
			internal readonly int left;
			/// <summary>
			/// The sound level for the right channel. </summary>
			
			internal readonly int right;

			/// <summary>
			/// Creates and initialises a sound Level object using values encoded
			/// in the Flash binary format.
			/// </summary>
			/// <param name="coder">
			///            an SWFDecoder object that contains the encoded Flash data.
			/// </param>
			/// <exception cref="IOException">
			///             if an error occurs while decoding the data. </exception>



		   public Level(SWFDecoder coder)
		   {
				mark = coder.readInt();
				left = coder.readUnsignedShort();
				right = coder.readUnsignedShort();
		   }

			/// <summary>
			/// Creates a envelope specifying the mark, left and right sound levels.
			/// </summary>
			/// <param name="markValue">
			///            the sample number in the 44.1KHz playback data stream
			///            where the levels for the channels is applied. </param>
			/// <param name="leftValue">
			///            the level for the left sound channel, in the range
			///            0..65535. </param>
			/// <param name="rightValue">
			///            the level for the right sound channel, in the range
			///            0..65535. </param>


			public Level(int markValue, int leftValue, int rightValue)
			{

				mark = markValue;

				if ((leftValue < 0) || (leftValue > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, leftValue);
				}
				left = leftValue;

				if ((rightValue < 0) || (rightValue > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, rightValue);
				}
				right = rightValue;
			}

			/// <summary>
			/// Get the sample number in the 44.1KHz playback data stream where
			/// the level information is applied.
			/// </summary>
			/// <returns> the sample number where the sound levels will be applied. </returns>
			public int Mark => mark;

		    /// <summary>
			/// Get the level of the sound played in the left channel.
			/// </summary>
			/// <returns> the sound level for the left channel. </returns>
			public int Left => left;

		    /// <summary>
			/// Get the level of the sound played in the right channel.
			/// </summary>
			/// <returns> the sound level for the right channel. </returns>
			public int Right => right;

		    public override string ToString()
			{
				return ObjectExtensions.FormatJava(FORMAT, mark, left, right);
			}



			public override bool Equals(object @object)
			{
				bool result;
				Level level;

				if (@object == null)
				{
					result = false;
				}
				else if (@object == this)
				{
					result = true;
				}
				else if (@object is Level)
				{
					level = (Level) @object;
					result = (mark == level.mark) && (left == level.left) && (right == level.right);
				}
				else
				{
					result = false;
				}
				return result;
			}

			public override int GetHashCode()
			{
				return ((mark * Constants.PRIME) + left) * Constants.PRIME + right;
			}

			/// <summary>
			/// {@inheritDoc} </summary>


			public int prepareToEncode(Context context)
			{
				// CHECKSTYLE IGNORE MagciNumberCheck FOR NEXT 1 LINES
				return 8;
			}

			/// <summary>
			/// {@inheritDoc} </summary>



			public void encode(SWFEncoder coder, Context context)
			{
				coder.writeInt(mark);
				coder.writeShort(left);
				coder.writeShort(right);
			}
		}

		/// <summary>
		/// List of sound levels that define the envelope. </summary>
		private IList<Level> levels;
		/// <summary>
		/// Number of levels in list. </summary>
		
		private int count;

		/// <summary>
		/// Creates and initialises an Envelope object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Envelope(SWFDecoder coder)
		{
			count = coder.readByte();
			levels = new List<Level>(count);

			for (int i = 0; i < count; i++)
			{
				levels.Add(new Level(coder));
			}
		}

		/// <summary>
		/// Creates and initialises an Envelope object using the values copied
		/// from another Envelope object.
		/// </summary>
		/// <param name="object">
		///            an Envelope object from which the values will be
		///            copied. </param>


		public Envelope(Envelope @object)
		{
			levels = new List<Level>(@object.levels);
		}

		/// <summary>
		/// Add a Envelope object to the list of envelope objects.
		/// </summary>
		/// <param name="level">
		///            a SoundLevel object. Must not be null. </param>
		/// <returns> this object. </returns>


		public Envelope add(Level level)
		{
			if (level == null)
			{
				throw new ArgumentException();
			}
			levels.Add(level);
			return this;
		}

		/// <summary>
		/// Get the list of Levels that control the volume of the sound.
		/// </summary>
		/// <returns> the Levels that define the envelope. </returns>
		public IList<Level> Levels
		{
			get => levels;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				levels = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public Envelope copy()
		{
			return new Envelope(this);
		}

		public override string ToString()
		{
			return levels.ToString();
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 2 LINES
			count = levels.Count;
			return 1 + (count << 3);
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(count);

			foreach (Level level in levels)
			{
				level.encode(coder, context);
			}
		}
	}

}