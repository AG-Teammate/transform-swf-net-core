

/*
 * IllegalArgumentRangeException.java
 * Transform
 *
 * Copyright (c) 2009-2010 Flagstone Software Ltd. All rights reserved.
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

using System;

namespace com.flagstone.transform.exception
{
	/// <summary>
	/// IllegalArgumentValueException is thrown when a value is used that is not
	/// within a pre-defined range.
	/// </summary>
	public sealed class IllegalArgumentRangeException : ArgumentException
	{

		/// <summary>
		/// Serial number identifying the version of the object. </summary>
		private const long serialVersionUID = -9208368642722953411L;
		/// <summary>
		/// The lower value of the range. </summary>
		
		private readonly int lower;
		/// <summary>
		/// The upper value of the range. </summary>
		
		private readonly int upper;
		/// <summary>
		/// The actual value used. </summary>
		
		private readonly int value;

		/// <summary>
		/// Creates an IllegalArgumentRangeException with the range of expected
		/// values and the actual value used.
		/// </summary>
		/// <param name="start"> the start of the expected range. </param>
		/// <param name="end"> the end of the expected range inclusive. </param>
		/// <param name="actual"> the actual value used. </param>


		public IllegalArgumentRangeException(int start, int end, int actual) : base("Lower Bound: " + start + " Upper Bound: " + end + " Value: " + actual)
		{
			lower = start;
			upper = end;
			value = actual;
		}
		/// <summary>
		/// Get the lower value of the expected range. </summary>
		/// <returns> the range's lower value. </returns>
		public int Lower => lower;

	    /// <summary>
		/// Get the upper value of the expected range. </summary>
		/// <returns> the range's upper value. </returns>
		public int Upper => upper;

	    /// <summary>
		/// Get the actual value that triggered the exception. </summary>
		/// <returns> the actual value used. </returns>
		public int Value => value;
	}

}