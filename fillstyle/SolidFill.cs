﻿/*
 * SolidFill.java
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
using com.flagstone.transform.datatype;

namespace com.flagstone.transform.fillstyle
{
    /// <summary>
	/// SolidFill defines a solid colour that is used to fill an enclosed area in a
	/// shape. Shapes can be filled with transparent colours but only if the fill
	/// style is used in a DefineShape3 object.
	/// </summary>
	public sealed class SolidFill : FillStyle
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "SolidFill: { color=%s}";
		/// <summary>
		/// The colour used to fill the shape. </summary>
		private Color color;

		/// <summary>
		/// Creates and initialises a SolidFill fill style using values encoded
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



		public SolidFill(SWFDecoder coder, Context context)
		{
			color = new Color(coder, context);
		}

		/// <summary>
		/// Creates a SolidFill object with the specified colour.
		/// </summary>
		/// <param name="aColor">
		///            an Color object that defines the colour that the area will be
		///            filled with. Must not be null. </param>


		public SolidFill(Color aColor)
		{
			Color = aColor;
		}

		/// <summary>
		/// Creates and initialises a SolidFill fill style using the values copied
		/// from another SolidFill object.
		/// </summary>
		/// <param name="object">
		///            a SolidFill fill style from which the values will be
		///            copied. </param>


		public SolidFill(SolidFill @object)
		{
			color = @object.color;
		}

		/// <summary>
		/// Get the colour of the fill style.
		/// </summary>
		/// <returns> the fill colour </returns>
		public Color Color
		{
			get => color;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				color = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public SolidFill copy()
		{
			return new SolidFill(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, color.ToString());
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
			coder.writeByte(FillStyleTypes.SOLID_COLOR);
			color.encode(coder, context);
		}
	}

}