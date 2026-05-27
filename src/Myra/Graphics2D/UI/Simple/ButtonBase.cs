using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Utility;
using Myra.Events;

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
	/// An abstract base class for button-like widgets that can be pressed, toggled, and respond to click events.
	/// </summary>
	/// <typeparam name="T">The type of widget to use as the button's content.</typeparam>
	[Obsolete("Use ButtonBase2<T> instead.")]
	public class ButtonBase<T> : Widget where T : Widget
	{
		private readonly SingleItemLayout<T> _layout;

		private bool _isPressed = false;
		private bool _isClicked = false;

		/// <summary>
		/// Gets or sets the horizontal alignment of the button's content.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(HorizontalAlignment.Center)]
		public virtual HorizontalAlignment ContentHorizontalAlignment
		{
			get { return InternalChild.HorizontalAlignment; }
			set { InternalChild.HorizontalAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the button's content.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(VerticalAlignment.Center)]
		public virtual VerticalAlignment ContentVerticalAlignment
		{
			get { return InternalChild.VerticalAlignment; }
			set { InternalChild.VerticalAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the brush used to draw the background when the button is pressed.
		/// </summary>
		[Category("Appearance")]
		public virtual IBrush PressedBackground { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the button remains pressed after being clicked (toggle behavior).
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool Toggleable { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the button is currently in the pressed state.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public virtual bool IsPressed
		{
			get
			{
				return _isPressed;
			}

			set
			{
				if (value == _isPressed)
				{
					return;
				}

				_isPressed = value;
				OnPressedChanged();
			}
		}

		internal bool ReleaseOnTouchLeft;

		/// <summary>
		/// Gets or sets the desktop that manages this button.
		/// </summary>
		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}

			internal set
			{
				// If we're not releasing the button on touch left,
				// we have to do it on touch up
				if (!ReleaseOnTouchLeft && Desktop != null)
				{
					Desktop.TouchUp -= DesktopTouchUp;
				}

				base.Desktop = value;

				if (!ReleaseOnTouchLeft && Desktop != null)
				{
					Desktop.TouchUp += DesktopTouchUp;
				}
			}
		}

		/// <summary>
		/// Occurs when the button is clicked.
		/// </summary>
		public event MyraEventHandler Click;

		/// <summary>
		/// Occurs when the pressed state changes.
		/// </summary>
		public event MyraEventHandler PressedChanged;

		/// <summary>
		/// Occurs when the pressed state is about to change due to user interaction. Set Cancel to true to prevent the change.
		/// </summary>
		public event MyraEventHandler<ValueChangingEventArgs<bool>> PressedChangingByUser;

		/// <summary>
		/// Gets or sets the internal child widget of the button.
		/// </summary>
		protected T InternalChild
		{
			get => _layout.Child;
			set => _layout.Child = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonBase{T}"/> class.
		/// </summary>
		public ButtonBase()
		{
			_layout = new SingleItemLayout<T>(this);
			ChildrenLayout = _layout;
			Toggleable = false;
			ReleaseOnTouchLeft = true;
		}

		/// <summary>
		/// Simulates a click on the button by firing touch down and touch up events.
		/// </summary>
		public void DoClick()
		{
			OnTouchDown();
			OnTouchUp();
		}

		/// <summary>
		/// Raises the pressed changed event.
		/// </summary>
		public virtual void OnPressedChanged()
		{
			PressedChanged.Invoke(this, InputEventType.PressedChanged);
		}

		private void SetValueByUser(bool value)
		{
			if (value != IsPressed && PressedChangingByUser != null)
			{
				var args = new ValueChangingEventArgs<bool>(_isPressed, value);
				PressedChangingByUser(this, args);

				if (args.Cancel)
				{
					return;
				}
			}

			IsPressed = value;
		}

		/// <summary>
		/// Handles the event when the mouse leaves the button.
		/// </summary>
		public override void OnTouchLeft()
		{
			base.OnTouchLeft();

			if (ReleaseOnTouchLeft && !Toggleable)
			{
				SetValueByUser(false);
			}
		}

		/// <summary>
		/// Handles touch up events on the button.
		/// </summary>
		public override void OnTouchUp()
		{
			base.OnTouchUp();

			if (!Enabled)
			{
				return;
			}

			if (ReleaseOnTouchLeft && !Toggleable)
			{
				SetValueByUser(false);
			}

			if (_isClicked)
			{
				Click.Invoke(this, InputEventType.TouchUp);
				_isClicked = false;
			}
		}

		/// <summary>
		/// Handles touch down events on the button.
		/// </summary>
		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (!Enabled)
			{
				return;
			}

			if (!Toggleable)
			{
				SetValueByUser(true);
			}
			else
			{
				SetValueByUser(!IsPressed);
			}

			_isClicked = true;
		}

		/// <summary>
		/// Handles keyboard input, simulating a click when Space is pressed.
		/// </summary>
		/// <param name="k">The key being pressed.</param>
		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (!Enabled)
			{
				return;
			}

			if (k == Keys.Space)
			{
				if (!Toggleable)
				{
					// Emulate click
					DoClick();
				}
				else
				{
					SetValueByUser(!IsPressed);
				}
			}
		}

		/// <summary>
		/// Gets the current background brush based on button state (pressed, hovered, disabled, or normal).
		/// </summary>
		/// <returns>The background brush to use for rendering.</returns>
		public override IBrush GetCurrentBackground()
		{
			var result = base.GetCurrentBackground();

			if (Enabled)
			{
				if (IsPressed && PressedBackground != null)
				{
					result = PressedBackground;
				}
				else if (IsMouseInside && OverBackground != null)
				{
					result = OverBackground;
				}
			}
			else
			{
				if (DisabledBackground != null)
				{
					result = DisabledBackground;
				}
			}

			return result;
		}

		/// <summary>
		/// Applies the specified style to the button.
		/// </summary>
		/// <param name="style">The style to apply.</param>
		public void ApplyButtonStyle(ButtonStyle style)
		{
			ApplyWidgetStyle(style);

			PressedBackground = style.PressedBackground;
		}

		private void DesktopTouchUp(object sender, MyraEventArgs args)
		{
			IsPressed = false;
		}

		/// <summary>
		/// Applies a named button style from the stylesheet to the button.
		/// </summary>
		/// <param name="stylesheet">The stylesheet containing the style.</param>
		/// <param name="name">The name of the button style to apply.</param>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name));
		}
	}
}