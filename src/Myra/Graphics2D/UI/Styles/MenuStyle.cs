#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI.Styles
{
	/// <summary>
	/// Style class for menu controls.
	/// </summary>
	public class MenuStyle : WidgetStyle
	{
		/// <summary>
		/// Gets or sets the style for menu item images.
		/// </summary>
		public PressableImageStyle ImageStyle { get; set; }

		/// <summary>
		/// Gets or sets the style for menu item text labels.
		/// </summary>
		public LabelStyle LabelStyle { get; set; }

		/// <summary>
		/// Gets or sets the style for menu item keyboard shortcut text.
		/// </summary>
		public LabelStyle ShortcutStyle { get; set; }

		/// <summary>
		/// Gets or sets the style for menu item separators.
		/// </summary>
		public SeparatorStyle SeparatorStyle { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background when the mouse is over a menu item.
		/// </summary>
		public IBrush SelectionHoverBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used for the background of a selected menu item.
		/// </summary>
		public IBrush SelectionBackground { get; set; }

		/// <summary>
		/// Gets or sets the color for the special character indicator in keyboard shortcuts.
		/// </summary>
		public Color? SpecialCharColor { get; set; }

		/// <summary>
		/// Initializes a new instance of the MenuStyle class.
		/// </summary>
		public MenuStyle()
		{
		}

		/// <summary>
		/// Initializes a new instance of the MenuStyle class by copying properties from another style.
		/// </summary>
		/// <param name="style">The menu style to copy from.</param>
		public MenuStyle(MenuStyle style) : base(style)
		{
			ImageStyle = style.ImageStyle != null ? new PressableImageStyle(style.ImageStyle) : null;
			LabelStyle = style.LabelStyle != null ? new LabelStyle(style.LabelStyle) : null;
			ShortcutStyle = style.ShortcutStyle != null ? new LabelStyle(style.ShortcutStyle) : null;
			SeparatorStyle = style.SeparatorStyle != null ? new SeparatorStyle(style.SeparatorStyle) : null;
			SpecialCharColor = style.SpecialCharColor;
		}

		/// <summary>
		/// Creates a copy of this menu style.
		/// </summary>
		/// <returns>A new menu style with the same properties.</returns>
		public override WidgetStyle Clone()
		{
			return new MenuStyle(this);
		}
	}
}
