namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for tab control widgets.
	/// </summary>
	public class TabControlStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style for individual tab items.
		/// </summary>
		public ImageTextButtonStyle TabItemStyle { get; set; }

		/// <summary>
		/// Gets or sets the style for the tab content area.
		/// </summary>
		public WidgetStyle ContentStyle { get; set; }

		/// <summary>
		/// Gets or sets the spacing between tab buttons.
		/// </summary>
		public int ButtonSpacing { get; set; }

		/// <summary>
		/// Gets or sets the spacing around the header area.
		/// </summary>
		public int HeaderSpacing { get; set; }

		/// <summary>
		/// Gets or sets the position of the tab selector bar.
		/// </summary>
		public TabSelectorPosition TabSelectorPosition { get; set; }

		/// <summary>
		/// Gets or sets the style for the tab close button.
		/// </summary>
		public ImageButtonStyle CloseButtonStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the TabControlStyle class.
		/// </summary>
		public TabControlStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the TabControlStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The tab control style to copy from.</param>
		public TabControlStyle(TabControlStyle style) : base(style)
		{
			TabItemStyle = style.TabItemStyle != null ? new ImageTextButtonStyle(style.TabItemStyle) : null;
			ContentStyle = style.ContentStyle != null ? new WidgetStyle(style.ContentStyle) : null;

			ButtonSpacing = style.ButtonSpacing;
			HeaderSpacing = style.HeaderSpacing;

			TabSelectorPosition = style.TabSelectorPosition;
		}

		/// <summary>
		/// Creates a copy of this tab control style.
		/// </summary>
		/// <returns>A new tab control style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new TabControlStyle(this);
		}
	}
}
