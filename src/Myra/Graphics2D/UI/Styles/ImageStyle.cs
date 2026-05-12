namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for image display widgets.
	/// </summary>
	public class ImageStyle: WidgetStyle
	{
		/// <summary>
		/// Gets or sets the image to display.
		/// </summary>
		public IImage Image { get; set; }

		/// <summary>
		/// Gets or sets the image to display when the mouse is over the widget.
		/// </summary>
		public IImage OverImage { get; set; }

		/// <summary>
		/// Initializes a new instance of the ImageStyle class.
		/// </summary>
		public ImageStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ImageStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The image style to copy from.</param>
		public ImageStyle(ImageStyle style): base(style)
		{
			Image = style.Image;
			OverImage = style.OverImage;
		}

		/// <summary>
		/// Creates a copy of this image style.
		/// </summary>
		/// <returns>A new image style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ImageStyle(this);
		}
	}
}
