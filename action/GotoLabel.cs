using System;
using com.flagstone.transform.coder;

/*
 * GotoLabel.java
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
	/// The GotoLabel action instructs the player to move to the frame in the current
	/// movie with the specified label - previously assigned using a FrameLabel
	/// object.
	/// </summary>
	public sealed class GotoLabel : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "GotoLabel: { label=%s}";

		/// <summary>
		/// The frame label. </summary>
		
		private readonly string label;

		/// <summary>
		/// The length of the action, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a GotoLabel action using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public GotoLabel(SWFDecoder coder)
		{
			length = coder.readUnsignedShort();
			label = coder.readString();
		}

		/// <summary>
		/// Creates a GotoLabel action with the specified frame label.
		/// </summary>
		/// <param name="aString">
		///            the label assigned a particular frame in the movie. Must not
		///            be null or an empty string. </param>


		public GotoLabel(string aString)
		{
			if (ReferenceEquals(aString, null) || aString.Length == 0)
			{
				throw new ArgumentException();
			}
			label = aString;
		}

		/// <summary>
		/// Creates and initialises a GotoLabel action using the values
		/// copied from another GotoLabel action.
		/// </summary>
		/// <param name="object">
		///            a GotoLabel action from which the values will be
		///            copied. </param>


		public GotoLabel(GotoLabel @object)
		{
			label = @object.label;
		}

		/// <summary>
		/// Get the frame label.
		/// </summary>
		/// <returns> the label assigned a particular frame in the movie. </returns>
		public string Label => label;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public GotoLabel copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, label);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = context.strlen(label);

			return Coder.ACTION_HEADER + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ActionTypes.GOTO_LABEL);
			coder.writeShort(length);
			coder.writeString(label);
		}
	}

}