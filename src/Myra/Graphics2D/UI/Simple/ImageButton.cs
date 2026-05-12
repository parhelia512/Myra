using System;
using System.ComponentModel;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// An image button control with image display and sizing options.
	/// </summary>
	/// <remarks>This class is obsolete. Use Button with an Image as content instead.</remarks>
	[Obsolete("Switch to Button")]
	[StyleTypeName("Button")]
	public class ImageButton : ButtonBase<Image>
	{
		/// <summary>
		/// Gets or sets the image displayed on this button.
		/// </summary>
		[Category("Appearance")]
		public IImage Image
		{
			get
			{
				return InternalChild.Renderable;
			}

			set
			{
				InternalChild.Renderable = value;
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
				return InternalChild.OverRenderable;
			}

			set
			{
				InternalChild.OverRenderable = value;
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
				return InternalChild.PressedRenderable;
			}

			set
			{
				InternalChild.PressedRenderable = value;
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
				return InternalChild.Width;
			}
			set
			{
				InternalChild.Width = value;
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
				return InternalChild.Height;
			}
			set
			{
				InternalChild.Height = value;
			}
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the image within the button.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(HorizontalAlignment.Center)]
		public HorizontalAlignment ImageHorizontalAlignment
		{
			get => InternalChild.HorizontalAlignment;
			set => InternalChild.HorizontalAlignment = value;
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the image within the button.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(VerticalAlignment.Center)]
		public VerticalAlignment ImageVerticalAlignment
		{
			get => InternalChild.VerticalAlignment;
			set => InternalChild.VerticalAlignment = value;
		}

		/// <summary>
		/// Initializes a new instance of the ImageButton class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public ImageButton(string styleName = Stylesheet.DefaultStyleName)
		{
			InternalChild = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			SetStyle(styleName);
		}

		public void ApplyImageButtonStyle(ImageButtonStyle style)
		{
			ApplyButtonStyle(style);

			var imageStyle = style.ImageStyle;

			if (imageStyle != null)
			{
				InternalChild.ApplyWidgetStyle(imageStyle);

				Image = imageStyle.Image;
				OverImage = imageStyle.OverImage;
				PressedImage = imageStyle.PressedImage;
			}
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			InternalChild.IsPressed = IsPressed;
		}
	}
}