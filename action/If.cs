﻿using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * If.java
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
	/// The If action is used to perform a conditional branch to control the actions
	/// executed by the Flash Player.
	/// 
	/// <para>
	/// When executed, the If action pops a value from the stack and evaluates it to
	/// see whether it evaluates as true or false. If the boolean value is true the
	/// offset attribute of the If action is added to the Flash Player's instruction
	/// pointer and execution of the stream of actions continues from that location.
	/// If the boolean value is false then no branch is taken.
	/// </para>
	/// 
	/// <para>
	/// Although the Flash Player contains an instruction pointer it does not support
	/// an explicit address space. The instruction pointer is used to reference
	/// actions within the current stream of actions being executed whether they are
	/// associated with a given frame, button or movie clip. The value contained in
	/// the instruction pointer is the address relative to the start of the current
	/// stream.
	/// </para>
	/// 
	/// <para>
	/// The offset is a signed number, allowing branches up to -32768 to 32767 bytes.
	/// The instruction pointer points to the next instruction in the stream of
	/// actions being executed so specifying an offset of zero will have no effect on
	/// the sequence of instructions executed.
	/// </para>
	/// 
	/// <para>
	/// If the value popped off the stack is a number it is evaluated as true if it
	/// is non-zero. If the value is a string it is evaluated to true if it is not an
	/// empty string ("") or the strings "0" or "false".
	/// </para>
	/// </summary>
	/// <seealso cref= Jump </seealso>
	public sealed class If : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "If: { offset=%d}";
		/// <summary>
		/// Minimum coder pointer offset. </summary>
		private const int MIN_CODE_JUMP = -32768;
		/// <summary>
		/// Maximum coder pointer offset. </summary>
		private const int MAX_CODE_JUMP = 32767;

		/// <summary>
		/// The offset to the next action. </summary>
		
		private readonly int offset;

		/// <summary>
		/// Creates and initialises an If action using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public If(SWFDecoder coder)
		{
			coder.readUnsignedShort();
			offset = coder.readSignedShort();
		}

		/// <summary>
		/// Creates an if action with the specified offset. The offset must be in the
		/// range -32768..32767.
		/// </summary>
		/// <param name="anOffset">
		///            the number of bytes to add to the instruction pointer if the
		///            value popped off the stack evaluates to true. </param>


		public If(int anOffset)
		{
			if ((anOffset < MIN_CODE_JUMP) || (anOffset > MAX_CODE_JUMP))
			{
				throw new IllegalArgumentRangeException(MIN_CODE_JUMP, MAX_CODE_JUMP, anOffset);
			}
			offset = anOffset;
		}

		/// <summary>
		/// Creates and initialises an If action using the values copied from another
		/// If action.
		/// </summary>
		/// <param name="object">
		///            an If action from which the values will be
		///            copied. </param>


		public If(If @object)
		{
			offset = @object.offset;
		}

		/// <summary>
		/// Get the offset that will be added to the instruction pointer if the
		/// value at the top of the stack evaluates to true (non-zero).
		/// </summary>
		/// <returns> the offset to the next action. </returns>
		public int Offset => offset;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public If copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, offset);
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
			coder.writeByte(ActionTypes.IF);
			coder.writeShort(2);
			coder.writeShort(offset);
		}
	}

}