using System;
using System.IO;
using System.Net.Http;
using com.flagstone.transform.coder;
using com.flagstone.transform.sound;

/*
 * WAVDecoder.java
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
    /// Decoder for WAV sounds so they can be added to a flash file.
    /// </summary>
    public sealed class WAVDecoder : SoundProvider, SoundDecoder
    {

        /// <summary>
        /// The binary signature for xIFF files. </summary>
        private static readonly int[] RIFF = { 82, 73, 70, 70 };
        /// <summary>
        /// The binary signature for WAV files. </summary>
        private static readonly int[] WAV = { 87, 65, 86, 69 };
        /// <summary>
        /// The identifier of a format block. </summary>
        private const int FMT = 0x20746d66;
        /// <summary>
        /// The identifier of a data block. </summary>
        private const int DATA = 0x61746164;

        /// <summary>
        /// The sound format. </summary>

        private SoundFormat format;
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
        /// The number of bytes in each sample. </summary>

        private int sampleSize;
        /// <summary>
        /// The sound samples. </summary>

        private byte[] sound;

        /// <summary>
        /// The frame rate for the movie. </summary>

        private float movieRate;
        /// <summary>
        /// The number of bytes already streamed. </summary>

        private int bytesSent;

        /// <summary>
        /// {@inheritDoc} </summary>
        public SoundDecoder newDecoder()
        {
            return new WAVDecoder();
        }

        /// <summary>
        /// {@inheritDoc} </summary>



        public void read(FileInfo file)
        {
            read(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }

        /// <summary>
        /// {@inheritDoc} </summary>


        public DefineSound defineSound(int identifier)
        {
            return new DefineSound(identifier, format, sampleRate, numberOfChannels, sampleSize, samplesPerChannel, sound);
        }

        /// <summary>
        /// {@inheritDoc} </summary>


        public DefineSound defineSound(int identifier, float duration)
        {
            return new DefineSound(identifier, format, sampleRate, numberOfChannels, sampleSize, samplesPerChannel, sound);
        }

        /// <summary>
        /// {@inheritDoc} </summary>


        public MovieTag streamHeader(float frameRate)
        {
            movieRate = frameRate;
            return new SoundStreamHead2(format, sampleRate, numberOfChannels, sampleSize, sampleRate, numberOfChannels, sampleSize, (int)(sampleRate / frameRate));
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public MovieTag streamSound()
        {


            int samplesPerBlock = (int)(sampleRate / movieRate);


            int bytesPerBlock = samplesPerBlock * sampleSize * numberOfChannels;

            SoundStreamBlock block = null;

            if (bytesSent < sound.Length)
            {


                int bytesRemaining = sound.Length - bytesSent;


                int numberOfBytes = (bytesRemaining < bytesPerBlock) ? bytesRemaining : bytesPerBlock;



                byte[] bytes = new byte[numberOfBytes];
                Array.Copy(sound, bytesSent, bytes, 0, numberOfBytes);

                block = new SoundStreamBlock(bytes);
                bytesSent += numberOfBytes;
            }
            return block;
        }

        /// <summary>
        /// {@inheritDoc} </summary>



        public void read(Stream stream)
        {



            LittleDecoder coder = new LittleDecoder(stream);

            for (int i = 0; i < RIFF.Length; i++)
            {
                if (coder.readByte() != RIFF[i])
                {
                    throw new Exception("Unsupported format");
                }
            }

            coder.readInt();

            for (int i = 0; i < WAV.Length; i++)
            {
                if (coder.readByte() != WAV[i])
                {
                    throw new Exception("Unsupported format");
                }
            }

            int chunkType;
            int length;

            bool readFMT = false;
            bool readDATA = false;

            do
            {
                chunkType = coder.readInt();
                length = coder.readInt();

                switch (chunkType)
                {
                    case FMT:
                        decodeFMT(coder);
                        readFMT = true;
                        break;
                    case DATA:
                        decodeDATA(coder, length);
                        readDATA = true;
                        break;
                    default:
                        coder.skip(length);
                        break;
                }
            } while (!(readFMT && readDATA));
        }

        /// <summary>
        /// Decode the FMT block.
        /// </summary>
        /// <param name="coder"> an SWFDecoder containing the bytes to be decoded.
        /// </param>
        /// <exception cref="IOException"> if there is an error decoding the data. </exception>
        /// <exception cref="Exception"> if the block is in a format not supported
        /// by this decoder. </exception>



        private void decodeFMT(LittleDecoder coder)
        {
            format = SoundFormat.PCM;

            if (coder.readUnsignedShort() != 1)
            {
                throw new Exception("Unsupported format");
            }

            numberOfChannels = coder.readUnsignedShort();
            sampleRate = coder.readInt();
            coder.readInt(); // total data length
            coder.readUnsignedShort(); // total bytes per sample
            sampleSize = coder.readUnsignedShort() >> 3;
        }

        /// <summary>
        /// Decode the Data block containing the sound samples.
        /// </summary>
        /// <param name="coder"> an SWFDecoder containing the bytes to be decoded. </param>
        /// <param name="length"> the length of the block in bytes. </param>
        /// <exception cref="IOException"> if there is an error decoding the data. </exception>



        private void decodeDATA(LittleDecoder coder, int length)
        {
            samplesPerChannel = length / (sampleSize * numberOfChannels);

            sound = coder.readBytes(new byte[length]);
        }
    }

}