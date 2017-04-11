using System;

/*
 * ImageBlock.java
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
	/// ImageBlock is used to sub-divide an image into a set of blocks so they can be
	/// streamed using Screen Video. Image blocks are compared so only pixel
	/// information for the portions of the image that change are sent.
	/// 
	/// <para>
	/// An image is divided by tiling the blocks across the image from top-left to
	/// bottom right. If the image is not covered an integer number of blocks then
	/// the size of the blocks along the right and bottom edges of the image are
	/// reduced in size.
	/// </para>
	/// </summary>
	/// <seealso cref= DefineVideo </seealso>
	public sealed class ImageBlock
	{
		/// <summary>
		/// Width of the block in pixels. </summary>
		
		private readonly int width;
		/// <summary>
		/// Height of the block in pixels. </summary>
		
		private readonly int height;
		/// <summary>
		/// The block pixels. </summary>
		
		private readonly byte[] block;

		/// <summary>
		/// Create a new image block with the specified width and height and image
		/// data. The image is compressed using the zip format.
		/// </summary>
		/// <param name="blockWidth">
		///            the width of the block in pixels. </param>
		/// <param name="blockHeight">
		///            the height of the block in pixels </param>
		/// <param name="pixels">
		///            the pixels covered by the block, compressed using the zip
		///            format. </param>


		public ImageBlock(int blockWidth, int blockHeight, byte[] pixels)
		{
			width = blockWidth;
			height = blockHeight;
			block = Arrays.copyOf(pixels, pixels.Length);
		}

		/// <summary>
		/// Creates and initialises a ImageBlock object using the values copied
		/// from another ImageBlock object.
		/// </summary>
		/// <param name="object">
		///            a ImageBlock object from which the values will be
		///            copied. </param>


		public ImageBlock(ImageBlock @object)
		{
			width = @object.width;
			height = @object.height;
			block = @object.block;
		}

		/// <summary>
		/// Get the width of the block. although the block size is specified in
		/// parent ScreenVideoPacket object the actual block size used may vary if
		/// the tiled array of blocks overlaps the edge of the image.
		/// </summary>
		/// <returns> the width of the block. </returns>
		public int Width => width;

	    /// <summary>
		/// Get the height of the block. although the block size is specified in
		/// parent ScreenVideoPacket object the actual block size used may vary if
		/// the tiled array of blocks overlaps the edge of the image.
		/// </summary>
		/// <returns> the height of the block. </returns>
		public int Height => height;

	    /// <summary>
		/// Get the zipped image data for the block.
		/// </summary>
		/// <returns> a copy of the block data. </returns>
		public byte[] Block => Arrays.copyOf(block, block.Length);

	    /// <summary>
		/// When a ScreenVideo stream is created only the image blocks that change
		/// are included. The blocks that do not change are encoded as empty blocks
		/// which have width and height of zero and do not contain any image data.
		/// This convenience method is used to determine when an image block contains
		/// any valid image data.
		/// </summary>
		/// <returns> true if the block covers an area of the image that changed or
		///         false if no image data is included. </returns>
		public bool Empty => (width == 0) || (height == 0) || (block == null) || (block.Length == 0);

	    /// <summary>
		/// {@inheritDoc} </summary>
		public ImageBlock copy()
		{
			return new ImageBlock(this);
		}
	}

}