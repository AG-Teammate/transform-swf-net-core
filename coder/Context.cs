using System;
using System.Collections.Generic;

/*
 * Context.java
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

namespace com.flagstone.transform.coder
{

	/// <summary>
	/// Contexts are used to pass information between objects when they are being
	/// encoded or decoded.
	/// </summary>
	public class Context
	{
		/// <summary>
		/// Flash Version. </summary>
		public const int VERSION = 1;
		/// <summary>
		/// Type identifying the current MovieTag being decoded. </summary>
		public const int TYPE = 2;
		/// <summary>
		/// Whether the alpha channel should be encoded / decoded. </summary>
		public const int TRANSPARENT = 3;
		/// <summary>
		/// Character codes are 16-bits. </summary>
		public const int WIDE_CODES = 4;
		/// <summary>
		/// Arrays of fill or line styles can contain more than 255 entries. </summary>
		public const int ARRAY_EXTENDED = 8;
		/// <summary>
		/// The glyphs were derived from a Postscript font. </summary>
		public const int POSTSCRIPT = 9;
		/// <summary>
		/// The line styles define a scaling stroke. </summary>
		public const int SCALING_STROKE = 10;
		/// <summary>
		/// The number of bit used to encode a fill style selection. </summary>
		public const int FILL_SIZE = 11;
		/// <summary>
		/// The number of bit used to encode a line style selection. </summary>
		public const int LINE_SIZE = 12;
		/// <summary>
		/// The number of bit used to encode a glyph advance. </summary>
		public const int ADVANCE_SIZE = 13;
		/// <summary>
		/// The number of bit used to encode a glyph index. </summary>
		public const int GLYPH_SIZE = 14;
		/// <summary>
		/// The number of bits used to encode a given shape. </summary>
		public const int SHAPE_SIZE = 15;
		/// <summary>
		/// Indicates that this is the last EventHandler to be encoded/decoded. </summary>
		public const int LAST = 16;
		/// <summary>
		/// Indicates the flash file is compressed. </summary>
		public const int COMPRESSED = 17;
		/// <summary>
		/// Indicates a definition is for menu button. </summary>
		public const int MENU_BUTTON = 18;

		/// <summary>
		/// The character encoding used for strings. </summary>
		private string encoding;
		/// <summary>
		/// The registry containing the objects that perform the decoding. </summary>
		private DecoderRegistry registry;
		/// <summary>
		/// A table of variables used to pass information between objects. </summary>
		
		private readonly IDictionary<int?, int?> variables;

		/// <summary>
		/// Create a Context object.
		/// </summary>
		public Context()
		{
			encoding = CharacterEncoding.UTF8.ToString();
			variables = new Dictionary<int?, int?>();
		}

		/// <summary>
		/// Get character encoding scheme used when encoding or decoding strings.
		/// </summary>
		/// <returns> the character encoding used for strings. </returns>
		public string Encoding
		{
			get => encoding;
		    set => encoding = value;
		}


		/// <summary>
		/// Calculates the length of a string when encoded using the specified
		/// character set.
		/// </summary>
		/// <param name="string">
		///            the string to be encoded.
		/// </param>
		/// <returns> the number of bytes required to encode the string plus 1 for a
		///         terminating null character. </returns>



		public int strlen(string @string)
		{
			try
			{
				return @string.GetBytes(encoding).Length + 1;
			}


			catch (Exception e)
			{
			    throw e;
			}
		}

		/// <summary>
		/// Get the registry containing the decoders for different types of objects. </summary>
		/// <returns> the decoder registry. </returns>
		public DecoderRegistry Registry
		{
			get => registry;
		    set => registry = value;
		}


		/// <summary>
		/// Is a variable set. </summary>
		/// <param name="key"> the name of the variable. </param>
		/// <returns> true if the variable is set, false if not. </returns>


		public bool contains(int? key)
		{
			return variables.ContainsKey(key);
		}

		/// <summary>
		/// Delete the context variable.
		/// </summary>
		/// <param name="key"> the identifier for the variable. </param>


		public void remove(int? key)
		{
			variables.Remove(key);
		}

		/// <summary>
		/// Get the value of a variable. </summary>
		/// <param name="key"> the name of the variable. </param>
		/// <returns> the variable value. </returns>


		public int get(int? key)
		{
			return (int) variables[key];
		}

		/// <summary>
		/// Set a variable. </summary>
		/// <param name="key"> the name of the variable. </param>
		/// <param name="value"> the variable value. </param>
		/// <returns> this object. </returns>


		public Context put(int? key, int? value)
		{
			variables[key] = value;
			return this;
		}
	}

}