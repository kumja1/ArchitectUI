using Architect.Input.Events;
using Architect.UI.Widgets;
using Cosmos.Kernel.System.Keyboard;

namespace Architect.Input;

public sealed record InputHandler(
    Widget Widget,
    EventHandler<InputEvent> Handler,
    ConsoleKeyEx[]? Keys);