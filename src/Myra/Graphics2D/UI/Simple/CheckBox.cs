using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Graphics2D.UI.Styles;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A checkbox control that can be checked or unchecked.
	/// </summary>
	/// <remarks>This class is obsolete. Use <see cref="CheckButton"/> instead.</remarks>
	[Obsolete("Use CheckButton")]
	public class CheckBox : ImageTextButton
	{
		/// <summary>
		/// Gets or sets a value indicating whether the checkbox is toggleable (always true for checkboxes).
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
		/// Gets or sets a value indicating whether the checkbox is checked.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		public bool IsChecked
		{
			get => IsPressed;
			set => IsPressed = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CheckBox"/> class with the specified style.
		/// </summary>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public CheckBox(string styleName = Stylesheet.DefaultStyleName) : base(styleName)
		{
			Toggleable = true;
		}

		/// <summary>
		/// Applies a named checkbox style from the stylesheet to the checkbox.
		/// </summary>
		/// <param name="stylesheet">The stylesheet containing the style.</param>
		/// <param name="name">The name of the checkbox style to apply.</param>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyImageTextButtonStyle(stylesheet.CheckBoxStyles.SafelyGetStyle(name));
		}
	}
}
