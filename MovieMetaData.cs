using System;
using com.flagstone.transform.coder;

/*
 * MetaData.java
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

namespace com.flagstone.transform
{
    /// <summary>
	/// MetaData is used to add a user-defined information into a Flash file. The
	/// information uses the Resource Description Format (RDF) and is compliant
	/// with Adobe's Extensible Metadata Platform, see
	/// http://www.adobe.com/products/xmp.
	/// </summary>
	public sealed class MovieMetaData : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "MetaData: { metadata=%s}";
		/// <summary>
		/// The meta-data for the movie. </summary>
		private string metaData;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a MoveMetaData object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public MovieMetaData(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			metaData = coder.readString(length - 1);
			coder.readByte();
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a MoveMetaData object with the specified string containing the
		/// meta-data for the movie.
		/// </summary>
		/// <param name="aString">
		///            an arbitrary string containing the meta-data. Must not be
		///            null. </param>


		public MovieMetaData(string aString)
		{
			MetaData = aString;
		}

		/// <summary>
		/// Creates and initialises a MovieMetaData object using the values copied
		/// from another MovieMetaData object.
		/// </summary>
		/// <param name="object">
		///            a MovieMetaData object from which the values will be
		///            copied. </param>


		public MovieMetaData(MovieMetaData @object)
		{
			metaData = @object.metaData;
		}

		/// <summary>
		/// Get the meta-data for the movie.
		/// </summary>
		/// <returns> the string containing the meta-data. </returns>
		public string MetaData
		{
			get => metaData;
		    set
			{
				if (ReferenceEquals(value, null))
				{
					throw new ArgumentException();
				}
				metaData = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public MovieMetaData copy()
		{
			return new MovieMetaData(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, metaData);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = context.strlen(metaData);

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.METADATA << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.METADATA << Coder.LENGTH_FIELD_SIZE) | length);
			}
			coder.writeString(metaData);
		}
	}

}