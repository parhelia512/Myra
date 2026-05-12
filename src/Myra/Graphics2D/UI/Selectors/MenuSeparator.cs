using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Represents a separator line in a menu.
	/// </summary>
	public class MenuSeparator : IMenuItem
	{
		internal SeparatorWidget Separator;

		/// <summary>
		/// Gets or sets the ID of this menu separator.
		/// </summary>
		[DefaultValue(null)]
		[Browsable(false)]
		[XmlIgnore]
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the parent menu for this separator.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Menu Menu { get; set; }

		/// <summary>
		/// Gets the underscore character from this separator (always null).
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public char? UnderscoreChar { get { return null; } }

		/// <summary>
		/// Gets or sets the index of this separator within its parent menu.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int Index { get; set; }
	}
}
