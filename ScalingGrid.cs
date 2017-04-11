using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;

/*
 * ScalingGrid.java
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
	/// ScalingGrid is used to define a 9-slice grid that can be used to control
	/// the way an object is scaled. The Bounds defines the central square of the
	/// grid. The (two) squares above and below the Bounds are scaled horizontally;
	/// the (two) squares to the left and right are scaled vertically and the (four)
	/// corner squares are not scaled at all.
	/// </summary>
	public sealed class ScalingGrid : DefineTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "ScalingGrid: { identifier=%d;" + " bounds=%s}";

		/// <summary>
		/// the unique identifier of the object. </summary>
		private int identifier;
		/// <summary>
		/// The box that defines the centre of the grid. </summary>
		private Bounds bounds;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a ScalingGrid object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public ScalingGrid(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			identifier = coder.readUnsignedShort();
			bounds = new Bounds(coder);
		}

		/// <summary>
		/// Creates and initialises a ScalingGrid with the specified object
		/// identifier and bounding box for the centre section.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier of the object to which the grid will be
		///            applied </param>
		/// <param name="rect">
		///            the bounding box that defines the coordinates of the centre
		///            section of the grid. </param>


		public ScalingGrid(int uid, Bounds rect)
		{
			Identifier = uid;
			Bounds = rect;
		}

		/// <summary>
		/// Creates and initialises a ScalingGrid object using the values copied
		/// from another ScalingGrid object.
		/// </summary>
		/// <param name="object">
		///            a ScalingGrid object from which the values will be
		///            copied. </param>


		public ScalingGrid(ScalingGrid @object)
		{
			identifier = @object.identifier;
			bounds = @object.bounds;
		}

		/// <summary>
		/// Get the identifier of the object which the scaling grid will be applied
		/// to.
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
		/// Get the bounding box that defined the centre section of the scaling grid. </summary>
		/// <returns> the box defining the centre of the grid. </returns>
		public Bounds Bounds
		{
			get => bounds;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				bounds = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public ScalingGrid copy()
		{
			return new ScalingGrid(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, bounds.ToString());
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 2 + bounds.prepareToEncode(context);
			return 2 + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			coder.writeShort((MovieTypes.DEFINE_SCALING_GRID << Coder.LENGTH_FIELD_SIZE) | length);
			coder.writeShort(identifier);
			bounds.encode(coder, context);
		}
	}

}