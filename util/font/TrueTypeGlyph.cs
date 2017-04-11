using System;
using com.flagstone.transform.datatype;
using com.flagstone.transform.shape;

/*
 * Glyph.java
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

namespace com.flagstone.transform.util.font
{
    /// <summary>
	/// TrueTypeGlyph is a simple container class used to decode TrueType glyphs.
	/// </summary>
	public sealed class TrueTypeGlyph : Glyph
	{

		/// <summary>
		/// The set of x coordinates representing a segment of a glyph. </summary>
		
		private int[] xCoordinates = {};
		/// <summary>
		/// The set of y coordinates representing a segment of a glyph. </summary>
		
		private int[] yCoordinates = {};
		/// <summary>
		/// Flags indicating which point in on the segment. </summary>
		
		private bool[] onCurve = {};
		/// <summary>
		/// The set of end points for each point on the segment. </summary>
		
		private int[] endPoints = {};

		/// <summary>
		/// Create a TrueTypeGlyph with the specified outline, bounding box and
		/// advance. </summary>
		/// <param name="aShape"> the outline of the glyph. </param>
		/// <param name="rect"> the bounding box that encloses the glyph. </param>
		/// <param name="dist"> the advance to the next glyph. </param>


		public TrueTypeGlyph(Shape aShape, Bounds rect, int dist) : base(aShape, rect, dist)
		{
		}

		/// <summary>
		/// Create a TrueTypeGlyph with the specified outline - the bounding box
		/// and advance default to zero. </summary>
		/// <param name="aShape"> the outline of the glyph. </param>


		public TrueTypeGlyph(Shape aShape) : base(aShape)
		{
		}

		/// <summary>
		/// Get the set of x coordinates representing a segment of a glyph. </summary>
		/// <param name="array"> an array where the points will be stored. </param>


		public void getXCoordinates(int[] array)
		{
			Array.Copy(xCoordinates, 0, array, 0, xCoordinates.Length);
		}

		/// <summary>
		/// Get the set of y coordinates representing a segment of a glyph. </summary>
		/// <param name="array"> an array where the points will be stored. </param>


		public void getYCoordinates(int[] array)
		{
			Array.Copy(yCoordinates, 0, array, 0, yCoordinates.Length);
		}

		/// <summary>
		/// Get the set of end points for the segment of a glyph. </summary>
		/// <param name="array"> an array where the points will be stored. </param>


		public void getEnd(int[] array)
		{
			Array.Copy(endPoints, 0, array, 0, endPoints.Length);
		}

		/// <summary>
		/// Get the set of flags which indicate which point are on the segment. </summary>
		/// <param name="array"> an array where the flags will be stored. </param>


		public void getCurve(bool[] array)
		{
			Array.Copy(onCurve, 0, array, 0, onCurve.Length);
		}

		/// <summary>
		/// Set the points for a segment of a glyph. </summary>
		/// <param name="xcoords"> the set of x-coordinates for the points. </param>
		/// <param name="ycoords"> the set of y-coordinates for the points. </param>


		public void setCoordinates(int[] xcoords, int[] ycoords)
		{
			xCoordinates = Arrays.copyOf(xcoords, xcoords.Length);
			yCoordinates = Arrays.copyOf(ycoords, ycoords.Length);
		}

		/// <summary>
		/// Indicate which points are on the segment. </summary>
		/// <param name="array"> an array where the flags will be stored. </param>


		public bool[] OnCurve
		{
			set => onCurve = Arrays.copyOf(value, value.Length);
		}

		/// <summary>
		/// Set the end-points for a segment of a glyph. </summary>
		/// <param name="array"> the set of end points. </param>


		public int[] Ends
		{
			set => endPoints = Arrays.copyOf(value, value.Length);
		}

		/// <summary>
		/// Get the number of points in the segment. </summary>
		/// <returns> the number of points. </returns>
		public int numberOfPoints()
		{
			return xCoordinates.Length;
		}

		/// <summary>
		/// Get the number of contours. </summary>
		/// <returns> the number of contours. </returns>
		public int numberOfContours()
		{
			return endPoints.Length;
		}
	}

}