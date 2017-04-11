using System;
using com.flagstone.transform.coder;

/*
 * ActionObject.java
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
	/// ActionObject is a general-purpose class that can be used to represent any
	/// action. It allow actions not supported in the current version of Transform to
	/// be decoded and encoded from movies until direct support is provided in the
	/// framework.
	/// </summary>
	public sealed class ActionObject : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "ActionObject: {" + "type=%d; data=byte[%s] {...} }";

		/// <summary>
		/// The type used to identify the action. </summary>
		
		private readonly int type;
		/// <summary>
		/// The encoded arguments, if any, used by the action. </summary>
		
		private readonly byte[] data;

		/// <summary>
		/// Creates and initialises an ActionObject using values encoded in the Flash
		/// binary format.
		/// </summary>
		/// <param name="actionType"> the value that identifies the action when it is
		/// encoded.
		/// </param>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public ActionObject(int actionType, SWFDecoder coder)
		{
			type = actionType;

			if (type > ActionTypes.HIGHEST_BYTE_CODE)
			{
				data = coder.readBytes(new byte[coder.readUnsignedShort()]);
			}
			else
			{
				data = new byte[0];
			}
		}

		/// <summary>
		/// Creates an ActionObject specifying only the type.
		/// </summary>
		/// <param name="actionType">
		///            the value identifying the action when it is encoded. </param>


		public ActionObject(int actionType)
		{
			type = actionType;
			data = new byte[0];
		}

		/// <summary>
		/// Creates an ActionObject specifying the type and the data that represents
		/// the body of the action encoded in the Flash binary format.
		/// </summary>
		/// <param name="actionType">
		///            the value identifying the action when it is encoded. </param>
		/// <param name="bytes">
		///            the body of the action when it is encoded in the Flash format. </param>


		public ActionObject(int actionType, byte[] bytes)
		{
			type = actionType;
			data = Arrays.copyOf(bytes, bytes.Length);
		}

		/// <summary>
		/// Creates an ActionObject by copying an existing one.
		/// </summary>
		/// <param name="object">
		///            an ActionObject. </param>


		public ActionObject(ActionObject @object)
		{
			type = @object.type;
			data = @object.data;
		}

		/// <summary>
		/// Returns the type that identifies the type of action when it is encoded in
		/// the Flash binary format.
		/// </summary>
		/// <returns> the value identifying the action when it is encoded. </returns>
		public int Type => type;

	    /// <summary>
		/// Returns the encoded data for the action.
		/// </summary>
		/// <returns> the bytes representing the encoded arguments of the action. </returns>
		public byte[] Data => Arrays.copyOf(data, data.Length);

	    /// <summary>
		/// {@inheritDoc} </summary>
		public ActionObject copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, type, data.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			int length;
			if (type > ActionTypes.HIGHEST_BYTE_CODE)
			{
				length = Coder.ACTION_HEADER + data.Length;
			}
			else
			{
				length = 1;
			}
			return length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(type);

			if (type > ActionTypes.HIGHEST_BYTE_CODE)
			{
				coder.writeShort(data.Length);
				coder.writeBytes(data);
			}
		}
	}

}