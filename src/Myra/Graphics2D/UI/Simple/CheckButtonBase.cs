using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System.Xml.Serialization;


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
	/// Specifies the position of the check mark relative to the content.
	/// </summary>
	public enum CheckPosition
	{
		/// <summary>The check mark is displayed on the left.</summary>
		Left,
		/// <summary>The check mark is displayed on the right.</summary>
		Right
	}

	/// <summary>
	/// An abstract base class for checkbox and radio button controls.
	/// </summary>
	public class CheckButtonBase : ButtonBase2
	{
		/// <summary>
		/// An internal image class that inherits the mouse-over state from its parent.
		/// </summary>
		private class CheckImageInternal : Image
		{
			/// <summary>
			/// Gets whether the over background should be used based on parent's mouse state.
			/// </summary>
			protected override bool UseOverBackground
			{
				get
				{
					if (Parent == null)
					{
						return IsMouseInside;
					}

					return Parent.IsMouseInside;
				}
			}
		}


		private readonly StackPanelLayout _layout = new StackPanelLayout(Orientation.Horizontal);
		private CheckPosition _checkPosition = CheckPosition.Left;
		private readonly CheckImageInternal _check = new CheckImageInternal
		{
			HorizontalAlignment = HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Center,
		};
		private Widget _content;
		private IImage _checkedImage, _uncheckedImage;

		/// <summary>
		/// Gets or sets the position of the check mark relative to the content.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(CheckPosition.Left)]
		public CheckPosition CheckPosition
		{
			get => _checkPosition;
			set
			{
				if (_checkPosition == value) return;

				_checkPosition = value;
				UpdateChildren();
			}
		}

		/// <summary>
		/// Gets or sets the spacing between the check mark and content.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(0)]
		public int CheckContentSpacing
		{
			get => _layout.Spacing;
			set => _layout.Spacing = value;
		}

		/// <summary>
		/// Gets or sets the image displayed when the check button is unchecked.
		/// </summary>
		[Category("Appearance")]
		public IImage UncheckedImage
		{
			get => _uncheckedImage;
			set
			{
				if (value == _uncheckedImage)
				{
					return;
				}

				_uncheckedImage = value;
				UpdateImage();
			}
		}

		/// <summary>
		/// Gets or sets the image displayed when the check button is checked.
		/// </summary>
		[Category("Appearance")]
		public IImage CheckedImage
		{
			get => _checkedImage;
			set
			{
				if (value == _checkedImage)
				{
					return;
				}

				_checkedImage = value;
				UpdateImage();
			}
		}

		/// <summary>
		/// Gets or sets the content widget displayed next to the check mark.
		/// </summary>
		[Browsable(false)]
		[Content]
		public override Widget Content
		{
			get => _content;
			set
			{
				if (_content == value) return;

				_content = value;

				UpdateChildren();
			}
		}

		/// <summary>
		/// Gets the internal check mark image widget.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Image CheckImage => _check;

		/// <summary>
		/// Initializes a new instance of the CheckButtonBase class.
		/// </summary>
		protected CheckButtonBase()
		{
			ChildrenLayout = _layout;

			UpdateChildren();
		}

		/// <summary>
		/// Handles the touch up event (no action required).
		/// </summary>
		protected override void InternalOnTouchUp()
		{
		}

		/// <summary>
		/// Handles the touch down event by toggling the checked state.
		/// </summary>
		protected override void InternalOnTouchDown()
		{
			SetValueByUser(!IsPressed);
		}

		/// <summary>
		/// Raises the PressedChanged event and updates the check mark image.
		/// </summary>
		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			_check.IsPressed = IsPressed;
		}

		/// <summary>
		/// Handles keyboard input, toggling the checked state on Space key.
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
				SetValueByUser(!IsPressed);
			}
		}

		/// <summary>
		/// Applies a check button style to this control.
		/// </summary>
		/// <param name="style">The image text button style to apply.</param>
		public void ApplyCheckButtonStyle(ImageTextButtonStyle style)
		{
			ApplyButtonStyle(style);

			if (style.ImageStyle != null)
			{
				_check.ApplyPressableImageStyle(style.ImageStyle);

				UncheckedImage = style.ImageStyle.Image;
				CheckedImage = style.ImageStyle.PressedImage;
			}

			CheckContentSpacing = style.ImageTextSpacing;
		}

		/// <summary>
		/// Updates the children layout based on the check position.
		/// </summary>
		private void UpdateChildren()
		{
			Children.Clear();

			switch (_checkPosition)
			{
				case CheckPosition.Left:
					Children.Add(_check);
					if (_content != null)
					{
						Children.Add(_content);
					}

					break;

				case CheckPosition.Right:
					if (_content != null)
					{
						Children.Add(_content);
					}
					Children.Add(_check);

					break;
			}
		}

		/// <summary>
		/// Copies the style properties from another check button.
		/// </summary>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var checkButtonBase = (CheckButtonBase)w;

			CheckPosition = checkButtonBase.CheckPosition;
			CheckContentSpacing = checkButtonBase.CheckContentSpacing;
			CheckImage.CopyFrom(checkButtonBase.CheckImage);
		}

		/// <summary>
		/// Updates the check mark image based on the pressed state.
		/// </summary>
		private void UpdateImage()
		{
			_check.Renderable = IsPressed ? _checkedImage : _uncheckedImage;
		}
	}
}
