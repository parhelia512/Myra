using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.MML;
using System.Text;
using Myra.Utility;

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
	/// Represents an item in a list control. This class is obsolete; use ListView instead.
	/// </summary>
	[Obsolete("Use ListView")]
	public class ListItem : BaseObject, ISelectorItem
	{
		private string _text;
		private Color? _color;
		private Widget _widget;

		/// <summary>
		/// Gets or sets the text of this list item.
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
		/// Gets or sets the color of this list item.
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
		/// Gets or sets arbitrary user data associated with this list item.
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
		/// Gets or sets the image for this list item.
		/// </summary>
		public IImage Image { get; set; }

		/// <summary>
		/// Gets or sets the spacing between the image and text.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int ImageTextSpacing { get; set; }

		/// <summary>
		/// Gets or sets the widget associated with this list item.
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
		/// Gets or sets a value indicating whether this item is selected.
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
		/// Raised when the list item is changed.
		/// </summary>
		public event EventHandler Changed;

		/// <summary>
		/// Raised when the selected state of this item changes.
		/// </summary>
		public event EventHandler SelectedChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="ListItem"/> class.
		/// </summary>
		public ListItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListItem"/> class with text, color, and tag.
		/// </summary>
		/// <param name="text">The text of the item.</param>
		/// <param name="color">The color of the item.</param>
		/// <param name="tag">Optional user data.</param>
		public ListItem(string text, Color? color, object tag)
		{
			Text = text;
			Color = color;
			Tag = tag;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListItem"/> class with text and color.
		/// </summary>
		/// <param name="text">The text of the item.</param>
		/// <param name="color">The color of the item.</param>
		public ListItem(string text, Color? color) : this(text, color, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListItem"/> class with text.
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
				ev(this, EventArgs.Empty);
			}
		}

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

		protected internal override void OnIdChanged()
		{
			base.OnIdChanged();

			FireChanged();
		}

		protected void FireChanged()
		{
			Changed.Invoke(this);
		}

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
