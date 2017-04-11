using System;
using System.IO;

/*
 * CoderException.java
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

namespace com.flagstone.transform.coder
{

	/// <summary>
	/// CoderException is thrown when errors occur while encoding or decoding
	/// objects. Errors are reported as either underflow or overflow errors
	/// indicating that the class used to encode/decode a particular data structure
	/// did not encode or decode the expected number of bytes. This allows the
	/// majority of software errors and errors due to improperly encoded flash files
	/// to be detected.
	/// </summary>
	public sealed class CoderException : IOException
	{

		/// <summary>
		/// Serial number identifying the version of the object. </summary>
		private const long serialVersionUID = 1;

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "CoderException: { " + "location=%s; length=%d; delta=%d; message=%s}";

		/// <summary>
		/// The location of the start of the object being encoded/decoded
		/// when the error occurred.
		/// </summary>
		
		private readonly int start;
		/// <summary>
		/// The expected length of the encoded object. </summary>
		
		private readonly int length;
		/// <summary>
		/// The difference between the expected and actual number of bytes encoded
		/// or decoded.
		/// </summary>
		
		private readonly int delta;

		/// <summary>
		/// Creates a CoderException to report where a problem occurred when encoding
		/// or decoding a Flash (.swf) file.
		/// </summary>
		/// <param name="location">
		///            the address in the file where the data structure being
		///            encoded/decoded is located. This is only valid for files being
		///            decoded since the encoded file will not be written if an
		///            exception occurs.
		/// </param>
		/// <param name="message">
		///            a short description of the error. </param>


		public CoderException(int location, string message) : base(message)
		{
			start = location;
			length = 0;
			delta = 0;
		}

		/// <summary>
		/// Creates a CoderException to report where a problem occurred when encoding
		/// or decoding a Flash (.swf) file.
		/// </summary>
		/// <param name="location">
		///            the address in the file where the data structure being
		///            encoded/decoded is located. This is only valid for files being
		///            decoded since the encoded file will not be written if an
		///            exception occurs.
		/// </param>
		/// <param name="size">
		///            the number of bytes that were expected to be encoded or
		///            decoded.
		/// </param>
		/// <param name="difference">
		///            the difference between the expected number of bytes and the
		///            actual number encoded or decoded. </param>


		public CoderException(int location, int size, int difference)
		{
			start = location;
			length = size;
			delta = difference;
		}

		/// <summary>
		/// Get the byte address of the start of the object that caused the
		/// error.
		/// </summary>
		/// <returns> the location of the start of the encoded object when the error
		/// occurred. </returns>
		public int Start => start;

	    /// <summary>
		/// Get number of bytes the object was expected to occupy when encoded.
		/// </summary>
		/// <returns> get the number of bytes expected to be encoded or decoded. </returns>
		public int Length => length;

	    /// <summary>
		/// Get the difference between the expected number of bytes and the
		/// actual number of bytes encoded or decoded.
		/// </summary>
		/// <returns> the difference from the expected number of bytes. </returns>
		public int Delta => delta;

	    /// <summary>
		/// Get a string representation of the error.
		/// </summary>
		/// <returns> the string describing the error. </returns>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, start.ToString("x"), length, delta, Message);
		}
	}

}