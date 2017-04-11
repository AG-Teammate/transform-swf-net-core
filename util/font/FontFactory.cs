using System;
using System.Collections.Generic;
using System.IO;

/*
 * FontFactory.java
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
	/// ImageFactory is used to generate an image definition object from an image
	/// stored in a file, references by a URL or read from an stream. An plug-in
	/// architecture allows decoders to be registered to handle different image
	/// formats. The ImageFactory provides a standard interface for using the
	/// decoders.
	/// </summary>
	public sealed class FontFactory
	{
		/// <summary>
		/// The object used to decode the font. </summary>
		
		private FontDecoder decoder;

		/// <summary>
		/// Read a font stored in the specified file.
		/// </summary>
		/// <param name="file">
		///            a file containing the abstract path to the font.
		/// </param>
		/// <exception cref="IOException">
		///             if there is an error reading the file.
		/// </exception>
		/// <exception cref="Exception">
		///             if there is a problem decoding the font, either it is in an
		///             unsupported format or an error occurred while decoding the
		///             data. </exception>



		public void read(FileInfo file)
		{

			string fontType;

			if (file.Name.EndsWith("ttf"))
			{
				fontType = "ttf";
			}
			else if (file.Name.EndsWith("swf"))
			{
				fontType = "swf";
			}
			else
			{
				throw new Exception("Unsupported format");
			}

			decoder = FontRegistry.getFontProvider(fontType);
			decoder.read(file);
		}

		/// <summary>
		/// Get the list of fonts decoded. </summary>
		/// <returns> a list containing a Font object for each font decoded. </returns>
		public IList<Font> Fonts => decoder.Fonts;
	}

}