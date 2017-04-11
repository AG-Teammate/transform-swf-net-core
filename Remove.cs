/*
 * RemoveObject.java
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
	/// RemoveObject removes an object from the Flash Player's Display List.
	/// 
	/// <para>
	/// An object placed on the display list is displayed in every frame of a movie
	/// until it is explicitly removed. Objects must also be removed if its location
	/// or appearance is changed using PlaceObject.
	/// </para>
	/// 
	/// <para>
	/// Although only one object can be placed on any layer in the display list both
	/// the object's unique identifier and the layer number must be specified. The
	/// RemoveObject class is superseded in Flash 3 by the RemoveObject2 class which
	/// lifts this requirement allowing an object to be referenced by the layer
	/// number it occupies in the display list.
	/// </para>
	/// </summary>
	/// <seealso cref= Remove2 </seealso>
	/// <seealso cref= Place </seealso>
	/// <seealso cref= Place2 </seealso>
	public sealed class Remove : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Remove: { identifier=%d; layer=%d}";

		/// <summary>
		/// The unique identifier of the object on the display list. </summary>
		private int identifier;
		/// <summary>
		/// The layer where the object is displayed. </summary>
		private int layer;

		/// <summary>
		/// Creates and initialises a Remove object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Remove(SWFDecoder coder)
		{
			if ((coder.readUnsignedShort() & Coder.LENGTH_FIELD) == Coder.IS_EXTENDED)
			{
				coder.readInt();
			}
			identifier = coder.readUnsignedShort();
			layer = coder.readUnsignedShort();
		}

		/// <summary>
		/// Creates a RemoveObject object that will remove an object with the
		/// specified identifier from the given layer in the display list.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for the object currently on the display
		///            list. Must be in the range 1.65535. </param>
		/// <param name="level">
		///            the layer in the display list where the object is being
		///            displayed. Must be in the range 1.65535. </param>


		public Remove(int uid, int level)
		{
			Identifier = uid;
			Layer = level;
		}

		/// <summary>
		/// Creates and initialises a Remove object using the values copied
		/// from another Remove object.
		/// </summary>
		/// <param name="object">
		///            a Remove object from which the values will be
		///            copied. </param>


		public Remove(Remove @object)
		{
			identifier = @object.identifier;
			layer = @object.layer;
		}

		/// <summary>
		/// Get the identifier of the object to be removed from the display list.
		/// </summary>
		/// <returns> the identifier if the object to be removed. </returns>
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
		/// Get the layer in the display list where the object will be displayed.
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
		public Remove copy()
		{
			return new Remove(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, layer);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 6;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			coder.writeShort((MovieTypes.REMOVE << Coder.LENGTH_FIELD_SIZE) | 4);
			coder.writeShort(identifier);
			coder.writeShort(layer);
		}
	}

}