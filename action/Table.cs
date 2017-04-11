using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;

/*
 * Table.java
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

namespace com.flagstone.transform.action
{
    /// <summary>
	/// Table is used to create a table of string literals that can be referenced by
	/// an index rather than using the literal value when executing a sequence of
	/// actions.
	/// 
	/// <para>
	/// Variables and built-in functions are specified by their name and the Table
	/// class contains a table of the respective strings. References to a variable or
	/// function can then use its index in the table rather than the name resulting
	/// in a more compact representation when the actions are encoded into binary
	/// form.
	/// </para>
	/// </summary>
	/// <seealso cref= TableIndex </seealso>
	/// <seealso cref= Push </seealso>
	public sealed class Table : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Table: { values=%s}";

		/// <summary>
		/// The list of strings stored in the table. </summary>
		private IList<string> values;

		/// <summary>
		/// The length of the action, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// The number of entries in the table. </summary>
		
		private int tableSize;

		/// <summary>
		/// Creates and initialises a Table action using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Table(SWFDecoder coder)
		{
			length = coder.readUnsignedShort();
			tableSize = coder.readUnsignedShort();
			values = new List<string>(tableSize);

			if (tableSize > 0)
			{
				for (int i = 0; i < tableSize; i++)
				{
					values.Add(coder.readString());
				}
			}
			else
			{
				/*
				 * Reset the length as Macromedia is using the length of a table to
				 * hide code following an empty table.
				 */
				length = 2;
			}
		}

		/// <summary>
		/// Creates an empty Table.
		/// </summary>
		public Table()
		{
			values = new List<string>();
		}

		/// <summary>
		/// Creates a Table using the list of strings.
		/// </summary>
		/// <param name="list">
		///            of Strings that will be added to the table. Must not be null. </param>


		public Table(IList<string> list)
		{
			values = new List<string>();
			Values = list;
		}

		/// <summary>
		/// Creates and initialises a Table action using the values
		/// copied from another Table action.
		/// </summary>
		/// <param name="object">
		///            a Table action from which the values will be
		///            copied. </param>


		public Table(Table @object)
		{
			values = new List<string>(@object.values);
		}

		/// <summary>
		/// Adds a String to the variable table.
		/// </summary>
		/// <param name="aString">
		///            a String that will be added to the end of the table. Must not
		///            be null.
		/// </param>
		/// <returns> this table. </returns>


		public Table add(string aString)
		{
			if (ReferenceEquals(aString, null))
			{
				throw new ArgumentException();
			}
			values.Add(aString);
			return this;
		}

		/// <summary>
		/// Get the table of strings.
		/// </summary>
		/// <returns> the list of Strings stored in the table. </returns>
		public IList<string> Values
		{
			get => values;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				values = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public Table copy()
		{
			return new Table(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, values);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 2;

			foreach (String str in values)
			{
				length += context.strlen(str);
			}

			tableSize = values.Count;

			return Coder.ACTION_HEADER + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ActionTypes.TABLE);
			coder.writeShort(length);
			coder.writeShort(values.Count);

			foreach (String str in values)
			{
				coder.writeString(str);
			}
		}
	}

}