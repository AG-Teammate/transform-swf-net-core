using System;
using System.Collections.Generic;

/*
 * Interpolation.java
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
	/// Interpolation describes how the transition between colours in a gradient are
	/// calculated.
	/// </summary>
	public sealed class Interpolation
	{
		/// <summary>
		/// Normal RGBA interpolation. </summary>
		public static readonly Interpolation NORMAL = new Interpolation("NORMAL", InnerEnum.NORMAL, 0);
		/// <summary>
		/// Linear RGBA interpolation. </summary>
		public static readonly Interpolation LINEAR = new Interpolation("LINEAR", InnerEnum.LINEAR, 16);

		private static readonly IList<Interpolation> valueList = new List<Interpolation>();

		public enum InnerEnum
		{
			NORMAL,
			LINEAR
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal;

		/// <summary>
		/// Table used to store instances of Interpolation so only one object is
		/// created for each type.
		/// </summary>
		private static readonly IDictionary<int?, Interpolation> TABLE = new Dictionary<int?, Interpolation>();

		static Interpolation()
		{
			foreach (Interpolation action in values())
			{
				TABLE[action.value] = action;
			}

			valueList.Add(NORMAL);
			valueList.Add(LINEAR);
		}

		/// <summary>
		/// Returns the Interpolation for a given type.
		/// </summary>
		/// <param name="value">
		///            the type that identifies the Interpolation when it is encoded.
		/// </param>
		/// <returns> a shared instance of the object representing a given
		/// Interpolation. </returns>


		public static Interpolation fromInt(int value)
		{
			return TABLE[value];
		}

		/// <summary>
		/// Type used to identify the Interpolation when it is encoded. </summary>
		private readonly int value;

		/// <summary>
		/// Constructor used to create instances for each type of Interpolation.
		/// </summary>
		/// <param name="spread"> the value representing the Interpolation when it is
		/// encoded. </param>


		private Interpolation(string name, InnerEnum innerEnum, int spread)
		{
			value = spread;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Get the value used to represent the Interpolation when it is encoded. </summary>
		/// <returns> the encoded value for the Interpolation. </returns>
		public int Value => value;

	    public static IList<Interpolation> values()
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

		public static Interpolation valueOf(string name)
		{
			foreach (Interpolation enumInstance in valueList)
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