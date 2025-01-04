using Architect.Common.Models;

namespace Architect.Core.Events;

record MouseScrollEvent(Vector2 Position, int ScrollDelta) : BaseEvent, IMouseEvent;