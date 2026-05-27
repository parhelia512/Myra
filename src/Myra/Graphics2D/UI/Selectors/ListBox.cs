using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;
using Myra.Events;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Drawing;
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A list box widget that displays a scrollable list of items that can be selected.
	/// </summary>
	/// <remarks>This class is obsolete. Use <see cref="ListView"/> instead.</remarks>
	[Obsolete("Use ListView")]
	public class ListBox : Selector<ScrollViewer, ListItem>
	{
		private readonly VerticalStackPanel _box;
		internal ComboBox _parentComboBox;

		/// <summary>
		/// Gets or sets the style for the list box.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public ListBoxStyle ListBoxStyle
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the selection mode for the list box.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(SelectionMode.Single)]
		public override SelectionMode SelectionMode
		{
			get
			{
				return base.SelectionMode;
			}

			set
			{
				base.SelectionMode = value;
			}
		}

		/// <summary>
		/// Gets the scroll viewer that contains the list items.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public ScrollViewer ScrollViewer => InternalChild;

		/// <summary>
		/// Initializes a new instance of the <see cref="ListBox"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public ListBox(string styleName = Stylesheet.DefaultStyleName) : base(new ScrollViewer())
		{
			AcceptsKeyboardFocus = true;

			_box = new VerticalStackPanel();
			InternalChild.Content = _box;

			SetStyle(styleName);
		}

		private void ItemOnChanged(object sender, MyraEventArgs eventArgs)
		{
			var item = (ListItem)sender;

			var button = (ImageTextButton)item.Widget;
			button.Text = item.Text;
			button.TextColor = item.Color ?? ListBoxStyle.ListItemStyle.LabelStyle.TextColor;

			InvalidateMeasure();
		}

		/// <summary>
		/// Inserts an item at the specified index in the list box.
		/// </summary>
		/// <param name="item">The item to insert.</param>
		/// <param name="index">The zero-based index where the item should be inserted.</param>
		protected override void InsertItem(ListItem item, int index)
		{
			item.Changed += ItemOnChanged;

			Widget widget = null;

			if (!item.IsSeparator)
			{
				widget = new ListButton(ListBoxStyle.ListItemStyle, this)
				{
					Text = item.Text,
					TextColor = item.Color ?? ListBoxStyle.ListItemStyle.LabelStyle.TextColor,
					Tag = item,
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch,
					Image = item.Image,
					ImageTextSpacing = item.ImageTextSpacing
				};

				((ImageTextButton)widget).Click += ButtonOnClick;
			}
			else
			{
				var separator = new HorizontalSeparator(null);
				separator.ApplySeparatorStyle(ListBoxStyle.SeparatorStyle);
				widget = separator;
			}

			_box.Widgets.Insert(index, widget);

			item.Widget = widget;
		}

		/// <summary>
		/// Removes the specified item from the list box.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		protected override void RemoveItem(ListItem item)
		{
			item.Changed -= ItemOnChanged;

			var index = _box.Widgets.IndexOf(item.Widget);
			_box.Widgets.RemoveAt(index);

			if (SelectedItem == item)
			{
				SelectedItem = null;
			}
		}

		/// <summary>
		/// Clears all items from the list box.
		/// </summary>
		protected override void Reset()
		{
			while (_box.Widgets.Count > 0)
			{
				RemoveItem((ListItem)_box.Widgets[0].Tag);
			}
		}

		private void ButtonOnClick(object sender, MyraEventArgs eventArgs)
		{
			var item = (ImageTextButton)sender;
			if (!item.IsPressed)
			{
				return;
			}

			var listItem = (ListItem)item.Tag;
			if (SelectionMode == SelectionMode.Single)
			{
				SelectedItem = listItem;
			}

			ComboHideDropdown();
		}

		private void ComboHideDropdown()
		{
			if (_parentComboBox == null)
			{
				return;
			}

			_parentComboBox.HideDropdown();
		}

		/// <summary>
		/// Called when the selected item changes. Updates the parent combo box if this list box is part of one.
		/// </summary>
		protected override void OnSelectedItemChanged()
		{
			base.OnSelectedItemChanged();

			if (_parentComboBox != null)
			{
				_parentComboBox.UpdateSelectedItem();
			}
		}

		/// <summary>
		/// Handles keyboard input for navigating and selecting items in the list box.
		/// </summary>
		/// <param name="k">The key being pressed (supports Up, Down, and Enter keys).</param>
		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			switch(k)
			{
				case Keys.Up:
					if (SelectedIndex == null && Items.Count > 0)
					{
						SelectedIndex = Items.Count - 1;
						UpdateScrolling();
					}

					if (SelectedIndex != null && SelectedIndex.Value > 0)
					{
						SelectedIndex = SelectedIndex.Value - 1;
						UpdateScrolling();
					}
					break;
				case Keys.Down:
					if (SelectedIndex == null && Items.Count > 0)
					{
						SelectedIndex = 0;
						UpdateScrolling();
					}

					if (SelectedIndex != null && SelectedIndex.Value < Items.Count - 1)
					{
						SelectedIndex = SelectedIndex.Value + 1;
						UpdateScrolling();
					}
					break;
				case Keys.Enter:
					ComboHideDropdown();
					break;
			}
		}

		/// <summary>
		/// Called when the items collection changes. Resets the scroll position.
		/// </summary>
		protected override void OnItemCollectionChanged()
		{
			base.OnItemCollectionChanged();

			InternalChild.ResetScroll();
		}

		private void UpdateScrolling()
		{
			if (SelectedItem == null)
			{
				return;
			}

			InternalChild.UpdateArrange();

			// Determine item position within ListBox
			var widget = SelectedItem.Widget;
			var p = _box.ToLocal(widget.ToGlobal(widget.Bounds.Location));

			var lineHeight = ListBoxStyle.ListItemStyle.LabelStyle.Font.LineHeight;

			var sp = InternalChild.ScrollPosition;

			var sz = new Point(InternalChild.Bounds.Width, InternalChild.Bounds.Height);
			if (p.Y < sp.Y)
			{
				sp.Y = p.Y;
			}
			else if (p.Y + lineHeight > sp.Y + sz.Y)
			{
				sp.Y = p.Y + lineHeight - sz.Y;
			}

			InternalChild.ScrollPosition = sp;
		}

		/// <summary>
		/// Handles mouse wheel scrolling events within the list box.
		/// </summary>
		/// <param name="delta">The amount of scroll movement from the mouse wheel.</param>
		public override void OnMouseWheel(float delta)
		{
			base.OnMouseWheel(delta);

			InternalChild.OnMouseWheel(delta);
		}

		/// <summary>
		/// Applies the specified list box style to this list box and all its items.
		/// </summary>
		/// <param name="style">The style to apply.</param>
		public void ApplyListBoxStyle(ListBoxStyle style)
		{
			ApplyWidgetStyle(style);

			ListBoxStyle = style;

			foreach (var item in Items)
			{
				var asButton = item.Widget as ImageTextButton;
				if (asButton != null)
				{
					asButton.ApplyImageTextButtonStyle(style.ListItemStyle);
					if (item.Color != null)
					{
						asButton.TextColor = item.Color.Value;
					}
				}

				var asSeparator = item.Widget as SeparatorWidget;
				if (asSeparator != null)
				{
					asSeparator.ApplySeparatorStyle(style.SeparatorStyle);
				}
			}
		}

		/// <summary>
		/// Applies a named list box style from the stylesheet to the list box.
		/// </summary>
		/// <param name="stylesheet">The stylesheet containing the style.</param>
		/// <param name="name">The name of the list box style to apply.</param>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyListBoxStyle(stylesheet.ListBoxStyles.SafelyGetStyle(name));
		}
	}
}