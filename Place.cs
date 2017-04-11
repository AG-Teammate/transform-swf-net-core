using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;

/*
 * PlaceObject.java
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
	/// PlaceObject is used to add an object (shape, button, etc.) to the Flash
	/// Player's display list.
	/// 
	/// <para>
	/// When adding an object to the display list a coordinate transform can be
	/// applied to the object. This is principally used to specify the location of
	/// the object when it is drawn on the screen however more complex coordinate
	/// transforms can also be specified such as rotating or scaling the object
	/// without changing the original definition.
	/// </para>
	/// 
	/// <para>
	/// Similarly the color transform allows the color of the object to be changed
	/// when it is displayed without changing the original definition. The
	/// PlaceObject class only supports opaque colours so although the ColorTransform
	/// supports transparent colours this information is ignored by the Flash Player.
	/// The colour transform is optional and may be set to null, reducing the size of
	/// the PlaceObject instruction when it is encoded.
	/// </para>
	/// </summary>
	/// <seealso cref= Place2 </seealso>
	/// <seealso cref= Remove </seealso>
	/// <seealso cref= Remove2 </seealso>
	public sealed class Place : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Place: { layer=%d; identifier=%d;" + " transform=%s; colorTransform=%s}";

		/// <summary>
		/// The unique identifier of the object that will be displayed. </summary>
		private int identifier;
		/// <summary>
		/// The display list layer number. </summary>
		private int layer;
		/// <summary>
		/// The coordinate transform applied to the displayed object. </summary>
		private CoordTransform transform;
		/// <summary>
		/// The color transform applied to the displayed object. </summary>
		private ColorTransform colorTransform;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a Place object using values encoded
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



		public Place(SWFDecoder coder, Context context)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			layer = coder.readUnsignedShort();
			transform = new CoordTransform(coder);
			if (coder.bytesRead() < length)
			{
				colorTransform = new ColorTransform(coder, context);
			}
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a PlaceObject object that places the the object with the
		/// identifier at the specified layer and at the position specified by the
		/// coordinate transform.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for the object to the placed on the
		///            display list. Must be in the range 1..65535. </param>
		/// <param name="level">
		///            the layer in the display list where the object will be placed. </param>
		/// <param name="position">
		///            an CoordTransform object that defines the orientation, size
		///            and location of the object when it is drawn. Must not be null. </param>


		public Place(int uid, int level, CoordTransform position)
		{
			Identifier = uid;
			Layer = level;
			Transform = position;
		}

		/// <summary>
		/// Creates a PlaceObject object that places the the object with the
		/// identifier at the specified layer, coordinate transform and colour
		/// transform.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for the object to the placed on the
		///            display list. Must be in the range 1..65535. </param>
		/// <param name="level">
		///            the layer in the display list where the object will be placed. </param>
		/// <param name="matrix">
		///            an CoordTransform object that defines the orientation, size
		///            and location of the object when it is drawn. Must not be null. </param>
		/// <param name="cxform">
		///            an ColorTransform object that defines the colour of the object
		///            when it is drawn. </param>


		public Place(int uid, int level, CoordTransform matrix, ColorTransform cxform)
		{
			Identifier = uid;
			Layer = level;
			Transform = matrix;
			ColorTransform = cxform;
		}

		/// <summary>
		/// Creates and initialises a Place object using the values copied
		/// from another Place object.
		/// </summary>
		/// <param name="object">
		///            a Place object from which the values will be
		///            copied. </param>


		public Place(Place @object)
		{
			identifier = @object.identifier;
			layer = @object.layer;
			transform = @object.transform;
			colorTransform = @object.colorTransform;
		}

		/// <summary>
		/// Get the identifier of the object to be placed. This is only required
		/// when placing an object for the first time. Subsequent references to the
		/// object on this layer can simply use the layer number.
		/// </summary>
		/// <returns> the unique identifier of the object to be displayed. </returns>
		public int Identifier
		{
			get => identifier;
		    set => setIdentifier(value);
		}

		/// <summary>
		/// Sets the identifier of the object that will be added to the display list.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for the object to the placed on the
		///            display list. Must be in the range 1..65535. </param>
		/// <returns> this object. </returns>


		public Place setIdentifier(int uid)
		{
			if ((uid < 1) || (uid > Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, uid);
			}
			identifier = uid;
			return this;
		}

		/// <summary>
		/// Get the Layer on which the object will be displayed in the display
		/// list.
		/// </summary>
		/// <returns> the layer where the object will be displayed. </returns>
		public int Layer
		{
			get => layer;
		    set => setLayer(value);
		}

		/// <summary>
		/// Sets the layer that defines the order in which objects are displayed.
		/// </summary>
		/// <param name="aNumber">
		///            the layer in the display list where the object will be placed.
		///            Must be in the range 1..65535. </param>
		/// <returns> this object. </returns>


		public Place setLayer(int aNumber)
		{
			if ((aNumber < 1) || (aNumber > Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, aNumber);
			}
			layer = aNumber;
			return this;
		}

		/// <summary>
		/// Get the coordinate transform. May be null if no coordinate transform
		/// was defined.
		/// </summary>
		/// <returns> the coordinate transform that will be applied to the displayed
		/// object. </returns>
		public CoordTransform Transform
		{
			get => transform;
		    set => setTransform(value);
		}

		/// <summary>
		/// Sets the transform that defines the position where the object is
		/// displayed.
		/// </summary>
		/// <param name="matrix">
		///            an CoordTransform object that defines the orientation, size
		///            and location of the object when it is drawn. Must not be null. </param>
		/// <returns> this object. </returns>


		public Place setTransform(CoordTransform matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentException();
			}
			transform = matrix;
			return this;
		}

		/// <summary>
		/// Get the colour transform. May be null if no colour transform was
		/// defined.
		/// </summary>
		/// <returns> the colour transform that will be applied to the displayed
		/// object. </returns>
		public ColorTransform ColorTransform
		{
			get => colorTransform;
		    set => setColorTransform(value);
		}

		/// <summary>
		/// Sets the location where the object will be displayed.
		/// </summary>
		/// <param name="xCoord">
		///            the x-coordinate of the object's origin. </param>
		/// <param name="yCoord">
		///            the x-coordinate of the object's origin. </param>
		/// <returns> this object. </returns>


		public Place setLocation(int xCoord, int yCoord)
		{
			transform = CoordTransform.translate(xCoord, yCoord);
			return this;
		}

		/// <summary>
		/// Sets the colour transform that defines any colour effects applied when
		/// the object is displayed.
		/// </summary>
		/// <param name="cxform">
		///            an ColorTransform object that defines the colour of the object
		///            when it is drawn. May be set to null. </param>
		/// <returns> this object. </returns>


		public Place setColorTransform(ColorTransform cxform)
		{
			colorTransform = cxform;
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public Place copy()
		{
			return new Place(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, layer, transform, colorTransform);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			length = 4;
			length += transform.prepareToEncode(context);

			if (colorTransform != null)
			{
				length += colorTransform.prepareToEncode(context);
			}

			return 2 + length;
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.PLACE << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.PLACE << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			coder.writeShort(layer);
			transform.encode(coder, context);
		    colorTransform?.encode(coder, context);
		    if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}