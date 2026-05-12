using FontStashSharp.RichText;
using System;
using Myra.Graphics2D.UI.Styles;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.Brushes
{
	/// <summary>
	/// A brush that renders a solid color.
	/// </summary>
	public class SolidBrush : IBrush
	{
		private Color _color = Color.White;

		/// <summary>
		/// Gets or sets the color of the brush.
		/// </summary>
		public Color Color
		{
			get
			{
				return _color;
			}

			set
			{
				_color = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SolidBrush"/> class with a color.
		/// </summary>
		/// <param name="color">The color for the brush.</param>
		public SolidBrush(Color color)
		{
			Color = color;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SolidBrush"/> class with a color name.
		/// </summary>
		/// <param name="color">The name of the color for the brush.</param>
		/// <exception cref="ArgumentException">Thrown if the color name is not recognized.</exception>
		public SolidBrush(string color)
		{
			var c = ColorStorage.FromName(color);
			if (c == null)
			{
				throw new ArgumentException(string.Format("Could not recognize color '{0}'", color));
			}

			Color = c.Value;
		}

		/// <summary>
		/// Draws the solid color brush to the render context.
		/// </summary>
		/// <param name="context">The render context to draw to.</param>
		/// <param name="dest">The destination rectangle for drawing.</param>
		/// <param name="color">The color to apply during drawing.</param>
		public void Draw(RenderContext context, Rectangle dest, Color color)
		{
			var white = Stylesheet.Current.WhiteRegion;

			if (color == Color.White)
			{
				white.Draw(context, dest, Color);
			}
			else
			{
				var c = new Color((int)(Color.R * color.R / 255.0f),
					(int)(Color.G * color.G / 255.0f),
					(int)(Color.B * color.B / 255.0f),
					(int)(Color.A * color.A / 255.0f));

				white.Draw(context, dest, c);
			}
		}
	}
}