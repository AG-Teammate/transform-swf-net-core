﻿using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * WaitForFrame2.java
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
	/// The WaitForFrame2 action instructs the player to wait until the specified
	/// frame number or named frame has been loaded.
	/// 
	/// <para>
	/// If the frame has been loaded then the following <i>n</i> actions are
	/// executed. The WaitForFrame2 action extends the WaitForFrame action by
	/// allowing the name of a frame to be specified.
	/// </para>
	/// 
	/// <para>
	/// WaitForFrame2 is a stack-based action. The frame number or frame name which
	/// should be loaded to trigger execution of the following actions is popped from
	/// the Flash Player's stack. Note however that this method of waiting until a
	/// frame has been loaded is considered obsolete. Determining the number of
	/// frames loaded using the FramesLoaded property of the Flash player in
	/// combination with an If action is now preferred.
	/// </para>
	/// </summary>
	/// <seealso cref= Push </seealso>
	/// <seealso cref= If </seealso>
	public sealed class WaitForFrame2 : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "WaitForFrame2: { actionCount=%d}";

		/// <summary>
		/// The highest number of actions that can be executed. </summary>
		private const int MAX_COUNT = 255;

		/// <summary>
		/// The number of actions to be executed. </summary>
		
		private readonly int actionCount;

		/// <summary>
		/// Creates and initialises a WaitForFrame2 action using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public WaitForFrame2(SWFDecoder coder)
		{
			coder.readUnsignedShort();
			actionCount = coder.readByte();
		}

		/// <summary>
		/// Creates a WaitForFrame2 object with the number of actions to execute if
		/// the frame has been loaded.
		/// </summary>
		/// <param name="count">
		///            the number of actions to execute. Must be in the range 0..255. </param>


		public WaitForFrame2(int count)
		{
			if ((count < 0) || (count > MAX_COUNT))
			{
				throw new IllegalArgumentRangeException(0, MAX_COUNT, count);
			}
			actionCount = count;
		}

		/// <summary>
		/// Creates and initialises a WaitForFrame2 action using the values
		/// copied from another WaitForFrame2 action.
		/// </summary>
		/// <param name="object">
		///            a WaitForFrame2 action from which the values will be
		///            copied. </param>


		public WaitForFrame2(WaitForFrame2 @object)
		{
			actionCount = @object.actionCount;
		}

		/// <summary>
		/// Returns the number of actions to execute.
		/// </summary>
		/// <returns> the number of actions, (not encoded bytes). </returns>
		public int ActionCount => actionCount;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public WaitForFrame2 copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, actionCount);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			return Coder.ACTION_HEADER + 1;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ActionTypes.WAIT_FOR_FRAME_2);
			coder.writeShort(1);
			coder.writeByte(actionCount);
		}
	}

}