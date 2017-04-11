

/*
 *  DecoderRegistry.java
 *  Transform Utilities
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

using com.flagstone.transform.action;
using com.flagstone.transform.fillstyle;
using com.flagstone.transform.filter;
using com.flagstone.transform.shape;

namespace com.flagstone.transform.coder
{
    /// <summary>
	/// The DecoderRegistry is used to maintain a table of objects that can be used
	/// to decode the different types of object encountered in a Flash file.
	/// </summary>
	public sealed class DecoderRegistry
	{

		/// <summary>
		/// Registry containing a set of default decoders for different objects. </summary>
		private static DecoderRegistry defaultRegistry;

		static DecoderRegistry()
		{
			defaultRegistry = new DecoderRegistry();
			defaultRegistry.FilterDecoder = new FilterDecoder();
			defaultRegistry.FillStyleDecoder = new FillStyleDecoder();
			defaultRegistry.MorphFillStyleDecoder = new MorphFillStyleDecoder();
			defaultRegistry.ShapeDecoder = new ShapeDecoder();
			defaultRegistry.ActionDecoder = new ActionDecoder();
			defaultRegistry.MovieDecoder = new MovieDecoder();
		}

		/// <summary>
		/// Get the default registry.
		/// </summary>
		/// <returns> a registry with a default set of decoders. </returns>
		public static DecoderRegistry Default
		{
			get => new DecoderRegistry(defaultRegistry);
		    set => defaultRegistry = new DecoderRegistry(value);
		}


		/// <summary>
		/// The decoder for filters. </summary>
		
		private SWFFactory<Filter> filterDecoder;
		/// <summary>
		/// The decoder for fill styles. </summary>
		
		private SWFFactory<FillStyle> fillStyleDecoder;
		/// <summary>
		/// The decoder for morphing fill styles. </summary>
		
		private SWFFactory<FillStyle> morphStyleDecoder;
		/// <summary>
		/// The decoder for shape records. </summary>
		
		private SWFFactory<ShapeRecord> shapeDecoder;
		/// <summary>
		/// The decoder for actions. </summary>
		
		private SWFFactory<Action> actionDecoder;
		/// <summary>
		/// The decoder for movie objects. </summary>
		
		private SWFFactory<MovieTag> movieDecoder;

		/// <summary>
		/// Creates a DecoderRegistry with no decoders yet registered.
		/// </summary>
		public DecoderRegistry()
		{
			// All decoders default to null
		}

		/// <summary>
		/// Create a new registry and initialize it with the decoders from an
		/// existing registry.
		/// </summary>
		/// <param name="registry"> the DeocderRegistry to copy. </param>


		public DecoderRegistry(DecoderRegistry registry)
		{
			filterDecoder = registry.filterDecoder;
			fillStyleDecoder = registry.fillStyleDecoder;
			morphStyleDecoder = registry.morphStyleDecoder;
			shapeDecoder = registry.shapeDecoder;
			actionDecoder = registry.actionDecoder;
			movieDecoder = registry.movieDecoder;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public DecoderRegistry copy()
		{
			return new DecoderRegistry(this);
		}

		/// <summary>
		/// Get the decoder that will be used for Filter objects. </summary>
		/// <returns> the decoder for filters. </returns>
		public SWFFactory<Filter> FilterDecoder
		{
			get => filterDecoder;
		    set => filterDecoder = value;
		}


		/// <summary>
		/// Get the decoder that will be used for FillStyle objects. </summary>
		/// <returns> the decoder for fill styles. </returns>
		public SWFFactory<FillStyle> FillStyleDecoder
		{
			get => fillStyleDecoder;
		    set => fillStyleDecoder = value;
		}


		/// <summary>
		/// Get the decoder that will be used for FillStyle objects used in morphing
		/// shapes. </summary>
		/// <returns> the decoder for morphing fill styles. </returns>
		public SWFFactory<FillStyle> MorphFillStyleDecoder
		{
			get => morphStyleDecoder;
		    set => morphStyleDecoder = value;
		}


		/// <summary>
		/// Get the decoder that will be used for ShapeRecords. </summary>
		/// <returns> the decoder for the objects in Shapes. </returns>
		public SWFFactory<ShapeRecord> ShapeDecoder
		{
			get => shapeDecoder;
		    set => shapeDecoder = value;
		}


	   /// <summary>
	   /// Get the decoder that will be used for actions. </summary>
	   /// <returns> the decoder for actions. </returns>
		public SWFFactory<Action> ActionDecoder
		{
			get => actionDecoder;
	       set => actionDecoder = value;
	   }


		/// <summary>
		/// Get the decoder that will be used for movie objects. </summary>
		/// <returns> the decoder for the main objects decoded in a Flash file. </returns>

		public SWFFactory<MovieTag> MovieDecoder
		{
			get => movieDecoder;
		    set => movieDecoder = value;
		}

	}

}