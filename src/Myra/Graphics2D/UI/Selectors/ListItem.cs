using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.MML;
using System.Text;
using Myra.Utility;
using Myra.Events;


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
	/// Represents an item in a list box.
	/// </summary>
	/// <remarks>This class is obsolete. Use <see cref="ListView"/> instead.</remarks>
	[Obsolete("Use ListView")]
	public class ListItem : BaseObject, ISelectorItem
	{
		private string _text;
		private Color? _color;
		private Widget _widget;

		/// <summary>
		/// Gets or sets the text of the list item.
		/// </summary>
		[DefaultValue(null)]
		public string Text
		{
			get
			{
				return _text;
			}

			set
			{
				if (value == _text)
				{
					return;
				}

				_text = value;
				FireChanged();
			}
		}

		/// <summary>
		/// Gets or sets the color of the list item text.
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
		/// Gets or sets arbitrary user data associated with the list item.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public object Tag
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this item is a separator.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool IsSeparator { get; set; }

		/// <summary>
		/// Gets or sets the image displayed with the list item.
		/// </summary>
		public IImage Image { get; set; }

		/// <summary>
		/// Gets or sets the spacing between the image and text.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int ImageTextSpacing { get; set; }

		/// <summary>
		/// Gets or sets the widget representing this item.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		internal Widget Widget
		{
			get
			{
				return _widget;
			}

			set
			{
				_widget = value;

				var asButton = _widget as ImageTextButton;
				if (asButton != null)
				{
					asButton.PressedChanged += (s, a) => FireSelectedChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the list item is selected.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool IsSelected
		{
			get
			{
				var asButton = _widget as ImageTextButton;
				if (asButton == null)
				{
					return false;
				}

				return asButton.IsPressed;
			}

			set
			{
				var asButton = _widget as ImageTextButton;
				if (asButton == null)
				{
					return;
				}

				asButton.IsPressed = value;
			}
		}

		/// <summary>
		/// Occurs when the list item has changed.
		/// </summary>
		public event MyraEventHandler Changed;

		/// <summary>
		/// Occurs when the selected state of the list item has changed.
		/// </summary>
		public event MyraEventHandler SelectedChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="ListItem"/> class.
		/// </summary>
		public ListItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListItem"/> class with the specified text, color, and tag.
		/// </summary>
		/// <param name="text">The text of the item.</param>
		/// <param name="color">The color of the item text.</param>
		/// <param name="tag">Arbitrary user data associated with the item.</param>
		public ListItem(string text, Color? color, object tag)
		{
			Text = text;
			Color = color;
			Tag = tag;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListItem"/> class with the specified text and color.
		/// </summary>
		/// <param name="text">The text of the item.</param>
		/// <param name="color">The color of the item text.</param>
		public ListItem(string text, Color? color) : this(text, color, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListItem"/> class with the specified text.
		/// </summary>
		/// <param name="text">The text of the item.</param>
		public ListItem(string text) : this(text, null)
		{
		}

		/// <summary>
		/// Raises the SelectedChanged event.
		/// </summary>
		public void FireSelectedChanged()
		{
			var ev = SelectedChanged;
			if (ev != null)
			{
				ev(this,new MyraEventArgs(InputEventType.SelectionChanged));
			}
		}

		/// <summary>
		/// Returns a string representation of the list item, including its text and id if available.
		/// </summary>
		/// <returns>A string containing the item's text and id in the format "Text (#id)".</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();

			if (!string.IsNullOrEmpty(Text))
			{
				sb.Append(Text);
				sb.Append(" ");
			}

			if (!string.IsNullOrEmpty(Id))
			{
				sb.Append("(#");
				sb.Append(Id);
				sb.Append(")");
			}
			return sb.ToString();
		}

		/// <summary>
		/// Called when the Id property changes, and fires the Changed event.
		/// </summary>
		protected internal override void OnIdChanged()
		{
			base.OnIdChanged();

			FireChanged();
		}

		/// <summary>
		/// Fires the Changed event to notify listeners that this item has changed.
		/// </summary>
		protected void FireChanged()
		{
			Changed.Invoke(this, InputEventType.SelectionChanged);
		}

		/// <summary>
		/// Creates a deep copy of the list item.
		/// </summary>
		/// <returns>A new ListItem with the same properties as this item.</returns>
		public ListItem Clone()
		{
			return new ListItem
			{
				Id = Id,
				Text = Text,
				Color = Color,
				Tag = Tag,
				IsSeparator = IsSeparator,
				Image = Image,
				ImageTextSpacing = ImageTextSpacing,
			};
		}
	}
}
