using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Utility;

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
	/// Abstract base class for separator widgets that display a line dividing sections.
	/// </summary>
	public abstract class SeparatorWidget : Image
	{
		/// <summary>
		/// Gets or sets the thickness of the separator line.
		/// </summary>
		public int Thickness { get; set; }

		/// <summary>
		/// Gets the orientation (horizontal or vertical) of this separator.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		/// <summary>
		/// Initializes a new instance of the SeparatorWidget class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		protected SeparatorWidget(string styleName)
		{
			SetStyle(styleName);
		}

		/// <summary>
		/// Applies the specified separator style to this separator widget.
		/// </summary>
		/// <param name="style">The style to apply.</param>
		public void ApplySeparatorStyle(SeparatorStyle style)
		{
			ApplyWidgetStyle(style);

			Renderable = style.Image;
			Thickness = style.Thickness;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			var result = Mathematics.PointZero;

			if (Orientation == Orientation.Horizontal)
			{
				result.Y = Thickness;
			}
			else
			{
				result.X = Thickness;
			}

			return result;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var separator = (SeparatorWidget)w;
			Thickness = separator.Thickness;
		}
	}
}