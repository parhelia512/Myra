namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for window controls.
	/// </summary>
	public class WindowStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style for the window title.
		/// </summary>
		public LabelStyle TitleStyle { get; set; }

		/// <summary>
		/// Gets or sets the style for the window close button.
		/// </summary>
		public ImageButtonStyle CloseButtonStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the WindowStyle class.
		/// </summary>
		public WindowStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the WindowStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The window style to copy from.</param>
		public WindowStyle(WindowStyle style) : base(style)
		{
			TitleStyle = style.TitleStyle != null ? new LabelStyle(style.TitleStyle) : null;
			CloseButtonStyle = style.CloseButtonStyle != null ? new ImageButtonStyle(style.CloseButtonStyle) : null;
		}

		/// <summary>
		/// Creates a copy of this window style.
		/// </summary>
		/// <returns>A new window style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new WindowStyle(this);
		}
	}
}
