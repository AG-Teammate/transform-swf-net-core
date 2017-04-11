using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;

/*
 * With.java
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

namespace com.flagstone.transform.action
{
    /// <summary>
	/// With is a stack-based action and supports the <em>with</em> statement from
	/// the ActionScript language.
	/// 
	/// <pre>
	/// with(_root.movieClip) {
	///     gotoAndPlay(&quot;frame&quot;);
	/// }
	/// </pre>
	/// 
	/// <para>
	/// The action temporarily selects the movie clip allowing the following stream
	/// of actions to control the movie clip's time-line.
	/// </para>
	/// </summary>
	public sealed class With : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "With: { actions=%s}";

		/// <summary>
		/// The list of actions that will be executed. </summary>
		
		private readonly IList<Action> actions;

		/// <summary>
		/// The length of the action, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a With action using values encoded
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



		public With(SWFDecoder coder, Context context)
		{


			SWFFactory<Action> decoder = context.Registry.ActionDecoder;

			coder.readUnsignedShort();
			length = coder.readUnsignedShort();
			actions = new List<Action>();
			coder.mark();
			while (coder.bytesRead() < length)
			{
				decoder.getObject(actions, coder, context);
			}
			coder.unmark();
		}

		/// <summary>
		/// Creates a With object with a list of actions.
		/// </summary>
		/// <param name="list">
		///            the list of action objects. Must not be null. </param>


		public With(IList<Action> list)
		{
			if (list == null)
			{
				throw new ArgumentException();
			}
			actions = list;
		}

		/// <summary>
		/// Creates and initialises a With action using the values
		/// copied from another With action.
		/// </summary>
		/// <param name="object">
		///            a With action from which the values will be
		///            copied. </param>


		public With(With @object)
		{
			actions = new List<Action>(@object.actions);
		}

		/// <summary>
		/// Get the list of actions that are executed for the movie clip target.
		/// </summary>
		/// <returns> a copy of the list of actions that will be executed. </returns>
		public IList<Action> Actions => new List<Action>(actions);

	    /// <summary>
		/// {@inheritDoc} </summary>
		public With copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, actions);
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

			return Coder.ACTION_HEADER + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ActionTypes.WITH);
			coder.writeShort(2);
			coder.writeShort(length - 2);

			foreach (Action action in actions)
			{
				action.encode(coder, context);
			}
		}
	}

}