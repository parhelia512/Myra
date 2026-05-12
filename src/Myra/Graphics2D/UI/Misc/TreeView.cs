using System;
using System.Collections.Generic;
using Myra.Graphics2D.UI.Styles;
using System.ComponentModel;
using System.Xml.Serialization;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Drawing;
using Myra.Platform;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A tree control that displays a hierarchical structure of expandable nodes with selection support.
	/// </summary>
	public class TreeView : Widget, ITreeViewNode
	{
		private readonly StackPanelLayout _layout = new StackPanelLayout(Orientation.Vertical);
		private readonly List<TreeViewNode> _allNodes = new List<TreeViewNode>();
		private TreeViewNode _selectedNode;
		private bool _rowInfosDirty = true;

		internal List<TreeViewNode> AllNodes => _allNodes;

		public int ChildNodesCount => Children.Count;

		public int TotalNodesCount => _allNodes.Count;

		internal TreeViewNode HoverRow { get; set; }

		/// <summary>
		/// Gets or sets the currently selected node. This property is obsolete, use SelectedNode instead.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		[Obsolete("Use SelectedNode")]
		public TreeViewNode SelectedRow
		{
			get => SelectedNode;

			set => SelectedNode = value;
		}

		/// <summary>
		/// Gets or sets the currently selected tree node.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public TreeViewNode SelectedNode
		{
			get
			{
				return _selectedNode;
			}

			set
			{
				if (value == _selectedNode)
				{
					return;
				}

				_selectedNode = value;

				var ev = SelectionChanged;
				if (ev != null)
				{
					ev(this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the brush used to draw the background of the selected node.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used to draw the background of the hovered node when it's not selected.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionHoverBackground { get; set; }

		/// <summary>
		/// Raised when the selected node changes.
		/// </summary>
		public event EventHandler SelectionChanged;

		/// <summary>
		/// Initializes a new instance of the TreeView class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		public TreeView(string styleName = Stylesheet.DefaultStyleName)
		{
			ChildrenLayout = _layout;
			AcceptsKeyboardFocus = true;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
			SetStyle(styleName);
		}

		/// <summary>
		/// Handles keyboard input for tree navigation (Up, Down, Enter keys).
		/// </summary>
		public override void OnKeyDown(Keys k)
		{
			base.OnKeyDown(k);

			if (SelectedNode == null)
			{
				return;
			}

			int index = 0;
			IList<Widget> parentWidgets = null;
			if (SelectedNode.ParentNode != null)
			{
				parentWidgets = SelectedNode.ParentNode.ChildNodesGrid.Widgets;
				index = parentWidgets.IndexOf(SelectedNode);
				if (index == -1)
				{
					return;
				}
			}

			switch (k)
			{
				case Keys.Enter:
					SelectedNode.IsExpanded = !SelectedNode.IsExpanded;
					break;
				case Keys.Up:
					{
						if (parentWidgets != null)
						{
							if (index == 0 && SelectedNode.ParentNode != null)
							{
								SelectedNode = SelectedNode.ParentNode;
							}
							else if (index > 0)
							{
								var previousRow = (TreeViewNode)parentWidgets[index - 1];
								if (!previousRow.IsExpanded || previousRow.ChildNodesCount == 0)
								{
									SelectedNode = previousRow;
								}
								else
								{
									SelectedNode = (TreeViewNode)previousRow.ChildNodesGrid.Widgets[previousRow.ChildNodesCount - 1];
								}
							}
						}
					}
					break;
				case Keys.Down:
					{
						if (SelectedNode.IsExpanded && SelectedNode.ChildNodesCount > 0)
						{
							SelectedNode = (TreeViewNode)SelectedNode.ChildNodesGrid.Widgets[0];
						}
						else if (parentWidgets != null && index + 1 < parentWidgets.Count)
						{
							SelectedNode = (TreeViewNode)parentWidgets[index + 1];
						}
						else if (parentWidgets != null && index + 1 >= parentWidgets.Count)
						{
							var parentOfParent = SelectedNode.ParentNode.ParentNode;
							if (parentOfParent != null)
							{
								var parentIndex = parentOfParent.ChildNodesGrid.Widgets.IndexOf(SelectedNode.ParentNode);
								if (parentIndex + 1 < parentOfParent.ChildNodesCount)
								{
									SelectedNode = (TreeViewNode)parentOfParent.ChildNodesGrid.Widgets[parentIndex + 1];
								}
							}
						}
					}
					break;
			}
		}

		/// <summary>
		/// Handles the touch down event.
		/// </summary>
		public override void OnTouchDown()
		{
			base.OnTouchDown();

			if (Desktop == null)
			{
				return;
			}

			SetHoverRow(Desktop.TouchPosition.Value);

			if (HoverRow != null && HoverRow.RowVisible)
			{
				SelectedNode = HoverRow;
			}
		}

		/// <summary>
		/// Handles the touch double click event to toggle node expansion.
		/// </summary>
		public override void OnTouchDoubleClick()
		{
			base.OnTouchDoubleClick();

			if (HoverRow != null)
			{
				if (!HoverRow.RowVisible)
				{
					return;
				}

				if (HoverRow.Mark.Visible && !HoverRow.Mark.IsTouchInside)
				{
					HoverRow.Mark.DoClick();
				}
			}
		}

		/// <summary>
		/// Builds the rectangle bounds for a node row.
		/// </summary>
		private Rectangle BuildRowRect(TreeViewNode rowInfo)
		{
			var rowPos = ToLocal(rowInfo.ToGlobal(rowInfo.ActualBounds.Location));

			return new Rectangle(ActualBounds.Left, rowPos.Y, ActualBounds.Width, rowInfo.ContentHeight);
		}

		/// <summary>
		/// Sets the hover row based on the position.
		/// </summary>
		private void SetHoverRow(Point position)
		{
			if (!ContainsGlobalPoint(position))
			{
				return;
			}

			position = ToLocal(position);
			foreach (var rowInfo in _allNodes)
			{
				if (rowInfo.RowVisible)
				{
					var rect = BuildRowRect(rowInfo);
					if (rect.Contains(position))
					{
						HoverRow = rowInfo;
						return;
					}
				}
			}
		}

		/// <summary>
		/// Handles the mouse moved event.
		/// </summary>
		public override void OnMouseMoved()
		{
			base.OnMouseMoved();

			HoverRow = null;

			if (Desktop == null)
			{
				return;
			}

			SetHoverRow(Desktop.MousePosition);
		}

		/// <summary>
		/// Handles the mouse left event.
		/// </summary>
		public override void OnMouseLeft()
		{
			base.OnMouseLeft();

			HoverRow = null;
		}

		/// <summary>
		/// Adds a new child node with the specified content.
		/// </summary>
		/// <param name="content">The content widget for the new node.</param>
		/// <returns>The newly created tree view node.</returns>
		public TreeViewNode AddSubNode(Widget content)
		{
			var result = new TreeViewNode(this, StyleName)
			{
				Content = content
			};

			Grid.SetRow(result, Children.Count);

			Children.Add(result);

			return result;
		}

		/// <summary>
		/// Gets the child node at the specified index.
		/// </summary>
		/// <param name="index">The index of the child node.</param>
		/// <returns>The child node at the index.</returns>
		public TreeViewNode GetSubNode(int index) => (TreeViewNode)Children[index];

		/// <summary>
		/// Gets the node by its absolute index in the all nodes list.
		/// </summary>
		/// <param name="index">The absolute index of the node.</param>
		/// <returns>The node at the absolute index.</returns>
		public TreeViewNode GetNodeByAbsoluteIndex(int index) => _allNodes[index];

		/// <summary>
		/// Removes all child nodes from the tree.
		/// </summary>
		public void RemoveAllSubNodes()
		{
			HoverRow = SelectedNode = null;
			Children.Clear();
			_allNodes.Clear();
		}

		/// <summary>
		/// Recursively iterates through all nodes using the provided action.
		/// </summary>
		private bool Iterate(TreeViewNode node, Func<TreeViewNode, bool> action)
		{
			if (!action(node))
			{
				return false;
			}

			foreach (var widget in node.ChildNodesGrid.ChildrenCopy)
			{
				var subNode = (TreeViewNode)widget;
				if (!Iterate(subNode, action))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Iterates through all nodes
		/// </summary>
		/// <param name="action">Called for each node, returning false breaks iteration</param>
		public void Iterate(Func<TreeViewNode, bool> action)
		{
			foreach (TreeViewNode node in ChildrenCopy)
			{
				Iterate(node, action);
			}
		}

		/// <summary>
		/// Recursively updates the visibility of all nodes based on expansion state.
		/// </summary>
		private static void RecursiveUpdateRowVisibility(TreeViewNode tree)
		{
			tree.RowVisible = true;

			if (tree.IsExpanded)
			{
				foreach (var widget in tree.ChildNodesGrid.ChildrenCopy)
				{
					var TreeViewNode = (TreeViewNode)widget;
					RecursiveUpdateRowVisibility(TreeViewNode);
				}
			}
		}

		/// <summary>
		/// Updates the visibility of all rows based on node expansion states.
		/// </summary>
		private void UpdateRowInfos()
		{
			foreach (var rowInfo in _allNodes)
			{
				rowInfo.RowVisible = false;
			}

			foreach (TreeViewNode node in ChildrenCopy)
			{
				RecursiveUpdateRowVisibility(node);
			}
		}

		/// <summary>
		/// Handles internal arrangement of the tree.
		/// </summary>
		protected override void InternalArrange()
		{
			base.InternalArrange();
			_rowInfosDirty = true;
		}

		/// <summary>
		/// Renders the tree with selection backgrounds and child nodes.
		/// </summary>
		public override void InternalRender(RenderContext context)
		{
			if (_rowInfosDirty)
			{
				UpdateRowInfos();
				_rowInfosDirty = false;
			}
			if (SelectionBackground != null)
			{
				if (HoverRow != null && HoverRow != SelectedNode && SelectionHoverBackground != null)
				{
					var rect = BuildRowRect(HoverRow);
					SelectionHoverBackground.Draw(context, rect);
				}

				if (SelectedNode != null && SelectedNode.RowVisible)
				{
					var rect = BuildRowRect(SelectedNode);
					SelectionBackground.Draw(context, rect);
				}
			}
			else
			{
				if (HoverRow != null && SelectionHoverBackground != null)
				{
					var rect = BuildRowRect(HoverRow);
					SelectionHoverBackground.Draw(context, rect);
				}
			}

			base.InternalRender(context);
		}

		/// <summary>
		/// Recursively finds the path to a node.
		/// </summary>
		private static bool FindPath(Stack<TreeViewNode> path, TreeViewNode node)
		{
			var top = path.Peek();

			for (var i = 0; i < top.ChildNodesCount; ++i)
			{
				var child = top.GetSubNode(i);

				if (child == node)
				{
					return true;
				}

				path.Push(child);

				if (FindPath(path, node))
				{
					return true;
				}

				path.Pop();
			}

			return false;
		}

		/// <summary>
		/// Expands the path to the specified node by expanding all parent nodes.
		/// </summary>
		/// <param name="node">The node to expand the path to.</param>
		public void ExpandPath(TreeViewNode node)
		{
			var path = new Stack<TreeViewNode>();

			foreach (TreeViewNode childNode in Children)
			{
				path.Push(childNode);
			}

			if (!FindPath(path, node))
			{
				// Path not found
				return;
			}

			while (path.Count > 0)
			{
				var p = path.Pop();
				p.IsExpanded = true;
			}
		}

		/// <summary>
		/// Sets the tree view's style using a stylesheet.
		/// </summary>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			base.InternalSetStyle(stylesheet, name);
			ApplyTreeViewStyle(stylesheet.TreeStyles.SafelyGetStyle(name));
		}

		/// <summary>
		/// Applies a tree view style to this tree view.
		/// </summary>
		/// <param name="style">The tree view style to apply.</param>
		public void ApplyTreeViewStyle(TreeStyle style)
		{
			ApplyWidgetStyle(style);

			SelectionBackground = style.SelectionBackground;
			SelectionHoverBackground = style.SelectionHoverBackground;
		}

		/// <summary>
		/// Finds the first node that matches the specified predicate.
		/// </summary>
		/// <param name="predicate">The condition to search for.</param>
		/// <returns>The first matching node, or null if no match is found.</returns>
		public TreeViewNode FindNode(Func<TreeViewNode, bool> predicate)
		{
			foreach (var node in AllNodes)
			{
				if (predicate(node))
				{
					return node;
				}
			}

			return null;
		}

		/// <summary>
		/// Copies the style properties from another tree view.
		/// </summary>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var treeView = (TreeView)w;
			SelectionBackground = treeView.SelectionBackground;
			SelectionHoverBackground = treeView.SelectionHoverBackground;

			foreach (TreeViewNode node in treeView.Children)
			{
				AddSubNode(node.Content);
			}
		}
	}
}