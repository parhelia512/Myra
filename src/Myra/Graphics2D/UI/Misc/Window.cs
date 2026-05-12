using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.Attributes;
using FontStashSharp;
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
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A window control with a title bar, optional close button, and content area.
	/// </summary>
	public class Window : ContentControl
	{
		private readonly StackPanelLayout _layout = new StackPanelLayout(Orientation.Vertical);
		private readonly Label _titleLabel;
		private Widget _content;
		private Widget _previousKeyboardFocus;

		/// <summary>
		/// Gets or sets the title text displayed in the window's title bar.
		/// </summary>
		[Category("Appearance")]
		public string Title
		{
			get
			{
				return _titleLabel.Text;
			}

			set
			{
				_titleLabel.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets the color of the title text.
		/// </summary>
		[Category("Appearance")]
		[StylePropertyPath("TitleStyle/TextColor")]
		public Color TitleTextColor
		{
			get
			{
				return _titleLabel.TextColor;
			}
			set
			{
				_titleLabel.TextColor = value;
			}
		}

		/// <summary>
		/// Gets or sets the font used to render the title text.
		/// </summary>
		[Category("Appearance")]
		public SpriteFontBase TitleFont
		{
			get
			{
				return _titleLabel.Font;
			}
			set
			{
				_titleLabel.Font = value;
			}
		}

		/// <summary>
		/// Gets the panel that contains the title and close button.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public HorizontalStackPanel TitlePanel { get; private set; }

		/// <summary>
		/// Gets the close button in the window's title bar.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Button CloseButton { get; private set; }

		/// <summary>
		/// Gets or sets the content widget displayed in the window's content area.
		/// </summary>
		[Browsable(false)]
		[Content]
		public override Widget Content
		{
			get
			{
				return _content;
			}

			set
			{
				if (value == Content)
				{
					return;
				}

				// Remove existing
				if (_content != null)
				{
					Children.Remove(_content);
				}

				if (value != null)
				{
					StackPanel.SetProportionType(value, ProportionType.Fill);
					Children.Insert(1, value);
				}

				_content = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating the result of a dialog window.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool Result { get; set; }

		/// <summary>
		/// Gets or sets the horizontal alignment of this window.
		/// </summary>
		[DefaultValue(HorizontalAlignment.Left)]
		public override HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return base.HorizontalAlignment;
			}
			set
			{
				base.HorizontalAlignment = value;
			}
		}

		/// <summary>
		/// Gets or sets the vertical alignment of this window.
		/// </summary>
		[DefaultValue(VerticalAlignment.Top)]
		public override VerticalAlignment VerticalAlignment
		{
			get
			{
				return base.VerticalAlignment;
			}
			set
			{
				base.VerticalAlignment = value;
			}
		}

		/// <summary>
		/// Gets or sets the drag direction for moving this window.
		/// </summary>
		[DefaultValue(DragDirection.Both)]
		public override DragDirection DragDirection { get => base.DragDirection; set => base.DragDirection = value; }

		/// <summary>
		/// Gets or sets the key that closes this window (typically Escape).
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(Keys.Escape)]
		public Keys? CloseKey { get; set; }

		private bool IsWindowPlaced { get; set; }

		public event EventHandler<CancellableEventArgs> Closing;
		public event EventHandler Closed;

		public Window(string styleName = Stylesheet.DefaultStyleName)
		{
			_layout.Spacing = 8;
			ChildrenLayout = _layout;

			AcceptsKeyboardFocus = true;
			CloseKey = Keys.Escape;

			DragDirection = DragDirection.Both;

			Result = false;
			HorizontalAlignment = HorizontalAlignment.Left;
			VerticalAlignment = VerticalAlignment.Top;

			TitlePanel = new HorizontalStackPanel
			{
				Spacing = 8
			};
			DragHandle = TitlePanel;

			_titleLabel = new Label();
			StackPanel.SetProportionType(_titleLabel, ProportionType.Fill);
			TitlePanel.Widgets.Add(_titleLabel);

			CloseButton = new Button
			{
				Content = new Image()
			};

			CloseButton.Click += (sender, args) =>
			{
				Close();
			};

			TitlePanel.Widgets.Add(CloseButton);

			Children.Add(TitlePanel);

			SetStyle(styleName);
		}

		protected override void InternalArrange()
		{
			base.InternalArrange();

			if (!IsWindowPlaced)
			{
				CenterOnDesktop();
				IsWindowPlaced = true;
			}
		}

		public void CenterOnDesktop()
		{
			var size = Bounds.Size();
			Left = (ContainerBounds.Width - size.X) / 2;
			Top = (ContainerBounds.Height - size.Y) / 2;
		}

		public override void OnTouchDown()
		{
			BringToFront();
			base.OnTouchDown();
		}

		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (k == CloseKey)
			{
				Close();
			}
		}

		public void ApplyWindowStyle(WindowStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.TitleStyle != null)
			{
				_titleLabel.ApplyLabelStyle(style.TitleStyle);
			}

			if (style.CloseButtonStyle != null)
			{
				CloseButton.ApplyButtonStyle(style.CloseButtonStyle);
				if (style.CloseButtonStyle.ImageStyle != null)
				{
					var image = (Image)CloseButton.Content;
					image.ApplyPressableImageStyle(style.CloseButtonStyle.ImageStyle);
				}
			}
		}

		private void InternalShow(Desktop desktop, Point? position = null)
		{
			Visible = true;
			Desktop = desktop;
			Desktop.Widgets.Add(this);

			if (position != null)
			{
				Left = position.Value.X;
				Top = position.Value.Y;
				IsWindowPlaced = true;
			}
		}

		public void Show(Desktop desktop, Point? position = null)
		{
			IsModal = false;
			InternalShow(desktop, position);
		}

		public void ShowModal(Desktop desktop, Point? position = null)
		{
			IsModal = true;
			InternalShow(desktop, position);

			_previousKeyboardFocus = desktop.FocusedKeyboardWidget;

			// Force mouse wheel focused to be set to the first appropriate widget in the next Desktop.UpdateLayout
			if (AcceptsKeyboardFocus)
			{
				Desktop.FocusedKeyboardWidget = this;
			}
		}

		public virtual void Close()
		{
			if (Desktop == null)
			{
				// Is closed already
				return;
			}

			var ev = Closing;
			if (ev != null)
			{
				var args = new CancellableEventArgs();
				ev(this, args);
				if (args.Cancel)
				{
					return;
				}
			}

			if (IsModal)
			{
				Desktop.FocusedKeyboardWidget = _previousKeyboardFocus;
			}

			if (Desktop.Widgets.Contains(this))
			{
				RemoveFromDesktop();
			}
			else
			{
				//todo fix remove error. DONE
				RemoveFromParent();
			}

			Closed.Invoke(this);
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyWindowStyle(stylesheet.WindowStyles.SafelyGetStyle(name));
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var window = (Window)w;

			Title = window.Title;
			TitleTextColor = window.TitleTextColor;
			TitleFont = window.TitleFont;
			CloseKey = window.CloseKey;
		}
	}
}