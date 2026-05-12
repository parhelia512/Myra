 using Myra.Graphics2D.UI.Styles;
using System;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// An internal button used in list-based selectors to represent a selectable item.
	/// </summary>
	[Obsolete]
	internal class ListButton: ImageTextButton
	{
		private readonly ISelector _selector;

		/// <summary>
		/// Gets or sets a value indicating whether this list button is pressed (selected).
		/// </summary>
		public override bool IsPressed
		{
			get => base.IsPressed;

			set
			{
				if (IsPressed && _selector.SelectionMode == SelectionMode.Single && Parent != null)
				{
					// If this is last selected item
					// Don't allow it to be unselected
					var allow = false;
					foreach (var child in Parent.ChildrenCopy)
					{
						var asListButton = child as ListButton;
						if (asListButton == null || asListButton == this)
						{
							continue;
						}

						if (asListButton.IsPressed)
						{
							allow = true;
							break;
						}
					}

					if (!allow)
					{
						return;
					}
				}

				base.IsPressed = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the ListButton class.
		/// </summary>
		/// <param name="bs">The style to apply to this button.</param>
		/// <param name="selector">The selector that contains this button.</param>
		public ListButton(ImageTextButtonStyle bs, ISelector selector) : base(null)
		{
			_selector = selector;
			Toggleable = true;

			ApplyImageTextButtonStyle(bs);
		}

		/// <summary>
		/// Raises the PressedChanged event and deselects other items if in single selection mode.
		/// </summary>
		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			if (!IsPressed)
			{
				return;
			}

			// Release other pressed radio buttons
			foreach (var child in Parent.ChildrenCopy)
			{
				var asListButton = child as ListButton;
				if (asListButton == null || asListButton == this)
				{
					continue;
				}

				asListButton.IsPressed = false;
			}
		}
	}
}
