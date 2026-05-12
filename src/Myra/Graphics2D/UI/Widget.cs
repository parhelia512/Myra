using System;
using System.ComponentModel;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using System.Xml.Serialization;
using Myra.MML;
using Myra.Graphics2D.UI.Properties;
using Myra.Attributes;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Numerics;
using System.Drawing;
using Myra.Platform;
using Matrix = System.Numerics.Matrix3x2;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// Specifies the allowed directions for dragging a widget.
	/// </summary>
	[Flags]
	public enum DragDirection
	{
		/// <summary>No dragging is allowed.</summary>
		None = 0,
		/// <summary>Dragging is allowed in the vertical direction.</summary>
		Vertical = 1,
		/// <summary>Dragging is allowed in the horizontal direction.</summary>
		Horizontal = 2,
		/// <summary>Dragging is allowed in both vertical and horizontal directions.</summary>
		Both = Vertical | Horizontal
	}

	/// <summary>
	/// Base class for all UI widgets. Provides layout, rendering, input handling, and transformation capabilities.
	/// </summary>
	public partial class Widget : BaseObject, ITransformable
	{
		private MouseCursorType? _mouseCursorType;
		private Vector2? _startPos;
		private Point _startLeftTop;
		private Thickness _margin, _borderThickness, _padding;
		private int _left, _top;
		private int? _minWidth, _minHeight, _maxWidth, _maxHeight, _width, _height;
		private int _zIndex;
		private HorizontalAlignment _horizontalAlignment = HorizontalAlignment.Left;
		private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;
		private bool _measureDirty = true;
		private bool _arrangeDirty = true;
		private Desktop _desktop;

		private Point _lastMeasureSize;
		private Point _lastMeasureAvailableSize;

		private Rectangle _containerBounds;
		private Rectangle _layoutBounds;
		private bool _visible;

		private float _opacity = 1.0f;

		private bool _enabled;
		private bool _isKeyboardFocused = false;
		private Vector2 _scale = Vector2.One;
		private Vector2 _transformOrigin = new Vector2(0.5f, 0.5f);
		private float _rotation = 0.0f;
		private bool _transformDirty = true;
		private Transform _transform;

		/// <summary>
		/// Internal use only. (MyraPad)
		/// </summary>
		[DefaultValue(Stylesheet.DefaultStyleName)]
		public string StyleName { get; set; }

		/// <summary>
		/// Gets or sets the left position of the widget in pixels.
		/// </summary>
		[Category("Layout")]
		[DefaultValue(0)]
		public int Left
		{
			get { return _left; }

			set
			{
				if (value == _left)
				{
					return;
				}

				_left = value;
				InvalidateTransform();
				FireLocationChanged();
			}
		}

		/// <summary>
		/// Gets or sets the top position of the widget in pixels.
		/// </summary>
		[Category("Layout")]
		[DefaultValue(0)]
		public int Top
		{
			get { return _top; }

			set
			{
				if (value == _top)
				{
					return;
				}

				_top = value;
				InvalidateTransform();
				FireLocationChanged();
			}
		}

		/// <summary>
		/// Gets or sets the minimum width of the widget in pixels. Null means no minimum width constraint.
		/// </summary>
		[Category("Layout")]
		[DefaultValue(null)]
		public int? MinWidth
		{
			get { return _minWidth; }
			set
			{
				if (value == _minWidth)
				{
					return;
				}

				_minWidth = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		/// <summary>
		/// Gets or sets the maximum width of the widget in pixels. Null means no maximum width constraint.
		/// </summary>
		[Category("Layout")]
		[DefaultValue(null)]
		public int? MaxWidth
		{
			get { return _maxWidth; }
			set
			{
				if (value == _maxWidth)
				{
					return;
				}

				_maxWidth = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		/// <summary>
		/// Gets or sets the explicit width of the widget in pixels. Null means the width is determined by content or layout.
		/// </summary>
		[Category("Layout")]
		[DefaultValue(null)]
		public int? Width
		{
			get
			{
				return _width;
			}

			set
			{
				if (value == _width)
				{
					return;
				}

				_width = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		/// <summary>
		/// Gets or sets the minimum height of the widget in pixels. Null means no minimum height constraint.
		/// </summary>
		[Category("Layout")]
		[DefaultValue(null)]
		public int? MinHeight
		{
			get { return _minHeight; }
			set
			{
				if (value == _minHeight)
				{
					return;
				}

				_minHeight = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		/// <summary>
		/// Gets or sets the maximum height of the widget in pixels. Null means no maximum height constraint.
		/// </summary>
		[Category("Layout")]
		[DefaultValue(null)]
		public int? MaxHeight
		{
			get { return _maxHeight; }
			set
			{
				if (value == _maxHeight)
				{
					return;
				}

				_maxHeight = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		/// <summary>
		/// Gets or sets the explicit height of the widget in pixels. Null means the height is determined by content or layout.
		/// </summary>
		[Category("Layout")]
		[DefaultValue(null)]
		public int? Height
		{
			get
			{
				return _height;
			}

			set
			{
				if (value == _height)
				{
					return;
				}

				_height = value;
				InvalidateMeasure();
				FireSizeChanged();
			}
		}

		/// <summary>
		/// Gets or sets the outer spacing around the widget.
		/// </summary>
		[Category("Layout")]
		[DesignerFolded]
		public Thickness Margin
		{
			get
			{
				return _margin;
			}

			set
			{
				if (_margin == value)
				{
					return;
				}

				_margin = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the brush used to draw the widget's border.
		/// </summary>
		[Category("Layout")]
		public IBrush Border { get; set; }

		/// <summary>
		/// Gets or sets the thickness of the widget's border on all sides.
		/// </summary>
		[Category("Layout")]
		[DesignerFolded]
		public Thickness BorderThickness
		{
			get
			{
				return _borderThickness;
			}

			set
			{
				if (_borderThickness == value)
				{
					return;
				}

				_borderThickness = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the inner spacing between the border and the widget's content.
		/// </summary>
		[Category("Layout")]
		[DesignerFolded]
		public Thickness Padding
		{
			get
			{
				return _padding;
			}

			set
			{
				if (_padding == value)
				{
					return;
				}

				_padding = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the widget within its container.
		/// </summary>
		[Category("Layout")]
		[DefaultValue(HorizontalAlignment.Left)]
		public virtual HorizontalAlignment HorizontalAlignment
		{
			get { return _horizontalAlignment; }

			set
			{
				if (value == _horizontalAlignment)
				{
					return;
				}

				_horizontalAlignment = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the vertical alignment of the widget within its container.
		/// </summary>
		[Category("Layout")]
		[DefaultValue(VerticalAlignment.Top)]
		public virtual VerticalAlignment VerticalAlignment
		{
			get { return _verticalAlignment; }

			set
			{
				if (value == _verticalAlignment)
				{
					return;
				}

				_verticalAlignment = value;
				InvalidateMeasure();
			}
		}

		[Browsable(false)]
		[Obsolete("Use Grid.GetColumn/Grid.SetColumn")]

		public int GridColumn
		{
			get => Grid.GetColumn(this);
			set => Grid.SetColumn(this, value);
		}

		[Browsable(false)]
		[Obsolete("Use Grid.GetColumn/Grid.SetColumn")]
		public int GridRow
		{
			get => Grid.GetRow(this);
			set => Grid.SetRow(this, value);
		}

		[Browsable(false)]
		[Obsolete("Use Grid.GetColumnSpan/Grid.SetColumnSpan")]
		public int GridColumnSpan
		{
			get => Grid.GetColumnSpan(this);
			set => Grid.SetColumnSpan(this, value);
		}

		[Browsable(false)]
		[Obsolete("Use Grid.GetColumnSpan/Grid.SetColumnSpan")]
		public int GridRowSpan
		{
			get => Grid.GetRowSpan(this);
			set => Grid.SetRowSpan(this, value);
		}

		/// <summary>
		/// Gets or sets whether the widget is enabled for interaction. Disabled widgets and their children cannot receive input events.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		public bool Enabled
		{
			get
			{
				return _enabled;
			}

			set
			{
				if (value == _enabled)
				{
					return;
				}

				_enabled = value;

				foreach (var item in ChildrenCopy)
				{
					item.Enabled = value;
				}

				EnabledChanged.Invoke(this);
			}
		}

		/// <summary>
		/// Gets or sets whether the widget is visible on screen. Hidden widgets do not render or receive input.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		public bool Visible
		{
			get { return _visible; }

			set
			{
				if (_visible == value)
				{
					return;
				}

				_visible = value;
				LocalMousePosition = null;
				LocalTouchPosition = null;

				OnVisibleChanged();
			}
		}

		/// <summary>
		/// Gets or sets the directions in which this widget can be dragged.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(DragDirection.None)]
		public virtual DragDirection DragDirection { get; set; } = DragDirection.None;

		[XmlIgnore]
		[Browsable(false)]
		internal bool IsDraggable { get => DragDirection != DragDirection.None; }

		/// <summary>
		/// Gets or sets the z-index (stacking order) of the widget. Higher values appear on top.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(0)]
		public int ZIndex
		{
			get => _zIndex;
			set
			{
				if (value == _zIndex)
				{
					return;
				}

				_zIndex = value;
				InvalidateMeasure();
			}
		}

		/// <summary>
		/// Gets or sets the mouse cursor type to display when hovering over this widget.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(null)]
		public virtual MouseCursorType? MouseCursor
		{
			get => _mouseCursorType;
			set
			{
				if (value == _mouseCursorType)
				{
					return;
				}

				_mouseCursorType = value;
				foreach (var child in Children)
				{
					child.MouseCursor = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the text displayed in a tooltip when hovering over this widget.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(null)]
		public string Tooltip { get; set; }


		/// <summary>
		/// Gets or sets the scale transformation applied to the widget. Default is (1, 1) for normal size.
		/// </summary>
		[Category("Transform")]
		[DefaultValue("1, 1")]
		[DesignerFolded]
		public Vector2 Scale
		{
			get => _scale;
			set
			{
				if (value == _scale)
				{
					return;
				}

				_scale = value;
				InvalidateTransform();
			}
		}

		/// <summary>
		/// Gets or sets the origin point for scale and rotation transformations. Default is (0.5, 0.5) for center.
		/// </summary>
		[Category("Transform")]
		[DefaultValue("0.5, 0.5")]
		[DesignerFolded]
		public Vector2 TransformOrigin
		{
			get => _transformOrigin;
			set
			{
				if (value == _transformOrigin)
				{
					return;
				}

				_transformOrigin = value;
				InvalidateTransform();
			}
		}

		/// <summary>
		/// Gets or sets the rotation angle in degrees applied to the widget. Rotation is applied around the TransformOrigin.
		/// </summary>
		[Category("Transform")]
		[DefaultValue(0.0f)]
		[DesignerFolded]
		public float Rotation
		{
			get => _rotation;

			set
			{
				if (value == _rotation)
				{
					return;
				}

				_rotation = value;
				InvalidateTransform();
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		public Widget DragHandle { get; set; }

		/// <summary>
		/// Determines whether the widget had been placed on Desktop
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public bool IsPlaced
		{
			get
			{
				return Desktop != null;
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		public virtual Desktop Desktop
		{
			get
			{
				return _desktop;
			}

			internal set
			{
				if (_desktop != null && value == null)
				{
					if (_desktop.FocusedKeyboardWidget == this)
					{
						_desktop.FocusedKeyboardWidget = null;
					}

					if (_desktop.Tooltip != null && _desktop.Tooltip.Tag == this)
					{
						_desktop.HideTooltip();
					}
				}

				LocalMousePosition = null;
				LocalTouchPosition = null;

				_desktop = value;

				if (_desktop != null)
				{
					InvalidateMeasure();
				}

				SubscribeOnTouchMoved(IsPlaced && IsDraggable);

				foreach (var child in ChildrenCopy)
				{
					child.Desktop = value;
				}

				OnPlacedChanged();
			}
		}

		[XmlIgnore]
		[Browsable(false)]
		public bool IsModal { get; set; }

		/// <summary>
		/// Gets or sets the opacity of the widget. Valid range is 0.0 (fully transparent) to 1.0 (fully opaque).
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(1.0f)]
		[Range(0.0f, 1.0f)]
		public float Opacity
		{
			get
			{
				return _opacity;
			}

			set
			{
				if (value < 0 || value > 1.0f)
				{
					throw new ArgumentOutOfRangeException("value");
				}

				_opacity = value;
			}
		}

		/// <summary>
		/// Gets or sets the dynamic layout expression for this widget.
		/// </summary>
		[XmlIgnore]
		[Browsable(false)]
		public Layout2D Layout2d { get; set; } = Layout2D.NullLayout;

		/// <summary>
		/// Gets or sets the brush used to draw the widget's background.
		/// </summary>
		[Category("Appearance")]
		public IBrush Background { get; set; }

		/// <summary>
		/// Gets or sets the brush used to draw the widget's background when the mouse is over it.
		/// </summary>
		[Category("Appearance")]
		public IBrush OverBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used to draw the widget's background when it is disabled.
		/// </summary>
		[Category("Appearance")]
		public IBrush DisabledBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used to draw the widget's background when it has keyboard focus.
		/// </summary>
		[Category("Appearance")]
		public IBrush FocusedBackground { get; set; }

		/// <summary>
		/// Gets or sets the brush used to draw the widget's border when the mouse is over it.
		/// </summary>
		[Category("Appearance")]
		public IBrush OverBorder
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the brush used to draw the widget's border when it is disabled.
		/// </summary>
		[Category("Appearance")]
		public IBrush DisabledBorder
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the brush used to draw the widget's border when it has keyboard focus.
		/// </summary>
		[Category("Appearance")]
		public IBrush FocusedBorder
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets whether the widget's content is clipped to its bounds to prevent overflow.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(false)]
		public virtual bool ClipToBounds { get; set; }

		/// <summary>
		/// Gets the parent widget that contains this widget. Null if this widget is at the root level.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Widget Parent { get; internal set; }

		/// <summary>
		/// Gets or sets custom data associated with this widget.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public object Tag { get; set; }

		/// <summary>
		/// Gets the bounds of the widget with (0, 0) as the top-left corner.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Rectangle Bounds => new Rectangle(0, 0, _layoutBounds.Width, _layoutBounds.Height);

		/// <summary>
		/// Gets the bounds of the widget's content area, excluding margin, border, and padding.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Rectangle ActualBounds => Bounds - _margin - _borderThickness - _padding;

		/// <summary>
		/// Gets the bounds of the widget excluding the margin.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		internal Rectangle BorderBounds => Bounds - _margin;

		[Browsable(false)]
		[XmlIgnore] protected Rectangle BackgroundBounds => BorderBounds - _borderThickness;

		/// <summary>
		/// Gets the container bounds in which this widget is arranged.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Rectangle ContainerBounds => _containerBounds;

		/// <summary>
		/// Gets the total width of margin, border, and padding on left and right sides.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int MBPWidth => Margin.Left + Margin.Right +
					BorderThickness.Left + BorderThickness.Right +
					Padding.Left + Padding.Right;

		/// <summary>
		/// Gets the total height of margin, border, and padding on top and bottom sides.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public int MBPHeight => Margin.Top + Margin.Bottom +
					BorderThickness.Top + BorderThickness.Bottom +
					Padding.Top + Padding.Bottom;

		/// <summary>
		/// Gets or sets whether this widget can receive keyboard focus.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool AcceptsKeyboardFocus { get; set; }

		/// <summary>
		/// Gets whether this widget currently has keyboard focus.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool IsKeyboardFocused
		{
			get
			{
				return _isKeyboardFocused;
			}

			internal set
			{
				if (value == _isKeyboardFocused)
				{
					return;
				}

				_isKeyboardFocused = value;
				KeyboardFocusChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		internal Transform Transform
		{
			get
			{
				UpdateTransform();

				return _transform;
			}
		}

		protected virtual bool UseOverBackground => IsMouseInside;

		[Browsable(false)]
		[XmlIgnore]
		public Action<RenderContext> BeforeRender, AfterRender;

		public Widget()
		{
			Visible = true;
			Enabled = true;
			DragHandle = this;

			Children.CollectionChanged += ChildrenOnCollectionChanged;
		}

		public virtual IBrush GetCurrentBackground()
		{
			var result = Background;

			if (!Enabled && DisabledBackground != null)
			{
				result = DisabledBackground;
			}
			else if (Enabled && IsKeyboardFocused && FocusedBackground != null)
			{
				result = FocusedBackground;
			}
			else if (UseOverBackground && OverBackground != null)
			{
				result = OverBackground;
			}

			return result;
		}

		public virtual IBrush GetCurrentBorder()
		{
			var result = Border;

			if (!Enabled && DisabledBorder != null)
			{
				result = DisabledBorder;
			}
			else if (Enabled && IsKeyboardFocused && FocusedBorder != null)
			{
				result = FocusedBorder;
			}
			else if (IsMouseInside && OverBorder != null)
			{
				result = OverBorder;
			}

			return result;
		}

		/// <summary>
		/// Brings this widget to the front of its siblings, making it render on top.
		/// </summary>
		public void BringToFront()
		{
			var widgets = Parent != null ? Parent.Children : Desktop.Widgets;

			if (widgets[widgets.Count - 1] == this) return;

			widgets.Remove(this);
			widgets.Add(this);
		}

		/// <summary>
		/// Sends this widget to the back of its siblings, making it render behind others.
		/// </summary>
		public void BringToBack()
		{
			var widgets = Parent != null ? Parent.Children : Desktop.Widgets;

			if (widgets[widgets.Count - 1] == this) return;

			widgets.Remove(this);
			widgets.Insert(0, this);
		}

		/// <summary>
		/// Renders this widget and all its children to the render context.
		/// </summary>
		/// <param name="context">The render context to render to.</param>
		public void Render(RenderContext context)
		{
			if (!Visible)
			{
				return;
			}

			if (!string.IsNullOrEmpty(Tooltip) && (Desktop.Tooltip == null || Desktop.Tooltip.Tag != this) &&
				_lastMouseMovement != null && (DateTime.Now - _lastMouseMovement.Value).TotalMilliseconds > MyraEnvironment.TooltipDelayInMs)
			{
				var pos = Desktop.MousePosition;
				pos.X += MyraEnvironment.TooltipOffset.X;
				pos.Y += MyraEnvironment.TooltipOffset.Y;
				Desktop.ShowTooltip(this, pos);
				_lastMouseMovement = null;
			}

			UpdateArrange();

			var oldTransform = context.Transform;

			// Apply widget transforms
			context.Transform = Transform;

			Rectangle? oldScissorRectangle = null;
			if (context.Transform.Rotation.IsZero())
			{
				var absoluteBounds = Transform.Apply(Bounds);
				var scissorBounds = Rectangle.Intersect(context.Scissor, absoluteBounds);

				if (scissorBounds.Width == 0 || scissorBounds.Height == 0)
				{
					// Culled by scissor
					context.Transform = oldTransform;
					return;
				}

				if (ClipToBounds)
				{
					oldScissorRectangle = context.Scissor;

					context.Scissor = scissorBounds;
				}
			}

			var oldOpacity = context.Opacity;
			context.AddOpacity(Opacity);

			// Draw Background
			var background = GetCurrentBackground();
			if (background != null)
			{
				background.Draw(context, BackgroundBounds);
			}

			// Draw Borders
			var border = GetCurrentBorder();
			if (border != null)
			{
				var borderBounds = BorderBounds;
				if (BorderThickness.Left > 0)
				{
					border.Draw(context, new Rectangle(borderBounds.X, borderBounds.Y, BorderThickness.Left, borderBounds.Height));
				}

				if (BorderThickness.Top > 0)
				{
					border.Draw(context, new Rectangle(borderBounds.X, borderBounds.Y, borderBounds.Width, BorderThickness.Top));
				}

				if (BorderThickness.Right > 0)
				{
					border.Draw(context, new Rectangle(borderBounds.Right - BorderThickness.Right, borderBounds.Y, BorderThickness.Right, borderBounds.Height));
				}

				if (BorderThickness.Bottom > 0)
				{
					border.Draw(context, new Rectangle(borderBounds.X, borderBounds.Bottom - BorderThickness.Bottom, borderBounds.Width, BorderThickness.Bottom));
				}
			}

			// Internal rendering
			BeforeRender?.Invoke(context);
			InternalRender(context);
			AfterRender?.Invoke(context);

			if (oldScissorRectangle != null)
			{
				// Restore scissor
				context.Scissor = oldScissorRectangle.Value;
			}

			// Optional debug rendering
			if (MyraEnvironment.DrawWidgetsFrames)
			{
				context.DrawRectangle(Bounds, Color.LightGreen);
			}

			if (MyraEnvironment.DrawKeyboardFocusedWidgetFrame && IsKeyboardFocused)
			{
				context.DrawRectangle(Bounds, Color.Red);
			}

			if (MyraEnvironment.DrawMouseHoveredWidgetFrame && IsMouseInside)
			{
				context.DrawRectangle(Bounds, Color.Yellow);
			}

			// Restore context settings
			context.Transform = oldTransform;
			context.Opacity = oldOpacity;
		}

		/// <summary>
		/// Internal method that renders the widget's specific content. Subclasses override this to provide custom rendering.
		/// </summary>
		/// <param name="context">The render context to render to.</param>
		public virtual void InternalRender(RenderContext context)
		{
			foreach (var child in ChildrenCopy)
			{
				child.Render(context);
			}
		}

		/// <summary>
		/// Measures the widget's desired size based on available space.
		/// </summary>
		/// <param name="availableSize">The maximum size available for the widget.</param>
		/// <returns>The desired size of the widget.</returns>
		public Point Measure(Point availableSize)
		{
			if (!_measureDirty && _lastMeasureAvailableSize == availableSize)
			{
				return _lastMeasureSize;
			}

			Point result;

			// Lerp available size by Width/Height or MaxWidth/MaxHeight
			if (Width != null && availableSize.X > Width.Value)
			{
				availableSize.X = Width.Value;
			}
			else if (MaxWidth != null && availableSize.X > MaxWidth.Value)
			{
				availableSize.X = MaxWidth.Value;
			}

			if (Height != null && availableSize.Y > Height.Value)
			{
				availableSize.Y = Height.Value;
			}
			else if (MaxHeight != null && availableSize.Y > MaxHeight.Value)
			{
				availableSize.Y = MaxHeight.Value;
			}

			availableSize.X -= MBPWidth;
			availableSize.Y -= MBPHeight;

			// Do the actual measure
			// Previously I skipped this step if both Width & Height were set
			// However that raised an issue - custom InternalMeasure stuff(such as in Menu.InternalMeasure) was skipped as well
			// So now InternalMeasure is called every time
			result = InternalMeasure(availableSize);

			result.X += MBPWidth;
			result.Y += MBPHeight;

			// Result lerp
			if (Width.HasValue)
			{
				result.X = Width.Value;
			}
			else
			{
				if (MinWidth.HasValue && result.X < MinWidth.Value)
				{
					result.X = MinWidth.Value;
				}

				if (MaxWidth.HasValue && result.X > MaxWidth.Value)
				{
					result.X = MaxWidth.Value;
				}
			}

			if (Height.HasValue)
			{
				result.Y = Height.Value;
			}
			else
			{
				if (MinHeight.HasValue && result.Y < MinHeight.Value)
				{
					result.Y = MinHeight.Value;
				}

				if (MaxHeight.HasValue && result.Y > MaxHeight.Value)
				{
					result.Y = MaxHeight.Value;
				}
			}

			_lastMeasureSize = result;
			_lastMeasureAvailableSize = availableSize;
			_measureDirty = false;

			return result;
		}

		/// <summary>
		/// Arranges the widget within the given container bounds.
		/// </summary>
		/// <param name="containerBounds">The bounds in which to arrange the widget.</param>
		public void Arrange(Rectangle containerBounds)
		{
			if (!_arrangeDirty && _containerBounds == containerBounds)
			{
				return;
			}

			_arrangeDirty = true;
			_containerBounds = containerBounds;
			UpdateArrange();
		}

		/// <summary>
		/// Updates the widget's arrangement within its container bounds.
		/// </summary>
		public void UpdateArrange()
		{
			if (!_arrangeDirty)
			{
				return;
			}

			Point size;
			if (HorizontalAlignment != HorizontalAlignment.Stretch ||
					VerticalAlignment != VerticalAlignment.Stretch)
			{
				size = Measure(_containerBounds.Size());
			}
			else
			{
				size = _containerBounds.Size();
			}

			if (size.X > _containerBounds.Width)
			{
				size.X = _containerBounds.Width;
			}

			if (size.Y > _containerBounds.Height)
			{
				size.Y = _containerBounds.Height;
			}

			// Resolve possible conflict beetween Alignment set to Streth and Size explicitly set
			var containerSize = _containerBounds.Size();
			if (HorizontalAlignment == HorizontalAlignment.Stretch && Width != null && Width.Value < containerSize.X)
			{
				containerSize.X = Width.Value;
			}

			if (VerticalAlignment == VerticalAlignment.Stretch && Height != null && Height.Value < containerSize.Y)
			{
				containerSize.Y = Height.Value;
			}

			// Align
			var layoutBounds = LayoutUtils.Align(containerSize, size, HorizontalAlignment, VerticalAlignment);
			layoutBounds.Offset(_containerBounds.Location);

			_layoutBounds = layoutBounds;
			InvalidateTransform();

			InternalArrange();
			ArrangeUpdated.Invoke(this);

			_arrangeDirty = false;
		}

		protected virtual void InternalArrange()
		{
			if (ChildrenLayout == null)
			{
				return;
			}

			ChildrenLayout.Arrange(ChildrenCopy, ActualBounds);
		}

		protected virtual Point InternalMeasure(Point availableSize)
		{
			if (ChildrenLayout == null)
			{
				return Mathematics.PointZero;
			}

			return ChildrenLayout.Measure(ChildrenCopy, availableSize);
		}


		/// <summary>
		/// Marks the widget's arrangement as dirty, forcing it to be re-arranged on the next update.
		/// </summary>
		public void InvalidateArrange()
		{
			_arrangeDirty = true;
		}

		/// <summary>
		/// Finds a child widget by its ID. Throws an exception if not found.
		/// </summary>
		/// <param name="id">The ID of the widget to find.</param>
		/// <returns>The widget with the specified ID.</returns>
		/// <exception cref="Exception">Thrown when the widget with the specified ID is not found.</exception>
		public Widget EnsureWidgetById(string id)
		{
			var result = FindChildById(id);
			if (result == null)
			{
				throw new Exception(string.Format($"Could not find widget with id {id}"));
			}

			return result;
		}

		/// <summary>
		/// Internal method that marks the widget's transform as dirty, forcing recalculation.
		/// </summary>
		internal virtual void InvalidateTransform()
		{
			_transformDirty = true;

			foreach (var child in ChildrenCopy)
			{
				child.InvalidateTransform();
			}
		}

		/// <summary>
		/// Marks the widget's measurement as dirty, forcing it to be re-measured and re-arranged.
		/// </summary>
		public virtual void InvalidateMeasure()
		{
			_measureDirty = true;

			InvalidateArrange();

			if (Parent != null)
			{
				Parent.InvalidateMeasure();
			}
			else if (Desktop != null)
			{
				Desktop.InvalidateLayout();
			}
		}

		/// <summary>
		/// Applies a widget style to this widget.
		/// </summary>
		/// <param name="style">The style to apply.</param>
		public void ApplyWidgetStyle(WidgetStyle style)
		{
			Width = style.Width;
			Height = style.Height;
			MinWidth = style.MinWidth;
			MinHeight = style.MinHeight;
			MaxWidth = style.MaxWidth;
			MaxHeight = style.MaxHeight;

			Background = style.Background;
			OverBackground = style.OverBackground;
			DisabledBackground = style.DisabledBackground;
			FocusedBackground = style.FocusedBackground;

			Border = style.Border;
			OverBorder = style.OverBorder;
			DisabledBorder = style.DisabledBorder;
			FocusedBorder = style.FocusedBorder;

			Margin = style.Margin;
			BorderThickness = style.BorderThickness;
			Padding = style.Padding;
		}

		/// <summary>
		/// Sets the style of this widget from the specified stylesheet.
		/// </summary>
		/// <param name="stylesheet">The stylesheet containing the style.</param>
		/// <param name="name">The name of the style to apply.</param>
		public void SetStyle(Stylesheet stylesheet, string name)
		{
			StyleName = name;

			if (StyleName != null)
			{
				InternalSetStyle(stylesheet, StyleName);
			}
		}

		/// <summary>
		/// Sets the style of this widget from the current default stylesheet.
		/// </summary>
		/// <param name="name">The name of the style to apply.</param>
		public void SetStyle(string name)
		{
			SetStyle(Stylesheet.Current, name);
		}

		/// <summary>
		/// Internal method for subclasses to apply style properties. Override in derived classes.
		/// </summary>
		/// <param name="stylesheet">The stylesheet containing the style.</param>
		/// <param name="name">The name of the style being applied.</param>
		protected virtual void InternalSetStyle(Stylesheet stylesheet, string name)
		{
		}

		protected void FireKeyDown(Keys k)
		{
			KeyDown.Invoke(this, k);
		}

		/// <summary>
		/// Called when a keyboard key is pressed while this widget has focus.
		/// </summary>
		/// <param name="k">The key that was pressed.</param>
		public virtual void OnKeyDown(Keys k)
		{
			FireKeyDown(k);
		}

		/// <summary>
		/// Called when a keyboard key is released while this widget has focus.
		/// </summary>
		/// <param name="k">The key that was released.</param>
		public virtual void OnKeyUp(Keys k)
		{
			KeyUp.Invoke(this, k);
		}

		/// <summary>
		/// Called when a character is typed while this widget has focus.
		/// </summary>
		/// <param name="c">The character that was typed.</param>
		public virtual void OnChar(char c)
		{
			Char.Invoke(this, c);
		}

		/// <summary>
		/// Called when this widget is placed on or removed from a desktop.
		/// </summary>
		protected virtual void OnPlacedChanged()
		{
			PlacedChanged?.Invoke(this);
		}

		/// <summary>
		/// Called when the visibility of this widget changes.
		/// </summary>
		public virtual void OnVisibleChanged()
		{
			InvalidateMeasure();
			VisibleChanged.Invoke(this);
		}

		/// <summary>
		/// Called when this widget loses keyboard focus.
		/// </summary>
		public virtual void OnLostKeyboardFocus()
		{
			IsKeyboardFocused = false;
		}

		/// <summary>
		/// Called when this widget gains keyboard focus.
		/// </summary>
		public virtual void OnGotKeyboardFocus()
		{
			IsKeyboardFocused = true;
		}

		/// <summary>
		/// Removes this widget from its parent container.
		/// </summary>
		public void RemoveFromParent()
		{
			if (Parent == null)
			{
				return;
			}

			Parent.Children.Remove(this);
		}

		/// <summary>
		/// Removes this widget from the desktop.
		/// </summary>
		public void RemoveFromDesktop()
		{
			Desktop.Widgets.Remove(this);
		}

		private void FireLocationChanged()
		{
			LocationChanged.Invoke(this);
		}

		private void FireSizeChanged()
		{
			SizeChanged.Invoke(this);
		}

		/// <summary>
		/// Sets keyboard focus to this widget.
		/// </summary>
		public void SetKeyboardFocus()
		{
			Desktop.FocusedKeyboardWidget = this;
		}

		private void SubscribeOnTouchMoved(bool subscribe)
		{
			if (Parent != null)
			{
				Parent.TouchMoved -= DesktopOnTouchMoved;
				Parent.TouchUp -= DesktopTouchUp;
			}
			else if (Desktop != null)
			{
				Desktop.TouchMoved -= DesktopOnTouchMoved;
				Desktop.TouchUp -= DesktopTouchUp;
			}

			if (subscribe)
			{
				if (Parent != null)
				{
					Parent.TouchMoved += DesktopOnTouchMoved;
					Parent.TouchUp += DesktopTouchUp;
				}
				else if (Desktop != null)
				{
					Desktop.TouchMoved += DesktopOnTouchMoved;
					Desktop.TouchUp += DesktopTouchUp;
				}
			}
		}

		private void DesktopOnTouchMoved(object sender, EventArgs args)
		{
			if (_startPos == null || !IsDraggable || Desktop == null)
			{
				return;
			}

			var parent = Parent != null ? (ITransformable)Parent : Desktop;
			var newPos = parent.ToLocal(new Vector2(Desktop.TouchPosition.Value.X, Desktop.TouchPosition.Value.Y));
			var delta = newPos - _startPos.Value;

			var newLeft = Left;
			var newTop = Top;
			if (DragDirection.HasFlag(DragDirection.Horizontal))
			{
				newLeft = _startLeftTop.X + (int)delta.X;
			}

			if (DragDirection.HasFlag(DragDirection.Vertical))
			{
				newTop = _startLeftTop.Y + (int)delta.Y;
			}

			var parentBounds = Parent != null ? Parent.Bounds : Desktop.InternalBounds;
			if (newLeft < 0)
			{
				newLeft = 0;
			}

			if (newLeft + Bounds.Width > parentBounds.Width)
			{
				newLeft = parentBounds.Width - Bounds.Width;
			}

			if (newTop < 0)
			{
				newTop = 0;
			}

			if (newTop + Bounds.Height > parentBounds.Height)
			{
				newTop = parentBounds.Height - Bounds.Height;
			}

			Left = newLeft;
			Top = newTop;
		}

		/// <summary>
		/// Converts a position from local widget coordinates to global screen coordinates.
		/// </summary>
		/// <param name="pos">The position in local coordinates.</param>
		/// <returns>The position in global coordinates.</returns>
		public Vector2 ToGlobal(Vector2 pos)
		{
			UpdateTransform();

			return Transform.Apply(pos);
		}

		/// <summary>
		/// Converts a position from local widget coordinates to global screen coordinates.
		/// </summary>
		/// <param name="pos">The position in local coordinates.</param>
		/// <returns>The position in global coordinates.</returns>
		public Point ToGlobal(Point pos)
		{
			UpdateTransform();

			return Transform.Apply(pos);
		}

		/// <summary>
		/// Converts a position from global screen coordinates to local widget coordinates.
		/// </summary>
		/// <param name="pos">The position in global coordinates.</param>
		/// <returns>The position in local coordinates.</returns>
		public Vector2 ToLocal(Vector2 pos)
		{
			UpdateTransform();

			return Transform.InverseApply(pos);
		}

		/// <summary>
		/// Converts a position from global screen coordinates to local widget coordinates.
		/// </summary>
		/// <param name="pos">The position in global coordinates.</param>
		/// <returns>The position in local coordinates.</returns>
		public Point ToLocal(Point pos) => ToLocal(new Vector2(pos.X, pos.Y)).ToPoint();

		/// <summary>
		/// Determines whether a global screen position falls within this widget's border bounds.
		/// </summary>
		/// <param name="globalPos">The position in global coordinates.</param>
		/// <returns>True if the position is within the widget's border bounds; otherwise, false.</returns>
		public bool ContainsGlobalPoint(Point globalPos)
		{
			var localPos = ToLocal(globalPos);
			return BorderBounds.Contains(localPos);
		}

		private void UpdateTransform()
		{
			if (!_transformDirty)
			{
				return;
			}

			var p = new Point(_layoutBounds.X + Left, _layoutBounds.Y + Top);

			var localTransform = new Transform(p.ToVector2(),
				TransformOrigin * _layoutBounds.Size().ToVector2(),
				Scale,
				Rotation * (float)Math.PI / 180);

			if (Parent != null)
			{
				var transform = Parent.Transform;
				transform.AddTransform(ref localTransform);
				_transform = transform;
			}
			else if (Desktop != null)
			{
				var transform = Desktop.Transform;
				transform.AddTransform(ref localTransform);
				_transform = transform;
			}
			else
			{
				_transform = localTransform;
			}

			_transformDirty = false;
		}


		private void DesktopTouchUp(object sender, EventArgs args)
		{
			_startPos = null;
		}

		/// <summary>
		/// Performs hit testing to find which widget is at the given screen position.
		/// </summary>
		/// <param name="p">The position in global screen coordinates.</param>
		/// <returns>The widget at the position, or null if no widget is there.</returns>
		public virtual Widget HitTest(Point p)
		{
			if (Desktop == null || !Visible || !ContainsGlobalPoint(p))
			{
				return null;
			}

			Widget result = null;
			for (var i = _childrenCopy.Count - 1; i >= 0; i--)
			{
				var child = _childrenCopy[i];
				result = child.HitTest(p);
				if (result != null)
				{
					break;
				}
			}

			var localPos = ToLocal(p);
			if (result == null && !InputFallsThrough(localPos))
			{
				result = this;
			}

			return result;
		}

		/// <summary>
		/// Determines whether input at the given local position should pass through this widget to underlying widgets.
		/// </summary>
		/// <param name="localPos">The position in local widget coordinates.</param>
		/// <returns>True if input should pass through; otherwise, false.</returns>
		public virtual bool InputFallsThrough(Point localPos) => false;

		/// <summary>
		/// Creates a deep copy of this widget with all its properties and child widgets.
		/// </summary>
		/// <returns>A new widget that is a copy of this widget.</returns>
		public Widget Clone()
		{
			// Firstly try to use parameterless constructor
			var type = GetType();
			var constructor = type.GetConstructor(Type.EmptyTypes);

			Widget result;
			if (constructor != null)
			{
				result = (Widget)constructor.Invoke(new object[0]);
			}
			else
			{
				// Then string constructor
				result = (Widget)Activator.CreateInstance(GetType(), (string)null);
			}

			result.CopyFrom(this);

			// Copy attached properties
			foreach (var pair in AttachedPropertiesValues)
			{
				result.AttachedPropertiesValues[pair.Key] = pair.Value;
			}

			return result;
		}

		/// <summary>
		/// Internal method that copies properties from another widget. Override in derived classes to copy custom properties.
		/// </summary>
		/// <param name="w">The widget to copy properties from.</param>
		protected internal virtual void CopyFrom(Widget w)
		{
			StyleName = w.StyleName;
			Left = w.Left;
			Top = w.Top;
			MinWidth = w.MinWidth;
			MaxWidth = w.MaxWidth;
			Width = w.Width;
			MinHeight = w.MinHeight;
			MaxHeight = w.MaxHeight;
			Height = w.Height;
			Margin = w.Margin;
			Border = w.Border;
			BorderThickness = w.BorderThickness;
			Padding = w.Padding;
			HorizontalAlignment = w.HorizontalAlignment;
			VerticalAlignment = w.VerticalAlignment;
			Enabled = w.Enabled;
			Visible = w.Visible;
			DragDirection = w.DragDirection;
			ZIndex = w.ZIndex;
			MouseCursor = w.MouseCursor;
			Tooltip = w.Tooltip;
			Scale = w.Scale;
			TransformOrigin = w.TransformOrigin;
			Rotation = w.Rotation;
			DragHandle = w.DragHandle;
			IsModal = w.IsModal;
			Opacity = w.Opacity;
			Background = w.Background;
			OverBackground = w.OverBackground;
			DisabledBackground = w.DisabledBackground;
			FocusedBackground = w.FocusedBackground;
			OverBorder = w.OverBorder;
			DisabledBorder = w.DisabledBorder;
			FocusedBorder = w.FocusedBorder;
			ClipToBounds = w.ClipToBounds;
			Tag = w.Tag;
			AcceptsKeyboardFocus = w.AcceptsKeyboardFocus;
			BeforeRender = w.BeforeRender;
			AfterRender = w.AfterRender;
		}
	}
}