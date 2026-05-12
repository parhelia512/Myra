namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for image button controls.
	/// </summary>
	public class ImageButtonStyle: ButtonStyle
	{
		/// <summary>
		/// Gets or sets the pressable image style for the button's image.
		/// </summary>
		public PressableImageStyle ImageStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the ImageButtonStyle class.
		/// </summary>
		public ImageButtonStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ImageButtonStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The image button style to copy from.</param>
		public ImageButtonStyle(ImageButtonStyle style): base(style)
		{
			ImageStyle = style.ImageStyle != null ? new PressableImageStyle(style.ImageStyle) : null;
		}

		/// <summary>
		/// Initializes a new instance of the ImageButtonStyle class from a base button style.
		/// </summary>
		/// <param name="buttonStyle">The button style to copy from.</param>
		public ImageButtonStyle(ButtonStyle buttonStyle) : base(buttonStyle)
		{
		}

		/// <summary>
		/// Creates a copy of this image button style.
		/// </summary>
		/// <returns>A new image button style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ImageButtonStyle(this);
		}
	}
}
