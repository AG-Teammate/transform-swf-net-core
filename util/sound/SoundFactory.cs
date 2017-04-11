using System;
using System.IO;
using com.flagstone.transform.sound;

/*
 * SoundFactory.java
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
	/// SoundFactory is used to generate the objects representing an event or
	/// streaming sound from an sound stored in a file, references by a URL or read
	/// from an stream. An plug-in architecture allows decoders to be registered to
	/// handle different formats. The SoundFactory provides a standard interface
	/// for using the different decoders for each supported sound format.
	/// </summary>
	public sealed class SoundFactory
	{
		/// <summary>
		/// The object used to decode the sound. </summary>
		
		private SoundDecoder decoder;

		/// <summary>
		/// Decode a sound located in the specified file.
		/// </summary>
		/// <param name="file">
		///            a file containing the abstract path to the sound.
		/// </param>
		/// <exception cref="IOException">
		///             if there is an error reading the file.
		/// </exception>
		/// <exception cref="Exception">
		///             if there is a problem decoding the sound, either it is in an
		///             unsupported format or an error occurred while decoding the
		///             sound data. </exception>



		public void read(FileInfo file)
		{

			string mimeType;

			if (file.Name.EndsWith("wav"))
			{
				mimeType = "audio/x-wav";
			}
			else if (file.Name.EndsWith("mp3"))
			{
				mimeType = "audio/mpeg";
			}
			else
			{
				throw new Exception("Unsupported format");
			}

			decoder = SoundRegistry.getSoundProvider(mimeType);
			decoder.read(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
		}

		/// <summary>
		/// Decode a sound from a stream.
		/// </summary>
		/// <param name="stream">
		///            the InputStream containing the sound data.
		/// </param>
		/// <exception cref="IOException">
		///             if there is an error reading the stream.
		/// </exception>
		/// <exception cref="Exception">
		///             if there is a problem decoding the sound, either it is in an
		///             unsupported format or an error occurred while decoding the
		///             sound data. </exception>



		public void read(Stream stream)
		{
			decoder.read(stream);
		}

		/// <summary>
		/// Create a definition for an event sound that can be added to a Flash
		/// movie.
		/// </summary>
		/// <param name="identifier"> the unique identifier for the sound.
		/// </param>
		/// <returns> a DefineSound object containing the image definition.
		/// </returns>
		/// <exception cref="IOException">
		///             if there is an error reading the file.
		/// </exception>
		/// <exception cref="Exception">
		///             if there is a problem decoding the sound, either it is in an
		///             unsupported format or an error occurred while decoding the
		///             sound data. </exception>



		public DefineSound defineSound(int identifier)
		{
			return decoder.defineSound(identifier);
		}

		/// <summary>
		/// Create a definition for an event sound that can be added to a Flash
		/// movie.
		/// </summary>
		/// <param name="identifier"> the unique identifier for the sound.
		/// </param>
		/// <param name="duration"> the number of seconds to play the sound for.
		/// </param>
		/// <returns> a DefineSound object containing the image definition.
		/// </returns>
		/// <exception cref="IOException">
		///             if there is an error reading the file.
		/// </exception>
		/// <exception cref="Exception">
		///             if there is a problem decoding the sound, either it is in an
		///             unsupported format or an error occurred while decoding the
		///             sound data. </exception>



		public DefineSound defineSound(int identifier, float duration)
		{
			return decoder.defineSound(identifier, duration);
		}

		/// <summary>
		/// Generate the header for a streaming sound.
		/// </summary>
		/// <param name="frameRate"> the frame rate for the movie so the sound can be divided
		/// into sets of samples that can be played with each frame.
		/// </param>
		/// <returns> a SoundStreamHead2 object that defines the streaming sound. </returns>


		public MovieTag streamHeader(float frameRate)
		{
			return decoder.streamHeader(frameRate);
		}

		/// <summary>
		/// Generate a SoundStreamBlock with next set of sound samples.
		/// </summary>
		/// <returns> a SoundStreamBlock containing the sound samples or null if there
		/// are no more samples to available. </returns>
		/// <exception cref="IOException"> if there is an error reading the sound data. </exception>
		/// <exception cref="Exception"> if the file contains an unsupported format. </exception>


		public MovieTag streamSound()
		{
			return decoder.streamSound();

		}
	}

}