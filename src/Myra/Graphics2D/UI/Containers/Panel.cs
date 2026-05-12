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
	/// A container that stacks child widgets and arranges them to overlap within the same bounds.
	/// </summary>
	public class Panel : Container
	{
		protected override void InternalArrange()
		{
			foreach (var control in ChildrenCopy)
			{
				if (!control.Visible)
				{
					continue;
				}

				LayoutControl(control);
			}
		}

		private void LayoutControl(Widget control)
		{
			control.Arrange(ActualBounds);
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			Point result = Mathematics.PointZero;

			foreach (var control in ChildrenCopy)
			{
				if (!control.Visible)
				{
					continue;
				}

				Point measure = control.Measure(availableSize);

				if (measure.X > result.X)
				{
					result.X = measure.X;
				}

				if (measure.Y > result.Y)
				{
					result.Y = measure.Y;
				}
			}

			return result;
		}
	}
}