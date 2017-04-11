using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * GotoFrame.java
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
	/// The GotoFrame action instructs the player to move to the specified frame
	/// number in the current movie's main time-line.
	/// 
	/// <para>
	/// GotoFrame is only used to control the main time-line of a movie. Controlling
	/// how an individual movie clip is played is handled by a different mechanism.
	/// From Flash 5 onward movie clips are defined as objects and the ExecuteMethod
	/// action is used to execute the gotoAndPlay() or gotoAndStop() which start and
	/// stop playing a movie clip.
	/// </para>
	/// </summary>
	/// <seealso cref= GotoFrame2 </seealso>
	public sealed class GotoFrame : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "GotoFrame: { frameNumber=%d}";
		/// <summary>
		/// The maximum offset to the next frame. </summary>
		private const int MAX_FRAME_OFFSET = 65535;

		/// <summary>
		/// The frame number to be displayed. </summary>
		
		private readonly int frameNumber;

		/// <summary>
		/// Creates and initialises an GotoFrame action using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public GotoFrame(SWFDecoder coder)
		{
			coder.readUnsignedShort();
			frameNumber = coder.readUnsignedShort();
		}

		/// <summary>
		/// Creates a GotoFrame with the specified frame number.
		/// </summary>
		/// <param name="number">
		///            the number of the frame. Must be in the range 0..65535. </param>


		public GotoFrame(int number)
		{
			if ((number < 0) || (number > MAX_FRAME_OFFSET))
			{
				throw new IllegalArgumentRangeException(1, MAX_FRAME_OFFSET, number);
			}
			frameNumber = number;
		}

		/// <summary>
		/// Creates and initialises a GotoFrame action using the values
		/// copied from another GotoFrame action.
		/// </summary>
		/// <param name="object">
		///            a GotoFrame action from which the values will be
		///            copied. </param>


		public GotoFrame(GotoFrame @object)
		{
			frameNumber = @object.frameNumber;
		}

		/// <summary>
		/// Returns the number of the frame to move the main time-line to.
		/// </summary>
		/// <returns> the offset to the next frame to be displayed. </returns>
		public int FrameNumber => frameNumber;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public GotoFrame copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, frameNumber);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			return Coder.ACTION_HEADER + 2;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ActionTypes.GOTO_FRAME);
			coder.writeShort(2);
			coder.writeShort(frameNumber);
		}
	}

}