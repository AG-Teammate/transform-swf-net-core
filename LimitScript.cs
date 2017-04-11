/*
 * LimitScript.java
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

using System;
using System.IO;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

namespace com.flagstone.transform
{
    /// <summary>
	/// The LimitScript is used to define the execution environment of the Flash
	/// Player, limiting the resources available when executing actions and improving
	/// performance.
	/// 
	/// <para>
	/// LimitScript can be used to limit the maximum recursion depth and limit the
	/// time a sequence of actions can execute for. This provides a rudimentary
	/// mechanism for people viewing a movie to regain control of the Flash Player
	/// should a script fail.
	/// </para>
	/// </summary>
	public sealed class LimitScript : MovieTag
	{

		/// <summary>
		/// Maximum stack depth for recursive functions. </summary>
		private const int MAX_DEPTH = 65535;
		/// <summary>
		/// Maximum timeout in seconds for function execution. </summary>
		private const int MAX_TIMEOUT = 65535;

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "LimitScript: { depth=%d;" + " timeout=%d}";

		/// <summary>
		/// The maximum stack depth for nested functions. </summary>
		private int depth;
		/// <summary>
		/// The maximum execution time of a script. </summary>
		private int timeout;

		/// <summary>
		/// Creates and initialises a LimitScript object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public LimitScript(SWFDecoder coder)
		{
			if ((coder.readUnsignedShort() & Coder.LENGTH_FIELD) == Coder.IS_EXTENDED)
			{
				coder.readInt();
			}
			depth = coder.readUnsignedShort();
			timeout = coder.readUnsignedShort();
		}

		/// <summary>
		/// Creates a LimitScript object that limits the recursion depth to
		/// <em>depth</em> levels and specifies that any sequence of actions will
		/// timeout after <em>timeout</em> seconds.
		/// </summary>
		/// <param name="stackDepth">
		///            the maximum depth a sequence of actions can recurse to. Must
		///            be in the range 0..65535. </param>
		/// <param name="timeLimit">
		///            the time in seconds that a sequence of actions is allowed to
		///            execute before the Flash Player displays a dialog box asking
		///            whether the script should be terminated. Must be in the range
		///            0..65535. </param>


		public LimitScript(int stackDepth, int timeLimit)
		{
			Depth = stackDepth;
			Timeout = timeLimit;
		}

		/// <summary>
		/// Creates and initialises a LimitScript object using the values copied
		/// from another LimitScript object.
		/// </summary>
		/// <param name="object">
		///            a LimitScript object from which the values will be
		///            copied. </param>


		public LimitScript(LimitScript @object)
		{
			depth = @object.depth;
			timeout = @object.timeout;
		}

		/// <summary>
		/// Get the maximum stack depth for function execution.
		/// </summary>
		/// <returns> the maximum number of stack frames for recursive functions. </returns>
		public int Depth
		{
			get => depth;
		    set
			{
				if ((value < 0) || (value > MAX_DEPTH))
				{
					throw new IllegalArgumentRangeException(0, MAX_DEPTH, value);
				}
				depth = value;
			}
		}


		/// <summary>
		/// Get the maximum time a sequence of actions will execute before the
		/// Flash Player present a dialog box asking whether the script should be
		/// terminated.
		/// </summary>
		/// <returns> the maximum execution time of a script. </returns>
		public int Timeout
		{
			get => timeout;
		    set
			{
				if ((value < 0) || (value > MAX_TIMEOUT))
				{
					throw new IllegalArgumentRangeException(0, MAX_TIMEOUT, value);
				}
				timeout = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public LimitScript copy()
		{
			return new LimitScript(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, depth, timeout);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 6;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 2 LINES
			coder.writeShort((MovieTypes.LIMIT_SCRIPT << Coder.LENGTH_FIELD_SIZE) | 4);
			coder.writeShort(depth);
			coder.writeShort(timeout);
		}
	}

}