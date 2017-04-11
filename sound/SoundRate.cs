/*
 * SoundRates.java
 * Transform
 *
 * Copyright (c) 2010 Flagstone Software Ltd. All rights reserved.
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

namespace com.flagstone.transform.sound
{
	/// <summary>
	/// SoundRate defines the constants that identify the pre-defined sound
	/// sample rates supported by the Flash Player.
	/// </summary>
	public sealed class SoundRate
	{
		/// <summary>
		/// A 5 kHz sample rate. </summary>
		public const int KHZ_5K = 5512;
		/// <summary>
		/// An 8 kHz sample rate. </summary>
		public const int KHZ_8K = 8000;
		/// <summary>
		/// An 11 kHz sample rate. </summary>
		public const int KHZ_11K = 11025;
		/// <summary>
		/// A 22 kHz sample rate. </summary>
		public const int KHZ_22K = 22050;
		/// <summary>
		/// A 44 kHz sample rate. </summary>
		public const int KHZ_44K = 44100;

		/// <summary>
		/// SoundRate contains only constants. </summary>
		private SoundRate()
		{
			// private constructor
		}
	}

}