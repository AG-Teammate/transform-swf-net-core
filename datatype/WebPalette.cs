using System;
using System.Collections.Generic;

/*
 * WebPalette.java
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
namespace com.flagstone.transform.datatype
{
	/// <summary>
	/// WebPalette defined the set of colours from the Netscape Colour Table.
	/// 
	/// <style type="text/css">
	/// p.color {
	///     border: solid thin black;
	///     width:200px;
	///     padding:5px;
	///     text-align: center;
	/// }
	/// </style>
	/// </summary>
	public sealed class WebPalette
	{
		/// <summary>
		/// <p class="color" style="background-color:#F0F8FF;">0xF0F8FF</p>. </summary>
		public static readonly WebPalette ALICE_BLUE = new WebPalette("ALICE_BLUE", InnerEnum.ALICE_BLUE, 0xF0, 0xF8, 0xFF);
		/// <summary>
		/// <p class="color" style="background-color:#FAEBD7;">0xFAEBD7</p>. </summary>
		public static readonly WebPalette ANTIQUE_WHITE = new WebPalette("ANTIQUE_WHITE", InnerEnum.ANTIQUE_WHITE, 0xFA, 0xEB, 0xD7);
		/// <summary>
		/// <p class="color" style="background-color:#00FFFF;">0x00FFFF</p>. </summary>
		public static readonly WebPalette AQUA = new WebPalette("AQUA", InnerEnum.AQUA, 0x00, 0xFF, 0xFF);
		/// <summary>
		/// <p class="color" style="background-color:#7FFFD4;">0x7FFFD4</p>. </summary>
		public static readonly WebPalette AQUAMARINE = new WebPalette("AQUAMARINE", InnerEnum.AQUAMARINE, 0x7F, 0xFF, 0xD4);
		/// <summary>
		/// <p class="color" style="background-color:#F0FFFF;">0xF0FFFF</p>. </summary>
		public static readonly WebPalette AZURE = new WebPalette("AZURE", InnerEnum.AZURE, 0xF0, 0xFF, 0xFF);
		/// <summary>
		/// <p class="color" style="background-color:#F5F5DC;">0xF5F5DC</p>. </summary>
		public static readonly WebPalette BEIGE = new WebPalette("BEIGE", InnerEnum.BEIGE, 0xF5, 0xF5, 0xDC);
		/// <summary>
		/// <p class="color" style="background-color:#FFE4C4;">0xFFE4C4</p>. </summary>
		public static readonly WebPalette BISQUE = new WebPalette("BISQUE", InnerEnum.BISQUE, 0xFF, 0xE4, 0xC4);
		/// <summary>
		/// <p class="color" style="background-color:#000000;">0x000000</p>. </summary>
		public static readonly WebPalette BLACK = new WebPalette("BLACK", InnerEnum.BLACK, 0x00, 0x00, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#FFEBCD;">0xFFEBCD</p>. </summary>
		public static readonly WebPalette BLANCHED_ALMOND = new WebPalette("BLANCHED_ALMOND", InnerEnum.BLANCHED_ALMOND, 0xFF, 0xEB, 0xCD);
		/// <summary>
		/// <p class="color" style="background-color:#0000FF;">0x0000FF</p>. </summary>
		public static readonly WebPalette BLUE = new WebPalette("BLUE", InnerEnum.BLUE, 0x00, 0x00, 0xFF);
		/// <summary>
		/// <p class="color" style="background-color:#8A2BE2;">0x8A2BE2</p>. </summary>
		public static readonly WebPalette BLUE_VIOLET = new WebPalette("BLUE_VIOLET", InnerEnum.BLUE_VIOLET, 0x8A, 0x2B, 0xE2);
		/// <summary>
		/// <p class="color" style="background-color:#A52A2A;">0xA52A2A</p>. </summary>
		public static readonly WebPalette BROWN = new WebPalette("BROWN", InnerEnum.BROWN, 0xA5, 0x2A, 0x2A);
		/// <summary>
		/// <p class="color" style="background-color:#DEB887;">0xDEB887</p>. </summary>
		public static readonly WebPalette BURLYWOOD = new WebPalette("BURLYWOOD", InnerEnum.BURLYWOOD, 0xDE, 0xB8, 0x87);
		/// <summary>
		/// <p class="color" style="background-color:#5F9EA0;">0x5F9EA0</p>. </summary>
		public static readonly WebPalette CADET_BLUE = new WebPalette("CADET_BLUE", InnerEnum.CADET_BLUE, 0x5F, 0x9E, 0xA0);
		/// <summary>
		/// <p class="color" style="background-color:#7FFF00;">0x7FFF00</p>. </summary>
		public static readonly WebPalette CHARTREUSE = new WebPalette("CHARTREUSE", InnerEnum.CHARTREUSE, 0x7F, 0xFF, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#D2691E;">0xD2691E</p>. </summary>
		public static readonly WebPalette CHOCOLATE = new WebPalette("CHOCOLATE", InnerEnum.CHOCOLATE, 0xD2, 0x69, 0x1E);
		/// <summary>
		/// <p class="color" style="background-color:#FF7F50;">0xFF7F50</p>. </summary>
		public static readonly WebPalette CORAL = new WebPalette("CORAL", InnerEnum.CORAL, 0xFF, 0x7F, 0x50);
		/// <summary>
		/// <p class="color" style="background-color:#6495ED;">0x6495ED</p>. </summary>
		public static readonly WebPalette CORNFLOWER_BLUE = new WebPalette("CORNFLOWER_BLUE", InnerEnum.CORNFLOWER_BLUE, 0x64, 0x95, 0xED);
		/// <summary>
		/// <p class="color" style="background-color:#FFF8DC;">0xFFF8DC</p>. </summary>
		public static readonly WebPalette CORNSILK = new WebPalette("CORNSILK", InnerEnum.CORNSILK, 0xFF, 0xF8, 0xDC);
		/// <summary>
		/// <p class="color" style="background-color:#DC143C;">0xDC143C</p>. </summary>
		public static readonly WebPalette CRIMSON = new WebPalette("CRIMSON", InnerEnum.CRIMSON, 0xDC, 0x14, 0x3C);
		/// <summary>
		/// <p class="color" style="background-color:#00FFFF;">0x00FFFF</p>. </summary>
		public static readonly WebPalette CYAN = new WebPalette("CYAN", InnerEnum.CYAN, 0x00, 0xFF, 0xFF);
		/// <summary>
		/// <p class="color" style="background-color:#00008B;">0x00008B</p>. </summary>
		public static readonly WebPalette DARK_BLUE = new WebPalette("DARK_BLUE", InnerEnum.DARK_BLUE, 0x00, 0x00, 0x8B);
		/// <summary>
		/// <p class="color" style="background-color:#008B8B;">0x008B8B</p>. </summary>
		public static readonly WebPalette DARK_CYAN = new WebPalette("DARK_CYAN", InnerEnum.DARK_CYAN, 0x00, 0x8B, 0x8B);
		/// <summary>
		/// <p class="color" style="background-color:#B8860B;">0xB8860B</p>. </summary>
		public static readonly WebPalette DARK_GOLDENROD = new WebPalette("DARK_GOLDENROD", InnerEnum.DARK_GOLDENROD, 0xB8, 0x86, 0x0B);
		/// <summary>
		/// <p class="color" style="background-color:#A9A9A9;">0xA9A9A9</p>. </summary>
		public static readonly WebPalette DARK_GRAY = new WebPalette("DARK_GRAY", InnerEnum.DARK_GRAY, 0xA9, 0xA9, 0xA9);
		/// <summary>
		/// <p class="color" style="background-color:#006400;">0x006400</p>. </summary>
		public static readonly WebPalette DARK_GREEN = new WebPalette("DARK_GREEN", InnerEnum.DARK_GREEN, 0x00, 0x64, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#BDB76B;">0xBDB76B</p>. </summary>
		public static readonly WebPalette DARK_KHAKI = new WebPalette("DARK_KHAKI", InnerEnum.DARK_KHAKI, 0xBD, 0xB7, 0x6B);
		/// <summary>
		/// <p class="color" style="background-color:#8B008B;">0x8B008B</p>. </summary>
		public static readonly WebPalette DARK_MAGENTA = new WebPalette("DARK_MAGENTA", InnerEnum.DARK_MAGENTA, 0x8B, 0x00, 0x8B);
		/// <summary>
		/// <p class="color" style="background-color:#556B2F;">0x556B2F</p>. </summary>
		public static readonly WebPalette DARK_OLIVE_GREEN = new WebPalette("DARK_OLIVE_GREEN", InnerEnum.DARK_OLIVE_GREEN, 0x55, 0x6B, 0x2F);
		/// <summary>
		/// <p class="color" style="background-color:#FF8C00;">0xFF8C00</p>. </summary>
		public static readonly WebPalette DARK_ORANGE = new WebPalette("DARK_ORANGE", InnerEnum.DARK_ORANGE, 0xFF, 0x8C, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#9932CC;">0x9932CC</p>. </summary>
		public static readonly WebPalette DARK_ORCHID = new WebPalette("DARK_ORCHID", InnerEnum.DARK_ORCHID, 0x99, 0x32, 0xCC);
		/// <summary>
		/// <p class="color" style="background-color:#8B0000;">0x8B0000</p>. </summary>
		public static readonly WebPalette DARK_RED = new WebPalette("DARK_RED", InnerEnum.DARK_RED, 0x8B, 0x00, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#E9967A;">0xE9967A</p>. </summary>
		public static readonly WebPalette DARK_SALMON = new WebPalette("DARK_SALMON", InnerEnum.DARK_SALMON, 0xE9, 0x96, 0x7A);
		/// <summary>
		/// <p class="color" style="background-color:#8FBC8F;">0x8FBC8F</p>. </summary>
		public static readonly WebPalette DARK_SEA_GREEN = new WebPalette("DARK_SEA_GREEN", InnerEnum.DARK_SEA_GREEN, 0x8F, 0xBC, 0x8F);
		/// <summary>
		/// <p class="color" style="background-color:#483D8B;">0x483D8B</p>. </summary>
		public static readonly WebPalette DARK_SLATE_BLUE = new WebPalette("DARK_SLATE_BLUE", InnerEnum.DARK_SLATE_BLUE, 0x48, 0x3D, 0x8B);
		/// <summary>
		/// <p class="color" style="background-color:#2F4F4F;">0x2F4F4F</p>. </summary>
		public static readonly WebPalette DARK_SLATE_GRAY = new WebPalette("DARK_SLATE_GRAY", InnerEnum.DARK_SLATE_GRAY, 0x2F, 0x4F, 0x4F);
		/// <summary>
		/// <p class="color" style="background-color:#00CED1;">0x00CED1</p>. </summary>
		public static readonly WebPalette DARK_TURQUOISE = new WebPalette("DARK_TURQUOISE", InnerEnum.DARK_TURQUOISE, 0x00, 0xCE, 0xD1);
		/// <summary>
		/// <p class="color" style="background-color:#9400D3;">0x9400D3</p>. </summary>
		public static readonly WebPalette DARK_VIOLET = new WebPalette("DARK_VIOLET", InnerEnum.DARK_VIOLET, 0x94, 0x00, 0xD3);
		/// <summary>
		/// <p class="color" style="background-color:#FF1493;">0xFF1493</p>. </summary>
		public static readonly WebPalette DEEP_PINK = new WebPalette("DEEP_PINK", InnerEnum.DEEP_PINK, 0xFF, 0x14, 0x93);
		/// <summary>
		/// <p class="color" style="background-color:#00BFFF;">0x00BFFF</p>. </summary>
		public static readonly WebPalette DEEP_SKY_BLUE = new WebPalette("DEEP_SKY_BLUE", InnerEnum.DEEP_SKY_BLUE, 0x00, 0xBF, 0xFF);
		/// <summary>
		/// <p class="color" style="background-color:#696969;">0x696969</p>. </summary>
		public static readonly WebPalette DIM_GRAY = new WebPalette("DIM_GRAY", InnerEnum.DIM_GRAY, 0x69, 0x69, 0x69);
		/// <summary>
		/// <p class="color" style="background-color:#1E90FF;">0x1E90FF</p>. </summary>
		public static readonly WebPalette DODGER_BLUE = new WebPalette("DODGER_BLUE", InnerEnum.DODGER_BLUE, 0x1E, 0x90, 0xFF);
		/// <summary>
		/// <p class="color" style="background-color:#B22222;">0xB22222</p>. </summary>
		public static readonly WebPalette FIREBRICK = new WebPalette("FIREBRICK", InnerEnum.FIREBRICK, 0xB2, 0x22, 0x22);
		/// <summary>
		/// <p class="color" style="background-color:#FFFAF0;">0xFFFAF0</p>. </summary>
		public static readonly WebPalette FLORAL_WHITE = new WebPalette("FLORAL_WHITE", InnerEnum.FLORAL_WHITE, 0xFF, 0xFA, 0xF0);
		/// <summary>
		/// <p class="color" style="background-color:#228B22;">0x228B22</p>. </summary>
		public static readonly WebPalette FOREST_GREEN = new WebPalette("FOREST_GREEN", InnerEnum.FOREST_GREEN, 0x22, 0x8B, 0x22);
		/// <summary>
		/// <p class="color" style="background-color:#FF00FF;">0xFF00FF</p>. </summary>
		public static readonly WebPalette FUCHSIA = new WebPalette("FUCHSIA", InnerEnum.FUCHSIA, 0xFF, 0x00, 0xFF);
		/// <summary>
		/// <p class="color" style="background-color:#DCDCDC;">0xDCDCDC</p>. </summary>
		public static readonly WebPalette GAINSBORO = new WebPalette("GAINSBORO", InnerEnum.GAINSBORO, 0xDC, 0xDC, 0xDC);
		/// <summary>
		/// <p class="color" style="background-color:#F8F8FB;">0xF8F8FB</p>. </summary>
		public static readonly WebPalette GHOST_WHITE = new WebPalette("GHOST_WHITE", InnerEnum.GHOST_WHITE, 0xF8, 0xF8, 0xFB);
		/// <summary>
		/// <p class="color" style="background-color:#FFD700;">0xFFD700</p>. </summary>
		public static readonly WebPalette GOLD = new WebPalette("GOLD", InnerEnum.GOLD, 0xFF, 0xD7, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#DAA520;">0xDAA520</p>. </summary>
		public static readonly WebPalette GOLDENROD = new WebPalette("GOLDENROD", InnerEnum.GOLDENROD, 0xDA, 0xA5, 0x20);
		/// <summary>
		/// <p class="color" style="background-color:#808080;">0x808080</p>. </summary>
		public static readonly WebPalette GRAY = new WebPalette("GRAY", InnerEnum.GRAY, 0x80, 0x80, 0x80);
		/// <summary>
		/// <p class="color" style="background-color:#008000;">0x008000</p>. </summary>
		public static readonly WebPalette GREEN = new WebPalette("GREEN", InnerEnum.GREEN, 0x00, 0x80, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#ADFF2F;">0xADFF2F</p>. </summary>
		public static readonly WebPalette GREEN_YELLOW = new WebPalette("GREEN_YELLOW", InnerEnum.GREEN_YELLOW, 0xAD, 0xFF, 0x2F);
		/// <summary>
		/// <p class="color" style="background-color:#F0FFF0;">0xF0FFF0</p>. </summary>
		public static readonly WebPalette HONEYDEW = new WebPalette("HONEYDEW", InnerEnum.HONEYDEW, 0xF0, 0xFF, 0xF0);
		/// <summary>
		/// <p class="color" style="background-color:#FF69B4;">0xFF69B4</p>. </summary>
		public static readonly WebPalette HOT_PINK = new WebPalette("HOT_PINK", InnerEnum.HOT_PINK, 0xFF, 0x69, 0xB4);
		/// <summary>
		/// <p class="color" style="background-color:#CD5C5C;">0xCD5C5C</p>. </summary>
		public static readonly WebPalette INDIAN_RED = new WebPalette("INDIAN_RED", InnerEnum.INDIAN_RED, 0xCD, 0x5C, 0x5C);
		/// <summary>
		/// <p class="color" style="background-color:#4B0082;">0x4B0082</p>. </summary>
		public static readonly WebPalette INDIGO = new WebPalette("INDIGO", InnerEnum.INDIGO, 0x4B, 0x00, 0x82);
		/// <summary>
		/// <p class="color" style="background-color:#FFFFF0;">0xFFFFF0</p>. </summary>
		public static readonly WebPalette IVORY = new WebPalette("IVORY", InnerEnum.IVORY, 0xFF, 0xFF, 0xF0);
		/// <summary>
		/// <p class="color" style="background-color:#F0E68C;">0xF0E68C</p>. </summary>
		public static readonly WebPalette KHAKI = new WebPalette("KHAKI", InnerEnum.KHAKI, 0xF0, 0xE6, 0x8C);
		/// <summary>
		/// <p class="color" style="background-color:#E6E6FA;">0xE6E6FA</p>. </summary>
		public static readonly WebPalette LAVENDER = new WebPalette("LAVENDER", InnerEnum.LAVENDER, 0xE6, 0xE6, 0xFA);
		/// <summary>
		/// <p class="color" style="background-color:#FFF0F5;">0xFFF0F5</p>. </summary>
		public static readonly WebPalette LAVENDER_BLUSH = new WebPalette("LAVENDER_BLUSH", InnerEnum.LAVENDER_BLUSH, 0xFF, 0xF0, 0xF5);
		/// <summary>
		/// <p class="color" style="background-color:#7CFC00;">0x7CFC00</p>. </summary>
		public static readonly WebPalette LAWN_GREEN = new WebPalette("LAWN_GREEN", InnerEnum.LAWN_GREEN, 0x7C, 0xFC, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#FFFACD;">0xFFFACD</p>. </summary>
		public static readonly WebPalette LEMON_CHIFFON = new WebPalette("LEMON_CHIFFON", InnerEnum.LEMON_CHIFFON, 0xFF, 0xFA, 0xCD);
		/// <summary>
		/// <p class="color" style="background-color:#ADD8E6;">0xADD8E6</p>. </summary>
		public static readonly WebPalette LIGHT_BLUE = new WebPalette("LIGHT_BLUE", InnerEnum.LIGHT_BLUE, 0xAD, 0xD8, 0xE6);
		/// <summary>
		/// <p class="color" style="background-color:#F08080;">0xF08080</p>. </summary>
		public static readonly WebPalette LIGHT_CORAL = new WebPalette("LIGHT_CORAL", InnerEnum.LIGHT_CORAL, 0xF0, 0x80, 0x80);
		/// <summary>
		/// <p class="color" style="background-color:#E0FFFF;">0xE0FFFF</p>. </summary>
		public static readonly WebPalette LIGHT_CYAN = new WebPalette("LIGHT_CYAN", InnerEnum.LIGHT_CYAN, 0xE0, 0xFF, 0xFF);
		/// <summary>
		/// <p class="color" style="background-color:#FAFAD2;">0xFAFAD2</p>. </summary>
		public static readonly WebPalette LIGHT_GOLDENROD_YELLOW = new WebPalette("LIGHT_GOLDENROD_YELLOW", InnerEnum.LIGHT_GOLDENROD_YELLOW, 0xFA, 0xFA, 0xD2);
		/// <summary>
		/// <p class="color" style="background-color:#90EE90;">0x90EE90</p>. </summary>
		public static readonly WebPalette LIGHT_GREEN = new WebPalette("LIGHT_GREEN", InnerEnum.LIGHT_GREEN, 0x90, 0xEE, 0x90);
		/// <summary>
		/// <p class="color" style="background-color:#D3D3D3;">0xD3D3D3</p>. </summary>
		public static readonly WebPalette LIGHT_GREY = new WebPalette("LIGHT_GREY", InnerEnum.LIGHT_GREY, 0xD3, 0xD3, 0xD3);
		/// <summary>
		/// <p class="color" style="background-color:#FFB6C1;">0xFFB6C1</p>. </summary>
		public static readonly WebPalette LIGHT_PINK = new WebPalette("LIGHT_PINK", InnerEnum.LIGHT_PINK, 0xFF, 0xB6, 0xC1);
		/// <summary>
		/// <p class="color" style="background-color:#FFA07A;">0xFFA07A</p>. </summary>
		public static readonly WebPalette LIGHT_SALMON = new WebPalette("LIGHT_SALMON", InnerEnum.LIGHT_SALMON, 0xFF, 0xA0, 0x7A);
		/// <summary>
		/// <p class="color" style="background-color:#20B2AA;">0x20B2AA</p>. </summary>
		public static readonly WebPalette LIGHT_SEA_GREEN = new WebPalette("LIGHT_SEA_GREEN", InnerEnum.LIGHT_SEA_GREEN, 0x20, 0xB2, 0xAA);
		/// <summary>
		/// <p class="color" style="background-color:#87CEFA;">0x87CEFA</p>. </summary>
		public static readonly WebPalette LIGHT_SKY_BLUE = new WebPalette("LIGHT_SKY_BLUE", InnerEnum.LIGHT_SKY_BLUE, 0x87, 0xCE, 0xFA);
		/// <summary>
		/// <p class="color" style="background-color:#778899;">0x778899</p>. </summary>
		public static readonly WebPalette LIGHT_SLATE_GRAY = new WebPalette("LIGHT_SLATE_GRAY", InnerEnum.LIGHT_SLATE_GRAY, 0x77, 0x88, 0x99);
		/// <summary>
		/// <p class="color" style="background-color:#B0C4DE;">0xB0C4DE</p>. </summary>
		public static readonly WebPalette LIGHT_STEEL_BLUE = new WebPalette("LIGHT_STEEL_BLUE", InnerEnum.LIGHT_STEEL_BLUE, 0xB0, 0xC4, 0xDE);
		/// <summary>
		/// <p class="color" style="background-color:#FFFFE0;">0xFFFFE0</p>. </summary>
		public static readonly WebPalette LIGHT_YELLOW = new WebPalette("LIGHT_YELLOW", InnerEnum.LIGHT_YELLOW, 0xFF, 0xFF, 0xE0);
		/// <summary>
		/// <p class="color" style="background-color:#00FF00;">0x00FF00</p>. </summary>
		public static readonly WebPalette LIME = new WebPalette("LIME", InnerEnum.LIME, 0x00, 0xFF, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#32CD32;">0x32CD32</p>. </summary>
		public static readonly WebPalette LIME_GREEN = new WebPalette("LIME_GREEN", InnerEnum.LIME_GREEN, 0x32, 0xCD, 0x32);
		/// <summary>
		/// <p class="color" style="background-color:#FAF0E6;">0xFAF0E6</p>. </summary>
		public static readonly WebPalette LINEN = new WebPalette("LINEN", InnerEnum.LINEN, 0xFA, 0xF0, 0xE6);
		/// <summary>
		/// <p class="color" style="background-color:#FF00FF;">0xFF00FF</p>. </summary>
		public static readonly WebPalette MAGENTA = new WebPalette("MAGENTA", InnerEnum.MAGENTA, 0xFF, 0x00, 0xFF);
		/// <summary>
		/// <p class="color" style="background-color:#800000;">0x800000</p>. </summary>
		public static readonly WebPalette MAROON = new WebPalette("MAROON", InnerEnum.MAROON, 0x80, 0x00, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#66CDAA;">0x66CDAA</p>. </summary>
		public static readonly WebPalette MEDIUM_AQUAMARINE = new WebPalette("MEDIUM_AQUAMARINE", InnerEnum.MEDIUM_AQUAMARINE, 0x66, 0xCD, 0xAA);
		/// <summary>
		/// <p class="color" style="background-color:#0000CD;">0x0000CD</p>. </summary>
		public static readonly WebPalette MEDIUM_BLUE = new WebPalette("MEDIUM_BLUE", InnerEnum.MEDIUM_BLUE, 0x00, 0x00, 0xCD);
		/// <summary>
		/// <p class="color" style="background-color:#BA55D3;">0xBA55D3</p>. </summary>
		public static readonly WebPalette MEDIUM_ORCHID = new WebPalette("MEDIUM_ORCHID", InnerEnum.MEDIUM_ORCHID, 0xBA, 0x55, 0xD3);
		/// <summary>
		/// <p class="color" style="background-color:#9370DB;">0x9370DB</p>. </summary>
		public static readonly WebPalette MEDIUM_PURPLE = new WebPalette("MEDIUM_PURPLE", InnerEnum.MEDIUM_PURPLE, 0x93, 0x70, 0xDB);
		/// <summary>
		/// <p class="color" style="background-color:#3CB371;">0x3CB371</p>. </summary>
		public static readonly WebPalette MEDIUM_SEAGREEN = new WebPalette("MEDIUM_SEAGREEN", InnerEnum.MEDIUM_SEAGREEN, 0x3C, 0xB3, 0x71);
		/// <summary>
		/// <p class="color" style="background-color:#7B68EE;">0x7B68EE</p>. </summary>
		public static readonly WebPalette MEDIUM_SLATEBLUE = new WebPalette("MEDIUM_SLATEBLUE", InnerEnum.MEDIUM_SLATEBLUE, 0x7B, 0x68, 0xEE);
		/// <summary>
		/// <p class="color" style="background-color:#00FA9A;">0x00FA9A</p>. </summary>
		public static readonly WebPalette MEDIUM_SPRINGGREEN = new WebPalette("MEDIUM_SPRINGGREEN", InnerEnum.MEDIUM_SPRINGGREEN, 0x00, 0xFA, 0x9A);
		/// <summary>
		/// <p class="color" style="background-color:#48D1CC;">0x48D1CC</p>. </summary>
		public static readonly WebPalette MEDIUM_TURQUOISE = new WebPalette("MEDIUM_TURQUOISE", InnerEnum.MEDIUM_TURQUOISE, 0x48, 0xD1, 0xCC);
		/// <summary>
		/// <p class="color" style="background-color:#C71585;">0xC71585</p>. </summary>
		public static readonly WebPalette MEDIUM_VIOLETRED = new WebPalette("MEDIUM_VIOLETRED", InnerEnum.MEDIUM_VIOLETRED, 0xC7, 0x15, 0x85);
		/// <summary>
		/// <p class="color" style="background-color:#191970;">0x191970</p>. </summary>
		public static readonly WebPalette MIDNIGHT_BLUE = new WebPalette("MIDNIGHT_BLUE", InnerEnum.MIDNIGHT_BLUE, 0x19, 0x19, 0x70);
		/// <summary>
		/// <p class="color" style="background-color:#F5FFFA;">0xF5FFFA</p>. </summary>
		public static readonly WebPalette MINT_CREAM = new WebPalette("MINT_CREAM", InnerEnum.MINT_CREAM, 0xF5, 0xFF, 0xFA);
		/// <summary>
		/// <p class="color" style="background-color:#FFE4E1;">0xFFE4E1</p>. </summary>
		public static readonly WebPalette MISTY_ROSE = new WebPalette("MISTY_ROSE", InnerEnum.MISTY_ROSE, 0xFF, 0xE4, 0xE1);
		/// <summary>
		/// <p class="color" style="background-color:#FFE4B5;">0xFFE4B5</p>. </summary>
		public static readonly WebPalette MOCCASIN = new WebPalette("MOCCASIN", InnerEnum.MOCCASIN, 0xFF, 0xE4, 0xB5);
		/// <summary>
		/// <p class="color" style="background-color:#FFDEAD;">0xFFDEAD</p>. </summary>
		public static readonly WebPalette NAVAJO_WHITE = new WebPalette("NAVAJO_WHITE", InnerEnum.NAVAJO_WHITE, 0xFF, 0xDE, 0xAD);
		/// <summary>
		/// <p class="color" style="background-color:#000080;">0x000080</p>. </summary>
		public static readonly WebPalette NAVY = new WebPalette("NAVY", InnerEnum.NAVY, 0x00, 0x00, 0x80);
		/// <summary>
		/// <p class="color" style="background-color:#FDF5E6;">0xFDF5E6</p>. </summary>
		public static readonly WebPalette OLD_LACE = new WebPalette("OLD_LACE", InnerEnum.OLD_LACE, 0xFD, 0xF5, 0xE6);
		/// <summary>
		/// <p class="color" style="background-color:#808000;">0x808000</p>. </summary>
		public static readonly WebPalette OLIVE = new WebPalette("OLIVE", InnerEnum.OLIVE, 0x80, 0x80, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#6B8E23;">0x6B8E23</p>. </summary>
		public static readonly WebPalette OLIVE_DRAB = new WebPalette("OLIVE_DRAB", InnerEnum.OLIVE_DRAB, 0x6B, 0x8E, 0x23);
		/// <summary>
		/// <p class="color" style="background-color:#FFA500;">0xFFA500</p>. </summary>
		public static readonly WebPalette ORANGE = new WebPalette("ORANGE", InnerEnum.ORANGE, 0xFF, 0xA5, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#FF4500;">0xFF4500</p>. </summary>
		public static readonly WebPalette ORANGE_RED = new WebPalette("ORANGE_RED", InnerEnum.ORANGE_RED, 0xFF, 0x45, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#DA70D6;">0xDA70D6</p>. </summary>
		public static readonly WebPalette ORCHID = new WebPalette("ORCHID", InnerEnum.ORCHID, 0xDA, 0x70, 0xD6);
		/// <summary>
		/// <p class="color" style="background-color:#EEE8AA;">0xEEE8AA</p>. </summary>
		public static readonly WebPalette PALE_GOLDENROD = new WebPalette("PALE_GOLDENROD", InnerEnum.PALE_GOLDENROD, 0xEE, 0xE8, 0xAA);
		/// <summary>
		/// <p class="color" style="background-color:#98FB98;">0x98FB98</p>. </summary>
		public static readonly WebPalette PALE_GREEN = new WebPalette("PALE_GREEN", InnerEnum.PALE_GREEN, 0x98, 0xFB, 0x98);
		/// <summary>
		/// <p class="color" style="background-color:#AFEEEE;">0xAFEEEE</p>. </summary>
		public static readonly WebPalette PALE_TURQUOISE = new WebPalette("PALE_TURQUOISE", InnerEnum.PALE_TURQUOISE, 0xAF, 0xEE, 0xEE);
		/// <summary>
		/// <p class="color" style="background-color:#DB7093;">0xDB7093</p>. </summary>
		public static readonly WebPalette PALE_VIOLET_RED = new WebPalette("PALE_VIOLET_RED", InnerEnum.PALE_VIOLET_RED, 0xDB, 0x70, 0x93);
		/// <summary>
		/// <p class="color" style="background-color:#FFEFD5;">0xFFEFD5</p>. </summary>
		public static readonly WebPalette PAPAYA_WHIP = new WebPalette("PAPAYA_WHIP", InnerEnum.PAPAYA_WHIP, 0xFF, 0xEF, 0xD5);
		/// <summary>
		/// <p class="color" style="background-color:#FFDAB9;">0xFFDAB9</p>. </summary>
		public static readonly WebPalette PEACH_PUFF = new WebPalette("PEACH_PUFF", InnerEnum.PEACH_PUFF, 0xFF, 0xDA, 0xB9);
		/// <summary>
		/// <p class="color" style="background-color:#CD853F;">0xCD853F</p>. </summary>
		public static readonly WebPalette PERU = new WebPalette("PERU", InnerEnum.PERU, 0xCD, 0x85, 0x3F);
		/// <summary>
		/// <p class="color" style="background-color:#FFC0CB;">0xFFC0CB</p>. </summary>
		public static readonly WebPalette PINK = new WebPalette("PINK", InnerEnum.PINK, 0xFF, 0xC0, 0xCB);
		/// <summary>
		/// <p class="color" style="background-color:#DDA0DD;">0xDDA0DD</p>. </summary>
		public static readonly WebPalette PLUM = new WebPalette("PLUM", InnerEnum.PLUM, 0xDD, 0xA0, 0xDD);
		/// <summary>
		/// <p class="color" style="background-color:#B0E0E6;">0xB0E0E6</p>. </summary>
		public static readonly WebPalette POWDER_BLUE = new WebPalette("POWDER_BLUE", InnerEnum.POWDER_BLUE, 0xB0, 0xE0, 0xE6);
		/// <summary>
		/// <p class="color" style="background-color:#800080;">0x800080</p>. </summary>
		public static readonly WebPalette PURPLE = new WebPalette("PURPLE", InnerEnum.PURPLE, 0x80, 0x00, 0x80);
		/// <summary>
		/// <p class="color" style="background-color:#FF0000;">0xFF0000</p>. </summary>
		public static readonly WebPalette RED = new WebPalette("RED", InnerEnum.RED, 0xFF, 0x00, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#BC8F8F;">0xBC8F8F</p>. </summary>
		public static readonly WebPalette ROSY_BROWN = new WebPalette("ROSY_BROWN", InnerEnum.ROSY_BROWN, 0xBC, 0x8F, 0x8F);
		/// <summary>
		/// <p class="color" style="background-color:#4169E1;">0x4169E1</p>. </summary>
		public static readonly WebPalette ROYAL_BLUE = new WebPalette("ROYAL_BLUE", InnerEnum.ROYAL_BLUE, 0x41, 0x69, 0xE1);
		/// <summary>
		/// <p class="color" style="background-color:#8B4513;">0x8B4513</p>. </summary>
		public static readonly WebPalette SADDLE_BROWN = new WebPalette("SADDLE_BROWN", InnerEnum.SADDLE_BROWN, 0x8B, 0x45, 0x13);
		/// <summary>
		/// <p class="color" style="background-color:#FA8072;">0xFA8072</p>. </summary>
		public static readonly WebPalette SALMON = new WebPalette("SALMON", InnerEnum.SALMON, 0xFA, 0x80, 0x72);
		/// <summary>
		/// <p class="color" style="background-color:#F4A460;">0xF4A460</p>. </summary>
		public static readonly WebPalette SANDY_BROWN = new WebPalette("SANDY_BROWN", InnerEnum.SANDY_BROWN, 0xF4, 0xA4, 0x60);
		/// <summary>
		/// <p class="color" style="background-color:#2E8B57;">0x2E8B57</p>. </summary>
		public static readonly WebPalette SEA_GREEN = new WebPalette("SEA_GREEN", InnerEnum.SEA_GREEN, 0x2E, 0x8B, 0x57);
		/// <summary>
		/// <p class="color" style="background-color:#FFF5EE;">0xFFF5EE</p>. </summary>
		public static readonly WebPalette SEASHELL = new WebPalette("SEASHELL", InnerEnum.SEASHELL, 0xFF, 0xF5, 0xEE);
		/// <summary>
		/// <p class="color" style="background-color:#A0522D;">0xA0522D</p>. </summary>
		public static readonly WebPalette SIENNA = new WebPalette("SIENNA", InnerEnum.SIENNA, 0xA0, 0x52, 0x2D);
		/// <summary>
		/// <p class="color" style="background-color:#C0C0C0;">0xC0C0C0</p>. </summary>
		public static readonly WebPalette SILVER = new WebPalette("SILVER", InnerEnum.SILVER, 0xC0, 0xC0, 0xC0);
		/// <summary>
		/// <p class="color" style="background-color:#87CEEB;">0x87CEEB</p>. </summary>
		public static readonly WebPalette SKY_BLUE = new WebPalette("SKY_BLUE", InnerEnum.SKY_BLUE, 0x87, 0xCE, 0xEB);
		/// <summary>
		/// <p class="color" style="background-color:#6A5ACD;">0x6A5ACD</p>. </summary>
		public static readonly WebPalette SLATE_BLUE = new WebPalette("SLATE_BLUE", InnerEnum.SLATE_BLUE, 0x6A, 0x5A, 0xCD);
		/// <summary>
		/// <p class="color" style="background-color:#708090;">0x708090</p>. </summary>
		public static readonly WebPalette SLATE_GRAY = new WebPalette("SLATE_GRAY", InnerEnum.SLATE_GRAY, 0x70, 0x80, 0x90);
		/// <summary>
		/// <p class="color" style="background-color:#FFFAFA;">0xFFFAFA</p>. </summary>
		public static readonly WebPalette SNOW = new WebPalette("SNOW", InnerEnum.SNOW, 0xFF, 0xFA, 0xFA);
		/// <summary>
		/// <p class="color" style="background-color:#00FF7F;">0x00FF7F</p>. </summary>
		public static readonly WebPalette SPRING_GREEN = new WebPalette("SPRING_GREEN", InnerEnum.SPRING_GREEN, 0x00, 0xFF, 0x7F);
		/// <summary>
		/// <p class="color" style="background-color:#4682B4;">0x4682B4</p>. </summary>
		public static readonly WebPalette STEEL_BLUE = new WebPalette("STEEL_BLUE", InnerEnum.STEEL_BLUE, 0x46, 0x82, 0xB4);
		/// <summary>
		/// <p class="color" style="background-color:#D2B48C;">0xD2B48C</p>. </summary>
		public static readonly WebPalette TAN = new WebPalette("TAN", InnerEnum.TAN, 0xD2, 0xB4, 0x8C);
		/// <summary>
		/// <p class="color" style="background-color:#008080;">0x008080</p>. </summary>
		public static readonly WebPalette TEAL = new WebPalette("TEAL", InnerEnum.TEAL, 0x00, 0x80, 0x80);
		/// <summary>
		/// <p class="color" style="background-color:#D8BFD8;">0xD8BFD8</p>. </summary>
		public static readonly WebPalette THISTLE = new WebPalette("THISTLE", InnerEnum.THISTLE, 0xD8, 0xBF, 0xD8);
		/// <summary>
		/// <p class="color" style="background-color:#FF6347;">0xFF6347</p>. </summary>
		public static readonly WebPalette TOMATO = new WebPalette("TOMATO", InnerEnum.TOMATO, 0xFF, 0x63, 0x47);
		/// <summary>
		/// <p class="color" style="background-color:#40E0D0;">0x40E0D0</p>. </summary>
		public static readonly WebPalette TURQUOISE = new WebPalette("TURQUOISE", InnerEnum.TURQUOISE, 0x40, 0xE0, 0xD0);
		/// <summary>
		/// <p class="color" style="background-color:#EE82EE;">0xEE82EE</p>. </summary>
		public static readonly WebPalette VIOLET = new WebPalette("VIOLET", InnerEnum.VIOLET, 0xEE, 0x82, 0xEE);
		/// <summary>
		/// <p class="color" style="background-color:#F5DEB3;">0xF5DEB3</p>. </summary>
		public static readonly WebPalette WHEAT = new WebPalette("WHEAT", InnerEnum.WHEAT, 0xF5, 0xDE, 0xB3);
		/// <summary>
		/// <p class="color" style="background-color:#FFFFFF;">0xFFFFFF</p>. </summary>
		public static readonly WebPalette WHITE = new WebPalette("WHITE", InnerEnum.WHITE, 0xFF, 0xFF, 0xFF);
		/// <summary>
		/// <p class="color" style="background-color:#F5F5F5;">0xF5F5F5</p>. </summary>
		public static readonly WebPalette WHITE_SMOKE = new WebPalette("WHITE_SMOKE", InnerEnum.WHITE_SMOKE, 0xF5, 0xF5, 0xF5);
		/// <summary>
		/// <p class="color" style="background-color:#FFFF00;">0xFFFF00</p>. </summary>
		public static readonly WebPalette YELLOW = new WebPalette("YELLOW", InnerEnum.YELLOW, 0xFF, 0xFF, 0x00);
		/// <summary>
		/// <p class="color" style="background-color:#9ACD32;">0x9ACD32</p>. </summary>
		public static readonly WebPalette YELLOW_GREEN = new WebPalette("YELLOW_GREEN", InnerEnum.YELLOW_GREEN, 0x9A, 0xCD, 0x32);

		private static readonly IList<WebPalette> valueList = new List<WebPalette>();

		static WebPalette()
		{
			valueList.Add(ALICE_BLUE);
			valueList.Add(ANTIQUE_WHITE);
			valueList.Add(AQUA);
			valueList.Add(AQUAMARINE);
			valueList.Add(AZURE);
			valueList.Add(BEIGE);
			valueList.Add(BISQUE);
			valueList.Add(BLACK);
			valueList.Add(BLANCHED_ALMOND);
			valueList.Add(BLUE);
			valueList.Add(BLUE_VIOLET);
			valueList.Add(BROWN);
			valueList.Add(BURLYWOOD);
			valueList.Add(CADET_BLUE);
			valueList.Add(CHARTREUSE);
			valueList.Add(CHOCOLATE);
			valueList.Add(CORAL);
			valueList.Add(CORNFLOWER_BLUE);
			valueList.Add(CORNSILK);
			valueList.Add(CRIMSON);
			valueList.Add(CYAN);
			valueList.Add(DARK_BLUE);
			valueList.Add(DARK_CYAN);
			valueList.Add(DARK_GOLDENROD);
			valueList.Add(DARK_GRAY);
			valueList.Add(DARK_GREEN);
			valueList.Add(DARK_KHAKI);
			valueList.Add(DARK_MAGENTA);
			valueList.Add(DARK_OLIVE_GREEN);
			valueList.Add(DARK_ORANGE);
			valueList.Add(DARK_ORCHID);
			valueList.Add(DARK_RED);
			valueList.Add(DARK_SALMON);
			valueList.Add(DARK_SEA_GREEN);
			valueList.Add(DARK_SLATE_BLUE);
			valueList.Add(DARK_SLATE_GRAY);
			valueList.Add(DARK_TURQUOISE);
			valueList.Add(DARK_VIOLET);
			valueList.Add(DEEP_PINK);
			valueList.Add(DEEP_SKY_BLUE);
			valueList.Add(DIM_GRAY);
			valueList.Add(DODGER_BLUE);
			valueList.Add(FIREBRICK);
			valueList.Add(FLORAL_WHITE);
			valueList.Add(FOREST_GREEN);
			valueList.Add(FUCHSIA);
			valueList.Add(GAINSBORO);
			valueList.Add(GHOST_WHITE);
			valueList.Add(GOLD);
			valueList.Add(GOLDENROD);
			valueList.Add(GRAY);
			valueList.Add(GREEN);
			valueList.Add(GREEN_YELLOW);
			valueList.Add(HONEYDEW);
			valueList.Add(HOT_PINK);
			valueList.Add(INDIAN_RED);
			valueList.Add(INDIGO);
			valueList.Add(IVORY);
			valueList.Add(KHAKI);
			valueList.Add(LAVENDER);
			valueList.Add(LAVENDER_BLUSH);
			valueList.Add(LAWN_GREEN);
			valueList.Add(LEMON_CHIFFON);
			valueList.Add(LIGHT_BLUE);
			valueList.Add(LIGHT_CORAL);
			valueList.Add(LIGHT_CYAN);
			valueList.Add(LIGHT_GOLDENROD_YELLOW);
			valueList.Add(LIGHT_GREEN);
			valueList.Add(LIGHT_GREY);
			valueList.Add(LIGHT_PINK);
			valueList.Add(LIGHT_SALMON);
			valueList.Add(LIGHT_SEA_GREEN);
			valueList.Add(LIGHT_SKY_BLUE);
			valueList.Add(LIGHT_SLATE_GRAY);
			valueList.Add(LIGHT_STEEL_BLUE);
			valueList.Add(LIGHT_YELLOW);
			valueList.Add(LIME);
			valueList.Add(LIME_GREEN);
			valueList.Add(LINEN);
			valueList.Add(MAGENTA);
			valueList.Add(MAROON);
			valueList.Add(MEDIUM_AQUAMARINE);
			valueList.Add(MEDIUM_BLUE);
			valueList.Add(MEDIUM_ORCHID);
			valueList.Add(MEDIUM_PURPLE);
			valueList.Add(MEDIUM_SEAGREEN);
			valueList.Add(MEDIUM_SLATEBLUE);
			valueList.Add(MEDIUM_SPRINGGREEN);
			valueList.Add(MEDIUM_TURQUOISE);
			valueList.Add(MEDIUM_VIOLETRED);
			valueList.Add(MIDNIGHT_BLUE);
			valueList.Add(MINT_CREAM);
			valueList.Add(MISTY_ROSE);
			valueList.Add(MOCCASIN);
			valueList.Add(NAVAJO_WHITE);
			valueList.Add(NAVY);
			valueList.Add(OLD_LACE);
			valueList.Add(OLIVE);
			valueList.Add(OLIVE_DRAB);
			valueList.Add(ORANGE);
			valueList.Add(ORANGE_RED);
			valueList.Add(ORCHID);
			valueList.Add(PALE_GOLDENROD);
			valueList.Add(PALE_GREEN);
			valueList.Add(PALE_TURQUOISE);
			valueList.Add(PALE_VIOLET_RED);
			valueList.Add(PAPAYA_WHIP);
			valueList.Add(PEACH_PUFF);
			valueList.Add(PERU);
			valueList.Add(PINK);
			valueList.Add(PLUM);
			valueList.Add(POWDER_BLUE);
			valueList.Add(PURPLE);
			valueList.Add(RED);
			valueList.Add(ROSY_BROWN);
			valueList.Add(ROYAL_BLUE);
			valueList.Add(SADDLE_BROWN);
			valueList.Add(SALMON);
			valueList.Add(SANDY_BROWN);
			valueList.Add(SEA_GREEN);
			valueList.Add(SEASHELL);
			valueList.Add(SIENNA);
			valueList.Add(SILVER);
			valueList.Add(SKY_BLUE);
			valueList.Add(SLATE_BLUE);
			valueList.Add(SLATE_GRAY);
			valueList.Add(SNOW);
			valueList.Add(SPRING_GREEN);
			valueList.Add(STEEL_BLUE);
			valueList.Add(TAN);
			valueList.Add(TEAL);
			valueList.Add(THISTLE);
			valueList.Add(TOMATO);
			valueList.Add(TURQUOISE);
			valueList.Add(VIOLET);
			valueList.Add(WHEAT);
			valueList.Add(WHITE);
			valueList.Add(WHITE_SMOKE);
			valueList.Add(YELLOW);
			valueList.Add(YELLOW_GREEN);
		}

		public enum InnerEnum
		{
			ALICE_BLUE,
			ANTIQUE_WHITE,
			AQUA,
			AQUAMARINE,
			AZURE,
			BEIGE,
			BISQUE,
			BLACK,
			BLANCHED_ALMOND,
			BLUE,
			BLUE_VIOLET,
			BROWN,
			BURLYWOOD,
			CADET_BLUE,
			CHARTREUSE,
			CHOCOLATE,
			CORAL,
			CORNFLOWER_BLUE,
			CORNSILK,
			CRIMSON,
			CYAN,
			DARK_BLUE,
			DARK_CYAN,
			DARK_GOLDENROD,
			DARK_GRAY,
			DARK_GREEN,
			DARK_KHAKI,
			DARK_MAGENTA,
			DARK_OLIVE_GREEN,
			DARK_ORANGE,
			DARK_ORCHID,
			DARK_RED,
			DARK_SALMON,
			DARK_SEA_GREEN,
			DARK_SLATE_BLUE,
			DARK_SLATE_GRAY,
			DARK_TURQUOISE,
			DARK_VIOLET,
			DEEP_PINK,
			DEEP_SKY_BLUE,
			DIM_GRAY,
			DODGER_BLUE,
			FIREBRICK,
			FLORAL_WHITE,
			FOREST_GREEN,
			FUCHSIA,
			GAINSBORO,
			GHOST_WHITE,
			GOLD,
			GOLDENROD,
			GRAY,
			GREEN,
			GREEN_YELLOW,
			HONEYDEW,
			HOT_PINK,
			INDIAN_RED,
			INDIGO,
			IVORY,
			KHAKI,
			LAVENDER,
			LAVENDER_BLUSH,
			LAWN_GREEN,
			LEMON_CHIFFON,
			LIGHT_BLUE,
			LIGHT_CORAL,
			LIGHT_CYAN,
			LIGHT_GOLDENROD_YELLOW,
			LIGHT_GREEN,
			LIGHT_GREY,
			LIGHT_PINK,
			LIGHT_SALMON,
			LIGHT_SEA_GREEN,
			LIGHT_SKY_BLUE,
			LIGHT_SLATE_GRAY,
			LIGHT_STEEL_BLUE,
			LIGHT_YELLOW,
			LIME,
			LIME_GREEN,
			LINEN,
			MAGENTA,
			MAROON,
			MEDIUM_AQUAMARINE,
			MEDIUM_BLUE,
			MEDIUM_ORCHID,
			MEDIUM_PURPLE,
			MEDIUM_SEAGREEN,
			MEDIUM_SLATEBLUE,
			MEDIUM_SPRINGGREEN,
			MEDIUM_TURQUOISE,
			MEDIUM_VIOLETRED,
			MIDNIGHT_BLUE,
			MINT_CREAM,
			MISTY_ROSE,
			MOCCASIN,
			NAVAJO_WHITE,
			NAVY,
			OLD_LACE,
			OLIVE,
			OLIVE_DRAB,
			ORANGE,
			ORANGE_RED,
			ORCHID,
			PALE_GOLDENROD,
			PALE_GREEN,
			PALE_TURQUOISE,
			PALE_VIOLET_RED,
			PAPAYA_WHIP,
			PEACH_PUFF,
			PERU,
			PINK,
			PLUM,
			POWDER_BLUE,
			PURPLE,
			RED,
			ROSY_BROWN,
			ROYAL_BLUE,
			SADDLE_BROWN,
			SALMON,
			SANDY_BROWN,
			SEA_GREEN,
			SEASHELL,
			SIENNA,
			SILVER,
			SKY_BLUE,
			SLATE_BLUE,
			SLATE_GRAY,
			SNOW,
			SPRING_GREEN,
			STEEL_BLUE,
			TAN,
			TEAL,
			THISTLE,
			TOMATO,
			TURQUOISE,
			VIOLET,
			WHEAT,
			WHITE,
			WHITE_SMOKE,
			YELLOW,
			YELLOW_GREEN
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal;

		/// <summary>
		/// Holds the colour from the palette. </summary>

		private Color color_Renamed;

		/// <summary>
		/// Private constructor used to create the palette colours.
		/// </summary>
		/// <param name="red"> value for the red channel. </param>
		/// <param name="green"> value for the green channel. </param>
		/// <param name="blue"> value for the blue channel. </param>


		private WebPalette(string name, InnerEnum innerEnum, int red, int green, int blue)
		{
			color_Renamed = new Color(red, green, blue);

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		/// <summary>
		/// Gets an opaque Color object for this entry in the palette.
		/// 
		/// @return
		///           the Color object representing this entry in the table.
		/// </summary>
		public Color color()
		{
			return color_Renamed;
		}

		/// <summary>
		/// Gets a transparent Color object for this entry in the palette.
		/// </summary>
		/// <param name="alpha">
		///           the level for the alpha channel.
		/// @return
		///           the Color object representing this entry in the table. </param>


		public Color color(int alpha)
		{
			return new Color(color_Renamed.Red, color_Renamed.Green, color_Renamed.Blue, alpha);
		}

		public static IList<WebPalette> values()
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

		public static WebPalette valueOf(string name)
		{
			foreach (WebPalette enumInstance in valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new ArgumentException(name);
		}
	}

}