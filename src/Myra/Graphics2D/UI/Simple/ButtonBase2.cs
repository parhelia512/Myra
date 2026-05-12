using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.ComponentModel;
using System.Xml.Serialization;
using System;
using Myra.Events;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// An abstract base class for button controls that contain arbitrary content.
	/// </summary>
	public abstract class ButtonBase2 : ContentControl
	{
		private bool _isPressed = false;
		private bool _isClicked = false;

		/// <summary>
		/// Gets or sets the brush used to draw the button's background when pressed.
		/// </summary>
		[Category("Appearance")]
		public virtual IBrush PressedBackground { get; set; }

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
		/// Simulates a click by triggering touch down and touch up events.
		/// </summary>
		public void DoClick()
		{
			OnTouchDown();
			OnTouchUp();
		}

		/// <summary>
		/// Raises the PressedChanged event and updates the content if it implements IPressable.
		/// </summary>
		public virtual void OnPressedChanged()
		{
			PressedChanged.Invoke(this);

			var asPressable = Content as IPressable;
			if (asPressable != null)
			{
				asPressable.IsPressed = IsPressed;
			}
		}

		/// <summary>
		/// Sets the pressed state in response to user input, firing the PressedChangingByUser event.
		/// </summary>
		/// <param name="value">The new pressed state.</param>
		protected void SetValueByUser(bool value)
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
		/// When overridden in a derived class, handles the touch up event.
		/// </summary>
		protected abstract void InternalOnTouchUp();

		/// <summary>
		/// When overridden in a derived class, handles the touch down event.
		/// </summary>
		protected abstract void InternalOnTouchDown();

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

			InternalOnTouchUp();

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

			InternalOnTouchDown();

			_isClicked = true;
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
				else if (UseOverBackground && OverBackground != null)
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
		/// Applies an image button style to this button.
		/// </summary>
		/// <param name="style">The image button style to apply.</param>
		public void ApplyImageButtonStyle(ImageButtonStyle style)
		{
			ApplyButtonStyle(style);

			if (style.ImageStyle != null)
			{
				var image = (Image)Content;
				image.ApplyPressableImageStyle(style.ImageStyle);
			}
		}

		/// <summary>
		/// Sets the button's style using a stylesheet.
		/// </summary>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name));
		}

		/// <summary>
		/// Copies the style properties from another button.
		/// </summary>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var buttonBase = (ButtonBase2)w;
			PressedBackground = buttonBase.PressedBackground;
			IsPressed = buttonBase.IsPressed;
		}
	}
}
