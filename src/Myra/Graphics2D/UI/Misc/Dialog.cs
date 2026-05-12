using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using Myra.Attributes;

#if MONOGAME || FNA
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Input;
#else
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A dialog window with OK and Cancel buttons.
	/// </summary>
	[StyleTypeName("Window")]
	public class Dialog : Window
	{
		/// <summary>
		/// Gets the OK button of this dialog.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Button ButtonOk { get; private set; }

		/// <summary>
		/// Gets the Cancel button of this dialog.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Button ButtonCancel { get; private set; }

		/// <summary>
		/// Gets or sets the key used to confirm the dialog (typically Enter).
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(Keys.Enter)]
		public Keys ConfirmKey { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Dialog"/> class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public Dialog(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			ConfirmKey = Keys.Enter;

			var buttonsPanel = new HorizontalStackPanel()
			{
				Spacing = 8,
				HorizontalAlignment = HorizontalAlignment.Right
			};

			ButtonOk = new Button
			{
				Content = new Label
				{
					Text = "Ok"
				}
			};

			ButtonOk.Click += (sender, args) =>
			{
				OnOk();
			};

			buttonsPanel.Widgets.Add(ButtonOk);

			ButtonCancel = new Button
			{
				Content = new Label
				{
					Text = "Cancel",
				}
			};

			ButtonCancel.Click += (sender, args) =>
			{
				Result = false;
				Close();
			};

			buttonsPanel.Widgets.Add(ButtonCancel);
			Children.Add(buttonsPanel);
		}

		public override void OnKeyDown(Keys k)
		{
			FireKeyDown(k);

			if (k == CloseKey)
			{
				CloseButton.DoClick();
			} else if (k == ConfirmKey)
			{
				ButtonOk.DoClick();
			}
		}

		protected internal virtual void OnOk()
		{
			if (!CanCloseByOk())
			{
				return;
			}

			Result = true;
			Close();
		}

		protected internal virtual bool CanCloseByOk()
		{
			return true;
		}

		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var dialog = (Dialog)w;
			ConfirmKey = dialog.ConfirmKey;
		}


		public static Dialog CreateMessageBox(string title, Widget content)
		{
			var w = new Dialog
			{
				Title = title,
				Content = content
			};

			return w;
		}

		public static Dialog CreateMessageBox(string title, string message)
		{
			var messageLabel = new Label
			{
				Text = message,
				Wrap = true
			};

			return CreateMessageBox(title, messageLabel);
		}
	}
}