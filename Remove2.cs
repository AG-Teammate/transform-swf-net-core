/*
 * RemoveObject2.java
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

using System;
using System.IO;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

namespace com.flagstone.transform
{
    /// <summary>
	/// RemoveObject2 removes an object from the display list, requiring only the
	/// layer number.
	/// </summary>
	/// <seealso cref= Remove </seealso>
	public sealed class Remove2 : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Remove2: { layer=%d}";

		/// <summary>
		/// The layer where the object is displayed. </summary>
		private int layer;

		/// <summary>
		/// Creates and initialises a Remove2 object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Remove2(SWFDecoder coder)
		{
			if ((coder.readUnsignedShort() & Coder.LENGTH_FIELD) == Coder.IS_EXTENDED)
			{
				coder.readInt();
			}
			layer = coder.readUnsignedShort();
		}

		/// <summary>
		/// Creates a RemoveObject2, specifying the layer in the display list where
		/// the object to be removed is currently displayed.
		/// </summary>
		/// <param name="level">
		///            the layer number on which the object is displayed. Must be in
		///            the range 1.65535. </param>


		public Remove2(int level)
		{
			Layer = level;
		}

		/// <summary>
		/// Creates and initialises a Remove2 object using the values copied
		/// from another Remove2 object.
		/// </summary>
		/// <param name="object">
		///            a Remove2 object from which the values will be
		///            copied. </param>


		public Remove2(Remove2 @object)
		{
			layer = @object.layer;
		}

		/// <summary>
		/// Get the layer in the display list where the object to be removed is
		/// currently displayed.
		/// </summary>
		/// <returns> the layer number. </returns>
		public int Layer
		{
			get => layer;
		    set
			{
				if ((value < 1) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, value);
				}
				layer = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public Remove2 copy()
		{
			return new Remove2(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, layer);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			return 4;
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeShort((MovieTypes.REMOVE_2 << Coder.LENGTH_FIELD_SIZE) | 2);
			coder.writeShort(layer);
		}
	}

}