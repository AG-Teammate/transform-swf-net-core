/*
 * MorphGradient.java
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

namespace com.flagstone.transform.fillstyle
{
    /// <summary>
	/// MorphGradient defines the control points that is used to specify how a
	/// gradient fill is displayed at the start and end of the shape morphing
	/// process.
	/// 
	/// <para>
	/// The ratio is a number between 0 and 255 - that specifies the relative
	/// location in the square. For Linear Gradient Fills a ratio of zero is mapped
	/// to the left side of the gradient square and 255 is mapped to the right side
	/// of the square. For Radial Gradient Fills a ratio of zero is mapped to the
	/// centre of the gradient square and 255 is mapped to the edge of the largest
	/// circle that fits inside the gradient square. The color is the colour to be
	/// displayed at the point identified by the ratio.
	/// </para>
	/// 
	/// <para>
	/// The MorphGradient defines ratios and colours for the start and end of the
	/// morphing process, the Flash Player performs the interpolation between the two
	/// values as the shape is morphed.
	/// </para>
	/// </summary>
	/// <seealso cref= Gradient </seealso>
	/// <seealso cref= GradientFill </seealso>
	/// <seealso cref= MorphGradientFill </seealso>
	public sealed class MorphGradient : SWFEncodeable
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "MorphGradient: { start=%s; end=%s}";
		/// <summary>
		/// The gradient at the start of the morphing process. </summary>
		private Gradient start;
		/// <summary>
		/// The gradient at the end of the morphing process. </summary>
		private Gradient end;

		/// <summary>
		/// Creates and initialises a MorphGradient object using values encoded
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



		public MorphGradient(SWFDecoder coder, Context context)
		{
			start = new Gradient(coder, context);
			end = new Gradient(coder, context);
		}

		/// <summary>
		/// Creates a MorphGradient object specifying the starting and ending
		/// gradients.
		/// </summary>
		/// <param name="startGradient">
		///            the Gradient containing the initial ratio and colour. </param>
		/// <param name="endGradient">
		///            the Gradient containing the final ratio and colour. </param>


		public MorphGradient(Gradient startGradient, Gradient endGradient)
		{
			Start = startGradient;
			End = endGradient;
		}

		/// <summary>
		/// Creates and initialises a MorphGradient object using the values copied
		/// from another MorphGradient object.
		/// </summary>
		/// <param name="object">
		///            a MorphGradient object from which the values will be
		///            copied. </param>


		public MorphGradient(MorphGradient @object)
		{
			start = @object.start;
			end = @object.end;
		}

		/// <summary>
		/// Get the gradient containing the ratio and colour at the start of the
		/// morphing process.
		/// </summary>
		/// <returns> the starting gradient. </returns>
		public Gradient Start
		{
			get => start;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				start = value;
			}
		}

		/// <summary>
		/// Get the gradient containing the ratio and colour at the end of the
		/// morphing process.
		/// </summary>
		/// <returns> the final gradient. </returns>
		public Gradient End
		{
			get => end;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				end = value;
			}
		}



		/// <summary>
		/// {@inheritDoc} </summary>
		public MorphGradient copy()
		{
			return new MorphGradient(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, start.ToString(), end.ToString());
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			int length = start.prepareToEncode(context);
			length += end.prepareToEncode(context);

			return length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			start.encode(coder, context);
			end.encode(coder, context);
		}
	}

}