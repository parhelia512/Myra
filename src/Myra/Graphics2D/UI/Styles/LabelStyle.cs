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
	/// Style class for label controls.
	/// </summary>
	public class LabelStyle: WidgetStyle
	{
		/// <summary>
		/// Gets or sets the text color.
		/// </summary>
		public Color TextColor { get; set; }

		/// <summary>
		/// Gets or sets the text color when the label is disabled.
		/// </summary>
		public Color? DisabledTextColor { get; set; }

		/// <summary>
		/// Gets or sets the text color when the mouse is over the label.
		/// </summary>
		public Color? OverTextColor { get; set; }

		/// <summary>
		/// Gets or sets the text color when the label is pressed.
		/// </summary>
		public Color? PressedTextColor { get; set; }

		/// <summary>
		/// Gets or sets the font used to render the text.
		/// </summary>
		public SpriteFontBase Font { get; set; }

		/// <summary>
		/// Initializes a new instance of the LabelStyle class.
		/// </summary>
		public LabelStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the LabelStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The label style to copy from.</param>
		public LabelStyle(LabelStyle style) : base(style)
		{
			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			OverTextColor = style.OverTextColor;
			PressedTextColor = style.PressedTextColor;
			Font = style.Font;
		}

		/// <summary>
		/// Creates a copy of this label style.
		/// </summary>
		/// <returns>A new label style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new LabelStyle(this);
		}
	}
}
