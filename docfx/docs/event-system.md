# Event System

Myra provides a comprehensive event system for handling user interactions and widget state changes.

### Event Arguments

All event arguments inherit from the base `MyraEventArgs` class, which provides:

```csharp
public class MyraEventArgs
{
    /// <summary>
    /// Gets the type of the input event.
    /// </summary>
    public InputEventType EventType { get; }

    /// <summary>
    /// Stops the propagation of the current event, preventing it from reaching parent widgets.
    /// </summary>
    public void StopPropagation();
}
```

## Subscribing to Events

### Basic Event Subscription

Subscribing to events in Myra uses standard C# event patterns:

```csharp
var button = new Button();
button.MouseEntered += OnButtonMouseEntered;
button.MouseLeft += OnButtonMouseLeft;

private void OnButtonMouseEntered(object sender, MyraEventArgs e)
{
    Console.WriteLine("Mouse entered button");
}

private void OnButtonMouseLeft(object sender, MyraEventArgs e)
{
    Console.WriteLine("Mouse left button");
}
```

### Using Lambda Expressions

For simple event handlers, lambda expressions are convenient:

```csharp
var button = new Button();
button.PressedChanged += (sender, e) => 
{
    Console.WriteLine($"Button pressed: {((Button)sender).IsPressed}");
};
```

### Cancellable Events

Some events can be cancelled to prevent their default behavior. Check if an event is cancellable by looking at its argument type:

```csharp
var window = new Window();
window.Closing += (sender, e) => 
{
    if (UnsavedChanges)
    {
        e.Cancel = true;
        Console.WriteLine("Close cancelled - unsaved changes");
    }
};
```

## Event Propagation

Myra supports two event propagation strategies defined by the `EventHandlingStrategy` enumeration:

### Event Capturing (Default)

In **event capturing**, events are captured at the top level and propagate down to child widgets:

```
Desktop (receives event first)
  ↓
Parent Container
  ↓
Button (receives event last)
```

This allows parent containers to intercept events before they reach children.

### Event Bubbling

In **event bubbling**, events start at the widget level and propagate up to parent widgets:

```
Button (receives event first)
  ↓
Parent Container
  ↓
Desktop (receives event last)
```

This allows parent containers to respond to child widget events.

### Configuring Event Handling Strategy

By default, Myra uses **event capturing** strategy. You can change the event handling strategy globally by setting the `MyraEnvironment.EventHandlingModel` property:

```csharp
using Myra;
using Myra.Events;

// Change to event bubbling (events bubble up from child to parent)
MyraEnvironment.EventHandlingModel = EventHandlingStrategy.EventBubbling;

// Or explicitly set to event capturing (events captured from top down)
MyraEnvironment.EventHandlingModel = EventHandlingStrategy.EventCapturing;
```

## Stopping Event Propagation

You can stop an event from propagating to parent widgets by calling `StopPropagation()`:

```csharp
var button = new Button();
button.MouseEntered += (sender, e) => 
{
    Console.WriteLine("Button received mouse entered event");
    e.StopPropagation();  // Parent widgets won't receive this event
};

var container = new Panel();
container.MouseEntered += (sender, e) => 
{
    // This won't be called if button stopped propagation
    Console.WriteLine("Container received mouse entered event");
};
container.Widgets.Add(button);
```

## Common Event Patterns

### Text Input Handling

Text widgets provide multiple text-related events:

```csharp
var textBox = new TextBox();

// Fired when text changes (programmatically or by user)
textBox.TextChanged += (sender, e) => 
{
    Console.WriteLine($"Text is now: {textBox.Text}");
};

// Fired before text is deleted - can be cancelled
textBox.TextDeleting += (sender, e) => 
{
    Console.WriteLine($"Text '{e.Value}' will be deleted at position {e.StartPosition}");
};

// Fired after text is deleted
textBox.TextDeleted += (sender, e) => 
{
    Console.WriteLine($"Text '{e.Value}' was deleted");
};

// Fired when user types a character
textBox.CharInput += (sender, e) => 
{
    Console.WriteLine($"Character input: {e.Data}");
};
```

### Selection Changes

Collection-based widgets fire selection change events:

```csharp
var listBox = new ListBox();

listBox.SelectedIndexChanged += (sender, e) => 
{
    Console.WriteLine($"Selected index changed");
    Console.WriteLine($"Selection: {listBox.SelectedIndex}");
};

listBox.HoverIndexChanged += (sender, e) => 
{
    Console.WriteLine($"Hover index changed to {listBox.HoverIndex}");
};
```

### Focus Management

Desktop and widgets provide focus-related events:

```csharp
var desktop = new Desktop();

desktop.WidgetLosingKeyboardFocus += (sender, e) => 
{
    Console.WriteLine($"Widget {e.Data} is losing focus");
};

desktop.WidgetGotKeyboardFocus += (sender, e) => 
{
    Console.WriteLine($"Widget {e.Data} got focus");
};
```

### Dialog Closing

Windows and dialogs provide closing events:

```csharp
var dialog = new Window();

dialog.Closing += (sender, e) => 
{
    if (!ValidateForm())
    {
        e.Cancel = true;
        ShowError("Please fix validation errors");
    }
};

dialog.Closed += (sender, e) => 
{
    Console.WriteLine("Dialog closed");
};
```

## Value Change Events

Value-changing events are particularly useful as they allow you to validate and modify values before they are applied:

```csharp
var slider = new Slider();

slider.ValueChanging += (sender, e) => 
{
    // Prevent value from exceeding a certain threshold
    if (e.NewValue > 100)
    {
        e.NewValue = 100;  // Clamp to maximum
    }
    
    // Or cancel the change entirely
    if (ShouldPreventChange(e))
    {
        e.Cancel = true;
    }
};

slider.ValueChanged += (sender, e) => 
{
    // Value change is committed - only fires if not cancelled
    Console.WriteLine($"Value changed from {e.OldValue} to {e.NewValue}");
};
```