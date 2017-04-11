using System;
using com.flagstone.transform.coder;

/*
 * Bounds.java
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
namespace com.flagstone.transform.datatype
{
    /// <summary>
	/// <para>
	/// The Bounds class defines the area inside which shapes, text fields and
	/// characters are drawn.
	/// </para>
	/// 
	/// <para>
	/// In Flash the axes are specified relative to the top left corner of the screen
	/// and the bounding area is defined by two pairs of coordinates that identify
	/// the top left and bottom right corners of a rectangle.
	/// </para>
	/// 
	/// <img src="doc-files/bounds.gif">
	/// 
	/// <para>
	/// The coordinates for each corner also specify the coordinate range so
	/// specifying a bounding rectangle with the points (-100,-100) and (100,100)
	/// defines a rectangle 200 twips by 200 twips with the point (0,0) located in
	/// the centre. Specifying the points (0,0) and (200,200) defines a rectangle
	/// with the same size however the centre is now located at (100,100).
	/// </para>
	/// 
	/// <para>
	/// The bounding rectangle does not clip the object when it is drawn. Lines and
	/// curves drawn outside of the rectangle will still be displayed. However if the
	/// position of the object is changed or another object is displayed in front of
	/// it then only the pixels inside of the bounding box will be repainted.
	/// </para>
	/// </summary>
	public sealed class Bounds : SWFEncodeable
	{
		/// <summary>
		/// Offset to add to number of bits when calculating number of bytes. </summary>
		private const int ROUND_TO_BYTES = 7;
		/// <summary>
		/// Right shift to convert number of bits to number of bytes. </summary>
		private const int BITS_TO_BYTES = 3;
		/// <summary>
		/// Format used by toString() to display object representation. </summary>
		private const string FORMAT = "Bounds: (%d, %d) (%d, %d)";

		/// <summary>
		/// Size of bit-field used to specify the number of bits representing
		/// encoded bounding box values.
		/// </summary>
		private const int FIELD_SIZE = 5;

		/// <summary>
		/// Create a Bounds by applying a padding factor to all sides of the
		/// bounding box.
		/// </summary>
		/// <param name="rect"> the Bounds to adjust. </param>
		/// <param name="padding"> the margin to add to the coordinates of the bounds. </param>
		/// <returns> the adjusted Bounds. </returns>


		public static Bounds pad(Bounds rect, int padding)
		{
			return new Bounds(rect.MinX - padding, rect.MinY - padding, rect.MaxX + padding, rect.MaxY + padding);
		}

		/// <summary>
		/// Create a Bounds by applying a padding factor to all sides of the
		/// bounding box.
		/// </summary>
		/// <param name="rect"> the Bounds to adjust. </param>
		/// <param name="top"> the to apply to the top of the bounding box. </param>
		/// <param name="right"> the to apply to the right of the bounding box. </param>
		/// <param name="bottom"> the to apply to the bottom of the bounding box. </param>
		/// <param name="left"> the to apply to the left of the bounding box. </param>
		/// <returns> the adjusted Bounds. </returns>


		public static Bounds pad(Bounds rect, int top, int right, int bottom, int left)
		{
			return new Bounds(rect.MinX - left, rect.MinY - top, rect.MaxX + right, rect.MaxY + bottom);
		}

		/// <summary>
		/// X-coordinate of upper left corner of bounding box. </summary>
		
		private readonly int minX;
		/// <summary>
		/// Y-coordinate of upper left corner of bounding box. </summary>
		
		private readonly int minY;
		/// <summary>
		/// X-coordinate of lower right corner of bounding box. </summary>
		
		private readonly int maxX;
		/// <summary>
		/// Y-coordinate of lower right corner of bounding box. </summary>
		
		private readonly int maxY;

		/// <summary>
		/// Holds the field size for bounding box values when encoding and
		/// decoding objects.
		/// </summary>
		
		private int size;

		/// <summary>
		/// Creates and initialises a Bounds using values encoded in the Flash binary
		/// format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Bounds(SWFDecoder coder)
		{
			size = coder.readBits(FIELD_SIZE, false);
			minX = coder.readBits(size, true);
			maxX = coder.readBits(size, true);
			minY = coder.readBits(size, true);
			maxY = coder.readBits(size, true);
			coder.alignToByte();
		}

		/// <summary>
		/// Creates a Bounds object representing a rectangle with the corners at
		/// (xmin,ymin) and (xmax,ymax).
		/// </summary>
		/// <param name="xmin">
		///            x-coordinate of the top left corner. </param>
		/// <param name="ymin">
		///            y-coordinate of the top left corner. </param>
		/// <param name="xmax">
		///            x-coordinate of bottom right corner. </param>
		/// <param name="ymax">
		///            y-coordinate of bottom right corner. </param>


		public Bounds(int xmin, int ymin, int xmax, int ymax)
		{
			minX = xmin;
			minY = ymin;
			maxX = xmax;
			maxY = ymax;
		}

		/// <summary>
		/// Returns the x-coordinate of the top left corner of the bounding rectangle
		/// as seen on a screen.
		/// </summary>
		/// <returns> the x-coordinate of the upper left corner. </returns>
		public int MinX => minX;

	    /// <summary>
		/// Returns the x-coordinate of the bottom right corner of the bounding
		/// rectangle as seen on a screen.
		/// </summary>
		/// <returns> the x-coordinate of the lower right corner. </returns>
		public int MaxX => maxX;

	    /// <summary>
		/// Returns the y-coordinate of the top left corner of the bounding rectangle
		/// as seen on a screen.
		/// </summary>
		/// <returns> the y-coordinate of the upper left corner. </returns>
		public int MinY => minY;

	    /// <summary>
		/// Returns the y-coordinate of the bottom right corner of the bounding
		/// rectangle as seen on a screen.
		/// </summary>
		/// <returns> the y-coordinate of the lower right corner. </returns>
		public int MaxY => maxY;

	    /// <summary>
		/// Returns the width of the rectangle, measured in twips.
		/// </summary>
		/// <returns> the width of the bounding box in twips. </returns>
		public int Width => maxX - minX;

	    /// <summary>
		/// Returns the height of the rectangle, measured in twips.
		/// </summary>
		/// <returns> the height of the bounding box in twips. </returns>
		public int Height => maxY - minY;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, minX, minY, maxX, maxY);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public override bool Equals(object @object)
		{
			bool result;
			Bounds bounds;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is Bounds)
			{
				bounds = (Bounds) @object;
				result = (minX == bounds.minX) && (minY == bounds.minY) && (maxX == bounds.maxX) && (maxY == bounds.maxY);
			}
			else
			{
				result = false;
			}
			return result;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override int GetHashCode()
		{
			return ((minX * Constants.PRIME + minY) * Constants.PRIME + maxX) * Constants.PRIME + maxY;
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			size = Coder.maxSize(minX, minY, maxX, maxY);
			return (FIELD_SIZE + ROUND_TO_BYTES + (size << 2)) >> BITS_TO_BYTES;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeBits(size, FIELD_SIZE);
			coder.writeBits(minX, size);
			coder.writeBits(maxX, size);
			coder.writeBits(minY, size);
			coder.writeBits(maxY, size);
			coder.alignToByte();
		}
	}

}