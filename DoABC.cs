using System;
using com.flagstone.transform.coder;

/*
 * DoABC.java
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

namespace com.flagstone.transform
{
    /// <summary>
	/// DoABC is used to define scripts containing Actionscript 3.0 byte-codes.
	/// 
	/// <para>
	/// Execution of the script may be deferred until it is explicitly called using
	/// the assigned name.
	/// </para>
	/// </summary>
	public sealed class DoABC : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DoABC: { name=%s; deferred=%d;" + " actions=byte<%d> ...}";

		/// <summary>
		/// The name of the script. </summary>
		private string name;
		/// <summary>
		/// Is loading deferred until the script is called. </summary>
		private int deferred;
		/// <summary>
		/// The encoded actionscript 3 bytes codes. </summary>
		private byte[] data;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises an DoABC using values encoded in the Flash
		/// binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public DoABC(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			deferred = coder.readInt();
			name = coder.readString();
			data = coder.readBytes(new byte[length - coder.bytesRead()]);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a DoABC object with the name and compiled Actionscript 3.0
		/// byte-codes.
		/// </summary>
		/// <param name="scriptName">
		///            the name used to identify the script. </param>
		/// <param name="defer">
		///            whether execution of the script is deferred. </param>
		/// <param name="script">
		///            the compiled Actionscript 3.0 byte-codes. </param>


		public DoABC(string scriptName, bool defer, byte[] script)
		{
			Name = scriptName;
			Deferred = defer;
			Data = script;
		}

		/// <summary>
		/// Creates a DoABC initialised with a copy of the data from another object.
		/// </summary>
		/// <param name="object">
		///            a DoABC object used to initialize this one. </param>


		public DoABC(DoABC @object)
		{
			name = @object.name;
			deferred = @object.deferred;
			data = @object.data;
		}

		/// <summary>
		/// Get the name of the script.
		/// </summary>
		/// <returns> the name used to call the script. </returns>
		public string Name
		{
			get => name;
		    set
			{
				if (ReferenceEquals(value, null) || value.Length == 0)
				{
					throw new ArgumentException();
				}
				name = value;
			}
		}


		/// <summary>
		/// Is loading of the script deferred until it is called.
		/// </summary>
		/// <returns> true if loading of the script is deferred, false if it is loaded
		/// immediately. </returns>
		public bool Deferred
		{
			get
			{
				return (deferred & 1) != 0;
			}
			set
			{
				if (value)
				{
					deferred = 1;
				}
				else
				{
					deferred = 0;
				}
			}
		}


		/// <summary>
		/// Get a copy of the array containing the Actionscript byte-codes.
		/// </summary>
		/// <returns> a copy of the encoded actionscript. </returns>
		public byte[] Data
		{
			get => Arrays.copyOf(data, data.Length);
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				data = Arrays.copyOf(value, value.Length);
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public DoABC copy()
		{
			return new DoABC(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, name, deferred, data.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			length = 4 + context.strlen(name) + data.Length;

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DO_ABC << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DO_ABC << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeInt(deferred);
			coder.writeString(name);
			coder.writeBytes(data);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}