using System;
using System.Text;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;

/*
 * ConvolutionFilter.java
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
	/// ConvolutionFilter is used to apply a two-dimensional discrete convolution on
	/// the pixels of the object on the display list.
	/// </summary>
	public sealed class ConvolutionFilter : Filter
	{

		/// <summary>
		/// Builder for creating ConvolutionFilter objects.
		/// </summary>
		public sealed class Builder
		{
			/// <summary>
			/// The convolution matrix. </summary>
			
			internal float[][] matrix;
			/// <summary>
			/// The divisor for the convolution equation. </summary>
			
			internal float divisor;
			/// <summary>
			/// The bias for the convolution equation. </summary>
			
			internal float bias;
			/// <summary>
			/// The default colour used for pixels outside the display object. </summary>
			
			internal Color color;
			/// <summary>
			/// Whether outside pixels are clamped to the nearest inside one. </summary>
			
			internal bool clamp;
			/// <summary>
			/// Whether transparency is preserved. </summary>
			
			internal bool alpha;

			/// <summary>
			/// Set the matrix used for the convolution.
			/// </summary>
			/// <param name="aMatrix"> a 2D matrix. </param>
			/// <returns> this Builder. </returns>


			public Builder setMatrix(float[][] aMatrix)
			{


				int rows = aMatrix.Length;


				int cols = aMatrix[0].Length;


				matrix = RectangularArrays.ReturnRectangularFloatArray(rows, cols);
				for (int i = 0; i < rows; i++)
				{
					for (int j = 0; j < cols; j++)
					{
						matrix[i][j] = aMatrix[i][j];
					}
				}
				return this;
			}

			/// <summary>
			/// Set the divisor for the convolution. </summary>
			/// <param name="value"> the divisor </param>
			/// <returns> this Builder. </returns>


			public Builder setDivisor(float value)
			{
				divisor = value;
				return this;
			}

			/// <summary>
			/// Set the bias for the convolution. </summary>
			/// <param name="value"> the bias </param>
			/// <returns> this Builder. </returns>


			public Builder setBias(float value)
			{
				bias = value;
				return this;
			}

			/// <summary>
			/// Set the default colour applied to the pixels outside of the image. </summary>
			/// <param name="aColor"> the default colour. </param>
			/// <returns> this Builder. </returns>


			public Builder setColor(Color aColor)
			{
				color = aColor;
				return this;
			}

			/// <summary>
			/// Indicate whether the pixels outside the image will be clamped to the
			/// nearest pixel value (true) or to the default colour (false). </summary>
			/// <param name="nearest"> if true clamp to the nearest pixel, false use the
			/// default colour. </param>
			/// <returns> this Builder. </returns>


			public Builder setClamp(bool nearest)
			{
				clamp = nearest;
				return this;
			}

			/// <summary>
			/// Indicate whether the alpha value of the pixels should be preserved. </summary>
			/// <param name="preserve"> if true preserve the alpha values. </param>
			/// <returns> this Builder. </returns>


			public Builder setAlpha(bool preserve)
			{
				alpha = preserve;
				return this;
			}

			/// <summary>
			/// Generate an instance of ConvolutionFilter using the parameters set
			/// in the Builder. </summary>
			/// <returns> a ConvolutionFilter object. </returns>
			public ConvolutionFilter build()
			{
				return new ConvolutionFilter(this);
			}
		}

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "ConvolutionFilter: { matrix=%s;" + " divisor=%f; bias=%f; color=%s; clamp=%b; alpha=%b}";

		/// <summary>
		/// The convolution matrix. </summary>
		
		private readonly float[][] matrix;
		/// <summary>
		/// The divisor for the convolution equation. </summary>
		
		private readonly float divisor;
		/// <summary>
		/// The bias for the convolution equation. </summary>
		
		private readonly float bias;
		/// <summary>
		/// The default colour used for pixels outside the display object. </summary>
		
		private readonly Color color;
		/// <summary>
		/// Whether outside pixels are clamped to the nearest inside one. </summary>
		
		private readonly bool clamp;
		/// <summary>
		/// Whether transparency is preserved. </summary>
		
		private readonly bool alpha;

		/// <summary>
		/// The number of rows in the matrix. </summary>
		
		private int rows;
		/// <summary>
		/// The number of columns in the matrix. </summary>
		
		private int cols;

		/// <summary>
		/// Create a new ConvolutionFilter object using the parameters defined in
		/// the Builder. </summary>
		/// <param name="builder"> a Builder containing the parameters for the instance. </param>


		public ConvolutionFilter(Builder builder)
		{
			matrix = builder.matrix;
			divisor = builder.divisor;
			bias = builder.bias;
			color = builder.color;
			clamp = builder.clamp;
			alpha = builder.alpha;
		}

		/// <summary>
		/// Creates and initialises a ConvolutionFilter object using values encoded
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



		public ConvolutionFilter(SWFDecoder coder, Context context)
		{
			cols = coder.readByte();
			rows = coder.readByte();
			divisor = Float.intBitsToFloat(coder.readInt());
			bias = Float.intBitsToFloat(coder.readInt());


			matrix = RectangularArrays.ReturnRectangularFloatArray(rows, cols);
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					matrix[i][j] = Float.intBitsToFloat(coder.readInt());
				}
			}
			color = new Color(coder, context);


			int bits = coder.readByte();
			clamp = (bits & Coder.BIT1) != 0;
			alpha = (bits & Coder.BIT0) != 0;
		}

		/// <summary>
		/// Get the divisor for the convolution equation. </summary>
		/// <returns> the divisor value. </returns>
		public float Divisor => divisor;

	    /// <summary>
		/// Get the bias for the convolution equation. </summary>
		/// <returns> the bias value. </returns>
		public float Bias => divisor;

	    /// <summary>
		/// Get a copy of the convolution matrix. </summary>
		/// <returns> a copy of the matrix. </returns>
		public float[][] Matrix => throw new NotImplementedException();

	    /// <summary>
		/// Get the default colour used for pixel outside the display object. </summary>
		/// <returns> the colour used for outside pixels. </returns>
		public Color Color => color;

	    /// <summary>
		/// Are outside pixels clamped to the nearest inside one (true) or to the
		/// default colour (false). </summary>
		/// <returns> true if the pixels are clamped, false if the default colour is
		/// used. </returns>
		public bool Clamp => clamp;

	    /// <summary>
		/// Is the alpha value of the pixels in the display list object preserved
		/// in the output of the convolution. </summary>
		/// <returns> true if alpha is preserved, false if not. </returns>
		public bool Alpha => alpha;

	    public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("[");
			for (int i = 0; i < matrix.Length; i++)
			{
				builder.Append(Arrays.ToString(matrix[i]));
			}
			builder.Append("]");

			return ObjectExtensions.FormatJava(FORMAT, builder.ToString(), divisor, bias, color, clamp, alpha);
		}



		public override bool Equals(object @object)
		{
			bool result;
			ConvolutionFilter filter;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is ConvolutionFilter)
			{
                filter = (ConvolutionFilter) @object;
				result = Arrays.deepEquals(matrix, filter.matrix) && (divisor == filter.divisor) && (bias == filter.bias) && color.Equals(filter.color) && (clamp == filter.clamp) && (alpha == filter.alpha);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
		    return ((((Arrays.deepHashCode(matrix) * Constants.PRIME + Float.floatToIntBits(divisor)) * Constants.PRIME + Float.floatToIntBits(bias)) * Constants.PRIME + color.GetHashCode()) * Constants.PRIME + Convert.ToBoolean(clamp).GetHashCode()) * Constants.PRIME + Convert.ToBoolean(alpha).GetHashCode();
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			rows = matrix.Length;
			cols = matrix[0].Length;
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 16 + rows * cols * 4;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(FilterTypes.CONVOLUTION);
			coder.writeByte(cols);
			coder.writeByte(rows);
			coder.writeInt(Float.floatToIntBits(divisor));
			coder.writeInt(Float.floatToIntBits(bias));
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < cols; j++)
				{
					coder.writeInt(Float.floatToIntBits(matrix[i][j]));
				}
			}
			color.encode(coder, context);
			int bits = 0;
			bits |= clamp ? Coder.BIT1 : 0;
			bits |= alpha ? Coder.BIT0 : 0;
			coder.writeByte(bits);
		}
	}

}