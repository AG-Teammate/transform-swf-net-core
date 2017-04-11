using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * NewFunction2.java
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
	/// The NewFunction2 action is used to create a user-defined function with
	/// optimisations to improve performance.
	/// 
	/// <para>
	/// NewFunction2 was added in Flash 7 to improve the performance of function
	/// calls by allowing pre-defined variables such as <em>_root</em>,
	/// <em>_parent</em>, <em>_global</em>, <em>super</em>, <em>this</em> and the
	/// <em>arguments</em> passed to the function to be pre-loaded to a set of up to
	/// 256 internal registers.
	/// </para>
	/// 
	/// <para>
	/// The optimisation attribute is a compound code, containing a number of flags
	/// that control which variables are pre-loaded:
	/// </para>
	/// 
	/// <table class="datasheet">
	/// <tr>
	/// <td valign="top">CreateSuper</td>
	/// <td>Create and initialise the <em>super</em> variable with the parent class
	/// of the function.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">CreateArguments</td>
	/// <td>Create the <em>arguments</em> variable which contains the arguments
	/// passed to the function.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">CreateThis</td>
	/// <td>Create and initialise the <em>this</em> variable with the object.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">LoadThis</td>
	/// <td>Pre-load the <em>this</em> variable into register number 1.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">LoadArguments</td>
	/// <td>Pre-load the <em>parent</em> variable into register number 2.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">LoadSuper</td>
	/// <td>Pre-load the <em>super</em> variable into register number 3.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">LoadRoot</td>
	/// <td>Pre-load the <em>_root</em> variable into register number 4.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">LoadParent</td>
	/// <td>Pre-load the <em>_parent</em> variable into register number 5.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">LoadGlobal</td>
	/// <td>Pre-load the <em>_global</em> variable into register number 5.</td>
	/// </tr>
	/// </table>
	/// 
	/// <para>
	/// The register numbers that the predefined variables are assigned to are fixed.
	/// When specifying which of the functions arguments are also assigned to
	/// registers it is important avoid these locations otherwise the variables will
	/// be overwritten.
	/// </para>
	/// 
	/// <para>
	/// User-defined functions are also used to create methods for user-defined
	/// objects. The name of the function is omitted and the function definition is
	/// assigned to a variable which allows it to be referenced at a alter time. See
	/// the example below.
	/// </para>
	/// 
	/// <para>
	/// The arguments supplied to the function can be referenced by the name supplied
	/// in the arguments list.
	/// </para>
	/// 
	/// <para>
	/// All the action objects added are owned by the function. They will be deleted
	/// when the function definition is deleted.
	/// </para>
	/// </summary>
	/// <seealso cref= NewFunction </seealso>
	public sealed class NewFunction2 : Action
	{

		/// <summary>
		/// Number of last internal register in the Flash Player. </summary>
		private const int LAST_REGISTER = 255;

		/// <summary>
		/// The Builder class is used to generate a new NewFunction2 object
		/// using a small set of convenience methods.
		/// </summary>
		public sealed class Builder
		{
			/// <summary>
			/// The name, if any, for the function. </summary>
			
			internal string name = "";
			/// <summary>
			/// The number of registers to allocate for use by the function. </summary>
			
			internal int registerCount;
			/// <summary>
			/// The set of optimizations to speed the function. </summary>
			
			internal int optimizations;
			/// <summary>
			/// The set of arguments, with optional register assignments. </summary>
			
			internal readonly IDictionary<string, int?> arguments = new Dictionary<string, int?>();
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
			/// Set the number of registers to allocate for the function. </summary>
			/// <param name="count"> the number of registers. Must be in the range 0..255. </param>
			/// <returns> this object. </returns>


			public Builder allocate(int count)
			{
				if ((count < 0) || (count > LAST_REGISTER))
				{
					throw new IllegalArgumentRangeException(0, LAST_REGISTER, count);
				}
				registerCount = count;
				return this;
			}

			/// <summary>
			/// Add an Optimization to be used by the function. </summary>
			/// <param name="opt"> an Optimization used to speed up the function execution. </param>
			/// <returns> this object. </returns>


			public Builder optimize(Optimization opt)
			{
				optimizations |= opt.Value;
				return this;
			}

			/// <summary>
			/// Add the name of an argument to the list of arguments that will be
			/// passed to the function. Must not be null or an empty string. </summary>
			/// <param name="argName"> the name of the argument. </param>
			/// <returns> this object. </returns>


			public Builder addArgument(string argName)
			{
				return addArgument(argName, 0);
			}

			/// <summary>
			/// Add the name of an argument and the number of the register where it
			/// will be stored to the list of arguments that will be
			/// passed to the function. The name must not be null or an empty string
			/// and the register number must be in the range 0..255. If the number
			/// is set to zero then the argument will not be stored in a register.
			/// </summary>
			/// <param name="argName"> the name of the argument. </param>
			/// <param name="index"> the register number. </param>
			/// <returns> this object. </returns>


			public Builder addArgument(string argName, int index)
			{
				if (ReferenceEquals(argName, null) || argName.Length == 0)
				{
					throw new ArgumentException();
				}
				if ((index < 0) || (index > LAST_REGISTER))
				{
					throw new IllegalArgumentRangeException(0, LAST_REGISTER, index);
				}
				arguments[argName] = index;
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
			/// Generate an NewFunction2 using the parameters defined in the
			/// Builder. </summary>
			/// <returns> an initialized NewFunction2 object. </returns>
			public NewFunction2 build()
			{
				return new NewFunction2(this);
			}
		}

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "NewFunction2: { name=%s; " + "registerCount=%d; optimizations=%s; arguments=%s; actions=%s}";

		/// <summary>
		/// The set of optimisations that can be used to speed up the execution of
		/// functions.
		/// </summary>
		public sealed class Optimization
		{
			/// <summary>
			/// Create the predefined variable, <em>super</em>. </summary>
			public static readonly Optimization CREATE_SUPER = new Optimization("CREATE_SUPER", InnerEnum.CREATE_SUPER, 4);
			/// <summary>
			/// Create the predefined variable, <em>arguments</em>. </summary>
			public static readonly Optimization CREATE_ARGUMENTS = new Optimization("CREATE_ARGUMENTS", InnerEnum.CREATE_ARGUMENTS, 16);
			/// <summary>
			/// Create and initialised the predefined variable, <em>this</em>. </summary>
			public static readonly Optimization CREATE_THIS = new Optimization("CREATE_THIS", InnerEnum.CREATE_THIS, 64);
			/// <summary>
			/// Load the predefine variable, <em>this</em>, into register 1. </summary>
			public static readonly Optimization LOAD_THIS = new Optimization("LOAD_THIS", InnerEnum.LOAD_THIS, 128);
			/// <summary>
			/// Load the predefine variable, <em>arguments</em>, into register 2. </summary>
			public static readonly Optimization LOAD_ARGUMENTS = new Optimization("LOAD_ARGUMENTS", InnerEnum.LOAD_ARGUMENTS, 32);
			/// <summary>
			/// Load the predefine variable, <em>super</em>, into register 3. </summary>
			public static readonly Optimization LOAD_SUPER = new Optimization("LOAD_SUPER", InnerEnum.LOAD_SUPER, 8);
			/// <summary>
			/// Load the predefine variable, <em>_root</em>, into register 4. </summary>
			public static readonly Optimization LOAD_ROOT = new Optimization("LOAD_ROOT", InnerEnum.LOAD_ROOT, 2);
			/// <summary>
			/// Load the predefine variable, <em>_parent</em>, into register 5. </summary>
			public static readonly Optimization LOAD_PARENT = new Optimization("LOAD_PARENT", InnerEnum.LOAD_PARENT, 1);
			/// <summary>
			/// Load the predefine variable, <em>_global</em>, into register 6. </summary>
			public static readonly Optimization LOAD_GLOBAL = new Optimization("LOAD_GLOBAL", InnerEnum.LOAD_GLOBAL, 32768);

			private static readonly IList<Optimization> valueList = new List<Optimization>();

			public enum InnerEnum
			{
				CREATE_SUPER,
				CREATE_ARGUMENTS,
				CREATE_THIS,
				LOAD_THIS,
				LOAD_ARGUMENTS,
				LOAD_SUPER,
				LOAD_ROOT,
				LOAD_PARENT,
				LOAD_GLOBAL
			}

			public readonly InnerEnum innerEnumValue;
			private readonly string nameValue;
			private readonly int ordinalValue;
			private static int nextOrdinal;

			/// <summary>
			/// Table used to convert flags into Optimization values. </summary>
			internal static readonly IDictionary<int?, Optimization> TABLE;

			static Optimization()
			{
				TABLE = new Dictionary<int?, Optimization>();

				foreach (Optimization opt in values())
				{
					TABLE[opt.value] = opt;
				}

				valueList.Add(CREATE_SUPER);
				valueList.Add(CREATE_ARGUMENTS);
				valueList.Add(CREATE_THIS);
				valueList.Add(LOAD_THIS);
				valueList.Add(LOAD_ARGUMENTS);
				valueList.Add(LOAD_SUPER);
				valueList.Add(LOAD_ROOT);
				valueList.Add(LOAD_PARENT);
				valueList.Add(LOAD_GLOBAL);
			}

			/// <summary>
			/// The encoded value for the Optimization. </summary>
			internal readonly int value;

			/// <summary>
			/// Creates and initializes an Optimization for an encoded value.
			/// </summary>
			/// <param name="val"> the encoded value for an Optimization. </param>


			internal Optimization(string name, InnerEnum innerEnum, int val)
			{
				value = val;

				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			/// <summary>
			/// Get the value used to represent the Optimization when encoded. </summary>
			/// <returns> the value used to encode the Optimization. </returns>
			public int Value => value;

		    public static IList<Optimization> values()
			{
				return valueList;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static Optimization valueOf(string name)
			{
				foreach (Optimization enumInstance in valueList)
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new ArgumentException(name);
			}
		}

		/// <summary>
		/// Initial number of bytes when encoding. </summary>
		private const int INITIAL_LENGTH = 5;

		/// <summary>
		/// The name of the function or an empty string for methods. </summary>
		
		private readonly string name;
		/// <summary>
		/// The number of registers to allocate for variables. </summary>
		
		private readonly int registerCount;
		/// <summary>
		/// The set of flags identifying optimizations to be applied. </summary>
		
		private readonly int optimizations;
		/// <summary>
		/// The set of arguments with optional assignment to registers. </summary>
		
		private readonly IDictionary<string, int?> arguments;
		/// <summary>
		/// The set of actions that make up the function body. </summary>
		
		private readonly IList<Action> actions;

		/// <summary>
		/// The length of the action, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// The length of the encoded function body. </summary>
		
		private int actionsLength;

		/// <summary>
		/// Creates and initialises a NewFunction2 object using parameters defined
		/// in the Builder.
		/// </summary>
		/// <param name="builder"> a Builder object containing the parameters to generate
		/// the function definition. </param>


		public NewFunction2(Builder builder)
		{
			name = builder.name;
			registerCount = builder.registerCount;
			optimizations = builder.optimizations;
			arguments = new Dictionary<string, int?>(builder.arguments);
			actions = new List<Action>(builder.actions);
		}

		/// <summary>
		/// Creates and initialises a NewFunction2 definition using values encoded
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




		public NewFunction2(SWFDecoder coder, Context context)
		{


			SWFFactory<Action> decoder = context.Registry.ActionDecoder;

			length = coder.readUnsignedShort();
			name = coder.readString();


			int argumentCount = coder.readUnsignedShort();
			registerCount = coder.readByte();
			optimizations = (coder.readByte() << Coder.TO_UPPER_BYTE) + coder.readByte();

			int index;

			arguments = new Dictionary<string, int?>(argumentCount);

			for (int i = 0; i < argumentCount; i++)
			{
				index = coder.readByte();
				arguments[coder.readString()] = index;
			}

			actionsLength = coder.readUnsignedShort();
			coder.mark();
			length += actionsLength;
			actions = new List<Action>();

			while (coder.bytesRead() < actionsLength)
			{
				decoder.getObject(actions, coder, context);
			}
			coder.check(actionsLength);
			coder.unmark();
		}

		/// <summary>
		/// Creates a NewFunction2 with the specified name, argument names and
		/// actions to be executed. The order of the Strings in the argument list
		/// indicate the order in which the values will be popped off the stack when
		/// the function is executed. The first argument is popped from the stack
		/// first.
		/// </summary>
		/// <param name="aString">
		///            the name of the function. Can be an empty string if the
		///            function is anonymous. </param>
		/// <param name="count">
		///            the number of registers to allocate for variables. </param>
		/// <param name="opts">
		///            the set of optimizations that will be applied to boost
		///            function performance. </param>
		/// <param name="map">
		///            the arguments and any register numbers they will be
		///            assigned to (zero for no assignment). </param>
		/// <param name="list">
		///            the list of actions that define the operation performed by
		///            the function. </param>


		public NewFunction2(string aString, int count, ISet<Optimization> opts, IDictionary<string, int?> map, IList<Action> list)
		{

			if (ReferenceEquals(aString, null) || aString.Length == 0)
			{
				throw new ArgumentException();
			}
			name = aString;

			if ((count < 0) || (count > LAST_REGISTER))
			{
				throw new IllegalArgumentRangeException(0, LAST_REGISTER, count);
			}
			registerCount = count;

			int value = 0;
			foreach (Optimization opt in opts)
			{
				value |= opt.Value;
			}
			optimizations = value;

			if (map == null)
			{
				throw new ArgumentException();
			}
			arguments = map;

			if (list == null)
			{
				throw new ArgumentException();
			}
			actions = list;
		}

		/// <summary>
		/// Creates and initialises a NewFunction2 action using the values
		/// copied from another NewFunction2 action.
		/// </summary>
		/// <param name="object">
		///            a NewFunction2 action from which the values will be
		///            copied. References to immutable objects will be shared. </param>


		public NewFunction2(NewFunction2 @object)
		{
			name = @object.name;
			registerCount = @object.registerCount;
			optimizations = @object.optimizations;
			arguments = new Dictionary<string, int?>(@object.arguments);
			actions = new List<Action>(@object.actions);
		}

		/// <summary>
		/// Get the name of the function. If the function will be used as an object
		/// method then the name is an empty string.
		/// </summary>
		/// <returns> the name of the function or an empty string. </returns>
		public string Name => name;

	    /// <summary>
		/// Get the number of registers to allocate for function variables.
		/// </summary>
		/// <returns> the number of registers to allocate. </returns>
		public int RegisterCount => registerCount;

	    /// <summary>
		/// Get the list of Optimizations that will be used.
		/// </summary>
		/// <returns> the set of optimizations to increase performance. </returns>
		public ISet<Optimization> Optimizations => throw new NotImplementedException();

	    /// <summary>
		/// Get the list of RegisterVariables that define the function arguments
		/// and whether they are assigned to internal registers or to local variables
		/// in memory.
		/// </summary>
		/// <returns> a copy of the function arguments with optional register
		/// assignments. </returns>
		public IDictionary<string, int?> Arguments => new Dictionary<string, int?>(arguments);

	    /// <summary>
		/// Get the actions executed by the function.
		/// </summary>
		/// <returns> a copy of the list of actions that make up the function body. </returns>
		public IList<Action> Actions => new List<Action>(actions);

	    /// <summary>
		/// {@inheritDoc} </summary>
		public NewFunction2 copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, name, registerCount, optimizations, arguments, actions);
		}


		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = INITIAL_LENGTH + context.strlen(name);

			foreach (String arg in arguments.Keys)
			{
			    length += context.strlen(arg) + 2;
            }

			length += 2;

			if (actions.Count == 0)
			{
				actionsLength = 1;
			}
			else
			{
				actionsLength = 0;
			}

			foreach (Action action in actions)
			{
				actionsLength += action.prepareToEncode(context);
			}

			length += actionsLength;

			return Coder.ACTION_HEADER + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ActionTypes.NEW_FUNCTION_2);
			coder.writeShort(length - actionsLength);

			coder.writeString(name);
			coder.writeShort(arguments.Count);
			coder.writeByte(registerCount);
			coder.writeByte((int)((uint)optimizations >> Coder.TO_LOWER_BYTE));
			coder.writeByte(optimizations);

			foreach (String arg in arguments.Keys)
			{
				coder.writeByte(arguments[arg].Value);
				coder.writeString(arg);
			}

			coder.writeShort(actionsLength);

			foreach (Action action in actions)
			{
				action.encode(coder, context);
			}

			if (actions.Count == 0)
			{
				coder.writeByte(0);
			}
		}
	}

}