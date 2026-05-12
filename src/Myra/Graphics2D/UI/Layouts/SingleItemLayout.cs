using Myra.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

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
	/// A layout engine that arranges a single widget with optional alignment.
	/// </summary>
	/// <typeparam name="T">The type of widget to contain.</typeparam>
	public class SingleItemLayout<T> : ILayout where T : Widget
	{
		private readonly Widget _container;

		private ObservableCollection<Widget> Children => _container.Children;

		/// <summary>
		/// Gets or sets the single child widget.
		/// </summary>
		public T Child
		{
			get { return Children.Count > 0 ? (T)Children[0] : null; }
			set
			{
				Children.Clear();

				if (value != null)
				{
					Children.Add(value);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the SingleItemLayout class.
		/// </summary>
		/// <param name="container">The container widget that will hold the single item.</param>
		/// <exception cref="ArgumentNullException">Thrown when container is null.</exception>
		public SingleItemLayout(Widget container)
		{
			_container = container ?? throw new ArgumentNullException(nameof(container));
		}

		/// <summary>
		/// Measures the required size for the single child widget.
		/// </summary>
		/// <param name="widgets">The widgets to measure.</param>
		/// <param name="availableSize">The available size for the widget.</param>
		/// <returns>The required size for the child widget.</returns>
		public Point Measure(IEnumerable<Widget> widgets, Point availableSize)
		{
			var result = Mathematics.PointZero;

			if (Child != null)
			{
				result = Child.Measure(availableSize);
			}

			return result;
		}

		/// <summary>
		/// Arranges the child widget within the specified bounds.
		/// </summary>
		/// <param name="widgets">The widgets to arrange.</param>
		/// <param name="bounds">The bounds in which to arrange the widget.</param>
		public void Arrange(IEnumerable<Widget> widgets, Rectangle bounds)
		{
			if (Child != null && Child.Visible)
			{
				Child.Arrange(bounds);
			}
		}
	}
}
