using System;
using System.Collections.Generic;

/*
 * Blend.java
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
namespace com.flagstone.transform.datatype
{

	/// <summary>
	/// Blend modes let you control how the colours and transparency of successive
	/// layers are composited together when the Flash Player displays the objects on
	/// the screen. The effect is to create highlights, shadows or to control how an
	/// underlying object appears.
	/// </summary>
	public sealed class Blend
	{
		/// <summary>
		/// No Blend.
		/// </summary>
		public static readonly Blend NULL = new Blend("NULL", InnerEnum.NULL, BlendTypes.NULL);
		/// <summary>
		/// Applies colour form the current layer normally with no blending with the
		/// underlying layers.
		/// </summary>
		public static readonly Blend NORMAL = new Blend("NORMAL", InnerEnum.NORMAL, BlendTypes.NORMAL);
		/// <summary>
		/// Sets the opacity of the current layer at 100% before blending.
		/// </summary>
		public static readonly Blend LAYER = new Blend("LAYER", InnerEnum.LAYER, BlendTypes.LAYER);
		/// <summary>
		/// Multiplies layers together. This has the effect of darkening the layer.
		/// </summary>
		public static readonly Blend MULTIPLY = new Blend("MULTIPLY", InnerEnum.MULTIPLY, BlendTypes.MULTIPLY);
		/// <summary>
		/// Multiplies this inverse of the layer with the underlying layer, creating
		/// a bleaching effect.
		/// </summary>
		public static readonly Blend SCREEN = new Blend("SCREEN", InnerEnum.SCREEN, BlendTypes.SCREEN);
		/// <summary>
		/// Displays colours from the underlying layer that are lighter than the
		/// current layer.
		/// </summary>
		public static readonly Blend LIGHTEN = new Blend("LIGHTEN", InnerEnum.LIGHTEN, BlendTypes.LIGHTEN);
		/// <summary>
		/// Displays colours from the underlying layer that are darker than the
		/// current layer.
		/// </summary>
		public static readonly Blend DARKEN = new Blend("DARKEN", InnerEnum.DARKEN, BlendTypes.DARKEN);
		/// <summary>
		/// Add the colours of the layers together.
		/// </summary>
		public static readonly Blend ADD = new Blend("ADD", InnerEnum.ADD, BlendTypes.ADD);
		/// <summary>
		/// Subtract the current layer colour from the underlying layer.
		/// </summary>
		public static readonly Blend SUBTRACT = new Blend("SUBTRACT", InnerEnum.SUBTRACT, BlendTypes.SUBTRACT);
		/// <summary>
		/// Subtracts the largest colour value from the smallest, creating a colour
		/// negative effect.
		/// </summary>
		public static readonly Blend DIFFERENCE = new Blend("DIFFERENCE", InnerEnum.DIFFERENCE, BlendTypes.DIFFERENCE);
		/// <summary>
		/// Inverts the colours of the current layer.
		/// </summary>
		public static readonly Blend INVERT = new Blend("INVERT", InnerEnum.INVERT, BlendTypes.INVERT);
		/// <summary>
		/// Applies the transparency of the current layer to the underlying layer.
		/// </summary>
		public static readonly Blend ALPHA = new Blend("ALPHA", InnerEnum.ALPHA, BlendTypes.ALPHA);
		/// <summary>
		/// Delete the colours from the underlying layer that match the colour on the
		/// current layer.
		/// </summary>
		public static readonly Blend ERASE = new Blend("ERASE", InnerEnum.ERASE, BlendTypes.ERASE);
		/// <summary>
		/// Use the colour from the current layer to select colours from the
		/// underlying layer.
		/// </summary>
		public static readonly Blend OVERLAY = new Blend("OVERLAY", InnerEnum.OVERLAY, BlendTypes.OVERLAY);
		/// <summary>
		/// Select colours from the underlying layer using the values on the current
		/// layer.
		/// </summary>
		public static readonly Blend HARDLIGHT = new Blend("HARDLIGHT", InnerEnum.HARDLIGHT, BlendTypes.HARDLIGHT);

		private static readonly IList<Blend> valueList = new List<Blend>();

		public enum InnerEnum
		{
			NULL,
			NORMAL,
			LAYER,
			MULTIPLY,
			SCREEN,
			LIGHTEN,
			DARKEN,
			ADD,
			SUBTRACT,
			DIFFERENCE,
			INVERT,
			ALPHA,
			ERASE,
			OVERLAY,
			HARDLIGHT
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal;

		/// <summary>
		/// Table used to map encoded integer values to different Blends. </summary>
		private static readonly IDictionary<int?, Blend> TABLE = new Dictionary <int?, Blend>();

		static Blend()
		{
			foreach (Blend format in values())
			{
				TABLE[format.value] = format;
			}

			valueList.Add(NULL);
			valueList.Add(NORMAL);
			valueList.Add(LAYER);
			valueList.Add(MULTIPLY);
			valueList.Add(SCREEN);
			valueList.Add(LIGHTEN);
			valueList.Add(DARKEN);
			valueList.Add(ADD);
			valueList.Add(SUBTRACT);
			valueList.Add(DIFFERENCE);
			valueList.Add(INVERT);
			valueList.Add(ALPHA);
			valueList.Add(ERASE);
			valueList.Add(OVERLAY);
			valueList.Add(HARDLIGHT);
		}

		/// <summary>
		/// Get the Blend that is identified by an integer value. This method is
		/// used when decoding a Blend from a Flash file.
		/// </summary>
		/// <param name="type">
		///            the integer value read from a Flash file.
		/// </param>
		/// <returns> the Blend identified by the integer value. </returns>


		public static Blend fromInt(int type)
		{
			return TABLE[type];
		}

		/// <summary>
		/// Integer value representing the Blend when it is encoded. </summary>
		private readonly int value;

		/// <summary>
		/// Private constructor used to create the table mapping integer
		/// values to different Blends.
		/// </summary>
		/// <param name="val"> the value representing the Blend when it is encoded. </param>


		private Blend(string name, InnerEnum innerEnum, int val)
		{
			value = val;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Get the integer value that is used to identify this Blend. This method is
		/// used when encoding a Blend in a Flash file.
		/// </summary>
		/// <returns> the integer value used to encode this Blend. </returns>
		public int Value => value;

	    public static IList<Blend> values()
		{
			return valueList;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static Blend valueOf(string name)
		{
			foreach (Blend enumInstance in valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new ArgumentException(name);
		}
	}

}