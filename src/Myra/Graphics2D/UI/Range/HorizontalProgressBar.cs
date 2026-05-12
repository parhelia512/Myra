using System.ComponentModel;
using System.Linq;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A horizontal progress bar control.
	/// </summary>
	public class HorizontalProgressBar : ProgressBar
	{
		/// <summary>
		/// Gets the orientation of this progress bar.
		/// </summary>
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Horizontal;
			}
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of this progress bar.
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
		/// Gets or sets the vertical alignment of this progress bar.
		/// </summary>
		[DefaultValue(VerticalAlignment.Top)]
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
		/// Initializes a new instance of the <see cref="HorizontalProgressBar"/> class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public HorizontalProgressBar(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyProgressBarStyle(stylesheet.HorizontalProgressBarStyles.SafelyGetStyle(name));
		}
	}
}