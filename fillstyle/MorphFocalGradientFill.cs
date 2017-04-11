using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;

/*
 * FocalGradientFill.java
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

namespace com.flagstone.transform.fillstyle
{
    /// <summary>
	/// MorphFocalGradientFill extends the functionality of MorphGradientFill by
	/// allowing the focal point for the gradient at the start and end of the
	/// morphing process to be specified rather than defaulting to the centre of the
	/// shape.
	/// 
	/// The value for the focal point ranges from -1.0 to 1.0, where negative values
	/// up to -1.0 sets the focal point closer to the left border gradient circle and
	/// positive values up to 1.0 sets the focal point closer the right border. A
	/// value of zero means the focal point is in the centre.
	/// </summary>
	public sealed class MorphFocalGradientFill : FillStyle
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
		private const string FORMAT = "MorphFocalGradientFill: { spread=%s;" + " interpolation=%s; startFocalPoint=%f; endFocalPoint=%f;" + " startTransform=%s; endTransform=%s; gradients=%s}";

		/// <summary>
		/// Maps the Gradient Square to real coordinates at the start. </summary>
		private CoordTransform startTransform;
		/// <summary>
		/// Maps the Gradient Square to real coordinates at the end. </summary>
		private CoordTransform endTransform;
		/// <summary>
		/// Code for the Spread type. </summary>
		private int spread;
		/// <summary>
		/// Interpolation for colour changes. </summary>
		private int interpolation;
		/// <summary>
		/// The position of the focal point at the start of the morphing process. </summary>
		private int startFocalPoint;
		/// <summary>
		/// The position of the focal point at the end of the morphing process. </summary>
		private int endFocalPoint;
		/// <summary>
		/// List of gradients defining the colour changes. </summary>
		private IList<MorphGradient> gradients;

		/// <summary>
		/// Number of gradients in list. </summary>
		
		private int count;

		/// <summary>
		/// Creates and initialises a FocalGradientFill fill style using values
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



		public MorphFocalGradientFill(SWFDecoder coder, Context context)
		{
			startTransform = new CoordTransform(coder);
			endTransform = new CoordTransform(coder);
			count = coder.readByte() & Gradient.MAX_GRADIENTS;
			spread = count & SPREAD_MASK;
			interpolation = count & INTER_MASK;
			count = count & GRADIENT_MASK;
			gradients = new List<MorphGradient>(count);

			for (int i = 0; i < count; i++)
			{
				gradients.Add(new MorphGradient(coder, context));
			}

			startFocalPoint = coder.readSignedShort();
			endFocalPoint = coder.readSignedShort();
		}

		/// <summary>
		/// Creates a MorphFocalGradientFill.
		/// </summary>
		/// <param name="startMatrix">
		///            the coordinate transform mapping the gradient square onto
		///            physical coordinates at the start of the morphing process.
		///            Must not be null. </param>
		/// <param name="endMatrix">
		///            the coordinate transform mapping the gradient square onto
		///            physical coordinates at the end of the morphing process.
		///            Must not be null. </param>
		/// <param name="spreadType">
		///            To be documented. </param>
		/// <param name="interpolationType">
		///            how the changes in colours across the gradient are calculated. </param>
		/// <param name="startPoint">
		///            the position of the focal point relative to the centre of the
		///            radial circle at the start of the morphing process. Values
		///            range from -1.0 (close to the left edge), to 1.0 (close to
		///            the right edge). </param>
		/// <param name="endPoint">
		///            the position of the focal point relative to the centre of the
		///            radial circle at the start of the morphing process. Values
		///            range from -1.0 (close to the left edge), to 1.0 (close to
		///            the right edge). </param>
		/// <param name="list">
		///            a list of Gradient objects defining the control points for
		///            the gradient. For Flash 7 and earlier versions there can be up
		///            to 8 Gradients. For Flash 8 onwards this number was increased
		///            to 15. Must not be null. </param>


		public MorphFocalGradientFill(CoordTransform startMatrix, CoordTransform endMatrix, Spread spreadType, Interpolation interpolationType, float startPoint, float endPoint, IList<MorphGradient> list)
		{
			StartTransform = startMatrix;
			EndTransform = endMatrix;
			Spread = spreadType;
			Interpolation = interpolationType;
			StartFocalPoint = startPoint;
			EndFocalPoint = endPoint;
			Gradients = list;
		}

		/// <summary>
		/// Creates and initialises a FocalGradientFill fill style using the values
		/// copied from another FocalGradientFill object.
		/// </summary>
		/// <param name="object">
		///            a FocalGradientFill fill style from which the values will be
		///            copied. </param>


		public MorphFocalGradientFill(MorphFocalGradientFill @object)
		{
			spread = @object.spread;
			interpolation = @object.interpolation;
			startTransform = @object.startTransform;
			endTransform = @object.endTransform;
			startFocalPoint = @object.startFocalPoint;
			endFocalPoint = @object.endFocalPoint;
			gradients = new List<MorphGradient>(@object.gradients);
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
		/// Get the focal point at the start of the morphing process. </summary>
		/// <returns> the focal point in the range -1.0 to 1.0. </returns>
		public float StartFocalPoint
		{
			get => startFocalPoint / Coder.SCALE_8;
		    set => startFocalPoint = (int)(value * Coder.SCALE_8);
		}


		/// <summary>
		/// Get the focal point at the end of the morphing process. </summary>
		/// <returns> the focal point in the range -1.0 to 1.0. </returns>
		public float EndFocalPoint
		{
			get => endFocalPoint / Coder.SCALE_8;
		    set => endFocalPoint = (int)(value * Coder.SCALE_8);
		}


		/// <summary>
		/// Add a Gradient object to the list of gradient objects. For Flash 7 and
		/// earlier versions there can be up to 8 Gradients. For Flash 8 onwards this
		/// number was increased to 15.
		/// </summary>
		/// <param name="aGradient">
		///            an Gradient object. Must not be null. </param>
		/// <returns> this object. </returns>


		public MorphFocalGradientFill add(MorphGradient aGradient)
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
		/// Get the list of Gradient objects defining the points for the
		/// gradient fill.
		/// </summary>
		/// <returns> the set of points that define the gradient. </returns>
		public IList<MorphGradient> Gradients
		{
			get => gradients;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				if (gradients.Count > Gradient.MAX_GRADIENTS)
				{
					throw new InvalidOperationException("Maximum number of gradients exceeded.");
				}
				gradients = value;
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
		/// {@inheritDoc} </summary>
		public MorphFocalGradientFill copy()
		{
			return new MorphFocalGradientFill(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, Spread, Interpolation, StartFocalPoint, EndFocalPoint, startTransform.ToString(), endTransform.ToString(), gradients.ToString());
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			// Calculate size of gradient list directly.
			int length = 6 + startTransform.prepareToEncode(context) + endTransform.prepareToEncode(context);
			count = gradients.Count;

			foreach (MorphGradient gradient in gradients)
			{
				length += gradient.prepareToEncode(context);
			}
			//CHECKSTYLE:ON
			return length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(FillStyleTypes.FOCAL_GRADIENT);
			startTransform.encode(coder, context);
			endTransform.encode(coder, context);
			coder.writeByte(count | spread | interpolation);

			foreach (MorphGradient gradient in gradients)
			{
				gradient.encode(coder, context);
			}

			coder.writeShort(startFocalPoint);
			coder.writeShort(endFocalPoint);
		}
	}

}