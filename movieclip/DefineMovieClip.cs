using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * DefineMovieClip.java
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

namespace com.flagstone.transform.movieclip
{
    /// <summary>
	/// DefineMovieClip defines a movie clip that animates shapes within a movie. It
	/// contains an list of movie objects that define the placement of shapes,
	/// buttons, text and images and the order in which they are displayed through a
	/// time-line that is separate from the parent movie.
	/// 
	/// <para>
	/// Although a movie clip contains the commands that instructs the Flash Player
	/// on how to animate the clip it cannot contain any new definitions of objects.
	/// All definitions must be in the main movie. All objects referred to by the
	/// movie clip must be also defined in the main movie before they can be used.
	/// </para>
	/// 
	/// <para>
	/// When using the DefineMovieClip object can only contain objects from the
	/// following classes: ShowFrame, Place, Place2, Place3, Remove, Remove2
	/// DoAction, StartSound, StartSound2, FrameLabel, SoundStreamHead,
	/// SoundStreamHead2 or SoundStreamBlock. Other objects are not allowed.
	/// </para>
	/// </summary>
	public sealed class DefineMovieClip : DefineTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineMovieClip: { identifier=%d;" + " objects=%s}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The list of objects that describe how the movie clip is animated. </summary>
		private IList<MovieTag> objects;

		/// <summary>
		/// The number of frames in the movie clip, when it is encoded. </summary>
		
		private int frameCount;
		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a DefineMovieClip object using values encoded
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



		public DefineMovieClip(SWFDecoder coder, Context context)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			identifier = coder.readUnsignedShort();
			frameCount = coder.readUnsignedShort();
			objects = new List<MovieTag>();



			SWFFactory<MovieTag> decoder = context.Registry.MovieDecoder;

			while (coder.scanUnsignedShort() >> Coder.LENGTH_FIELD_SIZE != MovieTypes.END)
			{
			   decoder.getObject(objects, coder, context);
			}
			coder.readUnsignedShort(); // END
		}

		/// <summary>
		/// Creates a DefineMovieClip object with the unique identifier and list of
		/// movie objects.
		/// </summary>
		/// <param name="uid">
		///            a unique identifier for the movie clip. Must be in the range
		///            1..65535, </param>
		/// <param name="list">
		///            the list of movie objects. Must not be null. </param>


		public DefineMovieClip(int uid, IList<MovieTag> list)
		{
			Identifier = uid;
			Objects = list;
		}

		/// <summary>
		/// Creates and initialises a DefineMovieClip object using the values copied
		/// from another DefineMovieClip object.
		/// </summary>
		/// <param name="object">
		///            a DefineMovieClip object from which the values will be
		///            copied. </param>


		public DefineMovieClip(DefineMovieClip @object)
		{
			identifier = @object.identifier;
			objects = new List<MovieTag>(@object.objects.Count);
			foreach (MovieTag tag in @object.objects)
			{
				objects.Add(tag.copy());
			}
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public int Identifier
		{
			get => identifier;
		    set
			{
				if ((value < 1) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, value);
				}
				identifier = value;
			}
		}


		/// <summary>
		/// Adds the movie object to the list of objects that update the display
		/// list. See description above for the list of acceptable types.
		/// </summary>
		/// <param name="obj">
		///            a Movie object. Must not be null
		/// </param>
		/// <returns> this object. </returns>


		public DefineMovieClip add(MovieTag obj)
		{
			if (obj == null)
			{
				throw new ArgumentException();
			}
			objects.Add(obj);
			return this;
		}

		/// <summary>
		/// Get the list of movie objects that describe how the movie clip is
		/// animated.
		/// </summary>
		/// <returns> the list of objects for the movie clip. </returns>
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
		/// {@inheritDoc} </summary>
		public DefineMovieClip copy()
		{
			return new DefineMovieClip(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, objects);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{

			frameCount = 0;
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			length = 6;

			foreach (MovieTag @object in objects)
			{
				length += @object.prepareToEncode(context);

				if (@object is ShowFrame)
				{
					frameCount += 1;
				}
			}
			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_MOVIE_CLIP << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_MOVIE_CLIP << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			coder.writeShort(frameCount);

			foreach (MovieTag @object in objects)
			{
				@object.encode(coder, context);
			}
			coder.writeShort(0);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}