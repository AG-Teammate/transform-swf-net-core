using System;
using System.Collections.Generic;

/*
 * Language.java
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

namespace com.flagstone.transform.text
{

	/// <summary>
	/// Language is used to identify the spoken language for text (not the character
	/// encoding). It is primarily used to select the line-breaking rules when
	/// wrapping text in dynamic text fields.
	/// </summary>
	public sealed class Language
	{
		/// <summary>
		/// The spoken language will be defined by the Flash Player. </summary>
		public static readonly Language NONE = new Language("NONE", InnerEnum.NONE, 0);
		/// <summary>
		/// The spoken language for traditional Chinese fonts. </summary>
		public static readonly Language TRADITIONAL_CHINESE = new Language("TRADITIONAL_CHINESE", InnerEnum.TRADITIONAL_CHINESE, 5);
		/// <summary>
		/// The spoken language for simplified Chinese fonts. </summary>
		public static readonly Language SIMPLIFIED_CHINESE = new Language("SIMPLIFIED_CHINESE", InnerEnum.SIMPLIFIED_CHINESE, 4);
		/// <summary>
		/// The spoken language for Japanese fonts. </summary>
		public static readonly Language KOREAN = new Language("KOREAN", InnerEnum.KOREAN, 3);
		/// <summary>
		/// The spoken language for Korean fonts. </summary>
		public static readonly Language JAPANESE = new Language("JAPANESE", InnerEnum.JAPANESE, 2);
		/// <summary>
		/// The spoken language for Latin fonts. </summary>
		public static readonly Language LATIN = new Language("LATIN", InnerEnum.LATIN, 1);

		private static readonly IList<Language> valueList = new List<Language>();

		public enum InnerEnum
		{
			NONE,
			TRADITIONAL_CHINESE,
			SIMPLIFIED_CHINESE,
			KOREAN,
			JAPANESE,
			LATIN
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal;

		/// <summary>
		/// Table mapping code to keys. </summary>
		private static readonly IDictionary<int?, Language> TABLE = new Dictionary<int?, Language>();

		static Language()
		{
			foreach (Language type in values())
			{
				TABLE[type.value] = type;
			}

			valueList.Add(NONE);
			valueList.Add(TRADITIONAL_CHINESE);
			valueList.Add(SIMPLIFIED_CHINESE);
			valueList.Add(KOREAN);
			valueList.Add(JAPANESE);
			valueList.Add(LATIN);
		}

		/// <summary>
		/// Get the Language for an encoded value. </summary>
		/// <param name="code"> the encoded value representing a spoken language. </param>
		/// <returns> the Language for the encoded value. </returns>


		public static Language fromInt(int code)
		{
			return TABLE[code];
		}

		/// <summary>
		/// The value representing the Language when it is encoded. </summary>
		private readonly int value;

		/// <summary>
		/// Create a new Language. </summary>
		/// <param name="keyCode"> the value that is encoded to represent the key. </param>


		private Language(string name, InnerEnum innerEnum, int keyCode)
		{
			value = keyCode;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Get the value that will be encoded to represent the Language. </summary>
		/// <returns> the value that will be encoded. </returns>
		public int Value => value;

	    public static IList<Language> values()
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

		public static Language valueOf(string name)
		{
			foreach (Language enumInstance in valueList)
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