/*
 * Free.java
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
	/// Free deletes the object with the specified identifier, freeing up resources
	/// in the Flash Player.
	/// </summary>
	public sealed class Free : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Free: { identifier=%d}";

		/// <summary>
		/// The unique identifier of the object that will be deleted. </summary>
		private int identifier;

		/// <summary>
		/// Creates and initialises a Free object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Free(SWFDecoder coder)
		{
			if ((coder.readUnsignedShort() & Coder.LENGTH_FIELD) == Coder.IS_EXTENDED)
			{
				coder.readInt();
			}
			identifier = coder.readUnsignedShort();
		}

		/// <summary>
		/// Creates a Free object with the specified identifier.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier of the object to be deleted. Must be in
		///            the range 1..65535. </param>


		public Free(int uid)
		{
			Identifier = uid;
		}

		/// <summary>
		/// Creates a Free initialised with a copy of the data from another object.
		/// </summary>
		/// <param name="object">
		///            a Free object used to initialise this one. </param>


		public Free(Free @object)
		{
			identifier = @object.identifier;
		}

		/// <summary>
		/// Get the identifier of the object to be deleted.
		/// </summary>
		/// <returns> the object identifier. </returns>
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
		/// {@inheritDoc} </summary>
		public Free copy()
		{
			return new Free(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier);
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
			coder.writeShort((MovieTypes.FREE << Coder.LENGTH_FIELD_SIZE) | 2);
			coder.writeShort(identifier);
		}
	}

}