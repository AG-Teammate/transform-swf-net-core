using System;
using com.flagstone.transform.coder;

/*
 * ShapeData.java
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

namespace com.flagstone.transform.shape
{
    /// <summary>
	/// ShapeData is a convenience class for holding a set of encoded ShapeRecords
	/// so that a Shape can be lazily decoded.
	/// </summary>
	public sealed class ShapeData : ShapeRecord
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "ShapeData: byte<%d> ...";
		/// <summary>
		/// The encoded ShapeRecords. </summary>
		
		private readonly byte[] data;

		/// <summary>
		/// Create a new ShapeData object initialised with an array of bytes
		/// containing the encoded records for a shape.
		/// </summary>
		/// <param name="size">
		///            the number of bytes to read for the encoded shape data. </param>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while reading the encoded shape data. </exception>



		public ShapeData(int size, SWFDecoder coder)
		{
			if (size < 0)
			{
				throw new ArgumentException();
			}
			data = coder.readBytes(new byte[size]);
		}

		/// <summary>
		/// Create a new ShapeData object with an array of encoded ShapeRecords. </summary>
		/// <param name="bytes"> the encoded ShapeRecords. </param>


		public ShapeData(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentException();
			}
			data = Arrays.copyOf(bytes, bytes.Length);
		}

		/// <summary>
		/// Creates and initialises a ShapeData object using the values copied
		/// from another ShapeData object.
		/// </summary>
		/// <param name="object">
		///            a ShapeData object from which the values will be
		///            copied. </param>


		public ShapeData(ShapeData @object)
		{
			data = @object.data;
		}

		/// <summary>
		/// Get a copy of the encoded data for the action.
		/// </summary>
		/// <returns> a copy of the encoded shape. </returns>
		public byte[] Data => Arrays.copyOf(data, data.Length);

	    /// <summary>
		/// {@inheritDoc} </summary>
		public ShapeData copy()
		{
			return new ShapeData(this);
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