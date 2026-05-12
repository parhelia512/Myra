using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Myra.Attributes;

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// An abstract base class for containers that split their area between multiple child widgets with movable splitter handles.
	/// </summary>
	public abstract class SplitPane : Container
	{
		private readonly ObservableCollection<Widget> _widgets = new ObservableCollection<Widget>();
		private readonly GridLayout _layout = new GridLayout();
		private readonly List<Button> _handles = new List<Button>();
		private Button _handleDown;
		private int? _mouseCoord;
		private int _handlesSize;

		/// <summary>
		/// Gets the collection of child widgets that will be split.
		/// </summary>
		[Content]
		[Browsable(false)]
		public override IList<Widget> Widgets => _widgets;

		/// <summary>
		/// When overridden in a derived class, gets the orientation of this split pane.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public abstract Orientation Orientation { get; }

		/// <summary>
		/// Gets or sets the style for the splitter handles.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public SplitPanelButtonStyle HandleStyle { get; private set; }

		/// <summary>
		/// Raised when the splitter positions change.
		/// </summary>
		public event EventHandler ProportionsChanged;

		/// <summary>
		/// Initializes a new instance of the SplitPane class.
		/// </summary>
		/// <param name="styleName">The name of the style to apply.</param>
		protected SplitPane(string styleName)
		{
			ChildrenLayout = _layout;
			_widgets.CollectionChanged += WidgetsOnCollectionChanged;

			SetStyle(styleName);
		}

		/// <summary>
		/// Gets the proportion (relative size) of the widget at the specified index.
		/// </summary>
		/// <param name="widgetIndex">The index of the widget.</param>
		/// <returns>The proportion value of the widget.</returns>
		public float GetProportion(int widgetIndex)
		{
			if (widgetIndex < 0 || widgetIndex >= Widgets.Count)
			{
				return 0.0f;
			}

			var result = Orientation == Orientation.Horizontal
				? _layout.ColumnsProportions[widgetIndex * 2].Value
				: _layout.RowsProportions[widgetIndex * 2].Value;

			return result;
		}

		/// <summary>
		/// Handles the touch moved event for moving splitter handles.
		/// </summary>
		public override void OnTouchMoved()
		{
			base.OnTouchMoved();

			if (Desktop == null || _mouseCoord == null)
			{
				return;
			}

			var bounds = Bounds;
			if (bounds.Width == 0)
			{
				return;
			}

			var handleIndex = Children.IndexOf(_handleDown);
			Proportion firstProportion, secondProportion;
			float fp;

			var position = ToLocal(Desktop.TouchPosition.Value);
			if (Orientation == Orientation.Horizontal)
			{
				var firstWidth = position.X - _mouseCoord.Value;

				for (var i = 0; i < handleIndex - 1; ++i)
				{
					firstWidth -= _layout.GetColumnWidth(i);
				}

				fp = (float)Widgets.Count * firstWidth / (bounds.Width - _handlesSize);

				firstProportion = _layout.ColumnsProportions[handleIndex - 1];
				secondProportion = _layout.ColumnsProportions[handleIndex + 1];
			}
			else
			{
				var firstHeight = position.Y - _mouseCoord.Value;

				for (var i = 0; i < handleIndex - 1; ++i)
				{
					firstHeight -= _layout.GetRowHeight(i);
				}

				fp = (float)Widgets.Count * firstHeight / (bounds.Height - _handlesSize);

				firstProportion = _layout.RowsProportions[handleIndex - 1];
				secondProportion = _layout.RowsProportions[handleIndex + 1];
			}

			var fp2 = firstProportion.Value + secondProportion.Value - fp;
			if (fp >= 0 && fp2 >= 0)
			{
				firstProportion.Value = fp;
				secondProportion.Value = fp2;
				FireProportionsChanged();
			}

			InvalidateArrange();
		}

		/// <summary>
		/// Raises the ProportionsChanged event.
		/// </summary>
		private void FireProportionsChanged()
		{
			var ev = ProportionsChanged;
			if (ev != null)
			{
				ev(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Handles the pressed state change of a splitter handle.
		/// </summary>
		private void HandleOnPressedChanged(object sender, EventArgs args)
		{
			var handle = (Button)sender;

			if (!handle.IsPressed)
			{
				_handleDown = null;
				_mouseCoord = null;

				MyraEnvironment.SetMouseCursorFromWidget = true;
				MyraEnvironment.MouseCursorType = MouseCursor ?? MyraEnvironment.DefaultMouseCursorType;
			}
			else if (Desktop != null)
			{
				_handleDown = (Button)sender;

				var handleGlobalPos = _handleDown.ToGlobal(_handleDown.Bounds.Location);
				_mouseCoord = Orientation == Orientation.Horizontal
					? Desktop.TouchPosition.Value.X - handleGlobalPos.X
					: Desktop.TouchPosition.Value.Y - handleGlobalPos.Y;

				MyraEnvironment.SetMouseCursorFromWidget = false;
				if (Orientation == Orientation.Horizontal)
				{
					MyraEnvironment.MouseCursorType = MouseCursorType.SizeWE;
				}
				else
				{
					MyraEnvironment.MouseCursorType = MouseCursorType.SizeNS;
				}
			}
		}

		/// <summary>
		/// Handles changes to the widgets collection.
		/// </summary>
		private void WidgetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			Reset();
		}

		/// <summary>
		/// Gets the proportion values for widgets on either side of a splitter.
		/// </summary>
		private void GetProportions(int leftWidgetIndex,
			out Proportion leftProportion,
			out Proportion rightProportion,
			out float total)
		{
			var allProps = Orientation == Orientation.Horizontal
				? _layout.ColumnsProportions : _layout.RowsProportions;

			total = 0;
			for(var i = 0; i < allProps.Count; i += 2)
			{
				total += allProps[i].Value;
			}

			var baseIndex = leftWidgetIndex * 2;
			leftProportion = allProps[baseIndex];
			rightProportion = allProps[baseIndex + 2];
		}

		/// <summary>
		/// Gets the position of the splitter between two widgets.
		/// </summary>
		/// <param name="leftWidgetIndex">The index of the widget on the left side of the splitter.</param>
		/// <returns>The splitter position as a proportion (0 to 1).</returns>
		public float GetSplitterPosition(int leftWidgetIndex)
		{
			float total;
			Proportion leftProportion, rightProportion;
			GetProportions(leftWidgetIndex, out leftProportion, out rightProportion, out total);

			return leftProportion.Value / total;
		}

		/// <summary>
		/// Sets the position of the splitter between two widgets.
		/// </summary>
		/// <param name="leftWidgetIndex">The index of the widget on the left side of the splitter.</param>
		/// <param name="proportion">The new splitter position as a proportion (0 to 1).</param>
		public void SetSplitterPosition(int leftWidgetIndex, float proportion)
		{
			float total;
			Proportion leftProportion, rightProportion;
			GetProportions(leftWidgetIndex, out leftProportion, out rightProportion, out total);

			var fp = proportion * total;
			var fp2 = leftProportion.Value + rightProportion.Value - fp;
			leftProportion.Value = fp;
			rightProportion.Value = fp2;
		}

		/// <summary>
		/// Recreates the splitter layout based on the current widgets collection.
		/// </summary>
		public void Reset()
		{
			// Clear
			Children.Clear();
			_handles.Clear();
			_handlesSize = 0;

			_layout.ColumnsProportions.Clear();
			_layout.RowsProportions.Clear();

			var i = 0;

			var handleSize = 0;

			if (HandleStyle.HandleSize != null)
			{
				handleSize = HandleStyle.HandleSize.Value;
			}
			else
			{
				var asImage = HandleStyle.Background as IImage;
				if (asImage != null)
				{
					handleSize = Orientation == Orientation.Horizontal
						? asImage.Size.X
						: asImage.Size.Y;
				}
			}

			foreach (var w in Widgets)
			{
				Proportion proportion;
				if (i > 0)
				{
					// Add splitter
					var handle = new Button(null)
					{
						ReleaseOnTouchLeft = false
					};

					if (Orientation == Orientation.Horizontal)
					{
						handle.MouseCursor = MouseCursorType.SizeWE;
						handle.VerticalAlignment = VerticalAlignment.Stretch;
					}
					else
					{
						handle.MouseCursor = MouseCursorType.SizeNS;
						handle.HorizontalAlignment = HorizontalAlignment.Stretch;
					}

					handle.ApplyButtonStyle(HandleStyle);

					handle.PressedChanged += HandleOnPressedChanged;

					proportion = new Proportion(ProportionType.Auto);

					if (Orientation == Orientation.Horizontal)
					{
						_handlesSize += handleSize;
						Grid.SetColumn(handle, i * 2 - 1);
						_layout.ColumnsProportions.Add(proportion);
					}
					else
					{
						_handlesSize += handleSize;
						Grid.SetRow(handle, i * 2 - 1);
						_layout.RowsProportions.Add(proportion);
					}

					Children.Add(handle);
					_handles.Add(handle);
				}

				proportion = i < Widgets.Count - 1
					? new Proportion(ProportionType.Part, 1.0f)
					: new Proportion(ProportionType.Fill, 1.0f);

				// Set grid coord and add widget itself
				if (Orientation == Orientation.Horizontal)
				{
					Grid.SetColumn(w, i * 2);
					_layout.ColumnsProportions.Add(proportion);
				}
				else
				{
					Grid.SetRow(w, i * 2);
					_layout.RowsProportions.Add(proportion);
				}

				Children.Add(w);

				++i;
			}

			foreach (var h in _handles)
			{
				if (Orientation == Orientation.Horizontal)
				{
					h.Width = handleSize;
					h.Height = null;
				}
				else
				{
					h.Width = null;
					h.Height = handleSize;
				}
			}

			FireProportionsChanged();
		}

		/// <summary>
		/// Applies a split pane style to this split pane.
		/// </summary>
		/// <param name="style">The split pane style to apply.</param>
		public void ApplySplitPaneStyle(SplitPaneStyle style)
		{
			ApplyWidgetStyle(style);

			HandleStyle = style.HandleStyle;
			Reset();
		}

		/// <summary>
		/// Copies the style properties from another split pane.
		/// </summary>
		protected internal override void CopyFrom(Widget w)
		{
			base.CopyFrom(w);

			var splitPane = (SplitPane)w;
			HandleStyle = splitPane.HandleStyle;
		}
	}
}