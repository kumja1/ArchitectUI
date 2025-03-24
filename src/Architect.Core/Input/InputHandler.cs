using Architect.Common.Interfaces;
using Architect.Core.Input.Events;
using Cosmos.System;

namespace Architect.Core.Input;

public sealed class InputHandler
{
    public IWidget Widget { get; }
    public EventHandler<InputEvent> Action { get; }
    public List<ConsoleKeyEx>? Keys { get; }

    public InputHandler(
        IWidget widget,
        EventHandler<InputEvent> action,
        List<ConsoleKeyEx>? keys = null
    )
    {
        Widget = widget;
        Action = action;
        Keys = keys;
    }
}
