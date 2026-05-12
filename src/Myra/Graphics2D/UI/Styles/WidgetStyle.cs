namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Base style class that defines appearance properties for widgets.
	/// </summary>
	public class WidgetStyle
	{
		/// <summary>
		/// Gets or sets the unique identifier for this style.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the width of the widget.
		/// </summary>
		public int? Width { get; set; }

		/// <summary>
		/// Gets or sets the height of the widget.
		/// </summary>
		public int? Height { get; set; }

		/// <summary>
		/// Gets or sets the minimum width of the widget.
		/// </summary>
		public int? MinWidth { get; set; }

		/// <summary>
		/// Gets or sets the minimum height of the widget.
		/// </summary>
		public int? MinHeight { get; set; }

		/// <summary>
		/// Gets or sets the maximum width of the widget.
		/// </summary>
		public int? MaxWidth { get; set; }

		/// <summary>
		/// Gets or sets the maximum height of the widget.
		/// </summary>
		public int? MaxHeight { get; set; }

		/// <summary>
		/// Gets or sets the brush used to render the widget's background.
		/// </summary>
		public IBrush Background { get; set; }

		/// <summary>
		/// Gets or sets the brush used to render the background when the mouse is over the widget.
		/// </summary>
		public IBrush OverBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used to render the background when the widget is disabled.
		/// </summary>
		public IBrush DisabledBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used to render the background when the widget has focus.
		/// </summary>
		public IBrush FocusedBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used to render the widget's border.
		/// </summary>
		public IBrush Border { get; set; }

		/// <summary>
		/// Gets or sets the brush used to render the border when the mouse is over the widget.
		/// </summary>
		public IBrush OverBorder { get; set; }

		/// <summary>
		/// Gets or sets the brush used to render the border when the widget is disabled.
		/// </summary>
		public IBrush DisabledBorder { get; set; }

		/// <summary>
		/// Gets or sets the brush used to render the border when the widget has focus.
		/// </summary>
		public IBrush FocusedBorder { get; set; }

		/// <summary>
		/// Gets or sets the space around the widget's outer edge.
		/// </summary>
		public Thickness Margin { get; set; }

		/// <summary>
		/// Gets or sets the thickness of the widget's border.
		/// </summary>
		public Thickness BorderThickness { get; set; }

		/// <summary>
		/// Gets or sets the space between the widget's border and content.
		/// </summary>
		public Thickness Padding { get; set; }

		/// <summary>
		/// Initializes a new instance of the WidgetStyle class.
		/// </summary>
		public WidgetStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the WidgetStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The style to copy from.</param>
		public WidgetStyle(WidgetStyle style)
		{
			Width = style.Width;
			Height = style.Height;
			MinWidth = style.MinWidth;
			MinHeight = style.MinHeight;
			MaxWidth = style.MaxWidth;
			MaxHeight = style.MaxHeight;

			Background = style.Background;
			OverBackground = style.OverBackground;
			DisabledBackground = style.DisabledBackground;
			FocusedBackground = style.FocusedBackground;

			Border = style.Border;
			OverBorder = style.OverBorder;
			DisabledBorder = style.DisabledBorder;
			FocusedBorder = style.FocusedBorder;

			Margin = style.Margin;
			BorderThickness = style.BorderThickness;
			Padding = style.Padding;
		}

		public virtual WidgetStyle Clone()
		{
			return new WidgetStyle(this);
		}
	}
}
