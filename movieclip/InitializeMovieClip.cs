using System;
using System.Collections.Generic;
using com.flagstone.transform.action;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;
using Action = com.flagstone.transform.action.Action;

/*
 * Initialize.java
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
	/// Initialize is used to specify a sequence of actions that are executed to
	/// initialise a movie clip before it is displayed.
	/// 
	/// <para>
	/// Initialize implements the #initclip pragma defined in the ActionScript
	/// language.
	/// </para>
	/// 
	/// <para>
	/// Unlike the DoAction class which specifies the actions that are executed when
	/// a particular frame is displayed the actions contained in an Initialize object
	/// are executed only once, regardless of where the object is included in a
	/// movie. If a frame containing the Initialize object is played again the
	/// actions are skipped. Also there can only be one Initialize object for each
	/// movie clip defined in the movie.
	/// </para>
	/// </summary>
	public sealed class InitializeMovieClip : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Initialize: { identifier=%d;" + " actions=%s}";

		/// <summary>
		/// The unique identifier of the movie clip that will be initialized. </summary>
		private int identifier;
		/// <summary>
		/// The actions used to initialize the movie clip. </summary>
		private IList<Action> actions;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises an InitializeMovieClip object using values
		/// encoded in the Flash binary format.
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



		public InitializeMovieClip(SWFDecoder coder, Context context)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			actions = new List<Action>();



			SWFFactory<Action> decoder = context.Registry.ActionDecoder;

			if (decoder == null)
			{
				actions.Add(new ActionData(coder.readBytes(new byte[length - 2])));
			}
			else
			{
				while (coder.bytesRead() < length)
				{
					decoder.getObject(actions, coder, context);
				}
			}
			coder.unmark();
		}

		/// <summary>
		/// Creates a Initialize object that will initialise the movie clip with the
		/// specified identifier with the actions in the list.
		/// </summary>
		/// <param name="uid">
		///            the identifier of the movie clip to initialise. Must be in the
		///            range 1..65535. </param>
		/// <param name="list">
		///            the list of action objects. Must not be null. </param>


		public InitializeMovieClip(int uid, IList<Action> list)
		{
			Identifier = uid;
			Actions = list;
		}

		/// <summary>
		/// Creates and initialises an InitializeMovieClip object using the values
		/// copied from another InitializeMovieClip object.
		/// </summary>
		/// <param name="object">
		///            an InitializeMovieClip object from which the values will be
		///            copied. </param>


		public InitializeMovieClip(InitializeMovieClip @object)
		{
			identifier = @object.identifier;
			actions = new List<Action>(@object.actions);
		}

		/// <summary>
		/// Get the identifier of the movie clip that will be initialised.
		/// </summary>
		/// <returns> the movie clip identifier. </returns>
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
		/// Adds the action object to the list of actions.
		/// </summary>
		/// <param name="anAction">
		///            an object belonging to a class derived from Action. Must not
		///            be null. </param>
		/// <returns> this object. </returns>


		public InitializeMovieClip add(Action anAction)
		{
			if (anAction == null)
			{
				throw new ArgumentException();
			}
			actions.Add(anAction);
			return this;
		}

		/// <summary>
		/// Get the list of actions that are used to initialise the movie clip.
		/// </summary>
		/// <returns> the actions to initialize the movie clip. </returns>
		public IList<Action> Actions
		{
			get => actions;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				actions = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public InitializeMovieClip copy()
		{
			return new InitializeMovieClip(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, actions);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 2;

			foreach (Action action in actions)
			{
				length += action.prepareToEncode(context);
			}

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.INITIALIZE << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.INITIALIZE << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			foreach (Action action in actions)
			{
				action.encode(coder, context);
			}
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}