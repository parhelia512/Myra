using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A horizontal slider control for selecting a value within a range.
	/// </summary>
	public class HorizontalSlider : Slider
	{
		/// <summary>
		/// Gets the orientation of this slider.
		/// </summary>
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Horizontal;
			}
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of this slider.
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
		/// Initializes a new instance of the <see cref="HorizontalSlider"/> class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public HorizontalSlider(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Top;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySliderStyle(stylesheet.HorizontalSliderStyles.SafelyGetStyle(name));
		}
	}
}