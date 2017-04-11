using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;

/*
 * Push.java
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
	/// Push is used to push values on the Flash Player's internal stack.
	/// 
	/// <para>
	/// Push supports the full range of data types supported by Flash:
	/// </para>
	/// 
	/// <table class="datasheet">
	/// <tr>
	/// <td valign="top" nowrap width="20%">Boolean</td>
	/// <td>A boolean value, 1 (true) or 0 (false).</td>
	/// </tr>
	/// <tr>
	/// <td valign="top" nowrap width="20%">Integer</td>
	/// <td>A signed 32-bit integer, range -2,147,483,648 to 2,147,483,647.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top" nowrap width="20%">Double</td>
	/// <td>A double-precision (64-bit) floating-point number, range approximately
	/// +/- 1.79769313486231570E+308.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top" nowrap width="20%">String</td>
	/// <td>A String. The string is encoded as using the UTF-8 encoding which is
	/// backward compatible with ASCII encoding supported in Flash 5.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top" nowrap width="20%">Register Index</td>
	/// <td>
	/// The number (0..255) of one of the Flash player's internal registers.
	/// </td>
	/// </tr>
	/// <tr>
	/// <td valign="top" nowrap width="20%">Table Index</td>
	/// <td>
	/// An index into a table of string literals defined using the Table action.
	/// </td>
	/// </tr>
	/// <tr>
	/// <td valign="top" nowrap width="20%">Null</td>
	/// <td>A null value.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top" nowrap width="20%">Void</td>
	/// <td>A void value.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top" nowrap width="20%">Movie Clip Property</td>
	/// <td>
	/// A reserved number used to identify a specific property of a movie clip.
	/// </td>
	/// </tr>
	/// <tr>
	/// <td valign="top" nowrap width="20%">Player Property</td>
	/// <td>A reserved number used to identify a specific property of the Flash
	/// Player.</td>
	/// </tr>
	/// </table>
	/// </summary>
	/// <seealso cref= Null </seealso>
	/// <seealso cref= Property </seealso>
	/// <seealso cref= RegisterIndex </seealso>
	/// <seealso cref= TableIndex </seealso>
	/// <seealso cref= Void
	///  </seealso>


	public sealed class Push : Action
	{

		/// <summary>
		/// Number of bits in an int. </summary>
		private const int BITS_PER_INT = 32;
		/// <summary>
		/// Number of last internal register in the Flash Player. </summary>
		private const int LAST_REGISTER = 255;
		/// <summary>
		/// Bit mask used for reading writing double values. </summary>
		private const long MASK_32 = 0x00000000FFFFFFFFL;
		/// <summary>
		/// Shift used for reading writing double values. </summary>
		private const int WORD_ALIGN = 32;

		/// <summary>
		/// The Builder class is used to generate a new Push object.
		/// </summary>
		public sealed class Builder
		{
			/// <summary>
			/// The list of values to push onto the stack. </summary>
			
			internal readonly IList<object> objects = new List<object>();

			/// <summary>
			/// Adds a value to the list.
			/// </summary>
			/// <param name="value">
			///            a value that will be pushed onto the Flash Player's stack
			///            when the action is executed. </param>
			/// <returns> this object. </returns>


			public Builder add(object value)
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				objects.Add(value);
				return this;
			}

			/// <summary>
			/// Clear the list of values added to the Builder. </summary>
			/// <returns> this object. </returns>
			public Builder clear()
			{
				objects.Clear();
				return this;
			}

			/// <summary>
			/// Generate a Push using the set of values defined in the Builder. </summary>
			/// <returns> an initialized Push object. </returns>
			public Push build()
			{
				return new Push(objects);
			}
		}

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Push: %s";
		/// <summary>
		/// Type identifying Strings. </summary>
		private const int TYPE_STRING = 0;
		/// <summary>
		/// Type identifying Properties. </summary>
		private const int TYPE_PROPERTY = 1;
		/// <summary>
		/// Type identifying Null values. </summary>
		private const int TYPE_NULL = 2;
		/// <summary>
		/// Type identifying Void values. </summary>
		private const int TYPE_VOID = 3;
		/// <summary>
		/// Type identifying RegisterIndex object. </summary>
		private const int TYPE_REGISTER = 4;
		/// <summary>
		/// Type identifying Boolean values. </summary>
		private const int TYPE_BOOLEAN = 5;
		/// <summary>
		/// Type identifying Double values. </summary>
		private const int TYPE_DOUBLE = 6;
		/// <summary>
		/// Type identifying Integer values. </summary>
		private const int TYPE_INTEGER = 7;
		/// <summary>
		/// Type identifying indices into Tables with up to 255 entries. </summary>
		private const int TYPE_TINDEX = 8;
		/// <summary>
		/// Type identifying indices into Tables with more than 255 entries. </summary>
		private const int TYPE_LARGE_TINDEX = 9;

		/// <summary>
		/// Length of encoded Properties. </summary>
		private const int LENGTH_PROPERTY = 5;
		/// <summary>
		/// Length of encoded Null values. </summary>
		private const int LENGTH_NULL = 1;
		/// <summary>
		/// Length of encoded Void values. </summary>
		private const int LENGTH_VOID = 1;
		/// <summary>
		/// Length of encoded RegisterIndex object. </summary>
		private const int LENGTH_RINDEX = 2;
		/// <summary>
		/// Length of encoded Boolean values. </summary>
		private const int LENGTH_BOOLEAN = 2;
		/// <summary>
		/// Length of encoded Double values. </summary>
		private const int LENGTH_DOUBLE = 9;
		/// <summary>
		/// Length of encoded Integer values. </summary>
		private const int LENGTH_INTEGER = 5;
		/// <summary>
		/// Length of encoded indices for Tables with up to 255 entries. </summary>
		private const int LENGTH_TINDEX = 2;
		/// <summary>
		/// Length of encoded indices for Tables with more than 255 entries. </summary>
		private const int LENGTH_LTINDEX = 3;

		/// <summary>
		/// The list of values that will be pushed onto the Flash Player's stack. </summary>
		
		private readonly IList<object> values;

		/// <summary>
		/// The length of the action, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a Push action using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <param name="context">
		///            a Context object used to manage the decoders for different
		///            type of object and to pass information on how objects are
		///            decoded.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Push(SWFDecoder coder, Context context)
		{

			length = coder.readUnsignedShort();
			values = new List<object>();

			int valuesLength = length;

			while (valuesLength > 0)
			{


				int dataType = coder.readByte();

				switch (dataType)
				{
				case TYPE_STRING:


					string str = coder.readString();
					values.Add(str);
					valuesLength -= 1 + context.strlen(str);
					break;
				case TYPE_PROPERTY:
					if (context.get(Context.VERSION) < Property.VERSION_WITH_INTS)
					{
						values.Add(new Property((int) Float.intBitsToFloat(coder.readInt())));
					}
					else
					{
						values.Add(new Property(coder.readInt()));
					}
					 valuesLength -= LENGTH_PROPERTY;
					break;
				case TYPE_NULL:
					values.Add(Null.Instance);
					valuesLength -= LENGTH_NULL;
					break;
				case TYPE_VOID:
					values.Add(Void.Instance);
					valuesLength -= LENGTH_VOID;
					break;
				case TYPE_REGISTER:
					values.Add(new RegisterIndex(coder.readByte()));
					valuesLength -= LENGTH_RINDEX;
					break;
				case TYPE_BOOLEAN:
					values.Add(coder.readByte() != 0);
					valuesLength -= LENGTH_BOOLEAN;
					break;
				case TYPE_DOUBLE:
					long longValue = (long) coder.readInt() << WORD_ALIGN;
					longValue |= coder.readInt() & MASK_32;
					values.Add(BitConverter.DoubleToInt64Bits(longValue));
					valuesLength -= LENGTH_DOUBLE;
					break;
				case TYPE_INTEGER:
					values.Add(coder.readInt());
					valuesLength -= LENGTH_INTEGER;
					break;
				case TYPE_TINDEX:
					values.Add(new TableIndex(coder.readByte()));
					valuesLength -= LENGTH_TINDEX;
					break;
				case TYPE_LARGE_TINDEX:
					values.Add(new TableIndex(coder.readUnsignedShort()));
					valuesLength -= LENGTH_LTINDEX;
					break;
				default:
					break;
				}
			}
		}

		/// <summary>
		/// Creates a Push action that will push the values in the list onto the
		/// stack.
		/// </summary>
		/// <param name="list">
		///            a list of values to be pushed onto the stack. The values must
		///            be one of the following classes: Boolean, Integer, Double,
		///            String, RegisterIndex or TableIndex. Must not be null. </param>


		public Push(IList<object> list)
		{
			if (list == null)
			{
				throw new ArgumentException();
			}
			values = new List<object>(list);
		}

		/// <summary>
		/// Creates and initialises a Push action using the values
		/// copied from another Push action.
		/// </summary>
		/// <param name="object">
		///            a Push action from which the values will be
		///            copied. References to immutable objects will be shared. </param>


		public Push(Push @object)
		{
			values = new List<object>(@object.values);
		}


		/// <summary>
		/// Get the list of values that will be pushed onto the Flash Player's
		/// stack.
		/// </summary>
		/// <returns> a copy of the list of values. </returns>
		public IList<object> Values => new List<object>(values);

	    /// <summary>
		/// {@inheritDoc} </summary>
		public Push copy()
		{
			return this;
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

			length = 0;

			foreach (Object obj in values)
			{
				if (obj is bool?)
				{
					length += LENGTH_BOOLEAN;
				}
				else if (obj is Property)
				{
					length += LENGTH_PROPERTY;
				}
				else if (obj is int?)
				{
					length += LENGTH_INTEGER;
				}
				else if (obj is double?)
				{
					length += LENGTH_DOUBLE;
				}
				else if (obj is string)
				{
					length += 1 + context.strlen(obj.ToString());
				}
				else if (obj is Null)
				{
					length += LENGTH_NULL;
				}
				else if (obj is Void)
				{
					length += LENGTH_VOID;
				}
				else if (obj is TableIndex)
				{
					if (((TableIndex) obj).Index <= LAST_REGISTER)
					{
						length += LENGTH_TINDEX;
					}
					else
					{
						length += LENGTH_LTINDEX;
					}
				}
				else if (obj is RegisterIndex)
				{
					length += 2;
				}
			}

			return Coder.ACTION_HEADER + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			coder.writeByte(ActionTypes.PUSH);
			coder.writeShort(length);

			foreach (Object obj in values)
			{
				if (obj is bool?)
				{
					coder.writeByte(TYPE_BOOLEAN);
					if (((bool?) obj).Value)
					{
						coder.writeByte(1);
					}
					else
					{
						coder.writeByte(0);
					}
				}
				else if (obj is int?)
				{
					coder.writeByte(TYPE_INTEGER);
					coder.writeInt(((int?) obj).Value);
				}
				else if (obj is Property)
				{
					coder.writeByte(TYPE_PROPERTY);
					coder.writeInt(((Property) obj).getValue(context.get(Context.VERSION)));
				}
				else if (obj is double?)
				{
					coder.writeByte(TYPE_DOUBLE);


					long longValue = BitConverter.DoubleToInt64Bits(((double?) obj).Value);
					coder.writeInt((int)(longValue >> BITS_PER_INT));
					coder.writeInt((int) longValue);
				}
				else if (obj is string)
				{
					coder.writeByte(TYPE_STRING);
					coder.writeString(obj.ToString());
				}
				else if (obj is Null)
				{
					coder.writeByte(TYPE_NULL);
				}
				else if (obj is Void)
				{
					coder.writeByte(TYPE_VOID);
				}
				else if (obj is TableIndex)
				{
					if (((TableIndex) obj).Index <= LAST_REGISTER)
					{
						coder.writeByte(TYPE_TINDEX);
						coder.writeByte(((TableIndex) obj).Index);
					}
					else
					{
						coder.writeByte(TYPE_LARGE_TINDEX);
						coder.writeShort(((TableIndex) obj).Index);
					}
				}
				else if (obj is RegisterIndex)
				{
					coder.writeByte(TYPE_REGISTER);
					coder.writeByte(((RegisterIndex) obj).Number);
				}
				else
				{

					throw new CoderException(0, "Unsupported type: " + obj.GetType().FullName);
				}
			}
		}
	}

}