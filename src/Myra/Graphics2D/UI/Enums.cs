namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Specifies horizontal alignment options.
	/// </summary>
	public enum HorizontalAlignment
	{
		/// <summary>
		/// Align to the left.
		/// </summary>
		Left,

		/// <summary>
		/// Align to the center.
		/// </summary>
		Center,

		/// <summary>
		/// Align to the right.
		/// </summary>
		Right,

		/// <summary>
		/// Stretch to fill available space.
		/// </summary>
		Stretch
	}

	/// <summary>
	/// Specifies vertical alignment options.
	/// </summary>
	public enum VerticalAlignment
	{
		/// <summary>
		/// Align to the top.
		/// </summary>
		Top,

		/// <summary>
		/// Align to the center.
		/// </summary>
		Center,

		/// <summary>
		/// Align to the bottom.
		/// </summary>
		Bottom,

		/// <summary>
		/// Stretch to fill available space.
		/// </summary>
		Stretch
	}

	/// <summary>
	/// Specifies mouse button types.
	/// </summary>
	public enum MouseButtons
	{
		/// <summary>
		/// Left mouse button.
		/// </summary>
		Left,

		/// <summary>
		/// Middle mouse button.
		/// </summary>
		Middle,

		/// <summary>
		/// Right mouse button.
		/// </summary>
		Right
	}

	/// <summary>
	/// Specifies layout orientation.
	/// </summary>
	public enum Orientation
	{
		/// <summary>
		/// Horizontal orientation.
		/// </summary>
		Horizontal,

		/// <summary>
		/// Vertical orientation.
		/// </summary>
		Vertical
	}

	/// <summary>
	/// Specifies mouse cursor types.
	/// </summary>
	public enum MouseCursorType
	{
		/// <summary>
		/// Standard arrow cursor.
		/// </summary>
		Arrow,

		/// <summary>
		/// Text input (I-beam) cursor.
		/// </summary>
		IBeam,

		/// <summary>
		/// Wait/busy cursor.
		/// </summary>
		Wait,

		/// <summary>
		/// Crosshair cursor.
		/// </summary>
		Crosshair,

		/// <summary>
		/// Wait with arrow cursor.
		/// </summary>
		WaitArrow,

		/// <summary>
		/// Resize northwest-southeast cursor.
		/// </summary>
		SizeNWSE,

		/// <summary>
		/// Resize northeast-southwest cursor.
		/// </summary>
		SizeNESW,

		/// <summary>
		/// Resize west-east cursor.
		/// </summary>
		SizeWE,

		/// <summary>
		/// Resize north-south cursor.
		/// </summary>
		SizeNS,

		/// <summary>
		/// Resize all directions cursor.
		/// </summary>
		SizeAll,

		/// <summary>
		/// Prohibited/no action cursor.
		/// </summary>
		No,

		/// <summary>
		/// Hand/pointer cursor.
		/// </summary>
		Hand,
	}
}