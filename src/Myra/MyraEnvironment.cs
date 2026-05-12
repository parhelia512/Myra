using System;
using System.Reflection;
using Myra.Graphics2D.UI.Styles;
using Myra.Utility;
using AssetManagementBase;
using Myra.Graphics2D.UI;
using System.Collections.Generic;

#if MONOGAME || FNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#if FNA
using static SDL2.SDL;
using MouseCursor = System.Nullable<System.IntPtr>;
#endif

#elif STRIDE
using Stride.Engine;
using Stride.Graphics;
using Stride.Core.Mathematics;
using Stride.Input;
#else
using Myra.Platform;
using System.Drawing;
using Color = FontStashSharp.FSColor;
#endif

namespace Myra
{
	/// <summary>
	/// Provides global configuration and utility methods for the Myra UI framework.
	/// This class manages the game instance, asset manager, input handling, and various UI settings.
	/// </summary>
	public static class MyraEnvironment
	{
#if MONOGAME
		private static readonly Dictionary<MouseCursorType, MouseCursor> _mouseCursors = new Dictionary<MouseCursorType, MouseCursor>
		{
			[MouseCursorType.Arrow] = MouseCursor.Arrow,
			[MouseCursorType.IBeam] = MouseCursor.IBeam,
			[MouseCursorType.Wait] = MouseCursor.Wait,
			[MouseCursorType.Crosshair] = MouseCursor.Crosshair,
			[MouseCursorType.WaitArrow] = MouseCursor.WaitArrow,
			[MouseCursorType.SizeNWSE] = MouseCursor.SizeNWSE,
			[MouseCursorType.SizeNESW] = MouseCursor.SizeNESW,
			[MouseCursorType.SizeWE] = MouseCursor.SizeWE,
			[MouseCursorType.SizeNS] = MouseCursor.SizeNS,
			[MouseCursorType.SizeAll] = MouseCursor.SizeAll,
			[MouseCursorType.No] = MouseCursor.No,
			[MouseCursorType.Hand] = MouseCursor.Hand,
		};
#elif FNA
		private static readonly Dictionary<SDL_SystemCursor, IntPtr> _systemCursors = new Dictionary<SDL_SystemCursor, IntPtr>();

		private static IntPtr GetSystemCursor(SDL_SystemCursor type)
		{
			IntPtr result;
			if (_systemCursors.TryGetValue(type, out result))
			{
				return result;
			}

			result = SDL_CreateSystemCursor(type);
			_systemCursors[type] = result;

			return result;
		}

		private static readonly Dictionary<MouseCursorType, SDL_SystemCursor> _mouseCursors = new Dictionary<MouseCursorType, SDL_SystemCursor>
		{
			[MouseCursorType.Arrow] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_ARROW,
			[MouseCursorType.IBeam] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_IBEAM,
			[MouseCursorType.Wait] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_WAIT,
			[MouseCursorType.Crosshair] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_CROSSHAIR,
			[MouseCursorType.WaitArrow] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_WAITARROW,
			[MouseCursorType.SizeNWSE] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENWSE,
			[MouseCursorType.SizeNESW] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENESW,
			[MouseCursorType.SizeWE] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE,
			[MouseCursorType.SizeNS] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZENS,
			[MouseCursorType.SizeAll] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL,
			[MouseCursorType.No] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_NO,
			[MouseCursorType.Hand] = SDL_SystemCursor.SDL_SYSTEM_CURSOR_HAND,
		};
#endif

		private static MouseCursorType _mouseCursorType;
		private static AssetManager _defaultAssetManager;

		/// <summary>
		/// Gets or sets whether the mouse cursor type should be automatically updated based on widget properties.
		/// </summary>
		public static bool SetMouseCursorFromWidget { get; set; } = true;

		/// <summary>
		/// Gets or sets the current mouse cursor type displayed on screen.
		/// </summary>
		public static MouseCursorType MouseCursorType
		{
			get => _mouseCursorType;
			set
			{
				if (_mouseCursorType == value)
				{
					return;
				}

				_mouseCursorType = value;
#if MONOGAME
				MouseCursor mouseCursor;
				if (!_mouseCursors.TryGetValue(value, out mouseCursor))
				{
					throw new Exception($"Could not find mouse cursor {value}");
				}

				Mouse.SetCursor(mouseCursor);
#elif FNA
				SDL_SystemCursor mouseCursor;
				if (!_mouseCursors.TryGetValue(value, out mouseCursor))
				{
					throw new Exception($"Could not find mouse cursor {value}");
				}

				var mouseCursorPtr = GetSystemCursor(mouseCursor);
				SDL2.SDL.SDL_SetCursor(mouseCursorPtr);
#elif PLATFORM_AGNOSTIC
				Platform.SetMouseCursorType(value);
#endif
			}
		}

		/// <summary>
		/// Gets or sets the default mouse cursor type to use when no widget-specific cursor is set.
		/// </summary>
		public static MouseCursorType DefaultMouseCursorType { get; set; }

#if MONOGAME || FNA || STRIDE

		private static Game _game;

		/// <summary>
		/// Gets or sets the Game instance that Myra is running within.
		/// Must be set before using Myra in MonoGame, FNA, or Stride projects.
		/// </summary>
		public static Game Game
		{
			get
			{
				if (_game == null)
				{
					throw new Exception("MyraEnvironment.Game is null. Please, set it to the Game instance before using Myra.");
				}

				return _game;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (_game == value)
				{
					return;
				}

#if !STRIDE
				if (_game != null)
				{
					_game.Disposed -= GameOnDisposed;
				}
#endif

				_game = value;

#if !STRIDE
				if (_game != null)
				{
					_game.Disposed += GameOnDisposed;
				}
#endif
			}
		}

		/// <summary>
		/// Gets the graphics device used for rendering.
		/// </summary>
		public static GraphicsDevice GraphicsDevice
		{
			get => Game.GraphicsDevice;
		}
#else

		private static IMyraPlatform _platform;

		/// <summary>
		/// Gets or sets the platform abstraction layer used by Myra.
		/// Must be set before using Myra in platform-agnostic projects.
		/// </summary>
		public static IMyraPlatform Platform
		{
			get
			{
				if (_platform == null)
				{
					throw new Exception("MyraEnvironment.Platform is null. Please, set it before using Myra.");
				}

				return _platform;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_platform = value;
			}
		}
#endif

		/// <summary>
		/// Gets or sets the default asset manager used for loading resources.
		/// </summary>
		public static AssetManager DefaultAssetManager
		{
			get
			{
				if (_defaultAssetManager == null)
				{
					_defaultAssetManager = AssetManager.CreateFileAssetManager(PathUtils.ExecutingAssemblyDirectory);
				}

				return _defaultAssetManager;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));

				}
				_defaultAssetManager = value;
			}
		}

		/// <summary>
		/// Gets or sets whether to draw debug frames around all widgets.
		/// </summary>
		public static bool DrawWidgetsFrames { get; set; }

		/// <summary>
		/// Gets or sets whether to draw a debug frame around the widget with keyboard focus.
		/// </summary>
		public static bool DrawKeyboardFocusedWidgetFrame { get; set; }

		/// <summary>
		/// Gets or sets whether to draw a debug frame around the widget the mouse is hovering over.
		/// </summary>
		public static bool DrawMouseHoveredWidgetFrame { get; set; }

		/// <summary>
		/// Gets or sets whether to draw debug frames around text glyphs.
		/// </summary>
		public static bool DrawTextGlyphsFrames { get; set; }

		/// <summary>
		/// Gets or sets whether to disable scissor clipping for rendering.
		/// </summary>
		public static bool DisableClipping { get; set; }

		/// <summary>
		/// Gets or sets the function that provides the current mouse state.
		/// </summary>
		public static Func<MouseInfo> MouseInfoGetter { get; set; } = DefaultMouseInfoGetter;

		/// <summary>
		/// Gets or sets the function that fills an array with the current key down states.
		/// </summary>
		public static Action<bool[]> DownKeysGetter { get; set; } = DefaultDownKeysGetter;

		/// <summary>
		/// Gets or sets the interval in milliseconds for detecting double-clicks.
		/// </summary>
		public static int DoubleClickIntervalInMs { get; set; } = 500;

		/// <summary>
		/// Gets or sets the maximum pixel distance for clicks to be considered part of a double-click.
		/// </summary>
		public static int DoubleClickRadius { get; set; } = 2;

		/// <summary>
		/// Gets or sets the delay in milliseconds before a tooltip appears when hovering over a widget.
		/// </summary>
		public static int TooltipDelayInMs { get; set; } = 500;

		/// <summary>
		/// Gets or sets the offset in pixels from the mouse cursor to display a tooltip.
		/// </summary>
		public static Point TooltipOffset { get; set; } = new Point(0, 20);
		/// <summary>
		/// Gets or sets the function that creates a tooltip widget for a given widget.
		/// </summary>
		public static Func<Widget, Widget> TooltipCreator { get; set; } = w =>
		{
			var tooltip = new Label(null)
			{
				Text = w.Tooltip,
				Tag = w
			};

			tooltip.ApplyLabelStyle(Stylesheet.Current.TooltipStyle);

			return tooltip;
		};

		/// <summary>
		/// Gets or sets whether to apply smooth text rendering. Improves text quality when scaling but reduces performance.
		/// </summary>
		public static bool SmoothText { get; set; }

		/// <summary>
		/// Gets or sets whether to darken the background when modal dialogs are displayed.
		/// </summary>
		public static bool EnableModalDarkening { get; set; }

		/// <summary>
		/// Gets or sets the color used to darken the background when modal dialogs are displayed.
		/// </summary>
		public static Color DarkeningColor { get; set; } = new Color(0, 0, 0, 192);

		private static void GameOnDisposed(object sender, EventArgs eventArgs)
		{
			Reset();
		}

		/// <summary>
		/// Resets Myra environment, disposing of default assets and clearing the current stylesheet.
		/// </summary>
		public static void Reset()
		{
			DefaultAssets.Dispose();
			Stylesheet.Current = null;
		}

		/// <summary>
		/// Gets the version number of the Myra library.
		/// </summary>
		public static string Version
		{
			get
			{
				var assembly = typeof(MyraEnvironment).Assembly;
				var name = new AssemblyName(assembly.FullName);

				return name.Version.ToString();
			}
		}

		internal static string InternalClipboard;

		/// <summary>
		/// Gets the default mouse information including position and button states.
		/// This is the default implementation of MouseInfoGetter.
		/// </summary>
		/// <returns>A MouseInfo struct containing the current mouse state.</returns>
		public static MouseInfo DefaultMouseInfoGetter()
		{
#if MONOGAME || FNA
			var state = Mouse.GetState();

			var pos = new Point(state.X - GraphicsDevice.Viewport.X, state.Y - GraphicsDevice.Viewport.Y);

			return new MouseInfo
			{
				Position = pos,
				IsLeftButtonDown = Game.IsActive && state.LeftButton == ButtonState.Pressed,
				IsMiddleButtonDown = Game.IsActive && state.MiddleButton == ButtonState.Pressed,
				IsRightButtonDown = Game.IsActive && state.RightButton == ButtonState.Pressed,
				Wheel = state.ScrollWheelValue
			};
#elif STRIDE
			var input = Game.Input;

			var v = input.AbsoluteMousePosition;

			return new MouseInfo
			{
				Position = new Point((int)v.X, (int)v.Y),
				IsLeftButtonDown = input.IsMouseButtonDown(MouseButton.Left),
				IsMiddleButtonDown = input.IsMouseButtonDown(MouseButton.Middle),
				IsRightButtonDown = input.IsMouseButtonDown(MouseButton.Right),
				Wheel = input.MouseWheelDelta
			};
#else
			return Platform.GetMouseInfo();
#endif
		}

		/// <summary>
		/// Fills the provided array with the current state of all keyboard keys.
		/// This is the default implementation of DownKeysGetter.
		/// </summary>
		/// <param name="keys">An array of booleans where true indicates a key is pressed and false indicates it is released.</param>
		public static void DefaultDownKeysGetter(bool[] keys)
		{
#if MONOGAME || FNA
			var state = Keyboard.GetState();
			for (var i = 0; i < keys.Length; ++i)
			{
				keys[i] = state.IsKeyDown((Keys)i);
			}
#elif STRIDE
			var input = Game.Input;
			for (var i = 0; i < keys.Length; ++i)
			{
				keys[i] = input.IsKeyDown((Keys)i);
			}
#else
			Platform.SetKeysDown(keys);
#endif
		}
	}
}