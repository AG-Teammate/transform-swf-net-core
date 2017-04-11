using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * RegisterCopy.java
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
	/// RegisterCopy is used to copy the item at the top of the stack to one of the
	/// Flash Player's internal registers.
	/// 
	/// <para>
	/// The Flash Player uses a stack to store values when executing the actions
	/// associated with a button being pushed, frame being played, etc. If a value is
	/// used repeatedly in a calculation, it must be pushed onto the stack each time
	/// using an Push action. To speed up the execution of the calculation and reduce
	/// the amount of code required the value can be saved to one of the internal
	/// registers of the Flash Player using the RegisterCopy action. This copies the
	/// value currently at the top of the stack into the specified register. Pushing
	/// an RegisterIndex object onto the stack creates a reference to the register so
	/// the Flash Player uses the value directly rather than pushing the value onto
	/// the stack then immediately popping to use the value in a calculation.
	/// </para>
	/// 
	/// <para>
	/// The value is not removed from the stack. The number of registers supported
	/// was expanded in Flash 7 from 4 to 256.
	/// </para>
	/// </summary>
	/// <seealso cref= RegisterIndex </seealso>
	/// <seealso cref= Push </seealso>
	public sealed class RegisterCopy : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "RegisterCopy: { number=%d}";
		/// <summary>
		/// Number of last internal register in the Flash Player. </summary>
		private const int LAST_REGISTER = 255;

		/// <summary>
		/// The number of the Flash Player's internal register. </summary>
		
		private readonly int number;

		/// <summary>
		/// Creates and initialises a RegisterCopy action using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public RegisterCopy(SWFDecoder coder)
		{
			coder.readUnsignedShort();
			number = coder.readByte();
		}

		/// <summary>
		/// Creates a RegisterCopy object with the register number.
		/// </summary>
		/// <param name="register">
		///            the number of one of the Flash Player's internal registers.
		///            Must be in the range 0..255. </param>


		public RegisterCopy(int register)
		{
			if ((register < 0) || (register > LAST_REGISTER))
			{
				throw new IllegalArgumentRangeException(0, LAST_REGISTER, register);
			}
			number = register;
		}

		/// <summary>
		/// Creates and initialises a RegisterCopy action using the values
		/// copied from another RegisterCopy action.
		/// </summary>
		/// <param name="object">
		///            a RegisterCopy action from which the values will be
		///            copied. </param>


		public RegisterCopy(RegisterCopy @object)
		{
			number = @object.number;
		}

		/// <summary>
		/// Returns the number of the Player register that the value on the stack
		/// will be copied to.
		/// </summary>
		/// <returns> the register number. </returns>
		public int Number => number;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public RegisterCopy copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, number);
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
			coder.writeByte(ActionTypes.REGISTER_COPY);
			coder.writeShort(1);
			coder.writeByte(number);
		}
	}

}