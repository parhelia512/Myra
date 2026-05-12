using FontStashSharp.Interfaces;
using Myra.Graphics2D;
using System.Drawing;
using System.Numerics;
using Color = FontStashSharp.FSColor;

namespace Myra.Platform
{
	/// <summary>
	/// Specifies the type of rendering backend.
	/// </summary>
	public enum RendererType
	{
		/// <summary>
		/// Sprite-based rendering using sprite batches.
		/// </summary>
		Sprite,

		/// <summary>
		/// Quad-based rendering for direct vertex drawing.
		/// </summary>
		Quad
	}

	/// <summary>
	/// Provides platform-independent rendering interface for graphics operations.
	/// </summary>
	public interface IMyraRenderer
	{
		/// <summary>
		/// Gets the texture manager used for loading and managing textures.
		/// </summary>
		ITexture2DManager TextureManager { get; }

		/// <summary>
		/// Gets the type of renderer being used (sprite or quad).
		/// </summary>
		RendererType RendererType { get; }

		/// <summary>
		/// Gets or sets the scissor rectangle that limits the drawing area.
		/// </summary>
		Rectangle Scissor { get; set; }

		/// <summary>
		/// Prepares the graphics device for drawing with the specified texture filtering mode.
		/// </summary>
		/// <param name="textureFiltering">The texture filtering mode to use for drawing.</param>
		void Begin(TextureFiltering textureFiltering);

		/// <summary>
		/// Flushes any pending draw calls and ends the current rendering batch.
		/// </summary>
		void End();

		/// <summary>
		/// Draws a sprite to the screen.
		/// </summary>
		/// <param name="texture">The texture object to draw.</param>
		/// <param name="pos">The position to draw the sprite at.</param>
		/// <param name="src">The source rectangle within the texture, or null to draw the entire texture.</param>
		/// <param name="color">The color to tint the sprite with.</param>
		/// <param name="rotation">The rotation angle in radians.</param>
		/// <param name="scale">The scale factor for the sprite.</param>
		/// <param name="depth">The depth value for sorting (0 to 1).</param>
		void DrawSprite(object texture, Vector2 pos, Rectangle? src, Color color, float rotation, Vector2 scale, float depth);

		/// <summary>
		/// Draws a textured quad with individual vertex colors and positions.
		/// </summary>
		/// <param name="texture">The texture to apply to the quad.</param>
		/// <param name="topLeft">The top-left vertex with position, color, and texture coordinates.</param>
		/// <param name="topRight">The top-right vertex with position, color, and texture coordinates.</param>
		/// <param name="bottomLeft">The bottom-left vertex with position, color, and texture coordinates.</param>
		/// <param name="bottomRight">The bottom-right vertex with position, color, and texture coordinates.</param>
		void DrawQuad(object texture, ref VertexPositionColorTexture topLeft, ref VertexPositionColorTexture topRight, ref VertexPositionColorTexture bottomLeft, ref VertexPositionColorTexture bottomRight);
	}
}
