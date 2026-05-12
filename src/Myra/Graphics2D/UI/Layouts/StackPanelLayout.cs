using System.Collections.ObjectModel;
using System.Collections.Generic;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
#elif STRIDE
using Stride.Core.Mathematics;
#else
using System.Drawing;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// A layout engine that arranges widgets in a single row or column.
	/// </summary>
	public class StackPanelLayout : ILayout
	{
		private readonly GridLayout _layout = new GridLayout();
		private int _spacing;

		/// <summary>
		/// Gets the orientation of the stack panel (horizontal or vertical).
		/// </summary>
		public Orientation Orientation { get; private set; }

		/// <summary>
		/// Gets or sets the spacing between widgets in the stack panel.
		/// </summary>
		public int Spacing
		{
			get => _spacing;
			set
			{
				_spacing = value;
				if (Orientation == Orientation.Horizontal)
				{
					_layout.ColumnSpacing = value;
				}
				else
				{
					_layout.RowSpacing = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the default proportion for widgets in the stack panel.
		/// </summary>
		public Proportion DefaultProportion
		{
			get => Orientation == Orientation.Horizontal ? _layout.DefaultColumnProportion : _layout.DefaultRowProportion;
			set
			{
				if (Orientation == Orientation.Horizontal)
				{
					_layout.DefaultColumnProportion = value;
				}
				else
				{
					_layout.DefaultRowProportion = value;
				}
			}
		}

		/// <summary>
		/// Gets the proportions for all widgets in the stack panel.
		/// </summary>
		public ObservableCollection<Proportion> Proportions
		{
			get => Orientation == Orientation.Horizontal ? _layout.ColumnsProportions : _layout.RowsProportions;
		}

		/// <summary>
		/// Gets the X coordinates of the grid lines (for internal use).
		/// </summary>
		public List<int> GridLinesX => _layout.GridLinesX;

		/// <summary>
		/// Gets the Y coordinates of the grid lines (for internal use).
		/// </summary>
		public List<int> GridLinesY => _layout.GridLinesY;

		/// <summary>
		/// Initializes a new instance of the StackPanelLayout class.
		/// </summary>
		/// <param name="orientation">The orientation of the stack panel.</param>
		public StackPanelLayout(Orientation orientation)
		{
			Orientation = orientation;
			DefaultProportion = Proportion.StackPanelDefault;
		}

		/// <summary>
		/// Updates widget grid positions and proportions based on orientation.
		/// </summary>
		private void UpdateWidgets(IEnumerable<Widget> widgets)
		{
			var index = 0;
			Proportions.Clear();
			foreach (var widget in widgets)
			{
				if (Orientation == Orientation.Horizontal)
				{
					Grid.SetColumn(widget, index);
				}
				else
				{
					Grid.SetRow(widget, index);
				}

				Proportions.Add(new Proportion(StackPanel.GetProportionType(widget), StackPanel.GetProportionValue(widget)));

				++index;
			}
		}

		/// <summary>
		/// Measures the required size for all widgets.
		/// </summary>
		/// <param name="widgets">The widgets to measure.</param>
		/// <param name="availableSize">The available size for the widgets.</param>
		/// <returns>The required size to fit all widgets.</returns>
		public Point Measure(IEnumerable<Widget> widgets, Point availableSize)
		{
			UpdateWidgets(widgets);
			return _layout.Measure(widgets, availableSize);
		}

		/// <summary>
		/// Arranges all widgets within the specified bounds.
		/// </summary>
		/// <param name="widgets">The widgets to arrange.</param>
		/// <param name="bounds">The bounds in which to arrange the widgets.</param>
		public void Arrange(IEnumerable<Widget> widgets, Rectangle bounds)
		{
			UpdateWidgets(widgets);
			_layout.Arrange(widgets, bounds);
		}

		/// <summary>
		/// Gets the size of the cell at the specified index.
		/// </summary>
		/// <param name="index">The index of the cell.</param>
		/// <returns>The size of the cell.</returns>
		public int GetCellSize(int index)
		{
			return Orientation == Orientation.Horizontal ? _layout.GetColumnWidth(index) : _layout.GetRowHeight(index);
		}
	}
}