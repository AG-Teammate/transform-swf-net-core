using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;
using com.flagstone.transform.exception;

/*
 * ScenesAndLabels.java
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

namespace com.flagstone.transform
{
    /// <summary>
	/// ScenesAndLabels is used to list the scenes (main timeline only) and labelled
	/// frames for movies and movie clips.
	/// </summary>
	public sealed class ScenesAndLabels : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "ScenesAndLabels: { scenes=%s;" + " labels=%s}";

		/// <summary>
		/// The table of scenes. </summary>
		private IDictionary<int?, string> scenes;
		/// <summary>
		/// The table of labelled frames. </summary>
		private IDictionary<int?, string> labels;

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a ScenesAndLabels object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public ScenesAndLabels(SWFDecoder coder)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();

			int count = coder.readVarInt();
			scenes = new Dictionary<int?, string>(count);
			for (int i = 0; i < count; i++)
			{
				scenes[coder.readVarInt()] = coder.readString();
			}

			count = coder.readVarInt();
			labels = new Dictionary<int?, string>(count);
			for (int i = 0; i < count; i++)
			{
				labels[coder.readVarInt()] = coder.readString();
			}
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates a ScenesAndLabels object with empty tables for the scenes and
		/// labels.
		/// </summary>
		public ScenesAndLabels()
		{
			scenes = new Dictionary<int?, string>();
			labels = new Dictionary<int?, string>();
		}

		/// <summary>
		/// Create a new ScenesAndLabels object with the specified list of scenes
		/// and labelled frames. </summary>
		/// <param name="sceneMap"> a table of frame numbers and the associated name for
		/// the scenes on the main timeline of a movie. </param>
		/// <param name="labelMap"> a table of frame numbers and the associated name for
		/// the labelled frames in a movie or movie clip. </param>


		public ScenesAndLabels(IDictionary<int?, string> sceneMap, IDictionary<int?, string> labelMap)
		{
			scenes = sceneMap;
			labels = labelMap;
		}

		/// <summary>
		/// Creates and initialises a ScenesAndLabels object using the values copied
		/// from another ScenesAndLabels object.
		/// </summary>
		/// <param name="object">
		///            a ScenesAndLabels object from which the values will be
		///            copied. </param>


		public ScenesAndLabels(ScenesAndLabels @object)
		{
			scenes = new Dictionary<int?, string>(@object.scenes);
			labels = new Dictionary<int?, string>(@object.labels);
		}

		/// <summary>
		/// Add an entry to the list of scenes with the frame number and scene
		/// name. </summary>
		/// <param name="offset"> the frame number. </param>
		/// <param name="name"> the scene name. </param>
		/// <returns> this object. </returns>


		public ScenesAndLabels addScene(int offset, string name)
		{
			if ((offset < 0) || (offset > Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, offset);
			}
			if (ReferenceEquals(name, null) || name.Length == 0)
			{
				throw new ArgumentException();
			}
			scenes[offset] = name;
			return this;
		}

		/// <summary>
		/// Get the table of frame numbers and associated names. </summary>
		/// <returns> a map associating frame numbers to scene names. </returns>
		public IDictionary<int?, string> Scenes
		{
			get => scenes;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				scenes = value;
			}
		}


		/// <summary>
		/// Add an entry to the list of labelled frames with the frame number and
		/// frame label. </summary>
		/// <param name="offset"> the frame number. </param>
		/// <param name="name"> the frame label. </param>
		/// <returns> this object. </returns>


		public ScenesAndLabels addLabel(int offset, string name)
		{
			if ((offset < 0) || (offset > Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, offset);
			}
			if (ReferenceEquals(name, null) || name.Length == 0)
			{
				throw new ArgumentException();
			}
			labels[offset] = name;
			return this;
		}

		/// <summary>
		/// Get the table of frame numbers and frame labels. </summary>
		/// <returns> a map associating frame numbers to frame labels. </returns>
		public IDictionary<int?, string> Labels
		{
			get => labels;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				labels = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public ScenesAndLabels copy()
		{
			return new ScenesAndLabels(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, scenes, labels);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{

			length = Coder.sizeVariableU32(scenes.Count);

			foreach (int? offset in scenes.Keys)
			{
				length += Coder.sizeVariableU32((int)offset) + context.strlen(scenes[offset]);
			}

			length += Coder.sizeVariableU32(labels.Count);

			foreach (int? offset in labels.Keys)
			{
				length += Coder.sizeVariableU32((int)offset) + context.strlen(labels[offset]);
			}

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.SCENES_AND_LABELS << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.SCENES_AND_LABELS << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			coder.writeVarInt(scenes.Count);

			foreach (int? identifier in scenes.Keys)
			{
				coder.writeVarInt(identifier.Value);
				coder.writeString(scenes[identifier]);
			}

			coder.writeVarInt(labels.Count);

			foreach (int? identifier in labels.Keys)
			{
				coder.writeVarInt(identifier.Value);
				coder.writeString(labels[identifier]);
			}
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}
	}

}