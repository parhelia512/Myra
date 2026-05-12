using Myra.Attributes;
using Myra.Graphics2D.UI.Styles;
using System;
using System.ComponentModel;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A checkbox control with customizable content.
	/// </summary>
	[StyleTypeName("CheckBox")]
	public class CheckButton : CheckButtonBase
	{
		/// <summary>
		/// Gets or sets a value indicating whether this checkbox is checked.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool IsChecked
		{
			get => IsPressed;
			set => IsPressed = value;
		}

		/// <summary>
		/// Raised when the checked state changes.
		/// </summary>
		public event EventHandler IsCheckedChanged
		{
			add
			{
				PressedChanged += value;
			}

			remove
			{
				PressedChanged -= value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the CheckButton class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public CheckButton(string styleName = Stylesheet.DefaultStyleName)
		{
			SetStyle(styleName);
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			base.InternalSetStyle(stylesheet, name);

			var style = stylesheet.CheckBoxStyles.SafelyGetStyle(name);
			ApplyCheckButtonStyle(style);
		}
	}
}
