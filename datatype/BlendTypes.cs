﻿/*
 * BlendTypes.java
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
namespace com.flagstone.transform.datatype
{
	/// <summary>
	/// BlendTypes defines the values that are used to encode Blends.
	/// </summary>
	public sealed class BlendTypes
	{
		/// <summary>
		/// Value used to indicate that no Blend is used. </summary>
		public const int NULL = 0;
		/// <summary>
		/// Value used to identify a NORMAL Blend when it is encoded. </summary>
		public const int NORMAL = 1;
		/// <summary>
		/// Value used to identify a LAYER Blend when it is encoded. </summary>
		public const int LAYER = 2;
		/// <summary>
		/// Value used to identify a MULTIPLY Blend when it is encoded. </summary>
		public const int MULTIPLY = 3;
		/// <summary>
		/// Value used to identify a SCREEN Blend when it is encoded. </summary>
		public const int SCREEN = 4;
		/// <summary>
		/// Value used to identify a LIGHTEN Blend when it is encoded. </summary>
		public const int LIGHTEN = 5;
		/// <summary>
		/// Value used to identify a DARKEN Blend when it is encoded. </summary>
		public const int DARKEN = 6;
		/// <summary>
		/// Value used to identify a ADD Blend when it is encoded. </summary>
		public const int ADD = 7;
		/// <summary>
		/// Value used to identify a SUBTRACT Blend when it is encoded. </summary>
		public const int SUBTRACT = 8;
		/// <summary>
		/// Value used to identify a DIFFERENCE Blend when it is encoded. </summary>
		public const int DIFFERENCE = 9;
		/// <summary>
		/// Value used to identify a INVERT Blend when it is encoded. </summary>
		public const int INVERT = 10;
		/// <summary>
		/// Value used to identify a ALPHA Blend when it is encoded. </summary>
		public const int ALPHA = 11;
		/// <summary>
		/// Value used to identify a ERASE Blend when it is encoded. </summary>
		public const int ERASE = 12;
		/// <summary>
		/// Value used to identify a OVERLAY Blend when it is encoded. </summary>
		public const int OVERLAY = 13;
		/// <summary>
		/// Value used to identify a HARDLIGHT Blend when it is encoded. </summary>
		public const int HARDLIGHT = 14;

		/// <summary>
		/// Private constructor since class only contains constants. </summary>
		private BlendTypes()
		{
			// Class contains only constants.
		}
	}

}