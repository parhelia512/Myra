namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for combo box (dropdown) controls.
	/// </summary>
	public class ComboBoxStyle : ImageTextButtonStyle
	{
		/// <summary>
		/// Gets or sets the style for the dropdown list box.
		/// </summary>
		public ListBoxStyle ListBoxStyle { get; set; }

		/// <summary>
		/// Initializes a new instance of the ComboBoxStyle class.
		/// </summary>
		public ComboBoxStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ComboBoxStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The combo box style to copy from.</param>
		public ComboBoxStyle(ComboBoxStyle style) : base(style)
		{
			ListBoxStyle = style.ListBoxStyle != null ? new ListBoxStyle(style.ListBoxStyle) : null;
		}

		/// <summary>
		/// Creates a copy of this combo box style.
		/// </summary>
		/// <returns>A new combo box style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new ComboBoxStyle(this);
		}
	}
}