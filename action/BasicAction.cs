using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;

/*
 * BasicAction.java
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

namespace com.flagstone.transform.action
{
    /// <summary>
	/// BasicAction represents all the actions that can be encoded using a single
	/// byte-code.
	/// 
	/// Where appropriate the description for an action contains a simple example
	/// showing the order of arguments on the stack and any result: e.g. 3, 2 -> 1.
	/// Here 3 and 2 are the numbers on the stack with 2 being the top-most. When the
	/// action is executed the numbers are popped off and the result to the right of
	/// the arrow is the result is pushed onto the stack.
	/// 
	/// <h1>Notes:</h1>
	/// <para>
	/// FSPush is used to push literals onto the Stack. See also FSRegisterCopy which
	/// copies the value on top of the Stack to one of the Flash Player's internal
	/// registers.
	/// </para>
	/// 
	/// <para>
	/// Arithmetic add is supported by two actions. INTEGER_ADD was introduced in
	/// Flash 4. It was replaced in Flash 5 by the more flexible ADD action which is
	/// able to add any two numbers and also concatenate strings. If a string and a
	/// number are added then the number is converted to its string representation
	/// before concatenation.
	/// </para>
	/// 
	/// <para>
	/// For comparison, Flash 4 introduced INTEGER_LESS and INTEGER_EQUALS for
	/// comparing numbers and STRING_LESS and STRING_EQUALS for comparing strings.
	/// They were superseded in Flash 5 by LESS and EQUALS which work with either
	/// strings or numbers.
	/// </para>
	/// </summary>
	public sealed class BasicAction : Action
	{
		/// <summary>
		/// Signals the end of a list of actions. </summary>
		public static readonly BasicAction END = new BasicAction("END", InnerEnum.END, ActionTypes.END);
		/// <summary>
		/// Move to the next frame. </summary>
		public static readonly BasicAction NEXT_FRAME = new BasicAction("NEXT_FRAME", InnerEnum.NEXT_FRAME, ActionTypes.NEXT_FRAME);
		/// <summary>
		/// Move to the previous frame. </summary>
		public static readonly BasicAction PREV_FRAME = new BasicAction("PREV_FRAME", InnerEnum.PREV_FRAME, ActionTypes.PREV_FRAME);
		/// <summary>
		/// Start playing the movie or movie clip. </summary>
		public static readonly BasicAction PLAY = new BasicAction("PLAY", InnerEnum.PLAY, ActionTypes.PLAY);
		/// <summary>
		/// Stop playing the movie or movie clip. </summary>
		public static readonly BasicAction STOP = new BasicAction("STOP", InnerEnum.STOP, ActionTypes.STOP);
		/// <summary>
		/// Toggle the movie between high and low quality. </summary>
		public static readonly BasicAction TOGGLE_QUALITY = new BasicAction("TOGGLE_QUALITY", InnerEnum.TOGGLE_QUALITY, ActionTypes.TOGGLE_QUALITY);
		/// <summary>
		/// Stop playing all sounds. </summary>
		public static readonly BasicAction STOP_SOUNDS = new BasicAction("STOP_SOUNDS", InnerEnum.STOP_SOUNDS, ActionTypes.STOP_SOUNDS);
		/// <summary>
		/// Add two integers. </summary>
		public static readonly BasicAction INTEGER_ADD = new BasicAction("INTEGER_ADD", InnerEnum.INTEGER_ADD, ActionTypes.INTEGER_ADD);
		/// <summary>
		/// Subtract two integers. </summary>
		public static readonly BasicAction SUBTRACT = new BasicAction("SUBTRACT", InnerEnum.SUBTRACT, ActionTypes.SUBTRACT);
		/// <summary>
		/// Multiply two numbers. </summary>
		public static readonly BasicAction MULTIPLY = new BasicAction("MULTIPLY", InnerEnum.MULTIPLY, ActionTypes.MULTIPLY);
		/// <summary>
		/// Divide two numbers. </summary>
		public static readonly BasicAction DIVIDE = new BasicAction("DIVIDE", InnerEnum.DIVIDE, ActionTypes.DIVIDE);
		/// <summary>
		/// Test whether two integers are equal. </summary>
		public static readonly BasicAction INTEGER_EQUALS = new BasicAction("INTEGER_EQUALS", InnerEnum.INTEGER_EQUALS, ActionTypes.INTEGER_EQUALS);
		/// <summary>
		/// Test where one number is less than another. </summary>
		public static readonly BasicAction INTEGER_LESS = new BasicAction("INTEGER_LESS", InnerEnum.INTEGER_LESS, ActionTypes.INTEGER_LESS);
		/// <summary>
		/// Logically and two values together. </summary>
		public static readonly BasicAction LOGICAL_AND = new BasicAction("LOGICAL_AND", InnerEnum.LOGICAL_AND, ActionTypes.LOGICAL_AND);
		/// <summary>
		/// Logically invert a value. </summary>
		public static readonly BasicAction LOGICAL_NOT = new BasicAction("LOGICAL_NOT", InnerEnum.LOGICAL_NOT, ActionTypes.LOGICAL_NOT);
		/// <summary>
		/// Logically or two values together. </summary>
		public static readonly BasicAction LOGICAL_OR = new BasicAction("LOGICAL_OR", InnerEnum.LOGICAL_OR, ActionTypes.LOGICAL_OR);
		/// <summary>
		/// Test whether two strings are equal. </summary>
		public static readonly BasicAction STRING_EQUALS = new BasicAction("STRING_EQUALS", InnerEnum.STRING_EQUALS, ActionTypes.STRING_EQUALS);
		/// <summary>
		/// Get the length of an ASCII string. </summary>
		public static readonly BasicAction STRING_LENGTH = new BasicAction("STRING_LENGTH", InnerEnum.STRING_LENGTH, ActionTypes.STRING_LENGTH);
		/// <summary>
		/// Substring. </summary>
		public static readonly BasicAction STRING_EXTRACT = new BasicAction("STRING_EXTRACT", InnerEnum.STRING_EXTRACT, ActionTypes.STRING_EXTRACT);
		/// <summary>
		/// Pop value from the top of the stack. </summary>
		public static readonly BasicAction POP = new BasicAction("POP", InnerEnum.POP, ActionTypes.POP);
		/// <summary>
		/// Convert a value to an integer. </summary>
		public static readonly BasicAction TO_INTEGER = new BasicAction("TO_INTEGER", InnerEnum.TO_INTEGER, ActionTypes.TO_INTEGER);
		/// <summary>
		/// Get the value of a variable. </summary>
		public static readonly BasicAction GET_VARIABLE = new BasicAction("GET_VARIABLE", InnerEnum.GET_VARIABLE, ActionTypes.GET_VARIABLE);
		/// <summary>
		/// Set the value of a variable. "x", 3 -> </summary>
		public static readonly BasicAction SET_VARIABLE = new BasicAction("SET_VARIABLE", InnerEnum.SET_VARIABLE, ActionTypes.SET_VARIABLE);
		/// <summary>
		/// Execute the following actions with the named movie clip. </summary>
		public static readonly BasicAction SET_TARGET_2 = new BasicAction("SET_TARGET_2", InnerEnum.SET_TARGET_2, ActionTypes.SET_TARGET_2);
		/// <summary>
		/// Concatenate two strings. </summary>
		public static readonly BasicAction STRING_ADD = new BasicAction("STRING_ADD", InnerEnum.STRING_ADD, ActionTypes.STRING_ADD);
		/// <summary>
		/// Push the value of the specified property on the stack. </summary>
		public static readonly BasicAction GET_PROPERTY = new BasicAction("GET_PROPERTY", InnerEnum.GET_PROPERTY, ActionTypes.GET_PROPERTY);
		/// <summary>
		/// Set the value of a property. </summary>
		public static readonly BasicAction SET_PROPERTY = new BasicAction("SET_PROPERTY", InnerEnum.SET_PROPERTY, ActionTypes.SET_PROPERTY);
		/// <summary>
		/// Duplicate a movie clip on the display list. </summary>
		public static readonly BasicAction CLONE_SPRITE = new BasicAction("CLONE_SPRITE", InnerEnum.CLONE_SPRITE, ActionTypes.CLONE_SPRITE);
		/// <summary>
		/// Delete a movie clip. </summary>
		public static readonly BasicAction REMOVE_SPRITE = new BasicAction("REMOVE_SPRITE", InnerEnum.REMOVE_SPRITE, ActionTypes.REMOVE_SPRITE);
		/// <summary>
		/// Append value to debugging window. </summary>
		public static readonly BasicAction TRACE = new BasicAction("TRACE", InnerEnum.TRACE, ActionTypes.TRACE);
		/// <summary>
		/// Start dragging the mouse. </summary>
		public static readonly BasicAction START_DRAG = new BasicAction("START_DRAG", InnerEnum.START_DRAG, ActionTypes.START_DRAG);
		/// <summary>
		/// Stop dragging the mouse. </summary>
		public static readonly BasicAction END_DRAG = new BasicAction("END_DRAG", InnerEnum.END_DRAG, ActionTypes.END_DRAG);
		/// <summary>
		/// Test where one string is less than another. </summary>
		public static readonly BasicAction STRING_LESS = new BasicAction("STRING_LESS", InnerEnum.STRING_LESS, ActionTypes.STRING_LESS);
		/// <summary>
		/// Throw an exception. </summary>
		public static readonly BasicAction THROW = new BasicAction("THROW", InnerEnum.THROW, ActionTypes.THROW);
		/// <summary>
		/// Casts the type of an object. </summary>
		public static readonly BasicAction CAST = new BasicAction("CAST", InnerEnum.CAST, ActionTypes.CAST);
		/// <summary>
		/// Identifies a class implements a defined interface. </summary>
		public static readonly BasicAction IMPLEMENTS = new BasicAction("IMPLEMENTS", InnerEnum.IMPLEMENTS, ActionTypes.IMPLEMENTS);
		/// <summary>
		/// FSCommand2 function. </summary>
		public static readonly BasicAction FS_COMMAND2 = new BasicAction("FS_COMMAND2", InnerEnum.FS_COMMAND2, ActionTypes.FS_COMMAND2);
		/// <summary>
		/// Push a random number onto the stack. </summary>
		public static readonly BasicAction RANDOM_NUMBER = new BasicAction("RANDOM_NUMBER", InnerEnum.RANDOM_NUMBER, ActionTypes.RANDOM_NUMBER);
		/// <summary>
		/// Get the length of an multi-byte string. </summary>
		public static readonly BasicAction MB_STRING_LENGTH = new BasicAction("MB_STRING_LENGTH", InnerEnum.MB_STRING_LENGTH, ActionTypes.MB_STRING_LENGTH);
		/// <summary>
		/// Convert the first character of a string to its ASCII value. </summary>
		public static readonly BasicAction CHAR_TO_ASCII = new BasicAction("CHAR_TO_ASCII", InnerEnum.CHAR_TO_ASCII, ActionTypes.CHAR_TO_ASCII);
		/// <summary>
		/// Convert the ASCII value to the equivalent character. </summary>
		public static readonly BasicAction ASCII_TO_CHAR = new BasicAction("ASCII_TO_CHAR", InnerEnum.ASCII_TO_CHAR, ActionTypes.ASCII_TO_CHAR);
		/// <summary>
		/// Return the elapsed time since the start of the movie. </summary>
		public static readonly BasicAction GET_TIME = new BasicAction("GET_TIME", InnerEnum.GET_TIME, ActionTypes.GET_TIME);
		/// <summary>
		/// Substring of a multi-byte string. </summary>
		public static readonly BasicAction MB_STRING_EXTRACT = new BasicAction("MB_STRING_EXTRACT", InnerEnum.MB_STRING_EXTRACT, ActionTypes.MB_STRING_EXTRACT);
		/// <summary>
		/// Convert the first character of string to its Unicode value. </summary>
		public static readonly BasicAction MB_CHAR_TO_ASCII = new BasicAction("MB_CHAR_TO_ASCII", InnerEnum.MB_CHAR_TO_ASCII, ActionTypes.MB_CHAR_TO_ASCII);
		/// <summary>
		/// Convert a Unicode value to the equivalent character. </summary>
		public static readonly BasicAction MB_ASCII_TO_CHAR = new BasicAction("MB_ASCII_TO_CHAR", InnerEnum.MB_ASCII_TO_CHAR, ActionTypes.MB_ASCII_TO_CHAR);
		/// <summary>
		/// Delete a variable. </summary>
		public static readonly BasicAction DELETE_VARIABLE = new BasicAction("DELETE_VARIABLE", InnerEnum.DELETE_VARIABLE, ActionTypes.DELETE_VARIABLE);
		/// <summary>
		/// Delete an object or variable. </summary>
		public static readonly BasicAction DELETE = new BasicAction("DELETE", InnerEnum.DELETE, ActionTypes.DELETE);
		/// <summary>
		/// Create and set a variable. </summary>
		public static readonly BasicAction INIT_VARIABLE = new BasicAction("INIT_VARIABLE", InnerEnum.INIT_VARIABLE, ActionTypes.INIT_VARIABLE);
		/// <summary>
		/// Execute a function. </summary>
		public static readonly BasicAction EXECUTE_FUNCTION = new BasicAction("EXECUTE_FUNCTION", InnerEnum.EXECUTE_FUNCTION, ActionTypes.EXECUTE_FUNCTION);
		/// <summary>
		/// Return control from a function. </summary>
		public static readonly BasicAction RETURN = new BasicAction("RETURN", InnerEnum.RETURN, ActionTypes.RETURN);
		/// <summary>
		/// Calculate the modulus of two numbers. </summary>
		public static readonly BasicAction MODULO = new BasicAction("MODULO", InnerEnum.MODULO, ActionTypes.MODULO);
		/// <summary>
		/// Construct an instance of a built-in object. </summary>
		public static readonly BasicAction NAMED_OBJECT = new BasicAction("NAMED_OBJECT", InnerEnum.NAMED_OBJECT, ActionTypes.NAMED_OBJECT);
		/// <summary>
		/// Create a new variable. </summary>
		public static readonly BasicAction NEW_VARIABLE = new BasicAction("NEW_VARIABLE", InnerEnum.NEW_VARIABLE, ActionTypes.NEW_VARIABLE);
		/// <summary>
		/// Create a new array. </summary>
		public static readonly BasicAction NEW_ARRAY = new BasicAction("NEW_ARRAY", InnerEnum.NEW_ARRAY, ActionTypes.NEW_ARRAY);
		/// <summary>
		/// Define a new class. </summary>
		public static readonly BasicAction NEW_OBJECT = new BasicAction("NEW_OBJECT", InnerEnum.NEW_OBJECT, ActionTypes.NEW_OBJECT);
		/// <summary>
		/// Return the type of an object or value. </summary>
		public static readonly BasicAction GET_TYPE = new BasicAction("GET_TYPE", InnerEnum.GET_TYPE, ActionTypes.GET_TYPE);
		/// <summary>
		/// Return the path to the current movie clip. </summary>
		public static readonly BasicAction GET_TARGET = new BasicAction("GET_TARGET", InnerEnum.GET_TARGET, ActionTypes.GET_TARGET);
		/// <summary>
		/// Enumerate through the attributes of an object. </summary>
		public static readonly BasicAction ENUMERATE = new BasicAction("ENUMERATE", InnerEnum.ENUMERATE, ActionTypes.ENUMERATE);
		/// <summary>
		/// Add two numbers. </summary>
		public static readonly BasicAction ADD = new BasicAction("ADD", InnerEnum.ADD, ActionTypes.ADD);
		/// <summary>
		/// Test where one value is less than another. </summary>
		public static readonly BasicAction LESS = new BasicAction("LESS", InnerEnum.LESS, ActionTypes.LESS);
		/// <summary>
		/// Test where one value is equal to another. </summary>
		public static readonly BasicAction EQUALS = new BasicAction("EQUALS", InnerEnum.EQUALS, ActionTypes.EQUALS);
		/// <summary>
		/// Converts the string value to a number. </summary>
		public static readonly BasicAction TO_NUMBER = new BasicAction("TO_NUMBER", InnerEnum.TO_NUMBER, ActionTypes.TO_NUMBER);
		/// <summary>
		/// Converts the value to a string. </summary>
		public static readonly BasicAction TO_STRING = new BasicAction("TO_STRING", InnerEnum.TO_STRING, ActionTypes.TO_STRING);
		/// <summary>
		/// Duplicate the value at the top of the stack. </summary>
		public static readonly BasicAction DUPLICATE = new BasicAction("DUPLICATE", InnerEnum.DUPLICATE, ActionTypes.DUPLICATE);
		/// <summary>
		/// Swap the top two values on the stack. </summary>
		public static readonly BasicAction SWAP = new BasicAction("SWAP", InnerEnum.SWAP, ActionTypes.SWAP);
		/// <summary>
		/// Get the value of an object's attribute. </summary>
		public static readonly BasicAction GET_ATTRIBUTE = new BasicAction("GET_ATTRIBUTE", InnerEnum.GET_ATTRIBUTE, ActionTypes.GET_ATTRIBUTE);
		/// <summary>
		/// Set the value of an object's attribute. </summary>
		public static readonly BasicAction SET_ATTRIBUTE = new BasicAction("SET_ATTRIBUTE", InnerEnum.SET_ATTRIBUTE, ActionTypes.SET_ATTRIBUTE);
		/// <summary>
		/// Increment a number. </summary>
		public static readonly BasicAction INCREMENT = new BasicAction("INCREMENT", InnerEnum.INCREMENT, ActionTypes.INCREMENT);
		/// <summary>
		/// Decrement a number. </summary>
		public static readonly BasicAction DECREMENT = new BasicAction("DECREMENT", InnerEnum.DECREMENT, ActionTypes.DECREMENT);
		/// <summary>
		/// Execute a method. </summary>
		public static readonly BasicAction EXECUTE_METHOD = new BasicAction("EXECUTE_METHOD", InnerEnum.EXECUTE_METHOD, ActionTypes.EXECUTE_METHOD);
		/// <summary>
		/// Define a new method for an object. </summary>
		public static readonly BasicAction NEW_METHOD = new BasicAction("NEW_METHOD", InnerEnum.NEW_METHOD, ActionTypes.NEW_METHOD);
		/// <summary>
		/// Tests whether an object can be created using the constructor. </summary>
		public static readonly BasicAction INSTANCEOF = new BasicAction("INSTANCEOF", InnerEnum.INSTANCEOF, ActionTypes.INSTANCEOF);
		/// <summary>
		/// Enumerate through the attributes of an object. </summary>
		public static readonly BasicAction ENUMERATE_OBJECT = new BasicAction("ENUMERATE_OBJECT", InnerEnum.ENUMERATE_OBJECT, ActionTypes.ENUMERATE_OBJECT);
		/// <summary>
		/// Bitwise and tow numbers. </summary>
		public static readonly BasicAction BITWISE_AND = new BasicAction("BITWISE_AND", InnerEnum.BITWISE_AND, ActionTypes.BITWISE_AND);
		/// <summary>
		/// Bitwise or tow numbers. </summary>
		public static readonly BasicAction BITWISE_OR = new BasicAction("BITWISE_OR", InnerEnum.BITWISE_OR, ActionTypes.BITWISE_OR);
		/// <summary>
		/// Bitwise exclusive-or two numbers. </summary>
		public static readonly BasicAction BITWISE_XOR = new BasicAction("BITWISE_XOR", InnerEnum.BITWISE_XOR, ActionTypes.BITWISE_XOR);
		/// <summary>
		/// Shift a number left. </summary>
		public static readonly BasicAction SHIFT_LEFT = new BasicAction("SHIFT_LEFT", InnerEnum.SHIFT_LEFT, ActionTypes.SHIFT_LEFT);
		/// <summary>
		/// Arithmetically shift a number right. </summary>
		public static readonly BasicAction ARITH_SHIFT_RIGHT = new BasicAction("ARITH_SHIFT_RIGHT", InnerEnum.ARITH_SHIFT_RIGHT, ActionTypes.ARITH_SHIFT_RIGHT);
		/// <summary>
		/// Shift a number right. -1, 30 -> 3 </summary>
		public static readonly BasicAction SHIFT_RIGHT = new BasicAction("SHIFT_RIGHT", InnerEnum.SHIFT_RIGHT, ActionTypes.SHIFT_RIGHT);
		/// <summary>
		/// Test whether type and value of two objects are equal. </summary>
		public static readonly BasicAction STRICT_EQUALS = new BasicAction("STRICT_EQUALS", InnerEnum.STRICT_EQUALS, ActionTypes.STRICT_EQUALS);
		/// <summary>
		/// Test whether a number is greater than another. </summary>
		public static readonly BasicAction GREATER = new BasicAction("GREATER", InnerEnum.GREATER, ActionTypes.GREATER);
		/// <summary>
		/// Test whether a string is greater than another. </summary>
		public static readonly BasicAction STRING_GREATER = new BasicAction("STRING_GREATER", InnerEnum.STRING_GREATER, ActionTypes.STRING_GREATER);
		/// <summary>
		/// Identifies that a class inherits from a class. </summary>
		public static readonly BasicAction EXTENDS = new BasicAction("EXTENDS", InnerEnum.EXTENDS, ActionTypes.EXTENDS);

		private static readonly IList<BasicAction> valueList = new List<BasicAction>();

		public enum InnerEnum
		{
			END,
			NEXT_FRAME,
			PREV_FRAME,
			PLAY,
			STOP,
			TOGGLE_QUALITY,
			STOP_SOUNDS,
			INTEGER_ADD,
			SUBTRACT,
			MULTIPLY,
			DIVIDE,
			INTEGER_EQUALS,
			INTEGER_LESS,
			LOGICAL_AND,
			LOGICAL_NOT,
			LOGICAL_OR,
			STRING_EQUALS,
			STRING_LENGTH,
			STRING_EXTRACT,
			POP,
			TO_INTEGER,
			GET_VARIABLE,
			SET_VARIABLE,
			SET_TARGET_2,
			STRING_ADD,
			GET_PROPERTY,
			SET_PROPERTY,
			CLONE_SPRITE,
			REMOVE_SPRITE,
			TRACE,
			START_DRAG,
			END_DRAG,
			STRING_LESS,
			THROW,
			CAST,
			IMPLEMENTS,
			FS_COMMAND2,
			RANDOM_NUMBER,
			MB_STRING_LENGTH,
			CHAR_TO_ASCII,
			ASCII_TO_CHAR,
			GET_TIME,
			MB_STRING_EXTRACT,
			MB_CHAR_TO_ASCII,
			MB_ASCII_TO_CHAR,
			DELETE_VARIABLE,
			DELETE,
			INIT_VARIABLE,
			EXECUTE_FUNCTION,
			RETURN,
			MODULO,
			NAMED_OBJECT,
			NEW_VARIABLE,
			NEW_ARRAY,
			NEW_OBJECT,
			GET_TYPE,
			GET_TARGET,
			ENUMERATE,
			ADD,
			LESS,
			EQUALS,
			TO_NUMBER,
			TO_STRING,
			DUPLICATE,
			SWAP,
			GET_ATTRIBUTE,
			SET_ATTRIBUTE,
			INCREMENT,
			DECREMENT,
			EXECUTE_METHOD,
			NEW_METHOD,
			INSTANCEOF,
			ENUMERATE_OBJECT,
			BITWISE_AND,
			BITWISE_OR,
			BITWISE_XOR,
			SHIFT_LEFT,
			ARITH_SHIFT_RIGHT,
			SHIFT_RIGHT,
			STRICT_EQUALS,
			GREATER,
			STRING_GREATER,
			EXTENDS
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal;

		/// <summary>
		/// Table used to store instances of Basic Actions so only one object is
		/// created for each type of action decoded.
		/// </summary>
		private static readonly IDictionary<int?, BasicAction> TABLE = new Dictionary<int?, BasicAction>();

		static BasicAction()
		{


			valueList.Add(END);
			valueList.Add(NEXT_FRAME);
			valueList.Add(PREV_FRAME);
			valueList.Add(PLAY);
			valueList.Add(STOP);
			valueList.Add(TOGGLE_QUALITY);
			valueList.Add(STOP_SOUNDS);
			valueList.Add(INTEGER_ADD);
			valueList.Add(SUBTRACT);
			valueList.Add(MULTIPLY);
			valueList.Add(DIVIDE);
			valueList.Add(INTEGER_EQUALS);
			valueList.Add(INTEGER_LESS);
			valueList.Add(LOGICAL_AND);
			valueList.Add(LOGICAL_NOT);
			valueList.Add(LOGICAL_OR);
			valueList.Add(STRING_EQUALS);
			valueList.Add(STRING_LENGTH);
			valueList.Add(STRING_EXTRACT);
			valueList.Add(POP);
			valueList.Add(TO_INTEGER);
			valueList.Add(GET_VARIABLE);
			valueList.Add(SET_VARIABLE);
			valueList.Add(SET_TARGET_2);
			valueList.Add(STRING_ADD);
			valueList.Add(GET_PROPERTY);
			valueList.Add(SET_PROPERTY);
			valueList.Add(CLONE_SPRITE);
			valueList.Add(REMOVE_SPRITE);
			valueList.Add(TRACE);
			valueList.Add(START_DRAG);
			valueList.Add(END_DRAG);
			valueList.Add(STRING_LESS);
			valueList.Add(THROW);
			valueList.Add(CAST);
			valueList.Add(IMPLEMENTS);
			valueList.Add(FS_COMMAND2);
			valueList.Add(RANDOM_NUMBER);
			valueList.Add(MB_STRING_LENGTH);
			valueList.Add(CHAR_TO_ASCII);
			valueList.Add(ASCII_TO_CHAR);
			valueList.Add(GET_TIME);
			valueList.Add(MB_STRING_EXTRACT);
			valueList.Add(MB_CHAR_TO_ASCII);
			valueList.Add(MB_ASCII_TO_CHAR);
			valueList.Add(DELETE_VARIABLE);
			valueList.Add(DELETE);
			valueList.Add(INIT_VARIABLE);
			valueList.Add(EXECUTE_FUNCTION);
			valueList.Add(RETURN);
			valueList.Add(MODULO);
			valueList.Add(NAMED_OBJECT);
			valueList.Add(NEW_VARIABLE);
			valueList.Add(NEW_ARRAY);
			valueList.Add(NEW_OBJECT);
			valueList.Add(GET_TYPE);
			valueList.Add(GET_TARGET);
			valueList.Add(ENUMERATE);
			valueList.Add(ADD);
			valueList.Add(LESS);
			valueList.Add(EQUALS);
			valueList.Add(TO_NUMBER);
			valueList.Add(TO_STRING);
			valueList.Add(DUPLICATE);
			valueList.Add(SWAP);
			valueList.Add(GET_ATTRIBUTE);
			valueList.Add(SET_ATTRIBUTE);
			valueList.Add(INCREMENT);
			valueList.Add(DECREMENT);
			valueList.Add(EXECUTE_METHOD);
			valueList.Add(NEW_METHOD);
			valueList.Add(INSTANCEOF);
			valueList.Add(ENUMERATE_OBJECT);
			valueList.Add(BITWISE_AND);
			valueList.Add(BITWISE_OR);
			valueList.Add(BITWISE_XOR);
			valueList.Add(SHIFT_LEFT);
			valueList.Add(ARITH_SHIFT_RIGHT);
			valueList.Add(SHIFT_RIGHT);
			valueList.Add(STRICT_EQUALS);
			valueList.Add(GREATER);
			valueList.Add(STRING_GREATER);
			valueList.Add(EXTENDS);

		    foreach (BasicAction action in values())
		    {
		        TABLE[action.type] = action;
		    }
        }

		/// <summary>
		/// Returns the BasicAction for a given type.
		/// </summary>
		/// <param name="actionType">
		///            the type that identifies the action when it is encoded.
		/// </param>
		/// <returns> a shared instance of the object representing a given action type. </returns>


		public static BasicAction fromInt(int actionType)
		{
			return TABLE[actionType];
		}

		/// <summary>
		/// Type used to identify the action when it is encoded. </summary>
		private readonly int type;

		/// <summary>
		/// Constructor used to create instances for each type of action.
		/// </summary>
		/// <param name="actionType"> the value representing the action when it is encoded. </param>


		private BasicAction(string name, InnerEnum innerEnum, int actionType)
		{
			type = actionType;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public BasicAction copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			return 1;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(type);

		}

		public static IList<BasicAction> values()
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

		public static BasicAction valueOf(string name)
		{
			foreach (BasicAction enumInstance in valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new ArgumentException(name);
		}
	}

}