using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#elif STRIDE
using Stride.Core.Mathematics;
using Texture2D = Stride.Graphics.Texture;
#else
using System.Drawing;
using Texture2D = System.Object;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.TextureAtlases
{
	/// <summary>
	/// Represents a region within a texture that can be drawn.
	/// </summary>
	public class TextureRegion: IImage
	{
		private readonly Rectangle _bounds;

#if MONOGAME || FNA || STRIDE
		private readonly Texture2D _texture;
		/// <summary>
		/// Gets the texture associated with this region.
		/// </summary>
		public Texture2D Texture
		{
			get { return _texture; }
		}
#else
		private readonly object _texture;
		/// <summary>
		/// Gets the texture associated with this region.
		/// </summary>
		public object Texture
		{
			get { return _texture; }
		}
#endif

		/// <summary>
		/// Gets the bounds of this texture region within the texture.
		/// </summary>
		public Rectangle Bounds
		{
			get { return _bounds; }
		}

		/// <summary>
		/// Gets the size of this texture region.
		/// </summary>
		public Point Size
		{
			get
			{
				return new Point(Bounds.Width, Bounds.Height);
			}
		}

#if MONOGAME || FNA || STRIDE
		/// <summary>
		/// Initializes a new instance of the <see cref="TextureRegion"/> class that covers the whole texture.
		/// </summary>
		/// <param name="texture">The texture to cover.</param>
		public TextureRegion(Texture2D texture) : this(texture, new Rectangle(0, 0, texture.Width, texture.Height))
		{
		}

#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="TextureRegion"/> class with a specific texture and bounds.
		/// </summary>
		/// <param name="texture">The texture for this region.</param>
		/// <param name="bounds">The bounds within the texture.</param>
		/// <exception cref="ArgumentNullException">Thrown if texture is null.</exception>
		public TextureRegion(Texture2D texture, Rectangle bounds)
		{
			if (texture == null)
			{
				throw new ArgumentNullException("texture");
			}

			_texture = texture;
			_bounds = bounds;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextureRegion"/> class from another region with new bounds.
		/// </summary>
		/// <param name="region">The source texture region.</param>
		/// <param name="bounds">The bounds for the new region, relative to the source region.</param>
		/// <exception cref="ArgumentNullException">Thrown if region is null.</exception>
		public TextureRegion(TextureRegion region, Rectangle bounds)
		{
			if (region == null)
			{
				throw new ArgumentNullException("region");
			}

			_texture = region.Texture;
			bounds.Offset(region.Bounds.Location);
			_bounds = bounds;
		}

		/// <summary>
		/// Draws this texture region to the render context.
		/// </summary>
		/// <param name="context">The render context to draw to.</param>
		/// <param name="dest">The destination rectangle for drawing.</param>
		/// <param name="color">The color to apply during drawing.</param>
		public virtual void Draw(RenderContext context, Rectangle dest, Color color)
		{
			context.Draw(Texture, dest, Bounds, color);
		}
	}
}