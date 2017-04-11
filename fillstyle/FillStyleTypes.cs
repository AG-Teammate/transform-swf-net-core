/*
 * FillTypes.java
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

namespace com.flagstone.transform.fillstyle
{
	/// <summary>
	/// FillTypes defines the constants that identify a fill style when it is
	/// encoded according to the Flash file format specification.
	/// </summary>
	public sealed class FillStyleTypes
	{
		/// <summary>
		/// The type identifying a SolidFill object when it is encoded. </summary>
		public const int SOLID_COLOR = 0;
		/// <summary>
		/// The type identifying a linear GradientFill object when it is encoded. </summary>
		public const int LINEAR_GRADIENT = 0x10;
		/// <summary>
		/// The type identifying a radial GradientFill object when it is encoded. </summary>
		public const int RADIAL_GRADIENT = 0x12;
		/// <summary>
		/// The type identifying a FocalGradientFill object when it is encoded. </summary>
		public const int FOCAL_GRADIENT = 0x13;
		/// <summary>
		/// The type identifying a tiled BitmapFill object when it is encoded. </summary>
		public const int TILED_BITMAP = 0x40;
		/// <summary>
		/// The type identifying a clipped BitmapFill object when it is encoded. </summary>
		public const int CLIPPED_BITMAP = 0x41;
		/// <summary>
		/// The type identifying a unsmoothed tiled BitmapFill object
		/// when it is encoded. 
		/// </summary>
		public const int UNSMOOTH_TILED = 0x42;
		/// <summary>
		/// The type identifying a unsmoothed clipped BitmapFill object
		/// when it is encoded. 
		/// </summary>
		public const int UNSMOOTH_CLIPPED = 0x43;

		/// <summary>
		/// FillTypes contains only constants. </summary>
		private FillStyleTypes()
		{
			// private constructor
		}
	}

}