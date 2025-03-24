using Architect.Common.Models;
using Architect.Core.Input.Events;

public record TextChangedEvent(string NewText, string OldText) : InputEvent(Vector2.Zero);
