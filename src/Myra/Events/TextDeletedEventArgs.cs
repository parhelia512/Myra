using System;

namespace Myra.Events
{
	/// <summary>
	/// Provides event arguments for text deletion events.
	/// </summary>
	public class TextDeletedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the starting position of the deleted text.
		/// </summary>
		public int StartPosition { get; }

		/// <summary>
		/// Gets the deleted text value.
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TextDeletedEventArgs"/> class.
		/// </summary>
		/// <param name="startPosition">The starting position of the deleted text.</param>
		/// <param name="value">The deleted text value.</param>
		public TextDeletedEventArgs(int startPosition, string value)
		{
			StartPosition = startPosition;
			Value = value;
		}
	}
}