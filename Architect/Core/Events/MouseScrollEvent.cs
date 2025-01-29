using Architect.Common.Models;
using Architect.Core.Interfaces;

namespace Architect.Core.Events;

record MouseScrollEvent(Vector2 Position, int ScrollDelta) : IMouseEvent;