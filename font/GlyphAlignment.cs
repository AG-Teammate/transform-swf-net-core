using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;

/*
 * GlyphAlignment.java
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

namespace com.flagstone.transform.font
{
    /// <summary>
	/// GlyphAlignment holds the alignment zones for a font glyph.
	/// </summary>
	public sealed class GlyphAlignment : SWFEncodeable
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "GlyphAlignment: { alignments=%s;" + " alignX=%s; alignY=%s}";

		/// <summary>
		/// Alignments for a glyph. </summary>
		private IList<AlignmentZone> alignments;
		/// <summary>
		/// Flags describing alignments. </summary>
		
		private int masks;

		/// <summary>
		/// Creates and initialises a GlyphAlignment object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public GlyphAlignment(SWFDecoder coder)
		{


			int count = coder.readByte();

			alignments = new List<AlignmentZone>(count);

			for (int i = 0; i < count; i++)
			{
				alignments.Add(new AlignmentZone(coder));
			}
			masks = coder.readByte();
		}

		/// <summary>
		/// Creates a new GlyphAlignment with the alignment zones. </summary>
		/// <param name="list"> the list of alignment zones for the glyph. </param>
		/// <param name="xAlign"> whether there is a horizontal alignment zone. </param>
		/// <param name="yAlign"> whether there is a vertical alignment zone. </param>


		public GlyphAlignment(IList<AlignmentZone> list, bool xAlign, bool yAlign)
		{
			Alignments = list;
			AlignmentX = xAlign;
			AlignmentY = yAlign;
		}

		/// <summary>
		/// Creates and initialises a GlyphAlignment object using the values copied
		/// from another GlyphAlignment object.
		/// </summary>
		/// <param name="object">
		///            a GlyphAlignment object from which the values will be
		///            copied. </param>


		public GlyphAlignment(GlyphAlignment @object)
		{
			alignments = new List<AlignmentZone>(@object.alignments);
			masks = @object.masks;
		}

		/// <summary>
		/// Does the list of alignment zones contain a horizontal alignment. </summary>
		/// <returns> true if the list contains the alignment for the width of a glyph. </returns>
		public bool alignmentX()
		{
			return (masks & 0x01) != 0;
		}

		/// <summary>
		/// Indicates that the list of alignment zones contain a horizontal
		/// alignment. </summary>
		/// <param name="hasAlign"> true if the list contains the alignment for the width
		/// of a glyph, false otherwise. </param>


		public bool AlignmentX
		{
			set
			{
				masks &= ~Coder.BIT0;
				if (value)
				{
					masks |= Coder.BIT0;
				}
			}
		}

		/// <summary>
		/// Does the list of alignment zones contain a vertical alignment. </summary>
		/// <returns> true if the list contains the alignment for the height of
		/// a glyph. </returns>
		public bool alignmentY()
		{
			return (masks & Coder.BIT1) != 0;
		}

		/// <summary>
		/// Indicates that the list of alignment zones contain a vertical
		/// alignment. </summary>
		/// <param name="hasAlign"> true if the list contains the alignment for the height
		/// of a glyph, false otherwise. </param>


		public bool AlignmentY
		{
			set
			{
				masks &= ~Coder.BIT1;
				if (value)
				{
					masks |= Coder.BIT1;
				}
			}
		}

		/// <summary>
		/// Get the list of alignment zones. </summary>
		/// <returns> a list of AlignmentZones describing the alignment areas for a
		/// glyph. </returns>
		public IList<AlignmentZone> Alignments
		{
			get => alignments;
		    set => alignments = value;
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public GlyphAlignment copy()
		{
			return new GlyphAlignment(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, alignments, alignmentX().ToString(), alignmentY().ToString());
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 10;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(2);

			foreach (AlignmentZone zone in alignments)
			{
				zone.encode(coder, context);
			}
			coder.writeByte(masks);
		}
	}

}