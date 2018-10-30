/*
 * ImageDecoder.java
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

using com.flagstone.transform.image;
using SixLabors.ImageSharp;
using System;
using System.IO;

namespace com.flagstone.transform.util.image
{
    /// <summary>
    /// ImageDecoder is an interface that classes used to decode different image
    /// formats should implement in order to be registered with the ImageRegistry.
    /// </summary>
    public interface ImageDecoder
    {
        /// <summary>
        /// Read an image from a file. </summary>
        /// <param name="file"> the path to the file. </param>
        /// <exception cref="IOException"> if there is an error reading the image data. </exception>
        /// <exception cref="Exception"> if the file contains an unsupported format. </exception>


        void read(FileInfo file);
        /// <summary>
        /// Read an image from an input stream. </summary>
        /// <param name="stream"> the stream used to read the image data. </param>
        /// <exception cref="IOException"> if there is an error reading the image data. </exception>
        /// <exception cref="Exception"> if the file contains an unsupported format. </exception>


        void read(Stream stream);
        /// <summary>
        /// Get the width of the image. </summary>
        /// <returns> the width of the image in pixels. </returns>
        int Width { get; }
        /// <summary>
        /// Get the height of the image. </summary>
        /// <returns> the height of the image in pixels. </returns>
        int Height { get; }
        /// <summary>
        /// Get the array of bytes that make up the image. This method is used by
        /// the ImageFactory to generate a list of blocks for encoding an image as
        /// ScreenVideo.
        /// </summary>
        /// <returns> the array of bytes representing the image. </returns>
        byte[] Image { get; }
        /// <summary>
        /// Create the image definition so it can be added to a movie. </summary>
        /// <param name="identifier"> the unique identifier used to refer to the image. </param>
        /// <returns> the image definition. </returns>
        ImageTag defineImage(int identifier);
    }

}