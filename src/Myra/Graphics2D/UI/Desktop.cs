using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using Myra.Events;
using MonoGame.Utilities;


#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#elif STRIDE
using Stride.Core.Mathematics;
using Stride.Input;
#else
using System.Drawing;
using Myra.Platform;
using System.Numerics;
using Matrix = System.Numerics.Matrix3x2;
#endif

namespace Myra.Graphics2D.UI
{
	/// <summary>
	/// The root UI container that manages all widgets, input events, rendering, and layout for a UI hierarchy.
	/// </summary>
	public partial class Desktop : ITransformable, IDisposable
	{
		private Rectangle _lastBounds;
		private Vector2 _scale = Vector2.One;
		private Vector2 _transformOrigin = new Vector2(0.5f, 0.5f);
		private float _rotation = 0.0f;

		private bool _transformDirty = true;
		private Transform _transform;

		private readonly InputContext _inputContext = new InputContext();
		private readonly RenderContext _renderContext = new RenderContext();

		private bool _layoutDirty = true;
		private bool _widgetsDirty = true;
		private Widget _focusedKeyboardWidget;
		private readonly List<Widget> _widgetsCopy = new List<Widget>();
		private Widget _previousKeyboardFocus;
#if MONOGAME || PLATFORM_AGNOSTIC
		/// <summary>
		/// Indicates whether external text input is being used (for MonoGame and platform-agnostic builds).
		/// </summary>
		public bool HasExternalTextInput = false;
#endif

		private bool _isDisposed = false;

		/// <summary>
		/// Gets or sets the function that returns the bounds for the desktop.
		/// </summary>
		public Func<Rectangle> BoundsFetcher { get; set; } = DefaultBoundsFetcher;

		/// <summary>
		/// Gets or sets the root widget displayed on the desktop.
		/// </summary>
		public Widget Root
		{
			get
			{
				if (Widgets.Count == 0)
				{
					return null;
				}

				return Widgets[0];
			}

			set
			{
				if (Root == value)
				{
					return;
				}

				HideContextMenu();
				HideTooltip();
				Widgets.Clear();

				if (value != null)
				{
					Widgets.Add(value);
				}
			}
		}

		/// <summary>
		/// Gets the menu bar widget at the top of the desktop.
		/// </summary>
		public HorizontalMenu MenuBar { get; private set; }

		/// <summary>
		/// Gets a copy of the child widgets collection for internal use.
		/// </summary>
		internal List<Widget> ChildrenCopy
		{
			get
			{
				UpdateWidgetsCopy();
				return _widgetsCopy;
			}
		}

		/// <summary>
		/// Gets the collection of top-level widgets displayed on the desktop.
		/// </summary>
		public ObservableCollection<Widget> Widgets { get; } = new ObservableCollection<Widget>();

		/// <summary>
		/// Gets the internal bounds of the desktop.
		/// </summary>
		internal Rectangle InternalBounds
		{
			get => _lastBounds;

			set
			{
				if (_lastBounds == value)
				{
					return;
				}

				_lastBounds = value;

				InvalidateLayout();
				InvalidateTransform();
			}
		}

		internal Rectangle LayoutBounds
		{
			get
			{
				return new Rectangle(0, 0, _lastBounds.Width, _lastBounds.Height);
			}
		}

		/// <summary>
		/// Gets the currently displayed context menu widget, if any.
		/// </summary>
		public Widget ContextMenu { get; private set; }

		/// <summary>
		/// Gets the currently displayed tooltip widget, if any.
		/// </summary>
		public Widget Tooltip { get; private set; }

		/// <summary>
		/// Widget having keyboard focus
		/// </summary>
		public Widget FocusedKeyboardWidget
		{
			get { return _focusedKeyboardWidget; }

			set
			{
				if (value == _focusedKeyboardWidget)
				{
					return;
				}

				var oldValue = _focusedKeyboardWidget;
				if (oldValue != null)
				{
					if (WidgetLosingKeyboardFocus != null)
					{
						var args = new CancellableEventArgs<Widget>(oldValue);
						WidgetLosingKeyboardFocus(null, args);
						if (oldValue.IsPlaced && args.Cancel)
						{
							return;
						}
					}
				}

				_focusedKeyboardWidget = value;
				if (oldValue != null)
				{
					oldValue.OnLostKeyboardFocus();
				}

				if (_focusedKeyboardWidget != null)
				{
					_focusedKeyboardWidget.OnGotKeyboardFocus();
					WidgetGotKeyboardFocus.Invoke(_focusedKeyboardWidget);
				}
			}
		}

		/// <summary>
		/// Gets or sets the opacity of the desktop (0-1 range).
		/// </summary>
		public float Opacity { get; set; }

		/// <summary>
		/// Gets or sets the scale of the desktop.
		/// </summary>
		public Vector2 Scale
		{
			get => _scale;
			set
			{
				if (value.EpsilonEquals(_scale))
				{
					return;
				}

				_scale = value;
				InvalidateTransform();
			}

		}

		/// <summary>
		/// Gets or sets the origin point for rotation and scaling transformations (0-1 normalized coordinates).
		/// </summary>
		public Vector2 TransformOrigin
		{
			get => _transformOrigin;
			set
			{
				if (value.EpsilonEquals(_transformOrigin))
				{
					return;
				}

				_transformOrigin = value;
				InvalidateTransform();
			}
		}

		/// <summary>
		/// Gets or sets the rotation angle of the desktop in radians.
		/// </summary>
		public float Rotation
		{
			get => _rotation;

			set
			{
				if (value.EpsilonEquals(_rotation))
				{
					return;
				}

				_rotation = value;
				InvalidateTransform();
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

		/// <summary>
		/// Gets a value indicating whether the mouse pointer is over a GUI widget.
		/// </summary>
		public bool IsMouseOverGUI
		{
			get
			{
				return IsPointOverGUI(MousePosition);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the touch position is over a GUI widget.
		/// </summary>
		public bool IsTouchOverGUI
		{
			get
			{
				return TouchPosition != null && IsPointOverGUI(TouchPosition.Value);
			}
		}

		internal bool IsShiftDown
		{
			get
			{
				return IsKeyDown(Keys.LeftShift) || IsKeyDown(Keys.RightShift);
			}
		}

		internal bool IsControlDown
		{
			get
			{
#if !STRIDE
				return IsKeyDown(Keys.LeftControl) || IsKeyDown(Keys.RightControl);
#else
				return IsKeyDown(Keys.LeftCtrl) || IsKeyDown(Keys.RightCtrl);
#endif
			}
		}

		internal bool IsAltDown
		{
			get
			{
#if !STRIDE
				return IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt);
#else
				return IsKeyDown(Keys.LeftAlt) || IsKeyDown(Keys.RightAlt);
#endif
			}
		}

		/// <summary>
		/// Gets a value indicating whether a modal widget is currently active on the desktop.
		/// </summary>
		public bool HasModalWidget
		{
			get
			{
				var childrenCopy = ChildrenCopy;
				for (var i = childrenCopy.Count - 1; i >= 0; --i)
				{
					var w = childrenCopy[i];
					if (w.Visible && w.Enabled && w.IsModal)
					{
						return true;
					}
				}

				return false;
			}
		}

		private bool IsMenuBarActive
		{
			get
			{
				return MenuBar != null && (MenuBar.OpenMenuItem != null || IsAltDown);
			}
		}

		/// <summary>
		/// Gets or sets the background brush for the desktop.
		/// </summary>
		public IBrush Background { get; set; }

		/// <summary>
		/// Raised when a context menu is about to be closed.
		/// </summary>
		public event EventHandler<CancellableEventArgs<Widget>> ContextMenuClosing;

		/// <summary>
		/// Raised when a context menu is closed.
		/// </summary>
		public event EventHandler<GenericEventArgs<Widget>> ContextMenuClosed;

		/// <summary>
		/// Raised when a widget is about to lose keyboard focus.
		/// </summary>
		public event EventHandler<CancellableEventArgs<Widget>> WidgetLosingKeyboardFocus;

		/// <summary>
		/// Raised when a widget receives keyboard focus.
		/// </summary>
		public event EventHandler<GenericEventArgs<Widget>> WidgetGotKeyboardFocus;

		/// <summary>
		/// Gets or sets the handler function for keyboard key down events.
		/// </summary>
		public Action<Keys> KeyDownHandler;

		public Desktop()
		{
			Opacity = 1.0f;
			Widgets.CollectionChanged += WidgetsOnCollectionChanged;
			KeyDownHandler = OnKeyDown;

#if FNA
			TextInputEXT.StartTextInput();
			TextInputEXT.TextInput += OnChar;
#endif

			if (Stylesheet.Current.DesktopStyle != null)
			{
				Background = Stylesheet.Current.DesktopStyle.Background;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the specified keyboard key is currently pressed.
		/// </summary>
		/// <param name="keys">The key to check.</param>
		/// <returns>True if the key is pressed, false otherwise.</returns>
		public bool IsKeyDown(Keys keys)
		{
			return _downKeys[(int)keys];
		}

		/// <summary>
		/// Gets the child widget at the specified index.
		/// </summary>
		/// <param name="index">The index of the child widget.</param>
		/// <returns>The child widget at the specified index.</returns>
		public Widget GetChild(int index)
		{
			return ChildrenCopy[index];
		}

		private void InputOnTouchDown()
		{
			if (ContextMenu != null && !ContextMenu.IsTouchInside)
			{
				var ev = ContextMenuClosing;
				if (ev != null)
				{
					var args = new CancellableEventArgs<Widget>(ContextMenu);
					ev(null, args);

					if (args.Cancel)
					{
						return;
					}
				}
				HideContextMenu();
			}

			HideTooltip();
		}

		/// <summary>
		/// Hides the currently displayed context menu.
		/// </summary>
		public void HideContextMenu()
		{
			if (ContextMenu == null)
			{
				return;
			}

			Widgets.Remove(ContextMenu);
			ContextMenu.Visible = false;

			ContextMenuClosed.Invoke(ContextMenu);
			ContextMenu = null;

			if (_previousKeyboardFocus != null)
			{
				FocusedKeyboardWidget = _previousKeyboardFocus;
				_previousKeyboardFocus = null;
			}
		}

		private void FixOverWidgetPosition(Widget widget, Point position)
		{
			widget.HorizontalAlignment = HorizontalAlignment.Left;
			widget.VerticalAlignment = VerticalAlignment.Top;

			var measure = widget.Measure(LayoutBounds.Size());

			if (position.X + measure.X > LayoutBounds.Right)
			{
				position.X = LayoutBounds.Right - measure.X;
			}

			if (position.Y + measure.Y > LayoutBounds.Bottom)
			{
				position.Y = LayoutBounds.Bottom - measure.Y;
			}

			widget.Left = position.X;
			widget.Top = position.Y;
		}

		/// <summary>
		/// Shows the context menu
		/// </summary>
		/// <param name="menu">Widget to show</param>
		/// <param name="position">Show position in the global coordinates</param>
		public void ShowContextMenu(Widget menu, Point position)
		{
			HideContextMenu();

			ContextMenu = menu;
			if (ContextMenu == null)
			{
				return;
			}

			position = ToLocal(position);
			FixOverWidgetPosition(menu, position);

			ContextMenu.Visible = true;
			Widgets.Add(ContextMenu);

			if (ContextMenu.AcceptsKeyboardFocus)
			{
				_previousKeyboardFocus = FocusedKeyboardWidget;
				FocusedKeyboardWidget = ContextMenu;
			}
		}

		/// <summary>
		/// Hides the currently displayed tooltip.
		/// </summary>
		public void HideTooltip()
		{
			if (Tooltip == null)
			{
				return;
			}

			Widgets.Remove(Tooltip);
			Tooltip.Visible = false;
			Tooltip = null;
		}

		/// <summary>
		/// Shows a tooltip for the specified widget at the given position.
		/// </summary>
		/// <param name="widget">The widget to show the tooltip for.</param>
		/// <param name="position">The position where the tooltip should be displayed.</param>
		public void ShowTooltip(Widget widget, Point position)
		{
			if (string.IsNullOrEmpty(widget.Tooltip))
			{
				return;
			}

			HideTooltip();
			Tooltip = MyraEnvironment.TooltipCreator(widget);
			if (Tooltip == null)
			{
				return;
			}

			FixOverWidgetPosition(Tooltip, position);

			Tooltip.Visible = true;
			Widgets.Add(Tooltip);
		}

		private void WidgetsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (Widget w in args.NewItems)
				{
					w.Desktop = this;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (Widget w in args.OldItems)
				{
					w.Desktop = null;
				}
			}
			else if (args.Action == NotifyCollectionChangedAction.Reset)
			{
				foreach (Widget w in ChildrenCopy)
				{
					w.Desktop = null;
				}
			}

			InvalidateLayout();
			_widgetsDirty = true;
		}

		/// <summary>
		/// Renders the visual content of the desktop and all its widgets.
		/// </summary>
		public void RenderVisual()
		{
			var oldDeviceScissor = _renderContext.DeviceScissor;

			_renderContext.Begin();

			_renderContext.Transform = Transform;

			if (Rotation.IsZero())
			{
				var bounds = Transform.Apply(LayoutBounds);
				_renderContext.Scissor = bounds;
			}

			_renderContext.Opacity = Opacity;

			if (Background != null)
			{
				Background.Draw(_renderContext, LayoutBounds);
			}

			foreach (var widget in ChildrenCopy)
			{
				if (widget.Visible)
				{

					if (MyraEnvironment.EnableModalDarkening && widget.IsModal)
					{
						_renderContext.FillRectangle(LayoutBounds, MyraEnvironment.DarkeningColor);
					}

					widget.Render(_renderContext);
				}
			}

			_renderContext.End();

			_renderContext.DeviceScissor = oldDeviceScissor;
		}

		/// <summary>
		/// Performs a complete update cycle: layout, input processing, and rendering of the desktop.
		/// </summary>
		public void Render()
		{
			// Layout run
			UpdateLayout();

			// First input run: set Desktop/Widgets input states and schedule input events
			UpdateInput();

			_inputContext.Reset();

			var childrenCopy = ChildrenCopy;
			for (var i = childrenCopy.Count - 1; i >= 0; --i)
			{
				var widget = childrenCopy[i];
				widget.ProcessInput(_inputContext);
			}

			// Only one widget at a time can receive mouse wheel event
			// So scheduling it here
			if (_inputContext.MouseWheelWidget != null)
			{
				InputEventsManager.Queue(_inputContext.MouseWheelWidget, InputEventType.MouseWheel);
			}

			// Second input run: process input events
			InputEventsManager.ProcessEvents();

			// Do another layout run, since an input event could cause the layout change
			UpdateLayout();

			// Render run
			RenderVisual();
		}

		private void InvalidateTransform()
		{
			_transformDirty = true;

			foreach (var child in ChildrenCopy)
			{
				child.InvalidateTransform();
			}
		}

		/// <summary>
		/// Converts a local position to global (screen) coordinates.
		/// </summary>
		/// <param name="pos">The local position to convert.</param>
		/// <returns>The position in global coordinates.</returns>
		public Vector2 ToGlobal(Vector2 pos)
		{
			UpdateTransform();

			return Transform.Apply(pos);
		}

		/// <summary>
		/// Converts a local position to global (screen) coordinates.
		/// </summary>
		/// <param name="pos">The local position to convert.</param>
		/// <returns>The position in global coordinates.</returns>
		public Point ToGlobal(Point pos)
		{
			UpdateTransform();

			return Transform.Apply(pos);
		}

		/// <summary>
		/// Converts a global (screen) position to local coordinates.
		/// </summary>
		/// <param name="pos">The global position to convert.</param>
		/// <returns>The position in local coordinates.</returns>
		public Vector2 ToLocal(Vector2 pos)
		{
			UpdateTransform();

			return Transform.InverseApply(pos);
		}

		/// <summary>
		/// Converts a global (screen) position to local coordinates.
		/// </summary>
		/// <param name="pos">The global position to convert.</param>
		/// <returns>The position in local coordinates.</returns>
		public Point ToLocal(Point pos) => ToLocal(new Vector2(pos.X, pos.Y)).ToPoint();

		/// <summary>
		/// Marks the layout as dirty, requiring recalculation on the next update.
		/// </summary>
		public void InvalidateLayout()
		{
			_layoutDirty = true;
		}

		/// <summary>
		/// Updates the layout of all widgets on the desktop.
		/// </summary>
		public void UpdateLayout()
		{
			var bounds = BoundsFetcher();
			InternalBounds = bounds;
			if (bounds.IsEmpty)
			{
				return;
			}

			if (!_layoutDirty)
			{
				return;
			}

			foreach (var child in ChildrenCopy)
			{
				if (child.Visible)
				{
					child.Arrange(LayoutBounds);
				}
			}

			// Rest processing
			MenuBar = null;

			var childrenCopy = ChildrenCopy;
			for (var i = childrenCopy.Count - 1; i >= 0; --i)
			{
				var w = childrenCopy[i];
				if (!w.Visible)
				{
					continue;
				}

				MenuBar = w.FindChild<HorizontalMenu>();
				if (MenuBar != null)
				{
					break;
				}
			}

			UpdateRecursiveLayout(ChildrenCopy);

			_layoutDirty = false;
		}

		internal void ProcessWidgets(Func<Widget, bool> operation)
		{
			foreach (var w in ChildrenCopy)
			{
				var result = w.ProcessWidgets(operation);
				if (!result)
				{
					return;
				}
			}
		}

		private void UpdateRecursiveLayout(IEnumerable<Widget> widgets)
		{
			foreach (var i in widgets)
			{
				if (!i.Layout2d.Nullable)
				{
					ExpressionParser.Parse(i, ChildrenCopy);
				}

				UpdateRecursiveLayout(i.ChildrenCopy);
			}
		}

		private Widget FindChild(Widget root, Func<Widget, bool> predicate)
		{
			if (predicate(root))
			{
				return root;
			}

			foreach (var w in root.ChildrenCopy)
			{
				var result = w.FindChild(predicate);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		/// <summary>
		/// Finds a child widget that matches the specified predicate.
		/// </summary>
		/// <param name="filter">A predicate function to filter widgets.</param>
		/// <returns>The first widget that matches the predicate, or null if none found.</returns>
		public Widget FindChild(Func<Widget, bool> filter)
		{
			foreach (var w in ChildrenCopy)
			{
				var result = FindChild(w, filter);
				if (result != null)
				{
					return result;
				}
			}

			return null;
		}

		[Obsolete("Use FindChild")]
		public Widget GetWidgetBy(Func<Widget, bool> predicate) => FindChild(predicate);

		/// <summary>
		/// Finds a child widget with the specified identifier.
		/// </summary>
		/// <param name="id">The identifier of the widget to find.</param>
		/// <returns>The widget with the specified identifier, or null if not found.</returns>
		public Widget FindChild(string id)
		{
			return FindChild(w => w.Id == id);
		}

		[Obsolete("Use FindChild")]
		public Widget GetWidgetByID(string ID)
		{
			return FindChild(w => w.Id == ID);
		}

		/// <summary>
		/// Calculates the total number of widgets including all child widgets.
		/// </summary>
		/// <param name="visibleOnly">If true, only counts visible widgets. If false, counts all widgets.</param>
		/// <returns>The total number of widgets.</returns>
		public int CalculateTotalWidgets(bool visibleOnly)
		{
			var result = 0;
			foreach (var w in Widgets)
			{
				if (visibleOnly && !w.Visible)
				{
					continue;
				}

				++result;

				result += w.CalculateTotalChildCount(visibleOnly);
			}

			return result;
		}

		private void FocusNextWidget()
		{
			if (Widgets.Count == 0) return;

			var isNull = FocusedKeyboardWidget == null;
			var focusChanged = false;
			ProcessWidgets(w =>
			{
				if (isNull)
				{
					if (CanFocusWidget(w))
					{
						w.SetKeyboardFocus();
						focusChanged = true;
						return false;
					}
				}
				else
				{
					if (w == FocusedKeyboardWidget)
					{
						isNull = true;
						// Next widget will be focused
					}
				}

				return true;
			});

			if (focusChanged || FocusedKeyboardWidget == null)
			{
				// Either new focus had been set or there are no focusable widgets
				return;
			}

			// Next run - try to focus first widget before focused one
			ProcessWidgets(w =>
			{
				if (CanFocusWidget(w))
				{
					w.SetKeyboardFocus();
					return false;
				}

				return true;
			});
		}

		private static bool CanFocusWidget(Widget widget) =>
			widget != null && widget.Visible &&
			widget.Enabled && widget.AcceptsKeyboardFocus;

		/// <summary>
		/// Handles keyboard key down events.
		/// </summary>
		/// <param name="key">The keyboard key that was pressed.</param>
		public void OnKeyDown(Keys key)
		{
			KeyDown.Invoke(key);

			if (IsMenuBarActive)
			{
				MenuBar.OnKeyDown(key);
			}
			else
			{
				if (_focusedKeyboardWidget != null)
				{
					_focusedKeyboardWidget.OnKeyDown(key);

#if STRIDE
					var ch = key.ToChar(IsKeyDown(Keys.LeftShift) ||
										IsKeyDown(Keys.RightShift));
					if (ch != null)
					{
						_focusedKeyboardWidget.OnChar(ch.Value);
					}
#elif MONOGAME || PLATFORM_AGNOSTIC
					if (!HasExternalTextInput && !IsControlDown && !IsAltDown)
					{
						var c = key.ToChar(IsShiftDown);
						if (c != null)
						{
							OnChar(c.Value);
						}
					}
#endif
				}
			}

			if (key == Keys.Escape && ContextMenu != null)
			{
				HideContextMenu();
			}
		}

		/// <summary>
		/// Handles character input events.
		/// </summary>
		/// <param name="c">The character that was input.</param>
		public void OnChar(char c)
		{
			if (IsMenuBarActive)
			{
				// Don't accept chars if menubar is open
				return;
			}

			if (_focusedKeyboardWidget != null)
			{
				_focusedKeyboardWidget.OnChar(c);
			}

			Char.Invoke(c);
		}

		private void UpdateWidgetsCopy()
		{
			if (!_widgetsDirty)
			{
				return;
			}

			_widgetsCopy.Clear();
			_widgetsCopy.AddRange(Widgets);

			_widgetsCopy.SortWidgetsByZIndex();

			_widgetsDirty = false;
		}

		/// <summary>
		/// Determines whether the specified point is over a GUI widget.
		/// </summary>
		/// <param name="p">The point to check.</param>
		/// <returns>True if the point is over a widget, false otherwise.</returns>
		public bool IsPointOverGUI(Point p)
		{
			foreach (var widget in ChildrenCopy)
			{
				var result = widget.HitTest(p);
				if (result != null)
				{
					return true;
				}
			}

			return false;
		}

		private void UpdateTransform()
		{
			if (!_transformDirty)
			{
				return;
			}

			var bounds = InternalBounds;
			_transform = new Transform(bounds.Location.ToVector2(),
				TransformOrigin * bounds.Size().ToVector2(),
				Scale,
				Rotation * (float)Math.PI / 180);

			_transformDirty = false;
		}

		private void ReleaseUnmanagedResources()
		{
			_renderContext.Dispose();
		}

		public void Dispose()
		{
			if (_isDisposed)
				return;

#if FNA
			TextInputEXT.TextInput -= OnChar;
#endif

			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~Desktop()
		{
			ReleaseUnmanagedResources();
		}

		/// <summary>
		/// Default bounds fetcher that returns the viewport bounds.
		/// </summary>
		/// <returns>A rectangle representing the viewport bounds.</returns>
		public static Rectangle DefaultBoundsFetcher()
		{
			var size = CrossEngineStuff.ViewSize;

			return new Rectangle(0, 0, size.X, size.Y);
		}
	}
}