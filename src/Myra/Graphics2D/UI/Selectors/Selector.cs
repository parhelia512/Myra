using Myra.Attributes;
using Myra.Utility;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Abstract base class for selector controls that display a collection of selectable items.
	/// </summary>
	/// <typeparam name="WidgetType">The type of widget that contains the items.</typeparam>
	/// <typeparam name="ItemType">The type of items in the selector.</typeparam>
	public abstract class SelectorBase<WidgetType, ItemType> : Widget, ISelectorT<ItemType>
		where WidgetType : Widget
		where ItemType : class, ISelectorItem
	{
		private readonly SingleItemLayout<WidgetType> _layout;

		/// <summary>
		/// Gets or sets the selection mode (Single or Multiple) for this selector.
		/// </summary>
		[DefaultValue(SelectionMode.Single)]
		public abstract SelectionMode SelectionMode { get; set; }

		/// <summary>
		/// Gets the collection of items in this selector.
		/// </summary>
		[Browsable(false)]
		[Content]
		public abstract ObservableCollection<ItemType> Items { get; }

		/// <summary>
		/// Gets or sets the index of the selected item.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public abstract int? SelectedIndex { get; set; }

		/// <summary>
		/// Gets or sets the currently selected item.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public abstract ItemType SelectedItem { get; set; }

		/// <summary>
		/// Gets the internal child widget containing the items.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		protected WidgetType InternalChild => _layout.Child;

		/// <summary>
		/// Raised when the selected index changes.
		/// </summary>
		public abstract event EventHandler SelectedIndexChanged;

		/// <summary>
		/// Initializes a new instance of the SelectorBase class.
		/// </summary>
		/// <param name="widget">The widget that will contain the selector items.</param>
		protected SelectorBase(WidgetType widget)
		{
			_layout = new SingleItemLayout<WidgetType>(this)
			{
				Child = widget
			};
			ChildrenLayout = _layout;
		}
	}

	/// <summary>
	/// Generic base class for selector controls with a collection of items and selection support.
	/// </summary>
	/// <typeparam name="WidgetType">The type of widget that contains the items.</typeparam>
	/// <typeparam name="ItemType">The type of items in the selector.</typeparam>
	public abstract class Selector<WidgetType, ItemType> : SelectorBase<WidgetType, ItemType>
		where WidgetType : Widget
		where ItemType : class, ISelectorItem
	{
		private ItemType _selectedItem;

		/// <summary>
		/// Gets or sets the selection mode (Single or Multiple) for this selector.
		/// </summary>
		public override SelectionMode SelectionMode { get; set; }

		/// <summary>
		/// Gets the collection of items in this selector.
		/// </summary>
		public override ObservableCollection<ItemType> Items { get; } = new ObservableCollection<ItemType>();

		/// <summary>
		/// Gets or sets the index of the selected item.
		/// </summary>
		public override int? SelectedIndex
		{
			get
			{
				if (_selectedItem == null)
				{
					return null;
				}

				return Items.IndexOf(_selectedItem);
			}

			set
			{
				if (value == null || value.Value < 0 || value.Value >= Items.Count)
				{
					SelectedItem = default(ItemType);
					return;
				}

				SelectedItem = Items[value.Value];
			}
		}

		/// <summary>
		/// Gets or sets the currently selected item.
		/// </summary>
		public override ItemType SelectedItem
		{
			get
			{
				return _selectedItem;
			}

			set
			{
				if (value == _selectedItem)
				{
					return;
				}

				if (SelectionMode == SelectionMode.Single && _selectedItem != null)
				{
					_selectedItem.IsSelected = false;
				}

				_selectedItem = value;

				if (_selectedItem != null)
				{
					_selectedItem.IsSelected = true;
				}

				FireSelectedIndexChanged();
				OnSelectedItemChanged();
			}
		}

		/// <summary>
		/// Raised when the selected index changes.
		/// </summary>
		public override event EventHandler SelectedIndexChanged;

		/// <summary>
		/// Raised when the items collection changes.
		/// </summary>
		public event EventHandler ItemsCollectionChanged;

		/// <summary>
		/// Initializes a new instance of the Selector class.
		/// </summary>
		/// <param name="widget">The widget that will contain the selector items.</param>
		protected Selector(WidgetType widget): base(widget)
		{
			widget.HorizontalAlignment = HorizontalAlignment.Stretch;
			widget.VerticalAlignment = VerticalAlignment.Stretch;

			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			Items.CollectionChanged += ItemsOnCollectionChanged;
		}

		private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
				{
					var index = args.NewStartingIndex;
					foreach (ItemType item in args.NewItems)
					{
						InsertItem(item, index);
						++index;
					}
					break;
				}

				case NotifyCollectionChangedAction.Remove:
				{
					foreach (ItemType item in args.OldItems)
					{
						RemoveItem(item);
					}
					break;
				}

				case NotifyCollectionChangedAction.Reset:
				{
					Reset();
					break;
				}
			}

			OnItemCollectionChanged();
			InvalidateMeasure();
		}

		protected virtual void OnItemCollectionChanged()
		{
			ItemsCollectionChanged.Invoke(this);
		}

		protected virtual void OnSelectedItemChanged()
		{
		}

		private void FireSelectedIndexChanged()
		{
			SelectedIndexChanged.Invoke(this);
		}

		protected abstract void Reset();
		protected abstract void InsertItem(ItemType item, int index);
		protected abstract void RemoveItem(ItemType item);
	}
}