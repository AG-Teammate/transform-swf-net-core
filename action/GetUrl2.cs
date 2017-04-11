using System;
using System.Collections.Generic;
using com.flagstone.transform.coder;

/*
 * GetUrl2.java
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

namespace com.flagstone.transform.action
{
    /// <summary>
	/// The GetUrl2 action is used to either load a web page or movie clip or load or
	/// submit variable values to/from a server.
	/// 
	/// <para>
	/// It extends the functionality provided by the GetUrl action by allowing the
	/// variables defined in a movie to be submitted as form values to a server.
	/// Variables defined in a movie can also be initialised by loading a file
	/// containing variable name / value assignments.
	/// </para>
	/// 
	/// <para>
	/// GetUrl2 gets the URL and the target from the Flash Player stack. The
	/// <i>url</i> is the first argument popped from the stack and is a fully
	/// qualified uniform resource location where the movie clip or web page will be
	/// retrieved from. The second argument <i>target</i> - is either the name of a
	/// specific movie clip, e.g. _root.movieClip or the name of a level in the main
	/// movie into which a movie clip has been loaded, e.g. _level1.
	/// </para>
	/// 
	/// <para>
	/// The <i>target</i> can either be the name of the frame can be one of the
	/// following reserved words:
	/// </para>
	/// 
	/// <table class="datasheet">
	/// <tr>
	/// <td valign="top"><code>"name"</code></td>
	/// <td>opens the new page in the frame with the name defined in the HTML
	/// &lt;frame&gt; tag.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top"><code>_blank</code></td>
	/// <td>opens the new page in a new window.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top"><code>_self</code></td>
	/// <td>opens the new page in the current window.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top"><code>_top</code></td>
	/// <td>opens the new page in the top level frame of the current window.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top"><code>_parent</code></td>
	/// <td>opens the new page in the parent frame of the frame where the Flash
	/// Player id displayed.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top"><code>""</code></td>
	/// <td>(blank string) opens the new page in the current frame or window.</td>
	/// </tr>
	/// </table>
	/// 
	/// <para>
	/// Levels are virtual layers (analogous to the layers in the Display List).
	/// Higher levels are displayed in front of lower levels. The background of each
	/// level is transparent allowing movie clips on lower levels to be visible in
	/// areas not filled by the movie clip on a given level. The main movie is loaded
	/// into level 0. Movie clips are loaded into any level above this (1, 2, 124,
	/// etc.). If a movie clip is loaded into a level that already contains a movie
	/// clip then the existing clip is replaced by the new one. The level follows the
	/// general form: "_level<i>n</i>" loads a movie clip into the current movie at
	/// level <i>n</i>.
	/// </para>
	/// 
	/// </summary>
	public sealed class GetUrl2 : Action
	{

		/// <summary>
		/// Format string used in toString() method. </summary>
		private const string FORMAT = "GetUrl2: { requestType=%s}";

		/// <summary>
		/// The encoded value for a MOVIE_TO_LEVEL request. </summary>
		private const int MOVIE_LEVEL = 0;
		/// <summary>
		/// The encoded value for a MOVIE_TO_LEVEL_WITH_GET request. </summary>
		private const int MOVIE_LEVEL_GET = 1;
		/// <summary>
		/// The encoded value for a MOVIE_TO_LEVEL_WITH_POST request. </summary>
		private const int MOVIE_LEVEL_POST = 2;
		/// <summary>
		/// The encoded value for a MOVIE_TO_TARGET request. </summary>
		private const int MOVIE_TARGET = 64;
		/// <summary>
		/// The encoded value for a MOVIE_TO_TARGET_WITH_GET request. </summary>
		private const int MOVIE_TARGET_GET = 65;
		/// <summary>
		/// The encoded value for a MOVIE_TO_TARGET_WITH_POST request. </summary>
		private const int MOVIE_TARGET_POST = 66;
		/// <summary>
		/// The encoded value for a VARIABLES_TO_LEVEL request. </summary>
		private const int VAR_LEVEL = 128;
		/// <summary>
		/// The encoded value for a VARIABLES_TO_LEVEL_WITH_GET request. </summary>
		private const int VAR_LEVEL_GET = 129;
		/// <summary>
		/// The encoded value for a VARIABLES_TO_LEVEL_WITH_POST request. </summary>
		private const int VAR_LEVEL_POST = 130;
		/// <summary>
		/// The encoded value for a VARIABLES_TO_TARGET request. </summary>
		private const int VAR_TARGET = 192;
		/// <summary>
		/// The encoded value for a VARIABLES_TO_TARGET_WITH_GET request. </summary>
		private const int VAR_TARGET_GET = 193;
		/// <summary>
		/// The encoded value for a VARIABLES_TO_TARGET_WITH_POST request. </summary>
		private const int VAR_TARGET_POST = 194;

		/// <summary>
		/// Request defines the different types of request that can be submitted
		/// to a server using a GetUrl action.
		/// </summary>
		public sealed class Request
		{
			/// <summary>
			/// Load a movie without submitting the movie variables. </summary>
			public static readonly Request MOVIE_TO_LEVEL = new Request("MOVIE_TO_LEVEL", InnerEnum.MOVIE_TO_LEVEL, MOVIE_LEVEL);
			/// <summary>
			/// Load a movie submitting the movie variables using HTTP GET. </summary>
			public static readonly Request MOVIE_TO_LEVEL_WITH_GET = new Request("MOVIE_TO_LEVEL_WITH_GET", InnerEnum.MOVIE_TO_LEVEL_WITH_GET, MOVIE_LEVEL_GET);
			/// <summary>
			/// Load a movie submitting the movie variables using HTTP POST. </summary>
			public static readonly Request MOVIE_TO_LEVEL_WITH_POST = new Request("MOVIE_TO_LEVEL_WITH_POST", InnerEnum.MOVIE_TO_LEVEL_WITH_POST, MOVIE_LEVEL_POST);
			/// <summary>
			/// Load a movie or web page without submitting the movie variables. </summary>
			public static readonly Request MOVIE_TO_TARGET = new Request("MOVIE_TO_TARGET", InnerEnum.MOVIE_TO_TARGET, MOVIE_TARGET);
			/// <summary>
			/// Load a movie or web page sending variables using HTTP GET. </summary>
			public static readonly Request MOVIE_TO_TARGET_WITH_GET = new Request("MOVIE_TO_TARGET_WITH_GET", InnerEnum.MOVIE_TO_TARGET_WITH_GET, MOVIE_TARGET_GET);
			/// <summary>
			/// Load a movie or web page sending the variables using HTTP POST. </summary>
			public static readonly Request MOVIE_TO_TARGET_WITH_POST = new Request("MOVIE_TO_TARGET_WITH_POST", InnerEnum.MOVIE_TO_TARGET_WITH_POST, MOVIE_TARGET_POST);
			/// <summary>
			/// Load variables without submitting the movie variables. </summary>
			public static readonly Request VARIABLES_TO_LEVEL = new Request("VARIABLES_TO_LEVEL", InnerEnum.VARIABLES_TO_LEVEL, VAR_LEVEL);
			/// <summary>
			/// Load variables submitting the movie variables using HTTP GET. </summary>
			public static readonly Request VARIABLES_TO_LEVEL_WITH_GET = new Request("VARIABLES_TO_LEVEL_WITH_GET", InnerEnum.VARIABLES_TO_LEVEL_WITH_GET, VAR_LEVEL_GET);
			/// <summary>
			/// Load variables submitting the movie variables using HTTP POST. </summary>
			public static readonly Request VARIABLES_TO_LEVEL_WITH_POST = new Request("VARIABLES_TO_LEVEL_WITH_POST", InnerEnum.VARIABLES_TO_LEVEL_WITH_POST, VAR_LEVEL_POST);
			/// <summary>
			/// Load variables without submitting the movie variables. </summary>
			public static readonly Request VARIABLES_TO_TARGET = new Request("VARIABLES_TO_TARGET", InnerEnum.VARIABLES_TO_TARGET, VAR_TARGET);
			/// <summary>
			/// Load variables submitting the movie variables using HTTP GET. </summary>
			public static readonly Request VARIABLES_TO_TARGET_WITH_GET = new Request("VARIABLES_TO_TARGET_WITH_GET", InnerEnum.VARIABLES_TO_TARGET_WITH_GET, VAR_TARGET_GET);
			/// <summary>
			/// Load variables submitting the movie variables using HTTP POST. </summary>
			public static readonly Request VARIABLES_TO_TARGET_WITH_POST = new Request("VARIABLES_TO_TARGET_WITH_POST", InnerEnum.VARIABLES_TO_TARGET_WITH_POST, VAR_TARGET_POST);

			private static readonly IList<Request> valueList = new List<Request>();

			public enum InnerEnum
			{
				MOVIE_TO_LEVEL,
				MOVIE_TO_LEVEL_WITH_GET,
				MOVIE_TO_LEVEL_WITH_POST,
				MOVIE_TO_TARGET,
				MOVIE_TO_TARGET_WITH_GET,
				MOVIE_TO_TARGET_WITH_POST,
				VARIABLES_TO_LEVEL,
				VARIABLES_TO_LEVEL_WITH_GET,
				VARIABLES_TO_LEVEL_WITH_POST,
				VARIABLES_TO_TARGET,
				VARIABLES_TO_TARGET_WITH_GET,
				VARIABLES_TO_TARGET_WITH_POST
			}

			public readonly InnerEnum innerEnumValue;
			private readonly string nameValue;
			private readonly int ordinalValue;
			private static int nextOrdinal;

			/// <summary>
			/// Table used to decode encoded values into enum values. </summary>
			internal static readonly IDictionary<int?, Request> TABLE;

			static Request()
			{
				TABLE = new Dictionary<int?, Request>();

				foreach (Request property in values())
				{
					TABLE[property.value] = property;
				}

				valueList.Add(MOVIE_TO_LEVEL);
				valueList.Add(MOVIE_TO_LEVEL_WITH_GET);
				valueList.Add(MOVIE_TO_LEVEL_WITH_POST);
				valueList.Add(MOVIE_TO_TARGET);
				valueList.Add(MOVIE_TO_TARGET_WITH_GET);
				valueList.Add(MOVIE_TO_TARGET_WITH_POST);
				valueList.Add(VARIABLES_TO_LEVEL);
				valueList.Add(VARIABLES_TO_LEVEL_WITH_GET);
				valueList.Add(VARIABLES_TO_LEVEL_WITH_POST);
				valueList.Add(VARIABLES_TO_TARGET);
				valueList.Add(VARIABLES_TO_TARGET_WITH_GET);
				valueList.Add(VARIABLES_TO_TARGET_WITH_POST);
			}

			/// <summary>
			/// Used to generate the Request from its encoded value.
			/// </summary>
			/// <param name="requestType"> the value for an encoded Request.
			/// </param>
			/// <returns> the decoded Request type. </returns>


			internal static Request fromInt(int requestType)
			{
				return TABLE[requestType];
			}

			/// <summary>
			/// The value represented the request when encoded. </summary>
			internal readonly int value;

			/// <summary>
			/// Creates a Request representing an encoded value.
			/// </summary>
			/// <param name="encodedValue"> the value representing the encoded Request. </param>


			internal Request(string name, InnerEnum innerEnum, int encodedValue)
			{
				value = encodedValue;

				nameValue = name;
				ordinalValue = nextOrdinal++;
				innerEnumValue = innerEnum;
			}

			/// <summary>
			/// Get the value used to represent the request when it is encoded.
			/// </summary>
			/// <returns> the value for the encoded request. </returns>
			public int Value => value;

		    public static IList<Request> values()
			{
				return valueList;
			}

			public int ordinal()
			{
				return ordinalValue;
			}

			public override string ToString()
			{
				return nameValue;
			}

			public static Request valueOf(string name)
			{
				foreach (Request enumInstance in valueList)
				{
					if (enumInstance.nameValue == name)
					{
						return enumInstance;
					}
				}
				throw new ArgumentException(name);
			}
		}

		/// <summary>
		/// The value represented the request when encoded. </summary>
		
		private readonly int request;

		/// <summary>
		/// Creates and initialises a GetUrl2 action using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>



		public GetUrl2(SWFDecoder coder)
		{
			coder.readUnsignedShort();
			request = coder.readByte();
		}

		/// <summary>
		/// Creates a GetUrl2 using the specified request type.
		/// </summary>
		/// <param name="req">
		///            the type of request to be performed. </param>


		public GetUrl2(Request req)
		{
			request = req.value;
		}

		/// <summary>
		/// Creates and initialises a GetUrl2 action using the values
		/// copied from another GetUrl2 action.
		/// </summary>
		/// <param name="object">
		///            a GetUrl2 action from which the values will be
		///            copied. </param>


		public GetUrl2(GetUrl2 @object)
		{
			request = @object.request;
		}

		/// <summary>
		/// Gets the request that will be executed by this action.
		/// </summary>
		/// <returns> the type of Request that will be performed. </returns>
		public Request getRequest()
		{
			return Request.fromInt(request);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public GetUrl2 copy()
		{
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{
			return ObjectExtensions.FormatJava(FORMAT, request);
		}

		/// <summary>
		/// {@inheritDoc} </summary>


		public int prepareToEncode(Context context)
		{
			return Coder.ACTION_HEADER + 1;
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public void encode(SWFEncoder coder, Context context)
		{
			coder.writeByte(ActionTypes.GET_URL_2);
			coder.writeShort(1);
			coder.writeByte(request);
		}
	}

}