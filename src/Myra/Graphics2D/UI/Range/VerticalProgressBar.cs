using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A vertical progress bar control.
	/// </summary>
	public class VerticalProgressBar : ProgressBar
	{
		/// <summary>
		/// Gets the orientation of this progress bar.
		/// </summary>
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Vertical;
			}
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of this progress bar.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Left)]
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
		/// Initializes a new instance of the <see cref="VerticalProgressBar"/> class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public VerticalProgressBar(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyProgressBarStyle(stylesheet.VerticalProgressBarStyles.SafelyGetStyle(name));
		}
	}
}