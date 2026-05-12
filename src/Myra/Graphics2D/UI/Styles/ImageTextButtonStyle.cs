namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for buttons that display both an image and text.
	/// </summary>
	public class ImageTextButtonStyle : ButtonStyle
	{
		/// <summary>
		/// Gets or sets the pressable image style for the button's image.
		/// </summary>
		public PressableImageStyle ImageStyle
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the spacing between the image and text.
		/// </summary>
		public int ImageTextSpacing
		{
			get; set;
		}

		/// <summary>
		/// Initializes a new instance of the ImageTextButtonStyle class.
		/// </summary>
		public ImageTextButtonStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ImageTextButtonStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The image text button style to copy from.</param>
		public ImageTextButtonStyle(ImageTextButtonStyle style) : base(style)
		{
			ImageStyle = style.ImageStyle != null ? new PressableImageStyle(style.ImageStyle) : null;
		}

		/// <summary>
		/// Initializes a new instance of the ImageTextButtonStyle class from a base button style.
		/// </summary>
		/// <param name="buttonStyle">The button style to copy from.</param>
		public ImageTextButtonStyle(ButtonStyle buttonStyle) : base(buttonStyle)
		{
		}

		/// <summary>
		/// Creates a copy of this image text button style.
		/// </summary>
		/// <returns>A new image text button style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ImageTextButtonStyle(this);
		}
	}
}