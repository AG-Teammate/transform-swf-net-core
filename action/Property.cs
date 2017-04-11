using System;
using com.flagstone.transform.coder;

/*
 * Property.java
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
namespace com.flagstone.transform.action
{
	/// <summary>
	/// Property defines the set of attributes that can accessed for movies and movie
	/// clips when executing actions.
	/// </summary>
	/// <seealso cref= Push </seealso>
	public sealed class Property
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Property: { value=%d}";
		/// <summary>
		/// The first version of Flash that uses integers values for properties. </summary>
		public const int VERSION_WITH_INTS = 5;

		/// <summary>
		/// Value used to identify an X Property when it is encoded. </summary>
		private const int XCOORD_VAL = 0;
		/// <summary>
		/// Value used to identify an Y Property when it is encoded. </summary>
		private const int YCOORD_VAL = 1;
		/// <summary>
		/// Value used to identify an XSCALE Property when it is encoded. </summary>
		private const int XSCALE_VAL = 2;
		/// <summary>
		/// Value used to identify a YSCALE Property when it is encoded. </summary>
		private const int YSCALE_VAL = 3;
		/// <summary>
		/// Value used to identify a CURRENT_FRAME Property when it is encoded. </summary>
		private const int CUR_FRAME_VAL = 4;
		/// <summary>
		/// Value used to identify a TOTAL_FRAMES Property when it is encoded. </summary>
		private const int TOT_FRAMES_VAL = 5;
		/// <summary>
		/// Value used to identify a ALPHA Property when it is encoded. </summary>
		private const int ALPHA_VAL = 6;
		/// <summary>
		/// Value used to identify a VISIBLE Property when it is encoded. </summary>
		private const int VISIBLE_VAL = 7;
		/// <summary>
		/// Value used to identify a WIDTH Property when it is encoded. </summary>
		private const int WIDTH_VAL = 8;
		/// <summary>
		/// Value used to identify a HEIGHT Property when it is encoded. </summary>
		private const int HEIGHT_VAL = 9;
		/// <summary>
		/// Value used to identify a ROTATION Property when it is encoded. </summary>
		private const int ROTATION_VAL = 10;
		/// <summary>
		/// Value used to identify a TARGET Property when it is encoded. </summary>
		private const int TARGET_VAL = 11;
		/// <summary>
		/// Value used to identify a FRAMES_LOADED Property when it is encoded. </summary>
		private const int LOADED_VAL = 12;
		/// <summary>
		/// Value used to identify a NAME Property when it is encoded. </summary>
		private const int NAME_VAL = 13;
		/// <summary>
		/// Value used to identify a DROP_TARGET Property when it is encoded. </summary>
		private const int DROP_TARGET_VAL = 14;
		/// <summary>
		/// Value used to identify a URL Property when it is encoded. </summary>
		private const int URL_VAL = 15;
		/// <summary>
		/// Value used to identify a HIGH_QUALITY Property when it is encoded. </summary>
		private const int HI_QUALITY_VAL = 16;
		/// <summary>
		/// Value used to identify a FOCUS_RECT Property when it is encoded. </summary>
		private const int FOCUS_RECT_VAL = 17;
		/// <summary>
		/// Value used to identify a SOUND_BUF_TIME Property when it is encoded. </summary>
		private const int SOUND_TIME_VAL = 18;
		/// <summary>
		/// Value used to identify a QUALITY Property when it is encoded. </summary>
		private const int QUALITY_VAL = 19;
		/// <summary>
		/// Value used to identify a XMOUSE Property when it is encoded. </summary>
		private const int XMOUSE_VAL = 20;
		/// <summary>
		/// Value used to identify YMOUSE X Property when it is encoded. </summary>
		private const int YMOUSE_VAL = 21;

		/// <summary>
		/// The x-origin of the movie clip relative to the parent clip. This is
		/// equivalent to the _x property in actionscript.
		/// </summary>
		public static readonly Property XCOORD = new Property(XCOORD_VAL);
		/// <summary>
		/// The y-origin of the movie clip relative to the parent clip. This is
		/// equivalent to the _y property in actionscript.
		/// </summary>
		public static readonly Property YCOORD = new Property(YCOORD_VAL);
		/// <summary>
		/// The scaling factor of the movie clip in the x direction. This is
		/// equivalent to the _xscale property in actionscript.
		/// </summary>
		public static readonly Property XSCALE = new Property(XSCALE_VAL);
		/// <summary>
		/// The scaling factor of the movie clip in the x direction. This is
		/// equivalent to the _yscale property in actionscript.
		/// </summary>
		public static readonly Property YSCALE = new Property(YSCALE_VAL);
		/// <summary>
		/// The number of the current frame playing in the movie clip. This is
		/// equivalent to the _currentframe property in actionscript.
		/// </summary>
		public static readonly Property CURRENT_FRAME = new Property(CUR_FRAME_VAL);
		/// <summary>
		/// The total number of frames in the movie clip. This is equivalent to the
		/// _totalframes property in actionscript.
		/// </summary>
		public static readonly Property TOTAL_FRAMES = new Property(TOT_FRAMES_VAL);
		/// <summary>
		/// The transparency of the movie clip. This is equivalent to the _alpha
		/// property in actionscript.
		/// </summary>
		public static readonly Property ALPHA = new Property(ALPHA_VAL);
		/// <summary>
		/// Whether the movie clip is visible. This is equivalent to the _visible
		/// property in actionscript.
		/// </summary>
		public static readonly Property VISIBLE = new Property(VISIBLE_VAL);
		/// <summary>
		/// The width of the movie clip in pixels. This is equivalent to the _width
		/// property in actionscript.
		/// </summary>
		public static readonly Property WIDTH = new Property(WIDTH_VAL);
		/// <summary>
		/// The height of the movie clip in pixels. This is equivalent to the _height
		/// property in actionscript.
		/// </summary>
		public static readonly Property HEIGHT = new Property(HEIGHT_VAL);
		/// <summary>
		/// The angle of rotation of the movie clip in degrees. This is equivalent to
		/// the _height property in actionscript.
		/// </summary>
		public static readonly Property ROTATION = new Property(ROTATION_VAL);
		/// <summary>
		/// The path of the movie clip relative to the root movie in the Player. This
		/// is equivalent to the _rotation property in actionscript.
		/// </summary>
		public static readonly Property TARGET = new Property(TARGET_VAL);
		/// <summary>
		/// The number of frames form the movie clip loaded. This is equivalent to
		/// the _framesloaded property in actionscript.
		/// </summary>
		public static readonly Property FRAMES_LOADED = new Property(LOADED_VAL);
		/// <summary>
		/// The name of movie clip. This is equivalent to the _name property in
		/// actionscript.
		/// </summary>
		public static readonly Property NAME = new Property(NAME_VAL);
		/// <summary>
		/// The name of the movie clip currently being dragged. This is equivalent to
		/// the _target property in actionscript.
		/// </summary>
		public static readonly Property DROP_TARGET = new Property(DROP_TARGET_VAL);
		/// <summary>
		/// The URL from which the movie clip was loaded. This is equivalent to the
		/// _url property in actionscript.
		/// </summary>
		public static readonly Property URL = new Property(URL_VAL);
		/// <summary>
		/// Identifies the level of aliasing being performed by the Player. This is
		/// equivalent to the _highquality property in actionscript.
		/// </summary>
		public static readonly Property HIGH_QUALITY = new Property(HI_QUALITY_VAL);
		/// <summary>
		/// Identifies whether a rectangle is drawn around a button or text field
		/// that has the current focus This is equivalent to the _focusrect property
		/// in actionscript. .
		/// </summary>
		public static readonly Property FOCUS_RECT = new Property(FOCUS_RECT_VAL);
		/// <summary>
		/// The amount of time streaming sound is buffered by the Player before
		/// playing. This is equivalent to the _soundbuftime property in
		/// actionscript.
		/// </summary>
		public static readonly Property SOUND_BUF_TIME = new Property(SOUND_TIME_VAL);
		/// <summary>
		/// Identifies the level of rendering quality being performed by the Player.
		/// This is equivalent to the _quality property in actionscript.
		/// </summary>
		public static readonly Property QUALITY = new Property(QUALITY_VAL);
		/// <summary>
		/// The current x-coordinate of the mouse pointer on the Player screen. This
		/// is equivalent to the _xmouse property in actionscript.
		/// </summary>
		public static readonly Property XMOUSE = new Property(XMOUSE_VAL);
		/// <summary>
		/// The current y-coordinate of the mouse pointer on the Player screen. This
		/// is equivalent to the _ymouse property in actionscript.
		/// </summary>
		public static readonly Property YMOUSE = new Property(YMOUSE_VAL);

		/// <summary>
		/// Encoded Property value. </summary>
		
		private readonly int value;

		/// <summary>
		/// Creates a Property object with the specified value.
		/// </summary>
		/// <param name="pval">
		///            the value for the property. </param>


		public Property(int pval)
		{
			value = pval;
		}

		/// <summary>
		/// Get value used to encode the Property.
		/// </summary>
		/// <returns> the encoded property value. </returns>
		public int Value => value;

	    /// <summary>
		/// Returns value of the Property as it would be written to a Flash file.
		/// For Flash version 4 and earlier Properties were encoded as floating
		/// point values while for Flash 5 and later they are encoded as integers.
		/// </summary>
		/// <param name="version"> the Flash version that the property will be encoded for.
		/// </param>
		/// <returns> the value the Property will be encoded as. </returns>


		public int getValue(int version)
		{
			int val;
			if (version < VERSION_WITH_INTS)
			{
				val = Float.floatToIntBits(value);
			}
			else
			{
				val = value;
			}
			return val;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, value);
		}
	}

}