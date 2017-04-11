﻿using System;
using com.flagstone.transform.exception;

/*
 * RegisterIndex.java
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
	/// RegisterIndex is used references a value stored in one of the Flash Player's
	/// internal registers. The value will be pushed onto the top of the Flash
	/// Player's stack.
	/// </summary>
	/// <seealso cref= RegisterCopy </seealso>
	/// <seealso cref= Push </seealso>
	public sealed class RegisterIndex
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Register: { number=%d}";
		/// <summary>
		/// Number of last internal register in the Flash Player. </summary>
		private const int LAST_REGISTER = 255;

		/// <summary>
		/// The number of the Flash Player's internal register. </summary>
		
		private readonly int number;

		/// <summary>
		/// Creates a RegisterIndex object referencing the value stored in one of the
		/// Flash Player's internal registers.
		/// </summary>
		/// <param name="index">
		///            the register number. Must be in the range 0..255. </param>


		public RegisterIndex(int index)
		{
			if ((index < 0) || (index > LAST_REGISTER))
			{
				throw new IllegalArgumentRangeException(0, LAST_REGISTER, index);
			}
			number = index;
		}

		/// <summary>
		/// Get the number of the register that will be accessed and the value
		/// pushed onto the Flash Player's stack.
		/// </summary>
		/// <returns> the register number. </returns>
		public int Number => number;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, number);
		}
	}

}