using System;
using com.flagstone.transform.coder;

/*
 * MovieData.java
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

namespace com.flagstone.transform
{
    /// <summary>
	/// MovieData is used to store one or more MovieTags which already have been
	/// encoded for writing to a Flash file.
	/// 
	/// <para>
	/// You can use this class to either selectively decode the tags in a movie, so
	/// tags that are not of interest can be left encoded or to selectively encode
	/// tags that will not change when generating Flash files from a template.
	/// </para>
	/// </summary>
	public sealed class MovieData : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "MovieData: { data=byte<%d> ...}";

		/// <summary>
		/// The encoded MovieTag objects. </summary>
		
		private readonly byte[] data;

		/// <summary>
		/// Create a MovieData object containing a block of encoded MovieTag objects.
		/// </summary>
		/// <param name="bytes"> the encoded MovieTag objects. </param>


		public MovieData(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentException();
			}
			data = Arrays.copyOf(bytes, bytes.Length);
		}

		/// <summary>
		/// Creates and initialises a MovieData object using the values copied
		/// from another MovieData object.
		/// </summary>
		/// <param name="object">
		///            a MovieData object from which the values will be
		///            copied. </param>


		public MovieData(MovieData @object)
		{
			data = @object.data;
		}

		/// <summary>
		/// Get a copy the encoded MovieTag objects.
		/// </summary>
		/// <returns> a copy of the encoded objects. </returns>
		public byte[] Data => Arrays.copyOf(data, data.Length);

	    /// <summary>
		/// {@inheritDoc} </summary>
		public MovieData copy()
		{
			return new MovieData(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, data.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			return data.Length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeBytes(data);
		}
	}

}