namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for button controls.
	/// </summary>
	public class ButtonStyle: WidgetStyle
	{
		/// <summary>
		/// Gets or sets the brush used to render the button's background when pressed.
		/// </summary>
		public IBrush PressedBackground { get; set; }

		/// <summary>
		/// Gets or sets the label style applied to text content within the button.
		/// </summary>
		public LabelStyle LabelStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the ButtonStyle class.
		/// </summary>
		public ButtonStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ButtonStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The button style to copy from.</param>
		public ButtonStyle(ButtonStyle style): base(style)
		{
			PressedBackground = style.PressedBackground;
			LabelStyle = style.LabelStyle != null ? new LabelStyle(style.LabelStyle) : null;
		}

		/// <summary>
		/// Creates a copy of this button style.
		/// </summary>
		/// <returns>A new button style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ButtonStyle(this);
		}
	}
}
