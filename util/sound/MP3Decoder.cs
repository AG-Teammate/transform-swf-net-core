using System;
using System.IO;
using com.flagstone.transform.coder;
using com.flagstone.transform.sound;

/*
 * MP3Decoder.java
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
	/// Decoder for MP3 sounds so they can be added to a flash file.
	/// </summary>


	public sealed class MP3Decoder : SoundProvider, SoundDecoder
	{
		/// <summary>
		/// The bit mask to obtain the ID3 identifier. </summary>
		private const int ID3_MASK = unchecked((int)0xFFFFFF00);
		/// <summary>
		/// Value identifying ID3 Version 1 meta-data. </summary>
		private const int ID3_V1 = 0x54414700;
		/// <summary>
		/// The length of the meta-data block in ID3 Version 1. </summary>
		private const int ID3_V1_LENGTH = 128;
		/// <summary>
		/// Value identifying ID3 Version 2 meta-data. </summary>
		private const int ID3_V2 = 0x49443300;
		/// <summary>
		/// The number of bytes in the footer of an ID3 V2 meta-data block. </summary>
		private const int ID3_V2_FOOTER = 10;
		/// <summary>
		/// Mask to identify whether the header contains MP3 sync data. </summary>
		private const int MP3_SYNC = unchecked((int)0xFFE00000);

		/// <summary>
		/// The version number of the MPEG sound format. In this case 3 for MP3. </summary>
		private const int MPEG1 = 3;
		/// <summary>
		/// The number of samples in each frame according to the MPEG version. </summary>
		private static readonly int[] MP3_FRAME_SIZE = {576, 576, 576, 1152};
		/// <summary>
		/// The number of channels supported by each MP3 version. </summary>
		private static readonly int[] CHANNEL_COUNT = {2, 2, 2, 1};
		/// <summary>
		/// The bit rates for the different MPEG sound versions. </summary>
		private static readonly int[][] BIT_RATES = {
			new[] {-1, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, -1},
			new[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
			new[] {-1, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, -1},
			new[] {-1, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, -1}
		};

		/// <summary>
		/// The playback rates for the different MPEG sound versions. </summary>
		private static readonly int[][] SAMPLE_RATES = {
			new[] {11025, -1, -1, -1},
			new[] {-1, -1, -1, -1},
			new[] {22050, -1, -1, -1},
			new[] {44100, -1, -1, -1}
		};
		/// <summary>
		/// The number of bytes in each sample. </summary>
		private const int SAMPLE_SIZE = 2;

		/// <summary>
		/// The frame rate of the movie where the MP3 sound will be played. </summary>
		
		private float movieRate;
		/// <summary>
		/// The number of sound channels: 1 - mono, 2 - stereo. </summary>
		
		private int numberOfChannels;
		/// <summary>
		/// The number of sound samples for each channel. </summary>
		
		private int samplesPerChannel;
		/// <summary>
		/// The rate at which the sound will be played. </summary>
		
		private int sampleRate;
		/// <summary>
		/// The sound samples. </summary>
		
		private byte[] sound;

		/// <summary>
		/// The decoder used to read the MP3 frames. </summary>
		
		private BigDecoder coder;

		/// <summary>
		/// The number of sound samples in each frame. </summary>
		
		private int samplesPerFrame;
		/// <summary>
		/// The contents of the current MP3 frame. </summary>
		
		private byte[] frame;
		/// <summary>
		/// Actual number of samples streamed so far. </summary>
		
		private int actualSamples;
		/// <summary>
		/// Expected number of samples streamed based on the movie frame rate. </summary>
		
		private int expectedSamples;

		/// <summary>
		/// {@inheritDoc} </summary>
		public SoundDecoder newDecoder()
		{
			return new MP3Decoder();
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void read(FileInfo file)
		{


			FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			try
			{
				read(stream);
			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public void read(Stream stream)
		{
			coder = new BigDecoder(stream);
			readFrame();
			actualSamples += samplesPerFrame;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public DefineSound defineSound(int identifier)
		{

			int length;
			sound = new byte[2];

			do
			{
				length = sound.Length;
				sound = Arrays.copyOf(sound, length + frame.Length);
				Array.Copy(frame, 0, sound, length, frame.Length);
			} while (readFrame());

			return new DefineSound(identifier, SoundFormat.MP3, sampleRate, numberOfChannels, SAMPLE_SIZE, samplesPerChannel, sound);
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public DefineSound defineSound(int identifier, float duration)
		{

			sound = new byte[2];
			float played = 0;
			int length;

			while (played < duration)
			{
				length = sound.Length;
				sound = Arrays.copyOf(sound, length + frame.Length);
				Array.Copy(frame, 0, sound, length, frame.Length);
				played += samplesPerFrame / (float) sampleRate;
				if (!readFrame())
				{
					break;
				}
			}

			return new DefineSound(identifier, SoundFormat.MP3, sampleRate, numberOfChannels, SAMPLE_SIZE, samplesPerChannel, sound);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public MovieTag streamHeader(float frameRate)
		{
			movieRate = frameRate;
			return new SoundStreamHead2(SoundFormat.MP3, sampleRate, numberOfChannels, SAMPLE_SIZE, sampleRate, numberOfChannels, SAMPLE_SIZE, (int)(sampleRate / frameRate));
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public MovieTag streamSound()
		{


			int seek = expectedSamples > 0 ? actualSamples - expectedSamples : 0;

			expectedSamples += (int)(sampleRate / movieRate);
			sound = new byte[4];
			int sampleCount = 0;
			bool hasFrames = true;
			int length;
			do
			{
				length = sound.Length;
				sound = Arrays.copyOf(sound, length + frame.Length);
				Array.Copy(frame, 0, sound, length, frame.Length);
				sampleCount += samplesPerFrame;
				hasFrames = readFrame();
				actualSamples += samplesPerFrame;
			} while (hasFrames && (actualSamples < expectedSamples));

			SoundStreamBlock block = null;

			if (hasFrames)
			{
				sound[0] = (byte) sampleCount;
				sound[1] = (byte)(sampleCount >> Coder.TO_LOWER_BYTE);
				sound[2] = (byte) seek;
				sound[3] = (byte)(seek >> Coder.TO_LOWER_BYTE);

				block = new SoundStreamBlock(sound);
			}
			return block;
		}

		/// <summary>
		/// Read a MP3 frame. </summary>
		/// <returns> true if a frame was read. </returns>
		/// <exception cref="IOException"> if there is an error reading the data. </exception>
		/// <exception cref="Exception"> if the sound is not in MP3 format. </exception>


		private bool readFrame()
		{
			bool frameRead = false;
			int header;
			while ((!coder.eof()) && !frameRead)
			{
				header = coder.scanInt();
				if (header == -1)
				{
					coder.readUnsignedShort();
				}
				else if ((header & ID3_MASK) == ID3_V1)
				{
					readID3V1();
				}
				else if ((header & ID3_MASK) == ID3_V2)
				{
					readID3V2();
				}
				else if ((header & MP3_SYNC) == MP3_SYNC)
				{
					readFrame(header);
					frameRead = true;
				}
				else
				{
					coder.readUnsignedShort();
				}
			}
			return !coder.eof();
		}

		/// <summary>
		/// Read the ID3 V1 meta-data. </summary>
		/// <exception cref="IOException"> if there is an error reading the data. </exception>
		/// <exception cref="Exception"> if the ID3 V1 header cannot be decoded. </exception>


		private void readID3V1()
		{
			coder.skip(ID3_V1_LENGTH);
		}

		/// <summary>
		/// Read the ID3 V2 meta-data. </summary>
		/// <exception cref="IOException"> if there is an error reading the data. </exception>
		/// <exception cref="Exception"> if the ID3 V2 header cannot be decoded. </exception>


		private void readID3V2()
		{
			coder.readByte(); // I
			coder.readByte(); // D
			coder.readByte(); // 3
			coder.readByte(); // major version
			coder.readByte(); // minor version

			int length;


			int flags = coder.readByte();

			if ((flags & Coder.BIT4) == 0)
			{
				length = 0;
			}
			else
			{
				length = ID3_V2_FOOTER;
			}
			length += coder.readByte() << 21;
			length += coder.readByte() << 14;
			length += coder.readByte() << 7;
			length += coder.readByte();
			coder.skip(length);
		}

		/// <summary>
		/// Read an MP3 sync frame. </summary>
		/// <param name="header"> the header tag containing the MP3 data. </param>
		/// <exception cref="IOException"> if there is an error reading the data. </exception>
		/// <exception cref="Exception"> if the header cannot be decoded. </exception>



		private void readFrame(int header)
		{



			int version = (header & 0x180000) >> 19;


			int layer = (header & 0x060000) >> 17;
			//boolean hasCRC = (header & 0x010000) != 0;
			samplesPerFrame = MP3_FRAME_SIZE[version];


			int bitRate = BIT_RATES[version][(header & Coder.NIB3) >> Coder.ALIGN_NIB3];
			sampleRate = SAMPLE_RATES[version][(header & 0x0C00) >> 10];


			int padding = (header & 0x0200) >> 9;
			//int reserved = (header & 0x0100) >> 8;

			if (layer != 1)
			{
				throw new Exception("Flash only supports MPEG Layer 3");
			}

			if (bitRate == -1)
			{
				throw new Exception("Unsupported Bit-rate");
			}

			if (sampleRate == -1)
			{
				throw new Exception("Unsupported Sampling-rate");
			}

			numberOfChannels = CHANNEL_COUNT[(header & Coder.PAIR3) >> 6];
			samplesPerChannel += samplesPerFrame;



			int frameSize = 4 + (((version == MPEG1) ? 144 : 72) * bitRate * 1000 / sampleRate + padding) - 4;

			frame = coder.readBytes(new byte[frameSize]);
		}
	}

}