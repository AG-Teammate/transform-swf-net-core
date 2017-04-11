using System.Collections.Generic;
using com.flagstone.transform.datatype;
using com.flagstone.transform.fillstyle;
using com.flagstone.transform.linestyle;

namespace com.flagstone.transform.shape
{
    /// <summary>
	/// The ShapeTag interface provides a common set of methods for accessing the
	/// bounding box, line styles, fill styles and shape of all the different
	/// shape definitions available in Flash.
	/// </summary>
	public interface ShapeTag : DefineTag
	{
		/// <summary>
		/// Get the bounding rectangle that completely enclosed the shape.
		/// </summary>
		/// <returns> the Bounds that encloses the shape </returns>
		Bounds Bounds {get;set;}
		/// <summary>
		/// Sets the bounding rectangle that encloses the shape.
		/// </summary>
		/// <param name="rect">
		///            set the bounding rectangle for the shape. Must not be null. </param>



		/// <summary>
		/// Get the list line styles.
		/// </summary>
		/// <returns> the list of line styles used in the shape. </returns>
		IList<LineStyle> LineStyles {get;set;}
		/// <summary>
		/// Sets the list of styles that will be used to draw the outline of the
		/// shape.
		/// </summary>
		/// <param name="styles">
		///            the line styles for the shape. Must not be null. </param>


		/// <summary>
		/// Add a line style.
		/// </summary>
		/// <param name="style">
		///            an instance of LineStyle. Must not be null.
		/// </param>
		/// <returns> this ShapeTag object. </returns>


		ShapeTag add(LineStyle style);
		/// <summary>
		/// Get the list fill styles.
		/// </summary>
		/// <returns> the list of fill styles used in the shape. </returns>
		IList<FillStyle> FillStyles {get;set;}
		/// <summary>
		/// Sets the list fill styles that will be used to draw the shape.
		/// </summary>
		/// <param name="styles">
		///            the fill styles for the shape. Must not be null. </param>


		/// <summary>
		/// Add a fill style.
		/// </summary>
		/// <param name="style">
		///            an instance of FillStyle. Must not be null.
		/// </param>
		/// <returns> this ShapeTag object. </returns>


		ShapeTag add(FillStyle style);
		/// <summary>
		/// Get the shape.
		/// </summary>
		/// <returns> the shape. </returns>
		Shape Shape {get;set;}
		/// <summary>
		/// Sets the shape.
		/// </summary>
		/// <param name="aShape">
		///            set the shape to be drawn. Must not be null. </param>


	}

}