using System;

namespace Myra.Events
{
	/// <summary>
	/// Provides event arguments for cancellable events.
	/// </summary>
	public class CancellableEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets a value indicating whether the event should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CancellableEventArgs"/> class.
		/// </summary>
		public CancellableEventArgs()
		{
		}
	}
}
