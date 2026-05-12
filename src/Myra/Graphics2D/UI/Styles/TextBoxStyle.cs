using FontStashSharp;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for text input boxes.
	/// </summary>
	public class TextBoxStyle: WidgetStyle
	{
		/// <summary>
		/// Gets or sets the text color.
		/// </summary>
		public Color TextColor { get; set; }

		/// <summary>
		/// Gets or sets the text color when the text box is disabled.
		/// </summary>
		public Color? DisabledTextColor { get; set; }

		/// <summary>
		/// Gets or sets the text color when the text box has focus.
		/// </summary>
		public Color? FocusedTextColor { get; set; }

		/// <summary>
		/// Gets or sets the font used for the text.
		/// </summary>
		public SpriteFontBase Font { get; set; }

		/// <summary>
		/// Gets or sets the font used for hint/placeholder text.
		/// </summary>
		public SpriteFontBase MessageFont { get; set; }

		/// <summary>
		/// Gets or sets the image used for the text cursor.
		/// </summary>
		public IImage Cursor { get; set; }

		/// <summary>
		/// Gets or sets the brush used to render selected text.
		/// </summary>
		public IBrush Selection { get; set; }

		/// <summary>
		/// Initializes a new instance of the TextBoxStyle class.
		/// </summary>
		public TextBoxStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the TextBoxStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The text box style to copy from.</param>
		public TextBoxStyle(TextBoxStyle style) : base(style)
		{
			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			FocusedTextColor = style.FocusedTextColor;

			Font = style.Font;
			MessageFont = style.MessageFont;

			Cursor = style.Cursor;
			Selection = style.Selection;
		}

		/// <summary>
		/// Creates a copy of this text box style.
		/// </summary>
		/// <returns>A new text box style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new TextBoxStyle(this);
		}
	}
}
