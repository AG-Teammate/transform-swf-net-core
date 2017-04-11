using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.fillstyle;

/*
 * GradientBevelFilter.java
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
	/// GradientBevelFilter is used to create a smooth bevel with a gradient colour
	/// around an object on the display list.
	/// </summary>
	public sealed class GradientBevelFilter : Filter
	{

		/// <summary>
		/// Builder for creating GradientBevelFilter objects.
		/// </summary>
		public sealed class Builder
		{
			/// <summary>
			/// The list of gradients for the colour. </summary>
			
			internal readonly IList<Gradient> gradients;
			/// <summary>
			/// The horizontal blur amount. </summary>
			
			internal int blurX;
			/// <summary>
			/// The vertical blur amount. </summary>
			
			internal int blurY;
			/// <summary>
			/// Angle of gradient bevel in radians. </summary>
			
			internal int angle;
			/// <summary>
			/// Distance of the gradient bevel. </summary>
			
			internal int distance;
			/// <summary>
			/// Strength of the gradient bevel. </summary>
			
			internal int strength;
			/// <summary>
			/// Compositing mode. </summary>
			
			internal int mode;
			/// <summary>
			/// The number of blur passes. </summary>
			
			internal int passes;

			/// <summary>
			/// Creates a new Builder.
			/// </summary>
			public Builder()
			{
				gradients = new List<Gradient>();
			}

			/// <summary>
			/// Add a Gradient to the list. </summary>
			/// <param name="gradient"> a Gradient object. </param>
			/// <returns> this Builder. </returns>


			public Builder addGradient(Gradient gradient)
			{
				gradients.Add(gradient);
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
				case FilterMode.TOP:
					 mode = Coder.BIT4;
					break;
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
			/// Set the glow angle in radians. </summary>
			/// <param name="radians"> the angle. </param>
			/// <returns> this Builder. </returns>


			public Builder setAngle(float radians)
			{
				angle = (int)(radians * Coder.SCALE_16);
				return this;
			}

			/// <summary>
			/// Set the distance of the glow from the object. </summary>
			/// <param name="width"> the width of the glow. </param>
			/// <returns> this Builder. </returns>


			public Builder setDistance(float width)
			{
				distance = (int)(width * Coder.SCALE_16);
				return this;
			}

			/// <summary>
			/// Set the glow strength. </summary>
			/// <param name="weight"> the weight of the glow. </param>
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
			/// Create a GradientBevelFilter object using the parameters defined in
			/// the Builder. </summary>
			/// <returns> a GradientBevelFilter object. </returns>
			public GradientBevelFilter build()
			{
				return new GradientBevelFilter(this);
			}
		}

		/// <summary>
		/// Bit mask for encoding and decoding the filter mode. </summary>
		private const int MODE_MASK = 0x00D0;

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "GradientBevelFilter: {" + " gradients=%s; blurX=%f; blurY=%f;" + " angle=%f; distance=%f; strength=%f; mode=%s; passes=%d}";

		/// <summary>
		/// The list of gradients. </summary>
		
		private readonly IList<Gradient> gradients;
		/// <summary>
		/// The horizontal blur amount. </summary>
		
		private readonly int blurX;
		/// <summary>
		/// The vertical blur amount. </summary>
		
		private readonly int blurY;
		/// <summary>
		/// Angle of gradient bevel in radians. </summary>
		
		private readonly int angle;
		/// <summary>
		/// Distance of the gradient bevel. </summary>
		
		private readonly int distance;
		/// <summary>
		/// Strength of the gradient bevel. </summary>
		
		private readonly int strength;
		/// <summary>
		/// Compositing mode. </summary>
		
		private readonly int mode;
		/// <summary>
		/// The number of blur passes. </summary>
		
		private readonly int passes;

		/// <summary>
		/// Create a GradientBevelFilter and initialize it wit the values defined in
		/// the Builder. </summary>
		/// <param name="builder"> a Builder object. </param>


		public GradientBevelFilter(Builder builder)
		{
			gradients = builder.gradients;
			blurX = builder.blurX;
			blurY = builder.blurY;
			angle = builder.angle;
			distance = builder.distance;
			strength = builder.strength;
			mode = builder.mode;
			passes = builder.passes;
		}

		/// <summary>
		/// Creates and initialises a GradientBevelFilter object using values encoded
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



		public GradientBevelFilter(SWFDecoder coder, Context context)
		{


			int count = coder.readByte();


			Color[] colors = new Color[count];


			int[] ratioes = new int[count];

			for (int i = 0; i < count; i++)
			{
				colors[i] = new Color(coder, context);
			}
			for (int i = 0; i < count; i++)
			{
				ratioes[i] = coder.readByte();
			}

			gradients = new List<Gradient>(count);
			for (int i = 0; i < count; i++)
			{
				gradients.Add(new Gradient(ratioes[i], colors[i]));
			}

			blurX = coder.readInt();
			blurY = coder.readInt();
			angle = coder.readInt();
			distance = coder.readInt();
			strength = coder.readSignedShort();



			int value = coder.readByte();

			passes = value & Coder.NIB0;
			mode = value & MODE_MASK;
		}

		/// <summary>
		/// Get the list of gradients used to create the glow colour. </summary>
		/// <returns> the list of Gradient objects. </returns>
		public IList<Gradient> Gradients => gradients;

	    /// <summary>
		/// Get the blur amount in the x-direction. </summary>
		/// <returns> the horizontal blur amount. </returns>
		public float BlurX => blurX / Coder.SCALE_16;

	    /// <summary>
		/// Get the blur amount in the y-direction. </summary>
		/// <returns> the vertical blur amount. </returns>
		public float BlurY => blurY / Coder.SCALE_16;

	    /// <summary>
		/// Get the angle of the glow. </summary>
		/// <returns> the angle of the glow in radians. </returns>
		public float Angle => angle / Coder.SCALE_16;

	    /// <summary>
		/// Get the distance of the glow from the object. </summary>
		/// <returns> the width of the glow. </returns>
		public float Distance => distance / Coder.SCALE_16;

	    /// <summary>
		/// Get the strength of the glow. </summary>
		/// <returns> the glow strength. </returns>
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
				case Coder.BIT4:
					value = FilterMode.TOP;
					break;
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
			return ObjectExtensions.FormatJava(FORMAT, gradients.ToString(), BlurX, BlurY, Angle, Distance, Strength, mode, passes);
		}



		public override bool Equals(object @object)
		{
			bool result;
			GradientBevelFilter filter;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is GradientBevelFilter)
			{
				filter = (GradientBevelFilter) @object;


				result = gradients.SequenceEqual(filter.gradients) && (blurX == filter.blurX) && (blurY == filter.blurY) && (angle == filter.angle) && (distance == filter.distance) && (strength == filter.strength) && (mode == filter.mode) && (passes == filter.passes);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return (((((((gradients.GetHashCode() * Constants.PRIME + blurX) * Constants.PRIME + blurY) * Constants.PRIME + angle) * Constants.PRIME) + distance) * Constants.PRIME + strength) * Constants.PRIME + mode) * Constants.PRIME + passes;
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 21 + 5 * gradients.Count;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(FilterTypes.GRADIENT_BEVEL);
			coder.writeByte(gradients.Count);

			foreach (Gradient gradient in gradients)
			{
				gradient.Color.encode(coder, context);
			}

			foreach (Gradient gradient in gradients)
			{
				coder.writeByte(gradient.Ratio);
			}

			coder.writeInt(blurX);
			coder.writeInt(blurY);
			coder.writeInt(angle);
			coder.writeInt(distance);
			coder.writeShort(strength);
			coder.writeByte(Coder.BIT5 | mode | passes);
		}
	}

}