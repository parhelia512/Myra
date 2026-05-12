using System.Numerics;

namespace Myra.Platform
{
	/// <summary>
	/// Represents the location of a touch input on the screen.
	/// </summary>
	public struct TouchLocation
	{
		/// <summary>
		/// Gets or sets the position of the touch input in screen coordinates.
		/// </summary>
		public Vector2 Position { get; set; }
	}
}
