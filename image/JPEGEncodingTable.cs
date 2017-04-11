using System;
using com.flagstone.transform.coder;

/*
 * JPEGEncodingTable.java
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

namespace com.flagstone.transform.image
{
    /// <summary>
	/// JPEGEncodingTable defines the Huffman encoding table for JPEG images.
	/// 
	/// <para>
	/// The encoding table is shared between all images defined using the
	/// DefineJPEGImage class so there should only be one JPEGEncodingTable object
	/// defined in a movie.
	/// </para>
	/// 
	/// <para>
	/// The JPEGEncodingTable class is not essential to define JPEG encoded images in
	/// a movie using the DefineJPEGImage class. You can still display an image if it
	/// contains the encoding table. There is no need to separate it and add it to a
	/// JPEGEncodingTable object, particularly since different images contain
	/// different encoding tables.
	/// </para>
	/// </summary>
	/// <seealso cref= DefineJPEGImage </seealso>
	public sealed class JPEGEncodingTable : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "JPEGEncodingTable: {" + " table=byte<%d> ...}";

		/// <summary>
		/// Huffman encoding table. </summary>
		private byte[] table;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a JPEGEncodingTable object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public JPEGEncodingTable(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			table = coder.readBytes(new byte[length]);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a JPEGEncodingTable object with the encoding table data.
		/// </summary>
		/// <param name="bytes">
		///            a list of bytes contains the data for the encoding table.
		///            Must not be null. </param>


		public JPEGEncodingTable(byte[] bytes)
		{
			Table = bytes;
		}

		/// <summary>
		/// Creates and initialises a JPEGEncodingTable object using the values
		/// copied from another JPEGEncodingTable object.
		/// </summary>
		/// <param name="object">
		///            a JPEGEncodingTable object from which the values will be
		///            copied. </param>


		public JPEGEncodingTable(JPEGEncodingTable @object)
		{
			table = @object.table;
		}

		/// <summary>
		/// Get a copy of the encoding table.
		/// </summary>
		/// <returns> a copy of the table data. </returns>
		public byte[] Table
		{
			get => Arrays.copyOf(table, table.Length);
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				table = Arrays.copyOf(value, value.Length);
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public JPEGEncodingTable copy()
		{
			return new JPEGEncodingTable(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, table.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = table.Length;

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.JPEG_TABLES << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.JPEG_TABLES << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeBytes(table);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}