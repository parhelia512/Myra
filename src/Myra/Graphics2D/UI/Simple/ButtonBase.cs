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
	/// A generic base class for button controls that contain a single widget.
	/// </summary>
	/// <typeparam name="T">The type of widget contained within the button.</typeparam>
	public class ButtonBase<T> : Widget where T : Widget
	{
		private readonly SingleItemLayout<T> _layout;

		private bool _isPressed = false;
		private bool _isClicked = false;

		/// <summary>
		/// Gets or sets the horizontal alignment of the button's content widget.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(HorizontalAlignment.Center)]
		public virtual HorizontalAlignment ContentHorizontalAlignment
		{
			get { return InternalChild.HorizontalAlignment; }
			set { InternalChild.HorizontalAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the button's content widget.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(VerticalAlignment.Center)]
		public virtual VerticalAlignment ContentVerticalAlignment
		{
			get { return InternalChild.VerticalAlignment; }
			set { InternalChild.VerticalAlignment = value; }
		}

		/// <summary>
		/// Gets or sets the brush used to draw the button's background when pressed.
		/// </summary>
		[Category("Appearance")]
		public virtual IBrush PressedBackground { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this button toggles between pressed and unpressed states.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool Toggleable { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this button is currently pressed.
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

		/// <summary>
		/// When false, the button releases on touch up instead of touch left.
		/// </summary>
		internal bool ReleaseOnTouchLeft;

		/// <summary>
		/// Gets or sets the desktop this button is attached to.
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
		/// Raised when the button is clicked.
		/// </summary>
		public event EventHandler Click;

		/// <summary>
		/// Raised when the pressed state changes.
		/// </summary>
		public event EventHandler PressedChanged;

		/// <summary>
		/// Raised when the pressed state is about to change due to user interaction. Set Cancel to true to prevent the change.
		/// </summary>
		public event EventHandler<ValueChangingEventArgs<bool>> PressedChangingByUser;

		/// <summary>
		/// Gets or sets the internal child widget.
		/// </summary>
		protected T InternalChild
		{
			get => _layout.Child;
			set => _layout.Child = value;
		}

		/// <summary>
		/// Initializes a new instance of the ButtonBase class.
		/// </summary>
		public ButtonBase()
		{
			_layout = new SingleItemLayout<T>(this);
			ChildrenLayout = _layout;
			Toggleable = false;
			ReleaseOnTouchLeft = true;
		}

		/// <summary>
		/// Simulates a click by triggering touch down and touch up events.
		/// </summary>
		public void DoClick()
		{
			OnTouchDown();
			OnTouchUp();
		}

		/// <summary>
		/// Raises the PressedChanged event.
		/// </summary>
		public virtual void OnPressedChanged()
		{
			PressedChanged.Invoke(this);
		}

		/// <summary>
		/// Sets the pressed state in response to user input, firing the PressedChangingByUser event.
		/// </summary>
		/// <param name="value">The new pressed state.</param>
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
		/// Handles the touch left event.
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
		/// Handles the touch up event.
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
				Click.Invoke(this);
				_isClicked = false;
			}
		}

		/// <summary>
		/// Handles the touch down event.
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
		/// Handles keyboard input, triggering clicks on Space key.
		/// </summary>
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
		/// Gets the current background brush based on the button state.
		/// </summary>
		/// <returns>The brush to use for the background.</returns>
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
		/// Applies a button style to this button.
		/// </summary>
		/// <param name="style">The button style to apply.</param>
		public void ApplyButtonStyle(ButtonStyle style)
		{
			ApplyWidgetStyle(style);

			PressedBackground = style.PressedBackground;
		}

		/// <summary>
		/// Handles desktop touch up to release the button if not using ReleaseOnTouchLeft.
		/// </summary>
		private void DesktopTouchUp(object sender, EventArgs args)
		{
			IsPressed = false;
		}

		/// <summary>
		/// Sets the button's style using a stylesheet.
		/// </summary>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name));
		}
	}
}