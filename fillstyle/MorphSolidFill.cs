/*
 * MorphSolidFill.java
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
	/// MorphSolidFill defines the solid colours that are used to fill a shape at the
	/// start and end of the morphing process.
	/// </summary>
	public sealed class MorphSolidFill : FillStyle
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "MorphSolidFill: { start=%s;" + " end=%s}";
		/// <summary>
		/// The colour at the start of the morphing process. </summary>
		private Color startColor;
		/// <summary>
		/// The colour at the end of the morphing process. </summary>
		private Color endColor;

		/// <summary>
		/// Creates and initialises a MorphSolidFill fill style using values encoded
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



		public MorphSolidFill(SWFDecoder coder, Context context)
		{
			startColor = new Color(coder, context);
			endColor = new Color(coder, context);
		}

		/// <summary>
		/// Creates a MorphSolidFill object specifying the starting and ending
		/// colours.
		/// </summary>
		/// <param name="start">
		///            the colour at the start of the morphing process. </param>
		/// <param name="end">
		///            the colour at the end of the morphing process. </param>


		public MorphSolidFill(Color start, Color end)
		{
			StartColor = start;
			EndColor = end;
		}

		/// <summary>
		/// Creates and initialises a MorphSolidFill fill style using the values
		/// copied from another MorphSolidFill object.
		/// </summary>
		/// <param name="object">
		///            a MorphSolidFill fill style from which the values will be
		///            copied. </param>


		public MorphSolidFill(MorphSolidFill @object)
		{
			startColor = @object.startColor;
			endColor = @object.endColor;
		}

		/// <summary>
		/// Get the colour at the start of the morphing process.
		/// </summary>
		/// <returns> the starting fill colour. </returns>
		public Color StartColor
		{
			get => startColor;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				startColor = value;
			}
		}

		/// <summary>
		/// Get the colour at the end of the morphing process.
		/// </summary>
		/// <returns> the final fill colour. </returns>
		public Color EndColor
		{
			get => endColor;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				endColor = value;
			}
		}



		/// <summary>
		/// {@inheritDoc} </summary>
		public MorphSolidFill copy()
		{
			return new MorphSolidFill(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, startColor, endColor);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			return 9;
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(FillStyleTypes.SOLID_COLOR);
			startColor.encode(coder, context);
			endColor.encode(coder, context);
		}
	}

}