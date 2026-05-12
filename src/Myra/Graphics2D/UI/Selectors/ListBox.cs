using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;

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
	/// A list box control that displays a vertical list of selectable items with optional scrolling.
	/// </summary>
	/// <remarks>This class is obsolete. Use ListView instead.</remarks>
	[Obsolete("Use ListView")]
	public class ListBox : Selector<ScrollViewer, ListItem>
	{
		private readonly VerticalStackPanel _box;
		internal ComboBox _parentComboBox;

		/// <summary>
		/// Gets or sets the style applied to this list box.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public ListBoxStyle ListBoxStyle
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the selection mode for this list box (Single or Multiple).
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
		/// Gets the scroll viewer that manages the layout of this list box.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public ScrollViewer ScrollViewer => InternalChild;

		/// <summary>
		/// Initializes a new instance of the ListBox class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public ListBox(string styleName = Stylesheet.DefaultStyleName) : base(new ScrollViewer())
		{
			AcceptsKeyboardFocus = true;

			_box = new VerticalStackPanel();
			InternalChild.Content = _box;

			SetStyle(styleName);
		}

		private void ItemOnChanged(object sender, EventArgs eventArgs)
		{
			var item = (ListItem)sender;

			var button = (ImageTextButton)item.Widget;
			button.Text = item.Text;
			button.TextColor = item.Color ?? ListBoxStyle.ListItemStyle.LabelStyle.TextColor;

			InvalidateMeasure();
		}

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

		protected override void Reset()
		{
			while (_box.Widgets.Count > 0)
			{
				RemoveItem((ListItem)_box.Widgets[0].Tag);
			}
		}

		private void ButtonOnClick(object sender, EventArgs eventArgs)
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

		protected override void OnSelectedItemChanged()
		{
			base.OnSelectedItemChanged();

			if (_parentComboBox != null)
			{
				_parentComboBox.UpdateSelectedItem();
			}
		}

		/// <summary>
		/// Handles key down events for navigation with Up/Down arrow keys and selection with Enter.
		/// </summary>
		/// <param name="k">The key that was pressed.</param>
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
		/// Handles mouse wheel events for scrolling the list box contents.
		/// </summary>
		/// <param name="delta">The mouse wheel delta value (positive for scroll up, negative for scroll down).</param>
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

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyListBoxStyle(stylesheet.ListBoxStyles.SafelyGetStyle(name));
		}
	}
}