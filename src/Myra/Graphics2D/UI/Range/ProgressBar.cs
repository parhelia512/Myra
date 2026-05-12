using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Base abstract class for progress bar controls.
	/// </summary>
	public abstract class ProgressBar : Widget
	{
		private float _value;

		/// <summary>
		/// Gets the orientation (horizontal or vertical) of this progress bar.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public abstract Orientation Orientation { get; }

		/// <summary>
		/// Gets or sets the minimum value of the progress bar.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(0.0f)]
		public float Minimum { get; set; }

		/// <summary>
		/// Gets or sets the maximum value of the progress bar.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(100.0f)]
		public float Maximum { get; set; }

		/// <summary>
		/// Gets or sets the current value of the progress bar.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(0.0f)]
		public float Value
		{
			get
			{
				return _value;
			}

			set
			{
				if (_value.EpsilonEquals(value))
				{
					return;
				}

				_value = value;

				ValueChanged.Invoke(this);
			}
		}

		/// <summary>
		/// Gets or sets the brush used to render the filled portion of the progress bar.
		/// </summary>
		[Category("Appearance")]
		public IBrush Filler { get; set; }

		/// <summary>
		/// Raised when the value of the progress bar changes.
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressBar"/> class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		protected ProgressBar(string styleName)
		{
			Maximum = 100;
			SetStyle(styleName);
		}

		/// <summary>
		/// Applies a progress bar style to this progress bar.
		/// </summary>
		/// <param name="style">The style to apply.</param>
		public void ApplyProgressBarStyle(ProgressBarStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.Filler == null)
				return;

			Filler = style.Filler;
		}

		public override void InternalRender(RenderContext context)
		{
			base.InternalRender(context);

			if (Filler == null)
			{
				return;
			}

			var v = _value;
			if (v < Minimum)
			{
				v = Minimum;
			}

			if (v > Maximum)
			{
				v = Maximum;
			}

			var delta = Maximum - Minimum;
			if (delta.IsZero())
			{
				return;
			}

			var filledPart = (v - Minimum) / delta;
			if (filledPart.EpsilonEquals(0.0f))
			{
				return;
			}

			var bounds = ActualBounds;
			if (Orientation == Orientation.Horizontal)
			{
				Filler.Draw(context,
					new Rectangle(bounds.X, bounds.Y, (int)(filledPart * bounds.Width), bounds.Height),
					Color.White);
			}
			else
			{
				Filler.Draw(context,
					new Rectangle(bounds.X, bounds.Y, bounds.Width, (int)(filledPart * bounds.Height)),
					Color.White);
			}
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var progressBar = (ProgressBar)w;

			Minimum = progressBar.Minimum;
			Maximum = progressBar.Maximum;
			Value = progressBar.Value;
			Filler = progressBar.Filler;
		}
	}
}