using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A split pane that divides its area horizontally between multiple child widgets.
	/// </summary>
	public class HorizontalSplitPane : SplitPane
	{
		/// <summary>
		/// Gets the orientation of this split pane (always Horizontal).
		/// </summary>
		public override Orientation Orientation => Orientation.Horizontal;

		/// <summary>
		/// Initializes a new instance of the HorizontalSplitPane class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public HorizontalSplitPane(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
		}

		/// <summary>
		/// Sets the split pane's style using a stylesheet.
		/// </summary>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySplitPaneStyle(stylesheet.HorizontalSplitPaneStyles.SafelyGetStyle(name));
		}
	}
}