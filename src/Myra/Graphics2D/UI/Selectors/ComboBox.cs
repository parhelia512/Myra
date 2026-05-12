using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
	/// A dropdown combo box control that displays a single selection with a dropdown list.
	/// </summary>
	/// <remarks>This class is obsolete. Use ComboView instead.</remarks>
	[Obsolete("Use ComboView")]
	public class ComboBox : SelectorBase<ImageTextButton, ListItem>
	{
		private readonly ListBox _listBox = new ListBox(null);

		/// <summary>
		/// Gets or sets the maximum height of the dropdown list.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(300)]
		public int? DropdownMaximumHeight
		{
			get
			{
				return _listBox.MaxHeight;
			}

			set
			{
				_listBox.MaxHeight = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the dropdown list is expanded.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool IsExpanded
		{
			get { return InternalChild.IsPressed; }
		}

		/// <summary>
		/// Gets or sets the desktop this combo box is attached to.
		/// </summary>
		public override Desktop Desktop
		{
			get
			{
				return base.Desktop;
			}

			internal set
			{
				if (Desktop != null)
				{
					Desktop.ContextMenuClosed -= DesktopOnContextMenuClosed;
				}

				base.Desktop = value;

				if (Desktop != null)
				{
					Desktop.ContextMenuClosed += DesktopOnContextMenuClosed;
				}
			}
		}

		/// <summary>
		/// Gets the internal list box used for dropdown display.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public ListBox ListBox => _listBox;

		/// <summary>
		/// Gets the collection of items in the combo box.
		/// </summary>
		public override ObservableCollection<ListItem> Items => _listBox.Items;

		/// <summary>
		/// Gets or sets the currently selected item.
		/// </summary>
		public override ListItem SelectedItem { get => _listBox.SelectedItem; set => _listBox.SelectedItem = value; }

		/// <summary>
		/// Gets or sets the selection mode for the combo box.
		/// </summary>
		public override SelectionMode SelectionMode { get => _listBox.SelectionMode; set => _listBox.SelectionMode = value; }

		/// <summary>
		/// Gets or sets the index of the selected item.
		/// </summary>
		public override int? SelectedIndex { get => _listBox.SelectedIndex; set => _listBox.SelectedIndex = value; }

		/// <summary>
		/// Raised when the selected index changes.
		/// </summary>
		public override event EventHandler SelectedIndexChanged
		{
			add
			{
				_listBox.SelectedIndexChanged += value;
			}

			remove
			{
				_listBox.SelectedIndexChanged -= value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the ComboBox class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public ComboBox(string styleName = Stylesheet.DefaultStyleName) :
			base(new ImageTextButton(null)
			{
				Toggleable = true,
				HorizontalAlignment = HorizontalAlignment.Stretch
			})
		{
			AcceptsKeyboardFocus = true;

			InternalChild.PressedChanged += InternalChild_PressedChanged;

			_listBox._parentComboBox = this;
			_listBox.Items.CollectionChanged += Items_CollectionChanged;

			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			DropdownMaximumHeight = 300;

			SetStyle(styleName);
		}

		/// <summary>
		/// Hides the dropdown list if it's currently visible.
		/// </summary>
		internal void HideDropdown()
		{
			if (Desktop != null && Desktop.ContextMenu == _listBox)
			{
				Desktop.HideContextMenu();
			}
		}

		/// <summary>
		/// Handles the context menu closed event.
		/// </summary>
		private void DesktopOnContextMenuClosed(object sender, GenericEventArgs<Widget> genericEventArgs)
		{
			// Unpress the button only if mouse is outside
			// As if it is inside, then it'll get unpressed naturally
			if (!IsMouseInside)
			{
				InternalChild.IsPressed = false;
			}
		}

		/// <summary>
		/// Handles the pressed state change of the dropdown button.
		/// </summary>
		private void InternalChild_PressedChanged(object sender, EventArgs e)
		{
			if (_listBox.Items.Count == 0)
			{
				return;
			}

			if (InternalChild.IsPressed)
			{
				if (_listBox.SelectedIndex == null && Items.Count > 0)
				{
					_listBox.SelectedIndex = 0;
				}

				_listBox.Width = BorderBounds.Width;
				var pos = ToGlobal(new Point(0, Bounds.Height));
				Desktop.ShowContextMenu(_listBox, pos);
			}
		}

		/// <summary>
		/// Handles changes to list items.
		/// </summary>
		private void ItemOnChanged(object sender, EventArgs eventArgs)
		{
			var item = (ListItem)sender;

			if (SelectedItem == item)
			{
				InternalChild.Text = item.Text;

				var widget = (ListButton)item.Tag;
				InternalChild.TextColor = widget.TextColor;
			}

			InvalidateMeasure();
		}

		/// <summary>
		/// Handles changes to the items collection.
		/// </summary>
		private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						foreach (ListItem item in args.NewItems)
						{
							item.Changed += ItemOnChanged;
						}
						break;
					}

				case NotifyCollectionChangedAction.Remove:
					{
						foreach (ListItem item in args.OldItems)
						{
							item.Changed -= ItemOnChanged;
						}

						UpdateSelectedItem();
						break;
					}

				case NotifyCollectionChangedAction.Reset:
					UpdateSelectedItem();
					break;
			}

			InvalidateMeasure();
		}

		/// <summary>
		/// Updates the display of the selected item.
		/// </summary>
		internal void UpdateSelectedItem()
		{
			var item = SelectedItem;
			if (item != null)
			{
				InternalChild.Text = item.Text;
				InternalChild.TextColor = item.Color ?? _listBox.ListBoxStyle.ListItemStyle.LabelStyle.TextColor;
				((ImageTextButton)item.Widget).IsPressed = true;
			}
			else
			{
				InternalChild.Text = string.Empty;
			}
		}

		/// <summary>
		/// Applies a combo box style to this control.
		/// </summary>
		/// <param name="style">The combo box style to apply.</param>
		public void ApplyComboBoxStyle(ComboBoxStyle style)
		{
			if (style.ListBoxStyle != null)
			{
				var dropdownMaximumHeight = DropdownMaximumHeight;
				_listBox.ApplyListBoxStyle(style.ListBoxStyle);
				DropdownMaximumHeight = dropdownMaximumHeight;
			}

			InternalChild.ApplyImageTextButtonStyle(style);
		}

		/// <summary>
		/// Measures the required size for the combo box and its dropdown.
		/// </summary>
		protected override Point InternalMeasure(Point availableSize)
		{
			// Measure by the longest string
			var result = base.InternalMeasure(availableSize);

			// Temporary remove width, so it wont be used in the measure
			var oldWidth = _listBox.Width;
			_listBox.Width = null;

			// Make visible, otherwise Measure will return zero
			var wasVisible = _listBox.Visible;
			_listBox.Visible = true;

			var listResult = _listBox.Measure(new Point(10000, 10000));
			if (listResult.X > result.X)
			{
				result.X = listResult.X;
			}

			// Revert ListBox settings
			_listBox.Width = oldWidth;
			_listBox.Visible = wasVisible;

			// Add some x space
			result.X += 32;

			return result;
		}

		/// <summary>
		/// Arranges the combo box and sets the dropdown width.
		/// </summary>
		protected override void InternalArrange()
		{
			base.InternalArrange();

			_listBox.Width = BorderBounds.Width;
		}

		/// <summary>
		/// Handles keyboard input and delegates it to the list box.
		/// </summary>
		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			_listBox.OnKeyDown(k);
		}

		/// <summary>
		/// Sets the combo box's style using a stylesheet.
		/// </summary>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyComboBoxStyle(stylesheet.ComboBoxStyles.SafelyGetStyle(name));
		}
	}
}
