using System;
using com.flagstone.transform.coder;

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

namespace com.flagstone.transform
{
    /// <summary>
	/// The MovieAttributes tag defines characteristics of a Movie. It contains
	/// several flags to indicate types of objects in the movie and whether any
	/// hardware acceleration should be used if available.
	/// 
	/// For Flash Version 8 and above it must be the first object after the
	/// MovieHeader.
	/// </summary>
	public sealed class MovieAttributes : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "MovieAttributes: {" + " metadata=%b;  as3=%b;  network=%b; gpu=%b; directBlit=%b}";
		/// <summary>
		/// The set of encoded attributes. </summary>
		
		private int attributes;

		/// <summary>
		/// Creates a new MovieAttributes object.
		/// </summary>
		public MovieAttributes()
		{
			// Empty
		}

		/// <summary>
		/// Creates and initialises a MovieAttributes object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public MovieAttributes(SWFDecoder coder)
		{
			int length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			attributes = coder.readByte();
			coder.skip(length - 1);
		}

		/// <summary>
		/// Creates and initialises a MovieAttributes object using the values copied
		/// from another MovieAttributes object.
		/// </summary>
		/// <param name="object">
		///            a MovieAttributes object from which the values will be
		///            copied. </param>


		public MovieAttributes(MovieAttributes @object)
		{
			attributes = @object.attributes;
		}

		/// <summary>
		/// Does the Movie contain Actionscript 3 code.
		/// </summary>
		/// <returns> true if the movie contains at least one DoABC tag
		/// containing Actionscript 3 byte-codes. </returns>
		public bool hasMetaData()
		{
			return (attributes & Coder.BIT4) != 0;
		}

		/// <summary>
		/// Does the Movie contain meta-data.
		/// </summary>
		/// <returns> true if the movie contains a MovieMetaData tag. </returns>
		public bool hasAS3()
		{
			return (attributes & Coder.BIT3) != 0;
		}

		/// <summary>
		/// Does the Flash Player use direct bit block transfer to accelerate
		/// graphics.
		/// </summary>
		/// <returns> true if the Flash Player will use direct bit block transfer. </returns>
		public bool useDirectBlit()
		{
			return (attributes & Coder.BIT6) != 0;
		}

		/// <summary>
		/// Instruct the Flash Player to use direct bit block transfer to accelerate
		/// graphics.
		/// </summary>
		/// <param name="useBlit"> use graphics hardware accelerations. </param>


		public bool UseDirectBlit
		{
			set
			{
				if (value)
				{
					attributes |= Coder.BIT6;
				}
				else
				{
					attributes &= ~Coder.BIT6;
				}
			}
		}

		/// <summary>
		/// Does the Flash Player use the graphics processor to accelerate
		/// compositing - if available.
		/// </summary>
		/// <returns> true if the Flash Player will use the graphics process for
		/// compositing. </returns>
		public bool useGPU()
		{
			return (attributes & Coder.BIT5) != 0;
		}

		/// <summary>
		/// Instruct the Flash Player to use the graphics processor to accelerate
		/// compositing - if available.
		/// </summary>
		/// <param name="useGPU"> use graphics processor for compositing. </param>


		public bool UseGPU
		{
			set
			{
				if (value)
				{
					attributes |= Coder.BIT5;
				}
				else
				{
					attributes &= ~Coder.BIT5;
				}
			}
		}

		/// <summary>
		/// Does the Flash Player use the network for loading resources even if the
		/// movie is loaded from the local file system. </summary>
		/// <returns> true if the network will be used even if the movie is loaded
		/// locally, false otherwise. </returns>
		public bool useNetwork()
		{
			return (attributes & Coder.BIT0) != 0;
		}

		/// <summary>
		/// Instructor the Flash Player use the network for loading resources even
		/// if the movie is loaded from the local file system. </summary>
		/// <param name="useNetwork"> use the network even if the movie is loaded locally. </param>


		public bool UseNetwork
		{
			set
			{
				if (value)
				{
					attributes |= Coder.BIT0;
				}
				else
				{
					attributes &= ~Coder.BIT0;
				}
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public MovieAttributes copy()
		{
			return new MovieAttributes(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, hasMetaData(), hasAS3(), useNetwork(), useGPU(), useDirectBlit());
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 6;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 2 LINES
			coder.writeShort((MovieTypes.FILE_ATTRIBUTES << Coder.LENGTH_FIELD_SIZE) | 4);
			coder.writeByte(attributes);
			coder.writeByte(0);
			coder.writeByte(0);
			coder.writeByte(0);
		}
	}

}