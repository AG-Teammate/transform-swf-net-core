using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * DefineData.java
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
	/// DefineData is used to embed binary data in a Flash file.
	/// 
	/// <para>
	/// DefineData can also be used to initialize Actionscript3 classes when they are
	/// loaded into the Flash Player. The table in a SymbolClass object maps class
	/// names to object definitions in the movie using a unique identifier. If the
	/// class is a sub-class of ByteArray then the data from the DefineData object
	/// with a matching identifier will be used to initialize the class.
	/// </para>
	/// </summary>
	/// <seealso cref= SymbolClass </seealso>
	public sealed class DefineData : DefineTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineData: { identifier=%d;" + " data=byte<%d> ...}";

		/// <summary>
		/// Unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// Binary encoded data. </summary>
		private byte[] data;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a DefineData object using values encoded in the
		/// Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public DefineData(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			coder.readInt(); // always zero
			data = coder.readBytes(new byte[length - coder.bytesRead()]);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a DefineData object with the specified data.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier used to reference this object. </param>
		/// <param name="bytes">
		///            the data to initialize the object. </param>


		public DefineData(int uid, byte[] bytes)
		{
			Identifier = uid;
			Data = bytes;
		}

		/// <summary>
		/// Creates a DefineData initialize with a copy of the data from another
		/// object.
		/// </summary>
		/// <param name="object">
		///            a DefineData object used to initialize this one. </param>


		public DefineData(DefineData @object)
		{
			identifier = @object.identifier;
			data = @object.data;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public int Identifier
		{
			get => identifier;
		    set
			{
				if ((value < 1) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, value);
				}
				identifier = value;
			}
		}


		/// <summary>
		/// Get the array of bytes that will be embedded in the Flash file.
		/// </summary>
		/// <returns> a copy of the data. </returns>
		public byte[] Data
		{
			get => Arrays.copyOf(data, data.Length);
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				data = Arrays.copyOf(value, value.Length);
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public DefineData copy()
		{
			return new DefineData(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, data.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			//CHECKSTYLE:OFF
			length = 6 + data.Length;
			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
			//CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_BINARY_DATA << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_BINARY_DATA << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			coder.writeInt(0);
			coder.writeBytes(data);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}