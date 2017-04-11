﻿/*
 * SWFEncodeable.java
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

namespace com.flagstone.transform.coder
{

	/// <summary>
	/// The Encodeable interface defines the set of methods that all classes must
	/// implement so they can be encoded to a Flash (SWF) file.
	/// </summary>
	public interface SWFEncodeable
	{
		/// <summary>
		/// Prepare an object for encoding, returning the expected size of an object
		/// when it is encoded. This method also used to initialise variables, such
		/// as offsets and flags that will be used when the object is encoded.
		/// 
		/// Generally the method returns the size in bytes, however when called on
		/// objects that use bit fields such as shapes the methods will return the
		/// size in bits.
		/// </summary>
		/// <param name="context">
		///            an Context that allows information to be passed between
		///            objects to control how they are initialised for encoding.
		/// </param>
		/// <returns> the size of the object when it is encoded. </returns>


		int prepareToEncode(Context context);

		/// <summary>
		/// Encode an object to the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFEncoder object.
		/// </param>
		/// <param name="context">
		///            an Context that allows information to be passed between
		///            objects to control how they are initialised for encoding.
		/// </param>
		/// <exception cref="IOException"> if an error occurs while encoding the object. </exception>



		void encode(SWFEncoder coder, Context context);
	}

}