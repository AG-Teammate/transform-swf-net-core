using System;
using com.flagstone.transform.coder;

/*
 * SetTarget.java
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
	/// SetTarget selects a movie clip to allow its time-line to be controlled. The
	/// action performs a "context switch". All following actions such as GotoFrame,
	/// Play, etc. will be applied to the specified object until another
	/// <b>SetTarget</b> action is executed. Setting the target to be the empty
	/// string ("") returns the target to the movie's main time-line.
	/// 
	/// </summary>
	public sealed class SetTarget : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "SetTarget: { target=%s}";

		/// <summary>
		/// The name of the movie clip. </summary>
		
		private readonly string target;

		/// <summary>
		/// The length of the action, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a SetTarget action using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public SetTarget(SWFDecoder coder)
		{
			length = coder.readUnsignedShort();
			target = coder.readString(length);
		}

		/// <summary>
		/// Creates a SetTarget action that changes the context to the specified
		/// target.
		/// </summary>
		/// <param name="aString">
		///            the name of a movie clip. Must not be null. </param>


		public SetTarget(string aString)
		{
			if (ReferenceEquals(aString, null))
			{
				throw new ArgumentException();
			}
			target = aString;
		}

		/// <summary>
		/// Creates and initialises a SetTarget action using the values
		/// copied from another SetTarget action.
		/// </summary>
		/// <param name="object">
		///            a SetTarget action from which the values will be
		///            copied. </param>


		public SetTarget(SetTarget @object)
		{
			target = @object.target;
		}

		/// <summary>
		/// Get the name of the target movie clip.
		/// </summary>
		/// <returns> the name of the movie clip. </returns>
		public string Target => target;

	    /// <summary>
		/// {@inheritDoc} </summary>
		public SetTarget copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, target);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = context.strlen(target);

			return Coder.ACTION_HEADER + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ActionTypes.SET_TARGET);
			coder.writeShort(length);
			coder.writeString(target);
		}
	}

}