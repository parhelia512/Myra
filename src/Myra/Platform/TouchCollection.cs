using System;
using System.Collections.Generic;

namespace Myra.Platform
{
	/// <summary>
	/// Represents a collection of touch input locations.
	/// </summary>
	public struct TouchCollection
	{
		/// <summary>
		/// An empty touch collection with no touches.
		/// </summary>
		public static readonly TouchCollection Empty = new TouchCollection();

		private List<TouchLocation> _touches;

		/// <summary>
		/// Gets or sets the list of active touch locations.
		/// </summary>
		public List<TouchLocation> Touches
		{
			get => _touches;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_touches = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether any touch input is available.
		/// </summary>
		public bool IsConnected { get; set; }

		/// <summary>
		/// Gets the number of active touch points in this collection.
		/// </summary>
		public int Count { get => Touches.Count; }

		/// <summary>
		/// Gets the touch location at the specified index.
		/// </summary>
		/// <param name="index">The index of the touch location.</param>
		/// <returns>The touch location at the specified index.</returns>
		public TouchLocation this[int index] { get => Touches[index]; }
	}
}
