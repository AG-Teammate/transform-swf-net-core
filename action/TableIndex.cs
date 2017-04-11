﻿using System;
using com.flagstone.transform.exception;

/*
 * TableIndex.java
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
	/// TableIndex is used with an Push action to push a reference to an entry in a
	/// Table of string literals onto the stack.
	/// </summary>
	/// <seealso cref= Table </seealso>
	/// <seealso cref= Push </seealso>
	public sealed class TableIndex
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "TableIndex: { index=%d}";

		/// <summary>
		/// The highest index number supported. </summary>
		private const int LAST_INDEX = 65535;

		/// <summary>
		/// The index into the table of strings. </summary>
		
		private readonly int index;

		/// <summary>
		/// Creates a TableIndex object referencing the value stored in the literal
		/// table.
		/// </summary>
		/// <param name="anIndex">
		///            the index into the literal table. Must be in the range
		///            0..65535. </param>


		public TableIndex(int anIndex)
		{
			if ((anIndex < 0) || (anIndex > LAST_INDEX))
			{
				throw new IllegalArgumentRangeException(0, LAST_INDEX, anIndex);
			}
			index = anIndex;
		}

		/// <summary>
		/// Get the index in the table of string literals.
		/// </summary>
		/// <returns> the index of the string literal. </returns>
		public int Index => index;

	    /// <summary>
		/// {@inheritDoc} </summary>
	   public override string ToString()
	   {
			return ObjectExtensions.FormatJava(FORMAT, index);
	   }
	}

}