using System;
using System.Collections.Generic;

/*
 * ButtonKey.java
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

namespace com.flagstone.transform.button
{

	/// <summary>
	/// ButtonKey is used to provide mapping from special keys to codes recognised
	/// by the Flash Player.
	/// </summary>
	public sealed class ButtonKey
	{
		/// <summary>
		/// Code for the button event that occurs when the left arrow key is pressed
		/// on the keyboard.
		/// </summary>
		public static readonly ButtonKey LEFT = new ButtonKey("LEFT", InnerEnum.LEFT, 1);
		/// <summary>
		/// Code for the button event that occurs when the right arrow key is pressed
		/// on the keyboard.
		/// </summary>
		public static readonly ButtonKey RIGHT = new ButtonKey("RIGHT", InnerEnum.RIGHT, 2);
		/// <summary>
		/// Code for the button event that occurs when the home key is pressed on the
		/// keyboard.
		/// </summary>
		public static readonly ButtonKey HOME = new ButtonKey("HOME", InnerEnum.HOME, 3);
		/// <summary>
		/// Code for the button event that occurs when the end key is pressed on the
		/// keyboard.
		/// </summary>
		public static readonly ButtonKey END = new ButtonKey("END", InnerEnum.END, 4);
		/// <summary>
		/// Code for the button event that occurs when the insert key is pressed on
		/// the keyboard.
		/// </summary>
		public static readonly ButtonKey INSERT = new ButtonKey("INSERT", InnerEnum.INSERT, 5);
		/// <summary>
		/// Code for the button event that occurs when the delete key is pressed on
		/// the keyboard.
		/// </summary>
		public static readonly ButtonKey DELETE = new ButtonKey("DELETE", InnerEnum.DELETE, 6);
		/// <summary>
		/// Code for the button event that occurs when the backspace key is pressed
		/// on the keyboard.
		/// </summary>
		public static readonly ButtonKey BACKSPACE = new ButtonKey("BACKSPACE", InnerEnum.BACKSPACE, 8);
		/// <summary>
		/// Code for the button event that occurs when the enter key is pressed on
		/// the keyboard.
		/// </summary>
		public static readonly ButtonKey ENTER = new ButtonKey("ENTER", InnerEnum.ENTER, 13);
		/// <summary>
		/// Code for the button event that occurs when the up arrow key is pressed on
		/// the keyboard.
		/// </summary>
		public static readonly ButtonKey UP = new ButtonKey("UP", InnerEnum.UP, 14);
		/// <summary>
		/// Code for the button event that occurs when the down arrow key is pressed
		/// on the keyboard.
		/// </summary>
		public static readonly ButtonKey DOWN = new ButtonKey("DOWN", InnerEnum.DOWN, 15);
		/// <summary>
		/// Code for the button event that occurs when the page up key is pressed on
		/// the keyboard.
		/// </summary>
		public static readonly ButtonKey PAGE_UP = new ButtonKey("PAGE_UP", InnerEnum.PAGE_UP, 16);
		/// <summary>
		/// Code for the button event that occurs when the page down key is pressed
		/// on the keyboard.
		/// </summary>
		public static readonly ButtonKey PAGE_DOWN = new ButtonKey("PAGE_DOWN", InnerEnum.PAGE_DOWN, 17);
		/// <summary>
		/// Code for the button event that occurs when the tab key is pressed on the
		/// keyboard.
		/// </summary>
		public static readonly ButtonKey TAB = new ButtonKey("TAB", InnerEnum.TAB, 18);
		/// <summary>
		/// Code for the button event that occurs when the escape key is pressed on
		/// the keyboard.
		/// </summary>
		public static readonly ButtonKey ESCAPE = new ButtonKey("ESCAPE", InnerEnum.ESCAPE, 19);

		private static readonly IList<ButtonKey> valueList = new List<ButtonKey>();

		public enum InnerEnum
		{
			LEFT,
			RIGHT,
			HOME,
			END,
			INSERT,
			DELETE,
			BACKSPACE,
			ENTER,
			UP,
			DOWN,
			PAGE_UP,
			PAGE_DOWN,
			TAB,
			ESCAPE
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal;

		/// <summary>
		/// Table mapping code to keys. </summary>
		private static readonly IDictionary<int?, ButtonKey> TABLE = new Dictionary<int?, ButtonKey>();

		static ButtonKey()
		{
			foreach (ButtonKey type in values())
			{
				TABLE[type.value] = type;
			}

			valueList.Add(LEFT);
			valueList.Add(RIGHT);
			valueList.Add(HOME);
			valueList.Add(END);
			valueList.Add(INSERT);
			valueList.Add(DELETE);
			valueList.Add(BACKSPACE);
			valueList.Add(ENTER);
			valueList.Add(UP);
			valueList.Add(DOWN);
			valueList.Add(PAGE_UP);
			valueList.Add(PAGE_DOWN);
			valueList.Add(TAB);
			valueList.Add(ESCAPE);
		}

		/// <summary>
		/// Get the ButtonKey for an encoded value. </summary>
		/// <param name="keyCode"> the encoded value representing a key. </param>
		/// <returns> the ButtonKey for the encoded value. </returns>


		public static ButtonKey fromInt(int keyCode)
		{
			return TABLE[keyCode];
		}

		/// <summary>
		/// The value representing the ButtonKey when it is encoded. </summary>
		private readonly int value;

		/// <summary>
		/// Create a new ButtonKey. </summary>
		/// <param name="keyCode"> the value that is encoded to represent the key. </param>


		private ButtonKey(string name, InnerEnum innerEnum, int keyCode)
		{
			value = keyCode;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Get the value that will be encoded to represent the ButtonKey. </summary>
		/// <returns> the value that will be encoded. </returns>
		public int Value => value;

	    public static IList<ButtonKey> values()
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

		public static ButtonKey valueOf(string name)
		{
			foreach (ButtonKey enumInstance in valueList)
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