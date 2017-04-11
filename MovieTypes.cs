/*
 * MovieTypes.java
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

namespace com.flagstone.transform
{
	/// <summary>
	/// MovieTypes defines the constants that identify a MovieTag when it is encoded
	/// according to the Flash file format specification.
	/// </summary>


	public sealed class MovieTypes
	{
		/// <summary>
		/// Marker for the end of a Movie. </summary>
		public const int END = 0;
		/// <summary>
		/// Identifies ShowFrame objects when they are encoded. </summary>
		public const int SHOW_FRAME = 1;
		/// <summary>
		/// Identifies DefineShape objects when they are encoded. </summary>
		public const int DEFINE_SHAPE = 2;
		/// <summary>
		/// Identifies Free objects when they are encoded. </summary>
		public const int FREE = 3;
		/// <summary>
		/// Identifies Place objects when they are encoded. </summary>
		public const int PLACE = 4;
		/// <summary>
		/// Identifies Remove objects when they are encoded. </summary>
		public const int REMOVE = 5;
		/// <summary>
		/// Identifies DefineJPEGImage objects when they are encoded. </summary>
		public const int DEFINE_JPEG_IMAGE = 6;
		/// <summary>
		/// Identifies DefineButton objects when they are encoded. </summary>
		public const int DEFINE_BUTTON = 7;
		/// <summary>
		/// Identifies JPEGTables objects when they are encoded. </summary>
		public const int JPEG_TABLES = 8;
		/// <summary>
		/// Identifies SetBackgroundColor objects when they are encoded. </summary>
		public const int SET_BACKGROUND_COLOR = 9;
		/// <summary>
		/// Identifies DefineFont objects when they are encoded. </summary>
		public const int DEFINE_FONT = 10;
		/// <summary>
		/// Identifies DefineText objects when they are encoded. </summary>
		public const int DEFINE_TEXT = 11;
		/// <summary>
		/// Identifies DoAction objects when they are encoded. </summary>
		public const int DO_ACTION = 12;
		/// <summary>
		/// Identifies FontInfo objects when they are encoded. </summary>
		public const int FONT_INFO = 13;
		/// <summary>
		/// Identifies DefineSound objects when they are encoded. </summary>
		public const int DEFINE_SOUND = 14;
		/// <summary>
		/// Identifies StartSound objects when they are encoded. </summary>
		public const int START_SOUND = 15;
		/// <summary>
		/// Identifies ButtonSound objects when they are encoded. </summary>
		public const int BUTTON_SOUND = 17;
		/// <summary>
		/// Identifies SoundStreamHead objects when they are encoded. </summary>
		public const int SOUND_STREAM_HEAD = 18;
		/// <summary>
		/// Identifies SoundStreamBlock objects when they are encoded. </summary>
		public const int SOUND_STREAM_BLOCK = 19;
		/// <summary>
		/// Identifies DefineImage objects when they are encoded. </summary>
		public const int DEFINE_IMAGE = 20;
		/// <summary>
		/// Identifies DefineJPEGImage2 objects when they are encoded. </summary>
		public const int DEFINE_JPEG_IMAGE_2 = 21;
		/// <summary>
		/// Identifies DefineShape2 objects when they are encoded. </summary>
		public const int DEFINE_SHAPE_2 = 22;
		/// <summary>
		/// Identifies ButtonColorTransform objects when they are encoded. </summary>
		public const int BUTTON_COLOR_TRANSFORM = 23;
		/// <summary>
		/// Identifies Protect objects when they are encoded. </summary>
		public const int PROTECT = 24;
		/// <summary>
		/// Identifies PathsArePostscript objects when they are encoded. </summary>
		public const int PATHS_ARE_POSTSCRIPT = 25;
		/// <summary>
		/// Identifies Place2 objects when they are encoded. </summary>
		public const int PLACE_2 = 26;
		/// <summary>
		/// Identifies Remove2 objects when they are encoded. </summary>
		public const int REMOVE_2 = 28;
		/// <summary>
		/// Identifies DefineShape3 objects when they are encoded. </summary>
		public const int DEFINE_SHAPE_3 = 32;
		/// <summary>
		/// Identifies DefineText2 objects when they are encoded. </summary>
		public const int DEFINE_TEXT_2 = 33;
		/// <summary>
		/// Identifies DefineButton2 objects when they are encoded. </summary>
		public const int DEFINE_BUTTON_2 = 34;
		/// <summary>
		/// Identifies DefineJPEGImage3 objects when they are encoded. </summary>
		public const int DEFINE_JPEG_IMAGE_3 = 35;
		/// <summary>
		/// Identifies DefineImage2 objects when they are encoded. </summary>
		public const int DEFINE_IMAGE_2 = 36;
		/// <summary>
		/// Identifies DefineTextField objects when they are encoded. </summary>
		public const int DEFINE_TEXT_FIELD = 37;
		/// <summary>
		/// Identifies QuicktimeMovie objects when they are encoded. </summary>
		public const int QUICKTIME_MOVIE = 38;
		/// <summary>
		/// Identifies DefineMovieClip objects when they are encoded. </summary>
		public const int DEFINE_MOVIE_CLIP = 39;
		/// <summary>
		/// Identifies SerialNumber objects when they are encoded. </summary>
		public const int SERIAL_NUMBER = 41;
		/// <summary>
		/// Identifies FrameLabel objects when they are encoded. </summary>
		public const int FRAME_LABEL = 43;
		/// <summary>
		/// Identifies SoundStreamHead2 objects when they are encoded. </summary>
		public const int SOUND_STREAM_HEAD_2 = 45;
		/// <summary>
		/// Identifies DefineMorphShape objects when they are encoded. </summary>
		public const int DEFINE_MORPH_SHAPE = 46;
		/// <summary>
		/// Identifies DefineFont2 objects when they are encoded. </summary>
		public const int DEFINE_FONT_2 = 48;
		/// <summary>
		/// Identifies Export objects when they are encoded. </summary>
		public const int EXPORT = 56;
		/// <summary>
		/// Identifies Import objects when they are encoded. </summary>
		public const int IMPORT = 57;
		/// <summary>
		/// Identifies EnableDebugger objects when they are encoded. </summary>
		public const int ENABLE_DEBUGGER = 58;
		/// <summary>
		/// Identifies Initialise objects when they are encoded. </summary>
		public const int INITIALIZE = 59;
		/// <summary>
		/// Identifies DefineVideo objects when they are encoded. </summary>
		public const int DEFINE_VIDEO = 60;
		/// <summary>
		/// Identifies VideoFrame objects when they are encoded. </summary>
		public const int VIDEO_FRAME = 61;
		/// <summary>
		/// Identifies FontInfo2 objects when they are encoded. </summary>
		public const int FONT_INFO_2 = 62;
		/// <summary>
		/// Identifies EnableDebugger2 objects when they are encoded. </summary>
		public const int ENABLE_DEBUGGER_2 = 64;
		/// <summary>
		/// Identifies LimitScript objects when they are encoded. </summary>
		public const int LIMIT_SCRIPT = 65;
		/// <summary>
		/// Identifies TabOrder objects when they are encoded. </summary>
		public const int TAB_ORDER = 66;
		/// <summary>
		/// Identifies FileAttributes objects when they are encoded. </summary>
		public const int FILE_ATTRIBUTES = 69;
		/// <summary>
		/// Identifies Place3 objects when they are encoded. </summary>
		public const int PLACE_3 = 70;
		/// <summary>
		/// Identifies Import2 objects when they are encoded. </summary>
		public const int IMPORT_2 = 71;
		/// <summary>
		/// Identifies FontAlignment objects. </summary>
		public const int FONT_ALIGNMENT = 73;
		/// <summary>
		/// Identifies TextSettings objects. </summary>
		public const int TEXT_SETTINGS = 74;
		/// <summary>
		/// Identifies DefineFont3 objects when they are encoded. </summary>
		public const int DEFINE_FONT_3 = 75;
		/// <summary>
		/// Identifies SymbolClass objects when they are encoded. </summary>
		public const int SYMBOL = 76;
		/// <summary>
		/// Identifies MovieMetaData objects when they are encoded. </summary>
		public const int METADATA = 77;
		/// <summary>
		/// Identifies ScalingGrid objects when they are encoded. </summary>
		public const int DEFINE_SCALING_GRID = 78;
		/// <summary>
		/// Identifies DoABC objects when they are encoded. </summary>
		public const int DO_ABC = 82;
		/// <summary>
		/// Identifies DefineShape4 objects when they are encoded. </summary>
		public const int DEFINE_SHAPE_4 = 83;
		/// <summary>
		/// Identifies DefineMorphShape2 objects when they are encoded. </summary>
		public const int DEFINE_MORPH_SHAPE_2 = 84;
		/// <summary>
		/// Identifies ScenesAndLabels objects when they are encoded. </summary>
		public const int SCENES_AND_LABELS = 86;
		/// <summary>
		/// Identifies DefineData objects when they are encoded. </summary>
		public const int DEFINE_BINARY_DATA = 87;
		/// <summary>
		/// Identifies FontName objects when they are encoded. </summary>
		public const int FONT_NAME = 88;
		/// <summary>
		/// Identifies StartSound2 objects when they are encoded. </summary>
		public const int START_SOUND_2 = 89;
		/// <summary>
		/// Identifies DefineJPEGImage4 objects when they are encoded. </summary>
		public const int DEFINE_JPEG_IMAGE_4 = 90;
		/// <summary>
		/// Identifies DefineFont4 objects when they are encoded. </summary>
		public const int DEFINE_FONT_4 = 91;

		/// <summary>
		/// Private constructor.
		/// </summary>
		private MovieTypes()
		{
			// Class only contains constants
		}
	}

}