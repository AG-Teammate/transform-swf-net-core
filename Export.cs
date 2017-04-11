using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * Export.java
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
	/// Export is used to export one or more shapes and other objects so they can be
	/// used in another Flash file.
	/// 
	/// <para>
	/// Since the identifier for an object is only unique within a given Flash file,
	/// each object exported is assigned a name so it can referenced when it is
	/// imported.
	/// </para>
	/// </summary>
	public sealed class Export : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Export: { objects=%s}";
		/// <summary>
		/// Table containing the objects to export. </summary>
		private IDictionary<int?, string> objects;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises an Export object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Export(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();


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
		/// Creates an Export object with an empty map.
		/// </summary>
		public Export()
		{
			objects = new Dictionary<int?, string>();
		}

		/// <summary>
		/// Creates an Export object with the specified map.
		/// </summary>
		/// <param name="map">
		///            the table containing identifier/name pairs for the objects
		///            that will be exported from the movie. </param>


		public Export(IDictionary<int?, string> map)
		{
			objects = map;
		}

		/// <summary>
		/// Creates and initialises an Export object using the values copied
		/// from another Export object.
		/// </summary>
		/// <param name="object">
		///            a Export object from which the values will be
		///            copied. </param>


		public Export(Export @object)
		{
			objects = new Dictionary<int?, string>(@object.objects.Count);
		    foreach (var obj in @object.objects)
		    {
		        objects.Add(obj);
		    }
		}

		/// <summary>
		/// Adds the identifier and name to the list of objects to be exported.
		/// </summary>
		/// <param name="uid">
		///            the identifier of the object to be exported. </param>
		/// <param name="aString">
		///            the name of the exported object to allow it to be referenced.
		///            The name must not be null or an empty string. </param>
		/// <returns> this object. </returns>


		public Export add(int uid, string aString)
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
		/// Get the table of objects to be exported.
		/// </summary>
		/// <returns> the table of objects mapping identifiers to names. </returns>
		public IDictionary<int?, string> Objects
		{
			get => objects;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				objects = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public Export copy()
		{
			return new Export(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, objects);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 2;

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
				coder.writeShort((MovieTypes.EXPORT << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.EXPORT << Coder.LENGTH_FIELD_SIZE) | length);
			}
			coder.writeShort(objects.Count);

			foreach (int? identifier in objects.Keys)
			{
				coder.writeShort(identifier.Value);
				coder.writeString(objects[identifier]);
			}
		}
	}

}