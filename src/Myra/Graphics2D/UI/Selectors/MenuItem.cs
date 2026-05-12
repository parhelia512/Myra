using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;
using System;
using Myra.Attributes;
using Myra.MML;
using FontStashSharp.RichText;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Represents an item in a menu with optional text, color, image, shortcut, and sub-items.
	/// </summary>
	public class MenuItem : BaseObject, IMenuItem
	{
		private string _shortcutText;
		private Color? _shortcutColor;
		private IImage _image;
		private string _text;
		private Color? _color;
		private bool _displayTextDirty = true;
		private string _displayText, _disabledDisplayText;

		internal readonly Image ImageWidget = new Image
		{
			VerticalAlignment = VerticalAlignment.Center
		};

		internal readonly Label Label = new Label(null)
		{
			VerticalAlignment = VerticalAlignment.Center
		};

		internal readonly Label Shortcut = new Label(null)
		{
			VerticalAlignment = VerticalAlignment.Center
		};


		internal readonly Menu SubMenu = new VerticalMenu();

		/// <summary>
		/// Gets or sets the text of this menu item. Use '&' to specify a keyboard shortcut character.
		/// </summary>
		[DefaultValue(null)]
		public string Text
		{
			get { return _text; }
			set
			{
				if (value == _text)
				{
					return;
				}

				_text = value;
				_displayTextDirty = true;

				UnderscoreChar = null;
				if (value != null)
				{
					var underscoreIndex = value.IndexOf('&');
					if (underscoreIndex >= 0 && underscoreIndex + 1 < value.Length)
					{
						UnderscoreChar = char.ToLower(value[underscoreIndex + 1]);
					}
				}

				FireChanged();
			}
		}

		internal string DisplayText
		{
			get
			{
				UpdateDisplayText();
				return _displayText;
			}
		}

		internal string DisabledDisplayText
		{
			get
			{
				UpdateDisplayText();
				return _disabledDisplayText;
			}
		}

		/// <summary>
		/// Gets or sets the color of this menu item's text.
		/// </summary>
		[DefaultValue(null)]
		public Color? Color
		{
			get
			{
				return _color;
			}

			set
			{
				if (value == _color)
				{
					return;
				}

				_color = value;
				FireChanged();
			}
		}

		/// <summary>
		/// Gets or sets an arbitrary user-defined object associated with this menu item.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public object Tag
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the image displayed for this menu item.
		/// </summary>
		public IImage Image
		{
			get
			{
				return _image;
			}

			set
			{
				if (value == _image)
				{
					return;
				}

				_image = value;
				FireChanged();
			}
		}

		/// <summary>
		/// Gets or sets the text displayed as the keyboard shortcut hint (e.g., "Ctrl+S").
		/// </summary>
		[DefaultValue(null)]
		public string ShortcutText
		{
			get
			{
				return _shortcutText;
			}

			set
			{
				if (value == _shortcutText)
				{
					return;
				}

				_shortcutText = value;
				FireChanged();
			}
		}

		/// <summary>
		/// Gets or sets the color of the shortcut text.
		/// </summary>
		[DefaultValue(null)]
		public Color? ShortcutColor
		{
			get
			{
				return _shortcutColor;
			}

			set
			{
				if (value == _shortcutColor)
				{
					return;
				}

				_shortcutColor = value;
				FireChanged();
			}
		}

		/// <summary>
		/// Gets or sets the parent menu that contains this menu item.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Menu Menu { get; set; }

		/// <summary>
		/// Gets the collection of sub-items (menu items) for this menu item.
		/// </summary>
		[Browsable(false)]
		[Content]
		public ObservableCollection<IMenuItem> Items
		{
			get { return SubMenu.Items; }
		}

		/// <summary>
		/// Gets or sets whether this menu item is enabled and can be selected.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool Enabled
		{
			get { return ImageWidget.Enabled; }

			set
			{
				ImageWidget.Enabled = Label.Enabled = Shortcut.Enabled = value;
			}
		}

		/// <summary>
		/// Gets the keyboard shortcut character extracted from the text (marked with '&'). Returns null if no shortcut is specified.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public char? UnderscoreChar { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this menu item has sub-items that can be opened.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool CanOpen
		{
			get
			{
				return Items.Count > 0;
			}
		}

		/// <summary>
		/// Gets or sets the index of this menu item within its parent menu.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int Index { get; set; }

		/// <summary>
		/// Raised when this menu item is selected by the user.
		/// </summary>
		public event EventHandler Selected;

		/// <summary>
		/// Raised when any property of this menu item changes.
		/// </summary>
		public event EventHandler Changed;

		/// <summary>
		/// Initializes a new instance of the MenuItem class with ID, text, color, and tag.
		/// </summary>
		/// <param name="id">The unique identifier for this menu item.</param>
		/// <param name="text">The display text for this menu item.</param>
		/// <param name="color">The text color, or null to use the default color.</param>
		/// <param name="tag">An arbitrary object to associate with this menu item.</param>
		public MenuItem(string id, string text, Color? color, object tag)
		{
			Id = id;
			Text = text;
			Color = color;
			Tag = tag;
		}

		/// <summary>
		/// Initializes a new instance of the MenuItem class with ID, text, and color.
		/// </summary>
		/// <param name="id">The unique identifier for this menu item.</param>
		/// <param name="text">The display text for this menu item.</param>
		/// <param name="color">The text color, or null to use the default color.</param>
		public MenuItem(string id, string text, Color? color) : this(id, text, color, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MenuItem class with ID and text.
		/// </summary>
		/// <param name="id">The unique identifier for this menu item.</param>
		/// <param name="text">The display text for this menu item.</param>
		public MenuItem(string id, string text) : this(id, text, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MenuItem class with only an ID.
		/// </summary>
		/// <param name="id">The unique identifier for this menu item.</param>
		public MenuItem(string id) : this(id, string.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MenuItem class with no parameters.
		/// </summary>
		public MenuItem() : this(string.Empty)
		{
		}

		private void UpdateDisplayText()
		{
			if (!_displayTextDirty)
			{
				return;
			}

			if (UnderscoreChar == null)
			{
				_disabledDisplayText = _displayText = Text;
			}
			else
			{
				var originalColor = Menu.MenuStyle.LabelStyle.TextColor;
				if (Color != null)
				{
					originalColor = Color.Value;
				}

				var specialCharColor = Menu.MenuStyle.SpecialCharColor;
				var underscoreIndex = Text.IndexOf('&');

				var underscoreChar = Text[underscoreIndex + 1];

				_disabledDisplayText = Text.Substring(0, underscoreIndex) + Text.Substring(underscoreIndex + 1);

				if (specialCharColor != null)
				{
					_displayText = Text.Substring(0, underscoreIndex) +
						@"/c[" + specialCharColor.Value.ToHexString() + "]" +
						underscoreChar.ToString() +
						@"/c[" + originalColor.ToHexString() + "]" +
						Text.Substring(underscoreIndex + 2);
				}
				else
				{
					_displayText = _disabledDisplayText;
				}
			}

			_displayTextDirty = false;
		}

		/// <summary>
		/// Searches this menu item and its sub-items for a menu item with the specified ID.
		/// </summary>
		/// <param name="id">The ID to search for.</param>
		/// <returns>The menu item with the specified ID, or null if not found.</returns>
		internal MenuItem FindMenuItemById(string id)
		{
			if (Id == id)
			{
				return this;
			}

			foreach (var item in SubMenu.Items)
			{
				var asMenuItem = item as MenuItem;
				if (asMenuItem == null)
				{
					continue;
				}

				var result = asMenuItem.FindMenuItemById(id);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		/// <summary>
		/// Raises the Selected event to notify that this menu item was selected.
		/// </summary>
		public void FireSelected()
		{
			var ev = Selected;

			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Invoked when the ID of this menu item changes.
		/// </summary>
		protected internal override void OnIdChanged()
		{
			base.OnIdChanged();

			FireChanged();
		}

		/// <summary>
		/// Raises the Changed event to notify that a property of this menu item has changed.
		/// </summary>
		protected void FireChanged()
		{
			var ev = Changed;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}
	}
}