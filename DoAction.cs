using System;
using System.Collections.Generic;
using com.flagstone.transform.action;
using com.flagstone.transform.coder;
using Action = com.flagstone.transform.action.Action;

/*
 * DoAction.java
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
	/// DoAction is used to add a set of actions to a frame in a movie. The actions
	/// will be triggered when the Flash Player executes the ShowFrame command.
	/// 
	/// <para>
	/// Only one DoAction object can be used to specify the actions for a given
	/// frame. If more than one DoAction object is added in a single frame only the
	/// actions contained in the last DoAction object (before the ShowFrame object)
	/// will be executed when the frame is displayed. The other DoAction objects will
	/// be ignored.
	/// </para>
	/// 
	/// <para>
	/// IMPORTANT: The last action in the list must be BasicAction.END otherwise
	/// the object will not be encoded correctly.
	/// </para>
	/// 
	/// <para>
	/// When decoding a movie, if the decode actions flag is set to false then the
	/// actions will be decoded as a single ActionData object containing the encoded
	/// actions.
	/// </para>
	/// 
	/// <para>
	/// DoAction can only be used in movies that contain Actionscript 1.x or
	/// Actionscript 2.x code. For Actionscript 3.0 use the DoABC class.
	/// </para>
	/// </summary>
	public sealed class DoAction : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DoAction: { actions=%s}";
		/// <summary>
		/// The actions executed when the current frame is displayed. </summary>
		private IList<Action> actions;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a DoAction object using values encoded in the
		/// Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data. </param>
		/// <param name="context">
		///            a Context object used to pass values when decoding objects. </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public DoAction(SWFDecoder coder, Context context)
		{



			SWFFactory<Action> decoder = context.Registry.ActionDecoder;
			actions = new List<Action>();

			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();

			if (decoder == null)
			{
				actions.Add(new ActionData(coder.readBytes(new byte[length])));
			}
			else
			{
				while (coder.bytesRead() < length)
				{
					decoder.getObject(actions, coder, context);
				}
			}
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a new DoAction class with an empty list.
		/// </summary>
		public DoAction()
		{
			actions = new List<Action>();
		}

		/// <summary>
		/// Creates a DoAction object with a list of actions.
		/// </summary>
		/// <param name="list">
		///            the list of action objects. Cannot be null. </param>


		public DoAction(IList<Action> list)
		{
			Actions = list;
		}

		/// <summary>
		/// Creates a DoAction object with a copy of the actions from another
		/// DoAction object.
		/// </summary>
		/// <param name="object">
		///            a DoAction object to copy. </param>


		public DoAction(DoAction @object)
		{
			actions = new List<Action>(@object.actions);
		}

		/// <summary>
		/// Adds the action object to the list of actions. If the object already
		/// contains encoded actions then they will be deleted.
		/// </summary>
		/// <param name="anAction">
		///            an object belonging to a class derived from Action. The
		///            argument cannot be null.
		/// </param>
		/// <returns> this object. </returns>


		public DoAction add(Action anAction)
		{
			if (anAction == null)
			{
				throw new ArgumentException();
			}
			actions.Add(anAction);
			return this;
		}

		/// <summary>
		/// Returns the list of actions that are executed when the frame is
		/// displayed.
		/// </summary>
		/// <returns> the list of action objects. </returns>
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
		public DoAction copy()
		{
			return new DoAction(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, actions.ToString());
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 0;

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
				coder.writeShort((MovieTypes.DO_ACTION << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DO_ACTION << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
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