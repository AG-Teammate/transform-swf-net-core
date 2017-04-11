using System;
using System.Collections.Generic;

/*
 * SoundRegistry.java
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

namespace com.flagstone.transform.util.sound
{

	/// <summary>
	/// SoundRegistry is used to provide a directory for registering SoundProviders
	/// that are used to decode different sound formats.
	/// </summary>
	public sealed class SoundRegistry
	{

		/// <summary>
		/// Table of decoders for the different sound formats supported. </summary>
		private static IDictionary<string, SoundProvider> providers = new Dictionary<string, SoundProvider>();

		static SoundRegistry()
		{
			foreach (SoundEncoding encoding in SoundEncoding.values())
			{
				registerProvider(encoding.MimeType, encoding.Provider);
			}
		}

		/// <summary>
		/// Register an SoundDecoder to handle images in the specified format.
		/// </summary>
		/// <param name="mimeType">
		///            the string identifying the image format. </param>
		/// <param name="decoder">
		///            any class that implements the SoundDecoder interface. </param>


		public static void registerProvider(string mimeType, SoundProvider decoder)
		{
			providers[mimeType] = decoder;
		}

		/// <summary>
		/// Get an instance of the class that can be used to decode a sound of the
		/// specified format. </summary>
		/// <param name="mimeType"> the string representing the format. </param>
		/// <returns> an decoder that can be used to decode the sound data. </returns>


		public static SoundDecoder getSoundProvider(string mimeType)
		{
		    if (providers.ContainsKey(mimeType))
			{
				return providers[mimeType].newDecoder();
			}
		    throw new ArgumentException();
		}

		/// <summary>
		/// Private constructor for the singleton registry.
		/// </summary>
		private SoundRegistry()
		{
			// Registry is shared.
		}
	}

}