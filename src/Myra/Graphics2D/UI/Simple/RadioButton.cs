using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A radio button control that ensures only one button in a group is selected at a time.
	/// </summary>
	[StyleTypeName("RadioButton")]
	public class RadioButton : CheckButtonBase
	{
		private string _text;

		/// <summary>
		/// Gets or sets the text content of this radio button.
		/// </summary>
		/// <remarks>This property is obsolete. Set Content to a Label instead.</remarks>
		[Obsolete("Set Content to Label instead")]
		[Browsable(false)]
		[XmlIgnore]
		[Category("Appearance")]
		public string Text
		{
			get => _text;
			set
			{
				if (_text == value)
				{
					return;
				}

				Content = new Label
				{
					Text = value
				};

				_text = value;
			}
		}

		/// <summary>
		/// Gets or sets whether this radio button is pressed. Only one radio button in a parent can be pressed at a time.
		/// </summary>
		public override bool IsPressed
		{
			get => base.IsPressed;

			set
			{
				if (IsPressed && Parent != null)
				{
					// If this is last pressed button
					// Don't allow it to be unpressed
					var allow = false;
					foreach (var child in Parent.ChildrenCopy)
					{
						var asRadio = child as RadioButton;
						if (asRadio == null || asRadio == this)
						{
							continue;
						}

						if (asRadio.IsPressed)
						{
							allow = true;
							break;
						}
					}

					if (!allow)
					{
						return;
					}
				}

				base.IsPressed = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the RadioButton class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public RadioButton(string styleName = Stylesheet.DefaultStyleName)
		{
			SetStyle(styleName);
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			if (Parent == null || !IsPressed)
			{
				return;
			}

			// Release other pressed radio buttons
			foreach (var child in Parent.ChildrenCopy)
			{
				var asRadio = child as RadioButton;

				if (asRadio == null || asRadio == this)
				{
					continue;
				}

				asRadio.IsPressed = false;
			}
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyCheckButtonStyle(stylesheet.RadioButtonStyles.SafelyGetStyle(name));
		}
	}
}