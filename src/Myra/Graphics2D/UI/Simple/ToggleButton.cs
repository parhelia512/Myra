using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System;

#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#else
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A toggle button that can be toggled on and off by clicking or pressing Space.
	/// </summary>
	[StyleTypeName("Button")]
	public class ToggleButton : ButtonBase2
	{
		private readonly SingleItemLayout<Widget> _layout;

		/// <summary>
		/// Gets or sets a value indicating whether this toggle button is currently toggled on.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool IsToggled
		{
			get => IsPressed;
			set => IsPressed = value;
		}

		/// <summary>
		/// Gets or sets the content widget displayed inside this toggle button.
		/// </summary>
		[Browsable(false)]
		[Content]
		public override Widget Content
		{
			get => _layout.Child;
			set => _layout.Child = value;
		}

		/// <summary>
		/// Raised when the toggle state changes.
		/// </summary>
		public event EventHandler IsToggledChanged
		{
			add
			{
				PressedChanged += value;
			}

			remove
			{
				PressedChanged -= value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the ToggleButton class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public ToggleButton(string styleName = Stylesheet.DefaultStyleName)
		{
			_layout = new SingleItemLayout<Widget>(this);
			ChildrenLayout = _layout;
			SetStyle(styleName);
		}

		protected override void InternalOnTouchUp()
		{
		}

		protected override void InternalOnTouchDown()
		{
			SetValueByUser(!IsPressed);
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (!Enabled)
			{
				return;
			}

			if (k == Keys.Space)
			{
				SetValueByUser(!IsPressed);
			}
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name));
		}
	}
}
