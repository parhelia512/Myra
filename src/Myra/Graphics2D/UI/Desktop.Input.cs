using Myra.Utility;
using System;
using Myra.Events;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
#if MONOGAME
using MonoGame.Framework.Utilities;
#endif
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
	/// Contains information about the current mouse state including position and button states.
	/// </summary>
	public struct MouseInfo
	{
		/// <summary>
		/// Gets or sets the current mouse position.
		/// </summary>
		public Point Position;

		/// <summary>
		/// Gets or sets a value indicating whether the left mouse button is currently pressed.
		/// </summary>
		public bool IsLeftButtonDown;

		/// <summary>
		/// Gets or sets a value indicating whether the middle mouse button is currently pressed.
		/// </summary>
		public bool IsMiddleButtonDown;

		/// <summary>
		/// Gets or sets a value indicating whether the right mouse button is currently pressed.
		/// </summary>
		public bool IsRightButtonDown;

		/// <summary>
		/// Gets or sets the mouse wheel delta value.
		/// </summary>
		public float Wheel;
	}

	partial class Desktop : IInputEventsProcessor
	{
		private MouseInfo _lastMouseInfo;
		private DateTime? _lastKeyDown;
		private int _keyDownCount = 0;
		private readonly bool[] _downKeys = new bool[0xff], _lastDownKeys = new bool[0xff];
		private Point _mousePosition;
		private Point? _touchPosition;
		private float _mouseWheelDelta;

		/// <summary>
		/// Gets the previous mouse position in global coordinates.
		/// </summary>
		public Point PreviousMousePosition { get; private set; }

		/// <summary>
		/// Gets the previous touch position in global coordinates, or null if there was no touch.
		/// </summary>
		public Point? PreviousTouchPosition { get; private set; }

		/// <summary>
		/// Gets or sets the current mouse position in global coordinates.
		/// </summary>
		public Point MousePosition
		{
			get => _mousePosition;
			private set
			{
				if (value == _mousePosition)
				{
					return;
				}

				_mousePosition = value;
				InputEventsManager.Queue(this, InputEventType.MouseMoved);
			}
		}

		/// <summary>
		/// Gets or sets the current touch position in global coordinates, or null if there is no touch.
		/// </summary>
		public Point? TouchPosition
		{
			get => _touchPosition;

			private set
			{
				if (value == _touchPosition)
				{
					return;
				}

				var oldValue = _touchPosition;
				_touchPosition = value;

				if (value != null && oldValue == null)
				{
					InputEventsManager.Queue(this, InputEventType.TouchDown);
				}
				else if (value == null && oldValue != null)
				{
					InputEventsManager.Queue(this, InputEventType.TouchUp);
				}
				else if (value != null && oldValue != null &&
					value.Value != oldValue.Value)
				{
					InputEventsManager.Queue(this, InputEventType.TouchMoved);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether a touch input is currently active.
		/// </summary>
		public bool IsTouchDown => TouchPosition != null;

		/// <summary>
		/// Gets or sets the mouse wheel delta value.
		/// </summary>
		public float MouseWheelDelta
		{
			get => _mouseWheelDelta;

			set
			{
				_mouseWheelDelta = value;

				if (!value.IsZero())
				{
					InputEventsManager.Queue(this, InputEventType.MouseWheel);
				}
			}
		}

		/// <summary>
		/// Gets an array indicating which keyboard keys are currently pressed.
		/// </summary>
		public bool[] DownKeys => _downKeys;

		/// <summary>
		/// Gets or sets the time in milliseconds before a key starts repeating. Default is 500ms.
		/// </summary>
		public int RepeatKeyDownStartInMs { get; set; } = 500;

		/// <summary>
		/// Gets or sets the interval in milliseconds between key repeats. Default is 50ms.
		/// </summary>
		public int RepeatKeyDownInternalInMs { get; set; } = 50;

		/// <summary>
		/// Gets a value indicating whether the platform is a mobile device.
		/// </summary>
		public static bool IsMobile
		{
			get
			{
#if MONOGAME
				return PlatformInfo.MonoGamePlatform == MonoGamePlatform.Android ||
					PlatformInfo.MonoGamePlatform == MonoGamePlatform.iOS;
#else
				return false;
#endif
			}
		}

		/// <summary>
		/// Raised when the mouse position changes.
		/// </summary>
		public event EventHandler MouseMoved;

		/// <summary>
		/// Raised when a touch moves.
		/// </summary>
		public event EventHandler TouchMoved;

		/// <summary>
		/// Raised when a touch input begins.
		/// </summary>
		public event EventHandler TouchDown;

		/// <summary>
		/// Raised when a touch input ends.
		/// </summary>
		public event EventHandler TouchUp;

		/// <summary>
		/// Raised when a double-click touch input occurs.
		/// </summary>
		public event EventHandler TouchDoubleClick;

		/// <summary>
		/// Raised when the mouse wheel is rotated.
		/// </summary>
		public event EventHandler<GenericEventArgs<float>> MouseWheelChanged;

		/// <summary>
		/// Raised when a keyboard key is released.
		/// </summary>
		public event EventHandler<GenericEventArgs<Keys>> KeyUp;

		/// <summary>
		/// Raised when a keyboard key is pressed.
		/// </summary>
		public event EventHandler<GenericEventArgs<Keys>> KeyDown;

		/// <summary>
		/// Raised when a character is input.
		/// </summary>
		public event EventHandler<GenericEventArgs<char>> Char;

		public void UpdateMouseInput()
		{
			if (MyraEnvironment.MouseInfoGetter == null)
			{
				return;
			}

			var mouseInfo = MyraEnvironment.MouseInfoGetter();

			var mousePos = mouseInfo.Position;

			// Mouse Position
			MousePosition = mousePos;

			// Touch Position
			Point? touchPosition = null;
			if (mouseInfo.IsLeftButtonDown || mouseInfo.IsRightButtonDown || mouseInfo.IsMiddleButtonDown)
			{
				// Touch by mouse
				touchPosition = MousePosition;
			}

			TouchPosition = touchPosition;

#if STRIDE
			var handleWheel = mouseInfo.Wheel != 0;
#else
			var handleWheel = mouseInfo.Wheel != _lastMouseInfo.Wheel;
#endif

			if (handleWheel)
			{
				var delta = mouseInfo.Wheel;
#if !STRIDE
				delta -= _lastMouseInfo.Wheel;
#endif
				MouseWheelDelta = delta;
			}
			else
			{
				MouseWheelDelta = 0;
			}

			_lastMouseInfo = mouseInfo;
		}

#if MONOGAME || FNA || PLATFORM_AGNOSTIC
		public void UpdateTouchInput()
		{
#if MONOGAME || FNA
			var touchState = TouchPanel.GetState();
#else
			var touchState = MyraEnvironment.Platform.GetTouchState();
#endif

			if (touchState.IsConnected && touchState.Count > 0)
			{
				var pos = touchState[0].Position;
				TouchPosition = new Point((int)pos.X, (int)pos.Y);
			}
			else
			{
				TouchPosition = null;
			}
		}
#endif

		public void UpdateKeyboardInput()
		{
			if (MyraEnvironment.DownKeysGetter == null)
			{
				return;
			}

			MyraEnvironment.DownKeysGetter(_downKeys);

			var now = DateTime.Now;
			for (var i = 0; i < _downKeys.Length; ++i)
			{
				var key = (Keys)i;
				if (_downKeys[i] && !_lastDownKeys[i])
				{
					if (key == Keys.Tab)
					{
						FocusNextWidget();
					}

					KeyDownHandler?.Invoke(key);

					_lastKeyDown = now;
					_keyDownCount = 0;
				}
				else if (!_downKeys[i] && _lastDownKeys[i])
				{
					// Key had been released
					KeyUp.Invoke(key);
					if (_focusedKeyboardWidget != null)
					{
						_focusedKeyboardWidget.OnKeyUp(key);
					}

					_lastKeyDown = null;
					_keyDownCount = 0;
				}
				else if (_downKeys[i] && _lastDownKeys[i])
				{
					if (_lastKeyDown != null &&
									  ((_keyDownCount == 0 && (now - _lastKeyDown.Value).TotalMilliseconds > RepeatKeyDownStartInMs) ||
									  (_keyDownCount > 0 && (now - _lastKeyDown.Value).TotalMilliseconds > RepeatKeyDownInternalInMs)))
					{
						KeyDownHandler?.Invoke(key);

						_lastKeyDown = now;
						++_keyDownCount;
					}
				}
			}

			Array.Copy(_downKeys, _lastDownKeys, _downKeys.Length);
		}

		public void UpdateInput()
		{
			UpdateKeyboardInput();

			PreviousMousePosition = MousePosition;
			PreviousTouchPosition = TouchPosition;

			if (!IsMobile)
			{
				UpdateMouseInput();
			}
			else
			{
#if MONOGAME || FNA || PLATFORM_AGNOSTIC
				try
				{
					UpdateTouchInput();
				}
				catch (Exception)
				{
				}
#endif
			}
		}

		void IInputEventsProcessor.ProcessEvent(InputEventType eventType)
		{
			switch (eventType)
			{
				case InputEventType.MouseLeft:
					break;
				case InputEventType.MouseEntered:
					break;
				case InputEventType.MouseMoved:
					MouseMoved.Invoke(this);
					break;
				case InputEventType.MouseWheel:
					MouseWheelChanged.Invoke(this, MouseWheelDelta);
					break;
				case InputEventType.TouchLeft:
					break;
				case InputEventType.TouchEntered:
					break;
				case InputEventType.TouchMoved:
					TouchMoved.Invoke(this);
					break;
				case InputEventType.TouchDown:
					InputOnTouchDown();
					TouchDown.Invoke(this);
					break;
				case InputEventType.TouchUp:
					TouchUp.Invoke(this);
					break;
				case InputEventType.TouchDoubleClick:
					TouchDoubleClick.Invoke(this);
					break;
			}
		}
	}
}