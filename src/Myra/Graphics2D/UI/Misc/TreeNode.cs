using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using FontStashSharp;
using Myra.Events;


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
	/// An obsolete tree node widget for displaying hierarchical data. Use TreeView and TreeViewNode instead.
	/// </summary>
	[Obsolete("Use TreeView")]
	public class TreeNode : Widget
	{
		private readonly SingleItemLayout<Grid> _layout;
		private readonly Tree _topTree;
		private readonly Grid _childNodesGrid;
		private readonly ToggleButton _mark;
		private readonly Label _label;

		/// <summary>
		/// Gets or sets a value indicating whether the node is expanded to show its child nodes.
		/// </summary>
		public bool IsExpanded
		{
			get { return _mark.IsPressed; }

			set { _mark.IsPressed = value; }
		}

		/// <summary>
		/// Gets the label widget displaying the node's text.
		/// </summary>
		public Label Label
		{
			get
			{
				return _label;
			}
		}

		/// <summary>
		/// Gets the toggle button used to expand or collapse the node.
		/// </summary>
		public ToggleButton Mark
		{
			get { return _mark; }
		}

		/// <summary>
		/// Gets the grid containing the child nodes.
		/// </summary>
		public Grid ChildNodesGrid
		{
			get { return _childNodesGrid; }
		}

		/// <summary>
		/// Gets or sets the text displayed in the node's label.
		/// </summary>
		public string Text
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

		/// <summary>
		/// Gets or sets the font used to render the node's text.
		/// </summary>
		public SpriteFontBase Font
		{
			get { return _label.Font; }
			set { _label.Font = value; }
		}

		/// <summary>
		/// Gets or sets the color of the node's text.
		/// </summary>
		public Color TextColor
		{
			get { return _label.TextColor; }
			set { _label.TextColor = value; }
		}

		/// <summary>
		/// Gets the number of child nodes under this node.
		/// </summary>
		public int ChildNodesCount
		{
			get { return _childNodesGrid.Widgets.Count; }
		}

		/// <summary>
		/// Gets the grid layout containing this node's content and child nodes.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public Grid Grid
		{
			get { return _layout.Child; }
		}

		internal bool RowVisible { get; set; }

		/// <summary>
		/// Gets or sets the parent node of this node in the hierarchy.
		/// </summary>
		public TreeNode ParentNode { get; internal set; }

		/// <summary>
		/// Gets the tree style applied to this node.
		/// </summary>
		public TreeStyle TreeStyle { get; private set; }

		/// <summary>
		/// Gets or sets the brush used to draw the background of this node when selected.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionBackground
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the brush used to draw the background of this node when hovered.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionHoverBackground
		{
			get; set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeNode"/> class.
		/// </summary>
		/// <param name="topTree">The parent tree control. Can be null for standalone nodes.</param>
		/// <param name="styleName">The name of the style to apply. Defaults to the default stylesheet style.</param>
		public TreeNode(Tree topTree, string styleName = Stylesheet.DefaultStyleName)
		{
			_layout = new SingleItemLayout<Grid>(this)
			{
				Child = new Grid
				{
					ColumnSpacing = 2,
					RowSpacing = 2,
				}
			};
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
				_childNodesGrid.Visible = _mark.IsPressed;
			};

			Grid.Widgets.Add(_mark);

			_label = new Label();
			Grid.SetColumn(_label, 1);

			Grid.Widgets.Add(_label);

			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;

			Grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
			Grid.ColumnsProportions.Add(new Proportion(ProportionType.Fill));

			Grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
			Grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

			// Second is yet another grid holding child nodes
			_childNodesGrid = new Grid()
			{
				Visible = false,
			};
			Grid.SetColumn(_childNodesGrid, 1);
			Grid.SetRow(_childNodesGrid, 1);

			Grid.Widgets.Add(_childNodesGrid);

			SetStyle(styleName);

			UpdateMark();
		}

		private void MarkOnUp(object sender, MyraEventArgs args)
		{
			_childNodesGrid.Visible = false;
		}

		/// <summary>
		/// Updates the visibility of the mark (expand/collapse button) based on the number of child nodes.
		/// </summary>
		protected virtual void UpdateMark()
		{
			_mark.Visible = _childNodesGrid.Widgets.Count > 0;
		}

		/// <summary>
		/// Removes all child nodes from this node.
		/// </summary>
		public virtual void RemoveAllSubNodes()
		{
			_childNodesGrid.Widgets.Clear();
			_childNodesGrid.RowsProportions.Clear();

			UpdateMark();
		}

		/// <summary>
		/// Adds a new child node with the specified text.
		/// </summary>
		/// <param name="text">The text to display in the child node.</param>
		/// <returns>The newly created child node.</returns>
		public TreeNode AddSubNode(string text)
		{
			var result = new TreeNode(_topTree ?? (Tree)this, StyleName)
			{
				Text = text,
				ParentNode = this
			};
			Grid.SetRow(result, _childNodesGrid.Widgets.Count);

			_childNodesGrid.Widgets.Add(result);
			_childNodesGrid.RowsProportions.Add(new Proportion(ProportionType.Auto));

			UpdateMark();

			return result;
		}

		/// <summary>
		/// Gets a child node by its index.
		/// </summary>
		/// <param name="index">The zero-based index of the child node.</param>
		/// <returns>The child node at the specified index.</returns>
		public TreeNode GetSubNode(int index)
		{
			return (TreeNode)_childNodesGrid.Widgets[index];
		}

		/// <summary>
		/// Removes a specific child node and clears related state in the tree.
		/// </summary>
		/// <param name="subNode">The child node to remove.</param>
		public void RemoveSubNode(TreeNode subNode)
		{
			_childNodesGrid.Widgets.Remove(subNode);
			if (_topTree != null && _topTree.SelectedRow == subNode)
			{
				_topTree.SelectedRow = null;
			}
		}

		/// <summary>
		/// Removes the child node at the specified index and clears related state in the tree.
		/// </summary>
		/// <param name="index">The zero-based index of the child node to remove.</param>
		public void RemoveSubNodeAt(int index)
		{
			var subNode = _childNodesGrid.Widgets[index];
			_childNodesGrid.Widgets.RemoveAt(index);
			if (_topTree.SelectedRow == subNode)
			{
				_topTree.SelectedRow = null;
			}
		}

		/// <summary>
		/// Applies the specified tree node style to this node and its components.
		/// </summary>
		/// <param name="style">The tree style to apply.</param>
		public void ApplyTreeNodeStyle(TreeStyle style)
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


				_label.ApplyLabelStyle(style.LabelStyle);
			}

			TreeStyle = style;

			SelectionBackground = style.SelectionBackground;
			SelectionHoverBackground = style.SelectionHoverBackground;
		}

		/// <summary>
		/// Applies the style with the specified name from the stylesheet to this tree node.
		/// </summary>
		/// <param name="stylesheet">The stylesheet containing the style to apply.</param>
		/// <param name="name">The name of the tree style to apply.</param>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyTreeNodeStyle(stylesheet.TreeStyles.SafelyGetStyle(name));
		}
	}
}