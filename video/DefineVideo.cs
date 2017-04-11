using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * DefineVideo.java
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

namespace com.flagstone.transform.video
{
    /// <summary>
	/// The DefineVideo class is used to define a video stream within a Flash file.
	/// 
	/// <para>
	/// Video objects contain a unique identifier and are treated in the same way as
	/// shapes, buttons, images, etc. The video data displayed is define using the
	/// VideoFrame class. Each frame of video is displayed whenever display list is
	/// updated using the ShowFrame object - any timing information stored within the
	/// video data is ignored. The actual video data is encoded using the VideoFrame
	/// class.
	/// </para>
	/// </summary>
	public sealed class DefineVideo : DefineTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "DefineVideo: { identifier=%d;" + " frameCount=%d; width=%d; height=%d; deblocking=%s;" + " smoothing=%s; codec=%s}";

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The number of frames in the video. </summary>
		private int frameCount;
		/// <summary>
		/// The width of the frame. </summary>
		private int width;
		/// <summary>
		/// The height of the frame. </summary>
		private int height;
		/// <summary>
		/// Code indicating whether deblocking is applied. </summary>
		private int deblocking;
		/// <summary>
		/// Is smoothing applied. </summary>
		private bool smoothed;
		/// <summary>
		/// Code representing the codec used. </summary>
		private int codec;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a DefineVideo object using values encoded in the
		/// Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public DefineVideo(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			frameCount = coder.readUnsignedShort();
			width = coder.readUnsignedShort();
			height = coder.readUnsignedShort();



			int info = coder.readByte();
			deblocking = (info & 0x06) >> 1;
			smoothed = (info & 0x01) == 1;
			codec = coder.readByte();
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a DefineVideo object with the specified parameters.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier for this object. Must be in the range
		///            1..65535. </param>
		/// <param name="count">
		///            the number of video frames. Must be in the range 0..65535. </param>
		/// <param name="frameWidth">
		///            the width of each frame in pixels. Must be in the range
		///            0..65535. </param>
		/// <param name="frameHeight">
		///            the height of each frame in pixels. Must be in the range
		///            0..65535. </param>
		/// <param name="deblock">
		///            controls whether the Flash Player's deblocking filter is used,
		///            either Off, On or UseVideo to allow the video data to specify
		///            whether the deblocking filter is used. </param>
		/// <param name="smoothing">
		///            turns smoothing on or off to improve the quality of the
		///            displayed image. </param>
		/// <param name="videoCodec">
		///            the format of the video data. Flash 6 supports H263. Support
		///            for Macromedia's ScreenVideo format was added in Flash 7. </param>


		public DefineVideo(int uid, int count, int frameWidth, int frameHeight, Deblocking deblock, bool smoothing, VideoFormat videoCodec)
		{
			Identifier = uid;
			FrameCount = count;
			Width = frameWidth;
			Height = frameHeight;
			Deblocking = deblock;
			Smoothed = smoothing;
			Codec = videoCodec;
		}

		/// <summary>
		/// Creates and initialises an DefineVideo object using the values
		/// copied from another DefineVideo object.
		/// </summary>
		/// <param name="object">
		///            a DefineVideo object from which the values will be
		///            copied. </param>


		public DefineVideo(DefineVideo @object)
		{
			identifier = @object.identifier;
			frameCount = @object.frameCount;
			width = @object.width;
			height = @object.height;
			deblocking = @object.deblocking;
			smoothed = @object.smoothed;
			codec = @object.codec;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
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
		/// Get the number of frames in the video.
		/// </summary>
		/// <returns> the number of frames. </returns>
		public int FrameCount
		{
			get => frameCount;
		    set
			{
				if ((value < 0) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value);
				}
				frameCount = value;
			}
		}


		/// <summary>
		/// Get the width of each frame in pixels.
		/// </summary>
		/// <returns> the frame width. </returns>
		public int Width
		{
			get => width;
		    set
			{
				if ((value < 0) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value);
				}
				width = value;
			}
		}


		/// <summary>
		/// Get the height of each frame in pixels.
		/// </summary>
		/// <returns> the frame height. </returns>
		public int Height
		{
			get => height;
		    set
			{
				if ((value < 0) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, value);
				}
				height = value;
			}
		}


		/// <summary>
		/// Get the method used to control the Flash Player's deblocking filter,
		/// either OFF, ON or USE_VIDEO.
		/// </summary>
		/// <returns> the deblocking applied to the frame. </returns>
		public Deblocking Deblocking
		{
			get
			{
				Deblocking value;
				switch (deblocking)
				{
				case 1:
					value = Deblocking.OFF;
					break;
				case 2:
					value = Deblocking.ON;
					break;
				case 3:
					value = Deblocking.LEVEL2;
					break;
				case 4:
					value = Deblocking.LEVEL3;
					break;
				case 5:
					value = Deblocking.LEVEL4;
					break;
				default:
					value = Deblocking.VIDEO;
					break;
				}
				return value;
			}
			set
			{
				switch (value)
				{
				case Deblocking.VIDEO:
					deblocking = 0;
					break;
				case Deblocking.OFF:
					deblocking = 1;
					break;
				case Deblocking.ON:
					deblocking = 2;
					break;
				case Deblocking.LEVEL2:
					deblocking = 3;
					break;
				case Deblocking.LEVEL3:
					deblocking = 4;
					break;
				case Deblocking.LEVEL4:
					deblocking = 5;
					break;
				default:
					throw new ArgumentException();
				}
			}
		}


		/// <summary>
		/// Will the Flash Player will apply smoothing to the video when it is
		/// played.
		/// </summary>
		/// <returns> true if smoothing is applied. </returns>
		public bool Smoothed
		{
			get => smoothed;
		    set => smoothed = value;
		}


		/// <summary>
		/// Get the format used to encode the video data.
		/// </summary>
		/// <returns> the format used to encode the video. </returns>
		public VideoFormat Codec
		{
			get
			{
				VideoFormat value;
				switch (codec)
				{
				case Coder.BIT2 | Coder.BIT0:
					value = VideoFormat.VP6ALPHA;
					break;
				case Coder.BIT2:
					value = VideoFormat.VP6;
					break;
				case Coder.BIT1:
					value = VideoFormat.H263;
					break;
				case Coder.BIT0 | Coder.BIT1:
					value = VideoFormat.SCREEN;
					break;
				default:
					throw new InvalidOperationException();
				}
				return value;
			}
			set
			{
				switch (value)
				{
				case VideoFormat.H263:
					codec = Coder.BIT1;
					break;
				case VideoFormat.SCREEN:
					codec = Coder.BIT0 | Coder.BIT1;
					break;
				case VideoFormat.VP6:
					codec = Coder.BIT2;
					break;
				case VideoFormat.VP6ALPHA:
					codec = Coder.BIT2 | Coder.BIT0;
					goto default;
				default:
					throw new ArgumentException();
				}
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public MovieTag copy()
		{
			return new DefineVideo(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, frameCount, width, height, deblocking, smoothed, codec);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			length = 10;
			return Coder.SHORT_HEADER + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_VIDEO << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_VIDEO << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeShort(identifier);
			coder.writeShort(frameCount);
			coder.writeShort(width);
			coder.writeShort(height);
			int bits = deblocking << 1;
			bits |= smoothed ? Coder.BIT0 : 0;
			coder.writeByte(bits);
			coder.writeByte(codec);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}