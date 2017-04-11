using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * GotoFrame2.java
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
namespace com.flagstone.transform.action
{
    /// <summary>
	/// The GotoFrame2 action instructs the player to go to the named or numbered
	/// frame in the current movie's main time-line. It extends the functionality
	/// provided by the GotoFrame action by allowing the name of a frame, previously
	/// assigned using the FrameLabel object, to be specified.
	/// 
	/// <para>
	/// Up to Flash Version 4, movies contained a single sequence of 65536 frames. In
	/// Flash 5 the concept of Scenes was added which allowed movies to contain
	/// 'pages' of frames. GotoFrame2 contains a frameOffset attribute which allows
	/// the frames in each scene to be referenced by its 'logical' number. The
	/// frameOffset for a given scene is added to the frame number to generate the
	/// 'physical' page number.
	/// </para>
	/// 
	/// <para>
	/// GotoFrame2 is a stack-based action. The name or number of the frame is pushed
	/// onto the stack before the GotoFrame2 action is executed. If a frameOffset is
	/// specified it is added to the number of the frame identified by the stack
	/// arguments to give the final frame number. Whether the movie starts playing
	/// the frame is controlled by the boolean attribute, <i>play</i>. When set to
	/// true the movie starts playing the frame as soon as it has been loaded by the
	/// Flash Player.
	/// </para>
	/// 
	/// <para>
	/// GotoFrame2 is only used to control the main time-line of a movie. Controlling
	/// how an individual movie clip is played is handled by a different mechanism.
	/// From Flash 5 onward movie clips are defined as objects. The ExecuteMethod
	/// action is used to execute the gotoAndPlay() or gotoAndStop() methods that
	/// control a movie clip's time-line.
	/// </para>
	/// </summary>
	/// <seealso cref= GotoFrame </seealso>
	public sealed class GotoFrame2 : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Gotoframe2: { playFrame=%s;" + " frameOffset=%d}";

		/// <summary>
		/// The maximum offset to the next frame. </summary>
		private const int MAX_FRAME_OFFSET = 65535;
		/// <summary>
		/// Bit mask for field indication if the encoded action has an offset. </summary>
		private const int OFFSET_MASK = 0x02;
		/// <summary>
		/// Bit mask for field containing play attribute. </summary>
		private const int PLAY_MASK = 0x01;
		/// <summary>
		/// Encoded length with offset. </summary>
		private const int LEN_WITH_OFFSET = 3;
		/// <summary>
		/// Encoded length without offset. </summary>
		private const int LEN_NO_OFFSET = 1;

		/// <summary>
		/// Indicates whether the Flash Player should start playing the frame. </summary>
		
		private readonly bool play;
		/// <summary>
		/// The offset to the next frame. </summary>
		
		private readonly int frameOffset;

		/// <summary>
		/// The length of the action, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// Flag used to indicate the action contains a frame offset. </summary>
		
		private bool hasOffset;

		/// <summary>
		/// Creates and initialises an GotoFrame2 action using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public GotoFrame2(SWFDecoder coder)
		{
			length = coder.readUnsignedShort();



			int bits = coder.readByte();
			hasOffset = (bits & Coder.BIT1) != 0;
			play = (bits & Coder.BIT0) != 0;

			if (hasOffset)
			{
				frameOffset = coder.readSignedShort();
			}
			else
			{
				frameOffset = 0;
			}
		}

		/// <summary>
		/// Creates a GotoFrame2 object with the specified play flag setting.
		/// </summary>
		/// <param name="aBool">
		///            true if the player should being playing the movie at the
		///            specified frame. false if the player should stop playing the
		///            movie. </param>


		public GotoFrame2(bool aBool) : this(0, aBool)
		{
		}

		/// <summary>
		/// Creates a GotoFrame2 object with the specified play flag setting and
		/// frame offset for a given scene.
		/// </summary>
		/// <param name="offset">
		///            a number which will be added to the number of the frame popped
		///            from the stack to give the final frame number. Must be in the
		///            range 1..65535. </param>
		/// <param name="aBool">
		///            true if the player should being playing the movie at the
		///            specified frame, false if the player should stop playing the
		///            movie. </param>


		public GotoFrame2(int offset, bool aBool)
		{
			if ((offset < 0) || (offset > MAX_FRAME_OFFSET))
			{
				throw new IllegalArgumentRangeException(0, MAX_FRAME_OFFSET, offset);
			}
			frameOffset = offset;
			play = aBool;
		}

		/// <summary>
		/// Creates and initialises a GotoFrame2 action using the values
		/// copied from another GotoFrame2 action.
		/// </summary>
		/// <param name="object">
		///            a GotoFrame2 action from which the values will be
		///            copied. </param>


		public GotoFrame2(GotoFrame2 @object)
		{
			play = @object.play;
			frameOffset = @object.frameOffset;
		}

		/// <summary>
		/// Returns the offset that will be added to the 'logical' frame number
		/// obtained from the stack to generate the 'physical' frame number.
		/// </summary>
		/// <returns> the offset to the next frame to be displayed. </returns>
		public int FrameOffset => frameOffset;

	    /// <summary>
		/// Returns the play flag.
		/// </summary>
		/// <returns> true if the player will being playing the movie at the specified
		///         frame, false otherwise. </returns>
		public bool Play => play;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public GotoFrame2 copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, play.ToString(), frameOffset);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			hasOffset = frameOffset > 0;
			if (hasOffset)
			{
				length = LEN_WITH_OFFSET;
			}
			else
			{
				length = LEN_NO_OFFSET;
			}
			return Coder.ACTION_HEADER + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ActionTypes.GOTO_FRAME_2);
			coder.writeShort(length);

			int flags = 0;
			if (hasOffset)
			{
				flags |= OFFSET_MASK;
			}
			if (play)
			{
				flags |= PLAY_MASK;
			}
			coder.writeByte(flags);

			if (hasOffset)
			{
				coder.writeShort(frameOffset);
			}
		}
	}

}