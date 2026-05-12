namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for spin button controls.
	/// </summary>
	public class SpinButtonStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style for the up/increment button.
		/// </summary>
		public ImageButtonStyle UpButtonStyle
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the style for the down/decrement button.
		/// </summary>
		public ImageButtonStyle DownButtonStyle
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the style for the text input field.
		/// </summary>
		public TextBoxStyle TextBoxStyle
		{
			get; set;
		}

		/// <summary>
		/// Initializes a new instance of the SpinButtonStyle class.
		/// </summary>
		public SpinButtonStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the SpinButtonStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The spin button style to copy from.</param>
		public SpinButtonStyle(SpinButtonStyle style) : base(style)
		{
			UpButtonStyle = style.UpButtonStyle != null ? new ImageButtonStyle(style.UpButtonStyle) : null;
			DownButtonStyle = style.DownButtonStyle != null ? new ImageButtonStyle(style.DownButtonStyle) : null;
			TextBoxStyle = style.TextBoxStyle != null ? new TextBoxStyle(style.TextBoxStyle) : null;
		}

		/// <summary>
		/// Creates a copy of this spin button style.
		/// </summary>
		/// <returns>A new spin button style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new SpinButtonStyle(this);
		}
	}
}