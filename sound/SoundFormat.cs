using System;
using System.Collections.Generic;

/*
 * SoundFormat.java
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
namespace com.flagstone.transform.sound
{

	/// <summary>
	/// SoundFormat is used to identify the different encoding formats used for event
	/// and streaming sounds in Flash and Flash Video files.
	/// </summary>
	public sealed class SoundFormat
	{
		/// <summary>
		/// Uncompressed Pulse Code Modulated: samples are either 1 or 2 bytes. For
		/// two-byte samples the byte order is dependent on the platform on which the
		/// Flash Player is hosted. Sounds created on a platform which support
		/// big-endian byte order will not be played correctly when listened to on a
		/// platform which supports little-endian byte order.
		/// </summary>
		public static readonly SoundFormat NATIVE_PCM = new SoundFormat("NATIVE_PCM", InnerEnum.NATIVE_PCM, 0);
		/// <summary>
		/// Compressed ADaptive Pulse Code Modulated: samples are encoded and
		/// compressed by comparing the difference between successive sound sample
		/// which dramatically reduces the size of the encoded sound when compared to
		/// the uncompressed PCM formats. Use this format or MP3 whenever possible.
		/// </summary>
		public static readonly SoundFormat ADPCM = new SoundFormat("ADPCM", InnerEnum.ADPCM, 1);
		/// <summary>
		/// Compressed MPEG Audio Layer-3.
		/// 
		/// </summary>
		public static readonly SoundFormat MP3 = new SoundFormat("MP3", InnerEnum.MP3, 2);
		/// <summary>
		/// Uncompressed pulse code modulated sound. Samples are either 1 or 2 bytes.
		/// The byte ordering for 16-bit samples is little-endian.
		/// </summary>
		public static readonly SoundFormat PCM = new SoundFormat("PCM", InnerEnum.PCM, 3);
		/// <summary>
		/// Compressed Nellymoser Asao format for a mono sound played at 8KHz
		/// supporting low bit-rate sound for improved synchronisation between the
		/// sound and frame rate of movies. This format is not supported in SWF
		/// files, only in Flash Video files.
		/// </summary>
		public static readonly SoundFormat NELLYMOSER_8K = new SoundFormat("NELLYMOSER_8K", InnerEnum.NELLYMOSER_8K, 5);
		/// <summary>
		/// Compressed Nellymoser Asao format supporting low bit-rate sound for
		/// improved synchronisation between the sound and frame rate of movies. This
		/// format is for mono sounds.
		/// </summary>
		public static readonly SoundFormat NELLYMOSER = new SoundFormat("NELLYMOSER", InnerEnum.NELLYMOSER, 6);
		/// <summary>
		/// The Open Source SPEEX sound format.
		/// </summary>
		public static readonly SoundFormat SPEEX = new SoundFormat("SPEEX", InnerEnum.SPEEX, 11);

		private static readonly IList<SoundFormat> valueList = new List<SoundFormat>();

		public enum InnerEnum
		{
			NATIVE_PCM,
			ADPCM,
			MP3,
			PCM,
			NELLYMOSER_8K,
			NELLYMOSER,
			SPEEX
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal;

		/// <summary>
		/// Table used to convert codes into SoundFormats. </summary>
		private static readonly IDictionary<int?, SoundFormat> TABLE = new Dictionary<int?, SoundFormat>();

		static SoundFormat()
		{
			foreach (SoundFormat type in values())
			{
				TABLE[type.value] = type;
			}

			valueList.Add(NATIVE_PCM);
			valueList.Add(ADPCM);
			valueList.Add(MP3);
			valueList.Add(PCM);
			valueList.Add(NELLYMOSER_8K);
			valueList.Add(NELLYMOSER);
			valueList.Add(SPEEX);
		}

		/// <summary>
		/// Get the SoundFormat represented by an encoded value. </summary>
		/// <param name="value"> that represents the SoundFormat when encoded in a Flash
		/// file. </param>
		/// <returns> the corresponding SoundFormat. </returns>


		public static SoundFormat fromInt(int value)
		{
			return TABLE[value];
		}
		/// <summary>
		/// Code used to represent the SoundFormat. </summary>
		private int value;

		/// <summary>
		/// Private constructor for enum. </summary>
		/// <param name="format"> the code representing the SoundFormat. </param>


		private SoundFormat(string name, InnerEnum innerEnum, int format)
		{
			value = format;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Get the value used to represent the SoundFormat when it is encoded. </summary>
		/// <returns> the value used to encode the SoundFormat. </returns>
		public int Value => value;

	    public static IList<SoundFormat> values()
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

		public static SoundFormat valueOf(string name)
		{
			foreach (SoundFormat enumInstance in valueList)
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