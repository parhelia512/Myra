using Myra.Utility;
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Myra.Events;

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
#endif

namespace Myra.Graphics2D.UI
{
	partial class Widget : IInputEventsProcessor
	{
		private DateTime? _lastTouchDown;
		private DateTime? _lastMouseMovement;
		private Point _lastLocalTouchPosition;
		private Point? _localMousePosition;
		private Point? _localTouchPosition;

		/// <summary>
		/// Gets a value indicating whether the mouse pointer is currently inside this widget.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool IsMouseInside => _localMousePosition != null;

		/// <summary>
		/// Gets or sets the local coordinates of the mouse pointer relative to this widget, or null if the mouse is not over the widget.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Point? LocalMousePosition
		{
			get => _localMousePosition;
			private set
			{
				if (value == _localMousePosition)
				{
					return;
				}

				var oldValue = _localMousePosition;
				_localMousePosition = value;

				if (Desktop == null)
				{
					return;
				}

				if (value != null && oldValue == null)
				{
					InputEventsManager.Queue(this, InputEventType.MouseEntered);
				}
				else if (value == null && oldValue != null)
				{
					InputEventsManager.Queue(this, InputEventType.MouseLeft);
				}
				else if (value != null && oldValue != null && value.Value != oldValue.Value)
				{
					InputEventsManager.Queue(this, InputEventType.MouseMoved);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether a touch point is currently inside this widget.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public bool IsTouchInside => _localTouchPosition != null;

		/// <summary>
		/// Gets or sets the local coordinates of the touch point relative to this widget, or null if there is no active touch on the widget.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		public Point? LocalTouchPosition
		{
			get => _localTouchPosition;
			private set
			{
				if (value == _localTouchPosition)
				{
					return;
				}

				var oldValue = _localTouchPosition;
				_localTouchPosition = value;

				if (Desktop == null)
				{
					return;
				}

				if (value != null && oldValue == null)
				{
					if (Desktop.PreviousTouchPosition == null)
					{
						// Touch Down Event
						InputEventsManager.Queue(this, InputEventType.TouchDown);
						ProcessDoubleClick(value.Value);
					}
					else
					{
						// Touch Entered
						InputEventsManager.Queue(this, InputEventType.TouchEntered);
					}
				}
				else if (value == null && oldValue != null)
				{
					if (Desktop.TouchPosition == null)
					{
						InputEventsManager.Queue(this, InputEventType.TouchUp);
					}
					else
					{
						InputEventsManager.Queue(this, InputEventType.TouchLeft);
					}
				}
				else if (value != null && oldValue != null && value.Value != oldValue.Value)
				{
					InputEventsManager.Queue(this, InputEventType.TouchMoved);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this widget accepts mouse wheel input.
		/// </summary>
		protected internal virtual bool AcceptsMouseWheel => false;

		/// <summary>
		/// Occurs when the widget's position has changed.
		/// </summary>
		public event MyraEventHandler PlacedChanged;

		/// <summary>
		/// Occurs when the widget's visibility has changed.
		/// </summary>
		public event MyraEventHandler VisibleChanged;

		/// <summary>
		/// Occurs when the widget's enabled state has changed.
		/// </summary>
		public event MyraEventHandler EnabledChanged;

		/// <summary>
		/// Occurs when the widget's location has changed.
		/// </summary>
		public event MyraEventHandler LocationChanged;

		/// <summary>
		/// Occurs when the widget's size has changed.
		/// </summary>
		public event MyraEventHandler SizeChanged;

		/// <summary>
		/// Occurs when the widget's layout arrangement has been updated.
		/// </summary>
		public event MyraEventHandler ArrangeUpdated;

		/// <summary>
		/// Occurs when the mouse pointer leaves the widget.
		/// </summary>
		public event MyraEventHandler MouseLeft;

		/// <summary>
		/// Occurs when the mouse pointer enters the widget.
		/// </summary>
		public event MyraEventHandler MouseEntered;

		/// <summary>
		/// Occurs when the mouse pointer moves within the widget.
		/// </summary>
		public event MyraEventHandler MouseMoved;

		/// <summary>
		/// Occurs when a touch point leaves the widget.
		/// </summary>
		public event MyraEventHandler TouchLeft;

		/// <summary>
		/// Occurs when a touch point enters the widget.
		/// </summary>
		public event MyraEventHandler TouchEntered;

		/// <summary>
		/// Occurs when a touch point moves within the widget.
		/// </summary>
		public event MyraEventHandler TouchMoved;

		/// <summary>
		/// Occurs when a touch point is pressed on the widget.
		/// </summary>
		public event MyraEventHandler TouchDown;

		/// <summary>
		/// Occurs when a touch point is released on the widget.
		/// </summary>
		public event MyraEventHandler TouchUp;

		/// <summary>
		/// Occurs when the widget receives a double-tap touch event.
		/// </summary>
		public event MyraEventHandler TouchDoubleClick;

		/// <summary>
		/// Occurs when the keyboard focus on the widget has changed.
		/// </summary>
		public event MyraEventHandler KeyboardFocusChanged;

		/// <summary>
		/// Occurs when the mouse wheel is scrolled while over the widget.
		/// </summary>
		public event MyraEventHandler<GenericEventArgs<float>> MouseWheelChanged;

		/// <summary>
		/// Occurs when a key is released while the widget has keyboard focus.
		/// </summary>
		public event MyraEventHandler<GenericEventArgs<Keys>> KeyUp;

		/// <summary>
		/// Occurs when a key is pressed while the widget has keyboard focus.
		/// </summary>
		public event MyraEventHandler<GenericEventArgs<Keys>> KeyDown;

		/// <summary>
		/// Occurs when a character is entered while the widget has keyboard focus.
		/// </summary>
		public event MyraEventHandler<GenericEventArgs<char>> Char;

		private void ProcessDoubleClick(Point touchPos)
		{
			if (_lastTouchDown != null &&
				(DateTime.Now - _lastTouchDown.Value).TotalMilliseconds < MyraEnvironment.DoubleClickIntervalInMs &&
				Math.Abs(touchPos.X - _lastLocalTouchPosition.X) <= MyraEnvironment.DoubleClickRadius &&
				Math.Abs(touchPos.Y - _lastLocalTouchPosition.Y) <= MyraEnvironment.DoubleClickRadius)
			{
				_lastTouchDown = null;
				InputEventsManager.Queue(this, InputEventType.TouchDoubleClick);
			}
			else
			{
				_lastTouchDown = DateTime.Now;
				_lastLocalTouchPosition = LocalTouchPosition.Value;
			}
		}

		/// <summary>
		/// Processes input events for this widget, including mouse and touch input.
		/// </summary>
		/// <param name="inputContext">The input context containing the current input state.</param>
		protected internal virtual void ProcessInput(InputContext inputContext)
		{
			if (!Visible || Desktop == null)
			{
				return;
			}

			if (!inputContext.MouseOrTouchHandled)
			{
				var oldContainsMouse = inputContext.ParentContainsMouse;
				var oldContainsTouch = inputContext.ParentContainsTouch;

				if (!Desktop.IsMobile)
				{
					if (inputContext.ParentContainsMouse)
					{
						if (ContainsGlobalPoint(Desktop.MousePosition))
						{
							LocalMousePosition = ToLocal(Desktop.MousePosition);
						}
						else
						{
							LocalMousePosition = null;
							inputContext.ParentContainsMouse = false;
						}
					}
					else
					{
						LocalMousePosition = null;
					}
				}

				if (Desktop.TouchPosition != null && inputContext.ParentContainsTouch)
				{
					if (ContainsGlobalPoint(Desktop.TouchPosition.Value))
					{
						LocalTouchPosition = ToLocal(Desktop.TouchPosition.Value);
					}
					else
					{
						LocalTouchPosition = null;
						inputContext.ParentContainsTouch = false;
					}
				}
				else
				{
					LocalTouchPosition = null;
				}

				if (IsMouseInside &&
					!Desktop.MouseWheelDelta.IsZero() &&
					AcceptsMouseWheel)
				{
					inputContext.MouseWheelWidget = this;
				}

				for (var i = _childrenCopy.Count - 1; i >= 0; i--)
				{
					var child = _childrenCopy[i];
					child.ProcessInput(inputContext);
				}

				if (IsModal)
				{
					// Modal widget prevents all further input processing
					inputContext.MouseOrTouchHandled = true;
				}
				else
				{
					if (!Desktop.IsMobile)
					{
						if (IsMouseInside && !InputFallsThrough(LocalMousePosition.Value))
						{
							inputContext.MouseOrTouchHandled = true;
						}
					}
					else
					{
						if (IsTouchInside && !InputFallsThrough(LocalTouchPosition.Value))
						{
							inputContext.MouseOrTouchHandled = true;
						}
					}
				}

				inputContext.ParentContainsMouse = oldContainsMouse;
				inputContext.ParentContainsTouch = oldContainsTouch;
			}
			else
			{
				if (!Desktop.IsMobile)
				{
					LocalMousePosition = null;
				}

				LocalTouchPosition = null;

				for (var i = _childrenCopy.Count - 1; i >= 0; i--)
				{
					var child = _childrenCopy[i];
					child.ProcessInput(inputContext);
				}
			}
		}

		void IInputEventsProcessor.ProcessEvent(InputEventType eventType)
		{
			// It's important to note that widget should process input events even if Desktop is null
			// Just add corresponding null checks in that case

			switch (eventType)
			{
				case InputEventType.MouseLeft:
					if (Desktop != null && Desktop.Tooltip != null && Desktop.Tooltip.Tag == this)
					{
						// Tooltip for this widget is shown
						Desktop.HideTooltip();
					}

					_lastMouseMovement = null;

                    if (MyraEnvironment.SetMouseCursorFromWidget && MouseCursor != null)
                    {
                        Widget ancestor = Parent;
                        while (ancestor != null && !ancestor.IsMouseInside)
                        {
                            ancestor = ancestor.Parent;
                        }

                        if (ancestor != null && ancestor.MouseCursor != null)
                        {
                            MyraEnvironment.MouseCursorType = ancestor.MouseCursor.Value;
                        }
                        else
                        {
                            MyraEnvironment.MouseCursorType = MyraEnvironment.DefaultMouseCursorType;
                        }
                    }

                    OnMouseLeft();
					MouseLeft.Invoke(this, InputEventType.MouseLeft);
					break;
				case InputEventType.MouseEntered:
					_lastMouseMovement = DateTime.Now;
					if (MyraEnvironment.SetMouseCursorFromWidget && MouseCursor != null)
					{
						MyraEnvironment.MouseCursorType = MouseCursor.Value;
					}

					OnMouseEntered();
					MouseEntered.Invoke(this, InputEventType.MouseEntered);
					break;
				case InputEventType.MouseMoved:
					_lastMouseMovement = DateTime.Now;
					OnMouseMoved();
					MouseMoved.Invoke(this, InputEventType.MouseMoved);
					break;
				case InputEventType.MouseWheel:
					if (Desktop != null)
					{
						OnMouseWheel(Desktop.MouseWheelDelta);

						// Add yet another null check, since OnMouseWheel call might nullify the Desktop
						if (Desktop != null)
						{
							MouseWheelChanged.Invoke(this, Desktop.MouseWheelDelta, InputEventType.MouseWheel);
						}
					}
					break;
				case InputEventType.TouchLeft:
					OnTouchLeft();
					TouchLeft.Invoke(this, InputEventType.TouchLeft);
					break;
				case InputEventType.TouchEntered:
					OnTouchEntered();
					TouchEntered.Invoke(this, InputEventType.TouchEntered);
					break;
				case InputEventType.TouchMoved:
					OnTouchMoved();
					TouchMoved.Invoke(this, InputEventType.TouchMoved);
					break;
				case InputEventType.TouchDown:
					if (Desktop != null)
					{
						if (Enabled && AcceptsKeyboardFocus)
						{
							Desktop.FocusedKeyboardWidget = this;
						}

						if (DragHandle != null && DragHandle.IsTouchInside)
						{
							var parent = Parent != null ? (ITransformable)Parent : Desktop;
							_startPos = parent.ToLocal(new Vector2(Desktop.TouchPosition.Value.X, Desktop.TouchPosition.Value.Y));
							_startLeftTop = new Point(Left, Top);
						}
					}

					OnTouchDown();
					TouchDown.Invoke(this, InputEventType.TouchDown);
					break;
				case InputEventType.TouchUp:
					OnTouchUp();
					TouchUp.Invoke(this, InputEventType.TouchUp);
					break;
				case InputEventType.TouchDoubleClick:
					OnTouchDoubleClick();
					TouchDoubleClick.Invoke(this, InputEventType.TouchDoubleClick);
					break;
			}
		}

		/// <summary>
		/// Called when the mouse pointer leaves the widget.
		/// </summary>
		public virtual void OnMouseLeft()
		{
		}

		/// <summary>
		/// Called when the mouse pointer enters the widget.
		/// </summary>
		public virtual void OnMouseEntered()
		{
		}

		/// <summary>
		/// Called when the mouse pointer moves within the widget.
		/// </summary>
		public virtual void OnMouseMoved()
		{
		}

		/// <summary>
		/// Called when the mouse wheel is scrolled while over the widget.
		/// </summary>
		/// <param name="delta">The scroll delta value.</param>
		public virtual void OnMouseWheel(float delta)
		{
		}

		/// <summary>
		/// Called when a touch point leaves the widget.
		/// </summary>
		public virtual void OnTouchLeft()
		{
		}

		/// <summary>
		/// Called when a touch point enters the widget.
		/// </summary>
		public virtual void OnTouchEntered()
		{
		}

		/// <summary>
		/// Called when a touch point moves within the widget.
		/// </summary>
		public virtual void OnTouchMoved()
		{
		}

		/// <summary>
		/// Called when a touch point is pressed on the widget.
		/// </summary>
		public virtual void OnTouchDown()
		{
		}

		/// <summary>
		/// Called when a touch point is released on the widget.
		/// </summary>
		public virtual void OnTouchUp()
		{
		}

		/// <summary>
		/// Called when the widget receives a double-tap touch event.
		/// </summary>
		public virtual void OnTouchDoubleClick()
		{
		}
	}
}
