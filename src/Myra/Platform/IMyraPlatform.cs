using FontStashSharp.Interfaces;
using Myra.Graphics2D.UI;
using System.Drawing;

namespace Myra.Platform
{
	/// <summary>
	/// Provides platform-specific functionality for rendering, input handling, and windowing.
	/// </summary>
	public interface IMyraPlatform
	{
		/// <summary>
		/// Gets the size of the rendering view in pixels.
		/// </summary>
		Point ViewSize { get; }

		/// <summary>
		/// Gets the renderer used for drawing graphics to the screen.
		/// </summary>
		IMyraRenderer Renderer { get; }

		/// <summary>
		/// Gets the current state of the mouse, including position and button states.
		/// </summary>
		/// <returns>A MouseInfo struct containing mouse state information.</returns>
		MouseInfo GetMouseInfo();

		/// <summary>
		/// Updates the state of keyboard keys.
		/// The index of the array represents the value of <see cref="Keys"/>.
		/// Set array value to true if a key is pressed, and to false if a key is released.
		/// </summary>
		/// <param name="keys">An array of boolean values representing key states.</param>
		void SetKeysDown(bool[] keys);

		/// <summary>
		/// Sets the mouse cursor type to display.
		/// </summary>
		/// <param name="mouseCursorType">The type of cursor to display.</param>
		void SetMouseCursorType(MouseCursorType mouseCursorType);

		/// <summary>
		/// Gets the current state of all touch inputs.
		/// </summary>
		/// <returns>A TouchCollection containing the states of all active touch points.</returns>
		TouchCollection GetTouchState();
	}
}
