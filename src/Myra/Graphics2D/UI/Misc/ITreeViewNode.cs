namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Provides an interface for tree view nodes that can contain child nodes.
	/// </summary>
	public interface ITreeViewNode
	{
		/// <summary>
		/// Gets the number of child nodes.
		/// </summary>
		int ChildNodesCount { get; }

		/// <summary>
		/// Adds a child node with the specified content.
		/// </summary>
		/// <param name="content">The content widget for the new child node.</param>
		/// <returns>The newly created child node.</returns>
		TreeViewNode AddSubNode(Widget content);

		/// <summary>
		/// Gets the child node at the specified index.
		/// </summary>
		/// <param name="index">The index of the child node.</param>
		/// <returns>The child node at the specified index.</returns>
		TreeViewNode GetSubNode(int index);

		/// <summary>
		/// Removes all child nodes.
		/// </summary>
		void RemoveAllSubNodes();
	}
}
