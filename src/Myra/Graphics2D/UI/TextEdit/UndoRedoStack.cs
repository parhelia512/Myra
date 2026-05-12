using System.Collections.Generic;

namespace Myra.Graphics2D.UI.TextEdit
{
	/// <summary>
	/// A stack of undo/redo records for text editing operations.
	/// </summary>
	internal class UndoRedoStack
	{
		private readonly Stack<UndoRedoRecord> _stack = new Stack<UndoRedoRecord>();

		/// <summary>
		/// Gets the underlying stack of undo/redo records.
		/// </summary>
		public Stack<UndoRedoRecord> Stack
		{
			get
			{
				return _stack;
			}
		}

		/// <summary>
		/// Clears all records from the stack.
		/// </summary>
		public void Reset()
		{
			_stack.Clear();
		}

		/// <summary>
		/// Records a text insertion operation.
		/// </summary>
		/// <param name="where">The position where text was inserted.</param>
		/// <param name="length">The length of text inserted.</param>
		public void MakeInsert(int where, int length)
		{
			if (length <= 0)
			{
				return;
			}

			var record = new UndoRedoRecord
			{
				OperationType = OperationType.Insert,
				Where = where,
				Length = length
			};

			_stack.Push(record);
		}

		/// <summary>
		/// Records a text deletion operation.
		/// </summary>
		/// <param name="text">The original text before deletion.</param>
		/// <param name="where">The position where text was deleted.</param>
		/// <param name="length">The length of text deleted.</param>
		public void MakeDelete(string text, int where, int length)
		{
			if (length <= 0)
			{
				return;
			}

			var record = new UndoRedoRecord
			{
				OperationType = OperationType.Delete,
				Where = where,
				Length = length,
				Data = text.Substring(where, length)
			};

			_stack.Push(record);
		}

		/// <summary>
		/// Records a text replacement operation.
		/// </summary>
		/// <param name="text">The original text before replacement.</param>
		/// <param name="where">The position where text was replaced.</param>
		/// <param name="length">The length of text being replaced.</param>
		/// <param name="newLength">The length of the replacement text.</param>
		public void MakeReplace(string text, int where, int length, int newLength)
		{
			if (length <= 0)
			{
				MakeInsert(where, newLength);
				return;
			}

			if (newLength <= 0)
			{
				MakeDelete(text, where, length);
				return;
			}

			var record = new UndoRedoRecord
			{
				OperationType = OperationType.Replace,
				Where = where,
				Length = newLength,
				Data = text.Substring(where, length)
			};

			_stack.Push(record);
		}
	}
}