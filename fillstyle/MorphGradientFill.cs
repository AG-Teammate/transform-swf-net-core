using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;

/*
 * MorphGradientFill.java
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
	/// MorphGradientFill defines how a colour gradient changes across an area filled
	/// in a shape as it is morphed. <seealso cref="GradientFill"/> has a description of colour
	/// gradients.
	/// </summary>
	/// <seealso cref= MorphGradient </seealso>
	/// <seealso cref= GradientFill </seealso>
	public sealed class MorphGradientFill : FillStyle
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "MorphGradientFill: { start=%s;" + " end=%s; gradients=%s}";

		/// <summary>
		/// Code used to identify the fill style when it is encoded. </summary>
		private int type;
		/// <summary>
		/// Maps the Gradient Square to real coordinates at the start. </summary>
		private CoordTransform startTransform;
		/// <summary>
		/// Maps the Gradient Square to real coordinates at the end. </summary>
		private CoordTransform endTransform;
		/// <summary>
		/// List of gradients defining the colour changes. </summary>
		private IList<MorphGradient> gradients;

		/// <summary>
		/// Number of gradients in list. </summary>
		
		private int count;

		/// <summary>
		/// Creates and initialises an MorphGradientFill fill style using values
		/// encoded in the Flash binary format.
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



		public MorphGradientFill(int fillType, SWFDecoder coder, Context context)
		{
			type = fillType;
			startTransform = new CoordTransform(coder);
			endTransform = new CoordTransform(coder);
			count = coder.readByte() & Gradient.MAX_GRADIENTS;

			gradients = new List<MorphGradient>(count);

			for (int i = 0; i < count; i++)
			{
				gradients.Add(new MorphGradient(coder, context));
			}
		}

		/// <summary>
		/// Creates a MorphGradientFill object specifying the type of fill, starting
		/// and ending coordinate transforms and the list of gradient records.
		/// </summary>
		/// <param name="gradientType">
		///            identifies whether the gradient is rendered linearly or
		///            radially. </param>
		/// <param name="start">
		///            the coordinate transform mapping the gradient square onto
		///            physical coordinates at the start of the morphing process. </param>
		/// <param name="end">
		///            the coordinate transform mapping the gradient square onto
		///            physical coordinates at the end of the morphing process. </param>
		/// <param name="grads">
		///            a list of MorphGradient objects defining the control points
		///            for the gradient. </param>


		public MorphGradientFill(GradientType gradientType, CoordTransform start, CoordTransform end, IList<MorphGradient> grads)
		{
			Type = gradientType;
			StartTransform = start;
			EndTransform = end;
			Gradients = grads;
		}

		/// <summary>
		/// Creates and initialises a MorphGradientFill fill style using the values
		/// copied from another MorphGradientFill object.
		/// </summary>
		/// <param name="object">
		///            a MorphGradientFill fill style from which the values will be
		///            copied. </param>


		public MorphGradientFill(MorphGradientFill @object)
		{
			type = @object.type;
			startTransform = @object.startTransform;
			endTransform = @object.endTransform;

			gradients = new List<MorphGradient>(@object.gradients.Count);

			foreach (MorphGradient gradient in @object.gradients)
			{
				gradients.Add(gradient.copy());
			}
		}

		/// <summary>
		/// Add a MorphGradient object to the list of gradient objects.
		/// </summary>
		/// <param name="aGradient">
		///            an MorphGradient object. Must not be null. </param>
		/// <returns> this object. </returns>


		public MorphGradientFill add(MorphGradient aGradient)
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
		/// Get the type indicating whether the gradient is linear or radial. </summary>
		/// <returns> the gradient type, either LINEAR or RADIAL. </returns>
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
		/// Get the coordinate transform mapping the gradient square onto
		/// physical coordinates at the start of the morphing process.
		/// </summary>
		/// <returns> the starting transform for the gradient. </returns>
		public CoordTransform StartTransform
		{
			get => startTransform;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				startTransform = value;
			}
		}

		/// <summary>
		/// Get the coordinate transform mapping the gradient square onto
		/// physical coordinates at the end of the morphing process.
		/// </summary>
		/// <returns> the final transform for the gradient. </returns>
		public CoordTransform EndTransform
		{
			get => endTransform;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				endTransform = value;
			}
		}

		/// <summary>
		/// Get the list of MorphGradients defining the control points for the
		/// gradient.
		/// </summary>
		/// <returns> the list of points that define the gradient. </returns>
		public IList<MorphGradient> Gradients
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
		/// {@inheritDoc} </summary>
		public MorphGradientFill copy()
		{
			return new MorphGradientFill(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, startTransform, endTransform, gradients);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			count = gradients.Count;
			return 2 + startTransform.prepareToEncode(context) + endTransform.prepareToEncode(context) + (count * 10);
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(type);
			startTransform.encode(coder, context);
			endTransform.encode(coder, context);
			coder.writeByte(count);

			foreach (MorphGradient gradient in gradients)
			{
				gradient.encode(coder, context);
			}
		}
	}

}