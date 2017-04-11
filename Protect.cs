using System;
using com.flagstone.transform.coder;

/*
 * Protect.java
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

namespace com.flagstone.transform
{
    /// <summary>
	/// Protect marks a file as not-readable, preventing the file from being loaded
	/// into an editor.
	/// 
	/// <para>
	/// From Flash 4, a password field was added. In order to load a file in
	/// Macromedia's flash editor tool a password must be entered and the MD5 hash
	/// must match the value stored in the password field.
	/// </para>
	/// 
	/// <para>
	/// IMPORTANT: this form of protection only works with Macromedia's Flash
	/// Authoring tool. Any application that parses Flash files can choose to ignore
	/// or delete this data structure therefore it is not safe to use this to protect
	/// the contents of a Flash file.
	/// </para>
	/// 
	/// <para>
	/// Transform will parse all Flash files containing the Protect data structure.
	/// Since the encoded data is can be removed by trivial scripts the level of
	/// copy-protection offered is minimal. Indeed the use of the Protect mechanism
	/// in Flash movies may lead to a false sense of security, putting proprietary
	/// information at risk. Sensitive information should not be included in Flash
	/// movies.
	/// </para>
	/// </summary>
	public sealed class Protect : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Protect: { password=%s}";

		/// <summary>
		/// The MD5 encoded password. </summary>
		private string password;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a Protect object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Protect(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			/*
			 * Force a read of the entire password field, including any zero bytes
			 * that are encountered.
			 */
			if (length > 0)
			{
				coder.readUnsignedShort();
				password = coder.readString(length - 2);

				while (password[password.Length - 1] == 0)
				{
					password = password.Substring(0, password.Length - 1);
				}
			}
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a Protect object with no password - Flash versions 1 to 3 only.
		/// </summary>
		public Protect()
		{
			// password remains null
		}

		/// <summary>
		/// Creates a Protect object with the specified password - used for file with
		/// Flash version 4 and above.
		/// </summary>
		/// <param name="pass">
		///            the string defining the password. Must not be null. </param>


		public Protect(string pass)
		{
			Password = pass;
		}

		/// <summary>
		/// Creates and initialises a Protect object using the password copied
		/// from another Protect object.
		/// </summary>
		/// <param name="object">
		///            a Protect object from which the password will be
		///            copied. </param>


		public Protect(Protect @object)
		{
			password = @object.password;
		}

		/// <summary>
		/// Get the MD5 password hash. This may be null if the object was
		/// decoded from a file containing Flash version 2 or 3.
		/// </summary>
		/// <returns> the MD5 hash of the password used to protect the file. </returns>
		public string Password
		{
			get => password;
		    set
			{
				if (ReferenceEquals(value, null))
				{
					throw new ArgumentException();
				}
				password = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public Protect copy()
		{
			return new Protect(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, password);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			length = 0;

			if (!ReferenceEquals(password, null))
			{
				length += 2 + context.strlen(password);
			}
			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.PROTECT << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.PROTECT << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (!ReferenceEquals(password, null))
			{
				coder.writeShort(0);
				coder.writeString(password);
			}
		}
	}

}