using System.Collections.Generic;
using System.Linq;

/*
 * CharacterSet.java
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

namespace com.flagstone.transform.util.text
{

	/// <summary>
	/// CharacterSet is a convenience class for created a sorted list of characters
	/// that can be used to create a font definition.
	/// </summary>
	public sealed class CharacterSet
	{

		/// <summary>
		/// The set of characters. </summary>
		
		private readonly ISet<char?> characters = new HashSet<char?>();

		/// <summary>
		/// Add a character to the set of existing characters. </summary>
		/// <param name="character"> a character. If a character is already included then it
		/// is ignored. </param>


		public void add(char character)
		{
			characters.Add(character);
		}

		/// <summary>
		/// Add all the characters in a string to the set of existing characters. </summary>
		/// <param name="text"> a string of characters. If any character is already included
		/// then it is ignored. </param>


		public void add(string text)
		{
			for (int i = 0; i < text.Length; i++)
			{
				characters.Add(text[i]);
			}
		}

		/// <summary>
		/// Get the sorted list of characters.
		/// </summary>
		/// <returns> a list containing the characters in ascending order. </returns>
		public IList<char?> Characters
		{
			get
			{


				IList<char?> list = new List<char?>(characters);
				return list.OrderBy(l=>l).ToList();
			}
		}
	}

}