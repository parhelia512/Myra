namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for progress bar controls.
	/// </summary>
	public class ProgressBarStyle: WidgetStyle
	{
		/// <summary>
		/// Gets or sets the brush used to render the filled portion of the progress bar.
		/// </summary>
		public IBrush Filler { get; set; }

		/// <summary>
		/// Initializes a new instance of the ProgressBarStyle class.
		/// </summary>
		public ProgressBarStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ProgressBarStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The progress bar style to copy from.</param>
		public ProgressBarStyle(ProgressBarStyle style): base(style)
		{
			Filler = style.Filler;
		}

		/// <summary>
		/// Creates a copy of this progress bar style.
		/// </summary>
		/// <returns>A new progress bar style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ProgressBarStyle(this);
		}
	}
}
