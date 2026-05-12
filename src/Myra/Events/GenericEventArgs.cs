using System;

namespace Myra.Events
{
	/// <summary>
	/// Provides event arguments that carry typed data.
	/// </summary>
	/// <typeparam name="T">The type of data carried by the event.</typeparam>
	public sealed class GenericEventArgs<T> : EventArgs
	{
		/// <summary>
		/// Gets the data associated with the event.
		/// </summary>
		public T Data { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericEventArgs{T}"/> class.
		/// </summary>
		/// <param name="value">The data to associate with the event.</param>
		public GenericEventArgs(T value)
		{
			Data = value;
		}
	}
}
