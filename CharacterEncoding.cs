using System;
using System.Collections.Generic;

/*
 * CharacterEncoding.java
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
    /// CharacterEncoding is used to identify the encoding used for characters in
    /// strings stored in the movie.
    /// </summary>
    public sealed class CharacterEncoding
    {
        /// <summary>
        /// Defines that the characters in a font or string are encoded using SJIS
        /// standard for representing Kanji characters.
        /// </summary>
        public static readonly CharacterEncoding SJIS = new CharacterEncoding("SJIS", InnerEnum.SJIS, "Shift-JIS");
        /// <summary>
        /// Defines that the characters in a font or string are encoded using the
        /// ANSI (ASCII) standard.
        /// </summary>
        public static readonly CharacterEncoding ANSI = new CharacterEncoding("ANSI", InnerEnum.ANSI, "ASCII");
        /// <summary>
        /// Defines that the characters in a font or string are encoded using
        /// Unicode (UTF-8).
        /// </summary>
        public static readonly CharacterEncoding UTF8 = new CharacterEncoding("UTF8", InnerEnum.UTF8, "UTF-8");
        /// <summary>
        /// Microsoft's extension to Shift-JIS.
        /// MS932, windows-932, and csWindows31J are all known aliases of this
        /// character set.
        /// </summary>
        public static readonly CharacterEncoding WINDOWS31J = new CharacterEncoding("WINDOWS31J", InnerEnum.WINDOWS31J, "windows-31j");
        /// <summary>
        /// MS932 is an alias for Microsoft's extension to Shift-JIS. It is not
        /// clear whether the same set of extensions is used in CP932.
        /// </summary>
        public static readonly CharacterEncoding MS932 = new CharacterEncoding("MS932", InnerEnum.MS932, "MS932");
        /// <summary>
        /// Defines that the characters in a font or string are encoded using
        /// Microsoft's extensions (Code Page 932) to the SJIS standard for
        /// representing Kanji characters.
        /// </summary>
        public static readonly CharacterEncoding CP932 = new CharacterEncoding("CP932", InnerEnum.CP932, "CP932");

        private static readonly IList<CharacterEncoding> valueList = new List<CharacterEncoding>();

        public enum InnerEnum
        {
            SJIS,
            ANSI,
            UTF8,
            WINDOWS31J,
            MS932,
            CP932
        }

        public readonly InnerEnum innerEnumValue;
        private readonly string nameValue;
        private readonly int ordinalValue;
        private static int nextOrdinal;

        /// <summary>
        /// Table used to map CharSet canonical names to a CharacterEncoding. </summary>
        private static readonly IDictionary<string, CharacterEncoding> TABLE = new Dictionary<string, CharacterEncoding>();

        static CharacterEncoding()
        {
            foreach (CharacterEncoding set in values())
            {
                TABLE[set.encoding] = set;
            }

            valueList.Add(SJIS);
            valueList.Add(ANSI);
            valueList.Add(UTF8);
            valueList.Add(WINDOWS31J);
            valueList.Add(MS932);
            valueList.Add(CP932);
        }

        /// <summary>
        /// Holds character set encoding name used in Java. </summary>
        private string encoding;

        /// <summary>
        /// Private constructor used for enum values. </summary>
        /// <param name="enc"> the name representing the character encoding. </param>


        private CharacterEncoding(string name, InnerEnum innerEnum, string enc)
        {
            encoding = enc;

            nameValue = name;
            ordinalValue = nextOrdinal++;
            innerEnumValue = innerEnum;
        }

        /// <summary>
        /// Get the string used by Java to identify the character encoding.
        /// </summary>
        /// <returns> the name commonly used to represent the character encoding. </returns>
        public string Encoding => encoding;

        public static IList<CharacterEncoding> values()
        {
            return valueList;
        }

        public int ordinal()
        {
            return ordinalValue;
        }

        public override string ToString()
        {
            return nameValue;
        }

        public static CharacterEncoding valueOf(string name)
        {
            foreach (CharacterEncoding enumInstance in valueList)
            {
                if (enumInstance.nameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new ArgumentException(name);
        }
    }

}