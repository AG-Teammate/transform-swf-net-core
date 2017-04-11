/*
 * SoundDecoder.java
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

using System;
using System.IO;
using com.flagstone.transform.sound;

namespace com.flagstone.transform.util.sound
{
    /// <summary>
	/// SoundDecoder is an interface that classes used to decode different sound
	/// formats should implement in order to be registered with the SoundRegistry.
	/// </summary>
	public interface SoundDecoder
	{
		/// <summary>
		/// Read a sound from a file. </summary>
		/// <param name="file"> the path to the file. </param>
		/// <exception cref="IOException"> if there is an error reading the sound data. </exception>
		/// <exception cref="Exception"> if the file contains an unsupported format. </exception>


		void read(FileInfo file);
		/// <summary>
		/// Read a sound from an input stream. </summary>
		/// <param name="stream"> the stream used to read the sound data. </param>
		/// <exception cref="IOException"> if there is an error reading the sound data. </exception>
		/// <exception cref="Exception"> if the file contains an unsupported format. </exception>


		void read(Stream stream);
		/// <summary>
		/// Define an event sound. </summary>
		/// <param name="identifier"> he unique identifier that will be used to reference the
		/// sound in a Movie. </param>
		/// <returns> the definition used to add the sound to a Movie. </returns>
		/// <exception cref="IOException"> if there is an error reading the sound data. </exception>
		/// <exception cref="Exception"> if the file contains an unsupported format. </exception>


		DefineSound defineSound(int identifier);
		/// <summary>
		/// Define an event sound. </summary>
		/// <param name="identifier"> the unique identifier that will be used to reference
		/// the sound in a Movie. </param>
		/// <param name="duration"> the number of seconds to play the sound for. </param>
		/// <returns> the definition used to add the sound to a Movie. </returns>
		/// <exception cref="IOException"> if there is an error reading the sound data. </exception>
		/// <exception cref="Exception"> if the file contains an unsupported format. </exception>


		DefineSound defineSound(int identifier, float duration);
		/// <summary>
		/// Generate the header for a streaming sound.
		/// </summary>
		/// <param name="frameRate"> the frame rate for the movie so the sound can be divided
		/// into sets of samples that can be played with each frame.
		/// </param>
		/// <returns> the stream header object that contains information about the
		/// sound. </returns>
		MovieTag streamHeader(float frameRate);

		/// <summary>
		/// Generate a SoundStreamBlock with next set of sound samples.
		/// </summary>
		/// <returns> a SoundStreamBlock containing the sound samples or null if there
		/// are no more samples to available. </returns>
		/// <exception cref="IOException"> if there is an error reading the sound data. </exception>
		/// <exception cref="Exception"> if the file contains an unsupported format. </exception>


		MovieTag streamSound();
	}

}