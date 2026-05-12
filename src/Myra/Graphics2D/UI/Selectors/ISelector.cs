using System;
using System.Collections.ObjectModel;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Specifies whether a selector can have one or multiple selected items.
	/// </summary>
	public enum SelectionMode
	{
		/// <summary>
		/// Only one item can be selected at a time.
		/// </summary>
		Single,

		/// <summary>
		/// Multiple items can be selected at the same time.
		/// </summary>
		Multiple
	}

	/// <summary>
	/// Provides an internal interface for selector controls.
	/// </summary>
	internal interface ISelector
	{
		/// <summary>
		/// Gets or sets the selection mode for the selector.
		/// </summary>
		SelectionMode SelectionMode { get; set; }

		/// <summary>
		/// Gets or sets the index of the selected item.
		/// </summary>
		int? SelectedIndex { get; set; }

		/// <summary>
		/// Raised when the selected index changes.
		/// </summary>
		event EventHandler SelectedIndexChanged;
	}

	/// <summary>
	/// Provides a generic internal interface for selector controls with typed items.
	/// </summary>
	/// <typeparam name="ItemType">The type of items in the selector.</typeparam>
	internal interface ISelectorT<ItemType>: ISelector
	{
		/// <summary>
		/// Gets the collection of items in the selector.
		/// </summary>
		ObservableCollection<ItemType> Items { get; }

		/// <summary>
		/// Gets or sets the currently selected item.
		/// </summary>
		ItemType SelectedItem { get; set; }
	}
}
