namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for color picker dialog controls.
	/// </summary>
	public class ColorPickerDialogStyle : WindowStyle
	{
		/// <summary>
		/// Gets or sets the checker board pattern image used for transparency display.
		/// </summary>
		public IImage CheckerBoard { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background of selected color areas.
		/// </summary>
		public IBrush SelectionBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background when the mouse is over selected color areas.
		/// </summary>
		public IBrush SelectionHoverBackground { get; set; }

		/// <summary>
		/// Gets or sets the color wheel image used in the hue selection area.
		/// </summary>
		public IImage Wheel { get; set; }

		/// <summary>
		/// Gets or sets the gradient brush used for value and saturation adjustment.
		/// </summary>
		public IBrush Gradient { get; set; }

		/// <summary>
		/// Gets or sets the knob image used for the value/saturation picker.
		/// </summary>
		public IImage VSPickerKnob { get; set; }
	}
}
