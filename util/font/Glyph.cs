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

using com.flagstone.transform.datatype;
using com.flagstone.transform.shape;

namespace com.flagstone.transform.util.font
{
    /// <summary>
	/// Glyph is a simple container class used by Font to record the shape, bounding
	/// box and advance for a glyph.
	/// </summary>
	public class Glyph
	{
		/// <summary>
		/// The outline of the glyph. </summary>
		private Shape shape;
		/// <summary>
		/// The bounding box that encloses the glyph. </summary>
		private Bounds bounds;
		/// <summary>
		/// The distance in twips to the next glyph. </summary>
		private int advance;

		/// <summary>
		/// Create a Glyph with the specified outline, bounding box and advance. </summary>
		/// <param name="aShape"> the outline of the glyph. </param>
		/// <param name="rect"> the bounding box that encloses the glyph. </param>
		/// <param name="dist"> the advance to the next glyph. </param>


		public Glyph(Shape aShape, Bounds rect, int dist)
		{
			shape = aShape;
			bounds = rect;
			advance = dist;
		}

		/// <summary>
		/// Create a Glyph with the specified outline - the bounding box and advance
		/// default to zero. </summary>
		/// <param name="aShape"> the outline of the glyph. </param>


		public Glyph(Shape aShape)
		{
			shape = aShape;
		}

	   /// <summary>
	   /// Get the outline of the glyph.
	   /// </summary>
	   /// <returns> the Shape used to trace the outline of the glyph. </returns>
		public Shape Shape
		{
			get => shape;
	       set => shape = value;
	   }


		/// <summary>
		/// Get the bounding box enclosing the glyph. </summary>
		/// <returns> the Bounds that encloses the outline of the glyph. </returns>
		public Bounds Bounds
		{
			get => bounds;
		    set => bounds = value;
		}


		/// <summary>
		/// Get the distance to the next glyph. </summary>
		/// <returns> the distance in twips from this glyph to the next. </returns>
		public int Advance
		{
			get => advance;
		    set => advance = value;
		}

	}

}