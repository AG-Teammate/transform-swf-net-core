using System;
using System.Collections.Generic;

/*
 * FontRegistry.java
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
	/// FontRegistry is used to provide a directory for registering FontProviders
	/// that are used to decode different font formats.
	/// </summary>
	public sealed class FontRegistry
	{

		/// <summary>
		/// The table of font providers used to decode each supported format. </summary>
		private static IDictionary<string, FontProvider> providers = new Dictionary<string, FontProvider>();

		static FontRegistry()
		{
			foreach (FontEncoding encoding in FontEncoding.values())
			{
				registerProvider(encoding.Type, encoding.Provider);
			}
		}

		/// <summary>
		/// Register a FontDecoder to handle fonts in the specified format. Currently
		/// Java AWT, TrueType/OpenType and Flash fonts are supported.
		/// </summary>
		/// <param name="mimeType">
		///            the string identifying the image format. </param>
		/// <param name="decoder">
		///            any class that implements the FontDecoder interface. </param>


		public static void registerProvider(string mimeType, FontProvider decoder)
		{
			providers[mimeType] = decoder;
		}

		/// <summary>
		/// Get the provider that can be used to decode a given font format. </summary>
		/// <param name="type"> string defined in FontEncoding identifying the font format. </param>
		/// <returns> an object implementing the ImageDecoder interface that can be
		/// used to decode the image data. </returns>


		public static FontDecoder getFontProvider(string type)
		{
		    if (providers.ContainsKey(type))
			{
				return providers[type].newDecoder();
			}
		    throw new ArgumentException();
		}

		/// <summary>
		/// Private constructor for the image registry. </summary>
		private FontRegistry()
		{
			// Registry is shared.
		}
	}

}