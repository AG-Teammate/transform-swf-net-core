using System;
using System.Text;
using com.flagstone.transform.coder;
using com.flagstone.transform.datatype;
using com.flagstone.transform.exception;

/*
 * DefineTextField.java
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

namespace com.flagstone.transform.text
{
    /// <summary>
	/// DefineTextField defines an editable text field.
	/// 
	/// <para>
	/// The value entered into the text field is assigned to a specified variable
	/// allowing the creation of forms to accept values entered by a person viewing
	/// the Flash file.
	/// </para>
	/// 
	/// <para>
	/// The class contains a complex set of attributes which allows a high degree of
	/// control over how a text field is displayed:
	/// </para>
	/// 
	/// <table class="datasheet">
	/// 
	/// <tr>
	/// <td valign="top">wordWrap</td>
	/// <td>Indicates whether the text should be wrapped.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top">multiline</td>
	/// <td>Indicates whether the text field contains multiple lines.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top">password</td>
	/// <td>Indicates whether the text field will be used to display a password.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top">readOnly</td>
	/// <td>Indicates whether the text field is read only.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top">selectable</td>
	/// <td>Indicates whether the text field is selectable.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top">bordered</td>
	/// <td>Indicates whether the text field is bordered.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top">HTML</td>
	/// <td>Indicates whether the text field contains HTML.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top">useFontGlyphs</td>
	/// <td>Use either the glyphs defined in the movie to display the text or load
	/// the specified from the platform on which the Flash Player is hosted.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top">autosize</td>
	/// <td>Indicates whether the text field will resize automatically to fit the
	/// text entered.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top">maxLength</td>
	/// <td>The maximum length of the text field. May be set to zero is not maximum
	/// length is defined.</td>
	/// </tr>
	/// 
	/// </table>
	/// 
	/// <para>
	/// Additional layout information for the spacing of the text relative to the
	/// text field borders can also be specified through the following set of
	/// attributes:
	/// </para>
	/// 
	/// <table class="datasheet">
	/// 
	/// <tr>
	/// <td valign="top">alignment</td>
	/// <td>The text in the field is left-aligned, right-aligned, centred.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">leftMargin</td>
	/// <td>Left margin in twips.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">rightMargin</td>
	/// <td>Right margin in twips.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">indent</td>
	/// <td>Text indentation in twips.</td>
	/// </tr>
	/// <tr>
	/// <td valign="top">leading</td>
	/// <td>Leading in twips.</td>
	/// </tr>
	/// </table>
	/// 
	/// <para>
	/// <b>HTML Support</b><br/>
	/// Setting the HTML flag to true allows text marked up with a limited set of
	/// HTML tags to be displayed in the text field. The following tags are
	/// supported:
	/// </para>
	/// 
	/// <table>
	/// <tr>
	/// <td>&lt;p&gt;&lt;/p&gt;</td>
	/// <td>Delimits a paragraph. Only the align attribute is supported:<br>
	/// <p [align = left | right | center ]> </p></td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top" nowrap>&lt;br&gt;</td>
	/// <td>Inserts a line break.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top" nowrap>&lt;a&gt;&lt;/a&gt;</td>
	/// <td>Define a hyperlink. Two attributes are supported:
	/// <ul>
	/// <li>href - the URL of the link.</li>
	/// <li>target - name of a window or frame. (optional)</li>
	/// </ul>
	/// </td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top" nowrap>&lt;font&gt;&lt;/font&gt;</td>
	/// <td>Format enclosed text using the font. Three attributes are supported:
	/// <ul>
	/// <li>name - must match the name of a font defined using the DefineFont2 class.
	/// </li>
	/// <li>size - the height of the font in twips.</li>
	/// <li>color - the colour of the text in the hexadecimal format #RRGGBB.</li>
	/// </ul>
	/// </td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top" nowrap>&lt;b&gt;&lt;/b&gt;</td>
	/// <td>Delimits text that should be displayed in bold.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top" nowrap>&lt;b&gt;&lt;/b&gt;</td>
	/// <td>Delimits text that should be displayed in italics.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top" nowrap>&lt;b&gt;&lt;/b&gt;</td>
	/// <td>Delimits text that should be displayed underlined.</td>
	/// </tr>
	/// 
	/// <tr>
	/// <td valign="top" nowrap>&lt;li&gt;&lt;/li&gt;</td>
	/// <td>Display bulleted paragraph. Strictly speaking this is not an HTML list.
	/// The &lt;ul&gt; tag is not required and no list formats are supported.</td>
	/// </tr>
	/// 
	/// </table>
	/// 
	/// </summary>


	public sealed class DefineTextField : DefineTag
	{

		/// <summary>
		/// The maximum size of a dimension when defining the field layout. </summary>
		private const int MAX_SPACE = 65535;

		/// <summary>
		/// The unique identifier for this object. </summary>
		private int identifier;
		/// <summary>
		/// The bounding box for the field. </summary>
		private Bounds bounds;
		/// <summary>
		/// Is the field word-wrapped. </summary>
		private bool wordWrapped;
		/// <summary>
		/// Does the field contain more than one line. </summary>
		private bool multiline;
		/// <summary>
		/// Are * characters displayed for each key pressed. </summary>
		private bool password;
		/// <summary>
		/// Is the field read-only. </summary>
		private bool readOnly;
		/// <summary>
		/// Reserved. </summary>
		
		private int reserved1;
		/// <summary>
		/// Can the text be selected. </summary>
		private bool selectable;
		/// <summary>
		/// Is a border drawn around the field. </summary>
		private bool bordered;
		/// <summary>
		/// Reserved. </summary>
		
		private bool reserved2;
		/// <summary>
		/// Does the text contain HTML markup. </summary>
		private bool html;
		/// <summary>
		/// Does the text use an embedded font. </summary>
		private bool embedded;
		/// <summary>
		/// Does the field resize automatically to display the text. </summary>
		private bool autoSize;
		/// <summary>
		/// The unique identifier for the font. </summary>
		private int fontIdentifier;
		/// <summary>
		/// The name of the actionscript 3 class that provides the font. </summary>
		private string fontClass;
		/// <summary>
		/// The height of the font in twips. </summary>
		private int fontHeight;
		/// <summary>
		/// The colour of the text. </summary>
		private Color color;
		/// <summary>
		/// The maximum number of characters than can be entered. </summary>
		private int maxLength;
		/// <summary>
		/// Code representing the text alignment. </summary>
		private int alignment;
		/// <summary>
		/// The padding between the text and the left side of the field. </summary>
		private int? leftMargin;
		/// <summary>
		/// The padding between the text and the right side of the field. </summary>
		private int? rightMargin;
		/// <summary>
		/// The initial indent for the first line of any paragraph. </summary>
		private int? indent;
		/// <summary>
		/// The spacing between lines. </summary>
		private int? leading;
		/// <summary>
		/// The name of the actionscript variable. </summary>
		private string variableName = "";
		/// <summary>
		/// The initial contents of the field. </summary>
		private string initialText = "";

		/// <summary>
		/// The length of the object, minus the header, when it is encoded. </summary>
		
		private int length;

		/// <summary>
		/// Creates and initialises a DefineTextField object using values encoded
		/// in the Flash binary format.
		/// </summary>
		/// <param name="coder">
		///            an SWFDecoder object that contains the encoded Flash data.
		/// </param>
		/// <param name="context">
		///            a Context object used to manage the decoders for different
		///            type of object and to pass information on how objects are
		///            decoded.
		/// </param>
		/// <exception cref="IOException">
		///             if an error occurs while decoding the data. </exception>




		public DefineTextField(SWFDecoder coder, Context context)
		{
			length = coder.readUnsignedShort() & Coder.LENGTH_FIELD;
			if (length == Coder.IS_EXTENDED)
			{
				length = coder.readInt();
			}
			coder.mark();
			identifier = coder.readUnsignedShort();
			context.put(Context.TRANSPARENT, 1);

			bounds = new Bounds(coder);

			int bits = coder.readByte();


			bool containsText = (bits & Coder.BIT7) != 0;
			wordWrapped = (bits & Coder.BIT6) != 0;
			multiline = (bits & Coder.BIT5) != 0;
			password = (bits & Coder.BIT4) != 0;
			readOnly = (bits & Coder.BIT3) != 0;


			bool containsColor = (bits & Coder.BIT2) != 0;


			bool containsMaxLength = (bits & Coder.BIT1) != 0;


			bool containsFont = (bits & Coder.BIT0) != 0;

			bits = coder.readByte();


			bool containsClass = (bits & Coder.BIT7) != 0;
			autoSize = (bits & Coder.BIT6) != 0;


			bool containsLayout = (bits & Coder.BIT5) != 0;
			selectable = (bits & Coder.BIT4) != 0;
			bordered = (bits & Coder.BIT3) != 0;
			reserved2 = (bits & Coder.BIT2) != 0;
			html = (bits & Coder.BIT1) != 0;
			embedded = (bits & Coder.BIT0) != 0;

			if (containsFont)
			{
				fontIdentifier = coder.readUnsignedShort();

				if (containsClass)
				{
					fontClass = coder.readString();
				}
				fontHeight = coder.readUnsignedShort();
			}

			if (containsColor)
			{
				color = new Color(coder, context);
			}

			if (containsMaxLength)
			{
				maxLength = coder.readUnsignedShort();
			}

			if (containsLayout)
			{
				alignment = coder.readByte();
				leftMargin = coder.readUnsignedShort();
				rightMargin = coder.readUnsignedShort();
				indent = coder.readUnsignedShort();
				leading = coder.readSignedShort();
			}

			variableName = coder.readString();

			if (containsText)
			{
				initialText = coder.readString();
			}

			context.remove(Context.TRANSPARENT);
			coder.check(length);
			coder.unmark();
		}

		/// <summary>
		/// Creates an DefineTextField object with the specified identifier.
		/// </summary>
		/// <param name="uid"> the unique identifier for the text field. </param>


		public DefineTextField(int uid)
		{
			Identifier = uid;
		}

		/// <summary>
		/// Creates and initialises a DefineTextField object using the values copied
		/// from another DefineTextField object.
		/// </summary>
		/// <param name="object">
		///            a DefineTextField object from which the values will be
		///            copied. </param>


		public DefineTextField(DefineTextField @object)
		{
			identifier = @object.identifier;
			bounds = @object.bounds;
			wordWrapped = @object.wordWrapped;
			multiline = @object.multiline;
			password = @object.password;
			readOnly = @object.readOnly;
			reserved1 = @object.reserved1;
			selectable = @object.selectable;
			bordered = @object.bordered;
			reserved2 = @object.reserved2;
			html = @object.html;
			embedded = @object.embedded;
			autoSize = @object.autoSize;
			fontIdentifier = @object.fontIdentifier;
			fontClass = @object.fontClass;
			fontHeight = @object.fontHeight;
			color = @object.color;
			maxLength = @object.maxLength;
			alignment = @object.alignment;
			leftMargin = @object.leftMargin;
			rightMargin = @object.rightMargin;
			indent = @object.indent;
			leading = @object.leading;
			variableName = @object.variableName;
			initialText = @object.initialText;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public int Identifier
		{
			get => identifier;
		    set
			{
				if ((value < 1) || (value > Coder.USHORT_MAX))
				{
					throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, value);
				}
				identifier = value;
			}
		}


		/// <summary>
		/// Returns the bounding rectangle that completely encloses the text field.
		/// </summary>
		/// <returns> the bounding rectangle of the text. </returns>
		public Bounds Bounds => bounds;

	    /// <summary>
		/// Does the text field support word wrapping.
		/// </summary>
		/// <returns> true if the field will wrap the text. </returns>
		public bool WordWrapped => wordWrapped;

	    /// <summary>
		/// Does the text field support multiple lines of text.
		/// </summary>
		/// <returns> true if the field contains more than one line. </returns>
		public bool Multiline => multiline;

	    /// <summary>
		/// Does the text field protect passwords being entered.
		/// </summary>
		/// <returns> true if the field obscures the characters typed. </returns>
		public bool Password => password;

	    /// <summary>
		/// Is the text field read-only.
		/// </summary>
		/// <returns> true if the text cannot be edited. </returns>
		public bool ReadOnly => readOnly;

	    /// <summary>
		/// Is the text field selectable.
		/// </summary>
		/// <returns> true if the text can be selected with the mouse. </returns>
		public bool Selectable => selectable;

	    /// <summary>
		/// Is the text field bordered.
		/// </summary>
		/// <returns> true if the field has a border. </returns>
		public bool Bordered => bordered;

	    /// <summary>
		/// Does the text field contain HTML.
		/// </summary>
		/// <returns> true if the field displays HTML. </returns>
		public bool Html => html;

	    /// <summary>
		/// Does the text field resize to fit the contents.
		/// </summary>
		/// <returns> true if the field will automatically resize to fit the text. </returns>
		public bool AutoSize => autoSize;

	    /// <summary>
		/// Sets whether the text field will resize to fit the contents.
		/// </summary>
		/// <param name="aFlag">
		///            indicate whether the text field will resize automatically. </param>
		/// <returns> this object. </returns>


		public DefineTextField setAutoSize(bool aFlag)
		{
			autoSize = aFlag;
			return this;
		}

		/// <summary>
		/// Indicates whether the text will be displayed using the font defined in
		/// the movie or whether a font defined on the host platform will be used.
		/// </summary>
		/// <returns> true if the text will be displayed using the glyphs from the font
		///         defined in the movie, false if the glyphs will be loaded from the
		///         platform on which the Flash Player is hosted. </returns>
		public bool Embedded => embedded;

	    /// <summary>
		/// Get the identifier of the font used to display the characters.
		/// </summary>
		/// <returns> the unique identifier of the font. </returns>
		public int FontIdentifier => fontIdentifier;

	    /// <summary>
		/// Get the name of the Actionscript 3 class that provides the font. </summary>
		/// <returns> the name the Actionscript class. </returns>
		public string FontClass => fontClass;

	    /// <summary>
		/// Get the size of the font used to display the text.
		/// </summary>
		/// <returns> the height of the font in twips. </returns>
		public int FontHeight => fontHeight;

	    /// <summary>
		/// Get the text color.
		/// </summary>
		/// <returns> the colour used to display the text. </returns>
		public Color Color => color;

	    /// <summary>
		/// Get the maximum number of characters displayed in the field.
		/// </summary>
		/// <returns> the maximum number of characters displayed. </returns>
		public int MaxLength => maxLength;

	    /// <summary>
		/// Get the alignment of the text, either AlignLeft, AlignRight,
		/// AlignCenter or AlignJustify.
		/// </summary>
		/// <returns> the alignment of the text. </returns>
		public HorizontalAlign Alignment
		{
			get
			{
				HorizontalAlign value;
				switch (alignment)
				{
				case 0:
					value = HorizontalAlign.LEFT;
					break;
				case Coder.BIT0:
					value = HorizontalAlign.RIGHT;
					break;
				case Coder.BIT1:
					value = HorizontalAlign.CENTER;
					break;
				case Coder.BIT0 | Coder.BIT1:
					value = HorizontalAlign.JUSTIFY;
					break;
				default:
					throw new InvalidOperationException();
				}
				return value;
			}
		}

		/// <summary>
		/// Get the left margin in twips.
		/// </summary>
		/// <returns> the padding between the text and the left edge of the field. </returns>
		public int getLeftMargin()
		{
			return leftMargin.Value;
		}

		/// <summary>
		/// Get the right margin in twips.
		/// </summary>
		/// <returns> the padding between the text and the right edge of the field. </returns>
		public int getRightMargin()
		{
			return rightMargin.Value;
		}

		/// <summary>
		/// Get the indentation of the first line of text in twips.
		/// </summary>
		/// <returns> the initial indent for the first line of text. </returns>
		public int getIndent()
		{
			return indent.Value;
		}

		/// <summary>
		/// Get the leading in twips.
		/// </summary>
		/// <returns> the spacing between lines. </returns>
		public int getLeading()
		{
			return leading.Value;
		}

		/// <summary>
		/// Get the name of the variable the value in the text field will be
		/// assigned to.
		/// </summary>
		/// <returns> the name of the actionscript variable that the field contents
		/// are assigned to. </returns>
		public string VariableName => variableName;

	    /// <summary>
		/// Get the default text displayed in the field.
		/// </summary>
		/// <returns> the sting initially displayed in the field. </returns>
		public string InitialText => initialText;

	    /// <summary>
		/// Sets the bounding rectangle of the text field.
		/// </summary>
		/// <param name="rect">
		///            the bounding rectangle enclosing the text field. Must not be
		///            null. </param>
		/// <returns> this object. </returns>


		public DefineTextField setBounds(Bounds rect)
		{
			if (rect == null)
			{
				throw new ArgumentException();
			}
			bounds = rect;
			return this;
		}

		/// <summary>
		/// Set whether the text field supports word wrapping.
		/// </summary>
		/// <param name="aFlag">
		///            set whether the text field is word wrapped. </param>
		/// <returns> this object. </returns>


		public DefineTextField setWordWrapped(bool aFlag)
		{
			wordWrapped = aFlag;
			return this;
		}

		/// <summary>
		/// Set whether the text field contains multiple lines of text.
		/// </summary>
		/// <param name="aFlag">
		///            set whether the text field is multiline. </param>
		/// <returns> this object. </returns>


		public DefineTextField setMultiline(bool aFlag)
		{
			multiline = aFlag;
			return this;
		}

		/// <summary>
		/// Set whether the text field should protect passwords entered.
		/// </summary>
		/// <param name="aFlag">
		///            set whether the text field is password protected. </param>
		/// <returns> this object. </returns>


		public DefineTextField setPassword(bool aFlag)
		{
			password = aFlag;
			return this;
		}

		/// <summary>
		/// Set whether the text field is read-only.
		/// </summary>
		/// <param name="aFlag">
		///            set whether the text field is read-only. </param>
		/// <returns> this object. </returns>


		public DefineTextField setReadOnly(bool aFlag)
		{
			readOnly = aFlag;
			return this;
		}

		/// <summary>
		/// Set whether the text field is selectable.
		/// </summary>
		/// <param name="aFlag">
		///            set whether the text field is selectable. </param>
		/// <returns> this object. </returns>


		public DefineTextField setSelectable(bool aFlag)
		{
			selectable = aFlag;
			return this;
		}

		/// <summary>
		/// Set whether the text field is bordered.
		/// </summary>
		/// <param name="aFlag">
		///            set whether the text field is bordered. </param>
		/// <returns> this object. </returns>


	   public DefineTextField setBordered(bool aFlag)
	   {
			bordered = aFlag;
			return this;
	   }

		/// <summary>
		/// Set whether the text field contains HTML.
		/// </summary>
		/// <param name="aFlag">
		///            set whether the text field contains HTML. </param>
		/// <returns> this object. </returns>


		public DefineTextField setHtml(bool aFlag)
		{
			html = aFlag;
			return this;
		}

		/// <summary>
		/// Set whether the text field characters are displayed using the font
		/// defined in the movie or whether the Flash Player uses a font definition
		/// loaded from the platform on which it is hosted.
		/// </summary>
		/// <param name="aFlag">
		///            set whether the text field characters will be drawn using the
		///            font in the movie (true) or use a font loaded by the Flash
		///            Player (false). </param>
		/// <returns> this object. </returns>


		public DefineTextField setEmbedded(bool aFlag)
		{
			embedded = aFlag;
			return this;
		}

		/// <summary>
		/// Sets the identifier of the font used to display the characters.
		/// </summary>
		/// <param name="uid">
		///            the identifier for the font that the text will be rendered in.
		///            Must be in the range 1..65535. </param>
		/// <returns> this object. </returns>


		public DefineTextField setFontIdentifier(int uid)
		{
			if ((uid < 1) || (uid > Coder.USHORT_MAX))
			{
				 throw new IllegalArgumentRangeException(1, Coder.USHORT_MAX, uid);
			}
			fontIdentifier = uid;
			fontClass = null; //NOPMD
			return this;
		}


		/// <summary>
		/// Set the name of the Actionscript 3 class that provides the font. </summary>
		/// <param name="name"> the name the Actionscript class. </param>
		/// <returns> this object. </returns>


		public DefineTextField setFontClass(string name)
		{
			fontClass = name;
			fontIdentifier = 0;
			return this;
		}

		/// <summary>
		/// Sets the height of the characters.
		/// </summary>
		/// <param name="aNumber">
		///            the height of the font. Must be in the range 0..65535. </param>
		/// <returns> this object. </returns>


		public DefineTextField setFontHeight(int aNumber)
		{
			if ((aNumber < 0) || (aNumber > Coder.USHORT_MAX))
			{
				throw new IllegalArgumentRangeException(0, Coder.USHORT_MAX, aNumber);
			}
			fontHeight = aNumber;
			return this;
		}

		/// <summary>
		/// Sets the text color. If set to null then the text color defaults to
		/// black.
		/// </summary>
		/// <param name="aColor">
		///            the colour object that defines the text colour. </param>
		/// <returns> this object. </returns>


		public DefineTextField setColor(Color aColor)
		{
			if (aColor == null)
			{
				color = new Color(0, 0, 0);
			}
			else
			{
				color = aColor;
			}
			return this;
		}

		/// <summary>
		/// Sets the maximum length of the text displayed. May be set to zero if no
		/// maximum length is defined.
		/// </summary>
		/// <param name="aNumber">
		///            the maximum number of characters displayed in the field. Must
		///            be in the range 0..65535. </param>
		/// <returns> this object. </returns>


		public DefineTextField setMaxLength(int aNumber)
		{
			if ((aNumber < 0) || (aNumber > MAX_SPACE))
			{
				throw new IllegalArgumentRangeException(0, MAX_SPACE, aNumber);
			}
			maxLength = aNumber;
			return this;
		}

		/// <summary>
		/// Sets the alignment of the text, either AlignLeft, AlignRight, AlignCenter
		/// or AlignJustify.
		/// </summary>
		/// <param name="align">
		///            the type of alignment. Must be either ALIGN_LEFT, ALIGN_RIGHT
		///            or ALIGN_JUSTIFY. </param>
		/// <returns> this object. </returns>


		public DefineTextField setAlignment(HorizontalAlign align)
		{
			switch (align)
			{
			case HorizontalAlign.LEFT:
				alignment = 0;
				break;
			case HorizontalAlign.RIGHT:
				alignment = Coder.BIT0;
				break;
			case HorizontalAlign.CENTER:
				alignment = Coder.BIT1;
				break;
			case HorizontalAlign.JUSTIFY:
				alignment = Coder.BIT0 | Coder.BIT1;
				break;
			default:
				throw new ArgumentException();
			}
			return this;
		}

		/// <summary>
		/// Sets the left margin in twips.
		/// </summary>
		/// <param name="aNumber">
		///            the width of the left margin. Must be in the range 0..65535. </param>
		/// <returns> this object. </returns>


		public DefineTextField setLeftMargin(int? aNumber)
		{
			if ((aNumber != null) && ((aNumber < 0) || (aNumber > MAX_SPACE)))
			{
				throw new IllegalArgumentRangeException(0, MAX_SPACE, aNumber.Value);
			}
			leftMargin = aNumber;
			return this;
		}

		/// <summary>
		/// Sets the right margin in twips.
		/// </summary>
		/// <param name="aNumber">
		///            the width of the right margin. Must be in the range 0..65535. </param>
		/// <returns> this object. </returns>


		public DefineTextField setRightMargin(int? aNumber)
		{
			if ((aNumber != null) && ((aNumber < 0) || (aNumber > MAX_SPACE)))
			{
				throw new IllegalArgumentRangeException(0, MAX_SPACE, aNumber.Value);
			}
			rightMargin = aNumber;
			return this;
		}

		/// <summary>
		/// Returns the indentation of the first line of text in twips.
		/// </summary>
		/// <param name="aNumber">
		///            the indentation for the first line. Must be in the range
		///            0..65535. </param>
		/// <returns> this object. </returns>


		public DefineTextField setIndent(int? aNumber)
		{
			if ((aNumber != null) && ((aNumber < 0) || (aNumber > MAX_SPACE)))
			{
				throw new IllegalArgumentRangeException(0, MAX_SPACE, aNumber.Value);
			}
			indent = aNumber;
			return this;
		}

		/// <summary>
		/// Sets the spacing between lines, measured in twips.
		/// </summary>
		/// <param name="aNumber">
		///            the value for the leading. Must be in the range -32768..32767. </param>
		/// <returns> this object. </returns>


		public DefineTextField setLeading(int? aNumber)
		{
			if ((aNumber < Coder.SHORT_MIN) || (aNumber > Coder.SHORT_MAX))
			{
				throw new IllegalArgumentRangeException(Coder.SHORT_MIN, Coder.SHORT_MAX, aNumber.Value);
			}
			leading = aNumber;
			return this;
		}

		/// <summary>
		/// Sets the name of the variable the value in the text field will be
		/// assigned to.
		/// </summary>
		/// <param name="aString">
		///            the name of the variable. </param>
		/// <returns> this object. </returns>


		public DefineTextField setVariableName(string aString)
		{
			variableName = aString;
			return this;
		}

		/// <summary>
		/// Sets the value that will initially be displayed in the text field.
		/// </summary>
		/// <param name="aString">
		///            the initial text displayed. </param>
		/// <returns> this object. </returns>


		public DefineTextField setInitialText(string aString)
		{
			initialText = aString;
			return this;
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public DefineTextField copy()
		{
			return new DefineTextField(this);
		}

		/// <summary>
		/// {@inheritDoc} </summary>
		public override string ToString()
		{


			StringBuilder builder = new StringBuilder();

			builder.Append("DefineTextField: { identifier = ").Append(identifier);
			builder.Append("; bounds = ").Append(bounds);
			builder.Append("; wordWrapped = ").Append(wordWrapped);
			builder.Append("; multiline = ").Append(multiline);
			builder.Append("; password = ").Append(password);
			builder.Append("; readOnly = ").Append(readOnly);
			builder.Append("; autoSize = ").Append(autoSize);
			builder.Append("; selectable = ").Append(selectable);
			builder.Append("; bordered = ").Append(bordered);
			builder.Append("; HTML = ").Append(html);
			builder.Append("; embedded = ").Append(embedded);
			builder.Append("; fontIdentifier = ").Append(fontIdentifier).Append(";");
			builder.Append("; fontHeight = ").Append(fontHeight);
			builder.Append("; color = ").Append(color);
			builder.Append("; maxLength = ").Append(maxLength);
			builder.Append("; alignment = ").Append(alignment);
			builder.Append("; leftMargin = ").Append(leftMargin);
			builder.Append("; rightMargin = ").Append(rightMargin);
			builder.Append("; indent = ").Append(indent);
			builder.Append("; leading = ").Append(leading);
			builder.Append("; variableName = ").Append(variableName);
			builder.Append("; initalText = ").Append(initialText);
			builder.Append(" }");

			return builder.ToString();
		}

		/// <summary>
		/// {@inheritDoc} </summary>



		public int prepareToEncode(Context context)
		{
			// CHECKSTYLE:OFF
			context.put(Context.TRANSPARENT, 1);

			length = 2 + bounds.prepareToEncode(context);
			length += 2;
			length += (fontIdentifier == 0) ? 0 : 4;
			length += ReferenceEquals(fontClass, null) ? 0 : context.strlen(fontClass) + 2;
			length += color == null ? 0 : 4;
			length += (maxLength > 0) ? 2 : 0;
			length += (containsLayout()) ? 9 : 0;
			length += context.strlen(variableName);
			length += (ReferenceEquals(initialText, null)) ? 0 : context.strlen(initialText);

			context.remove(Context.TRANSPARENT);

			return (length > Coder.HEADER_LIMIT ? Coder.LONG_HEADER : Coder.SHORT_HEADER) + length;
			// CHECKSTYLE:ON
		}

		/// <summary>
		/// {@inheritDoc} </summary>




		public void encode(SWFEncoder coder, Context context)
		{

			if (length > Coder.HEADER_LIMIT)
			{
				coder.writeShort((MovieTypes.DEFINE_TEXT_FIELD << Coder.LENGTH_FIELD_SIZE) | Coder.IS_EXTENDED);
				coder.writeInt(length);
			}
			else
			{
				coder.writeShort((MovieTypes.DEFINE_TEXT_FIELD << Coder.LENGTH_FIELD_SIZE) | length);
			}
			if (Constants.DEBUG)
			{
				coder.mark();
			}
			context.put(Context.TRANSPARENT, 1);

			coder.writeShort(identifier);
			bounds.encode(coder, context);
			int bits = 0;
			bits |= ReferenceEquals(initialText, null) ? 0 : Coder.BIT7;
			bits |= wordWrapped ? Coder.BIT6 : 0;
			bits |= multiline ? Coder.BIT5 : 0;
			bits |= password ? Coder.BIT4 : 0;
			bits |= readOnly ? Coder.BIT3 : 0;
			bits |= color == null ? 0 : Coder.BIT2;
			bits |= maxLength > 0 ? Coder.BIT1 : 0;
			bits |= fontIdentifier == 0 ? 0 : Coder.BIT0;
			coder.writeByte(bits);

			bits = 0;
			bits |= ReferenceEquals(fontClass, null) ? 0 : Coder.BIT7;
			bits |= autoSize ? Coder.BIT6 : 0;
			bits |= containsLayout() ? Coder.BIT5 : 0;
			bits |= selectable ? Coder.BIT4 : 0;
			bits |= bordered ? Coder.BIT3 : 0;
			bits |= reserved2 ? Coder.BIT2 : 0;
			bits |= html ? Coder.BIT1 : 0;
			bits |= embedded ? Coder.BIT0 : 0;
			coder.writeByte(bits);

			if (fontIdentifier > 0)
			{
				coder.writeShort(fontIdentifier);
				coder.writeShort(fontHeight);
			}
			else if (!ReferenceEquals(fontClass, null))
			{
				coder.writeString(fontClass);
				coder.writeShort(fontHeight);
			}

		    color?.encode(coder, context);

		    if (maxLength > 0)
			{
				coder.writeShort(maxLength);
			}

			if (containsLayout())
			{
				coder.writeByte(alignment);
				coder.writeShort(leftMargin == null ? 0 : leftMargin.Value);
				coder.writeShort(rightMargin == null ? 0 : rightMargin.Value);
				coder.writeShort(indent == null ? 0 : indent.Value);
				coder.writeShort(leading == null ? 0 : leading.Value);
			}

			coder.writeString(variableName);

			if (!ReferenceEquals(initialText, null))
			{
				coder.writeString(initialText);
			}
			context.remove(Context.TRANSPARENT);
			if (Constants.DEBUG)
			{
				coder.check(length);
				coder.unmark();
			}
		}

		/// <summary>
		/// Indicates whether the field contains layout information. </summary>
		/// <returns> if the field contains values that control the layout. </returns>
		private bool containsLayout()
		{
			return (leftMargin != null) || (rightMargin != null) || (indent != null) || (leading != null);
		}
	}

}