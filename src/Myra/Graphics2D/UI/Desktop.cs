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
	/// Represents the main UI desktop/screen that manages all widgets and handles user input.
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
		/// Gets or sets a value indicating whether external text input is available.
		/// </summary>
		public bool HasExternalTextInput = false;
#endif

		private bool _isDisposed = false;

		/// <summary>
		/// Gets or sets the function that fetches the bounds of the desktop.
		/// </summary>
		public Func<Rectangle> BoundsFetcher { get; set; } = DefaultBoundsFetcher;

		/// <summary>
		/// Gets or sets the root widget that covers the entire desktop.
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
		/// Gets the menu bar widget for the desktop.
		/// </summary>
		public HorizontalMenu MenuBar { get; private set; }

		internal List<Widget> ChildrenCopy
		{
			get
			{
				UpdateWidgetsCopy();
				return _widgetsCopy;
			}
		}

		/// <summary>
		/// Gets the collection of all widgets on the desktop.
		/// </summary>
		public ObservableCollection<Widget> Widgets { get; } = new ObservableCollection<Widget>();

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
		/// Gets the context menu widget currently displayed on the desktop.
		/// </summary>
		public Widget ContextMenu { get; private set; }

		/// <summary>
		/// Gets the tooltip widget currently displayed on the desktop.
		/// </summary>
		public Widget Tooltip { get; private set; }

		/// <summary>
		/// Gets or sets the widget that currently has keyboard focus.
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
						var args = new CancellableEventArgs<Widget>(oldValue, InputEventType.KeyboardFocusLosing);
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
					WidgetGotKeyboardFocus.Invoke(_focusedKeyboardWidget, InputEventType.KeyboardFocusLosing);
				}
			}
		}

		/// <summary>
		/// Gets or sets the opacity of the desktop (0.0 to 1.0).
		/// </summary>
		public float Opacity { get; set; }

		/// <summary>
		/// Gets or sets the scale factor for the desktop and all its widgets.
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
		/// Gets or sets the origin point for transformations (rotation and scale).
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
		/// Gets a value indicating whether the mouse cursor is over a GUI widget.
		/// </summary>
		public bool IsMouseOverGUI
		{
			get
			{
				return IsPointOverGUI(MousePosition);
			}
		}

		/// <summary>
		/// Gets a value indicating whether a touch point is over a GUI widget.
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
		/// Gets a value indicating whether any modal widget is currently displayed on the desktop.
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
		/// Occurs before a context menu is closed.
		/// </summary>
		public event MyraEventHandler<CancellableEventArgs<Widget>> ContextMenuClosing;

		/// <summary>
		/// Occurs after a context menu is closed.
		/// </summary>
		public event MyraEventHandler<GenericEventArgs<Widget>> ContextMenuClosed;

		/// <summary>
		/// Occurs before a widget loses keyboard focus.
		/// </summary>
		public event MyraEventHandler<CancellableEventArgs<Widget>> WidgetLosingKeyboardFocus;

		/// <summary>
		/// Occurs after a widget gets keyboard focus.
		/// </summary>
		public event MyraEventHandler<GenericEventArgs<Widget>> WidgetGotKeyboardFocus;

		/// <summary>
		/// Gets or sets the handler for keyboard key down events.
		/// </summary>
		public Action<Keys> KeyDownHandler;

		/// <summary>
		/// Initializes a new instance of the <see cref="Desktop"/> class.
		/// </summary>
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
		/// Determines whether a specific keyboard key is currently pressed.
		/// </summary>
		/// <param name="keys">The key to check.</param>
		/// <returns>true if the key is pressed; otherwise, false.</returns>
		public bool IsKeyDown(Keys keys)
		{
			return _downKeys[(int)keys];
		}

		/// <summary>
		/// Gets the child widget at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the child widget.</param>
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
					var args = new CancellableEventArgs<Widget>(ContextMenu, InputEventType.ContextMenuClosing);
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

			ContextMenuClosed.Invoke(ContextMenu, InputEventType.ContextMenuClosing);
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
		/// <param name="position">The position to show the tooltip in global coordinates.</param>
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
		/// Renders the visual representation of the desktop and all its widgets.
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
		/// Updates the layout of all widgets and processes input events.
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
		/// Converts a local coordinate to global (screen) coordinates.
		/// </summary>
		/// <param name="pos">The local position.</param>
		/// <returns>The position in global coordinates.</returns>
		public Vector2 ToGlobal(Vector2 pos)
		{
			UpdateTransform();

			return Transform.Apply(pos);
		}

		/// <summary>
		/// Converts a local coordinate to global (screen) coordinates.
		/// </summary>
		/// <param name="pos">The local position.</param>
		/// <returns>The position in global coordinates.</returns>
		public Point ToGlobal(Point pos)
		{
			UpdateTransform();

			return Transform.Apply(pos);
		}

		/// <summary>
		/// Converts a global (screen) coordinate to local coordinates.
		/// </summary>
		/// <param name="pos">The global position.</param>
		/// <returns>The position in local coordinates.</returns>
		public Vector2 ToLocal(Vector2 pos)
		{
			UpdateTransform();

			return Transform.InverseApply(pos);
		}

		/// <summary>
		/// Converts a global (screen) coordinate to local coordinates.
		/// </summary>
		/// <param name="pos">The global position.</param>
		/// <returns>The position in local coordinates.</returns>
		public Point ToLocal(Point pos) => ToLocal(new Vector2(pos.X, pos.Y)).ToPoint();

		/// <summary>
		/// Marks the layout as dirty, requiring an update on the next render.
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
		/// Finds the first child widget that matches the specified predicate, recursively searching all descendants.
		/// </summary>
		/// <param name="filter">A function to test each widget.</param>
		/// <returns>The first matching widget, or null if no match is found.</returns>
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

		/// <summary>
		/// Finds a widget by predicate. This method is obsolete; use <see cref="FindChild(Func{Widget, bool})"/> instead.
		/// </summary>
		/// <param name="predicate">A function to test each widget.</param>
		/// <returns>The first matching widget, or null if no match is found.</returns>
		[Obsolete("Use FindChild")]
		public Widget GetWidgetBy(Func<Widget, bool> predicate) => FindChild(predicate);

		/// <summary>
		/// Finds a child widget by its ID, recursively searching all descendants.
		/// </summary>
		/// <param name="id">The ID of the widget to find.</param>
		/// <returns>The widget with the specified ID, or null if not found.</returns>
		public Widget FindChild(string id)
		{
			return FindChild(w => w.Id == id);
		}

		/// <summary>
		/// Finds a widget by ID. This method is obsolete; use <see cref="FindChild(string)"/> instead.
		/// </summary>
		/// <param name="ID">The ID of the widget to find.</param>
		/// <returns>The widget with the specified ID, or null if not found.</returns>
		[Obsolete("Use FindChild")]
		public Widget GetWidgetByID(string ID)
		{
			return FindChild(w => w.Id == ID);
		}

		/// <summary>
		/// Calculates the total number of widgets on the desktop and all their descendants.
		/// </summary>
		/// <param name="visibleOnly">If true, only counts visible widgets.</param>
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
		/// Handles a keyboard key down event.
		/// </summary>
		/// <param name="key">The key that was pressed.</param>
		public void OnKeyDown(Keys key)
		{
			KeyDown.Invoke(key, InputEventType.KeyDown);

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
		/// Handles character input from the keyboard.
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

			Char.Invoke(c, InputEventType.CharInput);
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
		/// Determines whether a point is over a GUI widget.
		/// </summary>
		/// <param name="p">The point to test.</param>
		/// <returns>True if the point is over a GUI widget; otherwise, false.</returns>
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

		/// <summary>
		/// Releases all resources used by the desktop.
		/// </summary>
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

		/// <summary>
		/// Finalizer that releases unmanaged resources.
		/// </summary>
		~Desktop()
		{
			ReleaseUnmanagedResources();
		}

		/// <summary>
		/// Gets the default bounds of the desktop from the game window.
		/// </summary>
		/// <returns>A rectangle representing the desktop bounds.</returns>
		public static Rectangle DefaultBoundsFetcher()
		{
			var size = CrossEngineStuff.ViewSize;

			return new Rectangle(0, 0, size.X, size.Y);
		}
	}
}