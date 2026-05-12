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
	/// A button control that displays both an image and text with flexible positioning options.
	/// </summary>
	/// <remarks>This class is obsolete. Use Button with a HorizontalStackPanel or custom layout instead.</remarks>
	[Obsolete("Use Button")]
	[StyleTypeName("Button")]
	public class ImageTextButton : ButtonBase<Grid>
	{
		/// <summary>
		/// Specifies the position of text relative to the image in an ImageTextButton.
		/// </summary>
		public enum TextPositionEnum
		{
			/// <summary>Text is positioned to the right of the image.</summary>
			Right,
			/// <summary>Text is positioned to the left of the image.</summary>
			Left,
			/// <summary>Text is positioned above the image.</summary>
			Top,
			/// <summary>Text is positioned below the image.</summary>
			Bottom,
			/// <summary>Text is positioned overlapping the image.</summary>
			OverlapsImage,
			/// <summary>Text is positioned behind the image.</summary>
			BehindImage
		}

		private readonly Image _image;
		private readonly Label _label;
		private TextPositionEnum _textPosition;

		/// <summary>
		/// Gets or sets the text content of this button.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(null)]
		public string Text
		{
			get
			{
				return _label.Text;
			}
			set
			{
				_label.Text = value;
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
				return _label.TextColor;
			}
			set
			{
				_label.TextColor = value;
			}
		}

		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/OverTextColor")]
		public Color? OverTextColor
		{
			get
			{
				return _label.OverTextColor;
			}
			set
			{
				_label.OverTextColor = value;
			}
		}

		[Category("Appearance")]
		[StylePropertyPath("/LabelStyle/PressedTextColor")]
		public Color? PressedTextColor
		{
			get
			{
				return _label.PressedTextColor;
			}
			set
			{
				_label.PressedTextColor = value;
			}
		}

		[Category("Appearance")]
		public SpriteFontBase Font
		{
			get
			{
				return _label.Font;
			}
			set
			{
				_label.Font = value;
			}
		}

		/// <summary>
		/// Gets or sets the image displayed on this button.
		/// </summary>
		[Category("Appearance")]
		public IImage Image
		{
			get
			{
				return _image.Renderable;
			}

			set
			{
				_image.Renderable = value;
			}
		}

		/// <summary>
		/// Gets or sets the image displayed when the mouse is over this button.
		/// </summary>
		[Category("Appearance")]
		public IImage OverImage
		{
			get
			{
				return _image.OverRenderable;
			}

			set
			{
				_image.OverRenderable = value;
			}
		}

		/// <summary>
		/// Gets or sets the image displayed when this button is pressed.
		/// </summary>
		[Category("Appearance")]
		public IImage PressedImage
		{
			get
			{
				return _image.PressedRenderable;
			}

			set
			{
				_image.PressedRenderable = value;
			}
		}

		/// <summary>
		/// Gets or sets the width of the displayed image.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(null)]
		public int? ImageWidth
		{
			get
			{
				return _image.Width;
			}
			set
			{
				_image.Width = value;
			}
		}

		/// <summary>
		/// Gets or sets the height of the displayed image.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(null)]
		public int? ImageHeight
		{
			get
			{
				return _image.Height;
			}
			set
			{
				_image.Height = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the image is visible.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(true)]
		public bool ImageVisible
		{
			get
			{
				return _image.Visible;
			}

			set
			{
				_image.Visible = value;
			}
		}

		/// <summary>
		/// Gets or sets the spacing between the image and text.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(0)]
		public int ImageTextSpacing
		{
			get
			{
				return InternalChild.ColumnSpacing;
			}

			set
			{
				InternalChild.ColumnSpacing = value;
				InternalChild.RowSpacing = value;
			}
		}

		/// <summary>
		/// Gets or sets the position of the text relative to the image.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(TextPositionEnum.Right)]
		public TextPositionEnum TextPosition
		{
			get
			{
				return _textPosition;
			}

			set
			{
				if (_textPosition == value)
				{
					return;
				}

				SetTextPosition(value);
			}
		}

		[Category("Appearance")]
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment LabelHorizontalAlignment
		{
			get => _label.HorizontalAlignment;
			set => _label.HorizontalAlignment = value;
		}

		[Category("Appearance")]
		[DefaultValue(VerticalAlignment.Center)]
		public VerticalAlignment LabelVerticalAlignment
		{
			get => _label.VerticalAlignment;
			set => _label.VerticalAlignment = value;
		}

		[Category("Appearance")]
		[DefaultValue(HorizontalAlignment.Center)]
		public HorizontalAlignment ImageHorizontalAlignment
		{
			get => _image.HorizontalAlignment;
			set => _image.HorizontalAlignment = value;
		}

		[Category("Appearance")]
		[DefaultValue(VerticalAlignment.Center)]
		public VerticalAlignment ImageVerticalAlignment
		{
			get => _image.VerticalAlignment;
			set => _image.VerticalAlignment = value;
		}

		[DefaultValue(HorizontalAlignment.Stretch)]
		public override HorizontalAlignment ContentHorizontalAlignment
		{
			get
			{
				return base.ContentHorizontalAlignment;
			}
			set
			{
				base.ContentHorizontalAlignment = value;
			}
		}

		[DefaultValue(VerticalAlignment.Stretch)]
		public override VerticalAlignment ContentVerticalAlignment
		{
			get
			{
				return base.ContentVerticalAlignment;
			}
			set
			{
				base.ContentVerticalAlignment = value;
			}
		}

		public ImageTextButton(string styleName = Stylesheet.DefaultStyleName)
		{
			InternalChild = new Grid
			{
				DefaultColumnProportion = Proportion.Auto,
				DefaultRowProportion = Proportion.Auto
			};

			_image = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			_label = new Label(null)
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
				Wrap = true
			};

			SetStyle(styleName);
			SetTextPosition(TextPositionEnum.Right);
		}

		private void SetTextPosition(TextPositionEnum value)
		{
			InternalChild.Widgets.Clear();
			InternalChild.ColumnsProportions.Clear();

			switch (value)
			{
				case TextPositionEnum.Right:
					Grid.SetColumn(_image, 0);
					Grid.SetRow(_image, 0);
					Grid.SetColumn(_label, 1);
					Grid.SetRow(_label, 0);
					InternalChild.Widgets.Add(_image);
					InternalChild.Widgets.Add(_label);
					break;
				case TextPositionEnum.Left:
					Grid.SetColumn(_image, 1);
					Grid.SetRow(_image, 0);
					Grid.SetColumn(_label, 0);
					Grid.SetRow(_label, 0);

					InternalChild.Widgets.Add(_image);
					InternalChild.Widgets.Add(_label);
					break;
				case TextPositionEnum.OverlapsImage:
					Grid.SetColumn(_image, 0);
					Grid.SetRow(_image, 0);
					Grid.SetColumn(_label, 0);
					Grid.SetRow(_label, 0);

					InternalChild.Widgets.Add(_image);
					InternalChild.Widgets.Add(_label);
					break;
				case TextPositionEnum.BehindImage:
					Grid.SetColumn(_image, 0);
					Grid.SetRow(_image, 0);
					Grid.SetColumn(_label, 0);
					Grid.SetRow(_label, 0);

					InternalChild.Widgets.Add(_label);
					InternalChild.Widgets.Add(_image);
					break;
				case TextPositionEnum.Top:
					Grid.SetColumn(_image, 0);
					Grid.SetRow(_image, 1);
					Grid.SetColumn(_label, 0);
					Grid.SetRow(_label, 0);

					InternalChild.Widgets.Add(_image);
					InternalChild.Widgets.Add(_label);
					break;
				case TextPositionEnum.Bottom:
					Grid.SetColumn(_image, 0);
					Grid.SetRow(_image, 0);
					Grid.SetColumn(_label, 0);
					Grid.SetRow(_label, 1);

					InternalChild.Widgets.Add(_image);
					InternalChild.Widgets.Add(_label);
					break;
			}

			_textPosition = value;
		}

		public void ApplyImageTextButtonStyle(ImageTextButtonStyle style)
		{
			ApplyButtonStyle(style);

			if (style.ImageStyle != null)
			{
				_image.ApplyPressableImageStyle(style.ImageStyle);
			}

			if (style.LabelStyle != null)
			{
				_label.ApplyLabelStyle(style.LabelStyle);
			}

			ImageTextSpacing = style.ImageTextSpacing;
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			_image.IsPressed = IsPressed;
			_label.IsPressed = IsPressed;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyImageTextButtonStyle(new ImageTextButtonStyle(stylesheet.ButtonStyles.SafelyGetStyle(name)));
		}
	}
}