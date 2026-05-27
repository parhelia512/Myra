using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Attributes;
using FontStashSharp;
using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A button widget that displays text.
	/// </summary>
	/// <remarks>This class is obsolete. Use <see cref="Button"/> instead.</remarks>
	[Obsolete("Switch to Button")]
	[StyleTypeName("Button")]
	public class TextButton : ButtonBase<Label>
	{
		/// <summary>
		/// Gets or sets the text displayed on the button.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(null)]
		public string Text
		{
			get
			{
				return InternalChild.Text;
			}
			set
			{
				InternalChild.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the color of the button text.
		/// </summary>
		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/TextColor")]
		public Color TextColor
		{
			get
			{
				return InternalChild.TextColor;
			}
			set
			{
				InternalChild.TextColor = value;
			}
		}

		/// <summary>
		/// Gets or sets the color of the button text when the cursor is over it.
		/// </summary>
		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/OverTextColor")]
		public Color? OverTextColor
		{
			get
			{
				return InternalChild.OverTextColor;
			}
			set
			{
				InternalChild.OverTextColor = value;
			}
		}

		/// <summary>
		/// Gets or sets the color of the button text when the button is pressed.
		/// </summary>
		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/PressedTextColor")]
		public Color? PressedTextColor
		{
			get
			{
				return InternalChild.PressedTextColor;
			}
			set
			{
				InternalChild.PressedTextColor = value;
			}
		}

		/// <summary>
		/// Gets or sets the font used to render the button text.
		/// </summary>
		[Category("Appearance")]
		public SpriteFontBase Font
		{
			get
			{
				return InternalChild.Font;
			}
			set
			{
				InternalChild.Font = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextButton"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public TextButton(string styleName = Stylesheet.DefaultStyleName)
		{
			InternalChild = new Label(null)
			{
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				Wrap = true
			};

			SetStyle(styleName);
		}

		/// <summary>
		/// Applies the specified button style to the text button and its text label.
		/// </summary>
		/// <param name="style">The button style to apply.</param>
		public void ApplyTextButtonStyle(ButtonStyle style)
		{
			ApplyButtonStyle(style);

			if (style.LabelStyle != null)
			{
				InternalChild.ApplyLabelStyle(style.LabelStyle);
			}
		}

		/// <summary>
		/// Handles the pressed state change and updates the text label's pressed state.
		/// </summary>
		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			InternalChild.IsPressed = IsPressed;
		}

		/// <summary>
		/// Applies a named button style from the stylesheet to the text button.
		/// </summary>
		/// <param name="stylesheet">The stylesheet containing the style.</param>
		/// <param name="name">The name of the button style to apply.</param>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyTextButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name));
		}
	}
}