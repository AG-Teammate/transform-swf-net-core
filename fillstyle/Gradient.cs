using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;

/*
 * Gradient.java
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

namespace com.flagstone.transform.fillstyle
{
    /// <summary>
	/// Gradient defines a control point that is used to specify how a gradient
	/// colour is displayed.
	/// 
	/// <para>
	/// Two or more control points are used to define how the colour changes across
	/// the gradient square. Each control point specifies the ratio indicating the
	/// location of the control point across the gradient square and the colour to be
	/// displayed at that point.
	/// </para>
	/// 
	/// <para>
	/// The ratio is a number between 0 and 255 - that specifies the relative
	/// location in the square. For Linear Gradient Fills a ratio of zero is mapped
	/// to the left side of the gradient square and 255 is mapped to the right side
	/// of the square. For Radial Gradient Fills a ratio of zero is mapped to the
	/// centre of the gradient square and 255 is mapped to the edge of the largest
	/// circle that fits inside the gradient square. A ratio is used rather than
	/// specifying coordinates within the gradient square as the coordinate space is
	/// transformed to fit the shape that the gradient is being displayed in.
	/// </para>
	/// 
	/// <para>
	/// Note that the object used to create the shape definition determines whether
	/// the alpha channel is encoded in the gradient colours. Simply specifying the
	/// level of transparency in the Color object is not sufficient.
	/// </para>
	/// </summary>
	/// <seealso cref= GradientFill </seealso>
	public sealed class Gradient : SWFEncodeable
	{

		/// <summary>
		/// Maximum number of gradient records. </summary>
		public const int MAX_GRADIENTS = 15;
		/// <summary>
		/// Maximum ratio along the gradient square, range 0..255. </summary>
		private const int MAX_RATIO = 255;

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Gradient: { ratio=%d; color=%s}";
		/// <summary>
		/// The ratio across the gradient square. </summary>
		
		private readonly int ratio;
		/// <summary>
		/// The colour for gradient. </summary>
		
		private readonly Color color;

		/// <summary>
		/// Creates and initialises a Gradient object using values encoded
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



		public Gradient(SWFDecoder coder, Context context)
		{
			ratio = coder.readByte();
			color = new Color(coder, context);
		}

		/// <summary>
		/// Creates a Gradient object with the specified ratio and color.
		/// </summary>
		/// <param name="aRatio">
		///            the ratio along the gradient square. Must be in the range
		///            0..255. </param>
		/// <param name="aColor">
		///            the color at the control point. Must not be null. </param>


		public Gradient(int aRatio, Color aColor)
		{
			if ((aRatio < 0) || (aRatio > MAX_RATIO))
			{
				throw new IllegalArgumentRangeException(0, MAX_RATIO, aRatio);
			}
			ratio = aRatio;

			if (aColor == null)
			{
				throw new ArgumentException();
			}
			color = aColor;
		}

		/// <summary>
		/// Get the ratio that defines the relative point across the gradient
		/// square.
		/// </summary>
		/// <returns> the ratio for the gradient in the range 0..255. </returns>
		public int Ratio => ratio;

	    /// <summary>
		/// Get the colour that is displayed at the control point across the
		/// gradient square defined by the ratio.
		/// </summary>
		/// <returns> the colour for the gradient at the ratio point. </returns>
		public Color Color => color;

	    public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, ratio, color);
		}



		public override bool Equals(object @object)
		{
			bool result;
			Gradient gradient;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is Gradient)
			{
				gradient = (Gradient) @object;
				result = (ratio == gradient.ratio) && color.Equals(gradient.color);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return (ratio * Constants.PRIME) + color.GetHashCode();
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			return 1 + color.prepareToEncode(context);
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ratio);
			color.encode(coder, context);
		}
	}

}