using System.Collections.Generic;
using System.IO;
using com.flagstone.transform.coder;

/*
 * ScreenVideoPacket.java
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
    /// The ScreenVideoPacket class is used to encode or decode a frame of video data
    /// using Macromedia's ScreenVideo format.
    /// </summary>
    public sealed class ScreenPacket
    {

        /// <summary>
        /// Multiplier for the encoded value representing the block width. </summary>
        private const int PIXELS_PER_BLOCK = 16;

        /// <summary>
        /// Is this frame a key frame with blocks for the entire image. </summary>
        private bool keyFrame;
        /// <summary>
        /// The width of each block. </summary>
        private int blockWidth;
        /// <summary>
        /// The height of each block. </summary>
        private int blockHeight;
        /// <summary>
        /// The width of the image. </summary>
        private int imageWidth;
        /// <summary>
        /// The height of the image. </summary>
        private int imageHeight;
        /// <summary>
        /// List of blocks that make up the image. </summary>
        private IList<ImageBlock> imageBlocks;

        /// <summary>
        /// Decode a screen packet from a block of data. </summary>
        /// <param name="data"> the encoded screen packet data. </param>
        /// <exception cref="IOException"> if the data cannot be decoded. </exception>



        public ScreenPacket(byte[] data)
        {


            MemoryStream stream = new MemoryStream(data);


            SWFDecoder coder = new SWFDecoder(stream);

            int info = coder.readByte();
            keyFrame = (info & Coder.NIB1) != 0;

            info = (coder.readByte() << Coder.TO_UPPER_BYTE) + coder.readByte();
            blockWidth = (((info & Coder.NIB3) >> Coder.ALIGN_NIB3) + 1) * PIXELS_PER_BLOCK;
            imageWidth = info & Coder.LOWEST12;

            info = (coder.readByte() << Coder.TO_UPPER_BYTE) + coder.readByte();
            blockHeight = (((info & Coder.NIB3) >> Coder.ALIGN_NIB3) + 1) * PIXELS_PER_BLOCK;
            imageHeight = info & Coder.LOWEST12;



            int columns = imageWidth / blockWidth + ((imageWidth % blockWidth > 0) ? 1 : 0);


            int rows = imageHeight / blockHeight + ((imageHeight % blockHeight > 0) ? 1 : 0);

            int height = imageHeight;
            int width = imageWidth;

            imageBlocks = new List<ImageBlock>();
            ImageBlock block;

            int length;

            for (int i = 0; i < rows; i++, height -= blockHeight)
            {
                for (int j = 0; j < columns; j++, width -= blockWidth)
                {
                    length = (coder.readByte() << Coder.TO_UPPER_BYTE) + coder.readByte();

                    if (length == 0)
                    {
                        block = new ImageBlock(0, 0, new byte[] { });
                    }
                    else
                    {


                        int dataHeight = (height < blockHeight) ? height : blockHeight;


                        int dataWidth = (width < blockWidth) ? width : blockWidth;

                        block = new ImageBlock(dataWidth, dataHeight, coder.readBytes(new byte[length]));
                    }

                    imageBlocks.Add(block);
                }
            }
        }

        /// <summary>
        /// Create a ScreenPacket with no image blocks.
        /// </summary>
        public ScreenPacket()
        {
            imageBlocks = new List<ImageBlock>();
        }

        /// <summary>
        /// Creates a ScreenVideoPacket.
        /// </summary>
        /// <param name="key">
        ///            indicates whether the packet contains a key frame. </param>
        /// <param name="imgWidth">
        ///            the width of the frame. </param>
        /// <param name="imgHeight">
        ///            the height of the frame. </param>
        /// <param name="blkWidth">
        ///            the width of the blocks that make up the frame. </param>
        /// <param name="blkHeight">
        ///            the height of the blocks that make up the frame. </param>
        /// <param name="blocks">
        ///            the array of ImageBlocks that make up the frame. </param>


        public ScreenPacket(bool key, int imgWidth, int imgHeight, int blkWidth, int blkHeight, IList<ImageBlock> blocks)
        {
            KeyFrame = key;
            ImageWidth = imgWidth;
            ImageHeight = imgHeight;
            BlockWidth = blkWidth;
            BlockHeight = blkHeight;
            ImageBlocks = blocks;
        }

        /// <summary>
        /// Creates and initialises a ScreenPacket object using the values copied
        /// from another ScreenPacket object.
        /// </summary>
        /// <param name="object">
        ///            a ScreenPacket object from which the values will be
        ///            copied. </param>


        public ScreenPacket(ScreenPacket @object)
        {
            keyFrame = @object.keyFrame;
            blockWidth = @object.blockWidth;
            blockHeight = @object.blockHeight;
            imageWidth = @object.imageWidth;
            imageHeight = @object.imageHeight;

            imageBlocks = new List<ImageBlock>(@object.imageBlocks.Count);

            foreach (ImageBlock block in @object.imageBlocks)
            {
                imageBlocks.Add(block.copy());
            }
        }

        /// <summary>
        /// Add an image block to the array that make up the frame.
        /// </summary>
        /// <param name="block">
        ///            an ImageBlock. Must not be null. </param>
        /// <returns> this object. </returns>


        public ScreenPacket add(ImageBlock block)
        {
            imageBlocks.Add(block);
            return this;
        }

        /// <summary>
        /// Does the packet contains a key frame.
        /// </summary>
        /// <returns> true if the packet is a key frame. </returns>
        public bool KeyFrame
        {
            get => keyFrame;
            set => keyFrame = value;
        }


        /// <summary>
        /// Get the width of the frame in pixels.
        /// </summary>
        /// <returns> the frame width. </returns>
        public int ImageWidth
        {
            get => imageWidth;
            set => imageWidth = value;
        }


        /// <summary>
        /// Get the height of the frame in pixels.
        /// </summary>
        /// <returns> the image height. </returns>
        public int ImageHeight
        {
            get => imageHeight;
            set => imageHeight = value;
        }


        /// <summary>
        /// Get the width of the blocks in pixels.
        /// </summary>
        /// <returns> the block width. </returns>
        public int BlockWidth
        {
            get => blockWidth;
            set => blockWidth = value;
        }


        /// <summary>
        /// Get the height of the blocks in pixels.
        /// </summary>
        /// <returns> the block width. </returns>
        public int BlockHeight
        {
            get => blockHeight;
            set => blockHeight = value;
        }


        /// <summary>
        /// Get the image blocks that have changed in this frame.
        /// </summary>
        /// <returns> the list of image blocks that make up the frame. </returns>
        public IList<ImageBlock> ImageBlocks
        {
            get => imageBlocks;
            set => imageBlocks = new List<ImageBlock>(value);
        }


        /// <summary>
        /// {@inheritDoc} </summary>
        public ScreenPacket copy()
        {
            return new ScreenPacket(this);
        }

        /// <summary>
        /// Encode this ScreenPacket. </summary>
        /// <returns> the data representing the encoded image blocks. </returns>
        /// <exception cref="IOException"> if there is an error encoding the blocks. </exception>


        public byte[] encode()
        {


            MemoryStream stream = new MemoryStream();


            SWFEncoder coder = new SWFEncoder(stream);

            int bits = keyFrame ? Coder.BIT4 : Coder.BIT5;
            bits |= Coder.BIT0 | Coder.BIT1;
            coder.writeByte(bits);

            int word = ((blockWidth / PIXELS_PER_BLOCK) - 1) << Coder.TO_UPPER_NIB;
            word |= imageWidth & Coder.LOWEST12;
            coder.writeByte(word >> Coder.TO_LOWER_BYTE);
            coder.writeByte(word);

            word = ((blockHeight / PIXELS_PER_BLOCK) - 1) << Coder.TO_UPPER_NIB;
            word |= imageHeight & Coder.LOWEST12;
            coder.writeByte(word >> Coder.TO_LOWER_BYTE);
            coder.writeByte(word);

            byte[] blockData;

            foreach (ImageBlock block in imageBlocks)
            {
                if (block.Empty)
                {
                    coder.writeShort(0);
                }
                else
                {
                    blockData = block.Block;
                    coder.writeByte(blockData.Length >> Coder.TO_LOWER_BYTE);
                    coder.writeByte(blockData.Length);
                    coder.writeBytes(blockData);
                }
            }

            using (var streamreader = new BinaryReader(stream))
            {
                return streamreader.ReadBytes((int)stream.Length);
            }
        }
    }

}