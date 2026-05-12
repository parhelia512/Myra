using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A vertical slider control for selecting a value within a range.
	/// </summary>
	public class VerticalSlider : Slider
	{
		/// <summary>
		/// Gets the orientation of this slider.
		/// </summary>
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Vertical;
			}
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of this slider.
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
		/// Gets or sets the vertical alignment of this slider.
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
		/// Initializes a new instance of the <see cref="VerticalSlider"/> class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public VerticalSlider(string styleName = Stylesheet.DefaultStyleName): base(styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplySliderStyle(stylesheet.VerticalSliderStyles.SafelyGetStyle(name));
		}
	}
}