namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for separator widgets.
	/// </summary>
	public class SeparatorStyle: WidgetStyle
	{
		/// <summary>
		/// Gets or sets the image to display for the separator.
		/// </summary>
		public IImage Image { get; set; }

		/// <summary>
		/// Gets or sets the thickness of the separator line.
		/// </summary>
		public int Thickness { get; set; }

		/// <summary>
		/// Initializes a new instance of the SeparatorStyle class.
		/// </summary>
		public SeparatorStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the SeparatorStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The separator style to copy from.</param>
		public SeparatorStyle(SeparatorStyle style): base(style)
		{
			Image = style.Image;
			Thickness = style.Thickness;
		}

		/// <summary>
		/// Creates a copy of this separator style.
		/// </summary>
		/// <returns>A new separator style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new SeparatorStyle(this);
		}
	}
}
