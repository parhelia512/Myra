namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for file dialog controls.
	/// </summary>
	public class FileDialogStyle : WindowStyle
	{
		/// <summary>
		/// Gets or sets the style for the back navigation button.
		/// </summary>
		public ImageButtonStyle BackButtonStyle { get; set; }

		/// <summary>
		/// Gets or sets the style for the forward navigation button.
		/// </summary>
		public ImageButtonStyle ForwardButtonStyle { get; set; }

		/// <summary>
		/// Gets or sets the style for the parent directory button.
		/// </summary>
		public ImageButtonStyle ParentButtonStyle { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background of selected file items.
		/// </summary>
		public IBrush SelectionBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background when the mouse is over selected file items.
		/// </summary>
		public IBrush SelectionHoverBackground { get; set; }

		/// <summary>
		/// Gets or sets the image used to represent folder items.
		/// </summary>
		public IImage IconFolder { get; set; }

		/// <summary>
		/// Gets or sets the image used to represent drive items.
		/// </summary>
		public IImage IconDrive { get; set; }
	}
}
