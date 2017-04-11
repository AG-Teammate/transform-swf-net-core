using System;
using System.Collections.Generic;
using com.flagstone.transform.action;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;
using Action = com.flagstone.transform.action.Action;

/*
 * DefineButton.java
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

namespace com.flagstone.transform.button
{
    /// <summary>
	/// DefineButton defines the appearance of a button and the actions performed
	/// when the button is clicked.
	/// 
	/// <para>
	/// DefineButton must contain at least one ButtonShape object. If more than one
	/// button shape is defined for a given button state then each shape will be
	/// displayed by the button. The order in which the shapes are displayed is
	/// determined by the layer assigned to each ButtonShape object.
	/// </P>
	/// 
	/// </para>
	/// </summary>
	/// <seealso cref= ButtonShape </seealso>
	public sealed class DefineButton : DefineTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineButton: { identifier=%d;" + " buttonRecords=%s; actions=%s}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The list of shapes used to draw the button. </summary>
		private IList<ButtonShape> shapes;
		/// <summary>
		/// The actions executed when the button is clicked. </summary>
		private IList<Action> actions;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a DefineButton object using values encoded
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




		public DefineButton(SWFDecoder coder, Context context)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			shapes = new List<ButtonShape>();

			while (coder.scanByte() != 0)
			{
				shapes.Add(new ButtonShape(coder, context));
			}

			coder.readByte();

			actions = new List<Action>();



			SWFFactory<Action> decoder = context.Registry.ActionDecoder;

			if (decoder == null)
			{
				actions.Add(new ActionData(coder.readBytes(new byte[length - coder.bytesRead()])));
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
		/// Creates a DefineButton object with the identifier, button shapes and
		/// actions.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for this button. </param>
		/// <param name="buttons">
		///            a list of ButtonShapes that are used to draw the button. </param>
		/// <param name="script">
		///            a list of actions that are executed when the button is
		///            clicked. </param>


		public DefineButton(int uid, IList<ButtonShape> buttons, IList<Action> script)
		{
			Identifier = uid;
			Shapes = buttons;
			Actions = script;
		}

		/// <summary>
		/// Creates and initialises a DefineButton object using the values copied
		/// from another DefineButton object.
		/// </summary>
		/// <param name="object">
		///            a DefineButton object from which the values will be
		///            copied. </param>


		public DefineButton(DefineButton @object)
		{
			identifier = @object.identifier;
			shapes = new List<ButtonShape>(@object.shapes.Count);
			foreach (ButtonShape shape in @object.shapes)
			{
				shapes.Add(shape.copy());
			}
			actions = new List<Action>(@object.actions);
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
		/// Adds the button shape to the list of button shapes.
		/// </summary>
		/// <param name="obj">
		///            an ButtonShape object. Must not be null. </param>
		/// <returns> this object. </returns>


		public DefineButton add(ButtonShape obj)
		{
			if (obj == null)
			{
				throw new ArgumentException();
			}
			shapes.Add(obj);
			return this;
		}

		/// <summary>
		/// Adds the action to the list of actions.
		/// </summary>
		/// <param name="obj">
		///            an action object. Must not be null. </param>
		/// <returns> this object. </returns>


		public DefineButton add(Action obj)
		{
			if (obj == null)
			{
				throw new ArgumentException();
			}
			actions.Add(obj);
			return this;
		}

		/// <summary>
		/// Get the list of button shapes.
		/// </summary>
		/// <returns> the list of shapes used to represent the button. </returns>
		public IList<ButtonShape> Shapes
		{
			get => shapes;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				shapes = value;
			}
		}

		/// <summary>
		/// Get the list of actions that will be executed when the button is
		/// clicked and released.
		/// </summary>
		/// <returns> the actions executed when the button is clicked. </returns>
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
		public DefineButton copy()
		{
			return new DefineButton(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, shapes, actions);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 2;

			foreach (ButtonShape shape in shapes)
			{
				length += shape.prepareToEncode(context);
			}

			length += 1;

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
				coder.writeShort((MovieTypes.DEFINE_BUTTON << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_BUTTON << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);

			foreach (ButtonShape shape in shapes)
			{
				shape.encode(coder, context);
			}

			coder.writeByte(0);

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