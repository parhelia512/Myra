#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D
{
	/// <summary>
	/// Provides an interface for images that can be drawn.
	/// </summary>
	public interface IImage: IBrush
	{
		/// <summary>
		/// Gets the size of the image.
		/// </summary>
		Point Size { get; }
	}
}
