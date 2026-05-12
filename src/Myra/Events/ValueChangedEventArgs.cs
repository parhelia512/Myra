using System;

namespace Myra.Events
{
	/// <summary>
	/// Provides event arguments for value changed events.
	/// </summary>
	/// <typeparam name="T">The type of the value being changed.</typeparam>
	public class ValueChangedEventArgs<T> : EventArgs
	{
		/// <summary>
		/// Gets the old value before the change.
		/// </summary>
		public T OldValue { get; private set; }

		/// <summary>
		/// Gets the new value after the change.
		/// </summary>
		public T NewValue { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangedEventArgs{T}"/> class.
		/// </summary>
		/// <param name="oldValue">The old value before the change.</param>
		/// <param name="newValue">The new value after the change.</param>
		public ValueChangedEventArgs(T oldValue, T newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
