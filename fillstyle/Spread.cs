using System;
using System.Collections.Generic;

/*
 * Spread.java
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
	/// The Spread describes how the gradient is used to fill the available shape
	/// when the area to be filled is larger than the area covered by the gradient.
	/// </summary>
	public sealed class Spread
	{
		/// <summary>
		/// The last colour of the gradient is used to fill the remaining area. </summary>
		public static readonly Spread PAD = new Spread("PAD", InnerEnum.PAD, 0);
		/// <summary>
		/// The gradient is reflected (repeatedly reversing the gradient) across
		/// the area to be filled.
		/// </summary>
		public static readonly Spread REFLECT = new Spread("REFLECT", InnerEnum.REFLECT, 0x40);
		/// <summary>
		/// The gradient is repeated across the area to be filled. </summary>
		public static readonly Spread REPEAT = new Spread("REPEAT", InnerEnum.REPEAT, 0xC0);

		private static readonly IList<Spread> valueList = new List<Spread>();

		public enum InnerEnum
		{
			PAD,
			REFLECT,
			REPEAT
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal;

		/// <summary>
		/// Table used to store instances of Spreads so only one object is
		/// created for each type decoded.
		/// </summary>
		private static readonly IDictionary<int?, Spread> TABLE = new Dictionary<int?, Spread>();

		static Spread()
		{
			foreach (Spread action in values())
			{
				TABLE[action.value] = action;
			}

			valueList.Add(PAD);
			valueList.Add(REFLECT);
			valueList.Add(REPEAT);
		}

		/// <summary>
		/// Returns the Spread for a given type.
		/// </summary>
		/// <param name="value">
		///            the type that identifies the Spread when it is encoded.
		/// </param>
		/// <returns> a shared instance of the object representing a given Spread type. </returns>


		public static Spread fromInt(int value)
		{
			return TABLE[value];
		}

		/// <summary>
		/// Type used to identify the Spread when it is encoded. </summary>
		private readonly int value;

		/// <summary>
		/// Constructor used to create instances for each type of Spread.
		/// </summary>
		/// <param name="spread"> the value representing the Spread when it is encoded. </param>


		private Spread(string name, InnerEnum innerEnum, int spread)
		{
			value = spread;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Get the value used to represent the Spread when it is encoded. </summary>
		/// <returns> the encoded value for the Spread. </returns>
		public int Value => value;

	    public static IList<Spread> values()
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

		public static Spread valueOf(string name)
		{
			foreach (Spread enumInstance in valueList)
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