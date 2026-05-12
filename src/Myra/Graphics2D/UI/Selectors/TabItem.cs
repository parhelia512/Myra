using System.ComponentModel;
using System;
using System.Text;
using System.Xml.Serialization;
using Myra.Attributes;
using Myra.Utility;
using Myra.MML;

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
	/// Represents a tab in a TabControl.
	/// </summary>
	public class TabItem : BaseObject, ISelectorItem, IContent
	{
		private Widget _content;
		private string _text;
		private Color? _color;
		private ListViewButton _button;

		/// <summary>
		/// Gets or sets the text displayed on the tab.
		/// </summary>
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
		/// Gets or sets the color of the tab text.
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
		/// Gets or sets the content widget for this tab.
		/// </summary>
		[Browsable(false)]
		[Content]
		public Widget Content
		{
			get => _content;
			set
			{
				if (_content == value)
				{
					return;
				}

				_content = value;
				FireChanged();
			}
		}

		/// <summary>
		/// Gets or sets arbitrary user data associated with this tab.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public object Tag { get; set; }

		/// <summary>
		/// Gets or sets the image displayed on the tab.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public IImage Image { get; set; }

		/// <summary>
		/// Gets or sets the spacing between the image and text on the tab.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int ImageTextSpacing { get; set; }

		/// <summary>
		/// Gets or sets the height of the tab content.
		/// </summary>
		[DefaultValue(null)]
		public int? Height { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		internal ListViewButton Button
		{
			get => _button;
			set
			{
				if (value == _button)
				{
					return;
				}

				if (_button != null)
				{
					_button.PressedChanged -= OnPressedChanged;
				}

				_button = value;

				if (_button != null)
				{
					_button.PressedChanged += OnPressedChanged;
				}
			}
		}

		[Browsable(false)]
		[XmlIgnore]
		private HorizontalStackPanel Panel => (HorizontalStackPanel)Button.Content;

		[Browsable(false)]
		[XmlIgnore]
		internal Image ImageWidget => (Image)Panel.Widgets[0];

		[Browsable(false)]
		[XmlIgnore]
		internal Label LabelWidget => (Label)Panel.Widgets[1];

		/// <summary>
		/// Gets or sets a value indicating whether this tab is selected.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool IsSelected
		{
			get => _button.IsPressed;
			set => _button.IsPressed = value;
		}

		/// <summary>
		/// Raised when the tab item is changed.
		/// </summary>
		public event EventHandler Changed;

		/// <summary>
		/// Raised when this tab is selected.
		/// </summary>
		public event EventHandler SelectedChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="TabItem"/> class.
		/// </summary>
		public TabItem()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TabItem"/> class with text, color, tag, and content.
		/// </summary>
		/// <param name="text">The text to display on the tab.</param>
		/// <param name="color">The color of the text.</param>
		/// <param name="tag">Optional user data.</param>
		/// <param name="content">The content widget for the tab.</param>
		public TabItem(string text, Color? color = null, object tag = null, Widget content = null)
		{
			Text = text;
			Color = color;
			Tag = tag;
			Content = content;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TabItem"/> class with text and content.
		/// </summary>
		/// <param name="text">The text to display on the tab.</param>
		/// <param name="content">The content widget for the tab.</param>
		public TabItem(string text, Widget content) : this(text, null, null, content)
		{
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

		private void OnPressedChanged(object sender, EventArgs args)
		{
			if (_button.IsPressed)
			{
				SelectedChanged.Invoke(this);
			}
		}

		public TabItem Clone()
		{
			var result = new TabItem
			{
				Text = Text,
				Color = Color,
				Tag = Tag,
				Image = Image,
				ImageTextSpacing = ImageTextSpacing,
				Height = Height
			};

			if (Content != null)
			{
				result.Content = Content.Clone();
			}

			return result;
		}
	}
}
