using System;
using System.Collections.Generic;
using com.flagstone.transform.datatype;
using com.flagstone.transform.fillstyle;
using com.flagstone.transform.image;
using com.flagstone.transform.linestyle;
using com.flagstone.transform.shape;

/*
 * ImageUtils.java
 * Transform
 *
 * Copyright (c) 2010 Flagstone Software Ltd. All rights reserved.
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

namespace com.flagstone.transform.util.image
{
    /// <summary>
	/// ImageShape is used to generate the shape definition that is required to
	/// display images in a Flash file.
	/// </summary>
	public sealed class ImageShape
	{
		/// <summary>
		/// The number of twips in a pixel. </summary>
		private const int TWIPS_PER_PIXEL = 20;
		/// <summary>
		/// The horizontal alignment of the image. </summary>
		
		private HorizontalAlign xAlign;
		/// <summary>
		/// The vertical alignment of the image. </summary>
		
		private VerticalAlign yAlign;
		/// <summary>
		/// The style used to draw the outline, if any of the shape. </summary>
		
		private LineStyle style;

		/// <summary>
		/// Set the line style used to draw the border around the image.
		/// </summary>
		/// <param name="lineStyle"> a LineStyle. May be null if no border will be drawn. </param>


		public LineStyle Style
		{
			set => style = value.copy();
		}

		/// <summary>
		/// Set the registration point, definition the position of the image
		/// relative to the origin of the shape.
		/// </summary>
		/// <param name="halign"> the alignment along the x-axis. </param>
		/// <param name="valign"> the alignment along the y-axis. </param>


		public void setRegistration(HorizontalAlign halign, VerticalAlign valign)
		{
			xAlign = halign;
			yAlign = valign;
		}

		/// <summary>
		/// Generates the shape definition used to display an image using the
		/// predefined registration point and border style.
		/// </summary>
		/// <param name="uid">
		///            an unique identifier that is used to reference the shape
		///            definition in a Flash movie.
		/// </param>
		/// <param name="image">
		///            the image definition.
		/// </param>
		/// <returns> the shape that is used to display the image in a Flash movie. </returns>


		public ShapeTag defineShape(int uid, ImageTag image)
		{
			int xOffset;
			int yOffset;

			if (xAlign == HorizontalAlign.LEFT)
			{
				xOffset = -(image.Width >> 2);
			}
			else if (xAlign == HorizontalAlign.RIGHT)
			{
				xOffset = image.Width >> 2;
			}
			else
			{
				xOffset = 0;
			}

			if (yAlign == VerticalAlign.TOP)
			{
				yOffset = -(image.Height >> 2);
			}
			else if (yAlign == VerticalAlign.BOTTOM)
			{
				yOffset = image.Height >> 2;
			}
			else
			{
				yOffset = 0;
			}

			return defineShape(uid, image, xOffset, yOffset, style);
		}

		/// <summary>
		/// Generates the shape definition used to display an image.
		/// </summary>
		/// <param name="uid">
		///            an unique identifier that is used to reference the shape
		///            definition in a Flash movie.
		/// </param>
		/// <param name="image">
		///            the image definition.
		/// </param>
		/// <param name="xOrigin">
		///            the offset in pixels along the x-axis, relative to the top
		///            left corner of the image, where the origin (0,0) of the shape
		///            will be located.
		/// </param>
		/// <param name="yOrigin">
		///            the offset in pixels along the y-axis, relative to the top
		///            left corner of the image, where the origin (0,0) of the shape
		///            will be located.
		/// </param>
		/// <param name="border">
		///            the style drawn around the border of the image. May be null if
		///            no border is drawn.
		/// </param>
		/// <returns> the shape that is used to display the image in a Flash movie. </returns>


		public ShapeTag defineShape(int uid, ImageTag image, int xOrigin, int yOrigin, LineStyle border)
		{



			Bounds bounds = getBounds(xOrigin, yOrigin, image.Width, image.Height, border);



			Shape shape = getShape(xOrigin, yOrigin, image.Width, image.Height, border);

			ShapeTag definition;

			if (border == null || border is LineStyle1)
			{
				definition = new DefineShape3(uid, bounds, new List<FillStyle>(), new List<LineStyle>(), shape);
			}
			else
			{


				Bounds edges = getEdges(xOrigin, yOrigin, image.Width, image.Height);

				definition = new DefineShape4(uid, bounds, edges, new List<FillStyle>(), new List<LineStyle>(), shape);
			}

			if (border != null)
			{
				definition.add(border);
			}

			definition.add(getFillStyle(image.Identifier, xOrigin, yOrigin));

			return definition;
		}

		/// <summary>
		/// Get the bound box that encloses the shape taking into account the
		/// thickness of the outline. </summary>
		/// <param name="xOrigin"> the x-coordinate of the origin. </param>
		/// <param name="yOrigin"> the y-coordinate of the origin. </param>
		/// <param name="width"> the width of the image. </param>
		/// <param name="height"> the height of the image. </param>
		/// <param name="border"> the style used to draw the outline around the image. </param>
		/// <returns> the bounding box that completely encloses the shape. </returns>


		private Bounds getBounds(int xOrigin, int yOrigin, int width, int height, LineStyle border)
		{

			int lineWidth;

			if (border is LineStyle1)
			{
				lineWidth = ((LineStyle1) border).Width / 2;
			}
			else if (border is LineStyle2)
			{
				lineWidth = ((LineStyle2) border).Width / 2;
			}
			else
			{
				lineWidth = 0;
			}



			Bounds bounds = new Bounds(-xOrigin * TWIPS_PER_PIXEL - lineWidth, -yOrigin * TWIPS_PER_PIXEL - lineWidth, (width - xOrigin) * TWIPS_PER_PIXEL + lineWidth, (height - yOrigin) * TWIPS_PER_PIXEL + lineWidth);

			return bounds;
		}

		/// <summary>
		/// Get the bound box that encloses the shape. </summary>
		/// <param name="xOrigin"> the x-coordinate of the origin. </param>
		/// <param name="yOrigin"> the y-coordinate of the origin. </param>
		/// <param name="width"> the width of the image. </param>
		/// <param name="height"> the height of the image. </param>
		/// <returns> the bounding box that encloses the shape. </returns>


		private Bounds getEdges(int xOrigin, int yOrigin, int width, int height)
		{

			return new Bounds(-xOrigin * TWIPS_PER_PIXEL, -yOrigin * TWIPS_PER_PIXEL, (width - xOrigin) * TWIPS_PER_PIXEL, (height - yOrigin) * TWIPS_PER_PIXEL);

		}

		/// <summary>
		/// Get the shape used to display the image. </summary>
		/// <param name="xOrigin"> the x-coordinate of the origin. </param>
		/// <param name="yOrigin"> the y-coordinate of the origin. </param>
		/// <param name="width"> the width of the image. </param>
		/// <param name="height"> the height of the image. </param>
		/// <param name="border"> the style used to draw the outline around the image. </param>
		/// <returns> the shape definition size correctly to display the image. </returns>


		private Shape getShape(int xOrigin, int yOrigin, int width, int height, LineStyle border)
		{


			Shape shape = new Shape(new List<ShapeRecord>());

			if (style is LineStyle2)
			{


				ShapeStyle2 shapeStyle = new ShapeStyle2();
				shapeStyle.LineStyle = 1;
				shapeStyle.FillStyle = 1;
				shapeStyle.setMove(-xOrigin * TWIPS_PER_PIXEL, -yOrigin * TWIPS_PER_PIXEL);
				shape.add(shapeStyle);
			}
			else
			{


				ShapeStyle shapeStyle = new ShapeStyle();
				shapeStyle.LineStyle = border == null ? 0 : 1;
				shapeStyle.FillStyle = 1;
				shapeStyle.setMove(-xOrigin * TWIPS_PER_PIXEL, -yOrigin * TWIPS_PER_PIXEL);
				shape.add(shapeStyle);
			}

			shape.add(new Line(width * TWIPS_PER_PIXEL, 0));
			shape.add(new Line(0, height * TWIPS_PER_PIXEL));
			shape.add(new Line(-width * TWIPS_PER_PIXEL, 0));
			shape.add(new Line(0, -height * TWIPS_PER_PIXEL));
			return shape;
		}

		/// <summary>
		/// Return the fill style that references the image and scales it to the
		/// correct size. </summary>
		/// <param name="uid"> the unique identifier of the image. </param>
		/// <param name="xOrigin"> the x-coordinate of the image origin. </param>
		/// <param name="yOrigin"> the y-coordinate of the image origin. </param>
		/// <returns> the FillStyle used to display the image. </returns>


		private FillStyle getFillStyle(int uid, int xOrigin, int yOrigin)
		{



			CoordTransform transform = new CoordTransform(TWIPS_PER_PIXEL, TWIPS_PER_PIXEL, 0, 0, -xOrigin * TWIPS_PER_PIXEL, -yOrigin * TWIPS_PER_PIXEL);

			return new BitmapFill(false, false, uid, transform);
		}

	}

}