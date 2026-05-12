using System.Runtime.InteropServices;

namespace Myra.Graphics2D.UI.TextEdit
{
	/// <summary>
	/// Specifies the type of text editing operation.
	/// </summary>
	internal enum OperationType
	{
		/// <summary>Text insertion operation.</summary>
		Insert,
		/// <summary>Text deletion operation.</summary>
		Delete,
		/// <summary>Text replacement operation.</summary>
		Replace
	}

	/// <summary>
	/// Represents a record of a text editing operation for undo/redo functionality.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	internal class UndoRedoRecord
	{
		/// <summary>
		/// Gets or sets the type of operation recorded.
		/// </summary>
		public OperationType OperationType;

		/// <summary>
		/// Gets or sets the text data associated with the operation.
		/// </summary>
		public string Data;

		/// <summary>
		/// Gets or sets the position where the operation occurred.
		/// </summary>
		public int Where;

		/// <summary>
		/// Gets or sets the length of text affected by the operation.
		/// </summary>
		public int Length;
	}
}