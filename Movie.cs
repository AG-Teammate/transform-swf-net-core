using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using com.flagstone.transform.coder;
using Ionic.Zlib;

/*
 * Movie.java
 * Transform
 *
 * Copyright (c) 2001-2010 Flagstone Software Ltd. All rights reserved.
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

namespace com.flagstone.transform
{
    /// <summary>
    /// Movie is a container class for the objects that represents the data
    /// structures in a Flash file.
    /// 
    /// <para>
    /// Movie is the core class of the Transform package. It is used to parse and
    /// generate Flash files, translating the binary format of the Flash file into an
    /// list objects that can be inspected and updated.
    /// </para>
    /// 
    /// <para>
    /// A Movie object also contains the attributes that make up the header
    /// information of the Flash file, identifying the version support, size of the
    /// Flash Player screen, etc.
    /// </para>
    /// 
    /// <para>
    /// Movie is also used to generate the unique identifiers that are used to
    /// reference objects. Each call to newIdentifier() returns a unique number for
    /// the current. The identifiers are generated using a simple counter. When a
    /// movie is decoded this counter is updated each time an object definition is
    /// decoded. This allows new objects to be added and ensures that the identifier
    /// does not conflict with an existing object.
    /// </para>
    /// </summary>
    public sealed class Movie
    {

        /// <summary>
        /// The version of Flash supported. </summary>
        public const int VERSION = 10;

        /// <summary>
        /// Length in bytes of the magic number used to identify the file type. </summary>
        private const int SIGNATURE_LENGTH = 3;
        /// <summary>
        /// Length in bytes of the signature and length fields. </summary>
        private const int HEADER_LENGTH = 8;
        /// <summary>
        /// Signature identifying Flash (SWF) files. </summary>
        public static readonly byte[] FWS = { 0x46, 0x57, 0x53 };
        /// <summary>
        /// Signature identifying Compressed Flash (SWF) files. </summary>
        public static readonly byte[] CWS = { 0x43, 0x57, 0x53 };

        /// <summary>
        /// Format string used in toString() method. </summary>
        private const string FORMAT = "Movie: { objects=%s}";
        /// <summary>
        /// The registry for the different types of decoder. </summary>

        private DecoderRegistry registry;
        /// <summary>
        /// The character encoding used for strings. </summary>

        private CharacterEncoding encoding;
        /// <summary>
        /// The list of objects that make up the movie. </summary>
        private IList<MovieTag> objects;

        /// <summary>
        /// Creates a new Movie.
        /// </summary>
        public Movie()
        {
            registry = DecoderRegistry.Default;
            encoding = CharacterEncoding.UTF8;
            objects = new List<MovieTag>();
        }

        /// <summary>
        /// Creates a complete copy of this movie.
        /// </summary>
        /// <param name="movie"> the Movie to copy. </param>


        public Movie(Movie movie)
        {
            if (movie.registry != null)
            {
                registry = movie.registry.copy();
            }
            encoding = movie.encoding;

            objects = new List<MovieTag>(movie.objects.Count);

            foreach (MovieTag tag in movie.objects)
            {
                //AG lazy to figure out inheritance stuff. just use reflection
                var method = tag.GetType().GetTypeInfo().GetDeclaredMethod("copy");
                var copy = method.Invoke(tag, null) as MovieTag;
                objects.Add(copy);
            }
        }

        /// <summary>
        /// Sets the registry containing the object used to decode the different
        /// types of object found in a movie.
        /// </summary>
        /// <param name="decoderRegistry"> a central registry to decoders of different types
        /// of object. </param>


        public DecoderRegistry Registry
        {
            set => registry = value;
        }

        /// <summary>
        /// Sets the encoding scheme for strings encoded and decoded from Flash
        /// files.
        /// </summary>
        /// <param name="enc"> the character encoding used for strings. </param>


        public CharacterEncoding Encoding
        {
            set => encoding = value;
        }

        /// <summary>
        /// Get the list of objects contained in the Movie.
        /// </summary>
        /// <returns> the list of objects that make up the movie. </returns>
        public IList<MovieTag> Objects
        {
            get => objects;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException();
                }
                objects = value;
            }
        }


        /// <summary>
        /// Adds the object to the Movie.
        /// </summary>
        /// <param name="anObject">
        ///            the object to be added to the movie. Must not be null. </param>
        /// <returns> this object. </returns>


        public Movie add(MovieTag anObject)
        {
            if (anObject == null)
            {
                throw new ArgumentException();
            }
            objects.Add(anObject);
            return this;
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public Movie copy()
        {
            return new Movie(this);
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public override string ToString()
        {
            return ObjectExtensions.FormatJava(FORMAT, objects);
        }

        /// <summary>
        /// Decodes the contents of the specified file.
        /// </summary>
        /// <param name="file">
        ///            the Flash file that will be parsed. </param>
        /// <exception cref="Exception">
        ///             - if the file does not contain Flash data. </exception>
        /// <exception cref="IOException">
        ///             - if an I/O error occurs while reading the file. </exception>



        public void decodeFromFile(FileInfo file)
        {
            decodeFromStream(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }

        /// <summary>
        /// Decodes the binary Flash data from an input stream. If an error occurs
        /// while the data is being decoded an exception is thrown. The list of
        /// objects in the Movie will contain the last tag successfully decoded.
        /// </summary>
        /// <param name="stream">
        ///            an InputStream from which the objects will be decoded.
        /// </param>
        /// <exception cref="Exception">
        ///             if the file does not contain Flash data. </exception>
        /// <exception cref="IOException">
        ///             if an I/O error occurs while reading the file. </exception>



        public void decodeFromStream(Stream stream)
        {

            Stream streamIn = null;

            try
            {


                Context context = new Context();
                context.Registry = registry;
                context.Encoding = encoding.Encoding;



                byte[] signature = new byte[SIGNATURE_LENGTH];
                if (stream.Read(signature, 0, signature.Length) != signature.Length)
                {
                    throw new Exception("Could not read file signature");
                }

                if (CWS.SequenceEqual(signature))
                {
                    streamIn = new ZlibStream(stream, CompressionMode.Decompress); //InflaterInputStream(stream);
                    context.put(Context.COMPRESSED, 1);
                }
                else if (FWS.SequenceEqual(signature))
                {
                    streamIn = stream;
                    context.put(Context.COMPRESSED, 0);
                }
                else
                {
                    throw new Exception();
                }

                context.put(Context.VERSION, stream.ReadByte());

                int length = stream.ReadByte();
                length |= stream.ReadByte() << Coder.ALIGN_BYTE1;
                length |= stream.ReadByte() << Coder.ALIGN_BYTE2;
                length |= stream.ReadByte() << Coder.ALIGN_BYTE3;



                /*
				 * If the file is shorter than the default buffer size then set the
				 * buffer size to be the file size - this gets around a bug in Java
				 * where the end of ZLIB streams are not detected correctly.
				 */
                SWFDecoder decoder;

                if (length < SWFDecoder.BUFFER_SIZE)
                {
                    decoder = new SWFDecoder(streamIn, length - HEADER_LENGTH);
                }
                else
                {
                    decoder = new SWFDecoder(streamIn);
                }

                decoder.Encoding = encoding;

                objects.Clear();



                SWFFactory<MovieTag> factory = registry.MovieDecoder;



                MovieHeader header = new MovieHeader(decoder, context);
                objects.Add(header);

                while ((int)((uint)decoder.scanUnsignedShort() >> Coder.LENGTH_FIELD_SIZE) != MovieTypes.END)
                {
                    factory.getObject(objects, decoder, context);
                }

                decoder.readUnsignedShort();

                header.Version = context.get(Context.VERSION);
                header.Compressed = context.get(Context.COMPRESSED) == 1;

            }
            finally
            {
                streamIn?.Dispose();
            }
        }

        /// <summary>
        /// Encodes the list of objects and writes the data to the specified file.
        /// If an error occurs while encoding the file then an exception is thrown.
        /// </summary>
        /// <param name="file">
        ///            the Flash file that the movie will be encoded to.
        /// </param>
        /// <exception cref="IOException">
        ///             - if an I/O error occurs while writing the file. </exception>
        /// <exception cref="Exception">
        ///             if an error occurs when compressing the flash file. </exception>



        public void encodeToFile(FileInfo file)
        {
            encodeToStream(new FileStream(file.FullName, FileMode.Create, FileAccess.Write));
        }

        /// <summary>
        /// Returns the encoded representation of the list of objects that this
        /// Movie contains. If an error occurs while encoding the file then an
        /// exception is thrown.
        /// </summary>
        /// <param name="stream">
        ///            the output stream that the video will be encoded to. </param>
        /// <exception cref="IOException">
        ///             - if an I/O error occurs while encoding the file. </exception>
        /// <exception cref="Exception">
        ///             if an error occurs when compressing the flash file. </exception>



        public void encodeToStream(Stream stream)
        {

            Stream streamOut = null;

            try
            {


                MovieHeader header = (MovieHeader)objects[0];



                Context context = new Context();
                context.Encoding = encoding.Encoding;
                context.put(Context.VERSION, header.Version);

                // length of signature, version, length and end
                // CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
                int length = 10;
                int frameCount = 0;

                foreach (MovieTag tag in objects)
                {
                    length += tag.prepareToEncode(context);

                    if (tag is ShowFrame)
                    {
                        frameCount++;
                    }
                }

                header.FrameCount = frameCount;

                if (header.Compressed)
                {
                    stream.Write(CWS, 0, CWS.Length);
                }
                else
                {
                    stream.Write(FWS, 0, FWS.Length);
                }

                stream.WriteByte((byte)header.Version);
                stream.WriteByte((byte)length);
                stream.WriteByte((byte)((uint)length >> Coder.ALIGN_BYTE1));
                stream.WriteByte((byte)((uint)length >> Coder.ALIGN_BYTE2));
                stream.WriteByte((byte)((uint)length >> Coder.ALIGN_BYTE3));

                if (header.Compressed)
                {
                    streamOut = new ZlibStream(stream, CompressionMode.Compress); //DeflaterOutputStream(stream);
                }
                else
                {
                    streamOut = stream;
                }



                SWFEncoder coder = new SWFEncoder(streamOut);
                coder.Encoding = encoding;

                foreach (MovieTag tag in objects)
                {
                    tag.encode(coder, context);
                }
                coder.writeShort(0);
                coder.flush();
            }
            finally
            {
                if (streamOut != null)
                {
                    streamOut.Flush();
                    streamOut.Dispose();
                }
            }
        }
    }

}