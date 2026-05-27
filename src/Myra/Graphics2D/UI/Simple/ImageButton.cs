using System;
using System.ComponentModel;
using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A button widget that displays an image.
	/// </summary>
	/// <remarks>This class is obsolete. Use <see cref="Button"/> instead.</remarks>
	[Obsolete("Switch to Button")]
	[StyleTypeName("Button")]
	public class ImageButton : ButtonBase<Image>
	{
		/// <summary>
		/// Gets or sets the image displayed on the button.
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
		/// Gets or sets the image displayed on the button when the cursor is over it.
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
		/// Gets or sets the image displayed on the button when it is pressed.
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
		/// Gets or sets the width of the image in pixels, or null to use automatic sizing.
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
		/// Gets or sets the height of the image in pixels, or null to use automatic sizing.
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
		/// Initializes a new instance of the <see cref="ImageButton"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public ImageButton(string styleName = Stylesheet.DefaultStyleName)
		{
			InternalChild = new Image
			{
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};

			SetStyle(styleName);
		}

		/// <summary>
		/// Applies the specified image button style to the button and its image.
		/// </summary>
		/// <param name="style">The image button style to apply.</param>
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

		/// <summary>
		/// Handles the pressed state change and updates the image's pressed state.
		/// </summary>
		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			InternalChild.IsPressed = IsPressed;
		}
	}
}