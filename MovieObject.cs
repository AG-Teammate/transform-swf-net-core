using System;
using com.flagstone.transform.coder;

/*
 * MovieObject.java
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
	/// MovieObject is used to represent any object decoded from a Movie that is not
	/// directly supported by Transform.
	/// 
	/// <para>
	/// This allow a certain amount of forward compatibility where file which use a
	/// version of Flash greater than the one supported by Transform can be decoded
	/// and encoded.
	/// </para>
	/// </summary>
	public sealed class MovieObject : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "MovieObject: { type=%d;" + " data=byte<%d> ...}";

		/// <summary>
		/// The type identifying the MovieTag. </summary>
		
		private readonly int type;
		/// <summary>
		/// The encoded data that make up the body of the tag. </summary>
		
		private readonly byte[] data;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a MovieObject object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public MovieObject(SWFDecoder coder)
		{
			type = (int)((uint)coder.scanUnsignedShort() >> Coder.LENGTH_FIELD_SIZE);
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			data = coder.readBytes(new byte[length]);
		}

		/// <summary>
		/// Creates and initialises a MovieObject object using the specified type
		/// and encoded data.
		/// </summary>
		/// <param name="aType"> the type that identifies the MovieTag when it is encoded. </param>
		/// <param name="bytes"> the encoded bytes that form the body of the object. </param>


		public MovieObject(int aType, byte[] bytes)
		{
			type = aType;

			if (bytes == null)
			{
				throw new ArgumentException();
			}
			data = Arrays.copyOf(bytes, bytes.Length);
		}

		/// <summary>
		/// Creates and initialises a MovieObject object using the values copied
		/// from another MovieObject object.
		/// </summary>
		/// <param name="object">
		///            a MovieObject object from which the values will be
		///            copied. </param>


		public MovieObject(MovieObject @object)
		{
			type = @object.type;
			data = @object.data;
		}

		/// <summary>
		/// Get the type that identifies the object when it is encoded. </summary>
		/// <returns> the type that identifies the encoded data structure. </returns>
		public int Type => type;

	    /// <summary>
		/// Get a copy of the encoded data for the movie tag object. </summary>
		/// <returns> a copy of the encoded data. </returns>
		public byte[] Data => Arrays.copyOf(data, data.Length);

	    /// <summary>
		/// {@inheritDoc} </summary>
		public MovieObject copy()
		{
			return new MovieObject(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, type, data.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = data.Length;
			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((type << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((type << Coder.LENGTH_FIELD_SIZE) | length);
			}
			coder.writeBytes(data);
		}
	}

}