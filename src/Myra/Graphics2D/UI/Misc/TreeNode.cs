using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using FontStashSharp;

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
	/// A node in a tree control that can contain child nodes. This class is obsolete; use TreeView instead.
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
		/// Gets or sets a value indicating whether this node is expanded to show child nodes.
		/// </summary>
		public bool IsExpanded
		{
			get { return _mark.IsPressed; }

			set { _mark.IsPressed = value; }
		}

		/// <summary>
		/// Gets the label widget that displays the node's text.
		/// </summary>
		public Label Label
		{
			get
			{
				return _label;
			}
		}

		/// <summary>
		/// Gets the expand/collapse toggle button for this node.
		/// </summary>
		public ToggleButton Mark
		{
			get { return _mark; }
		}

		/// <summary>
		/// Gets the grid that contains the child nodes.
		/// </summary>
		public Grid ChildNodesGrid
		{
			get { return _childNodesGrid; }
		}

		/// <summary>
		/// Gets or sets the text displayed for this node.
		/// </summary>
		public string Text
		{
			get { return _label.Text; }
			set { _label.Text = value; }
		}

		/// <summary>
		/// Gets or sets the font used to display the node's text.
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
		/// Gets the number of child nodes.
		/// </summary>
		public int ChildNodesCount
		{
			get { return _childNodesGrid.Widgets.Count; }
		}

		/// <summary>
		/// Gets the internal grid widget for this node.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public Grid Grid
		{
			get { return _layout.Child; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this node's row is visible in the tree view.
		/// </summary>
		internal bool RowVisible { get; set; }

		/// <summary>
		/// Gets or sets the parent node of this node.
		/// </summary>
		public TreeNode ParentNode { get; internal set; }

		/// <summary>
		/// Gets or sets the tree style applied to this node.
		/// </summary>
		public TreeStyle TreeStyle { get; private set; }

		/// <summary>
		/// Gets or sets the brush used to render the background when this node is selected.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionBackground
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the brush used to render the background when the mouse is over a selected node.
		/// </summary>
		[Category("Appearance")]
		public IBrush SelectionHoverBackground
		{
			get; set;
		}

		/// <summary>
		/// Initializes a new instance of the TreeNode class.
		/// </summary>
		/// <param name="topTree">The top-level tree control this node belongs to.</param>
		/// <param name="styleName">The name of the style to apply.</param>
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

		/// <summary>
		/// Handles the mark button press event.
		/// </summary>
		private void MarkOnUp(object sender, EventArgs args)
		{
			_childNodesGrid.Visible = false;
		}

		/// <summary>
		/// Updates the visibility of the expand/collapse mark based on whether child nodes exist.
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
		/// <param name="text">The text for the new child node.</param>
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
		/// Gets the child node at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the child node.</param>
		/// <returns>The child node at the specified index.</returns>
		public TreeNode GetSubNode(int index)
		{
			return (TreeNode)_childNodesGrid.Widgets[index];
		}

		/// <summary>
		/// Removes the specified child node.
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
		/// Removes the child node at the specified index.
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
		/// Applies a tree node style to this node.
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
		/// Sets the node's style using a stylesheet.
		/// </summary>
		protected override void InternalSetStyle(Stylesheet stylesheet, string name)
		{
			ApplyTreeNodeStyle(stylesheet.TreeStyles.SafelyGetStyle(name));
		}
	}
}