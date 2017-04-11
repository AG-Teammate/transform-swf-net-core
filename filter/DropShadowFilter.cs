using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;

/*
 * DropShadowFilter.java
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
	/// DropShadowFilter is used to create a drop shadow on a object on the display
	/// list.
	/// </summary>
	public sealed class DropShadowFilter : Filter
	{

		/// <summary>
		/// Builder for creating DropShadowFilter objects.
		/// </summary>
		public sealed class Builder
		{
			/// <summary>
			/// The shadow colour. </summary>
			
			internal Color color;
			/// <summary>
			/// The horizontal blur amount. </summary>
			
			internal int blurX;
			/// <summary>
			/// The vertical blur amount. </summary>
			
			internal int blurY;
			/// <summary>
			/// Angle of shadow in radians. </summary>
			
			internal int angle;
			/// <summary>
			/// The distance of the drop shadow. </summary>
			
			internal int distance;
			/// <summary>
			/// The strength of the drop shadow. </summary>
			
			internal int strength;
			/// <summary>
			/// Compositing mode. </summary>
			
			internal int mode;
			/// <summary>
			/// The number of blur passes. </summary>
			
			internal int passes;

			/// <summary>
			/// Set the colour of the shadow. </summary>
			/// <param name="aColor"> the shadow colour. </param>
			/// <returns> this Builder. </returns>


			public Builder setShadow(Color aColor)
			{
				color = aColor;
				return this;
			}

			/// <summary>
			/// Set the blur amounts. </summary>
			/// <param name="xAmount"> the horizontal blur amount. </param>
			/// <param name="yAmount"> the vertical blur amount. </param>
			/// <returns> this Builder. </returns>


			public Builder setBlur(float xAmount, float yAmount)
			{
				blurX = (int)(xAmount * Coder.SCALE_16);
				blurY = (int)(yAmount * Coder.SCALE_16);
				return this;
			}

			/// <summary>
			/// Set the compositing mode for the shadow. </summary>
			/// <param name="filterMode"> the compositing mode, either INNER, KNOCKOUT or
			/// TOP. </param>
			/// <returns> this Builder. </returns>


			public Builder setMode(FilterMode filterMode)
			{
				switch (filterMode)
				{
				case FilterMode.KNOCKOUT:
					mode = Coder.BIT6;
					break;
				case FilterMode.INNER:
					mode = Coder.BIT7;
					break;
				default:
					throw new ArgumentException();
				}
				return this;
			}

			/// <summary>
			/// Set the shadow angle in radians. </summary>
			/// <param name="radians"> the angle. </param>
			/// <returns> this Builder. </returns>


			public Builder setAngle(float radians)
			{
				angle = (int)(radians * Coder.SCALE_16);
				return this;
			}

			/// <summary>
			/// Set the distance from the object that the shadow is displayed. </summary>
			/// <param name="width"> the width of the shadow. </param>
			/// <returns> this Builder. </returns>


			public Builder setDistance(float width)
			{
				distance = (int)(width * Coder.SCALE_16);
				return this;
			}

			/// <summary>
			/// Set the shadow strength. </summary>
			/// <param name="weight"> the weight of the shadow. </param>
			/// <returns> this Builder. </returns>


			public Builder setStrength(float weight)
			{
				strength = (int)(weight * Coder.SCALE_8);
				return this;
			}

			/// <summary>
			/// Set the number of passes for creating the blur. </summary>
			/// <param name="count"> the number of blur passes. </param>
			/// <returns> this Builder. </returns>


			public Builder setPasses(int count)
			{
				passes = count;
				return this;
			}

			/// <summary>
			/// Create a DropShadowFilter object using the parameters defined in the
			/// Builder. </summary>
			/// <returns> a DropShadowFilter object. </returns>
			public DropShadowFilter build()
			{
				return new DropShadowFilter(this);
			}
		}

		/// <summary>
		/// Bit mask for encoding and decoding the filter mode. </summary>
		private const int MODE_MASK = 0x00C0;

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DropShadowFilter: {" + " color=%s; blurX=%f; blurY=%f;" + " angle=%f; distance=%f; strength=%f; mode=%s; passes=%d}";

		/// <summary>
		/// The shadow colour. </summary>
		
		private readonly Color color;
		/// <summary>
		/// The horizontal blur amount. </summary>
		
		private readonly int blurX;
		/// <summary>
		/// The vertical blur amount. </summary>
		
		private readonly int blurY;
		/// <summary>
		/// Angle of shadow in radians. </summary>
		
		private readonly int angle;
		/// <summary>
		/// The distance of the drop shadow. </summary>
		
		private readonly int distance;
		/// <summary>
		/// The strength of the drop shadow. </summary>
		
		private readonly int strength;
		/// <summary>
		/// Compositing mode. </summary>
		
		private readonly int mode;
		/// <summary>
		/// The number of blur passes. </summary>
		
		private readonly int passes;

		/// <summary>
		/// Create a DropShadowFilter and initialize it wit the values defined in
		/// the Builder. </summary>
		/// <param name="builder"> a Builder object. </param>


		public DropShadowFilter(Builder builder)
		{
			color = builder.color;
			blurX = builder.blurX;
			blurY = builder.blurY;
			angle = builder.angle;
			distance = builder.distance;
			strength = builder.strength;
			mode = builder.mode;
			passes = builder.passes;
		}

		/// <summary>
		/// Creates and initialises a DropShadowFilter object using values encoded
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



		public DropShadowFilter(SWFDecoder coder, Context context)
		{
			color = new Color(coder, context);
			blurX = coder.readInt();
			blurY = coder.readInt();
			angle = coder.readInt();
			distance = coder.readInt();
			strength = coder.readSignedShort();



			int value = coder.readByte();
			passes = value & Coder.LOWEST5;
			mode = value & MODE_MASK;
		}

		/// <summary>
		/// Get the shadow colour. </summary>
		/// <returns> the color of the shadow. </returns>
		public Color Color => color;

	    /// <summary>
		/// Get the blur amount in the x-direction. </summary>
		/// <returns> the horizontal blur amount. </returns>
		public float BlurX => blurX / Coder.SCALE_16;

	    /// <summary>
		/// Get the blur amount in the y-direction. </summary>
		/// <returns> the vertical blur amount. </returns>
		public float BlurY => blurY / Coder.SCALE_16;

	    /// <summary>
		/// Get the angle of the shadow. </summary>
		/// <returns> the angle of the shadow in radians. </returns>
		public float Angle => angle / Coder.SCALE_16;

	    /// <summary>
		/// Get the distance of the shadow from the object. </summary>
		/// <returns> the width of the shadow. </returns>
		public float Distance => distance / Coder.SCALE_16;

	    /// <summary>
		/// Get the strength of the shadow. </summary>
		/// <returns> the shadow strength. </returns>
		public float Strength => strength / Coder.SCALE_8;

	    /// <summary>
		/// Get the compositing mode. </summary>
		/// <returns> the mode used for compositing, either TOP, INNER or KNOCKOUT. </returns>
		public FilterMode Mode
		{
			get
			{
				FilterMode value;
				switch (mode)
				{
				case Coder.BIT6:
					value = FilterMode.KNOCKOUT;
					break;
				case Coder.BIT7:
					value = FilterMode.INNER;
					break;
				default:
					throw new InvalidOperationException();
				}
				return value;
			}
		}

		/// <summary>
		/// Get the number of passes for generating the blur. </summary>
		/// <returns> the number of blur passes. </returns>
		public int Passes => passes;

	    public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, color.ToString(), Angle, Distance, Strength, BlurX, BlurY, mode, passes);
		}



		public override bool Equals(object @object)
		{
			bool result;
			DropShadowFilter filter;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is DropShadowFilter)
			{
				filter = (DropShadowFilter) @object;
				result = color.Equals(filter.color) && (blurX == filter.blurX) && (blurY == filter.blurY) && (angle == filter.angle) && (distance == filter.distance) && (strength == filter.strength) && (mode == filter.mode) && (passes == filter.passes);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return ((((((color.GetHashCode() * Constants.PRIME + blurX) * Constants.PRIME + blurY) * Constants.PRIME + angle * Constants.PRIME) + distance) * Constants.PRIME + strength) * Constants.PRIME + mode) * Constants.PRIME + passes;
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 24;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(FilterTypes.DROP_SHADOW);
			color.encode(coder, context);
			coder.writeInt(blurX);
			coder.writeInt(blurY);
			coder.writeInt(angle);
			coder.writeInt(distance);
			coder.writeShort(strength);
			coder.writeByte(Coder.BIT5 | mode | passes);
		}
	}
}