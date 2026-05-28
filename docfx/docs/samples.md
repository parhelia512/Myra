# Myra Samples

Myra includes a collection of sample projects that demonstrate various features and capabilities of the framework. These samples serve as practical guides for implementing common UI patterns, custom widgets, and integration scenarios.

## Core Samples

### AllWidgets
Demonstrates all available built-in Myra widgets including buttons, checkboxes, radio buttons, text boxes, trees, grids, and dialogs. This is a comprehensive showcase of the widget library and how to use each control with event handling and styling.

### AndroidAllWidgets
A platform-specific version of the AllWidgets sample optimized for Android devices, demonstrating how to use Myra in mobile applications.

### DebugConsole
Shows how to implement an in-game debug console for testing and development. Useful for debugging applications during gameplay.

### PlatformAgnostic
Demonstrates how to use Myra in a platform-agnostic manner, showing compatibility across different target platforms without platform-specific code.

## Layout and Container Samples

### Layout2D
Covers 2D layout systems in Myra including positioning, alignment, and responsive design patterns.

### GridContainer
Demonstrates the use of grid-based layout containers for organizing UI elements in rows and columns.

### SplitPaneContainer
Shows how to implement resizable split panes for dividing UI space between multiple content areas.

### NonModalWindows
Demonstrates the creation and management of multiple non-modal windows that don't block interaction with other UI elements.

## Styling and Theming Samples

### CustomUIStylesheet
Shows how to create and apply custom UI stylesheets to customize the appearance of Myra widgets through code.

## Asset and Data Management Samples

### AssetManagement
Demonstrates how to manage and load assets (textures, fonts, models) within Myra applications.

### ObjectEditor
Shows how to create a property editor UI for editing object properties, useful for tools and debugging utilities.

### Notepad
A practical example of building a text editor application with file operations, including open, save, and edit functionality.

### Viewports
Demonstrates the use of viewport-based rendering for displaying content in constrained areas of the UI.

## Custom Widget Samples

### Arrow
**Demonstrates custom widget drawing through Myra RenderContext.** This sample implements a custom `Arrow` widget that draws arrows (left or right direction) using the RenderContext's `DrawLine` and `DrawPolygon` methods. It shows how to implement the `InternalRender` method to perform custom 2D graphics rendering.

### LogView
**Demonstrates custom widget containing a hierarchy of stock widgets.** This sample implements a `LogView` widget that extends `ScrollViewer` and contains a `VerticalStackPanel` filled with `Label` widgets. It shows how to compose custom widgets from existing built-in widgets and manage dynamic content with scrolling and animation.

### Scene3D
**Demonstrates custom widget doing custom rendering through BasicEffect.** This sample implements a `Scene3D` widget that performs 3D rendering using XNA Framework's `BasicEffect`. It handles mesh rendering, lighting, texture mapping, and manages 3D graphics state within the widget's render context, allowing seamless integration of 3D content into the Myra UI.

## Engine Integration Samples

### Using Myra in Stride Engine Tutorial
Shows how to integrate Myra UI into the Stride game engine, allowing you to use Myra for UI while leveraging Stride's 3D capabilities.

## Specialized Rendering Samples

### Silk.NET
Demonstrates using Myra with Silk.NET, a modern OpenGL/Vulkan binding for .NET, showing cross-platform graphics capabilities.

### Silk.NET.TrippyGL
An advanced example using Silk.NET with TrippyGL for specialized graphics rendering effects.

## Summary

These samples provide a complete overview of Myra's capabilities:
- **Built-in Widgets**: AllWidgets demonstrates the complete widget library
- **Layout Systems**: GridContainer, Layout2D, and SplitPaneContainer show organization strategies
- **Custom Rendering**: Arrow, Scene3D, and LogView demonstrate extending Myra with custom functionality
- **Integration**: Stride and Silk.NET samples show integration with other frameworks
- **Practical Applications**: Notepad and ObjectEditor show real-world use cases
- **Asset Management**: AssetManagement and Viewports demonstrate resource handling
