using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * Kerning.java
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

namespace com.flagstone.transform.font
{
    /// <summary>
	/// Kerning is used to fine-tune the spacing between specific pairs of characters
	/// to make them visually more appealing.
	/// 
	/// <para>
	/// The glyphs are identified by an index into the glyph table for the font. The
	/// adjustment, in twips, is specified relative to the advance define for the
	/// left hand glyph.
	/// </para>
	/// 
	/// <para>
	/// Kerning objects are only used within DefineFont2 objects and provide more
	/// precise control over the layout of a font's glyph than was possible using the
	/// DefineFont and FontInfo objects.
	/// </para>
	/// </summary>
	/// <seealso cref= DefineFont2 </seealso>
	public sealed class Kerning : SWFEncodeable
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Kerning: { leftGlyph=%d;" + " rightGlyph=%d; adjustment=%d}";

		/// <summary>
		/// The index of the left glyph in the font definition. </summary>
		
		private readonly int leftGlyph;
		/// <summary>
		/// The index of the right glyph in the font definition. </summary>
		
		private readonly int rightGlyph;
		/// <summary>
		/// The adjustment to the advance of the left glyph. </summary>
		
		private readonly int adjustment;

		/// <summary>
		/// Creates and initialises a Kerning object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <param name="context">
		///            a Context object used to manage the decoders for different
		///            type of object and to pass information on how objects are
		///            decoded.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public Kerning(SWFDecoder coder, Context context)
		{
			if (context.contains(Context.WIDE_CODES))
			{
				leftGlyph = coder.readSignedShort();
				rightGlyph = coder.readSignedShort();
			}
			else
			{
				leftGlyph = coder.readByte();
				rightGlyph = coder.readByte();
			}
			adjustment = coder.readSignedShort();
		}

		/// <summary>
		/// Creates a Kerning object specifying the glyph indexes and adjustment. The
		/// value for the adjustment must be specified in twips.
		/// </summary>
		/// <param name="leftIndex">
		///            the index in a code table for the glyph on the left side of
		///            the pair. Must be in the range 0..65535. </param>
		/// <param name="rightIndex">
		///            the index in a code table for the glyph on the right side of
		///            the pair. Must be in the range 0..65535. </param>
		/// <param name="adjust">
		///            the adjustment that will be added to the advance defined for
		///            the left glyph. Must be in the range -32768..32767. </param>


		public Kerning(int leftIndex, int rightIndex, int adjust)
		{
			if ((leftIndex < 0) || (leftIndex >= Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX - 1, leftIndex);
			}
			leftGlyph = leftIndex;

			if ((rightIndex < 0) || (rightIndex >= Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX - 1, rightIndex);
			}
			rightGlyph = rightIndex;

			if ((adjust < Coder.SHORT_MIN) || (adjust > Coder.SHORT_MAX))
			{
				throw new IllegalArgumentRangeException(Coder.SHORT_MIN, Coder.SHORT_MAX, adjust);
			}
			adjustment = adjust;
		}

		/// <summary>
		/// Get the index of the left glyph in the kerning pair.
		/// </summary>
		/// <returns> the index of the left glyph. </returns>
		public int LeftGlyph => leftGlyph;

	    /// <summary>
		/// Get the index of the right glyph in the kerning pair.
		/// </summary>
		/// <returns> the index of the right glyph. </returns>
		public int RightGlyph => rightGlyph;

	    /// <summary>
		/// Get the adjustment, in twips, to the advance of the left glyph.
		/// </summary>
		/// <returns> the adjustment to the glyph spacing. </returns>
		public int Adjustment => adjustment;

	    public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, leftGlyph, rightGlyph, adjustment);
		}



		public override bool Equals(object @object)
		{
			bool result;
			Kerning kerning;

			if (@object == null)
			{
				result = false;
			}
			else if (@object == this)
			{
				result = true;
			}
			else if (@object is Kerning)
			{
				kerning = (Kerning) @object;
				result = (leftGlyph == kerning.leftGlyph) && (rightGlyph == kerning.rightGlyph) && (adjustment == kerning.adjustment);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return ((leftGlyph * Constants.PRIME) + rightGlyph) * Constants.PRIME + adjustment;
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			return context.contains(Context.WIDE_CODES) ? 6 : 4;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			if (context.contains(Context.WIDE_CODES))
			{
				coder.writeShort(leftGlyph);
				coder.writeShort(rightGlyph);
			}
			else
			{
				coder.writeByte(leftGlyph);
				coder.writeByte(rightGlyph);
			}
			coder.writeShort(adjustment);
		}
	}

}