using System;
using System.Collections.Generic;
using System.IO;
using com.flagstone.transform.coder;

/*
 * Shape.java
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

namespace com.flagstone.transform.shape
{
    /// <summary>
	/// Shape is a container class for the shape objects (Line, Curve, ShapeStyle
	/// and ShapeStyle2 objects) that describe how a particular shape is drawn.
	/// 
	/// <para>
	/// Shapes are used in shape and font definitions. The Shape class is used to
	/// simplify the design of these classes and provides no added functionality
	/// other than acting as a container class.
	/// </para>
	/// </summary>
	public sealed class Shape : SWFEncodeable
	{

		/// <summary>
		/// The minimum coordinate in along the x or y axes.
		/// </summary>
		public const int MIN_COORD = -65536;
		/// <summary>
		/// The maximum coordinate in along the x or y axes.
		/// </summary>
		public const int MAX_COORD = 65535;

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "Shape: { records=%s}";

		/// <summary>
		/// Decode a ShapeData object into the set of ShapeRecord objects that
		/// describe how a shape is drawn. </summary>
		/// <param name="shapeData"> a ShapeData object containing the encoded shape. </param>
		/// <returns> the decoded Shape. </returns>
		/// <exception cref="IOException"> if there is an error decoding the shape. </exception>



		public static Shape shapeFromData(ShapeData shapeData)
		{


			byte[] data = shapeData.Data;


			MemoryStream stream = new MemoryStream(data);


			SWFDecoder coder = new SWFDecoder(stream);


			Context context = new Context();
			return new Shape(coder, context);
		}

		/// <summary>
		/// List of ShapeRecords that draws the shape. </summary>
		private IList<ShapeRecord> objects;
		/// <summary>
		/// Indicates whether the ShapeRecords are already encoded. </summary>
		
		private bool isEncoded;

		/// <summary>
		/// Creates and initialises a Shape object using values encoded in the Flash
		/// binary format.
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




		public Shape(SWFDecoder coder, Context context)
		{
			objects = new List<ShapeRecord>();



			int sizes = coder.readByte();
			context.put(Context.FILL_SIZE, (sizes & Coder.NIB1) >> Coder.TO_LOWER_NIB);
			context.put(Context.LINE_SIZE, sizes & Coder.NIB0);



			SWFFactory<ShapeRecord> decoder = context.Registry.ShapeDecoder;

			while (coder.scanBits(6, false) != 0)
			{
				decoder.getObject(objects, coder, context);
			}
			coder.readBits(6, false);
			coder.alignToByte();
		}

		/// <summary>
		/// Constructs an empty Shape.
		/// </summary>
		public Shape()
		{
			objects = new List<ShapeRecord>();
		}

		/// <summary>
		/// Creates a Shape object, specifying the Objects that describe how the
		/// shape is drawn.
		/// </summary>
		/// <param name="list">
		///            the list of shape records. Must not be null. </param>


		public Shape(IList<ShapeRecord> list)
		{
			Objects = list;
		}

		/// <summary>
		/// Creates and initialises a Shape object using the values copied from
		/// another Shape object.
		/// </summary>
		/// <param name="object">
		///            a Shape object from which the values will be copied. </param>


		public Shape(Shape @object)
		{
			objects = new List<ShapeRecord>(@object.objects.Count);

			foreach (ShapeRecord record in @object.objects)
			{
				objects.Add(record.copy());
			}
		}

		/// <summary>
		/// Adds the object to the list of shape records.
		/// </summary>
		/// <param name="anObject">
		///            an instance of ShapeStyle, Line or Curve. Must not be null. </param>
		/// <returns> this object. </returns>


		public Shape add(ShapeRecord anObject)
		{
			if (anObject == null)
			{
				throw new ArgumentException();
			}
			objects.Add(anObject);
			return this;
		}

		/// <summary>
		/// Get the list of shape records that define the shape.
		/// </summary>
		/// <returns> the list of shape records. </returns>
		public IList<ShapeRecord> Objects
		{
			get => objects;
		    set
			{
				if (value == null)
				{
					throw new ArgumentException();
				}
				objects = value;
			}
		}


		/// <summary>
		/// {@inheritDoc} </summary>
		public Shape copy()
		{
			return new Shape(this);
		}

		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, objects);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{

			int length = 0;

			isEncoded = objects.Count == 1 && objects[0] is ShapeData;

			if (isEncoded)
			{
				length += objects[0].prepareToEncode(context);
			}
			else
			{
				context.put(Context.SHAPE_SIZE, 0);

				// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 6 LINES
				int numberOfBits = 21; // Includes end of shape and align to byte

				foreach (ShapeRecord record in objects)
				{
					numberOfBits += record.prepareToEncode(context);
				}
				length += ((int)((uint)numberOfBits >> 3));
			}
			return length;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{

			if (isEncoded)
			{
				objects[0].encode(coder, context);
			}
			else
			{
				int bits = context.get(Context.FILL_SIZE) << Coder.TO_UPPER_NIB;
				bits |= context.get(Context.LINE_SIZE);
				coder.writeByte(bits);

				foreach (ShapeRecord record in objects)
				{
					record.encode(coder, context);
				}
				// CHECKSTYLE IGNORE MagicNumberCheck FOR NEXT 1 LINES
				coder.writeBits(0, 6); // End of shape
				coder.alignToByte();
			}
		}
	}

}