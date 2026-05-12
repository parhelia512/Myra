using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A vertical separator widget that displays a vertical dividing line.
	/// </summary>
	public class VerticalSeparator : SeparatorWidget
	{
		/// <summary>
		/// Gets or sets the horizontal alignment of this separator.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Center)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return base.HorizontalAlignment;
			}
			set
			{
				base.HorizontalAlignment = value;
			}
		}

		/// <summary>
		/// Gets or sets the vertical alignment of this separator.
		/// </summary>
		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment VerticalAlignment
		{
			get
			{
				return base.VerticalAlignment;
			}
			set
			{
				base.VerticalAlignment = value;
			}
		}

		/// <summary>
		/// Gets the orientation of this separator (always Vertical).
		/// </summary>
		public override Orientation Orientation => Orientation.Vertical;

		/// <summary>
		/// Initializes a new instance of the VerticalSeparator class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public VerticalSeparator(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Center;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySeparatorStyle(stylesheet.VerticalSeparatorStyles.SafelyGetStyle(name));
		}
	}
}