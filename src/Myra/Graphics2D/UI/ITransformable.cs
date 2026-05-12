#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Numerics;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Provides an interface for coordinate transformation between local and global coordinate systems.
	/// </summary>
	internal interface ITransformable
	{
		/// <summary>
		/// Converts a global coordinate to local coordinates.
		/// </summary>
		/// <param name="source">The global coordinate to convert.</param>
		/// <returns>The converted local coordinate.</returns>
		Vector2 ToLocal(Vector2 source);

		/// <summary>
		/// Converts a local coordinate to global coordinates.
		/// </summary>
		/// <param name="pos">The local coordinate to convert.</param>
		/// <returns>The converted global coordinate.</returns>
		Vector2 ToGlobal(Vector2 pos);
	}
}
