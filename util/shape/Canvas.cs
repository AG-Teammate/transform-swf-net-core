using System;
using System.Collections.Generic;
using com.flagstone.transform.datatype;
using com.flagstone.transform.fillstyle;
using com.flagstone.transform.linestyle;
using com.flagstone.transform.shape;

/*
 * Canvas.java
 * Transform
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

namespace com.flagstone.transform.util.shape
{
    /// <summary>
	/// <para>
	/// The Canvas class is used to create shape definitions. Arbitrary paths can be
	/// created using a series of move, line or curve segments. Drawing operations
	/// using both absolute coordinates and coordinates relative to the current point
	/// (updated after every operation) are supported.
	/// </para>
	/// 
	/// <para>
	/// For curves both cubic and quadratic curves are supported. Flash only supports
	/// quadratic curves so cubic curves are approximated by a series of line
	/// segments using (converting cubic to quadratic curves is mathematically
	/// difficult). The smoothness of cubic curves is controlled by the flatness
	/// attribute which can be used to limit the number of line segments that are
	/// drawn.
	/// </para>
	/// 
	/// <para>
	/// As a path is drawn the maximum and minimum x and y coordinates are recorded
	/// so that the bounding rectangle that completely encloses the shape can be
	/// defined. This is used when creating shape definitions using the DefineShape,
	/// DefineShape2 or DefineShape3 classes.
	/// </para>
	/// <para>
	/// 
	/// </para>
	/// <para>
	/// When drawing paths whether coordinates are specified in twips or pixels is
	/// set when the Canvas object is created. When specifying coordinates in pixels
	/// all coordinates are converted internally to twips to perform the actual
	/// drawing.
	/// </para>
	/// </summary>


	public sealed class Canvas
	{
		/// <summary>
		/// Value used in the algorithm to convert quadratic Bezier curves in to
		/// a set of straight lines that approximate the curve.
		/// </summary>
		private const double FLATTEN_LIMIT = 0.25;
		/// <summary>
		/// Number of twips in a pixel. </summary>
		private const int TWIPS_PER_PIXEL = 20;

		/// <summary>
		/// Index of the start point in the array of cubic Bezier points. </summary>
		private const int START = 0;
		/// <summary>
		/// Index of the first control point in the array of cubic points. </summary>
		private const int CTRLA = 1;
		/// <summary>
		/// Index of the second control point in the array of cubic points. </summary>
		private const int CTRLB = 2;
		/// <summary>
		/// Index of the anchor point in the array of cubic points. </summary>
		private const int ANCHOR = 3;
		/// <summary>
		/// Divisor for averaging the distance between points. </summary>
		private const int MID = 2;
		/// <summary>
		/// Number of points used to define a cubic Bezier curve. </summary>
		private const int CUBIC_POINTS = 4;
		/// <summary>
		/// Factor used to calculate the control point for a quadratic curve. </summary>
		private const double CTRL_AVG = 2.0;
		/// <summary>
		/// Factor used to calculate the anchor point for a quadratic curve. </summary>
		private const double ANCHOR_AVG = 3.0;

		/// <summary>
		/// Whether coordinate used to draw a path a specified in pixels. </summary>
		
		private bool pixels;
		/// <summary>
		/// Indicates whether a path is currently being drawn. </summary>
		
		private bool pathInProgress;

		/// <summary>
		/// X coordinates of a cubic Bezier curve. </summary>
		
		private readonly double[] cubicX = new double[CUBIC_POINTS];
		/// <summary>
		/// Y coordinates of a cubic Bezier curve. </summary>
		
		private readonly double[] cubicY = new double[CUBIC_POINTS];

		/// <summary>
		/// The x-coordinate of the initial point on the path. </summary>
		
		private int initialX;
		/// <summary>
		/// The y-coordinate of the initial point on the path. </summary>
		
		private int initialY;

		/// <summary>
		/// The x-coordinate of the current point on the path. </summary>
		
		private int currentX;
		/// <summary>
		/// The y-coordinate of the current point on the path. </summary>
		
		private int currentY;
		/// <summary>
		/// The x-coordinate for the last control point when drawing a curve. </summary>
		
		private int controlX;
		/// <summary>
		/// The y-coordinate for the last control point when drawing a curve. </summary>
		
		private int controlY;

		/// <summary>
		/// The minimum x-coordinate, accounting for line width. </summary>
		
		private int minX;
		/// <summary>
		/// The minimum y-coordinate, accounting for line width. </summary>
		
		private int minY;
		/// <summary>
		/// The maximum x-coordinate, accounting for line width. </summary>
		
		private int maxX;
		/// <summary>
		/// The maximum y-coordinate, accounting for line width. </summary>
		
		private int maxY;
		/// <summary>
		/// The current line width. </summary>
		
		private int lineWidth;
		/// <summary>
		/// The list of ShapeRecords make make up the current path. </summary>
		
		private readonly IList<ShapeRecord> objects;
		/// <summary>
		/// The list of line styles available. </summary>
		
		private readonly IList<LineStyle> lineStyles;
		/// <summary>
		/// The list of fill styles available. </summary>
		
		private readonly IList<FillStyle> fillStyles;

		/// <summary>
		/// Creates a new Canvas object with no path defined.
		/// </summary>
		public Canvas()
		{
			objects = new List<ShapeRecord>();
			lineStyles = new List<LineStyle>();
			fillStyles = new List<FillStyle>();
		}

		/// <summary>
		/// Are the coordinates used when drawing a path are expressed in
		/// pixels (true) or twips (false). </summary>
		/// <returns> true if coordinates are expressed in pixels, false if
		/// they are twips. </returns>
		public bool Pixels
		{
			get => pixels;
		    set => pixels = value;
		}


		/// <summary>
		/// Generates the bounding box that encloses the current path.
		/// </summary>
		/// <returns> the bounding box that encloses the current shape. </returns>
		public Bounds Bounds => new Bounds(minX, minY, maxX, maxY);

	    /// <summary>
		/// Get a copy of the list of line styles.
		/// </summary>
		/// <returns> the list of line styles. </returns>
		public IList<LineStyle> LineStyles
		{
			get
			{


				IList<LineStyle> list = new List<LineStyle>(lineStyles.Count);
    
				foreach (LineStyle style in lineStyles)
				{
					list.Add(style.copy());
				}
    
				return list;
			}
		}

		/// <summary>
		/// Get a copy of the list of fill styles.
		/// </summary>
		/// <returns> the list of fill styles. </returns>
		public IList<FillStyle> FillStyles
		{
			get
			{


				IList<FillStyle> list = new List<FillStyle>(fillStyles.Count);
    
				foreach (FillStyle style in fillStyles)
				{
					list.Add(style.copy());
				}
    
				return list;
			}
		}

		/// <summary>
		/// Returns the Shape object containing the objects used to draw the current
		/// path.
		/// </summary>
		/// <returns> an Shape object contain the Line, Curve and ShapeStyle objects
		///         used to construct the current path. </returns>
		public Shape Shape
		{
			get
			{


				IList<ShapeRecord> list = new List<ShapeRecord>(objects.Count);
    
				foreach (ShapeRecord record in objects)
				{
					list.Add(record.copy());
				}
				return new Shape(list);
			}
		}

		/// <summary>
		/// Set the style used to draw lines.
		/// </summary>
		/// <param name="style">
		///            a line style. </param>


		public LineStyle1 LineStyle
		{
			set
			{
				int index;
    
				if (lineStyles.Contains(value))
				{
					index = lineStyles.IndexOf(value);
				}
				else
				{
					index = lineStyles.Count;
					lineStyles.Add(value.copy());
				}
				lineWidth = value.Width;
				objects.Add((new ShapeStyle()).setLineStyle(index + 1));
			}
		}

		/// <summary>
		/// Set the style used to fill enclosed areas.
		/// </summary>
		/// <param name="style">
		///            a fill style. </param>


		public FillStyle FillStyle
		{
			set
			{
				int index;
    
				if (fillStyles.Contains(value))
				{
					index = fillStyles.IndexOf(value);
				}
				else
				{
					index = fillStyles.Count;
					fillStyles.Add(value.copy());
				}
				objects.Add((new ShapeStyle()).setFillStyle(index + 1));
			}
		}

		/// <summary>
		/// Set the style used to fill overlapping enclosed areas.
		/// </summary>
		/// <param name="style">
		///            a fill style. </param>


		public FillStyle AltStyle
		{
			set
			{
				int index;
    
				if (fillStyles.Contains(value))
				{
					index = fillStyles.IndexOf(value);
				}
				else
				{
					index = fillStyles.Count;
					fillStyles.Add(value.copy());
				}
				objects.Add((new ShapeStyle()).setAltFillStyle(index + 1));
			}
		}

		/// <summary>
		/// Generates a shape containing the current path and styles.
		/// 
		/// The shape is constructed with copies of the line and fill styles and the
		/// shape representing the path drawn. This allows the number of styles to be
		/// changed without affecting previously created shapes.
		/// </summary>
		/// <param name="identifier">
		///            an unique identifier for the shape. </param>
		/// <returns> this object. </returns>


		public DefineShape2 defineShape(int identifier)
		{
			return new DefineShape2(identifier, Bounds, FillStyles, LineStyles, Shape);
		}

		/// <summary>
		/// Generates a transparent shape containing the current path and styles.
		/// 
		/// The shape is constructed with copies of the line and fille styles and the
		/// shape representing the path drawn. This allows the number of styles to be
		/// changed without affecting previously created shapes.
		/// </summary>
		/// <param name="identifier">
		///            an unique identifier for the shape. </param>
		/// <returns> this object. </returns>


		public DefineShape3 defineTransparentShape(int identifier)
		{
			return new DefineShape3(identifier, Bounds, FillStyles, LineStyles, Shape);
		}

		/// <summary>
		/// Creates a new path, discarding any path elements drawn.
		/// </summary>
		public void clear()
		{
			pathInProgress = false;
			setInitial(0, 0);
			setCurrent(0, 0);
			setControl(0, 0);
			setBounds(0, 0, 0, 0);
			lineStyles.Clear();
			fillStyles.Clear();
			objects.Clear();
			lineWidth = 0;
		}

		/// <summary>
		/// Closes the current path by drawing a line from the current point to the
		/// starting point of the path.
		/// </summary>
		public void close()
		{


			int deltaX = initialX - currentX;


			int deltaY = initialY - currentY;

			if ((deltaX != 0) || (deltaY != 0))
			{
				objects.Add(new Line(deltaX, deltaY));
			}

			setCurrent(initialX, initialY);
			pathInProgress = false;
		}

		/// <summary>
		/// Move to the point (x,y).
		/// </summary>
		/// <param name="xCoord">
		///            the x-coordinate of the point to move to. </param>
		/// <param name="yCoord">
		///            the y-coordinate of the point to move to. </param>


		public void move(int xCoord, int yCoord)
		{


			int pointX = pixels ? xCoord * TWIPS_PER_PIXEL : xCoord;


			int pointY = pixels ? yCoord * TWIPS_PER_PIXEL : yCoord;

			objects.Add((new ShapeStyle()).setMove(pointX, pointY));

			setControl((currentX + pointX) / 2, (currentY + pointY) / 2);
			setCurrent(pointX, pointY);
			setInitial(pointX, pointY);
		}

		/// <summary>
		/// Move to the point (x,y). Use only when creating font definitions.
		/// </summary>
		/// <param name="xCoord">
		///            the x-coordinate of the point to move to. </param>
		/// <param name="yCoord">
		///            the y-coordinate of the point to move to. </param>


		public void moveForFont(int xCoord, int yCoord)
		{


			int pointX = pixels ? xCoord * TWIPS_PER_PIXEL : xCoord;


			int pointY = pixels ? yCoord * TWIPS_PER_PIXEL : yCoord;


			ShapeStyle style = (new ShapeStyle()).setMove(pointX, pointY);

			if (objects.Count == 0)
			{
				style.FillStyle = 1;
			}

			objects.Add(style);

			setControl((currentX + pointX) / 2, (currentY + pointY) / 2);
			setCurrent(pointX, pointY);
			setInitial(pointX, pointY);
		}

		/// <summary>
		/// Move relative to the current point.
		/// </summary>
		/// <param name="xCoord">
		///            the distance along the x-axis. </param>
		/// <param name="yCoord">
		///            the distance along the y-axis. </param>


		public void rmove(int xCoord, int yCoord)
		{


			int pointX = pixels ? xCoord * TWIPS_PER_PIXEL : xCoord;


			int pointY = pixels ? yCoord * TWIPS_PER_PIXEL : yCoord;

			objects.Add((new ShapeStyle()).setMove(pointX + currentX, pointY + currentY));

			setControl(currentX + pointX / 2, currentY + pointY / 2);
			setCurrent(currentX + pointX, currentY + pointY);
		}

		/// <summary>
		/// draw a line from the current point to the point (x,y).
		/// </summary>
		/// <param name="xCoord">
		///            the x-coordinate of the end of the line. </param>
		/// <param name="yCoord">
		///            the y-coordinate of the end of the line. </param>


		public void line(int xCoord, int yCoord)
		{


			int pointX = (pixels ? xCoord * TWIPS_PER_PIXEL : xCoord) - currentX;


			int pointY = (pixels ? yCoord * TWIPS_PER_PIXEL : yCoord) - currentY;

			objects.Add(new Line(pointX, pointY));

			if (!pathInProgress)
			{
				setInitial(currentX, currentY);
				pathInProgress = true;
			}
			setControl(currentX + pointX / 2, currentY + pointY / 2);
			setCurrent(currentX + pointX, currentY + pointY);
		}

		/// <summary>
		/// Draw a line relative to the current point.
		/// </summary>
		/// <param name="xCoord">
		///            the distance along the x-axis to the end of the line. </param>
		/// <param name="yCoord">
		///            the distance along the y-axis to the end of the line. </param>


		public void rline(int xCoord, int yCoord)
		{


			int pointX = pixels ? xCoord * TWIPS_PER_PIXEL : xCoord;


			int pointY = pixels ? yCoord * TWIPS_PER_PIXEL : yCoord;

			objects.Add(new Line(pointX, pointY));

			if (!pathInProgress)
			{
				setInitial(currentX, currentY);
				pathInProgress = true;
			}
			setControl(currentX + pointX / 2, currentY + pointY / 2);
			setCurrent(currentX + pointX, currentY + pointY);
		}

		/// <summary>
		/// Draw a quadratic bezier curve from the current point to the point (x,y)
		/// with the control point (x1, y1).
		/// </summary>
		/// <param name="acontrolX">
		///            the x-coordinate of the control point. </param>
		/// <param name="acontrolY">
		///            the y-coordinate of the control point. </param>
		/// <param name="aanchorX">
		///            the x-coordinate of the end of the curve. </param>
		/// <param name="aanchorY">
		///            the y-coordinate of the end of the curve. </param>


		public void curve(int acontrolX, int acontrolY, int aanchorX, int aanchorY)
		{
			int rcontrolX;
			int rcontrolY;
			int ranchorX;
			int ranchorY;

			if (pixels)
			{
				rcontrolX = acontrolX * TWIPS_PER_PIXEL - currentX;
				rcontrolY = acontrolY * TWIPS_PER_PIXEL - currentY;
				ranchorX = aanchorX * TWIPS_PER_PIXEL - currentX - rcontrolX;
				ranchorY = aanchorY * TWIPS_PER_PIXEL - currentY - rcontrolY;
			}
			else
			{
				rcontrolX = acontrolX - currentX;
				rcontrolY = acontrolY - currentY;
				ranchorX = aanchorX - currentX - rcontrolX;
				ranchorY = aanchorY - currentY - rcontrolY;
			}

			objects.Add(new Curve(rcontrolX, rcontrolY, ranchorX, ranchorY));

			if (!pathInProgress)
			{
				setInitial(currentX, currentY);
				pathInProgress = true;
			}
			setControl(currentX + rcontrolX, currentY + rcontrolY);
			setCurrent(currentX + rcontrolX + ranchorX, currentY + rcontrolY + ranchorY);
		}

		/// <summary>
		/// Draw a quadratic bezier curve relative to the current point to the point.
		/// </summary>
		/// <param name="rcontrolX">
		///            the distance along the x-axis from the current point to the
		///            control point. </param>
		/// <param name="rcontrolY">
		///            the distance along the y-axis from the current point to the
		///            control point. </param>
		/// <param name="ranchorX">
		///            the distance along the x-axis from the current point to the
		///            end of the curve. </param>
		/// <param name="ranchorY">
		///            the distance along the y-axis from the current point to the
		///            end of the curve. </param>


		public void rcurve(int rcontrolX, int rcontrolY, int ranchorX, int ranchorY)
		{
			int px1;
			int py1;
			int px2;
			int py2;

			if (pixels)
			{
				px1 = rcontrolX * TWIPS_PER_PIXEL;
				py1 = rcontrolY * TWIPS_PER_PIXEL;
				px2 = ranchorX * TWIPS_PER_PIXEL;
				py2 = ranchorY * TWIPS_PER_PIXEL;
			}
			else
			{
				px1 = rcontrolX;
				py1 = rcontrolY;
				px2 = ranchorX;
				py2 = ranchorY;
			}

			objects.Add(new Curve(px1, py1, px2, py2));

			if (!pathInProgress)
			{
				setInitial(currentX, currentY);
				pathInProgress = true;
			}

			setControl(currentX + px1, currentY + py1);
			setCurrent(currentX + px1 + px2, currentY + py1 + py2);
		}

		/// <summary>
		/// Draw a cubic bezier curve from the current point to the point (x,y) with
		/// the off-curve control points (x1, y1) and (x2, y2).
		/// 
		/// IMPORTANT: Converting cubic bezier curves to the quadratic bezier curves
		/// supported by Flash is mathematically difficult. The cubic curve is
		/// approximated by a series of straight line segments.
		/// </summary>
		/// <param name="cax">
		///            the x-coordinate of the first control point. </param>
		/// <param name="cay">
		///            the y-coordinate of the first control point. </param>
		/// <param name="cbx">
		///            the x-coordinate of the second control point. </param>
		/// <param name="cby">
		///            the y-coordinate of the second control point. </param>
		/// <param name="anx">
		///            the x-coordinate of the end of the curve. </param>
		/// <param name="any">
		///            the y-coordinate of the end of the curve. </param>


		public void curve(int cax, int cay, int cbx, int cby, int anx, int any)
		{
			cubicX[0] = currentX;
			cubicY[0] = currentY;

			if (pixels)
			{
				cubicX[CTRLA] = cax * TWIPS_PER_PIXEL;
				cubicY[CTRLA] = cay * TWIPS_PER_PIXEL;
				cubicX[CTRLB] = cbx * TWIPS_PER_PIXEL;
				cubicY[CTRLB] = cby * TWIPS_PER_PIXEL;
				cubicX[ANCHOR] = anx * TWIPS_PER_PIXEL;
				cubicY[ANCHOR] = any * TWIPS_PER_PIXEL;
			}
			else
			{
				cubicX[CTRLA] = cax;
				cubicY[CTRLA] = cay;
				cubicX[CTRLB] = cbx;
				cubicY[CTRLB] = cby;
				cubicX[ANCHOR] = anx;
				cubicY[ANCHOR] = any;
			}
			flatten();
		}

		/// <summary>
		/// Draw a cubic bezier curve relative to the current point.
		/// 
		/// IMPORTANT: Converting cubic bezier curves to the quadratic bezier curves
		/// supported by Flash is mathematically difficult. The cubic curve is
		/// approximated by a series of straight line segments.
		/// </summary>
		/// <param name="controlAX">
		///            the distance along the x-axis from the current point to the
		///            first control point. </param>
		/// <param name="controlAY">
		///            the distance along the y-axis from the current point to the
		///            first control point. </param>
		/// <param name="controlBX">
		///            the distance along the x-axis from the current point to the
		///            second control point. </param>
		/// <param name="controlBY">
		///            the distance along the y-axis from the current point to the
		///            second control point. </param>
		/// <param name="anchorX">
		///            the distance along the x-axis from the current point to the
		///            end of the curve. </param>
		/// <param name="anchorY">
		///            the distance along the y-axis from the current point to the
		///            end of the curve. </param>


		public void rcurve(int controlAX, int controlAY, int controlBX, int controlBY, int anchorX, int anchorY)
		{
			cubicX[0] = currentX;
			cubicY[0] = currentY;

			if (pixels)
			{
				cubicX[CTRLA] = currentX + controlAX * TWIPS_PER_PIXEL;
				cubicY[CTRLA] = currentY + controlAY * TWIPS_PER_PIXEL;
				cubicX[CTRLB] = currentX + controlBX * TWIPS_PER_PIXEL;
				cubicY[CTRLB] = currentY + controlBY * TWIPS_PER_PIXEL;
				cubicX[ANCHOR] = currentX + anchorX * TWIPS_PER_PIXEL;
				cubicY[ANCHOR] = currentY + anchorY * TWIPS_PER_PIXEL;
			}
			else
			{
				cubicX[CTRLA] = currentX + controlAX;
				cubicY[CTRLA] = currentY + controlAY;
				cubicX[CTRLB] = currentX + controlBX;
				cubicY[CTRLB] = currentY + controlBY;
				cubicX[ANCHOR] = currentX + anchorX;
				cubicY[ANCHOR] = currentY + anchorY;
			}

			flatten();
		}

		/// <summary>
		/// Draw a quadratic bezier curve from the current point to the point (x,y)
		/// using the control point for the previously drawn curve.
		/// 
		/// If no curve has been drawn previously then a control point midway along
		/// the previous line or move is used.
		/// </summary>
		/// <param name="xCoord">
		///            the x-coordinate of the end of the curve. </param>
		/// <param name="yCoord">
		///            the y-coordinate of the end of the curve. </param>


		public void reflect(int xCoord, int yCoord)
		{


			int rcontrolX = currentX - controlX;


			int rcontrolY = currentY - controlY;



			int pointX = (pixels ? xCoord * TWIPS_PER_PIXEL : xCoord) - currentX;


			int pointY = (pixels ? yCoord * TWIPS_PER_PIXEL : yCoord) - currentY;

			objects.Add(new Curve(rcontrolX, rcontrolY, pointX, pointY));

			if (!pathInProgress)
			{
				setInitial(currentX, currentY);
				pathInProgress = true;
			}

			setControl(rcontrolX + currentX, rcontrolY + currentY);
			setCurrent(pointX + currentX, pointY + currentY);
		}

		/// <summary>
		/// Draw a quadratic bezier curve relative to the current point to the point
		/// using the control point for the previously drawn curve.
		/// 
		/// If no curve has been drawn previously then a control point midway along
		/// the previous line or move is used.
		/// </summary>
		/// <param name="xCoord">
		///            the distance along the x-axis from the current point to the
		///            end of the curve. </param>
		/// <param name="yCoord">
		///            the distance along the y-axis from the current point to the
		///            end of the curve. </param>


		public void rreflect(int xCoord, int yCoord)
		{


			int rcontrolX = currentX - controlX;


			int rcontrolY = currentY - controlY;



			int pointX = pixels ? xCoord * TWIPS_PER_PIXEL : xCoord;


			int pointY = pixels ? yCoord * TWIPS_PER_PIXEL : yCoord;

			objects.Add(new Curve(rcontrolX, rcontrolY, pointX, pointY));

			if (!pathInProgress)
			{
				setInitial(currentX, currentY);
				pathInProgress = true;
			}

			setControl(rcontrolX + currentX, rcontrolY + currentY);
			setCurrent(pointX + currentX, pointY + currentY);
		}

		/// <summary>
		/// Draw a cubic bezier curve from the current point to the point (x,y). The
		/// first control point is the one defined for the previously drawn curve.
		/// The second control point is the coordinates (x2, y2).
		/// 
		/// If no curve has been drawn previously then a control point midway along
		/// the previous line or move is used.
		/// </summary>
		/// <param name="ctrlX">
		///            the x-coordinate of the control point. </param>
		/// <param name="ctrlY">
		///            the y-coordinate of the control point. </param>
		/// <param name="anchorX">
		///            the x-coordinate of the end of the curve. </param>
		/// <param name="anchorY">
		///            the y-coordinate of the end of the curve. </param>


		public void reflect(int ctrlX, int ctrlY, int anchorX, int anchorY)
		{


			int acontrolX = currentX - controlX;


			int acontrolY = currentY - controlY;

			int bcontrolX;
			int bcontrolY;

			int pointX;
			int pointY;

			if (pixels)
			{
				bcontrolX = ctrlX * TWIPS_PER_PIXEL - currentX;
				bcontrolY = ctrlY * TWIPS_PER_PIXEL - currentY;
				pointX = anchorX * TWIPS_PER_PIXEL - currentX;
				pointY = anchorY * TWIPS_PER_PIXEL - currentY;
			}
			else
			{
				bcontrolX = ctrlX - currentX;
				bcontrolY = ctrlY - currentY;
				pointX = anchorX - currentX;
				pointY = anchorY - currentY;
			}

			rcurve(acontrolX, acontrolY, bcontrolX, bcontrolY, pointX, pointY);
		}

		/// <summary>
		/// Draw a cubic bezier curve relative to the current point. The first
		/// control point is the one defined for the previously drawn curve. The
		/// second control point is the relative point (x2, y2).
		/// 
		/// If no curve has been drawn previously then a control point midway along
		/// the previous line or move is used.
		/// </summary>
		/// <param name="ctrlX">
		///            the distance along the x-axis from the current point to the
		///            second control point. </param>
		/// <param name="ctrlY">
		///            the distance along the y-axis from the current point to the
		///            second control point. </param>
		/// <param name="anchorX">
		///            the distance along the x-axis from the current point to the
		///            end of the curve. </param>
		/// <param name="anchorY">
		///            the distance along the y-axis from the current point to the
		///            end of the curve. </param>


		public void rreflect(int ctrlX, int ctrlY, int anchorX, int anchorY)
		{


			int acontrolX = currentX - controlX;


			int acontrolY = currentY - controlY;

			int bcontrolX;
			int bcontrolY;
			int pointX;
			int pointY;

			if (pixels)
			{
				bcontrolX = ctrlX * TWIPS_PER_PIXEL;
				bcontrolY = ctrlY * TWIPS_PER_PIXEL;
				pointX = anchorX * TWIPS_PER_PIXEL;
				pointY = anchorY * TWIPS_PER_PIXEL;
			}
			else
			{
				bcontrolX = ctrlX;
				bcontrolY = ctrlY;
				pointX = anchorX;
				pointY = anchorY;
			}

			rcurve(acontrolX, acontrolY, bcontrolX, bcontrolY, pointX, pointY);
		}

		/// <summary>
		/// Draws a closed shape with vertices defines by pairs of coordinates from
		/// the array argument. The first pair of points in the array specifies a
		/// move. Line segments a drawn relative to the current point which is
		/// updated after each segment is drawn.
		/// 
		/// If the number of points is an odd number then the last point will be
		/// ignored.
		/// </summary>
		/// <param name="points">
		///            and array of coordinate pairs. The first pair of points
		///            defines the coordinates of a move operation, successive pairs
		///            define the coordinates for relative lines. </param>


		public void rpolygon(int[] points)
		{
			int length = points.Length;

			if (length % 2 == 1)
			{
				length -= 1;
			}

			rmove(points[0], points[1]);

			for (int i = 2; i < length; i += 2)
			{
				rline(points[i], points[i + 1]);
			}

			close();
		}

		/// <summary>
		/// Draws a closed shape with vertices defines by pairs of coordinates from
		/// the array argument. The first pair of points in the array specifies a
		/// move. Line segments a drawn using absolute coordinates. The current point
		/// which is updated after each segment is drawn.
		/// 
		/// If the number of points is an odd number then the last point will be
		/// ignored.
		/// </summary>
		/// <param name="points">
		///            and array of coordinate pairs. The first pair of points
		///            defines the coordinates of a move operation, successive pairs
		///            define the coordinates of the lines. </param>


		public void polygon(int[] points)
		{
			int length = points.Length;

			if (length % 2 == 1)
			{
				length -= 1;
			}

			move(points[0], points[1]);

			for (int i = 2; i < length; i += 2)
			{
				line(points[i], points[i + 1]);
			}

			close();
		}

		/// <summary>
		/// Set the initial point. </summary>
		/// <param name="xCoord"> the x-coordinate of the initial point. </param>
		/// <param name="yCoord"> the y-coordinate of the initial point. </param>


		private void setInitial(int xCoord, int yCoord)
		{
			initialX = xCoord;
			initialY = yCoord;
		}

		/// <summary>
		/// Set the current point. </summary>
		/// <param name="xCoord"> the x-coordinate of the current point. </param>
		/// <param name="yCoord"> the y-coordinate of the current point. </param>


		private void setCurrent(int xCoord, int yCoord)
		{
			currentX = xCoord;
			currentY = yCoord;

			if ((xCoord - lineWidth / 2) < minX)
			{
				minX = xCoord - lineWidth / 2;
			}
			if ((yCoord - lineWidth / 2) < minY)
			{
				minY = yCoord - lineWidth / 2;
			}
			if ((xCoord + lineWidth / 2) > maxX)
			{
				maxX = xCoord + lineWidth / 2;
			}
			if ((yCoord + lineWidth / 2) > maxY)
			{
				maxY = yCoord + lineWidth / 2;
			}
		}

		/// <summary>
		/// Set the control point on a quadratic curve. </summary>
		/// <param name="xCoord"> the x-coordinate of the control point. </param>
		/// <param name="yCoord"> the y-coordinate of the control point. </param>


		private void setControl(int xCoord, int yCoord)
		{
			controlX = xCoord;
			controlY = yCoord;

			if ((xCoord - lineWidth / 2) < minX)
			{
				minX = xCoord - lineWidth / 2;
			}
			if ((yCoord - lineWidth / 2) < minY)
			{
				minY = yCoord - lineWidth / 2;
			}
			if ((xCoord + lineWidth / 2) > maxX)
			{
				maxX = xCoord + lineWidth / 2;
			}
			if ((yCoord + lineWidth / 2) > maxY)
			{
				maxY = yCoord + lineWidth / 2;
			}
		}

		/// <summary>
		/// Set the bounds for the shape being drawn.
		/// </summary>
		/// <param name="xmin">
		///            x-coordinate of the top left corner. </param>
		/// <param name="ymin">
		///            y-coordinate of the top left corner. </param>
		/// <param name="xmax">
		///            x-coordinate of bottom right corner. </param>
		/// <param name="ymax">
		///            y-coordinate of bottom right corner. </param>


		private void setBounds(int xmin, int ymin, int xmax, int ymax)
		{
			minX = xmin;
			minY = ymin;
			maxX = xmax;
			maxY = ymax;
		}

		/// <summary>
		/// Flatten a cubic Bezier curve into a series of straight line segments.
		/// </summary>
		private void flatten()
		{


			double[] quadX = {0.0, 0.0, 0.0, 0.0};


			double[] quadY = {0.0, 0.0, 0.0, 0.0};

			double delta;
			double pointAX;
			double pointAY;
			double pointBX;
			double pointBY;

			while (true)
			{
				pointAX = CTRL_AVG * cubicX[START] + cubicX[ANCHOR] - ANCHOR_AVG * cubicX[CTRLA];
				pointAX *= pointAX;
				pointBX = CTRL_AVG * cubicX[ANCHOR] + cubicX[START] - ANCHOR_AVG * cubicX[CTRLB];
				pointBX *= pointBX;

				if (pointAX < pointBX)
				{
					pointAX = pointBX;
				}

				pointAY = CTRL_AVG * cubicY[START] + cubicY[ANCHOR] - ANCHOR_AVG * cubicY[CTRLA];
				pointAY *= pointAY;
				pointBY = CTRL_AVG * cubicY[ANCHOR] + cubicY[START] - ANCHOR_AVG * cubicY[CTRLB];
				pointBY *= pointBY;

				if (pointAY < pointBY)
				{
					pointAY = pointBY;
				}

				if ((pointAX + pointAY) < FLATTEN_LIMIT)
				{
					objects.Add(new Line((int)(cubicX[ANCHOR]) - currentX, (int)(cubicY[ANCHOR]) - currentY));
					setControl((int) cubicX[CTRLA], (int) cubicY[CTRLA]);
					setControl((int) cubicX[CTRLB], (int) cubicY[CTRLB]);
					setCurrent((int) cubicX[ANCHOR], (int) cubicY[ANCHOR]);
					break;
				}
			    quadX[ANCHOR] = cubicX[ANCHOR];
			    delta = (cubicX[CTRLA] + cubicX[CTRLB]) / MID;
			    cubicX[1] = (cubicX[START] + cubicX[CTRLA]) / MID;
			    quadX[2] = (cubicX[CTRLB] + cubicX[ANCHOR]) / MID;
			    cubicX[2] = (cubicX[CTRLA] + delta) / MID;
			    quadX[1] = (delta + quadX[CTRLB]) / MID;
			    cubicX[ANCHOR] = (cubicX[CTRLB] + quadX[CTRLA]) / MID;
			    quadX[0] = (cubicX[CTRLB] + quadX[CTRLA]) / MID;

			    quadY[ANCHOR] = cubicY[ANCHOR];
			    delta = (cubicY[CTRLA] + cubicY[CTRLB]) / MID;
			    cubicY[1] = (cubicY[START] + cubicY[CTRLA]) / MID;
			    quadY[2] = (cubicY[CTRLB] + cubicY[ANCHOR]) / MID;
			    cubicY[2] = (cubicY[CTRLA] + delta) / MID;
			    quadY[1] = (delta + quadY[CTRLB]) / MID;
			    cubicY[ANCHOR] = (cubicY[CTRLB] + quadY[CTRLA]) / MID;
			    quadY[0] = (cubicY[CTRLB] + quadY[CTRLA]) / MID;

			    flatten();

			    cubicX[START] = quadX[START];
			    cubicY[START] = quadY[START];
			    cubicX[CTRLA] = quadX[CTRLA];
			    cubicY[CTRLA] = quadY[CTRLA];
			    cubicX[CTRLB] = quadX[CTRLB];
			    cubicY[CTRLB] = quadY[CTRLB];
			    cubicX[ANCHOR] = quadX[ANCHOR];
			    cubicY[ANCHOR] = quadY[ANCHOR];
			}
		}
	}

}