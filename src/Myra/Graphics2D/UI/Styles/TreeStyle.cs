namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for tree view controls.
	/// </summary>
	public class TreeStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style for the expand/collapse mark buttons.
		/// </summary>
		public ImageButtonStyle MarkStyle { get; set; }

		/// <summary>
		/// Gets or sets the style for node labels.
		/// </summary>
		public LabelStyle LabelStyle { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background of a selected node.
		/// </summary>
		public IBrush SelectionBackground
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the brush used for the background when the mouse is over a selected node.
		/// </summary>
		public IBrush SelectionHoverBackground
		{
			get; set;
		}

		/// <summary>
		/// Initializes a new instance of the TreeStyle class.
		/// </summary>
		public TreeStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the TreeStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The tree style to copy from.</param>
		public TreeStyle(TreeStyle style): base(style)
		{
			MarkStyle = style.MarkStyle != null ? new ImageButtonStyle(style.MarkStyle) : null;
			LabelStyle = style.LabelStyle != null ? new LabelStyle(style.LabelStyle) : null;
			SelectionBackground = style.SelectionBackground;
			SelectionHoverBackground = style.SelectionHoverBackground;
		}

		/// <summary>
		/// Creates a copy of this tree style.
		/// </summary>
		/// <returns>A new tree style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new TreeStyle(this);
		}
	}
}