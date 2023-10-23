/* Generated by MyraPad at 10/23/2023 7:10:17 AM */
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI.Properties;
using FontStashSharp.RichText;
using AssetManagementBase;

#if STRIDE
using Stride.Core.Mathematics;
#elif PLATFORM_AGNOSTIC
using System.Drawing;
using System.Numerics;
using Color = FontStashSharp.FSColor;
#else
// MonoGame/FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace Myra.Graphics2D.UI
{
	partial class DebugOptionsWindow: Window
	{
		private void BuildUI()
		{
			_checkBoxWidgetFrames = new CheckBox();
			_checkBoxWidgetFrames.Text = "Draw green frame around every widget";
			_checkBoxWidgetFrames.Id = "_checkBoxWidgetFrames";

			_checkBoxKeyboardFocusedWidgetFrame = new CheckBox();
			_checkBoxKeyboardFocusedWidgetFrame.Text = "Draw red frame around the keyboard focused widget";
			_checkBoxKeyboardFocusedWidgetFrame.Id = "_checkBoxKeyboardFocusedWidgetFrame";

			_checkBoxMouseInsideWidgetFrame = new CheckBox();
			_checkBoxMouseInsideWidgetFrame.Text = "Draw yellow frame around the mouse hovered widget";
			_checkBoxMouseInsideWidgetFrame.Id = "_checkBoxMouseInsideWidgetFrame";

			_checkBoxGlyphFrames = new CheckBox();
			_checkBoxGlyphFrames.Text = "Draw white frame around every TextBox letter glyph";
			_checkBoxGlyphFrames.Id = "_checkBoxGlyphFrames";

			_checkBoxDisableClipping = new CheckBox();
			_checkBoxDisableClipping.Text = "Disable clipping";
			_checkBoxDisableClipping.Id = "_checkBoxDisableClipping";

			_checkBoxSmoothText = new CheckBox();
			_checkBoxSmoothText.Text = "Smooth text";
			_checkBoxSmoothText.Id = "_checkBoxSmoothText";

			Root = new VerticalStackPanel();
			Root.Id = "Root";
			Root.Widgets.Add(_checkBoxWidgetFrames);
			Root.Widgets.Add(_checkBoxKeyboardFocusedWidgetFrame);
			Root.Widgets.Add(_checkBoxMouseInsideWidgetFrame);
			Root.Widgets.Add(_checkBoxGlyphFrames);
			Root.Widgets.Add(_checkBoxDisableClipping);
			Root.Widgets.Add(_checkBoxSmoothText);

			
			Title = "Debug Options";
			Left = 1057;
			Top = 584;
			Content = Root;
		}

		
		public CheckBox _checkBoxWidgetFrames;
		public CheckBox _checkBoxKeyboardFocusedWidgetFrame;
		public CheckBox _checkBoxMouseInsideWidgetFrame;
		public CheckBox _checkBoxGlyphFrames;
		public CheckBox _checkBoxDisableClipping;
		public CheckBox _checkBoxSmoothText;
		public VerticalStackPanel Root;
	}
}
