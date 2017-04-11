using System;
using System.IO;
using com.flagstone.transform.image;

/*
 * ImageFactory.java
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
    /// <para>
    /// ImageFactory is used to generate an image definition object from an image
    /// stored in a file, references by a URL or read from an stream. An plug-in
    /// architecture allows decoders to be registered to handle different image
    /// formats. The ImageFactory provides a standard interface for using the
    /// decoders.
    /// </para>
    /// 
    /// <para>
    /// Currently PNG, BMP and JPEG encoded images are supported by dedicated
    /// decoders. The BufferedImageDecoder can be used to decode any format supported
    /// using Java's ImageIO, including PNG, BMP and JPG format images. New decoders
    /// can be added by implementing the ImageDecoder interface and registering them
    /// in the ImageRegistry.
    /// </para>
    /// 
    /// <P>
    /// The defineImage() methods return an Definition (the abstract base class for
    /// all objects used to define shapes etc. in a Flash file. The exact class of
    /// the object generated depends of the format of the image loaded.
    /// </P>
    /// 
    /// <table>
    /// <tr>
    /// <th>Class</th>
    /// <th>Generated when...</th>
    /// </tr>
    /// 
    /// <tr>
    /// <td valign="top">DefineJPEGImage2</td>
    /// <td>A JPEG encoded image is loaded. The getFormat() method returns the class
    /// constant JPEG.</td>
    /// </tr>
    /// 
    /// <tr>
    /// <td valign="top">DefineImage</td>
    /// <td>An indexed BMP or PNG image contains a colour table without transparent
    /// colours or when a true colour image contains 16-bit or 24-bit colours is
    /// loaded. The getFormat() method returns the class constants IDX8, RGB5 or
    /// RGB8.</td>
    /// </tr>
    /// 
    /// <tr>
    /// <td valign="top">DefineImage2</td>
    /// <td>A BMP or PNG indexed image contains a colour table with transparent
    /// colours is loaded or when a true colour image contains 32-bit bit colours.
    /// The getFormat() method returns the class constants IDXA or RGBA.</td>
    /// </tr>
    /// 
    /// </table>
    /// 
    /// <P>
    /// Images are displayed in Flash by filling a shape with the image bitmap. The
    /// defineEnclosingShape() method generates a rectangular shape object which
    /// wraps the image:
    /// 
    /// <pre>
    ///     int imageId = movie.newIdentifier();
    ///     int shapeId = movie.newIdentifier();
    /// 
    ///     Definition image = Image(defineImage(imageId, ...);
    /// 
    ///     int x = image.getWidth()/2;
    ///     int y = image.getHeight()/2;
    /// 
    ///     LineStyle style = new LineStyle(20, ColorTable.black());
    /// 
    ///     movie.add(image);
    ///     movie.add(Image.defineEnclosingShape(shapeId, image, x, y, style);
    /// </pre>
    /// 
    /// <P>
    /// Here the origin, used when placing the shape on the screen, is defined as the
    /// centre of the shape. Other points may be defined to suit the alignment of the
    /// shape when it is placed on the display list.
    /// </P>
    /// </summary>
    public sealed class ImageFactory
    {

        /// <summary>
        /// The object used to decode the image. </summary>

        private ImageDecoder decoder;

        /// <summary>
        /// Create an image definition for the image located in the specified file.
        /// </summary>
        /// <param name="file">
        ///            a file containing the abstract path to the image.
        /// </param>
        /// <exception cref="IOException">
        ///             if there is an error reading the file.
        /// </exception>
        /// <exception cref="Exception">
        ///             if there is a problem decoding the image, either it is in an
        ///             unsupported format or an error occurred while decoding the
        ///             image data. </exception>



        public void read(FileInfo file)
        {



            ImageInfo info = new ImageInfo();
            info.Input = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); //new RandomAccessFile(file, "r");
                                                                                   //        info.setDetermineImageNumber(true);

            if (!info.check())
            {
                throw new Exception("Unsupported format");
            }

            decoder = ImageRegistry.getImageProvider(info.ImageFormat.MimeType);
            decoder.read(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }

        /// <summary>
        /// Create an image definition for an image read from a stream.
        /// </summary>
        /// <param name="stream">
        ///            the InputStream containing the image data.
        /// </param>
        /// <exception cref="IOException">
        ///             if there is an error reading the stream.
        /// </exception>
        /// <exception cref="Exception">
        ///             if there is a problem decoding the image, either it is in an
        ///             unsupported format or an error occurred while decoding the
        ///             image data. </exception>



        public void read(Stream stream)
        {
            ImageInfo info = new ImageInfo();
            var position = stream.Position;
            info.Input = stream;
            //        info.setDetermineImageNumber(true);

            if (!info.check())
            {
                throw new Exception("Unsupported format");
            }

            decoder = ImageRegistry.getImageProvider(info.ImageFormat.MimeType);
            stream.Position = position;
            decoder.read(stream);
        }

        /// <summary>
        /// Create a definition for the image so it can be added to a Flash movie. </summary>
        /// <param name="identifier"> the unique identifier for the image. </param>
        /// <returns> an ImageTag representing one of the image definitions supported
        /// in Flash. </returns>


        public ImageTag defineImage(int identifier)
        {
            return decoder.defineImage(identifier);
        }

        /// <summary>
        /// Get the ImageDecoder used to decode the image.
        /// </summary>
        /// <returns> the ImageDecoder instance that the factory created to decode the
        /// image. </returns>
        public ImageDecoder Decoder => decoder;
    }

}