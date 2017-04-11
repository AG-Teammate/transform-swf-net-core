﻿using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * TextSettings.java
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

namespace com.flagstone.transform.text
{
    /// <summary>
	/// TextSettings allows you to control how individual text fields are rendered.
	/// 
	/// <para>
	/// There are four parameters that control how the text is rendered:
	/// </para>
	/// <ol>
	/// <li>Advanced Rendering - whether the text is rendered using the advanced
	/// anti-aliasing engine added in Flash 8.</li>
	/// <li>Grid Alignment - how letters are aligned with respect to the pixel grid
	/// used in LCD monitors.</li>
	/// <li>Thickness - a parameter used to control the thickness of the line when
	/// anti-aliasing is used.</li>
	/// <li>Sharpness - a parameter used to control the sharpness of the line when
	/// anti-aliasing is used.</li>
	/// </ol>
	/// <para>
	/// The thickness and sharpness control the how the text is rendered:
	/// 
	/// <pre>
	///    outsideCutoff = (0.5 * sharpness - thickness) * fontSize
	///    insideCutoff = (-0.5 * sharpness - thickness) * fontSize
	/// </pre>
	/// 
	/// Note that Adobe reports the results can be poor when the text is scaled by a
	/// significant amount and so the default values of 0.0 should be used for the
	/// thickness and sharpness values.
	/// </para>
	/// </summary>
	public sealed class TextSettings : MovieTag
	{
		/// <summary>
		/// Grid specifies how letters are aligned with respect to the pixel grid on
		/// a screen.
		/// </summary>
		public enum Grid
		{
			/// <summary>
			/// Do not use grid fitting. </summary>
			NONE,
			/// <summary>
			/// Align letters on pixel boundaries. </summary>
			PIXEL,
			/// <summary>
			/// Align letters on 1/3 pixel boundaries. </summary>
			SUBPIXEL
		}

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "TextSettings: { identifier=%d;" + " useAdvanced=%s; grid=%s; thickness=%f; sharpness=%f}";

		/// <summary>
		/// The unique identifier of the text field. </summary>
		
		private int identifier;
		/// <summary>
		/// Compound code for the rendering settings. </summary>
		
		private int rendering;
		/// <summary>
		/// Control for the thickness of the line. </summary>
		
		private int thickness;
		/// <summary>
		/// Control for the sharpness of the line. </summary>
		
		private int sharpness;

		/// <summary>
		/// Creates and initialises an TextSettings using values encoded in the Flash
		/// binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public TextSettings(SWFDecoder coder)
		{
			if ((coder.readUnsignedShort() & Coder.LENGTH_FIELD) == Coder.IS_EXTENDED)
			{
				coder.readInt();
			}
			identifier = coder.readUnsignedShort();
			rendering = coder.readByte();
			thickness = coder.readInt();
			sharpness = coder.readInt();
			coder.readByte();
		}

		/// <summary>
		/// Creates a TextSettings object with the specified values.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier of an existing text field. </param>
		/// <param name="advanced">
		///            whether the advanced rendering engine will be used to display
		///            the text. </param>
		/// <param name="grid">
		///            how letters are aligned with respect to the pixel grid. </param>
		/// <param name="thick">
		///            the thickness used when anti-aliasing the text. </param>
		/// <param name="sharp">
		///            the sharpness used when anti-aliasing the text. </param>


		public TextSettings(int uid, bool advanced, Grid grid, float thick, float sharp)
		{
			Identifier = uid;
			useAdvanced(advanced);
			setGrid(grid);
			Thickness = thick;
			Sharpness = sharp;
		}

		/// <summary>
		/// Creates an TextSettings object and initialised it by copying the values
		/// from an existing one.
		/// </summary>
		/// <param name="object">
		///            a TextSettings object. </param>


		public TextSettings(TextSettings @object)
		{
			identifier = @object.identifier;
			rendering = @object.rendering;
			thickness = @object.thickness;
			sharpness = @object.sharpness;
		}

		/// <summary>
		/// Get the unique identifier of the text definition that this object
		/// applies to.
		/// </summary>
		/// <returns> the unique identifier of the text object. </returns>
		public int Identifier
		{
			get => identifier;
		    set
			{
				if ((value < 1) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, value);
				}
				identifier = value;
			}
		}


		/// <summary>
		/// Will the advanced text rendering engine, introduced in Flash 8
		/// be used.
		/// </summary>
		/// <returns> true if advanced text rendering is used, false if the standard
		/// rendering engine is used. </returns>
		public bool useAdvanced()
		{
			return (rendering & Coder.BIT6) != 0;
		}

		/// <summary>
		/// Sets whether the advanced text rendering engine (true) or standard engine
		/// (false) will be used to render the text.
		/// </summary>
		/// <param name="flag"> set true to select the advanced text rendering engine, false
		/// for the standard rendering engine. </param>


		public void useAdvanced(bool flag)
		{
			rendering |= Coder.BIT6;
		}

		/// <summary>
		/// Returns the alignment of letters with respect to the pixel grid.
		/// </summary>
		/// <returns> the alignment, either NONE, PIXEL or SUBPIXEL. </returns>
		public Grid getGrid()
		{
			Grid alignment;

			if ((rendering & Coder.BIT4) > 0)
			{
				alignment = Grid.SUBPIXEL;
			}
			else if ((rendering & Coder.BIT3) > 0)
			{
				alignment = Grid.PIXEL;
			}
			else
			{
				alignment = Grid.NONE;
			}
			return alignment;
		}

		/// <summary>
		/// Selects how the text letters will be aligned with respect to the pixel
		/// grid used in LCD screens.
		/// </summary>
		/// <param name="alignment">
		///            the alignment with respect to the pixel grid, either NONE,
		///            PIXEL or SUBPIXEL. </param>


		public void setGrid(Grid alignment)
		{

			rendering &= ~(Coder.BIT3 | Coder.BIT4 | Coder.BIT5 | Coder.BIT6);

			switch (alignment)
			{
			case Grid.PIXEL:
				rendering |= Coder.BIT3;
				break;
			case Grid.SUBPIXEL:
				rendering |= Coder.BIT4;
				break;
			default:
				break;
			}
		}

		/// <summary>
		/// Get the value used to control the thickness of a line when rendered.
		/// May be set to 0.0 if the default anti-aliasing value will be used.
		/// </summary>
		/// <returns> the adjustment applied to the line thickness. </returns>
		public float Thickness
		{
			get => Float.intBitsToFloat(thickness);
		    set => thickness = Float.floatToIntBits(value);
		}


		/// <summary>
		/// Get the value used to control the sharpness of a line when rendered.
		/// May be set to 0.0 if the default anti-aliasing value will be used.
		/// </summary>
		/// <returns> the adjustment applied to the line sharpness. </returns>
		public float Sharpness
		{
			get => Float.intBitsToFloat(sharpness);
		    set => sharpness = Float.floatToIntBits(value);
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public TextSettings copy()
		{
			return new TextSettings(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, useAdvanced().ToString(), getGrid(), thickness / Coder.SCALE_16, sharpness / Coder.SCALE_16);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			return 14;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 2 LINES
			coder.writeShort((MovieTypes.TEXT_SETTINGS << Coder.LENGTH_FIELD_SIZE) | 12);
			coder.writeShort(identifier);
			coder.writeByte(rendering);
			coder.writeInt(thickness);
			coder.writeInt(sharpness);
			coder.writeByte(0);
		}
	}

}