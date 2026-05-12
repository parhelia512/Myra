using System.Collections.Generic;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Provides an interface for layout engines that measure and arrange widgets.
	/// </summary>
	public interface ILayout
	{
		/// <summary>
		/// Measures the size required for the specified widgets.
		/// </summary>
		/// <param name="widgets">The widgets to measure.</param>
		/// <param name="availableSize">The available size for the widgets.</param>
		/// <returns>The required size for the widgets.</returns>
		Point Measure(IEnumerable<Widget> widgets, Point availableSize);

		/// <summary>
		/// Arranges the specified widgets within the given bounds.
		/// </summary>
		/// <param name="widgets">The widgets to arrange.</param>
		/// <param name="bounds">The bounds to arrange the widgets within.</param>
		void Arrange(IEnumerable<Widget> widgets, Rectangle bounds);
	}
}
