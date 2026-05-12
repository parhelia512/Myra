using Myra.MML;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Provides an interface for items that can be contained in a menu.
	/// </summary>
	public interface IMenuItem: IItemWithId
	{
		/// <summary>
		/// Gets or sets the parent menu for this menu item.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		Menu Menu { get; set; }

		/// <summary>
		/// Gets the underscore character from the menu item's text (used for keyboard shortcuts).
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		char? UnderscoreChar { get; }

		/// <summary>
		/// Gets or sets the index of this menu item within its parent menu.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		int Index { get; set; }
	}
}
