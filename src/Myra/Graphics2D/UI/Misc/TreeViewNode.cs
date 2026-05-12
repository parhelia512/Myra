using System;
using Myra.Graphics2D.UI.Styles;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A node within a tree view that can contain child nodes and arbitrary content.
	/// </summary>
	public class TreeViewNode : ContentControl, ITreeViewNode
	{
		private readonly GridLayout _layout = new GridLayout();
		private readonly TreeView _topTree;
		private readonly VerticalStackPanel _childNodesStackPanel;
		private readonly ToggleButton _mark;
		private Widget _content;

		/// <summary>
		/// Gets or sets a value indicating whether this node is expanded to show child nodes.
		/// </summary>
		public bool IsExpanded
		{
			get { return _mark.IsPressed; }

			set { _mark.IsPressed = value; }
		}

		/// <summary>
		/// Gets the toggle button used to expand/collapse this node.
		/// </summary>
		public ToggleButton Mark => _mark;

		/// <summary>
		/// Gets the grid that contains the child nodes.
		/// </summary>
		internal VerticalStackPanel ChildNodesGrid => _childNodesStackPanel;

		/// <summary>
		/// Gets the height of the content row.
		/// </summary>
		public int ContentHeight => _layout.GetRowHeight(0);

		/// <summary>
		/// Gets the number of child nodes.
		/// </summary>
		public int ChildNodesCount => _childNodesStackPanel.Children.Count;

		/// <summary>
		/// Gets or sets a value indicating whether this node is visible in the tree view.
		/// </summary>
		internal bool RowVisible { get; set; }

		/// <summary>
		/// Gets or sets the parent node of this node.
		/// </summary>
		public TreeViewNode ParentNode { get; internal set; }

		/// <summary>
		/// Gets or sets the content widget displayed in this node.
		/// </summary>
		public override Widget Content
		{
			get => _content;

			set
			{
				if (_content == value)
				{
					return;
				}

				if (_content != null)
				{
					Children.Remove(_content);
				}

				_content = value;

				if (_content != null)
				{
					Grid.SetColumn(_content, 1);
					Children.Add(_content);
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the TreeViewNode class.
		/// </summary>
		/// <param name="topTree">The root tree view containing this node.</param>
		/// <param name="styleName">The name of the style to apply.</param>
		internal TreeViewNode(TreeView topTree, string styleName = Stylesheet.DefaultStyleName)
		{
			_layout.ColumnSpacing = 2;
			_layout.RowSpacing = 2;
			ChildrenLayout = _layout;

			_topTree = topTree;

			if (_topTree != null)
			{
				_topTree.AllNodes.Add(this);
			}

			_mark = new ToggleButton(null)
			{
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
				Content = new Image()
			};

			_mark.PressedChanged += (s, a) =>
			{
				_childNodesStackPanel.Visible = _mark.IsPressed;
			};

			Children.Add(_mark);

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			_layout.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			_layout.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			_layout.RowsProportions.Add(new Proportion(ProportionType.Auto));
			_layout.RowsProportions.Add(new Proportion(ProportionType.Auto));

			// Second is yet another grid holding child nodes
			_childNodesStackPanel = new VerticalStackPanel
			{
				Visible = false,
			};
			Grid.SetColumn(_childNodesStackPanel, 1);
			Grid.SetRow(_childNodesStackPanel, 1);

			Children.Add(_childNodesStackPanel);

			SetStyle(styleName);

			UpdateMark();
		}

		/// <summary>
		/// Handles the mark button up event.
		/// </summary>
		private void MarkOnUp(object sender, EventArgs args)
		{
			_childNodesStackPanel.Visible = false;
		}

		/// <summary>
		/// Updates the visibility of the expansion mark based on whether there are child nodes.
		/// </summary>
		protected virtual void UpdateMark()
		{
			_mark.Visible = _childNodesStackPanel.Children.Count > 0;
		}

		/// <summary>
		/// Removes all child nodes.
		/// </summary>
		public virtual void RemoveAllSubNodes()
		{
			_childNodesStackPanel.Children.Clear();
			UpdateMark();
		}

		/// <summary>
		/// Adds a new child node with the specified content.
		/// </summary>
		/// <param name="content">The content widget for the new node.</param>
		/// <returns>The newly created child node.</returns>
		public TreeViewNode AddSubNode(Widget content)
		{
			var result = new TreeViewNode(_topTree, StyleName)
			{
				ParentNode = this,
				Content = content
			};
			Grid.SetRow(result, _childNodesStackPanel.Children.Count);

			_childNodesStackPanel.Children.Add(result);

			UpdateMark();

			return result;
		}

		/// <summary>
		/// Gets the child node at the specified index.
		/// </summary>
		/// <param name="index">The index of the child node.</param>
		/// <returns>The child node at the index.</returns>
		public TreeViewNode GetSubNode(int index)
		{
			return (TreeViewNode)_childNodesStackPanel.Children[index];
		}

		/// <summary>
		/// Removes the specified child node.
		/// </summary>
		/// <param name="subNode">The child node to remove.</param>
		public void RemoveSubNode(TreeViewNode subNode)
		{
			_childNodesStackPanel.Children.Remove(subNode);
			_topTree.AllNodes.Remove(subNode);
			if (_topTree != null)
			{
				if (_topTree.SelectedNode == subNode)
				{
					_topTree.SelectedNode = null;
				}

				if (_topTree.HoverRow == subNode)
				{
					_topTree.HoverRow = null;
				}
			}
		}

		/// <summary>
		/// Removes the child node at the specified index.
		/// </summary>
		/// <param name="index">The index of the child node to remove.</param>
		public void RemoveSubNodeAt(int index)
		{
			var subNode = (TreeViewNode)_childNodesStackPanel.Children[index];
			_childNodesStackPanel.Children.RemoveAt(index);
			_topTree.AllNodes.Remove(subNode);
			if (_topTree.SelectedNode == subNode)
			{
				_topTree.SelectedNode = null;
			}
		}

		/// <summary>
		/// Applies a tree view node style to this node.
		/// </summary>
		/// <param name="style">The tree view style to apply.</param>
		public void ApplyTreeViewNodeStyle(TreeStyle style)
		{
			ApplyWidgetStyle(style);

			if (style.MarkStyle != null)
			{
				_mark.ApplyButtonStyle(style.MarkStyle);
				if (style.MarkStyle.ImageStyle != null)
				{
					var image = (Image)_mark.Content;
					image.ApplyPressableImageStyle(style.MarkStyle.ImageStyle);
				}
			}
		}

		/// <summary>
		/// Sets the node's style using a stylesheet.
		/// </summary>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyTreeViewNodeStyle(stylesheet.TreeStyles.SafelyGetStyle(name));
		}
	}
}