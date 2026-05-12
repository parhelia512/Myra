using System;

namespace Myra.Events
{
	/// <summary>
	/// Provides event arguments for cancellable events that carry typed data.
	/// </summary>
	/// <typeparam name="T">The type of data carried by the event.</typeparam>
	public class CancellableEventArgs<T> : EventArgs
	{
		/// <summary>
		/// Gets the data associated with the event.
		/// </summary>
		public T Data { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CancellableEventArgs{T}"/> class.
		/// </summary>
		/// <param name="data">The data to associate with the event.</param>
		public CancellableEventArgs(T data)
		{
			Data = data;
		}
	}
}