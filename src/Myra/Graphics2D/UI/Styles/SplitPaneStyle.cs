namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for the split pane divider/handle button.
	/// </summary>
	public class SplitPanelButtonStyle: ButtonStyle
	{
		/// <summary>
		/// Gets or sets the size of the split pane handle.
		/// </summary>
		public int? HandleSize { get; set; }

		/// <summary>
		/// Initializes a new instance of the SplitPanelButtonStyle class.
		/// </summary>
		public SplitPanelButtonStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the SplitPanelButtonStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The split panel button style to copy from.</param>
		public SplitPanelButtonStyle(SplitPanelButtonStyle style): base(style)
		{
			HandleSize = style.HandleSize;
		}

		/// <summary>
		/// Creates a copy of this split panel button style.
		/// </summary>
		/// <returns>A new split panel button style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new SplitPanelButtonStyle(this);
		}
	}

	/// <summary>
	/// Style class for split pane containers.
	/// </summary>
	public class SplitPaneStyle: WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style for the split pane handle/divider.
		/// </summary>
		public SplitPanelButtonStyle HandleStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the SplitPaneStyle class.
		/// </summary>
		public SplitPaneStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the SplitPaneStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The split pane style to copy from.</param>
		public SplitPaneStyle(SplitPaneStyle style) : base(style)
		{
			HandleStyle = style.HandleStyle != null ? new SplitPanelButtonStyle(style.HandleStyle) : null;
		}

		/// <summary>
		/// Creates a copy of this split pane style.
		/// </summary>
		/// <returns>A new split pane style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new SplitPaneStyle(this);
		}
	}
}
