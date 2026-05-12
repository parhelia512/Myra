namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// An internal toggle button used in list views to represent a selectable item.
	/// </summary>
	internal class ListViewButton : ToggleButton
	{
		/// <summary>
		/// Gets or sets the container holding multiple buttons.
		/// </summary>
		public Widget ButtonsContainer { get; set; }

		/// <summary>
		/// Gets the top parent container.
		/// </summary>
		public Widget TopParent => ButtonsContainer ?? Parent;

		/// <summary>
		/// Initializes a new instance of the ListViewButton class.
		/// </summary>
		public ListViewButton() : base(null)
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether this list view button is pressed (selected).
		/// </summary>
		public override bool IsPressed
		{
			get => base.IsPressed;

			set
			{
				if (IsPressed && Parent != null)
				{
					// If this is last pressed button
					// Don't allow it to be unpressed
					var allow = false;
					foreach (var child in TopParent.ChildrenCopy)
					{
						var asListViewButton = child as ListViewButton;
						if (asListViewButton == this)
						{
							continue;
						}

						if (asListViewButton == null)
						{
							asListViewButton = child.FindChild<ListViewButton>();
							if (asListViewButton == null || asListViewButton == this)
							{
								continue;
							}
						}

						if (asListViewButton.IsPressed)
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
		/// Raises the PressedChanged event and deselects other items.
		/// </summary>
		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			if (Parent == null || !IsPressed)
			{
				return;
			}

			// Release other pressed radio buttons
			foreach (var child in TopParent.ChildrenCopy)
			{
				var asListViewButton = child as ListViewButton;
				if (asListViewButton == this)
				{
					continue;
				}

				if (asListViewButton == null)
				{
					asListViewButton = child.FindChild<ListViewButton>();
					if (asListViewButton == null || asListViewButton == this)
					{
						continue;
					}
				}

				asListViewButton.IsPressed = false;
			}
		}
	}
}
