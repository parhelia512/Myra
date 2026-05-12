using System;
using System.Collections;

namespace Myra.Graphics2D.UI.Properties
{
	/// <summary>
	/// A widget that provides a user interface for editing collection items with property editors and item management.
	/// </summary>
	public class CollectionEditor : Widget
	{
		private readonly StackPanelLayout _layout = new StackPanelLayout(Orientation.Vertical);
		private readonly IList _collection;
		private readonly Type _type;
		private readonly ListView _listItems;
		private readonly PropertyGrid _propertyGrid;
		private readonly Button _buttonDelete, _buttonMoveUp, _buttonMoveDown;

		/// <summary>
		/// Initializes a new instance of the CollectionEditor class.
		/// </summary>
		/// <param name="collection">The collection to edit.</param>
		/// <param name="type">The type of items in the collection.</param>
		public CollectionEditor(IList collection, Type type)
		{
			ChildrenLayout = _layout;

			Width = 500;
			Height = 400;

			_collection = collection;
			_type = type;

			Children.Add(new HorizontalSeparator());

			var splitPanel = new HorizontalSplitPane();
			StackPanel.SetProportionType(splitPanel, ProportionType.Fill);

			_listItems = new ListView
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};

			_listItems.SelectedIndexChanged += ListItemsOnSelectedIndexChanged;

			// Add initial items
			foreach (var item in _collection)
			{
				var widget = new Label
				{
					Text = BuildItemText(item),
					Tag = item
				};

				_listItems.Widgets.Add(widget);
			}

			splitPanel.Widgets.Add(_listItems);

			_propertyGrid = new PropertyGrid();
			Grid.SetColumn(_propertyGrid, 1);
			_propertyGrid.PropertyChanged += PropertyGridOnPropertyChanged;
			splitPanel.Widgets.Add(_propertyGrid);

			Children.Add(splitPanel);
			Children.Add(new HorizontalSeparator());

			var buttonsPanel = new HorizontalStackPanel
			{
				Spacing = 4
			};

			var buttonNew = Button.CreateTextButton("New");
			buttonNew.Click += ButtonNewOnUp;
			buttonsPanel.Widgets.Add(buttonNew);

			_buttonDelete = Button.CreateTextButton("Delete");
			Grid.SetColumn(_buttonDelete, 1);
			_buttonDelete.Click += ButtonDeleteOnUp;
			buttonsPanel.Widgets.Add(_buttonDelete);

			_buttonMoveUp = Button.CreateTextButton("Up");
			Grid.SetColumn(_buttonMoveUp, 2);
			_buttonMoveUp.Click += ButtonMoveUpOnUp;
			buttonsPanel.Widgets.Add(_buttonMoveUp);

			_buttonMoveDown = Button.CreateTextButton("Down");
			Grid.SetColumn(_buttonMoveDown, 3);
			_buttonMoveDown.Click += ButtonMoveDownOnUp;
			buttonsPanel.Widgets.Add(_buttonMoveDown);

			Children.Add(buttonsPanel);

			UpdateButtonsEnabled();
		}

		/// <summary>
		/// Builds a display text string for the specified item.
		/// </summary>
		/// <param name="item">The item to build text for.</param>
		/// <returns>The display text for the item.</returns>
		private string BuildItemText(object item)
		{
			return item.ToString();
		}

		/// <summary>
		/// Updates the enabled state of the action buttons based on the current selection.
		/// </summary>
		private void UpdateButtonsEnabled()
		{
			_buttonDelete.Enabled = _listItems.SelectedIndex >= 0;
			_buttonMoveUp.Enabled = _listItems.SelectedIndex > 0;
			_buttonMoveDown.Enabled = _listItems.Widgets.Count > 1 && _listItems.SelectedIndex < _listItems.Widgets.Count - 1;
		}

		/// <summary>
		/// Moves the currently selected item to a new position in the list.
		/// </summary>
		/// <param name="newIndex">The new position for the selected item.</param>
		private void MoveSelectedItem(int newIndex)
		{
			var removed = _listItems.SelectedItem;
			_listItems.Widgets.Remove(removed);
			_listItems.Widgets.Insert(newIndex, removed);
			_listItems.SelectedItem = removed;
			UpdateButtonsEnabled();
		}

		private void ButtonMoveDownOnUp(object sender, EventArgs eventArgs)
		{
			MoveSelectedItem(_listItems.SelectedIndex.Value + 1);
		}

		private void ButtonMoveUpOnUp(object sender, EventArgs eventArgs)
		{
			MoveSelectedItem(_listItems.SelectedIndex.Value - 1);
		}

		private void ButtonDeleteOnUp(object sender, EventArgs eventArgs)
		{
			_listItems.Widgets.Remove(_listItems.SelectedItem);
			UpdateButtonsEnabled();
		}

		private void ButtonNewOnUp(object sender, EventArgs eventArgs)
		{
			var newItem = Activator.CreateInstance(_type);

			var widget = new Label
			{
				Text = newItem.ToString(),
				Tag = newItem
			};
			_listItems.Widgets.Add(widget);

			_listItems.SelectedIndex = _listItems.Widgets.Count - 1;
			UpdateButtonsEnabled();
		}

		/// <summary>
		/// Handles property grid changes by updating the selected item's display text.
		/// </summary>
		private void PropertyGridOnPropertyChanged(object sender, EventArgs eventArgs)
		{
			if (_listItems.SelectedItem == null)
			{
				return;
			}

			((Label)_listItems.SelectedItem).Text = BuildItemText(_listItems.SelectedItem.Tag);
		}

		/// <summary>
		/// Handles list selection changes by updating the property grid to show the selected item's properties.
		/// </summary>
		private void ListItemsOnSelectedIndexChanged(object sender, EventArgs eventArgs)
		{
			if (_listItems.SelectedItem != null)
			{
				_propertyGrid.Object = _listItems.SelectedItem.Tag;
			}
			else
			{
				_propertyGrid.Object = null;
			}

			UpdateButtonsEnabled();
		}

		/// <summary>
		/// Saves the edited collection back to the original collection.
		/// </summary>
		public void SaveChanges()
		{
			_collection.Clear();

			foreach (var listItem in _listItems.Widgets)
			{
				_collection.Add(listItem.Tag);
			}
		}
	}
}