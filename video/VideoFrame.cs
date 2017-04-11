using System;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * VideoFrame.java
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
	/// VideoFrame contains the video data displayed in a single frame of a Flash
	/// movie (.swf).
	/// 
	/// <para>
	/// Each frame of video is displayed whenever display list is updated using the
	/// ShowFrame object - any timing information stored within the video data is
	/// ignored. Since the video is updated at the same time as the display list the
	/// frame rate of the video may be the same or less than the frame rate of the
	/// Flash movie but not higher.
	/// </para>
	/// </summary>
	/// <seealso cref= DefineVideo </seealso>
	public sealed class VideoFrame : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "VideoFrame: { identifier=%d;" + " frameNumber=%d; data=%d}";

		/// <summary>
		/// The unique identifier of the video that this frame belongs to. </summary>
		private int identifier;
		/// <summary>
		/// The frame number in the video. </summary>
		private int frameNumber;
		/// <summary>
		/// The encoded video data. </summary>
		private byte[] data;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a VideoFrame object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public VideoFrame(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			identifier = coder.readUnsignedShort();
			frameNumber = coder.readUnsignedShort();
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			data = coder.readBytes(new byte[length - 4]);
		}

		/// <summary>
		/// Constructs a new VideoFrame object which will display the specified frame
		/// of video data in the DefineVideo object that matches the identifier.
		/// </summary>
		/// <param name="uid">
		///            the unique identifier of the DefineVideo object. Must be in
		///            the range 1..65535. </param>
		/// <param name="frame">
		///            the number of the frame. Must be in the range 1..65535. </param>
		/// <param name="videoData">
		///            the encoded video data. For Flash 6 this is encoded in the
		///            H263 format. In Flash 7 H263 and ScreenVideo is supported. </param>


		public VideoFrame(int uid, int frame, byte[] videoData)
		{
			Identifier = uid;
			FrameNumber = frame;
			Data = videoData;
		}

		/// <summary>
		/// Creates and initialises a VideoFrame object using the values copied
		/// from another VideoFrame object.
		/// </summary>
		/// <param name="object">
		///            a VideoFrame object from which the values will be
		///            copied. </param>


		public VideoFrame(VideoFrame @object)
		{
			identifier = @object.identifier;
			frameNumber = @object.frameNumber;
			data = @object.data;
		}

		/// <summary>
		/// Get the identifier of the DefineVideo object where the frame will be
		/// displayed.
		/// </summary>
		/// <returns> the unique identifier of the video. </returns>
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
		/// Get the number of the frame.
		/// </summary>
		/// <returns> the frame number. </returns>
		public int FrameNumber
		{
			get => frameNumber;
		    set
			{
				if ((value < 1) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, value);
				}
				frameNumber = value;
			}
		}


		/// <summary>
		/// Get a copy of the encoded video data. In Flash 6 modified H263
		/// encoded video is supported. Flash 7 supports both modified H263 and
		/// ScreenVideo.
		/// </summary>
		/// <returns> a copy of the video data. </returns>
		public byte[] Data
		{
			get => Arrays.copyOf(data, data.Length);
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				data = Arrays.copyOf(value, value.Length);
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public VideoFrame copy()
		{
			return new VideoFrame(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, identifier, frameNumber, data.Length);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
			length = 4 + data.Length;

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			 if (length > Coder.HEADER_LIMIT)
			 {
				coder.writeShort((MovieTypes.VIDEO_FRAME << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			 }
			else
			{
				coder.writeShort((MovieTypes.VIDEO_FRAME << Coder.LENGTH_FIELD_SIZE) | length);
			}
			coder.writeShort(identifier);
			coder.writeShort(frameNumber);
			coder.writeBytes(data);
		}
	}

}