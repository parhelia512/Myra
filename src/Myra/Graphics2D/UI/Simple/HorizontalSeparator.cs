using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A horizontal separator widget that displays a horizontal dividing line.
	/// </summary>
	public class HorizontalSeparator : SeparatorWidget
	{
		/// <summary>
		/// Gets or sets the horizontal alignment of this separator.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Stretch)]
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
		[DefaultValue(VerticalAlignment.Center)]
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
		/// Gets the orientation of this separator (always Horizontal).
		/// </summary>
		public override Orientation Orientation => Orientation.Horizontal;

		/// <summary>
		/// Initializes a new instance of the HorizontalSeparator class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public HorizontalSeparator(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Center;
		}


		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySeparatorStyle(stylesheet.HorizontalSeparatorStyles.SafelyGetStyle(name));
		}
	}
}