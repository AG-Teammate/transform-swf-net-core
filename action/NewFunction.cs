using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;

/*
 * NewFunction.java
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
	/// The NewFunction action is used to create a user-defined function.
	/// 
	/// <para>
	/// User-defined functions are also used to create methods for user-defined
	/// objects. The name of the function is omitted and the function definition is
	/// assigned to a variable which allows it to be referenced at a later time.
	/// </para>
	/// 
	/// <para>
	/// In the actions which form the function body all the arguments passed to the
	/// function can be referenced by the name supplied in the arguments list.
	/// </para>
	/// 
	/// <para>
	/// All the action objects created are owned by the function. They will be
	/// deleted when the function definition is deleted.
	/// </para>
	/// </summary>
	/// <seealso cref= NewFunction2 </seealso>
	public sealed class NewFunction : Action
	{

		/// <summary>
		/// The Builder class is used to generate a new NewFunction object
		/// using a small set of convenience methods.
		/// </summary>
		public sealed class Builder
		{
			/// <summary>
			/// The name, if any, for the function. </summary>
			
			internal string name = "";
			/// <summary>
			/// The function arguments. </summary>
			
			internal readonly IList<string> arguments = new List<string>();
			/// <summary>
			/// The list of actions that make up the function body. </summary>
			
			internal readonly IList<Action> actions = new List<Action>();

			/// <summary>
			/// Set the name of the function. Must not be null or an empty string.
			/// The name defaults to an empty string so this method is not needed to
			/// define methods.
			/// </summary>
			/// <param name="aString"> the name of the function. </param>
			/// <returns> this object. </returns>


			public Builder setName(string aString)
			{
				if (ReferenceEquals(aString, null) || aString.Length == 0)
				{
					throw new ArgumentException();
				}
				name = aString;
				return this;
			}

			/// <summary>
			/// Add the name of an argument to the list of arguments that will be
			/// passed to the function. Must not be null of an empty string. </summary>
			/// <param name="argName"> the name of the argument. </param>
			/// <returns> this object. </returns>


			public Builder setArgument(string argName)
			{
				if (ReferenceEquals(argName, null) || argName.Length == 0)
				{
					throw new ArgumentException();
				}
				arguments.Add(argName);
				return this;
			}

			/// <summary>
			/// Add an action to the list of actions that will make up the body of
			/// the function. Must not be null. </summary>
			/// <param name="action"> the action to add to the function body. </param>
			/// <returns> this object. </returns>


			public Builder addAction(Action action)
			{
				if (action == null)
				{
					throw new ArgumentException();
				}
				actions.Add(action);
				return this;
			}

			/// <summary>
			/// Generate an NewFunction using the parameters defined in the
			/// Builder. </summary>
			/// <returns> an initialized NewFunction object. </returns>
			public NewFunction build()
			{
				return new NewFunction(this);
			}
		}

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "NewFunction: { name=%s;" + " arguments=%s; actions=%s}";

		/// <summary>
		/// The name of the function or an empty string if a method. </summary>
		
		private readonly string name;
		/// <summary>
		/// The names of the arguments. </summary>
		
		private readonly IList<string> arguments;
		/// <summary>
		/// The actions that make up the function body. </summary>
		
		private readonly IList<Action> actions;

		/// <summary>
		/// The length of the action, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// The length of the actions when encoded. </summary>
		
		private int actionsLength;

		/// <summary>
		/// Creates and initialises a NewFunction object using parameters defined
		/// in the Builder.
		/// </summary>
		/// <param name="builder"> a Builder object containing the parameters to generate
		/// the function definition. </param>


		public NewFunction(Builder builder)
		{
			name = builder.name;
			arguments = new List<string>(builder.arguments);
			actions = new List<Action>(builder.actions);
		}

		/// <summary>
		/// Creates and initialises a NewFunction definition using values encoded
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



		public NewFunction(SWFDecoder coder, Context context)
		{
			length = coder.readUnsignedShort();

			name = coder.readString();



			int argumentCount = coder.readUnsignedShort();

			arguments = new List<string>(argumentCount);

			if (argumentCount > 0)
			{
				for (int i = argumentCount; i > 0; i--)
				{
					arguments.Add(coder.readString());
				}
			}



			SWFFactory<Action> decoder = context.Registry.ActionDecoder;
			actions = new List<Action>();

			actionsLength = coder.readUnsignedShort();
			coder.mark();

			while (coder.bytesRead() < actionsLength)
			{
				decoder.getObject(actions, coder, context);
			}

			coder.unmark();
		}

		/// <summary>
		/// Creates a NewFunction with the specified name, argument names and actions
		/// to be executed. The order of the Strings in the argument list indicate
		/// the order in which the values will be popped off the stack when the
		/// function is executed. The fist argument is popped from the stack first.
		/// </summary>
		/// <param name="aString">
		///            the name of the function. May not be null. </param>
		/// <param name="argumentArray">
		///            the list of Strings giving the names of the arguments. </param>
		/// <param name="actionArray">
		///            the list of actions that define the operation performed by
		///            the function. </param>


		public NewFunction(string aString, IList<string> argumentArray, IList<Action> actionArray)
		{
			if (ReferenceEquals(aString, null))
			{
				throw new ArgumentException();
			}
			name = aString;
			if (argumentArray == null)
			{
				throw new ArgumentException();
			}
			arguments = argumentArray;

			if (actionArray == null)
			{
				throw new ArgumentException();
			}
			actions = actionArray;
		}

		/// <summary>
		/// Creates a anonymous NewFunction with the specified argument names and
		/// actions to be executed. Use this constructor when defining functions that
		/// will be assigned to object variables and used as methods.
		/// </summary>
		/// <param name="argumentArray">
		///            a list of Strings giving the names of the arguments. </param>
		/// <param name="actionArray">
		///            a list of actions that define the operation performed by
		///            the function. </param>


		public NewFunction(IList<string> argumentArray, IList<Action> actionArray) : this("", argumentArray, actionArray)
		{
		}

		/// <summary>
		/// Creates and initialises a NewFunction action using the values
		/// copied from another NewFunction action.
		/// </summary>
		/// <param name="object">
		///            a NewFunction action from which the values will be
		///            copied. References to immutable objects will be shared. </param>


		public NewFunction(NewFunction @object)
		{
			name = @object.name;

			arguments = new List<string>(@object.arguments);
			actions = new List<Action>(@object.actions);
		}

		/// <summary>
		/// Get the name of the function. If the function will be used as an object
		/// method then the name is an empty string.
		/// </summary>
		/// <returns> the name of the function or an empty string. </returns>
		public string Name => name;

	    /// <summary>
		/// Get the names of the function arguments.
		/// </summary>
		/// <returns> a list of argument names in the order they appear in the
		/// function definition. </returns>
		public IList<string> Arguments => new List<string>(arguments);

	    /// <summary>
		/// Get the actions that will be executed.
		/// </summary>
		/// <returns> the actions that define the function behaviour. </returns>
		public IList<Action> Actions => new List<Action>(actions);

	    /// <summary>
		/// {@inheritDoc} </summary>
		public NewFunction copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, name, arguments, actions);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 2 + context.strlen(name);

			foreach (String argument in arguments)
			{
				length += context.strlen(argument);
			}

			length += 2;
			actionsLength = 0;

			foreach (Action action in actions)
			{
				actionsLength += action.prepareToEncode(context);
			}

			return Coder.ACTION_HEADER + length + actionsLength;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ActionTypes.NEW_FUNCTION);
			coder.writeShort(length);

			coder.writeString(name);

			coder.writeShort(arguments.Count);

			foreach (String argument in arguments)
			{
				coder.writeString(argument);
			}

			coder.writeShort(actionsLength);

			foreach (Action action in actions)
			{
				action.encode(coder, context);
			}
		}
	}

}