using System;
using com.flagstone.transform.coder;

/*
 * FrameLabel.java
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
	/// FrameLabel defines a name for the current frame in a movie or movie clip.
	/// 
	/// <para>
	/// The name can be referenced from other objects such as GotoFrame2 to simplify
	/// the creation of scripts to control movies by using a predefined name rather
	/// than the frame number. The label assigned to a particular frame should be
	/// unique. A frame cannot be referenced within a movie before the Player has
	/// loaded and displayed the frame that contains the corresponding FrameLabel
	/// object.
	/// </para>
	/// 
	/// <para>
	/// If a frame is defined as an anchor it may also be referenced externally when
	/// specifying the movie to play using a URL - similar to the way names links are
	/// used in HTML. When the Flash Player loads a movie it will begin playing at
	/// the frame specified in the URL.
	/// </para>
	/// </summary>
	public sealed class FrameLabel : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "FrameLabel: { label=%s; anchor=%s}";

		/// <summary>
		/// The label for the frame. </summary>
		private string label;
		/// <summary>
		/// Whether the frame can be referenced by a URL. </summary>
		private bool anchor;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a FrameLabel object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public FrameLabel(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			label = coder.readString();
			if (coder.bytesRead() < length)
			{
				anchor = coder.readByte() != 0;
			}
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a FrameLabel object with the specified name.
		/// </summary>
		/// <param name="aString">
		///            the string that defines the label that will be assigned to the
		///            current frame. Must not be null or an empty string. </param>


		public FrameLabel(string aString)
		{
			Label = aString;
		}

		/// <summary>
		/// Creates a FrameLabel object with the specified name. If the isAnchor flag
		/// is true then the frame can be directly addressed by a URL and the Flash
		/// Player will begin playing the movie at the specified frame.
		/// </summary>
		/// <param name="aString">
		///            the string that defines the label that will be assigned to the
		///            current frame. Must not be null or an empty string. </param>
		/// <param name="isAnchor">
		///            if true the name will be used as an anchor when referencing
		///            the frame in a URL. </param>


		public FrameLabel(string aString, bool isAnchor)
		{
			Label = aString;
			anchor = isAnchor;
		}

		/// <summary>
		/// Creates a FrameLabel object with a copy of the label and anchor from
		/// another FrameLabel object.
		/// </summary>
		/// <param name="object">
		///            a FrameLabel object to copy. </param>


		public FrameLabel(FrameLabel @object)
		{
			label = @object.label;
			anchor = @object.anchor;
		}

		/// <summary>
		/// Get the label for the frame.
		/// </summary>
		/// <returns> the string used to label the frame. </returns>
		public string Label
		{
			get => label;
		    set
			{
				if (ReferenceEquals(value, null) || value.Length == 0)
				{
					throw new ArgumentException();
				}
				label = value;
			}
		}


		/// <summary>
		/// Is the frame name is also used as an anchor so the frame can be
		/// referenced from outside of the movie.
		/// </summary>
		/// <returns> true is the name can be used as an external reference to the
		/// frame. </returns>
		public bool Anchor
		{
			get => anchor;
		    set => anchor = value;
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public FrameLabel copy()
		{
			return new FrameLabel(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, label, anchor.ToString());
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{

			length = context.strlen(label);
			length += anchor ? 1 : 0;

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.FRAME_LABEL << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.FRAME_LABEL << Coder.LENGTH_FIELD_SIZE) | length);
			}
			coder.writeString(label);

			if (anchor)
			{
				coder.writeByte(1);
			}
		}
	}

}