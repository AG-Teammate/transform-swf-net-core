using System.Collections.Generic;
using com.flagstone.transform.datatype;

namespace com.flagstone.transform.text
{
    /// <summary>
	/// The StaticTextTag interface provides a common set of methods for accessing
	/// the bounding box, coordinate transform and TextSpans for the different
	/// static text definition: DefineText, DefineText2.
	/// </summary>
	public interface StaticTextTag : DefineTag
	{
		/// <summary>
		/// Get the bounding rectangle that completely enclosed the text.
		/// </summary>
		/// <returns> the Bounds that encloses the text. </returns>
		Bounds Bounds {get;set;}
		/// <summary>
		/// Sets the bounding rectangle that encloses the text.
		/// </summary>
		/// <param name="rect">
		///            set the bounding rectangle for the text. Must not be null. </param>



		/// <summary>
		/// Get the coordinate transform that controls the size, location and
		/// orientation of the text when it is displayed.
		/// </summary>
		/// <returns> the coordinate transform used to position the text. </returns>
		CoordTransform Transform {get;set;}
		/// <summary>
		/// Sets the coordinate transform that changes the orientation and size of
		/// the text displayed.
		/// </summary>
		/// <param name="matrix">
		///            an CoordTransform to change the size and orientation of the
		///            text. Must not be null. </param>



		/// <summary>
		/// Get the list of text spans that define the text to be displayed.
		/// </summary>
		/// <returns> the list of text blocks. </returns>
		IList<TextSpan> Spans {get;set;}
		/// <summary>
		/// Sets the list of text spans that define the text to be displayed.
		/// </summary>
		/// <param name="list">
		///            a list of TextSpan objects that define the text to be
		///            displayed. Must not be null. </param>


	}

}