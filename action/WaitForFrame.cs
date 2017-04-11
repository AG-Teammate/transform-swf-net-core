using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * WaitForFrame.java
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
	/// The WaitForFrame action instructs the player to wait until the specified
	/// frame number has been loaded.
	/// 
	/// <para>
	/// If the frame has been loaded then the actions in the following <i>n</i>
	/// actions are executed. This action is most often used to execute a short
	/// animation loop that plays until the main part of a movie has been loaded.
	/// </para>
	/// 
	/// <para>
	/// This method of waiting until a frame has been loaded is considered obsolete.
	/// Determining the number of frames loaded using the FramesLoaded property of
	/// the Flash player in combination with an If action is now the preferred
	/// mechanism.
	/// </para>
	/// </summary>
	/// <seealso cref= Push </seealso>
	/// <seealso cref= If </seealso>
	public sealed class WaitForFrame : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "WaitForFrame: { frameNumber=%d;" + " actionCount=%d}";

		/// <summary>
		/// The maximum offset to the next frame. </summary>
		private const int MAX_FRAME_OFFSET = 65535;
		/// <summary>
		/// The highest number of actions that can be executed. </summary>
		private const int MAX_COUNT = 255;

		/// <summary>
		/// The frame number to test. </summary>
		
		private readonly int frameNumber;
		/// <summary>
		/// The number of actions to execute if the frame has been loaded. </summary>
		
		private readonly int actionCount;

		/// <summary>
		/// Creates and initialises a WaitForFrame action using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public WaitForFrame(SWFDecoder coder)
		{
			coder.readUnsignedShort();
			frameNumber = coder.readUnsignedShort();
			actionCount = coder.readByte();
		}

		/// <summary>
		/// Creates a WaitForFrame object with the specified frame number and the
		/// number of actions that will be executed when the frame is loaded.
		/// </summary>
		/// <param name="frame">
		///            the number of the frame to wait for. Must be in the range
		///            1..65535. </param>
		/// <param name="count">
		///            the number (not bytes) of actions to execute. Must be in the
		///            range 0..255. </param>


		public WaitForFrame(int frame, int count)
		{
			if ((frame < 1) || (frame > MAX_FRAME_OFFSET))
			{
				throw new IllegalArgumentRangeException(1, MAX_FRAME_OFFSET, frame);
			}
			frameNumber = frame;
			if ((count < 0) || (count > MAX_COUNT))
			{
				throw new IllegalArgumentRangeException(1, MAX_COUNT, count);
			}
			actionCount = count;
		}

		/// <summary>
		/// Creates and initialises a WaitForFrame action using the values
		/// copied from another WaitForFrame action.
		/// </summary>
		/// <param name="object">
		///            a WaitForFrame action from which the values will be
		///            copied. </param>


		public WaitForFrame(WaitForFrame @object)
		{
			frameNumber = @object.frameNumber;
			actionCount = @object.actionCount;
		}

		/// <summary>
		/// Get the frame number to test to see if has been loaded.
		/// </summary>
		/// <returns> the frame number to test. </returns>
		public int FrameNumber => frameNumber;

	    /// <summary>
		/// Get the number of actions that will be executed when the specified
		/// frame is loaded.
		/// </summary>
		/// <returns> the number of actions to execute. </returns>
		public int ActionCount => actionCount;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public WaitForFrame copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, frameNumber, actionCount);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			//CHECKSTYLE:OFF - Fixed length when encoded.
			return Coder.ACTION_HEADER + 3;
			//CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ActionTypes.WAIT_FOR_FRAME);
			//CHECKSTYLE:OFF - Fixed length when encoded.
			coder.writeShort(3);
			//CHECKSTYLE:ON
			coder.writeShort(frameNumber);
			coder.writeByte(actionCount);
		}
	}

}