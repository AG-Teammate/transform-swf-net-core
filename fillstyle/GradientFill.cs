using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;

/*
 * GradientFill.java
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
	/// GradientFill defines how a colour changes across an area to be filled with
	/// colour. Two types of gradient fill are supported:
	/// 
	/// <ol>
	/// <li>Linear - where the gradient changes in one direction across the area to
	/// be filled.</li>
	/// 
	/// <li>Radial - where the gradient changes radially from the centre of the area
	/// to be filled.</li>
	/// </ol>
	/// 
	/// <para>
	/// Gradients are defined in terms of a standard space called the gradient
	/// square, centred at (0,0) and extending from (-16384, -16384) to (16384,
	/// 16384).
	/// </para>
	/// 
	/// <img src="doc-files/gradientSquare.gif">
	/// 
	/// <para>
	/// A coordinate transform is required to map the gradient square to the
	/// coordinates of the filled area. The transformation is applied in two steps.
	/// First the gradient square is scaled so the colour covers the shape followed
	/// by a translation to map the gradient square coordinates to the coordinate
	/// range of the shape.
	/// </para>
	/// 
	/// <img src="gradientMapping.gif">
	/// 
	/// <para>
	/// A series of gradient points is used to control how the colour displayed
	/// changes across the gradient. At least two points are required to define a
	/// gradient - one for the starting colour and one for the final colour. When the
	/// Flash Player displays the control points they are sorted by the ratio defined
	/// in each Gradient object, with the smallest ratio value displayed first.
	/// </para>
	/// </summary>
	/// <seealso cref= Gradient </seealso>
	public sealed class GradientFill : FillStyle
	{

		/// <summary>
		/// Bit mask for extracting the spread field in gradient fills. </summary>
		private const int SPREAD_MASK = 0x00C0;
		/// <summary>
		/// Bit mask for extracting the interpolation field in gradient fills. </summary>
		private const int INTER_MASK = 0x0030;
		/// <summary>
		/// Bit mask for extracting the interpolation field in gradient fills. </summary>
		private const int GRADIENT_MASK = 0x000F;
		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "GradientFill: { transform=%s;" + " gradients=%s}";

		/// <summary>
		/// Code used to identify the fill style when it is encoded. </summary>
		
		private int type;
		/// <summary>
		/// Code for the Spread type. </summary>
		private int spread;
		/// <summary>
		/// Interpolation for colour changes. </summary>
		private int interpolation;
		/// <summary>
		/// Maps the Gradient Square to real coordinates. </summary>
		private CoordTransform transform;
		/// <summary>
		/// List of gradients defining the colour changes. </summary>
		private IList<Gradient> gradients;

		/// <summary>
		/// Number of gradients in list. </summary>
		
		private int count;

		/// <summary>
		/// Creates and initialises a GradientFill fill style using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="fillType"> the value used to identify the fill style when it is
		/// encoded.
		/// </param>
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



		public GradientFill(int fillType, SWFDecoder coder, Context context)
		{
			type = fillType;
			transform = new CoordTransform(coder);
			count = coder.readByte();
			spread = count & SPREAD_MASK;
			interpolation = count & INTER_MASK;
			count = count & GRADIENT_MASK;
			gradients = new List<Gradient>(count);

			for (int i = 0; i < count; i++)
			{
				gradients.Add(new Gradient(coder, context));
			}
		}

		/// <summary>
		/// Creates a GradientFill object specifying the type, coordinate transform
		/// and list of gradient points.
		/// </summary>
		/// <param name="gradientType">
		///            identifies whether the gradient is rendered linearly or
		///            radially. </param>
		/// <param name="matrix">
		///            the coordinate transform mapping the gradient square onto
		///            physical coordinates. Must not be null. </param>
		/// <param name="list">
		///            a list of Gradient objects defining the control points for
		///            the gradient. For Flash 7 and earlier versions there can be up
		///            to 8 Gradients. For Flash 8 onwards this number was increased
		///            to 15. Must not be null. </param>


		public GradientFill(GradientType gradientType, CoordTransform matrix, IList<Gradient> list)
		{
			Type = gradientType;
			Transform = matrix;
			Gradients = list;
		}

		/// <summary>
		/// Creates a GradientFill object specifying the type, coordinate transform
		/// and list of gradient points.
		/// </summary>
		/// <param name="gradientType">
		///            identifies whether the gradient is rendered linearly or
		///            radially. </param>
		/// <param name="matrix">
		///            the coordinate transform mapping the gradient square onto
		///            physical coordinates. Must not be null. </param>
		/// <param name="spreadType">
		///            To be documented. </param>
		/// <param name="interpolationType">
		///            how the changes in colours across the gradient are calculated. </param>
		/// <param name="list">
		///            a list of Gradient objects defining the control points for
		///            the gradient. For Flash 7 and earlier versions there can be up
		///            to 8 Gradients. For Flash 8 onwards this number was increased
		///            to 15. Must not be null. </param>


		public GradientFill(GradientType gradientType, CoordTransform matrix, Spread spreadType, Interpolation interpolationType, IList<Gradient> list)
		{
			Type = gradientType;
			Transform = matrix;
			Spread = spreadType;
			Interpolation = interpolationType;
			Gradients = list;
		}

		/// <summary>
		/// Creates and initialises a GradientFill fill style using the values copied
		/// from another GradientFill object.
		/// </summary>
		/// <param name="object">
		///            a  GradientFill fill style from which the values will be
		///            copied. </param>


		public GradientFill(GradientFill @object)
		{
			type = @object.type;
			transform = @object.transform;
			gradients = new List<Gradient>(@object.gradients);
		}

		/// <summary>
		/// Get the GradientType that identifies whether the gradient is linear or
		/// radial.
		/// </summary>
		/// <returns> the GradientType for the fill, either LINEAR or RADIAL. </returns>
		public GradientType Type
		{
			get
			{
				GradientType value;
				if (type == FillStyleTypes.LINEAR_GRADIENT)
				{
					value = GradientType.LINEAR;
				}
				else
				{
					value = GradientType.RADIAL;
				}
				return value;
			}
			set
			{
				if (value == GradientType.LINEAR)
				{
					type = FillStyleTypes.LINEAR_GRADIENT;
				}
				else
				{
					type = FillStyleTypes.RADIAL_GRADIENT;
				}
			}
		}


		/// <summary>
		/// Get the Spread describing how the gradient fills the area: PAD - the
		/// last colour fills the remaining area; REPEAT - the gradient is repeated;
		/// REFLECT - the gradient is repeated but reflected (reversed) each time.
		/// </summary>
		/// <returns> the Spread, either PAD, REFLECT or REPEAT. </returns>
		public Spread Spread
		{
			get => Spread.fromInt(spread);
		    set => spread = value.Value;
		}


		/// <summary>
		/// Get the method used to calculate the colour changes across the gradient.
		/// </summary>
		/// <returns> the Interpolation that describes how colours change. </returns>
		public Interpolation Interpolation
		{
			get => Interpolation.fromInt(interpolation);
		    set => interpolation = value.Value;
		}


		/// <summary>
		/// Get the coordinate transform mapping the gradient square onto
		/// physical coordinates.
		/// </summary>
		/// <returns> the coordinate transform that defines the gradient displayed. </returns>
		public CoordTransform Transform
		{
			get => transform;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				transform = value;
			}
		}

		/// <summary>
		/// Get the list of Gradient objects defining the points for the
		/// gradient fill.
		/// </summary>
		/// <returns> the list of points defining the gradient. </returns>
		public IList<Gradient> Gradients
		{
			get => gradients;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				if (value.Count > Gradient.MAX_GRADIENTS)
				{
					throw new InvalidOperationException("Maximum number of gradients exceeded.");
				}
				gradients = value;
			}
		}



		/// <summary>
		/// Add a Gradient object to the list of gradient objects. For Flash 7 and
		/// earlier versions there can be up to 8 Gradients. For Flash 8 onwards this
		/// number was increased to 15.
		/// </summary>
		/// <param name="aGradient">
		///            an Gradient object. Must not be null.
		/// </param>
		/// <returns> this object. </returns>


		public GradientFill add(Gradient aGradient)
		{
			if (aGradient == null)
			{
				throw new ArgumentException();
			}
			if (gradients.Count == Gradient.MAX_GRADIENTS)
			{
				throw new InvalidOperationException("Maximum number of gradients exceeded.");
			}
			gradients.Add(aGradient);
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public GradientFill copy()
		{
			return new GradientFill(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, transform, gradients);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			count = gradients.Count;
			return 2 + transform.prepareToEncode(context) + (count * (context.contains(Context.TRANSPARENT) ? 5 : 4));
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(type);
			transform.encode(coder, context);
			coder.writeByte(count | spread | interpolation);

			foreach (Gradient gradient in gradients)
			{
				gradient.encode(coder, context);
			}
		}
	}

}