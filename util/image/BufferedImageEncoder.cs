using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.image;
using ImageSharp;

/*
 * BufferedImageEncoder.java
 * Transform
 *
 * Copyright (c) 2010 Flagstone Software Ltd. All rights reserved.
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
    /// BufferedImageEncoder generates BufferedImages from Flash image definitions.
    /// </summary>
    public sealed class BufferedImageEncoder
    {
        /// <summary>
        /// The number of bytes per pixel in a RGBA format image. </summary>
        private const int BYTES_PER_PIXEL = 4;
        /// <summary>
        /// The alpha channel level for an opaque pixel. </summary>
        private const int OPAQUE = -1;
        /// <summary>
        /// Position in 32-bit word of red channel. </summary>
        private const int RED = 0;
        /// <summary>
        /// Position in 32-bit word of green channel. </summary>
        private const int GREEN = 1;
        /// <summary>
        /// Position in 32-bit word of blue channel. </summary>
        private const int BLUE = 2;
        /// <summary>
        /// Position in 32-bit word of alpha channel. </summary>
        private const int ALPHA = 3;
        /// <summary>
        /// Mask applied to extract 5-bit values. </summary>
        private const int MASK_5BIT = 0x001F;
        /// <summary>
        /// Mask applied to extract 8-bit values. </summary>
        private const int MASK_8BIT = 0x00FF;
        /// <summary>
        /// Number of bits to shift when aligning to the second byte in a 16-bit
        /// or 32-bit word. 
        /// </summary>
        private const int ALIGN_BYTE2 = 8;
        /// <summary>
        /// Number of bits to shift when aligning to the second byte in a 16-bit
        /// or 32-bit word. 
        /// </summary>
        private const int ALIGN_BYTE3 = 16;
        /// <summary>
        /// Number of bits to shift when aligning to the second byte in a 32-bit
        /// word. 
        /// </summary>
        private const int ALIGN_BYTE4 = 24;

        /// <summary>
        /// Value added to offsets to ensure image width is aligned on a 16-bit
        /// boundary, 3 == 2 bytes + 1.
        /// </summary>
        private const int WORD_ALIGN = 3;

        /// <summary>
        /// Size of a pixel in a RGB555 true colour image. </summary>
        private const int RGB5_SIZE = 16;

        /// <summary>
        /// The format of the decoded image. </summary>

        private ImageFormat format;
        /// <summary>
        /// The width of the image in pixels. </summary>

        private int width;
        /// <summary>
        /// The height of the image in pixels. </summary>

        private int height;
        /// <summary>
        /// The colour table for indexed images. </summary>

        private byte[] table;
        /// <summary>
        /// The image data. </summary>

        private byte[] image;

        /// <summary>
        /// Decode an ImageTeg definition.
        /// </summary>
        /// <param name="definition">
        ///            a DefineImage object.
        /// </param>
        /// <exception cref="Exception">
        ///             if there is a problem decoding the image definition. </exception>



        public void setImage(ImageTag definition)
        {

            if (definition is DefineImage)
            {
                setImage((DefineImage)definition);
            }
            else if (definition is DefineImage2)
            {
                setImage((DefineImage2)definition);
            }
        }

        /// <summary>
        /// Decode a DefineImage definition.
        /// </summary>
        /// <param name="definition">
        ///            a DefineImage object.
        /// </param>
        /// <exception cref="Exception">
        ///             if there is a problem decoding the image definition. </exception>



        public void setImage(DefineImage definition)
        {

            width = definition.Width;
            height = definition.Height;

            if (definition.TableSize > 0)
            {
                IDX = definition;
            }
            else
            {
                if (definition.PixelSize == RGB5_SIZE)
                {
                    RGB5 = definition;
                }
                else
                {
                    RGB8 = definition;
                }
            }
        }

        /// <summary>
        /// Decode a DefineImage2 definition.
        /// </summary>
        /// <param name="definition">
        ///            a DefineImage2 object.
        /// </param>
        /// <exception cref="Exception">
        ///             if there is a problem decoding the image definition. </exception>



        public void setImage(DefineImage2 definition)
        {
            if (definition.TableSize > 0)
            {
                IDXA = definition;
            }
            else
            {
                RGBA = definition;
            }
        }

        /// <summary>
        /// Get the width of the image. </summary>
        /// <returns> the width of the image in pixels. </returns>
        public int Width => width;

        /// <summary>
        /// Get the height of the image. </summary>
        /// <returns> the height of the image in pixels. </returns>
        public int Height => height;

        /// <summary>
        /// Get the array of bytes that make up the image.
        /// </summary>
        /// <returns> the array of bytes representing the image. </returns>
        public byte[] getImage()
        {
            return Arrays.copyOf(image, image.Length);
        }

        /// <summary>
        /// Decode an indexed image from a Flash image definition. </summary>
        /// <param name="definition"> the Flash object containing the indexed image. </param>
        /// <exception cref="Exception"> if the image is in an unsupported format. </exception>



        private DefineImage IDX
        {
            set
            {



                byte[] data = unzip(value.Image, width, height);


                int scanLength = (width + WORD_ALIGN) & ~WORD_ALIGN;


                int tableLength = value.TableSize;

                int pos = 0;
                int index = 0;

                format = ImageFormat.IDX8;
                table = new byte[tableLength * BYTES_PER_PIXEL];
                image = new byte[height * width];

                for (int i = 0; i < tableLength; i++, index += BYTES_PER_PIXEL)
                {
                    table[index + ALPHA] = 255;//OPAQUE;
                    table[index + BLUE] = data[pos++];
                    table[index + GREEN] = data[pos++];
                    table[index] = data[pos++];
                }

                index = 0;

                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++, index++)
                    {
                        image[index] = data[pos++];
                    }
                    pos += (scanLength - width);
                }
            }
        }

        /// <summary>
        /// Decode a true-colour image from a Flash image definition. The image
        /// contains 16-bit pixels.
        /// </summary>
        /// <param name="definition"> the Flash object containing the image. </param>
        /// <exception cref="Exception"> if the image is in an unsupported format. </exception>



        private DefineImage RGB5
        {
            set
            {


                byte[] data = unzip(value.Image, width, height);


                int scanLength = (width + WORD_ALIGN) & ~WORD_ALIGN;

                int pos = 0;
                int index = 0;

                format = ImageFormat.RGB8;
                image = new byte[height * width * BYTES_PER_PIXEL];

                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {


                        int color = (data[pos++] << ALIGN_BYTE2 | (data[pos++] & MASK_8BIT)) & Coder.LOWEST15;

                        image[index + ALPHA] = 255; //OPAQUE;
                        image[index + RED] = (byte)(color >> 10);
                        image[index + GREEN] = (byte)((color >> 5) & MASK_5BIT);
                        image[index + BLUE] = (byte)(color & MASK_5BIT);
                        index += BYTES_PER_PIXEL;
                    }
                    pos += (scanLength - width);
                }
            }
        }

        /// <summary>
        /// Decode a true-colour image from a Flash image definition. The image
        /// contains 24-bit pixels.
        /// </summary>
        /// <param name="definition"> the Flash object containing the image. </param>
        /// <exception cref="Exception"> if the image is in an unsupported format. </exception>



        private DefineImage RGB8
        {
            set
            {



                byte[] data = unzip(value.Image, width, height);


                int scanLength = (width + WORD_ALIGN) & ~WORD_ALIGN;

                int pos = 0;
                int index = 0;

                format = ImageFormat.RGB8;
                image = new byte[height * width * BYTES_PER_PIXEL];

                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        image[index + ALPHA] = 255;//OPAQUE;
                        image[index + RED] = data[pos++];
                        image[index + GREEN] = data[pos++];
                        image[index + BLUE] = data[pos++];
                        index += BYTES_PER_PIXEL;
                    }
                    pos += (scanLength - width);
                }
            }
        }

        /// <summary>
        /// Decode a indexed image from a Flash image definition. The colour table
        /// contains 32-bit pixels.
        /// </summary>
        /// <param name="definition"> the Flash object containing the image. </param>
        /// <exception cref="Exception"> if the image is in an unsupported format. </exception>



        private DefineImage2 IDXA
        {
            set
            {

                width = value.Width;
                height = value.Height;



                byte[] data = unzip(value.Image, width, height);


                int scanLength = (width + WORD_ALIGN) & ~WORD_ALIGN;


                int tableLength = value.TableSize;

                int pos = 0;
                int index = 0;

                format = ImageFormat.IDXA;
                table = new byte[tableLength * BYTES_PER_PIXEL];
                image = new byte[height * width];

                for (int i = 0; i < tableLength; i++, index += BYTES_PER_PIXEL)
                {
                    table[index + ALPHA] = data[pos++];
                    table[index + BLUE] = data[pos++];
                    table[index + GREEN] = data[pos++];
                    table[index] = data[pos++];
                }

                index = 0;

                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++, index++)
                    {
                        image[index] = data[pos++];
                    }
                    pos += (scanLength - width);
                }
            }
        }

        /// <summary>
        /// Decode a true-colour image from a Flash image definition. The image
        /// contains 32-bit pixels.
        /// </summary>
        /// <param name="definition"> the Flash object containing the image. </param>
        /// <exception cref="Exception"> if the image is in an unsupported format. </exception>



        private DefineImage2 RGBA
        {
            set
            {

                width = value.Width;
                height = value.Height;



                byte[] data = unzip(value.Image, width, height);
                // final int scanLength = (imgWidth + WORD_ALIGN) & ~WORD_ALIGN;

                int pos = 0;
                int index = 0;

                image = new byte[height * width * BYTES_PER_PIXEL];

                for (int h = 0; h < height; h++)
                {
                    for (int w = 0; w < width; w++, index += BYTES_PER_PIXEL)
                    {
                        image[index + ALPHA] = data[pos++];
                        image[index + RED] = data[pos++];
                        image[index + GREEN] = data[pos++];
                        image[index + BLUE] = data[pos++];
                    }
                }
            }
        }

        /// <summary>
        /// Create a BufferedImage from the decoded Flash image.
        /// </summary>
        /// <returns> a BufferedImage containing the image. </returns>
        public Image BufferedImage
        {
            get
            {
                Image bufferedImage;
                if (format == ImageFormat.IDX8 || format == ImageFormat.IDXA)
                {
                    bufferedImage = IndexedImage;
                }
                else
                {
                    bufferedImage = RGBAImage;
                }
                return bufferedImage;
            }
        }

        /// <summary>
        /// Return the indexed image as a BufferedImage.
        /// </summary>
        /// <returns> A BufferedImage containing the image data. </returns>
        private Image IndexedImage
        {
            get
            {



                byte[] red = new byte[table.Length];


                byte[] green = new byte[table.Length];


                byte[] blue = new byte[table.Length];


                byte[] alpha = new byte[table.Length];



                int count = table.Length / BYTES_PER_PIXEL;
                int index = 0;

                for (int i = 0; i < count; i++)
                {
                    red[i] = table[index + BLUE];
                    green[i] = table[index + GREEN];
                    blue[i] = table[index + RED];
                    alpha[i] = table[index + ALPHA];
                    index += BYTES_PER_PIXEL;
                }



                Image bufferedImage = new Image(width, height);



                int[] row = new int[width];
                int color;
                index = 0;
                using (var pixels = bufferedImage.Lock())
                {
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++, index++)
                        {
                            color = (image[index] & MASK_8BIT) << 2;

                            row[j] = (table[color + ALPHA] & MASK_8BIT) << ALIGN_BYTE4;
                            row[j] = row[j] | ((table[color + 2] & MASK_8BIT) << ALIGN_BYTE3);
                            row[j] = row[j] | ((table[color + 1] & MASK_8BIT) << ALIGN_BYTE2);
                            row[j] = row[j] | (table[color + 0] & MASK_8BIT);

                            var colorS = new Color((byte)(table[color + 2] & MASK_8BIT),
                                (byte)(table[color + 1] & MASK_8BIT),
                                (byte)(table[color + 0] & MASK_8BIT),
                                (byte)(table[color + ALPHA] & MASK_8BIT));
                            pixels[j, i] = colorS;
                        }
                        ////setRGB(int startX, int startY, int w, int h, int[] rgbArray, int offset, int scansize)
                        //bufferedImage.setRGB(0, i, width, 1, row, 0, width);
                    }
                }
                return bufferedImage;
            }
        }

        /// <summary>
        /// Return the 32-bit true-colour image as a BufferedImage.
        /// </summary>
        /// <returns> A BufferedImage containing the image data. </returns>
        private Image RGBAImage
        {
            get
            {



                Image bufferedImage = new Image(width, height);



                //int[] buffer = new int[width];
                int index = 0;
                using (var pixels = bufferedImage.Lock())
                {
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++, index += BYTES_PER_PIXEL)
                        {
                            //buffer[j] = (image[index + ALPHA] & MASK_8BIT) << ALIGN_BYTE4;
                            //buffer[j] = buffer[j] | ((image[index + RED] & MASK_8BIT) << ALIGN_BYTE3);
                            //buffer[j] = buffer[j] | ((image[index + GREEN] & MASK_8BIT) << ALIGN_BYTE2);
                            //buffer[j] = buffer[j] | (image[index + BLUE] & MASK_8BIT);
                            var color = new Color((byte)(image[index + RED] & MASK_8BIT),
                                (byte)(image[index + GREEN] & MASK_8BIT),
                                (byte)(image[index + BLUE] & MASK_8BIT),
                                (byte)(image[index + ALPHA] & MASK_8BIT));
                            pixels[j, i] = color;
                        }
                        //setRGB(int startX, int startY, int w, int h, int[] rgbArray, int offset, int scansize)
                        //bufferedImage.setRGB(0, i, width, 1, buffer, 0, width);

                    }
                }
                return bufferedImage;
            }
        }
        /// <summary>
        /// Resizes a BufferedImage to the specified width and height. The aspect
        /// ratio of the image is maintained so the area in the new image not covered
        /// by the resized original will be transparent.
        /// </summary>
        /// <param name="bufferedImg">
        ///            the BufferedImage to resize. </param>
        /// <param name="imgWidth">
        ///            the width of the resized image in pixels. </param>
        /// <param name="imgHeight">
        ///            the height of the resized image in pixels. </param>
        /// <returns> a new BufferedImage with the specified width and height. </returns>



        public Image resizeImage(Image bufferedImg, int imgWidth, int imgHeight)
        {
            return (Image)bufferedImg.Resize(imgWidth, imgHeight);
        }

        /// <summary>
        /// Uncompress the image using the ZIP format. </summary>
        /// <param name="bytes"> the compressed image data. </param>
        /// <param name="imgWidth"> the width of the image in pixels. </param>
        /// <param name="imgHeight"> the height of the image in pixels. </param>
        /// <returns> the uncompressed image. </returns>
        /// <exception cref="Exception"> if the compressed image is not in the ZIP
        /// format or cannot be uncompressed. </exception>



        private byte[] unzip(byte[] bytes, int imgWidth, int imgHeight)
        {


            byte[] data = new byte[imgWidth * imgHeight * 8];
            int count = 0;



            Inflater inflater = new Inflater();
            inflater.Input = bytes;
            count = inflater.inflate(data);
            inflater.end();



            byte[] uncompressedData = new byte[count];

            Array.Copy(data, 0, uncompressedData, 0, count);

            return uncompressedData;
        }
    }

}