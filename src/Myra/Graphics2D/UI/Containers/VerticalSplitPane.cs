using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A split pane that divides its area vertically between multiple child widgets.
	/// </summary>
	public class VerticalSplitPane : SplitPane
	{
		/// <summary>
		/// Gets the orientation of this split pane (always Vertical).
		/// </summary>
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Vertical;
			}
		}

		/// <summary>
		/// Initializes a new instance of the VerticalSplitPane class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public VerticalSplitPane(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
		}

		/// <summary>
		/// Sets the split pane's style using a stylesheet.
		/// </summary>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySplitPaneStyle(stylesheet.VerticalSplitPaneStyles.SafelyGetStyle(name));
		}
	}
}