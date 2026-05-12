using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A checkbox control with text and image support.
	/// </summary>
	/// <remarks>This class is obsolete. Use CheckButton instead.</remarks>
	[Obsolete("Use CheckButton")]
	public class CheckBox : ImageTextButton
	{
		/// <summary>
		/// Gets or sets a value indicating whether this checkbox is toggleable. Always true for checkboxes.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		[DefaultValue(true)]
		public override bool Toggleable
		{
			get { return base.Toggleable; }
			set { base.Toggleable = value; }
		}

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
		/// Initializes a new instance of the CheckBox class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public CheckBox(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			Toggleable = true;
		}

		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyImageTextButtonStyle(stylesheet.CheckBoxStyles.SafelyGetStyle(name));
		}
	}
}
