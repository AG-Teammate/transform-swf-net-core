using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * Import.java
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
	/// Import is used to import shapes and other objects from another Flash file
	/// referenced by a URL.
	/// 
	/// <para>
	/// To provide a degree of security the Flash Player will only import files that
	/// originate from the same domain as the file that it is currently playing. For
	/// example if the Flash file being shown was loaded from
	/// www.mydomain.com/flash.swf then the file contains the exported objects must
	/// reside somewhere at www.mydomain.com. This prevents a malicious Flash file
	/// from loading files from an unknown third party.
	/// </para>
	/// 
	/// <para>
	/// Since the identifier for an object is only unique within a given Flash file,
	/// imported objects are referenced by a name assigned when the object is
	/// exported.
	/// </para>
	/// </summary>
	/// <seealso cref= Export </seealso>
	public sealed class Import : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Import: { url=%s; objects=%s}";

		/// <summary>
		/// The URL referencing the file to import. </summary>
		private string url;
		/// <summary>
		/// The table of mapping named objects to unique identifiers. </summary>
		private IDictionary<int?, string> objects;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises an Import object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Import(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			url = coder.readString();


			int count = coder.readUnsignedShort();
			objects = new Dictionary<int?, string>(count);
			for (int i = 0; i < count; i++)
			{
				objects[coder.readUnsignedShort()] = coder.readString();
			}
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates an Import object with an empty table.
		/// </summary>
		public Import()
		{
			objects = new Dictionary<int?, string>();
		}

		/// <summary>
		/// Creates a Import object that imports an object from the specified file.
		/// </summary>
		/// <param name="aUrl">
		///            the URL referencing the file to be imported.
		/// </param>
		/// <param name="map">
		///            the table to add the identifier-name pairs of the objects that
		///            will be imported. </param>


		public Import(string aUrl, IDictionary<int?, string> map)
		{
			Url = aUrl;
			objects = map;
		}

		/// <summary>
		/// Creates and initialises an Import object using the values copied
		/// from another Import object.
		/// </summary>
		/// <param name="object">
		///            an Import object from which the values will be
		///            copied. </param>


		public Import(Import @object)
		{
			url = @object.url;
			objects = new Dictionary<int?, string>(@object.objects);
		}

		/// <summary>
		/// Adds the identifier and name to the list of objects to be imported.
		/// </summary>
		/// <param name="uid">
		///            the identifier of the object to be imported. Must be in the
		///            range 1..65535. </param>
		/// <param name="aString">
		///            the name of the imported object to allow it to be referenced.
		///            Must not be null or an empty string. </param>
		/// <returns> this object. </returns>


		public Import add(int uid, string aString)
		{
			if ((uid < 1) || (uid > Coder.USHORT_MAX))
			{
				 throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, uid);
			}
			if (ReferenceEquals(aString, null) || aString.Length == 0)
			{
				throw new ArgumentException();
			}
			objects[uid] = aString;
			return this;
		}

		/// <summary>
		/// Get the URL of the file containing the object to be imported. Limited
		/// security is provided by requiring that the URL must be in the same domain
		/// or sub-domain as the URL of the movie which contains this object.
		/// </summary>
		/// <returns> the URL used to import the objects. </returns>
		public string Url
		{
			get => url;
		    set
			{
				if (ReferenceEquals(value, null) || value.Length == 0)
				{
					throw new ArgumentException();
				}
				url = value;
			}
		}

		/// <summary>
		/// Get the table of objects to be imported.
		/// </summary>
		/// <returns> the mapping of the names of the imported objects to unique
		/// identifiers in this movie. </returns>
		public IDictionary<int?, string> Objects
		{
			get => objects;
		    set => objects = value;
		}



		/// <summary>
		/// {@inheritDoc} </summary>
		public Import copy()
		{
			return new Import(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, url, objects);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 2 + context.strlen(url);

			foreach (int? identifier in objects.Keys)
			{
				length += 2 + context.strlen(objects[identifier]);
			}

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.IMPORT << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.IMPORT << Coder.LENGTH_FIELD_SIZE) | length);
			}
			coder.writeString(url);
			coder.writeShort(objects.Count);

			foreach (int? identifier in objects.Keys)
			{
				coder.writeShort(identifier.Value);
				coder.writeString(objects[identifier]);
			}
		}
	}

}