using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * ExceptionHandler.java
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
	/// The ExceptionHandler class is used to represent try..catch blocks in
	/// Actionscript.
	/// 
	/// <para>
	/// When an exception is thrown, the object can be assigned to either one of the
	/// Flash Player's 256 internal registers or to a variable in memory.
	/// </para>
	/// 
	/// <para>
	/// The ExceptionHandler class contains three lists of actions supporting the
	/// standard syntax for an exception with try, catch and finally blocks. Both the
	/// catch and finally blocks are optional when defining an exception, the
	/// corresponding arguments in constructors and methods may be set to empty.
	/// </para>
	/// </summary>
	public sealed class ExceptionHandler : Action
	{

		/// <summary>
		/// The Builder class is used to generate a new ExceptionHandler object
		/// using a small set of convenience methods.
		/// </summary>
		public sealed class Builder
		{
			/// <summary>
			/// The register where the thrown object will be stored. </summary>
			
			internal int register;
			/// <summary>
			/// The name of the variable where the thrown object will be stored. </summary>
			
			internal string variable;
			/// <summary>
			/// The list of actions that make up the try block. </summary>
			
			internal readonly IList<Action> tryActions = new List<Action>();
			/// <summary>
			/// The list of actions that make up the catch block. </summary>
			
			internal readonly IList<Action> catchActions = new List<Action>();
			/// <summary>
			/// The list of actions that make up the finally block. </summary>
			
			internal readonly IList<Action> finalActions = new List<Action>();

			/// <summary>
			/// Set the register where the thrown object will be stored.
			/// </summary>
			/// <param name="index"> the register number. Must be in the range 0..255. </param>
			/// <returns> this object. </returns>


			public Builder setRegister(int index)
			{
				if ((index < 0) || (index > HIGHEST_REGISTER))
				{
					throw new IllegalArgumentRangeException(0, HIGHEST_REGISTER, index);
				}
				variable = "";
				register = index;
				return this;
			}

			/// <summary>
			/// Set the name of the variable where thrown object will be assigned. </summary>
			/// <param name="name"> the name of the actionsctipt variable. </param>
			/// <returns> this object. </returns>


			public Builder setVariable(string name)
			{
				if (ReferenceEquals(name, null) || name.Length == 0)
				{
					throw new ArgumentException();
				}
				variable = name;
				register = 0;
				return this;
			}

			/// <summary>
			/// Add an action to the try block of the exception handler. </summary>
			/// <param name="action"> the action to the executed in the try block. </param>
			/// <returns> this object. </returns>


			public Builder addToTry(Action action)
			{
				if (action == null)
				{
					throw new ArgumentException();
				}
				tryActions.Add(action);
				return this;
			}

			/// <summary>
			/// Add an action to the catch block of the exception handler. </summary>
			/// <param name="action"> the action to the executed in the catch block. </param>
			/// <returns> this object. </returns>


			public Builder addToCatch(Action action)
			{
				if (action == null)
				{
					throw new ArgumentException();
				}
				catchActions.Add(action);
				return this;
			}

			/// <summary>
			/// Add an action to the final block of the exception handler. </summary>
			/// <param name="action"> the action to the executed in the final block. </param>
			/// <returns> this object. </returns>


			public Builder addToFinal(Action action)
			{
				if (action == null)
				{
					throw new ArgumentException();
				}
				finalActions.Add(action);
				return this;
			}

			/// <summary>
			/// Generate an ExceptionHandler using the parameters defined in the
			/// Builder. </summary>
			/// <returns> an initialized ExceptionHandler object. </returns>
			public ExceptionHandler build()
			{
				return new ExceptionHandler(this);
			}
		}

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "ExceptionHandler: { variable=%s;" + " register=%d try=%s; catch=%s; final=%s}";

		/// <summary>
		/// Bit mask used to read the containsVariable field. </summary>
		private const int VARIABLE_MASK = 0x04;
		/// <summary>
		/// Bit mask used to read the containsVariable field. </summary>
		private const int FINAL_MASK = 0x02;
		/// <summary>
		/// Bit mask used to read the containsVariable field. </summary>
		private const int CATCH_MASK = 0x01;
		/// <summary>
		/// Length of an empty exception handler with no actions. </summary>
		private const int EMPTY_LENGTH = 8;
		/// <summary>
		/// Number of registers in the FLash Player. </summary>
		private const int HIGHEST_REGISTER = 255;

		/// <summary>
		/// The number of the register that contains the thrown object. </summary>
		
		private readonly int register;
		/// <summary>
		/// The name of the variable that the thrown object will be assigned to. </summary>
		
		private readonly string variable;
		/// <summary>
		/// Set of actions where the exception might be thrown. </summary>
		
		private readonly IList<Action> tryActions;
		/// <summary>
		/// Set of actions used to process the exception. </summary>
		
		private readonly IList<Action> catchActions;
		/// <summary>
		/// Final set of actions executed, whether or not an exception occurred. </summary>
		
		private readonly IList<Action> finalActions;

		/// <summary>
		/// The length of the action, minus the header, when it is encoded. </summary>
		
		private int length;
		/// <summary>
		/// Holds the length of the try block when it is encoded. </summary>
		
		private int tryLength;
		/// <summary>
		/// Holds the length of the catch block when it is encoded. </summary>
		
		private int catchLength;
		/// <summary>
		/// Holds the length of the final block when it is encoded. </summary>
		
		private int finalLength;

		/// <summary>
		/// Creates and initialises an ExceptionHandler using parameters defined
		/// in the Builder.
		/// </summary>
		/// <param name="builder"> a Builder object containing the parameters to generate
		/// the ExceptionHandler. </param>


		public ExceptionHandler(Builder builder)
		{
			register = builder.register;
			variable = builder.variable;
			tryActions = new List<Action>(builder.tryActions);
			catchActions = new List<Action>(builder.catchActions);
			finalActions = new List<Action>(builder.finalActions);
		}

		/// <summary>
		/// Creates and initialises an ExceptionHandler action using values encoded
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



		public ExceptionHandler(SWFDecoder coder, Context context)
		{
			length = coder.readUnsignedShort();



			int flags = coder.readByte();


			bool containsVariable = (flags & VARIABLE_MASK) >> 2 == 1;


			bool containsFinal = (flags & FINAL_MASK) >> 1 == 1;


			bool containsCatch = (flags & CATCH_MASK) == 1;

			tryLength = coder.readUnsignedShort();
			catchLength = coder.readUnsignedShort();
			finalLength = coder.readUnsignedShort();

			if (length == EMPTY_LENGTH)
			{
				length += tryLength;
				length += catchLength;
				length += finalLength;
			}

			if (containsVariable)
			{
				variable = coder.readString();
				register = 0;
			}
			else
			{
				variable = "";
				register = coder.readByte();
			}

			tryActions = new List<Action>();
			catchActions = new List<Action>();
			finalActions = new List<Action>();



			SWFFactory<Action> decoder = context.Registry.ActionDecoder;

			coder.mark();
			while (coder.bytesRead() < tryLength)
			{
				decoder.getObject(tryActions, coder, context);
			}
			coder.unmark();

			if (containsCatch)
			{
				coder.mark();
				while (coder.bytesRead() < catchLength)
				{
					decoder.getObject(catchActions, coder, context);
				}
				coder.unmark();
			}

			if (containsFinal)
			{
				coder.mark();
				while (coder.bytesRead() < finalLength)
				{
					decoder.getObject(finalActions, coder, context);
				}
				coder.unmark();
			}
		}

		/// <summary>
		/// Creates a new exception handler with the thrown object assigned to a
		/// local variable.
		/// </summary>
		/// <param name="name">
		///            the name of the variable that the thrown object will be
		///            assigned to. Must not be null. </param>
		/// <param name="tryArray">
		///            actions that will be executed in the try block of the
		///            exception. Must not be null. </param>
		/// <param name="catchArray">
		///            actions that will be executed in the catch block of the
		///            exception, if one is defined. This may be empty if no
		///            catch block is required - the exception will be handled by
		///            another catch block higher in the exception tree. </param>
		/// <param name="finallyArray">
		///            actions that will be executed in the finally block of the
		///            exception, if one is defined. This may be empty if no
		///            finally block is required. </param>



		public ExceptionHandler(string name, IList<Action> tryArray, IList<Action> catchArray, IList<Action> finallyArray)
		{
			if (ReferenceEquals(name, null) || name.Length == 0)
			{
				throw new ArgumentException();
			}
			variable = name;
			register = 0;

			if (tryArray == null)
			{
				throw new ArgumentException();
			}
			tryActions = tryArray;

			if (catchArray == null)
			{
				throw new ArgumentException();
			}
			catchActions = catchArray;

			if (finallyArray == null)
			{
				throw new ArgumentException();
			}
			finalActions = finallyArray;
		}

		/// <summary>
		/// Constructs a new exception handler with the thrown object assigned to one
		/// of the Flash Player's internal registers.
		/// </summary>
		/// <param name="index">
		///            the number of the register that the thrown object will be
		///            assigned to. Must be in the range 0..255. </param>
		/// <param name="tryArray">
		///            actions that will be executed in the try block of the
		///            exception. Must not be null. </param>
		/// <param name="catchArray">
		///            actions that will be executed in the catch block of the
		///            exception, if one is defined. This may be empty if no
		///            catch block is required - the exception will be handled by
		///            another catch block higher in the exception tree. </param>
		/// <param name="finallyArray">
		///            actions that will be executed in the finally block of the
		///            exception, if one is defined. This may be empty is no
		///            finally block is required. </param>


		public ExceptionHandler(int index, IList<Action> tryArray, IList<Action> catchArray, IList<Action> finallyArray)
		{
			if ((index < 0) || (index > HIGHEST_REGISTER))
			{
				throw new IllegalArgumentRangeException(0, HIGHEST_REGISTER, index);
			}
			variable = "";
			register = index;

			if (tryArray == null)
			{
				throw new ArgumentException();
			}
			tryActions = tryArray;

			if (catchArray == null)
			{
				throw new ArgumentException();
			}
			catchActions = catchArray;

			if (finallyArray == null)
			{
				throw new ArgumentException();
			}
			finalActions = finallyArray;
		}

		/// <summary>
		/// Creates and initialises an ExceptionHandler action using the values
		/// copied from another ExceptionHandler.
		/// </summary>
		/// <param name="object">
		///            an ExceptionHandler object from which the values will be
		///            copied. References to immutable objects will be shared. </param>


		public ExceptionHandler(ExceptionHandler @object)
		{
			variable = @object.variable;
			register = @object.register;
			tryActions = new List<Action>(@object.tryActions);
			catchActions = new List<Action>(@object.catchActions);
			finalActions = new List<Action>(@object.finalActions);
		}

		/// <summary>
		/// Returns the name of the variable which the exception object is assigned
		/// to.
		/// </summary>
		/// <returns> the name of the function. Returns null if the exception object
		///         will be assigned to a register. </returns>
		public string Variable => variable;

	    /// <summary>
		/// Returns the index of the register that the exception object is assigned
		/// to.
		/// </summary>
		/// <returns> the number of register. Returns 0 if the exception object will be
		///         assigned to a local variable. </returns>
		public int Register => register;

	    /// <summary>
		/// Returns the list of actions executed in the try block.
		/// </summary>
		/// <returns> the list of actions for the try block. </returns>
		public IList<Action> TryActions => new List<Action>(tryActions);

	    /// <summary>
		/// Returns the list of actions executed in the catch block.
		/// </summary>
		/// <returns> the list of actions for the catch block. </returns>
		public IList<Action> CatchActions => new List<Action>(catchActions);

	    /// <summary>
		/// Returns the list of actions executed in the finally block.
		/// </summary>
		/// <returns> the list of actions for the finally block. </returns>
		public IList<Action> FinalActions => new List<Action>(finalActions);

	    /// <summary>
		/// {@inheritDoc} </summary>
		public ExceptionHandler copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, variable, register, tryActions, catchActions, finalActions);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = EMPTY_LENGTH; // assume thrown object is stored in register.

			if (register == 0)
			{
				length += context.strlen(variable) - 1;
			}

			tryLength = 0;
			catchLength = 0;
			finalLength = 0;

			foreach (Action action in tryActions)
			{
				tryLength += action.prepareToEncode(context);
			}

			foreach (Action action in catchActions)
			{
				catchLength += action.prepareToEncode(context);
			}

			foreach (Action action in finalActions)
			{
				finalLength += action.prepareToEncode(context);
			}

			length += tryLength;
			length += catchLength;
			length += finalLength;

			return Coder.ACTION_HEADER + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			coder.writeByte(ActionTypes.EXCEPTION_HANDLER);
			coder.writeShort(length);

			int flags = 0;

			if (register == 0)
			{
				flags |= VARIABLE_MASK;
			}
			if (finalLength > 0)
			{
				flags |= FINAL_MASK;
			}
			if (catchLength > 0)
			{
				flags |= CATCH_MASK;
			}
			coder.writeByte(flags);

			coder.writeShort(tryLength);
			coder.writeShort(catchLength);
			coder.writeShort(finalLength);

			if (register == 0)
			{
				coder.writeString(variable);
			}
			else
			{
				coder.writeByte(register);
			}

			foreach (Action action in tryActions)
			{
				action.encode(coder, context);
			}

			foreach (Action action in catchActions)
			{
				action.encode(coder, context);
			}

			foreach (Action action in finalActions)
			{
				action.encode(coder, context);
			}
		}
	}

}