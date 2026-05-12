using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;

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
	/// A vertical menu control that displays menu items in a vertical layout.
	/// </summary>
	public class VerticalMenu : Menu
	{
		/// <summary>
		/// Gets the orientation of this menu (always Vertical).
		/// </summary>
		public override Orientation Orientation
		{
			get
			{
				return Orientation.Vertical;
			}
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of this menu.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Left)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return base.HorizontalAlignment;
			}
			set
			{
				base.HorizontalAlignment = value;
			}
		}

		/// <summary>
		/// Gets or sets the vertical alignment of this menu.
		/// </summary>
		[DefaultValue(VerticalAlignment.Top)]
		public override VerticalAlignment VerticalAlignment
		{
			get
			{
				return base.VerticalAlignment;
			}
			set
			{
				base.VerticalAlignment = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the VerticalMenu class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public VerticalMenu(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;
		}

		/// <summary>
		/// Handles key down events, with Up/Down arrow keys navigating the menu items.
		/// </summary>
		/// <param name="k">The key that was pressed.</param>
		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch (k)
			{
				case Keys.Up:
					MoveHover(-1);
					break;
				case Keys.Down:
					MoveHover(1);
					break;
			}
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyMenuStyle(stylesheet.VerticalMenuStyles.SafelyGetStyle(name));
		}
	}
}