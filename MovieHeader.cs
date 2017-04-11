/*
 * MovieAttributes.java
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
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;

namespace com.flagstone.transform
{
    /// <summary>
	/// MovieHeader contains the attributes that make up the header fields of a Flash
	/// file. Previously these were attributes of the Movie class.
	/// </summary>
	public sealed class MovieHeader : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Header: { version=%d; compressed=%b;" + " frameSize=%s; frameRate=%f; frameCount=%d}";

		/// <summary>
		/// The Flash version number. </summary>
		private int version;
		/// <summary>
		/// The Flash Player screen coordinates. </summary>
		private Bounds frameSize;
		/// <summary>
		/// The frame rate of the movie. </summary>
		private int frameRate;
		/// <summary>
		/// The number of frames in the movie. </summary>
		private int frameCount;
		/// <summary>
		/// Flag indicating whether the movie is compressed. </summary>
		private bool compressed;

		/// <summary>
		/// Creates and initialises a MovieAttributes object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <param name="context">
		///            a Context object used to manage the decoders for different
		///            type of object and to pass information on how objects are
		///            decoded.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public MovieHeader(SWFDecoder coder, Context context)
		{
			version = context.get(Context.VERSION);
			compressed = context.get(Context.COMPRESSED) == 1;
			frameSize = new Bounds(coder);
			frameRate = coder.readUnsignedShort();
			frameCount = coder.readUnsignedShort();
		}

		/// <summary>
		/// Construct a new MovieHeader with Flash version and compression set to
		/// the values supported in this version of Transform SWF.
		/// </summary>
		public MovieHeader()
		{
			version = Movie.VERSION;
			compressed = true;
		}

		/// <summary>
		/// Creates and initialises a MovieAttributes object using the values copied
		/// from another MovieAttributes object.
		/// </summary>
		/// <param name="object">
		///            a MovieAttributes object from which the values will be
		///            copied. </param>


		public MovieHeader(MovieHeader @object)
		{
			version = @object.version;
			compressed = @object.compressed;
			frameSize = @object.frameSize;
			frameRate = @object.frameRate;
			frameCount = @object.frameCount;
		}

		/// <summary>
		/// Get the number representing the version of Flash that the movie
		/// represents.
		/// </summary>
		/// <returns> the version number of Flash that this movie contains. </returns>
		public int Version
		{
			get => version;
		    set
			{
				if (value < 0)
				{
					throw new IllegalArgumentRangeException(0, int.MaxValue, value);
				}
				version = value;
			}
		}


		/// <summary>
		/// Get the bounding rectangle that defines the size of the player
		/// screen.
		/// </summary>
		/// <returns> the bounding box that defines the screen. </returns>
		public Bounds FrameSize
		{
			get => frameSize;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				frameSize = value;
			}
		}


		/// <summary>
		/// Get the number of frames played per second that the movie will be
		/// displayed at.
		/// </summary>
		/// <returns> the movie frame rate. </returns>
		public float FrameRate
		{
			get => frameRate / Coder.SCALE_8;
		    set => frameRate = (int)(value * Coder.SCALE_8);
		}


		/// <summary>
		/// Get the number of frames in the movie. The value returned is only valid
		/// for movies that have just been decoded or encoded. The number returned
		/// will not be valid if the movie was edited.
		/// </summary>
		/// <returns> the number of frames. </returns>
		public int FrameCount
		{
			get => frameCount;
		    set => frameCount = value;
		}


		/// <summary>
		/// Is the movie compressed.
		/// </summary>
		/// <returns> true if the movie contains zlib compressed data or false if it
		/// is not compressed. </returns>
		public bool Compressed
		{
			get => compressed;
		    set => compressed = value;
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public MovieHeader copy()
		{
			return new MovieHeader(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, version, compressed, frameSize, FrameRate, frameCount);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 4 + frameSize.prepareToEncode(context);
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			frameSize.encode(coder, context);
			coder.writeShort(frameRate);
			coder.writeShort(frameCount);
		}
	}

}