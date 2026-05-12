namespace Myra.MML
{
	/// <summary>
	/// Represents an object that can have a unique identifier.
	/// </summary>
	public interface IItemWithId
	{
		/// <summary>
		/// Gets or sets the unique identifier for this object.
		/// </summary>
		string Id { get; set; }
	}
}
