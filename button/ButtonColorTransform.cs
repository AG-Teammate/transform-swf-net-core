using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;

/*
 * ButtonColorTransform.java
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

namespace com.flagstone.transform.button
{
    /// <summary>
	/// <para>
	/// ButtonColorTransform defines the colour transform that is applied to each
	/// shape that is used to draw a button.
	/// </para>
	/// 
	/// <para>
	/// This class is only used in conjunction with <seealso cref="DefineButton"/>. The
	/// <seealso cref="DefineButton2"/> class allows colour transforms to be specified in the
	/// ButtonRecord object that identifies each shape that is displayed for a given
	/// button state.
	/// </para>
	/// </summary>
	/// <seealso cref= DefineButton </seealso>
	/// <seealso cref= DefineButton2 </seealso>
	public sealed class ButtonColorTransform : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "ButtonColorTransform: {" + " identifier=%d; colorTransform=%s}";

		/// <summary>
		/// The unique identifier of the button. </summary>
		private int identifier;
		/// <summary>
		/// The colour transform that will be applied to the shape. </summary>
		private ColorTransform colorTransform;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a ButtonColorTransform object using values
		/// encoded in the Flash binary format.
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



		public ButtonColorTransform(SWFDecoder coder, Context context)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			colorTransform = new ColorTransform(coder, context);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a ButtonColorTransform object with a colour transform for the
		/// specified button.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier of a button that this object applies to.
		///            Must be in the range 1..65535. </param>
		/// <param name="cxform">
		///            an ColorTransform object that will be applied to the button. </param>


		public ButtonColorTransform(int uid, ColorTransform cxform)
		{
			Identifier = uid;
			ColorTransform = cxform;
		}

		/// <summary>
		/// Creates and initialises a ButtonColorTransform object using the values
		/// copied from another ButtonColorTransform object.
		/// </summary>
		/// <param name="object">
		///            a ButtonColorTransform object from which the values will be
		///            copied. </param>


		public ButtonColorTransform(ButtonColorTransform @object)
		{
			identifier = @object.identifier;
			colorTransform = @object.colorTransform;
		}

		/// <summary>
		/// Get the unique identifier of the button that this object applies to.
		/// </summary>
		/// <returns> the unique identifier of the button. </returns>
		public int Identifier
		{
			get => identifier;
		    set
			{
				if ((value < 1) || (value > Coder.USHORT_MAX))
				{
					 throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, value);
				}
				identifier = value;
			}
		}

		/// <summary>
		/// Get the colour transform that will be applied to the button.
		/// </summary>
		/// <returns> the colour transform applied to the button. </returns>
		public ColorTransform ColorTransform
		{
			get => colorTransform;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				colorTransform = value;
			}
		}



		/// <summary>
		/// {@inheritDoc} </summary>
		public ButtonColorTransform copy()
		{
			return new ButtonColorTransform(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, colorTransform);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			length = 4 + colorTransform.prepareToEncode(context);
			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.BUTTON_COLOR_TRANSFORM << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.BUTTON_COLOR_TRANSFORM << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			colorTransform.encode(coder, context);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}