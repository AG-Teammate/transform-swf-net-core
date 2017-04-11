﻿/*
 * Call.java
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

using com.flagstone.transform.coder;

namespace com.flagstone.transform.action
{
    /// <summary>
	/// Call is used to execute the actions previously assigned to a given frame with
	/// an DoAction object. Call is a stack-based action, the value for the <i>frame
	/// name</i> or <i>frame number</i> is retrieved from the top of the stack when
	/// it is executed.
	/// </summary>
	public sealed class Call : Action
	{

		/// <summary>
		/// Shared instance. </summary>
		private static readonly Call INSTANCE = new Call();

		/// <summary>
		/// Returns a shared instance of the Call action.
		/// </summary>
		/// <returns> a singleton used to represent all Call actions. </returns>
		public static Call Instance => INSTANCE;

	    /// <summary>
		/// Constructor used to created the singleton action. </summary>
		private Call()
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public Call copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			return Coder.ACTION_HEADER;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



	   public void encode(SWFEncoder coder, Context context)
	   {
			coder.writeByte(ActionTypes.CALL);
			coder.writeShort(0);
	   }
	}

}