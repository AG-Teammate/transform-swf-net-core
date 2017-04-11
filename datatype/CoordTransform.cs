using System;
using com.flagstone.transform.coder;

/*
 * CoordTransform.java
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
	/// CoordTransform is used to specify a two-dimensional coordinate transform
	/// which allows an object to be scaled, rotated or moved without changing the
	/// original definition of how the object is drawn.
	/// 
	/// <para>
	/// A two-dimensional transform is defined using a 3x3 matrix and the new values
	/// for a pair of coordinates (x,y) are calculated using the following matrix
	/// multiplication:
	/// </para>
	/// 
	/// <img src="doc-files/transform.gif">
	/// 
	/// <para>
	/// Different transformations such as scaling, rotation, shearing and translation
	/// can be performed using the above matrix multiplication. More complex
	/// transformations can be defined by performing successive matrix
	/// multiplications in a process known as compositing. This allows a complex
	/// transformations to performed on an object. However not that compositing
	/// transforms is not commutative, the order in which transformations are applied
	/// will affect the final result.
	/// </para>
	/// </summary>
	public sealed class CoordTransform : SWFEncodeable
	{
		/// <summary>
		/// Offset to add to number of bits when calculating number of bytes. </summary>
		private const int ROUND_TO_BYTES = 7;
		/// <summary>
		/// Right shift to convert number of bits to number of bytes. </summary>
		private const int BITS_TO_BYTES = 3;
		/// <summary>
		/// Format used by toString() to display object representation. </summary>
		private const string FORMAT = "CoordTransform: { scaleX=%f;" + " scaleY=%f; shearX=%f; shearY=%f; transX=%d; transY=%d}";

		/// <summary>
		/// The default value used for the scaling terms when a translation or
		/// shearing transform is created.
		/// </summary>
		public const float DEFAULT_SCALE = 1.0f;
		/// <summary>
		/// The default value used for the shearing terms when a translation or
		/// scaling transform is created.
		/// </summary>
		public const float DEFAULT_SHEAR = 0.0f;
		/// <summary>
		/// The default value used for the translation terms when a scaling or
		/// shearing transform is created.
		/// </summary>
		public const int DEFAULT_COORD = 0;
		/// <summary>
		/// The factor applied to real numbers used for scaling terms when storing
		/// them as fixed point values.
		/// </summary>
		public const float SCALE_FACTOR = 65536.0f;
		/// <summary>
		/// The factor applied to real numbers used for shearing terms when storing
		/// them as fixed point values.
		/// </summary>
		public const float SHEAR_FACTOR = 65536.0f;

		/// <summary>
		/// Size of bit-field used to specify the number of bits representing
		/// encoded transform values.
		/// </summary>
		private const int FIELD_SIZE = 5;
		/// <summary>
		/// Default value for scaling terms. </summary>
		private const int DEFAULT_INT_SCALE = 65536;
		/// <summary>
		/// Default value for shearing terms. </summary>
		private const int DEFAULT_INT_SHEAR = 0;

		/// <summary>
		/// Create a new coordinate transform by multiplying two matrices together to
		/// calculate the product. Since matrix multiplication is not commutative the
		/// order in which the arguments are passed is important.
		/// </summary>
		/// <param name="left">
		///            a 3x3 matrix </param>
		/// <param name="right">
		///            a 3x3 matrix </param>
		/// <returns> a new 3x3 matrix contains the product of the two arguments. </returns>


		public static float[][] product(float[][] left, float[][] right)
		{
			return new[]
			{
				new[] {left[0][0] * right[0][0] + left[0][1] * right[1][0] + left[0][2] * right[2][0], left[0][0] * right[0][1] + left[0][1] * right[1][1] + left[0][2] * right[2][1], left[0][0] * right[0][2] + left[0][1] * right[1][2] + left[0][2] * right[2][2]},
				new[] {left[1][0] * right[0][0] + left[1][1] * right[1][0] + left[1][2] * right[2][0], left[1][0] * right[0][1] + left[1][1] * right[1][1] + left[1][2] * right[2][1], left[1][0] * right[0][2] + left[1][1] * right[1][2] + left[1][2] * right[2][2]},
				new[] {left[2][0] * right[0][0] + left[2][1] * right[1][0] + left[2][2] * right[2][0], left[2][0] * right[0][1] + left[2][1] * right[1][1] + left[2][2] * right[2][1], left[2][0] * right[0][2] + left[2][1] * right[1][2] + left[2][2] * right[2][2]}
			};
		}

		/// <summary>
		/// Create a translation transform.
		/// </summary>
		/// <param name="xCoord">
		///            the x coordinate of the transformation. </param>
		/// <param name="yCoord">
		///            the y coordinate of the transformation. </param>
		/// <returns> a CoordTransform containing the translation. </returns>


		public static CoordTransform translate(int xCoord, int yCoord)
		{
			return new CoordTransform(1.0f, 1.0f, 0.0f, 0.0f, xCoord, yCoord);
		}

		/// <summary>
		/// Create a scaling transform.
		/// </summary>
		/// <param name="xScale">
		///            the scaling factor along the x-axis. </param>
		/// <param name="yScale">
		///            the scaling factor along the y-axis </param>
		/// <returns> a CoordTransform containing the scaling transform. </returns>


		public static CoordTransform scale(float xScale, float yScale)
		{
			return new CoordTransform(xScale, yScale, 0.0f, 0.0f, 0, 0);
		}

		/// <summary>
		/// Create a CoordTransform initialised for a shearing operation.
		/// </summary>
		/// <param name="xShear">
		///            the shearing factor along the x-axis. </param>
		/// <param name="yShear">
		///            the shearing factor along the y-axis </param>
		/// <returns> a CoordTransform containing the shearing transform. </returns>


		public static CoordTransform shear(float xShear, float yShear)
		{
			return new CoordTransform(1.0f, 1.0f, xShear, yShear, 0, 0);
		}

		/// <summary>
		/// Create a CoordTransform initialised for a rotation in degrees.
		/// </summary>
		/// <param name="angle">
		///            the of rotation in degrees. </param>
		/// <returns> a CoordTransform containing the rotation. </returns>


		public static CoordTransform rotate(int angle)
		{



		    double radians = angle * Math.PI / 180;


            double cos = Math.Cos(radians);


			double sin = Math.Sin(radians);

			return new CoordTransform((float) cos, (float) cos, (float) sin, -(float) sin, 0, 0);
		}

		/// <summary>
		/// Holds the value for scaling in the x-direction. </summary>
		
		private readonly int scaleX;
		/// <summary>
		/// Holds the value for scaling in the y-direction. </summary>
		
		private readonly int scaleY;
		/// <summary>
		/// Holds the value for shearing in the x-direction. </summary>
		
		private readonly int shearX;
		/// <summary>
		/// Holds the value for shearing in the y-direction. </summary>
		
		private readonly int shearY;
		/// <summary>
		/// Holds the value for a translation in the x-direction. </summary>
		
		private readonly int translateX;
		/// <summary>
		/// Holds the value for a translation in the x-direction. </summary>
		
		private readonly int translateY;

		/// <summary>
		/// Flag used to optimise encoding so checking whether scaling terms are
		/// set is performed only once.
		/// </summary>
		
		private bool hasScale;
		/// <summary>
		/// Flag used to optimise encoding so checking whether shearing terms are
		/// set is performed only once.
		/// </summary>
		
		private bool hasShear;

		/// <summary>
		/// Used to store the number of bits required to encode or decode
		/// scaling terms.
		/// </summary>
		
		private int scaleSize;
		/// <summary>
		/// Used to store the number of bits required to encode or decode
		/// shearing terms.
		/// </summary>
		
		private int shearSize;
		/// <summary>
		/// Used to store the number of bits required to encode or decode
		/// translation terms.
		/// </summary>
		
		private int transSize;


		/// <summary>
		/// Creates and initialises a CoordTransform object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public CoordTransform(SWFDecoder coder)
		{

			coder.alignToByte();

			hasScale = coder.readBits(1, false) != 0;

			if (hasScale)
			{
				scaleSize = coder.readBits(FIELD_SIZE, false);
				scaleX = coder.readBits(scaleSize, true);
				scaleY = coder.readBits(scaleSize, true);
			}
			else
			{
				scaleX = DEFAULT_INT_SCALE;
				scaleY = DEFAULT_INT_SCALE;
			}

			hasShear = coder.readBits(1, false) != 0;

			if (hasShear)
			{
				shearSize = coder.readBits(FIELD_SIZE, false);
				shearX = coder.readBits(shearSize, true);
				shearY = coder.readBits(shearSize, true);
			}
			else
			{
				shearX = DEFAULT_INT_SHEAR;
				shearY = DEFAULT_INT_SHEAR;
			}

			transSize = coder.readBits(FIELD_SIZE, false);
			translateX = coder.readBits(transSize, true);
			translateY = coder.readBits(transSize, true);

			coder.alignToByte();
		}

		/// <summary>
		/// Creates and initialises a CoordTransform object using a 3x3 matrix.
		/// </summary>
		/// <param name="matrix">
		///            a 3x3 matrix containing the transform values. </param>


		public CoordTransform(float[][] matrix)
		{
			scaleX = (int)(matrix[0][0] * SCALE_FACTOR);
			scaleY = (int)(matrix[1][1] * SCALE_FACTOR);
			shearX = (int)(matrix[1][0] * SHEAR_FACTOR);
			shearY = (int)(matrix[0][1] * SHEAR_FACTOR);
			translateX = (int) matrix[0][2];
			translateY = (int) matrix[1][2];
		}

		/// <summary>
		/// Creates an initialises a CoordTransform with scaling, shearing and
		/// translation values.
		/// </summary>
		/// <param name="xScale">
		///            the scaling factor along the x-axis. </param>
		/// <param name="yScale">
		///            the scaling factor along the y-axis </param>
		/// <param name="xShear">
		///            the shearing factor along the x-axis. </param>
		/// <param name="yShear">
		///            the shearing factor along the y-axis </param>
		/// <param name="xCoord">
		///            the x coordinate of the transformation. </param>
		/// <param name="yCoord">
		///            the y coordinate of the transformation. </param>


		public CoordTransform(float xScale, float yScale, float xShear, float yShear, int xCoord, int yCoord)
		{
			scaleX = (int)(xScale * SCALE_FACTOR);
			scaleY = (int)(yScale * SCALE_FACTOR);
			shearX = (int)(xShear * SHEAR_FACTOR);
			shearY = (int)(yShear * SHEAR_FACTOR);
			translateX = xCoord;
			translateY = yCoord;
		}

		/// <summary>
		/// Returns the scaling factor along the x-axis.
		/// </summary>
		/// <returns> the scaling factor in the x-direction. </returns>
		public float ScaleX => scaleX / SCALE_FACTOR;

	    /// <summary>
		/// Returns the scaling factor along the y-axis.
		/// </summary>
		/// <returns> the scaling factor in the y-direction. </returns>
		public float ScaleY => scaleY / SCALE_FACTOR;

	    /// <summary>
		/// Returns the shearing factor along the x-axis.
		/// </summary>
		/// <returns> the shear factor in the x-direction. </returns>
		public float ShearX => shearX / SHEAR_FACTOR;

	    /// <summary>
		/// Returns the shearing factor along the y-axis.
		/// </summary>
		/// <returns> the shear factor in the y-direction. </returns>
		public float ShearY => shearY / SHEAR_FACTOR;

	    /// <summary>
		/// Returns the translation in the x direction.
		/// </summary>
		/// <returns> the translation, measured in twips, in the x-direction. </returns>
		public int TranslateX => translateX;

	    /// <summary>
		/// Returns the translation along the y-axis.
		/// </summary>
		/// <returns> the translation, measured in twips, in the y-direction. </returns>
		public int TranslateY => translateY;

	    /// <summary>
		/// Returns a matrix that can be used to create composite transforms.
		/// </summary>
		/// <returns> the 3 X 3 array that is used to store the transformation values. </returns>
		public float[][] Matrix => new[]
	    {
	        new[] {scaleX / SCALE_FACTOR, shearY / SHEAR_FACTOR, translateX},
	        new[] {shearX / SHEAR_FACTOR, scaleY / SCALE_FACTOR, translateY},
	        new[] {0.0f, 0.0f, 1.0f}
	    };

	    /// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, ScaleX, ScaleY, ShearX, ShearY, translateX, translateY);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public override bool Equals(object @object)
		{
			bool result;
			CoordTransform transform;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is CoordTransform)
			{
				transform = (CoordTransform) @object;
				result = (scaleX == transform.scaleX) && (scaleY == transform.scaleY) && (shearX == transform.shearX) && (shearY == transform.shearY) && (translateX == transform.translateX) && (translateY == transform.translateY);
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
			return ((((scaleX * Constants.PRIME + scaleY) * Constants.PRIME + shearX) * Constants.PRIME + shearY) * Constants.PRIME + translateX) * Constants.PRIME + translateY;
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{

			int numberOfBits = 2 + FIELD_SIZE + ROUND_TO_BYTES;

			hasScale = (scaleX != DEFAULT_INT_SCALE) || (scaleY != DEFAULT_INT_SCALE);
			hasShear = (shearX != 0) || (shearY != 0);

			if (hasScale || hasShear || ((translateX != 0) || (translateY != 0)))
			{
				transSize = Math.Max(Coder.size(translateX), Coder.size(translateY));
			}
			else
			{
				transSize = 0;
			}

			numberOfBits += transSize << 1;

			if (hasScale)
			{
				scaleSize = Math.Max(Coder.size(scaleX), Coder.size(scaleY));
				numberOfBits += FIELD_SIZE + (scaleSize << 1);
			}

			if (hasShear)
			{
				shearSize = Math.Max(Coder.size(shearX), Coder.size(shearY));
				numberOfBits += FIELD_SIZE + (shearSize << 1);
			}

			return numberOfBits >> BITS_TO_BYTES;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (hasScale)
			{
				coder.writeBits(1, 1);
				coder.writeBits(scaleSize, FIELD_SIZE);
				coder.writeBits(scaleX, scaleSize);
				coder.writeBits(scaleY, scaleSize);
			}
			else
			{
				coder.writeBits(0, 1);
			}

			if (hasShear)
			{
				coder.writeBits(1, 1);
				coder.writeBits(shearSize, FIELD_SIZE);
				coder.writeBits(shearX, shearSize);
				coder.writeBits(shearY, shearSize);
			}
			else
			{
				coder.writeBits(0, 1);
			}

			coder.writeBits(transSize, FIELD_SIZE);
			coder.writeBits(translateX, transSize);
			coder.writeBits(translateY, transSize);

			coder.alignToByte();
		}
	}

}