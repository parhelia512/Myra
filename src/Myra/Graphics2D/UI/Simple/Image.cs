using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Specifies how an image is resized to fit its bounds.
	/// </summary>
	public enum ImageResizeMode
	{
		/// <summary>Stretch the image to fill the bounds, potentially distorting the aspect ratio.</summary>
		Stretch,

		/// <summary>Scale the image to fit the bounds while preserving the aspect ratio.</summary>
		KeepAspectRatio
	}

	internal interface IPressable
	{
		bool IsPressed { get; set; }
	}

	/// <summary>
	/// An image display widget that renders an image with optional hover and pressed states.
	/// </summary>
	public class Image : Widget, IPressable
	{
		private IImage _image, _overImage, _pressedImage;

#if MONOGAME
		private bool _isAnisotropicFiltering = false;

		/// <summary>
		/// Gets or sets a value indicating whether anisotropic filtering is applied to this image.
		/// </summary>
		[DefaultValue(false)]
		public bool IsAnisotropicFiltering
		{
			get
			{
				return _isAnisotropicFiltering;
			}
			set
			{
				_isAnisotropicFiltering = value;
				InvalidateMeasure();
			}
		}
#endif

		/// <summary>
		/// Gets or sets the image to render.
		/// </summary>
		[Category("Appearance")]
		public IImage Renderable
		{
			get
			{
				return _image;
			}

			set
			{
				if (value == _image)
				{
					return;
				}

				_image = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the image displayed when the mouse is over this image.
		/// </summary>
		[Category("Appearance")]
		public IImage OverRenderable
		{
			get
			{
				return _overImage;
			}

			set
			{
				if (value == _overImage)
				{
					return;
				}

				_overImage = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the image displayed when this image is pressed.
		/// </summary>
		[Category("Appearance")]
		public IImage PressedRenderable
		{
			get
			{
				return _pressedImage;
			}

			set
			{
				if (value == _pressedImage)
				{
					return;
				}

				_pressedImage = value;
				InvalidateMeasure();
			}
		}

		internal bool IsPressed { get; set; }

		bool IPressable.IsPressed
		{
			get => IsPressed;
			set => IsPressed = value;
		}

		/// <summary>
		/// Gets or sets the color tint applied to the rendered image.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue("#FFFFFFFF")]
		public Color Color { get; set; } = Color.White;

		/// <summary>
		/// Gets or sets how the image is resized to fit its bounds.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(ImageResizeMode.Stretch)]
		public ImageResizeMode ResizeMode { get; set; }

		protected override Point InternalMeasure(Point availableSize)
		{
			var result = _image != null ? _image.Size : Mathematics.PointZero;

			var overSize = _overImage != null ? _overImage.Size : Mathematics.PointZero;
			if (overSize.X > result.X)
			{
				result.X = overSize.X;
			}

			if (overSize.Y > result.Y)
			{
				result.Y = overSize.Y;
			}

			var pressedSize = _pressedImage != null ? _pressedImage.Size : Mathematics.PointZero;
			if (pressedSize.X > result.X)
			{
				result.X = pressedSize.X;
			}

			if (pressedSize.Y > result.Y)
			{
				result.Y = pressedSize.Y;
			}

			return result;
		}

		public override void InternalRender(RenderContext context)
		{
			var image = Renderable;

			if (IsMouseInside && OverRenderable != null)
			{
				image = OverRenderable;
			}

			if (IsPressed && PressedRenderable != null)
			{
				image = PressedRenderable;
			}

			if (image != null)
			{
				var bounds = ActualBounds;

				if (ResizeMode == ImageResizeMode.KeepAspectRatio)
				{
					var aspect = (float)image.Size.X / image.Size.Y;
					bounds.Height = (int)(bounds.Width * aspect);
				}

#if MONOGAME
				context.SetAnisotropicFilteringMode(_isAnisotropicFiltering);
#endif
				image.Draw(context, bounds, Color);
#if MONOGAME
				context.SetAnisotropicFilteringMode(false);
#endif
			}
		}

		/// <summary>
		/// Applies the specified pressable image style to this image.
		/// </summary>
		/// <param name="imageStyle">The style to apply.</param>
		public void ApplyPressableImageStyle(PressableImageStyle imageStyle)
		{
			ApplyWidgetStyle(imageStyle);

			Renderable = imageStyle.Image;
			OverRenderable = imageStyle.OverImage;
			PressedRenderable = imageStyle.PressedImage;
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var image = (Image)w;

			Renderable = image.Renderable;
			OverRenderable = image.OverRenderable;
			PressedRenderable = image.PressedRenderable;
			Color = image.Color;
			ResizeMode = image.ResizeMode;
		}
	}
}