/*
 * ActionTypes.java
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
	/// ActionTypes defines the constants that identify the different types of action
	/// when encoded according to the Flash file format specification.
	/// <para>
	/// IMPORTANT: Only Actions for Actionscript 1.0 and 2.0 are included, actions
	/// defined for Actionscript 3.0 are not included.
	/// </para>
	/// </summary>
	public sealed class ActionTypes
	{
		/// <summary>
		/// The type for creating the end of a sequence of actions. </summary>
		public const int END = 0;
		/// <summary>
		/// The type used to identify NextFrame action when encoded. </summary>
		public const int NEXT_FRAME = 4;
		/// <summary>
		/// The type used to identify PrevFrame action when encoded. </summary>
		public const int PREV_FRAME = 5;
		/// <summary>
		/// The type used to identify Play action when encoded. </summary>
		public const int PLAY = 6;
		/// <summary>
		/// The type used to identify Stop action when encoded. </summary>
		public const int STOP = 7;
		/// <summary>
		/// The type used to identify ToggleQuality action when encoded. </summary>
		public const int TOGGLE_QUALITY = 8;
		/// <summary>
		/// The type used to identify StopSounds action when encoded. </summary>
		public const int STOP_SOUNDS = 9;
		/// <summary>
		/// The type used to identify IntegerAdd action when encoded. </summary>
		public const int INTEGER_ADD = 10;
		/// <summary>
		/// The type used to identify Subtract action when encoded. </summary>
		public const int SUBTRACT = 11;
		/// <summary>
		/// The type used to identify Multiply action when encoded. </summary>
		public const int MULTIPLY = 12;
		/// <summary>
		/// The type used to identify Divide action when encoded. </summary>
		public const int DIVIDE = 13;
		/// <summary>
		/// The type used to identify IntegerEquals action when encoded. </summary>
		public const int INTEGER_EQUALS = 14;
		/// <summary>
		/// The type used to identify IntegerLess action when encoded. </summary>
		public const int INTEGER_LESS = 15;
		/// <summary>
		/// The type used to identify And action when encoded. </summary>
		public const int LOGICAL_AND = 16;
		/// <summary>
		/// The type used to identify Not action when encoded. </summary>
		public const int LOGICAL_NOT = 18;
		/// <summary>
		/// The type used to identify Or action when encoded. </summary>
		public const int LOGICAL_OR = 17;
		/// <summary>
		/// The type used to identify StringEquals action when encoded. </summary>
		public const int STRING_EQUALS = 19;
		/// <summary>
		/// The type used to identify StringLength action when encoded. </summary>
		public const int STRING_LENGTH = 20;
		/// <summary>
		/// The type used to identify StringExtract action when encoded. </summary>
		public const int STRING_EXTRACT = 21;
		/// <summary>
		/// The type used to identify Pop action when encoded. </summary>
		public const int POP = 23;
		/// <summary>
		/// The type used to identify ToInteger action when encoded. </summary>
		public const int TO_INTEGER = 24;
		/// <summary>
		/// The type used to identify GetVariable action when encoded. </summary>
		public const int GET_VARIABLE = 28;
		/// <summary>
		/// The type used to identify SetVariable action when encoded. </summary>
		public const int SET_VARIABLE = 29;
		/// <summary>
		/// The type used to identify SetTarget2 action when encoded. </summary>
		public const int SET_TARGET_2 = 32;
		/// <summary>
		/// The type used to identify StringAdd action when encoded. </summary>
		public const int STRING_ADD = 33;
		/// <summary>
		/// The type used to identify GetProperty action when encoded. </summary>
		public const int GET_PROPERTY = 34;
		/// <summary>
		/// The type used to identify SetProperty action when encoded. </summary>
		public const int SET_PROPERTY = 35;
		/// <summary>
		/// The type used to identify CloneSprite action when encoded. </summary>
		public const int CLONE_SPRITE = 36;
		/// <summary>
		/// The type used to identify RemoveSprite action when encoded. </summary>
		public const int REMOVE_SPRITE = 37;
		/// <summary>
		/// The type used to identify Trace action when encoded. </summary>
		public const int TRACE = 38;
		/// <summary>
		/// The type used to identify StartDrag action when encoded. </summary>
		public const int START_DRAG = 39;
		/// <summary>
		/// The type used to identify EndDrag action when encoded. </summary>
		public const int END_DRAG = 40;
		/// <summary>
		/// The type used to identify StringLess action when encoded. </summary>
		public const int STRING_LESS = 41;
		/// <summary>
		/// The type used to identify Throw action when encoded. </summary>
		public const int THROW = 42;
		/// <summary>
		/// The type used to identify Cast action when encoded. </summary>
		public const int CAST = 43;
		/// <summary>
		/// The type used to identify Implements action when encoded. </summary>
		public const int IMPLEMENTS = 44;
		/// <summary>
		/// The type used to identify FSCommand2 action when encoded. </summary>
		public const int FS_COMMAND2 = 45;
		/// <summary>
		/// The type used to identify RandomNumber action when encoded. </summary>
		public const int RANDOM_NUMBER = 48;
		/// <summary>
		/// The type used to identify MBStringLength action when encoded. </summary>
		public const int MB_STRING_LENGTH = 49;
		/// <summary>
		/// The type used to identify CharToAscii action when encoded. </summary>
		public const int CHAR_TO_ASCII = 50;
		/// <summary>
		/// The type used to identify AsciiToChar action when encoded. </summary>
		public const int ASCII_TO_CHAR = 51;
		/// <summary>
		/// The type used to identify GetTime action when encoded. </summary>
		public const int GET_TIME = 52;
		/// <summary>
		/// The type used to identify MBStringExtract action when encoded. </summary>
		public const int MB_STRING_EXTRACT = 53;
		/// <summary>
		/// The type used to identify MBCharToAscii action when encoded. </summary>
		public const int MB_CHAR_TO_ASCII = 54;
		/// <summary>
		/// The type used to identify MBAsciiToChar action when encoded. </summary>
		public const int MB_ASCII_TO_CHAR = 55;
		/// <summary>
		/// The type used to identify DeleteVariable action when encoded. </summary>
		public const int DELETE_VARIABLE = 58;
		/// <summary>
		/// The type used to identify Delete action when encoded. </summary>
		public const int DELETE = 59;
		/// <summary>
		/// The type used to identify InitVariable action when encoded. </summary>
		public const int INIT_VARIABLE = 60;
		/// <summary>
		/// The type used to identify ExecuteFunction action when encoded. </summary>
		public const int EXECUTE_FUNCTION = 61;
		/// <summary>
		/// The type used to identify Return action when encoded. </summary>
		public const int RETURN = 62;
		/// <summary>
		/// The type used to identify Modulo action when encoded. </summary>
		public const int MODULO = 63;
		/// <summary>
		/// The type used to identify NamedObject action when encoded. </summary>
		public const int NAMED_OBJECT = 64;
		/// <summary>
		/// The type used to identify NewVariable action when encoded. </summary>
		public const int NEW_VARIABLE = 65;
		/// <summary>
		/// The type used to identify NewArray action when encoded. </summary>
		public const int NEW_ARRAY = 66;
		/// <summary>
		/// The type used to identify NewObject action when encoded. </summary>
		public const int NEW_OBJECT = 67;
		/// <summary>
		/// The type used to identify GetType action when encoded. </summary>
		public const int GET_TYPE = 68;
		/// <summary>
		/// The type used to identify GetTarget action when encoded. </summary>
		public const int GET_TARGET = 69;
		/// <summary>
		/// The type used to identify Enumerate action when encoded. </summary>
		public const int ENUMERATE = 70;
		/// <summary>
		/// The type used to identify Add action when encoded. </summary>
		public const int ADD = 71;
		/// <summary>
		/// The type used to identify Less action when encoded. </summary>
		public const int LESS = 72;
		/// <summary>
		/// The type used to identify Equals action when encoded. </summary>
		public const int EQUALS = 73;
		/// <summary>
		/// The type used to identify ToNumber action when encoded. </summary>
		public const int TO_NUMBER = 74;
		/// <summary>
		/// The type used to identify ToString action when encoded. </summary>
		public const int TO_STRING = 75;
		/// <summary>
		/// The type used to identify Duplicate action when encoded. </summary>
		public const int DUPLICATE = 76;
		/// <summary>
		/// The type used to identify Swap action when encoded. </summary>
		public const int SWAP = 77;
		/// <summary>
		/// The type used to identify GetAttribute action when encoded. </summary>
		public const int GET_ATTRIBUTE = 78;
		/// <summary>
		/// The type used to identify SetAttribute action when encoded. </summary>
		public const int SET_ATTRIBUTE = 79;
		/// <summary>
		/// The type used to identify Increment action when encoded. </summary>
		public const int INCREMENT = 80;
		/// <summary>
		/// The type used to identify Decrement action when encoded. </summary>
		public const int DECREMENT = 81;
		/// <summary>
		/// The type used to identify ExecuteMethod action when encoded. </summary>
		public const int EXECUTE_METHOD = 82;
		/// <summary>
		/// The type used to identify NewMethod action when encoded. </summary>
		public const int NEW_METHOD = 83;
		/// <summary>
		/// The type used to identify InstanceOf action when encoded. </summary>
		public const int INSTANCEOF = 84;
		/// <summary>
		/// The type used to identify EnumerateObject action when encoded. </summary>
		public const int ENUMERATE_OBJECT = 85;
		/// <summary>
		/// The type used to identify BitwiseAnd action when encoded. </summary>
		public const int BITWISE_AND = 96;
		/// <summary>
		/// The type used to identify BitwiseOr action when encoded. </summary>
		public const int BITWISE_OR = 97;
		/// <summary>
		/// The type used to identify BitwiseXOr action when encoded. </summary>
		public const int BITWISE_XOR = 98;
		/// <summary>
		/// The type used to identify LogicalShiftLeft action when encoded. </summary>
		public const int SHIFT_LEFT = 99;
		/// <summary>
		/// The type used to identify ArithmeticShiftRight action when encoded. </summary>
		public const int ARITH_SHIFT_RIGHT = 100;
		/// <summary>
		/// The type used to identify LogicalShiftRight action when encoded. </summary>
		public const int SHIFT_RIGHT = 101;
		/// <summary>
		/// The type used to identify StrictEquals action when encoded. </summary>
		public const int STRICT_EQUALS = 102;
		/// <summary>
		/// The type used to identify Greater action when encoded. </summary>
		public const int GREATER = 103;
		/// <summary>
		/// The type used to identify StringGreater action when encoded. </summary>
		public const int STRING_GREATER = 104;
		/// <summary>
		/// The type used to identify Extends action when encoded. </summary>
		public const int EXTENDS = 105;
		/// <summary>
		/// The type used to identify GotoFrame action when encoded. </summary>
		public const int GOTO_FRAME = 129;
		/// <summary>
		/// The type used to identify GetUrl action when encoded. </summary>
		public const int GET_URL = 131;
		/// <summary>
		/// The type used to identify RegisterCopy action when encoded. </summary>
		public const int REGISTER_COPY = 135;
		/// <summary>
		/// The type used to identify Table action when encoded. </summary>
		public const int TABLE = 136;
		/// <summary>
		/// The type used to identify WaitForFrame action when encoded. </summary>
		public const int WAIT_FOR_FRAME = 138;
		/// <summary>
		/// The type used to identify SetTarget action when encoded. </summary>
		public const int SET_TARGET = 139;
		/// <summary>
		/// The type used to identify GotoLabel action when encoded. </summary>
		public const int GOTO_LABEL = 140;
		/// <summary>
		/// The type used to identify WaitForFrame2 action when encoded. </summary>
		public const int WAIT_FOR_FRAME_2 = 141;
		/// <summary>
		/// The type used to identify NewFunction2 action when encoded. </summary>
		public const int NEW_FUNCTION_2 = 142;
		/// <summary>
		/// The type used to identify ExceptionHandler action when encoded. </summary>
		public const int EXCEPTION_HANDLER = 143;
		/// <summary>
		/// The type used to identify With action when encoded. </summary>
		public const int WITH = 148;
		/// <summary>
		/// The type used to identify Push action when encoded. </summary>
		public const int PUSH = 150;
		/// <summary>
		/// The type used to identify Jump action when encoded. </summary>
		public const int JUMP = 153;
		/// <summary>
		/// The type used to identify GetUrl2 action when encoded. </summary>
		public const int GET_URL_2 = 154;
		/// <summary>
		/// The type for creating an If action. </summary>
		public const int IF = 157; //NOPMD
		/// <summary>
		/// The type used to identify Call action when encoded. </summary>
		public const int CALL = 158;
		/// <summary>
		/// The type used to identify GotoFrame2 action when encoded. </summary>
		public const int GOTO_FRAME_2 = 159;
		/// <summary>
		/// The type used to identify NewFunction action when encoded. </summary>
		public const int NEW_FUNCTION = 155;

		/// <summary>
		/// The highest value used to encode an action that only operates on values
		/// on the Flash Player's stack.
		/// </summary>
		public const int HIGHEST_BYTE_CODE = 127;

		/// <summary>
		/// Private constructor for a class that contains only constants. </summary>
		private ActionTypes()
		{
			// Class only contains constants
		}
	}

}