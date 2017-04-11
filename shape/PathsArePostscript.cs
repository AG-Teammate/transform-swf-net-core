/*
 * PathsArePostscript.java
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

using com.flagstone.transform.coder;

namespace com.flagstone.transform.shape
{
    /// <summary>
	/// The PathsArePostscript class is used to notify the Flash Player that the
	/// glyphs encoded in a font definition were derived from a PostScript-based font
	/// definition.
	/// 
	/// <para>
	/// The PathsArePostscript is not documented in the current Macromedia Flash
	/// (SWF) File Format Specification. It was referenced in earlier editions but
	/// its exact function was not known. It is thought that is used to signal to the
	/// Flash Player that the paths describing the outlines of the glyphs in a font
	/// were derived from a font defined using Postscript. The information can be
	/// used to provide better rendering of the glyphs.
	/// </P>
	/// 
	/// </para>
	/// </summary>
	public sealed class PathsArePostscript : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "PathsArePostscript";
		/// <summary>
		/// Singleton. </summary>
		private static readonly PathsArePostscript INSTANCE = new PathsArePostscript();

		/// <summary>
		/// Returns a singleton.
		/// </summary>
		/// <returns> an object that can safely be shared among objects. </returns>
		public static PathsArePostscript Instance => INSTANCE;

	    /// <summary>
		/// Returns a singleton.
		/// </summary>
		/// <param name="coder">
		///            an SWFEncoder object.
		/// </param>
		/// <param name="context"> a Context object used to obtain the total number of
		/// frames in a movie.
		/// </param>
		/// <returns> an object that can safely be shared among objects.
		/// </returns>
		/// <exception cref="IOException"> if an error occurs while encoding the object. </exception>



		public static PathsArePostscript getInstance(SWFDecoder coder, Context context)
		{
			context.put(Context.POSTSCRIPT, 1);
			coder.readUnsignedShort();
			return INSTANCE;
		}

		/// <summary>
		/// Private constructor used to create singleton. </summary>
		private PathsArePostscript()
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public PathsArePostscript copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return FORMAT;
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			context.put(Context.POSTSCRIPT, 1);
			return 2;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeShort(MovieTypes.PATHS_ARE_POSTSCRIPT << Coder.LENGTH_FIELD_SIZE);
		}
	}

}