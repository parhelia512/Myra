#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D
{
	/// <summary>
	/// Provides an interface for drawing brushes in rendering operations.
	/// </summary>
	public interface IBrush
	{
		/// <summary>
		/// Draws the brush content to the specified render context.
		/// </summary>
		/// <param name="context">The render context to draw to.</param>
		/// <param name="dest">The destination rectangle for drawing.</param>
		/// <param name="color">The color to apply during drawing.</param>
		void Draw(RenderContext context, Rectangle dest, Color color);
	}

	/// <summary>
	/// Provides extension methods for the <see cref="IBrush"/> interface.
	/// </summary>
	public static class IBrushExtensions
	{
		/// <summary>
		/// Draws the brush with a white color to the specified render context.
		/// </summary>
		/// <param name="brush">The brush to draw.</param>
		/// <param name="context">The render context to draw to.</param>
		/// <param name="dest">The destination rectangle for drawing.</param>
		public static void Draw(this IBrush brush, RenderContext context, Rectangle dest)
		{
			brush.Draw(context, dest, Color.White);
		}
	}
}
