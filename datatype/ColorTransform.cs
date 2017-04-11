using System;
using com.flagstone.transform.coder;

/*
 * ColorTransform.java
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
	/// A ColorTransform is used to change the colour of a shape or button without
	/// changing the values in the original definition of the object.
	/// 
	/// <para>
	/// Two types of transformation are supported: Add and Multiply. In Add
	/// transformations a value is added to each colour channel:
	/// </para>
	/// 
	/// <pre>
	/// newRed = red + addRedTerm
	/// newGreen = green + addGreenTerm
	/// newBlue = blue + addBlueTerm
	/// newAlpha = alpha + addAlphaTerm
	/// </pre>
	/// 
	/// <para>
	/// In Multiply transformations each colour channel is multiplied by a given
	/// value:
	/// </para>
	/// 
	/// <pre>
	/// newRed = red * multiplyRedTerm
	/// newGreen = green * multiplyGreenTerm
	/// newBlue = blue * multiplyBlueTerm
	/// newAlpha = alpha * multiplyAlphaTerm
	/// </pre>
	/// 
	/// <para>
	/// Add and Multiply transforms may be combined in which case the multiply terms
	/// are applied to the colour channel before the add terms.
	/// </para>
	/// 
	/// <pre>
	/// newRed = (red * multiplyRedTerm) + addRedTerm
	/// newGreen = (green * multiplyGreenTerm) + addGreenTerm
	/// newBlue = (blue * multiplyBlueTerm) + addBlueTerm
	/// newAlpha = (alpha * multiplyAlphaTerm) + addAlphaTerm
	/// </pre>
	/// 
	/// <para>
	/// For each type of transform the result of the calculation is limited to the
	/// range 0..255. If the result is less than 0 or greater than 255 then it is
	/// clamped at 0 and 255 respectively.
	/// </para>
	/// 
	/// <para>
	/// Not all objects containing a colour transform use the add or multiply terms
	/// defined for the alpha channel. The colour objects defined in an DefineButton,
	/// ButtonColorTransform or PlaceObject object do not use the alpha channel while
	/// DefineButton2 and PlaceObject2 do. Whether the parent object uses the alpha
	/// channel is stored in a SWFContext which is passed when the transform is
	/// encoded or decoded.
	/// </para>
	/// 
	/// </summary>
	public sealed class ColorTransform : SWFEncodeable
	{
		/// <summary>
		/// Offset to add to number of bits when calculating number of bytes. </summary>
		private const int ROUND_TO_BYTES = 7;
		/// <summary>
		/// Right shift to convert number of bits to number of bytes. </summary>
		private const int BITS_TO_BYTES = 3;
		/// <summary>
		/// Format used by toString() to display object representation. </summary>
		private const string FORMAT = "ColorTransform: {" + "multiply=[%f, %f, %f, %f]; add=[%d, %d, %d, %d]}";

		/// <summary>
		/// Size of bit-field used to specify the number of bits representing
		/// encoded transform values.
		/// </summary>
		private const int FIELD_SIZE = 4;
		/// <summary>
		/// Default value for scaling multiply terms so they can be stored
		/// internally as integers.
		/// </summary>
		private const float SCALE_MULTIPLY = 256.0f;
		/// <summary>
		/// Default value for multiply terms when only an additive transform is
		/// created.
		/// </summary>
		private const int DEFAULT_MULTIPLY = 256;
		/// <summary>
		/// Default value for add terms when only an multiplicative transform is
		/// created.
		/// </summary>
		private const int DEFAULT_ADD = 0;

		/// <summary>
		/// Multiply term for the red colour channel. </summary>
		
		private readonly int multiplyRed;
		/// <summary>
		/// Multiply term for the green colour channel. </summary>
		
		private readonly int multiplyGreen;
		/// <summary>
		/// Multiply term for the blue colour channel. </summary>
		
		private readonly int multiplyBlue;
		/// <summary>
		/// Multiply term for the alpha colour channel. </summary>
		
		private readonly int multiplyAlpha;

		/// <summary>
		/// Add term for the red colour channel. </summary>
		
		private readonly int addRed;
		/// <summary>
		/// Add term for the green colour channel. </summary>
		
		private readonly int addGreen;
		/// <summary>
		/// Add term for the blue colour channel. </summary>
		
		private readonly int addBlue;
		/// <summary>
		/// Add term for the alpha colour channel. </summary>
		
		private readonly int addAlpha;

		/// <summary>
		/// Used in decoding and to optimise encoding so checking whether the
		/// transform contains multiply terms is performed only once.
		/// </summary>
		
		private readonly bool hasMultiply;
		/// <summary>
		/// Used in decoding and to optimise encoding so checking whether the
		/// transform contains add terms is performed only once.
		/// </summary>
		
		private readonly bool hasAdd;

		/// <summary>
		/// Used to store the number of bits required to encode or decode the
		/// transform terms.
		/// </summary>
		
		private int size;
		/// <summary>
		/// Used to optimise decoding and encoding so checking the context to see
		/// whether add and multiply terms for the alpha channel should be read
		/// written is only performed once.
		/// </summary>
		
		private bool hasAlpha;

		/// <summary>
		/// Creates and initialises a ColorTransform object using values encoded
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



		public ColorTransform(SWFDecoder coder, Context context)
		{

			hasAlpha = context.contains(Context.TRANSPARENT);
			hasAdd = coder.readBits(1, false) != 0;
			hasMultiply = coder.readBits(1, false) != 0;
			size = coder.readBits(FIELD_SIZE, false);

			if (hasMultiply)
			{
				multiplyRed = coder.readBits(size, true);
				multiplyGreen = coder.readBits(size, true);
				multiplyBlue = coder.readBits(size, true);

				if (hasAlpha)
				{
					multiplyAlpha = coder.readBits(size, true);
				}
				else
				{
					multiplyAlpha = DEFAULT_MULTIPLY;
				}
			}
			else
			{
				multiplyRed = DEFAULT_MULTIPLY;
				multiplyGreen = DEFAULT_MULTIPLY;
				multiplyBlue = DEFAULT_MULTIPLY;
				multiplyAlpha = DEFAULT_MULTIPLY;
			}

			if (hasAdd)
			{
				addRed = coder.readBits(size, true);
				addGreen = coder.readBits(size, true);
				addBlue = coder.readBits(size, true);

				if (hasAlpha)
				{
					addAlpha = coder.readBits(size, true);
				}
				else
				{
					addAlpha = DEFAULT_ADD;
				}
			}
			else
			{
				addRed = DEFAULT_ADD;
				addGreen = DEFAULT_ADD;
				addBlue = DEFAULT_ADD;
				addAlpha = DEFAULT_ADD;
			}

			coder.alignToByte();
		}

		/// <summary>
		/// Creates an add colour transform.
		/// </summary>
		/// <param name="rAdd">
		///            value to add to the red colour channel. </param>
		/// <param name="gAdd">
		///            value to add to the green colour channel. </param>
		/// <param name="bAdd">
		///            value to add to the blue colour channel. </param>
		/// <param name="aAdd">
		///            value to add to the alpha colour channel. </param>


		public ColorTransform(int rAdd, int gAdd, int bAdd, int aAdd)
		{
			multiplyRed = DEFAULT_MULTIPLY;
			multiplyGreen = DEFAULT_MULTIPLY;
			multiplyBlue = DEFAULT_MULTIPLY;
			multiplyAlpha = DEFAULT_MULTIPLY;

			addRed = rAdd;
			addGreen = gAdd;
			addBlue = bAdd;
			addAlpha = aAdd;

			hasMultiply = false;
			hasAdd = true;
		}

		/// <summary>
		/// Creates a multiply colour transform.
		/// </summary>
		/// <param name="rMul">
		///            value to multiply the red colour channel by. </param>
		/// <param name="gMul">
		///            value to multiply the green colour channel by. </param>
		/// <param name="bMul">
		///            value to multiply the blue colour channel by. </param>
		/// <param name="aMul">
		///            value to multiply the alpha colour channel by. </param>


		public ColorTransform(float rMul, float gMul, float bMul, float aMul)
		{
			multiplyRed = (int)(rMul * SCALE_MULTIPLY);
			multiplyGreen = (int)(gMul * SCALE_MULTIPLY);
			multiplyBlue = (int)(bMul * SCALE_MULTIPLY);
			multiplyAlpha = (int)(aMul * SCALE_MULTIPLY);

			addRed = DEFAULT_ADD;
			addGreen = DEFAULT_ADD;
			addBlue = DEFAULT_ADD;
			addAlpha = DEFAULT_ADD;

			hasMultiply = true;
			hasAdd = false;
		}

		/// <summary>
		/// Creates a colour transform that contains add and multiply terms.
		/// </summary>
		/// <param name="rMul">
		///            value to multiply the red colour channel by. </param>
		/// <param name="gMul">
		///            value to multiply the green colour channel by. </param>
		/// <param name="bMul">
		///            value to multiply the blue colour channel by. </param>
		/// <param name="aMul">
		///            value to multiply the alpha colour channel by. </param>
		/// <param name="rAdd">
		///            value to add to the red colour channel. </param>
		/// <param name="gAdd">
		///            value to add to the green colour channel. </param>
		/// <param name="bAdd">
		///            value to add to the blue colour channel. </param>
		/// <param name="aAdd">
		///            value to add to the alpha colour channel. </param>
		// CHECKSTYLE:OFF


		public ColorTransform(int rAdd, int gAdd, int bAdd, int aAdd, float rMul, float gMul, float bMul, float aMul)
		{
		// CHECKSTYLE:ON

			addRed = rAdd;
			addGreen = gAdd;
			addBlue = bAdd;
			addAlpha = aAdd;

			multiplyRed = (int)(rMul * SCALE_MULTIPLY);
			multiplyGreen = (int)(gMul * SCALE_MULTIPLY);
			multiplyBlue = (int)(bMul * SCALE_MULTIPLY);
			multiplyAlpha = (int)(aMul * SCALE_MULTIPLY);

			hasMultiply = true;
			hasAdd = true;
		}

		/// <summary>
		/// Returns the value of the multiply term for the red channel.
		/// </summary>
		/// <returns> the value the red colour channel will be multiplied by. </returns>
		public float MultiplyRed => multiplyRed / SCALE_MULTIPLY;

	    /// <summary>
		/// Returns the value of the multiply term for the green channel.
		/// </summary>
		/// <returns> the value the green colour channel will be multiplied by. </returns>
		public float MultiplyGreen => multiplyGreen / SCALE_MULTIPLY;

	    /// <summary>
		/// Returns the value of the multiply term for the blue channel.
		/// </summary>
		/// <returns> the value the blue colour channel will be multiplied by. </returns>
		public float MultiplyBlue => multiplyBlue / SCALE_MULTIPLY;

	    /// <summary>
		/// Returns the value of the multiply term for the alpha channel.
		/// </summary>
		/// <returns> the value the alpha channel will be multiplied by. </returns>
		public float MultiplyAlpha => multiplyAlpha / SCALE_MULTIPLY;

	    /// <summary>
		/// Returns the value of the add term for the red channel.
		/// </summary>
		/// <returns> the value that will be added to the red colour channel. </returns>
		public int AddRed => addRed;

	    /// <summary>
		/// Returns the value of the add term for the green channel.
		/// </summary>
		/// <returns> the value that will be added to the green colour channel. </returns>
		public int AddGreen => addGreen;

	    /// <summary>
		/// Returns the value of the add term for the blue channel.
		/// </summary>
		/// <returns> the value that will be added to the blue colour channel. </returns>
		public int AddBlue => addBlue;

	    /// <summary>
		/// Returns the value of the add term for the alpha channel.
		/// </summary>
		/// <returns> the value that will be added to the alpha channel. </returns>
		public int AddAlpha => addAlpha;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, MultiplyRed, MultiplyGreen, MultiplyBlue, MultiplyAlpha, addRed, addGreen, addBlue, addAlpha);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public override bool Equals(object @object)
		{
			bool result;
			ColorTransform transform;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is ColorTransform)
			{
				transform = (ColorTransform) @object;
				result = (addRed == transform.addRed) && (addGreen == transform.addGreen) && (addBlue == transform.addBlue) && (addAlpha == transform.addAlpha) && (multiplyRed == transform.multiplyRed) && (multiplyGreen == transform.multiplyGreen) && (multiplyBlue == transform.multiplyBlue) && (multiplyAlpha == transform.multiplyAlpha);
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
			return ((((((addRed * Constants.PRIME + addGreen) * Constants.PRIME + addBlue) * Constants.PRIME + addAlpha) * Constants.PRIME + multiplyRed) * Constants.PRIME + multiplyGreen) * Constants.PRIME + multiplyBlue) * Constants.PRIME + multiplyAlpha;
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{

			int numberOfBits = 2 + FIELD_SIZE + ROUND_TO_BYTES;

			hasAlpha = context.contains(Context.TRANSPARENT);
			size = 0;

			int numberOfBytes;

			if (hasAlpha)
			{
				numberOfBytes = Color.RGBA;
			}
			else
			{
				numberOfBytes = Color.RGB;
			}

			if (hasMultiply)
			{
				sizeTerms(multiplyRed, multiplyGreen, multiplyBlue, multiplyAlpha);
			}

			if (hasAdd)
			{
				sizeTerms(addRed, addGreen, addBlue, addAlpha);
			}

			if (hasMultiply)
			{
				numberOfBits += size * numberOfBytes;
			}

			if (hasAdd)
			{
				numberOfBits += size * numberOfBytes;
			}

			return numberOfBits >> BITS_TO_BYTES;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			coder.writeBits(hasAdd ? 1 : 0, 1);
			coder.writeBits(hasMultiply ? 1 : 0, 1);
			coder.writeBits(size, FIELD_SIZE);

			if (hasMultiply)
			{
				encodeTerms(multiplyRed, multiplyGreen, multiplyBlue, multiplyAlpha, coder);
			}

			if (hasAdd)
			{
				encodeTerms(addRed, addGreen, addBlue, addAlpha, coder);
			}

			coder.alignToByte();
		}

		/// <summary>
		/// Calculate the number of bits to encode either the add or multiply terms.
		/// </summary>
		/// <param name="red"> the term for the red channel. </param>
		/// <param name="green"> the term for the green channel. </param>
		/// <param name="blue"> the term for the blue channel. </param>
		/// <param name="alpha"> the term for the alpha channel. </param>


		private void sizeTerms(int red, int green, int blue, int alpha)
		{
			size = Math.Max(size, Coder.size(red));
			size = Math.Max(size, Coder.size(green));
			size = Math.Max(size, Coder.size(blue));

			if (hasAlpha)
			{
				size = Math.Max(size, Coder.size(alpha));
			}
		}

		/// <summary>
		/// Encode the add or multiply terms.
		/// </summary>
		/// <param name="red"> the term for the red channel. </param>
		/// <param name="green"> the term for the green channel. </param>
		/// <param name="blue"> the term for the blue channel. </param>
		/// <param name="alpha"> the term for the alpha channel. </param>
		/// <param name="coder"> the Coder used to encode the data.
		/// </param>
		/// <exception cref="IOException"> if there is an error writing to the underlying
		/// stream. </exception>



		private void encodeTerms(int red, int green, int blue, int alpha, SWFEncoder coder)
		{
			coder.writeBits(red, size);
			coder.writeBits(green, size);
			coder.writeBits(blue, size);

			if (hasAlpha)
			{
				coder.writeBits(alpha, size);
			}
		}
	}

}