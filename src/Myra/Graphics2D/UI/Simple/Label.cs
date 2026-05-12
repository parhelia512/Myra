using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System;
using FontStashSharp;
using Myra.Utility;
using FontStashSharp.RichText;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using System.Numerics;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A text display widget that renders text with optional wrapping, rich text commands, and multi-color support.
	/// </summary>
	public class Label : Widget
	{
		private readonly RichTextLayout _richText = new RichTextLayout
		{
			SupportsCommands = true
		};

		private bool _wrap = false;

		private readonly RichTextLayout _errorText = new RichTextLayout
		{
			SupportsCommands = false
		};

		private bool _singleLine = false;

		/// <summary>
		/// Gets or sets the vertical spacing between lines of text.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(0)]
		public int VerticalSpacing
		{
			get
			{
				return _richText.VerticalSpacing;
			}
			set
			{
				_richText.VerticalSpacing = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the text content to display. Supports rich text commands for styling.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(null)]
		public string Text
		{
			get
			{
				return _richText.Text;
			}
			set
			{
				if (_richText.Text == value)
				{
					return;
				}

				_richText.Text = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the font used to render the text.
		/// </summary>
		[Category("Appearance")]
		public SpriteFontBase Font
		{
			get
			{
				return _richText.Font;
			}
			set
			{
				_richText.Font = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether text wraps to the next line when it exceeds the available width.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(false)]
		public bool Wrap
		{
			get
			{
				return _wrap;
			}

			set
			{
				if (value == _wrap)
				{
					return;
				}

				_wrap = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the label displays text on a single line, clipping any overflow.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(false)]
		public bool SingleLine
		{
			get
			{
				return _singleLine;
			}

			set
			{
				if (value == _singleLine)
				{
					return;
				}

				_singleLine = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the method used to abbreviate text that overflows its bounds.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(AutoEllipsisMethod.None)]
		public AutoEllipsisMethod AutoEllipsisMethod
		{
			get => _richText.AutoEllipsisMethod;
			set => _richText.AutoEllipsisMethod = value;
		}

		/// <summary>
		/// Gets or sets the string to use as the ellipsis when abbreviating overflowing text.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("...")]
		public string AutoEllipsisString
		{
			get => _richText.AutoEllipsisString;
			set => _richText.AutoEllipsisString = value;
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the text within the label.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(TextHorizontalAlignment.Left)]
		public TextHorizontalAlignment TextAlign
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the color of the text.
		/// </summary>
		[Category("Appearance")]
		public Color TextColor
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the color of the text when the label is disabled.
		/// </summary>
		[Category("Appearance")]
		public Color? DisabledTextColor
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the color of the text when the mouse is over the label.
		/// </summary>
		[Category("Appearance")]
		public Color? OverTextColor
		{
			get; set;
		}

		internal Color? PressedTextColor
		{
			get; set;
		}

		internal bool IsPressed
		{
			get; set;
		}

		/// <summary>
		/// Initializes a new instance of the Label class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public Label(string styleName = Stylesheet.DefaultStyleName)
		{
			SetStyle(styleName);
		}

		public override void InternalRender(RenderContext context)
		{
			if (_richText.Font == null)
			{
				return;
			}

			var color = TextColor;
			var useChunkColor = true;
			if (!Enabled && DisabledTextColor != null)
			{
				color = DisabledTextColor.Value;
				useChunkColor = false;
			}
			else if (IsPressed && PressedTextColor != null)
			{
				color = PressedTextColor.Value;
				useChunkColor = false;
			}
			else if (IsMouseInside && OverTextColor != null)
			{
				color = OverTextColor.Value;
				useChunkColor = false;
			}

			var textToDraw = _richText;

			textToDraw.IgnoreColorCommand = !useChunkColor;
			var bounds = ActualBounds;

			var x = bounds.X;
			if (TextAlign == TextHorizontalAlignment.Center)
			{
				x += bounds.Width / 2;
			}
			else if (TextAlign == TextHorizontalAlignment.Right)
			{
				x += bounds.Width;
			}

			try
			{
				context.DrawRichText(textToDraw, new Vector2(x, bounds.Y), color, horizontalAlignment: TextAlign);
			}
			catch (Exception ex)
			{
				x = bounds.X;
				_errorText.Font = Font;
				_errorText.Text = BuildRtlError(ex);
				context.DrawRichText(_errorText, new Vector2(x, bounds.Y), Color.Red);
			}
		}

		private static string BuildRtlError(Exception ex)
		{
			return "RTL Error: " + ex.Message;
		}

		protected override Point InternalMeasure(Point availableSize)
		{
			if (Font == null)
			{
				return Mathematics.PointZero;
			}

			var width = availableSize.X;
			var height = availableSize.Y;

			var result = Mathematics.PointZero;
			try
			{
				result = _richText.Measure(_wrap ? width : default(int?));
			}
			catch (Exception ex)
			{
				_errorText.Font = Font;
				_errorText.Text = BuildRtlError(ex);
				result = _errorText.Measure(_wrap ? width : default(int?));
			}

			if (result.Y < Font.LineHeight)
			{
				result.Y = Font.LineHeight;
			}

			return result;
		}

		protected override void InternalArrange()
		{
			base.InternalArrange();

			if (_singleLine)
			{
				_richText.Width = ActualBounds.Width;
				_richText.Height = Font.LineHeight;
			}
			else if (_wrap)
			{
				_richText.Width = ActualBounds.Width;
				_richText.Height = ActualBounds.Height;
			}
			else
			{
				_richText.Width = default(int?);
				_richText.Height = default(int?);
			}
		}

		/// <summary>
		/// Applies the specified label style to this label.
		/// </summary>
		/// <param name="style">The style to apply.</param>
		public void ApplyLabelStyle(LabelStyle style)
		{
			ApplyWidgetStyle(style);

			TextColor = style.TextColor;
			DisabledTextColor = style.DisabledTextColor;
			OverTextColor = style.OverTextColor;
			PressedTextColor = style.PressedTextColor;
			Font = style.Font;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyLabelStyle(stylesheet.LabelStyles.SafelyGetStyle(name));
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var label = (Label)w;

			VerticalSpacing = label.VerticalSpacing;
			Text = label.Text;
			Font = label.Font;
			Wrap = label.Wrap;
			AutoEllipsisMethod = label.AutoEllipsisMethod;
			AutoEllipsisString = label.AutoEllipsisString;
			TextAlign = label.TextAlign;
			TextColor = label.TextColor;
			DisabledTextColor = label.DisabledTextColor;
			OverTextColor = label.OverTextColor;
			PressedTextColor = label.PressedTextColor;
		}
	}
}