using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;

/*
 * MorphBitmapFill.java
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
	/// MorphBitmapFill uses a bitmap image to fill an area of a morphing shape. Four
	/// types of bitmap fill are supported:
	/// 
	/// <ol>
	/// <li>Clipped - If the image is larger than the shape then it will be clipped.
	/// Conversely if the area to be filled is larger than the image the colour at
	/// the edge of the image is used to fill the remainder of the shape.</li>
	/// 
	/// <li>Tiled - if the area to be filled is larger than the image then the image
	/// is tiled to fill the area, otherwise as with the Clipped style the colour at
	/// the edge of the image will be use to fill the space available.</li>
	/// 
	/// <li>Unsmoothed Clipped - Same as Clipped but if the image is smaller than the
	/// shape the colour used to fill the space available is not smoothed. This style
	/// was added to increase performance with few visible artifacts.</li>
	/// 
	/// <li>Unsmoothed Tiled - Same as Tiled but no smoothing is applied if the
	/// colour at the edge of the image is used to fill the space available. Again
	/// this was introduced to increase performance.</li>
	/// </ol>
	/// 
	/// <para>
	/// Two coordinate transforms define the appearance of the image at the start and
	/// end of the morphing process. The most common use of the coordinate transform
	/// is to scale an image so it displayed at the correct resolution. When an image
	/// is loaded its width and height default to twips rather than pixels. An image
	/// 300 x 200 pixels will be displayed as 300 x 200 twips (15 x 10 pixels).
	/// Scaling the image by 20 (20 twips = 1 pixel) using the CoordTransform object
	/// will restore it to its original size.
	/// </para>
	/// 
	/// <para>
	/// The coordinate transform is also used to control the image registration. An
	/// image is drawn with the top left corner placed at the origin (0, 0) of the
	/// shape being filled. The transform can be used to apply different translations
	/// to the image so its position can be adjusted relative to the origin of the
	/// enclosing shape.
	/// </para>
	/// </summary>
	public sealed class MorphBitmapFill : FillStyle
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "MorphBitmapFill: { identifier=%d;" + " start=%s; end=%s}";
		/// <summary>
		/// Bit mask for tiled or clipped field in bitmap fills. </summary>
		private const int CLIPPED_MASK = 1;
		/// <summary>
		/// Bit mask for smoothed or unsmoothed field in bitmap fills. </summary>
		private const int SMOOTHED_MASK = 2;

		/// <summary>
		/// Code used to identify the fill style when it is encoded. </summary>
		
		private int type;
		/// <summary>
		/// The unique identifier of the image that will be displayed. </summary>
		private int identifier;
		/// <summary>
		/// Coordinate transform at start of morphing process. </summary>
		private CoordTransform startTransform;
		/// <summary>
		/// Coordinate transform at end of morphing process. </summary>
		private CoordTransform endTransform;

		/// <summary>
		/// Creates and initialises a MorphBitmapFill fill style using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="fillType"> the value used to identify the fill style when it is
		/// encoded.
		/// </param>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		 public MorphBitmapFill(int fillType, SWFDecoder coder)
		 {
			type = fillType;
			identifier = coder.readUnsignedShort();
			startTransform = new CoordTransform(coder);
			endTransform = new CoordTransform(coder);
		 }

		/// <summary>
		/// Creates a MorphBitmapFill specifying the type, bitmap image and
		/// coordinate transforms for the image at the start and end of the morphing
		/// process.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for the image. Must be in the range
		///            1..65535.
		/// </param>
		/// <param name="tiled"> indicates whether the image will be tiled across the area
		/// defined by the shape.
		/// </param>
		/// <param name="smoothed"> whether smoothing will be applied to the image to
		/// improve its appearance.
		/// </param>
		/// <param name="start">
		///            the coordinate transform defining the appearance of the image
		///            at the start of the morphing process. </param>
		/// <param name="end">
		///            the coordinate transform defining the appearance of the image
		///            at the end of the morphing process. </param>


		public MorphBitmapFill(bool tiled, bool smoothed, int uid, CoordTransform start, CoordTransform end)
		{
			type = FillStyleTypes.TILED_BITMAP;
			Tiled = tiled;
			Smoothed = smoothed;
			Identifier = uid;
			StartTransform = start;
			EndTransform = end;
		}

		/// <summary>
		/// Creates and initialises a MorphBitmapFill fill style using the values
		/// copied from another MorphBitmapFill object.
		/// </summary>
		/// <param name="object">
		///            a MorphBitmapFill fill style from which the values will be
		///            copied. </param>


		public MorphBitmapFill(MorphBitmapFill @object)
		{
			type = @object.type;
			identifier = @object.identifier;
			startTransform = @object.startTransform;
			endTransform = @object.endTransform;
		}

		/// <summary>
		/// Is the image tiled across the filled area. </summary>
		/// <returns> true if the image is tiled to completely cover the area to be
		/// filled, false otherwise. </returns>
		public bool Tiled
		{
			get
			{
				return (type & CLIPPED_MASK) != 0;
			}
			set
			{
				if (value)
				{
					type &= ~CLIPPED_MASK;
				}
				else
				{
					type |= CLIPPED_MASK;
				}
			}
		}


		/// <summary>
		/// Is the image smoothed to improve display quality. </summary>
		/// <returns> true if the image will be smoothed, false if smoothing is not
		/// applied to increase performance. </returns>
		public bool Smoothed
		{
			get
			{
				return (type & SMOOTHED_MASK) != 0;
			}
			set
			{
				if (value)
				{
					type &= ~SMOOTHED_MASK;
				}
				else
				{
					type |= SMOOTHED_MASK;
				}
			}
		}


		/// <summary>
		/// Get the unique identifier of the bitmap image.
		/// </summary>
		/// <returns> the image identifier. </returns>
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
		/// Get the coordinate transform defining the appearance of the image at
		/// the start of the morphing process.
		/// </summary>
		/// <returns> the starting coordinate transform applied to the image. </returns>
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
		/// Get the coordinate transform defining the appearance of the image at
		/// the end of the morphing process.
		/// </summary>
		/// <returns> the final coordinate transform applied to the image. </returns>
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
		public MorphBitmapFill copy()
		{
			return new MorphBitmapFill(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, startTransform, endTransform);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			return 3 + startTransform.prepareToEncode(context) + endTransform.prepareToEncode(context);
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(type);
			coder.writeShort(identifier);
			startTransform.encode(coder, context);
			endTransform.encode(coder, context);
		}
	}

}