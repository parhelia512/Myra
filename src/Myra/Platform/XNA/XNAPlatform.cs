﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Myra.Platform.XNA
{
	internal class XNAPlatform : IMyraPlatform
	{
		private readonly GraphicsDevice _device;

		public Point ViewSize
		{
			get
			{
#if !STRIDE
				return new Point(_device.Viewport.Width, _device.Viewport.Height);
#else
				return new Point(_device.Presenter.BackBuffer.Width, _device.Presenter.BackBuffer.Height);
#endif
			}
		}

		public XNAPlatform(GraphicsDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException(nameof(device));
			}

			_device = device;
		}

		public object CreateTexture(int width, int height)
		{
#if MONOGAME || FNA
			var texture2d = new Texture2D(_device, width, height);
#elif STRIDE
			var texture2d = Texture2D.New2D(_device, width, height, false, PixelFormat.R8G8B8A8_UNorm, TextureFlags.ShaderResource);
#endif

			return texture2d;
		}

		public void SetTextureData(object texture, Rectangle bounds, byte[] data)
		{
#if MONOGAME || FNA
			var xnaTexture = (Texture2D)texture;
			xnaTexture.SetData(0, bounds, data, 0, bounds.Width * bounds.Height * 4);
#elif STRIDE
			var size = bounds.Width * bounds.Height * 4;
			byte[] temp;
			if (size == data.Length)
			{
				temp = data;
			}
			else
			{
				// Since Stride requres buffer size to match exactly, copy data in the temporary buffer
				temp = new byte[bounds.Width * bounds.Height * 4];
				Array.Copy(data, temp, temp.Length);
			}

			var context = new GraphicsContext(Texture.GraphicsDevice);
			Texture.SetData(context.CommandList, temp, 0, 0, new ResourceRegion(bounds.Left, bounds.Top, 0, bounds.Right, bounds.Bottom, 1));
#endif
		}

		public IMyraRenderer CreateRenderer()
		{
			return new XNARenderer(_device);
		}
	}
}
