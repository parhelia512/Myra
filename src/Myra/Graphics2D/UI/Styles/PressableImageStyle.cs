namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for images that support a pressed state.
	/// </summary>
	public class PressableImageStyle: ImageStyle
	{
		/// <summary>
		/// Gets or sets the image to display when the widget is pressed.
		/// </summary>
		public IImage PressedImage { get; set; }

		/// <summary>
		/// Initializes a new instance of the PressableImageStyle class.
		/// </summary>
		public PressableImageStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the PressableImageStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The pressable image style to copy from.</param>
		public PressableImageStyle(PressableImageStyle style) : base(style)
		{
			PressedImage = style.PressedImage;
		}

		/// <summary>
		/// Creates a copy of this pressable image style.
		/// </summary>
		/// <returns>A new pressable image style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new PressableImageStyle(this);
		}
	}
}
