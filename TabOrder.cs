/*
 * TabOrder.java
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
	/// The TabOrder class is used to set the tabbing order of text fields, movie
	/// clips and buttons visible on the display list.
	/// 
	/// <para>
	/// The objects are referenced by the number of the layer on which they displayed
	/// rather than the unique identifier. This differs from the other classes in the
	/// framework but it does allow objects creating at run-time by ActionScript
	/// statements to be referenced.
	/// </para>
	/// </summary>
	public sealed class TabOrder : MovieTag
	{

		/// <summary>
		/// The highest index when defining an objects tab order. </summary>
		public const int MAX_TAB = 65535;

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "TabOrder: { layer=%d; index=%d}";
		/// <summary>
		/// The layer on which the object is displayed. </summary>
		private int layer;
		/// <summary>
		/// The order in which the object will received keyboard focus. </summary>
		private int index;

		/// <summary>
		/// Creates and initialises a TabOrder object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public TabOrder(SWFDecoder coder)
		{
			if ((coder.readUnsignedShort() & Coder.LENGTH_FIELD) == Coder.IS_EXTENDED)
			{
				coder.readInt();
			}
			layer = coder.readUnsignedShort();
			index = coder.readUnsignedShort();
		}

		/// <summary>
		/// Construct a TabOrder object that set the tab order for the object on the
		/// display list at the specified layer.
		/// </summary>
		/// <param name="level">
		///            the layer number which contains the object assigned to the
		///            tabbing order. Must be in the range 1..65535. </param>
		/// <param name="idx">
		///            the index of the object in the tabbing order. Must be in the
		///            range 0..65535. </param>


		public TabOrder(int level, int idx)
		{
			Layer = level;
			Index = idx;
		}

		/// <summary>
		/// Creates and initialises a TabOrder object using the values copied
		/// from another TabOrder object.
		/// </summary>
		/// <param name="object">
		///            a TabOrder object from which the values will be
		///            copied. </param>


		public TabOrder(TabOrder @object)
		{
			layer = @object.layer;
			index = @object.index;
		}

		/// <summary>
		/// Get the layer number which contains the object assigned to the
		/// tabbing order. </summary>
		/// <returns> the layer number. </returns>
		public int Layer
		{
			get => layer;
		    set
			{
				if ((value < 1) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, value);
				}
				layer = value;
			}
		}


		/// <summary>
		/// Get the index of the object in the tabbing order. </summary>
		/// <returns> the order in which the object will get keyboard focus. </returns>
		public int Index
		{
			get => index;
		    set
			{
				if ((value < 0) || (value > MAX_TAB))
				{
					throw new IllegalArgumentRangeException(0, MAX_TAB, value);
				}
				index = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public TabOrder copy()
		{
			return new TabOrder(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, layer, index);
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
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			coder.writeShort((MovieTypes.TAB_ORDER << Coder.LENGTH_FIELD_SIZE) | 4);
			coder.writeShort(layer);
			coder.writeShort(index);
		}
	}

}