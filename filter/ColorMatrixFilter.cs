using System;
using System.IO;
using System.Linq;
using com.flagstone.transform.coder;

/*
 * ColorMatrixFilter.java
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
	/// ColorMatrixFilter is used to apply a colour transform to the pixels of an
	/// object on the display list.
	/// </summary>
	public sealed class ColorMatrixFilter : Filter
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "ColorMatrix: { matrix=[" + "[%f %f %f %f %f];[%f %f %f %f %f];" + "[%f %f %f %f %f];[%f %f %f %f %f];]}";

		/// <summary>
		/// The number of elements in the colour matrix. </summary>
		private const int MATRIX_SIZE = 20;
		/// <summary>
		/// The colour matrix. </summary>
		
		private readonly float[] matrix;

		/// <summary>
		/// Creates and initialises a ColorMatrix object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public ColorMatrixFilter(SWFDecoder coder)
		{
			matrix = new float[MATRIX_SIZE];
			for (int i = 0; i < MATRIX_SIZE; i++)
			{
				matrix[i] = Float.intBitsToFloat(coder.readInt());
			}
		}

		/// <summary>
		/// Create a ColorMatrixFilter with the specified matrix.
		/// </summary>
		/// <param name="aMatrix"> a matrix that will be applied to each pixel of the
		/// object. </param>


		public ColorMatrixFilter(float[] aMatrix)
		{
			if ((aMatrix == null) || (aMatrix.Length != MATRIX_SIZE))
			{
				throw new ArgumentException("Value not set");
			}
			matrix = Arrays.copyOf(aMatrix, aMatrix.Length);
		}

		/// <summary>
		/// Get the colour matrix.
		/// </summary>
		/// <returns> the 5x4 matrix that will be applied to the object. </returns>
		public float[] Matrix => Arrays.copyOf(matrix, matrix.Length);

	    public override string ToString()
		{
			// CHECKSTYLE:OFF
			return ObjectExtensions.FormatJava(FORMAT, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5], matrix[6], matrix[7], matrix[8], matrix[9], matrix[10], matrix[11], matrix[12], matrix[13], matrix[14], matrix[15], matrix[16], matrix[17], matrix[18], matrix[19]);
			// CHECKSTYLE:ON
		}



		public override bool Equals(object @object)
		{
			bool result;
			ColorMatrixFilter filter;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is ColorMatrixFilter)
			{
				filter = (ColorMatrixFilter) @object;
				result = matrix.SequenceEqual(filter.matrix);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return Arrays.GetHashCode(matrix);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 81;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(FilterTypes.COLOR_MATRIX);
			foreach (float value in matrix)
			{
				coder.writeInt(Float.floatToIntBits(value));
			}
		}
	}

}