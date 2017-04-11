using System;
using System.Collections.Generic;

/*
 * SoundEncoding.java
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
	/// SoundEncoding describes the different sound formats that can be decoded and
	/// added to a Flash movie.
	/// </summary>
	internal sealed class SoundEncoding
	{
		/// <summary>
		/// MPEG Version 3 (MP3) format. </summary>
		public static readonly SoundEncoding MP3 = new SoundEncoding("MP3", InnerEnum.MP3, "audio/mpeg", new MP3Decoder());
		/// <summary>
		/// Waveform Audio File Format. </summary>
		public static readonly SoundEncoding WAV = new SoundEncoding("WAV", InnerEnum.WAV, "audio/x-wav", new WAVDecoder());

		private static readonly IList<SoundEncoding> valueList = new List<SoundEncoding>();

		static SoundEncoding()
		{
			valueList.Add(MP3);
			valueList.Add(WAV);
		}

		public enum InnerEnum
		{
			MP3,
			WAV
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal;

		/// <summary>
		/// The MIME type used to identify the sound format. </summary>
		private readonly string mimeType;
		/// <summary>
		/// The SoundProvider that can be used to decode the sound format. </summary>
		private readonly SoundProvider provider;

		/// <summary>
		/// Private constructor for the enum.
		/// </summary>
		/// <param name="type"> the string representing the mime-type. </param>
		/// <param name="soundProvider"> the SoundProvider that can be used to decode the
		/// sound format. </param>


		private SoundEncoding(string name, InnerEnum innerEnum, string type, SoundProvider soundProvider)
		{
			mimeType = type;
			provider = soundProvider;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Get the mime-type used to represent the sound format.
		/// </summary>
		/// <returns> the string identifying the sound format. </returns>
		public string MimeType => mimeType;

	    /// <summary>
		/// Get the SoundProvider that can be registered in the SoundRegistry to
		/// decode the sound.
		/// </summary>
		/// <returns> the SoundProvider that can be used to decode sounds of the given
		/// mime-type. </returns>
		public SoundProvider Provider => provider;

	    public static IList<SoundEncoding> values()
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

		public static SoundEncoding valueOf(string name)
		{
			foreach (SoundEncoding enumInstance in valueList)
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