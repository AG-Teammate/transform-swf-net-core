using System;

/*
 * FontFace.java
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
	/// FontFace is a convenience class that can be used to create tables of
	/// fonts for use in an application.
	/// </summary>
	public sealed class FontFace
	{
		/// <summary>
		/// The family name of the font. </summary>
		
		private readonly string name;
		/// <summary>
		/// Is the font bold. </summary>
		
		private readonly bool bold;
		/// <summary>
		/// Is the font italicised. </summary>
		
		private readonly bool italic;

		/// <summary>
		/// Create a new font face.
		/// </summary>
		/// <param name="fontName"> the family name of the font. </param>
		/// <param name="isBold"> is the font bold. </param>
		/// <param name="isItalic"> is the font italicised. </param>


		public FontFace(string fontName, bool isBold, bool isItalic)
		{
			name = fontName;
			bold = isBold;
			italic = isItalic;
		}

		/// <summary>
		/// Create a new font face.
		/// </summary>
		/// <param name="fontName"> the family name of the font. </param>
		/// <param name="style"> a java.awt.Font constant describing whether the font is
		/// normal, bold and/or italicised. </param>


		public FontFace(string fontName, int style)
		{
			name = fontName;
			bold = (style & 1) != 0;
			italic = (style & 2) != 0;
		}

		/// <summary>
		/// Get the (family) name of the font. </summary>
		/// <returns> the family name of the font, e.g. Arial </returns>
		public string Name => name;

	    /// <summary>
		/// Is the font bold or normal. </summary>
		/// <returns> true if the font is bold, false if the font has normal weight. </returns>
		public bool Bold => bold;

	    /// <summary>
		/// Is the font italicised or normal.
		/// </summary>
		/// <returns> true if the font is italicised, false if the font has normal
		/// weight. </returns>
		public bool Italic => italic;

	    /// <summary>
		/// Get the font style, as defined in the AWT Font class, either Font.PLAIN
		/// or a combination of Font.BOLD and Font.ITALIC.
		/// </summary>
		/// <returns> the java.awt.Font constant defining the font style. </returns>
		public int Style
		{
			get
			{
				int style = 0;
    
				if (bold)
				{
					style |= 1;
				}
				else if (italic)
				{
					style |= 2;
				}
    
				return style;
			}
		}

		/// <summary>
		/// Get name of the font face which contains the name of the font
		/// followed by "Bold" for bold fonts and "Italic" for fonts with an italic
		/// style.
		/// </summary>
		/// <returns> a human readable string describing the font. </returns>
		public override string ToString()
		{
			return name + (bold ? " Bold" : "") + (italic ? " Italic" : "");
		}



		public override bool Equals(object @object)
		{
			bool result;
			FontFace key;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is FontFace)
			{
				key = (FontFace) @object;
				result = name.Equals(key.name) && bold == key.bold && italic == key.italic;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return (name.GetHashCode() * Constants.PRIME + Convert.ToBoolean(bold).GetHashCode()) * Constants.PRIME + Convert.ToBoolean(italic).GetHashCode();
		}
	}

}