namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for scroll viewer controls.
	/// </summary>
	public class ScrollViewerStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the image for the horizontal scrollbar background.
		/// </summary>
		public IImage HorizontalScrollBackground { get; set; }

		/// <summary>
		/// Gets or sets the image for the horizontal scrollbar knob (thumb).
		/// </summary>
		public IImage HorizontalScrollKnob { get; set; }

		/// <summary>
		/// Gets or sets the image for the vertical scrollbar background.
		/// </summary>
		public IImage VerticalScrollBackground { get; set; }

		/// <summary>
		/// Gets or sets the image for the vertical scrollbar knob (thumb).
		/// </summary>
		public IImage VerticalScrollKnob { get; set; }

		/// <summary>
		/// Initializes a new instance of the ScrollViewerStyle class.
		/// </summary>
		public ScrollViewerStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ScrollViewerStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The scroll viewer style to copy from.</param>
		public ScrollViewerStyle(ScrollViewerStyle style) : base(style)
		{
			HorizontalScrollBackground = style.HorizontalScrollBackground;
			HorizontalScrollKnob = style.HorizontalScrollKnob;
			VerticalScrollBackground = style.VerticalScrollBackground;
			VerticalScrollKnob = style.VerticalScrollKnob;
		}

		/// <summary>
		/// Creates a copy of this scroll viewer style.
		/// </summary>
		/// <returns>A new scroll viewer style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ScrollViewerStyle(this);
		}
	}
}