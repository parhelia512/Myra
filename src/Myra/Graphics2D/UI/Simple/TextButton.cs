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
	/// A text button control with customizable font and colors.
	/// </summary>
	/// <remarks>This class is obsolete. Use Button with a Label as content instead.</remarks>
	[Obsolete("Switch to Button")]
	[StyleTypeName("Button")]
	public class TextButton : ButtonBase<Label>
	{
		/// <summary>
		/// Gets or sets the text content of this button.
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
		/// Gets or sets the text color of this button.
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
		/// Gets or sets the text color when the mouse is over this button.
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
		/// Gets or sets the text color when this button is pressed.
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
		/// Initializes a new instance of the TextButton class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
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

		public void ApplyTextButtonStyle(ButtonStyle style)
		{
			ApplyButtonStyle(style);

			if (style.LabelStyle != null)
			{
				InternalChild.ApplyLabelStyle(style.LabelStyle);
			}
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			InternalChild.IsPressed = IsPressed;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyTextButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name));
		}
	}
}