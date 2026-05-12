using Myra.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;




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
	/// Provides an interface for containers that hold multiple widgets.
	/// </summary>
	public interface IContainer
	{
		/// <summary>
		/// Gets the collection of child widgets in this container.
		/// </summary>
		IList<Widget> Widgets { get; }
	}

	/// <summary>
	/// Base abstract class for containers that hold multiple child widgets.
	/// </summary>
	public abstract class Container : Widget, IContainer
	{
		/// <summary>
		/// Gets the collection of child widgets in this container.
		/// </summary>
		[Content]
		[Browsable(false)]
		public virtual IList<Widget> Widgets => Children;

		/// <summary>
		/// Gets or sets the horizontal alignment of this container.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get { return base.HorizontalAlignment; }
			set { base.HorizontalAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the vertical alignment of this container.
		/// </summary>
		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment VerticalAlignment
		{
			get { return base.VerticalAlignment; }
			set { base.VerticalAlignment = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Container"/> class.
		/// </summary>
		public Container()
		{
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
		}

		/// <summary>
		/// Adds a child widget to this container. This method is obsolete; use <see cref="Widgets"/>.Add instead.
		/// </summary>
		/// <param name="child">The child widget to add.</param>
		[Obsolete("Use Widgets.Add")]
		public void AddChild(Widget child)
		{
			Widgets.Add(child);
		}

		/// <summary>
		/// Removes a child widget from this container. This method is obsolete; use <see cref="Widgets"/>.Remove instead.
		/// </summary>
		/// <param name="child">The child widget to remove.</param>
		[Obsolete("Use Widgets.Remove")]
		public void RemoveChild(Widget child)
		{
			Widgets.Remove(child);
		}

		/// <summary>
		/// Determines whether input events fall through to widgets behind this container.
		/// </summary>
		/// <param name="localPos">The local position to check.</param>
		/// <returns>true if input falls through (no background is set); otherwise, false.</returns>
		public override bool InputFallsThrough(Point localPos) => Background == null;

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var container = (Container)w;
			foreach(var child in container.Widgets)
			{
				Widgets.Add(child.Clone());
			}
		}
	}
}