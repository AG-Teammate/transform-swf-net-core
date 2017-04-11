using System;
using com.flagstone.transform.coder;

/*
 * ActionData.java
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
	/// ActionData is used to store one or more actions which already have been
	/// encoded for writing to a Flash file.
	/// 
	/// <para>
	/// You can use this class to reduce the time it takes to decode and encode a
	/// movie. By selectively decoding the actions in a movie,  actions that are
	/// not of interest can be left encoded. Similarly selectively encoding actions
	/// that will not change will improve performance when generating files using a
	/// movie as a template.
	/// </para>
	/// </summary>
	public sealed class ActionData : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "ActionData: { data=byte<%d> ...}";

		/// <summary>
		/// Encoded actions. </summary>
		
		private readonly byte[] data;

		/// <summary>
		/// Creates an ActionData object initialised with a set of encoded actions.
		/// </summary>
		/// <param name="bytes">
		///            the encoded actions. Must not be null or empty. </param>


		public ActionData(byte[] bytes)
		{
			data = Arrays.copyOf(bytes, bytes.Length);
		}

		/// <summary>
		/// Creates a copy of this ActionData object.
		/// </summary>
		/// <param name="object">
		///            the ActionData object used to initialise this one. </param>


		public ActionData(ActionData @object)
		{
			data = @object.data;
		}

		/// <summary>
		/// Creates and returns a copy of the encoded actions.
		/// </summary>
		/// <returns> a copy of the encoded actions. </returns>
		public byte[] Data => Arrays.copyOf(data, data.Length);

	    /// <summary>
		/// {@inheritDoc} </summary>
		public Action copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, data.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			return data.Length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeBytes(data);
		}
	}

}