using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * BlurFilter.java
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

namespace com.flagstone.transform.filter
{
    /// <summary>
	/// BlurFilter is used to apply a Box filter to the pixel of an object on the
	/// display list.
	/// </summary>
	public sealed class BlurFilter : Filter
	{
		/// <summary>
		/// Maximum number of passes to blur an object. </summary>
		private const int MAX_BLUR_COUNT = 31;

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "BlurFilter: { blurX=%f; blurY=%f;" + " passes=%d}";

		/// <summary>
		/// The horizontal blur amount. </summary>
		
		private readonly int blurX;
		/// <summary>
		/// The vertical blur amount. </summary>
		
		private readonly int blurY;
		/// <summary>
		/// The number of blur passes. </summary>
		
		private readonly int passes;

		/// <summary>
		/// Creates and initialises a BlueFilter object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public BlurFilter(SWFDecoder coder)
		{
			blurX = coder.readInt();
			blurY = coder.readInt();
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			passes = (int)((uint)coder.readByte() >> 3);
		}

		/// <summary>
		/// Create a BlurFilter with the horizontal and vertical blur values and the
		/// number of passes.
		/// </summary>
		/// <param name="xBlur"> the blur amount in the x-direction. </param>
		/// <param name="yBlur"> the blue amount in the y-direction. </param>
		/// <param name="count"> the number of passes to apply. </param>


		public BlurFilter(float xBlur, float yBlur, int count)
		{
			blurX = (int)(xBlur * Coder.SCALE_16);
			blurY = (int)(yBlur * Coder.SCALE_16);

			if ((count < 0) || (count > MAX_BLUR_COUNT))
			{
				throw new IllegalArgumentRangeException(0, MAX_BLUR_COUNT, count);
			}
			passes = count;
		}

		/// <summary>
		/// Get the blur amount in the x-direction. </summary>
		/// <returns> the horizontal blur amount. </returns>
		public float BlurX => blurX / Coder.SCALE_16;

	    /// <summary>
		/// Get the blur amount in the y-direction. </summary>
		/// <returns> the vertical blur amount. </returns>
		public float BlurY => blurY / Coder.SCALE_16;

	    /// <summary>
		/// Get the number of passes. </summary>
		/// <returns> the times the blurring is applied. </returns>
		public int Passes => passes;

	    public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, BlurX, BlurY, passes);
		}



		public override bool Equals(object @object)
		{
			bool result;
			BlurFilter filter;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is BlurFilter)
			{
				filter = (BlurFilter) @object;
				result = (blurX == filter.blurX) && (blurY == filter.blurY) && (passes == filter.passes);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return ((blurX * Constants.PRIME) + blurY) * Constants.PRIME + passes;
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 10;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(FilterTypes.BLUR);
			coder.writeInt(blurX);
			coder.writeInt(blurY);
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			coder.writeByte(passes << 3);

		}
	}

}