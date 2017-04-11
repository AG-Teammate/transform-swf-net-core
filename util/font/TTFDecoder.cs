using System;
using System.Collections.Generic;
using System.IO;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.font;
using com.flagstone.transform.shape;
using com.flagstone.transform.util.shape;

/*
 * TTFDecoder.java
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

namespace com.flagstone.transform.util.font
{
    /// <summary>
    /// TTFDecoder decodes TrueType or OpenType Fonts so they can be used in a
    /// Flash file.
    /// </summary>


    public sealed class TTFDecoder : FontProvider, FontDecoder
    {

        /// <summary>
        /// TableEntry is used to load the encoded TrueType tables so they can
        /// be decoded.
        /// </summary>
        private sealed class TableEntry : IComparable<TableEntry>
        {
            /// <summary>
            /// The table name/signature. </summary>

            internal int type;
            /// <summary>
            /// The offset to the start of the table. </summary>

            internal int offset;
            /// <summary>
            /// The number of bytes in the table. </summary>

            internal int length;
            /// <summary>
            /// The encoded table data. </summary>

            internal byte[] data;

            /// <summary>
            /// {@inheritDoc} </summary>


            public int CompareTo(TableEntry obj)
            {
                int result;
                if (offset < obj.offset)
                {
                    result = -1;
                }
                else if (offset == obj.offset)
                {
                    result = 0;
                }
                else
                {
                    result = 1;
                }
                return result;
            }

            /// <summary>
            /// {@inheritDoc} </summary>


            public override bool Equals(object @object)
            {
                bool result;
                TableEntry entry;

                if (@object == null)
                {
                    result = false;
                }
                else if (@object == this)
                {
                    result = true;
                }
                else if (@object is TableEntry)
                {
                    entry = (TableEntry)@object;
                    result = offset == entry.offset;
                }
                else
                {
                    result = false;
                }
                return result;
            }

            /// <summary>
            /// {@inheritDoc} </summary>
            public override int GetHashCode()
            {
                return offset * Constants.PRIME;
            }

            /// <summary>
            /// Set the table data. </summary>
            /// <param name="bytes"> the contents of the table. </param>


            public byte[] Data
            {
                set => data = Arrays.copyOf(value, value.Length);
                get => Arrays.copyOf(data, data.Length);
            }
        }

        /// <summary>
        /// Use to store entries from the NAME table.
        /// </summary>
        private sealed class NameEntry
        {
            /// <summary>
            /// platform identifier. </summary>
            internal int platform;
            /// <summary>
            /// character encoding identifier. </summary>
            internal int encoding;
            /// <summary>
            /// language identifier. </summary>
            internal int language;
            /// <summary>
            /// name identifier. </summary>
            internal int name;
            /// <summary>
            /// length of the name string. </summary>
            internal int length;
            /// <summary>
            /// offset from the start of the table where the string is located. </summary>
            internal int offset;
        }

        /// <summary>
        /// Number of bits to shift to convert bytes to bits. </summary>
        private const int BYTES_TO_BITS = 3;
        /// <summary>
        /// The number of bits to shift a byte to sign extend to 32-bits. </summary>
        private const int SIGN_EXTEND = 24;

        /// <summary>
        /// The name of the OS/2 table. </summary>
        private const int OS_2 = 0x4F532F32;
        /// <summary>
        /// The name of the head table. </summary>
        private const int HEAD = 0x68656164;
        /// <summary>
        /// The name of the hhea table. </summary>
        private const int HHEA = 0x68686561;
        /// <summary>
        /// The name of the maxp table. </summary>
        private const int MAXP = 0x6D617870;
        /// <summary>
        /// The name of the loca table. </summary>
        private const int LOCA = 0x6C6F6361;
        /// <summary>
        /// The name of the cmap table. </summary>
        private const int CMAP = 0x636D6170;
        /// <summary>
        /// The name of the hmtx table. </summary>
        private const int HMTX = 0x686D7478;
        /// <summary>
        /// The name of the name table. </summary>
        private const int NAME = 0x6E616D65;
        /// <summary>
        /// The name of the glyf table. </summary>
        private const int GLYF = 0x676C7966;

        /// <summary>
        /// Indicates the offset to the glyph is encoded in 16-bits. </summary>
        private const int ITLF_SHORT = 0;
        //    private static final int ITLF_LONG = 1;

        //    private static final int WEIGHT_THIN = 100;
        //    private static final int WEIGHT_EXTRALIGHT = 200;
        //    private static final int WEIGHT_LIGHT = 300;
        //    private static final int WEIGHT_NORMAL = 400;
        //    private static final int WEIGHT_MEDIUM = 500;
        //    private static final int WEIGHT_SEMIBOLD = 600;
        /// <summary>
        /// Code identifying a font is bold. </summary>
        private const int WEIGHT_BOLD = 700;
        //    private static final int WEIGHT_EXTRABOLD = 800;
        //    private static final int WEIGHT_BLACK = 900;
        /// <summary>
        /// Mask for the field that identifies whether a point is located on the
        /// outline of a glyph.
        /// </summary>
        private const int ON_CURVE = 0x01;
        /// <summary>
        /// Mask for the field that identifies whether the x-coordinate of a point
        /// encoded in 16-bits.
        /// </summary>
        private const int X_SHORT = 0x02;
        /// <summary>
        /// Mask for the field that identifies whether the y-coordinate of a point
        /// encoded in 16-bits.
        /// </summary>
        private const int Y_SHORT = 0x04;
        /// <summary>
        /// Mask for the field that identifies whether the coordinate of a point
        /// is repeated.
        /// </summary>
        private const int REPEAT_FLAG = 0x08;
        /// <summary>
        /// Mask for the field that identifies whether the x-coordinate of a point
        /// is unchanged.
        /// </summary>
        private const int X_SAME = 0x10;
        /// <summary>
        /// Mask for the field that identifies whether the y-coordinate of a point
        /// is unchanged.
        /// </summary>
        private const int Y_SAME = 0x20;
        /// <summary>
        /// Mask for the field that identifies whether the value for the relative
        /// change in the x-coordinate of a point is added to the previous value.
        /// </summary>
        private const int X_POSITIVE = 0x10;
        /// <summary>
        /// Mask for the field that identifies whether the value for the relative
        /// change in the y-coordinate of a point is added to the previous value.
        /// </summary>
        private const int Y_POSITIVE = 0x20;
        /// <summary>
        /// The coordinates for the encoded glyph is 32-bits. </summary>
        private const int ARGS_ARE_WORDS = 0x01;
        /// <summary>
        /// X and Y coordinates are encoded. </summary>
        private const int ARGS_ARE_XY = 0x02;
        /// <summary>
        /// The font contains scaling information. </summary>
        private const int HAVE_SCALE = 0x08;
        /// <summary>
        /// Scaling for both the x and y axes are included. </summary>
        private const int HAVE_XYSCALE = 0x40;
        /// <summary>
        /// Scaling for both the x and y axes includes an offset. </summary>
        private const int HAVE_2X2 = 0x80;
        /// <summary>
        /// The outline of the glyph has more points to be decoded. </summary>
        private const int HAS_MORE = 0x10;

        /// <summary>
        /// The name of the font. </summary>

        private string name;
        /// <summary>
        /// Indicates whether the font weight is bold. </summary>

        private bool bold;
        /// <summary>
        /// Indicates whether the font is italicised. </summary>

        private bool italic;
        /// <summary>
        /// The encoding used for characters. </summary>

        private CharacterFormat encoding;
        /// <summary>
        /// The ascent of the font. </summary>

        private float ascent;
        /// <summary>
        /// The descent of the font. </summary>

        private float descent;
        /// <summary>
        /// The leading of the font. </summary>

        private float leading;
        /// <summary>
        /// Table mapping character code to glyphs. </summary>

        private int[] charToGlyph;
        /// <summary>
        /// Table mapping glyph to character codes. </summary>

        private int[] glyphToChar;
        /// <summary>
        /// Glyphs. </summary>

        private TrueTypeGlyph[] glyphTable;
        /// <summary>
        /// The number of glyphs defined in the font. </summary>

        private int glyphCount;
        /// <summary>
        /// The index of the glyph that represents unsupported characters. </summary>

        private int missingGlyph;
        /// <summary>
        /// The highest character code represented in the font. </summary>

        private char maxChar;
        /// <summary>
        /// The amount to scale coordinates so the font maps to the EM-SQUARE. </summary>

        private int scale = 1;
        /// <summary>
        /// The number of entries in the table of advances for each glyph. </summary>

        private int metrics;
        /// <summary>
        /// The size of each entry in the glyph table, either 16 or 32 bits. </summary>

        private int glyphOffset;
        /// <summary>
        /// The offsets in bytes to each glyph in the GLYF table. </summary>

        private int[] offsets;

        /// <summary>
        /// Directory of tables encoded in the font. </summary>

        private readonly IDictionary<int?, TableEntry> table = new Dictionary<int?, TableEntry>();

        /// <summary>
        /// Table of fonts decoded from the font definition. </summary>

        private readonly IList<Font> fonts = new List<Font>();

        /// <summary>
        /// {@inheritDoc} </summary>
        public FontDecoder newDecoder()
        {
            return new TTFDecoder();
        }

        /// <summary>
        /// {@inheritDoc} </summary>



        public void read(FileInfo file)
        {


            FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            try
            {
                read(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// {@inheritDoc} </summary>
        public IList<Font> Fonts => fonts;

        /// <summary>
        /// Read a font from an input stream. </summary>
        /// <param name="stream"> the stream containing the font data. </param>
        /// <exception cref="IOException"> if there is an error reading the font data. </exception>



        public void read(Stream stream)
        {
            loadTables(stream);
            decodeTables();



            Font font = new Font();

            font.Face = new FontFace(name, bold, italic);
            font.Encoding = encoding;
            font.Ascent = (int)ascent;
            font.Descent = (int)descent;
            font.Leading = (int)leading;
            font.NumberOfGlyphs = glyphCount;
            font.MissingGlyph = missingGlyph;
            font.HighestChar = maxChar;

            for (int i = 0; i < glyphCount; i++)
            {
                font.addGlyph((char)glyphToChar[i], glyphTable[i]);
            }

            fonts.Add(font);
        }

        /// <summary>
        /// Load the tables from the TrueType table directory. </summary>
        /// <param name="stream"> the InputStream containing the font data. </param>
        /// <exception cref="IOException"> if there is an error loading the table data. </exception>



        private void loadTables(Stream stream)
        {


            BigDecoder coder = new BigDecoder(stream);
            coder.mark();

            /* float version = */
            coder.readInt();


            int tableCount = coder.readUnsignedShort();
            /* int searchRange = */
            coder.readUnsignedShort();
            /* int entrySelector = */
            coder.readUnsignedShort();
            /* int rangeShift = */
            coder.readUnsignedShort();

            TableEntry[] directory = new TableEntry[tableCount];

            for (int i = 0; i < tableCount; i++)
            {
                directory[i] = new TableEntry();
                directory[i].type = coder.readInt();
                /* checksum */
                coder.readInt();
                directory[i].offset = coder.readInt();
                directory[i].length = coder.readInt();
            }

            Array.Sort(directory);

            foreach (TableEntry entry in directory)
            {
                coder.skip(entry.offset - coder.bytesRead());
                entry.Data = coder.readBytes(new byte[entry.length]);
                table[entry.type] = entry;
            }
        }

        /// <summary>
        /// Decode the data from the loaded tables. The order is important since
        /// some tables (maxp) contains values such as the number of glyphs etc.
        /// that are used to size the table used to decode the glyphs. </summary>
        /// <exception cref="IOException"> if there is an error decoding the data. </exception>


        private void decodeTables()
        {
            decodeMAXP(table[MAXP]);
            decodeOS2(table[OS_2]);
            decodeHEAD(table[HEAD]);
            decodeHHEA(table[HHEA]);
            decodeNAME(table[NAME]);
            decodeLOCA(table[LOCA]);
            decodeGlyphs(table[GLYF]);
            decodeHMTX(table[HMTX]);
            decodeCMAP(table[CMAP]);
        }

        /// <summary>
        /// Decode the HEAD table.
        /// </summary>
        /// <param name="entry"> the bleEntry containing the encoded HEAD table data. </param>
        /// <exception cref="IOException"> if an error occurs while decoding the table data. </exception>



        private void decodeHEAD(TableEntry entry)
        {


            byte[] data = entry.Data;


            MemoryStream stream = new MemoryStream(data);


            BigDecoder coder = new BigDecoder(stream, data.Length);



            byte[] date = new byte[8];

            coder.readInt(); // table version fixed 16
            coder.readInt(); // font version fixed 16
            coder.readInt(); // checksum adjustment
            coder.readInt(); // magic number
            coder.readUnsignedShort(); // See following comments
                                       // bit15: baseline at y=0
                                       // bit14: side bearing at x=0;
                                       // bit13: instructions depend on point size
                                       // bit12: force ppem to integer values
                                       // bit11: instructions may alter advance
                                       // bits 10-0: unused.
            scale = coder.readUnsignedShort() / 1024; // units per em

            if (scale == 0)
            {
                scale = 1;
            }

            coder.readBytes(date); // number of seconds since midnight, Jan 01 1904
            coder.readBytes(date); // number of seconds since midnight, Jan 01 1904

            coder.readShort(); // xMin for all glyph bounding boxes
            coder.readShort(); // yMin for all glyph bounding boxes
            coder.readShort(); // xMax for all glyph bounding boxes
            coder.readShort(); // yMax for all glyph bounding boxes

            /*
			 * Next two byte define font appearance on Macs, values are specified in
			 * the OS/2 table
			 */


            int flags = coder.readUnsignedShort();
            bold = (flags & Coder.BIT15) != 0;
            italic = (flags & Coder.BIT10) != 0;

            coder.readUnsignedShort(); // smallest readable size in pixels
            coder.readShort(); // font direction hint
            glyphOffset = coder.readShort();
            coder.readShort(); // glyph data format
        }

        /// <summary>
        /// Decode the HHEA table.
        /// </summary>
        /// <param name="entry"> the bleEntry containing the encoded HHEA table data. </param>
        /// <exception cref="IOException"> if an error occurs while decoding the table data. </exception>



        private void decodeHHEA(TableEntry entry)
        {


            byte[] data = entry.Data;


            MemoryStream stream = new MemoryStream(data);


            BigDecoder coder = new BigDecoder(stream, data.Length);

            coder.readInt(); // table version, fixed 16
            ascent = coder.readShort() / (float)scale;
            descent = -(coder.readShort() / (float)scale);
            leading = coder.readShort() / (float)scale;

            coder.readUnsignedShort(); // maximum advance in the htmx table
            coder.readShort(); // minimum left side bearing in the htmx table
            coder.readShort(); // minimum right side bearing in the htmx table
            coder.readShort(); // maximum extent
            coder.readShort(); // caret slope rise
            coder.readShort(); // caret slope run
            coder.readShort(); // caret offset

            coder.readUnsignedShort(); // reserved
            coder.readUnsignedShort(); // reserved
            coder.readUnsignedShort(); // reserved
            coder.readUnsignedShort(); // reserved

            coder.readShort(); // metric data format

            metrics = coder.readUnsignedShort();
        }

        /// <summary>
        /// Decode the OS/2 table.
        /// </summary>
        /// <param name="entry"> the bleEntry containing the encoded OS/2 table data. </param>
        /// <exception cref="IOException"> if an error occurs while decoding the table data. </exception>



        private void decodeOS2(TableEntry entry)
        {


            byte[] data = entry.Data;


            MemoryStream stream = new MemoryStream(data);


            BigDecoder coder = new BigDecoder(stream, data.Length);



            byte[] panose = new byte[10];


            int[] unicodeRange = new int[4];


            byte[] vendor = new byte[4];



            int version = coder.readUnsignedShort(); // version
            coder.readShort(); // average character width



            int weight = coder.readUnsignedShort();

            if (weight == WEIGHT_BOLD)
            {
                bold = true;
            }

            coder.readUnsignedShort(); // width class
            coder.readUnsignedShort(); // embedding licence

            coder.readShort(); // subscript x size
            coder.readShort(); // subscript y size
            coder.readShort(); // subscript x offset
            coder.readShort(); // subscript y offset
            coder.readShort(); // superscript x size
            coder.readShort(); // superscript y size
            coder.readShort(); // superscript x offset
            coder.readShort(); // superscript y offset
            coder.readShort(); // width of strikeout stroke
            coder.readShort(); // strikeout stroke position
            coder.readShort(); // font family class

            coder.readBytes(panose);

            for (int i = 0; i < 4; i++)
            {
                unicodeRange[i] = coder.readInt();
            }

            coder.readBytes(vendor); // font vendor identification


            int flags = coder.readUnsignedShort();
            italic = (flags & Coder.BIT15) != 0;
            bold = (flags & Coder.BIT10) != 0;

            coder.readUnsignedShort(); // first unicode character code
            coder.readUnsignedShort(); // last unicode character code

            ascent = coder.readUnsignedShort() / (float)scale;
            descent = -(coder.readUnsignedShort() / (float)scale);
            leading = coder.readUnsignedShort() / (float)scale;

            coder.readUnsignedShort(); // ascent in Windows
            coder.readUnsignedShort(); // descent in Windows

            if (version > 0)
            {
                coder.readInt(); // code page range
                coder.readInt(); // code page range

                if (version > 1)
                {
                    coder.readShort(); // height
                    coder.readShort(); // Capitals height
                    missingGlyph = coder.readUnsignedShort();
                    coder.readUnsignedShort(); // break character
                    coder.readUnsignedShort(); // maximum context
                }
            }
        }

        /// <summary>
        /// Decode the NAME table.
        /// </summary>
        /// <param name="entry"> the bleEntry containing the encoded NAME table data. </param>
        /// <exception cref="IOException"> if an error occurs while decoding the table data. </exception>



        private void decodeNAME(TableEntry entry)
        {


            byte[] data = entry.Data;


            MemoryStream stream = new MemoryStream(data);


            BigDecoder coder = new BigDecoder(stream, data.Length);
            /* final int format = */
            coder.readUnsignedShort();


            int names = coder.readUnsignedShort();


            int tableOffset = coder.readUnsignedShort();

            NameEntry[] nameTable = new NameEntry[names];

            for (int i = 0; i < names; i++)
            {
                nameTable[i] = new NameEntry();
                nameTable[i].platform = coder.readUnsignedShort();
                nameTable[i].encoding = coder.readUnsignedShort();
                nameTable[i].language = coder.readUnsignedShort();
                nameTable[i].name = coder.readUnsignedShort();
                nameTable[i].length = coder.readUnsignedShort();
                nameTable[i].offset = coder.readUnsignedShort();
            }

            for (int i = 0; i < names; i++)
            {
                coder.reset();
                coder.skip(tableOffset + nameTable[i].offset);



                byte[] bytes = new byte[nameTable[i].length];
                coder.readBytes(bytes);

                string nameEncoding = "UTF-8";

                if (nameTable[i].platform == 0)
                {
                    // Unicode
                    nameEncoding = "UTF-16";
                }
                else if (nameTable[i].platform == 1)
                {
                    // Macintosh
                    if ((nameTable[i].encoding == 0) && (nameTable[i].language == 0))
                    {
                        nameEncoding = "ISO8859-1";
                    }
                }
                else if (nameTable[i].platform == 3)
                {
                    // Microsoft
                    switch (nameTable[i].encoding)
                    {
                        case 1:
                            nameEncoding = "UTF-16";
                            break;
                        case 2:
                            nameEncoding = "SJIS";
                            break;
                        case 4:
                            nameEncoding = "Big5";
                            break;
                        default:
                            nameEncoding = "UTF-8";
                            break;
                    }
                }

                try
                {
                    if (nameTable[i].name == 1)
                    {
                        name = StringHelperClass.NewString(bytes, nameEncoding);
                    }
                }


                catch (ArgumentException)
                {
                    name = StringHelperClass.NewString(bytes);
                }
            }
        }

        /// <summary>
        /// Decode the MAXP table.
        /// </summary>
        /// <param name="entry"> the bleEntry containing the encoded MAXP table data. </param>
        /// <exception cref="IOException"> if an error occurs while decoding the table data. </exception>



        private void decodeMAXP(TableEntry entry)
        {


            byte[] data = entry.Data;


            MemoryStream stream = new MemoryStream(data);


            BigDecoder coder = new BigDecoder(stream, data.Length);


            float version = coder.readInt() / Coder.SCALE_16;

            glyphCount = coder.readUnsignedShort();
            glyphTable = new TrueTypeGlyph[glyphCount];
            glyphToChar = new int[glyphCount];

            if (version == 1.0f)
            {
                coder.readUnsignedShort(); // max no. of points in a simple glyph
                coder.readUnsignedShort(); // max no. of contours in a simple glyph
                coder.readUnsignedShort(); // max no. of points in a composite glyph
                coder.readUnsignedShort(); // max no. of composite glyph contours
                coder.readUnsignedShort(); // max no. of zones
                coder.readUnsignedShort(); // max no. of point in Z0
                coder.readUnsignedShort(); // number of storage area locations
                coder.readUnsignedShort(); // max no. of FDEFs
                coder.readUnsignedShort(); // max no. of IDEFs
                coder.readUnsignedShort(); // maximum stack depth
                coder.readUnsignedShort(); // max byte count for glyph instructions
                coder.readUnsignedShort(); // max no. of composite glyphs components
                coder.readUnsignedShort(); // max levels of recursion
            }
        }

        /// <summary>
        /// Decode the HMTX table.
        /// </summary>
        /// <param name="entry"> the bleEntry containing the encoded HMTX table data. </param>
        /// <exception cref="IOException"> if an error occurs while decoding the table data. </exception>



        private void decodeHMTX(TableEntry entry)
        {


            byte[] data = entry.Data;


            MemoryStream stream = new MemoryStream(data);


            BigDecoder coder = new BigDecoder(stream, data.Length);
            int index = 0;

            for (index = 0; index < metrics; index++)
            {
                glyphTable[index].Advance = (coder.readUnsignedShort() / scale);
                coder.readShort(); // left side bearing
            }



            int advance = glyphTable[index - 1].Advance;

            while (index < glyphCount)
            {
                glyphTable[index++].Advance = advance;
            }

            while (index < glyphCount)
            {
                coder.readShort();
                index++;
            }
        }

        /// <summary>
        /// Decode the CMAP table.
        /// </summary>
        /// <param name="entry"> the bleEntry containing the encoded CMAP table data. </param>
        /// <exception cref="IOException"> if an error occurs while decoding the table data. </exception>



        private void decodeCMAP(TableEntry entry)
        {


            byte[] data = entry.Data;


            MemoryStream stream = new MemoryStream(data);


            BigDecoder coder = new BigDecoder(stream, data.Length);
            /* final int version = */
            coder.readUnsignedShort();


            int numberOfTables = coder.readUnsignedShort();

            int platformId;
            int encodingId;
            int offset = 0;
            int format = 0;

            for (int tableCount = 0; tableCount < numberOfTables; tableCount++)
            {
                platformId = coder.readUnsignedShort();
                encodingId = coder.readUnsignedShort();
                offset = coder.readInt();
                coder.mark();

                if (platformId == 0)
                {
                    // Unicode
                    encoding = CharacterFormat.UCS2;
                }
                else if (platformId == 1)
                {
                    // Macintosh
                    if (encodingId == 1)
                    {
                        encoding = CharacterFormat.SJIS;
                    }
                    else
                    {
                        encoding = CharacterFormat.ANSI;
                    }
                }
                else if (platformId == 3)
                {
                    // Microsoft
                    if (encodingId == 1)
                    {
                        encoding = CharacterFormat.UCS2;
                    }
                    else if (encodingId == 2)
                    {
                        encoding = CharacterFormat.SJIS;
                    }
                    else
                    {
                        encoding = CharacterFormat.ANSI;
                    }
                }

                coder.move(offset);

                format = coder.readUnsignedShort();
                /* length = */
                coder.readUnsignedShort();
                /* language = */
                coder.readUnsignedShort();

                if (format == 0)
                {
                    decodeSimpleCMAP(coder);
                }
                else if (format == 4)
                {
                    decodeRangeCMAP(coder);
                }
                else
                {
                    throw new IOException();
                }
                coder.reset();
            }
            encoding = CharacterFormat.SJIS;
        }

        /// <summary>
        /// Decode a simple character table.
        /// </summary>
        /// <param name="coder"> a BigDecoder containing data for the table. </param>
        /// <exception cref="IOException"> if an error occurs while decoding the table data. </exception>



        private void decodeSimpleCMAP(BigDecoder coder)
        {
            charToGlyph = new int[256];
            maxChar = (char)255;
            for (int index = 0; index < 256; index++)
            {
                charToGlyph[index] = coder.readByte();
                glyphToChar[charToGlyph[index]] = index;
            }
        }

        /// <summary>
        /// Decode a range (type 4) character table.
        /// </summary>
        /// <param name="coder"> a BigDecoder containing data for the table. </param>
        /// <exception cref="IOException"> if an error occurs while decoding the table data. </exception>



        private void decodeRangeCMAP(BigDecoder coder)
        {


            int segmentCount = coder.readUnsignedShort() / 2;
            coder.readUnsignedShort(); // search range
            coder.readUnsignedShort(); // entry selector
            coder.readUnsignedShort(); // range shift



            int[] startCount = new int[segmentCount];


            int[] endCount = new int[segmentCount];


            int[] delta = new int[segmentCount];


            int[] range = new int[segmentCount];


            int[] rangeAdr = new int[segmentCount];

            for (int index = 0; index < segmentCount; index++)
            {
                endCount[index] = coder.readUnsignedShort();

                if (endCount[index] > maxChar)
                {
                    maxChar = (char)endCount[index];
                }
            }

            charToGlyph = new int[maxChar + 1];

            coder.readUnsignedShort(); // reserved padding

            for (int index = 0; index < segmentCount; index++)
            {
                startCount[index] = coder.readUnsignedShort();
            }

            for (int index = 0; index < segmentCount; index++)
            {
                delta[index] = coder.readShort();
            }

            for (int index = 0; index < segmentCount; index++)
            {
                rangeAdr[index] = coder.mark();
                range[index] = coder.readShort();
                coder.unmark();
            }

            int glyphIndex = 0;
            int location = 0;

            for (int index = 0; index < segmentCount; index++)
            {
                for (int code = startCount[index]; code <= endCount[index]; code++)
                {
                    if (range[index] == 0)
                    {
                        glyphIndex = (delta[index] + code) % Coder.USHORT_MAX;
                    }
                    else
                    {
                        location = rangeAdr[index] + range[index] + ((code - startCount[index]) << 1);
                        coder.move(location);
                        glyphIndex = coder.readUnsignedShort();

                        if (glyphIndex != 0)
                        {
                            glyphIndex = (glyphIndex + delta[index]) % Coder.USHORT_MAX;
                        }
                    }

                    charToGlyph[code] = glyphIndex;
                    glyphToChar[glyphIndex] = code;
                }
            }
        }

        /// <summary>
        /// Decode the LOCA table.
        /// </summary>
        /// <param name="entry"> the bleEntry containing the encoded LOCA table data. </param>
        /// <exception cref="IOException"> if an error occurs while decoding the table data. </exception>



        private void decodeLOCA(TableEntry entry)
        {


            byte[] data = entry.Data;


            MemoryStream stream = new MemoryStream(data);


            BigDecoder coder = new BigDecoder(stream, data.Length);
            offsets = new int[glyphCount];

            if (glyphOffset == ITLF_SHORT)
            {
                offsets[0] = (coder.readUnsignedShort() * 2 << BYTES_TO_BITS);
            }
            else
            {
                offsets[0] = (coder.readInt() << BYTES_TO_BITS);
            }

            for (int i = 1; i < glyphCount; i++)
            {
                if (glyphOffset == ITLF_SHORT)
                {
                    offsets[i] = (coder.readUnsignedShort() * 2 << BYTES_TO_BITS);
                }
                else
                {
                    offsets[i] = (coder.readInt() << BYTES_TO_BITS);
                }

                if (offsets[i] == offsets[i - 1])
                {
                    offsets[i - 1] = 0;
                }
            }
        }

        /// <summary>
        /// Decode the GLYF table.
        /// </summary>
        /// <param name="entry"> the bleEntry containing the encoded GLYF table data. </param>
        /// <exception cref="IOException"> if an error occurs while decoding the table data. </exception>



        private void decodeGlyphs(TableEntry entry)
        {


            byte[] data = entry.Data;


            MemoryStream stream = new MemoryStream(data);


            BigDecoder coder = new BigDecoder(stream, data.Length);
            int numberOfContours = 0;

            for (int i = 0; i < glyphCount; i++)
            {
                coder.skip(offsets[i] >> 3);

                numberOfContours = coder.readShort();

                if (numberOfContours >= 0)
                {
                    decodeSimpleGlyph(coder, i, numberOfContours);
                }
                coder.reset();
            }

            for (int i = 0; i < glyphCount; i++)
            {
                if (offsets[i] != 0)
                {
                    coder.skip(offsets[i] >> 3);

                    if (coder.readShort() == -1)
                    {
                        decodeCompositeGlyph(coder, i);
                    }
                    coder.reset();
                }
            }
        }

        /// <summary>
        /// Decode a simple glyph.
        /// </summary>
        /// <param name="coder"> the decoder containing the encoded glyph data. </param>
        /// <param name="glyphIndex"> the position of the Glyph table to store the glyph. </param>
        /// <param name="numberOfContours"> the number of segments in the glyph outline. </param>
        /// <exception cref="IOException"> if an error occurs reading the glyph data. </exception>



        private void decodeSimpleGlyph(BigDecoder coder, int glyphIndex, int numberOfContours)
        {



            int xMin = coder.readShort() / scale;


            int yMin = coder.readShort() / scale;


            int xMax = coder.readShort() / scale;


            int yMax = coder.readShort() / scale;



            int[] endPtsOfContours = new int[numberOfContours];

            for (int i = 0; i < numberOfContours; i++)
            {
                endPtsOfContours[i] = coder.readUnsignedShort();
            }



            int instructionCount = coder.readUnsignedShort();


            int[] instructions = new int[instructionCount];

            for (int i = 0; i < instructionCount; i++)
            {
                instructions[i] = coder.readByte();
            }



            int numberOfPoints = (numberOfContours == 0) ? 0 : endPtsOfContours[endPtsOfContours.Length - 1] + 1;



            int[] flags = new int[numberOfPoints];


            int[] xCoordinates = new int[numberOfPoints];


            int[] yCoordinates = new int[numberOfPoints];


            bool[] onCurve = new bool[numberOfPoints];

            int repeatCount = 0;
            int repeatFlag = 0;

            for (int i = 0; i < numberOfPoints; i++)
            {
                if (repeatCount > 0)
                {
                    flags[i] = repeatFlag;
                    repeatCount--;
                }
                else
                {
                    flags[i] = coder.readByte();

                    if ((flags[i] & REPEAT_FLAG) > 0)
                    {
                        repeatCount = coder.readByte();
                        repeatFlag = flags[i];
                    }
                }
                onCurve[i] = (flags[i] & ON_CURVE) > 0;
            }

            int last = 0;

            for (int i = 0; i < numberOfPoints; i++)
            {
                if ((flags[i] & X_SHORT) > 0)
                {
                    if ((flags[i] & X_POSITIVE) > 0)
                    {
                        xCoordinates[i] = last + coder.readByte();
                        last = xCoordinates[i];
                    }
                    else
                    {
                        xCoordinates[i] = last - coder.readByte();
                        last = xCoordinates[i];
                    }
                }
                else
                {
                    if ((flags[i] & X_SAME) > 0)
                    {
                        xCoordinates[i] = last;
                    }
                    else
                    {
                        xCoordinates[i] = last + coder.readShort();
                        last = xCoordinates[i];
                    }
                }
            }

            last = 0;

            for (int i = 0; i < numberOfPoints; i++)
            {
                if ((flags[i] & Y_SHORT) > 0)
                {
                    if ((flags[i] & Y_POSITIVE) > 0)
                    {
                        yCoordinates[i] = last + coder.readByte();
                        last = yCoordinates[i];
                    }
                    else
                    {
                        yCoordinates[i] = last - coder.readByte();
                        last = yCoordinates[i];
                    }
                }
                else
                {
                    if ((flags[i] & Y_SAME) > 0)
                    {
                        yCoordinates[i] = last;
                    }
                    else
                    {
                        yCoordinates[i] = last + coder.readShort();
                        last = yCoordinates[i];
                    }
                }
            }

            /*
			 * Convert the coordinates into a shape
			 */


            Canvas path = new Canvas();

            bool contourStart = true;
            bool offPoint = false;

            int contour = 0;

            int xCoord = 0;
            int yCoord = 0;

            int prevX = 0;
            int prevY = 0;

            int initX = 0;
            int initY = 0;

            for (int i = 0; i < numberOfPoints; i++)
            {
                xCoord = xCoordinates[i] / scale;
                yCoord = yCoordinates[i] / scale;

                if (onCurve[i])
                {
                    if (contourStart)
                    {
                        path.moveForFont(xCoord, -yCoord);
                        contourStart = false;
                        initX = xCoord;
                        initY = yCoord;
                    }
                    else if (offPoint)
                    {
                        path.curve(prevX, -prevY, xCoord, -yCoord);
                        offPoint = false;
                    }
                    else
                    {
                        path.line(xCoord, -yCoord);
                    }
                }
                else
                {
                    if (offPoint)
                    {
                        path.curve(prevX, -prevY, (xCoord + prevX) / 2, -(yCoord + prevY) / 2);
                    }

                    prevX = xCoord;
                    prevY = yCoord;
                    offPoint = true;
                }

                if (i == endPtsOfContours[contour])
                {
                    if (offPoint)
                    {
                        path.curve(xCoord, -yCoord, initX, -initY);
                    }
                    else
                    {
                        path.close();
                    }
                    contourStart = true;
                    offPoint = false;
                    prevX = 0;
                    prevY = 0;
                    contour++;
                }
            }

            glyphTable[glyphIndex] = new TrueTypeGlyph(path.Shape, new Bounds(xMin, -yMax, xMax, -yMin), 0);
            glyphTable[glyphIndex].setCoordinates(xCoordinates, yCoordinates);
            glyphTable[glyphIndex].OnCurve = onCurve;
            glyphTable[glyphIndex].Ends = endPtsOfContours;
        }

        /// <summary>
        /// Decode a glyph that contains a series of simple glyphs.
        /// </summary>
        /// <param name="coder"> the decoder containing the encoded glyph data. </param>
        /// <param name="glyphIndex"> the position of the Glyph table to store the glyph. </param>
        /// <exception cref="IOException"> if an error occurs reading the glyph data. </exception>



        private void decodeCompositeGlyph(BigDecoder coder, int glyphIndex)
        {



            Shape shape = new Shape(new List<ShapeRecord>());
            CoordTransform transform = null;



            int xMin = coder.readShort();


            int yMin = coder.readShort();


            int xMax = coder.readShort();


            int yMax = coder.readShort();

            TrueTypeGlyph points = null;

            int numberOfPoints = 0;

            int[] endPtsOfContours = null;
            int[] xCoordinates = null;
            int[] yCoordinates = null;
            bool[] onCurve = null;

            int flags = 0;
            int sourceGlyph = 0;

            int xOffset = 0;
            int yOffset = 0;

            //        int sourceIndex = 0;
            //        int destIndex = 0;

            do
            {
                flags = coder.readUnsignedShort();
                sourceGlyph = coder.readUnsignedShort();

                if ((sourceGlyph >= glyphTable.Length) || (glyphTable[sourceGlyph] == null))
                {
                    glyphTable[glyphIndex] = new TrueTypeGlyph(null, new Bounds(xMin, yMin, xMax, yMax), 0);
                    return;
                }

                points = glyphTable[sourceGlyph];
                numberOfPoints = points.numberOfPoints();

                endPtsOfContours = new int[points.numberOfContours()];
                points.getEnd(endPtsOfContours);

                xCoordinates = new int[numberOfPoints];
                points.getXCoordinates(xCoordinates);

                yCoordinates = new int[numberOfPoints];
                points.getYCoordinates(yCoordinates);

                onCurve = new bool[numberOfPoints];
                points.getCurve(onCurve);

                if (((flags & ARGS_ARE_WORDS) == 0) && ((flags & ARGS_ARE_XY) == 0))
                {
                    /* destIndex = */
                    coder.readByte();
                    /* sourceIndex = */
                    coder.readByte();

                    //xCoordinates[destIndex] =
                    //glyphTable[sourceGlyph].xCoordinates[sourceIndex];
                    //yCoordinates[destIndex] =
                    //glyphTable[sourceGlyph].yCoordinates[sourceIndex];
                    transform = CoordTransform.translate(0, 0);
                }
                else if (((flags & ARGS_ARE_WORDS) == 0) && ((flags & ARGS_ARE_XY) > 0))
                {
                    xOffset = (coder.readByte() << SIGN_EXTEND) >> SIGN_EXTEND;
                    yOffset = (coder.readByte() << SIGN_EXTEND) >> SIGN_EXTEND;
                    transform = CoordTransform.translate(xOffset, yOffset);
                }
                else if (((flags & ARGS_ARE_WORDS) > 0) && ((flags & ARGS_ARE_XY) == 0))
                {
                    /* destIndex = */
                    coder.readUnsignedShort();
                    /* sourceIndex = */
                    coder.readUnsignedShort();

                    //xCoordinates[destIndex] =
                    //glyphTable[sourceGlyph].xCoordinates[sourceIndex];
                    //yCoordinates[destIndex] =
                    //glyphTable[sourceGlyph].yCoordinates[sourceIndex];
                    transform = CoordTransform.translate(0, 0);
                }
                else
                {
                    xOffset = coder.readShort();
                    yOffset = coder.readShort();
                    transform = CoordTransform.translate(xOffset, yOffset);
                }

                if ((flags & HAVE_SCALE) > 0)
                {


                    float scaleXY = coder.readShort() / Coder.SCALE_14;
                    transform = new CoordTransform(scaleXY, scaleXY, 0, 0, xOffset, yOffset);
                }
                else if ((flags & HAVE_XYSCALE) > 0)
                {


                    float scaleX = coder.readShort() / Coder.SCALE_14;


                    float scaleY = coder.readShort() / Coder.SCALE_14;
                    transform = new CoordTransform(scaleX, scaleY, 0, 0, xOffset, yOffset);
                }
                else if ((flags & HAVE_2X2) > 0)
                {


                    float scaleX = coder.readShort() / Coder.SCALE_14;


                    float scale01 = coder.readShort() / Coder.SCALE_14;


                    float scale10 = coder.readShort() / Coder.SCALE_14;


                    float scaleY = coder.readShort() / Coder.SCALE_14;

                    transform = new CoordTransform(scaleX, scaleY, scale01, scale10, xOffset, yOffset);
                }



                float[][] matrix = transform.Matrix;
                float[][] result;

                for (int i = 0; i < numberOfPoints; i++)
                {
                    result = CoordTransform.product(matrix, CoordTransform.translate(xCoordinates[i], yCoordinates[i]).Matrix);

                    xCoordinates[i] = (int)result[0][2];
                    yCoordinates[i] = (int)result[1][2];
                }



                Canvas path = new Canvas();

                bool contourStart = true;
                bool offPoint = false;

                int contour = 0;

                int xCoord = 0;
                int yCoord = 0;

                int prevX = 0;
                int prevY = 0;

                int initX = 0;
                int initY = 0;

                for (int i = 0; i < numberOfPoints; i++)
                {
                    xCoord = xCoordinates[i] / scale;
                    yCoord = yCoordinates[i] / scale;

                    if (onCurve[i])
                    {
                        if (contourStart)
                        {
                            path.moveForFont(xCoord, -yCoord);
                            contourStart = false;
                            initX = xCoord;
                            initY = yCoord;
                        }
                        else if (offPoint)
                        {
                            path.curve(prevX, -prevY, xCoord, -yCoord);
                            offPoint = false;
                        }
                        else
                        {
                            path.line(xCoord, -yCoord);
                        }
                    }
                    else
                    {
                        if (offPoint)
                        {
                            path.curve(prevX, -prevY, (xCoord + prevX) / 2, -(yCoord + prevY) / 2);
                        }

                        prevX = xCoord;
                        prevY = yCoord;
                        offPoint = true;
                    }

                    if (i == endPtsOfContours[contour])
                    {
                        if (offPoint)
                        {
                            path.curve(xCoord, -yCoord, initX, -initY);
                        }
                        else
                        {
                            path.close();
                        }
                        contourStart = true;
                        offPoint = false;
                        prevX = 0;
                        prevY = 0;
                        contour++;
                    }
                }
                ((List<ShapeRecord>)shape.Objects).AddRange(path.Shape.Objects);

            } while ((flags & HAS_MORE) > 0);

            glyphTable[glyphIndex] = new TrueTypeGlyph(shape, new Bounds(xMin, yMin, xMax, yMax), 0);

            glyphTable[glyphIndex].setCoordinates(xCoordinates, yCoordinates);
            glyphTable[glyphIndex].OnCurve = onCurve;
            glyphTable[glyphIndex].Ends = endPtsOfContours;
        }
    }

}