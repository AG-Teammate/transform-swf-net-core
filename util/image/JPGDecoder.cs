using System;
using System.IO;
using com.flagstone.transform.coder;
using com.flagstone.transform.image;

/*
 * JPGDecoder.java
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

namespace com.flagstone.transform.util.image
{
    /// <summary>
    /// JPGDecoder decodes JPEG images so they can be used in a Flash file.
    /// </summary>
    public sealed class JPGDecoder : ImageProvider, ImageDecoder
    {

/*
        /// <summary>
        /// Message used to signal that the image cannot be decoded. </summary>
        private const string BAD_FORMAT = "Unsupported format";
*/

        /// <summary>
        /// The width of the image in pixels. </summary>

        private int width;
        /// <summary>
        /// The height of the image in pixels. </summary>

        private int height;
        /// <summary>
        /// The image data. </summary>

        private byte[] image = new byte[0];

        /// <summary>
        /// {@inheritDoc} </summary>



        public void read(FileInfo file)
        {
            read(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }

        /// <summary>
        /// {@inheritDoc} </summary>


        public ImageTag defineImage(int identifier)
        {
            return new DefineJPEGImage2(identifier, image);
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public ImageDecoder newDecoder()
        {
            return new JPGDecoder();
        }

        /// <summary>
        /// {@inheritDoc} </summary>



        public void read(Stream stream)
        {



            BigDecoder coder = new BigDecoder(stream);

            int marker;
            int length;

            do
            {
                marker = coder.readUnsignedShort();
                switch (marker)
                {
                    case JPEGInfo.SOI:
                    case JPEGInfo.EOI:
                        copyTag(marker, 0, coder);
                        break;
                    case JPEGInfo.SOF0:
                    case JPEGInfo.SOF2:
                    case JPEGInfo.DHT:
                    case JPEGInfo.DQT:
                    case JPEGInfo.COM:
                        length = coder.readUnsignedShort();
                        copyTag(marker, length, coder);
                        break;
                    case JPEGInfo.SOS:
                        length = coder.readUnsignedShort();
                        copyTag(marker, length, coder);
                        readEntropyData(coder);
                        break;
                    case JPEGInfo.DRI:
                        copyTag(marker, 0, coder);
                        readEntropyData(coder);
                        break;
                    default:
                        if ((marker & JPEGInfo.APP) == JPEGInfo.APP)
                        {
                            length = coder.readUnsignedShort();
                            copyTag(marker, length, coder);
                        }
                        else
                        {
                            copyTag(marker, 0, coder);
                        }
                        break;
                }
            } while (marker != JPEGInfo.EOI);



            JPEGInfo info = new JPEGInfo();
            info.decode(image);
            width = info.Width;
            height = info.Height;
        }

        /// <summary>
        /// Copy a JPEG tag. </summary>
        /// <param name="marker"> the identifier for the tag. </param>
        /// <param name="length"> the length of the tag data. </param>
        /// <param name="coder"> the decoder containing the JPEG image data. </param>
        /// <exception cref="IOException"> if there is an error reading the image data. </exception>



        private void copyTag(int marker, int length, BigDecoder coder)
        {
            byte[] bytes;
            if (length > 0)
            {
                bytes = new byte[length + 2];
            }
            else
            {
                bytes = new byte[2];
            }
            bytes[0] = (byte)(marker >> Coder.TO_LOWER_BYTE);
            bytes[1] = (byte)marker;

            if (length > 0)
            {
                bytes[2] = (byte)(length >> Coder.TO_LOWER_BYTE);
                bytes[3] = (byte)length;
                coder.readBytes(bytes, 4, length - 2);
            }


            int imgLength = image.Length;
            image = Arrays.copyOf(image, imgLength + bytes.Length);
            Array.Copy(bytes, 0, image, imgLength, bytes.Length);
        }

        /// <summary>
        /// Read the encoded image data from a JPEG image. </summary>
        /// <param name="coder"> the decoder containing the JPEG image data. </param>
        /// <exception cref="IOException"> if there is an error reading the image data. </exception>



        private void readEntropyData(BigDecoder coder)
        {
            byte[] bytes = new byte[2048];
            int index = 0;
            int current;
            int next;

            do
            {
                coder.mark();
                current = coder.readByte();

                if (current == 255)
                {
                    next = coder.readByte();

                    if (next == 0)
                    {
                        if (index + 2 >= bytes.Length)
                        {


                            int imgLength = image.Length;
                            image = Arrays.copyOf(image, imgLength + index);
                            Array.Copy(bytes, 0, image, imgLength, index);
                            index = 0;
                        }
                        bytes[index++] = (byte)current;
                        bytes[index++] = (byte)next;
                    }
                    else
                    {
                        if (index > 0)
                        {


                            int imgLength = image.Length;
                            image = Arrays.copyOf(image, imgLength + index);
                            Array.Copy(bytes, 0, image, imgLength, index);
                            index = 0;
                        }
                        coder.reset();
                        break;
                    }
                }
                else
                {
                    if (index >= bytes.Length)
                    {


                        int imgLength = image.Length;
                        image = Arrays.copyOf(image, imgLength + bytes.Length);
                        Array.Copy(bytes, 0, image, imgLength, bytes.Length);
                        index = 0;
                    }
                    bytes[index++] = (byte)current;
                }
                coder.unmark();

            } while (true);
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public int Width => width;

        /// <summary>
        /// {@inheritDoc} </summary>
        public int Height => height;

        /// <summary>
        /// {@inheritDoc} </summary>
        public byte[] Image => Arrays.copyOf(image, image.Length);
    }

}