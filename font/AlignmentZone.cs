using System;
using System.IO;
using com.flagstone.transform.coder;

/*
 * AlignmentZone.java
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

namespace com.flagstone.transform.font
{
    /// <summary>
	/// AlignmentZone defines a bounding box that is used by the advanced text
	/// rendering engine in the Flash Player to snap glyphs to the nearest pixel.
	/// </summary>
	public sealed class AlignmentZone : SWFEncodeable
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "AlignmentZone: {" + " coordinate=%f; range=%f}";

		/// <summary>
		/// The position of the edge of the zone. </summary>
		
		private readonly float coordinate;
		/// <summary>
		/// The width or height of the zone. </summary>
		
		private readonly float range;

		/// <summary>
		/// Creates and initialises an AlignmentZone object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public AlignmentZone(SWFDecoder coder)
		{
			coordinate = coder.readHalf();
			range = coder.readHalf();
		}

		/// <summary>
		/// Creates a new AlignmentZone with the specified coordinate and size.
		/// </summary>
		/// <param name="coord"> the x or y coordinate of the left edge or bottom of the box. </param>
		/// <param name="size"> the width or height of the box. </param>


		public AlignmentZone(float coord, float size)
		{
			coordinate = coord;
			range = size;
		}

		/// <summary>
		/// Get the coordinate of the left or bottom edge of the alignment box. </summary>
		/// <returns> the x or y coordinate of the box. </returns>
		public float Coordinate => coordinate;

	    /// <summary>
		/// Get the width or height of the alignment box. </summary>
		/// <returns> the size of the box. </returns>
		public float Range => range;

	    public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, coordinate, range);
		}



		public override bool Equals(object @object)
		{
			bool result;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is AlignmentZone)
			{


				AlignmentZone zone = (AlignmentZone) @object;
				result = (coordinate == zone.coordinate) && (range == zone.range);
			}
			else
			{
				result = false;
			}

			return result;
		}

		public override int GetHashCode()
		{
			return (Float.floatToIntBits(coordinate) * Constants.PRIME) + Float.floatToIntBits(range);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 4;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeHalf(coordinate);
			coder.writeHalf(range);
		}
	}

}