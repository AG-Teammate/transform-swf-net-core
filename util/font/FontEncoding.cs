using System;
using System.Collections.Generic;

/*
 * FontEncoding.java
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

namespace com.flagstone.transform.util.font
{
	/// <summary>
	/// FontEncoding describes the different fonts that can be decoded from a file.
	/// 
	/// Note: AWT fonts are not included as they are decoded directly from the AWT
	/// Font object and not from a file.
	/// </summary>
	internal sealed class FontEncoding
	{
		/// <summary>
		/// Font definitions stored in a Flash (.swf) file. </summary>
		public static readonly FontEncoding SWF = new FontEncoding("SWF", InnerEnum.SWF, "swf", new SWFFontDecoder());
		/// <summary>
		/// Font definitions stored in a TrueType (.ttf) or OpenType (.otf) file. </summary>
		public static readonly FontEncoding TTF = new FontEncoding("TTF", InnerEnum.TTF, "ttf", new TTFDecoder());

		private static readonly IList<FontEncoding> valueList = new List<FontEncoding>();

		static FontEncoding()
		{
			valueList.Add(SWF);
			valueList.Add(TTF);
		}

		public enum InnerEnum
		{
			SWF,
			TTF
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal;

		/// <summary>
		/// The identifier for the font format. </summary>
		private readonly string key;
		/// <summary>
		/// The FontProvider that can be used to decode the font format. </summary>
		private readonly FontProvider provider;

		/// <summary>
		/// Private constructor for the enum.
		/// </summary>
		/// <param name="type"> the string representing the font format. </param>
		/// <param name="fontProvider"> the FontProvider that can be used to decode the
		/// font format. </param>


		private FontEncoding(string name, InnerEnum innerEnum, string type, FontProvider fontProvider)
		{
			key = type;
			provider = fontProvider;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Get the identifier for the font format.
		/// </summary>
		/// <returns> the string identify the font format. </returns>
		public string Type => key;

	    /// <summary>
		/// Get the FontProvider that can be registered in the FontRegistry to
		/// decode the font.
		/// </summary>
		/// <returns> the FontProvider that can be used to decode font of the given
		/// format. </returns>
		public FontProvider Provider => provider;

	    public static IList<FontEncoding> values()
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

		public static FontEncoding valueOf(string name)
		{
			foreach (FontEncoding enumInstance in valueList)
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