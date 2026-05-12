namespace Myra.Events
{
	/// <summary>
	/// Provides event arguments for value changing events that can be cancelled.
	/// </summary>
	/// <typeparam name="T">The type of the value being changed.</typeparam>
	public class ValueChangingEventArgs<T> : CancellableEventArgs
	{
		/// <summary>
		/// Gets the old value before the change.
		/// </summary>
		public T OldValue { get; private set; }

		/// <summary>
		/// Gets or sets the new value after the change.
		/// </summary>
		public T NewValue { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChangingEventArgs{T}"/> class.
		/// </summary>
		/// <param name="oldValue">The old value before the change.</param>
		/// <param name="newValue">The new value after the change.</param>
		public ValueChangingEventArgs(T oldValue, T newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
