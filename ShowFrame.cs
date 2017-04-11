/*
 * ShowFrame.java
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

namespace com.flagstone.transform
{
    /// <summary>
	/// ShowFrame is used to instruct the Flash Player to display a single frame in a
	/// movie or movie clip.
	/// 
	/// <para>
	/// When a frame is displayed the Flash Player performs the following:
	/// <ul>
	/// <li>Any actions defined using a DoAction object are executed.</li>
	/// <li>
	/// The contents of the Flash Player's display list are drawn on the screen.
	/// </li>
	/// </ul>
	/// </para>
	/// 
	/// <para>
	/// The scope of a frame is delineated by successive ShowFrame objects. All the
	/// commands that affect change the state of the display list or define actions
	/// to be executed take effect when the Flash Player displays the frame. All the
	/// objects displayed in a frame must be defined before they can be displayed.
	/// </para>
	/// </summary>
	public sealed class ShowFrame : MovieTag
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "ShowFrame";
		/// <summary>
		/// Singleton. </summary>
		private static readonly ShowFrame INSTANCE = new ShowFrame();

		/// <summary>
		/// Returns a shared ShowFrame object.
		/// </summary>
		/// <returns> an object that can safely be shared among objects. </returns>
		public static ShowFrame Instance => INSTANCE;

	    /// <summary>
		/// Returns a shared ShowFrame object.
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



		public static ShowFrame getInstance(SWFDecoder coder, Context context)
		{
			coder.readUnsignedShort();
			return INSTANCE;
		}

		/// <summary>
		/// Private constructor for the singleton. </summary>
		private ShowFrame()
		{
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public ShowFrame copy()
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
			return 2;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeShort(MovieTypes.SHOW_FRAME << Coder.LENGTH_FIELD_SIZE);
		}
	}

}