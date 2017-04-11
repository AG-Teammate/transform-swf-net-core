using System;

/*
 * IllegalArgumentValueException.java
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

namespace com.flagstone.transform.exception
{

	/// <summary>
	/// IllegalArgumentValueException is thrown when a value is used that is not
	/// a member of the expected set.
	/// </summary>
	public sealed class IllegalArgumentValueException : ArgumentException
	{

/*
		/// <summary>
		/// Serial number identifying the version of the object. </summary>
		private const long serialVersionUID = 3748031731035981638L;
*/
		/// <summary>
		/// The set of expected values. </summary>
		
		private readonly int[] expected;
		/// <summary>
		/// The actual value used. </summary>
		
		private readonly int actual;

		/// <summary>
		/// Creates an IllegalArgumentValueException with the set of expected values
		/// and the actual value used.
		/// </summary>
		/// <param name="set"> the set of expected values. </param>
		/// <param name="value"> the actual value used. </param>


		public IllegalArgumentValueException(int[] set, int value) : base("Valid values: " + Arrays.ToString(set) + " Value: " + value)
		{
			expected = Arrays.copyOf(set, set.Length);
			actual = value;
		}
		/// <summary>
		/// Get the set of expected values.
		/// </summary>
		/// <returns> a copy of the expected values. </returns>
		public int[] Expected => Arrays.copyOf(expected, expected.Length);

	    /// <summary>
		/// Get the actual value that triggered the exception. </summary>
		/// <returns> the actual value used. </returns>
		public int Actual => actual;
	}

}