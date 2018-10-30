

/*
 * BufferedImageDecoder.java
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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace com.flagstone.transform.util.image
{

    using com.flagstone.transform.coder;
    using LittleDecoder = com.flagstone.transform.coder.LittleDecoder;
    using DefineImage = com.flagstone.transform.image.DefineImage;
    using DefineImage2 = com.flagstone.transform.image.DefineImage2;
    using ImageFormat = com.flagstone.transform.image.ImageFormat;
    using ImageTag = com.flagstone.transform.image.ImageTag;

    /// <summary>
    /// ImageDecoder decodes Images so they can be used in a Flash
    /// file. The class also provides a set of convenience methods for converting
    /// Flash images definitions into Images allowing the images to easily
    /// be extracted from a Flash movie.
    /// </summary>


    public sealed class BufferedImageDecoder : ImageProvider, ImageDecoder
    {
        /// <summary>
        /// Message used to signal that the image cannot be decoded. </summary>
        private const string BAD_FORMAT = "Unsupported format";
        /// <summary>
        /// The number of bytes per pixel in a RGBA format image. </summary>
        private const int BYTES_PER_PIXEL = 4;
        /// <summary>
        /// The alpha channel level for an opaque pixel. </summary>
        private const int OPAQUE = 255;
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
        /// Size of each colour table entry or pixel in a true colour image. </summary>
        private const int COLOUR_CHANNELS = 4;
        /// <summary>
        /// Size of each colour table entry or pixel in a RGB image. </summary>
        private const int RGB_CHANNELS = 3;

        /// <summary>
        /// Size of a pixel in a RGB555 true colour image. </summary>
        private const int RGB5_SIZE = 16;
        /// <summary>
        /// Size of a pixel in a RGB8 true colour image. </summary>
        private const int RGB8_SIZE = 24;

        /// <summary>
        /// Shift used to align the RGB555 red channel to a 8-bit pixel. </summary>
        private const int RGB5_MSB_MASK = 0x00F8;
        /// <summary>
        /// Shift used to align the RGB555 red channel to a 8-bit pixel. </summary>
        private const int RGB5_RED_SHIFT = 7;
        /// <summary>
        /// Shift used to align the RGB555 green channel to a 8-bit pixel. </summary>
        private const int RGB5_GREEN_SHIFT = 2;
        /// <summary>
        /// Shift used to align the RGB555 blue channel to a 8-bit pixel. </summary>
        private const int RGB5_BLUE_SHIFT = 3;


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
        /// {@inheritDoc} </summary>
        public ImageDecoder newDecoder()
        {
            return new BufferedImageDecoder();
        }

        /// <summary>
        /// {@inheritDoc} </summary>



        public void read(FileInfo file)
        {
            read(new System.IO.FileStream(file.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite));
        }


        /// <summary>
        /// {@inheritDoc} </summary>



        public void read(System.IO.Stream stream)
        {
            read(SixLabors.ImageSharp.Image.Load(stream));
        }


        /// <summary>
        /// {@inheritDoc} </summary>


        public ImageTag defineImage(int identifier)
        {
            ImageTag @object = null;

            switch (format)
            {
                case ImageFormat.IDX8:
                    @object = new DefineImage(identifier, width, height, table.Length / COLOUR_CHANNELS, zip(merge(adjustScan(width, height, image), table)));
                    break;
                case ImageFormat.IDXA:
                    @object = new DefineImage2(identifier, width, height, table.Length / COLOUR_CHANNELS, zip(mergeAlpha(adjustScan(width, height, image), table)));
                    break;
                case ImageFormat.RGB5:
                    @object = new DefineImage(identifier, width, height, zip(packColours(width, height, image)), RGB5_SIZE);
                    break;
                case ImageFormat.RGB8:
                    orderAlpha(image);
                    @object = new DefineImage(identifier, width, height, zip(image), RGB8_SIZE);
                    break;
                case ImageFormat.RGBA:
                    applyAlpha(image);
                    @object = new DefineImage2(identifier, width, height, zip(image));
                    break;
                default:
                    throw new Exception(BAD_FORMAT);
            }
            return @object;
        }

        /// <summary>
        /// Create an image definition from a Image.
        /// </summary>
        /// <param name="identifier">
        ///            the unique identifier that will be used to refer to the image
        ///            in the Flash file.
        /// </param>
        /// <param name="obj">
        ///            the Image containing the image.
        /// </param>
        /// <returns> an image definition that can be added to a Movie.
        /// </returns>
        /// <exception cref="IOException">
        ///             if there is a problem extracting the image, from the
        ///             Image.
        /// </exception>
        /// <exception cref="Exception">
        ///             if the Image contains a format that is not currently
        ///             supported. </exception>



        public ImageTag defineImage(int identifier, Image<Rgba32> obj)
        {
            ImageTag @object = null;



            BufferedImageDecoder decoder = new BufferedImageDecoder();
            decoder.read(obj);

            switch (format)
            {
                case ImageFormat.IDX8:
                    @object = new DefineImage(identifier, width, height, table.Length, zip(merge(adjustScan(width, height, image), table)));
                    break;
                case ImageFormat.IDXA:
                    @object = new DefineImage2(identifier, width, height, table.Length, zip(mergeAlpha(adjustScan(width, height, image), table)));
                    break;
                case ImageFormat.RGB5:
                    @object = new DefineImage(identifier, width, height, zip(packColours(width, height, image)), RGB5_SIZE);
                    break;
                case ImageFormat.RGB8:
                    orderAlpha(image);
                    @object = new DefineImage(identifier, width, height, zip(image), RGB8_SIZE);
                    break;
                case ImageFormat.RGBA:
                    applyAlpha(image);
                    @object = new DefineImage2(identifier, width, height, zip(image));
                    break;
                default:
                    throw new Exception(BAD_FORMAT);
            }
            return @object;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public int Width => width;

        /// <summary>
        /// {@inheritDoc} </summary>
        public int Height
        {
            get
            {
                return height;
            }
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public byte[] Image
        {
            get
            {
                return Arrays.copyOf(image, image.Length);
            }
        }

        /// <summary>
        /// Decode a Image.
        /// </summary>
        /// <param name="obj">
        ///            a Image.
        /// </param>
        /// <exception cref="IOException">
        ///             if there is a problem extracting the image, from the
        ///             Image.
        /// </exception>
        /// <exception cref="Exception">
        ///             if there is a problem decoding the Image. </exception>



        public void read(Image<Rgba32> obj)
        {
            width = obj.Width;
            height = obj.Height;
            var pixels = obj.GetPixelSpan();

            format = ImageFormat.RGBA;
            image = new byte[height * width * BYTES_PER_PIXEL];
            int index = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, index += BYTES_PER_PIXEL)
                {
                    var pixel = pixels[y * width + x];

                    image[index + ALPHA] = pixel.A;
                    image[index + BLUE] = pixel.B;
                    image[index + GREEN] = pixel.G;
                    image[index] = pixel.R;
                }
            }
        }



        /// <summary>
        /// Reorder the image pixels from RGBA to ARGB.
        /// </summary>
        /// <param name="img"> the image data. </param>


        private void orderAlpha(byte[] img)
        {
            byte alpha;

            for (int i = 0; i < img.Length; i += BYTES_PER_PIXEL)
            {
                alpha = img[i + ALPHA];

                img[i + ALPHA] = img[i + BLUE];
                img[i + BLUE] = img[i + GREEN];
                img[i + GREEN] = img[i];
                img[i] = alpha;
            }
        }

        /// <summary>
        /// Apply the level for the alpha channel to the red, green and blue colour
        /// channels for encoding the image so it can be added to a Flash movie. </summary>
        /// <param name="img"> the image data. </param>


        private void applyAlpha(byte[] img)
        {
            int alpha;

            for (int i = 0; i < img.Length; i += BYTES_PER_PIXEL)
            {
                alpha = img[i + ALPHA] & MASK_8BIT;

                img[i + ALPHA] = (byte)(((img[i + BLUE] & MASK_8BIT) * alpha) / OPAQUE);
                img[i + BLUE] = (byte)(((img[i + GREEN] & MASK_8BIT) * alpha) / OPAQUE);
                img[i + GREEN] = (byte)(((img[i + RED] & MASK_8BIT) * alpha) / OPAQUE);
                img[i + RED] = (byte)alpha;
            }
        }

        /// <summary>
        /// Concatenate the colour table and the image data together. </summary>
        /// <param name="img"> the image data. </param>
        /// <param name="colors"> the colour table. </param>
        /// <returns> a single array containing the red, green and blue (not alpha)
        /// entries from the colour table followed by the red, green, blue and
        /// alpha channels from the image. The alpha defaults to 255 for an opaque
        /// image. </returns>


        private byte[] merge(byte[] img, byte[] colors)
        {


            byte[] merged = new byte[(colors.Length / BYTES_PER_PIXEL) * RGB_CHANNELS + img.Length];
            int dst = 0;

            for (int i = 0; i < colors.Length; i += BYTES_PER_PIXEL)
            {
                merged[dst++] = colors[i + RED];
                merged[dst++] = colors[i + GREEN];
                merged[dst++] = colors[i + BLUE];
            }

            foreach (byte element in img)
            {
                merged[dst++] = element;
            }

            return merged;
        }

        /// <summary>
        /// Concatenate the colour table and the image data together. </summary>
        /// <param name="img"> the image data. </param>
        /// <param name="colors"> the colour table. </param>
        /// <returns> a single array containing entries from the colour table followed
        /// by the image. </returns>


        private byte[] mergeAlpha(byte[] img, byte[] colors)
        {


            byte[] merged = new byte[colors.Length + img.Length];
            int dst = 0;

            foreach (byte element in colors)
            {
                merged[dst++] = element;
            }

            foreach (byte element in img)
            {
                merged[dst++] = element;
            }
            return merged;
        }

        /// <summary>
        /// Compress the image using the ZIP format. </summary>
        /// <param name="img"> the image data. </param>
        /// <returns> the compressed image. </returns>


        private byte[] zip(byte[] img)
        {


            Deflater deflater = new Deflater();
            deflater.Input = img;
            deflater.finish();



            byte[] compressedData = new byte[img.Length * 2];


            int bytesCompressed = deflater.deflate(compressedData);


            byte[] newData = Arrays.copyOf(compressedData, bytesCompressed);

            return newData;
        }

        /// <summary>
        /// Adjust the width of each row in an image so the data is aligned to a
        /// 16-bit word boundary when loaded in memory. The additional bytes are
        /// all set to zero and will not be displayed in the image.
        /// </summary>
        /// <param name="imgWidth"> the width of the image in pixels. </param>
        /// <param name="imgHeight"> the height of the image in pixels. </param>
        /// <param name="img"> the image data. </param>
        /// <returns> the image data with each row aligned to a 16-bit boundary. </returns>


        private byte[] adjustScan(int imgWidth, int imgHeight, byte[] img)
        {
            int src = 0;
            int dst = 0;
            int row;
            int col;

            int scan = 0;
            byte[] formattedImage = null;

            scan = (imgWidth + WORD_ALIGN) & ~WORD_ALIGN;
            formattedImage = new byte[scan * imgHeight];

            for (row = 0; row < imgHeight; row++)
            {
                for (col = 0; col < imgWidth; col++)
                {
                    formattedImage[dst++] = img[src++];
                }

                while (col++ < scan)
                {
                    formattedImage[dst++] = 0;
                }
            }

            return formattedImage;
        }

        /// <summary>
        /// Convert an image with 32-bits for the red, green, blue and alpha channels
        /// to one where the channels each take 5-bits in a 16-bit word. </summary>
        /// <param name="imgWidth"> the width of the image in pixels. </param>
        /// <param name="imgHeight"> the height of the image in pixels. </param>
        /// <param name="img"> the image data. </param>
        /// <returns> the image data with the red, green and blue channels packed into
        /// 16-bit words. Alpha is discarded. </returns>


        private byte[] packColours(int imgWidth, int imgHeight, byte[] img)
        {
            int src = 0;
            int dst = 0;
            int row;
            int col;



            int scan = imgWidth + (imgWidth & 1);


            byte[] formattedImage = new byte[scan * imgHeight * 2];

            for (row = 0; row < imgHeight; row++)
            {
                for (col = 0; col < imgWidth; col++, src++)
                {


                    int red = (img[src++] & RGB5_MSB_MASK) << RGB5_RED_SHIFT;


                    int green = (img[src++] & RGB5_MSB_MASK) << RGB5_GREEN_SHIFT;


                    int blue = (img[src++] & RGB5_MSB_MASK) >> RGB5_BLUE_SHIFT;


                    int colour = (red | green | blue) & Coder.LOWEST15;

                    formattedImage[dst++] = (byte)(colour >> ALIGN_BYTE2);
                    formattedImage[dst++] = (byte)colour;
                }

                while (col < scan)
                {
                    formattedImage[dst++] = 0;
                    formattedImage[dst++] = 0;
                    col++;
                }
            }
            return formattedImage;
        }
    }

}