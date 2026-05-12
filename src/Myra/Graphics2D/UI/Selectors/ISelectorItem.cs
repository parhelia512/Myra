namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Provides an interface for items that can be selected in a selector control.
	/// </summary>
	public interface ISelectorItem
	{
		/// <summary>
		/// Gets or sets a value indicating whether this item is selected.
		/// </summary>
		bool IsSelected
		{
			get; set;
		}
	}
}
